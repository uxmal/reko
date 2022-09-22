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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Reko.Core.Memory
{
    public class Word64ImageReader : ImageReader
    {
        protected readonly Word64MemoryArea mem;
        private readonly long endOffset;

        public Word64ImageReader(Word64MemoryArea mem, long beginOffset)
            : this(mem, beginOffset, mem.Words.Length)
        {
        }

        public Word64ImageReader(Word64MemoryArea mem, Address addr)
            : this(mem, addr - mem.BaseAddress, mem.Words.Length)
        {
        }

        public Word64ImageReader(Word64MemoryArea mem, Address addr, long cUnits)
            : this(mem, addr - mem.BaseAddress, (addr - mem.BaseAddress) + cUnits)
        {
        }

        public Word64ImageReader(Word64MemoryArea mem, long beginOffset, long endOffset)
        {
            this.mem = mem;
            this.Offset = beginOffset;
            this.endOffset = endOffset;
        }

        public Address Address => mem.BaseAddress + Offset;

        public bool IsValid => (ulong) Offset < (ulong) endOffset; 

        public long Offset { get; set; }

        public Word64MemoryArea MemoryArea => mem;

        public BinaryReader CreateBinaryReader()
        {
            throw new NotImplementedException();
        }

        public bool IsValidOffset(long offset)
        {
            throw new NotImplementedException();
        }

        public short PeekBeInt16(int offset)
        {
            throw new NotImplementedException();
        }

        public int PeekBeInt32(int offset)
        {
            throw new NotImplementedException();
        }

        public uint PeekBeUInt32(int offset)
        {
            throw new NotImplementedException();
        }

        public ulong PeekBeUInt64(int offset)
        {
            throw new NotImplementedException();
        }

        public byte PeekByte(int offset)
        {
            throw new NotImplementedException();
        }

        public short PeekLeInt16(int offset)
        {
            throw new NotImplementedException();
        }

        public int PeekLeInt32(int offset)
        {
            throw new NotImplementedException();
        }

        public ushort PeekLeUInt16(int offset)
        {
            throw new NotImplementedException();
        }

        public uint PeekLeUInt32(int offset)
        {
            throw new NotImplementedException();
        }

        public ulong PeekLeUInt64(int offset)
        {
            throw new NotImplementedException();
        }

        public sbyte PeekSByte(int offset)
        {
            throw new NotImplementedException();
        }

        public short ReadBeInt16()
        {
            throw new NotImplementedException();
        }

        public int ReadBeInt32()
        {
            throw new NotImplementedException();
        }

        public ushort ReadBeUInt16()
        {
            throw new NotImplementedException();
        }

        public uint ReadBeUInt32()
        {
            throw new NotImplementedException();
        }

        public ulong ReadBeUInt64()
        {
            throw new NotImplementedException();
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public byte[] ReadBytes(int addressUnits)
        {
            var bytes = new byte[addressUnits * 8];
            var iEnd = Math.Min(Offset + addressUnits, endOffset);
            int iByte = 0;
            for (int iWord = (int) Offset; iWord < iEnd; ++iWord)
            {
                var w = mem.Words[iWord];
                bytes[iByte++] = (byte) (w >> 56);
                bytes[iByte++] = (byte) (w >> 48);
                bytes[iByte++] = (byte) (w >> 40);
                bytes[iByte++] = (byte) (w >> 32);
                bytes[iByte++] = (byte) (w >> 24);
                bytes[iByte++] = (byte) (w >> 16);
                bytes[iByte++] = (byte) (w >> 8);
                bytes[iByte++] = (byte) w;
            }
            return bytes;
        }

        public byte[] ReadBytes(uint addressUnits) => ReadBytes((int) addressUnits);

        public short ReadLeInt16()
        {
            throw new NotImplementedException();
        }

        public int ReadLeInt32()
        {
            throw new NotImplementedException();
        }

        public long ReadLeInt64()
        {
            throw new NotImplementedException();
        }

        public ushort ReadLeUInt16()
        {
            throw new NotImplementedException();
        }

        public uint ReadLeUInt32()
        {
            throw new NotImplementedException();
        }

        public ulong ReadLeUInt64()
        {
            throw new NotImplementedException();
        }

        public sbyte ReadSByte()
        {
            throw new NotImplementedException();
        }

        public long Seek(long offset, SeekOrigin origin = SeekOrigin.Current)
        {
            throw new NotImplementedException();
        }

        public bool TryPeekBeUInt16(int offset, out ushort value)
        {
            throw new NotImplementedException();
        }

        public bool TryPeekBeUInt32(int offset, out uint value)
        {
            throw new NotImplementedException();
        }

        public bool TryPeekBeUInt64(int offset, out ulong value)
        {
            throw new NotImplementedException();
        }

        public bool TryPeekByte(int offset, out byte value)
        {
            throw new NotImplementedException();
        }

        public bool TryPeekLeUInt16(int offset, out ushort value)
        {
            throw new NotImplementedException();
        }


        public bool TryPeekLeUInt32(int offset, out uint value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadBe(DataType dataType, [MaybeNullWhen(false)] out Constant value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadBeInt16(out short value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadBeInt32(out int value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadBeUInt16(out ushort value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadBeUInt32(out uint value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadByte(out byte value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadLe(DataType dataType, [MaybeNullWhen(false)] out Constant value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadLeInt16(out short value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadLeSigned(DataType dataType, out long value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadLeUInt16(out ushort value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadLeUInt32(out uint value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadLeUInt64(out ulong value)
        {
            throw new NotImplementedException();
        }
    }
}
