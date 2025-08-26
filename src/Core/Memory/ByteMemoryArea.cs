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
using Reko.Core.Lib;
using Reko.Core.Loading;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Buffers.Binary;

namespace Reko.Core.Memory
{
    /// <summary>
    /// Representation of a byte-granular area of memory. Byte-addressable CPU architectures
    /// should use this class to represent in-memory data.
    /// </summary>
    /// <remarks>
    /// Loading sparse images should load multiple memory areas. Use <see cref="SegmentMap"/>
    /// and <see cref="ImageSegment"/>s to accomplish this.
    /// </remarks>
    public class ByteMemoryArea : MemoryArea
	{
        /// <summary>
        /// Constructs a byte memory area.
        /// </summary>
        /// <param name="addrBase">Start address of the memory area.</param>
        /// <param name="bytes">The bytes of the memory area.</param>
		public ByteMemoryArea(Address addrBase, byte [] bytes)
            : base(addrBase, bytes.Length, 8, new MemoryFormatter(PrimitiveType.Byte, 1, 16, 2, 1))
		{
			this.Bytes = bytes;
		}

        /// <summary>
        /// The contents of the memory area.
        /// </summary>
        public byte[] Bytes { get; }

        /// <summary>
        /// Compares two byte arrays for equality.
        /// </summary>
        /// <param name="src">First array.</param>
        /// <param name="iSrc">Starting position in first array.</param>
        /// <param name="dst">Second array.</param>
        /// <param name="cb">number of bytes to compare.</param>
        /// <returns>True if the byte areas were equal; otherwise false.</returns>
        public static bool CompareArrays(byte[] src, int iSrc, byte[] dst, int cb)
        {
            if (iSrc + cb > src.Length)
                return false;
            int iDst = 0;
            while (cb != 0)
            {
                if (src[iSrc++] != dst[iDst++])
                    return false;
                --cb;
            }
            return true;
        }

        /// <summary>
        /// Returns a string representation of this byte area.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Image {0}{1} - length {2} bytes{3}", "{", BaseAddress, this.Length, "}");
        }

        /// <summary>
        /// Creates a big-endian reader, positioned at the given address.
        /// </summary>
        /// <param name="addr">Address from which the created reader starts reading.</param>
        /// <returns>A big-endian <see cref="EndianImageReader"/>.
        /// </returns>
        public override EndianImageReader CreateBeReader(Address addr)
        {
            return new BeImageReader(this, addr);
        }

        /// <summary>
        /// Creates a big-endian reader, positioned at the given address.
        /// </summary>
        /// <param name="addr">Address from which the created reader starts reading.</param>
        /// <param name="cUnits">Maximum number of bytes to read.</param>
        /// <returns>A big-endian <see cref="EndianImageReader"/>.
        /// </returns>
        public override EndianImageReader CreateBeReader(Address addr, long cUnits)
        {
            return new BeImageReader(this, addr, cUnits);
        }

        /// <summary>
        /// Creates a big-endian reader, positioned at the given offset.
        /// </summary>
        /// <param name="offset">Offset from which the created reader starts reading.</param>
        /// <returns>A big-endian <see cref="EndianImageReader"/>.
        /// </returns>
        public override EndianImageReader CreateBeReader(long offset)
        {
            return new BeImageReader(this, offset);
        }

        /// <summary>
        /// Creates a big-endian reader, positioned at the given offset.
        /// </summary>
        /// <param name="offsetBegin">Offset from which the created reader starts reading.</param>
        /// <param name="offsetEnd">Offset at which the created reader stops reading.</param>
        /// <returns>A big-endian <see cref="EndianImageReader"/>.
        /// </returns>
        public override EndianImageReader CreateBeReader(long offsetBegin, long offsetEnd)
        {
            return new BeImageReader(this, offsetBegin, offsetEnd);
        }

        /// <summary>
        /// Creates a big-endian writer, positioned at the given address.
        /// </summary>
        /// <param name="addr">Address at which to start writing.</param>
        /// <returns>A big-endian <see cref="BeImageWriter"/>.</returns>
        public override BeImageWriter CreateBeWriter(Address addr)
        {
            return new BeImageWriter(this, addr);
        }

        /// <summary>
        /// Creates a big-endian writer, positioned at the given offset.
        /// </summary>
        /// <param name="offset">Offset at which to start writing.</param>
        /// <returns>A big-endian <see cref="BeImageWriter"/>.</returns>
        public override BeImageWriter CreateBeWriter(long offset)
        {
            return new BeImageWriter(this, offset);
        }

        /// <summary>
        /// Creates a little-endian reader, positioned at the given address.
        /// </summary>
        /// <param name="addr">Address from which the created reader starts reading.</param>
        /// <returns>A little-endian <see cref="EndianImageReader"/>.
        /// </returns>

        public override EndianImageReader CreateLeReader(Address addr)
        {
            return new LeImageReader(this, addr);
        }

