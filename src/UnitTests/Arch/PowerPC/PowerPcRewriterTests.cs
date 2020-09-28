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

using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Rtl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcRewriterTests : RewriterTestBase
    {
        private InstructionBuilder b;
        private PowerPcArchitecture arch;
        private IEnumerable<PowerPcInstruction> ppcInstrs;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new PowerPcBe32Architecture("ppc-be-32");
            this.addr = Address.Ptr32(0x00100000);
            this.ppcInstrs = null;
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress => addr;

        private void RunTest(Action<InstructionBuilder> m)
        {
            b = new InstructionBuilder(arch, Address.Ptr32(0x00100000));
            m(b);
            ppcInstrs = b.Instructions;
        }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            if (ppcInstrs != null)
                return new PowerPcRewriter(arch, ppcInstrs, binder, host);
            var rdr = arch.CreateImageReader(mem, 0);
            return arch.CreateRewriter(rdr, arch.CreateProcessorState(), binder, host);
        }

        private void Given_PowerPcBe64()
        {
            this.arch = new PowerPcBe64Architecture("ppc-be-64");
        }

        private void Given_Xenon()
        {
            this.arch = new PowerPcBe64Architecture("ppc-be-64");
        }

        private void Given_750()
        {
            this.arch = new PowerPcBe32Architecture("ppc-be-32");
            this.arch.LoadUserOptions(new Dictionary<string, object>
            {
                { "Model", "750" }
            });
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
                "1|L--|r4 = r0 | 0x12340000");
        }

        [Test]
        public void PPCRw_Add()
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
        public void PPCRw_Add_()
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
        public void PPCRW_lwzu()
        {
            RunTest((m) =>
            {
                m.Lwzu(m.r2, 4, m.r1);
            });
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2 = Mem0[r1 + 4:word32]",
                "2|L--|r1 = r1 + 4"
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
                "1|L--|r2 = Mem0[0xFFFFFFFC:word32]"
                );
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
                "1|L--|Mem0[r3 + 18:byte] = (byte) r2",
                "2|L--|r3 = r3 + 18"
                );
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
                "1|L--|Mem0[r3 + r0:byte] = (byte) r2",
                "2|L--|r3 = r3 + r0"
                );
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

        [Test]
        public void PPCRw_rlwinm()
        {
            AssertCode(0x57200036, // "rlwinm\tr0,r25,04,00,1B");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r25 & 0xFFFFFFF0");
            AssertCode(0x5720EEFA, //,rlwinm	r9,r31,1D,1B,1D not handled yet.
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r25 >>u 0x03 & 0x0000001C");
            AssertCode(0x5720421E, //  rlwinm	r0,r25,08,08,0F not handled yet.
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r25 << 0x08 & 0x00FF0000");
            AssertCode(0x57897c20, // rlwinm  r9,r28,15,16,16	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r28 << 0x0F & 0x00008000");
            AssertCode(0x556A06F7, // rlwinm.\tr10,r11,00,1B,1B
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r10 = r11 & 0x00000010",
                "2|L--|cr0 = cond(r10)");
        }

        [Test]
        public void PPCRw_lwzx()
        {
            AssertCode(0x7c9c002e, // "lwzx\tr4,r28,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = Mem0[r28 + r0:word32]");
        }

        [Test]
        public void PPCRw_stwx()
        {
            AssertCode(0x7c95012e, // "stwx\tr4,r21,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r21 + r0:word32] = r4");
        }

        [Test]
        public void PPCRw_subf()
        {
            AssertCode(0x7c154850, // "subf\tr0,r21,r9");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r9 - r21");
        }

        [Test]
        public void PPCRw_srawi()
        {
            AssertCode(0x7c002670, //"srawi\tr0,r0,04");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 >> 0x00000004");
        }

        [Test]
        public void PPCRw_bctr()
        {
            AssertCode(0x4e800420, //"bcctr\t14,00");
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto ctr");
        }

        [Test]
        public void PPCRw_stwu()
        {
            AssertCode(0x9521016e, // "stwu\tr9,r1,r0");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r1 + 366:word32] = r9",
                "2|L--|r1 = r1 + 366");
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
                "1|L--|r10 = __cntlzw(r10)");
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
                "1|L--|r0 = 0");
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
        public void PPCRw_fctiwz()
        {
            AssertCode(0xfc00081E, //"fctiwz\tf0,f1");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = (int32) f1");
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
                "1|L--|__mtcrf(0x00000008, r12)");
        }

        [Test]
        public void PPCRw_bctrl()
        {
            AssertCode(0x4e800421, // "bctrl\t14,00");
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call ctr (0)");
        }

        [Test]
        public void PPCRw_rlwimi()
        {
            AssertCode(0x5120f042, // "rlwimi\tr0,r9,1E,01,01");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __rlwimi(r9, 0x0000001E, 0x00000001, 0x00000001)");
        }

        [Test]
        public void PPCRw_bl()
        {
            AssertCode(0x48000045,
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 00100044 (0)");
        }

        [Test]
        public void PPCRw_addis_r0()
        {
            AssertCode(0x3C000045,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = 0x00450000");
        }

        [Test]
        public void PPCRw_addis()
        {
            AssertCode(0x3C810045,
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r1 + 0x00450000");
        }

        [Test]
        public void PPCRw_cmpwi()
        {
            AssertCode(0x2f830005, // 	cmpwi   cr7,r3,5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(r3 - 5)");
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
        public void PPCRw_stb()
        {
            AssertCode(0x98010018u, // "stb\tr0,1(r1)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 24:byte] = (byte) r0");
        }

        [Test]
        public void PPCRw_blr()
        {
            AssertCode(0x4E800020, // blr
                "0|T--|00100000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void PPCRw_lbz()
        {
            AssertCode(0x8809002a,	//lbz     r0,42(r9)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = (word32) Mem0[r9 + 42:byte]");
        }

        [Test]
        public void PPCRw_crxor()
        {
            AssertCode(0x4cc63182, // crclr   4*cr1+eq	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__crxor(0x00000006, 0x00000006, 0x00000006)");
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
                "1|L--|r0 = r0 | 0x00000020");
        }

        [Test]
        public void PPCRw_rlwinm_1_31_31()
        {
            AssertCode(0x54630ffe, // rlwinm r3,r3,1,31,31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 >>u 0x1F");
        }

        [Test]
        public void PPCRw_regression_1()
        {
            AssertCode(0xfdad0024, // fdiv    f13,f13,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f13 = f13 / f0");

            AssertCode(0x38a0ffff, // li      r5,-1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = -1");

            AssertCode(0x575a1838, // rlwinm  r26,r26,3,0,28 
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r26 = r26 << 0x03");

            AssertCode(0x7c03db96, // divwu   r0,r3,r27
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r3 /u r27");

            AssertCode(0x7fe0d9d6, // mullw   r31,r0,r27
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r31 = r0 * r27");

            AssertCode(0x6fde8000, // xoris   r30,r30,32768
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = r30 ^ 0x80000000");

            AssertCode(0x7f891800, // cmpw    cr7,r9,r3	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(r9 - r3)");

            AssertCode(0xdbe10038, // stfd    f31,56(r1)	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 56:real64] = f31");

            AssertCode(0xc00b821c, // lfs     f0,-32228(r11)	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = (real64) Mem0[r11 + -32228:real32]");

            AssertCode(0xc8098220, // lfd     f0,-32224(r9)	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = Mem0[r9 + -32224:real64]");

            AssertCode(0x1f9c008c, // mulli   r28,r28,140	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = r28 * 140");

            AssertCode(0x7c1ed9ae, // stbx    r0,r30,r27	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r30 + r27:byte] = (byte) r0");

            AssertCode(0xa001001c, // lhz     r0,28(r1)	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = (word32) Mem0[r1 + 28:word16]");

            AssertCode(0x409c0ff0, // bge-   cr7,0x00001004	
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GE,cr7)) branch 00100FF0");

            AssertCode(0x2b8300ff, // cmplwi  cr7,r3,255	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(r3 - 0x000000FF)");
        }

        [Test]
        public void PPCRw_lbzu()
        {
            AssertCode(0x8D010004, // lbzu
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r8 = (word32) Mem0[r1 + 4:byte]",
                "2|L--|r1 = r1 + 4");
        }

        [Test]
        public void PPCRw_sth()
        {
            AssertCode(0xB0920004u, // sth
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r18 + 4:word16] = (word16) r4"
                );
        }

        [Test]
        public void PPCrw_subfic()
        {
            AssertCode(0x20320100, // subfic
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = 256 - r18");
        }

        [Test]
        public void PPCrw_andis()
        {
            AssertCode(0x74320100, // andis
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r18 = r1 & 0x01000000",
                "2|L--|cr0 = cond(r18)");
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
                "1|L--|__creqv(0x00000006, 0x00000006, 0x00000006)");
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
        public void PPCrw_and()
        {
            AssertCode(0x7c7ef039, //"and.\tr30,r3,r30");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r30 = r3 & r30",
                "2|L--|cr0 = cond(r30)");
        }

        [Test]
        public void PPCrw_mulhw_()
        {
            AssertCode(0x7ce03897, //"mulhw.\tr7,r0,r7");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r7 = r0 * r7 >> 0x00000020",
                "2|L--|cr0 = cond(r7)");
        }

        [Test]
        public void PPCrw_divw()
        {
            AssertCode(0x7d3d03d6, //"divw\tr9,r29,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r29 / r0");
        }

        [Test]
        public void PPCrw_lbzux()
        {
            AssertCode(0x7c1ee8ee, // "lbzux\tr0,r30,r29");
                    "0|L--|00100000(4): 2 instructions",
                    "1|L--|r0 = (word32) Mem0[r30 + r29:byte]",
                    "2|L--|r30 = r30 + r29");
        }

        [Test]
        public void PPCrw_subfze()
        {
            AssertCode(0x7fde0190, // "subfze\tr30,r30");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = 0x00000000 - r30 + xer");
        }


        [Test]
        public void PPCrw_subfe()
        {
            AssertCode(0x7c631910, // "subfe\tr3,r3,r3");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 - r3 + xer");
        }

        [Test]
        public void PPCrw_extsb()
        {
            AssertCode(0x7c000775, //"extsb.\tr0,r0");
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = (int8) r0",
                "2|L--|r0 = (int32) v3",
                "3|L--|cr0 = cond(r0)");
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
                 "1|L--|r19 = __reverse_bytes_32(Mem0[r3:word32])");
            AssertCode(0x7c00252c, // stwbrx r0,0,r4
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[r4:word32] = __reverse_bytes_32(r0)");
        }

        [Test]
        public void PPCrw_sthx()
        {
            AssertCode(0x7c1b1b2e, //	sthx    r0,r27,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r27 + r3:word16] = (word16) r0");
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
                "1|L--|Mem0[r1 + 40:word64] = r2");
        }

        [Test]
        public void PPCrw_stdu()
        {
            AssertCode(0xf8410029, //	stdu    r2,40(r1))"
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r1 + 40:word64] = (word64) r2",
                "2|L--|r1 = r1 + 40");
        }

        [Test]
        public void PPCrw_rldicl()
        {
            Given_PowerPcBe64();
            AssertCode(0x790407c0,    // clrldi  r4,r8,31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r8 & 0x00000001FFFFFFFF");
            AssertCode(0x79040020,    // clrldi  r4,r8,63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r8 & 0x00000000FFFFFFFF");
            AssertCode(0x78840fe2, // rldicl  r4,r4,33,63	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r4 << 0x21 & 0x0000000000000001");
        }

        [Test]
        public void PPCrw_fcfid()
        {
            AssertCode(0xff00069c,  // fcfid   f24,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f24 = (real64) f0");
        }

        [Test]
        public void PPCrw_stfs()
        {
            AssertCode(0xd0010208, //stfs    f0,520(r1)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 520:real32] = (real32) f0");
        }

        [Test]
        public void PPCrw_frsp()
        {
            AssertCode(0xfd600018, //"frsp\tf11,f0");
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|f11 = (real32) f0");
        }

        [Test]
        public void PPCrw_fmadds()
        {
            AssertCode(0xec1f07ba, //"fmadds\tf0,f31,f30,f0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = (real64) ((real32) f31 * (real32) f30 + (real32) f0)");
        }

        [Test]
        public void PPCrw_fdivs()
        {
            AssertCode(0xec216824, //"fdivs\tf1,f1,f13");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f1 = f1 / f13");
        }

        [Test]
        public void PPCrw_lvx()
        {
            AssertCode(0x7c4048ce, //"lvx\tv2,r0,r9");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v2 = Mem0[r9:word128]");
        }

        [Test]
        public void PPCrw_beqlr()
        {
            AssertCode(0x4d9e0020, // beqlr\tcr7");
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,cr7)) branch 00100004",
                "2|T--|return (0,0)");
        }

        [Test]
        public void PPCrw_vspltw()
        {
            AssertCode(0x10601a8c, // vspltw\tv3,v3,00");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v3 = __vspltw(v3, 0x00000000)");
        }

        [Test]
        public void PPCrw_vxor()
        {
            AssertCode(0x100004c4, ///vxor\tv0,v0,v0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = 0x0000000000000000");
            AssertCode(0x100404c4, ///vxor\tv0,v4,v0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = v4 ^ v0");
        }

        [Test]
        public void PPCrw_rlwnm()
        {
            AssertCode(0x5c00c03e, //"rlwnm\tr0,r0,r24,00,1F");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __rol(r0, r24)");
        }

        [Test]
        public void PPCrw_blelr()
        {
            AssertCode(0x4c9d0020, //"blelr\tcr7");
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(GT,cr7)) branch 00100004",
                "2|T--|return (0,0)");
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
                "1|L--|r11 = r0 >> 0x0000003F");
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
            AssertCode(0x7c07492a, //stdx\tr0,r7,r9");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r7 + r9:word64] = (word64) r0");
        }

        [Test]
        public void PPCRw_regression_3()
        {
            AssertCode(0x4200fff8, // bdnz+   0xfffffffffffffff8	
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 0x00000001",
                        "2|T--|if (ctr != 0x00000000) branch 000FFFF8");
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
                        "1|L--|ctr = ctr - 0x00000001",
                        "2|T--|if (ctr != 0x00000000 && Test(GE,cr0)) branch 000FFEF8");
            AssertCode(0x4040fef8, //"bdzf\tlt,$000FFEF8");
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 0x00000001",
                        "2|T--|if (ctr == 0x00000000 && Test(GE,cr0)) branch 000FFEF8");
            AssertCode(0x4080fef8, //"bge\t$000FFEF8");
                        "0|T--|00100000(4): 1 instructions",
                        "1|T--|if (Test(GE,cr0)) branch 000FFEF8");
            AssertCode(0x4100fef8, //"bdnzt\tlt,$000FFEF8");
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 0x00000001",
                        "2|T--|if (ctr != 0x00000000 && Test(LT,cr0)) branch 000FFEF8");
            AssertCode(0x4180fef8, //"blt\t$000FFEF8");
                        "0|T--|00100000(4): 1 instructions",
                        "1|T--|if (Test(LT,cr0)) branch 000FFEF8");
            AssertCode(0x4200fef8, //"bdnz\t$000FFEF8");
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 0x00000001",
                        "2|T--|if (ctr != 0x00000000) branch 000FFEF8");
            AssertCode(0x4220fef9, //"bdnzl\t$000FFEF8");
                        "0|T--|00100000(4): 3 instructions",
                        "1|L--|ctr = ctr - 0x00000001",
                        "2|T--|if (ctr == 0x00000000) branch 00100004",
                        "3|T--|call 000FFEF8 (0)");
            AssertCode(0x4240fef8, //"bdz\t$000FFEF8");
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 0x00000001",
                        "2|T--|if (ctr == 0x00000000) branch 000FFEF8");
            AssertCode(0x4260fef9, //"bdzl\t$000FFEF8");
                        "0|T--|00100000(4): 3 instructions",
                        "1|L--|ctr = ctr - 0x00000001",
                        "2|T--|if (ctr != 0x00000000) branch 00100004",
                        "3|T--|call 000FFEF8 (0)");
            //AssertCode(0x4280fef8//, "bc+    20,lt,0xffffffffffffff24	 ");
            AssertCode(0x4300fef8, //"bdnz\t$000FFEF8");
                        "0|T--|00100000(4): 2 instructions",
                        "1|L--|ctr = ctr - 0x00000001",
                        "2|T--|if (ctr != 0x00000000) branch 000FFEF8");
        }

        [Test]
        public void PPCRw_rldicl()
        {
            Given_PowerPcBe64();
            AssertCode(0x7863e102, //"rldicl\tr3,r3,3C,04");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 >>u 0x04");
            AssertCode(0x790407c0, //"rldicl\tr4,r8,00,1F");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r8 & 0x00000001FFFFFFFF");
            AssertCode(0x790407E0, //"rldicl\tr4,r8,00,3F");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r8 & 0x0000000000000001");
        }

        [Test]
        public void PPCRw_rldicr()
        {
            Given_PowerPcBe64();
            AssertCode(0x798C0F86, // rldicr\tr12,r12,33,30
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r12 = r12 << 0x21");
        }

        [Test]
        public void PPCRw_rldimi()
        {
            Given_PowerPcBe64();
            AssertCode(0x790A000E,  // rldimi r10,r8,20,00
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = DPB(r10, (word32) r8, 32)");
        }

        [Test]
        public void PPCRw_mftb()
        {
            AssertCode(0x7eac42e6, //"mftb\tr21,0188");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __mftb()");
        }

        [Test]
        public void PPCRw_stvx()
        {
            AssertCode(0x7c2019ce, //"stvx\tv1,r0,r3");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r3:word128] = v1");
        }

        [Test]
        public void PPCRw_stfiwx()
        {
            AssertCode(0x7c004fae, //"stfiwx\tf0,r0,r9");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r9:int32] = (int32) f0");
        }

        [Test]
        public void PPCRw_bnelr()
        {
            AssertCode(0x4c820020, // bnelr	
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,cr0)) branch 00100004",
                "2|T--|return (0,0)");
        }

        [Test]
        public void PPCRw_bdzt()
        {
            Given_750();
            AssertCode(0x41700000,   // bdzt	cr4+lt,$803CDB28
                "0|T--|00100000(4): 2 instructions",
                "1|L--|ctr = ctr - 0x00000001",
                "2|T--|if (ctr == 0x00000000 && Test(LT,cr4)) branch 00100000");
        }

        [Test]
        public void PPCRw_bdzfl()
        {
            Given_750();
            AssertCode(0x40490FDB,   // bdzfl	cr2+gt,$00000FD8
                "0|T--|00100000(4): 3 instructions",
                "1|L--|ctr = ctr - 0x00000001",
                "2|T--|if (ctr != 0x00000000 || Test(GT,cr2)) branch 00100004",
                "3|T--|call 00000FD8 (0)");
        }

        [Test]
        public void PPCRw_bdnztl()
        {
            Given_750();
            AssertCode(0x412F6C6F,   // bdnztl	cr3+so,$00006C6C
                "0|T--|00100000(4): 3 instructions",
                "1|L--|ctr = ctr - 0x00000001",
                "2|T--|if (ctr == 0x00000000 || Test(NO,cr3)) branch 00100004",
                "3|T--|call 00006C6C (0)");
        }
        [Test]
        public void PPCRw_bdztl()
        {
            Given_750();
            AssertCode(0x41747461,   // bdztl	cr5+lt,$80273FB0
                "0|T--|00100000(4): 3 instructions",
                "1|L--|ctr = ctr - 0x00000001",
                "2|T--|if (ctr != 0x00000000 || Test(GE,cr5)) branch 00100004",
                "3|T--|call 00107460 (0)");
        }

        [Test]
        public void PPCRw_fmuls()
        {
            AssertCode(0xec000072, // fmuls   f0,f0,f1
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|f0 = f0 * f1");
        }

        [Test]
        public void PPCRw_Muxx()
        {
            AssertCode(0x7c6b040e, // .long 0x7c6b040e	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = __lvlx(r11, r0)");
        }

        [Test]
        public void PPCRw_vspltw()
        {
            AssertCode(0x10601a8c, // vspltw  v3,v3,0	
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v3 = __vspltw(v3, 0x00000000)");
        }

        [Test]
        public void PPCRw_cntlzd()
        {
            AssertCode(0x7d600074, // cntlzd  r0,r11
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|r0 = __cntlzd(r11)");
        }

        [Test]
        public void PPCRw_vectorops()
        {
            AssertCode(0x10c6600a, //"vaddfp\tv6,v6,v12");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v6 = __vaddfp(v6, v12)");
            AssertCode(0x10000ac6, //"vcmpgtfp\tv0,v0,v1");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v0 = __vcmpgtfp(v0, v1)");
            AssertCode(0x118108c6, //"vcmpeqfp\tv12,v1,v1");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v12 = __vcmpeqfp(v1, v1)");
            AssertCode(0x10ed436e, //"vmaddfp\tv7,v13,v13,v8");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v7 = __vmaddfp(v13, v13, v8)");
            AssertCode(0x10a9426e, //"vmaddfp\tv5,v9,v9,v8");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v5 = __vmaddfp(v9, v9, v8)");
            AssertCode(0x10200a8c, //"vspltw\tv1,v1,0");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v1 = __vspltw(v1, 0x00000000)");
            AssertCode(0x1160094a, //"vrsqrtefp\tv11,v1");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v11 = __vrsqrtefp(v1)");
            AssertCode(0x102bf06f, //"vnmsubfp\tv1,v11,v1,v30");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v1 = __vnmsubfp(v11, v1, v30)");
            AssertCode(0x116b0b2a, //"vsel\tv11,v11,v1,v12");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v11 = __vsel(v11, v1, v12)");
            AssertCode(0x1000002c, //"vsldoi\tv0,v0,v0,0");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v0 = __vsldoi(v0, v0, 0x00000000)");
            AssertCode(0x101f038c, //"vspltisw\tv0,140");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v0 = __vspltisw(-1)");
            AssertCode(0x114948ab, //"vperm\tv10,v9,v9,v2");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v10 = __vperm(v9, v9, v2)");
            AssertCode(0x112c484a, //"vsubfp\tv9,v12,v9");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v9 = __vsubfp(v12, v9)");
            AssertCode(0x118000c6, //"vcmpeqfp\tv12,v0,v0");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v12 = __vcmpeqfp(v0, v0)");
            AssertCode(0x11ad498c, //"vmrglw\tv13,v13,v9");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v13 = __vmrglw(v13, v9)");
            AssertCode(0x118c088c, //"vmrghw\tv12,v12,v1");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v12 = __vmrghw(v12, v1)");
            AssertCode(0x125264c4, //"vxor\tv18,v18,v12");
                          "0|L--|00100000(4): 1 instructions",
                          "1|L--|v18 = v18 ^ v12");
        }

        [Test]
        public void PPCRw_regression4()
        {
            AssertCode(0x10000ac6,//"vcmpgtfp\tv0,v0,v1");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vcmpgtfp(v0, v1)");
            AssertCode(0xec0c5038,//"fmsubs\tf0,f12,f0,f10");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = (real64) ((real32) f12 * (real32) f0 - (real32) f10)");
            AssertCode(0x7c20480c,//"lvsl\tv1,r0,r9");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v1 = __lvsl(r9)");
            AssertCode(0x1000fcc6,//"vcmpeqfp.\tv0,v0,v31");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v0 = __vcmpeqfp(v0, v31)",
                "2|L--|cr6 = cond(v0)");
            AssertCode(0x10c63184,//"vslw\tv6,v6,v6");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v6 = __vslw(v6, v6)");
            AssertCode(0x7c01008e,//"lvewx\tv0,r1,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __lvewx(r1 + r0)");
            AssertCode(0x11a0010a, //"vrefp\tv13,v0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = __vrefp(v0)");
            AssertCode(0x10006e86, //"vcmpgtuw.\tv0,v0,v13");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v0 = __vcmpgtuw(v0, v13)",
                "2|L--|cr6 = cond(v0)");
            AssertCode(0x7c00418e, //"stvewx\tv0,r0,r8");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__stvewx(v0, r8)");
            AssertCode(0x118063ca, //"vctsxs\tv12,v12,00");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v12 = __vctsxs(v12, 0x00000000)");
            AssertCode(0x1020634a, //"vcfsx\tv1,v12,00");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v1 = __vcfsx(v12, 0x00000000)");
            AssertCode(0x118c0404, //"vand\tv12,v12,v0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v12 = v12 & v0");
            AssertCode(0x118c0444, //"vandc\tv12,v12,v0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v12 = v12 & ~v0");
            AssertCode(0x116c5080, //"vadduwm\tv11,v12,v10");
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = v12",
                "2|L--|v6 = v10",
                "3|L--|v11 = __vadduwm(v5, v6)");
            AssertCode(0x11083086, //"vcmpequw\tv8,v8,v6");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v8 = __vcmpequw(v8, v6)");
        }

        [Test]
        public void PPCrw_lfsx()
        {
            AssertCode(0x7c01042e,// "lfsx\tf0,r1,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = (real64) Mem0[r1 + r0:real32]");
        }

        [Test]
        public void PPCRw_mffs()
        {
            AssertCode(0xfc00048e, // "mffs\tf0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = fpscr");
        }

        [Test]
        public void PPCrw_mtfsf()
        {
            AssertCode(0xfdfe058e, //"mtfsf\tFF,f0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__mtfsf(f0, 0x000000FF)");
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
        public void PPCrw_lhaux()
        {
            AssertCode(0x7D2E4AEE,  //"lhaux\tr9,r14,r9");
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r9 = (int32) Mem0[r14 + r9:int16]",
                "2|L--|r9 = r14 + r9");
        }

        [Test]
        public void PPCrw_addme()
        {
            AssertCode(0x7D0301D4,  // "addme\tr8,r3,r0");
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = r3 + cr0 - 0xFFFFFFFF");
        }

        [Test]
        public void PPCRw_lhau()
        {
            AssertCode(0xAD49FFFE, // lhau r10,-2(r9)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r10 = (int32) Mem0[r9 + -2:int16]",
                "2|L--|r10 = r9 + -2");
        }

        [Test]
        public void PPCRw_crnor()
        {
            AssertCode(0x4FDCE042, // crnor 1E,1C,1C
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__crnor(0x0000001E, 0x0000001C, 0x0000001C)");
        }


        [Test]
        public void PPCRw_mtspr()
        {
            AssertCode(0x7C7A03A6, // mtspr 0000340, r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__write_spr(0x0000001A, r3)");
        }

        [Test]
        public void PPCRw_stmw()
        {
            AssertCode(0xBFC10008, // stmw r30,8(r1)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|Mem0[v3:word32] = r30",
                "2|L--|v3 = v3 + 4",
                "3|L--|Mem0[v3:word32] = r31",
                "4|L--|v3 = v3 + 4");
        }

        [Test]
        public void PPCRw_mfmsr()
        {
            AssertCode(0x7C6000A6, // mfmsr r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = __read_msr()");
        }

        [Test]
        public void PPCRw_mtmsr()
        {
            AssertCode(0x7C600124, // mfmsr r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__write_msr(r3)");
        }

        [Test]
        public void PPCrw_rfi()
        {
            AssertCode(0x4C000064, //  rfi
                "0|T--|00100000(4): 2 instructions",
                "1|L--|__write_msr(srr1)",
                "2|T--|goto srr0");
        }

        [Test]
        public void PPCrw_bgtlr()
        {
            AssertCode(0x4D9D0020, // bgtlrcr7
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(LE,cr7)) branch 00100004",
                "2|T--|return (0,0)");
        }

        [Test]
        public void PPCrw_lmw()
        {
            AssertCode(0xBBA1000C, // lmwr29,12(r1)
                "0|L--|00100000(4): 7 instructions",
                "1|L--|v3 = r1 + 12",
                "2|L--|r29 = Mem0[v3:word32]",
                "3|L--|v3 = v3 + 4",
                "4|L--|r30 = Mem0[v3:word32]",
                "5|L--|v3 = v3 + 4",
                "6|L--|r31 = Mem0[v3:word32]",
                "7|L--|v3 = v3 + 4");
        }

        [Test]
        public void PPCRw_mfspr()
        {
            AssertCode(0x7CB0E2A6, // mfspr 0000021C,r5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = __read_spr(0x00000390)");
        }

        [Test]
        public void PPCRw_lhax()
        {
            AssertCode(0x7C84EAAE,  // lhax r4,r4,r29
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = (int32) Mem0[r4 + r29:int16]");
        }

        [Test]
        public void PPCRw_lq()
        {
            Given_PowerPcBe64();
            AssertCode(0xE0030000,   // lq	r0,0(r3)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0_r1 = Mem0[r3 + 0:word128]");
        }

        [Test]
        public void PPCRw_dcbf()
        {
            AssertCode(0x7C0018AC,   // dcbf	r0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dcbf(r3)");
        }

        [Test]
        public void PPCRw_icbi()
        {
            AssertCode(0x7C001FAC,   // icbi	r0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__icbi(r3)");
        }

        [Test]
        public void PPCRw_dcbi()
        {
            AssertCode(0x7C001BAC,   // dcbi	r0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dcbi(r3)");
        }

        [Test]
        public void PPCRw_dcbst()
        {
            AssertCode(0x7C00186C,   // dcbst\tr0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dcbst(r3)");
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
                "1|L--|v5 = Mem0[r11 + r9:word16]",
                "2|L--|r0 = (word32) __swap16(v5)");
        }

        [Test]
        public void PPCRw_stfsx()
        {
            AssertCode(0x7DABF52E,   // stfsx	f13,r11,r30
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + r30:real32] = (real32) f13");
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
                "1|L--|Mem0[r11 + 16:real64] = f0",
                "2|L--|r11 = r11 + 16");
        }

        [Test]
        public void PPCRw_lfdu()
        {
            AssertCode(0xCC0B0010,   // lfdu	f0,16(r11)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f0 = Mem0[r11 + 16:real64]",
                "2|L--|r11 = r11 + 16");
        }

        [Test]
        public void PPCRw_lfsu()
        {
            AssertCode(0xC43D0004,   // lfsu	f1,4(r29)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f1 = (real64) Mem0[r29 + 4:real32]",
                "2|L--|r29 = r29 + 4");
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
                "1|L--|f13 = (real64) -((real32) f12 * (real32) f13 - (real32) f11)");
        }

        [Test]
        public void PPCRw_fsel()
        {
            AssertCode(0xFC0A682E,   // fsel	f0,f10,f0,f13
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f10 >= 0.0 ? f0 : f13");
        }

        [Test]
        public void PPCRw_srd()
        {
            AssertCode(0x7CC84436,   // srd	r8,r6,r8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = r6 >>u r8");
        }

        [Test]
        public void PPCRw_twi()
        {
            AssertCode(0x0CCA0000,   // twi	06,r10,+0000
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r10 >u 0) branch 00100004",
                "2|L--|__trap()");
        }

        [Test]
        public void PPCRw_bltlr()
        {
            AssertCode(0x4D980020,   // bltlr	cr6
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(GE,cr6)) branch 00100004",
                "2|T--|return (0,0)");
        }

        [Test]
        public void PPCRw_lwarx()
        {
            AssertCode(0x7D405828,   // lwarx	r10,r0,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = __lwarx(&Mem0[r11:word32])");
        }

        [Test]
        public void PPCRw_eieio()
        {
            AssertCode(0x7C0006AC,   // eieio
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__eieio()");
        }

        [Test]
        public void PPCRw_stfsu()
        {
            AssertCode(0xD41C0010,   // stfsu	f0,16(r28)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r28 + 16:real32] = (real32) f0",
                "2|L--|r28 = r28 + 16");
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
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(LT,cr6)) branch 00100004",
                "2|T--|return (0,0)");
        }

        [Test]
        public void PPCRw_fsqrt()
        {
            AssertCode(0xFC00002C,   // fsqrt	f0,f0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = sqrt(f0)");
        }

        [Test]
        public void PPCRw_lwzux()
        {
            Given_PowerPcBe64();
            AssertCode(0x7CC9506E,   // lwzux	r6,r9,r10
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r6 = (word64) Mem0[r9 + r10:word32]",
                "2|L--|r9 = r9 + r10");
        }

        [Test]
        public void PPCRw_lvx128()
        {
            AssertCode(0x13E058C7,     // vcmpequd\tv31,v0,v11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = Mem0[r11:word128]");
        }

        [Test]
        public void PPCRw_vmr()
        {
            AssertCode(0x11400484,     // vor\tv10,v0,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = v0");
        }

        [Test]
        public void PPCRw_vor()
        {
            AssertCode(0x11480484,     // vor\tv10,v0,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = v8 | v0");
        }

        [Test]
        [Ignore("reenable when we have switching between PPC models implemented")]
        public void PPCRw_evmhessfaaw()
        {
            AssertCode(0x11A91D03,     // evmhessfaaw\tr13,r9,r3
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = r9",
                "2|L--|v6 = r3",
                "3|L--|r13 = __evmhessfaaw(v5, v6)",
                "4|L--|acc = r13");
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
        public void PPCRw_ldx()
        {
            Given_PowerPcBe64();
            AssertCode(0x7C6BF82A,     // ldx\tr3,r11,r31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = Mem0[r11 + r31:word64]");
        }

        [Test]
        public void PPCRw_ldarx()
        {
            Given_PowerPcBe64();
            AssertCode(0x7D0018A8,     // ldarx\tr8,r0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __ldarx(&Mem0[r3:word64])");
        }

        [Test]
        public void PPCRw_stwcx_()
        {
            AssertCode(0x7D40592D,     // stwcx.\tr10,r0,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr0 = __stwcx(&Mem0[r11:word32], r10)");
        }

        [Test]
        public void PPCRw_mtmsrd()
        {
            AssertCode(0x7DA10164,     // mtmsrd\tr13,01
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__write_msr(r13)");
        }

        [Test]
        public void PPCRw_stdcx_()
        {
            Given_PowerPcBe64();
            AssertCode(0x7D4019AD,     // stdcx.\tr10,r0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|cr0 = __stdcx(&Mem0[r3:word64], r10)");
        }

        [Test]
        public void PPCRw_eqv()
        {
            AssertCode(0x7D6B5238,     // eqv\tr11,r11,r10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = ~(r11 ^ r10)");
        }

        [Test]
        public void PPCRw_lwax()
        {
            Given_PowerPcBe64();
            AssertCode(0x7C8B22AA,     // lwax\tr4,r11,r4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = (int64) Mem0[r11 + r4:int32]");
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
                "1|L--|f0 = (real64) Mem0[r10 + r11:real32]",
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
        public void PPCRw_dcbz()
        {
            AssertCode(0x7C23F7EC,     // dcbz\tr3,r30
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dcbz(r3 + r30)");
        }

        [Test]
        public void PPCRw_vaddubm()
        {
            AssertCode(0x13040000,     // vaddubm\tv24,v4,v0
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = v4",
                "2|L--|v6 = v0",
                "3|L--|v24 = __vaddubm(v5, v6)");
        }

        [Test]
        public void PPCRw_vmaxub()
        {
            AssertCode(0x10011002,     // vmaxub\tv0,v1,v2
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = v1",
                "2|L--|v6 = v2",
                "3|L--|v0 = __vmaxub(v5, v6)");
        }

        [Test]
        [Ignore("reenable when all is happy again.")]
        public void PPCRw_mulhhwu()
        {
            AssertCode(0x10101010,     // mulhhwu\tr0,r16,r2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = (r16 >>u 0x10) *u (r2 >>u 0x10)");
        }

        [Test]
        public void PPCRw_vmladduhm()
        {
            AssertCode(0x10000022,     // vmladduhm\tv0,v0,v0,v0
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = v0",
                "2|L--|v4 = v0",
                "3|L--|v0 = __vmladduhm(v3, v4)");
        }

        [Test]
        public void PPCRw_vmaxuh()
        {
            AssertCode(0x10000042,     // vmaxuh\tv0,v0,v0
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = v0",
                "2|L--|v4 = v0",
                "3|L--|v0 = __vmaxuh(v3, v4)");
        }

        [Test]
        public void PPCRw_vadduqm()
        {
            AssertCode(0x12020100,     // vadduqm\tv16,v2,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v16 = v2 + v0");
        }

        [Test]
        public void PPCRw_vaddubs()
        {
            AssertCode(0x1003c200,     // vaddubs\tv0,v3,v24
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = v3",
                "2|L--|v6 = v24",
                "3|L--|v0 = __vaddubs(v5, v6)");
        }

        [Test]
        public void PPCRw_bcdadd_()
        {
            AssertCode(0x10010401,     // bcdadd.\tv0,v1,v0,00
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __bcdadd(v1, v0)");
        }

        [Test]
        public void PPCRw_vcmpequb()
        {
            AssertCode(0x117d9406,     // vcmpequb.\tv11,v29,v18
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v11 = __vcmpequb(v29, v18)",
                "2|L--|cr6 = cond(v11)");
        }

        [Test]
        public void PPCRw_dcbtst()
        {
            AssertCode(0x7c0019ec,     // dcbtst\tr0,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dcbtst(r3)");
        }

        [Test]
        public void PPCRw_stvrx128()
        {
            AssertCode(0x13E85D47,   // stvrx128	v63,r8,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r8 + r11:word128] = v63");
        }

        [Test]
        public void PPCRw_lvlx128()
        {
            AssertCode(0x13A05C07,   // lvlx128	v61,r0,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v61 = __lvlx(r0, r11)");
        }

        [Test]
        public void PPCRw_vspltw128()
        {
            AssertCode(0x1923CF31,   // vspltw128	v9,v57,03
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v9 = __vspltw(v57, 0x00000003)");
        }

        [Test]
        public void PPCRw_vmsub4fp128()
        {
            AssertCode(0x157FA9F1,   // vmsub4fp128	v11,v63,v53
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = v63",
                "2|L--|v6 = v53",
                "3|L--|v11 = __vmsub4fp(v5, v6)");
        }

        [Test]
        public void PPCRw_stvx128()
        {
            AssertCode(0x116021C3,   // stvx128	v11,r0,r4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r4:word128] = v11");
        }

        [Test]
        public void PPCRw_lvrx128()
        {
            AssertCode(0x13C55C47,   // lvrx128	v62,r5,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v62 = __lvrx(r5, r11)");
        }

        [Test]
        public void PPCRw_vxor128()
        {
            AssertCode(0x145AE331,   // vxor128	v2,v58,v60
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v2 = v58 ^ v60");
        }

        [Test]
        public void PPCRw_vmulfp128()
        {
            AssertCode(0x1497B0B1,   // vmulfp128	v4,v55,v54
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = v55",
                "2|L--|v6 = v54",
                "3|L--|v4 = __vmulfp(v5, v6)");
        }

        [Test]
        public void PPCRw_vslw128()
        {
            AssertCode(0x1B5FF8F5,   // vslw128	v58,v63,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v58 = __vslw(v63, v63)");
        }

        [Test]
        public void PPCRw_vspltisw128()
        {
            AssertCode(0x1B600774,   // vspltisw128	v59,v0,+20
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v59 = __vspltisw(v0)");
        }

        [Test]
        public void PPCRw_vmrghw128()
        {
            AssertCode(0x1B1FF325,   // vmrghw128	v56,v63,v62
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v56 = __vmrghw(v63, v62)");
        }

        [Test]
        public void PPCRw_vmrglw128()
        {
            AssertCode(0x1BFFF365,   // vmrglw128	v63,v63,v62
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = __vmrglw(v63, v62)");
        }

        [Test]
        public void PPCRw_vor128()
        {
            AssertCode(0x15B8C2F1,   // vor128	v13,v56,v56
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = v56");
        }

        [Test]
        public void PPCRw_vupkd3d128()
        {
            AssertCode(0x1B24DFF5,   // vupkd3d128	v57,v59,04
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v57 = __vupkd3d(v59, 0x00000004)");
        }

        [Test]
        public void PPCRw_vrlimi128()
        {
            AssertCode(0x19ACFF91,   // vrlimi128	v13,v63,0C,03
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = __vrlimi(v63, 0x0000000C, 0x00000003)");
        }

        [Test]
        public void PPCRw_tdi()
        {
            Given_PowerPcBe64();
            AssertCode(0x08C40000,   // tdi	06,r4,+0000
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r4 >u 0) branch 00100004",
                "2|L--|__trap()");
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
            AssertCode(0x1AC0FA35,   // vcfpsxws128	v54,v63,+0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v54 = __vcfpsxws(v63, 0)");
        }

        [Test]
        public void PPCRw_vcmpeqfp128()
        {
            Given_Xenon();
            AssertCode(0x187EF823,   // vcmpeqfp128	v3,v62,v127
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v3 = __vcmpeqfp(v62, v127)");
        }

        [Test]
        public void PPCRw_vcmpgtfp128()
        {
            Given_Xenon();
            AssertCode(0x1ABBF925,   // vcmpgtfp128	v53,v59,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v53 = __vcmpgtfp(v59, v63)");
        }

        [Test]
        public void PPCRw_vcsxwfp128()
        {
            Given_Xenon();
            AssertCode(0x1801F2B1,   // vcsxwfp128	v0,v62,01
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vcsxwfp(v62, 0x0000000000000001)");
        }

        [Test]
        public void PPCRw_vexptefp128()
        {
            Given_Xenon();
            AssertCode(0x1BA0EEB5,   // vexptefp128	v61,v61
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v61 = __vexptefp(v61)");
        }

        [Test]
        public void PPCRw_vlogefp128()
        {
            Given_Xenon();
            AssertCode(0x1AA0EEF5,   // vlogefp128	v53,v61
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v53 = __vlogefp(v61)");
        }

        [Test]
        public void PPCRw_vmaddcfp128()
        {
            Given_Xenon();
            AssertCode(0x17DE591C,   // vmaddcfp128	v126,v30,v11
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = v30",
                "2|L--|v6 = v11",
                "3|L--|v126 = __vmaddcfp(v5, v6)");
        }

        [Test]
        public void PPCRw_vmaxfp128()
        {
            Given_Xenon();
            AssertCode(0x1BDEEAA5,   // vmaxfp128	v62,v62,v61
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = v62",
                "2|L--|v5 = v61",
                "3|L--|v62 = __vmaxfp(v4, v5)");
        }

        [Test]
        public void PPCRw_vminfp128()
        {
            Given_Xenon();
            AssertCode(0x1BFFF2E5,   // vminfp128	v63,v63,v62
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = v63",
                "2|L--|v5 = v62",
                "3|L--|v63 = __vminfp(v4, v5)");
        }

        [Test]
        public void PPCRw_vmsub3fp128()
        {
            Given_Xenon();
            AssertCode(0x17E21194,   // vmsub3fp128	v63,v2,v2
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = v2",
                "2|L--|v5 = v2",
                "3|L--|v63 = __vmsub3fp(v4, v5)");
        }

        [Test]
        public void PPCRw_vperm128()
        {
            Given_Xenon();
            AssertCode(0x17FFF025,   // vperm128	v63,v63,v62,v0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = __vperm(v63, v62, v0)");
        }

        [Test]
        public void PPCRw_vpkd3d128()
        {
            Given_Xenon();
            AssertCode(0x1BEDFED7,   // vpkd3d128	v63,v127,03,01,03
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v63 = __vpkd3d(v127, 0x0000000000000003, 0x0000000000000001, 0x0000000000000003)");
        }

        [Test]
        public void PPCRw_vrefp128()
        {
            Given_Xenon();
            AssertCode(0x1800FE31,   // vrefp128	v0,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v0 = __vrefp(v63)");
        }

        [Test]
        public void PPCRw_vrfin128()
        {
            Given_Xenon();
            AssertCode(0x1BC0DB75,   // vrfin128	v62,v59
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v62 = __vrfin(v59)");
        }

        [Test]
        public void PPCRw_vrfiz128()
        {
            Given_Xenon();
            AssertCode(0x1AC0FBF5,   // vrfiz128	v54,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v54 = __vrfiz(v63)");
        }

        [Test]
        public void PPCRw_vrsqrtefp128()
        {
            Given_Xenon();
            AssertCode(0x19A0FE71,   // vrsqrtefp128	v13,v63
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v13 = __vrsqrtefp(v63)");
        }

        [Test]
        public void PPCRw_vsrw128()
        {
            Given_Xenon();
            AssertCode(0x195CB9F1,   // vsrw128	v10,v60,v55
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v10 = __vsrw(v60, v55)");
        }

        [Test]
        public void PPCRw_vsubfp128()
        {
            Given_Xenon();
            AssertCode(0x145D0870,   // vsubfp128	v2,v61,v1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v2 = __vsubfp(v61, v1)");
        }

        [Test]
        public void PPCRw_psq_st()
        {
            Given_750();
            AssertCode(0xF3E10038,   // psq_st	f31,r1,+00000038,01,07
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = f31",
                "2|L--|v4 = __pack_quantized(v3, 0x00000001, 0x00000007)",
                "3|L--|Mem0[r1 + 56:word64] = v4");
        }

        [Test]
        public void PPCRw_psq_stx()
        {
            Given_750();
            AssertCode(0x11C0180E,   // psq_stx	f14,r0,r3,00,07
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = f14",
                "2|L--|v4 = __pack_quantized(v3, 0x00000000, 0x00000007)",
                "3|L--|Mem0[r3:word64] = v4");
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
                "1|L--|v3 = Mem0[r1 + 40:word64]",
                "2|L--|v4 = __unpack_quantized(v3, 0x00000001, 0x00000007)",
                "3|L--|f31 = v4");
        }

        [Test]
        public void PPCRw_psq_lx()
        {
            Given_750();
            AssertCode(0x1009000C,   // psq_lx	f0,r9,r0,00,00
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = Mem0[r9 + r0:word64]",
                "2|L--|v5 = __unpack_quantized(v4, 0x00000000, 0x00000000)",
                "3|L--|f0 = v5");
        }

        [Test]
        public void PPCRw_psq_lux()
        {
            Given_750();
            AssertCode(0x10245F4C,   // psq_lux	f1,r4,r11,01,00
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = Mem0[r4 + r11:word64]",
                "2|L--|v5 = __unpack_quantized(v4, 0x00000001, 0x00000000)",
                "3|L--|f1 = v5",
                "4|L--|r4 = r4 + r11");
        }


        [Test]
        public void PPCRw_fnmadds()
        {
            Given_750();
            AssertCode(0xEC0400FE,   // fnmadds	f0,f4,f0,f3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = (real64) -((real32) f4 * (real32) f0 + (real32) f3)");
        }

        [Test]
        public void PPCRw_ps_add_cr()
        {
            Given_750();
            AssertCode(0x11AA80EB,   // ps_add.	f13,f10,f16
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v5 = f10",
                "2|L--|v6 = f16",
                "3|L--|v7 = __ps_add(v5, v6)",
                "4|L--|f13 = v7",
                "5|L--|cr1 = cond(f13[0])");
        }

        [Test]
        public void PPCRw_ps_rsqrte()
        {
            Given_750();
            AssertCode(0x10228034,   // ps_rsqrte	f1,f16
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = f16",
                "2|L--|v5 = __ps_rsqrte(v4)",
                "3|L--|f1 = v5");
        }

        [Test]
        public void PPCRw_ps_sub()
        {
            Given_750();
            AssertCode(0x10000028,   // ps_sub	f0,f0,f0
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = f0",
                "2|L--|v4 = f0",
                "3|L--|v5 = __ps_sub(v3, v4)",
                "4|L--|f0 = v5");
        }

        [Test]
        public void PPCRw_ps_div()
        {
            Given_750();
            AssertCode(0x102C0024,   // ps_div	f1,f12,f0
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = f12",
                "2|L--|v6 = f0",
                "3|L--|v7 = __ps_div(v5, v6)",
                "4|L--|f1 = v7");
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
                "1|L--|v4 = f13",
                "2|L--|v5 = __ps_neg(v4)",
                "3|L--|f4 = v5");
        }

        [Test]
        public void PPCRw_ps_res()
        {
            Given_750();
            AssertCode(0x10320030,   // ps_res	f1,f0
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = f0",
                "2|L--|v5 = __ps_res(v4)",
                "3|L--|f1 = v5");
        }

        [Test]
        public void PPCRw_ps_madds0()
        {
            Given_750();
            AssertCode(0x1031801D,   // ps_madds0.	f1,f17,f0,f16
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v6 = f17",
                "2|L--|v7 = f0",
                "3|L--|v8 = f16",
                "4|L--|v9 = __ps_madds0(v6, v7, v8)",
                "5|L--|f1 = v9",
                "6|L--|cr1 = cond(f1[0])");
        }

        [Test]
        public void PPCRw_ps_madds1()
        {
            Given_750();
            AssertCode(0x1206029F,   // ps_madds1.	f16,f6,f0,f10
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v6 = f6",
                "2|L--|v7 = f10",
                "3|L--|v8 = f0",
                "4|L--|v9 = __ps_madds1(v6, v7, v8)",
                "5|L--|f16 = v9",
                "6|L--|cr1 = cond(f16[0])");
        }

        [Test]
        public void PPCRw_ps_merge11()
        {
            Given_750();
            AssertCode(0x11D39CE0,   // ps_merge11	f14,f19,f19
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = f19",
                "2|L--|v5 = f19",
                "3|L--|v6 = __ps_merge11(v4, v5)",
                "4|L--|f14 = v6");
        }

        [Test]
        public void PPCRw_ps_muls0()
        {
            Given_750();
            AssertCode(0x12870718,   // ps_muls0	f20,f7,f28
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = f7",
                "2|L--|v6 = f28",
                "3|L--|v7 = __ps_muls0(v5, v6)",
                "4|L--|f20 = v7");
        }

        [Test]
        public void PPCRw_vcmpbfp128()
        {
            Given_750();
            AssertCode(0x196F818F,   // vcmpbfp128	v107,v15,v112
                "0|L--|00100000(4): 1 instructions",
                "1|L--|v107 = __vcmpebfp(v15, v112)");
        }

        [Test]
        public void PPCRw_ps_nmsub()
        {
            Given_750();
            AssertCode(0x1021003C,   // ps_nmsub	f1,f1,f0,f0
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v4 = f1",
                "2|L--|v5 = f0",
                "3|L--|v6 = f0",
                "4|L--|v7 = __ps_nmsub(v4, v5, v6)",
                "5|L--|f1 = v7");
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
                "1|L--|v5 = f13",
                "2|L--|v6 = f0",
                "3|L--|v7 = f0",
                "4|L--|v8 = __ps_sum0(v5, v6, v7)",
                "5|L--|f1 = v8");
        }

        [Test]
        public void PPCRw_ps_madd()
        {
            Given_750();
            AssertCode(0x1065113A,   // ps_madd	f3,f5,f4,f2
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v6 = f5",
                "2|L--|v7 = f4",
                "3|L--|v8 = f2",
                "4|L--|v9 = __ps_madd(v6, v7, v8)",
                "5|L--|f3 = v9");
        }

        [Test]
        public void PPCRw_ps_cmpo0()
        {
            Given_750();
            AssertCode(0x129D0040,   // ps_cmpo0	cr4,f29,f0
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = f29",
                "2|L--|v6 = f0",
                "3|L--|cr4 = __ps_cmpo0(v5, v6)");
        }

        [Test]
        public void PPCRw_ps_sel()
        {
            Given_750();
            AssertCode(0x121153AE,   // ps_sel	f16,f17,f14,f10
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v6 = f17",
                "2|L--|v7 = f14",
                "3|L--|v8 = f10",
                "4|L--|v9 = __ps_sel(v6, v7, v8)",
                "5|L--|f16 = v9");
        }

        [Test]
        public void PPCRw_ps_nabs()
        {
            Given_750();
            AssertCode(0x12008910,   // ps_nabs	f16,f17
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = f17",
                "2|L--|v5 = __ps_nabs(v4)",
                "3|L--|f16 = v5");
        }

        [Test]
        public void PPCRw_bcctrne()
        {
            AssertCode(0x4C820420,  // bcctr\t04,02
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,cr0)) branch 00100004",
                "2|T--|goto ctr");
        }
    }
}
