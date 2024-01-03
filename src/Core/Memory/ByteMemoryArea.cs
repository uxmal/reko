#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
		public ByteMemoryArea(Address addrBase, byte [] bytes)
            : base(addrBase, bytes.Length, 8, new MemoryFormatter(PrimitiveType.Byte, 1, 16, 2, 1))
		{
			this.Bytes = bytes;
		}

        /// <summary>
        /// The contents of the memory area.
        /// </summary>
        public byte[] Bytes { get; }

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

        public override string ToString()
        {
            return string.Format("Image {0}{1} - length {2} bytes{3}", "{", BaseAddress, this.Length, "}");
        }

        public override EndianImageReader CreateBeReader(Address addr)
        {
            return new BeImageReader(this, addr);
        }

        public override EndianImageReader CreateBeReader(Address addr, long cUnits)
        {
            return new BeImageReader(this, addr, cUnits);
        }

        public override EndianImageReader CreateBeReader(long offset)
        {
            return new BeImageReader(this, offset);
        }

        public override EndianImageReader CreateBeReader(long offsetBegin, long offsetEnd)
        {
            return new BeImageReader(this, offsetBegin, offsetEnd);
        }

        public override BeImageWriter CreateBeWriter(Address addr)
        {
            return new BeImageWriter(this, addr);
        }

        public override BeImageWriter CreateBeWriter(long offset)
        {
            return new BeImageWriter(this, offset);
        }

        public override EndianImageReader CreateLeReader(Address addr)
        {
            return new LeImageReader(this, addr);
        }

        public override EndianImageReader CreateLeReader(Address addr, long cUnits)
        {
            return new LeImageReader(this, addr, cUnits);
        }

        public override EndianImageReader CreateLeReader(long offset)
        {
            return new LeImageReader(this, offset);
        }

        public override EndianImageReader CreateLeReader(long offsetBegin, long offsetEnd)
        {
            return new LeImageReader(this, offsetBegin, offsetEnd);
        }

        public override LeImageWriter CreateLeWriter(Address addr)
        {
            return new LeImageWriter(this, addr);
        }

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

        public Constant? ReadRelocation(long imageOffset)
        {
            return Relocations[BaseAddress.ToLinear() + (ulong) imageOffset];
        }

        public override bool TryReadBeUInt16(long offset, out ushort value)
        {
            return TryReadBeUInt16(Bytes, offset, out value);
        }

        public override bool TryReadBeUInt32(long offset, out uint value)
        {
            return TryReadBeUInt32(Bytes, offset, out value);
        }

        public override bool TryReadBeUInt64(long offset, out ulong value)
        {
            return TryReadBeUInt64(Bytes, offset, out value);
        }


        public override bool TryReadLeInt32(long offset, out int value)
        {
            return TryReadLeInt32(Bytes, (uint)offset, out value);
        }

        public override bool TryReadLeUInt16(long offset, out ushort value)
        {
            return TryReadLeUInt16(Bytes, offset, out value);
        }

        public override bool TryReadLeUInt32(long offset, out uint value)
        {
            return TryReadLeUInt32(Bytes, offset, out value);
        }

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
        /// <param name="type">Size of the word being requested.</param>
        /// <returns>Typed constant from the image.</returns>
        public override bool TryReadLe(long imageOffset, DataType type, out Constant c)
        {
            var rc = ReadRelocation(imageOffset);
            int size = type.Size;
            if (rc != null && rc.DataType.Size == size)
            {
                c = rc;
                return true;
            }
            return TryReadLe(this.Bytes, imageOffset, type, out c);
        }

        public override bool TryReadBe(long imageOffset, DataType type, out Constant c)
        {
            var rc = ReadRelocation(imageOffset);
            if (rc != null && rc.DataType.Size == type.Size)
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

        public static bool TryReadBeInt64(byte[] image, long off, out long value)
        {
            if (off + 8 <= image.Length)
            {
                var span = image.AsSpan((int) off, 8);
                value = BinaryPrimitives.ReadInt64BigEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static bool TryReadBeUInt64(byte[] image, long off, out ulong value)
        {
            if (off + 8 <= image.Length)
            {
                var span = image.AsSpan((int) off, 8);
                value = BinaryPrimitives.ReadUInt64BigEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static long ReadBeInt64(byte[] image, long off)
        {
            var span = image.AsSpan((int) off, 8);
            return BinaryPrimitives.ReadInt64BigEndian(span);
        }

        public static bool TryReadLeInt64(byte[] image, long off, out long value)
        {
            if (off + 8 <= image.Length)
            {
                var span = image.AsSpan((int) off, 8);
                value = BinaryPrimitives.ReadInt64LittleEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static bool TryReadLeUInt64(byte[] image, long off, out ulong value)
        {
            if (off + 8 <= image.Length)
            {
                var span = image.AsSpan((int) off, 8);
                value = BinaryPrimitives.ReadUInt64LittleEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static long ReadLeInt64(byte[] image, long off)
        {
            var span = image.AsSpan((int) off, 8);
            return BinaryPrimitives.ReadInt64LittleEndian(span);
        }

        //$REVIEW: consider making this an extension method hosted in x86.
        public static bool TryReadLeReal80(byte[] image, long off, out Float80 value)
        {
            if (!TryReadLeUInt64(image, off, out ulong significand) ||
                !TryReadLeUInt16(image, (uint)off + 8, out ushort expsign))
            {
                value = default;
                return false;
            }
            value = new Float80(expsign, significand);
            return true;
        }

        public static int ReadBeInt32(byte[] abImage, long off)
        {
            var span = abImage.AsSpan((int) off, 4);
            return BinaryPrimitives.ReadInt32BigEndian(span);
        }

        public static bool TryReadBeInt32(byte[] abImage, long off, out int value)
        {
            if (off <= abImage.Length - 4)
            {
                var span = abImage.AsSpan((int) off, 4);
                value = BinaryPrimitives.ReadInt32BigEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static bool TryReadLeInt32(byte [] abImage, uint off, out int value)
        {
            if (off <= abImage.Length - 4)
            {
                var span = abImage.AsSpan((int) off, 4);
                value = BinaryPrimitives.ReadInt32LittleEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static bool TryReadLeUInt32(byte[] abImage, long off, out uint value)
        {
            if (off <= abImage.Length - 4)
            {
                var span = abImage.AsSpan((int) off, 4);
                value = BinaryPrimitives.ReadUInt32LittleEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static bool TryReadBeUInt32(byte[] abImage, long off, out uint value)
        {
            if ((long)off <= abImage.Length - 4)
            {
                var span = abImage.AsSpan((int) off, 4);
                value = BinaryPrimitives.ReadUInt32BigEndian(span);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static int ReadLeInt32(byte[] abImage, long off)
        {
            var span = abImage.AsSpan((int) off, 4);
            return BinaryPrimitives.ReadInt32LittleEndian(span);
        }


        public static bool TryReadBeInt16(byte[] img, long offset, out short value)
        {
            if (offset <= img.Length - 2)
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

        public static bool TryReadBeUInt16(byte[] img, long offset, out ushort value)
        {
            if (offset <= img.Length - 2)
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

        public static bool TryReadLeInt16(byte[] img, long offset, out short value)
        {
            if (offset <= img.Length - 2)
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

        public static short ReadBeInt16(byte[] img, long offset)
        {
            var span = img.AsSpan((int) offset, 2);
            return BinaryPrimitives.ReadInt16BigEndian(span);
        }

        public static short ReadLeInt16(byte[] abImage, long offset)
        {
            var span = abImage.AsSpan((int) offset, 2);
            return BinaryPrimitives.ReadInt16LittleEndian(span);
        }

        public static bool TryReadLeUInt16(byte[] abImage, long offset, out ushort us)
        {
            if (offset + 1 >= abImage.Length)
            {
                us = 0;
                return false;
            }
            us = (ushort)(abImage[offset] + ((ushort)abImage[offset + 1] << 8));
            return true;
        }

        public static Constant ReadBeDouble(byte[] abImage, long off)
        {
            return Constant.DoubleFromBitpattern(ReadBeInt64(abImage, off));
        }

        public static Constant ReadLeDouble(byte[] abImage, long off)
        {
            return Constant.DoubleFromBitpattern(ReadLeInt64(abImage, off));
        }

        public static Constant ReadBeFloat(byte[] abImage, long off)
        {
            return Constant.FloatFromBitpattern(ReadBeInt32(abImage, off));
        }

        public static Constant ReadLeFloat(byte[] abImage, long off)
        {
            return Constant.FloatFromBitpattern(ReadLeInt32(abImage, off));
        }

        public static ulong ReadBeUInt64(byte[] abImage, long off)
        {
            return (ulong)ReadBeInt64(abImage, off);
        }

        public static ulong ReadLeUInt64(byte[] img, long off)
        {
            return (ulong)ReadLeInt64(img, off);
        }

        public static uint ReadBeUInt32(byte[] abImage, long off)
        {
            return (uint)ReadBeInt32(abImage, off);
        }

        public static uint ReadLeUInt32(byte[] img, long off)
        {
            return (uint)ReadLeInt32(img, off);
        }

        public static ushort ReadBeUInt16(byte[] abImage, long off)
        {
            return (ushort) ReadBeInt16(abImage, off);
        }

        public static ushort ReadLeUInt16(byte[] img, long off)
        {
            return (ushort)ReadLeInt16(img, off);
        }

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

        public int ReadBeInt32(uint off) { return ReadBeInt32(this.Bytes, off); }
        public uint ReadBeUInt32(uint off) { return ReadBeUInt32(this.Bytes, off); }
        public short ReadBeInt16(uint off) { return ReadBeInt16(this.Bytes, off); }
        public ushort ReadBeUInt16(uint off) { return ReadBeUInt16(this.Bytes, off); }

        public Constant ReadLeDouble(long off) { return ReadLeDouble(Bytes, off); }
        public Constant ReadLeFloat(long off) { return ReadLeFloat(Bytes, off); }
		public long ReadLeInt64(uint off) {  return ReadLeInt64(this.Bytes, off); }
		public ulong ReadLeUint64(uint off) { return ReadLeUInt64(this.Bytes, off); }
        public int ReadLeInt32(uint off) { return ReadLeInt32(this.Bytes, off); }
        public uint ReadLeUInt32(uint off) { return ReadLeUInt32(this.Bytes, off); }
        public short ReadLeInt16(uint off) { return ReadLeInt16(this.Bytes, off); }
        public ushort ReadLeUInt16(uint off) { return ReadLeUInt16(this.Bytes, off); }

        public override bool TryReadByte(long off, out byte b) { return TryReadByte(this.Bytes, off, out b); }

        public Constant ReadLeDouble(Address addr) { return ReadLeDouble(Bytes, ToOffset(addr)); }
        public Constant ReadLeFloat(Address addr) { return ReadLeFloat(Bytes, ToOffset(addr)); }
        public long ReadLeInt64(Address addr) { return ReadLeInt64(this.Bytes, ToOffset(addr)); }
        public ulong ReadLeUint64(Address addr) { return ReadLeUInt64(this.Bytes, ToOffset(addr)); }
        public int ReadLeInt32(Address addr) { return ReadLeInt32(this.Bytes, ToOffset(addr)); }
        public uint ReadLeUInt32(Address addr) { return ReadLeUInt32(this.Bytes, ToOffset(addr)); }
        public short ReadLeInt16(Address addr) { return ReadLeInt16(this.Bytes, ToOffset(addr)); }
        public ushort ReadLeUInt16(Address addr) { return ReadLeUInt16(this.Bytes, ToOffset(addr)); }
        public byte ReadByte(Address addr) { return this.Bytes[ToOffset(addr)]; }
        public bool TryReadByte(Address addr, out byte b) { return TryReadByte(this.Bytes, ToOffset(addr), out b); }
        public bool TryReadBytes(Address addr, int length, byte[] membuf) { return TryReadBytes(ToOffset(addr), length, membuf); }

        private long ToOffset(Address addr)
        {
            return (long) addr.ToLinear() - (long) this.BaseAddress.ToLinear();
        }

        public override void WriteByte(long offset, byte b)
        {
            Bytes[offset] = b;
        }

        public override void WriteBeUInt16(long offset, ushort w)
        {
            Bytes[offset + 0] = (byte) w;
            Bytes[offset + 1] = (byte) (w >> 16);
        }

        public override void WriteLeUInt16(long offset, ushort w)
		{
			Bytes[offset] = (byte) (w & 0xFF);
			Bytes[offset+1] = (byte) (w >> 8);
		}

        public override void WriteBeUInt32(long offset, uint dw)
        {
            Bytes[offset + 0] = (byte) (dw >> 24);
            Bytes[offset + 1] = (byte) (dw >> 16);
            Bytes[offset + 2] = (byte) (dw >> 8);
            Bytes[offset + 3] = (byte) (dw & 0xFF);
        }

        public override void WriteLeUInt32(long offset, uint dw)
        {
            Bytes[offset] = (byte) (dw & 0xFF);
            Bytes[offset + 1] = (byte) (dw >> 8);
            Bytes[offset + 2] = (byte) (dw >> 16);
            Bytes[offset + 3] = (byte) (dw >> 24);
        }

        public static void WriteLeUInt32(byte[] abImage, uint offset, uint dw)
        {
            abImage[offset] = (byte) (dw & 0xFF);
            abImage[offset + 1] = (byte) (dw >> 8);
            abImage[offset + 2] = (byte) (dw >> 16);
            abImage[offset + 3] = (byte) (dw >> 24);
        }

        public static void WriteBeUInt32(byte[] abImage, uint offset, uint dw)
        {
            abImage[offset] = (byte) (dw >> 24);
            abImage[offset + 1] = (byte) (dw >> 16);
            abImage[offset + 2] = (byte) (dw >> 8);
            abImage[offset + 3] = (byte) dw;
        }

        public static void WriteLeInt16(byte[] abImage, long offset, short w)
        {
            abImage[offset] = (byte)(w & 0xFF);
            abImage[offset + 1] = (byte)(w >> 8);
        }

        public void WriteByte(Address addr, byte b) { WriteByte(ToOffset(addr), b); }
        public void WriteLeUInt16(Address addr, ushort w) { WriteLeUInt16(ToOffset(addr), w); }

        public void WriteBytes(byte[] srcBytes, long offset, int count)
        {
            WriteBytes(srcBytes, offset, count, this.Bytes);
        }

        public static void WriteBytes(byte[] srcBytes, long offset, int count, byte[] dstBytes)
        {
            Array.Copy(srcBytes, 0, dstBytes, offset, count);
        }
    }
}
