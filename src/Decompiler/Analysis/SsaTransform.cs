#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
        private static TraceSwitch trace = new TraceSwitch("SsaTransform", "Traces the progress of SSA analysis") { Level = TraceLevel.Info };

        private readonly IProcessorArchitecture arch;
        private readonly Program program;
        private readonly ProgramDataFlow programFlow;
        private readonly IImportResolver importResolver;
        private readonly Dictionary<Block, SsaBlockState> blockstates;
        private readonly SsaState ssa;
        private readonly TransformerFactory factory;
        public readonly HashSet<SsaIdentifier> incompletePhis;
        private readonly HashSet<Procedure> sccProcs;
        private readonly ExpressionEmitter m;
        private HashSet<SsaIdentifier> sidsToRemove;
        private Block block;
        private Statement stmCur;

        public SsaTransform(
            Program program,
            Procedure proc,
            HashSet<Procedure> sccProcs,
            IImportResolver importResolver,
            ProgramDataFlow programFlow)
        {
            this.arch = proc.Architecture;
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
            if (ssa.Procedure.Name == "fn0800_441C" && stmCur.LinearAddress == 0x000000000000c469)
                ssa.ToString(); //$DEBUG
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

            if (c != null)
            {
                var e = importResolver.ResolveToImportedValue(stmCur, c);
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
            if (!(e is BinaryExpression bin))
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
                x.AddPhiOperands(phi);
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
    }
}
