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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.IO;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Reads bytes and differently sized words sequentially from an 
    /// associated LoadedImage. Concrete derived classes 
    /// <see cref="BeImageReader"/> and <see cref="LeImageReader"/> 
    /// implement big- and little-endian interpretation of byte sequences.
    /// </summary>
    public class ImageReader
    {
        protected MemoryArea image;
        protected byte[] bytes;
		protected long offStart;
		protected long offEnd;
		protected long off;
		protected Address addrStart;

		protected ImageReader(MemoryArea img, Address addr)
        {
            if (img == null)
                throw new ArgumentNullException("img");
            if (addr == null)
                throw new ArgumentNullException("addr");
            long o = addr - img.BaseAddress;
            if (o >= img.Length)
                throw new ArgumentOutOfRangeException("addr", $"Address {addr} is outside of image.");
            this.offStart = o;
            this.offEnd = img.Bytes.Length;
            this.off = offStart;
            this.image = img;
            this.bytes = img.Bytes;
            this.addrStart = addr;
        }

        protected ImageReader(MemoryArea img, Address addrBegin, Address addrEnd)
        {
            if (img == null)
                throw new ArgumentNullException("img");
            if (addrBegin == null)
                throw new ArgumentNullException("addrBegin");
            if (addrEnd == null)
                throw new ArgumentNullException("addrBegin");
            this.offStart = addrBegin - img.BaseAddress;
            // Prevent walking off the end of the bytes.
            this.offEnd = Math.Min(addrEnd - img.BaseAddress, img.Bytes.Length);
            this.off = this.offStart;
            this.image = img;
            this.bytes = img.Bytes;
            this.addrStart = addrBegin;
        }

        protected ImageReader(MemoryArea img, ulong off)
        {
            if (img == null)
                throw new ArgumentNullException("img");
            this.image = img;
            this.bytes = img.Bytes;
            this.addrStart = img.BaseAddress + off;
            this.offStart = (long) off;
            this.offEnd = img.Bytes.Length;
            this.off = offStart;
        }

        protected ImageReader(byte[] img, ulong off)
        {
            this.bytes = img;
            this.offStart =(long) off;
            this.offEnd = img.Length;
            this.off = offStart;
        }

		public ImageReader(byte[] img) : this(img, 0) { }

		public LeImageReader CreateLeReader()
        {
            return new LeImageReader(bytes, (ulong)off)
            {
                image = this.image,
                addrStart = this.addrStart,
                offStart = this.offStart,
                offEnd = this.offEnd,
            };
        }

        public Address Address { get { return addrStart + (off - offStart); } }
        public byte[] Bytes { get { return bytes; } }
        public MemoryArea Image { get { return image; } }
        public long Offset { get { return off; } set { off = value; } }
        public bool IsValid { get { return IsValidOffset(Offset); } }
        public bool IsValidOffset(long offset) { return 0 <= offset && offset < offEnd; }

        public virtual T ReadStruct<T>() where T : struct
        {
            return new StructureReader<T>(this).Read();
        }

        public virtual T ReadAt<T>(long offset, Func<ImageReader, T> action)
        {
            long prevOffset = Offset;
            Offset = offset;
                T result = action.Invoke(this);
            Offset = prevOffset;
            return result;
        }

        public byte ReadByte()
        {
            byte b = bytes[off];
            ++off;
            return b;
        }

        public bool TryReadByte(out byte b)
        {
            if (!IsValidOffset(off))
            {
                b = 0;
                return false;
            }
            b = bytes[off];
            ++off;
            return true;
        }

        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        public byte PeekByte(int offset)
        {
            return bytes[(long)off + offset];
        }

        public sbyte PeekSByte(int offset) { return (sbyte)bytes[(long)off + offset]; }

        public byte[] ReadBytes(int length) { return ReadBytes((uint)length); }

        public byte[] ReadBytes(uint length)
        {
            int avail = Math.Min((int) length, bytes.Length -(int) off);
            if (avail <= 0)
                return new byte[0];
            byte[] dst = new byte[avail];
            Array.Copy(bytes,(int) off, dst, 0, avail);
            Offset += length;
            return dst;
        }

        public int ReadBytes(byte[] dst, int offset, uint length)
        {
            Array.Copy(bytes, (int)off, dst, offset, length);
            Offset += length;
            return (int)length;
        }

        /// <summary>
        /// Reads a chunk of bytes and interprets it in Little-Endian mode.
        /// </summary>
        /// <param name="type">Enough bytes read </param>
        /// <returns>The read value as a <see cref="Constant"/>.</returns>
        public Constant ReadLe(PrimitiveType type)
        {
            Constant c;
            if (image != null)
                c = image.ReadLe(off, type);
            else
                c = MemoryArea.ReadLe(bytes, Offset, type);
            off += (uint)type.Size;
            return c;
        }

        public bool TryReadLe(PrimitiveType dataType, out Constant c)
        {
            bool ret = image.TryReadLe(off, dataType, out c);
            if (ret)
                off += (uint)dataType.Size;
            return ret;
        }

        /// <summary>
        /// Reads a chunk of bytes and interprets it in Big-Endian mode.
        /// </summary>
        /// <param name="type">Enough bytes read </param>
        /// <returns>The read value as a <see cref="Constant"/>.</returns>
        public Constant ReadBe(PrimitiveType type)
        {
            Constant c = image.ReadBe(off, type);
            off += (uint)type.Size;
            return c;
        }

        public bool TryReadBe(PrimitiveType dataType, out Constant c)
        {
            bool ret = image.TryReadBe(off, dataType, out c);
            if (ret)
                off += (uint)dataType.Size;
            return ret;
        }

        public long ReadLeSigned(PrimitiveType w)
        {
            if ((w.Domain & Domain.Integer) != 0)
            {
                switch (w.Size)
                {
                case 1: return (sbyte)ReadByte();
                case 2: return ReadLeInt16();
                case 4: return ReadLeInt32();
                case 8: return ReadLeInt64();
                default: throw new ArgumentOutOfRangeException();
                }
            }
            throw new ArgumentOutOfRangeException();
        }

        public uint ReadLeUnsigned(PrimitiveType w)
        {
            if (w.IsIntegral)
            {
                switch (w.Size)
                {
                case 1: return ReadByte();
                case 2: return ReadLeUInt16();
                case 4: return ReadLeUInt32();
                default: throw new ArgumentOutOfRangeException();
                }
            }
            throw new ArgumentOutOfRangeException();
        }

        public ushort ReadLeUInt16()
        {
            ushort u = MemoryArea.ReadLeUInt16(bytes, off);
            off += 2;
            return u;
        }

        public bool TryReadLeUInt16(out ushort us)
        {
            if (!MemoryArea.TryReadLeUInt16(bytes, (uint)off, out us))
                return false;
            off += 2;
            return true;
        }

        public bool TryReadBeUInt16(out ushort us)
        {
            if (!MemoryArea.TryReadBeUInt16(bytes, (uint)off, out us))
                return false;
            off += 2;
            return true;
        }

        public bool TryReadBeInt16(out short s)
        {
            if (!MemoryArea.TryReadBeInt16(bytes, (uint)off, out s))
                return false;
            off += 2;
            return true;
        }

        public ushort ReadBeUInt16()
        {
            ushort u = MemoryArea.ReadBeUInt16(bytes, (uint)off);
            off += 2;
            return u;
        }

        public short ReadBeInt16() { return (short)ReadBeUInt16(); }
        public short ReadLeInt16() { return (short)ReadLeUInt16(); }

        public ushort PeekLeUInt16(int offset) { return MemoryArea.ReadLeUInt16(bytes, offset + (uint)off); }
        public ushort PeekBeUInt16(int offset) { return MemoryArea.ReadBeUInt16(bytes, offset + (uint) off); }
        public short PeekLeInt16(int offset) { return (short)MemoryArea.ReadLeUInt16(bytes, offset + (uint) off); }
        public short PeekBeInt16(int offset) { return (short)MemoryArea.ReadBeUInt16(bytes, offset + (uint) off); }

        public bool TryPeekByte(int offset, out byte value) { return MemoryArea.TryReadByte(bytes, offset + off, out value); }
        public bool TryPeekBeUInt16(int offset, out ushort value) { return MemoryArea.TryReadBeUInt16(bytes, offset + off, out value); }
        public bool TryPeekBeUInt32(int offset, out uint value) { return MemoryArea.TryReadBeUInt32(bytes, offset + off, out value); }
        public bool TryPeekBeUInt64(int offset, out ulong value) { return MemoryArea.TryReadBeUInt64(bytes, offset + off, out value); }

        public bool TryPeekLeUInt32(int offset, out uint value) { return MemoryArea.TryReadLeUInt32(bytes, offset + off, out value); }
        
        public uint ReadLeUInt32()
        {
            uint u = MemoryArea.ReadLeUInt32(bytes, (uint)off);
            off += 4;
            return u;
        }

        public uint ReadBeUInt32()
        {
            uint u = MemoryArea.ReadBeUInt32(bytes, off);
            off += 4;
            return u;
        }

        public bool TryReadLeInt16(out short i16)
        {
            if (!MemoryArea.TryReadLeInt16(this.bytes, (uint)off, out i16))
                return false;
            off += 2;
            return true;
        }

        public bool TryReadLeInt32(out int i32)
        {
            if (!MemoryArea.TryReadLeInt32(this.bytes, (uint)off, out i32))
                return false;
            off += 4;
            return true;
        }

        public bool TryReadBeInt32(out int i32)
        {
            if (!MemoryArea.TryReadBeInt32(this.bytes,(uint) off, out i32))
                return false;
            off += 4;
            return true;
        }

        public bool TryReadLeUInt32(out uint ui32)
        {
            if (!MemoryArea.TryReadLeUInt32(this.bytes, (uint)off, out ui32))
                return false;
            off += 4;
            return true;
        }

        public bool TryReadBeUInt32(out uint ui32)
        {
            if (!MemoryArea.TryReadBeUInt32(this.bytes, (uint)off, out ui32))
                return false;
            off += 4;
            return true;
        }

        public bool TryReadLeInt64(out long value)
        {
            if (!MemoryArea.TryReadLeInt64(this.bytes, off, out value))
                return false;
            off += 8;
            return true;
        }

        public bool TryReadLeUInt64(out ulong value)
        {
            if (!MemoryArea.TryReadLeUInt64(this.bytes, off, out value))
                return false;
            off += 8;
            return true;
        }

        public bool TryReadBeInt64(out long value)
        {
            if (!MemoryArea.TryReadBeInt64(this.bytes, off, out value))
                return false;
            off += 8;
            return true;
        }

        public bool TryReadBeUInt64(out ulong value)
        {
            if (!MemoryArea.TryReadBeUInt64(this.bytes, off, out value))
                return false;
            off += 8;
            return true;
        }

        public int ReadBeInt32() { return (int)ReadBeUInt32(); }
        public int ReadLeInt32() { return (int)ReadLeUInt32(); }

        public uint PeekLeUInt32(int offset) { return MemoryArea.ReadLeUInt32(bytes, offset + off); }
        public uint PeekBeUInt32(int offset) { return MemoryArea.ReadBeUInt32(bytes, offset + off); }
        public int PeekLeInt32(int offset) { return (int)MemoryArea.ReadLeUInt32(bytes, offset + off); }
        public int PeekBeInt32(int offset) { return (int)MemoryArea.ReadBeUInt32(bytes, offset + off); }

        public ulong ReadLeUInt64()
        {
            ulong u = MemoryArea.ReadLeUInt64(bytes, off);
            off += 8;
            return u;
        }

        public ulong ReadBeUInt64()
        {
            ulong u = MemoryArea.ReadBeUInt64(bytes, off);
            off += 8;
            return u;
        }

        public long ReadBeInt64() { return (long)ReadBeUInt64(); }
        public long ReadLeInt64() { return (long)ReadLeUInt64(); }

        public ulong PeekLeUInt64(int offset) { return MemoryArea.ReadLeUInt64(bytes, off + offset); }
        public ulong PeekBeUInt64(int offset) { return MemoryArea.ReadBeUInt64(bytes, off + offset); }
        public long PeekLeInt64(int offset) { return (long)MemoryArea.ReadLeUInt64(bytes, off + offset); }
        public long PeekBeInt64(int offset) { return (long)MemoryArea.ReadBeUInt64(bytes, off + offset); }


        public long Seek(long offset, SeekOrigin origin = SeekOrigin.Current)
        {
            switch (origin)
            {
            case SeekOrigin.Begin:
                off = offStart + offset;
                break;
            case SeekOrigin.Current:
                off += offset;
                break;
            case SeekOrigin.End:
                off = offEnd + offset;
                break;
            }
            return off;
        }

        public byte[] ReadToEnd()
        {
            var ab = new byte[this.offEnd - Offset];
            Array.Copy(Bytes, (int)Offset, ab, 0, ab.Length);
            off += ab.Length;
            return ab;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = (int)Math.Min(buffer.Length - offset, count);
            bytesRead =  (int)Math.Min(bytesRead, offEnd - offset);
            Array.Copy(bytes, this.off, buffer, offset, bytesRead);
            off += bytesRead;
            return bytesRead;
        }
    }
}