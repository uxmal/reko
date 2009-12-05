/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Core
{
	/// <summary>
	/// Contains the bytes that are present in memory when a program is loaded.
	/// </summary>
	public class ProgramImage
	{
		private byte [] abImage;
		private Address addrBase;				// address of start of image.
		private ImageMap map;
		private RelocationDictionary relocations;

		public ProgramImage(Address addrBase, byte [] ab)
		{
			this.addrBase = addrBase;
			this.abImage = ab;
			this.map = new ImageMap(addrBase, ab.Length);
			this.relocations = new RelocationDictionary();
		}

		public Address BaseAddress
		{
			get { return addrBase; }
			set { addrBase = value; }
		}

		public byte [] Bytes
		{
			get { return abImage; }
		}

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

		public ImageReader CreateReader(Address addr)
		{
			return new ImageReader(this, addr);
		}

		public ImageReader CreateReader(int offset)
		{
			return new ImageReader(this, (uint) offset);
		}

		public ImageReader CreateReader(uint offset)
		{
			return new ImageReader(this, offset);
		}

		/// <summary>
		/// Adds the delta to the ushort at the given offset.
		/// </summary>
		/// <param name="imageOffset"></param>
		/// <param name="delta"></param>
		/// <returns>The new value of the ushort</returns>
		public ushort FixupLeUint16(int imageOffset, ushort delta)
		{
			ushort seg = ReadLeUint16(abImage, imageOffset);
			seg = (ushort) (seg + delta);
			WriteLeUint16(imageOffset, seg);
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
            return IsValidLinearAddress(addr.Linear);
        }

        public bool IsValidLinearAddress(int linearAddr)
        {
			int offset = linearAddr - addrBase.Linear;
			return 0 <= offset && offset < abImage.Length;
        }

		public ImageMap Map
		{
			get { return map; }
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
		public Constant ReadLe(int imageOffset, PrimitiveType type)
		{
			Constant c = relocations[imageOffset];
			if (c != null && c.DataType.Size == type.Size)
				return c;
			switch (type.Size)
			{
			case 1: return new Constant(type, abImage[imageOffset]);
			case 2: return new Constant(type, ReadLeUint16(abImage, imageOffset));
			case 4: return new Constant(type, ReadLeUint32(abImage, imageOffset));
			}
			throw new NotImplementedException(string.Format("Primitive type {0} not supported.", type));
		}

        public Constant ReadBe(int imageOffset, PrimitiveType type)
        {
            Constant c = relocations[imageOffset];
            if (c != null && c.DataType.Size == type.Size)
                return c;
            switch (type.Size)
            {
            case 1: return new Constant(type, abImage[imageOffset]);
            case 2: return new Constant(type, ReadBeUint16(abImage, imageOffset));
            case 4: return new Constant(type, ReadBeUint32(abImage, imageOffset));
            }
            throw new NotImplementedException(string.Format("Primitive type {0} not supported.", type));
        }

		public Constant ReadLeDouble(int off)
		{
			return ReadLeDouble(abImage, off);
		}

		public static Constant ReadLeDouble(byte [] abImage, int off)
		{
			long bits = 
				(abImage[off] |
				((long) abImage[off+1] << 8)  |
				((long) abImage[off+2] << 16) |
				((long) abImage[off+3] << 24) |
				((long) abImage[off+4] << 32) |
				((long) abImage[off+5] << 40) |
				((long) abImage[off+6] << 48) |
				((long) abImage[off+7] << 56));

			return Constant.DoubleFromBitpattern(bits);
		}


		public Constant ReadLeFloat(int off)
		{
			int bits = 
				(abImage[off] |
				((int) abImage[off+1] << 8)  |
				((int) abImage[off+2] << 16)) |
				((int) abImage[off+3]);
			return Constant.FloatFromBitpattern(bits);
		}

        public static int ReadBeInt32(byte[] abImage, int off)
        {
            return (int) ReadBeUint32(abImage, off);
        }

		public static int ReadLeInt32(byte [] abImage, int off)
		{
			int u = abImage[off] | 
				((int) abImage[off+1] << 8) |
				((int) abImage[off+2] << 16) |
				((int) abImage[off+3] << 24);
			return u;
		}

		public int ReadLeInt32(int off)
		{
			return ReadLeInt32(this.abImage, off);
		}

		public uint ReadLeUint32(int off)
		{
			return (uint) ReadLeInt32(abImage, off);
		}

		public uint ReadLeUint32(Address addr)
		{
			return (uint) ReadLeInt32(abImage, addr - addrBase);
		}

        public static uint ReadBeUint32(byte[] abImage, int off)
        {
            uint u =
                ((uint) abImage[off] << 24) |
                ((uint) abImage[off + 1] << 16) |
                ((uint) abImage[off + 2] << 8) |
                abImage[off + 3];
            return u;
        }

		public static uint ReadLeUint32(byte [] img, int off)
		{
			return (uint) ReadLeInt32(img, off);
		}

		public ushort ReadLeUint16(int offset)
		{
			return ReadLeUint16(abImage, offset);
		}

		public static ushort ReadLeUint16(byte[] abImage, int offset)
		{
			ushort w = (ushort) (abImage[offset] + ((ushort) abImage[offset + 1] << 8));
			return w;
		}


        public static ushort ReadBeUint16(byte[] img, int offset)
        {
            ushort w = (ushort) (img[offset] << 8 | img[offset + 1]);
            return w;
        }


		public RelocationDictionary Relocations
		{
			get { return relocations; }
		}

		public void WriteLeUint16(int offset, ushort w)
		{
			abImage[offset] = (byte) (w & 0xFF);
			abImage[offset+1] = (byte) (w >> 8);
		}

        public void WriteBeUint32(int offset, uint dw)
        {
            abImage[offset + 0] = (byte) (dw >> 24);
            abImage[offset + 1] = (byte) (dw >> 16);
            abImage[offset + 2] = (byte) (dw >> 8);
            abImage[offset + 3] = (byte) (dw & 0xFF);
        }

        public void WriteLeUint32(int offset, uint dw)
        {
            abImage[offset] = (byte) (dw & 0xFF);
            abImage[offset + 1] = (byte) (dw >> 8);
            abImage[offset + 2] = (byte) (dw >> 16);
            abImage[offset + 3] = (byte) (dw >> 24);
        }


    }
}
