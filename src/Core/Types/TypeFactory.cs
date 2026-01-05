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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;

namespace Reko.Core.Types
{
    /// <summary>
    /// Factory class for creating types.
    /// </summary>
	public class TypeFactory
	{
		private int typeVars;

        /// <summary>
        /// Constructs a <see cref="TypeFactory"/> instance.
        /// </summary>
		public TypeFactory()
		{
			typeVars = 0;
		}

		private int AllocateTypeVariable()
		{
			return ++typeVars;
		}

        /// <summary>
        /// Creates an array.
        /// </summary>
        /// <param name="elType">Element type.</param>
        /// <param name="length">Length of the array.</param>
        /// <returns>The created array.</returns>
		public ArrayType CreateArrayType(DataType elType, int length)
		{
			return new ArrayType(elType, length);
		}

        /// <summary>
        /// Creates a function type.
        /// </summary>
        /// <param name="returnType">Function return type.</param>
        /// <param name="parameters">Function parameters.</param>
        /// <returns>The created function type.</returns>
		public FunctionType CreateFunctionType(Identifier returnType, Identifier [] parameters)
		{
			return new FunctionType(parameters, [returnType]);
		}

        /// <summary>
        /// Creates a primitive type.
        /// </summary>
        /// <param name="dom">Type domain.</param>
        /// <param name="bitSize">Bit size.</param>
        /// <returns>The created primitive type.</returns>
		public PrimitiveType CreatePrimitiveType(Domain dom, int bitSize)
		{
			return PrimitiveType.Create(dom, bitSize);
		}

        /// <summary>
        /// Creates an empty structure type.
        /// </summary>
        /// <returns>The created structure type.</returns>
        public StructureType CreateStructureType()
        {
            return new StructureType();
        }

        /// <summary>
        /// Creates a structure type with a known size.
        /// </summary>
        /// <param name="name">Optional name of the structure.</param>
        /// <param name="size">Size of the structure in storage units.
        /// A value of 0 means "unknown size".
        /// </param>
        /// <returns></returns>
		public StructureType CreateStructureType(string? name, int size)
		{
			return new StructureType(name, size);
		}

        /// <summary>
        /// Creates a structure type with a single field.
        /// </summary>
        /// <param name="name">Optional name of the structure.</param>
        /// <param name="size">Size of the structure in storage units.
        /// A value of 0 means "unknown size".
        /// </param>
        /// <param name="field">Field to add to the structure.</param>
        /// <returns>The created structure.</returns>
		public StructureType CreateStructureType(string? name, int size, StructureField field)
		{
            return new StructureType(name, size) { Fields = { field } };
		}

        /// <summary>
        /// Creates a member pointer type.
        /// </summary>
        /// <param name="basePointer">Base type.</param>
        /// <param name="pointee">Type of the offset.</param>
        /// <param name="bitSize">Bit size of the pointer.</param>
        /// <returns>The created member pointer.</returns>
		public MemberPointer CreateMemberPointer(DataType basePointer, DataType pointee, int bitSize)
		{
			return new MemberPointer(basePointer, pointee, bitSize);
		}

        /// <summary>
        /// Creates a pointer type.
        /// </summary>
        /// <param name="pointee">The type pointed to.</param>
        /// <param name="bitSize">The bit size of the pointer.</param>
        /// <returns>The created pointer.</returns>
		public Pointer CreatePointer(DataType pointee, int bitSize)
		{
			return new Pointer(pointee, bitSize);
		}

        /// <summary>
        /// Creates a type variable.
        /// </summary>
        /// <returns>The new type variable.</returns>
		public TypeVariable CreateTypeVariable()
		{
			return new TypeVariable(AllocateTypeVariable());
		}

        /// <summary>
        /// Creates a type variable with a name.
        /// </summary>
        /// <param name="name">Name of the type variable.</param>
        /// <returns>The new type variable.</returns>
		public TypeVariable CreateTypeVariable(string name)
		{
			return new TypeVariable(name, AllocateTypeVariable());
		}

        /// <summary>
        /// Creates an instance of an <see cref="UnknownType"/>.
        /// </summary>
        /// <returns>The new unknown type.</returns>
		public UnknownType CreateUnknown()
		{
			return new UnknownType();
		}

        /// <summary>
        /// Creates an instance of an <see cref="UnknownType"/> with a known size.
        /// </summary>
        /// <param name="size">The size of the unknown type.</param>
        /// <returns>The new unknown type.</returns>
        public UnknownType CreateUnknown(int size)
        {
            //$TODO: consider caching these, like PrimitiveType does.
            return new UnknownType(size);
        }

        /// <summary>
        /// Creates a union type.
        /// </summary>
        /// <param name="name">Name of the union type.</param>
        /// <param name="preferred">Optional preferred alternative.</param>
        /// <returns>The new union type.</returns>
		public UnionType CreateUnionType(string name, DataType preferred)
		{
			return new UnionType(name, preferred);
		}

        /// <summary>
        /// Creates a union type with a preferred alternative and a list of alternatives.
        /// </summary>
        /// <param name="name">Name of the union type.</param>
        /// <param name="preferred">Optional preferred alternative.</param>
        /// <returns>The new union type.</returns>
        /// <param name="alternatives">The union alternatives.</param>
        /// <returns>The new union type.</returns>
		public UnionType CreateUnionType(string? name, DataType? preferred, ICollection<DataType> alternatives)
		{
			return new UnionType(name, preferred, alternatives);
		}

        /// <summary>
        /// Creates the void type instance.
        /// </summary>
        public VoidType CreateVoidType()
        {
            return VoidType.Instance;
        }

        /// <summary>
        /// Creates an enum type.
        /// </summary>
        /// <param name="Size">Size of the members of the enum.</param>
        /// <param name="Domain">Type domain of the enum members.</param>
        /// <param name="Name">The name of the enum.</param>
        /// <param name="Values">The values</param>
        /// <returns>The new enum type.</returns>
        public EnumType CreateEnum(int Size, Domain Domain, string Name, Serialization.SerializedEnumValue[] Values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an instance of <see cref="CodeType"/>.
        /// </summary>
        /// <returns>The new <see cref="CodeType"/>.</returns>
        public DataType CreateCodeType()
        {
            return new CodeType();
        }
    }
 }
