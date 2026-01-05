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
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Reko.Core.Memory
{
    /// <summary>
    /// Image reader for a memory area organized in 64-bit words.
    /// </summary>
    public class Word64ImageReader : ImageReader
    {
        /// <summary>
        /// Memory area organized in 64-bit words.
        /// </summary>
        protected readonly Word64MemoryArea mem;
        private readonly long endOffset;

        /// <summary>
        /// Constructs a new image reader for a memory area organized in 64-bit words.
        /// </summary>
        /// <param name="mem">64-bit memory area.</param>
        /// <param name="beginOffset">Starting offset.</param>
        public Word64ImageReader(Word64MemoryArea mem, long beginOffset)
            : this(mem, beginOffset, mem.Words.Length)
        {
        }

        /// <summary>
        /// Constructs a new image reader for a memory area organized in 64-bit words.
        /// </summary>
        /// <param name="mem">64-bit memory area.</param>
        /// <param name="addr">Starting address.</param>
        public Word64ImageReader(Word64MemoryArea mem, Address addr)
            : this(mem, addr - mem.BaseAddress, mem.Words.Length)
        {
        }

        /// <summary>
        /// Constructs a new image reader for a memory area organized in 64-bit words.
        /// </summary>
        /// <param name="mem">64-bit memory area.</param>
        /// <param name="addr">Starting address.</param>
        /// <param name="cUnits">Maximum number of words to read.</param>
        public Word64ImageReader(Word64MemoryArea mem, Address addr, long cUnits)
            : this(mem, addr - mem.BaseAddress, (addr - mem.BaseAddress) + cUnits)
        {
        }

        /// <summary>
        /// Constructs a new image reader for a memory area organized in 64-bit words.
        /// </summary>
        /// <param name="mem">64-bit memory area.</param>
        /// <param name="beginOffset">Starting offset.</param>
        /// <param name="endOffset">Ending offset.</param>
        public Word64ImageReader(Word64MemoryArea mem, long beginOffset, long endOffset)
        {
            this.mem = mem;
            this.Offset = beginOffset;
            this.endOffset = endOffset;
        }

        /// <inheritdoc/>
        public Address Address => mem.BaseAddress + Offset;

        /// <inheritdoc/>
        public int CellBitSize => 64;

        /// <inheritdoc/>
        public bool IsValid => (ulong) Offset < (ulong) endOffset; 

        /// <inheritdoc/>
        public long Offset { get; set; }

        /// <inheritdoc/>
        public Word64MemoryArea MemoryArea => mem;

        /// <inheritdoc/>
        public BinaryReader CreateBinaryReader()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsValidOffset(long offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public short PeekBeInt16(int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int PeekBeInt32(int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public uint PeekBeUInt32(int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ulong PeekBeUInt64(int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public byte PeekByte(int offset)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public short ReadBeInt16()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int ReadBeInt32()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ushort ReadBeUInt16()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public uint ReadBeUInt32()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ulong ReadBeUInt64()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public byte[] ReadBytes(uint addressUnits) => ReadBytes((int) addressUnits);

        /// <inheritdoc/>
        public byte[] ReadToEnd()
        {
            return ReadBytes((int) (endOffset - Offset));
        }

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadBe(DataType dataType, [MaybeNullWhen(false)] out Constant value)
        {
            throw new NotImplementedException();
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
        public bool TryReadBeUInt16(out ushort value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadBeUInt32(out uint value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadByte(out byte value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadLe(DataType dataType, [MaybeNullWhen(false)] out Constant value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadLeInt16(out short value)
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadLeUInt32(out uint value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryReadLeUInt64(out ulong value)
        {
            throw new NotImplementedException();
        }
    }
}
