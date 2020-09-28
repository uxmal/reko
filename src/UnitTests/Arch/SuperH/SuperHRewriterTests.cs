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
using Reko.Core.Configuration;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Tlcs
{
    [TestFixture]
    public class SuperHRewriterTests : RewriterTestBase
    {
        private readonly SuperHArchitecture arch = new SuperHLeArchitecture("superH");
        private Address baseAddr = Address.Ptr32(0x00100000);

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => baseAddr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var state = (SuperHState)arch.CreateProcessorState();
            return new SuperHRewriter(arch, new LeImageReader(mem, 0), state, binder, host);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SHRw_add_imm_rn()
        {
            Given_HexString("FF73"); // add\t#FF,r3
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = r3 + 0xFFFFFFFF");
        }

        [Test]
        public void SHRw_add_rm_rn()
        {
            Given_HexString("4C32"); // add\tr4,r2
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r2 = r2 + r4");
        }

        [Test]
        public void SHRw_addc_rm_rn()
        {
            Given_HexString("4E32"); // addc\tr4,r2
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r2 = r2 + r4 + T");
        }

        [Test]
        public void SHRw_addv_rm_rn()
        {
            Given_HexString("4F32"); // addv\tr4,r2
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r2 = r2 + r4",
                "2|L--|T = Test(OV,r2)");
        }

        [Test]
        public void SHRw_and_rm_rn()
        {
            Given_HexString("4923"); // and\tr4,r3
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = r3 & r4");

        }

        [Test]
        public void SHRw_and_imm_r0()
        {
            Given_HexString("F0C9"); // and\t#F0,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0 & 0x000000F0");

        }

        [Test]
        public void SHRw_and_b_imm_r0()
        {
            Given_HexString("F0CD"); // and.b\t#F0,@(r0,gbr)
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v2 = Mem0[r0 + gbr:byte]",
                "2|L--|Mem0[r0 + gbr:byte] = v2 & 0x000000F0");
        }

        [Test]
        public void SHRw_bf()
        {
            Given_HexString("F08B"); // bf\t000FFFE4
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (!T) branch 000FFFE4");
        }

        [Test]
        public void SHRw_bf_s()
        {
            Given_HexString("F08F"); // bf/s\t000FFFE4
            AssertCode(
               "0|TD-|00100000(2): 1 instructions",
               "1|TD-|if (!T) branch 000FFFE4");
        }

        [Test]
        public void SHRw_bra()
        {
            Given_HexString("F0AF"); // bra\t0000FFE4
            AssertCode(
                "0|TD-|00100000(2): 1 instructions",
                "1|TD-|goto 000FFFE4");
        }

        [Test]
        public void SHRw_braf_reg()
        {
            Given_HexString("2301"); // braf\tr1
            AssertCode(
                "0|TD-|00100000(2): 1 instructions",
                "1|TD-|goto 0x00100004 + r1");
        }

        [Test]
        public void SHRw_brk()
        {
            Given_HexString("3B00"); // brk
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__brk()");
        }

        [Test]
        public void SHRw_bsr()
        {
            Given_HexString("F0BF"); // bsr\t0000FFE4
            AssertCode(
                "0|TD-|00100000(2): 1 instructions",
                "1|TD-|call 000FFFE4 (0)");
        }

        [Test]
        public void SHRw_bsrf()
        {
            Given_HexString("0301"); // bsrf\tr1
            AssertCode(
                "0|TD-|00100000(2): 1 instructions",
                "1|TD-|call 0x00100004 + r1 (0)");
        }

        [Test]
        public void SHRw_bt()
        {
            Given_HexString("F089"); // bt\t0000FFE4
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (T) branch 000FFFE4");
        }

        [Test]
        public void SHRw_bt_s()
        {
            Given_HexString("F08D"); // bt/s\t0000FFE4
            AssertCode(
                "0|TD-|00100000(2): 1 instructions",
                "1|TD-|if (T) branch 000FFFE4");
        }

        [Test]
        public void SHRw_clrmac()
        {
            Given_HexString("2800"); // clrmac
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|mac = 0x0000000000000000");
        }

        [Test]
        public void SHRw_cmpeq()
        {
            Given_HexString("4035"); // cmp/eq\tr4,r5
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = r5 == r4");
        }

        [Test]
        public void SHRw_fcmp_eq()
        {
            Given_HexString("54F9");    // cmp/eq
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = fr9 == fr5");
        }

        [Test]
        public void SHRw_cmpeq_imm()
        {
            Given_HexString("F088"); // cmp/eq\t#F0,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = r0 == 0xFFFFFFF0");
        }

        [Test]
        public void SHRw_div0s()
        {
            Given_HexString("4723"); // div0s\tr4,r3
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = __div0s(r3, r4)");
        }

        [Test]
        public void SHRw_div0u()
        {
            Given_HexString("1900"); // div0u
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = __div0u()");
        }

        [Test]
        public void SHRw_div1()
        {
            Given_HexString("4433"); // div1\tr4,r3
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = __div1(r3, r4)");
        }

        [Test]
        public void SHRw_dmuls_l()
        {
            Given_HexString("4D33"); // dmuls.l\tr4,r3
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|mac = r3 *s r4");
        }

        [Test]
        public void SHRw_dt()
        {
            Given_HexString("104F"); // dt\tr15
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r15 = r15 - 0x00000001",
                "2|L--|T = r15 == 0x00000000");
        }

        [Test]
        public void SHRw_exts_b()
        {
            Given_HexString("FE6E"); // exts.b\tr15,r14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = (int8) r15");
        }

        [Test]
        public void SHRw_exts_w()
        {
            Given_HexString("FF6E"); // exts.w\tr15,r14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = (int16) r15");
        }

        [Test]
        public void SHRw_extu_b()
        {
            Given_HexString("FC6E"); // extu.b\tr15,r14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = (byte) r15");
        }

        [Test]
        public void SHRw_extu_w()
        {
            Given_HexString("FD6E"); // extu.w\tr15,r14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = (uint16) r15");

        }

        [Test]
        public void SHRw_fabs_dr()
        {
            Given_HexString("5DFE"); // fabs\tdr14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|dr14 = fabs(dr14)");
        }

        [Test]
        public void SHRw_fabs_fr()
        {
            Given_HexString("5DFF"); // fabs\tfr15
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|fr15 = fabs(fr15)");
        }

        [Test]
        public void SHRw_fadd_dr()
        {
            Given_HexString("C0FE"); // fadd\tdr12,dr14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|dr14 = dr14 + dr12");
        }

        [Test]
        public void SHRw_fadd_fr()
        {
            Given_HexString("C0FF"); // fadd\tfr12,fr15
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|fr15 = fr15 + fr12");
        }

        [Test]
        public void SHRw_fcmp_eq_dr()
        {
            Given_HexString("C4FE"); // fcmp/eq\tdr12,dr14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = dr14 == dr12");
        }

        [Test]
        public void SHRw_fcmp_eq_fr()
        {
            Given_HexString("C4FF"); // fcmp/eq\tfr12,fr15
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = fr15 == fr12");
        }

        [Test]
        public void SHRw_fcmp_gt_dr()
        {
            Given_HexString("C5FE"); // fcmp/gt\tdr12,dr14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = dr14 > dr12");
        }

        [Test]
        public void SHRw_fcmp_gt_fr()
        {
            Given_HexString("C5FF"); // fcmp/gt\tfr12,fr15
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = fr15 > fr12");
        }

        [Test]
        public void SHRw_fcnvds()
        {
            Given_HexString("BDFE"); // fcnvds\tdr14,fpul
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|fpul = (real32) dr14");
        }

        [Test]
        public void SHRw_fcnvsd()
        {
            Given_HexString("ADFE"); // fcnvsd\tfpul,dr14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|dr14 = (real64) fpul");
        }

        [Test]
        public void SHRw_fdiv_dr()
        {
            Given_HexString("C3FE"); // fdiv\tdr12,dr14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|dr14 = dr14 / dr12");
        }

        [Test]
        public void SHRw_fdiv_fr()
        {
            Given_HexString("C3FF"); // fdiv\tfr12,fr15
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|fr15 = fr15 / fr12");
        }

        [Test]
        [Ignore("Wait until encountering this in real code")]
        public void SHRw_fipr()
        {
            Given_HexString("EDFE"); // fipr\tfv8,fv12
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void SHRw_flds()
        {
            Given_HexString("1DF8"); // flds\tfr8,fpul
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|fpul = dr8");
        }

        [Test]
        public void SHRw_fldi0()
        {
            Given_HexString("8DF8"); // fldi0\tfr8
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|fr8 = 0.0F");
        }

        [Test]
        public void SHRw_fldi1()
        {
            Given_HexString("9DF8"); // fldi1\tfr8
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|fr8 = 1.0F");
        }

        [Test]
        public void SHRw_jmp_r()
        {
            Given_HexString("2B40"); // jmp\t@r0
            AssertCode(
                "0|TD-|00100000(2): 1 instructions",
                "1|TD-|goto r0");
        }

        [Test]
        public void SHRw_lds_l_pr()
        {
            Given_HexString("264F"); // lds.l\t@r15+,pr
            AssertCode(
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v2 = Mem0[r15:word32]",
                "2|L--|r15 = r15 + 4",
                "3|L--|pr = v2");
        }

        [Test]
        public void SHRw_mov_I_r()
        {
            Given_HexString("FFE1"); // mov\t#FF,r1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = 0xFFFFFFFF");
        }

        [Test]
        public void SHRw_mov_l_predec()
        {
            Given_HexString("862F"); // mov.l\tr8,@-r15
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r15 = r15 - 4",
                "2|L--|Mem0[r15:word32] = r8");
        }

        [Test]
        public void SHRw_mov_l_disp_pc()
        {
            Given_HexString("02D0"); // mov.l\t@(08,pc),r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = Mem0[0x0010000C:word32]");
        }

        [Test]
        public void SHRw_mov_l_indexed_r()
        {
            Given_HexString("CE 01"); // mov.l\t@(r0,r12),r1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = Mem0[r0 + r12:word32]");
        }

        [Test]
        public void SHRw_mov_l_r_indexed()
        {
            Given_HexString("62 52"); // mov.l\t@(8,r6),r2
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r2 = Mem0[r6 + 8:word32]");

        }

        [Test]
        public void SHRw_mov_l_indirect_r()
        {
            Given_HexString("12 62"); // mov.l\t@r1,r2
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r2 = Mem0[r1:word32]");

        }

        [Test]
        public void SHRw_mov_r_r()
        {
            Given_HexString("7364"); // mov\tr7,r4
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = r7");

        }

        [Test]
        public void SHRw_mov_w_indirect_r()
        {
            Given_HexString("0500"); // mov.w\tr0,@(r0,r0)
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r0 + r0:word16] = r0");
        }

        [Test]
        public void SHRw_mov()
        {
            Given_HexString("8669"); // mov.l\t@r8+,r9
            AssertCode(
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v2 = Mem0[r8:word32]",
                "2|L--|r8 = r8 + 4",
                "3|L--|r9 = v2");
        }

        [Test]
        public void SHRw_mov_w_pc()
        {
            Given_HexString("3390"); // mov.w\t@(66,pc),r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = Mem0[0x0010006A:word16]");
        }

        [Test]
        public void SHRw_mova()
        {
            Given_HexString("3E C7"); // mova\t@(F8,pc),r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = 001000FC");
        }

        [Test]
        public void SHRw_movt()
        {
            Given_HexString("29 00"); // movt\tr0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = (int32) T");
        }

        [Test]
        public void SHRw_nop()
        {
            Given_HexString("0900"); // nop
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void SHRw_neg()
        {
            Given_HexString("0B60"); // neg\tr0,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = -r0");
        }

        [Test]
        public void SHRw_not()
        {
            Given_HexString("9760"); // not\tr9,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = ~r9");
        }

        [Test]
        public void SHRw_rts()
        {
            Given_HexString("0B00"); // rts
            AssertCode(
                "0|TD-|00100000(2): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void SHRw_shll2()
        {
            Given_HexString("0840"); // shll2\tr0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0 << 2");
        }

        [Test]
        public void SHRw_sts_l_pr_predec()
        {
            Given_HexString("224F"); // sts.l\tpr,@-r15
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r15 = r15 - 4",
                "2|L--|Mem0[r15:word32] = pr");
        }

        [Test]
        public void SHRw_tst_imm()
        {
            Given_HexString("01C8"); // tst\t#01,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = (r0 & 0x00000001) == 0x00000000");
        }

        [Test]
        public void SHRw_tst_r_r()
        {
            Given_HexString("6826"); // tst\tr6,r6
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = (r6 & r6) == 0x00000000");
        }

        [Test]
        public void SHRw_sts_mach()
        {
            Given_HexString("0A00");   // sts\tmach,r0",
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = mach");
        }

        [Test]
        public void SHRw_sts_shld()
        {
            Given_HexString("1D40");  // "shld\tr1,r0"
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r1 >= 0x00000000 ? r0 << r1 : r0 >>u r1");
        }

        [Test]
        public void SHRw_sts_shad()
        {
            Given_HexString("1C40");  // "shad\tr1,r0"
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r1 >= 0x00000000 ? r0 << r1 : r0 >> r1");
        }

        [Test]
        public void SHRw_sts_subc()
        {
            Given_HexString("1A 32");  // "subc\tr1,r2"
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r2 = r2 - r1 - T");
        }

        [Test]
        public void SHRw_sts_swap_w()
        {
            Given_HexString("4960");  // "swap.w\tr4,r0"
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = __swap_w(r4)");
        }

        [Test]
        public void SHRw_sts_shlr_16()
        {
            Given_HexString("2944");  // "shlr16\tr4"
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = r4 >>u 16");
        }

        [Test]
        public void SHRw_sts_shll_16()
        {
            Given_HexString("2845");  // "shll16\tr5"
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r5 = r5 << 16");
        }

        [Test]
        public void SHRw_sts_xtrct()
        {
            Given_HexString("4D20");  // "xtrct\tr4,r0"
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = __xtrct(r0, r4)");
        }

        [Test]
        public void SHRw_sts_shar()
        {
            Given_HexString("2141");  // "shar\tr1"
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = r1 >> 1");
        }

        [Test]
        public void SHRw_shll()
        {
            Given_HexString("0041");	// shll	r1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = r1 << 1");
        }

        [Test]
        public void SHRw_cmp_pz()
        {
            Given_HexString("1141");	// cmp/pz	r1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = r1 >= 0x00000000");
        }

        [Test]
        public void SHRw_cmp_pl()
        {
            Given_HexString("1546");	// cmp/pl	r6
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = r6 > 0x00000000");
        }

        [Test]
        public void SHRw_shlr8()
        {
            Given_HexString("1940");	// shlr8	r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0 >>u 8");
        }

        [Test]
        public void SHRw_shlr()
        {
            Given_HexString("0942");	// shlr	r2
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r2 = r2 >>u 1");
        }

        [Test]
        public void SHRw_clrt()
        {
            Given_HexString("0800");	// clrt
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = false");
        }

        [Test]
        public void SHRw_rotcl()
        {
            Given_HexString("2440");	// invalid
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = __rcl(r0, 1, T)");
        }

        [Test]
        public void SHRw_mulu_w()
        {
            Given_HexString("BE20");	// mulu.w
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|macl = (uint16) r0 *u (uint16) r11");
        }

        [Test]
        public void SHRw_sett()
        {
            Given_HexString("1800");	// sett
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = true");
        }

        [Test]
        public void SHRw_negc()
        {
            Given_HexString("9A69");	// negc	r9,r9
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r9 = -r9 - T");
        }

        [Test]
        public void SHRw_ror()
        {
            Given_HexString("0540");	// invalid
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = __ror(r0, 1)");
        }

        [Test]
        public void SHRw_muls_w()
        {
            Given_HexString("1F22");	// invalid
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|macl = (int16) r2 *s (int16) r1");
        }

        [Test]
        public void SHRw_pc_relative_load()
        {
            Given_HexString("00DD44332211");    // mov.l@(0,pc),r13
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r13 = Mem0[0x00100004:word32]",
                "2|L--|00100002(2): 1 instructions",
                "3|L--|r3 = __div1(r3, r4)",
                "4|L--|00100004(2): 1 instructions",
                "5|L--|Mem0[r1 + 8:word32] = r2");
        }

        [Test]
        public void SHRw_mac_l()
        {
            Given_HexString("FF00");	// mac.l	@r15+,@r0+
            AssertCode(
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v2 = Mem0[r15:word32]",
                "2|L--|r15 = r15 + 4",
                "3|L--|v4 = Mem0[r0:word32]",
                "4|L--|r0 = r0 + 4",
                "5|L--|mac = v2 *s v4 + mac");
        }

        [Test]
        public void SHRw_cmp_str()
        {
            Given_HexString("AC25");	// cmp/str	r10,r5
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|T = __cmp_str(r5, r10)");
        }

        [Test]
        public void SHRw_stc()
        {
            Given_HexString("0200");	// stc	sr,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = sr");
        }

        [Test]
        public void SHRw_lds()
        {
            Given_HexString("2A4F");	// lds	r15,pr
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|pr = r15");
        }

        [Test]
        public void SHRw_clrs()
        {
            Given_HexString("4800");	// clrs
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|S = false");
        }

        [Test]
        public void SHRw_fmac()
        {
            Given_HexString("0EF4");	// fmac	fr0,fr0,fr4
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|fr4 = fr0 * fr0 + fr4");
        }

        [Test]
        public void SHRw_ocbi()
        {
            Given_HexString("9300");	// ocbi	@r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__ocbi(r0)");
        }

        [Test]
        public void SHRw_fmov_d()
        {
            Given_HexString("08FB");	// fmov.d	@r11,dr0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|dr0 = Mem0[r11:word64]");
        }

        [Test]
        public void SHRw_fmov_s()
        {
            Given_HexString("D8FC");	// fmov.s	@r12,fr13
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|fr13 = Mem0[r12:word32]");
        }
    }
}

