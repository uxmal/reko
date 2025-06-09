#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using UserSignatureBuilder = Reko.Core.Hll.C.UserSignatureBuilder;
using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Reko.Core.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Reko.Services;
using System.IO;
using Reko.Core.Loading;

namespace Reko.Scanning
{
    /// <summary>
    /// Scans the binary, locating and creating procedures and basic blocks by following calls, jumps,
    /// and branches. Simple data type analysis is done as well: for instance, pointers to
    /// code are located, as are global data pointers.
    /// Regions of bytes that are not reachable by simple recursive disassembly of the binary 
    /// are optionally scanned using the ShingledScanner pass, which then may discover more 
    /// procedures.
    /// </summary>
    /// <remarks>
    /// Callers feed the scanner by calling EnqueueXXX methods before calling ProcessQueue().
    /// ProcessQueue() then processes the queues.
    /// </remarks>
    public class Scanner : ScannerBase, IScanner, IScannerServices, IRewriterHost
    {
        private const int PriorityEntryPoint = 5;
        private const int PriorityJumpTarget = 6;
        private const int PriorityGlobalData = 7;
        private const int PriorityVector = 4;
        private const int PriorityBlockPromote = 3;

        private static readonly TraceSwitch trace = new TraceSwitch("Scanner", "Traces the progress of the Scanner") { Level = TraceLevel.Info };

        protected IDecompilerEventListener eventListener;

        private readonly TypeLibrary metadata;
        private readonly SegmentMap segmentMap;
        private readonly ImageMap imageMap;
        private readonly IDynamicLinker dynamicLinker;
        private readonly BTreeDictionary<Address, BlockRange> blocks;
        private readonly Dictionary<Block, Address> blockStarts;
        private readonly Dictionary<Address, ImportReference> importReferences;
        private readonly HashSet<Procedure> visitedProcs;
        private readonly CancellationTokenSource? cancelSvc;
        private readonly HashSet<Address> scannedGlobalData = new();
        private readonly CommentInjector cinj;
        private PriorityQueue<WorkItem> procQueue;
        private ScanResults sr;
        
        public Scanner(
            Program program,
            TypeLibrary metadata,
            IDynamicLinker dynamicLinker,
            IServiceProvider services)
            : base(program, services.RequireService<IDecompilerEventListener>())
        {
            this.metadata = metadata;
            this.segmentMap = program.SegmentMap;
            this.dynamicLinker = dynamicLinker;
            this.Services = services;
            this.eventListener = services.RequireService<IDecompilerEventListener>();
            this.cancelSvc = services.GetService<CancellationTokenSource>();
            if (segmentMap is null)
                throw new InvalidOperationException("Program must have an segment map.");
            if (program.ImageMap is null)
            {
                program.ImageMap = segmentMap.CreateImageMap();
            }
            this.imageMap = program.ImageMap;
            this.procQueue = new PriorityQueue<WorkItem>();
            this.blocks = new BTreeDictionary<Address, BlockRange>();
            this.blockStarts = new Dictionary<Block, Address>();
            this.importReferences = program.ImportReferences;
            // The procedures in program.Procedures don't need to be revisited.
            this.visitedProcs = new HashSet<Procedure>(program.Procedures.Values);
            var annotations = program.User.Annotations.Concat(metadata.Annotations.Values);
            this.cinj = new CommentInjector(annotations);
            this.sr = new ScanResults
            {
                KnownProcedures = CollectKnownProcedures(program),
                KnownAddresses = program.ImageSymbols.ToDictionary(de => de.Key, de => de.Value),
                ICFG = new DiGraph<RtlBlock>(),
            };
        }

        private static HashSet<Address> CollectKnownProcedures(Program program)
        {
            return program.User.Procedures.Keys.ToHashSet();
        }

        public IServiceProvider Services { get; }

        public Constant? GlobalRegisterValue => this.Program.GlobalRegisterValue;

        private class BlockRange
        {
            /// <summary>
            /// Creates a block range. A (linear) address addr is considered part of the block if it
            /// satisifies the conditions Start <= addr < End.
            /// </summary>
            /// <param name="block"></param>
            /// <param name="start">Start address of the block</param>
            /// <param name="end">Address of the byte/word beyond the block's end.</param>
            public BlockRange(Block block, Address start, Address end)
            {
                this.Block = block ?? throw new ArgumentNullException(nameof(block));
                this.Start = start;
                this.End = end;
                Debug.Assert(start < end);
            }

            public Block Block { get; }
            public Address Start { get; set; }
            public Address End { get; set; }

            public override string ToString()
            {
                return string.Format("[{0:X}..{1:X}) - {2:X}", Start, End, End - Start);
            }
        }

        /// <summary>
        /// Adds a new basic block to the procedure <paramref name="proc"/>.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="proc"></param>
        /// <param name="blockName"></param>
        /// <returns></returns>
        public Block AddBlock(Address addr, Procedure proc, string blockName)
        {
            var b = new Block(proc, addr, blockName);
            if (Program.User.BlockLabels.TryGetValue(blockName, out var userLabel))
                b.UserLabel = userLabel;
            if (!blocks.TryGetUpperBound(addr, out var br))
            {
                if (!segmentMap.TryFindSegment(addr, out var segment))
                    throw new InvalidOperationException($"No segment for address {addr}.");
                var mem = segment.MemoryArea;
                blocks.Add(addr, new BlockRange(b, addr, segment.Address + segment.Size));
            }
            else
            {
                blocks.Add(addr, new BlockRange(b, addr, br.Start));
            }
            blockStarts.Add(b, addr);
            proc.ControlGraph.Blocks.Add(b);

            imageMap.AddItem(addr, new ImageMapBlock(addr) { Block = b });
            return b;
        }

