/* 
 * Copyright (C) 1999-2007 John Källén.
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

namespace Decompiler.Core.Types
{
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
		private readonly Domain dom;
		private readonly int byteSize;
		private readonly Sign sign;

		static private Hashtable primitives;
		static private PrimitiveType _void;
		static private PrimitiveType bool1;
		static private PrimitiveType byte8;
		static private PrimitiveType char8;
		static private PrimitiveType sbyte8;
		static private PrimitiveType word16;
		static private PrimitiveType int16;
		static private PrimitiveType uint16;
		static private PrimitiveType word32;
		static private PrimitiveType int32;
		static private PrimitiveType uint32;
		static private PrimitiveType word64;
		static private PrimitiveType int64;
		static private PrimitiveType real32;
		static private PrimitiveType real64;
		static private PrimitiveType real80;
		static private PrimitiveType pointer;
		static private PrimitiveType pointer32;
		static private PrimitiveType segment;

		public PrimitiveType(Domain dom, int byteSize, Sign sign)
		{
			if (byteSize == 3)
				this.byteSize = byteSize;
			this.dom = dom;
			this.byteSize = byteSize;
			this.sign = sign;
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
			int d = (int) dom - (int) p.dom;
			if (d != 0)
				return d;
			d = (int) byteSize - (int) p.byteSize;
			if (d != 0)
				return d;
			return (int) sign - (int) p.sign;
		}

		public static PrimitiveType Create(Domain dom, int byteSize, Sign sign)
		{
			PrimitiveType p = new PrimitiveType(dom, byteSize, sign);
			PrimitiveType pShared = (PrimitiveType) primitives[p];
			if (pShared == null)
			{
				primitives[p] = p;
				pShared = p;
			}
			return pShared;
		}

		public Domain Domain
		{
			get { return dom; }
		}

		public override bool Equals(object obj)
		{
			PrimitiveType p = obj as PrimitiveType;
			if (p == null)
				return false;
			return dom == p.dom && byteSize == p.byteSize && sign == p.sign;
		}

		public override int GetHashCode()
		{
			return ((((int) sign << 8) | (int) dom) << 8) | (int) byteSize;
		}

		public PrimitiveType GetSignedEquivalent()
		{
			switch (this.dom)
			{
			case Domain.Integral: return Create(dom, this.Size, Sign.Signed);
			default: throw new InvalidOperationException(string.Format("{0} cannot be made signed", this.ToString()));
			}
		}


		public PrimitiveType GetUnsignedEquivalent()
		{
			switch (this.dom)
			{
			case Domain.Integral: return Create(dom, this.Size, Sign.Unsigned);
			default: throw new InvalidOperationException(string.Format("{0} cannot be made signed", this.ToString()));
			}
		}

		public override string Prefix
		{
			get 
			{ 
				switch (dom)
				{
				case Domain.None:
					return "v";
				case Domain.Boolean:
					return "f";
				case Domain.Integral:
					switch (byteSize)
					{
					case 1: return "b";
					case 2: return "w";
					case 4: return "dw";
					case 8: return "qw";
					}
					break;
				case Domain.Real:
					return "r";
				case Domain.Pointer:
					return "ptr";
				case Domain.Segment:
					return "pseg";
				}
				throw new ArgumentOutOfRangeException();
			}
		}

		public Sign Sign
		{
			get { return sign; }
		}

		/// <summary>
		/// Size of type in bytes.
		/// </summary>
		public override int Size
		{
			get { return byteSize; }		
			set { ThrowBadSize(); }
		}

		public override void Write(TextWriter writer)
		{
			int bitSize = BitSize;

			switch (Domain)
			{
			case Domain.None:
				writer.Write("void");
				break;
			case Domain.Boolean:
				writer.Write("bool");
				break;
			case Domain.Integral:
				if (Sign == Sign.Unknown)
				{
					if (bitSize == 8)
						writer.Write("byte");
					else
						writer.Write("word{0}", bitSize);
				}
				else if (bitSize == 8)
				{
					writer.Write("{0}byte", Sign == Sign.Signed ? "s" : "");
				}
				else
				{
					writer.Write("{0}int{1}", 
						Sign == Sign.Unsigned ? "u" : "",
						bitSize);
				}
				return;
			case Domain.Real:
				writer.Write("real{0}", bitSize);
				return;
			case Domain.Pointer:
				writer.Write("ptr{0}", bitSize);
				return;
			case Domain.Segment:
				writer.Write("segment");
				return;
			default:
				throw new ArgumentException("Illegal domain.");
			}
		}

		public static PrimitiveType Void
		{
			get { return _void; }
		}

		public static PrimitiveType Bool
		{
			get { return bool1; }
		}

		public static PrimitiveType Byte
		{
			get { return byte8; }
		}

		public static PrimitiveType Char
		{
			get { return char8; }
		}

		public static PrimitiveType SByte
		{
			get { return sbyte8; }
		}

		public static PrimitiveType Word16
		{
			get { return word16; }
		}

		public static PrimitiveType Int16
		{
			get { return int16; }
		}

		public static PrimitiveType UInt16
		{
			get { return uint16; }
		}

		public static PrimitiveType Word32
		{
			get { return word32; }
		}

		public static PrimitiveType Int32
		{
			get { return int32; }
		}

		public static PrimitiveType UInt32
		{
			get { return uint32; }
		}

		public static PrimitiveType Word64
		{
			get { return word64; }
		}

		public static PrimitiveType Int64
		{
			get { return int64; }
		}

		public static PrimitiveType Real32
		{
			get { return real32; }
		}

		public static PrimitiveType Real64
		{
			get { return real64; }
		}

		public static PrimitiveType Real80
		{
			get { return real80; }
		}


		public static PrimitiveType Pointer
		{
			get { return pointer; } 
		}

		public static PrimitiveType Pointer32
		{
			get { return pointer32; }
		}

		public static PrimitiveType Segment
		{
			get { return segment; }
		}

		static PrimitiveType()
		{
			primitives = new Hashtable();
			_void = Create(Domain.None, 0, Sign.Unknown);
			bool1 = Create(Domain.Boolean, 1, Sign.Unknown);
			byte8 = Create(Domain.Integral, 1, Sign.Unknown);
			char8 = Create(Domain.Integral, 1, Sign.Unsigned);
			sbyte8 = Create(Domain.Integral, 1, Sign.Signed);
			word16 = Create(Domain.Integral, 2, Sign.Unknown);
			int16 =  Create(Domain.Integral, 2, Sign.Signed);
			uint16 = Create(Domain.Integral, 2, Sign.Unsigned);
			word32 = Create(Domain.Integral, 4, Sign.Unknown);
			int32 =  Create(Domain.Integral, 4, Sign.Signed);
			uint32 = Create(Domain.Integral, 4, Sign.Unsigned);
			word64 = Create(Domain.Integral, 8, Sign.Unsigned);
			int64 =  Create(Domain.Integral, 8, Sign.Signed);
			real32 = Create(Domain.Real, 4, Sign.Signed);
			real64 = Create(Domain.Real, 8, Sign.Signed);
			real80 = Create(Domain.Real, 10, Sign.Signed);
			pointer = Create(Domain.Pointer, 0, Sign.Unsigned);
			pointer32 = Create(Domain.Pointer, 4, Sign.Unsigned);
			segment = Create(Domain.Segment, 2, Sign.Unsigned);
		}
		
	}

	public enum Sign
	{
		Unknown,
		Signed,
		Unsigned
	}

	public enum Domain
	{
		None,
		Boolean,
		Integral,
		Real,
		Pointer,
		Segment
	}
}
