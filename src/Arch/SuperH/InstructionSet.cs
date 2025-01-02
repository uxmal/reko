#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Text.RegularExpressions;

namespace Reko.Arch.SuperH
{
    using static System.Formats.Asn1.AsnWriter;
    using Decoder = Decoder<SuperHDisassembler, Mnemonic, SuperHInstruction>;
    using NyiDecoder = NyiDecoder<SuperHDisassembler, Mnemonic, SuperHInstruction>;

    public partial class SuperHDisassembler
    {
        public class InstructionSet
        {
            private Decoder invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

            private Func<Mnemonic, InstrClass, Mutator<SuperHDisassembler>[], Decoder> mkSh2a;
            private Func<Mnemonic, InstrClass, Mutator<SuperHDisassembler>[], Decoder> mkSh4a;
            private Func<Mnemonic, InstrClass, Mutator<SuperHDisassembler>[], Decoder> mkSh4_4a;
            private Func<Mnemonic, InstrClass, Mutator<SuperHDisassembler>[], Decoder> mkSh4_4a_2a;
            private Func<Mnemonic, InstrClass, Mutator<SuperHDisassembler>[], Decoder> mkSh3_4_2a;
            private Func<Mnemonic, InstrClass, Mutator<SuperHDisassembler>[], Decoder> mkSh2_3_4_2a;
            private Func<Mnemonic, InstrClass, Mutator<SuperHDisassembler>[], Decoder> mkSh2_3_4;
            private Func<Mnemonic, InstrClass, Mutator<SuperHDisassembler>[], Decoder> mkSh3_4;
            private Func<Mnemonic, InstrClass, Mutator<SuperHDisassembler>[], Decoder> mkSh3e_4_2a;
            private Func<Mnemonic, InstrClass, Mutator<SuperHDisassembler>[], Decoder> mkSh2e_3e_4_2a;
            private Func<Mnemonic, InstrClass, Mutator<SuperHDisassembler>[], Decoder> mkShDsp;
            private bool isDspModel;
            private bool isSh2aModel;

            public InstructionSet(Dictionary<string, object>? options)
            {
                this.mkSh2a = MakeInvalid;
                this.mkSh4a = MakeInvalid;
                this.mkSh4_4a = MakeInvalid;
                this.mkSh4_4a_2a = MakeInvalid;
                this.mkSh3_4_2a = MakeInvalid;
                this.mkSh2_3_4_2a = MakeInvalid;
                this.mkSh2_3_4 = MakeInvalid;
                this.mkSh3_4 = MakeInvalid;
                this.mkSh3e_4_2a = MakeInvalid;
                this.mkSh2e_3e_4_2a = MakeInvalid;
                this.mkShDsp = MakeInvalid;
                if (options is not null && options.TryGetValue(ProcessorOption.Model, out var oModel) &&
                    oModel is string model)
                {
                    isDspModel = false;
                    isSh2aModel = false;
                    switch (model)
                    {
                    case "sh2":
                        mkSh2_3_4_2a = Instr;
                        mkSh2_3_4 = Instr;
                        break;
                    case "sh2a":
                        mkSh2a = Instr;
                        mkSh4_4a_2a = Instr;
                        mkSh3_4_2a = Instr;
                        mkSh2_3_4_2a = Instr;
                        mkSh3e_4_2a = Instr;
                        mkSh2e_3e_4_2a = Instr;
                        isSh2aModel = true;
                        break;
                    case "sh2e":
                        mkSh2_3_4_2a = Instr;
                        mkSh2_3_4 = Instr;
                        mkSh2e_3e_4_2a = Instr;
                        break;

                    case "sh3e":
                        mkSh3e_4_2a = Instr;
                        mkSh2e_3e_4_2a = Instr;
                        goto case "sh3";
                    case "sh3":
                        mkSh3_4_2a = Instr;
                        mkSh2_3_4_2a = Instr;
                        mkSh2_3_4 = Instr;
                        mkSh3_4 = Instr;
                        break;
                    case "sh4a":
                        mkSh4a = Instr;
                        goto case "sh4";
                    case "sh4":
                        mkSh4_4a = Instr;
                        mkSh4_4a_2a = Instr;
                        mkSh3_4_2a = Instr;
                        mkSh2_3_4_2a = Instr;
                        mkSh2_3_4 = Instr;
                        mkSh3_4 = Instr;
                        mkSh3e_4_2a = Instr;
                        mkSh2e_3e_4_2a = Instr;
                        break;
                    case "dsp":
                    case "sh4_dsp":
                        mkShDsp = Instr;
                        mkSh4_4a = Instr;
                        mkSh4_4a_2a = Instr;
                        mkSh3_4_2a = Instr;
                        mkSh2_3_4_2a = Instr;
                        mkSh2_3_4 = Instr;
                        mkSh3_4 = Instr;
                        mkSh3e_4_2a = Instr;
                        mkSh2e_3e_4_2a = Instr;
                        isDspModel = true;
                        break;
                    }
                }
            }

            private Decoder MakeInvalid(Mnemonic mn, InstrClass c, params Mutator<SuperHDisassembler>[] m)
            {
                return invalid;
            }


