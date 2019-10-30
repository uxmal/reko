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

using System;
using System.Collections.Generic;
using System.Linq;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;

namespace Reko.Arch.MicroBlaze
{
    using Decoder = Decoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction>;

    public class MicroBlazeDisassembler : DisassemblerBase<MicroBlazeInstruction>
    {
        private static readonly Decoder rootDecoder;

        private readonly MicroBlazeArchitecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;
        private List<MachineOperand> ops;

        public MicroBlazeDisassembler(MicroBlazeArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override MicroBlazeInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint wInstr))
                return null;
            this.ops.Clear();
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - this.addr);
            return instr;
        }


        /// <summary>
        /// Create a Reko bitfield using PA Risc bit position and bit length.
        /// </summary>
        /// <remarks>
        /// PA Risc instruction bits are numbered from the MSB to LSB, but 
        /// Reko bitfields are numbered from LSB to MSB.
        /// </summary>
        private static Bitfield BeField(int bitPos, int bitLength)
        {
            return new Bitfield(32 - (bitPos + bitLength), bitLength);
        }

        private static Bitfield[] BeFields(params (int bitPos, int bitLength)[] flds)
        {
            return flds.Select(f => BeField(f.bitPos, f.bitLength)).ToArray();
        }

        protected override MicroBlazeInstruction CreateInvalidInstruction()
        {
            return new MicroBlazeInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = new MachineOperand[0]
            };
        }

        #region Mutators

        private static Mutator<MicroBlazeDisassembler> Reg(int bePos, int len)
        {
            var field = BeField(bePos, len);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = Registers.GpRegs[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        private static Mutator<MicroBlazeDisassembler> RD = Reg(6, 5);
        private static Mutator<MicroBlazeDisassembler> RA = Reg(11, 5);
        private static Mutator<MicroBlazeDisassembler> RB = Reg(16, 5);

        #endregion

        #region Decoders
        private class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Mnemonic mnemonic;
            private readonly Mutator<MicroBlazeDisassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Mnemonic mnemonic, Mutator<MicroBlazeDisassembler>[] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override MicroBlazeInstruction Decode(uint wInstr, MicroBlazeDisassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                var instr = new MicroBlazeInstruction
                {
                    InstructionClass = iclass,
                    Mnemonic = mnemonic,
                    Operands = dasm.ops.ToArray()
                };
                return instr;
            }
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, params Mutator<MicroBlazeDisassembler>[] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mnemonic, mutators);
        }


        private static InstrDecoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<MicroBlazeDisassembler>[] mutators)
        {
            return new InstrDecoder(iclass, mnemonic, mutators);
        }

        protected static NyiDecoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction> Nyi(string message)
        {
            return new NyiDecoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction>(message);
        }

        protected static MaskDecoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction> MaskBe(int bitPos, int bitLength, string tag, params Decoder[] decoders)
        {
            var pos = 32 - (bitPos + bitLength);
            return new MaskDecoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction>(pos, bitLength, tag, decoders);
        }

        #endregion


        static MicroBlazeDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            rootDecoder = MaskBe(0, 6, "",
                Nyi("0x00"),
                Nyi("0x01"),
                Nyi("0x02"),
                Nyi("0x03"),
                Nyi("0x04"),
                Nyi("0x05"),
                Nyi("0x06"),
                Nyi("0x07"),
                Nyi("0x08"),
                Nyi("0x09"),
                Nyi("0x0A"),
                Nyi("0x0B"),
                Nyi("0x0C"),
                Nyi("0x0D"),
                Nyi("0x0E"),
                Nyi("0x0F"),

                Nyi("0x10"),
                Nyi("0x11"),
                Nyi("0x12"),
                Nyi("0x13"),
                Nyi("0x14"),
                Nyi("0x15"),
                Nyi("0x16"),
                Nyi("0x17"),
                Nyi("0x18"),
                Nyi("0x19"),
                Nyi("0x1A"),
                Nyi("0x1B"),
                Nyi("0x1C"),
                Nyi("0x1D"),
                Nyi("0x1E"),
                Nyi("0x1F"),

                Nyi("0x20"),
                Nyi("0x21"),
                Instr(Mnemonic.xor, RD, RA, RB), //$TODO zero?
                Nyi("0x23"),
                Nyi("0x24"),
                Nyi("0x25"),
                Nyi("0x26"),
                Nyi("0x27"),
                Nyi("0x28"),
                Nyi("0x29"),
                Nyi("0x2A"),
                Nyi("0x2B"),
                Nyi("0x2C"),
                Nyi("0x2D"),
                Nyi("0x2E"),
                Nyi("0x2F"),

                Nyi("0x30"),
                Nyi("0x31"),
                Nyi("0x32"),
                Nyi("0x33"),
                Nyi("0x34"),
                Nyi("0x35"),
                Nyi("0x36"),
                Nyi("0x37"),
                Nyi("0x38"),
                Nyi("0x39"),
                Nyi("0x3A"),
                Nyi("0x3B"),
                Nyi("0x3C"),
                Nyi("0x3D"),
                Nyi("0x3E"),
                Nyi("0x3F"));
        }
    }
}