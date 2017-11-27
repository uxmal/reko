#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.Arch.Microchip.PIC18
{
    /// <summary>
    /// The state of an PIC18 processor. Used in the Scanning phase of the decompiler.
    /// </summary>
    public class PIC18State : ProcessorState 
	{
		private ulong[] regs;              // register values
		private ulong[] valid;             // masks out only valid bits
		private uint flags;
		private uint validFlags;
		private PIC18Architecture arch;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="arch">The PIC18 target architecture.</param>
		public PIC18State(PIC18Architecture arch)
		{
			this.arch = arch;
			this.regs = new ulong[Registers.Max];
			this.valid = new ulong[Registers.Max];
		}

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="st">The PIC18 state to copy.</param>
		public PIC18State(PIC18State st) : base(st)
		{
			arch = st.arch;
			regs = (ulong[])st.regs.Clone();
			valid = (ulong[])st.valid.Clone();
		}

		public override IProcessorArchitecture Architecture { get { return arch; } }

		public override ProcessorState Clone()
		{
			return new PIC18State(this);
		}

		public bool IsValid(RegisterStorage reg)
		{
			return (valid[reg.Number] & reg.BitMask) == reg.BitMask;
		}

		public override Constant GetRegister(RegisterStorage reg)
		{
			if (IsValid(reg))
			{
				var val = (regs[reg.Number] & reg.BitMask) >> (int)reg.BitAddress;
				return Constant.Create(reg.DataType, val);
			}
			else
				return Constant.Invalid;
		}

		public override void SetRegister(RegisterStorage reg, Constant c)
		{
			if (c == null || !c.IsValid)
			{
				valid[reg.Number] &= ~reg.BitMask;
			}
			else
			{
				valid[reg.Number] |= reg.BitMask;
				regs[reg.Number] = (regs[reg.Number] & ~reg.BitMask) | (c.ToUInt64() << (int)reg.BitAddress);
			}
		}

		public override void SetInstructionPointer(Address addr)
		{
            ulong off = addr.ToUInt32();
            SetRegister(Registers.pcl, Constant.Byte((byte)(off & 0xFF)));
            SetRegister(Registers.pclh, Constant.Byte((byte)((off>>8) & 0xFF)));
            SetRegister(Registers.pclu, Constant.Byte((byte)((off>>16) & 0xFF)));
        }

        public override void OnProcedureEntered()
		{
		}

		public override void OnProcedureLeft(FunctionType sig)
		{
		}

		public override CallSite OnBeforeCall(Identifier sp, int returnAddressSize)
		{
			return new CallSite(returnAddressSize, 0);
		}

		public override void OnAfterCall(FunctionType sig)
		{
		}



	}

}
