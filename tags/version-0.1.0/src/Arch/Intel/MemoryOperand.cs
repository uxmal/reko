/* 
 * Copyright (C) 1999-2009 John Källén.
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
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Text;

namespace Decompiler.Arch.Intel
{
	public class MemoryOperand : MachineOperand
	{
		public MachineRegister SegOverride = MachineRegister.None;
		public MachineRegister Base;
		public MachineRegister Index;
		public byte Scale;
		private Constant offset;

		public MemoryOperand(PrimitiveType width) : base(width)
		{
			Base = MachineRegister.None;
            Index = MachineRegister.None;
            SegOverride = MachineRegister.None;
			Scale = 1;
		}

		public MemoryOperand(PrimitiveType width, Constant off) : base(width)
		{
			offset = off;
            Base = MachineRegister.None;
            Index = MachineRegister.None;
			Scale = 1;
		}

		public MemoryOperand(PrimitiveType width, MachineRegister @base, Constant off) : base(width)
		{
			offset = off;
			Base = @base;
            Index = MachineRegister.None;
			Scale = 1;
		}

		public MemoryOperand(PrimitiveType width, MachineRegister @base, MachineRegister index, byte scale,  Constant off) : base(width)
		{
			offset = off;
			Base = @base;
			Index = index;
			Scale = scale;
		}

		public bool IsAbsolute
		{
            get { return offset.IsValid && Base == MachineRegister.None && Index == MachineRegister.None; }
		}

		public Constant Offset
		{
			get { return offset; }
			set { offset = value; }
		}

		public override string ToString()
		{
			return ToString(true);
		}

		public override string ToString(bool fExplicit)
		{
			StringBuilder sb = new StringBuilder();
			if (fExplicit)
			{
				string s;
				if (Width == PrimitiveType.Byte)
					s = "byte ptr ";
				else if (Width == PrimitiveType.Word16)
					s = "word ptr ";
				else if (Width.Size == 4)
					s = "dword ptr ";
				else if (Width == PrimitiveType.Word64)
					s = "qword ptr ";
				else if (Width == PrimitiveType.Real32)
					s = "float ptr ";
				else if (Width == PrimitiveType.Real64)
					s = "double ptr ";
				else if (Width == PrimitiveType.Real80)
					s = "tword ptr ";
				else
					throw new ArgumentOutOfRangeException();
				sb.Append(s);
			}

            if (SegOverride != MachineRegister.None)
			{
				sb.Append(SegOverride.ToString());
				sb.Append(":");
			}
			sb.Append("[");
			if (Base != MachineRegister.None)
			{
				sb.Append(Base.ToString());
			}
			else
			{
				sb.AppendFormat(FormatUnsignedValue(offset));
			}

			if (Index != MachineRegister.None)
			{
				sb.Append("+");
				sb.Append(Index.ToString());
				if (Scale > 1)
				{
					sb.Append("*");
					sb.Append(Scale);
				}
			}
			if (Base != MachineRegister.None && offset != null && offset.IsValid)
			{
				if (offset.DataType == PrimitiveType.Byte || offset.DataType == PrimitiveType.SByte)
				{
					sb.Append(FormatSignedValue(offset));
				}
				else
				{	
					sb.Append("+");
					sb.Append(FormatUnsignedValue(offset));
				}
			}
			sb.Append("]");
			return sb.ToString();
		}

		// Given an instruction and an operand, returns the segment to use when referring to data.

		public MachineRegister DefaultSegment
		{					 
			get 
			{
				if (SegOverride != MachineRegister.None)
					return SegOverride;
				if (Base == Registers.bp || Base == Registers.ebp || Base == Registers.sp || Base == Registers.esp)
					return Registers.ss;
				return Registers.ds;
			}
		}
	}
}
