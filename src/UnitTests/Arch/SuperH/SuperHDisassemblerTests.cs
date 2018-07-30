#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
        public void SHDis_flds()
        {
            AssertCode("flds\tfr8,fpul", "1DF8");
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
        
        // A SuperH decoder for the instruction 0093 has not been implemented yet.
        [Test]
        public void ShDis_ocbi()
        {
            AssertCode("ocbi\t@r0", "9300");
        }

        [Test]
        public void ShDis_stc_gbr_r1()
        {
            AssertCode("stc\tgbr,r1", "1200");
        }

        // A SuperH decoder for the instruction FB0A has not been implemented yet.
        [Test]
        public void ShDis_FB0A()
        {
            AssertCode("@@@", "0AFB");
        }

        // A SuperH decoder for the instruction FFFE has not been implemented yet.
        [Test]
        public void ShDis_fmac()
        {
            AssertCode("fmac\tfr0,fr15,fr15", "FEFF");
        }

        // A SuperH decoder for the instruction F91A has not been implemented yet.
        [Test]
        public void ShDis_F91A()
        {
            AssertCode("@@@", "1AF9");
        }

        // A SuperH decoder for the instruction F7AA has not been implemented yet.
        [Test]
        public void ShDis_F7AA()
        {
            AssertCode("@@@", "AAF7");
        }

        // A SuperH decoder for the instruction FA48 has not been implemented yet.
        [Test]
        public void ShDis_FA48()
        {
            AssertCode("@@@", "48FA");
        }

        // A SuperH decoder for the instruction F9F0 has not been implemented yet.
        [Test]
        public void ShDis_F9F0()
        {
            AssertCode("@@@", "F0F9");
        }

        // A SuperH decoder for the instruction F5AA has not been implemented yet.
        [Test]
        public void ShDis_F5AA()
        {
            AssertCode("@@@", "AAF5");
        }

        // A SuperH decoder for the instruction FE0C has not been implemented yet.
        [Test]
        public void ShDis_FE0C()
        {
            AssertCode("@@@", "0CFE");
        }

        // A SuperH decoder for the instruction F92E has not been implemented yet.
        [Test]
        public void ShDis_F92E()
        {
            AssertCode("@@@", "2EF9");
        }

        // A SuperH decoder for the instruction F4CC has not been implemented yet.
        [Test]
        public void ShDis_F4CC()
        {
            AssertCode("@@@", "CCF4");
        }

        // A SuperH decoder for the instruction F70C has not been implemented yet.
        [Test]
        public void ShDis_F70C()
        {
            AssertCode("@@@", "0CF7");
        }

        // A SuperH decoder for the instruction F6FC has not been implemented yet.
        [Test]
        public void ShDis_F6FC()
        {
            AssertCode("@@@", "FCF6");
        }

        // A SuperH decoder for the instruction F870 has not been implemented yet.
        [Test]
        public void ShDis_F870()
        {
            AssertCode("@@@", "70F8");
        }

        // A SuperH decoder for the instruction F6B6 has not been implemented yet.
        [Test]
        public void ShDis_F6B6()
        {
            AssertCode("@@@", "B6F6");
        }

        // A SuperH decoder for the instruction FA22 has not been implemented yet.
        [Test]
        public void ShDis_FA22()
        {
            AssertCode("@@@", "22FA");
        }

        // A SuperH decoder for the instruction F59A has not been implemented yet.
        [Test]
        public void ShDis_F59A()
        {
            AssertCode("@@@", "9AF5");
        }

        // A SuperH decoder for the instruction F3CA has not been implemented yet.
        [Test]
        public void ShDis_F3CA()
        {
            AssertCode("@@@", "CAF3");
        }

        // A SuperH decoder for the instruction F62E has not been implemented yet.
        [Test]
        public void ShDis_F62E()
        {
            AssertCode("@@@", "2EF6");
        }

        // A SuperH decoder for the instruction F368 has not been implemented yet.
        [Test]
        public void ShDis_F368()
        {
            AssertCode("@@@", "68F3");
        }

        // A SuperH decoder for the instruction F050 has not been implemented yet.
        [Test]
        public void ShDis_F050()
        {
            AssertCode("@@@", "50F0");
        }

        // A SuperH decoder for the instruction F222 has not been implemented yet.
        [Test]
        public void ShDis_F222()
        {
            AssertCode("@@@", "22F2");
        }

        // A SuperH decoder for the instruction F618 has not been implemented yet.
        [Test]
        public void ShDis_F618()
        {
            AssertCode("@@@", "18F6");
        }

        // A SuperH decoder for the instruction FB7C has not been implemented yet.
        [Test]
        public void ShDis_FB7C()
        {
            AssertCode("@@@", "7CFB");
        }

        // A SuperH decoder for the instruction F51C has not been implemented yet.
        [Test]
        public void ShDis_F51C()
        {
            AssertCode("@@@", "1CF5");
        }

        // A SuperH decoder for the instruction F04C has not been implemented yet.
        [Test]
        public void ShDis_F04C()
        {
            AssertCode("@@@", "4CF0");
        }

        // A SuperH decoder for the instruction F778 has not been implemented yet.
        [Test]
        public void ShDis_F778()
        {
            AssertCode("@@@", "78F7");
        }

        // A SuperH decoder for the instruction F692 has not been implemented yet.
        [Test]
        public void ShDis_F692()
        {
            AssertCode("@@@", "92F6");
        }

        // A SuperH decoder for the instruction F19C has not been implemented yet.
        [Test]
        public void ShDis_F19C()
        {
            AssertCode("@@@", "9CF1");
        }

        // A SuperH decoder for the instruction F17C has not been implemented yet.
        [Test]
        public void ShDis_F17C()
        {
            AssertCode("@@@", "7CF1");
        }

        // A SuperH decoder for the instruction F15A has not been implemented yet.
        [Test]
        public void ShDis_F15A()
        {
            AssertCode("@@@", "5AF1");
        }

        // A SuperH decoder for the instruction F02C has not been implemented yet.
        [Test]
        public void ShDis_F02C()
        {
            AssertCode("@@@", "2CF0");
        }

        // A SuperH decoder for the instruction F0DE has not been implemented yet.
        [Test]
        public void ShDis_F0DE()
        {
            AssertCode("@@@", "DEF0");
        }

        // A SuperH decoder for the instruction F16A has not been implemented yet.
        [Test]
        public void ShDis_F16A()
        {
            AssertCode("@@@", "6AF1");
        }

        // A SuperH decoder for the instruction F422 has not been implemented yet.
        [Test]
        public void ShDis_F422()
        {
            AssertCode("@@@", "22F4");
        }

        // A SuperH decoder for the instruction F3E8 has not been implemented yet.
        [Test]
        public void ShDis_F3E8()
        {
            AssertCode("@@@", "E8F3");
        }

        // A SuperH decoder for the instruction 400E has not been implemented yet.
        [Test]
        public void ShDis_ldc_sr()
        {
            AssertCode("ldc\tr0,sr", "0E40");
        }

        // A SuperH decoder for the instruction F62A has not been implemented yet.
        [Test]
        public void ShDis_F62A()
        {
            AssertCode("@@@", "2AF6");
        }

        // A SuperH decoder for the instruction F0AC has not been implemented yet.
        [Test]
        public void ShDis_F0AC()
        {
            AssertCode("@@@", "ACF0");
        }

        // A SuperH decoder for the instruction F288 has not been implemented yet.
        [Test]
        public void ShDis_F288()
        {
            AssertCode("@@@", "88F2");
        }

        // A SuperH decoder for the instruction F4DC has not been implemented yet.
        [Test]
        public void ShDis_F4DC()
        {
            AssertCode("@@@", "DCF4");
        }

        // A SuperH decoder for the instruction F130 has not been implemented yet.
        [Test]
        public void ShDis_F130()
        {
            AssertCode("@@@", "30F1");
        }

        // A SuperH decoder for the instruction 0138 has not been implemented yet.
        [Test]
        public void ShDis_ldtlb()
        {
            AssertCode("ldtlb", "3800");
        }

        // A SuperH decoder for the instruction F31A has not been implemented yet.
        [Test]
        public void ShDis_F31A()
        {
            AssertCode("@@@", "1AF3");
        }

        // A SuperH decoder for the instruction F168 has not been implemented yet.
        [Test]
        public void ShDis_F168()
        {
            AssertCode("@@@", "68F1");
        }

        // A SuperH decoder for the instruction F188 has not been implemented yet.
        [Test]
        public void ShDis_F188()
        {
            AssertCode("@@@", "88F1");
        }

        // A SuperH decoder for the instruction F348 has not been implemented yet.
        [Test]
        public void ShDis_F348()
        {
            AssertCode("@@@", "48F3");
        }

        // A SuperH decoder for the instruction F198 has not been implemented yet.
        [Test]
        public void ShDis_F198()
        {
            AssertCode("@@@", "98F1");
        }

        // A SuperH decoder for the instruction F2EE has not been implemented yet.
        [Test]
        public void ShDis_F2EE()
        {
            AssertCode("@@@", "EEF2");
        }

        // A SuperH decoder for the instruction F2B0 has not been implemented yet.
        [Test]
        public void ShDis_F2B0()
        {
            AssertCode("@@@", "B0F2");
        }

        // A SuperH decoder for the instruction F210 has not been implemented yet.
        [Test]
        public void ShDis_F210()
        {
            AssertCode("@@@", "10F2");
        }

        // A SuperH decoder for the instruction F1B8 has not been implemented yet.
        [Test]
        public void ShDis_F1B8()
        {
            AssertCode("@@@", "B8F1");
        }

        // A SuperH decoder for the instruction F21C has not been implemented yet.
        [Test]
        public void ShDis_F21C()
        {
            AssertCode("@@@", "1CF2");
        }

        // A SuperH decoder for the instruction F1F8 has not been implemented yet.
        [Test]
        public void ShDis_F1F8()
        {
            AssertCode("@@@", "F8F1");
        }

        // A SuperH decoder for the instruction F230 has not been implemented yet.
        [Test]
        public void ShDis_F230()
        {
            AssertCode("@@@", "30F2");
        }

        // A SuperH decoder for the instruction F0D0 has not been implemented yet.
        [Test]
        public void ShDis_F0D0()
        {
            AssertCode("@@@", "D0F0");
        }

        // A SuperH decoder for the instruction F0CE has not been implemented yet.
        [Test]
        public void ShDis_F0CE()
        {
            AssertCode("@@@", "CEF0");
        }

        // A SuperH decoder for the instruction F25C has not been implemented yet.
        [Test]
        public void ShDis_F25C()
        {
            AssertCode("@@@", "5CF2");
        }

        // A SuperH decoder for the instruction F1A8 has not been implemented yet.
        [Test]
        public void ShDis_F1A8()
        {
            AssertCode("@@@", "A8F1");
        }

        // A SuperH decoder for the instruction F0B2 has not been implemented yet.
        [Test]
        public void ShDis_F0B2()
        {
            AssertCode("@@@", "B2F0");
        }

        // A SuperH decoder for the instruction F290 has not been implemented yet.
        [Test]
        public void ShDis_F290()
        {
            AssertCode("@@@", "90F2");
        }

        // A SuperH decoder for the instruction F046 has not been implemented yet.
        [Test]
        public void ShDis_F046()
        {
            AssertCode("@@@", "46F0");
        }

        // A SuperH decoder for the instruction F2F8 has not been implemented yet.
        [Test]
        public void ShDis_F2F8()
        {
            AssertCode("@@@", "F8F2");
        }

        // A SuperH decoder for the instruction F14C has not been implemented yet.
        [Test]
        public void ShDis_F14C()
        {
            AssertCode("@@@", "4CF1");
        }

        // A SuperH decoder for the instruction F32C has not been implemented yet.
        [Test]
        public void ShDis_F32C()
        {
            AssertCode("@@@", "2CF3");
        }

        // A SuperH decoder for the instruction F338 has not been implemented yet.
        [Test]
        public void ShDis_F338()
        {
            AssertCode("@@@", "38F3");
        }

        // A SuperH decoder for the instruction F2BC has not been implemented yet.
        [Test]
        public void ShDis_F2BC()
        {
            AssertCode("@@@", "BCF2");
        }

        // A SuperH decoder for the instruction F3B0 has not been implemented yet.
        [Test]
        public void ShDis_F3B0()
        {
            AssertCode("@@@", "B0F3");
        }

        // A SuperH decoder for the instruction F3B8 has not been implemented yet.
        [Test]
        public void ShDis_F3B8()
        {
            AssertCode("@@@", "B8F3");
        }

        // A SuperH decoder for the instruction F410 has not been implemented yet.
        [Test]
        public void ShDis_F410()
        {
            AssertCode("@@@", "10F4");
        }

        // A SuperH decoder for the instruction F358 has not been implemented yet.
        [Test]
        public void ShDis_F358()
        {
            AssertCode("@@@", "58F3");
        }

        // A SuperH decoder for the instruction F4F0 has not been implemented yet.
        [Test]
        public void ShDis_F4F0()
        {
            AssertCode("@@@", "F0F4");
        }

        // A SuperH decoder for the instruction 4F52 has not been implemented yet.
        [Test]
        public void ShDis_4F52()
        {
            AssertCode("@@@", "524F");
        }

        // A SuperH decoder for the instruction FC3C has not been implemented yet.
        [Test]
        public void ShDis_FC3C()
        {
            AssertCode("@@@", "3CFC");
        }

        // A SuperH decoder for the instruction FC18 has not been implemented yet.
        [Test]
        public void ShDis_FC18()
        {
            AssertCode("@@@", "18FC");
        }

        // A SuperH decoder for the instruction F4A8 has not been implemented yet.
        [Test]
        public void ShDis_F4A8()
        {
            AssertCode("@@@", "A8F4");
        }

        // A SuperH decoder for the instruction 4341 has not been implemented yet.
        [Test]
        public void ShDis_4341()
        {
            AssertCode("@@@", "4143");
        }

        // A SuperH decoder for the instruction 4441 has not been implemented yet.
        [Test]
        public void ShDis_4441()
        {
            AssertCode("@@@", "4144");
        }

        // A SuperH decoder for the instruction 4152 has not been implemented yet.
        [Test]
        public void ShDis_4152()
        {
            AssertCode("@@@", "5241");
        }

        [Test]
        public void ShDis_stc_spc()
        {
            AssertCode("stc.l\tspc,r1", "4341");
        }

        // A SuperH decoder for the instruction 4D44 has not been implemented yet.
        [Test]
        public void ShDis_4D44()
        {
            AssertCode("@@@", "444D");
        }

        // A SuperH decoder for the instruction FBE8 has not been implemented yet.
        [Test]
        public void ShDis_FBE8()
        {
            AssertCode("@@@", "E8FB");
        }

        // A SuperH decoder for the instruction F570 has not been implemented yet.
        [Test]
        public void ShDis_F570()
        {
            AssertCode("@@@", "70F5");
        }

        // A SuperH decoder for the instruction F588 has not been implemented yet.
        [Test]
        public void ShDis_F588()
        {
            AssertCode("@@@", "88F5");
        }

        // A SuperH decoder for the instruction F54C has not been implemented yet.
        [Test]
        public void ShDis_F54C()
        {
            AssertCode("@@@", "4CF5");
        }

        [Test]
        public void ShDis_mac_l()
        {
            AssertCode("mac.l\t@r15+,@r0+", "FF00");
        }

        // A SuperH decoder for the instruction F5D0 has not been implemented yet.
        [Test]
        public void ShDis_F5D0()
        {
            AssertCode("@@@", "D0F5");
        }

        // A SuperH decoder for the instruction F5FC has not been implemented yet.
        [Test]
        public void ShDis_F5FC()
        {
            AssertCode("@@@", "FCF5");
        }

        // A SuperH decoder for the instruction F93C has not been implemented yet.
        [Test]
        public void ShDis_F93C()
        {
            AssertCode("@@@", "3CF9");
        }

        // A SuperH decoder for the instruction F610 has not been implemented yet.
        [Test]
        public void ShDis_F610()
        {
            AssertCode("@@@", "10F6");
        }

        // A SuperH decoder for the instruction F61C has not been implemented yet.
        [Test]
        public void ShDis_F61C()
        {
            AssertCode("@@@", "1CF6");
        }

        // A SuperH decoder for the instruction FA08 has not been implemented yet.
        [Test]
        public void ShDis_FA08()
        {
            AssertCode("@@@", "08FA");
        }

        // A SuperH decoder for the instruction F63C has not been implemented yet.
        [Test]
        public void ShDis_F63C()
        {
            AssertCode("@@@", "3CF6");
        }

        // A SuperH decoder for the instruction F648 has not been implemented yet.
        [Test]
        public void ShDis_F648()
        {
            AssertCode("@@@", "48F6");
        }

        // A SuperH decoder for the instruction F6D8 has not been implemented yet.
        [Test]
        public void ShDis_F6D8()
        {
            AssertCode("@@@", "D8F6");
        }

        // A SuperH decoder for the instruction F828 has not been implemented yet.
        [Test]
        public void ShDis_F828()
        {
            AssertCode("@@@", "28F8");
        }

        // A SuperH decoder for the instruction F698 has not been implemented yet.
        [Test]
        public void ShDis_F698()
        {
            AssertCode("@@@", "98F6");
        }

        // A SuperH decoder for the instruction F88C has not been implemented yet.
        [Test]
        public void ShDis_F88C()
        {
            AssertCode("@@@", "8CF8");
        }

        // A SuperH decoder for the instruction 4920 has not been implemented yet.
        [Test]
        public void ShDis_4920()
        {
            AssertCode("@@@", "2049");
        }

        // A SuperH decoder for the instruction FA68 has not been implemented yet.
        [Test]
        public void ShDis_FA68()
        {
            AssertCode("@@@", "68FA");
        }

        // A SuperH decoder for the instruction F830 has not been implemented yet.
        [Test]
        public void ShDis_F830()
        {
            AssertCode("@@@", "30F8");
        }

        // A SuperH decoder for the instruction F670 has not been implemented yet.
        [Test]
        public void ShDis_F670()
        {
            AssertCode("@@@", "70F6");
        }

        // A SuperH decoder for the instruction F68C has not been implemented yet.
        [Test]
        public void ShDis_F68C()
        {
            AssertCode("@@@", "8CF6");
        }

        // A SuperH decoder for the instruction F630 has not been implemented yet.
        [Test]
        public void ShDis_F630()
        {
            AssertCode("@@@", "30F6");
        }

        // A SuperH decoder for the instruction F638 has not been implemented yet.
        [Test]
        public void ShDis_F638()
        {
            AssertCode("@@@", "38F6");
        }

        // A SuperH decoder for the instruction F001 has not been implemented yet.
        [Test]
        public void ShDis_F001()
        {
            AssertCode("@@@", "01F0");
        }

        // A SuperH decoder for the instruction F6A8 has not been implemented yet.
        [Test]
        public void ShDis_F6A8()
        {
            AssertCode("@@@", "A8F6");
        }

        // A SuperH decoder for the instruction 4E20 has not been implemented yet.
        [Test]
        public void ShDis_4E20()
        {
            AssertCode("@@@", "204E");
        }

        // A SuperH decoder for the instruction F6BC has not been implemented yet.
        [Test]
        public void ShDis_F6BC()
        {
            AssertCode("@@@", "BCF6");
        }

        // A SuperH decoder for the instruction F6CC has not been implemented yet.
        [Test]
        public void ShDis_F6CC()
        {
            AssertCode("@@@", "CCF6");
        }

        // A SuperH decoder for the instruction F6F0 has not been implemented yet.
        [Test]
        public void ShDis_F6F0()
        {
            AssertCode("@@@", "F0F6");
        }

        // A SuperH decoder for the instruction 4320 has not been implemented yet.
        [Test]
        public void ShDis_4320()
        {
            AssertCode("@@@", "2043");
        }

        // A SuperH decoder for the instruction FA5C has not been implemented yet.
        [Test]
        public void ShDis_FA5C()
        {
            AssertCode("@@@", "5CFA");
        }

        // A SuperH decoder for the instruction F7E8 has not been implemented yet.
        [Test]
        public void ShDis_F7E8()
        {
            AssertCode("@@@", "E8F7");
        }

        // A SuperH decoder for the instruction F850 has not been implemented yet.
        [Test]
        public void ShDis_F850()
        {
            AssertCode("@@@", "50F8");
        }

        // A SuperH decoder for the instruction F86C has not been implemented yet.
        [Test]
        public void ShDis_F86C()
        {
            AssertCode("@@@", "6CF8");
        }

        // A SuperH decoder for the instruction F87C has not been implemented yet.
        [Test]
        public void ShDis_F87C()
        {
            AssertCode("@@@", "7CF8");
        }

        // A SuperH decoder for the instruction FA3C has not been implemented yet.
        [Test]
        public void ShDis_FA3C()
        {
            AssertCode("@@@", "3CFA");
        }

        // A SuperH decoder for the instruction F9CC has not been implemented yet.
        [Test]
        public void ShDis_F9CC()
        {
            AssertCode("@@@", "CCF9");
        }

        // A SuperH decoder for the instruction F96C has not been implemented yet.
        [Test]
        public void ShDis_F96C()
        {
            AssertCode("@@@", "6CF9");
        }

        // A SuperH decoder for the instruction F8B0 has not been implemented yet.
        [Test]
        public void ShDis_F8B0()
        {
            AssertCode("@@@", "B0F8");
        }

        // A SuperH decoder for the instruction F890 has not been implemented yet.
        [Test]
        public void ShDis_F890()
        {
            AssertCode("@@@", "90F8");
        }

        // A SuperH decoder for the instruction F5EC has not been implemented yet.
        [Test]
        public void ShDis_F5EC()
        {
            AssertCode("@@@", "ECF5");
        }

        // A SuperH decoder for the instruction F450 has not been implemented yet.
        [Test]
        public void ShDis_F450()
        {
            AssertCode("@@@", "50F4");
        }

        // A SuperH decoder for the instruction F730 has not been implemented yet.
        [Test]
        public void ShDis_F730()
        {
            AssertCode("@@@", "30F7");
        }

        // A SuperH decoder for the instruction F768 has not been implemented yet.
        [Test]
        public void ShDis_F768()
        {
            AssertCode("@@@", "68F7");
        }

        // A SuperH decoder for the instruction F79C has not been implemented yet.
        [Test]
        public void ShDis_F79C()
        {
            AssertCode("@@@", "9CF7");
        }

        // A SuperH decoder for the instruction F7BC has not been implemented yet.
        [Test]
        public void ShDis_F7BC()
        {
            AssertCode("@@@", "BCF7");
        }

        // A SuperH decoder for the instruction F7FC has not been implemented yet.
        [Test]
        public void ShDis_F7FC()
        {
            AssertCode("@@@", "FCF7");
        }

        // A SuperH decoder for the instruction FCF8 has not been implemented yet.
        [Test]
        public void ShDis_FCF8()
        {
            AssertCode("@@@", "F8FC");
        }

        // A SuperH decoder for the instruction FD08 has not been implemented yet.
        [Test]
        public void ShDis_FD08()
        {
            AssertCode("@@@", "08FD");
        }

        // A SuperH decoder for the instruction FAF2 has not been implemented yet.
        [Test]
        public void ShDis_FAF2()
        {
            AssertCode("@@@", "F2FA");
        }

        // A SuperH decoder for the instruction FD48 has not been implemented yet.
        [Test]
        public void ShDis_FD48()
        {
            AssertCode("@@@", "48FD");
        }

        // A SuperH decoder for the instruction F71A has not been implemented yet.
        [Test]
        public void ShDis_F71A()
        {
            AssertCode("@@@", "1AF7");
        }

        // A SuperH decoder for the instruction F866 has not been implemented yet.
        [Test]
        public void ShDis_F866()
        {
            AssertCode("@@@", "66F8");
        }

        // A SuperH decoder for the instruction FD9C has not been implemented yet.
        [Test]
        public void ShDis_FD9C()
        {
            AssertCode("@@@", "9CFD");
        }

        // A SuperH decoder for the instruction FD7C has not been implemented yet.
        [Test]
        public void ShDis_FD7C()
        {
            AssertCode("@@@", "7CFD");
        }

        // A SuperH decoder for the instruction FD8C has not been implemented yet.
        [Test]
        public void ShDis_FD8C()
        {
            AssertCode("@@@", "8CFD");
        }

        // A SuperH decoder for the instruction FDAC has not been implemented yet.
        [Test]
        public void ShDis_FDAC()
        {
            AssertCode("@@@", "ACFD");
        }

        // A SuperH decoder for the instruction 445A has not been implemented yet.
        [Test]
        public void ShDis_445A()
        {
            AssertCode("@@@", "5A44");
        }

        // A SuperH decoder for the instruction F42D has not been implemented yet.
        [Test]
        public void ShDis_F42D()
        {
            AssertCode("@@@", "2DF4");
        }

        // A SuperH decoder for the instruction F242 has not been implemented yet.
        [Test]
        public void ShDis_F242()
        {
            AssertCode("@@@", "42F2");
        }

        // A SuperH decoder for the instruction F23D has not been implemented yet.
        [Test]
        public void ShDis_F23D()
        {
            AssertCode("@@@", "3DF2");
        }

        [Test]
        public void ShDis_sts_fpul()
        {
            AssertCode("sts\tfpul,r1", "5A01");
        }

        // A SuperH decoder for the instruction 4DD3 has not been implemented yet.
        [Test]
        public void ShDis_4DD3()
        {
            AssertCode("@@@", "D34D");
        }

        [Test]
        public void ShDis_ldc_vbr()
        {
            AssertCode("ldc\tr1,vbr", "2E41");
        }

        // A SuperH decoder for the instruction 41F0 has not been implemented yet.
        [Test]
        public void ShDis_41F0()
        {
            AssertCode("@@@", "F041");
        }

        [Test]
        public void ShDis_lds_l_mach()
        {
            AssertCode("lds.l\t@r0+,mach", "0640");
        }

        // A SuperH decoder for the instruction F248 has not been implemented yet.
        [Test]
        public void ShDis_F248()
        {
            AssertCode("@@@", "48F2");
        }

        // A SuperH decoder for the instruction FE18 has not been implemented yet.
        [Test]
        public void ShDis_FE18()
        {
            AssertCode("@@@", "18FE");
        }

        // A SuperH decoder for the instruction FCA8 has not been implemented yet.
        [Test]
        public void ShDis_FCA8()
        {
            AssertCode("@@@", "A8FC");
        }

        // A SuperH decoder for the instruction FE90 has not been implemented yet.
        [Test]
        public void ShDis_FE90()
        {
            AssertCode("@@@", "90FE");
        }

        // A SuperH decoder for the instruction FE3C has not been implemented yet.
        [Test]
        public void ShDis_FE3C()
        {
            AssertCode("@@@", "3CFE");
        }

        // A SuperH decoder for the instruction FE50 has not been implemented yet.
        [Test]
        public void ShDis_FE50()
        {
            AssertCode("@@@", "50FE");
        }
    }
}

