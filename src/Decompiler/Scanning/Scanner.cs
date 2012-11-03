#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Decompiler.Scanning
{
    public interface IScanner
    {
        void EnqueueEntryPoint(EntryPoint ep);
        Block EnqueueJumpTarget(Address addr, Procedure proc, ProcessorState state);
        ProcedureBase ScanProcedure(Address addr, string procedureName, ProcessorState state);
        void EnqueueUserProcedure(SerializedProcedure sp);
        void EnqueueVectorTable(Address addrUser, Address addrTable, PrimitiveType stride, ushort segBase, bool calltable, Procedure proc, ProcessorState state);
        void ProcessQueue();

        CallGraph CallGraph { get; }
        ProgramImage Image { get; }
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
    /// Scans the binary, locating procedures and basic blocks by following calls, jumps,
    /// and branches. Simple data type analysis is done as well: for instance, pointers to
    /// code are located, as are global data pointers.
    /// </summary>
    /// <remarks>
    /// Callers feed the scanner by calling EnqueueXXX methods before calling Scan(). Scan() then
    /// processes the queues.
    /// </remarks>
    public class Scanner : IScanner, IRewriterHost
    {
        private IProcessorArchitecture arch;
        private PriorityQueue<WorkItem> queue;
        private ProgramImage image;
        private Map<uint, BlockRange> blocks;
        private Dictionary<Block, uint> blockStarts;
        private CallGraph callgraph;
        private Dictionary<string, PseudoProcedure> pseudoProcs;
        private Platform platform;
        private IDictionary<Address, ProcedureSignature> callSigs;
        private IDictionary<Address, ImageMapVectorTable> vectors;
        private Dictionary<uint, PseudoProcedure> importThunks;
        private DecompilerEventListener eventListener;
        private HashSet<Procedure> visitedProcs;

        private const int PriorityEntryPoint = 5;
        private const int PriorityJumpTarget = 6;
        private const int PriorityVector = 7;

        public Scanner(Program program, IDictionary<Address, ProcedureSignature> callSigs, DecompilerEventListener eventListener)
        {
            this.arch = program.Architecture;
            this.image = program.Image;
            this.platform = program.Platform;
            this.callSigs = callSigs;
            this.eventListener = eventListener;

            this.Procedures = program.Procedures;
            this.queue = new PriorityQueue<WorkItem>();
            this.blocks = new Map<uint, BlockRange>();
            this.blockStarts = new Dictionary<Block, uint>();
            this.callgraph = program.CallGraph;
            this.pseudoProcs = program.PseudoProcedures;
            this.vectors = program.Vectors;
            this.VectorUses = new Dictionary<Address, VectorUse>();
            this.importThunks = program.ImportThunks;
            this.visitedProcs = new HashSet<Procedure>();
        }

        public IProcessorArchitecture Architecture { get { return arch; } }
        public CallGraph CallGraph { get { return callgraph; } }
        public ProgramImage Image { get { return image; } }
        public Platform Platform { get { return platform; } }
        public PriorityQueue<WorkItem> Queue { get { return queue; } }
        public SortedList<Address, Procedure> Procedures { get; private set; }
        public IDictionary<Address, VectorUse> VectorUses { get; private set; }

        private class BlockRange
        {
            public BlockRange(Block block, uint start, uint end)
            {
                if (block == null)
                    throw new ArgumentNullException("block");
                this.Block = block;
                this.Start = start;
                this.End = end;
            }

            public Block Block { get; set; }
            public uint Start { get; set; }
            public uint End { get; set; }
        }


        #region IScanner Members

        public Block AddBlock(Address addr, Procedure proc, string blockName)
        {
            Block b = new Block(proc, blockName);
            blocks.Add(addr.Linear, new BlockRange(b, addr.Linear, image.BaseAddress.Linear + (uint)image.Bytes.Length));
            blockStarts.Add(b, addr.Linear);
            proc.ControlGraph.Blocks.Add(b);

            image.Map.AddItem(addr, new ImageMapBlock { Block = b }); 
            return b;
        }

        public void TerminateBlock(Block block, Address addr)
        {
            BlockRange range;
            if (blocks.TryGetLowerBound(addr.Linear, out range) && range.Start < addr.Linear)
                range.End = addr.Linear;
            image.Map.TerminateItem(addr);
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

        public virtual BlockWorkitem CreateBlockWorkItem(Address addrStart, Procedure proc, ProcessorState stateOnEntry)
        {
            return new BlockWorkitem(
                this,
                this.arch.CreateRewriter(CreateReader(addrStart), stateOnEntry, proc.Frame, this),
                stateOnEntry,
                proc.Frame,
                addrStart);
        }


        public void EnqueueEntryPoint(EntryPoint ep)
        {
            queue.Enqueue(PriorityEntryPoint, new EntryPointWorkitem(this, ep));
        }

        // Method is virtual because we want to peek into the parameters being passed to it.
        public virtual Block EnqueueJumpTarget(Address addrStart, Procedure proc, ProcessorState state)
        {
            Block block = FindExactBlock(addrStart);
            if (block != null)
            {
                if (block.Procedure != proc)
                {
                    Procedure procNew;
                    if (!Procedures.TryGetValue(addrStart, out procNew))
                    {
                        procNew = Procedure.Create(addrStart, arch.CreateFrame());
                        Procedures.Add(addrStart, procNew);
                        CallGraph.AddProcedure(procNew);
                        procNew.Frame.ReturnAddressSize = proc.Frame.ReturnAddressSize;
                    }
                    var bp = new BlockPromoter(block, procNew, arch);
                    bp.Promote();
                    block = bp.CallRetThunkBlock;
                }
                return block;
            }

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
            return block;
        }

        public void EnqueueVectorTable(Address addrFrom, Address addrTable, PrimitiveType stride, ushort segBase, bool calltable, Procedure proc, ProcessorState state)
        {
            ImageMapVectorTable table;
            if (vectors.TryGetValue(addrTable, out table))
                return;

            table = new ImageMapVectorTable(addrTable, calltable);
            var wi = new VectorWorkItem(this, image, table, proc);
            wi.State = state.Clone();
            wi.Stride = stride;
            wi.SegBase = segBase;
            wi.Table = table;
            wi.AddrFrom = addrFrom;

            image.Map.AddItem(addrTable, table);
            vectors[addrTable] = table;
            queue.Enqueue(PriorityVector, wi);
        }


        public ProcedureBase ScanProcedure(Address addr, string procedureName, ProcessorState state)
        {
            var pb = GetImportedProcedure(addr.Linear);
            if (pb != null)
                return pb;

            Procedure proc = EnsureProcedure(addr, procedureName);
            TerminateAnyBlockAt(addr);
            if (visitedProcs.Contains(proc))
                return proc;
            visitedProcs.Add(proc);

            Debug.Print("Scanning procedure at {0}", addr);

            var oldQueue = queue;
            queue = new PriorityQueue<WorkItem>();
            var st = state.Clone();
            st.OnProcedureEntered();
            st.SetValue(proc.Frame.EnsureRegister(arch.StackRegister), proc.Frame.FramePointer);
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
                proc = Procedure.Create(procedureName, addr, arch.CreateFrame());
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
                arch.CreateProcessorState());
            if (sp.Signature != null)
            {
                var sser = new ProcedureSerializer(arch, platform.DefaultCallingConvention);
                proc.Signature = sser.Deserialize(sp.Signature, proc.Frame);
            }
            if (sp.Characteristics != null)
            {
                proc.Characteristics = sp.Characteristics;
            }
        }

        public Block FindContainingBlock(Address address)
        {
            BlockRange b;
            if (blocks.TryGetLowerBound(address.Linear, out b) && address.Linear < b.End)
                return b.Block;
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
            foreach (var stm in blockToSplit.Statements.FindAll(s => s.LinearAddress >= linAddr))
            {
                blockNew.Statements.Add(stm.LinearAddress, stm.Instruction);
            }
            if (blockToSplit.Statements.Count > 0 && blockToSplit.Statements.Last.LinearAddress >= linAddr)
            {
                graph.AddEdge(blockToSplit, blockNew);
                blockToSplit.Statements.RemoveAll(s => s.LinearAddress >= linAddr);
            }
            blocks[blockStarts[blockToSplit]].End = linAddr;
            return blockNew;
        }

        public void ProcessQueue()
        {
            while (queue.Count > 0)
            {
                var workitem = queue.Dequeue();
                workitem.Process();
            }
        }

        #endregion


        #region IRewriterHost2 Members

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

        private static string GenerateBlockName(Address addrStart)
        {
            return addrStart.GenerateName("l", "");
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

        #endregion
    }
}

