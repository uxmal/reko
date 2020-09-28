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

using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Linq;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Collect type information by pulling type information from
    /// the leaves of expression trees to their roots.
    /// </summary>
    /// <remarks>
    ///    root
    ///  ↑ /  \ ↑
    /// leaf  leaf
    /// </remarks>
    public abstract class ExpressionTypeAscenderBase : ExpressionVisitor<DataType>
    {
        private IPlatform platform;
        private TypeFactory factory;
        private StructureType globalFields;

        public ExpressionTypeAscenderBase(Program program, TypeFactory factory)
        {
            this.platform = program.Platform;
            this.globalFields = program.GlobalFields;
            this.factory = factory;
        }

        protected abstract DataType RecordDataType(DataType dt, Expression exp);
        protected abstract DataType EnsureDataType(DataType dt, Expression exp);

        public DataType VisitAddress(Address addr)
        {
            var c = addr.ToConstant();
            c.DataType = PrimitiveType.Create(Domain.Pointer, addr.DataType.BitSize);
            var dt = ExistingGlobalField(c) ?? addr.DataType;
            return RecordDataType(dt, addr);
        }

        public DataType VisitApplication(Application appl)
        {
            foreach (var a in appl.Arguments)
            {
                RecordDataType(a.Accept(this), a);
            }
            RecordDataType(appl.Procedure.Accept(this), appl.Procedure);
            var dt = RecordApplicationReturnType(appl.Procedure, appl);
            return dt;
        }

        private DataType RecordApplicationReturnType(Expression pfn, Application appl)
        {
            var dt = RecordDataType(appl.DataType, appl);
            if (pfn is ProcedureConstant pc && 
                pc.Procedure.Signature.ParametersValid)
            {
                dt = RecordDataType(pc.Procedure.Signature.ReturnValue.DataType, appl);
            }
            return dt;
        }
        public DataType VisitArrayAccess(ArrayAccess acc)
        {
            acc.Array.Accept(this);
            acc.Index.Accept(this);
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
                dt = GetPossibleFieldType(dtLeft, dtRight, binExp.Right);
                if (dt == null)
                {
                    dt = PullSumDataType(dtLeft, dtRight);
                }
            }
            else if (binExp.Operator == Operator.ISub)
            {
                dt = PullDiffDataType(dtLeft, dtRight);
            }
            else if (binExp.Operator == Operator.And || 
                binExp.Operator == Operator.Or)
            {
                dt = PrimitiveType.CreateWord(dtLeft.BitSize).MaskDomain(Domain.Boolean | Domain.Integer | Domain.Character);
            }
            else if (
                binExp.Operator == Operator.IMul ||
                binExp.Operator == Operator.Shl ||
                binExp.Operator == Operator.IMod)
            {
                dt = PrimitiveType.CreateWord(binExp.DataType.BitSize).MaskDomain(Domain.Integer);
            }
            else if (
                binExp.Operator == Operator.SMul ||
                binExp.Operator == Operator.SDiv)
            {
                dt = PrimitiveType.CreateWord(binExp.DataType.BitSize).MaskDomain(Domain.SignedInt);
            }
            else if (
                binExp.Operator == Operator.UMul ||
                binExp.Operator == Operator.UDiv)
            {
                dt = PrimitiveType.CreateWord(binExp.DataType.BitSize).MaskDomain(Domain.UnsignedInt);
            }
            else if (binExp.Operator is ConditionalOperator ||
                binExp.Operator is CorOperator ||
                binExp.Operator is CandOperator)
            {
                dt = PrimitiveType.Bool;
            }
            else if (
                binExp.Operator == Operator.FAdd ||
                binExp.Operator == Operator.FSub ||
                binExp.Operator == Operator.FMul ||
                binExp.Operator == Operator.FDiv)
            {
                dt = PrimitiveType.Create(Domain.Real, binExp.DataType.BitSize);
            }
            else if (binExp.Operator == Operator.Shr)
            {
                dt = PrimitiveType.Create(Domain.UnsignedInt, dtLeft.BitSize);
            }
            else if (binExp.Operator == Operator.Sar)
            {
                dt = PrimitiveType.Create(Domain.SignedInt, dtLeft.BitSize);
            }
            else if (binExp.Operator == Operator.Xor ||
                     binExp.Operator == Operator.Shl)
            {
                dt = PrimitiveType.Create(Domain.Integer, dtLeft.BitSize);
            }
            else
                throw NYI(binExp);
            return RecordDataType(dt, binExp);
        }

        /// <summary>
        /// If dtLeft is a (ptr (struct ...)) and dtRight is a non-pointer
        /// constant, this could be a field access.
        /// </summary>
        /// <param name="dtLeft">Possible pointer to a structure</param>
        /// <param name="dtRight">Type of possible offset</param>
        /// <param name="right">Possible constant offset from start of structure</param>
        /// <returns>A (ptr field-type) if it was a ptr-to-struct, else null.</returns>
        private Pointer GetPossibleFieldType(DataType dtLeft, DataType dtRight, Expression right)
        {
            if (right is Constant cOffset)
            {
                if (dtRight is PrimitiveType ptRight && ptRight.Domain != Domain.Pointer)
                {
                    int offset = cOffset.ToInt32();
                    return GetPossibleFieldType(dtLeft, offset);
                }
            }
            return null;
        }

        /// <summary>
        /// If dtLeft is a (ptr (struct ...)) and there is a field at the
        /// given offset in that structure, this could be a field access.
        /// </summary>
        /// <param name="dtLeft">Possible pointer to a structure</param>
        /// <param name="offset"></param>
        /// <returns>A (ptr field-type) if it was a ptr-to-struct, else null.</returns>
        private Pointer GetPossibleFieldType(DataType dtLeft, int offset)
        {
            var ptrLeft = dtLeft.ResolveAs<Pointer>();
            if (ptrLeft == null)
                return null;

            var pointee = ptrLeft.Pointee;
            var strPointee = pointee.ResolveAs<StructureType>();
            if (strPointee == null)
                return null;

            var field = strPointee.Fields.LowerBound(offset);
            if (field == null)
                return null;
            // We're collecting _DataTypes_, so if we encounter
            // a TypeReference, we need to drill past it.
            var dtField = field.DataType.ResolveAs<DataType>();
            if (dtField == null)
                return null;
            //$TODO This really should be offset != field.Offset
            // However doing so causes a regression in hello_ppc.exe
            if (offset >= field.Offset + dtField.Size)
                return null;
            return factory.CreatePointer(dtField, dtLeft.BitSize);
        }

        private DataType PullSumDataType(DataType dtLeft, DataType dtRight)
        {
            var ptLeft = dtLeft.ResolveAs<PrimitiveType>();
            var ptRight = dtRight.ResolveAs<PrimitiveType>();
            if (ptLeft != null && ptLeft.Domain == Domain.Pointer)
            {
                if (ptRight != null && ptRight.Domain != Domain.Pointer)
                    return PrimitiveType.Create(Domain.Pointer, dtLeft.BitSize);
            }

            if (ptLeft != null && ptLeft.IsIntegral && ptRight != null && ptRight.IsIntegral)
            {
                // According to the C language definition, the sum
                // of unsigned and signed integers is always unsigned.
                if (ptLeft.Domain == Domain.UnsignedInt)
                {
                    return ptLeft;
                }
                if (ptRight.Domain == Domain.UnsignedInt)
                {
                    return ptRight;
                }
            }
            if (dtLeft.ResolveAs<Pointer>() != null)
            {
                if (dtLeft is TypeReference)
                    return dtLeft;
                else 
                    return PrimitiveType.Create(Domain.Pointer, dtLeft.BitSize);
            }
            return dtLeft;
        }

        private DataType PullDiffDataType(DataType dtLeft, DataType dtRight)
        {
            var ptLeft = dtLeft as PrimitiveType;
            var ptRight = dtRight.ResolveAs<PrimitiveType>();
            if (ptLeft != null && ptLeft.Domain == Domain.Pointer || 
                dtLeft is Pointer)
            {
                if (ptRight != null)
                {
                    if ((ptRight.Domain & Domain.Integer) != 0)
                        return PrimitiveType.Create(Domain.Pointer, dtLeft.BitSize);
                    else if ((ptRight.Domain & Domain.Pointer) != 0)
                        return PrimitiveType.Create(Domain.SignedInt, dtLeft.BitSize);
                }
                if (dtRight is Pointer)
                    return PrimitiveType.Create(Domain.SignedInt, dtLeft.BitSize);
                throw new TypeInferenceException(string.Format("Pulling difference {0} and {1}", dtLeft, dtRight));
            }
            if (ptRight != null && ptRight.Domain == Domain.Pointer || 
                dtRight is Pointer)
            {
                if (ptRight != null && (ptRight.Domain & Domain.Integer) != 0)
                    return dtLeft;
                // If a dtRight is a pointer and it's being subtracted from 
                // something, then the result has to be a ptrdiff_t, i.e.
                // integer.
                if (ptLeft != null && (ptLeft.Domain & Domain.Pointer) != 0)
                    return PrimitiveType.Create(Domain.Integer, dtLeft.BitSize);
                throw new TypeInferenceException(string.Format("Pulling difference {0} and {1}", dtLeft, dtRight));
            }
            return dtLeft;
        }

        public DataType VisitCast(Cast cast)
        {
            cast.Expression.Accept(this);
            RecordDataType(cast.DataType, cast);
            return cast.DataType;
        }

        public DataType VisitConditionalExpression(ConditionalExpression cond)
        {
            cond.ThenExp.Accept(this);
            cond.FalseExp.Accept(this);
            cond.Condition.Accept(this);
            return RecordDataType(cond.DataType, cond);
        }

        public DataType VisitConditionOf(ConditionOf cof)
        {
            cof.Expression.Accept(this);
            RecordDataType(cof.DataType, cof);
            return cof.DataType;
        }

        public DataType VisitConstant(Constant c)
        {
            var dt = ExistingGlobalField(c) ?? c.DataType;
            return RecordDataType(dt, c);
        }

        private DataType ExistingGlobalField(Constant c)
        {
            if (!(c.DataType is PrimitiveType pt) || (pt.Domain & Domain.Pointer) == 0)
                return null;
            var global = factory.CreatePointer(globalFields, pt.BitSize);
            return GetPossibleFieldType(global, PrimitiveType.Int32, c);
        }

        public DataType VisitDepositBits(DepositBits d)
        {
            var dtSource = d.Source.Accept(this);
            d.InsertedBits.Accept(this);
            return EnsureDataType(dtSource, d);
        }

        public DataType VisitDereference(Dereference deref)
        {
            //$TODO: if deref.Expression is of pointer type, this
            // should be the pointeee.
            return deref.DataType;
        }

        public DataType VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public DataType VisitIdentifier(Identifier id)
        {
            return EnsureDataType(id.DataType, id);
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
            var ptEa = GetPossibleFieldType(dtEa, 0);
            if (ptEa == null)
            {
                ptEa = dtEa.ResolveAs<Pointer>();
            }
            DataType dt;
            if (ptEa != null)
            {
                //$REVIEW: what if sizeof(access) != sizeof(field_at_0)?
                dt = ptEa.Pointee;
            }
            else
                dt = access.DataType;
            return RecordDataType(dt, access);
        }

        public DataType VisitMkSequence(MkSequence seq)
        {
            var dtElems = seq.Expressions.Select(e => e.Accept(this)).ToArray();
            DataType dtSeq;
            if (dtElems.Length == 2 && IsSelector(dtElems[0]))
            {
                dtSeq = PrimitiveType.Create(Domain.Pointer, seq.DataType.BitSize);
            }
            else 
            {
                if (dtElems[0] is PrimitiveType ptHead && ptHead.IsIntegral)
                {
                    dtSeq = PrimitiveType.Create(ptHead.Domain, seq.DataType.BitSize);
                }
                else
                {
                    dtSeq = seq.DataType;
                }
            }
            return RecordDataType(dtSeq, seq);
        }

        private bool IsSelector(DataType dt)
        {
            return dt is PrimitiveType pt && pt.Domain == Domain.Selector;
        }

        private bool IsIntegral(DataType dt)
        {
            return dt is PrimitiveType pt && pt.IsIntegral;
        }

        public DataType VisitOutArgument(OutArgument outArgument)
        {
            var dt = outArgument.Expression.Accept(this);
            return dt;
            //Expression exp = outArgument;
            //return RecordDataType(OutPointerTo(outArgument.TypeVariable), exp);
        }

        private DataType OutPointerTo(TypeVariable tv)
        {
            return new Pointer(tv, platform.FramePointerType.BitSize);
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
            return RecordDataType(slice.DataType, slice);
        }

        public DataType VisitTestCondition(TestCondition tc)
        {
            tc.Expression.Accept(this);
            return RecordDataType(PrimitiveType.Bool, tc);
        }

        public DataType VisitUnaryExpression(UnaryExpression unary)
        {
            var dt = unary.Expression.Accept(this);
            if (unary.Operator == Operator.AddrOf)
            {
                dt = factory.CreatePointer(dt, unary.DataType.BitSize);
            }
            return RecordDataType(dt, unary);
        }
    }
}