        /// <summary>
        /// Verify that blockranges are not overlapping.
        /// </summary>
        /// <param name="blocks"></param>
        [Conditional("DEBUG")]
        private static void SanityCheck(SortedList<Address, BlockRange> blocks)
        {
            for (int i = 1; i < blocks.Count; ++i)
            {
                var prev = blocks.Values[i - 1];
                var cur = blocks.Values[i];
                Debug.Assert(prev.End <= cur.Start);
            }
        }

        /// <summary>
        /// Terminates the <paramref name="block"/> at 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="addr"></param>
        public void TerminateBlock(Block block, Address addr)
        {
            if (blocks.TryGetLowerBound(addr, out var range) && range.Start < addr)
                range.End = addr;
            imageMap.TerminateItem(addr);
        }

        public void TerminateAnyBlockAt(Address addr)
        {
            var block = FindContainingBlock(addr);
            if (block is not null)
            {
                TerminateBlock(block, addr);
            }
        }

        public bool TryRead(IProcessorArchitecture arch, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value)
        {
            return arch.Endianness.TryRead(this.Program.Memory, addr, dt, out value);
        }

        /// <summary>
        /// Creates a work item which will process code starting at the address
        /// <paramref name="addrStart"/>. The resulting block will belong to 
        /// the procedure <paramref name="proc"/>.
        /// </summary>
        /// <param name="addrStart"></param>
        /// <param name="proc"></param>
        /// <param name="stateOnEntry"></param>
        /// <returns></returns>
        public virtual BlockWorkitem CreateBlockWorkItem(
            Address addrStart,
            Procedure proc,
            ProcessorState stateOnEntry)
        {
            return new BlockWorkitem(
                this,
                Program,
                proc.Architecture,
                stateOnEntry,
                addrStart);
        }

        public IProcessorArchitecture GetArchitecture(string archMoniker)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            return Program.EnsureArchitecture(archMoniker, cfgSvc.GetArchitecture!);
        }

        public IEnumerable<RtlInstructionCluster> GetTrace(IProcessorArchitecture arch, Address addrStart, ProcessorState state, IStorageBinder binder)
        {
            if (!Program.TryCreateImageReader(arch, addrStart, out var rdr))
                return Array.Empty<RtlInstructionCluster>();
            return arch.CreateRewriter(
                rdr,
                state,
                binder,
                this);
        }

        public PromoteBlockWorkItem CreatePromoteWorkItem(Address addrStart, Block block, Procedure procNew)
        {
            return new PromoteBlockWorkItem(addrStart,
                this,
                Program,
                block,
                procNew);
        }

        public void EnqueueImageSymbol(ImageSymbol sym, bool isEntryPoint)
        {
            if (sym.ProcessorState is null)
                sym.ProcessorState = Program.Architecture.CreateProcessorState();
            procQueue.Enqueue(PriorityEntryPoint, new ImageSymbolWorkItem(this, Program, sym, isEntryPoint));
        }

        public void EnqueueUserProcedure(IProcessorArchitecture arch, Address addr, FunctionType sig, string? name)
        {
            if (Program.Procedures.ContainsKey(addr))
                return; // Already scanned. Do nothing.
            if (IsNoDecompiledProcedure(addr))
                return;
            var proc = Program.EnsureProcedure(arch, addr, name);
            proc.Signature = sig;
            procQueue.Enqueue(PriorityEntryPoint, new ProcedureWorkItem(this, arch, addr, proc.Name));
        }

        public Block? EnqueueJumpTarget(Address addrSrc, Address addrDest, Procedure proc, ProcessorState state)
        {
            Procedure procDest;
            Block? block = FindExactBlock(addrDest);
            if (block is null)
            {
                // Target wasn't a block before. Make sure it exists.
                block = FindContainingBlock(addrDest);
                if (block is not null)
                {
                    block = SplitBlock(block, addrDest);
                }
                else
                {
                    var label = Program.NamingPolicy.BlockName(addrDest);
                    block = AddBlock(addrDest, proc, label);
                }

                if (proc == block.Procedure)
                {
                    // Easy case: split a block in our own procedure.
                    var wi = CreateBlockWorkItem(addrDest, proc, state);
                    procQueue.Enqueue(PriorityJumpTarget, wi);
                }
                else if (IsBlockLinearProcedureExit(block))
                {
                    block = CloneBlockIntoOtherProcedure(block, proc);
                }
                else
                {
                    // We just created a block in a foreign procedure. 
                    blocks.Remove(addrDest);
                    block.Procedure.RemoveBlock(block);
                    procDest = Program.EnsureProcedure(block.Procedure.Architecture, addrDest, null);
                    EnqueueProcedure(block.Procedure.Architecture, addrDest);
                    var blockThunk = CreateCallRetThunk(addrSrc, proc, state, procDest);
                    var wi = CreatePromoteWorkItem(addrDest, block, procDest);
                    procQueue.Enqueue(PriorityBlockPromote, wi);
                    block = blockThunk;
                }
            }
            else if (block.Procedure != proc)
            {
                // Jumped to a block with a different procedure than the 
                // current one. Was the jump to the entry of an existing procedure?
                if (Program.Procedures.TryGetValue(addrDest, out procDest))
                {
                    if (procDest == proc)
                    {
                        proc.Signature.StackDelta = block.Procedure.Signature.StackDelta;
                        proc.Signature.FpuStackDelta = block.Procedure.Signature.FpuStackDelta;
                        var wi = CreatePromoteWorkItem(addrDest, block, procDest);
                        procQueue.Enqueue(PriorityBlockPromote, wi);
                    }
                    else
                    {
                        // We jumped to the entry of a different procedure.
                        block = CreateCallRetThunk(addrSrc, proc, state, procDest);
                    }
                }
                else
                {
                    // Jumped into the middle of another procedure. Is it worth
                    // promoting the destination block to a new procedure?
                    if (IsBlockLinearProcedureExit(block))
                    {
                        // No, just clone the block into the new procedure.
                        block = CloneBlockIntoOtherProcedure(block, proc);
                    }
                    else
                    {
                        // We jumped into a pre-existing block of another 
                        // procedure which was hairy enough that we need to 
                        // promote the block to a new procedure.
                        procDest = Program.EnsureProcedure(proc.Architecture, addrDest, null);
                        var blockNew = CreateCallRetThunk(addrSrc, proc, state, procDest);
                        EstablishInitialState(addrDest, Program.Architecture.CreateProcessorState(), procDest);
                        procDest.ControlGraph.AddEdge(procDest.EntryBlock, block);
                        InjectProcedureEntryInstructions(addrDest, procDest);
                        var wi = CreatePromoteWorkItem(addrDest, block, procDest);
                        procQueue.Enqueue(PriorityBlockPromote, wi);
                        return blockNew;
                    }
                }
            }
            return block;
        }

