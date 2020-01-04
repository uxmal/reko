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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public partial class PowerPcDisassemblerTests : DisassemblerTestBase<PowerPcInstruction>
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

        private void AssertCode(uint instr, string sExp)
        {
            var i = DisassembleWord(instr);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void Given_PowerPcBe64()
        {
            this.arch = new PowerPcBe64Architecture("ppc-be-32");
        }

        private void Given_ProcessorModel_750()
        {
            this.arch.LoadUserOptions(new Dictionary<string, object>
            {
                { "Model", "750" }
            });
        }

        [SetUp]
        public void Setup()
        {
            this.arch = new PowerPcBe32Architecture("ppc-be-32");
        }

   

        [Test]
        public void PPCDis_IllegalOpcode()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 00, 00, 00, 00 });
            Assert.AreEqual(Mnemonic.illegal, instr.Mnemonic);
            Assert.IsNotNull(instr.Operands);
        }

        [Test]
        public void PPCDis_Ori()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 0x60, 0x1F, 0x44, 0x44 });
            Assert.AreEqual(Mnemonic.ori, instr.Mnemonic);
            Assert.AreEqual(3, instr.Operands.Length);
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
        public void PPCDis_bcctrne()
        {
            //$TODO: prefer bcctrne
            AssertCode(0x4C820420, "bcctr\t04,02");
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
            AssertCode(0x101f038c, "vspltisw\tv0,-00000001");
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
            Given_PowerPcBe64();
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
            AssertCode(0x1AC0FA35, "vcfpsxws128\tv54,v63,+00000000"); // 06 - 023(35)
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
            AssertCode(0x1B600774, "vspltisw128\tv59,v0,+00000000");  // 06 - 077(119)
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
    }
}