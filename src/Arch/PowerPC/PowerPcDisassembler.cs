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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

#pragma warning disable IDE1006 // Naming Styles

namespace Reko.Arch.PowerPC
{
    using Decoder = Decoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>;

    public partial class PowerPcDisassembler : DisassemblerBase<PowerPcInstruction, Mnemonic>
    {
        private readonly PowerPcArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly PrimitiveType defaultWordWidth;
        private readonly Decoder[] primaryDecoders;
        private Address addr;
        private bool allowSetCR0;
        private readonly List<MachineOperand> ops;

        public PowerPcDisassembler(PowerPcArchitecture arch, Decoder[] primaryDecoders, EndianImageReader rdr, PrimitiveType defaultWordWidth)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.defaultWordWidth = defaultWordWidth;
            this.primaryDecoders = primaryDecoders;
            this.ops = new List<MachineOperand>();
            this.addr = null!;
        }

        public override PowerPcInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint wInstr))
                return null;
            this.allowSetCR0 = false;
            this.ops.Clear();
            var instrCur = primaryDecoders[wInstr >> 26].Decode(wInstr, this);
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
                Operands = Array.Empty<MachineOperand>()
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

        /// <summary>
        /// If the instruction's LSB is '1', then set the setsCR0 bit.
        /// </summary>
        internal static bool C(uint wInstr, PowerPcDisassembler dasm)
        {
            dasm.allowSetCR0 = (wInstr & 1) != 0;
            return true;
        }

        /// <summary>
        /// If the instruction's bit 10 is '1', then set the setsCR0 bit.
        /// </summary>
        internal static bool C6(uint wInstr, PowerPcDisassembler dasm)
        {
            dasm.allowSetCR0 = (wInstr & (1 << 6)) != 0;
            return true;
        }

        internal static bool C10(uint wInstr, PowerPcDisassembler dasm)
        {
            dasm.allowSetCR0 = (wInstr & (1 << 10)) != 0;
            return true;
        }


        /// <summary>
        /// Force the setsCR0 flag to '1'.
        /// </summary>
        internal static bool CC(uint _, PowerPcDisassembler dasm)
        {
            dasm.allowSetCR0 = true;
            return true;
        }

        // Signed integer in bottom 16 bits of instruction.
        internal static bool S(uint wInstr, PowerPcDisassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Int16((short) wInstr));
            return true;
        }

        // Unsigned integer in bottom 16 bits of instruction.
        internal static bool U(uint wInstr, PowerPcDisassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Word16((ushort) wInstr));
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

        /// <summary>
        /// Displacement, with bottom 2 bits masked off
        /// </summary>
        internal static bool DS_2(uint wInstr, PowerPcDisassembler dasm)
        {
            var op = dasm.MemOff(wInstr >> 16, wInstr & ~3u);
            dasm.ops.Add(op);
            return true;
        }

        /// <summary>
        /// Displacement, with bottom 4 bits masked off
        /// </summary>
        internal static bool DQ_4(uint wInstr, PowerPcDisassembler dasm)
        {
            var op = dasm.MemOff(wInstr >> 16, wInstr & ~0xFu);
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

        /// <summary>
        /// Floating point register encoded by the 5 bits at offset <paramref name="offset"/>.
        /// </summary>
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
                if ((op.Number & 1) == 1)
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

        /// <summary>
        /// Register pair; if the register number is odd the instruction is invalid.
        /// </summary>
        internal static Mutator<PowerPcDisassembler> rp(int offset)
        {
            return (u, d) =>
            {
                var ireg = u >> offset;
                if ((ireg & 1) == 1)
                    return false;
                var op = d.RegFromBits(ireg);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> rp1 = r(21);
        internal static readonly Mutator<PowerPcDisassembler> rp2 = r(16);
        internal static readonly Mutator<PowerPcDisassembler> rp3 = r(11);
        internal static readonly Mutator<PowerPcDisassembler> rp4 = r(6);


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


        /// <summary>
        /// 6-bit reference to vsr register.
        /// </summary>
        internal static Mutator<PowerPcDisassembler> vsr(int bitpos_hi, int bitpos_lo)
        {
            var bitfields = new Bitfield[] {
                new Bitfield(bitpos_hi, 5),
                new Bitfield(bitpos_lo, 1)
            };
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(bitfields, u);
                var op = d.VRegFromBits(iReg);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> vsr1 = vsr(21, 0);
        internal static readonly Mutator<PowerPcDisassembler> vsr2 = vsr(16, 2);
        internal static readonly Mutator<PowerPcDisassembler> vsr3 = vsr(11, 1);
        internal static readonly Mutator<PowerPcDisassembler> vsr4 = vsr(6, 3);

        internal static Mutator<PowerPcDisassembler> Xt(int bitpos_hi, int bitpos_lo)
        {
            var bitfields = BeFields((bitpos_hi, 1), (bitpos_lo, 5));
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(bitfields, u);
                var op = d.VRegFromBits(iReg);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> xt29_11 = Xt(29, 11);
        internal static readonly Mutator<PowerPcDisassembler> xt30_16 = Xt(30, 16);
        internal static readonly Mutator<PowerPcDisassembler> xt31_6 = Xt(31, 6);

        ///<summary>
        /// vector register using only 3 bits of encoding
        /// </summary>
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


        /// <summary>
        /// 5-bit immediate field.
        /// </summary>
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
        internal static readonly Mutator<PowerPcDisassembler> I4 = I(6);
        internal static readonly Mutator<PowerPcDisassembler> I5 = I(1);

        internal static Mutator<PowerPcDisassembler> i(params Bitfield[] bitfields)
        {
            return (u, d) =>
            {
                var i = (int) Bitfield.ReadFields(bitfields, u);
                var op = ImmediateOperand.Int32(i);
                d.ops.Add(op);
                return true;
            };
        }

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
                var op = ImmediateOperand.Byte((byte) field.Read(u));
                d.ops.Add(op);
                return true;
            };
        }

        internal static Mutator<PowerPcDisassembler> u(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var n = Bitfield.ReadSignedFields(fields, u);
                var op = ImmediateOperand.Int32(n);
                d.ops.Add(op);
                return true;
            };
        }

        internal static readonly Mutator<PowerPcDisassembler> u6_2 = u(6, 2);
        internal static readonly Mutator<PowerPcDisassembler> u6_4 = u(6, 4);
        internal static readonly Mutator<PowerPcDisassembler> u6_5 = u(6, 5);
        internal static readonly Mutator<PowerPcDisassembler> u9_1 = u(9, 1);
        internal static readonly Mutator<PowerPcDisassembler> u9_2 = u(9, 2);
        internal static readonly Mutator<PowerPcDisassembler> u9_3 = u(9, 3);
        internal static readonly Mutator<PowerPcDisassembler> u10_6 = u(10, 6);
        internal static readonly Mutator<PowerPcDisassembler> u11_1 = u(11, 1);
        internal static readonly Mutator<PowerPcDisassembler> u11_5 = u(11, 5);
        internal static readonly Mutator<PowerPcDisassembler> u11_10 = u(11, 10);
        internal static readonly Mutator<PowerPcDisassembler> u12_4 = u(12, 4);
        internal static readonly Mutator<PowerPcDisassembler> u14_2 = u(14, 2);
        internal static readonly Mutator<PowerPcDisassembler> u16_1 = u(16, 1);
        internal static readonly Mutator<PowerPcDisassembler> u16_2 = u(16, 2);
        internal static readonly Mutator<PowerPcDisassembler> u16_3 = u(16, 3);
        internal static readonly Mutator<PowerPcDisassembler> u16_4 = u(16, 4);
        internal static readonly Mutator<PowerPcDisassembler> u16_5 = u(16, 5);
        internal static readonly Mutator<PowerPcDisassembler> u16_6 = u(16, 6);
        internal static readonly Mutator<PowerPcDisassembler> u16_7 = u(16, 7);
        internal static readonly Mutator<PowerPcDisassembler> u17_1 = u(17, 1);
        internal static readonly Mutator<PowerPcDisassembler> u17_8 = u(17, 8);
        internal static readonly Mutator<PowerPcDisassembler> u18_2 = u(18, 2);
        internal static readonly Mutator<PowerPcDisassembler> u18_3 = u(18, 3);
        internal static readonly Mutator<PowerPcDisassembler> u20_1 = u(20, 1);
        internal static readonly Mutator<PowerPcDisassembler> u21_1 = u(21, 1);
        internal static readonly Mutator<PowerPcDisassembler> u21_4 = u(21, 4);
        internal static readonly Mutator<PowerPcDisassembler> u21_5 = u(21, 5);
        internal static readonly Mutator<PowerPcDisassembler> u22_3 = u(22, 3);
        internal static readonly Mutator<PowerPcDisassembler> u22_5 = u(22, 5);
        internal static readonly Mutator<PowerPcDisassembler> u23_3 = u(23, 3);

        internal static Mutator<PowerPcDisassembler> s(int bitOffset, int len)
        {
            var field = new Bitfield(bitOffset, len);
            return (u, d) =>
            {
                var op = ImmediateOperand.Int32(field.ReadSigned(u));
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<PowerPcDisassembler> s0_12 = s(0, 12);
        internal static readonly Mutator<PowerPcDisassembler> s11_5 = s(11, 5);
        internal static readonly Mutator<PowerPcDisassembler> s16_5 = s(16, 5);

        internal static Mutator<PowerPcDisassembler> s(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var n = Bitfield.ReadSignedFields(fields, u);
                var op = ImmediateOperand.Int32(n);
                d.ops.Add(op);
                return true;
            };
        }

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

        /// <summary>
        /// Wc encoding only allows for v0-v7
        /// </summary>
        internal static bool Wc(uint wInstr, PowerPcDisassembler dasm)
        {
            var op = dasm.VRegFromBits((wInstr >> 6) & 7);
            dasm.ops.Add(op);
            return true;
        }

        /// <summary>
        /// Special purpose register field has its two halves swapped.
        /// </summary>
        internal static bool SPR(uint wInstr, PowerPcDisassembler dasm)
        {
            var nSpr = ((wInstr >> 16) & 0x1F) | ((wInstr >> 6) & 0x3E0);
            if (dasm.arch.SpRegisters.TryGetValue((int) nSpr, out var spr))
            {
                dasm.ops.Add(spr);
            }
            else
            {
                dasm.ops.Add(ImmediateOperand.UInt32(nSpr));
            }
            return true;
        }

        // Special format used by the CMP[L][I] instructions.
        internal static bool X3(uint wInstr, PowerPcDisassembler dasm)
        {
            var op = dasm.CRegFromBits((wInstr >> 23) & 0x7);
            dasm.ops.Add(op);
            return true;
        }

        internal static bool Is64Bit(uint _, PowerPcDisassembler dasm)
        {
            return dasm.defaultWordWidth.BitSize == 64;
        }

        internal static Mutator<PowerPcDisassembler> FnCode(int ppcBitPos, int bitLength, params FunctionCode[] codes)
        {
            var bitfield = BeField(ppcBitPos, bitLength);
            return (u, d) =>
            {
                var c = bitfield.Read(u);
                var fc = codes[c];
                if (fc == FunctionCode.Invalid)
                    return false;
                d.ops.Add(new FunctionCodeOperand(fc));
                return true;

            };
        }
        internal static Mutator<PowerPcDisassembler> fc_atomic_load = FnCode(16, 5,
            FunctionCode.add,
            FunctionCode.xor,
            FunctionCode.or,
            FunctionCode.and,

            FunctionCode.umax,
            FunctionCode.smax,
            FunctionCode.umin,
            FunctionCode.smin,

            FunctionCode.swap,
            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,

            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,

            FunctionCode.casne,
            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,

            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,

            FunctionCode.incb,
            FunctionCode.ince,
            FunctionCode.Invalid,
            FunctionCode.Invalid,

            FunctionCode.decb,
            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid);

        internal static Mutator<PowerPcDisassembler> fc_atomic_store = FnCode(16, 5,
            FunctionCode.add,
            FunctionCode.xor,
            FunctionCode.or,
            FunctionCode.and,

            FunctionCode.umax,
            FunctionCode.smax,
            FunctionCode.umin,
            FunctionCode.smin,

            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,

            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,


            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,

            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,

            FunctionCode.twin,
            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,

            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid,
            FunctionCode.Invalid
            );

        #endregion

        /// <summary>
        /// Create a Reko <see cref="Bitfield"/> using big-endian bit position and bit length.
        /// </summary>
        /// <remarks>
        /// PowerPC instruction bits are numbered from the MSB to LSB, but 
        /// Reko bitfields are numbered from LSB to MSB.
        /// </summary>
        internal static Bitfield BeField(int bitPos, int bitLength)
        {
            return new Bitfield(32 - (bitPos + bitLength), bitLength);
        }

        internal static Bitfield[] BeFields(params (int bitPos, int bitLength)[] fieldDescs)
        {
            var bitfields = new Bitfield[fieldDescs.Length];
            for (int i = 0; i < fieldDescs.Length; ++i)
            {
                var (bitPos, bitLength) = fieldDescs[i];
                bitfields[i] = BeField(bitPos, bitLength);
            }
            return bitfields;
        }

        private MachineOperand MemOff(uint reg, uint wInstr)
        {
            return new MemoryOperand(PrimitiveType.Word32, arch.Registers[(int)reg & 0x1F],
                ImmediateOperand.Int32((short) wInstr));
        }

        private RegisterStorage CRegFromBits(uint r)
        {
            return arch.CrRegisters[(int)r & 0x7];
        }

        private RegisterStorage RegFromBits(uint r)
        {
            return arch.Registers[(int)r & 0x1F];
        }

        private RegisterStorage FRegFromBits(uint r)
        {
            return arch.FpRegisters[(int)r & 0x1F];
        }

        private RegisterStorage VRegFromBits(uint r)
        {
            return arch.VecRegisters[(int)r];
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

        public override PowerPcInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("PPCDis", this.addr, this.rdr, message);
            return new PowerPcInstruction(Mnemonic.nyi)
            {
                InstructionClass = InstrClass.Invalid,
                Operands = Array.Empty<MachineOperand>()
            };
        }
    }
}
