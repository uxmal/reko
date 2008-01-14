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
using Decompiler.Core.Lib;
using Decompiler.Core.Types;
using System;
using System.Text;

namespace Decompiler.Arch.Intel
{
	public class IntelRegister : MachineRegister
	{
		protected readonly int regDword;
		protected readonly int regWord;
		protected readonly int regLoByte;
		protected readonly int regHiByte;
		protected bool isBaseRegister;

		public IntelRegister(string name, int number, PrimitiveType width, int regDword, int regWord, int regLoByte, int regHiByte)
			: base(name, number, width)
		{
			this.regDword  = regDword;
			this.regWord   = regWord;
			this.regLoByte = regLoByte;
			this.regHiByte = regHiByte;
			this.isBaseRegister = true;
		}

		public bool IsBaseRegister
		{
			get { return isBaseRegister; }
		}

		public override bool IsSubRegisterOf(MachineRegister reg2)
		{
			if (this != reg2 &&
				0 <= Number && Number <= Registers.bh.Number&&
				0 <= reg2.Number && reg2.Number <= Registers.bh.Number)
			{
				return (agrfStrictSubRegisters[reg2.Number] & (1 << Number)) != 0;
			}
			else
				return false;
		}

		internal readonly static int[] agrfStrictSubRegisters = 
		{
			0x00110100,	0x00220200, 0x00440400, 0x00880800,		// EAX, ECX, EDX, EBX 
			0x00001000, 0x00002000, 0x00004000, 0x00008000,		// ESP, EBP, ESI, EDI
			0x00110000,	0x00220000, 0x00440000, 0x00880000,		// AX, CX, DX, BX 
			0x00000000, 0x00000000, 0x00000000, 0x00000000,		// SP, BP, SI, DI
			0x00000000, 0x00000000, 0x00000000, 0x00000000,		// AL, CL, DL, BL
			0x00000000, 0x00000000, 0x00000000, 0x00000000,		// AH, CH, DH, BH
			0x00000000, 0x00000000, 0x00000000, 0x00000000,		// CS, ES, SS, DS
			0x00000000, 0x00000000, 0x00000000, 0x00000000		// FS, GS,
		};
	}

	public class Intel32Register : IntelRegister
	{
		public Intel32Register(string name, int number, int regWord, int regHiByte, int regLoByte)
			: base(name, number, PrimitiveType.Word32, number, regWord, regHiByte, regLoByte) 
		{
		}

		public override MachineRegister GetSubregister(int offset, int size)
		{
			if (offset == 0)
			{
				if (size == 16)
					return Registers.GetRegister(Number + Registers.di.Number - Registers.edi.Number);
				if (size == 32)
					return this;
			}
			return null;
		}

		public override MachineRegister GetWidestSubregister(BitSet bits)
		{
			if (bits[Number])
				return this;
			int w = Number + Registers.ax.Number - Registers.eax.Number;
			if (bits[w])
				return Registers.GetRegister(w);
			return null; 
		}

		public override void SetAliases(BitSet bits, bool f)
		{
			bits[Number] = f;
			bits[regWord] = f;
		}

		public override void SetRegisterFileValues(ulong[] registerFile, ulong value, bool[] valid)
		{
			base.SetRegisterFileValues (registerFile, value, valid);
			int r = regWord;
			registerFile[r] = value & 0xFFFFU;
			valid[r] = true;
		}
	}

	public class Intel32AccRegister : Intel32Register
	{
		public Intel32AccRegister(string name, int number, int regWord, int regLoByte, int regHiByte)
			: base(name, number, regWord, regLoByte, regHiByte)
		{
		}

		public override MachineRegister GetSubregister(int offset, int size)
		{
			if (offset == 0)
			{
				if (size == 8)
					return Registers.GetRegister(Number + Registers.al.Number);
				if (size == 16)
					return Registers.GetRegister(Number + Registers.ax.Number);
				if (size == 32)
					return this;
			}
			if (offset == 8)
			{
				if (size == 8)
					return Registers.GetRegister(Number + Registers.ah.Number - Registers.eax.Number);
			}
			return null;
		}

