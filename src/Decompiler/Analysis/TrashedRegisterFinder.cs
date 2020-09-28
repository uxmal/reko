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
    public class TrashedRegisterFinder : InstructionVisitor<bool>
    {
        private static TraceSwitch trace = new TraceSwitch("TrashedRegisters", "Trashed value propagation") { Level = TraceLevel.Error };

        private readonly IProcessorArchitecture arch;
        private readonly SegmentMap segmentMap;
        private readonly ProgramDataFlow flow;
        private readonly HashSet<SsaTransform> sccGroup;
        private readonly CallGraph callGraph;
        private readonly DecompilerEventListener listener;
        private readonly WorkStack<Block> worklist;
        private readonly Dictionary<Procedure, SsaState> ssas;
        private readonly ExpressionValueComparer cmp;
        private Block block;
        private Dictionary<Procedure, ProcedureFlow> procCtx;
        private Dictionary<Block, Context> blockCtx;
        private Context ctx;
        private ExpressionSimplifier eval;
        private bool propagateToCallers;
        private bool selfRecursiveCalls;

        public TrashedRegisterFinder(
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
        /// the stack pointer will have the same value in the exit block 
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
            // after calling a procedure, but using such a procedure is very difficult
            // to do as you must somehow understand how the procedure changes the
            // stack pointers depending on ... anything! 
            // It seems safe to assume that all branches leading to the exit block
            // have the same stack pointer value.
            
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
                    if (!((call.Callee as ProcedureConstant)?.Procedure is Procedure proc))
                        continue;
                    if (savedSps.TryGetValue(proc, out var delta) &&
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
                Identifier fp;
                if (sst.SsaState.Identifiers.TryGetValue(proc.Frame.FramePointer, out var sidFp))
                {
                    fp = sidFp.Identifier;
                }
                else
                {
                    fp = null;
                }
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

           // Propagate defined registers to calling procedures.
            var callingBlocks = callGraph
                .CallerStatements(block.Procedure)
                .Cast<Statement>()
                .Select(s => s.Block)
                .Where(b => ssas.ContainsKey(b.Procedure));
            foreach (var caller in callingBlocks)
            {
                if (!blockCtx.TryGetValue(caller, out var succCtx))
                {
                    var ssaCaller = ssas[caller.Procedure];
                    var fpCaller = ssaCaller.Identifiers[caller.Procedure.Frame.FramePointer].Identifier;
                    var idsCaller = blockCtx[caller.Procedure.EntryBlock].IdState;
                    var clone = new Context(
                        ssaCaller,
                        fpCaller,
                        idsCaller,
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
                    var ctxClone = ctx.Clone();
                    blockCtx.Add(s, ctxClone);
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
            DebugEx.Verbose(trace, "{0} = [{1}]", ass.Dst, value);

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
            if (!(ci.Callee is ProcedureConstant pc))
            {
                foreach (var d in ci.Definitions)
                {
                    ctx.SetValue((Identifier) d.Expression, Constant.Invalid);
                    DebugEx.Verbose(trace, "  {0} = [{1}]", d.Expression, Constant.Invalid);
                }
                return true;
            }

            if (pc.Procedure is ExternalProcedure ep)
            {
                // External procedures with signatures will have generated
                // an Application earlier; without signatures they will have
                // generated hell nodes. In either case we can't propagate 
                // anything, so we leave early.
                return true;
            }
            if (!(pc.Procedure is Procedure callee))
                throw new NotImplementedException();

            if (sccGroup.Any(s => s.SsaState.Procedure == callee))
            {
                // We're calling a function in the recursion group. If 
                // we are not in propagate to callers mode, simply stop 
                // at this block.
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
                    DebugEx.Verbose(trace, "  {0} = [{1}]", d.Expression, Constant.Invalid);
                }
                if (flow.Preserved.Contains(d.Storage))
                {
                    var before = ci.Uses
                        .Where(u => u.Storage == d.Storage)
                        .Select(u => u.Expression.Accept(eval))
                        .SingleOrDefault();
                    ctx.SetValue((Identifier)d.Expression, before);
                    DebugEx.Verbose(trace, "  {0} = [{1}]", d.Expression, before);
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
            DebugEx.Verbose(trace, "{0} = [{1}]", decl.Identifier, value);
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
            foreach (var de in phi.Src.Arguments)
            {
                var p = de.Block;
                var phiarg = (Identifier) de.Value;
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
            DebugEx.Verbose(trace, "{0} = φ[{1}]", phi.Dst, total);
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
            if (store.Dst is MemoryAccess mem)
            {
                ctx.SetValueEa(mem.EffectiveAddress, value);
            }
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
            if (!(use.Expression is Identifier id))
                return true;
            var sid = ssas[block.Procedure].Identifiers[id];
            switch (id.Storage)
            {
            case RegisterStorage reg:
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
                    ctx.ProcFlow.Preserved.Remove(stg);
                    ctx.ProcFlow.Constants.Remove(stg);
                }
                break;
            case FlagGroupStorage grfStorage:
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
            case FpuStackStorage fpuStg:
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
            }
            return true;
        }

        [Conditional("DEBUG")]
        public void Dump(Block block)
        {
            var b = block ?? this.block;
            foreach (var de in blockCtx[b].IdState.OrderBy(i => i.Key.Name))
            {
                DebugEx.Verbose(trace, "{0}: [{1}]", de.Key, de.Value);
            }
            foreach (var de in blockCtx[b].StackState.OrderBy(i => i.Key))
            {
                DebugEx.Verbose(trace, "fp {0} {1}: [{2}]", de.Key >= 0 ? "+" : "-", Math.Abs(de.Key), de.Value);
            }
        }

        public class Context : EvaluationContext
        {
            public readonly Identifier FramePointer;
            public readonly Dictionary<Identifier, Tuple<Expression, BitRange>> IdState;
            public readonly Dictionary<int, Expression> StackState;
            public ProcedureFlow ProcFlow;
            public bool IsDirty { get; set; }
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
                    if (!this.StackState.TryGetValue(de.Key, out var oldValue))
                    {
                        changed = true;
                        this.StackState.Add(de.Key, de.Value);
                    }
                    else if (oldValue != Constant.Invalid && !cmp.Equals(oldValue, de.Value))
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

            public List<Statement> GetDefiningStatementClosure(Identifier id)
            {
                return new List<Statement>();
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
                    if (!(args[i] is OutArgument outArg))
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
                if (id.Storage is StackStorage stack)
                    return StackState.Get(stack.StackOffset);
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
                if (!(src.Instruction is Assignment assSrc))
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
                if (id.Storage is StackStorage stack)
                {
                    if (!StackState.TryGetValue(stack.StackOffset, out Expression oldValue))
                    {
                        DebugEx.Verbose(trace, "Trf: Stack offset {0:X4} now has value {1}", stack.StackOffset, value);
                        IsDirty = true;
                        StackState.Add(stack.StackOffset, value);
                    }
                    else if (!cmp.Equals(oldValue, value) && oldValue != Constant.Invalid)
                    {
                        DebugEx.Verbose(trace, "Trf: Stack offset {0:X4} now has value {1}, was {2}", stack.StackOffset, value, oldValue);
                        IsDirty = true;
                        StackState[stack.StackOffset] = Constant.Invalid;
                    }
                }
                else
                {
                    if (!IdState.TryGetValue(id, out Tuple<Expression, BitRange> oldValue))
                    {
                        DebugEx.Verbose(trace, "Trf: id {0} now has value {1}", id, value);
                        IsDirty = true;
                        IdState.Add(id, Tuple.Create(value, range));
                    }
                    else if (!cmp.Equals(oldValue.Item1, value) && oldValue.Item1 != Constant.Invalid)
                    {
                        DebugEx.Verbose(trace, "Trf: id {0} now has value {1}, was {2}", id, value, oldValue);
                        IsDirty = true;
                        IdState[id] = Tuple.Create((Expression)Constant.Invalid, range);
                    }
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
                    DebugEx.Verbose(trace, "Trf: Stack offset {0:X4} now has value {1}", offset.Value, value);
                    StackState.Add(offset.Value, value);
                }
                else if (!cmp.Equals(oldValue, value) && oldValue != Constant.Invalid)
                {
                    IsDirty = true;
                    DebugEx.Verbose(trace, "Trf: Stack offset {0:X4} now has value {1}, was {2}", offset.Value, value, oldValue);
                    StackState[offset.Value] = Constant.Invalid;
                }
            }

            private int? GetFrameOffset(Expression effectiveAddress)
            {
                if (!(effectiveAddress is BinaryExpression ea) || ea.Left != FramePointer)
                    return null;
                if (!(ea.Right is Constant o))
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
