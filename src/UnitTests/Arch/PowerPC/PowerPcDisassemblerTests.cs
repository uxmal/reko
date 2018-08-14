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

using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcDisassemblerTests : DisassemblerTestBase<PowerPcInstruction>
    {
        private PowerPcArchitecture arch;

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        private PowerPcInstruction DisassembleX(uint op, uint rs, uint ra, uint rb, uint xo, uint rc)
        {
            uint w =
                (op << 26) |
                (rs << 21) |
                (ra << 16) |
                (rb << 11) |
                (xo << 1) |
                rc;
            MemoryArea img = new MemoryArea(Address.Ptr32(0x00100000), new byte[4]);
            img.WriteBeUInt32(0, w);
            return Disassemble(img);
        }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new BeImageWriter(bytes);
        }

        private void RunTest(string expected, string bits)
        {
            var instr = DisassembleBits(bits);
            Assert.AreEqual(expected, instr.ToString());
        }

        [SetUp]
        public void Setup()
        {
            this.arch = new PowerPcBe32Architecture("ppc-be-32");
        }

        private void Given_PowerPcBe64()
        {
            this.arch = new PowerPcBe64Architecture("ppc-be-32");
        }

        [Test]
        public void PPCDis_IllegalOpcode()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 00, 00, 00, 00 });
            Assert.AreEqual(Opcode.illegal, instr.Opcode);
        }

        [Test]
        public void PPCDis_Ori()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 0x60, 0x1F, 0x44, 0x44 });
            Assert.AreEqual(Opcode.ori, instr.Opcode);
            Assert.AreEqual(3, instr.Operands);
            Assert.AreEqual("ori\tr31,r0,4444", instr.ToString());
        }

        [Test]
        public void PPCDis_Oris()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 0x64, 0x1F, 0x44, 0x44 });
            Assert.AreEqual("oris\tr31,r0,4444", instr.ToString());
        }

        [Test]
        public void PPCDis_Addi()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 0x38, 0x1F, 0xFF, 0xFC });
            Assert.AreEqual("addi\tr0,r31,-0004", instr.ToString());
        }

        [Test]
        public void PPCDis_Or()
        {
            PowerPcInstruction instr = DisassembleX(31, 2, 1, 3, 444, 0);
            Assert.AreEqual("or\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void PPCDis_Or_()
        {
            PowerPcInstruction instr = DisassembleX(31, 2, 1, 3, 444, 1);
            Assert.AreEqual("or.\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void PPCDis_b()
        {
            var instr = DisassembleWord(0x48000008);
            Assert.AreEqual("b\t$00100008", instr.ToString());
        }

        [Test]
        public void PPCDis_b_absolute()
        {
            var instr = DisassembleWord(0x4800000A);
            Assert.AreEqual("b\t$00000008", instr.ToString());
        }

        [Test]
        public void PPCDis_bl()
        {
            var instr = DisassembleWord(0x4BFFFFFD);
            Assert.AreEqual("bl\t$000FFFFC", instr.ToString());
        }

        [Test]
        public void PPCDis_bcxx()
        {
            var instr = DisassembleWord(0x4000FFF0);
            Assert.AreEqual("bdnzf\tlt,$000FFFF0", instr.ToString());
        }

        [Test]
        public void PPCDis_mtlr()
        {
            var instr = DisassembleWord(0x7C0803A6);
            Assert.AreEqual("mtlr\tr0", instr.ToString());
        }

        [Test]
        public void PPCDis_blr()
        {
            var instr = DisassembleWord(0x4e800020);
            Assert.AreEqual("blr", instr.ToString());
        }

        [Test]
        public void PPCDis_lwz()
        {
            var instr = DisassembleWord(0x803F0005);
            Assert.AreEqual("lwz\tr1,5(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stw()
        {
            var instr = DisassembleWord(0x903FFFF8);
            Assert.AreEqual("stw\tr1,-8(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stwu()
        {
            var instr = DisassembleWord(0x943F0005);
            Assert.AreEqual("stwu\tr1,5(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stb()
        {
            var instr = DisassembleWord(0x9A3FFE00);
            Assert.AreEqual("stb\tr17,-512(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_lhzx()
        {
            var instr = DisassembleWord(0x7F04022E);
            Assert.AreEqual("lhzx\tr24,r4,r0", instr.ToString());
        }

        [Test]
        public void PPCDis_lbzx()
        {
            var instr = DisassembleWord(0x7F0408AE);
            Assert.AreEqual("lbzx\tr24,r4,r1", instr.ToString());
        }

        [Test]
        public void PPCDis_mulli()
        {
            var instr = DisassembleWord(0x1F1F0003);
            Assert.AreEqual("mulli\tr24,r31,+0003", instr.ToString());
        }

        [Test]
        public void PPCDis_fadd()
        {
            var instr = DisassembleWord(0xFFF4402A);
            Assert.AreEqual("fadd\tf31,f20,f8", instr.ToString());
        }

        [Test]
        public void PPCDis_lbz()
        {
            var instr = DisassembleWord(0x88010203);
            Assert.AreEqual("lbz\tr0,515(r1)", instr.ToString());
        }

        [Test]
        public void PPCDis_stbux()
        {
            var instr = DisassembleWord(0x7CAA01EE);
            Assert.AreEqual("stbux\tr5,r10,r0", instr.ToString());
        }

        [Test]
        public void PPCDis_stbu()
        {
            var instr = DisassembleWord(0x9C01FFEE);
            Assert.AreEqual("stbu\tr0,-18(r1)", instr.ToString());
        }

        [Test]
        public void PPCDis_lwarx()
        {
            var instr = DisassembleWord(0x7C720028);
            Assert.AreEqual("lwarx\tr3,r18,r0", instr.ToString());
        }

        [Test]
        public void PPCDis_lbzu()
        {
            var instr = DisassembleWord(0x8C0A0000);
            Assert.AreEqual("lbzu\tr0,0(r10)", instr.ToString());
        }

        [Test]
        public void PPCDis_xori()
        {
            var instr = DisassembleBits("011010 00001 00011 0101010101010101");
            Assert.AreEqual("xori\tr3,r1,5555", instr.ToString());
        }

        [Test]
        public void PPCDis_xoris()
        {
            var instr = DisassembleBits("011011 00001 00011 0101010101010101");
            Assert.AreEqual("xoris\tr3,r1,5555", instr.ToString());
        }

        [Test]
        public void PPCDis_xor_()
        {
            var instr = DisassembleBits("011111 00010 00001 00011 0100111100 1");
            Assert.AreEqual("xor.\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void PPCDis_lhz()
        {
            var instr = DisassembleBits("101000 00010 00001 1111111111111000");
            Assert.AreEqual("lhz\tr2,-8(r1)", instr.ToString());
        }

        [Test]
        public void PPCDis_twi()
        {
            var instr = DisassembleBits("000011 00010 00001 1111111111111000");
            Assert.AreEqual("twi\t02,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_subfic()
        {
            var instr = DisassembleBits("001000 00010 00001 1111111111111000");
            Assert.AreEqual("subfic\tr2,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_cmplwi()
        {
            var instr = DisassembleBits("001010 00010 00001 1111111111111000");
            Assert.AreEqual("cmplwi\tcr0,r1,FFF8", instr.ToString());
        }

        [Test]
        public void PPCDis_cmpi()
        {
            var instr = DisassembleBits("001011 00010 00001 1111111111111000");
            Assert.AreEqual("cmpwi\tcr0,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_addic()
        {
            var instr = DisassembleBits("001100 00010 00001 1111111111111000");
            Assert.AreEqual("addic\tr2,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_addic_()
        {
            var instr = DisassembleBits("001101 00010 00001 1111111111111000");
            Assert.AreEqual("addic.\tr2,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_sc()
        {
            var instr = DisassembleBits("010001 00010 00000 0000000000000010");
            Assert.AreEqual("sc", instr.ToString());
        }

        [Test]
        public void PPCDis_crnor()
        {
            var instr = DisassembleBits("010011 00001 00010 00011 00001000010");
            Assert.AreEqual("crnor\t01,02,03", instr.ToString());
        }

        [Test]
        public void PPCDis_cror()
        {
            var instr = DisassembleBits("010011 00001 00010 00011 01110000010");
            Assert.AreEqual("cror\t01,02,03", instr.ToString());
        }

        [Test]
        public void PPCDis_rfi()
        {
            var instr = DisassembleBits("010011 00000 00000 00000 0 0001100100");
            Assert.AreEqual("rfi", instr.ToString());
        }

        [Test]
        public void PPCDis_andi_()
        {
            var instr = DisassembleBits("011100 00001 00011 1111110001100100");
            Assert.AreEqual("andi.\tr3,r1,FC64", instr.ToString());
        }

        [Test]
        public void PPCDis_andis_()
        {
            var instr = DisassembleBits("011101 00001 00011 1111110001100100");
            Assert.AreEqual("andis.\tr3,r1,FC64", instr.ToString());
        }

        [Test]
        public void PPCDis_cmp()
        {
            var instr = DisassembleBits("011111 01100 00001 00010 0000000000 0");
            Assert.AreEqual("cmp\tcr3,r1,r2", instr.ToString());
        }

        [Test]
        public void PPCDis_tw()
        {
            var instr = DisassembleBits("011111 01100 00001 00010 0000000100 0");
            Assert.AreEqual("tw\t0C,r1,r2", instr.ToString());
        }

        [Test]
        public void PPCDis_lmw()
        {
            var instr = DisassembleBits("101110 00001 00010 111111111110100 0");
            Assert.AreEqual("lmw\tr1,-24(r2)", instr.ToString());
        }

        [Test]
        public void PPCDis_stmw()
        {
            var instr = DisassembleBits("101111 00001 00010 111111111110100 0");
            Assert.AreEqual("stmw\tr1,-24(r2)", instr.ToString());
        }

        [Test]
        public void PPCDis_lfs()
        {
            var instr = DisassembleBits("110000 00001 00010 111111111110100 0");
            Assert.AreEqual("lfs\tf1,-24(r2)", instr.ToString());
        }

        [Test]
        public void PPCDis_unknown()
        {
            var instr = base.DisassembleWord(0xEC6729D4);
            Assert.AreEqual("illegal", instr.ToString());
        }

        [Test]
        public void PPCDis_fpu_single_precision_instructions()
        {
            RunTest("fdivs.\tf1,f2,f3", "111011 00001 00010 00011 00000 10010 1");
            RunTest("fsubs.\tf1,f2,f3", "111011 00001 00010 00011 00000 10100 1");
            RunTest("fadds.\tf1,f2,f3", "111011 00001 00010 00011 00000 10101 1");
            RunTest("fsqrts.\tf1,f3", "111011 00001 00010 00011 00000 10110 1");
            RunTest("fres.\tf1,f3", "111011 00001 00010 00011 00000 11000 1");
            RunTest("fmuls.\tf1,f2,f4", "111011 00001 00010 00000 00100 11001 1");
            RunTest("fmsubs.\tf1,f2,f4,f3", "111011 00001 00010 00011 00100 11100 1");
            RunTest("fmadds.\tf1,f2,f4,f3", "111011 00001 00010 00011 00100 11101 1");
            RunTest("fnmsubs.\tf1,f2,f3,f4", "111011 00001 00010 00011 00100 11110 1");
            RunTest("fnmadds.\tf1,f2,f3,f4", "111011 00001 00010 00011 00100 11111 1");
        }

        [Test]
        public void PPCDis_mflr()
        {
            var instr = DisassembleWord(0x7C0802A6);
            Assert.AreEqual("mflr\tr0", instr.ToString());
        }

        [Test]
        public void PPCDis_add()
        {
            var instr = DisassembleWord(0x7c9a2214);
            Assert.AreEqual("add\tr4,r26,r4", instr.ToString());
        }

        [Test]
        public void PPCDis_mfcr()
        {
            var instr = DisassembleWord(0x7d800026);
            Assert.AreEqual("mfcr\tr12", instr.ToString());
        }

        private void AssertCode(uint instr, string sExp)
        {
            var i = DisassembleWord(instr);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void PPCDis_rlwinm()
        {
            AssertCode(0x5729103a, "rlwinm\tr9,r25,02,00,1D");
            AssertCode(0x57202036, "rlwinm\tr0,r25,04,00,1B");
            AssertCode(0x556A06F7, "rlwinm.\tr10,r11,00,1B,1B");
        }

        [Test]
        public void PPCDis_lwzx()
        {
            AssertCode(0x7c9c002e, "lwzx\tr4,r28,r0");
        }

        [Test]
        public void PPCDis_stwx()
        {
            AssertCode(0x7c95012e, "stwx\tr4,r21,r0");
        }

        [Test]
        public void PPCDis_subf()
        {
            AssertCode(0x7c154850, "subf\tr0,r21,r9");
        }

        [Test]
        public void PPCDis_srawi()
        {
            AssertCode(0x7c002670, "srawi\tr0,r0,04");
        }

        [Test]
        public void PPCDis_bctr()
        {
            AssertCode(0x4e800420, "bcctr\t14,00");
        }

        [Test]
        public void PPCDis_stwux()
        {
            AssertCode(0x7d21016e, "stwux\tr9,r1,r0");
        }

        [Test]
        public void PPCDis_fmr()
        {
            AssertCode(0xFFE00890, "fmr\tf31,f1");
        }

        [Test]
        public void PPCDis_mtctr()
        {
            AssertCode(0x7d0903a6, "mtctr\tr8");
        }

        [Test]
        public void PPCDis_cmpl()
        {
            AssertCode(0x7f904840, "cmplw\tcr7,r16,r9");
        }

        [Test]
        public void PPCDis_neg()
        {
            AssertCode(0x7c0000d0, "neg\tr0,r0");
        }

        [Test]
        public void PPCDis_cntlzw()
        {
            AssertCode(0x7d4a0034, "cntlzw\tr10,r10");
        }

        [Test]
        public void PPCDis_fsub()
        {
            AssertCode(0xfc21f828, "fsub\tf1,f1,f31");
        }

        [Test]
        public void PPCDis_li()
        {
            AssertCode(0x38000000, "addi\tr0,r0,+0000");
        }

        [Test]
        public void PPCDis_addze()
        {
            AssertCode(0x7c000194, "addze\tr0,r0");
        }

        [Test]
        public void PPCDis_slw()
        {
            AssertCode(0x7d400030, "slw\tr0,r10,r0");
        }

        [Test]
        public void PPCDis_fctiwz()
        {
            AssertCode(0xfc00081e, "fctiwz\tf0,f1");
        }
        [Test]
        public void PPCDis_fmul()
        {
            AssertCode(0xfc010032, "fmul\tf0,f1,f0");
        }
        [Test]
        public void PPCDis_fcmpu()
        {
            AssertCode(0xff810000, "fcmpu\tcr7,f1,f0");
        }
        [Test]
        public void PPCDis_mtcrf()
        {
            AssertCode(0x7d808120, "mtcrf\t08,r12");
        }

        [Test]
        public void PPCDis_bctrl()
        {
            AssertCode(0x4e800421, "bctrl\t14,00");
        }

        [Test]
        public void PPCDis_rlwimi()
        {
            AssertCode(0x5120f042, "rlwimi\tr0,r9,1E,01,01");
        }



        [Test]
        public void PPCDis_cror_2()
        {
            AssertCode(0x4fddf382, "cror\t1E,1D,1E");
        }

        [Test]
        public void PPCDis_add_()
        {
            var instr = DisassembleWord(0x7c9a2215);
            Assert.AreEqual("add.\tr4,r26,r4", instr.ToString());
        }

        [Test]
        public void PPCDis_cmpwi()
        {
            AssertCode(0x2f830005, "cmpwi\tcr7,r3,+0005");
        }

        [Test]
        public void PPCDis_bcXX()
        {
            AssertCode(0x40bc011c, "bge\tcr7,$0010011C");
            AssertCode(0x40bd011c, "ble\tcr7,$0010011C");
            AssertCode(0x40be011c, "bne\tcr7,$0010011C");
            AssertCode(0x40bf011c, "bns\tcr7,$0010011C");
            AssertCode(0x41bc011c, "blt\tcr7,$0010011C");
            AssertCode(0x41bd011c, "bgt\tcr7,$0010011C");
            AssertCode(0x41be011c, "beq\tcr7,$0010011C");
            AssertCode(0x41bf011c, "bso\tcr7,$0010011C");
        }

        [Test]
        public void PPCDis_nor()
        {
            AssertCode(0x7c6318f8, "nor\tr3,r3,r3");
        }

        [Test]
        public void PPCDis_regression3()
        {
            AssertCode(0xfc000050, "fneg\tf0,f0");
            AssertCode(0xfc0062fa, "fmadd\tf0,f0,f11,f12");
            AssertCode(0x4cc63242, "creqv\t06,06,06");
            //AssertCode(0x4e080000, "mcrf\tcr4,cr2");
            AssertCode(0x7c684430, "srw\tr8,r3,r8");
            AssertCode(0x7cd9a810, "subfc\tr6,r25,r21");
            AssertCode(0x7c7ef038, "and\tr30,r3,r30");
            AssertCode(0x7ce03896, "mulhw\tr7,r0,r7");
            AssertCode(0x7d5be016, "mulhwu\tr10,r27,r28");
            AssertCode(0x7d3d03d6, "divw\tr9,r29,r0");
            AssertCode(0x7fda0016, "mulhwu\tr30,r26,r0");
            AssertCode(0x7c1ee8ee, "lbzux\tr0,r30,r29");
            AssertCode(0x7c0bd039, "and.\tr11,r0,r26");
            AssertCode(0x7fde0190, "subfze\tr30,r30");
            AssertCode(0x7c03fbd6, "divw\tr0,r3,r31");
            AssertCode(0x7c040096, "mulhw\tr0,r4,r0");
            AssertCode(0x7c000774, "extsb\tr0,r0");
            AssertCode(0x7c00252c, "stwbrx\tr0,r0,r4");
            AssertCode(0x7d080190, "subfze\tr8,r8");
            AssertCode(0x7d4a5110, "subfe\tr10,r10,r10");
            AssertCode(0x7c000775, "extsb.\tr0,r0");
            AssertCode(0x7c631910, "subfe\tr3,r3,r3");
            AssertCode(0x7c880039, "and.\tr8,r4,r0");
            AssertCode(0x7d605896, "mulhw\tr11,r0,r11");
            AssertCode(0x7e310038, "and\tr17,r17,r0");
            AssertCode(0x7e601c2c, "lwbrx\tr19,r0,r3");
            AssertCode(0xfdad02f2, "fmul\tf13,f13,f11");
        }

        [Test]
        public void PPCDis_sraw()
        {
            AssertCode(0x7c052e30, "sraw\tr5,r0,r5");
        }

        [Test]
        public void PPCDis_64bit()
        {
            AssertCode(0xf8410028, "std\tr2,40(r1)");
            AssertCode(0xebe10078, "ld\tr31,120(r1)");
            AssertCode(0x7fa307b4, "extsw\tr3,r29");
        }

        [Test]
        public void PPCDis_rldicl()
        {
            Given_PowerPcBe64();
            AssertCode(0x790407c0, "rldicl\tr4,r8,00,1F");
            AssertCode(0x790407E0, "rldicl\tr4,r8,00,3F");
            AssertCode(0x7863e102, "rldicl\tr3,r3,3C,04");
        }

        [Test]
        public void PPCDis_More64()
        {
            AssertCode(0xfd600018, "frsp\tf11,f0");
            AssertCode(0xec1f07ba, "fmadds\tf0,f31,f30,f0");
            AssertCode(0xec216824, "fdivs\tf1,f1,f13");
            AssertCode(0x7c4048ce, "lvx\tv2,r0,r9");
            AssertCode(0x4d9e0020, "beqlr\tcr7");
            AssertCode(0x10601a8c, "vspltw\tv3,v3,00");
            AssertCode(0x100004c4, "vxor\tv0,v0,v0");
            AssertCode(0x5c00c03e, "rlwnm\tr0,r0,r24,00,1F");
            AssertCode(0x4c9d0020, "blelr\tcr7");
            AssertCode(0x7c00222c, "dcbt\tr0,r4,00");
            AssertCode(0x7c0004ac, "sync");
            AssertCode(0x7c00f078, "andc\tr0,r0,r30");
            AssertCode(0x7c005836, "sld\tr0,r0,r11");
            AssertCode(0x7c0bfe76, "sradi\tr11,r0,3F");
            AssertCode(0x7c0a31d2, "mulld\tr0,r10,r6");
            AssertCode(0x7c07492a, "stdx\tr0,r7,r9");
        }

        [Test]
        public void PPCDis_lvlx()
        {
            AssertCode(0x7c6b040e, "lvlx\tr3,r11,r0");
        }

        [Test]
        public void PPCDis_bcctr()
        {
            AssertCode(0x4000fef8, "bdnzf\tlt,$000FFEF8");
            AssertCode(0x4040fef8, "bdzf\tlt,$000FFEF8");
            AssertCode(0x4080fef8, "bge\t$000FFEF8");
            AssertCode(0x4100fef8, "bdnzt\tlt,$000FFEF8");
            AssertCode(0x4180fef8, "blt\t$000FFEF8");

            AssertCode(0x4200fef8, "bdnz\t$000FFEF8");
            AssertCode(0x4220fef9, "bdnzl\t$000FFEF8");
            AssertCode(0x4240fef8, "bdz\t$000FFEF8");
            AssertCode(0x4260fef9, "bdzl\t$000FFEF8");
            //AssertCode(0x4280fef8, "bc+    20,lt,0xffffffffffffff24	 ");
            AssertCode(0x4300fef8, "bdnz\t$000FFEF8");
        }

        [Test]
        public void PPCDis_mftb()
        {
            AssertCode(0x7eac42e6, "mftb\tr21,0188");
        }

        [Test]
        public void PPCDis_stvx()
        {
            AssertCode(0x7c2019ce, "stvx\tv1,r0,r3");
        }

        [Test]
        public void PPCDis_stfiwx()
        {
            AssertCode(0x7c004fae, "stfiwx\tf0,r0,r9");
        }

        [Test]
        public void PPCDis_cntlzd()
        {
            AssertCode(0x7d600074, "cntlzd\tr0,r11");
        }

        [Test]
        public void PPCDis_vectorops()
        {
            AssertCode(0x10c6600a, "vaddfp\tv6,v6,v12");
            AssertCode(0x10000ac6, "vcmpgtfp\tv0,v0,v1");
            AssertCode(0x118108c6, "vcmpeqfp\tv12,v1,v1");
            AssertCode(0x10ed436e, "vmaddfp\tv7,v13,v13,v8");
            AssertCode(0x10a9426e, "vmaddfp\tv5,v9,v9,v8");
            AssertCode(0x10200a8c, "vspltw\tv1,v1,00");
            AssertCode(0x1160094a, "vrsqrtefp\tv11,v1");
            AssertCode(0x102b406e, "vmaddfp\tv1,v11,v1,v8");
            AssertCode(0x102bf06f, "vnmsubfp\tv1,v11,v1,v30");
            AssertCode(0x1020014a, "vrsqrtefp\tv1,v0");
            AssertCode(0x116b0b2a, "vsel\tv11,v11,v1,v12");
            AssertCode(0x1000012c, "vsldoi\tv0,v0,v0,04");
            AssertCode(0x101f038c, "vspltisw\tv0,-01");
            AssertCode(0x1000028c, "vspltw\tv0,v0,00");
            AssertCode(0x114948ab, "vperm\tv10,v9,v9,v2");
            AssertCode(0x112c484a, "vsubfp\tv9,v12,v9");
            AssertCode(0x118000c6, "vcmpeqfp\tv12,v0,v0");
            AssertCode(0x11ad498c, "vmrglw\tv13,v13,v9");
            AssertCode(0x118c088c, "vmrghw\tv12,v12,v1");
            AssertCode(0x125264c4, "vxor\tv18,v18,v12");
        }

        [Test]
        public void PPCDis_regression4()
        {
            //AssertCode(0x10000ac6, "vcmpgtfp\tv0,v0,v1");
            AssertCode(0xec0c5038, "fmsubs\tf0,f12,f0,f10");
            AssertCode(0x7c20480c, "lvsl\tv1,r0,r9");
            AssertCode(0x1000fcc6, "vcmpeqfp.\tv0,v0,v31");
            AssertCode(0x10c63184, "vslw\tv6,v6,v6");
            AssertCode(0x10e73984, "vslw\tv7,v7,v7");
            AssertCode(0x7c01008e, "lvewx\tv0,r1,r0");

            AssertCode(0x11a0010a, "vrefp\tv13,v0");
            AssertCode(0x10006e86, "vcmpgtuw.\tv0,v0,v13");
            AssertCode(0x7c00418e, "stvewx\tv0,r0,r8");
            AssertCode(0x118063ca, "vctsxs\tv12,v12,00");
            AssertCode(0x1020634a, "vcfsx\tv1,v12,00");
            AssertCode(0x118c0404, "vand\tv12,v12,v0");
            AssertCode(0x116c5080, "vadduwm\tv11,v12,v10");
            AssertCode(0x110c5404, "vand\tv8,v12,v10");
            AssertCode(0x1021ac44, "vandc\tv1,v1,v21");
            AssertCode(0x11083086, "vcmpequw\tv8,v8,v6");
        }

        [Test]
        public void PPCDis_lfsx()
        {
            AssertCode(0x7c01042e, "lfsx\tf0,r1,r0");
        }

        [Test]
        public void PPCDis_mffs()
        {
            AssertCode(0xfc00048e, "mffs\tf0");
        }

        [Test]
        public void PPCDis_mtfsf()
        {
            AssertCode(0xfdfe058e, "mtfsf\tFF,f0");
        }

        [Test]
        public void PPCDis_regression5()
        {
            AssertCode(0x7D2E4AEE, "lhaux\tr9,r14,r9");
            AssertCode(0x7D0301D4, "addme\tr8,r3");
        }

        [Test]
        public void PPCDis_regression6()
        {
            AssertCode(0x7C6000A6, "mfmsr\tr3");
            AssertCode(0x7C7A03A6, "mtspr\t0000001A,r3");
            AssertCode(0x7C600124, "mtmsr\tr3,00");
            AssertCode(0x4C00012C, "isync");
        }

        [Test]
        public void PPCDis_regression7()
        {
            AssertCode(0x7CA464AA, "lswi\tr5,r4,0C");
            AssertCode(0x7CA965AA, "stswi\tr5,r9,0C");
            AssertCode(0x7C0018AC, "dcbf\tr0,r3");
            //AssertCode(0x7c00188c, ".long 0x7c00188c");
            AssertCode(0xE0030000, "lq\tr0,0(r3)");
            AssertCode(0xF0090000, "xsaddsp\tv0,v9,v0");
            AssertCode(0x7D0B506E, "lwzux\tr8,r11,r10");

            AssertCode(0x7c001fac, "icbi\tr0,r3");
            AssertCode(0x7c001bac, "dcbi\tr0,r3");
            AssertCode(0x7c0006ac, "eieio");
            AssertCode(0x7c0b4e2c, "lhbrx\tr0,r11,r9");
            AssertCode(0x7fa65aae, "lhax\tr29,r6,r11");
            AssertCode(0xf0030008, "xsmaddasp\tv0,v3,v0");
            //AssertCode(0x10400c60, "vmhaddshs\tv2,v0,v1,v17");
            //AssertCode(0xf0030018, "psq_st\tf0,24(r3),0,0");
            //AssertCode(0xF0430010, "xxsldwi\tvs2,vs3,vs0,0");
            AssertCode(0x7C0534AE, "lfdx\tf0,r5,r6");
            AssertCode(0x7D6525AE, "stfdx\tf11,r5,r4");
            AssertCode(0x7C3DF52E, "stfsx\tf1,r29,r30");
            AssertCode(0x7DAB4D6E, "stfsux\tf13,r11,r9");
            AssertCode(0x7C00186C, "dcbst\tr0,r3");
        }

        [Test]
        public void PPCDis_regression8()
        {
            AssertCode(0x13E058C7, "lvx128\tv63,r0,r11");
            AssertCode(0x11400484, "vor\tv10,v0,v0");

            AssertCode(0x11A91D03, "stvlx128\tv13,r9,r3");
            AssertCode(0x1001350B, "stvlx128\tv64,r1,r6");
            AssertCode(0x7C6BF82A, "ldx\tr3,r11,r31");
            AssertCode(0x7D0018A8, "ldarx\tr8,r0,r3");
            AssertCode(0x7D40592D, "stwcx.\tr10,r0,r11");
            AssertCode(0x7DA10164, "mtmsrd\tr13,01");
            AssertCode(0x7D4019AD, "stdcx.\tr10,r0,r3");
            AssertCode(0x7D6B5238, "eqv\tr11,r11,r10");
            AssertCode(0x7C8B22AA, "lwax\tr4,r11,r4");
            AssertCode(0x7D6B5392, "divdu\tr11,r11,r10");
            AssertCode(0x7D2943D2, "divd\tr9,r9,r8");
            AssertCode(0x7C0A5C6E, "lfsux\tf0,r10,r11");
            AssertCode(0x7DAA44EE, "lfdux\tf13,r10,r8");
            AssertCode(0x7D2B5E34, "srad\tr11,r9,r11");
            AssertCode(0x7C23F7EC, "dcbz\tr3,r30");

            AssertCode(0x13040000, "vaddubm\tv24,v4,v0");
            AssertCode(0x10011002, "vmaxub\tv0,v1,v2");
            AssertCode(0x10000022, "vmladduhm\tv0,v0,v0,v0");
            AssertCode(0x10000042, "vmaxuh\tv0,v0,v0");
            AssertCode(0x11b268e2, "vmladduhm\tv13,v18,v13,v3");
            AssertCode(0x12020100, "vadduqm\tv16,v2,v0");
            AssertCode(0x1003c200, "vaddubs\tv0,v3,v24");
            AssertCode(0x10010401, "bcdadd.\tv0,v1,v0,00");
            AssertCode(0x117d9406, "vcmpequb.\tv11,v29,v18");
            AssertCode(0x7c0019ec, "dcbtst\tr0,r3");


            // The following instructions were found in an
            // XBox 360 binary, but no PowerPC documentation
            // seems to exist for them.
            /*
            AssertCode(0x12a0f9c7, ".long 0x12a0f9c7");
            AssertCode(0x10030001, ".long 0x10030001");
            AssertCode(0x10011003, ".long 0x10011003");
            AssertCode(0x111110b0, ".long 0x111110b0");
            AssertCode(0x100050c3, ".long 0x100050c3");
            AssertCode(0x100130cb, ".long 0x100130cb");
            AssertCode(0x13fff935, ".long 0x13fff935");
            AssertCode(0x136a2987, ".long 0x136a2987");
            AssertCode(0x13D29A35, ".long 0x13d29a35");
            AssertCode(0x13e95187, ".long 0x13e95187");
            AssertCode(0x100059c3, ".long 0x100059c3");
            AssertCode(0x100b61cb, ".long 0x100b61cb");
            AssertCode(0x13d29a35, ".long 0x13d29a35");
            AssertCode(0x4d48c976, ".long 0x4d48c976");
            AssertCode(0x4f8e1ae5, ".long 0x4f8e1ae5");
            AssertCode(0x4c4d4e4f, ".long 0x4c4d4e4f");
            AssertCode(0x7c53b17e, ".long 0x7c53b17e");
            AssertCode(0x7dc2dec0, ".long 0x7dc2dec0");
            AssertCode(0x7f7f7f7f, ".long 0x7f7f7f7f");
            AssertCode(0x7f7f7f7f, ".long 0x7f7f7f7f");
            AssertCode(0x7fefffff, ".long 0x7fefffff");



            AssertCode(0x102038C3, ".long 0x102038c3");
            AssertCode(0x102020CB, ".long 0x102020cb");
            AssertCode(0x13CA1987, ".long 0x13ca1987");
            AssertCode(0x100059C3, ".long 0x100059c3");
            AssertCode(0x13E051C7, ".long 0x13e051c7");
            AssertCode(0x116021C3, ".long 0x116021c3");
            AssertCode(0x126B61CB, ".long 0x126b61cb");


            AssertCode(0x100b60cf, "psq_stux\tf0,r11,r12,0,1");
            AssertCode(0x100b61cf, "psq_stux\tf0,r11,r12,0,3");
            AssertCode(0x1000001a, "ps_muls1\tf0,f0,f0");
            AssertCode(0xf3d4a7eb, "psq_st\tf30,2027(r20),1,2");
            AssertCode(0xf3f895aa, "psq_st\tf31,1450(r24),1,1");
            AssertCode(0x13C100CF, "psq_stux\tf30,r1,r0,0,1");
            AssertCode(0x13A05C07, "udi0fcm\t29,0,11");
            AssertCode(0x13C55C47, "udi1fcm\t30,5,11");
            AssertCode(0x13E03507, "udi4fcm 31,0,6");
            AssertCode(0x13E85D47, "udi5fcm 31,8,11");
            AssertCode(0xf0a65dff, "xxsel\tvs37,vs38,vs43,vs55");
            */
        }

        [Test]
        public void PPCDis_VMX128()
        {
            AssertCode(0x102038C3, "lvx128\tv1,r0,r7");         // 04 - 0C3(195)
            AssertCode(0x102338CB, "lvx128\tv65,r3,r7");        // 04 - 0CB(203)
            AssertCode(0x13C100CF, "lvx128\tv126,r1,r0");       // 04 - 0CF(207)
            //AssertCode(0x13A05187, "@@@"); // 04 - 187(391)
            AssertCode(0x116021C3, "stvx128\tv11,r0,r4");       // 04 - 1C3(451)
            AssertCode(0x13C031C7, "stvx128\tv62,r0,r6");       // 04 - 1C7(455)
            AssertCode(0x100B61CB, "stvx128\tv64,r11,r12");     // 04 - 1CB(459)
            AssertCode(0x13C161CF, "stvx128\tv126,r1,r12");     // 04 - 1CF(463)
            //AssertCode(0x13D29A35, "@@@"); // 04 - 235(565)
            AssertCode(0x13C55C47, "lvrx128\tv62,r5,r11");      // 04 - 447(1095)
            AssertCode(0x13A05C07, "lvlx128\tv61,r0,r11");      // 04 - 407(1031)
            AssertCode(0x13E04507, "stvlx128\tv63,r0,r8");      // 04 - 507(1287)
            AssertCode(0x13E85D47, "stvrx128\tv63,r8,r11");     // 04 - 547(1351)

            AssertCode(0x1497B0B1, "vmulfp128\tv4,v55,v54");    // 05 - 009(9)
            AssertCode(0x1400E851, "vsubfp128\tv0,v0,v61");     // 05 - 005(5)
            AssertCode(0x14020100, "vperm128\tv0,v2,v0,v4");    // 05 - 010(16)
            AssertCode(0x177B011C, "vmaddcfp128\tv123,v27,v0"); // 05 - 011(17)
            AssertCode(0x173FE1B5, "vmsub3fp128\tv57,v63,v60"); // 05 - 019(25)
            AssertCode(0x157FA9F1, "vmsub4fp128\tv11,v63,v53"); // 05 - 01D(29)
            AssertCode(0x16D6BA35, "vand128\tv54,v54,v55");     // 05 - 021(33)
            //AssertCode(0x15BAAA71, "@@@"); // 05 - 025(37)
            AssertCode(0x15B8C2F1, "vor128\tv13,v56,v56");      // 05 - 02D(45)
            AssertCode(0x145AE331, "vxor128\tv2,v58,v60");      // 05 - 031(49)

            AssertCode(0x18000000, "vcmpeqfp128\tv0,v0,v0");    // 06 - 000(0)
            AssertCode(0x187EF823, "vcmpeqfp128\tv3,v62,v127"); // 06 - 002(2)
            AssertCode(0x1B5FF8F5, "vslw128\tv58,v63,v63");     // 06 - 00F(15)
            AssertCode(0x18F7E121, "vcmpgtfp128\tv7,v55,v60");  // 06 - 012(18)
            AssertCode(0x18280186, "vcmpbfp128\tv33,v8,v64");   // 06 - 018(24)
            AssertCode(0x195CB9F1, "vsrw128\tv10,v60,v55");     // 06 - 01F(31)
            //AssertCode(0x1BA5AA15, "@@@");    // 06 - 021(33)  - permutation odd encoding
            AssertCode(0x1AC0FA35, "vcfpsxws128\tv54,v63,+00"); // 06 - 023(35)
            //AssertCode(0x1918F251, "@@@");    // 06 - 025(37)
            //AssertCode(0x180EB291, "@@@");    // 06 - 029(41)
            AssertCode(0x1BDEE2A5, "vmaxfp128\tv62,v62,v60");   // 06 - 02A(42)
            AssertCode(0x1801F2B1, "vcsxwfp128\tv0,v62,01");    // 06 - 02B(43)
            //AssertCode(0x1AE1D2D5, "@@@");    // 06 - 02D(45)
            AssertCode(0x1BFFF2E5, "vminfp128\tv63,v63,v62");   // 06 - 02E(46)
            //AssertCode(0x1B04AB15, "@@@");    // 06 - 031(49)
            AssertCode(0x1B1FF325, "vmrghw128\tv56,v63,v62");   // 06 - 032(50)
            //AssertCode(0x1BA0AB35, "@@@");      // 06 - 033(51)
            //AssertCode(0x1B1BD355, "@@@");    // 06 - 035(53)
            AssertCode(0x1BFFF365, "vmrglw128\tv63,v63,v62");   // 06 - 036(54)
            AssertCode(0x1BC0DB75, "vrfin128\tv62,v59");        // 06 - 037(55)
            //AssertCode(0x1B4CD395, "@@@");    // 06 - 039(57)
            //AssertCode(0x18ADA3D1, "@@@");    // 06 - 03D(61)


            AssertCode(0x18A0DBF1, "vrfiz128\tv5,v59");         // 06 - 03F(63)
            AssertCode(0x1BCECCED, "vcmpgefp128.\tv126,v110,v57"); // 06 - 04E(78)
            AssertCode(0x1A013E1A, "vpkd3d128\tv80,v71,00,01,00"); // 06 - 061(97)
            AssertCode(0x1800F631, "vrefp128\tv0,v62");         // 06 - 063(99)
            AssertCode(0x19000640, "vcmpequw128\tv8,v64,v0"); // 06 - 064(100)
            AssertCode(0x1800F671, "vrsqrtefp128\tv0,v62"); // 06 - 067(103)
            AssertCode(0x1BA0EEB5, "vexptefp128\tv61,v61"); // 06 - 06B(107)
            AssertCode(0x1BEDFED7, "vpkd3d128\tv63,v127,03,01,03"); // 06 - 06D(109)
            AssertCode(0x1AA0EEF5, "vlogefp128\tv53,v61");      // 06 - 06F(111)
            AssertCode(0x19C49F15, "vrlimi128\tv46,v51,04,02"); // 06 - 071(113)
            AssertCode(0x1923CF31, "vspltw128\tv9,v57,03");     // 06 - 073(115)
            AssertCode(0x18019F51, "vrlimi128\tv0,v51,01,02");  // 06 - 075(117)
            AssertCode(0x1B600774, "vspltisw128\tv59,v0,+20");  // 06 - 077(119)
            AssertCode(0x19ACFF91, "vrlimi128\tv13,v63,0C,03"); // 06 - 079(121)
            AssertCode(0x18099FD1, "vrlimi128\tv0,v51,09,02");  // 06 - 07D(125)
            AssertCode(0x1B24DFF5, "vupkd3d128\tv57,v59,04");   // 06 - 07F(127)
        }

        [Test]
        public void PPCDis_vaddfp128()
        {
            //| 0 0 0 1 0 1 | VD128 | VA128 | VB128 | A | 0 0 0 0 | a | 1 | VDh | VBh |
            // 000101 01010 11111 10101 1 0000 1 1 10 01
            // 0001 0101 0101 1111 1010 1100 0011 1001
            AssertCode(0x155FAC39, "vaddfp128\tv74,v127,v53");
        }

        [Test]
        public void PPCDis_regression9()
        {
            AssertCode(0x08030000, "tdi\t00,r3,+0000");
            AssertCode(0x085888dc, "tdi\t02,r24,-7724");
            AssertCode(0x0bcc0000, "tdi\t1E,r12,+0000");
        }

        [Test]
        public void PPCDis_lfdp()
        {
            AssertCode(0xe4a1ac7d, "illegal"); // lfdp\tf5,-2137D(r1)") -- can't have odd offset or register
            AssertCode(0xe481ac7e, "illegal"); // lfdp\tf5,-2137E(r1)") -- can't have odd offset or register
            AssertCode(0xe481ac80, "lfdp\tf4,-21376(r1)");
        }

        [Test]
        public void PPCDis_stfdp()
        {
            AssertCode(0xf7Ad3927, "illegal"); // stfdp\tf28,14628(r13) odd floating point register
            AssertCode(0xf78d392E, "illegal"); // "stfdp\tf28,14628(r13) odd offset
            AssertCode(0xf78d3928, "stfdp\tf28,14632(r13)");
        }

        [Test]
        public void PPCDis_mfspr()
        {
            AssertCode(0x7FF94AA6, "mfspr\t00000139,r31");
        }

        // Unknown PowerPC VMX instruction 14008E00 05-020 (32)
        [Test]
        public void PPCDis_14008E00()
        {
            AssertCode(0x14008E00, "@@@");
        }

        // Unknown PowerPC VMX instruction 14288F83 05-038 (56)
        [Test]
        public void PPCDis_14288F83()
        {
            AssertCode(0x14288F83, "@@@");
        }

        // Unknown PowerPC VMX instruction 14355361 05-034 (52)
        [Test]
        public void PPCDis_14355361()
        {
            AssertCode(0x14355361, "@@@");
        }

        // Unknown PowerPC VMX instruction 143F9B41 05-034 (52)
        [Test]
        public void PPCDis_143F9B41()
        {
            AssertCode(0x143F9B41, "@@@");
        }

        // Unknown PowerPC VMX instruction 14480264 05-024 (36)
        [Test]
        public void PPCDis_14480264()
        {
            AssertCode(0x14480264, "@@@");
        }

        // Unknown PowerPC VMX instruction 144DFFB8 05-039 (57)
        [Test]
        public void PPCDis_144DFFB8()
        {
            AssertCode(0x144DFFB8, "@@@");
        }

        // Unknown PowerPC VMX instruction 144DFFC0 05-03C(60)
        [Test]
        public void PPCDis_144DFFC0()
        {
            AssertCode(0x144DFFC0, "@@@");
        }

        // Unknown PowerPC VMX instruction 144DFFE8 05-03C(60)
        [Test]
        public void PPCDis_144DFFE8()
        {
            AssertCode(0x144DFFE8, "@@@");
        }

        // Unknown PowerPC VMX instruction 145AFF88 05-038 (56)
        [Test]
        public void PPCDis_145AFF88()
        {
            AssertCode(0x145AFF88, "@@@");
        }


        // Unknown PowerPC VMX instruction 14A02600 05-020 (32)
        [Test]
        public void PPCDis_14A02600()
        {
            AssertCode(0x14A02600, "@@@");
        }

        // Unknown PowerPC VMX instruction 14A6A95E 05-015 (21)
        [Test]
        public void PPCDis_14A6A95E()
        {
            AssertCode(0x14A6A95E, "@@@");
        }

        // Unknown PowerPC VMX instruction 14C214FF 05-00D (13)
        [Test]
        public void PPCDis_14C214FF()
        {
            AssertCode(0x14C214FF, "@@@");
        }

        // Unknown PowerPC VMX instruction 14CF70F0 05-00D (13)
        [Test]
        public void PPCDis_14CF70F0()
        {
            AssertCode(0x14CF70F0, "@@@");
        }

        // Unknown PowerPC VMX instruction 153E157D 05-015 (21)
        [Test]
        public void PPCDis_153E157D()
        {
            AssertCode(0x153E157D, "@@@");
        }

        // Unknown PowerPC VMX instruction 153E22C9 05-02C(44)
        [Test]
        public void PPCDis_153E22C9()
        {
            AssertCode(0x153E22C9, "@@@");
        }

        // Unknown PowerPC VMX instruction 158A6D50 05-015 (21)
        [Test]
        public void PPCDis_158A6D50()
        {
            AssertCode(0x158A6D50, "@@@");
        }

        // Unknown PowerPC VMX instruction 15948A9E 05-029 (41)
        [Test]
        public void PPCDis_15948A9E()
        {
            AssertCode(0x15948A9E, "@@@");
        }

        // Unknown PowerPC VMX instruction 15AF5553 05-015 (21)
        [Test]
        public void PPCDis_15AF5553()
        {
            AssertCode(0x15AF5553, "@@@");
        }

        // Unknown PowerPC VMX instruction 15CB6EC4 05-02C(44)
        [Test]
        public void PPCDis_15CB6EC4()
        {
            AssertCode(0x15CB6EC4, "@@@");
        }

        // Unknown PowerPC VMX instruction 16035950 05-015 (21)
        [Test]
        public void PPCDis_16035950()
        {
            AssertCode(0x16035950, "@@@");
        }

        // Unknown PowerPC VMX instruction 16103F81 05-038 (56)
        [Test]
        public void PPCDis_16103F81()
        {
            AssertCode(0x16103F81, "@@@");
        }

        // Unknown PowerPC VMX instruction 163E1680 05-028 (40)
        [Test]
        public void PPCDis_163E1680()
        {
            AssertCode(0x163E1680, "@@@");
        }

        // Unknown PowerPC VMX instruction 16C31706 05-030 (48)
        [Test]
        public void PPCDis_16C31706()
        {
            AssertCode(0x16C31706, "@@@");
        }

        // Unknown PowerPC VMX instruction 16CB0280 05-028 (40)
        [Test]
        public void PPCDis_16CB0280()
        {
            AssertCode(0x16CB0280, "@@@");
        }


        // Unknown PowerPC VMX instruction 16CB0780 05-038 (56)
        [Test]
        public void PPCDis_16CB0780()
        {
            AssertCode(0x16CB0780, "@@@");
        }

        // Unknown PowerPC VMX instruction 16CD03C0 05-03C(60)
        [Test]
        public void PPCDis_16CD03C0()
        {
            AssertCode(0x16CD03C0, "@@@");
        }

        //         // Unknown PowerPC VMX instruction 16CD07C0 05-03C(60)
        [Test]
        public void PPCDis_16CD07C0()
        {
            AssertCode(0x16CD07C0, "@@@");
        }


        // Unknown PowerPC VMX instruction 16CD07D0 05-03D (61)
        [Test]
        public void PPCDis_16CD07D0()
        {
            AssertCode(0x16CD07D0, "@@@");
        }

        // Unknown PowerPC VMX instruction 16CD0A40 05-024 (36)
        [Test]
        public void PPCDis_16CD0A40()
        {
            AssertCode(0x16CD0A40, "@@@");
        }

        // Unknown PowerPC VMX instruction 16CD0E48 05-024 (36)
        [Test]
        public void PPCDis_16CD0E48()
        {
            AssertCode(0x16CD0E48, "@@@");
        }

        // Unknown PowerPC VMX instruction 16CD0E80 05-028 (40)
        [Test]
        public void PPCDis_16CD0E80()
        {
            AssertCode(0x16CD0E80, "@@@");
        }

        // Unknown PowerPC VMX instruction 16D2D77C 05-035 (53)
        [Test]
        public void PPCDis_16D2D77C()
        {
            AssertCode(0x16D2D77C, "@@@");
        }

        // Unknown PowerPC VMX instruction 16FCFEED 05-02C(44)
        [Test]
        public void PPCDis_16FCFEED()
        {
            AssertCode(0x16FCFEED, "@@@");
        }

        // Unknown PowerPC VMX instruction 172DB65E 05-025 (37)
        [Test]
        public void PPCDis_172DB65E()
        {
            AssertCode(0x172DB65E, "@@@");
        }

        // Unknown PowerPC VMX instruction 174A178F 05-038 (56)
        [Test]
        public void PPCDis_174A178F()
        {
            AssertCode(0x174A178F, "@@@");
        }

        // Unknown PowerPC VMX instruction 174EF57B 05-015 (21)
        [Test]
        public void PPCDis_174EF57B()
        {
            AssertCode(0x174EF57B, "@@@");
        }

        // Unknown PowerPC VMX instruction 177BE788 05-038 (56)
        [Test]
        public void PPCDis_177BE788()
        {
            AssertCode(0x177BE788, "@@@");
        }

        //        
        [Test]
        public void PPCDis_17BADD56()
        {
            AssertCode(0x17BADD56, "@@@");
        }

        // Unknown PowerPC VMX instruction 180E8A9A 06-029 (41)
        [Test]
        public void PPCDis_180E8A9A()
        {
            AssertCode(0x180E8A9A, "@@@");
        }

        // Unknown PowerPC VMX instruction 1813EA1B 06-021 (33)
        [Test]
        public void PPCDis_1813EA1B()
        {
            AssertCode(0x1813EA1B, "@@@");
        }

        // Unknown PowerPC VMX instruction 1815B554 06-055 (85)
        [Test]
        public void PPCDis_1815B554()
        {
            AssertCode(0x1815B554, "@@@");
        }

        // Unknown PowerPC VMX instruction 18181818 06-001 (1)
        [Test]
        public void PPCDis_18181818()
        {
            AssertCode(0x18181818, "@@@");
        }


        // Unknown PowerPC VMX instruction 181D7012 06-001 (1)
        [Test]
        public void PPCDis_181D7012()
        {
            AssertCode(0x181D7012, "@@@");
        }

        // Unknown PowerPC VMX instruction 185F1B1F 06-031 (49)
        [Test]
        public void PPCDis_185F1B1F()
        {
            AssertCode(0x185F1B1F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1869E45D 06-045 (69)
        [Test]
        public void PPCDis_1869E45D()
        {
            AssertCode(0x1869E45D, "@@@");
        }

        // Unknown PowerPC VMX instruction 189E181B 06-001 (1)
        [Test]
        public void PPCDis_189E181B()
        {
            AssertCode(0x189E181B, "@@@");
        }

        // Unknown PowerPC VMX instruction 18A05DBB 06-05B(91)
        [Test]
        public void PPCDis_18A05DBB()
        {
            AssertCode(0x18A05DBB, "@@@");
        }

        // Unknown PowerPC VMX instruction 18C735B4 06-05B(91)
        [Test]
        public void PPCDis_18C735B4()
        {
            AssertCode(0x18C735B4, "@@@");
        }

        // Unknown PowerPC VMX instruction 18F5193F 06-013 (19)
        [Test]
        public void PPCDis_18F5193F()
        {
            AssertCode(0x18F5193F, "@@@");
        }

        // Unknown PowerPC VMX instruction 190031B3 06-01B(27)
        [Test]
        public void PPCDis_190031B3()
        {
            AssertCode(0x190031B3, "@@@");
        }

        // Unknown PowerPC VMX instruction 19189059 06-005 (5)
        [Test]
        public void PPCDis_19189059()
        {
            AssertCode(0x19189059, "@@@");
        }

        // Unknown PowerPC VMX instruction 191AF858 06-005 (5)
        [Test]
        public void PPCDis_191AF858()
        {
            AssertCode(0x191AF858, "@@@");
        }

        // Unknown PowerPC VMX instruction 191F191D 06-011 (17)
        [Test]
        public void PPCDis_191F191D()
        {
            AssertCode(0x191F191D, "@@@");
        }

        // Unknown PowerPC VMX instruction 1923017D 06-017 (23)
        [Test]
        public void PPCDis_1923017D()
        {
            AssertCode(0x1923017D, "@@@");
        }

        // Unknown PowerPC VMX instruction 193A1938 06-013 (19)
        [Test]
        public void PPCDis_193A1938()
        {
            AssertCode(0x193A1938, "@@@");
        }

        // Unknown PowerPC VMX instruction 193CB179 06-017 (23)
        [Test]
        public void PPCDis_193CB179()
        {
            AssertCode(0x193CB179, "@@@");
        }

        // Unknown PowerPC VMX instruction 193E193C 06-013 (19)
        [Test]
        public void PPCDis_193E193C()
        {
            AssertCode(0x193E193C, "@@@");
        }

        // Unknown PowerPC VMX instruction 19487FC0 06-07C(124)
        [Test]
        public void PPCDis_19487FC0()
        {
            AssertCode(0x19487FC0, "@@@");
        }

        // Unknown PowerPC VMX instruction 194EE0B3 06-00B(11)
        [Test]
        public void PPCDis_194EE0B3()
        {
            AssertCode(0x194EE0B3, "@@@");
        }

        // Unknown PowerPC VMX instruction 1954AC7D 06-047 (71)
        [Test]
        public void PPCDis_1954AC7D()
        {
            AssertCode(0x1954AC7D, "@@@");
        }

        // Unknown PowerPC VMX instruction 195B1959 06-015 (21)
        [Test]
        public void PPCDis_195B1959()
        {
            AssertCode(0x195B1959, "@@@");
        }

        // Unknown PowerPC VMX instruction 195F195F 06-015 (21)
        [Test]
        public void PPCDis_195F195F()
        {
            AssertCode(0x195F195F, "@@@");
        }

        // Unknown PowerPC VMX instruction 195F1B1F 06-031 (49)
        [Test]
        public void PPCDis_195F1B1F()
        {
            AssertCode(0x195F1B1F, "@@@");
        }

        // Unknown PowerPC VMX instruction 197A1978 06-017 (23)
        [Test]
        public void PPCDis_197A1978()
        {
            AssertCode(0x197A1978, "@@@");
        }

        // Unknown PowerPC VMX instruction 197C1B1A 06-031 (49)
        [Test]
        public void PPCDis_197C1B1A()
        {
            AssertCode(0x197C1B1A, "@@@");
        }

        // Unknown PowerPC VMX instruction 197E1B1A 06-031 (49)
        [Test]
        public void PPCDis_197E1B1A()
        {
            AssertCode(0x197E1B1A, "@@@");
        }

        // Unknown PowerPC VMX instruction 197F9930 06-013 (19)
        [Test]
        public void PPCDis_197F9930()
        {
            AssertCode(0x197F9930, "@@@");
        }

        // Unknown PowerPC VMX instruction 19938CB0 06-04B(75)
        [Test]
        public void PPCDis_19938CB0()
        {
            AssertCode(0x19938CB0, "@@@");
        }

        // Unknown PowerPC VMX instruction 19F2578B 06-078 (120)
        [Test]
        public void PPCDis_19F2578B()
        {
            AssertCode(0x19F2578B, "@@@");
        }

        // Unknown PowerPC VMX instruction 1A1B1213 06-021 (33)
        [Test]
        public void PPCDis_1A1B1213()
        {
            AssertCode(0x1A1B1213, "@@@");
        }

        // Unknown PowerPC VMX instruction 1A99F016 06-001 (1)
        [Test]
        public void PPCDis_1A99F016()
        {
            AssertCode(0x1A99F016, "@@@");
        }

        // Unknown PowerPC VMX instruction 1AAF8F8A 06-078 (120)
        [Test]
        public void PPCDis_1AAF8F8A()
        {
            AssertCode(0x1AAF8F8A, "@@@");
        }

        // Unknown PowerPC VMX instruction 1AC1CF8C 06-078 (120)
        [Test]
        public void PPCDis_1AC1CF8C()
        {
            AssertCode(0x1AC1CF8C, "@@@");
        }

        // Unknown PowerPC VMX instruction 1ACED55C 06-055 (85)
        [Test]
        public void PPCDis_1ACED55C()
        {
            AssertCode(0x1ACED55C, "@@@");
        }

        // Unknown PowerPC VMX instruction 1ADF189F 06-009 (9)
        [Test]
        public void PPCDis_1ADF189F()
        {
            AssertCode(0x1ADF189F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1ADF1ADC 06-02D (45)
        [Test]
        public void PPCDis_1ADF1ADC()
        {
            AssertCode(0x1ADF1ADC, "@@@");
        }

        // Unknown PowerPC VMX instruction 1AEA229D 06-029 (41)
        [Test]
        public void PPCDis_1AEA229D()
        {
            AssertCode(0x1AEA229D, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B08C471 06-047 (71)
        [Test]
        public void PPCDis_1B08C471()
        {
            AssertCode(0x1B08C471, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B15978C 06-078 (120)
        [Test]
        public void PPCDis_1B15978C()
        {
            AssertCode(0x1B15978C, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B1E1B3F 06-033 (51)
        [Test]
        public void PPCDis_1B1E1B3F()
        {
            AssertCode(0x1B1E1B3F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B1F185F 06-005 (5)
        [Test]
        public void PPCDis_1B1F185F()
        {
            AssertCode(0x1B1F185F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B3E1B3C 06-033 (51)
        [Test]
        public void PPCDis_1B3E1B3C()
        {
            AssertCode(0x1B3E1B3C, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5C007B 06-007 (7)
        [Test]
        public void PPCDis_1B5C007B()
        {
            AssertCode(0x1B5C007B, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5C193E 06-013 (19)
        [Test]
        public void PPCDis_1B5C193E()
        {
            AssertCode(0x1B5C193E, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5D1BAE 06-03A(58)
        [Test]
        public void PPCDis_1B5D1BAE()
        {
            AssertCode(0x1B5D1BAE, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5E029F 06-029 (41)
        [Test]
        public void PPCDis_1B5E029F()
        {
            AssertCode(0x1B5E029F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5E1B5C 06-035 (53)
        [Test]
        public void PPCDis_1B5E1B5C()
        {
            AssertCode(0x1B5E1B5C, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5F009F 06-009 (9)
        [Test]
        public void PPCDis_1B5F009F()
        {
            AssertCode(0x1B5F009F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5F1B5D 06-035 (53)
        [Test]
        public void PPCDis_1B5F1B5D()
        {
            AssertCode(0x1B5F1B5D, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B77F19F 06-019 (25)
        [Test]
        public void PPCDis_1B77F19F()
        {
            AssertCode(0x1B77F19F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B7DA7AA 06-07A(122)
        [Test]
        public void PPCDis_1B7DA7AA()
        {
            AssertCode(0x1B7DA7AA, "@@@");
        }


        // Unknown PowerPC VX instruction 10050044 04-044 (68)
        [Test]
        [Ignore("Don't know how to decode this yet")]
        public void PPCDis_10050044()
        {
            AssertCode(0x10050044, "@@@");
        }

        [Test]
        public void PPCDis_ps_madd()
        {
            AssertCode(0x1008333A, "ps_madd\tf0,f8,f6,f12");
        }

        [Test]
        public void PPCDis_ps_madds1()
        {
            AssertCode(0x1009011E, "ps_madds1\tf0,f9,f0,f4");
        }

        [Test]
        public void PPCDis_ps_madds0()
        {
            AssertCode(0x100B015C, "ps_madds0\tf0,f11,f0,f5");
        }

        [Test]
        public void PPCDis_ps_muls0_Rc()
        {
            AssertCode(0x10111819, "ps_muls0.\tf0,f17,f0");
        }


        [Test]
        public void PPCDis_psq_lx()
        {
            AssertCode(0x1015000C, "psq_lx\tf0,r21,r0,00,00");
        }

        [Test]
        public void PPCDis_psq_stx()
        {
            AssertCode(0x13E0180E, "psq_stx\tf31,r0,r3,00,00");
        }

        [Test]
        public void PPCDis_ps_muls1()
        {
            AssertCode(0x1018191A, "ps_muls1\t@@@");
        }

        [Test]
        public void PPCDis_psq_lux()
        {
            AssertCode(0x101D104D, "psq_lux\tf0,r29,r2,00,00");
        }

        [Test]
        public void PPCDis_ps_neg()
        {
            AssertCode(0x10000050, "ps_neg\tf0,f0");
        }

        [Test]
        public void PPCDis_ps_mul()
        {
            AssertCode(0x10000072, "ps_mul\tf0,f0,f1");
        }

        [Test]
        public void PPCDis_ps_muls0()
        {
            AssertCode(0x10000318, "ps_muls0\tf0,f0,f12");
        }

        [Test]
        public void PPCDis_ps_div()
        {
            AssertCode(0x10020024, "ps_div\tf0,f2,f0");
        }

        [Test]
        public void PPCDis_ps_sum0()
        {
            AssertCode(0x10040014, "ps_sum0\tf0,f4,f0,f0");
        }

#if NYI

        // Unknown PowerPC VX instruction 100E0206 04-206 (518)
        [Test]
        public void PPCDis_100E0206()
        {
            AssertCode(0x100E0206, "@@@");
        }

        // Unknown PowerPC VX instruction 10130034 04-034 (52)
        [Test]
        public void PPCDis_10130034()
        {
            AssertCode(0x10130034, "@@@");
        }

        // Unknown PowerPC VX instruction 10200090 04-090 (144)
        [Test]
        public void PPCDis_10200090()
        {
            AssertCode(0x10200090, "@@@");
        }

        // Unknown PowerPC VX instruction 102004A0 04-4A0(1184)
        [Test]
        public void PPCDis_102004A0()
        {
            AssertCode(0x102004A0, "@@@");
        }

        // Unknown PowerPC VX instruction 102104A0 04-4A0(1184)
        [Test]
        public void PPCDis_102104A0()
        {
            AssertCode(0x102104A0, "@@@");
        }

        // Unknown PowerPC VX instruction 10210CA0 04-4A0(1184)
        [Test]
        public void PPCDis_10210CA0()
        {
            AssertCode(0x10210CA0, "@@@");
        }

        // Unknown PowerPC VX instruction 10220034 04-034 (52)
        [Test]
        public void PPCDis_10220034()
        {
            AssertCode(0x10220034, "@@@");
        }

        // Unknown PowerPC VX instruction 10228034 04-034 (52)
        [Test]
        public void PPCDis_10228034()
        {
            AssertCode(0x10228034, "@@@");
        }

           [Test]
        public void PPCDis_psq_stux()
        {
            AssertCode(0x1023674E, "psq_stux\tf1,r3,r12,01,06");
        }

        // Unknown PowerPC VX instruction 10248036 04-036 (54)
        [Test]
        public void PPCDis_10248036()
        {
            AssertCode(0x10248036, "@@@");
        }

        // Unknown PowerPC VX instruction 10270034 04-034 (52)
        [Test]
        public void PPCDis_10270034()
        {
            AssertCode(0x10270034, "@@@");
        }

        // Unknown PowerPC VX instruction 10280004 04-004 (4)
        [Test]
        public void PPCDis_10280004()
        {
            AssertCode(0x10280004, "@@@");
        }

        // Unknown PowerPC VX instruction 10290034 04-034 (52)
        [Test]
        public void PPCDis_10290034()
        {
            AssertCode(0x10290034, "@@@");
        }

        // Unknown PowerPC VX instruction 10290044 04-044 (68)
        [Test]
        public void PPCDis_10290044()
        {
            AssertCode(0x10290044, "@@@");
        }

          // Unknown PowerPC VX instruction 10290074 04-074 (116)
        [Test]
        public void PPCDis_10290074()
        {
            AssertCode(0x10290074, "@@@");
        }

        // Unknown PowerPC VX instruction 10290084 04-084 (132)
        [Test]
        public void PPCDis_10290084()
        {
            AssertCode(0x10290084, "@@@");
        }

        // Unknown PowerPC VX instruction 102A804B 04-04B(75)
        [Test]
        public void PPCDis_102A804B()
        {
            AssertCode(0x102A804B, "@@@");
        }

      

        // Unknown PowerPC VX instruction 102B0034 04-034 (52)
        [Test]
        public void PPCDis_102B0034()
        {
            AssertCode(0x102B0034, "@@@");
        }

        // Unknown PowerPC VX instruction 102B0044 04-044 (68)
        [Test]
        public void PPCDis_102B0044()
        {
            AssertCode(0x102B0044, "@@@");
        }

        // Unknown PowerPC VX instruction 102C0034 04-034 (52)
        [Test]
        public void PPCDis_102C0034()
        {
            AssertCode(0x102C0034, "@@@");
        }

        // Unknown PowerPC VX instruction 102F0044 04-044 (68)
        [Test]
        public void PPCDis_102F0044()
        {
            AssertCode(0x102F0044, "@@@");
        }

        // Unknown PowerPC VX instruction 10310044 04-044 (68)
        [Test]
        public void PPCDis_10310044()
        {
            AssertCode(0x10310044, "@@@");
        }

        // Unknown PowerPC VX instruction 10311053 04-053 (83)
        [Test]
        public void PPCDis_10311053()
        {
            AssertCode(0x10311053, "@@@");
        }

        // Unknown PowerPC VX instruction 10320030 04-030 (48)
        [Test]
        public void PPCDis_10320030()
        {
            AssertCode(0x10320030, "@@@");
        }

        // Unknown PowerPC VX instruction 10350004 04-004 (4)
        [Test]
        public void PPCDis_10350004()
        {
            AssertCode(0x10350004, "@@@");
        }

        // Unknown PowerPC VX instruction 103E0034 04-034 (52)
        [Test]
        public void PPCDis_103E0034()
        {
            AssertCode(0x103E0034, "@@@");
        }

        // Unknown PowerPC VX instruction 10400090 04-090 (144)
        [Test]
        public void PPCDis_10400090()
        {
            AssertCode(0x10400090, "@@@");
        }

        // Unknown PowerPC VX instruction 10400C60 04-460 (1120)
        [Test]
        public void PPCDis_10400C60()
        {
            AssertCode(0x10400C60, "@@@");
        }

        // Unknown PowerPC VX instruction 10600090 04-090 (144)
        [Test]
        public void PPCDis_10600090()
        {
            AssertCode(0x10600090, "@@@");
        }

        // Unknown PowerPC VX instruction 10680008 04-008 (8)
        [Test]
        public void PPCDis_10680008()
        {
            AssertCode(0x10680008, "@@@");
        }

        // Unknown PowerPC VX instruction 107EF2E3 04-2E3 (739)
        [Test]
        public void PPCDis_107EF2E3()
        {
            AssertCode(0x107EF2E3, "@@@");
        }

        // Unknown PowerPC VX instruction 10800090 04-090 (144)
        [Test]
        public void PPCDis_10800090()
        {
            AssertCode(0x10800090, "@@@");
        }

      
        // Unknown PowerPC VX instruction 108BC940 04-140 (320)
        [Test]
        public void PPCDis_108BC940()
        {
            AssertCode(0x108BC940, "@@@");
        }

        // Unknown PowerPC VX instruction 108C0170 04-170 (368)
        [Test]
        public void PPCDis_108C0170()
        {
            AssertCode(0x108C0170, "@@@");
        }

        // Unknown PowerPC VX instruction 108CF1C8 04-1C8(456)
        [Test]
        public void PPCDis_108CF1C8()
        {
            AssertCode(0x108CF1C8, "@@@");
        }

        // Unknown PowerPC VX instruction 108D0140 04-140 (320)
        [Test]
        public void PPCDis_108D0140()
        {
            AssertCode(0x108D0140, "@@@");
        }

        // Unknown PowerPC VX instruction 108E1E09 04-609 (1545)
        [Test]
        public void PPCDis_108E1E09()
        {
            AssertCode(0x108E1E09, "@@@");
        }

        // Unknown PowerPC VX instruction 10980010 04-010 (16)
        [Test]
        public void PPCDis_10980010()
        {
            AssertCode(0x10980010, "@@@");
        }

        // Unknown PowerPC VX instruction 10980034 04-034 (52)
        [Test]
        public void PPCDis_10980034()
        {
            AssertCode(0x10980034, "@@@");
        }

        // Unknown PowerPC VX instruction 10A00090 04-090 (144)
        [Test]
        public void PPCDis_10A00090()
        {
            AssertCode(0x10A00090, "@@@");
        }

        // Unknown PowerPC VX instruction 10A0AD05 04-505 (1285)
        [Test]
        public void PPCDis_10A0AD05()
        {
            AssertCode(0x10A0AD05, "@@@");
        }

        // Unknown PowerPC VX instruction 10A21CA0 04-4A0(1184)
        [Test]
        public void PPCDis_10A21CA0()
        {
            AssertCode(0x10A21CA0, "@@@");
        }


        // Unknown PowerPC VX instruction 10A789F0 04-1F0 (496)
        [Test]
        public void PPCDis_10A789F0()
        {
            AssertCode(0x10A789F0, "@@@");
        }

        // Unknown PowerPC VX instruction 10C00090 04-090 (144)
        [Test]
        public void PPCDis_10C00090()
        {
            AssertCode(0x10C00090, "@@@");
        }

        // Unknown PowerPC VX instruction 10C314A0 04-4A0(1184)
        [Test]
        public void PPCDis_10C314A0()
        {
            AssertCode(0x10C314A0, "@@@");
        }

        // Unknown PowerPC VX instruction 10CE1CF4 04-4F4 (1268)
        [Test]
        public void PPCDis_10CE1CF4()
        {
            AssertCode(0x10CE1CF4, "@@@");
        }

        // Unknown PowerPC VX instruction 10DF1111 04-111 (273)
        [Test]
        public void PPCDis_10DF1111()
        {
            AssertCode(0x10DF1111, "@@@");
        }

        // Unknown PowerPC VX instruction 10E00090 04-090 (144)
        [Test]
        public void PPCDis_10E00090()
        {
            AssertCode(0x10E00090, "@@@");
        }

        [Test]
        public void PPCDis_10E424A0()
        {
            AssertCode(0x10E424A0, "@@@");
        }

        // Unknown PowerPC VX instruction 10EB6FD2 04-7D2 (2002)
        [Test]
        public void PPCDis_10EB6FD2()
        {
            AssertCode(0x10EB6FD2, "@@@");
        }

        // Unknown PowerPC VX instruction 10FB4492 04-492 (1170)
        [Test]
        public void PPCDis_10FB4492()
        {
            AssertCode(0x10FB4492, "@@@");
        }

        // Unknown PowerPC VX instruction 11000090 04-090 (144)
        [Test]
        public void PPCDis_11000090()
        {
            AssertCode(0x11000090, "@@@");
        }

        [Test]
        public void PPCDis_110844A0()
        {
            AssertCode(0x110844A0, "@@@");
        }

        // Unknown PowerPC VX instruction 111705E3 04-5E3 (1507)
        [Test]
        public void PPCDis_111705E3()
        {
            AssertCode(0x111705E3, "@@@");
        }

        // Unknown PowerPC VX instruction 112007FE 04-7FE(2046)
        [Test]
        public void PPCDis_112007FE()
        {
            AssertCode(0x112007FE, "@@@");
        }

        // Unknown PowerPC VX instruction 11200843 04-043 (67)
        [Test]
        public void PPCDis_11200843()
        {
            AssertCode(0x11200843, "@@@");
        }

        // Unknown PowerPC VX instruction 11328062 04-062 (98)
        [Test]
        public void PPCDis_11328062()
        {
            AssertCode(0x11328062, "@@@");
        }

        // Unknown PowerPC VX instruction 11400090 04-090 (144)
        [Test]
        public void PPCDis_11400090()
        {
            AssertCode(0x11400090, "@@@");
        }

        // Unknown PowerPC VX instruction 11441177 04-177 (375)
        [Test]
        public void PPCDis_11441177()
        {
            AssertCode(0x11441177, "@@@");
        }

        // Unknown PowerPC VX instruction 114F0DE3 04-5E3 (1507)
        [Test]
        public void PPCDis_114F0DE3()
        {
            AssertCode(0x114F0DE3, "@@@");
        }

        // Unknown PowerPC VX instruction 11600090 04-090 (144)
        [Test]
        public void PPCDis_11600090()
        {
            AssertCode(0x11600090, "@@@");
        }

        // Unknown PowerPC VX instruction 116B0004 04-004 (4)
        [Test]
        public void PPCDis_116B0004()
        {
            AssertCode(0x116B0004, "@@@");
        }

        // Unknown PowerPC VX instruction 11800090 04-090 (144)
        [Test]
        public void PPCDis_11800090()
        {
            AssertCode(0x11800090, "@@@");
        }

        // Unknown PowerPC VX instruction 11806034 04-034 (52)
        [Test]
        public void PPCDis_11806034()
        {
            AssertCode(0x11806034, "@@@");
        }

        // Unknown PowerPC VX instruction 11811F67 04-767 (1895)
        [Test]
        public void PPCDis_11811F67()
        {
            AssertCode(0x11811F67, "@@@");
        }

        // Unknown PowerPC VX instruction 11890008 04-008 (8)
        [Test]
        public void PPCDis_11890008()
        {
            AssertCode(0x11890008, "@@@");
        }

        // Unknown PowerPC VX instruction 118C6420 04-420 (1056)
        [Test]
        public void PPCDis_118C6420()
        {
            AssertCode(0x118C6420, "@@@");
        }

        // Unknown PowerPC VX instruction 118C97B0 04-7B0(1968)
        [Test]
        public void PPCDis_118C97B0()
        {
            AssertCode(0x118C97B0, "@@@");
        }

        // Unknown PowerPC VX instruction 11A00090 04-090 (144)
        [Test]
        public void PPCDis_11A00090()
        {
            AssertCode(0x11A00090, "@@@");
        }

        // Unknown PowerPC VX instruction 11A00D7E 04-57E (1406)
        [Test]
        public void PPCDis_11A00D7E()
        {
            AssertCode(0x11A00D7E, "@@@");
        }

        // Unknown PowerPC VX instruction 11C00090 04-090 (144)
        [Test]
        public void PPCDis_11C00090()
        {
            AssertCode(0x11C00090, "@@@");
        }

        // Unknown PowerPC VX instruction 11C44144 04-144 (324)
        [Test]
        public void PPCDis_11C44144()
        {
            AssertCode(0x11C44144, "@@@");
        }

        // Unknown PowerPC VX instruction 11D39CE0 04-4E0 (1248)
        [Test]
        public void PPCDis_11D39CE0()
        {
            AssertCode(0x11D39CE0, "@@@");
        }

        // Unknown PowerPC VX instruction 11E00090 04-090 (144)
        [Test]
        public void PPCDis_11E00090()
        {
            AssertCode(0x11E00090, "@@@");
        }

        // Unknown PowerPC VX instruction 12000090 04-090 (144)
        [Test]
        public void PPCDis_12000090()
        {
            AssertCode(0x12000090, "@@@");
        }

        // Unknown PowerPC VX instruction 12008910 04-110 (272)
        [Test]
        public void PPCDis_12008910()
        {
            AssertCode(0x12008910, "@@@");
        }

        // Unknown PowerPC VX instruction 1200B001 04-001 (1)
        [Test]
        public void PPCDis_1200B001()
        {
            AssertCode(0x1200B001, "@@@");
        }

        // Unknown PowerPC VX instruction 1200C812 04-012 (18)
        [Test]
        public void PPCDis_1200C812()
        {
            AssertCode(0x1200C812, "@@@");
        }

        // Unknown PowerPC VX instruction 120A09E0 04-1E0 (480)
        [Test]
        public void PPCDis_120A09E0()
        {
            AssertCode(0x120A09E0, "@@@");
        }

        // Unknown PowerPC VX instruction 1214124A 04-24A(586)
        [Test]
        public void PPCDis_1214124A()
        {
            AssertCode(0x1214124A, "@@@");
        }

        // Unknown PowerPC VX instruction 12200090 04-090 (144)
        [Test]
        public void PPCDis_12200090()
        {
            AssertCode(0x12200090, "@@@");
        }

        // Unknown PowerPC VX instruction 122E9F43 04-743 (1859)
        [Test]
        public void PPCDis_122E9F43()
        {
            AssertCode(0x122E9F43, "@@@");
        }

        // Unknown PowerPC VX instruction 12400090 04-090 (144)
        [Test]
        public void PPCDis_12400090()
        {
            AssertCode(0x12400090, "@@@");
        }

        // Unknown PowerPC VX instruction 12541F75 04-775 (1909)
        [Test]
        public void PPCDis_12541F75()
        {
            AssertCode(0x12541F75, "@@@");
        }

        // Unknown PowerPC VX instruction 1256BCBF 04-4BF(1215)
        [Test]
        public void PPCDis_1256BCBF()
        {
            AssertCode(0x1256BCBF, "@@@");
        }

        // Unknown PowerPC VX instruction 12600090 04-090 (144)
        [Test]
        public void PPCDis_12600090()
        {
            AssertCode(0x12600090, "@@@");
        }

        // Unknown PowerPC VX instruction 12708A36 04-236 (566)
        [Test]
        public void PPCDis_12708A36()
        {
            AssertCode(0x12708A36, "@@@");
        }

        // Unknown PowerPC VX instruction 12800090 04-090 (144)
        [Test]
        public void PPCDis_12800090()
        {
            AssertCode(0x12800090, "@@@");
        }

        // Unknown PowerPC VX instruction 128012B7 04-2B7(695)
        [Test]
        public void PPCDis_128012B7()
        {
            AssertCode(0x128012B7, "@@@");
        }

        // Unknown PowerPC VX instruction 1295BC60 04-460 (1120)
        [Test]
        public void PPCDis_1295BC60()
        {
            AssertCode(0x1295BC60, "@@@");
        }

        // Unknown PowerPC VX instruction 129D0040 04-040 (64)
        [Test]
        public void PPCDis_129D0040()
        {
            AssertCode(0x129D0040, "@@@");
        }

        // Unknown PowerPC VX instruction 12A00090 04-090 (144)
        [Test]
        public void PPCDis_12A00090()
        {
            AssertCode(0x12A00090, "@@@");
        }

        // Unknown PowerPC VX instruction 12C00090 04-090 (144)
        [Test]
        public void PPCDis_12C00090()
        {
            AssertCode(0x12C00090, "@@@");
        }

        // Unknown PowerPC VX instruction 12C34FCC 04-7CC(1996)
        [Test]
        public void PPCDis_12C34FCC()
        {
            AssertCode(0x12C34FCC, "@@@");
        }

        // Unknown PowerPC VX instruction 12D7AC60 04-460 (1120)
        [Test]
        public void PPCDis_12D7AC60()
        {
            AssertCode(0x12D7AC60, "@@@");
        }

        // Unknown PowerPC VX instruction 12E00090 04-090 (144)
        [Test]
        public void PPCDis_12E00090()
        {
            AssertCode(0x12E00090, "@@@");
        }

        // Unknown PowerPC VX instruction 12E005C9 04-5C9(1481)
        [Test]
        public void PPCDis_12E005C9()
        {
            AssertCode(0x12E005C9, "@@@");
        }

        // Unknown PowerPC VX instruction 12E0380C 04-00C(12)
        [Test]
        public void PPCDis_12E0380C()
        {
            AssertCode(0x12E0380C, "@@@");
        }

        // Unknown PowerPC VX instruction 12E34FCC 04-7CC(1996)
        [Test]
        public void PPCDis_12E34FCC()
        {
            AssertCode(0x12E34FCC, "@@@");
        }

        // Unknown PowerPC VX instruction 12EE1326 04-326 (806)
        [Test]
        public void PPCDis_12EE1326()
        {
            AssertCode(0x12EE1326, "@@@");
        }

        // Unknown PowerPC VX instruction 13000090 04-090 (144)
        [Test]
        public void PPCDis_13000090()
        {
            AssertCode(0x13000090, "@@@");
        }

        // Unknown PowerPC VX instruction 13000558 04-558 (1368)
        [Test]
        public void PPCDis_13000558()
        {
            AssertCode(0x13000558, "@@@");
        }

        // Unknown PowerPC VX instruction 1300380C 04-00C(12)
        [Test]
        public void PPCDis_1300380C()
        {
            AssertCode(0x1300380C, "@@@");
        }

        // Unknown PowerPC VX instruction 1301000C 04-00C(12)
        [Test]
        public void PPCDis_1301000C()
        {
            AssertCode(0x1301000C, "@@@");
        }

        // Unknown PowerPC VX instruction 1304C5DC 04-5DC(1500)
        [Test]
        public void PPCDis_1304C5DC()
        {
            AssertCode(0x1304C5DC, "@@@");
        }

        // Unknown PowerPC VX instruction 13080558 04-558 (1368)
        [Test]
        public void PPCDis_13080558()
        {
            AssertCode(0x13080558, "@@@");
        }

        // Unknown PowerPC VX instruction 130CC5DC 04-5DC(1500)
        [Test]
        public void PPCDis_130CC5DC()
        {
            AssertCode(0x130CC5DC, "@@@");
        }

        // Unknown PowerPC VX instruction 131AD4E0 04-4E0 (1248)
        [Test]
        public void PPCDis_131AD4E0()
        {
            AssertCode(0x131AD4E0, "@@@");
        }

        // Unknown PowerPC VX instruction 131B1A12 04-212 (530)
        [Test]
        public void PPCDis_131B1A12()
        {
            AssertCode(0x131B1A12, "@@@");
        }

        // Unknown PowerPC VX instruction 131B1A19 04-219 (537)
        [Test]
        public void PPCDis_131B1A19()
        {
            AssertCode(0x131B1A19, "@@@");
        }

        // Unknown PowerPC VX instruction 131E0040 04-040 (64)
        [Test]
        public void PPCDis_131E0040()
        {
            AssertCode(0x131E0040, "@@@");
        }

        // Unknown PowerPC VX instruction 13200090 04-090 (144)
        [Test]
        public void PPCDis_13200090()
        {
            AssertCode(0x13200090, "@@@");
        }

        // Unknown PowerPC VX instruction 13201747 04-747 (1863)
        [Test]
        public void PPCDis_13201747()
        {
            AssertCode(0x13201747, "@@@");
        }

        // Unknown PowerPC VX instruction 1320380C 04-00C(12)
        [Test]
        public void PPCDis_1320380C()
        {
            AssertCode(0x1320380C, "@@@");
        }

        // Unknown PowerPC VX instruction 13204BE2 04-3E2 (994)
        [Test]
        public void PPCDis_13204BE2()
        {
            AssertCode(0x13204BE2, "@@@");
        }

        // Unknown PowerPC VX instruction 1321000C 04-00C(12)
        [Test]
        public void PPCDis_1321000C()
        {
            AssertCode(0x1321000C, "@@@");
        }

        // Unknown PowerPC VX instruction 13210558 04-558 (1368)
        [Test]
        public void PPCDis_13210558()
        {
            AssertCode(0x13210558, "@@@");
        }

        // Unknown PowerPC VX instruction 1325CDDC 04-5DC(1500)
        [Test]
        public void PPCDis_1325CDDC()
        {
            AssertCode(0x1325CDDC, "@@@");
        }

        // Unknown PowerPC VX instruction 13290558 04-558 (1368)
        [Test]
        public void PPCDis_13290558()
        {
            AssertCode(0x13290558, "@@@");
        }

        // Unknown PowerPC VX instruction 133266F1 04-6F1 (1777)
        [Test]
        public void PPCDis_133266F1()
        {
            AssertCode(0x133266F1, "@@@");
        }

        // Unknown PowerPC VX instruction 13400090 04-090 (144)
        [Test]
        public void PPCDis_13400090()
        {
            AssertCode(0x13400090, "@@@");
        }

        // Unknown PowerPC VX instruction 13400298 04-298 (664)
        [Test]
        public void PPCDis_13400298()
        {
            AssertCode(0x13400298, "@@@");
        }

        // Unknown PowerPC VX instruction 1340380C 04-00C(12)
        [Test]
        public void PPCDis_1340380C()
        {
            AssertCode(0x1340380C, "@@@");
        }

        // Unknown PowerPC VX instruction 1341000C 04-00C(12)
        [Test]
        public void PPCDis_1341000C()
        {
            AssertCode(0x1341000C, "@@@");
        }

        // Unknown PowerPC VX instruction 1342D29E 04-29E (670)
        [Test]
        public void PPCDis_1342D29E()
        {
            AssertCode(0x1342D29E, "@@@");
        }

        // Unknown PowerPC VX instruction 1344D2DC 04-2DC(732)
        [Test]
        public void PPCDis_1344D2DC()
        {
            AssertCode(0x1344D2DC, "@@@");
        }

        // Unknown PowerPC VX instruction 1358D7DC 04-7DC(2012)
        [Test]
        public void PPCDis_1358D7DC()
        {
            AssertCode(0x1358D7DC, "@@@");
        }

        // Unknown PowerPC VX instruction 135F1399 04-399 (921)
        [Test]
        public void PPCDis_135F1399()
        {
            AssertCode(0x135F1399, "@@@");
        }

        // Unknown PowerPC VX instruction 13600090 04-090 (144)
        [Test]
        public void PPCDis_13600090()
        {
            AssertCode(0x13600090, "@@@");
        }

        // Unknown PowerPC VX instruction 1360380C 04-00C(12)
        [Test]
        public void PPCDis_1360380C()
        {
            AssertCode(0x1360380C, "@@@");
        }

        // Unknown PowerPC VX instruction 1361000C 04-00C(12)
        [Test]
        public void PPCDis_1361000C()
        {
            AssertCode(0x1361000C, "@@@");
        }

        // Unknown PowerPC VX instruction 13610298 04-298 (664)
        [Test]
        public void PPCDis_13610298()
        {
            AssertCode(0x13610298, "@@@");
        }

        // Unknown PowerPC VX instruction 1363DA9E 04-29E (670)
        [Test]
        public void PPCDis_1363DA9E()
        {
            AssertCode(0x1363DA9E, "@@@");
        }

        // Unknown PowerPC VX instruction 1365DADC 04-2DC(732)
        [Test]
        public void PPCDis_1365DADC()
        {
            AssertCode(0x1365DADC, "@@@");
        }

        // Unknown PowerPC VX instruction 1379DFDC 04-7DC(2012)
        [Test]
        public void PPCDis_1379DFDC()
        {
            AssertCode(0x1379DFDC, "@@@");
        }

        // Unknown PowerPC VX instruction 13800090 04-090 (144)
        [Test]
        public void PPCDis_13800090()
        {
            AssertCode(0x13800090, "@@@");
        }

        // Unknown PowerPC VX instruction 1380380C 04-00C(12)
        [Test]
        public void PPCDis_1380380C()
        {
            AssertCode(0x1380380C, "@@@");
        }

        // Unknown PowerPC VX instruction 1381000C 04-00C(12)
        [Test]
        public void PPCDis_1381000C()
        {
            AssertCode(0x1381000C, "@@@");
        }

        // Unknown PowerPC VX instruction 13840458 04-458 (1112)
        [Test]
        public void PPCDis_13840458()
        {
            AssertCode(0x13840458, "@@@");
        }

        // Unknown PowerPC VX instruction 13880458 04-458 (1112)
        [Test]
        public void PPCDis_13880458()
        {
            AssertCode(0x13880458, "@@@");
        }

        // Unknown PowerPC VX instruction 139F0040 04-040 (64)
        [Test]
        public void PPCDis_139F0040()
        {
            AssertCode(0x139F0040, "@@@");
        }

        // Unknown PowerPC VX instruction 13A00090 04-090 (144)
        [Test]
        public void PPCDis_13A00090()
        {
            AssertCode(0x13A00090, "@@@");
        }

        // Unknown PowerPC VX instruction 13A50458 04-458 (1112)
        [Test]
        public void PPCDis_13A50458()
        {
            AssertCode(0x13A50458, "@@@");
        }

        // Unknown PowerPC VX instruction 13A90458 04-458 (1112)
        [Test]
        public void PPCDis_13A90458()
        {
            AssertCode(0x13A90458, "@@@");
        }

        // Unknown PowerPC VX instruction 13A97D87 04-587 (1415)
        [Test]
        public void PPCDis_13A97D87()
        {
            AssertCode(0x13A97D87, "@@@");
        }

        // Unknown PowerPC VX instruction 13B6670B 04-70B(1803)
        [Test]
        public void PPCDis_13B6670B()
        {
            AssertCode(0x13B6670B, "@@@");
        }

        // Unknown PowerPC VX instruction 13BD039A 04-39A(922)
        [Test]
        public void PPCDis_13BD039A()
        {
            AssertCode(0x13BD039A, "@@@");
        }

        // Unknown PowerPC VX instruction 13C00090 04-090 (144)
        [Test]
        public void PPCDis_13C00090()
        {
            AssertCode(0x13C00090, "@@@");
        }

        // Unknown PowerPC VX instruction 13D90008 04-008 (8)
        [Test]
        public void PPCDis_13D90008()
        {
            AssertCode(0x13D90008, "@@@");
        }

        // Unknown PowerPC VX instruction 13E00090 04-090 (144)
        [Test]
        public void PPCDis_13E00090()
        {
            AssertCode(0x13E00090, "@@@");
        }




        // Unknown PowerPC X instruction 4C0000FE 13-07F (127)
        [Test]
        public void PPCDis_4C0000FE()
        {
            AssertCode(0x4C0000FE, "@@@");
        }

        // Unknown PowerPC X instruction 4C001B5E 13-1AF(431)
        [Test]
        public void PPCDis_4C001B5E()
        {
            AssertCode(0x4C001B5E, "@@@");
        }

        // Unknown PowerPC X instruction 4C001C7E 13-23F (575)
        [Test]
        public void PPCDis_4C001C7E()
        {
            AssertCode(0x4C001C7E, "@@@");
        }

        // Unknown PowerPC X instruction 4C00424F 13-127 (295)
        [Test]
        public void PPCDis_4C00424F()
        {
            AssertCode(0x4C00424F, "@@@");
        }

        // Unknown PowerPC X instruction 4C004D4B 13-2A5(677)
        [Test]
        public void PPCDis_4C004D4B()
        {
            AssertCode(0x4C004D4B, "@@@");
        }

        // Unknown PowerPC X instruction 4C005061 13-030 (48)
        [Test]
        public void PPCDis_4C005061()
        {
            AssertCode(0x4C005061, "@@@");
        }

        // Unknown PowerPC X instruction 4C005348 13-1A4(420)
        [Test]
        public void PPCDis_4C005348()
        {
            AssertCode(0x4C005348, "@@@");
        }

        // Unknown PowerPC X instruction 4C111C40 13-220 (544)
        [Test]
        public void PPCDis_4C111C40()
        {
            AssertCode(0x4C111C40, "@@@");
        }

        // Unknown PowerPC X instruction 4C119AB0 13-158 (344)
        [Test]
        public void PPCDis_4C119AB0()
        {
            AssertCode(0x4C119AB0, "@@@");
        }

        // Unknown PowerPC X instruction 4C204D55 13-2AA(682)
        [Test]
        public void PPCDis_4C204D55()
        {
            AssertCode(0x4C204D55, "@@@");
        }

        // Unknown PowerPC X instruction 4C204F4E 13-3A7(935)
        [Test]
        public void PPCDis_4C204F4E()
        {
            AssertCode(0x4C204F4E, "@@@");
        }

        // Unknown PowerPC X instruction 4C24D37E 13-1BF(447)
        [Test]
        public void PPCDis_4C24D37E()
        {
            AssertCode(0x4C24D37E, "@@@");
        }

        // Unknown PowerPC X instruction 4C312069 13-034 (52)
        [Test]
        public void PPCDis_4C312069()
        {
            AssertCode(0x4C312069, "@@@");
        }

        // Unknown PowerPC X instruction 4C322063 13-031 (49)
        [Test]
        public void PPCDis_4C322063()
        {
            AssertCode(0x4C322063, "@@@");
        }

        // Unknown PowerPC X instruction 4C41494C 13-0A6(166)
        [Test]
        public void PPCDis_4C41494C()
        {
            AssertCode(0x4C41494C, "@@@");
        }

        // Unknown PowerPC X instruction 4C414D45 13-2A2(674)
        [Test]
        public void PPCDis_4C414D45()
        {
            AssertCode(0x4C414D45, "@@@");
        }

        // Unknown PowerPC X instruction 4C414E44 13-322 (802)
        [Test]
        public void PPCDis_4C414E44()
        {
            AssertCode(0x4C414E44, "@@@");
        }

        // Unknown PowerPC X instruction 4C414E4B 13-325 (805)
        [Test]
        public void PPCDis_4C414E4B()
        {
            AssertCode(0x4C414E4B, "@@@");
        }

        // Unknown PowerPC X instruction 4C41505D 13-02E (46)
        [Test]
        public void PPCDis_4C41505D()
        {
            AssertCode(0x4C41505D, "@@@");
        }

        // Unknown PowerPC X instruction 4C41534D 13-1A6(422)
        [Test]
        public void PPCDis_4C41534D()
        {
            AssertCode(0x4C41534D, "@@@");
        }

        // Unknown PowerPC X instruction 4C415445 13-222 (546)
        [Test]
        public void PPCDis_4C415445()
        {
            AssertCode(0x4C415445, "@@@");
        }

        // Unknown PowerPC X instruction 4C415446 13-223 (547)
        [Test]
        public void PPCDis_4C415446()
        {
            AssertCode(0x4C415446, "@@@");
        }

        // Unknown PowerPC X instruction 4C41545F 13-22F (559)
        [Test]
        public void PPCDis_4C41545F()
        {
            AssertCode(0x4C41545F, "@@@");
        }

        // Unknown PowerPC X instruction 4C415645 13-322 (802)
        [Test]
        public void PPCDis_4C415645()
        {
            AssertCode(0x4C415645, "@@@");
        }

        // Unknown PowerPC X instruction 4C424F4E 13-3A7(935)
        [Test]
        public void PPCDis_4C424F4E()
        {
            AssertCode(0x4C424F4E, "@@@");
        }

        // Unknown PowerPC X instruction 4C42616C 13-0B6 (182)
        [Test]
        public void PPCDis_4C42616C()
        {
            AssertCode(0x4C42616C, "@@@");
        }

        // Unknown PowerPC X instruction 4C436F70 13-3B8(952)
        [Test]
        public void PPCDis_4C436F70()
        {
            AssertCode(0x4C436F70, "@@@");
        }

        // Unknown PowerPC X instruction 4C454400 13-200 (512)
        [Test]
        public void PPCDis_4C454400()
        {
            AssertCode(0x4C454400, "@@@");
        }

        // Unknown PowerPC X instruction 4C45525F 13-12F (303)
        [Test]
        public void PPCDis_4C45525F()
        {
            AssertCode(0x4C45525F, "@@@");
        }

        // Unknown PowerPC X instruction 4C455445 13-222 (546)
        [Test]
        public void PPCDis_4C455445()
        {
            AssertCode(0x4C455445, "@@@");
        }

        // Unknown PowerPC X instruction 4C455452 13-229 (553)
        [Test]
        public void PPCDis_4C455452()
        {
            AssertCode(0x4C455452, "@@@");
        }

        // Unknown PowerPC X instruction 4C455645 13-322 (802)
        [Test]
        public void PPCDis_4C455645()
        {
            AssertCode(0x4C455645, "@@@");
        }

        // Unknown PowerPC X instruction 4C495445 13-222 (546)
        [Test]
        public void PPCDis_4C495445()
        {
            AssertCode(0x4C495445, "@@@");
        }

        // Unknown PowerPC X instruction 4C4B5F53 13-3A9(937)
        [Test]
        public void PPCDis_4C4B5F53()
        {
            AssertCode(0x4C4B5F53, "@@@");
        }

        // Unknown PowerPC X instruction 4C4C2055 13-02A(42)
        [Test]
        public void PPCDis_4C4C2055()
        {
            AssertCode(0x4C4C2055, "@@@");
        }

        // Unknown PowerPC X instruction 4C4C4552 13-2A9(681)
        [Test]
        public void PPCDis_4C4C4552()
        {
            AssertCode(0x4C4C4552, "@@@");
        }

        // Unknown PowerPC X instruction 4C4C4F57 13-3AB(939)
        [Test]
        public void PPCDis_4C4C4F57()
        {
            AssertCode(0x4C4C4F57, "@@@");
        }

        // Unknown PowerPC X instruction 4C4F424D 13-126 (294)
        [Test]
        public void PPCDis_4C4F424D()
        {
            AssertCode(0x4C4F424D, "@@@");
        }

        // Unknown PowerPC X instruction 4C4F5452 13-229 (553)
        [Test]
        public void PPCDis_4C4F5452()
        {
            AssertCode(0x4C4F5452, "@@@");
        }

        // Unknown PowerPC X instruction 4C4F5544 13-2A2(674)
        [Test]
        public void PPCDis_4C4F5544()
        {
            AssertCode(0x4C4F5544, "@@@");
        }

        // Unknown PowerPC X instruction 4C4F5645 13-322 (802)
        [Test]
        public void PPCDis_4C4F5645()
        {
            AssertCode(0x4C4F5645, "@@@");
        }

        // Unknown PowerPC X instruction 4C4F5F53 13-3A9(937)
        [Test]
        public void PPCDis_4C4F5F53()
        {
            AssertCode(0x4C4F5F53, "@@@");
        }

        // Unknown PowerPC X instruction 4C542030 13-018 (24)
        [Test]
        public void PPCDis_4C542030()
        {
            AssertCode(0x4C542030, "@@@");
        }

        // Unknown PowerPC X instruction 4C545F46 13-3A3(931)
        [Test]
        public void PPCDis_4C545F46()
        {
            AssertCode(0x4C545F46, "@@@");
        }

        // Unknown PowerPC X instruction 4C545F48 13-3A4(932)
        [Test]
        public void PPCDis_4C545F48()
        {
            AssertCode(0x4C545F48, "@@@");
        }

        // Unknown PowerPC X instruction 4C554520 13-290 (656)
        [Test]
        public void PPCDis_4C554520()
        {
            AssertCode(0x4C554520, "@@@");
        }

        // Unknown PowerPC X instruction 4C55475D 13-3AE(942)
        [Test]
        public void PPCDis_4C55475D()
        {
            AssertCode(0x4C55475D, "@@@");
        }

        // Unknown PowerPC X instruction 4C595D2E 13-297 (663)
        [Test]
        public void PPCDis_4C595D2E()
        {
            AssertCode(0x4C595D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4C5D2E72 13-339 (825)
        [Test]
        public void PPCDis_4C5D2E72()
        {
            AssertCode(0x4C5D2E72, "@@@");
        }

        // Unknown PowerPC X instruction 4C5F5041 13-020 (32)
        [Test]
        public void PPCDis_4C5F5041()
        {
            AssertCode(0x4C5F5041, "@@@");
        }

        // Unknown PowerPC X instruction 4C616E64 13-332 (818)
        [Test]
        public void PPCDis_4C616E64()
        {
            AssertCode(0x4C616E64, "@@@");
        }

        // Unknown PowerPC X instruction 4C617373 13-1B9(441)
        [Test]
        public void PPCDis_4C617373()
        {
            AssertCode(0x4C617373, "@@@");
        }

        // Unknown PowerPC X instruction 4C61756E 13-2B7(695)
        [Test]
        public void PPCDis_4C61756E()
        {
            AssertCode(0x4C61756E, "@@@");
        }

        // Unknown PowerPC X instruction 4C656164 13-0B2 (178)
        [Test]
        public void PPCDis_4C656164()
        {
            AssertCode(0x4C656164, "@@@");
        }

        // Unknown PowerPC X instruction 4C65616B 13-0B5 (181)
        [Test]
        public void PPCDis_4C65616B()
        {
            AssertCode(0x4C65616B, "@@@");
        }

        // Unknown PowerPC X instruction 4C656674 13-33A(826)
        [Test]
        public void PPCDis_4C656674()
        {
            AssertCode(0x4C656674, "@@@");
        }

        // Unknown PowerPC X instruction 4C656E67 13-333 (819)
        [Test]
        public void PPCDis_4C656E67()
        {
            AssertCode(0x4C656E67, "@@@");
        }

        // Unknown PowerPC X instruction 4C657474 13-23A(570)
        [Test]
        public void PPCDis_4C657474()
        {
            AssertCode(0x4C657474, "@@@");
        }

        // Unknown PowerPC X instruction 4C65F67A 13-33D (829)
        [Test]
        public void PPCDis_4C65F67A()
        {
            AssertCode(0x4C65F67A, "@@@");
        }

        // Unknown PowerPC X instruction 4C696665 13-332 (818)
        [Test]
        public void PPCDis_4C696665()
        {
            AssertCode(0x4C696665, "@@@");
        }

        // Unknown PowerPC X instruction 4C696674 13-33A(826)
        [Test]
        public void PPCDis_4C696674()
        {
            AssertCode(0x4C696674, "@@@");
        }

        // Unknown PowerPC X instruction 4C696768 13-3B4(948)
        [Test]
        public void PPCDis_4C696768()
        {
            AssertCode(0x4C696768, "@@@");
        }

        // Unknown PowerPC X instruction 4C696E65 13-332 (818)
        [Test]
        public void PPCDis_4C696E65()
        {
            AssertCode(0x4C696E65, "@@@");
        }

        // Unknown PowerPC X instruction 4C69EECE 13-367 (871)
        [Test]
        public void PPCDis_4C69EECE()
        {
            AssertCode(0x4C69EECE, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F6164 13-0B2 (178)
        [Test]
        public void PPCDis_4C6F6164()
        {
            AssertCode(0x4C6F6164, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F6361 13-1B0(432)
        [Test]
        public void PPCDis_4C6F6361()
        {
            AssertCode(0x4C6F6361, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F6F6B 13-3B5(949)
        [Test]
        public void PPCDis_4C6F6F6B()
        {
            AssertCode(0x4C6F6F6B, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F6F70 13-3B8(952)
        [Test]
        public void PPCDis_4C6F6F70()
        {
            AssertCode(0x4C6F6F70, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F7365 13-1B2(434)
        [Test]
        public void PPCDis_4C6F7365()
        {
            AssertCode(0x4C6F7365, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F7665 13-332 (818)
        [Test]
        public void PPCDis_4C6F7665()
        {
            AssertCode(0x4C6F7665, "@@@");
        }

        // Unknown PowerPC X instruction 4C8C1527 13-293 (659)
        [Test]
        public void PPCDis_4C8C1527()
        {
            AssertCode(0x4C8C1527, "@@@");
        }

        // Unknown PowerPC X instruction 4C916FED 13-3F6 (1014)
        [Test]
        public void PPCDis_4C916FED()
        {
            AssertCode(0x4C916FED, "@@@");
        }

        // Unknown PowerPC X instruction 4C9B684E 13-027 (39)
        [Test]
        public void PPCDis_4C9B684E()
        {
            AssertCode(0x4C9B684E, "@@@");
        }

        // Unknown PowerPC X instruction 4CA03CBA 13-25D (605)
        [Test]
        public void PPCDis_4CA03CBA()
        {
            AssertCode(0x4CA03CBA, "@@@");
        }

        // Unknown PowerPC X instruction 4CC710DA 13-06D (109)
        [Test]
        public void PPCDis_4CC710DA()
        {
            AssertCode(0x4CC710DA, "@@@");
        }

        // Unknown PowerPC X instruction 4CECC21A 13-10D (269)
        [Test]
        public void PPCDis_4CECC21A()
        {
            AssertCode(0x4CECC21A, "@@@");
        }

        // Unknown PowerPC X instruction 4D00157E 13-2BF(703)
        [Test]
        public void PPCDis_4D00157E()
        {
            AssertCode(0x4D00157E, "@@@");
        }

        // Unknown PowerPC X instruction 4D0202BD 13-15E (350)
        [Test]
        public void PPCDis_4D0202BD()
        {
            AssertCode(0x4D0202BD, "@@@");
        }

        // Unknown PowerPC X instruction 4D128DBE 13-2DF(735)
        [Test]
        public void PPCDis_4D128DBE()
        {
            AssertCode(0x4D128DBE, "@@@");
        }

        // Unknown PowerPC X instruction 4D1C71CD 13-0E6 (230)
        [Test]
        public void PPCDis_4D1C71CD()
        {
            AssertCode(0x4D1C71CD, "@@@");
        }

        // Unknown PowerPC X instruction 4D204752 13-3A9(937)
        [Test]
        public void PPCDis_4D204752()
        {
            AssertCode(0x4D204752, "@@@");
        }

        // Unknown PowerPC X instruction 4D204E05 13-302 (770)
        [Test]
        public void PPCDis_4D204E05()
        {
            AssertCode(0x4D204E05, "@@@");
        }

        // Unknown PowerPC X instruction 4D2F8C18 13-20C(524)
        [Test]
        public void PPCDis_4D2F8C18()
        {
            AssertCode(0x4D2F8C18, "@@@");
        }

        // Unknown PowerPC X instruction 4D38DC45 13-222 (546)
        [Test]
        public void PPCDis_4D38DC45()
        {
            AssertCode(0x4D38DC45, "@@@");
        }

        // Unknown PowerPC X instruction 4D412041 13-020 (32)
        [Test]
        public void PPCDis_4D412041()
        {
            AssertCode(0x4D412041, "@@@");
        }

        // Unknown PowerPC X instruction 4D414E00 13-300 (768)
        [Test]
        public void PPCDis_4D414E00()
        {
            AssertCode(0x4D414E00, "@@@");
        }

        // Unknown PowerPC X instruction 4D424943 13-0A1(161)
        [Test]
        public void PPCDis_4D424943()
        {
            AssertCode(0x4D424943, "@@@");
        }

        // Unknown PowerPC X instruction 4D424945 13-0A2(162)
        [Test]
        public void PPCDis_4D424945()
        {
            AssertCode(0x4D424945, "@@@");
        }

        // Unknown PowerPC X instruction 4D434175 13-0BA(186)
        [Test]
        public void PPCDis_4D434175()
        {
            AssertCode(0x4D434175, "@@@");
        }

        // Unknown PowerPC X instruction 4D435053 13-029 (41)
        [Test]
        public void PPCDis_4D435053()
        {
            AssertCode(0x4D435053, "@@@");
        }

        // Unknown PowerPC X instruction 4D454348 13-1A4(420)
        [Test]
        public void PPCDis_4D454348()
        {
            AssertCode(0x4D454348, "@@@");
        }

        // Unknown PowerPC X instruction 4D455441 13-220 (544)
        [Test]
        public void PPCDis_4D455441()
        {
            AssertCode(0x4D455441, "@@@");
        }

        // Unknown PowerPC X instruction 4D455D2E 13-297 (663)
        [Test]
        public void PPCDis_4D455D2E()
        {
            AssertCode(0x4D455D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4D475220 13-110 (272)
        [Test]
        public void PPCDis_4D475220()
        {
            AssertCode(0x4D475220, "@@@");
        }

        // Unknown PowerPC X instruction 4D47DF84 13-3C2(962)
        [Test]
        public void PPCDis_4D47DF84()
        {
            AssertCode(0x4D47DF84, "@@@");
        }

        // Unknown PowerPC X instruction 4D49545F 13-22F (559)
        [Test]
        public void PPCDis_4D49545F()
        {
            AssertCode(0x4D49545F, "@@@");
        }

        // Unknown PowerPC X instruction 4D4D5D2E 13-297 (663)
        [Test]
        public void PPCDis_4D4D5D2E()
        {
            AssertCode(0x4D4D5D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4D4E0054 13-02A(42)
        [Test]
        public void PPCDis_4D4E0054()
        {
            AssertCode(0x4D4E0054, "@@@");
        }

        // Unknown PowerPC X instruction 4D4E5533 13-299 (665)
        [Test]
        public void PPCDis_4D4E5533()
        {
            AssertCode(0x4D4E5533, "@@@");
        }

        // Unknown PowerPC X instruction 4D4E5534 13-29A(666)
        [Test]
        public void PPCDis_4D4E5534()
        {
            AssertCode(0x4D4E5534, "@@@");
        }

        // Unknown PowerPC X instruction 4D4F4B45 13-1A2(418)
        [Test]
        public void PPCDis_4D4F4B45()
        {
            AssertCode(0x4D4F4B45, "@@@");
        }

        // Unknown PowerPC X instruction 4D4F5245 13-122 (290)
        [Test]
        public void PPCDis_4D4F5245()
        {
            AssertCode(0x4D4F5245, "@@@");
        }

        // Unknown PowerPC X instruction 4D4F5645 13-322 (802)
        [Test]
        public void PPCDis_4D4F5645()
        {
            AssertCode(0x4D4F5645, "@@@");
        }

        // Unknown PowerPC X instruction 4D5188A7 13-053 (83)
        [Test]
        public void PPCDis_4D5188A7()
        {
            AssertCode(0x4D5188A7, "@@@");
        }

        // Unknown PowerPC X instruction 4D535049 13-024 (36)
        [Test]
        public void PPCDis_4D535049()
        {
            AssertCode(0x4D535049, "@@@");
        }

        // Unknown PowerPC X instruction 4D5D2E64 13-332 (818)
        [Test]
        public void PPCDis_4D5D2E64()
        {
            AssertCode(0x4D5D2E64, "@@@");
        }

        // Unknown PowerPC X instruction 4D5D2E76 13-33B(827)
        [Test]
        public void PPCDis_4D5D2E76()
        {
            AssertCode(0x4D5D2E76, "@@@");
        }

        // Unknown PowerPC X instruction 4D5F4A45 13-122 (290)
        [Test]
        public void PPCDis_4D5F4A45()
        {
            AssertCode(0x4D5F4A45, "@@@");
        }

        // Unknown PowerPC X instruction 4D61676E 13-3B7(951)
        [Test]
        public void PPCDis_4D61676E()
        {
            AssertCode(0x4D61676E, "@@@");
        }

        // Unknown PowerPC X instruction 4D617374 13-1BA(442)
        [Test]
        public void PPCDis_4D617374()
        {
            AssertCode(0x4D617374, "@@@");
        }

        // Unknown PowerPC X instruction 4D617463 13-231 (561)
        [Test]
        public void PPCDis_4D617463()
        {
            AssertCode(0x4D617463, "@@@");
        }

        // Unknown PowerPC X instruction 4D617849 13-024 (36)
        [Test]
        public void PPCDis_4D617849()
        {
            AssertCode(0x4D617849, "@@@");
        }

        // Unknown PowerPC X instruction 4D617853 13-029 (41)
        [Test]
        public void PPCDis_4D617853()
        {
            AssertCode(0x4D617853, "@@@");
        }

        // Unknown PowerPC X instruction 4D650046 13-023 (35)
        [Test]
        public void PPCDis_4D650046()
        {
            AssertCode(0x4D650046, "@@@");
        }

        // Unknown PowerPC X instruction 4D656C65 13-232 (562)
        [Test]
        public void PPCDis_4D656C65()
        {
            AssertCode(0x4D656C65, "@@@");
        }

        // Unknown PowerPC X instruction 4D656D6F 13-2B7(695)
        [Test]
        public void PPCDis_4D656D6F()
        {
            AssertCode(0x4D656D6F, "@@@");
        }

        // Unknown PowerPC X instruction 4D657472 13-239 (569)
        [Test]
        public void PPCDis_4D657472()
        {
            AssertCode(0x4D657472, "@@@");
        }

        // Unknown PowerPC X instruction 4D696E64 13-332 (818)
        [Test]
        public void PPCDis_4D696E64()
        {
            AssertCode(0x4D696E64, "@@@");
        }

        // Unknown PowerPC X instruction 4D6F6465 13-232 (562)
        [Test]
        public void PPCDis_4D6F6465()
        {
            AssertCode(0x4D6F6465, "@@@");
        }

        // Unknown PowerPC X instruction 4D6F756E 13-2B7(695)
        [Test]
        public void PPCDis_4D6F756E()
        {
            AssertCode(0x4D6F756E, "@@@");
        }

        // Unknown PowerPC X instruction 4D6F7665 13-332 (818)
        [Test]
        public void PPCDis_4D6F7665()
        {
            AssertCode(0x4D6F7665, "@@@");
        }

        // Unknown PowerPC X instruction 4D6F7669 13-334 (820)
        [Test]
        public void PPCDis_4D6F7669()
        {
            AssertCode(0x4D6F7669, "@@@");
        }

        // Unknown PowerPC X instruction 4D757369 13-1B4(436)
        [Test]
        public void PPCDis_4D757369()
        {
            AssertCode(0x4D757369, "@@@");
        }

        // Unknown PowerPC X instruction 4D772FC0 13-3E0 (992)
        [Test]
        public void PPCDis_4D772FC0()
        {
            AssertCode(0x4D772FC0, "@@@");
        }

        // Unknown PowerPC X instruction 4DABE0E1 13-070 (112)
        [Test]
        public void PPCDis_4DABE0E1()
        {
            AssertCode(0x4DABE0E1, "@@@");
        }

        // Unknown PowerPC X instruction 4DDDA0AE 13-057 (87)
        [Test]
        public void PPCDis_4DDDA0AE()
        {
            AssertCode(0x4DDDA0AE, "@@@");
        }

        // Unknown PowerPC X instruction 4DE96EA9 13-354 (852)
        [Test]
        public void PPCDis_4DE96EA9()
        {
            AssertCode(0x4DE96EA9, "@@@");
        }

        // Unknown PowerPC X instruction 4DF06BFB 13-1FD(509)
        [Test]
        public void PPCDis_4DF06BFB()
        {
            AssertCode(0x4DF06BFB, "@@@");
        }

        // Unknown PowerPC X instruction 4E004E50 13-328 (808)
        [Test]
        public void PPCDis_4E004E50()
        {
            AssertCode(0x4E004E50, "@@@");
        }

        // Unknown PowerPC X instruction 4E0A0268 13-134 (308)
        [Test]
        public void PPCDis_4E0A0268()
        {
            AssertCode(0x4E0A0268, "@@@");
        }

        // Unknown PowerPC X instruction 4E204E56 13-32B(811)
        [Test]
        public void PPCDis_4E204E56()
        {
            AssertCode(0x4E204E56, "@@@");
        }

        // Unknown PowerPC X instruction 4E205341 13-1A0(416)
        [Test]
        public void PPCDis_4E205341()
        {
            AssertCode(0x4E205341, "@@@");
        }

        // Unknown PowerPC X instruction 4E205549 13-2A4(676)
        [Test]
        public void PPCDis_4E205549()
        {
            AssertCode(0x4E205549, "@@@");
        }

        // Unknown PowerPC X instruction 4E2101DE 13-0EF (239)
        [Test]
        public void PPCDis_4E2101DE()
        {
            AssertCode(0x4E2101DE, "@@@");
        }

        // Unknown PowerPC X instruction 4E412045 13-022 (34)
        [Test]
        public void PPCDis_4E412045()
        {
            AssertCode(0x4E412045, "@@@");
        }

        // Unknown PowerPC X instruction 4E440050 13-028 (40)
        [Test]
        public void PPCDis_4E440050()
        {
            AssertCode(0x4E440050, "@@@");
        }

        // Unknown PowerPC X instruction 4E443100 13-080 (128)
        [Test]
        public void PPCDis_4E443100()
        {
            AssertCode(0x4E443100, "@@@");
        }

        // Unknown PowerPC X instruction 4E443300 13-180 (384)
        [Test]
        public void PPCDis_4E443300()
        {
            AssertCode(0x4E443300, "@@@");
        }

        // Unknown PowerPC X instruction 4E445550 13-2A8(680)
        [Test]
        public void PPCDis_4E445550()
        {
            AssertCode(0x4E445550, "@@@");
        }

        // Unknown PowerPC X instruction 4E445D2E 13-297 (663)
        [Test]
        public void PPCDis_4E445D2E()
        {
            AssertCode(0x4E445D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F41 13-3A0(928)
        [Test]
        public void PPCDis_4E445F41()
        {
            AssertCode(0x4E445F41, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F42 13-3A1(929)
        [Test]
        public void PPCDis_4E445F42()
        {
            AssertCode(0x4E445F42, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F43 13-3A1(929)
        [Test]
        public void PPCDis_4E445F43()
        {
            AssertCode(0x4E445F43, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F47 13-3A3(931)
        [Test]
        public void PPCDis_4E445F47()
        {
            AssertCode(0x4E445F47, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F48 13-3A4(932)
        [Test]
        public void PPCDis_4E445F48()
        {
            AssertCode(0x4E445F48, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F4B 13-3A5(933)
        [Test]
        public void PPCDis_4E445F4B()
        {
            AssertCode(0x4E445F4B, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F4C 13-3A6(934)
        [Test]
        public void PPCDis_4E445F4C()
        {
            AssertCode(0x4E445F4C, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F4D 13-3A6(934)
        [Test]
        public void PPCDis_4E445F4D()
        {
            AssertCode(0x4E445F4D, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F4F 13-3A7(935)
        [Test]
        public void PPCDis_4E445F4F()
        {
            AssertCode(0x4E445F4F, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F50 13-3A8(936)
        [Test]
        public void PPCDis_4E445F50()
        {
            AssertCode(0x4E445F50, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F53 13-3A9(937)
        [Test]
        public void PPCDis_4E445F53()
        {
            AssertCode(0x4E445F53, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F54 13-3AA(938)
        [Test]
        public void PPCDis_4E445F54()
        {
            AssertCode(0x4E445F54, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F56 13-3AB(939)
        [Test]
        public void PPCDis_4E445F56()
        {
            AssertCode(0x4E445F56, "@@@");
        }

        // Unknown PowerPC X instruction 4E450057 13-02B(43)
        [Test]
        public void PPCDis_4E450057()
        {
            AssertCode(0x4E450057, "@@@");
        }

        // Unknown PowerPC X instruction 4E452030 13-018 (24)
        [Test]
        public void PPCDis_4E452030()
        {
            AssertCode(0x4E452030, "@@@");
        }

        // Unknown PowerPC X instruction 4E455753 13-3A9(937)
        [Test]
        public void PPCDis_4E455753()
        {
            AssertCode(0x4E455753, "@@@");
        }

        // Unknown PowerPC X instruction 4E46006C 13-036 (54)
        [Test]
        public void PPCDis_4E46006C()
        {
            AssertCode(0x4E46006C, "@@@");
        }

        // Unknown PowerPC X instruction 4E464952 13-0A9(169)
        [Test]
        public void PPCDis_4E464952()
        {
            AssertCode(0x4E464952, "@@@");
        }

        // Unknown PowerPC X instruction 4E474230 13-118 (280)
        [Test]
        public void PPCDis_4E474230()
        {
            AssertCode(0x4E474230, "@@@");
        }

        // Unknown PowerPC X instruction 4E474231 13-118 (280)
        [Test]
        public void PPCDis_4E474231()
        {
            AssertCode(0x4E474231, "@@@");
        }

        // Unknown PowerPC X instruction 4E474232 13-119 (281)
        [Test]
        public void PPCDis_4E474232()
        {
            AssertCode(0x4E474232, "@@@");
        }

        // Unknown PowerPC X instruction 4E474233 13-119 (281)
        [Test]
        public void PPCDis_4E474233()
        {
            AssertCode(0x4E474233, "@@@");
        }

        // Unknown PowerPC X instruction 4E474234 13-11A(282)
        [Test]
        public void PPCDis_4E474234()
        {
            AssertCode(0x4E474234, "@@@");
        }

        // Unknown PowerPC X instruction 4E474235 13-11A(282)
        [Test]
        public void PPCDis_4E474235()
        {
            AssertCode(0x4E474235, "@@@");
        }

        // Unknown PowerPC X instruction 4E474236 13-11B(283)
        [Test]
        public void PPCDis_4E474236()
        {
            AssertCode(0x4E474236, "@@@");
        }

        // Unknown PowerPC X instruction 4E474237 13-11B(283)
        [Test]
        public void PPCDis_4E474237()
        {
            AssertCode(0x4E474237, "@@@");
        }

        // Unknown PowerPC X instruction 4E474238 13-11C(284)
        [Test]
        public void PPCDis_4E474238()
        {
            AssertCode(0x4E474238, "@@@");
        }

        // Unknown PowerPC X instruction 4E474239 13-11C(284)
        [Test]
        public void PPCDis_4E474239()
        {
            AssertCode(0x4E474239, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423A 13-11D (285)
        [Test]
        public void PPCDis_4E47423A()
        {
            AssertCode(0x4E47423A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423B 13-11D (285)
        [Test]
        public void PPCDis_4E47423B()
        {
            AssertCode(0x4E47423B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423C 13-11E (286)
        [Test]
        public void PPCDis_4E47423C()
        {
            AssertCode(0x4E47423C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423D 13-11E (286)
        [Test]
        public void PPCDis_4E47423D()
        {
            AssertCode(0x4E47423D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423E 13-11F (287)
        [Test]
        public void PPCDis_4E47423E()
        {
            AssertCode(0x4E47423E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423F 13-11F (287)
        [Test]
        public void PPCDis_4E47423F()
        {
            AssertCode(0x4E47423F, "@@@");
        }

        // Unknown PowerPC X instruction 4E474240 13-120 (288)
        [Test]
        public void PPCDis_4E474240()
        {
            AssertCode(0x4E474240, "@@@");
        }

        // Unknown PowerPC X instruction 4E474241 13-120 (288)
        [Test]
        public void PPCDis_4E474241()
        {
            AssertCode(0x4E474241, "@@@");
        }

        // Unknown PowerPC X instruction 4E474244 13-122 (290)
        [Test]
        public void PPCDis_4E474244()
        {
            AssertCode(0x4E474244, "@@@");
        }

        // Unknown PowerPC X instruction 4E474245 13-122 (290)
        [Test]
        public void PPCDis_4E474245()
        {
            AssertCode(0x4E474245, "@@@");
        }

        // Unknown PowerPC X instruction 4E474246 13-123 (291)
        [Test]
        public void PPCDis_4E474246()
        {
            AssertCode(0x4E474246, "@@@");
        }

        // Unknown PowerPC X instruction 4E474247 13-123 (291)
        [Test]
        public void PPCDis_4E474247()
        {
            AssertCode(0x4E474247, "@@@");
        }

        // Unknown PowerPC X instruction 4E474248 13-124 (292)
        [Test]
        public void PPCDis_4E474248()
        {
            AssertCode(0x4E474248, "@@@");
        }

        // Unknown PowerPC X instruction 4E474249 13-124 (292)
        [Test]
        public void PPCDis_4E474249()
        {
            AssertCode(0x4E474249, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424A 13-125 (293)
        [Test]
        public void PPCDis_4E47424A()
        {
            AssertCode(0x4E47424A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424B 13-125 (293)
        [Test]
        public void PPCDis_4E47424B()
        {
            AssertCode(0x4E47424B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424C 13-126 (294)
        [Test]
        public void PPCDis_4E47424C()
        {
            AssertCode(0x4E47424C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424D 13-126 (294)
        [Test]
        public void PPCDis_4E47424D()
        {
            AssertCode(0x4E47424D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424E 13-127 (295)
        [Test]
        public void PPCDis_4E47424E()
        {
            AssertCode(0x4E47424E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424F 13-127 (295)
        [Test]
        public void PPCDis_4E47424F()
        {
            AssertCode(0x4E47424F, "@@@");
        }

        // Unknown PowerPC X instruction 4E474250 13-128 (296)
        [Test]
        public void PPCDis_4E474250()
        {
            AssertCode(0x4E474250, "@@@");
        }

        // Unknown PowerPC X instruction 4E474251 13-128 (296)
        [Test]
        public void PPCDis_4E474251()
        {
            AssertCode(0x4E474251, "@@@");
        }

        // Unknown PowerPC X instruction 4E474252 13-129 (297)
        [Test]
        public void PPCDis_4E474252()
        {
            AssertCode(0x4E474252, "@@@");
        }

        // Unknown PowerPC X instruction 4E474253 13-129 (297)
        [Test]
        public void PPCDis_4E474253()
        {
            AssertCode(0x4E474253, "@@@");
        }

        // Unknown PowerPC X instruction 4E474254 13-12A(298)
        [Test]
        public void PPCDis_4E474254()
        {
            AssertCode(0x4E474254, "@@@");
        }

        // Unknown PowerPC X instruction 4E474255 13-12A(298)
        [Test]
        public void PPCDis_4E474255()
        {
            AssertCode(0x4E474255, "@@@");
        }

        // Unknown PowerPC X instruction 4E474256 13-12B(299)
        [Test]
        public void PPCDis_4E474256()
        {
            AssertCode(0x4E474256, "@@@");
        }

        // Unknown PowerPC X instruction 4E474257 13-12B(299)
        [Test]
        public void PPCDis_4E474257()
        {
            AssertCode(0x4E474257, "@@@");
        }

        // Unknown PowerPC X instruction 4E474258 13-12C(300)
        [Test]
        public void PPCDis_4E474258()
        {
            AssertCode(0x4E474258, "@@@");
        }

        // Unknown PowerPC X instruction 4E474259 13-12C(300)
        [Test]
        public void PPCDis_4E474259()
        {
            AssertCode(0x4E474259, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425A 13-12D (301)
        [Test]
        public void PPCDis_4E47425A()
        {
            AssertCode(0x4E47425A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425B 13-12D (301)
        [Test]
        public void PPCDis_4E47425B()
        {
            AssertCode(0x4E47425B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425C 13-12E (302)
        [Test]
        public void PPCDis_4E47425C()
        {
            AssertCode(0x4E47425C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425D 13-12E (302)
        [Test]
        public void PPCDis_4E47425D()
        {
            AssertCode(0x4E47425D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425E 13-12F (303)
        [Test]
        public void PPCDis_4E47425E()
        {
            AssertCode(0x4E47425E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425F 13-12F (303)
        [Test]
        public void PPCDis_4E47425F()
        {
            AssertCode(0x4E47425F, "@@@");
        }

        // Unknown PowerPC X instruction 4E474260 13-130 (304)
        [Test]
        public void PPCDis_4E474260()
        {
            AssertCode(0x4E474260, "@@@");
        }

        // Unknown PowerPC X instruction 4E474261 13-130 (304)
        [Test]
        public void PPCDis_4E474261()
        {
            AssertCode(0x4E474261, "@@@");
        }

        // Unknown PowerPC X instruction 4E474262 13-131 (305)
        [Test]
        public void PPCDis_4E474262()
        {
            AssertCode(0x4E474262, "@@@");
        }

        // Unknown PowerPC X instruction 4E474263 13-131 (305)
        [Test]
        public void PPCDis_4E474263()
        {
            AssertCode(0x4E474263, "@@@");
        }

        // Unknown PowerPC X instruction 4E474430 13-218 (536)
        [Test]
        public void PPCDis_4E474430()
        {
            AssertCode(0x4E474430, "@@@");
        }

        // Unknown PowerPC X instruction 4E474431 13-218 (536)
        [Test]
        public void PPCDis_4E474431()
        {
            AssertCode(0x4E474431, "@@@");
        }

        // Unknown PowerPC X instruction 4E474542 13-2A1(673)
        [Test]
        public void PPCDis_4E474542()
        {
            AssertCode(0x4E474542, "@@@");
        }

        // Unknown PowerPC X instruction 4E474A30 13-118 (280)
        [Test]
        public void PPCDis_4E474A30()
        {
            AssertCode(0x4E474A30, "@@@");
        }

        // Unknown PowerPC X instruction 4E474A31 13-118 (280)
        [Test]
        public void PPCDis_4E474A31()
        {
            AssertCode(0x4E474A31, "@@@");
        }

        // Unknown PowerPC X instruction 4E474A32 13-119 (281)
        [Test]
        public void PPCDis_4E474A32()
        {
            AssertCode(0x4E474A32, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D30 13-298 (664)
        [Test]
        public void PPCDis_4E474D30()
        {
            AssertCode(0x4E474D30, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D31 13-298 (664)
        [Test]
        public void PPCDis_4E474D31()
        {
            AssertCode(0x4E474D31, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D32 13-299 (665)
        [Test]
        public void PPCDis_4E474D32()
        {
            AssertCode(0x4E474D32, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D33 13-299 (665)
        [Test]
        public void PPCDis_4E474D33()
        {
            AssertCode(0x4E474D33, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D34 13-29A(666)
        [Test]
        public void PPCDis_4E474D34()
        {
            AssertCode(0x4E474D34, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D35 13-29A(666)
        [Test]
        public void PPCDis_4E474D35()
        {
            AssertCode(0x4E474D35, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D36 13-29B(667)
        [Test]
        public void PPCDis_4E474D36()
        {
            AssertCode(0x4E474D36, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D37 13-29B(667)
        [Test]
        public void PPCDis_4E474D37()
        {
            AssertCode(0x4E474D37, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D38 13-29C(668)
        [Test]
        public void PPCDis_4E474D38()
        {
            AssertCode(0x4E474D38, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D39 13-29C(668)
        [Test]
        public void PPCDis_4E474D39()
        {
            AssertCode(0x4E474D39, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3A 13-29D (669)
        [Test]
        public void PPCDis_4E474D3A()
        {
            AssertCode(0x4E474D3A, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3B 13-29D (669)
        [Test]
        public void PPCDis_4E474D3B()
        {
            AssertCode(0x4E474D3B, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3C 13-29E (670)
        [Test]
        public void PPCDis_4E474D3C()
        {
            AssertCode(0x4E474D3C, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3D 13-29E (670)
        [Test]
        public void PPCDis_4E474D3D()
        {
            AssertCode(0x4E474D3D, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3E 13-29F (671)
        [Test]
        public void PPCDis_4E474D3E()
        {
            AssertCode(0x4E474D3E, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3F 13-29F (671)
        [Test]
        public void PPCDis_4E474D3F()
        {
            AssertCode(0x4E474D3F, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D40 13-2A0(672)
        [Test]
        public void PPCDis_4E474D40()
        {
            AssertCode(0x4E474D40, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D41 13-2A0(672)
        [Test]
        public void PPCDis_4E474D41()
        {
            AssertCode(0x4E474D41, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D42 13-2A1(673)
        [Test]
        public void PPCDis_4E474D42()
        {
            AssertCode(0x4E474D42, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D43 13-2A1(673)
        [Test]
        public void PPCDis_4E474D43()
        {
            AssertCode(0x4E474D43, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D44 13-2A2(674)
        [Test]
        public void PPCDis_4E474D44()
        {
            AssertCode(0x4E474D44, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D45 13-2A2(674)
        [Test]
        public void PPCDis_4E474D45()
        {
            AssertCode(0x4E474D45, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D46 13-2A3(675)
        [Test]
        public void PPCDis_4E474D46()
        {
            AssertCode(0x4E474D46, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D47 13-2A3(675)
        [Test]
        public void PPCDis_4E474D47()
        {
            AssertCode(0x4E474D47, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D48 13-2A4(676)
        [Test]
        public void PPCDis_4E474D48()
        {
            AssertCode(0x4E474D48, "@@@");
        }

        // Unknown PowerPC X instruction 4E474E30 13-318 (792)
        [Test]
        public void PPCDis_4E474E30()
        {
            AssertCode(0x4E474E30, "@@@");
        }

        // Unknown PowerPC X instruction 4E474E35 13-31A(794)
        [Test]
        public void PPCDis_4E474E35()
        {
            AssertCode(0x4E474E35, "@@@");
        }

        // Unknown PowerPC X instruction 4E474E36 13-31B(795)
        [Test]
        public void PPCDis_4E474E36()
        {
            AssertCode(0x4E474E36, "@@@");
        }

        // Unknown PowerPC X instruction 4E474E37 13-31B(795)
        [Test]
        public void PPCDis_4E474E37()
        {
            AssertCode(0x4E474E37, "@@@");
        }

        // Unknown PowerPC X instruction 4E475230 13-118 (280)
        [Test]
        public void PPCDis_4E475230()
        {
            AssertCode(0x4E475230, "@@@");
        }

        // Unknown PowerPC X instruction 4E475231 13-118 (280)
        [Test]
        public void PPCDis_4E475231()
        {
            AssertCode(0x4E475231, "@@@");
        }

        // Unknown PowerPC X instruction 4E475232 13-119 (281)
        [Test]
        public void PPCDis_4E475232()
        {
            AssertCode(0x4E475232, "@@@");
        }

        // Unknown PowerPC X instruction 4E475233 13-119 (281)
        [Test]
        public void PPCDis_4E475233()
        {
            AssertCode(0x4E475233, "@@@");
        }

        // Unknown PowerPC X instruction 4E475234 13-11A(282)
        [Test]
        public void PPCDis_4E475234()
        {
            AssertCode(0x4E475234, "@@@");
        }

        // Unknown PowerPC X instruction 4E475235 13-11A(282)
        [Test]
        public void PPCDis_4E475235()
        {
            AssertCode(0x4E475235, "@@@");
        }

        // Unknown PowerPC X instruction 4E475236 13-11B(283)
        [Test]
        public void PPCDis_4E475236()
        {
            AssertCode(0x4E475236, "@@@");
        }

        // Unknown PowerPC X instruction 4E475237 13-11B(283)
        [Test]
        public void PPCDis_4E475237()
        {
            AssertCode(0x4E475237, "@@@");
        }

        // Unknown PowerPC X instruction 4E475238 13-11C(284)
        [Test]
        public void PPCDis_4E475238()
        {
            AssertCode(0x4E475238, "@@@");
        }

        // Unknown PowerPC X instruction 4E475239 13-11C(284)
        [Test]
        public void PPCDis_4E475239()
        {
            AssertCode(0x4E475239, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523A 13-11D (285)
        [Test]
        public void PPCDis_4E47523A()
        {
            AssertCode(0x4E47523A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523B 13-11D (285)
        [Test]
        public void PPCDis_4E47523B()
        {
            AssertCode(0x4E47523B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523C 13-11E (286)
        [Test]
        public void PPCDis_4E47523C()
        {
            AssertCode(0x4E47523C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523D 13-11E (286)
        [Test]
        public void PPCDis_4E47523D()
        {
            AssertCode(0x4E47523D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523E 13-11F (287)
        [Test]
        public void PPCDis_4E47523E()
        {
            AssertCode(0x4E47523E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523F 13-11F (287)
        [Test]
        public void PPCDis_4E47523F()
        {
            AssertCode(0x4E47523F, "@@@");
        }

        // Unknown PowerPC X instruction 4E475240 13-120 (288)
        [Test]
        public void PPCDis_4E475240()
        {
            AssertCode(0x4E475240, "@@@");
        }

        // Unknown PowerPC X instruction 4E475241 13-120 (288)
        [Test]
        public void PPCDis_4E475241()
        {
            AssertCode(0x4E475241, "@@@");
        }

        // Unknown PowerPC X instruction 4E475244 13-122 (290)
        [Test]
        public void PPCDis_4E475244()
        {
            AssertCode(0x4E475244, "@@@");
        }

        // Unknown PowerPC X instruction 4E475245 13-122 (290)
        [Test]
        public void PPCDis_4E475245()
        {
            AssertCode(0x4E475245, "@@@");
        }

        // Unknown PowerPC X instruction 4E475246 13-123 (291)
        [Test]
        public void PPCDis_4E475246()
        {
            AssertCode(0x4E475246, "@@@");
        }

        // Unknown PowerPC X instruction 4E475247 13-123 (291)
        [Test]
        public void PPCDis_4E475247()
        {
            AssertCode(0x4E475247, "@@@");
        }

        // Unknown PowerPC X instruction 4E475248 13-124 (292)
        [Test]
        public void PPCDis_4E475248()
        {
            AssertCode(0x4E475248, "@@@");
        }

        // Unknown PowerPC X instruction 4E475249 13-124 (292)
        [Test]
        public void PPCDis_4E475249()
        {
            AssertCode(0x4E475249, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524A 13-125 (293)
        [Test]
        public void PPCDis_4E47524A()
        {
            AssertCode(0x4E47524A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524B 13-125 (293)
        [Test]
        public void PPCDis_4E47524B()
        {
            AssertCode(0x4E47524B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524C 13-126 (294)
        [Test]
        public void PPCDis_4E47524C()
        {
            AssertCode(0x4E47524C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524D 13-126 (294)
        [Test]
        public void PPCDis_4E47524D()
        {
            AssertCode(0x4E47524D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524E 13-127 (295)
        [Test]
        public void PPCDis_4E47524E()
        {
            AssertCode(0x4E47524E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524F 13-127 (295)
        [Test]
        public void PPCDis_4E47524F()
        {
            AssertCode(0x4E47524F, "@@@");
        }

        // Unknown PowerPC X instruction 4E475250 13-128 (296)
        [Test]
        public void PPCDis_4E475250()
        {
            AssertCode(0x4E475250, "@@@");
        }

        // Unknown PowerPC X instruction 4E475251 13-128 (296)
        [Test]
        public void PPCDis_4E475251()
        {
            AssertCode(0x4E475251, "@@@");
        }

        // Unknown PowerPC X instruction 4E475252 13-129 (297)
        [Test]
        public void PPCDis_4E475252()
        {
            AssertCode(0x4E475252, "@@@");
        }

        // Unknown PowerPC X instruction 4E475253 13-129 (297)
        [Test]
        public void PPCDis_4E475253()
        {
            AssertCode(0x4E475253, "@@@");
        }

        // Unknown PowerPC X instruction 4E475254 13-12A(298)
        [Test]
        public void PPCDis_4E475254()
        {
            AssertCode(0x4E475254, "@@@");
        }

        // Unknown PowerPC X instruction 4E475255 13-12A(298)
        [Test]
        public void PPCDis_4E475255()
        {
            AssertCode(0x4E475255, "@@@");
        }

        // Unknown PowerPC X instruction 4E475256 13-12B(299)
        [Test]
        public void PPCDis_4E475256()
        {
            AssertCode(0x4E475256, "@@@");
        }

        // Unknown PowerPC X instruction 4E475257 13-12B(299)
        [Test]
        public void PPCDis_4E475257()
        {
            AssertCode(0x4E475257, "@@@");
        }

        // Unknown PowerPC X instruction 4E475258 13-12C(300)
        [Test]
        public void PPCDis_4E475258()
        {
            AssertCode(0x4E475258, "@@@");
        }

        // Unknown PowerPC X instruction 4E475259 13-12C(300)
        [Test]
        public void PPCDis_4E475259()
        {
            AssertCode(0x4E475259, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525A 13-12D (301)
        [Test]
        public void PPCDis_4E47525A()
        {
            AssertCode(0x4E47525A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525B 13-12D (301)
        [Test]
        public void PPCDis_4E47525B()
        {
            AssertCode(0x4E47525B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525C 13-12E (302)
        [Test]
        public void PPCDis_4E47525C()
        {
            AssertCode(0x4E47525C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525D 13-12E (302)
        [Test]
        public void PPCDis_4E47525D()
        {
            AssertCode(0x4E47525D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525E 13-12F (303)
        [Test]
        public void PPCDis_4E47525E()
        {
            AssertCode(0x4E47525E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525F 13-12F (303)
        [Test]
        public void PPCDis_4E47525F()
        {
            AssertCode(0x4E47525F, "@@@");
        }

        // Unknown PowerPC X instruction 4E475260 13-130 (304)
        [Test]
        public void PPCDis_4E475260()
        {
            AssertCode(0x4E475260, "@@@");
        }

        // Unknown PowerPC X instruction 4E475261 13-130 (304)
        [Test]
        public void PPCDis_4E475261()
        {
            AssertCode(0x4E475261, "@@@");
        }

        // Unknown PowerPC X instruction 4E475262 13-131 (305)
        [Test]
        public void PPCDis_4E475262()
        {
            AssertCode(0x4E475262, "@@@");
        }

        // Unknown PowerPC X instruction 4E475263 13-131 (305)
        [Test]
        public void PPCDis_4E475263()
        {
            AssertCode(0x4E475263, "@@@");
        }

        // Unknown PowerPC X instruction 4E475264 13-132 (306)
        [Test]
        public void PPCDis_4E475264()
        {
            AssertCode(0x4E475264, "@@@");
        }

        // Unknown PowerPC X instruction 4E475265 13-132 (306)
        [Test]
        public void PPCDis_4E475265()
        {
            AssertCode(0x4E475265, "@@@");
        }

        // Unknown PowerPC X instruction 4E475266 13-133 (307)
        [Test]
        public void PPCDis_4E475266()
        {
            AssertCode(0x4E475266, "@@@");
        }

        // Unknown PowerPC X instruction 4E475267 13-133 (307)
        [Test]
        public void PPCDis_4E475267()
        {
            AssertCode(0x4E475267, "@@@");
        }

        // Unknown PowerPC X instruction 4E475268 13-134 (308)
        [Test]
        public void PPCDis_4E475268()
        {
            AssertCode(0x4E475268, "@@@");
        }

        // Unknown PowerPC X instruction 4E475269 13-134 (308)
        [Test]
        public void PPCDis_4E475269()
        {
            AssertCode(0x4E475269, "@@@");
        }

        // Unknown PowerPC X instruction 4E47526A 13-135 (309)
        [Test]
        public void PPCDis_4E47526A()
        {
            AssertCode(0x4E47526A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47526B 13-135 (309)
        [Test]
        public void PPCDis_4E47526B()
        {
            AssertCode(0x4E47526B, "@@@");
        }

        // Unknown PowerPC X instruction 4E475330 13-198 (408)
        [Test]
        public void PPCDis_4E475330()
        {
            AssertCode(0x4E475330, "@@@");
        }

        // Unknown PowerPC X instruction 4E475331 13-198 (408)
        [Test]
        public void PPCDis_4E475331()
        {
            AssertCode(0x4E475331, "@@@");
        }

        // Unknown PowerPC X instruction 4E475332 13-199 (409)
        [Test]
        public void PPCDis_4E475332()
        {
            AssertCode(0x4E475332, "@@@");
        }

        // Unknown PowerPC X instruction 4E475333 13-199 (409)
        [Test]
        public void PPCDis_4E475333()
        {
            AssertCode(0x4E475333, "@@@");
        }

        // Unknown PowerPC X instruction 4E475334 13-19A(410)
        [Test]
        public void PPCDis_4E475334()
        {
            AssertCode(0x4E475334, "@@@");
        }

        // Unknown PowerPC X instruction 4E475335 13-19A(410)
        [Test]
        public void PPCDis_4E475335()
        {
            AssertCode(0x4E475335, "@@@");
        }

        // Unknown PowerPC X instruction 4E475336 13-19B(411)
        [Test]
        public void PPCDis_4E475336()
        {
            AssertCode(0x4E475336, "@@@");
        }

        // Unknown PowerPC X instruction 4E475337 13-19B(411)
        [Test]
        public void PPCDis_4E475337()
        {
            AssertCode(0x4E475337, "@@@");
        }

        // Unknown PowerPC X instruction 4E475338 13-19C(412)
        [Test]
        public void PPCDis_4E475338()
        {
            AssertCode(0x4E475338, "@@@");
        }

        // Unknown PowerPC X instruction 4E475339 13-19C(412)
        [Test]
        public void PPCDis_4E475339()
        {
            AssertCode(0x4E475339, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533A 13-19D (413)
        [Test]
        public void PPCDis_4E47533A()
        {
            AssertCode(0x4E47533A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533B 13-19D (413)
        [Test]
        public void PPCDis_4E47533B()
        {
            AssertCode(0x4E47533B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533C 13-19E (414)
        [Test]
        public void PPCDis_4E47533C()
        {
            AssertCode(0x4E47533C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533D 13-19E (414)
        [Test]
        public void PPCDis_4E47533D()
        {
            AssertCode(0x4E47533D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533E 13-19F (415)
        [Test]
        public void PPCDis_4E47533E()
        {
            AssertCode(0x4E47533E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533F 13-19F (415)
        [Test]
        public void PPCDis_4E47533F()
        {
            AssertCode(0x4E47533F, "@@@");
        }

        // Unknown PowerPC X instruction 4E475340 13-1A0(416)
        [Test]
        public void PPCDis_4E475340()
        {
            AssertCode(0x4E475340, "@@@");
        }

        // Unknown PowerPC X instruction 4E475341 13-1A0(416)
        [Test]
        public void PPCDis_4E475341()
        {
            AssertCode(0x4E475341, "@@@");
        }

        // Unknown PowerPC X instruction 4E475342 13-1A1(417)
        [Test]
        public void PPCDis_4E475342()
        {
            AssertCode(0x4E475342, "@@@");
        }

        // Unknown PowerPC X instruction 4E475430 13-218 (536)
        [Test]
        public void PPCDis_4E475430()
        {
            AssertCode(0x4E475430, "@@@");
        }

        // Unknown PowerPC X instruction 4E475431 13-218 (536)
        [Test]
        public void PPCDis_4E475431()
        {
            AssertCode(0x4E475431, "@@@");
        }

        // Unknown PowerPC X instruction 4E475432 13-219 (537)
        [Test]
        public void PPCDis_4E475432()
        {
            AssertCode(0x4E475432, "@@@");
        }

        // Unknown PowerPC X instruction 4E475433 13-219 (537)
        [Test]
        public void PPCDis_4E475433()
        {
            AssertCode(0x4E475433, "@@@");
        }

        // Unknown PowerPC X instruction 4E475434 13-21A(538)
        [Test]
        public void PPCDis_4E475434()
        {
            AssertCode(0x4E475434, "@@@");
        }

        // Unknown PowerPC X instruction 4E475435 13-21A(538)
        [Test]
        public void PPCDis_4E475435()
        {
            AssertCode(0x4E475435, "@@@");
        }

        // Unknown PowerPC X instruction 4E475830 13-018 (24)
        [Test]
        public void PPCDis_4E475830()
        {
            AssertCode(0x4E475830, "@@@");
        }

        // Unknown PowerPC X instruction 4E475831 13-018 (24)
        [Test]
        public void PPCDis_4E475831()
        {
            AssertCode(0x4E475831, "@@@");
        }

        // Unknown PowerPC X instruction 4E475832 13-019 (25)
        [Test]
        public void PPCDis_4E475832()
        {
            AssertCode(0x4E475832, "@@@");
        }

        // Unknown PowerPC X instruction 4E475D2E 13-297 (663)
        [Test]
        public void PPCDis_4E475D2E()
        {
            AssertCode(0x4E475D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4E4B544F 13-227 (551)
        [Test]
        public void PPCDis_4E4B544F()
        {
            AssertCode(0x4E4B544F, "@@@");
        }

        // Unknown PowerPC X instruction 4E4B5F4A 13-3A5(933)
        [Test]
        public void PPCDis_4E4B5F4A()
        {
            AssertCode(0x4E4B5F4A, "@@@");
        }

        // Unknown PowerPC X instruction 4E4B5F50 13-3A8(936)
        [Test]
        public void PPCDis_4E4B5F50()
        {
            AssertCode(0x4E4B5F50, "@@@");
        }

        // Unknown PowerPC X instruction 4E4D4752 13-3A9(937)
        [Test]
        public void PPCDis_4E4D4752()
        {
            AssertCode(0x4E4D4752, "@@@");
        }

        // Unknown PowerPC X instruction 4E4D8266 13-133 (307)
        [Test]
        public void PPCDis_4E4D8266()
        {
            AssertCode(0x4E4D8266, "@@@");
        }

        // Unknown PowerPC X instruction 4E4F4E45 13-322 (802)
        [Test]
        public void PPCDis_4E4F4E45()
        {
            AssertCode(0x4E4F4E45, "@@@");
        }

        // Unknown PowerPC X instruction 4E504320 13-190 (400)
        [Test]
        public void PPCDis_4E504320()
        {
            AssertCode(0x4E504320, "@@@");
        }

        // Unknown PowerPC X instruction 4E504330 13-198 (408)
        [Test]
        public void PPCDis_4E504330()
        {
            AssertCode(0x4E504330, "@@@");
        }

        // Unknown PowerPC X instruction 4E504342 13-1A1(417)
        [Test]
        public void PPCDis_4E504342()
        {
            AssertCode(0x4E504342, "@@@");
        }

        // Unknown PowerPC X instruction 4E504344 13-1A2(418)
        [Test]
        public void PPCDis_4E504344()
        {
            AssertCode(0x4E504344, "@@@");
        }

        // Unknown PowerPC X instruction 4E504353 13-1A9(425)
        [Test]
        public void PPCDis_4E504353()
        {
            AssertCode(0x4E504353, "@@@");
        }

        // Unknown PowerPC X instruction 4E50435F 13-1AF(431)
        [Test]
        public void PPCDis_4E50435F()
        {
            AssertCode(0x4E50435F, "@@@");
        }

        // Unknown PowerPC X instruction 4E50437C 13-1BE(446)
        [Test]
        public void PPCDis_4E50437C()
        {
            AssertCode(0x4E50437C, "@@@");
        }

        // Unknown PowerPC X instruction 4E534F4F 13-3A7(935)
        [Test]
        public void PPCDis_4E534F4F()
        {
            AssertCode(0x4E534F4F, "@@@");
        }

        // Unknown PowerPC X instruction 4E544552 13-2A9(681)
        [Test]
        public void PPCDis_4E544552()
        {
            AssertCode(0x4E544552, "@@@");
        }

        // Unknown PowerPC X instruction 4E544630 13-318 (792)
        [Test]
        public void PPCDis_4E544630()
        {
            AssertCode(0x4E544630, "@@@");
        }

        // Unknown PowerPC X instruction 4E544631 13-318 (792)
        [Test]
        public void PPCDis_4E544631()
        {
            AssertCode(0x4E544631, "@@@");
        }

        // Unknown PowerPC X instruction 4E545200 13-100 (256)
        [Test]
        public void PPCDis_4E545200()
        {
            AssertCode(0x4E545200, "@@@");
        }

        // Unknown PowerPC X instruction 4E54524F 13-127 (295)
        [Test]
        public void PPCDis_4E54524F()
        {
            AssertCode(0x4E54524F, "@@@");
        }

        // Unknown PowerPC X instruction 4E545D2E 13-297 (663)
        [Test]
        public void PPCDis_4E545D2E()
        {
            AssertCode(0x4E545D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4E545F52 13-3A9(937)
        [Test]
        public void PPCDis_4E545F52()
        {
            AssertCode(0x4E545F52, "@@@");
        }

        // Unknown PowerPC X instruction 4E553320 13-190 (400)
        [Test]
        public void PPCDis_4E553320()
        {
            AssertCode(0x4E553320, "@@@");
        }

        // Unknown PowerPC X instruction 4E564559 13-2AC(684)
        [Test]
        public void PPCDis_4E564559()
        {
            AssertCode(0x4E564559, "@@@");
        }

        // Unknown PowerPC X instruction 4E5AED51 13-2A8(680)
        [Test]
        public void PPCDis_4E5AED51()
        {
            AssertCode(0x4E5AED51, "@@@");
        }

        // Unknown PowerPC X instruction 4E5F4249 13-124 (292)
        [Test]
        public void PPCDis_4E5F4249()
        {
            AssertCode(0x4E5F4249, "@@@");
        }

        // Unknown PowerPC X instruction 4E5F504C 13-026 (38)
        [Test]
        public void PPCDis_4E5F504C()
        {
            AssertCode(0x4E5F504C, "@@@");
        }

        // Unknown PowerPC X instruction 4E6E6B28 13-194 (404)
        [Test]
        public void PPCDis_4E6E6B28()
        {
            AssertCode(0x4E6E6B28, "@@@");
        }

        // Unknown PowerPC X instruction 4E6F2046 13-023 (35)
        [Test]
        public void PPCDis_4E6F2046()
        {
            AssertCode(0x4E6F2046, "@@@");
        }

        // Unknown PowerPC X instruction 4E6F206F 13-037 (55)
        [Test]
        public void PPCDis_4E6F206F()
        {
            AssertCode(0x4E6F206F, "@@@");
        }

        // Unknown PowerPC X instruction 4E6F6E2D 13-316 (790)
        [Test]
        public void PPCDis_4E6F6E2D()
        {
            AssertCode(0x4E6F6E2D, "@@@");
        }

        // Unknown PowerPC X instruction 4E6F6E52 13-329 (809)
        [Test]
        public void PPCDis_4E6F6E52()
        {
            AssertCode(0x4E6F6E52, "@@@");
        }

        // Unknown PowerPC X instruction 4E6F726D 13-136 (310)
        [Test]
        public void PPCDis_4E6F726D()
        {
            AssertCode(0x4E6F726D, "@@@");
        }

        // Unknown PowerPC X instruction 4E7610E8 13-074 (116)
        [Test]
        public void PPCDis_4E7610E8()
        {
            AssertCode(0x4E7610E8, "@@@");
        }

        // Unknown PowerPC X instruction 4E7DAA4F 13-127 (295)
        [Test]
        public void PPCDis_4E7DAA4F()
        {
            AssertCode(0x4E7DAA4F, "@@@");
        }

        // Unknown PowerPC X instruction 4E96FD28 13-294 (660)
        [Test]
        public void PPCDis_4E96FD28()
        {
            AssertCode(0x4E96FD28, "@@@");
        }

        // Unknown PowerPC X instruction 4E991995 13-0CA(202)
        [Test]
        public void PPCDis_4E991995()
        {
            AssertCode(0x4E991995, "@@@");
        }

        // Unknown PowerPC X instruction 4EAC546D 13-236 (566)
        [Test]
        public void PPCDis_4EAC546D()
        {
            AssertCode(0x4EAC546D, "@@@");
        }

        // Unknown PowerPC X instruction 4EB6C4D1 13-268 (616)
        [Test]
        public void PPCDis_4EB6C4D1()
        {
            AssertCode(0x4EB6C4D1, "@@@");
        }

        // Unknown PowerPC X instruction 4EC08723 13-391 (913)
        [Test]
        public void PPCDis_4EC08723()
        {
            AssertCode(0x4EC08723, "@@@");
        }

        // Unknown PowerPC X instruction 4EC0B831 13-018 (24)
        [Test]
        public void PPCDis_4EC0B831()
        {
            AssertCode(0x4EC0B831, "@@@");
        }

        // Unknown PowerPC X instruction 4ED09E2D 13-316 (790)
        [Test]
        public void PPCDis_4ED09E2D()
        {
            AssertCode(0x4ED09E2D, "@@@");
        }

        // Unknown PowerPC X instruction 4ED66055 13-02A(42)
        [Test]
        public void PPCDis_4ED66055()
        {
            AssertCode(0x4ED66055, "@@@");
        }

        // Unknown PowerPC X instruction 4EEC4FD6 13-3EB(1003)
        [Test]
        public void PPCDis_4EEC4FD6()
        {
            AssertCode(0x4EEC4FD6, "@@@");
        }

        // Unknown PowerPC X instruction 4F0F9915 13-08A(138)
        [Test]
        public void PPCDis_4F0F9915()
        {
            AssertCode(0x4F0F9915, "@@@");
        }

        // Unknown PowerPC X instruction 4F202875 13-03A(58)
        [Test]
        public void PPCDis_4F202875()
        {
            AssertCode(0x4F202875, "@@@");
        }

        // Unknown PowerPC X instruction 4F23937F 13-1BF(447)
        [Test]
        public void PPCDis_4F23937F()
        {
            AssertCode(0x4F23937F, "@@@");
        }

        // Unknown PowerPC X instruction 4F338754 13-3AA(938)
        [Test]
        public void PPCDis_4F338754()
        {
            AssertCode(0x4F338754, "@@@");
        }

        // Unknown PowerPC X instruction 4F423A57 13-12B(299)
        [Test]
        public void PPCDis_4F423A57()
        {
            AssertCode(0x4F423A57, "@@@");
        }

        // Unknown PowerPC X instruction 4F424F54 13-3AA(938)
        [Test]
        public void PPCDis_4F424F54()
        {
            AssertCode(0x4F424F54, "@@@");
        }

        // Unknown PowerPC X instruction 4F42534F 13-1A7(423)
        [Test]
        public void PPCDis_4F42534F()
        {
            AssertCode(0x4F42534F, "@@@");
        }

        // Unknown PowerPC X instruction 4F434B00 13-180 (384)
        [Test]
        public void PPCDis_4F434B00()
        {
            AssertCode(0x4F434B00, "@@@");
        }

        // Unknown PowerPC X instruction 4F442063 13-031 (49)
        [Test]
        public void PPCDis_4F442063()
        {
            AssertCode(0x4F442063, "@@@");
        }

        // Unknown PowerPC X instruction 4F442074 13-03A(58)
        [Test]
        public void PPCDis_4F442074()
        {
            AssertCode(0x4F442074, "@@@");
        }

        // Unknown PowerPC X instruction 4F44425A 13-12D (301)
        [Test]
        public void PPCDis_4F44425A()
        {
            AssertCode(0x4F44425A, "@@@");
        }

        // Unknown PowerPC X instruction 4F444445 13-222 (546)
        [Test]
        public void PPCDis_4F444445()
        {
            AssertCode(0x4F444445, "@@@");
        }

        // Unknown PowerPC X instruction 4F4B455F 13-2AF(687)
        [Test]
        public void PPCDis_4F4B455F()
        {
            AssertCode(0x4F4B455F, "@@@");
        }

        // Unknown PowerPC X instruction 4F4C4445 13-222 (546)
        [Test]
        public void PPCDis_4F4C4445()
        {
            AssertCode(0x4F4C4445, "@@@");
        }

        // Unknown PowerPC X instruction 4F4C4554 13-2AA(682)
        [Test]
        public void PPCDis_4F4C4554()
        {
            AssertCode(0x4F4C4554, "@@@");
        }

        // Unknown PowerPC X instruction 4F4C4C45 13-222 (546)
        [Test]
        public void PPCDis_4F4C4C45()
        {
            AssertCode(0x4F4C4C45, "@@@");
        }

        // Unknown PowerPC X instruction 4F4C545F 13-22F (559)
        [Test]
        public void PPCDis_4F4C545F()
        {
            AssertCode(0x4F4C545F, "@@@");
        }

        // Unknown PowerPC X instruction 4F4D4F4E 13-3A7(935)
        [Test]
        public void PPCDis_4F4D4F4E()
        {
            AssertCode(0x4F4D4F4E, "@@@");
        }

        // Unknown PowerPC X instruction 4F4D5045 13-022 (34)
        [Test]
        public void PPCDis_4F4D5045()
        {
            AssertCode(0x4F4D5045, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E004E 13-027 (39)
        [Test]
        public void PPCDis_4F4E004E()
        {
            AssertCode(0x4F4E004E, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E0053 13-029 (41)
        [Test]
        public void PPCDis_4F4E0053()
        {
            AssertCode(0x4F4E0053, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E2033 13-019 (25)
        [Test]
        public void PPCDis_4F4E2033()
        {
            AssertCode(0x4F4E2033, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E2034 13-01A(26)
        [Test]
        public void PPCDis_4F4E2034()
        {
            AssertCode(0x4F4E2034, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E2055 13-02A(42)
        [Test]
        public void PPCDis_4F4E2055()
        {
            AssertCode(0x4F4E2055, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E4520 13-290 (656)
        [Test]
        public void PPCDis_4F4E4520()
        {
            AssertCode(0x4F4E4520, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E4747 13-3A3(931)
        [Test]
        public void PPCDis_4F4E4747()
        {
            AssertCode(0x4F4E4747, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E5645 13-322 (802)
        [Test]
        public void PPCDis_4F4E5645()
        {
            AssertCode(0x4F4E5645, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E5F5A 13-3AD(941)
        [Test]
        public void PPCDis_4F4E5F5A()
        {
            AssertCode(0x4F4E5F5A, "@@@");
        }

        // Unknown PowerPC X instruction 4F4F4400 13-200 (512)
        [Test]
        public void PPCDis_4F4F4400()
        {
            AssertCode(0x4F4F4400, "@@@");
        }

        // Unknown PowerPC X instruction 4F4F4445 13-222 (546)
        [Test]
        public void PPCDis_4F4F4445()
        {
            AssertCode(0x4F4F4445, "@@@");
        }

        // Unknown PowerPC X instruction 4F4F4B20 13-190 (400)
        [Test]
        public void PPCDis_4F4F4B20()
        {
            AssertCode(0x4F4F4B20, "@@@");
        }

        // Unknown PowerPC X instruction 4F4F5400 13-200 (512)
        [Test]
        public void PPCDis_4F4F5400()
        {
            AssertCode(0x4F4F5400, "@@@");
        }

        // Unknown PowerPC X instruction 4F505F53 13-3A9(937)
        [Test]
        public void PPCDis_4F505F53()
        {
            AssertCode(0x4F505F53, "@@@");
        }

        // Unknown PowerPC X instruction 4F505F57 13-3AB(939)
        [Test]
        public void PPCDis_4F505F57()
        {
            AssertCode(0x4F505F57, "@@@");
        }

        // Unknown PowerPC X instruction 4F524500 13-280 (640)
        [Test]
        public void PPCDis_4F524500()
        {
            AssertCode(0x4F524500, "@@@");
        }

        // Unknown PowerPC X instruction 4F52455F 13-2AF(687)
        [Test]
        public void PPCDis_4F52455F()
        {
            AssertCode(0x4F52455F, "@@@");
        }

        // Unknown PowerPC X instruction 4F524B53 13-1A9(425)
        [Test]
        public void PPCDis_4F524B53()
        {
            AssertCode(0x4F524B53, "@@@");
        }

        // Unknown PowerPC X instruction 4F524D5F 13-2AF(687)
        [Test]
        public void PPCDis_4F524D5F()
        {
            AssertCode(0x4F524D5F, "@@@");
        }

        // Unknown PowerPC X instruction 4F525441 13-220 (544)
        [Test]
        public void PPCDis_4F525441()
        {
            AssertCode(0x4F525441, "@@@");
        }

        // Unknown PowerPC X instruction 4F535320 13-190 (400)
        [Test]
        public void PPCDis_4F535320()
        {
            AssertCode(0x4F535320, "@@@");
        }

        // Unknown PowerPC X instruction 4F535350 13-1A8(424)
        [Test]
        public void PPCDis_4F535350()
        {
            AssertCode(0x4F535350, "@@@");
        }

        // Unknown PowerPC X instruction 4F542055 13-02A(42)
        [Test]
        public void PPCDis_4F542055()
        {
            AssertCode(0x4F542055, "@@@");
        }

        // Unknown PowerPC X instruction 4F554400 13-200 (512)
        [Test]
        public void PPCDis_4F554400()
        {
            AssertCode(0x4F554400, "@@@");
        }

        // Unknown PowerPC X instruction 4F554E44 13-322 (802)
        [Test]
        public void PPCDis_4F554E44()
        {
            AssertCode(0x4F554E44, "@@@");
        }

        // Unknown PowerPC X instruction 4F57127D 13-13E (318)
        [Test]
        public void PPCDis_4F57127D()
        {
            AssertCode(0x4F57127D, "@@@");
        }

        // Unknown PowerPC X instruction 4F572055 13-02A(42)
        [Test]
        public void PPCDis_4F572055()
        {
            AssertCode(0x4F572055, "@@@");
        }

        // Unknown PowerPC X instruction 4F592D8C 13-2C6(710)
        [Test]
        public void PPCDis_4F592D8C()
        {
            AssertCode(0x4F592D8C, "@@@");
        }

        // Unknown PowerPC X instruction 4F626A65 13-132 (306)
        [Test]
        public void PPCDis_4F626A65()
        {
            AssertCode(0x4F626A65, "@@@");
        }

        // Unknown PowerPC X instruction 4F66426F 13-137 (311)
        [Test]
        public void PPCDis_4F66426F()
        {
            AssertCode(0x4F66426F, "@@@");
        }

        // Unknown PowerPC X instruction 4F66576F 13-3B7(951)
        [Test]
        public void PPCDis_4F66576F()
        {
            AssertCode(0x4F66576F, "@@@");
        }

        // Unknown PowerPC X instruction 4F666600 13-300 (768)
        [Test]
        public void PPCDis_4F666600()
        {
            AssertCode(0x4F666600, "@@@");
        }

        // Unknown PowerPC X instruction 4F666673 13-339 (825)
        [Test]
        public void PPCDis_4F666673()
        {
            AssertCode(0x4F666673, "@@@");
        }

        // Unknown PowerPC X instruction 4F6E0053 13-029 (41)
        [Test]
        public void PPCDis_4F6E0053()
        {
            AssertCode(0x4F6E0053, "@@@");
        }

        // Unknown PowerPC X instruction 4F6E2053 13-029 (41)
        [Test]
        public void PPCDis_4F6E2053()
        {
            AssertCode(0x4F6E2053, "@@@");
        }

        // Unknown PowerPC X instruction 4F707469 13-234 (564)
        [Test]
        public void PPCDis_4F707469()
        {
            AssertCode(0x4F707469, "@@@");
        }

        // Unknown PowerPC X instruction 4F756368 13-1B4(436)
        [Test]
        public void PPCDis_4F756368()
        {
            AssertCode(0x4F756368, "@@@");
        }

        // Unknown PowerPC X instruction 4F757400 13-200 (512)
        [Test]
        public void PPCDis_4F757400()
        {
            AssertCode(0x4F757400, "@@@");
        }

        // Unknown PowerPC X instruction 4F75744F 13-227 (551)
        [Test]
        public void PPCDis_4F75744F()
        {
            AssertCode(0x4F75744F, "@@@");
        }

        // Unknown PowerPC X instruction 4F87DC71 13-238 (568)
        [Test]
        public void PPCDis_4F87DC71()
        {
            AssertCode(0x4F87DC71, "@@@");
        }

        // Unknown PowerPC X instruction 4F8B6900 13-080 (128)
        [Test]
        public void PPCDis_4F8B6900()
        {
            AssertCode(0x4F8B6900, "@@@");
        }

        // Unknown PowerPC X instruction 4F9C1BD2 13-1E9 (489)
        [Test]
        public void PPCDis_4F9C1BD2()
        {
            AssertCode(0x4F9C1BD2, "@@@");
        }

        // Unknown PowerPC X instruction 4FB921D9 13-0EC(236)
        [Test]
        public void PPCDis_4FB921D9()
        {
            AssertCode(0x4FB921D9, "@@@");
        }

        // Unknown PowerPC X instruction 4FE3D097 13-04B(75)
        [Test]
        public void PPCDis_4FE3D097()
        {
            AssertCode(0x4FE3D097, "@@@");
        }

        // Unknown PowerPC X instruction 4FF594AA 13-255 (597)
        [Test]
        public void PPCDis_4FF594AA()
        {
            AssertCode(0x4FF594AA, "@@@");
        }

        // Unknown PowerPC X instruction 4FF6EFEB 13-3F5 (1013)
        [Test]
        public void PPCDis_4FF6EFEB()
        {
            AssertCode(0x4FF6EFEB, "@@@");
        }

        // Unknown PowerPC X instruction 7C004A00 1F-100 (256)
        [Test]
        public void PPCDis_7C004A00()
        {
            AssertCode(0x7C004A00, "@@@");
        }

        // Unknown PowerPC X instruction 7C004A2A 1F-115 (277)
        [Test]
        public void PPCDis_7C004A2A()
        {
            AssertCode(0x7C004A2A, "@@@");
        }

        // Unknown PowerPC X instruction 7C02014D 1F-0A6(166)
        [Test]
        public void PPCDis_7C02014D()
        {
            AssertCode(0x7C02014D, "@@@");
        }

        // Unknown PowerPC X instruction 7C025374 1F-1BA(442)
        [Test]
        public void PPCDis_7C025374()
        {
            AssertCode(0x7C025374, "@@@");
        }

        // Unknown PowerPC X instruction 7C0B7C15 1F-20A(522)
        [Test]
        public void PPCDis_7C0B7C15()
        {
            AssertCode(0x7C0B7C15, "@@@");
        }

        // Unknown PowerPC X instruction 7C0FEF59 1F-3AC(940)
        [Test]
        public void PPCDis_7C0FEF59()
        {
            AssertCode(0x7C0FEF59, "@@@");
        }

        // Unknown PowerPC X instruction 7C1E7C27 1F-213 (531)
        [Test]
        public void PPCDis_7C1E7C27()
        {
            AssertCode(0x7C1E7C27, "@@@");
        }

        // Unknown PowerPC X instruction 7C257300 1F-180 (384)
        [Test]
        public void PPCDis_7C257300()
        {
            AssertCode(0x7C257300, "@@@");
        }

        // Unknown PowerPC X instruction 7C307C39 1F-21C(540)
        [Test]
        public void PPCDis_7C307C39()
        {
            AssertCode(0x7C307C39, "@@@");
        }

        // Unknown PowerPC X instruction 7C3B01BA 1F-0DD(221)
        [Test]
        public void PPCDis_7C3B01BA()
        {
            AssertCode(0x7C3B01BA, "@@@");
        }

        // Unknown PowerPC X instruction 7C3DD424 1F-212 (530)
        [Test]
        public void PPCDis_7C3DD424()
        {
            AssertCode(0x7C3DD424, "@@@");
        }

        // Unknown PowerPC X instruction 7C426F73 1F-3B9(953)
        [Test]
        public void PPCDis_7C426F73()
        {
            AssertCode(0x7C426F73, "@@@");
        }

        // Unknown PowerPC X instruction 7C427C4B 1F-225 (549)
        [Test]
        public void PPCDis_7C427C4B()
        {
            AssertCode(0x7C427C4B, "@@@");
        }

        // Unknown PowerPC X instruction 7C43616D 1F-0B6 (182)
        [Test]
        public void PPCDis_7C43616D()
        {
            AssertCode(0x7C43616D, "@@@");
        }

        // Unknown PowerPC X instruction 7C486F6F 1F-3B7(951)
        [Test]
        public void PPCDis_7C486F6F()
        {
            AssertCode(0x7C486F6F, "@@@");
        }

        // Unknown PowerPC X instruction 7C486F72 1F-3B9(953)
        [Test]
        public void PPCDis_7C486F72()
        {
            AssertCode(0x7C486F72, "@@@");
        }

        // Unknown PowerPC X instruction 7C546170 1F-0B8 (184)
        [Test]
        public void PPCDis_7C546170()
        {
            AssertCode(0x7C546170, "@@@");
        }

        // Unknown PowerPC X instruction 7C546869 1F-034 (52)
        [Test]
        public void PPCDis_7C546869()
        {
            AssertCode(0x7C546869, "@@@");
        }

        // Unknown PowerPC X instruction 7C547572 1F-2B9(697)
        [Test]
        public void PPCDis_7C547572()
        {
            AssertCode(0x7C547572, "@@@");
        }

        // Unknown PowerPC X instruction 7C557C5E 1F-22F (559)
        [Test]
        public void PPCDis_7C557C5E()
        {
            AssertCode(0x7C557C5E, "@@@");
        }

        // Unknown PowerPC X instruction 7C566572 1F-2B9(697)
        [Test]
        public void PPCDis_7C566572()
        {
            AssertCode(0x7C566572, "@@@");
        }

        // Unknown PowerPC X instruction 7C677C70 1F-238 (568)
        [Test]
        public void PPCDis_7C677C70()
        {
            AssertCode(0x7C677C70, "@@@");
        }

        // Unknown PowerPC X instruction 7C6F0890 1F-048 (72)
        [Test]
        public void PPCDis_7C6F0890()
        {
            AssertCode(0x7C6F0890, "@@@");
        }

        // Unknown PowerPC X instruction 7C706F73 1F-3B9(953)
        [Test]
        public void PPCDis_7C706F73()
        {
            AssertCode(0x7C706F73, "@@@");
        }

        // Unknown PowerPC X instruction 7C766172 1F-0B9 (185)
        [Test]
        public void PPCDis_7C766172()
        {
            AssertCode(0x7C766172, "@@@");
        }

        // Unknown PowerPC X instruction 7C78004C 1F-026 (38)
        [Test]
        public void PPCDis_7C78004C()
        {
            AssertCode(0x7C78004C, "@@@");
        }

        // Unknown PowerPC X instruction 7C79004C 1F-026 (38)
        [Test]
        public void PPCDis_7C79004C()
        {
            AssertCode(0x7C79004C, "@@@");
        }

        // Unknown PowerPC X instruction 7C797C82 1F-241 (577)
        [Test]
        public void PPCDis_7C797C82()
        {
            AssertCode(0x7C797C82, "@@@");
        }

        // Unknown PowerPC X instruction 7C7A4E50 1F-328 (808)
        [Test]
        public void PPCDis_7C7A4E50()
        {
            AssertCode(0x7C7A4E50, "@@@");
        }

        // Unknown PowerPC X instruction 7C7D7E7F 1F-33F (831)
        [Test]
        public void PPCDis_7C7D7E7F()
        {
            AssertCode(0x7C7D7E7F, "@@@");
        }

        // Unknown PowerPC X instruction 7C8274C1 1F-260 (608)
        [Test]
        public void PPCDis_7C8274C1()
        {
            AssertCode(0x7C8274C1, "@@@");
        }

        // Unknown PowerPC X instruction 7C8C7C95 1F-24A(586)
        [Test]
        public void PPCDis_7C8C7C95()
        {
            AssertCode(0x7C8C7C95, "@@@");
        }

        // Unknown PowerPC X instruction 7C8CD8A2 1F-051 (81)
        [Test]
        public void PPCDis_7C8CD8A2()
        {
            AssertCode(0x7C8CD8A2, "@@@");
        }

        // Unknown PowerPC X instruction 7C996F00 1F-380 (896)
        [Test]
        public void PPCDis_7C996F00()
        {
            AssertCode(0x7C996F00, "@@@");
        }

        // Unknown PowerPC X instruction 7C9E7CA7 1F-253 (595)
        [Test]
        public void PPCDis_7C9E7CA7()
        {
            AssertCode(0x7C9E7CA7, "@@@");
        }

        // Unknown PowerPC X instruction 7C9F4AD1 1F-168 (360)
        [Test]
        public void PPCDis_7C9F4AD1()
        {
            AssertCode(0x7C9F4AD1, "@@@");
        }

        // Unknown PowerPC X instruction 7CB07CBA 1F-25D (605)
        [Test]
        public void PPCDis_7CB07CBA()
        {
            AssertCode(0x7CB07CBA, "@@@");
        }

        // Unknown PowerPC X instruction 7CC37CCC 1F-266 (614)
        [Test]
        public void PPCDis_7CC37CCC()
        {
            AssertCode(0x7CC37CCC, "@@@");
        }

        // Unknown PowerPC X instruction 7CD57CDE 1F-26F (623)
        [Test]
        public void PPCDis_7CD57CDE()
        {
            AssertCode(0x7CD57CDE, "@@@");
        }

        // Unknown PowerPC X instruction 7CD846DF 1F-36F (879)
        [Test]
        public void PPCDis_7CD846DF()
        {
            AssertCode(0x7CD846DF, "@@@");
        }

        // Unknown PowerPC X instruction 7CDF69CD 1F-0E6 (230)
        [Test]
        public void PPCDis_7CDF69CD()
        {
            AssertCode(0x7CDF69CD, "@@@");
        }

        // Unknown PowerPC X instruction 7CE87CF1 1F-278 (632)
        [Test]
        public void PPCDis_7CE87CF1()
        {
            AssertCode(0x7CE87CF1, "@@@");
        }

        // Unknown PowerPC X instruction 7CF8A16A 1F-0B5 (181)
        [Test]
        public void PPCDis_7CF8A16A()
        {
            AssertCode(0x7CF8A16A, "@@@");
        }

        // Unknown PowerPC X instruction 7CF97AF3 1F-179 (377)
        [Test]
        public void PPCDis_7CF97AF3()
        {
            AssertCode(0x7CF97AF3, "@@@");
        }

        // Unknown PowerPC X instruction 7CFA7D03 1F-281 (641)
        [Test]
        public void PPCDis_7CFA7D03()
        {
            AssertCode(0x7CFA7D03, "@@@");
        }

        // Unknown PowerPC X instruction 7CFCAA55 1F-12A(298)
        [Test]
        public void PPCDis_7CFCAA55()
        {
            AssertCode(0x7CFCAA55, "@@@");
        }

        // Unknown PowerPC X instruction 7D001CDD 1F-26E (622)
        [Test]
        public void PPCDis_7D001CDD()
        {
            AssertCode(0x7D001CDD, "@@@");
        }

        // Unknown PowerPC X instruction 7D007B69 1F-1B4(436)
        [Test]
        public void PPCDis_7D007B69()
        {
            AssertCode(0x7D007B69, "@@@");
        }

        // Unknown PowerPC X instruction 7D0C7D16 1F-28B(651)
        [Test]
        public void PPCDis_7D0C7D16()
        {
            AssertCode(0x7D0C7D16, "@@@");
        }

        // Unknown PowerPC X instruction 7D167E88 1F-344 (836)
        [Test]
        public void PPCDis_7D167E88()
        {
            AssertCode(0x7D167E88, "@@@");
        }

        // Unknown PowerPC X instruction 7D1DEFFD 1F-3FE(1022)
        [Test]
        public void PPCDis_7D1DEFFD()
        {
            AssertCode(0x7D1DEFFD, "@@@");
        }

        // Unknown PowerPC X instruction 7D1E6187 1F-0C3(195)
        [Test]
        public void PPCDis_7D1E6187()
        {
            AssertCode(0x7D1E6187, "@@@");
        }

        // Unknown PowerPC X instruction 7D1F7D28 1F-294 (660)
        [Test]
        public void PPCDis_7D1F7D28()
        {
            AssertCode(0x7D1F7D28, "@@@");
        }

        // Unknown PowerPC X instruction 7D218D40 1F-2A0(672)
        [Test]
        public void PPCDis_7D218D40()
        {
            AssertCode(0x7D218D40, "@@@");
        }

        // Unknown PowerPC X instruction 7D22DB16 1F-18B(395)
        [Test]
        public void PPCDis_7D22DB16()
        {
            AssertCode(0x7D22DB16, "@@@");
        }

        // Unknown PowerPC X instruction 7D25737B 1F-1BD(445)
        [Test]
        public void PPCDis_7D25737B()
        {
            AssertCode(0x7D25737B, "@@@");
        }

        // Unknown PowerPC X instruction 7D291B5F 1F-1AF(431)
        [Test]
        public void PPCDis_7D291B5F()
        {
            AssertCode(0x7D291B5F, "@@@");
        }

        // Unknown PowerPC X instruction 7D2E9B3D 1F-19E (414)
        [Test]
        public void PPCDis_7D2E9B3D()
        {
            AssertCode(0x7D2E9B3D, "@@@");
        }

        // Unknown PowerPC X instruction 7D314E3B 1F-31D (797)
        [Test]
        public void PPCDis_7D314E3B()
        {
            AssertCode(0x7D314E3B, "@@@");
        }

        // Unknown PowerPC X instruction 7D317D3A 1F-29D (669)
        [Test]
        public void PPCDis_7D317D3A()
        {
            AssertCode(0x7D317D3A, "@@@");
        }

        // Unknown PowerPC X instruction 7D3C75FD 1F-2FE(766)
        [Test]
        public void PPCDis_7D3C75FD()
        {
            AssertCode(0x7D3C75FD, "@@@");
        }

        // Unknown PowerPC X instruction 7D447D4D 1F-2A6(678)
        [Test]
        public void PPCDis_7D447D4D()
        {
            AssertCode(0x7D447D4D, "@@@");
        }

        // Unknown PowerPC X instruction 7D4638BE 1F-05F (95)
        [Test]
        public void PPCDis_7D4638BE()
        {
            AssertCode(0x7D4638BE, "@@@");
        }

        // Unknown PowerPC X instruction 7D55756C 1F-2B6(694)
        [Test]
        public void PPCDis_7D55756C()
        {
            AssertCode(0x7D55756C, "@@@");
        }

        // Unknown PowerPC X instruction 7D567D5F 1F-2AF(687)
        [Test]
        public void PPCDis_7D567D5F()
        {
            AssertCode(0x7D567D5F, "@@@");
        }

        // Unknown PowerPC X instruction 7D57ACA2 1F-251 (593)
        [Test]
        public void PPCDis_7D57ACA2()
        {
            AssertCode(0x7D57ACA2, "@@@");
        }

        // Unknown PowerPC X instruction 7D697D72 1F-2B9(697)
        [Test]
        public void PPCDis_7D697D72()
        {
            AssertCode(0x7D697D72, "@@@");
        }

        // Unknown PowerPC X instruction 7D78EBDD 1F-1EE(494)
        [Test]
        public void PPCDis_7D78EBDD()
        {
            AssertCode(0x7D78EBDD, "@@@");
        }

        // Unknown PowerPC X instruction 7D7B7D84 1F-2C2(706)
        [Test]
        public void PPCDis_7D7B7D84()
        {
            AssertCode(0x7D7B7D84, "@@@");
        }

        // Unknown PowerPC X instruction 7D8E7D97 1F-2CB(715)
        [Test]
        public void PPCDis_7D8E7D97()
        {
            AssertCode(0x7D8E7D97, "@@@");
        }

        // Unknown PowerPC X instruction 7DA07DA9 1F-2D4 (724)
        [Test]
        public void PPCDis_7DA07DA9()
        {
            AssertCode(0x7DA07DA9, "@@@");
        }

        // Unknown PowerPC X instruction 7DAC1CEB 1F-275 (629)
        [Test]
        public void PPCDis_7DAC1CEB()
        {
            AssertCode(0x7DAC1CEB, "@@@");
        }

        // Unknown PowerPC X instruction 7DADA77F 1F-3BF(959)
        [Test]
        public void PPCDis_7DADA77F()
        {
            AssertCode(0x7DADA77F, "@@@");
        }

        // Unknown PowerPC X instruction 7DAEBAF8 1F-17C(380)
        [Test]
        public void PPCDis_7DAEBAF8()
        {
            AssertCode(0x7DAEBAF8, "@@@");
        }

        // Unknown PowerPC X instruction 7DB37DBC 1F-2DE(734)
        [Test]
        public void PPCDis_7DB37DBC()
        {
            AssertCode(0x7DB37DBC, "@@@");
        }

        // Unknown PowerPC X instruction 7DB4A45E 1F-22F (559)
        [Test]
        public void PPCDis_7DB4A45E()
        {
            AssertCode(0x7DB4A45E, "@@@");
        }

        // Unknown PowerPC X instruction 7DBBFFFC 1F-3FE(1022)
        [Test]
        public void PPCDis_7DBBFFFC()
        {
            AssertCode(0x7DBBFFFC, "@@@");
        }

        // Unknown PowerPC X instruction 7DC57DCE 1F-2E7 (743)
        [Test]
        public void PPCDis_7DC57DCE()
        {
            AssertCode(0x7DC57DCE, "@@@");
        }

        // Unknown PowerPC X instruction 7DCBDC46 1F-223 (547)
        [Test]
        public void PPCDis_7DCBDC46()
        {
            AssertCode(0x7DCBDC46, "@@@");
        }

        // Unknown PowerPC X instruction 7DD7F79F 1F-3CF(975)
        [Test]
        public void PPCDis_7DD7F79F()
        {
            AssertCode(0x7DD7F79F, "@@@");
        }

        // Unknown PowerPC X instruction 7DD87DE1 1F-2F0 (752)
        [Test]
        public void PPCDis_7DD87DE1()
        {
            AssertCode(0x7DD87DE1, "@@@");
        }

        // Unknown PowerPC X instruction 7DD92685 1F-342 (834)
        [Test]
        public void PPCDis_7DD92685()
        {
            AssertCode(0x7DD92685, "@@@");
        }

        // Unknown PowerPC X instruction 7DDBA387 1F-1C3(451)
        [Test]
        public void PPCDis_7DDBA387()
        {
            AssertCode(0x7DDBA387, "@@@");
        }

        // Unknown PowerPC X instruction 7DDC6E1F 1F-30F (783)
        [Test]
        public void PPCDis_7DDC6E1F()
        {
            AssertCode(0x7DDC6E1F, "@@@");
        }

        // Unknown PowerPC X instruction 7DDF1B84 1F-1C2(450)
        [Test]
        public void PPCDis_7DDF1B84()
        {
            AssertCode(0x7DDF1B84, "@@@");
        }

        // Unknown PowerPC X instruction 7DEA7DF4 1F-2FA(762)
        [Test]
        public void PPCDis_7DEA7DF4()
        {
            AssertCode(0x7DEA7DF4, "@@@");
        }

        // Unknown PowerPC X instruction 7DEE03D9 1F-1EC(492)
        [Test]
        public void PPCDis_7DEE03D9()
        {
            AssertCode(0x7DEE03D9, "@@@");
        }

        // Unknown PowerPC X instruction 7DEF7B30 1F-198 (408)
        [Test]
        public void PPCDis_7DEF7B30()
        {
            AssertCode(0x7DEF7B30, "@@@");
        }

        // Unknown PowerPC X instruction 7DF50207 1F-103 (259)
        [Test]
        public void PPCDis_7DF50207()
        {
            AssertCode(0x7DF50207, "@@@");
        }

        // Unknown PowerPC X instruction 7DFA4EEE 1F-377 (887)
        [Test]
        public void PPCDis_7DFA4EEE()
        {
            AssertCode(0x7DFA4EEE, "@@@");
        }

        // Unknown PowerPC X instruction 7DFD7E06 1F-303 (771)
        [Test]
        public void PPCDis_7DFD7E06()
        {
            AssertCode(0x7DFD7E06, "@@@");
        }

        // Unknown PowerPC X instruction 7E0001A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7E0001A4()
        {
            AssertCode(0x7E0001A4, "@@@");
        }

        // Unknown PowerPC X instruction 7E0004A6 1F-253 (595)
        [Test]
        public void PPCDis_7E0004A6()
        {
            AssertCode(0x7E0004A6, "@@@");
        }

        // Unknown PowerPC X instruction 7E0227CF 1F-3E7 (999)
        [Test]
        public void PPCDis_7E0227CF()
        {
            AssertCode(0x7E0227CF, "@@@");
        }

        // Unknown PowerPC X instruction 7E0F7E19 1F-30C(780)
        [Test]
        public void PPCDis_7E0F7E19()
        {
            AssertCode(0x7E0F7E19, "@@@");
        }

        // Unknown PowerPC X instruction 7E1306F0 1F-378 (888)
        [Test]
        public void PPCDis_7E1306F0()
        {
            AssertCode(0x7E1306F0, "@@@");
        }

        // Unknown PowerPC X instruction 7E17D383 1F-1C1(449)
        [Test]
        public void PPCDis_7E17D383()
        {
            AssertCode(0x7E17D383, "@@@");
        }

        // Unknown PowerPC X instruction 7E2101A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7E2101A4()
        {
            AssertCode(0x7E2101A4, "@@@");
        }

        // Unknown PowerPC X instruction 7E2104A6 1F-253 (595)
        [Test]
        public void PPCDis_7E2104A6()
        {
            AssertCode(0x7E2104A6, "@@@");
        }

        // Unknown PowerPC X instruction 7E227E2B 1F-315 (789)
        [Test]
        public void PPCDis_7E227E2B()
        {
            AssertCode(0x7E227E2B, "@@@");
        }

        // Unknown PowerPC X instruction 7E270CF2 1F-279 (633)
        [Test]
        public void PPCDis_7E270CF2()
        {
            AssertCode(0x7E270CF2, "@@@");
        }

        // Unknown PowerPC X instruction 7E2B3219 1F-10C(268)
        [Test]
        public void PPCDis_7E2B3219()
        {
            AssertCode(0x7E2B3219, "@@@");
        }

        // Unknown PowerPC X instruction 7E357E3E 1F-31F (799)
        [Test]
        public void PPCDis_7E357E3E()
        {
            AssertCode(0x7E357E3E, "@@@");
        }

        // Unknown PowerPC X instruction 7E4201A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7E4201A4()
        {
            AssertCode(0x7E4201A4, "@@@");
        }

        // Unknown PowerPC X instruction 7E4204A6 1F-253 (595)
        [Test]
        public void PPCDis_7E4204A6()
        {
            AssertCode(0x7E4204A6, "@@@");
        }

        // Unknown PowerPC X instruction 7E477E51 1F-328 (808)
        [Test]
        public void PPCDis_7E477E51()
        {
            AssertCode(0x7E477E51, "@@@");
        }

        // Unknown PowerPC X instruction 7E525D9E 1F-2CF(719)
        [Test]
        public void PPCDis_7E525D9E()
        {
            AssertCode(0x7E525D9E, "@@@");
        }

        // Unknown PowerPC X instruction 7E54C6E4 1F-372 (882)
        [Test]
        public void PPCDis_7E54C6E4()
        {
            AssertCode(0x7E54C6E4, "@@@");
        }

        // Unknown PowerPC X instruction 7E59EBDE 1F-1EF (495)
        [Test]
        public void PPCDis_7E59EBDE()
        {
            AssertCode(0x7E59EBDE, "@@@");
        }

        // Unknown PowerPC X instruction 7E5A7E63 1F-331 (817)
        [Test]
        public void PPCDis_7E5A7E63()
        {
            AssertCode(0x7E5A7E63, "@@@");
        }

        // Unknown PowerPC X instruction 7E6301A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7E6301A4()
        {
            AssertCode(0x7E6301A4, "@@@");
        }

        // Unknown PowerPC X instruction 7E6304A6 1F-253 (595)
        [Test]
        public void PPCDis_7E6304A6()
        {
            AssertCode(0x7E6304A6, "@@@");
        }

        // Unknown PowerPC X instruction 7E77854A 1F-2A5(677)
        [Test]
        public void PPCDis_7E77854A()
        {
            AssertCode(0x7E77854A, "@@@");
        }

        // Unknown PowerPC X instruction 7E7CEFDF 1F-3EF (1007)
        [Test]
        public void PPCDis_7E7CEFDF()
        {
            AssertCode(0x7E7CEFDF, "@@@");
        }

        // Unknown PowerPC X instruction 7E7D874E 1F-3A7(935)
        [Test]
        public void PPCDis_7E7D874E()
        {
            AssertCode(0x7E7D874E, "@@@");
        }

        // Unknown PowerPC X instruction 7E7F7E88 1F-344 (836)
        [Test]
        public void PPCDis_7E7F7E88()
        {
            AssertCode(0x7E7F7E88, "@@@");
        }

        // Unknown PowerPC X instruction 7E8401A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7E8401A4()
        {
            AssertCode(0x7E8401A4, "@@@");
        }

        // Unknown PowerPC X instruction 7E8404A6 1F-253 (595)
        [Test]
        public void PPCDis_7E8404A6()
        {
            AssertCode(0x7E8404A6, "@@@");
        }

        // Unknown PowerPC X instruction 7E8E6656 1F-32B(811)
        [Test]
        public void PPCDis_7E8E6656()
        {
            AssertCode(0x7E8E6656, "@@@");
        }

        // Unknown PowerPC X instruction 7E927E9B 1F-34D (845)
        [Test]
        public void PPCDis_7E927E9B()
        {
            AssertCode(0x7E927E9B, "@@@");
        }

        // Unknown PowerPC X instruction 7E967699 1F-34C(844)
        [Test]
        public void PPCDis_7E967699()
        {
            AssertCode(0x7E967699, "@@@");
        }

        // Unknown PowerPC X instruction 7E9C1D16 1F-28B(651)
        [Test]
        public void PPCDis_7E9C1D16()
        {
            AssertCode(0x7E9C1D16, "@@@");
        }

        // Unknown PowerPC X instruction 7EA47EAE 1F-357 (855)
        [Test]
        public void PPCDis_7EA47EAE()
        {
            AssertCode(0x7EA47EAE, "@@@");
        }

        // Unknown PowerPC X instruction 7EA501A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7EA501A4()
        {
            AssertCode(0x7EA501A4, "@@@");
        }

        // Unknown PowerPC X instruction 7EA504A6 1F-253 (595)
        [Test]
        public void PPCDis_7EA504A6()
        {
            AssertCode(0x7EA504A6, "@@@");
        }

        // Unknown PowerPC X instruction 7EB07297 1F-14B(331)
        [Test]
        public void PPCDis_7EB07297()
        {
            AssertCode(0x7EB07297, "@@@");
        }

        // Unknown PowerPC X instruction 7EB77EC0 1F-360 (864)
        [Test]
        public void PPCDis_7EB77EC0()
        {
            AssertCode(0x7EB77EC0, "@@@");
        }

        // Unknown PowerPC X instruction 7EBCDAF5 1F-17A(378)
        [Test]
        public void PPCDis_7EBCDAF5()
        {
            AssertCode(0x7EBCDAF5, "@@@");
        }

        // Unknown PowerPC X instruction 7EC601A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7EC601A4()
        {
            AssertCode(0x7EC601A4, "@@@");
        }

        // Unknown PowerPC X instruction 7EC604A6 1F-253 (595)
        [Test]
        public void PPCDis_7EC604A6()
        {
            AssertCode(0x7EC604A6, "@@@");
        }

        // Unknown PowerPC X instruction 7ECA7ED3 1F-369 (873)
        [Test]
        public void PPCDis_7ECA7ED3()
        {
            AssertCode(0x7ECA7ED3, "@@@");
        }

        // Unknown PowerPC X instruction 7EDC7EE6 1F-373 (883)
        [Test]
        public void PPCDis_7EDC7EE6()
        {
            AssertCode(0x7EDC7EE6, "@@@");
        }

        // Unknown PowerPC X instruction 7EE701A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7EE701A4()
        {
            AssertCode(0x7EE701A4, "@@@");
        }

        // Unknown PowerPC X instruction 7EE704A6 1F-253 (595)
        [Test]
        public void PPCDis_7EE704A6()
        {
            AssertCode(0x7EE704A6, "@@@");
        }

        // Unknown PowerPC X instruction 7EEF7EF8 1F-37C(892)
        [Test]
        public void PPCDis_7EEF7EF8()
        {
            AssertCode(0x7EEF7EF8, "@@@");
        }

        // Unknown PowerPC X instruction 7EF45BA5 1F-1D2 (466)
        [Test]
        public void PPCDis_7EF45BA5()
        {
            AssertCode(0x7EF45BA5, "@@@");
        }

        // Unknown PowerPC X instruction 7EFBFA9F 1F-14F (335)
        [Test]
        public void PPCDis_7EFBFA9F()
        {
            AssertCode(0x7EFBFA9F, "@@@");
        }

        // Unknown PowerPC X instruction 7EFCDAC8 1F-164 (356)
        [Test]
        public void PPCDis_7EFCDAC8()
        {
            AssertCode(0x7EFCDAC8, "@@@");
        }

        // Unknown PowerPC X instruction 7F027F0B 1F-385 (901)
        [Test]
        public void PPCDis_7F027F0B()
        {
            AssertCode(0x7F027F0B, "@@@");
        }

        // Unknown PowerPC X instruction 7F0801A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7F0801A4()
        {
            AssertCode(0x7F0801A4, "@@@");
        }

        // Unknown PowerPC X instruction 7F0804A6 1F-253 (595)
        [Test]
        public void PPCDis_7F0804A6()
        {
            AssertCode(0x7F0804A6, "@@@");
        }

        // Unknown PowerPC X instruction 7F157F1E 1F-38F (911)
        [Test]
        public void PPCDis_7F157F1E()
        {
            AssertCode(0x7F157F1E, "@@@");
        }

        // Unknown PowerPC X instruction 7F18B3EB 1F-1F5 (501)
        [Test]
        public void PPCDis_7F18B3EB()
        {
            AssertCode(0x7F18B3EB, "@@@");
        }

        // Unknown PowerPC X instruction 7F277F31 1F-398 (920)
        [Test]
        public void PPCDis_7F277F31()
        {
            AssertCode(0x7F277F31, "@@@");
        }

        // Unknown PowerPC X instruction 7F2901A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7F2901A4()
        {
            AssertCode(0x7F2901A4, "@@@");
        }

        // Unknown PowerPC X instruction 7F2904A6 1F-253 (595)
        [Test]
        public void PPCDis_7F2904A6()
        {
            AssertCode(0x7F2904A6, "@@@");
        }

        // Unknown PowerPC X instruction 7F2BB4A1 1F-250 (592)
        [Test]
        public void PPCDis_7F2BB4A1()
        {
            AssertCode(0x7F2BB4A1, "@@@");
        }

        // Unknown PowerPC X instruction 7F3A7F43 1F-3A1(929)
        [Test]
        public void PPCDis_7F3A7F43()
        {
            AssertCode(0x7F3A7F43, "@@@");
        }

        // Unknown PowerPC X instruction 7F4A01A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7F4A01A4()
        {
            AssertCode(0x7F4A01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7F4A04A6 1F-253 (595)
        [Test]
        public void PPCDis_7F4A04A6()
        {
            AssertCode(0x7F4A04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7F4D7F56 1F-3AB(939)
        [Test]
        public void PPCDis_7F4D7F56()
        {
            AssertCode(0x7F4D7F56, "@@@");
        }

        // Unknown PowerPC X instruction 7F577755 1F-3AA(938)
        [Test]
        public void PPCDis_7F577755()
        {
            AssertCode(0x7F577755, "@@@");
        }

        // Unknown PowerPC X instruction 7F607F69 1F-3B4(948)
        [Test]
        public void PPCDis_7F607F69()
        {
            AssertCode(0x7F607F69, "@@@");
        }

        // Unknown PowerPC X instruction 7F6B01A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7F6B01A4()
        {
            AssertCode(0x7F6B01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7F6B04A6 1F-253 (595)
        [Test]
        public void PPCDis_7F6B04A6()
        {
            AssertCode(0x7F6B04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7F727F7C 1F-3BE(958)
        [Test]
        public void PPCDis_7F727F7C()
        {
            AssertCode(0x7F727F7C, "@@@");
        }

        // Unknown PowerPC X instruction 7F7A7996 1F-0CB(203)
        [Test]
        public void PPCDis_7F7A7996()
        {
            AssertCode(0x7F7A7996, "@@@");
        }

        // Unknown PowerPC X instruction 7F857F8F 1F-3C7(967)
        [Test]
        public void PPCDis_7F857F8F()
        {
            AssertCode(0x7F857F8F, "@@@");
        }

        // Unknown PowerPC X instruction 7F8C01A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7F8C01A4()
        {
            AssertCode(0x7F8C01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7F8C04A6 1F-253 (595)
        [Test]
        public void PPCDis_7F8C04A6()
        {
            AssertCode(0x7F8C04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7F987FA1 1F-3D0 (976)
        [Test]
        public void PPCDis_7F987FA1()
        {
            AssertCode(0x7F987FA1, "@@@");
        }

        // Unknown PowerPC X instruction 7FAD01A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7FAD01A4()
        {
            AssertCode(0x7FAD01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7FAD04A6 1F-253 (595)
        [Test]
        public void PPCDis_7FAD04A6()
        {
            AssertCode(0x7FAD04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7FAFA1FE 1F-0FF(255)
        [Test]
        public void PPCDis_7FAFA1FE()
        {
            AssertCode(0x7FAFA1FE, "@@@");
        }

        // Unknown PowerPC X instruction 7FBE7FC7 1F-3E3 (995)
        [Test]
        public void PPCDis_7FBE7FC7()
        {
            AssertCode(0x7FBE7FC7, "@@@");
        }

        // Unknown PowerPC X instruction 7FCE01A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7FCE01A4()
        {
            AssertCode(0x7FCE01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7FCE04A6 1F-253 (595)
        [Test]
        public void PPCDis_7FCE04A6()
        {
            AssertCode(0x7FCE04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7FD07FDA 1F-3ED (1005)
        [Test]
        public void PPCDis_7FD07FDA()
        {
            AssertCode(0x7FD07FDA, "@@@");
        }

        // Unknown PowerPC X instruction 7FDDF80F 1F-007 (7)
        [Test]
        public void PPCDis_7FDDF80F()
        {
            AssertCode(0x7FDDF80F, "@@@");
        }

        // Unknown PowerPC X instruction 7FEF01A4 1F-0D2 (210)
        [Test]
        public void PPCDis_7FEF01A4()
        {
            AssertCode(0x7FEF01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7FEF04A6 1F-253 (595)
        [Test]
        public void PPCDis_7FEF04A6()
        {
            AssertCode(0x7FEF04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7FF03AA9 1F-154 (340)
        [Test]
        public void PPCDis_7FF03AA9()
        {
            AssertCode(0x7FF03AA9, "@@@");
        }

        // Unknown PowerPC X instruction 7FF5A686 1F-343 (835)
        [Test]
        public void PPCDis_7FF5A686()
        {
            AssertCode(0x7FF5A686, "@@@");
        }

        // Unknown PowerPC X instruction 7FFF817B 1F-0BD(189)
        [Test]
        public void PPCDis_7FFF817B()
        {
            AssertCode(0x7FFF817B, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0030018 3C-003 (3)
        [Test]
        public void PPCDis_F0030018()
        {
            AssertCode(0xF0030018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0030020 3C-004 (4)
        [Test]
        public void PPCDis_F0030020()
        {
            AssertCode(0xF0030020, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0040010 3C-002 (2)
        [Test]
        public void PPCDis_F0040010()
        {
            AssertCode(0xF0040010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0050028 3C-005 (5)
        [Test]
        public void PPCDis_F0050028()
        {
            AssertCode(0xF0050028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F00501C8 3C-039 (57)
        [Test]
        public void PPCDis_F00501C8()
        {
            AssertCode(0xF00501C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0060010 3C-002 (2)
        [Test]
        public void PPCDis_F0060010()
        {
            AssertCode(0xF0060010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0066000 3C-C00(3072)
        [Test]
        public void PPCDis_F0066000()
        {
            AssertCode(0xF0066000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F01B7427 3C-E84(3716)
        [Test]
        public void PPCDis_F01B7427()
        {
            AssertCode(0xF01B7427, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0230028 3C-005 (5)
        [Test]
        public void PPCDis_F0230028()
        {
            AssertCode(0xF0230028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0238008 3C-1001 (4097)
        [Test]
        public void PPCDis_F0238008()
        {
            AssertCode(0xF0238008, "@@@");
        }

        // Unknown PowerPC XX3 instruction F02501D0 3C-03A(58)
        [Test]
        public void PPCDis_F02501D0()
        {
            AssertCode(0xF02501D0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F026E000 3C-1C00(7168)
        [Test]
        public void PPCDis_F026E000()
        {
            AssertCode(0xF026E000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F03E7336 3C-E66(3686)
        [Test]
        public void PPCDis_F03E7336()
        {
            AssertCode(0xF03E7336, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0430010 3C-002 (2)
        [Test]
        public void PPCDis_F0430010()
        {
            AssertCode(0xF0430010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0450020 3C-004 (4)
        [Test]
        public void PPCDis_F0450020()
        {
            AssertCode(0xF0450020, "@@@");
        }

        // Unknown PowerPC XX3 instruction F04501D8 3C-03B(59)
        [Test]
        public void PPCDis_F04501D8()
        {
            AssertCode(0xF04501D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0466000 3C-C00(3072)
        [Test]
        public void PPCDis_F0466000()
        {
            AssertCode(0xF0466000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F06501E0 3C-03C(60)
        [Test]
        public void PPCDis_F06501E0()
        {
            AssertCode(0xF06501E0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F066E000 3C-1C00(7168)
        [Test]
        public void PPCDis_F066E000()
        {
            AssertCode(0xF066E000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F069A9F1 3C-153E (5438)
        [Test]
        public void PPCDis_F069A9F1()
        {
            AssertCode(0xF069A9F1, "@@@");
        }

        // Unknown PowerPC XX3 instruction F07B197D 3C-32F (815)
        [Test]
        public void PPCDis_F07B197D()
        {
            AssertCode(0xF07B197D, "@@@");
        }

        // Unknown PowerPC XX3 instruction F07F3BFF 3C-77F (1919)
        [Test]
        public void PPCDis_F07F3BFF()
        {
            AssertCode(0xF07F3BFF, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0830A26 3C-144 (324)
        [Test]
        public void PPCDis_F0830A26()
        {
            AssertCode(0xF0830A26, "@@@");
        }

        // Unknown PowerPC XX3 instruction F08501E8 3C-03D (61)
        [Test]
        public void PPCDis_F08501E8()
        {
            AssertCode(0xF08501E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F08DAF16 3C-15E2 (5602)
        [Test]
        public void PPCDis_F08DAF16()
        {
            AssertCode(0xF08DAF16, "@@@");
        }

        // Unknown PowerPC XX3 instruction F09EF686 3C-1ED0 (7888)
        [Test]
        public void PPCDis_F09EF686()
        {
            AssertCode(0xF09EF686, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0A501F0 3C-03E (62)
        [Test]
        public void PPCDis_F0A501F0()
        {
            AssertCode(0xF0A501F0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0BDE0FB 3C-1C1F(7199)
        [Test]
        public void PPCDis_F0BDE0FB()
        {
            AssertCode(0xF0BDE0FB, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0C501F8 3C-03F (63)
        [Test]
        public void PPCDis_F0C501F8()
        {
            AssertCode(0xF0C501F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0DDAFA3 3C-15F4 (5620)
        [Test]
        public void PPCDis_F0DDAFA3()
        {
            AssertCode(0xF0DDAFA3, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0E1C36A 3C-186D (6253)
        [Test]
        public void PPCDis_F0E1C36A()
        {
            AssertCode(0xF0E1C36A, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0E1C3EB 3C-187D (6269)
        [Test]
        public void PPCDis_F0E1C3EB()
        {
            AssertCode(0xF0E1C3EB, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0E1DFAA 3C-1BF5(7157)
        [Test]
        public void PPCDis_F0E1DFAA()
        {
            AssertCode(0xF0E1DFAA, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0E50200 3C-040 (64)
        [Test]
        public void PPCDis_F0E50200()
        {
            AssertCode(0xF0E50200, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0EBE080 3C-1C10(7184)
        [Test]
        public void PPCDis_F0EBE080()
        {
            AssertCode(0xF0EBE080, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0F1F2F3 3C-1E5E(7774)
        [Test]
        public void PPCDis_F0F1F2F3()
        {
            AssertCode(0xF0F1F2F3, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0FFF080 3C-1E10 (7696)
        [Test]
        public void PPCDis_F0FFF080()
        {
            AssertCode(0xF0FFF080, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1001A3F 3C-347 (839)
        [Test]
        public void PPCDis_F1001A3F()
        {
            AssertCode(0xF1001A3F, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1038028 3C-1005 (4101)
        [Test]
        public void PPCDis_F1038028()
        {
            AssertCode(0xF1038028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1050208 3C-041 (65)
        [Test]
        public void PPCDis_F1050208()
        {
            AssertCode(0xF1050208, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1250210 3C-042 (66)
        [Test]
        public void PPCDis_F1250210()
        {
            AssertCode(0xF1250210, "@@@");
        }

        // Unknown PowerPC XX3 instruction F12C8008 3C-1001 (4097)
        [Test]
        public void PPCDis_F12C8008()
        {
            AssertCode(0xF12C8008, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1318139 3C-1027 (4135)
        [Test]
        public void PPCDis_F1318139()
        {
            AssertCode(0xF1318139, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1321A3F 3C-347 (839)
        [Test]
        public void PPCDis_F1321A3F()
        {
            AssertCode(0xF1321A3F, "@@@");
        }

        // Unknown PowerPC XX3 instruction F13E809D 3C-1013 (4115)
        [Test]
        public void PPCDis_F13E809D()
        {
            AssertCode(0xF13E809D, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1450218 3C-043 (67)
        [Test]
        public void PPCDis_F1450218()
        {
            AssertCode(0xF1450218, "@@@");
        }

        // Unknown PowerPC XX3 instruction F14C0010 3C-002 (2)
        [Test]
        public void PPCDis_F14C0010()
        {
            AssertCode(0xF14C0010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F15C3497 3C-692 (1682)
        [Test]
        public void PPCDis_F15C3497()
        {
            AssertCode(0xF15C3497, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1638038 3C-1007 (4103)
        [Test]
        public void PPCDis_F1638038()
        {
            AssertCode(0xF1638038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1650220 3C-044 (68)
        [Test]
        public void PPCDis_F1650220()
        {
            AssertCode(0xF1650220, "@@@");
        }

        // Unknown PowerPC XX3 instruction F16C8018 3C-1003 (4099)
        [Test]
        public void PPCDis_F16C8018()
        {
            AssertCode(0xF16C8018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1838008 3C-1001 (4097)
        [Test]
        public void PPCDis_F1838008()
        {
            AssertCode(0xF1838008, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1850228 3C-045 (69)
        [Test]
        public void PPCDis_F1850228()
        {
            AssertCode(0xF1850228, "@@@");
        }

        // Unknown PowerPC XX3 instruction F18A074E 3C-0E9 (233)
        [Test]
        public void PPCDis_F18A074E()
        {
            AssertCode(0xF18A074E, "@@@");
        }

        // Unknown PowerPC XX3 instruction F18C0020 3C-004 (4)
        [Test]
        public void PPCDis_F18C0020()
        {
            AssertCode(0xF18C0020, "@@@");
        }

        // Unknown PowerPC XX3 instruction F18FBC95 3C-1792 (6034)
        [Test]
        public void PPCDis_F18FBC95()
        {
            AssertCode(0xF18FBC95, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1A30010 3C-002 (2)
        [Test]
        public void PPCDis_F1A30010()
        {
            AssertCode(0xF1A30010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1A38018 3C-1003 (4099)
        [Test]
        public void PPCDis_F1A38018()
        {
            AssertCode(0xF1A38018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1A50230 3C-046 (70)
        [Test]
        public void PPCDis_F1A50230()
        {
            AssertCode(0xF1A50230, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1AC8028 3C-1005 (4101)
        [Test]
        public void PPCDis_F1AC8028()
        {
            AssertCode(0xF1AC8028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1AFFA26 3C-1F44 (8004)
        [Test]
        public void PPCDis_F1AFFA26()
        {
            AssertCode(0xF1AFFA26, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1C100E8 3C-01D (29)
        [Test]
        public void PPCDis_F1C100E8()
        {
            AssertCode(0xF1C100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1C50010 3C-002 (2)
        [Test]
        public void PPCDis_F1C50010()
        {
            AssertCode(0xF1C50010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1C50238 3C-047 (71)
        [Test]
        public void PPCDis_F1C50238()
        {
            AssertCode(0xF1C50238, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1CC0030 3C-006 (6)
        [Test]
        public void PPCDis_F1CC0030()
        {
            AssertCode(0xF1CC0030, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1E100F8 3C-01F (31)
        [Test]
        public void PPCDis_F1E100F8()
        {
            AssertCode(0xF1E100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1E50018 3C-003 (3)
        [Test]
        public void PPCDis_F1E50018()
        {
            AssertCode(0xF1E50018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1E50240 3C-048 (72)
        [Test]
        public void PPCDis_F1E50240()
        {
            AssertCode(0xF1E50240, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1EC8038 3C-1007 (4103)
        [Test]
        public void PPCDis_F1EC8038()
        {
            AssertCode(0xF1EC8038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1F8167F 3C-2CF(719)
        [Test]
        public void PPCDis_F1F8167F()
        {
            AssertCode(0xF1F8167F, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2006E00 3C-DC0(3520)
        [Test]
        public void PPCDis_F2006E00()
        {
            AssertCode(0xF2006E00, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2010108 3C-021 (33)
        [Test]
        public void PPCDis_F2010108()
        {
            AssertCode(0xF2010108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2050248 3C-049 (73)
        [Test]
        public void PPCDis_F2050248()
        {
            AssertCode(0xF2050248, "@@@");
        }

        // Unknown PowerPC XX3 instruction F21BB423 3C-1684 (5764)
        [Test]
        public void PPCDis_F21BB423()
        {
            AssertCode(0xF21BB423, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2210118 3C-023 (35)
        [Test]
        public void PPCDis_F2210118()
        {
            AssertCode(0xF2210118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2250250 3C-04A(74)
        [Test]
        public void PPCDis_F2250250()
        {
            AssertCode(0xF2250250, "@@@");
        }

        // Unknown PowerPC XX3 instruction F22979B8 3C-F37(3895)
        [Test]
        public void PPCDis_F22979B8()
        {
            AssertCode(0xF22979B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F22D681F 3C-D03(3331)
        [Test]
        public void PPCDis_F22D681F()
        {
            AssertCode(0xF22D681F, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2410028 3C-005 (5)
        [Test]
        public void PPCDis_F2410028()
        {
            AssertCode(0xF2410028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F24100D8 3C-01B(27)
        [Test]
        public void PPCDis_F24100D8()
        {
            AssertCode(0xF24100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2410128 3C-025 (37)
        [Test]
        public void PPCDis_F2410128()
        {
            AssertCode(0xF2410128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2450258 3C-04B(75)
        [Test]
        public void PPCDis_F2450258()
        {
            AssertCode(0xF2450258, "@@@");
        }

        // Unknown PowerPC XX3 instruction F25567D1 3C-CFA(3322)
        [Test]
        public void PPCDis_F25567D1()
        {
            AssertCode(0xF25567D1, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2598ED7 3C-11DA(4570)
        [Test]
        public void PPCDis_F2598ED7()
        {
            AssertCode(0xF2598ED7, "@@@");
        }

        // Unknown PowerPC XX3 instruction F25BF000 3C-1E00 (7680)
        [Test]
        public void PPCDis_F25BF000()
        {
            AssertCode(0xF25BF000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2610038 3C-007 (7)
        [Test]
        public void PPCDis_F2610038()
        {
            AssertCode(0xF2610038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F26100E8 3C-01D (29)
        [Test]
        public void PPCDis_F26100E8()
        {
            AssertCode(0xF26100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2610138 3C-027 (39)
        [Test]
        public void PPCDis_F2610138()
        {
            AssertCode(0xF2610138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2650260 3C-04C(76)
        [Test]
        public void PPCDis_F2650260()
        {
            AssertCode(0xF2650260, "@@@");
        }

        // Unknown PowerPC XX3 instruction F274002E 3C-005 (5)
        [Test]
        public void PPCDis_F274002E()
        {
            AssertCode(0xF274002E, "@@@");
        }

        // Unknown PowerPC XX3 instruction F27547BD 3C-8F7 (2295)
        [Test]
        public void PPCDis_F27547BD()
        {
            AssertCode(0xF27547BD, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2758092 3C-1012 (4114)
        [Test]
        public void PPCDis_F2758092()
        {
            AssertCode(0xF2758092, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2786E68 3C-DCD(3533)
        [Test]
        public void PPCDis_F2786E68()
        {
            AssertCode(0xF2786E68, "@@@");
        }

        // Unknown PowerPC XX3 instruction F279BC1E 3C-1783 (6019)
        [Test]
        public void PPCDis_F279BC1E()
        {
            AssertCode(0xF279BC1E, "@@@");
        }

        // Unknown PowerPC XX3 instruction F27B7000 3C-E00(3584)
        [Test]
        public void PPCDis_F27B7000()
        {
            AssertCode(0xF27B7000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2810098 3C-013 (19)
        [Test]
        public void PPCDis_F2810098()
        {
            AssertCode(0xF2810098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F28100F8 3C-01F (31)
        [Test]
        public void PPCDis_F28100F8()
        {
            AssertCode(0xF28100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2810148 3C-029 (41)
        [Test]
        public void PPCDis_F2810148()
        {
            AssertCode(0xF2810148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2850268 3C-04D (77)
        [Test]
        public void PPCDis_F2850268()
        {
            AssertCode(0xF2850268, "@@@");
        }

        // Unknown PowerPC XX3 instruction F288EA4C 3C-1D49 (7497)
        [Test]
        public void PPCDis_F288EA4C()
        {
            AssertCode(0xF288EA4C, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A10058 3C-00B(11)
        [Test]
        public void PPCDis_F2A10058()
        {
            AssertCode(0xF2A10058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A10088 3C-011 (17)
        [Test]
        public void PPCDis_F2A10088()
        {
            AssertCode(0xF2A10088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A100A8 3C-015 (21)
        [Test]
        public void PPCDis_F2A100A8()
        {
            AssertCode(0xF2A100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A10108 3C-021 (33)
        [Test]
        public void PPCDis_F2A10108()
        {
            AssertCode(0xF2A10108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A10118 3C-023 (35)
        [Test]
        public void PPCDis_F2A10118()
        {
            AssertCode(0xF2A10118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A10158 3C-02B(43)
        [Test]
        public void PPCDis_F2A10158()
        {
            AssertCode(0xF2A10158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A50270 3C-04E (78)
        [Test]
        public void PPCDis_F2A50270()
        {
            AssertCode(0xF2A50270, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2ADF96A 3C-1F2D (7981)
        [Test]
        public void PPCDis_F2ADF96A()
        {
            AssertCode(0xF2ADF96A, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2B83D71 3C-7AE(1966)
        [Test]
        public void PPCDis_F2B83D71()
        {
            AssertCode(0xF2B83D71, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2BE5F39 3C-BE7(3047)
        [Test]
        public void PPCDis_F2BE5F39()
        {
            AssertCode(0xF2BE5F39, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10018 3C-003 (3)
        [Test]
        public void PPCDis_F2C10018()
        {
            AssertCode(0xF2C10018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10068 3C-00D (13)
        [Test]
        public void PPCDis_F2C10068()
        {
            AssertCode(0xF2C10068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10098 3C-013 (19)
        [Test]
        public void PPCDis_F2C10098()
        {
            AssertCode(0xF2C10098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C100B8 3C-017 (23)
        [Test]
        public void PPCDis_F2C100B8()
        {
            AssertCode(0xF2C100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10118 3C-023 (35)
        [Test]
        public void PPCDis_F2C10118()
        {
            AssertCode(0xF2C10118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10128 3C-025 (37)
        [Test]
        public void PPCDis_F2C10128()
        {
            AssertCode(0xF2C10128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10168 3C-02D (45)
        [Test]
        public void PPCDis_F2C10168()
        {
            AssertCode(0xF2C10168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C102A8 3C-055 (85)
        [Test]
        public void PPCDis_F2C102A8()
        {
            AssertCode(0xF2C102A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C50278 3C-04F (79)
        [Test]
        public void PPCDis_F2C50278()
        {
            AssertCode(0xF2C50278, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2CB563B 3C-AC7(2759)
        [Test]
        public void PPCDis_F2CB563B()
        {
            AssertCode(0xF2CB563B, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10028 3C-005 (5)
        [Test]
        public void PPCDis_F2E10028()
        {
            AssertCode(0xF2E10028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10058 3C-00B(11)
        [Test]
        public void PPCDis_F2E10058()
        {
            AssertCode(0xF2E10058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10078 3C-00F (15)
        [Test]
        public void PPCDis_F2E10078()
        {
            AssertCode(0xF2E10078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E100A8 3C-015 (21)
        [Test]
        public void PPCDis_F2E100A8()
        {
            AssertCode(0xF2E100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E100C8 3C-019 (25)
        [Test]
        public void PPCDis_F2E100C8()
        {
            AssertCode(0xF2E100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E100E8 3C-01D (29)
        [Test]
        public void PPCDis_F2E100E8()
        {
            AssertCode(0xF2E100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10128 3C-025 (37)
        [Test]
        public void PPCDis_F2E10128()
        {
            AssertCode(0xF2E10128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10138 3C-027 (39)
        [Test]
        public void PPCDis_F2E10138()
        {
            AssertCode(0xF2E10138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10178 3C-02F (47)
        [Test]
        public void PPCDis_F2E10178()
        {
            AssertCode(0xF2E10178, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E102B8 3C-057 (87)
        [Test]
        public void PPCDis_F2E102B8()
        {
            AssertCode(0xF2E102B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E102C8 3C-059 (89)
        [Test]
        public void PPCDis_F2E102C8()
        {
            AssertCode(0xF2E102C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E3F2E7 3C-1E5C(7772)
        [Test]
        public void PPCDis_F2E3F2E7()
        {
            AssertCode(0xF2E3F2E7, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E50280 3C-050 (80)
        [Test]
        public void PPCDis_F2E50280()
        {
            AssertCode(0xF2E50280, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E7F278 3C-1E4F (7759)
        [Test]
        public void PPCDis_F2E7F278()
        {
            AssertCode(0xF2E7F278, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2FEBBDD 3C-177B(6011)
        [Test]
        public void PPCDis_F2FEBBDD()
        {
            AssertCode(0xF2FEBBDD, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010038 3C-007 (7)
        [Test]
        public void PPCDis_F3010038()
        {
            AssertCode(0xF3010038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010068 3C-00D (13)
        [Test]
        public void PPCDis_F3010068()
        {
            AssertCode(0xF3010068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010088 3C-011 (17)
        [Test]
        public void PPCDis_F3010088()
        {
            AssertCode(0xF3010088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30100B8 3C-017 (23)
        [Test]
        public void PPCDis_F30100B8()
        {
            AssertCode(0xF30100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30100D8 3C-01B(27)
        [Test]
        public void PPCDis_F30100D8()
        {
            AssertCode(0xF30100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30100F8 3C-01F (31)
        [Test]
        public void PPCDis_F30100F8()
        {
            AssertCode(0xF30100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010138 3C-027 (39)
        [Test]
        public void PPCDis_F3010138()
        {
            AssertCode(0xF3010138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010148 3C-029 (41)
        [Test]
        public void PPCDis_F3010148()
        {
            AssertCode(0xF3010148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010188 3C-031 (49)
        [Test]
        public void PPCDis_F3010188()
        {
            AssertCode(0xF3010188, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30101B8 3C-037 (55)
        [Test]
        public void PPCDis_F30101B8()
        {
            AssertCode(0xF30101B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30102C8 3C-059 (89)
        [Test]
        public void PPCDis_F30102C8()
        {
            AssertCode(0xF30102C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30102D8 3C-05B(91)
        [Test]
        public void PPCDis_F30102D8()
        {
            AssertCode(0xF30102D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010338 3C-067 (103)
        [Test]
        public void PPCDis_F3010338()
        {
            AssertCode(0xF3010338, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3050288 3C-051 (81)
        [Test]
        public void PPCDis_F3050288()
        {
            AssertCode(0xF3050288, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30D8CC8 3C-1199 (4505)
        [Test]
        public void PPCDis_F30D8CC8()
        {
            AssertCode(0xF30D8CC8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3121BB8 3C-377 (887)
        [Test]
        public void PPCDis_F3121BB8()
        {
            AssertCode(0xF3121BB8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F318A729 3C-14E5 (5349)
        [Test]
        public void PPCDis_F318A729()
        {
            AssertCode(0xF318A729, "@@@");
        }

        // Unknown PowerPC XX3 instruction F31BE000 3C-1C00(7168)
        [Test]
        public void PPCDis_F31BE000()
        {
            AssertCode(0xF31BE000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F31C473E 3C-8E7 (2279)
        [Test]
        public void PPCDis_F31C473E()
        {
            AssertCode(0xF31C473E, "@@@");
        }

        // Unknown PowerPC XX3 instruction F31C6030 3C-C06(3078)
        [Test]
        public void PPCDis_F31C6030()
        {
            AssertCode(0xF31C6030, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210028 3C-005 (5)
        [Test]
        public void PPCDis_F3210028()
        {
            AssertCode(0xF3210028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210038 3C-007 (7)
        [Test]
        public void PPCDis_F3210038()
        {
            AssertCode(0xF3210038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210068 3C-00D (13)
        [Test]
        public void PPCDis_F3210068()
        {
            AssertCode(0xF3210068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210078 3C-00F (15)
        [Test]
        public void PPCDis_F3210078()
        {
            AssertCode(0xF3210078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210098 3C-013 (19)
        [Test]
        public void PPCDis_F3210098()
        {
            AssertCode(0xF3210098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32100C8 3C-019 (25)
        [Test]
        public void PPCDis_F32100C8()
        {
            AssertCode(0xF32100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32100E8 3C-01D (29)
        [Test]
        public void PPCDis_F32100E8()
        {
            AssertCode(0xF32100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210108 3C-021 (33)
        [Test]
        public void PPCDis_F3210108()
        {
            AssertCode(0xF3210108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210148 3C-029 (41)
        [Test]
        public void PPCDis_F3210148()
        {
            AssertCode(0xF3210148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210158 3C-02B(43)
        [Test]
        public void PPCDis_F3210158()
        {
            AssertCode(0xF3210158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210198 3C-033 (51)
        [Test]
        public void PPCDis_F3210198()
        {
            AssertCode(0xF3210198, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32101C8 3C-039 (57)
        [Test]
        public void PPCDis_F32101C8()
        {
            AssertCode(0xF32101C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32102D8 3C-05B(91)
        [Test]
        public void PPCDis_F32102D8()
        {
            AssertCode(0xF32102D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32102E8 3C-05D (93)
        [Test]
        public void PPCDis_F32102E8()
        {
            AssertCode(0xF32102E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210348 3C-069 (105)
        [Test]
        public void PPCDis_F3210348()
        {
            AssertCode(0xF3210348, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210998 3C-133 (307)
        [Test]
        public void PPCDis_F3210998()
        {
            AssertCode(0xF3210998, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3250290 3C-052 (82)
        [Test]
        public void PPCDis_F3250290()
        {
            AssertCode(0xF3250290, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32B718D 3C-E31(3633)
        [Test]
        public void PPCDis_F32B718D()
        {
            AssertCode(0xF32B718D, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32BD4C3 3C-1A98(6808)
        [Test]
        public void PPCDis_F32BD4C3()
        {
            AssertCode(0xF32BD4C3, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32F039C 3C-073 (115)
        [Test]
        public void PPCDis_F32F039C()
        {
            AssertCode(0xF32F039C, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3385CB8 3C-B97(2967)
        [Test]
        public void PPCDis_F3385CB8()
        {
            AssertCode(0xF3385CB8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F33B6000 3C-C00(3072)
        [Test]
        public void PPCDis_F33B6000()
        {
            AssertCode(0xF33B6000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F33FC277 3C-184E (6222)
        [Test]
        public void PPCDis_F33FC277()
        {
            AssertCode(0xF33FC277, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410018 3C-003 (3)
        [Test]
        public void PPCDis_F3410018()
        {
            AssertCode(0xF3410018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410028 3C-005 (5)
        [Test]
        public void PPCDis_F3410028()
        {
            AssertCode(0xF3410028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410038 3C-007 (7)
        [Test]
        public void PPCDis_F3410038()
        {
            AssertCode(0xF3410038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410058 3C-00B(11)
        [Test]
        public void PPCDis_F3410058()
        {
            AssertCode(0xF3410058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410078 3C-00F (15)
        [Test]
        public void PPCDis_F3410078()
        {
            AssertCode(0xF3410078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410088 3C-011 (17)
        [Test]
        public void PPCDis_F3410088()
        {
            AssertCode(0xF3410088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410098 3C-013 (19)
        [Test]
        public void PPCDis_F3410098()
        {
            AssertCode(0xF3410098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34100A8 3C-015 (21)
        [Test]
        public void PPCDis_F34100A8()
        {
            AssertCode(0xF34100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34100D8 3C-01B(27)
        [Test]
        public void PPCDis_F34100D8()
        {
            AssertCode(0xF34100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34100F8 3C-01F (31)
        [Test]
        public void PPCDis_F34100F8()
        {
            AssertCode(0xF34100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410118 3C-023 (35)
        [Test]
        public void PPCDis_F3410118()
        {
            AssertCode(0xF3410118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410128 3C-025 (37)
        [Test]
        public void PPCDis_F3410128()
        {
            AssertCode(0xF3410128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410158 3C-02B(43)
        [Test]
        public void PPCDis_F3410158()
        {
            AssertCode(0xF3410158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410168 3C-02D (45)
        [Test]
        public void PPCDis_F3410168()
        {
            AssertCode(0xF3410168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34101A8 3C-035 (53)
        [Test]
        public void PPCDis_F34101A8()
        {
            AssertCode(0xF34101A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34101D8 3C-03B(59)
        [Test]
        public void PPCDis_F34101D8()
        {
            AssertCode(0xF34101D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34102E8 3C-05D (93)
        [Test]
        public void PPCDis_F34102E8()
        {
            AssertCode(0xF34102E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34102F8 3C-05F (95)
        [Test]
        public void PPCDis_F34102F8()
        {
            AssertCode(0xF34102F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410358 3C-06B(107)
        [Test]
        public void PPCDis_F3410358()
        {
            AssertCode(0xF3410358, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410738 3C-0E7 (231)
        [Test]
        public void PPCDis_F3410738()
        {
            AssertCode(0xF3410738, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34109A8 3C-135 (309)
        [Test]
        public void PPCDis_F34109A8()
        {
            AssertCode(0xF34109A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3450298 3C-053 (83)
        [Test]
        public void PPCDis_F3450298()
        {
            AssertCode(0xF3450298, "@@@");
        }

        // Unknown PowerPC XX3 instruction F35BE000 3C-1C00(7168)
        [Test]
        public void PPCDis_F35BE000()
        {
            AssertCode(0xF35BE000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610018 3C-003 (3)
        [Test]
        public void PPCDis_F3610018()
        {
            AssertCode(0xF3610018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610028 3C-005 (5)
        [Test]
        public void PPCDis_F3610028()
        {
            AssertCode(0xF3610028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610038 3C-007 (7)
        [Test]
        public void PPCDis_F3610038()
        {
            AssertCode(0xF3610038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610058 3C-00B(11)
        [Test]
        public void PPCDis_F3610058()
        {
            AssertCode(0xF3610058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610068 3C-00D (13)
        [Test]
        public void PPCDis_F3610068()
        {
            AssertCode(0xF3610068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610078 3C-00F (15)
        [Test]
        public void PPCDis_F3610078()
        {
            AssertCode(0xF3610078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610088 3C-011 (17)
        [Test]
        public void PPCDis_F3610088()
        {
            AssertCode(0xF3610088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610098 3C-013 (19)
        [Test]
        public void PPCDis_F3610098()
        {
            AssertCode(0xF3610098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36100A8 3C-015 (21)
        [Test]
        public void PPCDis_F36100A8()
        {
            AssertCode(0xF36100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36100B8 3C-017 (23)
        [Test]
        public void PPCDis_F36100B8()
        {
            AssertCode(0xF36100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36100C8 3C-019 (25)
        [Test]
        public void PPCDis_F36100C8()
        {
            AssertCode(0xF36100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36100D8 3C-01B(27)
        [Test]
        public void PPCDis_F36100D8()
        {
            AssertCode(0xF36100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36100E8 3C-01D (29)
        [Test]
        public void PPCDis_F36100E8()
        {
            AssertCode(0xF36100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610108 3C-021 (33)
        [Test]
        public void PPCDis_F3610108()
        {
            AssertCode(0xF3610108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610128 3C-025 (37)
        [Test]
        public void PPCDis_F3610128()
        {
            AssertCode(0xF3610128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610138 3C-027 (39)
        [Test]
        public void PPCDis_F3610138()
        {
            AssertCode(0xF3610138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610158 3C-02B(43)
        [Test]
        public void PPCDis_F3610158()
        {
            AssertCode(0xF3610158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610168 3C-02D (45)
        [Test]
        public void PPCDis_F3610168()
        {
            AssertCode(0xF3610168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610178 3C-02F (47)
        [Test]
        public void PPCDis_F3610178()
        {
            AssertCode(0xF3610178, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36101B8 3C-037 (55)
        [Test]
        public void PPCDis_F36101B8()
        {
            AssertCode(0xF36101B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36101E8 3C-03D (61)
        [Test]
        public void PPCDis_F36101E8()
        {
            AssertCode(0xF36101E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36102F8 3C-05F (95)
        [Test]
        public void PPCDis_F36102F8()
        {
            AssertCode(0xF36102F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610308 3C-061 (97)
        [Test]
        public void PPCDis_F3610308()
        {
            AssertCode(0xF3610308, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610368 3C-06D (109)
        [Test]
        public void PPCDis_F3610368()
        {
            AssertCode(0xF3610368, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610748 3C-0E9 (233)
        [Test]
        public void PPCDis_F3610748()
        {
            AssertCode(0xF3610748, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36107C8 3C-0F9 (249)
        [Test]
        public void PPCDis_F36107C8()
        {
            AssertCode(0xF36107C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36109B8 3C-137 (311)
        [Test]
        public void PPCDis_F36109B8()
        {
            AssertCode(0xF36109B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36502A0 3C-054 (84)
        [Test]
        public void PPCDis_F36502A0()
        {
            AssertCode(0xF36502A0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F365DF2C 3C-1BE5(7141)
        [Test]
        public void PPCDis_F365DF2C()
        {
            AssertCode(0xF365DF2C, "@@@");
        }

        // Unknown PowerPC XX3 instruction F371286D 3C-50D (1293)
        [Test]
        public void PPCDis_F371286D()
        {
            AssertCode(0xF371286D, "@@@");
        }

        // Unknown PowerPC XX3 instruction F37B6000 3C-C00(3072)
        [Test]
        public void PPCDis_F37B6000()
        {
            AssertCode(0xF37B6000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810018 3C-003 (3)
        [Test]
        public void PPCDis_F3810018()
        {
            AssertCode(0xF3810018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810028 3C-005 (5)
        [Test]
        public void PPCDis_F3810028()
        {
            AssertCode(0xF3810028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810038 3C-007 (7)
        [Test]
        public void PPCDis_F3810038()
        {
            AssertCode(0xF3810038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810058 3C-00B(11)
        [Test]
        public void PPCDis_F3810058()
        {
            AssertCode(0xF3810058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810068 3C-00D (13)
        [Test]
        public void PPCDis_F3810068()
        {
            AssertCode(0xF3810068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810078 3C-00F (15)
        [Test]
        public void PPCDis_F3810078()
        {
            AssertCode(0xF3810078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810088 3C-011 (17)
        [Test]
        public void PPCDis_F3810088()
        {
            AssertCode(0xF3810088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810098 3C-013 (19)
        [Test]
        public void PPCDis_F3810098()
        {
            AssertCode(0xF3810098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100A8 3C-015 (21)
        [Test]
        public void PPCDis_F38100A8()
        {
            AssertCode(0xF38100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100B8 3C-017 (23)
        [Test]
        public void PPCDis_F38100B8()
        {
            AssertCode(0xF38100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100C8 3C-019 (25)
        [Test]
        public void PPCDis_F38100C8()
        {
            AssertCode(0xF38100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100D8 3C-01B(27)
        [Test]
        public void PPCDis_F38100D8()
        {
            AssertCode(0xF38100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100E8 3C-01D (29)
        [Test]
        public void PPCDis_F38100E8()
        {
            AssertCode(0xF38100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100F8 3C-01F (31)
        [Test]
        public void PPCDis_F38100F8()
        {
            AssertCode(0xF38100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810118 3C-023 (35)
        [Test]
        public void PPCDis_F3810118()
        {
            AssertCode(0xF3810118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810128 3C-025 (37)
        [Test]
        public void PPCDis_F3810128()
        {
            AssertCode(0xF3810128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810138 3C-027 (39)
        [Test]
        public void PPCDis_F3810138()
        {
            AssertCode(0xF3810138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810148 3C-029 (41)
        [Test]
        public void PPCDis_F3810148()
        {
            AssertCode(0xF3810148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810168 3C-02D (45)
        [Test]
        public void PPCDis_F3810168()
        {
            AssertCode(0xF3810168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810178 3C-02F (47)
        [Test]
        public void PPCDis_F3810178()
        {
            AssertCode(0xF3810178, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810188 3C-031 (49)
        [Test]
        public void PPCDis_F3810188()
        {
            AssertCode(0xF3810188, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810198 3C-033 (51)
        [Test]
        public void PPCDis_F3810198()
        {
            AssertCode(0xF3810198, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38101A8 3C-035 (53)
        [Test]
        public void PPCDis_F38101A8()
        {
            AssertCode(0xF38101A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38101C8 3C-039 (57)
        [Test]
        public void PPCDis_F38101C8()
        {
            AssertCode(0xF38101C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38101E8 3C-03D (61)
        [Test]
        public void PPCDis_F38101E8()
        {
            AssertCode(0xF38101E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38101F8 3C-03F (63)
        [Test]
        public void PPCDis_F38101F8()
        {
            AssertCode(0xF38101F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810308 3C-061 (97)
        [Test]
        public void PPCDis_F3810308()
        {
            AssertCode(0xF3810308, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810318 3C-063 (99)
        [Test]
        public void PPCDis_F3810318()
        {
            AssertCode(0xF3810318, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810378 3C-06F (111)
        [Test]
        public void PPCDis_F3810378()
        {
            AssertCode(0xF3810378, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810758 3C-0EB(235)
        [Test]
        public void PPCDis_F3810758()
        {
            AssertCode(0xF3810758, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38107D8 3C-0FB(251)
        [Test]
        public void PPCDis_F38107D8()
        {
            AssertCode(0xF38107D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38109C8 3C-139 (313)
        [Test]
        public void PPCDis_F38109C8()
        {
            AssertCode(0xF38109C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3842EEC 3C-5DD(1501)
        [Test]
        public void PPCDis_F3842EEC()
        {
            AssertCode(0xF3842EEC, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38502A8 3C-055 (85)
        [Test]
        public void PPCDis_F38502A8()
        {
            AssertCode(0xF38502A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38704AC 3C-095 (149)
        [Test]
        public void PPCDis_F38704AC()
        {
            AssertCode(0xF38704AC, "@@@");
        }

        // Unknown PowerPC XX3 instruction F39BE000 3C-1C00(7168)
        [Test]
        public void PPCDis_F39BE000()
        {
            AssertCode(0xF39BE000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10018 3C-003 (3)
        [Test]
        public void PPCDis_F3A10018()
        {
            AssertCode(0xF3A10018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10028 3C-005 (5)
        [Test]
        public void PPCDis_F3A10028()
        {
            AssertCode(0xF3A10028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10038 3C-007 (7)
        [Test]
        public void PPCDis_F3A10038()
        {
            AssertCode(0xF3A10038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10058 3C-00B(11)
        [Test]
        public void PPCDis_F3A10058()
        {
            AssertCode(0xF3A10058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10068 3C-00D (13)
        [Test]
        public void PPCDis_F3A10068()
        {
            AssertCode(0xF3A10068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10078 3C-00F (15)
        [Test]
        public void PPCDis_F3A10078()
        {
            AssertCode(0xF3A10078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10088 3C-011 (17)
        [Test]
        public void PPCDis_F3A10088()
        {
            AssertCode(0xF3A10088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10098 3C-013 (19)
        [Test]
        public void PPCDis_F3A10098()
        {
            AssertCode(0xF3A10098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100A8 3C-015 (21)
        [Test]
        public void PPCDis_F3A100A8()
        {
            AssertCode(0xF3A100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100B8 3C-017 (23)
        [Test]
        public void PPCDis_F3A100B8()
        {
            AssertCode(0xF3A100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100C8 3C-019 (25)
        [Test]
        public void PPCDis_F3A100C8()
        {
            AssertCode(0xF3A100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100D8 3C-01B(27)
        [Test]
        public void PPCDis_F3A100D8()
        {
            AssertCode(0xF3A100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100E8 3C-01D (29)
        [Test]
        public void PPCDis_F3A100E8()
        {
            AssertCode(0xF3A100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100F8 3C-01F (31)
        [Test]
        public void PPCDis_F3A100F8()
        {
            AssertCode(0xF3A100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10108 3C-021 (33)
        [Test]
        public void PPCDis_F3A10108()
        {
            AssertCode(0xF3A10108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10118 3C-023 (35)
        [Test]
        public void PPCDis_F3A10118()
        {
            AssertCode(0xF3A10118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10128 3C-025 (37)
        [Test]
        public void PPCDis_F3A10128()
        {
            AssertCode(0xF3A10128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10138 3C-027 (39)
        [Test]
        public void PPCDis_F3A10138()
        {
            AssertCode(0xF3A10138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10148 3C-029 (41)
        [Test]
        public void PPCDis_F3A10148()
        {
            AssertCode(0xF3A10148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10158 3C-02B(43)
        [Test]
        public void PPCDis_F3A10158()
        {
            AssertCode(0xF3A10158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10178 3C-02F (47)
        [Test]
        public void PPCDis_F3A10178()
        {
            AssertCode(0xF3A10178, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10188 3C-031 (49)
        [Test]
        public void PPCDis_F3A10188()
        {
            AssertCode(0xF3A10188, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10198 3C-033 (51)
        [Test]
        public void PPCDis_F3A10198()
        {
            AssertCode(0xF3A10198, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A101A8 3C-035 (53)
        [Test]
        public void PPCDis_F3A101A8()
        {
            AssertCode(0xF3A101A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A101B8 3C-037 (55)
        [Test]
        public void PPCDis_F3A101B8()
        {
            AssertCode(0xF3A101B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A101D8 3C-03B(59)
        [Test]
        public void PPCDis_F3A101D8()
        {
            AssertCode(0xF3A101D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A101F8 3C-03F (63)
        [Test]
        public void PPCDis_F3A101F8()
        {
            AssertCode(0xF3A101F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10208 3C-041 (65)
        [Test]
        public void PPCDis_F3A10208()
        {
            AssertCode(0xF3A10208, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10318 3C-063 (99)
        [Test]
        public void PPCDis_F3A10318()
        {
            AssertCode(0xF3A10318, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10328 3C-065 (101)
        [Test]
        public void PPCDis_F3A10328()
        {
            AssertCode(0xF3A10328, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10388 3C-071 (113)
        [Test]
        public void PPCDis_F3A10388()
        {
            AssertCode(0xF3A10388, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10568 3C-0AD(173)
        [Test]
        public void PPCDis_F3A10568()
        {
            AssertCode(0xF3A10568, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10768 3C-0ED (237)
        [Test]
        public void PPCDis_F3A10768()
        {
            AssertCode(0xF3A10768, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A107E8 3C-0FD(253)
        [Test]
        public void PPCDis_F3A107E8()
        {
            AssertCode(0xF3A107E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A109D8 3C-13B(315)
        [Test]
        public void PPCDis_F3A109D8()
        {
            AssertCode(0xF3A109D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A502B0 3C-056 (86)
        [Test]
        public void PPCDis_F3A502B0()
        {
            AssertCode(0xF3A502B0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3B5F7EC 3C-1EFD(7933)
        [Test]
        public void PPCDis_F3B5F7EC()
        {
            AssertCode(0xF3B5F7EC, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3BB6000 3C-C00(3072)
        [Test]
        public void PPCDis_F3BB6000()
        {
            AssertCode(0xF3BB6000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3BC2D54 3C-5AA(1450)
        [Test]
        public void PPCDis_F3BC2D54()
        {
            AssertCode(0xF3BC2D54, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3BF8AA6 3C-1154 (4436)
        [Test]
        public void PPCDis_F3BF8AA6()
        {
            AssertCode(0xF3BF8AA6, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10018 3C-003 (3)
        [Test]
        public void PPCDis_F3C10018()
        {
            AssertCode(0xF3C10018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10028 3C-005 (5)
        [Test]
        public void PPCDis_F3C10028()
        {
            AssertCode(0xF3C10028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10038 3C-007 (7)
        [Test]
        public void PPCDis_F3C10038()
        {
            AssertCode(0xF3C10038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10058 3C-00B(11)
        [Test]
        public void PPCDis_F3C10058()
        {
            AssertCode(0xF3C10058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10068 3C-00D (13)
        [Test]
        public void PPCDis_F3C10068()
        {
            AssertCode(0xF3C10068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10078 3C-00F (15)
        [Test]
        public void PPCDis_F3C10078()
        {
            AssertCode(0xF3C10078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10088 3C-011 (17)
        [Test]
        public void PPCDis_F3C10088()
        {
            AssertCode(0xF3C10088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10098 3C-013 (19)
        [Test]
        public void PPCDis_F3C10098()
        {
            AssertCode(0xF3C10098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100A8 3C-015 (21)
        [Test]
        public void PPCDis_F3C100A8()
        {
            AssertCode(0xF3C100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100B8 3C-017 (23)
        [Test]
        public void PPCDis_F3C100B8()
        {
            AssertCode(0xF3C100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100C8 3C-019 (25)
        [Test]
        public void PPCDis_F3C100C8()
        {
            AssertCode(0xF3C100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100D8 3C-01B(27)
        [Test]
        public void PPCDis_F3C100D8()
        {
            AssertCode(0xF3C100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100E8 3C-01D (29)
        [Test]
        public void PPCDis_F3C100E8()
        {
            AssertCode(0xF3C100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100F8 3C-01F (31)
        [Test]
        public void PPCDis_F3C100F8()
        {
            AssertCode(0xF3C100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10108 3C-021 (33)
        [Test]
        public void PPCDis_F3C10108()
        {
            AssertCode(0xF3C10108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10118 3C-023 (35)
        [Test]
        public void PPCDis_F3C10118()
        {
            AssertCode(0xF3C10118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10128 3C-025 (37)
        [Test]
        public void PPCDis_F3C10128()
        {
            AssertCode(0xF3C10128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10138 3C-027 (39)
        [Test]
        public void PPCDis_F3C10138()
        {
            AssertCode(0xF3C10138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10148 3C-029 (41)
        [Test]
        public void PPCDis_F3C10148()
        {
            AssertCode(0xF3C10148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10158 3C-02B(43)
        [Test]
        public void PPCDis_F3C10158()
        {
            AssertCode(0xF3C10158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10168 3C-02D (45)
        [Test]
        public void PPCDis_F3C10168()
        {
            AssertCode(0xF3C10168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10188 3C-031 (49)
        [Test]
        public void PPCDis_F3C10188()
        {
            AssertCode(0xF3C10188, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10198 3C-033 (51)
        [Test]
        public void PPCDis_F3C10198()
        {
            AssertCode(0xF3C10198, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C101A8 3C-035 (53)
        [Test]
        public void PPCDis_F3C101A8()
        {
            AssertCode(0xF3C101A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C101B8 3C-037 (55)
        [Test]
        public void PPCDis_F3C101B8()
        {
            AssertCode(0xF3C101B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C101C8 3C-039 (57)
        [Test]
        public void PPCDis_F3C101C8()
        {
            AssertCode(0xF3C101C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C101E8 3C-03D (61)
        [Test]
        public void PPCDis_F3C101E8()
        {
            AssertCode(0xF3C101E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10208 3C-041 (65)
        [Test]
        public void PPCDis_F3C10208()
        {
            AssertCode(0xF3C10208, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10218 3C-043 (67)
        [Test]
        public void PPCDis_F3C10218()
        {
            AssertCode(0xF3C10218, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10328 3C-065 (101)
        [Test]
        public void PPCDis_F3C10328()
        {
            AssertCode(0xF3C10328, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10338 3C-067 (103)
        [Test]
        public void PPCDis_F3C10338()
        {
            AssertCode(0xF3C10338, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10398 3C-073 (115)
        [Test]
        public void PPCDis_F3C10398()
        {
            AssertCode(0xF3C10398, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C103E8 3C-07D (125)
        [Test]
        public void PPCDis_F3C103E8()
        {
            AssertCode(0xF3C103E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10578 3C-0AF(175)
        [Test]
        public void PPCDis_F3C10578()
        {
            AssertCode(0xF3C10578, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10668 3C-0CD(205)
        [Test]
        public void PPCDis_F3C10668()
        {
            AssertCode(0xF3C10668, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10778 3C-0EF (239)
        [Test]
        public void PPCDis_F3C10778()
        {
            AssertCode(0xF3C10778, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C107F8 3C-0FF(255)
        [Test]
        public void PPCDis_F3C107F8()
        {
            AssertCode(0xF3C107F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C109E8 3C-13D (317)
        [Test]
        public void PPCDis_F3C109E8()
        {
            AssertCode(0xF3C109E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C30020 3C-004 (4)
        [Test]
        public void PPCDis_F3C30020()
        {
            AssertCode(0xF3C30020, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C502B8 3C-057 (87)
        [Test]
        public void PPCDis_F3C502B8()
        {
            AssertCode(0xF3C502B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3DB35E9 3C-6BD(1725)
        [Test]
        public void PPCDis_F3DB35E9()
        {
            AssertCode(0xF3DB35E9, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3DBE000 3C-1C00(7168)
        [Test]
        public void PPCDis_F3DBE000()
        {
            AssertCode(0xF3DBE000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3DC9B15 3C-1362 (4962)
        [Test]
        public void PPCDis_F3DC9B15()
        {
            AssertCode(0xF3DC9B15, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10018 3C-003 (3)
        [Test]
        public void PPCDis_F3E10018()
        {
            AssertCode(0xF3E10018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10028 3C-005 (5)
        [Test]
        public void PPCDis_F3E10028()
        {
            AssertCode(0xF3E10028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10038 3C-007 (7)
        [Test]
        public void PPCDis_F3E10038()
        {
            AssertCode(0xF3E10038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10058 3C-00B(11)
        [Test]
        public void PPCDis_F3E10058()
        {
            AssertCode(0xF3E10058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10068 3C-00D (13)
        [Test]
        public void PPCDis_F3E10068()
        {
            AssertCode(0xF3E10068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10078 3C-00F (15)
        [Test]
        public void PPCDis_F3E10078()
        {
            AssertCode(0xF3E10078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10088 3C-011 (17)
        [Test]
        public void PPCDis_F3E10088()
        {
            AssertCode(0xF3E10088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10098 3C-013 (19)
        [Test]
        public void PPCDis_F3E10098()
        {
            AssertCode(0xF3E10098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100A8 3C-015 (21)
        [Test]
        public void PPCDis_F3E100A8()
        {
            AssertCode(0xF3E100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100B8 3C-017 (23)
        [Test]
        public void PPCDis_F3E100B8()
        {
            AssertCode(0xF3E100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100C8 3C-019 (25)
        [Test]
        public void PPCDis_F3E100C8()
        {
            AssertCode(0xF3E100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100D8 3C-01B(27)
        [Test]
        public void PPCDis_F3E100D8()
        {
            AssertCode(0xF3E100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100E8 3C-01D (29)
        [Test]
        public void PPCDis_F3E100E8()
        {
            AssertCode(0xF3E100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100F8 3C-01F (31)
        [Test]
        public void PPCDis_F3E100F8()
        {
            AssertCode(0xF3E100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10108 3C-021 (33)
        [Test]
        public void PPCDis_F3E10108()
        {
            AssertCode(0xF3E10108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10118 3C-023 (35)
        [Test]
        public void PPCDis_F3E10118()
        {
            AssertCode(0xF3E10118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10128 3C-025 (37)
        [Test]
        public void PPCDis_F3E10128()
        {
            AssertCode(0xF3E10128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10138 3C-027 (39)
        [Test]
        public void PPCDis_F3E10138()
        {
            AssertCode(0xF3E10138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10148 3C-029 (41)
        [Test]
        public void PPCDis_F3E10148()
        {
            AssertCode(0xF3E10148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10158 3C-02B(43)
        [Test]
        public void PPCDis_F3E10158()
        {
            AssertCode(0xF3E10158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10168 3C-02D (45)
        [Test]
        public void PPCDis_F3E10168()
        {
            AssertCode(0xF3E10168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10178 3C-02F (47)
        [Test]
        public void PPCDis_F3E10178()
        {
            AssertCode(0xF3E10178, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10188 3C-031 (49)
        [Test]
        public void PPCDis_F3E10188()
        {
            AssertCode(0xF3E10188, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10198 3C-033 (51)
        [Test]
        public void PPCDis_F3E10198()
        {
            AssertCode(0xF3E10198, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E101A8 3C-035 (53)
        [Test]
        public void PPCDis_F3E101A8()
        {
            AssertCode(0xF3E101A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E101B8 3C-037 (55)
        [Test]
        public void PPCDis_F3E101B8()
        {
            AssertCode(0xF3E101B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E101C8 3C-039 (57)
        [Test]
        public void PPCDis_F3E101C8()
        {
            AssertCode(0xF3E101C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E101D8 3C-03B(59)
        [Test]
        public void PPCDis_F3E101D8()
        {
            AssertCode(0xF3E101D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E101F8 3C-03F (63)
        [Test]
        public void PPCDis_F3E101F8()
        {
            AssertCode(0xF3E101F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10218 3C-043 (67)
        [Test]
        public void PPCDis_F3E10218()
        {
            AssertCode(0xF3E10218, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10228 3C-045 (69)
        [Test]
        public void PPCDis_F3E10228()
        {
            AssertCode(0xF3E10228, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10238 3C-047 (71)
        [Test]
        public void PPCDis_F3E10238()
        {
            AssertCode(0xF3E10238, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10338 3C-067 (103)
        [Test]
        public void PPCDis_F3E10338()
        {
            AssertCode(0xF3E10338, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10348 3C-069 (105)
        [Test]
        public void PPCDis_F3E10348()
        {
            AssertCode(0xF3E10348, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10388 3C-071 (113)
        [Test]
        public void PPCDis_F3E10388()
        {
            AssertCode(0xF3E10388, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E103A8 3C-075 (117)
        [Test]
        public void PPCDis_F3E103A8()
        {
            AssertCode(0xF3E103A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E103F8 3C-07F (127)
        [Test]
        public void PPCDis_F3E103F8()
        {
            AssertCode(0xF3E103F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E104B8 3C-097 (151)
        [Test]
        public void PPCDis_F3E104B8()
        {
            AssertCode(0xF3E104B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E104C8 3C-099 (153)
        [Test]
        public void PPCDis_F3E104C8()
        {
            AssertCode(0xF3E104C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10538 3C-0A7(167)
        [Test]
        public void PPCDis_F3E10538()
        {
            AssertCode(0xF3E10538, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10588 3C-0B1 (177)
        [Test]
        public void PPCDis_F3E10588()
        {
            AssertCode(0xF3E10588, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10678 3C-0CF(207)
        [Test]
        public void PPCDis_F3E10678()
        {
            AssertCode(0xF3E10678, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10788 3C-0F1 (241)
        [Test]
        public void PPCDis_F3E10788()
        {
            AssertCode(0xF3E10788, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10808 3C-101 (257)
        [Test]
        public void PPCDis_F3E10808()
        {
            AssertCode(0xF3E10808, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10928 3C-125 (293)
        [Test]
        public void PPCDis_F3E10928()
        {
            AssertCode(0xF3E10928, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E109F8 3C-13F (319)
        [Test]
        public void PPCDis_F3E109F8()
        {
            AssertCode(0xF3E109F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E30030 3C-006 (6)
        [Test]
        public void PPCDis_F3E30030()
        {
            AssertCode(0xF3E30030, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E502C0 3C-058 (88)
        [Test]
        public void PPCDis_F3E502C0()
        {
            AssertCode(0xF3E502C0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E6F6B9 3C-1ED7 (7895)
        [Test]
        public void PPCDis_F3E6F6B9()
        {
            AssertCode(0xF3E6F6B9, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3EBAE65 3C-15CC(5580)
        [Test]
        public void PPCDis_F3EBAE65()
        {
            AssertCode(0xF3EBAE65, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3F55C48 3C-B89(2953)
        [Test]
        public void PPCDis_F3F55C48()
        {
            AssertCode(0xF3F55C48, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3FB6000 3C-C00(3072)
        [Test]
        public void PPCDis_F3FB6000()
        {
            AssertCode(0xF3FB6000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3FCA9D0 3C-153A(5434)
        [Test]
        public void PPCDis_F3FCA9D0()
        {
            AssertCode(0xF3FCA9D0, "@@@");
        }
#endif
    }
}