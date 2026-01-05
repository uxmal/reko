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
using Reko.Core.Lib;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// This class contains methods that encapsulate endianness when
    /// accessing more than one consecutive storage unit in memory.
    /// </summary>
    public abstract class EndianServices
    {
        /// <summary>
        /// An instance of <see cref="EndianServices"/> that can be used
        /// to perform little-endian memory operations.
        /// </summary>
        public static EndianServices Little { get; } = new LeServices();

        /// <summary>
        /// An instance of <see cref="EndianServices"/> that can be used
        /// to perform big-endian memory operations.
        /// </summary>
        public static EndianServices Big { get; } = new BeServices();

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred endianness of the processor.
        /// </summary>
        /// <param name="memory">Memory to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <param name="rdr"> An <seealso cref="ImageReader"/> of the appropriate endianness</param>
        /// <returns>True if the provided address refers to valid memory,
        /// otherwise false.</returns>
        public abstract bool TryCreateImageReader(IMemory memory, Address addr, [MaybeNullWhen(false)] out EndianImageReader rdr);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor, limited to the specified number of units.
        /// </summary>
        /// <param name="memory">Memory to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <param name="cbUnits">Number of memory units after which stop reading.</param>
        /// <param name="rdr"> An <seealso cref="ImageReader"/> of the appropriate endianness</param>
        /// <returns>True if the provided address refers to valid memory,
        /// otherwise false.</returns>
        public abstract bool TryCreateImageReader(IMemory memory, Address addr, long cbUnits, [MaybeNullWhen(false)] out EndianImageReader rdr);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred endianness of the processor.
        /// </summary>
        /// <param name="mem">Memory area to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <returns>An <seealso cref="ImageReader"/> of the appropriate endianness</returns>
        public abstract EndianImageReader CreateImageReader(MemoryArea mem, Address addr);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor, limited to the specified number of units.
        /// </summary>
        /// <param name="memoryArea">Memory area to read</param>
        /// <param name="addr">Address at which to start</param>
        /// <param name="cbUnits">Number of memory units after which stop reading.</param>
        /// <returns>An <seealso cref="EndianImageReader"/> of the appropriate endianness</returns>
        public abstract EndianImageReader CreateImageReader(MemoryArea memoryArea, Address addr, long cbUnits);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred 
        /// endianness of the processor, limited to the specified offset
        /// range.
        /// </summary>
        /// <param name="memoryArea">Program image to read</param>
        /// <param name="offsetBegin">Offset within the memory area which to begin reading.</param>
        /// <param name="offsetEnd">Offset within the memory area at which to stop reading.</param>
        /// <returns>An <seealso cref="ImageReader"/> of the appropriate endianness</returns>
        public abstract EndianImageReader CreateImageReader(MemoryArea memoryArea, long offsetBegin, long offsetEnd);
        
        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor.
        /// </summary>
        /// <param name="mem">Memory area to read</param>
        /// <param name="offset">offset from the start of the image</param>
        /// <returns>An <seealso cref="ImageReader"/> of the appropriate endianness</returns>
        public abstract EndianImageReader CreateImageReader(MemoryArea mem, long offset);

        /// <summary>
        /// Creates an <see cref="EndianImageReader" /> with the preferred
        /// endianness of the processor.
        /// </summary>
        /// <param name="bytes">Memory area to read</param>
        /// <param name="offset">offset from the start of the image</param>
        /// <returns>An <seealso cref="ImageReader"/> of the appropriate endianness</returns>
        public abstract EndianImageReader CreateImageReader(byte[] bytes, long offset);

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
        /// Creates an <see cref="ImageWriter"/> with the preferred endianness, which will 
        /// write into the given <paramref name="memoryArea"/>
        /// starting at offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="memoryArea">Memory area to write to.</param>
        /// <param name="offset">Offset from the beginning of the memory area to 
        /// start writing at.</param>
        /// <returns>An <see cref="ImageWriter"/> of the appropriate endianness.</returns>
        public abstract ImageWriter CreateImageWriter(MemoryArea memoryArea, long offset);


        /// <summary>
        /// Creates an <see cref="ImageWriter"/> with the preferred endianness, which will 
        /// write into the given <paramref name="bytes"/>
        /// starting at offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="bytes">Bytes to write to.</param>
        /// <param name="offset">Offset at which to start writing at.</param>
        /// <returns>An <see cref="ImageWriter"/> of the appropriate endianness.</returns>
        public abstract ImageWriter CreateImageWriter(byte [] bytes, long offset);

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
        /// <param name="bitsPerUnit">Number of bits per memory unit of the current architecture.</param>
        /// <returns></returns>
        public abstract Slice MakeSlice(DataType dataType, Expression expr, int loOffset, int bitsPerUnit);

        /// <summary>
        /// Given two offsets and a size, determines whether the
        /// offsets are adjacent in memory.
        /// </summary>
        /// <param name="oLsb">Possible MSB offset</param>
        /// <param name="oMsb">Possible LSB offset</param>
        /// <param name="size">Spacing between offsets</param>
        public abstract bool OffsetsAdjacent(long oLsb, long oMsb, long size);

        /// <summary>
        /// Given the components of a <see cref="MemoryAccess"/>, construct a new memory
        /// access instance that is the result of slicing that memory access.
        /// </summary>
        /// <param name="memoryId">MemoryID of the resulting memory access.</param>
        /// <param name="effectiveAddress">The effective address of the memory access.</param>
        /// <param name="dataType">The original data type of the memory access.</param>
        /// <param name="bitRange">The bitrange of the slice being taken.</param>
        /// <param name="granularity">Number of bits per memory unit of the currecnt architecture.</param>
        /// <returns>A <see cref="MemoryAccess"/> instance with the offset correctly adjusted
        /// for endianness.</returns>
        public abstract MemoryAccess SliceMemoryAccess(Identifier memoryId, Expression effectiveAddress, DataType dataType, BitRange bitRange, int granularity);

        /// <summary>
        /// Given a stack storage, generate a slice of said storage.
        /// </summary>
        /// <param name="stg">The storage to slice.</param>
        /// <param name="bitRange">The bit range to slice out of the storage.</param>
        /// <param name="granularity">The memory granularity of the processor.</param>
        public abstract StackStorage SliceStackStorage(StackStorage stg, BitRange bitRange, int granularity);

        /// <summary>
        /// Reads a value from memory, respecting the processor's endianness. Use this
        /// instead of ImageWriter when random access of memory is required.
        /// </summary>
        /// <param name="mem">Memory to read from</param>
        /// <param name="addr">Address to read from</param>
        /// <param name="dt">Data type of the data to be read</param>
        /// <param name="value">The value read from memory, if successful.</param>
        /// <returns>True if the read succeeded, false if the address was out of range.</returns>
        public abstract bool TryRead(IMemory mem, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value);

        /// <summary>
        /// Reads a value from memory, respecting the processor's endianness. Use this
        /// instead of ImageWriter when random access of memory is required.
        /// </summary>
        /// <param name="mem">Memory area to read from</param>
        /// <param name="addr">Address to read from</param>
        /// <param name="dt">Data type of the data to be read</param>
        /// <param name="value">The value read from memory, if successful.</param>
        /// <returns>True if the read succeeded, false if the address was out of range.</returns>
        public abstract bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value);

        /// <summary>
        /// Reverses the bytes in the provided array, in groups of size <paramref name="wordSize"/>.
        /// </summary>
        /// <param name="bytes">The bytes to reverse.</param>
        /// <param name="wordSize">Size of each group to reverse.</param>
        /// <returns>A new array containing the bytes in reversed order.</returns>
        public static byte[] SwapByGroups(ReadOnlySpan<byte> bytes, int wordSize)
        {
            if (wordSize <= 0 || bytes.Length % wordSize != 0)
                throw new ArgumentOutOfRangeException(nameof(wordSize));
            var newBytes = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i += wordSize)
            {
                for (int j = 0; j < wordSize; ++j)
                {
                    newBytes[i + (wordSize - j - 1)] = bytes[i + j];
                }
            }
            return newBytes;
        }


        /// <summary>
        /// Create a sliced memory access expression.
        /// </summary>
        /// <param name="memoryId">Memory identifier of the expresion.</param>
        /// <param name="effectiveAddress">Effective address of the expression.</param>
        /// <param name="byteOffset">Byte offset from the beginning of the effective address.</param>
        /// <param name="bitRange">The bit range of interest.</param>
        /// <returns></returns>
        protected MemoryAccess MakeSlicedMemoryAccess(
            Identifier memoryId,
            Expression effectiveAddress,
            int byteOffset,
            BitRange bitRange)
        {
            var eaBitsize = effectiveAddress.DataType.BitSize;
            if (byteOffset != 0)
            {
                if (effectiveAddress is Address addr)
                {
                    effectiveAddress = addr.Add(byteOffset);
                }
                else
                {
                    effectiveAddress = new BinaryExpression(
                        Operator.IAdd,
                        effectiveAddress.DataType,
                        effectiveAddress,
                        Constant.Create(
                            PrimitiveType.Create(Domain.SignedInt, eaBitsize),
                            byteOffset));
                }
            }
            var dt = PrimitiveType.CreateWord(bitRange.Extent);
            return new MemoryAccess(memoryId, effectiveAddress, dt);
        }

        private class LeServices : EndianServices
        {
            public override bool TryCreateImageReader(IMemory mem, Address addr, [MaybeNullWhen(false)] out EndianImageReader rdr)
            {
                return mem.TryCreateLeReader(addr, out rdr);
            }

            public override bool TryCreateImageReader(IMemory mem, Address addr, long cUnits, [MaybeNullWhen(false)] out EndianImageReader rdr)
            {
                return mem.TryCreateLeReader(addr, cUnits, out rdr);
            }

            public override EndianImageReader CreateImageReader(MemoryArea mem, Address addr)
            {
                return mem.CreateLeReader(addr);
            }

            public override EndianImageReader CreateImageReader(MemoryArea mem, Address addr, long cUnits)
            {
                return mem.CreateLeReader(addr, cUnits);
            }

            public override EndianImageReader CreateImageReader(MemoryArea mem, long offsetBegin, long offsetEnd)
            {
                return mem.CreateLeReader(offsetBegin, offsetEnd);
            }

            public override EndianImageReader CreateImageReader(MemoryArea mem, long offset)
            {
                return mem.CreateLeReader(offset);
            }

            public override EndianImageReader CreateImageReader(byte[] bytes, long off)
            {
                return new LeImageReader(bytes, off);
            }

            public override ImageWriter CreateImageWriter()
            {
                return new LeImageWriter();
            }

            public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
            {
                return mem.CreateLeWriter(addr);
            }

            public override ImageWriter CreateImageWriter(MemoryArea mem, long offset)
            {
                return mem.CreateLeWriter(offset);
            }

            public override ImageWriter CreateImageWriter(byte[] bytes, long offset)
            {
                return new LeImageWriter(bytes, (uint)offset);
            }

            public override MkSequence MakeSequence(DataType dataType, Expression[] accesses)
            {
                // Little endian memory accesses are least significant first,
                // so we must reverse the array before returning.
                return new MkSequence(
                    dataType,
                    accesses.Reverse().ToArray());
            }

            public override Slice MakeSlice(DataType dataType, Expression expr, int loOffset, int bitsPerUnit)
            {
                return new Slice(dataType, expr, loOffset);
            }

            public override bool OffsetsAdjacent(long oLsb, long oMsb, long size)
            {
                return oLsb + size == oMsb;
            }

            /// <inheritdoc />
            public override MemoryAccess SliceMemoryAccess(Identifier memoryId, Expression effectiveAddress, DataType dataType, BitRange bitRange, int granularity)
            {
                var byteOffset = bitRange.Lsb / granularity;

                var mem = MakeSlicedMemoryAccess(memoryId, effectiveAddress, byteOffset, bitRange);
                return mem;
            }


            public override StackStorage SliceStackStorage(StackStorage stg, BitRange bitRange, int granularity)
            {
                var byteOffset = bitRange.Lsb / granularity;
                var dt = PrimitiveType.CreateWord(bitRange.Extent);
                return new StackStorage(stg.StackOffset + byteOffset, dt);
            }

            public override bool TryRead(IMemory mem, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value)
            {
                return mem.TryReadLe(addr, dt, out value);
            }

            public override bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value)
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
            public override bool TryCreateImageReader(IMemory mem, Address addr, [MaybeNullWhen(false)] out EndianImageReader rdr)
            {
                return mem.TryCreateBeReader(addr, out rdr);
            }

            public override bool TryCreateImageReader(IMemory mem, Address addr, long cUnits, [MaybeNullWhen(false)] out EndianImageReader rdr)
            {
                return mem.TryCreateBeReader(addr, cUnits, out rdr);
            }

            public override EndianImageReader CreateImageReader(MemoryArea mem, Address addr)
            {
                return mem.CreateBeReader(addr);
            }

            public override EndianImageReader CreateImageReader(MemoryArea mem, Address addr, long cUnits)
            {
                return mem.CreateBeReader(addr, cUnits);
            }

            public override EndianImageReader CreateImageReader(MemoryArea mem, long offsetBegin, long offsetEnd)
            {
                return mem.CreateBeReader(offsetBegin, offsetEnd);
            }

            public override EndianImageReader CreateImageReader(MemoryArea mem, long offset)
            {
                return mem.CreateBeReader(offset);
            }

            public override EndianImageReader CreateImageReader(byte[] bytes, long off)
            {
                return new BeImageReader(bytes, off, bytes.Length);
            }


            public override ImageWriter CreateImageWriter()
            {
                return new BeImageWriter();
            }

            public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
            {
                return mem.CreateBeWriter(addr);
            }

            public override ImageWriter CreateImageWriter(MemoryArea mem, long offset)
            {
                return mem.CreateBeWriter(offset);
            }

            public override ImageWriter CreateImageWriter(byte[] bytes, long offset)
            {
                return new BeImageWriter(bytes, (uint)offset);
            }

            public override MkSequence MakeSequence(DataType dataType, Expression[] accesses)
            {
                // Big endian accesses are most significant first, so
                // the accesses can be used directly in the resulting
                // sequence.
                return new MkSequence(dataType, accesses);
            }

            public override Slice MakeSlice(DataType dataType, Expression expr, int bitOffset, int bitsPerUnit)
            {
                return new Slice(dataType, expr, expr.DataType.MeasureBitSize(bitsPerUnit) - (dataType.MeasureBitSize(bitsPerUnit) + bitOffset));
            }

            public override bool OffsetsAdjacent(long oLsb, long oMsb, long size)
            {
                return oMsb + size == oLsb;
            }

            /// <summary>
            /// <inheritdoc />
            /// </summary>
            public override MemoryAccess SliceMemoryAccess(Identifier memoryId, Expression effectiveAddress, DataType dataType, BitRange bitRange, int granularity)
            {
                var byteOffset = (dataType.BitSize - bitRange.Msb) / granularity;

                var mem = MakeSlicedMemoryAccess(memoryId, effectiveAddress, byteOffset, bitRange);
                return mem;
            }

            public override StackStorage SliceStackStorage(StackStorage stg, BitRange bitRange, int granularity)
            {
                var byteOffset = (stg.DataType.BitSize - bitRange.Msb) / granularity;
                var dt = PrimitiveType.CreateWord(bitRange.Extent);
                return new StackStorage(stg.StackOffset +  byteOffset, dt);
            }

            public override bool TryRead(IMemory mem, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value)
            {
                return mem.TryReadBe(addr, dt, out value);
            }

            public override bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value)
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
