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

using Reko.Core.Expressions;
using System;

namespace Reko.Core.Types
{
	/// <summary>
	/// Implements the "Visitor" pattern on types, with the intent of returning
	/// a possibly completely different data type as a return value
	/// </summary>
	public abstract class DataTypeTransformer : IDataTypeVisitor<DataType>
	{
        /// <inheritdoc/>
		public virtual DataType VisitArray(ArrayType at)
		{
			at.ElementType = at.ElementType.Accept(this);
			return at;
		}

        /// <inheritdoc/>
        public virtual DataType VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual DataType VisitCode(CodeType c)
        {
            return c;
        }

        /// <inheritdoc/>
        public virtual DataType VisitEnum(EnumType e)
        {
            return e;
        }

        /// <inheritdoc/>
		public virtual DataType VisitEquivalenceClass(EquivalenceClass eq)
		{
			return eq;
		}

        /// <inheritdoc/>
        public virtual DataType VisitFunctionType(FunctionType ft)
        {
            if (ft.ReturnValue is not null)
                ft.ReturnValue.DataType = ft.ReturnValue.DataType.Accept(this);

            Identifier[]? p = ft.Parameters;
            if (p is not null)
            { 
                for (int i = 0; i < p.Length; ++i)
                {
                    DataType dt = p[i].DataType.Accept(this);
                    p[i].DataType = dt;
                }
            }
			return ft;
		}

        /// <inheritdoc/>
        public virtual DataType VisitPrimitive(PrimitiveType pt)
		{
			return pt;
		}

        /// <inheritdoc/>
        public virtual DataType VisitStructure(StructureType str)
		{
            // Do not transform user-defined types
            if (str.UserDefined)
                return str;
			foreach (StructureField field in str.Fields)
			{
				field.DataType = field.DataType.Accept(this);
			}
			return str;
		}

        /// <inheritdoc/>
        public virtual DataType VisitMemberPointer(MemberPointer memptr)
		{
			var pointee = memptr.Pointee.Accept(this);
			var basePointer = memptr.BasePointer.Accept(this);
            return new MemberPointer(basePointer, pointee, memptr.BitSize);
		}

        /// <inheritdoc/>
        public virtual DataType VisitPointer(Pointer ptr)
		{
			ptr.Pointee = ptr.Pointee.Accept(this);
			return ptr;
		}

        /// <inheritdoc/>
        public virtual DataType VisitReference(ReferenceTo refTo)
        {
            refTo.Referent = refTo.Referent.Accept(this);
            return refTo;
        }

        /// <inheritdoc/>
        public virtual DataType VisitString(StringType str)
        {
            return str;
        }

        /// <inheritdoc/>
        public virtual DataType VisitTypeReference(TypeReference typeref)
        {
            return new TypeReference(typeref.Name, typeref.Referent.Accept(this));
        }

        /// <inheritdoc/>
        public virtual DataType VisitTypeVariable(TypeVariable tv)
		{
			return tv;
		}

        /// <inheritdoc/>
        public virtual DataType VisitUnion(UnionType ut)
		{
            // Do not transform user-defined types
            if (ut.UserDefined)
                return ut;
            foreach (UnionAlternative a in ut.Alternatives.Values)
			{
				a.DataType = a.DataType.Accept(this);
			}
			return ut;
		}

        /// <inheritdoc/>
        public virtual DataType VisitUnknownType(UnknownType unk)
		{
			return unk;
		}

        /// <inheritdoc/>
        public virtual DataType VisitVoidType(VoidType vt)
        {
            return vt;
        }
	}
}

