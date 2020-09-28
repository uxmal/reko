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

using System;
using System.Collections.Generic;
using System.Linq;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;

namespace Reko.Arch.MicroBlaze
{
    using Decoder = Decoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction>;

    public class MicroBlazeDisassembler : DisassemblerBase<MicroBlazeInstruction, Mnemonic>
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

        public override MicroBlazeInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new MicroBlazeInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override MicroBlazeInstruction CreateInvalidInstruction()
        {
            return new MicroBlazeInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        public override MicroBlazeInstruction NotYetImplemented(uint wInstr, string message)
        {
            var hex = $"{wInstr:X8}";
            EmitUnitTest("MicroBlaze", hex, message, "MicroBlazeDis", this.addr, w =>
            {
                w.WriteLine("AssertCode(\"@@@\", \"{0}\");", hex);
            });
            return CreateInvalidInstruction();
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

        private static readonly Mutator<MicroBlazeDisassembler> Rd = Reg(6, 5);
        private static readonly Mutator<MicroBlazeDisassembler> Ra = Reg(11, 5);
        private static readonly Mutator<MicroBlazeDisassembler> Rb = Reg(16, 5);

        private static Mutator<MicroBlazeDisassembler> ImmS(int bePos, int len)
        {
            var field = BeField(bePos, len);
            return (u, d) =>
            {
                var n = field.ReadSigned(u);
                d.ops.Add(ImmediateOperand.Word32(n));
                return true;
            };
        }

        private static readonly Mutator<MicroBlazeDisassembler> Is16 = ImmS(16, 16);


        private static Mutator<MicroBlazeDisassembler> ImmU(int bePos, int len)
        {
            var field = BeField(bePos, len);
            return (u, d) =>
            {
                var n = field.Read(u);
                d.ops.Add(ImmediateOperand.Word32(n));
                return true;
            };
        }

        private static readonly Mutator<MicroBlazeDisassembler> Iu16 = ImmU(16, 16);

        private static Mutator<MicroBlazeDisassembler> Abs(int bePos, int len)
        {
            var field = BeField(bePos, len);
            return (u, d) =>
            {
                var n = (uint) field.ReadSigned(u);
                var addr = Address.Ptr32(n);
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }

        private static readonly Mutator<MicroBlazeDisassembler> Abs16 = Abs(16, 16);


        private static Mutator<MicroBlazeDisassembler> PcRel(int bePos, int len)
        {
            var field = BeField(bePos, len);
            return (u, d) =>
            {
                var n = field.ReadSigned(u);
                var addr = d.addr + n;
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }

        private static readonly Mutator<MicroBlazeDisassembler> Pc16 = PcRel(16, 16);

        #endregion

        #region Decoders

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<MicroBlazeDisassembler>[] mutators)
        {
            return new InstrDecoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction>(InstrClass.Linear, mnemonic, mutators);
        }


        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<MicroBlazeDisassembler>[] mutators)
        {
            return new InstrDecoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction>(iclass, mnemonic, mutators);
        }

        protected static NyiDecoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction> Nyi(string message)
        {
            return new NyiDecoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction>(message);
        }

        protected static MaskDecoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction> MaskBe(int beBitPos, int bitLength, string tag, params Decoder[] decoders)
        {
            var pos = 32 - (beBitPos + bitLength);
            return new MaskDecoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction>(pos, bitLength, tag, decoders);
        }

        protected static MaskDecoder<MicroBlazeDisassembler, Mnemonic, MicroBlazeInstruction> SparseBe(int beBitPos, int bitLength, string tag, Decoder defaultDecoder, params (uint,Decoder)[] decoders)
        {
            var pos = 32 - (beBitPos + bitLength);
            return Sparse(pos, bitLength, tag, defaultDecoder, decoders);
        }

        #endregion


        static MicroBlazeDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            rootDecoder = MaskBe(0, 6, "",
                Instr(Mnemonic.add, Rd, Ra, Rb),
                Instr(Mnemonic.rsub, Rd, Ra, Rb),
                Instr(Mnemonic.addc, Rd, Ra, Rb),
                Instr(Mnemonic.rsubc, Rd, Ra, Rb),

                Instr(Mnemonic.addk, Rd, Ra, Rb),
                MaskBe(30, 2, "rsubk",
                    Instr(Mnemonic.rsubk, Rd, Ra, Rb),
                    Instr(Mnemonic.cmp, Rd, Ra, Rb),
                    invalid,
                    Instr(Mnemonic.cmpu, Rd, Ra, Rb)),
                Nyi("0x06"),
                Nyi("0x07"),

                Instr(Mnemonic.addi, Rd,Ra, Is16),
                Instr(Mnemonic.rsubi, Rd,Ra, Is16),
                Instr(Mnemonic.addic, Rd, Ra, Is16),
                Instr(Mnemonic.rsubic, Rd, Ra, Is16),

                Instr(Mnemonic.addik, Rd, Ra, Is16),
                Instr(Mnemonic.rsubik, Rd, Ra, Is16),
                Instr(Mnemonic.addikc, Rd, Ra, Is16),
                Instr(Mnemonic.rsubikc, Rd, Ra, Is16),

                // 10
                MaskBe(30, 2, "  0x10",
                    Instr(Mnemonic.mul, Rd, Ra, Rb),
                    Instr(Mnemonic.mulh, Rd, Ra, Rb),
                    Instr(Mnemonic.mulhsu, Rd, Ra, Rb),
                    Instr(Mnemonic.mulhu, Rd, Ra, Rb)),
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

                // 20
                Select(u => u == 0x80000000,
                    Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding), 
                    Instr(Mnemonic.or, Rd, Ra, Rb)),
                Instr(Mnemonic.and, Rd, Ra, Rb),
                Instr(Mnemonic.xor, Rd, Ra, Rb),
                Instr(Mnemonic.andn, Rd, Ra, Rb),

                MaskBe(31, 1, "  0x24",
                    Instr(Mnemonic.sext8, Rd,Ra),
                    Mask(5, 2, "  0x24 ...1",
                        Instr(Mnemonic.sra, Rd,Ra),
                        Instr(Mnemonic.src, Rd,Ra),
                        Instr(Mnemonic.srl, Rd,Ra),
                        Instr(Mnemonic.sext16, Rd,Ra))),
                Nyi("0x25"),
                SparseBe(11, 5, "0x26", invalid,
                    (0x00, Instr(Mnemonic.br, InstrClass.Transfer, Rb)),
                    (0x08, Instr(Mnemonic.bra, InstrClass.Transfer, Rb)),
                    (0x0C, Instr(Mnemonic.brk, InstrClass.Transfer | InstrClass.Delay | InstrClass.Call, Rd, Rb)),
                    (0x10, Instr(Mnemonic.brd, InstrClass.Transfer| InstrClass.Delay, Rb)),
                    (0x14, Instr(Mnemonic.brld, InstrClass.Transfer| InstrClass.Delay | InstrClass.Call, Rd, Rb)),
                    (0x18, Instr(Mnemonic.brad, InstrClass.Transfer | InstrClass.Delay, Rb)),
                    (0x1C, Instr(Mnemonic.brald, InstrClass.Transfer | InstrClass.Delay | InstrClass.Call, Rd, Rb))),
                Nyi("0x27"),

                Instr(Mnemonic.ori, Rd,Ra,Is16),
                Instr(Mnemonic.andi, Rd,Ra,Is16),
                Instr(Mnemonic.xori, Rd,Ra,Is16),
                Instr(Mnemonic.andni, Rd,Ra,Is16),

                Instr(Mnemonic.imm, Iu16),
                SparseBe(6, 5, "0x2D", invalid,
                    (0x10, Instr(Mnemonic.rtsd, InstrClass.Transfer | InstrClass.Delay, Ra,Is16)),
                    (0x11, Instr(Mnemonic.rtid, InstrClass.Transfer | InstrClass.Delay, Ra,Iu16)),
                    (0x12, Instr(Mnemonic.rtbd, InstrClass.Transfer | InstrClass.Delay, Ra,Iu16)),
                    (0x14, Instr(Mnemonic.rted, InstrClass.Transfer | InstrClass.Delay, Ra,Iu16))),
                SparseBe(11, 5, "0x2E", invalid,
                    (0x00, Instr(Mnemonic.bri, InstrClass.Transfer, Pc16)),
                    (0x08, Instr(Mnemonic.brai, InstrClass.Transfer, Abs16)),
                    (0x0C, Instr(Mnemonic.brki, InstrClass.Transfer | InstrClass.Delay, Rd, Abs16)),
                    (0x10, Instr(Mnemonic.brid, InstrClass.Transfer | InstrClass.Delay, Pc16)),
                    (0x14, Instr(Mnemonic.brlid, InstrClass.Transfer | InstrClass.Delay | InstrClass.Call, Rd, Pc16)),
                    (0x18, Instr(Mnemonic.braid, InstrClass.Transfer | InstrClass.Delay, Abs16)),
                    (0x1C, Instr(Mnemonic.bralid, InstrClass.Transfer | InstrClass.Delay | InstrClass.Call, Rd, Abs16))),
                SparseBe(6, 5, "  0x2F", invalid,
                    (0x00, Instr(Mnemonic.beqi, InstrClass.Transfer, Ra, Pc16)),
                    (0x01, Instr(Mnemonic.bnei, InstrClass.Transfer, Ra, Pc16)),
                    (0x02, Instr(Mnemonic.blti, InstrClass.Transfer, Ra, Pc16)),
                    (0x03, Instr(Mnemonic.blei, InstrClass.Transfer, Ra, Pc16)),
                    (0x04, Instr(Mnemonic.bgti, InstrClass.Transfer, Ra, Pc16)),
                    (0x05, Instr(Mnemonic.bgei, InstrClass.Transfer, Ra, Pc16)),

                    (0x10, Instr(Mnemonic.beqid, InstrClass.Transfer | InstrClass.Delay, Ra, Pc16)),
                    (0x11, Instr(Mnemonic.bneid, InstrClass.Transfer | InstrClass.Delay, Ra, Pc16)),
                    (0x12, Instr(Mnemonic.bltid, InstrClass.Transfer | InstrClass.Delay, Ra, Pc16)),
                    (0x13, Instr(Mnemonic.bleid, InstrClass.Transfer | InstrClass.Delay, Ra, Pc16)),
                    (0x14, Instr(Mnemonic.bgtid, InstrClass.Transfer | InstrClass.Delay, Ra, Pc16)),
                    (0x15, Instr(Mnemonic.bgeid, InstrClass.Transfer | InstrClass.Delay, Ra, Pc16))),

                // 30
                Instr(Mnemonic.lbu, Rd,Ra,Rb),
                Instr(Mnemonic.lhu, Rd,Ra,Rb),
                Instr(Mnemonic.lw, Rd,Ra,Rb),
                Nyi("0x33"),

                Instr(Mnemonic.sb, Rd, Ra, Rb),
                Instr(Mnemonic.sh, Rd, Ra, Rb),
                Instr(Mnemonic.sw, Rd, Ra, Rb),
                Nyi("0x37"),

                Instr(Mnemonic.lbui, Rd, Ra, Is16),
                Instr(Mnemonic.lhui, Rd, Ra, Is16),
                Instr(Mnemonic.lwi, Rd, Ra,  Is16),
                Nyi("0x3B"),

                Instr(Mnemonic.sbi, Rd, Ra, Is16),
                Instr(Mnemonic.shi, Rd, Ra, Is16),
                Instr(Mnemonic.swi, Rd, Ra, Is16),
                Nyi("0x3F"));
        }
    }
}