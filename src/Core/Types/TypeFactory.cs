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

using Reko.Core.Code;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;

namespace Reko.Core.Types
{
	public class TypeFactory
	{
		private int typeVars;
		private readonly Dictionary<PrimitiveType,PrimitiveType> primitives = new Dictionary<PrimitiveType,PrimitiveType>();

		public TypeFactory()
		{
			typeVars = 0;
		
			primitives[PrimitiveType.Bool] = PrimitiveType.Bool;
			primitives[PrimitiveType.Char] = PrimitiveType.Char;
			primitives[PrimitiveType.SByte] = PrimitiveType.SByte;
			primitives[PrimitiveType.Word16] = PrimitiveType.Word16;
			primitives[PrimitiveType.Int16] = PrimitiveType.Int16;
			primitives[PrimitiveType.UInt16] = PrimitiveType.UInt16;
			primitives[PrimitiveType.Word32] = PrimitiveType.Word32;
			primitives[PrimitiveType.Int32] = PrimitiveType.Int32;
			primitives[PrimitiveType.UInt32] = PrimitiveType.UInt32;
			primitives[PrimitiveType.Real32] = PrimitiveType.Real32;
			primitives[PrimitiveType.Real64] = PrimitiveType.Real64;
			primitives[PrimitiveType.Bool] = PrimitiveType.Bool;
			primitives[PrimitiveType.Byte] = PrimitiveType.Byte;
		}

		private int AllocateTypeVariable()
		{
			return ++typeVars;
		}

		public ArrayType CreateArrayType(DataType elType, int length)
		{
			return new ArrayType(elType, length);
		}

		public FunctionType CreateFunctionType(Identifier returnType, Identifier [] parameters)
		{
			return new FunctionType(returnType, parameters);
		}

		public PrimitiveType CreatePrimitiveType(Domain dom, int bitSize)
		{
			return PrimitiveType.Create(dom, bitSize);
		}

        public DataType CreateStructureType()
        {
            return new StructureType();
        }

		public StructureType CreateStructureType(string name, int size)
		{
			return new StructureType(name, size);
		}

		public StructureType CreateStructureType(string name, int size, StructureField field)
		{
            return new StructureType(name, size) { Fields = { field } };
		}

		public MemberPointer CreateMemberPointer(DataType basePointer, DataType pointee, int byteSize)
		{
			return new MemberPointer(basePointer, pointee, byteSize);
		}

		public Pointer CreatePointer(DataType pointee, int bitSize)
		{
			return new Pointer(pointee, bitSize);
		}

		public TypeVariable CreateTypeVariable()
		{
			return new TypeVariable(AllocateTypeVariable());
		}

		public TypeVariable CreateTypeVariable(string name)
		{
			return new TypeVariable(name, AllocateTypeVariable());
		}

		public UnknownType CreateUnknown()
		{
			return new UnknownType();
		}

        public UnknownType CreateUnknown(int size)
        {
            //$TODO: consider caching these, like PrimitiveType does.
            return new UnknownType(size);
        }

		public UnionType CreateUnionType(string name, DataType preferred)
		{
			return new UnionType(name, preferred);
		}

		public UnionType CreateUnionType(string name, DataType preferred, ICollection<DataType> alternatives)
		{
			return new UnionType(name, preferred, alternatives);
		}

        public VoidType CreateVoidType()
        {
            return VoidType.Instance;
        }

        public EnumType CreateEnum(int Size, Domain Domain, string Name, Serialization.SerializedEnumValue[] Values)
        {
            throw new NotImplementedException();
        }

        public DataType CreateCodeType()
        {
            return new CodeType();
        }
    }
 }
