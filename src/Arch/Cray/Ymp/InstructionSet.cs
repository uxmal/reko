#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System.Text;
using static Reko.Arch.Cray.Ymp.YmpDisassembler;

namespace Reko.Arch.Cray.Ymp
{
    using Decoder = Decoder<YmpDisassembler, Mnemonic, CrayInstruction>;
    using static Reko.Core.Machine.DisassemblerBase<CrayInstruction, Mnemonic>;

    public abstract class InstructionSet
    {
        protected readonly Decoder<YmpDisassembler, Mnemonic, CrayInstruction> invalid;

        public InstructionSet()
        {
            this.invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
        }

        #region Decoders

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<YmpDisassembler>[] mutators)
        {
            return new InstrDecoder<YmpDisassembler, Mnemonic, CrayInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<YmpDisassembler>[] mutators)
        {
            return new InstrDecoder<YmpDisassembler, Mnemonic, CrayInstruction>(iclass, mnemonic, mutators);
        }


        /// <summary>
        /// Creates a decoder for either the extended C90 or the base Ymp instruction set.
        /// </summary>
        /// <param name="c90decoder">Decoder to use if this is C90.</param>
        /// <param name="ympDecoder">Decoder to use if this is Ymp.</param>
        /// <returns>A new decoder.</returns>
        protected abstract Decoder C90(Decoder c90decoder, Decoder ympDecoder);

        /// <summary>
        /// Creates a decoder for the extended C90 instruction set, or an invalid decoder.
        /// </summary>
        /// <param name="c90decoder">Decoder to use if this is C90.</param>
        protected abstract Decoder C90(Decoder c90decoder);

        //private static Decoder Select((int, int) field, Predicate<uint> predicate, string tag, 
        //    Decoder trueDecoder,
        //    Decoder falseDecoder)
        //{
        //    var fields = DisassemblerBase.Bf(field);
        //    return new Core.Machine.ConditionalDecoder<YmpDisassembler, Mnemonic, CrayInstruction>(fields, predicate, tag, trueDecoder, falseDecoder);
        //}

        private static NyiDecoder<YmpDisassembler, Mnemonic, CrayInstruction> Nyi(string message)
        {
            return new NyiDecoder<YmpDisassembler, Mnemonic, CrayInstruction>(message);
        }

        #endregion

