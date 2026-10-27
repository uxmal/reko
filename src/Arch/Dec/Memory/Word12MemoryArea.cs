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
        throw new NotImplementedException();
    }

    public override EndianImageReader CreateBeReader(long offsetBegin, long offsetEnd)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
