#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Types;
using System;

namespace Reko.Typing
{
	/// <summary>
	/// Gives names to complex types.
	/// </summary>
	public class ComplexTypeNamer : IDataTypeVisitor<DataType>
	{
		private EquivalenceClass? eq;

        /// <summary>
        /// Renames all types in the given type store.
        /// </summary>
        /// <param name="store">Type store to use.</param>
		public void RenameAllTypes(TypeStore store)
		{
			foreach (EquivalenceClass e in store.UsedEquivalenceClasses)
			{
				eq = e;
				if (eq.DataType is not null)
					eq.DataType.Accept(this);
			}

			eq = null;
			foreach (TypeVariable tv in store.TypeVariables)
			{
				tv.DataType.Accept(this);
			}
		}

        /// <inheritdoc/>
        public DataType VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public DataType VisitStructure(StructureType str)
		{
			if (str.Name is null && eq is not null)
				str.Name = eq.Name;
            return str;
		}

        /// <inheritdoc/>
		public DataType VisitUnion(UnionType ut)
		{
			if (ut.Name is null && eq is not null)
				ut.Name = eq.Name;
            return ut;
		}

        /// <inheritdoc/>
        public DataType VisitArray(ArrayType at)
        {
            at.ElementType = at.ElementType.Accept(this);
            return at;
        }

        /// <inheritdoc/>
        public DataType VisitCode(CodeType c)
        {
            return c;
        }

        /// <inheritdoc/>
        public DataType VisitEnum(EnumType e)
        {
            return e;
        }

        /// <inheritdoc/>
        public DataType VisitEquivalenceClass(EquivalenceClass eq)
        {
            return eq;
        }

        /// <inheritdoc/>
        public DataType VisitFunctionType(FunctionType ft)
        {
            foreach (var param in ft.Parameters!)
            {
                param.DataType.Accept(this);
            }
            if (!ft.HasVoidReturn)
                ft.Outputs[0].DataType.Accept(this);
            return ft;
        }

        /// <inheritdoc/>
        public DataType VisitPrimitive(PrimitiveType pt)
        {
            return pt;
        }

        /// <inheritdoc/>
        public DataType VisitMemberPointer(MemberPointer memptr)
        {
            memptr.BasePointer = memptr.BasePointer.Accept(this);
            memptr.Pointee = memptr.Pointee.Accept(this);
            return memptr;
        }

        /// <inheritdoc/>
        public DataType VisitPointer(Pointer ptr)
        {
            ptr.Pointee = ptr.Pointee.Accept(this);
            return ptr;
        }

        /// <inheritdoc/>
        public DataType VisitReference(ReferenceTo refTo)
        {
            refTo.Referent = refTo.Referent.Accept(this);
            return refTo;
        }

        /// <inheritdoc/>
        public DataType VisitString(StringType str)
        {
            return str;
        }

        /// <inheritdoc/>
        public DataType VisitTypeReference(TypeReference typeref)
        {
            return typeref;
        }

        /// <inheritdoc/>
        public DataType VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public DataType VisitUnknownType(UnknownType ut)
        {
            return ut;
        }

        /// <inheritdoc/>
        public DataType VisitVoidType(VoidType vt)
        {
            return vt;
        }
    }
}
