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
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Altera.Nios2
{
    using Decoder = Reko.Core.Machine.Decoder<Nios2Disassembler, Mnemonic, Nios2Instruction>;

    public class Nios2Disassembler : DisassemblerBase<Nios2Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly Nios2Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public Nios2Disassembler(Nios2Architecture arch, EndianImageReader imageReader)
        {
            this.arch = arch;
            this.rdr = imageReader;
            this.ops = new List<MachineOperand>();
            this.addr = default!;
        }

        public override Nios2Instruction? DisassembleInstruction()
        {
            addr = rdr.Address;
            if (!rdr.TryReadLeUInt32(out uint uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            ops.Clear();
            return instr;
        }

        public override Nios2Instruction CreateInvalidInstruction()
        {
            return new Nios2Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        public override Nios2Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Nios2Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray(),
            };
        }

        public override Nios2Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Nios2Dasm", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        private static readonly Bitfield i_regA= new Bitfield(27, 5);
        private static readonly Bitfield i_regB = new Bitfield(22, 5);
        private static readonly Bitfield i_regC = new Bitfield(17, 5);
        private static readonly Bitfield i_imm = new Bitfield(6, 16);
        
        private static bool I(uint uInstr, Nios2Disassembler dasm)
        {
            var regA = Registers.GpRegisters[i_regA.Read(uInstr)];
            var regB = Registers.GpRegisters[i_regB.Read(uInstr)];
            var imm = i_imm.Read(uInstr);
            dasm.ops.Add(new RegisterOperand(regB));
            dasm.ops.Add(new RegisterOperand(regA));
            dasm.ops.Add(ImmediateOperand.UInt32(imm));
            return true;
        }

        private static bool Is(uint uInstr, Nios2Disassembler dasm)
        {
            var regA = Registers.GpRegisters[i_regA.Read(uInstr)];
            var regB = Registers.GpRegisters[i_regB.Read(uInstr)];
            var imm = i_imm.ReadSigned(uInstr);
            dasm.ops.Add(new RegisterOperand(regB));
            dasm.ops.Add(new RegisterOperand(regA));
            dasm.ops.Add(ImmediateOperand.Int32(imm));
            return true;
        }

        private static Mutator<Nios2Disassembler> Im(PrimitiveType dt)
        {
            return (uInstr, dasm) =>
            {
                var regA = Registers.GpRegisters[i_regA.Read(uInstr)];
                var regB = Registers.GpRegisters[i_regB.Read(uInstr)];
                var imm = i_imm.ReadSigned(uInstr);
                dasm.ops.Add(new RegisterOperand(regB));
                dasm.ops.Add(new MemoryOperand(dt)
                {
                    Base = regA,
                    Offset = imm
                });
                return true;
            };
        }
        private static readonly Mutator<Nios2Disassembler> Im_8 = Im(PrimitiveType.Byte);
        private static readonly Mutator<Nios2Disassembler> Im_16 = Im(PrimitiveType.Word16);
        private static readonly Mutator<Nios2Disassembler> Im_32 = Im(PrimitiveType.Word16);
        private static readonly Mutator<Nios2Disassembler> Im_i8 = Im(PrimitiveType.Int8);
        private static readonly Mutator<Nios2Disassembler> Im_i16 = Im(PrimitiveType.Int16);

        private static bool Br(uint uInstr, Nios2Disassembler dasm)
        {
            var regA = Registers.GpRegisters[i_regA.Read(uInstr)];
            var regB = Registers.GpRegisters[i_regB.Read(uInstr)];
            var offset = i_imm.ReadSigned(uInstr);
            dasm.ops.Add(new RegisterOperand(regA));
            dasm.ops.Add(new RegisterOperand(regB));
            dasm.ops.Add(AddressOperand.Create(dasm.rdr.Address + offset));
            return true;
        }

        private static Bitfield j_offset = new Bitfield(6, 26);

        private static bool J(uint uInstr, Nios2Disassembler dasm)
        {
            var regA = Registers.GpRegisters[i_regA.Read(uInstr)];
            var regB = Registers.GpRegisters[i_regB.Read(uInstr)];
            var offset = j_offset.ReadSigned(uInstr) * 4;
            dasm.ops.Add(AddressOperand.Create(dasm.rdr.Address + offset));
            return true;
        }

        private static bool R(uint uInstr, Nios2Disassembler dasm)
        {
            var regA = Registers.GpRegisters[i_regA.Read(uInstr)];
            var regB = Registers.GpRegisters[i_regB.Read(uInstr)];
            var regC = Registers.GpRegisters[i_regC.Read(uInstr)];
            dasm.ops.Add(new RegisterOperand(regC));
            dasm.ops.Add(new RegisterOperand(regA));
            dasm.ops.Add(new RegisterOperand(regB));
            return true;
        }

        private static bool Ra(uint uInstr, Nios2Disassembler dasm)
        {
            var regA = Registers.GpRegisters[i_regA.Read(uInstr)];
            dasm.ops.Add(new RegisterOperand(regA));
            return true;
        }

        private static bool Rc(uint uInstr, Nios2Disassembler dasm)
        {
            var regC = Registers.GpRegisters[i_regC.Read(uInstr)];
            dasm.ops.Add(new RegisterOperand(regC));
            return true;
        }

        private static readonly Bitfield sh_amt = new Bitfield(6, 5);

        private static bool Rsh(uint uInstr, Nios2Disassembler dasm)
        {
            var regA = Registers.GpRegisters[i_regA.Read(uInstr)];
            var immSh = (int)sh_amt.Read(uInstr);
            var regC = Registers.GpRegisters[i_regC.Read(uInstr)];
            dasm.ops.Add(new RegisterOperand(regC));
            dasm.ops.Add(new RegisterOperand(regA));
            dasm.ops.Add(ImmediateOperand.Int32(immSh));
            return true;
        }

        private static bool Rca(uint uInstr, Nios2Disassembler dasm)
        {
            var regA = Registers.GpRegisters[i_regA.Read(uInstr)];
            var regC = Registers.GpRegisters[i_regC.Read(uInstr)];
            dasm.ops.Add(new RegisterOperand(regC));
            dasm.ops.Add(new RegisterOperand(regA));
            return true;
        }


        private static bool Rna(uint uInstr, Nios2Disassembler dasm)
        {
            var regA = Registers.GpRegisters[i_regA.Read(uInstr)];
            var immN = (int) sh_amt.Read(uInstr);
            dasm.ops.Add(ImmediateOperand.Int32(immN));
            dasm.ops.Add(new RegisterOperand(regA));
            return true;
        }



        private static Decoder Instr(Mnemonic mnemonic, params Mutator<Nios2Disassembler>[] mutators)
        {
            return new InstrDecoder<Nios2Disassembler, Mnemonic, Nios2Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Nios2Disassembler>[] mutators)
        {
            return new InstrDecoder<Nios2Disassembler, Mnemonic, Nios2Instruction>(iclass, mnemonic, mutators);
        }

        private static Decoder Nyi(Mnemonic mnemonic)
        {
            return new NyiDecoder<Nios2Disassembler, Mnemonic, Nios2Instruction>(mnemonic.ToString());
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<Nios2Disassembler, Mnemonic, Nios2Instruction>(message);
        }

        static Nios2Disassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            var rType = Sparse(11, 6, "  R-type", invalid,
                (0x01, Nyi(Mnemonic.eret)),
                (0x02, Instr(Mnemonic.roli, Rsh)),
                (0x03, Instr(Mnemonic.rol, R)),
                (0x04, Nyi(Mnemonic.flushp)),
                (0x05, Instr(Mnemonic.ret, InstrClass.Transfer)),
                (0x06, Instr(Mnemonic.nor, R)),
                (0x07, Instr(Mnemonic.mulxuu, R)),
                (0x08, Instr(Mnemonic.cmpge, R)),
                (0x09, Nyi(Mnemonic.bret)),
                (0x0B, Instr(Mnemonic.ror, R)),
                (0x0C, Nyi(Mnemonic.flushi)),
                (0x0D, Instr(Mnemonic.jmp, InstrClass.Transfer, Ra)),
                (0x0E, Instr(Mnemonic.and, R)),

                (0x10, Nyi(Mnemonic.cmplt)),
                (0x12, Nyi(Mnemonic.slli)),
                (0x13, Instr(Mnemonic.sll, R)),
                (0x14, Instr(Mnemonic.wrprs, InstrClass.Linear|InstrClass.System, Rca)),
                (0x16, Instr(Mnemonic.or, R)),
                (0x17, Instr(Mnemonic.mulxsu, R)),
                (0x18, Instr(Mnemonic.cmpne, R)),
                (0x1A, Instr(Mnemonic.srli, Rsh)),
                (0x1B, Instr(Mnemonic.srl, R)),
                (0x1C, Instr(Mnemonic.nextpc, Rc)),
                (0x1D, Instr(Mnemonic.callr, InstrClass.Transfer|InstrClass.Call, Ra)),
                (0x1E, Instr(Mnemonic.xor, R)),
                (0x1F, Instr(Mnemonic.mulxss, R)),

                (0x20, Instr(Mnemonic.cmpeq, R)),
                (0x24, Instr(Mnemonic.divu, R)),
                (0x25, Instr(Mnemonic.div, R)),
                (0x26, Nyi(Mnemonic.rdctl)),
                (0x27, Instr(Mnemonic.mul, R)),
                (0x28, Instr(Mnemonic.cmpgeu, R)),
                (0x29, Nyi(Mnemonic.initi)),
                (0x2D, Nyi(Mnemonic.trap)),
                (0x2E, Instr(Mnemonic.wrctl, InstrClass.Linear|InstrClass.System, Rna)),

                (0x30, Instr(Mnemonic.cmpltu, R)),
                (0x31, Instr(Mnemonic.add, R)),
                (0x34, Nyi(Mnemonic.@break)),
                (0x36, Instr(Mnemonic.sync)),
                (0x39, Instr(Mnemonic.sub, R)),
                (0x3A, Instr(Mnemonic.srai, Rsh)),
                (0x3B, Instr(Mnemonic.sra, R)));

            rootDecoder = Mask(0, 6, "Nios2", new Decoder[64] {
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, J),
                Instr(Mnemonic.jmpi, InstrClass.Transfer, J),
                invalid,
                Instr(Mnemonic.ldbu, Im_16),
                Instr(Mnemonic.addi, Is),
                Instr(Mnemonic.stb, Im_8),
                Instr(Mnemonic.br, InstrClass.Transfer, Br),
                Instr(Mnemonic.ldb, Im_i8),

                Instr(Mnemonic.cmpgei, Is),
                invalid,
                invalid,
                Instr(Mnemonic.ldhu, Im_16),
                Instr(Mnemonic.andi, I),
                Instr(Mnemonic.sth, Im_16),
                Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, Br),
                Instr(Mnemonic.ldh, Im_i16),

                Instr(Mnemonic.cmplti, Is),
                invalid,
                invalid,
                Nyi(Mnemonic.initda),
                Instr(Mnemonic.ori, I),
                Instr(Mnemonic.stw, Im_32),
                Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, Br),
                Instr(Mnemonic.ldw, Im_32),

                Instr(Mnemonic.cmpnei, Is),
                invalid,
                invalid,
                /* 0x1B */ Nyi(Mnemonic.flushda),
                Instr(Mnemonic.xori, I),
                invalid,
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, Br),
                invalid,

                // 20
                Instr(Mnemonic.cmpeqi, Is),
                invalid,
                invalid,
                Instr(Mnemonic.ldbuio, Im_8),
                Instr(Mnemonic.muli, Is),
                Instr(Mnemonic.stbio, Im_8),
                Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, Br), 
                Instr(Mnemonic.ldbio, Im_i8),

                Instr(Mnemonic.cmpgeui, I),
                invalid,
                invalid,
                Instr(Mnemonic.ldhuio, Im_16),
                Instr(Mnemonic.andhi, I),
                Instr(Mnemonic.sthio, Im_16),
                Instr(Mnemonic.bgeu, InstrClass.ConditionalTransfer, Br),
                Instr(Mnemonic.ldhio, Im_i16),

                Instr(Mnemonic.cmpltui, I),
                invalid,
                /* 0x32 */ Nyi(Mnemonic.custom),
                /* 0x33 */ Nyi(Mnemonic.initd),
                Instr(Mnemonic.orhi, I),
                Instr(Mnemonic.stwio, Im_32),
                Instr(Mnemonic.bltu, InstrClass.ConditionalTransfer, Br),
                Instr(Mnemonic.ldwio, Im_32),

                /* 0x38 */ Nyi(Mnemonic.rdprs),
                invalid,
                rType,
                /* 0x3B */ Nyi(Mnemonic.flushd),
                Instr(Mnemonic.xorhi, I),
                invalid,
                invalid,
                invalid,
            });

        }
    }
}