		public override MachineRegister GetWidestSubregister(BitSet bits)
		{
			if (bits[Number])
				return this;
			if (bits[regWord])
				return Registers.GetRegister(regWord);
			if (bits[regHiByte] && bits[regLoByte])
				return Registers.GetRegister(regWord);
			if (bits[regHiByte])
				return Registers.GetRegister(regHiByte);
			if (bits[regLoByte])
				return Registers.GetRegister(regLoByte);

			return null;
		}

		public override void SetAliases(BitSet bits, bool f)
		{
			bits[Number] = f;
			bits[regWord] = f;
			bits[regLoByte] = f;
			bits[regHiByte] = f;
		}

		public override void SetRegisterFileValues(ulong[] registerFile, ulong value, bool[] valid)
		{
			base.SetRegisterFileValues (registerFile, value, valid);
			int r = regLoByte;
			registerFile[r] = value & 0xFFU;
			valid[r] = true;
			r = regHiByte;
			registerFile[r] = (value >> 8) & 0xFFU;
			valid[r] = true;

		}

	}


	public class Intel16Register : IntelRegister
	{
		public Intel16Register(string name, int number, int regDword, int regLoByte, int regHiByte)
			: base(name, number, PrimitiveType.Word16, regDword, number, regLoByte, regHiByte)
		{
		}

		public override MachineRegister GetSubregister(int offset, int size)
		{
			if (offset == 0)
			{
				if (size == 16)
					return this;
			}
			return null;
		}

		public override MachineRegister GetWidestSubregister(BitSet bits)
		{
			int dw = Number + Registers.eax.Number - Registers.ax.Number;
			if (bits[dw])
				return this;
			if (bits[Number])
				return this;
			return null;
		}

		public override void SetRegisterFileValues(ulong[] registerFile, ulong value, bool[] valid)
		{
			base.SetRegisterFileValues(registerFile, value, valid);
			int r = regDword;
			registerFile[r] = (uint) (registerFile[r] & ~0xFFFFU) | (value & 0xFFFFU);
		}

	}

	public class Intel16AccRegister : Intel16Register
	{
		public Intel16AccRegister(string name, int number, int regDword, int regLoByte, int regHiByte)
			: base(name, number, regDword, regLoByte, regHiByte)
		{
		}

		public override MachineRegister GetSubregister(int offset, int size)
		{
			if (offset == 0)
			{
				if (size == 8)
					return Registers.GetRegister(Number + Registers.al.Number - Registers.ax.Number);
				if (size >= 16)
					return this;
			}
			if (offset == 8)
			{
				if (size == 8)
					return Registers.GetRegister(Number + Registers.ah.Number - Registers.ax.Number);
			}
			return null;
		}

		public override MachineRegister GetWidestSubregister(BitSet bits)
		{
			if (bits[Number])
				return this;
			int dw = Number + Registers.eax.Number - Registers.ax.Number;
			if (bits[dw])
				return this;
			int h = Number + Registers.ah.Number - Registers.ax.Number;
			int l = Number + Registers.al.Number - Registers.ax.Number;
			if (bits[h] && bits[l])
				return this;
			if (bits[h])
				return Registers.GetRegister(h);
			if (bits[l])
				return Registers.GetRegister(l);

			return null;
		}

		public override void SetAliases(BitSet bits, bool f)
		{
			bits[regWord] = f;
			bits[regLoByte] = f;
			bits[regHiByte] = f;
		}

		public override void SetRegisterFileValues(ulong[] registerFile, ulong value, bool[] valid)
		{
			base.SetRegisterFileValues(registerFile, value, valid);
			int r = regLoByte;
			registerFile[r] = value & 0xFFU;
			valid[r] = true;
			r = regHiByte;
			registerFile[r] = (value >> 8) & 0xFFU;
			valid[r] = true;
		}
	}

