#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using System.Text;

namespace Reko.Analysis
{
	/// <summary>
	/// Transforms a <see cref="Reko.Core.Procedure"/> to Static Single Assignment form.
	/// </summary>
    /// <remarks>
    /// This class implements an SSA algorithm that does not require the
    /// calculation of the dominator graph. It is based on the algorithm
    /// described in "Simple and Efficient Construction of Static Single
    /// Assignment Form" by Matthias Braun, Sebastian Buchwald, Sebastian 
    /// Hack, Roland LeiЯa, Christoph Mallon, and Andreas Zwinkau. 
    /// It has been augmented with storage alias analysis, and could be
    /// augmented with expression simplification if we can prove that the
    /// CFG graph is completed (future work).
    /// </remarks>
    [DebuggerDisplay("{SsaState.Procedure.Name}")]
    public partial class SsaTransform : InstructionTransformer 
    {
        private static readonly TraceSwitch trace = new TraceSwitch("SsaTransform", "Traces the progress of SSA analysis") { Level = TraceLevel.Warning };

        private readonly IProcessorArchitecture arch;
        private readonly Program program;
        private readonly ProgramDataFlow programFlow;
        private readonly IDynamicLinker dynamicLinker;
        private readonly Dictionary<Block, SsaBlockState> blockstates;
        private readonly SsaState ssa;
        private readonly TransformerFactory factory;
        private readonly HashSet<SsaIdentifier> incompletePhis;
        private readonly HashSet<Procedure> sccProcs;
        private readonly ExpressionEmitter m;
        private HashSet<SsaIdentifier> sidsToRemove;
        private Block block;
        private Statement stmCur;

        public SsaTransform(
            Program program,
            Procedure proc,
            HashSet<Procedure> sccProcs,
            IDynamicLinker dynamicLinker,
            ProgramDataFlow programFlow)
        {
            this.arch = proc.Architecture;
            this.program = program;
            this.programFlow = programFlow;
            this.dynamicLinker = dynamicLinker;
            this.sccProcs = sccProcs;
            this.ssa = new SsaState(proc);
            this.blockstates = ssa.Procedure.ControlGraph.Blocks.ToDictionary(k => k, v => new SsaBlockState(v));
            this.factory = new TransformerFactory(this);
            this.incompletePhis = new HashSet<SsaIdentifier>();
            this.m = new ExpressionEmitter();
        }

        /// <summary>
        /// If set, only renames frame accesses.
        /// </summary>
        public bool RenameFrameAccesses { get; set; }

        /// <summary>
        /// The SSA graph of the procedure being transformed.
        /// </summary>
        public SsaState SsaState { get { return ssa; } }

        /// <summary>
        /// Transforms <paramref name="proc"/> into Static Single
        /// Assignment form.
        /// </summary>
        /// <remarks>
        /// The resulting SSA identifiers are conveniently kept in the
        /// SsaState property.
        /// </remarks>
        /// <param name="proc"></param>
        public SsaState Transform()
        {
            DebugEx.Inform(trace, "SsaTransform: {0}, rename frame accesses {1}", ssa.Procedure.Name, this.RenameFrameAccesses);
            this.sidsToRemove = new HashSet<SsaIdentifier>();
            foreach (var bs in blockstates.Values)
            {
                bs.Visited = false;
            }

            // Visit blocks in RPO order so that we are guaranteed that a 
            // block with predecessors is always visited after them.

            foreach (Block b in new DfsIterator<Block>(ssa.Procedure.ControlGraph).ReversePostOrder())
            {
                // TerminateBlockAfterStatement may be orphaned blocks. Detect these early and bail.
                if (b != b.Procedure.EntryBlock && b.Pred.Count == 0)
                    continue;

                DebugEx.Verbose(trace, "SsaTransform:   {0} ({1} statements)", b.Name, b.Statements.Count);
                this.block = b;
                blockstates[b].Terminates = false;
                foreach (var s in b.Statements.ToList())
                {
                    this.stmCur = s;
                    DebugEx.Verbose(trace, "SsaTransform:     {0:X4} {1}", s.LinearAddress, s);
                    s.Instruction = s.Instruction.Accept(this);
                    if (blockstates[b].Terminates)
                    {
                        TerminateBlockAfterStatement(b, s);
                        blockstates[b].Terminates = true;
                        break;
                    }
                }
                blockstates[b].Visited = true;
            }
            ProcessIncompletePhis();
            RemoveRedundantPhis();
            RemoveDeadSsaIdentifiers();
            RemoveOrphanedBasicBlocks();
            return ssa;
        }

        /// <summary>
        /// Remove any SSA identifiers with no uses. There will be a lot
        /// of these, especially for flag registers.
        /// </summary>
        public void RemoveDeadSsaIdentifiers()
        {
            foreach (var sid in sidsToRemove.Where(s => s.Uses.Count == 0))
            {
                sid.DefStatement = null;
                sid.DefExpression = null;
                ssa.Identifiers.Remove(sid);
            }
        }

