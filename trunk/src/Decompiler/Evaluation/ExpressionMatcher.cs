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

using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Evaluation
{
    public class ExpressionMatcher : ExpressionVisitor<bool>
    {
        private Expression pattern;
        private Expression p;
        private Dictionary<string, Expression> capturedExpressions;

        public ExpressionMatcher(Expression pattern)
        {
            this.pattern = pattern;
            this.capturedExpressions = new Dictionary<string, Expression>();
        }

        public Expression CapturedExpression(string label)
        {
            Expression value;
            if (string.IsNullOrEmpty(label) || !capturedExpressions.TryGetValue(label, out value))
                return null;
            return value;
        }

        public bool Match(Expression expr)
        {
            return Match(pattern, expr);
        }

        private bool Match(Expression p, Expression expr)
        {
            this.p = p;
            return expr.Accept(this);
        }

        #region ExpressionVisitor<bool> Members

        bool ExpressionVisitor<bool>.VisitAddress(Decompiler.Core.Address addr)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitApplication(Application appl)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitBinaryExpression(BinaryExpression binExp)
        {
            var bP = p as BinaryExpression;
            if (bP == null)
                return false;
            if (binExp.Operator != bP.Operator)
                return false;

            return (Match(bP.Left, binExp.Left) && Match(bP.Right, binExp.Right));
        }

        bool ExpressionVisitor<bool>.VisitCast(Cast cast)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitConditionOf(ConditionOf cof)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitConstant(Constant c)
        {
            var anyC = p as WildConstant;
            if (anyC != null)
            {
                if (!string.IsNullOrEmpty(anyC.Label))
                    capturedExpressions.Add(anyC.Label, c);
                return true;
            }
            var cP = p as Constant;
            if (p == null)
                return false;
            return (c.ToInt64() == cP.ToInt64());
        }

        bool ExpressionVisitor<bool>.VisitDepositBits(DepositBits d)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitIdentifier(Identifier id)
        {
            var anyId = p as WildId;
            if (anyId != null)
            {
                if (!string.IsNullOrEmpty(anyId.Label))
                    capturedExpressions.Add(anyId.Label, id);
                return true;
            }
            var idP = p as Identifier;
            if (idP == null)
                return false;
            return (id.Name == idP.Name);
        }

        bool ExpressionVisitor<bool>.VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitMemoryAccess(MemoryAccess access)
        {
            var mp = p as MemoryAccess;
            if (mp == null)
                return false;
            if (mp.DataType.Size != access.DataType.Size)
                return false;
            return Match(mp.EffectiveAddress, access.EffectiveAddress);
        }

        bool ExpressionVisitor<bool>.VisitMkSequence(MkSequence seq)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitProcedureConstant(ProcedureConstant pc)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitSegmentedAccess(SegmentedAccess access)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitSlice(Slice slice)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitTestCondition(TestCondition tc)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitUnaryExpression(UnaryExpression unary)
        {
            throw new NotImplementedException();
        }

        #endregion

        public static Expression AnyConstant()
        {
            return new WildConstant(null);
        }

        public static Expression AnyConstant(string label)
        {
            return new WildConstant(label);
        }

        public static Expression AnyId()
        {
            return new WildId(null);
        }
        public static Expression AnyId(string label)
        {
            return new WildId(label);
        }

        private class WildExpression : Expression
        {
            public WildExpression(string label) : base(null)
            {
                this.Label = label;
            }

            public string Label { get; private set; }

            public override void Accept(IExpressionVisitor visit)
            {
                throw new NotSupportedException();
            }

            public override T Accept<T>(ExpressionVisitor<T> visitor)
            {
                throw new NotSupportedException();
            }

            public override Expression CloneExpression()
            {
                throw new NotSupportedException();
            }
        }

        private class WildConstant : WildExpression
        {
            public WildConstant(string label) : base(label)
            {
            }
        }

        private class WildId : WildExpression
        {
            public WildId(string label) : base(label)
            {
            }
        }
    }
}