        public void EnqueueProcedure(IProcessorArchitecture arch, Address addr)
        {
            procQueue.Enqueue(PriorityEntryPoint, new ProcedureWorkItem(this, arch, addr, null));
        }

        public Address? EnqueueUserProcedure(IProcessorArchitecture arch, UserProcedure sp)
        {
            var de = EnsureUserProcedure(arch, sp);
            if (de is null)
                return null;
            procQueue.Enqueue(PriorityEntryPoint, new ProcedureWorkItem(this, arch, de.Value.Key, sp.Name));
            return de.Value.Key;
        }

        public void EnsureEntryPoint(ImageSymbol sym)
        {
            var proc = Program.EnsureProcedure(sym.Architecture, sym.Address!, sym.Name);
            if (sym.Signature is not null && !proc.Signature.ParametersValid)
            {
                var sser = Program.CreateProcedureSerializer();
                var sig = sser.Deserialize(sym.Signature, proc.Frame);
                proc.Signature = sig!;
            }
            Program.CallGraph.EntryPoints.Add(proc);
        }

        protected Procedure? EnsureProcedure(IProcessorArchitecture arch, Address addr, string name, FunctionType signature)
        {
            if (Program.Procedures.ContainsKey(addr))
                return null; // Already scanned. Do nothing.
            var proc = Program.EnsureProcedure(arch, addr, name);
            proc.Signature = signature;
            return proc;
        }

        protected KeyValuePair<Address, Procedure>? EnsureUserProcedure(IProcessorArchitecture arch, UserProcedure sp)
        {
            if (Program.Procedures.ContainsKey(sp.Address))
                return null; // Already scanned. Do nothing.
            if (!sp.Decompile)
                return null;
            var proc = Program.EnsureProcedure(arch, sp.Address, sp.Name);
            if (sp.Signature is not null)
            {
                var sser = Program.CreateProcedureSerializer();
                proc.Signature = sser.Deserialize(sp.Signature, proc.Frame)!;
            }
            if (sp.Characteristics is not null)
            {
                proc.Characteristics = sp.Characteristics;
            }
            return new KeyValuePair<Address, Procedure>(sp.Address, proc);
        }

        public static bool IsBlockLinearProcedureExit(Block block)
        {
            if (block.Statements.Count == 0)
                return false;
            return block.Statements[^1].Instruction is ReturnInstruction;
        }

        private Block? CloneBlockIntoOtherProcedure(Block block, Procedure proc)
        {
            trace.Verbose("Cloning {0} to {1}", block.Id, proc);
            var clonedBlock = new BlockCloner(block, proc, Program.CallGraph).Execute();
            return clonedBlock;
        }

        /// <summary>
        /// Creates a small basic block, consisting solely of a 'call' followed by a 'return'
        /// instruction.
        /// </summary>
        /// <remarks>
        /// This is done when encountering tail calls (i.e. jumps) from one 
        /// procedure into another.
        /// </remarks>
        /// <param name="addrFrom"></param>
        /// <param name="procOld"></param>
        /// <param name="procNew"></param>
        /// <returns></returns>
        public Block CreateCallRetThunk(Address addrFrom, Procedure procOld, ProcessorState state, Procedure procNew)
        {
            //$BUG: ReturnAddressOnStack property needs to be properly set, the
            // EvenOdd sample shows how this doesn't work currently. 
            var blockName = string.Format(
                "{0}_thunk_{1}",
                Program.NamingPolicy.BlockName(addrFrom),
                procNew.Name);
            var callRetThunkBlock = procOld.AddSyntheticBlock(
                addrFrom,
                blockName);
            if (Program.User.BlockLabels.TryGetValue(blockName, out var userLabel))
                callRetThunkBlock.UserLabel = userLabel;

            var call = CreateCall(procOld, addrFrom, procNew, state, new CallSite(0, 0));
            var stmLast = callRetThunkBlock.Statements.Add(addrFrom, call);
            Program.CallGraph.AddEdge(stmLast, procNew);

            callRetThunkBlock.Statements.Add(addrFrom, new ReturnInstruction());
            procOld.ControlGraph.AddEdge(callRetThunkBlock, procOld.ExitBlock);
            if (procNew.Frame.ReturnAddressSize.HasValue)
            {
                SetProcedureReturnAddressBytes(
                    procOld, procNew.Frame.ReturnAddressSize.Value, addrFrom);
            }
            SetProcedureStackDelta(procOld, procNew.Signature.StackDelta, addrFrom);
            return callRetThunkBlock;
        }