        public void RemoveOrphanedBasicBlocks()
        {
            var orphans = ssa.Procedure.ControlGraph.Blocks
                .Where(b => b.Pred.Count == 0 && b != b.Procedure.EntryBlock)
                .ToHashSet();
            if (orphans.Count == 0)
                return;
            var wl = new WorkList<Block>(ssa.Procedure.ControlGraph.Blocks.Except(orphans));
            while (wl.GetWorkItem(out var item))
            {
                var allPredsOrphan = item.Pred.All(p => orphans.Contains(p));
                if (allPredsOrphan && item != item.Procedure.EntryBlock)
                {
                    orphans.Add(item);
                    wl.AddRange(item.Succ.Where(s => !orphans.Contains(s)));
                }
            }

            foreach (var stm in ssa.Procedure.Statements)
            {
                if (stm.Instruction is PhiAssignment phi)
                {
                    var orphanedArgs = phi.Src.Arguments
                        .Where(a => orphans.Contains(a.Block));
                    foreach (var arg in orphanedArgs)
                    {
                        ssa.Identifiers[(Identifier) arg.Value].Uses.Remove(stm);
                    }
                    var newArgs = phi.Src.Arguments
                        .Where(a => !orphans.Contains(a.Block))
                        .ToArray();

                    stm.Instruction = new PhiAssignment(phi.Dst, newArgs);
                }
            }
            var cfg = ssa.Procedure.ControlGraph;

            foreach (var orphan in orphans)
            {
                foreach (var stm in orphan.Statements.ToList())
                {
                    if (stm.Instruction is CallInstruction)
                    {
                        program.CallGraph.RemoveCaller(stm);
                    }
                    ssa.DeleteStatement(stm);
                }
                if (orphan != orphan.Procedure.ExitBlock)
                {
                    var ps = orphan.Pred.ToArray();
                    var ss = orphan.Succ.ToArray();
                    foreach (var p in ps)
                    {
                        cfg.RemoveEdge(p, orphan);
                    }
                    foreach (var s in ss)
                    {
                        cfg.RemoveEdge(orphan, s);
                    }
                    orphan.Procedure.RemoveBlock(orphan);
                    orphan.Procedure = null;
                }
            }
            orphans.Count.ToString();
        }

        /// <summary>
        /// Adds a UseInstruction for each SsaIdentifier.
        /// </summary>
        /// <remarks>
        /// Doing this will allow us to detect what definitions reach the end
        /// of the function.
        /// //$TODO: what about functions that don't terminate, or have branches
        /// that don't terminate? In such cases,
        /// the identifiers should be removed.
        /// </remarks>
        public void AddUsesToExitBlock()
        {
            //$TODO: flag groups need to be grouped on exit
            // TrashedRegisterFinder should collect aliased registers
            // (e.g. eax, ax, al, ah) and render them as a single
            // register (eax).
            this.block = ssa.Procedure.ExitBlock;
            DebugEx.Verbose(trace, "SsaTransform: AddUsesToExitBlock  {0}", this.block);

            // Compute the set of all blocks b such that there is a path from
            // b to the exit block.
            var reachingBlocks = FindPredecessorClosure(ssa.Procedure.ExitBlock);
            var existing = block.Statements
                .Select(s => s.Instruction as UseInstruction)
                .Where(u => u != null)
                .Select(u => u.Expression)
                .ToHashSet();
            var reachingIds = ssa.Identifiers
                .Where(sid => sid.DefStatement != null &&
                              reachingBlocks.Contains(sid.DefStatement.Block) &&
                              sid.Identifier.Name != sid.OriginalIdentifier.Name &&
                              !(sid.Identifier.Storage is MemoryStorage) &&
                              !(sid.Identifier.Storage is StackStorage) &&
                              !(sid.Identifier.Storage is TemporaryStorage) &&
                              !existing.Contains(sid.Identifier))
                .Select(sid => sid.OriginalIdentifier);
            reachingIds = SeparateSequences(reachingIds);
            reachingIds = ExpandFlagGroups(reachingIds);
            var sortedIds = ResolveOverlaps(reachingIds)
                .Distinct()
                .OrderBy(id => id.Name);    // Sort them for stability; unit test are sensitive to shifting order 

            var stms = sortedIds
                .Select(id => new Statement(
                    block.Address.ToLinear(),
                    new UseInstruction(id),
                    block))
                .ToList();
            block.Statements.AddRange(stms);
            DebugEx.Verbose(trace, "AddUsesToExitBlock");
            stms.ForEach(u =>
            {
                var use = (UseInstruction)u.Instruction;
                DebugEx.Verbose(trace, "SsaTransform:   {0}", use);
                use.Expression = NewUse((Identifier)use.Expression, u, true);
            });
        }

        /// <summary>
        /// Remove SCC's of phi assignments whose arguments are other variables in the SCC
        /// or a single external value.
        /// </summary>
        /// <remarks>
        /// Implements Algorithm 5 of the Braun et al. paper.
        /// </remarks>
        private void RemoveRedundantPhis(PhiAssignment[] phis = null)
        {
            if (phis == null)
            {
                phis = ssa.Procedure.Statements
                    .Select(stm => stm.Instruction as PhiAssignment)
                    .Where(p => p != null)
                    .ToArray();
            }

            SccFinder<PhiAssignment> sccFinder = new SccFinder<PhiAssignment>(new PhiGraph(ssa, phis), processScc);
            sccFinder.FindAll();

            void processScc(IList<PhiAssignment> scc)
            {
                if (scc.Count == 1)
                    return;
                var sccIds = new HashSet<Expression>(scc.Select(p => (Expression) p.Dst));
                var inner = new HashSet<PhiAssignment>();
                var outerOps = new HashSet<Expression>();
                foreach (var phi in scc)
                {
                    bool isInner = true;
                    foreach (var arg in phi.Src.Arguments)
                    {
                        if (!sccIds.Contains(arg.Value))
                        {
                            outerOps.Add(arg.Value);
                            isInner = false;
                        }
                    }
                    if (isInner)
                        inner.Add(phi);
                }

                if (outerOps.Count == 1)
                {
                    ReplaceWithValue(scc, outerOps.First());
                }
                //$TODO: this code is causing stack overflows in many places.
                //else if (outerOps.Count > 1)
                //{
                //    RemoveRedundantPhis(inner.ToArray());
                //}
            }
        }

