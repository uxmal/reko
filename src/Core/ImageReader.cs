#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
        private ProgramImage image;
        private byte[] img;
        private uint offStart;
        private uint off;
        private Address addrStart;

        public ImageReader(ProgramImage img, Address addr)
        {
            int o = addr - img.BaseAddress;
            if (o < 0 || o >= img.Bytes.Length)
                throw new ArgumentOutOfRangeException("addr", "Address is outside of image.");
            this.image = img;
            this.img = img.Bytes;
            this.addrStart = addr;
            this.off = offStart = (uint)o;
        }

        public ImageReader(ProgramImage img, uint off)
        {
            this.image = img;
            this.img = img.Bytes;
            this.addrStart = img.BaseAddress + off;
            this.off = offStart = off;
        }

        public ImageReader(byte[] img, uint off)
        {
            this.img = img;
            this.off = offStart = off;
        }

        public Address Address { get { return addrStart + (off - offStart); } }
        public uint Offset { get { return off; } }
        public bool IsValid { get { return IsValidOffset(Offset); } }
        public bool IsValidOffset(uint offset) { return 0 <= offset && offset < img.Length; }


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
            off += (uint)type.Size;
            return c;
        }

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
            ushort u = ProgramImage.ReadLeUInt16(img, off);
            off += 2;
            return u;
        }

        public ushort ReadBeUInt16()
        {
            ushort u = ProgramImage.ReadBeUInt16(img, off);
            off += 2;
            return u;
        }

        public short ReadBeInt16() { return (short)ReadBeUInt16(); }
        public short ReadLeInt16() { return (short)ReadLeUInt16(); }

        public ushort ReadLeUInt16(uint offset) { return ProgramImage.ReadLeUInt16(img, off); }
        public ushort ReadBeUInt16(uint offset) { return ProgramImage.ReadBeUInt16(img, off); }
        public short ReadLeInt16(uint offset) { return (short)ProgramImage.ReadLeUInt16(img, off); }
        public short ReadBeInt16(uint offset) { return (short)ProgramImage.ReadBeUInt16(img, off); }



        public uint ReadLeUInt32()
        {
            uint u = ProgramImage.ReadLeUInt32(img, off);
            off += 4;
            return u;
        }

        public uint ReadBeUInt32()
        {
            uint u = ProgramImage.ReadBeUInt32(img, off);
            off += 4;
            return u;
        }

        public int ReadBeInt32() { return (int)ReadBeUInt32(); }
        public int ReadLeInt32() { return (int)ReadLeUInt32(); }

        public uint ReadLeUInt32(uint offset) { return ProgramImage.ReadLeUInt32(img, off); }
        public uint ReadBeUInt32(uint offset) { return ProgramImage.ReadBeUInt32(img, off); }
        public int ReadLeInt32(uint offset) { return (int)ProgramImage.ReadLeUInt32(img, off); }
        public int ReadBeInt32(uint offset) { return (int)ProgramImage.ReadBeUInt32(img, off); }



        public ulong ReadLeUint64()
        {
            ulong u = ProgramImage.ReadLeUInt64(img, off);
            off += 8;
            return u;
        }

        public ulong ReadBeUInt64()
        {
            ulong u = ProgramImage.ReadBeUInt64(img, off);
            off += 8;
            return u;
        }

        public long ReadBeInt64() { return (long)ReadBeUInt64(); }
        public long ReadLeInt64() { return (long)ReadLeUint64(); }

        public ulong ReadLeUint64(uint offset) { return ProgramImage.ReadLeUInt64(img, off); }
        public ulong ReadBeUInt64(uint offset) { return ProgramImage.ReadBeUInt64(img, off); }
        public long ReadLeInt64(uint offset) { return (long)ProgramImage.ReadLeUInt64(img, off); }
        public long ReadBeInt64(uint offset) { return (long)ProgramImage.ReadBeUInt64(img, off); }

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
            default: throw new NotSupportedException(string.Format("Character size {0} not supported."));
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
            return new StringConstant(charType, encoding.GetString(img, iStart, (int)Offset - iStart - 1));
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
    }

    public class LeImageReader : ImageReader
    {
        public LeImageReader(byte[] bytes, uint offset) : base(bytes, offset) { }
        public LeImageReader(ProgramImage image, uint offset) : base(image, offset) { }
        public override short ReadInt16() { return ReadLeInt16(); }
        public override int ReadInt32() { return ReadLeInt32(); }
        public override long ReadInt64() { return ReadLeInt64(); }
        public override ushort ReadUInt16() { return ReadLeUInt16(); }
        public override uint ReadUInt32() { return ReadLeUInt32(); }
        public override ulong ReadUInt64() { return ReadLeUint64(); }

        public override short ReadInt16(uint offset) { return ReadLeInt16(offset); }
        public override int ReadInt32(uint offset) { return ReadLeInt32(offset); }
        public override long ReadInt64(uint offset) { return ReadLeInt64(offset); }
        public override ushort ReadUInt16(uint offset) { return ReadLeUInt16(offset); }
        public override uint ReadUInt32(uint offset) { return ReadLeUInt32(offset); }
        public override ulong ReadUInt64(uint offset) { return ReadLeUint64(offset); }

        public override Constant Read(PrimitiveType dataType) { return ReadLe(dataType); }
    }

    public class BeImageReader : ImageReader
    {
        public BeImageReader(byte[] bytes, uint offset) : base(bytes, offset) { }
        public override short ReadInt16() { return ReadBeInt16(); }
        public override int ReadInt32() { return ReadBeInt32(); }
        public override long ReadInt64() { return ReadBeInt64(); }
        public override ushort ReadUInt16() { return ReadBeUInt16(); }
        public override uint ReadUInt32() { return ReadBeUInt32(); }
        public override ulong ReadUInt64() { return ReadBeUInt64(); }

        public override short ReadInt16(uint offset) { return ReadBeInt16(offset); }
        public override int ReadInt32(uint offset) { return ReadBeInt32(offset); }
        public override long ReadInt64(uint offset) { return ReadBeInt64(offset); }
        public override ushort ReadUInt16(uint offset) { return ReadBeUInt16(offset); }
        public override uint ReadUInt32(uint offset) { return ReadBeUInt32(offset); }
        public override ulong ReadUInt64(uint offset) { return ReadBeUInt64(offset); }

        public override Constant Read(PrimitiveType dataType) { return ReadBe(dataType); }

    }
}