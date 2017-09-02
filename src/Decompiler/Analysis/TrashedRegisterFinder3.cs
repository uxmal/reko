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
        private IProcessorArchitecture arch;
        private ProgramDataFlow flow;
        private Dictionary<Procedure, HashSet<Storage>> assumedPreserved;
        private ExpressionValueComparer cmp;
        private CallGraph callGraph;
        private DecompilerEventListener listener;
        private HashSet<SsaTransform> sccGroup;
        private WorkStack<Block> worklist;
        private Dictionary<Procedure, ProcedureFlow> procCtx;
        private Dictionary<Block, Context> blockCtx;
        private Context ctx;
        private ExpressionSimplifier eval;
        private Block block;
        private bool propagateToCallers;
        private bool selfRecursiveCalls;

        public TrashedRegisterFinder3(
            IProcessorArchitecture arch,
            ProgramDataFlow flow,
            CallGraph callGraph,
            IEnumerable<SsaTransform> sccGroup,
            DecompilerEventListener listener)
        {
            this.arch = arch;
            this.flow = flow;
            this.sccGroup = sccGroup.ToHashSet();
            this.callGraph = callGraph;
            this.assumedPreserved = sccGroup.ToDictionary(k => k.SsaState.Procedure, v => new HashSet<Storage>());
            this.listener = listener;
            this.cmp = new ExpressionValueComparer();
            this.worklist = new WorkStack<Block>();
        }

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

            var savedSps = CollectStackPointers(flow);
            CreateState();
            ApplyStackPointers(savedSps, flow);

            this.propagateToCallers = true;
            while (worklist.GetWorkItem(out block))
            {
                ProcessBlock(block);
            }
        }

        private Dictionary<Procedure, int?> CollectStackPointers(ProgramDataFlow flow)
        {
            return flow.ProcedureFlows.ToDictionary(
                de => de.Key,
                de =>
                {
                    if (de.Value.Trashed.Contains(arch.StackRegister))
                        return (int?)null;
                    if (de.Value.Preserved.Contains(arch.StackRegister))
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
                    p.Trashed.Remove(arch.StackRegister);
                    p.Preserved.Add(arch.StackRegister);
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
                var idState = new Dictionary<Identifier, Expression>();
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
            this.eval = new ExpressionSimplifier(ctx, listener);

            this.block = block;
            this.eval = new ExpressionSimplifier(ctx, listener);

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
                .Select(s => s.Block);
            worklist.AddRange(callingBlocks);
        }

        private void UpateBlockSuccessors(Block block)
        {
            var bf = blockCtx[block];
            foreach (var s in block.Succ)
            {
                Context succCtx;
                if (!blockCtx.TryGetValue(s, out succCtx))
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
            Debug.Print("{0} = [{1}]", ass.Dst, value);

            ctx.SetValue(ass.Dst, value);
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
                throw new NotImplementedException();
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
                    Debug.Print("  {0} = [{1}]", d.Expression, Constant.Invalid);
                }
                if (flow.Preserved.Contains(d.Storage))
                {
                    var before = ci.Uses
                        .Where(u => u.Storage == d.Storage)
                        .Select(u => u.Expression.Accept(eval))
                        .SingleOrDefault();
                    ctx.SetValue((Identifier)d.Expression, before);
                    Debug.Print("  {0} = [{1}]", d.Expression, before);
                }
            }
            return true;
        }

        public bool VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        public bool VisitDefInstruction(DefInstruction def)
        {
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
                if (blockCtx.ContainsKey(p))
                {
                    // We've visited p.
                    var value = ctx.GetValue((Identifier)phi.Src.Arguments[i]);
                    if (total == null)
                    {
                        total = value;
                    }
                    else if (!cmp.Equals(value, total))
                    {
                        total = Constant.Invalid;
                        break;
                    }
                }
            }
            if (total != null)
            {
                ctx.SetValue(phi.Dst, total);
            }
            Debug.Print("{0} = φ[{1}]", phi.Dst, total);
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
            //$TODO What about terminating functions like "exit"?
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// We're in the exit block now, so collect all identifiers
        /// that are registers into the procedureflow of the 
        /// current procedure.
        /// </summary>
        public bool VisitUseInstruction(UseInstruction use)
        {
            var id = use.Expression as Identifier;
            if (id == null || !(id.Storage is RegisterStorage))
                return true;
            var value = ctx.GetValue(id);
            if (value == Constant.Invalid)
            {
                ctx.ProcFlow.Trashed.Add(id.Storage);
                ctx.ProcFlow.Preserved.Remove(id.Storage);
                ctx.ProcFlow.Constants.Remove(id.Storage);
                return true;
            }
            var c = value as Constant;
            if (c != null)
            {
                ctx.ProcFlow.Constants[id.Storage] = c;
                ctx.ProcFlow.Preserved.Remove(id.Storage);
                ctx.ProcFlow.Trashed.Add(id.Storage);
                return true;
            }
            var idV = value as Identifier;
            if (idV != null)
            {
                if (id.Storage == arch.StackRegister)
                {
                    if (idV == ctx.FramePointer)
                    {
                        ctx.ProcFlow.Preserved.Add(arch.StackRegister);
                        return true;
                    }
                }
                else if (idV.Storage == id.Storage)
                {
                    ctx.ProcFlow.Preserved.Add(id.Storage);
                    return true;
                }
            }
            ctx.ProcFlow.Trashed.Add(id.Storage);
            return true;
        }

        [Conditional("DEBUG")]
        public void Dump(Block block)
        {
            var b = block ?? this.block;
            foreach (var de in blockCtx[b].IdState.OrderBy(i => i.Key.Name))
            {
                Debug.Print("{0}: [{1}]", de.Key, de.Value);
            }
            foreach (var de in blockCtx[b].StackState.OrderBy(i => i.Key))
            {
                Debug.Print("fp {0} {1}: [{2}]", de.Key >= 0 ? "+" : "-", Math.Abs(de.Key), de.Value);
            }
        }

        public class Context : EvaluationContext
        {
            public readonly Identifier FramePointer;
            public readonly Dictionary<Identifier, Expression> IdState;
            public readonly ProcedureFlow ProcFlow;
            public readonly Dictionary<int, Expression> StackState;
            public bool IsDirty;
            private SsaState ssa;
            private readonly ExpressionValueComparer cmp;

            public Context(
                SsaState ssa,
                Identifier fp,
                Dictionary<Identifier, Expression> idState,
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
                Dictionary<Identifier, Expression> idState,
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

            public bool MergeWith(Context ctxOther)
            {
                return true;
            }

            public Expression GetDefiningExpression(Identifier id)
            {
                return null;
            }

            public Expression GetValue(SegmentedAccess access)
            {
                throw new NotImplementedException();
            }

            public Expression GetValue(Application appl)
            {
                var args = appl.Arguments;
                for (int i = 0; i < args.Length; ++i)
                {
                    var outArg = args[i] as OutArgument;
                    if (outArg == null)
                        continue;
                    var outId = outArg.Expression as Identifier;
                    if (outId != null)
                        SetValue(outId, Constant.Invalid);
                }
                return Constant.Invalid;
            }

            public Expression GetValue(MemoryAccess access)
            {
                var offset = this.GetFrameOffset(access.EffectiveAddress);
                Expression value;
                if (offset.HasValue && StackState.TryGetValue(offset.Value, out value))
                {
                    return value;
                }
                return Constant.Invalid;
            }

            public Expression GetValue(Identifier id)
            {
                Expression value;
                if (!IdState.TryGetValue(id, out value))
                    return null;
                else
                    return value;
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
                Expression oldValue;
                if (!IdState.TryGetValue(id, out oldValue))
                {
                    IsDirty = true;
                    IdState.Add(id, value);
                }
                else if (!cmp.Equals(oldValue, value))
                {
                    IsDirty = true;
                    IdState[id] = value;
                }
            }

            public void SetValueEa(Expression effectiveAddress, Expression value)
            {
                var offset = GetFrameOffset(effectiveAddress);
                if (!offset.HasValue)
                    return;

                Expression oldValue;
                if (!StackState.TryGetValue(offset.Value, out oldValue))
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
