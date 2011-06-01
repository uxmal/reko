#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core
{
    public abstract class ImageWriter
    {
        public ImageWriter() 
        {
            this.Bytes = new byte[16];
            this.Position = 0;
        }

        public ImageWriter(byte[] image)
        {
            this.Bytes = image;
            this.Position = 0;
        }

        public byte[] Bytes { get; private set;}
        public int Position { get; set; }

        public void WriteByte(byte b)
        {
            if (Position >= Bytes.Length)
            {
                var bytes = Bytes;
                Array.Resize<byte>(ref bytes, (Bytes.Length + 1) * 2);
                Bytes = bytes;
            }
            Bytes[Position++] = b;
        }

        public void WriteBytes(byte b, uint count)
        {
            while (count > 0)
            {
                WriteByte(b);
                --count;
            }
        }

        public void WriteBytes(byte[] bytes)
        {
            foreach (byte b in bytes)
                WriteByte(b);
        }

        public void WriteString(string str, Encoding enc)
        {
            WriteBytes(enc.GetBytes(str));
        }

        public void WriteLeUInt16(ushort us)
        {
            WriteByte((byte) us);
            WriteByte((byte) (us >> 8));
        }

        public void WriteBeUInt32(uint offset, uint ui)
        {
            Bytes[offset] = (byte)(ui >> 24);
            Bytes[offset + 1] = (byte)(ui >> 16);
            Bytes[offset + 2] = (byte)(ui >> 8);
            Bytes[offset + 3] = (byte)ui;
        }

        public void WriteLeUInt32(uint offset, uint ui)
        {
            Bytes[offset] = (byte)ui;
            Bytes[offset+1] = (byte)(ui >> 8);
            Bytes[offset+2] = (byte)(ui >> 16);
            Bytes[offset+3] = (byte)(ui >> 24);
        }

        public abstract void WriteUInt32(uint offset, uint w);

    }

    public class BeImageWriter : ImageWriter
    {
        public BeImageWriter()
        {
        }

        public BeImageWriter(byte [] image) : base(image)
        {
        }

        public override void WriteUInt32(uint offset, uint w) { WriteBeUInt32(offset, w); }
    }

    public class LeImageWriter : ImageWriter
    {
        public LeImageWriter()
        {
        }

        public LeImageWriter(byte [] image) :base(image)
        {
        }

        public override void WriteUInt32(uint offset, uint w) { WriteLeUInt32(offset, w); }
    }
}
