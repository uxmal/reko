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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.IO;

namespace Reko.Arch.Pdp.Memory
{
    /// <summary>
    /// An <see cref="ImageReader"/> that reads 36-bit granular data from a <see cref="Word18MemoryArea"/>.
    /// </summary>
    public class Word18ImageReader : ImageReader
    {
        protected readonly Word18MemoryArea mem;
        private readonly long endOffset;

        public Word18ImageReader(Word18MemoryArea mem, long beginOffset)
            : this(mem, beginOffset, mem.Words.Length)
        {
        }

        public Word18ImageReader(Word18MemoryArea mem, long beginOffset, long endOffset)
        {
            this.mem = mem;
            Offset = beginOffset;
            this.endOffset = endOffset;
        }

        public Address Address => mem.BaseAddress + Offset;

        public int CellBitSize => 36;

        public bool IsValid => (ulong) Offset < (ulong) endOffset;

        public long Offset { get; set; }


        public BinaryReader CreateBinaryReader()
        {
            throw new NotImplementedException();
        }

        public bool IsValidOffset(long offset)
        {
            return (ulong) (Offset + offset) < (ulong) endOffset;
        }

        public short PeekBeInt16(int offset)
        {
            return (short) mem.Words[Offset + offset];
        }

        public int PeekBeInt32(int offset)
        {
            return (int) mem.ReadBeUInt32(Offset + offset);
        }

        public uint PeekBeUInt32(int offset) => mem.ReadBeUInt32(Offset + offset);

        public ulong PeekBeUInt64(int offset) => mem.ReadBeUInt64(Offset + offset);

        public byte PeekByte(int offset)
        {
            throw new NotSupportedException();
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
            // Byte accesses aren't supported.
            throw new NotSupportedException();
        }

        public short ReadBeInt16() => (short) ReadBeUInt16();
        public int ReadBeInt32() => (int) ReadBeUInt32();
        public long ReadBeInt64() => (long) ReadBeUInt64();

        public ushort ReadBeUInt16()
        {
            throw new NotSupportedException();
        }

        public uint ReadBeUInt32()
        {
            throw new NotSupportedException();
        }

        public ulong ReadBeUInt64()
        {
            throw new NotSupportedException();
        }

        public byte ReadByte()
        {
            // Byte accesses aren't supported.
            throw new NotSupportedException();
        }

        public byte[] ReadBytes(int addressUnits)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadBytes(uint addressUnits)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadToEnd()
        {
            return ReadBytes((int) (endOffset - Offset));
        }

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
            throw new NotSupportedException();
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

        public bool TryReadBe(DataType dataType, out Constant value)
        {
            switch (dataType.BitSize)
            {
            case 18:
                if (!TryReadBeUInt18(out uint w18))
                    break;
                value = Constant.Create(PdpTypes.Word18, w18);
                return true;
            default:
                throw new NotImplementedException($"Reading data type {dataType} not implemented yet.");
            }
            value = null!;
            return false;
        }

        public bool TryReadBeInt16(out short value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadBeInt32(out int value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadBeInt64(out long value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadBeUInt16(out ushort value)
        {
            throw new NotSupportedException();
        }

        public bool TryReadBeUInt32(out uint value)
        {
            throw new NotSupportedException();
        }

        public bool TryReadBeUInt18(out uint value)
        {
            if (!IsValidOffset(0))
            {
                value = 0;
                return false;
            }
            value = mem.Words[Offset];
            ++Offset;
            return true;
        }

        public bool TryReadBeUInt64(out ulong value)
        {
            throw new NotSupportedException();
        }

        public bool TryReadByte(out byte value)
        {
            throw new NotImplementedException();
        }

        public bool TryReadLe(DataType dataType, out Constant value)
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
