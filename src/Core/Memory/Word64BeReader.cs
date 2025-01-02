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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Reko.Core.Memory
{
    public class Word64BeReader : Word64ImageReader, EndianImageReader
    {
        public Word64BeReader(Word64MemoryArea mem) : base(mem, 0)
        {
        }

        public Word64BeReader(Word64MemoryArea mem, long offset) : base(mem, offset)
        {
        }

        public Word64BeReader(Word64MemoryArea mem, Address addr) : base(mem, addr)
        {
        }

        public Word64BeReader(Word64MemoryArea mem, Address addr, long cUnits) : base(mem, addr, cUnits)
        {
        }
        
        public EndianImageReader Clone()
        {
            return new Word64BeReader(mem, Offset);
        }

        public EndianImageReader CreateNew(MemoryArea image, Address addr)
        {
            throw new System.NotImplementedException();
        }

        public string ReadNulTerminatedString(DataType charType, Encoding enc)
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

        public bool ReadNullCharTerminator(DataType dtChar)
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

        public bool TryPeekUInt32(int offset, out uint value)
        {
            throw new System.NotImplementedException();
        }

        public bool TryPeekUInt64(int offset, out ulong value) => TryPeekBeUInt64(offset, out value);

        public bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant value)
        {
            value = default!;
            return false;
        }

        public bool TryReadInt16(out short _) => throw new System.NotImplementedException();

        public bool TryReadInt32(out int value)
        {
            throw new System.NotImplementedException();
        }

        public bool TryReadInt64(out long value)
        {
            throw new System.NotImplementedException();
        }

        public bool TryReadUInt16(out ushort value)
        {
            throw new System.NotImplementedException();
        }

        public bool TryReadUInt32(out uint value)
        {
            if (!mem.TryReadBeUInt32(Offset, out value))
                return false;
            Offset += 1;
            return true;
        }

        public bool TryReadUInt64(out ulong value)
        {
            throw new System.NotImplementedException();
        }
    }
}