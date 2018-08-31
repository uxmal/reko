#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.M6800.M6812
{
    using Mutator = Func<byte, M6812Disassembler, bool>;

    public class M6812Disassembler : DisassemblerBase<M6812Instruction>
    {
        private readonly static Decoder[] decoders;

        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> operands;

        public M6812Disassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
            this.operands = new List<MachineOperand>();
        }

        public override M6812Instruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadByte(out var bInstr))
                return null;
            M6812Instruction instr = decoders[bInstr].Decode(bInstr, this);
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        private M6812Instruction Invalid()
        {
            return new M6812Instruction
            {
                Opcode = Opcode.invalid,
                Operands = new MachineOperand[0]
            };
        }

        public abstract class Decoder
        {
            public abstract M6812Instruction Decode(byte bInstr, M6812Disassembler dasm);
        }

        public class InstrDecoder : Decoder
        {
            private readonly Opcode opcode;
            private readonly Mutator[] mutators;

            public InstrDecoder(Opcode opcode, params Mutator[] mutators)
            {
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public override M6812Instruction Decode(byte bInstr, M6812Disassembler dasm)
            {
                dasm.operands.Clear();
                foreach (var mutator in mutators)
                {
                    if (!mutator(bInstr, dasm))
                        return dasm.Invalid();
                }
                return new M6812Instruction
                {
                    Opcode = this.opcode,
                    Operands = dasm.operands.ToArray()
                };
            }
        }


        private static Decoder Instr(Opcode opcode, params Mutator [] mutators)
        {
            return new InstrDecoder(opcode, mutators);
        }

        private static Decoder Nyi(string message)
        {
            return new InstrDecoder(Opcode.invalid, NotYetImplemented);
        }

        private static HashSet<byte> seen = new HashSet<byte>();
        private static bool NotYetImplemented(byte bInstr, M6812Disassembler dasm)
        {
            if (!seen.Contains(bInstr))
            {
                seen.Add(bInstr);
                Debug.Print("// An M6812 decoder for instruction {0:X2} has not been implemented.", bInstr);
                Debug.Print("[Test]");
                Debug.Print("public void M6812Dis_{0:X2}()", bInstr);
                Debug.Print("{");
                Debug.Print("    Given_Code(\"{0:X2}\");", bInstr);
                Debug.Print("    Expect_Instruction(\"@@@\");");
                Debug.Print("}");
                Debug.WriteLine("");
            }
            return true;
        }

        static M6812Disassembler()
        {
            var invalid = Instr(Opcode.invalid);

            decoders = new Decoder[256]
            {
                // 00
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
                // 10
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
                // 20
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
                // 30
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
                Instr(Opcode.pshd),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 40
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
                // 50
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
                // 60
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
                // 70
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
                // 80
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
                // 90
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
                // A0
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
                // B0
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
                // C0
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
                // D0
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
                // E0
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
                // F0
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
