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

using Reko.Arch.Dec.Output;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Reko.Arch.Dec.Memory;

/// <summary>
/// Represents a memory area of 12-bit words.
/// </summary>
public class Word12MemoryArea : MemoryArea
{
    public Word12MemoryArea(Address addr, ushort[] words)
        : base(addr, words.Length, 12, new Word12MemoryFormatter(8, 4, 2))
    {
        Words = words;
    }

    /// <summary>
    /// Creates a <see cref="Word12MemoryArea"/> from a byte array
    /// containing 12-bit words stored as 16-bit little-endian values.
    /// </summary>
    /// <param name="address">Base address of the resulting memory area.</param>
    /// <param name="bytes">Byte array containing 12-bit words stored as 16-bit little-endian values.</param>
    /// <returns>A <see cref="Word12MemoryArea"/> representing the specified bytes as 12-bit words.</returns>
    public static MemoryArea CreateFromLeBytes(Address address, byte[] bytes)
    {
        static ushort[] CreateFromLeBytes(byte[] bytes)
        {
            var words = new ushort[bytes.Length / 2];
            for (int i = 0; i < words.Length; ++i)
            {
                words[i] = (ushort) ((bytes[i * 2 + 1] << 8) | bytes[i * 2]);
            }
            return words;
        }
        var words = CreateFromLeBytes(bytes);
        return new Word12MemoryArea(address, words);
    }

    public ushort[] Words { get; }

    public override EndianImageReader CreateBeReader(Address addr)
    {
        long offset = addr - this.BaseAddress;
        return new Word12BeImageReader(this, offset);
    }

    public override EndianImageReader CreateBeReader(Address addr, long cUnits)
    {
        return new Word12BeImageReader(this, addr, cUnits);
    }

    public override EndianImageReader CreateBeReader(long offset)
    {
        return new Word12BeImageReader(this, offset);
    }

    public override EndianImageReader CreateBeReader(long offsetBegin, long offsetEnd)
    {
        return new Word12BeImageReader(this, offsetBegin, offsetEnd);
    }

    public override BeImageWriter CreateBeWriter(Address addr)
    {
        throw new NotImplementedException();
    }

    public override BeImageWriter CreateBeWriter(long offset)
    {
        throw new NotImplementedException();
    }

    public override EndianImageReader CreateLeReader(Address addr)
    {
        throw new NotImplementedException();
    }

    public override EndianImageReader CreateLeReader(Address addr, long cUnits)
    {
        throw new NotImplementedException();
    }

    public override EndianImageReader CreateLeReader(long offset)
    {
        return new Word12BeImageReader(this, offset);
    }

    public override EndianImageReader CreateLeReader(long offsetBegin, long offsetEnd)
    {
        throw new NotImplementedException();
    }

    public override LeImageWriter CreateLeWriter(Address addr)
    {
        throw new NotImplementedException();
    }

    public override LeImageWriter CreateLeWriter(long offset)
    {
        throw new NotImplementedException();
    }

    public override bool TryReadBe(long imageOffset, DataType type, [MaybeNullWhen(false)] out Constant c)
    {
        throw new NotImplementedException();
    }

    public override bool TryReadBeUInt16(long off, out ushort retvalue)
    {
        throw new NotImplementedException();
    }

    public override bool TryReadBeUInt32(long off, out uint retvalue)
    {
        throw new NotImplementedException();
    }

    public override bool TryReadBeUInt64(long off, out ulong retvalue)
    {
        throw new NotImplementedException();
    }

    public override bool TryReadByte(long offset, out byte b)
    {
        throw new NotImplementedException();
    }

    public override bool TryReadLe(long imageOffset, DataType type, [MaybeNullWhen(false)] out Constant c)
    {
        throw new NotImplementedException();
    }

    public override bool TryReadLeInt32(long offset, out int retvalue)
    {
        throw new NotImplementedException();
    }

    public override bool TryReadLeUInt16(long offset, out ushort retvalue)
    {
        throw new NotImplementedException();
    }

    public override bool TryReadLeUInt32(long offset, out uint retvalue)
    {
        throw new NotImplementedException();
    }

    public override bool TryReadLeUInt64(long offset, out ulong retvalue)
    {
        throw new NotImplementedException();
    }

    public override void WriteBeUInt16(long off, ushort value)
    {
        throw new NotImplementedException();
    }

    public override void WriteBeUInt32(long off, uint value)
    {
        throw new NotImplementedException();
    }

    public override void WriteBeUInt64(long off, ulong value)
    {
        throw new NotImplementedException();
    }

    public override void WriteByte(long off, byte value)
    {
        throw new NotImplementedException();
    }

    public override void WriteLeUInt16(long off, ushort value)
    {
        throw new NotImplementedException();
    }

    public override void WriteLeUInt32(long off, uint value)
    {
        throw new NotImplementedException();
    }

    public override void WriteLeUInt64(long off, ulong value)
    {
        throw new NotImplementedException();
    }
}
