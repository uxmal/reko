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
    public class TrashedRegisterFinder3 : InstructionVisitor<Instruction>
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
            foreach (var sst in sccGroup)
            {
                var proc = sst.SsaState.Procedure;
                if (proc.Signature.ParametersValid)
                {
                    ApplySignature(proc.Signature, flow[proc]);
                }
                else
                {
                    worklist.Add(proc.EntryBlock);
                }
            }
            Block block;
            while (worklist.GetWorkItem(out block))
            {
                ProcessBlock(block);
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

                procCtx.Add(proc, procFlow);
                var idState = new Dictionary<Identifier, Expression>();
                var fp = sst.SsaState.Identifiers[proc.Frame.FramePointer].Identifier;
                var block = proc.EntryBlock;
                blockCtx.Add(block, new Context(fp, idState, procFlow));
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
            this.block = block;
            this.eval = new ExpressionSimplifier(ctx, listener);

            foreach (var stm in block.Statements)
            {
                stm.Instruction.Accept(this);
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
            if (!propagateToCallers)
                return;
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

        public Instruction VisitAssignment(Assignment ass)
        {
            var eval = new ExpressionSimplifier(ctx, listener);
            var value = ass.Src.Accept(eval);
            Debug.Print("{0} = [{1}]", ass.Dst, value);
            ctx.SetValue(ass.Dst, value);
            return ass;
        }

        public Instruction VisitBranch(Branch branch)
        {
            var eval = new ExpressionSimplifier(ctx, listener);
            branch.Condition.Accept(eval);
            return branch;
        }

        public Instruction VisitCallInstruction(CallInstruction ci)
        {
            var pc = ci.Callee as ProcedureConstant;
            if (pc == null)
                throw new NotImplementedException();
            var callee = pc.Procedure as Procedure;
            if (callee == null)
                throw new NotImplementedException();

            if (sccGroup.Any(s => s.SsaState.Procedure == callee))
                throw new NotImplementedException();

            var flow = this.flow[callee];
            foreach (var x in flow.Trashed)
            {
                x.ToString();
            }
            return ci;
        }

        public Instruction VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitDefInstruction(DefInstruction def)
        {
            return def;
        }

        public Instruction VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitPhiAssignment(PhiAssignment phi)
        {
            Expression total = null;
            for (int i = 0; i < phi.Src.Arguments.Length; ++i)
            {
                var value = phi.Src.Arguments[i].Accept(eval);
                if (total == null)
                {
                    total = value;
                }
                else if (total is Constant)
                {
                    var cTotal = total as Constant;
                    var cValue = value as Constant;
                    if (cValue == null || !cValue.Equals(cTotal))
                    {
                        total = Constant.Invalid;
                        break;
                    }
                }
                else if (total is Identifier)
                {
                    if (value != total)
                    {
                        total = Constant.Invalid;   
                    }
                }
            }
            ctx.SetValue(phi.Dst, total);
            Debug.Print("{0} = [{1}]", phi.Dst, total);
            return phi;
        }

        public Instruction VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
            {
                var eval = new ExpressionSimplifier(ctx, listener);
                ret.Expression.Accept(eval);
            }
            return ret;
        }

        public Instruction VisitSideEffect(SideEffect side)
        {
            var eval = new ExpressionSimplifier(ctx, listener);
            side.Expression.Accept(eval);
            return side;
        }

        public Instruction VisitStore(Store store)
        {
            var eval = new ExpressionSimplifier(ctx, listener);
            var value = store.Src.Accept(eval);
            ctx.SetValueEa(((MemoryAccess)store.Dst).EffectiveAddress, value);
            return store;
        }

        public Instruction VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// We're in the exit block now, so collect all identifiers
        /// that are registers into the procedureflow of the 
        /// current procedure.
        /// </summary>
        public Instruction VisitUseInstruction(UseInstruction use)
        {
            var id = use.Expression as Identifier;
            if (id == null || !(id.Storage is RegisterStorage))
                return use;
            var value = ctx.GetValue(id);
            if (value == Constant.Invalid)
            {
                ctx.ProcFlow.Trashed.Add(id.Storage);
                return use;
            }
            var c = value as Constant;
            if (c != null)
            {
                ctx.ProcFlow.Constants[id.Storage] = c;
                ctx.ProcFlow.Trashed.Add(id.Storage);
                return use;
            }
            var idV = value as Identifier;
            if (idV != null)
            {
                if (id.Storage == arch.StackRegister)
                {
                    if (idV == ctx.FramePointer)
                    {
                        ctx.ProcFlow.Preserved.Add(arch.StackRegister);
                        return use;
                    }
                }
                else if (idV.Storage == id.Storage)
                {
                    ctx.ProcFlow.Preserved.Add(id.Storage);
                    return use;
                }
            }
            ctx.ProcFlow.Trashed.Add(id.Storage);
            return use;
        }

        public class Context : EvaluationContext
        {
            public readonly Identifier FramePointer;
            public readonly Dictionary<Identifier, Expression> IdState;
            public readonly ProcedureFlow ProcFlow;
            public readonly Dictionary<int, Expression> StackState;
            public bool IsDirty;
            private readonly ExpressionValueComparer cmp;

            public Context(
                Identifier fp,
                Dictionary<Identifier, Expression> idState,
                ProcedureFlow procFlow) 
                : this(
                      fp,
                      idState,
                      procFlow, 
                      new Dictionary<int,Expression>(),
                      new ExpressionValueComparer())
            {
            }

            private Context(
                Identifier fp,
                Dictionary<Identifier, Expression> idState,
                ProcedureFlow procFlow,
                Dictionary<int, Expression> stack,
                ExpressionValueComparer cmp)
            {
                this.FramePointer = fp;
                this.IdState = idState;
                this.ProcFlow = procFlow;
                this.StackState = stack;
                this.cmp = cmp;
            }

            public Context Clone()
            {
                return new Context(this.FramePointer, this.IdState, this.ProcFlow, new Dictionary<int, Expression>(StackState), cmp);
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
                    return id;
                else
                    return value;
            }

            public bool IsUsedInPhi(Identifier id)
            {
                throw new NotImplementedException();
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