        private void ReplaceWithValue(IEnumerable<PhiAssignment> scc, Expression value)
        {
            foreach (var phi in scc)
            {
                var stm = ssa.Identifiers[phi.Dst].DefStatement;
                ssa.RemoveUses(stm);
                stm.Instruction = new AliasAssignment(phi.Dst, value);
                ssa.AddUses(stm);
            }
        }

        private ISet<Block> FindPredecessorClosure(Block start)
        {
            var wl = new WorkList<Block>();
            var preds = new HashSet<Block>();
            wl.Add(start);
            while (wl.GetWorkItem(out var b))
            {
                foreach (var p in b.Pred)
                {
                    if (!preds.Contains(p))
                    {
                        preds.Add(p);
                        wl.Add(p);
                    }
                }
            }
            return preds;
        }

        /// <summary>
        /// Given a stream of <see cref="Identifier"/>s, some of which are sequences, return
        /// a stream where the sequences have been broken apart into their constituent parts.
        /// </summary>
        public IEnumerable<Identifier> SeparateSequences(IEnumerable<Identifier> ids)
        {
            foreach (var id in ids)
            {
                if (id.Storage is SequenceStorage seq)
                {
                    foreach (var e in seq.Elements)
                    {
                        yield return ssa.Procedure.Frame.EnsureIdentifier(e);
                    }
                }
                else
                {
                    yield return id;
                }
            }
        }

        /// <summary>
        /// Given a sequence of identifiers, returns a new sequence where
        /// identifiers that overlap in memory are fused.
        /// </summary>
        /// <remarks>
        /// For instance, if the input sequence contains identifiers whose
        /// storages are [bx], [bh], and [rbx], the output sequence contains
        /// only [rbx].</remarks>
        public static IEnumerable<Identifier> ResolveOverlaps(IEnumerable<Identifier> ids)
        {
            var registerBag = new Dictionary<StorageDomain, HashSet<Identifier>>();
            var others = new List<Identifier>();
            foreach (var id in ids)
            {
                if (id.Storage is RegisterStorage)
                {
                    var dom = id.Storage.Domain;
                    if (registerBag.TryGetValue(dom, out var aliases))
                    {
                        aliases.RemoveWhere(a => id.Storage.Covers(a.Storage));
                        if (!aliases.Any(a => a.Storage.Covers(id.Storage)))
                            aliases.Add(id);
                    }
                    else
                    {
                        aliases = new HashSet<Identifier> { id };
                        registerBag.Add(dom, aliases);
                    }
                }
                else
                {
                    others.Add(id);
                }
            }
            return registerBag.Values.SelectMany(s => s).Concat(others);
        }

        /// <summary>
        /// Given a sequence of identifiers, collects all the processor flags as a summary.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<Identifier> CollectFlagGroups(
            IEnumerable<Identifier> ids, 
            IStorageBinder binder,
            IProcessorArchitecture arch)
        {
            var grfs = new SortedList<RegisterStorage, uint>();
            foreach (var id in ids)
            {
                if (id.Storage is FlagGroupStorage grf)
                {
                    var grfTotal = grfs.Get(grf.FlagRegister);
                    grfs[grf.FlagRegister] = grfTotal | grf.FlagGroupBits;
                }
                else
                {
                    yield return id;
                }
            }
            if (grfs.Count > 0)
            {
                foreach (var de in grfs)
                {
                    var grfNew = arch.GetFlagGroup(de.Key, de.Value);
                    yield return binder.EnsureFlagGroup(grfNew);
                }
            }
        }

        /// <summary>
        /// Iterates through a list of Identifiers, expading every flag group
        /// into its constituent flag bits.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<Identifier> ExpandFlagGroups(IEnumerable<Identifier> ids)
        {
            foreach (var id in ids)
            {
                if (id.Storage is FlagGroupStorage grf)
                {
                    foreach (var singleFlag in arch.GetSubFlags(grf))
                    {
                        yield return ssa.Procedure.Frame.EnsureFlagGroup(singleFlag);
                    }
                }
                else
                {
                    yield return id;
                }
            }
        }

        /// <summary>
        /// Removes any statements after stm in the block and removes 
        /// all out edges from the 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="stm"></param>
        private void TerminateBlockAfterStatement(Block block, Statement stm)
        {
            int iStm = block.Statements.IndexOf(stm);
            Debug.Assert(iStm >= 0);
            for (int i = block.Statements.Count - 1; i > iStm; --i)
            {
                // We're assuming these statements haven't been visited 
                // by SSA transform yet, so they are not in the SSA graph.
                block.Statements.RemoveAt(i);
            }
            // Now remove edges from CFG
            var succs = block.Succ.ToList();
            foreach (var s in succs)
            {
                block.Procedure.ControlGraph.RemoveEdge(block, s);
            }
        }

        public override Instruction TransformAssignment(Assignment a)
        {
            if (a is AliasAssignment)
                return a;
            var src = a.Src.Accept(this);
            Identifier idNew = this.RenameFrameAccesses ? a.Dst : NewDef(a.Dst, src, false);
            return new Assignment(idNew, src);
        }