        public Decoder<YmpDisassembler, Mnemonic, CrayInstruction> CreateDecoder()
        {
            var opc_001 = Select((0, 9), Is0, "  001",
                Instr(Mnemonic.pass, InstrClass.Linear | InstrClass.Padding),
                Nyi("001"));

            var opc_002 = Select((3, 6), Is0, "  002",
                Instr(Mnemonic._mov, Reg(Registers.vl), Ak),
                Nyi("002"));

            var opc_004 = Select((0, 9), Is0, "  004",
                Instr(Mnemonic.ex, InstrClass.Terminates),
                invalid);

            var opc_005 = Select((6, 3), Is0, "  005x",
                    Instr(Mnemonic.j, InstrClass.Transfer, Bjk),
                    invalid);

            var opc_006 = C90(
                Select((0, 8), Is0, "  006 0",
                    Instr(Mnemonic.j, InstrClass.Transfer, Jnm),
                    Mask(5, 1, " 0 006 4",
                        Instr(Mnemonic.jts, InstrClass.Transfer, Ijk, Jnm),
                        Instr(Mnemonic.jts, InstrClass.Transfer, Ak, Jnm))),
                Mask(8, 1, "  006",
                    Instr(Mnemonic.j, InstrClass.Transfer, Jijkm),
                    invalid));

            var opc_007 = C90(Select((0, 9), Is0, "  007",
                    Instr(Mnemonic.r, InstrClass.Call | InstrClass.Transfer, Jnm),
                    invalid),
                Instr(Mnemonic.r, InstrClass.Call | InstrClass.Transfer, Jijkm));

            var opc_010 = C90(Select((0, 9), Is0, "  010",
                    Instr(Mnemonic.jaz, InstrClass.ConditionalTransfer, Jnm),
                    invalid),
                Instr(Mnemonic.jaz, InstrClass.ConditionalTransfer, Jijkm));

            var opc_011 = C90(Select((0, 9), Is0, "  011",
                    Instr(Mnemonic.jan, InstrClass.ConditionalTransfer, Jnm),
                    invalid),
                Instr(Mnemonic.jan, InstrClass.ConditionalTransfer, Jijkm));

            var opc_012 = C90(Select((0, 9), Is0, "  012",
                    Instr(Mnemonic.jap, InstrClass.ConditionalTransfer, Jnm),
                    invalid),
                Instr(Mnemonic.jap, InstrClass.ConditionalTransfer, Jijkm));

            var opc_013 = C90(Select((0, 9), Is0, "  013",
                    Instr(Mnemonic.jam, InstrClass.ConditionalTransfer, Jnm),
                    invalid),
                Instr(Mnemonic.jam, InstrClass.ConditionalTransfer, Jijkm));

            var opc_014 = C90(Select((0, 9), Is0, "  014",
                    Instr(Mnemonic.jsz, InstrClass.ConditionalTransfer, Jnm),
                    invalid),
                Instr(Mnemonic.jsz, InstrClass.ConditionalTransfer, Jijkm));

            var opc_015 = C90(Select((0, 9), Is0, "  015",
                    Instr(Mnemonic.jsn, InstrClass.ConditionalTransfer, Jnm),
                    invalid),
                Instr(Mnemonic.jsn, InstrClass.ConditionalTransfer, Jijkm));

            var opc_016 = C90(Select((0, 9), Is0, "  016",
                    Instr(Mnemonic.jsp, InstrClass.ConditionalTransfer, Jnm),
                    invalid),
                Instr(Mnemonic.jsp, InstrClass.ConditionalTransfer, Jijkm));

            var opc_017 = C90(Select((0, 9), Is0, "  017",
                    Instr(Mnemonic.jsm, InstrClass.ConditionalTransfer, Jnm),
                    invalid),
                Instr(Mnemonic.jsm, InstrClass.ConditionalTransfer, Jijkm));

            var opc_026 = Sparse(0, 3, "  026", Nyi("026"),
                (0, Instr(Mnemonic._popcnt, Ai, Sj)));

            var opc_027 = Sparse(0, 3, "  027", Nyi("026"),
                (0, Instr(Mnemonic._clz, Ai, Sj)));

            var opc_030 = Select((3, 3), Is0, "  030",
                Select((0, 3), Is0, "  031 x0",
                    Instr(Mnemonic._mov, Ai, Imm1),
                    Instr(Mnemonic._mov, Ai, Ak)),
                Select((0, 3), Is0, "  030 j!=0)",
                    Instr(Mnemonic._iadd, Ai, Aj, Imm1),
                    Instr(Mnemonic._iadd, Ai, Aj, Ak)));

            var opc_031 = Select((3, 3), Is0, "  031",
                Select((0, 3), Is0, "  031 x0",
                    Instr(Mnemonic._mov, Ai, Imm_1),
                    Instr(Mnemonic._neg, Ai, Ak)),
                Select((0, 3), Is0, "  031 j!=0)",
                    Instr(Mnemonic._isub, Ai, Aj, Imm1),
                    Instr(Mnemonic._isub, Ai, Aj, Ak)));

            var opc_040 = Sparse(3, 3, "  040", invalid,
                (0, Instr(Mnemonic._mov, Si, InmZext)),
                (2, Instr(Mnemonic._movlo, Si, Si, Inm)),
                (4, Instr(Mnemonic._movhi, Si, Inm, Si)));

            var opc_042 = Select((0, 6), Is0, "  042",
                Instr(Mnemonic._mov, Si, Imm_1),
                Select((0, 6), u => u == 0x3F,
                    Instr(Mnemonic._mov, Si, Imm1),
                    Instr(Mnemonic._lmask, Si, Ijk_from_40)));

            var opc_043 = Select((0, 6), Is0, "  043",
                Instr(Mnemonic._mov, Si, Imm0),
                Instr(Mnemonic._rmask, Si, Ijk));

            var opc_056 = Select((3, 3), Is0, "  056",
                Instr(Mnemonic._lsl, Si, Si, Ak),
                Select((0, 3), Is0, "  056 j!=0",
                    Instr(Mnemonic._dlsl, Si, Si, Sj, Imm1),
                    Instr(Mnemonic._dlsl, Si, Si, Sj, Ak)));

            var opc_057 = Select((3, 3), Is0, "  057",
                Instr(Mnemonic._lsr, Si, Si, Ak),
                Select((0, 3), Is0, "  057 j!=0",
                    Instr(Mnemonic._dlsr, Si, Si, Sj, Imm1),
                    Instr(Mnemonic._dlsr, Si, Si, Sj, Ak)));

            var opc_060 = Select((3, 3), Is0, "  060",
                Select((0, 3), Is0, "  060 j=0",
                    Instr(Mnemonic._mov, Si, Imm_2_63),
                    Instr(Mnemonic._mov, Si, Sk)),
                Select((0, 3), Is0, "  060 j!=0",
                    Nyi(" (Si) = (Sj) with bit 263 complemented."),
                    Instr(Mnemonic._iadd, Si, Sj, Sk)));

            var opc_061 = Select((3, 3), Is0, "  061",
                Select((0, 3), Is0, "  060 j=0",
                    Instr(Mnemonic._mov, Si, Imm_2_63),
                    Instr(Mnemonic._neg, Si, Sk)),
                Select((0, 3), Is0, "  060 j!=0",
                    Nyi(" (Sz) = (Sj) with bit 263 complemented."),
                    Instr(Mnemonic._isub, Si, Sj, Sk)));

            var opc_071 = Sparse(3, 3, " 071", Nyi("071"),
                (0, Instr(Mnemonic._movz, Si, Ak)),
                (1, Instr(Mnemonic._movs, Si, Ak)));

            var opc_072 = Sparse(0, 3, " 072", invalid,
                (0, Instr(Mnemonic._mov, Si, Reg(Registers.rt))),
                (2, Instr(Mnemonic._mov, Si, Reg(Registers.sm))),
                (3, Instr(Mnemonic._mov, Si, STj)),
                (6, Instr(Mnemonic._movst, Si, Reg(Registers.st), Aj)));

            var opc_073 = Sparse(0, 3, " 072", Nyi("073"),
                (2, Instr(Mnemonic._mov, Reg(Registers.sm), Si)));

            var opc_176 = Sparse(3, 3, "  177", invalid,
                (0, Select((0, 3), Is0, "  177 0",
                    Instr(Mnemonic._load_inc, Vi, A0, Imm32(1)),
                    Instr(Mnemonic._load_inc, Vi, A0, Ak))),
                (1, Instr(Mnemonic._load_inc, Vi, A0, Vk)));

            var opc_177 = Sparse(6, 3, "  177", invalid,
                (0, Select((0, 3), Is0, "  177 0",
                    Instr(Mnemonic._store_inc, A0, Imm32(1), Vj),
                    Instr(Mnemonic._store_inc, A0, Ak, Vj))),
                (1, Instr(Mnemonic._store_inc, A0, Vk, Vj)));

            var rootDecoder = Sparse(9, 7, "YMP",
                Nyi("YMP"),
                (0x00, Select((0, 6), Is0,
                    Instr(Mnemonic.err),
                    invalid)),
                (0x01, opc_001),
                (0x02, opc_002),
                (0x04, opc_004),
                (0x05, opc_005),
                (0x06, opc_006),
                (0x07, opc_007),

                (0x08, opc_010),
                (0x09, opc_011),
                (0x0A, opc_012),
                (0x0B, opc_013),

                (0x0C, opc_014),
                (0x0D, opc_015),
                (0x0E, opc_016),
                (0x0F, opc_017),

                (0x10, Select((0, 6), Is0, "  020",          // 0o020
                    Instr(Mnemonic._mov, Ai, Inm),
                    invalid)),
                (0x12, Instr(Mnemonic._mov, Ai, Ijk_32)),       // 0o022
                (0x13, Instr(Mnemonic._mov, Ai, Sj)),           // 0o023
                (0x14, Instr(Mnemonic._mov, Ai, Bjk)),          // 0o024
                (0x15, Instr(Mnemonic._mov, Bjk, Ai)),          // 0o025
                (0x16, opc_026),                                // 0o026
                (0x17, opc_027),                                // 0o027
                (0x18, opc_030),                                // 0o030
                (0x19, opc_031),                                // 0o031
                (0x1A, Instr(Mnemonic._imul, Ai, Aj, Ak)),      // 0o032
                (0x20, opc_040),                                // 0o040
                (0x22, opc_042),                                // 0o042
                (0x23, opc_043),                                // 0o043
                (0x24, Instr(Mnemonic._and, Si, Sj, Sk_SB)),    // 0o044
                (0x25, Instr(Mnemonic._andnot, Si, Sk_SB, Sj)),  // 0o045
                (0x26, Instr(Mnemonic._xor, Si, Sj, Sk_SB)),      // 0o046
                (0x27, Instr(Mnemonic._eqv, Si, Sk_SB, Sj)),    // 0o047
                (0x28, Instr(Mnemonic._and_or, Si, Sj, Sk_SB)), // 0o050
                (0x29, Instr(Mnemonic._vor, Si, Sj, Sk_SB)), // 0o051
                (0x2A, Instr(Mnemonic._lsl, S0, Si, Ijk)),          // 0o052
                (0x2B, Instr(Mnemonic._lsr, S0, Si, IjkFrom64)),    // 0o053
                (0x2C, Instr(Mnemonic._lsl, Si, Si, Ijk)),          // 0o054
                (0x2D, Instr(Mnemonic._lsr, Si, Si, IjkFrom64)),    // 0o055
                (0x2E, opc_056),                                    // 0o056
                (0x2F, opc_057),                                    // 0o057

                (0x30, opc_060),                                    // 0o060
                (0x31, opc_061),                                    // 0o061
                (0x34, Instr(Mnemonic._fmul, Si, Sj, Sk)),  // 0o064
                (0x39, opc_071),       // 0o071
                (0x3A, opc_072),       // 0o072
                (0x3B, opc_073),       // 0o073
                (0x3C, Instr(Mnemonic._mov, Si, Tjk)),          // 0o074
                (0x3D, Instr(Mnemonic._mov, Tjk, Si)),          // 0o075
                (0x3E, Instr(Mnemonic._mov, Si, Vj, Ak)),       // 0o076

                (0x40, Instr(Mnemonic._load, Ai, Inm)),         // 0o100
                (0x41, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o101
                (0x42, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o102
                (0x43, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o103
                (0x44, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o104
                (0x45, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o105
                (0x46, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o106
                (0x47, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o107
                (0x48, Instr(Mnemonic._store, Inm, Ai)),        // 0o100
                (0x49, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o111
                (0x4A, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o112
                (0x4B, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o113
                (0x4C, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o114
                (0x4D, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o115
                (0x4E, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o116
                (0x4F, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o117

                (0x50, Instr(Mnemonic._load, Si, Inm)),         // 0o120
                (0x51, Instr(Mnemonic._load, Si, Inm, Ah)),      // 0o121
                (0x52, Instr(Mnemonic._load, Si, Inm, Ah)),      // 0o122
                (0x53, Instr(Mnemonic._load, Si, Inm, Ah)),      // 0o123
                (0x54, Instr(Mnemonic._load, Si, Inm, Ah)),      // 0o124
                (0x55, Instr(Mnemonic._load, Si, Inm, Ah)),      // 0o125
                (0x56, Instr(Mnemonic._load, Si, Inm, Ah)),      // 0o126
                (0x57, Instr(Mnemonic._load, Si, Inm, Ah)),      // 0o127
                (0x58, Instr(Mnemonic._store, Inm, Si)),        // 0o130
                (0x59, Instr(Mnemonic._store, Inm, Ah, Si)),    // 0o131
                (0x5A, Instr(Mnemonic._store, Inm, Ah, Si)),    // 0o132
                (0x5B, Instr(Mnemonic._store, Inm, Ah, Si)),    // 0o133
                (0x5C, Instr(Mnemonic._store, Inm, Ah, Si)),    // 0o134
                (0x5D, Instr(Mnemonic._store, Inm, Ah, Si)),    // 0o135
                (0x5E, Instr(Mnemonic._store, Inm, Ah, Si)),    // 0o136
                (0x5F, Instr(Mnemonic._store, Inm, Ah, Si)),    // 0o137

                (0x60, Nyi("140")),                             // 0o140
                (0x61, Nyi("141")),                             // 0o141
                (0x62, Select((3, 3), Is0, "  142",
                    Instr(Mnemonic._vmov, Vi, Vk),              // 0o142
                    Instr(Mnemonic._vor, Vi, Sj, Vk))), 
                (0x63, Nyi("143")),                             // 0o143

                (0x6C, Select((3, 3), Is0, "  154",             // 0o154
                    Instr(Mnemonic._vmov, Vi, Vk),
                    Instr(Mnemonic._viadd, Vi, Sj, Vk))),
                (0x7E, opc_176),                                // 0o176
                (0x7F, opc_177)                                 // 0o177

                );
            return rootDecoder;
        }

    }

    public class YmpInstructionSet : InstructionSet
    {
        protected override Decoder C90(Decoder c90decoder, Decoder ympDecoder)
        {
            return ympDecoder;
        }

        protected override Decoder C90(Decoder c90decoder)
        {
            return invalid;
        }
    }

    public class C90InstructionSet : InstructionSet
    {
        protected override Decoder C90(Decoder c90decoder, Decoder ympDecoder)
        {
            return c90decoder;
        }

        protected override Decoder C90(Decoder c90decoder)
        {
            return c90decoder;
        }
    }

}
