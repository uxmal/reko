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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Gives names to complex types.
	/// </summary>
	public class ComplexTypeNamer : IDataTypeVisitor<DataType>
	{
		private EquivalenceClass eq;

		public ComplexTypeNamer()
		{
		}

		public void RenameAllTypes(TypeStore store)
		{
			foreach (EquivalenceClass e in store.UsedEquivalenceClasses)
			{
				eq = e;
				if (eq.DataType != null)
					eq.DataType.Accept(this);
			}

			eq = null;
			foreach (TypeVariable tv in store.TypeVariables)
			{
				tv.DataType.Accept(this);
			}
		}

		public DataType VisitStructure(StructureType str)
		{
			if (str.Name == null && eq != null)
				str.Name = eq.Name;
            return str;
		}

		public DataType VisitUnion(UnionType ut)
		{
			if (ut.Name == null && eq != null)
				ut.Name = eq.Name;
			int i = 0; 
			foreach (UnionAlternative a in ut.Alternatives.Values)
			{
				a.Name = a.MakeName(i++);
			}
            return ut;
		}

        public DataType VisitArray(ArrayType at)
        {
            at.ElementType = at.ElementType.Accept(this);
            return at;
        }

        public DataType VisitCode(CodeType c)
        {
            return c;
        }

        public DataType VisitEnum(EnumType e)
        {
            return e;
        }

        public DataType VisitEquivalenceClass(EquivalenceClass eq)
        {
            return eq;
        }

        public DataType VisitFunctionType(FunctionType ft)
        {
            foreach (var arg in ft.ArgumentTypes)
            {
                arg.Accept(this);
            }
            if (ft.ReturnType != null)
                ft.ReturnType.Accept(this);
            return ft;
        }

        public DataType VisitPrimitive(PrimitiveType pt)
        {
            return pt;
        }

        public DataType VisitMemberPointer(MemberPointer memptr)
        {
            memptr.BasePointer = memptr.BasePointer.Accept(this);
            memptr.Pointee = memptr.Pointee.Accept(this);
            return memptr;
        }

        public DataType VisitPointer(Pointer ptr)
        {
            ptr.Pointee = ptr.Pointee.Accept(this);
            return ptr;
        }

        public DataType VisitString(StringType str)
        {
            return str;
        }

        public DataType VisitTypeReference(TypeReference typeref)
        {
            return typeref;
        }

        public DataType VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public DataType VisitUnknownType(UnknownType ut)
        {
            return ut;
        }

        public DataType VisitVoidType(VoidType vt)
        {
            return vt;
        }
    }
}
