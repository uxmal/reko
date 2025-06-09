
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using System.Numerics;

namespace Reko.Arch.X86
{
    /// <summary>
    /// The state of an X86 processor. Used in the Scanning phase of the decompiler.
    /// </summary>
	public class X86State : ProcessorState
	{
		private ulong [] regs;              // register values
        private BigInteger[] xmmregs;       // XMM register values
		private ulong [] valid;             // masks out only valid bits
        private uint flags;
        private uint validFlags;
        private IntelArchitecture arch;

        private static readonly int ymm = Registers.ymm0.Number;
        

        private const int StackItemSize = 2;

		public X86State(IntelArchitecture arch)
		{
            this.arch = arch;
			this.regs = new ulong[(int)Registers.Max];
            this.xmmregs = new BigInteger[16];
            this.valid = new ulong[(int)Registers.Max];
		}

		public X86State(X86State st) : base(st)
		{
            arch = st.arch;
            regs = (ulong[])st.regs.Clone();
            xmmregs = (BigInteger[]) st.xmmregs.Clone();
			valid = (ulong []) st.valid.Clone();
		}

        public override IProcessorArchitecture Architecture { get { return arch; } }

		public Address? AddressFromSegOffset(RegisterStorage seg, uint offset)
		{
			Constant c = GetRegister(seg);
			if (c.IsValid)
			{
				return arch.ProcessorMode.CreateSegmentedAddress((ushort) c.ToUInt32(), offset & 0xFFFF);
			}
			else
				return null;
		}

        public Address? AddressFromSegReg(RegisterStorage seg, RegisterStorage reg)
		{
			Constant c = GetRegister(reg);
			if (c.IsValid)
			{
				return AddressFromSegOffset(seg, c.ToUInt32());
			}
			else 
				return null;
		}

		public override ProcessorState Clone()
		{
			return new X86State(this);
		}

        public bool IsValid(RegisterStorage reg)
        {
            return (valid[reg.Number] & reg.BitMask) == reg.BitMask;
        }

        public override Constant GetRegister(RegisterStorage reg)
        {
            if (IsValid(reg))
            {
                var y = reg.Number - ymm;
                if (0 <= y && y < 16)
                {
                    var val = (xmmregs[y] & reg.BitMask) >> (int) reg.BitAddress;
                    return new BigConstant(reg.DataType, val);
                }
                else
                {
                    var val = (regs[reg.Number] & reg.BitMask) >> (int) reg.BitAddress;
                    return Constant.Create(reg.DataType, val);
                }
            }
            else
                return InvalidConstant.Create(reg.DataType);
        }

		public override void SetRegister(RegisterStorage reg, Constant c)
		{
			if (c is null || !c.IsValid)
			{
                valid[reg.Number] &= ~reg.BitMask;
			}
			else
			{
                valid[reg.Number] |= reg.BitMask;
                var y = reg.Number - ymm;
                if (0 <= y && y < 16)
                {
                    xmmregs[y] = (regs[y] & ~reg.BitMask) | (c.ToBigInteger() << (int) reg.BitAddress);
                }
                else
                {
                    regs[reg.Number] = (regs[reg.Number] & ~reg.BitMask) | (c.ToUInt64() << (int) reg.BitAddress);
                }
			}
		}

        public override Address InstructionPointer
        {
            get
            {
                return base.InstructionPointer;
            }

            set
            {
                base.InstructionPointer = value;
                if (value.Selector.HasValue)
                    SetRegister(Registers.cs, Constant.Word16(value.Selector.Value));
            }
        }

        public override void OnProcedureEntered(Address addr)
        {
            // We're making an assumption that the direction flag is always clear
            // when a procedure is entered. This is true of the vast majority of
            // x86 code out there, and the assumption is certainly made by most
            // compilers and code libraries. If you know the DF flag is set on
            // procedure entry, you can manually set that flag using a user-
            // defined register value.
            SetFlagGroup(arch.GetFlagGroup(Registers.eflags, (uint) FlagM.DF), Constant.False());
            if (addr.Selector.HasValue)
            {
                var cs = Constant.Create(PrimitiveType.SegmentSelector, addr.Selector.Value);
                SetRegister(Registers.cs, cs);
            }
        }

        public override void OnProcedureLeft(FunctionType sig)
        {
        }

        public override CallSite OnBeforeCall(Identifier sp, int returnAddressSize)
        {
            return new CallSite(returnAddressSize, 0);
        }

        public override void OnAfterCall(FunctionType? sig)
        {
        }

        public Constant GetFlagGroup(uint mask)
        {
            bool sigle = Bits.IsSingleBitSet(mask);
            if ((mask & validFlags) == mask)
            {
                if (sigle)
                {
                    return Constant.Bool((flags & mask) != 0);
                }
                else {
                    return Constant.Byte((byte)(flags & mask));
                }
            }
            else 
            {
                return InvalidConstant.Create(PrimitiveType.Byte);
            }
        }

        public void SetFlagGroup(FlagGroupStorage reg, Constant value)
        {
            uint mask = reg.FlagGroupBits;
            if (value.IsValid)
            {
                validFlags |= mask;
                if (value.ToBoolean())
                {
                    this.flags |= mask;
                }
                else
                {
                    this.flags &= ~mask;
                }
            }
            else
            {
                validFlags &= ~mask;
            }
        }

    }
}