        /// <summary>
        /// Creates a little-endian reader, positioned at the given address.
        /// </summary>
        /// <param name="addr">Address from which the created reader starts reading.</param>
        /// <param name="cUnits">Maximum number of bytes to read.</param>
        /// <returns>A little-endian <see cref="EndianImageReader"/>.
        /// </returns>
        public override EndianImageReader CreateLeReader(Address addr, long cUnits)
        {
            return new LeImageReader(this, addr, cUnits);
        }

        /// <summary>
        /// Creates a little-endian reader, positioned at the given offset.
        /// </summary>
        /// <param name="offset">Offset from which the created reader starts reading.</param>
        /// <returns>A little-endian <see cref="EndianImageReader"/>.
        /// </returns>
        public override EndianImageReader CreateLeReader(long offset)
        {
            return new LeImageReader(this, offset);
        }

        /// <summary>
        /// Creates a little-endian reader, positioned at the given offset.
        /// </summary>
        /// <param name="offsetBegin">Offset from which the created reader starts reading.</param>
        /// <param name="offsetEnd">Offset at which the created reader stops reading.</param>
        /// <returns>A little-endian <see cref="EndianImageReader"/>.
        /// </returns>
        public override EndianImageReader CreateLeReader(long offsetBegin, long offsetEnd)
        {
            return new LeImageReader(this, offsetBegin, offsetEnd);
        }

        /// <summary>
        /// Creates a little-endian writer, positioned at the given address.
        /// </summary>
        /// <param name="addr">Offset at which to start writing.</param>
        /// <returns>A little-endian <see cref="BeImageWriter"/>.</returns>
        public override LeImageWriter CreateLeWriter(Address addr)
        {
            return new LeImageWriter(this, addr);
        }

        /// <summary>
        /// Creates a little-endian writer, positioned at the given offset.
        /// </summary>
        /// <param name="offset">Offset at which to start writing.</param>
        /// <returns>A little-endian <see cref="BeImageWriter"/>.</returns>
        public override LeImageWriter CreateLeWriter(long offset)
        {
            return new LeImageWriter(this, offset);
        }


        /// <summary>
        /// Adds the delta to the ushort at the given offset.
        /// </summary>
        /// <param name="imageOffset"></param>
        /// <param name="delta"></param>
        /// <returns>The new value of the ushort</returns>
        public ushort FixupLeUInt16(uint imageOffset, ushort delta)
		{
			ushort seg = ReadLeUInt16(Bytes, imageOffset);
			seg = (ushort) (seg + delta);
			WriteLeUInt16(imageOffset, seg);
			return seg;
		}

        /// <summary>
        /// Read any relocation at the given image offset.
        /// </summary>
        /// <param name="imageOffset">Offset to read from.</param>
        /// <returns>A <see cref="Constant"/> containing a relocated value if 
        /// a relocation is located at the given <paramref name="imageOffset"/>;
        /// otherwise null.
        /// </returns>
        public Constant? ReadRelocation(long imageOffset)
        {
            return Relocations[BaseAddress.ToLinear() + (ulong) imageOffset];
        }

        /// <inheritdoc/>
        public override bool TryReadBeUInt16(long offset, out ushort value)
        {
            return TryReadBeUInt16(Bytes, offset, out value);
        }

        /// <inheritdoc/>
        public override bool TryReadBeUInt32(long offset, out uint value)
        {
            return TryReadBeUInt32(Bytes, offset, out value);
        }

        /// <inheritdoc/>
        public override bool TryReadBeUInt64(long offset, out ulong value)
        {
            return TryReadBeUInt64(Bytes, offset, out value);
        }


        /// <inheritdoc/>
        public override bool TryReadLeInt32(long offset, out int value)
        {
            return TryReadLeInt32(Bytes, (uint)offset, out value);
        }

        /// <inheritdoc/>
        public override bool TryReadLeUInt16(long offset, out ushort value)
        {
            return TryReadLeUInt16(Bytes, offset, out value);
        }

        /// <inheritdoc/>
        public override bool TryReadLeUInt32(long offset, out uint value)
        {
            return TryReadLeUInt32(Bytes, offset, out value);
        }

        /// <inheritdoc/>
        public override bool TryReadLeUInt64(long offset, out ulong value)
        {
            return TryReadLeUInt64(Bytes, offset, out value);
        }


        /// <summary>
        /// Reads a little-endian word from image offset.
        /// </summary>
        /// <remarks>
        /// If the word being read was a relocation, it is returned with a [[pointer]]
        /// or [[segment]] data type. Otherwise a neutral [[word]] is returned.
        /// </remarks>
        /// <param name="imageOffset">Offset from image start, in bytes.</param>
        /// <param name="type">Size of the value being requested.</param>
        /// <param name="c">If successful, returns the requested value.</param>
        /// <returns>True if the read succeeded; false if the <paramref name="imageOffset"/> was outside
        /// the memory area.</returns>
        public override bool TryReadLe(long imageOffset, DataType type, out Constant c)
        {
            var rc = ReadRelocation(imageOffset);
            int size = type.Size;
            if (rc is not null && rc.DataType.Size == size)
            {
                c = rc;
                return true;
            }
            return TryReadLe(this.Bytes, imageOffset, type, out c);
        }

