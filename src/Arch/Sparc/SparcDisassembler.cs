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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    using Decoder = Decoder<SparcDisassembler, Mnemonic, SparcInstruction>;
#pragma warning disable IDE1006

    public class SparcDisassembler : DisassemblerBase<SparcInstruction, Mnemonic>
    {
        private readonly SparcArchitecture arch;
        private readonly Decoder rootDecoder;
        private readonly EndianImageReader imageReader;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private SparcInstruction instrCur;
        private Prediction pred;

        public SparcDisassembler(SparcArchitecture arch, Decoder rootDecoder, EndianImageReader imageReader)
        {
            this.arch = arch;
            this.rootDecoder = rootDecoder;
            this.imageReader = imageReader;
            this.ops = new List<MachineOperand>();
        }

        public override SparcInstruction DisassembleInstruction()
        {
            this.addr = imageReader.Address;
            if (!imageReader.TryReadBeUInt32(out uint wInstr))
                return null;
            ops.Clear();
            pred = Prediction.None;
            instrCur = rootDecoder.Decode(wInstr, this);
            instrCur.Address = addr;
            instrCur.Length = 4;
            instrCur.InstructionClass |= wInstr == 0 ? InstrClass.Zero : 0;
            return instrCur;
        }

        public override SparcInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new SparcInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Prediction = pred,
                Operands = this.ops.ToArray()
            };
        }

        public override SparcInstruction CreateInvalidInstruction()
        {
            return new SparcInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.illegal,
                Operands = MachineInstruction.NoOperands,
            };
        }

        public override SparcInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("SparcDasm", this.addr, this.imageReader, message);
            return CreateInvalidInstruction();
        }

        #region Mutators

        // Register reference
        private static Mutator<SparcDisassembler> r(int pos)
        {
            return (wInstr, dasm) =>
            {
                var reg = dasm.arch.Registers.GetRegister((wInstr >> pos) & 0x1F);
                dasm.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> r0 = r(0);
        internal static readonly Mutator<SparcDisassembler> r14 = r(14);
        internal static readonly Mutator<SparcDisassembler> r25 = r(25);

        private static Mutator<SparcDisassembler> r(Func<SparcArchitecture, RegisterStorage> getreg)
        {
            return (u, d) =>
            {
                d.ops.Add(new RegisterOperand(getreg(d.arch)));
                return true;
            };
        }

        internal static readonly Mutator<SparcDisassembler> rfsr = r(a => a.Registers.fsr);
        internal static readonly Mutator<SparcDisassembler> ry = r(a => a.Registers.y);

        // FPU register
        private static Mutator<SparcDisassembler> f(int pos)
        {
            return (u, d) =>
            {
                var freg = d.arch.Registers.FFloatRegisters[(u >> pos) & 0x1F];
                d.ops.Add(new RegisterOperand(freg));
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> f0 = f(0);
        internal static readonly Mutator<SparcDisassembler> f14 = f(14);
        internal static readonly Mutator<SparcDisassembler> f24 = f(24);
        internal static readonly Mutator<SparcDisassembler> f25 = f(25);

        // double FPU register encoding
        private static Mutator<SparcDisassembler> d(int pos)
        {
            return (u, d) =>
            {
                var dreg = GetDoubleRegisterOperand(d.arch.Registers, u, pos);
                if (dreg == null)
                    return false;
                d.ops.Add(dreg);
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> d0 = d(0);
        internal static readonly Mutator<SparcDisassembler> d14 = d(14);
        internal static readonly Mutator<SparcDisassembler> d25 = d(25);

        // quad FPU register encoding
        private static Mutator<SparcDisassembler> q(int pos)
        {
            return (u, d) =>
            {
                var qreg = GetQuadRegisterOperand(d.arch.Registers, u, pos);
                if (qreg == null)
                    return false;
                d.ops.Add(qreg);
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> q0 = q(0);
        internal static readonly Mutator<SparcDisassembler> q14 = q(14);
        internal static readonly Mutator<SparcDisassembler> q25 = q(25);

        private static Mutator<SparcDisassembler> A(PrimitiveType size)
        {
            return (u, d) =>
            {
                d.ops.Add(GetAlternateSpaceOperand(d.arch.Registers, u, size));
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> Ab = A(PrimitiveType.Byte);
        internal static readonly Mutator<SparcDisassembler> Ah = A(PrimitiveType.Word16);
        internal static readonly Mutator<SparcDisassembler> Aw = A(PrimitiveType.Word32);
        internal static readonly Mutator<SparcDisassembler> Ad = A(PrimitiveType.Word64);
        internal static readonly Mutator<SparcDisassembler> Asb = A(PrimitiveType.SByte);
        internal static readonly Mutator<SparcDisassembler> Ash = A(PrimitiveType.Int16);
        internal static readonly Mutator<SparcDisassembler> Asw = A(PrimitiveType.Int32);


        // 22-bit immediate value
        internal static bool I(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(GetImmOperand(wInstr, 22));
            return true;
        }

        internal static bool J22(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(GetAddressOperand(dasm.imageReader.Address, wInstr, 22));
            return true;
        }

        internal static bool J19(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(GetAddressOperand(dasm.imageReader.Address, wInstr, 19));
            return true;
        }

        private static readonly Bitfield[] bf20_2_0_16 = Bf((20, 2), (0, 16));
        internal static bool J2_16(uint wInstr, SparcDisassembler dasm)
        {
            int offset = Bitfield.ReadSignedFields(bf20_2_0_16, wInstr) << 2;
            dasm.ops.Add(AddressOperand.Create(dasm.imageReader.Address + (offset - 4)));
            return true;
        }

        internal static bool JJ(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(AddressOperand.Create((dasm.imageReader.Address - 4) + ((int) wInstr << 2)));
            return true;
        }

        private static Mutator<SparcDisassembler> M(PrimitiveType size)
        {
            return (u, d) =>
            {
                d.ops.Add(GetMemoryOperand(d.arch.Registers, u, size));
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> Mb = M(PrimitiveType.Byte);
        internal static readonly Mutator<SparcDisassembler> Mh = M(PrimitiveType.Word16);
        internal static readonly Mutator<SparcDisassembler> Mw = M(PrimitiveType.Word32);
        internal static readonly Mutator<SparcDisassembler> Md = M(PrimitiveType.Word64);
        internal static readonly Mutator<SparcDisassembler> Mq = M(PrimitiveType.Word128);
        internal static readonly Mutator<SparcDisassembler> Msb = M(PrimitiveType.SByte);
        internal static readonly Mutator<SparcDisassembler> Msh = M(PrimitiveType.Int16);
        internal static readonly Mutator<SparcDisassembler> Msw = M(PrimitiveType.Int32);

        // Register or simm13.
        private static Mutator<SparcDisassembler> R(bool signed)
        {
            return (u, d) =>
            {
                // if 's', return a signed immediate operand where relevant.
                d.ops.Add(d.GetRegImmOperand(d.arch.Registers, u, signed, 13));
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> R0 = R(false);
        internal static readonly Mutator<SparcDisassembler> Rs = R(true);

        // Register or simm10
        private static Mutator<SparcDisassembler> RorSimm10()
        {
            return (u, d) =>
            {
                // if 's', return a signed immediate operand where relevant.
                d.ops.Add(d.GetRegImmOperand(d.arch.Registers, u, true, 10));
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> Rs10 = RorSimm10();

        // Register or uimm5/6
        internal static bool S(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(dasm.GetRegUImmOperand(dasm.arch.Registers, wInstr, 6));
            return true;
        }

        // trap number
        internal static bool T(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(dasm.GetRegImmOperand(dasm.arch.Registers, wInstr, false, 7));
            return true;
        }

        internal static bool nyi(uint wInstr, SparcDisassembler dasm)
        {
            dasm.NotYetImplemented("NYI");
            return false;
        }

        #endregion

        private static int SignExtend(uint word, int bits)
        {
            int imm = (int) word & ((1 << bits) - 1);
            int mask = (0 - (imm & (1 << (bits - 1)))) << 1;
            return imm | mask;
        }

        private static AddressOperand GetAddressOperand(Address addr, uint wInstr, int bitLength)
        {
            int offset = SignExtend(wInstr, bitLength) << 2;
            return AddressOperand.Create(addr + (offset - 4));
        }

        private static MachineOperand GetAlternateSpaceOperand(Registers registers, uint wInstr, PrimitiveType type)
        {
            RegisterStorage b = registers.GetRegister(wInstr >> 14);
            RegisterStorage idx = registers.GetRegister(wInstr);
            var asi = (wInstr >> 4) & 0xFF;
            return new MemoryOperand(b, Constant.Int32((int) asi), type);
        }

        private static MachineOperand GetMemoryOperand(Registers registers, uint wInstr, PrimitiveType type)
        {
            RegisterStorage b = registers.GetRegister(wInstr >> 14);
            if ((wInstr & (1 << 13)) != 0)
            {
                return new MemoryOperand(b, Constant.Int32(SignExtend(wInstr, 13)), type);
            }
            else
            {
                RegisterStorage idx = registers.GetRegister(wInstr);
                return new IndexedMemoryOperand(b, idx, type);
            }
        }

        private static RegisterOperand GetDoubleRegisterOperand(Registers registers, uint wInstr, int offset)
        {
            int encodedReg = (int) (wInstr >> offset) & 0x1F;
            int reg = ((encodedReg & 1) << 5) | (encodedReg & ~1);
            return new RegisterOperand(registers.DFloatRegisters[reg >> 1]);
        }

        private static RegisterOperand GetQuadRegisterOperand(Registers registers, uint wInstr, int offset)
        {
            int encodedReg = (int) (wInstr >> offset) & 0x1F;
            int reg = ((encodedReg & 1) << 5) | (encodedReg & ~1);
            if ((reg & 0x3) != 0)
                return null;
            return new RegisterOperand(registers.QFloatRegisters[reg>>2]);
        }

        private MachineOperand GetRegImmOperand(Registers registers, uint wInstr, bool signed, int bits)
        {
            if ((wInstr & (1 << 13)) != 0)
            {
                // Sign-extend the bastard.
                int imm = (int) wInstr & ((1 << bits) - 1);
                int mask = (0 - (imm & (1 << (bits - 1)))) << 1;
                imm |= mask;
                Constant c = signed
                    ? Constant.Create(arch.SignedWord, (long) imm)
                    : Constant.Create(arch.WordWidth, (long) imm);
                return new ImmediateOperand(c);
            }
            else
            {
                return new RegisterOperand(registers.GetRegister(wInstr & 0x1Fu));
            }
        }
        
        private MachineOperand GetRegUImmOperand(Registers registers, uint wInstr, int bits)
        {
            if ((wInstr & (1 << 13)) != 0)
            {
                // Sign-extend the bastard.
                uint imm = wInstr & ((1u << bits) - 1);
                return new ImmediateOperand(Constant.Create(this.arch.WordWidth, imm));
            }
            else
            {
                return new RegisterOperand(registers.GetRegister(wInstr & 0x1Fu));
            }
        }

        private static ImmediateOperand GetImmOperand(uint wInstr, int bits)
        {
            uint imm = wInstr & ((1u << bits) - 1);
            return new ImmediateOperand(Constant.Word32(imm));
        }

        internal static bool Pred(uint wInstr, SparcDisassembler dasm)
        {
            dasm.pred = Bits.IsBitSet(wInstr, 19)
                ? Prediction.Taken
                : Prediction.NotTaken;
            return true;
        }
    }
}