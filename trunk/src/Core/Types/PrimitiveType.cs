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
using System.Collections;
using System.IO;
using System.Text;

namespace Decompiler.Core.Types
{

	[Flags]
	public enum Domain
	{
		None = 0,
		Void = 1,
		Boolean = 2,
		Character = 4,
		SignedInt = 8,
		UnsignedInt = 16,
		Real = 32,
		Pointer = 64,
		Segment = 128,
	}

	/// <summary>
	/// Represents a primitive machine data type, with no internal structure.
	/// </summary>
	/// <remarks>
	/// Examples of primitives are integers of different signedness and sizes, as well as real types and booleans.
	/// Pointers to types are not considered primitives, as they are type constructors. Primitives are implemented
	/// as flyweights since there are so many of them.
	/// </remarks>
	public class PrimitiveType : DataType
	{
		private Domain domain;
		private int byteSize;
		
		private PrimitiveType(Domain dom, int byteSize, string name)
		{
			if (dom == 0)
				throw new ArgumentException("Domain is empty.");
			this.domain = dom;
			this.byteSize = byteSize;
			this.Name = name;
		}

		public override DataType Accept(DataTypeTransformer t)
		{
			return t.TransformPrimitiveType(this);
		}

		public override void Accept(IDataTypeVisitor v)
		{
			v.VisitPrimitive(this);
		}


		public override DataType Clone()
		{
			return this;
		}

		public int Compare(object a)
		{
			PrimitiveType p = (PrimitiveType) a;
			int d = (int) domain - (int) p.domain;
			if (d != 0)
				return d;
			return byteSize - p.byteSize;
		}
		public static PrimitiveType Create(Domain dom, int byteSize)
		{
			return Create(dom, byteSize, null);
		}

		private static PrimitiveType Create(Domain dom, int byteSize, string name)
		{
			PrimitiveType p = new PrimitiveType(dom, byteSize, null);
			PrimitiveType shared = (PrimitiveType) cache[p];
			if (shared == null)
			{
				shared = p;
				shared.Name = name != null ? name : GenerateName(dom, p.BitSize);
				cache[p] = shared;
			}
			return shared;
		}

		public static PrimitiveType CreateWord(int byteSize)
		{
			Domain w;
			string name;
			switch (byteSize)
			{
			case 1:
				w = Domain.Boolean|Domain.Character|Domain.SignedInt|Domain.UnsignedInt|Domain.UnsignedInt;
				name = "byte";
				break;
			case 2:
				w = Domain.SignedInt|Domain.UnsignedInt|Domain.Pointer|Domain.Segment;
				name = "word16";
				break;
			case 4:
				w = Domain.SignedInt|Domain.UnsignedInt|Domain.Pointer|Domain.Real;
				name = "word32";
				break;
			case 8:
				w = Domain.SignedInt|Domain.UnsignedInt|Domain.Pointer|Domain.Real;
				name = "word64";
				break;
			default:
				throw new ArgumentException("Only primitives of sizes 1, 2, 4, or 8 bytes are supported.");
			}
			return Create(w, byteSize, name);
		}

		public Domain Domain
		{
			get { return domain; }
		}

		public override bool Equals(object obj)
		{
			PrimitiveType p = obj as PrimitiveType;
			if (p == null)
				return false;
			return p.domain == domain && p.byteSize == byteSize;
		}
	
		public static string GenerateName(Domain dom, int bitSize)
		{
			StringBuilder sb;
			switch (dom)
			{
			case Domain.Void:
				return "void";
			case Domain.Boolean:
				return "bool";
			case Domain.Character:
				return "char";
			case Domain.SignedInt:
				return "int" + bitSize;
			case Domain.UnsignedInt:
				return "uint" + bitSize;
			case Domain.Pointer:
				return "ptr" + bitSize;
			case Domain.Real:
				return "real" + bitSize;
			case Domain.Segment:
				return "segment";
			default:
				sb = new StringBuilder();
				if ((dom & Domain.Boolean) != 0)
					sb.Append('b');
				if ((dom & Domain.Character) != 0)
					sb.Append('c');
				if ((dom & Domain.SignedInt) != 0)
					sb.Append('i');
				if ((dom & Domain.UnsignedInt) != 0)
					sb.Append('u');
				if ((dom & Domain.Pointer) != 0)
					sb.Append('p');
				if ((dom & Domain.Segment) != 0)
					sb.Append('s');
				if ((dom & Domain.Real) != 0)
					sb.Append('r');
				sb.Append(bitSize);
				return sb.ToString();
			}
		}

		public override int GetHashCode()
		{
			return byteSize * 256 ^ domain.GetHashCode();
		}

		public bool IsIntegral
		{
			get { return (domain & (Domain.SignedInt|Domain.UnsignedInt)) != 0; }
		}

