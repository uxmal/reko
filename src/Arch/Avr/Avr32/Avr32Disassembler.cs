#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Types;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Reko.Arch.Avr.Avr32
{
#pragma warning disable IDE1006

    using Decoder = Decoder<Avr32Disassembler, Mnemonic, Avr32Instruction>;

    public class Avr32Disassembler : DisassemblerBase<Avr32Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly Avr32Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private InstrClass iclass;
        private Avr32Condition condition;

        public Avr32Disassembler(Avr32Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = null!;
        }

        public override Avr32Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out ushort uInstr))
                return null;
            this.iclass = InstrClass.None;
            this.condition = Avr32Condition.al;
            ops.Clear();
            var instr = rootDecoder.Decode(uInstr, this);
            if (uInstr == 0)
                instr.InstructionClass |= InstrClass.Zero;
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override Avr32Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Avr32Instruction
            {
                InstructionClass = (this.iclass != InstrClass.None)
                    ? this.iclass
                    : iclass,
                Mnemonic = mnemonic,
                Condition = condition,
                Operands = ops.ToArray()
            };
        }

        public override Avr32Instruction CreateInvalidInstruction()
        {
            return new Avr32Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Condition = Avr32Condition.al,
                Operands = Array.Empty<MachineOperand>(),
            };
        }

        public override Avr32Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Avr32Dis", this.addr, rdr, message);
            return CreateInvalidInstruction();
        }

        #region Mutators
        private static Mutator<Avr32Disassembler> Register(int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                var reg = Registers.GpRegisters[iReg];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> R0 = Register(0);
        private static readonly Mutator<Avr32Disassembler> R9 = Register(9);
        private static readonly Mutator<Avr32Disassembler> R16 = Register(16);
        private static readonly Mutator<Avr32Disassembler> R25 = Register(25);

        private static Mutator<Avr32Disassembler> RegisterWithShift(int bitpos, int shiftAm, int shiftSize, Mnemonic shiftType)
        {
            var bfRegister = new Bitfield(bitpos, 4);
            var bfShift = new Bitfield(shiftAm, shiftSize);
            return (u, d) =>
            {
                var iReg = bfRegister.Read(u);
                var shiftAmt = (int)bfShift.Read(u);
                d.ops.Add(new RegisterImmediateOperand(Registers.GpRegisters[iReg], shiftType, shiftAmt));
                return true;
            };
        }
        
        private static readonly Mutator<Avr32Disassembler> Rlsl16_4_2 = RegisterWithShift(16, 4, 2, Mnemonic.lsl);
        private static readonly Mutator<Avr32Disassembler> Rlsl16_4_5 = RegisterWithShift(16, 4, 5, Mnemonic.lsl);
        private static readonly Mutator<Avr32Disassembler> Rlsr16_4_5 = RegisterWithShift(16, 4, 5, Mnemonic.lsr);
        private static readonly Mutator<Avr32Disassembler> Rlsr16_0_5 = RegisterWithShift(16, 0, 5, Mnemonic.lsr);

        private static Mutator<Avr32Disassembler> RegisterWrite(int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                if (iReg == 0xF)
                    d.iclass = InstrClass.Transfer;
                var reg = Registers.GpRegisters[iReg];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> Rw0 = RegisterWrite(0);
        private static readonly Mutator<Avr32Disassembler> Rw16 = RegisterWrite(16);
        private static readonly Mutator<Avr32Disassembler> Rw25 = RegisterWrite(25);

        private static Mutator<Avr32Disassembler> DoubleRegister(int bitPos)
        {
            var bitfield = new Bitfield(bitPos, 3);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u) << 1;
                var gp = Registers.GpRegisters;
                d.ops.Add(new RegisterPairOperand(gp[iReg + 1], gp[iReg]));
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> Rd1 = DoubleRegister(1);
        private static readonly Mutator<Avr32Disassembler> Rd17 = DoubleRegister(17);

        private static Mutator<Avr32Disassembler> RegisterHalf(int bitposReg, int bitPosRegPart, bool write)
        {
            var bitfield = new Bitfield(bitposReg, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                if (write && iReg == 0xF)
                    d.iclass = InstrClass.Transfer;
                var isHighPart = Bits.IsBitSet(u, bitPosRegPart)
                    ? RegisterPart.Top 
                    : RegisterPart.Bottom;
                d.ops.Add(new RegisterPartOperand(Registers.GpRegisters[iReg], isHighPart));
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> Rh16_4 = RegisterHalf(16, 4, false);
        private static readonly Mutator<Avr32Disassembler> Rh16_12 = RegisterHalf(16, 12, false);
        private static readonly Mutator<Avr32Disassembler> Rhw16_12 = RegisterHalf(16, 12, true);
        private static readonly Mutator<Avr32Disassembler> Rh25_4 = RegisterHalf(25, 4, false);
        private static readonly Mutator<Avr32Disassembler> Rh25_5 = RegisterHalf(25, 5, false);
        private static readonly Mutator<Avr32Disassembler> Rh25_13 = RegisterHalf(25, 13, false);


        private static Mutator<Avr32Disassembler> RegisterBytePart(int bitposReg, int bitPosRegPart, bool write)
        {
            var rDstField = new Bitfield(bitposReg, 4);
            var regParField = new Bitfield(bitPosRegPart, 2);
            return (u, d) =>
            {
                var iReg = rDstField.Read(u);
                if (write && iReg == 0xF)
                    d.iclass = InstrClass.Transfer;
                var regPart = regParts[regParField.Read(u)];
                d.ops.Add(new RegisterPartOperand(Registers.GpRegisters[iReg], regPart));
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> Rhb16_12 = RegisterBytePart(16, 12, true);


        private static Mutator<Avr32Disassembler> Rdec(int a, int b) => (u, d) => false;
        private static Mutator<Avr32Disassembler> Rinc(int a, int b) => (u, d) => false;

        private static RegisterPart[] regParts =
        {
            RegisterPart.Top,
            RegisterPart.Upper,
            RegisterPart.Lower,
            RegisterPart.Bottom,
        };

        /// <summary>
        /// Coprocessor register
        /// </summary>
        private static Mutator<Avr32Disassembler> CoprocessorRegister(int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                d.ops.Add(Registers.CoprocRegisters[iReg]);
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> CR0 = CoprocessorRegister(0);
        private static readonly Mutator<Avr32Disassembler> CR4 = CoprocessorRegister(4);
        private static readonly Mutator<Avr32Disassembler> CR8 = CoprocessorRegister(8);


        private static Mutator<Avr32Disassembler> M(PrimitiveType dt, int baseRegPos)
        {
            var baseRegField = new Bitfield(baseRegPos, 4);
            return (u, d) =>
            {
                var iReg = baseRegField.Read(u);
                var reg = Registers.GpRegisters[iReg];
                d.ops.Add(MemoryOperand.Displaced(dt, reg, 0));
                return true;
            };
        }

        private static Mutator<Avr32Disassembler> MunsignedDisplacement(PrimitiveType dt, int regPos, int bitpos, int length)
        {
            var regField = new Bitfield(regPos, 4);
            var bitfield = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var iReg = regField.Read(u);
                var reg = Registers.GpRegisters[iReg];
                var disp = bitfield.ReadSigned(u) * dt.Size;
                d.ops.Add(MemoryOperand.Displaced(dt, reg, disp));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Mdisp_w0_16 = MunsignedDisplacement(PrimitiveType.Word32, 16, 0, 16);
        private static readonly Mutator<Avr32Disassembler> Mdisp_w4_5 = MunsignedDisplacement(PrimitiveType.Word32, 9, 4, 5);
        private static readonly Mutator<Avr32Disassembler> Mdisp_w4_4 = MunsignedDisplacement(PrimitiveType.Word32, 9, 4, 4);
        private static readonly Mutator<Avr32Disassembler> Mdisp_ub4_3 = MunsignedDisplacement(PrimitiveType.Byte, 9, 4, 3);
        private static readonly Mutator<Avr32Disassembler> Mdisp_h4_3 = MunsignedDisplacement(PrimitiveType.Word16, 9, 4, 3);
        private static readonly Mutator<Avr32Disassembler> Mdisp_sh4_3 = MunsignedDisplacement(PrimitiveType.Int16, 9, 4, 3);
        private static readonly Mutator<Avr32Disassembler> Mdisp_uh4_3 =  MunsignedDisplacement(PrimitiveType.UInt16, 9, 4, 3);

        private static Mutator<Avr32Disassembler> MunsignedDisplacement(PrimitiveType dt, int regPos, params Bitfield[] displacementFields)
        {
            var regField = new Bitfield(regPos, 4);
            return (u, d) =>
            {
                var iReg = regField.Read(u);
                var reg = Registers.GpRegisters[iReg];
                var disp = Bitfield.ReadSignedFields(displacementFields, u) * dt.Size;
                d.ops.Add(MemoryOperand.Displaced(dt, reg, disp));
                return true;
            };
        }


        private static Mutator<Avr32Disassembler> MsignedDisplacement(PrimitiveType dt, int regPos, int bitpos, int length)
        {
            var regField = new Bitfield(regPos, 4);
            var bitfield = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var iReg = regField.Read(u);
                var reg = Registers.GpRegisters[iReg];
                var disp = bitfield.ReadSigned(u);
                d.ops.Add(MemoryOperand.Displaced(dt, reg, disp));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Msdisp_d0_16 = MsignedDisplacement(PrimitiveType.Word64, 25, 0, 16);
        private static readonly Mutator<Avr32Disassembler> Msdisp_w0_16 = MsignedDisplacement(PrimitiveType.Word32, 25, 0, 16);
        private static readonly Mutator<Avr32Disassembler> Msdisp_h0_16 = MsignedDisplacement(PrimitiveType.Word16, 25, 0, 16);
        private static readonly Mutator<Avr32Disassembler> Msdisp_b0_16 = MsignedDisplacement(PrimitiveType.Byte, 25, 0, 16);

        private static Mutator<Avr32Disassembler> Mpost(PrimitiveType dt, int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                var reg = Registers.GpRegisters[iReg];
                d.ops.Add(MemoryOperand.PostInc(dt, reg));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Mpost_w9 = Mpost(PrimitiveType.Word32, 9);
        private static readonly Mutator<Avr32Disassembler> Mpost_b9 = Mpost(PrimitiveType.Byte, 9);
        private static readonly Mutator<Avr32Disassembler> Mpost_w16 = Mpost(PrimitiveType.Word32, 16);

        private static Mutator<Avr32Disassembler> Mpre(PrimitiveType dt, int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                var reg = Registers.GpRegisters[iReg];
                d.ops.Add(MemoryOperand.PreDec(dt, reg));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Mpre_w9 = Mpre(PrimitiveType.Word32, 9);
        private static readonly Mutator<Avr32Disassembler> Mpre_b9 = Mpre(PrimitiveType.Byte, 9);
        private static readonly Mutator<Avr32Disassembler> Mpre_w16 = Mpre(PrimitiveType.Word32, 16);


        private static Mutator<Avr32Disassembler> Mrelative(RegisterStorage baseReg, int bitpos, int length)
        {
            var bitfield = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var iOffset = (int) bitfield.Read(u) << 2;
                var mem = MemoryOperand.Displaced(PrimitiveType.Word32, baseReg, iOffset);
                d.ops.Add(mem);
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Mpc4_7 = Mrelative(Registers.pc, 4, 7);
        private static readonly Mutator<Avr32Disassembler> Msp4_7 = Mrelative(Registers.sp, 4, 7);


        private static Mutator<Avr32Disassembler> Midx(PrimitiveType dt, int rBase, int rIndex, int shift)
        {
            var fieldBase = new Bitfield(rBase, 4);
            var fieldIndex = new Bitfield(rIndex, 4);
            var fieldShift = new Bitfield(shift, 2);
            return (u, d) =>
            {
                var gp = Registers.GpRegisters;
                var b = gp[fieldBase.Read(u)];
                var x = gp[fieldIndex.Read(u)];
                var sh = (int)fieldShift.Read(u);
                d.ops.Add(MemoryOperand.Indexed(dt, b, x, sh));
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> Midx_d = Midx(PrimitiveType.Word64, 25, 16, 4);
        private static readonly Mutator<Avr32Disassembler> Midx_sb = Midx(PrimitiveType.SByte, 25, 16, 4);
        private static readonly Mutator<Avr32Disassembler> Midx_sh = Midx(PrimitiveType.Int16, 25, 16, 4);
        private static readonly Mutator<Avr32Disassembler> Midx_ub = Midx(PrimitiveType.Byte, 25, 16, 4);
        private static readonly Mutator<Avr32Disassembler> Midx_uh = Midx(PrimitiveType.Word16, 25, 16, 4);
        private static readonly Mutator<Avr32Disassembler> Midx_w = Midx(PrimitiveType.Word32, 25, 16, 4);

        private static Mutator<Avr32Disassembler> MidxPartial(PrimitiveType dt, int rBase, int rIndex, int rPart)
        {
            var fieldBase = new Bitfield(rBase, 4);
            var fieldIndex = new Bitfield(rIndex, 4);
            var fieldPart = new Bitfield(rPart, 2);
            return (u, d) =>
            {
                var gp = Registers.GpRegisters;
                var b = gp[fieldBase.Read(u)];
                var x = gp[fieldIndex.Read(u)];
                var part = regParts[fieldPart.Read(u)];
                d.ops.Add(MemoryOperand.Indexed(dt, b, x, 2, part));
                return true;
            };
        }

        private static Mutator<Avr32Disassembler> Imm_signed(int bitPos, int length)
        {
            var bitfield = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                var imm = bitfield.ReadSigned(u);
                d.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Is0_16 = Imm_signed(0, 16);
        private static readonly Mutator<Avr32Disassembler> Is4_6 = Imm_signed(4, 6);
        private static readonly Mutator<Avr32Disassembler> Is4_8 = Imm_signed(4, 8);

        private static Mutator<Avr32Disassembler> Imm_signed(params Bitfield [] fields)
        {
            return (u, d) =>
            {
                var imm = Bitfield.ReadSignedFields(fields, u);
                d.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static Mutator<Avr32Disassembler> Imm_signedShifted(int bitPos, int length, int shift)
        {
            var bitfield = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                var imm = bitfield.ReadSigned(u) << shift;
                d.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> Is4_8sh2 = Imm_signedShifted(4, 8, 2);

        private static Mutator<Avr32Disassembler> Imm_unsigned(PrimitiveType dt, int bitPos, int length)
        {
            var bitfield = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                var imm = bitfield.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, imm)));
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> Iu32_5_5 = Imm_unsigned(PrimitiveType.Int32, 5, 5);
        private static readonly Mutator<Avr32Disassembler> Iu32_0_5 = Imm_unsigned(PrimitiveType.Int32, 0, 5);
        private static readonly Mutator<Avr32Disassembler> Iu8_0_8 = Imm_unsigned(PrimitiveType.Byte, 0, 8);
        private static readonly Mutator<Avr32Disassembler> Iu8_4_5 = Imm_unsigned(PrimitiveType.Byte, 4, 5);
        private static readonly Mutator<Avr32Disassembler> Iu8_13_3 = Imm_unsigned(PrimitiveType.Byte, 13, 3);
        private static readonly Mutator<Avr32Disassembler> Iu8_16_3 = Imm_unsigned(PrimitiveType.Byte, 16, 3);

        private static Mutator<Avr32Disassembler> Imm_unsignedShifted(PrimitiveType dt, int bitPos, int length, int shift)
        {
            var bitfield = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                var imm = bitfield.Read(u) << shift;
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, imm)));
                return true;
            };
        }

        private static Mutator<Avr32Disassembler> Imm_unsigned(PrimitiveType dt, params Bitfield[] fields)
        {
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, imm)));
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> Iu_h0_16 = Imm_unsigned(PrimitiveType.UInt16, 0, 16);
        private static readonly Mutator<Avr32Disassembler> Iu_w0_5 = Imm_unsigned(PrimitiveType.Word32, 0, 5);


        private static Mutator<Avr32Disassembler> Imm_unsigned_mapped(
            int bitpos,
            int bitlength,
            ImmediateOperand[] mapping)
        {
            var bitfield = new Bitfield(bitpos, bitlength);
            return (u, d) =>
            {
                var code = bitfield.Read(u);
                var op = mapping[code];
                d.ops.Add(op);
                return true;
            };
        }

        private static Mutator<Avr32Disassembler> multiRegister(bool read, params RegisterStorage[] regs)
        {
            return (uint uInstr, Avr32Disassembler dasm) =>
            {
                var regMask = uInstr & 0xFFFF;
                if (regMask == 0)
                    return false;
                if (read && (regMask & 0x8000) != 0)   // if we read PC, we are a jmp/return
                    dasm.iclass = InstrClass.Transfer;
                int iRegFirst = -1;
                uint m = 1u;
                for (int i = 0; i < regs.Length; ++i, m <<= 1)
                {
                    if ((regMask & m) != 0)
                    {
                        if (iRegFirst < 0)
                        {
                            iRegFirst = i;
                        }
                    }
                    else
                    {
                        if (iRegFirst >= 0)
                        {
                            dasm.ops.Add(new RegisterRange(regs, iRegFirst, i - iRegFirst));
                            iRegFirst = -1;
                        }
                    }
                }
                if (iRegFirst >= 0)
                {
                    dasm.ops.Add(new RegisterRange(regs, iRegFirst, regs.Length - iRegFirst));
                }
                return true;
            };
        }

        private static Mutator<Avr32Disassembler> multiRegisterCompact(bool pop, bool setr12)
        {
            return (uint uInstr, Avr32Disassembler dasm) =>
            {
                var regMask = Bits.ZeroExtend(uInstr >> 4, 8);
                if (regMask == 0)
                    return false;
                var gp = Registers.GpRegisters;
                if ((regMask & 1) != 0)
                {
                    dasm.ops.Add(new RegisterRange(gp, 0, 4));
                }
                if ((regMask & 2) != 0)
                {
                    dasm.ops.Add(new RegisterRange(gp, 4, 4));
                }
                if ((regMask & 4) != 0)
                {
                    dasm.ops.Add(new RegisterRange(gp, 8, 2));
                }
                if ((regMask & 8) != 0)
                {
                    dasm.ops.Add(gp[10]);
                }
                if ((regMask & 16) != 0)
                {
                    dasm.ops.Add(gp[11]);
                }
                if ((regMask & 32) != 0)
                {
                    dasm.ops.Add(gp[12]);
                }
                if ((regMask & 64) != 0 && !(pop && setr12))
                {
                    dasm.ops.Add(gp[14]);
                }
                if ((regMask & 128) != 0)
                {
                    if (pop)
                        dasm.iclass = InstrClass.Transfer;
                    dasm.ops.Add(gp[15]);
                    if (setr12)
                    {
                        switch (Bits.ZeroExtend(regMask >> 5, 2))
                        {
                        case 0: dasm.ops.Add(new RegisterImmediateOperand(gp[12], Mnemonic.mov, 0)); break;
                        case 1: dasm.ops.Add(new RegisterImmediateOperand(gp[12], Mnemonic.mov, 1)); break;
                        default: dasm.ops.Add(new RegisterImmediateOperand(gp[12], Mnemonic.mov, -1)); break;
                        }
                    }
                }
                return true;
            };
        }

        private static Mutator<Avr32Disassembler> PCRelative(Bitfield bfOffset)
        {
            return (u, d) =>
            {
                var offset = bfOffset.ReadSigned(u) << 1;
                var addr = d.addr + offset;
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }

        private static Mutator<Avr32Disassembler> PCRelative(params Bitfield [] bfOffset)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(bfOffset, u) << 1;
                var addr = d.addr + offset;
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Pcrel4_8 = PCRelative(new Bitfield(4,8));
        private static readonly Mutator<Avr32Disassembler> Pcrel_0_2_4_8 = PCRelative(Bf((0,2),(4,8)));
        private static readonly Mutator<Avr32Disassembler> PcRelBranch = PCRelative(Bf((25, 4), (20, 1), (0, 16)));

        private static bool COH(uint uInstr, Avr32Disassembler dasm)
        {
            dasm.ops.Add(new LiteralOperand("COH"));
            return true;
        }

        public static Mutator<Avr32Disassembler> Cond(int bitPos, int len)
        {
            var bitField = new Bitfield(bitPos, len);
            return (u, d) =>
            {
                var cond = (Avr32Condition) bitField.Read(u);
                d.condition = cond;
                return true;
            };
        }

        /// <summary>
        /// Checks the last two operands to make sure they make sense as a bitfield.
        /// </summary>
        private static bool bitfieldCheck(uint uInstr, Avr32Disassembler dasm)
        {
            if (dasm.ops.Count >= 2 &&
                dasm.ops[^2] is ImmediateOperand pos &&
                dasm.ops[^1] is ImmediateOperand width)
            {
                var p = pos.Value.ToInt32();
                var w = width.Value.ToUInt32();
                return w != 0 && p + w <= 32;
            }
            return false;
        }

        #endregion

        #region Predicates
        private static bool IsSp(uint iReg)
        {
            return iReg == 13;
        }

        #endregion

        #region Decoders

        private class UInt32Decoder : Decoder
        {
            private readonly Decoder decoder;

            public UInt32Decoder(Decoder decoder)
            {
                this.decoder = decoder;
            }

            public override Avr32Instruction Decode(uint wInstr, Avr32Disassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort uLow))
                    return dasm.CreateInvalidInstruction();
                uint wInstrLong = (wInstr << 16) | uLow;
                return decoder.Decode(wInstrLong, dasm);
            }
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<Avr32Disassembler, Mnemonic, Avr32Instruction>(message);
        }

        #endregion

        static Avr32Disassembler()
        {
            var invalid = Instr<Avr32Disassembler>(Mnemonic.invalid, InstrClass.Invalid);

            var ld_ub = Instr(Mnemonic.ld_ub, Rw0, Mdisp_ub4_3);
            var twoRegs = Mask(4, 5, "  twoRegs",
                Instr(Mnemonic.add, Rw0, R9),
                Instr(Mnemonic.sub, Rw0, R9),
                Instr(Mnemonic.rsub, Rw0, R9),
                Instr(Mnemonic.cp_w, R0, R9),

                Instr(Mnemonic.or, Rw0, R9),
                Instr(Mnemonic.eor, Rw0, R9),
                Instr(Mnemonic.and, Rw0, R9),
                Instr(Mnemonic.tst, R0, R9),

                Instr(Mnemonic.andn, Rw0, R9),
                Instr(Mnemonic.mov, Rw0, R9),
                Instr(Mnemonic.st_w, Mpost_w9, R0),
                Instr(Mnemonic.st_h, Mpost(PrimitiveType.Word16,9), R0),

                Instr(Mnemonic.st_b, Mpost_b9, R0),
                Instr(Mnemonic.st_w, Mpre_w9, R0),
                Instr(Mnemonic.st_h, Mpre(PrimitiveType.Word16, 9), R0),
                Instr(Mnemonic.st_b, Mpre_b9, R0),

                Instr(Mnemonic.ld_w, Rw0, Mpost_w9),
                Instr(Mnemonic.ld_sh, Rw0, Mpost(PrimitiveType.Int16, 9)),
                Instr(Mnemonic.ld_uh, Rw0, Mpost(PrimitiveType.Word16, 9)),
                Instr(Mnemonic.ld_ub, Rw0, Mpost(PrimitiveType.Byte, 9)),

                Instr(Mnemonic.ld_w, Rw0, Mpre_w9),
                Instr(Mnemonic.ld_sh, Rw0, Mpre(PrimitiveType.Int16, 9)),
                Instr(Mnemonic.ld_uh, Rw0, Mpre(PrimitiveType.Word16, 9)),
                Instr(Mnemonic.ld_ub, Rw0, Mpre(PrimitiveType.Byte, 9)),

                ld_ub,
                ld_ub,
                ld_ub,
                ld_ub,

                ld_ub,
                ld_ub,
                ld_ub,
                ld_ub);

            var k8immediateAndSingleRegister = Mask(12, 1, "  K8 immediate and single register",
                Select((0, 4), IsSp, " sub imm8",
                    Instr(Mnemonic.sub, Rw0, Is4_8sh2),
                    Instr(Mnemonic.sub, Rw0, Is4_8)),
                Instr(Mnemonic.mov, Rw0, Is4_8));

            var dispLdStK3imm = Mask(7, 2, "  Displacement load / store with k3 immediate",
                Instr(Mnemonic.ld_sh, Rw0, Mdisp_sh4_3),
                Instr(Mnemonic.ld_uh, Rw0, Mdisp_uh4_3),
                Instr(Mnemonic.st_w, Mdisp_w4_4, R0),
                Instr(Mnemonic.st_w, Mdisp_w4_4, R0));

            var singleReg = Sparse(4, 5, "Single register instructions", Nyi("Single register instructions"),
                (0b00000, Instr(Mnemonic.acr, R0)),
                (0b00001, Instr(Mnemonic.scr, R0)),
                (0b00010, Instr(Mnemonic.cpc, R0)),
                (0b00011, Instr(Mnemonic.neg, R0)),
                (0b00100, Instr(Mnemonic.abs, R0)),
                (0b00101, Instr(Mnemonic.castu_b, R0)),
                (0b00110, Instr(Mnemonic.casts_b, R0)),
                (0b00111, Instr(Mnemonic.castu_h, R0)),
                (0b01000, Instr(Mnemonic.casts_h, R0)),
                (0b01001, Instr(Mnemonic.brev, R0)),
                (0b01010, Instr(Mnemonic.swap_h, R0)),
                (0b01011, Instr(Mnemonic.swap_b, R0)),
                (0b01100, Instr(Mnemonic.swap_bh, R0)),
                (0b01101, Instr(Mnemonic.com, R0)),
                (0b01110, Instr(Mnemonic.tnbz, R0)),
                (0b01111, Instr(Mnemonic.rol, R0)),
                (0b10000, Instr(Mnemonic.ror, R0)),
                (0b10001, Instr(Mnemonic.icall, InstrClass.Transfer | InstrClass.Call,  R0)),
                (0b10010, Instr(Mnemonic.mustr, R0)),
                (0b10011, Instr(Mnemonic.musfr, R0))

                );

            var returnConditionally = Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return, Cond(4, 4), R0);

            var setRegisterConditionally = Instr(Mnemonic.sr, Cond(4, 4), Rw0);

            var spPcRelativeLdSt = Mask(11, 2, "  spPcRelativeLdSt",
                Instr(Mnemonic.lddsp, Rw0, Msp4_7),
                Instr(Mnemonic.lddpc, Rw0, Mpc4_7),
                Instr(Mnemonic.stdsp, Msp4_7, R0),
                Mask(9, 2, "  11",
                    Instr(Mnemonic.cp_w, R0, Is4_6),
                    Instr(Mnemonic.cp_w, R0, Is4_6),
                    singleReg,
                    Mask(8, 1, "  11",
                        returnConditionally,
                        setRegisterConditionally)));

            var add3WithShift = Instr(Mnemonic.add, Rw0, R25, Rlsl16_4_2);
            var sub3WithShift = Instr(Mnemonic.sub, Rw0, R25, Rlsl16_4_2);
            var lddIndexed = Instr(Mnemonic.ld_d, Rd1, Midx_d);
            var ldsbIndexed = Instr(Mnemonic.ld_sb, Rw0, Midx_sb);
            var ldshIndexed = Instr(Mnemonic.ld_sh, Rw0, Midx_sh);
            var ldubIndexed = Instr(Mnemonic.ld_ub, Rw0, Midx_ub);
            var lduhIndexed = Instr(Mnemonic.ld_uh, Rw0, Midx_uh);
            var ldwIndexed = Instr(Mnemonic.ld_w, Rw0, Midx_w);
            var ld_w_vi = Instr(Mnemonic.ld_w, Rw0, MidxPartial(PrimitiveType.Word32, 25, 16, 4));
            var machh_d = Instr(Mnemonic.machh_d, Rw0, Rh25_5, Rh16_4);
            var machh_w = Instr(Mnemonic.machh_w, Rw0, Rh25_5, Rh16_4);
            var macsathh_w = Instr(Mnemonic.macsathh_w, Rw0, Rh25_5, Rh16_4);
            var mulsathh_h = Instr(Mnemonic.mulsathh_h, Rw0, Rh25_5, Rh16_4);
            var mulsathh_w = Instr(Mnemonic.mulsathh_w, Rw0, Rh25_5, Rh16_4);
            var mulsatrndhh_w = Instr(Mnemonic.mulsatrndhh_h, Rw0, Rh25_5, Rh16_4);
            var mulsatrndwh_w = Instr(Mnemonic.mulsatrndwh_w, Rw0, Rh25_5, Rh16_4);
            var mulsatwh_w = Instr(Mnemonic.mulsatwh_w, Rw0, R25, Rh16_4);
            var mulwh_d = Instr(Mnemonic.mulwh_d, Rw0, R25, Rh16_4);

            var paddsub_h = Instr(Mnemonic.paddsub_h, Rw0, Rh25_5, Rh16_4);
            var paddsubh_sh = Instr(Mnemonic.paddsubh_sh, Rw0, Rh25_5, Rh16_4);
            var paddsubs_sh = Instr(Mnemonic.paddsubs_sh, Rw0, Rh25_5, Rh16_4);
            var paddsubs_uh = Instr(Mnemonic.paddsubs_uh, Rw0, Rh25_5, Rh16_4);
            var psubadd_h = Instr(Mnemonic.psubadd_h, Rw0, Rh25_5, Rh16_4);
            var psubaddh_sh = Instr(Mnemonic.psubaddh_sh, Rw0, Rh25_5, Rh16_4);
            var psubadds_sh = Instr(Mnemonic.psubadds_sh, Rw0, Rh25_5, Rh16_4);
            var psubadds_uh = Instr(Mnemonic.psubadds_uh, Rw0, Rh25_5, Rh16_4);

            var stbIndexed = Instr(Mnemonic.st_b, Midx_ub, R0);
            var stdIndexed = Instr(Mnemonic.st_d, Midx_ub, Rd1);
            var sthIndexed = Instr(Mnemonic.st_h, Midx_uh, R0);
            var stwIndexed = Instr(Mnemonic.st_w, Midx_w, R0);

            var subhh_w = Instr(Mnemonic.subhh_w, Rw0, Rh25_5, Rh16_4);

            var wide00000 = Sparse(8, 8, "  wide00000", Nyi(""),
                (0x00, Sparse(4, 4, "  0x00", Nyi(""),
                    (0b0000, add3WithShift),
                    (0b0001, add3WithShift),
                    (0b0010, add3WithShift),
                    (0b0011, add3WithShift),

                    (0b0100, Instr(Mnemonic.adc, Rw0, R25, R16)),
                    (0b1100, Instr(Mnemonic.satadd_w, Rw0, R25, R16)))),
                (0x01, Sparse(4, 4, "  0x00", Nyi(""),
                    (0b0000, sub3WithShift),
                    (0b0001, sub3WithShift),
                    (0b0010, sub3WithShift),
                    (0b0011, sub3WithShift),
                    (0b0100, Instr(Mnemonic.sbc, Rw0, R25, R16)),
                    (0b1001, Instr(Mnemonic.mulnhh_w, Rw0, Rh25_5, Rh16_4)),
                    (0b1100, Instr(Mnemonic.satsub_w, Rw0, R25, R16)))),
                (0x02, Sparse(4, 4, "  0x02", Nyi(""),
                    (0b0000, lddIndexed),
                    (0b0001, lddIndexed),
                    (0b0010, lddIndexed),
                    (0b0011, lddIndexed),
                    (0b0100, Instr(Mnemonic.mul, Rw0, R25, R16)),
                    (0b1000, Instr(Mnemonic.mulnwh_d, Rw0, Rh25_5, Rh16_4)),
                    (0b1001, Instr(Mnemonic.mulnwh_d, Rw0, Rh25_5, Rh16_4)),
                    (0b1100, Instr(Mnemonic.satadd_h, Rw0, R25, R16)))),
                (0x03, Sparse(4, 4, "  0x03", Nyi(""),
                    (0b0000, ldwIndexed),
                    (0b0001, ldwIndexed),
                    (0b0010, ldwIndexed),
                    (0b0011, ldwIndexed),
                    (0b0100, Instr(Mnemonic.mac, Rw0, R25, R16)),
                    (0b1100, Instr(Mnemonic.satsub_h, Rw0, R25, R16)))),
                (0x04, Sparse(4, 4, "  0x04", Nyi(""),
                    (0b0000, ldshIndexed),
                    (0b0001, ldshIndexed),
                    (0b0010, ldshIndexed),
                    (0b0011, ldshIndexed),
                    (0b0100, Instr(Mnemonic.muls_d, Rw0, R25, R16)),
                    (0b1010, machh_w),
                    (0b1011, machh_w))),
                (0x05, Sparse(4, 4, "  0x05", Nyi(""),
                    (0b0000, lduhIndexed),
                    (0b0001, lduhIndexed),
                    (0b0010, lduhIndexed),
                    (0b0011, lduhIndexed),
                    (0b0100, Instr(Mnemonic.macs_d, Rw0, R25, R16)),
                    (0b1010, machh_d),
                    (0b1011, machh_d))),
                (0x06, Sparse(4, 4, "  0x06", Nyi(""),
                    (0b0000, ldsbIndexed),
                    (0b0001, ldsbIndexed),
                    (0b0010, ldsbIndexed),
                    (0b0011, ldsbIndexed),
                    (0b0100, Instr(Mnemonic.mulu_d, Rw0, R25, R16)),
                    (0b1010, macsathh_w),
                    (0b1011, macsathh_w))),
                (0x07, Sparse(4, 4, "  0x07", Nyi(""),
                    (0b0000, ldubIndexed),
                    (0b0001, ldubIndexed),
                    (0b0010, ldubIndexed),
                    (0b0011, ldubIndexed),
                    (0b0100, Instr(Mnemonic.macu_d, Rw0, R25, R16)),
                    (0b1010, Instr(Mnemonic.mulhh_w, Rw0, Rh25_5, Rh16_4)))),
                (0x08, Sparse(4, 4, "  0x08", Nyi(""),
                    (0b0000, stdIndexed),
                    (0b0001, stdIndexed),
                    (0b0010, stdIndexed),
                    (0b0011, stdIndexed),
                    (0b0100, Instr(Mnemonic.asr, Rw0, R25, R16)),
                    (0b1000, mulsathh_h),
                    (0b1001, mulsathh_h),
                    (0b1010, mulsathh_h),
                    (0b1011, mulsathh_h))),
                (0x09, Sparse(4, 4, "  0x09", Nyi(""),
                    (0b0000, stwIndexed),
                    (0b0001, stwIndexed),
                    (0b0010, stwIndexed),
                    (0b0011, stwIndexed),
                    (0b0100, Instr(Mnemonic.lsl, Rw0, R25, R16)),
                    (0b1000, mulsathh_w),
                    (0b1001, mulsathh_w),
                    (0b1010, mulsathh_w),
                    (0b1011, mulsathh_w))),
                (0x0A, Sparse(4, 4, "  0x0A", Nyi(""),
                    (0b0000, sthIndexed),
                    (0b0001, sthIndexed),
                    (0b0010, sthIndexed),
                    (0b0011, sthIndexed),
                    (0b0100, Instr(Mnemonic.lsr, Rw0, R25, R16)),
                    (0b1000, mulsatrndhh_w),
                    (0b1001, mulsatrndhh_w),
                    (0b1010, mulsatrndhh_w),
                    (0b1011, mulsatrndhh_w))),
                (0x0B, Sparse(4, 4, "  0x0B", Nyi("111..00000..1101.."),
                    (0b0000, stbIndexed),
                    (0b0001, stbIndexed),
                    (0b0010, stbIndexed),
                    (0b0011, stbIndexed),
                    (0b0100, Instr(Mnemonic.xchg, Rw0, R25, R16)),
                    (0b1000, mulsatrndwh_w),
                    (0b1001, mulsatrndwh_w))),
                (0x0C, Sparse(4, 4, "  0x0C", Nyi(""),
                    (0b0000, Instr(Mnemonic.divs, Rw0, R25, R16)),
                    (0b0100, Instr(Mnemonic.max, Rw0, R25, R16)),
                    (0b1000, Instr(Mnemonic.macwh_d, Rw0, R25, Rh16_4)),
                    (0b1001, Instr(Mnemonic.macwh_d, Rw0, R25, Rh16_4)))),
                (0x0D, Sparse(4, 4, "  0x0D", Nyi(""),
                    (0b0000, Instr(Mnemonic.divu, Rw0, R25, R16)),
                    (0b0100, Instr(Mnemonic.min, Rw0, R25, R16)),
                    (0b1000, mulwh_d),
                    (0b1001, mulwh_d))),
                (0x0E, Sparse(4, 4, "  0x0E", Nyi("wide00000 0xE"),
                    (0b0000, Instr(Mnemonic.addhh_w, Rw0, Rh25_5, Rh16_4)),
                    (0b0001, Instr(Mnemonic.addhh_w, Rw0, Rh25_5, Rh16_4)),
                    (0b0010, Instr(Mnemonic.addhh_w, Rw0, Rh25_5, Rh16_4)),
                    (0b0011, Instr(Mnemonic.addhh_w, Rw0, Rh25_5, Rh16_4)),
                    (0b0100, Instr(Mnemonic.addabs, Rw0, R25, R16)),
                    (0b1000, mulsatwh_w),
                    (0b1001, mulsatwh_w))),
                (0x0F, Sparse(4, 4, "  0x0F", Nyi("0x0F"),
                    (0b0000, subhh_w), 
                    (0b0001, subhh_w), 
                    (0b0010, subhh_w), 
                    (0b0011, subhh_w), 
                    (0b1011, ld_w_vi))),
                (0x10, Instr(Mnemonic.mul, Rw16, R25, Imm_signed(0, 8))),
                (0x11, Instr(Mnemonic.rsub, Rw16, R25, Imm_signed(0, 8))),
                (0x12, Sparse(0, 8, "  0x12??", Nyi("wide00000 0x12"),
                    (0x00, Instr(Mnemonic.clz, Rw16, R25)))),
                (0x13, Sparse(0, 8, "  0x13??", Nyi("wide00000 0x13"),
                    (0x00, Instr(Mnemonic.cpc, Rw16, R25)))),
                (0x14, Sparse(5, 3, "  0x14", Nyi(""),
                    (0b000, Instr(Mnemonic.asr, R16, R25, Iu_w0_5)))),
                (0x15, Sparse(5, 3, "  0x15", Nyi(""),
                    (0b000, Instr(Mnemonic.lsl, Rw16, R25, Iu_w0_5)))),
                (0x16, Sparse(5, 3, "  0x16", Nyi(""),
                    (0b000, Instr(Mnemonic.lsr, Rw16, R25, Iu_w0_5)))),
                (0x17, Instr(Mnemonic.mov, Cond(4, 4), Rw16, R25)),
                (0x18, Sparse(0, 8, "  0x18", Nyi(""),
                    (0x00, Instr(Mnemonic.cp_b, R16, R25)))),
                (0x19, Sparse(0, 8, "  0x19", Nyi(""),
                    (0x00, Instr(Mnemonic.cp_h, R16, R25)))),
                (0x20, Sparse(4, 4, "  0x20", Nyi("0x20"),
                    (0b0000, Instr(Mnemonic.padd_h, Rw0, R25, R16)),
                    (0b0001, Instr(Mnemonic.psub_h, Rw0, R25, R16)),
                    (0b0010, Instr(Mnemonic.paddx_h, Rw0, R25, R16)),
                    (0b0011, Instr(Mnemonic.psubx_h, Rw0, R25, R16)),
                    (0b0100, Instr(Mnemonic.padds_sh, Rw0, R25, R16)),
                    (0b0101, Instr(Mnemonic.psubs_sh, Rw0, R25, R16)),
                    (0b0110, Instr(Mnemonic.paddxs_sh, Rw0, R25, R16)),
                    (0b0111, Instr(Mnemonic.psubxs_sh, Rw0, R25, R16)),
                    (0b1000, Instr(Mnemonic.padds_uh, Rw0, R25, R16)),
                    (0b1001, Instr(Mnemonic.psubs_uh, Rw0, R25, R16)),
                    (0b1010, Instr(Mnemonic.paddxs_uh, Rw0, R25, R16)),
                    (0b1011, Instr(Mnemonic.psubxs_uh, Rw0, R25, R16)),
                    (0b1100, Instr(Mnemonic.paddh_sh, Rw0, R25, R16)),
                    (0b1101, Instr(Mnemonic.psubh_sh, Rw0, R25, R16)),
                    (0b1110, Instr(Mnemonic.paddxh_sh, Rw0, R25, R16)),
                    (0b1111, Instr(Mnemonic.psubxh_sh, Rw0, R25, R16)))),
                (0x21, Sparse(4, 4, "  0x20", Nyi("0x20"),
                    (0b0000, paddsub_h),
                    (0b0001, paddsub_h),
                    (0b0010, paddsub_h),
                    (0b0011, paddsub_h),
                    (0b0100, psubadd_h),
                    (0b0101, psubadd_h),
                    (0b0110, psubadd_h),
                    (0b0111, psubadd_h),
                    (0b1000, paddsubs_sh),
                    (0b1001, paddsubs_sh),
                    (0b1010, paddsubs_sh),
                    (0b1011, paddsubs_sh),
                    (0b1100, psubadds_sh),
                    (0b1101, psubadds_sh),
                    (0b1110, psubadds_sh),
                    (0b1111, psubadds_sh))),
                (0x22, Sparse(4, 4, "  0x20", Nyi("0x20"),
                    (0b0000, paddsubs_uh),
                    (0b0001, paddsubs_uh),
                    (0b0010, paddsubs_uh),
                    (0b0011, paddsubs_uh),
                    (0b0100, psubadds_uh),
                    (0b0101, psubadds_uh),
                    (0b0110, psubadds_uh),
                    (0b0111, psubadds_uh),
                    (0b1000, paddsubh_sh),
                    (0b1001, paddsubh_sh),
                    (0b1010, paddsubh_sh),
                    (0b1011, paddsubh_sh),
                    (0b1100, psubaddh_sh),
                    (0b1101, psubaddh_sh),
                    (0b1110, psubaddh_sh),
                    (0b1111, psubaddh_sh))),
                (0x23, Sparse(4, 4, "  0x23", Nyi("0x23"),
                    (0b0000, Instr(Mnemonic.padd_b, Rw0, R25, R16)),
                    (0b0001, Instr(Mnemonic.psub_b, Rw0, R25, R16)),
                    (0b0010, Instr(Mnemonic.padds_sb, Rw0, R25, R16)),
                    (0b0011, Instr(Mnemonic.psubs_sb, Rw0, R25, R16)),
                    (0b0100, Instr(Mnemonic.padds_ub, Rw0, R25, R16)),
                    (0b0101, Instr(Mnemonic.psubs_ub, Rw0, R25, R16)),
                    (0b0110, Instr(Mnemonic.paddh_ub, Rw0, R25, R16)),
                    (0b0111, Instr(Mnemonic.psubh_ub, Rw0, R25, R16)),
                    (0b1000, Instr(Mnemonic.pmax_ub, Rw0, R25, R16)),
                    (0b1001, Instr(Mnemonic.pmax_sh, Rw0, R25, R16)),
                    (0b1010, Instr(Mnemonic.pmin_ub, Rw0, R25, R16)),
                    (0b1011, Instr(Mnemonic.pmin_sh, Rw0, R25, R16)),
                    (0b1100, Instr(Mnemonic.pavg_ub, Rw0, R25, R16)),
                    (0b1101, Instr(Mnemonic.pavg_sh, Rw0, R25, R16)),
                    (0b1110, Instr(Mnemonic.pabs_sb, Rw0, R16)),
                    (0b1111, Instr(Mnemonic.pabs_sw, Rw0, R16)))),
                (0x24, Sparse(4, 4, "  0x24", Nyi("0x24"),
                    (0b0000, Instr(Mnemonic.psad, Rw0, R25, R16)),
                    (0b0001, Instr(Mnemonic.pasr_b, Rw0, R25, Iu8_16_3)),
                    (0b0010, Instr(Mnemonic.plsl_b, Rw0, R25, Iu8_16_3)),
                    (0b0011, Instr(Mnemonic.plsr_b, Rw0, R25, Iu8_16_3)),
                    (0b0100, Instr(Mnemonic.pasr_h, Rw0, R25, Iu8_16_3)),
                    (0b0101, Instr(Mnemonic.plsl_h, Rw0, R25, Iu8_16_3)),
                    (0b0110, Instr(Mnemonic.plsr_h, Rw0, R25, Iu8_16_3)),
                    (0b0111, Instr(Mnemonic.packw_sh, Rw0, R25, R16)),
                    (0b1001, Instr(Mnemonic.punpckub_h, Rw0, Rh25_4)),
                    (0b1011, Instr(Mnemonic.punpcksb_h, Rw0, Rh25_4)),
                    (0b1100, Instr(Mnemonic.packsh_ub, Rw0, R25, R16)),
                    (0b1101, Instr(Mnemonic.packsh_sb, Rw0, R25, R16)))));

            var cacheOperation =
                Instr(Mnemonic.cache, InstrClass.Linear | InstrClass.Privileged,
                    MsignedDisplacement(PrimitiveType.Byte, 25, 0, 11),
                    Imm_unsigned(PrimitiveType.Byte, 11, 5));
             
            var c = Registers.CoprocRegisters;
            var copregs = multiRegister(false,
                c[0], c[2], c[4], c[6], c[8], c[10], c[12], c[14]);
            var copregsLo = multiRegister(false,
                c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7]);
            var copregsHi = multiRegister(false,
                c[8], c[9], c[10], c[11], c[12], c[13], c[14], c[15]);

            var wide00001 = Mask(25, 4, "  00001",
                Instr(Mnemonic.andl, Rw16,Iu_h0_16),
                Instr(Mnemonic.andl, Rw16,Iu_h0_16,COH),
                Instr(Mnemonic.andh, Rw16,Iu_h0_16),
                Instr(Mnemonic.andh, Rw16,Iu_h0_16,COH),

                Instr(Mnemonic.orl, Rw16,Iu_h0_16),
                Instr(Mnemonic.orh, Rw16,Iu_h0_16),
                Instr(Mnemonic.eorl, Rw16, Iu_h0_16),
                Instr(Mnemonic.eorh, Rw16, Iu_h0_16),

                Instr(Mnemonic.mcall, InstrClass.Transfer|InstrClass.Call, Mdisp_w0_16),
                Instr(Mnemonic.pref, Msdisp_b0_16),
                cacheOperation,
                Instr(Mnemonic.memc, Imm_signed(0, 15), Imm_unsigned(PrimitiveType.Byte, 15, 5)),

                Instr(Mnemonic.mems, Imm_signed(0, 15), Imm_unsigned(PrimitiveType.Byte, 15, 5)),
                Instr(Mnemonic.memt, Imm_signed(0, 15), Imm_unsigned(PrimitiveType.Byte, 15, 5)),
                Instr(Mnemonic.movh, Rw16, Iu_h0_16),
                Nyi("0b1111"));

            var brLong = Instr(Mnemonic.br, InstrClass.ConditionalTransfer, Cond(16, 4), PcRelBranch);

            var rcallLong = Instr(Mnemonic.rcall, InstrClass.Transfer | InstrClass.Call, PcRelBranch);

            var addLongImm = Instr(Mnemonic.sub, Rw16, Imm_signed(Bf((25, 4), (20, 1), (0, 16))));

            var cpwLongImm = Instr(Mnemonic.cp_w, R16, Imm_signed(Bf((25, 4), (20, 1), (0, 16))));

            var ldc_d_iii = Instr(Mnemonic.ldc_d, Iu8_13_3, CR8, Midx(PrimitiveType.Word32, 16, 0, 4));
            var ldc_w_vi = Instr(Mnemonic.ldc_w, Iu8_13_3, CR8, Midx(PrimitiveType.Word32, 16, 0, 4));

            var movLongImm = Instr(Mnemonic.mov, Rw16, Imm_signed(Bf((25, 4), (20, 1), (0, 16))));
            var ldins_b = Instr(Mnemonic.ldins_b, Rhb16_12, MsignedDisplacement(PrimitiveType.Byte, 25, 0, 12));
            var ldins_h = Instr(Mnemonic.ldins_h, Rhw16_12, MsignedDisplacement(PrimitiveType.Word16, 25, 0, 12));

            var cop = Instr(Mnemonic.cop, Iu8_13_3, CR8, CR4, CR0, Imm_unsigned(PrimitiveType.Byte, Bf((25, 2), (16, 4), (12, 1))));
            var stc_d_iii = Instr(Mnemonic.stc_d, Iu8_13_3, Midx(PrimitiveType.Word32, 16, 0, 4), CR8);
            var stc_w_vi = Instr(Mnemonic.stc_w, Iu8_13_3, Midx(PrimitiveType.Word32, 16, 0, 4), CR8);

            var wideInstr = new UInt32Decoder(Mask(20, 5, "  wide",
                wide00000,
                wide00001,
                addLongImm,
                addLongImm,

                cpwLongImm,
                cpwLongImm,
                movLongImm,
                movLongImm,

                brLong,
                brLong,
                rcallLong,
                rcallLong,

                Instr(Mnemonic.sub, Rw16, R25, Is0_16),
                Instr(Mnemonic.satsub_w, Rw16, R25, Is0_16),
                Mask(16, 1, "  01110",
                    Instr(Mnemonic.ld_d, Rd17, Msdisp_d0_16),
                    Instr(Mnemonic.st_d, Msdisp_d0_16, Rd17)),
                Instr(Mnemonic.ld_w, Rw16, Msdisp_w0_16),

                Instr(Mnemonic.ld_sh, Rw16, MsignedDisplacement(PrimitiveType.Int16, 25, 0, 16)),
                Instr(Mnemonic.ld_uh, Rw16, MsignedDisplacement(PrimitiveType.Word16, 25, 0, 16)),
                Instr(Mnemonic.ld_sb, Rw16, MsignedDisplacement(PrimitiveType.SByte, 25, 0, 16)),
                Instr(Mnemonic.ld_ub, Rw16, MsignedDisplacement(PrimitiveType.Byte, 25, 0, 16)),

                Instr(Mnemonic.st_w, Msdisp_w0_16, R16),
                Instr(Mnemonic.st_h, Msdisp_h0_16, R16),
                Instr(Mnemonic.st_b, Msdisp_b0_16, R16),
                Instr(Mnemonic.stcond, Msdisp_w0_16, R16),

                Nyi("0b11000"),
                Nyi("0b11001"),
                Sparse(25, 4, "  11010", Nyi("11010"),
                    (0x0, cop),
                    (0x1, cop),
                    (0x2, cop),
                    (0x3, cop),
                    (0x4, Mask(12, 1, "  ldc",
                        Instr(Mnemonic.ldc_w, Iu8_13_3, CR8, MunsignedDisplacement(PrimitiveType.Word64, 16, 0, 8)),
                        Instr(Mnemonic.ldc_d, Iu8_13_3, CR8, MunsignedDisplacement(PrimitiveType.Word64, 16, 0, 8)))),
                    (0x5, Mask(12, 1, "  stc",
                        Instr(Mnemonic.stc_w, Iu8_13_3, MunsignedDisplacement(PrimitiveType.Word64, 16, 0, 8), CR8),
                        Instr(Mnemonic.stc_d, Iu8_13_3, MunsignedDisplacement(PrimitiveType.Word64, 16, 0, 8), CR8))),
                    (0x6, Sparse(8, 4, "  ldcm", Nyi("ldcm"),
                        (0x0, Mask(12, 1, "  ldcm.w (lo)",
                            Instr(Mnemonic.ldcm_w, Iu8_13_3, R16, copregsLo),
                            Instr(Mnemonic.ldcm_w, Iu8_13_3, Mpost_w16, copregsLo))),
                        (0x1, Mask(12, 1, "  ldcm.w (hio)",
                            Instr(Mnemonic.ldcm_w, Iu8_13_3, R16, copregsHi),
                            Instr(Mnemonic.ldcm_w, Iu8_13_3, Mpost_w16, copregsHi))),
                        (0x2, Mask(12, 1, "  stcm.w (hio)",
                            Instr(Mnemonic.stcm_w, Iu8_13_3, R16, copregsLo),
                            Instr(Mnemonic.stcm_w, Iu8_13_3, Mpre_w16, copregsLo))),
                        (0x3, Mask(12, 1, "  stcm.w (hio)",
                            Instr(Mnemonic.stcm_w, Iu8_13_3, R16, copregsHi),
                            Instr(Mnemonic.stcm_w, Iu8_13_3, Mpre_w16, copregsHi))),
                        (0x4, Mask(12, 1, "  ldcm.d",
                            Instr(Mnemonic.ldcm_d, Iu8_13_3, R16, copregs),
                            Instr(Mnemonic.ldcm_d, Iu8_13_3, Mpost_w16, copregs))),
                        (0x5, Mask(12, 1, "  stcm.d",
                            Instr(Mnemonic.stcm_d, Iu8_13_3, R16, copregs),
                            Instr(Mnemonic.stcm_d, Iu8_13_3, Mpre_w16, copregs))))),
                    (0x7, Mask(12, 1, 4, 4, "  0111", Nyi("cr"),
                        (0b0_0000, Instr(Mnemonic.mvcr_w,Iu8_13_3, R16, CR8)),
                        (0b0_0001, Instr(Mnemonic.mvcr_d,Iu8_13_3, R16, CR8)),
                        (0b0_0010, Instr(Mnemonic.mvrc_w, Iu8_13_3, CR8, R16)),
                        (0b0_0011, Instr(Mnemonic.mvrc_d, Iu8_13_3, CR8, R16)),
                        (0b0_0100, Instr(Mnemonic.ldc_w, Iu8_13_3, CR8, Mpost_w16)),
                        (0b0_0101, Instr(Mnemonic.ldc_d, Iu8_13_3, CR8, Mpost_w16)),
                        (0b0_0110, Instr(Mnemonic.stc_w, Iu8_13_3, Mpre_w16, CR8)),
                        (0b0_0111, Instr(Mnemonic.stc_d, Iu8_13_3, Mpre_w16, CR8)),
                        (0b1_0000, ldc_w_vi),
                        (0b1_0001, ldc_w_vi),
                        (0b1_0010, ldc_w_vi),
                        (0b1_0011, ldc_w_vi),
                        (0b1_0100, ldc_d_iii),
                        (0b1_0101, ldc_d_iii),
                        (0b1_0110, ldc_d_iii),
                        (0b1_0111, ldc_d_iii),
                        (0b1_1000, stc_w_vi),
                        (0b1_1001, stc_w_vi),
                        (0b1_1010, stc_w_vi),
                        (0b1_1011, stc_w_vi),
                        (0b1_1100, stc_d_iii),
                        (0b1_1101, stc_d_iii),
                        (0b1_1110, stc_d_iii),
                        (0b1_1111, stc_d_iii))),

                    (0x8, Instr(Mnemonic.ldc0_w, CR8, MunsignedDisplacement(PrimitiveType.Word32, 16, Bf((12,4),(0,8))))),
                    (0x9, Instr(Mnemonic.ldc0_d, CR8, MunsignedDisplacement(PrimitiveType.Word32, 16, Bf((12,4), (0,8))))),
                    (0xA, Instr(Mnemonic.stc0_w, MunsignedDisplacement(PrimitiveType.Word32, 16, Bf((12, 4), (0, 8))), CR8)),
                    (0xB, Instr(Mnemonic.stc0_d, MunsignedDisplacement(PrimitiveType.Word32, 16, Bf((12, 4), (0, 8))), CR8))),
                Sparse(25, 4, "  wide 11011", Nyi("0b11011"),
                    (0b0000, Instr(Mnemonic.mfsr, InstrClass.Linear|InstrClass.Privileged, Rw16, Imm_unsigned(PrimitiveType.UInt32, 0, 8))),
                    (0b0001, Instr(Mnemonic.mtsr, InstrClass.Linear|InstrClass.Privileged, Imm_unsigned(PrimitiveType.UInt32, 0, 8), Rw16)),
                    (0b0010, Instr(Mnemonic.mfdr, InstrClass.Linear|InstrClass.Privileged, Rw16, Imm_unsigned(PrimitiveType.UInt32, 0, 8))),
                    (0b0011, Instr(Mnemonic.mtdr, InstrClass.Linear|InstrClass.Privileged, Imm_unsigned(PrimitiveType.UInt32, 0, 8), R16)),
                    (0b0100, Instr(Mnemonic.sleep, InstrClass.Linear | InstrClass.Privileged, Iu8_0_8)),
                    (0b0101, Instr(Mnemonic.sync, Iu8_0_8)),
                    (0b0110, Instr(Mnemonic.bld, Rw16, Imm_unsigned(PrimitiveType.Int32, 0, 5))),
                    (0b0111, Instr(Mnemonic.bst, Rw16, Imm_unsigned(PrimitiveType.Int32, 0, 5))),
                    (0b1000, Sparse(10, 6, "  1000", Nyi("111100011011"),
                        (0b0000_00, Instr(Mnemonic.sats, Rlsr16_0_5, Imm_unsigned(PrimitiveType.Int32, 5, 5))),
                        (0b0000_01, Instr(Mnemonic.satu, Rlsr16_0_5, Imm_unsigned(PrimitiveType.Int32, 5, 5))))),
                    (0b1001, Sparse(10, 6, "  1001", Nyi("111100111011"),
                        (0b0000_00, Instr(Mnemonic.satrnds, Rlsr16_0_5, Imm_unsigned(PrimitiveType.Int32, 5, 5))),
                        (0b0000_01, Instr(Mnemonic.satrndu, Rlsr16_0_5, Imm_unsigned(PrimitiveType.Int32, 5, 5))))),
                    // There seems to be an error in the manual: subf doesn't have an 
                    // addf counterpart.
                    (0b1010, Instr(Mnemonic.subf, Cond(8, 4), Rw16, Imm_signed(0, 8))),
                    (0b1011, Instr(Mnemonic.sub, Cond(8, 4), Rw16, Imm_signed(0, 8))),
                    (0b1100, Instr(Mnemonic.mov, Cond(8, 4), Rw16, Imm_signed(0, 8))),
                    (0b1101, Instr(Mnemonic.rsub, Cond(8, 4), Rw16, Imm_signed(0, 8)))),

                Mask(26, 3, "  STMxx",
                    Mask(25, 1, "  LDM",
                        Instr(Mnemonic.ldm, R16, multiRegister(true, Registers.GpRegisters)),
                        Instr(Mnemonic.ldm, Mpost_w16, multiRegister(true, Registers.GpRegisters))),
                    Mask(25, 1, "  LDM",
                        Instr(Mnemonic.ldmts, R16, multiRegister(true, Registers.GpRegisters)),
                        Instr(Mnemonic.ldmts, Mpost_w16, multiRegister(true, Registers.GpRegisters))),
                    Mask(25, 1, "  STM",
                        Instr(Mnemonic.stm, R16, multiRegister(false, Registers.GpRegisters)),
                        Instr(Mnemonic.stm, Mpre_w16, multiRegister(false, Registers.GpRegisters))),
                    Mask(25, 1, "  STMTS",
                        Instr(Mnemonic.stmts, R16, multiRegister(false, Registers.GpRegisters)),
                        Instr(Mnemonic.stmts, Mpre_w16, multiRegister(false, Registers.GpRegisters))),
                    invalid,
                    invalid,
                    invalid,
                    invalid),
                Sparse(12, 4, "  11101", Nyi("11101"),
                    (0b0000, ldins_h),
                    (0b0001, ldins_h),
                    (0b0010, Instr(Mnemonic.ldswp_sh, Rw16, MsignedDisplacement(PrimitiveType.Int16, 25, 0, 12))),
                    (0b0011, Instr(Mnemonic.ldswp_uh, Rw16, MsignedDisplacement(PrimitiveType.UInt16, 25, 0, 12))),
                    (0b0100, ldins_b),
                    (0b0101, ldins_b),
                    (0b0110, ldins_b),
                    (0b0111, ldins_b),
                    (0b1000, Instr(Mnemonic.ldswp_w, Rw16, MsignedDisplacement(PrimitiveType.Word32, 25, 0, 12))),
                    (0b1001, Instr(Mnemonic.stswp_h, MsignedDisplacement(PrimitiveType.Word16, 25, 0, 12), R16)),
                    (0b1010, Instr(Mnemonic.stswp_w, MsignedDisplacement(PrimitiveType.Word32, 25, 0, 12), R16)),
                    (0b1011, Instr(Mnemonic.bfexts, Rw25, R16, Iu32_5_5, Iu32_0_5, bitfieldCheck)),
                    (0b1100, Instr(Mnemonic.bfextu, Rw25, R16, Iu32_5_5, Iu32_0_5, bitfieldCheck)),
                    (0b1101, Instr(Mnemonic.bfins, Rw25, R16, Iu32_5_5, Iu32_0_5, bitfieldCheck)),
                    (0b1110, Sparse(4, 4, "  1110", Nyi("111 11011 1110"),
                        (0b0000, Instr(Mnemonic.add, Cond(8, 4), Rw0, R25, R16)),
                        (0b0001, Instr(Mnemonic.sub, Cond(8, 4), Rw0, R25, R16)),
                        (0b0010, Instr(Mnemonic.and, Cond(8, 4), Rw0, R25, R16)),
                        (0b0011, Instr(Mnemonic.or, Cond(8, 4), Rw0, R25, R16)),
                        (0b0100, Instr(Mnemonic.eor, Cond(8, 4), Rw0, R25, R16))))),
                Mask(14, 2, "  11110",
                    Sparse(9, 5, "0b11110", Nyi("0b11110"),
                        (0b00000, Instr(Mnemonic.and, Rw0, R25, Rlsl16_4_5)),
                        (0b00001, Instr(Mnemonic.and, Rw0, R25, Rlsr16_4_5)),
                        (0b01000, Instr(Mnemonic.or, Rw0, R25, Rlsl16_4_5)),
                        (0b01001, Instr(Mnemonic.or, Rw0, R25, Rlsr16_4_5)),
                        (0b10000, Instr(Mnemonic.eor, Rw0, R25, Rlsl16_4_5)),
                        (0b10001, Instr(Mnemonic.eor, Rw0, R25, Rlsr16_4_5))),
                    Nyi("0b01"),
                    Instr(Mnemonic.sthh_w, Midx(PrimitiveType.Word32, 0, 8, 4), Rh25_13, Rh16_12),
                    Instr(Mnemonic.sthh_w, MunsignedDisplacement(PrimitiveType.Word32, 0, 4, 8), Rh25_13, Rh16_12)),
                Mask(9, 3, "  0b11111",
                    Instr(Mnemonic.ld_w, Cond(12, 4), Rw16, MunsignedDisplacement(PrimitiveType.Word32, 25, 0, 9)),
                    Instr(Mnemonic.ld_sh, Cond(12, 4), Rw16, MunsignedDisplacement(PrimitiveType.Int16, 25, 0, 9)),
                    Instr(Mnemonic.ld_uh, Cond(12, 4), Rw16, MunsignedDisplacement(PrimitiveType.Word16, 25, 0, 9)),
                    Instr(Mnemonic.ld_sb, Cond(12, 4), Rw16, MunsignedDisplacement(PrimitiveType.SByte, 25, 0, 9)),

                    Instr(Mnemonic.ld_ub, Cond(12, 4), Rw16, MunsignedDisplacement(PrimitiveType.Byte, 25, 0, 9)),
                    Instr(Mnemonic.st_w, Cond(12, 4), MunsignedDisplacement(PrimitiveType.Word32, 25, 0, 9), R16),
                    Instr(Mnemonic.st_h, Cond(12, 4), MunsignedDisplacement(PrimitiveType.Word16, 25, 0, 9), R16),
                    Instr(Mnemonic.st_b, Cond(12, 4), MunsignedDisplacement(PrimitiveType.Byte, 25, 0, 9), R16))));

            var shortBranch = Instr(Mnemonic.br, InstrClass.ConditionalTransfer, Cond(0, 3), Pcrel4_8);

            var incjosp = Instr(Mnemonic.incjosp, Imm_unsigned_mapped(4, 3, new[]
            {
                ImmediateOperand.Int32(1),
                ImmediateOperand.Int32(2),
                ImmediateOperand.Int32(3),
                ImmediateOperand.Int32(4),
                ImmediateOperand.Int32(-4),
                ImmediateOperand.Int32(-3),
                ImmediateOperand.Int32(-2),
                ImmediateOperand.Int32(-1),
            }));
            
            rootDecoder = Mask(13, 3, "AVR32",
                twoRegs,
                k8immediateAndSingleRegister,
                spPcRelativeLdSt,
                Instr(Mnemonic.ld_w, Rw0, Mdisp_w4_5),
                dispLdStK3imm,
                Mask(7, 2, "  101",
                    Instr(Mnemonic.st_h, Mdisp_h4_3, R0),
                    Instr(Mnemonic.st_b, Mdisp_ub4_3, R0),
                    Mask(5, 2, "  10??",
                        Mask(4, 1, "  1000?",
                            Mask(0, 1, "  10000 ?",
                                Instr(Mnemonic.ld_d, Rd1, Mpre(PrimitiveType.Word64, 9)),
                                Instr(Mnemonic.ld_d, Rd1, Mpost(PrimitiveType.Word64, 9))),
                            Mask(0, 1, "  10001 ?",
                                Instr(Mnemonic.ld_d, Rd1, M(PrimitiveType.Word64, 9)),
                                Instr(Mnemonic.st_d, M(PrimitiveType.Word64, 9), Rd1))),
                        Mask(4, 1, "  1001?",
                            Mask(0, 1, "  10010",
                                Instr(Mnemonic.st_d, Mpost(PrimitiveType.Word64, 9), Rd1),
                                Instr(Mnemonic.st_d, Mpre(PrimitiveType.Word64, 9), Rd1)),
                            Instr(Mnemonic.mul,Rw0,R9)),
                        Instr(Mnemonic.asr, Rw0,Imm_unsigned(PrimitiveType.Int32, Bf((9,4),(4,1)))),
                        Instr(Mnemonic.lsl, Rw0,Imm_unsigned(PrimitiveType.Int32, Bf((9,4),(4,1))))),
                    Mask(5, 2, "  0b11??",
                        Instr(Mnemonic.lsr, Rw0,Imm_unsigned(PrimitiveType.Int32, Bf((9,4),(4,1)))),
                        Instr(Mnemonic.sbr, Rw0,Imm_unsigned(PrimitiveType.Int32, Bf((9,4),(4,1)))),
                        Instr(Mnemonic.cbr, Rw0,Imm_unsigned(PrimitiveType.Int32, Bf((9,4),(4,1)))),
                        Nyi("0b1111"))),
                Mask(12, 1, "  110",
                    Mask(2, 2, "  0",
                        shortBranch,
                        shortBranch,
                        Instr(Mnemonic.rjmp, InstrClass.Transfer, Pcrel_0_2_4_8),
                        Instr(Mnemonic.rcall, InstrClass.Call | InstrClass.Transfer, Pcrel_0_2_4_8)),
                    Mask(0, 4, "  1",
                        Instr(Mnemonic.acall, InstrClass.Call | InstrClass.Transfer, Imm_unsignedShifted(PrimitiveType.Int32, 4, 8, 2)),
                        Instr(Mnemonic.pushm, multiRegisterCompact(false, false)),
                        Instr(Mnemonic.popm, multiRegisterCompact(true, false)),
                        Mask(9, 3, "  0011",
                            Mask(8, 1, "  csrfcz",
                                Instr(Mnemonic.csrfcz, Iu8_4_5),
                                Instr(Mnemonic.csrfcz, InstrClass.Linear | InstrClass.Privileged, Iu8_4_5)),
                            Mask(8, 1, "  ssrf",
                                Instr(Mnemonic.ssrf, Iu8_4_5),
                                Instr(Mnemonic.ssrf, InstrClass.Linear | InstrClass.Privileged, Iu8_4_5)),
                            Mask(8, 1, "  csrf",
                                Instr(Mnemonic.csrf, Iu8_4_5),
                                Instr(Mnemonic.csrf, InstrClass.Linear|InstrClass.Privileged, Iu8_4_5)),
                            Sparse(4, 5, "  011", Nyi("110..011"),
                                (0x00, Instr<Avr32Disassembler>(Mnemonic.rete, InstrClass.Transfer|InstrClass.Return)),
                                (0x01, Instr<Avr32Disassembler>(Mnemonic.rets, InstrClass.Transfer|InstrClass.Return)),
                                (0x02, Instr<Avr32Disassembler>(Mnemonic.retd, InstrClass.Transfer|InstrClass.Return)),
                                (0x03, Instr<Avr32Disassembler>(Mnemonic.retj, InstrClass.Transfer|InstrClass.Return)),
                                (0x04, Instr<Avr32Disassembler>(Mnemonic.tlbr, InstrClass.Linear|InstrClass.Privileged)),
                                (0x05, Instr<Avr32Disassembler>(Mnemonic.tlbs, InstrClass.Linear|InstrClass.Privileged)),
                                (0x06, Instr<Avr32Disassembler>(Mnemonic.tlbw, InstrClass.Linear|InstrClass.Privileged)),
                                (0x07, Instr<Avr32Disassembler>(Mnemonic.breakpoint)),
                                (0x08, incjosp),
                                (0x09, incjosp),
                                (0x0A, incjosp),
                                (0x0B, incjosp),
                                (0x0C, incjosp),
                                (0x0D, incjosp),
                                (0x0E, incjosp),
                                (0x0F, incjosp),
                                (0x10, Instr<Avr32Disassembler>(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding)),
                                (0x11, Instr<Avr32Disassembler>(Mnemonic.popjc)),
                                (0x12, Instr<Avr32Disassembler>(Mnemonic.pushjc)),
                                (0x13, Instr<Avr32Disassembler>(Mnemonic.scall)),
                                (0x14, Instr<Avr32Disassembler>(Mnemonic.frs)),
                                (0x15, Instr<Avr32Disassembler>(Mnemonic.sscall)),
                                (0x16, Instr<Avr32Disassembler>(Mnemonic.retss, InstrClass.Transfer|InstrClass.Return))),
                            Nyi("100"),
                            Nyi("101"),
                            Nyi("110"),
                            Nyi("111")),

                        Nyi("0b0100"),
                        Nyi("0b0101"),
                        invalid,
                        invalid,

                        Nyi("0b1000"),
                        Nyi("0b1001"),
                        Instr(Mnemonic.popm, multiRegisterCompact(true, true)),
                        invalid,
                        
                        invalid,
                        invalid,
                        invalid,
                        invalid)),
                wideInstr);
        }
    }
}