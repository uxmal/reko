#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Core.Memory
{
    public class Word16LeImageReader : Word16ImageReader, EndianImageReader
    {
        public Word16LeImageReader(Word16MemoryArea mem, long offset)
            : base(mem, offset, mem.Words.Length)
        {
        }

        public Word16LeImageReader(Word16MemoryArea mem, Address addr)
            : base(mem, addr - mem.BaseAddress)
        {
        }

        public Word16LeImageReader(Word16MemoryArea mem, Address addr, long cUnits)
            : base(mem, addr - mem.BaseAddress, (addr - mem.BaseAddress) + cUnits)
        {
        }

        public Word16LeImageReader(Word16MemoryArea mem, long offset, long endOffset) 
            : base(mem, offset, endOffset)
        {
        }

        public EndianImageReader Clone()
        {
            return new Word16LeImageReader(this.mem, this.Offset);
        }

        public EndianImageReader CreateNew(MemoryArea image, Address addr)
        {
            return new Word16LeImageReader((Word16MemoryArea) image, addr);
        }

        public string ReadNulTerminatedString(DataType charType, Encoding enc)
        {
            throw new NotImplementedException();
        }

        public StringConstant ReadCString(DataType charType, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public short ReadInt16() => ReadLeInt16();
        public int ReadInt32() => ReadLeInt32();
        public long ReadInt64() => ReadLeInt64();

        public bool ReadNullCharTerminator(DataType dtChar)
        {
            throw new NotImplementedException();
        }

        public ushort ReadUInt16() => ReadLeUInt16();
        public uint ReadUInt32() => ReadLeUInt32();
        public ulong ReadUInt64() => ReadLeUInt64();

        public bool TryPeekUInt32(int offset, out uint value) => TryPeekLeUInt32(offset, out value);
        public bool TryPeekUInt64(int offset, out ulong value) => TryPeekLeUInt64(offset, out value);

        public bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant value) => TryReadLe(dataType, out value);

        public bool TryReadInt16(out short value) => TryReadLeInt16(out value);
        public bool TryReadInt32(out int value) => TryReadLeInt32(out value);
        public bool TryReadInt64(out long value) => TryReadLeInt64(out value);
        public bool TryReadUInt16(out ushort value) => TryReadLeUInt16(out value);
        public bool TryReadUInt32(out uint value) => TryReadLeUInt32(out value);
        public bool TryReadUInt64(out ulong value) => TryReadLeUInt64(out value);
    }
}
