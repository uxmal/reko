#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Expressions;
using System;
using System.Linq;

namespace Reko.Evaluation
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
            if (fn is InvalidConstant)
                return InvalidConstant.Create(appl.DataType);
            var exprs = new Expression[appl.Arguments.Length];
            for (int i = 0; i < exprs.Length; ++i)
            {
                var exp = appl.Arguments[i].Accept(this);
                if (fn is InvalidConstant)
                    return InvalidConstant.Create(appl.DataType);
                exprs[i] = exp;
            }
            return new Application(fn, appl.DataType, exprs);
        }

        public Expression VisitArrayAccess(ArrayAccess acc)
        {
            var arr = acc.Array.Accept(this);
            var idx = acc.Index.Accept(this);
            if (arr is InvalidConstant || idx is InvalidConstant)
                return InvalidConstant.Create(acc.DataType);
            return new ArrayAccess(acc.DataType, arr, idx);
        }

        public Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            var left = binExp.Left.Accept(this);
            var right = binExp.Right.Accept(this);
            if (left is InvalidConstant || right is InvalidConstant)
                return InvalidConstant.Create(binExp.DataType);
            return new BinaryExpression(
                binExp.Operator,
                binExp.DataType,
                left,
                right);
        }

        public Expression VisitCast(Cast cast)
        {
            var exp = cast.Expression.Accept(this);
            if (exp is InvalidConstant)
                return InvalidConstant.Create(cast.DataType);
            if (exp is Constant ||
                exp is Identifier)
                return new Cast(cast.DataType, exp);
            return InvalidConstant.Create(cast.DataType);
        }

        public Expression VisitConditionalExpression(ConditionalExpression c)
        {
            var cond = c.Condition.Accept(this);
            if (cond is InvalidConstant)
                return InvalidConstant.Create(c.DataType);
            var then = c.ThenExp.Accept(this);
            if (then is InvalidConstant)
                return InvalidConstant.Create(c.DataType);
            var fals = c.FalseExp.Accept(this);
            if (fals is InvalidConstant)
                return InvalidConstant.Create(c.DataType);
            return new ConditionalExpression(c.DataType, cond, then, fals);
        }

        public Expression VisitConditionOf(ConditionOf cof)
        {
            var exp = cof.Expression.Accept(this);
            if (exp is InvalidConstant)
                return exp;
            return new ConditionOf(exp);
        }

        public Expression VisitConstant(Constant c)
        {
            return c;
        }

        public Expression VisitConversion(Conversion conversion)
        {
            var exp = conversion.Expression.Accept(this);
            if (exp is InvalidConstant)
                return InvalidConstant.Create(conversion.DataType);
            if (exp is Constant ||
                exp is Identifier)
                return new Conversion(exp, conversion.SourceDataType, conversion.DataType);
           return InvalidConstant.Create(conversion.DataType);
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
            return ctx.GetValue(id)!;
        }

        public Expression VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public Expression VisitMemoryAccess(MemoryAccess access)
        {
            var ea = access.EffectiveAddress.Accept(this);
            if (ea is InvalidConstant)
                return InvalidConstant.Create(access.DataType);
            return new MemoryAccess(access.MemoryId, ea, access.DataType);
        }

        public Expression VisitMkSequence(MkSequence seq)
        {
            var newSeq = seq.Expressions.Select(e => e.Accept(this)).ToArray();
            if (newSeq.Any(e => e is InvalidConstant))
                return InvalidConstant.Create(seq.DataType);
            else
                return new MkSequence(seq.DataType, newSeq);
        }

        public Expression VisitOutArgument(OutArgument outArg)
        {
            return InvalidConstant.Create(outArg.DataType);
        }

        public Expression VisitPhiFunction(PhiFunction phi)
        {
            var args = new PhiArgument[phi.Arguments.Length];
            for (int i = 0; i < args.Length; ++i)
            {
                var exp = phi.Arguments[i].Value.Accept(this);
                if (exp is InvalidConstant)
                    return exp;
                args[i] = new PhiArgument(phi.Arguments[i].Block, exp);
            }
            return new PhiFunction(phi.DataType, args);
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
            if (seg is InvalidConstant)
                return seg;
            var off = access.EffectiveAddress.Accept(this);
            if (off is InvalidConstant)
                return off;
            return new SegmentedAccess(access.MemoryId, seg, off, access.DataType);
        }

        public Expression VisitSlice(Slice slice)
        {
            var exp = slice.Expression.Accept(this);
            if (exp is InvalidConstant)
                return exp;
            return new Slice(slice.DataType, exp, slice.Offset);
        }

        public Expression VisitStringConstant(StringConstant str)
        {
            return str;
        }

        public Expression VisitTestCondition(TestCondition tc)
        {
            var cond = tc.Expression.Accept(this);
            if (cond is InvalidConstant)
                return tc;
            return new TestCondition(tc.ConditionCode, cond);
        }

        public Expression VisitUnaryExpression(UnaryExpression unary)
        {
            var e = unary.Expression.Accept(this);
            if (e is InvalidConstant)
                return e;
            return new UnaryExpression(
                unary.Operator,
                unary.DataType,
                e);
        }
    }
}
