/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;

using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Given an expression with a complex type, rebuilds it to accomodate the 
	/// complex data type.
	/// </summary>
	public class ComplexExpressionBuilder : DataTypeVisitor
	{
		private DataType dt;
		private DataType dtOriginal;
		private Expression basePointer;
		private Expression baseExp;
		private int offset;
		private Expression complexExp;
		private bool dereferenced;

		public ComplexExpressionBuilder(DataType dt, DataType dtOrig, Expression b, int offset) : this(dt, dtOrig, null, b, offset)
		{
		}

		public ComplexExpressionBuilder(DataType dt, DataType dtOrig, Expression basePointer, Expression b, int offset)
		{
			this.dt = dt;
			this.dtOriginal = dtOrig;
			this.basePointer = basePointer;
			this.baseExp = b;
			this.offset = offset;
		}

		public Expression BuildComplex()
		{
			complexExp = null;
			dt.Accept(this);
			return complexExp;
		}

		private Expression CreateDereference(DataType dt, Expression e)
		{
			if (basePointer != null)
				return new MemberPointerSelector(dt, new Dereference(null, basePointer), e);
			else
				return new Dereference(dt, e);
		}

		private void RewritePointer(DataType dtPtr, DataType dtPointee, DataType dtPointeeOriginal)
		{
			if (dtPointee is PrimitiveType || dtPointee is Pointer)
			{
				if (offset % dtPointee.Size == 0)
				{
					int idx = offset / dtPointee.Size;
					if (idx == 0)
					{
						if (Dereferenced)
						{
							complexExp = CreateDereference(dtPointee, baseExp);
						}
						else
							complexExp = baseExp;
					}
					else
					{
						complexExp = new ArrayAccess(dtPointee, baseExp, 
							new Constant(PrimitiveType.Word32, idx));
						if (!Dereferenced)
						{
							complexExp = new UnaryExpression(UnaryOperator.addrOf, dtPtr, complexExp);
						}
					}
				}
				else
				{
					complexExp = new PointerAddition(dtPtr, baseExp, offset);
				}
			}
			else
			{
				dtOriginal = dtPointeeOriginal;
				baseExp = CreateDereference(dtPointee, baseExp);
				basePointer = null;
				dtPointee.Accept(this);
				if (!Dereferenced)
				{
					complexExp = new UnaryExpression(UnaryOperator.addrOf, dtPtr, complexExp);
				}
			}
		}


		public bool Dereferenced
		{
			get { return dereferenced; } 
			set { dereferenced = value; }
		}


		public override void VisitArray(ArrayType array)
		{
			int i = (int) (offset / array.ElementType.Size);
			int r = (int) (offset % array.ElementType.Size);
			dt = array.ElementType;
			dtOriginal = array.ElementType;
			baseExp.DataType = array;
			baseExp = new ArrayAccess(dt, baseExp, new Constant(PrimitiveType.Int32, i));
			offset = r;
			dt.Accept(this);
		}


		public override void VisitFunctionType(FunctionType ft)
		{
			throw new NotImplementedException();
		}

		public override void VisitPrimitive(PrimitiveType pt)
		{
			complexExp = baseExp;
		}

		public override void VisitEquivalenceClass(EquivalenceClass eq)
		{
			EquivalenceClass eqOriginal = dtOriginal as EquivalenceClass;
			if (eqOriginal != null && eq.Number == eqOriginal.Number)
			{
				complexExp = baseExp;
				complexExp.DataType = eq;
			}
			else
			{
				dt = eq.DataType;
				dt.Accept(this);
			}
		}

		public override void VisitPointer(Pointer ptr)
		{
			RewritePointer(ptr, ptr.Pointee, ((Pointer) this.dtOriginal).Pointee);
		}

		public override void VisitMemberPointer(MemberPointer memptr)
		{
			if (!(dtOriginal is MemberPointer))
				throw new TypeInferenceException("MemberPointer expression {0}  was expected to have MemberPointer as its " +
					"original type, but was {1}.", memptr, dtOriginal);
			RewritePointer(memptr, memptr.Pointee, ((MemberPointer) dtOriginal).Pointee);
		}


		public override void VisitStructure(StructureType str)
		{
			StructureField field = str.Fields.LowerBound(this.offset);
			if (field == null)
				throw new TypeInferenceException("Expected structure type {0} to have a field at offset {1}.", str.Name, offset);
		
			dt = field.DataType;
			dtOriginal = field.DataType;
			baseExp = CreateFieldAccess(field.DataType, baseExp, field.Name);
			offset -= field.Offset;
			dt.Accept(this);
		}

		private Expression CreateFieldAccess(DataType dt, Expression exp, string fieldName)
		{
			if (basePointer != null)
			{
				exp = new MemberPointerSelector(dt, basePointer, exp);
			}
			return new FieldAccess(dt, exp, fieldName);
		}

		public override void VisitUnion(UnionType ut)
		{
			UnionAlternative alt = ut.FindAlternative(dtOriginal);
			if (alt == null)
				throw new TypeInferenceException("Unable to find {0} in {1} (offset {2}).", dtOriginal, ut, offset);

			dt = alt.DataType;
			dtOriginal = alt.DataType;
			if (ut.PreferredType != null)
			{
				baseExp = new Cast(ut.PreferredType, baseExp);
			}
			else
			{
				baseExp = new FieldAccess(alt.DataType, baseExp, alt.Name);
			}
			dt.Accept(this);
		}
	}
}