        /// <summary>
        /// Handle a call to another procedure. If the procedure has a
        /// signature, we can create an Application immediately. If the
        /// procedure has a defined ProcedureFlow, we use the BitsUsed
        /// and Trashed sets to set the uses and defs sets of the call
        /// instruction. If the called procedure is part of a recursive
        /// nest, or is a "hell node" (a hell node is an indirect call or
        /// indirect jump that prior passes have been unable to resolve),
        /// we must assume the worst and use all defined registers and 
        /// trash everything. The hope is that, for recursive procedures
        /// at least, we can eliminate some of the uses and defines.
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        public override Instruction TransformCallInstruction(CallInstruction ci)
        {
            ci.Callee = ci.Callee.Accept(this);
            ProcedureBase callee = GetCalleeProcedure(ci);
            if (callee != null && callee.Signature.ParametersValid)
            {
                // Signature is known: build the application immediately,
                // after removing all uses of the old call.
                ssa.RemoveUses(stmCur);
                var ab = CreateApplicationBuilder(ci.Callee.DataType, callee, ci);
                var instr = ab.CreateInstruction(callee.Signature, callee.Characteristics);
                return instr.Accept(this);
            }
            if (callee is Procedure proc &&
                programFlow.ProcedureFlows.TryGetValue(proc, out var calleeFlow) && 
                !sccProcs.Contains(proc))
            {
                // If the callee is a procedure constant and it's not part of the
                // current recursion group, we should know what storages are live
                // in and trashed.
                GenerateUseDefsForKnownCallee(ci, proc, calleeFlow);
            }
            else
            {
                GenerateUseDefsForUnknownCallee(ci);
            }
            return ci;
        }

        private ApplicationBuilder CreateApplicationBuilder(DataType dt, ProcedureBase eCallee, CallInstruction call)
        {
            var pc = new ProcedureConstant(dt, eCallee);
            var ab = arch.CreateFrameApplicationBuilder(ssa.Procedure.Frame, call.CallSite, pc);
            return ab;
        }

        private void GenerateUseDefsForKnownCallee(CallInstruction ci, Procedure callee, ProcedureFlow calleeFlow)
        {
            int fpuStackDelta;
            if (this.RenameFrameAccesses)
            {
                fpuStackDelta = callee.Signature.FpuStackDelta;
                foreach (var use in ci.Uses)
                {
                    use.Expression = use.Expression.Accept(this);
                }
                foreach (var def in ci.Definitions)
                {
                    def.Expression = TransformLValue(def.Expression, ci.Callee);
                }
            }
            else
            {
                fpuStackDelta = calleeFlow.GetFpuStackDelta(arch);
                var ab = arch.CreateFrameApplicationBuilder(ssa.Procedure.Frame, ci.CallSite, ci.Callee);
                foreach (var stgUse in calleeFlow.BitsUsed.Keys)
                {
                    Expression e;
                    if (stgUse is FpuStackStorage fpuUse)
                    {
                        e = arch.CreateFpuStackAccess(
                            ssa.Procedure.Frame,
                            fpuUse.FpuStackOffset,
                            PrimitiveType.Word64); //$TODO: datatype?
                    }
                    else
                    {
                        e = stgUse.Accept(ab);
                    }
                    e = e.Accept(this);
                    ci.Uses.Add(new CallBinding(stgUse, e));
                }

                if (calleeFlow.TerminatesProcess)
                {
                    // We just discovered this basic block doesn't terminate.
                    this.blockstates[block].Terminates = true;
                    return;
                }

                foreach (var def in calleeFlow.Trashed.Where(dd => !(dd is FpuStackStorage)))
                {
                    var d = ssa.Procedure.Frame.EnsureIdentifier(def);

                    //$HACK: special case for FPU stacks; is there a cleaner way
                    // to do this?
                    if (def == arch.FpuStackRegister && calleeFlow.Constants.ContainsKey(def))
                    {
                        // We will add an explicit increment statement later.
                        continue;
                    }
                    ci.Definitions.Add(
                        new CallBinding(
                            def,
                            NewDef(d, ci.Callee, false)));
                }
                foreach (var de in calleeFlow.grfTrashed)
                {
                    var grfs = arch.GetFlagGroup(de.Key, de.Value);
                    foreach (var grf in arch.GetSubFlags(grfs))
                    {
                        var d = ssa.Procedure.Frame.EnsureFlagGroup(grf);
                        ci.Definitions.Add(
                            new CallBinding(
                                grf,
                                NewDef(d, ci.Callee, false)));
                    }
                }
                //$REVIEW: this is very x86/x87 specific; find a way to generalize
                // this to any sort of stack-based discipline.
                var fpuDefs = CreateFpuStackTemporaryBindings(
                    calleeFlow,
                    ci.Callee,
                    fpuStackDelta);
                ci.Definitions.UnionWith(fpuDefs);
                InsertFpuStackAccessAssignments(fpuDefs);
            }

            //$REFACTOR: this is common code with BlockWorkItem; find
            // a way to reuse it
            if (fpuStackDelta != 0)
            {
                var fpuStackReg = SsaState.Procedure.Frame.EnsureRegister(arch.FpuStackRegister);
                var src = m.AddSubSignedInt(fpuStackReg, fpuStackDelta);
                var iCur = stmCur.Block.Statements.IndexOf(stmCur);
                stmCur = stmCur.Block.Statements.Insert(iCur + 1,
                    stmCur.LinearAddress,
                    new Assignment(fpuStackReg, src));

                stmCur.Instruction = stmCur.Instruction.Accept(this);
            }
        }

