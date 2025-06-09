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
            this.instrCur = null!;
        }

        public override SparcInstruction? DisassembleInstruction()
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
                Operands = Array.Empty<MachineOperand>(),
            };
        }

        public override SparcInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("SparcDasm", this.addr, this.imageReader, message);
            return CreateInvalidInstruction();
        }

        #region Mutators

        /// <summary>
        /// Register reference
        /// </summary>
        private static Mutator<SparcDisassembler> r(int bitpos)
        {
            return (wInstr, dasm) =>
            {
                var reg = dasm.arch.Registers.GetRegister((wInstr >> bitpos) & 0x1F);
                dasm.ops.Add(reg);
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
                d.ops.Add(getreg(d.arch));
                return true;
            };
        }

        internal static readonly Mutator<SparcDisassembler> rfsr = r(a => a.Registers.fsr);
        internal static readonly Mutator<SparcDisassembler> ry = r(a => a.Registers.y);

        /// <summary>
        /// Register in a register pair
        /// </summary>
        private static Mutator<SparcDisassembler> rp(int bitpos)
        {
            return (wInstr, dasm) =>
            {
                var ireg = (wInstr >> bitpos) & 0x1F;
                if ((ireg & 1) != 0)
                    return false;   // odd numbered registers are not allowed.
                var reg = dasm.arch.Registers.GetRegister(ireg);
                dasm.ops.Add(reg);
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> rp25 = rp(25);

        /// <summary>
        /// FPU register
        /// </summary>
        private static Mutator<SparcDisassembler> f(int bitpos)
        {
            return (u, d) =>
            {
                var freg = d.arch.Registers.FFloatRegisters[(u >> bitpos) & 0x1F];
                d.ops.Add(freg);
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> f0 = f(0);
        internal static readonly Mutator<SparcDisassembler> f14 = f(14);
        internal static readonly Mutator<SparcDisassembler> f24 = f(24);
        internal static readonly Mutator<SparcDisassembler> f25 = f(25);

        /// <summary>
        /// Double FPU register encoding
        /// </summary>
        private static Mutator<SparcDisassembler> d(int bitpos)
        {
            return (u, d) =>
            {
                var dreg = GetDoubleRegisterOperand(d.arch.Registers, u, bitpos);
                if (dreg is null)
                    return false;
                d.ops.Add(dreg);
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> d0 = d(0);
        internal static readonly Mutator<SparcDisassembler> d14 = d(14);
        internal static readonly Mutator<SparcDisassembler> d25 = d(25);

        /// <summary>
        /// Quad FPU register encoding
        /// </summary>
        private static Mutator<SparcDisassembler> q(int bitpos)
        {
            return (u, d) =>
            {
                var qreg = GetQuadRegisterOperand(d.arch.Registers, u, bitpos);
                if (qreg is null)
                    return false;
                d.ops.Add(qreg);
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> q0 = q(0);
        internal static readonly Mutator<SparcDisassembler> q14 = q(14);
        internal static readonly Mutator<SparcDisassembler> q25 = q(25);

        /// <summary>
        /// Alternate space
        /// </summary>
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


        /// <summary>
        /// 22-bit immediate value
        /// </summary>
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
            dasm.ops.Add(dasm.imageReader.Address + (offset - 4));
            return true;
        }

        internal static bool JJ(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add((dasm.imageReader.Address - 4) + ((int) wInstr << 2));
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

        /// <summary>
        /// Register or signed immediate.
        /// </summary>
        private static Mutator<SparcDisassembler> R(bool signed, int bitsize)
        {
            return (u, d) =>
            {
                // if 's', return a signed immediate operand where relevant.
                d.ops.Add(d.GetRegImmOperand(d.arch.Registers, u, signed, bitsize));
                return true;
            };
        }
        internal static readonly Mutator<SparcDisassembler> Ru13 = R(false, 13);
        internal static readonly Mutator<SparcDisassembler> Rs13 = R(true, 13);
        internal static readonly Mutator<SparcDisassembler> Rs11 = R(true, 11);
        internal static readonly Mutator<SparcDisassembler> Rs10 = R(true, 10);


        /// <summary>
        /// Register or uimm5/6
        /// </summary>
        internal static bool S(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(dasm.GetRegUImmOperand(dasm.arch.Registers, wInstr, 6));
            return true;
        }

        /// <summary>
        /// Trap number
        /// </summary>
        internal static bool T(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(dasm.GetRegImmOperand(dasm.arch.Registers, wInstr, false, 7));
            return true;
        }

        /// <summary>
        /// FCC encoding
        /// </summary>
        internal static bool fcc(uint wInstr, SparcDisassembler dasm)
        {
            var fccCode = ccField.Read(wInstr);
            dasm.ops.Add(new ConditionCodeOperand(ffields[fccCode]));
            return true;
        }

        /// <summary>
        /// ICC/XCC encoding
        /// </summary>
        internal static bool icc(uint wInstr, SparcDisassembler dasm)
        {
            var iccCode = ifields[ccField.Read(wInstr)];
            if (iccCode == ConditionField.None)
                return false;
            dasm.ops.Add(new ConditionCodeOperand(iccCode));
            return true;
        }

        private static readonly Bitfield ccField = new Bitfield(11, 2);

        private static readonly ConditionField[] ffields = new[]
        {
            ConditionField.fcc0,ConditionField.fcc1,ConditionField.fcc2,ConditionField.fcc3,
        };

        private static readonly ConditionField[] ifields = new[]
        {
            ConditionField.icc,ConditionField.None,ConditionField.xcc,ConditionField.None,
        };

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

        private static Address GetAddressOperand(Address addr, uint wInstr, int bitLength)
        {
            int offset = SignExtend(wInstr, bitLength) << 2;
            return addr + (offset - 4);
        }

        private static MachineOperand GetAlternateSpaceOperand(Registers registers, uint wInstr, PrimitiveType type)
        {
            RegisterStorage b = registers.GetRegister(wInstr >> 14);
            RegisterStorage idx = registers.GetRegister(wInstr);
            var asi = (wInstr >> 4) & 0xFF;
            return MemoryOperand.Indirect(b, Constant.Int32((int) asi), type);
        }

        private static MachineOperand GetMemoryOperand(Registers registers, uint wInstr, PrimitiveType type)
        {
            RegisterStorage b = registers.GetRegister(wInstr >> 14);
            if ((wInstr & (1 << 13)) != 0)
            {
                return MemoryOperand.Indirect(b, Constant.Int32(SignExtend(wInstr, 13)), type);
            }
            else
            {
                RegisterStorage idx = registers.GetRegister(wInstr);
                return MemoryOperand.Indexed(b, idx, type);
            }
        }

        private static RegisterStorage GetDoubleRegisterOperand(Registers registers, uint wInstr, int offset)
        {
            int encodedReg = (int) (wInstr >> offset) & 0x1F;
            int reg = ((encodedReg & 1) << 5) | (encodedReg & ~1);
            return registers.DFloatRegisters[reg >> 1];
        }

        private static RegisterStorage? GetQuadRegisterOperand(Registers registers, uint wInstr, int offset)
        {
            int encodedReg = (int) (wInstr >> offset) & 0x1F;
            int reg = ((encodedReg & 1) << 5) | (encodedReg & ~1);
            if ((reg & 0x3) != 0)
                return null;
            return registers.QFloatRegisters[reg>>2];
        }

        private MachineOperand GetRegImmOperand(Registers registers, uint wInstr, bool signed, int bits)
        {
            if ((wInstr & (1 << 13)) != 0)
            {
                int imm = (int) Bits.SignExtend(wInstr, bits);
                var dt = signed ? arch.SignedWord : arch.WordWidth;
                Constant c = Constant.Create(dt, (long) imm);
                return c;
            }
            else
            {
                return registers.GetRegister(wInstr & 0x1Fu);
            }
        }
        
        private MachineOperand GetRegUImmOperand(Registers registers, uint wInstr, int bits)
        {
            if ((wInstr & (1 << 13)) != 0)
            {
                ulong imm = Bits.ZeroExtend(wInstr, bits);
                return Constant.Create(this.arch.WordWidth, imm);
            }
            else
            {
                return registers.GetRegister(wInstr & 0x1Fu);
            }
        }

        private static Constant GetImmOperand(uint wInstr, int bits)
        {
            uint imm = wInstr & ((1u << bits) - 1);
            return Constant.Word32(imm);
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