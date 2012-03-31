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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Analysis
{
    public class ExpressionPropagator : InstructionVisitor<Instruction>, ExpressionVisitor<Expression>
    {
        private ExpressionSimplifier eval;
        private SymbolicEvaluationContext ctx;
        private ProgramDataFlow flow;

        public ExpressionPropagator(ExpressionSimplifier eval, SymbolicEvaluationContext ctx, ProgramDataFlow flow)
        {
            this.eval = eval;
            this.ctx = ctx;
            this.flow = flow;
        }

        public Instruction VisitAssignment(Assignment a)
        {
            var src = a.Src.Accept(this);
            ctx.SetValue(a.Dst, src);
            return new Assignment(a.Dst, src);
        }

        public Instruction VisitBranch(Branch b)
        {
            return new Branch(
                b.Condition.Accept(this),
                b.Target);
        }

        public Instruction VisitCallInstruction(CallInstruction ci)
        {
            var proc = ci.Callee as Procedure;
            if (proc != null)
            {
                ctx.UpdateRegistersTrashedByProcedure(flow[proc]);
            }
            return ci;
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

        public Instruction VisitIndirectCall(IndirectCall ic)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
                ret.Expression.Accept(this);
            return ret;
        }

        public Instruction VisitSideEffect(SideEffect side)
        {
            return new SideEffect(side.Expression.Accept(this));
        }

        public Instruction VisitStore(Store store)
        {
            var src = store.Src.Accept(this);
            var dst = store.Dst.Accept(this);
            if (dst != Constant.Invalid)
                ctx.SetValueEa(dst, src);
            if (src != Constant.Invalid)
            {
                store.Src = src;
            }
            if (dst != Constant.Invalid)
            {
                store.Dst = dst;
            }
            return store;
        }

        public Instruction VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitUseInstruction(UseInstruction u)
        {
            throw new NotImplementedException();
        }

        public Expression VisitAddress(Address addr)
        {
            return addr;
        }

        public Expression VisitApplication(Application appl)
        {
            throw new NotImplementedException();
        }

        public Expression VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            var l = binExp.Left.Accept(this);
            var r = binExp.Right.Accept(this);
            var b = new BinaryExpression(binExp.op, binExp.DataType, l, r);
            return SimplifyExpression(b);
        }

        private Expression SimplifyExpression(Expression e)
        {
            var simp = e.Accept(eval);
            return (simp != Constant.Invalid) ? simp : e;
        }

        public Expression VisitCast(Cast cast)
        {
            var e = cast.Expression.Accept(this);
            return SimplifyExpression(new Cast(cast.DataType, e));
        }

        public Expression VisitConditionOf(ConditionOf cof)
        {
            var c = cof.Expression.Accept(this);
            return SimplifyExpression(new ConditionOf(c));
        }

        public Expression VisitConstant(Constant c)
        {
            return c;
        }

        public Expression VisitDepositBits(DepositBits dpb)
        {
            var d = new DepositBits(
                dpb.Source.Accept(this),
                dpb.InsertedBits.Accept(this),
                dpb.BitPosition,
                dpb.BitCount);
            return SimplifyExpression(d);
        }

        public Expression VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        public Expression VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public Expression VisitIdentifier(Identifier id)
        {
            return SimplifyExpression(id.Accept(eval));
        }

        public Expression VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public Expression VisitMemoryAccess(MemoryAccess access)
        {
            var m = new MemoryAccess(
                access.MemoryId,
                access.EffectiveAddress.Accept(this),
                access.DataType);
            return SimplifyExpression(m);
        }

        public Expression VisitMkSequence(MkSequence seq)
        {
            throw new NotImplementedException();
        }

        public Expression VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        public Expression VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        public Expression VisitProcedureConstant(ProcedureConstant pc)
        {
            return pc;
        }

        public Expression VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        public Expression VisitSegmentedAccess(SegmentedAccess access)
        {
            var m = new SegmentedAccess(
                access.MemoryId, 
                access.BasePointer.Accept(this),
                access.EffectiveAddress.Accept(this),
                access.DataType);
            return SimplifyExpression(m);
        }

        public Expression VisitSlice(Slice slice)
        {
            var s = new Slice(
                slice.DataType,
                slice.Expression.Accept(this),
                (byte) slice.Offset);
            return SimplifyExpression(s);
        }

        public Expression VisitTestCondition(TestCondition tc)
        {
            throw new NotImplementedException();
        }

        public Expression VisitUnaryExpression(UnaryExpression unary)
        {
            throw new NotImplementedException();
        }
    }
}
