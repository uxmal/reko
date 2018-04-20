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
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;

namespace Reko.Analysis
{
    public class TrashedRegisterFinder3 : InstructionVisitor<bool>
    {
        private static TraceSwitch trace = new TraceSwitch("TrashedRegisters", "Trashed value propagation");

        private IProcessorArchitecture arch;
        private SegmentMap segmentMap;
        private ProgramDataFlow flow;
        private HashSet<SsaTransform> sccGroup;
        private CallGraph callGraph;
        private DecompilerEventListener listener;
        private WorkStack<Block> worklist;
        private Dictionary<Procedure, ProcedureFlow> procCtx;
        private Dictionary<Block, Context> blockCtx;
        private Dictionary<Procedure, SsaState> ssas;
        private Context ctx;
        private ExpressionSimplifier eval;
        private Block block;
        private ExpressionValueComparer cmp;
        private bool propagateToCallers;
        private bool selfRecursiveCalls;

        public TrashedRegisterFinder3(
            Program program,
            ProgramDataFlow flow,
            IEnumerable<SsaTransform> sccGroup,
            DecompilerEventListener listener)
        {
            this.arch = program.Architecture;
            this.segmentMap = program.SegmentMap;
            this.flow = flow;
            this.sccGroup = sccGroup.ToHashSet();
            this.callGraph = program.CallGraph;
            this.listener = listener;
            this.cmp = new ExpressionValueComparer();
            this.worklist = new WorkStack<Block>();
            this.ssas = sccGroup.ToDictionary(s => s.SsaState.Procedure, s => s.SsaState);
        }

        /// <summary>
        /// Computes the trashed registers for all the procedures in the 
        /// SCC group.
        /// </summary>
        /// <remarks>
        /// To deal with recursive functions -- including deeply nested
        /// mutually recursive functions, we first compute what registers
        /// are trashed when recursion is disregarded. If there are no
        /// recursive calls, we are done and leave early. 
        /// If there are recursive calls, we make one unwarranted but
        /// highly likely assumption: for each involved procedure, 
        /// the stack pointer will have the  same value in the exit block 
        /// after traversing both non-recursive and recursive paths
        /// of the program.
        /// </remarks>
        public void Compute()
        {
            CreateState();
            this.propagateToCallers = false;

            Block block;
            while (worklist.GetWorkItem(out block))
            {
                ProcessBlock(block);
            }
            
            if (!selfRecursiveCalls)
            {
                return;
            }

            // We make a big, but very reasonable assumption here: if a procedure
            // has a recursive branch and a non-recursive branch, the stack pointer
            // will have the same value at the point where the branches join.
            // It certainly possible for an assembly language programmer to construct
            // a program where procedures deliberately put the stack in imbalance
            // after calling a procedure, so it seems a safe assumption to make.
            
            var savedSps = CollectStackPointers(flow, arch.StackRegister);
            //$REVIEW: Ew. This hardwires a dependency on x87 in common code.
            // We need a general mechanism for dealing with "stack pointers"
            // that abstracts over platform integer stack pointers and the
            // x87 FPU stack pointer.
            var savedTops = new Dictionary<Procedure, int?>();
            if (arch.TryGetRegister("Top", out var top))
            {
                savedTops = CollectStackPointers(flow, arch.GetRegister("Top"));
            }

            CreateState();

            ApplyStackPointers(savedSps, flow);
            ApplyStackPointers(savedTops, flow);

            BypassRegisterOffsets(savedSps, arch.StackRegister);

            this.propagateToCallers = true;
            while (worklist.GetWorkItem(out block))
            {
                ProcessBlock(block);
            }
        }

        private Dictionary<Procedure, int?> CollectStackPointers(ProgramDataFlow flow, Storage stackRegister)
        {
            if (stackRegister == null)
                return new Dictionary<Procedure, int?>();
            return flow.ProcedureFlows.ToDictionary(
                de => de.Key,
                de =>
                {
                    if (de.Value.Trashed.Contains(stackRegister))
                        return (int?)null;
                    if (de.Value.Preserved.Contains(stackRegister))
                        return 0;
                    //$TODO: x86 RET N instructions.
                    return 0;
                });
        }

