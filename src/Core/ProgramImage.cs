/* 
 * Copyright (C) 1999-2008 John Källén.
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
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Core
{
	/// <summary>
	/// Contains the bytes that are present in memory when a program is loaded.
	/// </summary>
	/// <remarks>
	///$TODO: this is little endian. Need separate routines for big-endian reader.
	/// </remarks>
	public class ProgramImage
	{
		private byte [] abImage;
		private Address addrBase;				// address of start of image.
		private ImageMap map;

		public ProgramImage(Address addrBase, byte [] ab)
		{
			this.addrBase = addrBase;
			this.abImage = ab;
			this.map = new ImageMap(addrBase, ab.Length);
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
		/// <param name="offset"></param>
		/// <param name="delta"></param>
		/// <returns>The new value of the ushort</returns>
		public ushort FixupUShort(int offset, ushort delta)
		{
			ushort seg = ReadUShort(offset);
			seg = (ushort) (seg + delta);
			WriteUShort(offset, seg);
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
			int offset = addr.Linear - addrBase.Linear;
			return 0 <= offset && offset < abImage.Length;
		}

		public ImageMap Map
		{
			get { return map; }
		}

		public Constant ReadDouble(int off)
		{
			return ReadDouble(abImage, off);
		}

		public static Constant ReadDouble(byte [] abImage, int off)
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


		public Constant ReadFloat(int off)
		{
			int bits = 
				(abImage[off] |
				((int) abImage[off+1] << 8)  |
				((int) abImage[off+2] << 16)) |
				((int) abImage[off+3]);
			return Constant.FloatFromBitpattern(bits);
		}

		public short ReadShort(int off)
		{
			return (short) ReadUShort(off);
		}

		public int ReadInt(int off)
		{
			return ReadInt(abImage, off);
		}

		public static int ReadInt(byte [] abImage, int off)
		{
			int u = abImage[off] | 
				((int) abImage[off+1] << 8) |
				((int) abImage[off+2] << 16) |
				((int) abImage[off+3] << 24);
			return u;
		}


		public uint ReadUint(int off)
		{
			return (uint) ReadInt(off);
		}

		public uint ReadUint(Address addr)
		{
			return (uint) ReadInt(addr - addrBase);
		}

		public static uint ReadUint(byte [] img, int off)
		{
			return (uint) ReadInt(img, off);
		}

		public ushort ReadUShort(int off)
		{
			return ReadUShort(abImage, off);
		}

		public static ushort ReadUShort(byte [] abImage, int off)
		{
			ushort w = (ushort) (abImage[off] + ((ushort) abImage[off+1] << 8));
			return w;
		}


		public void WriteUShort(int offset, ushort w)
		{
			abImage[offset] = (byte) (w & 0xFF);
			abImage[offset+1] = (byte) (w >> 8);
		}

		public void WriteUint(int offset, uint dw)
		{
			abImage[offset] = (byte) (dw & 0xFF);
			abImage[offset + 1] = (byte) (dw >> 8);
			abImage[offset + 2] = (byte) (dw >> 16);
			abImage[offset + 3] = (byte) (dw >> 24);
		}
	}
}

