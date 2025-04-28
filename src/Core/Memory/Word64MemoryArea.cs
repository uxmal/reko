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
using Reko.Core.Output;
using Reko.Core.Types;

namespace Reko.Core.Memory
{
    /// <summary>
    /// This class represents memory organized in 64-bit storage units (e.g. Cray YMP).
    /// </summary>
    public class Word64MemoryArea : MemoryArea
    {
        /// <summary>
        /// Construct a memory area with 64-bit words.
        /// </summary>
        /// <param name="addr">Address of the beginning of the memory area.</param>
        /// <param name="words"></param>
        public Word64MemoryArea(Address addr, ulong[] words) :
            base(addr, words.Length, 64, new MemoryFormatter(PrimitiveType.Word64, 1, 2, 16, 8))
        {
            this.Words = words;
        }

        /// <summary>
        /// The underlying data.
        /// </summary>
        public ulong[] Words { get; }

        /// <summary>
        /// Creates a big-endian image reader for this memory area.
        /// </summary>
        /// <param name="addr">Address at which to start reading.</param>
        /// <returns>Big endian image reader.</returns>
        public override EndianImageReader CreateBeReader(Address addr)
        {
            return new Word64BeReader(this, addr);
        }

        /// <summary>
        /// Creates a big-endian image reader for this memory area.
        /// </summary>
        /// <param name="addr">Address at which to start reading.</param>
        /// <param name="cUnits">Maximum number of words to read.</param>
        /// <returns>Big endian image reader.</returns>
        public override EndianImageReader CreateBeReader(Address addr, long cUnits)
        {
            return new Word64BeReader(this, addr, cUnits);
        }

        /// <summary>
        /// Creates a big-endian image reader for this memory area.
        /// </summary>
        /// <param name="offset">Offset from the start of the memory area from
        /// which to start reading.</param>
        /// <returns>Big endian image reader.</returns>
        public override EndianImageReader CreateBeReader(long offset)
        {
            return new Word64BeReader(this, offset);
        }

#pragma warning disable CS1591
        public override EndianImageReader CreateBeReader(long offsetBegin, long offsetEnd)
        {
            throw new System.NotImplementedException();
        }

        public override BeImageWriter CreateBeWriter(Address addr)
        {
            throw new System.NotImplementedException();
        }

        public override BeImageWriter CreateBeWriter(long offset)
        {
            throw new System.NotImplementedException();
        }

        public override EndianImageReader CreateLeReader(Address addr)
        {
            throw new System.NotImplementedException();
        }

        public override EndianImageReader CreateLeReader(Address addr, long cUnits)
        {
            throw new System.NotImplementedException();
        }

        public override EndianImageReader CreateLeReader(long offset)
        {
            throw new System.NotImplementedException();
        }

        public override EndianImageReader CreateLeReader(long offsetBegin, long offsetEnd)
        {
            throw new System.NotImplementedException();
        }

        public override LeImageWriter CreateLeWriter(Address addr)
        {
            throw new System.NotImplementedException();
        }

        public override LeImageWriter CreateLeWriter(long offset)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryReadBe(long imageOffset, DataType type, out Constant c)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryReadBeUInt16(long off, out ushort retvalue)
        {
            throw new System.NotImplementedException();
        }

#pragma warning restore CS1591

        public override bool TryReadBeUInt32(long off, out uint retvalue)
        {
            if (0 <= off && off < Words.Length)
            {
                retvalue = (uint) (Words[off] >> 32);
                return true;
            }
            else
            {
                retvalue = 0;
                return false;
            }
        }

        public override bool TryReadBeUInt64(long off, out ulong retvalue)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryReadByte(long off, out byte b)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryReadLe(long imageOffset, DataType type, out Constant c)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryReadLeInt32(long off, out int retvalue)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryReadLeUInt16(long off, out ushort retvalue)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryReadLeUInt32(long off, out uint retvalue)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryReadLeUInt64(long off, out ulong retvalue)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteBeUInt16(long off, ushort value)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteBeUInt32(long off, uint value)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteBeUInt64(long off, ulong value)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteByte(long off, byte value)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteLeUInt16(long off, ushort value)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteLeUInt32(long off, uint value)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteLeUInt64(long off, ulong value)
        {
            throw new System.NotImplementedException();
        }
    }
}