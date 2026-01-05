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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Reko.Core.Memory
{
    /// <summary>
    /// An <see cref="ImageReader"/> that reads 16-bit granular data from a <see cref="Word16MemoryArea"/>.
    /// </summary>
    public class Word16ImageReader : ImageReader
    {
        /// <summary>
        /// The memory area to read from.
        /// </summary>
        protected readonly Word16MemoryArea mem;
        private readonly long endOffset;

        /// <summary>
        /// Constructs a reader on a memory area, starting at the given offset.
        /// </summary>
        /// <param name="mem">A <see cref="Word64MemoryArea"/> to read from.</param>
        /// <param name="beginOffset">An offset within the words of the memory area.</param>
        public Word16ImageReader(Word16MemoryArea mem, long beginOffset)
            : this(mem, beginOffset, mem.Words.Length)
        {
        }

        /// <summary>
        /// Constructs a reader on the memory area, restricted to reading words
        /// between the two given offsets.
        /// </summary>
        /// <param name="mem">A <see cref="Word64MemoryArea"/> to read from.</param>
        /// <param name="beginOffset">Starting offset.</param>
        /// <param name="endOffset">Ending offset.</param>
        public Word16ImageReader(Word16MemoryArea mem, long beginOffset, long endOffset)
        {
            this.mem = mem;
            this.Offset = beginOffset;
            this.endOffset = endOffset;
        }

        /// <inheritdoc/>
        public Address Address => mem.BaseAddress + Offset;

        /// <inheritdoc/>
        public int CellBitSize => 16;

        /// <inheritdoc/>
        public bool IsValid => (ulong) Offset < (ulong) endOffset;

        /// <inheritdoc/>
        public long Offset { get; set; }


        /// <inheritdoc/>
        public BinaryReader CreateBinaryReader()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsValidOffset(long offset)
        {
            return (ulong) (Offset + offset) < (ulong) endOffset;
        }

        /// <inheritdoc/>
        public short PeekBeInt16(int offset)
        {
            return (short) mem.Words[Offset + offset];
        }

        /// <inheritdoc/>
        public int PeekBeInt32(int offset)
        {
            return (int) mem.ReadBeUInt32(Offset + offset);
        }

        /// <inheritdoc/>
        public uint PeekBeUInt32(int offset) => mem.ReadBeUInt32(Offset + offset);

        /// <inheritdoc/>
        public ulong PeekBeUInt64(int offset) => mem.ReadBeUInt64(Offset + offset);

        /// <inheritdoc/>
        public byte PeekByte(int offset)
        {
            return (byte) mem.Words[Offset + offset];
        }

        /// <inheritdoc/>
        public short PeekLeInt16(int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int PeekLeInt32(int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ushort PeekLeUInt16(int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public uint PeekLeUInt32(int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ulong PeekLeUInt64(int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public sbyte PeekSByte(int offset)
        {
            return (sbyte) mem.Words[Offset + offset];
        }

        /// <inheritdoc/>
        public short ReadBeInt16() => (short) ReadBeUInt16();

        /// <inheritdoc/>
        public int ReadBeInt32() => (int) ReadBeUInt32();

        /// <inheritdoc/>
        public long ReadBeInt64() => (long) ReadBeUInt64();

        /// <inheritdoc/>
        public ushort ReadBeUInt16()
        {
            var w = mem.Words[Offset];
            Offset += 1;
            return w;
        }

        /// <inheritdoc/>
        public uint ReadBeUInt32()
        {
            var w = mem.ReadBeUInt32(Offset);
            Offset += 2;
            return w;
        }

        /// <inheritdoc/>
        public ulong ReadBeUInt64()
        {
            var w = mem.ReadBeUInt64(Offset);
            Offset += 4;
            return w;
        }

        /// <inheritdoc/>
        public byte ReadByte()
        {
            var w = mem.Words[Offset];
            Offset += 1;
            return (byte)w;
        }

        /// <inheritdoc/>
        public byte[] ReadBytes(int addressUnits)
        {
            var bytes = new List<byte>(addressUnits * 2);
            var iEnd = Math.Min(Offset + addressUnits, endOffset);
            for (int i = (int)Offset; i < iEnd; ++i)
            {
                var w = mem.Words[i];
                bytes.Add((byte)(w >> 8));
                bytes.Add((byte) w);
            }
            Offset = iEnd;
            return bytes.ToArray();
        }

        /// <inheritdoc/>
        public byte[] ReadToEnd()
        {
            return ReadBytes((int)(endOffset - Offset));
        }

        /// <inheritdoc/>
        public byte[] ReadBytes(uint addressUnits) => ReadBytes((int) addressUnits);

        /// <inheritdoc/>
        public short ReadLeInt16()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int ReadLeInt32()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public long ReadLeInt64()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ushort ReadLeUInt16()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public uint ReadLeUInt32()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ulong ReadLeUInt64()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public sbyte ReadSByte()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public long Seek(long offset, SeekOrigin origin = SeekOrigin.Current)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryPeekBeUInt16(int offset, out ushort value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryPeekBeUInt32(int offset, out uint value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryPeekBeUInt64(int offset, out ulong value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryPeekByte(int offset, out byte value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryPeekLeUInt16(int offset, out ushort value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryPeekLeUInt32(int offset, out uint value)
        {
            return mem.TryReadLeUInt32(this.Offset + offset, out value);
        }

        /// <inheritdoc/>
        public bool TryPeekLeUInt64(int offset, out ulong value)
        {
            return mem.TryReadLeUInt64(this.Offset + offset, out value);
        }

        /// <inheritdoc/>
        public bool TryReadBe(DataType dataType, [MaybeNullWhen(false)] out Constant value)
        {
            switch (dataType.BitSize)
            {
            case 16:
                if (!TryReadBeUInt16(out ushort u16))
                    break;
                value = Constant.Word16(u16);
                return true;
            case 32:
                if (!TryReadBeUInt32(out uint u32))
                    break;
                value = Constant.Word32(u32);
                return true;
            default:
                throw new NotImplementedException($"Reading data type {dataType} not implemented yet.");
            }
            value = null!;
            return false;
        }

        /// <inheritdoc/>
        public bool TryReadBeInt16(out short value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadBeInt32(out int value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadBeInt64(out long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadBeUInt16(out ushort value)
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

        /// <inheritdoc/>
        public bool TryReadBeUInt32(out uint value)
        {
            if (!IsValidOffset(1))
            {
                value = 0;
                return false;
            }
            value = mem.ReadBeUInt32(Offset);
            Offset += 2;
            return true;
        }

        /// <inheritdoc/>
        public bool TryReadBeUInt64(out ulong value)
        {
            if (!IsValidOffset(3))
            {
                value = 0;
                return false;
            }
            value = mem.ReadBeUInt32(Offset);
            Offset += 4;
            return true;
        }

        /// <inheritdoc/>
        public bool TryReadByte(out byte value)
        {
            if (!IsValidOffset(0))
            {
                value = 0;
                return false;
            }
            value = (byte)mem.Words[Offset];
            ++Offset;
            return true;
        }

        /// <inheritdoc/>
        public bool TryReadLe(DataType dataType, [MaybeNullWhen(false)] out Constant value)
        {
            switch (dataType.BitSize)
            {
            case 8:
                if (!TryReadLeUInt16(out ushort us))
                {
                    value = default;
                    return false;
                }
                value = Constant.Byte((byte)us);
                return true;
            case 16: 
                if (!TryReadLeUInt16(out  us))
                {
                    value = default;
                    return false;
                }
                value = Constant.Word16(us);
                return true;
            }
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadLeInt16(out short value)
        {
            if (!IsValidOffset(0))
            {
                value = 0;
                return false;
            }
            value = (short) mem.Words[Offset];
            ++Offset;
            return true;
        }

        /// <inheritdoc/>
        public bool TryReadLeInt32(out int value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadLeInt64(out long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadLeSigned(DataType dataType, out long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadLeUInt16(out ushort value)
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

        /// <inheritdoc/>
        public bool TryReadLeUInt32(out uint value)
        {
            if (!TryReadLeUInt16(out ushort low) ||
                !TryReadLeUInt16(out ushort high))
            {
                value = 0;
                return false;
            }
            value = ((uint) high << 16) | low;
            return true;
        }

        /// <inheritdoc/>
        public bool TryReadLeUInt64(out ulong value)
        {
            throw new NotImplementedException();
        }
    }
}
