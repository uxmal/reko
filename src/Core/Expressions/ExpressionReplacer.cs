
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Expressions
{
    public class ExpressionReplacer : ExpressionVisitor<Expression>
    {
        private ExpressionValueComparer cmp;
        private Expression original;
        private Expression replacement;

        private ExpressionReplacer(Expression original, Expression replacement)
        {
            this.original = original;
            this.replacement = replacement;
            this.cmp = new ExpressionValueComparer();
        }

        /// <summary>
        /// Replaces all occurrances of <paramref name="original"/> in 
        /// <paramref name="root"/> with <paramref name="replacement" />.
        /// </summary>
        /// <returns></returns>
        public static Expression Replace(Expression original, Expression replacement, Expression root)
        {
            var rep = new ExpressionReplacer(original, replacement);
            return root?.Accept(rep);
        }

        public Expression VisitAddress(Address addr)
        {
            if (cmp.Equals(addr, original))
                return replacement;
            else
                return addr;
        }

        public Expression VisitApplication(Application appl)
        {
            if (cmp.Equals(appl, original))
                return replacement;
            var args = appl.Arguments.Select(a => a.Accept(this)).ToArray();
            var fn = appl.Procedure;
            return new Application(fn, appl.DataType, args);
        }

        public Expression VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            if (cmp.Equals(binExp, original))
                return replacement;
            var left = binExp.Left.Accept(this);
            var right = binExp.Right.Accept(this);
            return new BinaryExpression(binExp.Operator, binExp.DataType, left, right);
        }

        public Expression VisitCast(Cast cast)
        {
            if (cmp.Equals(cast, original))
                return replacement;
            var exp = cast.Expression.Accept(this);
            return new Cast(cast.DataType, exp);
        }

        public Expression VisitConditionalExpression(ConditionalExpression cond)
        {
            throw new NotImplementedException();
        }

        public Expression VisitConditionOf(ConditionOf cof)
        {
            if (cmp.Equals(cof, original))
                return replacement;
            var expr = cof.Expression.Accept(this);
            return new ConditionOf(expr);
        }

        public Expression VisitConstant(Constant c)
        {
            if (cmp.Equals(c, original))
                return replacement;
            else
                return c;
        }

        public Expression VisitDepositBits(DepositBits d)
        {
            if (cmp.Equals(d, original))
                return replacement;
            var src = d.Source.Accept(this);
            var bits = d.InsertedBits.Accept(this);
            return new DepositBits(src, bits, d.BitPosition);
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
            if (cmp.Equals(id, original))
                return replacement;
            else
                return id;
        }

        public Expression VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public Expression VisitMemoryAccess(MemoryAccess access)
        {
            if (cmp.Equals(access, original))
                return replacement;
            var ea = access.EffectiveAddress.Accept(this);
            return new MemoryAccess(ea, access.DataType);
        }

        public Expression VisitMkSequence(MkSequence seq)
        {
            if (cmp.Equals(seq, original))
                return replacement;
            var exprs = seq.Expressions
                .Select(e => e.Accept(this))
                .ToArray();
            return new MkSequence(seq.DataType, exprs);
        }

        public Expression VisitOutArgument(OutArgument outArgument)
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
            if (cmp.Equals(pc, original))
                return replacement;
            else
                return pc;
        }

        public Expression VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        public Expression VisitSegmentedAccess(SegmentedAccess access)
        {
            if (cmp.Equals(access, original))
                return replacement;
            var seg = access.BasePointer.Accept(this);
            var off = access.EffectiveAddress.Accept(this);
            return new SegmentedAccess(access.MemoryId, seg, off, access.DataType);
        }

        public Expression VisitSlice(Slice slice)
        {
            if (cmp.Equals(slice, original))
                return replacement;
            var exp = slice.Expression.Accept(this);
            return new Slice(slice.DataType, exp, slice.Offset);
        }

        public Expression VisitTestCondition(TestCondition tc)
        {
            throw new NotImplementedException();
        }

        public Expression VisitUnaryExpression(UnaryExpression unary)
        {
            throw new NotImplementedException();
        }

        public static Expression Replace(Expression dst, object srcExpr, Expression jumpTableFormat)
        {
            throw new NotImplementedException();
        }
    }
}
