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

namespace Decompiler.Core
{
	/// <summary>
	/// Reads bytes and differently sized words sequentially from an associated ProgramImage.
	/// </summary>
	public class ImageReader
	{
		private ProgramImage image;
		private byte[] img;
		private int offStart;
		private int off;
		private Address addrStart;

		public ImageReader(ProgramImage img, Address addr)
		{
			this.image = img;
			this.img = img.Bytes;
			this.addrStart = addr;
			this.off = offStart = addr - img.BaseAddress;
		}

		public ImageReader(ProgramImage img, uint off)
		{
			this.image = img;
			this.img = img.Bytes;
            this.addrStart = img.BaseAddress + off;
			this.off = offStart = (int) off;
		}

		public ImageReader(byte[] img, uint off)
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


		public byte ReadByte()
		{
			byte b = img[off];
			++off;
			return b;
		}

        /// <summary>
        /// Little-endian read.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
		public Constant ReadLe(PrimitiveType type)
		{
			Constant c = image.ReadLe(off, type);
			off += type.Size;
			return c;
		}

        public Constant ReadBe(PrimitiveType type)
        {
            Constant c = image.ReadBe(off, type);
            off += type.Size;
            return c;
        }

		public short ReadLeInt16()
		{
			return (short) ReadLeUint16();
		}

		public int ReadLeInt32()
		{
			return (int) ReadLeUint32();
		}

		public ushort ReadLeUint16()
		{
			ushort w = ProgramImage.ReadLeUint16(img, off);
			off += 2;
			return w;
		}

		public int ReadLeSigned(PrimitiveType w)
		{
			if (w.IsIntegral)
			{
				switch (w.Size)
				{
				case 1: return (sbyte) ReadByte();
				case 2: return ReadLeInt16();
				case 4: return ReadLeInt32();
				default: throw new ArgumentOutOfRangeException();
				}
			}
			throw new ArgumentOutOfRangeException();
		}

		public uint ReadLeUint32()
		{
			uint u = ProgramImage.ReadLeUint32(img, off);
			off += 4;
			return u;
		}

		public uint ReadLeUnsigned(PrimitiveType w)
		{
			if (w.IsIntegral)
			{
				switch (w.Size)
				{
				case 1: return ReadByte();
				case 2: return ReadLeUint16();
				case 4: return ReadLeUint32();
				default: throw new ArgumentOutOfRangeException();
				}
			}
			throw new ArgumentOutOfRangeException();
		}

        public uint ReadBeUint32()
        {
            uint u = ProgramImage.ReadBeUint32(img, off);
            off += 4;
            return u;
        }

        public ushort ReadBeUint16()
        {
            ushort u = ProgramImage.ReadBeUint16(img, off);
            off += 2;
            return u;
        }

        public short ReadBeInt16()
        {
            return (short)ReadBeUint16();
        }

        public int ReadBeInt32()
        {
            return (int) ReadBeUint32();
        }
    }
}