	public class IntelLoByteRegister : IntelRegister
	{
		public IntelLoByteRegister(string name, int number, int regDword, int regWord, int regLoByte, int regHiByte)
			: base(name, number, PrimitiveType.Byte, regDword, regWord, regLoByte, regHiByte)
		{
		}

		public override MachineRegister GetSubregister(int offset, int size)
		{
			if (offset == 0)
			{
				if (size == 8)
					return this;
			}
			return null;
		}

		public override MachineRegister GetWidestSubregister(BitSet bits)
		{
			int dw = Number + Registers.eax.Number - Registers.al.Number;
			if (bits[dw])
				return this;
			int w = Number + Registers.ax.Number - Registers.al.Number;
			if (bits[w])
				return this;

			if (bits[Number])
				return this;

			return null;
		}

		public override void SetAliases(BitSet bits, bool f)
		{
			bits[Number] = f;
			if (!f && bits[regWord])
				bits[regHiByte] = true;
			bits[regWord] = f;
		}

		public override void SetRegisterFileValues(ulong[] registerFile, ulong value, bool[] valid)
		{
			base.SetRegisterFileValues(registerFile, value, valid);
			int r = regWord;
			registerFile[r] = (ulong) (registerFile[r] & ~0xFFU) | (value & 0xFF);
			valid[r] = valid[regHiByte];
			r = regDword;
			registerFile[r] = (ulong) (registerFile[r] & ~0xFFU) | (value & 0xFF);
		}
	}

	public class IntelHiByteRegister : IntelRegister
	{
		public IntelHiByteRegister(string name, int number, int regDword, int regWord, int regLoByte, int regHiByte)
			: base(name, number, PrimitiveType.Byte, regDword, regWord, regLoByte, regHiByte)
		{
		}

		public override int AliasOffset
		{
			get { return 8; }
		}

		public override MachineRegister GetSubregister(int offset, int size)
		{
			if (offset == 0)
			{
				if (size == 16)
					return Registers.GetRegister(Number + Registers.ax.Number - Registers.ah.Number);
				if (size == 8)
					return this;
			}
			if (offset == 8)
			{
				if (size == 8)
					return this;
			}
			return null;
		}
	
		public override MachineRegister GetWidestSubregister(BitSet bits)
		{
			int dw = Number + Registers.eax.Number - Registers.ah.Number;
			if (bits[dw])
				return this;
			int w = Number + Registers.ax.Number - Registers.ah.Number;
			if (bits[w])
				return this;

			if (bits[Number])
				return this;

			return null;
		}

		public override void SetAliases(BitSet bits, bool f)
		{
			bits[Number] = f;
			if (!f && bits[regWord])
				bits[regLoByte] = true;
			bits[regWord] = f;
		}

		public override void SetRegisterFileValues(ulong[] registerFile, ulong value, bool[] valid)
		{
			base.SetRegisterFileValues(registerFile, value, valid);
			int r = regWord;
			registerFile[r] = (ulong) (registerFile[r] & ~0xFF00U) | (value << 8);
			valid[r] = valid[regLoByte];
			r = regDword;
			registerFile[r] = (ulong) (registerFile[r] & ~0xFF00U) | (value << 8);
		}
	}

	public class SegmentRegister : IntelRegister
	{
		public SegmentRegister(string name, int number)
			: base(name, number, PrimitiveType.Segment, -1, number, -1, -1)
		{
			isBaseRegister = false;
		}

		public override MachineRegister GetSubregister(int offset, int size)
		{
			if (offset == 0)
			{
				if (size == 16)
					return this;
			}
			return null;
		}

		public override MachineRegister GetWidestSubregister(BitSet bits)
		{
			if (bits[Number])
				return this;
			return null;
		}
	}

	public class FlagRegister : IntelRegister
	{
		public FlagRegister(string name, int number)
			: base(name, number, PrimitiveType.Bool, -1, -1, -1, -1)
		{
			isBaseRegister = false;
		}

		public override bool IsAluRegister
		{
			get { return false; }
		}

		public override MachineRegister GetWidestSubregister(BitSet bits)
		{
			if (bits[Number])
				return this;
			return null;
		}
	}
}
