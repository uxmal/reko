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
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Core.Memory
{
    /// <summary>
    /// This interface is used to read multi-byte quantities from a byte array or memory area
    /// using a default endianness.
    /// </summary>
    public interface EndianImageReader : ImageReader
    {
        /// <summary>
        /// Create a new <see cref="EndianImageReader" /> with the same endianness as this one.
        /// </summary>
        /// <returns>A clone of this reader.</returns>
        EndianImageReader Clone();

        /// <summary>
        /// Creates a new <see cref="EndianImageReader" /> with the same endianness as this one,
        /// but with a different backing memory area.
        /// </summary>
        /// <param name="image">Memory area to use for the new image reader.</param>
        /// <param name="addr">Address at which to position the new image reader.</param>
        /// <returns>A new <see cref="EndianImageReader"/> with the same endianness
        /// as this one.</returns>
        EndianImageReader CreateNew(MemoryArea image, Address addr);

        /// <summary>
        /// Reads a character of size <paramref name="dtChar"/> and determines if it is 
        /// the NUL character. 
        /// </summary>
        /// <param name="dtChar">Size of the character to read.</param>
        /// <returns>True if a NUL character was encountered; false otherwise.
        /// </returns>
        bool ReadNullCharTerminator(DataType dtChar);

        /// <summary>
        /// Reads a null terminated string starting at the current position of the reader.
        /// </summary>
        /// <param name="charType">Data type of the individual characters.</param>
        /// <param name="encoding">Text encoding to use to decode the string.</param>
        string ReadNulTerminatedString(DataType charType, Encoding encoding);

        /// <summary>
        /// Reads a null terminated string starting at the current position of the reader
        /// and wrtaps in a <see cref="StringConstant"/>.
        /// </summary>
        /// <param name="charType">Data type of the individual characters.</param>
        /// <param name="encoding">Text encoding to use to decode the string.</param>
        StringConstant ReadCString(DataType charType, Encoding encoding);

        /// <summary>
        /// Reads a 16-bit signed integer from the current position of the underlying array,
        /// then advances the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
        short ReadInt16();

        /// <summary>
        /// Reads a 32-bit signed integer from the current position of the underlying array,
        /// then advances the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
        int ReadInt32();

        /// <summary>
        /// Reads a 64-bit signed integer from the current position of the underlying array,
        /// then advances the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
        long ReadInt64();

        /// <summary>
        /// Reads a 16-bit unsigned integer from the current position of the underlying array,
        /// then advances the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
        ushort ReadUInt16();

        /// <summary>
        /// Reads a 16-bit unsigned integer from the current position of the underlying array.
        /// then advances the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
        uint ReadUInt32();

        /// <summary>
        /// Reads a 16-bit unsigned integer from the current position of the underlying array,
        /// then advances the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
        ulong ReadUInt64();

        /// <summary>
        /// Reads a 32-bit unsigned integer from the current position of the underlying array, offset
        /// by <paramref name="offset"/>, but doesn't advance the reader's current position.
        /// </summary>
        /// <param name="offset">Offset from the current position from which to read.</param>
        /// <param name="value">The read value.</param>
        /// <returns>True if the offset position was within bounds; otherwise false.</returns>
        bool TryPeekUInt32(int offset, out uint value);

        /// <summary>
        /// Reads a 32-bit unsigned integer from the current position of the underlying array, offset
        /// by <paramref name="offset"/>, but doesn't advance the reader's current position.
        /// </summary>
        /// <param name="offset">Offset from the current position from which to read.</param>
        /// <param name="value">The read value.</param>
        /// <returns>True if the offset position was within bounds; otherwise false.</returns>
        bool TryPeekUInt64(int offset, out ulong value);

        /// <summary>
        /// Reads a <see cref="Constant"/> of type <paramref name="dataType"/>, 
        /// and advances the reader's current position.
        /// </summary>
        /// <param name="dataType">Data type of the value to be read.</param>
        /// <param name="value">The read value.</param>
        /// <returns>True if the offset position was within bounds; otherwise false.</returns>
        bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant value);

        /// <summary>
        /// Reads a 16-bit signed integer from the current position of the underlying array,
        /// then advances the reader's position.
        /// </summary>
        /// <param name="value">The read value.</param>
        /// <returns>True if the offset position was within bounds; otherwise false.</returns>
        bool TryReadInt16(out short value);

        /// <summary>
        /// Reads a 32-bit signed integer from the current position of the underlying array,
        /// then advances the reader's position.
        /// </summary>
        /// <param name="value">The read value.</param>
        /// <returns>True if the offset position was within bounds; otherwise false.</returns>
        bool TryReadInt32(out int value);

        /// <summary>
        /// Reads a 64-bit signed integer from the current position of the underlying array,
        /// then advances the reader's position.
        /// </summary>
        /// <param name="value">The read value.</param>
        /// <returns>True if the offset position was within bounds; otherwise false.</returns>
        bool TryReadInt64(out long value);

        /// <summary>
        /// Reads a 16-bit unsigned integer from the current position of the underlying array,
        /// then advances the reader's position.
        /// </summary>
        /// <param name="value">The read value.</param>
        /// <returns>True if the offset position was within bounds; otherwise false.</returns>
        bool TryReadUInt16(out ushort value);

        /// <summary>
        /// Reads a 32-bit unsigned integer from the current position of the underlying array,
        /// then advances the reader's position.
        /// </summary>
        /// <param name="value">The read value.</param>
        /// <returns>True if the offset position was within bounds; otherwise false.</returns>
        bool TryReadUInt32(out uint value);

        /// <summary>
        /// Reads a 64-bit unsigned integer from the current position of the underlying array,
        /// then advances the reader's position.
        /// </summary>
        /// <param name="value">The read value.</param>
        /// <returns>True if the offset position was within bounds; otherwise false.</returns>
        bool TryReadUInt64(out ulong value);
    }

    /// <summary>
    /// This byte image reader has an associated notion of endianness. Abstract methods are provided for reading
    /// values that change bitpatterns depending on endianness.
    /// </summary>
	public abstract class EndianByteImageReader : ByteImageReader, EndianImageReader
    {
        /// <summary>
        /// Initializes an instance.
        /// </summary>
        /// <param name="img">Byte memory area.</param>
        /// <param name="addr">Address to start reading at.</param>
        protected EndianByteImageReader(ByteMemoryArea img, Address addr) : base(img, addr) { }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        /// <param name="img">Byte memory area.</param>
        /// <param name="addr">Address to start reading at.</param>
        /// <param name="cUnits">Maximum number of bytes to read.</param>
        protected EndianByteImageReader(ByteMemoryArea img, Address addr, long cUnits) : base(img, addr, cUnits) { }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        /// <param name="img">Byte memory area.</param>
        /// <param name="offsetBegin">Offset at which to start reading.</param>
        /// <param name="offsetEnd">Offset at which to stop reading.</param>
        protected EndianByteImageReader(ByteMemoryArea img, long offsetBegin, long offsetEnd) : base(img, offsetBegin, offsetEnd) { }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        /// <param name="img">Byte memory area.</param>
        /// <param name="addrBegin">Address at which to start reading.</param>
        /// <param name="addrEnd">Address at which to stop reading.</param>
        protected EndianByteImageReader(ByteMemoryArea img, Address addrBegin, Address addrEnd) : base(img, addrBegin, addrEnd) { }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        /// <param name="img">Byte memory area.</param>
        /// <param name="offset">Offset at which to start reading.</param>
        protected EndianByteImageReader(ByteMemoryArea img, long offset) : base(img, offset) { }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        /// <param name="img">Byte array to read from.</param>
        /// <param name="offsetBegin">Offset at which to start reading.</param>
        /// <param name="offsetEnd">Offset at which to stop reading.</param>
        protected EndianByteImageReader(byte[] img, long offsetBegin, long offsetEnd) : base(img, offsetBegin, offsetEnd) { }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        /// <param name="img">Byte array to read from.</param>
        /// <param name="offset">Offset at which to start reading.</param>
        protected EndianByteImageReader(byte[] img, long offset) : base(img, offset, img.Length) { }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        /// <param name="img">Byte array to read from.</param>
        protected EndianByteImageReader(byte[] img) : base(img, 0, img.Length) { }

        /// <summary>
        /// Create a new EndianImageReader with the same endianness as this one.
        /// </summary>
        /// <param name="bytes">Byte area to read from.</param>
        /// <param name="offset">Offset from which to read.</param>
        /// <returns>A new <see cref="EndianImageReader"/>.</returns>
		public abstract EndianImageReader CreateNew(byte[] bytes, long offset);

        /// <summary>
        /// Create a new EndianImageReader with the same endianness as this one.
        /// </summary>
        /// <param name="mem">Memory area to read from.</param>
        /// <param name="address">Address from which to read.</param>
        /// <returns>A new <see cref="EndianImageReader"/>.</returns>
        public abstract EndianImageReader CreateNew(MemoryArea mem, Address address);

        /// <summary>
        /// Creates a new <see cref="EndianImageReader"/> with the same endianness as this one.
        /// </summary>
        /// <returns>A copy if this reader.
        /// </returns>
        public new virtual EndianImageReader Clone()
        {
            EndianImageReader rdr;
            if (mem is not null)
            {
                rdr = CreateNew(mem, addrStart!.Value);
                rdr.Offset = off;
            }
            else
            {
                rdr = CreateNew(bytes, off);
            }
            return rdr;
        }

        /// <inheritdoc/>
        public T ReadAt<T>(long offset, Func<EndianImageReader, T> action)
        {
            return base.ReadAt(offset, rdr => action.Invoke((EndianImageReader) rdr));
        }

        /// <inheritdoc/>
        public bool ReadNullCharTerminator(DataType charType)
        {
            return charType.BitSize switch
            {
                8 => (char) ReadByte() == 0,
                16 => (char) ReadUInt16() == 0,
                _ => throw new NotSupportedException(string.Format("Character bit size {0} not supported.", charType.BitSize)),
            };
        }

        /// <inheritdoc/>
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
    /// <param name="charType">Data type of the code units of the string.</param>
    /// <param name="encoding">The <see cref="Encoding"/> to use when decoding the string.
    /// </param>
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

        /// <summary>
        /// Reads a 16-bit signed integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract short ReadInt16();

        /// <summary>
        /// Reads a 16-bit signed integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <param name="i16">The read value.</param>
        /// <returns>True if the reader's position was within bounds; otherwise false.</returns>
		public abstract bool TryReadInt16(out short i16);

        /// <summary>
        /// Reads a 32-bit signed integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract int ReadInt32();

        /// <summary>
        /// Reads a 32-bit signed integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <param name="i32">The read value.</param>
        /// <returns>True if the reader's position was within bounds; otherwise false.</returns>
        public abstract bool TryReadInt32(out int i32);

        /// <summary>
        /// Reads a 64-bit signed integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract long ReadInt64();

        /// <summary>
        /// Reads a 64-bit signed integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <param name="value">The read value.</param>
        /// <returns>True if the reader's position was within bounds; otherwise false.</returns>
		public abstract bool TryReadInt64(out long value);

        /// <summary>
        /// Reads a 16-bit unsigned integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract ushort ReadUInt16();

        /// <summary>
        /// Reads a 16-bit unsigned integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <param name="ui16">The read value.</param>
        /// <returns>True if the reader's position was within bounds; otherwise false.</returns>
		public abstract bool TryReadUInt16(out ushort ui16);

        /// <summary>
        /// Reads a 32-bit unsigned integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract uint ReadUInt32();

        /// <summary>
        /// Reads a 32-bit unsigned integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <param name="ui32">The read value.</param>
        /// <returns>True if the reader's position was within bounds; otherwise false.</returns>
		public abstract bool TryReadUInt32(out uint ui32);

        /// <summary>
        /// Reads a 64-bit unsigned integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract ulong ReadUInt64();

        /// <summary>
        /// Reads a 64-bit unsigned integer from the current position of the underlying array,
        /// advancing the reader's position.
        /// </summary>
        /// <param name="ui64">The read value.</param>
        /// <returns>True if the reader's position was within bounds; otherwise false.</returns>
		public abstract bool TryReadUInt64(out ulong ui64);

        /// <summary>
        /// Reads a 16-bit signed integer from the current position offset by <paramref name="offset"/>
        /// of the underlying array, but doesn't advance the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract short ReadInt16(int offset);

        /// <summary>
        /// Reads a 32-bit signed integer from the current position offset by <paramref name="offset"/>
        /// of the underlying array, but doesn't advance the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract int ReadInt32(int offset);

        /// <summary>
        /// Reads a 64-bit signed integer from the current position offset by <paramref name="offset"/>
        /// of the underlying array, but doesn't advance the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract long ReadInt64(int offset);

        /// <summary>
        /// Reads a 16-bit unsigned integer from the current position offset by <paramref name="offset"/>
        /// of the underlying array, but doesn't advance the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract ushort ReadUInt16(int offset);

        /// <summary>
        /// Reads a 32-bit unsigned integer from the current position offset by <paramref name="offset"/>
        /// of the underlying array, but doesn't advance the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract uint ReadUInt32(int offset);

        /// <summary>
        /// Reads a 64-bit unsigned integer from the current position offset by <paramref name="offset"/>
        /// of the underlying array, but doesn't advance the reader's position.
        /// </summary>
        /// <returns>The read value.</returns>
		public abstract ulong ReadUInt64(int offset);

        /// <summary>
        /// Reads a <see cref="Constant"/> of type <paramref name="dataType"/>, 
        /// and advances the reader's current position.
        /// </summary>
        /// <param name="dataType">Data type of the value to be read.</param>
        /// <param name="c">The read value.</param>
        /// <returns>True if the reader's position was within bounds; otherwise false.</returns>
		public abstract bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant c);

        /// <summary>
        /// Reads a 32-bit unsigned integer from the current position offset by <paramref name="offset"/>
        /// of the underlying array, but doesn't advance the reader's position.
        /// </summary>
        /// <param name="offset">Offset from the current position from which to read.</param>
        /// <param name="value">The read value.</param>
        /// <returns>True if the offset position was within bounds; otherwise false.</returns>
		public abstract bool TryPeekUInt32(int offset, out uint value);

        /// <summary>
        /// Reads a 64-bit unsigned integer from the current position offset by <paramref name="offset"/>
        /// of the underlying array, but doesn't advance the reader's position.
        /// </summary>
        /// <param name="offset">Offset from the current position from which to read.</param>
        /// <param name="value">The read value.</param>
        /// <returns>True if the offset position was within bounds; otherwise false.</returns>
		public abstract bool TryPeekUInt64(int offset, out ulong value);
	}

	/// <summary>
	/// Use this reader when the processor is in Little-Endian mode to read multi-
	/// byte quantities from memory.
	/// </summary>
	public class LeImageReader : EndianByteImageReader
	{
        /// <inheritdoc/>
		public LeImageReader(ByteMemoryArea image, long offset) : base(image, offset) { }
        /// <inheritdoc/>
		public LeImageReader(ByteMemoryArea image, Address addr) : base(image, addr) { }
        /// <inheritdoc/>
		public LeImageReader(ByteMemoryArea image, Address addr, long cUnits) : base(image, addr, cUnits) { }
        /// <inheritdoc/>
		public LeImageReader(ByteMemoryArea image, long offsetBegin, long offsetEnd) : base(image, offsetBegin, offsetEnd) { }
        /// <inheritdoc/>
		public LeImageReader(ByteMemoryArea image, Address addrBegin, Address addrEnd) : base(image, addrBegin, addrEnd) { }
        /// <inheritdoc/>
        public LeImageReader(byte[] bytes, long offsetBegin, long offsetEnd) : base(bytes, offsetBegin, offsetEnd) { }
        /// <inheritdoc/>
        public LeImageReader(byte[] bytes, long offset) : base(bytes, offset, bytes.Length) { }
        /// <inheritdoc/>
        public LeImageReader(byte[] bytes) : base(bytes, 0, bytes.Length) { }

        /// <inheritdoc/>
        public override EndianImageReader CreateNew(byte[] bytes, long offset)
		{
			return new LeImageReader(bytes, offset);
		}

        /// <inheritdoc/>
		public override EndianImageReader CreateNew(MemoryArea image, Address addr)
		{
			return new LeImageReader((ByteMemoryArea) image, (uint)(addr - image.BaseAddress));
		}

        /// <inheritdoc/>
        public T ReadAt<T>(long offset, Func<LeImageReader, T> action)
        {
            return base.ReadAt(offset, rdr => action.Invoke((LeImageReader)rdr));
        }

        /// <inheritdoc/>
        public override short ReadInt16() { return ReadLeInt16(); }

        /// <inheritdoc/>
		public override int ReadInt32() { return ReadLeInt32(); }

        /// <inheritdoc/>
		public override long ReadInt64() { return ReadLeInt64(); }

        /// <inheritdoc/>
		public override ushort ReadUInt16() { return ReadLeUInt16(); }

        /// <inheritdoc/>
		public override uint ReadUInt32() { return ReadLeUInt32(); }

        /// <inheritdoc/>
		public override ulong ReadUInt64() { return ReadLeUInt64(); }

        /// <inheritdoc/>
		public override bool TryPeekUInt32(int offset, out uint value) { return TryPeekLeUInt32(offset, out value); }

        /// <inheritdoc/>
		public override bool TryPeekUInt64(int offset, out ulong value) { return TryPeekLeUInt64(offset, out value); }


        /// <inheritdoc/>
		public override bool TryReadInt16(out short i16) { return TryReadLeInt16(out i16); }

        /// <inheritdoc/>
		public override bool TryReadInt32(out int i32) { return TryReadLeInt32(out i32); }

        /// <inheritdoc/>
		public override bool TryReadInt64(out long value) { return TryReadLeInt64(out value); }

        /// <inheritdoc/>
		public override bool TryReadUInt16(out ushort value) { return TryReadLeUInt16(out value); }

        /// <inheritdoc/>
		public override bool TryReadUInt32(out uint ui32) { return TryReadLeUInt32(out ui32); }

        /// <inheritdoc/>
		public override bool TryReadUInt64(out ulong ui64) { return TryReadLeUInt64(out ui64); }


        /// <inheritdoc/>
		public override short ReadInt16(int offset) { return PeekLeInt16(offset); }

        /// <inheritdoc/>
		public override int ReadInt32(int offset) { return PeekLeInt32(offset); }

        /// <inheritdoc/>
		public override long ReadInt64(int offset) { return PeekLeInt64(offset); }

        /// <inheritdoc/>
		public override ushort ReadUInt16(int offset) { return PeekLeUInt16(offset); }

        /// <inheritdoc/>
		public override uint ReadUInt32(int offset) { return PeekLeUInt32(offset); }

        /// <inheritdoc/>
		public override ulong ReadUInt64(int offset) { return PeekLeUInt64(offset); }

        /// <inheritdoc/>
		public override bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant c) => TryReadLe(dataType, out c);
	}

	/// <summary>
	/// Use this reader when the processor is in Big-Endian mode to read multi-
	/// byte quantities from memory.
	/// </summary>
	public class BeImageReader : EndianByteImageReader
	{
        /// <inheritdoc/>
		public BeImageReader(ByteMemoryArea image, long offset) : base(image, offset) { }
        /// <inheritdoc/>
		public BeImageReader(ByteMemoryArea image, Address addr) : base(image, addr) { }
        /// <inheritdoc/>
		public BeImageReader(ByteMemoryArea image, Address addr, long cUnits) : base(image, addr, cUnits) { }
        /// <inheritdoc/>
        public BeImageReader(ByteMemoryArea image, long offsetBegin, long offsetEnd) : base(image, offsetBegin, offsetEnd) { }
        /// <inheritdoc/>
		public BeImageReader(ByteMemoryArea image, Address addrBegin, Address addrEnd) : base(image, addrBegin, addrEnd) { }
        /// <inheritdoc/>
        public BeImageReader(byte[] bytes, long offset, long offsetEnd) : base(bytes, offset, offsetEnd) { }
        /// <inheritdoc/>
        public BeImageReader(byte[] bytes, long offset) : base(bytes, offset, bytes.Length) { }
        /// <inheritdoc/>
        public BeImageReader(byte[] bytes) : base(bytes, 0, bytes.Length) { }

        /// <inheritdoc/>
        public override EndianImageReader CreateNew(byte[] bytes, long offset)
		{
			return new BeImageReader(bytes, offset, this.offEnd);
		}

        /// <inheritdoc/>
		public override EndianImageReader CreateNew(MemoryArea image, Address addr)
		{
			return new BeImageReader((ByteMemoryArea)image, (uint)(addr - image.BaseAddress));
		}

        /// <inheritdoc/>
        public T ReadAt<T>(long offset, Func<BeImageReader, T> action)
        {
            return base.ReadAt<T>(offset, rdr => action((BeImageReader)rdr));
        }

        /// <inheritdoc/>
        public override short ReadInt16() { return ReadBeInt16(); }

        /// <inheritdoc/>
		public override int ReadInt32() { return ReadBeInt32(); }

        /// <inheritdoc/>
		public override long ReadInt64() { return ReadBeInt64(); }

        /// <inheritdoc/>
		public override ushort ReadUInt16() { return ReadBeUInt16(); }

        /// <inheritdoc/>
		public override uint ReadUInt32() { return ReadBeUInt32(); }

        /// <inheritdoc/>
		public override ulong ReadUInt64() { return ReadBeUInt64(); }

        /// <inheritdoc/>
		public override bool TryPeekUInt32(int offset, out uint value) { return TryPeekBeUInt32(offset, out value); }

        /// <inheritdoc/>
		public override bool TryPeekUInt64(int offset, out ulong value) { return TryPeekBeUInt64(offset, out value); }

        /// <inheritdoc/>
		public override bool TryReadInt16(out short i16) { return TryReadBeInt16(out i16); }

        /// <inheritdoc/>
		public override bool TryReadInt32(out int i32) { return TryReadBeInt32(out i32); }

        /// <inheritdoc/>
		public override bool TryReadInt64(out long value) { return TryReadBeInt64(out value); }

        /// <inheritdoc/>
		public override bool TryReadUInt16(out ushort ui16) { return TryReadBeUInt16(out ui16); }

        /// <inheritdoc/>
		public override bool TryReadUInt32(out uint ui32) { return TryReadBeUInt32(out ui32); }

        /// <inheritdoc/>
		public override bool TryReadUInt64(out ulong ui64) { return TryReadBeUInt64(out ui64); }

        /// <inheritdoc/>
		public override short ReadInt16(int offset) { return PeekBeInt16(offset); }

        /// <inheritdoc/>
		public override int ReadInt32(int offset) { return PeekBeInt32(offset); }

        /// <inheritdoc/>
		public override long ReadInt64(int offset) { return PeekBeInt64(offset); }

        /// <inheritdoc/>
		public override ushort ReadUInt16(int offset) { return PeekBeUInt16(offset); }

        /// <inheritdoc/>
		public override uint ReadUInt32(int offset) { return PeekBeUInt32(offset); }

        /// <inheritdoc/>
		public override ulong ReadUInt64(int offset) { return PeekBeUInt64(offset); }


        /// <inheritdoc/>
		public override bool TryRead(PrimitiveType dataType, [MaybeNullWhen(false)] out Constant c) => TryReadBe(dataType, out c);
    }
}
