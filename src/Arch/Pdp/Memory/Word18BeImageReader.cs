#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Text;

namespace Reko.Arch.Pdp.Memory
{
    public class Word18BeImageReader : Word18ImageReader, EndianImageReader
    {
        public Word18BeImageReader(Word18MemoryArea mem, long offset)
            : base(mem, offset, mem.Words.Length)
        {
        }

        public Word18BeImageReader(Word18MemoryArea mem, Address addr)
            : base(mem, addr - mem.BaseAddress)
        {
        }

        public Word18BeImageReader(Word18MemoryArea mem, Address addr, long cUnits)
            : base(mem, addr - mem.BaseAddress, addr - mem.BaseAddress + cUnits)
        {
        }

        public Word18BeImageReader(Word18MemoryArea mem, long offset, long endOffset)
            : base(mem, offset, endOffset)
        {
        }

        public EndianImageReader Clone()
        {
            return new Word18BeImageReader(mem, Offset);
        }

        public EndianImageReader CreateNew(MemoryArea image, Address addr)
        {
            return new Word18BeImageReader((Word18MemoryArea) image, addr);
        }

        public StringConstant ReadCString(DataType charType, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public short ReadInt16() => throw new NotSupportedException();
        public int ReadInt32() => throw new NotSupportedException();
        public long ReadInt64() => throw new NotSupportedException();

        public bool ReadNullCharTerminator(DataType dtChar)
        {
            throw new NotImplementedException();
        }

        public ushort ReadUInt16() => throw new NotSupportedException();
        public uint ReadUInt32() => throw new NotSupportedException();
        public ulong ReadUInt64() => throw new NotSupportedException();

        public bool TryPeekUInt32(int offset, out uint value) => throw new NotSupportedException();
        public bool TryPeekUInt64(int offset, out ulong value) => throw new NotSupportedException();

        public bool TryRead(PrimitiveType dataType, out Constant value) => TryReadBe(dataType, out value);

        public bool TryReadInt16(out short value) => throw new NotSupportedException();
        public bool TryReadInt32(out int value) => throw new NotSupportedException();
        public bool TryReadInt64(out long value) => throw new NotSupportedException();
        public bool TryReadUInt16(out ushort value) => throw new NotSupportedException();
        public bool TryReadUInt32(out uint value)
        {
            value = 0;
            return false;
        }
        public bool TryReadUInt64(out ulong value) => throw new NotSupportedException();
    }
}
