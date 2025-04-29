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

#pragma warning disable IDE1006

using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Reko.Core.Memory
{
    /// <summary>
    /// Implementers of this interface support reading of values from an area
    /// of memory.
    /// </summary>
    public interface ImageReader
    {
        /// <summary>
        /// The next address to be read.
        /// </summary>
        Address Address { get; }

        /// <summary>
        /// Size of an individual addressable cell, in bits.
        /// </summary>
        int CellBitSize { get; }

        /// <summary>
        /// True if the image reader is positioned such that it can read more data.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Offset of the next read operation..
        /// </summary>
        long Offset { get; set; }

        /// <summary>
        /// Determines if the offset is valid in the underlying memory area.
        /// </summary>
        /// <param name="offset">Offset whose validity needs to be determine.</param>
        /// <returns>
        /// True if the offset is valid in the underlying memory area.
        /// </returns>
        bool IsValidOffset(long offset);

        /// <summary>
        /// Creates a <see cref="BinaryReader"/> on the underlying data.
        /// </summary>
        BinaryReader CreateBinaryReader();

        /// <summary>
        /// Peek a byte at the specified offset. The offset is relative to the current 
        /// position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <returns>Peeked value.</returns>
        byte PeekByte(int offset);

        /// <summary>
        /// Peek a 16-bit big-endian signed integer at the specified offset. The offset is relative to the current 
        /// position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <returns>Peeked value.</returns>
        short PeekBeInt16(int offset);

        /// <summary>
        /// Peek a 32-bit big-endian signed integer at the specified offset. The offset is relative to the current 
        /// position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <returns>Peeked value.</returns>
        int PeekBeInt32(int offset);

        /// <summary>
        /// Peek a 32-bit big-endian unsigned integer at the specified offset. The offset is relative to the current 
        /// position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <returns>Peeked value.</returns>
        uint PeekBeUInt32(int offset);

        /// <summary>
        /// Peek a 64-bit big-endian unsigned integer at the specified offset. The offset is relative to the current 
        /// position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <returns>Peeked value.</returns>
        ulong PeekBeUInt64(int offset);

        /// <summary>
        /// Peek a 16-bit little-endian signed integer at the specified offset. The offset is relative to the current 
        /// position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <returns>Peeked value.</returns>
        short PeekLeInt16(int offset);

        /// <summary>
        /// Peek a 32-bit little-endian signed integer at the specified offset. The offset is relative to the current 
        /// position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <returns>Peeked value.</returns>
        int PeekLeInt32(int offset);

        /// <summary>
        /// Peek a 16-bit little-endian unsigned integer at the specified offset. The offset is relative to the current 
        /// position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <returns>Peeked value.</returns>
        ushort PeekLeUInt16(int offset);

        /// <summary>
        /// Peek a 32-bit little-endian unsigned integer at the specified offset. The offset is relative to the current 
        /// position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <returns>Peeked value.</returns>
        uint PeekLeUInt32(int offset);

        /// <summary>
        /// Peek a 64-bit little-endian unsigned integer at the specified offset. The offset is relative to the current 
        /// position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <returns>Peeked value.</returns>
        ulong PeekLeUInt64(int offset);

        /// <summary>
        /// Peek a signed byte at the specified offset. The offset is relative to the current 
        /// position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <returns>Peeked value.</returns>
        sbyte PeekSByte(int offset);

        /// <summary>
        /// Read a 16-bit big-endian signed integer at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        short ReadBeInt16();

        /// <summary>
        /// Read a 32-bit big-endian signed integer at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        int ReadBeInt32();

        /// <summary>
        /// Read a 16-bit big-endian unsigned integer at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        ushort ReadBeUInt16();

        /// <summary>
        /// Read a 32-bit big-endian unsigned integer at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        uint ReadBeUInt32();

        /// <summary>
        /// Read a 64-bit big-endian signed integer at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        ulong ReadBeUInt64();

        /// <summary>
        /// Read an octet at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        byte ReadByte();

        /// <summary>
        /// Read the specified number of bytes, starting at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="addressUnits"></param>
        /// <returns>An array of bytes.</returns>
        byte[] ReadBytes(int addressUnits);

        /// <summary>
        /// Read the specified number of bytes, starting at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="addressUnits"></param>
        /// <returns>An array of bytes.</returns>
        byte[] ReadBytes(uint addressUnits);

        /// <summary>
        /// Reads all bytes until the end of the memory area.
        /// </summary>
        /// <returns>An array of bytes.</returns>
        byte[] ReadToEnd();

        /// <summary>
        /// Read a 16-bit little-endian signed integer at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        short ReadLeInt16();

        /// <summary>
        /// Read a 32-bit little-endian signed integer at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        int ReadLeInt32();

        /// <summary>
        /// Read a 64-bit little-endian signed integer at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        long ReadLeInt64();

        /// <summary>
        /// Read a 16-bit little-endian unsigned integer at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        ushort ReadLeUInt16();

        /// <summary>
        /// Read a 32-bit little-endian unsigned integer at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        uint ReadLeUInt32();

        /// <summary>
        /// Read a 64-bit little-endian unsigned integer at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        ulong ReadLeUInt64();

        /// <summary>
        /// Read a signed octet at the current offset.
        /// After reading the value, the offset is incremented.
        /// </summary>
        /// <returns>The value read.</returns>
        sbyte ReadSByte();

        /// <summary>
        /// Change the current position of the image reader.
        /// </summary>
        /// <param name="offset">Offset from the origin.</param>
        /// <param name="origin">Seek origin to use.</param>
        /// <returns>The new offset.</returns>
        long Seek(long offset, SeekOrigin origin = SeekOrigin.Current);

        /// <summary>
        /// Peek an octet at the current offset.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <param name="value">Peeked value.</param>
        /// <returns>True if it was possible to peek a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryPeekByte(int offset, out byte value);

        /// <summary>
        /// Peek a 16-bit big-endian unsigned integer at the current offset.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <param name="value">Peeked value.</param>
        /// <returns>True if it was possible to peek a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryPeekBeUInt16(int offset, out ushort value);

        /// <summary>
        /// Peek a 32-bit big-endian unsigned integer at the current offset.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <param name="value">Peeked value.</param>
        /// <returns>True if it was possible to peek a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryPeekBeUInt32(int offset, out uint value);

        /// <summary>
        /// Peek a 64-bit big-endian unsigned integer at the current offset.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <param name="value">Peeked value.</param>
        /// <returns>True if it was possible to peek a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryPeekBeUInt64(int offset, out ulong value);

        /// <summary>
        /// Peek a 16-bit little-endian unsigned integer at the current offset.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <param name="value">Peeked value.</param>
        /// <returns>True if it was possible to peek a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryPeekLeUInt16(int offset, out ushort value);

        /// <summary>
        /// Peek a 32-bit little-endian unsigned integer at the current offset.
        /// </summary>
        /// <param name="offset">Offset from the current position.</param>
        /// <param name="value">Peeked value.</param>
        /// <returns>True if it was possible to peek a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryPeekLeUInt32(int offset, out uint value);

        /// <summary>
        /// Reads a big-endian value of the specified type from the current position of the 
        /// image reader. After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="dataType">Data type of the value to read</param>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadBe(DataType dataType, [MaybeNullWhen(false)] out Constant value);

        /// <summary>
        /// Reads a 16-bit big-endian signed integer from the current position of the 
        /// image reader. After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadBeInt16(out short value);

        /// <summary>
        /// Reads a 32-bit big-endian signed integer from the current position of the 
        /// image reader. After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadBeInt32(out int value);

        /// <summary>
        /// Reads a 16-bit big-endian unsigned integer from the current position of the 
        /// image reader. After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadBeUInt16(out ushort value);

        /// <summary>
        /// Reads a 32-bit big-endian unsigned integer from the current position of the 
        /// image reader. After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadBeUInt32(out uint value);

        /// <summary>
        /// Reads an octet from the current position of the 
        /// image reader. After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadByte(out byte value);

        /// <summary>
        /// Reads a little-endian value of the specified type from the current position of the 
        /// image reader.
        /// </summary>
        /// <param name="dataType">Data type of the value to read</param>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadLe(DataType dataType, [MaybeNullWhen(false)] out Constant value);

        /// <summary>
        /// Reads a little-endian integer of the specified type from the current position of the 
        /// image reader.
        /// </summary>
        /// <param name="dataType">Data type of the value to read</param>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadLeSigned(DataType dataType, out long value);

        /// <summary>
        /// Reads a 16-bit little-endian signed integer from the current position of the 
        /// image reader. After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadLeInt16(out short value);

        /// <summary>
        /// Reads a 16-bit little-endian unsigned integer from the current position of the 
        /// image reader. After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadLeUInt16(out ushort value);

        /// <summary>
        /// Reads a 32-bit little-endian unsigned integer from the current position of the 
        /// image reader. After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadLeUInt32(out uint value);

        /// <summary>
        /// Reads a 64-bit little-endian unsigned integer from the current position of the 
        /// image reader. After reading the value, the offset is incremented.
        /// </summary>
        /// <param name="value">Value that was read.</param>
        /// <returns>True if it was possible to read a value; false if 
        /// the offset is such that a value couldn't be peeked.</returns>
        bool TryReadLeUInt64(out ulong value);
    }
}