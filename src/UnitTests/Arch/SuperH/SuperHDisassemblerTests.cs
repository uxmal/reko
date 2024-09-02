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

using NUnit.Framework;
using Reko.Arch.SuperH;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.Tlcs
{
    [TestFixture]
    public class SuperHDisassemblerTests : DisassemblerTestBase<SuperHInstruction>
    {
        private readonly SuperHArchitecture arch2a;
        private readonly SuperHArchitecture arch4a;
        private readonly SuperHArchitecture archDsp;
        private SuperHArchitecture arch;

        public SuperHDisassemblerTests()
        {
            this.arch2a = new SuperHArchitecture(new ServiceContainer(), "superH", new Dictionary<string, object>
            {
                { ProcessorOption.Endianness, "le" },
                { ProcessorOption.Model, "sh2a" }
            });
            this.arch4a = new SuperHArchitecture(new ServiceContainer(), "superH", new Dictionary<string, object>
            {
                { ProcessorOption.Endianness, "le" },
                { ProcessorOption.Model, "sh4a" }
            });
            this.archDsp = new SuperHArchitecture(new ServiceContainer(), "superH", new Dictionary<string, object>
            {
                { ProcessorOption.Endianness, "le" },
                { ProcessorOption.Model, "dsp" }
            });
            this.arch = arch4a;
            this.LoadAddress = Address.Ptr32(0x00010000);
            Reko.Core.Machine.Decoder.trace.Level = System.Diagnostics.TraceLevel.Verbose;
        }

        public override IProcessorArchitecture Architecture => arch;


        public override Address LoadAddress { get; }


        private void AssertCode_Sh2a(string sExp, string hexBytes)
        {
            arch = arch2a;
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void AssertCode_Sh4a(string sExp, string hexBytes)
        {
            arch = arch4a;
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void AssertCode_Dsp(string sExp, string hexBytes)
        {
            arch = archDsp;
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void SHDis_add_imm_rn()
        {
            AssertCode_Sh4a("add\t#FF,r3", "FF73");
        }

        [Test]
        public void SHDis_add_rm_rn()
        {
            AssertCode_Sh4a("add\tr4,r2", "4C32");
        }

        [Test]
        public void SHDis_addc_rm_rn()
        {
            AssertCode_Sh4a("addc\tr4,r2", "4E32");
        }

        [Test]
        public void SHDis_addv_rm_rn()
        {
            AssertCode_Sh4a("addv\tr4,r2", "4F32");
        }

        [Test]
        public void SHDis_and_rm_rn()
        {
            AssertCode_Sh4a("and\tr4,r3", "4923");
        }

        [Test]
        public void SHDis_and_imm_r0()
        {
            AssertCode_Sh4a("and\t#F0,r0", "F0C9");
        }

        [Test]
        public void SHDis_and_b_imm_r0()
        {
            AssertCode_Sh4a("and.b\t#F0,@(r0,gbr)", "F0CD");
        }

        [Test]
        public void SHDis_bf()
        {
            AssertCode_Sh4a("bf\t0000FFE4", "F08B");
        }

        [Test]
        public void SHDis_bf_s()
        {
            AssertCode_Sh4a("bf/s\t0000FFE4", "F08F");
        }

        [Test]
        public void SHDis_bra()
        {
            AssertCode_Sh4a("bra\t0000FFE4", "F0AF");
        }

        [Test]
        public void SHDis_braf_reg()
        {
            AssertCode_Sh4a("braf\tr1", "2301");
        }

        [Test]
        public void SHDis_brk()
        {
            AssertCode_Sh4a("brk", "3B00");
        }

        [Test]
        public void SHDis_bsr()
        {
            AssertCode_Sh4a("bsr\t0000FFE4", "F0BF");
        }

        [Test]
        public void SHDis_bsrf()
        {
            AssertCode_Sh4a("bsrf\tr1", "0301");
        }

        [Test]
        public void SHDis_bt()
        {
            AssertCode_Sh4a("bt\t0000FFE4", "F089");
        }

        [Test]
        public void SHDis_bt_s()
        {
            AssertCode_Sh4a("bt/s\t0000FFE4", "F08D");
        }

        [Test]
        public void SHDis_clrmac()
        {
            AssertCode_Sh4a("clrmac", "2800");
        }

        [Test]
        public void SHDis_cmpeq()
        {
            AssertCode_Sh4a("cmp/eq\tr4,r5", "4035");
        }

        [Test]
        public void SHDis_cmpeq_imm()
        {
            AssertCode_Sh4a("cmp/eq\t#F0,r0", "F088");
        }

        [Test]
        public void SHDis_div0s()
        {
            AssertCode_Sh4a("div0s\tr4,r3", "4723");
        }

        [Test]
        public void SHDis_div0u()
        {
            AssertCode_Sh4a("div0u", "1900");
        }

        [Test]
        public void SHDis_div1()
        {
            AssertCode_Sh4a("div1\tr4,r3", "4433");
        }

        [Test]
        public void SHDis_dmuls_l()
        {
            AssertCode_Sh4a("dmuls.l\tr4,r3", "4D33");
        }

        [Test]
        public void SHDis_dt()
        {
            AssertCode_Sh4a("dt\tr15", "104F");
        }

        [Test]
        public void SHDis_exts_b()
        {
            AssertCode_Sh4a("exts.b\tr15,r14", "FE6E");
        }

        [Test]
        public void SHDis_exts_w()
        {
            AssertCode_Sh4a("exts.w\tr15,r14", "FF6E");
        }

        [Test]
        public void SHDis_extu_b()
        {
            AssertCode_Sh4a("extu.b\tr15,r14", "FC6E");
        }

        [Test]
        public void SHDis_extu_w()
        {
            AssertCode_Sh4a("extu.w\tr15,r14", "FD6E");
        }

        [Test]
        public void SHDis_fabs_dr()
        {
            AssertCode_Sh4a("fabs\tdr14", "5DFE");
        }

        [Test]
        public void SHDis_fabs_fr()
        {
            AssertCode_Sh4a("fabs\tfr15", "5DFF");
        }

        [Test]
        public void SHDis_fadd_dr()
        {
            AssertCode_Sh4a("fadd\tdr12,dr14", "C0FE");
        }

        [Test]
        public void SHDis_fadd_fr()
        {
            AssertCode_Sh4a("fadd\tfr12,fr15", "C0FF");
        }

        [Test]
        public void SHDis_fcmp_eq_dr()
        {
            AssertCode_Sh4a("fcmp/eq\tdr12,dr14", "C4FE");
        }

        [Test]
        public void SHDis_fcmp_eq_fr()
        {
            AssertCode_Sh4a("fcmp/eq\tfr12,fr15", "C4FF");
        }

        [Test]
        public void SHDis_fcmp_gt_dr()
        {
            AssertCode_Sh4a("fcmp/gt\tdr12,dr14", "C5FE");
        }

        [Test]
        public void SHDis_fcmp_gt_fr()
        {
            AssertCode_Sh4a("fcmp/gt\tfr12,fr15", "C5FF");
        }

        [Test]
        public void SHDis_fcnvds()
        {
            AssertCode_Sh4a("fcnvds\tdr14,fpul", "BDFE");
        }

        [Test]
        public void SHDis_fcnvsd()
        {
            AssertCode_Sh4a("fcnvsd\tfpul,dr14", "ADFE");
        }

        [Test]
        public void SHDis_fdiv_dr()
        {
            AssertCode_Sh4a("fdiv\tdr12,dr14", "C3FE");
        }


        [Test]
        public void ShDis_fdiv_dr_2()
        {
            AssertCode_Sh4a("fdiv\tdr12,dr8", "C3F8");
        }

        [Test]
        public void SHDis_fdiv_fr()
        {
            AssertCode_Sh4a("fdiv\tfr12,fr15", "C3FF");
        }

        [Test]
        public void SHDis_fipr()
        {
            AssertCode_Sh4a("fipr\tfv8,fv12", "EDFE");
        }

        [Test]
        public void SHDis_flds_dr()
        {
            AssertCode_Sh4a("flds\tdr8,fpul", "1DF8");
        }

        [Test]
        public void SHDis_fldi0()
        {
            AssertCode_Sh2a("fldi0\tfr8", "8DF8");
        }

        [Test]
        public void SHDis_fldi1()
        {
            AssertCode_Sh4a("fldi1\tfr8", "9DF8");
        }

        [Test]
        public void SHDis_jmp_r()
        {
            AssertCode_Sh4a("jmp\t@r0", "2B40");
        }

        [Test]
        public void SHDis_lds_l_pr()
        {
            AssertCode_Sh4a("lds.l\t@r15+,pr", "264F");
        }

        [Test]
        public void SHDis_mov_I_r()
        {
            AssertCode_Sh4a("mov\t#FF,r1", "FFE1");
        }

        [Test]
        public void SHDis_mov_l_predec()
        {
            AssertCode_Sh4a("mov.l\tr8,@-r15", "862F");
        }

        [Test]
        public void SHDis_mov_l_disp_pc()
        {
            AssertCode_Sh4a("mov.l\t@(08,pc),r0", "02D0");
        }

        [Test]
        public void SHDis_mov_l_indexed_r()
        {
            AssertCode_Sh4a("mov.l\t@(r0,r12),r1", "CE 01");
        }

        [Test]
        public void SHDis_mov_l_r_indexed()
        {
            AssertCode_Sh4a("mov.l\t@(8,r6),r2", "62 52");
        }

        [Test]
        public void SHDis_mov_l_indirect_r()
        {
            AssertCode_Sh4a("mov.l\t@r1,r2", "12 62");
        }

        [Test]
        public void SHDis_mov_r_r()
        {
            AssertCode_Sh4a("mov\tr7,r4", "7364");
        }

        [Test]
        public void SHDis_mov_w_indirect_r()
        {
            AssertCode_Sh4a("mov.w\tr0,@(r0,r0)", "0500");
        }

        [Test]
        public void SHDis_mov()
        {
            AssertCode_Sh4a("mov.l\t@r8+,r9", "8669");
        }

        [Test]
        public void SHDis_mov_w_pc()
        {
            AssertCode_Sh4a("mov.w\t@(66,pc),r0", "3390");
        }

        [Test]
        public void ShDis_mov_w_pcrel()
        {
            AssertCode_Sh4a("mov.w\t@(9A,pc),r14", "4D9E");
        }

        [Test]
        public void SHDis_mova()
        {
            AssertCode_Sh4a("mova\t@(F8,pc),r0", "3E C7");
        }

        [Test]
        public void SHDis_mul_l()
        {
            AssertCode_Sh4a("mul.l\tr1,r5", "1705");
        }

        [Test]
        public void SHDis_nop()
        {
            AssertCode_Sh4a("nop", "0900");
        }

        [Test]
        public void SHDis_neg()
        {
            AssertCode_Sh4a("neg\tr0,r0", "0B60");
        }

        [Test]
        public void SHDis_not()
        {
            AssertCode_Sh4a("not\tr9,r0", "9760");
        }

        [Test]
        public void SHDis_rts()
        {
            AssertCode_Sh4a("rts", "0B00");
        }

        [Test]
        public void SHDis_shll2()
        {
            AssertCode_Sh4a("shll2\tr0", "0840");
        }

        [Test]
        public void SHDis_shlr2()
        {
            AssertCode_Sh4a("shlr2\tr2", "0942");
        }

        [Test]
        public void SHDis_sts_l_pr_predec()
        {
            AssertCode_Sh4a("sts.l\tpr,@-r15", "224F");
        }

        [Test]
        public void SHDis_tst_imm()
        {
            AssertCode_Sh4a("tst\t#01,r0", "01C8");
        }

        [Test]
        public void SHDis_tst_r_r()
        {
            AssertCode_Sh4a("tst\tr6,r6", "6826");
        }

        [Test]
        public void SHDis_sts_mach()
        {
            AssertCode_Sh4a("sts\tmach,r0", "0A00");
        }

        [Test]
        public void SHDis_sts_shld()
        {
            AssertCode_Sh4a("shld\tr1,r0", "1D40");
        }

        [Test]
        public void SHDis_sts_shad()
        {
            AssertCode_Sh4a("shad\tr1,r0", "1C40");
        }

        [Test]
        public void SHDis_sts_subc()
        {
            AssertCode_Sh4a("subc\tr1,r1", "1A 31");
        }

        [Test]
        public void SHDis_sts_swap_w()
        {
            AssertCode_Sh4a("swap.w\tr4,r0", "4960");
        }

        [Test]
        public void SHDis_sts_shlr_16()
        {
            AssertCode_Sh4a("shlr16\tr4", "2944");
        }

        [Test]
        public void SHDis_sts_shll_16()
        {
            AssertCode_Sh4a("shll16\tr5", "2845");
        }

        [Test]
        public void SHDis_sts_xtrct()
        {
            AssertCode_Sh4a("xtrct\tr4,r0", "4D20");
        }

        [Test]
        public void SHDis_sts_shar()
        {
            AssertCode_Sh4a("shar\tr1", "2141");
        }

        [Test]
        public void SHDis_fcmp_eq()
        {
            AssertCode_Sh4a("fcmp/eq\tfr5,fr9", "﻿54F9");
        }

        [Test]
        public void SHDis_lds_pr()
        {
            AssertCode_Sh4a("lds\tr3,pr", "2A43");
        }

        /////////////////////////////////////////////

        [Test]
        public void ShDis_ocbi()
        {
            AssertCode_Sh4a("ocbi\t@r0", "9300");
        }

        [Test]
        public void ShDis_stc_gbr_r1()
        {
            AssertCode_Sh4a("stc\tgbr,r1", "1201");
        }

        [Test]
        public void ShDis_stc_spc_reg()
        {
            AssertCode_Sh4a("stc\tspc,r4", "4204");
        }

        [Test]
        public void ShDis_sts_dsr()
        {
            AssertCode_Sh4a("sts\tfpscr,r6", "6A06");
        }

        [Test]
        public void ShDis_stc_r0bank()
        {
            AssertCode_Sh4a("stc\tr0_bank,r0", "8200");
        }

        [Test]
        public void ShDis_ocbp()
        {
            AssertCode_Sh4a("ocbp\t@r14", "A30E");
        }

        [Test]
        public void ShDis_std_dbr()
        {
            AssertCode_Sh4a("stc\tdbr,r10", "FA0A");
        }

        [Test]
        public void ShDis_stc_ssr()
        {
            AssertCode_Sh4a("stc\tssr,r1", "3201");
        }

        [Test]
        public void ShDis_sts_pr()
        {
            AssertCode_Sh4a("sts\tpr,r1", "2A01");
        }

        [Test]
        public void ShDis_movco_l()
        {
            AssertCode_Sh4a("movco.l\tr0,@r1", "7301");
        }

        [Test]
        public void ShDis_movca_l()
        {
            AssertCode_Sh4a("movca.l\tr0,@r1", "C301");
        }



        [Test]
        public void ShDis_shal()
        {
            AssertCode_Sh4a("shal\tr2", "2042");
        }

        [Test]
        public void ShDis_ldc_sgr()
        {
            AssertCode_Sh4a("ldc.l\t@r2+,sgr", "3642");
        }

        [Test]
        public void ShDis_tas_b()
        {
            AssertCode_Sh4a("tas.b\t@r3", "1B43");
        }

        [Test]
        public void ShDis_stc_gbr()
        {
            AssertCode_Sh4a("stc.l\tgbr,@-r4", "1344");
        }



        [Test]
        public void ShDis_sts_fpul()
        {
            AssertCode_Sh4a("sts\tfpul,r1", "5A01");
        }

        [Test]
        public void ShDis_lds_fpul()
        {
            AssertCode_Sh4a("lds\tr4,fpul", "5A44");
        }

        [Test]
        public void ShDis_stc_l_bank()
        {
            AssertCode_Sh4a("stc.l\tr5_bank,@-r13", "D34D");
        }

        [Test]
        public void ShDis_mac_w()
        {
            AssertCode_Sh4a("mac.w\t@r3+,@r10+", "3F4A");
        }

        [Test]
        public void ShDis_ldc_l_bank()
        {
            AssertCode_Sh4a("ldc.l\t@r10+,r2_bank", "A74A");
        }

        [Test]
        public void ShDis_lds_l_dsr_post()
        {
            AssertCode_Sh4a("lds.l\t@r12+,fpscr", "664C");
        }

        [Test]
        public void ShDis_lds_l_fpscr_reg()
        {
            AssertCode_Sh4a("lds\tr12,fpscr", "6A4C");
        }

        [Test]
        public void ShDis_fsub()
        {
            AssertCode_Sh4a("fsub\tfr0,fr1", "01F1");
        }

        [Test]
        public void ShDis_fmov_d()
        {
            AssertCode_Sh4a("fmov\tdr2,dr0", "2CF0");
        }

        [Test]
        public void ShDis_fmov_s_idx()
        {
            AssertCode_Sh4a("fmov.s\t@(r0,r1),fr4", "46F1");
        }

        [Test]
        public void ShDis_fmov_s_indir()
        {
            AssertCode_Sh4a("fmov.s\tfr1,@r4", "1AF4");
        }

        [Test]
        public void ShDis_fadd_dr()
        {
            AssertCode_Sh4a("fadd\tfr1,fr3", "10F3");
        }

        [Test]
        public void ShDis_fmov_d_ind()
        {
            AssertCode_Sh4a("fmov.d\t@r0,dr4", "48F0");
        }

        [Test]
        public void ShDis_pref()
        {
            AssertCode_Sh4a("pref\t@r10", "830A");
        }

        [Test]
        public void ShDis_fmul()
        {
            AssertCode_Sh4a("fmul\tfr11,fr0", "B2F0");
        }

        [Test]
        public void ShDis_mac_l()
        {
            AssertCode_Sh4a("mac.l\t@r15+,@r0+", "FF00");
        }

        // A SuperH decoder for the instruction FFFE has not been implemented yet.
        [Test]
        public void ShDis_fmac()
        {
            AssertCode_Sh4a("fmac\tfr0,fr15,fr15", "FEFF");
        }


        [Test]
        public void ShDis_ldc_sr()
        {
            AssertCode_Sh4a("ldc\tr0,sr", "0E40");
        }

        [Test]
        public void ShDis_ldtlb()
        {
            AssertCode_Sh4a("ldtlb", "3800");
        }

        [Test]
        public void ShDis_ldc_vbr()
        {
            AssertCode_Sh4a("ldc\tr1,vbr", "2E41");
        }

        [Test]
        public void ShDis_lds_l_mach()
        {
            AssertCode_Sh4a("lds.l\t@r0+,mach", "0640");
        }

        [Test]
        public void ShDis_ftrc()
        {
            AssertCode_Sh4a("ftrc\tdr2,fpul", "3DF2");
        }

        [Test]
        public void ShDis_fmov_s_predec()
        {
            AssertCode_Sh4a("fmov.s\tfr5,@-r1", "5BF1");
        }

        [Test]
        public void ShDis_fmov_s_postinc()
        {
            AssertCode_Sh4a("fmov.s\t@r1+,fr3", "19F3");
        }

        [Test]
        public void ShDis_fsts_fpul()
        {
            AssertCode_Sh4a("fsts\tfpul,fr1", "0DF1");
        }

        [Test]
        public void ShDis_mov_l()
        {
            AssertCode_Sh2a("mov.l\tr0,@(520,gbr)", "82C2 C282 C304D9422CEDB0CA48A7FA1547");
        }


        [Test]
        public void ShDis_or_b()
        {
            AssertCode_Sh4a("or.b\t#F2,@(r0,gbr)", "F2CF");
        }

        [Test]
        public void ShDis_divs()
        {
            AssertCode_Sh2a("divs\tr0,r3", "9443");
        }

        [Test]
        public void SHDis_jsr_n_double_indirect()
        {
            AssertCode_Sh2a("jsr/n\t@@(1020,tbr)", "FF83");
        }

        [Test]
        public void ShDis_ldc_tbr()
        {
            AssertCode_Sh2a("ldc\tr10,tbr", "4A4A");
        }

        [Test]
        public void SHDis_mov_b_disp12()
        {
            AssertCode_Sh2a("mov.b\tr5,@(4095,r4)", "5134 FF0F");
        }

        [Test]
        public void ShDis_movmu_l_to_regs()
        {
            AssertCode_Sh2a("movmu.l\t@r15+,r8", "F448");
        }

        [Test]
        public void ShDis_movmu_l()
        {
            AssertCode_Sh2a("movmu.l\tr1,@-r15", "F041");
        }

        [Test]
        public void ShDis_mulr()
        {
            AssertCode_Sh2a("mulr\tr0,r6", "8046");
        }

        /* Verify that the rewriter is generating a sign-extended constant here
    R:add   #A6,r15                              7F A6
    O:add   #-90,r15                             7F A6
         */

        [Test]
        public void ShDis_setrc()
        {
            AssertCode_Dsp("setrc\tr10", "144A");
        }

        [Test]
        public void ShDis_stc_mod()
        {
            AssertCode_Dsp("stc\tmod,r0", "5200");
        }

        [Test]
        public void ShDis_stc_rs()
        {
            AssertCode_Dsp("stc\trs,r6", "6206");
        }

        [Test]
        public void ShDis_sts_x0()
        {
            AssertCode_Dsp("sts\tx0,r0", "8A00");
        }

        [Test]
        public void ShDis_sts_x1()
        {
            AssertCode_Dsp("sts\tx1,r0", "9A00");
        }

        [Test]
        public void ShDis_sts_y1()
        {
            AssertCode_Dsp("sts\ty1,r1", "BA01");
        }

    }
}
