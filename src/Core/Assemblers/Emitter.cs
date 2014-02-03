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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Core.Assemblers
{
	/// <summary>
	/// Emits bytes into a bytestream belonging to a segment/section.
	/// </summary>
    /// //$TODO: create BeEmitter and LeEmitter subclasses.
	public class Emitter
	{
		private MemoryStream stmOut = new MemoryStream();

		public byte [] Bytes
		{
			get { return stmOut.ToArray(); }
		}

        public void Align(int skip, int alignment)
        {
            if (skip < 0)
                throw new ArgumentException("skip", "Argument must be >= 0.");
            if ((alignment & (alignment - 1)) != 0 || alignment <= 0)
                throw new ArgumentException("alignment", "Alignment must be a power of 2 largen than 0.");
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

		public void EmitLeUint32(uint l)
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
			case 2: EmitLeUint16(c.ToInt32()); return;
			case 4: EmitLeUint32(c.ToUInt32()); return;
			default: throw new NotSupportedException(string.Format("Unsupported type: {0}", dt));
			}
		}

		public void EmitLe(PrimitiveType vt, int v)
		{
			switch (vt.Size)
			{
			case 1: EmitByte(v); return;
			case 2: EmitLeUint16(v); return;
			case 4: EmitLeUint32((uint) v); return;
			default: throw new ArgumentException();
			}
		}

		public void EmitString(string pstr)
		{
			for (int i = 0; i != pstr.Length; ++i)
			{
				EmitByte(pstr[i]);
			}
		}

        public void EmitLeUint16(int s)
		{
			stmOut.WriteByte((byte)(s & 0xFF));
			stmOut.WriteByte((byte)(s >> 8));
		}

        public void EmitBeUInt16(int s)
        {
            stmOut.WriteByte((byte)(s >> 8));
            stmOut.WriteByte((byte)(s & 0xFF));
        }

        public void EmitBeUint32(int s)
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
        /// Patches a value by fetching it from the stream and adding an offset.
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
		/// Patches a value by fetching it from the stream and adding an offset.
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
    }
}