		public PrimitiveType MaskDomain(Domain dom)
		{
			return Create(this.Domain & dom, Size);
		}

		public override string Prefix
		{
			get
			{
				switch (domain)
				{
				case Domain.None:
					return "v";
				case Domain.Boolean:
					return "f";
				case Domain.Real:
					return "r";
				case Domain.Pointer:
					return "ptr";
				case Domain.Segment:
					return "pseg";
				default:
					switch (byteSize)
					{
					case 1: return "b";
					case 2: return "w";
					case 4: return "dw";
					case 8: return "qw";
					default: throw new ArgumentOutOfRangeException();
					}
				}
			}
		}

		public override int Size
		{
			get { return byteSize; }
			set { throw new InvalidOperationException("Size of a primitive type cannot be changed."); }
		}

		private static Hashtable cache;

		static PrimitiveType()
		{
			cache = new Hashtable();

			_void = Create(Domain.Void, 0);

			_byte = CreateWord(1);
			bool1 = Create(Domain.Boolean, 1);
			_char = Create(Domain.Character, 1);
			int8 = Create(Domain.SignedInt, 1);
			uint8 = Create(Domain.UnsignedInt, 1);

			word16 = CreateWord(2);
			int16 = Create(Domain.SignedInt, 2);
			uint16 = Create(Domain.UnsignedInt, 2);
			ptr16 = Create(Domain.Pointer, 2);
			segment = Create(Domain.Segment, 2);

			word32 = CreateWord(4);
			int32 = Create(Domain.SignedInt, 4);
			uint32 = Create(Domain.UnsignedInt, 4);
			pointer32 = Create(Domain.Pointer, 4);
			real32 = Create(Domain.Real, 4);

			word64 = CreateWord(8);
			int64 = Create(Domain.SignedInt, 8);
			uint64 = Create(Domain.UnsignedInt, 8);
			pointer64 = Create(Domain.Pointer, 8);
			real64 = Create(Domain.Real, 8);

			real80 = Create(Domain.Real, 10);

			pointer = Create(Domain.Pointer, 0);
		}

		static private PrimitiveType _void;

		static private PrimitiveType _byte;
		static private PrimitiveType bool1;
		static private PrimitiveType _char;
		static private PrimitiveType int8;
		static private PrimitiveType uint8;

		static private PrimitiveType word16;
		static private PrimitiveType int16;
		static private PrimitiveType uint16;
		static private PrimitiveType ptr16;
		static private PrimitiveType segment;

		static private PrimitiveType word32;
		static private PrimitiveType int32;
		static private PrimitiveType uint32;
		static private PrimitiveType pointer32;
		static private PrimitiveType real32;

		static private PrimitiveType word64;
		static private PrimitiveType int64;
		static private PrimitiveType uint64;
		static private PrimitiveType pointer64;
		static private PrimitiveType real64;

		static private PrimitiveType real80;

		static private PrimitiveType pointer;

		static public PrimitiveType Void
		{
			get { return _void; }
		}
		
		static public PrimitiveType Bool
		{
			get { return bool1; }
		}

		static public PrimitiveType Byte
		{
			get { return _byte; }
		}
		static public PrimitiveType Char
		{
			get { return _char; }
		}
		static public PrimitiveType SByte
		{
			get { return int8; }
		}
		static public PrimitiveType UInt8
		{
			get { return uint8; }
		}

		static public PrimitiveType Word16
		{
			get { return word16; }
		}
		static public PrimitiveType Int16
		{
			get { return int16; }
		}
		static public PrimitiveType UInt16
		{
			get { return uint16; }
		}
		static public PrimitiveType Ptr16
		{
			get { return ptr16; }
		}
		static public PrimitiveType Segment
		{
			get { return segment; }
		}

		static public PrimitiveType Word32
		{
			get { return word32; }
		}
		static public PrimitiveType Int32
		{
			get { return int32; }
		}
		static public PrimitiveType UInt32
		{
			get { return uint32; }
		}
		static public PrimitiveType Pointer32
		{
			get { return pointer32; }
		}
		static public PrimitiveType Real32
		{
			get { return real32; }
		}

		static public PrimitiveType Word64
		{
			get { return word64; }
		}
		static public PrimitiveType Int64
		{
			get { return int64; }
		}
		static public PrimitiveType UInt64
		{
			get { return uint64; }
		}
		static public PrimitiveType Pointer64
		{
			get { return pointer64; }
		}
		static public PrimitiveType Real64
		{
			get { return real64; }
		}

		static public PrimitiveType Real80
		{
			get { return real80; }
		}

		static public PrimitiveType Pointer
		{
			get { return pointer; }
		}
	}
}