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

namespace Reko.Core.Memory
{
    public class Word64MemoryArea : MemoryArea
    {
        public Word64MemoryArea(Address addr, ulong[] words) : base(addr, words.Length, 64)
        {
            this.Words = words;
        }

        public ulong[] Words { get; }

        public override EndianImageReader CreateBeReader(Address addr)
        {
            return new Word64BeReader(this);
        }

        public override EndianImageReader CreateBeReader(long offset)
        {
            return new Word64BeReader(this, offset);
        }

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
    }
}