        private List<CallBinding> CreateFpuStackTemporaryBindings(
            ProcedureFlow calleeFlow,
            Expression callee,
            int fpuStackDelta)
        {
            var fpuDefs = new List<CallBinding>();
            foreach (var def in calleeFlow.Trashed.OfType<FpuStackStorage>()
                .Where(def => def.FpuStackOffset >= fpuStackDelta))
            {
                var name = $"rRet{def.FpuStackOffset - fpuStackDelta}";
                var id = ssa.Procedure.Frame.CreateTemporary(
                    name, PrimitiveType.Word64); //$TODO: datatype?
                var fpuDefId = NewDef(id, callee, false);
                fpuDefs.Add(new CallBinding(def, fpuDefId));
            }
            return fpuDefs;
        }

        private void InsertFpuStackAccessAssignments(List<CallBinding> fpuDefs)
        {
            foreach(var fpuDef in fpuDefs)
            {
                var fpuStackStorage = (FpuStackStorage)fpuDef.Storage;
                var fpuAccess = arch.CreateFpuStackAccess(
                    ssa.Procedure.Frame,
                    fpuStackStorage.FpuStackOffset,
                    PrimitiveType.Word64); //$TODO: datatype?
                var iCur = stmCur.Block.Statements.IndexOf(stmCur);
                stmCur = stmCur.Block.Statements.Insert(
                    iCur + 1,
                    stmCur.LinearAddress,
                    new Store(fpuAccess, fpuDef.Expression));
                stmCur.Instruction = stmCur.Instruction.Accept(this);
            }
        }

        private void GenerateUseDefsForUnknownCallee(CallInstruction ci)
        {
            var existingUses = ci.Uses
                .Select(u => u.Storage)
                .ToHashSet();
            var existingDefs = ci.Definitions
                .Select(d => d.Storage)
                .ToHashSet();
            var trashedRegisters = program.Platform.CreateTrashedRegisters();
            var stackDepth = this.GetStackDepthAtCall(ssa.Procedure, ci);
            var frame = ssa.Procedure.Frame;

            // Hell node implementation - use and define all variables.

            // We're making a guess here. Take all registers defined in the 
            // current basic block prior to the call, then keep only those
            // which are used in the calling convention. 
            // If the guess is wrong, the user can correct it with a 
            // decompilation directive.

            var ids = GuessParameterIdentifiers(ci, stmCur, stackDepth)
                .Concat(ssa.Procedure.EntryBlock.Statements
                    .Select(s => s.Instruction)
                    .OfType<DefInstruction>()
                    .Select(d => d.Identifier)
                    .Where(i => ssa.Identifiers[i].Uses.Count > 0))
                // Stack pointers should be added to uses list.
                // They are required for reconstruction of signature of
                // indirect calls by IndirectCallRewriter
                .Concat(
                    frame.Identifiers.Where(id =>
                        id.Storage == arch.StackRegister ||
                        id.Storage == arch.FpuStackRegister));
            ids = CollectFlagGroups(ids, frame, arch);
            foreach (Identifier id in ResolveOverlaps(ids))
            {
                var calleeStg = FrameShift(ci, id.Storage, stackDepth);
                if (!existingUses.Contains(calleeStg) &&
                    (calleeStg is RegisterStorage ||
                     calleeStg is StackArgumentStorage))
                {
                    var idNew = NewUse(id, stmCur, true);
                    ci.Uses.Add(new CallBinding(calleeStg, idNew));
                    existingUses.Add(calleeStg);
                }
            }

            ids = SeparateSequences(frame.Identifiers);
            ids = CollectFlagGroups(frame.Identifiers, frame, arch);
            foreach (Identifier id in ResolveOverlaps(ids))
            {
                var calleeStg = FrameShift(ci, id.Storage, stackDepth);
                if (!existingDefs.Contains(calleeStg) &&
                    (IsTrashed(trashedRegisters, calleeStg)
                    || calleeStg is FlagGroupStorage))
                {
                    ci.Definitions.Add(new CallBinding(
                        calleeStg,
                        NewDef(id, ci.Callee, false)));
                    existingDefs.Add(calleeStg);
                }
            }
        }

        /// <summary>
        /// Make an informed guess of what identifiers are being used an input 
        /// parameters. The tactic is to walk backwards from the call point and 
        /// collect assignments to processor registers or stack locations until
        /// we reach the beginning of the block or an instruction that isn't 
        /// an assignment or store.
        /// </summary>
        /// <remarks>
        /// This is just a guess; we can't do much better without more information from the user.
        /// </remarks>
        /// <param name="procedure"></param>
        /// <param name="stmCall"></param>
        /// <returns></returns>
        public IEnumerable<Identifier> GuessParameterIdentifiers(CallInstruction call, Statement stmCall, int stackDepth)
        {
            var ids = new List<Identifier>();
            var stms = stmCall.Block.Statements;
            var i = stms.IndexOf(stmCall) - 1;
            for (; i >= 0; --i)
            {
                var stm = stms[i];
                switch (stm.Instruction)
                {
                case Assignment ass:
                    // Avoid using the callee as an argument,
                    // it's unlikely that real code ever does this,
                    // and currently causes the type inference phase 
                    // serious problems (deeply recursive function pointers)
                    if (call.Callee == ass.Dst)
                        continue;
                    switch (ass.Dst.Storage)
                    {
                    case RegisterStorage reg:
                        ids.Add(ass.Dst);
                        break;
                    case SequenceStorage seq:
                        ids.Add(ass.Dst);
                        break;
                    case StackStorage stk:
                        var calleeStg = FrameShift(call, stk, stackDepth);
                        ids.Add(ass.Dst);
                        break;
                    }
                    break;
                case Store store:
                    break;
                default:
                    return ids;
                }
            }
            return ids;
        }

