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

using NUnit.Framework;
using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcRewriterTests : RewriterTestBase
    {
        private InstructionBuilder b;
        private PowerPcArchitecture arch;
        private PowerPcArchitecture archBe32;
        private PowerPcArchitecture archBe64;
        private PowerPcArchitecture archLe64;
        private PowerPcArchitecture archXenon;
        private PowerPcArchitecture arch750;
        private IEnumerable<PowerPcInstruction> ppcInstrs;
        private Address addr;

        public PowerPcRewriterTests()
        {
            var sc = CreateServiceContainer();
            this.archBe32 = new PowerPcBe32Architecture(sc, "ppc-be-32", []);
            this.archBe64 = new PowerPcBe64Architecture(sc, "ppc-be-64", []);
            this.archLe64 = new PowerPcLe64Architecture(sc, "ppc-le-64", []);

            this.arch750 = new PowerPcBe32Architecture(sc, "ppc-be-32", new Dictionary<string, object>
            {
                { "Model", "750cl" }
            });

            this.archXenon = new PowerPcBe32Architecture(sc, "ppc-be-32", new Dictionary<string, object>
            {
                { "Model", "Xenon" }
            });

            this.addr = Address.Ptr32(0x00100000);
        }

        [SetUp]
        public void Setup()
        {
            this.arch = archBe32;
            this.ppcInstrs = null;
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress => addr;

        private void AssertCode(uint instr, params string[] sExp)
        {
            Given_UInt32s(instr);
            AssertCode(sExp);
        }

        private void AssertCode64(uint instr, params string[] sExp)
        {
            Given_UInt32s(instr);
            AssertCode(sExp);
        }

        private void RunTest(Action<InstructionBuilder> m)
        {
            b = new InstructionBuilder(arch, Address.Ptr32(0x00100000));
            m(b);
            ppcInstrs = b.Instructions;
        }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            if (ppcInstrs is not null)
                return new PowerPcRewriter(arch, ppcInstrs, binder, host);
            return base.GetRtlStream(mem, binder, host);
        }

        private void Given_PowerPcBe64()
        {
            this.arch = archBe64;
        }

        private void Given_PowerPcLe64()
        {
            this.arch = archLe64;
        }

        private void Given_Xenon()
        {
            this.arch = archXenon;
        }

        private void Given_750()
        {
            this.arch = this.arch750;
        }


        [Test]
        public void PPCRw_Oris()
        {
            RunTest((m) =>
            {
                m.Oris(m.r4, m.r0, 0x1234);
            });
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r0 | 0x12340000<32>");
        }

        [Test]
        public void PPCRw_nop()
        {
            Given_HexString("60000000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void PPCRw_add()
        {
            RunTest((m) =>
            {
                m.Add(m.r4, m.r1, m.r3);
            });
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r1 + r3");
        }

        [Test]
        public void PPCRw_add_()
        {
            RunTest((m) =>
            {
                m.Add_(m.r4, m.r1, m.r3);
            });
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r4 = r1 + r3",
                "2|L--|cr0 = cond(r4)");
        }

        [Test]
        public void PPCRw_addeo()
        {
            Given_HexString("7C8BBD15");
            AssertCode(     // addeo.	r4,r11,r23
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r4 = r11 + r23 + xer",
                "2|L--|cr0 = cond(r4)",
                "3|L--|xer = cond(r4)");
        }

        [Test]
        public void PPCRw_addi_r0()
        {
            AssertCode(0x38008045,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = 0xFFFF8045<32>");
        }

        [Test]
        public void PPCRw_addi_r0_64bit()
        {
            Given_PowerPcBe64();
            AssertCode(0x38008045,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = 0xFFFFFFFFFFFF8045<64>");
        }


        [Test]
        public void PPCRw_addis_r0()
        {
            AssertCode(0x3C000045,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = 0x450000<32>");
        }

        [Test]
        public void PPCRw_addis_r0_64()
        {
            Given_PowerPcBe64();
            AssertCode(0x3C000045,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = 0x450000<64>");
        }

        [Test]
        public void PPCRw_addis()
        {
            AssertCode(0x3C810045,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r1 + 0x450000<32>");
        }

        [Test]
        public void PPCRw_addpcis()
        {
            Given_HexString("4D43D645");
            AssertCode(     // addpcis	r10,-000029B9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = 000D6474");
        }

        [Test]
        public void PPCrw_and()
        {
            AssertCode(0x7c7ef039, //"and.\tr30,r3,r30");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r30 = r3 & r30",
                "2|L--|cr0 = cond(r30)");
        }

        [Test]
        public void PPCrw_andis()
        {
            AssertCode(0x74320100, // andis
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r18 = r1 & 0x1000000<32>",
                "2|L--|cr0 = cond(r18)");
        }

        [Test]
        public void PPCRw_lswi()
        {
            //$TODO: test for LE; PPC docs say that is an invalid instruction
            // in little endian mode.
            Given_HexString("7CAC84AA");
            AssertCode(     // lswi	r5,r12,10
                "0|L--|00100000(4): 8 instructions",
                "1|L--|r5 = Mem0[r12:word32]",
                "2|L--|r12 = r12 + 4<i32>",
                "3|L--|r6 = Mem0[r12:word32]",
                "4|L--|r12 = r12 + 4<i32>",
                "5|L--|r7 = Mem0[r12:word32]",
                "6|L--|r12 = r12 + 4<i32>",
                "7|L--|r8 = Mem0[r12:word32]",
                "8|L--|r12 = r12 + 4<i32>");
        }

        [Test]
        public void PPCRW_lwzu()
        {
            RunTest((m) =>
            {
                m.Lwzu(m.r2, 4, m.r1);
            });
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2 = Mem0[r1 + 4<i32>:word32]",      //$LIT should be 4<i32>
                "2|L--|r1 = r1 + 4<i32>"
                );
        }

        [Test]
        public void PPCRW_lwz_r0()
        {
            RunTest((m) =>
            {
                m.Lwz(m.r2, -4, m.r0);
            });
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = Mem0[0xFFFFFFFC<32>:word32]"
                );
        }

        [Test]
        public void PPCRw_lxvd2x()
        {
            Given_PowerPcLe64();
            Given_HexString("98360A7C");
            AssertCode(     // lxvd2x	v0,r10,r6
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = Mem0[r10 + r6:word128]");
        }

        [Test]
        public void PPCRw_maddhd()
        {
            Given_PowerPcBe64();
            Given_HexString("1349E370");
            AssertCode(     // maddhd	r26,r9,r28,r13
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v7 = CONVERT(r9, int64, int128) *s CONVERT(r28, int64, int128) + CONVERT(r13, int64, int128)",
                "2|L--|r26 = SLICE(v7, int64, 64)");
        }

        [Test]
        public void PPCRw_maddhdu()
        {
            Given_PowerPcBe64();
            Given_HexString("13B6CAB1");
            AssertCode(     // maddhdu	r29,r22,r25,r10
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v7 = CONVERT(r22, uint64, uint128) *u CONVERT(r25, uint64, uint128) + CONVERT(r10, uint64, uint128)",
                "2|L--|r29 = SLICE(v7, uint64, 64)");
        }

        [Test]
        public void PPCRw_maddld()
        {
            Given_PowerPcBe64();
            Given_HexString("101BD033");
            AssertCode(     // maddld	r0,r27,r26,r0
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = CONVERT(r27, int64, int128) *s CONVERT(r26, int64, int128) + CONVERT(r0, int64, int128)",
                "2|L--|r0 = SLICE(v6, int64, 0)");
        }

        [Test]
        public void PPCRw_mflr()
        {
            Given_UInt32s(0x7C0802A6);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = lr");
        }

        [Test]
        public void PPCRw_mfcr()
        {
            Given_UInt32s(0x7d800026);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r12 = cr");
        }

        [Test]
        public void PPCRw_mfocrf()
        {
            Given_HexString("7D702026");
            AssertCode(     // mfocrf	r11,02
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = __mfocrf<word32>(cr, 2<32>)");
        }

        [Test]
        public void PPCRw_srd()
        {
            AssertCode(0x7CC84436,   // srd	r8,r6,r8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = r6 >>u r8");
        }

        [Test]
        public void PPCRw_stq()
        {
            this.Given_PowerPcBe64();
            Given_HexString("F8F9FAFB");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r25 + -1288<i64>:word128] = r7_r8");
        }

        [Test]
        public void PPCRw_stswi()
        {
            Given_HexString("7CA345AA");
            AssertCode(     // stswi	r5,r3,08
                "0|L--|00100000(4): 4 instructions",
                "1|L--|Mem0[r3:word32] = r5",
                "2|L--|r3 = r3 + 4<i32>",
                "3|L--|Mem0[r3:word32] = r6",
                "4|L--|r3 = r3 + 4<i32>");
        }

        [Test]
        public void PPCRw_stwu()
        {
            AssertCode(0x9521016e, // "stwu\tr9,r1,r0");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r1 + 366<i32>:word32] = r9",
                "2|L--|r1 = r1 + 366<i32>");
        }

        [Test]
        public void PPCRw_stwux()
        {
            AssertCode(0x7d21016e, // "stwux\tr9,r1,r0");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r1 + r0:word32] = r9",
                "2|L--|r1 = r1 + r0");
        }

        [Test]
        public void PPCRw_stwx()
        {
            AssertCode(0x7c95012e, // "stwx\tr4,r21,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r21 + r0:word32] = r4");
        }

        [Test]
        public void PPCRw_stwcix()
        {
            AssertCode(0x7D722F2A,  // stwcix
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__stwcix<word32>(&Mem0[r18 + r5:word32], r11)");
        }

        [Test]
        public void PPCRw_stwx_64()
        {
            Given_PowerPcBe64();
            AssertCode(0x7c95012e, // "stwx\tr4,r21,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r21 + r0:word32] = SLICE(r4, word32, 0)");
        }

        [Test]
        public void PPCRw_subf()
        {
            AssertCode(0x7c154850, // "subf\tr0,r21,r9");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r9 - r21");
        }

        [Test]
        public void PPCRw_subfco()
        {
            AssertCode(0x7C0A5C11, //subfco
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r0 = r11 - r10",
                "2|L--|cr0 = cond(r0)",
                "3|L--|xer = cond(r0)");
        }

        [Test]
        public void PPCRw_subfzeo()
        {
            Given_HexString("7DDC8591");
            AssertCode(     // subfzeo.	r14,r28
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r14 = 0<32> - r28 + xer",
                "2|L--|cr0 = cond(r14)");
        }


        [Test]
        public void PPCRw_srawi()
        {
            AssertCode(0x7c002670, //"srawi\tr0,r0,04");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 >> 4<32>");
        }


        [Test]
        public void PPCRw_fmr()
        {
            AssertCode(0xFFE00890, // "fmr\tf31,f1");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f31 = f1");
        }

        [Test]
        public void PPCRw_mtctr()
        {
            AssertCode(0x7d0903a6, // "mtctr\tr8");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ctr = r8");
        }

        [Test]
        public void PPCRw_cmpl()
        {
            AssertCode(0x7f904840, //"cmplw\tcr7,r16,r9");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(r16 - r9)");
        }

        [Test]
        public void PPCRw_neg()
        {
            AssertCode(0x7c0000d0, // "neg\tr0,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = -r0");
        }

        [Test]
        public void PPCRw_cntlzw()
        {
            AssertCode(0x7d4a0034, //"cntlzw\tr10,r10");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = __count_leading_zeros<word32>(r10)");
        }

        [Test]
        public void PPCRw_fsub()
        {
            AssertCode(0xfc21f828, // "fsub\tf1,f1,f31");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f1 = f1 - f31");
        }

        [Test]
        public void PPCRw_li()
        {
            AssertCode(0x38000000,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = 0<32>");
        }

        [Test]
        public void PPCRw_addze()
        {
            AssertCode(0x7c000195,// "addze\tr0,r0");
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r0 = r0 + xer",
                "2|L--|cr0 = cond(r0)",
                "3|L--|xer = cond(r0)");
        }

        [Test]
        public void PPCRw_slw()
        {
            AssertCode(0x7d400030, //"slw\tr0,r10,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r10 << r0");
        }

        [Test]
        public void PPCRw_fctiw()
        {
            AssertCode(0xFC00081C,   // fctiw	f0,f1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = f1",
                "2|L--|f0 = __fctiw(v5)");
        }

        [Test]
        public void PPCRw_fctiwz()
        {
            AssertCode(0xfc00081E, //"fctiwz\tf0,f1");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = CONVERT(f1, real64, int32)");
        }

        [Test]
        public void PPCRw_fmul()
        {
            AssertCode(0xfc010032,
               "0|L--|00100000(4): 1 instructions",
               "1|L--|f0 = f1 * f0");
        }

        [Test]
        public void PPCRw_fmul_()
        {
            AssertCode(0xfc010033,
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f0 = f1 * f0",
                "2|L--|cr1 = cond(f0)");
        }

        [Test]
        public void PPCRw_fcmpu()
        {
            AssertCode(0xff810000, // , "fcmpu\tcr7,f1,f0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(f1 - f0)");
        }

        [Test]
        public void PPCRw_mtcrf()
        {
            AssertCode(0x7d808120, //"mtcrf\t08,r12");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__mtcrf<word32>(8<32>, r12)");
        }

        [Test]
        public void PPCRw_bcds()
        {
            Given_HexString("1143AEC1");
            AssertCode(     // bcds.	v10,v3,v21
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = __bcd_shift(v3, v21)");
        }

        [Test]
        public void PPCRw_bcdtrunc()
        {
            Given_HexString("11544F01");
            AssertCode(     // bcdtrunc.	v10,v20,v9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = __bcd_truncate(v20, v9)");
        }

        [Test]
        public void PPCRw_bcdus()
        {
            Given_HexString("135C9681");
            AssertCode(     // bcdus.	v26,v28,v18
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v26 = __bcd_unsigned_shift(v28, v18)");
        }

        [Test]
        public void PPCRw_bctr()
        {
            AssertCode(0x4e800420, //"bcctr\t14,00");
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto ctr");
        }

        [Test]
        public void PPCRw_bctrl()
        {
            AssertCode(0x4e800421, // "bctrl\t14,00");
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call ctr (0)");
        }

        [Test]
        public void PPCRw_bdzt()
        {
            Given_750();
            AssertCode(0x41700000,   // bdzt	cr4+lt,$803CDB28
                "0|T--|00100000(4): 2 instructions",
                "1|L--|ctr = ctr - 1<32>",
                "2|T--|if (ctr == 0<32> && Test(LT,cr4)) branch 00100000");
        }

        [Test]
        public void PPCRw_bdzfl()
        {
            Given_750();
            AssertCode(0x40490FDB,   // bdzfl	cr2+gt,$00000FD8
                "0|T--|00100000(4): 3 instructions",
                "1|L--|ctr = ctr - 1<32>",
                "2|T--|if (ctr != 0<32> || Test(GT,cr2)) branch 00100004",
                "3|T--|call 00000FD8 (0)");
        }

        [Test]
        public void PPCRw_bdnzfl()
        {
            Given_HexString("401D26CF");
            AssertCode(     // bdnzfl	cr7+gt,$000026CC
                "0|T--|00100000(4): 3 instructions",
                "1|L--|ctr = ctr - 1<32>",
                "2|T--|if (ctr == 0<32> || Test(GT,cr7)) branch 00100004",
                "3|T--|call 000026CC (0)");
        }

        [Test]
        public void PPCRw_bdnztl()
        {
            Given_750();
            AssertCode(0x412F6C6F,   // bdnztl	cr3+so,$00006C6C
                "0|T--|00100000(4): 3 instructions",
                "1|L--|ctr = ctr - 1<32>",
                "2|T--|if (ctr == 0<32> || Test(NO,cr3)) branch 00100004",
                "3|T--|call 00006C6C (0)");
        }
        [Test]
        public void PPCRw_bdztl()
        {
            Given_750();
            AssertCode(0x41747461,   // bdztl	cr5+lt,$80273FB0
                "0|T--|00100000(4): 3 instructions",
                "1|L--|ctr = ctr - 1<32>",
                "2|T--|if (ctr != 0<32> || Test(GE,cr5)) branch 00100004",
                "3|T--|call 00107460 (0)");
        }


        [Test]
        public void PPCRw_bnelr()
        {
            AssertCode(0x4c820020, // bnelr	
                "0|R--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,cr0)) branch 00100004",
                "2|R--|return (0,0)");
        }

        [Test]
        public void PPCRw_bl()
        {
            AssertCode(0x48000045,
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 00100044 (0)");
        }

        [Test]
        public void PPCRw_bltlr()
        {
            AssertCode(0x4D980020,   // bltlr	cr6
                "0|R--|00100000(4): 2 instructions",
                "1|T--|if (Test(GE,cr6)) branch 00100004",
                "2|R--|return (0,0)");
        }

        [Test]
        public void PPCRw_bsolr()
        {
            AssertCode(0x4D830020,   // bsolr	cr0
                "0|R--|00100000(4): 2 instructions",
                "1|T--|if (Test(NO,cr0)) branch 00100004",
                "2|R--|return (0,0)");
        }

        [Test]
        public void PPCRw_cmpwi()
        {
            AssertCode(0x2f830005, // 	cmpwi   cr7,r3,5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(r3 - 5<32>)");
        }

        [Test]
        public void PPCRw_bcXX()
        {
            //   AssertCode(0x40bc011c, "bge\tcr7,$0010011C");
            //    AssertCode(0x40bd011c, "ble\tcr7,$0010011C");
            AssertCode(0x40be011c, // "bne\tcr7,$0010011C");
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,cr7)) branch 0010011C");
            //    AssertCode(0x40bf011c, "bns\tcr7,$0010011C");
            //    AssertCode(0x41bc011c, "blt\tcr7,$0010011C");
            //    AssertCode(0x41bd011c, "bgt\tcr7,$0010011C");
            //    AssertCode(0x41be011c, "beq\tcr7,$0010011C");
            //    AssertCode(0x41bf011c, "bso\tcr7,$0010011C");
            //}
        }

        [Test]
        public void PPCRw_blr()
        {
            AssertCode(0x4E800020, // blr
                "0|R--|00100000(4): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void PPCRw_lbz()
        {
            AssertCode(0x8809002a,	//lbz     r0,42(r9)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = CONVERT(Mem0[r9 + 42<i32>:byte], byte, word32)");
        }

        [Test]
        public void PPCRw_crandc()
        {
            Given_HexString("4C428903");
            AssertCode(     // crandc	02,02,11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__crandc(2<8>, 2<8>, 0x11<8>)");
        }

        [Test]
        public void PPCRw_crnand()
        {
            Given_HexString("4CC601C2");
            AssertCode(     // crnand	06,06,00
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__crnand(6<8>, 6<8>, 0<8>)");
        }

        [Test]
        public void PPCRw_crorc()
        {
            Given_HexString("4C40C342");
            AssertCode(     // crorc	02,00,18
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__crorc(2<8>, 0<8>, 0x18<8>)");
        }

        [Test]
        public void PPCRw_crxor()
        {
            AssertCode(0x4cc63182, // crclr   4*cr1+eq	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__crxor(6<8>, 6<8>, 6<8>)");
        }

        [Test]
        public void PPCRw_not()
        {
            AssertCode(0x7c6318f8, // not     r3,r3	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = ~r3");
        }

        [Test]
        public void PPCRw_xor_()
        {
            AssertCode(0x7d290279, // xor.    r9,r9,r0	
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r9 = r9 ^ r0",
                "2|L--|cr0 = cond(r9)");
        }

        [Test]
        public void PPCRw_ori()
        {
            AssertCode(0x60000020, // ori     r0,r0,32	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 | 0x20<32>");
        }

        [Test]
        public void PPCRw_ori_highbitset()
        {
            Given_PowerPcBe64();
            AssertCode(0x60008020, // ori     r0,r0,0x8020<16>	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 | 0x8020<64>");
        }

        [Test]
        public void PPCRw_oris_64()
        {
            Given_PowerPcBe64();
            RunTest((m) =>
            {
                m.Oris(m.r4, m.r0, 0x1234);
            });
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r0 | 0x12340000<64>");
        }

        [Test]
        public void PPCRw_rlwimi()
        {
            AssertCode(0x5120f042, // "rlwimi\tr0,r9,1E,01,01");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __rlwimi<word32>(r9, 0x1E<8>, 1<8>, 1<8>)");
        }

        [Test]
        public void PPCRw_rlwinm_1_31_31()
        {
            AssertCode(0x54630ffe, // rlwinm r3,r3,1,31,31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 >>u 0x1F<8>");
        }

        [Test]
        public void PPCRw_regression_1()
        {
            AssertCode(0xfdad0024, // fdiv    f13,f13,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f13 = f13 / f0");

            AssertCode(0x38a0ffff, // li      r5,-1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = 0xFFFFFFFF<32>");

            AssertCode(0x575a1838, // rlwinm  r26,r26,3,0,28 
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r26 = r26 << 3<8>");

            AssertCode(0x7c03db96, // divwu   r0,r3,r27
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r3 /u r27");

            AssertCode(0x7fe0d9d6, // mullw   r31,r0,r27
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r31 = r0 * r27");

            AssertCode(0x6fde8000, // xoris   r30,r30,32768
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = r30 ^ 0x80000000<32>");

            AssertCode(0x7f891800, // cmpw    cr7,r9,r3	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(r9 - r3)");

            AssertCode(0xdbe10038, // stfd    f31,56(r1)	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 56<i32>:real64] = f31");

            AssertCode(0xc00b821c, // lfs     f0,-32228(r11)	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = CONVERT(Mem0[r11 + -32228<i32>:real32], real32, real64)");

            AssertCode(0xc8098220, // lfd     f0,-32224(r9)	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = Mem0[r9 + -32224<i32>:real64]");

            AssertCode(0x1f9c008c, // mulli   r28,r28,140	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = r28 * 0x8C<32>");

            AssertCode(0x7c1ed9ae, // stbx    r0,r30,r27	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r30 + r27:byte] = SLICE(r0, byte, 0)");

            AssertCode(0xa001001c, // lhz     r0,28(r1)	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = CONVERT(Mem0[r1 + 28<i32>:word16], word16, word32)");

            AssertCode(0x409c0ff0, // bge-   cr7,0x00001004<32>	
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GE,cr7)) branch 00100FF0");

            AssertCode(0x2b8300ff, // cmplwi  cr7,r3,255	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(r3 - 0xFF<32>)");
        }

        [Test]
        public void PPCRw_lbzu()
        {
            AssertCode(0x8D010004, // lbzu
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r8 = CONVERT(Mem0[r1 + 4<i32>:byte], byte, word32)",
                "2|L--|r1 = r1 + 4<i32>");
        }

        [Test]
        public void PPCRw_sth()
        {
            AssertCode(0xB0920004u, // sth
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r18 + 4<i32>:word16] = SLICE(r4, word16, 0)"
                );
        }

        [Test]
        public void PPCRw_sthbrx()
        {
            Given_HexString("7D601F2C");
            AssertCode(     // sthbrx	r0,r11,r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = SLICE(r0, word16, 0)",
                "2|L--|Mem0[r11 + r3:word16] = __reverse_bytes<word16>(v6)");
        }


        [Test]
        public void PPCrw_subfic()
        {
            AssertCode(0x20320100, // subfic
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = 0x100<32> - r18");
        }

        [Test]
        public void PPCrw_fneg_()
        {
            AssertCode(0xfc200051, // "fneg\tf1,f0");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f1 = -f0",
                "2|L--|cr1 = cond(f1)");
        }

        [Test]
        public void PPCrw_fmadd()
        {
            AssertCode(0xfc0062fa, // "fmadd\tf0,f0,f11,f12");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f0 * f11 + f12");
        }

        [Test]
        public void PPCrw_creqv()
        {
            AssertCode(0x4cc63242, // "creqv\t06,06,06");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__creqv(6<8>, 6<8>, 6<8>)");
        }
        //AssertCode(0x4e080000, "mcrf\tcr4,cr2");

        [Test]
        public void PPCrw_srw()
        {
            AssertCode(0x7c684430, // "srw\tr8,r3,r8");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = r3 >>u r8");
        }

        [Test]
        public void PPCrw_subfc()
        {
            AssertCode(0x7cd9a810, //"subfc\tr6,r25,r21");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r6 = r21 - r25",
                "2|L--|xer = cond(r6)");
        }

        [Test]
        public void PPCRw_dcbtst()
        {
            AssertCode(0x7c0019ec,     // dcbtst\tr0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dcbtst<word32>(r3)");
        }

        [Test]
        public void PPCRw_dcbz()
        {
            AssertCode(0x7C23F7EC,     // dcbz\tr3,r30
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dcbz<word32>(r3 + r30)");
        }

        [Test]
        public void PPCrw_divw()
        {
            AssertCode(0x7d3d03d6, //"divw\tr9,r29,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r29 / r0");
        }

        [Test]
        public void PPCRw_dmulq()
        {
            Given_PowerPcBe64();
            Given_HexString("FFDEF044");
            AssertCode(     // dmulq	f30,f30,f30
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f30_f31 = f30_f31 * f30_f31");
        }

        [Test]
        public void PPCRw_eieio()
        {
            AssertCode(0x7C0006AC,   // eieio
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__eieio()");
        }


        [Test]
        public void PPCRw_eqv()
        {
            AssertCode(0x7D6B5238,     // eqv\tr11,r11,r10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = ~(r11 ^ r10)");
        }


        [Test]
        public void PPCrw_mulhw_()
        {
            AssertCode(0x7ce03897, //"mulhw.\tr7,r0,r7");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r7 = r0 * r7 >> 0x20<32>",
                "2|L--|cr0 = cond(r7)");
        }

        [Test]
        public void PPCrw_subfze()
        {
            AssertCode(0x7fde0190, // "subfze\tr30,r30");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = 0<32> - r30 + xer");
        }


        [Test]
        public void PPCrw_subfe()
        {
            AssertCode(0x7c631910, // "subfe\tr3,r3,r3");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 - r3 + xer");
        }

        [Test]
        [Ignore("reenable when we have switching between PPC models implemented")]
        public void PPCRw_evmhessfaaw()
        {
            AssertCode(0x11A91D03,     // evmhessfaaw\tr13,r9,r3
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v6 = r9",
                "2|L--|v7 = r3",
                "3|L--|r13 = __evmhessfaaw(v6, v7)",
                "4|L--|acc = r13");
        }


        [Test]
        public void PPCrw_extsb()
        {
            AssertCode(0x7c000775, //"extsb.\tr0,r0");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = CONVERT(SLICE(r0, int8, 0), int8, int32)",
                "2|L--|cr0 = cond(r0)");
        }

        //AssertCode(0x7c00252c, "stwbrx\tr0,r0,r4");
        //AssertCode(0x7e601c2c, "lwbrx\tr19,r0,r3");

        [Test]
        public void PPCrw_fmul()
        {
            AssertCode(0xfdad02f2, //"fmul\tf13,f13,f11");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f13 = f13 * f11");
        }

        [Test]
        public void PPCrw_regression_2()
        {
            AssertCode(0x4e080000, // mcrf cr4,cr2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr4 = cr2");

            AssertCode(0x4e0c0000, // mcrf cr4,cr3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr4 = cr3");

            AssertCode(0x7ca35914, // adde r5,r3,r11
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r5 = r3 + r11 + xer",
                "2|L--|xer = cond(r5)");

            AssertCode(0x7e601c2c, // lwbrx r19,0,r3
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|r19 = __reverse_bytes<word32>(Mem0[r3:word32])");
            AssertCode(0x7c00252c, // stwbrx r0,0,r4
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|v5 = SLICE(r0, word32, 0)",
                 "2|L--|Mem0[r4:word32] = __reverse_bytes<word32>(v5)");
        }

        [Test]
        public void PPCrw_sthx()
        {
            AssertCode(0x7c1b1b2e, //	sthx    r0,r27,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r27 + r3:word16] = SLICE(r0, word16, 0)");
        }

        [Test]
        public void PPCrw_nand()
        {
            AssertCode(0x7d6043b8, //	nand    r0,r11,r8	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = ~(r11 & r8)");
        }

        [Test]
        public void PPCrw_orc()
        {
            AssertCode(0x7d105b38, // orc     r16,r8,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = r8 | ~r11");
        }

        [Test]
        public void PPCrw_bclr_getPC()
        {
            AssertCode(0x429f0005, // bcl-     20,cr7,xxxx
                "0|L--|00100000(4): 1 instructions",
                "1|L--|lr = 00100004");
        }

        [Test]
        public void PPCrw_std()
        {
            Given_PowerPcBe64();
            AssertCode(0xf8410028, // "std\tr2,40(r1)");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 40<i64>:word64] = r2");
        }

        [Test]
        public void PPCrw_stdu()
        {
            Given_PowerPcBe64();
            AssertCode(0xf8410029, //	stdu    r2,40(r1))"
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r1 + 40<i64>:word64] = r2",
                "2|L--|r1 = r1 + 40<i64>");
        }

        [Test]
        public void PPCRw_rldcl()
        {
            Given_PowerPcBe64();
            Given_HexString("7A312551");
            AssertCode(     // rldcl	r17,r17,04,15
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = __rol<word64,byte>(r17, SLICE(4<64>, byte, 0))",
                "2|L--|r17 = v4 & 0x7FFFFFFFFFF<64>");
        }

        [Test]
        public void PPCRw_rldcr()
        {
            Given_PowerPcBe64();
            Given_HexString("7AD3A112");
            AssertCode(     // rldcr	r19,r22,34,04
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = __rol<word64,byte>(r22, SLICE(0x34<64>, byte, 0))",
                "2|L--|r19 = v5 & 0x7800000000000000<64>");
        }

        [Test]
        public void PPCRw_rldic()
        {
            Given_PowerPcBe64();
            Given_HexString("7864D46A");
            AssertCode(     // rldic	r4,r3,3A,31
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = __rol<word64,byte>(r3, 0x3A<8>)",
                "2|L--|r4 = v5 & 0x7FFF<64>");
        }

        [Test]
        public void PPCRw_rlwinm()
        {
            AssertCode(0x57200036, // "rlwinm\tr0,r25,04,00,1B");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r25 & 0xFFFFFFF0<u32>");
            AssertCode(0x5720EEFA, //,rlwinm	r9,r31,1D,1B,1D not handled yet.
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r25 >>u 3<8> & 0x1C<32>");
            AssertCode(0x5720421E, //  rlwinm	r0,r25,08,08,0F not handled yet.
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r25 << 8<8> & 0xFF0000<32>");
            AssertCode(0x57897c20, // rlwinm  r9,r28,15,16,16	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r28 << 0xF<8> & 0x8000<32>");
            AssertCode(0x556A06F7, // rlwinm.\tr10,r11,00,1B,1B
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r10 = r11 & 0x10<u32>",
                "2|L--|cr0 = cond(r10)");
        }

        [Test]
        public void PPCRw_rldicl()
        {
            Given_PowerPcBe64();
            AssertCode(0x790407c0,    // clrldi  r4,r8,31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r8 & 0x1FFFFFFFF<64>");
            AssertCode(0x79040020,    // clrldi  r4,r8,63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r8 & 0xFFFFFFFF<64>");
            AssertCode(0x78840fe2, // rldicl  r4,r4,33,63	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r4 << 0x21<8> & 1<64>");
            AssertCode(0x7863AB02,   // rldicl	r3,r3,35,0C
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = CONVERT(SLICE(r3, word52, 11), word52, word64)");
            AssertCode(0x7863e102, //"rldicl\tr3,r3,3C,04");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 >>u 4<8>");
            AssertCode(0x790407c0, //"rldicl\tr4,r8,00,1F");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r8 & 0x1FFFFFFFF<64>");
            AssertCode(0x790407E0, //"rldicl\tr4,r8,00,3F");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r8 & 1<64>");
            AssertCode(0x794A5840,     // rldicl   r10,r10,0B,01
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = __rol<word64,byte>(r10, 0xB<8>) & 0x7FFFFFFFFFFFFFFF<64>" );
        }

        [Test]
        public void PPCrw_rldicl_clearHighBits()
        {
            Given_PowerPcBe64();
            AssertCode(0x79290040,              // rldicl	r9,r9,00,01
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r9 & 0x7FFFFFFFFFFFFFFF<64>");
        }

        [Test]
        public void PPCRw_rldicr()
        {
            Given_PowerPcBe64();
            AssertCode(0x798C0F86, // rldicr\tr12,r12,33,30
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r12 = r12 << 0x21<8>");
        }

        [Test]
        public void PPCRw_rldicr_rotate()
        {
            Given_PowerPcBe64();
            AssertCode(0x798CCFE6,              // rldicr	r12,r12,39,3F
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r12 = __rol<word64,byte>(r12, 0x39<8>)");
        }

        [Test]
        public void PPCrw_fcfid()
        {
            AssertCode(0xff00069c,  // fcfid   f24,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f24 = CONVERT(f0, int64, real64)");
        }

        [Test]
        public void PPCrw_frsp()
        {
            AssertCode(0xfd600018, //"frsp\tf11,f0");
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|f11 = CONVERT(f0, real64, real32)");
        }

        [Test]
        public void PPCrw_fmadds()
        {
            AssertCode(0xec1f07ba, //"fmadds\tf0,f31,f30,f0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = CONVERT(SLICE(f31, real32, 0) * SLICE(f30, real32, 0) + SLICE(f0, real32, 0), real32, real64)");
        }

        [Test]
        public void PPCrw_fdivs()
        {
            AssertCode(0xec216824, //"fdivs\tf1,f1,f13");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f1 = f1 / f13");
        }

        [Test]
        public void PPCRw_lvsr()
        {
            Given_HexString("7C00404C");
            AssertCode(     // lvsr	v0,r0,r8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __lvsr(r8)");
        }

        [Test]
        public void PPCrw_lvx()
        {
            AssertCode(0x7c4048ce, //"lvx\tv2,r0,r9");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v2 = Mem0[r9:word128]");
        }

        [Test]
        public void PPCRw_lvxl()
        {
            Given_HexString("7DC95ACE");
            AssertCode(     // lvxl	r14,r9,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v14 = Mem0[r9 + r11:word128]");
        }

        [Test]
        public void PPCrw_beqlr()
        {
            AssertCode(0x4d9e0020, // beqlr\tcr7");
                "0|R--|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,cr7)) branch 00100004",
                "2|R--|return (0,0)");
        }

        [Test]
        public void PPCRw_vexptefp()
        {
            Given_HexString("1060598A");
            AssertCode(     // vexptefp	v3,v11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v3 = __vector_2_exp_estimate<real32[4]>(v11)");
        }

        [Test]
        public void PPCrw_Xenon_vspltw()
        {
            Given_Xenon();
            AssertCode(0x10601a8c, // vspltw\tv3,v3,00");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v3 = __vector_splat<word32[4]>(v3, 0<32>)");
        }

        [Test]
        public void PPCRw_vsbox()
        {
            Given_HexString("11BD0DC8");
            AssertCode(     // vsbox	v13,v29
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = __aes_subbytes(v29)");
        }

        [Test]
        public void PPCRw_vsel128()
        {
            Given_Xenon();
            Given_HexString("17E0635C");
            AssertCode(     // vsel128	v127,v0,v12,v127
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v127 = __vector_select(v0, v12, v127)");
        }

        [Test]
        public void PPCRw_vspltb()
        {
            Given_HexString("1000520C");
            AssertCode(     // vspltb	v0,v10,00
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_splat<byte[16]>(v10, 0<32>)");
        }

        [Test]
        public void PPCRw_vsplth()
        {
            Given_HexString("1000024C");
            AssertCode(     // vsplth	v0,v0,00
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_splat<word16[8]>(v0, 0<32>)");
        }

        [Test]
        public void PPCRw_vspltisb()
        {
            Given_HexString("1004030C");
            AssertCode(     // vspltisb	v0,+00000004
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_splat_imm<int8[16]>(4<32>)");
        }

        [Test]
        public void PPCRw_vspltish()
        {
            Given_HexString("1245AB4C");
            AssertCode(     // vspltish	v18,+00000005
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v18 = __vector_splat_imm<int16[8]>(5<32>)");
        }

        [Test]
        public void PPCrw_Xenon_vxor()
        {
            Given_Xenon();
            AssertCode(0x100004c4, ///vxor\tv0,v0,v0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = 0<128>");
            AssertCode(0x100404c4, ///vxor\tv0,v4,v0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = v4 ^ v0");
        }

        [Test]
        public void PPCrw_rlwnm()
        {
            AssertCode(0x5c00c03e, //"rlwnm\tr0,r0,r24,00,1F");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __rol<word32,word32>(r0, r24)");
        }

        [Test]
        public void PPCrw_blelr()
        {
            AssertCode(0x4c9d0020, //"blelr\tcr7");
                "0|R--|00100000(4): 2 instructions",
                "1|T--|if (Test(GT,cr7)) branch 00100004",
                "2|R--|return (0,0)");
        }

        [Test]
        public void PPCrw_dcbt()
        {
            AssertCode(0x7c00222c, //"dcbt\tr0,r4,E0");
                "0|L--|00100000(4): 0 instructions");
        }

        [Test]
        public void PPCrw_sync()
        {
            AssertCode(0x7c0004ac, // sync");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__sync()");
        }

        [Test]
        public void PPCrw_andc()
        {
            AssertCode(0x7c00f078, //andc\tr0,r0,r30");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 & ~r30");
        }

        [Test]
        public void PPCrw_sld()
        {
            AssertCode(0x7c005836, //sld\tr0,r0,r11");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 << r11");
        }

        [Test]
        public void PPCrw_sradi()
        {
            AssertCode(0x7c0bfe76, //sradi\tr11,r0,3F");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = r0 >> 0x3F<32>");
        }

        [Test]
        public void PPCrw_mulldt()
        {
            AssertCode(0x7c0a31d2, //mulld\tr0,r10,r6");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r10 * r6");
        }

        [Test]
        public void PPCrw_stdx()
        {
            Given_PowerPcBe64();
            AssertCode(0x7c07492a, //stdx\tr0,r7,r9");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r7 + r9:word64] = r0");
        }

        [Test]
        public void PPCRw_regression_3()
        {
            AssertCode(0x4200fff8, // bdnz+   0xfffffffffffffff8<64>	
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 1<32>",
                        "2|T--|if (ctr != 0<32>) branch 000FFFF8");
            AssertCode(0x7cc73378, // mr      r7,r6	
                        "0|L--|00100000(4): 1 instructions",
                        "1|L--|r7 = r6");
            AssertCode(0x7e2902a6, // mfctr   r17	
                        "0|L--|00100000(4): 1 instructions",
                        "1|L--|r17 = ctr");
        }

        [Test]
        public void PPCRw_bcctr()
        {
            AssertCode(0x4000fef8, //"bdnzf\tlt,$000FFEF8");
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 1<32>",
                        "2|T--|if (ctr != 0<32> && Test(GE,cr0)) branch 000FFEF8");
            AssertCode(0x4040fef8, //"bdzf\tlt,$000FFEF8");
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 1<32>",
                        "2|T--|if (ctr == 0<32> && Test(GE,cr0)) branch 000FFEF8");
            AssertCode(0x4080fef8, //"bge\t$000FFEF8");
                        "0|T--|00100000(4): 1 instructions",
                        "1|T--|if (Test(GE,cr0)) branch 000FFEF8");
            AssertCode(0x4100fef8, //"bdnzt\tlt,$000FFEF8");
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 1<32>",
                        "2|T--|if (ctr != 0<32> && Test(LT,cr0)) branch 000FFEF8");
            AssertCode(0x4180fef8, //"blt\t$000FFEF8");
                        "0|T--|00100000(4): 1 instructions",
                        "1|T--|if (Test(LT,cr0)) branch 000FFEF8");
            AssertCode(0x4200fef8, //"bdnz\t$000FFEF8");
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 1<32>",
                        "2|T--|if (ctr != 0<32>) branch 000FFEF8");
            AssertCode(0x4220fef9, //"bdnzl\t$000FFEF8");
                        "0|T--|00100000(4): 3 instructions",
                        "1|L--|ctr = ctr - 1<32>",
                        "2|T--|if (ctr == 0<32>) branch 00100004",
                        "3|T--|call 000FFEF8 (0)");
            AssertCode(0x4240fef8, //"bdz\t$000FFEF8");
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 1<32>",
                        "2|T--|if (ctr == 0<32>) branch 000FFEF8");
            AssertCode(0x4260fef9, //"bdzl\t$000FFEF8");
                        "0|T--|00100000(4): 3 instructions",
                        "1|L--|ctr = ctr - 1<32>",
                        "2|T--|if (ctr != 0<32>) branch 00100004",
                        "3|T--|call 000FFEF8 (0)");
            //AssertCode(0x4280fef8<32>//, "bc+    20,lt,0xffffffffffffff24<64>	 ");
            AssertCode(0x4300fef8, //"bdnz\t$000FFEF8");
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 1<32>",
                        "2|T--|if (ctr != 0<32>) branch 000FFEF8");
        }

        [Test]
        public void PPCRw_bcctrne()
        {
            AssertCode(0x4C820420,  // bcctr\t04,02
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,cr0)) branch 00100004",
                "2|T--|goto ctr");
        }

        [Test]
        public void PPCRw_rldimi_highword()
        {
            Given_PowerPcBe64();
            AssertCode(0x790A000E,  // rldimi r10,r8,20,00
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r10, word32, 0)",
                "2|L--|r10 = SEQ(SLICE(r8, word32, 0), v5)");
        }

        [Test]
        public void PPCRw_rldicr_00()
        {
            Given_PowerPcBe64();
            AssertCode(0x796B04E4,   // rldicr      r11,r11,00,33
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r11 = r11 & 0xFFFFFFFFFFFFF000<64>");
        }

        [Test]
        //[Ignore("These PPC masks are horrible")]
        public void PPCRw_rldimi_General()
        {
            Given_PowerPcBe64();
            AssertCode(0x78A3A04E,   // rldimi	r3,r5,34,01
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r3, word52, 0)",
                "2|L--|v6 = SLICE(r3, bool, 63)",
                "3|L--|r3 = SEQ(v6, SLICE(r5, word11, 0), v5)");
        }

        [Test]
        public void PPCRw_mftb()
        {
            AssertCode(0x7eac42e6, //"mftb\tr21,0188");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __mftb<word32>()");
        }

        [Test]
        public void PPCRw_stvehx()
        {
            Given_HexString("7C00394E");
            AssertCode(     // stvehx	v0,r0,r7
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__store_vector_element<word16 *,word16>(v0, r7)");
        }

        [Test]
        public void PPCRw_stvewx128()
        {
            Given_Xenon();
            Given_HexString("13E01987");
            AssertCode(     // stvewx128        v63,r0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__store_vector_element<word32 *,word32>(v63, r3)");
        }

        [Test]
        public void PPCRw_stvx()
        {
            AssertCode(0x7c2019ce, //"stvx\tv1,r0,r3");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r3:word128] = v1");
        }

        [Test]
        public void PPCRw_stfdp()
        {
            Given_HexString("F769F324");
            AssertCode(     // stfdp	f27,-3292(r9)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r9 + -3292<i32>:real64] = f27",
                "2|L--|Mem0[r9 + -3284<i32>:real64] = f28");
        }

        [Test]
        public void PPCRw_stfdpx()
        {
            Given_HexString("7DCBFF2E");
            AssertCode(     // stfdpx	f14,r11,r31
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r11 + r31:real64] = f14",
                "2|L--|Mem0[r11 + r31 + 8<i32>:real64] = f15");
        }

        [Test]
        public void PPCRw_stfiwx()
        {
            AssertCode(0x7c004fae, //"stfiwx\tf0,r0,r9");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r9:int32] = SLICE(f0, int32, 0)");
        }

        [Test]
        public void PPCrw_stfs()
        {
            AssertCode(0xd0010208, //stfs    f0,520(r1)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 520<i32>:real32] = CONVERT(f0, real64, real32)");
        }

        [Test]
        public void PPCRw_fmuls()
        {
            AssertCode(0xec000072, // fmuls   f0,f0,f1
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|f0 = f0 * f1");
        }

        [Test]
        public void PPCRw_lvlx()
        {
            AssertCode(0x7c6b040e, // .long 0x7c6b040e<32>	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v3 = __lvlx<word128>(r11, r0)");
        }

        [Test]
        public void PPCRw_Xenon_vspltw()
        {
            Given_Xenon();
            AssertCode(0x10601a8c, // vspltw  v3,v3,0	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v3 = __vector_splat<word32[4]>(v3, 0<32>)");
        }

        [Test]
        public void PPCRw_cntlzd()
        {
            Given_PowerPcBe64();
            AssertCode(0x7d600074, // cntlzd  r0,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __count_leading_zeros<word64>(r11)");
        }

        [Test]
        public void PPCRw_Xenon_vectorops()
        {
            Given_Xenon();
            AssertCode(0x10c6600a, //"vaddfp\tv6,v6,v12");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v6 = __simd_fadd<real32[4]>(v6, v12)");
            AssertCode(0x10000ac6, //"vcmpgtfp\tv0,v0,v1");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v0 = __vector_fp_cmpgt<real32[4]>(v0, v1)");
            AssertCode(0x118108c6, //"vcmpeqfp\tv12,v1,v1");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v12 = __vector_fp_cmpeq<real32[4]>(v1, v1)");
            AssertCode(0x10ed436e, //"vmaddfp\tv7,v13,v13,v8");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v7 = __vmaddfp<real32[4]>(v13, v13, v8)");
            AssertCode(0x10a9426e, //"vmaddfp\tv5,v9,v9,v8");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v5 = __vmaddfp<real32[4]>(v9, v9, v8)");
            AssertCode(0x10200a8c, //"vspltw\tv1,v1,0");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v1 = __vector_splat<word32[4]>(v1, 0<32>)");
            AssertCode(0x1160094a, //"vrsqrtefp\tv11,v1");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v11 = __vector_reciprocal_sqrt_estimate<real32[4]>(v1)");
            AssertCode(0x116b0b2a, //"vsel\tv11,v11,v1,v12");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v11 = __vector_select(v11, v1, v12)");
            AssertCode(0x1000002c, //"vsldoi\tv0,v0,v0,0");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v0 = __vector_shift_left_double(v0, v0, SLICE(0<32>, byte, 0))");
            AssertCode(0x101f038c, //"vspltisw\tv0,140");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v0 = __vector_splat_imm<int32[4]>(0xFFFFFFFF<32>)");
            //AssertCode(0x114948ab, //"vperm\tv10,v9,v9,v2");
            //              "0|L--|00100000(4): 1 instructions",
            //              "1|L--|v10 = __vperm(v9, v9, v3)");
            AssertCode(0x112c484a, //"vsubfp\tv9,v12,v9");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v9 = __simd_fsub<real32[4]>(v12, v9)");
            AssertCode(0x118000c6, //"vcmpeqfp\tv12,v0,v0");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v12 = __vector_fp_cmpeq<real32[4]>(v0, v0)");
            AssertCode(0x11ad498c, //"vmrglw\tv13,v13,v9");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v13 = __vector_merge_low<word32[4]>(v13, v9)");
            AssertCode(0x118c088c, //"vmrghw\tv12,v12,v1");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v12 = __vector_merge_high<word32[4]>(v12, v1)");
            AssertCode(0x125264c4, //"vxor\tv18,v18,v12");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v18 = v18 ^ v12");
        }

        [Test]
        public void PPCRw_regression4()
        {
            //Given_Xenon();
            AssertCode(0xec0c5038,//"fmsubs\tf0,f12,f0,f10");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = CONVERT(SLICE(f12, real32, 0) * SLICE(f0, real32, 0) - SLICE(f10, real32, 0), real32, real64)");
            AssertCode(0x7c20480c,//"lvsl\tv1,r0,r9");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v1 = __lvsl(r9)");
            AssertCode(0x1000fcc6,//"vcmpeqfp.\tv0,v0,v31");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v0 = __vector_fp_cmpeq<real32[4]>(v0, v31)",
                "2|L--|cr6 = cond(v0)");
            AssertCode(0x10c63184,//"vslw\tv6,v6,v6");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v6 = __vector_shift_left<word32[4]>(v6, SLICE(v6, byte, 0))");
            AssertCode(0x7c01008e,//"lvewx\tv0,r1,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __load_vector_element<word32>(r1 + r0, v0)");
            AssertCode(0x10006e86, //"vcmpgtuw.\tv0,v0,v13");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v0 = __vector_cmpgt<uint32[4]>(v0, v13)",
                "2|L--|cr6 = cond(v0)");
            AssertCode(0x7c00418e, //"stvewx\tv0,r0,r8");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__store_vector_element<word32 *,word32>(v0, r8)");
            AssertCode(0x118063ca, //"vctsxs\tv12,v12,00");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v12 = __vector_cvt_fixedpt<int32[4],real32[4]>(v12, 0<32>)");
            AssertCode(0x1020634a, //"vcfsx\tv1,v12,00");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v1 = __vector_cvt_fixedpt_saturate<int32[4],real32[4]>(v12, 0<32>)");
            AssertCode(0x118c0404, //"vand\tv12,v12,v0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v12 = v12 & v0");
            AssertCode(0x118c0444, //"vandc\tv12,v12,v0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v12 = v12 & ~v0");
            AssertCode(0x116c5080, //"vadduwm\tv11,v12,v10");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v11 = __vector_add_modulo<uint32[4]>(v12, v10)");
            AssertCode(0x11083086, //"vcmpequw\tv8,v8,v6");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v8 = __vector_cmpeq<uint32[4]>(v8, v6)");
        }

        [Test]
        public void PPCRw_lfdp()
        {
            Given_HexString("E65F6FDC");
            AssertCode(     // lfdp	f18,28636(r31)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f18 = Mem0[r31 + 28636<i32>:real64]",
                "2|L--|f19 = Mem0[r31 + 28644<i32>:real64]");
        }

        [Test]
        public void PPCRw_lfdpx()
        {
            Given_HexString("7FA6B62F");
            AssertCode(     // lfdpx	f29,r6,r22
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f29 = Mem0[f29:real64]",
                "2|L--|f30 = Mem0[f29 + 8<i64>:real64]");
        }

        [Test]
        public void PPCRw_lfiwax()
        {
            Given_HexString("7D1876AE");
            AssertCode(     // lfiwax	f8,r24,r14
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f8 = CONVERT(Mem0[r24 + r14:int32], int32, int64)");
        }

        [Test]
        public void PPCrw_lfsx()
        {
            AssertCode(0x7c01042e,// "lfsx\tf0,r1,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = CONVERT(Mem0[r1 + r0:real32], real32, real64)");
        }

        [Test]
        public void PPCRw_mffs()
        {
            AssertCode(0xfc00048e, // "mffs\tf0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = fpscr");
        }

        [Test]
        public void PPCRw_mfvrd()
        {
            Given_PowerPcLe64();
            Given_HexString("6700087C");
            AssertCode(     // mfvrd	r8,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = SLICE(v0, word64, 64)");
        }

        [Test]
        public void PPCRw_mtfsb1()
        {
            Given_HexString("FE23484C");
            AssertCode(     // mtfsb1	18
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__mtfsb(0x18<32>, true)");
        }

        [Test]
        public void PPCrw_mtfsf()
        {
            AssertCode(0xfdfe058e, //"mtfsf\tFF,f0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__mtfsf<word64>(f0, 0xFF<8>)");
        }

        [Test]
        public void PPCRw_mtmsrd()
        {
            AssertCode(0x7DA10164,     // mtmsrd\tr13,01
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__write_msr<word64>(r13)");
        }

        [Test]
        public void PPCrw_lhaux()
        {
            AssertCode(0x7D2E4AEE,  //"lhaux\tr9,r14,r9");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r9 = CONVERT(Mem0[r14 + r9:int16], int16, int32)",
                "2|L--|r9 = r14 + r9");
        }

        [Test]
        public void PPCRw_lhzux()
        {
            AssertCode(0x7D69026E, // lhzux
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r11 = CONVERT(Mem0[r9 + r0:word16], word16, word32)",
                "2|L--|r9 = r9 + r0");
        }


        [Test]
        public void PPCrw_addme()
        {
            AssertCode(0x7D0301D4,  // "addme\tr8,r3,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = r3 + cr0 - 0xFFFFFFFF<32>");
        }

        [Test]
        public void PPCRw_lhau()
        {
            AssertCode(0xAD49FFFE, // lhau r10,-2(r9)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r10 = CONVERT(Mem0[r9 + -2<i32>:int16], int16, int32)",
                "2|L--|r10 = r9 + -2<i32>");
        }

        [Test]
        public void PPCRw_crnor()
        {
            AssertCode(0x4FDCE042, // crnor 1E,1C,1C
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__crnor(0x1E<8>, 0x1C<8>, 0x1C<8>)");
        }

        [Test]
        public void PPCRw_mtspr()
        {
            AssertCode(0x7C7A03A6, // mtspr 0000340, r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|srr0 = r3");
        }

        [Test]
        public void PPCRw_stmw()
        {
            AssertCode(0xBFC10008, // stmw r30,8(r1)
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v4 = r1 + 8<i32>",
                "2|L--|Mem0[v4:word32] = r30",
                "3|L--|v4 = v4 + 4<i32>",
                "4|L--|Mem0[v4:word32] = r31",
                "5|L--|v4 = v4 + 4<i32>");
        }

        [Test]
        public void PPCRw_mfmsr()
        {
            AssertCode(0x7C6000A6, // mfmsr r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = __read_msr<word32>()");
        }

        [Test]
        public void PPCRw_mtmsr()
        {
            AssertCode(0x7C600124, // mfmsr r3
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__write_msr<word32>(r3)");
        }

        [Test]
        public void PPCrw_rfi()
        {
            AssertCode(0x4C000024, //  rfi
                "0|R--|00100000(4): 2 instructions",
                "1|L--|__write_msr<word32>(srr1)",
                "2|T--|goto srr0");
        }

        [Test]
        public void PPCrw_bgtlr()
        {
            AssertCode(0x4D9D0020, // bgtlrcr7
                "0|R--|00100000(4): 2 instructions",
                "1|T--|if (Test(LE,cr7)) branch 00100004",
                "2|R--|return (0,0)");
        }

        [Test]
        public void PPCrw_lmw()
        {
            AssertCode(0xBBA1000C, // lmwr29,12(r1)
                "0|L--|00100000(4): 7 instructions",
                "1|L--|v4 = r1 + 12<i32>",
                "2|L--|r29 = Mem0[v4:word32]",
                "3|L--|v4 = v4 + 4<i32>",
                "4|L--|r30 = Mem0[v4:word32]",
                "5|L--|v4 = v4 + 4<i32>",
                "6|L--|r31 = Mem0[v4:word32]",
                "7|L--|v4 = v4 + 4<i32>");
        }

        [Test]
        public void PPCRw_mfspr()
        {
            AssertCode(0x7CB0E2A6, // mfspr 0000021C,r5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = __read_spr<word32>(0x390<32>)");
        }

        [Test]
        public void PPCRw_lhax()
        {
            AssertCode(0x7C84EAAE,  // lhax r4,r4,r29
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = CONVERT(Mem0[r4 + r29:int16], int16, int32)");
        }

        [Test]
        public void PPCRw_lq()
        {
            Given_PowerPcBe64();
            AssertCode(0xE0030000,   // lq	r0,0(r3)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0_r1 = Mem0[r3:word128]");
        }

        [Test]
        public void PPCRw_dcbf()
        {
            AssertCode(0x7C0018AC,   // dcbf	r0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dcbf<ptr32>(r3)");
        }

        [Test]
        public void PPCRw_dcbi()
        {
            AssertCode(0x7C001BAC,   // dcbi	r0,r3
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__dcbi<ptr32>(r3)");
        }

        [Test]
        public void PPCRw_dcbst()
        {
            AssertCode(0x7C00186C,   // dcbst\tr0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dcbst<ptr32>(r3)");
        }

        [Test]
        public void PPCRw_fabs()
        {
            AssertCode(0xFFE0FA10,   // fabs	f31,f31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f31 = fabs(f31)");
        }

        [Test]
        public void PPCRw_fmsub()
        {
            AssertCode(0xFC016038,   // fmsub	f0,f1,f0,f12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f1 * f0 - f12");
        }

        [Test]
        public void PPCRw_fnabs()
        {
            Given_HexString("FD3BB911");
            AssertCode(     // fnabs.	f9,f23
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f9 = -fabs(f23)",
                "2|L--|cr1 = cond(f9)");
        }

        [Test]
        public void PPCRw_icbi()
        {
            AssertCode(0x7C001FAC,   // icbi	r0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__icbi<ptr32>(r3)");
        }

        [Test]
        public void PPCRw_lfdx()
        {
            AssertCode(0x7C0904AE,   // lfdx	f0,r9,r0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = Mem0[r9 + r0:real64]");
        }

        [Test]
        public void PPCRw_lhbrx()
        {
            AssertCode(0x7C0B4E2C,   // lhbrx	r0,r11,r9
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r11 + r9:word16]",
                "2|L--|r0 = CONVERT(__swap16(v6), word16, word32)");
        }

        [Test]
        public void PPCRw_lswx()
        {
            Given_HexString("7D96C42B");
            AssertCode(     // lswx	r12,r22,r24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__lwsx<word32 *,word32>(r22 + r24, xer)");
        }

        [Test]
        public void PPCRw_stfsx()
        {
            AssertCode(0x7DABF52E,   // stfsx	f13,r11,r30
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + r30:real32] = CONVERT(f13, real64, real32)");
        }

        [Test]
        public void PPCRw_stfdx()
        {
            AssertCode(0x7C07ADAE,   // stfdx	f0,r7,r21
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r7 + r21:real64] = f0");
        }

        [Test]
        public void PPCRw_stfdu()
        {
            AssertCode(0xDC0B0010,   // stfdu	f0,16(r11)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r11 + 16<i32>:real64] = f0",
                "2|L--|r11 = r11 + 16<i32>");
        }

        [Test]
        public void PPCRw_lfdu()
        {
            AssertCode(0xCC0B0010,   // lfdu	f0,16(r11)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f0 = Mem0[r11 + 16<i32>:real64]",
                "2|L--|r11 = r11 + 16<i32>");
        }

        [Test]
        public void PPCRw_lfsu()
        {
            AssertCode(0xC43D0004,   // lfsu	f1,4(r29)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f1 = CONVERT(Mem0[r29 + 4<i32>:real32], real32, real64)",
                "2|L--|r29 = r29 + 4<i32>");
        }

        [Test]
        public void PPCRw_fctidz()
        {
            AssertCode(0xFD80665E,   // fctidz	f12,f12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f12 = trunc(f12)");
        }

        [Test]
        public void PPCRw_fnmsubs()
        {
            AssertCode(0xEDAC6AFC,   // fnmsubs	f13,f12,f13,f11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f13 = CONVERT(-(SLICE(f12, real32, 0) * SLICE(f13, real32, 0) - SLICE(f11, real32, 0)), real32, real64)");
        }

        [Test]
        public void PPCRw_fsel()
        {
            AssertCode(0xFC0A682E,   // fsel	f0,f10,f0,f13
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f10 >= 0.0 ? f0 : f13");
        }

        [Test]
        public void PPCRw_mtvsrd()
        {
            Given_PowerPcLe64();
            Given_HexString("6601097C");
            AssertCode(     // mtvsrd	v0,r9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = SEQ(r9, SLICE(v0, word64, 0))");
        }

        [Test]
        public void PPCRw_mtvsrws()
        {
            AssertCode(0x7D652327, // mtvsrws v11,r5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v43 = __mtvsrws<word128>(r5)");
        }


        [Test]
        public void PPCRw_twi()
        {
            AssertCode(0x0CCA0000,   // twi	06,r10,+0000
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r10 >u 0<32>) branch 00100004",
                "2|L--|__trap()");
        }

        [Test]
        public void PPCrw_lbzux()
        {
            AssertCode(0x7c1ee8ee, // "lbzux\tr0,r30,r29");
                    "0|L--|00100000(4): 2 instructions",
                    "1|L--|r0 = CONVERT(Mem0[r30 + r29:byte], byte, word32)",
                    "2|L--|r30 = r30 + r29");
        }

        [Test]
        public void PPCRw_ldarx()
        {
            Given_PowerPcBe64();
            AssertCode(0x7D0018A8,     // ldarx\tr8,r0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __larx<word64>(&Mem0[r3:word64])");
        }

        [Test]
        public void PPCRw_ldbrx()
        {
            Given_PowerPcLe64();
            Given_HexString("28BC407D");
            AssertCode(     // ldbrx	r10,r0,r23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = __reverse_bytes<word64>(Mem0[r23:word64])");
        }

        [Test]
        public void PPCRw_ldux()
        {
            Given_PowerPcBe64();
            Given_HexString("7EA0286B");
            AssertCode(     // ldux	r21,r0,r5
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r21 = Mem0[r0 + r5:word64]",
                "2|L--|r0 = r0 + r5");
        }

        [Test]
        public void PPCRw_ldx()
        {
            Given_PowerPcBe64();
            AssertCode(0x7C6BF82A,     // ldx\tr3,r11,r31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = Mem0[r11 + r31:word64]");
        }

        [Test]
        public void PPCRw_lwa()
        {
            this.Given_PowerPcBe64();
            Given_HexString("E8ABFFFA");    // lwa\tr5,-8(r11)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = CONVERT(Mem0[r11 + -8<i64>:word32], word32, int64)");
        }

        [Test]
        public void PPCRw_lwarx()
        {
            AssertCode(0x7D405828,   // lwarx	r10,r0,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = __larx<word32>(&Mem0[r11:word32])");
        }

        [Test]
        public void PPCRw_lwzx()
        {
            AssertCode(0x7c9c002e, // "lwzx\tr4,r28,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = Mem0[r28 + r0:word32]");
        }

        [Test]
        public void PPCRw_fctid()
        {
            AssertCode(0xFD606E5C,   // fctid	f11,f13
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f11 = round(f13)");
        }

        [Test]
        public void PPCRw_fnmsub()
        {
            AssertCode(0xFD6A037C,   // fnmsub	f11,f10,f13,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f11 = -(f10 * f13 - f0)");
        }

        [Test]
        public void PPCRw_bgelr()
        {
            AssertCode(0x4C980020,   // bgelr	cr6
                "0|R--|00100000(4): 2 instructions",
                "1|T--|if (Test(LT,cr6)) branch 00100004",
                "2|R--|return (0,0)");
        }

        [Test]
        public void PPCRw_fres()
        {
            Given_HexString("EFE5EDF0");
            AssertCode(     // fres	f31,f29
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = __fp_reciprocal_estimate<real32>(CONVERT(f29, real64, real32))",
                "2|L--|f31 = CONVERT(v5, real32, real64)");
        }

        [Test]
        public void PPCRw_fsqrt()
        {
            AssertCode(0xFC00002C,   // fsqrt	f0,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = sqrt(f0)");
        }

        [Test]
        public void PPCRw_fsqrts_()
        {
            Given_HexString("EDCB4EED");
            AssertCode(     // fsqrts.	f14,f9
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = sqrtf(CONVERT(f9, real64, real32))",
                "2|L--|f14 = CONVERT(v5, real32, real64)",
                "3|L--|cr1 = cond(v5)");
        }

        [Test]
        public void PPCRw_Xenon_lvewx128()
        {
            Given_Xenon();
            Given_HexString("1320D887");
            AssertCode(     // lvewx128 v57,r0,r27
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v57 = __load_vector_element<word32>(r27, v57)");
        }

        [Test]
        public void PPCRw_Xenon_lvx128()
        {
            Given_Xenon();
            AssertCode(0x13E058C7,     // vcmpequd\tv31,v0,v11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = Mem0[r11:word128]");
        }

        [Test]
        public void PPCRw_lwzux()
        {
            Given_PowerPcBe64();
            AssertCode(0x7CC9506E,   // lwzux	r6,r9,r10
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r6 = CONVERT(Mem0[r9 + r10:word32], word32, word64)",
                "2|L--|r9 = r9 + r10");
        }

        [Test]
        public void PPCRw_Xenon_vmr()
        {
            Given_Xenon();
            AssertCode(0x11400484,     // vor\tv10,v0,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = v0");
        }

        [Test]
        public void PPCRw_Xenon_vor()
        {
            Given_Xenon();
            AssertCode(0x11480484,     // vor\tv10,v0,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = v8 | v0");
        }

        [Test]
        public void PPCRw_stb()
        {
            AssertCode(0x98010018u, // "stb\tr0,1(r1)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 24<i32>:byte] = SLICE(r0, byte, 0)");
        }

        [Test]
        public void PPCRw_stbcx()
        {
            Given_HexString("7DFDE56C");
            AssertCode(     // stbcx.	r15,r29,r28
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr0 = __store_conditional<byte *,byte>(&Mem0[r29 + r28:byte], SLICE(r15, byte, 0))");
        }

        [Test]
        public void PPCRW_stbu()
        {
            RunTest((m) =>
            {
                m.Stbu(m.r2, 18, m.r3);
            });
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r3 + 18<i32>:byte] = SLICE(r2, byte, 0)",
                "2|L--|r3 = r3 + 18<i32>");
        }

        [Test]
        public void PPCRW_stbux()
        {
            RunTest((m) =>
            {
                m.Stbux(m.r2, m.r3, m.r0);
            });
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r3 + r0:byte] = SLICE(r2, byte, 0)",
                "2|L--|r3 = r3 + r0"
                );
        }

        [Test]
        public void PPCRw_stfsu()
        {
            AssertCode(0xD41C0010,   // stfsu	f0,16(r28)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r28 + 16<i32>:real32] = CONVERT(f0, real64, real32)",
                "2|L--|r28 = r28 + 16<i32>");
        }

        [Test]
        public void PPCRw_stvlx128()
        {
            Given_Xenon();
            AssertCode(0x1001350B,     // stvlx128\tv0,r1,r6
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + r6:word128] = v64");
        }

        [Test]
        public void PPCRw_stwcx_()
        {
            AssertCode(0x7D40592D,     // stwcx.\tr10,r0,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr0 = __store_conditional<word32 *,word32>(&Mem0[r11:word32], r10)");
        }

        [Test]
        public void PPCRw_stxsdx()
        {
            Given_PowerPcLe64();
            Given_HexString("994D007C");
            AssertCode(     // stxsdx	v32,r0,r9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r9:word64] = SLICE(v32, word64, 64)");
        }

        [Test]
        public void PPCRw_stxvd2x()
        {
            Given_PowerPcLe64();
            Given_HexString("9847097C");
            AssertCode(     // stxvd2x	v0,r9,r8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r9 + r8:word128] = v0");
        }

        [Test]
        public void PPCRw_stdcx_()
        {
            Given_PowerPcBe64();
            AssertCode(0x7D4019AD,     // stdcx.\tr10,r0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr0 = __store_conditional<word64 *,word64>(&Mem0[r3:word64], r10)");
        }

        [Test]
        public void PPCRw_stdux()
        {
            Given_PowerPcLe64();
            Given_HexString("6A49417D");
            AssertCode(     // stdux	r10,r1,r9
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r1 + r9:word64] = r10",
                "2|L--|r1 = r1 + r9");
        }

        [Test]
        public void PPCRw_tlbie()
        {
            AssertCode(0x7C004A64, // tlbie\tr9
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__tlbie<ptr32>(r9)");
        }

        [Test]
        public void PPCrw_tw()
        {
            AssertCode(0x7c201008, //"twlgt   r0,r2");
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r0 <=u r2) branch 00100004",
                "2|L--|__trap()");
            AssertCode(0x7c401008, //"twllt   r0,r2");
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r0 >=u r2) branch 00100004",
                "2|L--|__trap()");
            AssertCode(0x7c801008, //"tweq    r0,r2");
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r0 != r2) branch 00100004",
                "2|L--|__trap()");
            AssertCode(0x7d001008, //"twgt    r0,r2");
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r0 <= r2) branch 00100004",
                "2|L--|__trap()");
            AssertCode(0x7e001008, //"twlt    r0,r2");
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r0 >= r2) branch 00100004",
                "2|L--|__trap()");
        }

        [Test]
        public void PPCRw_lwax()
        {
            Given_PowerPcBe64();
            AssertCode(0x7C8B22AA,     // lwax\tr4,r11,r4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = CONVERT(Mem0[r11 + r4:int32], int32, int64)");
        }

        [Test]
        public void PPCRw_divdu()
        {
            AssertCode(0x7D6B5392,     // divdu\tr11,r11,r10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = r11 /u r10");
        }

        [Test]
        public void PPCRw_divd8()
        {
            AssertCode(0x7D2943D2,     // divd\tr9,r9,r8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r9 / r8");
        }

        [Test]
        public void PPCRw_lfsux()
        {
            AssertCode(0x7C0A5C6E,     // lfsux\tf0,r10,r11
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f0 = CONVERT(Mem0[r10 + r11:real32], real32, real64)",
                "2|L--|r10 = r10 + r11");
        }

        [Test]
        public void PPCRw_lfdux()
        {
            AssertCode(0x7DAA44EE,     // lfdux\tf13,r10,r8
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f13 = Mem0[r10 + r8:real64]",
                "2|L--|r10 = r10 + r8");
        }

        [Test]
        public void PPCRw_srad()
        {
            AssertCode(0x7D2B5E34,     // srad\tr11,r9,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = r9 >> r11");
        }

        [Test]
        public void PPCRw_tdi()
        {
            Given_PowerPcBe64();
            AssertCode(0x08C40000,   // tdi	06,r4,+0000
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r4 >u 0<64>) branch 00100004",
                "2|L--|__trap()");
        }

        [Test]
        public void PPCRw_vabsduw()
        {
            Given_HexString("12A71483");
            AssertCode(     // vabsduw	v21,v7,v2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v21 = __vector_abs_difference<uint32[4]>(v7, v2)");
        }

        [Test]
        public void PPCRw_vaddshs()
        {
            Given_HexString("114A4B40");
            AssertCode(     // vaddshs	v10,v10,v9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = __vector_add_saturate<int16[8]>(v10, v9)");
        }

        [Test]
        public void PPCRw_vaddsws()
        {
            Given_HexString("102C5380");
            AssertCode(     // vaddsws	v1,v12,v10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v1 = __vector_add_saturate<int32[4]>(v12, v10)");
        }

        [Test]
        public void PPCRw_vaddubm()
        {
            Given_Xenon();
            AssertCode(0x13040000,     // vaddubm\tv24,v4,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v24 = __vector_add_modulo<uint8[16]>(v4, v0)");
        }

        [Test]
        public void PPCRw_vadduhm()
        {
            Given_HexString("11095040");
            AssertCode(     // vadduhm	v8,v9,v10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v8 = __vector_add_modulo<uint8[16]>(v9, v10)");
        }

        [Test]
        public void PPCRw_vadduws()
        {
            Given_HexString("116C0280");
            AssertCode(     // vadduws	v11,v12,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v11 = __vector_add_saturate<uint32[4]>(v12, v0)");
        }

        [Test]
        public void PPCRw_vandc128()
        {
            Given_Xenon();
            Given_HexString("1698D275");
            AssertCode(     // vandc128	v52,v56,v58
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v52 = v56 & ~v58");
        }

        [Test]
        public void PPCRw_vavgsb()
        {
            Given_HexString("10E8FD02");
            AssertCode(     // vavgsb	v7,v8,v31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v7 = __vector_average<int8[16]>(v8, v31)");
        }

        [Test]
        public void PPCRw_vavgsh()
        {
            Given_HexString("11204D42");
            AssertCode(     // vavgsh	v9,v0,v9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v9 = __vector_average<int16[8]>(v0, v9)");
        }

        [Test]
        public void PPCRw_vavgub()
        {
            Given_HexString("100D0402");
            AssertCode(     // vavgub	v0,v13,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_average<uint8[16]>(v13, v0)");
        }

        [Test]
        public void PPCRw_vavguh()
        {
            Given_HexString("10215C42");
            AssertCode(     // vavguh	v1,v1,v11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v1 = __vector_average<uint16[8]>(v1, v11)");
        }

        [Test]
        public void PPCRw_Xenon_vmaxub()
        {
            Given_Xenon();
            AssertCode(0x10011002,     // vmaxub\tv0,v1,v2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_max<uint8[16]>(v1, v2)");
        }

        [Test]
        public void PPCRw_Xenon_vmladduhm()
        {
            Given_Xenon();
            AssertCode(0x10000022,     // vmladduhm\tv0,v0,v0,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vmladduhm<uint16[8]>(v0, v0)");
        }

        [Test]
        public void PPCRw_Xenon_vmaxuh()
        {
            Given_Xenon();
            AssertCode(0x10000042,     // vmaxuh\tv0,v0,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_max<uint16[8]>(v0, v0)");
        }

        [Test]
        public void PPCRw_vadduqm()
        {
            Given_Xenon();
            AssertCode(0x12020100,     // vadduqm\tv16,v2,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v16 = v2 + v0");
        }

        [Test]
        public void PPCRw_Xenon_vaddubs()
        {
            Given_Xenon();
            AssertCode(0x1003c200,     // vaddubs\tv0,v3,v24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_add_saturate<uint8[16]>(v3, v24)");
        }

        [Test]
        public void PPCRw_Xenon_vaddfp128()
        {
            Given_Xenon();
            Given_HexString("17FEF835");
            AssertCode(     // vaddfp128	v63,v62,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = __simd_fadd<real32[4]>(v62, v63)");
        }

        [Test]
        public void PPCRw_Xenon_bcdadd_()
        {
            Given_Xenon();
            AssertCode(0x10010401,     // bcdadd.\tv0,v1,v0,00
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __bcd_add(v1, v0)");
        }

        [Test]
        public void PPCRw_vcfux()
        {
            Given_HexString("10C03B0A");
            AssertCode(     // vcfux	v6,v7,00
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v6 = __vector_cvt_fixedpt_saturate<uint32[4],real32[4]>(v7, 0<32>)");
        }

        [Test]
        public void PPCRw_vcmpeqfp128()
        {
            Given_Xenon();
            AssertCode(0x187EF823,   // vcmpeqfp128	v3,v62,v127
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v3 = __vector_fp_cmpeq<real32[4]>(v62, v127)");
        }

        [Test]
        public void PPCRw_Xenon_vcmpequb()
        {
            Given_Xenon();
            AssertCode(0x117d9406,     // vcmpequb.\tv11,v29,v18
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v11 = __vector_cmpeq<uint8[16]>(v29, v18)",
                "2|L--|cr6 = cond(v11)");
        }

        [Test]
        public void PPCRw_vcmpequh()
        {
            Given_HexString("11806C46");
            AssertCode(     // vcmpequh.	v12,v0,v13
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v12 = __vector_cmpeq<word16[8]>(v0, v13)",
                "2|L--|cr6 = cond(v12)");
        }

        [Test]
        public void PPCRw_vcmpequw128()
        {
            Given_Xenon();
            Given_HexString("1BFFF265");
            AssertCode(     // vcmpequw128.	v63,v63,v62
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v63 = __vector_cmpeq<uint32[4]>(v63, v62)",
                "2|L--|cr6 = cond(v63)");
        }

        [Test]
        public void PPCRw_vcmpgefp()
        {
            Given_HexString("13C3F9C6");
            AssertCode(     // vcmpgefp	v30,v3,v31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v30 = __vector_fp_cmpge<real32[4]>(v3, v31)");
        }

        [Test]
        public void PPCRw_vcmpgefp128()
        {
            Given_Xenon();
            Given_HexString("1BBCE8A5");
            AssertCode(     // vcmpgefp128	v61,v60,v61
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v61 = __vector_fp_cmpge<real32[4]>(v60, v61)");
        }

        [Test]
        public void PPCRw_vcmpgtfp()
        {
            Given_Xenon();
            AssertCode(0x10000ac6,//"vcmpgtfp\tv0,v0,v1");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_fp_cmpgt<real32[4]>(v0, v1)");
        }

        [Test]
        public void PPCRw_vcmpgtsw()
        {
            Given_HexString("13F1FF86");
            AssertCode(     // vcmpgtsw.	v31,v17,v31
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v31 = __vector_cmpgt<int32[4]>(v17, v31)",
                "2|L--|cr6 = cond(v31)");
        }

        [Test]
        public void PPCRw_Xenon_vcmpgtfp128()
        {
            Given_Xenon();
            AssertCode(0x1ABBF925,   // vcmpgtfp128	v53,v59,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v53 = __vector_fp_cmpgt<real32[4]>(v59, v63)");
        }

        [Test]
        public void PPCRw_vcmpgtsb()
        {
            Given_HexString("1126D306");
            AssertCode(     // vcmpgtsb	v9,v6,v26
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v9 = __vector_cmpgt<int8[16]>(v6, v26)");
        }

        [Test]
        public void PPCRw_vcmpgtsh()
        {
            Given_HexString("121CE346");
            AssertCode(     // vcmpgtsh	v16,v28,v28
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v16 = __vector_cmpgt<int16[8]>(v28, v28)");
        }

        [Test]
        public void PPCRw_vcmpgtub()
        {
            Given_HexString("11806206");
            AssertCode(     // vcmpgtub	v12,v0,v12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v12 = __vector_cmpgt<uint8[16]>(v0, v12)");
        }

        [Test]
        public void PPCRw_vcmpgtuh()
        {
            Given_HexString("11406246");
            AssertCode(     // vcmpgtuh	v10,v0,v12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = __vector_cmpgt<uint16[8]>(v0, v12)");
        }

        [Test]
        public void PPCRw_vcmpnew()
        {
            Given_HexString("1320D887");
            AssertCode(     // vcmpnew	v25,v0,v27
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v25 = __vector_cmpne<uint32[4]>(v0, v27)");
        }

        [Test]
        public void PPCRw_vcmpnezh()
        {
            Given_HexString("13D39947");
            AssertCode(     // vcmpnezh	v30,v19,v19
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v30 = __vector_cmpne_or_0<word16[8]>(v19, v19)");
        }

        [Test]
        public void PPCRw_vcuxwfp128()
        {
            Given_Xenon();
            Given_HexString("1BE106D4");
            AssertCode(     // vcuxwfp128	v63,v0,01
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = __vcuxwfp<word128>(v0, 1<32>)");
        }

        [Test]
        public void PPCRw_vgbbd()
        {
            Given_HexString("12008D0C");
            AssertCode(     // vgbbd	v16,v17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v16 = __vector_gather_bits_bytes<uint32[4]>(v17)");
        }

        [Test]
        public void PPCRw_Xenon_stvrx128()
        {
            Given_Xenon();
            AssertCode(0x13E85D47,   // stvrx128	v63,r8,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r8 + r11:word128] = v63");
        }

        [Test]
        public void PPCRw_Xenon_lvlx128()
        {
            Given_Xenon();
            AssertCode(0x13A05C07,   // lvlx128	v61,r0,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v61 = __lvlx<word128>(r0, r11)");
        }

        [Test]
        public void PPCRw_Xenon_lvrx128()
        {
            Given_Xenon();
            AssertCode(0x13C55C47,   // lvrx128	v62,r5,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v62 = __lvrx<word128>(r5, r11)");
        }

        [Test]
        public void PPCRw_vmaddfp128()
        {
            Given_Xenon();
            Given_HexString("141EF8F3");
            AssertCode(     // vmaddfp128	v0,v62,v127,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vmaddfp<real32[4]>(v62, v127, v0)");
        }

        [Test]
        public void PPCRw_vmaxfp()
        {
            Given_HexString("110A040A");
            AssertCode(     // vmaxfp	v8,v10,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v8 = __vector_fp_max<real32[4]>(v10, v0)");
        }

        [Test]
        public void PPCRw_vmaxfp128()
        {
            Given_Xenon();
            AssertCode(0x1BDEEAA5,   // vmaxfp128	v62,v62,v61
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v62 = __vector_fp_max<real32[4]>(v62, v61)");
        }

        [Test]
        public void PPCRw_vmaxsb()
        {
            Given_HexString("1245F902");
            AssertCode(     // vmaxsb	v18,v5,v31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v18 = __vector_max<int8[16]>(v5, v31)");
        }

        [Test]
        public void PPCRw_vmaxsh()
        {
            Given_HexString("1159D142");
            AssertCode(     // vmaxsh	v10,v25,v26
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = __vector_max<int16[8]>(v25, v26)");
        }

        [Test]
        public void PPCRw_vmaxsw()
        {
            Given_HexString("109C0982");
            AssertCode(     // vmaxsw	v10,v28,v1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v4 = __vector_max<int32[4]>(v28, v1)");
        }

        [Test]
        public void PPCRw_vmhaddshs()
        {
            Given_HexString("123FDBA0");
            AssertCode(     // vmhaddshs	v17,v31,v27,v14
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v17 = __vector_mul_high_add_sat<int16[8]>(v31, v27, v14)");
        }

        [Test]
        public void PPCRw_vmhraddshs()
        {
            Given_HexString("106F06E1");
            AssertCode(     // vmhraddshs	v3,v15,v0,v27
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v3 = __vector_mul_high_round_add_sat<int16[8]>(v15, v0, v27)");
        }

        [Test]
        public void PPCRw_vminfp()
        {
            Given_HexString("12F83C4A");
            AssertCode(     // vminfp	v23,v24,v7
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v23 = __vector_fp_min<real32[4]>(v24, v7)");
        }

        [Test]
        public void PPCRw_vminfp128()
        {
            Given_Xenon();
            AssertCode(0x1BFFF2E5,   // vminfp128	v63,v63,v62
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = __vector_fp_min<real32[4]>(v63, v62)");
        }

        [Test]
        public void PPCRw_vminsh()
        {
            Given_HexString("118C5B42");
            AssertCode(     // vminsh	v12,v12,v11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v12 = __vector_min<int16[8]>(v12, v11)");
        }

        [Test]
        public void PPCRw_vminuw()
        {
            Given_HexString("11D0EA82");
            AssertCode(     // vminuw	v14,v16,v29
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v14 = __vector_min<uint32[4]>(v16, v29)");
        }

        [Test]
        public void PPCRw_vmsub4fp128()
        {
            Given_Xenon();
            AssertCode(0x157FA9F1,   // vmsub4fp128	v11,v63,v53
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v11 = __vmsub4fp<real32[4]>(v63, v53)");
        }

        [Test]
        public void PPCRw_vmsummbm()
        {
            Given_HexString("11DC8D65");
            AssertCode(     // vmsummbm	v14,v28,v17,v21
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v14 = __vector_mul_sum_mixed_modulo<int8[16],int32[4]>(v28, v17, v21)");
        }


        [Test]
        public void PPCRw_vmsumshm()
        {
            Given_HexString("10000128");
            AssertCode(     // vmsumshm	v0,v0,v0,v4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vmsumshm<int16[8]>(v0, v0, v4)");
        }

        [Test]
        public void PPCRw_vmsumshs()
        {
            Given_HexString("10F7B029");
            AssertCode(     // vmsumshs	v7,v23,v22,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v7 = __vector_mul_sum_sat<int16[8],int32[4]>(v23, v22, v0)");
        }

        [Test]
        public void PPCRw_vmsumubm()
        {
            Given_HexString("13C46CA4");
            AssertCode(     // vmsumubm	v30,v4,v13,v18
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v30 = __vector_mul_sum_modulo<uint8[16],int32[4]>(v4, v13, v18)");
        }

        [Test]
        public void PPCRw_vmsumuhm()
        {
            Given_HexString("128EF5A6");
            AssertCode(     // vmsumuhm	v20,v14,v30,v22
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v20 = __vector_mul_sum_modulo<uint16[8],int32[4]>(v14, v30, v22)");
        }

        [Test]
        public void PPCRw_vmsumuhs()
        {
            Given_HexString("128EF127");
            AssertCode(     // vmsumuhs	v20,v14,v30,v4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v20 = __vector_mul_sum_sat<uint16[8],int32[4]>(v14, v30, v4)");
        }

        [Test]
        public void PPCRw_Xenon_stvx128()
        {
            Given_Xenon();
            AssertCode(0x116021C3,   // stvx128	v11,r0,r4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r4:word128] = v11");
        }



        [Test]
        public void PPCRw_vxor128()
        {
            Given_Xenon();
            AssertCode(0x145AE331,   // vxor128	v2,v58,v60
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v2 = v58 ^ v60");
        }

        [Test]
        public void PPCRw_vmul10euq()
        {
            Given_HexString("10231A41");
            AssertCode(     // vmul10euq	v1,v3,v3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v1 = __vector_mul10_extended(v3, v3)");
        }

        [Test]
        public void PPCRw_vmulfp128()
        {
            Given_Xenon();
            AssertCode(0x1497B0B1,   // vmulfp128	v4,v55,v54
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v4 = __simd_fmul<real32[4]>(v55, v54)");
        }

        [Test]
        public void PPCRw_vmulosh()
        {
            Given_HexString("12F59948");
            AssertCode(     // vmulosh	v23,v21,v19
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v23 = __vector_mul_odd<int16[8],int32[4]>(v21, v19)");
        }

        [Test]
        public void PPCRw_vmuluwm()
        {
            Given_HexString("10861889");
            AssertCode(     // vmuluwm	v4,v6,v3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v4 = __vector_mul_modulo<uint32[4]>(v6, v3)");
        }

        [Test]
        public void PPCRw_vslb()
        {
            Given_HexString("11C02104");
            AssertCode(     // vslb	v14,v0,v4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v14 = __vector_shift_left<byte[16]>(v0, SLICE(v4, byte, 0))");
        }

        [Test]
        public void PPCRw_vsldoi128()
        {
            Given_Xenon();
            Given_HexString("1339C935");
            AssertCode(     // vsldoi128	v57,v57,v57,04
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v57 = __vector_shift_left_double(v57, v57, SLICE(4<32>, byte, 0))");
        }

        [Test]
        public void PPCRw_vslh()
        {
            Given_HexString("104B4144");
            AssertCode(     // vslh	v2,v11,v8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v2 = __vector_shift_left<word16[8]>(v11, SLICE(v8, byte, 0))");
        }

        [Test]
        public void PPCRw_vslw128()
        {
            Given_Xenon();
            AssertCode(0x1B5FF8F5,   // vslw128	v58,v63,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v58 = __vector_shift_left<word32[4]>(v63, SLICE(v63, byte, 0))");
        }

        [Test]
        public void PPCRw_vspltisw128()
        {
            Given_Xenon();
            AssertCode(0x1B620774,   // vspltisw128	v59,v0,+20
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v59 = __vector_splat_imm<int32[4]>(v0)");
        }

        [Test]
        public void PPCRw_vspltw128()
        {
            Given_Xenon();
            AssertCode(0x1923CF31,   // vspltw128	v9,v57,03
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v9 = __vector_splat<word32[4]>(v57, 3<32>)");
        }

        [Test]
        public void PPCRw_vmrghb()
        {
            Given_HexString("1045000C");
            AssertCode(     // vmrghb	v2,v5,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v2 = __vector_merge_high<byte[16]>(v5, v0)");
        }

        [Test]
        public void PPCRw_vmrghh()
        {
            Given_HexString("1109004C");
            AssertCode(     // vmrghh	v8,v9,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v8 = __vector_merge_high<word16[8]>(v9, v0)");
        }

        [Test]
        public void PPCRw_vmrglb()
        {
            Given_HexString("1025010C");
            AssertCode(     // vmrglb	v1,v5,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v1 = __vector_merge_low<byte[16]>(v5, v0)");
        }

        [Test]
        public void PPCRw_vmrglh()
        {
            Given_HexString("10E9014C");
            AssertCode(     // vmrglh	v7,v9,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v7 = __vector_merge_low<word16[8]>(v9, v0)");
        }

        [Test]
        public void PPCRw_vmrghw128()
        {
            Given_Xenon();
            AssertCode(0x1B1FF325,   // vmrghw128	v56,v63,v62
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v56 = __vector_merge_high<word32[4]>(v63, v62)");
        }

        [Test]
        public void PPCRw_vmrglw128()
        {
            Given_Xenon();
            AssertCode(0x1BFFF365,   // vmrglw128	v63,v63,v62
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = __vector_merge_low<word32[4]>(v63, v62)");
        }

        [Test]
        public void PPCRw_vnmsubfp128()
        {
            Given_Xenon();
            Given_HexString("15BEE971");
            AssertCode(     // vnmsubfp128	v13,v62,v61,v13
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = __vnmsubfp<real32[4]>(v62, v61, v13)");
        }

        [Test]
        public void PPCRw_vnor()
        {
            AssertCode(0x11338D04,   // vnor	v9,v19,v17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v9 = ~(v19 | v17)");
        }

        [Test]
        public void PPCRw_vor128()
        {
            Given_Xenon();
            AssertCode(0x15B8C2F1,   // vor128	v13,v56,v56
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = v56");
        }

        [Test]
        public void PPCRw_vorc()
        {
            AssertCode(0x11338D44,   // vorc	v9,v19,v17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v9 = v19 | ~v17");
        }

        [Test]
        public void PPCRw_vpkshss()
        {
            Given_HexString("11AD018E");
            AssertCode(     // vpkshss	v13,v13,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = __vector_pack_sat<int16[8],int8[16]>(v13, v0)");
        }

        [Test]
        public void PPCRw_vpkuhus()
        {
            Given_HexString("1000688E");
            AssertCode(     // vpkuhus	v0,v0,v13
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_pack_sat<uint16[8],uint8[16]>(v0, v13)");
        }

        [Test]
        public void PPCRw_vpopcntb()
        {
            Given_HexString("1127BF03");
            AssertCode(     // vpopcntb	v9,v23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v9 = __vector_popcnt<byte[16]>(v23)");
        }

        [Test]
        public void PPCRw_vpopcntd()
        {
            Given_HexString("11E2FFC3");
            AssertCode(     // vpopcntd	v15,v31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v15 = __vector_popcnt<word64[2]>(v31)");
        }


        [Test]
        public void PPCRw_vupkd3d128()
        {
            Given_Xenon();
            AssertCode(0x1B24DFF5,   // vupkd3d128	v57,v59,04
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v57 = __vupkd3d<word128>(v59, 4<32>)");
        }

        [Test]
        public void PPCRw_vupkhpx()
        {
            Given_HexString("1327C34E");
            AssertCode(     // vupkhpx	v25,v7,v24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v25 = __vector_unpack_high_pixel(v7)");
        }

        [Test]
        public void PPCRw_vupkhsb128()
        {
            Given_Xenon();
            Given_HexString("1A000384");
            AssertCode(     // vupkhsb128	v48,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v48 = __vector_unpack_high<int8[16],int16[8]>(v0)");
        }

        [Test]
        public void PPCRw_vupkhsh()
        {
            Given_HexString("13C05A4E");
            AssertCode(     // vupkhsh	v30,v0,v11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v30 = __vector_unpack_high<int16[8],int32[4]>(v11)");
        }

        [Test]
        public void PPCRw_vupklsb()
        {
            Given_HexString("1156CA8E");
            AssertCode(     // vupklsb	v10,v22,v25
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = __vector_unpack_low<int8[16],int16[8]>(v25)");
        }

        [Test]
        public void PPCRw_vupklsb128()
        {
            Given_Xenon();
            Given_HexString("19E003C4");
            AssertCode(     // vupklsb128	v47,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v47 = __vector_unpack_low<int8[16],int16[8]>(v0)");
        }

        [Test]
        public void PPCRw_vupklsh()
        {
            Given_HexString("11605ACE");
            AssertCode(     // vupklsh	v11,v0,v11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v11 = __vector_unpack_low<int16[8],int32[4]>(v11)");
        }

        [Test]
        public void PPCRw_vrefp()
        {
            AssertCode(0x11a0010a, //"vrefp\tv13,v0");
                    "0|L--|00100000(4): 1 instructions",
                    "1|L--|v13 = __vector_reciprocal_estimate<real32[4]>(v0)");
        }

        [Test]
        public void PPCRw_vrfim128()
        {
            Given_Xenon();
            Given_HexString("1BE0FF35");
            AssertCode(     // vrfim128	v63,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = __vector_floor<real32[4]>(v63)");
        }

        [Test]
        public void PPCRw_vrfin()
        {
            Given_HexString("12E0BA0A");
            AssertCode(     // vrfin	v23,v23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v23 = __vector_round<real32[4]>(v23)");
        }

        [Test]
        public void PPCRw_vrfip128()
        {
            Given_Xenon();
            AssertCode(0x19ACFF91,   // vrfip128	v13,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = __vector_ceil<real32[4]>(v63)");
        }

        [Test]
        public void PPCRw_vrfiz()
        {
            Given_HexString("10E0FA4A");
            AssertCode(     // vrfiz	v7,v31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v7 = __vector_round_toward_zero<real32[4]>(v31)");
        }

        [Test]
        public void PPCRw_vrlh()
        {
            Given_HexString("10000044");
            AssertCode(     // vrlh	v0,v0,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_rotate_left<word16[8]>(v0, SLICE(v0, byte, 0))");
        }

        [Test]
        public void PPCRw_vaddecuq()
        {
            Given_HexString("1066F53D");
            AssertCode(     // vaddecuq	v3,v6,v30,v20
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v3 = __vector_add_extended_write_carry<word128>(v6, v30, v20)");
        }

        [Test]
        public void PPCRw_vaddeuqm()
        {
            Given_HexString("13EF14BC");
            AssertCode(     // vaddeuqm	v31,v15,v2,v18
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v31 = __vector_add_extended_modulo(v15, v2, v18)");
        }

        [Test]
        public void PPCRw_vand128()
        {
            Given_Xenon();
            AssertCode(0x16D6BA35,   // vand128	v54,v54,v55
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v54 = v54 & v55");
        }

        [Test]
        public void PPCRw_vcfpsxws128()
        {
            Given_Xenon();
            AssertCode(0x1AC2FA35,   // vcfpsxws128	v54,v63,+0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v54 = __vector_cvt_fixedpt<int32[4],real32[4]>(v63, 2<32>)");
        }

        [Test]
        public void PPCRw_vcfpuxws128()
        {
            Given_Xenon();
            Given_HexString("1898EA51");
            AssertCode(     // vcfpuxws128	v4,v61,18
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v4 = __vector_cvt_fixedpt<uint32[4],real32[4]>(v61, 0x18<32>)");
        }

        [Test]
        public void PPCRw_Xenon_vcsxwfp128()
        {
            Given_Xenon();
            AssertCode(0x1801F2B1,   // vcsxwfp128	v0,v62,01
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vcsxwfp<word128>(v62, 1<32>)");
        }

        [Test]
        public void PPCRw_vexptefp128()
        {
            Given_Xenon();
            AssertCode(0x1BA0EEB5,   // vexptefp128	v61,v61
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v61 = __vector_2_exp_estimate<real32[4]>(v61)");
        }

        [Test]
        public void PPCRw_vextractub()
        {
            Given_HexString("11BAFA0D");
            AssertCode(     // vextractub	v13,v31,0A
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = __vector_extract<byte>(v31, 0xA<32>)",
                "2|L--|v13 = SEQ(SLICE(0<128>, word56, 72), v5, SLICE(0<128>, word64, 0))");
        }

        [Test]
        public void PPCRw_vextractuw()
        {
            Given_HexString("10585A8D");
            AssertCode(     // vextractuw	v2,v11,08
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = __vector_extract<uint32>(v11, 8<32>)",
                "2|L--|v2 = SEQ(SLICE(0<128>, word32, 96), v5, SLICE(0<128>, word64, 0))");
        }

        [Test]
        public void PPCRw_vextuwrx()
        {
            Given_HexString("10988F8D");
            AssertCode(     // vextuwrx	r4,r24,v17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __vector_extract_right_indexed(r24, v17)");
        }

        [Test]
        public void PPCRw_vlogefp()
        {
            Given_HexString("138021CA");
            AssertCode(     // vlogefp	v28,v4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v28 = __vector_log2_estimate<real32[4]>(v4)");
        }

        [Test]
        public void PPCRw_vlogefp128()
        {
            Given_Xenon();
            AssertCode(0x1AA0EEF5,   // vlogefp128	v53,v61
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v53 = __vector_log2_estimate<real32[4]>(v61)");
        }

        [Test]
        public void PPCRw_vmaddcfp128()
        {
            Given_Xenon();
            AssertCode(0x17DE591C,   // vmaddcfp128	v126,v30,v11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v126 = __vmaddcfp<real32[4]>(v30, v11)");
        }

        [Test]
        public void PPCRw_vmsub3fp128()
        {
            Given_Xenon();
            AssertCode(0x17E21194,   // vmsub3fp128	v63,v2,v2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = __vmsub3fp<real32[4]>(v2, v2)");
        }

        [Test]
        public void PPCRw_vbpermd()
        {
            Given_HexString("134A05CC"); // CC054A13");
            AssertCode(     // vbpermd	v26,v10,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v26 = __vector_bit_permute<uint64[2]>(v10, v0)");
        }

        [Test]
        public void PPCRw_vperm128()
        {
            Given_Xenon();
            AssertCode(0x17FFF025,   // vperm128	v63,v63,v62,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = __vector_permute<byte[16]>(v63, v62, v0)");
        }

        [Test]
        public void PPCRw_vpermr()
        {
            Given_HexString("138000BB");
            AssertCode(     // vpermr	v28,v0,v0,v2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v28 = __vector_permute_right_indexed<byte[16]>(v0, v0, v2)");
        }

        [Test]
        public void PPCRw_vpermxor()
        {
            Given_HexString("120E7DED");
            AssertCode(     // vpermxor	v16,v14,v15,v23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v16 = __vector_permute_xor<byte[16]>(v14, v15, v23)");
        }

        [Test]
        public void PPCRw_vpksdss()
        {
            Given_HexString("116E9DCE");
            AssertCode(     // vpksdss	v11,v14,v19
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v11 = __vector_pack_sat<int64[2],int32[4]>(v14, v19)");
        }

        [Test]
        public void PPCRw_vpksdus()
        {
            Given_HexString("12C0354E");
            AssertCode(     // vpksdus	v22,v0,v6
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v22 = __vector_pack_sat<int64[2],uint32[4]>(v0, v6)");
        }

        [Test]
        public void PPCRw_vpkshus()
        {
            Given_HexString("100D010E");
            AssertCode(     // vpkshus	v0,v13,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_pack_sat<int16[8],uint8[16]>(v13, v0)");
        }

        [Test]
        public void PPCRw_vpkswss()
        {
            Given_HexString("10B631CE");
            AssertCode(     // vpkswss	v5,v22,v6
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v5 = __vector_pack_sat<int32[4],int16[8]>(v22, v6)");
        }

        [Test]
        public void PPCRw_vpkswus()
        {
            Given_HexString("1000614E");
            AssertCode(     // vpkswus	v0,v0,v12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_pack_sat<int32[4],uint16[8]>(v0, v12)");
        }

        [Test]
        public void PPCRw_vpmsumd()
        {
            Given_HexString("102CA4C8");
            AssertCode(     // vpmsumd	v1,v12,v20
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v1 = __vector_pack_sat<word64[2],word128[1]>(v12, v20)");
        }

        [Test]
        public void PPCRw_vrefp128()
        {
            Given_Xenon();
            AssertCode(0x1800FE31,   // vrefp128	v0,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_reciprocal_estimate<real32[4]>(v63)");
        }

        [Test]
        public void PPCRw_vrfin128()
        {
            Given_Xenon();
            AssertCode(0x1BC0DB75,   // vrfin128	v62,v59
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v62 = __vector_round<real32[4]>(v59)");
        }

        [Test]
        public void PPCRw_vrfiz128()
        {
            Given_Xenon();
            AssertCode(0x1AC0FBF5,   // vrfiz128	v54,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v54 = __vector_round_toward_zero<real32[4]>(v63)");
        }

        [Test]
        public void PPCRw_vrsqrtefp128()
        {
            Given_Xenon();
            AssertCode(0x19A0FE71,   // vrsqrtefp128	v13,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = __vector_reciprocal_sqrt_estimate<real32[4]>(v63)");
        }

        [Test]
        public void PPCRw_vsr()
        {
            Given_HexString("116062C4");
            AssertCode(     // vsr	v11,v0,v11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v11 = __vector_shift_right<word128>(v0, v12)");
        }

        [Test]
        public void PPCRw_vsraw128()
        {
            Given_Xenon();
            Given_HexString("194BF951");
            AssertCode(     // vsraw128	v10,v11,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = __vector_shift_right_arithmetic<int32[4]>(v11, SLICE(v63, byte, 0))");
        }

        [Test]
        public void PPCRw_vsrb()
        {
            Given_HexString("13F86204");
            AssertCode(     // vsrb	v31,v24,v12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v31 = __vector_shift_right<byte[16]>(v24, SLICE(v12, byte, 0))");
        }

        [Test]
        public void PPCRw_vsrd()
        {
            Given_HexString("12305EC4"); // C45E3012");
            AssertCode(     // vsrd	v17,v16,v11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v17 = __vector_shift_right<word64[2]>(v16, SLICE(v11, byte, 0))");
        }

        [Test]
        public void PPCRw_vsrw()
        {
            Given_HexString("11095284");
            AssertCode(     // vsrw	v8,v9,v10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v8 = __vector_shift_right<word32[4]>(v9, SLICE(v10, byte, 0))");
        }

        [Test]
        public void PPCRw_vsrw128()
        {
            Given_Xenon();
            AssertCode(0x195CB9F1,   // vsrw128	v10,v60,v55
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = __vector_shift_right<word32[4]>(v60, SLICE(v55, byte, 0))");
        }

        [Test]
        public void PPCRw_vsubcuw()
        {
            Given_HexString("11B01D80"); // 801DB011");
            AssertCode(     // vsubcuw	v13,v16,v3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = __vector_sub_carry<uint32[4]>(v16, v3)");
        }

        [Test]
        public void PPCRw_vsubecuq()
        {
            Given_HexString("13C72B7F");
            AssertCode(     // vsubecuq	v30,v7,v5,v13
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v30 = __vector_sub_extend_carry<word128>(v7, v5, v13)");
        }

        [Test]
        public void PPCRw_vsubeuqm()
        {
            Given_HexString("139BA47E");
            AssertCode(     // vsubeuqm	v28,v27,v20,v17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v28 = __vector_sub_extended_modulo(v27, v20, v17)");
        }

        [Test]
        public void PPCRw_vsubfp128()
        {
            Given_Xenon();
            AssertCode(0x145D0870,   // vsubfp128	v2,v61,v1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v2 = __simd_fsub<real32[4]>(v61, v1)");
        }

        [Test]
        public void PPCRw_vsubsws()
        {
            Given_HexString("13390780");
            AssertCode(     // vsubsws	v25,v25,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v25 = __vector_sub_saturate<int32[4]>(v25, v0)");
        }

        [Test]
        public void PPCRw_vsububm()
        {
            Given_HexString("10D60C00");
            AssertCode(     // vsububm	v6,v22,v1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v6 = __vector_sub_modulo<uint8[16]>(v22, v1)");
        }

        [Test]
        public void PPCRw_vsubuhm()
        {
            Given_HexString("10C03C40");
            AssertCode(     // vsubuhm	v6,v0,v7
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v6 = __vector_sub_modulo<uint16[8]>(v0, v7)");
        }

        [Test]
        public void PPCRw_vsum4shs()
        {
            Given_HexString("1193BE48");
            AssertCode(     // vsum4shs	v12,v19,v23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v12 = __vector_sum_across_quarter_sat<int16[8],int32[4]>(v19, v23)");
        }

        [Test]
        public void PPCRw_psq_st()
        {
            Given_750();
            AssertCode(0xF3E10038,   // psq_st	f31,r1,+00000038,01,07
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = f31",
                "2|L--|v5 = __pack_quantized(v4, 1<32>, 7<32>)",
                "3|L--|Mem0[r1 + 56<i32>:word64] = v5");
        }

        [Test]
        public void PPCRw_psq_stx()
        {
            Given_750();
            AssertCode(0x11C0180E,   // psq_stx	f14,r0,r3,00,07
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = f14",
                "2|L--|v5 = __pack_quantized(v4, 0<32>, 7<32>)",
                "3|L--|Mem0[r3:word64] = v5");
        }


        [Test]
        public void PPCRw_fcmpo()
        {
            Given_750();
            AssertCode(0xFC160040,   // fcmpo	cr0,f22,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr0 = cond(f22 - f0)");
        }

        [Test]
        public void PPCRw_psq_l()
        {
            Given_750();
            AssertCode(0xE3E10028,   // psq_l	f31,r1,+00000028,01,07
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = Mem0[r1 + 40<i32>:word64]",
                "2|L--|v5 = __unpack_quantized(v4, 1<8>, 7<8>)",
                "3|L--|f31 = v5");
        }

        [Test]
        public void PPCRw_psq_lx()
        {
            Given_750();
            AssertCode(0x1009000C,   // psq_lx	f0,r9,r0,00,00
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = Mem0[r9:word64]",
                "2|L--|v5 = __unpack_quantized(v4, 0<8>, 0<8>)",
                "3|L--|f0 = v5");
        }

        [Test]
        public void PPCRw_psq_lux()
        {
            Given_750();
            AssertCode(0x10245F4C,   // psq_lux	f1,r4,r11,01,00
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = Mem0[r4 + r11:word64]",
                "2|L--|v6 = __unpack_quantized(v5, 1<8>, 0<8>)",
                "3|L--|f1 = v6",
                "4|L--|r4 = r4 + r11");
        }


        [Test]
        public void PPCRw_fnmadds()
        {
            Given_750();
            AssertCode(0xEC0400FE,   // fnmadds	f0,f4,f0,f3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = CONVERT(-(SLICE(f4, real32, 0) * SLICE(f0, real32, 0) + SLICE(f3, real32, 0)), real32, real64)");
        }

        [Test]
        public void PPCRw_ps_add_cr()
        {
            Given_750();
            AssertCode(0x11AA80EB,   // ps_add.	f13,f10,f16
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v6 = f10",
                "2|L--|v7 = f16",
                "3|L--|v8 = __ps_add<real32[2]>(v6, v7)",
                "4|L--|f13 = v8",
                "5|L--|cr1 = cond(f13[0<i32>])");
        }

        [Test]
        public void PPCRw_ps_rsqrte()
        {
            Given_750();
            AssertCode(0x10228034,   // ps_rsqrte	f1,f16
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = f16",
                "2|L--|v6 = __ps_rsqrte<real32[2]>(v5)",
                "3|L--|f1 = v6");
        }

        [Test]
        public void PPCRw_ps_sub()
        {
            Given_750();
            AssertCode(0x10000028,   // ps_sub	f0,f0,f0
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = f0",
                "2|L--|v5 = f0",
                "3|L--|v6 = __ps_sub<real32[2]>(v4, v5)",
                "4|L--|f0 = v6");
        }

        [Test]
        public void PPCRw_ps_div()
        {
            Given_750();
            AssertCode(0x102C0024,   // ps_div	f1,f12,f0
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v6 = f12",
                "2|L--|v7 = f0",
                "3|L--|v8 = __ps_div<real32[2]>(v6, v7)",
                "4|L--|f1 = v8");
        }


        [Test]
        public void PPCRw_frsqrte()
        {
            Given_750();
            AssertCode(0xFC005834,   // frsqrte	f0,f11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = __frsqrte(f11)");
        }

        [Test]
        public void PPCRw_fnmadd()
        {
            Given_750();
            AssertCode(0xFFFFFFFF,   // fnmadd.	f31,f31,f31,f31
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f31 = -(f31 * f31 + f31)",
                "2|L--|cr1 = cond(f31)");
        }

        [Test]
        public void PPCRw_ps_neg()
        {
            Given_750();
            AssertCode(0x108B6850,   // ps_neg	f4,f13
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = f13",
                "2|L--|v6 = __ps_neg<real32[2]>(v5)",
                "3|L--|f4 = v6");
        }

        [Test]
        public void PPCRw_ps_res()
        {
            Given_750();
            AssertCode(0x10320030,   // ps_res	f1,f0
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = f0",
                "2|L--|v6 = __ps_res<real32[2]>(v5)",
                "3|L--|f1 = v6");
        }

        [Test]
        public void PPCRw_ps_madds0()
        {
            Given_750();
            AssertCode(0x1031801D,   // ps_madds0.	f1,f17,f0,f16
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v7 = f17",
                "2|L--|v8 = f0",
                "3|L--|v9 = f16",
                "4|L--|v10 = __ps_madds0<real32[2]>(v7, v8, v9)",
                "5|L--|f1 = v10",
                "6|L--|cr1 = cond(f1[0<i32>])");
        }

        [Test]
        public void PPCRw_ps_madds1()
        {
            Given_750();
            AssertCode(0x1206029F,   // ps_madds1.	f16,f6,f0,f10
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v7 = f6",
                "2|L--|v8 = f10",
                "3|L--|v9 = f0",
                "4|L--|v10 = __ps_madds1<real32[2]>(v7, v8, v9)",
                "5|L--|f16 = v10",
                "6|L--|cr1 = cond(f16[0<i32>])");
        }

        [Test]
        public void PPCRw_ps_merge11()
        {
            Given_750();
            AssertCode(0x11D39CE0,   // ps_merge11	f14,f19,f19
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = f19",
                "2|L--|v6 = f19",
                "3|L--|v7 = __ps_merge11<real32[2]>(v5, v6)",
                "4|L--|f14 = v7");
        }

        [Test]
        public void PPCRw_ps_muls0()
        {
            Given_750();
            AssertCode(0x12870718,   // ps_muls0	f20,f7,f28
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v6 = f7",
                "2|L--|v7 = f28",
                "3|L--|v8 = __ps_muls0<real32[2]>(v6, v7)",
                "4|L--|f20 = v8");
        }

        [Test]
        public void PPCRw_ps_nmsub()
        {
            Given_750();
            AssertCode(0x1021003C,   // ps_nmsub	f1,f1,f0,f0
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v5 = f1",
                "2|L--|v6 = f0",
                "3|L--|v7 = f0",
                "4|L--|v8 = __ps_nmsub<real32[2]>(v5, v6, v7)",
                "5|L--|f1 = v8");
        }

        [Test]
        public void PPCRw_ps_mr()
        {
            Given_750();
            AssertCode(0x10200090,   // ps_mr	f1,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f1 = f0");
        }

        [Test]
        public void PPCRw_ps_sum0()
        {
            Given_750();
            AssertCode(0x102D0014,   // ps_sum0	f1,f13,f0,f0
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v6 = f13",
                "2|L--|v7 = f0",
                "3|L--|v8 = f0",
                "4|L--|v9 = __ps_sum0<real32[2]>(v6, v7, v8)",
                "5|L--|f1 = v9");
        }

        [Test]
        public void PPCRw_ps_madd()
        {
            Given_750();
            AssertCode(0x1065113A,   // ps_madd	f3,f5,f4,f2
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v7 = f5",
                "2|L--|v8 = f4",
                "3|L--|v9 = f2",
                "4|L--|v10 = __ps_madd<real32[2]>(v7, v8, v9)",
                "5|L--|f3 = v10");
        }

        [Test]
        public void PPCRw_ps_cmpo0()
        {
            Given_750();
            AssertCode(0x129D0040,   // ps_cmpo0	cr4,f29,f0
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v6 = f29",
                "2|L--|v7 = f0",
                "3|L--|cr4 = __ps_cmpo<real32[2],word32>(v6, v7)");
        }

        [Test]
        public void PPCRw_ps_sel()
        {
            Given_750();
            AssertCode(0x121153AE,   // ps_sel	f16,f17,f14,f10
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v7 = f17",
                "2|L--|v8 = f14",
                "3|L--|v9 = f10",
                "4|L--|v10 = __ps_sel<real32[2]>(v7, v8, v9)",
                "5|L--|f16 = v10");
        }

        [Test]
        public void PPCRw_ps_nabs()
        {
            Given_750();
            AssertCode(0x12008910,   // ps_nabs	f16,f17
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = f17",
                "2|L--|v6 = __ps_nabs<real32[2]>(v5)",
                "3|L--|f16 = v6");
        }

        [Test]
        public void PPCRw_xsaddsp()
        {
            Given_HexString("F31BC007");
            AssertCode(     // xsaddsp	v49,v55,v49
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = SLICE(v55, real64, 64)",
                "2|L--|v6 = SLICE(v49, real64, 64)",
                "3|L--|v7 = CONVERT(v5 + v6, real64, real32)",
                "4|L--|v49 = SEQ(SLICE(v49, word32, 96), v7, SLICE(v49, word64, 0))");
        }

        [Test]
        public void PPCRw_xvadddp()
        {
            Given_HexString("F2010B00");
            AssertCode(     // xvadddp	v32,v2,v2"
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v32 = __simd_fadd<real64[2]>(v2, v2)");
        }

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues







        [Test]
        public void PPCRw_vsrab()
        {
            Given_HexString("118C6B04");
            AssertCode(     // vsrab	v12,v12,v13
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v12 = __vector_shift_right_arithmetic<int8[16]>(v12, SLICE(v13, byte, 0))");
        }

        [Test]
        public void PPCRw_vaddsbs()
        {
            Given_HexString("11AD6B00");
            AssertCode(     // vaddsbs	v13,v13,v13
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = __vector_add_saturate<int8[16]>(v13, v13)");
        }

        [Test]
        public void PPCRw_vsrah()
        {
            Given_HexString("13AD8344");
            AssertCode(     // vsrah	v29,v13,v16
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v29 = __vector_shift_right_arithmetic<int16[8]>(v13, SLICE(v16, byte, 0))");
        }

        [Test]
        public void PPCRw_vsrh()
        {
            Given_HexString("13C94A44");
            AssertCode(     // vsrh	v30,v9,v9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v30 = __vector_shift_right<word16[8]>(v9, SLICE(v9, byte, 0))");
        }

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues







        [Test]
        public void PPCRw_xxpermdi()
        {
            Given_PowerPcLe64();
            Given_HexString("500000F0");
            AssertCode(     // xxpermdi	v0,v0,v0,00
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __xxpermdi<word64[2]>(v0, v0, 0<64>)");
        }

        [Test]
        public void PPCRw_mtocrf()
        {
            Given_PowerPcLe64();
            Given_HexString("2081707D");
            AssertCode(     // mtocrf	0B,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__move_to_condition_field<word64>(r11, 0xB<64>)");
        }


        [Test]
        public void PPCRw_mulhd()
        {
            Given_PowerPcLe64();
            Given_HexString("9250297D");
            AssertCode(     // mulhd	r9,r9,r10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = SLICE(r9 *s128 r10, word64, 64)");
        }

        [Test]
        public void PPCRw_mulhdu()
        {
            Given_PowerPcLe64();
            Given_HexString("1248367D");
            AssertCode(     // mulhdu	r9,r22,r9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = SLICE(r22 *u128 r9, word64, 64)");
        }

        [Test]
        public void PPCRw_xxlorc()
        {
            Given_PowerPcLe64();
            Given_HexString("576D00F0");
            AssertCode(     // xxlorc	v1,v1,v27
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v1 = v1 | ~v27");
        }

        [Test]
        public void PPCRw_xxlxor()
        {
            Given_PowerPcLe64();
            Given_HexString("D0544AF1");
            AssertCode(     // xxlxor	v20,v20,v20
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v20 = 0<128>");
        }

        [Test]
        public void PPCRw_fcfidus()
        {
            Given_PowerPcLe64();
            Given_HexString("9C0700EC");
            AssertCode(     // fcfidus	f0,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = SEQ(SLICE(f0, word32, 32), CONVERT(f0, int64, real32))");
        }

        [Test]
        public void PPCRw_fctiduz()
        {
            Given_PowerPcLe64();
            Given_HexString("5E0700FC");
            AssertCode(     // fctiduz	f0,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = __convert_to_uint64(f0)");
        }

        [Test]
        public void PPCRw_mffprd()
        {
            Given_PowerPcLe64();
            Given_HexString("6600037C");
            AssertCode(     // mffprd	r3,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = SLICE(f0, word64, 64)");
        }

        [Test]
        public void PPCRw_lxsdx()
        {
            Given_PowerPcLe64();
            Given_HexString("994CE07F");
            AssertCode(     // lxsdx	v63,r0,r9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = SEQ(Mem0[r9:word64], SLICE(v63, word64, 0))");
        }

        [Test]
        public void PPCRw_xscvuxddp()
        {
            Given_PowerPcLe64();
            Given_HexString("A2F520F0");
            AssertCode(     // xscvuxddp	v1,v62
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(v62, int64, 64)",
                "2|L--|v1 = SEQ(CONVERT(v5, int64, real64), SLICE(v1, word64, 0))");
        }

        [Test]
        public void PPCRw_xsdivdp()
        {
            Given_PowerPcLe64();
            Given_HexString("C2F921F0");
            AssertCode(     // xsdivdp	v2,v2,v63
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = SLICE(v2, real64, 64)",
                "2|L--|v6 = SLICE(v63, real64, 64)",
                "3|L--|v7 = CONVERT(v5 / v6, real64, real32)",
                "4|L--|v2 = SEQ(v7, SLICE(v2, word64, 0))");
        }

        [Test]
        public void PPCRw_vaddudm()
        {
            Given_PowerPcLe64();
            Given_HexString("C0080010");
            AssertCode(     // vaddudm	v0,v0,v1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vector_add_modulo<uint64[2]>(v0, v1)");
        }

        [Test]
        public void PPCRw_mtvsrwa()
        {
            Given_PowerPcLe64();
            Given_HexString("A6019A7F");
            AssertCode(     // mtvsrwa	v28,r26
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r26, word32, 0)",
                "2|L--|v5 = CONVERT(v4, int32, int64)",
                "3|L--|v28 = SEQ(v5, SLICE(v28, word64, 0))");
        }

        [Test]
        public void PPCRw_xscmpexpqp()
        {
            Given_PowerPcLe64();
            Given_HexString("48F9FFFF");
            AssertCode(     // xscmpexpqp	07,v31,v31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr1 = __xcompare_exponents128(7<64>, v31, v31)");
        }

        [Test]
        public void PPCRw_mtfsb0()
        {
            Given_PowerPcLe64();
            Given_HexString("8C00C0FF");
            AssertCode(     // mtfsb0	1F
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__mtfsb(0x1F<64>, false)");
        }

        [Test]
        public void PPCRw_xscvdpsxws()
        {
            Given_PowerPcLe64();
            Given_HexString("616100F0");
            AssertCode(     // xscvdpsxws	v32,v12
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = CONVERT(SLICE(v12, int64, 64), int64, real64)",
                "2|L--|v5 = SLICE(v12, real64, 64)",
                "3|L--|v32 = SEQ(SLICE(v32, word32, 96), CONVERT(v5, real64, real32), SLICE(v32, word64, 0))"
);
        }
    }
}