        private Instruction CreateCall(
            Procedure procCaller, 
            Address addrCall, 
            Procedure procNew, 
            ProcessorState state,
            CallSite site)
        {
            //$TODO: this is almost identical to BlockWorkItem.EmitCall.
            // In ScannerV2, these 2 fns should merge. The existing 
            // Scanner/ BlockWorkItem are too interdependent to 
            // easily reuse the function.

            var callee = new ProcedureConstant(Program.Platform.PointerType, procNew);
            var sig = procNew.Signature;
            var chr = procNew.Characteristics;

            var ab = procCaller.Architecture.CreateFrameApplicationBuilder(procCaller.Frame, site);
            if (VarargsFormatScanner.IsVariadicParserKnown(sig, chr))
            {
                // Attempt to perform varargs substitution. If it fails,
                // let the Analysis stage handle it.
                var vaScanner = new VarargsFormatScanner(Program, procCaller.Architecture, state, Services, eventListener);
                if (vaScanner.TryScan(addrCall, callee, sig, chr, ab, out var varargs))
                {
                    return vaScanner.BuildInstruction(callee, sig, varargs.Signature, chr, ab);
                }
                else
                {
                    return new CallInstruction(callee, site);
                }
            }
            else if (sig is not null && sig.ParametersValid)
            {
                return ab.CreateInstruction(callee, sig, chr);
            }
            else
            {
                return new CallInstruction(callee, site);
            }
        }

        /// <summary>
        /// Determines whether a block is a linear sequence of assignments followed by a return 
        /// statement.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public static bool IsLinearReturning(Block block)
        {
            for (;;)
            {
                if (block.Statements.Count == 0)
                    return false;
                var stmLast = block.Statements[^1];
                if (stmLast.Instruction is ReturnInstruction)
                    return true;
                if (stmLast.Instruction is not Assignment)
                    return false;
                if (block.Succ.Count == 0)
                    return false;
                block = block.Succ[0];
            }
        }

        public void ScanImageSymbol(ImageSymbol sym, bool isEntryPoint)
        {
            try
            {
                Address addr = sym.Address!;
                if (sym.NoDecompile || IsNoDecompiledProcedure(addr))
                    return;
                Procedure? proc = Program.EnsureProcedure(sym.Architecture, addr, sym.Name);
                if (visitedProcs.Contains(proc))
                    return; // Already scanned. Do nothing.

                if (sym.Signature is not null)
                {
                    var sser = Program.CreateProcedureSerializer();
                    proc.Signature = sser.Deserialize(sym.Signature, proc.Frame)!;
                }
                else if (!string.IsNullOrEmpty(sym.Name))
                {
                    var sProc = Program.Platform.SignatureFromName(sym.Name!);
                    if (sProc is not null)
                    {
                        var loader = Program.CreateTypeLibraryDeserializer();
                        var exp = loader.LoadExternalProcedure(sProc);
                        if (exp is not null)
                        {
                            proc.Name = exp!.Name;
                            proc.Signature = exp.Signature;
                            proc.EnclosingType = exp.EnclosingType;
                        }
                    }
                    else
                    {
                        proc.Name = sym.Name!;
                    }
                }

                var state = sym.ProcessorState ?? sym.Architecture.CreateProcessorState();
                var pb = ScanProcedure(sym.Architecture, sym.Address!, sym.Name, state);
                proc = pb as Procedure;
                if (isEntryPoint && proc is not null)
                {
                    Program.CallGraph.AddEntryPoint(proc);
                }
            }
            catch (AddressCorrelatedException aex)
            {
                Error(aex.Address, aex.Message);
            }
        }


        public ProcedureBase ScanProcedure(IProcessorArchitecture arch, Address addr, string? procedureName, ProcessorState state)
        {
            TerminateAnyBlockAt(addr);
            if (TryGetNoDecompiledProcedure(addr, out var ep))
                return ep;
            if (Program.InterceptedCalls.TryGetValue(addr, out ep))
                return ep;
            var trampoline = GetTrampoline(arch, addr);
            if (trampoline is not null)
            {
                return trampoline;
            }

            var imp = GetImportedProcedure(arch, addr, addr);
            if (imp is not null)
                return imp;
            Procedure proc = Program.EnsureProcedure(arch, addr, procedureName);
            if (visitedProcs.Contains(proc))
                return proc;

            visitedProcs.Add(proc);
            trace.Inform("Scanning procedure at {0}", addr);

            var st = state.Clone();
            EstablishInitialState(addr, st, proc);

            //$REFACTOR: make the stack explicit?
            var oldQueue = procQueue;
            procQueue = new PriorityQueue<WorkItem>();
            var block = EnqueueJumpTarget(addr, addr, proc, st);
            if (proc.EntryBlock.Succ.Count == 0)
            {
                proc.ControlGraph.AddEdge(proc.EntryBlock, block!);
            }
            ProcessQueue();

            procQueue = oldQueue;

            InjectProcedureEntryInstructions(addr, proc);
            var usb = new UserSignatureBuilder(Program);
            usb.BuildSignature(addr, proc);
            cinj.InjectComments(proc);
            this.eventListener.OnProcedureFound(Program, addr);
            return proc;
        }

        /// <summary>
        /// Before processing the body of a procedure, perform housekeeping tasks.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="state"></param>
        /// <param name="proc"></param>
        /// <returns></returns>
        private void EstablishInitialState(Address addr, ProcessorState st, Procedure proc)
        {
            st.InstructionPointer = addr;
            st.OnProcedureEntered(addr);
            var sp = proc.Frame.EnsureRegister(Program.Architecture.StackRegister);
            st.SetValue(sp, proc.Frame.FramePointer);
            SetAssumedRegisterValues(addr, st);
        }

