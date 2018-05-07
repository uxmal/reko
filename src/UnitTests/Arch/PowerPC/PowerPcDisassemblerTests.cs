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
            AssertCode(0x7C7A03A6, "mtspr\t00000340,r3");
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
            //AssertCode(0x13FFF935, "@@@"); // 04 - 135(309)
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
            AssertCode(0x157FA9F1, "vmsub4fp128\tv11,v63,v53"); // 05 - 01D(29)
            AssertCode(0x15B8C2F1, "vor128\tv13,v56,v56");      // 05 - 02D(45)
            AssertCode(0x145AE331, "vxor128\tv2,v58,v60");      // 05 - 031(49)

            AssertCode(0x1B5FF8F5, "vslw128\tv58,v63,v63");     // 06 - 00F(15)
            //AssertCode(0x1BA5AA15, "@@@");    // 06 - 021(33)  - permutation odd encoding
            //AssertCode(0x1918F251, "@@@");    // 06 - 025(37)
            //AssertCode(0x180EB291, "@@@");    // 06 - 029(41)
            //AssertCode(0x1AE1D2D5, "@@@");    // 06 - 02D(45)
            //AssertCode(0x1B04AB15, "@@@");    // 06 - 031(49)
            AssertCode(0x1B1FF325, "vmrghw128\tv56,v63,v62");   // 06 - 032(50)
            //AssertCode(0x1B1BD355, "@@@");    // 06 - 035(53)
            AssertCode(0x1BFFF365, "vmrglw128\tv63,v63,v62");   // 06 - 036(54)
            //AssertCode(0x1B4CD395, "@@@");    // 06 - 039(57)
            //AssertCode(0x18ADA3D1, "@@@");    // 06 - 03D(61)
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
            // There's always some more work to do.... but most of these really are
            // invalid and should be removable.
            /*
            Unknown PowerPC instruction 00000000
            Unknown PowerPC instruction 00000001
            Unknown PowerPC instruction 00000002
            Unknown PowerPC instruction 00000003
            Unknown PowerPC instruction 00000004
            Unknown PowerPC instruction 00000005
            Unknown PowerPC instruction 00000006
            Unknown PowerPC instruction 00000007
            Unknown PowerPC instruction 00000008
            Unknown PowerPC instruction 00000009
            Unknown PowerPC instruction 0000000A
            Unknown PowerPC instruction 0000000B
            Unknown PowerPC instruction 0000000C
            Unknown PowerPC instruction 0000000D
            Unknown PowerPC instruction 0000000E
            Unknown PowerPC instruction 0000000F
            Unknown PowerPC instruction 00000010
            Unknown PowerPC instruction 00000011
            Unknown PowerPC instruction 00000012
            Unknown PowerPC instruction 00000013
            Unknown PowerPC instruction 00000014
            Unknown PowerPC instruction 00000015
            Unknown PowerPC instruction 00000016
            Unknown PowerPC instruction 00000017
            Unknown PowerPC instruction 00000018
            Unknown PowerPC instruction 00000019
            Unknown PowerPC instruction 0000001A
            Unknown PowerPC instruction 0000001B
            Unknown PowerPC instruction 0000001C
            Unknown PowerPC instruction 0000001D
            Unknown PowerPC instruction 0000001E
            Unknown PowerPC instruction 0000001F
            Unknown PowerPC instruction 00000020
            Unknown PowerPC instruction 00000021
            Unknown PowerPC instruction 00000022
            Unknown PowerPC instruction 00000023
            Unknown PowerPC instruction 00000024
            Unknown PowerPC instruction 00000025
            Unknown PowerPC instruction 00000026
            Unknown PowerPC instruction 00000027
            Unknown PowerPC instruction 00000028
            Unknown PowerPC instruction 00000029
            Unknown PowerPC instruction 0000002A
            Unknown PowerPC instruction 0000002B
            Unknown PowerPC instruction 0000002C
            Unknown PowerPC instruction 0000002D
            Unknown PowerPC instruction 0000002E
            Unknown PowerPC instruction 0000002F
            Unknown PowerPC instruction 00000030
            Unknown PowerPC instruction 00000031
            Unknown PowerPC instruction 00000032
            Unknown PowerPC instruction 00000033
            Unknown PowerPC instruction 00000034
            Unknown PowerPC instruction 00000035
            Unknown PowerPC instruction 00000036
            Unknown PowerPC instruction 00000037
            Unknown PowerPC instruction 00000038
            Unknown PowerPC instruction 00000039
            Unknown PowerPC instruction 0000003A
            Unknown PowerPC instruction 0000003B
            Unknown PowerPC instruction 0000003C
            Unknown PowerPC instruction 0000003D
            Unknown PowerPC instruction 0000003E
            Unknown PowerPC instruction 0000003F
            Unknown PowerPC instruction 00000040
            Unknown PowerPC instruction 00000041
            Unknown PowerPC instruction 00000042
            Unknown PowerPC instruction 00000043
            Unknown PowerPC instruction 00000044
            Unknown PowerPC instruction 00000045
            Unknown PowerPC instruction 00000046
            Unknown PowerPC instruction 00000047
            Unknown PowerPC instruction 00000048
            Unknown PowerPC instruction 00000049
            Unknown PowerPC instruction 0000004A
            Unknown PowerPC instruction 0000004B
            Unknown PowerPC instruction 0000004C
            Unknown PowerPC instruction 0000004D
            Unknown PowerPC instruction 0000004E
            Unknown PowerPC instruction 0000004F
            Unknown PowerPC instruction 00000050
            Unknown PowerPC instruction 00000051
            Unknown PowerPC instruction 00000052
            Unknown PowerPC instruction 00000053
            Unknown PowerPC instruction 00000054
            Unknown PowerPC instruction 00000055
            Unknown PowerPC instruction 00000056
            Unknown PowerPC instruction 00000057
            Unknown PowerPC instruction 00000058
            Unknown PowerPC instruction 00000059
            Unknown PowerPC instruction 0000005A
            Unknown PowerPC instruction 0000005B
            Unknown PowerPC instruction 0000005C
            Unknown PowerPC instruction 0000005D
            Unknown PowerPC instruction 0000005E
            Unknown PowerPC instruction 0000005F
            Unknown PowerPC instruction 00000060
            Unknown PowerPC instruction 00000061
            Unknown PowerPC instruction 00000062
            Unknown PowerPC instruction 00000063
            Unknown PowerPC instruction 00000064
            Unknown PowerPC instruction 00000065
            Unknown PowerPC instruction 00000066
            Unknown PowerPC instruction 00000067
            Unknown PowerPC instruction 00000068
            Unknown PowerPC instruction 00000069
            Unknown PowerPC instruction 0000006B
            Unknown PowerPC instruction 0000006C
            Unknown PowerPC instruction 0000006D
            Unknown PowerPC instruction 00000070
            Unknown PowerPC instruction 00000072
            Unknown PowerPC instruction 0000007F
            Unknown PowerPC instruction 00000080
            Unknown PowerPC instruction 00000081
            Unknown PowerPC instruction 00000082
            Unknown PowerPC instruction 00000083
            Unknown PowerPC instruction 00000084
            Unknown PowerPC instruction 00000087
            Unknown PowerPC instruction 00000091
            Unknown PowerPC instruction 0000009E
            Unknown PowerPC instruction 000000A1
            Unknown PowerPC instruction 000000A4
            Unknown PowerPC instruction 000000A7
            Unknown PowerPC instruction 000000B7
            Unknown PowerPC instruction 000000C0
            Unknown PowerPC instruction 000000CE
            Unknown PowerPC instruction 000000D7
            Unknown PowerPC instruction 000000F0
            Unknown PowerPC instruction 000000FF
            Unknown PowerPC instruction 00000100
            Unknown PowerPC instruction 00000190
            Unknown PowerPC instruction 00000191
            Unknown PowerPC instruction 000001A4
            Unknown PowerPC instruction 000001A9
            Unknown PowerPC instruction 000001C0
            Unknown PowerPC instruction 000001C4
            Unknown PowerPC instruction 000001C5
            Unknown PowerPC instruction 000001CC
            Unknown PowerPC instruction 000001CD
            Unknown PowerPC instruction 000001CE
            Unknown PowerPC instruction 000001CF
            Unknown PowerPC instruction 000001D7
            Unknown PowerPC instruction 000001D9
            Unknown PowerPC instruction 000001E0
            Unknown PowerPC instruction 000001FC
            Unknown PowerPC instruction 000001FD
            Unknown PowerPC instruction 000001FE
            Unknown PowerPC instruction 000001FF
            Unknown PowerPC instruction 00000200
            Unknown PowerPC instruction 00000204
            Unknown PowerPC instruction 00000280
            Unknown PowerPC instruction 000002DC
            Unknown PowerPC instruction 00000300
            Unknown PowerPC instruction 000003CB
            Unknown PowerPC instruction 000003CD
            Unknown PowerPC instruction 000003D1
            Unknown PowerPC instruction 000003F8
            Unknown PowerPC instruction 000003FF
            Unknown PowerPC instruction 00000400
            Unknown PowerPC instruction 00000404
            Unknown PowerPC instruction 00000409
            Unknown PowerPC instruction 0000044F
            Unknown PowerPC instruction 0000047F
            Unknown PowerPC instruction 000005C8
            Unknown PowerPC instruction 000005F8
            Unknown PowerPC instruction 000006F8
            Unknown PowerPC instruction 00000718
            Unknown PowerPC instruction 00000778
            Unknown PowerPC instruction 000007B8
            Unknown PowerPC instruction 000007D8
            Unknown PowerPC instruction 000007E8
            Unknown PowerPC instruction 000007F0
            Unknown PowerPC instruction 00000800
            Unknown PowerPC instruction 00000A00
            Unknown PowerPC instruction 00000A31
            Unknown PowerPC instruction 00000C38
            Unknown PowerPC instruction 00000C39
            Unknown PowerPC instruction 00000C3C
            Unknown PowerPC instruction 00000C80
            Unknown PowerPC instruction 00000C81
            Unknown PowerPC instruction 00000C82
            Unknown PowerPC instruction 00000C83
            Unknown PowerPC instruction 00000C84
            Unknown PowerPC instruction 00000C94
            Unknown PowerPC instruction 00000CA4
            Unknown PowerPC instruction 00000E00
            Unknown PowerPC instruction 00000E18
            Unknown PowerPC instruction 00000E1C
            Unknown PowerPC instruction 00000E25
            Unknown PowerPC instruction 00000E2E
            Unknown PowerPC instruction 00000E37
            Unknown PowerPC instruction 00000E41
            Unknown PowerPC instruction 00000F01
            Unknown PowerPC instruction 00000F26
            Unknown PowerPC instruction 00000FFC
            Unknown PowerPC instruction 00001000
            Unknown PowerPC instruction 00001003
            Unknown PowerPC instruction 00001004
            Unknown PowerPC instruction 00001005
            Unknown PowerPC instruction 00001006
            Unknown PowerPC instruction 00001010
            Unknown PowerPC instruction 00001200
            Unknown PowerPC instruction 00001388
            Unknown PowerPC instruction 00001800
            Unknown PowerPC instruction 00002000
            Unknown PowerPC instruction 00002001
            Unknown PowerPC instruction 00002007
            Unknown PowerPC instruction 0000200E
            Unknown PowerPC instruction 00002020
            Unknown PowerPC instruction 00002200
            Unknown PowerPC instruction 00002201
            Unknown PowerPC instruction 00002203
            Unknown PowerPC instruction 00002206
            Unknown PowerPC instruction 00002208
            Unknown PowerPC instruction 00002222
            Unknown PowerPC instruction 00002301
            Unknown PowerPC instruction 00002309
            Unknown PowerPC instruction 00002312
            Unknown PowerPC instruction 00002318
            Unknown PowerPC instruction 00002319
            Unknown PowerPC instruction 0000231A
            Unknown PowerPC instruction 0000231B
            Unknown PowerPC instruction 00002694
            Unknown PowerPC instruction 00002709
            Unknown PowerPC instruction 00002B09
            Unknown PowerPC instruction 00002EE0
            Unknown PowerPC instruction 00002FFD
            Unknown PowerPC instruction 00003333
            Unknown PowerPC instruction 00004000
            Unknown PowerPC instruction 00006009
            Unknown PowerPC instruction 00006015
            Unknown PowerPC instruction 00008000
            Unknown PowerPC instruction 00009873
            Unknown PowerPC instruction 00009875
            Unknown PowerPC instruction 0000E00F
            Unknown PowerPC instruction 0000FFFF
            Unknown PowerPC instruction 00010000
            Unknown PowerPC instruction 00010001
            Unknown PowerPC instruction 00010002
            Unknown PowerPC instruction 0001004D
            Unknown PowerPC instruction 0001007B
            Unknown PowerPC instruction 00010083
            Unknown PowerPC instruction 00010089
            Unknown PowerPC instruction 000100BD
            Unknown PowerPC instruction 00010102
            Unknown PowerPC instruction 0001013A
            Unknown PowerPC instruction 000101BD
            Unknown PowerPC instruction 000101C7
            Unknown PowerPC instruction 0001025B
            Unknown PowerPC instruction 00010A2F
            Unknown PowerPC instruction 00011002
            Unknown PowerPC instruction 00012180
            Unknown PowerPC instruction 00018006
            Unknown PowerPC instruction 0001F400
            Unknown PowerPC instruction 00020000
            Unknown PowerPC instruction 00020001
            Unknown PowerPC instruction 00020003
            Unknown PowerPC instruction 00020004
            Unknown PowerPC instruction 00020005
            Unknown PowerPC instruction 00020006
            Unknown PowerPC instruction 00020007
            Unknown PowerPC instruction 00020008
            Unknown PowerPC instruction 00020009
            Unknown PowerPC instruction 0002000A
            Unknown PowerPC instruction 0002000B
            Unknown PowerPC instruction 0002000C
            Unknown PowerPC instruction 0002000D
            Unknown PowerPC instruction 0002000E
            Unknown PowerPC instruction 0002000F
            Unknown PowerPC instruction 00020010
            Unknown PowerPC instruction 00020011
            Unknown PowerPC instruction 00020012
            Unknown PowerPC instruction 00022080
            Unknown PowerPC instruction 00022100
            Unknown PowerPC instruction 00022204
            Unknown PowerPC instruction 00030088
            Unknown PowerPC instruction 00032388
            Unknown PowerPC instruction 0003238C
            Unknown PowerPC instruction 00032390
            Unknown PowerPC instruction 00032394
            Unknown PowerPC instruction 00032398
            Unknown PowerPC instruction 0003239C
            Unknown PowerPC instruction 0003E01F
            Unknown PowerPC instruction 00040000
            Unknown PowerPC instruction 00040086
            Unknown PowerPC instruction 00042180
            Unknown PowerPC instruction 0005210F
            Unknown PowerPC instruction 00054800
            Unknown PowerPC instruction 000548BA
            Unknown PowerPC instruction 00072380
            Unknown PowerPC instruction 00080000
            Unknown PowerPC instruction 00080008
            Unknown PowerPC instruction 00080010
            Unknown PowerPC instruction 00080020
            Unknown PowerPC instruction 00090000
            Unknown PowerPC instruction 000A61FF
            Unknown PowerPC instruction 000B2200
            Unknown PowerPC instruction 000C000D
            Unknown PowerPC instruction 000F2000
            Unknown PowerPC instruction 000F4000
            Unknown PowerPC instruction 000F4400
            Unknown PowerPC instruction 000FF000
            Unknown PowerPC instruction 000FF100
            Unknown PowerPC instruction 00100000
            Unknown PowerPC instruction 00130001
            Unknown PowerPC instruction 00130002
            Unknown PowerPC instruction 00130003
            Unknown PowerPC instruction 00130004
            Unknown PowerPC instruction 00130005
            Unknown PowerPC instruction 00130006
            Unknown PowerPC instruction 00142100
            Unknown PowerPC instruction 00142280
            Unknown PowerPC instruction 00166C00
            Unknown PowerPC instruction 001A1A6C
            Unknown PowerPC instruction 00200000
            Unknown PowerPC instruction 00200020
            Unknown PowerPC instruction 00252300
            Unknown PowerPC instruction 00253B08
            Unknown PowerPC instruction 00253B48
            Unknown PowerPC instruction 00274900
            Unknown PowerPC instruction 002E0000
            Unknown PowerPC instruction 00300000
            Unknown PowerPC instruction 00400000
            Unknown PowerPC instruction 00414243
            Unknown PowerPC instruction 00616263
            Unknown PowerPC instruction 006C1AC6
            Unknown PowerPC instruction 006D1A6C
            Unknown PowerPC instruction 006D6D6C
            Unknown PowerPC instruction 006D6E6C
            Unknown PowerPC instruction 00700730
            Unknown PowerPC instruction 00800050
            Unknown PowerPC instruction 00AABC00
            Unknown PowerPC instruction 00AC00B1
            Unknown PowerPC instruction 00B00000
            Unknown PowerPC instruction 00B00080
            Unknown PowerPC instruction 00B01A6C
            Unknown PowerPC instruction 00B06C00
            Unknown PowerPC instruction 00B06CC6
            Unknown PowerPC instruction 00B06D6C
            Unknown PowerPC instruction 00B0B000
            Unknown PowerPC instruction 00B0B26C
            Unknown PowerPC instruction 00B0C600
            Unknown PowerPC instruction 00BC6CB1
            Unknown PowerPC instruction 00BF4800
            Unknown PowerPC instruction 00C00100
            Unknown PowerPC instruction 00C71A6C
            Unknown PowerPC instruction 00FFFF00
            Unknown PowerPC instruction 00FFFFFF
            Unknown PowerPC instruction 01000000
            Unknown PowerPC instruction 01000101
            Unknown PowerPC instruction 01000190
            Unknown PowerPC instruction 01000191
            Unknown PowerPC instruction 010001A4
            Unknown PowerPC instruction 01000202
            Unknown PowerPC instruction 01000300
            Unknown PowerPC instruction 01000302
            Unknown PowerPC instruction 010003D1
            Unknown PowerPC instruction 01000400
            Unknown PowerPC instruction 01000404
            Unknown PowerPC instruction 01000C14
            Unknown PowerPC instruction 01010000
            Unknown PowerPC instruction 01010001
            Unknown PowerPC instruction 01010003
            Unknown PowerPC instruction 0101000D
            Unknown PowerPC instruction 01010010
            Unknown PowerPC instruction 01010015
            Unknown PowerPC instruction 01010019
            Unknown PowerPC instruction 01010028
            Unknown PowerPC instruction 01010052
            Unknown PowerPC instruction 01010053
            Unknown PowerPC instruction 0101005A
            Unknown PowerPC instruction 0101005D
            Unknown PowerPC instruction 0101005F
            Unknown PowerPC instruction 0101006B
            Unknown PowerPC instruction 0101006C
            Unknown PowerPC instruction 0101006F
            Unknown PowerPC instruction 0101007D
            Unknown PowerPC instruction 0101008F
            Unknown PowerPC instruction 01010097
            Unknown PowerPC instruction 01010099
            Unknown PowerPC instruction 0101009A
            Unknown PowerPC instruction 010100B0
            Unknown PowerPC instruction 010100BA
            Unknown PowerPC instruction 010100CC
            Unknown PowerPC instruction 010100CF
            Unknown PowerPC instruction 010100D1
            Unknown PowerPC instruction 010100D2
            Unknown PowerPC instruction 010100DA
            Unknown PowerPC instruction 010100DB
            Unknown PowerPC instruction 010100DC
            Unknown PowerPC instruction 010100DF
            Unknown PowerPC instruction 010100E4
            Unknown PowerPC instruction 010100E7
            Unknown PowerPC instruction 010100E8
            Unknown PowerPC instruction 010100EE
            Unknown PowerPC instruction 010100EF
            Unknown PowerPC instruction 010100F0
            Unknown PowerPC instruction 010100F1
            Unknown PowerPC instruction 010100F7
            Unknown PowerPC instruction 010100FD
            Unknown PowerPC instruction 010100FF
            Unknown PowerPC instruction 01010101
            Unknown PowerPC instruction 01010102
            Unknown PowerPC instruction 01010103
            Unknown PowerPC instruction 01010104
            Unknown PowerPC instruction 01010105
            Unknown PowerPC instruction 01010110
            Unknown PowerPC instruction 0101011B
            Unknown PowerPC instruction 01010126
            Unknown PowerPC instruction 01010127
            Unknown PowerPC instruction 0101012B
            Unknown PowerPC instruction 0101012C
            Unknown PowerPC instruction 0101012D
            Unknown PowerPC instruction 0101012F
            Unknown PowerPC instruction 01010133
            Unknown PowerPC instruction 01010135
            Unknown PowerPC instruction 01010136
            Unknown PowerPC instruction 01010142
            Unknown PowerPC instruction 01010143
            Unknown PowerPC instruction 01010147
            Unknown PowerPC instruction 0101014D
            Unknown PowerPC instruction 01010152
            Unknown PowerPC instruction 01010153
            Unknown PowerPC instruction 01010154
            Unknown PowerPC instruction 01010155
            Unknown PowerPC instruction 01010194
            Unknown PowerPC instruction 01010197
            Unknown PowerPC instruction 01010199
            Unknown PowerPC instruction 010101A1
            Unknown PowerPC instruction 010101A5
            Unknown PowerPC instruction 010101B1
            Unknown PowerPC instruction 010101BA
            Unknown PowerPC instruction 010101C2
            Unknown PowerPC instruction 010101C5
            Unknown PowerPC instruction 010101C6
            Unknown PowerPC instruction 010101C9
            Unknown PowerPC instruction 010101CA
            Unknown PowerPC instruction 010101D3
            Unknown PowerPC instruction 010101D4
            Unknown PowerPC instruction 010101D5
            Unknown PowerPC instruction 010101DC
            Unknown PowerPC instruction 01010269
            Unknown PowerPC instruction 0101026A
            Unknown PowerPC instruction 01020000
            Unknown PowerPC instruction 01020203
            Unknown PowerPC instruction 01800000
            Unknown PowerPC instruction 01E00280
            Unknown PowerPC instruction 02000000
            Unknown PowerPC instruction 02000001
            Unknown PowerPC instruction 02000190
            Unknown PowerPC instruction 02000191
            Unknown PowerPC instruction 020001A4
            Unknown PowerPC instruction 020003D1
            Unknown PowerPC instruction 02010000
            Unknown PowerPC instruction 02010001
            Unknown PowerPC instruction 02010003
            Unknown PowerPC instruction 0201000D
            Unknown PowerPC instruction 02010010
            Unknown PowerPC instruction 02010015
            Unknown PowerPC instruction 02010019
            Unknown PowerPC instruction 02010028
            Unknown PowerPC instruction 02010052
            Unknown PowerPC instruction 02010053
            Unknown PowerPC instruction 0201005A
            Unknown PowerPC instruction 0201005D
            Unknown PowerPC instruction 0201005F
            Unknown PowerPC instruction 0201006B
            Unknown PowerPC instruction 0201006C
            Unknown PowerPC instruction 0201006F
            Unknown PowerPC instruction 0201007D
            Unknown PowerPC instruction 0201008F
            Unknown PowerPC instruction 02010097
            Unknown PowerPC instruction 02010099
            Unknown PowerPC instruction 0201009A
            Unknown PowerPC instruction 020100B0
            Unknown PowerPC instruction 020100BA
            Unknown PowerPC instruction 020100CC
            Unknown PowerPC instruction 020100CF
            Unknown PowerPC instruction 020100D1
            Unknown PowerPC instruction 020100D2
            Unknown PowerPC instruction 020100DA
            Unknown PowerPC instruction 020100DB
            Unknown PowerPC instruction 020100DC
            Unknown PowerPC instruction 020100DF
            Unknown PowerPC instruction 020100E4
            Unknown PowerPC instruction 020100E7
            Unknown PowerPC instruction 020100E8
            Unknown PowerPC instruction 020100EE
            Unknown PowerPC instruction 020100EF
            Unknown PowerPC instruction 020100F0
            Unknown PowerPC instruction 020100F1
            Unknown PowerPC instruction 020100F7
            Unknown PowerPC instruction 020100FD
            Unknown PowerPC instruction 020100FF
            Unknown PowerPC instruction 02010103
            Unknown PowerPC instruction 02010104
            Unknown PowerPC instruction 02010105
            Unknown PowerPC instruction 02010110
            Unknown PowerPC instruction 0201011B
            Unknown PowerPC instruction 02010126
            Unknown PowerPC instruction 02010127
            Unknown PowerPC instruction 0201012B
            Unknown PowerPC instruction 0201012C
            Unknown PowerPC instruction 0201012D
            Unknown PowerPC instruction 0201012F
            Unknown PowerPC instruction 02010133
            Unknown PowerPC instruction 02010135
            Unknown PowerPC instruction 02010136
            Unknown PowerPC instruction 02010142
            Unknown PowerPC instruction 02010143
            Unknown PowerPC instruction 02010147
            Unknown PowerPC instruction 0201014D
            Unknown PowerPC instruction 02010152
            Unknown PowerPC instruction 02010153
            Unknown PowerPC instruction 02010154
            Unknown PowerPC instruction 02010155
            Unknown PowerPC instruction 02010194
            Unknown PowerPC instruction 02010197
            Unknown PowerPC instruction 02010199
            Unknown PowerPC instruction 020101A1
            Unknown PowerPC instruction 020101A5
            Unknown PowerPC instruction 020101B1
            Unknown PowerPC instruction 020101BA
            Unknown PowerPC instruction 020101C2
            Unknown PowerPC instruction 020101C5
            Unknown PowerPC instruction 020101C6
            Unknown PowerPC instruction 020101C9
            Unknown PowerPC instruction 020101CA
            Unknown PowerPC instruction 020101D3
            Unknown PowerPC instruction 020101D4
            Unknown PowerPC instruction 020101D5
            Unknown PowerPC instruction 020101DC
            Unknown PowerPC instruction 02010201
            Unknown PowerPC instruction 02010269
            Unknown PowerPC instruction 0201026A
            Unknown PowerPC instruction 02020000
            Unknown PowerPC instruction 02020101
            Unknown PowerPC instruction 02020201
            Unknown PowerPC instruction 02020202
            Unknown PowerPC instruction 02030201
            Unknown PowerPC instruction 02030304
            Unknown PowerPC instruction 03000000
            Unknown PowerPC instruction 03000100
            Unknown PowerPC instruction 03010000
            Unknown PowerPC instruction 03030302
            Unknown PowerPC instruction 04000000
            Unknown PowerPC instruction 04000002
            Unknown PowerPC instruction 04000010
            Unknown PowerPC instruction 04001000
            Unknown PowerPC instruction 04010002
            Unknown PowerPC instruction 04040404
            Unknown PowerPC instruction 04B06C00
            Unknown PowerPC instruction 04C56C00
            Unknown PowerPC instruction 04C8D2CE
            Unknown PowerPC instruction 05000000
            Unknown PowerPC instruction 05010000
            Unknown PowerPC instruction 05030000
            Unknown PowerPC instruction 05F80000
            Unknown PowerPC instruction 06000000
            Unknown PowerPC instruction 06600640
            Unknown PowerPC instruction 07000000
            Unknown PowerPC instruction 07020000
            Unknown PowerPC instruction 08000000
            Unknown PowerPC instruction 08010000
            Unknown PowerPC instruction 08030000
            Unknown PowerPC instruction 08100100
            Unknown PowerPC instruction 085888DC
            Unknown PowerPC instruction 08A7FFFF
            Unknown PowerPC instruction 08A9FFFF
            Unknown PowerPC instruction 08AAFFFF
            Unknown PowerPC instruction 08ABFFFF
            Unknown PowerPC instruction 08C40000
            Unknown PowerPC instruction 08C80000
            Unknown PowerPC instruction 08CA0000
            Unknown PowerPC instruction 09000000
            Unknown PowerPC instruction 09010000
            Unknown PowerPC instruction 0A000000
            Unknown PowerPC instruction 0A000280
            Unknown PowerPC instruction 0A010000
            Unknown PowerPC instruction 0A3D70A3
            Unknown PowerPC instruction 0AD8A6DD
            Unknown PowerPC instruction 0B000000
            Unknown PowerPC instruction 0B040000
            Unknown PowerPC instruction 0B0A0B0A
            Unknown PowerPC instruction 0BCA0000
            Unknown PowerPC instruction 0BCC0000
            Unknown PowerPC instruction 24000000
            Unknown PowerPC instruction 25000000
            Unknown PowerPC instruction 256BCEAE
            Unknown PowerPC instruction 25FD5DD0
            Unknown PowerPC instruction 26000000
            Unknown PowerPC instruction 58480000
            Unknown PowerPC instruction 58595A00
            Unknown PowerPC instruction 5A88E79E
            Unknown PowerPC instruction 5A8BFDD1
            Unknown PowerPC instruction 5A929E8B
            Unknown PowerPC instruction E4A1AC7D
            Unknown PowerPC instruction F78D3927

            Unknown PowerPC VX instruction 1000001A 04-01A (26)
            Unknown PowerPC VX instruction 10011003 04-003 (3)
            Unknown PowerPC VX instruction 10030001 04-001 (1)
            Unknown PowerPC VX instruction 10101010 04-010 (16)
            Unknown PowerPC VX instruction 111110B0 04-0B0 (176)
            Unknown PowerPC VX instruction 136A2987 04-187 (391)
            Unknown PowerPC VX instruction 136A3987 04-187 (391)
            Unknown PowerPC VX instruction 138A2187 04-187 (391)
            Unknown PowerPC VX instruction 138A3187 04-187 (391)
            Unknown PowerPC VX instruction 13A05187 04-187 (391)
            Unknown PowerPC VX instruction 13C05987 04-187 (391)
            Unknown PowerPC VX instruction 13CA1987 04-187 (391)
            Unknown PowerPC VX instruction 13D29A35 04-235 (565)
            Unknown PowerPC VX instruction 13E05987 04-187 (391)
            Unknown PowerPC VX instruction 13E95187 04-187 (391)
            Unknown PowerPC VX instruction 13FFF935 04-135 (309)

            Unknown PowerPC VMX instruction 14000825 05-000 (0)
            Unknown PowerPC VMX instruction 1400E851 05-005 (5)
            Unknown PowerPC VMX instruction 14020100 05-010 (16)
            Unknown PowerPC VMX instruction 177B011C 05-011 (17)
            Unknown PowerPC VMX instruction 173FE1B5 05-019 (25)
            Unknown PowerPC VMX instruction 16D6BA35 05-021 (33)
            Unknown PowerPC VMX instruction 15BAAA71 05-025 (37)

            Unknown PowerPC VMX instruction 18000000 06-000 (0)
            Unknown PowerPC VMX instruction 187EF823 06-002 (2)
            Unknown PowerPC VMX instruction 18F7E121 06-012 (18)
            Unknown PowerPC VMX instruction 18280186 06-018 (24)
            Unknown PowerPC VMX instruction 195CB9F1 06-01F (31)
            Unknown PowerPC VMX instruction 1925FA11 06-021 (33)
            Unknown PowerPC VMX instruction 1AC0FA35 06-023 (35)
            Unknown PowerPC VMX instruction 18F4D251 06-025 (37)
            Unknown PowerPC VMX instruction 180EB291 06-029 (41)
            Unknown PowerPC VMX instruction 1BDEE2A5 06-02A (42)
            Unknown PowerPC VMX instruction 1801F2B1 06-02B (43)
            Unknown PowerPC VMX instruction 1803FAD1 06-02D (45)
            Unknown PowerPC VMX instruction 1BFFF2E5 06-02E (46)
            Unknown PowerPC VMX instruction 1927FB11 06-031 (49)
            Unknown PowerPC VMX instruction 1BA00334 06-033 (51)
            Unknown PowerPC VMX instruction 181AF353 06-035 (53)
            Unknown PowerPC VMX instruction 1BC0DB75 06-037 (55)
            Unknown PowerPC VMX instruction 18069391 06-039 (57)
            Unknown PowerPC VMX instruction 18ADA3D1 06-03D (61)
            Unknown PowerPC VMX instruction 18A0DBF1 06-03F (63)
            Unknown PowerPC VMX instruction 1BCECCED 06-04E (78)
            Unknown PowerPC VMX instruction 1A013E1A 06-061 (97)
            Unknown PowerPC VMX instruction 1800F631 06-063 (99)
            Unknown PowerPC VMX instruction 19000640 06-064 (100)
            Unknown PowerPC VMX instruction 1800F671 06-067 (103)
            Unknown PowerPC VMX instruction 1BA0EEB5 06-06B (107)
            Unknown PowerPC VMX instruction 1BEDFED7 06-06D (109)
            Unknown PowerPC VMX instruction 1AA0EEF5 06-06F (111)

            Unknown PowerPC X instruction 4C4D4E4F 13-327 (807)
            Unknown PowerPC X instruction 4D48C976 13-0BB (187)
            Unknown PowerPC X instruction 4F8E1AE5 13-172 (370)
            Unknown PowerPC X instruction 7C53B17E 1F-0BF (191)
            Unknown PowerPC X instruction 7DC2DEC0 1F-360 (864)
            Unknown PowerPC X instruction 7F7F7F7F 1F-3BF (959)
            Unknown PowerPC X instruction 7FEFFFFF 1F-3FF (1023)
            Unknown PowerPC XX3 instruction F0A65DFF 3C-BBF (3007)
            Unknown PowerPC XX3 instruction F3D4A7EB 3C-14FD (5373)
            Unknown PowerPC XX3 instruction F3F895AA 3C-12B5 (4789)
             */
        }
    }
}
