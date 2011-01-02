#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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
using System.Text;

namespace Decompiler.Scanning
{
    /// <summary>
    /// Used by the Scanner and related classes to evaluate possibly constant expressions.
    /// </summary>
    public class ScannerEvaluator : IExpressionVisitor
    {
        private ProcessorState state;
        private Constant value;

        public ScannerEvaluator(ProcessorState state)
        {
            this.state = state;
        }

        public Constant GetValue(Expression expr)
        {
            value = Constant.Invalid;
            expr.Accept(this);
            return value;
        }

        #region IExpressionVisitor Members

        void IExpressionVisitor.VisitAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitApplication(Application appl)
        {
            value = Constant.Invalid;
        }

        void IExpressionVisitor.VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitBinaryExpression(BinaryExpression bin)
        {
            // Special case XOR/SUB (self,self)
            if ((bin.op == Operator.Xor ||
                bin.op == Operator.Sub) && bin.Left == bin.Right)
            {
                value = Constant.Zero(bin.Left.DataType);
            }
            else
            {
                var c1 = GetValue(bin.Left);
                var c2 = GetValue(bin.Right);
                if (c1.IsValid && c2.IsValid)
                {
                    value = bin.op.ApplyConstants(c1, c2);
                }
            }
        }

        void IExpressionVisitor.VisitCast(Cast cast)
        {
            value = Constant.Invalid;
        }

        void IExpressionVisitor.VisitConditionOf(ConditionOf cof)
        {
            value = Constant.Invalid;
        }


        void IExpressionVisitor.VisitConstant(Constant c)
        {
            value = c;
        }

        void IExpressionVisitor.VisitDepositBits(DepositBits d)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitIdentifier(Identifier id)
        {
            var reg = id.Storage as RegisterStorage;
            if (reg != null)
            {
                value = state.Get(reg.Register);
            }
        }

        void IExpressionVisitor.VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitMemoryAccess(MemoryAccess access)
        {
            value = Constant.Invalid;
        }

        void IExpressionVisitor.VisitMkSequence(MkSequence seq)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitProcedureConstant(ProcedureConstant pc)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitSegmentedAccess(SegmentedAccess access)
        {
            value = Constant.Invalid;
        }

        void IExpressionVisitor.VisitSlice(Slice slice)
        {
            throw new NotImplementedException();
        }

        void IExpressionVisitor.VisitTestCondition(TestCondition tc)
        {
            value = Constant.Invalid;
        }

        void IExpressionVisitor.VisitUnaryExpression(UnaryExpression unary)
        {
            var c1 = GetValue(unary.Expression);
            if (c1.IsValid)
            {
                value = unary.op.ApplyConstant(c1);
            }
        }

        #endregion
    }
}
