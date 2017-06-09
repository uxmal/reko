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
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    /// <summary>
    /// Propagates expressions used in TrashedRegisters to replace expressions
    /// of the type
    ///     mem[fp + c]
    /// with
    ///     idC
    /// and expressions of the type
    ///     fp + c
    /// with
    ///     &idC
    /// </summary>
    public class ExpressionPropagator : InstructionVisitor<Instruction>, ExpressionVisitor<ExpressionPropagator.Result>
    {
        public class Result
        {
            public Expression Value;
            public Expression PropagatedExpression;
            
            public override string ToString()
            {
                return string.Format("V: {0}; E:{1}", Value, PropagatedExpression);
            }
        }

        private IPlatform platform;
        private ExpressionSimplifier eval;
        private SymbolicEvaluationContext ctx;
        private Substitutor sub;

        private ProgramDataFlow flow;
        private bool storing;

        public ExpressionPropagator(
            IPlatform platform, 
            ExpressionSimplifier simplifier, 
            SymbolicEvaluationContext ctx, 
            ProgramDataFlow flow)
        {
            this.platform = platform;
            this.eval = simplifier;
            this.ctx = ctx;
            this.flow = flow;
            this.sub = new Substitutor(ctx);
        }

        public Instruction VisitAssignment(Assignment ass)
        {
            var src = ass.Src.Accept(this);
            ctx.SetValue(ass.Dst, src.Value);
            return new Assignment(ass.Dst, src.PropagatedExpression);
        }

        private bool MayReplace(Expression exp)
        {
            if (exp == ctx.Frame.FramePointer)
                return true;
            if (exp is Constant)
                return true;
            var id = exp as Identifier;
            if (id != null && (id.Storage is StackStorage))
                return true;
            if (IsConstantOffsetFromFramePointer(exp))
                return true;
            return false;
        }

        public Instruction VisitBranch(Branch b)
        {
            return new Branch(
                b.Condition.Accept(this).PropagatedExpression,
                b.Target);
        }

        public Instruction VisitCallInstruction(CallInstruction ci)
        {
            ci.CallSite.StackDepthOnEntry =
                GetStackDepthBeforeCall() +
                ci.CallSite.SizeOfReturnAddressOnStack;
            var pc = ci.Callee as ProcedureConstant;
            if (pc != null)
            {
                var proc = pc.Procedure as Procedure;
                if (proc != null)
                {
                    ctx.UpdateRegistersTrashedByProcedure(flow[proc]);
                }
                return ci;
            }
            else
            {
                var fn = ci.Callee.Accept(this);
                // Hell node: will want to assume that registers which aren't
                // guaranteed to be preserved by the ABI are trashed.
                foreach (var r in ctx.RegisterState.Keys.ToList())
                {
                    foreach (var reg in platform.CreateTrashedRegisters())
                    {
                        //$PERF: not happy about the O(n^2) algorithm,
                        // but this is better in the analysis-development 
                        // branch.
                        if (r.Domain == reg.Domain)
                        {
                            ctx.RegisterState[r] = Constant.Invalid;
                        }
                    }
                }
                return new CallInstruction(fn.PropagatedExpression, ci.CallSite);
            }
        }

        private int GetStackDepthBeforeCall()
        {
            var spVal = ctx.RegisterState[platform.Architecture.StackRegister];
            if (ctx.IsFramePointer(spVal))
                return 0;
            var bin = spVal as BinaryExpression;
            if (bin == null || !ctx.IsFramePointer(bin.Left))
                return 0;
            var c = bin.Right as Constant;
            if (c == null)
                throw new NotImplementedException("Expected stack depth to be known.");
            int depth = c.ToInt32();
            if (bin.Operator == Operator.ISub)
                depth = -depth;
            if (depth > 0)
                throw new NotImplementedException("Expected stack depth to be negative.");
            return -depth;
        }

        public Instruction VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
                ret.Expression = ret.Expression.Accept(this).PropagatedExpression;
            return ret;
        }

        public Instruction VisitSideEffect(SideEffect side)
        {
            return new SideEffect(side.Expression.Accept(this).PropagatedExpression);
        }

        public Instruction VisitStore(Store store)
        {
            var src = store.Src.Accept(this);
            this.storing = true;
            var dst = store.Dst.Accept(this);
            this.storing = false;

            var m = dst.PropagatedExpression as MemoryAccess;
            if (m != null)
            {
                ctx.SetValueEa(m.EffectiveAddress, src.Value);
            }
            else
            {
                var sm = dst.PropagatedExpression as SegmentedAccess;
                if (sm != null)
                {
                    ctx.SetValueEa(sm.BasePointer, sm.EffectiveAddress, src.Value);
                }
            }

            var idDst = dst.PropagatedExpression as Identifier;
            if (idDst != null)
            {
                ctx.SetValue(idDst, src.Value);
                return new Assignment(idDst, src.PropagatedExpression);
            }
            else if (!(dst.PropagatedExpression is Constant))
            {
                return new Store(dst.PropagatedExpression, src.PropagatedExpression);
            }
            else
            {
                return new Store(store.Dst, src.PropagatedExpression);
            }
        }

        public Instruction VisitSwitchInstruction(SwitchInstruction si)
        {
            return new SwitchInstruction(
                si.Expression.Accept(this).PropagatedExpression,
                si.Targets);
        }

        public Instruction VisitUseInstruction(UseInstruction u)
        {
            throw new NotImplementedException();
        }

        public Result VisitAddress(Address addr)
        {
            return new Result { Value = addr, PropagatedExpression = addr };
        }

        public Result VisitApplication(Application appl)
        {
            var fn = SimplifyExpression(appl.Procedure);
            var args = new Expression[appl.Arguments.Length];
            for (int i = 0; i < args.Length; ++i)
            {
                args[i] = appl.Arguments[i].Accept(this).PropagatedExpression;
            }
            var a = new Application(fn.PropagatedExpression, appl.DataType, args);
            return SimplifyExpression(a);
        }

        public Result VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public Result VisitBinaryExpression(BinaryExpression binExp)
        {
            var l = binExp.Left.Accept(this).PropagatedExpression;
            var r = binExp.Right.Accept(this).PropagatedExpression;
            var b = new BinaryExpression(binExp.Operator, binExp.DataType, l, r);
            return SimplifyExpression(b);
        }

        private Result SimplifyExpression(Expression e)
        {
            var simp = e.Accept(eval);
            if (simp == Constant.Invalid)
                return new Result { Value = simp, PropagatedExpression = e };
            if (ctx.IsFramePointer(simp))
                return new Result { Value = simp, PropagatedExpression = simp };
            if (simp is Constant)
                return new Result { Value = simp, PropagatedExpression = simp };

            if (IsConstantOffsetFromFramePointer(simp) &&
                !(e is MemoryAccess || e is SegmentedAccess))
            {
                return new Result { Value = simp, PropagatedExpression = simp };
            }
            return new Result { Value = simp, PropagatedExpression = e };
        }

        private bool IsConstantOffsetFromFramePointer(Expression e)
        {
            var binExp = e as BinaryExpression;
            if (binExp == null)
                return false;
            if (binExp.Operator != Operator.IAdd && binExp.Operator != Operator.ISub)
                return false;
            return ctx.IsFramePointer(binExp.Left);
        }

        private Result ConvertToParamOrLocal(Result res)
        {
            var m = res.PropagatedExpression as MemoryAccess;
            if (m == null)
                return res;
            var address = m.EffectiveAddress;
            return ConvertAddressToStackVariable(res, address);
        }

        private Result ConvertAddressToStackVariable(Result res, Expression address)
        {
            if (ctx.IsFramePointer(address))
                return new Result { Value = res.Value, PropagatedExpression= ctx.Frame.EnsureStackArgument(0, res.PropagatedExpression.DataType) };
            var bin = address as BinaryExpression;
            if (bin == null)
                return res;
            if (!ctx.IsFramePointer(bin.Left))
                return res;
            var c = bin.Right as Constant;
            if (c == null)
                return res;
            int cc = c.ToInt32();
            if (bin.Operator == Operator.ISub)
                cc = -cc;
            var sv = ctx.Frame.EnsureStackVariable(cc, res.PropagatedExpression.DataType);
            if (sv.DataType.Size > res.PropagatedExpression.DataType.Size)
            {
                return new Result { Value = res.Value, PropagatedExpression = new Slice(res.PropagatedExpression.DataType, sv, 0) };
            }
            else
            {
                return new Result { Value=res.Value, PropagatedExpression = sv };
            }
        }

        public Result VisitCast(Cast cast)
        {
            var e = cast.Expression.Accept(this);
            return SimplifyExpression(new Cast(cast.DataType, e.PropagatedExpression));
        }

        public Result VisitConditionalExpression(ConditionalExpression cond)
        {
            var c = cond.Condition.Accept(this).PropagatedExpression;
            var t = cond.ThenExp.Accept(this).PropagatedExpression;
            var e = cond.FalseExp.Accept(this).PropagatedExpression;
            return SimplifyExpression(new ConditionalExpression(cond.DataType, c, t, e));
        }

        public Result VisitConditionOf(ConditionOf cof)
        {
            var e = cof.Expression.Accept(this);
            return SimplifyExpression(new ConditionOf(e.PropagatedExpression));
        }

        public Result VisitConstant(Constant c)
        {
            return new Result { Value = c, PropagatedExpression = c };
        }

        public Result VisitDepositBits(DepositBits dpb)
        {
            var d = new DepositBits(
                dpb.Source.Accept(this).PropagatedExpression,
                dpb.InsertedBits.Accept(this).PropagatedExpression,
                dpb.BitPosition);
            return SimplifyExpression(d);
        }

        public Result VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        public Result VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public Result VisitIdentifier(Identifier id)
        {
            var ev = id.Accept(eval);
            if (!MayReplace(ev))
                return new Result { Value = ctx.GetValue(id), PropagatedExpression = id };
            else 
                return SimplifyExpression(ev);
        }

        public Result VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public Result VisitMemoryAccess(MemoryAccess access)
        {
            bool storing = this.storing;
            this.storing = false;
            var m = new MemoryAccess(
                access.MemoryId,
                access.EffectiveAddress.Accept(this).PropagatedExpression,
                access.DataType);
            if (storing)
            {
                return ConvertToParamOrLocal(new Result { Value = Constant.Invalid, PropagatedExpression = m });
            }
            else
            {
                return ConvertToParamOrLocal(SimplifyExpression(m));
            }
        }

        public Result VisitMkSequence(MkSequence seq)
        {
            var h = SimplifyExpression(seq.Head).PropagatedExpression;
            var t = SimplifyExpression(seq.Tail).PropagatedExpression;
            return SimplifyExpression(new MkSequence(
                seq.DataType, h, t));
        }

        public Result VisitOutArgument(OutArgument outArg)
        {
            var id = outArg.Expression as Identifier;
            if (id != null)
            {
                ctx.SetValue(id, Constant.Invalid);
                return new Result
                {
                    PropagatedExpression = outArg,
                    Value = Constant.Invalid,
                };
            }
            var exp = SimplifyExpression(outArg.Expression);
            return new Result
            {
                PropagatedExpression = new OutArgument(outArg.DataType, exp.PropagatedExpression),
                Value = Constant.Invalid
            };
        }

        public Result VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        public Result VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        public Result VisitProcedureConstant(ProcedureConstant pc)
        {
            return new Result { Value = pc, PropagatedExpression = pc };
        }

        public Result VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        public Result VisitSegmentedAccess(SegmentedAccess access)
        {
            var m = new SegmentedAccess(
                access.MemoryId, 
                SimplifyExpression(access.BasePointer).PropagatedExpression,
                SimplifyExpression(access.EffectiveAddress).PropagatedExpression,
                access.DataType);
            if (storing)
            {
                return ConvertToParamOrLocal(new Result { Value = Constant.Invalid, PropagatedExpression = m });
            }
            else
            {
                return ConvertToParamOrLocal(SimplifyExpression(m));
            }
        }

        public Result VisitSlice(Slice slice)
        {
            var s = new Slice(
                slice.DataType,
                SimplifyExpression(slice.Expression).PropagatedExpression,
                (byte) slice.Offset);
            return SimplifyExpression(s);
        }

        public Result VisitTestCondition(TestCondition tc)
        {
            var t = new TestCondition(
                tc.ConditionCode,
                SimplifyExpression(tc.Expression).PropagatedExpression);
            return SimplifyExpression(t);
        }

        public Result VisitUnaryExpression(UnaryExpression unary)
        {
            if (unary.Operator == Operator.AddrOf && unary.Expression is Identifier)
                return new Result { Value = Constant.Invalid, PropagatedExpression = unary };
            return SimplifyExpression(
                new UnaryExpression(unary.Operator, unary.DataType,
                    SimplifyExpression(unary.Expression).PropagatedExpression));
        }
    }
}
