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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Reko.Core.Assemblers
{
	/// <summary>
	/// Emits bytes into a bytestream belonging to a segment/section.
	/// </summary>
    public interface IEmitter
    {
        /// <summary>
        /// The number of bytes accumulated so far.
        /// </summary>
        int Size { get; }
        
        /// <summary>
        /// Position within the byte stream.
        /// </summary>
        int Position { get; set; }

        /// <summary>
        /// Align the byte stream to a given alignment. The current position is
        /// padded by <paramref name="padding"/>, then aligned up to the next
        /// even multiple of <paramref name="align"/>. The padding is filled with zeros.
        /// </summary>
        /// <param name="padding">Number of padding bytes to write first.</param>
        /// <param name="align">Alignment to applay after padding bytes are written.
        /// </param>
        void Align(int padding, int align);

        /// <summary>
        /// Write a big-endian 16-bit unsigned integer to the stream.
        /// </summary>
        /// <param name="us">Value to write.</param>
        void EmitBeUInt16(int us);

        /// <summary>
        /// Write a big-endian 32-bit unsigned integer to the stream.   
        /// </summary>
        /// <param name="ui">Value to write.</param>
        void EmitBeUInt32(int ui);

        /// <summary>
        /// Write a byte to the byte stream.
        /// </summary>
        /// <param name="b">Value to write.</param>
        void EmitByte(int b);

        /// <summary>
        /// Write the byte value <paramref name="b"/> to the stream <paramref name="count"/> times.
        /// </summary>
        /// <param name="b">Byte value to write.</param>
        /// <param name="count">Number of times to write the byte value.</param>
        void EmitBytes(int b, int count);

        /// <summary>
        /// Writes a value to the stream, using the little-endian format.
        /// </summary>
        /// <param name="width">The size of the value to be written.</param>
        /// <param name="value">The value to be written. It may be truncated
        /// if the data type specified by <paramref name="width"/> is narrower.</param>
        void EmitLe(DataType width, int value);

        /// <summary>
        /// Writes a constant value, using the little-endian format.
        /// </summary>
        /// <param name="c">The value to be written.</param>
        /// <param name="dt">The size of the value to be written.</param>
        void EmitLeImmediate(Constant c, DataType dt);

        /// <summary>
        /// Write a little-endian 16-bit unsigned integer to the stream.
        /// </summary>
        /// <param name="us">Value to write.</param>
        void EmitLeUInt16(int us);

        /// <summary>
        /// Write a little-endian 32-bit unsigned integer to the stream.
        /// </summary>
        /// <param name="ui">Value to write.</param>
        void EmitLeUInt32(uint ui);

        /// <summary>
        /// Write a string to the stream using the provided text encoding.
        /// </summary>
        /// <param name="str">Text string to write.</param>
        /// <param name="encoding">Encoding to use when writing the string.</param>
        void EmitString(string str, Encoding encoding);

        /// <summary>
        /// Collect all the bytes in the byte stream and return them as an
        /// array of bytes.
        /// </summary>
        /// <returns>A copy of all the bytes in the byte stream.</returns>
        byte[] GetBytes();

        /// <summary>
        /// Performs a patch on a big-endian value by fetching it from the stream and adding an offset. 
        /// </summary>
        /// <param name="offsetPatch">The position in the stream to be patched.</param>
        /// <param name="offsetRef">The value to patch.</param>
        /// <param name="width">The size of the patch.</param>
        void PatchBe(int offsetPatch, int offsetRef, DataType width);

        /// <summary>
        /// Performs a patch on a little-endian value by fetching it from the stream and adding an offset. 
        /// </summary>
        /// <param name="offsetPatch">The position in the stream to be patched.</param>
        /// <param name="offsetRef">The value to patch.</param>
        /// <param name="width">The size of the patch.</param>
        void PatchLe(int offsetPatch, int offsetRef, DataType width);

        /// <summary>
        /// Read the value of a big-endian 32-bit unsigned integer from the given
        /// position in the byte stream.
        /// </summary>
        /// <param name="offset">Position at which to read the value.</param>
        /// <returns>The read value.</returns>
        uint ReadBeUInt32(int offset);

        /// <summary>
        /// Read a byte from the given position in the byte stream.
        /// </summary>
        /// <param name="offset">Position at which to read the byte.</param>
        /// <returns>The read byte.</returns>
        byte ReadByte(int offset);

        /// <summary>
        /// Write some padding bytes to the byte stream.
        /// </summary>
        /// <param name="padding">Number of padding 0 bytes to write.</param>
        void Reserve(int padding);

        /// <summary>
        /// Writes a little-endian 32-bit unsigned integer to the stream at the given offset.
        /// </summary>
        /// <param name="offset">Offset at which to write the value.</param>
        /// <param name="value">The value to write.</param>
        void WriteBeUInt32(int offset, uint value);

        /// <summary>
        /// Writes a byte to the stream at the given offset.
        /// </summary>
        /// <param name="offset">Offset at which to write the byte.</param>
        /// <param name="value">The byte to write.</param>
        void WriteByte(int offset, int value);
    }

    /// <summary>
    /// This class implements the <see cref="IEmitter"/> interface and can be 
    /// used for the implementation of an assembler.
    /// </summary>
    public class Emitter : IEmitter
    {
        private readonly MemoryStream stmOut;

        /// <summary>
        /// Creates an instance of the <see cref="Emitter"/> class.
        /// </summary>
        public Emitter()
        {
            this.stmOut = new MemoryStream();
        }

        /// <summary>
        /// Creates an instance of the <see cref="Emitter"/> class. 
        /// The output of the assembler is written to a <see cref="MemoryArea"/>.
        /// </summary>
        /// <param name="mem"></param>
        public Emitter(MemoryArea mem)
        {
            var existingBytes = ((ByteMemoryArea) mem).Bytes;
            this.stmOut = new MemoryStream(existingBytes);
        }

        /// <inheritdoc/>
        public int Size
        {
            get { return (int) stmOut.Length; }
        }

        /// <inheritdoc/>
        public int Position
        {
            get { return (int) stmOut.Position; }
            set { stmOut.Position = value; }
        }

        /// <inheritdoc/>
        public byte[] GetBytes()
        {
            return stmOut.ToArray();
        }

        /// <inheritdoc/>
        public void Align(int padding, int alignment)
        {
            if (padding < 0)
                throw new ArgumentException("Padding must be >= 0.", nameof(padding));
            if ((alignment & (alignment - 1)) != 0 || alignment <= 0)
                throw new ArgumentException("Alignment must be a power of 2 larger than 0.", nameof(alignment));
            EmitBytes(0, padding);

            var mask = alignment - 1;
            while ((stmOut.Position & mask) != 0)
                stmOut.WriteByte(0);
        }

        /// <inheritdoc/>
        public void EmitByte(int b)
        {
            stmOut.WriteByte((byte) b);
        }

        /// <inheritdoc/>
        public void EmitBytes(int b, int count)
        {
            byte by = (byte) b;
            for (int i = 0; i < count; ++i)
            {
                stmOut.WriteByte(by);
            }
        }

        /// <inheritdoc/>
        public void EmitBytes(byte[] bytes)
        {
            stmOut.Write(bytes);
        }

        /// <inheritdoc/>
        public void EmitLeUInt32(uint l)
        {
            stmOut.WriteByte((byte) (l));
            l >>= 8;
            stmOut.WriteByte((byte) (l));
            l >>= 8;
            stmOut.WriteByte((byte) (l));
            l >>= 8;
            stmOut.WriteByte((byte) (l));
        }

        /// <inheritdoc/>
        public void EmitLeImmediate(Constant c, DataType dt)
        {
            switch (dt.Size)
            {
            case 1: EmitByte(c.ToInt32()); return;
            case 2: EmitLeUInt16(c.ToInt32()); return;
            case 4: EmitLeUInt32(c.ToUInt32()); return;
            default: throw new NotSupportedException(string.Format("Unsupported type: {0}", dt));
            }
        }

        /// <inheritdoc/>
        public void EmitLe(DataType vt, int v)
        {
            switch (vt.Size)
            {
            case 1: EmitByte(v); return;
            case 2: EmitLeUInt16(v); return;
            case 4: EmitLeUInt32((uint) v); return;
            default: throw new ArgumentException();
            }
        }

        /// <inheritdoc/>
        public void EmitString(string pstr, Encoding encoding)
        {
            var bytes = encoding.GetBytes(pstr);
            for (int i = 0; i != bytes.Length; ++i)
            {
                EmitByte(bytes[i]);
            }
        }

        /// <inheritdoc/>
        public void EmitLeUInt16(int s)
        {
            stmOut.WriteByte((byte) (s & 0xFF));
            stmOut.WriteByte((byte) (s >> 8));
        }

        /// <inheritdoc/>
        public void EmitBeUInt16(int s)
        {
            stmOut.WriteByte((byte) (s >> 8));
            stmOut.WriteByte((byte) (s & 0xFF));
        }

        /// <inheritdoc/>
        public void EmitBeUInt16(uint s) => EmitBeUInt16((int) s);


        /// <inheritdoc/>
        public void EmitBeUInt32(int s)
        {
            stmOut.WriteByte((byte) (s >> 24));
            stmOut.WriteByte((byte) (s >> 16));
            stmOut.WriteByte((byte) (s >> 8));
            stmOut.WriteByte((byte) (s & 0xFF));
        }

        /// <inheritdoc/>
        public void EmitBeUInt32(uint s) => EmitBeUInt32((int) s);

        /// <summary>
        /// Patches a big-endian value by fetching it from the stream and adding an offset.
        /// </summary>
        /// <param name="offsetPatch"></param>
        /// <param name="offsetRef"></param>
        /// <param name="width"></param>
        public void PatchBe(int offsetPatch, int offsetRef, DataType width)
        {
            Debug.Assert(offsetPatch < stmOut.Length);
            long posOrig = stmOut.Position;
            stmOut.Position = offsetPatch;
            int patchVal;
            switch (width.Size)
            {
            default:
                throw new ApplicationException("unexpected");
            case 1:
                patchVal = stmOut.ReadByte() + offsetRef;
                stmOut.Position = offsetPatch;
                stmOut.WriteByte((byte) patchVal);
                break;
            case 2:
                patchVal = stmOut.ReadByte() << 8;
                patchVal |= stmOut.ReadByte();
                patchVal += offsetRef;
                stmOut.Position = offsetPatch;
                stmOut.WriteByte((byte) (patchVal >> 8));
                stmOut.WriteByte((byte) patchVal);
                break;
            case 4:
                patchVal = stmOut.ReadByte() << 24;
                patchVal |= stmOut.ReadByte() << 16;
                patchVal |= stmOut.ReadByte() << 8;
                patchVal |= stmOut.ReadByte();
                patchVal += offsetRef;
                stmOut.Position = offsetPatch;
                stmOut.WriteByte((byte) (patchVal >> 24));
                stmOut.WriteByte((byte) (patchVal >> 16));
                stmOut.WriteByte((byte) (patchVal >> 8));
                stmOut.WriteByte((byte) patchVal);
                break;
            }
            stmOut.Position = posOrig;
        }

        /// <summary>
        /// Patches a little-endian value by fetching it from the stream and adding an offset.
        /// </summary>
        /// <param name="offsetPatch"></param>
        /// <param name="offsetRef"></param>
        /// <param name="width"></param>
        public void PatchLe(int offsetPatch, int offsetRef, DataType width)
        {
            Debug.Assert(offsetPatch < stmOut.Length);
            long posOrig = stmOut.Position;
            stmOut.Position = offsetPatch;
            int patchVal;
            switch (width.Size)
            {
            default:
                throw new ApplicationException("unexpected");
            case 1:
                patchVal = stmOut.ReadByte() + offsetRef;
                stmOut.Position = offsetPatch;
                stmOut.WriteByte((byte) patchVal);
                break;
            case 2:
                patchVal = stmOut.ReadByte();
                patchVal |= stmOut.ReadByte() << 8;
                patchVal += offsetRef;
                stmOut.Position = offsetPatch;
                stmOut.WriteByte((byte) patchVal);
                stmOut.WriteByte((byte) (patchVal >> 8));
                break;
            case 4:
                patchVal = stmOut.ReadByte();
                patchVal |= stmOut.ReadByte() << 8;
                patchVal |= stmOut.ReadByte() << 16;
                patchVal |= stmOut.ReadByte() << 24;
                patchVal += offsetRef;
                stmOut.Position = offsetPatch;
                stmOut.WriteByte((byte) patchVal);
                stmOut.WriteByte((byte) (patchVal >>= 8));
                stmOut.WriteByte((byte) (patchVal >>= 8));
                stmOut.WriteByte((byte) (patchVal >>= 8));
                break;
            }
            stmOut.Position = posOrig;
        }

        /// <inheritdoc/>
        public uint ReadBeUInt32(int offset)
        {
            var pos = stmOut.Position;
            stmOut.Position = offset;
            uint u0 = (byte) stmOut.ReadByte();
            uint u1 = (byte) stmOut.ReadByte();
            uint u2 = (byte) stmOut.ReadByte();
            uint u3 = (byte) stmOut.ReadByte();
            uint value = (u0 << 24) | (u1 << 16) | (u2 << 8) | u3;
            stmOut.Position = pos;
            return value;
        }

        /// <inheritdoc/>
        public byte ReadByte(int offset)
        {
            var pos = stmOut.Position;
            stmOut.Position = offset;
            byte value = (byte) stmOut.ReadByte();
            stmOut.Position = pos;
            return value;
        }

        /// <inheritdoc/>
        public void Reserve(int size)
        {
            if (size < 0) throw new NotImplementedException();
            while (size > 0)
            {
                stmOut.WriteByte(0);
                --size;
            }
        }

        /// <inheritdoc/>
        public void WriteBeUInt32(int offset, uint value)
        {
            var pos = stmOut.Position;
            stmOut.Position = offset;
            stmOut.WriteByte((byte) (value >> 24));
            stmOut.WriteByte((byte) (value >> 16));
            stmOut.WriteByte((byte) (value >> 8));
            stmOut.WriteByte((byte) value);
            stmOut.Position = pos;
        }

        /// <inheritdoc/>
        public void WriteByte(int offset, int value)
        {
            var pos = stmOut.Position;
            stmOut.Position = offset;
            stmOut.WriteByte((byte)value);
            stmOut.Position = pos;
        }
    }
}
