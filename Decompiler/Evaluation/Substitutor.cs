#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Evaluation
{
    public class Substitutor : ExpressionVisitor<Expression>
    {
        private EvaluationContext ctx;

        public Substitutor(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public Expression VisitAddress(Address addr)
        {
            return addr;
        }

        public Expression VisitApplication(Application appl)
        {
            var fn = appl.Procedure.Accept(this);
            if (fn == Constant.Invalid)
                return fn;
            var exprs = new Expression[appl.Arguments.Length];
            for (int i = 0; i < exprs.Length; ++i)
            {
                var exp = appl.Arguments[i].Accept(this);
                if (exp == Constant.Invalid)
                    return exp;
                exprs[i] = exp;
            }
            return new Application(fn, appl.DataType, exprs);
        }

        public Expression VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            var left = binExp.Left.Accept(this);
            var right = binExp.Right.Accept(this);
            if (left == Constant.Invalid || right == Constant.Invalid)
                return Constant.Invalid;
            return new BinaryExpression(
                binExp.Operator,
                binExp.DataType,
                left,
                right);
        }

        public Expression VisitCast(Cast cast)
        {
            var exp = cast.Expression.Accept(this);
            if (exp == Constant.Invalid)
                return exp;
            return new Cast(cast.DataType, exp);
        }

        public Expression VisitConditionOf(ConditionOf cof)
        {
            var exp = cof.Expression.Accept(this);
            if (exp == Constant.Invalid)
                return exp;
            return new ConditionOf(exp);
        }

        public Expression VisitConstant(Constant c)
        {
            return c;
        }

        public Expression VisitDepositBits(DepositBits d)
        {
            var source = d.Source.Accept(this);
            if (source == Constant.Invalid)
                return source;
            var inserted = d.InsertedBits.Accept(this);
            if (inserted == Constant.Invalid)
                return inserted;
            return new DepositBits(source, inserted, d.BitPosition, d.BitCount);
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
            return ctx.GetValue(id);
        }

        public Expression VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public Expression VisitMemoryAccess(MemoryAccess access)
        {
            var ea = access.EffectiveAddress.Accept(this);
            if (ea == Constant.Invalid)
                return Constant.Invalid;
            return new MemoryAccess(access.MemoryId, ea, access.DataType);
        }

        public Expression VisitMkSequence(MkSequence seq)
        {
            throw new NotImplementedException();
        }

        public Expression VisitOutArgument(OutArgument outArg)
        {
            return Constant.Invalid;
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
            var seg = access.BasePointer.Accept(this);
            if (seg == Constant.Invalid)
                return seg;
            var off = access.EffectiveAddress.Accept(this);
            if (off == Constant.Invalid)
                return off;
            return new SegmentedAccess(access.MemoryId, seg, off, access.DataType);
        }

        public Expression VisitSlice(Slice slice)
        {
            throw new NotImplementedException();
        }

        public Expression VisitTestCondition(TestCondition tc)
        {
            var cond = tc.Expression.Accept(this);
            if (cond == Constant.Invalid)
                return tc;
            return new TestCondition(tc.ConditionCode, cond);
        }

        public Expression VisitUnaryExpression(UnaryExpression unary)
        {
            var e = unary.Expression.Accept(this);
            if (e == Constant.Invalid)
                return e;
            return new UnaryExpression(
                unary.Operator,
                unary.DataType,
                e);
        }
    }
}
