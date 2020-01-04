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

using NUnit.Framework;
using Reko.Arch.SuperH;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Tlcs
{
    [TestFixture]
    public class SuperHDisassemblerTests : DisassemblerTestBase<SuperHInstruction>
    {
        private SuperHArchitecture arch;

        public SuperHDisassemblerTests()
        {
            this.arch = new SuperHLeArchitecture("superH");
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress { get { return Address.Ptr32(0x00010000); } }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new LeImageWriter(bytes);
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void SHDis_add_imm_rn()
        {
            AssertCode("add\t#FF,r3", "FF73");
        }

        [Test]
        public void SHDis_add_rm_rn()
        {
            AssertCode("add\tr4,r2", "4C32");
        }

        [Test]
        public void SHDis_addc_rm_rn()
        {
            AssertCode("addc\tr4,r2", "4E32");
        }

        [Test]
        public void SHDis_addv_rm_rn()
        {
            AssertCode("addv\tr4,r2", "4F32");
        }

        [Test]
        public void SHDis_and_rm_rn()
        {
            AssertCode("and\tr4,r3", "4923");
        }

        [Test]
        public void SHDis_and_imm_r0()
        {
            AssertCode("and\t#F0,r0", "F0C9");
        }

        [Test]
        public void SHDis_and_b_imm_r0()
        {
            AssertCode("and.b\t#F0,@(r0,gbr)", "F0CD");
        }

        [Test]
        public void SHDis_bf()
        {
            AssertCode("bf\t0000FFE4", "F08B");
        }

        [Test]
        public void SHDis_bf_s()
        {
            AssertCode("bf/s\t0000FFE4", "F08F");
        }

        [Test]
        public void SHDis_bra()
        {
            AssertCode("bra\t0000FFE4", "F0AF");
        }

        [Test]
        public void SHDis_braf_reg()
        {
            AssertCode("braf\tr1", "2301");
        }

        [Test]
        public void SHDis_brk()
        {
            AssertCode("brk", "3B00");
        }

        [Test]
        public void SHDis_bsr()
        {
            AssertCode("bsr\t0000FFE4", "F0BF");
        }

        [Test]
        public void SHDis_bsrf()
        {
            AssertCode("bsrf\tr1", "0301");
        }

        [Test]
        public void SHDis_bt()
        {
            AssertCode("bt\t0000FFE4", "F089");
        }

        [Test]
        public void SHDis_bt_s()
        {
            AssertCode("bt/s\t0000FFE4", "F08D");
        }

        [Test]
        public void SHDis_clrmac()
        {
            AssertCode("clrmac", "2800");
        }

        [Test]
        public void SHDis_cmpeq()
        {
            AssertCode("cmp/eq\tr4,r5", "4035");
        }

        [Test]
        public void SHDis_cmpeq_imm()
        {
            AssertCode("cmp/eq\t#F0,r0", "F088");
        }

        [Test]
        public void SHDis_div0s()
        {
            AssertCode("div0s\tr4,r3", "4723");
        }

        [Test]
        public void SHDis_div0u()
        {
            AssertCode("div0u", "1900");
        }

        [Test]
        public void SHDis_div1()
        {
            AssertCode("div1\tr4,r3", "4433");
        }

        [Test]
        public void SHDis_dmuls_l()
        {
            AssertCode("dmuls.l\tr4,r3", "4D33");
        }

        [Test]
        public void SHDis_dt()
        {
            AssertCode("dt\tr15", "104F");
        }

        [Test]
        public void SHDis_exts_b()
        {
            AssertCode("exts.b\tr15,r14", "FE6E");
        }

        [Test]
        public void SHDis_exts_w()
        {
            AssertCode("exts.w\tr15,r14", "FF6E");
        }

        [Test]
        public void SHDis_extu_b()
        {
            AssertCode("extu.b\tr15,r14", "FC6E");
        }

        [Test]
        public void SHDis_extu_w()
        {
            AssertCode("extu.w\tr15,r14", "FD6E");
        }

        [Test]
        public void SHDis_fabs_dr()
        {
            AssertCode("fabs\tdr14", "5DFE");
        }

        [Test]
        public void SHDis_fabs_fr()
        {
            AssertCode("fabs\tfr15", "5DFF");
        }

        [Test]
        public void SHDis_fadd_dr()
        {
            AssertCode("fadd\tdr12,dr14", "C0FE");
        }

        [Test]
        public void SHDis_fadd_fr()
        {
            AssertCode("fadd\tfr12,fr15", "C0FF");
        }

        [Test]
        public void SHDis_fcmp_eq_dr()
        {
            AssertCode("fcmp/eq\tdr12,dr14", "C4FE");
        }

        [Test]
        public void SHDis_fcmp_eq_fr()
        {
            AssertCode("fcmp/eq\tfr12,fr15", "C4FF");
        }

        [Test]
        public void SHDis_fcmp_gt_dr()
        {
            AssertCode("fcmp/gt\tdr12,dr14", "C5FE");
        }

        [Test]
        public void SHDis_fcmp_gt_fr()
        {
            AssertCode("fcmp/gt\tfr12,fr15", "C5FF");
        }

        [Test]
        public void SHDis_fcnvds()
        {
            AssertCode("fcnvds\tdr14,fpul", "BDFE");
        }

        [Test]
        public void SHDis_fcnvsd()
        {
            AssertCode("fcnvsd\tfpul,dr14", "ADFE");
        }

        [Test]
        public void SHDis_fdiv_dr()
        {
            AssertCode("fdiv\tdr12,dr14", "C3FE");
        }

        [Test]
        public void SHDis_fdiv_fr()
        {
            AssertCode("fdiv\tfr12,fr15", "C3FF");
        }

        [Test]
        public void SHDis_fipr()
        {
            AssertCode("fipr\tfv8,fv12", "EDFE");
        }

        [Test]
        public void SHDis_flds_dr()
        {
            AssertCode("flds\tdr8,fpul", "1DF8");
        }

        [Test]
        public void SHDis_fldi0()
        {
            AssertCode("fldi0\tfr8", "8DF8");
        }

        [Test]
        public void SHDis_fldi1()
        {
            AssertCode("fldi1\tfr8", "9DF8");
        }

        [Test]
        public void SHDis_jmp_r()
        {
            AssertCode("jmp\t@r0", "2B40");
        }

        [Test]
        public void SHDis_lds_l_pr()
        {
            AssertCode("lds.l\t@r15+,pr", "264F");
        }

        [Test]
        public void SHDis_mov_I_r()
        {
            AssertCode("mov\t#FF,r1", "FFE1");
        }

        [Test]
        public void SHDis_mov_l_predec()
        {
            AssertCode("mov.l\tr8,@-r15", "862F");
        }

        [Test]
        public void SHDis_mov_l_disp_pc()
        {
            AssertCode("mov.l\t@(08,pc),r0", "02D0");
        }

        [Test]
        public void SHDis_mov_l_indexed_r()
        {
            AssertCode("mov.l\t@(r0,r12),r1", "CE 01");
        }

        [Test]
        public void SHDis_mov_l_r_indexed()
        {
            AssertCode("mov.l\t@(8,r6),r2", "62 52");
        }

        [Test]
        public void SHDis_mov_l_indirect_r()
        {
            AssertCode("mov.l\t@r1,r2", "12 62");
        }

        [Test]
        public void SHDis_mov_r_r()
        {
            AssertCode("mov\tr7,r4", "7364");
        }

        [Test]
        public void SHDis_mov_w_indirect_r()
        {
            AssertCode("mov.w\tr0,@(r0,r0)", "0500");
        }

        [Test]
        public void SHDis_mov()
        {
            AssertCode("mov.l\t@r8+,r9", "8669");
        }

        [Test]
        public void SHDis_mov_w_pc()
        {
            AssertCode("mov.w\t@(66,pc),r0", "3390");
        }

        [Test]
        public void SHDis_mova()
        {
            AssertCode("mova\t@(F8,pc),r0", "3E C7");
        }

        [Test]
        public void SHDis_mul_l()
        {
            AssertCode("mul.l\tr1,r5", "1705");
        }

        [Test]
        public void SHDis_nop()
        {
            AssertCode("nop", "0900");
        }

        [Test]
        public void SHDis_neg()
        {
            AssertCode("neg\tr0,r0", "0B60");
        }

        [Test]
        public void SHDis_not()
        {
            AssertCode("not\tr9,r0", "9760");
        }

        [Test]
        public void SHDis_rts()
        {
            AssertCode("rts", "0B00");
        }

        [Test]
        public void SHDis_shll2()
        {
            AssertCode("shll2\tr0", "0840");
        }

        [Test]
        public void SHDis_sts_l_pr_predec()
        {
            AssertCode("sts.l\tpr,@-r15", "224F");
        }

        [Test]
        public void SHDis_tst_imm()
        {
            AssertCode("tst\t#01,r0", "01C8");
        }

        [Test]
        public void SHDis_tst_r_r()
        {
            AssertCode("tst\tr6,r6", "6826");
        }

        [Test]
        public void SHDis_sts_mach()
        {
            AssertCode("sts\tmach,r0", "0A00");
        }

        [Test]
        public void SHDis_sts_shld()
        {
            AssertCode("shld\tr1,r0", "1D40");
        }

        [Test]
        public void SHDis_sts_shad()
        {
            AssertCode("shad\tr1,r0", "1C40");
        }

        [Test]
        public void SHDis_sts_subc()
        {
            AssertCode("subc\tr1,r1", "1A 31");
        }

        [Test]
        public void SHDis_sts_swap_w()
        {
            AssertCode("swap.w\tr4,r0", "4960");
        }

        [Test]
        public void SHDis_sts_shlr_16()
        {
            AssertCode("shlr16\tr4", "2944");
        }

        [Test]
        public void SHDis_sts_shll_16()
        {
            AssertCode("shll16\tr5", "2845");
        }

        [Test]
        public void SHDis_sts_xtrct()
        {
            AssertCode("xtrct\tr4,r0", "4D20");
        }

        [Test]
        public void SHDis_sts_shar()
        {
            AssertCode("shar\tr1", "2141");
        }

        [Test]
        public void SHDis_fcmp_eq()
        {
            AssertCode("fcmp/eq\tfr5,fr9", "﻿54F9");
        }

        [Test]
        public void SHDis_lds_pr()
        {
            AssertCode("lds\tr3,pr", "2A43");
        }

        /////////////////////////////////////////////

        [Test]
        public void ShDis_ocbi()
        {
            AssertCode("ocbi\t@r0", "9300");
        }

        [Test]
        public void ShDis_stc_gbr_r1()
        {
            AssertCode("stc\tgbr,r1", "1201");
        }

        [Test]
        public void ShDis_stc_spc_reg()
        {
            AssertCode("stc\tspc,r4", "4204");
        }

        [Test]
        public void ShDis_stc_mod()
        {
            AssertCode("stc\tmod,r0", "5200");
        }

        [Test]
        public void ShDis_stc_rs()
        {
            AssertCode("stc\trs,r6", "6206");
        }

        [Test]
        public void ShDis_sts_dsr()
        {
            AssertCode("sts\tdsr,r6", "6A06");
        }

        [Test]
        public void ShDis_stc_r0bank()
        {
            AssertCode("stc\tr0_bank,r0", "8200");
        }

        [Test]
        public void ShDis_ocbp()
        {
            AssertCode("ocbp\t@r14", "A30E");
        }

        [Test]
        public void ShDis_std_dbr()
        {
            AssertCode("stc\tdbr,r10", "FA0A");
        }

        [Test]
        public void ShDis_stc_ssr()
        {
            AssertCode("stc\tssr,r1", "3201");
        }

        [Test]
        public void ShDis_sts_pr()
        {
            AssertCode("sts\tpr,r1", "2A01");
        }

        [Test]
        public void ShDis_movco_l()
        {
            AssertCode("movco.l\tr0,@r1", "7301");
        }

        [Test]
        public void ShDis_movca_l()
        {
            AssertCode("movca.l\tr0,@r1", "C301");
        }



        [Test]
        public void ShDis_shal()
        {
            AssertCode("shal\tr2", "2042");
        }

        [Test]
        public void ShDis_ldc_sgr()
        {
            AssertCode("ldc.l\t@r2+,sgr", "3642");
        }

        [Test]
        public void ShDis_tas_b()
        {
            AssertCode("tas.b\t@r3", "1B43");
        }

        [Test]
        public void ShDis_divs()
        {
            AssertCode("divs\tr0,r3", "9443");
        }

        [Test]
        public void ShDis_stc_gbr()
        {
            AssertCode("stc.l\tgbr,@-r4", "1344");
        }

 

        [Test]
        public void ShDis_sts_fpul()
        {
            AssertCode("sts\tfpul,r1", "5A01");
        }

        [Test]
        public void ShDis_lds_fpul()
        {
            AssertCode("lds\tr4,fpul", "5A44");
        }

        [Test]
        public void ShDis_mulr()
        {
            AssertCode("mulr\tr0,r6", "8046");
        }

        [Test]
        public void ShDis_ldc_tbr()
        {
            AssertCode("ldc\tr10,tbr", "4A4A");
        }

        [Test]
        public void ShDis_movmu_l_to_regs()
        {
            AssertCode("movmu.l\t@r15+,r8", "F448");
        }

        [Test]
        public void ShDis_movmu_l()
        {
            AssertCode("movmu.l\tr1,@-r15", "F041");
        }

        [Test]
        public void ShDis_stc_l_bank()
        {
            AssertCode("stc.l\tr5_bank,r13", "D34D");
        }

        [Test]
        public void ShDis_mac_w()
        {
            AssertCode("mac.w\t@r3+,@r10+", "3F4A");
        }

        [Test]
        public void ShDis_ldc_l_bank()
        {
            AssertCode("ldc.l\t@r10+,r2_bank", "A74A");
        }

        [Test]
        public void ShDis_lds_l_dsr_post()
        {
            AssertCode("lds.l\t@r12+,dsr", "664C");
        }

        [Test]
        public void ShDis_lds_l_dsr_reg()
        {
            AssertCode("lds\tr12,dsr", "6A4C");
        }

        [Test]
        public void ShDis_fsub()
        {
            AssertCode("fsub\tfr0,fr1", "01F1");
        }

        [Test]
        public void ShDis_fmov_d()
        {
            AssertCode("fmov\tdr2,dr0", "2CF0");
        }

        [Test]
        public void ShDis_fmov_d_idx()
        {
            AssertCode("fmov.d\t@(r0,r1),dr4", "46F1");
        }

        [Test]
        public void ShDis_fadd_dr()
        {
            AssertCode("fadd\tdr0,dr0", "10F0");
        }

        [Test]
        public void ShDis_fmov_d_ind()
        {
            AssertCode("fmov.d\t@r0,dr4", "48F0");
        }

        [Test]
        public void ShDis_pref()
        {
            AssertCode("pref\t@r10", "830A");
        }

        [Test]
        public void ShDis_fmul()
        {
            AssertCode("fmul\tfr11,fr0", "B2F0");
        }

        [Test]
        public void ShDis_mac_l()
        {
            AssertCode("mac.l\t@r15+,@r0+", "FF00");
        }

        // A SuperH decoder for the instruction FFFE has not been implemented yet.
        [Test]
        public void ShDis_fmac()
        {
            AssertCode("fmac\tfr0,fr15,fr15", "FEFF");
        }


        // A SuperH decoder for the instruction 400E has not been implemented yet.
        [Test]
        public void ShDis_ldc_sr()
        {
            AssertCode("ldc\tr0,sr", "0E40");
        }

        // A SuperH decoder for the instruction 0138 has not been implemented yet.
        [Test]
        public void ShDis_ldtlb()
        {
            AssertCode("ldtlb", "3800");
        }

        [Test]
        public void ShDis_ldc_vbr()
        {
            AssertCode("ldc\tr1,vbr", "2E41");
        }

        [Test]
        public void ShDis_lds_l_mach()
        {
            AssertCode("lds.l\t@r0+,mach", "0640");
        }

        [Test]
        public void ShDis_ftrc()
        {
            AssertCode("ftrc\tdr2,fpul", "3DF2");
        }

#if NYI
        //$TODO: DSP instructions
        [Test]
        public void ShDis_008A()
        {
        AssertCode("@@@", "8A00");
        }
        [Test]
        public void ShDis_009A()
        {
            AssertCode("@@@", "9A00");
        }
         A SuperH decoder for the instruction 01BA has not been implemented yet.
        [Test]
        public void ShDis_01BA()
        {
            AssertCode("@@@", "BA01");
        }
              // A SuperH decoder for the instruction 4A14 has not been implemented yet.
        [Test]
        public void ShDis_4A14()
        {
            AssertCode("@@@", "144A");
        }
#endif
    }
}

