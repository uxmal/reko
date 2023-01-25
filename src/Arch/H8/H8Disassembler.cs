#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System;
using System.Collections.Generic;

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
        private uint word;                  // used in some addressing modes

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
            word = 0;
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
            //$DEBUG
            return new H8Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Nyi
            };
            //return CreateInvalidInstruction();
        }

        #region Mutators

        /// <summary>
        /// Switch to using 8-bit registers.
        /// </summary>
        private static bool b(uint _, H8Disassembler dasm)
        {
            dasm.dataSize = PrimitiveType.Byte;
            dasm.regs = Registers.Gp8Registers;
            return true;
        }

        /// <summary>
        /// Switch to using 16-bit registers.
        /// </summary>
        private static bool w(uint _, H8Disassembler dasm)
        {
            dasm.dataSize = PrimitiveType.Word16;
            dasm.regs = Registers.Gp16Registers;
            return true;
        }

        /// <summary>
        /// Switch to using 32-bit registers.
        /// </summary>
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
            dasm.ops.Add(Registers.GpRegisters[iReg]);
            return true;
        }

        /// <summary>
        /// 8-bit register encoded in low 4 bits.
        /// </summary>
        private static bool rbl(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r0_4.Read(uInstr);
            dasm.ops.Add(Registers.Gp8Registers[iReg]);
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
            dasm.ops.Add(dasm.regs[iReg]);
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
            dasm.ops.Add(dasm.regs[iReg]);
            return true;
        }

        /// <summary>
        /// 8-bit register encoded in bits [4-7].
        /// </summary>
        private static bool rbh(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r4_4.Read(uInstr);
            if (iReg >= Registers.Gp8Registers.Length)
                return false;
            dasm.ops.Add(Registers.Gp8Registers[iReg]);
            return true;
        }

        private static bool rh3(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r4_3.Read(uInstr);
            if (iReg >= dasm.regs.Length)
                return false;
            dasm.ops.Add(dasm.regs[iReg]);
            return true;
        }

        private static bool rl3(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r0_3.Read(uInstr);
            if (iReg >= dasm.regs.Length)
                return false;
            dasm.ops.Add(dasm.regs[iReg]);
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
            dasm.ops.Add(dasm.regs[iReg]);
            return true;
        }
        private static readonly Bitfield r0_3 = new Bitfield(0, 3);
        private static readonly Bitfield r0_4 = new Bitfield(0, 4);
        private static readonly Bitfield r4_2 = new Bitfield(4, 2);
        private static readonly Bitfield r4_3 = new Bitfield(4, 3);
        private static readonly Bitfield r4_4 = new Bitfield(4, 4);
        private static readonly Bitfield r8_4 = new Bitfield(8, 4);
        private static readonly Bitfield r20_4 = new Bitfield(20, 4);

        /// <summary>
        /// 8-bit register encoded in 4 bits at bit position 0
        /// </summary>
        private static bool rb0(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r0_4.Read(uInstr);
            dasm.ops.Add(Registers.Gp8Registers[iReg]);
            return true;
        }

        /// <summary>
        /// 8-bit register encoded in 4 bits at bit position 4
        /// </summary>
        private static bool rb4(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r4_4.Read(uInstr);
            dasm.ops.Add(Registers.Gp8Registers[iReg]);
            return true;
        }

        /// <summary>
        /// 8-bit register encoded in 4 bits at bit position 8
        /// </summary>
        private static bool rb8(uint uInstr, H8Disassembler dasm)
        {
            var iReg = r8_4.Read(uInstr);
            dasm.ops.Add(Registers.Gp8Registers[iReg]);
            return true;
        }

        private static bool ccr(uint _, H8Disassembler dasm)
        {
            dasm.ops.Add(Registers.CcRegister);
            return true;
        }

        private static bool exr(uint _, H8Disassembler dasm)
        {
            dasm.ops.Add(Registers.ExrRegister);
            return true;
        }

        private static bool mach(uint _, H8Disassembler dasm)
        {
            dasm.ops.Add(Registers.Mach);
            return true;
        }

        private static bool macl(uint _, H8Disassembler dasm)
        {
            dasm.ops.Add(Registers.Macl);
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

        private static Mutator<H8Disassembler> Imm16(int n)
        {
            return (u, d) =>
            {
                d.ops.Add(ImmediateOperand.Word16((ushort)n));
                return true;
            };
        }

        /// <summary>
        /// Immediate 32-bit number encoded in next 32 bits.
        /// </summary>
        private static bool I32(uint _, H8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt32(out var imm))
                return false;
            dasm.ops.Add(ImmediateOperand.Word32(imm));
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

        /// <summary>
        /// Reads a bit position from the field [4-7).
        /// </summary>
        private static bool Bit(uint uInstr, H8Disassembler dasm)
        {
            var imm = bfImm.Read(uInstr);
            dasm.ops.Add(ImmediateOperand.Byte((byte)imm));
            return true;
        }
        private static readonly Bitfield bfImm = new Bitfield(4, 3);


        private static bool TrapNo(uint uInstr, H8Disassembler dasm)
        {
            var imm = (byte)r4_2.Read(uInstr);
            dasm.ops.Add(ImmediateOperand.Byte(imm));
            return true;
        }

        /// <summary>
        /// Memory access with base register.
        /// </summary>
        private static bool Mind(uint uInstr, H8Disassembler dasm)
        {
            var iBaseReg = hi3.Read(uInstr);
            dasm.ops.Add(MemoryOperand.BaseOffset(dasm.dataSize!, 0, PrimitiveType.Ptr16, Registers.GpRegisters[iBaseReg]));
            return true;
        }

        private static bool Mind_20(uint uInstr, H8Disassembler dasm)
        {
            var iBaseReg = bf20_3.Read(uInstr);
            dasm.ops.Add(MemoryOperand.BaseOffset(PrimitiveType.Byte, 0, PrimitiveType.Ptr16, Registers.GpRegisters[iBaseReg]));
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

        /// <summary>
        /// Memory access with base register and 32-bit displacement; displacement follows opcode.
        /// </summary>
        private static bool Md32_20(uint uInstr, H8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt32(out uint offset))
                return false;
            var iBaseReg = bf20_3.Read(uInstr);
            dasm.ops.Add(MemoryOperand.BaseOffset(dasm.dataSize!, (int)offset, PrimitiveType.Ptr32, Registers.GpRegisters[iBaseReg]));
            return true;
        }

        private static bool PreEh(uint uInstr, H8Disassembler dasm)
        {
            var iReg = hi3.Read(uInstr);
            dasm.ops.Add(MemoryOperand.Pre(dasm.dataSize!, Registers.GpRegisters[iReg]));
            return true;
        }

        private static bool PostEl(uint uInstr, H8Disassembler dasm)
        {
            var iReg = lo3.Read(uInstr);
            dasm.ops.Add(MemoryOperand.Post(dasm.dataSize!, Registers.GpRegisters[iReg]));
            return true;
        }

        private static bool PostEh(uint uInstr, H8Disassembler dasm)
        {
            var iReg = hi3.Read(uInstr);
            dasm.ops.Add(MemoryOperand.Post(dasm.dataSize!, Registers.GpRegisters[iReg]));
            return true;
        }

        private static bool SpPre(uint uInstr, H8Disassembler dasm)
        {
            dasm.ops.Add(MemoryOperand.Pre(dasm.dataSize!, Registers.GpRegisters[7]));
            return true;
        }

        private static bool SpPost(uint uInstr, H8Disassembler dasm)
        {
            dasm.ops.Add(MemoryOperand.Post(dasm.dataSize!, Registers.GpRegisters[7]));
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
        /// 16 bit signed displacement from the instruction following this one.
        /// </summary>
        private static bool disp16(uint uInstr, H8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeInt16(out short disp))
                return false;
            if ((disp & 1) != 0)
                return false;       // Branch destination must be even.
            var addrTarget = dasm.rdr.Address + disp;
            dasm.ops.Add(AddressOperand.Create(addrTarget));
            return true;
        }

        /// <summary>
        /// 8 bit signed displacement from the address of the call instruction.
        /// </summary>
        private static bool Cdisp8(uint uInstr, H8Disassembler dasm)
        {
            var disp = (sbyte) uInstr;
            if ((disp & 1) != 0)
                return false;       // Branch destination must be even.
            var addrTarget = dasm.addr + disp;
            dasm.ops.Add(AddressOperand.Create(addrTarget));
            return true;
        }

        /// <summary>
        /// 16 bit signed displacement from the address of the call instruction.
        /// </summary>
        private static bool Cdisp16(uint uInstr, H8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeInt16(out short disp))
                return false;
            if ((disp & 1) != 0)
                return false;       // Branch destination must be even.
            var addrTarget = dasm.addr + disp;
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
        /// 8-bit absolute memory access, stored starting at bit 16 of this instruction.
        /// </summary>
        private static bool aa8_16(uint uInstr, H8Disassembler dasm)
        {
            uint uAddr = (uInstr >> 16) & 0xFF;
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

        /// <summary>
        /// 32-bit absolute memory access, stored in the next 32 bits.
        /// </summary>
        private static bool aa32(uint uInstr, H8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt32(out uint uAddr))
                return false;
            dasm.ops.Add(MemoryOperand.Abs(dasm.dataSize!, uAddr, PrimitiveType.Ptr32));
            return true;
        }

        private static readonly Bitfield lo3 = new Bitfield(0, 3);
        private static readonly Bitfield hi3 = new Bitfield(4, 3);
        private static readonly Bitfield bf20_3 = new Bitfield(20, 3);

        /// <summary>
        /// Decode a 16-bit absolute memory access, stored in the 'word' variable.
        /// </summary>
        private static bool aa16_word(uint uInstr, H8Disassembler dasm)
        {
            dasm.ops.Add(MemoryOperand.Abs(dasm.dataSize!, dasm.word, PrimitiveType.Ptr16));
            return true;
        }

        /// <summary>
        /// Decode a 32-bit absolute memory access, stored in the 'word' variable.
        /// </summary>
        private static bool aa32_word(uint uInstr, H8Disassembler dasm)
        {
            dasm.ops.Add(MemoryOperand.Abs(dasm.dataSize!, dasm.word, PrimitiveType.Ptr32));
            return true;
        }

        /// <summary>
        /// Encodes a register list.
        /// </summary>
        private static Mutator<H8Disassembler> RegisterList(int nRegs, int nAdjustment)
        {
            return (u, d) =>
            {
                var iReg = (int)(u & 7) - nAdjustment;
                d.ops.Add(new RegisterListOperand(iReg, nRegs));
                return true;
            };
        }



        #endregion


        #region Decoders

        private class NextDecoder : Decoder<H8Disassembler, Mnemonic, H8Instruction>
        {
            private Decoder<H8Disassembler, Mnemonic, H8Instruction> subdecoder;

            public NextDecoder(Decoder subdecoder)
            {
                this.subdecoder = subdecoder;
            }

            public override H8Instruction Decode(uint wInstr, H8Disassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort uInstr))
                    return dasm.CreateInvalidInstruction();
                return subdecoder.Decode((wInstr<<16)|uInstr, dasm);
            }
        }

        private class Next16Decoder : Decoder<H8Disassembler, Mnemonic, H8Instruction>
        { 
            private readonly Decoder<H8Disassembler, Mnemonic, H8Instruction> subdecoder;

            public Next16Decoder(Decoder subdecoder)
            {
                this.subdecoder = subdecoder;
            }

            public override H8Instruction Decode(uint wInstr, H8Disassembler dasm)
            {
                if (!dasm.rdr.TryReadUInt16(out ushort u))
                    return dasm.CreateInvalidInstruction();
                if (!dasm.rdr.TryReadBeUInt16(out ushort uInstr))
                    return dasm.CreateInvalidInstruction();
                dasm.word = u;
                return subdecoder.Decode((wInstr << 16) | uInstr, dasm);
            }
        }

        private class Next32Decoder : Decoder<H8Disassembler, Mnemonic, H8Instruction>
        {
            private readonly Decoder<H8Disassembler, Mnemonic, H8Instruction> subdecoder;

            public Next32Decoder(Decoder subdecoder)
            {
                this.subdecoder = subdecoder;
            }

            public override H8Instruction Decode(uint wInstr, H8Disassembler dasm)
            {
                if (!dasm.rdr.TryReadUInt32(out uint u))
                    return dasm.CreateInvalidInstruction();
                if (!dasm.rdr.TryReadBeUInt16(out ushort uInstr))
                    return dasm.CreateInvalidInstruction();
                dasm.word = u;
                return subdecoder.Decode((wInstr << 16) | uInstr, dasm);
            }
        }


        private static Decoder Instr(Mnemonic mnemonic, params Mutator<H8Disassembler>[] mutators)
        {
            return new InstrDecoder<H8Disassembler, Mnemonic, H8Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<H8Disassembler>[] mutators)
        {
            return new InstrDecoder<H8Disassembler, Mnemonic, H8Instruction>(iclass, mnemonic, mutators);
        }

        private static Decoder Next(Decoder decoder)
        {
            return new NextDecoder(decoder);
        }

        /// <summary>
        /// Read a 16-bit word into the word buffer, then another opcode.
        /// </summary>
        private static Decoder Read16Next(Decoder decoder)
        {
            return new Next16Decoder(decoder);
        }

        /// <summary>
        /// Read a 32-bit word into the word buffer, then another opcode.
        /// </summary>
        private static Decoder Read32Next(Decoder decoder)
        {
            return new Next32Decoder(decoder);
        }

        private static Decoder Cond(
            int bitpos, int bitlength,
            Predicate<uint> pred,
            Decoder trueDecoder,
            Decoder falseDecoder)
        {
            var fields = new Bitfield[]
            {
                new Bitfield(bitpos, bitlength)
            };
            return new ConditionalDecoder<H8Disassembler, Mnemonic, H8Instruction>(
                fields, pred, "", trueDecoder, falseDecoder);
        }

        private static Decoder Cond(
            int bitpos, int bitlength, 
            Predicate<uint> pred,
            string tag,
            Decoder trueDecoder,
            Decoder falseDecoder)
        {
            var fields = new Bitfield[]
            {
                new Bitfield(bitpos, bitlength)
            };
            return new ConditionalDecoder<H8Disassembler, Mnemonic, H8Instruction>(
                fields, pred, tag, trueDecoder, falseDecoder);
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<H8Disassembler, Mnemonic, H8Instruction>(message);
        }
        #endregion

        private static bool Is0(uint u) => u == 0;
        private static bool IsMask8FZero(uint u) => (u & 0x8F) == 0;
        private static bool IsMask0FZero(uint u) => (u & 0x0F) == 0;

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
            var mov_l_reg_reg = Instr(Mnemonic.mov, l, rh3, rl3);
            var cmp_l_reg_reg = Instr(Mnemonic.cmp, l, rh3, rl3);

            var decoder010 = Next(Sparse(8, 8, "  010", invalid,
                (0x69, If(3, 1, Is0,
                    Cond(7, 1, Is0, "  mov.l indirect",
                        Instr(Mnemonic.mov, l, Mind, rl3),
                        Instr(Mnemonic.mov, l, rl3, Mind)))),
                (0x6F, If(3, 1, Is0,
                    Cond(7, 1, Is0, "  mov.l disp16",
                        Instr(Mnemonic.mov, l, Md16, rl3),
                        Instr(Mnemonic.mov, l, rl3, Md16)))),
                (0x78, Cond(0, 8, IsMask8FZero, "  mov.l disp32",
                    Next(Cond(4, 12, u => u == 0x6B2, "  mov.l disp32 second word",
                        Instr(Mnemonic.mov, l, Md32_20, rl3),
                        If(4, 12, u => u == 0x6BA,
                        Instr(Mnemonic.mov, l, rl3, Md32_20)))),
                    invalid)),
                (0x6D, Cond(3, 1, Is0, "  mov.l pre/post",
                    Cond(7, 1, Is0, "  mov.l pre/post",
                        Instr(Mnemonic.mov, l, PostEh, rl3),
                        Instr(Mnemonic.mov, l, rl3, PreEh)),
                    invalid)),
                (0x6B, Sparse(4, 4, "mov.l absoluteAddress", invalid,
                    (0x0, Instr(Mnemonic.mov, l, aa16, rl3)),
                    (0x2, Instr(Mnemonic.mov, l, aa32, rl3)),
                    (0x8, Instr(Mnemonic.mov, l, rl3, aa16)),
                    (0xA, Instr(Mnemonic.mov, l, rl3, aa32))))));

            var decoder011 = Select((0, 4), Is0,
                Next(Sparse(8, 8, "  0110", invalid,
                    (0x6D, Sparse(4, 4, invalid,
                        (0x7, Instr(Mnemonic.ldm, l, SpPost, RegisterList(2, 1))),      //$TODO: H8S/2000
                        (0xF, Instr(Mnemonic.stm, l, RegisterList(2, 0), SpPre)))))),   //$TODO: H8S/2000 
                invalid);

            var decoder012 = Select((0, 4), Is0,
                Next(Sparse(8, 8, "  0120", invalid,
                    (0x6D, Sparse(4, 4, invalid,
                        (0x7, Instr(Mnemonic.ldm, l, SpPost, RegisterList(3, 2))),      //$TODO: H8S/2000
                        (0xF, Instr(Mnemonic.stm, l, RegisterList(3, 0), SpPre)))))),   //$TODO: H8S/2000 
                invalid);

            var decoder013 = Select((0, 4), Is0,
                Next(Sparse(8, 8, "  0130", invalid,
                    (0x6D, Sparse(4, 4, invalid,
                        (0x7, Instr(Mnemonic.ldm, l, SpPost, RegisterList(4, 3))),      //$TODO: H8S/2000
                        (0xF, Instr(Mnemonic.stm, l, RegisterList(4, 0), SpPre)))))),   //$TODO: H8S/2000 
                invalid);

            var decoder014 = Sparse(0, 4, "  014", invalid,
                (0, Next(Sparse(8, 8, "  ldc/stc", invalid,
                    (0x69, Select((0, 8), IsMask0FZero, Mask(7, 1,
                        Instr(Mnemonic.ldc, w, Mind, ccr),
                        Instr(Mnemonic.stc, w, ccr, Mind)), invalid)),
                    (0x6B, Mask(7,1, 5, 1, "  01406B..",
                        Instr(Mnemonic.ldc, w, aa16, ccr),
                        Instr(Mnemonic.ldc, w, aa32, ccr),
                        Instr(Mnemonic.stc, w, ccr, aa16),
                        Instr(Mnemonic.stc, w, ccr, aa32))),
                    (0x6D, Select((0, 8), IsMask0FZero, Mask(7, 1,
                        Instr(Mnemonic.ldc, w, PostEh, ccr),
                        Instr(Mnemonic.stc, w, ccr, PreEh)), invalid)),
                    (0x78, Select((0, 8), IsMask0FZero, Next(Mask(7, 1,
                        Instr(Mnemonic.ldc, w, Md32_20, ccr),
                        Instr(Mnemonic.stc, w, ccr, Md32_20))), invalid)),
                    (0x6F, Select((0, 8), IsMask0FZero, Mask(7, 1,
                        Instr(Mnemonic.ldc, w, Md16, ccr),
                        Instr(Mnemonic.stc, w, ccr, Md16)), invalid))))),
                (1, Next(Sparse(8, 8, "  0141", invalid,
                    (0x04, Instr(Mnemonic.orc, I8, exr)),
                    (0x05, Instr(Mnemonic.xorc, I8, exr)),
                    (0x06, Instr(Mnemonic.andc, I8, exr)),
                    (0x07, Instr(Mnemonic.ldc, b, I8, exr)),
                    (0x69, Select((0, 8), IsMask0FZero, Mask(7, 1,
                        Instr(Mnemonic.ldc, w, Mind, exr),
                        Instr(Mnemonic.stc, w, exr, Mind)), invalid)),
                    (0x6B, Mask(7, 1, 5, 1, "  01416B..",
                        Instr(Mnemonic.ldc, w, aa16, exr),
                        Instr(Mnemonic.ldc, w, aa32, exr),
                        Instr(Mnemonic.stc, w, exr, aa16),
                        Instr(Mnemonic.stc, w, exr, aa32))),
                    (0x6D, Select((0, 8), IsMask0FZero, Mask(7, 1,
                        Instr(Mnemonic.ldc, w, PostEh, exr),
                        Instr(Mnemonic.stc, w, exr, PreEh)), invalid)),
                    (0x6F, Select((0, 8), IsMask0FZero, Mask(7, 1,
                        Instr(Mnemonic.ldc, w, Md16, exr),
                        Instr(Mnemonic.stc, w, exr, Md16)), invalid)),
                    (0x78, Select((0, 8), IsMask0FZero, Next(Mask(7, 1,
                        Instr(Mnemonic.ldc, w, Md32_20, exr),
                        Instr(Mnemonic.stc, w, exr, Md32_20))), invalid))))));

            var decoder01F = Select((0, 4), Is0, "  01F",
                Next(Select((12, 4), u => u == 6, "  next",
                    Sparse(8, 4, "  01F06", invalid,
                        (4, Instr(Mnemonic.or, l, rh, rl)),
                        (5, Instr(Mnemonic.xor, l, rh, rl)),
                        (6, Instr(Mnemonic.and, l, rh, rl))),
                    invalid)),
                invalid);

            var mac = Next(Select((0, 20), u => (u & 0xFFF88) == 0x06D00,
                Instr(Mnemonic.mac, PostEh, PostEl),    //$TODO: H8S/2600
                invalid));
            var tas = Next(Select(u => (u & 0xFF8F) == 0x7B0C,
                Instr(Mnemonic.tas, Mind), invalid));

            var decoder01C0 = Next(Sparse(8, 8, invalid,
                (0x50, Instr(Mnemonic.mulxs, b, rh, w, rl, b)),
                (0x52, Instr(Mnemonic.mulxs, w, rh, l, rl, w))));

            var decoder01D0 = Next(Sparse(8, 8, invalid,
                (0x51, Instr(Mnemonic.divxs, b, rh, w, rl, b)),
                (0x53, Instr(Mnemonic.divxs, w, rh, l, rl, w))));

            var decoder0F = Sparse(4, 4, "  0F", invalid,
                (0x0, Instr(Mnemonic.daa, rbl)),
                (0x8, mov_l_reg_reg),
                (0x9, mov_l_reg_reg),
                (0xA, mov_l_reg_reg),
                (0xB, mov_l_reg_reg),

                (0xC, mov_l_reg_reg),
                (0xD, mov_l_reg_reg),
                (0xE, mov_l_reg_reg),
                (0xF, mov_l_reg_reg));

            var decoder58 = If(0, 4, Is0, Mask(4, 4, "  058",
                Instr(Mnemonic.bra, InstrClass.Transfer, disp16),
                Instr(Mnemonic.brn, InstrClass.Linear | InstrClass.Padding, disp16),
                Instr(Mnemonic.bhi, InstrClass.ConditionalTransfer, disp16),
                Instr(Mnemonic.bls, InstrClass.ConditionalTransfer, disp16),

                Instr(Mnemonic.bcc, InstrClass.ConditionalTransfer, disp16),
                Instr(Mnemonic.bcs, InstrClass.ConditionalTransfer, disp16),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, disp16),
                Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, disp16),

                Instr(Mnemonic.bvc, InstrClass.ConditionalTransfer, disp16),
                Instr(Mnemonic.bvs, InstrClass.ConditionalTransfer, disp16),
                Instr(Mnemonic.bpl, InstrClass.ConditionalTransfer, disp16),
                Instr(Mnemonic.bmi, InstrClass.ConditionalTransfer, disp16),

                Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, disp16),
                Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, disp16),
                Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer, disp16),
                Instr(Mnemonic.ble, InstrClass.ConditionalTransfer, disp16)));

            var decoder6A1 = Sparse(0, 4, "  6A1x", invalid,
                (0, Read16Next(Sparse(8, 8, "  6A10", invalid,
                    (0x63, Instr(Mnemonic.btst, rbh, aa16_word)),
                    (0x73, Instr(Mnemonic.btst, Bit, aa16_word)),
                    (0x74, Mask(7, 1, "  6A10....74..",
                        Instr(Mnemonic.bor, Bit, aa16_word),
                        Instr(Mnemonic.bior, Bit, aa16_word))),
                    (0x75, Mask(7, 1, "  6A10....75..",
                        Instr(Mnemonic.bxor, Bit, aa16_word),
                        Instr(Mnemonic.bixor, Bit, aa16_word))),
                    (0x76, Mask(7, 1, "  6A10....76..",
                        Instr(Mnemonic.band, Bit, aa16_word),
                        Instr(Mnemonic.biand, Bit, aa16_word))),
                    (0x77, Mask(7, 1, "  6A10....77..",
                        Instr(Mnemonic.bld, Bit, aa16_word),
                        Instr(Mnemonic.bild, Bit, aa16_word)))))),
                (8, Read16Next(Sparse(8, 8, "  6A18", invalid,
                    (0x60, Instr(Mnemonic.bset, rbh, aa16_word)),
                    (0x61, Instr(Mnemonic.bnot, rbh, aa16_word)),
                    (0x62, Instr(Mnemonic.bclr, rbh, aa16_word)),
                    (0x67, Mask(7, 1, "  6A18....67..",
                        Instr(Mnemonic.bst, Bit, aa16_word),
                        Instr(Mnemonic.bist, Bit, aa16_word))),
                    (0x70, Instr(Mnemonic.bset, Bit, aa16_word)),
                    (0x71, Instr(Mnemonic.bnot, Bit, aa16_word)),
                    (0x72, Instr(Mnemonic.bclr, Bit, aa16_word))))));

            var decoder6A3 = Sparse(0, 4, "  6A3x", invalid,
                (0, Read32Next(Sparse(8, 8, "  6A30", invalid,
                   (0x63, Instr(Mnemonic.btst, rbh, aa32_word)),
                   (0x73, Instr(Mnemonic.btst, Bit, aa32_word)),
                   (0x74, Mask(7, 1, "  6A30........74..",
                        Instr(Mnemonic.bor, Bit, aa32_word),
                        Instr(Mnemonic.bior, Bit, aa32_word))),
                   (0x75, Mask(7, 1, "  6A30........75..",
                        Instr(Mnemonic.bxor, Bit, aa32_word),
                        Instr(Mnemonic.bixor, Bit, aa32_word))),
                   (0x76, Mask(7, 1, "  6A30........76..",
                        Instr(Mnemonic.band, Bit, aa32_word),
                        Instr(Mnemonic.biand, Bit, aa32_word))),
                   (0x77, Mask(7, 1, "  6A30........77..",
                        Instr(Mnemonic.bld, Bit, aa32_word),
                        Instr(Mnemonic.bild, Bit, aa32_word)))))),
                (8, Read32Next(Sparse(8, 8, "  6A38", invalid,
                   (0x60, Instr(Mnemonic.bset, rbh, aa32_word)),
                   (0x61, Instr(Mnemonic.bnot, rbh, aa32_word)),
                   (0x62, Instr(Mnemonic.bclr, rbh, aa32_word)),
                   (0x67, Mask(7, 1, "  6A38....67..",
                        Instr(Mnemonic.bst, Bit, aa32_word),
                        Instr(Mnemonic.bist, Bit, aa32_word))),
                   (0x70, Instr(Mnemonic.bset, Bit, aa32_word)),
                   (0x71, Instr(Mnemonic.bnot, Bit, aa32_word)),
                   (0x72, Instr(Mnemonic.bclr, Bit, aa32_word))))));

            rootDecoder = Mask(8, 8, "H8", new Decoder<H8Disassembler, Mnemonic, H8Instruction>[256] {
                Select((0, 8), Is0, " 00",
                    Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding|InstrClass.Zero),
                    invalid),
                Sparse(4, 4, "  01", invalid,
                    (0x0, decoder010),
                    (0x1, decoder011),
                    (0x2, decoder012),
                    (0x3, decoder013),

                    (0x4, decoder014),
                    (0x6, mac),
                    (0x8, Select((0, 4), Is0, Instr(Mnemonic.sleep), invalid)),
                    (0xA, Select((0, 4), Is0, Instr(Mnemonic.clrmac), invalid)), //$TODO H8S/2600
                    (0xC, Select((0, 4), Is0, "  01 C0",
                        decoder01C0,
                        invalid)),
                    (0xD, Select((0, 4), Is0, "  01 D0",
                        decoder01D0,
                        invalid)),
                    (0xE, Select((0,4), Is0, tas, invalid)),    //$TODO: H8S/2000
                    (0xF, decoder01F)),
                Sparse(4, 4, "  02", invalid,
                    (0x0, Instr(Mnemonic.stc, b, ccr, rbl)),
                    (0x1, Instr(Mnemonic.stc, b, exr, rbl)),
                    (0x2, Instr(Mnemonic.stmac, mach, rgpl)),  //$TODO: H8S/2600
                    (0x3, Instr(Mnemonic.stmac, macl, rgpl)),  //$TODO: H8S/2600
                    (0x4, invalid)),
                Sparse(4, 4, "  03", invalid,
                    (0x0, Instr(Mnemonic.ldc, b, rbl, ccr)),
                    (0x1, Instr(Mnemonic.ldc, b, rbl, exr)),
                    (0x2, Instr(Mnemonic.ldmac, rgpl, mach)),  //$TODO: H8S/2600
                    (0x3, Instr(Mnemonic.ldmac, rgpl, macl))), //$TODO: H8S/2600

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
                    (0x0, Instr(Mnemonic.adds, Imm32(1), rgpl)),
                    (0x5, Instr(Mnemonic.inc, w, Imm16(1), rl)),
                    (0x7, Instr(Mnemonic.inc, l, Imm32(1), rl)),
                    (0x8, Instr(Mnemonic.adds, Imm32(2), rgpl)),
                    (0x9, Instr(Mnemonic.adds, Imm32(4), rgpl)),
                    (0xD, Instr(Mnemonic.inc, w, Imm16(2), rl)),
                    (0xF, Instr(Mnemonic.inc, l, Imm32(2), rl))),

                Instr(Mnemonic.mov, b, rh,rl),
                Instr(Mnemonic.mov, w, rh,rl),
                Instr(Mnemonic.addx, b, rbh,rbl),
                decoder0F,

                // 10
                Sparse(4, 4, "  10", invalid,
                    (0x0, Instr(Mnemonic.shll, b, rl)),
                    (0x1, Instr(Mnemonic.shll, w, rl)),
                    (0x3, Instr(Mnemonic.shll, l, rl)),
                    (0x4, Instr(Mnemonic.shll, b, Imm16(2), rl)),
                    (0x5, Instr(Mnemonic.shll, w, Imm16(2), rl)),
                    (0x7, Instr(Mnemonic.shll, l, Imm16(2), rl)),
                    (0x8, Instr(Mnemonic.shal, b, rl)),
                    (0x9, Instr(Mnemonic.shal, w, rl)),
                    (0xB, Instr(Mnemonic.shal, l, rl)),
                    (0xC, Instr(Mnemonic.shal, b, Imm16(2), rl)),
                    (0xD, Instr(Mnemonic.shal, w, Imm16(2), rl)),
                    (0xF, Instr(Mnemonic.shal, l, Imm16(2), rl))),
                Sparse(4, 4, "  11", invalid,
                    (0x0, Instr(Mnemonic.shlr, b,rl)),
                    (0x1, Instr(Mnemonic.shlr, w,rl)),
                    (0x3, Instr(Mnemonic.shlr, l,rl)),
                    (0x4, Instr(Mnemonic.shlr, b, Imm16(2), rl)),
                    (0x5, Instr(Mnemonic.shlr, w, Imm16(2), rl)),
                    (0x7, Instr(Mnemonic.shlr, l, Imm16(2), rl)),
                    (0x8, Instr(Mnemonic.shar, b,rl)),
                    (0x9, Instr(Mnemonic.shar, w,rl)),
                    (0xB, Instr(Mnemonic.shar, l,rl)),
                    (0xC, Instr(Mnemonic.shar, b, Imm16(2), rl)),
                    (0xD, Instr(Mnemonic.shar, w, Imm16(2), rl)),
                    (0xF, Instr(Mnemonic.shar, l, Imm16(2), rl))),
                Sparse(4, 4, "  12", invalid,
                    (0x0, Instr(Mnemonic.rotxl, b, rl)),
                    (0x1, Instr(Mnemonic.rotxl, w, rl)),
                    (0x3, Instr(Mnemonic.rotxl, l, rl)),
                    (0x4, Instr(Mnemonic.rotxl, b, Imm16(2), rl)),
                    (0x5, Instr(Mnemonic.rotxl, w, Imm16(2), rl)),
                    (0x7, Instr(Mnemonic.rotxl, l, Imm16(2), rl)),
                    (0x8, Instr(Mnemonic.rotl, b, rl)),
                    (0x9, Instr(Mnemonic.rotl, w, rl)),
                    (0xB, Instr(Mnemonic.rotl, l, rl)),
                    (0xC, Instr(Mnemonic.rotl, b, Imm16(2), rl)),
                    (0xD, Instr(Mnemonic.rotl, w, Imm16(2), rl)),
                    (0xF, Instr(Mnemonic.rotl, l, Imm16(2), rl))),
                Sparse(4, 4, "  13", invalid,
                    (0x0, Instr(Mnemonic.rotxr, b,rl)),
                    (0x1, Instr(Mnemonic.rotxr, w,rl)),
                    (0x3, Instr(Mnemonic.rotxr, l,rl)),
                    (0x4, Instr(Mnemonic.rotxr, b, Imm16(2), rl)),
                    (0x5, Instr(Mnemonic.rotxr, w, Imm16(2), rl)),
                    (0x7, Instr(Mnemonic.rotxr, l, Imm16(2), rl)),
                    (0x8, Instr(Mnemonic.rotr, b,rl)),
                    (0x9, Instr(Mnemonic.rotr, w,rl)),
                    (0xB, Instr(Mnemonic.rotr, l,rl)),
                    (0xC, Instr(Mnemonic.rotr, b, Imm16(2), rl)),
                    (0xD, Instr(Mnemonic.rotr, w, Imm16(2), rl)),
                    (0xF, Instr(Mnemonic.rotr, l, Imm16(2), rl))),

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
                    Instr(Mnemonic.exts, w, rl),    //$TODO: ext 300H
                    invalid,
                    Instr(Mnemonic.exts, l, rl)),   //$TODO: ext 300H

                Instr(Mnemonic.sub, b, rh, rl),
                Instr(Mnemonic.sub, w, rh, rl),
                Sparse(4, 4, "  1A", invalid,
                    (0, Instr(Mnemonic.dec, b, rl)),
                    (0x8, Instr(Mnemonic.sub, l, rh3, rl3)),
                    (0x9, Instr(Mnemonic.sub, l, rh3, rl3)),
                    (0xA, Instr(Mnemonic.sub, l, rh3, rl3)),
                    (0xB, Instr(Mnemonic.sub, l, rh3, rl3)),
                    (0xC, Instr(Mnemonic.sub, l, rh3, rl3)),
                    (0xD, Instr(Mnemonic.sub, l, rh3, rl3)),
                    (0xE, Instr(Mnemonic.sub, l, rh3, rl3)),
                    (0xF, Instr(Mnemonic.sub, l, rh3, rl3))),
                Sparse(4, 4, "  1B", invalid,
                    (0x0, Instr(Mnemonic.subs, Imm32(1), rgpl)),
                    (0x5, Instr(Mnemonic.dec, w, Imm16(1), rl)),
                    (0x7, Instr(Mnemonic.dec, l, Imm32(1), rl)),
                    (0x8, Instr(Mnemonic.subs, Imm32(2), rgpl)),
                    (0x9, Instr(Mnemonic.subs, Imm32(4), rgpl)),
                    (0xD, Instr(Mnemonic.dec, w, Imm16(2), rl)),
                    (0xF, Instr(Mnemonic.dec, l, Imm32(2), rl))),

                Instr(Mnemonic.cmp, b, rh,rl),
                Instr(Mnemonic.cmp, w, rh,rl),
                Instr(Mnemonic.subx, b, rh,rl),
                Sparse(4, 4, "  1F", invalid,
                    (0x0, Instr(Mnemonic.das, rbl)),
                    (0x8, cmp_l_reg_reg),
                    (0x9, cmp_l_reg_reg),
                    (0xA, cmp_l_reg_reg),
                    (0xB, cmp_l_reg_reg),
                    (0xC, cmp_l_reg_reg),
                    (0xD, cmp_l_reg_reg),
                    (0xE, cmp_l_reg_reg),
                    (0xF, cmp_l_reg_reg)),

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
                Instr(Mnemonic.divxu, b, rh, w, rl, b),
                Instr(Mnemonic.mulxu, w, rh, rl),
                Instr(Mnemonic.divxu, w, rh, l, rl, w),

                If(0, 8, u => u == 0x70, Instr(Mnemonic.rts, InstrClass.Transfer|InstrClass.Return)),
                Instr(Mnemonic.bsr, InstrClass.Transfer|InstrClass.Call, Cdisp8),
                If(0, 8, u => u == 0x70, Instr(Mnemonic.rte, InstrClass.Transfer|InstrClass.Return)),
                If(u => (u & 0xC) == 0, Instr(Mnemonic.trapa, TrapNo)),

                decoder58,
                Instr(Mnemonic.jmp, InstrClass.Transfer, Mind),
                Instr(Mnemonic.jmp, InstrClass.Transfer, aa24_16),
                Instr(Mnemonic.jmp, InstrClass.Transfer, Def_aa8),

                If(0, 8, Is0, Instr(Mnemonic.bsr, InstrClass.Transfer|InstrClass.Call, Cdisp16)),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, Mind),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, aa24_16),
                Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, Def_aa8),

                // 60 
                Instr(Mnemonic.bset, rbh, rbl),
                Instr(Mnemonic.bnot, rbh, rbl),
                Instr(Mnemonic.bclr, rbh, rbl),
                Instr(Mnemonic.btst, rbh, rbl),

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
                    (0x1, decoder6A1),
                    (0x2, Instr(Mnemonic.mov, b, aa32, rl)),
                    (0x3, decoder6A3),
                    (0x4, Instr(Mnemonic.movfpe, aa16, rbl)),
                    (0x8, Instr(Mnemonic.mov, b, rl, aa16)),
                    (0xA, Instr(Mnemonic.mov, b, rl, aa32)),
                    (0xC, Instr(Mnemonic.movtpe, rbl, aa16))),
                Sparse(4, 4, "  6B", invalid,
                    (0x0, Instr(Mnemonic.mov, w, aa16, rl)),
                    (0x2, Instr(Mnemonic.mov, w, aa32, rl)),
                    (0x8, Instr(Mnemonic.mov, w, rl, aa16)),
                    (0xA, Instr(Mnemonic.mov, w, rl, aa32))),

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

                Next(Select((4, 16), u => u == 0x06A2, Instr(Mnemonic.mov, b, Md32_20, rl),
                     Select((4, 16), u => u == 0x06AA, Instr(Mnemonic.mov, b, rl, Md32_20),
                     Select((4, 16), u => u == 0x06B2, Instr(Mnemonic.mov, w, Md32_20, rl),
                     Select((4, 16), u => u == 0x06BA, Instr(Mnemonic.mov, b, rl, Md32_20),
                     invalid))))),
                Sparse(4, 4, "  79", invalid,
                    (0, Instr(Mnemonic.mov, w,I16, rl)),
                    (1, Instr(Mnemonic.add, w,I16, rl)),
                    (2, Instr(Mnemonic.cmp, w,I16, rl)),
                    (3, Instr(Mnemonic.sub, w,I16, rl)),
                    (4, Instr(Mnemonic.or,  w,I16, rl)),
                    (5, Instr(Mnemonic.xor, w,I16, rl)),
                    (6, Instr(Mnemonic.and, w,I16, rl))),
                Sparse(4, 4, "  7A", invalid,
                    (0, Instr(Mnemonic.mov, l,I32, rl)),
                    (1, Instr(Mnemonic.add, l,I32, rl)),
                    (2, Instr(Mnemonic.cmp, l,I32, rl)),
                    (3, Instr(Mnemonic.sub, l,I32, rl)),
                    (4, Instr(Mnemonic.or,  l,I32, rl)),
                    (5, Instr(Mnemonic.xor, l,I32, rl)),
                    (6, Instr(Mnemonic.and, l,I32, rl))),
                Next(Select(u => u == 0x7B5C598F, Instr(Mnemonic.eepmov, b),
                     If(u => u == 0x7BD4598F, Instr(Mnemonic.eepmov, b)))),

                Next(Sparse(8, 8, "  7C", invalid,
                     (0x63, Instr(Mnemonic.btst, rbh, Mind_20)),
                     (0x73, Instr(Mnemonic.btst, Bit, Mind_20)),
                     (0x74, Mask(7, 1, "  7Cr074",
                        Instr(Mnemonic.bor, Bit, Mind_20),
                        Instr(Mnemonic.bior, Bit, Mind_20))),
                     (0x75, Mask(7, 1, "  7Cr075",
                        Instr(Mnemonic.bxor, Bit, Mind_20),
                        Instr(Mnemonic.bixor, Bit, Mind_20))),
                     (0x76, Mask(7, 1, "  7Cr076",
                        Instr(Mnemonic.band, Bit, Mind_20),
                        Instr(Mnemonic.biand, Bit, Mind_20))),
                     (0x77, Mask(7, 1, "  7Cr077",
                        Instr(Mnemonic.bld, Bit, Mind_20),
                        Instr(Mnemonic.bild, Bit, Mind_20))))),
                Next(Sparse(8, 8, "  7D", invalid,
                     (0x60, Instr(Mnemonic.bset, rbh, Mind_20)),
                     (0x61, Instr(Mnemonic.bnot, rbh, Mind_20)),
                     (0x62, Instr(Mnemonic.bclr, rbh, Mind_20)),
                     (0x67, Mask(7, 1, "   7Dr067",
                            Instr(Mnemonic.bst, Bit, Mind_20),
                            Instr(Mnemonic.bist, Bit, Mind_20))),
                     (0x70, Instr(Mnemonic.bset, Bit, Mind_20)),
                     (0x71, Instr(Mnemonic.bnot, Bit, Mind_20)),
                     (0x72, Instr(Mnemonic.bclr, Bit, Mind_20)))),
                Next(Sparse(8, 8, "  7E", invalid,
                     (0x63, Instr(Mnemonic.btst, rbh, aa8_16)),
                     (0x73, Instr(Mnemonic.btst, Bit, aa8_16)),
                     (0x74, Mask(7, 1, "  7E..74..",
                        Instr(Mnemonic.bor, Bit, aa8_16),
                        Instr(Mnemonic.bior, Bit, aa8_16))),
                     (0x75, Mask(7, 1, "  7E..75..",
                        Instr(Mnemonic.bxor, Bit, aa8_16),
                        Instr(Mnemonic.bixor, Bit, aa8_16))),
                     (0x76, Mask(7, 1, "  7E..76..",
                        Instr(Mnemonic.band, Bit, aa8_16),
                        Instr(Mnemonic.biand, Bit, aa8_16))),
                     (0x77, Mask(7, 1, "  7E..77..",
                        Instr(Mnemonic.bild, Bit, aa8_16),
                        Instr(Mnemonic.bild, Bit, aa8_16))))),
                Next(Sparse(8, 8, "  7F", invalid,
                     (0x60, Instr(Mnemonic.bset, rb4, aa8_16)),
                     (0x61, Instr(Mnemonic.bnot, rb4, aa8_16)),
                     (0x62, Instr(Mnemonic.bclr, rb4, aa8_16)),
                     (0x67, Mask(7, 1, "   7F..67..",
                            Instr(Mnemonic.bst, Bit, aa8_16),
                            Instr(Mnemonic.bist, Bit, aa8_16))),
                     (0x70, Instr(Mnemonic.bset, Bit, aa8_16)),
                     (0x71, Instr(Mnemonic.bnot, Bit, aa8_16)),
                     (0x72, Instr(Mnemonic.bclr, Bit, aa8_16)))),

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
                Select(u => u == 0xFFFF,
                    Instr(Mnemonic.mov, InstrClass.Linear|InstrClass.Padding, b, I8, rb8),
                    mov_b_imm)
            });
        }
    }
}