        private int GetStackDepthAtCall(Procedure proc, CallInstruction call)
        {
            var sp = call.Uses.FirstOrDefault(u => u.Storage == proc.Architecture.StackRegister);
            if (sp == null)
                return 0;
            if (sp.Expression is Identifier fp && fp.Storage == proc.Frame.FramePointer.Storage)
                return 0;
            if (sp.Expression is BinaryExpression bin &&
                bin.Left is Identifier fpLeft && fpLeft.Storage == proc.Frame.FramePointer.Storage &&
                bin.Right is Constant cRight)
            {
                if (bin.Operator == Operator.IAdd)
                {
                    return cRight.ToInt32();
                }
                else if (bin.Operator == Operator.ISub)
                {
                    return -cRight.ToInt32();
                }
            }
            // Give up.
            return 0;
        }

        private Storage FrameShift(CallInstruction call, Storage callerStorage, int spDepth)
        {
            if (callerStorage is StackStorage stgArg)
            {
                return new StackArgumentStorage(
                    stgArg.StackOffset - spDepth + call.CallSite.SizeOfReturnAddressOnStack,
                    stgArg.DataType);
            }
            return callerStorage;
        }

        private bool IsTrashed(
            HashSet<RegisterStorage> trashedRegisters,
            Storage stg)
        {
            if (!(stg is RegisterStorage) || stg is TemporaryStorage)
                return false;
            // If the platform has no clue what registers may be affected by call,
            // assume all are.
            if (trashedRegisters.Count == 0)
                return true;
            return trashedRegisters.Where(r => r.OverlapsWith(stg)).Any();
        }

        private ProcedureBase GetCalleeProcedure(CallInstruction ci)
        {
            if (ci.Callee is Identifier id)
            {
                if (ssa.Identifiers[id].DefExpression is ProcedureConstant pc)
                    return pc.Procedure;
            }
            else if (ci.Callee is ProcedureConstant pc2)
            {
                return pc2.Procedure;
            }
            return null;
        }

        public override Instruction TransformDefInstruction(DefInstruction def)
        {
            return def;
        }

        public override Instruction TransformStore(Store store)
        {
            store.Src = store.Src.Accept(this);
            var exp = TransformLValue(store.Dst, store.Src);
            if (exp is Identifier idDst)
            {
                return new Assignment(idDst, store.Src);
            }
            else
            {
                store.Dst = exp;
                return store;
            }
        }

        private Expression TransformLValue(Expression exp, Expression src)
        { 
            if (exp is MemoryAccess acc)
            {
                if (this.RenameFrameAccesses && IsFrameAccess(ssa.Procedure, acc.EffectiveAddress))
                {
                    if (acc is SegmentedAccess segacc)
                    {
                        ssa.Identifiers[(Identifier)segacc.BasePointer].Uses.Remove(stmCur);
                    }
                    ssa.Identifiers[ssa.Procedure.Frame.FramePointer].Uses.Remove(stmCur);
                    ssa.Identifiers[acc.MemoryId].Uses.Remove(stmCur);
                    ssa.Identifiers[acc.MemoryId].DefStatement = null;
                    var idFrame = EnsureStackVariable(ssa.Procedure, acc.EffectiveAddress, acc.DataType);
                    var idDst = NewDef(idFrame, src, false);
                    return idDst;
                }
                else if (this.RenameFrameAccesses && IsConstFpuStackAccess(acc))
                {
                    ssa.Identifiers[acc.MemoryId].DefStatement = null;
                    var idFrame = ssa.Procedure.Frame.EnsureFpuStackVariable(((Constant)acc.EffectiveAddress).ToInt32(), acc.DataType);
                    var idDst = NewDef(idFrame, src, false);
                    return idDst;
                }
                else
                {
                    Expression basePtr = null;
                    if (acc is SegmentedAccess sa)
                    {
                        basePtr = sa.BasePointer.Accept(this);
                    }
                    var ea = acc.EffectiveAddress.Accept(this);
                    var memId = acc.MemoryId;
                    if (!this.RenameFrameAccesses)
                        memId = UpdateMemoryIdentifier(memId, true);
                    if (basePtr != null)
                        return new SegmentedAccess(memId, basePtr, ea, acc.DataType);
                    return new MemoryAccess(memId, ea, acc.DataType);
                }
            }
            else
            {
                return exp.Accept(this);
            }
        }

        public override Instruction TransformUseInstruction(UseInstruction u)
        {
            if (u.OutArgument != null && !RenameFrameAccesses)
            {
                var sidOut = ssa.Identifiers.Add(u.OutArgument, null, null, false);
                sidOut.DefStatement = stmCur;
                u.OutArgument = sidOut.Identifier;
            }
            return base.TransformUseInstruction(u);
        }

        public override Expression VisitApplication(Application appl)
        {
            var args = new Expression[appl.Arguments.Length];
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                if (appl.Arguments[i] is OutArgument outArg &&
                    outArg.Expression is Identifier id)
                {
                    var idOut = NewDef(id, appl, true);
                    outArg = new OutArgument(outArg.DataType, idOut);
                    args[i] = outArg;
                    ssa.Identifiers[idOut].DefExpression = outArg;
                }
                else
                {
                    args[i] = appl.Arguments[i].Accept(this);
                }
            }

