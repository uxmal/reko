#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.PowerPC;
using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Core.Rtl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcRewriterTests : RewriterTestBase
    {
        private InstructionBuilder b;
        private PowerPcArchitecture arch = new PowerPcArchitecture(PrimitiveType.Word32);
        private IEnumerable<PowerPcInstruction> ppcInstrs;

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return new Address(0x00100000); } }

        private void RunTest(Action<InstructionBuilder> m)
        {
            b = new InstructionBuilder(new Address(0x01000000));
            m(b);
            ppcInstrs = b.Instructions;
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            return new PowerPcRewriter(arch, ppcInstrs, frame, host);
        }

        protected override LoadedImage RewriteCode(uint[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) (w >> 24),
                (byte) (w >> 16),
                (byte) (w >> 8),
                (byte) w
            }).ToArray();
            var image = new LoadedImage(LoadAddress, bytes);
            ppcInstrs = new PowerPcDisassembler(arch, image.CreateBeReader(LoadAddress), PrimitiveType.Word32);
            return image;
        }

        [Test]
        public void PPCRW_Oris()
        {
            RunTest((m) =>
            {
                m.Oris(m.r4, m.r0, 0x1234);
            });
            AssertCode(
                "0|01000000(4): 1 instructions",
                "1|L--|r4 = r0 | 0x12340000");
        }

        [Test]
        public void PPCRW_Add()
        {
            RunTest((m) =>
            {
                m.Add(m.r4, m.r1, m.r3);
            });
            AssertCode(
                "0|01000000(4): 1 instructions",
                "1|L--|r4 = r1 + r3");
        }

        [Test]
        public void PPCRW_Add_()
        {
            RunTest((m) =>
            {
                m.Add_(m.r4, m.r1, m.r3);
            });
            AssertCode(
                "0|01000000(4): 2 instructions",
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
                "0|01000000(4): 2 instructions",
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
                "0|01000000(4): 1 instructions",
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
                "0|01000000(4): 2 instructions",
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
                "0|01000000(4): 2 instructions",
                "1|L--|Mem0[r3 + r0:byte] = (byte) r2",
                "2|L--|r3 = r3 + r0"
                );
        }

        [Test]
        public void PPCRW_mflr()
        {
            Rewrite(0x7C0802A6);
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = lr");
        }

        [Test]
        public void PPCRw_mfcr()
        {
            AssertCode(0x7d800026,
                "0|00100000(4): 1 instructions",
                "1|L--|r12 = cr");
        }

        private void AssertCode(uint instr, params string[] sExp)
        {
            Rewrite(instr);
            AssertCode(sExp);
        }

        [Test]
        public void PPCRw_rlwinm()
        {
            AssertCode(0x57200036, // "rlwinm\tr0,r25,04,00,1B");
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = r25 & 0xFFFFFFF0");
        }

        [Test]
        public void PPCRw_lwzx()
        {
            AssertCode(0x7c9c002e, // "lwzx\tr4,r28,r0");
                "0|00100000(4): 1 instructions",
                "1|L--|r4 = Mem0[r28 + r0:word32]");
        }

        [Test]
        public void PPCRw_stwx()
        {
            AssertCode(0x7c95012e, // "stwx\tr4,r21,r0");
                "0|00100000(4): 1 instructions",
                "1|L--|Mem0[r21 + r0:word32] = r4");
        }

        [Test]
        public void PPCRw_subf()
        {
            AssertCode(0x7c154850, // "subf\tr0,r21,r9");
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = r9 - r21");
        }

        [Test]
        public void PPCRw_srawi()
        {
            AssertCode(0x7c002670, //"srawi\tr0,r0,04");
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = r0 >> 0x00000004");
        }

        [Test]
        public void PPCRw_bctr()
        {
            AssertCode(0x4e800420, //"bcctr\t14,00");
                "0|00100000(4): 1 instructions",
                "1|T--|goto ctr");
        }

        [Test]
        public void PPCRw_stwu()
        {
            AssertCode(0x9521016e, // "stwu\tr9,r1,r0");
                "0|00100000(4): 2 instructions",
                "1|L--|Mem0[r1 + 366:word32] = r9",
                "2|L--|r1 = r1 + 366");
        }

        [Test]
        public void PPCRw_stwux()
        {
            AssertCode(0x7d21016e, // "stwux\tr9,r1,r0");
                "0|00100000(4): 2 instructions",
                "1|L--|Mem0[r1 + r0:word32] = r9",
                "2|L--|r1 = r1 + r0");
        }

        [Test]
        public void PPCRw_fmr()
        {
            AssertCode(0xFFE00890, // "fmr\tf31,f1");
                "0|00100000(4): 1 instructions",
                "1|L--|f31 = f1");
        }

        [Test]
        public void PPCRw_mtctr()
        {
            AssertCode(0x7d0903a6, // "mtctr\tr8");
                "0|00100000(4): 1 instructions",
                "1|L--|ctr = r8");
        }

        [Test]
        public void PPCRw_cmpl()
        {
            AssertCode(0x7f904840, //"cmplw\tcr7,r16,r9");
                "0|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(r16 - r9)");
        }

        [Test]
        public void PPCRw_neg()
        {
            AssertCode(0x7c0000d0, // "neg\tr0,r0");
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = -r0");
        }

        [Test]
        public void PPCRw_cntlzw()
        {
            AssertCode(0x7d4a0034, //"cntlzw\tr10,r10");
                "0|00100000(4): 1 instructions",
                "1|L--|r10 = __cntlzw(r10)");
        }

        [Test]
        public void PPCRw_fsub()
        {
            AssertCode(0xfc21f828, // "fsub\tf1,f1,f31");
                "0|00100000(4): 1 instructions",
                "1|L--|f1 = f1 - f31");
        }

        [Test]
        public void PPCRw_li()
        {
            AssertCode(0x38000000,
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = 0");
        }

        [Test]
        public void PPCRw_addze()
        {
            AssertCode(0x7c000195,// "addze\tr0,r0");
                "0|00100000(4): 3 instructions",
                "1|L--|r0 = r0 + xer",
                "2|L--|cr0 = cond(r0)",
                "3|L--|xer = cond(r0)");
        }

        [Test]
        public void PPCRw_slw()
        {
            AssertCode(0x7d400030, //"slw\tr0,r10,r0");
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = r10 << r0");
        }

        [Test]
        public void PPCRw_fctiwz()
        {
            AssertCode(0xfc00081E, //"fctiwz\tf0,f1");
                "0|00100000(4): 1 instructions",
                "1|L--|f0 = (int32) f1");
        }

        [Test]
        public void PPCRw_fmul()
        {
            AssertCode(0xfc010032,
               "0|00100000(4): 1 instructions",
               "1|L--|f0 = f1 * f0");
        }

        [Test]
        public void PPCRw_fmul_()
        {
            AssertCode(0xfc010033,
                "0|00100000(4): 2 instructions",
                "1|L--|f0 = f1 * f0",
                "2|L--|cr1 = cond(f0)");
        }

        [Test]
        public void PPCRw_fcmpu()
        {
            AssertCode(0xff810000, // , "fcmpu\tcr7,f1,f0");
                "0|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(f1 - f0)");
        }

        [Test]
        public void PPCRw_mtcrf()
        {
            AssertCode(0x7d808120, //"mtcrf\t08,r12");
                "0|00100000(4): 1 instructions",
                "1|L--|__mtcrf(0x00000008, r12)");
        }

        [Test]
        public void PPCRw_bctrl()
        {
            AssertCode(0x4e800421, // "bctrl\t14,00");
                "0|00100000(4): 1 instructions",
                "1|T--|call ctr (0)");
        }

        [Test]
        public void PPCRw_rlwimi()
        {
            AssertCode(0x5120f042, // "rlwimi\tr0,r9,1E,01,01");
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = __rlwimi(r9, 0x0000001E, 0x00000001, 0x00000001)");
        }

        [Test]
        public void PPCRw_bl()
        {
            AssertCode(0x48000045,
                "0|00100000(4): 1 instructions",
                "1|T--|call 00100044 (0)");
        }

        [Test]
        public void PPCRw_addis_r0()
        {
            AssertCode(0x3C000045,
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = 0x00450000");
        }

        [Test]
        public void PPCRw_addis()
        {
            AssertCode(0x3C810045,
                "0|00100000(4): 1 instructions",
                "1|L--|r4 = r1 + 0x00450000");
        }

        [Test]
        public void PPCRw_cmpwi()
        {
            AssertCode(0x2f830005, // 	cmpwi   cr7,r3,5
                "0|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(r3 - 5)");
        }

        [Test]
        public void PPCRw_bcXX()
        {
            //   AssertCode(0x40bc011c, "bge\tcr7,$0010011C");
            //    AssertCode(0x40bd011c, "ble\tcr7,$0010011C");
            AssertCode(0x40be011c, // "bne\tcr7,$0010011C");
                "0|00100000(4): 1 instructions",
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
                "0|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 24:byte] = r0");
        }

        [Test]
        public void PPCRw_blr()
        {
            AssertCode(0x4E800020, // blr
                "0|00100000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void PPCRw_lbz()
        {
            AssertCode(0x8809002a,	//lbz     r0,42(r9)
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = Mem0[r9 + 42:byte]");
        }

        [Test]
        public void PPCRw_crxor()
        {
            AssertCode(0x4cc63182, // crclr   4*cr1+eq	
                "0|00100000(4): 1 instructions",
                "1|L--|__crxor(0x00000006, 0x00000006, 0x00000006)");
        }

        [Test]
        public void PPCRw_not()
        {
            AssertCode(0x7c6318f8, // not     r3,r3	
                "0|00100000(4): 1 instructions",
                "1|L--|r3 = ~r3");
        }

        [Test]
        public void PPCRw_xor_()
        {
            AssertCode(0x7d290279, // xor.    r9,r9,r0	
                "0|00100000(4): 2 instructions",
                "1|L--|r9 = r9 ^ r0",
                "2|L--|cr0 = cond(r9)");
        }

        [Test]
        public void PPCRw_ori()
        {
            AssertCode(0x60000020, // ori     r0,r0,32	
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = r0 | 0x00000020");
        }


        [Test]
        public void PPCRw_rlwinm_1_31_31()
        {
            AssertCode(0x54630ffe, // rlwinm r3,r3,1,31,31
                "0|00100000(4): 1 instructions",
                "1|L--|r3 = r3 >>u 0x1F");
        }

        [Test]
        public void PPCRw_regression_1()
        {
            AssertCode(0xfdad0024, // fdiv    f13,f13,f0
                "0|00100000(4): 1 instructions",
                "1|L--|f13 = f13 / f0");

            AssertCode(0x38a0ffff, // li      r5,-1
                "0|00100000(4): 1 instructions",
                "1|L--|r5 = -1");

            AssertCode(0x575a1838, // rlwinm  r26,r26,3,0,28 
                "0|00100000(4): 1 instructions",
                "1|L--|r26 = r26 << 0x00000003");

            AssertCode(0x7c03db96, // divwu   r0,r3,r27
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = r3 /u r27");

            AssertCode(0x7fe0d9d6, // mullw   r31,r0,r27
                "0|00100000(4): 1 instructions",
                "1|L--|r31 = r0 * r27");

            AssertCode(0x6fde8000, // xoris   r30,r30,32768
                "0|00100000(4): 1 instructions",
                "1|L--|r30 = r30 ^ 0x80000000");

            AssertCode(0x7f891800, // cmpw    cr7,r9,r3	
                "0|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(r9 - r3)");

            AssertCode(0xdbe10038, // stfd    f31,56(r1)	
                "0|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 56:real64] = f31");

            AssertCode(0xc00b821c, // lfs     f0,-32228(r11)	
                "0|00100000(4): 1 instructions",
                "1|L--|f0 = (real64) Mem0[r11 + -32228:real32]");

            AssertCode(0xc8098220, // lfd     f0,-32224(r9)	
                "0|00100000(4): 1 instructions",
                "1|L--|f0 = Mem0[r9 + -32224:real64]");

            AssertCode(0x1f9c008c, // mulli   r28,r28,140	
                "0|00100000(4): 1 instructions",
                "1|L--|r28 = r28 * 140");

            AssertCode(0x7c1ed9ae, // stbx    r0,r30,r27	
                "0|00100000(4): 1 instructions",
                "1|L--|Mem0[r30 + r27:byte] = (byte) r0");

            AssertCode(0xa001001c, // lhz     r0,28(r1)	
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = Mem0[r1 + 28:word16]");

            AssertCode(0x409c0ff0, // bge-   cr7,0x00001004	
                "0|00100000(4): 1 instructions",
                "1|T--|if (Test(GE,cr7)) branch 00100FF0");

            AssertCode(0x2b8300ff, // cmplwi  cr7,r3,255	
                "0|00100000(4): 1 instructions",
                "1|L--|cr7 = cond(r3 - 0x000000FF)");
        }

        [Test]
        public void PPCRw_lbzu()
        {
            AssertCode(0x8D010004, // lbzu
                "0|00100000(4): 2 instructions",
                "1|L--|r8 = Mem0[r1 + 4:byte]",
                "2|L--|r1 = r1 + 4"
                );
        }

        [Test]
        public void PPCRw_sth()
        {
            AssertCode(0xB0920004u, // sth
                "0|00100000(4): 1 instructions",
                "1|L--|Mem0[r18 + 4:word16] = r4"
                );
        }

        [Test]
        public void PPCrw_subfic()
        {
            AssertCode(0x20320100, // subfic
                "0|00100000(4): 1 instructions",
                "1|L--|r1 = 256 - r18");
        }

        [Test]
        public void PPCrw_andis()
        {
            AssertCode(0x74320100, // andis
                "0|00100000(4): 2 instructions",
                "1|L--|r18 = r1 & 0x01000000",
                "2|L--|cr0 = cond(r18)");
        }

        [Test]
        public void PPCrw_fneg_()
        {
            AssertCode(0xfc200051, // "fneg\tf1,f0");
                "0|00100000(4): 2 instructions",
                "1|L--|f1 = -f0",
                "2|L--|cr1 = cond(f1)");
        }

        [Test]
        public void PPCrw_fmadd()
        {
            AssertCode(0xfc0062fa, // "fmadd\tf0,f0,f11,f12");
                "0|00100000(4): 1 instructions",
                "1|L--|f0 = f12 + f0 * f11");
        }

        [Test]
        public void PPCrw_creqv()
        {
            AssertCode(0x4cc63242, // "creqv\t06,06,06");
                "0|00100000(4): 1 instructions",
                "1|L--|__creqv(0x00000006, 0x00000006, 0x00000006)");
        }
        //AssertCode(0x4e080000, "mcrf\tcr4,cr2");

        [Test]
        public void PPCrw_srw()
        {
            AssertCode(0x7c684430, // "srw\tr8,r3,r8");
                "0|00100000(4): 1 instructions",
                "1|L--|r8 = r3 >>u r8");
        }

        [Test]
        public void PPCrw_subfc()
        {
            AssertCode(0x7cd9a810, //"subfc\tr6,r25,r21");
                "0|00100000(4): 2 instructions",
                "1|L--|r6 = r21 - r25",
                "2|L--|xer = cond(r6)");
        }

        [Test]
        public void PPCrw_and()
        {
            AssertCode(0x7c7ef039, //"and.\tr30,r3,r30");
                "0|00100000(4): 2 instructions",
                "1|L--|r30 = r3 & r30",
                "2|L--|cr0 = cond(r30)");
        }

        [Test]
        public void PPCrw_mulhw_()
        {
            AssertCode(0x7ce03897, //"mulhw.\tr7,r0,r7");
                "0|00100000(4): 2 instructions",
                "1|L--|r7 = r0 * r7 >> 0x20",
                "2|L--|cr0 = cond(r7)");
        }

        [Test]
        public void PPCrw_divw()
        {
            AssertCode(0x7d3d03d6, //"divw\tr9,r29,r0");
                "0|00100000(4): 1 instructions",
                "1|L--|r9 = r29 / r0");
        }

        [Test]
        public void PPCrw_lbzux()
        {
            AssertCode(0x7c1ee8ee, // "lbzux\tr0,r30,r29");
                    "0|00100000(4): 2 instructions",
                    "1|L--|r0 = Mem0[r30 + r29:byte]",
                    "2|L--|r30 = r30 + r29");
        }

        [Test]
        public void PPCrw_subfze()
        {
            AssertCode(0x7fde0190, // "subfze\tr30,r30");
                "0|00100000(4): 1 instructions",
                "1|L--|r30 = 0x00000000 - r30 + xer");
        }


        [Test]
        public void PPCrw_subfe()
        {
            AssertCode(0x7c631910, // "subfe\tr3,r3,r3");
                "0|00100000(4): 1 instructions",
                "1|L--|r3 = r3 - r3 + xer");
        }

        [Test]
        public void PPCrw_extsb()
        {
            AssertCode(0x7c000775, //"extsb.\tr0,r0");
                "0|00100000(4): 3 instructions",
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
                "0|00100000(4): 1 instructions",
                "1|L--|f13 = f13 * f11");
        }

        [Test]
        public void PPCrw_regression_2()
        {
            AssertCode(0x4e080000, // mcrf cr4,cr2
                "0|00100000(4): 1 instructions",
                "1|L--|cr4 = cr2");

            AssertCode(0x4e0c0000, // mcrf cr4,cr3
                "0|00100000(4): 1 instructions",
                "1|L--|cr4 = cr3");

            AssertCode(0x7ca35914, // adde r5,r3,r11
                "0|00100000(4): 2 instructions",
                "1|L--|r5 = r3 + r11 + xer",
                "2|L--|xer = cond(r5)");

            AssertCode(0x7e601c2c, // lwbrx r19,0,r3
                 "0|00100000(4): 1 instructions",
                 "1|L--|r19 = __reverse_bytes_32(Mem0[r3:word32])");
            AssertCode(0x7c00252c, // stwbrx r0,0,r4
                 "0|00100000(4): 1 instructions",
                 "1|L--|Mem0[r4:word32] = __reverse_bytes_32(r0)");
        }

        [Test]
        public void PPCrw_sthx()
        {
            AssertCode(0x7c1b1b2e, //	sthx    r0,r27,r3
                "0|00100000(4): 1 instructions",
                "1|L--|Mem0[r27 + r3:word16] = (word16) r0");
        }

        [Test]
        public void PPCrw_nand()
        {
            AssertCode(0x7d6043b8, //	nand    r0,r11,r8	
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = ~(r11 & r8)");
        }

        [Test]
        public void PPCrw_orc()
        {
            AssertCode(0x7d105b38, // orc     r16,r8,r11
                "0|00100000(4): 1 instructions",
                "1|L--|r16 = r8 | ~r11");
        }

        [Test]
        public void PPCrw_bclr_getPC()
        {
            AssertCode(0x429f0005, // bcl-     20,cr7,xxxx
                "0|00100000(4): 1 instructions",
                "1|L--|lr = 00100004");
        }

        [Test]
        public void PPCrw_std()
        {
            AssertCode(0xf8410028, // "std\tr2,40(r1)");
                "0|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 40:word64] = r2");
        }

        [Test]
        public void PPCrw_stdu()
        {
            AssertCode(0xf8410029, //	stdu    r2,40(r1))"
                "0|00100000(4): 2 instructions",
                "1|L--|Mem0[r1 + 40:word64] = (word64) r2",
                "2|L--|r1 = r1 + 40");
        }
    }
}
