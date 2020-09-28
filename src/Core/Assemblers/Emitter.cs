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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
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
        int Length { get; }
        int Position { get; }

        void Align(int extra, int align);
        void EmitBeUInt16(int us);
        void EmitBeUInt32(int ui);
        void EmitByte(int b);
        void EmitBytes(int b, int count);
        void EmitLe(PrimitiveType width, int value);
        void EmitLeImmediate(Constant c, PrimitiveType dt);
        void EmitLeUInt16(int us);
        void EmitLeUInt32(uint ui);
        void EmitString(string str, Encoding encoding);
        byte[] GetBytes();
        void PatchBe(int offsetPatch, int offsetRef, DataType width);
        void PatchLe(int offsetPatch, int offsetRef, DataType width);
        void Reserve(int delta);
    }
    
	public class Emitter : IEmitter
	{
		private MemoryStream stmOut = new MemoryStream();

        public byte[] GetBytes()
        {
            return stmOut.ToArray();
        }

        public void Align(int skip, int alignment)
        {
            if (skip < 0)
                throw new ArgumentException(nameof(skip), "Argument must be >= 0.");
            if ((alignment & (alignment - 1)) != 0 || alignment <= 0)
                throw new ArgumentException(nameof(alignment), "Alignment must be a power of 2 larger than 0.");
            EmitBytes(0, skip);

            var mask = alignment - 1;
            while ((stmOut.Position & mask) != 0)
                stmOut.WriteByte(0);
        }

		public void EmitByte(int b)
		{
			stmOut.WriteByte((byte) b);
		}

		public void EmitBytes(int b, int count)
		{
			byte by = (byte) b;
			for (int i = 0; i < count; ++i)
			{
				stmOut.WriteByte(by);
			}
		}

		public void EmitLeUInt32(uint l)
		{
			stmOut.WriteByte((byte)(l));
			l >>= 8;
			stmOut.WriteByte((byte)(l));
			l >>= 8;
			stmOut.WriteByte((byte)(l));
			l >>= 8;
			stmOut.WriteByte((byte)(l));
		}

		public void EmitLeImmediate(Constant c, PrimitiveType dt)
		{
			switch (dt.Size)
			{
			case 1: EmitByte(c.ToInt32()); return;
			case 2: EmitLeUInt16(c.ToInt32()); return;
			case 4: EmitLeUInt32(c.ToUInt32()); return;
			default: throw new NotSupportedException(string.Format("Unsupported type: {0}", dt));
			}
		}

		public void EmitLe(PrimitiveType vt, int v)
		{
			switch (vt.Size)
			{
			case 1: EmitByte(v); return;
			case 2: EmitLeUInt16(v); return;
			case 4: EmitLeUInt32((uint) v); return;
			default: throw new ArgumentException();
			}
		}

		public void EmitString(string pstr, Encoding encoding)
		{
            var bytes = encoding.GetBytes(pstr);
			for (int i = 0; i != bytes.Length; ++i)
			{
				EmitByte(bytes[i]);
			}
		}

        public void EmitLeUInt16(int s)
		{
			stmOut.WriteByte((byte)(s & 0xFF));
			stmOut.WriteByte((byte)(s >> 8));
		}

        public void EmitBeUInt16(int s)
        {
            stmOut.WriteByte((byte)(s >> 8));
            stmOut.WriteByte((byte)(s & 0xFF));
        }

        public void EmitBeUInt32(int s)
        {
            stmOut.WriteByte((byte) (s >> 24));
            stmOut.WriteByte((byte) (s >> 16));
            stmOut.WriteByte((byte) (s >> 8));
            stmOut.WriteByte((byte) (s & 0xFF));
        }

		public int Length
		{
			get { return (int) stmOut.Length; }
		}

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

		public int Position
		{
			get { return (int) stmOut.Position; }
		}

        public void Reserve(int size)
        {
            if (size < 0) throw new NotImplementedException();
            while (size > 0)
            {
                stmOut.WriteByte(0);
                --size;
            }
        }
    }
}
