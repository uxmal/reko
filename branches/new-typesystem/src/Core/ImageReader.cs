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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Core
{
	public class ImageReader
	{
		private byte [] img;
		private int offStart;
		private int off;
		private Address addrStart;

		public ImageReader(ProgramImage img, Address addr)
		{
			this.img = img.Bytes;;
			this.addrStart = addr;
			this.off = offStart = addr - img.BaseAddress;
		}

		public ImageReader(ProgramImage img, uint off)
		{
			this.img = img.Bytes;
			this.off = offStart = (int) off;
		}

		public ImageReader(byte [] img, uint off)
		{
			this.img = img;
			this.off = offStart = (int) off;
		}

		public Address Address
		{
			get { return addrStart + (off - offStart); }
		}

		public int Offset 
		{
			get { return off; }
		}

		public int ReadSigned(PrimitiveType w)
		{
			if (w.IsIntegral)
			{
				switch (w.Size)
				{
				case 1: return (sbyte) ReadByte();
				case 2: return ReadShort();
				case 4: return ReadInt();
				default: throw new ArgumentOutOfRangeException();
				}
			}
			throw new ArgumentOutOfRangeException();
		}

		public byte ReadByte()
		{
			byte b = img[off];
			++off;
			return b;
		}

		public int ReadInt()
		{
			return (int) ReadUint();
		}

		public short ReadShort()
		{
			return (short) ReadUShort();
		}

		public uint ReadUint()
		{
			uint u = ProgramImage.ReadUint(img, off);
			off += 4;
			return u;
		}

		public uint ReadUnsigned(PrimitiveType w)
		{
			switch (w.Size)
			{
			case 1: return ReadByte();
			case 2: return ReadUShort();
			case 4: return ReadUint();
			default: throw new ArgumentOutOfRangeException();
			}
		}

		public ushort ReadUShort()
		{
			ushort w = ProgramImage.ReadUShort(img, off);
			off += 2;
			return w;
		}
	}
}

