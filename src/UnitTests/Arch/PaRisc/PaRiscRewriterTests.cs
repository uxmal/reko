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

using NUnit.Framework;
using Reko.Arch.PaRisc;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.PaRisc
{
    [TestFixture]
    public partial class PaRiscRewriterTests : RewriterTestBase
    {
        private readonly PaRiscArchitecture arch;

        public PaRiscRewriterTests()
        {
            var options = new Dictionary<string, object>
            {
                { ProcessorOption.WordSize, 64 },
            };

            this.arch = new PaRiscArchitecture(CreateServiceContainer(), "parisc", options);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr32(0x00100000);

        [Test]
        public void PaRiscRw_add()
        {
            Given_HexString("08E18624");  // add\tr1,r7,r4,tr
            AssertCode(
                "0|T--|00100000(4): 3 instructions",
                "1|L--|r4 = r1 + r7",
                "2|L--|C = cond(r4)",
                "3|T--|if (Test(OV,r4 -u 0<64>)) branch 00100008");
        }

        [Test]
        public void PaRiscRw_add_c()
        {
            Given_HexString("0AF80729");	// add,c	r24,r23,r9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r24 + r23 + C");
        }

        [Test]
        public void PaRiscRw_add_l()
        {
            Given_HexString("0A57DA08");	// add,l,nsv	r23,r18,r8
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r8 = r23 + r18",
                "2|T--|if (Test(OV,r8 - 0<64>)) branch 00100008");
        }

        [Test]
        public void paRiscRw_add_l_nsv()
        {
            Given_HexString("0898DA21");
            AssertCode(     // add,l,*nsv	r0,r0,r1
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r1 = r24 + r4",
                "2|T--|if (Test(OV,r1 - 0<64>)) branch 00100008");
        }

        [Test]
        public void PaRiscRw_add_l_uv()
        {
            Given_HexString("082A9A13");
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r19 = r10 + r1",
                "2|T--|if (Test(NO,r19 -u 0<64>)) branch 00100008");
        }

        [Test]
        public void PaRiscRw_add_sv()
        {
            Given_HexString("094FCA2F");
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r15 = r15 + r10",
                "2|T--|if (Test(NO,r15 - 0<64>)) branch 00100008");
        }

        [Test]
        public void PaRiscRw_add_znv()
        {
            Given_HexString("0859AA0A");
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r10 = r25 + r2",
                "2|T--|if (r10 != 0<64> & Test(OV,r10 - 0<64>)) branch 00100008");
        }

        [Test]
        public void PaRiscRw_addb()
        {
            Given_HexString("A8C1CFD5");	// addb,nsv	r1,r6,00000A70
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|L--|r6 = r6 + r1",
                "2|TD-|if (Test(OV,r6 - 0<64>)) branch 000FF7F0");
        }

        [Test]
        public void PaRiscRw_addib_64()
        {
            Given_HexString("AFC1CFD5");  // addib\t*>=-00000010,r30,00101FB4
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|L--|r30 = r30 + -16<i64>",
                "2|TD-|if (r30 >= 0<64>) branch 000FF7F0");
        }

        // memMgmt
        [Test]
        [Ignore("Format is complex; try simpler ones first")]
        public void PaRiscRw_058C7910()
        {
            Given_HexString("058C7910");  // @@@
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_and()
        {
            Given_HexString("08392203");	// and,=	r25,r1,r3
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r25 & r1",
                "2|T--|if (r3 == 0<64>) branch 00100008");
        }

        [Test]
        public void PaRiscRw_break()
        {
            Given_HexString("00000000");  // break\t00,0000
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|L--|__break()");
        }

        [Test]
        public void PaRiscRw_bl()
        {
            Given_HexString("E800A3D8");  // bl\t00101EC8
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call 001001F4 (0)");
        }

        [Test]
        public void PaRiscRw_cmpib()
        {
            Given_HexString("8D1978D0");	// cmpib,>	FFFFFFF9,r8,00001D5C
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (r8 > 0xFFFFFFF9<32>) branch 00100C70");
        }

        [Test]
        public void PaRiscRw_nop()
        {
            Given_HexString("08000240");  // or\tr0,r0,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void PaRiscRw_ldw()
        {
            Given_HexString("4BC23FD1");  // ldw\t-24(sr0,r30),r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = CONVERT(Mem0[r30 + -24<i64>:word32], word32, uint64)");
        }

        [Test]
        public void PaRiscRw_bv_n_r2()
        {
            Given_HexString("E840D002");  // bv,n\tr0(r2)
            AssertCode(
                "0|TDA|00100000(4): 1 instructions",
                "1|RD-|return (0,0)");
        }

        [Test]
        public void PaRiscRw_bv_n_r3()
        {
            Given_HexString("E860D002");  // bv,n\tr0(r3)
            AssertCode(
                "0|TDA|00100000(4): 1 instructions",
                "1|TD-|goto r3");
        }

        [Test]
        public void PaRiscRw_ldw_short()
        {
            Given_HexString("0EC41093");  // ldw\t2(sr0,r22),r19
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = CONVERT(Mem0[r22 + 2<i64>:word32], word32, uint64)");
        }

        [Test]
        public void PaRiscRw_ldsid()
        {
            Given_HexString("02C010A1");  // ldsid\tr22,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r22");
        }

        [Test]
        public void PaRiscRw_mtsp()
        {
            Given_HexString("00011820");  // mtsp\tr1,sr0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|sr0 = r1");
        }

        [Test]
        public void PaRiscRw_bb()
        {
            Given_HexString("C7D6C012");
            AssertCode(     // bb,>=,n	r22,0000001E,0000232C
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (!__is_bit_set<word64,int32>(r22, 30<i32>)) branch 00100010");
        }

        [Test]
        public void PaRiscRw_be()
        {
            Given_HexString("E2C00000");  // be\t0(sr0,r22)
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|goto r22 + 0<i64>");
        }

        [Test]
        public void PaRiscRw_be_l()
        {
            Given_HexString("E4C00000");  // be,l\t0(sr0,r22)
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call r6 + 0<i64> (0)");
        }

        [Test]
        public void PaRiscRw_be_l_n()
        {
            // Nullifying is implemented by injecting a jump
            // that skips the nullified instruction.
            // We don't have to make a delay slot in this case.
            Given_HexString("E4C00002");  // be,l\t0(sr0,r22)
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|call r6 + 0<i64> (0)",
                "2|T--|goto 00100008");
        }

        [Test]
        public void PaRiscRw_cmpclr()
        {
            Given_HexString("081A5880");
            AssertCode(     // cmpclr,>=	r26,r0,r0
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = r26",
                "2|L--|v5 = 0<64>",
                "3|T--|if (v4 >= v5) branch 00100008");
        }

        [Test]
        public void PaRiscRw_cmpiclr()
        {
            Given_HexString("905C2000");
            AssertCode(     // cmpiclr,=	+00000000,r2,r28
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = r2",
                "2|L--|v5 = 0<i64>",
                "3|L--|r28 = 0<64>",
                "4|T--|if (v4 == v5) branch 00100008");
        }

        [Test]
        public void PaRiscRw_ds()
        {
            Given_HexString("08200440");
            AssertCode(     // ds	r0,r1,r0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __division_step<word64>(0<64>, r1)");
        }

        [Test]
        public void PaRiscRw_stw()
        {
            Given_HexString("6BC23FD1");  // stw\tr2,-18(sp)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r30 + -24<i64>:word32] = SLICE(r2, word32, 0)");
        }

        [Test]
        public void PaRiscRw_ldo()
        {
            Given_HexString("37DE0080");  // ldo\t40(r30),r30
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = r30 + 64<i64>");
        }

        [Test]
        public void PaRiscRw_ldil()
        {
            Given_HexString("23E12000");  // ldil\tL%3000,r31
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r31 = 12288<i32>");
        }

        [Test]
        public void PaRiscRw_ble()
        {
            Given_HexString("E7E02EF0");  // ble\t1912(sr0,r31)
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call r31 + 1912<i64> (0)");
        }

        [Test]
        public void PaRiscRw_cmpb_ult()
        {
            Given_HexString("83C78EEC");
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (r7 <u r30) branch 0010177C");
        }

        [Test]
        public void PaRiscRw_depw()
        {
            Given_HexString("D4390838");
            AssertCode(     // depw,z	r25,1E,00000008,r1
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r25, byte, 0)",
                "2|L--|r1 = 0<32>",
                "3|L--|r1 = SEQ(SLICE(r1, word31, 33), v5, SLICE(r1, bool, 0))");
        }

        [Test]
        public void PaRiscRw_ldo_copy()
        {
            Given_HexString("37E20000");  // ldo\t0(r31),r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r31");
        }

        [Test]
        public void PaRiscRw_stb()
        {
            Given_HexString("61716B15");  // stb\tr17,-2A76(r11)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + -10870<i64>:byte] = SLICE(r17, byte, 0)");
        }

        [Test]
        public void PaRiscRw_sth()
        {
            Given_HexString("656e6400");  // sth\tr14,1024(r11)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + 12800<i64>:word16] = SLICE(r14, word16, 0)");
        }

        [Test]
        public void PaRiscRw_depwi()
        {
            Given_HexString("d7c01c1d");  // depwi\t00,1F,00000003,r30
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = SEQ(SLICE(r30, word61, 3), 0<3>)");
        }

        [Test]
        public void PaRiscRw_depwi2()
        {
            Given_HexString("D70E1F60");
            AssertCode( // depwi	+00000007,04,00000020,r24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r24 = SEQ(SLICE(r24, word5, 59), 7<32>, SLICE(r24, word27, 0))");
        }

        [Test]
        public void PaRiscRw_fadd()
        {
            Given_HexString("38160621");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fr1L = fr0L + fr22L");
        }

        [Test]
        public void PaRiscRw_fcnvxf()
        {
            Given_HexString("3080A204");
            AssertCode(     // fcnvxf,w,dbl	fr4,fr4
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(fr4, int32, 0)",
                "2|L--|fr4 = CONVERT(v4, int32, real64)");
        }

        [Test]
        public void PaRiscRw_fcpy()
        {
            Given_HexString("3BC541D4");	// fcpy,dbl	fr29R,fr20R
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fr20R = fr29R");
        }

        [Test]
        public void PaRiscRw_fldd()
        {
            Given_HexString("2E736C00");	// fldd,s	r19(sr1,r19),fr0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fr0 = Mem0[r19 + r19:real64]");
        }

        [Test]
        public void PaRiscRw_fldw()
        {
            Given_HexString("27791000");  // fldw\t-4(r27),fr0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fr0L = Mem0[r27 + -4<i64>:real32]");
        }

        [Test]
        public void PaRiscRw_fstw()
        {
            Given_HexString("27791200");  // fstw\tfr0,-4(r27)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r27 + -4<i64>:real32] = fr0L");
        }

        [Test]
        public void PaRisc_ldd_idx()
        {
            Given_HexString("0F4A 8CF0");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = Mem0[r26 + r10:word64]");
        }

        [Test]
        public void PaRiscRw_addi()
        {
            Given_HexString("B4C810C2");  // addi,tr\t+00000061,r6,r8
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r8 = r6 + 97<i32>",
                "2|T--|goto 00100008");
        }

        [Test]
        public void PaRiscRw_cmpb_ugt_n()
        {
            Given_HexString("8bd7a06a");  // cmpb,>>,n\tr23,r30,0010003C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r23 >u r30) branch 0010003C");
        }

        [Test]
        public void PaRiscRw_cmpb_ult_n()
        {
            Given_HexString("83178062");  // cmpb,<<,n\tr23,r24,00100038
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r23 <u r24) branch 00100038");
        }

        [Test]
        public void PaRiscRw_extrw_u()
        {
            Given_HexString("d0a619fa");  // extrw,u\tr5,0F,06,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = CONVERT(SLICE(r5, word6, 17), word6, uint32)");
        }

        [Test]
        public void PaRiscRw_addil()
        {
            Given_HexString("2B6AAAAA");  // addil\tL%55595000,r27,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r27 + 1431916544<i32>");
        }

        [Test]
        public void PaRiscRw_stw_ma()
        {
            Given_HexString("6fc30100");  // stw,ma\tr3,128(r30)
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = r30 + 128<i64>",
                "2|L--|r30 = v4",
                "3|L--|Mem0[v4:word32] = SLICE(r3, word32, 0)");
        }

        [Test]
        public void PaRiscRw_stw_mb()
        {
            Given_HexString("6fc32103");  // stw,ma\tr3,128(r30)
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r30 = r30 + -3968<i64>",
                "2|L--|Mem0[r30:word32] = SLICE(r3, word32, 0)");
        }

        [Test]
        [Ignore("The HPPA encodings are _atrocious_")]
        public void PaRiscRw_stw_ma_negative_offset()
        {
            Given_HexString("6FC35555");  // stw,ma\tr3,128(r30)
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r30 = r30 + -5464<i64>",
                "2|L--|Mem0[r30:word32] = SLICE(r3, word32, 0)");
        }

        [Test]
        public void PaRiscRw_ldb()
        {
            Given_HexString("0fe01018");  // ldb\t0(sr0,r31),r24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r24 = CONVERT(Mem0[r31:byte], byte, uint64)");
        }

        [Test]
        public void PaRiscRw_addil_neg()
        {
            Given_HexString("2B7FFFFF");	// addil	L%-00000800,r27,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r27 + -2048<i32>");
        }

        [Test]
        public void PaRiscRw_shladd()
        {
            Given_HexString("0BE30A84");	// shladd 2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r31 + (r3 << 2<i32>)");
        }

        [Test]
        public void PaRiscRw_addib()
        {
            Given_HexString("AC7F5FDD");	// addibf	-1,r3,00003140
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|L--|r3 = r3 + -1<i64>",
                "2|TD-|if (r3 >= 0<64>) branch 000FFFF4");
        }

        [Test]
        public void PaRiscRw_ldw_mb()
        {
            Given_HexString("4FC33F81");	// ldw,mb
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r30 = r30 + -64<i64>",
                "2|L--|r3 = CONVERT(Mem0[r30:word32], word32, uint64)");
        }

        [Test]
        public void PaRiscRw_sub_b()
        {
            Given_HexString("0BB9051D");
            AssertCode(     // sub,b	r25,r29,r29
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r29 = r25 - r29 - C");
        }

        [Test]
        public void PaRiscRw_sub_od()
        {
            Given_HexString("083BE41F");
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r31 = r27 - r1",
                "2|L--|if ((r31 & 1<64>) != 0<64>) branch 00100008");
        }

        [Test]
        public void PaRiscRw_subi()
        {
            Given_HexString("97E40002");	// subi	+00000001,r31,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r31 - 1<i32>");     //$LIT
        }

        [Test]
        public void PaRiscRw_ldd()
        {
            Given_HexString("0E6B1CCB");	// ldd	-11(r19),r11
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = Mem0[r19 + -11<i64>:word64]");
        }

        [Test]
        public void PaRiscRw_ldd_m()
        {
            Given_HexString("0C4140E9");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = Mem0[r2 + r1:word64]");
        }

        [Test]
        public void PaRiscRw_ldwa()
        {
            Given_HexString("0D77A9A4");	// ldwa,sm	r23(sr2,r11),r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = CONVERT(Mem0[r11 + r23:word32], word32, uint64)");
        }

        [Test]
        public void PaRiscRw_movb()
        {
            Given_HexString("C8752050");
            AssertCode(     // movb,=	r21,r3,00002290
                "0|TD-|00100000(4): 2 instructions",
                "1|L--|r3 = r21",
                "2|TD-|if (r21 == 0<64>) branch 00100030");
        }

        [Test]
        public void PaRiscRw_shrpd()
        {
            Given_HexString("D0A9A573");	// shrpd,*<>	r9,r5,00000034,r19
            AssertCode(
                "0|T--|00100000(4): 3 instructions",
                "1|L--|r9_r5 = SEQ(r9, r5)",
                "2|L--|r19 = SLICE(r9_r5 >>u 0x34<u32>, word64, 0)",
                "3|T--|if (r19 != 0<64>) branch 00100008");
        }

        [Test]
        public void PaRiscRw_shrpd_3args()
        {
            Given_HexString("D36CCE06");
            AssertCode( // shrpd,*>=	r12,r27,r6
                "0|T--|00100000(4): 3 instructions",
                "1|L--|r12_r27 = SEQ(r12, r27)",
                "2|L--|r6 = SLICE(r12_r27 >>u 0xF<u32>, word64, 0)",
                "3|T--|if (r6 >= 0<64>) branch 00100008");
        }

        [Test]
        public void PaRiscRw_mfctl_w()
        {
            Given_HexString("000068B8");	// mfctl,w	rctr,r24
            AssertCode(
                "0|S--|00100000(4): 1 instructions",
                "1|L--|r24 = rctr");
        }

        [Test]
        public void PaRiscRw_stda()
        {
            Given_HexString("0D00FFFF");	// stda,mb	r0,-1(sr3,r8)
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r8 = r8 + -1<i64>",
                "2|L--|Mem0[r8:word64] = 0<64>");
        }

        [Test]
        public void PaRiscRw_addi_tc()
        {
            Given_HexString("B3212005");	// addi,tc,=	r1,r25,r5
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r1 = r25 + -1022<i32>",
                "2|T--|if (r1 != 0<32>) branch 00100004",
                "3|L--|__trap()");
        }

        [Test]
        public void PaRiscRw_addi_tc_const()
        {
            Given_HexString("B1FA0674");
            AssertCode( // addi,tc	+0000033A,r15,r26
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r26 = r15 + 826<i32>",
                "2|L--|__trap()");
        }

        // If you're making a backward jump, annul the following instruction
        // only if you don't take the branch back.
        [Test]
        public void PaRiscRw_addib_annul_back()
        {
            Given_HexString("A45930FF"); //  "addib,=\t-00000004,r2,000FF884");
            AssertCode(
                "0|TD-|00100000(4): 3 instructions",
                "1|L--|r2 = r2 + -4<i64>",
                "2|T--|if (r2 != 0<64>) branch 00100008",
                "3|TD-|goto 000FF884");
        }

        // If you're making a forward jump, annul the following instruction
        // only if you do take the branch forward.
        [Test]
        public void PaRiscRw_addib_annul_forward()
        {
            Given_HexString("A459200A"); //  "addib,*=\t-00000004,r2,0010F7F0");
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r2 = r2 + -4<i64>",
                "2|T--|if (r2 == 0<64>) branch 0010000C");
        }

        [Test]
        public void PaRiscRw_andcm()
        {
            Given_HexString("0BB80001");	// andcm	r24,r29,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r24 & ~r29");
        }

        [Test]
        public void PaRiscRw_mtctl()
        {
            Given_HexString("037A1840");	// mtctl	r26,tr3
            AssertCode(
                "0|S--|00100000(4): 1 instructions",
                "1|L--|tr3 = r26");
        }

        [Test]
        public void PaRiscRw_mfctl()
        {
            Given_HexString("036008BC");	// mfctl	tr3,r28
            AssertCode(
                "0|S--|00100000(4): 1 instructions",
                "1|L--|r28 = tr3");
        }

        [Test]
        public void PaRiscRw_fsub()
        {
            Given_HexString("31362F31");	// fsub,dbl	fr9,fr22,fr17
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fr17 = fr9 - fr22");
        }

        [Test]
        public void PaRiscRw_fmpy()
        {
            Given_HexString("31444E18");	// fmpy,dbl	fr10,fr4,fr24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fr24 = fr10 * fr4");
        }

        [Test]
        public void PaRiscRw_fstd()
        {
            Given_HexString("2D707269");	// fstd,mb	8(sr1,r11),fr9
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r11 = r11 + 8<i64>",
                "2|L--|Mem0[r11:real64] = fr9");
        }

        [Test]
        public void PaRiscRw_fid()
        {
            Given_HexString("30000000");	// fid
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__fid()");
        }

        [Test]
        public void PaRiscRw_diag()
        {
            Given_HexString("14008000");	// diag	+00008000
            AssertCode(
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__diag<int32>(32768<i32>)");
        }

        [Test]
        public void PaRiscRw_rfi()
        {
            Given_HexString("00002C00");	// rfi
            AssertCode(
                "0|R--|00100000(4): 2 instructions",
                "1|L--|__rfi()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void PaRiscRw_rfi_r()
        {
            Given_HexString("00002CA0");	// rfi,r
            AssertCode(
                "0|R--|00100000(4): 2 instructions",
                "1|L--|__rfi_r()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void PaRiscRw_mtsm()
        {
            Given_HexString("00003870");	// mtsm	r0
            AssertCode(
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__mtsm<word64>(0<64>)");
        }

        [Test]
        public void PaRiscRw_stb_disp_0()
        {
            Given_HexString("0f201212");  // stb\tr0,9(r25)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r25 + 9<i64>:byte] = 0<8>");
        }

        [Test]
        public void PaRiscRw_xor()
        {
            Given_HexString("0B3A029A");
            AssertCode(     // xor	r26,r25,r26
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r26 = r26 ^ r25");
        }

#if NYI

        [Test]
        public void PaRiscRw_addi_tsv()
        {
            Given_HexBytes("B7F96CE9");	// addi,tsv,<=	-0000018C,r31,r25
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|@@@",
                "2|L--|@@@",
                "3|L--|@@@");
        }

        [Test]
        public void PaRiscRw_bb()
        {
            Given_HexBytes("C7D6C012");	// bb,>=,n	r22,0000001E,00001D10
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_cldd()
        {
            Given_HexBytes("2F188176");	// cldd,5,m	r24(sr2,r24),r22
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_cldw()
        {
            Given_HexBytes("2446696C");	// cldw,5,sm,sl	r6(sr1,r2),r12
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_cstd()
        {
            Given_HexBytes("2D363733");	// cstd,4,mb,bc	r19,11(r9)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_cstw()
        {
            Given_HexBytes("271536E6");	// cstw,3,mb,bc	r6,-6(r24)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_depw()
        {
            Given_HexBytes("D7E00C1E");	// depw	r0,1F,00000002,r31
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_depwi_z()
        {
            Given_HexBytes("d7c6181d");  // depwi,z\t03,1F,00000003,r30
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = DPB(0<32>, 0x3, 0)");
        }

        [Test]
        public void PaRiscRw_ds()
        {
            Given_HexBytes("0BA00440");	// ds	r0,r29,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcmp()
        {
            Given_HexBytes("38206D61");	// fcmp,false	fr1R,fr0L
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcnv()
        {
            Given_HexBytes("389EBAE5");	// fcnv,uqw,dbl	fr4R,fr5R
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcnv_t()
        {
            Given_HexBytes("388B9A76");	// fcnv,t,quad,uw	fr4R,fr22L
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcnvfx()
        {
            Given_HexBytes("32313A30");	// fcnvfx,quad,dw	fr17,fr16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcnvfxt()
        {
            Given_HexBytes("33018A07");	// fcnvfxt,dbl,w	fr24,fr7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcnvxf()
        {
            Given_HexBytes("30E0A206");	// fcnvxf,w,dbl	fr7,fr6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fmpyadd()
        {
            Given_HexBytes("1B8177DB");	// fmpyadd,dbl	fr28,fr1,fr27,fr31,fr14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fmpysub()
        {
            Given_HexBytes("99C4BB7D");	// fmpysub,sgl	fr30L,fr20L,fr29L,fr29L,fr23L
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_hshl()
        {
            Given_HexBytes("F90B8A37");	// hshl
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_hshradd()
        {
            Given_HexBytes("0A526570");	// hshradd	r18,+00000001,r18,r16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_hsub_ss()
        {
            Given_HexBytes("0AA5817C");	// hsub,ss	r5,r21,r28
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_ldcw()
        {
            Given_HexBytes("0D77A9D4");	// ldcw,s	r23(sr2,r11),r20
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_movib()
        {
            Given_HexBytes("CCF006D9");	// movib	+00000008,r7,FFFFF538
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_permh()
        {
            Given_HexBytes("FAF53C79");	// permh
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_spop0()
        {
            Given_HexBytes("13010000");	// spop0	00000000,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_uaddcm()
        {
            Given_HexBytes("086B99B6");	// uaddcm	r11,r3,r22
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_xmpyu()
        {
            Given_HexBytes("3925471A");	// xmpyu	fr18L,fr10L,fr26
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }
#endif
    }
}
