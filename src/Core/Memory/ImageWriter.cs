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

using System;
using System.Text;

namespace Reko.Core.Memory
{
    /// <summary>
    /// This class is used to write bytes to arrays of bytes. Its two
    /// subclasses know all about endian conventions.
    /// </summary>
    public abstract class ImageWriter
    {
        /// <summary>
        /// Create an empty image writer.
        /// </summary>
        public ImageWriter() : this(new byte[16])
        {
        }

        /// <summary>
        /// Creates an image writer on an array of bytes.
        /// </summary>
        /// <param name="image">Array of bytes.</param>
        public ImageWriter(byte[] image)
        {
            this.Bytes = image;
            this.Position = 0;
        }

        /// <summary>
        /// Creates an image writer with a specific current position.
        /// </summary>
        /// <param name="image">Array of bytes.</param>
        /// <param name="offset">Initial position.</param>
        public ImageWriter(byte[] image, uint offset)
        {
            this.Bytes = image;
            this.Position = (int)offset;
        }

        /// <summary>
        /// Creates an image writer on a <see cref="ByteMemoryArea"/>,
        /// positioned at a specific address.
        /// </summary>
        /// <param name="mem"><see cref="ByteMemoryArea"/> to write to.</param>
        /// <param name="addr">Address at which to start writing.
        /// </param>
        public ImageWriter(ByteMemoryArea mem, Address addr)
        {
            this.Bytes = mem.Bytes;
            this.Position = (int)(addr - mem.BaseAddress);
        }

        /// <summary>
        /// Creates an image writer on a <see cref="ByteMemoryArea"/>,
        /// positioned at a specific offset from the memory area's beginning.
        /// </summary>
        /// <param name="mem"><see cref="ByteMemoryArea"/> to write to.</param>
        /// <param name="offset">Offset from beginning at which to start writing.
        /// </param>
        public ImageWriter(ByteMemoryArea mem, long offset)
        {
            this.Bytes = mem.Bytes;
            this.Position = (int)offset;
        }

        /// <summary>
        /// Clones this image writer.
        /// </summary>
        /// <returns>A copy of the image writer.</returns>
        public abstract ImageWriter Clone();

        /// <summary>
        /// The underlying bytes of this image writer.
        /// </summary>
        public byte[] Bytes { get; private set; }

        /// <summary>
        /// The offset from the beginning of the underlying bytes.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Copy the bytes from position 0 to the current position in
        /// the underlying byte array.
        /// </summary>
        /// <returns>A byte array containing a copy of those bytes.</returns>
        public byte[] ToArray()
        {
            var b = new byte[Position];
            Array.Copy(Bytes, b, Position);
            return b;
        }

        /// <summary>
        /// Writes a single byte to the underlying byte array, 
        /// advancing the position.
        /// </summary>
        /// <param name="b">Byte to write</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
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

        /// <summary>
        /// Writes a single byte <paramref name="count"/> times to the 
        /// underlying byte array, 
        /// advancing the position by <paramref name="count"/>.
        /// </summary>
        /// <param name="b">Byte to write.</param>
        /// <param name="count">Number of times to write the byte.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteBytes(byte b, uint count)
        {
            while (count > 0)
            {
                WriteByte(b);
                --count;
            }
            return this;
        }

        /// <summary>
        /// Writes an array of bytes to the underlying byte array, 
        /// advancing the position.
        /// </summary>
        /// <param name="bytes">Array of bytes to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteBytes(byte[] bytes)
        {
            foreach (byte b in bytes)
                WriteByte(b);
            return this;
        }

        /// <summary>
        /// Writes an array of bytes to the underlying byte array, 
        /// advancing the position.
        /// </summary>
        /// <param name="bytes">Array of bytes to write.</param>
        /// <param name="offset">Offset into <paramref name="bytes"/> from
        /// which to start reading.
        /// </param>
        /// <param name="count">Number of bytes to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
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

        /// <summary>
        /// Writes an array of bytes to the underlying byte array, 
        /// advancing the position.
        /// </summary>
        /// <param name="bytes">Array of bytes to write.</param>
        /// <param name="offset">Offset into <paramref name="bytes"/> from
        /// which to start reading.
        /// </param>
        /// <param name="count">Number of bytes to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
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

        /// <summary>
        /// Writes a string, encoding it using the text encoding 
        /// <paramref name="encoding"/>, to the underlying byte array, 
        /// advancing the position.
        /// </summary>
        /// <param name="str">String to write.</param>
        /// <param name="encoding">Encoding to use on the string.
        /// </param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteString(string str, Encoding encoding)
        {
            WriteBytes(encoding.GetBytes(str));
            return this;
        }

        /// <summary>
        /// Writes a 16-bit little endian signed integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="us">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteLeInt16(short us)
        {
            return WriteLeUInt16((ushort) us);
        }

        /// <summary>
        /// Writes a 16-bit little endian unsigned integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="us">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteLeUInt16(ushort us)
        {
            WriteByte((byte) us);
            WriteByte((byte) (us >> 8));
            return this;
        }

        /// <summary>
        /// Writes a 16-bit big endian unsigned integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="us">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteBeUInt16(ushort us)
        {
            WriteByte((byte)(us >> 8));
            WriteByte((byte)us);
            return this;
        }

