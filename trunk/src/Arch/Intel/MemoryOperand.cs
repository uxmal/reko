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
using System.Text;

namespace Decompiler.Arch.Intel
{
	public class MemoryOperand : Operand
	{
		public IntelRegister SegOverride = Registers.None;
		public IntelRegister Base;
		public IntelRegister Index;
		public byte Scale;
		public Value Offset;

		public MemoryOperand(PrimitiveType width) : base(width)
		{
			Base = Registers.None;
			Index = Registers.None;
			SegOverride = Registers.None;
			Scale = 1;
		}

		public MemoryOperand(PrimitiveType width, Value off) : base(width)
		{
			Offset = off;
			Base = Registers.None;
			Index = Registers.None;
			Scale = 1;
		}

		public MemoryOperand(PrimitiveType width, IntelRegister @base, Value off) : base(width)
		{
			Offset = off;
			Base = @base;
			Index = Registers.None;
			Scale = 1;
		}

		public MemoryOperand(PrimitiveType width, IntelRegister @base, IntelRegister index, byte scale,  Value off) : base(width)
		{
			Offset = off;
			Base = @base;
			Index = index;
			Scale = scale;
		}

		public bool IsAbsolute
		{
			get { return Offset.IsValid && Base == Registers.None && Index == Registers.None; }
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

			if (SegOverride != Registers.None)
			{
				sb.Append(SegOverride.ToString());
				sb.Append(":");
			}
			sb.Append("[");
			if (Base != Registers.None)
			{
				sb.Append(Base.ToString());
			}
			else
			{
				sb.Append(Offset.ToString());
			}

			if (Index != Registers.None)
			{
				sb.Append("+");
				sb.Append(Index.ToString());
				if (Scale > 1)
				{
					sb.Append("*");
					sb.Append(Scale);
				}
			}
			if (Base != Registers.None && Offset.IsValid)
			{
				if (Offset.Width == PrimitiveType.Byte || Offset.Width == PrimitiveType.SByte)
				{
					sb.Append(Offset.FormatSigned());
				}
				else
				{	
					sb.Append("+");
					sb.Append(Offset.FormatUnsigned());
				}
			}
			sb.Append("]");
			return sb.ToString();
		}

		// Given an instruction and an operand, returns the segment to use when referring to data.

		public IntelRegister DefaultSegment
		{					 
			get 
			{
				if (SegOverride != Registers.None)
					return SegOverride;
				if (Base == Registers.bp || Base == Registers.ebp || Base == Registers.sp || Base == Registers.esp)
					return Registers.ss;
				return Registers.ds;
			}
		}
	}
}
