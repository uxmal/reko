#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
using System.IO;
using System.Text;

namespace Decompiler.Core.Types
{

    /// <summary>
    /// A Domain specifies the possible interpretation of a chunk of bytes.
    /// </summary>
    /// <remarks>
    /// A 32-bit load from memory could mean that the variable could be treated as an signed int, unsigned int,
    /// floating point number, a pointer to something. As the decompiler records how the value is used,
    /// some of these alternatives will be discarded. For instance, if the 32-bit word is used in a memory access,
    /// it is certain that it is a pointer to (something), and it can't be a float.
    /// </remarks>
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
		Selector = 128,
        PtrCode = 256,      // Pointer to executable code.
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
		private short byteSize;
		
		private PrimitiveType(Domain dom, short byteSize, string name)
		{
			if (dom == 0)
				throw new ArgumentException("Domain is empty.");
			this.Domain = dom;
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

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitPrimitive(this);
        }

		public override DataType Clone()
		{
			return this;
		}

		public int Compare(object a)
		{
			PrimitiveType p = (PrimitiveType) a;
			int d = (int) Domain - (int) p.Domain;
			if (d != 0)
				return d;
			return byteSize - p.byteSize;
		}

		public static PrimitiveType Create(Domain dom, int byteSize)
		{
			return Create(dom, (short) byteSize, null);
		}

		private static PrimitiveType Create(Domain dom, short byteSize, string name)
		{
			PrimitiveType p = new PrimitiveType(dom, byteSize, null);
			PrimitiveType shared;
            if (!cache.TryGetValue(p, out shared))
			{
				shared = p;
				shared.Name = name != null ? name : GenerateName(dom, p.BitSize);
                cache.Add(shared, shared);
                lookupByName.Add(shared.Name, shared);
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
				w = Domain.SignedInt|Domain.UnsignedInt|Domain.Pointer|Domain.Selector|Domain.PtrCode;
				name = "word16";
				break;
			case 4:
                w = Domain.SignedInt|Domain.UnsignedInt|Domain.Pointer|Domain.Real|Domain.PtrCode;
				name = "word32";
				break;
			case 8:
                w = Domain.SignedInt|Domain.UnsignedInt|Domain.Pointer|Domain.Real|Domain.PtrCode;
				name = "word64";
				break;
			default:
				throw new ArgumentException("Only primitives of sizes 1, 2, 4, or 8 bytes are supported.");
			}
			return Create(w, (short) byteSize, name);
		}

        public Domain Domain { get; private set; }

