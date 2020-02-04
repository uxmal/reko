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
using Reko.Core.Lib;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    using Decoder = Reko.Core.Machine.Decoder<Mips16eDisassembler, Mnemonic, MipsInstruction>;

    public class Mips16eDisassembler : DisassemblerBase<MipsInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly MipsProcessorArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public Mips16eDisassembler(MipsProcessorArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override MipsInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            this.ops.Clear();
            if (!rdr.TryReadUInt16(out ushort wInstr))
                return null;
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override MipsInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new MipsInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override MipsInstruction NotYetImplemented(uint wInstr, string message)
        {
            var hex = $"{wInstr:X8}";
            base.EmitUnitTest("Mips16e", hex, message, "Mips16eDis", this.addr, w =>
            {
                w.WriteLine("           AssertCode(\"@@@\", \"{0}\");", hex);
            });

            return base.NotYetImplemented(wInstr, message);
        }

        public override MipsInstruction CreateInvalidInstruction()
        {
            return new MipsInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.illegal,
                Operands = MachineInstruction.NoOperands,
            };
        }

        private static readonly RegisterOperand[] registerEncoding = new uint[8] { 16, 17, 2, 3, 4, 5, 6, 7 }
            .Select(iReg => new RegisterOperand(Registers.generalRegs[iReg]))
            .ToArray();

        /// <summary>
        /// Decode a 3-bit register field.
        /// </summary>
        private static Mutator<Mips16eDisassembler> Reg(int bitpos)
        {
            var field = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                var encReg = field.Read(u);
                var reg = registerEncoding[encReg];
                d.ops.Add(reg);
                return true;
            };
        }

        /// <summary>
        /// Decode a single bit corresponding to a register.
        /// </summary>
        public static Mutator<Mips16eDisassembler> RBit(int bitpos, int iReg)
        {
            var reg = new RegisterOperand(Registers.generalRegs[iReg]);
            return (u, d) =>
            {
                if (Bits.IsBitSet(u, bitpos))
                {
                    d.ops.Add(reg);
                }
                return true;
            };
        }

        private static readonly ImmediateOperand[] frameSizeEncoding = new int[16]
        {
            16, 1, 2, 3,  4, 5, 6, 7,  8, 9, 10, 11,  12, 13, 14, 15
        }
            .Select(n => ImmediateOperand.Int32(n * 8))
            .ToArray();

        private static Mutator<Mips16eDisassembler> Framesize(Bitfield[] bitfields)
        {
            return (u, d) =>
            {
                var encFramesize = Bitfield.ReadFields(bitfields, u);
                var imm = frameSizeEncoding[encFramesize];
                d.ops.Add(imm);
                return true;
            };
        }
        private static Mutator<Mips16eDisassembler> SaveFramesize = Framesize(Bf((0, 4)));

        public static Decoder Instr(Mnemonic mnemonic, params Mutator<Mips16eDisassembler> [] mutators)
        {
            return new InstrDecoder<Mips16eDisassembler, Mnemonic, MipsInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        public static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Mips16eDisassembler>[] mutators)
        {
            return new InstrDecoder<Mips16eDisassembler, Mnemonic, MipsInstruction>(iclass, mnemonic, mutators);
        }

        public static Decoder Nyi(string message)
        {
            return new NyiDecoder<Mips16eDisassembler, Mnemonic, MipsInstruction>(message);
        }

        static Mips16eDisassembler()
        {
            var invalid = Instr(Mnemonic.illegal, InstrClass.Invalid);

            var svrsDecoders = Mask(7, 1,
                Instr(Mnemonic.restore),
                Instr(Mnemonic.save, RBit(6, 31), RBit(4, 17), RBit(5, 16), SaveFramesize));

            var i8decoders = Mask(8, 3, "  I8",
               Nyi("BTEQZ     "),
               Nyi("BTNEZ     "),
               Nyi("SWRASP1   "),
               Nyi("ADJSP2    "),

               svrsDecoders,
               Nyi("MOV32R3 ∉ "),
               Nyi("*         "),
               Nyi("MOVR324   "));

            rootDecoder = Mask(11, 5, "MIPS16e",
                Nyi("ADDIUSP1"),
                Nyi("ADDIUPC2"),
                Nyi("B       "),
                Nyi("JAL(X)  "),

                Nyi("BEQZ   "),
                Nyi("BNEZ   "),
                Nyi("SHIFT  "),
                invalid,      // reserved

                Nyi("RRI - A "),
                Nyi("ADDIU8  "),
                Nyi("SLTI    "),
                Nyi("SLTIU   "),

                i8decoders,
                Nyi("LI      "),
                Nyi("CMPI    "),
                invalid,      // reserved

                Nyi("LB   "),
                Nyi("LH   "),
                Nyi("LWSP4"),
                Nyi("LW   "),

                Nyi("LBU  "),
                Nyi("LHU  "),
                Nyi("LWPC5"),
                invalid,      // reserved

                Nyi("SB"),
                Nyi("SH"),
                Nyi("SWSP "),
                Nyi("SW         "),

                Nyi("RRR        "),
                Nyi("RR         "),
                Nyi("EXTEND     "),
                invalid      // reserved
                );
        }
    }
}
