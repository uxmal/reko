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
using Reko.Core.Rtl;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    /// <summary>
    /// Walks code backwards, performing a backward slice, to find
    /// "dominating" comparisons against constants, which may provide
    /// vector table limits.
    /// </summary>
    public class Backwalker2<TBlock, TInstr> : ExpressionVisitor<object>
    {
        private ExpressionSimplifier eval;
        private IBackWalkHost<TBlock, TInstr> host;
        private Expression jtt;
        private Expression tableBase;

        public Backwalker2(IBackWalkHost<TBlock, TInstr> host, RtlTransfer xfer, ExpressionSimplifier eval)
        {
            this.host = host;
            this.eval = eval;
            this.IndexVariables = new Dictionary<Expression, VariableInfo>();
            jtt = xfer.Target;
            jtt.Accept(this);
        }

        public Dictionary<Expression, VariableInfo> IndexVariables { get; }

        public object VisitAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        public object VisitApplication(Application appl)
        {
            throw new NotImplementedException();
        }

        public object VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public object VisitBinaryExpression(BinaryExpression binExp)
        {
            throw new NotImplementedException();
        }

        public object VisitCast(Cast cast)
        {
            throw new NotImplementedException();
        }

        public object VisitConditionalExpression(ConditionalExpression cond)
        {
            throw new NotImplementedException();
        }

        public object VisitConditionOf(ConditionOf cof)
        {
            throw new NotImplementedException();
        }

        public object VisitConstant(Constant c)
        {
            throw new NotImplementedException();
        }

        public object VisitDepositBits(DepositBits d)
        {
            throw new NotImplementedException();
        }

        public object VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        public object VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public object VisitIdentifier(Identifier id)
        {
            this.IndexVariables[id] = new VariableInfo
            {
            };
            return null;
        }

        public object VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public object VisitMemoryAccess(MemoryAccess access)
        {
            access.EffectiveAddress.Accept(this);
            return null;
        }

        public object VisitMkSequence(MkSequence seq)
        {
            throw new NotImplementedException();
        }

        public object VisitOutArgument(OutArgument outArgument)
        {
            throw new NotImplementedException();
        }

        public object VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        public object VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        public object VisitProcedureConstant(ProcedureConstant pc)
        {
            throw new NotImplementedException();
        }

        public object VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        public object VisitSegmentedAccess(SegmentedAccess access)
        {
            throw new NotImplementedException();
        }

        public object VisitSlice(Slice slice)
        {
            throw new NotImplementedException();
        }

        public object VisitTestCondition(TestCondition tc)
        {
            throw new NotImplementedException();
        }

        public object VisitUnaryExpression(UnaryExpression unary)
        {
            throw new NotImplementedException();
        }

        public class VariableInfo
        { }

    }
}
