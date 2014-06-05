#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Core.Operators;
using Decompiler.Core.Types;

using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Given an expression with a complex type, rebuilds it to accomodate the 
	/// complex data type.
	/// </summary>
    /// <remarks>
    /// Complex expressions are assumed to take the form of a + b, where a is of complex type
    /// (such as pointer, structure, array etc) and b is "simple", such as an constant of integer type
    /// or an expression of integer type. Expressions where both a and b are complex make no sense:
    /// what do you get if you add two pointers? Also, expressions where both a and b are simple should never reach this
    /// class, as such expressions are by definition simple also.
    /// </remarks>
	public class ComplexExpressionBuilder : IDataTypeVisitor<Expression>
	{
        private DataType dtResult;
		private DataType dt;
		private DataType dtOriginal;
		private Expression basePointer;
		private Expression complexExp;
        private Expression indexExp;
		private int offset;
        //private Expression result;
		private bool dereferenced;
		private bool seenPtr;
        private DataTypeComparer comp; 
        
        public ComplexExpressionBuilder(DataType dtResult, DataType dt, DataType dtOrig, Expression basePointer, Expression complexExp, Expression indexExp, int offset)
        {
            this.dtResult = dtResult;
            this.dt = dt;
            this.dtOriginal = dtOrig;
            this.basePointer = basePointer;
            this.complexExp = complexExp;
            this.indexExp = indexExp;
            this.offset = offset;
            this.comp = new DataTypeComparer();
        }

		public Expression BuildComplex()
		{
			return dt.Accept(this);
		}

		private Expression CreateDereference(DataType dt, Expression e)
		{
			if (basePointer != null)
				return new MemberPointerSelector(dt, new Dereference(dt, basePointer), e);
			else if (e != null)
				return new Dereference(dt, e);
			else
				return new ScopeResolution(dt, dt.Name);
		}

        private Expression CreateFieldAccess(DataType dtStructure, DataType dtField, Expression exp, string fieldName)
        {
            if (exp != null)
            {
                if (basePointer != null)
                {
                    exp = new MemberPointerSelector(dtField, basePointer, exp);
                }
                return new FieldAccess(dtField, exp, fieldName);
            }
            else
            {
                return new ScopeResolution(dtStructure, dtStructure.Name + "::" + fieldName);
            }
        }

		private Expression RewritePointer(DataType dtPtr, DataType dtPointee, DataType dtPointeeOriginal)
		{
			if (seenPtr)
			{
				return complexExp;
			}

			seenPtr = true;
            Expression result;
			if (dtPointee is PrimitiveType || dtPointee is Pointer || dtPointee is MemberPointer ||
                comp.Compare(dtPtr, dtResult) == 0)
			{
                if (offset == 0 || offset % dtPointee.Size == 0)
                {
                    int idx = offset == 0
                        ? 0
                        : offset / dtPointee.Size;
                    if (idx == 0)
                    {
                        if (Dereferenced)
                            result = CreateDereference(dtPointee, complexExp);
                        else
                            result = complexExp;
                    }
                    else
                    {
                        result = CreateArrayAccess(dtPointee, dtPtr, idx, null, Dereferenced);
                    }
                }
                else
                {
                    result = new PointerAddition(dtPtr, complexExp, offset);
                }
			}
			else
			{
				dtOriginal = dtPointeeOriginal;
				complexExp = CreateDereference(dtPointee, complexExp);
				bool deref = Dereferenced;
				Dereferenced = false;
				basePointer = null;
				result = dtPointee.Accept(this);
				if (!deref)
				{
					result = new UnaryExpression(UnaryOperator.AddrOf, dtPtr, result);
				}
				Dereferenced = deref; 
			}
			seenPtr = false;
            return result;
		}


        private Expression CreateArrayAccess(DataType dtPointee, DataType dtPointer, int offset, Expression arrayIndex, bool dereferenced)
        {
            arrayIndex = CreateArrayIndexExpression(offset, arrayIndex);
            if (dereferenced)
            {
                return new ArrayAccess(dtPointee, complexExp, arrayIndex);
            }
            else
            {
                return new BinaryExpression(Operator.IAdd, dtPointer, complexExp, arrayIndex);
            }
        }

        private static Expression CreateArrayIndexExpression(int offset, Expression arrayIndex)
        {
            BinaryOperator op = offset < 0 ? Operator.ISub : Operator.IAdd;
            offset = Math.Abs(offset);
            Constant cOffset = Constant.Create(PrimitiveType.Create(Domain.SignedInt, arrayIndex.DataType.Size), offset);
            if (arrayIndex != null)
            {
                if (offset != 0)
                {
                    return new BinaryExpression(op, arrayIndex.DataType, arrayIndex, cOffset);
                }
            }
            else
            {
                return cOffset;
            }
            return arrayIndex;
        }
        
        public bool Dereferenced
		{
			get { return dereferenced; } 
			set { dereferenced = value; }
		}


		public Expression VisitArray(ArrayType array)
		{
			int i = (int) (offset / array.ElementType.Size);
			int r = (int) (offset % array.ElementType.Size);
			dt = array.ElementType;
			dtOriginal = array.ElementType;
			complexExp.DataType = array;
			complexExp = CreateArrayAccess(dt, array, i, indexExp, true);
			offset = r;
			return dt.Accept(this);
		}


		public Expression VisitFunctionType(FunctionType ft)
		{
			throw new NotImplementedException();
		}

		public Expression VisitPrimitive(PrimitiveType pt)
		{
			return complexExp;
		}

		public Expression VisitEquivalenceClass(EquivalenceClass eq)
		{
			EquivalenceClass eqOriginal = dtOriginal as EquivalenceClass;
			if (eqOriginal != null && eq.Number == eqOriginal.Number)
			{
				complexExp.DataType = eq;
                return complexExp;
			}
			else
			{
				dt = eq.DataType;
				return dt.Accept(this);
			}
		}

		public Expression VisitPointer(Pointer ptr)
		{
			return RewritePointer(ptr, ptr.Pointee, ((Pointer) this.dtOriginal).Pointee);
		}

		public Expression VisitMemberPointer(MemberPointer memptr)
		{
			if (!(dtOriginal is MemberPointer))
				throw new TypeInferenceException("MemberPointer expression {0}  was expected to have MemberPointer as its " +
					"original type, but was {1}.", memptr, dtOriginal);
			return RewritePointer(memptr, memptr.Pointee, ((MemberPointer) dtOriginal).Pointee);
		}

        public Expression VisitString(StringType str)
        {
            throw new NotImplementedException();
        }

		public Expression VisitStructure(StructureType str)
		{
			StructureField field = str.Fields.LowerBound(this.offset);
			if (field == null)
				throw new TypeInferenceException("Expected structure type {0} to have a field at offset {1}.", str.Name, offset);
		
			dt = field.DataType;
			dtOriginal = field.DataType;
			complexExp = CreateFieldAccess(str, field.DataType, complexExp, field.Name);
			offset -= field.Offset;
			return dt.Accept(this);
		}

        public Expression VisitTypeReference(TypeReference typeref)
        {
            throw new NotImplementedException();
        }

        public Expression VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

		public Expression VisitUnion(UnionType ut)
		{
			UnionAlternative alt = ut.FindAlternative(dtOriginal);
			if (alt == null)
				throw new TypeInferenceException("Unable to find {0} in {1} (offset {2}).", dtOriginal, ut, offset);

			dt = alt.DataType;
			dtOriginal = alt.DataType;
			if (ut.PreferredType != null)
			{
				complexExp = new Cast(ut.PreferredType, complexExp);
			}
			else
			{
				complexExp = new FieldAccess(alt.DataType, complexExp, alt.Name);
			}
			return dt.Accept(this);
		}

        public Expression VisitUnknownType(UnknownType unk)
        {
            throw new NotImplementedException();
        }

        public Expression VisitVoidType(VoidType vt)
        {
            throw new NotImplementedException();
        }
	}
}
