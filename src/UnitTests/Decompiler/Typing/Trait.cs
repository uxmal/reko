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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Text;

namespace Reko.UnitTests.Decompiler.Typing
{
	/// <summary>
	/// Traits describe a particular use of a data type.
	/// </summary>
	/// <remarks>
	/// Traits are extracted from the examined program and contain
	/// all the type information of a program. For different aspects, different
	/// forms exist
	/// </remarks>
		
	public abstract class Trait
	{
	}

	public class TraitFunc : Trait
	{
		public TypeVariable FuncType;
		public int FuncPointerSize;
		public TypeVariable ReturnType;		// return type.
		public TypeVariable [] ArgumentTypes;	// types of the parameters.

		public TraitFunc(TypeVariable func, int funcPtrSize, TypeVariable tRet, TypeVariable [] argumentTypes)
		{
			FuncType = func;
			FuncPointerSize = funcPtrSize;
			ReturnType = tRet;
			ArgumentTypes = argumentTypes;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("trait_func(");
			string fmt = "{0}";
			for (int i = 0; i < ArgumentTypes.Length; ++i)
			{
				sb.AppendFormat(fmt, ArgumentTypes[i]);
				fmt = ", {0}";
			}
			sb.AppendFormat(" -> {0})", ReturnType);
			return sb.ToString();
		}
	}

	public class TraitEqual : Trait
	{
		public TypeVariable Type;
		public TypeVariable TypeOther;

		public TraitEqual(TypeVariable typeOther)
		{
			this.TypeOther = typeOther;
		}

		public override string ToString()
		{
			return string.Format("trait_equal({0})", TypeOther);
		}
	}


	public class TraitDataType: Trait
	{
		private DataType dt;

		public TraitDataType(DataType dt)
		{
			this.dt = dt;
		}

		public override string ToString()
		{
			return string.Format("trait_primitive({0})", dt);
		}

		public DataType DataType
		{
			get { return dt; }
		}
	}


	public class TraitMem : Trait // Memory Access
	{
		public TypeVariable BasePointer;
		public int MemPointerSize;
		public TypeVariable FieldType;	// type of a field.
		public int Offset;			// offset of field.

		public TraitMem(TypeVariable basePtr, int memPointerSize, TypeVariable fieldType, int offset)
		{
			this.BasePointer = basePtr;
			this.MemPointerSize = memPointerSize;
			this.FieldType = fieldType; 
			this.Offset = offset; 
		}

		public override string ToString()
		{
			return string.Format(
				BasePointer is null
				? "trait_mem({1}, {2:X})"
				: "trait_mem({0}:{1}, {2:X})",
				BasePointer, FieldType, Offset);
		}

	}

	public class TraitMemArray : Trait
	{
		public TypeVariable BasePointer;
		public int MemPointerSize;
		public TypeVariable AccessType;
		public int Offset;
		public int ElementSize;
		public int Length;

		public TraitMemArray(TypeVariable basePtr, int structPtrSize, int offset, int elementSize, int length, TypeVariable tAccess)
		{
			this.BasePointer = basePtr;
			this.Offset = offset;
			this.ElementSize = elementSize;
			this.Length = length;
			this.AccessType = tAccess;
			if (tAccess is null) 
				throw new ArgumentNullException(nameof(tAccess));
		}

		public override string ToString()
		{
			return string.Format(
				BasePointer is null ?
				"trait_mem_array({1:X}, {2}, {3}, {4})"
				: "trait_mem_array({0}:{1:X}, {2}, {3}, {4})",
				BasePointer, Offset, ElementSize, Length, AccessType);
		}
	}


	public class TraitMemSize : Trait
	{
		public int Size;			// size in bytes.

		public TraitMemSize(int size)
		{
			this.Size = size;
		}

		public override string ToString()
		{
			return string.Format("trait_memsize({0})", Size);
		}

	}

	public class TraitArray : Trait
	{
		public TypeVariable Type;
		public int ArrayPointerSize;
		public int ElementSize;			
		public int Length;		// if 0, unknown # of elements

		public TraitArray(int elementSize, int length)
		{
			this.ElementSize = elementSize;
			this.Length = length;
		}

		public override string ToString()
		{
			return string.Format("trait_array({0}, {1})", ElementSize, Length);
		}
	}

	public class UserSelect : Trait
	{
		public TypeVariable Type;
		public UnionType union_type;
		public int wished_Type;	// sizeof (t) == sizeof(u);
	}

	public class UserTrait : Trait
	{
		public TypeVariable Type;
		public Trait trait;
	}

	public class UserNameElement : Trait
	{
			// Type is a struct, union, or function.
		public TypeVariable Type;
		public int index;
		public string Name;			
	}

	public class UserNameType : Trait
	{
		// Used to give a type a name.
		public TypeVariable Type;
		public string Name;
	}

	/// <summary>
	/// Provides a new type of group, which is formed from the range starting from offset and
	/// the size of size from type of group the t. No elements of t may intersect the range
	/// </summary>
	public class UserMemMem : Trait
	{
		public TypeVariable Type;
		public int Offset;
		public int Size;
	}

	public class UserMemArray : Trait
	{
		public TypeVariable Type;
		public int t, offset, size, elements;
		// Provides a new array time with elements elements, 
		// which are located at offset and whose size is size and 
		// whose type is t. The elements in the range
		// must be compatible with the field type.
	}

	public class UserArrayMem : Trait
	{
		public TypeVariable Type;
		public ArrayType type; // converts the array type t to a struct
	}

	public class UserFunctionSignature : Trait
	{
		public TypeVariable Type;
		public string Name;
		public TypeVariable ReturnType;
		public TypeVariable [] ParameterTypes;
		public string [] ArgumentNames;
	}

	public class TraitPointer : Trait
	{
		private TypeVariable pointee;

		public TraitPointer(TypeVariable pointee)
		{
			this.pointee = pointee;
		}

		public override string ToString()
		{
			return string.Format("trait_ptr({0}", pointee);
		}
	}
	///

	public class TraitConstant : Trait
	{
		public Constant Value;

		public TraitConstant(Constant c)
		{
			this.Value = c;
		}

		public override string ToString()
		{
			return string.Format("trait_const({0})", Value);
		}
	}
}
