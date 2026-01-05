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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Abstraction of the notion of "address". Some processors have nice 
    /// linear addresses (z80, PowerPC) and some others have eldritch 
    /// segmented addresses (x86).
    /// </summary>
    public struct Address :
        Expression,
        IComparable<Address>, IComparable,
        MachineOperand
    {
        private DataType dt;
        private readonly ulong uOffset;
        private readonly ushort selector;
        private readonly byte type;
        private readonly byte offsetBitsize;

        private const byte Invalid = 0;
        private const byte LinearHex = 1;
        private const byte LinearOctal = 2;
        private const byte SegmentedReal = 3;
        private const byte SegmentedProt = 4;

        /// <inheritdoc />
        public readonly IEnumerable<Expression> Children => [];

        private Address(DataType dt, byte type, byte bitsize, ulong offset, ushort selector)
        {
            this.dt = dt;
            this.uOffset = offset;
            this.selector = selector;
            this.type = type;
            this.offsetBitsize = bitsize;
        }

        /// <inheritdoc />
        public DataType DataType {
            readonly get => dt;
            set => dt = value;
        }

        /// <summary>
        /// Returns true if this pointer is to be regarded as a null pointer.
        /// </summary>
        public readonly bool IsNull => ToLinear() == 0;

        /// <summary>
        /// Returns true if this pointer is to be regarded as a null pointer.
        /// </summary>
        public readonly bool IsZero => ToLinear() == 0;

        /// <summary>
        /// Returns the offset part of this address.
        /// </summary>
        public readonly ulong Offset => uOffset;

        /// <summary>
        /// If this is a segmented address, returns the segment selector. If this is a
        /// linear address, returns null.
        /// </summary>
        public readonly ushort? Selector => GetInfo().SelectorShift != 0 ? this.selector : null;

        /// <summary>
        /// Adds a signed offset to this address.
        /// </summary>
        /// <param name="offset">Offset to add.</param>
        /// <returns>The new address.</returns>
        public readonly Address Add(long offset) => GetInfo().Add(this, offset);

        /// <inheritdoc />
        public readonly void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitAddress(this);
        }

        /// <inheritdoc />
        public readonly T Accept<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitAddress(this);
        }

        /// <inheritdoc />
        public readonly T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitAddress(this, context);
        }

        /// <summary>
        /// Align the address to the specified alignment. If the offset is not aligned
        /// the result will have an offset set to the nexteven multiple of the alignment.
        /// </summary>
        /// <param name="alignment">Requested alignment.</param>
        /// <returns>Resulting aligned address.</returns>
        public readonly Address Align(int alignment)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(alignment, nameof(alignment));
            var uAl = (uint) alignment;
            return Info.NewOffset(this, uAl * ((this.uOffset + uAl - 1) / uAl));
        }

        /// <inheritdoc/>
        public readonly Expression CloneExpression()
        {
            return this;
        }

        /// <inheritdoc/>
        public readonly int CompareTo(Address that)
        {
            return this.ToLinear().CompareTo(that.ToLinear());
        }

        readonly int IComparable.CompareTo(object? obj)
        {
            if (obj is not Address that)
                return 1;
            return this.CompareTo(that);
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not Address that)
                return false;
            return this.ToLinear() == that.ToLinear();
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return this.ToLinear().GetHashCode();
        }

        /// <summary>
        /// Converts a <see cref="Constant"/> to a linear <see cref="Address"/>.
        /// </summary>
        /// <param name="value">Constant value to convert.</param>
        /// <returns>Resulting address.</returns>
        public static Address FromConstant(Constant value)
        {
            int bitSize = value.DataType.BitSize;
            var dt = PrimitiveType.Create(Domain.Pointer, bitSize);
            return new Address(dt, LinearHex, (byte)bitSize, value.ToUInt64(), 0);
        }

        /// <summary>
        /// Generate a string representation of the address.
        /// </summary>
        /// <param name="prefix">Prefix to prepend before the numeric part of the address.</param>
        /// <param name="suffix">Suffix to append after the numeric part of the address.</param>
        /// <returns>A string representation.</returns>
        public readonly string GenerateName(string prefix, string suffix) => GetInfo().GenerateName(prefix, suffix, this);

        private readonly Info GetInfo() => infos[this.type];

        /// <inheritdoc/>
        public readonly Expression Invert()
        {
            throw new NotSupportedException($"Expression of type {GetType().Name} doesn't support Invert.");
        }

        /// <summary>
        /// Creates a new address with the same seletor (if one is present)
        /// but with a different offset.
        /// </summary>
        /// <param name="newOffset">The new offset to use.</param>
        /// <returns>A new <see cref="Address"/> instance that uses the new
        /// offset.
        /// </returns>
        public readonly Address NewOffset(ulong newOffset) => Info.NewOffset(this, newOffset);

        /// <summary>
        /// Preferred numeric base when rendering the address as text.
        /// </summary>
        public readonly int PreferredBase => this.type == LinearOctal ? 8 : 0;

        readonly void MachineOperand.Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.BeginOperand();
            renderer.WriteAddress(this.ToString()!, this);
            renderer.EndOperand();
        }

        /// <summary>
        /// Converts this address to a <see cref="Constant"/> value.
        /// </summary>
        /// <returns>The resulting constant.</returns>
        public readonly Constant ToConstant()
        {
            return Constant.Create(this.dt, this.ToLinear());
        }

        /// <summary>
        /// Converts this address to a 16-bit unsigned integer.
        /// </summary>
        public readonly ushort ToUInt16()
        {
            if (this.offsetBitsize > 16)
                throw new InvalidOperationException("Returning UInt16 would lose precision.");
            return (ushort) this.uOffset;
        }

        /// <summary>
        /// Converts this address to a 32-bit unsigned integer.
        /// </summary>
        public readonly uint ToUInt32()
        {
            if (this.offsetBitsize > 32)
                throw new InvalidOperationException("Returning UInt32 would lose precision.");
            return (uint) this.uOffset;
        }

        /// <summary>
        /// Converts an address to a linear value.
        /// </summary>
        /// <remarks>
        /// If the address is "flat", this is a trivial operation.
        /// If the address is segmented, various tricks need to be
        /// applied to generate a "flat" linear value.
        /// </remarks>
        /// <returns>An unsigned long value.</returns>
        public readonly ulong ToLinear() => GetInfo().ToLinear(this);

        /// <inheritdoc/>
        public override readonly string ToString() => GetInfo().ConvertToString(this);
        
        readonly string MachineOperand.ToString(MachineInstructionRendererOptions options)
        {
            return ToString();
        }

        /// <summary>
        /// Tests if two addresses are equal.
        /// </summary>
        /// <param name="a">First address.</param>
        /// <param name="b">Second address.</param>
        /// <returns>Returns true if the addresses are equal.</returns>
        public static bool operator ==(Address a, Address b)
        {
            return a.ToLinear() == b.ToLinear();
        }

        /// <summary>
        /// Tests if two addresses are not equal.
        /// </summary>
        /// <param name="a">First address.</param>
        /// <param name="b">Second address.</param>
        /// <returns>Returns true if the addresses are not equal.</returns>
        public static bool operator !=(Address a, Address b)
        {
            return a.ToLinear() != b.ToLinear();
        }

        /// <summary>
        /// Tests if address <paramref name="a"/> is strictly less than
        /// <paramref name="b"/>.
        /// </summary>
        /// <param name="a">First address.</param>
        /// <param name="b">Second address.</param>
        /// <returns>Returns true if the <paramref name="a"/> is strictly less
        /// than <paramref name="b"/>.</returns>
        public static bool operator <(Address a, Address b)
        {
            return a.ToLinear() < b.ToLinear();
        }

        /// <summary>
        /// Tests if address <paramref name="a"/> is less than
        /// or equal to <paramref name="b"/>.
        /// </summary>
        /// <param name="a">First address.</param>
        /// <param name="b">Second address.</param>
        /// <returns>Returns true if the <paramref name="a"/> is less
        /// than or equal to <paramref name="b"/>.</returns>
        public static bool operator <=(Address a, Address b)
        {
            return a.ToLinear() <= b.ToLinear();
        }

        /// <summary>
        /// Tests if address <paramref name="a"/> is strictly greater than
        /// <paramref name="b"/>.
        /// </summary>
        /// <param name="a">First address.</param>
        /// <param name="b">Second address.</param>
        /// <returns>Returns true if the <paramref name="a"/> is strictly greater
        /// than <paramref name="b"/>.</returns>
        public static bool operator >(Address a, Address b)
        {
            return a.ToLinear() > b.ToLinear();
        }

        /// <summary>
        /// Tests if address <paramref name="a"/> is greater than or equal to
        /// <paramref name="b"/>.
        /// </summary>
        /// <param name="a">First address.</param>
        /// <param name="b">Second address.</param>
        /// <returns>Returns true if the <paramref name="a"/> is greater
        /// than or equal to <paramref name="b"/>.</returns>
        public static bool operator >=(Address a, Address b)
        {
            return a.ToLinear() >= b.ToLinear();
        }

        /// <summary>
        /// Adds an unsigned offset to an address.
        /// </summary>
        /// <param name="a">Augend.</param>
        /// <param name="off">Unsigned offset.</param>
        /// <returns>The new address.</returns>
        public static Address operator +(Address a, ulong off)
        {
            return a.Add((long) off);
        }

        /// <summary>
        /// Adds an signed offset to an address.
        /// </summary>
        /// <param name="a">Augend.</param>
        /// <param name="off">Signed offset.</param>
        /// <returns>The new address.</returns>
        public static Address operator +(Address a, long off)
        {
            return a.Add(off);
        }

        /// <summary>
        /// Subtracts an signed offset from an address.
        /// </summary>
        /// <param name="a">Minuend.</param>
        /// <param name="off">Signed offset.</param>
        /// <returns>The new address.</returns>
        public static Address operator -(Address a, long off)
        {
            return a.Add(-off);
        }

        /// <summary>
        /// Computes the signed difference between two addresses.
        /// </summary>
        /// <param name="a">Minuend.</param>
        /// <param name="b">Subtrahend.</param>
        /// <returns>The difference between the addresses.</returns>
        public static long operator -(Address a, Address b)
        {
            return (long) a.ToLinear() - (long) b.ToLinear();
        }

        /// <summary>
        /// Returns the larger of two <see cref="Address">Addresses</see>.
        /// </summary>
        /// <param name="a">The first address to compare.</param>
        /// <param name="b">The second address to compare.</param>
        /// <returns>Address <paramref name="a"/> or <paramref name="b"/>,
        /// whichever is larger.
        /// </returns>
        public static Address Max(Address a, Address b)
        {
            if (a.ToLinear() >= b.ToLinear())
                return a;
            else
                return b;
        }

        /// <summary>
        /// Returns the lesser of two <see cref="Address">Addresses</see>.
        /// </summary>
        /// <param name="a">The first address to compare.</param>
        /// <param name="b">The second address to compare.</param>
        /// <returns>Address <paramref name="a"/> or <paramref name="b"/>,
        /// whichever is smaller.
        /// </returns>
        public static Address Min(Address a, Address b)
        {
            if (a.ToLinear() <= b.ToLinear())
                return a;
            else
                return b;
        }

        /// <summary>
        /// Create a 16-bit linear address.
        /// </summary>
        /// <param name="uAddr">Unsigned value.</param>
        /// <returns>A 16-bit linear address.</returns>

        public static Address Ptr16(ushort uAddr)
        {
            return new Address(PrimitiveType.Ptr16, LinearHex, 16, uAddr, 0);
        }

        /// <summary>
        /// Create a 32-bit linear address.
        /// </summary>
        /// <param name="uAddr">32-bit numeric value.</param>
        /// <returns>A 32-bit linear address.</returns>
        public static Address Ptr32(uint uAddr)
        {
            return new Address(PrimitiveType.Ptr32, LinearHex, 32, uAddr, 0);
        }

        /// <summary>
        /// Create a 64-bit linear address.
        /// </summary>
        /// <param name="uAddr">16-bit numeric value.</param>
        /// <returns>A 64-bit linear address.</returns>
        public static Address Ptr64(ulong uAddr)
        {
            return new Address(PrimitiveType.Ptr64, LinearHex, 64, uAddr, 0);
        }

        /// <summary>
        /// Creates a real-mode segmented address.
        /// </summary>
        /// <param name="seg">Value of the selector of the address.</param>
        /// <param name="offset">Value of the offset of the address.</param>
        /// <returns>A 32-bit segmented address instance.</returns>
        /// <remarks>
        /// This address type is suitable for real-mode x86 programs.
        /// </remarks>
        public static Address SegPtr(ushort seg, uint offset)
        {
            return new Address(
                PrimitiveType.SegPtr32,
                SegmentedReal,
                16,
                offset,
                seg);
        }

        /// <summary>
        /// Creates a protected-mode segmented address.
        /// </summary>
        /// <param name="seg">Value of the selector of the address.</param>
        /// <param name="offset">Value of the offset of the address.</param>
        /// <returns>A 32-bit segmented address instance.</returns>
        /// <remarks>
        /// This address type is suitable for
        /// protected-mode x86 programs with segmented pointers.
        /// </remarks>
        public static Address ProtectedSegPtr(ushort seg, uint offset)
        {
            return new Address(
                PrimitiveType.SegPtr32,
                SegmentedProt,
                16,
                offset,
                seg);
        }

        /// <summary>
        /// Create a linear address based on the provided data type.
        /// </summary>
        /// <param name="dt">The data type wanted for the addresss.</param>
        /// <param name="offset">Numeric value of the address.</param>
        /// <returns>The resulting address.</returns>
        public static Address Create(DataType dt, ulong offset)
        {
            return new Address(dt, LinearHex, (byte) dt.BitSize, offset, 0);
        }

        /// <summary>
        /// Create a linear address based on the provided data type.
        /// The address will have a preference to be rendered in octal.
        /// </summary>
        /// <param name="dt">The data type wanted for the addresss.</param>
        /// <param name="offset">Numeric value of the address.</param>
        /// <returns>The resulting address.</returns>
        public static Address OctalPtr(DataType dt, ulong offset)
        {
            return new Address(dt, LinearOctal, (byte) dt.BitSize, offset, 0);
        }

        /// <summary>
        /// Converts a string representation of an address to a 16-bit Address.
        /// </summary>
        /// <param name="s">The string representation of the Address</param>
        /// <param name="result">The resulting <see cref="Address"/> if the string
        /// representation of the address is valid.</param>
        /// <returns>True if the parsing of the string succeeded; otherwise false.</returns>
        public static bool TryParse16(string? s, [MaybeNullWhen(false)] out Address result)
        {
            if (s is not null)
            {
                if (ushort.TryParse(s, NumberStyles.HexNumber, null, out var uAddr))
                {
                    result = Ptr16(uAddr);
                    return true;
                }
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Converts a string representation of an address to a 32-bit Address.
        /// </summary>
        /// <param name="s">The string representation of the Address</param>
        /// <param name="result">The resulting <see cref="Address"/> if the string
        /// representation of the address is valid.</param>
        /// <returns>True if the parsing of the string succeeded; otherwise false.</returns>
        public static bool TryParse32(string? s, [MaybeNullWhen(false)] out Address result)
        {
            if (s is not null)
            {
                if (uint.TryParse(s, NumberStyles.HexNumber, null, out var uAddr))
                {
                    result = Ptr32(uAddr);
                    return true;
                }
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Tries to parse a hexadecimal string representation of an address to a 64-bit address.
        /// </summary>
        /// <param name="s">Hexadecimal string.</param>
        /// <param name="result">The resulting 64-bit address</param>
        /// <returns>True if the provided string <paramref name="s"/> was a valid 
        /// hexadecimal string, false otherwise.
        /// </returns>
        public static bool TryParse64(string? s, [MaybeNullWhen(false)] out Address result)
        {
            if (s is not null)
            {
                if (ulong.TryParse(s, NumberStyles.HexNumber, null, out var uAddr))
                {
                    result = Ptr64(uAddr);
                    return true;
                }
            }
            result = default;
            return false;
        }

        /// <summary>
        /// <see cref="IEqualityComparer{T}" /> implementation to compare
        /// addresses for equality.
        /// </summary>
        public class Comparer : IEqualityComparer<Address>
        {
            /// <inheritdoc/>
            public bool Equals(Address x, Address y)
            {
                return x.ToLinear() == y.ToLinear();
            }

            /// <inheritdoc/>
            public int GetHashCode(Address obj)
            {
                return obj.ToLinear().GetHashCode();
            }
        }

        private static readonly Info[] infos =
        [
            new Info(0, 0, 0),
            new Info(16, 0, 0),
            new Info(8, 0, 0),
            new Info(16, 4, 0xFFFF),
            new Info(16, 9, 0xFFF8),
        ];

        private record Info(int Base, int SelectorShift, uint SelectorMask)
        {
            internal Address Add(in Address addr, long offset)
            {
                var lValue = (long) addr.uOffset + offset;
                return NewOffset(addr, (ulong) lValue);
            }

            public string ConvertToString(in Address addr)
            {
                return GenerateName("", ":", "", addr);
            }

            internal string GenerateName(string prefix, string suffix, in Address addr)
            {
                return GenerateName(prefix, "_", suffix, addr);
            }

            internal string GenerateName(string prefix, string segmentSeparator, string suffix, in Address addr)
            {
                const string hexdigits = "0123456789ABCDEF";

                var sb = new StringBuilder(prefix);
                int bits = addr.offsetBitsize;
                if (this.SelectorShift != 0)
                {
                    sb.AppendFormat("{0:X4}{1}", addr.selector, segmentSeparator);
                }
                int bitsPerDigit = this.Base == 8 ? 3 : 4;
                int mask = (int) Bits.Mask(0, bitsPerDigit);
                int digits = (bits + bitsPerDigit - 1) / bitsPerDigit;
                var offset = addr.uOffset;
                for (int i = digits - 1; i >= 0; --i)
                {
                    char ch = hexdigits[(int) (offset >> i * bitsPerDigit) & mask];
                    sb.Append(ch);
                }
                return sb.ToString();
            }

            public static Address NewOffset(in Address addr, ulong newOffset)
            {
                var mask = (~0ul >> (64 - addr.offsetBitsize));
                return new Address(addr.dt, addr.type, addr.offsetBitsize, newOffset & mask, addr.selector);
            }


            public ulong ToLinear(in Address address)
            {
                var result = ((address.selector & this.SelectorMask) << this.SelectorShift) + address.uOffset;
                return result;
            }
        }
    }

#pragma warning disable CS1591

    /// <summary>
    /// Experimental address representation.
    /// </summary>
    public struct CompactAddress : Expression, IComparable<CompactAddress>, IComparable,
        MachineOperand
    {
        // 63-bit offset                  |0  - 64bithex
        //  offset | offset size |x|0|0|1  - flat hex
        //  offset | offset size |x|0|1|1  - flat octal
        // segment | offset | offset size |x|1|0|1  - segmented
        // segment | offset | offset size |1|1|1|1  - protected mode
        private const uint TypeMask = 0b111;
        private const uint Type_LinearHex = 0b001;
        private const uint Type_LinearOctal = 0b011;
        private const uint Type_SegmentedReal = 0b101;
        private const uint Type_SegmentedProtected = 0b111;

        private const uint SizeMask = 0xFF;
        private const ulong SizeTypeMask = 0xFFFul;
        private const ulong SelectorSizeTypeMask = SelectorMask | SizeTypeMask;

        private const int SizeShift = 4;
        private const int LinearSize = 36;
        private const int OffsetShift = 12;
        private const int SelectorShift = 48;
        private const ulong SelectorMask = 0xFFFF_0000_0000_0000ul;

        private static readonly Info[] Infos =
{
            new Info(1, 1, 16, 0, 0, 0),
            new Info(OffsetShift, SizeTypeMask, 16, 0, 0, 0),
            new Info(1, 63, 16, 0, 0, 0),
            new Info(OffsetShift, SizeTypeMask, 8, 0, 0, 0),

            new Info(1, 63, 16, 0, 0, 0),
            new Info(OffsetShift, SelectorSizeTypeMask, 16, 44, SelectorMask, SelectorMask),
            new Info(1, 63, 16, 0, 0, 0),
            new Info(OffsetShift, SelectorSizeTypeMask, 16, 39, SelectorMask, 0xFFF8_0000_0000_0000),
        };

        /// <summary>
        /// Low-level information used to unpack the encoded representation
        /// of an address.
        /// </summary>
        /// <param name="BitOffset"></param>
        /// <param name="NonOffsetMask"></param>
        /// <param name="Base"></param>
        /// <param name="LinearizeSelectorShift"></param>
        /// <param name="SelectorMask"></param>
        /// <param name="LinearizeSelectorMask"></param>
        private readonly record struct Info(
            int BitOffset,
            ulong NonOffsetMask,
            int Base,
            int LinearizeSelectorShift,
            ulong SelectorMask,
            ulong LinearizeSelectorMask)
        {
            private const string hexdigits = "0123456789ABCDEF";

            public ulong OffsetMask { get; } = ~NonOffsetMask;

            internal ulong Offset(ulong uValue)
            {
                return (ulong)((long)(uValue & ~SelectorMask) >> this.BitOffset); 
            }

            private int OffsetBitSize(ulong uValue)
            {
                int bits = this.BitOffset < 4
                    ? 64
                    : (byte) (uValue >> SizeShift);
                return bits;
            }

            internal ushort? Selector(ulong uValue)
            {
                if (SelectorMask == 0)
                    return null;
                return (ushort) (uValue >> CompactAddress.SelectorShift);
            }

            internal bool IsZero(ulong uValue)
            {
                return (uValue >> this.BitOffset) == 0;
            }

            internal CompactAddress NewOffset(in CompactAddress addr, ulong newOffset)
            {
                var sizeType = addr.uValue & SizeMask;
                ulong newValue = 
                    (newOffset << this.BitOffset) & OffsetMask |
                    (addr.uValue & this.NonOffsetMask);
                return new CompactAddress(addr.DataType, newValue);
            }

            internal string GenerateName(string prefix, string suffix, ulong uValue)
            {
                return GenerateName(prefix, "_", suffix, uValue);
            }

            internal string GenerateName(string prefix, string segmentSeparator, string suffix, ulong uValue)
            {
                var sb = new StringBuilder(prefix);
                int bits = this.BitOffset < 4
                    ? 64
                    : (byte) (uValue >> SizeShift);
                var sel = Selector(uValue);
                if (sel.HasValue)
                {
                    sb.AppendFormat("{0:X4}", sel.Value);
                    bits -= 16;
                    sb.Append(segmentSeparator);
                }
                int bitsPerDigit = this.Base == 8 ? 3 : 4;
                int mask = (int) Bits.Mask(0, bitsPerDigit);
                int digits = (bits + bitsPerDigit - 1) / bitsPerDigit;
                var offset = this.Offset(uValue);
                for (int i = digits - 1; i >= 0; --i)
                {
                    char ch = hexdigits[(int) (offset >> i * bitsPerDigit) & mask];
                    sb.Append(ch);
                }
                return sb.ToString();
            }

            internal CompactAddress Add(in CompactAddress addr, long offset)
            {
                var lValue = (long)Offset(addr.uValue) + offset;
                return NewOffset(addr, (ulong)lValue);
            }

            internal CompactAddress Align(in CompactAddress addr, int alignment)
            {
                if (alignment <= 0)
                    throw new ArgumentOutOfRangeException(nameof(alignment));
                var uAl = (uint) alignment;
                return NewOffset(addr, uAl * ((this.Offset(addr.uValue) + uAl - 1) / uAl));
            }

            internal string ConvertToString(ulong uValue)
            {
                return GenerateName("", ":", "", uValue);
            }

            internal Constant ToConstant(DataType dt, ulong uValue)
            {
                return Constant.Create(dt, ToLinear(uValue));
            }

            internal ushort ToUInt16(ulong uValue)
            {
                if (this.OffsetBitSize(uValue) > 16)
                    throw new InvalidOperationException("Returning UInt16 would lose precision.");
                return (ushort) this.Offset(uValue);
            }

            internal uint ToUInt32(ulong uValue)
            {
                if (this.OffsetBitSize(uValue) > 32)
                    throw new InvalidOperationException("Returning UInt32 would lose precision.");
                return (uint) this.Offset(uValue);
            }

            internal ulong ToLinear(ulong uValue)
            {
                var linearizedSelector = (uValue & this.LinearizeSelectorMask) >> LinearizeSelectorShift;
                var offset = Offset(uValue);
                return linearizedSelector + offset;
            }

            private ulong ExposeShiftedOffset(ulong uValue)
            {
                return (this.BitOffset < 2)
                    ? uValue
                    : (uValue & ~CompactAddress.SizeTypeMask) << (52 - (int)((uValue >> SizeShift) & 0xFF));
            }
        }

        private readonly ulong uValue;

        private CompactAddress(DataType size, ulong uValue)
        {
            this.DataType = size;
            this.uValue = uValue;
        }

        /// <summary>
        /// Create a linear <see cref="CompactAddress"/> of the type <paramref name="dtAddress"/>
        /// from the raw bit pattern <paramref name="linearAddress"/>.
        /// </summary>
        /// <param name="dtAddress">Data type that determines the size of the address.</param>
        /// <param name="linearAddress">The value of the address.</param>
        /// <returns>An <see cref="CompactAddress"/> instance of the requested size.
        /// </returns>
        public static CompactAddress Create(DataType dtAddress, ulong linearAddress)
        {
            var bitSize = dtAddress.BitSize;
            if (bitSize >= 63)
            {
                return new CompactAddress(dtAddress, (linearAddress << 1));
            }
            return new CompactAddress(
                dtAddress,
                (linearAddress << OffsetShift) | ((uint) bitSize << SizeShift) | Type_LinearHex);
        }

        public static CompactAddress OctalPtr(DataType dtAddress, ulong bitPattern)
        {
            return new CompactAddress(
                dtAddress,
                (bitPattern << OffsetShift) | ((uint) dtAddress.BitSize << SizeShift) | Type_LinearOctal);
        }

        /// <summary>
        /// Creates a 16-bit linear address.
        /// </summary>
        /// <param name="uAddr">Value of the address.</param>
        /// <returns>A 16-bit address instance.</returns>
        public static CompactAddress Ptr16(ushort uAddr)
        {
            return Create(PrimitiveType.Ptr16, uAddr);
        }

        /// <summary>
        /// Creates a 32-bit linear address.
        /// </summary>
        /// <param name="uAddr">Value of the address.</param>
        /// <returns>A 32-bit address instance.</returns>
        public static CompactAddress Ptr32(uint uAddr)
        {
            return Create(PrimitiveType.Ptr32, uAddr);
        }

        /// <summary>
        /// Creates a 64-bit linear address.
        /// </summary>
        /// <param name="uAddr">Value of the address.</param>
        /// <returns>A 64-bit address instance.</returns>
        public static CompactAddress Ptr64(ulong uAddr)
        {
            return Create(PrimitiveType.Ptr64, uAddr);
        }

        /// <summary>
        /// Creates a real-mode segmented address.
        /// </summary>
        /// <param name="seg">Value of the selector of the address.</param>
        /// <param name="offset">Value of the offset of the address.</param>
        /// <returns>A 32-bit segmented address instance.</returns>
        /// <remarks>
        /// This address type is suitable for
        /// real-mode x86 programs.
        /// </remarks>
        public static CompactAddress SegPtr(ushort seg, uint offset)
        {
            return new CompactAddress(
                PrimitiveType.SegPtr32,
                ((ulong) seg << SelectorShift) | ((ulong) offset << OffsetShift) |
                ((uint) 32u << SizeShift) | Type_SegmentedReal);
        }

        /// <summary>
        /// Creates a protected-mode segmented address.
        /// </summary>
        /// <param name="seg">Value of the selector of the address.</param>
        /// <param name="offset">Value of the offset of the address.</param>
        /// <returns>A 32-bit segmented address instance.</returns>
        /// <remarks>
        /// This address type is suitable for
        /// protected-mode x86 programs with segmented pointers.
        /// </remarks>
        public static CompactAddress ProtectedSegPtr(ushort seg, uint offset)
        {
            return new CompactAddress(
                PrimitiveType.SegPtr32,
                ((ulong) seg << SelectorShift) | ((ulong) offset << OffsetShift) |
                ((uint) 32u << SizeShift) | Type_SegmentedProtected);
        }

        /// <inheritdoc />
        public DataType DataType { get; set; }

        /// <summary>
        /// Returns the larger of two <see cref="CompactAddress">Addresses</see>.
        /// </summary>
        /// <param name="a">The first address to compare.</param>
        /// <param name="b">The second address to compare.</param>
        /// <returns>Address <paramref name="a"/> or <paramref name="b"/>,
        /// whichever is larger.
        /// </returns>
        public static CompactAddress Max(CompactAddress a, CompactAddress b)
        {
            if (a.ToLinear() >= b.ToLinear())
                return a;
            else
                return b;
        }

        /// <summary>
        /// Returns the lesser of two <see cref="CompactAddress">Addresses</see>.
        /// </summary>
        /// <param name="a">The first address to compare.</param>
        /// <param name="b">The second address to compare.</param>
        /// <returns>Address <paramref name="a"/> or <paramref name="b"/>,
        /// whichever is smaller.
        /// </returns>
        public static CompactAddress Min(CompactAddress a, CompactAddress b)
        {
            if (a.ToLinear() <= b.ToLinear())
                return a;
            else
                return b;
        }

        public static CompactAddress FromConstant(Constant value)
        {
            int bitSize = value.DataType.BitSize;
            var dt = PrimitiveType.Create(Domain.Pointer, bitSize);
            return Create(dt, value.ToUInt64());
        }

        /// <inheritdoc/>
        public readonly IEnumerable<Expression> Children => Array.Empty<Expression>();

        public readonly bool IsNull => GetInfo().ToLinear(uValue) == 0;
        public bool IsZero => GetInfo().IsZero(uValue);
        public ulong Offset => GetInfo().Offset(uValue);


        /// <summary>
        /// If this is a segmented address, returns the segment selector. If this is a 
        /// linear address, returns null.
        /// </summary>
        public ushort? Selector => GetInfo().Selector(uValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly Info GetInfo()
        {
            return Infos[this.uValue & TypeMask];
        }

        /// <summary>
        /// Creates an address with same selector, but a different offset.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns>An address whose offset is <paramref name="offset"/>, and whose selector is the same
        /// as the original address.
        /// </returns>
        public CompactAddress NewOffset(ulong offset) => GetInfo().NewOffset(this, offset);

        public T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            throw new NotImplementedException("return v.VisitAddress(this, context);");
        }

        public T Accept<T>(ExpressionVisitor<T> v)
        {
            throw new NotImplementedException("return v.VisitAddress(this);");
        }

        public void Accept(IExpressionVisitor visit)
        {
            throw new NotImplementedException("v.VisitAddress(this);");
        }

        public Expression CloneExpression()
        {
            return new CompactAddress(this.DataType, this.uValue);
        }

        public override bool Equals(object? obj)
        {
            return CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return ToLinear().GetHashCode();
        }

        public string GenerateName(string prefix, string suffix) => GetInfo().GenerateName(prefix, suffix, this.uValue);

        public Expression Invert()
        {
            throw new NotSupportedException($"Expression of type {GetType().Name} doesn't support Invert.");
        }

        public static bool operator ==(CompactAddress a, CompactAddress b)
        {
            return a.ToLinear() == b.ToLinear();
        }

        public static bool operator !=(CompactAddress a, CompactAddress b)
        {
            return a.ToLinear() != b.ToLinear();
        }

        public static bool operator < (CompactAddress a, CompactAddress b)
		{
			return a.ToLinear() < b.ToLinear();
		}

		public static bool operator <= (CompactAddress a, CompactAddress b)
		{
			return a.ToLinear() <= b.ToLinear();
		}

		public static bool operator > (CompactAddress a, CompactAddress b)
		{
			return a.ToLinear() > b.ToLinear();
		}

        public static bool operator >= (CompactAddress a, CompactAddress b)
		{
			return a.ToLinear() >= b.ToLinear();
		}

        public static CompactAddress operator + (CompactAddress a, ulong off)
		{
			return a.Add((long) off);
		}

		public static CompactAddress operator + (CompactAddress a, long off)
		{
            return a.Add(off);
        }

        public CompactAddress Add(long offset) => GetInfo().Add(this, offset);

        public CompactAddress Align(int alignment) => GetInfo().Align(this, alignment);

        public static CompactAddress operator - (CompactAddress a, long delta)
		{
			return a.Add(-delta);
		}

        public static long operator - (CompactAddress a, CompactAddress b)
		{
			return (long) a.ToLinear() - (long) b.ToLinear();
		}

        public int CompareTo(CompactAddress that)
        {
            return this.ToLinear().CompareTo(that.ToLinear());
        }

		public int CompareTo(object? a)
		{
            if (a is not CompactAddress that)
                return 1;
            return this.ToLinear().CompareTo(that.ToLinear());
		}

        public static int Compare(CompactAddress a, CompactAddress b)
        {
            return a.CompareTo(b);
        }

        void MachineOperand.Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.BeginOperand();
          //  renderer.WriteAddress(this.ToString()!, this);
            renderer.EndOperand();
        }

        public override string ToString() => GetInfo().ConvertToString(this.uValue);

        string MachineOperand.ToString(MachineInstructionRendererOptions options)
        {
            return GetInfo().ConvertToString(this.uValue)!;
        }

        public Constant ToConstant() => GetInfo().ToConstant(this.DataType, uValue);
        public ushort ToUInt16() => GetInfo().ToUInt16(uValue);
        public uint ToUInt32() => GetInfo().ToUInt32(uValue);
        public ulong ToLinear() => GetInfo().ToLinear(uValue);

        /// <summary>
        /// Converts a hexadecimal string representation of an address to a 16-bit Address.
        /// </summary>
        /// <param name="s">The string representation of the Address</param>
        /// <param name="result">Resulting address, if the string representation is valid.</param>
		/// <returns>True if the string is a valid address representation; otherwise false.</returns>
        public static bool TryParse16(string? s, [MaybeNullWhen(false)] out CompactAddress result)
        {
            if (s is not null)
            {
                if (ushort.TryParse(s, NumberStyles.HexNumber, null, out var uAddr))
                {
                    result = Ptr16(uAddr);
                    return true;
                }
            }
            result = default;
            return false;
        }

        public static bool TryParse32(string? s, [MaybeNullWhen(false)] out CompactAddress result)
        {
            if (s is not null)
            {
                if (uint.TryParse(s, NumberStyles.HexNumber, null, out var uAddr))
                {
                    result = Ptr32(uAddr);
                    return true;
                }
            }
            result = default;
            return false;
        }

        public static bool TryParse64(string? s, [MaybeNullWhen(false)] out CompactAddress result)
        {
            if (s is not null)
            {
                if (ulong.TryParse(s, NumberStyles.HexNumber, null, out var uAddr))
                {
                    result = Ptr64(uAddr);
                    return true;
                }
            }
            result = default;
            return false;
        }

        /// <summary>
        /// <see cref="IEqualityComparer{T}" /> implementation to compare
        /// addresses for equality.
        /// </summary>
        public class Comparer : IEqualityComparer<CompactAddress>
        {
            /// <inheritdoc />
            public bool Equals(CompactAddress x, CompactAddress y)
            {
                return x.ToLinear() == y.ToLinear();
            }

            /// <inheritdoc />
            public int GetHashCode(CompactAddress obj)
            {
                return obj.ToLinear().GetHashCode();
            }
        }
    }
/*
    internal class Address16 : CompactAddress
    {
        private readonly ushort uValue;

        public Address16(ushort addr)
            : base(PrimitiveType.Ptr16)
        {
            this.uValue = addr;
        }

        public override bool IsNull { get { return uValue == 0; } }
        public override ulong Offset { get { return uValue; } }
        public override ushort? Selector { get { return null; } }
        
        public override Address Add(long offset)
        {
            return new Address16((ushort)((int)uValue + (int)offset));
        }

        public override Address Align(int alignment)
        {
            return new Address16((ushort)(alignment * ((uValue + alignment - 1) / alignment)));
        }

        public override Expression CloneExpression()
        {
            return new Address16(uValue);
        }

        public override string GenerateName(string prefix, string suffix)
        {
            return string.Format("{0}{1:X4}{2}", prefix, uValue, suffix);
        }

        public override Address NewOffset(ulong offset)
        {
            return new Address16((ushort)offset);
        }

        public override Constant ToConstant()
        {
            return Constant.UInt16(uValue);
        }

        public override ushort ToUInt16()
        {
            return uValue;
        }

        public override uint ToUInt32()
        {
            return uValue;
        }

        public override ulong ToLinear()
        {
            return uValue;
        }

        protected override string ConvertToString()
        {
            return $"{uValue:X4}";
        }
    }

    public class Address32 : Address
    {
        private readonly uint uValue;
		public static readonly Address NULL = Address32.Ptr32(0);

        public Address32(uint addr)
            : this(addr, PrimitiveType.Ptr32)
        {
        }

        public Address32(uint addr, DataType ptrType)
            : base(ptrType)
        {
            this.uValue = addr;
        }

        public override bool IsNull { get { return uValue == 0; } }
        public override ulong Offset { get { return uValue; } }
        public override ushort? Selector { get { return null; } }

        public override Address Add(long offset)
        {
            var uNew = uValue + offset;
            return new Address32((uint)uNew);
        }

        public override Address Align(int alignment)
        {
            return new Address32((uint)(alignment * ((uValue + alignment - 1) / alignment)));
        }

        public override Expression CloneExpression()
        {
            return new Address32(uValue, this.DataType);
        }

        public override string GenerateName(string prefix, string suffix)
        {
            return string.Format("{0}{1:X8}{2}", prefix, uValue, suffix);
        }

        public override Address NewOffset(ulong offset)
        {
            return new Address32((uint) offset);
        }

        public override Constant ToConstant()
        {
            return Constant.UInt32(uValue);
        }

        public override ushort ToUInt16()
        {
            throw new InvalidOperationException("Returning UInt16 would lose precision.");
        }

        public override uint ToUInt32()
        {
            return uValue;
        }

        public override ulong ToLinear()
        {
            return uValue;
        }

        protected override string ConvertToString()
        {
            return $"{uValue:X8}";
        }
    }

    public class RealSegmentedAddress : Address
    {
        private readonly ushort uSegment;
        private readonly ushort uOffset;

        public RealSegmentedAddress(ushort segment, ushort offset)
            : base(PrimitiveType.SegPtr32)
        {
            this.uSegment = segment;
            this.uOffset = offset;
        }

        public override bool IsNull { get { return uSegment == 0 && uOffset == 0; } }
        public override ulong Offset { get { return uOffset; } }
        public override ushort? Selector { get { return uSegment; } }

        public override Address Add(long offset)
        {
            ushort sel = this.uSegment;
			uint newOff = (uint) (uOffset + offset);
			if (newOff > 0xFFFF)
			{
                sel += (ushort)((newOff & ~0xFFFFu) >> 4);
				newOff &= 0xFFFF;
			}
			return new RealSegmentedAddress(sel, (ushort) newOff);
		}

        public override Address Align(int alignment)
        {
            return new RealSegmentedAddress(uSegment, ((ushort)(alignment * ((uOffset + alignment - 1) / alignment))));
        }

        public override Expression CloneExpression()
        {
            return new RealSegmentedAddress(uSegment, uOffset);
        }

        public override string GenerateName(string prefix, string suffix)
        {
            return string.Format("{0}{1:X4}_{2:X4}{3}", prefix, uSegment, uOffset, suffix);
        }

        public override Address NewOffset(ulong offset)
        {
            return new RealSegmentedAddress(uSegment, (ushort)offset);
        }

        public override ushort ToUInt16()
        {
            throw new InvalidOperationException("Returning UInt16 would lose precision.");
        }

        public override Constant ToConstant()
        {
            return Constant.UInt32(ToUInt32());
        }

        public override uint ToUInt32()
        {
            return (((uint)uSegment) << 4) + uOffset;
        }

        public override ulong ToLinear()
        {
            return (((ulong)uSegment) << 4) + uOffset;
        }

        protected override string ConvertToString()
        {
            return $"{uSegment:X4}:{uOffset:X4}";
        }
    }

    /// <summary>
    /// Implements an Intel 80286+ protected segmented address.
    /// </summary>
    /// <remarks>
    /// Starting with the 80386, the offset could be 32-bit. It's possible 
    /// we may need a separate ProtectedSegmentedAddress48 class for such
    /// "long" segmented addresses. See discussion in #498.
    /// </remarks>
    public class ProtectedSegmentedAddress : Address
    {
        private readonly ushort uSelector;
        private readonly ushort uOffset;

        public ProtectedSegmentedAddress(ushort segment, ushort offset)
            : base(PrimitiveType.SegPtr32)
        {
            this.uSelector = segment;
            this.uOffset = offset;
        }

        public override bool IsNull { get { return uSelector == 0 && uOffset == 0; } }
        public override ulong Offset { get { return uOffset; } }
        public override ushort? Selector { get { return uSelector; } }

        public override Address Add(long offset)
        {
            ushort sel = this.uSelector;
            // As per discussion in #498, we allow the offset to overflow
            // quietly.
            ushort newOff = (ushort)(uOffset + offset);
            return new ProtectedSegmentedAddress(sel, newOff);
        }

        public override Address Align(int alignment)
        {
            return new ProtectedSegmentedAddress(uSelector, ((ushort)(alignment * ((uOffset + alignment - 1) / alignment))));
        }

        public override Expression CloneExpression()
        {
            return new ProtectedSegmentedAddress(uSelector, uOffset);
        }

        public override string GenerateName(string prefix, string suffix)
        {
            return string.Format("{0}{1:X4}_{2:X4}{3}", prefix, uSelector, uOffset, suffix);
        }

        public override Address NewOffset(ulong offset)
        {
            return new ProtectedSegmentedAddress(uSelector, (ushort)offset);
        }

        public override Constant ToConstant()
        {
            return Constant.UInt32(ToUInt32());
        }

        public override ushort ToUInt16()
        {
            throw new InvalidOperationException("Returning UInt16 would lose precision.");
        }

        public override uint ToUInt32()
        {
            return (((uint)(uSelector & ~7)) << 9) + uOffset;
        }

        public override ulong ToLinear()
        {
            return (((ulong)(uSelector & ~7)) << 9) + uOffset;
        }

        protected override string ConvertToString()
        {
            return $"{uSelector:X4}:{uOffset:X4}";
        }
    }

    public class Address64 : Address
    {
        private readonly ulong uValue;
		public static readonly Address NULL = Address32.Ptr64(0);

		public Address64(ulong addr)
            : base(PrimitiveType.Ptr64)
        {
            this.uValue = addr;
        }

        public override bool IsNull { get { return uValue == 0; } }
        public override ulong Offset { get { return uValue; } }
        public override ushort? Selector { get { return null; } }

        public override Address Add(long offset)
        {
            return new Address64(uValue + (ulong)offset);
        }

        public override Address Align(int alignment)
        {
            if (alignment <= 0)
                throw new ArgumentOutOfRangeException(nameof(alignment));
            var uAl = (uint)alignment;
            return new Address64(uAl * ((uValue + uAl - 1) / uAl));
        }

        public override Expression CloneExpression()
        {
            return new Address64(uValue);
        }

        public override string GenerateName(string prefix, string suffix)
        {
            return string.Format("{0}{1:X16}{2}", prefix, uValue, suffix);
        }

        public override Address NewOffset(ulong offset)
        {
            return new Address64(offset);
        }

        public override Constant ToConstant()
        {
            return Constant.UInt64(uValue);
        }

        public override ushort ToUInt16()
        {
            throw new InvalidOperationException("Returning UInt16 would lose precision.");
        }

        public override uint ToUInt32()
        {
            throw new InvalidOperationException("Returning UInt32 would lose precision.");
        }

        public override ulong ToLinear()
        {
            return uValue;
        }

        protected override string ConvertToString()
        {
            return $"{uValue:X16}";
        }
    }
*/
}