            /*
                     SH2A
                SH4A
            SH4 SH4A
            SH4 SH4A SH2A
        SH3 SH4 SH4A
        SH3 SH4 SH4A SH2A
    SH2 SH3 SH4 SH4A SH2A
       SH3E SH4 SH4A SH2A
  SH2E SH3E SH4 SH4A SH2A
SH1 SH2 SH3 SH4 SH4A SH2A
  SH2E SH3E SH4 SH4A SH2A
    */
            public Decoder CreateDecoder()
            {
                const InstrClass LinPri = InstrClass.Linear | InstrClass.Privileged;

                var decoders_0_2 = Mask(7, 1, "  0..2",
                    Mask(4, 3, "  0.02",
                        Instr(Mnemonic.stc, LinPri, sr, r1),
                        Instr(Mnemonic.stc, gbr, r1),
                        Instr(Mnemonic.stc, LinPri, vbr, r1),
                        Instr_3_4(Mnemonic.stc, LinPri, ssr, r1),

                        Instr_3_4(Mnemonic.stc, LinPri, spc, r1),
                        Instr_DSP(Mnemonic.stc, mod, r1),
                        Instr_DSP(Mnemonic.stc, rs, r1),
                        Instr_DSP(Mnemonic.stc, re, r1)),
                    Instr_3_4(Mnemonic.stc, LinPri, RBank2_3bit, r1));

                var decoders_0_3 = Mask(4, 4, "  0..3",
                    Instr_2(Mnemonic.bsrf, InstrClass.Transfer | InstrClass.Call | InstrClass.Delay, r1),
                    invalid,
                    Instr_2(Mnemonic.braf, InstrClass.Transfer | InstrClass.Delay, r1),
                    invalid,

                    invalid,
                    invalid,
                    Instr_4a(Mnemonic.movli_l, Ind1l,R0),
                    Instr_4a(Mnemonic.movco_l, R0,Ind1l),

                    Instr_3_4_2a(Mnemonic.pref, Ind1l),
                    Instr_4(Mnemonic.ocbi, Ind1l),
                    Instr_4(Mnemonic.ocbp, Ind1l),
                    Instr_4(Mnemonic.ocbwb, Ind1l),

                    Instr_4(Mnemonic.movca_l, R0,Ind1l),
                    Instr_4a(Mnemonic.prefi, Ind1l),
                    Instr_4a(Mnemonic.icbi, Ind1l),
                    invalid);

                var decoders_0_8 = Select((7, 5), u => u == 0,
                    Mask(4, 3, "00.8",
                        Instr(Mnemonic.clrt),
                        Instr(Mnemonic.sett),
                        Instr(Mnemonic.clrmac),
                        Instr_3_4(Mnemonic.ldtlb, LinPri),

                        Instr_3_4(Mnemonic.clrs),
                        Instr_3_4(Mnemonic.sets),
                        Instr_2a(Mnemonic.nott),
                        invalid),
                    invalid);

                var decoders_0_9 = Sparse(4, 4, "  0..9", invalid,
                    (0, Select((8, 4), u => u == 0,
                        Instr(Mnemonic.nop, InstrClass.Linear | InstrClass.Padding),
                        invalid)),
                    (1, Select((8, 4), u => u == 0,
                        Instr(Mnemonic.div0u),
                        invalid)),
                    (2, Instr(Mnemonic.movt, r1)),
                    (3, Instr_2a(Mnemonic.movrt, r1)));

                var decoders_0_A = Mask(4, 4, "  0..A",
                    Instr(Mnemonic.sts, mh, r1),
                    Instr(Mnemonic.sts, ml, r1),
                    Instr(Mnemonic.sts, pr, r1),
                    Instr_4a(Mnemonic.sts, LinPri, sgr, r1),

                    Instr_2a(Mnemonic.sts, tbr, r1),
                    Instr_2E_3E_4_2A(Mnemonic.sts, fpul, r1),
                    this.DspModel(
                        Instr(Mnemonic.sts, dsr, r1),
                        Instr_2E_3E_4_2A(Mnemonic.sts, fpscr, r1)),
                    Instr_DSP(Mnemonic.sts, a0, r1),

                    Instr_DSP(Mnemonic.sts, x0, r1),
                    Instr_DSP(Mnemonic.sts, x1, r1),
                    Instr_DSP(Mnemonic.sts, y0, r1),
                    Instr_DSP(Mnemonic.sts, y1, r1),

                    invalid,
                    invalid,
                    invalid,
                    Instr_4(Mnemonic.stc, LinPri, dbr, r1));

                var decoders_0_B = Sparse(4, 4, "  0..B", invalid,
                    (0x0, Instr(Mnemonic.rts, InstrClass.Transfer | InstrClass.Return | InstrClass.Delay)),
                    (0x1, Instr(Mnemonic.sleep, InstrClass.Transfer | InstrClass.Privileged)),
                    (0x2, Instr(Mnemonic.rte, InstrClass.Transfer | InstrClass.Return | InstrClass.Delay | InstrClass.Privileged)),
                    (0x3, Instr(Mnemonic.brk)),
                    (0x5, Instr_2a(Mnemonic.resbank)),
                    (0x6, Instr_2a(Mnemonic.rts_n)),
                    (0x7, Instr_2a(Mnemonic.rtv_n, r1)),
                    (0xA, Instr_4a(Mnemonic.synco)));

                var decoders_0 = Mask(0, 4, "  0...",
                    Instr32_2a(Instr(Mnemonic.movi20, i20, r24)),
                    Instr32_2a(Instr(Mnemonic.movi20s, i20, r24)),
                    decoders_0_2,
                    decoders_0_3,

                    Instr(Mnemonic.mov_b, r2, X1b),
                    Instr(Mnemonic.mov_w, r2, X1w),
                    Instr(Mnemonic.mov_l, r2, X1l),
                    Instr(Mnemonic.mul_l, r2, r1),

                    decoders_0_8,
                    decoders_0_9,
                    decoders_0_A,
                    decoders_0_B,

                    Instr(Mnemonic.mov_b, X2b, r1),
                    Instr(Mnemonic.mov_w, X2w, r1),
                    Instr(Mnemonic.mov_l, X2l, r1),
                    Instr(Mnemonic.mac_l, Post2l, Post1l));





                var decoders_1 = Instr(Mnemonic.mov_l, r2, D1l);
                var decoders_2 = Mask(0, 4, "  2...",
                    Instr(Mnemonic.mov_b, r2, Ind1b),
                    Instr(Mnemonic.mov_w, r2, Ind1w),
                    Instr(Mnemonic.mov_l, r2, Ind1l),
                    invalid,

                    Instr(Mnemonic.mov_b, r2, Pre1b),
                    Instr(Mnemonic.mov_w, r2, Pre1w),
                    Instr(Mnemonic.mov_l, r2, Pre1l),
                    Instr(Mnemonic.div0s, r2, r1),

                    Instr(Mnemonic.tst, r2, r1),
                    Instr(Mnemonic.and, r2, r1),
                    Instr(Mnemonic.xor, r2, r1),
                    Instr(Mnemonic.or, r2, r1),

                    Instr(Mnemonic.cmp_str, r2, r1),
                    Instr(Mnemonic.xtrct, r2, r1),
                    Instr(Mnemonic.mulu_w, r2, r1),
                    Instr(Mnemonic.muls_w, r2, r1));

                var decoders_3 = Mask(0, 4, "  3...",
                    Instr(Mnemonic.cmp_eq, r2, r1),
                    Instr32_2a(Mask(12, 4, "  3..1",
                        Instr(Mnemonic.mov_b, r20, D12b_dst),
                        Instr(Mnemonic.mov_w, r20, D12w_dst),
                        Instr(Mnemonic.mov_l, r20, D12l_dst),
                        Mask(20, 1, "  3..13",
                            Instr(Mnemonic.fmov_d, d20, D12d_dst),
                            Instr(Mnemonic.fmov_s, f20, D12s_dst)),

                        Instr(Mnemonic.mov_b, D12b_src, r24),
                        Instr(Mnemonic.mov_w, D12w_src, r24),
                        Instr(Mnemonic.mov_l, D12l_src, r24),
                        Mask(20, 1, "  3..17",
                            Instr(Mnemonic.fmov_d, D12d_src, d24),
                            Instr(Mnemonic.fmov_s, D12s_src, f24)),

                        Instr(Mnemonic.movu_b, D12b_src, r24),
                        Instr(Mnemonic.movu_w, D12w_src, r24),
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid)),
                    Instr(Mnemonic.cmp_hs, r2, r1),
                    Instr(Mnemonic.cmp_ge, r2, r1),

                    Instr(Mnemonic.div1, r2, r1),
                    Instr_2(Mnemonic.dmulu_l, r2, r1),
                    Instr(Mnemonic.cmp_hi, r2, r1),
                    Instr(Mnemonic.cmp_gt, r2, r1),

                    Instr(Mnemonic.sub, r2, r1),
                    new NyiDecoder<SuperHDisassembler, Mnemonic, SuperHInstruction>("  3..9 ?"),
                    Instr(Mnemonic.subc, r2, r1),
                    Instr(Mnemonic.subv, r2, r1),

                    Instr(Mnemonic.add, r2, r1),
                    Instr_2(Mnemonic.dmuls_l, r2, r1),
                    Instr(Mnemonic.addc, r2, r1),
                    Instr(Mnemonic.addv, r2, r1));

                // 0011nnnn0iii1001 0000dddddddddddd ### SH2A          @@ bclr.b     #imm3,@(disp12,Rn) $$ 0 -> (imm of (disp+Rn)) </div>
                // 0011nnnn0iii1001 0001dddddddddddd ### SH2A          @@ bset.b     #imm3,@(disp12,Rn) $$ 1 -> (imm of (disp+Rn)) </div>
                // 0011nnnn0iii1001 0010dddddddddddd ### SH2A          @@ bst.b      #imm3,@(disp12,Rn) $$ T -> (imm of (disp+Rn)) </div>
                // 0011nnnn0iii1001 0011dddddddddddd ### SH2A          @@ bld.b      #imm3,@(disp12,Rn) $$ (imm of (disp+Rn)) -> T </div>
                // 0011nnnn0iii1001 0100dddddddddddd ### SH2A          @@ band.b     #imm3,@(disp12,Rn) $$ (imm of (disp+Rn)) & T -> T </div>
                // 0011nnnn0iii1001 0101dddddddddddd ### SH2A          @@ bor.b      #imm3,@(disp12,Rn) $$ (imm of (disp+Rn)) | T -> T </div>
                // 0011nnnn0iii1001 0110dddddddddddd ### SH2A          @@ bxor.b     #imm3,@(disp12,Rn) $$ (imm of (disp+Rn)) ^ T -> T </div>
                // 0011nnnn0iii1001 1011dddddddddddd ### SH2A          @@ bldnot.b   #imm3,@(disp12,Rn) $$ ~(imm of (disp+Rn)) -> T </div>
                // 0011nnnn0iii1001 1100dddddddddddd ### SH2A          @@ bandnot.b  #imm3,@(disp12,Rn) $$ ~(imm of (disp+Rn)) & T -> T </div>
                // 0011nnnn0iii1001 1101dddddddddddd ### SH2A          @@ bornot.b   #imm3,@(disp12,Rn) $$ ~(imm of (disp+Rn)) | T -> T </div>
                // 0011nnnnmmmm1010 ### SH1 SH2 SH3 SH4 SH4A SH2A      @@ subc	Rm,Rn                   $$ Rn - Rm - T -> Rn, borrow -> T </div>
                // 0011nnnnmmmm1011 ### SH1 SH2 SH3 SH4 SH4A SH2A      @@ subv	Rm,Rn                   $$ Rn - Rm -> Rn, underflow -> T </div>
                // 0011nnnnmmmm1100 ### SH1 SH2 SH3 SH4 SH4A SH2A      @@ add	Rm,Rn                   $$ Rn + Rm -> Rn </div>
                // 0011nnnnmmmm1101 ###     SH2 SH3 SH4 SH4A SH2A      @@ dmuls.l	Rm,Rn               $$ Signed, Rn * Rm -> MACH:MACL;; 32 * 32 -> 64 bits </div>
                // 0011nnnnmmmm1110 ### SH1 SH2 SH3 SH4 SH4A SH2A      @@ addc	Rm,Rn                   $$ Rn + Rm + T -> Rn, carry -> T </div>
                // 0011nnnnmmmm1111 ### SH1 SH2 SH3 SH4 SH4A SH2A      @@ addv	Rm,Rn                   $$ Rn + Rm -> Rn, overflow -> T </div>

                var shad = Instr_3_4_2a(Mnemonic.shad, r2, r1);
                var shld = Instr_3_4_2a(Mnemonic.shld, r2, r1);
                var mac_w = Instr(Mnemonic.mac_w, Post2w, Post1w);

                var decoders_4 = Sparse(0, 8, "  4...", invalid,
                    (0x00, Instr(Mnemonic.shll, r1)),
                    (0x01, Instr(Mnemonic.shlr, r1)),
                    (0x02, Instr(Mnemonic.sts_l, mh, Pre1l)),
                    (0x03, Instr(Mnemonic.stc_l, LinPri, sr, Pre1l)),

                    (0x04, Instr(Mnemonic.rotl, r1)),
                    (0x05, Instr(Mnemonic.rotr, r1)),
                    (0x06, Instr(Mnemonic.lds_l, Post1l, mh)),
                    (0x07, Instr(Mnemonic.ldc_l, InstrClass.Linear | InstrClass.Privileged, Post1l, sr)),

                    (0x08, Instr(Mnemonic.shll2, r1)),
                    (0x09, Instr(Mnemonic.shlr2, r1)),
                    (0x0A, Instr(Mnemonic.lds, r1, mh)),
                    (0x0B, Instr(Mnemonic.jsr, Ind1l)),

                    (0x0E, Instr(Mnemonic.ldc, InstrClass.Linear | InstrClass.Privileged, r1, sr)),

                    (0x10, Instr_2(Mnemonic.dt, r1)),
                    (0x11, Instr(Mnemonic.cmp_pz, r1)),
                    (0x12, Instr(Mnemonic.sts_l, ml, Pre1l)),
                    (0x13, Instr(Mnemonic.stc_l, gbr, Pre1l)),

                    (0x14, Instr_DSP(Mnemonic.setrc, r1)),
                    (0x15, Instr(Mnemonic.cmp_pl, r1)),
                    (0x16, Instr(Mnemonic.lds_l, Post1l, ml)),
                    (0x17, Instr(Mnemonic.ldc_l, Post1l, gbr)),

                    (0x18, Instr(Mnemonic.shll8, r1)),
                    (0x19, Instr(Mnemonic.shlr8, r1)),
                    (0x1A, Instr(Mnemonic.lds, r1, ml)),
                    (0x1B, Instr(Mnemonic.tas_b, Ind1b)),

                    (0x1E, Instr(Mnemonic.ldc, LinPri, r1, gbr)),

                    (0x20, Instr(Mnemonic.shal, r1)),
                    (0x21, Instr(Mnemonic.shar, r1)),
                    (0x22, Instr(Mnemonic.sts_l, pr, Pre1l)),
                    (0x23, Instr(Mnemonic.sts_l, LinPri, vbr, Pre1l)),

                    (0x24, Instr(Mnemonic.rotcl, r1)),
                    (0x25, Instr(Mnemonic.rotcr, r1)),
                    (0x26, Instr(Mnemonic.lds_l, Post1l, pr)),
                    (0x27, Instr(Mnemonic.ldc_l, LinPri, Post1l, vbr)),

                    (0x28, Instr(Mnemonic.shll16, r1)),
                    (0x29, Instr(Mnemonic.shlr16, r1)),
                    (0x2A, Instr(Mnemonic.lds, LinPri, r1, pr)),
                    (0x2B, Instr(Mnemonic.jmp, InstrClass.Transfer, Ind1l)),

                    (0x2E, Instr(Mnemonic.ldc, LinPri, r1, vbr)),

                    (0x32, Instr_4(Mnemonic.stc_l, LinPri, sgr, Pre1l)),
                    (0x33, Instr_3_4(Mnemonic.stc_l, LinPri, ssr, Pre1l)),
                    (0x36, Instr_4(Mnemonic.ldc_l, LinPri, Post1l, sgr)),
                    (0x37, Instr_3_4(Mnemonic.ldc_l, LinPri, Post1l, ssr)),

                    (0x3A, Instr_4(Mnemonic.ldc_l, LinPri, r1, sgr)),
                    (0x3E, Instr_3_4(Mnemonic.ldc_l, LinPri, r1, ssr)),

                    (0x43, Instr_3_4(Mnemonic.stc_l, LinPri, spc, Pre1l)),
                    (0x47, Instr_3_4(Mnemonic.ldc_l, LinPri, Post1l, spc)),

                    (0x4A, Instr_2a(Mnemonic.ldc, r1, tbr)),
                    (0x4B, Instr_2a(Mnemonic.jsr_n, InstrClass.Transfer | InstrClass.Call, Ind1l)),

                    (0x4E, Instr_3_4(Mnemonic.ldc, LinPri, r1, spc)),

                    (0x52, Instr_2E_3E_4_2A(Mnemonic.sts_l, fpul, Pre1l)),
                    (0x53, Instr_DSP(Mnemonic.stc_l, mod, Pre1l)),

                    (0x56, Instr_2E_3E_4_2A(Mnemonic.lds_l, Post1l, fpul)),
                    (0x57, Instr_DSP(Mnemonic.ldc_l, Post1l, mod)),

                    (0x5A, Instr_2E_3E_4_2A(Mnemonic.lds, r1, fpul)),
                    (0x5E, Instr_DSP(Mnemonic.ldc, r1, mod)),

                    (0x62, Instr_2E_3E_4_2A(Mnemonic.sts_l, fpscr, Pre1l)),
                // 0100nnnn01100010 ### SH2E SH3E   SH4 SH4A SH2A      @@ sts.l	FPSCR,@-Rn      $$ Rn-4 -> Rn, FPSCR -> (Rn) </div>
                // 0100nnnn01100010 ### DSP                            @@ sts.l	A0,@-Rn         $$ Rn-4 -> Rn, A0 -> (Rn) </div>
                // 0100nnnn01100010 ### DSP                            @@ sts.l	DSR,@-Rn        $$ Rn-4 -> Rn, DSR -> (Rn) </div>
                    (0x63, Instr_DSP(Mnemonic.stc_l, rs, Pre1l)),
                    (0x66, Instr_2E_3E_4_2A(Mnemonic.lds_l, Post1l, fpscr)),
                // 0100mmmm01100110 ### DSP                            @@ lds.l	@Rm+,DSR        $$ (Rm) -> DSR, Rm+4 -> Rm </div>
                    (0x67, Instr_DSP(Mnemonic.ldc_l, Post1l, rs)),

                    (0x6A, Instr_2E_3E_4_2A(Mnemonic.lds, r1, fpscr)),
                // 0100mmmm01101010 ### DSP                            @@ lds	Rm,DSR          $$ Rm -> DSR </div>
                    (0x6E, Instr_DSP(Mnemonic.ldc, r1, rs)),

                    (0x73, Instr_DSP(Mnemonic.stc_l, re, Pre1l)),
                    (0x76, Instr_DSP(Mnemonic.lds, r1, a0)),
                // 0100mmmm01110110 ### DSP                            @@ lds	Rm,A0           $$ Rm -> A0 </div>
                // 0100mmmm01110110 ### DSP                            @@ lds.l	@Rm+,A0         $$ (Rm) -> A0, Rm+4 -> Rm </div>
                // 0100mmmm01110111 ### DSP                            @@ ldc.l	@Rm+,RE         $$ (Rm) -> RE, Rm+4 -> Rm </div>
                // 0100mmmm01111110 ### DSP                            @@ ldc	Rm,RE           $$ Rm -> RE </div>

                    (0x80, Instr_2a(Mnemonic.mulr, R0, r1)),
                    (0x81, Instr_2a(Mnemonic.clipu_b, r1)),
                    (0x82, Instr_DSP(Mnemonic.sts_l, x0, Pre1l)),

                    (0x84, Instr_2a(Mnemonic.divu, R0, r1)),
                    (0x85, Instr_2a(Mnemonic.clipu_w, r1)),
                    (0x86, Instr_DSP(Mnemonic.lds_l, Post1l, x0)),

                    (0x8A, Instr_DSP(Mnemonic.lds, r1, x0)),
                    (0x8B, Instr_2a(Mnemonic.mov_b, R0, Post1b)),

                    (0x91, Instr_2a(Mnemonic.clips_b, r1)),
                    (0x92, Instr_DSP(Mnemonic.sts_l, x1, Pre1l)),

                    (0x94, Instr_2a(Mnemonic.divs, R0, r1)),
                    (0x95, Instr_2a(Mnemonic.clips_w, r1)),
                    (0x96, Instr_DSP(Mnemonic.lds_l, Post1l, x1)),

                    (0x9A, Instr_DSP(Mnemonic.lds, r1, x1)),
                    (0x9B, Instr_2a(Mnemonic.mov_w, r1, Post1w)),

                    (0xA2, Instr_DSP(Mnemonic.sts_l, y0, Pre1l)),
                    (0xA6, Instr_DSP(Mnemonic.lds_l, Post1l, y0)),

                    (0xA9, Instr_4a(Mnemonic.movua_l, Ind1l, R0)),
                    (0xAA, Instr_DSP(Mnemonic.lds, r1, y0)),
                    (0xAB, Instr_DSP(Mnemonic.mov_l, R0, Post1l)),

                    (0xB2, Instr_DSP(Mnemonic.sts_l, y1, Pre1l)),

                    (0xB6, Instr_DSP(Mnemonic.lds_l, Post1l, y1)),
                    (0xBA, Instr_DSP(Mnemonic.lds, r1, y1)),

                    (0xCB, Instr_2a(Mnemonic.mov_b, Pre1b, R0)),

                    (0xDB, Instr_2a(Mnemonic.mov_w, Pre1w, R0)),

                    (0xE1, Instr_2a(Mnemonic.stbank, R0, Ind1l)),
                    (0xE5, Instr_2a(Mnemonic.ldbank, Ind1l, R0)),

                    (0xE9, Instr_4a(Mnemonic.movua_l, Post1l, R0)),
                    (0xEB, Instr_2a(Mnemonic.mov_l, Pre1l, R0)),

                    (0xF0, Instr_2a(Mnemonic.movmu_l, r1, Pre15l)),
                    (0xF1, Instr_2a(Mnemonic.movml_l, r1, Pre15l)),
                    (0xF2, Instr_4(Mnemonic.stc_l, LinPri, dbr, Pre1l)),

                    (0xF4, Instr_2a(Mnemonic.movmu_l, Post15l, r1)),
                    (0xF5, Instr_2a(Mnemonic.movml_l, Post15l, r1)),
                    (0xF6, Instr_4(Mnemonic.ldc_l, Post1l, dbr)),

                    (0xFA, Instr_4(Mnemonic.ldc, r1, dbr)),

                    (0x83, Instr_3_4(Mnemonic.stc_l, RBank2_3bit, Pre1l)),
                    (0x93, Instr_3_4(Mnemonic.stc_l, RBank2_3bit, Pre1l)),
                    (0xA3, Instr_3_4(Mnemonic.stc_l, RBank2_3bit, Pre1l)),
                    (0xB3, Instr_3_4(Mnemonic.stc_l, RBank2_3bit, Pre1l)),
                    (0xC3, Instr_3_4(Mnemonic.stc_l, RBank2_3bit, Pre1l)),
                    (0xD3, Instr_3_4(Mnemonic.stc_l, RBank2_3bit, Pre1l)),
                    (0xE3, Instr_3_4(Mnemonic.stc_l, RBank2_3bit, Pre1l)),
                    (0xF3, Instr_3_4(Mnemonic.stc_l, RBank2_3bit, Pre1l)),

                    (0x87, Instr_3_4(Mnemonic.ldc_l, Post1l, RBank2_3bit)),
                    (0x97, Instr_3_4(Mnemonic.ldc_l, Post1l, RBank2_3bit)),
                    (0xA7, Instr_3_4(Mnemonic.ldc_l, Post1l, RBank2_3bit)),
                    (0xB7, Instr_3_4(Mnemonic.ldc_l, Post1l, RBank2_3bit)),
                    (0xC7, Instr_3_4(Mnemonic.ldc_l, Post1l, RBank2_3bit)),
                    (0xD7, Instr_3_4(Mnemonic.ldc_l, Post1l, RBank2_3bit)),
                    (0xE7, Instr_3_4(Mnemonic.ldc_l, Post1l, RBank2_3bit)),
                    (0xF7, Instr_3_4(Mnemonic.ldc_l, Post1l, RBank2_3bit)),

                    (0x0C, shad),
                    (0x1C, shad),
                    (0x2C, shad),
                    (0x3C, shad),
                    (0x4C, shad),
                    (0x5C, shad),
                    (0x6C, shad),
                    (0x7C, shad),
                    (0x8C, shad),
                    (0x9C, shad),
                    (0xAC, shad),
                    (0xBC, shad),
                    (0xCC, shad),
                    (0xDC, shad),
                    (0xEC, shad),
                    (0xFC, shad),

                    (0x0D, shld),
                    (0x1D, shld),
                    (0x2D, shld),
                    (0x3D, shld),
                    (0x4D, shld),
                    (0x5D, shld),
                    (0x6D, shld),
                    (0x7D, shld),
                    (0x8D, shld),
                    (0x9D, shld),
                    (0xAD, shld),
                    (0xBD, shld),
                    (0xCD, shld),
                    (0xDD, shld),
                    (0xED, shld),
                    (0xFD, shld),

                    (0x8E, Instr_3_4(Mnemonic.ldc, r1, RBank2_3bit)),
                    (0x9E, Instr_3_4(Mnemonic.ldc, r1, RBank2_3bit)),
                    (0xAE, Instr_3_4(Mnemonic.ldc, r1, RBank2_3bit)),
                    (0xBE, Instr_3_4(Mnemonic.ldc, r1, RBank2_3bit)),
                    (0xCE, Instr_3_4(Mnemonic.ldc, r1, RBank2_3bit)),
                    (0xDE, Instr_3_4(Mnemonic.ldc, r1, RBank2_3bit)),
                    (0xEE, Instr_3_4(Mnemonic.ldc, r1, RBank2_3bit)),
                    (0xFE, Instr_3_4(Mnemonic.ldc, r1, RBank2_3bit)),

                    (0x0F, mac_w),
                    (0x1F, mac_w),
                    (0x2F, mac_w),
                    (0x3F, mac_w),
                    (0x4F, mac_w),
                    (0x5F, mac_w),
                    (0x6F, mac_w),
                    (0x7F, mac_w),
                    (0x8F, mac_w),
                    (0x9F, mac_w),
                    (0xAF, mac_w),
                    (0xBF, mac_w),
                    (0xCF, mac_w),
                    (0xDF, mac_w),
                    (0xEF, mac_w),
                    (0xFF, mac_w));

                // 0100mmmm1nnn0111 ###         SH3 SH4 SH4A      Priv @@ ldc.l	@Rm+,Rn_BANK    $$ (Rm) -> Rn_BANK, Rm+4 -> Rm </div>
                // 0100mmmm1nnn1110 ###         SH3 SH4 SH4A      Priv @@ ldc	Rm,Rn_BANK      $$ Rm -> Rn_BANK (n = 0-7) </div>
                // 0100nnnn1mmm0011 ###         SH3 SH4 SH4A      Priv @@ stc.l	Rm_BANK,@-Rn    $$ Rn-4 -> Rn, Rm_BANK -> (Rn) (m = 0-7) </div>

                var decoders_5 = Instr(Mnemonic.mov_l, D2l, r1);

                var decoders_6 = Mask(0, 4, "  6...",
                    Instr(Mnemonic.mov_b, Ind2b, r1),
                    Instr(Mnemonic.mov_w, Ind2w, r1),
                    Instr(Mnemonic.mov_l, Ind2l, r1),
                    Instr(Mnemonic.mov, r2, r1),

                    Instr(Mnemonic.mov_b, Post2b, r1),
                    Instr(Mnemonic.mov_w, Post2w, r1),
                    Instr(Mnemonic.mov_l, Post2l, r1),
                    Instr(Mnemonic.not, r2, r1),

                    Instr(Mnemonic.swap_b, r2, r1),
                    Instr(Mnemonic.swap_w, r2, r1),
                    Instr(Mnemonic.negc, r2, r1),
                    Instr(Mnemonic.neg, r2, r1),

                    Instr(Mnemonic.extu_b, r2, r1),
                    Instr(Mnemonic.extu_w, r2, r1),
                    Instr(Mnemonic.exts_b, r2, r1),
                    Instr(Mnemonic.exts_w, r2, r1));

                var decoders_7 = Instr(Mnemonic.add, I, r1);

                var decoders_8 = Mask(8, 4, "  8...",
                    Instr(Mnemonic.mov_b, R0, D2b),
                    Instr(Mnemonic.mov_w, R0, D2w),
                    Instr_DSP(Mnemonic.setrc, I),
                    Instr_2a(Mnemonic.jsr_n, InstrClass.Transfer | InstrClass.Call, DindTbr_l),

                    Instr(Mnemonic.mov_b, D2b, R0),
                    Instr(Mnemonic.mov_w, D2w, R0),
                    Mask(3, 1, "  86..",
                        Instr_2a(Mnemonic.bclr, i3, r2),
                        Instr_2a(Mnemonic.bset, i3, r2)),
                    Mask(3, 1, "  87..",
                        Instr_2a(Mnemonic.bst, i3, r2),
                        Instr_2a(Mnemonic.bld, i3, r2)),

                    Instr(Mnemonic.cmp_eq, I, R0),
                    Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, j),
                    invalid,
                    Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, j),