        /// <inheritdoc/>
        public override bool TryReadBe(long imageOffset, DataType type, out Constant c)
        {
            var rc = ReadRelocation(imageOffset);
            if (rc is not null && rc.DataType.Size == type.Size)
            {
                c = rc;
                return true;
            }
            if (type.Size + imageOffset > Bytes.Length)
            {
                c = default!;
                return false;
            }
            return TryReadBe(Bytes, imageOffset, type, out c);
        }

        /// <inheritdoc/>
        public static bool TryReadLe(byte[] abImage, long imageOffset, DataType type, out Constant c)
        {
            if (type.IsReal)
            {
                switch (type.Size)
                {
                case 4:
                    if (!TryReadLeInt32(abImage, (uint)imageOffset, out var fl))
                        break;
                    c = Constant.FloatFromBitpattern(fl);
                    return true;
                case 8:
                    if (!TryReadLeInt64(abImage, imageOffset, out var dbl))
                        break;
                    c = Constant.DoubleFromBitpattern(dbl);
                    return true;
                case 10:
                    if (!TryReadLeReal80(abImage, imageOffset, out var real80))
                        break;
                    c = Constant.Real80(real80);
                    return true;
                default:
                    throw new InvalidOperationException(string.Format("Real type {0} not supported.", type));
                }
            }

            switch (type.Size)
            {
            case 1:
                if (!TryReadByte(abImage, imageOffset, out var b))
                    break;
                c = Constant.Create(type, b);
                return true;
            case 2:
                if (!TryReadLeUInt16(abImage, (uint) imageOffset, out var h))
                    break;
                c = Constant.Create(type, h);
                return true;
            case 3:
            case 4:
                if (!TryReadLeUInt32(abImage, imageOffset, out var i))
                    break;
                c = Constant.Create(type, i);
                return true;
            case 5:
            case 8:
                if (!TryReadLeUInt64(abImage, imageOffset, out var l))
                    break;
                c = Constant.Create(type, l);
                return true;
            default:
               throw new NotImplementedException(string.Format("Primitive type {0} not supported.", type));
            }
            c = default!;
            return false;
        }

        /// <inheritdoc/>
        public static bool TryReadBe(byte[] abImage, long imageOffset, DataType type, out Constant value)
        {
            if (type.IsReal)
            {
                switch (type.Size)
                {
                case 4:
                    if (!TryReadBeInt32(abImage, imageOffset, out int fl))
                        break;
                    value = Constant.FloatFromBitpattern(fl);
                    return true;
                case 8:
                    if (!TryReadBeInt64(abImage, imageOffset, out long dbl))
                        break;
                    value = Constant.DoubleFromBitpattern(dbl);
                    return true;
                default: throw new NotSupportedException(string.Format("Real type {0} not supported.", type));
                }
            }
            else
            {
                switch (type.Size)
                {
                case 1:
                    if (!TryReadByte(abImage, imageOffset, out byte b))
                        break;
                    value = Constant.Create(type, b);
                    return true;
                case 2:
                    if (!TryReadBeUInt16(abImage, imageOffset, out ushort h))
                        break;
                    value = Constant.Create(type, h);
                    return true;
                case 4:
                    if (!TryReadBeUInt32(abImage, imageOffset, out uint w))
                        break;
                    value = Constant.Create(type, w);
                    return true;
                case 8:
                    if (!TryReadBeUInt64(abImage, imageOffset, out ulong d))
                        break;
                    value = Constant.Create(type, d);
                    return true;
                default:
                    throw new NotImplementedException(string.Format("Primitive type {0} not supported.", type));
                }
            }
            value = default!;
            return false;
        }

        /// <summary>
        /// Reads a 64-bit signed big endian value from the given offset from the start 
        /// of the <paramref name="image"/>.
        /// </summary>
        /// <param name="image">Byte array to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <param name="value">Value read.</param>
        /// <returns>True if the read was successful; false if not.
        /// </returns>
        public static bool TryReadBeInt64(byte[] image, long offset, out long value)
        {
            return TryReadBeInt64(image, offset, image.Length, out value);
        }