        private void ApplyStackPointers(
            Dictionary<Procedure, int?> savedSps,
            ProgramDataFlow flow)
        {
            foreach (var de in savedSps)
            {
                var p = flow[de.Key];
                if (de.Value.HasValue && de.Value.Value == 0)
                {
                    //$TODO: x86 RET-N instructions unbalance the stack
                    // register.
                    p.Trashed.Remove(arch.StackRegister);
                    p.Preserved.Add(arch.StackRegister);
                }
            }
        }

        private void BypassRegisterOffsets(Dictionary<Procedure, int?> savedSps, RegisterStorage register)
        {
            foreach (var ssa in this.ssas.Values)
            {
                var callStms = ssa.Procedure.Statements
                    .Where(stm => stm.Instruction is CallInstruction)
                    .ToList();
                var ssam = new SsaMutator(ssa);
                foreach (var stm in callStms)
                {
                    var call = (CallInstruction)stm.Instruction;
                    var proc = (call.Callee as ProcedureConstant)?.Procedure as Procedure;
                    if (proc == null)
                        continue;
                    if (savedSps.TryGetValue(proc, out var delta) ||
                        delta.HasValue)
                    {
                        ssam.AdjustRegisterAfterCall(stm, call, register, delta.Value);
                    }
                }
            }
        }


        private void CreateState()
        {
            this.blockCtx = new Dictionary<Block, Context>();
            this.procCtx = new Dictionary<Procedure, ProcedureFlow>();
            foreach (var sst in sccGroup)
            {
                var proc = sst.SsaState.Procedure;
                var procFlow = this.flow[proc];
                procFlow.Trashed.Clear();
                procFlow.Preserved.Clear();
                procFlow.Constants.Clear();

                procCtx.Add(proc, procFlow);
                var idState = new Dictionary<Identifier, Tuple<Expression,BitRange>>();
                //$REVIEW: this assumes the existence of a frame pointer.
                var fp = sst.SsaState.Identifiers[proc.Frame.FramePointer].Identifier;
                var block = proc.EntryBlock;
                blockCtx.Add(block, new Context(sst.SsaState, fp, idState, procFlow));

                if (proc.Signature.ParametersValid)
                {
                    ApplySignature(proc.Signature, flow[proc]);
                }
                else
                {
                    worklist.Add(proc.EntryBlock);
                }
                procFlow.TerminatesProcess = proc.ExitBlock.Pred.Count == 0;
            }
        }

        private void ApplySignature(FunctionType sig, ProcedureFlow procFlow)
        {
            //$REVIEW: do we need this? if a procedure has a signature,
            // we will always trust that rather than the flow.
            if (!sig.HasVoidReturn)
            {
                procFlow.Trashed.Add(sig.ReturnValue.Storage);
            }
            foreach (var stg in sig.Parameters
                .Select(p => p.Storage)
                .OfType<OutArgumentStorage>())
            {
                procFlow.Trashed.Add(stg.OriginalIdentifier.Storage);
            }
        }

        private void ProcessBlock(Block block)
        {
            this.ctx = blockCtx[block];
            this.eval = new ExpressionSimplifier(segmentMap, ctx, listener);

            this.block = block;

            this.ctx.IsDirty = false;
            foreach (var stm in block.Statements)
            {
                if (!stm.Instruction.Accept(this))
                    return;
            }
            if (block == block.Procedure.ExitBlock)
            {
                UpdateProcedureSummary(this.ctx, block);
            }
            else
            {
                UpateBlockSuccessors(block);
            }
        }