                    Instr_DSP(Mnemonic.ldrs, Pl),
                    Instr(Mnemonic.bt_s, j),
                    Instr_DSP(Mnemonic.ldre, Pl),
                    Instr(Mnemonic.bf_s, j));

                var decoders_9 = Instr(Mnemonic.mov_w, Pw, r1);
                var decoders_A = Instr(Mnemonic.bra, J);
                var decoders_B = Instr(Mnemonic.bsr, J);
                var decoders_C = Mask(8, 4, "  C...",
                    Instr(Mnemonic.mov_b, R0, Gin_b),
                    Instr(Mnemonic.mov_w, R0, Gin_w),
                    Instr(Mnemonic.mov_l, R0, Gin_l),
                    Instr(Mnemonic.trapa, InstrClass.Transfer | InstrClass.Call, I),

                    Instr(Mnemonic.mov_b, Gin_b, R0),
                    Instr(Mnemonic.mov_w, Gin_w, R0),
                    Instr(Mnemonic.mov_l, Gin_l, R0),
                    Instr(Mnemonic.mova, Pl, R0),

                    Instr(Mnemonic.tst, I, R0),
                    Instr(Mnemonic.and, I, R0),
                    Instr(Mnemonic.xor, I, R0),
                    Instr(Mnemonic.or, I, R0),

                    Instr(Mnemonic.tst_b, I, Gix_b),
                    Instr(Mnemonic.and_b, I, Gix_b),
                    Instr(Mnemonic.xor_b, I, Gix_b),
                    Instr(Mnemonic.or_b, I, Gix_b));

