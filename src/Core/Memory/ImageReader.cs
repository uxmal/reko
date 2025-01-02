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

        bool IsValid { get; }
        long Offset { get; set; }

        bool IsValidOffset(long offset);

        BinaryReader CreateBinaryReader();

        byte PeekByte(int offset);
        short PeekBeInt16(int offset);
        int PeekBeInt32(int offset);
        uint PeekBeUInt32(int offset);
        ulong PeekBeUInt64(int offset);

        short PeekLeInt16(int offset);
        int PeekLeInt32(int offset);

        ushort PeekLeUInt16(int offset);
        uint PeekLeUInt32(int offset);
        ulong PeekLeUInt64(int offset);
        sbyte PeekSByte(int offset);

        short ReadBeInt16();
        int ReadBeInt32();
        ushort ReadBeUInt16();

        uint ReadBeUInt32();
        ulong ReadBeUInt64();

        byte ReadByte();
        byte[] ReadBytes(int addressUnits);
        byte[] ReadBytes(uint addressUnits);
        byte[] ReadToEnd();

        short ReadLeInt16();
        int ReadLeInt32();
        long ReadLeInt64();
        ushort ReadLeUInt16();
        uint ReadLeUInt32();
        ulong ReadLeUInt64();
        sbyte ReadSByte();


        long Seek(long offset, SeekOrigin origin = SeekOrigin.Current);

        bool TryPeekByte(int offset, out byte value);
        bool TryPeekBeUInt16(int offset, out ushort value);
        bool TryPeekBeUInt32(int offset, out uint value);
        bool TryPeekBeUInt64(int offset, out ulong value);
        bool TryPeekLeUInt16(int offset, out ushort value);
        bool TryPeekLeUInt32(int offset, out uint value);

        bool TryReadBe(DataType dataType, [MaybeNullWhen(false)] out Constant value);
        bool TryReadBeInt16(out short value);
        bool TryReadBeInt32(out int value);
        bool TryReadBeUInt16(out ushort value);
        bool TryReadBeUInt32(out uint value);

        bool TryReadByte(out byte value);
        bool TryReadLe(DataType dataType, [MaybeNullWhen(false)] out Constant value);
        bool TryReadLeSigned(DataType dataType, out long value);

        bool TryReadLeInt16(out short value);

        bool TryReadLeUInt16(out ushort value);
        bool TryReadLeUInt32(out uint value);
        bool TryReadLeUInt64(out ulong value);
    }
}