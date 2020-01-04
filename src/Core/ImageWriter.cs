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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// This class is used to write bytes to arrays of bytes. Its two
    /// subclasses know all about endian conventions.
    /// </summary>
    public abstract class ImageWriter
    {
        public ImageWriter() : this(new byte[16])
        {
        }

        public ImageWriter(byte[] image)
        {
            this.Bytes = image;
            this.Position = 0;
        }

        public ImageWriter(byte[] image, uint offset)
        {
            this.Bytes = image;
            this.Position = (int)offset;
        }

        public ImageWriter(MemoryArea mem, Address addr)
        {
            this.Bytes = mem.Bytes;
            this.Position = (int)(addr - mem.BaseAddress);
            this.MemoryArea = mem;
        }

        public ImageWriter(MemoryArea mem, long offset)
        {
            this.Bytes = mem.Bytes;
            this.Position = (int)offset;
            this.MemoryArea = mem;
        }

        public abstract ImageWriter Clone();

        public byte[] Bytes { get; private set; }
        public MemoryArea MemoryArea { get; protected set; }
        public int Position { get; set; }

        public byte[] ToArray()
        {
            var b = new byte[Position];
            Array.Copy(Bytes, b, Position);
            return b;
        }

        public ImageWriter WriteByte(byte b)
        {
            if (Position >= Bytes.Length)
            {
                int newSize = (Bytes.Length + 1) * 2;
                while (newSize < Position - 1)
                {
                    newSize *= 2;
                }
                var bytes = Bytes;
                Array.Resize(ref bytes, newSize);
                Bytes = bytes;
            }
            Bytes[Position++] = b;
            return this;
        }

        public ImageWriter WriteBytes(byte b, uint count)
        {
            while (count > 0)
            {
                WriteByte(b);
                --count;
            }
            return this;
        }

        public ImageWriter WriteBytes(byte[] bytes)
        {
            foreach (byte b in bytes)
                WriteByte(b);
            return this;
        }


        public ImageWriter WriteBytes(byte[] bytes, uint offset, uint count)
        {
            while (count > 0)
            {
                WriteByte(bytes[offset]);
                ++offset;
                --count;
            }
            return this;
        }

        public ImageWriter WriteBytes(byte[] bytes, int offset, int count)
        {
            while (count > 0)
            {
                WriteByte(bytes[offset]);
                ++offset;
                --count;
            }
            return this;
        }

        public ImageWriter WriteString(string str, Encoding enc)
        {
            WriteBytes(enc.GetBytes(str));
            return this;
        }

        public ImageWriter WriteLeInt16(short us)
        {
            return WriteLeUInt16((ushort) us);
        }

        public ImageWriter WriteLeUInt16(ushort us)
        {
            WriteByte((byte) us);
            WriteByte((byte) (us >> 8));
            return this;
        }

        public ImageWriter WriteBeUInt16(ushort us)
        {
            WriteByte((byte)(us >> 8));
            WriteByte((byte)us);
            return this;
        }

        public ImageWriter WriteBeUInt32(uint offset, uint ui)
        {
            MemoryArea.WriteBeUInt32(Bytes, offset, ui);
            return this;
        }

        public ImageWriter WriteBeUInt32(uint ui)
        {
            WriteByte((byte) (ui >> 24));
            WriteByte((byte) (ui >> 16));
            WriteByte((byte) (ui >> 8));
            WriteByte((byte) ui);
            return this;
        }

        public ImageWriter WriteLeUInt32(uint offset, uint ui)
        {
            Bytes[offset] = (byte)ui;
            Bytes[offset+1] = (byte)(ui >> 8);
            Bytes[offset+2] = (byte)(ui >> 16);
            Bytes[offset+3] = (byte)(ui >> 24);
            return this;
        }

        public abstract ImageWriter WriteUInt16(ushort us);
        public abstract ImageWriter WriteUInt32(uint w);
        public abstract ImageWriter WriteUInt32(uint offset, uint w);
        public abstract ImageWriter WriteUInt64(ulong w);

        public ImageWriter WriteLeUInt32(uint ui)
        {
            WriteLeUInt32((uint) Position, ui);
            Position += 4;
            return this;
        }

        public ImageWriter WriteLeInt32(int i)
        {
            return WriteLeUInt32((uint)i);
        }

        public ImageWriter WriteBeUInt64(ulong qw)
        {
            WriteByte((byte)(qw >> 56));
            WriteByte((byte)(qw >> 48));
            WriteByte((byte)(qw >> 40));
            WriteByte((byte)(qw >> 32));
            WriteByte((byte)(qw >> 24));
            WriteByte((byte)(qw >> 16));
            WriteByte((byte)(qw >> 8));
            WriteByte((byte)qw);
            return this;
        }

        public ImageWriter WriteLeUInt64(uint offset, ulong qw)
        {
            Bytes[offset] = (byte)qw;
            Bytes[offset + 1] = (byte)(qw >> 8);
            Bytes[offset + 2] = (byte)(qw >> 16);
            Bytes[offset + 3] = (byte)(qw >> 24);
            Bytes[offset + 4] = (byte)(qw >> 32);
            Bytes[offset + 5] = (byte)(qw >> 40);
            Bytes[offset + 6] = (byte)(qw >> 48);
            Bytes[offset + 7] = (byte)(qw >> 56);
            return this;
        }

        public ImageWriter WriteLeUInt64(ulong qw)
        {
            WriteLeUInt64((uint)Position, qw);
            Position += 8;
            return this;
        }

        public ImageWriter WriteLeInt64(long qw)
        {
            return WriteLeUInt64((ulong)qw);
        }
    }

    public class BeImageWriter : ImageWriter
    {
        public BeImageWriter()
            : base()
        {
        }

        public BeImageWriter(byte [] image) : base(image)
        {
        }

        public BeImageWriter(byte[] image, uint offset)
            : base(image, offset)
        {
        }

        public BeImageWriter(MemoryArea mem, Address addr) 
            : base(mem, addr)
        {
        }

        public BeImageWriter(MemoryArea mem, long offset)
            : base(mem, offset)
        {
        }

        public override ImageWriter Clone()
        {
            var w = new BeImageWriter(Bytes, (uint) Position);
            w.MemoryArea = this.MemoryArea;
            return w;
        }

        public override ImageWriter WriteUInt16(ushort us) { return WriteBeUInt16(us); }
        public override ImageWriter WriteUInt32(uint w) { return WriteBeUInt32(w); }
        public override ImageWriter WriteUInt32(uint offset, uint w) { return WriteBeUInt32(offset, w); }
        public override ImageWriter WriteUInt64(ulong w) { return WriteBeUInt64(w); }
    }

    public class LeImageWriter : ImageWriter
    {
        public LeImageWriter()
            : base()
        {
        }

        public LeImageWriter(byte [] image) :base(image)
        {
        }

        public LeImageWriter(byte[] image, uint offset)
            : base(image, offset)
        {
        }

        public LeImageWriter(MemoryArea mem, Address addr) 
            : base(mem, addr)
        {
        }

        public LeImageWriter(MemoryArea mem, long offset)
            : base(mem, offset)
        {
        }

        public override ImageWriter Clone()
        {
            var w = new LeImageWriter(Bytes, (uint)Position);
            w.MemoryArea = this.MemoryArea;
            return w;
        }

        public override ImageWriter WriteUInt16(ushort us) { return WriteLeUInt16(us); }
        public override ImageWriter WriteUInt32(uint w) { return WriteLeUInt32(w); }
        public override ImageWriter WriteUInt32(uint offset, uint w) { return WriteLeUInt32(offset, w); }
        public override ImageWriter WriteUInt64(ulong w) { return WriteLeUInt64(w); }
    }
}
