#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Memory
{
    public class Word16BeImageReader : Word16ImageReader, EndianImageReader
    {
        public Word16BeImageReader(Word16MemoryArea mem, long offset)
            : base(mem, offset, mem.Words.Length)
        {
        }

        public Word16BeImageReader(Word16MemoryArea mem, Address addr)
            : base(mem, addr - mem.BaseAddress)
        {
        }

        public Word16BeImageReader(Word16MemoryArea mem, long offset, long endOffset) 
            : base(mem, offset, endOffset)
        {
        }

        public EndianImageReader Clone()
        {
            return new Word16BeImageReader(this.mem, this.Offset);
        }

        public EndianImageReader CreateNew(MemoryArea image, Address addr)
        {
            return new Word16BeImageReader((Word16MemoryArea) image, addr);
        }

        public StringConstant ReadCString(DataType charType, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public short ReadInt16() => ReadBeInt16();
        public int ReadInt32() => ReadBeInt32();
        public long ReadInt64() => ReadBeInt64();

        public bool ReadNullCharTerminator(DataType dtChar)
        {
            throw new NotImplementedException();
        }

        public ushort ReadUInt16() => ReadBeUInt16();
        public uint ReadUInt32() => ReadUInt32();
        public ulong ReadUInt64() => ReadUInt64();

        public bool TryPeekUInt32(int offset, out uint value) => TryPeekBeUInt32(offset, out value);

        public bool TryRead(PrimitiveType dataType, out Constant value) => TryReadBe(dataType, out value);

        public bool TryReadInt16(out short value) => TryReadBeInt16(out value);
        public bool TryReadInt32(out int value) => TryReadBeInt32(out value);
        public bool TryReadInt64(out long value) => TryReadBeInt64(out value);
        public bool TryReadUInt16(out ushort value) => TryReadBeUInt16(out value);
        public bool TryReadUInt32(out uint value) => TryReadBeUInt32(out value);
        public bool TryReadUInt64(out ulong value) => TryReadBeUInt64(out value);
    }
}
