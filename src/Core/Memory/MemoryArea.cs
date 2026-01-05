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

using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Core.Memory
{
    /// <summary>
    /// This class abstracts the notion of a memory area, consisting of a
    /// number of addressable memory cells at a particular address. Note that
    /// the memory cells can be of any size; they are not restricted to being
    /// 8-bit bytes. 
    /// </summary>
    /// <remarks>
    /// Most modern CPU architectures are byte-addressable, and should use
    /// <see cref="ByteMemoryArea"/>. However, other architectures (like
    /// PDP-10, Cray, MIL-SPEC-1750) are word-addressable, and need other
    /// implementations of <see cref="MemoryArea"/> for their specific word-
    /// sizes.
    /// </remarks>
    public abstract class MemoryArea
    {
        /// <summary>
        /// Initializes a memory area.
        /// </summary>
        /// <param name="addrBase">Address of the start of the memory area.</param>
        /// <param name="length">The length of the area.</param>
        /// <param name="cellBitSize">Size of individual storage units.</param>
        /// <param name="formatter">Preferred formatter to use when displaying memory.
        /// </param>
        protected MemoryArea(Address addrBase, int length, int cellBitSize, MemoryFormatter formatter)
        {
            this.BaseAddress = addrBase;
            this.Length = length;
            this.CellBitSize = cellBitSize;
            this.Relocations = new RelocationDictionary();
            this.Formatter = formatter;
        }

        /// <summary>
        /// Starting address of the memory area.
        /// </summary>
        public Address BaseAddress { get; }

        /// <summary>
        /// The size of a memory cell measured in bits.
        /// </summary>
        public int CellBitSize { get; }

        /// <summary>
        /// The number of addressable units of the memory area.
        /// </summary>
        public long Length { get; }

        /// <summary>
        /// Formatter to use when rendering this memory area.
        /// </summary>
        public MemoryFormatter Formatter { get; set; }

        /// <summary>
        /// Relocations made in the memory area are tracked here..
        /// </summary>
        //$TODO: move to Program.
        public RelocationDictionary Relocations { get; }

        /// <summary>
        /// Creates a big-endian reader for the memory area. The reader will read
        /// starting at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address at which to start.</param>
        /// <returns>A big-endian reader.</returns>
        public abstract EndianImageReader CreateBeReader(Address addr);

        /// <summary>
        /// Creates a big-endian reader for the memory area. The reader will read
        /// starting at address <paramref name="addr"/>, up to <paramref name="cUnits"/>
        /// storage units.
        /// </summary>
        /// <param name="addr">Address at which to start.</param>
        /// <param name="cUnits">Maximal number of storage units to read.</param>
        /// <returns>A big-endian reader.</returns>
        public abstract EndianImageReader CreateBeReader(Address addr, long cUnits);

        /// <summary>
        /// Creates a big-endian reader for the memory area. The reader will read
        /// starting at offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset at which to start.</param>
        /// <returns>A big-endian reader.</returns>
        public abstract EndianImageReader CreateBeReader(long offset);

        /// <summary>
        /// Creates a big-endian reader for the memory area. The reader will read
        /// starting at offset <paramref name="offsetBegin"/>, and stop at offet
        /// <paramref name="offsetEnd"/>.
        /// </summary>
        /// <param name="offsetBegin">Offset at which to start.</param>
        /// <param name="offsetEnd">Offset at which to end.</param>
        /// <returns>A big-endian reader.</returns>
        public abstract EndianImageReader CreateBeReader(long offsetBegin, long offsetEnd);

        /// <summary>
        /// Creates a big-endian writer for the memory area. The writer will write
        /// starting at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address at which to start writing.</param>
        /// <returns>A big-endian writer.</returns>
        public abstract BeImageWriter CreateBeWriter(Address addr);

        /// <summary>
        /// Creates a big-endian writer for the memory area. The writer will write
        /// starting at offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Address at which to start writing.</param>
        /// <returns>A big-endian writer.</returns>
        public abstract BeImageWriter CreateBeWriter(long offset);


        /// <summary>
        /// Creates a little-endian reader for the memory area. The reader will read
        /// starting at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address at which to start.</param>
        /// <returns>A little-endian reader.</returns>
        public abstract EndianImageReader CreateLeReader(Address addr);

        /// <summary>
        /// Creates a little-endian reader for the memory area. The reader will read
        /// starting at address <paramref name="addr"/>, up to <paramref name="cUnits"/>
        /// storage units.
        /// </summary>
        /// <param name="addr">Address at which to start.</param>
        /// <param name="cUnits">Maximal number of storage units to read.</param>
        /// <returns>A little-endian reader.</returns>
        public abstract EndianImageReader CreateLeReader(Address addr, long cUnits);

        /// <summary>
        /// Creates a big-endian reader for the memory area. The reader will read
        /// starting at offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset at which to start.</param>
        /// <returns>A big-endian reader.</returns>
        public abstract EndianImageReader CreateLeReader(long offset);

        /// <summary>
        /// Creates a little-endian reader for the memory area. The reader will read
        /// starting at offset <paramref name="offsetBegin"/>, and stop at offet
        /// <paramref name="offsetEnd"/>.
        /// </summary>
        /// <param name="offsetBegin">Offset at which to start.</param>
        /// <param name="offsetEnd">Offset at which to end.</param>
        /// <returns>A little-endian reader.</returns>
        public abstract EndianImageReader CreateLeReader(long offsetBegin, long offsetEnd);

        /// <summary>
        /// Creates a little-endian writer for the memory area. The writer will write
        /// starting at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address at which to start writing.</param>
        /// <returns>A little-endian writer.</returns>
        public abstract LeImageWriter CreateLeWriter(Address addr);

        /// <summary>
        /// Creates a little-endian writer for the memory area. The writer will write
        /// starting at offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Address at which to start writing.</param>
        /// <returns>A little-endian writer.</returns>

        public abstract LeImageWriter CreateLeWriter(long offset);

        /// <summary>
        /// Determines whether the specified address is valid in this memory area.
        /// </summary>
        /// <param name="addr">Address to test.</param>
        /// <returns>True if the address is in the range of the memory area;
        /// otherwise false.</returns>
        public bool IsValidAddress(Address addr)
        {
            return IsValidLinearAddress(addr.ToLinear());
        }

        /// <summary>
        /// Determines whether the specified linear address is valid in this memory area.
        /// </summary>
        /// <param name="linearAddr">Linear address to test.</param>
        /// <returns>True if the linear address is in the range of the memory area;
        /// otherwise false.</returns>
        public bool IsValidLinearAddress(ulong linearAddr)
        {
            if (linearAddr < BaseAddress.ToLinear())
                return false;
            ulong offset = (linearAddr - BaseAddress.ToLinear());
            return offset < (ulong) Length;
        }

        /// <summary>
        /// Reads a constant from the memory area at the specified offset.
        /// </summary>
        /// <param name="imageOffset">Offset from which to read the constant.</param>
        /// <param name="type">Datatype to read.</param>
        /// <param name="c">Constant that was read.</param>
        /// <returns>True if the <paramref name="imageOffset"/> was valid and a constant could
        /// successfully be read; otherwise false.</returns>
        public abstract bool TryReadBe(long imageOffset, DataType type, [MaybeNullWhen(false)] out Constant c);

        /// <summary>
        /// Reads a big-endian constant from the memory area at the specified address.
        /// </summary>
        /// <param name="addr">Address from which to read the constant.</param>
        /// <param name="type">Datatype to read.</param>
        /// <param name="c">Constant that was read.</param>
        /// <returns>True if the address <paramref name="addr"/> was valid and a constant could
        /// successfully be read; otherwise false.</returns>
        public bool TryReadBe(Address addr, PrimitiveType type, [MaybeNullWhen(false)] out Constant c)
        {
            return TryReadBe(addr - BaseAddress, type, out c);
        }

        /// <summary>
        /// Reads a 64-bit big endian IEEE floating point number from the offset <paramref name="off"/>.
        /// </summary>
        /// <param name="off">Offset from which to read the 64-bit floating point number.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the offset is valid and a 64-bit value could be read; otherwise false.</returns>
        public bool TryReadBeDouble(long off, out double retvalue)
        {
            if (TryReadBeUInt64(off, out ulong uDouble))
            {
                retvalue = BitConverter.Int64BitsToDouble((long) uDouble);
                return true;
            }
            else
            {
                retvalue = 0.0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 16-bit big endian unsigned integer from offset <paramref name="off"/>.
        /// </summary>
        /// <param name="off">Offset from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the offset is valid and a value could be read; otherwise false.</returns>
        public abstract bool TryReadBeUInt16(long off, out ushort retvalue);

        /// <summary>
        /// Reads a 32-bit big endian unsigned integer from offset <paramref name="off"/>.
        /// </summary>
        /// <param name="off">Offset from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the offset is valid and a value could be read; otherwise false.</returns>
        public abstract bool TryReadBeUInt32(long off, out uint retvalue);

        /// <summary>
        /// Reads a 64-bit big endian unsigned integer from offset <paramref name="off"/>.
        /// </summary>
        /// <param name="off">Offset from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the offset is valid and a value could be read; otherwise false.</returns>
        public abstract bool TryReadBeUInt64(long off, out ulong retvalue);


        /// <summary>
        /// Reads a 16-bit big endian unsigned integer from address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the address is valid and a value could be read; otherwise false.</returns>
        public bool TryReadBeUInt16(Address addr, out ushort retvalue) => TryReadBeUInt16(addr - BaseAddress, out retvalue);

        /// <summary>
        /// Reads a 32-bit big endian unsigned integer from address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the address is valid and a value could be read; otherwise false.</returns>
        public bool TryReadBeUInt32(Address addr, out uint retvalue) => TryReadBeUInt32(addr - BaseAddress, out retvalue);

        /// <summary>
        /// Reads a 64-bit big endian unsigned integer from address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the address is valid and a value could be read; otherwise false.</returns>
        public bool TryReadBeUInt64(Address addr, out ulong retvalue) => TryReadBeUInt64(addr - BaseAddress, out retvalue);

        /// <summary>
        /// Reads a little endian constant from the memory area at the specified offset.
        /// </summary>
        /// <param name="imageOffset">Offset from which to read the constant.</param>
        /// <param name="type">Datatype to read.</param>
        /// <param name="c">Constant that was read.</param>
        /// <returns>True if the address <paramref name="imageOffset"/> was valid and a constant could
        /// successfully be read; otherwise false.</returns>
        public abstract bool TryReadLe(long imageOffset, DataType type, [MaybeNullWhen(false)] out Constant c);

        /// <summary>
        /// Reads a little endian constant from the memory area at the specified address.
        /// </summary>
        /// <param name="addr">Offset from which to read the constant.</param>
        /// <param name="type">Datatype to read.</param>
        /// <param name="c">Constant that was read.</param>
        /// <returns>True if the <paramref name="addr"/> was valid and a constant could
        /// successfully be read; otherwise false.</returns>
        public bool TryReadLe(Address addr, PrimitiveType type, [MaybeNullWhen(false)] out Constant c)
        {
            return TryReadLe(addr - BaseAddress, type, out c);
        }

        /// <summary>
        /// Reads a 64-bit little endian IEEE floating point number from the offset <paramref name="off"/>.
        /// </summary>
        /// <param name="off">Offset from which to read the 64-bit floating point number.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the offset is valid and a 64-bit value could be read; otherwise false.</returns>
        public bool TryReadLeDouble(long off, out double retvalue)
        {
            if (TryReadLeUInt64(off, out ulong uDouble))
            {
                retvalue = BitConverter.Int64BitsToDouble((long) uDouble);
                return true;
            }
            else
            {
                retvalue = 0.0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 32-bit little endian signed integer from offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the address is valid and a value could be read; otherwise false.</returns>
        public abstract bool TryReadLeInt32(long offset, out int retvalue);

        /// <summary>
        /// Reads a 16-bit little endian unsigned integer from offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the address is valid and a value could be read; otherwise false.</returns>
        public abstract bool TryReadLeUInt16(long offset, out ushort retvalue);

        /// <summary>
        /// Reads a 32-bit little endian unsigned integer from offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the address is valid and a value could be read; otherwise false.</returns>
        public abstract bool TryReadLeUInt32(long offset, out uint retvalue);

        /// <summary>
        /// Reads a 64-bit little endian unsigned integer from offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the address is valid and a value could be read; otherwise false.</returns>
        public abstract bool TryReadLeUInt64(long offset, out ulong retvalue);


        /// <summary>
        /// Reads a 16-bit little endian unsigned integer from address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the address is valid and a value could be read; otherwise false.</returns>
        public bool TryReadLeUInt16(Address addr, out ushort retvalue) => TryReadLeUInt16(addr - BaseAddress, out retvalue);

        /// <summary>
        /// Reads a 32-bit little endian unsigned integer from address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the address is valid and a value could be read; otherwise false.</returns>
        public bool TryReadLeUInt32(Address addr, out uint retvalue) => TryReadLeUInt32(addr - BaseAddress, out retvalue);

        /// <summary>
        /// Reads a 64-bit little endian unsigned integer from address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address from which to read the value.</param>
        /// <param name="retvalue">The value read.</param>
        /// <returns>True of the address is valid and a value could be read; otherwise false.</returns>
        public bool TryReadLeUInt64(Address addr, out ulong retvalue) => TryReadLeUInt64(addr - BaseAddress, out retvalue);
        
        /// <summary>
        /// Reads a byte from the offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset from which to read one byte.</param>
        /// <param name="b">The byte that was read.</param>
        /// <returns>True if the offset is a valid position; otherwise false.</returns>
        public abstract bool TryReadByte(long offset, out byte b);


        /// <summary>
        /// Writes a byte at the specified offset.
        /// </summary>
        /// <param name="off">Offset to write the byte at.</param>
        /// <param name="value">Value to write.</param>
        public abstract void WriteByte(long off, byte value);

        /// <summary>
        /// Writes a big-endian 16-bit value at the specified offset.
        /// </summary>
        /// <param name="off">Offset to write the byte at.</param>
        /// <param name="value">Value to write.</param>
        public abstract void WriteBeUInt16(long off, ushort value);

        /// <summary>
        /// Writes a big-endian 32-bit value at the specified offset.
        /// </summary>
        /// <param name="off">Offset to write the byte at.</param>
        /// <param name="value">Value to write.</param>
        public abstract void WriteBeUInt32(long off, uint value);

        /// <summary>
        /// Writes a big-endian 64-bit value at the specified offset.
        /// </summary>
        /// <param name="off">Offset to write the byte at.</param>
        /// <param name="value">Value to write.</param>
        public abstract void WriteBeUInt64(long off, ulong value);

        /// <summary>
        /// Writes a little-endian 16-bit value at the specified offset.
        /// </summary>
        /// <param name="off">Offset to write the byte at.</param>
        /// <param name="value">Value to write.</param>
        public abstract void WriteLeUInt16(long off, ushort value);

        /// <summary>
        /// Writes a little-endian 32-bit value at the specified offset.
        /// </summary>
        /// <param name="off">Offset to write the byte at.</param>
        /// <param name="value">Value to write.</param>
        public abstract void WriteLeUInt32(long off, uint value);

        /// <summary>
        /// Writes a little-endian 64-bit value at the specified offset.
        /// </summary>
        /// <param name="off">Offset to write the byte at.</param>
        /// <param name="value">Value to write.</param>
        public abstract void WriteLeUInt64(long off, ulong value);

        /// <summary>
        /// Writes a little-endian 32-bit value at the specified address.
        /// </summary>
        /// <param name="address">Address at which to write the byte.</param>
        /// <param name="value">Value to write.</param>

        public void WriteLeUInt32(Address address, uint value) => WriteLeUInt32(address - BaseAddress, value);
    }
}
