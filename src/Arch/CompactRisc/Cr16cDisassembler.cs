#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

namespace Reko.Arch.CompactRisc
{
    using Decoder = Decoder<Cr16cDisassembler, Mnemonic, Cr16Instruction>;

    public class Cr16cDisassembler : DisassemblerBase<Cr16Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly Cr16Architecture arch;
        private readonly EndianImageReader rdr;
        private List<MachineOperand> ops;
        private Address addr;

        public Cr16cDisassembler(Cr16Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = rdr.Address;
        }

        public override Cr16Instruction? DisassembleInstruction()
        {
            if (!rdr.TryReadLeUInt16(out ushort uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            var addrNew = rdr.Address;
            instr.Address = this.addr;
            instr.Length = (int) (addrNew - this.addr);
            this.addr = addrNew;
            this.ops.Clear();
            return instr;
        }

        public override Cr16Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Cr16Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override Cr16Instruction CreateInvalidInstruction()
        {
            return new Cr16Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = MachineInstruction.NoOperands,
            };
        }

        public override Cr16Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Cr16Dasm", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        #region Mutator

        private static Mutator<Cr16cDisassembler> Imm(int bitpos)
        {
            var immField = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var imm = (ushort) immField.Read(u);
                if (imm == 9)
                {
                    imm = 0xFFFF;
                }
                else if (imm == 11)
                {
                    if (!d.rdr.TryReadLeUInt16(out imm))
                        return false;
                }
                d.ops.Add(ImmediateOperand.Word16(imm));
                return true;
            };
        }

        private static readonly Mutator<Cr16cDisassembler> Imm4 = Imm(4);

        private static Mutator<Cr16cDisassembler> Imm20(int bitpos)
        {
            var immField = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var high = immField.Read(u);
                if (!d.rdr.TryReadLeUInt16(out var low))
                    return false;
                var imm = (high << 16) | low;
                d.ops.Add(new ImmediateOperand(Constant.Create(Cr16Architecture.Word24, imm)));
                return true;
            };
        }
        private static readonly Mutator<Cr16cDisassembler> Imm20_0 = Imm20(0);

        private static Mutator<Cr16cDisassembler> Reg(int bitpos)
        {
            var regfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var ireg = regfield.Read(u);
                d.ops.Add(new RegisterOperand(Registers.GpRegisters[ireg]));
                return true;
            };
        }

        private static readonly Mutator<Cr16cDisassembler> R0 = Reg(0);
        private static readonly Mutator<Cr16cDisassembler> R4 = Reg(4);

        #endregion


        #region Decoder

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<Cr16cDisassembler, Mnemonic, Cr16Instruction>(message);
        }

        #endregion

        static Cr16cDisassembler()
        {
            var decode0 = Mask(8, 4,
                Nyi("0x00"),
                Nyi("0x01"),
                Nyi("0x02"),
                Nyi("0x03"),
                Nyi("0x04"),
                Instr(Mnemonic.movd, Imm20_0, R4),
                Nyi("0x06"),
                Nyi("0x07"),
                Nyi("0x08"),
                Nyi("0x09"),
                Nyi("0x0A"),
                Nyi("0x0B"),
                Nyi("0x0C"),
                Nyi("0x0D"),
                Nyi("0x0E"),
                Nyi("0x0F"));

            var decode5 = Mask(8, 4,
                Nyi("0x50"),
                Nyi("0x51"),
                Nyi("0x52"),
                Nyi("0x53"),
                Nyi("0x54"),
                Nyi("0x55"),
                Nyi("0x56"),
                Nyi("0x57"),
                Instr(Mnemonic.movb, Imm4, R0),
                Instr(Mnemonic.movb, R4, R0),
                Nyi("0x5A"),
                Nyi("0x5B"),
                Nyi("0x5C"),
                Nyi("0x5D"),
                Nyi("0x5E"),
                Nyi("0x5F"));

            rootDecoder = Mask(12, 4, "CR16C",
                decode0,
                Nyi("0x1"),
                Nyi("0x2"),
                Nyi("0x3"),

                Nyi("0x4"),
                decode5,
                Nyi("0x6"),
                Nyi("0x7"),

                Nyi("0x8"),
                Nyi("0x9"),
                Nyi("0xA"),
                Nyi("0xB"),

                Nyi("0xC"),
                Nyi("0xD"),
                Nyi("0xE"),
                Nyi("0xF"));
        }
    }
}