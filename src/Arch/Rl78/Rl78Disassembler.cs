#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.Arch.Rl78
{
    public class Rl78Disassembler : DisassemblerBase<Rl78Instruction>
    {
        private static readonly Decoder[] s_decoders;

        private readonly Rl78Architecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;

        public Rl78Disassembler(Rl78Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override Rl78Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte op))
                return null;
            var instr = s_decoders[op].Decode(op, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        private void EmitUnitTest(string message)
        { 

        }

        private abstract class Decoder
        {
            public abstract Rl78Instruction Decode(uint uInstr, Rl78Disassembler dasm);
        }

        private class InstrDecoder : Decoder
        {
            private readonly InstructionClass iclass;
            private readonly Mnemonic mnem;
            private readonly Mutator<Rl78Disassembler>[] mutators;

            public InstrDecoder(InstructionClass iclass, Mnemonic mnem, Mutator<Rl78Disassembler>[] mutators)
            {
                this.iclass = iclass;
                this.mnem = mnem;
                this.mutators = mutators;
            }

            public override Rl78Instruction Decode(uint uInstr, Rl78Disassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        private class NyiDecoder : Decoder
        {
            private string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override Rl78Instruction Decode(uint uInstr, Rl78Disassembler dasm)
            {
                dasm.EmitUnitTest(message);
                return new Rl78Instruction
                {
                    IClass = InstructionClass.Invalid,
                    Mnemonic = Mnemonic.invalid,
                    Operands = new MachineOperand[0]
                };
            }
        }

        private static Decoder Instr(Mnemonic mnem, InstructionClass iclass, params Mutator<Rl78Disassembler> [] mutators)
        {
            return new InstrDecoder(iclass, mnem, mutators);
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }

        static Rl78Disassembler()
        {
            s_decoders = new Decoder[256]
            {
                // 0x00
                Instr(Mnemonic.nop, InstructionClass.Linear),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0x10
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0x20
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0x30
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0x40
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0x50
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0x60
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0x70
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0x80
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0x90
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0xA0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0xB0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0xC0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0xD0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0xE0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                // 0xF0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

            };
        }
    }
}