		public override bool Equals(object obj)
		{
			PrimitiveType p = obj as PrimitiveType;
			if (p == null)
				return false;
			return p.Domain == Domain && p.byteSize == byteSize;
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
                if (bitSize == 8)
				    return "char";
                if (bitSize == 16)
                    return "wchar_t";
                return "char" + bitSize;
			case Domain.SignedInt:
				return "int" + bitSize;
			case Domain.UnsignedInt:
				return "uint" + bitSize;
			case Domain.Pointer:
				return "ptr" + bitSize;
			case Domain.Real:
				return "real" + bitSize;
			case Domain.Selector:
				return "selector";
            case Domain.PtrCode:
                return "pfn" + bitSize;
			default:
				sb = new StringBuilder();
				if ((dom & Domain.Boolean) != 0)
					sb.Append('b');
				if ((dom & Domain.Character) != 0)
					sb.Append('c');
				if ((dom & Domain.UnsignedInt) != 0)
					sb.Append('u');
                if ((dom & Domain.SignedInt) != 0)
                    sb.Append('i');
                if ((dom & Domain.Pointer) != 0)
					sb.Append('p');
				if ((dom & Domain.Selector) != 0)
					sb.Append('s');
				if ((dom & Domain.Real) != 0)
					sb.Append('r');
                if ((dom & Domain.PtrCode) != 0)
                    sb.Append("pfn");
				sb.Append(bitSize);
				return sb.ToString();
			}
		}

        public static bool TryParse(string primitiveTypeName, out PrimitiveType type)
        {
            return lookupByName.TryGetValue(primitiveTypeName, out type);
        }

		public override int GetHashCode()
		{
			return byteSize * 256 ^ Domain.GetHashCode();
		}

        /// <summary>
        /// True if the type can only be some kind of numeric type
        /// </summary>
		public bool IsIntegral
		{
			get { return (Domain & (Domain.SignedInt|Domain.UnsignedInt)) != 0; }
		}

        /// <summary>
        /// Creates a new primitive type, whose domain is the original domain ANDed 
        /// with the supplied domain mask.
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
		public PrimitiveType MaskDomain(Domain domainMask)
		{
			return Create(this.Domain & domainMask, Size);
		}

		public override string Prefix
		{
			get
			{
				switch (Domain)
				{
				case Domain.None:
                case Domain.Void:
					return "v";
				case Domain.Boolean:
					return "f";
				case Domain.Real:
					return "r";
				case Domain.Pointer:
					return "ptr";
				case Domain.Selector:
					return "pseg";
                case Domain.PtrCode:
                    return "pfn";
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

		/// <summary>
		/// Size of this primitive type in bytes.
		/// </summary>
		public override int Size
		{
			get { return byteSize; }
			set { throw new InvalidOperationException("Size of a primitive type cannot be changed."); }
		}

		private static Dictionary<PrimitiveType,PrimitiveType> cache;
        private static Dictionary<string, PrimitiveType> lookupByName;

		static PrimitiveType()
		{
			cache = new Dictionary<PrimitiveType,PrimitiveType>();
            lookupByName = new Dictionary<string, PrimitiveType>();

			Void = Create(Domain.Void, 0);

			Byte = CreateWord(1);
			Bool = Create(Domain.Boolean, 1);
		    Char = Create(Domain.Character, 1);
			SByte = Create(Domain.SignedInt, 1);
			UInt8 = Create(Domain.UnsignedInt, 1);

			Word16 = CreateWord(2);
			Int16 = Create(Domain.SignedInt, 2);
			UInt16 = Create(Domain.UnsignedInt, 2);
			Ptr16 = Create(Domain.Pointer, 2);
			SegmentSelector = Create(Domain.Selector, 2);
            WChar = Create(Domain.Character, 2);
            PtrCode16 = Create(Domain.PtrCode, 2);

			Word32 = CreateWord(4);
			Int32 = Create(Domain.SignedInt, 4);
			UInt32 = Create(Domain.UnsignedInt, 4);
			Pointer32 = Create(Domain.Pointer, 4);
			Real32 = Create(Domain.Real, 4);
            PtrCode32 = Create(Domain.PtrCode, 4);

			Word64 = CreateWord(8);
			Int64 = Create(Domain.SignedInt, 8);
			UInt64 = Create(Domain.UnsignedInt, 8);
			Pointer64 = Create(Domain.Pointer, 8);
			Real64 = Create(Domain.Real, 8);
            PtrCode64 = Create(Domain.PtrCode, 8);

			Real80 = Create(Domain.Real, 10);

            Real128 = Create(Domain.Real, 16);
		}

		public static PrimitiveType Void { get; private set; }
		
		public static PrimitiveType Bool {get; private set; }

		public static PrimitiveType Byte {get; private set; }
		public static PrimitiveType Char {get; private set; }
		public static PrimitiveType SByte {get; private set; }
		public static PrimitiveType UInt8 {get; private set; }

		public static PrimitiveType Word16 {get; private set; }
		public static PrimitiveType Int16 {get; private set; }
		public static PrimitiveType UInt16 {get; private set; }
        public static PrimitiveType Ptr16 { get; private set; }
        public static PrimitiveType PtrCode16 { get; private set; }

		public static PrimitiveType SegmentSelector {get; private set; }

		public static PrimitiveType Word32 {get; private set; }
		public static PrimitiveType Int32 {get; private set; }
		public static PrimitiveType UInt32 {get; private set; }
		public static PrimitiveType Pointer32 {get; private set; }
		public static PrimitiveType Real32 {get; private set; }
        public static PrimitiveType PtrCode32 { get; private set; }

		public static PrimitiveType Word64 {get; private set; }
		public static PrimitiveType Int64 {get; private set; }
		public static PrimitiveType UInt64 {get; private set; }
		public static PrimitiveType Pointer64 {get; private set; }
        public static PrimitiveType Real64 { get; private set; }
        public static PrimitiveType PtrCode64 { get; private set; }

		public static PrimitiveType Real80 {get; private set; }

        public static PrimitiveType Real128 { get; private set; }

        public static PrimitiveType WChar { get; private set; } 
    }
}