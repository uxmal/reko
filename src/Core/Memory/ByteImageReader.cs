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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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
        /// <summary>
        /// Optional <see cref="ByteMemoryArea"/> from which <see cref="bytes"/> are read.
        /// </summary>
        protected ByteMemoryArea? mem;

        /// <summary>
        /// The byte array from which data is read.
        /// </summary>
        protected byte[] bytes;

        /// <summary>
        /// Offset of the first readable byte the image in the byte array.
        /// </summary>
        protected long offStart;

        /// <summary>
        /// Offset of the last readable byte in the byte array.
        /// </summary>
        protected long offEnd;

        /// <summary>
        /// Current position in the byte array.
        /// </summary>
        private long offset;

        /// <summary>
        /// Starting address of the image in the byte array.
        /// </summary>
        protected Address? addrStart;

        /// <summary>
        /// Initializes a new <see cref="ByteImageReader"/> instance.
        /// </summary>
        /// <param name="mem">Memory area to read from.</param>
        /// <param name="addr">Address to start reading from.</param>
        protected ByteImageReader(ByteMemoryArea mem, Address addr)
        {
            this.mem = mem ?? throw new ArgumentNullException(nameof(mem));
            this.addrStart = addr;
            long o = addr - mem.BaseAddress;
            if (o >= mem.Length)
                throw new ArgumentOutOfRangeException(nameof(addr), $"Address {addr} is outside of image.");
            this.offStart = o;
            this.offEnd = mem.Length;
            this.Offset = offStart;
            this.bytes = mem.Bytes;
        }

        /// <summary>
        /// Initializes a new <see cref="ByteImageReader"/> instance.
        /// </summary>
        /// <param name="mem">Memory area to read from.</param>
        /// <param name="addr">Address to start reading from.</param>
        /// <param name="cUnits">Maximum number of bytes to read.</param>
        protected ByteImageReader(ByteMemoryArea mem, Address addr, long cUnits)
        {
            this.mem = mem ?? throw new ArgumentNullException(nameof(mem));
            this.addrStart = addr;
            long o = addr - mem.BaseAddress;
            if (o >= mem.Length)
                throw new ArgumentOutOfRangeException(nameof(addr), $"Address {addr} is outside of image.");
            this.offStart = o;
            this.offEnd = Math.Min(o + cUnits, mem.Bytes.Length);
            this.Offset = offStart;
            this.bytes = mem.Bytes;
        }

        /// <summary>
        /// Initializes a new <see cref="ByteImageReader"/> instance.
        /// </summary>
        /// <param name="mem">Memory area to read from.</param>
        /// <param name="addrBegin">Address to start reading from.</param>
        /// <param name="addrEnd">Address at which to stop reading.</param>
        protected ByteImageReader(ByteMemoryArea mem, Address addrBegin, Address addrEnd)
        {
            this.mem = mem ?? throw new ArgumentNullException(nameof(mem));
            this.addrStart = addrBegin;
            this.offStart = addrBegin - mem.BaseAddress;
            // Prevent walking off the end of the bytes.
            this.offEnd = Math.Min(addrEnd - mem.BaseAddress, mem.Bytes.Length);
            this.Offset = this.offStart;
            this.bytes = mem.Bytes;
        }

        /// <summary>
        /// Initializes a new <see cref="ByteImageReader"/> instance.
        /// </summary>
        /// <param name="mem">Memory area to read from.</param>
        /// <param name="offset">Offset at which to start reading from.</param>
        protected ByteImageReader(ByteMemoryArea mem, long offset)
        {
            this.mem = mem ?? throw new ArgumentNullException(nameof(mem));
            this.bytes = mem.Bytes;
            this.addrStart = mem.BaseAddress + offset;
            this.offStart = offset;
            this.offEnd = mem.Bytes.Length;
            this.Offset = offStart;
        }

        /// <summary>
        /// Initializes a new <see cref="ByteImageReader"/> instance.
        /// </summary>
        /// <param name="mem">Memory area to read from.</param>
        /// <param name="offsetBegin">Offset at which to start reading from.</param>
        /// <param name="offsetEnd">Offset at which to stop reading from.</param>
        protected ByteImageReader(ByteMemoryArea mem, long offsetBegin, long offsetEnd)
        {
            this.mem = mem ?? throw new ArgumentNullException(nameof(mem));
            this.bytes = mem.Bytes;
            this.addrStart = mem.BaseAddress + offsetBegin;
            if (offsetBegin < 0 || offsetBegin >= mem.Length)
                throw new ArgumentOutOfRangeException(nameof(offsetBegin), $"Starting Offset {offsetBegin} is outside of memory area.");
            this.offStart = offsetBegin;
            this.offEnd = Math.Min(Math.Max(offsetEnd, offsetBegin), mem.Length);
            this.Offset = offsetBegin;
        }

        /// <summary>
        /// Initializes a new <see cref="ByteImageReader"/> instance.
        /// </summary>
        /// <param name="bytes">Byte array to read from.</param>
        /// <param name="offsetBegin">Offset at which to start reading from.</param>
        /// <param name="offsetEnd">Offset at which to stop reading from.</param>
        public ByteImageReader(byte[] bytes, long offsetBegin, long offsetEnd)
        {
            this.bytes = bytes;
            this.offStart = offsetBegin;
            this.offEnd = offsetEnd;
            this.Offset = offStart;
        }

        /// <summary>
        /// Constructs a <see cref="ByteImageReader"/> in the given byte array.
        /// </summary>
        /// <param name="bytes">Bytes to read.</param>
        public ByteImageReader(byte[] bytes) : this(bytes, 0, bytes.Length) { }


        /// <summary>
        /// Constructs a <see cref="ByteImageReader"/> in the given byte array
        /// starting at a given offset.
        /// </summary>
        /// <param name="img">Bytes to read.</param>
        /// <param name="off">Offset into the byte array from which to start reading.
        /// </param>
        public ByteImageReader(byte[] img, long off) : this(img, off, img.Length) { }

        /// <inheritdoc/>
        public Address Address { get { return addrStart!.Value + (Offset - offStart); } }

        /// <summary>
        /// The byte array from which data is read.
        /// </summary>
        public byte[] Bytes => bytes;

        /// <inheritdoc/>
        public int CellBitSize => 8;

        /// <inheritdoc/>
        public long Offset { get { return this.offset; } set { this.offset = value; } }

        /// <inheritdoc/>
        public bool IsValid { get { return IsValidOffset(Offset); } }

        /// <inheritdoc/>
        public bool IsValidOffset(long offset) { return 0 <= offset && offset < offEnd; }

        /// <inheritdoc/>
        public ByteImageReader Clone()
        {
            var that = new ByteImageReader(this.bytes, this.Offset, this.offEnd);
            that.mem = this.mem;
            that.offStart = this.offStart;
            that.offEnd = this.offEnd;
            that.addrStart = this.addrStart;
            return that;
        }

        /// <inheritdoc/>
        public BinaryReader CreateBinaryReader()
        {
            return new BinaryReader(new MemoryStream(Bytes));
        }

        /// <summary>
        /// Reads a structure from the image reader.
        /// </summary>
        /// <typeparam name="T">Data type of the structure to be read.</typeparam>
        /// <returns>The read value.</returns>
        public virtual T ReadStruct<T>() where T : struct
        {
            return new StructureReader<T>(this).Read();
        }

        /// <summary>
        /// Reads a structure from the image reader.
        /// </summary>
        /// <typeparam name="T">Data type of the structure to be read.</typeparam>
        /// <param name="offset">Offset at which to start reading.</param>
        /// <param name="action">Delegate responsible for extracting the structure.
        /// </param>
        /// <returns>The read value.</returns>
        public virtual T ReadAt<T>(long offset, Func<ImageReader, T> action)
        {
            long prevOffset = Offset;
            Offset = offset;
            T result = action.Invoke(this);
            Offset = prevOffset;
            return result;
        }

        /// <inheritdoc/>
        public byte ReadByte()
        {
            byte b = bytes[Offset];
            ++Offset;
            return b;
        }

        /// <inheritdoc/>
        public bool TryReadByte(out byte b)
        {
            if (!IsValidOffset(Offset))
            {
                b = 0;
                return false;
            }
            b = bytes[Offset];
            ++Offset;
            return true;
        }

        /// <inheritdoc/>
        public sbyte ReadSByte()
        {
            return (sbyte) ReadByte();
        }

        /// <inheritdoc/>
        public byte PeekByte(int offset)
        {
            return bytes[(long) Offset + offset];
        }

        /// <inheritdoc/>
        public sbyte PeekSByte(int offset) { return (sbyte)bytes[(long)Offset + offset]; }

        /// <inheritdoc/>
        public byte[] ReadBytes(int length) { return ReadBytes((uint) length); }

        /// <inheritdoc/>
        public byte[] ReadBytes(uint length)
        {
            int avail = Math.Min((int) length, bytes.Length - (int) Offset);
            if (avail <= 0)
                return Array.Empty<byte>();
            byte[] dst = new byte[avail];
            Array.Copy(bytes, (int) Offset, dst, 0, avail);
            Offset += length;
            return dst;
        }

        /// <inheritdoc/>
        public int ReadBytes(byte[] dst, int offset, uint length)
        {
            Array.Copy(bytes, (int) Offset, dst, offset, length);
            Offset += length;
            return (int) length;
        }

        /// <summary>
        /// Reads a chunk of bytes and interprets it in Little-Endian mode.
        /// </summary>
        /// <param name="dataType">The size of the bytes to read.</param>
        /// <param name="c">If successful, the read value as a <see cref="Constant"/>.</param>
        /// <returns>True of it was possible to read a constant of the requested size;
        /// otherwise false.
        /// </returns>
        public bool TryReadLe(DataType dataType, [MaybeNullWhen(false)] out Constant c)
        {
            var size = dataType.Size;
            if (size + Offset > offEnd)
            {
                c = default!;
                return false;
            }
            bool ret;
            if (mem is null)
                ret = ByteMemoryArea.TryReadLe(bytes, Offset, dataType, out c);
            else
                ret = mem.TryReadLe(Offset, dataType, out c);
            if (ret)
                Offset += size;
            return ret;
        }

        /// <summary>
        /// Reads a chunk of bytes and interprets it in Big-Endian mode.
        /// </summary>
        /// <param name="dataType">The size of the bytes to read.</param>
        /// <param name="c">If successful, the read value as a <see cref="Constant"/>.</param>
        /// <returns>True of it was possible to read a constant of the requested size;
        /// otherwise false.
        /// </returns>
        public bool TryReadBe(DataType dataType, [MaybeNullWhen(false)] out Constant c)
        {
            var size = dataType.Size;
            if (size + Offset > offEnd)
            {
                c = default!;
                return false;
            }
            bool ret = mem!.TryReadBe(Offset, dataType, out c);
            if (ret)
                Offset += size;
            return ret;
        }

        /// <inheritdoc/>
        public bool TryReadLeSigned(DataType w, out long value)
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

        /// <inheritdoc/>
        public ushort ReadLeUInt16()
        {
            ushort u = ByteMemoryArea.ReadLeUInt16(bytes, Offset);
            Offset += 2;
            return u;
        }

        /// <inheritdoc/>
        public bool TryReadLeUInt16(out ushort us)
        {
            if (!ByteMemoryArea.TryReadLeUInt16(bytes, (uint) Offset, offEnd, out us))
                return false;
            Offset += 2;
            return true;
        }

        /// <inheritdoc/>
        public bool TryReadBeUInt16(out ushort us)
        {
            if (!ByteMemoryArea.TryReadBeUInt16(bytes, (uint) Offset, offEnd, out us))
                return false;
            Offset += 2;
            return true;
        }

        /// <inheritdoc/>
        public bool TryReadBeInt16(out short s)
        {
            if (!ByteMemoryArea.TryReadBeInt16(bytes, (uint) Offset, offEnd, out s))
                return false;
            Offset += 2;
            return true;
        }

        /// <inheritdoc/>
        public ushort ReadBeUInt16()
        {
            ushort u = ByteMemoryArea.ReadBeUInt16(bytes, (uint) Offset);
            Offset += 2;
            return u;
        }

        /// <inheritdoc/>
        public short ReadBeInt16() { return (short) ReadBeUInt16(); }

        /// <inheritdoc/>
        public short ReadLeInt16() { return (short) ReadLeUInt16(); }

        /// <inheritdoc/>
        public ushort PeekLeUInt16(int offset) { return ByteMemoryArea.ReadLeUInt16(bytes, offset + (uint) Offset); }

        /// <inheritdoc/>
        public ushort PeekBeUInt16(int offset) { return ByteMemoryArea.ReadBeUInt16(bytes, offset + (uint) Offset); }

        /// <inheritdoc/>
        public short PeekLeInt16(int offset) { return (short) ByteMemoryArea.ReadLeUInt16(bytes, offset + (uint) Offset); }

        /// <inheritdoc/>
        public short PeekBeInt16(int offset) { return (short) ByteMemoryArea.ReadBeUInt16(bytes, offset + (uint) Offset); }


        /// <inheritdoc/>
        public bool TryPeekByte(int offset, out byte value) { return ByteMemoryArea.TryReadByte(bytes, offset + Offset, out value); }

        /// <inheritdoc/>
        public bool TryPeekBeUInt16(int offset, out ushort value) { return ByteMemoryArea.TryReadBeUInt16(bytes, offset + Offset, out value); }

        /// <inheritdoc/>
        public bool TryPeekBeUInt32(int offset, out uint value) { return ByteMemoryArea.TryReadBeUInt32(bytes, offset + Offset, out value); }

        /// <inheritdoc/>
        public bool TryPeekBeUInt64(int offset, out ulong value) { return ByteMemoryArea.TryReadBeUInt64(bytes, offset + Offset, out value); }


        /// <inheritdoc/>
        public bool TryPeekLeUInt16(int offset, out ushort value) { return ByteMemoryArea.TryReadLeUInt16(bytes, offset + Offset, out value); }

        /// <inheritdoc/>
        public bool TryPeekLeUInt32(int offset, out uint value) { return ByteMemoryArea.TryReadLeUInt32(bytes, offset + Offset, out value); }

        /// <inheritdoc/>
        public bool TryPeekLeUInt64(int offset, out ulong value) { return ByteMemoryArea.TryReadLeUInt64(bytes, offset + Offset, out value); }

        /// <summary>
        /// Reads a 32-bit unsigned little-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <returns>The value read.</returns>
        public uint ReadLeUInt32()
        {
            uint u = ByteMemoryArea.ReadLeUInt32(bytes, (uint) Offset);
            Offset += 4;
            return u;
        }

        /// <summary>
        /// Reads a 32-bit unsigned big-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <returns>The value read.</returns>
        public uint ReadBeUInt32()
        {
            uint u = ByteMemoryArea.ReadBeUInt32(bytes, Offset);
            Offset += 4;
            return u;
        }

        /// <summary>
        /// Reads a 16-bit signed little-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <param name="i16">The value read.</param>
        /// <returns>True if the value was successfully read; otherwise false.</returns>
        public bool TryReadLeInt16(out short i16)
        {
            if (!ByteMemoryArea.TryReadLeInt16(this.bytes, Offset, offEnd, out i16))
                return false;
            Offset += 2;
            return true;
        }

        /// <summary>
        /// Reads a 32-bit signed little-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <param name="i32">The value read.</param>
        /// <returns>True if the value was successfully read; otherwise false.</returns>
        public bool TryReadLeInt32(out int i32)
        {
            if (!ByteMemoryArea.TryReadLeInt32(this.bytes, Offset, offEnd, out i32))
                return false;
            Offset += 4;
            return true;
        }

        /// <summary>
        /// Reads a 32-bit signed big-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <param name="i32">The value read.</param>
        /// <returns>True if the value was successfully read; otherwise false.</returns>
        public bool TryReadBeInt32(out int i32)
        {
            if (!ByteMemoryArea.TryReadBeInt32(this.bytes, Offset, offEnd, out i32))
                return false;
            Offset += 4;
            return true;
        }

        /// <summary>
        /// Reads a 32-bit unsigned little-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <param name="ui32">The value read.</param>
        /// <returns>True if the value was successfully read; otherwise false.</returns>
        public bool TryReadLeUInt32(out uint ui32)
        {
            if (!ByteMemoryArea.TryReadLeUInt32(this.bytes, Offset, offEnd, out ui32))
                return false;
            Offset += 4;
            return true;
        }

        /// <summary>
        /// Reads a 32-bit unsigned big-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <param name="ui32">The value read.</param>
        /// <returns>True if the value was successfully read; otherwise false.</returns>
        public bool TryReadBeUInt32(out uint ui32)
        {
            if (!ByteMemoryArea.TryReadBeUInt32(this.bytes, Offset, offEnd, out ui32))
                return false;
            Offset += 4;
            return true;
        }

        /// <summary>
        /// Reads a 64-bit signed little-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <param name="value">The value read.</param>
        /// <returns>True if the value was successfully read; otherwise false.</returns>
        public bool TryReadLeInt64(out long value)
        {
            if (!ByteMemoryArea.TryReadLeInt64(this.bytes, Offset, offEnd, out value))
                return false;
            Offset += 8;
            return true;
        }

        /// <summary>
        /// Reads a 64-bit unsigned little-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <param name="value">The value read.</param>
        /// <returns>True if the value was successfully read; otherwise false.</returns>
        public bool TryReadLeUInt64(out ulong value)
        {
            if (!ByteMemoryArea.TryReadLeUInt64(this.bytes, Offset, offEnd, out value))
                return false;
            Offset += 8;
            return true;
        }

        /// <summary>
        /// Reads a 64-bit signed big-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <param name="value">The value read.</param>
        /// <returns>True if the value was successfully read; otherwise false.</returns>
        public bool TryReadBeInt64(out long value)
        {
            if (!ByteMemoryArea.TryReadBeInt64(this.bytes, Offset, offEnd, out value))
                return false;
            Offset += 8;
            return true;
        }

        /// <summary>
        /// Reads a 64-bit unsigned big-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <param name="value">The value read.</param>
        /// <returns>True if the value was successfully read; otherwise false.</returns>
        public bool TryReadBeUInt64(out ulong value)
        {
            if (!ByteMemoryArea.TryReadBeUInt64(this.bytes, Offset, out value))
                return false;
            Offset += 8;
            return true;
        }

        /// <summary>
        /// Reads a 32-bit signed big-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <returns>The value read.</returns>
        public int ReadBeInt32() { return (int) ReadBeUInt32(); }

        /// <summary>
        /// Reads a 32-bit signed little-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <returns>The value read.</returns>
        public int ReadLeInt32() { return (int) ReadLeUInt32(); }

        /// <summary>
        /// Reads a 32-bit unsigned little-endian integer from the 
        /// given offset in the image, but doesn't advance the offset.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public uint PeekLeUInt32(int offset) { return ByteMemoryArea.ReadLeUInt32(bytes, offset + Offset); }

        /// <summary>
        /// Reads a 32-bit unsigned big-endian integer from the 
        /// given offset in the image, but doesn't advance the offset.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public uint PeekBeUInt32(int offset) { return ByteMemoryArea.ReadBeUInt32(bytes, offset + Offset); }

        /// <summary>
        /// Reads a 32-bit signed little-endian integer from the 
        /// given offset in the image, but doesn't advance the offset.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public int PeekLeInt32(int offset) { return (int) ByteMemoryArea.ReadLeUInt32(bytes, offset + Offset); }

        /// <summary>
        /// Reads a 32-bit signed big-endian integer from the 
        /// given offset in the image, but doesn't advance the offset.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public int PeekBeInt32(int offset) { return (int) ByteMemoryArea.ReadBeUInt32(bytes, offset + Offset); }

        /// <summary>
        /// Reads a 64-bit signed little-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <returns>The value read.</returns>
        public ulong ReadLeUInt64()
        {
            ulong u = ByteMemoryArea.ReadLeUInt64(bytes, Offset);
            Offset += 8;
            return u;
        }

        /// <summary>
        /// Reads a 64-bit unsigned little-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <returns>The value read.</returns>
        public ulong ReadBeUInt64()
        {
            ulong u = ByteMemoryArea.ReadBeUInt64(bytes, Offset);
            Offset += 8;
            return u;
        }

        /// <summary>
        /// Reads a 64-bit signed big-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <returns>The value read.</returns>
        public long ReadBeInt64() { return (long) ReadBeUInt64(); }

        /// <summary>
        /// Reads a 64-bit signed little-endian integer from the image,
        /// and advances the offset.
        /// </summary>
        /// <returns>The value read.</returns>
        public long ReadLeInt64() { return (long) ReadLeUInt64(); }


        /// <summary>
        /// Reads a 64-bit unsigned little-endian integer from the 
        /// given offset in the image, but doesn't advance the offset.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public ulong PeekLeUInt64(int offset) { return ByteMemoryArea.ReadLeUInt64(bytes, Offset + offset); }

        /// <summary>
        /// Reads a 64-bit unsigned big-endian integer from the 
        /// given offset in the image, but doesn't advance the offset.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public ulong PeekBeUInt64(int offset) { return ByteMemoryArea.ReadBeUInt64(bytes, Offset + offset); }

        /// <summary>
        /// Reads a 64-bit signed little-endian integer from the 
        /// given offset in the image, but doesn't advance the offset.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public long PeekLeInt64(int offset) { return (long) ByteMemoryArea.ReadLeUInt64(bytes, Offset + offset); }

        /// <summary>
        /// Reads a 64-bit signed big-endian integer from the 
        /// given offset in the image, but doesn't advance the offset.
        /// </summary>
        /// <returns>The value peeked.</returns>
        public long PeekBeInt64(int offset) { return (long) ByteMemoryArea.ReadBeUInt64(bytes, Offset + offset); }


        /// <inheritdoc/>
        public long Seek(long offset, SeekOrigin origin = SeekOrigin.Current)
        {
            switch (origin)
            {
            case SeekOrigin.Begin:
                Offset = offStart + offset;
                break;
            case SeekOrigin.Current:
                Offset += offset;
                break;
            case SeekOrigin.End:
                Offset = offEnd + offset;
                break;
            }
            return Offset;
        }

        /// <inheritdoc/>
        public byte[] ReadToEnd()
        {
            long avail = this.offEnd - Offset;
            if (avail <= 0)
                return Array.Empty<byte>();
            byte[] dst = new byte[avail];
            Array.Copy(bytes, (int) Offset, dst, 0, avail);
            Offset = offEnd;
            return dst;
        }

        /// <inheritdoc/>
        public byte[] Read(int count)
        {
            long bytesRead = (int) Math.Min(count, offEnd - this.Offset);
            if (bytesRead <= 0)
                return Array.Empty<byte>();
            byte[] dst = new byte[bytesRead];
            Array.Copy(this.bytes, (int) Offset, dst, 0, bytesRead);
            this.Offset += bytesRead;
            return dst;
        }

        /// <inheritdoc/>
        public int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = (int) Math.Min(buffer.Length - offset, count);
            bytesRead = (int) Math.Min(bytesRead, offEnd - offset);
            Array.Copy(bytes, this.Offset, buffer, offset, bytesRead);
            Offset += bytesRead;
            return bytesRead;
        }
    }
}
