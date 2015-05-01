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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;


namespace Decompiler.Typing
{
    /// <summary>
    /// Collect type information by pulling type information from the leaves of expression trees to their roots.
    /// </summary>
    public class ExpressionTypeAscender : ExpressionVisitor<DataType>
    {
        private Platform platform;
        private TypeStore store;
        private TypeFactory factory;
        private Unifier unifier;

        public ExpressionTypeAscender(Platform platform, TypeStore store, TypeFactory factory)
        {
            this.platform = platform;
            this.store = store;
            this.factory = factory;
            this.unifier = new DataTypeBuilderUnifier(factory, store);
        }

        public DataType VisitAddress(Address addr)
        {
            return RecordDataType(
                PrimitiveType.Create(Domain.Pointer, addr.DataType.Size),
                addr);
        }

        public DataType VisitApplication(Application appl)
        {
            foreach (var a in appl.Arguments)
            {
                RecordDataType(a.Accept(this), a);
            }
            RecordDataType(appl.Procedure.Accept(this), appl.Procedure);
            return RecordDataType(appl.DataType, appl);
        }

        public DataType VisitArrayAccess(ArrayAccess acc)
        {
            DataType dtArr = acc.Array.Accept(this);
            DataType dtIdx = acc.Index.Accept(this);
            return RecordDataType(acc.DataType, acc);
        }

        private Exception NYI(Expression e)
        {
            return new NotImplementedException(string.Format("Not implemented: {0}", e));
        }

        public DataType VisitBinaryExpression(BinaryExpression binExp)
        {
            DataType dtLeft = binExp.Left.Accept(this);
            DataType dtRight = binExp.Right.Accept(this);
            DataType dt;
            if (binExp.Operator == Operator.IAdd)
            {
                dt = PullSumDataType(dtLeft, dtRight);
            }
            else if (binExp.Operator == Operator.ISub)
            {
                dt = PullDiffDataType(dtLeft, dtRight);
            }
            else if (binExp.Operator == Operator.And || 
                binExp.Operator == Operator.Or)
            {
                dt = PrimitiveType.CreateWord(dtLeft.Size).MaskDomain(Domain.Boolean | Domain.Integer | Domain.Character);
            }
            else if (binExp.Operator == Operator.IMul || binExp.Operator == Operator.Shl)
            {
                dt = PrimitiveType.CreateWord(binExp.DataType.Size).MaskDomain(Domain.Integer);
            }
            else if (binExp.Operator == Operator.SMul)
            {
                dt = PrimitiveType.CreateWord(binExp.DataType.Size).MaskDomain(Domain.SignedInt);
            }
            else if (binExp.Operator == Operator.UMul)
            {
                dt = PrimitiveType.CreateWord(binExp.DataType.Size).MaskDomain(Domain.UnsignedInt);
            }
            else if (binExp.Operator is ConditionalOperator)
            {
                dt = PrimitiveType.Bool;
            }
            else if (
                binExp.Operator == Operator.FAdd ||
                binExp.Operator == Operator.FSub ||
                binExp.Operator == Operator.FMul ||
                binExp.Operator == Operator.FDiv)
            {
                dt = PrimitiveType.Create(Domain.Real, dtLeft.Size);
            }
            else if (binExp.Operator == Operator.Shr)
            {
                dt = PrimitiveType.Create(Domain.UnsignedInt, dtLeft.Size);
            }
            else
                throw NYI(binExp);
            binExp.TypeVariable.DataType = dt;
            binExp.TypeVariable.OriginalDataType = dt;
            return dt;
        }

        private DataType PullSumDataType(DataType dtLeft, DataType dtRight)
        {
            var ptLeft = dtLeft as PrimitiveType;
            var ptRight = dtRight as PrimitiveType;
            if (ptLeft != null && ptLeft.Domain == Domain.Pointer ||
                dtLeft is Pointer)
            {
                if (ptRight.IsIntegral)
                    return PrimitiveType.Create(Domain.Pointer, dtLeft.Size);
            }
            if (ptLeft != null && ptLeft.IsIntegral)
            {
                if (ptRight != null)
                    return ptRight;
            }
            return dtLeft;
        }

