#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Decompiler.Scanning
{
    public interface IScanner
    {
        void ScanImage();

        void EnqueueEntryPoint(EntryPoint ep);
        Block EnqueueJumpTarget(Address addr, Procedure proc, ProcessorState state);
        ProcedureBase ScanProcedure(Address addr, string procedureName, ProcessorState state);
        void EnqueueUserProcedure(SerializedProcedure sp);
        void EnqueueVectorTable(Address addrUser, Address addrTable, PrimitiveType stride, ushort segBase, bool calltable, Procedure proc, ProcessorState state);

        CallGraph CallGraph { get; }
        LoadedImage Image { get; }
        ImageMap ImageMap { get; }  //$REVIEW: don't expose this?
        IProcessorArchitecture Architecture { get; }
        Platform Platform { get; }
        IDictionary<Address, VectorUse> VectorUses { get; }

        Block AddBlock(Address addr, Procedure proc, string blockName);
        void AddDiagnostic(Address addr, Diagnostic d);
        ProcedureSignature GetCallSignatureAtAddress(Address addrCallInstruction);
        PseudoProcedure GetImportedProcedure(uint linAddr);
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
        private Program program;
        private PriorityQueue<WorkItem> queue;
        private LoadedImage image;
        private ImageMap imageMap;
        private Map<uint, BlockRange> blocks;
        private Dictionary<Block, uint> blockStarts;
        private Dictionary<string, PseudoProcedure> pseudoProcs;
        private IDictionary<Address, ProcedureSignature> callSigs;
        private IDictionary<Address, ImageMapVectorTable> vectors;
        private Dictionary<uint, PseudoProcedure> importThunks;
        private DecompilerEventListener eventListener;
        private HashSet<Procedure> visitedProcs;

        private static TraceSwitch trace = new TraceSwitch("Scanner", "Traces the progress of the Scanner");
        
        private const int PriorityEntryPoint = 5;
        private const int PriorityJumpTarget = 6;
        private const int PriorityVector = 7;

        public Scanner(
            Program program, 
            IDictionary<Address, ProcedureSignature> callSigs,
            DecompilerEventListener eventListener)
        {
            this.program = program;
            this.image = program.Image;
            this.imageMap = program.ImageMap;
            this.callSigs = callSigs;
            this.eventListener = eventListener;
            if (imageMap == null)
                throw new InvalidOperationException("Program must have an image map.");
            this.Procedures = program.Procedures;
            this.queue = new PriorityQueue<WorkItem>();
            this.blocks = new Map<uint, BlockRange>();
            this.blockStarts = new Dictionary<Block, uint>();
            this.pseudoProcs = program.PseudoProcedures;
            this.vectors = program.Vectors;
            this.VectorUses = new Dictionary<Address, VectorUse>();
            this.importThunks = program.ImportThunks;
            this.visitedProcs = new HashSet<Procedure>();
        }

        public IProcessorArchitecture Architecture { get { return program.Architecture; } }
        public CallGraph CallGraph { get { return program.CallGraph; } }
        public LoadedImage Image { get { return image; } }
        public ImageMap ImageMap { get { return imageMap; } } //$REVIEW: don't expose this?
        public Platform Platform { get { return program.Platform; } }
        public PriorityQueue<WorkItem> Queue { get { return queue; } }
        public SortedList<Address, Procedure> Procedures { get; private set; }
        public IDictionary<Address, VectorUse> VectorUses { get; private set; }

        private class BlockRange
        {
            /// <summary>
            /// Creates a block range. A (linear) address addr is considered part of the block if it
            /// satisifies the conditions Start <= addr < End.
            /// </summary>
            /// <param name="block"></param>
            /// <param name="start">Linear start address of the block</param>
            /// <param name="end">Linear address of the byte/word beyond the block's end.</param>
            public BlockRange(Block block, uint start, uint end)
            {
                if (block == null)
                    throw new ArgumentNullException("block");
                this.Block = block;
                this.Start = start;
                this.End = end;
            }

            public Block Block { get; private set; }
            public uint Start { get; set; }
            public uint End { get; set; }
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
            Block b = new Block(proc, blockName);
            blocks.Add(addr.Linear, new BlockRange(b, addr.Linear, image.BaseAddress.Linear + (uint)image.Bytes.Length));
            blockStarts.Add(b, addr.Linear);
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
            if (blocks.TryGetLowerBound(addr.Linear, out range) && range.Start < addr.Linear)
                range.End = addr.Linear;
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

        public void AddDiagnostic(Address addr, Diagnostic d)
        {
            eventListener.AddDiagnostic(eventListener.CreateAddressNavigator(addr), d);
        }

        public ImageReader CreateReader(Address addr)
        {
            return image.CreateReader(addr);
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
                this.program.Architecture.CreateRewriter(
                    CreateReader(addrStart), 
                    stateOnEntry, 
                    proc.Frame,
                    this),
                stateOnEntry,
                proc.Frame,
                addrStart);
        }

        public void EnqueueEntryPoint(EntryPoint ep)
        {
            queue.Enqueue(PriorityEntryPoint, new EntryPointWorkitem(this, ep));
        }

        public Block EnqueueJumpTarget(Address addrStart, Procedure proc, ProcessorState state)
        {
            Block block = FindExactBlock(addrStart);
            if (block == null)
            {
                block = FindContainingBlock(addrStart);
                if (block != null)
                {
                    block = SplitBlock(block, addrStart);
                }
                else
                {
                    block = AddBlock(addrStart, proc, GenerateBlockName(addrStart));
                }
                var wi = CreateBlockWorkItem(addrStart, proc, state);
                queue.Enqueue(PriorityJumpTarget, wi);
            }
            return block;
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

        public void EnqueueVectorTable(Address addrFrom, Address addrTable, PrimitiveType stride, ushort segBase, bool calltable, Procedure proc, ProcessorState state)
        {
            ImageMapVectorTable table;
            if (vectors.TryGetValue(addrTable, out table))
                return;

            table = new ImageMapVectorTable(addrTable, calltable);
            var wi = new VectorWorkItem(this, table, proc);
            wi.State = state.Clone();
            wi.Stride = stride;
            wi.SegBase = segBase;
            wi.Table = table;
            wi.AddrFrom = addrFrom;

            imageMap.AddItem(addrTable, table);
            vectors[addrTable] = table;
            queue.Enqueue(PriorityVector, wi);
        }

        /// <summary>
        /// Performs a scan of the blocks that constitute a procedure named <paramref name="procedureName"/>
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="procedureName"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ProcedureBase ScanProcedure(Address addr, string procedureName, ProcessorState state)
        {
            TerminateAnyBlockAt(addr);
            PseudoProcedure imp = GetImportedProcedure(addr.Linear);
            if (imp != null)
                return imp;
            Procedure proc = EnsureProcedure(addr, procedureName);
            if (visitedProcs.Contains(proc))
                return proc;

            visitedProcs.Add(proc);
            Debug.WriteLineIf(trace.TraceInfo, string.Format("Scanning procedure at {0}", addr));

            //$REFACTOR: make the stack explicit?
            var oldQueue = queue;
            queue = new PriorityQueue<WorkItem>();
            var st = state.Clone();
            st.OnProcedureEntered();
            st.SetValue(proc.Frame.EnsureRegister(program.Architecture.StackRegister), proc.Frame.FramePointer);
            var block = EnqueueJumpTarget(addr, proc, st);
            proc.ControlGraph.AddEdge(proc.EntryBlock, block);
            ProcessQueue();
            queue = oldQueue;

            return proc;
        }

        private Procedure EnsureProcedure(Address addr, string procedureName)
        {
            Procedure proc;
            if (!Procedures.TryGetValue(addr, out proc))
            {
                proc = Procedure.Create(procedureName, addr, program.Architecture.CreateFrame());
                Procedures.Add(addr, proc);
                CallGraph.AddProcedure(proc);
            }
            return proc;
        }

        public void EnqueueUserProcedure(SerializedProcedure sp)
        {
            var proc = (Procedure)ScanProcedure(
                Address.ToAddress(sp.Address, 16),
                sp.Name,
                program.Architecture.CreateProcessorState());
            if (sp.Signature != null)
            {
                var sser = new ProcedureSerializer(program.Architecture, program.Platform.DefaultCallingConvention);
                proc.Signature = sser.Deserialize(sp.Signature, proc.Frame);
            }
            if (sp.Characteristics != null)
            {
                proc.Characteristics = sp.Characteristics;
            }
        }

        public const string CallRetThunkSuffix = "_tmp";

        public Block FindContainingBlock(Address address)
        {
            BlockRange b;
            if (blocks.TryGetLowerBound(address.Linear, out b) && address.Linear < b.End)
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
            if (blocks.TryGetValue(address.Linear, out b))
                return b.Block;
            else
                return null;
        }

        public PseudoProcedure GetImportedProcedure(uint linearAddress)
        {
            PseudoProcedure ppp;
            if (importThunks.TryGetValue(linearAddress, out ppp))
                return ppp;
            else
                return null;
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
            var blockNew = AddBlock(addr, blockToSplit.Procedure, GenerateBlockName(addr));
            foreach (var succ in graph.Successors(blockToSplit))
            {
                graph.AddEdge(blockNew, succ);
            }
            foreach (var succ in graph.Successors(blockNew))
            {
                graph.RemoveEdge(blockToSplit, succ);
            }

            var linAddr = addr.Linear;
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
            HandleCrossProcedureJumps();
        }

        private void HandleCrossProcedureJumps()
        {
            CrossProcedureAnalyzer crpa = new CrossProcedureAnalyzer(program);
            crpa.Analyze(program);
            Dump("Blocks needing promotion", crpa.BlocksNeedingPromotion);
            Dump("Blocks needing cloning", crpa.BlocksNeedingCloning);
            crpa.PromoteBlocksToProcedures(crpa.BlocksNeedingPromotion);
            crpa.CloneBlocksIntoOtherProcedures(crpa.BlocksNeedingCloning);
            crpa.ReplaceCrossJumpsWithCalls(program);
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

        private void ProcessQueue()
        {
            while (queue.Count > 0)
            {
                var workitem = queue.Dequeue();
                workitem.Process();
            }
        }

        public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
        {
            PseudoProcedure p;
            if (!pseudoProcs.TryGetValue(name, out p))
            {
                p = new PseudoProcedure(name, returnType, arity);
                pseudoProcs[name] = p;
            }
            return p;
        }

        /// <summary>
        /// Generates the name for a block stating at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns>The name as a string.</returns>
        private static string GenerateBlockName(Address addr)
        {
            return addr.GenerateName("l", "");
        }

        public ProcedureSignature GetCallSignatureAtAddress(Address addrCallInstruction)
        {
            ProcedureSignature sig = null;
            callSigs.TryGetValue(addrCallInstruction, out sig);
            return sig;
        }

        public PseudoProcedure GetImportThunkAtAddress(uint linaddrThunk)
        {
            PseudoProcedure ppp;
            if (importThunks.TryGetValue(linaddrThunk, out ppp))
                return ppp;
            else
                return null;
        }
    }
}
