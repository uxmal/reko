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
using Reko.Core.Services;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;

namespace Reko.Arch.H8
{
    using Decoder = Decoder<H8Disassembler, Mnemonic, H8Instruction>;

    public class H8Disassembler : DisassemblerBase<H8Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly H8Architecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;

        public H8Disassembler(H8Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.addr = rdr.Address;
        }

        public override H8Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out ushort uInstr))
                return null;
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



        public override H8Instruction NotYetImplemented(uint wInstr, string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("H8Dis", this.addr, rdr, message);
            return CreateInvalidInstruction();
        }

        #region Decoders

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<H8Disassembler, Mnemonic, H8Instruction>(message);
        }
        #endregion

        static H8Disassembler()
        {
            rootDecoder = Mask(8, 8, "H8", new Decoder<H8Disassembler, Mnemonic, H8Instruction>[256] {
                Nyi("nop"),
                Nyi("table 2-5"),
               Nyi("stc"),
                Nyi("ldc"),

           Nyi("org"),
           Nyi("xorg"),
           Nyi("andc"),
           Nyi("ldc"),

           Nyi("add"),
           Nyi("add"),
           Nyi("2-5"),
           Nyi("2-5"),

           Nyi("mov"),
           Nyi("mov"),
           Nyi("addx"),
           Nyi("2-5"),

                //10
           Nyi("2-5"),
           Nyi("2-5"),
           Nyi("2-5"),
           Nyi("2-5"),

           Nyi("or_b"),
           Nyi("xor_b"),
           Nyi("and_b"),
           Nyi("2-5"),

           Nyi("sub_b"),
           Nyi("sub_w"),
           Nyi("2-5"),
           Nyi("2-5"),

           Nyi("cmp"),
           Nyi("cmp"),
           Nyi("subx"),
           Nyi("2-5"),

                // 20
           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),

           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),

           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),

           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),

                // 30
           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),

           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),

           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),

           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),
           Nyi("mov_b"),

                // 40
           Nyi("bra"),
           Nyi("brn"),
           Nyi("bhi"),
           Nyi("bls"),

           Nyi("bcc"),
           Nyi("bcs"),
           Nyi("bne"),
           Nyi("beq"),

           Nyi("bvc"),
           Nyi("bvs"),
           Nyi("bpl"),
           Nyi("bmi"),

           Nyi("bge"),
           Nyi("blt"),
           Nyi("bgt"),
           Nyi("ble"),

                // 50
           Nyi("mulxu"),
           Nyi("divxu"),
           Nyi("nmulxu"),
           Nyi("divxu"),

           Nyi("rts"),
           Nyi("bsr"),
           Nyi("rte"),
           Nyi("trapa"),

           Nyi("2-5"),
           Nyi("jmp"),
           Nyi("jmp"),
           Nyi("jmp"),

           Nyi("bsr"),
           Nyi("jsr"),
           Nyi("jsr"),
           Nyi("jsr"),

                // 60 
           Nyi("bset"),
           Nyi("bnot"),
           Nyi("bclr"),
           Nyi("btst"),

           Nyi("or_w"),
           Nyi("xor_w"),
           Nyi("and_w"),
           Nyi("bst/bist"),

           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov"),

           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov"),

                // 70
           Nyi("bset"),
           Nyi("bnot"),
           Nyi("bclr"),
           Nyi("btst"),

           Nyi("bor/bior"),
           Nyi("bxor/bixor"),
           Nyi("band/biand"),
           Nyi("bld/bild"),

           Nyi("mov"),
           Nyi("2-5"),
           Nyi("2-5"),
           Nyi("eepmov"),

           Nyi("2-6"),
           Nyi("2-6"),
           Nyi("2-6"),
           Nyi("2-6"),

                // 80
           Nyi("add"),
           Nyi("add"),
           Nyi("add"),
           Nyi("add"),

           Nyi("add"),
           Nyi("add"),
           Nyi("add"),
           Nyi("add"),

           Nyi("add"),
           Nyi("add"),
           Nyi("add"),
           Nyi("add"),

           Nyi("add"),
           Nyi("add"),
           Nyi("add"),
           Nyi("add"),

                // 90
           Nyi("addx"),
           Nyi("addx"),
           Nyi("addx"),
           Nyi("addx"),

           Nyi("addx"),
           Nyi("addx"),
           Nyi("addx"),
           Nyi("addx"),

           Nyi("addx"),
           Nyi("addx"),
           Nyi("addx"),
           Nyi("addx"),

           Nyi("addx"),
           Nyi("addx"),
           Nyi("addx"),
           Nyi("addx"),

                // A0
           Nyi("cmp"),
           Nyi("cmp"),
           Nyi("cmp"),
           Nyi("cmp"),

           Nyi("cmp"),
           Nyi("cmp"),
           Nyi("cmp"),
           Nyi("cmp"),

           Nyi("cmp"),
           Nyi("cmp"),
           Nyi("cmp"),
           Nyi("cmp"),

           Nyi("cmp"),
           Nyi("cmp"),
           Nyi("cmp"),
           Nyi("cmp"),

                // B0
           Nyi("subx"),
           Nyi("subx"),
           Nyi("subx"),
           Nyi("subx"),

           Nyi("subx"),
           Nyi("subx"),
           Nyi("subx"),
           Nyi("subx"),

           Nyi("subx"),
           Nyi("subx"),
           Nyi("subx"),
           Nyi("subx"),

           Nyi("subx"),
           Nyi("subx"),
           Nyi("subx"),
           Nyi("subx"),

                // C0
           Nyi("or"),
           Nyi("or"),
           Nyi("or"),
           Nyi("or"),

           Nyi("or"),
           Nyi("or"),
           Nyi("or"),
           Nyi("or"),

           Nyi("or"),
           Nyi("or"),
           Nyi("or"),
           Nyi("or"),

           Nyi("or"),
           Nyi("or"),
           Nyi("or"),
           Nyi("or"),

                // D0
           Nyi("xor"),
           Nyi("xor"),
           Nyi("xor"),
           Nyi("xor"),

           Nyi("xor"),
           Nyi("xor"),
           Nyi("xor"),
           Nyi("xor"),

           Nyi("xor"),
           Nyi("xor"),
           Nyi("xor"),
           Nyi("xor"),

           Nyi("xor"),
           Nyi("xor"),
           Nyi("xor"),
           Nyi("xor"),

                // E0
           Nyi("and"),
           Nyi("and"),
           Nyi("and"),
           Nyi("and"),

           Nyi("and"),
           Nyi("and"),
           Nyi("and"),
           Nyi("and"),

           Nyi("and"),
           Nyi("and"),
           Nyi("and"),
           Nyi("and"),

           Nyi("and"),
           Nyi("and"),
           Nyi("and"),
           Nyi("and"),

                // F0
           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov"),

           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov"),

           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov"),

           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov"),
           Nyi("mov") });

        }
    }
}