        private DataType PullDiffDataType(DataType dtLeft, DataType dtRight)
        {
            var ptLeft = dtLeft as PrimitiveType;
            var ptRight = dtRight as PrimitiveType;
            if (ptLeft.Domain == Domain.Pointer || dtLeft is Pointer)
                throw new NotImplementedException(string.Format("Pulling difference {0} and {1}", dtLeft, dtRight));
            if (ptRight.Domain == Domain.Pointer || dtRight is Pointer)
                throw new NotImplementedException(string.Format("Pulling difference {0} and {1}", dtLeft, dtRight));
            if (ptLeft.IsIntegral)
                throw new NotImplementedException(string.Format("Pulling difference {0} and {1}", dtLeft, dtRight));
            if (ptRight.IsIntegral)
                throw new NotImplementedException(string.Format("Pulling difference {0} and {1}", dtLeft, dtRight));
            return dtLeft;
        }

        public DataType VisitCast(Cast cast)
        {
            cast.Expression.Accept(this);
            return cast.DataType;
        }

        public DataType VisitConditionOf(ConditionOf cof)
        {
            throw new NotImplementedException();
        }

        public DataType VisitConstant(Constant c)
        {
            if (c.TypeVariable.DataType == null)
            {
                c.TypeVariable.DataType = c.DataType;
                c.TypeVariable.OriginalDataType = c.DataType;
            }
            return c.TypeVariable.DataType;
        }

        public DataType VisitDepositBits(DepositBits d)
        {
            throw new NotImplementedException();
        }

        public DataType VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        public DataType VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public DataType VisitIdentifier(Identifier id)
        {
            if (id.TypeVariable.DataType == null)
            {
                id.TypeVariable.DataType = id.DataType;
                id.TypeVariable.OriginalDataType = id.DataType;
            }
            return id.DataType;
        }

        public DataType VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public DataType VisitMemoryAccess(MemoryAccess access)
        {
            return VisitMemoryAccessCommon(access, access.EffectiveAddress);
        }

        public DataType VisitMemoryAccessCommon(Expression access, Expression ea)
        {
            var dtEa = ea.Accept(this);
            var ptEa = dtEa as Pointer;
            DataType dt;
            if (ptEa != null)
                dt = ptEa.Pointee;
            else 
                dt = access.DataType;
            return RecordDataType(dt, access);
        }

        public DataType VisitMkSequence(MkSequence seq)
        {
            var dtHead = seq.Head.Accept(this);
            var dtTail = seq.Tail.Accept(this);
            if (IsSelector(dtHead))
            {
                return RecordDataType(PrimitiveType.Create(Domain.Pointer,  dtHead.Size + dtTail.Size), seq); 
            }
            throw new NotImplementedException();
        }

        private bool IsSelector(DataType dt)
        {
            var pt = dt as PrimitiveType;
            return pt != null && pt.Domain == Domain.Selector;
        }

        private bool IsIntegral(DataType dt)
        {
            var pt = dt as PrimitiveType;
            return pt != null && pt.IsIntegral;
        }

        public DataType VisitOutArgument(OutArgument outArgument)
        {
            var dt = outArgument.Expression.Accept(this);
            Expression exp = outArgument;
            return RecordDataType(PointerTo(outArgument.TypeVariable), exp);
        }

        private DataType PointerTo(TypeVariable tv)
        {
            return new Pointer(tv, platform.PointerType.Size);
        }

        private DataType RecordDataType(DataType dt, Expression exp)
        {
            exp.TypeVariable.DataType = unifier.Unify(exp.TypeVariable.DataType, dt);
            exp.TypeVariable.OriginalDataType = unifier.Unify(exp.TypeVariable.OriginalDataType, dt);
            return exp.TypeVariable.DataType;
        }

        public DataType VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        public DataType VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        public DataType VisitProcedureConstant(ProcedureConstant pc)
        {
            return pc.DataType;
        }

        public DataType VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        public DataType VisitSegmentedAccess(SegmentedAccess access)
        {
            access.BasePointer.Accept(this);
            return VisitMemoryAccessCommon(access, access.EffectiveAddress);
        }

        public DataType VisitSlice(Slice slice)
        {
            slice.Expression.Accept(this);
            return slice.DataType;
        }

        public DataType VisitTestCondition(TestCondition tc)
        {
            throw new NotImplementedException();
        }

        public DataType VisitUnaryExpression(UnaryExpression unary)
        {
            throw new NotImplementedException();
        }
    }
}
