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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.PowerPC
{
    public partial class PowerPcDisassembler : DisassemblerBase<PowerPcInstruction, Mnemonic>
    {
        private readonly PowerPcArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly PrimitiveType defaultWordWidth;
        private readonly Decoder[] primaryDecoders;
        private Address addr;
        private bool allowSetCR0;
        private readonly List<MachineOperand> ops;

        public PowerPcDisassembler(PowerPcArchitecture arch, Decoder [] primaryDecoders, EndianImageReader rdr, PrimitiveType defaultWordWidth)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.defaultWordWidth = defaultWordWidth;
            this.primaryDecoders = primaryDecoders;
            this.ops = new List<MachineOperand>();
        }

        public override PowerPcInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint wInstr))
                return null;
            this.allowSetCR0 = false;
            this.ops.Clear();
            var instrCur = primaryDecoders[wInstr >> 26].Decode(this, wInstr);
            if (wInstr == 0)
                instrCur.InstructionClass |= InstrClass.Zero;
            instrCur.Address = addr;
            instrCur.Length = 4;
            return instrCur;
        }

        public override PowerPcInstruction CreateInvalidInstruction()
        {
            return new PowerPcInstruction(Mnemonic.illegal)
            {
                InstructionClass = InstrClass.Invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        public override PowerPcInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new PowerPcInstruction(mnemonic)
            {
                Address = addr,
                InstructionClass = iclass,
                Length = 4,
                Operands = ops.ToArray(),
                setsCR0 = allowSetCR0,
            };
        }

        #region Mutators

        // If the instructions LSB is '1', then set the setsCR0
        internal static bool C(uint wInstr, PowerPcDisassembler dasm)
        {
            dasm.allowSetCR0 = (wInstr & 1) != 0;
            return true;
        }
        
        // Force the setsCR0 flag to '1'.
        internal static bool CC(uint uInstr, PowerPcDisassembler dasm)
        {
            dasm.allowSetCR0 = true;
            return true;
        }

        // Signed integer in bottom 16 bits of instruction.
        internal static bool S(uint wInstr, PowerPcDisassembler dasm)
        {
            dasm.ops.Add(new ImmediateOperand(Constant.Int16((short) wInstr)));
            return true;
        }

        // Unsigned integer in bottom 16 bits of instruction.
        internal static bool U(uint wInstr, PowerPcDisassembler dasm)
        {
            dasm.ops.Add(new ImmediateOperand(Constant.Word16((ushort) wInstr)));
            return true;
        }

        internal static Mutator<PowerPcDisassembler> E(int bitPos)
        {
            return (u, d) =>
            {
                var op = d.MemOff(u >> bitPos, u);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> E1 = E(21);
        internal static readonly Mutator<PowerPcDisassembler> E2 = E(16);
        internal static readonly Mutator<PowerPcDisassembler> E3 = E(11);
        internal static readonly Mutator<PowerPcDisassembler> E4 = E(6);

        internal static bool E2_2(uint wInstr, PowerPcDisassembler dasm)
        {
            if ((wInstr & 0x3) != 0)
                return false;
            var op = dasm.MemOff(wInstr >> 16, wInstr);
            dasm.ops.Add(op);
            return true;
        }

        internal static Mutator<PowerPcDisassembler> c(int bitPos)
        {
            return (u, d) =>
            {
                var op = d.CRegFromBits(u >> bitPos);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> c1 = c(21);
        internal static readonly Mutator<PowerPcDisassembler> c2 = c(16);
        internal static readonly Mutator<PowerPcDisassembler> c3 = c(11);
        internal static readonly Mutator<PowerPcDisassembler> c4 = c(6);

        // CR field in certain opcodes.

        internal static Mutator<PowerPcDisassembler> Cr(int offset)
        {
            return (u, d) =>
            {
                var op = d.CRegFromBits(u >> offset);
                d.ops.Add(op);
                return true;
            };
        }

        internal static readonly Mutator<PowerPcDisassembler> C1 = Cr(23);
        internal static readonly Mutator<PowerPcDisassembler> C2 = Cr(18);

        internal static Mutator<PowerPcDisassembler> f(int offset)
        {
            return (u, d) =>
            {
                var op = d.FRegFromBits(u >> offset);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> f1 = f(21);
        internal static readonly Mutator<PowerPcDisassembler> f2 = f(16);
        internal static readonly Mutator<PowerPcDisassembler> f3 = f(11);
        internal static readonly Mutator<PowerPcDisassembler> f4 = f(6);

        internal static Mutator<PowerPcDisassembler> p(int offset)
        {
            return (u, d) =>
            {
                var op = d.FRegFromBits(u >> offset);
                // The floating point register must be even.
                if ((op.Register.Number & 1) == 1)
                    return false;
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> p1 = f(21);
        internal static readonly Mutator<PowerPcDisassembler> p2 = f(16);
        internal static readonly Mutator<PowerPcDisassembler> p3 = f(11);
        internal static readonly Mutator<PowerPcDisassembler> p4 = f(6);

        internal static Mutator<PowerPcDisassembler> r(int offset)
        {
            return (u, d) =>
            {
                var op = d.RegFromBits(u >> offset);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> r1 = r(21);
        internal static readonly Mutator<PowerPcDisassembler> r2 = r(16);
        internal static readonly Mutator<PowerPcDisassembler> r3 = r(11);
        internal static readonly Mutator<PowerPcDisassembler> r4 = r(6);

        internal static Mutator<PowerPcDisassembler> v(int offset)
        {
            return (u, d) =>
            {
                var op = d.VRegFromBits((u >> offset) & 0x1F);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> v1 = v(21);
        internal static readonly Mutator<PowerPcDisassembler> v2 = v(16);
        internal static readonly Mutator<PowerPcDisassembler> v3 = v(11);
        internal static readonly Mutator<PowerPcDisassembler> v4 = v(6);

        // vector register using only 3 bits of encoding
        internal static Mutator<PowerPcDisassembler> v3_(int offset)
        {
            return (u, d) =>
            {
                var op = d.VRegFromBits((u >> offset) & 0x7);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> v3_6 = v3_(6);


        internal static Mutator<PowerPcDisassembler> I(int offset)
        {
            return (u, d) =>
            {
                var op = ImmediateOperand.Byte((byte) ((u >> offset) & 0x1F));
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> I1 = I(21);
        internal static readonly Mutator<PowerPcDisassembler> I2 = I(16);
        internal static readonly Mutator<PowerPcDisassembler> I3 = I(11);
        internal static readonly Mutator<PowerPcDisassembler> I4 = I(6) ;
        internal static readonly Mutator<PowerPcDisassembler> I5 = I(1);

        // Condition register fields.
        internal static bool M(uint wInstr, PowerPcDisassembler dasm)
        {
            var op = ImmediateOperand.Byte((byte) ((wInstr >> 12) & 0xFF));
            dasm.ops.Add(op);
            return true;
        }

        internal static Mutator<PowerPcDisassembler> u(int pos, int len)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var op = new ImmediateOperand(Constant.Byte((byte) field.Read(u)));
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> u6_2 = u(6, 2);
        internal static readonly Mutator<PowerPcDisassembler> u6_5 = u(6, 5);
        internal static readonly Mutator<PowerPcDisassembler> u9_1 = u(9, 1);
        internal static readonly Mutator<PowerPcDisassembler> u14_2 = u(14, 2);
        internal static readonly Mutator<PowerPcDisassembler> u16_1 = u(16, 1);
        internal static readonly Mutator<PowerPcDisassembler> u16_2 = u(16, 2);
        internal static readonly Mutator<PowerPcDisassembler> u16_5 = u(16, 5);
        internal static readonly Mutator<PowerPcDisassembler> u17_8 = u(17, 8);
        internal static readonly Mutator<PowerPcDisassembler> u18_3 = u(18, 3);
        internal static readonly Mutator<PowerPcDisassembler> u21_1 = u(21, 1);
        internal static readonly Mutator<PowerPcDisassembler> u21_4 = u(21, 4);
        internal static readonly Mutator<PowerPcDisassembler> u22_3 = u(22, 3);

        internal static Mutator<PowerPcDisassembler> s(int bitOffset, int len)
        {
            var field = new Bitfield(bitOffset, len);
            return (u, d) =>
            {
                var op = new ImmediateOperand(Constant.Int32(field.ReadSigned(u)));
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> s0_12 = s(0, 12);
        internal static readonly Mutator<PowerPcDisassembler> s16_5 = s(16, 5);

        // VMX extension to access 128 vector regs
        //    //| A | 0 0 0 0 | a | 1 | VDh | VBh |

        internal static bool Wd(uint wInstr, PowerPcDisassembler dasm)
        {
            var op = dasm.VRegFromBits(((wInstr >> 21) & 0x1Fu) | ((wInstr & 0xCu) << 3));
            dasm.ops.Add(op);
            return true;
        }

        internal static bool Wa(uint wInstr, PowerPcDisassembler dasm)
        {
            var op = dasm.VRegFromBits(((wInstr >> 16) & 0x1Fu) | ((wInstr >> 4) & 0x40) | (wInstr & 0x20));
            dasm.ops.Add(op);
            return true;
        }

        internal static bool Wb(uint wInstr, PowerPcDisassembler dasm)
        {
            var op = dasm.VRegFromBits(((wInstr >> 11) & 0x1Fu) | ((wInstr & 0x3) << 5));
            dasm.ops.Add(op);
            return true;
        }

        // Special format used by the CMP[L][I] instructions.
        internal static bool X3(uint wInstr, PowerPcDisassembler dasm)
        {
            var op = dasm.CRegFromBits((wInstr >> 23) & 0x7);
            dasm.ops.Add(op);
            return true;
        }

        internal static bool Is64Bit(uint wInstr, PowerPcDisassembler dasm)
        {
            return dasm.defaultWordWidth.BitSize == 64;
        }

        #endregion

        private MachineOperand MemOff(uint reg, uint wInstr)
        {
            var d = Constant.Int32((short)wInstr);
            return new MemoryOperand(PrimitiveType.Word32, arch.Registers[(int)reg & 0x1F], d);
        }

        private RegisterOperand CRegFromBits(uint r)
        {
            return new RegisterOperand(arch.CrRegisters[(int)r & 0x7]);
        }

        private RegisterOperand RegFromBits(uint r)
        {
            return new RegisterOperand(arch.Registers[(int)r & 0x1F]);
        }

        private RegisterOperand FRegFromBits(uint r)
        {
            return new RegisterOperand(arch.FpRegisters[(int)r & 0x1F]);
        }

        private RegisterOperand VRegFromBits(uint r)
        {
            return new RegisterOperand(arch.VecRegisters[(int)r]);
        }

        private PowerPcInstruction EmitUnknown(uint instr)
        {
#if DEBUG
            //            Debug.WriteLine(
            //$@"        [Test]
            //        public void PPCDis_{instr:X8}()
            //        {{
            //            AssertCode(0x{instr:X8}, ""@@@"");
            //        }}
            //");
#endif
            return CreateInvalidInstruction();
        }
    }
}
