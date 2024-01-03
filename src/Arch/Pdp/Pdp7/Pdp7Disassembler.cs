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

using Reko.Arch.Pdp.Memory;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Pdp.Pdp7
{
    using Decoder = Decoder<Pdp7Disassembler, Mnemonic, Pdp7Instruction>;
    using InstrDecoder = InstrDecoder<Pdp7Disassembler, Mnemonic, Pdp7Instruction>;
    using Mutator = Reko.Core.Machine.Mutator<Pdp7Disassembler>;

    public class Pdp7Disassembler : DisassemblerBase<Pdp7Instruction, Mnemonic>
    {
        private static Decoder rootDecoder;

        private readonly Pdp7Architecture arch;
        private readonly Word18BeImageReader rdr;
        private readonly List<MachineOperand> ops;

        public Pdp7Disassembler(Pdp7Architecture arch, Word18BeImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override Pdp7Instruction? DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadBeUInt18(out uint uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            if (uInstr == 0)
            {
                instr.InstructionClass |= InstrClass.Zero;
            }
            instr.Address = addr;
            instr.Length = 1;
            ops.Clear();
            return instr;
        }

        public override Pdp7Instruction CreateInvalidInstruction()
        {
            throw new NotImplementedException();
        }

        public override Pdp7Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Pdp7Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override Pdp7Instruction NotYetImplemented(string message)
        {
            throw new NotImplementedException();
        }

        public static InstrDecoder Instr(Mnemonic mnemonic, params Mutator[] mutators)
        {
            return new InstrDecoder(Core.InstrClass.Linear, mnemonic, mutators);
        }

        public static InstrDecoder Instr(Mnemonic mnemonic, InstrClass iclass,  params Mutator[] mutators)
        {
            return new InstrDecoder(iclass, mnemonic, mutators);
        }

        private static bool Y(uint uInstr, Pdp7Disassembler dasm)
        {
            var mem = new MemoryOperand(PdpTypes.Word18, uInstr & 0x1FFF);
            dasm.ops.Add(mem);
            return true;
        }

        public static Decoder Nyi(string message)
        {
            return new NyiDecoder<Pdp7Disassembler, Mnemonic, Pdp7Instruction>("PDP-7");
        }

        internal static Bitfield BeField(int bitPos, int bitLength)
        {
            return new Bitfield(18 - (bitPos + bitLength), bitLength);
        }

        static Pdp7Disassembler()
        {
            var operateDecoder = Nyi("  Operate decoder");
            rootDecoder = Mask(new[] { BeField(0, 4) }, "PDP-7",
                Instr(Mnemonic.cal, InstrClass.Transfer | InstrClass.Call),
                Instr(Mnemonic.dac, Y),
                Instr(Mnemonic.jms, InstrClass.Transfer | InstrClass.Call, Y),
                Instr(Mnemonic.dzm, Y),

                Instr(Mnemonic.lac, Y),
                Instr(Mnemonic.xor, Y),
                Instr(Mnemonic.add, Y),
                Instr(Mnemonic.tad, Y),

                Instr(Mnemonic.xct, Y),
                Instr(Mnemonic.isz, Y),
                Instr(Mnemonic.and, Y),
                Instr(Mnemonic.sad, Y),

                Instr(Mnemonic.jmp, InstrClass.Transfer, Y),
                Nyi("64"),
                Nyi("70"),
                operateDecoder);
        }
    }
}
