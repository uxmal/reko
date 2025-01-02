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

#pragma warning disable IDE1006

using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Core.Memory
{
    public interface EndianImageReader : ImageReader
    {
        EndianImageReader Clone();

        EndianImageReader CreateNew(MemoryArea image, Address addr);

        bool ReadNullCharTerminator(DataType dtChar);

        string ReadNulTerminatedString(DataType charType, Encoding encoding);

        StringConstant ReadCString(DataType charType, Encoding encoding);

        short ReadInt16();
        int ReadInt32();
        long ReadInt64();

        ushort ReadUInt16();
        uint ReadUInt32();
        ulong ReadUInt64();

        bool TryPeekUInt32(int offset, out uint value);
        bool TryPeekUInt64(int offset, out ulong value);
        bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant value);
        bool TryReadInt16(out short value);
        bool TryReadInt32(out int value);
        bool TryReadInt64(out long value);
        bool TryReadUInt16(out ushort value);
        bool TryReadUInt32(out uint value);
        bool TryReadUInt64(out ulong value);
    }

    /// <summary>
    /// This byte image reader has an associated notion of endianness. Abstract methods are provided for reading
    /// values that change bitpatterns depending on endianness.
    /// </summary>
	public abstract class EndianByteImageReader : ByteImageReader, EndianImageReader
    {
        protected EndianByteImageReader(ByteMemoryArea img, Address addr) : base(img, addr) { }
        protected EndianByteImageReader(ByteMemoryArea img, Address addr, long cUnits) : base(img, addr, cUnits) { }
        protected EndianByteImageReader(ByteMemoryArea img, long offsetBegin, long offsetEnd) : base(img, offsetBegin, offsetEnd) { }
        protected EndianByteImageReader(ByteMemoryArea img, Address addrBegin, Address addrEnd) : base(img, addrBegin, addrEnd) { }
        protected EndianByteImageReader(ByteMemoryArea img, long off) : base(img, off) { }
        protected EndianByteImageReader(byte[] img, long off, long offEnd) : base(img, off, offEnd) { }
        protected EndianByteImageReader(byte[] img, long off) : base(img, off, img.Length) { }
        protected EndianByteImageReader(byte[] img) : base(img, 0, img.Length) { }

        /// <summary>
        /// Create a new EndianImageReader with the same endianness as this one.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
		public abstract EndianImageReader CreateNew(byte[] bytes, long offset);
        public abstract EndianImageReader CreateNew(MemoryArea image, Address addr);

        public new virtual EndianImageReader Clone()
        {
            EndianImageReader rdr;
            if (mem != null)
            {
                rdr = CreateNew(mem, addrStart!);
                rdr.Offset = off;
            }
            else
            {
                rdr = CreateNew(bytes, off);
            }
            return rdr;
        }


        public T ReadAt<T>(long offset, Func<EndianImageReader, T> action)
        {
            return base.ReadAt(offset, rdr => action.Invoke((EndianImageReader) rdr));
        }

        /// <summary>
        /// </summary>
        /// <param name="charType"></param>
        /// <returns></returns>
        public bool ReadNullCharTerminator(DataType charType)
        {
            return charType.BitSize switch
            {
                8 => (char) ReadByte() == 0,
                16 => (char) ReadUInt16() == 0,
                _ => throw new NotSupportedException(string.Format("Character bit size {0} not supported.", charType.BitSize)),
            };
        }

        public string ReadNulTerminatedString(DataType charType, Encoding encoding)
        {
            int iStart = (int) Offset;
            while (IsValid && !ReadNullCharTerminator(charType))
                ;
            return encoding.GetString(
                    bytes,
                    iStart,
                    (int) Offset - iStart - 1);
        }

    /// <summary>
    /// Reads a NUL-terminated string starting at the current position
    /// and returns it as a <see cref="StringConstant"/>.
    /// </summary>
    /// <param name="charType"></param>
    /// <returns>A <see cref="StringConstant"/>.</returns>
    public StringConstant ReadCString(DataType charType, Encoding encoding)
		{
            var str = ReadNulTerminatedString(charType, encoding);
            return new StringConstant(
                StringType.NullTerminated(charType),
                str);
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
            if (!TryRead(lengthType, out var cLength))
                throw new InvalidOperationException("Unable to read string length prefix.");
			int length = cLength.ToInt32();
			int iStart = (int)Offset;
            var cStr = new StringConstant(
				StringType.LengthPrefixedStringType(charType, lengthType),
				encoding.GetString(
					bytes,
					iStart,
					length * charType.Size));
            Offset += length;
            return cStr;
		}

		public abstract short ReadInt16();
		public abstract bool TryReadInt16(out short i16);
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

		public abstract short ReadInt16(int offset);
		public abstract int ReadInt32(int offset);
		public abstract long ReadInt64(int offset);

		public abstract ushort ReadUInt16(int offset);
		public abstract uint ReadUInt32(int offset);
		public abstract ulong ReadUInt64(int offset);

		public abstract bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant c);

		public abstract bool TryPeekUInt32(int offset, out uint value);
		public abstract bool TryPeekUInt64(int offset, out ulong value);
	}

	/// <summary>
	/// Use this reader when the processor is in Little-Endian mode to read multi-
	/// byte quantities from memory.
	/// </summary>
	public class LeImageReader : EndianByteImageReader
	{
		public LeImageReader(ByteMemoryArea image, long offset) : base(image, offset) { }
		public LeImageReader(ByteMemoryArea image, Address addr) : base(image, addr) { }
		public LeImageReader(ByteMemoryArea image, Address addr, long cUnits) : base(image, addr, cUnits) { }
		public LeImageReader(ByteMemoryArea image, long offsetBegin, long offsetEnd) : base(image, offsetBegin, offsetEnd) { }
		public LeImageReader(ByteMemoryArea image, Address addrBegin, Address addrEnd) : base(image, addrBegin, addrEnd) { }
        public LeImageReader(byte[] bytes, long offsetBegin, long offsetEnd) : base(bytes, offsetBegin, offsetEnd) { }
        public LeImageReader(byte[] bytes, long offset) : base(bytes, offset, bytes.Length) { }
        public LeImageReader(byte[] bytes) : base(bytes, 0, bytes.Length) { }

        public override EndianImageReader CreateNew(byte[] bytes, long offset)
		{
			return new LeImageReader(bytes, offset);
		}

		public override EndianImageReader CreateNew(MemoryArea image, Address addr)
		{
			return new LeImageReader((ByteMemoryArea) image, (uint)(addr - image.BaseAddress));
		}

        public T ReadAt<T>(long offset, Func<LeImageReader, T> action)
        {
            return base.ReadAt(offset, rdr => action.Invoke((LeImageReader)rdr));
        }

        public override short ReadInt16() { return ReadLeInt16(); }
		public override int ReadInt32() { return ReadLeInt32(); }
		public override long ReadInt64() { return ReadLeInt64(); }
		public override ushort ReadUInt16() { return ReadLeUInt16(); }
		public override uint ReadUInt32() { return ReadLeUInt32(); }
		public override ulong ReadUInt64() { return ReadLeUInt64(); }
		public override bool TryPeekUInt32(int offset, out uint value) { return TryPeekLeUInt32(offset, out value); }
		public override bool TryPeekUInt64(int offset, out ulong value) { return TryPeekLeUInt64(offset, out value); }

		public override bool TryReadInt16(out short i16) { return TryReadLeInt16(out i16); }
		public override bool TryReadInt32(out int i32) { return TryReadLeInt32(out i32); }
		public override bool TryReadInt64(out long value) { return TryReadLeInt64(out value); }
		public override bool TryReadUInt16(out ushort value) { return TryReadLeUInt16(out value); }
		public override bool TryReadUInt32(out uint ui32) { return TryReadLeUInt32(out ui32); }
		public override bool TryReadUInt64(out ulong ui64) { return TryReadLeUInt64(out ui64); }

		public override short ReadInt16(int offset) { return PeekLeInt16(offset); }
		public override int ReadInt32(int offset) { return PeekLeInt32(offset); }
		public override long ReadInt64(int offset) { return PeekLeInt64(offset); }
		public override ushort ReadUInt16(int offset) { return PeekLeUInt16(offset); }
		public override uint ReadUInt32(int offset) { return PeekLeUInt32(offset); }
		public override ulong ReadUInt64(int offset) { return PeekLeUInt64(offset); }

		public override bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant c) => TryReadLe(dataType, out c);
	}

	/// <summary>
	/// Use this reader when the processor is in Big-Endian mode to read multi-
	/// byte quantities from memory.
	/// </summary>
	public class BeImageReader : EndianByteImageReader
	{
		public BeImageReader(ByteMemoryArea image, long offset) : base(image, offset) { }
		public BeImageReader(ByteMemoryArea image, Address addr) : base(image, addr) { }
		public BeImageReader(ByteMemoryArea image, Address addr, long cUnits) : base(image, addr, cUnits) { }
        public BeImageReader(ByteMemoryArea image, long offsetBegin, long offsetEnd) : base(image, offsetBegin, offsetEnd) { }
		public BeImageReader(ByteMemoryArea image, Address addrBegin, Address addrEnd) : base(image, addrBegin, addrEnd) { }
        public BeImageReader(byte[] bytes, long offset, long offsetEnd) : base(bytes, offset, offsetEnd) { }
        public BeImageReader(byte[] bytes, long offset) : base(bytes, offset, bytes.Length) { }
        public BeImageReader(byte[] bytes) : base(bytes, 0, bytes.Length) { }

        public override EndianImageReader CreateNew(byte[] bytes, long offset)
		{
			return new BeImageReader(bytes, offset, this.offEnd);
		}

		public override EndianImageReader CreateNew(MemoryArea image, Address addr)
		{
			return new BeImageReader((ByteMemoryArea)image, (uint)(addr - image.BaseAddress));
		}

        public T ReadAt<T>(long offset, Func<BeImageReader, T> action)
        {
            return base.ReadAt<T>(offset, rdr => action((BeImageReader)rdr));
        }

        public override short ReadInt16() { return ReadBeInt16(); }
		public override int ReadInt32() { return ReadBeInt32(); }
		public override long ReadInt64() { return ReadBeInt64(); }
		public override ushort ReadUInt16() { return ReadBeUInt16(); }
		public override uint ReadUInt32() { return ReadBeUInt32(); }
		public override ulong ReadUInt64() { return ReadBeUInt64(); }
		public override bool TryPeekUInt32(int offset, out uint value) { return TryPeekBeUInt32(offset, out value); }
		public override bool TryPeekUInt64(int offset, out ulong value) { return TryPeekBeUInt64(offset, out value); }
		public override bool TryReadInt16(out short i16) { return TryReadBeInt16(out i16); }
		public override bool TryReadInt32(out int i32) { return TryReadBeInt32(out i32); }
		public override bool TryReadInt64(out long value) { return TryReadBeInt64(out value); }
		public override bool TryReadUInt16(out ushort ui16) { return TryReadBeUInt16(out ui16); }
		public override bool TryReadUInt32(out uint ui32) { return TryReadBeUInt32(out ui32); }
		public override bool TryReadUInt64(out ulong ui64) { return TryReadBeUInt64(out ui64); }

		public override short ReadInt16(int offset) { return PeekBeInt16(offset); }
		public override int ReadInt32(int offset) { return PeekBeInt32(offset); }
		public override long ReadInt64(int offset) { return PeekBeInt64(offset); }
		public override ushort ReadUInt16(int offset) { return PeekBeUInt16(offset); }
		public override uint ReadUInt32(int offset) { return PeekBeUInt32(offset); }
		public override ulong ReadUInt64(int offset) { return PeekBeUInt64(offset); }

		public override bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant c) => TryReadBe(dataType, out c);
    }
}