        /// <summary>
        /// Inject statements into the entry block that establish the frame,
        /// and if the procedure has been given a valid signature already,
        /// copy the input arguments into their local counterparts.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="proc"></param>
        public void InjectProcedureEntryInstructions(Address addr, Procedure proc)
        {
            var bb = new StatementInjector(proc, proc.EntryBlock, addr);
            var sp = proc.Frame.EnsureRegister(proc.Architecture.StackRegister);
            bb.Assign(sp, proc.Frame.FramePointer);
            Program.Platform.InjectProcedureEntryStatements(proc, addr, bb);
        }

        public void EnqueueUserGlobalData(Address addr, DataType dt, string? name)
        {
            if (scannedGlobalData.Contains(addr))
                return;
            scannedGlobalData.Add(addr);
            if (!Program.TryCreateImageReader(addr, out var rdr))
                return;
            procQueue.Enqueue(PriorityGlobalData, new GlobalDataWorkItem(this, Program, addr, rdr, dt, name));
        }

        public Block? FindContainingBlock(Address address)
        {
            if (blocks.TryGetLowerBound(address, out var b) && address < b.End)
            {
                if (b.Block.Succ.Count == 0)
                    return b.Block;
                string succName = b.Block.Succ[0].Id;
                if (succName != b.Block.Id && succName.StartsWith(b.Block.Id) &&
                    !b.Block.Succ[0].IsSynthesized)
                    return b.Block.Succ[0];
                return b.Block;
            }
            else
                return null;
        }

        public Block? FindExactBlock(Address address)
        {
            if (blocks.TryGetValue(address, out var b))
                return b.Block;
            else
                return null;
        }

        /// <summary>
        /// Tries to determine if the instruction at <paramref name="addr"/> is 
        /// a trampoline instruction. If so, we return a call to the imported 
        /// function directly.
        /// procedure.
        /// </summary>
        /// <remarks>
        /// A trampoline is a procedure whose only contents is an indirect
        /// JUMP to a location that contains the address of an imported
        /// function. Because these trampolines may take on different
        /// appearances depending on the processor architecture, we have to 
        /// call out to the architecture to assist in matching them.
        /// </remarks>
        /// <param name="addr"></param>
        /// <returns>Null if there was no trampoline.</returns>
        public ProcedureBase? GetTrampoline(IProcessorArchitecture arch, Address addr)
        {
            if (!Program.TryCreateImageReader(arch, addr, out var rdr))
                return null;
            var rw = arch.CreateRewriter(rdr, arch.CreateProcessorState(), arch.CreateFrame(), this);
            var target = Program.Platform.GetTrampolineDestination(addr, rw.SelectMany(c => c.Instructions), this);
            return target;
        }

        public Expression? GetImport(Address addrImportThunk, Address addrInstruction)
        {
            if (importReferences.TryGetValue(addrImportThunk, out var impref))
            {
                var global = impref.ResolveImport(
                    dynamicLinker,
                    Program.Platform,
                    new ProgramAddress(Program, addrInstruction),
                    this.eventListener);
                return global;
            }
            return null;
        }

        /// <summary>
        /// If <paramref name="addrImportThunk"/> is the known address of an
        /// import thunk / trampoline, return the imported function as an
        /// ExternaProcedure. Otherwise, check to see if the call is an
        /// intercepted call.
        /// </summary>
        /// <param name="addrImportThunk"></param>
        /// <param name="addrInstruction">Used to display diagnostics.</param>
        /// <returns></returns>
        public ExternalProcedure? GetImportedProcedure(IProcessorArchitecture arch, Address addrImportThunk, Address addrInstruction)
        {
            if (importReferences.TryGetValue(addrImportThunk, out var impref))
            {
                var extProc = impref.ResolveImportedProcedure(
                    dynamicLinker,
                    Program.Platform,
                    new ProgramAddress(Program, addrInstruction),
                    this.eventListener);
                return extProc;
            }

            if (Program.InterceptedCalls.TryGetValue(addrImportThunk, out var ep))
                return ep;
            return GetInterceptedCall(arch, addrImportThunk);
        }

        /// <summary>
        /// This method is used to detect if a trampoline (call [foo] where foo: jmp bar)
        /// is jumping into the body of a procedure that was loaded with GetProcAddress or 
        /// the like.
        /// </summary>
        /// <param name="addrImportThunk"></param>
        /// <returns></returns>
        public ExternalProcedure? GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
        {
            if (!Program.TryCreateImageReader(arch, addrImportThunk, out var rdr))
                return null;
            //$REVIEW: WHOA! This is 32-bit code!
            if (!rdr.TryReadUInt32(out var uDest))
                return null;
            var addrDest = Address.Ptr32(uDest);
            Program.InterceptedCalls.TryGetValue(addrDest, out var ep);
            return ep;
        }

        /// <summary>
        /// Splits the given block at the specified address, yielding two 
        /// blocks. The first resulting block is the original block, now
        /// truncated, with a single out edge to the new block. The second 
        /// block receives the out edges of the first block.
        /// </summary>
        /// <param name="block">The block to split</param>
        /// <param name="addr">The address at which to split it.</param>
        /// <returns>The newly created, empty second block.</returns>
        public Block SplitBlock(Block blockToSplit, Address addr)
        {
            var graph = blockToSplit.Procedure.ControlGraph;
            var label = Program.NamingPolicy.BlockName(addr);
            var blockNew = AddBlock(addr, blockToSplit.Procedure, label);
            foreach (var succ in graph.Successors(blockToSplit))
            {
                graph.AddEdge(blockNew, succ);
            }
            foreach (var succ in graph.Successors(blockNew))
            {
                graph.RemoveEdge(blockToSplit, succ);
            }

            var stmsToMove = blockToSplit.Statements.FindAll(s => s.Address >= addr).ToArray();

            if (blockToSplit.Statements.Count > 0 && blockToSplit.Statements[^1].Address >= addr)
            {
                graph.AddEdge(blockToSplit, blockNew);
                blockToSplit.Statements.RemoveAll(s => s.Address >= addr);
            }
            blockNew.Statements.AddRange(stmsToMove);
            foreach (var stm in stmsToMove)
            {
                stm.Block = blockNew;
            }
            blocks[addr].End = blocks[blockStarts[blockToSplit]].End;
            blocks[blockStarts[blockToSplit]].End = addr;
			// Calling SanityCheck while scanning large binaries is very slow
			// resulting in a O(n^2) performance.
       		//     SanityCheck(blocks);
            return blockNew;
        }

