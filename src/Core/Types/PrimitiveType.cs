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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Core.Types
{
	/// <summary>
	/// Represents a primitive machine data type, with no internal structure.
	/// </summary>
	/// <remarks>
	/// Examples of primitives are integers of different signedness and sizes,
    /// as well as real types and booleans. Pointers to types are not 
    /// considered primitives, as they are type constructors. Primitives are
    /// implemented as immutable flyweights since there are so many of them.
	/// </remarks>
	public class PrimitiveType : DataType
	{
        private static readonly ConcurrentDictionary<(Domain,int), PrimitiveType> cache;
        private static readonly ConcurrentDictionary<string, PrimitiveType> lookupByName;
        private static readonly Dictionary<int, Domain> mpBitWidthToAllowableDomain;
        private static readonly ConcurrentDictionary<int, PrimitiveType> mpBitsizeToWord;

        private readonly int bitSize;
        private readonly int byteSize;
		
		private PrimitiveType(Domain dom, int bitSize, bool isWord, string? name)
            : base(dom, name)
		{
			this.bitSize = bitSize;
            this.byteSize = (bitSize + (BitsPerByte-1)) / BitsPerByte;
            this.IsWord = isWord;
		}

        /// <inheritdoc/>
        public override bool IsPointer => Domain == Domain.Pointer;

        /// <inheritdoc/>
        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitPrimitive(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitPrimitive(this);
        }

        /// <inheritdoc/>
        public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
		{
			return this;
		}

        /// <summary>
        /// Compares this primitive type with another <see cref="PrimitiveType"/>,
        /// first by <see cref="Domain"/>, then by <see cref="BitSize"/>.
        /// </summary>
        /// <param name="that">Other <see cref="PrimitiveType"/>.</param>
        /// <returns>An integer reflecting the </returns>
		public int Compare(PrimitiveType that)
		{
			int d = (int) this.Domain - (int) that.Domain;
			if (d != 0)
				return d;
			return this.bitSize - that.bitSize;
		}

        /// <summary>
        /// Creates a new primitive type, with the specified domain and bit size.
        /// </summary>
        /// <param name="dom">The domain of the type.</param>
        /// <param name="bitSize">The size of the type in bits.</param>
        /// <returns>A new instance of <see cref="PrimitiveType"/>.</returns>
		public static PrimitiveType Create(Domain dom, int bitSize)
		{
			return Create(dom, bitSize, null);
		}

        /// <summary>
        /// Creates a new integer primitive type, with the specified bit size.
        /// </summary>
        /// <param name="bitlength">The size of the type in bits.</param>
        /// <returns>A new instance of <see cref="PrimitiveType"/>.</returns>
        public static PrimitiveType CreateBitSlice(int bitlength)
        {
            if (!cache.TryGetValue((Domain.Integer, bitlength), out PrimitiveType? shared))
            {
                var name = GenerateName(Domain.Integer, bitlength);
                shared = new PrimitiveType(Domain.Integer, bitlength, false, name);
                cache.TryAdd((Domain.Integer, bitlength), shared);
                lookupByName.TryAdd(shared.Name, shared);
            }
            return shared;
        }

        private static PrimitiveType Create(Domain dom, int bitSize, string? name)
        {
            if (mpBitWidthToAllowableDomain.TryGetValue(bitSize, out var domainMask))
            {
                dom &= domainMask;
            }
            if (cache.TryGetValue((dom, bitSize), out var shared))
                return shared;
            var p = new PrimitiveType(dom, bitSize, false, name ?? GenerateName(dom, bitSize));
            return Cache(p);
        }

        private static PrimitiveType Cache(PrimitiveType p)
        {
            if (!cache.TryGetValue((p.Domain, p.BitSize), out var shared))
            {
                shared = p;
                cache.TryAdd((p.Domain, p.bitSize), shared);
                lookupByName.TryAdd(shared.Name, shared);
            }
			return shared;
		}

        /// <summary>
        /// Creates a new primitive type of word, with the specified bit size.
        /// </summary>
        /// <param name="bitSize">Bit size of the word to be created.</param>
        public static PrimitiveType CreateWord(int bitSize)
		{
            if (bitSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bitSize));
            if (mpBitsizeToWord.TryGetValue(bitSize, out var ptWord))
                return ptWord;
			string name;
            if (bitSize == 1)
			{
                name = "bool";
            }
            else if (bitSize == 8)
            {
				name = "byte";
			}
            else
            { 
                name = $"word{bitSize}";
            }
            if (!mpBitWidthToAllowableDomain.TryGetValue(bitSize, out var dom))
            {
                dom = Domain.Integer | Domain.Pointer;
            }
			ptWord = new PrimitiveType(dom, bitSize, true, name);
            Cache(ptWord);
            mpBitsizeToWord[bitSize] = ptWord;
            return ptWord;
		}

        /// <summary>
        /// Creates a new primitive type of word, with the specified bit size.
        /// </summary>
        /// <param name="bitSize">Bit size of the word to be created.</param>
        public static PrimitiveType CreateWord(uint bitSize)
            => CreateWord((int)bitSize);

        /// <inheritdoc/>
		public override bool Equals(object? obj)
		{
            return (obj is PrimitiveType that &&
                    that.Domain == this.Domain && 
                    that.bitSize == this.bitSize);
		}
	
        /// <summary>
        /// Generates a string based on domain and bitsize
        /// </summary>
        /// <remarks>
        /// Note that these are not C data types, but Reko internal types.
        /// The Reko output formatters are responsible for translation to
        /// C (or whatever output language has been chosen by the user).
        /// </remarks>
        /// <param name="dom">Type domain.</param>
        /// <param name="bitSize">Bit size.</param>
		public static string GenerateName(Domain dom, int bitSize)
		{
			StringBuilder sb;
			switch (dom)
			{
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
            case Domain.Offset:
                return "mp" + bitSize;
            case Domain.SegPointer:
                return "segptr" + bitSize;
			case Domain.Real:
				return "real" + bitSize;
			case Domain.Selector:
				return "selector";
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
                if ((dom & Domain.SegPointer) != 0)
                    sb.Append('s');
                if ((dom & Domain.Pointer) != 0)
                    sb.Append('p');
                if ((dom & Domain.Offset) != 0)
                    sb.Append('o');
                if ((dom & Domain.Selector) != 0)
					sb.Append('s');
				if ((dom & Domain.Real) != 0)
					sb.Append('r');
				sb.Append(bitSize);
				return sb.ToString();
			}
		}

        /// <summary>
        /// Given a string <paramref name="primitiveTypeName"/>, tries to parse it into a
        /// <see cref="PrimitiveType"/> instance.
        /// </summary>
        /// <param name="primitiveTypeName">String representation of a primitive data type.</param>
        /// <param name="type">The corresponding <see cref="PrimitiveType"/> instance.</param>
        /// <returns>True if a primitive type with the name <paramref name="primitiveTypeName"/>
        /// existed.
        /// </returns>
        public static bool TryParse(string primitiveTypeName, [MaybeNullWhen(false)] out PrimitiveType type)
        {
            return lookupByName.TryGetValue(primitiveTypeName, out type);
        }

        /// <inheritdoc/>
		public override int GetHashCode()
		{
			return bitSize * 256 ^ Domain.GetHashCode();
		}

        /// <summary>
        /// True if the type can only be some kind of integral numeric type.
        /// </summary>
		public override bool IsIntegral =>
            (Domain & Domain.Integer) != 0 && (Domain & ~Domain.Integer) == 0;

        /// <inheritdoc/>
        public override bool IsReal =>
            Domain == Domain.Real;

        /// <summary>
        /// True if the only thing we known about the type is its size.
        /// It is in effect a vector of bits, and its semantic interpretation
        /// is unknown.
        /// </summary>
        public override bool IsWord { get; }

        /// <summary>
        /// Creates a new primitive type, whose domain is the original domain ANDed 
        /// with the supplied domain mask. If the resulting domain is empty, use the 
        /// supplied domain.
        /// </summary>
        /// <param name="domainMask">Domains to keep.</param>
        /// <returns></returns>
		public PrimitiveType MaskDomain(Domain domainMask)
		{
            var dom = this.Domain & domainMask;
            if (dom == 0)
                dom = domainMask;
            return Create(dom, BitSize);
		}

        /// <inheritdoc/>
        public override int BitSize => this.bitSize;

        /// <summary>
        /// Size of this primitive type in bytes.
        /// </summary>
        public override int Size
		{
			get => byteSize; 
			set => throw new InvalidOperationException("Size of a primitive type cannot be changed."); 
		}

        /// <summary>
        /// All primitive types that have been created so far, indexed by name.
        /// </summary>
        public static ConcurrentDictionary<string, PrimitiveType> AllTypes => lookupByName;

        static PrimitiveType()
        {
            cache = new ConcurrentDictionary<(Domain,int), PrimitiveType>();
            lookupByName = new ConcurrentDictionary<string, PrimitiveType>();
            mpBitWidthToAllowableDomain = new Dictionary<int, Domain>
            {
                { 0, Domain.Any },
                { 1, Domain.Boolean },
                { 8, Domain.Boolean|Domain.Character|Domain.Integer },
                { 16, Domain.Character | Domain.Integer | Domain.Pointer | Domain.Offset | Domain.Selector | Domain.Real },
                { 32, Domain.Integer | Domain.Pointer | Domain.Real | Domain.SegPointer },
                { 64, Domain.Integer | Domain.Pointer | Domain.Real },
                { 80, Domain.Integer | Domain.Pointer | Domain.SegPointer | Domain.Real | Domain.Bcd },
                { 96, Domain.Integer | Domain.Real },
                { 128, Domain.Integer | Domain.Real },
                { 256, Domain.Integer | Domain.Real },
            };
            mpBitsizeToWord = new ConcurrentDictionary<int, PrimitiveType>();

            Bool = Create(Domain.Boolean, 1);

            Byte = CreateWord(8);
            Char = Create(Domain.Character, 8);
            SByte = Create(Domain.SignedInt, 8);
            Int8 = SByte;
            UInt8 = Create(Domain.UnsignedInt, 8);

            Word16 = CreateWord(16);
            Int16 = Create(Domain.SignedInt, 16);
            UInt16 = Create(Domain.UnsignedInt, 16);
            Ptr16 = Create(Domain.Pointer, 16);
            SegmentSelector = Create(Domain.Selector, 16);
            WChar = Create(Domain.Character, 16);
            Offset16 = Create(Domain.Offset, 16);
            Real16 = Create(Domain.Real, 16);

            Word32 = CreateWord(32);
            Int32 = Create(Domain.SignedInt, 32);
            UInt32 = Create(Domain.UnsignedInt, 32);
            Ptr32 = Create(Domain.Pointer, 32);
            SegPtr32 = Create(Domain.SegPointer, 32);
            Real32 = Create(Domain.Real, 32);

            SegPtr48 = Create(Domain.SegPointer, 48);

            Word64 = CreateWord(64);
            Int64 = Create(Domain.SignedInt, 64);
            UInt64 = Create(Domain.UnsignedInt, 64);
            Ptr64 = Create(Domain.Pointer, 64);
            Real64 = Create(Domain.Real, 64);

            Word80 = CreateWord(80);
            Real80 = Create(Domain.Real, 80);
            Bcd80 = Create(Domain.Bcd, 80);

            Real96 = Create(Domain.Real, 96);

            Word128 = CreateWord(128);
            Int128 = Create(Domain.SignedInt, 128);
            UInt128 = Create(Domain.UnsignedInt, 128);
            Real128 = Create(Domain.Real, 128);

            Word256 = CreateWord(256);

            Word512 = CreateWord(512);
        }

        /// <summary>
        /// One-byte boolean type.
        /// </summary>
        public static PrimitiveType Bool { get; }

        /// <summary>
        /// Eight-bit byte.
        /// </summary>
		public static PrimitiveType Byte { get; }

        /// <summary>
        /// 8-bit character.
        /// </summary>
        public static PrimitiveType Char { get; }

        /// <summary>
        /// A signed 8-bit byte.
        /// </summary>
		public static PrimitiveType SByte { get; }

        /// <summary>
        /// Eight-bit signed integer.
        /// </summary>
		public static PrimitiveType Int8 { get; }

        /// <summary>
        /// Eight-bit unsigned integer.
        /// </summary>
		public static PrimitiveType UInt8 { get; }

        /// <summary>
        /// 16-bit word.
        /// </summary>
		public static PrimitiveType Word16 { get; }

        /// <summary>
        /// 16-bit signed integer.
        /// </summary>
		public static PrimitiveType Int16 { get; }

        /// <summary>
        /// 16-bit unsigned integer.
        /// </summary>
		public static PrimitiveType UInt16 { get; }

        /// <summary>
        /// 16-bit pointer.
        /// </summary>
        public static PrimitiveType Ptr16 { get; }

        /// <summary>
        /// 16-bit segment offset.
        /// </summary>
        public static PrimitiveType Offset16 { get; }

        /// <summary>
        /// 16-bit floating point number.
        /// </summary>
        public static PrimitiveType Real16 { get; }

        /// <summary>
        /// Segment selector.
        /// </summary>
		public static PrimitiveType SegmentSelector  {get; }

        /// <summary>
        /// 32-bit word.
        /// </summary>
		public static PrimitiveType Word32 { get; }

        /// <summary>
        /// 32-bit signed integer.
        /// </summary>
		public static PrimitiveType Int32 { get; }

        /// <summary>
        /// 32-bit unsigned integer.
        /// </summary>
		public static PrimitiveType UInt32 { get; }

        /// <summary>
        /// 32-bit pointer.
        /// </summary>
		public static PrimitiveType Ptr32 { get; }

        /// <summary>
        /// 32-bit floating point number.
        /// </summary>
		public static PrimitiveType Real32 { get; }

        /// <summary>
        /// 32-bit segmented pointer.
        /// </summary>
        public static PrimitiveType SegPtr32 { get; }

        /// <summary>
        /// 48-bit segmented pointer.
        /// </summary>
        public static PrimitiveType SegPtr48 { get; }


        /// <summary>
        /// 64-bit word.
        /// </summary>
        public static PrimitiveType Word64 { get; }

        /// <summary>
        /// 64-bit signed integer.
        /// </summary>
		public static PrimitiveType Int64 { get; }

        /// <summary>
        /// 64-bit unsigned integer.
        /// </summary>
		public static PrimitiveType UInt64 { get; }

        /// <summary>
        /// 64-bit pointer.
        /// </summary>
		public static PrimitiveType Ptr64 { get; }

        /// <summary>
        /// 64-bit floating point number.
        /// </summary>
        public static PrimitiveType Real64 { get; }

        /// <summary>
        /// 80-bit word.
        /// </summary>
        public static PrimitiveType Word80 { get; }

        /// <summary>
        /// 80-bit floating point number.
        /// </summary>
		public static PrimitiveType Real80 { get; }

        /// <summary>
        /// 80-bit binary coded decimal number.
        /// </summary>
        public static PrimitiveType Bcd80 { get; }

        /// <summary>
        /// 96-bit floating point number.
        /// </summary>
        public static PrimitiveType Real96 { get; }

        /// <summary>
        /// 128-bit word.
        /// </summary>
        public static PrimitiveType Word128 { get; }

        /// <summary>
        /// 128-bit signed integer.
        /// </summary>
        public static PrimitiveType Int128 { get; }

        /// <summary>
        /// 128-bit unsigned integer.
        /// </summary>
        public static PrimitiveType UInt128 { get; }

        /// <summary>
        /// 128-bit floating point number.
        /// </summary>
        public static PrimitiveType Real128 { get; }

        /// <summary>
        /// 256-bit word.
        /// </summary>
        public static PrimitiveType Word256 { get; }

        /// <summary>
        /// 512-bit word.
        /// </summary>
        public static PrimitiveType Word512 { get; }

        /// <summary>
        /// 16-bit wide character.
        /// </summary>
        public static PrimitiveType WChar { get; }
    }
}