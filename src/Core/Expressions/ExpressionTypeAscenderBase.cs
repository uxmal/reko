#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
        private readonly IPlatform platform;
        private readonly TypeFactory factory;
        private readonly StructureType globalFields;

        /// <summary>
        /// Initializes an instance of the <see cref="ExpressionTypeAscenderBase"/> class.
        /// </summary>
        /// <param name="program">Program being analyzed.</param>
        /// <param name="factory">Type factory to use when constructing types.</param>
        public ExpressionTypeAscenderBase(IReadOnlyProgram program, TypeFactory factory)
        {
            this.platform = program.Platform;
            this.globalFields = program.GlobalFields;
            this.factory = factory;
        }

        /// <summary>
        /// Records the data type of an expression.
        /// </summary>
        /// <param name="dt">Data type inference.</param>
        /// <param name="exp">Expression being the target of the inference.</param>
        /// <returns></returns>
        protected abstract DataType RecordDataType(DataType dt, Expression exp);

        /// <summary>
        /// Ensures the type variable for the expression has a data type.
        /// </summary>
        /// <param name="dt">Data type inference.</param>
        /// <param name="exp">Expression being the target of the inference.</param>
        /// <returns></returns>
        protected abstract DataType EnsureDataType(DataType dt, Expression exp);

        /// <inheritdoc />
        public DataType VisitAddress(Address addr)
        {
            var c = addr.ToConstant();
            c.DataType = PrimitiveType.Create(Domain.Pointer, addr.DataType.BitSize);
            var dt = ExistingGlobalField(c) ?? addr.DataType;
            return RecordDataType(dt, addr);
        }

        /// <inheritdoc />
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
            if (pfn is ProcedureConstant pc)
            {
                var sig = pc.Signature;
                if (sig.ParametersValid)
                {
                    dt = RecordDataType(sig.ReturnValue!.DataType, appl);
                }
            }
            return dt;
        }
        /// <inheritdoc />
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

        /// <inheritdoc />
        public DataType VisitBinaryExpression(BinaryExpression binExp)
        {
            DataType dtLeft = binExp.Left.Accept(this);
            DataType dtRight = binExp.Right.Accept(this);
            DataType dt;
            switch (binExp.Operator.Type)
            {
            case OperatorType.IAdd:
                var dtField = GetPossibleFieldType(dtLeft, dtRight, binExp.Right);
                if (dtField is not null)
                {
                    dt = dtField;
                }
                else
                {
                    dtField = GetPossibleFieldType(dtRight, dtLeft, binExp.Left);
                    if (dtField is not null)
                    {
                        dt = dtField;
                    }
                    else
                    {
                        dt = PullSumDataType(dtLeft, dtRight);
                    }
                }
                break;
            case OperatorType.ISub:
                dt = PullDiffDataType(dtLeft, dtRight, Domain.SignedInt);
                break;
            case OperatorType.USub:
                dt = PullDiffDataType(dtLeft, dtRight, Domain.UnsignedInt);
                break;
            case OperatorType.And:
            case OperatorType.Or:
                dt = PrimitiveType.CreateWord(binExp.DataType.BitSize).MaskDomain(Domain.Boolean | Domain.Integer | Domain.Character);
                break;
            case OperatorType.IMul:
            case OperatorType.Shl:
            case OperatorType.IMod:
                dt = PrimitiveType.CreateWord(binExp.DataType.BitSize).MaskDomain(Domain.Integer);
                break;
            case OperatorType.SMul:
            case OperatorType.SDiv:
            case OperatorType.SMod:
                dt = PrimitiveType.CreateWord(binExp.DataType.BitSize).MaskDomain(Domain.SignedInt);
                break;
            case OperatorType.UMul:
            case OperatorType.UDiv:
            case OperatorType.UMod:
                dt = PrimitiveType.CreateWord(binExp.DataType.BitSize).MaskDomain(Domain.UnsignedInt);
                break;
            case OperatorType.Eq:
            case OperatorType.Ne:
            case OperatorType.Lt:
            case OperatorType.Le:
            case OperatorType.Ge:
            case OperatorType.Gt:
            case OperatorType.Ult:
            case OperatorType.Ule:
            case OperatorType.Uge:
            case OperatorType.Ugt:
            case OperatorType.Feq:
            case OperatorType.Fne:
            case OperatorType.Flt:
            case OperatorType.Fle:
            case OperatorType.Fge:
            case OperatorType.Fgt:
            case OperatorType.Cand:
            case OperatorType.Cor:
                dt = PrimitiveType.Bool;
                break;
            case OperatorType.FAdd:
            case OperatorType.FSub:
            case OperatorType.FMul:
            case OperatorType.FDiv:
            case OperatorType.FMod:
                dt = PrimitiveType.Create(Domain.Real, binExp.DataType.BitSize);
                break;
            case OperatorType.Shr:
                dt = PrimitiveType.Create(Domain.UnsignedInt, dtLeft.BitSize);
                break;
            case OperatorType.Sar:
                dt = PrimitiveType.Create(Domain.SignedInt, dtLeft.BitSize);
                break;
            case OperatorType.Xor:
                dt = PrimitiveType.Create(Domain.Integer, dtLeft.BitSize);
                break;
            default:
                throw NYI(binExp);
            }
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
        private Pointer? GetPossibleFieldType(DataType dtLeft, DataType dtRight, Expression right)
        {
            if (right is BigConstant)
                return null;
            if (right is Constant cOffset)
            {
                if (dtRight.Domain != Domain.Pointer)
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
        private Pointer? GetPossibleFieldType(DataType dtLeft, int offset)
        {
            var ptrLeft = dtLeft.ResolveAs<Pointer>();
            if (ptrLeft is null)
                return null;

            var pointee = ptrLeft.Pointee;
            var strPointee = pointee.ResolveAs<StructureType>();
            if (strPointee is null)
                return null;

            var field = strPointee.Fields.LowerBound(offset);
            if (field is null)
                return null;
            // We're collecting _DataTypes_, so if we encounter
            // a TypeReference, we need to drill past it.
            var dtField = field.DataType.ResolveAs<DataType>();
            if (dtField is null)
                return null;
            // If we access beyond the start of the field, we can't have the
            // same type as the field.
            if (offset != field.Offset)
            {
                // Check if field is nested structure
                var ptrField = factory.CreatePointer(dtField, ptrLeft.BitSize);
                return GetPossibleFieldType(ptrField, offset - field.Offset);
            }
            return factory.CreatePointer(dtField, dtLeft.BitSize);
        }

        private DataType PullSumDataType(DataType dtLeft, DataType dtRight)
        {
            var ptLeft = dtLeft.ResolveAs<PrimitiveType>();
            var ptRight = dtRight.ResolveAs<PrimitiveType>();
            if (ptLeft is not null && ptLeft.Domain == Domain.Pointer)
            {
                if (ptRight is not null && ptRight.Domain != Domain.Pointer)
                    return PrimitiveType.Create(Domain.Pointer, dtLeft.BitSize);
            }
            if (ptLeft is not null && ptLeft.IsIntegral && ptRight is not null && ptRight.IsIntegral)
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
            if (dtLeft.ResolveAs<Pointer>() is not null)
            {
                if (dtLeft is TypeReference)
                    return dtLeft;
                else 
                    return PrimitiveType.Create(Domain.Pointer, dtLeft.BitSize);
            }
            return dtLeft;
        }

        private DataType PullDiffDataType(DataType dtLeft, DataType dtRight, Domain sign)
        {
            var ptRight = dtRight.ResolveAs<PrimitiveType>();
            if (dtLeft.Domain == Domain.Pointer)
            {
                if (ptRight is not null)
                {
                    if ((ptRight.Domain & Domain.Integer) != 0)
                        return PrimitiveType.Create(Domain.Pointer, dtLeft.BitSize);
                    else if ((ptRight.Domain & Domain.Pointer) != 0)
                        return PrimitiveType.Create(sign, dtLeft.BitSize);
                }
                if (dtRight is Pointer)
                    return PrimitiveType.Create(sign, dtLeft.BitSize);
                // We are unable to reconcile the differences here. 
                return PrimitiveType.CreateWord(dtLeft.BitSize);
                //$TODO: should be a warning? throw new TypeInferenceException(string.Format("Pulling difference {0} and {1}", dtLeft, dtRight));
            }
            if (ptRight is not null && ptRight.Domain == Domain.Pointer || 
                dtRight is Pointer)
            {
                if (ptRight is not null && (ptRight.Domain & Domain.Integer) != 0)
                    return dtLeft;
                // If a dtRight is a pointer and it's being subtracted from 
                // something, then the result has to be a ptrdiff_t, i.e.
                // integer.
                if ((dtLeft.Domain & Domain.Pointer) != 0)
                    return PrimitiveType.Create(Domain.Integer, dtLeft.BitSize);
                // We are unable to reconcile the differences here. 
                return PrimitiveType.CreateWord(dtLeft.BitSize);
                //$TODO: should be a warning? throw new TypeInferenceException(string.Format("Pulling difference {0} and {1}", dtLeft, dtRight));
            }
            return dtLeft;
        }

        /// <inheritdoc />
        public DataType VisitCast(Cast cast)
        {
            cast.Expression.Accept(this);
            RecordDataType(cast.DataType, cast);
            return cast.DataType;
        }

        /// <inheritdoc />
        public DataType VisitConditionalExpression(ConditionalExpression cond)
        {
            cond.ThenExp.Accept(this);
            cond.FalseExp.Accept(this);
            cond.Condition.Accept(this);
            return RecordDataType(cond.DataType, cond);
        }

        /// <inheritdoc />
        public DataType VisitConditionOf(ConditionOf cof)
        {
            cof.Expression.Accept(this);
            RecordDataType(cof.DataType, cof);
            return cof.DataType;
        }

        /// <inheritdoc />
        public DataType VisitConstant(Constant c)
        {
            var dt = ExistingGlobalField(c) ?? c.DataType;
            return RecordDataType(dt, c);
        }

        private DataType? ExistingGlobalField(Expression c)
        {
            if ((c.DataType.Domain & Domain.Pointer) == 0)
                return null;
            var global = factory.CreatePointer(globalFields, c.DataType.BitSize);
            return GetPossibleFieldType(global, PrimitiveType.Int32, c);
        }

        /// <inheritdoc />
        public DataType VisitConversion(Conversion conversion)
        {
            conversion.Expression.Accept(this);
            RecordDataType(conversion.DataType, conversion);
            return conversion.DataType;
        }

        /// <inheritdoc />
        public DataType VisitDereference(Dereference deref)
        {
            //$TODO: if deref.Expression is of pointer type, this
            // should be the pointeee.
            return deref.DataType;
        }

        /// <inheritdoc />
        public DataType VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public DataType VisitIdentifier(Identifier id)
        {
            return EnsureDataType(id.DataType, id);
        }

        /// <inheritdoc />
        public DataType VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public DataType VisitMemoryAccess(MemoryAccess access)
        {
            var dtEa = access.EffectiveAddress.Accept(this);
            var ptEa = GetPossibleFieldType(dtEa, 0);
            if (ptEa is null)
            {
                ptEa = dtEa.ResolveAs<Pointer>();
            }
            DataType dt;
            if (ptEa is not null)
            {
                //$REVIEW: what if sizeof(access) != sizeof(field_at_0)?
                dt = ptEa.Pointee;
            }
            else
                dt = access.DataType;
            return RecordDataType(dt, access);
        }

        /// <inheritdoc />
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
                if (dtElems[0].IsIntegral)
                {
                    dtSeq = PrimitiveType.Create(dtElems[0].Domain, seq.DataType.BitSize);
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
            return dt.Domain == Domain.Selector;
        }

        /// <inheritdoc />
        public DataType VisitOutArgument(OutArgument outArgument)
        {
            var dt = outArgument.Expression.Accept(this);
            return dt;
            //Expression exp = outArgument;
            //return RecordDataType(OutPointerTo(outArgument.TypeVariable), exp);
        }

        /// <inheritdoc />
        public DataType VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public DataType VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public DataType VisitProcedureConstant(ProcedureConstant pc)
        {
            return pc.DataType;
        }

        /// <inheritdoc />
        public DataType VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public DataType VisitSegmentedAddress(SegmentedPointer address)
        {
            var dtBase = address.BasePointer.Accept(this);
            var dtOffset =  address.Offset.Accept(this);
            var segptrType = PrimitiveType.Create(Domain.SegPointer, address.DataType.BitSize);
            return RecordDataType(segptrType, address);
        }

        /// <inheritdoc />
        public DataType VisitSlice(Slice slice)
        {
            slice.Expression.Accept(this);
            return RecordDataType(slice.DataType, slice);
        }

        /// <inheritdoc />
        public DataType VisitStringConstant(StringConstant str)
        {
            return RecordDataType(str.DataType, str);
        }

        /// <inheritdoc />
        public DataType VisitTestCondition(TestCondition tc)
        {
            tc.Expression.Accept(this);
            return RecordDataType(PrimitiveType.Bool, tc);
        }

        /// <inheritdoc />
        public DataType VisitUnaryExpression(UnaryExpression unary)
        {
            var dt = unary.Expression.Accept(this);
            if (unary.Operator.Type == OperatorType.AddrOf)
            {
                dt = factory.CreatePointer(dt, unary.DataType.BitSize);
            }
            return RecordDataType(dt, unary);
        }
    }
}