        public virtual void ScanImage()
        {
            trace.Inform("= Loaded file ======");
            trace.Inform("{0} entry points", Program.EntryPoints.Count);
            trace.Inform("{0} symbols", Program.ImageSymbols.Count);
            trace.Inform("{0} executable bytes",
                Program.SegmentMap.Segments.Values
                    .Where(s => s.IsExecutable)
                    .Sum(s => s.MemoryArea.Length));

            // Find all blobs of data, and potentially pointers to code.
            ScanDataItems();

            // Find all reachable procedures.
            trace.Inform("= Recursive scan ======");
            var recTime = Time(ScanProceduresRecursively);
            trace.Inform("Found {0} procs", Program.Procedures.Count);
            trace.Inform("      {0} basic blocks", Program.Procedures.Values.Sum(p => p.ControlGraph.Blocks.Count));
            trace.Inform("      in {0} msec", (int) recTime.TotalMilliseconds);

            if (Program.User.Heuristics.Contains("shingle"))
            {
                // User has requested shingle scanning of unscanned areas.
                var shTime = Time(ShingleScanProcedures);
                trace.Inform("Found {0} procs", Program.Procedures.Count);
                trace.Inform("      {0} basic blocks", Program.Procedures.Values.Sum(p => p.ControlGraph.Blocks.Count));
                trace.Inform("      in {0} msec", (int) shTime.TotalMilliseconds);
            }
            // Scanner.DumpGraph(Program);
            RemoveRedundantGotos();
        }

        private TimeSpan Time(Action fn)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            fn();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        [Conditional("DEBUG")]
        private static void Dump(string title, IEnumerable<Block> blocks)
        {
            Debug.WriteLine(title);
            foreach (var block in blocks.OrderBy(b => b.Id))
            {
                Debug.Print("    {0}", block.Id);
            }
        }

        protected void ProcessQueue()
        {
            while (procQueue.TryDequeue(out var workitem))
            {
                if (eventListener.IsCanceled())
                {
                    break;
                }
                try
                {
                    workitem.Process();
                }
                catch (AddressCorrelatedException aex)
                {
                    Error(aex.Address, "{0}", aex.Message);
                }
                catch (Exception ex)
                {
                    Error(workitem.Address, "{0}", ex.Message);
                }
                if (cancelSvc is not null && cancelSvc.IsCancellationRequested)
                    break;
            }
        }

        public void SetAssumedRegisterValues(Address addr, ProcessorState st)
        {
            if (!Program.User.Procedures.TryGetValue(addr, out var userProc) ||
                userProc.Assume is null)
                return;
            foreach (var rv in userProc.Assume)
            {
                if (rv.Register is not null)
                {
                    var reg = Program.Architecture.GetRegister(rv.Register)!;
                    var val = rv.Value == "*"
                        ? InvalidConstant.Create(reg.DataType)
                        : Constant.Create(reg.DataType, Convert.ToUInt64(rv.Value, 16));
                    st.SetValue(reg, val);
                }
            }
        }

        public void SetProcedureStackDelta(
            Procedure proc, int stackDelta, Address address)
        {
            if (stackDelta == 0)
                return;
            if (proc.Signature.StackDelta != 0 && proc.Signature.StackDelta != stackDelta)
            {
                this.Warn(
                    address,
                    "Multiple different values of stack delta in procedure {0} when processing RET instruction; was {1} previously.",
                    proc.Name,
                    proc.Signature.StackDelta);
            }
            else
            {
                proc.Signature.StackDelta = stackDelta;
            }
        }

        public void SetProcedureReturnAddressBytes(Procedure proc, int returnAddressBytes, Address address)
        {
            if (proc.Frame.ReturnAddressSize.HasValue)
            {
                if (proc.Frame.ReturnAddressSize.Value != returnAddressBytes)
                {
                    this.Warn(
                        address,
                        string.Format(
                            "Procedure {1} previously had a return address of {2} bytes on the stack, " +
                            "but now seems to have a return address of {0} bytes on the stack.",
                            returnAddressBytes,
                            proc.Name,
                            proc.Frame.ReturnAddressSize.Value));
                }
            }
            else
            {
                proc.Frame.ReturnAddressSize = returnAddressBytes;
                proc.Signature.ReturnAddressOnStack = returnAddressBytes;
            }
        }

        private class StatementInjector : CodeEmitter
        {
            private readonly Address addr;
            private readonly Block block;
            private readonly Procedure proc;
            private int iStm;

            public StatementInjector(Procedure proc, Block block, Address addr)
            {
                this.proc = proc;
                this.addr = addr;
                this.block = block;
                this.iStm = 0;
            }

            public override Frame Frame { get { return proc.Frame; } }

            public override Statement Emit(Instruction instr)
            {
                var stm = block.Statements.Insert(iStm, addr, instr);
                ++iStm;
                return stm;
            }
        }

