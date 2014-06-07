#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using System.Text;

namespace Decompiler.Core
{
    /// <summary>
    /// Reads bytes and differently sized words sequentially from an associated ProgramImage.
    /// </summary>
    public class ImageReader
    {
        private LoadedImage image;
        protected byte[] bytes;
        private uint offStart;
        private uint off;
        private Address addrStart;

        public virtual ImageReader Clone()
        {
            ImageReader rdr;
            if (image != null)
            {
                rdr = CreateNew(image, addrStart);
                off = rdr.off;
            }
            else
            {
                rdr = CreateNew(bytes, off);
            }
            return rdr;
        }

        public virtual ImageReader CreateNew(byte[] bytes, uint offset)
        {
            return new ImageReader(bytes, offset);
        }

        public virtual ImageReader CreateNew(LoadedImage image, Address addr)
        {
            return new ImageReader(image, addr);
        }

        public ImageReader(LoadedImage img, Address addr)
        {
            int o = addr - img.BaseAddress;
            if (o < 0 || o >= img.Bytes.Length)
                throw new ArgumentOutOfRangeException("addr", "Address is outside of image.");
            this.image = img;
            this.bytes = img.Bytes;
            this.addrStart = addr;
            this.off = offStart = (uint)o;
        }

        public ImageReader(LoadedImage img, uint off)
        {
            this.image = img;
            this.bytes = img.Bytes;
            this.addrStart = img.BaseAddress + off;
            this.off = offStart = off;
        }

        public ImageReader(byte[] img, uint off)
        {
            this.bytes = img;
            this.off = offStart = off;
        }

        public LeImageReader CreateLeReader()
        {
            return new LeImageReader(bytes, off)
            {
                image = this.image,
                addrStart = this.addrStart,
                offStart = this.offStart,
            };
        }

        public Address Address { get { return addrStart + (off - offStart); } }
        public byte[] Bytes { get { return bytes; } }
        public uint Offset { get { return off; } set { off = value; } }
        public bool IsValid { get { return IsValidOffset(Offset); } }
        public bool IsValidOffset(uint offset) { return 0 <= offset && offset < bytes.Length; }

        public byte ReadByte()
        {
            byte b = bytes[off];
            ++off;
            return b;
        }

        public sbyte ReadSByte()
        {
            return (sbyte) ReadByte();
        }

        public byte PeekByte(int offset)
        {
            return bytes[off + offset];
        }

        public sbyte PeekSByte(int offset) { return (sbyte) bytes[off + offset]; }

        public byte[] ReadBytes(int length) { return ReadBytes((uint) length); }

        public byte[] ReadBytes(uint length)
        {
            byte[] dst = new byte[length];
            Array.Copy(bytes, off, dst, 0, length);
            Offset += length;
            return dst;
        }

        /// <summary>
        /// Reads a chunk of bytes and interpret it in Little-Endian mode.
        /// </summary>
        /// <param name="type">Enough bytes read </param>
        /// <returns>The read value as a <see cref="Constant"/>.</returns>
        public Constant ReadLe(PrimitiveType type)
        {
            Constant c = image.ReadLe(off, type);
            off += (uint)type.Size;
            return c;
        }

        /// <summary>
        /// Reads a chunk of bytes and interret it in Big-Endian mode.
        /// </summary>
        /// <param name="type">Enough bytes read </param>
        /// <returns>The read value as a <see cref="Constant"/>.</returns>
        public Constant ReadBe(PrimitiveType type)
        {
            Constant c = image.ReadBe(off, type);
            off += (uint)type.Size;
            return c;
        }