                var decoders_D = Instr(Mnemonic.mov_l, Pl, r1);
                var decoders_E = Instr(Mnemonic.mov, I, r1);

                var decoder_F10_8_dsp32 = Mask(12, 4, "   F10..8 dsp",
                    invalid,
                    Instr(Mnemonic.pshl, Sx_xa,Sy_ym,Dz),     // 10000001xxyyzzzz ### DSP           @@ pshl		Sx,Sy,Dz    $$ If Sy >= 0: Sx << Sy -> Dz, clear LSW of Dz;;If Sy < 0: Sx >> Sy -> Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.dct_pshl, Sx_xa,Sy_ym,Dz), // 10000010xxyyzzzz ### DSP           @@ dct pshl	Sx,Sy,Dz    $$ If DC = 1 & Sy >= 0: Sx << Sy -> Dz, clear LSW of Dz;;If DC = 1 & Sy < 0: Sx >> Sy -> Dz, clear LSW of Dz;;If DC = 0: nop </div>
                    Instr(Mnemonic.dcf_pshl, Sx_xa,Sy_ym,Dz), // 10000011xxyyzzzz ### DSP           @@ dcf pshl	Sx,Sy,Dz    $$ If DC = 0 & Sy >= 0: Sx << Sy -> Dz, clear LSW of Dz;;If DC = 0 & Sy < 0: Sx >> Sy -> Dz, clear LSW of Dz;;If DC = 1: nop </div>

                    Instr(Mnemonic.pcmp, Sx_xa,Sy_ym),        // 10000100xxyy0000 ### DSP           @@ pcmp		Sx,Sy       $$ Sx - Sy </div>
                    invalid,
                    invalid,
                    invalid,

                    Instr(Mnemonic.pabs, Sx_xa,Dz),        // 10001000xx00zzzz ### DSP           @@ pabs		Sx,Dz       $$ If Sx >= 0: Sx -> Dz;;If Sx < 0: 0 - Sx -> Dz </div>
                    Instr(Mnemonic.pdec, Sx_xa,Dz),        // 10001001xx00zzzz ### DSP           @@ pdec		Sx,Dz       $$ MSW of Sx - 1 -> MSW of Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.dct_pdec, Sx_xa,Dz),    // 10001010xx00zzzz ### DSP           @@ dct pdec	Sx,Dz       $$ If DC = 1: MSW of Sx - 1 -> MSW of DZ, clear LSW of Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pdec, Sx_xa,Dz),    // 10001011xx00zzzz ### DSP           @@ dcf pdec	Sx,Dz       $$ If DC = 0: MSW of Sx - 1 -> MSW of DZ, clear LSW of Dz;;Else: nop </div>

                    invalid,
                    Instr(Mnemonic.pclr, Dz),           // 100011010000zzzz ### DSP           @@ pclr		Dz          $$ 0x00000000 -> Dz </div>
                    Instr(Mnemonic.dct_pclr, Dz),       // 100011100000zzzz ### DSP           @@ dct pclr	Dz          $$ If DC = 1: 0x00000000 -> Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pclr, Dz));      // 100011110000zzzz ### DSP           @@ dcf pclr	Dz          $$ If DC = 0: 0x00000000 -> Dz;;Else: nop </div>

                var decoder_F10_9_dsp32 = Mask(12, 4, "   F10..9 dsp",
                    invalid,
                    Instr(Mnemonic.psha,Sx_xy, Sy_ym, Dz),    // 10010001xxyyzzzz ### DSP           @@ psha		Sx,Sy,Dz    $$ If Sy >= 0: Sx << Sy -> Dz;;If Sy < 0: Sx >> Sy -> Dz </div>
                    Instr(Mnemonic.dct_psha,Sx_xy,Sy_ym,Dz),  // 10010010xxyyzzzz ### DSP           @@ dct psha	Sx,Sy,Dz    $$ If DC = 1 & Sy >= 0: Sx << Sy -> Dz;;If DC = 1 & Sy < 0: Sx >> Sy -> Dz;;If DC = 0: nop </div>
                    Instr(Mnemonic.dcf_psha,Sx_xy,Sy_ym,Dz),  // 10010011xxyyzzzz ### DSP           @@ dcf psha	Sx,Sy,Dz    $$ If DC = 0 & Sy >= 0: Sx << Sy -> Dz;;If DC = 0 & Sy < 0: Sx >> Sy -> Dz;;If DC = 1: nop </div>

                    invalid,
                    Instr(Mnemonic.pand, Sx_xy,Sy_ym,Dz),     // 10010101xxyyzzzz ### DSP           @@ pand		Sx,Sy,Dz    $$ Sx & Sy -> Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.dct_pand, Sx_xy,Sy_ym,Dz),  // 10010110xxyyzzzz ### DSP           @@ dct pand	Sx,Sy,Dz    $$ If DC = 1: Sx & Sy -> Dz, clear LSW of Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pand, Sx_xy,Sy_ym,Dz),  // 10010111xxyyzzzz ### DSP           @@ dcf pand	Sx,Sy,Dz    $$ If DC = 0: Sx & Sy -> Dz, clear LSW of Dz;;Else: nop </div>

                    Instr(Mnemonic.prnd, Sx_xa,Dz),        // 10011000xx00zzzz ### DSP           @@ prnd		Sx,Dz       $$ Sx + 0x00008000 -> Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.pinc, Sx_xy,Dz),        // 10011001xx00zzzz ### DSP           @@ pinc		Sx,Dz       $$ MSW of Sy + 1 -> MSW of Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.dct_pinc, Sx_xy,Dz),     // 10011010xx00zzzz ### DSP           @@ dct pinc	Sx,Dz       $$ If DC = 1: MSW of Sx + 1 -> MSW of Dz, clear LSW of Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pinc, Sx_xy,Dz),     // 10011011xx00zzzz ### DSP           @@ dcf pinc	Sx,Dz       $$ If DC = 0: MSW of Sx + 1 -> MSW of Dz, clear LSW of Dz;;Else: nop </div>

                    invalid,
                    Instr(Mnemonic.pdmsb,Sx_xy,Dz),        // 10011101xx00zzzz ### DSP           @@ pdmsb		Sx,Dz       $$ Sx data MSB position -> MSW of Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.dct_pdmsb,Sx_xy,Dz),    // 10011110xx00zzzz ### DSP           @@ dct pdmsb	Sx,Dz       $$ If DC = 1: Sx data MSB position -> MSW of Dz, clear LSW of Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pdmsb,Sx_xy, Dz));  // 10011111xx00zzzz ### DSP           @@ dcf pdmsb	Sx,Dz       $$ If DC = 0: Sx data MSB position -> MSW of Dz, clear LSW of Dz;;Else: nop </div>


                var decoder_F10_A_dsp32 = Mask(12, 4, "   F10..A dsp",
                    Instr(Mnemonic.psubc, Sx_xa, Sy_ym, Dz),    // 10100000xxyyzzzz ### DSP           @@ psubc		Sx,Sy,Dz    $$ Sx - Sy - DC -> Dz </div>
                    Instr(Mnemonic.psub, Sx_xa, Sy_ym, Dz),     // 10100001xxyyzzzz ### DSP           @@ psub		Sx,Sy_ym,Dz    $$ Sx - Sy -> Dz </div>
                    Instr(Mnemonic.dct_psub, Sx_xa, Sy_ym, Dz), // 10100010xxyyzzzz ### DSP           @@ dct psub	Sx,Sy,Dz    $$ If DC = 1: Sx - Sy -> Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_psub, Sx_xa, Sy_ym, Dz), // 10100011xxyyzzzz ### DSP           @@ dcf psub 	Sx,Sy,Dz    $$ If DC = 0: Sx - Sy -> Dz;;Else: nop </div>

                    invalid,
                    Instr(Mnemonic.pxor, Sx_xy, Sy_ym, Dz),     // 10100101xxyyzzzz ### DSP           @@ pxor		Sx,Sy,Dz    $$ Sx ^ Sy -> Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.dct_pxor, Sx_xy, Sy_ym, Dz), // 10100110xxyyzzzz ### DSP           @@ dct pxor	Sx,Sy,Dz    $$ If DC = 1: Sx ^ Sy -> Dz, clear LSW of Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pxor, Sx_xy, Sy_ym, Dz), // 10100111xxyyzzzz ### DSP           @@ dcf pxor	Sx,Sy,Dz    $$ If DC = 0: Sx ^ Sy -> Dz, clear LSW of Dz;;Else: nop </div>

                    Instr(Mnemonic.pabs, Sy_ym, Dz),            // 1010100000yyzzzz ### DSP           @@ pabs		Sy,Dz       $$ If Sy >= 0: Sy -> Dz;;If Sy < 0: 0 - Sy -> Dz </div>
                    Instr(Mnemonic.pdec, Sy_ym, Dz),            // 1010100100yyzzzz ### DSP           @@ pdec		Sy,Dz       $$ MSW of Sy - 1 -> MSW of Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.dct_pdec, Sy_ym, Dz),        // 1010101000yyzzzz ### DSP           @@ dct pdec	Sy,Dz       $$ If DC = 1: MSW of Sy - 1 -> MSW of DZ, clear LSW of Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pdec, Sy_ym, Dz),       // 1010101100yyzzzz ### DSP           @@ dcf pdec	Sy,Dz       $$ If DC = 0: MSW of Sy - 1 -> MSW of DZ, clear LSW of Dz;;Else: nop </div>

                    invalid,
                    invalid,
                    invalid,
                    invalid);

                var decoder_F10_B_dsp32 = Mask(12, 4, "   F10..B dsp",
                    Instr(Mnemonic.paddc, Sx_xa, Sy_ym, Dz),    // 10110000xxyyzzzz ### DSP           @@ paddc		Sx,Sy,Dz    $$ Sx + Sy + DC -> Dz </div>
                    Instr(Mnemonic.padd, Sx_xa, Sy_ym, Dz),     // 10110001xxyyzzzz ### DSP           @@ padd		Sx,Sy,Dz    $$ Sx + Sy -> Dz </div>
                    Instr(Mnemonic.dct_padd, Sx_xa, Sy_ym, Dz), // 10110010xxyyzzzz ### DSP           @@ dct padd	Sx,Sy,Dz    $$ If DC = 1: Sx + Sy -> Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_padd, Sx_xa, Sy_ym, Dz), // 10110011xxyyzzzz ### DSP           @@ dcf padd	Sx,Sy,Dz    $$ If DC = 0: Sx + Sy -> Dz;;Else: nop </div>

                    invalid,
                    Instr(Mnemonic.por, Sx_xy, Sy_ym, Dz),      // 10110101xxyyzzzz ### DSP           @@ por		Sx,Sy,Dz    $$ Sx | Sy -> Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.dct_por, Sx_xy, Sy_ym, Dz),  // 10110110xxyyzzzz ### DSP           @@ dct por	Sx,Sy,Dz    $$ If DC = 1: Sx | Sy -> Dz, clear LSW of Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_por, Sx_xy, Sy_ym, Dz),  // 10110111xxyyzzzz ### DSP           @@ dcf por	Sx,Sy,Dz    $$ If DC = 0: Sx | Sy -> Dz, clear LSW of Dz;;Else: nop </div>

                    Instr(Mnemonic.prnd, Sy_ym, Dz),        // 1011100000yyzzzz ### DSP           @@ prnd		Sy,Dz       $$ Sy + 0x00008000 -> Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.pinc, Sy_ym, Dz),        // 1011100100yyzzzz ### DSP           @@ pinc		Sy,Dz       $$ MSW of Sy + 1 -> MSW of Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.dct_pinc, Sy_ym, Dz),    // 1011101000yyzzzz ### DSP           @@ dct pinc	Sy,Dz       $$ If DC = 1: MSW of Sy + 1 -> MSW of Dz, clear LSW of Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pinc, Sy_ym, Dz),    // 1011101100yyzzzz ### DSP           @@ dcf pinc	Sy,Dz       $$ If DC = 0: MSW of Sy + 1 -> MSW of Dz, clear LSW of Dz;;Else: nop </div>

                    invalid,
                    Instr(Mnemonic.pdmsb, Sy_ym, Dz),       // 1011110100yyzzzz ### DSP           @@ pdmsb		Sy,Dz       $$ Sy data MSB position -> MSW of Dz, clear LSW of Dz </div>
                    Instr(Mnemonic.dct_pdmsb, Sy_ym, Dz),   // 1011111000yyzzzz ### DSP           @@ dct pdmsb	Sy,Dz       $$ If DC = 1: Sy data MSB position -> MSW of Dz, clear LSW of Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pdmsb, Sy_ym, Dz));  // 1011111100yyzzzz ### DSP           @@ dcf pdmsb	Sy,Dz       $$ If DC = 0: Sy data MSB position -> MSW of Dz, clear LSW of Dz;;Else: nop </div>


                var decoder_F10_C_dsp32 = Mask(12, 4, "   F10..C dsp",
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    Instr(Mnemonic.pneg, Sx_xy,Dz),        // 11001001xx00zzzz ### DSP           @@ pneg		Sx,Dz       $$ 0 - Sx -> Dz </div>
                    Instr(Mnemonic.dct_pneg, Sx_xy,Dz),    // 11001010xx00zzzz ### DSP           @@ dct pneg	Sx,Dz       $$ If DC = 1: 0 - Sx -> Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pneg, Sx_xy,Dz),    // 11001011xx00zzzz ### DSP           @@ dcf pneg	Sx,Dz       $$ If DC = 0: 0 - Sx -> Dz;;Else: nop </div>

                    invalid,
                    Instr(Mnemonic.psts, mh,Dz),        // 110011010000zzzz ### DSP           @@ psts		MACH,Dz     $$ MACH -> Dz </div>
                    Instr(Mnemonic.dct_psts, mh,Dz),    // 110011100000zzzz ### DSP           @@ dct psts	MACH,Dz     $$ If DC = 1: MACH -> Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_psts, mh, Dz));  // 110011110000zzzz ### DSP           @@ dcf psts	MACH,Dz     $$ If DC = 0: MACH -> Dz;;Else: nop </div>

                var decoder_F10_D_dsp32 = Nyi("   F10..D dsp");
                 //  Instr(Mnemonic.psubc,Sx_xa,Sy_ym,Dz),       // 1101 0000 0xxyyzzzz ### DSP           @@ psubc		Sx,Sy,Dz    $$ Sx - Sy - DC -> Dz </div>
                 //  Instr(Mnemonic.psub, Sx_xa,Sy_ym,Dz),       // 1101 0000 1xxyyzzzz ### DSP           @@ psub		Sx,Sy,Dz    $$ Sx - Sy -> Dz </div>
                 //  Instr(Mnemonic.dct_psub, Sx_xa,Sy_ym,Dz),   // 1101 0001 0xxyyzzzz ### DSP           @@ dct psub	Sx,Sy,Dz    $$ If DC = 1: Sx - Sy -> Dz;;Else: nop </div>
                 //  Instr(Mnemonic.dcf_psub, Sx_xa,Sy_ym,Dz),   // 1101 0001 1xxyyzzzz ### DSP           @@ dcf psub 	Sx,Sy,Dz    $$ If DC = 0: Sx - Sy -> Dz;;Else: nop </div>
                 //
                 //  invalid,
                 //  Instr(Mnemonic.pxor,Sx_xy,Sy_ym,Dz),        // 1101 0010 1xxyyzzzz ### DSP           @@ pxor		Sx,Sy,Dz    $$ Sx ^ Sy -> Dz, clear LSW of Dz </div>
                 //  Instr(Mnemonic.dct_pxor, Sx_xy,Sy_ym,Dz),   // 1101 0011 0xxyyzzzz ### DSP           @@ dct pxor	Sx,Sy,Dz    $$ If DC = 1: Sx ^ Sy -> Dz, clear LSW of Dz;;Else: nop </div>
                 //  Instr(Mnemonic.dcf_pxor, Sx_xy,Sy_ym,Dz),   // 1101 0011 1xxyyzzzz ### DSP           @@ dcf pxor	Sx,Sy,Dz    $$ If DC = 0: Sx ^ Sy -> Dz, clear LSW of Dz;;Else: nop </div>
                 //                                                           
                 //  Instr(Mnemonic.pabs,  Sy_ym,Dz   ),         // 1101 0100 000yyzzzz ### DSP           @@ pabs		Sy,Dz       $$ If Sy >= 0: Sy -> Dz;;If Sy < 0: 0 - Sy -> Dz </div>
                 //  Instr(Mnemonic.pdec,  Sy_ym,Dz   ),         // 1101 0100 100yyzzzz ### DSP           @@ pdec		Sy,Dz       $$ MSW of Sy - 1 -> MSW of Dz, clear LSW of Dz </div>
                 //  Instr(Mnemonic.dct_pdec, Sy_ym,Dz   ),      // 1101 0101 000yyzzzz ### DSP           @@ dct pdec	Sy,Dz       $$ If DC = 1: MSW of Sy - 1 -> MSW of DZ, clear LSW of Dz;;Else: nop </div>
                 //  Instr(Mnemonic.dcf_pdec, Sy_ym, Dz  ),      // 1101 0101 100yyzzzz ### DSP           @@ dcf pdec	Sy,Dz       $$ If DC = 0: MSW of Sy - 1 -> MSW of DZ, clear LSW of Dz;;Else: nop </div>
                 //                                                           
                 //  Instr(Mnemonic.pcopy,		Sx_xy,Dz),      // 1101 1001 xx00zzzz ### DSP           @@ pcopy		Sx,Dz       $$ Sx -> Dz </div>
                 //  Instr(Mnemonic.dct_pcopy,	Sx_xy,Dz),      // 1101 1010 xx00zzzz ### DSP           @@ dct pcopy	Sx,Dz       $$ If DC = 1: Sx -> Dz;;Else: nop </div>
                 //  Instr(Mnemonic.dcf_pcopy,	Sx_xy,Dz),      // 1101 1011 xx00zzzz ### DSP           @@ dcf pcopy	Sx,Dz       $$ If DC = 0: Sx -> Dz;;Else: nop </div>
                 //  Instr(Mnemonic.psts, ml,Dz),                // 1101 1101 0000zzzz ### DSP           @@ psts		MACL,Dz     $$ MACL -> Dz </div>
                 //  Instr(Mnemonic.dct_psts, ml, Dz  ),         // 1101 1110 0000zzzz ### DSP           @@ dct psts	MACL,Dz     $$ If DC = 1: MACL -> Dz;;Else: nop </div>
                 //  Instr(Mnemonic.dcf_psts, ml, Dz ));         // 1101 1111 0000zzzz ### DSP           @@ dcf psts	MACL,Dz     $$ If DC = 0: MACL -> Dz;;Else: nop </div>


                var decoder_F10_E_dsp32 = Mask(12, 4, "   F10..E dsp",
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    Instr(Mnemonic.pneg, Sy_ym, Dz), // 1110100100yyzzzz ### DSP           @@ pneg		Sy,Dz       $$ 0 - Sy -> Dz </div>
                    Instr(Mnemonic.dct_pneg, Sy_ym, Dz), // 1110101000yyzzzz ### DSP           @@ dct pneg	Sy,Dz       $$ If DC = 1: 0 - Sy -> Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pneg, Sy_ym, Dz),  // 1110101100yyzzzz ### DSP           @@ dcf pneg	Sy,Dz       $$ If DC = 0: 0 - Sy -> Dz;;Else: nop </div>
                
                    invalid,
                    Instr(Mnemonic.plds, Dz, mh),   // 111011010000zzzz ### DSP           @@ plds		Dz,MACH     $$ Dz -> MACH </div>
                    Instr(Mnemonic.dct_plds, Dz, mh),   // 111011100000zzzz ### DSP           @@ dct plds	Dz,MACH     $$ If DC = 1: Dz -> MACH;;Else: nop </div>
                    Instr(Mnemonic.dcf_plds, Dz, mh));  // 111011110000zzzz ### DSP           @@ dcf plds	Dz,MACH     $$ If DC = 0: Dz -> MACH;;Else: nop </div>
                var decoder_F10_F_dsp32 = Mask(12, 4, "   F10..F dsp",
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    Instr(Mnemonic.pcopy, Sy_ym,Dz),        // 1111100100yyzzzz ### DSP           @@ pcopy		Sy,Dz       $$ Sy -> Dz </div>
                    Instr(Mnemonic.dct_pcopy, Sy_ym,Dz),    // 1111101000yyzzzz ### DSP           @@ dct pcopy	Sy,Dz       $$ If DC = 1: Sy -> Dz;;Else: nop </div>
                    Instr(Mnemonic.dcf_pcopy, Sy_ym, Dz),   // 1111101100yyzzzz ### DSP           @@ dcf pcopy	Sy,Dz       $$ If DC = 0: Sy -> Dz;;Else: nop </div>

                    invalid,
                    Instr(Mnemonic.plds, Dz, ml), // 111111010000zzzz ### DSP           @@ plds		Dz,MACL     $$ Dz -> MACL </div>
                    Instr(Mnemonic.dct_plds, Dz, ml), // 111111100000zzzz ### DSP           @@ dct plds	Dz,MACL     $$ If DC = 1: Dz -> MACL;;Else: nop </div>
                    Instr(Mnemonic.dcf_plds, Dz, ml)); // 111111110000zzzz ### DSP           @@ dcf plds	Dz,MACL     $$ If DC = 0: Dz -> MACL;;Else: nop </div>

                var decoder_F10_dsp32 = Mask(12, 4, "  F10.. dsp",
                    If(11, 1, u => u == 0, Instr(Mnemonic.psha, i7, Dz)),
                    If(11, 1, u => u == 0, Instr(Mnemonic.pshl, i7, Dz)),
                // 111110********** 00000iiiiiiizzzz ### DSP           @@ psha		#imm,Dz     $$ If imm >= 0: Dz << imm -> Dz;;If imm < 0: Dz >> imm -> Dz </div>
                // 111110********** 00010iiiiiiizzzz ### DSP           @@ pshl		#imm,Dz     $$ If imm >= 0: Dz << imm -> Dz, clear LSW of Dz;;If imm < 0: Dz >> imm, clear LSW of Dz </div>
                    invalid,
                    invalid,

                // 111110********** 0100eeff0000gg00 ### DSP           @@ pmuls	Se,Sf,Dg        $$ MSW of Se * MSW of Sf -> Dg </div>
                    Instr(Mnemonic.pmuls, Se, Sf, Dg),
                    invalid,
                // 111110********** 0110eeffxxyygguu ### DSP           @@ psub		Sx,Sy,Du;;pmuls		Se,Sf,Dg $$ Sx - Sy -> Du;;MSW of Se * MSW of Sf -> Dg </div>
                // 111110********** 0111eeffxxyygguu ### DSP           @@ padd		Sx,Sy,Du;;pmuls		Se,Sf,Dg $$ Sx + Sy -> Du;;MSW of Se * MSW of Sf -> Dg </div>
                    Nyi("psub/pmuls"),
                    Nyi("psub/pmuls"),

                    decoder_F10_8_dsp32,
                    decoder_F10_9_dsp32,
                    decoder_F10_A_dsp32,
                    decoder_F10_B_dsp32,
                    decoder_F10_C_dsp32,
                    decoder_F10_D_dsp32,
                    decoder_F10_E_dsp32,
                    decoder_F10_F_dsp32);

                var decoder_F11_dsp32 = Nyi("    F10.. dsp");


                var decoders_F_dsp = Mask(10, 2, "  F... DSP",
                    new NyiDecoder<SuperHDisassembler, Mnemonic, SuperHInstruction>("  00"),
                    // 111100*0*0*0**00 ### DSP   @@ nopy               $$ No Operation
                    // 111100*A*D*0**01 ### DSP   @@ movy.w	@Ay,Dy      $$ (Ay) -> MSW of Dy, 0 -> LSW of Dy </div>
                    // 111100*A*D*0**10 ### DSP   @@ movy.w	@Ay+,Dy     $$ (Ay) -> MSW of Dy, 0 -> LSW of Dy, Ay+2 -> Ay </div>
                    // 111100*A*D*0**11 ### DSP   @@ movy.w	@Ay+Iy,Dy   $$ (Ay) -> MSW of Dy, 0 -> LSW of Dy, Ay+Iy -> Ay </div>

                    // 111100*A*D*1**01 ### DSP   @@ movy.w	Da,@Ay      $$ MSW of Da -> (Ay) </div>
                    // 111100*A*D*1**10 ### DSP   @@ movy.w	Da,@Ay+     $$ MSW of Da -> (Ay), Ay+2 -> Ay </div>
                    // 111100*A*D*1**11 ### DSP   @@ movy.w	Da,@Ay+Iy   $$ MSW of Da -> (Ay), Ay+Iy -> Ay </div>

                    // 1111000*0*0*00** ### DSP   @@ nopx               $$ No operation
                    // 111100A*D*0*01** ### DSP   @@ movx.w	@Ax,Dx      $$ (Ax) -> MSW of Dx, 0 -> LSW of Dx </div>
                    // 111100A*D*0*10** ### DSP   @@ movx.w	@Ax+,Dx     $$ (Ax) -> MSW of Dx, 0 -> LSW of Dx, Ax+2 -> Ax </div>
                    // 111100A*D*0*11** ### DSP   @@ movx.w	@Ax+Ix,Dx   $$ (Ax) -> MSW of Dx, 0 -> LSW of Dx, Ax+Ix -> Ax </div>

                    // 111100A*D*1*01** ### DSP   @@ movx.w	Da,@Ax      $$ MSW of Da -> (Ax) </div>
                    // 111100A*D*1*10** ### DSP   @@ movx.w	Da,@Ax+     $$ MSW of Da -> (Ax), Ax+2 -> Ax </div>
                    // 111100A*D*1*11** ### DSP   @@ movx.w	Da,@Ax+Ix   $$ MSW of Da -> (Ax), Ax+Ix -> Ax </div>
                    new NyiDecoder<SuperHDisassembler, Mnemonic, SuperHInstruction>("  01"),

                // 111101AADDDD0000 ### DSP   @@ movs.w	@-As,Ds     $$ As-2 -> As, (As) -> MSW of Ds, 0 -> LSW of Ds </div>
                // 111101AADDDD0001 ### DSP   @@ movs.w	Ds,@-As     $$ As-2 -> As, MSW of Ds -> (As) </div>
                // 111101AADDDD0010 ### DSP   @@ movs.l	@-As,Ds     $$ As-4 -> As, (As) -> Ds </div>
                // 111101AADDDD0011 ### DSP   @@ movs.l	Ds,@-As     $$ As-4 -> As, Ds -> (As) </div>
                // 111101AADDDD0100 ### DSP   @@ movs.w	@As,Ds      $$ (As) -> MSW of Ds, 0 -> LSW of Ds </div>
                // 111101AADDDD0101 ### DSP   @@ movs.w	Ds,@As      $$ MSW of Ds -> (As) </div>
                // 111101AADDDD0110 ### DSP   @@ movs.l	@As,Ds      $$ (As) -> Ds </div>
                // 111101AADDDD0111 ### DSP   @@ movs.l	Ds,@As      $$ Ds -> (As) </div>
                // 111101AADDDD1000 ### DSP   @@ movs.w	@As+,Ds     $$ (As) -> MSW of Ds, 0 -> LSW of Ds, As+2 -> As </div>
                // 111101AADDDD1001 ### DSP   @@ movs.w	Ds,@As+     $$ MSW of Ds -> (As), As+2 -> As </div>
                // 111101AADDDD1010 ### DSP   @@ movs.l	@As+,Ds     $$ (As) -> Ds, As+4 -> As </div>
                // 111101AADDDD1011 ### DSP   @@ movs.l	Ds,@As+     $$ Ds -> (As), As+4 -> As </div>
                // 111101AADDDD1100 ### DSP   @@ movs.w	@As+Ix,Ds   $$ (As) -> MSW of Ds, 0 -> LSW of DS, As+Ix -> As </div>
                // 111101AADDDD1101 ### DSP   @@ movs.w	Ds,@As+Is   $$ MSW of DS -> (As), As+Is -> As </div>
                // 111101AADDDD1110 ### DSP   @@ movs.l	@As+Is,Ds   $$ (As) -> Ds, As+Is -> As </div>
                // 111101AADDDD1111 ### DSP   @@ movs.l	Ds,@As+Is   $$ Ds -> (As), As+Is -> As </div>
                    Instr32(decoder_F10_dsp32),
                    invalid);

                var decode_FxFD = Mask(8, 4,
                    Instr(Mnemonic.fsca, fpul, f1),
                    Instr(Mnemonic.ftrv, xmtrx, v2),
                    Instr(Mnemonic.fsca, fpul, f1),
                    Instr(Mnemonic.fschg),

                    Instr(Mnemonic.fsca, fpul, f1),
                    Instr(Mnemonic.ftrv, xmtrx, v2),
                    Instr(Mnemonic.fsca, fpul, f1),
                    invalid,

                    Instr(Mnemonic.fsca, fpul, f1),
                    Instr(Mnemonic.ftrv, xmtrx, v2),
                    Instr(Mnemonic.fsca, fpul, f1),
                    Instr(Mnemonic.frchg),

                    Instr(Mnemonic.fsca, fpul, f1),
                    Instr(Mnemonic.ftrv, xmtrx, v2),
                    Instr(Mnemonic.fsca, fpul, f1),
                    invalid);

                var decode_FxxD = Sparse(4, 4, "  FxxD", invalid,
                    (0x00, Mask(8, 1,
                        Instr(Mnemonic.fsts, fpul, d1),
                        Instr(Mnemonic.fsts, fpul, f1))),
                    (0x01, Mask(8, 1,
                        Instr(Mnemonic.flds, d1, fpul),
                        Instr(Mnemonic.flds, f1, fpul))),
                    (0x02, Mask(8, 1,
                        Instr(Mnemonic.@float, fpul, d1),
                        Instr(Mnemonic.@float, fpul, f1))),
                    (0x03, Mask(8, 1,
                        Instr(Mnemonic.ftrc, d1, fpul),
                        Instr(Mnemonic.ftrc, f1, fpul))),
                    (0x04, Mask(8, 1,
                        Instr(Mnemonic.fneg, d1),
                        Instr(Mnemonic.fneg, f1))),
                    (0x05, Mask(8, 1,
                        Instr(Mnemonic.fabs, d1),
                        Instr(Mnemonic.fabs, f1))),
                    (0x06, Mask(8, 1,
                        Instr(Mnemonic.fsqrt, d1),
                        Instr(Mnemonic.fsqrt, f1))),
                    (0x08, Instr(Mnemonic.fldi0, f1)),
                    (0x09, Instr(Mnemonic.fldi1, f1)),
                    (0x0A, Instr(Mnemonic.fcnvsd, fpul, d1)),
                    (0x0B, Instr(Mnemonic.fcnvds, d1, fpul)),
                    (0x0E, Instr(Mnemonic.fipr, v2, v1)),
                    (0x0F, decode_FxFD));

                var decoders_F_nondsp = Mask(0, 4, "  F...",
                    Select(bfFpuBinop, u => u == 0, "F..0",
                            Instr(Mnemonic.fadd, d2, d1),
                            Instr(Mnemonic.fadd, f2, f1)),
                    Select(bfFpuBinop, u => u == 0, "F..1",
                            Instr(Mnemonic.fsub, d2, d1),
                            Instr(Mnemonic.fsub, f2, f1)),
                    Select(bfFpuBinop, u => u == 0, "F..2",
                            Instr(Mnemonic.fmul, d2, d1),
                            Instr(Mnemonic.fmul, f2, f1)),
                    Select(bfFpuBinop, u => u == 0, "F..3",
                            Instr(Mnemonic.fdiv, d2, d1),
                            Instr(Mnemonic.fdiv, f2, f1)),

                    Select(bfFpuBinop, u => u == 0, "F..4",
                            Instr(Mnemonic.fcmp_eq, d2, d1),
                            Instr(Mnemonic.fcmp_eq, f2, f1)),
                    Select(bfFpuBinop, u => u == 0, "F..5",
                            Instr(Mnemonic.fcmp_gt, d2, d1),
                            Instr(Mnemonic.fcmp_gt, f2, f1)),
                    Mask(8, 1, "F..6",
                        Instr(Mnemonic.fmov_d, X1d, d2),
                        Instr(Mnemonic.fmov_s, X1l, f2)),
                    Mask(4, 1, "F..7",
                        Instr(Mnemonic.fmov_d, d2, X1d),
                        Instr(Mnemonic.fmov_s, f2, X1l)),

                    Mask(4, 1, "F..8",
                        Instr(Mnemonic.fmov_d, Ind1d, d2),
                        Instr(Mnemonic.fmov_s, Ind1l, f2)),
                    Mask(8, 1, "F..9",
                        Instr(Mnemonic.fmov_d, Post2d, d1),
                        Instr(Mnemonic.fmov_s, Post2l, f1)),
                    Mask(4, 1, "F..A",
                        Instr(Mnemonic.fmov_d, d2, Ind1d),
                        Instr(Mnemonic.fmov_s, f2, Ind1l)),
                    Mask(4, 1, "F..B",
                        Instr(Mnemonic.fmov_d, d2, Pre1d),
                        Instr(Mnemonic.fmov_s, f2, Pre1l)),

                    Select(bfFpuBinop, u => u == 0, "F..C",
                        Instr(Mnemonic.fmov, d2, d1),
                        Instr(Mnemonic.fmov, f2, f1)),
                    decode_FxxD,
                    Instr(Mnemonic.fmac, F0, f2, f1),
                    invalid
            // 1111nnnnmmmm0000 ### SH2E SH3E  SH4 SH4A SH2A       @@ fadd	FRm,FRn         $$ FRn + FRm -> FRn </div>
            // 1111nnn0mmm00000 ###            SH4 SH4A SH2A       @@ fadd	DRm,DRn         $$ DRn + DRm -> DRn </div>
            // 1111nnnnmmmm0001 ### SH2E SH3E  SH4 SH4A SH2A       @@ fsub	FRm,FRn         $$ FRn - FRm -> FRn </div>
            // 1111nnn0mmm00001 ###            SH4 SH4A SH2A       @@ fsub	DRm,DRn         $$ DRn - DRm -> DRn </div>
            // 1111nnnnmmmm0010 ### SH2E SH3E  SH4 SH4A SH2A       @@ fmul	FRm,FRn         $$ FRn * FRm -> FRn </div>
            // 1111nnn0mmm00010 ###            SH4 SH4A SH2A       @@ fmul	DRm,DRn         $$ DRn * DRm -> DRn </div>
            // 1111nnnnmmmm0011 ### SH2E SH3E  SH4 SH4A SH2A       @@ fdiv	FRm,FRn         $$ FRn / FRm -> FRn </div>
            // 1111nnn0mmm00011 ###            SH4 SH4A SH2A       @@ fdiv	DRm,DRn         $$ DRn / DRm -> DRn </div>

            // 1111nnnnmmmm0100 ### SH2E SH3E  SH4 SH4A SH2A       @@ fcmp/eq	FRm,FRn     $$ If FRn = FRm: 1 -> T;;Else: 0 -> T </div>
            // 1111nnn0mmm00100 ###            SH4 SH4A SH2A       @@ fcmp/eq	DRm,DRn     $$ If DRn = DRm: 1 -> T;; Else: 0 -> T </div>
            // 1111nnnnmmmm0101 ### SH2E SH3E  SH4 SH4A SH2A       @@ fcmp/gt	FRm,FRn     $$ If FRn > FRm: 1 -> T;;Else: 0 -> T </div>
            // 1111nnn0mmm00101 ###            SH4 SH4A SH2A       @@ fcmp/gt	DRm,DRn     $$ If DRn > DRm: 1 -> T;; Else: 0 -> T </div>
            // 1111nnnnmmmm0110 ### SH2E SH3E  SH4 SH4A SH2A       @@ fmov.s	@(R0,Rm),FRn $$ (R0 + Rm) -> FRn </div>
            // 1111nnn0mmmm0110 ###            SH4 SH4A SH2A       @@ fmov.d	@(R0,Rm),DRn $$ (R0 + Rm) -> DRn </div>
            // 1111nnn1mmmm0110 ###            SH4 SH4A            @@ fmov.d	@(R0,Rm),XDn $$ (R0 + Rm) -> XDn </div>
            // 1111nnnnmmmm0111 ### SH2E SH3E  SH4 SH4A SH2A       @@ fmov.s	FRm,@(R0,Rn) $$ FRm -> (R0 + Rn) </div>

            // 1111nnnnmmmm1000 ### SH2E SH3E  SH4 SH4A SH2A       @@ fmov.s	@Rm,FRn     $$ (Rm) -> FRn </div>

            // 1111nnn0mmmm1000 ###            SH4 SH4A SH2A       @@ fmov.d	@Rm,DRn     $$ (Rm) -> DRn </div>
            // 1111nnn1mmmm1000 ###            SH4 SH4A            @@ fmov.d	@Rm,XDn     $$ (Rm) -> XDn </div>
            // 1111nnnnmmmm1001 ### SH2E SH3E  SH4 SH4A SH2A       @@ fmov.s	@Rm+,FRn    $$ (Rm) -> FRn, Rm+4 -> Rm </div>
            // 1111nnn0mmmm1001 ###            SH4 SH4A SH2A       @@ fmov.d	@Rm+,DRn    $$ (Rm) -> DRn, Rm + 8 -> Rm </div>
            // 1111nnn1mmmm1001 ###            SH4 SH4A            @@ fmov.d	@Rm+,XDn    $$ (Rm) -> XDn, Rm+8 -> Rm </div>
            // 1111nnnnmmmm1010 ### SH2E SH3E  SH4 SH4A SH2A       @@ fmov.s	FRm,@Rn     $$ FRm -> (Rn) </div>
            // 1111nnnnmmmm1011 ### SH2E SH3E  SH4 SH4A SH2A       @@ fmov.s	FRm,@-Rn    $$ Rn-4 -> Rn, FRm -> (Rn) </div>
            // 1111nnnnmmmm1100 ### SH2E SH3E  SH4 SH4A SH2A       @@ fmov	FRm,FRn         $$ FRm -> FRn </div>
            // 1111nnn0mmm01100 ###            SH4 SH4A SH2A       @@ fmov	DRm,DRn         $$ DRm -> DRn </div>
            // 1111nnn0mmm11100 ###            SH4 SH4A            @@ fmov	XDm,DRn         $$ XDm -> DRn </div>
            // 1111nnn1mmm01100 ###            SH4 SH4A            @@ fmov	DRm,XDn         $$ DRm -> XDn </div>
            // 1111nnn1mmm11100 ###            SH4 SH4A            @@ fmov	XDm,XDn         $$ XDm -> XDn </div>

            // 1111nnnn00001101 ### SH2E SH3E  SH4 SH4A SH2A       @@ fsts	FPUL,FRn        $$ FPUL -> FRn </div>
            // 1111mmmm00011101 ### SH2E SH3E SH4 SH4A SH2A        @@ flds	FRm,FPUL        $$ FRm -> FPUL </div>
            // 1111nnnn00101101 ### SH2E SH3E  SH4 SH4A SH2A       @@ float	FPUL,FRn        $$ (float)FPUL -> FRn </div>
            // 1111mmmm00111101 ### SH2E SH3E SH4 SH4A SH2A   Priv @@ ftrc	FRm,FPUL        $$ (long)FRm -> FPUL </div>
            // 1111mmm000111101 ### SH4 SH4A SH2A                  @@ ftrc	DRm,FPUL        $$ (long)DRm -> FPUL </div>
            // 1111mmm010111101 ### SH4 SH4A SH2A                  @@ fcnvds	DRm,FPUL    $$ double_to_float (DRm) -> FPUL </div>
            // 1111nn0111111101 ###             SH4 SH4A           @@ ftrv	XMTRX,FVn       $$ transform_vector (XMTRX, FVn) -> FVn </div>
            // 1111nnmm11101101 ###             SH4 SH4A           @@ fipr	FVm,FVn         $$ inner_product (FVm, FVn) -> FR[n+3] </div>
            // 1111nnn000101101 ###            SH4 SH4A SH2A       @@ float	FPUL,DRn        $$ (double)FPUL -> DRn </div>
            // 1111nnn001001101 ###            SH4 SH4A SH2A       @@ fneg	DRn             $$ DRn ^ 0x8000000000000000 -> DRn </div>
            // 1111nnn001011101 ###            SH4 SH4A SH2A       @@ fabs	DRn             $$ DRn & 0x7FFFFFFFFFFFFFFF -> DRn </div>
            // 1111nnn001101101 ###            SH4 SH4A SH2A       @@ fsqrt	DRn             $$ sqrt (DRn) -> DRn </div>
            // 1111nnn010101101 ###            SH4 SH4A SH2A       @@ fcnvsd	FPUL,DRn    $$ float_to_double (FPUL) -> DRn </div>
            // 1111nnn011111101 ###                SH4A            @@ fsca	FPUL,DRn        $$ sin (FPUL) -> FRn;; cos (FPUL) -> FR[n+1] </div>
            // 1111001111111101 ###             SH4 SH4A SH2A      @@ fschg                 $$ If FPSCR.PR = 0: ~FPSCR.SZ -> FPSCR.SZ;; Else: Undefined Operation </div>
            // 1111011111111101 ###                 SH4A           @@ fpchg                 $$ ~FPSCR.PR -> FPSCR.PR </div>
            // 1111101111111101 ###             SH4 SH4A           @@ frchg                 $$ If FPSCR.PR = 0: ~FPSCR.FR -> FPSCR.FR;; Else: Undefined Operation </div>
            // 1111nnnn01001101 ### SH2E SH3E  SH4 SH4A SH2A       @@ fneg	FRn             $$ FRn ^ 0x80000000 -> FRn </div>
            // 1111nnnn01011101 ### SH2E SH3E  SH4 SH4A SH2A       @@ fabs	FRn             $$ FRn & 0x7FFFFFFF -> FRn </div>
            // 1111nnnn01101101 ###       SH3E SH4 SH4A SH2A       @@ fsqrt	FRn             $$ sqrt (FRn) -> FRn </div>
            // 1111nnnn01111101 ###                SH4A            @@ fsrra	FRn             $$ 1.0 / sqrt (FRn) -> FRn </div>
            // 1111nnnn10001101 ### SH2E SH3E  SH4 SH4A SH2A       @@ fldi0	FRn             $$ 0x00000000 -> FRn </div>
            // 1111nnnn10011101 ### SH2E SH3E  SH4 SH4A SH2A       @@ fldi1	FRn             $$ 0x3F800000 -> FRn </div>
            // 1111nnnnmmm00111 ###            SH4 SH4A SH2A       @@ fmov.d	DRm,@(R0,Rn) $$ DRm -> (R0 + Rn) </div>
            // 1111nnnnmmm01010 ###            SH4 SH4A SH2A       @@ fmov.d	DRm,@Rn     $$ DRm -> (Rn) </div>
            // 1111nnnnmmm01011 ###            SH4 SH4A SH2A       @@ fmov.d	DRm,@-Rn    $$ Rn-8 -> Rn, DRm -> (Rn) </div>
            // 1111nnnnmmm10111 ###            SH4 SH4A            @@ fmov.d	XDm,@(R0,Rn) $$ XDm -> (R0 + Rn) </div>
            // 1111nnnnmmm11010 ###            SH4 SH4A            @@ fmov.d	XDm,@Rn     $$ XDm -> (Rn) </div>
            // 1111nnnnmmm11011 ###            SH4 SH4A            @@ fmov.d	XDm,@-Rn    $$ Rn-8 -> Rn, (Rn) -> XDm </div>
            // 1111nnnnmmmm1110 ### SH2E SH3E  SH4 SH4A SH2A       @@ fmac	FR0,FRm,FRn     $$ FR0 * FRm + FRn -> FRn </div>

            );

                var decoders_F = DspModel(
                    decoders_F_dsp,
                    decoders_F_nondsp);


                return Mask(12, 4, "SuperH",
                    decoders_0,
                    decoders_1,
                    decoders_2,
                    decoders_3,

                    decoders_4,
                    decoders_5,
                    decoders_6,
                    decoders_7,

                    decoders_8,
                    decoders_9,
                    decoders_A,
                    decoders_B,

                    decoders_C,
                    decoders_D,
                    decoders_E,
                    decoders_F);
            }

            private Decoder Instr_2(Mnemonic mnemonic, params Mutator<SuperHDisassembler> [] mutators)
            {
                return mkSh2_3_4_2a(mnemonic, InstrClass.Linear, mutators);
            }


            private Decoder Instr_2(Mnemonic mnemonic, InstrClass iclass, params Mutator<SuperHDisassembler>[] mutators)
            {
                return mkSh2_3_4_2a(mnemonic, iclass, mutators);
            }

            private Decoder Instr_2E_3E_4_2A(Mnemonic mnemonic, params Mutator<SuperHDisassembler>[] mutators)
            {
                return mkSh2e_3e_4_2a(mnemonic, InstrClass.Linear, mutators);
            }

            private Decoder Instr_2a(Mnemonic mnemonic, params Mutator<SuperHDisassembler>[] mutators)
            {
                return mkSh2a(mnemonic, InstrClass.Linear, mutators);
            }

            private Decoder Instr_2a(Mnemonic mnemonic, InstrClass iclass, params Mutator<SuperHDisassembler>[] mutators)
            {
                return mkSh2a(mnemonic, iclass, mutators);
            }

            private Decoder Instr32_2a(Decoder decoder)
            {
                if (this.isSh2aModel)
                    return new Instr32Decoder(decoder);
                else
                    return invalid;
            }

            private Decoder Instr_3_4(Mnemonic mnemonic, params Mutator<SuperHDisassembler>[] mutators)
            {
                return mkSh3_4(mnemonic, InstrClass.Linear, mutators);
            }

            private Decoder Instr_3_4(Mnemonic mnemonic, InstrClass iclass, params Mutator<SuperHDisassembler>[] mutators)
            {
                return mkSh3_4(mnemonic, iclass, mutators);
            }

            private Decoder Instr_3_4_2a(Mnemonic mnemonic, params Mutator<SuperHDisassembler>[] mutators)
            {
                return mkSh3_4_2a(mnemonic, InstrClass.Linear, mutators);
            }

            private Decoder Instr_4(Mnemonic mnemonic, params Mutator<SuperHDisassembler>[] mutators) 
            {
                return mkSh4_4a(mnemonic, InstrClass.Linear, mutators);
            }

            private Decoder Instr_4(Mnemonic mnemonic, InstrClass iclass, params Mutator<SuperHDisassembler>[] mutators)
            {
                return mkSh4_4a(mnemonic, iclass, mutators);
            }

            private Decoder Instr_4a(Mnemonic mnemonic, params Mutator<SuperHDisassembler>[] mutators)
            {
                return mkSh4a(mnemonic, InstrClass.Linear, mutators);
            }

            private Decoder Instr_4a(Mnemonic mnemonic, InstrClass iclass, params Mutator<SuperHDisassembler>[] mutators)
            {
                return mkSh4a(mnemonic, iclass, mutators);
            }

            private Decoder Instr_DSP(Mnemonic mnemonic, params Mutator<SuperHDisassembler>[] mutators)
            {
                return mkShDsp(mnemonic, InstrClass.Linear, mutators);
            }

            private Decoder DspModel(Decoder dspDecoder, Decoder nonDspDecoder)
            {
                return (isDspModel) ? dspDecoder : nonDspDecoder;
            }
        }
    }
}