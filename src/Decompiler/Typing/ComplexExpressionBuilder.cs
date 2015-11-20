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

using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;

using System;
using System.Diagnostics;

namespace Reko.Typing
{
	/// <summary>
	/// Given an expression with a complex type, rebuilds it to accomodate the 
	/// complex data type.
	/// </summary>
    /// <remarks>
    /// Complex expressions are assumed to take the form of a + b, where a is of complex type
    /// (such as pointer, structure, array etc) and b is "simple", such as an constant of integer type
    /// or an expression of integer type. Expressions where both a and b are complex make no sense:
    /// what do you get if you add two pointers? Also, expressions where both a and b are simple 
    /// should never reach this class, as such expressions are by definition simple also.
    /// </remarks>
	public class ComplexExpressionBuilder : IDataTypeVisitor<Expression>
	{
        private DataType dtResult;          // The data type of the resulting expression (field type).
		private DataType dt;                // Data type of the complex expression
		private DataType dtOriginal;
		private Expression basePointer;     // (possibly null) 'base' in Segmented address.
		private Expression complexExp;      // The "root" expression we will wrap with 
        private Expression indexExp;        // if not null, an index expression to use for arrays.
		private int offset;
		private bool dereferenced;
		private DataType enclosingPtr;
        private DataTypeComparer comp;
        private bool wasDereferenced;

        public ComplexExpressionBuilder(
            DataType dtResult, 
            DataType dt, 
            DataType dtOrig,
            Expression basePointer,
            Expression complexExp, 
            Expression indexExp,
            int offset)
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

		public Expression BuildComplex(bool dereferenced)
		{
            this.enclosingPtr = null;
            this.dereferenced = dereferenced;
            this.wasDereferenced = false;
            var exp = dt.Accept(this);
            if (!dereferenced && wasDereferenced)
            {
                exp = new UnaryExpression(Operator.AddrOf, dt, exp);
            }
            if (dereferenced && !wasDereferenced)
            {
                exp = new Dereference(dt, exp);
            }
            return exp;
		}

		private Expression CreateDereference(DataType dt, Expression e)
		{
            this.wasDereferenced = true;
			if (basePointer != null)
				return new MemberPointerSelector(dt, new Dereference(dt, basePointer), e);
			else if (e != null)
				return new Dereference(dt, e);
			else
				return new ScopeResolution(dt);
		}

        private Expression CreateUnreferenced(DataType dt, Expression e)
        {
            if (basePointer != null)
            {
                var mps = new MemberPointerSelector(dt, new Dereference(dt, basePointer), e);
                if (dt is ArrayType)
                {
                    return mps;
                }
                return new UnaryExpression(
                    Operator.AddrOf,
                    new Pointer(dt, 4),         //$BUG: hardwired '4'.
                    mps);
            }
            else if (e != null)
            {
                return e;
            }
            else
                throw new NotImplementedException();
        }

        private Expression CreateFieldAccess(DataType dtStructure, DataType dtField, Expression exp, Field field)
        {
            if (exp != null)
            {
                if (basePointer != null)
                {
                    exp = new MemberPointerSelector(dtField, basePointer, exp);
                }
                if (enclosingPtr != null)
                {
                    exp = CreateDereference(dtStructure, exp);
                }
                exp = new FieldAccess(dtField, exp, field); 
                return exp;
            }
            else
            {
                var scope = new ScopeResolution(dtStructure);
                if (dereferenced)
                {
                    exp = CreateDereference(dtStructure, exp);
                    dereferenced = false;
                }
                return new FieldAccess(dtField, scope, field);
            }
        }

		private Expression RewritePointer(DataType dtPtr, DataType dtPointee, DataType dtPointeeOriginal)
		{
			if (enclosingPtr != null)
			{
				return complexExp;
			}

			enclosingPtr = dtPtr;
            Expression result;
            // Drill down.
            dt = dtPointee;
            dtOriginal = dtPointeeOriginal;
            result = dtPointee.Accept(this);
            return result;
        }


