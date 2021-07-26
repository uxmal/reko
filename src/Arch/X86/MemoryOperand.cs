#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Text;

namespace Reko.Arch.X86
{
	public class MemoryOperand : MachineOperand
	{
		public RegisterStorage SegOverride  {get;set;}
        public RegisterStorage Base { get; set; }
        public RegisterStorage Index { get; set; }
        public byte Scale { get; set; }
		public Constant? Offset {get;set;}

		public MemoryOperand(PrimitiveType width) : base(width)
		{
            SegOverride = RegisterStorage.None;
			Base = RegisterStorage.None;
            Index = RegisterStorage.None;
            SegOverride = RegisterStorage.None;
			Scale = 1;
		}

		public MemoryOperand(PrimitiveType width, Constant? off) : base(width)
		{
            SegOverride = RegisterStorage.None;
            Base = RegisterStorage.None;
			Offset = off;
            Index = RegisterStorage.None;
			Scale = 1;
		}

		public MemoryOperand(PrimitiveType width, RegisterStorage @base, Constant? off) : base(width)
		{
            SegOverride = RegisterStorage.None;
			Base = @base;
			Offset = off;
            Index = RegisterStorage.None;
			Scale = 1;
		}

		public MemoryOperand(PrimitiveType width, RegisterStorage @base, RegisterStorage index, byte scale, Constant? off) : base(width)
		{
            SegOverride = RegisterStorage.None;
			Base = @base;
			Offset = off;
			Index = index;
			Scale = scale;
		}

		public bool IsAbsolute
		{
            get { return Offset != null && Offset.IsValid && Base == RegisterStorage.None && Index == RegisterStorage.None; }
		}

		protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
			if ((options.Flags & MachineInstructionRendererFlags.ExplicitOperandSize) != 0)
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
                else if (Width == PrimitiveType.Real80 || Width == PrimitiveType.Bcd80)
                    s = "tword ptr ";
                else if (Width == PrimitiveType.Word128)
                    s = "xmmword ptr ";
                else if (Width == PrimitiveType.Word256)
                    s = "ymmword ptr ";
                else
                    s = "";
				renderer.WriteString(s);
			}

            if (SegOverride != RegisterStorage.None)
			{
				renderer.WriteString(SegOverride.ToString());
				renderer.WriteString(":");
			}
			renderer.WriteString("[");
			if (Base != RegisterStorage.None)
			{
				renderer.WriteString(Base.ToString());
			}
			else
			{
                var s = FormatUnsignedValue(Offset!);
				renderer.WriteAddress(s, Address.FromConstant(Offset!));
			}

			if (Index != RegisterStorage.None)
			{
				renderer.WriteString("+");
				renderer.WriteString(Index.ToString());
				if (Scale > 1)
				{
					renderer.WriteString("*");
					renderer.WriteUInt32(Scale);
				}
			}
			if (Base != RegisterStorage.None && Offset != null && Offset.IsValid)
			{
				if (Offset.DataType == PrimitiveType.Byte || Offset.DataType == PrimitiveType.SByte)
				{
					renderer.WriteString(FormatSignedValue(Offset));
				}
				else
				{
                    var off = Offset.ToInt32();
                    if (off == Int32.MinValue)
                    {
                        renderer.WriteString("-80000000");
                    }
                    else
                    {
                        var absOff = Math.Abs(off);
                        if (Offset.DataType.Size > 2 && off < 0 && absOff < 0x10000)
                        {
                            // Special case for negative 32-bit offsets whose 
                            // absolute value < 0x10000 (GitHub issue #252)
                            renderer.WriteString("-");
                            renderer.WriteFormat("{0:X8}", absOff);
                        }
                        else
                        {
                            renderer.WriteString("+");
                            renderer.WriteString(FormatUnsignedValue(Offset));
                        }
                    }
				}
			}
			renderer.WriteString("]");
		}

		/// <summary>
        /// Returns the segment to use when referring to data unless an overriding segment was specfied.
		/// </summary>
		public RegisterStorage DefaultSegment
		{					 
			get 
			{
				if (SegOverride != RegisterStorage.None)
					return SegOverride!;
				if (Base == Registers.bp || Base == Registers.ebp || Base == Registers.sp || Base == Registers.esp)
					return Registers.ss;
				return Registers.ds;
			}
		}
	}
}