        public int ReadLeSigned(PrimitiveType w)
        {
            if (w.IsIntegral)
            {
                switch (w.Size)
                {
                case 1: return (sbyte)ReadByte();
                case 2: return ReadLeInt16();
                case 4: return ReadLeInt32();
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
            ushort u = LoadedImage.ReadLeUInt16(bytes, off);
            off += 2;
            return u;
        }

        public ushort ReadBeUInt16()
        {
            ushort u = LoadedImage.ReadBeUInt16(bytes, off);
            off += 2;
            return u;
        }

        public short ReadBeInt16() { return (short)ReadBeUInt16(); }
        public short ReadLeInt16() { return (short)ReadLeUInt16(); }

        public ushort PeekLeUInt16(uint offset) { return LoadedImage.ReadLeUInt16(bytes, offset + off); }
        public ushort PeekBeUInt16(uint offset) { return LoadedImage.ReadBeUInt16(bytes, offset + off); }
        public short PeekLeInt16(uint offset) { return (short) LoadedImage.ReadLeUInt16(bytes, offset + off); }
        public short PeekBeInt16(uint offset) { return (short) LoadedImage.ReadBeUInt16(bytes, offset + off); }

        public uint ReadLeUInt32()
        {
            uint u = LoadedImage.ReadLeUInt32(bytes, off);
            off += 4;
            return u;
        }

        public uint ReadBeUInt32()
        {
            uint u = LoadedImage.ReadBeUInt32(bytes, off);
            off += 4;
            return u;
        }

        public int ReadBeInt32() { return (int)ReadBeUInt32(); }
        public int ReadLeInt32() { return (int)ReadLeUInt32(); }

        public uint PeekLeUInt32(uint offset) { return LoadedImage.ReadLeUInt32(bytes, offset + off); }
        public uint PeekBeUInt32(uint offset) { return LoadedImage.ReadBeUInt32(bytes, offset + off); }
        public int PeekLeInt32(uint offset) { return (int) LoadedImage.ReadLeUInt32(bytes, offset + off); }
        public int PeekBeInt32(uint offset) { return (int) LoadedImage.ReadBeUInt32(bytes, offset + off); }

        public ulong ReadLeUint64()
        {
            ulong u = LoadedImage.ReadLeUInt64(bytes, off);
            off += 8;
            return u;
        }

        public ulong ReadBeUInt64()
        {
            ulong u = LoadedImage.ReadBeUInt64(bytes, off);
            off += 8;
            return u;
        }

        public long ReadBeInt64() { return (long)ReadBeUInt64(); }
        public long ReadLeInt64() { return (long)ReadLeUint64(); }

        public ulong PeekLeUint64(uint offset) { return LoadedImage.ReadLeUInt64(bytes, off); }
        public ulong PeekBeUInt64(uint offset) { return LoadedImage.ReadBeUInt64(bytes, off); }
        public long PeekLeInt64(uint offset) { return (long)LoadedImage.ReadLeUInt64(bytes, off); }
        public long PeekBeInt64(uint offset) { return (long)LoadedImage.ReadBeUInt64(bytes, off); }

        public virtual short ReadInt16() { throw new NotSupportedException(); }
        public virtual int ReadInt32() { throw new NotSupportedException(); }
        public virtual long ReadInt64() { throw new NotSupportedException(); }

        public virtual ushort ReadUInt16() { throw new NotSupportedException(); }
        public virtual uint ReadUInt32() { throw new NotSupportedException(); }
        public virtual ulong ReadUInt64() { throw new NotSupportedException(); }

        public virtual short ReadInt16(uint offset) { throw new NotSupportedException(); }
        public virtual int ReadInt32(uint offset) { throw new NotSupportedException(); }
        public virtual long ReadInt64(uint offset) { throw new NotSupportedException(); }

        public virtual ushort ReadUInt16(uint offset) { throw new NotSupportedException(); }
        public virtual uint ReadUInt32(uint offset) { throw new NotSupportedException(); }
        public virtual ulong ReadUInt64(uint offset) { throw new NotSupportedException(); }

        public virtual Constant Read(PrimitiveType dataType) { throw new NotSupportedException(); }

        public char ReadChar(DataType charType)
        {
            switch (charType.Size)
            {
            case 1: return (char)ReadByte();
            case 2: return (char)ReadUInt16();
            default: throw new NotSupportedException(string.Format("Character size {0} not supported.", charType.Size));
            }
        }

        /// <summary>
        /// Reads a NUL-terminated string starting at the current position.
        /// </summary>
        /// <param name="charType"></param>
        /// <returns></returns>
        public StringConstant ReadCString(DataType charType)
        {
            return ReadCString(charType, Encoding.GetEncoding("ISO_8859-1"));
        }

        public StringConstant ReadCString(DataType charType, Encoding encoding)
        {
            int iStart = (int)Offset;
            var sb = new StringBuilder();
            for (char ch = ReadChar(charType); ch != 0; ch = ReadChar(charType))
            {
                sb.Append(ch);
            }
            return new StringConstant(charType, encoding.GetString(bytes, iStart, (int)Offset - iStart - 1));
        }

        public StringConstant ReadLengthPrefixedString(PrimitiveType lengthType, PrimitiveType charType)
        {
            int length = Read(lengthType).ToInt32();
            var sb = new StringBuilder();
            for (int i = 0; i < length; ++i)
            {
                sb.Append(ReadChar(charType));
            }
            return new StringConstant(charType, sb.ToString());
        }

        public void Seek(int offset)
        {
            off = (uint)(off + offset);
        }

        public byte[] ReadToEnd()
        {
            var ab = new byte[this.Bytes.Length - Offset];
            Array.Copy(Bytes, Offset, ab, 0, ab.Length);
            return ab;
        }
    }

    /// <summary>
    /// Use this reader when the processor is in Little-Endian mode to read multi-byte quantities from memory.
    /// </summary>
    public class LeImageReader : ImageReader
    {
        public LeImageReader(byte[] bytes, uint offset=0) : base(bytes, offset) { }
        public LeImageReader(LoadedImage image, uint offset) : base(image, offset) { }

        public override ImageReader CreateNew(byte[] bytes, uint offset)
        {
            return new LeImageReader(bytes, offset);
        }

        public override ImageReader CreateNew(LoadedImage image, Address addr)
        {
            return new LeImageReader(image, (uint)(addr - image.BaseAddress));
        }

        public override short ReadInt16() { return ReadLeInt16(); }
        public override int ReadInt32() { return ReadLeInt32(); }
        public override long ReadInt64() { return ReadLeInt64(); }
        public override ushort ReadUInt16() { return ReadLeUInt16(); }
        public override uint ReadUInt32() { return ReadLeUInt32(); }
        public override ulong ReadUInt64() { return ReadLeUint64(); }

        public override short ReadInt16(uint offset) { return PeekLeInt16(offset); }
        public override int ReadInt32(uint offset) { return PeekLeInt32(offset); }
        public override long ReadInt64(uint offset) { return PeekLeInt64(offset); }
        public override ushort ReadUInt16(uint offset) { return PeekLeUInt16(offset); }
        public override uint ReadUInt32(uint offset) { return PeekLeUInt32(offset); }
        public override ulong ReadUInt64(uint offset) { return PeekLeUint64(offset); }

        public override Constant Read(PrimitiveType dataType) { return ReadLe(dataType); }
    }

    /// <summary>
    /// Use this reader when the processor is in Big-Endian mode to read multi-byte quantities from memory.
    /// </summary>
    public class BeImageReader : ImageReader
    {
        public BeImageReader(byte[] bytes, uint offset) : base(bytes, offset) { }
        public BeImageReader(LoadedImage image, uint offset) : base(image, offset) { }

        public override short ReadInt16() { return ReadBeInt16(); }
        public override int ReadInt32() { return ReadBeInt32(); }
        public override long ReadInt64() { return ReadBeInt64(); }
        public override ushort ReadUInt16() { return ReadBeUInt16(); }
        public override uint ReadUInt32() { return ReadBeUInt32(); }
        public override ulong ReadUInt64() { return ReadBeUInt64(); }

        public override short ReadInt16(uint offset) { return PeekBeInt16(offset); }
        public override int ReadInt32(uint offset) { return PeekBeInt32(offset); }
        public override long ReadInt64(uint offset) { return PeekBeInt64(offset); }
        public override ushort ReadUInt16(uint offset) { return PeekBeUInt16(offset); }
        public override uint ReadUInt32(uint offset) { return PeekBeUInt32(offset); }
        public override ulong ReadUInt64(uint offset) { return PeekBeUInt64(offset); }

        public override Constant Read(PrimitiveType dataType) { return ReadBe(dataType); }
    }
}