        /*
            if (dtPointee is Pointer || dtPointee is MemberPointer ||
                dtPointee is CodeType ||
                comp.Compare(dtPtr, dtResult) == 0)
            {
                if (dtPointee.Size == 0)
                    Debug.Print("WARNING: {0} has size 0, which should be impossible", dtPointee);
                if (offset == 0 || dtPointee is ArrayType || dtPointee.Size > 0 && offset % dtPointee.Size == 0)
                {
                    int idx = (offset == 0 || dtPointee is ArrayType)
                        ? 0
                        : offset / dtPointee.Size;
                    if (idx == 0 && this.indexExp == null)
                    {
                        if (Dereferenced)
                            result = CreateDereference(dtPointee, complexExp);
                        else
                            result = CreateUnreferenced(dtPointee, complexExp);
                    }
                    else
                    {
                        result = CreateArrayAccess(dtPointee, dtPtr, idx, indexExp, Dereferenced);
                    }
                }
                else
                {
                    result = new PointerAddition(dtPtr, complexExp, offset);
                }
            }
            else
            {
            }
			enclosingPtr = null;    //$BUG: should be previous.
            return result;
		}
        */

        private Expression CreateArrayAccess(DataType dtPointee, DataType dtPointer, int offset, Expression arrayIndex, bool dereferenced)
        {
            if (offset == 0 && !dereferenced)
                return complexExp;
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
            Constant cOffset = Constant.Int32(offset); //$REVIEW: forcing 32-bit ints
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
			complexExp = CreateArrayAccess(dt, array, i, indexExp, dereferenced);
            dereferenced = true;
			offset = r;
			return dt.Accept(this);
		}

		public Expression VisitFunctionType(FunctionType ft)
		{
			throw new NotImplementedException();
		}

        public Expression VisitCode(CodeType ct)
        {
            return complexExp;
        }

		public Expression VisitPrimitive(PrimitiveType pt)
		{
            if (enclosingPtr == null)
            {
                return complexExp;
            }
            if (offset == 0)
                return CreateUnreferenced(pt, complexExp);

            if (pt.Size == 0)
                Debug.Print("WARNING: {0} has size 0, which should be impossible", pt);
            if (offset == 0 || pt.Size > 0 && offset % pt.Size == 0)
            {
                int idx = (offset == 0)
                    ? 0
                    : offset / pt.Size;
                if (idx == 0 && this.indexExp == null)
                {
                    if (Dereferenced)
                        return CreateDereference(pt, complexExp);
                    else
                        return CreateUnreferenced(pt, complexExp);
                }
                else
                {
                    return CreateArrayAccess(pt, enclosingPtr, idx, indexExp, Dereferenced);
                }
            }
            else
            {
                return new PointerAddition(enclosingPtr, complexExp, offset);
            }

        }

        public Expression VisitEnum(EnumType e)
        {
            return complexExp;
        }

        public Expression VisitEquivalenceClass(EquivalenceClass eq)
        {
            dt = eq.DataType;
            return dt.Accept(this);
        }

		public Expression VisitPointer(Pointer ptr)
		{
            DataType dtPointeeOrig = ptr.Pointee;
            if (complexExp.TypeVariable != null)
            {
                var ptOrig = complexExp.TypeVariable.OriginalDataType.ResolveAs<Pointer>();
                if (ptOrig != null)
                    dtPointeeOrig = ptOrig.Pointee;
            }
			return RewritePointer(ptr, ptr.Pointee, dtPointeeOrig);
		}

		public Expression VisitMemberPointer(MemberPointer memptr)
		{
            //if (!(dtOriginal is MemberPointer))
            //    throw new TypeInferenceException("MemberPointer expression {0}  was expected to have MemberPointer as its " +
            //        "original type, but was {1}.", memptr, dtOriginal);
			return RewritePointer(memptr, memptr.Pointee,  dtOriginal.ResolveAs<MemberPointer>().Pointee);
		}

        public Expression VisitString(StringType str)
        {
            throw new NotImplementedException();
        }

		public Expression VisitStructure(StructureType str)
		{
			StructureField field = str.Fields.LowerBound(this.offset);
			if (field == null)
				throw new TypeInferenceException("Expected structure type {0} to have a field at offset {1} ({1:X}).", str.Name, offset);
		
			dt = field.DataType;
			dtOriginal = field.DataType;
			complexExp = CreateFieldAccess(str, field.DataType, complexExp, field);
			offset -= field.Offset;
			return dt.Accept(this);
		}

        public Expression VisitTypeReference(TypeReference typeref)
        {
            return typeref.Referent.Accept(this);
        }

        public Expression VisitTypeVariable(TypeVariable tv)
        {
            throw new InvalidOperationException("TypeVariables should have been eliminated by now.");
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
				complexExp = CreateFieldAccess(ut, alt.DataType, complexExp, alt);
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
