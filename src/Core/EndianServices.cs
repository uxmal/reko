#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core
{
    /// <summary>
    /// This class contains methods that encapsulate endianness when
    /// accessing more than one consecutive storage unit in memory.
    /// </summary>
    public abstract class EndianServices
    {
        public static EndianServices Little { get; } = new LeServices();

        public static EndianServices Big { get; } = new BeServices();

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred endianness of the processor.
        /// </summary>
        /// <param name="img">Program image to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <returns>An <seealso cref="ImageReader"/> of the appropriate endianness</returns>
        public abstract EndianImageReader CreateImageReader(MemoryArea img, Address addr);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred 
        /// endianness of the processor, limited to the specified address
        /// range.
        /// </summary>
        /// <param name="img">Program image to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <returns>An <seealso cref="ImageReader"/> of the appropriate endianness</returns>
        public abstract EndianImageReader CreateImageReader(MemoryArea memoryArea, Address addrBegin, Address addrEnd);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor.
        /// </summary>
        /// <param name="img">Program image to read</param>
        /// <param name="addr">offset from the start of the image</param>
        /// <returns>An <seealso cref="ImageReader"/> of the appropriate endianness</returns>
        public abstract EndianImageReader CreateImageReader(MemoryArea img, ulong off);

        /// <summary>
        /// Creates an <see cref="ImageWriter" /> with the preferred 
        /// endianness of the processor.
        /// </summary>
        /// <returns>An <see cref="ImageWriter"/> of the appropriate endianness.</returns>
        public abstract ImageWriter CreateImageWriter();

        /// <summary>
        /// Creates an <see cref="ImageWriter"/> with the preferred endianness, which will 
        /// write into the given <paramref name="memoryArea"/>
        /// starting at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="memoryArea">Memory area to write to.</param>
        /// <param name="addr">Address to start writing at.</param>
        /// <returns>An <see cref="ImageWriter"/> of the appropriate endianness.</returns>
        public abstract ImageWriter CreateImageWriter(MemoryArea memoryArea, Address addr);

        /// <summary>
        /// Given a sequence of adjacent subexpressions, ordered by ascending memory access,
        /// group them in a `MkSequence` expression so that they are order by 
        /// descending significance.
        /// </summary>
        /// <param name="dataType">The DataType of the resulting sequence</param>
        /// <param name="accesses">The memory</param>
        /// <returns></returns>
        public abstract MkSequence MakeSequence(DataType dataType, Expression[] accesses);

        /// <summary>
        /// Given an expression <paramref name="expr"/> referring to memory, create a slice whose
        /// low memory address is <paramref name="loOffset" /> addresses beyond the <paramref name="expr" />.
        /// </summary>
        /// <param name="dataType">DataType of the resulting slice.</param>
        /// <param name="expr">The memory expression.</param>
        /// <param name="loOffset"></param>
        /// <returns></returns>
        public abstract Slice MakeSlice(DataType dataType, Expression expr, int loOffset);

        /// <summary>
        /// Given two offsets and a size, determines whether the
        /// offsets are adjacent in memory.
        /// </summary>
        /// <param name="oLsb">Possible MSB offset</param>
        /// <param name="oMsb">Possible LSB offset</param>
        /// <param name="size">Spacing between offsets</param>
        public abstract bool OffsetsAdjacent(long oLsb, long oMsb, long size);

        /// <summary>
        /// Reads a value from memory, respecting the processor's endianness. Use this
        /// instead of ImageWriter when random access of memory is requored.
        /// </summary>
        /// <param name="mem">Memory area to read from</param>
        /// <param name="addr">Address to read from</param>
        /// <param name="dt">Data type of the data to be read</param>
        /// <param name="value">The value read from memory, if successful.</param>
        /// <returns>True if the read succeeded, false if the address was out of range.</returns>
        public abstract bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value);


        private class LeServices : EndianServices
        {
            public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
            {
                return new LeImageReader(image, addr);
            }

            public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
            {
                return new LeImageReader(image, addrBegin, addrEnd);
            }

            public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
            {
                return new LeImageReader(image, offset);
            }

            public override ImageWriter CreateImageWriter()
            {
                return new LeImageWriter();
            }

            public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
            {
                return new LeImageWriter(mem, addr);
            }

            public override MkSequence MakeSequence(DataType dataType, Expression[] accesses)
            {
                // Little endian memory accesses are least significant first,
                // so we must reverse the array before returning.
                return new MkSequence(
                    dataType,
                    accesses.Reverse().ToArray());
            }

            public override Slice MakeSlice(DataType dataType, Expression expr, int loOffset)
            {
                return new Slice(dataType, expr, loOffset);
            }

            public override bool OffsetsAdjacent(long oLsb, long oMsb, long size)
            {
                return oLsb + size == oMsb;
            }

            public override bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value)
            {
                return mem.TryReadLe(addr, dt, out value);
            }

            public override string ToString()
            {
                return "LittleEndian";
            }
        }

        private class BeServices : EndianServices
        {
            public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
            {
                return new BeImageReader(image, addr);
            }

            public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
            {
                return new BeImageReader(image, addrBegin, addrEnd);
            }

            public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
            {
                return new BeImageReader(image, offset);
            }

            public override ImageWriter CreateImageWriter()
            {
                return new BeImageWriter();
            }

            public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
            {
                return new BeImageWriter(mem, addr);
            }

            public override MkSequence MakeSequence(DataType dataType, Expression[] accesses)
            {
                // Big endian accesses are most significant first, so
                // the accesses can be used directly in the resulting
                // sequence.
                return new MkSequence(dataType, accesses);
            }

            public override Slice MakeSlice(DataType dataType, Expression expr, int bitOffset)
            {
                return new Slice(dataType, expr, expr.DataType.BitSize - (dataType.BitSize + bitOffset));
            }

            public override bool OffsetsAdjacent(long oLsb, long oMsb, long size)
            {
                return oMsb + size == oLsb;
            }

            public override bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value)
            {
                return mem.TryReadBe(addr, dt, out value);
            }

            public override string ToString()
            {
                return "BigEndian";
            }
        }
    }
}