        /// <summary>
        /// Writes a 32-bit big endian unsigned integer to the underlying byte array,
        /// a offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset at which to write the value.</param>
        /// <param name="ui">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteBeUInt32(uint offset, uint ui)
        {
            ByteMemoryArea.WriteBeUInt32(Bytes, offset, ui);
            return this;
        }

        /// <summary>
        /// Writes a 32-bit little endian unsigned integer to the underlying byte array,
        /// a offset <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset at which to write the value.</param>
        /// <param name="ui">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteLeUInt32(uint offset, uint ui)
        {
            ByteMemoryArea.WriteLeUInt32(Bytes, offset, ui);
            return this;
        }

        /// <summary>
        /// Writes a 32-bit big endian unsigned integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="ui">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteBeUInt32(uint ui)
        {
            WriteByte((byte) (ui >> 24));
            WriteByte((byte) (ui >> 16));
            WriteByte((byte) (ui >> 8));
            WriteByte((byte) ui);
            return this;
        }

        /// <summary>
        /// Writes a 32-bit little endian unsigned integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="ui">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteLeUInt32(uint ui)
        {
            WriteByte((byte) ui);
            WriteByte((byte) (ui >> 8));
            WriteByte((byte) (ui >> 16));
            WriteByte((byte) (ui >> 24));
            return this;
        }

        /// <summary>
        /// Writes a 16-bit unsigned integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="us">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public abstract ImageWriter WriteUInt16(ushort us);

        /// <summary>
        /// Writes a 32-bit unsigned integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="w">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public abstract ImageWriter WriteUInt32(uint w);

        /// <summary>
        /// Writes a 32-bit unsigned integer to the underlying byte array,
        /// at the given offset.
        /// </summary>
        /// <param name="offset">Offset to write at.</param>
        /// <param name="w">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public abstract ImageWriter WriteUInt32(uint offset, uint w);

        /// <summary>
        /// Writes a 64-bit unsigned integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="w">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public abstract ImageWriter WriteUInt64(ulong w);

        /// <summary>
        /// Writes a 32-bit little endian signed integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="i">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteLeInt32(int i)
        {
            return WriteLeUInt32((uint)i);
        }

        /// <summary>
        /// Writes a 64-bit big endian unsigned integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="qw">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
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

        /// <summary>
        /// Writes a 64-bit unsigned integer to the underlying byte array,
        /// at the given offset.
        /// </summary>
        /// <param name="offset">Offset to write at.</param>
        /// <param name="qw">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteLeUInt64(uint offset, ulong qw)
        {
            var s = Bytes.AsSpan();
            System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(s);
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

        /// <summary>
        /// Writes a 64-bit little endian unsigned integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="qw">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteLeUInt64(ulong qw)
        {
            WriteLeUInt64((uint)Position, qw);
            Position += 8;
            return this;
        }

        /// <summary>
        /// Writes a 64-bit little endian signed integer to the underlying byte array,
        /// updating the position.
        /// </summary>
        /// <param name="qw">Value to write.</param>
        /// <returns>This instance, to simplify fluent programs.
        /// </returns>
        public ImageWriter WriteLeInt64(long qw)
        {
            return WriteLeUInt64((ulong)qw);
        }
    }

    /// <summary>
    /// Big-endian image writer.
    /// </summary>
    public class BeImageWriter : ImageWriter
    {
        /// <summary>
        /// Create an empty big-endian image writer.
        /// </summary>
        public BeImageWriter()
            : base()
        {
        }

        /// <summary>
        /// Create a big-endian image writer on an array of bytes.
        /// </summary>
        /// <param name="image">Image to write to.</param>
        public BeImageWriter(byte [] image) : base(image)
        {
        }

        /// <summary>
        /// Create an empty big-endian image writer.
        /// </summary>
        /// <param name="image">Image to write to.</param>
        /// <param name="offset">Offset into the image at which to start writing.</param>
        public BeImageWriter(byte[] image, uint offset)
            : base(image, offset)
        {
        }

        /// <summary>
        /// Create an empty big-endian image writer.
        /// </summary>
        /// <param name="mem">Memory area to write to.</param>
        /// <param name="addr">Address at which to start writing.</param>
        public BeImageWriter(ByteMemoryArea mem, Address addr) 
            : base(mem, addr)
        {
        }

        /// <summary>
        /// Create an empty big-endian image writer.
        /// </summary>
        /// <param name="mem">Memory area to write to.</param>
        /// <param name="offset">Offset into the image at which to start writing.</param>
        public BeImageWriter(ByteMemoryArea mem, long offset)
            : base(mem, offset)
        {
        }

        /// <inheritdoc/>
        public override ImageWriter Clone()
        {
            var w = new BeImageWriter(Bytes, (uint) Position);
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

        public LeImageWriter(ByteMemoryArea mem, Address addr) 
            : base(mem, addr)
        {
        }

        public LeImageWriter(ByteMemoryArea mem, long offset)
            : base(mem, offset)
        {
        }

        public override ImageWriter Clone()
        {
            var w = new LeImageWriter(Bytes, (uint)Position);
            return w;
        }

        public override ImageWriter WriteUInt16(ushort us) { return WriteLeUInt16(us); }
        public override ImageWriter WriteUInt32(uint w) { return WriteLeUInt32(w); }
        public override ImageWriter WriteUInt32(uint offset, uint w) { return WriteLeUInt32(offset, w); }
        public override ImageWriter WriteUInt64(ulong w) { return WriteLeUInt64(w); }
    }
}
