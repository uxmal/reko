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
	/// Visitor methods for types.
	/// </summary>
	public interface IDataTypeVisitor
	{
		void VisitArray(ArrayType at);
		void VisitEquivalenceClass(EquivalenceClass eq);
		void VisitFunctionType(FunctionType ft);
		void VisitPrimitive(PrimitiveType pt);
		void VisitMemberPointer(MemberPointer memptr);
		void VisitPointer(Pointer ptr);
		void VisitStructure(StructureType str);
		void VisitTypeVar(TypeVariable tv);
		void VisitUnion(UnionType ut);
		void VisitUnknownType(UnknownType ut);
	}

	/// <summary>
	/// Implements the "Visitor" pattern on types.
	/// </summary>
	public class DataTypeVisitor : IDataTypeVisitor
	{
		public virtual void VisitArray(ArrayType at)
		{
			at.ElementType.Accept(this);
		}

		public virtual void VisitEquivalenceClass(EquivalenceClass eq)
		{
		}

		public virtual void VisitFunctionType(FunctionType ft)
		{
			if (ft.ReturnType != null)
				ft.ReturnType.Accept(this);
			foreach (DataType dt in ft.ArgumentTypes)
			{
				dt.Accept(this);
			}
		}

		public virtual void VisitPrimitive(PrimitiveType pt)
		{
		}

		public virtual void VisitMemberPointer(MemberPointer memptr)
		{
			memptr.BasePointer.Accept(this);
			memptr.Pointee.Accept(this);
		}

		public virtual void VisitPointer(Pointer ptr)
		{
			ptr.Pointee.Accept(this);
		}

		public virtual void VisitStructure(StructureType str)
		{
			foreach (StructureField f in str.Fields)
			{
				f.DataType.Accept(this);
			}
		}

		public virtual void VisitTypeVar(TypeVariable tv)
		{
		}

		public virtual void VisitUnion(UnionType ut)
		{
			throw new NotImplementedException();
		}

		public virtual void VisitUnknownType(UnknownType ut)
		{
			throw new NotImplementedException();
		}
	}
}