        private void UpdateProcedureSummary(Context ctx, Block block)
        {
            if (!propagateToCallers || !ctx.IsDirty)
                return;

            var callingBlocks = callGraph
                .CallerStatements(block.Procedure)
                .Cast<Statement>()
                .Select(s => s.Block)
                .Where(b => ssas.ContainsKey(b.Procedure));
            foreach (var caller in callingBlocks)
            {
                if (!blockCtx.TryGetValue(caller, out var succCtx))
                {
                    var ssa = ssas[caller.Procedure];
                    var fp = ssa.Identifiers[caller.Procedure.Frame.FramePointer].Identifier;
                    var idState = blockCtx[caller.Procedure.EntryBlock].IdState;
                    var clone = new Context(
                        ssa,
                        fp,
                        idState,
                        procCtx[caller.Procedure]);
                    blockCtx.Add(caller, clone);
                }
                else
                {
                    succCtx.MergeWith(ctx);
                }
                worklist.Add(caller);
            }
        }

        private void UpateBlockSuccessors(Block block)
        {
            var bf = blockCtx[block];
            foreach (var s in block.Succ)
            {
                if (!blockCtx.TryGetValue(s, out var succCtx))
                {
                    blockCtx.Add(s, ctx.Clone());
                    worklist.Add(s);
                }
                else if (bf.IsDirty)
                {
                    succCtx.MergeWith(bf);
                    worklist.Add(s);
                }
            }
        }

        public bool VisitAssignment(Assignment ass)
        {
            var value = ass.Src.Accept(eval);
            DebugEx.PrintIf(trace.TraceVerbose, "{0} = [{1}]", ass.Dst, value);

            if (ass.Src is DepositBits dpb && 
                dpb.Source is Identifier idDpb &&
                ass.Dst.Storage == idDpb.Storage)
            {
                var oldRange = ctx.GetBitRange(idDpb);
                var newRange = new BitRange(
                    Math.Min(oldRange.Lsb, dpb.BitPosition),
                    Math.Max(oldRange.Msb, dpb.BitPosition + dpb.InsertedBits.DataType.BitSize));
                ctx.SetValue(ass.Dst, value, newRange);
            }
            else
            {
                ctx.SetValue(ass.Dst, value);
            }
            return true;
        }

        public bool VisitBranch(Branch branch)
        {
            branch.Condition.Accept(eval);
            return true;
        }

        public bool VisitCallInstruction(CallInstruction ci)
        {
            var pc = ci.Callee as ProcedureConstant;
            if (pc == null)
            {
                foreach (var d in ci.Definitions)
                {
                    ctx.SetValue((Identifier)d.Expression, Constant.Invalid);
                    DebugEx.PrintIf(trace.TraceVerbose, "  {0} = [{1}]", d.Expression, Constant.Invalid);
                }
                return true;
            }

            var callee = pc.Procedure as Procedure;
            if (callee == null)
                throw new NotImplementedException();

            if (sccGroup.Any(s => s.SsaState.Procedure == callee))
            {
                // we're calling a function in the recursion group. If 
                // we are not in propagate to callers, simply stop this block.
                if (!propagateToCallers)
                {
                    selfRecursiveCalls = true;
                    return false;
                }
                // Otherwise, use the flow information collected by
                // the previous pass.
            }

            var flow = this.flow[callee];
            Dump(block);
            foreach (var d in ci.Definitions)
            {
                if (flow.Trashed.Contains(d.Storage))
                {
                    ctx.SetValue((Identifier)d.Expression, Constant.Invalid);
                    DebugEx.PrintIf(trace.TraceVerbose, "  {0} = [{1}]", d.Expression, Constant.Invalid);
                }
                if (flow.Preserved.Contains(d.Storage))
                {
                    var before = ci.Uses
                        .Where(u => u.Storage == d.Storage)
                        .Select(u => u.Expression.Accept(eval))
                        .SingleOrDefault();
                    ctx.SetValue((Identifier)d.Expression, before);
                    DebugEx.PrintIf(trace.TraceVerbose, "  {0} = [{1}]", d.Expression, before);
                }
            }
            return true;
        }

        public bool VisitComment(CodeComment comment)
        {
            return true;
        }

        public bool VisitDeclaration(Declaration decl)
        {
            var value = decl.Expression.Accept(eval);
            DebugEx.PrintIf(trace.TraceVerbose, "{0} = [{1}]", decl.Identifier, value);
            ctx.SetValue(decl.Identifier, value);
            return true;
        }

