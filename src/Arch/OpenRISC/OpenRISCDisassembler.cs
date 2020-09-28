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

using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.OpenRISC
{
    using Decoder = Decoder<OpenRISCDisassembler, Mnemonic, OpenRISCInstruction>;

    public class OpenRISCDisassembler : DisassemblerBase<OpenRISCInstruction, Mnemonic>
    {
        private const InstrClass TD = InstrClass.Transfer | InstrClass.Delay;
        private const InstrClass TDC = InstrClass.Transfer | InstrClass.Delay | InstrClass.Call;

        private static readonly Decoder rootDecoder;

        private readonly OpenRISCArchitecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;
        private List<MachineOperand> ops;

        public OpenRISCDisassembler(OpenRISCArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override OpenRISCInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint wInstr))
                return null;
            this.ops.Clear();
            var instr = rootDecoder.Decode(wInstr, this);
            if (wInstr == 0)
            {
                instr.InstructionClass = InstrClass.Terminates | InstrClass.Padding | InstrClass.Zero;
            }
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override OpenRISCInstruction CreateInvalidInstruction()
        {
            return new OpenRISCInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        public override OpenRISCInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new OpenRISCInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override OpenRISCInstruction NotYetImplemented(uint wInstr, string message)
        {
            var hex = $"{wInstr:X8}";
            base.EmitUnitTest("OpenRISC", hex, message, "OpenRiscDis", this.addr, w =>
            {
                w.WriteLine("    AssertCode(\"@@@\", \"{0}\");", hex);
            });
            return CreateInvalidInstruction();
        }

        #region Mutators

        private static Mutator<OpenRISCDisassembler> R(int bitPos, int bitLength)
        {
            var field = new Bitfield(bitPos, bitLength);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = Registers.GpRegs[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        private static readonly Mutator<OpenRISCDisassembler> RD = R(21, 5);
        private static readonly Mutator<OpenRISCDisassembler> RA = R(16, 5);
        private static readonly Mutator<OpenRISCDisassembler> RB = R(11, 5);

        private static Mutator<OpenRISCDisassembler> Spr(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var iSpr = (int) Bitfield.ReadFields(fields, u);
                if (Registers.SpecialRegisters.TryGetValue(iSpr, out var spr))
                {
                    d.ops.Add(new RegisterOperand(spr));
                }
                else
                {
                    d.ops.Add(ImmediateOperand.UInt32((uint) iSpr));
                }
                return true;
            };
        }

        private static Mutator<OpenRISCDisassembler> Is(int bitPos, int bitLength)
        {
            var field = new Bitfield(bitPos, bitLength);
            return (u, d) =>
            {
                var n = field.ReadSigned(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.SignedWordWidth, n)));
                return true;
            };
        }

        private static readonly Mutator<OpenRISCDisassembler> Is16 = Is(0, 16);

        private static Mutator<OpenRISCDisassembler> Isu(int bitPos, int bitLength)
        {
            var field = new Bitfield(bitPos, bitLength);
            return (u, d) =>
            {
                var n = field.ReadSigned(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, n)));
                return true;
            };
        }

        private static readonly Mutator<OpenRISCDisassembler> Isu16 = Isu(0, 16);


        private static Mutator<OpenRISCDisassembler> Iu(int bitPos, int bitLength)
        {
            var field = new Bitfield(bitPos, bitLength);
            return (u, d) =>
            {
                var n = field.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, n)));
                return true;
            };
        }

        private static readonly Mutator<OpenRISCDisassembler> Iu6 = Iu(0, 6);
        private static readonly Mutator<OpenRISCDisassembler> Iu16 = Iu(0, 16);

        private static Mutator<OpenRISCDisassembler> Iu(Bitfield[] bitfields)
        {
            return (u, d) =>
            {
                var n = Bitfield.ReadFields(bitfields, u);
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, n)));
                return true;
            };
        }


        private static Mutator<OpenRISCDisassembler> Pc(int bitPos, int bitLength)
        {
            var field = new Bitfield(bitPos, bitLength);
            return (u, d) =>
            {
                var displacement = field.ReadSigned(u) << 2;
                var addrDest = d.addr + displacement;
                d.ops.Add(AddressOperand.Create(addrDest));
                return true;
            };
        }

        private static readonly Mutator<OpenRISCDisassembler> Pc26 = Pc(0, 26);

        private static Mutator<OpenRISCDisassembler> Mo(int raPos, PrimitiveType size)
        {
            var baseRegField = new Bitfield(raPos, 5);
            var offsetField = new Bitfield(0, 16);
            return (u, d) =>
            {
                var iReg = baseRegField.Read(u);
                var offset = offsetField.ReadSigned(u);
                var mop = new MemoryOperand(Registers.GpRegs[iReg], offset, size);
                d.ops.Add(mop);
                return true;
            };
        }

        private static Mutator<OpenRISCDisassembler> Mo(int raPos, Bitfield[] offsetFields, PrimitiveType size)
        {
            var baseRegField = new Bitfield(raPos, 5);
            return (u, d) =>
            {
                var iReg = baseRegField.Read(u);
                var offset = Bitfield.ReadSignedFields(offsetFields, u);
                var mop = new MemoryOperand(Registers.GpRegs[iReg], offset, size);
                d.ops.Add(mop);
                return true;
            };
        }

        private static Mutator<OpenRISCDisassembler> Page(int bitLen)
        {
            var offsetField = new Bitfield(0, bitLen);
            return (u, d) =>
            {
                long pageOffset = offsetField.ReadSigned(u) << 13;
                long lAddr = (long) (d.addr.ToLinear() & ~8192ul) + pageOffset;
                ulong uAddr = (ulong) lAddr;
                var aOp = d.arch.WordWidth.BitSize == 64
                    ? AddressOperand.Ptr64(uAddr)
                    : AddressOperand.Ptr32((uint) uAddr);
                d.ops.Add(aOp);
                return true;
            };
        }
        #endregion

        #region Decoders

        private class Instr64Decoder : Decoder
        {
            private Decoder<OpenRISCDisassembler, Mnemonic, OpenRISCInstruction> dec32bit;
            private Decoder<OpenRISCDisassembler, Mnemonic, OpenRISCInstruction> dec64bit;

            public Instr64Decoder(Decoder<OpenRISCDisassembler, Mnemonic, OpenRISCInstruction> dec32bit, Decoder<OpenRISCDisassembler, Mnemonic, OpenRISCInstruction> dec64bit)
            {
                this.dec32bit = dec32bit;
                this.dec64bit = dec64bit;
            }

            public override OpenRISCInstruction Decode(uint wInstr, OpenRISCDisassembler dasm)
            {
                var dec = (dasm.arch.WordWidth.BitSize == 64)
                    ? dec64bit
                    : dec32bit;
                return dec.Decode(wInstr, dasm);
            }
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<OpenRISCDisassembler>[] mutators)
        {
            return new InstrDecoder<OpenRISCDisassembler, Mnemonic, OpenRISCInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<OpenRISCDisassembler>[] mutators)
        {
            return new InstrDecoder<OpenRISCDisassembler, Mnemonic, OpenRISCInstruction>(iclass, mnemonic, mutators);
        }

        private static Instr64Decoder Instr64(Decoder dec32bit, Decoder dec64bit)
        {
            return new Instr64Decoder(dec32bit, dec64bit);
        }

        private static NyiDecoder<OpenRISCDisassembler, Mnemonic, OpenRISCInstruction> Nyi(string message)
        {
            return new NyiDecoder<OpenRISCDisassembler, Mnemonic, OpenRISCInstruction>(message);
        }

        #endregion

        static OpenRISCDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            rootDecoder = Mask(26, 6, "OpenRISC",
                // 00
                Instr(Mnemonic.l_j, TD, Pc26),
                Instr(Mnemonic.l_jal, TDC, Pc26),
                Instr64(
                    Instr(Mnemonic.l_adrp, RD,Page(19)),
                    Instr(Mnemonic.l_adrp, RD,Page(21))),
                Instr(Mnemonic.l_bnf, TD | InstrClass.Conditional, Pc26),

                Instr(Mnemonic.l_bf, TD | InstrClass.Conditional, Pc26),
                Instr(Mnemonic.l_nop, InstrClass.Linear | InstrClass.Padding),
                Mask(16, 1, "  0x06",
                    Instr(Mnemonic.l_movhi, RD,Iu16),
                    Instr(Mnemonic.l_macrc, RD)),
                invalid,

                Sparse(23, 3, "  0x08", invalid,
                    (0b000, Instr(Mnemonic.l_sys, Iu16)),
                    (0b010, Instr(Mnemonic.l_trap, Iu16)),
                    (0b100, Instr(Mnemonic.l_msync)),
                    (0b101, Instr(Mnemonic.l_psync)),
                    (0b110, Instr(Mnemonic.l_csync))),
                Instr(Mnemonic.l_rfe, TD),
                Nyi("0x0A"),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                // 10
                invalid,
                Instr(Mnemonic.l_jr, TD, RB),
                Instr(Mnemonic.l_jalr, TDC, RB),
                Instr(Mnemonic.l_maci, RA,Is16),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.l_lf, RD,Mo(16, PrimitiveType.Real32)),
                Instr(Mnemonic.l_lwa, RD,Mo(16, PrimitiveType.Word32)),

                invalid, // 0x1C - custom
                invalid, // 0x1D - custom
                invalid, // 0x1E - custom
                invalid, // 0x1F - custom

                // 20
                Instr64(
                    invalid,
                    Instr(Mnemonic.l_ld, RD,Mo(16,PrimitiveType.Word64))),
                Instr(Mnemonic.l_lwz, RD,Mo(16,PrimitiveType.Word32)),
                Instr(Mnemonic.l_lws, RD,Mo(16,PrimitiveType.Int32)),
                Instr(Mnemonic.l_lbz, RD,Mo(16,PrimitiveType.Byte)),

                Instr(Mnemonic.l_lbs, RD,Mo(16,PrimitiveType.SByte)),
                Instr(Mnemonic.l_lhz, RD, Mo(16, PrimitiveType.Word16)),
                Instr(Mnemonic.l_lhs, RD, Mo(16, PrimitiveType.Int16)),
                Instr(Mnemonic.l_addi, RD,RA,Is16),

                Instr(Mnemonic.l_addic, RD,RA,Is16),
                Instr(Mnemonic.l_andi, RD,RA,Iu16),
                Instr(Mnemonic.l_ori, RD,RA,Iu16),
                Instr(Mnemonic.l_xori, RD,RA,Iu16),

                Instr(Mnemonic.l_muli, RD,RA,Is16),
                Instr(Mnemonic.l_mfspr, RD,RA,Spr(Bf((0, 16)))),
                Mask(6, 2, "  0x2E",
                    Instr(Mnemonic.l_slli, RD,RA, Iu6),
                    Instr(Mnemonic.l_srli, RD,RA, Iu6),
                    Instr(Mnemonic.l_srai, RD,RA, Iu6),
                    Instr(Mnemonic.l_rori, RD,RA, Iu6)),
                Sparse(21, 5, "  0x2F", invalid,
                    (0x0, Instr(Mnemonic.l_sfeqi, RA,Isu16)),
                    (0x1, Instr(Mnemonic.l_sfnei, RA,Isu16)),
                    (0x2, Instr(Mnemonic.l_sfgtui, RA,Isu16)),
                    (0x3, Instr(Mnemonic.l_sfgeui, RA,Isu16)),
                    (0x4, Instr(Mnemonic.l_sfltui, RA, Isu16)),
                    (0x5, Instr(Mnemonic.l_sfleui, RA, Isu16)),

                    (0xA, Instr(Mnemonic.l_sfgtsi, RA,Is16)),
                    (0xB, Instr(Mnemonic.l_sfgesi, RA,Is16)),
                    (0xC, Instr(Mnemonic.l_sfltsi, RA,Is16)),
                    (0xD, Instr(Mnemonic.l_sflesi, RA,Is16))),

                // 30
                Instr(Mnemonic.l_mtspr, RA,RB,Spr(Bf((21, 5),(0, 11)))),
                Nyi("0x31"),
                Nyi("0x32"),
                Nyi("0x33"),

                Instr64(
                    invalid,
                    Instr(Mnemonic.l_sd, Mo(16, Bf((21,5),(0, 11)), PrimitiveType.Word32), RB)),
                Instr(Mnemonic.l_sw, Mo(16, Bf((21,5),(0, 11)), PrimitiveType.Word32), RB),
                Instr(Mnemonic.l_sb, Mo(16, Bf((21,5),(0, 11)), PrimitiveType.Byte), RB),
                Instr(Mnemonic.l_sh, Mo(16, Bf((21,5),(0, 11)), PrimitiveType.Word16), RB),

                Mask(8, 2, "  0x38",
                    Mask(0, 4, "  0x38-0",
                        Instr(Mnemonic.l_add, RD,RA,RB),
                        Instr(Mnemonic.l_addc, RD,RA,RB),
                        Instr(Mnemonic.l_sub, RD,RA,RB),
                        Instr(Mnemonic.l_and, RD,RA,RB),

                        Instr(Mnemonic.l_or, RD,RA,RB),
                        Instr(Mnemonic.l_xor, RD,RA,RB),
                        invalid,
                        invalid,

                        Mask(6, 2, " 0x30-0-8",
                            Instr(Mnemonic.l_sll, RD,RA,RB),
                            Instr(Mnemonic.l_srl, RD,RA,RB),
                            Instr(Mnemonic.l_sra, RD,RA,RB),
                            Instr(Mnemonic.l_ror, RD,RA,RB)),
                        invalid,
                        invalid,
                        invalid,

                        Nyi("  0x38-0-C"),
                        Nyi("  0x38-0-D"),
                        Instr(Mnemonic.l_cmov, RD,RA,RB),
                        Instr(Mnemonic.l_ff1, RD,RA)),
                    Sparse(0, 4, "  0x38-1", invalid,
                        (0xF, Instr(Mnemonic.l_fl1, RD,RA))),
                    Nyi("  0x38-2"),
                    Sparse(0, 4, "  0x38-3",
                        invalid,
                        (0x6, Instr(Mnemonic.l_mul, RD,RA,RB)),
                        (0x7, Instr(Mnemonic.l_muld, RA,RB)),
                        (0x9, Instr(Mnemonic.l_div, RD,RA,RB)),
                        (0xA, Instr(Mnemonic.l_divu, RD,RA,RB)),
                        (0xB, Instr(Mnemonic.l_mulu, RD,RA,RB)),
                        (0xC, Instr(Mnemonic.l_muldu, RA,RB)))),
                Sparse(21, 5, "  0x39", Nyi("0x39"),
                    (0x0, Instr(Mnemonic.l_sfeq, RA,RB)),
                    (0x1, Instr(Mnemonic.l_sfne, RA,RB)),
                    (0x2, Instr(Mnemonic.l_sfgtu, RA,RB)),
                    (0x3, Instr(Mnemonic.l_sfgeu, RA,RB)),
                    (0x4, Instr(Mnemonic.l_sfltu, RA,RB)),
                    (0x5, Instr(Mnemonic.l_sfleu, RA,RB)),
                    
                    (0xA, Instr(Mnemonic.l_sfgts, RA,RB)),
                    (0xB, Instr(Mnemonic.l_sfges, RA,RB)),
                    (0xC, Instr(Mnemonic.l_sflts, RA,RB)),
                    (0xD, Instr(Mnemonic.l_sfles, RA,RB))),
                Nyi("0x3A"),
                Nyi("0x3B"),

                Nyi("0x3C"),
                Nyi("0x3D"),
                Nyi("0x3E"),
                Nyi("0x3F"));
        }
    }
}