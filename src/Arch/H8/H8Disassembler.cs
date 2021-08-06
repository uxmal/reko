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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Reko.Arch.H8
{
#pragma warning disable IDE1006

    using Decoder = Decoder<H8Disassembler, Mnemonic, H8Instruction>;

    public class H8Disassembler : DisassemblerBase<H8Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly H8Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private PrimitiveType? dataSize;
        private RegisterStorage[] regs;

        public H8Disassembler(H8Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.addr = rdr.Address;
            this.ops = new List<MachineOperand>();
            this.regs = Registers.GpRegisters;
        }

        public override H8Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out ushort uInstr))
                return null;
            ops.Clear();
            dataSize = null;
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - this.addr);
            return instr;
        }

        public override H8Instruction CreateInvalidInstruction()
        {
            return new H8Instruction
            {
                Mnemonic = Mnemonic.Invalid,
                InstructionClass = InstrClass.Invalid,
                Operands = MachineInstruction.NoOperands,
            };
        }

        public override H8Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new H8Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Size = this.dataSize,
                Operands = ops.ToArray(),
            };
        }

        public override H8Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("H8Dis", this.addr, rdr, message);
            return CreateInvalidInstruction();
        }

        #region Mutators
        private static bool b(uint _, H8Disassembler dasm)
        {
            dasm.dataSize = PrimitiveType.Byte;
            dasm.regs = Registers.Gp8Registers;
            return true;
        }

        private static bool w(uint _, H8Disassembler dasm)
        {
            dasm.dataSize = PrimitiveType.Word16;
            dasm.regs = Registers.Gp16Registers;
            return true;
        }

        private static bool l(uint _, H8Disassembler dasm)
        {
            dasm.dataSize = PrimitiveType.Word32;
            dasm.regs = Registers.GpRegisters;
            return true;
        }

        /// <summary>
        /// 32-bit register encoded in low 4 bits.
        /// </summary>
        private static bool rgpl(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r0_4.Read(uInstr);
            if ((iReg & 8) != 0)
                return false;
            dasm.ops.Add(new RegisterOperand(Registers.GpRegisters[iReg]));
            return true;
        }


        /// <summary>
        /// 8-bit register encoded in low 4 bits.
        /// </summary>
        private static bool rbl(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r0_4.Read(uInstr);
            dasm.ops.Add(new RegisterOperand(Registers.Gp8Registers[iReg]));
            return true;
        }

        /// <summary>
        /// 16-bit register encoded in low 4 bits.
        /// </summary>
        private static bool rl(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r0_4.Read(uInstr);
            if (iReg >= dasm.regs.Length)
                return false;
            dasm.ops.Add(new RegisterOperand(dasm.regs[iReg]));
            return true;
        }

        /// <summary>
        /// 16-bit register encoded in bits [4-7].
        /// </summary>
        private static bool rh(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r4_4.Read(uInstr);
            if (iReg >= dasm.regs.Length)
                return false;
            dasm.ops.Add(new RegisterOperand(dasm.regs[iReg]));
            return true;
        }

        private static bool rh3(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r4_3.Read(uInstr);
            if (iReg >= dasm.regs.Length)
                return false;
            dasm.ops.Add(new RegisterOperand(dasm.regs[iReg]));
            return true;
        }

        private static bool rl3(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r0_3.Read(uInstr);
            if (iReg >= dasm.regs.Length)
                return false;
            dasm.ops.Add(new RegisterOperand(dasm.regs[iReg]));
            return true;
        }
        /// <summary>
        /// 16-bit register encoded in 4 bits at bit position 8
        /// </summary>
        private static bool r8(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r8_4.Read(uInstr);
            if (iReg >= dasm.regs.Length)
                return false;
            dasm.ops.Add(new RegisterOperand(dasm.regs[iReg]));
            return true;
        }
        private static readonly Bitfield r0_3 = new Bitfield(0, 3);
        private static readonly Bitfield r0_4 = new Bitfield(0, 4);
        private static readonly Bitfield r4_3 = new Bitfield(4, 3);
        private static readonly Bitfield r4_4 = new Bitfield(4, 4);
        private static readonly Bitfield r8_4 = new Bitfield(8, 4);

        /// <summary>
        /// 8-bit register encoded in 4 bits at bit position 0
        /// </summary>
        private static bool rb0(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r0_4.Read(uInstr);
            dasm.ops.Add(new RegisterOperand(Registers.Gp8Registers[iReg]));
            return true;
        }

        /// <summary>
        /// 8-bit register encoded in 4 bits at bit position 8
        /// </summary>
        private static bool rb8(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r8_4.Read(uInstr);
            dasm.ops.Add(new RegisterOperand(Registers.Gp8Registers[iReg]));
            return true;
        }

        private static bool ccr(uint _, H8Disassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(Registers.CcRegister));
            return true;
        }

        /// <summary>
        /// Immediate 8-bit number encoded in low 8 bits.
        /// </summary>
        private static bool I8(uint uInstr, H8Disassembler dasm)
        {
            var imm = (byte) uInstr;
            dasm.ops.Add(ImmediateOperand.Byte(imm));
            return true;
        }

        /// <summary>
        /// Immediate 16-bit number encoded in next 16 bits.
        /// </summary>
        private static bool I16(uint _, H8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out var imm))
                return false;
            dasm.ops.Add(ImmediateOperand.Word16(imm));
            return true;
        }

        private static Mutator<H8Disassembler> Imm32(int n)
        {
            return (u, d) =>
            {
                d.ops.Add(ImmediateOperand.Word32(n));
                return true;
            };
        }

        private static bool Bit(uint uInstr, H8Disassembler dasm)
        {
            var imm = bfImm.Read(uInstr);
            dasm.ops.Add(ImmediateOperand.Byte((byte)imm));
            return true;
        }
        private static readonly Bitfield bfImm = new Bitfield(4, 3);

        /// <summary>
        /// Memory access with base register.
        /// </summary>
        private static bool Mind(uint uInstr, H8Disassembler dasm)
        {
            var iBaseReg = hi3.Read(uInstr);
            dasm.ops.Add(MemoryOperand.BaseOffset(dasm.dataSize!, 0, PrimitiveType.Ptr16, Registers.GpRegisters[iBaseReg]));
            return true;
        }

        /// <summary>
        /// Memory access with base register and 16-bit displacement; displacement follows opcode.
        /// </summary>
        private static bool Md16(uint uInstr, H8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeInt16(out short offset))
                return false;
            var iBaseReg = hi3.Read(uInstr);
            dasm.ops.Add(MemoryOperand.BaseOffset(dasm.dataSize!, offset, PrimitiveType.Ptr16, Registers.GpRegisters[iBaseReg]));
            return true;
        }

        private static bool PreEh(uint uInstr, H8Disassembler dasm)
        {
            var iReg = hi3.Read(uInstr);
            dasm.ops.Add(MemoryOperand.Pre(dasm.dataSize!, Registers.GpRegisters[iReg]));
            return true;
        }



        private static bool PostEh(uint uInstr, H8Disassembler dasm)
        {
            var iReg = hi3.Read(uInstr);
            dasm.ops.Add(MemoryOperand.Post(dasm.dataSize!, Registers.GpRegisters[iReg]));
            return true;
        }

        /// <summary>
        /// 8 bit signed displacement from the low byte of the instruction.
        /// </summary>
        private static bool disp8(uint uInstr, H8Disassembler dasm)
        {
            var disp = (sbyte) uInstr;
            if ((disp & 1) != 0)
                return false;       // Branch destination must be even.
            var addrTarget = dasm.rdr.Address + disp;
            dasm.ops.Add(AddressOperand.Create(addrTarget));
            return true;
        }

        /// <summary>
        /// 8-bit absolute memory access, stored in the low 8 bits of this instruction.
        /// </summary>
        private static bool aa8(uint uInstr, H8Disassembler dasm)
        {
            uint uAddr = uInstr & 0xFF;
            dasm.ops.Add(MemoryOperand.Abs(dasm.dataSize!, uAddr, PrimitiveType.Byte));
            return true;
        }

        /// <summary>
        /// Deferred 8-bit absolute memory access, stored in the low 8 bits of this instruction.
        /// </summary>
        private static bool Def_aa8(uint uInstr, H8Disassembler dasm)
        {
            uint uAddr = uInstr & 0xFF;
            var mop = MemoryOperand.Abs(dasm.dataSize!, uAddr, PrimitiveType.Byte);
            mop.Deferred = true;
            dasm.ops.Add(mop);
            return true;
        }

        /// <summary>
        /// 16-bit absolute memory access, stored in the next 16 bits.
        /// </summary>
        private static bool aa16(uint uInstr, H8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort uAddr))
                return false;
            dasm.ops.Add(MemoryOperand.Abs(dasm.dataSize!, uAddr, PrimitiveType.Ptr16));
            return true;
        }

        /// <summary>
        /// 24-bit absolute memory access, stored in the next 24 bits.
        /// </summary>
        private static bool aa24(uint uInstr, H8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort uHigh16))
                return false;
            if (!dasm.rdr.TryReadByte(out byte uLow8))
                return false;
            uint uAddr = (uint) uHigh16 << 8;
            uAddr |= uLow8;
            dasm.ops.Add(MemoryOperand.Abs(dasm.dataSize!, uAddr, dasm.arch.Ptr24));
            return true;
        }

        /// <summary>
        /// 24-bit absolute memory access, stored in the low 8 bits of the instruction
        /// and the next 16 bits.
        /// </summary>
        private static bool aa24_16(uint uInstr, H8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort uLow16))
                return false;
            uint uAddr = ((uInstr & 0xFF) << 16) | uLow16;
            dasm.ops.Add(MemoryOperand.Abs(dasm.dataSize!, uAddr, dasm.arch.Ptr24));
            return true;
        }

        private static readonly Bitfield hi3 = new Bitfield(4, 3);

        #endregion


        #region Decoders

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<H8Disassembler>[] mutators)
        {
            return new InstrDecoder<H8Disassembler, Mnemonic, H8Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<H8Disassembler>[] mutators)
        {
            return new InstrDecoder<H8Disassembler, Mnemonic, H8Instruction>(iclass, mnemonic, mutators);
        }


        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<H8Disassembler, Mnemonic, H8Instruction>(message);
        }
        #endregion

        static H8Disassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            var mov_b_aa8_ld = Instr(Mnemonic.mov, b, aa8, r8);
            var mov_b_aa8_st = Instr(Mnemonic.mov, b, r8, aa8);
            var add_b_imm = Instr(Mnemonic.add, b, I8, rb8);
            var and_b_imm = Instr(Mnemonic.and, b, I8, rb8);
            var addx_b_imm = Instr(Mnemonic.addx, b, I8, rb8);
            var cmp_b_imm = Instr(Mnemonic.cmp, b, I8, rb8);
            var mov_b_imm = Instr(Mnemonic.mov, b, I8, rb8);
            var or_b_imm = Instr(Mnemonic.or, b, I8, rb8);
            var subx_b_imm = Instr(Mnemonic.subx, b, I8, rb8);
            var xor_b_imm = Instr(Mnemonic.xor, b, I8, rb8);
            var add_l = Instr(Mnemonic.add, rh3, rl3);

            rootDecoder = Mask(8, 8, "H8", new Decoder<H8Disassembler, Mnemonic, H8Instruction>[256] {
                Select((0, 8), u => u == 0, " 00",
                    Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding|InstrClass.Zero),
                    invalid),
                Sparse(4, 4, "  01", invalid,
                    (0x0, Nyi("mov")),
                    (0x4, Nyi("ldc/stc")),
                    (0x6, Nyi("mac")),
                    (0x8, Nyi("sleep")),
                    (0xA, Nyi("clrmac")),
                    (0xC, Select((0, 4), u => u == 0, "  01 C0",
                        Nyi("01 C0"),
                        invalid)),
                    (0xD, Select((0, 4), u => u == 0, "  01 D0",
                        Nyi("01 D0"),
                        invalid)),
                    (0xE, Nyi("tas")),
                    (0xF, Nyi("Table 2-6"))),
                Select((4, 4), u => u == 0,
                    Instr(Mnemonic.stc, b, ccr, rl),
                    invalid),
                Select((4, 4), u => u == 0,
                    Instr(Mnemonic.ldc, b, rl, ccr),
                    invalid),

                Instr(Mnemonic.orc, I8, ccr),
                Instr(Mnemonic.xorc, I8, ccr),
                Instr(Mnemonic.andc, I8, ccr),
                Instr(Mnemonic.ldc, b, I8, ccr),

                Instr(Mnemonic.add, b, rh,rl),
                Instr(Mnemonic.add, w, rh,rl),
                Sparse(4, 4, "  0A", invalid,
                    (0, Instr(Mnemonic.inc, b, rb0)),
                    (0x8, add_l),
                    (0x9, add_l),
                    (0xA, add_l),
                    (0xB, add_l),
                    (0xC, add_l),
                    (0xD, add_l),
                    (0xE, add_l),
                    (0xF, add_l)),
                Sparse(4, 4, "  0B", invalid,
                    (0, Instr(Mnemonic.adds, Imm32(1), rgpl)),
                    (8, Instr(Mnemonic.adds, Imm32(2), rgpl)),
                    (9, Instr(Mnemonic.adds, Imm32(3), rgpl))),

                Instr(Mnemonic.mov, b, rh,rl),
                Instr(Mnemonic.mov, w, rh,rl),
                Nyi("addx"),
                Nyi("2-5"),

                // 10
                Sparse(4, 4, "  10", invalid,
                    (0x0, Instr(Mnemonic.shll, b,rl)),
                    (0x1, Instr(Mnemonic.shll, w,rl)),
                    (0x3, Instr(Mnemonic.shll, l,rl)),
                    (0x8, Instr(Mnemonic.shal, b,rl)),
                    (0x9, Instr(Mnemonic.shal, w,rl)),
                    (0xB, Instr(Mnemonic.shal, l,rl))),
                Sparse(4, 4, "  11", invalid,
                    (0x0, Instr(Mnemonic.shlr, b,rl)),
                    (0x1, Instr(Mnemonic.shlr, w,rl)),
                    (0x3, Instr(Mnemonic.shlr, l,rl)),
                    (0x8, Instr(Mnemonic.shar, b,rl)),
                    (0x9, Instr(Mnemonic.shar, w,rl)),
                    (0xB, Instr(Mnemonic.shar, l,rl))),
                Sparse(4, 4, "  12", invalid,
                    (0x0, Instr(Mnemonic.rotxl, b,rl)),
                    (0x1, Instr(Mnemonic.rotxl, w,rl)),
                    (0x3, Instr(Mnemonic.rotxl, l,rl)),
                    (0x8, Instr(Mnemonic.rotl, b,rl)),
                    (0x9, Instr(Mnemonic.rotl, w,rl)),
                    (0xB, Instr(Mnemonic.rotl, l,rl))),
                Sparse(4, 4, "  13", invalid,
                    (0x0, Instr(Mnemonic.rotxr, b,rl)),
                    (0x1, Instr(Mnemonic.rotxr, w,rl)),
                    (0x3, Instr(Mnemonic.rotxr, l,rl)),
                    (0x8, Instr(Mnemonic.rotr, b,rl)),
                    (0x9, Instr(Mnemonic.rotr, w,rl)),
                    (0xB, Instr(Mnemonic.rotr, l,rl))),

                Instr(Mnemonic.or, b, rh,rl),
                Instr(Mnemonic.xor, b, rh,rl),
                Instr(Mnemonic.and, b, rh,rl),
                Mask(4, 4, "  17",
                    Instr(Mnemonic.not, b, rl),
                    Instr(Mnemonic.not, w, rl),
                    invalid,
                    Instr(Mnemonic.not, l, rl),

                    invalid,
                    Instr(Mnemonic.extu, w, rl),
                    invalid,
                    Instr(Mnemonic.extu, l, rl),

                    Instr(Mnemonic.neg, b, rbl),
                    Instr(Mnemonic.neg, w, rl),
                    invalid,
                    Instr(Mnemonic.neg, l, rl3),

                    invalid,
                    Instr(Mnemonic.exts, w, rl),
                    invalid,
                    Instr(Mnemonic.exts, l, rl)),

                Instr(Mnemonic.sub, b, rh, rl),
                Instr(Mnemonic.sub, w, rh, rl),
                Sparse(4, 4, "  1A", invalid,
                    (0, Instr(Mnemonic.dec, b, rl)),
                    (0x8, Instr(Mnemonic.sub, l, rh, rl)),
                    (0x9, Instr(Mnemonic.sub, l, rh, rl)),
                    (0xA, Instr(Mnemonic.sub, l, rh, rl)),
                    (0xB, Instr(Mnemonic.sub, l, rh, rl)),
                    (0xC, Instr(Mnemonic.sub, l, rh, rl)),
                    (0xD, Instr(Mnemonic.sub, l, rh, rl)),
                    (0xE, Instr(Mnemonic.sub, l, rh, rl)),
                    (0xF, Instr(Mnemonic.sub, l, rh, rl))),
                Sparse(4, 4, "  1B", invalid,
                    (0, Instr(Mnemonic.subs, Imm32(1), rgpl)),
                    (8, Instr(Mnemonic.subs, Imm32(2), rgpl)),
                    (9, Instr(Mnemonic.subs, Imm32(3), rgpl))),

                Instr(Mnemonic.cmp, b, rh,rl),
                Instr(Mnemonic.cmp, w, rh,rl),
                Instr(Mnemonic.subx, b, rh,rl),
                Sparse(4, 4, "  1F", invalid,
                    (0, Nyi("das")),
                    (0x8, Instr(Mnemonic.cmp, l, rh3, rl3)),
                    (0x9, Instr(Mnemonic.cmp, l, rh3, rl3)),
                    (0xA, Instr(Mnemonic.cmp, l, rh3, rl3)),
                    (0xB, Instr(Mnemonic.cmp, l, rh3, rl3)),
                    (0xC, Instr(Mnemonic.cmp, l, rh3, rl3)),
                    (0xD, Instr(Mnemonic.cmp, l, rh3, rl3)),
                    (0xE, Instr(Mnemonic.cmp, l, rh3, rl3)),
                    (0xF, Instr(Mnemonic.cmp, l, rh3, rl3))),

                // 20
                mov_b_aa8_ld,
                mov_b_aa8_ld,
                mov_b_aa8_ld,
                mov_b_aa8_ld,

                mov_b_aa8_ld,
                mov_b_aa8_ld,
                mov_b_aa8_ld,
                mov_b_aa8_ld,

                mov_b_aa8_ld,
                mov_b_aa8_ld,
                mov_b_aa8_ld,
                mov_b_aa8_ld,

                mov_b_aa8_ld,
                mov_b_aa8_ld,
                mov_b_aa8_ld,
                mov_b_aa8_ld,

                // 30
                mov_b_aa8_st,
                mov_b_aa8_st,
                mov_b_aa8_st,
                mov_b_aa8_st,

                mov_b_aa8_st,
                mov_b_aa8_st,
                mov_b_aa8_st,
                mov_b_aa8_st,

                mov_b_aa8_st,
                mov_b_aa8_st,
                mov_b_aa8_st,
                mov_b_aa8_st,

                mov_b_aa8_st,
                mov_b_aa8_st,
                mov_b_aa8_st,
                mov_b_aa8_st,

                // 40
                Instr(Mnemonic.bra, InstrClass.Transfer, disp8),
                Instr(Mnemonic.brn, InstrClass.Linear|InstrClass.Padding, disp8),
                Instr(Mnemonic.bhi, InstrClass.ConditionalTransfer, disp8),
                Instr(Mnemonic.bls, InstrClass.ConditionalTransfer, disp8),

                Instr(Mnemonic.bcc, InstrClass.ConditionalTransfer, disp8),
                Instr(Mnemonic.bcs, InstrClass.ConditionalTransfer, disp8),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, disp8),
                Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, disp8),

                Instr(Mnemonic.bvc, InstrClass.ConditionalTransfer, disp8),
                Instr(Mnemonic.bvs, InstrClass.ConditionalTransfer, disp8),
                Instr(Mnemonic.bpl, InstrClass.ConditionalTransfer, disp8),
                Instr(Mnemonic.bmi, InstrClass.ConditionalTransfer, disp8),

                Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, disp8),
                Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, disp8),
                Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer, disp8),
                Instr(Mnemonic.ble, InstrClass.ConditionalTransfer, disp8),

                // 50
                Instr(Mnemonic.mulxu, b, rh, rl),
                Nyi("divxu"),
                Instr(Mnemonic.mulxu, w, rh, rl),
                Nyi("divxu"),

                Select((0, 8), u => u == 0x70, "  54",
                    Instr(Mnemonic.rts, InstrClass.Transfer|InstrClass.Return),
                    invalid),
                Nyi("bsr"),
                Select((0, 8), u => u == 0x70, "  56",
                    Instr(Mnemonic.rte, InstrClass.Transfer|InstrClass.Return),
                    invalid),
                Nyi("trapa"),

                Nyi("2-5"),
                Instr(Mnemonic.jmp, InstrClass.Transfer, Mind),
                Instr(Mnemonic.jmp, InstrClass.Transfer, aa24_16),
                Instr(Mnemonic.jmp, InstrClass.Transfer, Def_aa8),

                Nyi("bsr"),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, Mind),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, aa24_16),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, Def_aa8),

                // 60 
                Nyi("bset"),
                Nyi("bnot"),
                Nyi("bclr"),
                Nyi("btst"),

                Instr(Mnemonic.or, w, rh,rl),
                Instr(Mnemonic.xor, w, rh,rl),
                Instr(Mnemonic.and, w, rh,rl),
                Mask(7, 1, "  67",
                    Instr(Mnemonic.bst, Bit, rbl),
                    Instr(Mnemonic.bist, Bit, rbl)),

                Mask(7, 1, "  68",
                    Instr(Mnemonic.mov, b, Mind, rl),
                    Instr(Mnemonic.mov, b, rl, Mind)),
                Mask(7, 1, "  69",
                    Instr(Mnemonic.mov, w, Mind, rl),
                    Instr(Mnemonic.mov, w, rl, Mind)),
                Sparse(4, 4, "  6A", invalid,
                    (0x0, Instr(Mnemonic.mov, b, aa16, rl)),
                    (0x2, Instr(Mnemonic.mov, b, aa24, rl)),
                    (0x8, Instr(Mnemonic.mov, b, rl, aa16)),
                    (0xA, Instr(Mnemonic.mov, b, rl, aa24))),
                Sparse(4, 4, "  6B", invalid,
                    (0x0, Instr(Mnemonic.mov, w, aa16, rl)),
                    (0x2, Instr(Mnemonic.mov, w, aa24, rl)),
                    (0x8, Instr(Mnemonic.mov, w, rl, aa16)),
                    (0xA, Instr(Mnemonic.mov, w, rl, aa24))),

                Mask(7, 1, "  6C",
                    Instr(Mnemonic.mov, b, PostEh, rl),
                    Instr(Mnemonic.mov, b, rl, PreEh)),
                Mask(7, 1, "  6D",
                    Instr(Mnemonic.mov, w, PostEh, rl),
                    Instr(Mnemonic.mov, w, rl, PreEh)),
                Mask(7, 1, "  6E",
                    Instr(Mnemonic.mov, b, Md16, rl),
                    Instr(Mnemonic.mov, b, rl, Md16)),
                Mask(7, 1, "  6F",
                    Instr(Mnemonic.mov, w, Md16, rl),
                    Instr(Mnemonic.mov, w, rl, Md16)),

                // 70
                Instr(Mnemonic.bset, Bit, rbl),
                Instr(Mnemonic.bnot, Bit, rbl),
                Instr(Mnemonic.bclr, Bit, rbl),
                Instr(Mnemonic.btst, Bit, rbl),

                Mask(7, 1, "  74",
                    Instr(Mnemonic.bor, Bit, rbl),
                    Instr(Mnemonic.bior, Bit, rbl)),
                Mask(7, 1, "  75",
                    Instr(Mnemonic.bxor, Bit, rbl),
                    Instr(Mnemonic.bixor, Bit, rbl)),
                Mask(7, 1, "  76",
                    Instr(Mnemonic.band, Bit, rbl),
                    Instr(Mnemonic.biand, Bit, rbl)),
                Mask(7, 1, "  77",
                    Instr(Mnemonic.bld, Bit, rbl),
                    Instr(Mnemonic.bild, Bit, rbl)),

                Nyi("mov"),
                Sparse(4, 4, "  79", Nyi("79"),
                    (0, Instr(Mnemonic.mov, w,I16, rl)),
                    (7, invalid)),
               Nyi("2-5"),
               Nyi("eepmov"),

                Nyi("2-6"),
                Nyi("2-6"),
                Nyi("2-6"),
                Nyi("2-6"),

                // 80
                add_b_imm,
                add_b_imm,
                add_b_imm,
                add_b_imm,

                add_b_imm,
                add_b_imm,
                add_b_imm,
                add_b_imm,

                add_b_imm,
                add_b_imm,
                add_b_imm,
                add_b_imm,

                add_b_imm,
                add_b_imm,
                add_b_imm,
                add_b_imm,

                // 90
                addx_b_imm,
                addx_b_imm,
                addx_b_imm,
                addx_b_imm,

                addx_b_imm,
                addx_b_imm,
                addx_b_imm,
                addx_b_imm,

                addx_b_imm,
                addx_b_imm,
                addx_b_imm,
                addx_b_imm,

                addx_b_imm,
                addx_b_imm,
                addx_b_imm,
                addx_b_imm,

                // A0
                cmp_b_imm,
                cmp_b_imm,
                cmp_b_imm,
                cmp_b_imm,

                cmp_b_imm,
                cmp_b_imm,
                cmp_b_imm,
                cmp_b_imm,

                cmp_b_imm,
                cmp_b_imm,
                cmp_b_imm,
                cmp_b_imm,

                cmp_b_imm,
                cmp_b_imm,
                cmp_b_imm,
                cmp_b_imm,

                // B0
                subx_b_imm,
                subx_b_imm,
                subx_b_imm,
                subx_b_imm,

                subx_b_imm,
                subx_b_imm,
                subx_b_imm,
                subx_b_imm,

                subx_b_imm,
                subx_b_imm,
                subx_b_imm,
                subx_b_imm,

                subx_b_imm,
                subx_b_imm,
                subx_b_imm,
                subx_b_imm,

                // C0
                or_b_imm,
                or_b_imm,
                or_b_imm,
                or_b_imm,

                or_b_imm,
                or_b_imm,
                or_b_imm,
                or_b_imm,

                or_b_imm,
                or_b_imm,
                or_b_imm,
                or_b_imm,

                or_b_imm,
                or_b_imm,
                or_b_imm,
                or_b_imm,

                // D0
                xor_b_imm,
                xor_b_imm,
                xor_b_imm,
                xor_b_imm,

                xor_b_imm,
                xor_b_imm,
                xor_b_imm,
                xor_b_imm,

                xor_b_imm,
                xor_b_imm,
                xor_b_imm,
                xor_b_imm,

                xor_b_imm,
                xor_b_imm,
                xor_b_imm,
                xor_b_imm,

                // E0
                and_b_imm,
                and_b_imm,
                and_b_imm,
                and_b_imm,

                and_b_imm,
                and_b_imm,
                and_b_imm,
                and_b_imm,

                and_b_imm,
                and_b_imm,
                and_b_imm,
                and_b_imm,

                and_b_imm,
                and_b_imm,
                and_b_imm,
                and_b_imm,

                // F0
                mov_b_imm,
                mov_b_imm,
                mov_b_imm,
                mov_b_imm,

                mov_b_imm,
                mov_b_imm,
                mov_b_imm,
                mov_b_imm,

                mov_b_imm,
                mov_b_imm,
                mov_b_imm,
                mov_b_imm,

                mov_b_imm,
                mov_b_imm,
                mov_b_imm,
                mov_b_imm
            }); ;
        }
    }
}