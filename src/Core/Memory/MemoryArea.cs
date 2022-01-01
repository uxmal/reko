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
using System.Text;

namespace Reko.Core.Memory
{
    /// <summary>
    /// This class abstracts the notion of a memory area, consisting of a number of addressable
    /// memory cells at a particular address. Note that the memory cells can be of any size;
    /// they are not restricted to being 8-bit bytes. 
    /// </summary>
    /// <remarks>
    /// Most modern CPU architectures are byte-addressable, and should use <see cref="ByteMemoryArea"/>.
    /// However, other architectures (like PDP-10, Cray, MIL-SPEC-1750) are word-addressable, and 
    /// need other implementations of <see cref="MemoryArea"/> for their specific word-sizes.
    /// </remarks>
    public abstract class MemoryArea
    {
        protected MemoryArea(Address addrBase, int length, int cellBitSize)
        {
            this.BaseAddress = addrBase;
            this.Length = length;
            this.CellBitSize = cellBitSize;
            this.Relocations = new RelocationDictionary();
        }

        /// <summary>
        /// Starting address of the memory area.
        /// </summary>
        public Address BaseAddress { get; }

        /// <summary>
        /// The size of a memory cell measured in bits.
        /// </summary>
        public int CellBitSize { get; }

        /// <summary>
        /// The number of addressable units of the memory area.
        /// </summary>
        public long Length { get; }

        public RelocationDictionary Relocations { get; private set; }


        public abstract EndianImageReader CreateBeReader(Address addr);
        public abstract EndianImageReader CreateBeReader(long offset);
        public abstract EndianImageReader CreateBeReader(long offsetBegin, long offsetEnd);
        public abstract BeImageWriter CreateBeWriter(Address addr);
        public abstract BeImageWriter CreateBeWriter(long offset);


        public abstract EndianImageReader CreateLeReader(Address addr);
        public abstract EndianImageReader CreateLeReader(long offset);
        public abstract EndianImageReader CreateLeReader(long offsetBegin, long offsetEnd);
        public abstract LeImageWriter CreateLeWriter(Address addr);
        public abstract LeImageWriter CreateLeWriter(long offset);

        public bool IsValidAddress(Address addr)
        {
            return IsValidLinearAddress(addr.ToLinear());
        }

        public bool IsValidLinearAddress(ulong linearAddr)
        {
            if (linearAddr < BaseAddress.ToLinear())
                return false;
            ulong offset = (linearAddr - BaseAddress.ToLinear());
            return offset < (ulong) Length;
        }

        public abstract bool TryReadBe(long imageOffset, DataType type, out Constant c);

        public bool TryReadBe(Address addr, PrimitiveType type, out Constant c)
        {
            return TryReadBe(addr - BaseAddress, type, out c);
        }

        public bool TryReadBeDouble(long off, out double retvalue)
        {
            if (TryReadBeUInt64(off, out ulong uDouble))
            {
                retvalue = BitConverter.Int64BitsToDouble((long) uDouble);
                return true;
            }
            else
            {
                retvalue = 0.0;
                return false;
            }
        }

        public abstract bool TryReadBeUInt16(long off, out ushort retvalue);
        public abstract bool TryReadBeUInt32(long off, out uint retvalue);
        public abstract bool TryReadBeUInt64(long off, out ulong retvalue);

        public bool TryReadBeUInt16(Address off, out ushort retvalue) => TryReadBeUInt16(off - BaseAddress, out retvalue);
        public bool TryReadBeUInt32(Address off, out uint retvalue) => TryReadBeUInt32(off - BaseAddress, out retvalue);
        public bool TryReadBeUInt64(Address off, out ulong retvalue) => TryReadBeUInt64(off - BaseAddress, out retvalue);


        public abstract bool TryReadLe(long imageOffset, DataType type, out Constant c);

        public bool TryReadLe(Address addr, PrimitiveType type, out Constant c)
        {
            return TryReadLe(addr - BaseAddress, type, out c);
        }

        public bool TryReadLeDouble(long off, out double retvalue)
        {
            if (TryReadLeUInt64(off, out ulong uDouble))
            {
                retvalue = BitConverter.Int64BitsToDouble((long) uDouble);
                return true;
            }
            else
            {
                retvalue = 0.0;
                return false;
            }
        }

        public abstract bool TryReadLeInt32(long off, out int retvalue);

        public abstract bool TryReadLeUInt16(long off, out ushort retvalue);
        public abstract bool TryReadLeUInt32(long off, out uint retvalue);
        public abstract bool TryReadLeUInt64(long off, out ulong retvalue);


        public bool TryReadLeUInt16(Address off, out ushort retvalue) => TryReadLeUInt16(off - BaseAddress, out retvalue);
        public bool TryReadLeUInt32(Address off, out uint retvalue) => TryReadLeUInt32(off - BaseAddress, out retvalue);
        public bool TryReadLeUInt64(Address off, out ulong retvalue) => TryReadLeUInt64(off - BaseAddress, out retvalue);
        public abstract bool TryReadByte(long off, out byte b);


        public abstract void WriteByte(long off, byte value);

        public abstract void WriteBeUInt16(long off, ushort value);
        public abstract void WriteBeUInt32(long off, uint value);

        public abstract void WriteLeUInt16(long off, ushort value);
        public abstract void WriteLeUInt32(long off, uint value);
        public void WriteLeUInt32(Address ea, uint value) => WriteLeUInt32(ea - BaseAddress, value);
    }
}