        /// <summary>
        /// Reads a 64-bit signed big endian value from the given offset from the start 
        /// of the <paramref name="image"/>.
        /// </summary>
        /// <param name="image">Byte array to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <param name="offsetEnd">End of area from which to read.</param>
        /// <param name="value">Value read.</param>
        /// <returns>True if the read was successful; false if not.
        /// </returns>
        public static bool TryReadBeInt64(byte[] image, long offset, long offsetEnd, out long value)
        {
            if (offset + 8 <= offsetEnd)
            {
                var span = image.AsSpan((int) offset, 8);
                value = BinaryPrimitives.ReadInt64BigEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 64-bit unsigned big endian value from the given offset from the start 
        /// of the <paramref name="image"/>.
        /// </summary>
        /// <param name="image">Byte array to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <param name="value">Value read.</param>
        /// <returns>True if the read was successful; false if not.
        /// </returns>
        public static bool TryReadBeUInt64(byte[] image, long offset, out ulong value)
        {
            return TryReadBeUInt64(image, offset, image.Length, out value);
        }

        /// <summary>
        /// Reads a 64-bit unsigned big endian value from the given offset from the start 
        /// of the <paramref name="image"/>.
        /// </summary>
        /// <param name="image">Byte array to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <param name="offsetEnd">End of area from which to read.</param>
        /// <param name="value">Value read.</param>
        /// <returns>True if the read was successful; false if not.
        /// </returns>
        public static bool TryReadBeUInt64(byte[] image, long offset, long offsetEnd, out ulong value)
        {
            if (offset + 8 <= offsetEnd)
            {
                var span = image.AsSpan((int) offset, 8);
                value = BinaryPrimitives.ReadUInt64BigEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 64-bit signed big endian value from the given offset from the start 
        /// of the <paramref name="image"/>.
        /// </summary>
        /// <param name="image">Byte array to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <returns>The value read.</returns>
        public static long ReadBeInt64(byte[] image, long offset)
        {
            var span = image.AsSpan((int) offset, 8);
            return BinaryPrimitives.ReadInt64BigEndian(span);
        }

        /// <summary>
        /// Reads a 64-bit signed little endian value from the given offset from the start 
        /// of the <paramref name="image"/>.
        /// </summary>
        /// <param name="image">Byte array to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <param name="value">Value read.</param>
        /// <returns>True if the read was successful; false if not.
        /// </returns>
        public static bool TryReadLeInt64(byte[] image, long offset, out long value)
        {
            return TryReadLeInt64(image, offset, image.Length, out value);
        }

        /// <summary>
        /// Reads a 64-bit signed big endian value from the given offset from the start 
        /// of the <paramref name="image"/>.
        /// </summary>
        /// <param name="image">Byte array to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <param name="offsetEnd">End of area from which to read.</param>
        /// <param name="value">Value read.</param>
        /// <returns>True if the read was successful; false if not.
        /// </returns>
        public static bool TryReadLeInt64(byte[] image, long offset, long offsetEnd, out long value)
        {
            if (offset + 8 <= offsetEnd)
            {
                var span = image.AsSpan((int) offset, 8);
                value = BinaryPrimitives.ReadInt64LittleEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 64-bit unsigned little endian value from the given offset from the start 
        /// of the <paramref name="image"/>.
        /// </summary>
        /// <param name="image">Byte array to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <param name="value">Value read.</param>
        /// <returns>True if the read was successful; false if not.
        /// </returns>
        public static bool TryReadLeUInt64(byte[] image, long offset, out ulong value)
        {
            return TryReadLeUInt64(image, offset, image.Length, out value);
        }

        /// <summary>
        /// Reads a 64-bit unsigned little endian value from the given offset from the start 
        /// of the <paramref name="image"/>.
        /// </summary>
        /// <param name="image">Byte array to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <param name="offsetEnd">End of area from which to read.</param>
        /// <param name="value">Value read.</param>
        /// <returns>True if the read was successful; false if not.
        /// </returns>
        public static bool TryReadLeUInt64(byte[] image, long offset, long offsetEnd, out ulong value)
        {
            if (offset + 8 <= offsetEnd)
            {
                var span = image.AsSpan((int) offset, 8);
                value = BinaryPrimitives.ReadUInt64LittleEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 64-bit unsigned little endian value from the given offset from the start 
        /// of the <paramref name="image"/>.
        /// </summary>
        /// <param name="image">Byte array to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <returns>The value read.</returns>
        public static long ReadLeInt64(byte[] image, long offset)
        {
            var span = image.AsSpan((int) offset, 8);
            return BinaryPrimitives.ReadInt64LittleEndian(span);
        }

        //$REVIEW: consider making this an extension method hosted in x86.

        /// <summary>
        /// Reads a 80-bit floating point big endian value from the given offset from the start 
        /// of the <paramref name="image"/>.
        /// </summary>
        /// <param name="image">Byte array to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <param name="value">Value read.</param>
        /// <returns>True if the value was read successfull; otherwise false.</returns>
        public static bool TryReadLeReal80(byte[] image, long offset, out Float80 value)
        {
            if (!TryReadLeUInt64(image, offset, out ulong significand) ||
                !TryReadLeUInt16(image, (uint)offset + 8, out ushort expsign))
            {
                value = default;
                return false;
            }
            value = new Float80(expsign, significand);
            return true;
        }

        /// <summary>
        /// Reads a 32-bit signed big endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static int ReadBeInt32(byte[] abImage, long offset)
        {
            var span = abImage.AsSpan((int) offset, 4);
            return BinaryPrimitives.ReadInt32BigEndian(span);
        }

        /// <summary>
        /// Reads a 32-bit signed big endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadBeInt32(byte[] abImage, long offset, out int value)
        {
            return TryReadBeInt32(abImage, offset, abImage.Length, out value);
        }

        /// <summary>
        /// Reads a 32-bit signed big endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="offsetEnd">End offset of memory area to read.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadBeInt32(byte[] abImage, long offset, long offsetEnd, out int value)
        {
            if (offset <= offsetEnd - 4)
            {
                var span = abImage.AsSpan((int) offset, 4);
                value = BinaryPrimitives.ReadInt32BigEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        /// <summary>
        /// Reads a 32-bit signed little endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadLeInt32(byte[] abImage, long offset, out int value)
        {
            return TryReadLeInt32(abImage, offset, abImage.Length, out value);
        }

        /// <summary>
        /// Reads a 32-bit signed little endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="offsetEnd">End offset of memory area to read.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadLeInt32(byte [] abImage, long offset, long offsetEnd, out int value)
        {
            if (offset <= offsetEnd - 4)
            {
                var span = abImage.AsSpan((int) offset, 4);
                value = BinaryPrimitives.ReadInt32LittleEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 32-bit unsigned little endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadLeUInt32(byte[] abImage, long offset, out uint value)
        {
            return TryReadLeUInt32(abImage, offset, abImage.Length, out value);
        }

        /// <summary>
        /// Reads a 32-bit unsigned little endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="offsetEnd">End offset of memory area to read.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadLeUInt32(byte[] abImage, long offset, long offsetEnd, out uint value)
        {
            if (offset <= offsetEnd - 4)
            {
                var span = abImage.AsSpan((int) offset, 4);
                value = BinaryPrimitives.ReadUInt32LittleEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 32-bit unsigned little big value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadBeUInt32(byte[] abImage, long offset, out uint value)
        {
            return TryReadBeUInt32(abImage, offset, abImage.Length, out value);
        }

        /// <summary>
        /// Reads a 32-bit unsigned big endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="offsetEnd">End offset of memory area to read.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadBeUInt32(byte[] abImage, long offset, long offsetEnd, out uint value)
        {
            if (offset <= offsetEnd - 4)
            {
                var span = abImage.AsSpan((int) offset, 4);
                value = BinaryPrimitives.ReadUInt32BigEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 32-bit signed little endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static int ReadLeInt32(byte[] abImage, long offset)
        {
            var span = abImage.AsSpan((int) offset, 4);
            return BinaryPrimitives.ReadInt32LittleEndian(span);
        }

        /// <summary>
        /// Reads a 16-bit signed big endian value from the given offset from the start
        /// of the <paramref name="img"/>.
        /// </summary>
        /// <param name="img">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadBeInt16(byte[] img, long offset, out short value)
        {
            return TryReadBeInt16(img, offset, img.Length, out value);
        }

        /// <summary>
        /// Reads a 16-bit signed big endian value from the given offset from the start
        /// of the <paramref name="img"/>.
        /// </summary>
        /// <param name="img">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="offsetEnd">End offset of memory area to read.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadBeInt16(byte[] img, long offset, long offsetEnd, out short value)
        {
            if (offset <= offsetEnd - 2)
            {
                var span = img.AsSpan((int) offset, 2);
                value = BinaryPrimitives.ReadInt16BigEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 16-bit unsigned big endian value from the given offset from the start
        /// of the <paramref name="img"/>.
        /// </summary>
        /// <param name="img">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadBeUInt16(byte[] img, long offset, out ushort value)
        {
            return TryReadBeUInt16(img, offset, img.Length, out value);
        }

        /// <summary>
        /// Reads a 16-bit unsigned big endian value from the given offset from the start
        /// of the <paramref name="img"/>.
        /// </summary>
        /// <param name="img">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="offsetEnd">End offset of memory area to read.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadBeUInt16(byte[] img, long offset, long offsetEnd, out ushort value)
        {
            if (offset <= offsetEnd - 2)
            {
                var span = img.AsSpan((int) offset, 2);
                value = BinaryPrimitives.ReadUInt16BigEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 16-bit signed little endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="s">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadLeInt16(byte[] abImage, long offset, out short s)
        {
            return TryReadLeInt16(abImage, offset, abImage.Length, out s);
        }

        /// <summary>
        /// Reads a 16-bit signed big endian value from the given offset from the start
        /// of the <paramref name="img"/>.
        /// </summary>
        /// <param name="img">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="offsetEnd">End offset of memory area to read.</param>
        /// <param name="value">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadLeInt16(byte[] img, long offset, long offsetEnd, out short value)
        {
            if (offset <= offsetEnd - 2)
            {
                var span = img.AsSpan((int) offset, 2);
                value = BinaryPrimitives.ReadInt16LittleEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Reads a 16-bit signed big endian value from the given offset from the start
        /// of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static short ReadBeInt16(byte[] bytes, long offset)
        {
            var span = bytes.AsSpan((int) offset, 2);
            return BinaryPrimitives.ReadInt16BigEndian(span);
        }

        /// <summary>
        /// Reads a 16-bit signed little endian value from the given offset from the start
        /// of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static short ReadLeInt16(byte[] bytes, long offset)
        {
            var span = bytes.AsSpan((int) offset, 2);
            return BinaryPrimitives.ReadInt16LittleEndian(span);
        }

        /// <summary>
        /// Reads a 16-bit unsigned little endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="us">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadLeUInt16(byte[] abImage, long offset, out ushort us)
        {
            return TryReadLeUInt16(abImage, offset, abImage.Length, out us);
        }

        /// <summary>
        /// Reads a 16-bit unsigned little endian value from the given offset from the start
        /// of the <paramref name="abImage"/>.
        /// </summary>
        /// <param name="abImage">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="offsetEnd">End offset of memory area to read.</param>
        /// <param name="us">The value read.</param>
        /// <returns>True if a value was successfully read; otherwise false.</returns>
        public static bool TryReadLeUInt16(byte[] abImage, long offset,long offsetEnd, out ushort us)
        {
            if (offset + 1 >= offsetEnd)
            {
                us = 0;
                return false;
            }
            us = (ushort)(abImage[offset] + ((ushort)abImage[offset + 1] << 8));
            return true;
        }

        /// <summary>
        /// Reads a big endian double precision floating point value from 
        /// the given offset from the start of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static Constant ReadBeDouble(byte[] bytes, long offset)
        {
            return Constant.DoubleFromBitpattern(ReadBeInt64(bytes, offset));
        }

        /// <summary>
        /// Reads a little endian double precision floating point value from 
        /// the given offset from the start of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static Constant ReadLeDouble(byte[] bytes, long offset)
        {
            return Constant.DoubleFromBitpattern(ReadLeInt64(bytes, offset));
        }

        /// <summary>
        /// Reads a big endian single precision floating point value from 
        /// the given offset from the start of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static Constant ReadBeFloat(byte[] bytes, long offset)
        {
            return Constant.FloatFromBitpattern(ReadBeInt32(bytes, offset));
        }

        /// <summary>
        /// Reads a little endian single precision floating point value from 
        /// the given offset from the start of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static Constant ReadLeFloat(byte[] bytes, long offset)
        {
            return Constant.FloatFromBitpattern(ReadLeInt32(bytes, offset));
        }

        /// <summary>
        /// Reads a 64-bit big endian unsigned integer from the
        /// given offset from the start of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static ulong ReadBeUInt64(byte[] bytes, long offset)
        {
            return (ulong)ReadBeInt64(bytes, offset);
        }

        /// <summary>
        /// Reads a 64-bit little endian unsigned integer from the
        /// given offset from the start of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static ulong ReadLeUInt64(byte[] bytes, long offset)
        {
            return (ulong)ReadLeInt64(bytes, offset);
        }

        /// <summary>
        /// Reads a 32-bit big endian unsigned integer from the
        /// given offset from the start of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static uint ReadBeUInt32(byte[] bytes, long offset)
        {
            return (uint)ReadBeInt32(bytes, offset);
        }

        /// <summary>
        /// Reads a 32-bit little endian unsigned integer from the
        /// given offset from the start of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static uint ReadLeUInt32(byte[] bytes, long offset)
        {
            return (uint)ReadLeInt32(bytes, offset);
        }

        /// <summary>
        /// Reads a 16-bit big endian unsigned integer from the
        /// given offset from the start of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static ushort ReadBeUInt16(byte[] bytes, long offset)
        {
            return (ushort) ReadBeInt16(bytes, offset);
        }

        /// <summary>
        /// Reads a 16-bit big endian unsigned integer from the
        /// given offset from the start of the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes">Bytes to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public static ushort ReadLeUInt16(byte[] bytes, long offset)
        {
            return (ushort)ReadLeInt16(bytes, offset);
        }

        /// <summary>
        /// Reads a byte from the given offset from the start of the <paramref name="img"/>.
        /// </summary>
        /// <param name="img">Bytes to read from.</param>
        /// <param name="off">Offset to read from.</param>
        /// <param name="b">Byte read.</param>
        /// <returns>True if the offset was valid; otherwise false.</returns>
        public static bool TryReadByte(byte[] img, long off, out byte b)
        {
            if (off >= img.Length)
            {
                b = 0;
                return false;
            }
            else
            {
                b = img[off];
                return true;
            }
        }

        /// <summary>
        /// Reads bytes from the given offset.
        /// </summary>
        /// <param name="off">Offset from which to read.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="membuf">Destination of the read bytes.</param>
        /// <returns>True if arguments were in range; otherwise false.</returns>
        public bool TryReadBytes(long off, int length, byte[] membuf)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (off + (long)length <= (long) this.Length)
            {
                int s = (int)off;
                int d = 0;
                while (d < length)
                {
                    membuf[d] = Bytes[s];
                    ++s;
                    ++d;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reads a 32-bit big endian signed integer from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public int ReadBeInt32(uint off) { return ReadBeInt32(this.Bytes, off); }

        /// <summary>
        /// Reads a 32-bit big endian unsigned integer from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public uint ReadBeUInt32(uint off) { return ReadBeUInt32(this.Bytes, off); }

        /// <summary>
        /// Reads a 16-bit big endian signed integer from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public short ReadBeInt16(uint off) { return ReadBeInt16(this.Bytes, off); }

        /// <summary>
        /// Reads a 16-bit big endian unsigned integer from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public ushort ReadBeUInt16(uint off) { return ReadBeUInt16(this.Bytes, off); }

        /// <summary>
        /// Reads a 64-bit little endian double precision floating point value from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public Constant ReadLeDouble(long off) { return ReadLeDouble(Bytes, off); }

        /// <summary>
        /// Reads a 32-bit little endian single precision floating point value from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public Constant ReadLeFloat(long off) { return ReadLeFloat(Bytes, off); }

        /// <summary>
        /// Reads a 64-bit little endian signed integer from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public long ReadLeInt64(uint off) {  return ReadLeInt64(this.Bytes, off); }

        /// <summary>
        /// Reads a 64-bit little endian unsigned integer from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
		public ulong ReadLeUInt64(uint off) { return ReadLeUInt64(this.Bytes, off); }

        /// <summary>
        /// Reads a 32-bit little endian signed integer from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public int ReadLeInt32(uint off) { return ReadLeInt32(this.Bytes, off); }

        /// <summary>
        /// Reads a 32-bit little endian unsigned integer from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public uint ReadLeUInt32(uint off) { return ReadLeUInt32(this.Bytes, off); }

        /// <summary>
        /// Reads a 16-bit little endian signed integer from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public short ReadLeInt16(uint off) { return ReadLeInt16(this.Bytes, off); }

        /// <summary>
        /// Reads a 16-bit little endian unsigned integer from the
        /// given offset.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <returns>The value read.</returns>
        public ushort ReadLeUInt16(uint off) { return ReadLeUInt16(this.Bytes, off); }

        /// <summary>
        /// Reads a byte from the given offset from the start of this memory area.
        /// </summary>
        /// <param name="off">Offset to read from.</param>
        /// <param name="b">The value read.</param>
        /// <returns>True if the offset was valid; otherwise false.</returns>
        public override bool TryReadByte(long off, out byte b) { return TryReadByte(this.Bytes, off, out b); }

        /// <summary>
        /// Reads a 64-bit little endian double precision floating point value from the
        /// given address.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <returns>The value read.</returns>
        public Constant ReadLeDouble(Address addr) { return ReadLeDouble(Bytes, ToOffset(addr)); }

        /// <summary>
        /// Reads a 32-bit little endian single precision floating point value from the
        /// given address.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <returns>The value read.</returns>
        public Constant ReadLeFloat(Address addr) { return ReadLeFloat(Bytes, ToOffset(addr)); }

        /// <summary>
        /// Reads a 64-bit little endian signed integer from the
        /// given address.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <returns>The value read.</returns>
        public long ReadLeInt64(Address addr) { return ReadLeInt64(this.Bytes, ToOffset(addr)); }

        /// <summary>
        /// Reads a 64-bit little endian unsigned integer from the
        /// given address.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <returns>The value read.</returns>
        public ulong ReadLeUInt64(Address addr) { return ReadLeUInt64(this.Bytes, ToOffset(addr)); }

        /// <summary>
        /// Reads a 32-bit little endian signed integer from the
        /// given address.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <returns>The value read.</returns>
        public int ReadLeInt32(Address addr) { return ReadLeInt32(this.Bytes, ToOffset(addr)); }

        /// <summary>
        /// Reads a 32-bit little endian unsigned integer from the
        /// given address.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <returns>The value read.</returns>
        public uint ReadLeUInt32(Address addr) { return ReadLeUInt32(this.Bytes, ToOffset(addr)); }

        /// <summary>
        /// Reads a 16-bit little endian signed integer from the
        /// given address.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <returns>The value read.</returns>
        public short ReadLeInt16(Address addr) { return ReadLeInt16(this.Bytes, ToOffset(addr)); }

        /// <summary>
        /// Reads a 32-bit little endian unsigned integer from the
        /// given address.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <returns>The value read.</returns>
        public ushort ReadLeUInt16(Address addr) { return ReadLeUInt16(this.Bytes, ToOffset(addr)); }

        /// <summary>
        /// Reads a byte from the given address.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <returns>The value read.</returns>
        public byte ReadByte(Address addr) { return this.Bytes[ToOffset(addr)]; }

        /// <summary>
        /// Reads a byte from the given address.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">The value read</param>
        /// <returns>True if the address was within bounds; otherwise false.</returns>
        public bool TryReadByte(Address addr, out byte b) { return TryReadByte(this.Bytes, ToOffset(addr), out b); }

        /// <summary>
        /// Reads bytes from the given address.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <param name="membuf">Buffer into which to store the bytes.
        /// </param>
        /// <returns>True if the arguments were valid; otherwise false.</returns>
        public bool TryReadBytes(Address addr, int length, byte[] membuf) { return TryReadBytes(ToOffset(addr), length, membuf); }

        private long ToOffset(Address addr)
        {
            return (long) addr.ToLinear() - (long) this.BaseAddress.ToLinear();
        }

        /// <inheritdoc/>
        public override void WriteByte(long offset, byte b)
        {
            Bytes[offset] = b;
        }

        /// <inheritdoc/>
        public override void WriteBeUInt16(long offset, ushort w)
        {
            Bytes[offset + 0] = (byte) w;
            Bytes[offset + 1] = (byte) (w >> 16);
        }

        /// <inheritdoc/>
        public override void WriteLeUInt16(long offset, ushort w)
		{
			Bytes[offset] = (byte) (w & 0xFF);
			Bytes[offset+1] = (byte) (w >> 8);
		}

        /// <inheritdoc/>
        public override void WriteBeUInt32(long offset, uint dw)
        {
            Bytes[offset + 0] = (byte) (dw >> 24);
            Bytes[offset + 1] = (byte) (dw >> 16);
            Bytes[offset + 2] = (byte) (dw >> 8);
            Bytes[offset + 3] = (byte) (dw & 0xFF);
        }

        /// <inheritdoc/>
        public override void WriteLeUInt32(long offset, uint dw)
        {
            Bytes[offset] = (byte) (dw & 0xFF);
            Bytes[offset + 1] = (byte) (dw >> 8);
            Bytes[offset + 2] = (byte) (dw >> 16);
            Bytes[offset + 3] = (byte) (dw >> 24);
        }

        /// <inheritdoc/>
        public override void WriteBeUInt64(long offset, ulong value)
        {
            Bytes[offset + 0] = (byte) (value >> 56);
            Bytes[offset + 1] = (byte) (value >> 48);
            Bytes[offset + 2] = (byte) (value >> 40);
            Bytes[offset + 3] = (byte) (value >> 32);
            Bytes[offset + 4] = (byte) (value >> 24);
            Bytes[offset + 5] = (byte) (value >> 16);
            Bytes[offset + 6] = (byte) (value >> 8);
            Bytes[offset + 7] = (byte) (value);
        }

        /// <inheritdoc/>
        public override void WriteLeUInt64(long offset, ulong dw)
        {
            Bytes[offset] = (byte) dw;
            Bytes[offset + 1] = (byte) (dw >> 8);
            Bytes[offset + 2] = (byte) (dw >> 16);
            Bytes[offset + 3] = (byte) (dw >> 24);
            Bytes[offset + 4] = (byte) (dw >> 32);
            Bytes[offset + 5] = (byte) (dw >> 40);
            Bytes[offset + 6] = (byte) (dw >> 48);
            Bytes[offset + 7] = (byte) (dw >> 56);
        }

        /// <inheritdoc/>
        public static void WriteLeUInt32(byte[] abImage, uint offset, uint dw)
        {
            abImage[offset] = (byte) (dw & 0xFF);
            abImage[offset + 1] = (byte) (dw >> 8);
            abImage[offset + 2] = (byte) (dw >> 16);
            abImage[offset + 3] = (byte) (dw >> 24);
        }

        /// <inheritdoc/>
        public static void WriteBeUInt32(byte[] abImage, uint offset, uint dw)
        {
            abImage[offset] = (byte) (dw >> 24);
            abImage[offset + 1] = (byte) (dw >> 16);
            abImage[offset + 2] = (byte) (dw >> 8);
            abImage[offset + 3] = (byte) dw;
        }

        /// <inheritdoc/>
        public static void WriteLeInt16(byte[] abImage, long offset, short w)
        {
            abImage[offset] = (byte)(w & 0xFF);
            abImage[offset + 1] = (byte)(w >> 8);
        }

        /// <inheritdoc/>
        public void WriteByte(Address addr, byte b) { WriteByte(ToOffset(addr), b); }

        /// <inheritdoc/>
        public void WriteLeUInt16(Address addr, ushort w) { WriteLeUInt16(ToOffset(addr), w); }

        /// <summary>
        /// Writes bytes to the given offset.
        /// </summary>
        /// <param name="srcBytes">Bytes to write.</param>
        /// <param name="offset">Position in the memory area to write to.</param>
        /// <param name="count">Number of bytes to write.</param>
        public void WriteBytes(byte[] srcBytes, long offset, int count)
        {
            WriteBytes(srcBytes, offset, count, this.Bytes);
        }

        /// <summary>
        /// Writes bytes to the given offset in <paramref name="dstBytes"/>.
        /// </summary>
        /// <param name="srcBytes">Bytes to write.</param>
        /// <param name="offset">Position in the <paramref name="dstBytes"/> to write to.</param>
        /// <param name="count">Number of bytes to write.</param>
        /// <param name="dstBytes">Destination byte array to write to.</param>
        public static void WriteBytes(byte[] srcBytes, long offset, int count, byte[] dstBytes)
        {
            Array.Copy(srcBytes, 0, dstBytes, offset, count);
        }
    }
}