        public bool VisitDefInstruction(DefInstruction def)
        {
            var id = def.Identifier;
            ctx.SetValue(id, id, BitRange.Empty);
            return true;
        }

        public bool VisitGotoInstruction(GotoInstruction g)
        {
            g.Condition.Accept(eval);
            g.Target.Accept(eval);
            return true;
        }

        public bool VisitPhiAssignment(PhiAssignment phi)
        {
            Expression total = null;
            for (int i = 0; i < phi.Src.Arguments.Length; ++i)
            {
                var p = block.Pred[i];
                var phiarg = (Identifier)phi.Src.Arguments[i];
                // If phiarg hasn't been evaluated yet, it will have
                // the value null after ctx.GetValue below. If not, we 
                // use that value and hope all of the phi args have
                // the same value.
                var value = ctx.GetValue(phiarg);
                if (total == null)
                {
                    total = value;
                }
                else if (value != null && !cmp.Equals(value, total))
                {
                    total = Constant.Invalid;
                    break;
                }
            }
            if (total != null)
            {
                ctx.SetValue(phi.Dst, total);
            }
            DebugEx.PrintIf(trace.TraceVerbose, "{0} = φ[{1}]", phi.Dst, total);
            return true;
        }

        public bool VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
            {
                ret.Expression.Accept(eval);
            }
            return true;
        }

        public bool VisitSideEffect(SideEffect side)
        {
            side.Expression.Accept(eval);
            return true;
        }

        public bool VisitStore(Store store)
        {
            var value = store.Src.Accept(eval);
            ctx.SetValueEa(((MemoryAccess)store.Dst).EffectiveAddress, value);
            return true;
        }

        public bool VisitSwitchInstruction(SwitchInstruction si)
        {
            si.Expression.Accept(eval);
            return true;
        }

        /// <summary>
        /// We're in the exit block now, so collect all identifiers
        /// that are registers into the procedureflow of the 
        /// current procedure.
        /// </summary>
        public bool VisitUseInstruction(UseInstruction use)
        {
            var id = use.Expression as Identifier;
            if (id == null)
                return true;
            var sid = ssas[block.Procedure].Identifiers[id];
            if (id.Storage is RegisterStorage)
            {
                var value = ctx.GetValue(id);
                var range = ctx.GetBitRange(id);
                var stg = arch.GetRegister(id.Storage.Domain, range) ?? id.Storage;
                if (value == Constant.Invalid)
                {
                    ctx.ProcFlow.Trashed.Add(stg);
                    ctx.ProcFlow.Preserved.Remove(stg);
                    ctx.ProcFlow.Constants.Remove(stg);
                    return true;
                }
                if (value is Constant c)
                {
                    ctx.ProcFlow.Constants[stg] = c;
                    ctx.ProcFlow.Preserved.Remove(stg);
                    ctx.ProcFlow.Trashed.Add(stg);
                    return true;
                }
                if (value is Identifier idV)
                {
                    if (id.Storage == arch.StackRegister)
                    {
                        if (idV == ctx.FramePointer)
                        {
                            // Special case: if we deduce that the CPU stack
                            // register is equal to the pseudo-register FP
                            // (frame pointer), we make note that the stack
                            // register is preserved.
                            ctx.ProcFlow.Preserved.Add(arch.StackRegister);
                            return true;
                        }
                    }
                    else if (idV.Storage == id.Storage)
                    {
                        if (sid.OriginalIdentifier == idV &&
                            sid.OriginalIdentifier != id)
                        {
                            ctx.ProcFlow.Preserved.Add(stg);
                        }
                        return true;
                    }
                }
                ctx.ProcFlow.Trashed.Add(stg);
            }
            else if (id.Storage is FlagGroupStorage grfStorage)
            {
                var value = ctx.GetValue(id);
                if (value is Identifier idV && idV == sid.OriginalIdentifier)
                {
                    ctx.ProcFlow.grfPreserved[grfStorage.FlagRegister] =
                        ctx.ProcFlow.grfPreserved.Get(grfStorage.FlagRegister) | grfStorage.FlagGroupBits;
                    ctx.ProcFlow.grfTrashed[grfStorage.FlagRegister] =
                        ctx.ProcFlow.grfTrashed.Get(grfStorage.FlagRegister) & ~grfStorage.FlagGroupBits;
                }
                else
                {
                    ctx.ProcFlow.grfTrashed[grfStorage.FlagRegister] =
                        ctx.ProcFlow.grfTrashed.Get(grfStorage.FlagRegister) | grfStorage.FlagGroupBits;
                    ctx.ProcFlow.grfPreserved[grfStorage.FlagRegister] =
                        ctx.ProcFlow.grfPreserved.Get(grfStorage.FlagRegister) & ~grfStorage.FlagGroupBits;
                }
                return true;
            }
            else if (id.Storage is FpuStackStorage fpuStg)
            {
                var value = ctx.GetValue(id);
                if (value is Identifier idV && idV == sid.OriginalIdentifier)
                {
                    ctx.ProcFlow.Preserved.Add(fpuStg);
                }
                else
                {
                    ctx.ProcFlow.Trashed.Add(fpuStg);
                }
                return true;
            }
            return true;
        }

