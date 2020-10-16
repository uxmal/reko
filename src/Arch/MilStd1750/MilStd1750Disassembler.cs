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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using System.Collections.Generic;

namespace Reko.Arch.MilStd1750
{
    public class MilStd1750Disassembler : DisassemblerBase<Instruction, Mnemonic>
    {
        private static readonly Decoder<MilStd1750Disassembler, Mnemonic, Instruction> rootDecoder;

        private readonly MilStd1750Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public MilStd1750Disassembler(MilStd1750Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = rdr.Address;
        }

        public override Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt16(out ushort wInstr))
                return null;
            this.ops.Clear();
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = this.addr;
            instr.Length = (int) (rdr.Address - this.addr);
            return instr;
        }

        public override Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override Instruction CreateInvalidInstruction()
        {
            return new Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
            };
        }

        public override Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("MS1750Dis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }


        private static bool Is0(uint u) => u == 0;

        private static InstrDecoder<MilStd1750Disassembler, Mnemonic,Instruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<MilStd1750Disassembler> [] mutators)
        {
            return new InstrDecoder<MilStd1750Disassembler, Mnemonic, Instruction>(iclass, mnemonic, mutators);
        }

        private static InstrDecoder<MilStd1750Disassembler, Mnemonic, Instruction> Instr(Mnemonic mnemonic, params Mutator<MilStd1750Disassembler>[] mutators)
        {
            return new InstrDecoder<MilStd1750Disassembler, Mnemonic, Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static NyiDecoder<MilStd1750Disassembler, Mnemonic, Instruction> Nyi(string message)
        {
            return new NyiDecoder<MilStd1750Disassembler, Mnemonic, Instruction>(message);
        }


        static MilStd1750Disassembler()
        {
            var invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);
            var nyi = Instr(Mnemonic.nyi, InstrClass.Invalid);

            rootDecoder = Mask(8, 8, "MIL-STD-1750", new Decoder<MilStd1750Disassembler, Mnemonic, Instruction>[256]
            {
                // 00
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // 10
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // 20
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // 30
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // 40
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // 50
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // 60
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // 70
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // 80
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // 90
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // A0
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // B0
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // C0
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // D0
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // E0
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                // F0
                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                nyi,

                nyi,
                nyi,
                nyi,
                Select((0, 8), Is0, "  0xFF",
                    Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                    Nyi("FF"))
            });
        }
    }
}