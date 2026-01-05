#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reko.Arch.Zilog.Z8000
{
    public class Z8000Disassembler : DisassemblerBase<Z8000Instruction, Mnemonic>
    {
        private static readonly Decoder<Z8000Disassembler, Mnemonic, Z8000Instruction> rootDecoder;

        private readonly Z8000Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr = default!;

        public Z8000Disassembler(Z8000Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override Z8000Instruction CreateInvalidInstruction()
        {
            return new Z8000Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        public override Z8000Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            var offset = rdr.Offset;
            if (!rdr.TryReadUInt16(out var uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            this.ops.Clear();
            instr.Address = addr;
            instr.Length = (int) (rdr.Offset - offset);
            return instr;
        }

        public override Z8000Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Z8000Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override Z8000Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Z80dis", this.addr, this.rdr, message);
            return new Z8000Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Nyi,
            };
        }

        private static Mutator<Z8000Disassembler> Register(int bitstart, int bitlength, RegisterStorage[] regs)
        {
            var field = new Bitfield(bitstart, bitlength);
            return (u, d) =>
            {
                var ireg = field.Read(u);
                var reg = regs[ireg];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<Z8000Disassembler> r0 = Register(0, 4, Registers.WordRegisters);
        private static readonly Mutator<Z8000Disassembler> rb0 = Register(0, 4, Registers.ByteRegisters);
        private static readonly Mutator<Z8000Disassembler> rr0 = Register(0, 4, Registers.LongRegisters);
        private static readonly Mutator<Z8000Disassembler> r4 = Register(4, 4, Registers.WordRegisters);
        private static readonly Mutator<Z8000Disassembler> rb4 = Register(4, 4, Registers.ByteRegisters);
        private static readonly Mutator<Z8000Disassembler> rr4 = Register(4, 4, Registers.LongRegisters);


        private static bool imb(uint uInstr, Z8000Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out var uImm))
                return false;
            dasm.ops.Add(Constant.Byte((byte)uImm));
            return true;
        }

        private static bool im(uint uInstr, Z8000Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out var uImm))
                return false;
            dasm.ops.Add(Constant.Word16(uImm));
            return true;
        }

        private static bool iml(uint uInstr, Z8000Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt32(out var uImm))
                return false;
            dasm.ops.Add(Constant.Word32(uImm));
            return true;
        }


        private static Decoder<Z8000Disassembler, Mnemonic, Z8000Instruction> ShiftByte(
            Decoder<Z8000Disassembler, Mnemonic, Z8000Instruction> inner)
        {
            return new NextInstructionPartDecoder(inner);
        }


        private class NextInstructionPartDecoder : Decoder<Z8000Disassembler, Mnemonic, Z8000Instruction>
        {
            private readonly Decoder<Z8000Disassembler, Mnemonic, Z8000Instruction> inner;

            public NextInstructionPartDecoder(Decoder<Z8000Disassembler, Mnemonic, Z8000Instruction> inner)
            {
                this.inner = inner;
            }

            public override Z8000Instruction Decode(uint wInstr, Z8000Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out var nextByte))
                    return dasm.CreateInvalidInstruction();
                uint uInstr = wInstr << 8 | nextByte;
                return inner.Decode(uInstr, dasm);
            }
        }

        private static Decoder<Z8000Disassembler, Mnemonic, Z8000Instruction> Instr(
            Mnemonic mnemonic,
            params Mutator<Z8000Disassembler>[] mutators)
        {
            return new InstrDecoder<Z8000Disassembler, Mnemonic, Z8000Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder<Z8000Disassembler,Mnemonic,Z8000Instruction> Instr(
            Mnemonic mnemonic,
            InstrClass iclass,
            params Mutator<Z8000Disassembler>[] mutators)
        {
            return new InstrDecoder<Z8000Disassembler, Mnemonic, Z8000Instruction>(iclass, mnemonic, mutators);
        }

        private static Decoder<Z8000Disassembler, Mnemonic, Z8000Instruction> Nyi(string msg)
        {
            return new NyiDecoder<Z8000Disassembler, Mnemonic, Z8000Instruction>(msg);
        }

        static Z8000Disassembler()
        {
            var nyi = Nyi("nyi");

            var decoder00 = Sparse(8, 6, "  00", nyi,
                (0b000000, Sparse(4, 4, "  000000", nyi,
                    (0b0000, Instr(Mnemonic.addb, rb0, imb)))),
                (0b000001, Sparse(4, 4, "  000001", nyi,
                    (0b0000, Instr(Mnemonic.add, r0, im)))),
                (0b010110, Sparse(4, 4, "  000001", nyi,
                    (0b0000, Instr(Mnemonic.addl, rr0, iml)))));

            var decoder10 = Sparse(8, 6, "  10", nyi,
                (0b000000, Instr(Mnemonic.addb, rb0, rb4)),
                (0b000001, Instr(Mnemonic.add, r0, r4)),
                (0b010110, Instr(Mnemonic.addl, rr0, rr4)),
                (0b110100, Instr(Mnemonic.adcb, rb0, rb4)),
                (0b110101, Instr(Mnemonic.adc, r0, r4)));

            rootDecoder = Mask(14, 2, "Z8000",
                decoder00,
                nyi,
                decoder10,
                nyi);
        }
    }
}
