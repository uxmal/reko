#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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
        void EnqueueProcedure(WorkItem wiPrev, Procedure proc, Address addrProc);
        Procedure EnqueueProcedure(WorkItem wiPrev, Address addr, string procedureName, ProcessorState state);
        void EnqueueUserProcedure(SerializedProcedure sp);
        void EnqueueVectorTable(Address addrUser, Address addrTable, PrimitiveType stride, ushort segBase, bool calltable, Procedure proc, ProcessorState state);
        void ProcessQueue();

        CallGraph CallGraph { get; }
        IProcessorArchitecture Architecture { get; } 
        Platform Platform { get; }
        IDictionary<Address, VectorUse> VectorUses { get; } 

        Block AddBlock(Address addr, Procedure proc, string blockName);
        void AddDiagnostic(Address addr, Diagnostic d);
        ProcedureSignature GetCallSignatureAtAddress(Address addrCallInstruction);
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
    public class Scanner : IScanner, IRewriterHost2
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

        private DecompilerEventListener eventListener;

        private const int PriorityEntryPoint = 5;
        private const int PriorityJumpTarget = 6;
        private const int PriorityVector = 7;

        public Scanner(
            IProcessorArchitecture arch, 
            ProgramImage image, 
            Platform platform,
            IDictionary<Address, ProcedureSignature> callSigs,
            DecompilerEventListener eventListener)
        {
            this.arch = arch;
            this.image = image;
            this.platform = platform;
            this.callSigs = callSigs;
            this.eventListener = eventListener;

            this.Procedures = new SortedList<Address, Procedure>();
            this.queue = new PriorityQueue<WorkItem>();
            this.blocks = new Map<uint, BlockRange>();
            this.blockStarts = new Dictionary<Block, uint>();
            this.callgraph = new CallGraph();
            this.pseudoProcs = new Dictionary<string, PseudoProcedure>();
            this.vectors = new Dictionary<Address, ImageMapVectorTable>();
            this.VectorUses = new Dictionary<Address, VectorUse>();
        }

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
        }

        public IProcessorArchitecture Architecture { get { return arch; } }
        public CallGraph CallGraph { get { return callgraph; } }
        public Platform Platform { get { return platform; } }
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
            blocks.Add(addr.Linear, new BlockRange(b, addr.Linear, image.BaseAddress.Linear + (uint) image.Bytes.Length));
            blockStarts.Add(b, addr.Linear);
            proc.ControlGraph.Nodes.Add(b);
            return b;
        }

        public void TerminateBlock(Block block, Address addr)
        {
            BlockRange range;
            if (blocks.TryGetLowerBound(addr.Linear, out range))
                range.End = addr.Linear;
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

        public void EnqueueEntryPoint(EntryPoint ep)
        {
            queue.Enqueue(PriorityEntryPoint, new EntryPointWorkitem2(this, ep));
        }

        public void EnqueueProcedure(WorkItem wiPrev, Procedure proc, Address addrProc)
        {
            throw new NotImplementedException();
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
            }
            queue.Enqueue(
                PriorityJumpTarget,
                new BlockWorkitem(
                    this,
                    this.arch.CreateRewriter2(CreateReader(addrStart), state, proc.Frame, this),
                    state,
                    proc.Frame,
                    block));
            return block;
        }

        public void EnqueueVectorTable(Address addrUser, Address addrTable, PrimitiveType stride, ushort segBase, bool calltable, Procedure proc, ProcessorState state)
        {
            ImageMapVectorTable table;
            if (vectors.TryGetValue(addrTable, out table))
                return;

            table = new ImageMapVectorTable(addrTable, calltable);
            var wi = new VectorWorkItem(this, image, table, proc);
            wi.state = state.Clone();
            wi.stride = stride;
            wi.segBase = segBase;
            wi.table = table;
            wi.addrFrom = addrUser;

            image.Map.AddItem(addrTable, table);
            vectors[addrTable] = table;
            queue.Enqueue(PriorityVector, wi);
        }


        public Procedure EnqueueProcedure(WorkItem wiPrev, Address addr, string procedureName, ProcessorState state)
        {
            var st = state.Clone();
            Procedure proc;
            if (!Procedures.TryGetValue(addr, out proc))
            {
                proc = Procedure.Create(procedureName, addr, arch.CreateFrame());
                Procedures.Add(addr, proc);
                CallGraph.AddProcedure(proc);
            }
            TerminateAnyBlockAt(addr);
            var block = EnqueueJumpTarget(addr, proc, state);
            proc.ControlGraph.AddEdge(proc.EntryBlock, block);
            return proc;
        }

        public void EnqueueUserProcedure(SerializedProcedure sp)
        {
            throw new NotImplementedException();
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


        /// <summary>
        /// Splits the given block at the specified address, yielding two blocks. The first block is the original block,
        /// now truncated, with a single out edge to the new block. The second block receives the out edges of the first block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="addr"></param>
        /// <returns>The newly created, empty second block</returns>
        public Block SplitBlock(Block block, Address addr)
        {
            var graph = block.Procedure.ControlGraph;
            var blockNew = AddBlock(addr, block.Procedure, GenerateBlockName(addr));
            foreach (var succ in graph.Successors(block))
            {
                graph.AddEdge(blockNew, succ);
            }
            foreach (var succ in graph.Successors(blockNew))
            {
                graph.RemoveEdge(block, succ);
            }

            var linAddr = addr.Linear;
            blockNew.Statements.AddRange(block.Statements.FindAll(s => s.LinearAddress >= linAddr));
            block.Statements.RemoveAll(s => s.LinearAddress >= linAddr);
            blocks[blockStarts[block]].End = linAddr;
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

        #endregion
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
    [Obsolete("Use Scanner for all new code")]
    public class ScannerOld : IScanner, ICodeWalkerListener
    {
        private Program program;
        private ImageMap map;			// cached copy of program.Image.Map; it's used very often.
        private DecompilerEventListener eventListener;
        private ImageMapBlock blockCur;
        private Dictionary<ImageMapBlock, ImageMapBlock> blocksVisited;
        private Map<Address, VectorUse> vectorUses;
        private SortedList<Address, SystemService> syscalls;
        private WorkItemOld wiCur;
        private DirectedGraphImpl<object> jumpGraph;    //$TODO: what is the right type? This is a bipartite graph with statements and procedures.

        private Queue<WorkItemOld> qProcs;
        private Queue<WorkItemOld> qJumps;
        private Queue<WorkItemOld> qStarts;
        private Queue<WorkItemOld> qSegments;
        private Queue<VectorWorkItemOld> qVectors;

        private static TraceSwitch trace = new TraceSwitch("Scanner", "Enables tracing in the scanning phase");

        public ScannerOld(Program program, DecompilerEventListener eventListener)
        {
            if (program.Image == null)
                throw new InvalidOperationException("Program.Image must be defined.");
            this.program = program;
            this.map = program.Image.Map;
            this.eventListener = eventListener;
            this.vectorUses = new Map<Address, VectorUse>();
            this.syscalls = new SortedList<Address, SystemService>();

            //$REVIEW: All these queues... could they be replaced with a priority queue?
            qStarts = new Queue<WorkItemOld>();
            qProcs = new Queue<WorkItemOld>();
            qJumps = new Queue<WorkItemOld>();
            qSegments = new Queue<WorkItemOld>();
            qVectors = new Queue<VectorWorkItemOld>();
            this.blocksVisited = new Dictionary<ImageMapBlock, ImageMapBlock>();
            this.jumpGraph = new DirectedGraphImpl<object>();
        }

        public virtual CodeWalker CreateCodeWalker(Address addr, ProcessorState state)
        {
            return program.Architecture.CreateCodeWalker(program.Image, program.Platform, addr, state);
        }

        /// <summary>
        /// An entry point into the binary has been found.
        /// </summary>
        /// <param name="addr"></param>
        public void EnqueueEntryPoint(EntryPoint ep)
        {
            Debug.Assert(ep.Address != null);
            WorkItemOld wi = new WorkItemOld(null, BlockType.Procedure, ep.Address);
            wi.state = ep.ProcessorState;
            EnqueueProcedure(null, ep.Address, ep.Name, wi.state);
            qStarts.Enqueue(wi);
        }


        /// <summary>
        /// The target of a branch or jump instruction has been found.
        /// </summary>
        /// <param name="addrTarget">The address to which we jump</param>
        /// <param name="st"></param>
        /// <param name="proc"></param>
        private void EnqueueJumpTarget(Address addrTarget, ProcessorState st, Procedure proc)
        {
            if (proc == null)
                throw new ArgumentNullException("proc");
            Debug.Assert(program.Image.IsValidAddress(addrTarget), string.Format("{0} is not a valid address.", addrTarget));
            WorkItemOld wi = new WorkItemOld(wiCur, BlockType.JumpTarget, addrTarget);
            wi.state = st.Clone();
            qJumps.Enqueue(wi);
            ImageMapBlock bl = new ImageMapBlock();
            bl.Procedure = proc;
            ImageMapBlock item = map.AddItem(addrTarget, bl) as ImageMapBlock;
            if (item != null && item.Procedure != proc)
            {
                // When two procedures join, they generate a new procedure. If there is
                // no procedure yet, make one and OWN all the reachable nodes.

                PromoteBlockToProcedure(bl, proc);
            }
        }

        public Procedure EnqueueProcedure(WorkItemOld wiPrev, Address addr, string procedureName)
        {
            return EnqueueProcedure(wiPrev, addr, procedureName, program.Architecture.CreateProcessorState());
        }

        /// <summary>
        /// The first instruction of a procedure has been found.
        /// </summary>
        /// <param name="addr">The address at which the procedure was found.</param>
        /// <param name="state"></param>
        public Procedure EnqueueProcedure(WorkItemOld wiPrev, Address addr, string procedureName, ProcessorState state)
        {
            WorkItemOld wi = new WorkItemOld(wiPrev, BlockType.Procedure, addr);
            wi.state = state.Clone();
            Procedure proc;
            if (!program.Procedures.TryGetValue(addr, out proc))
            {
                proc = Procedure.Create(procedureName, wi.Address, program.Architecture.CreateFrame());
                program.Procedures[wi.Address] = proc;
                program.CallGraph.AddProcedure(proc);
            }
            ImageMapBlock entry = new ImageMapBlock();
            entry.Procedure = proc;
            map.AddItem(addr, entry);

            qProcs.Enqueue(wi);
            return proc;
        }

        public void EnqueueProcedure(WorkItemOld wiPrev, Procedure proc, Address addrProc)
        {
            if (program.Procedures[addrProc] == null)
            {
                program.Procedures[addrProc] = proc;
                program.CallGraph.AddProcedure(proc);
            }
            WorkItemOld wi = new WorkItemOld(wiPrev, BlockType.Procedure, addrProc);
            wi.state = program.Architecture.CreateProcessorState();
            qProcs.Enqueue(wi);
        }

        /// <summary>
        /// A segment of the binary has been found. 
        /// </summary>
        /// <param name="addr"></param>
        public void EnqueueSegment(Address addr)
        {
            qSegments.Enqueue(new WorkItemOld(wiCur, BlockType.Segment, addr));
        }


        public void EnqueueUserProcedure(SerializedProcedure sp)
        {
            Procedure proc = EnqueueProcedure(null, Address.ToAddress(sp.Address, 16), sp.Name);
            if (sp.Signature != null)
            {
                var sser = new ProcedureSerializer(program.Architecture, program.Platform.DefaultCallingConvention); //$TODO: where do default signatures come from? Platform? Yes!
                proc.Signature = sser.Deserialize(sp.Signature, proc.Frame);
            }
            if (sp.Characteristics != null)
            {
                proc.Characteristics = sp.Characteristics;
            }
        }

        /// <summary>
        /// A vector table of jumps or procedures has been found.
        /// </summary>
        /// <param name="addrUser"></param>
        /// <param name="addrTable"></param>
        /// <param name="stride"></param>
        /// <param name="segBase">For x86 16-bit binaries, the code segment of the calling code. For all other architectures, zero.</param>
        /// <param name="calltable"></param>
        /// <param name="state"></param>
        public void EnqueueVectorTable(Address addrUser, Address addrTable, PrimitiveType stride, ushort segBase, bool calltable, Procedure proc, ProcessorState state)
        {
            ImageMapVectorTable table;
            if (!program.Vectors.TryGetValue(addrTable, out table))
            {
                table = new ImageMapVectorTable(addrTable, calltable);
                var wi = new VectorWorkItemOld(wiCur, proc, addrTable);
                wi.state = state.Clone();
                wi.reader = program.Image.CreateReader(addrTable);
                wi.stride = stride;
                wi.segBase = segBase;
                wi.table = table;
                wi.addrFrom = addrUser;

                map.AddItem(addrTable, table);
                program.Vectors[addrTable] = table;
                qVectors.Enqueue(wi);
            }
        }

        /// <summary>
        /// Heuristic function: returns false if the bytes at address <paramref>addr</paramref>
        /// don't look like the kind of code that might follow a table jump.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        private bool IsCodeAddress(Address addr)
        {
            return false;
        }

        #region ICodeWalkerListener methods

        public void OnBranch(ProcessorState st, Address addrInstr, Address addrTerm, Address addrBranch)
        {
            map.AddItem(addrTerm, new ImageMapItem());
            if (!program.Image.IsValidAddress(addrBranch))
            {
                Warn(addrInstr, "Instruction attempted to branch to illegal address {0}.", addrBranch);
                return;
            }

            jumpGraph.AddEdge(addrTerm - 1, addrTerm);
            jumpGraph.AddEdge(addrTerm - 1, addrBranch);

            EnqueueJumpTarget(addrBranch, st, blockCur.Procedure);
            EnqueueJumpTarget(addrTerm, st, blockCur.Procedure);
        }

        public void OnGlobalVariable(Address addr, PrimitiveType width, Constant c)
        {
            throw new NotImplementedException("NYI");
        }


        public void OnIllegalOpcode(Address addrIllegal)
        {
            throw new NotImplementedException("NYI");
        }

        public void OnJump(ProcessorState st, Address addrInstr, Address addrTerm, Address addrJump)
        {
            map.AddItem(addrTerm, new ImageMapItem());
            if (!program.Image.IsValidAddress(addrJump))
            {
                Warn(addrInstr, "Instruction attempted to jump to illegal address {0}.", addrJump);
                return;
            }
            jumpGraph.AddEdge(addrTerm - 1, addrJump);
            EnqueueJumpTarget(addrJump, st, blockCur.Procedure);
        }

        public void OnJumpPointer(ProcessorState st, Address addrFrom, Address addrJump, PrimitiveType width)
        {
            map.AddItem(addrFrom, new ImageMapItem());
        }

        public void OnJumpTable(ProcessorState st, Address addrInstr, Address addrTable, ushort segBase, PrimitiveType width)
        {
            if (!program.Image.IsValidAddress(addrTable))
            {
                Warn(addrInstr, "Unncertain about table start {0}.", addrTable);
                return;
            }
            EnqueueVectorTable(addrInstr, addrTable, width, segBase, false, blockCur.Procedure, st);
        }

        public void OnProcedure(ProcessorState st, Address addrCallee)
        {
            EnqueueProcedure(wiCur, addrCallee, null, st);
        }

        public void OnProcedurePointer(ProcessorState st, Address addrBase, Address addr, PrimitiveType width)
        {
        }

        public void OnProcedureTable(
            ProcessorState st,
            Address addrInstr,
            Address addrTable,
            ushort segBase,
            PrimitiveType width)
        {
            if (!program.Image.IsValidAddress(addrTable))
            {
                this.Warn(addrInstr, "Uncertain about table start {0}.", addrTable);
                return;
            }
            EnqueueVectorTable(addrInstr, addrTable, width, segBase, true, blockCur.Procedure, st);
        }

        public void OnProcessExit(Address addrTerm)
        {
            map.AddItem(addrTerm, new ImageMapItem());
        }

        public void OnReturn(Address addrTerm)
        {
            map.AddItem(addrTerm, new ImageMapItem());
        }

        public void OnSystemServiceCall(Address addrInstr, SystemService svc)
        {
            syscalls[addrInstr] = svc;
        }

        public void OnTrampoline(ProcessorState st, Address addrInstr, Address addrGlob)
        {
            PseudoProcedure ppp;
            if (program.ImportThunks.TryGetValue((uint)addrGlob.Linear, out ppp))
            {
                program.Trampolines[addrInstr.Linear] = ppp;
            }
        }

        public void Warn(Address addr, string format, params object[] args)
        {
            eventListener.AddDiagnostic(
                eventListener.CreateAddressNavigator(addr),
                new WarningDiagnostic(string.Format(format, args)));
        }

        #endregion

        public void ProcessQueue()
        {
            map.ItemSplit += ImageMap_ItemSplit;
            map.ItemCoincides += ImageMap_ItemCoincides;

            while (ProcessItem())
                ;

            map.ItemSplit -= new ItemSplitHandler(ImageMap_ItemSplit);
            map.ItemCoincides -= new ItemSplitHandler(ImageMap_ItemCoincides);
        }

        /// <summary>
        /// If we haven't been to this block before, create a codewalker and walk the block until we fall off the end.
        /// </summary>
        /// <remarks>
        /// During the call to Codewalker.WalkInstruction(), it is possible that the code block
        /// we're currently in (blockCur) is fragmented (due to a backwards jump, for instance).
        /// </remarks>
        /// <param name="wi"></param>
        public void ParseJumpTarget(WorkItemOld wi)
        {
            if (!program.Image.IsValidAddress(wi.Address))
            {
                Warn(wi.Address, "Attempted decompilation at invalid address.");
                return;
            }

            blockCur = SetCurrentBlock(wi.Address);
            if (blocksVisited.ContainsKey(blockCur))
                return;

            blocksVisited.Add(blockCur, blockCur);
            wi.state.SetInstructionPointer(wi.Address);
            CodeWalker cw = CreateCodeWalker(wi.Address, wi.state);
            try
            {
                do
                {
                    cw.WalkInstruction(this);
                } while (blockCur.IsInRange(cw.Address));
            }
            catch (Exception ex)
            {
                map.AddItem(cw.Address, new ImageMapItem());
                Warn(cw.Address, "Error occurred while disassembling. {0}", ex.Message);
            }
        }

        private ImageMapBlock SetCurrentBlock(Address addr)
        {
            ImageMapBlock b;
            ImageMapItem item;
            if (map.TryFindItem(addr, out item))
            {
                b = item as ImageMapBlock;
                if (b != null)
                    return b;
            }

            b = new ImageMapBlock();
            map.AddItem(addr, b);
            return b;
        }

        private void ParseProcedure(WorkItemOld wi)
        {
            //$TODO: make Item part of the workItem!
            ImageMapItem item;
            if (map.TryFindItemExact(wi.Address, out item))
            {
                ImageMapBlock bl = item as ImageMapBlock;
                if (bl != null)
                {
                    // We've already parsed this code, so it must be part of another procedure.
                    if (blocksVisited.ContainsKey(bl))
                        bl.Procedure = null;				// means that this is a block that is jumped into.
                }
            }
            if (program.Image.IsValidAddress(wi.Address))
            {
                EnqueueJumpTarget(wi.Address, wi.state, program.Procedures[wi.Address]);
            }
        }

        private void ParseSegment(WorkItemOld wi)
        {
        }

        private void ProcessVector(VectorWorkItemOld wi)
        {
            //$TODO: pass imagemapvectortable in workitem.
            ImageMapItem q;
            map.TryFindItem(wi.Address, out q);
            ImageMapVectorTable item = (ImageMapVectorTable)q;
            VectorBuilder builder = new VectorBuilder(program, map, jumpGraph);
            Address[] vector = builder.Build(wi.Address, wi.addrFrom, wi.segBase, wi.stride);
            if (vector == null)
            {
                Address addrNext = wi.Address + wi.stride.Size;
                if (program.Image.IsValidAddress(addrNext))
                {
                    // Can't determine the size of the table, but surely it has one entry?
                    map.AddItem(addrNext, new ImageMapItem());
                }
                return;
            }

            item.Addresses.AddRange(vector);
            for (int i = 0; i < vector.Length; ++i)
            {
                ProcessorState st = wi.state.Clone();
                if (wi.table.IsCallTable)
                {
                    EnqueueProcedure(wiCur, vector[i], null, st);
                }
                else
                {
                    jumpGraph.AddEdge(wi.addrFrom - 1, vector[i]);
                    EnqueueJumpTarget(vector[i], st, wi.proc);
                }
            }
            vectorUses[wi.addrFrom] = new VectorUse(wi.Address, builder.IndexRegister);
            map.AddItem(wi.Address + builder.TableByteSize, new ImageMapItem());
        }

        public bool ProcessItem()
        {
            wiCur = null;
            if (qStarts.Count > 0)
            {
                wiCur = qStarts.Dequeue();
                ParseProcedure(wiCur);
                return true;
            }

            if (qSegments.Count > 0)
            {
                wiCur = qSegments.Dequeue();
                ParseSegment(wiCur);
                return true;
            }

            if (qProcs.Count > 0)
            {
                wiCur = qProcs.Dequeue();
                ParseProcedure(wiCur);
                return true;
            }

            if (qJumps.Count > 0)
            {
                wiCur = qJumps.Dequeue();
                ParseJumpTarget(wiCur);
                return true;
            }

            if (qVectors.Count > 0)
            {
                VectorWorkItemOld vwi = qVectors.Dequeue();
                wiCur = vwi;
                ProcessVector(vwi);
                return true;
            }
            return false;
        }

        private void PromoteBlockToProcedure(ImageMapBlock bl, Procedure proc)
        {
            if (!program.Procedures.ContainsKey(bl.Address))
            {
                Procedure procNew = Procedure.Create(null, bl.Address, program.Architecture.CreateFrame());
                program.Procedures[bl.Address] = procNew;
                program.CallGraph.AddProcedure(procNew);
                bl.Procedure = procNew;
            }
        }


        public SortedList<Address, SystemService> SystemCalls
        {
            get { return syscalls; }
        }

        public Map<Address, VectorUse> VectorUses
        {
            get { return vectorUses; }
        }


        // Event handlers ///////////////

        private void ImageMap_ItemSplit(object o, ItemSplitArgs e)
        {
            ImageMapBlock oldBlock = e.ItemOld as ImageMapBlock;
            ImageMapBlock newBlock = e.ItemNew as ImageMapBlock;
            if (oldBlock != null && newBlock != null)
            {
                Debug.WriteIf(trace.TraceVerbose, string.Format("Split into {0} (in {1})", oldBlock.Address, oldBlock.Procedure != null ? oldBlock.Procedure.Name : "<none>"));
                Debug.WriteLineIf(trace.TraceVerbose, string.Format("       and {0} (in {1})", newBlock.Address, newBlock.Procedure != null ? newBlock.Procedure.Name : "<none>"));
            }
        }

        private void ImageMap_ItemCoincides(object o, ItemSplitArgs e)
        {
            if (e.ItemOld.GetType() == typeof(ImageMapItem))
            {
                map.Items[e.ItemOld.Address] = e.ItemNew;
                e.ItemNew.Size = e.ItemOld.Size;
            }
        }

        #region IScanner Members

        public Block AddBlock(Address addr, Procedure proc, string blockName)
        {
            throw new NotImplementedException();
        }

        void IScanner.AddDiagnostic(Address addr, Diagnostic d)
        {
            throw new NotImplementedException();
        }

        public Block FindExactBlock(Address addr)
        {
            throw new NotImplementedException();
        }

        public Block SplitBlock(Block block, Address addr)
        {
            throw new NotImplementedException();
        }

        public Block EnqueueJumpTarget(Address addr, Procedure proc, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public ImageReader CreateReader(Address addr)
        {
            throw new NotImplementedException();
        }

        Procedure IScanner.EnqueueProcedure(WorkItem wiPrev, Address addr, string procedureName, ProcessorState state)
        {
            throw new NotImplementedException();
        }


        void IScanner.EnqueueProcedure(WorkItem wiPrev, Procedure proc, Address addrProc)
        {
            throw new NotImplementedException();
        }

         IProcessorArchitecture IScanner.Architecture { get { return program.Architecture; } } 

        CallGraph IScanner.CallGraph { get { throw new NotImplementedException(); } }

        Platform IScanner.Platform { get { return program.Platform; } }

        ProcedureSignature IScanner.GetCallSignatureAtAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        IDictionary<Address, VectorUse> IScanner.VectorUses { get { return vectorUses; } }
        void IScanner.TerminateBlock(Block block, Address addr) { }
        Block IScanner.FindContainingBlock(Address addr) { throw new NotImplementedException(); }
        #endregion
    }
}
