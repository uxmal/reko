/* 
 * Copyright (C) 1999-2009 John Källén.
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
using System;
using System.Collections.Generic;

namespace Decompiler.Core.Types
{
	public class TypeFactory
	{
		private int typeVars;
		private Dictionary<PrimitiveType,PrimitiveType> primitives = new Dictionary<PrimitiveType,PrimitiveType>();

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

		public FunctionType CreateFunctionType(string functionName, DataType returnType, DataType [] paramTypes, string [] paramNames)
		{
			return new FunctionType(functionName, returnType, paramTypes, paramNames);
		}

		public PrimitiveType CreatePrimitiveType(Domain dom, int size)
		{
			return PrimitiveType.Create(dom, size);
		}

		public StructureType CreateStructureType(string name, int size)
		{
			return new StructureType(name, size);
		}

		public StructureType CreateStructureType(string name, int size, StructureField field)
		{
			return new StructureType(name, size, field);
		}

		public MemberPointer CreateMemberPointer(DataType basePointer, DataType pointee, int byteSize)
		{
			return new MemberPointer(basePointer, pointee, byteSize);
		}

		public Pointer CreatePointer(DataType pointee, int byteSize)
		{
			return new Pointer(pointee, byteSize);
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

		public UnionType CreateUnionType(string name, DataType preferred)
		{
			return new UnionType(name, preferred);
		}

		public UnionType CreateUnionType(string name, DataType preferred, ICollection<DataType> alternatives)
		{
			return new UnionType(name, preferred, alternatives);
		}

	}
 }