            var proc = appl.Procedure.Accept(this);
            if (proc is ProcedureConstant pc)
            {
                blockstates[block].Terminates |= ProcedureTerminates(pc.Procedure);
            }
            return new Application(proc, appl.DataType, args);
        }

        private bool ProcedureTerminates(ProcedureBase proc)
        {
            if (proc.Characteristics != null && proc.Characteristics.Terminates)
                return true;
            return
                proc is Procedure callee &&
                programFlow.ProcedureFlows.TryGetValue(callee, out ProcedureFlow pflow) &&
                pflow.TerminatesProcess;
        }

        public override Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            // Handling sub(R,R) and xor(R,R) specially, since they otherwise
            // introduce a false use of R.
            if (binExp.Operator == Operator.ISub || binExp.Operator == Operator.Xor)
            {
                if (binExp.Left is Identifier id && binExp.Right == id)
                {
                    var c = Constant.Zero(id.DataType);
                    return c;
                }
            }
            return base.VisitBinaryExpression(binExp);
        }

        public override Expression VisitIdentifier(Identifier id)
        {
            return NewUse(id, stmCur, false);
        }

        public override Expression VisitOutArgument(OutArgument outArg)
        {
            Expression exp;
            if (outArg.Expression is Identifier id)
            {
                if (RenameFrameAccesses)
                {
                    exp = id;
                }
                else
                {
                    exp = NewDef(id, outArg, true);
                }
            }
            else
            {
                exp = outArg.Expression.Accept(this);
            }
            return new OutArgument(outArg.DataType, exp);
        }

        public Identifier NewDef(Identifier idOld, Expression src, bool isSideEffect)
        {
            if (idOld != null && ssa.Identifiers.TryGetValue(idOld, out var sidOld))
            {
                if (sidOld.OriginalIdentifier != sidOld.Identifier)
                {
                    // Already renamed by a previous pass.
                    return sidOld.Identifier;
                }
            }
            var sid = ssa.Identifiers.Add(idOld, stmCur, src, isSideEffect);
            var bs = blockstates[block];
            var x = factory.Create(idOld, stmCur);
            return x.WriteVariable(bs, sid);
        }

        private Expression NewUse(Identifier id, Statement stm, bool force)
        {
            if (RenameFrameAccesses && !force)
                return id;
            var bs = blockstates[block];
            var x = factory.Create(id, stm);
            var sid = x.ReadVariable(bs);
            sid.Uses.Add(stm);
            return sid.Identifier;
        }

        public override Expression VisitMemoryAccess(MemoryAccess access)
        {
            if (this.RenameFrameAccesses)
            {
                if (IsFrameAccess(ssa.Procedure, access.EffectiveAddress))
                {
                    ssa.Identifiers[access.MemoryId].Uses.Remove(stmCur);
                    ssa.Identifiers[ssa.Procedure.Frame.FramePointer].Uses.Remove(stmCur);
                    var idFrame = EnsureStackVariable(ssa.Procedure, access.EffectiveAddress, access.DataType);
                    var idNew = NewUse(idFrame, stmCur, true);
                    return idNew;
                }
                if (IsConstFpuStackAccess(access))
                {
                    ssa.Identifiers[access.MemoryId].Uses.Remove(stmCur);
                    var idFrame = ssa.Procedure.Frame.EnsureFpuStackVariable(
                        ((Constant)access.EffectiveAddress).ToInt32(), access.DataType);
                    var idNew = NewUse(idFrame, stmCur, true);
                    return idNew;
                }
            }

            var ea = access.EffectiveAddress.Accept(this);
            if (ea is BinaryExpression bin &&
                bin.Left is Identifier id && 
                bin.Right is Constant c)
            {
                var sid = ssa.Identifiers[id];
                if (sid.DefExpression is Constant cOther)
                {
                    // Replace id + c  where id = cOther with c
                    c = bin.Operator.ApplyConstants(cOther, c);
                    sid.Uses.Remove(stmCur);
                }
                else
                {
                    c = null;
                }
            }
            else
            {
                c = ea as Constant;
            }

            if (c != null &&
                // Search imported procedures only in Global Memory
                access.MemoryId.Storage == MemoryStorage.Instance)
            {
                var e = dynamicLinker.ResolveToImportedValue(stmCur, c);
                if (e != null)
                    return e;
                ea = c;
            }
            var memId = UpdateMemoryIdentifier(access.MemoryId, false);
            return new MemoryAccess(memId, ea, access.DataType);
        }

        public override Expression VisitSegmentedAccess(SegmentedAccess access)
        {
            if (this.RenameFrameAccesses && IsFrameAccess(ssa.Procedure, access.EffectiveAddress))
            {
                ssa.Identifiers[access.MemoryId].Uses.Remove(stmCur);
                ssa.Identifiers[(Identifier)access.BasePointer].Uses.Remove(stmCur);
                ssa.Identifiers[ssa.Procedure.Frame.FramePointer].Uses.Remove(stmCur);
                var idFrame = EnsureStackVariable(ssa.Procedure, access.EffectiveAddress, access.DataType);
                var idNew = NewUse(idFrame, stmCur, true);
                return idNew;
            }
            else
            {
                var basePtr = access.BasePointer.Accept(this);
                var ea = access.EffectiveAddress.Accept(this);
                var memId = (MemoryIdentifier)NewUse(access.MemoryId, stmCur, false);
                return new SegmentedAccess(memId, basePtr, ea, access.DataType);
            }
        }

        private MemoryIdentifier UpdateMemoryIdentifier(MemoryIdentifier memId, bool storing)
        {
            if (storing)
            {
                var sid = ssa.Identifiers.Add(memId, this.stmCur, null, false);
                var ss = new RegisterTransformer(memId, stmCur, this);
                return (MemoryIdentifier)ss.WriteVariable(blockstates[block], sid);
            }
            else
            {
                return (MemoryIdentifier)memId.Accept(this);
            }
        }

        private static bool IsFrameAccess(Procedure proc, Expression e)
        {
            if (e == proc.Frame.FramePointer)
                return true;
            if (!(e is BinaryExpression bin))
                return false;
            if (bin.Left != proc.Frame.FramePointer)
                return false;
            return bin.Right is Constant;
        }

        private bool IsConstFpuStackAccess(MemoryAccess acc)
        {
            if (!acc.MemoryId.Name.StartsWith("ST"))  //$HACK: gross hack but we have to start somewhere.
                return false;
            return acc.EffectiveAddress is Constant;
        }

        private static Identifier EnsureStackVariable(Procedure proc, Expression effectiveAddress, DataType dt)
        {
            if (effectiveAddress == proc.Frame.FramePointer)
                return proc.Frame.EnsureStackVariable(0, dt);
            var bin = (BinaryExpression)effectiveAddress;
            var offset = ((Constant)bin.Right).ToInt32();
            if (bin.Operator == Operator.ISub)
                offset = -offset;
            var idFrame = proc.Frame.EnsureStackVariable(offset, dt);
            return idFrame;
        }

        private void ProcessIncompletePhis()
        {
            while (incompletePhis.Count > 0)
            {
                var work = incompletePhis.ToArray();
                incompletePhis.Clear();
                foreach (var phi in work)
                {
                    var phiBlock = phi.DefStatement.Block;
                    var x = factory.Create(phi.OriginalIdentifier, phi.DefStatement);
                    x.AddPhiOperands(phi);
                }
            }
        }

        public class SsaBlockState
        {
            public readonly Block Block;
            public readonly Dictionary<StorageDomain, AliasState> currentDef;
            public readonly IntervalTree<int, Alias> currentStackDef;
            public readonly Dictionary<StorageDomain, FlagAliasState> currentFlagDef;
            public readonly Dictionary<Storage, SsaIdentifier> currentSimpleDef;
            public bool Visited;
            public bool Terminates;

            public SsaBlockState(Block block)
            {
                this.Block = block;
                this.Visited = false;
                this.currentDef = new Dictionary<StorageDomain, AliasState>();
                this.currentStackDef = new IntervalTree<int, Alias>();
                this.currentFlagDef = new Dictionary<StorageDomain, FlagAliasState>();
                this.currentSimpleDef = new Dictionary<Storage, SsaIdentifier>();
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("BlockState {0}", Block.Name);
                sb.AppendLine();
                sb.AppendFormat("    {0}",
                    string.Join(",", currentDef.Keys.Select(k => ((int)k).ToString())));
                return sb.ToString();
            }
        }

        /// <summary>
        /// Describes a set of register identifiers that alias each other in a particular block.
        /// </summary>
        public class AliasState
        {
            /// <summary>
            /// List of defined identifiers, in chronological order (i.e. the
            /// most recent definition is last in the list.
            /// </summary>
            public List<(SsaIdentifier, BitRange, int)> Definitions;

            /// <summary>
            /// Exact aliases are used to speed up lookups.
            /// </summary>
            public Dictionary<Storage, SsaIdentifier> ExactAliases;

            public AliasState()
            {
                this.Definitions = new List<(SsaIdentifier, BitRange, int)>();
                this.ExactAliases = new Dictionary<Storage, SsaIdentifier>();
            }
        }

        public class FlagAliasState
        {
            public List<(SsaIdentifier, uint)> Definitions;
            public Dictionary<Identifier, SsaIdentifier> ExactAliases;

            public FlagAliasState()
            {
                this.Definitions = new List<(SsaIdentifier, uint)>();
                this.ExactAliases = new Dictionary<Identifier, SsaIdentifier>();
            }
        }

        public class Alias
        {
            public SsaIdentifier SsaId;
        }

        public class TransformerFactory : StorageVisitor<IdentifierTransformer>
        {
            private readonly SsaTransform transform;
            private Identifier id;
            private Statement stm;

            public TransformerFactory(SsaTransform transform)
            {
                this.transform = transform;
            }

            public IdentifierTransformer Create(Identifier id, Statement stm)
            {
                this.id = id;
                this.stm = stm;
                return id.Storage.Accept(this);
            }

            public IdentifierTransformer VisitFlagGroupStorage(FlagGroupStorage grf)
            {
                return new FlagGroupTransformer(id, grf, stm, transform);
            }

            public IdentifierTransformer VisitFpuStackStorage(FpuStackStorage fpu)
            {
                return new SimpleTransformer(id, fpu, stm, transform);
            }

            public IdentifierTransformer VisitMemoryStorage(MemoryStorage global)
            {
                return new RegisterTransformer(id, stm, transform);
            }

            public IdentifierTransformer VisitOutArgumentStorage(OutArgumentStorage arg)
            {
                throw new NotImplementedException();
            }

            public IdentifierTransformer VisitRegisterStorage(RegisterStorage reg)
            {
                return new RegisterTransformer(id, stm, transform);
            }

            public IdentifierTransformer VisitSequenceStorage(SequenceStorage seq)
            {
                return new SequenceTransformer(id, seq, stm, transform);
            }

            public IdentifierTransformer VisitStackArgumentStorage(StackArgumentStorage stack)
            {
                return new StackTransformer(id, stack.StackOffset, stm, transform);
            }

            public IdentifierTransformer VisitStackLocalStorage(StackLocalStorage local)
            {
                return new StackTransformer(id, local.StackOffset, stm, transform);
            }

            public IdentifierTransformer VisitTemporaryStorage(TemporaryStorage temp)
            {
                return new SimpleTransformer(id, temp, stm, transform);
            }
        }
    }
}