        private void RemoveRedundantGotos()
        {
            var blocks = Program.Procedures.Values.SelectMany(
                p => p.ControlGraph.Blocks);
            foreach(var block in blocks)
            {
                RemoveRedundantGotos(block);
            }
        }

        private static void RemoveRedundantGotos(Block block)
        {
            if (block.Statements.Count == 0)
                return;
            if (block.Statements[^1].Instruction is GotoInstruction)
                block.Statements.RemoveAt(block.Statements.Count-1);
        }

        /// <summary>
        /// Using user-specified globals and image metadata, scan all known 
        /// data. If we discover pointers to procedures, we add them to
        /// the list of known procedures. They are known because we reached
        /// them by tracing from known roots.
        /// </summary>
        /// <returns></returns>
        public ScanResults ScanDataItems()
        {
            var tlDeser = Program.CreateTypeLibraryDeserializer();
            var dataScanner = new DataScanner(Program, this.sr, this.eventListener);

            // Enqueue known data items, then process them. If we find code
            // pointers, they will be added to sr.KnownProcedures.

            foreach (var (addr, global) in metadata.GlobalsByAddress)
            {
                var dt = global.DataType!.Accept(tlDeser);
                this.imageMap.AddItemWithSize(
                    addr,
                    new ImageMapItem(addr)
                    {
                        Name = global.Name,
                        DataType = dt,
                        Size = (uint) dt.MeasureSize(),
                    });
                dataScanner.EnqueueUserGlobalData(addr, dt, global.Name);
                Program.User.Globals[addr] = global;
            }
            foreach (var global in Program.User.Globals)
            {
                var addr = global.Key;
                var dt = global.Value.DataType!.Accept(tlDeser);
                dataScanner.EnqueueUserGlobalData(addr, dt, global.Value.Name!);
            }
            
            foreach (var sym in Program.ImageSymbols.Values.Where(
                s => s.Type == SymbolType.Data && s.DataType is not null))
            {
                dataScanner.EnqueueUserGlobalData(sym.Address!, sym.DataType!, sym.Name!);
            }

            var mapSegments = Program.Platform.MemoryMap?.Segments;
            if (mapSegments is not null)
            {
                foreach (var global in mapSegments.SelectMany(s => s.Globals ?? []))
                {
                    if (!Program.Platform.Architecture.TryParseAddress(global.Address, out var addr))
                        continue;
                    var dt = global.DataType?.Accept(tlDeser)!;
                    dataScanner.EnqueueUserGlobalData(addr, dt, global.Name);
                }
            }

            dataScanner.ProcessQueue();
            foreach (var sym in dataScanner.Procedures.Values)
            {
                this.EnqueueImageSymbol(sym, false);
            }
            return sr;
        }

        /// <summary>
        /// Performs a recursive disassembly of the executable parts of the
        /// image, to find all potential basic blocks. We use symbols from the
        /// binary and user-specified procedures as "seeds" of a depth-first
        /// search.
        /// </summary>
        public void ScanProceduresRecursively()
        {
            var noDecompiles = Program.User.Procedures
                .Where(de => !de.Value.Decompile)
                .Select(de => de.Key)
                .ToHashSet();
            foreach (var de in Program.User.Procedures)
            {
                if (noDecompiles.Contains(de.Key))
                {
                    Program.EnsureUserProcedure(de.Key, de.Value.Name, false);
                }
                else
                {
                    var addr = EnqueueUserProcedure(Program.Architecture, de.Value);
                    if (addr is not null)
                    {
                        sr.KnownProcedures.Add(addr.Value);
                    }
                }
            }
            foreach (var de in metadata.Procedures)
            {
                var proc = EnsureProcedure(Program.Architecture, de.Key, de.Value.Name, de.Value.Signature);
                if (proc is not null)
                {
                    sr.KnownProcedures.Add(proc.EntryAddress);
                    if (metadata.Characteristics.TryGetValue(proc.Name, out var chr))
                    {
                        proc.Characteristics = chr;
                    }
                }
            }
            EnqueueImageSymbolProcedures(Program.EntryPoints.Values, noDecompiles);
            EnqueueImageSymbolProcedures(Program.ImageSymbols.Values
                .Where(sym => sym.Type == SymbolType.Procedure ||
                              sym.Type == SymbolType.ExternalProcedure),    // PLT entries.
                              noDecompiles);

            this.ProcessQueue();
        }

        private void EnqueueImageSymbolProcedures(IEnumerable<ImageSymbol> imageSymbols, HashSet<Address> noDecompiles)
        {
            foreach (var sym in imageSymbols)
            {
                if (!Program.SegmentMap.IsExecutableAddress(sym.Address))
                    continue;
                if (sym.NoDecompile || noDecompiles.Contains(sym.Address!))
                {
                    Program.EnsureUserProcedure(sym.Address!, sym.Name, false);
                }
                else
                {
                    Program.EnsureProcedure(sym.Architecture, sym.Address!, sym.Name);
                    EnqueueImageSymbol(sym, false);
                }
                sr.KnownProcedures.Add(sym.Address);
            }
        }


        /// <summary>
        /// Performs a shingle scan to recover procedures that weren't found 
        /// a recursive search. Assumed that Program.ImageMap may contain
        /// blocks of "known code" and only scans in the gaps between the
        /// blocks.
        /// </summary>
        public void ShingleScanProcedures()
        {
            var hsc = new ScannerInLinq(Services, Program, this, eventListener);
            sr = hsc.ScanImage(sr);
            if (sr is not null)
            {
                // The heuristic scanner will have detected any remaining 
                // procedures.

                var procs = sr.Procedures;

                // At this point, we have RtlProcedures and RtlBlocks.
                //$TODO: However, Reko hasn't had a chance to reconstitute constants yet, 
                // because that requires SSA, so we may be missing
                // opportunities to build and detect pointers. This typically happens in 
                // the type inference phase, when we both have constants and their types.
                // 
                // When this gets merged into analyis-development phase, fold 
                // Procedure construction into SSA construction.
                foreach (var rtlProc in procs.Where(FilterRtlProcedure)
                    .OrderBy(a => a.Address.ToLinear()))
                {
                    Dump(rtlProc);
                    var addrProc = rtlProc.Address;
                    TerminateAnyBlockAt(addrProc);
                    Procedure proc = Program.EnsureProcedure(Program.Architecture, addrProc, null);
                    EnqueueProcedure(Program.Architecture, addrProc);
                }
                ProcessQueue();
            }
        }

