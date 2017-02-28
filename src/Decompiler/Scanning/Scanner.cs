#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Reko.Scanning
{
    /// <summary>
    /// Scans the binary, locating and creating procedures and basic blocks by following calls, jumps,
    /// and branches. Simple data type analysis is done as well: for instance, pointers to
    /// code are located, as are global data pointers.
    /// </summary>
    /// <remarks>
    /// Callers feed the scanner by calling EnqueueXXX methods before calling ProcessQueue(). ProcessQueue() then
    /// processes the queues.
    /// </remarks>
    public class ScannerOld : IScanner, IRewriterHost
    {
        private const int PriorityEntryPoint = 5;
        private const int PriorityJumpTarget = 6;
        private const int PriorityGlobalData = 7;
        private const int PriorityVector = 4;
        private const int PriorityBlockPromote = 3;

        private static TraceSwitch trace = new TraceSwitch("Scanner", "Traces the progress of the Scanner");

        protected Program program;
        protected DecompilerEventListener eventListener;

        private PriorityQueue<WorkItem> procQueue;
        private SegmentMap segmentMap;
        private ImageMap imageMap;
        private IImportResolver importResolver;
        private SortedList<Address, BlockRange> blocks;
        private Dictionary<Block, Address> blockStarts;
        private Dictionary<string, PseudoProcedure> pseudoProcs;
        private Dictionary<Address, ImportReference> importReferences;
        private HashSet<Procedure> visitedProcs;
        private Dictionary<Address, Procedure_v1> noDecompiledProcs;
        private CancellationTokenSource cancelSvc;
        private HashSet<Address> scannedGlobalData = new HashSet<Address>();

        public ScannerOld(
            Program program,
            IImportResolver importResolver,
            IServiceProvider services)
        {
            this.program = program;
            this.segmentMap = program.SegmentMap;
            this.importResolver = importResolver;
            this.Services = services;
            this.eventListener = services.RequireService<DecompilerEventListener>();
            this.cancelSvc = services.GetService<CancellationTokenSource>();
            if (segmentMap == null)
                throw new InvalidOperationException("Program must have an segment map.");
            if (program.ImageMap == null)
            {
                program.ImageMap = segmentMap.CreateImageMap();
            }
            this.imageMap = program.ImageMap;
            this.procQueue = new PriorityQueue<WorkItem>();
            this.blocks = new SortedList<Address, BlockRange>();
            this.blockStarts = new Dictionary<Block, Address>();
            this.pseudoProcs = program.PseudoProcedures;
            this.importReferences = program.ImportReferences;
            this.visitedProcs = new HashSet<Procedure>();
            this.noDecompiledProcs = new Dictionary<Address, Procedure_v1>();
        }

        public IServiceProvider Services { get; private set; }

        private class BlockRange
        {
            /// <summary>
            /// Creates a block range. A (linear) address addr is considered part of the block if it
            /// satisifies the conditions Start <= addr < End.
            /// </summary>
            /// <param name="block"></param>
            /// <param name="start">Linear start address of the block</param>
            /// <param name="end">Linear address of the byte/word beyond the block's end.</param>
            public BlockRange(Block block, ulong start, ulong end)
            {
                if (block == null)
                    throw new ArgumentNullException("block");
                this.Block = block;
                this.Start = start;
                this.End = end;
                Debug.Assert(start < end);
            }

            public Block Block { get; private set; }
            public ulong Start { get; set; }
            public ulong End { get; set; }
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
            Block b = new Block(proc, blockName) { Address = addr };
            var lastMem = segmentMap.Segments.Values.Last().MemoryArea;
            blocks.Add(addr, new BlockRange(b, addr.ToLinear(), lastMem.BaseAddress.ToLinear() + (uint)lastMem.Length));
            blockStarts.Add(b, addr);
            proc.ControlGraph.Blocks.Add(b);

            imageMap.AddItem(addr, new ImageMapBlock { Block = b });
            return b;
        }

        /// <summary>
        /// Terminates the <paramref name="block"/> at 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="addr"></param>
        public void TerminateBlock(Block block, Address addr)
        {
            BlockRange range;
            if (blocks.TryGetLowerBound(addr, out range) && range.Start < addr.ToLinear())
                range.End = addr.ToLinear();
            imageMap.TerminateItem(addr);
        }

        private void TerminateAnyBlockAt(Address addr)
        {
            var block = FindContainingBlock(addr);
            if (block != null)
            {
                TerminateBlock(block, addr);
            }
        }

        public void Warn(Address addr, string message)
        {
            eventListener.Warn(eventListener.CreateAddressNavigator(program, addr), message);
        }

        public void Warn(Address addr, string message, params object[] args)
        {
            eventListener.Warn(eventListener.CreateAddressNavigator(program, addr), message, args);
        }

        public void Error(Address addr, string message, params object[] args)
        {
            eventListener.Error(eventListener.CreateAddressNavigator(program, addr), message, args);
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
                program,
                stateOnEntry,
                addrStart);
        }

        public IEnumerable<RtlInstructionCluster> GetTrace(Address addrStart, ProcessorState state, IStorageBinder frame)
        {
            return program.Architecture.CreateRewriter(
                program.CreateImageReader(addrStart),
                state,
                frame,
                this);
        }

        public PromoteBlockWorkItem CreatePromoteWorkItem(Address addrStart, Block block, Procedure procNew)
        {
            return new PromoteBlockWorkItem(addrStart)
            {
                Scanner = this,
                Program = program,
                Block = block,
                ProcNew = procNew,
            };
        }

        public void EnqueueImageSymbol(ImageSymbol sym, bool isEntryPoint)
        {
            if (sym.ProcessorState == null)
                sym.ProcessorState = program.Architecture.CreateProcessorState();
            procQueue.Enqueue(PriorityEntryPoint, new ImageSymbolWorkItem(this, program, sym, isEntryPoint));
        }

        public void EnqueueUserProcedure(Address addr, FunctionType sig, string name)
        {
            if (program.Procedures.ContainsKey(addr))
                return; // Already scanned. Do nothing.
            if (IsNoDecompiledProcedure(addr))
                return;
            var proc = EnsureProcedure(addr, name);
            proc.Signature = (FunctionType)sig.Clone();
            procQueue.Enqueue(PriorityEntryPoint, new ProcedureWorkItem(this, program, addr, proc.Name));
        }

        public Block EnqueueJumpTarget(Address addrSrc, Address addrDest, Procedure proc, ProcessorState state)
        {
            Procedure procDest;
            Block block = FindExactBlock(addrDest);
            if (block == null)
            {
                // Target wasn't a block before. Make sure it exists.
                block = FindContainingBlock(addrDest);
                if (block != null)
                {
                    block = SplitBlock(block, addrDest);
                }
                else
                {
                    block = AddBlock(addrDest, proc, Block.GenerateName(addrDest));
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
                    procDest = (Procedure)ScanProcedure(addrDest, null, state);
                    var blockThunk = CreateCallRetThunk(addrSrc, proc, procDest);
                    var wi = CreatePromoteWorkItem(addrDest, block, procDest);
                    procQueue.Enqueue(PriorityBlockPromote, wi);
                    block = blockThunk;
                }
            }
            else if (block.Procedure != proc)
            {
                // Jumped to a block with a different procedure than the 
                // current one. Was the jump to the entry of an existing procedure?
                if (program.Procedures.TryGetValue(addrDest, out procDest))
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
                        block = CreateCallRetThunk(addrSrc, proc, procDest);
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
                        procDest = EnsureProcedure(addrDest, null);
                        var blockNew = CreateCallRetThunk(addrSrc, proc, procDest);
                        EstablishInitialState(addrDest, program.Architecture.CreateProcessorState(), procDest);
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

        public void EnqueueProcedure(Address addr)
        {
            procQueue.Enqueue(PriorityEntryPoint, new ProcedureWorkItem(this, program, addr, null));
        }

        public void EnqueueUserProcedure(Procedure_v1 sp)
        {
            var de = EnsureUserProcedure(sp);
            if (de == null)
                return;
            procQueue.Enqueue(PriorityEntryPoint, new ProcedureWorkItem(this, program, de.Value.Key, sp.Name));
        }

        public void EnsureEntryPoint(ImageSymbol ep)
        {
            var proc = EnsureProcedure(ep.Address, ep.Name);
            if (ep.Signature != null)
            {
                var sser = program.CreateProcedureSerializer();
                proc.Signature = sser.Deserialize(ep.Signature, proc.Frame);
            }
            program.CallGraph.EntryPoints.Add(proc);
        }

        protected KeyValuePair<Address, Procedure>? EnsureUserProcedure(Procedure_v1 sp)
        {
            Address addr;
            if (!program.Architecture.TryParseAddress(sp.Address, out addr))
                return null;
            Procedure proc;
            if (program.Procedures.TryGetValue(addr, out proc))
                return null; // Already scanned. Do nothing.
            if (!sp.Decompile)
                return null;
            proc = EnsureProcedure(addr, sp.Name);
            if (sp.Signature != null)
            {
                var sser = program.CreateProcedureSerializer();
                proc.Signature = sser.Deserialize(sp.Signature, proc.Frame);
            }
            if (sp.Characteristics != null)
            {
                proc.Characteristics = sp.Characteristics;
            }
            return new KeyValuePair<Address, Procedure>(addr, proc);
        }

        public bool IsBlockLinearProcedureExit(Block block)
        {
            if (block.Statements.Count == 0)
                return false;
            return block.Statements.Last.Instruction is ReturnInstruction;
        }

        private Block CloneBlockIntoOtherProcedure(Block block, Procedure proc)
        {
            Debug.Print("Cloning {0} to {1}", block.Name, proc);
            var clonedBlock = new BlockCloner(block, proc, program.CallGraph).Execute();
            //ReplaceSuccessorsWith(pred, block, clonedBlock);
            //pred.Procedure.ControlGraph.Blocks.Remove(block);
            return clonedBlock;
        }

        /// <summary>
        /// Creates a small basic block, consisting solely of a 'call' followed by a 'return'
        /// instruction. 
        /// </summary>
        /// <param name="addrFrom"></param>
        /// <param name="procOld"></param>
        /// <param name="procNew"></param>
        /// <returns></returns>
        public Block CreateCallRetThunk(Address addrFrom, Procedure procOld, Procedure procNew)
        {
            //$BUG: ReturnAddressOnStack property needs to be properly set, the
            // EvenOdd sample shows how this doesn't work currently. 
            var blockName = string.Format(
                "{0}_thunk_{1}",
                Block.GenerateName(addrFrom),
                procNew.Name);
            var callRetThunkBlock = procOld.AddBlock(blockName);
            callRetThunkBlock.IsSynthesized = true;

            var linFrom = addrFrom.ToLinear();
            callRetThunkBlock.Statements.Add(
                addrFrom.ToLinear(),
                new CallInstruction(
                    new ProcedureConstant(program.Platform.PointerType, procNew),
                    new CallSite(
                        procNew.Signature.ReturnAddressOnStack,
                        0)));
            program.CallGraph.AddEdge(callRetThunkBlock.Statements.Last, procNew);

            callRetThunkBlock.Statements.Add(linFrom, new ReturnInstruction());
            procOld.ControlGraph.AddEdge(callRetThunkBlock, procOld.ExitBlock);
            SetProcedureReturnAddressBytes(procOld, procNew.Frame.ReturnAddressSize, addrFrom);
            return callRetThunkBlock;
        }

        /// <summary>
        /// Determines whether a block is a linear sequence of assignments followed by a return 
        /// statement.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public bool IsLinearReturning(Block block)
        {
            for (;;)
            {
                if (block.Statements.Count == 0)
                    return false;
                if (block.Statements.Last.Instruction is ReturnInstruction)
                    return true;
                if (!(block.Statements.Last.Instruction is Assignment))
                    return false;
                if (block.Succ.Count == 0)
                    return false;
                block = block.Succ[0];
            }
        }

        public void ScanImageSymbol(Program program, ImageSymbol sym, bool isEntryPoint)
        {
            try
            {
                Address addr = sym.Address;
                Procedure proc;
                if (program.Procedures.TryGetValue(addr, out proc))
                    return; // Already scanned. Do nothing.
                if (sym.NoDecompile || IsNoDecompiledProcedure(addr))
                    return;

                proc = EnsureProcedure(addr, sym.Name);
                if (sym.Signature != null)
                {
                    var sser = program.CreateProcedureSerializer();
                    proc.Signature = sser.Deserialize(sym.Signature, proc.Frame);
                }
                else if (sym.Name != null)
                {
                    var exp = program.Platform.SignatureFromName(sym.Name);
                    if (exp != null)
                    {
                        proc.Name = exp.Name;
                        proc.Signature = exp.Signature;
                        proc.EnclosingType = exp.EnclosingType;
                    }
                    else
                    {
                        proc.Name = sym.Name;
                    }
                }

                //if (sp.Characteristics != null)
                //{
                //    proc.Characteristics = sp.Characteristics;
                //}

                var pb = ScanProcedure(sym.Address, sym.Name, sym.ProcessorState);
                proc = pb as Procedure;
                if (isEntryPoint && proc != null)
                {
                    program.CallGraph.AddEntryPoint(proc);
                }
            }
            catch (AddressCorrelatedException aex)
            {
                Error(aex.Address, aex.Message);
            }
        }


        /// <summary>
        /// Performs a scan of the blocks that constitute a procedure named <paramref name="procedureName"/>
        /// </summary>
        /// <param name="addr">Address of the code from which we will start scanning.</param>
        /// <param name="procedureName"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ProcedureBase ScanProcedure(Address addr, string procedureName, ProcessorState state)
        {
            TerminateAnyBlockAt(addr);
            ExternalProcedure ep;
            if (TryGetNoDecompiledProcedure(addr, out ep))
                return ep;
            if (program.InterceptedCalls.TryGetValue(addr, out ep))
                return ep;
            var trampoline = GetTrampoline(addr);
            if (trampoline != null)
                return trampoline;

            var imp = GetImportedProcedure(addr, addr);
            if (imp != null)
                return imp;
            Procedure proc = EnsureProcedure(addr, procedureName);
            if (visitedProcs.Contains(proc))
                return proc;

            visitedProcs.Add(proc);
            Debug.WriteLineIf(trace.TraceInfo, string.Format("Scanning procedure at {0}", addr));

            var st = state.Clone();
            EstablishInitialState(addr, st, proc);

            //$REFACTOR: make the stack explicit?
            var oldQueue = procQueue;
            procQueue = new PriorityQueue<WorkItem>();
            var block = EnqueueJumpTarget(addr, addr, proc, st);
            proc.ControlGraph.AddEdge(proc.EntryBlock, block);
            ProcessQueue();

            procQueue = oldQueue;

            InjectProcedureEntryInstructions(addr, proc);
            var usb = new UserSignatureBuilder(program);
            usb.BuildSignature(addr, proc);
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
            st.SetInstructionPointer(addr);
            st.OnProcedureEntered();
            var sp = proc.Frame.EnsureRegister(program.Architecture.StackRegister);
            st.SetValue((Identifier)sp, (Expression)proc.Frame.FramePointer);
            SetAssumedRegisterValues(addr, st);
        }

        /// <summary>
        /// Inject statements into the starting block that establish the frame,
        /// and if the procedure has been given a valid signature already,
        /// copy the input arguments into their local counterparts.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="proc"></param>
        /// <param name="sp"></param>
        public void InjectProcedureEntryInstructions(Address addr, Procedure proc)
        {
            var bb = new StatementInjector(proc, proc.EntryBlock.Succ[0], addr);
            var sp = proc.Frame.EnsureRegister(program.Architecture.StackRegister);
            bb.Assign((Identifier)sp, (Expression)proc.Frame.FramePointer);
            program.Platform.InjectProcedureEntryStatements(proc, addr, bb);
        }

        private bool TryGetNoDecompiledProcedure(Address addr, out Procedure_v1 sProc)
        {
            if (!program.User.Procedures.TryGetValue(addr, out sProc) ||
                sProc.Decompile)
            {
                sProc = null;
                return false;
            }
            return true;
        }

        private bool IsNoDecompiledProcedure(Address addr)
        {
            Procedure_v1 sProc;
            return TryGetNoDecompiledProcedure(addr, out sProc);
        }

        private bool TryGetNoDecompiledParsedProcedure(Address addr, out Procedure_v1 parsedProc)
        {
            Procedure_v1 sProc;
            if (!TryGetNoDecompiledProcedure(addr, out sProc))
            {
                parsedProc = null;
                return false;
            }
            if (noDecompiledProcs.TryGetValue(addr, out parsedProc))
                return true;
            parsedProc = new Procedure_v1()
            {
                Name = sProc.Name,
            };
            noDecompiledProcs[addr] = parsedProc;
            if (string.IsNullOrEmpty(sProc.CSignature))
            {
                Warn(addr, "The user-defined procedure at address {0} did not have a signature.", addr);
                return true;
            }
            var usb = new UserSignatureBuilder(program);
            var procDecl = usb.ParseFunctionDeclaration(sProc.CSignature);
            if (procDecl == null)
            {
                Warn(addr, "The user-defined procedure signature at address {0} could not be parsed.", addr);
                return true;
            }
            parsedProc.Signature = procDecl.Signature;
            return true;
        }

        private bool TryGetNoDecompiledProcedure(Address addr, out ExternalProcedure ep)
        {
            Procedure_v1 sProc;
            if (!TryGetNoDecompiledParsedProcedure(addr, out sProc))
            {
                ep = null;
                return false;
            }
            var ser = program.CreateProcedureSerializer();
            var sig = ser.Deserialize(
                sProc.Signature,
                program.Architecture.CreateFrame());
            ep = new ExternalProcedure(sProc.Name, sig);
            return true;
        }

        public void EnqueueUserGlobalData(Address addr, DataType dt, string name)
        {
            if (scannedGlobalData.Contains(addr))
                return;
            scannedGlobalData.Add(addr);
            procQueue.Enqueue(PriorityGlobalData, new GlobalDataWorkItem(this, program, addr, dt, name));
        }

        public Block FindContainingBlock(Address address)
        {
            BlockRange b;
            if (blocks.TryGetLowerBound(address, out b) && address.ToLinear() < b.End)
            {
                if (b.Block.Succ.Count == 0)
                    return b.Block;
                string succName = b.Block.Succ[0].Name;
                if (succName != b.Block.Name && succName.StartsWith(b.Block.Name) &&
                    !b.Block.Succ[0].IsSynthesized)
                    return b.Block.Succ[0];
                return b.Block;
            }
            else
                return null;
        }

        public Block FindExactBlock(Address address)
        {
            BlockRange b;
            if (blocks.TryGetValue(address, out b))
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
        public ProcedureBase GetTrampoline(Address addr)
        {
            var rdr = program.CreateImageReader(addr);
            var target = program.Platform.GetTrampolineDestination(rdr, this);
            return target;
        }

        public Expression GetImport(Address addrImportThunk, Address addrInstruction)
        {
            ImportReference impref;
            if (importReferences.TryGetValue(addrImportThunk, out impref))
            {
                var global = impref.ResolveImport(
                    importResolver,
                    program.Platform,
                    new AddressContext(program, addrInstruction, this.eventListener));
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
        public ExternalProcedure GetImportedProcedure(Address addrImportThunk, Address addrInstruction)
        {
            ImportReference impref;
            if (importReferences.TryGetValue(addrImportThunk, out impref))
            {
                var extProc = impref.ResolveImportedProcedure(
                    importResolver,
                    program.Platform,
                    new AddressContext(program, addrInstruction, this.eventListener));
                return extProc;
            }

            ExternalProcedure ep;
            if (program.InterceptedCalls.TryGetValue(addrImportThunk, out ep))
                return ep;
            return GetInterceptedCall(addrImportThunk);
        }

        /// <summary>
        /// This method is used to detect if a trampoline (call [foo] where foo: jmp bar)
        /// is jumping into the body of a procedure that was loaded with GetProcAddress or 
        /// the like.
        /// </summary>
        /// <param name="addrImportThunk"></param>
        /// <returns></returns>
        public ExternalProcedure GetInterceptedCall(Address addrImportThunk)
        {
            ExternalProcedure ep;
            if (!segmentMap.IsValidAddress(addrImportThunk))
                return null;
            var rdr = program.CreateImageReader(addrImportThunk);
            uint uDest;
            if (!rdr.TryReadUInt32(out uDest))
                return null;
            var addrDest = Address.Ptr32(uDest);
            program.InterceptedCalls.TryGetValue(addrDest, out ep);
            return ep;
        }

        /// <summary>
        /// Splits the given block at the specified address, yielding two blocks. The first block is the original block,
        /// now truncated, with a single out edge to the new block. The second block receives the out edges of the first block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="addr"></param>
        /// <returns>The newly created, empty second block</returns>
        public Block SplitBlock(Block blockToSplit, Address addr)
        {
            var graph = blockToSplit.Procedure.ControlGraph;
            var blockNew = AddBlock(addr, blockToSplit.Procedure, Block.GenerateName(addr));
            foreach (var succ in graph.Successors(blockToSplit))
            {
                graph.AddEdge(blockNew, succ);
            }
            foreach (var succ in graph.Successors(blockNew))
            {
                graph.RemoveEdge(blockToSplit, succ);
            }

            var linAddr = addr.ToLinear();
            var stmsToMove = blockToSplit.Statements.FindAll(s => s.LinearAddress >= linAddr).ToArray();

            if (blockToSplit.Statements.Count > 0 && blockToSplit.Statements.Last.LinearAddress >= linAddr)
            {
                graph.AddEdge(blockToSplit, blockNew);
                blockToSplit.Statements.RemoveAll(s => s.LinearAddress >= linAddr);
            }
            blockNew.Statements.AddRange(stmsToMove);
            foreach (var stm in stmsToMove)
            {
                stm.Block = blockNew;
            }
            blocks[blockStarts[blockToSplit]].End = linAddr;
            return blockNew;
        }

        /// <summary>
        /// Performs the work of scanning the image and resolving any 
        /// cross procedure jumps after the scan is done.
        /// </summary>
        public virtual void ScanImage()
        {
            var tlDeser = program.CreateTypeLibraryDeserializer();
            foreach (var global in program.User.Globals)
            {
                var addr = global.Key;
                var dt = global.Value.DataType.Accept(tlDeser);
                EnqueueUserGlobalData(addr, dt, global.Value.Name);
            }
            foreach (ImageSymbol ep in program.EntryPoints.Values)
            {
                EnqueueImageSymbol(ep, true);
            }
            foreach (Procedure_v1 up in program.User.Procedures.Values)
            {
                EnqueueUserProcedure(up);
            }
            foreach (ImageSymbol sym in program.ImageSymbols.Values.Where(s => s.Type == SymbolType.Procedure))
            {
                if (sym.NoDecompile)
                    program.EnsureUserProcedure(sym.Address, sym.Name, false);
                else
                    EnqueueImageSymbol(sym, false);
            }

            //var hsc = new HeuristicScanner(Services, program, this, eventListener);
            //var sr = hsc.ScanImage();

            ProcessQueue();
        }

        /// <summary>
        /// Uses the HeuristicScanner to try to located code heuristically.
        /// </summary>
        public void ScanImageHeuristically()
        {
            var heuristicScanner = new HeuristicScanner(Services, program, this, eventListener);
            heuristicScanner.ScanImageHeuristically();
        }

        [Conditional("DEBUG")]
        private void Dump(string title, IEnumerable<Block> blocks)
        {
            Debug.WriteLine(title);
            foreach (var block in blocks.OrderBy(b => b.Name))
            {
                Debug.Print("    {0}", block.Name);
            }
        }

        protected Procedure EnsureProcedure(Address addr, string procedureName)
        {
            Procedure proc;
            if (program.Procedures.TryGetValue(addr, out proc))
                return proc;
            ImageSymbol sym;
            if (procedureName == null && program.ImageSymbols.TryGetValue(addr, out sym))
            {
                procedureName = sym.Name;
            }
            proc = Procedure.Create(procedureName, addr, program.Architecture.CreateFrame());
            program.Procedures.Add(addr, proc);
            program.CallGraph.AddProcedure(proc);
            return proc;
        }

        protected void ProcessQueue()
        {
            while (procQueue.Count > 0)
            {
                if (eventListener.IsCanceled())
                {
                    break;
                }
                var workitem = procQueue.Dequeue();
                try
                {
                    workitem.Process();
                }
                catch (AddressCorrelatedException aex)
                {
                    Error(aex.Address, aex.Message);
                }
                catch (Exception ex)
                {
                    Error(workitem.Address, ex.Message);
                }
                if (cancelSvc != null && cancelSvc.IsCancellationRequested)
                    break;
            }
        }

        //$REVIEW: can't the callers call Program.EnsurePse
        public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
        {
            return program.EnsurePseudoProcedure(name, returnType, arity);
        }

        public Expression PseudoProcedure(string name, DataType returnType, params Expression[] args)
        {
            var ppp = program.EnsurePseudoProcedure(name, returnType, args.Length);
            return new Application(
                new ProcedureConstant(program.Architecture.PointerType, ppp),
                returnType,
                args);
        }

        public Expression PseudoProcedure(string name, ProcedureCharacteristics c, DataType returnType, params Expression[] args)
        {
            var ppp = program.EnsurePseudoProcedure(name, returnType, args.Length);
            ppp.Characteristics = c;
            return new Application(
                new ProcedureConstant(program.Architecture.PointerType, ppp),
                returnType,
                args);
        }

        public void SetAssumedRegisterValues(Address addr, ProcessorState st)
        {
            Procedure_v1 userProc;
            if (!program.User.Procedures.TryGetValue(addr, out userProc) ||
                userProc.Assume == null)
                return;
            foreach (var rv in userProc.Assume)
            {
                var reg = program.Architecture.GetRegister(rv.Register);
                var val = rv.Value == "*"
                    ? Constant.Invalid
                    : Constant.Create(reg.DataType, Convert.ToUInt64(rv.Value, 16));
                st.SetValue(reg, val);
            }
        }

        public void SetProcedureReturnAddressBytes(Procedure proc, int returnAddressBytes, Address address)
        {
            if (proc.Frame.ReturnAddressKnown)
            {
                if (proc.Frame.ReturnAddressSize != returnAddressBytes)
                {
                    this.Warn(
                        address,
                        string.Format(
                            "Procedure {1} previously had a return address of {2} bytes on the stack, " +
                            "but now seems to have a return address of {0} bytes on the stack.",
                        returnAddressBytes,
                        proc.Name,
(object)proc.Frame.ReturnAddressSize));
                }
            }
            else
            {
                proc.Frame.ReturnAddressSize = returnAddressBytes;
                proc.Frame.ReturnAddressKnown = true;
                proc.Signature.ReturnAddressOnStack = returnAddressBytes;
            }
        }

        private class StatementInjector : CodeEmitter
        {
            private Address addr;
            private Block block;
            private int iStm;
            private Procedure proc;

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
                var stm = block.Statements.Insert(iStm, addr.ToLinear(), instr);
                ++iStm;
                return stm;
            }

            public override Identifier Register(int i)
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// The new scanning algorithm, which incorporates the HeuristicScanner
    /// to disassemble pockets of bytes that are not reachable by simple 
    /// recursive disassembly of the binary.
    /// </summary>
    public class Scanner : ScannerOld
    {
        private ScanResults sr;

        public Scanner(Program program, IImportResolver importResolver, IServiceProvider services) : base(program, importResolver, services)
        {
            this.sr = new ScanResults
            {
                KnownProcedures = program.User.Procedures.Keys.ToHashSet(),
                KnownAddresses = program.ImageSymbols.ToDictionary(de => de.Key, de=> de.Value),
                ICFG = new DiGraph<RtlBlock>(),
            };
        }

        /// <summary>
        /// Scans the image, locating blobs of data and procedures.
        /// End result is that the program.ImageMap is populated with
        /// chunks of data, and the program.Procedures dictionary contains
        /// all procedures to decompile.
        /// </summary>
        public override void ScanImage()
        {
            ScanDataItems();

            // Now scan the executable parts of the image, to find all 
            // potential basic blocks. We use symbols, user procedures, and
            // the current contents of sr.DirectlyCalledAddresses as "seeds".
            // The end result is sr.ICFG, the interprocedural control graph.

            foreach (Procedure_v1 up in program.User.Procedures.Values)
            {
                var tup = EnsureUserProcedure(up);
            }
            foreach (ImageSymbol ep in program.EntryPoints.Values)
            {
                EnsureEntryPoint(ep);
            }
            foreach (ImageSymbol sym in program.ImageSymbols.Values.Where(s => s.Type == SymbolType.Procedure))
            {
                EnsureProcedure(sym.Address, sym.Name);
                if (sym.NoDecompile)
                    program.EnsureUserProcedure(sym.Address, sym.Name, false);
                else
                    EnqueueImageSymbol(sym, false);
            }

            var hsc = new HeuristicScanner(Services, program, this, eventListener);
            var sr = hsc.ScanImage();
            if (sr != null)
            {
                // Once the ICFG is built, detect the procedures.

                var pd = new ProcedureDetector(program, sr, eventListener);
                var procs = pd.DetectProcedures();

                // At this point, we have RtlProcedures and RtlBlocks.
                //$TODO: However, Reko hasn't had a chance to reconstitute constants yet, 
                // because that requires SSA, so we may be missing
                // opportunities to build and detect pointers. This typically happens in 
                // the type inference phase, when we both have constants and their types.
                // 
                // When this gets merged into analyis-development phase, fold 
                // Procedure construction into SSA construction.
                foreach (var rtlProc in procs)
                {
                    EnqueueProcedure(rtlProc.Entry.Address);
                }
                ProcessQueue();
            }
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
            var tlDeser = program.CreateTypeLibraryDeserializer();
            var dataScanner = new DataScanner(program, sr, eventListener);
            // Enqueue known data items, then process them. If we find code
            // pointers, they will be added to sr.DirectlyCalledAddresses.

            foreach (var global in program.User.Globals)
            {
                var addr = global.Key;
                var dt = global.Value.DataType.Accept(tlDeser);
                dataScanner.EnqueueUserGlobalData(addr, dt, global.Value.Name);
            }
            foreach (var sym in program.ImageSymbols.Values.Where(s => s.Type == SymbolType.Data))
            {
                dataScanner.EnqueueUserGlobalData(sym.Address, sym.DataType, sym.Name);
            }
            dataScanner.ProcessQueue();
            return sr;
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
