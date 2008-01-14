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

using System;

namespace Decompiler.Core.Types
{
	/// <summary>
	/// Implements the "Visitor" pattern on types, with the intent of returning
	/// a possibly completely different data type as a return value
	/// </summary>
	public abstract class DataTypeTransformer
	{
		public virtual DataType TransformArrayType(ArrayType at)
		{
			at.ElementType = at.ElementType.Accept(this);
			return at;
		}

		public virtual DataType TransformEquivalenceClass(EquivalenceClass eq)
		{
			return eq;
		}

		public virtual DataType TransformFunctionType(FunctionType ft)
		{
			if (ft.ReturnType != null)
				ft.ReturnType = ft.ReturnType.Accept(this);

			DataType [] p = ft.ArgumentTypes;
			for (int i = 0; i < p.Length; ++i)
			{
				DataType dt = p[i].Accept(this);
				p[i] = dt;
			}
			return ft;
		}

		public virtual DataType TransformPrimitiveType(PrimitiveType pt)
		{
			return pt;
		}

		public virtual  DataType TransformStructure(StructureType str)
		{
			foreach (StructureField field in str.Fields)
			{
				field.DataType = field.DataType.Accept(this);
			}
			return str;
		}

		public virtual DataType TransformMemberPointer(MemberPointer memptr)
		{
			memptr.Pointee = memptr.Pointee.Accept(this);
			memptr.BasePointer = memptr.BasePointer.Accept(this);
			return memptr;
		}

		public virtual DataType TransformPointer(Pointer ptr)
		{
			ptr.Pointee = ptr.Pointee.Accept(this);
			return ptr;
		}
		
		public virtual DataType TransformTypeVar(TypeVariable tv)
		{
			return tv;
		}

		public virtual DataType TransformUnionType(UnionType ut)
		{
			foreach (UnionAlternative a in ut.Alternatives)
			{
				a.DataType = a.DataType.Accept(this);
			}
			return ut;
		}

		public virtual DataType TransformUnknownType(UnknownType ut)
		{
			return ut;
		}
	}
}

