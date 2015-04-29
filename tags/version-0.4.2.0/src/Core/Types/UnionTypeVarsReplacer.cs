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

using System;
using System.Collections.Generic;

namespace Decompiler.Core.Types
{
	/// <summary>
	/// Replaces a union of TypeVars with a reference to an equivalence class.
	/// </summary>
	public class UnionTypeVarsReplacer : IDataTypeVisitor<DataType>
	{
		private TypeStore store;

		public UnionTypeVarsReplacer(TypeStore store)
		{
			this.store = store;
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
            if (ft.ReturnType != null)
                ft.ReturnType = ft.ReturnType.Accept(this);
            for (int i = 0; i < ft.ArgumentTypes.Length; ++i)
            {
                ft.ArgumentTypes[i] = ft.ArgumentTypes[i].Accept(this);
            }
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

        public DataType VisitStructure(StructureType str)
        {
            foreach (var field in str.Fields)
            {
                field.DataType = field.DataType.Accept(this);
            }
            return str;
        }

        public DataType VisitTypeReference(TypeReference tr)
        {
            return tr;
        }

        public DataType VisitTypeVariable(TypeVariable tv)
        {
            return tv;
        }

        public DataType VisitUnion(UnionType ut)
        {
            List<TypeVariable> typeVars = new List<TypeVariable>();
            foreach (UnionAlternative a in ut.Alternatives.Values)
            {
                TypeVariable tv = a.DataType as TypeVariable;
                if (tv == null)
                    return ut;
                typeVars.Add(tv);
            }

            // Merge all the type variables.

            EquivalenceClass eq = null;
            foreach (TypeVariable tv in typeVars)
            {
                if (eq == null)
                    eq = tv.Class;
                else
                    eq = store.MergeClasses(eq.Representative, tv);
            }
            eq.DataType = ut;
            return eq.Representative;
        }

        public DataType VisitUnknownType(UnknownType unk)
        {
            return unk;
        }

        public DataType VisitVoidType(VoidType v)
        {
            return v;
        }
    }
}
