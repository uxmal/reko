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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Arch.RiscV
{
    using Decoder = Reko.Core.Machine.Decoder<RiscVDisassembler, Mnemonic, RiscVInstruction>;
    using MaskDecoder = Reko.Core.Machine.MaskDecoder<RiscVDisassembler, Mnemonic, RiscVInstruction>;

    public partial class RiscVDisassembler
    {
        public class InstructionSet
        {
            private static readonly Decoder invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

            private Func<Mnemonic, Mutator<RiscVDisassembler>[], Decoder> instr32Support;
            private Func<Mnemonic, Mutator<RiscVDisassembler>[], Decoder> instr64Support;
            private Func<Mnemonic, Mutator<RiscVDisassembler>[], Decoder> float16Support;
            private Func<Mnemonic, Mutator<RiscVDisassembler>[], Decoder> float32Support;
            private Func<Mnemonic, Mutator<RiscVDisassembler>[], Decoder> float64Support;
            private Func<Mnemonic, Mutator<RiscVDisassembler>[], Decoder> float128Support;
            private Func<Decoder, Decoder> zbaSupport;
            private Func<Decoder, Decoder> zbbSupport;
            private Func<Decoder, Decoder> zbcSupport;
            private Func<Decoder, Decoder> zbkbSupport;
            private Func<Decoder, Decoder> zbsSupport;
            private Func<Decoder, Decoder> zfaSupport;
            private Dictionary<string, object> options;

            public static InstructionSet Create(Dictionary<string, object> options)
            {
                var isa = new InstructionSet(options);
                return isa;
            }

            private InstructionSet(Dictionary<string, object> options)
            {
                this.options = options;

                instr32Support = !RiscVArchitecture.Is64Bit(options)
                  ? Instr
                  : (a, B) => invalid;
                instr64Support = RiscVArchitecture.Is64Bit(options)
                    ? Instr
                    : (a, B) => invalid;

                if (options.TryGetValue("FloatAbi", out var oFloatAbi) &&
                    int.TryParse(oFloatAbi.ToString(), out int floatAbi))
                {
                    switch (floatAbi)
                    {
                    case 128:
                        float128Support = Instr;
                        float64Support = Instr;
                        float32Support = Instr;
                        float16Support = Instr;
                        break;
                    case 64:
                        float128Support = MakeInvalid;
                        float64Support = Instr;
                        float32Support = Instr;
                        float16Support = Instr;
                        break;
                    case 32:
                        float128Support = MakeInvalid;
                        float64Support = MakeInvalid;
                        float32Support = Instr;
                        float16Support = Instr;
                        break;
                    case 16:
                        float128Support = MakeInvalid;
                        float64Support = MakeInvalid;
                        float32Support = MakeInvalid;
                        float16Support = Instr;
                        break;
                    default:
                        float128Support = MakeInvalid;
                        float64Support = MakeInvalid;
                        float32Support = MakeInvalid;
                        float16Support = MakeInvalid;
                        break;
                    }
                }
                else
                {
                    float128Support = MakeInvalid;
                    float64Support = MakeInvalid;
                    float32Support = MakeInvalid;
                    float16Support = MakeInvalid;
                }
                zbaSupport = GetSupportedDecoder(options, "Zba");
                zbbSupport = GetSupportedDecoder(options, "Zbb");
                zbcSupport = GetSupportedDecoder(options, "Zbc");
                zbkbSupport = GetSupportedDecoder(options, "Zbkb");
                zbsSupport = GetSupportedDecoder(options, "Zbs");
                zfaSupport = GetSupportedDecoder(options, "Zfa");
            }

            private static Decoder Identity(Decoder d) => d;
            private static Decoder Invalid(Decoder _) => invalid;

            private static Func<Decoder, Decoder> GetSupportedDecoder(
                Dictionary<string, object> options,
                string extensionName)
            {
                return GetSupportedDecoder(
                    !options.TryGetValue(extensionName, out var oExt) ||
                    !bool.TryParse(oExt.ToString()!, out var ext) ||
                    ext);
            }

            private static Func<Decoder, Decoder> GetSupportedDecoder(
                bool supported)
            {
                return supported ? Identity : Invalid;
            }

            private static Decoder MakeInvalid(Mnemonic mnemonic, params Mutator<RiscVDisassembler>[] mutators)
            {
                return invalid;
            }

            private static Decoder Instr(Mnemonic mnemonic, params Mutator<RiscVDisassembler>[] mutators)
            {
                return new InstrDecoder<RiscVDisassembler, Mnemonic, RiscVInstruction>(InstrClass.Linear, mnemonic, mutators);
            }

            private Decoder Instr32(Mnemonic mnemonic, params Mutator<RiscVDisassembler>[] mutators)
            {
                return instr32Support(mnemonic, mutators);
            }

            private Decoder Instr64(Mnemonic mnemonic, params Mutator<RiscVDisassembler>[] mutators)
            {
                return instr64Support(mnemonic, mutators);
            }

            private Decoder FpInstr16(Mnemonic mnemonic, params Mutator<RiscVDisassembler>[] mutators)
            {
                return float16Support(mnemonic, mutators);
            }

            private Decoder FpInstr32(Mnemonic mnemonic, params Mutator<RiscVDisassembler>[] mutators)
            {
                return float32Support(mnemonic, mutators);
            }

            private Decoder FpInstr64(Mnemonic mnemonic, params Mutator<RiscVDisassembler>[] mutators)
            {
                return float64Support(mnemonic, mutators);
            }

            private Decoder FpInstr128(Mnemonic mnemonic, params Mutator<RiscVDisassembler>[] mutators)
            {
                return float128Support(mnemonic, mutators);
            }

            private Decoder Zfa(Decoder decoder) => zfaSupport(decoder);

            private Decoder Zba(Decoder decoder) => zbaSupport(decoder);

            private Decoder Zbb(Decoder decoder) => zbbSupport(decoder);

            private Decoder Zbc(Decoder decoder) => zbcSupport(decoder);

            private Decoder Zbkb(Decoder decoder) => zbkbSupport(decoder);

            private Decoder Zbs(Decoder decoder) => zbsSupport(decoder);


            private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<RiscVDisassembler>[] mutators)
            {
                return new InstrDecoder<RiscVDisassembler, Mnemonic, RiscVInstruction>(iclass, mnemonic, mutators);
            }

            // Conditional decoder

            private static WordSizeDecoder WordSize(
                Decoder? rv32 = null,
                Decoder? rv64 = null,
                Decoder? rv128 = null)
            {
                return new WordSizeDecoder(rv32 ?? invalid, rv64 ?? invalid, rv128 ?? invalid);
            }

            private static NyiDecoder Nyi(string message)
            {
                return new NyiDecoder(message);
            }

            public Decoder[] CreateRootDecoders()
            {

                var loads = new Decoder[]           // 0b00000
                {
                    Instr(Mnemonic.lb, rd,Ls),
                    Instr(Mnemonic.lh, rd,Ls),
                    Instr(Mnemonic.lw, rd,Ls),
                    Instr(Mnemonic.ld, rd,Ls),    // 64I

                    Instr(Mnemonic.lbu, rd,Ls),
                    Instr(Mnemonic.lhu, rd,Ls),
                    Instr(Mnemonic.lwu, rd,Ls),    // 64I
                    Nyi(""),
                };

                var fploads = new Decoder[8]        // 0b00001
                {
                    invalid,
                    FpInstr16(Mnemonic.flh, Fd, MemSignedOffset(PrimitiveType.Real16, 15, (20, 12))),
                    FpInstr32(Mnemonic.flw, Fd, MemSignedOffset(PrimitiveType.Real32, 15, (20, 12))),
                    FpInstr64(Mnemonic.fld, Fd, MemSignedOffset(PrimitiveType.Real64, 15, (20, 12))),
                    FpInstr128(Mnemonic.flq, Fd, MemSignedOffset(PrimitiveType.Real128, 15, (20, 12))),
                    invalid,
                    invalid,
                    invalid,
                };

                var stores = new Decoder[]          // 0b01000
                {
                    Instr(Mnemonic.sb, r2,Ss),
                    Instr(Mnemonic.sh, r2,Ss),
                    Instr(Mnemonic.sw, r2,Ss),
                    Instr(Mnemonic.sd, r2,Ss),   // I64

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                };

                var bf_25_7_7_5 = Bf((25,7),(7,5));

                var fpstores = new Decoder[8]       // 0b01001
                {
                    invalid,
                    FpInstr16(Mnemonic.fsh, F2, MemSignedOffset(PrimitiveType.Real16, 15, bf_25_7_7_5)),
                    FpInstr32(Mnemonic.fsw, F2, MemSignedOffset(PrimitiveType.Real32, 15, bf_25_7_7_5)),
                    FpInstr64(Mnemonic.fsd, F2, MemSignedOffset(PrimitiveType.Real64, 15, bf_25_7_7_5)),
                    FpInstr128(Mnemonic.fsq, F2, MemSignedOffset(PrimitiveType.Real128, 15, bf_25_7_7_5)),
                    invalid,
                    invalid,
                    invalid,
                };

                var miscMem = new Decoder[8]
                {
                    Select((7,25), u => u == 0b0000_0001_0000_00000_000_00000, "  misc-mem 0",
                        Instr(Mnemonic.pause),
                        Select((7, 25), u => u == 0b1000_0011_0011_00000_000_00000, "  misc-mem fence",
                            Instr(Mnemonic.fence_tso),
                            Instr(Mnemonic.fence, ps_24, ps_20))),
                    Instr(Mnemonic.fence_i),
                    Nyi("misc-mem 2"),
                    Nyi("misc-mem 3"),

                    Nyi("misc-mem 4"),
                    Nyi("misc-mem 5"),
                    Nyi("misc-mem 6"),
                    Nyi("misc-mem 7")
                };

                var opimm = new Decoder[]           // 0b00100
                {
                    Instr(Mnemonic.addi, rd,r1,i),
                    Sparse(25, 7, "  slli", invalid,
                        (0b0000000, Instr(Mnemonic.slli, rd,r1,z)),
                        (0b0000001, Instr64(Mnemonic.slli, rd,r1,Z)),
                        (0b0000100, Sparse(20, 5, "  04", Nyi(""),
                            (0b11110, Zbkb(Instr(Mnemonic.zip, rd, r1))))),
                        (0b0010100, Zbs(Instr(Mnemonic.bseti, rd,r1,z))),
                        (0b0010101, Zbs(Instr64(Mnemonic.bseti, rd,r1,Z))),
                        (0b0100100, Zbs(Instr(Mnemonic.bclri, rd,r1,z))),
                        (0b0100101, Zbs(Instr64(Mnemonic.bclri, rd,r1,Z))),
                        (0b0110000, Sparse(20, 5, "  clz", Nyi(""),
                            (0b00000, Zbb(Instr(Mnemonic.clz, rd,r1))),
                            (0b00001, Zbb(Instr(Mnemonic.ctz, rd,r1))),
                            (0b00010, Zbb(Instr(Mnemonic.cpop, rd,r1))),
                            (0b00100, Zbb(Instr(Mnemonic.sext_b, rd,r1))),
                            (0b00101, Zbb(Instr(Mnemonic.sext_h, rd,r1))))),
                        (0b0110100, Zbs(Instr(Mnemonic.binvi, rd,r1,z))),
                        (0b0110101, Zbs(Instr64(Mnemonic.binvi, rd,r1,Z)))),
                    Instr(Mnemonic.slti, rd,r1,i),
                    Instr(Mnemonic.sltiu, rd,r1,i),

                    Instr(Mnemonic.xori, rd,r1,i),
                    Sparse(25, 7, "  srli", invalid,
                        (0b0000000, Instr(Mnemonic.srli, rd,r1,z)),
                        (0b0000001, Instr64(Mnemonic.srli, rd,r1,Z)),
                        (0b0000100, Sparse(20, 5, "  unzip", Nyi(""),
                            (0b11111, Zbkb(Instr32(Mnemonic.unzip, rd,r1))))),
                        (0b0010100, Sparse(20, 5, "  orc.b", Nyi(""),
                            (0b00111, Zbb(Instr(Mnemonic.orc_b, rd,r1))))),
                        (0b0100000, Instr(Mnemonic.srai, rd,r1,z)),
                        (0b0100001, Instr64(Mnemonic.srai, rd,r1,Z)),
                        (0b0100100, Zbs(Instr(Mnemonic.bexti, rd,r1,z))),
                        (0b0100101, Zbs(Instr64(Mnemonic.bexti, rd,r1,Z))),
                        (0b0110000, Zbb(Instr(Mnemonic.rori, rd,r1,z))),
                        (0b0110001, Zbb(Instr64(Mnemonic.rori, rd,r1,Z))),
                        (0b0110100, Sparse(20, 5, "  rev8", Nyi(""),
                            (0b11000, Zbb(Instr32(Mnemonic.rev8, rd,r1))))),
                        (0b0110101, Sparse(20, 5, "  rev8_64", Nyi(""),
                            (0b11000, Zbb(Instr64(Mnemonic.rev8, rd,r1)))))),
                    Instr(Mnemonic.ori, rd,r1,i),
                    Instr(Mnemonic.andi, rd,r1,i),
                };

                var opimm32 = new Decoder[]         // 0b00110
                {
                    Instr(Mnemonic.addiw, Rd,R1,I20s),
                    Sparse(25, 7, "  slliw", Nyi(""),
                        (0b0000000, Instr(Mnemonic.slliw, Rd,R1,Imm(20, 5))),
                        (0b0000101, Instr(Mnemonic.slli_uw, Rd,R1,Imm(20, 5))),
                        (0b0110000, Sparse(20, 6, "  clzw", Nyi(""),
                            (0b00000, Zbb(Instr(Mnemonic.clzw, Rd,R1))),
                            (0b00001, Zbb(Instr(Mnemonic.ctzw, Rd,R1))),
                            (0b00010, Zbb(Instr(Mnemonic.cpopw, Rd,R1)))))),
                    Nyi(""),
                    Nyi(""),

                    Nyi(""),
                    Sparse(25, 7, "srliw", Nyi(""),
                        (0b0000000, Instr(Mnemonic.srliw, rd,r1,Z)),
                        (0b0100000, Instr(Mnemonic.sraiw, rd,r1,Z)),
                        (0b0110000, Instr(Mnemonic.roriw, rd,r1,Z))),
                    Nyi(""),
                    Nyi(""),
                };

                var op = Sparse(25, 7, "  op", Nyi("op"),      // 0b01100
                   (0b0000000, Mask(12, 3, "  0x0",
                        Instr(Mnemonic.add, Rd, R1, R2),
                        Instr(Mnemonic.sll, Rd, R1, R2),
                        Instr(Mnemonic.slt, Rd, R1, R2),
                        Instr(Mnemonic.sltu, Rd, R1, R2),

                        Instr(Mnemonic.xor, Rd, R1, R2),
                        Instr(Mnemonic.srl, Rd, R1, R2),
                        Instr(Mnemonic.or, Rd, R1, R2),
                        Instr(Mnemonic.and, Rd, R1, R2))),
                    (0b0000001, Mask(12, 3, "  M32",
                        Instr(Mnemonic.mul, Rd, R1, R2),
                        Instr(Mnemonic.mulh, Rd, R1, R2),
                        Instr(Mnemonic.mulhsu, Rd, R1, R2),
                        Instr(Mnemonic.mulhu, Rd, R1, R2),

                        Instr(Mnemonic.div, Rd, R1, R2),
                        Instr(Mnemonic.divu, Rd, R1, R2),
                        Instr(Mnemonic.rem, Rd, R1, R2),
                        Instr(Mnemonic.remu, Rd, R1, R2))),
                     (0b0000100, Mask(12, 3, "  0x04",
                        Nyi("op - 04 - 0b000"),
                        Nyi("op - 04 - 0b001"),
                        Nyi("op - 04 - 0b010"),
                        Nyi("op - 04 - 0b011"),

                        Select((20, 5), Eq0, "  pack",
                            Zbb(Instr32(Mnemonic.zext_h, Rd, R1)),
                            Zbkb(Instr(Mnemonic.pack, Rd, R1, R2))),
                        Nyi("op - 04 - 0b101"),
                        Nyi("op - 04 - 0b110"),
                        Zbkb(Instr(Mnemonic.packh, Rd, R1, R2)))),
                     (0b0000101, Mask(12, 3, "  0x05",
                        Nyi("op - 05 - 0b000"),
                        Zbc(Instr(Mnemonic.clmul, Rd, R1, R2)),
                        Zbc(Instr(Mnemonic.clmulr, Rd, R1, R2)),
                        Zbc(Instr(Mnemonic.clmulh, Rd, R1, R2)),

                        Zbb(Instr(Mnemonic.min, Rd, R1, R2)),
                        Zbb(Instr(Mnemonic.minu, Rd, R1, R2)),
                        Zbb(Instr(Mnemonic.max, Rd, R1, R2)),
                        Zbb(Instr(Mnemonic.maxu, Rd, R1, R2)))),
                    (0b0010000, Mask(12, 3, "  0x10",
                        Nyi("op - 10 - 0b000"),
                        Nyi("op - 10 - 0b001"),
                        Zba(Instr(Mnemonic.sh1add, Rd, R1, R2)),
                        Nyi("op - 10 - 0b011"),

                        Zba(Instr(Mnemonic.sh2add, Rd, R1, R2)),
                        Nyi("op - 10 - 0b101"),
                        Zba(Instr(Mnemonic.sh3add, Rd, R1, R2)),
                        Nyi("op - 10 - 0b111"))),
                    (0b0010100, Mask(12, 3, "  0x14",
                        Nyi("op - 14 - 0b000"),
                        Zbs(Instr(Mnemonic.bset, Rd, R1, R2)),
                        Zbkb(Instr(Mnemonic.xperm_n, Rd,R1,R2)),
                        Nyi("op - 14 - 0b011"),

                        Zbkb(Instr(Mnemonic.xperm_b, Rd,R1,R2)),
                        Nyi("op - 14 - 0b101"),
                        Nyi("op - 14 - 0b110"),
                        Nyi("op - 14 - 0b111"))),
                    (0b0100000, Mask(12, 3, "  0x20",
                        Instr(Mnemonic.sub, Rd, R1, R2),
                        Nyi("op - 20 - 0b001"),
                        Nyi("op - 20 - 0b010"),
                        Nyi("op - 20 - 0b011"), 

                        Zbb(Instr(Mnemonic.xnor, Rd, R1, R2)),
                        Instr(Mnemonic.sra, Rd, R1, R2),
                        Zbb(Instr(Mnemonic.orn, Rd, R1, R2)),
                        Zbb(Instr(Mnemonic.andn, Rd, R1, R2)))),
                    (0b0100100, Mask(12, 3, "  0x24",
                        Nyi("op - 24 - 0b000"),
                        Zbs(Instr(Mnemonic.bclr, Rd, R1, R2)),
                        Nyi("op - 24 - 0b010"),
                        Nyi("op - 24 - 0b011"),
                        
                        Nyi("op - 24 - 0b100"),
                        Zbs(Instr(Mnemonic.bext, Rd, R1, R2)),
                        Nyi("op - 24 - 0b110"),
                        Nyi("op - 24 - 0b111"))),
                    (0b110000, Mask(12, 3, "  0x30",
                        Nyi("op - 30 - 0b000"),
                        Zbb(Instr(Mnemonic.rol, Rd, R1, R2)),
                        Nyi("op - 30 - 0b010"),
                        Nyi("op - 30 - 0b011"),

                        Nyi("op - 30 - 0b100"),
                        Zbb(Instr(Mnemonic.ror, Rd, R1, R2)),
                        Nyi("op - 30 - 0b110"),
                        Nyi("op - 30 - 0b111"))),

                    (0b110100, Mask(12, 3, "  0x34",
                        Nyi("op - 34 - 0b000"),
                        Zbs(Instr(Mnemonic.binv, Rd, R1, R2)),
                        Nyi("op - 34 - 0b010"),
                        Nyi("op - 34 - 0b011"),

                        Nyi("op - 34 - 0b100"),
                        Nyi("op - 34 - 0b101"),
                        Nyi("op - 34 - 0b110"),
                        Nyi("op - 34 - 0b111"))));

                var op32 = Sparse(25, 7, "  op-32", Nyi(""),         // 0b01110
                    (0b0000000, Sparse(12, 3, "  00", Nyi(""),
                         (0, Instr(Mnemonic.addw, rd, r1, r2)),
                         (1, Instr(Mnemonic.sllw, rd, r1, r2)),
                         (5, Instr(Mnemonic.srlw, rd, r1, r2)))),
                    (0b0000001, Sparse(12, 3, "  01", Nyi(""),
                         (0, Instr(Mnemonic.mulw, rd, r1, r2)),
                         (4, Instr(Mnemonic.divw, rd, r1, r2)),
                         (5, Instr(Mnemonic.divuw, rd, r1, r2)),
                         (6, Instr(Mnemonic.remw, rd, r1, r2)),
                         (7, Instr(Mnemonic.remuw, rd, r1, r2)))),
                    (0b0000100, Sparse(12, 3, "  04", Nyi(""),
                         (0, Zba(Instr64(Mnemonic.add_uw, rd, r1, r2))),
                         (4, Select((20, 5), Eq0, "  pack",
                            Zbb(Instr64(Mnemonic.zext_h, Rd, R1)),
                            Zbkb(Instr(Mnemonic.packw, Rd, R1, R2)))))),
                    (0b0010000, Sparse(12, 3, "  0x10", Nyi(""),
                         (0b010, Zba(Instr(Mnemonic.sh1add_uw, Rd,R1,R2))), 
                         (0b100, Zba(Instr(Mnemonic.sh2add_uw, Rd, R1,R2))), 
                         (0b110, Zba(Instr(Mnemonic.sh3add_uw, Rd, R1,R2))))), 
                    (0b0100000, Sparse(12, 3, "  04", Nyi(""),
                         (0, Instr(Mnemonic.subw, rd, r1, r2)),
                         (5, Instr(Mnemonic.sraw, rd, r1, r2)))),
                    (0b0110000, Sparse(12, 3, "  04", Nyi(""),
                         (1, Zbb(Instr(Mnemonic.rolw, rd, r1, r2))),
                         (5, Zbb(Instr(Mnemonic.rorw, rd, r1, r2))))));

                var opfp = new (uint, Decoder)[]     // 0b10100
                {
                    ( 0x00, FpInstr32(Mnemonic.fadd_s, Fd,F1,F2, rm12) ),
                    ( 0x01, FpInstr64(Mnemonic.fadd_d, Fd,F1,F2, rm12) ),
                    ( 0x02, FpInstr16(Mnemonic.fadd_h, Fd,F1,F2, rm12) ),
                    ( 0x03, FpInstr128(Mnemonic.fadd_q, Fd,F1,F2, rm12) ),

                    ( 0x04, FpInstr32(Mnemonic.fsub_s, Fd,F1,F2, rm12) ),
                    ( 0x05, FpInstr64(Mnemonic.fsub_d, Fd,F1,F2, rm12) ),
                    ( 0x06, FpInstr16(Mnemonic.fsub_h, Fd,F1,F2, rm12) ),
                    ( 0x07, FpInstr128(Mnemonic.fsub_q, Fd,F1,F2, rm12) ),

                    ( 0x08, FpInstr32(Mnemonic.fmul_s, Fd,F1,F2, rm12) ),
                    ( 0x09, FpInstr64(Mnemonic.fmul_d, Fd,F1,F2, rm12) ),
                    ( 0x0A, FpInstr16(Mnemonic.fmul_h, Fd,F1,F2, rm12) ),
                    ( 0x0B, FpInstr128(Mnemonic.fmul_q, Fd,F1,F2, rm12) ),

                    ( 0x0C, FpInstr32(Mnemonic.fdiv_s, Fd,F1,F2, rm12) ),
                    ( 0x0D, FpInstr64(Mnemonic.fdiv_d, Fd,F1,F2, rm12) ),
                    ( 0x0E, FpInstr16(Mnemonic.fdiv_h, Fd,F1,F2, rm12) ),
                    ( 0x0F, FpInstr128(Mnemonic.fdiv_q, Fd,F1,F2, rm12) ),

                    ( 0x10, Sparse(12, 3, "fsgn.s", invalid,
                        (0x0, Select(R1EqR2,
                            FpInstr32(Mnemonic.fmv_s, Fd,F1),
                            FpInstr32(Mnemonic.fsgnj_s, Fd,F1, F2))),
                        (0x1, Select(R1EqR2,
                            FpInstr32(Mnemonic.fneg_s, Fd,F1),
                            FpInstr32(Mnemonic.fsgnjn_s, Fd,F1, F2))),
                        (0x2, Select(R1EqR2,
                            FpInstr32(Mnemonic.fabs_s, Fd,F1),
                            FpInstr32(Mnemonic.fsgnjx_s, Fd,F1, F2))))),
                    ( 0x11, Sparse(12, 3, "fsgn.d", invalid,
                        (0x0, Select(R1EqR2,
                            FpInstr64(Mnemonic.fmv_d, Fd,F1),
                            FpInstr64(Mnemonic.fsgnj_d, Fd,F1, F2))),
                        (0x1, Select(R1EqR2,
                            FpInstr64(Mnemonic.fneg_d, Fd,F1),
                            FpInstr64(Mnemonic.fsgnjn_d, Fd,F1, F2))),
                        (0x2, Select(R1EqR2,
                            FpInstr64(Mnemonic.fabs_d, Fd,F1),
                            FpInstr64(Mnemonic.fsgnjx_d, Fd,F1, F2))))),
                    ( 0x12, Sparse(12, 3, "fsgn.h", invalid,
                        (0x0, Select(R1EqR2,
                            FpInstr64(Mnemonic.fmv_h, Fd,F1),
                            FpInstr64(Mnemonic.fsgnj_h, Fd,F1, F2))),
                        (0x1, Select(R1EqR2,
                            FpInstr64(Mnemonic.fneg_h, Fd,F1),
                            FpInstr64(Mnemonic.fsgnjn_h, Fd,F1, F2))),
                        (0x2, Select(R1EqR2,
                            FpInstr64(Mnemonic.fabs_h, Fd,F1),
                            FpInstr64(Mnemonic.fsgnjx_h, Fd,F1, F2))))),
                    ( 0x13, Sparse(12, 3, "fsgn.q", invalid,
                        (0x0, Select(R1EqR2,
                            FpInstr128(Mnemonic.fmv_q, Fd,F1, F2),
                            FpInstr128(Mnemonic.fsgnj_q, Fd,F1, F2))),
                        (0x1, Select(R1EqR2,
                            FpInstr128(Mnemonic.fneg_q, Fd,F1, F2),
                            FpInstr128(Mnemonic.fsgnjn_q, Fd,F1, F2))),
                        (0x2, Select(R1EqR2,
                            FpInstr128(Mnemonic.fabs_q, Fd,F1, F2),
                            FpInstr128(Mnemonic.fsgnjx_q, Fd,F1, F2))))),

                    ( 0x14, Sparse(12, 3, "fmin/fmax.s", invalid,
                        (0x0, FpInstr32(Mnemonic.fmin_s, Fd,F1, F2)),
                        (0x1, FpInstr32(Mnemonic.fmax_s, Fd,F1, F2)),
                        (0x2, FpInstr32(Mnemonic.fminm_s, Fd,F1, F2)),
                        (0x3, FpInstr32(Mnemonic.fmaxm_s, Fd,F1, F2)))),
                    ( 0x15, Sparse(12, 3, "fmin/fmax.d", invalid,
                        (0x0, FpInstr64(Mnemonic.fmin_d, Fd,F1, F2)),
                        (0x1, FpInstr64(Mnemonic.fmax_d, Fd,F1, F2)),
                        (0x2, FpInstr64(Mnemonic.fminm_d, Fd,F1, F2)),
                        (0x3, FpInstr64(Mnemonic.fmaxm_d, Fd,F1, F2)))),
                    ( 0x16, Sparse(12, 3, "fmin/fmax.h", invalid,
                        (0x0, FpInstr32(Mnemonic.fmin_h, Fd,F1, F2)),
                        (0x1, FpInstr32(Mnemonic.fmax_h, Fd,F1, F2)),
                        (0x2, FpInstr32(Mnemonic.fminm_h, Fd,F1, F2)),
                        (0x3, FpInstr32(Mnemonic.fmaxm_h, Fd,F1, F2)))),
                    ( 0x17, Sparse(12, 3, "fmin/fmax.q", invalid,
                        (0x0, FpInstr128(Mnemonic.fmin_q, Fd,F1, F2)),
                        (0x1, FpInstr128(Mnemonic.fmax_q, Fd,F1, F2)),
                        (0x2, FpInstr128(Mnemonic.fminm_q, Fd,F1, F2)),
                        (0x3, FpInstr128(Mnemonic.fmaxm_q, Fd,F1, F2)))),

                    ( 0x20, Sparse(20, 5, "fcvt.s", invalid,
                        (0x1, FpInstr32(Mnemonic.fcvt_s_d, Fd,F1, rm12) ),
                        (0x2, FpInstr32(Mnemonic.fcvt_s_h, Fd,F1) ),
                        (0x3, FpInstr32(Mnemonic.fcvt_s_q, Fd,F1, rm12) ),
                        (0x4, FpInstr32(Mnemonic.fround_s, Fd,F1, rm12) ),
                        (0x5, FpInstr32(Mnemonic.froundnx_s, Fd,F1, rm12) ))),
                    ( 0x21, Sparse(20, 5, "fcvt.d", invalid,
                        (0x0, FpInstr64(Mnemonic.fcvt_d_s, Fd,F1) ),
                        (0x2, FpInstr64(Mnemonic.fcvt_d_h, Fd,F1) ),
                        (0x3, FpInstr64(Mnemonic.fcvt_d_q, Fd,F1, rm12) ),
                        (0x4, FpInstr64(Mnemonic.fround_d, Fd,F1, rm12) ),
                        (0x5, FpInstr64(Mnemonic.froundnx_d, Fd,F1, rm12) ))),
                    ( 0x22, Sparse(20, 5, "fcvt.h", invalid,
                        (0x0, FpInstr16(Mnemonic.fcvt_h_s, Fd,F1, rm12)),
                        (0x1, FpInstr16(Mnemonic.fcvt_h_d, Fd,F1, rm12)),
                        (0x3, FpInstr16(Mnemonic.fcvt_h_q, Fd,F1, rm12)),
                        (0x4, FpInstr16(Mnemonic.fround_h, Fd,F1, rm12) ),
                        (0x5, FpInstr16(Mnemonic.froundnx_h, Fd,F1, rm12) ))),

                    ( 0x23, Sparse(20, 5, "fcvt.q", invalid,
                        (0x0, FpInstr128(Mnemonic.fcvt_q_s, Fd,F1) ),
                        (0x1, FpInstr128(Mnemonic.fcvt_q_d, Fd,F1) ),
                        (0x2, FpInstr128(Mnemonic.fcvt_q_h, Fd,F1) ),
                        (0x4, FpInstr128(Mnemonic.fround_q, Fd,F1, rm12) ),
                        (0x5, FpInstr128(Mnemonic.froundnx_q, Fd,F1, rm12) ))),

                    ( 0x2C, Select((20, 5), Ne0, invalid, FpInstr32(Mnemonic.fsqrt_s, Fd,F1, rm12)) ),
                    ( 0x2D, Select((20, 5), Ne0, invalid, FpInstr64(Mnemonic.fsqrt_d, Fd,F1, rm12)) ),
                    ( 0x2E, Select((20, 5), Ne0, invalid, FpInstr16(Mnemonic.fsqrt_h, Fd,F1, rm12)) ),
                    ( 0x2F, Select((20, 5), Ne0, invalid, FpInstr128(Mnemonic.fsqrt_q, Fd,F1, rm12)) ),

                    ( 0x50, Sparse(12, 3, "  0x50", invalid,
                        ( 0, FpInstr32(Mnemonic.fle_s, rd,F1,F2)),
                        ( 1, FpInstr32(Mnemonic.flt_s, rd,F1,F2)),
                        ( 2, FpInstr32(Mnemonic.feq_s, rd,F1,F2)))),
                    ( 0x51, Sparse(12, 3, "  0x51", invalid,
                        ( 0, FpInstr64(Mnemonic.fle_d, rd,F1,F2)),
                        ( 1, FpInstr64(Mnemonic.flt_d, rd,F1,F2)),
                        ( 2, FpInstr64(Mnemonic.feq_d, rd,F1,F2)))),
                    ( 0x52, Sparse(12, 3, "  0x52", invalid,
                        ( 0, FpInstr16(Mnemonic.fle_h, rd,F1,F2)),
                        ( 1, FpInstr16(Mnemonic.flt_h, rd,F1,F2)),
                        ( 2, FpInstr16(Mnemonic.feq_h, rd,F1,F2)))),
                    ( 0x53, Sparse(12, 3, "  0x53", invalid,
                        ( 0, FpInstr128(Mnemonic.fle_q, rd,F1,F2)),
                        ( 1, FpInstr128(Mnemonic.flt_q, rd,F1,F2)),
                        ( 2, FpInstr128(Mnemonic.feq_q, rd,F1,F2)))),

                    ( 0x60, Sparse(20, 5, "  0x60", invalid,
                        ( 0, FpInstr32(Mnemonic.fcvt_w_s, Rd,F1,rm12)),
                        ( 1, FpInstr32(Mnemonic.fcvt_wu_s, Rd,F1,rm12)),
                        ( 2, FpInstr32(Mnemonic.fcvt_l_s, Rd,F1,rm12)),
                        ( 3, FpInstr32(Mnemonic.fcvt_lu_s, Rd,F1,rm12)))),
                    ( 0x61, Sparse(20, 5, "  0x61", invalid,
                        ( 0, FpInstr64(Mnemonic.fcvt_w_d, Rd,F1,rm12)),
                        ( 1, FpInstr64(Mnemonic.fcvt_wu_d, Rd,F1,rm12)),
                        ( 2, FpInstr64(Mnemonic.fcvt_l_d, Rd,F1,rm12)),
                        ( 3, FpInstr64(Mnemonic.fcvt_lu_d, Rd,F1,rm12)))),
                    ( 0x62, Sparse(20, 5, "  0x62", invalid,
                        ( 0, FpInstr16(Mnemonic.fcvt_w_h, Rd,F1,rm12)),
                        ( 1, FpInstr16(Mnemonic.fcvt_wu_h, Rd,F1,rm12)),
                        ( 2, FpInstr16(Mnemonic.fcvt_l_h, Rd,F1,rm12)),
                        ( 3, FpInstr16(Mnemonic.fcvt_lu_h, Rd,F1,rm12)))),
                    ( 0x63, Sparse(20, 5, "  0x63", invalid,
                        ( 0, FpInstr128(Mnemonic.fcvt_w_q, Rd,F1,rm12)),
                        ( 1, FpInstr128(Mnemonic.fcvt_wu_q, Rd,F1,rm12)),
                        ( 2, FpInstr128(Mnemonic.fcvt_l_q, Rd,F1,rm12)),
                        ( 3, FpInstr128(Mnemonic.fcvt_lu_q, Rd,F1,rm12)))),

                    ( 0x68, Sparse(20, 5, "fcvt.to.s", invalid,
                        ( 0, FpInstr32(Mnemonic.fcvt_s_w, Fd,R1,rm12)),
                        ( 1, FpInstr32(Mnemonic.fcvt_s_wu, Fd,R1,rm12)),
                        ( 2, FpInstr32(Mnemonic.fcvt_s_l, Fd,R1,rm12)),
                        ( 3, FpInstr32(Mnemonic.fcvt_s_lu, Fd,R1,rm12)))),
                    ( 0x69, Sparse(20, 5, "fcvt.to.d", invalid,
                        ( 0, FpInstr64(Mnemonic.fcvt_d_w, Fd,R1)),
                        ( 1, FpInstr64(Mnemonic.fcvt_d_wu, Fd,R1)),
                        ( 2, FpInstr64(Mnemonic.fcvt_d_l, Fd,R1,rm12)),
                        ( 3, FpInstr64(Mnemonic.fcvt_d_lu, Fd,R1,rm12)))),
                    ( 0x6A, Sparse(20, 5, "fcvt.to.h", invalid,
                        ( 0, FpInstr16(Mnemonic.fcvt_h_w, Fd,R1,rm12)),
                        ( 1, FpInstr16(Mnemonic.fcvt_h_wu, Fd,R1,rm12)),
                        ( 2, FpInstr16(Mnemonic.fcvt_h_l, Fd,R1,rm12)),
                        ( 3, FpInstr16(Mnemonic.fcvt_h_lu, Fd,R1,rm12)))),
                    ( 0x6B, Sparse(20, 5, "fcvt.to.q", invalid,
                        ( 0, FpInstr128(Mnemonic.fcvt_q_w, Fd,R1)),
                        ( 1, FpInstr128(Mnemonic.fcvt_q_wu, Fd,R1)),
                        ( 2, FpInstr128(Mnemonic.fcvt_q_l, Fd,R1,rm12)),
                        ( 3, FpInstr128(Mnemonic.fcvt_q_lu, Fd,R1,rm12)))),
                    ( 0x70, Sparse(12, 3, "  0x70", invalid,
                        (0, FpInstr32(Mnemonic.fmv_x_w, Rd,F1) ),
                        (1, FpInstr32(Mnemonic.fclass_s, Rd,F1) ))),
                    ( 0x71, Sparse(12, 3, "  0x71", invalid,
                        (0, FpInstr64(Mnemonic.fmv_x_d, Rd,F1) ),
                        (1, FpInstr64(Mnemonic.fclass_d, Rd,F1) ))),
                    ( 0x72, Sparse(12, 3, "  0x72", invalid,
                        (0, FpInstr16(Mnemonic.fmv_x_h, Rd,F1) ),
                        (1, FpInstr16(Mnemonic.fclass_h, Rd,F1) ))),
                    ( 0x73, Sparse(12, 3, "fclass.q", invalid,
                        // (0, FpInstr128(Mnemonic.fmv_x_q, Rd,F1) ), //$TODO: this will be part of RV128
                        ( 1, FpInstr128(Mnemonic.fclass_q, Rd,F1)))),
                    ( 0x78, Sparse(20, 5, "  0x78", invalid,
                        (0, FpInstr32(Mnemonic.fmv_w_x, Fd,r1) ),
                        (1, Zfa(FpInstr32(Mnemonic.fli_s, Fd, fpImm_s))))),
                    ( 0x79, Sparse(20, 5, "  0x79", invalid,
                        (0, FpInstr64(Mnemonic.fmv_d_x, Fd,r1) ),
                        (1, Zfa(FpInstr64(Mnemonic.fli_d, Fd, fpImm_d))))),
                    ( 0x7A, Sparse(20, 5, "  0x7A", invalid,
                        (0, FpInstr16(Mnemonic.fmv_h_x, Fd,r1) ),
                        (1, Zfa(FpInstr16(Mnemonic.fli_h, Fd, fpImm_d))))),
                    ( 0x7B, Sparse(20, 5, "  0x7B", invalid,
                        (1, Zfa(FpInstr128(Mnemonic.fli_q, Fd, fpImm_d))))),
                };

                var branches = new Decoder[]            // 0b11000
                {
                    Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, r1,r2,B),
                    Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, r1,r2,B),
                    Nyi(""),
                    Nyi(""),

                    Instr(Mnemonic.blt,  InstrClass.ConditionalTransfer, r1,r2,B),
                    Instr(Mnemonic.bge,  InstrClass.ConditionalTransfer, r1,r2,B),
                    Instr(Mnemonic.bltu, InstrClass.ConditionalTransfer, r1,r2,B),
                    Instr(Mnemonic.bgeu, InstrClass.ConditionalTransfer, r1,r2,B),
                };

                var amo = Mask(12, 3, "  AMO",
                    Nyi("amo - 000"),
                    Nyi("amo - 001"),
                    Sparse(27, 5, "  AMO",
                        Nyi("amo - 010"),
                        (0x02, Instr(Mnemonic.lr_w, aq_rl, rd, Mem(PrimitiveType.Word32, 15))),
                        (0x03, Instr(Mnemonic.sc_w, aq_rl, rd, r2, Mem(PrimitiveType.Word32, 15))),
                        (0x01, Instr(Mnemonic.amoswap_w, aq_rl, rd, r2, Mem(PrimitiveType.Word32, 15))),
                        (0x00, Instr(Mnemonic.amoadd_w, aq_rl, rd, r2, Mem(PrimitiveType.Word32, 15))),
                        (0x04, Instr(Mnemonic.amoxor_w, aq_rl, rd, r2, Mem(PrimitiveType.Word32, 15))),
                        (0x0C, Instr(Mnemonic.amoand_w, aq_rl, rd, r2, Mem(PrimitiveType.Word32, 15))),
                        (0x08, Instr(Mnemonic.amoor_w, aq_rl, rd, r2, Mem(PrimitiveType.Word32, 15))),
                        (0x10, Instr(Mnemonic.amomin_w, aq_rl, rd, r2, Mem(PrimitiveType.Int32, 15))),
                        (0x14, Instr(Mnemonic.amomax_w, aq_rl, rd, r2, Mem(PrimitiveType.Int32, 15))),
                        (0x18, Instr(Mnemonic.amominu_w, aq_rl, rd, r2, Mem(PrimitiveType.UInt32, 15))),
                        (0x1C, Instr(Mnemonic.amomaxu_w, aq_rl, rd, r2, Mem(PrimitiveType.UInt32, 15)))),
                    Sparse(27, 5, "  AMO",
                        Nyi("amo - 011"),
                        (0x02, Instr(Mnemonic.lr_d, aq_rl, rd, Mem(PrimitiveType.Word64, 15))),
                        (0x03, Instr(Mnemonic.sc_d, aq_rl, rd, r2, Mem(PrimitiveType.Word64, 15))),
                        (0x01, Instr(Mnemonic.amoswap_d, aq_rl, rd, r2, Mem(PrimitiveType.Word64, 15))),
                        (0x00, Instr(Mnemonic.amoadd_d, aq_rl, rd, r2, Mem(PrimitiveType.Word64, 15))),
                        (0x04, Instr(Mnemonic.amoxor_d, aq_rl, rd, r2, Mem(PrimitiveType.Word64, 15))),
                        (0x0C, Instr(Mnemonic.amoand_d, aq_rl, rd, r2, Mem(PrimitiveType.Word64, 15))),
                        (0x08, Instr(Mnemonic.amoor_d, aq_rl, rd, r2, Mem(PrimitiveType.Word64, 15))),
                        (0x10, Instr(Mnemonic.amomin_d, aq_rl, rd, r2, Mem(PrimitiveType.Int64, 15))),
                        (0x14, Instr(Mnemonic.amomax_d, aq_rl, rd, r2, Mem(PrimitiveType.Int64, 15))),
                        (0x18, Instr(Mnemonic.amominu_d, aq_rl, rd, r2, Mem(PrimitiveType.UInt64, 15))),
                        (0x1C, Instr(Mnemonic.amomaxu_d, aq_rl, rd, r2, Mem(PrimitiveType.UInt64, 15)))),
                    Nyi("amo - 100"),
                    Nyi("amo - 101"),
                    Nyi("amo - 110"),
                    Nyi("amo - 111"));

                var system = Mask(12, 3, "  system",
                    Sparse(25, 7, "  system 000",  
                        Nyi("system 000"),
                        (0x00, Sparse(20, 5, "  system 0x00", Nyi("system 0x00"),
                            (0, Instr(Mnemonic.ecall, InstrClass.Transfer | InstrClass.Call)),
                            (1, Instr(Mnemonic.ebreak, InstrClass.Terminates)),
                            (2, Instr(Mnemonic.uret, InstrClass.Transfer | InstrClass.Return)))),
                        (0x08, Sparse(20, 5, "  system 0x08", Nyi("system 0x08"),
                            (2, Instr(Mnemonic.sret, InstrClass.Privileged | InstrClass.Transfer | InstrClass.Return)),
                            (4, Instr(Mnemonic.sfence_vm, InstrClass.Privileged | InstrClass.Linear, r1)),
                            (5, Instr(Mnemonic.wfi, InstrClass.Privileged | InstrClass.Linear)))),
                        (0x09, Instr(Mnemonic.sfence_vma, InstrClass.Privileged | InstrClass.Linear, r1, r2)),
                        (0x0B, Instr(Mnemonic.sinval_vma, InstrClass.Privileged | InstrClass.Linear, r1, r2)),
                        (0x0C, Sparse(20, 5, "  system 0x0C", Nyi("system 0x0C"),
                            (0, Instr(Mnemonic.sfence_w_inval, InstrClass.Privileged | InstrClass.Linear)),
                            (1, Instr(Mnemonic.sfence_inval_ir, InstrClass.Privileged | InstrClass.Linear)))),
                        (0x11, Instr(Mnemonic.hfence_vvma, InstrClass.Privileged | InstrClass.Linear, r1, r2)),
                        (0x13, Instr(Mnemonic.hinval_vvma, InstrClass.Privileged | InstrClass.Linear, r1, r2)),
                        (0x31, Instr(Mnemonic.hfence_gvma, InstrClass.Privileged | InstrClass.Linear, r1, r2)),
                        (0x33, Instr(Mnemonic.hinval_gvma, InstrClass.Privileged | InstrClass.Linear, r1, r2)),

                        (0x18, Sparse(20, 5, "  system 11000", Nyi("system 11000"),
                            (2, Instr(Mnemonic.mret, InstrClass.Privileged | InstrClass.Transfer | InstrClass.Return))))),

                    Instr(Mnemonic.csrrw, rd, Csr20, r1),
                    Instr(Mnemonic.csrrs, rd, Csr20, r1),
                    Instr(Mnemonic.csrrc, rd, Csr20, r1),
                    Sparse(25, 7, "  system 100", 
                        Nyi("system 100"),
                        (0x30, Sparse(20, 5, " system 0x30", Nyi("system 0x30"),
                            (0, Instr(Mnemonic.hlv_b, InstrClass.Privileged | InstrClass.Linear, Rd, Mem(PrimitiveType.Int8, 15))),
                            (1, Instr(Mnemonic.hlv_bu, InstrClass.Privileged | InstrClass.Linear, Rd, Mem(PrimitiveType.UInt8, 15))))),
                        (0x32, Sparse(20, 5, " system 0x32", Nyi("system 0x32"),
                            (0, Instr(Mnemonic.hlv_h, InstrClass.Privileged | InstrClass.Linear, Rd, Mem(PrimitiveType.Int16, 15))),
                            (1, Instr(Mnemonic.hlv_hu, InstrClass.Privileged | InstrClass.Linear, Rd, Mem(PrimitiveType.UInt16, 15))),
                            (3, Instr(Mnemonic.hlvx_hu, InstrClass.Privileged | InstrClass.Linear, Rd, Mem(PrimitiveType.UInt16, 15))))),
                        (0x34, Sparse(20, 5, " system 0x34", Nyi("system 0x34"),
                            (0, Instr(Mnemonic.hlv_w, InstrClass.Privileged | InstrClass.Linear, Rd, Mem(PrimitiveType.Int32, 15))),
                            (1, Instr(Mnemonic.hlv_wu, InstrClass.Privileged | InstrClass.Linear, Rd, Mem(PrimitiveType.UInt32, 15))),
                            (3, Instr(Mnemonic.hlvx_wu, InstrClass.Privileged | InstrClass.Linear, Rd, Mem(PrimitiveType.UInt32, 15))))),
                        (0x36, Sparse(20, 5, " system 0x36", Nyi("system 0x36"),
                            (0, Instr(Mnemonic.hlv_d, InstrClass.Privileged | InstrClass.Linear, Rd, Mem(PrimitiveType.UInt64, 15))))),
                        (0x31, Instr(Mnemonic.hsv_b, InstrClass.Privileged | InstrClass.Linear, r2, Mem(PrimitiveType.Byte, 15))),
                        (0x33, Instr(Mnemonic.hsv_h, InstrClass.Privileged | InstrClass.Linear, r2, Mem(PrimitiveType.Word16, 15))),
                        (0x35, Instr(Mnemonic.hsv_w, InstrClass.Privileged | InstrClass.Linear, r2, Mem(PrimitiveType.Word32, 15))),
                        (0x37, Instr(Mnemonic.hsv_d, InstrClass.Privileged | InstrClass.Linear, r2, Mem(PrimitiveType.Word64, 15)))),
                    Instr(Mnemonic.csrrwi, rd, Csr20, Imm(15, 5)),
                    Instr(Mnemonic.csrrsi, rd, Csr20, Imm(15, 5)),
                    Instr(Mnemonic.csrrci, rd, Csr20, Imm(15, 5)));

                // These long instructions have not been defined yet.
                var instr48bit = invalid;
                var instr64bit = invalid;
                var instr80bit = invalid;

                var w32decoders = Mask(2, 5, "w32decoders",
                    // 00
                    Mask(12, 3, "loads", loads),
                    Mask(12, 3, "fploads", fploads),
                    Nyi("custom-0"),
                    Mask(12, 3, "misc-mem", miscMem),

                    Mask(12, 3, "opimm", opimm),
                    Instr(Mnemonic.auipc, rd, Iu),
                    Mask(12, 3, "opimm32", opimm32),
                    instr48bit,

                    Mask(12, 3, "stores", stores),
                    Mask(12, 3, "fpstores", fpstores),
                    Nyi("custom-1"),
                    amo,

                    op,
                    Instr(Mnemonic.lui, rd, Iu),
                    op32,
                    instr64bit,

                    // 10
                    Mask(25, 2, " fmadd",
                        FpInstr32(Mnemonic.fmadd_s, Fd, F1, F2, F3, rm12),
                        FpInstr64(Mnemonic.fmadd_d, Fd, F1, F2, F3, rm12),
                        FpInstr16(Mnemonic.fmadd_h, Fd, F1, F2, F3, rm12),
                        FpInstr128(Mnemonic.fmadd_q, Fd, F1, F2, F3, rm12)),
                    Mask(25, 2, " fmsub",
                        FpInstr32(Mnemonic.fmsub_s, Fd, F1, F2, F3, rm12),
                        FpInstr64(Mnemonic.fmsub_d, Fd, F1, F2, F3, rm12),
                        FpInstr16(Mnemonic.fmsub_h, Fd, F1, F2, F3, rm12),
                        FpInstr128(Mnemonic.fmsub_q, Fd, F1, F2, F3, rm12)),
                    Mask(25, 2, " fnmsub",
                        FpInstr32(Mnemonic.fnmsub_s, Fd, F1, F2, F3, rm12),
                        FpInstr64(Mnemonic.fnmsub_d, Fd, F1, F2, F3, rm12),
                        FpInstr16(Mnemonic.fnmsub_h, Fd, F1, F2, F3, rm12),
                        FpInstr128(Mnemonic.fnmsub_q, Fd, F1, F2, F3, rm12)),
                    Mask(25, 2, " fnmadd",
                        FpInstr32(Mnemonic.fnmadd_s, Fd, F1, F2, F3, rm12),
                        FpInstr64(Mnemonic.fnmadd_d, Fd, F1, F2, F3, rm12),
                        FpInstr16(Mnemonic.fnmadd_h, Fd, F1, F2, F3, rm12),
                        FpInstr128(Mnemonic.fnmadd_q, Fd, F1, F2, F3, rm12)),

                    Sparse(25, 7, invalid, opfp),
                    Nyi("Reserved"),
                    Nyi("custom-2"),
                    instr48bit,

                    Mask(12, 3, "branches", branches),
                    Instr(Mnemonic.jalr, InstrClass.Transfer | InstrClass.Return, rd, r1, i),
                    Nyi("Reserved"),
                    Instr(Mnemonic.jal, InstrClass.Transfer | InstrClass.Call, Rd, J),

                    system,
                    Nyi("Reserved"),
                    Nyi("custom-3"),
                    instr80bit);

                var bf_7_2_9_4 = Bf((7,2), (9,4));
                var bf_12_1_2_5 = Bf((12,1), (2,5));
                var bf_2_2_12_1_4_3 = Bf((2,2), (12,1), (4,3));
                var bf_5_2_10_3 = Bf((5,2), (10,3));
                var bf_2_3_12_1_5_2 = Bf((2,3), (12,1), (5,2));
                var bf_5_1_10_3_6_1 = Bf((5,1), (10,3), (6,1));
                var bf_7_3_10_3 = Bf((7,3), (10,3));
                var bf_12_1_5_2_2_1_10_2_3_2 = Bf((12,1), (5,2), (2,1), (10,2), (3, 2));

                var compressed00 = new Decoder[8]
                {
                    Select((0, 16), Ne0, "zero",
                        Instr(Mnemonic.c_addi4spn, Rc(2), Rsp, ImmSh(2, (7,4), (11,2), (5,1), (6,1))),
                        Instr(Mnemonic.invalid, InstrClass.Invalid|InstrClass.Zero)),
                    WordSize(
                        rv32: FpInstr32(Mnemonic.c_fld, Fc(2), Memc(PrimitiveType.Real64, 7, bf_5_2_10_3)),
                        rv64: FpInstr64(Mnemonic.c_fld, Fc(2), Memc(PrimitiveType.Real64, 7, bf_5_2_10_3)),
                        rv128: Nyi("lq")),
                    Instr(Mnemonic.c_lw, Rc(2), Memc(PrimitiveType.Word32, 7, bf_5_1_10_3_6_1)),
                    WordSize(
                        rv32: FpInstr32(Mnemonic.c_flw, Fc(2), Memc(PrimitiveType.Real32, 7, bf_5_1_10_3_6_1)),
                        rv64: Instr(Mnemonic.c_ld, Rc(2), Memc(PrimitiveType.Word64, 7, bf_5_2_10_3)),
                        rv128: Instr(Mnemonic.c_ld, Rc(2), Memc(PrimitiveType.Word64, 7, bf_5_2_10_3))),
                    invalid, // Nyi("reserved"),
                    WordSize(
                        rv32: FpInstr64(Mnemonic.c_fsd, Fc(2), Memc(PrimitiveType.Real64, 7, bf_5_2_10_3)),
                        rv64: FpInstr64(Mnemonic.c_fsd, Fc(2), Memc(PrimitiveType.Real64, 7, bf_5_2_10_3)),
                        rv128: Nyi("sq")),
                    Instr(Mnemonic.c_sw, Rc(2), Memc(PrimitiveType.Word32, 7, bf_5_1_10_3_6_1)),
                    WordSize(
                        rv32: Instr(Mnemonic.c_fsw, Rc(2), Memc(PrimitiveType.Word32, 7, bf_5_1_10_3_6_1)),
                        rv64: Instr(Mnemonic.c_sd, Rc(2), Memc(PrimitiveType.Real64, 7, bf_5_2_10_3)),
                        rv128: Instr(Mnemonic.c_sd, Rc(2), Memc(PrimitiveType.Real64, 7, bf_5_2_10_3))),
                };

                var compressed01 = new Decoder[8]
                {
                    Select((7,5), Eq0, "  c.addi",
                        Select(u => u == 0x0001,
                            Instr(Mnemonic.c_nop, InstrClass.Linear|InstrClass.Padding),
                            invalid),
                        Instr(Mnemonic.c_addi, R(7), ImmS(bf_12_1_2_5))),
                    WordSize(
                        rv32: Instr(Mnemonic.c_jal, InstrClass.Transfer|InstrClass.Call, Jc),
                        rv64: Instr(Mnemonic.c_addiw, R_nz(7), ImmS(bf_12_1_2_5)),
                        rv128: Instr(Mnemonic.c_addiw, R(7), ImmS(bf_12_1_2_5))),
                    Instr(Mnemonic.c_li, R(7), ImmS(bf_12_1_2_5)),
                    Select((7, 5), u => u == 2,
                        Instr(Mnemonic.c_addi16sp, Rsp, ImmShS(4, (12,1), (3,2), (5,1), (2,1), (6, 1))),
                        Instr(Mnemonic.c_lui, R(7), ImmShS(12, bf_12_1_2_5))),

                    Mask(10, 2, "comp1",
                        Select(bf_12_1_2_5, u => u == 0, "",
                            Instr(Mnemonic.c_srli64, Rc(7)),
                            Instr(Mnemonic.c_srli, Rc(7), Imm(bf_12_1_2_5))
                        ),
                        Select(bf_12_1_2_5, u => u == 0, "",
                            Instr(Mnemonic.c_srai64, Rc(7)),
                            Instr(Mnemonic.c_srai, Rc(7), Imm(bf_12_1_2_5))
                        ),
                        Instr(Mnemonic.c_andi, Rc(7), ImmS(bf_12_1_2_5)),
                        Mask(12, 1, "comp1_1",
                            Mask(5, 2, "comp1_1_1",
                                Instr(Mnemonic.c_sub, Rc(7), Rc(2)),
                                Instr(Mnemonic.c_xor, Rc(7), Rc(2)),
                                Instr(Mnemonic.c_or, Rc(7), Rc(2)),
                                Instr(Mnemonic.c_and, Rc(7), Rc(2))),
                            Mask(5, 2, "comp1_1_2",
                                WordSize(
                                    rv64: Instr(Mnemonic.c_subw, Rc(7),Rc(2)),
                                    rv128: Instr(Mnemonic.c_subw, Rc(7),Rc(2))),
                                WordSize(
                                    rv64: Instr(Mnemonic.c_addw, Rc(7),Rc(2)),
                                    rv128: Instr(Mnemonic.c_addw, Rc(7),Rc(2))),
                                invalid,
                                invalid))),

                    Instr(Mnemonic.c_j, InstrClass.Transfer, Jc),
                    Instr(Mnemonic.c_beqz, InstrClass.ConditionalTransfer, Rc(7), PcRel(1, bf_12_1_5_2_2_1_10_2_3_2)),
                    Instr(Mnemonic.c_bnez, InstrClass.ConditionalTransfer, Rc(7), PcRel(1, bf_12_1_5_2_2_1_10_2_3_2)),
                };

                var compressed10 = new Decoder[8]
                {
                    Select(bf_12_1_2_5, u => u == 0, "",
                        Instr(Mnemonic.c_slli64, R_nz(7)),
                        Instr(Mnemonic.c_slli, R_nz(7), ImmB(bf_12_1_2_5))
                    ),
                    WordSize(
                        rv32: FpInstr64(Mnemonic.c_fldsp, F(7), MemcSpRel(PrimitiveType.Word64, bf_2_3_12_1_5_2)),
                        rv64: FpInstr64(Mnemonic.c_fldsp, F(7), MemcSpRel(PrimitiveType.Word64, bf_2_3_12_1_5_2)),
                        rv128: Instr(Mnemonic.c_lqsp, R_nz(7), MemcSpRel(PrimitiveType.Word128,  (2,4),(12,1),(6,1)))
                    ),
                    Instr(Mnemonic.c_lwsp, R_nz(7), MemcSpRel(PrimitiveType.Word32, bf_2_2_12_1_4_3)),
                    WordSize(
                        rv32: FpInstr32(Mnemonic.c_flwsp, F(7), MemcSpRel(PrimitiveType.Word32, bf_2_2_12_1_4_3)),
                        rv64: Instr(Mnemonic.c_ldsp, R_nz(7), MemcSpRel(PrimitiveType.Word64, bf_2_3_12_1_5_2)),
                        rv128: Instr(Mnemonic.c_ldsp, R_nz(7), MemcSpRel(PrimitiveType.Word64, bf_2_3_12_1_5_2))
                    ),
                    Mask(12, 1,  "",
                        Select((2, 5), u => u == 0, "",
                            Instr(Mnemonic.c_jr, InstrClass.Transfer, R(7), useLr(7)),
                            Instr(Mnemonic.c_mv, R(7), R(2))),
                        Select((2, 5), Eq0,
                            Select((7, 5), Eq0,
                                Instr(Mnemonic.c_ebreak, InstrClass.Terminates),
                                Instr(Mnemonic.c_jalr, InstrClass.Transfer|InstrClass.Return, R(7))),
                            Instr(Mnemonic.c_add, R(7), R(2)))),
                    WordSize(
                        rv32: FpInstr64(Mnemonic.c_fsdsp, F(2), MemcSpRel(PrimitiveType.Word64, bf_7_3_10_3)),
                        rv64: FpInstr64(Mnemonic.c_fsdsp, F(2), MemcSpRel(PrimitiveType.Word64, bf_7_3_10_3)),
                        rv128:Nyi("sqsp")),
                    Instr(Mnemonic.c_swsp, R(2), MemcSpRel(PrimitiveType.Word32, bf_7_2_9_4)),
                    WordSize(
                        rv32: FpInstr64(Mnemonic.c_fswsp, F(2), MemcSpRel(PrimitiveType.Word32, bf_7_2_9_4)),
                        rv64: Instr(Mnemonic.c_sdsp, R(2), MemcSpRel(PrimitiveType.Word64, bf_7_3_10_3)),
                        rv128:Instr(Mnemonic.c_sdsp, R(2), MemcSpRel(PrimitiveType.Word64, bf_7_3_10_3))
                    )
                };

                return new Decoder[4]
                {
                    Mask(13, 3, "compressed00", compressed00),
                    Mask(13, 3, "compressed01", compressed01),
                    Mask(13, 3, "compressed10", compressed10),
                    new W32Decoder(w32decoders)
                };
            }
        }
    }
}
