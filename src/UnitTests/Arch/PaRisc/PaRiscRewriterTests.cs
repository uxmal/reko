#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.PaRisc
{
    [TestFixture]
    public class PaRiscRewriterTests : RewriterTestBase
    {
        private PaRiscArchitecture arch;
        private MemoryArea mem;

        public PaRiscRewriterTests()
        {
            this.arch = new PaRiscArchitecture("parisc");
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr32(0x00100000);

        private void BuildTest(string hexBytes)
        {
            var bytes = base.ParseHexPattern(hexBytes);
            this.mem = new MemoryArea(LoadAddress, bytes);
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            return new PaRiscRewriter(
                arch,
                mem.CreateBeReader(0),
                arch.CreateProcessorState(),
                binder,
                host);
        }

        [Test]
        public void PaRiscRw_add()
        {
            BuildTest("08E18624");  // add\tr1,r7,r4,tr
            AssertCode(
                "0|T--|00100000(4): 3 instructions",
                "1|L--|r4 = r1 + r7",
                "2|L--|C = cond(r4)",
                "3|T--|if (Test(OV,r4 - 0x0000000000000000)) branch 00100008");
        }

        // memMgmt
        [Test]
        [Ignore("Format is complex; try simpler ones first")]
        public void PaRiscRw_058C7910()
        {
            BuildTest("058C7910");  // @@@
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|@@@");
        }


        [Test]
        public void PaRiscRw_break()
        {
            BuildTest("00000000");  // break\t00,0000
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|L--|__break()");
        }

        [Test]
        public void PaRiscRw_bl()
        {
            BuildTest("E800A3D8");  // bl\t00101EC8
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call 001001F4 (0)");
        }

        [Test]
        public void PaRiscRw_nop()
        {
            BuildTest("08000240");  // or\tr0,r0,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void PaRiscRw_ldw()
        {
            BuildTest("4BC23FD1");  // ldw\t-24(sr0,r30),r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = (uint64) Mem0[r30 + -24:word32]");
        }

        [Test]
        public void PaRiscRw_bv_n_r2()
        {
            BuildTest("E840D002");  // bv,n\tr0(r2)
            AssertCode(
                "0|TDA|00100000(4): 1 instructions",
                "1|TD-|return (0,0)");
        }

        [Test]
        public void PaRiscRw_bv_n_r3()
        {
            BuildTest("E860D002");  // bv,n\tr0(r3)
            AssertCode(
                "0|TDA|00100000(4): 1 instructions",
                "1|TD-|goto r3");
        }

        [Test]
        public void PaRiscRw_ldw_short()
        {
            BuildTest("0EC41093");  // ldw\t2(sr0,r22),r19
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = (uint64) Mem0[r22 + 2:word32]");
        }

        [Test]
        public void PaRiscRw_ldsid()
        {
            BuildTest("02C010A1");  // ldsid\tr22,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r22");
        }

        [Test]
        public void PaRiscRw_mtsp()
        {
            BuildTest("00011820");  // mtsp\tr1,sr0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|sr0 = r1");
        }

        [Test]
        public void PaRiscRw_be()
        {
            BuildTest("E2C00000");  // be\t0(sr0,r22)
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|goto r22 + 0");
        }

        [Test]
        public void PaRiscRw_stw()
        {
            BuildTest("6BC23FD1");  // stw\tr2,-18(sp)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r30 + -24:word32] = SLICE(r2, word32, 0)");
        }

        [Test]
        public void PaRiscRw_ldo()
        {
            BuildTest("37DE0080");  // ldo\t40(r30),r30
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = r30 + 64");
        }

        [Test]
        public void PaRiscRw_ldil()
        {
            BuildTest("23E12000");  // ldil\t00012000,r31
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r31 = 0x09000000");
        }

        [Test]
        public void PaRiscRw_ble()
        {
            BuildTest("E7E02EF0");  // ble\t7648(sr0,r31)
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|goto r31 + 7648");
        }

        [Test]
        public void PaRiscRw_ldo_copy()
        {
            BuildTest("37E20000");  // ldo\t0(r31),r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r31");
        }

        [Test]
        public void PaRiscRw_cmpb_ult()
        {
            BuildTest("83C78EEC");
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (r7 <u r30) branch 0010177C");
        }

        [Test]
        public void PaRiscRw_addib_64()
        {
            BuildTest("AFC1CFD5");  // addibf\t-00000010,r30,00101FB4
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|L--|r30 = r30 + -16",
                "2|TD-|if (Test(OV,r30 - 0x0000000000000000)) branch 000FF7F0");
        }

        [Test]
        public void PaRiscRw_stb()
        {
            BuildTest("61716B15");  // stb\tr17,-2A76(r11)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + -2678:byte] = SLICE(r17, byte, 0)");
        }

        [Test]
        public void PaRiscRw_sth()
        {
            BuildTest("656e6400");  // sth\tr14,1024(r11)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + 4608:word16] = SLICE(r14, word16, 0)");
        }

        [Test]
        public void PaRiscRw_depwi()
        {
            BuildTest("d7c01c1d");  // depwi\t00,1F,00000003,r30
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = DPB(r30, 0x0, 0)");
        }

        [Test]
        public void PaRiscRw_fstw()
        {
            BuildTest("27791200");  // fstw\tfr0,-4(r27)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r27 + -4:real32] = fr0L");
        }

        [Test]
        public void PaRiscRw_fldw()
        {
            BuildTest("27791000");  // fldw\t-4(r27),fr0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fr0L = Mem0[r27 + -4:real32]");
        }

        [Test]
        public void PaRiscRw_addi()
        {
            BuildTest("B4C810C2");  // addi,tr\t+00000061,r6,r8
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r8 = r6 + 97",
                "2|T--|goto 00100008");
        }

        [Test]
        public void PaRiscRw_cmpb_ugt_n()
        {
            BuildTest("8bd7a06a");  // cmpb,>>,n\tr23,r30,0010003C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r23 >u r30) branch 0010003C");
        }

        [Test]
        public void PaRiscRw_cmpb_ult_n()
        {
            BuildTest("83178062");  // cmpb,<<,n\tr23,r24,00100038
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r23 <u r24) branch 00100038");
        }

        [Test]
        public void PaRiscRw_extrw_u()
        {
            BuildTest("d0a619fa");  // extrw,u\tr5,0F,06,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = (uint32) SLICE(r5, word6, 17)");
        }

        [Test]
        public void PaRiscRw_addil()
        {
            BuildTest("2B6AAAAA");  // addil\tL%55595000,r27,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r27 + 1431916544");
        }

        [Test]
        public void PaRiscRw_stw_ma()
        {
            BuildTest("6fc30100");  // stw,ma\tr3,128(r30)
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = r30 + 128",
                "2|L--|r30 = v3",
                "3|L--|Mem0[v3:word32] = SLICE(r3, word32, 0)");
        }

        [Test]
        public void PaRiscRw_stw_mb()
        {
            BuildTest("6fc32103");  // stw,ma\tr3,128(r30)
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r30 = r30 + -3968",
                "2|L--|Mem0[r30:word32] = SLICE(r3, word32, 0)");
        }

        [Test]
        public void PaRiscRw_stw_ma_negative_offset()
        {
            BuildTest("6FC35555");  // stw,ma\tr3,128(r30)
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r30 = r30 + -5464",
                "2|L--|Mem0[r30:word32] = SLICE(r3, word32, 0)");
        }

        [Test]
        public void PaRiscRw_ldb()
        {
            BuildTest("0fe01018");  // ldb\t0(sr0,r31),r24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r24 = (uint64) Mem0[r31:byte]");
        }

        [Test]
        public void PaRiscRw_addil_neg()
        {
            BuildTest("2B7FFFFF");	// addil	L%-00000800,r27,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r27 + -2048");
        }

        [Test]
        public void PaRiscRw_shladd()
        {
            BuildTest("0BE30A84");	// shladd 2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r31 + (r3 << 2)");
        }

        [Test]
        public void PaRiscRw_addib()
        {
            BuildTest("AC7F5FDD");	// addibf	-1,r3,00003140
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|L--|r3 = r3 + -1",
                "2|TD-|if (r3 >= 0x0000000000000000) branch 000FFFF4");
        }

        [Test]
        public void PaRiscRw_ldw_mb()
        {
            BuildTest("4FC33F81");	// ldw,mb
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r30 = r30 + -64",
                "2|L--|r3 = (uint64) Mem0[r30:word32]");
        }

        // If you're making a backward jump, annul the following instruction
        // only if you don't take the branch back.
        [Test]
        public void PaRiscRw_addib_annul_back()
        {
            BuildTest("A45930FF"); //  "addib,=\t-00000004,r2,000FF884");
            AssertCode(
                "0|TD-|00100000(4): 3 instructions",
                "1|L--|r2 = r2 + -4",
                "2|T--|if (r2 != 0x0000000000000000) branch 00100008",
                "3|TD-|goto 000FF884");
        }

        // If you're making a forward jump, annul the following instruction
        // only if you do take the branch forward.
        [Test]
        public void PaRiscRw_addib_annul_forward()
        {
            BuildTest("A459200A"); //  "addib,*=\t-00000004,r2,0010F7F0");
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r2 = r2 + -4",
                "2|T--|if (r2 == 0x0000000000000000) branch 0010000C");
        }

        [Test]
        public void PaRiscRw_subi()
        {
            BuildTest("97E40002");	// subi	+00000002,r31,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r31 - 1");
        }

        [Test]
        public void PaRiscRw_fcpy()
        {
            BuildTest("3BC541D4");	// fcpy,dbl	fr29R,fr20R
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fr20R = fr29R");
        }

        [Test]
        public void PaRiscRw_ldwa()
        {
            BuildTest("0D77A9A4");	// ldwa,sm	r23(sr2,r11),r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = (uint64) Mem0[r11 + r23:word32]");
        }

        [Test]
        public void PaRiscRw_cmpib()
        {
            BuildTest("8D1978D0");	// cmpib,>	FFFFFFF9,r8,00001D5C
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (r8 > 0xFFFFFFF9) branch 00100C70");
        }

        [Test]
        public void PaRiscRw_add_l()
        {
            BuildTest("0A57DA08");	// add,l,nsv	r23,r18,r8
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r8 = r23 + r18",
                "2|T--|if (Test(OV,r8 - 0x0000000000000000)) branch 00100008");
        }

        [Test]
        public void PaRiscRw_add_c()
        {
            BuildTest("0AF80729");	// add,c	r24,r23,r9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r24 + r23 + C");
        }

        [Test]
        public void PaRiscRw_ldd()
        {
            BuildTest("0E6B1CCB");	// ldd	-11(r19),r11
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = Mem0[r19 + -11:word64]");
        }

        [Test]
        public void PaRiscRw_shrpd()
        {
            BuildTest("D0A9A573");	// shrpd,*<>	r9,r5,00000034,r19
            AssertCode(
                "0|T--|00100000(4): 3 instructions",
                "1|L--|r9_r5 = SEQ(r9, r5)",
                "2|L--|r19 = SLICE(r9_r5 >>u 0x00000034, word64, 0)",
                "3|T--|if (r19 != 0x0000000000000000) branch 00100008");
        }

        [Test]
        public void PaRiscRw_addb()
        {
            BuildTest("A8C1CFD5");	// addb,nsv	r1,r6,00000A70
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|L--|r6 = r6 + r1",
                "2|TD-|if (Test(OV,r6 - 0x0000000000000000)) branch 000FF7F0");
        }

        [Test]
        public void PaRiscRw_mfctl_w()
        {
            BuildTest("000068B8");	// mfctl,w	rctr,r24
            AssertCode(
                "0|S--|00100000(4): 1 instructions",
                "1|L--|r24 = rctr");
        }

        [Test]
        public void PaRiscRw_stda()
        {
            BuildTest("0D00FFFF");	// stda,mb	r0,-1(sr3,r8)
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r8 = r8 + -1",
                "2|L--|Mem0[r8:word64] = 0x0000000000000000");
        }

        [Test]
        public void PaRiscRw_fldd()
        {
            BuildTest("2E736C00");	// fldd,s	r19(sr1,r19),fr0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fr0 = Mem0[r19 + r19:real64]");
        }

        [Test]
        public void PaRiscRw_and()
        {
            BuildTest("08392203");	// and,=	r25,r1,r3
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r25 & r1",
                "2|T--|if (r3 == 0x0000000000000000) branch 00100008");
        }

        [Test]
        public void PaRiscRw_addi_tc()
        {
            BuildTest("B3212005");	// addi,tc,=	r1,r25,r5
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r1 = r25 + -1022",
                "2|T--|if (r1 != 0x00000000) branch 00100004",
                "3|L--|__trap()");
        }

        [Test]
        public void PaRiscRw_andcm()
        {
            BuildTest("0BB80001");	// andcm	r24,r29,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r24 & ~r29");
        }

        [Test]
        public void PaRiscRw_mtctl()
        {
            BuildTest("037A1840");	// mtctl	r26,tr3
            AssertCode(
                "0|S--|00100000(4): 1 instructions",
                "1|L--|tr3 = r26");
        }

        [Test]
        public void PaRiscRw_mfctl()
        {
            BuildTest("036008BC");	// mfctl	tr3,r28
            AssertCode(
                "0|S--|00100000(4): 1 instructions",
                "1|L--|r28 = tr3");
        }

        [Test]
        public void PaRiscRw_fsub()
        {
            BuildTest("31362F31");	// fsub,dbl	fr9,fr22,fr17
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fr17 = fr9 - fr22");
        }

        [Test]
        public void PaRiscRw_fmpy()
        {
            BuildTest("31444E18");	// fmpy,dbl	fr10,fr4,fr24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fr24 = fr10 * fr4");
        }

        [Test]
        public void PaRiscRw_fstd()
        {
            BuildTest("2D707269");	// fstd,mb	8(sr1,r11),fr9
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r11 = r11 + 8",
                "2|L--|Mem0[r11:real64] = fr9");
        }

        [Test]
        public void PaRiscRw_fid()
        {
            BuildTest("30000000");	// fid
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__fid()");
        }

        [Test]
        public void PaRiscRw_diag()
        {
            BuildTest("14008000");	// diag	+00008000
            AssertCode(
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__diag(32768)");
        }

        [Test]
        public void PaRiscRw_rfi()
        {
            BuildTest("00002C00");	// rfi
            AssertCode(
                "0|S--|00100000(4): 2 instructions",
                "1|L--|__rfi()",
                "2|T--|return (0,0)");
        }

        [Test]
        public void PaRiscRw_rfi_r()
        {
            BuildTest("00002CA0");	// rfi,r
            AssertCode(
                "0|S--|00100000(4): 2 instructions",
                "1|L--|__rfi_r()",
                "2|T--|return (0,0)");
        }

        [Test]
        public void PaRiscRw_mtsm()
        {
            BuildTest("00003870");	// mtsm	r0
            AssertCode(
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__mtsm(0x0000000000000000)");
        }

        [Test]
        public void PaRiscRw_stb_disp_0()
        {
            BuildTest("0f201212");  // stb\tr0,9(r25)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r25 + 9:byte] = 0x00");
        }

#if NYI

        [Test]
        public void PaRiscRw_addi_tsv()
        {
            BuildTest("B7F96CE9");	// addi,tsv,<=	-0000018C,r31,r25
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|@@@",
                "2|L--|@@@",
                "3|L--|@@@");
        }

        [Test]
        public void PaRiscRw_bb()
        {
            BuildTest("C7D6C012");	// bb,>=,n	r22,0000001E,00001D10
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_cldd()
        {
            BuildTest("2F188176");	// cldd,5,m	r24(sr2,r24),r22
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_cldw()
        {
            BuildTest("2446696C");	// cldw,5,sm,sl	r6(sr1,r2),r12
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_cmpiclr()
        {
            BuildTest("93A06000");	// cmpiclr,<=	+00000000,r29,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_cmpclr()
        {
            BuildTest("0A984880");	// cmpclr,<	r24,r20,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_cstd()
        {
            BuildTest("2D363733");	// cstd,4,mb,bc	r19,11(r9)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_cstw()
        {
            BuildTest("271536E6");	// cstw,3,mb,bc	r6,-6(r24)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_depw()
        {
            BuildTest("D7E00C1E");	// depw	r0,1F,00000002,r31
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_depwi_z()
        {
            BuildTest("d7c6181d");  // depwi,z\t03,1F,00000003,r30
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = DPB(0x00000000, 0x3, 0)");
        }

        [Test]
        public void PaRiscRw_ds()
        {
            BuildTest("0BA00440");	// ds	r0,r29,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcmp()
        {
            BuildTest("38206D61");	// fcmp,false	fr1R,fr0L
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcnv()
        {
            BuildTest("389EBAE5");	// fcnv,uqw,dbl	fr4R,fr5R
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcnv_t()
        {
            BuildTest("388B9A76");	// fcnv,t,quad,uw	fr4R,fr22L
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcnvfx()
        {
            BuildTest("32313A30");	// fcnvfx,quad,dw	fr17,fr16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcnvfxt()
        {
            BuildTest("33018A07");	// fcnvfxt,dbl,w	fr24,fr7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fcnvxf()
        {
            BuildTest("30E0A206");	// fcnvxf,w,dbl	fr7,fr6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fmpyadd()
        {
            BuildTest("1B8177DB");	// fmpyadd,dbl	fr28,fr1,fr27,fr31,fr14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fmpysub()
        {
            BuildTest("99C4BB7D");	// fmpysub,sgl	fr30L,fr20L,fr29L,fr29L,fr23L
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_hshl()
        {
            BuildTest("F90B8A37");	// hshl
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_hshradd()
        {
            BuildTest("0A526570");	// hshradd	r18,+00000001,r18,r16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_hsub_ss()
        {
            BuildTest("0AA5817C");	// hsub,ss	r5,r21,r28
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_ldcw()
        {
            BuildTest("0D77A9D4");	// ldcw,s	r23(sr2,r11),r20
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        public void PaRiscRw_movb()
        {
            BuildTest("CA500580");	// movb	r16,r18,00001474
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_movib()
        {
            BuildTest("CCF006D9");	// movib	+00000008,r7,FFFFF538
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_permh()
        {
            BuildTest("FAF53C79");	// permh
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_spop0()
        {
            BuildTest("13010000");	// spop0	00000000,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_uaddcm()
        {
            BuildTest("086B99B6");	// uaddcm	r11,r3,r22
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_xmpyu()
        {
            BuildTest("3925471A");	// xmpyu	fr18L,fr10L,fr26
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }
#endif
    }
}
