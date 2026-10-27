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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Reko.Arch.Dec.Memory;

public class Word12BeImageReader : EndianImageReader
{
    private readonly Word12MemoryArea mem;
    private readonly long endOffset;

    public Word12BeImageReader(Word12MemoryArea mem, long offset)
        : this(mem, offset, mem.Words.Length)
    {
    }

    public Word12BeImageReader(Word12MemoryArea mem, long offset, long endOffset)
    { 
        this.mem = mem;
        this.Offset = offset;
        this.endOffset = Math.Min(mem.Words.Length, endOffset);
    }

    public Word12BeImageReader(Word12MemoryArea mem, Address addr, long cUnits)
        : this(mem, addr - mem.BaseAddress, addr - mem.BaseAddress + cUnits)
    {
    }


    public Address Address => mem.BaseAddress + Offset;

    public int CellBitSize => 12;

    public bool IsValid => (ulong) Offset < (ulong) endOffset;

    public long Offset { get; set; }


    public EndianImageReader Clone()
    {
        return new Word12BeImageReader(mem, Offset);
    }

    public BinaryReader CreateBinaryReader()
    {
        throw new System.NotImplementedException();
    }

    public EndianImageReader CreateNew(MemoryArea image, Address addr)
    {
        return new Word12BeImageReader((Word12MemoryArea) image, addr - mem.BaseAddress);
    }

    public bool IsValidOffset(long offset)
    {
        return (ulong) (Offset + offset) < (ulong) endOffset;
    }

    public short PeekBeInt16(int offset)
    {
        throw new System.NotImplementedException();
    }

    public int PeekBeInt32(int offset)
    {
        throw new System.NotImplementedException();
    }

    public uint PeekBeUInt32(int offset)
    {
        throw new System.NotImplementedException();
    }

    public ulong PeekBeUInt64(int offset)
    {
        throw new System.NotImplementedException();
    }

    public byte PeekByte(int offset)
    {
        throw new System.NotImplementedException();
    }

    public short PeekLeInt16(int offset)
    {
        throw new System.NotImplementedException();
    }

    public int PeekLeInt32(int offset)
    {
        throw new System.NotImplementedException();
    }

    public ushort PeekLeUInt16(int offset)
    {
        throw new System.NotImplementedException();
    }

    public uint PeekLeUInt32(int offset)
    {
        throw new System.NotImplementedException();
    }

    public ulong PeekLeUInt64(int offset)
    {
        throw new System.NotImplementedException();
    }

    public sbyte PeekSByte(int offset)
    {
        throw new System.NotImplementedException();
    }

    public short ReadBeInt16()
    {
        throw new System.NotImplementedException();
    }

    public int ReadBeInt32()
    {
        throw new System.NotImplementedException();
    }

    public ushort ReadBeUInt16()
    {
        throw new System.NotImplementedException();
    }

    public uint ReadBeUInt32()
    {
        throw new System.NotImplementedException();
    }

    public ulong ReadBeUInt64()
    {
        throw new System.NotImplementedException();
    }

    public byte ReadByte()
    {
        throw new System.NotImplementedException();
    }

    public byte[] ReadBytes(int addressUnits)
    {
        throw new System.NotImplementedException();
    }

    public byte[] ReadBytes(uint addressUnits)
    {
        throw new System.NotImplementedException();
    }

    public StringConstant ReadCString(DataType charType, Encoding encoding)
    {
        throw new System.NotImplementedException();
    }

    public short ReadInt16()
    {
        throw new System.NotImplementedException();
    }

    public int ReadInt32()
    {
        throw new System.NotImplementedException();
    }

    public long ReadInt64()
    {
        throw new System.NotImplementedException();
    }

    public short ReadLeInt16()
    {
        throw new System.NotImplementedException();
    }

    public int ReadLeInt32()
    {
        throw new System.NotImplementedException();
    }

    public long ReadLeInt64()
    {
        throw new System.NotImplementedException();
    }

    public ushort ReadLeUInt16()
    {
        throw new System.NotImplementedException();
    }

    public uint ReadLeUInt32()
    {
        throw new System.NotImplementedException();
    }

    public ulong ReadLeUInt64()
    {
        throw new System.NotImplementedException();
    }

    public bool ReadNullCharTerminator(DataType dtChar)
    {
        throw new System.NotImplementedException();
    }

    public string ReadNulTerminatedString(DataType charType, Encoding encoding)
    {
        throw new System.NotImplementedException();
    }

    public sbyte ReadSByte()
    {
        throw new System.NotImplementedException();
    }

    public byte[] ReadToEnd()
    {
        throw new System.NotImplementedException();
    }

    public ushort ReadUInt16()
    {
        throw new System.NotImplementedException();
    }

    public uint ReadUInt32()
    {
        throw new System.NotImplementedException();
    }

    public ulong ReadUInt64()
    {
        throw new System.NotImplementedException();
    }

    public long Seek(long offset, SeekOrigin origin = SeekOrigin.Current)
    {
        throw new System.NotImplementedException();
    }

    public bool TryPeekBeUInt16(int offset, out ushort value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryPeekBeUInt32(int offset, out uint value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryPeekBeUInt64(int offset, out ulong value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryPeekByte(int offset, out byte value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryPeekLeUInt16(int offset, out ushort value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryPeekLeUInt32(int offset, out uint value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryPeekUInt32(int offset, out uint value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryPeekUInt64(int offset, out ulong value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant value)
    {
        if (dataType.BitSize > 16)
        {
            value = null;
            return false;
        }
        var n = mem.Words[Offset++];
        value = Constant.Create(dataType, n);
        return true;
    }

    public bool TryReadBe(DataType dataType, [MaybeNullWhen(false)] out Constant value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadBeInt16(out short value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadBeInt32(out int value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadBeUInt16(out ushort value)
    {
        if (!this.IsValid)
        {
            value = 0;
            return false;
        }
        value = mem.Words[Offset];
        ++Offset;
        return true;
    }

    public bool TryReadBeUInt32(out uint value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadByte(out byte value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadInt16(out short value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadInt32(out int value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadInt64(out long value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadLe(DataType dataType, [MaybeNullWhen(false)] out Constant value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadLeInt16(out short value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadLeSigned(DataType dataType, out long value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadLeUInt16(out ushort value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadLeUInt32(out uint value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadLeUInt64(out ulong value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadUInt16(out ushort value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryReadUInt32(out uint value)
    {
        value = 0;
        return false;
    }

    public bool TryReadUInt64(out ulong value)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Read at most <paramref name="cUnits"/> words 
    /// from the underlying memory area, starting at the current offset.
    /// </summary>
    /// <param name="cUnits">Number of words to read.</param>
    /// <returns>An array of words read from the memory.</returns>
    public ushort[] ReadWords(int cUnits)
    {
        int cRead = Math.Min(cUnits, (int) (endOffset - Offset));
        cRead = Math.Max(cRead, 0);
        var words = new ushort[cRead];
        for (int i = 0; i < words.Length; ++i)
        {
            words[i] = mem.Words[Offset++];
        }
        return words;
    }
}