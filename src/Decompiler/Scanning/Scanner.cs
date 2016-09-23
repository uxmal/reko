#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
    /// A class implementing IScanner is responsible for tracing the execution
    /// flow of a binary image. It uses a Rewriter to convert the machine-
    /// specific instructions into lower-level register-transfer (RTL) 
    /// instructions. By simulating the execution of branch and call
    /// instructions, the IScanner can reconstruct the basic blocks and 
    /// procedures that constituted the original source program. Any 
    /// discoveries made are added to the Program instance.
    /// </summary>
    public interface IScanner
    {
        IServiceProvider Services { get; }

        void ScanImage();
        ProcedureBase ScanProcedure(Address addr, string procedureName, ProcessorState state);

        void EnqueueImageSymbol(ImageSymbol sym, bool isEntryPoint);
        void EnqueueProcedure(Address addr);
        Block EnqueueJumpTarget(Address addrSrc, Address addrDst, Procedure proc, ProcessorState state);
        void EnqueueUserProcedure(Procedure_v1 sp);
        void EnqueueUserProcedure(Address addr, FunctionType sig);
        void EnqueueUserGlobalData(Address addr, DataType dt);

        void Warn(Address addr, string message);
        void Warn(Address addr, string message, params object[] args);
        void Error(Address addr, string message);
        FunctionType GetCallSignatureAtAddress(Address addrCallInstruction);
        ExternalProcedure GetImportedProcedure(Address addrImportThunk, Address addrInstruction);
        void TerminateBlock(Block block, Address addrEnd);

        /// <summary>
        /// Find the block that contains the address <paramref name="addr"/>, or return null if there
        /// is no such block.
        /// </summary>
        /// <param name="addrStart"></param>
        /// <returns></returns>
        Block FindContainingBlock(Address addr);
        Block FindExactBlock(Address addr);
        Block SplitBlock(Block block, Address addr);

        ImageReader CreateReader(Address addr);

        Block CreateCallRetThunk(Address addrFrom, Procedure procOld, Procedure procNew);
        void SetProcedureReturnAddressBytes(Procedure proc, int returnAddressBytes, Address address);

        IEnumerable<RtlInstructionCluster> GetTrace(Address addrStart, ProcessorState state, Frame frame);

        void ScanImageHeuristically();
    }

    /// <summary>
    /// Scans the binary, locating and creating procedures and basic blocks by following calls, jumps,
    /// and branches. Simple data type analysis is done as well: for instance, pointers to
    /// code are located, as are global data pointers.
    /// </summary>
    /// <remarks>
    /// Callers feed the scanner by calling EnqueueXXX methods before calling ProcessQueue(). ProcessQueue() then
    /// processes the queues.
    /// </remarks>
    public class Scanner : IScanner, IRewriterHost
    {
        private const int PriorityEntryPoint = 5;
        private const int PriorityJumpTarget = 6;
        private const int PriorityGlobalData = 7;
        private const int PriorityVector = 4;
        private const int PriorityBlockPromote = 3;

        private static TraceSwitch trace = new TraceSwitch("Scanner", "Traces the progress of the Scanner");

        private Program program;
        private PriorityQueue<WorkItem> queue;
        private SegmentMap segmentMap;
        private ImageMap imageMap;
        private IImportResolver importResolver;
        private SortedList<Address, BlockRange> blocks;
        private Dictionary<Block, Address> blockStarts;
        private Dictionary<string, PseudoProcedure> pseudoProcs;
        private Dictionary<Address, ImportReference> importReferences;
        private DecompilerEventListener eventListener;
        private HashSet<Procedure> visitedProcs;
        private CancellationTokenSource cancelSvc;
        private HashSet<Address> scannedGlobalData = new HashSet<Address>();

        public Scanner(
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
            this.queue = new PriorityQueue<WorkItem>();
            this.blocks = new SortedList<Address, BlockRange>();
            this.blockStarts = new Dictionary<Block, Address>();
            this.pseudoProcs = program.PseudoProcedures;
            this.importReferences = program.ImportReferences;
            this.visitedProcs = new HashSet<Procedure>();
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
            eventListener.Warn(eventListener.CreateAddressNavigator(program, addr), string.Format(message, args));
        }

        public void Error(Address addr, string message)
        {
            eventListener.Error(eventListener.CreateAddressNavigator(program, addr), message);
        }

        public ImageReader CreateReader(Address addr)
        {
            return program.CreateImageReader(addr);
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

        public IEnumerable<RtlInstructionCluster> GetTrace(Address addrStart, ProcessorState state, Frame frame)
        {
            return program.Architecture.CreateRewriter(
                CreateReader(addrStart),
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
            queue.Enqueue(PriorityEntryPoint, new ImageSymbolWorkItem(this, program, sym, isEntryPoint));
        }

        public void EnqueueProcedure(Address addr)
        {
            queue.Enqueue(PriorityEntryPoint, new ProcedureWorkItem(this, program, addr, null));
        }

        public void EnqueueUserProcedure(Procedure_v1 sp) {
            Address addr;
            if (!program.Architecture.TryParseAddress(sp.Address, out addr))
                return;
            Procedure proc;
            if (program.Procedures.TryGetValue(addr, out proc))
                return; // Already scanned. Do nothing.
            if (!sp.Decompile)
                return;
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
            queue.Enqueue(PriorityEntryPoint, new ProcedureWorkItem(this, program, addr, sp.Name));
        }

        public void EnqueueUserProcedure(Address addr, FunctionType sig)
        {
            if (program.Procedures.ContainsKey(addr))
                return; // Already scanned. Do nothing.
            var proc = EnsureProcedure(addr, null);
            proc.Signature = sig;
            queue.Enqueue(PriorityEntryPoint, new ProcedureWorkItem(this, program, addr, proc.Name));
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
                    queue.Enqueue(PriorityJumpTarget, wi);
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
                    procDest = (Procedure) ScanProcedure(addrDest, null, state);
                    var blockThunk = CreateCallRetThunk(addrSrc, proc, procDest);
                    var wi = CreatePromoteWorkItem(addrDest, block, procDest);
                    queue.Enqueue(PriorityBlockPromote, wi);
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
                        queue.Enqueue(PriorityBlockPromote, wi);
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
                        queue.Enqueue(PriorityBlockPromote, wi);
                        return blockNew;
                    }
                }
            }
            return block;
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
            for (; ; )
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
                ExternalProcedure extProc;
                if (program.Procedures.TryGetValue(addr, out proc))
                    return; // Already scanned. Do nothing.
                if (TryGetNoDecompiledProcedure(addr, out extProc))
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
            var oldQueue = queue;
            queue = new PriorityQueue<WorkItem>();
            var block = EnqueueJumpTarget(addr, addr, proc, st);
            proc.ControlGraph.AddEdge(proc.EntryBlock, block);
            ProcessQueue();

            queue = oldQueue;

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
            st.SetValue(sp, proc.Frame.FramePointer);
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
            bb.Assign(sp, proc.Frame.FramePointer);
            program.Platform.InjectProcedureEntryStatements(proc, addr, bb);
        }

        private bool TryGetNoDecompiledProcedure(Address addr, out ExternalProcedure ep)
        {
            Procedure_v1 sProc;
            if (!program.User.Procedures.TryGetValue(addr, out sProc) ||
                sProc.Decompile)
            {
                ep = null;
                return false;
            }

            FunctionType sig = null;
            if (!string.IsNullOrEmpty(sProc.CSignature))
            {
                var usb = new UserSignatureBuilder(program);
                var procDecl = usb.ParseFunctionDeclaration(sProc.CSignature);
                if (procDecl != null)
                {
                    var ser = program.CreateProcedureSerializer();
                    sig = ser.Deserialize(
                        procDecl.Signature,
                        program.Architecture.CreateFrame());
                }
            }
            else
            {
                Warn(addr, "The user-defined procedure at address {0} did not have a signature.", addr); 
            }

            ep = new ExternalProcedure(sProc.Name, sig);
            return true;
        }

        public void EnqueueUserGlobalData(Address addr, DataType dt)
        {
            if (scannedGlobalData.Contains(addr))
                return;
            scannedGlobalData.Add(addr);
            queue.Enqueue(PriorityGlobalData, new GlobalDataWorkItem(this, program, addr, dt));
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

        public Identifier GetImportedGlobal(Address addrImportThunk, Address addrInstruction)
        {
            ImportReference impref;
            if (importReferences.TryGetValue(addrImportThunk, out impref))
            {
                var global = impref.ResolveImportedGlobal(
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
            var rdr= program.CreateImageReader(addrImportThunk);
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
        public void ScanImage()
        {
            ProcessQueue();
        }

        /// <summary>
        /// Uses the HeuristicScanner to try to located code heuristically.
        /// </summary>
        public void ScanImageHeuristically()
        {
            var heuristicScanner = new HeuristicScanner(program, this, eventListener);
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

        private Procedure EnsureProcedure(Address addr, string procedureName)
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

        private void ProcessQueue()
        {
            while (queue.Count > 0)
            {
                if (eventListener.IsCanceled())
                {
                    break;
                }
                var workitem = queue.Dequeue();
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

        public FunctionType GetCallSignatureAtAddress(Address addrCallInstruction)
        {
            UserCallData call = null;
            if (!program.User.Calls.TryGetValue(addrCallInstruction, out call))
                return null;
            return call.Signature;
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
                    Warn(
                        address,
                        string.Format(
                            "Procedure {1} previously had a return address of {2} bytes on the stack, " +
                            "but now seems to have a return address of {0} bytes on the stack.",
                        returnAddressBytes,
                        proc.Name,
                        proc.Frame.ReturnAddressSize));
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
