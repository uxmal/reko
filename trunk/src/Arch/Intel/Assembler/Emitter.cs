/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Arch.Intel.Assembler
{
	/// <summary>
	/// Emits bytes into a bytestream belonging to a segment/section.
	/// </summary>
	public class Emitter
	{
		private IntelRegister segOverride;
		private PrimitiveType dataWidthSeg;	// Default data width for this segment.
		private PrimitiveType addrWidthSeg;	// Default address width for this segment.
		private PrimitiveType addrWidth;	// width of the address of this opcode.

		private MemoryStream stmOut = new MemoryStream();

		public Emitter()
		{
			segOverride = Registers.None;
		}

		public PrimitiveType AddressWidth
		{
			get { return addrWidth; }
			set { addrWidth = value; }
		}

		public byte [] Bytes
		{
			get { return stmOut.ToArray(); }
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

		public void EmitDword(uint l)
		{
			stmOut.WriteByte((byte)(l));
			l >>= 8;
			stmOut.WriteByte((byte)(l));
			l >>= 8;
			stmOut.WriteByte((byte)(l));
			l >>= 8;
			stmOut.WriteByte((byte)(l));
		}

		public void EmitImmediate(Value v, PrimitiveType w)
		{
			switch (w.Size)
			{
			case 1: EmitByte(v.Byte); return;
			case 2: EmitWord(v.Word); return;
			case 4: EmitDword(v.Unsigned); return;
			}
		}

		public void EmitInteger(PrimitiveType vt, int v)
		{
			switch (vt.Size)
			{
			case 1: EmitByte(v); return;
			case 2: EmitWord(v); return;
			case 4: EmitDword((uint) v); return;
			default: throw new ArgumentException();
			}
		}

		public void EmitOpcode(int b, PrimitiveType dataWidth)
		{
			if (SegmentOverride != Registers.None)
			{
				byte bOv;
				if (SegmentOverride == Registers.es) bOv = 0x26; else
				if (SegmentOverride == Registers.cs) bOv = 0x2E; else
				if (SegmentOverride == Registers.ss) bOv = 0x36; else
				if (SegmentOverride == Registers.ds) bOv = 0x3E; else
				if (SegmentOverride == Registers.fs) bOv = 0x64; else
				if (SegmentOverride == Registers.gs) bOv = 0x65; else
				throw new ArgumentOutOfRangeException("Invalid segment register: " + SegmentOverride);
				EmitByte(bOv);
				SegmentOverride = Registers.None;
			}
			if (IsDataWidthOverridden(dataWidth))
			{
				EmitByte(0x66);
			}
			if (AddressWidth != null && AddressWidth != SegmentAddressWidth)
			{
				EmitByte(0x67);
			}
			EmitByte(b);
		}

		public void EmitString(string pstr)
		{
			for (int i = 0; i != pstr.Length; ++i)
			{
				EmitByte(pstr[i]);
			}
		}

		public void EmitWord(int s)
		{
			stmOut.WriteByte((byte)(s & 0xFF));
			stmOut.WriteByte((byte)(s >> 8));
		}

		private bool IsDataWidthOverridden(PrimitiveType dataWidth)
		{
			return dataWidth != null && 
				dataWidth.Domain != Domain.Real &&
				dataWidth.Size != 1 && 
				dataWidth != SegmentDataWidth;
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
		public void Patch(int offsetPatch, int offsetRef, PrimitiveType width)
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

		public PrimitiveType SegmentAddressWidth
		{
			get { return addrWidthSeg; }
			set { addrWidthSeg = value; }
		}

		public PrimitiveType SegmentDataWidth
		{
			get { return dataWidthSeg; }
			set { dataWidthSeg = value; }
		}

		public IntelRegister SegmentOverride
		{
			get { return segOverride; }
			set { segOverride = value; }
		}
	}
}
