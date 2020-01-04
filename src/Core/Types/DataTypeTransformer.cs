#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
		public virtual DataType VisitArray(ArrayType at)
		{
			at.ElementType = at.ElementType.Accept(this);
			return at;
		}

        public virtual DataType VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        public virtual DataType VisitCode(CodeType c)
        {
            return c;
        }

        public virtual DataType VisitEnum(EnumType e)
        {
            return e;
        }

		public virtual DataType VisitEquivalenceClass(EquivalenceClass eq)
		{
			return eq;
		}

        public virtual DataType VisitFunctionType(FunctionType ft)
        {
            if (ft.ReturnValue != null)
                ft.ReturnValue.DataType = ft.ReturnValue.DataType.Accept(this);

            Identifier[] p = ft.Parameters;
            if (p != null)
            { 
                for (int i = 0; i < p.Length; ++i)
                {
                    DataType dt = p[i].DataType.Accept(this);
                    p[i].DataType = dt;
                }
            }
			return ft;
		}

        public virtual DataType VisitPrimitive(PrimitiveType pt)
		{
			return pt;
		}

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

        public virtual DataType VisitMemberPointer(MemberPointer memptr)
		{
			var pointee = memptr.Pointee.Accept(this);
			var basePointer = memptr.BasePointer.Accept(this);
            return new MemberPointer(basePointer, pointee, memptr.Size);
		}

        public virtual DataType VisitPointer(Pointer ptr)
		{
			ptr.Pointee = ptr.Pointee.Accept(this);
			return ptr;
		}

        public virtual DataType VisitReference(ReferenceTo refTo)
        {
            refTo.Referent = refTo.Referent.Accept(this);
            return refTo;
        }

        public virtual DataType VisitString(StringType str)
        {
            return str;
        }

        public virtual DataType VisitTypeReference(TypeReference typeref)
        {
            return new TypeReference(typeref.Name, typeref.Referent.Accept(this));
        }

        public virtual DataType VisitTypeVariable(TypeVariable tv)
		{
			return tv;
		}

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

        public virtual DataType VisitUnknownType(UnknownType unk)
		{
			return unk;
		}

        public virtual DataType VisitVoidType(VoidType vt)
        {
            return vt;
        }
	}
}