        [Conditional("DEBUG")]
        private void Dump(RtlProcedure rtlProc)
        {
            Debug.Print("== {0} ===============", rtlProc.Address);
            rtlProc.Dump();
        }


        /// <summary>
        /// Determines whether or not an RtlProcedure should be fed into the later stages of 
        /// the scanner.
        /// </summary>
        /// <param name="rtlProc"></param>
        /// <returns></returns>
        public bool FilterRtlProcedure(RtlProcedure rtlProc)
        {
            var addrRtlProc = rtlProc.Address;
            var entryBlock = rtlProc.Blocks.FirstOrDefault(b => b.Address == addrRtlProc);
            if (entryBlock is not null)
            {
                var trampoline = Program.Platform.GetTrampolineDestination(addrRtlProc, entryBlock.Instructions.SelectMany(c => c.Instructions), this);
                if (trampoline is not null)
                {
                    //$REVIEW: consider adding known trampolines to Program. Then, when code calls or 
                    // jumps to a trampoline, we don't have to call IPlatform.GetTrampolineDestination again.
                    return false;
                }
            }

            if (Program.User.Procedures.TryGetValue(addrRtlProc, out var sProc))
            {
                return sProc.Decompile;
            }
            return true;
        }

        public static void DumpGraph(Program program)
        {
            if (program.DisassemblyDirectory is null)
                return;
            var scanner_txt = Path.Combine(program.DisassemblyDirectory, "scanner.txt");
            using StreamWriter output = File.CreateText(scanner_txt);
            output.WriteLine("== V1 ===============");
            foreach (var proc in program.Procedures.Values)
            {
                var callers = string.Join(",", program.CallGraph.FindCallerStatements(proc)
                    .Select(s => s.Block?.Address.ToString() ?? "<null>"));
                output.WriteLine("{0}:{1}", proc.EntryAddress, proc.Characteristics.Terminates ? " <Terminates>" : "");
                output.WriteLine("    {0}", callers);
            }
            output.WriteLine("== Blocks ===============");

            foreach (var block in program.Procedures.Values
                .SelectMany(p => p.ControlGraph.Blocks)
                .Where(b => b != b.Procedure.EntryBlock)
                .Distinct()
                .OrderBy(b => b.Address))
            {
                output.WriteLine(block.Address);
                for (int i =0; i < block.Statements.Count-1; ++i)
                {
                    var stm = block.Statements[i];
                    if (stm.Instruction is CallInstruction)
                    {
                        var addrNext = block.Statements[i + 1].Address;
                        output.WriteLine("    {0}", block.Statements[i + 1].Address);
                        output.WriteLine(addrNext);
                    }
                }
                output.WriteLine("    {0}", string.Join(",", block.Succ
                    .Where(b => b != b.Procedure.ExitBlock)
                    .Select(b => b.Address)));
            }
            output.WriteLine();
        }

        public static void DumpGraph(Program program, ScanResultsV2 cfg)
        {
            if (program.DisassemblyDirectory is null)
                return;
            var scannerv2_txt = Path.Combine(program.DisassemblyDirectory, "scannerv2.txt");
            using StreamWriter output = File.CreateText(scannerv2_txt);
            output.WriteLine("== V2 ===============");
            foreach (var (a, rs) in cfg.ProcReturnStatus.OrderBy(e => e.Key))
            {
                output.WriteLine("{0}:{1}", a, rs != ReturnStatus.Returns? " <Terminates>" : "");
            }
            output.WriteLine("== Blocks ===============");
            foreach (var block in cfg.Blocks.Values
                .OrderBy(b => b.Address))
            {
                output.WriteLine(block.Address);
                IEnumerable<Address> succ = cfg.Successors.TryGetValue(block.Address, out var succs)
                    ? succs
                    : Array.Empty<Address>();
                output.WriteLine("    {0}", string.Join(",", succ));
            }
            output.WriteLine();
        }

        /*
         * State:
         *   regs:
         *      abstr_value
         *      reaching_defs
         *   stack:
         *   globals:
         *   
         * Scanner:
         *   q = stack of known code addresses
         *   while q
         *      b = blockat(q.addr)
         *      if (exists)
         *          split(q.addr)
         *      else 
         *          b = new block(q.addr)
         *      successors = process block(b)
         *      enqueue(successors)
         *      
         *  process block(b)
         *      while i = reader:
         *          if i.addr inside (other block)
         *              successors += other_block
         *              return;
         *          if i is assign
         *              b.add(i)
         *              state = eval(i, state)
         *          if i is goto
         *              if goto.label const
         *                  succs += goto.label
         *                  return
         *              if (calltable (state))
         *                  return
         *              if state(goto.label) = continuation
         *                  cur_fn.returns = true
         *                  emit return
         *                  return
         *          if i is call
         *              if (eval(target)  = { set of constants }
         *                  emit call(target)
         *                  callsite += [ set of constants ]
         *                  foreach (target) 
         *                      if (fn = functions.completed(target))
         *                          if fn.returns
         */
    }
}
