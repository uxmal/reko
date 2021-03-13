#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Core.Memory
{
    /// <summary>
    /// Reads bytes and differently sized words sequentially from an 
    /// associated <see cref="ByteMemoryArea"/>. Concrete derived classes 
    /// <see cref="BeImageReader"/> and <see cref="LeImageReader"/> 
    /// implement big- and little-endian interpretation of byte sequences.
    /// </summary>
    public class ByteImageReader : ImageReader
    {
        protected MemoryArea? mem;
        protected byte[] bytes;
        protected long offStart;
        protected long offEnd;
        protected long off;
        protected Address? addrStart;

        protected ByteImageReader(ByteMemoryArea mem, Address addr)
        {
            this.mem = mem ?? throw new ArgumentNullException(nameof(mem));
            this.addrStart = addr ?? throw new ArgumentNullException(nameof(addr));
            long o = addr - mem.BaseAddress;
            if (o >= mem.Length)
                throw new ArgumentOutOfRangeException(nameof(addr), $"Address {addr} is outside of image.");
            this.offStart = o;
            this.offEnd = mem.Length;
            this.off = offStart;
            this.bytes = mem.Bytes;
        }

        protected ByteImageReader(ByteMemoryArea img, Address addrBegin, Address addrEnd)
        {
            this.mem = img ?? throw new ArgumentNullException(nameof(img));
            this.addrStart = addrBegin ?? throw new ArgumentNullException(nameof(addrBegin));
            if (addrEnd is null)
                throw new ArgumentNullException(nameof(addrEnd));
            this.offStart = addrBegin - img.BaseAddress;
            // Prevent walking off the end of the bytes.
            this.offEnd = Math.Min(addrEnd - img.BaseAddress, img.Bytes.Length);
            this.off = this.offStart;
            this.bytes = img.Bytes;
        }

        protected ByteImageReader(ByteMemoryArea mem, long off)
        {
            this.mem = mem ?? throw new ArgumentNullException(nameof(mem));
            this.bytes = mem.Bytes;
            this.addrStart = mem.BaseAddress + off;
            this.offStart = off;
            this.offEnd = mem.Bytes.Length;
            this.off = offStart;
        }

        protected ByteImageReader(ByteMemoryArea mem, long offStart, long offEnd)
        {
            this.mem = mem ?? throw new ArgumentNullException(nameof(mem));
            this.bytes = mem.Bytes;
            this.addrStart = mem.BaseAddress + offStart;
            if (offStart < 0 || offStart >= mem.Length)
                throw new ArgumentOutOfRangeException(nameof(offStart), $"Starting offset {offStart} is outside of memory area.");
            this.offStart = offStart;
            this.offEnd = Math.Min(Math.Max(offEnd, offStart), mem.Length);
            this.Offset = offStart;
        }

        public ByteImageReader(byte[] img, long off)
        {
            this.bytes = img;
            this.offStart = off;
            this.offEnd = img.Length;
            this.off = offStart;
        }

        public ByteImageReader(byte[] img) : this(img, 0) { }

        public Address Address { get { return addrStart! + (off - offStart); } }
        public byte[] Bytes { get { return bytes; } }
        public long Offset { get { return off; } set { off = value; } }
        public bool IsValid { get { return IsValidOffset(Offset); } }
        public bool IsValidOffset(long offset) { return 0 <= offset && offset < offEnd; }

        public BinaryReader CreateBinaryReader()
        {
            return new BinaryReader(new MemoryStream(Bytes));
        }

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
            return (sbyte) ReadByte();
        }

        public byte PeekByte(int offset)
        {
            return bytes[(long) off + offset];
        }

        public sbyte PeekSByte(int offset) { return (sbyte)bytes[(long)off + offset]; }

        public byte[] ReadBytes(int length) { return ReadBytes((uint) length); }

        public byte[] ReadBytes(uint length)
        {
            int avail = Math.Min((int) length, bytes.Length - (int) off);
            if (avail <= 0)
                return new byte[0];
            byte[] dst = new byte[avail];
            Array.Copy(bytes, (int) off, dst, 0, avail);
            Offset += length;
            return dst;
        }

        public int ReadBytes(byte[] dst, int offset, uint length)
        {
            Array.Copy(bytes, (int) off, dst, offset, length);
            Offset += length;
            return (int) length;
        }

        /// <summary>
        /// Reads a chunk of bytes and interprets it in Little-Endian mode.
        /// </summary>
        /// <param name="type">Enough bytes read </param>
        /// <returns>The read value as a <see cref="Constant"/>.</returns>
        public bool TryReadLe(PrimitiveType dataType, out Constant c)
        {
            bool ret;
            if (mem is null)
                ret = ByteMemoryArea.TryReadLe(bytes, Offset, dataType, out c);
            else
                ret = mem.TryReadLe(off, dataType, out c);
            if (ret)
                off += (uint) dataType.Size;
            return ret;
        }

        /// <summary>
        /// Reads a chunk of bytes and interprets it in Big-Endian mode.
        /// </summary>
        /// <param name="type">Enough bytes read </param>
        /// <returns>The read value as a <see cref="Constant"/>.</returns>
        public bool TryReadBe(PrimitiveType dataType, out Constant c)
        {
            bool ret = mem!.TryReadBe(off, dataType, out c);
            if (ret)
                off += (uint) dataType.Size;
            return ret;
        }

        public bool TryReadLeSigned(PrimitiveType w, out long value)
        {
            if ((w.Domain & Domain.Integer) != 0)
            {
                bool retval;
                switch (w.Size)
                {
                case 1:
                    retval = TryReadByte(out var b);
                    value = (sbyte) b;
                    return retval;
                case 2:
                    retval = TryReadLeInt16(out var s);
                    value = s;
                    return retval;
                case 4:
                    retval = TryReadLeInt32(out var i);
                    value = i;
                    return retval;
                case 8:
                    return TryReadLeInt64(out value);
                default: throw new ArgumentOutOfRangeException();
                }
            }
            throw new ArgumentOutOfRangeException();
        }

        public ushort ReadLeUInt16()
        {
            ushort u = ByteMemoryArea.ReadLeUInt16(bytes, off);
            off += 2;
            return u;
        }

        public bool TryReadLeUInt16(out ushort us)
        {
            if (!ByteMemoryArea.TryReadLeUInt16(bytes, (uint) off, out us))
                return false;
            off += 2;
            return true;
        }

        public bool TryReadBeUInt16(out ushort us)
        {
            if (!ByteMemoryArea.TryReadBeUInt16(bytes, (uint) off, out us))
                return false;
            off += 2;
            return true;
        }

        public bool TryReadBeInt16(out short s)
        {
            if (!ByteMemoryArea.TryReadBeInt16(bytes, (uint) off, out s))
                return false;
            off += 2;
            return true;
        }

        public ushort ReadBeUInt16()
        {
            ushort u = ByteMemoryArea.ReadBeUInt16(bytes, (uint) off);
            off += 2;
            return u;
        }

        public short ReadBeInt16() { return (short) ReadBeUInt16(); }
        public short ReadLeInt16() { return (short) ReadLeUInt16(); }

        public ushort PeekLeUInt16(int offset) { return ByteMemoryArea.ReadLeUInt16(bytes, offset + (uint) off); }
        public ushort PeekBeUInt16(int offset) { return ByteMemoryArea.ReadBeUInt16(bytes, offset + (uint) off); }
        public short PeekLeInt16(int offset) { return (short) ByteMemoryArea.ReadLeUInt16(bytes, offset + (uint) off); }
        public short PeekBeInt16(int offset) { return (short) ByteMemoryArea.ReadBeUInt16(bytes, offset + (uint) off); }

        public bool TryPeekByte(int offset, out byte value) { return ByteMemoryArea.TryReadByte(bytes, offset + off, out value); }
        public bool TryPeekBeUInt16(int offset, out ushort value) { return ByteMemoryArea.TryReadBeUInt16(bytes, offset + off, out value); }
        public bool TryPeekBeUInt32(int offset, out uint value) { return ByteMemoryArea.TryReadBeUInt32(bytes, offset + off, out value); }
        public bool TryPeekBeUInt64(int offset, out ulong value) { return ByteMemoryArea.TryReadBeUInt64(bytes, offset + off, out value); }

        public bool TryPeekLeUInt16(int offset, out ushort value) { return ByteMemoryArea.TryReadLeUInt16(bytes, offset + off, out value); }
        public bool TryPeekLeUInt32(int offset, out uint value) { return ByteMemoryArea.TryReadLeUInt32(bytes, offset + off, out value); }

        public uint ReadLeUInt32()
        {
            uint u = ByteMemoryArea.ReadLeUInt32(bytes, (uint) off);
            off += 4;
            return u;
        }

        public uint ReadBeUInt32()
        {
            uint u = ByteMemoryArea.ReadBeUInt32(bytes, off);
            off += 4;
            return u;
        }

        public bool TryReadLeInt16(out short i16)
        {
            if (!ByteMemoryArea.TryReadLeInt16(this.bytes, (uint) off, out i16))
                return false;
            off += 2;
            return true;
        }

        public bool TryReadLeInt32(out int i32)
        {
            if (!ByteMemoryArea.TryReadLeInt32(this.bytes, (uint) off, out i32))
                return false;
            off += 4;
            return true;
        }

        public bool TryReadBeInt32(out int i32)
        {
            if (!ByteMemoryArea.TryReadBeInt32(this.bytes, (uint) off, out i32))
                return false;
            off += 4;
            return true;
        }

        public bool TryReadLeUInt32(out uint ui32)
        {
            if (!ByteMemoryArea.TryReadLeUInt32(this.bytes, (uint) off, out ui32))
                return false;
            off += 4;
            return true;
        }

        public bool TryReadBeUInt32(out uint ui32)
        {
            if (!ByteMemoryArea.TryReadBeUInt32(this.bytes, (uint) off, out ui32))
                return false;
            off += 4;
            return true;
        }

        public bool TryReadLeInt64(out long value)
        {
            if (!ByteMemoryArea.TryReadLeInt64(this.bytes, off, out value))
                return false;
            off += 8;
            return true;
        }

        public bool TryReadLeUInt64(out ulong value)
        {
            if (!ByteMemoryArea.TryReadLeUInt64(this.bytes, off, out value))
                return false;
            off += 8;
            return true;
        }

        public bool TryReadBeInt64(out long value)
        {
            if (!ByteMemoryArea.TryReadBeInt64(this.bytes, off, out value))
                return false;
            off += 8;
            return true;
        }

        public bool TryReadBeUInt64(out ulong value)
        {
            if (!ByteMemoryArea.TryReadBeUInt64(this.bytes, off, out value))
                return false;
            off += 8;
            return true;
        }

        public int ReadBeInt32() { return (int) ReadBeUInt32(); }
        public int ReadLeInt32() { return (int) ReadLeUInt32(); }

        public uint PeekLeUInt32(int offset) { return ByteMemoryArea.ReadLeUInt32(bytes, offset + off); }
        public uint PeekBeUInt32(int offset) { return ByteMemoryArea.ReadBeUInt32(bytes, offset + off); }
        public int PeekLeInt32(int offset) { return (int) ByteMemoryArea.ReadLeUInt32(bytes, offset + off); }
        public int PeekBeInt32(int offset) { return (int) ByteMemoryArea.ReadBeUInt32(bytes, offset + off); }

        public ulong ReadLeUInt64()
        {
            ulong u = ByteMemoryArea.ReadLeUInt64(bytes, off);
            off += 8;
            return u;
        }

        public ulong ReadBeUInt64()
        {
            ulong u = ByteMemoryArea.ReadBeUInt64(bytes, off);
            off += 8;
            return u;
        }

        public long ReadBeInt64() { return (long) ReadBeUInt64(); }
        public long ReadLeInt64() { return (long) ReadLeUInt64(); }

        public ulong PeekLeUInt64(int offset) { return ByteMemoryArea.ReadLeUInt64(bytes, off + offset); }
        public ulong PeekBeUInt64(int offset) { return ByteMemoryArea.ReadBeUInt64(bytes, off + offset); }
        public long PeekLeInt64(int offset) { return (long) ByteMemoryArea.ReadLeUInt64(bytes, off + offset); }
        public long PeekBeInt64(int offset) { return (long) ByteMemoryArea.ReadBeUInt64(bytes, off + offset); }


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
            Array.Copy(Bytes, (int) Offset, ab, 0, ab.Length);
            off += ab.Length;
            return ab;
        }
        public int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = (int) Math.Min(buffer.Length - offset, count);
            bytesRead = (int) Math.Min(bytesRead, offEnd - offset);
            Array.Copy(bytes, this.off, buffer, offset, bytesRead);
            off += bytesRead;
            return bytesRead;
        }
    }
}
