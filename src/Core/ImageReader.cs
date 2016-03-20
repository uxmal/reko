#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Reads bytes and differently sized words sequentially from an 
    /// associated LoadedImage. Concrete derived classes 
    /// <see cref="BeImageReader"/> and <see cref="LeImageReader"/> 
    /// implement big- and little-endian interpretation of byte sequences.
    /// </summary>
    public abstract class ImageReader
    {
        private MemoryArea image;
        protected byte[] bytes;
        private long offStart;
        private long offEnd;
        private long off;
        private Address addrStart;

        protected ImageReader(MemoryArea img, Address addr)
        {
            if (img == null)
                throw new ArgumentNullException("img");
            if (addr == null)
                throw new ArgumentNullException("addr");
            long o = addr - img.BaseAddress;
            if (o >= img.Length)
                throw new ArgumentOutOfRangeException("addr", "Address is outside of image.");
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

        public virtual ImageReader Clone()
        {
            ImageReader rdr;
            if (image != null)
            {
                rdr = CreateNew(image, addrStart);
                rdr.off = off;
            }
            else
            {
                rdr = CreateNew(bytes, (ulong)off);
            }
            return rdr;
        }

        public abstract ImageReader CreateNew(byte[] bytes, ulong offset);

        public abstract ImageReader CreateNew(MemoryArea image, Address addr);

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
        public long Offset { get { return off; } set { off = value; } }
        public bool IsValid { get { return IsValidOffset(Offset); } }
        public bool IsValidOffset(long offset) { return 0 <= offset && offset < offEnd; }

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
            byte[] dst = new byte[length];
            Array.Copy(bytes,(int) off, dst, 0, length);
            Offset += length;
            return dst;
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

        public ushort ReadBeUInt16()
        {
            ushort u = MemoryArea.ReadBeUInt16(bytes, (uint)off);
            off += 2;
            return u;
        }

        public short ReadBeInt16() { return (short)ReadBeUInt16(); }
        public short ReadLeInt16() { return (short)ReadLeUInt16(); }

        public ushort PeekLeUInt16(uint offset) { return MemoryArea.ReadLeUInt16(bytes, offset + (uint)off); }
        public ushort PeekBeUInt16(uint offset) { return MemoryArea.ReadBeUInt16(bytes, offset + (uint) off); }
        public short PeekLeInt16(uint offset) { return (short)MemoryArea.ReadLeUInt16(bytes, offset + (uint) off); }
        public short PeekBeInt16(uint offset) { return (short)MemoryArea.ReadBeUInt16(bytes, offset + (uint) off); }

        public bool TryPeekByte(int offset, out byte value) { return MemoryArea.TryReadByte(bytes, offset + off, out value); }
        public bool TryPeekBeUInt16(int offset, out ushort value) { return MemoryArea.TryReadBeUInt16(bytes, offset + off, out value); }
        public bool TryPeekBeUInt32(int offset, out uint value) { return MemoryArea.TryReadBeUInt32(bytes, offset + off, out value); }
        public bool TryPeekBeUInt64(int offset, out ulong value) { return MemoryArea.TryReadBeUInt64(bytes, offset + off, out value); }

        public bool TryPeekLeUInt32(int offset, out uint value) { return MemoryArea.TryReadLeUInt32(bytes, offset + off, out value); }

        public abstract bool TryPeekUInt32(int offset, out uint value);
        
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

        public uint PeekLeUInt32(uint offset) { return MemoryArea.ReadLeUInt32(bytes, offset + off); }
        public uint PeekBeUInt32(uint offset) { return MemoryArea.ReadBeUInt32(bytes, offset + off); }
        public int PeekLeInt32(uint offset) { return (int)MemoryArea.ReadLeUInt32(bytes, offset + off); }
        public int PeekBeInt32(uint offset) { return (int)MemoryArea.ReadBeUInt32(bytes, offset + off); }

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

        public ulong PeekLeUInt64(uint offset) { return MemoryArea.ReadLeUInt64(bytes, off); }
        public ulong PeekBeUInt64(uint offset) { return MemoryArea.ReadBeUInt64(bytes, off); }
        public long PeekLeInt64(uint offset) { return (long)MemoryArea.ReadLeUInt64(bytes, off); }
        public long PeekBeInt64(uint offset) { return (long)MemoryArea.ReadBeUInt64(bytes, off); }

        public abstract short ReadInt16();
        public abstract int ReadInt32();
        public abstract bool TryReadInt32(out int i32);
        public abstract long ReadInt64();
        public abstract bool TryReadInt64(out long value);

        public abstract ushort ReadUInt16();
        public abstract bool TryReadUInt16(out ushort ui16);
        public abstract uint ReadUInt32();
        public abstract bool TryReadUInt32(out uint ui32);
        public abstract ulong ReadUInt64();
        public abstract bool TryReadUInt64(out ulong ui64);

        public abstract short ReadInt16(uint offset);
        public abstract int ReadInt32(uint offset);
        public abstract long ReadInt64(uint offset);

        public abstract ushort ReadUInt16(uint offset);
        public abstract uint ReadUInt32(uint offset);
        public abstract ulong ReadUInt64(uint offset);

        public abstract Constant Read(PrimitiveType dataType);
        public abstract bool TryRead(PrimitiveType dataType, out Constant c);

        /// <summary>
        /// </summary>
        /// <param name="charType"></param>
        /// <returns></returns>
        public bool ReadNullCharTerminator(DataType charType)
        {
            switch (charType.Size)
            {
            case 1: return (char)ReadByte() == 0;
            case 2: return (char)ReadUInt16() == 0;
            default: throw new NotSupportedException(string.Format("Character size {0} not supported.", charType.Size));
            }
        }

        /// <summary>
        /// Reads a NUL-terminated string starting at the current position.
        /// </summary>
        /// <param name="charType"></param>
        /// <returns></returns>
        public StringConstant ReadCString(DataType charType, Encoding encoding)
        {
            int iStart = (int)Offset;
            while (IsValid && !ReadNullCharTerminator(charType))
                ;
            return new StringConstant(
                StringType.NullTerminated(charType),
                encoding.GetString(
                    bytes,
                    iStart,
                    (int)Offset - iStart - 1));
        }

        /// <summary>
        /// Read a character string that is preceded by a character count. 
        /// </summary>
        /// <param name="lengthType"></param>
        /// <param name="charType"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public StringConstant ReadLengthPrefixedString(PrimitiveType lengthType, PrimitiveType charType, Encoding encoding)
        {
            int length = Read(lengthType).ToInt32();
            int iStart = (int)Offset;
            return new StringConstant(
                StringType.LengthPrefixedStringType(charType, lengthType),
                encoding.GetString(
                    bytes,
                    iStart,
                    length * charType.Size));
        }

        public void Seek(int offset)
        {
            off = off + offset;
        }

        public byte[] ReadToEnd()
        {
            var ab = new byte[this.offEnd - Offset];
            Array.Copy(Bytes, (int)Offset, ab, 0, ab.Length);
            return ab;
        }

    }

    /// <summary>
    /// Use this reader when the processor is in Little-Endian mode to read multi-
    /// byte quantities from memory.
    /// </summary>
    public class LeImageReader : ImageReader
    {
        public LeImageReader(byte[] bytes, ulong offset=0) : base(bytes, offset) { }
        public LeImageReader(MemoryArea image, ulong offset) : base(image, offset) { }
        public LeImageReader(MemoryArea image, Address addr) : base(image, addr) { }
        public LeImageReader(MemoryArea image, Address addrBegin, Address addrEnd) : base(image, addrBegin, addrEnd) { }

        public override ImageReader CreateNew(byte[] bytes, ulong offset)
        {
            return new LeImageReader(bytes, offset);
        }

        public override ImageReader CreateNew(MemoryArea image, Address addr)
        {
            return new LeImageReader(image, (uint)(addr - image.BaseAddress));
        }

        public override short ReadInt16() { return ReadLeInt16(); }
        public override int ReadInt32() { return ReadLeInt32(); }
        public override long ReadInt64() { return ReadLeInt64(); }
        public override ushort ReadUInt16() { return ReadLeUInt16(); }
        public override uint ReadUInt32() { return ReadLeUInt32(); }
        public override ulong ReadUInt64() { return ReadLeUInt64(); }
        public override bool TryPeekUInt32(int offset, out uint value) { return TryPeekLeUInt32(offset, out value); }

        public override bool TryReadInt32(out int i32) { return TryReadLeInt32(out i32); }
        public override bool TryReadInt64(out long value) { return TryReadLeInt64(out value); }
        public override bool TryReadUInt16(out ushort value) { return TryReadLeUInt16(out value); }
        public override bool TryReadUInt32(out uint ui32) { return TryReadLeUInt32(out ui32); }
        public override bool TryReadUInt64(out ulong ui64) { return TryReadLeUInt64(out ui64); }

        public override short ReadInt16(uint offset) { return PeekLeInt16(offset); }
        public override int ReadInt32(uint offset) { return PeekLeInt32(offset); }
        public override long ReadInt64(uint offset) { return PeekLeInt64(offset); }
        public override ushort ReadUInt16(uint offset) { return PeekLeUInt16(offset); }
        public override uint ReadUInt32(uint offset) { return PeekLeUInt32(offset); }
        public override ulong ReadUInt64(uint offset) { return PeekLeUInt64(offset); }

        public override Constant Read(PrimitiveType dataType) { return ReadLe(dataType); }
        public override bool TryRead(PrimitiveType dataType, out Constant c) { return TryReadLe(dataType, out c); }

    }

    /// <summary>
    /// Use this reader when the processor is in Big-Endian mode to read multi-
    /// byte quantities from memory.
    /// </summary>
    public class BeImageReader : ImageReader
    {
        public BeImageReader(byte[] bytes, ulong offset) : base(bytes, offset) { }
        public BeImageReader(MemoryArea image, ulong offset) : base(image, offset) { }
        public BeImageReader(MemoryArea image, Address addr) : base(image, addr) { }
        public BeImageReader(MemoryArea image, Address addrBegin, Address addrEnd) : base(image, addrBegin, addrEnd) { }

        public override ImageReader CreateNew(byte[] bytes, ulong offset)
        {
            return new BeImageReader(bytes, offset);
        }

        public override ImageReader CreateNew(MemoryArea image, Address addr)
        {
            return new BeImageReader(image, (uint) (addr - image.BaseAddress));
        }

        public override short ReadInt16() { return ReadBeInt16(); }
        public override int ReadInt32() { return ReadBeInt32(); }
        public override long ReadInt64() { return ReadBeInt64(); }
        public override ushort ReadUInt16() { return ReadBeUInt16(); }
        public override uint ReadUInt32() { return ReadBeUInt32(); }
        public override ulong ReadUInt64() { return ReadBeUInt64(); }
        public override bool TryPeekUInt32(int offset, out uint value) { return TryPeekBeUInt32(offset, out value); }
        public override bool TryReadInt32(out int i32) { return TryReadBeInt32(out i32); }
        public override bool TryReadInt64(out long value) { return TryReadBeInt64(out value); }
        public override bool TryReadUInt16(out ushort ui16) { return TryReadBeUInt16(out ui16); }
        public override bool TryReadUInt32(out uint ui32) { return TryReadBeUInt32(out ui32); }
        public override bool TryReadUInt64(out ulong ui64) { return TryReadBeUInt64(out ui64); }

        public override short ReadInt16(uint offset) { return PeekBeInt16(offset); }
        public override int ReadInt32(uint offset) { return PeekBeInt32(offset); }
        public override long ReadInt64(uint offset) { return PeekBeInt64(offset); }
        public override ushort ReadUInt16(uint offset) { return PeekBeUInt16(offset); }
        public override uint ReadUInt32(uint offset) { return PeekBeUInt32(offset); }
        public override ulong ReadUInt64(uint offset) { return PeekBeUInt64(offset); }

        public override Constant Read(PrimitiveType dataType) { return ReadBe(dataType); }
        public override bool TryRead(PrimitiveType dataType, out Constant c) { return TryReadBe(dataType, out c); }
    }
}