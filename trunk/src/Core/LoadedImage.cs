#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Core
{
	/// <summary>
	/// Contains the bytes that are present in memory when a program is loaded.
	/// </summary>
    /// <remarks>
    /// Currently, the loadedimage is a range of contiguous bytes, but it really
    /// should be a sparse list of bytes for 64-bit address spaces.
    /// </remarks>
	public class LoadedImage
	{
		private byte [] abImage;

		public LoadedImage(Address addrBase, byte [] bytes)
		{
			this.BaseAddress = addrBase;
			this.abImage = bytes;
			this.Relocations = new RelocationDictionary();
		}

        public Address BaseAddress { get; set; }        // Address of start of image.
        public byte[] Bytes { get { return abImage; } }
        public long Length { get { return abImage.Length; } }
        public RelocationDictionary Relocations { get; private set; }

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
            return string.Format("Image {0}{1} - length {2}{3}", "{", BaseAddress, this.Length, "}");
        }

        public BeImageReader CreateBeReader(Address addr)
        {
            return new BeImageReader(this, addr);
        }

        public BeImageReader CreateBeReader(ulong offset)
        {
            return new BeImageReader(this, offset);
        }

        public LeImageReader CreateLeReader(Address addr)
        {
            return new LeImageReader(this, addr);
        }

        public LeImageReader CreateLeReader(ulong offset)
        {
            return new LeImageReader(this, offset);
        }

        public ImageMap CreateImageMap()
        {
            return new ImageMap(BaseAddress, this.Length);
        }

		/// <summary>
		/// Adds the delta to the ushort at the given offset.
		/// </summary>
		/// <param name="imageOffset"></param>
		/// <param name="delta"></param>
		/// <returns>The new value of the ushort</returns>
		public ushort FixupLeUInt16(uint imageOffset, ushort delta)
		{
			ushort seg = ReadLeUInt16(abImage, imageOffset);
			seg = (ushort) (seg + delta);
			WriteLeUInt16(imageOffset, seg);
			return seg;
		}

		private static double IntPow(double b, int e)
		{
			double acc = 1.0;

			while (e != 0)
			{
				if ((e & 1) == 1)
				{
					acc *= b;
					--e;
				}
				else
				{
					b *= b;
					e >>= 1;
				}
			}
			return acc;
		}

		public bool IsValidAddress(Address addr)
		{
			if (addr == null)
				return false;
            return IsValidLinearAddress(addr.ToLinear());
        }

        public bool IsValidLinearAddress(ulong linearAddr)
        {
            if (linearAddr < BaseAddress.ToLinear())
                return false;
            ulong offset = (linearAddr - BaseAddress.ToLinear());
			return offset < (ulong) abImage.Length;
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
		public Constant ReadLe(ulong imageOffset, PrimitiveType type)
		{
			Constant c = Relocations[(uint)imageOffset];
			if (c != null && c.DataType.Size == type.Size)
				return c;
            return ReadLe(abImage, imageOffset, type);
        }

        public static Constant ReadLe(byte[] abImage, ulong imageOffset, PrimitiveType type)
        {
            if (type.Domain == Domain.Real)
            {
                switch (type.Size)
                {
                case 4:return Constant.FloatFromBitpattern(ReadLeInt32(abImage, imageOffset));
                case 8: return Constant.DoubleFromBitpattern(ReadLeInt64(abImage, imageOffset));
                default: throw new InvalidOperationException(string.Format("Real type {0} not supported.", type));
                }
            }
            
			switch (type.Size)
			{
			case 1: return Constant.Create(type, abImage[imageOffset]);
            case 2: return Constant.Create(type, ReadLeUInt16(abImage, imageOffset));
            case 4: return Constant.Create(type, ReadLeUInt32(abImage, imageOffset));
            case 8: return Constant.Create(type, ReadLeUInt64(abImage, imageOffset));
			}
			throw new NotImplementedException(string.Format("Primitive type {0} not supported.", type));
		}

        public Constant ReadBe(ulong imageOffset, PrimitiveType type)
        {
            Constant c = Relocations[(uint)imageOffset];
            if (c != null && c.DataType.Size == type.Size)
                return c;
            if (type.Domain == Domain.Real)
            {
                switch (type.Size)
                {
                case 4:return Constant.FloatFromBitpattern(ReadLeInt32(abImage, imageOffset));
                case 8: return Constant.DoubleFromBitpattern(ReadLeInt64(abImage, imageOffset));
                default: throw new InvalidOperationException(string.Format("Real type {0} not supported.", type));
                }
            }
            switch (type.Size)
            {
            case 1: return Constant.Create(type, abImage[imageOffset]);
            case 2: return Constant.Create(type, ReadBeUInt16(abImage, imageOffset));
            case 4: return Constant.Create(type, ReadBeUInt32(abImage, imageOffset));
            case 8: return Constant.Create(type, ReadBeUInt64(abImage, imageOffset));
            }
            throw new NotImplementedException(string.Format("Primitive type {0} not supported.", type));
        }

        public static bool TryReadBeInt64(byte[] image, ulong off, out long value)
        {
            if (off + 8 <= (ulong)image.Length)
            {
                value =
                    ((long)image[off] << 56) |
                    ((long)image[off + 1] << 48) |
                    ((long)image[off + 2] << 40) |
                    ((long)image[off + 3] << 32) |
                    ((long)image[off + 4] << 24) |
                    ((long)image[off + 5] << 16) |
                    ((long)image[off + 6] << 8) |
                    ((long)image[off + 7]);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static bool TryReadBeUInt64(byte[] image, ulong off, out ulong value)
        {
            if (off + 8 <= (ulong)image.Length)
            {
                value =
                    ((ulong)image[off] << 56) |
                    ((ulong)image[off + 1] << 48) |
                    ((ulong)image[off + 2] << 40) |
                    ((ulong)image[off + 3] << 32) |
                    ((ulong)image[off + 4] << 24) |
                    ((ulong)image[off + 5] << 16) |
                    ((ulong)image[off + 6] << 8) |
                    ((ulong)image[off + 7]);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }
        public static long ReadBeInt64(byte[] image, ulong off)
        {
            return ((long)image[off] << 56) |
                   ((long)image[off + 1] << 48) |
                   ((long)image[off + 2] << 40) |
                   ((long)image[off + 3] << 32) |
                   ((long)image[off + 4] << 24) |
                   ((long)image[off + 5] << 16) |
                   ((long)image[off + 6] << 8) |
                   ((long)image[off + 7]);
        }

        public static bool TryReadLeInt64(byte[] image, ulong off, out long value)
        {
            if (off + 8 <= (ulong)image.Length)
            {
                value =
                    (long)image[off] |
                    ((long)image[off + 1] << 8) |
                    ((long)image[off + 2] << 16) |
                    ((long)image[off + 3] << 24) |
                    ((long)image[off + 4] << 32) |
                    ((long)image[off + 5] << 40) |
                    ((long)image[off + 6] << 48) |
                    ((long)image[off + 7] << 56);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static bool TryReadLeUInt64(byte[] image, ulong off, out ulong value)
        {
            if (off + 8 <= (ulong)image.Length)
            {
                value =
                     (ulong)image[off] |
                    ((ulong)image[off + 1] << 8) |
                    ((ulong)image[off + 2] << 16) |
                    ((ulong)image[off + 3] << 24) |
                    ((ulong)image[off + 4] << 32) |
                    ((ulong)image[off + 5] << 40) |
                    ((ulong)image[off + 6] << 48) |
                    ((ulong)image[off + 7] << 56);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }
        public static long ReadLeInt64(byte[] image, ulong off)
        {
            return 
                (long) image[off] |
                ((long)image[off+1] << 8) |
                ((long)image[off+2] << 16) | 
                ((long)image[off+3] << 24) |
                ((long)image[off+4] << 32) |
                ((long)image[off+5] << 40) |
                ((long)image[off+6] << 48) |
                ((long)image[off+7] << 56);
        }

        public static int ReadBeInt32(byte[] abImage, ulong off)
        {
            int u =
                ((int)abImage[off] << 24) |
                ((int)abImage[off + 1] << 16) |
                ((int)abImage[off + 2] << 8) |
                abImage[off + 3];
            return u;
        }

        public static bool TryReadBeInt32(byte[] abImage, ulong off, out int value)
        {
            if (off <= (ulong)abImage.Length - 4)
            {
                value =
                    ((int)abImage[off] << 24) |
                    ((int)abImage[off + 1] << 16) |
                    ((int)abImage[off + 2] << 8) |
                    abImage[off + 3];
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
                value = abImage[off] |
                   ((int)abImage[off + 1] << 8) |
                   ((int)abImage[off + 2] << 16) |
                   ((int)abImage[off + 3] << 24);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static bool TryReadLeUInt32(byte[] abImage, ulong off, out uint value)
        {
            if ((long) off <= abImage.Length - 4)
            {
                value = abImage[off] |
                   ((uint)abImage[off + 1] << 8) |
                   ((uint)abImage[off + 2] << 16) |
                   ((uint)abImage[off + 3] << 24);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static bool TryReadBeUInt32(byte[] abImage, uint off, out uint value)
        {
            if (off <= abImage.Length - 4)
            {
                value =
                    ((uint)abImage[off] << 24) |
                    ((uint)abImage[off + 1] << 16) |
                    ((uint)abImage[off + 2] << 8) |
                    abImage[off + 3];
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static int ReadLeInt32(byte[] abImage, ulong off)
        {
            int u = abImage[off] |
                ((int)abImage[off + 1] << 8) |
                ((int)abImage[off + 2] << 16) |
                ((int)abImage[off + 3] << 24);
            return u;
        }

        public static bool TryReadBeUInt16(byte[] img, ulong offset, out ushort value)
        {
            if (offset <= (ulong) img.Length - 2)
            {
                value = (ushort)(img[offset] << 8 | img[offset + 1]);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static short ReadBeInt16(byte[] img, ulong offset)
        {
            return (short)(img[offset] << 8 | img[offset + 1]);
        }

        public static short ReadLeInt16(byte[] abImage, ulong offset)
        {
            return (short)(abImage[offset] + ((short)abImage[offset + 1] << 8));
        }

        public static bool TryReadLeUInt16(byte[] abImage, uint offset, out ushort us)
        {
            if (offset + 1 >= abImage.Length)
            {
                us = 0;
                return false;
            }
            us = (ushort)(abImage[offset] + ((ushort)abImage[offset + 1] << 8));
            return true;
        }

        public static Constant ReadLeDouble(byte[] abImage, ulong off)
        {
            return Constant.DoubleFromBitpattern(ReadLeInt64(abImage, off));
        }

        public static Constant ReadLeFloat(byte[] abImage, ulong off)
        {
            return Constant.FloatFromBitpattern(ReadLeInt32(abImage, off));
        }

        public static ulong ReadBeUInt64(byte[] abImage, ulong off)
        {
            return (ulong)ReadBeInt64(abImage, off);
        }

        public static ulong ReadLeUInt64(byte[] img, ulong off)
        {
            return (ulong)ReadLeInt64(img, off);
        }

        public static uint ReadBeUInt32(byte[] abImage, ulong off)
        {
            return (uint)ReadBeInt32(abImage, off);
        }

        public bool TryReadLeUInt32(Address address, out uint dw) { return TryReadLeUInt32(abImage, ToOffset(address), out dw); }
        public bool TryReadLeUInt64(Address address, out ulong dw) { return TryReadLeUInt64(abImage, ToOffset(address), out dw); }

        public static uint ReadLeUInt32(byte[] img, ulong off)
        {
            return (uint)ReadLeInt32(img, off);
        }

        public static ushort ReadBeUInt16(byte[] abImage, ulong off)
        {
            return (ushort) ReadBeInt16(abImage, off);
        }

        public static ushort ReadLeUInt16(byte[] img, ulong off)
        {
            return (ushort)ReadLeInt16(img, off);
        }

        public static bool TryReadByte(byte[] img, ulong off, out byte b)
        {
            if (off >= (ulong)img.Length)
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

        public bool TryReadBytes(ulong off, int length, byte[] membuf)
        {
            if (length < 0)
                throw new ArgumentException("length");
            if (off + (ulong)length <= (ulong) this.Length)
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

        public Constant ReadLeDouble(ulong off) { return ReadLeDouble(abImage, off); }
        public Constant ReadLeFloat(ulong off) { return ReadLeFloat(abImage, off); }
		public long ReadLeInt64(uint off) {  return ReadLeInt64(this.abImage, off); }
		public ulong ReadLeUint64(uint off) { return ReadLeUInt64(this.abImage, off); }
        public int ReadLeInt32(uint off) { return ReadLeInt32(this.abImage, off); }
        public uint ReadLeUInt32(uint off) { return ReadLeUInt32(this.abImage, off); }
        public short ReadLeInt16(uint off) { return ReadLeInt16(this.abImage, off); }
        public ushort ReadLeUInt16(uint off) { return ReadLeUInt16(this.abImage, off); }
        public bool TryReadByte(ulong off, out byte b) { return TryReadByte(this.abImage, off, out b); }

        public Constant ReadLeDouble(Address addr) { return ReadLeDouble(abImage, ToOffset(addr)); }
        public Constant ReadLeFloat(Address addr) { return ReadLeFloat(abImage, ToOffset(addr)); }
        public long ReadLeInt64(Address addr) { return ReadLeInt64(this.abImage, ToOffset(addr)); }
        public ulong ReadLeUint64(Address addr) { return ReadLeUInt64(this.abImage, ToOffset(addr)); }
        public int ReadLeInt32(Address addr) { return ReadLeInt32(this.abImage, ToOffset(addr)); }
        public uint ReadLeUInt32(Address addr) { return ReadLeUInt32(this.abImage, ToOffset(addr)); }
        public short ReadLeInt16(Address addr) { return ReadLeInt16(this.abImage, ToOffset(addr)); }
        public ushort ReadLeUInt16(Address addr) { return ReadLeUInt16(this.abImage, ToOffset(addr)); }
        public bool TryReadByte(Address addr, out byte b) { return TryReadByte(this.abImage, ToOffset(addr), out b); }
        public bool TryReadBytes(Address addr, int length, byte[] membuf) { return TryReadBytes(ToOffset(addr), length, membuf); }

        private ulong ToOffset(Address addr)
        {
            return addr.ToLinear() - this.BaseAddress.ToLinear();
        }

        public void WriteByte(ulong offset, byte b)
        {
            abImage[offset] = b;
        }

        public void WriteLeUInt16(ulong offset, ushort w)
		{
			abImage[offset] = (byte) (w & 0xFF);
			abImage[offset+1] = (byte) (w >> 8);
		}

        public void WriteBeUInt32(ulong offset, uint dw)
        {
            abImage[offset + 0] = (byte) (dw >> 24);
            abImage[offset + 1] = (byte) (dw >> 16);
            abImage[offset + 2] = (byte) (dw >> 8);
            abImage[offset + 3] = (byte) (dw & 0xFF);
        }

        public void WriteLeUInt32(ulong offset, uint dw)
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

        public static void WriteLeInt16(byte[] abImage, ulong offset, short w)
        {
            abImage[offset] = (byte)(w & 0xFF);
            abImage[offset + 1] = (byte)(w >> 8);
        }

        public void WriteByte(Address addr, byte b) { WriteByte(ToOffset(addr), b); }
        public void WriteLeUInt32(Address addr, uint dw) { WriteLeUInt32(ToOffset(addr), dw); }

        public static void WriteBytes(byte[] srcBytes, ulong offset, int count, byte[] dstBytes)
        {
            Array.Copy(srcBytes, 0, dstBytes, (long)offset, count);
        }
    }
}
