#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
	/// Transforms a <see cref="Reko.Core.Procedure"/> to Static Single Assignment
    /// form.
	/// </summary>
    /// <remarks>
    /// This class implements an SSA algorithm that doesn't require 
    /// calculation of the dominator graph. It is based on the algorithm
    /// described in "Simple and Efficient Construction of Static Single
    /// Assignment Form" by Matthias Braun, Sebastian Buchwald, Sebastian 
    /// Hack, Roland LeiЯa, Christoph Mallon, and Andreas Zwinkau. 
    /// It has been augmented with storage alias analysis, and could be
    /// augmented with expression simplification if we can prove that the
    /// CFG graph is completed (future work).
    /// </remarks>
    public class SsaTransform : InstructionTransformer 
    {
        private static TraceSwitch trace = new TraceSwitch("SsaTransform", "Traces the progress of SSA analysis") { Level = TraceLevel.Info };

        private IProcessorArchitecture arch;
        private Program program;
        private ProgramDataFlow programFlow;
        private IImportResolver importResolver;
        private Block block;
        private Statement stmCur;
        private Dictionary<Block, SsaBlockState> blockstates;
        private SsaState ssa;
        private TransformerFactory factory;
        public readonly HashSet<SsaIdentifier> incompletePhis;
        private HashSet<SsaIdentifier> sidsToRemove;
        private HashSet<Procedure> sccProcs;
        private ExpressionEmitter m;

        public SsaTransform(
            Program program,
            Procedure proc,
            HashSet<Procedure> sccProcs,
            IImportResolver importResolver,
            ProgramDataFlow programFlow)
        {
            this.arch = program.Architecture;
            this.program = program;
            this.programFlow = programFlow;
            this.importResolver = importResolver;
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
            this.sidsToRemove = new HashSet<SsaIdentifier>();
            foreach(var bs in blockstates.Values)
            {
                bs.Visited = false;
            }

            // Visit blocks in RPO order so that we are guaranteed that a 
            // block with predecessors is always visited after them.

            foreach (Block b in new DfsIterator<Block>(ssa.Procedure.ControlGraph).ReversePostOrder())
            {
                this.block = b;
                foreach (var s in b.Statements.ToList())
                {
                    this.stmCur = s;
                    s.Instruction = s.Instruction.Accept(this);
                    if (blockstates[b].terminates)
                    {
                        TerminateBlockAfterStatement(b, s);
                        break;
                    }
                }
                blockstates[b].Visited = true;
                blockstates[b].terminates = false;
            }
            ProcessIncompletePhis();
            RemoveDeadSsaIdentifiers();
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
            DebugEx.PrintIf(trace.TraceVerbose, "AddUsesToExitBlock");
            stms.ForEach(u =>
            {
                var use = (UseInstruction)u.Instruction;
                use.Expression = NewUse((Identifier)use.Expression, u, true);
            });
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

        public IEnumerable<Identifier> SeparateSequences(IEnumerable<Identifier> ids)
        {
            foreach (var id in ids)
            {
                if (id.Storage is SequenceStorage seq)
                {
                    yield return ssa.Procedure.Frame.EnsureIdentifier(seq.Head);
                    yield return ssa.Procedure.Frame.EnsureIdentifier(seq.Tail);
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
        /// indirect jump that prior Reko passes have been unable to resolve),
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
                var spDepth = GetStackDepthAtCall(ssa.Procedure, ci);
                var ab = arch.CreateFrameApplicationBuilder(ssa.Procedure.Frame, ci.CallSite, ci.Callee);
                foreach (var stgUse in calleeFlow.BitsUsed.Keys)
                {
                    if (stgUse is FpuStackStorage fpuUse)
                    {
                        var fpuUseExpr = arch.CreateFpuStackAccess(
                            ssa.Procedure.Frame,
                            fpuUse.FpuStackOffset,
                            PrimitiveType.Word64); //$TODO: datatype?
                        fpuUseExpr = fpuUseExpr.Accept(this);
                        ci.Uses.Add(
                            new CallBinding(
                                stgUse,
                                fpuUseExpr));
                    }
                    else
                    {
                        var arg = stgUse.Accept(ab);
                        arg = arg.Accept(this);
                        ci.Uses.Add(new CallBinding(stgUse, arg));
                    }
                }

                if (calleeFlow.TerminatesProcess)
                {
                    // We just discovered this basic block doesn't terminate.
                    this.blockstates[block].terminates = true;
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

            // Hell node implementation - use and define all variables.
            var frame = ssa.Procedure.Frame;
            var stackDepth = this.GetStackDepthAtCall(ssa.Procedure, ci);
            var ids = CollectFlagGroups(frame.Identifiers, frame, arch);
            foreach (Identifier id in ResolveOverlaps(ids))
            {
                var calleeStg = FrameShift(ci, id.Storage, stackDepth);
                if (!existingUses.Contains(calleeStg) &&
                    (calleeStg is RegisterStorage ||
                     calleeStg is StackArgumentStorage))
                {
                    ci.Uses.Add(new CallBinding(
                        calleeStg,
                        NewUse(id, stmCur, true)));
                    existingUses.Add(calleeStg);
                }
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
                else if (this.RenameFrameAccesses && IsConstFpuStackAccess(ssa.Procedure, acc))
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
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                if (appl.Arguments[i] is OutArgument outArg &&
                    outArg.Expression is Identifier id)
                {
                    var idOut = NewDef(id, appl, true);
                    outArg = new OutArgument(outArg.DataType, idOut);
                    appl.Arguments[i] = outArg;
                    ssa.Identifiers[idOut].DefExpression = outArg;
                    continue;
                }
                appl.Arguments[i] = appl.Arguments[i].Accept(this);
            }
            appl.Procedure = appl.Procedure.Accept(this);
            if (appl.Procedure is ProcedureConstant pc)
            {
                blockstates[block].terminates |= ProcedureTerminates(pc.Procedure);
            }
            return appl;
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
            return x.NewDef(bs, sid);
        }

        private Expression NewUse(Identifier id, Statement stm, bool force)
        {
            if (RenameFrameAccesses && !force)
                return id;
            var bs = blockstates[block];
            var x = factory.Create(id, stm);
            return x.NewUse(bs);
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
                if (IsConstFpuStackAccess(ssa.Procedure, access))
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

            if (c != null)
            {
                var e = importResolver.ResolveToImportedProcedureConstant(stmCur, c);
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
                return (MemoryIdentifier)ss.WriteVariable(blockstates[block], sid, false);
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
            var bin = e as BinaryExpression;
            if (bin == null)
                return false;
            if (bin.Left != proc.Frame.FramePointer)
                return false;
            return bin.Right is Constant;
        }

        private static bool IsConstFpuStackAccess(Procedure proc, MemoryAccess acc)
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
            foreach (var phi in incompletePhis)
            {
                var phiBlock = phi.DefStatement.Block;
                var x = factory.Create(phi.OriginalIdentifier, phi.DefStatement);
                x.AddPhiOperandsCore(phi);
            }
            incompletePhis.Clear();
        }

        private Expression ReadParameter(Block b, FunctionType sig, Storage stg)
        {
            if (!sig.ParametersValid)
                return null;
            var param = sig.Parameters
                .FirstOrDefault(p => p.Storage.Covers(stg));
            if (param == null)
                return null;
            var sidParam = ssa.EnsureSsaIdentifier(param, b);
            var idParam = sidParam.Identifier;
            if (idParam.Storage.BitSize == stg.BitSize)
                return idParam;
            var dt = PrimitiveType.CreateWord((int) stg.BitSize);
            return m.Slice(dt, idParam, idParam.Storage.OffsetOf(stg));
        }

        public class SsaBlockState
        {
            public readonly Block Block;
            public readonly Dictionary<StorageDomain, AliasState> currentDef;
            public readonly IntervalTree<int, AliasState> currentStackDef;
            public readonly Dictionary<int, SsaIdentifier> currentFpuDef;
            public bool Visited;
            internal bool terminates;

            public SsaBlockState(Block block)
            {
                this.Block = block;
                this.Visited = false;
                this.currentDef = new Dictionary<StorageDomain, AliasState>();
                this.currentStackDef = new IntervalTree<int, AliasState>();
                this.currentFpuDef = new Dictionary<int, SsaIdentifier>();
            }

#if DEBUG
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("BlockState {0}", Block.Name);
                sb.AppendLine();
                sb.AppendFormat("    {0}",
                    string.Join(",", currentDef.Keys.Select(k => ((int)k).ToString())));
                return sb.ToString();
            }
#endif
        }

        /// <summary>
        /// Describes a set of identifiers that alias each other.
        /// </summary>
        public class AliasState
        {
            public SsaIdentifier SsaId;        // The id that actually was modified.
            public readonly AliasState PrevState;
            public readonly IDictionary<Identifier, SsaIdentifier> Aliases;     // Other ids that were affected by this stm.

            public AliasState(SsaIdentifier ssaId, AliasState prevState)
            {
                this.SsaId = ssaId;
                this.PrevState = prevState;
                this.Aliases = new Dictionary<Identifier, SsaIdentifier>();
            }

#if DEBUG
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Alias: {0}", SsaId.Identifier.Name);
                if (Aliases.Count > 0)
                {
                    sb.AppendFormat(" = {0}", string.Join(", ", Aliases.Values.Select(v => v.Identifier.Name).OrderBy(v => v)));
                }
                return sb.ToString();
            }
#endif
        }

        public class TransformerFactory : StorageVisitor<IdentifierTransformer>
        {
            private SsaTransform transform;
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
                return new FpuStackTransformer(id, fpu, stm, transform);
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
                return new RegisterTransformer(id, stm, transform);
            }
        }

        public abstract class IdentifierTransformer
        {
            protected Identifier id;
            protected BitRange liveBits;
            protected readonly Statement stm;
            protected readonly SsaTransform outer;
            protected readonly SsaIdentifierCollection ssaIds;
            protected readonly IDictionary<Block, SsaBlockState> blockstates;

            public IdentifierTransformer(Identifier id, Statement stm, SsaTransform outer)
            {
                this.id = id;
                this.liveBits = id.Storage.GetBitRange();
                this.stm = stm;
                this.ssaIds = outer.ssa.Identifiers;
                this.blockstates = outer.blockstates;
                this.outer = outer;
            }

            public virtual Expression NewUse(SsaBlockState bs)
            {
                var sid = ReadVariable(bs);
                sid.Uses.Add(stm);
                return sid.Identifier;
            }

            public virtual Identifier NewDef(SsaBlockState bs, SsaIdentifier sid)
            {
                return WriteVariable(bs, sid, true);
            }

            /// <summary>
            /// Registers the fact that identifier <paramref name="id"/> is
            /// modified in the block <paramref name="b" /> and generates a 
            /// fresh SSA identifier. 
            /// </summary>
            /// <param name="bs">The block in which the identifier was changed</param>
            /// <param name="sid">The identifier after being SSA transformed.</param>
            /// <param name="performProbe">if true, looks "backwards" to see
            ///   if <paramref name="id"/> overlaps with another identifier</param>
            /// <returns>The new SSA identifier</returns>
            public virtual Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                if (bs.currentDef.TryGetValue(id.Storage.Domain, out var prevState))
                {
                    while (prevState != null && id.Storage.Covers(prevState.SsaId.Identifier.Storage))
                    {
                        prevState = prevState.PrevState;
                    }
                }
                bs.currentDef[id.Storage.Domain] = new AliasState(sid, prevState);
                return sid.Identifier;
            }

            /// <summary>
            /// Reaches "backwards" to locate the SSA identifier that defines
            /// the identifier <paramref name="id"/>, starting in block <paramref name="b"/>.
            /// </summary>
            /// If no definition of <paramref name="id"/> is found, a new 
            /// DefStatement is created in the entry block of the procedure.
            /// </summary>
            /// <param name="bs"></param>
            /// <returns>The SSA name of the identifier that was read.</returns>
            public virtual SsaIdentifier ReadVariable(SsaBlockState bs)
            {
                var sid = ReadBlockLocalVariable(bs);
                if (sid != null)
                    return sid;
                // Keep probin'.
                return ReadVariableRecursive(bs);
            }


            public abstract SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs);

            public SsaIdentifier ReadVariableRecursive(SsaBlockState bs)
            {
                SsaIdentifier val;
                if (false)  // !sealedBlocks.Contains(b))
                {
                    // Incomplete CFG
                    //val = NewPhi(id, bs.Block);
                    //incompletePhis[b][id.Storage] = val;
                }
                else if (bs.Block.Pred.Count == 0)
                {
                    // Undef'ined or unreachable parameter; assume it's a def.
                    val = NewDefInstruction(id, bs.Block);
                }
                else if (bs.Block.Pred.Count == 1)
                {
                    // Search for the variable in the single predecessor.
                    val = ReadVariable(blockstates[bs.Block.Pred[0]]);
                }
                else
                {
                    // Break potential cycles with operandless phi
                    val = NewPhi(id, bs.Block);
                    WriteVariable(bs, val, false);
                    val = AddPhiOperands(val);
                }
                if (val != null)
                    WriteVariable(bs, val, false);
                return val;
            }

            /// <summary>
            /// If <paramref name="idTo"/> is smaller than <paramref name="sidFrom" />, then
            /// it doesn't cover it completely. Therefore, we must generate a SLICE / cast 
            /// statement.
            /// </summary>
            /// <param name="idTo"></param>
            /// <param name="sidFrom"></param>
            /// <returns></returns>
            protected SsaIdentifier MaybeGenerateAliasStatement(AliasState aliasFrom, SsaBlockState bsTo)
            {
                var sidFrom = aliasFrom.SsaId;
                var blockFrom = sidFrom.DefStatement.Block;
                var stgFrom = sidFrom.Identifier.Storage;
                var stgTo = id.Storage;
                DebugEx.PrintIf(trace.TraceVerbose, "  MaybeGenerateAliasStatement({0},{1})", sidFrom.Identifier.Name, id.Name);

                if (stgFrom == stgTo)
                {
                    aliasFrom.Aliases[id] = sidFrom;
                    return aliasFrom.SsaId;
                }

                Expression e = null;
                SsaIdentifier sidUse;
                if (stgFrom.Covers(stgTo))
                {
                    // Defined identifer is "wider" than the storage
                    // being read.
                    int offset = stgFrom.OffsetOf(stgTo);
                    if (offset > 0)
                        e = new Slice(id.DataType, sidFrom.Identifier, offset);
                    else
                        e = new Cast(id.DataType, sidFrom.Identifier);
                    sidUse = aliasFrom.SsaId;
                }
                else if (aliasFrom.PrevState != null && aliasFrom.PrevState.SsaId.DefStatement != null)
                {
                    // There is a previous alias, try using that.
                    sidUse = MaybeGenerateAliasStatement(aliasFrom.PrevState, bsTo);
                    e = new DepositBits(sidUse.Identifier, aliasFrom.SsaId.Identifier, (int)stgFrom.BitAddress);
                }
                else 
                {
                    this.liveBits = this.liveBits - stgFrom.GetBitRange();
                    DebugEx.PrintIf(trace.TraceVerbose, "  MaybeGenerateAliasStatement proceeding to {0}", blockFrom.Name);
                    sidUse = ReadVariableRecursive(bsTo);
                    e = new DepositBits(sidUse.Identifier, aliasFrom.SsaId.Identifier, (int)stgFrom.BitAddress);
                }
                var ass = new AliasAssignment(id, e);
                var sidAlias = InsertAfterDefinition(sidFrom.DefStatement, ass);
                sidUse.Uses.Add(sidAlias.DefStatement);
                if (e is DepositBits)
                    sidFrom.Uses.Add(sidAlias.DefStatement);
                aliasFrom.Aliases[id] = sidAlias;
                return sidAlias;
            }


            /// <summary>
            /// Inserts the statement <paramref name="ass"/> after the statement
            /// <paramref name="stmBefore"/>, skipping any AliasAssignments that
            /// statements that may have been added after 
            /// <paramref name="stmBefore"/>.
            /// </summary>
            /// <param name="stmBefore"></param>
            /// <param name="ass"></param>
            /// <returns></returns>
            public SsaIdentifier InsertAfterDefinition(Statement stmBefore, AliasAssignment ass)
            {
                var b = stmBefore.Block;
                int i = b.Statements.IndexOf(stmBefore);
                // Skip alias statements
                while (i < b.Statements.Count - 1 && b.Statements[i + 1].Instruction is AliasAssignment)
                    ++i;
                var stm = new Statement(stmBefore.LinearAddress, ass, stmBefore.Block);
                stmBefore.Block.Statements.Insert(i + 1, stm);

                var sidTo = ssaIds.Add(ass.Dst, stm, ass.Src, false);
                ass.Dst = sidTo.Identifier;
                return sidTo;
            }

            /// <summary>
            /// Creates a phi statement with no slots for the predecessor blocks, then
            /// inserts the phi statement as the first statement of the block.
            /// </summary>
            /// <param name="b">Block into which the phi statement is inserted</param>
            /// <param name="v">Destination variable for the phi assignment</param>
            /// <returns>The inserted phi Assignment</returns>
            private SsaIdentifier NewPhi(Identifier id, Block b)
            {
                var phiAss = new PhiAssignment(id, 0);
                var stm = new Statement(b.Address.ToLinear(), phiAss, b);
                b.Statements.Insert(0, stm);
                var sid = ssaIds.Add(phiAss.Dst, stm, phiAss.Src, false);
                phiAss.Dst = sid.Identifier;
                return sid;
            }

            private SsaIdentifier AddPhiOperands(SsaIdentifier phi)
            {
                // Determine operands from predecessors.
                var preds = phi.DefStatement.Block.Pred;

                if (preds.Any(p => !blockstates[p].Visited))
                {
                    // Haven't visited some of the predecessors yet,
                    // so we can't backwalk... yet. 
                    ((PhiAssignment)phi.DefStatement.Instruction).Src =
                                new PhiFunction(phi.Identifier.DataType, new Expression[preds.Count]);
                    outer.incompletePhis.Add(phi);
                    return phi;
                }
                return AddPhiOperandsCore(phi);
            }

            public SsaIdentifier AddPhiOperandsCore(SsaIdentifier phi)
            {
                var preds = phi.DefStatement.Block.Pred;
                var sids = preds.Select(p => ReadVariable(blockstates[p])).ToArray();
                GeneratePhiFunction(phi, sids);

                if (!TryRemoveTrivial(phi, out var newSid))
                {
                    // A real phi; use all its arguments.
                    UsePhiArguments(phi);
                    return phi;
                }
                return newSid;
            }

            private static void GeneratePhiFunction(SsaIdentifier phi, SsaIdentifier[] sids)
            {
                ((PhiAssignment)phi.DefStatement.Instruction).Src =
                new PhiFunction(
                        phi.Identifier.DataType,
                        sids.Select(s => s.Identifier).ToArray());
            }

            /// <summary>
            /// If the phi function is trivial, remove it.
            /// </summary>
            /// <param name="phi">SSA identifier of phi function</param>
            /// <param name="sid">
            /// Returns SSA identifier for simplified phi function if it was
            /// trivial, otherwise returns null
            /// </param>
            /// <returns>true if the phi function was trivial</returns>
            private bool TryRemoveTrivial(SsaIdentifier phi, out SsaIdentifier sid)
            {
                Identifier same = null;
                var phiFunc = ((PhiAssignment)phi.DefStatement.Instruction).Src;
                DebugEx.PrintIf(trace.TraceVerbose, "  Checking {0} for triviality", phiFunc);
                foreach (Identifier op in phiFunc.Arguments)
                {
                    if (op == same || op == phi.Identifier)
                        continue;
                    if (same != null)
                    {
                        sid = null;
                        return false;
                    }
                    same = op;
                }
                if (same == null)
                {
                    DebugEx.PrintIf(trace.TraceVerbose, "  {0} is a def", phi.Identifier);
                    // Undef'ined or unreachable parameter; assume it's a def.
                    sid = NewDefInstruction(phi.OriginalIdentifier, phi.DefStatement.Block);
                }
                else
                {
                    sid = ssaIds[same];
                }

                // Remember all users except for phi
                var users = phi.Uses.Where(u => u != phi.DefStatement).ToList();

                // Reroute all uses of phi to use same. Remove phi.
                ReplaceBy(phi, sid);

                sid.Uses.RemoveAll(u => u == phi.DefStatement);

                // Remove all phi uses which may have become trivial now.
                DebugEx.PrintIf(trace.TraceVerbose, "Removing {0} and uses {1}", phi.Identifier.Name, string.Join(",", users));
                foreach (var use in users)
                {
                    if (use.Instruction is PhiAssignment phiAss)
                    {
                        var sidU = ssaIds[phiAss.Dst];
                        if (TryRemoveTrivial(sidU, out var sidNew) &&
                            sidU == sid)
                        {
                            same = sidNew.Identifier;
                            sid = sidNew;
                        }
                    }
                }
                if (blockstates[phi.DefStatement.Block].currentDef.TryGetValue(same.Storage.Domain, out var alias))
                {
                    alias.SsaId = outer.ssa.Identifiers[same];
                }
                phi.DefStatement.Block.Statements.Remove(phi.DefStatement);
                this.outer.sidsToRemove.Add(phi);
                return true;
            }

            private void UsePhiArguments(SsaIdentifier phi)
            {
                var phiFunc = ((PhiAssignment)phi.DefStatement.Instruction).Src;
                foreach (Identifier id in phiFunc.Arguments)
                {
                    ssaIds[id].Uses.Add(phi.DefStatement);
                }
            }

            /// <summary>
            /// Generate a 'def' instruction for identifiers that are used
            /// without any previous definitions inside the current procedure.
            /// </summary>
            /// <remarks>
            /// The generated def statements show us what identifiers are live-in
            /// to the procedure. Some of the generated def statements are later
            /// eliminated. Typically this is because the identifiers are copied
            /// to the stack and the restored on exit.
            /// </remarks>
            /// <param name="id">Identifier whose definition we wish to generate.</param>
            /// <param name="b">The entry block of the procedure.</param>
            /// <returns></returns>
            public SsaIdentifier NewDefInstruction(Identifier id, Block b)
            {
                var sig = outer.ssa.Procedure.Signature;
                var param = outer.ReadParameter(b, sig, id.Storage);
                if (param != null)
                {
                    var copy = new Assignment(id, param);
                    var stmCopy = b.Statements.Add(b.Address.ToLinear(), copy);
                    var sidCopy = ssaIds.Add(id, stmCopy, null, false);
                    copy.Dst = sidCopy.Identifier;
                    sidCopy.DefExpression = param;

                    outer.ssa.AddUses(stmCopy);
                    return sidCopy;
                }
                return outer.ssa.EnsureSsaIdentifier(id, b);
            }

            private void ReplaceBy(SsaIdentifier sidOld, SsaIdentifier idNew)
            {
                foreach (var use in sidOld.Uses.ToList())
                {
                    use.Instruction.Accept(new IdentifierReplacer(this.ssaIds, use, sidOld.Identifier, idNew.Identifier, false));
                }
                foreach (var bs in outer.blockstates.Values)
                {
                    if (bs.currentDef.TryGetValue(sidOld.Identifier.Storage.Domain, out var alias))
                    {
                        if (alias.SsaId == sidOld)
                        {
                            alias.SsaId = idNew;
                        }
                    }
                    foreach (var de in bs.currentStackDef.ToList())
                    {
                        if (de.Value.SsaId == sidOld)
                        {
                            de.Value.SsaId = idNew;
                        }
                    }
                    foreach (var de in bs.currentFpuDef.ToList())
                    {
                        if (de.Value == sidOld)
                        {
                            bs.currentFpuDef[de.Key] = idNew;
                        }
                    }
                }
            }
        }

        public class RegisterTransformer : IdentifierTransformer
        {
            public RegisterTransformer(Identifier id,  Statement stm, SsaTransform outer)
                : base(id, stm, outer)
            {
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                DebugEx.PrintIf(trace.TraceVerbose, "  ReadBlockLocalVariable: ({0}, {1}, ({2})", bs.Block.Name, id, this.liveBits);
                if (!bs.currentDef.TryGetValue(id.Storage.Domain, out var alias))
                    return null;

                // Identifier id is defined locally in this block.
                // Has the alias already been calculated?
                for (var a = alias; a != null; a = a.PrevState)
                {
                    DebugEx.PrintIf(trace.TraceVerbose, "    found alias ({0}, {1}, ({2})", bs.Block.Name, a.SsaId.Identifier.Name, string.Join(",", a.Aliases.Select(aa => aa.Value.Identifier.Name)));
                    SsaIdentifier ssaId = a.SsaId;
                    if (a.SsaId.OriginalIdentifier == id ||
                        a.Aliases.TryGetValue(id, out ssaId))
                    {
                        return ssaId;
                    }

                    // Does the alias overlap the probed value?
                    if (a.SsaId.Identifier.Storage.OverlapsWith(id.Storage))
                    {
                        var sid = MaybeGenerateAliasStatement(a, bs);
                        if (sid != null)
                        {
                            bs.currentDef[id.Storage.Domain] = a;
                            return sid;
                        }
                    }
                }
                return null;
            }
        }

        public class FlagGroupTransformer : IdentifierTransformer
        {
            private uint flagMask;
            private FlagGroupStorage flagGroup;

            public FlagGroupTransformer(Identifier id, FlagGroupStorage flagGroup, Statement stm, SsaTransform outer)
                : base(id, stm, outer)
            {
                this.flagGroup = flagGroup;
                this.flagMask = flagGroup.FlagGroupBits;
            }

            private Expression OrTogether(IEnumerable<SsaIdentifier> sids, Statement stm)
            {
                Expression e = null;
                foreach (var sid in sids.OrderBy(id => id.Identifier.Name))
                {
                    sid.Uses.Add(stm);
                    if (e == null)
                        e = sid.Identifier;
                    else
                        e = new BinaryExpression(Operator.Or, PrimitiveType.Byte, e, sid.Identifier);
                }
                return e;
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                if (!bs.currentDef.TryGetValue(flagGroup.FlagRegister.Domain, out var alias))
                    return null;

                // Defined locally in this block.
                // Has the alias already been calculated?
                for (var a = alias; a != null; a = a.PrevState)
                {
                    SsaIdentifier ssaId = a.SsaId;
                    if (a.SsaId.OriginalIdentifier == id ||
                        a.Aliases.TryGetValue(id, out ssaId))
                    {
                        return ssaId;
                    }

                    // Does ssaId cover the probed value?
                    if (a.SsaId.Identifier.Storage.OverlapsWith(this.flagGroup))
                    {
                            var sid = MaybeGenerateAliasStatement(a);
                            return sid;
                    }
                }
                return null;
            }

            /// <summary>
            /// If the defining statement doesn't exactly match the bits of
            /// the using statements, we have to generate an alias assignment
            /// after the defining statement.
            /// </summary>
            /// <returns></returns>
            protected  SsaIdentifier MaybeGenerateAliasStatement(AliasState aliasFrom)
            {
                var sidFrom = aliasFrom.SsaId;
                var b = sidFrom.DefStatement.Block;
                var stgUse = id.Storage;
                var stgFrom = (FlagGroupStorage)sidFrom.Identifier.Storage;
                if (stgFrom == stgUse)
                {
                    // Exact match, no need for alias statement.
                    aliasFrom.Aliases[id] = sidFrom;
                    return sidFrom;
                }

                Expression e = null;
                SsaIdentifier sidUse;
                if (stgFrom.Covers(stgUse))
                {
                    // No merge needed, since all bits used 
                    // are defined by sidDef.
                    int offset = Bits.Log2(this.flagMask);
                    e = new Slice(PrimitiveType.Bool, sidFrom.Identifier, offset);
                    sidUse = sidFrom;
                }
                else
                {
                    // Not all bits were set by the definition, find
                    // the remaining bits by masking off the 
                    // defined ones.
                    var grf = this.flagGroup.FlagGroupBits & ~stgFrom.FlagGroupBits;
                    if (grf == 0)
                        return null;

                    var oldGrf = this.flagGroup;
                    var oldId = this.id;
                    this.flagGroup = outer.arch.GetFlagGroup(oldGrf.FlagRegister, grf);
                    this.id = outer.ssa.Procedure.Frame.EnsureFlagGroup(this.flagGroup);
                    if (aliasFrom.PrevState != null && aliasFrom.PrevState.SsaId.DefStatement != null)
                    {
                        sidUse = MaybeGenerateAliasStatement(aliasFrom.PrevState);
                    }
                    else
                    {
                        this.liveBits = this.liveBits - stgFrom.GetBitRange();
                        sidUse = ReadVariableRecursive(blockstates[aliasFrom.SsaId.DefStatement.Block]);
                    }

                    this.flagGroup = oldGrf;
                    this.id = oldId;
                    e = new BinaryExpression(
                        Operator.Or,
                        PrimitiveType.Bool,
                        sidFrom.Identifier,
                        sidUse.Identifier);
                }

                var ass = new AliasAssignment(id, e);
                var sidAlias = InsertAfterDefinition(sidFrom.DefStatement, ass);
                sidUse.Uses.Add(sidAlias.DefStatement);
                if (e is BinaryExpression)
                    sidFrom.Uses.Add(sidAlias.DefStatement);
                aliasFrom.Aliases[id] = sidAlias;
                return sidAlias;
            }
        }

        public class StackTransformer : IdentifierTransformer
        {
            private Interval<int> offsetInterval;

            public StackTransformer(
                Identifier id,
                int stackOffset,
                Statement stm,
                SsaTransform outer)
                : base(id, stm, outer)
            {
                this.offsetInterval = Interval.Create(
                    stackOffset,
                    stackOffset + id.DataType.Size);
            }

            public override Expression NewUse(SsaBlockState bs)
            {
                var sid = ReadVariable(bs);
                sid.Uses.Add(stm);
                return sid.Identifier;
            }

            public override Identifier NewDef(SsaBlockState bs, SsaIdentifier sid)
            {
                return WriteVariable(bs, sid, true);
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                var ints = bs.currentStackDef.GetIntervalsOverlappingWith(offsetInterval)
                    .OrderByDescending(i => i.Key.End - i.Key.Start)
                    .ThenBy(i => i.Key.Start)
                    .Select(SliceAndShift)
                    .ToArray();
                if (ints.Length == 0)
                    return null;
                // Defined locally in this block.
                if (ints.Length == 1)
                {
                    if (offsetInterval.Start == ints[0].Item3.Start &&
                        offsetInterval.End == ints[0].Item3.End)
                    {
                        // Exact match
                        return ints[0].Item1;
                    }
                    return null;
                }
                else
                {
                    var prev = offsetInterval;
                    var sequence = new List<SsaIdentifier>();
                    foreach (var src in ints)
                    {
                        Debug.Print("Analyze: {0} {1} {2}", src.Item1, src.Item2, src.Item3);
                        if (prev.End == src.Item3.Start)
                        {
                            // Previous item ended where next starts:
                            // extend it and create a sequence.
                            prev = Interval.Create(prev.Start, src.Item3.End);
                            sequence.Add(src.Item1);
                        }
                        else if (prev.End < src.Item3.Start)
                        {
                            // Gap betweeen prev item and this item.
                            // Emit the sequence somehow.
                            throw new NotImplementedException("do something with slice");
                        }
                        else if (src.Item3.Covers(prev))
                        {
                            sequence.Clear();
                            sequence.Add(src.Item1);
                            prev = src.Item3;
                        }
                        else if (sequence.Count > 0 && src.Item3.OverlapsWith(prev))
                        {
                            var dpb = new DepositBits(sequence[0].Identifier, src.Item2, src.Item3.Start - prev.Start);
                            var ass = new AliasAssignment(id, dpb);
                            var iStm = outer.stmCur.Block.Statements.IndexOf(outer.stmCur);
                            var stm = outer.stmCur.Block.Statements.Insert(iStm, outer.stmCur.LinearAddress, ass);
                            var sidTo = ssaIds.Add(ass.Dst, stm, ass.Src, false);
                            ass.Dst = sidTo.Identifier;
                            sequence[0].Uses.Add(stm);
                            src.Item1.Uses.Add(stm);
                            sequence = new List<SsaIdentifier> { sidTo };
                        }
                        else
                        {
                            prev = Interval.Create(prev.Start, src.Item3.End);
                            sequence.Add(src.Item1);
                        }
                    }
                    if (sequence.Count == 1)
                        return sequence[0];
                    if (sequence.Count > 1)
                    {
                        var head = sequence[1];
                        var tail = sequence[0];
                        var seq = new MkSequence(this.id.DataType, head.Identifier, tail.Identifier);
                        var ass = new AliasAssignment(id, seq);
                        var iStm = outer.stmCur.Block.Statements.IndexOf(outer.stmCur);
                        var stm = outer.stmCur.Block.Statements.Insert(iStm, outer.stmCur.LinearAddress, ass);
                        var sidTo = ssaIds.Add(ass.Dst, stm, ass.Src, false);
                        ass.Dst = sidTo.Identifier;
                        head.Uses.Add(stm);
                        tail.Uses.Add(stm);
                        return sidTo;
                    }
                }
                return null;
            }

            private Tuple<SsaIdentifier, Expression, Interval<int>> SliceAndShift(KeyValuePair<Interval<int>, AliasState> arg)
            {
                return new Tuple<SsaIdentifier, Expression, Interval<int>>(
                    arg.Value.SsaId,
                    arg.Value.SsaId.Identifier,
                    arg.Key.Intersect(this.offsetInterval));
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                var ints = bs.currentStackDef
                    .GetIntervalsOverlappingWith(offsetInterval)
                    .ToArray();
                foreach (var i in ints)
                {
                    if (this.offsetInterval.Covers(i.Key))
                    { 
                        // None of the bits of interval `i` will shone thr
                        bs.currentStackDef.Delete(i.Key);
                    }
                }
                bs.currentStackDef.Add(this.offsetInterval, new AliasState(sid, null));
                return sid.Identifier;
            }
        }

        public class SequenceTransformer : IdentifierTransformer
        {
            private SequenceStorage seq;

            public SequenceTransformer(
                Identifier id,
                SequenceStorage seq,
                Statement stm,
                SsaTransform outer)
                : base(id, stm, outer)
            {
                this.seq = seq;
            }

            public override SsaIdentifier ReadVariable(SsaBlockState bs)
            {
                var ss = outer.factory.Create(outer.ssa.Procedure.Frame.EnsureIdentifier(seq.Head), stm);
                var head = ss.ReadVariable(bs);
                ss = outer.factory.Create(outer.ssa.Procedure.Frame.EnsureIdentifier(seq.Tail), stm);
                var tail = ss.ReadVariable(bs);
                return Fuse(head, tail);
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                // We shouldn't reach this, as ReadVariable above should have 
                // broken the sequence into a head and tail read.
                throw new InvalidOperationException();
            }

            public SsaIdentifier Fuse(SsaIdentifier head, SsaIdentifier tail)
            {
                if (head.DefStatement.Instruction is AliasAssignment aassHead &&
                    tail.DefStatement.Instruction is AliasAssignment aassTail)
                {
                    if (aassHead.Src is Slice eHead && aassTail.Src is Cast eTail)
                    {
                        return ssaIds[(Identifier)eHead.Expression];
                    }
                }
                if (head.DefStatement.Instruction is DefInstruction defHead &&
                    tail.DefStatement.Instruction is DefInstruction defTail)
                {
                    // All subregisters came in from caller, so create an
                    // alias statement.
                    var seq = new MkSequence(this.id.DataType, head.Identifier, tail.Identifier);
                    var ass = new AliasAssignment(id, seq);
                    var stm = head.DefStatement.Block.Statements.Add(
                        head.DefStatement.LinearAddress,
                        ass);
                    var sidTo = ssaIds.Add(ass.Dst, stm, ass.Src, false);
                    ass.Dst = sidTo.Identifier;
                    head.Uses.Add(stm);
                    tail.Uses.Add(stm);
                    return sidTo;
                }

                if (head.DefStatement.Instruction is Assignment assHead &&
                    tail.DefStatement.Instruction is Assignment assTail)
                {
                    // If x_2 = Slice(y_3); z_4 = (cast) y_3 return y_3
                    if (assHead.Src is Slice slHead && 
                        assTail.Src is Cast caTail &&
                        slHead.Expression == caTail.Expression &&
                        slHead.Expression is Identifier id)
                    {
                        return ssaIds[id];
                    }
                }

                // Unrelated assignments; insert alias right before use.
                return FuseIntoMkSequence(head, tail);
            }

            private SsaIdentifier FuseIntoMkSequence(SsaIdentifier head, SsaIdentifier tail)
            {
                var seq = new MkSequence(this.id.DataType, head.Identifier, tail.Identifier);
                var ass = new AliasAssignment(this.id, seq);
                var iStm = this.stm.Block.Statements.IndexOf(this.stm);
                var stm = head.DefStatement.Block.Statements.Insert(iStm, this.stm.LinearAddress, ass);
                var sidTo = ssaIds.Add(ass.Dst, stm, ass.Src, false);
                ass.Dst = sidTo.Identifier;
                head.Uses.Add(stm);
                tail.Uses.Add(stm);
                return sidTo;
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                var ss = outer.factory.Create(outer.ssa.Procedure.Frame.EnsureIdentifier(seq.Head), stm);
                ss.WriteVariable(bs, sid, performProbe);
                ss = outer.factory.Create(outer.ssa.Procedure.Frame.EnsureIdentifier(seq.Tail), stm);
                ss.WriteVariable(bs, sid, performProbe);
                return sid.Identifier;
            }
        }

        public class FpuStackTransformer : IdentifierTransformer
        {
            private FpuStackStorage fpu;

            public FpuStackTransformer(Identifier id, FpuStackStorage fpu, Statement stm, SsaTransform outer) : base(id, stm, outer)
            {
                this.fpu = fpu;
            }

            public override Identifier NewDef(SsaBlockState bs, SsaIdentifier sid)
            {
                bs.currentFpuDef[fpu.FpuStackOffset] = sid;
                return base.NewDef(bs, sid);
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                bs.currentFpuDef.TryGetValue(fpu.FpuStackOffset, out var sid);
                return sid;
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                bs.currentFpuDef[fpu.FpuStackOffset] = sid;
                return sid.Identifier;
            }
        }
    }
}