        [Conditional("DEBUG")]
        public void Dump(Block block)
        {
            var b = block ?? this.block;
            foreach (var de in blockCtx[b].IdState.OrderBy(i => i.Key.Name))
            {
                DebugEx.PrintIf(trace.TraceVerbose, "{0}: [{1}]", de.Key, de.Value);
            }
            foreach (var de in blockCtx[b].StackState.OrderBy(i => i.Key))
            {
                DebugEx.PrintIf(trace.TraceVerbose, "fp {0} {1}: [{2}]", de.Key >= 0 ? "+" : "-", Math.Abs(de.Key), de.Value);
            }
        }

        public class Context : EvaluationContext
        {
            public readonly Identifier FramePointer;
            public readonly Dictionary<Identifier, Tuple<Expression, BitRange>> IdState;
            public readonly Dictionary<int, Expression> StackState;
            public ProcedureFlow ProcFlow;
            public bool IsDirty;
            private SsaState ssa;
            private readonly ExpressionValueComparer cmp;

            public Context(
                SsaState ssa,
                Identifier fp,
                Dictionary<Identifier, Tuple<Expression, BitRange>> idState,
                ProcedureFlow procFlow)
                : this(
                      ssa,
                      fp,
                      idState,
                      procFlow, 
                      new Dictionary<int,Expression>(),
                      new ExpressionValueComparer())
            {
            }

            private Context(
                SsaState ssa,
                Identifier fp,
                Dictionary<Identifier, Tuple<Expression, BitRange>> idState,
                ProcedureFlow procFlow,
                Dictionary<int, Expression> stack,
                ExpressionValueComparer cmp)
            {
                this.ssa = ssa;
                this.FramePointer = fp;
                this.IdState = idState;
                this.ProcFlow = procFlow;
                this.StackState = stack;
                this.cmp = cmp;
            }

            public Context Clone()
            {
                return new Context(ssa, this.FramePointer, this.IdState, this.ProcFlow, new Dictionary<int, Expression>(StackState), cmp);
            }

            /// <summary>
            /// Merge <paramref name="ctxOther"/> into this context.
            /// </summary>
            /// <param name="ctxOther"></param>
            /// <returns>True if a change resulted, otherwise false.</returns>
            public bool MergeWith(Context ctxOther)
            {
                bool changed = false;
                foreach (var de in ctxOther.StackState)
                {
                    if (!this.StackState.ContainsKey(de.Key))
                    {
                        changed = true;
                        this.StackState.Add(de.Key, de.Value);
                    }
                    else if (!cmp.Equals(this.StackState[de.Key], de.Value))
                    {
                        changed = true;
                        this.StackState[de.Key] = Constant.Invalid;
                    }
                }
                return changed;
            }

            public Expression GetDefiningExpression(Identifier id)
            {
                return null;
            }

            public Expression GetValue(SegmentedAccess access, SegmentMap segmentMap)
            {
                var offset = this.GetFrameOffset(access.EffectiveAddress);
                if (offset.HasValue && StackState.TryGetValue(offset.Value, out Expression value))
                {
                    return value;
                }
                return Constant.Invalid;
            }

            public Expression GetValue(Application appl)
            {
                var args = appl.Arguments;
                for (int i = 0; i < args.Length; ++i)
                {
                    var outArg = args[i] as OutArgument;
                    if (outArg == null)
                        continue;
                    if (outArg.Expression is Identifier outId)
                    {
                        SetValue(outId, Constant.Invalid);
                    }
                }
                return Constant.Invalid;
            }

            public Expression GetValue(MemoryAccess access, SegmentMap segmentMap)
            {
                var offset = this.GetFrameOffset(access.EffectiveAddress);
                if (offset.HasValue && StackState.TryGetValue(offset.Value, out Expression value))
                {
                    return value;
                }
                return Constant.Invalid;
            }

            public Expression GetValue(Identifier id)
            {
                if (!IdState.TryGetValue(id, out Tuple<Expression, BitRange> value))
                    return null;
                else
                    return value.Item1;
            }

            public BitRange GetBitRange(Identifier id)
            {
                if (!IdState.TryGetValue(id, out Tuple<Expression, BitRange> value))
                    return BitRange.Empty;
                else
                    return value.Item2;
            }


            public bool IsUsedInPhi(Identifier id)
            {
                var src = ssa.Identifiers[id].DefStatement;
                if (src == null)
                    return false;
                var assSrc = src.Instruction as Assignment;
                if (assSrc == null)
                    return false;
                return ExpressionIdentifierUseFinder.Find(ssa.Identifiers, assSrc.Src)
                    .Select(c => ssa.Identifiers[c].DefStatement)
                    .Where(d => d != null)
                    .Select(ph => ph.Instruction as PhiAssignment)
                    .Where(ph => ph != null)
                    .Any();
            }

            public Expression MakeSegmentedAddress(Constant c1, Constant c2)
            {
                throw new NotImplementedException();
            }

            public void RemoveExpressionUse(Expression expr)
            {
                throw new NotImplementedException();
            }

            public void RemoveIdentifierUse(Identifier id)
            {
            }

            public void SetValue(Identifier id, Expression value)
            {
                SetValue(id, value, id.Storage.GetBitRange());
            }

            public void SetValue(Identifier id, Expression value, BitRange range)
            {
                if (!IdState.TryGetValue(id, out Tuple<Expression, BitRange> oldValue))
                {
                    IsDirty = true;
                    IdState.Add(id, Tuple.Create(value, range));
                }
                else if (!cmp.Equals(oldValue.Item1, value))
                {
                    IsDirty = true;
                    IdState[id] = Tuple.Create(value, range);
                }
            }

            public void SetValueEa(Expression effectiveAddress, Expression value)
            {
                var offset = GetFrameOffset(effectiveAddress);
                if (!offset.HasValue)
                    return;

                if (!StackState.TryGetValue(offset.Value, out Expression oldValue))
                {
                    IsDirty = true;
                    StackState.Add(offset.Value, value);
                }
                else if (!cmp.Equals(oldValue, value))
                {
                    IsDirty = true;
                    StackState[offset.Value] = value;
                }
            }

            private int? GetFrameOffset(Expression effectiveAddress)
            {
                var ea = effectiveAddress as BinaryExpression;
                if (ea == null || ea.Left != FramePointer)
                    return null;
                var o = ea.Right as Constant;
                if (o == null)
                    return null;
                if (ea.Operator == Operator.IAdd)
                {
                    return o.ToInt32();
                }
                else if (ea.Operator == Operator.ISub)
                {
                    return -o.ToInt32();
                }
                return null;
            }

            public void SetValueEa(Expression basePointer, Expression ea, Expression value)
            {
                throw new NotImplementedException();
            }

            public void UseExpression(Expression expr)
            {
            }
        }
    }
}
