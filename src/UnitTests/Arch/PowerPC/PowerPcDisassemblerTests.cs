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
using Reko.Core.Machine;
using Reko.Core.Memory;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public partial class PowerPcDisassemblerTests : DisassemblerTestBase<PowerPcInstruction>
    {
        private PowerPcArchitecture arch;
        private Address addrLoad;

        [SetUp]
        public void Setup()
        {
            this.arch = new PowerPcBe32Architecture(new ServiceContainer(), "ppc-be-32", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private PowerPcInstruction DisassembleX(uint op, uint rs, uint ra, uint rb, uint xo, uint rc)
        {
            uint w =
                (op << 26) |
                (rs << 21) |
                (ra << 16) |
                (rb << 11) |
                (xo << 1) |
                rc;
            ByteMemoryArea mem = new ByteMemoryArea(Address.Ptr32(0x00100000), new byte[4]);
            mem.WriteBeUInt32(0, w);
            return Disassemble(mem);
        }

        private void RunTest(string expected, uint uInstr)
        {
            var instr = DisassembleWord(uInstr);
            Assert.AreEqual(expected, instr.ToString());
        }

        private void AssertCode(uint instr, string sExp)
        {
            var i = DisassembleWord(instr);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);

            if (sExp != i.ToString()) // && i.Mnemonic == Mnemonic.nyi)
            {
                Assert.AreEqual(sExp, i.ToString());
            }
        }

        private void Given_PowerPcBe64()
        {
            this.arch = new PowerPcBe64Architecture(new ServiceContainer(), "ppc-be-64", new Dictionary<string, object>());
        }

        private void Given_ProcessorModel_750cl()
        {
            this.arch.LoadUserOptions(new Dictionary<string, object>
            {
                { ProcessorOption.Model, "750cl" }
            });
        }

        [Test]
        public void PPCDis_IllegalOpcode()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 00, 00, 00, 00 });
            Assert.AreEqual(Mnemonic.illegal, instr.Mnemonic);
            Assert.IsNotNull(instr.Operands);
        }

        [Test]
        public void PPCDis_add()
        {
            var instr = DisassembleWord(0x7c9a2214);
            Assert.AreEqual("add\tr4,r26,r4", instr.ToString());
        }

        [Test]
        public void PPCDis_addic()
        {
            var instr = DisassembleWord(0b001100_00010_00001_1111111111111000);
            Assert.AreEqual("addic\tr2,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_addi()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 0x38, 0x1F, 0xFF, 0xFC });
            Assert.AreEqual("addi\tr0,r31,-0004", instr.ToString());
        }

        [Test]
        public void PPCDis_addo()
        {
            var instr = DisassembleWord(0x7C9AA614);
            Assert.AreEqual("addo\tr4,r26,r20", instr.ToString());
        }

        [Test]
        public void PPCDis_addze()
        {
            AssertCode(0x7c000194, "addze\tr0,r0");
        }

        [Test]
        public void PPCDis_andi_()
        {
            var instr = DisassembleWord(0b011100_00001_00011_1111110001100100);
            Assert.AreEqual("andi.\tr3,r1,FC64", instr.ToString());
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
        public void PPCDis_bctar()
        {
            AssertCode("bctar\t0A,13", "4D537460");
        }

        [Test]
        public void PPCDis_bctarl()
        {
            AssertCode("bctarl\t0A,13", "4D537461");
        }

        [Test]
        public void PPCDis_bcxx()
        {
            var instr = DisassembleWord(0x4000FFF0);
            Assert.AreEqual("bdnzf\tlt,$000FFFF0", instr.ToString());
        }

        [Test]
        public void PPCDis_beq_cr1()
        {
            AssertCode("beq\tcr1,$00100010", "41860010");
        }

        [Test]
        public void PPCDis_bl()
        {
            var instr = DisassembleWord(0x4BFFFFFD);
            Assert.AreEqual("bl\t$000FFFFC", instr.ToString());
        }

        [Test]
        public void PPCDis_bl_largeOffsets()
        {
            AssertCode("bl\t$FF7F42E0", "4B6F42E1");
            AssertCode("bl\t$010FFFE0", "48FFFFE1");
            AssertCode("bl\t$020FFFE0", "49FFFFE1");
            AssertCode("bl\t$FF0FFFE0", "4AFFFFE1");
            AssertCode("bl\t$000FFFE0", "4BFFFFE1");
        }

        [Test]
        public void PPCDis_bl_64()
        {
            Given_PowerPcBe64();
            addrLoad = Address.Ptr64(0x82BBE130);
            AssertCode("bl\t$00000000822C0890", "4B702761");
        }

        [Test]
        public void PPCDis_blr()
        {
            var instr = DisassembleWord(0x4e800020);
            Assert.AreEqual("blr", instr.ToString());
        }

        [Test]
        public void PPCDis_cbcdtd()
        {
            AssertCode("cbcdtd\tr28,r27", "7F7C8A75");
        }

        [Test]
        public void PPCDis_clrbhrb()
        {
            AssertCode("clrbhrb", "7CF7FB5C");
        }

        [Test]
        public void PPCDis_cmpb()
        {
            AssertCode("cmpb\tr3,r10,r26", "7D43D3F8");
        }

        [Test]
        public void PPCDis_cmplwi()
        {
            var instr = DisassembleWord(0b001010_00010_00001_1111111111111000);
            Assert.AreEqual("cmplwi\tcr0,r1,FFF8", instr.ToString());
        }

        [Test]
        public void PPCDis_cmpi()
        {
            var instr = DisassembleWord(0b001011_00010_00001_1111111111111000);
            Assert.AreEqual("cmpwi\tcr0,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_cntlzw()
        {
            AssertCode(0x7d4a0034, "cntlzw\tr10,r10");
        }

        [Test]
        public void PPCDis_cnttzw()
        {
            AssertCode("cnttzw.\tr7,r2", "7C47FC35");
        }

        [Test]
        public void PPCDis_copy()
        {
            AssertCode("copy\tr15,r8,00", "7D8F460D");
        }

        [Test]
        public void PPCDis_dadd()
        {
            AssertCode("dadd\tf31,f28,f27", "EFFCD804");
        }

        [Test]
        public void PPCDis_darn()
        {
            AssertCode("darn\tr25,03", "7F2B35E7");
        }

        [Test]
        public void PPCDis_dcmpu()
        {
            AssertCode("dcmpu\tcr3,f12,f0", "ED800505");
        }

        [Test]
        public void PPCDis_dctfixq()
        {
            AssertCode("dctfixq.\tf1,f8", "FC304245");
        }

        [Test]
        public void PPCDis_dctqpq()
        {
            AssertCode("dctqpq.\tf24,f4", "FF052205");
        }

        [Test]
        public void PPCDis_denbcd()
        {
            AssertCode("denbcd\t00,f0,f11", "EC085E84");
        }

        [Test]
        public void PPCDis_divde()
        {
            AssertCode("divde\tr29,r0,r19", "7FA09B52");
        }

        [Test]
        public void PPCDis_divweu()
        {
            AssertCode("divweu.\tr30,r11,r5", "7FCB2B17");
        }

        [Test]
        public void PPCDis_dmulq()
        {
            AssertCode("dmulq.\tf8,f0,f4", "FD002045");
        }

        [Test]
        public void PPCDis_dquaiq()
        {
            AssertCode("dquaiq\t1A,f31,f15,01", "FFFA7A86");
        }

        [Test]
        public void PPCDis_dquaq()
        {
            AssertCode("dquaq.\tf29,f3,f2,00", "FFA31007");
        }

        [Test]
        public void PPCDis_drintn()
        {
            AssertCode("drintn\t01,f29,f31,06", "EFBDFDC6");
        }

        [Test]
        public void PPCDis_drintxq()
        {
            AssertCode("drintxq\t01,f31,f24,03", "FFE1C6C6");
        }

        [Test]
        public void PPCDis_dscri()
        {
            AssertCode("dscri.\tf30,f31,10", "EFDF40C5");
        }

        [Test]
        public void PPCDis_dscriq()
        {
            AssertCode("dscriq.\tf6,f0,12", "FCC048C5");
        }

        [Test]
        public void PPCDis_dsub()
        {
            AssertCode("dsub\tf12,f0,f0", "ED800404");
        }

        [Test]
        public void PPCDis_dsubq()
        {
            AssertCode("dsubq\tf0,f0,f19", "FC009C04");
        }

        [Test]
        public void PPCDis_dtstdgq()
        {
            AssertCode("dtstdgq\t07,f31,3F", "FFFFFDC4");
        }

        [Test]
        public void PPCDis_dtstexq()
        {
            AssertCode("dtstexq\tcr7,f31,f0", "FFFF0144");
        }

        [Test]
        public void PPCDis_dtstsfiq()
        {
            AssertCode("dtstsfiq\tcr7,3F,f0", "FFFF0146");
        }

        [Test]
        public void PPCDis_extswsli()
        {
            AssertCode("extswsli.\tr20,r12,14", "7D94A6F5");
        }

        [Test]
        public void PPCDis_fadd()
        {
            var instr = DisassembleWord(0xFFF4402A);
            Assert.AreEqual("fadd\tf31,f20,f8", instr.ToString());
        }

        [Test]
        public void PPCDis_hrfid()
        {
            AssertCode("hrfid", "4C09A224");
        }

        [Test]
        public void PPCDis_icbt()
        {
            AssertCode("icbt\tr12,r3", "7CEC182D");
        }

        [Test]
        public void PPCDis_isel()
        {
            AssertCode("isel\tr12,r5,r12,1D", "7D85675F");
        }

        [Test]
        public void PPCDis_lbz()
        {
            var instr = DisassembleWord(0x88010203);
            Assert.AreEqual("lbz\tr0,515(r1)", instr.ToString());
        }

        [Test]
        public void PPCDis_lbzcix()
        {
            AssertCode("lbzcix\tr20,r31,r10", "7E9F56AB");
        }

        [Test]
        public void PPCDis_lbzu()
        {
            var instr = DisassembleWord(0x8C0A0000);
            Assert.AreEqual("lbzu\tr0,0(r10)", instr.ToString());
        }

        [Test]
        public void PPCDis_lbzx()
        {
            var instr = DisassembleWord(0x7F0408AE);
            Assert.AreEqual("lbzx\tr24,r4,r1", instr.ToString());
        }

        [Test]
        public void PPCDis_ldbrx()
        {
            AssertCode("ldbrx\tr10,r0,r3", "7D401C28");
        }

        [Test]
        public void PPCDis_ldcix()
        {
            AssertCode("ldcix\tr7,r0,r8", "7CE046EA");
        }

        [Test]
        public void PPCDis_ldmx()
        {
            AssertCode("ldmx\tr11,r26,r15", "7D7A7A6A");
        }

        [Test]
        public void PPCDis_lhzx()
        {
            AssertCode("lhzx\tr24,r4,r0", "7F04022E");
        }

        [Test]
        public void PPCDis_lwa()
        {
            this.Given_PowerPcBe64();
            AssertCode("lwa\tr5,-8(r11)", "E8ABFFFA");
        }

        [Test]
        public void PPCDis_lwarx()
        {
            var instr = DisassembleWord(0x7C720028);
            Assert.AreEqual("lwarx\tr3,r18,r0", instr.ToString());
        }

        [Test]
        public void PPCDis_lwz()
        {
            var instr = DisassembleWord(0x803F0005);
            Assert.AreEqual("lwz\tr1,5(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_lxsdx()
        {
            AssertCode("lxsdx\tv63,r0,r9", "7FE04C99");
        }

        [Test]
        public void PPCDis_lxsihzx()
        {
            AssertCode("lxsihzx\tv31,r26,r25", "7FFACE5A");
        }

        [Test]
        public void PPCDis_lxsiwzx()
        {
            AssertCode("lxsiwzx\tv51,r1,r18", "7E619019");
        }

        [Test]
        public void PPCDis_lxv()
        {
            AssertCode("lxv\tv12,-19120(r20)", "F4D4B551");
        }

        [Test]
        public void PPCDis_lxvb16x()
        {
            AssertCode("lxvb16x\tv12,r28,r14", "7D9C76D8");
        }

        [Test]
        public void PPCDis_lxvd2x()
        {
            AssertCode("lxvd2x\tv54,r22,r15", "7ED67E99");
        }

        [Test]
        public void PPCDis_lxvll()
        {
            AssertCode("lxvll\tv35,r27,r31", "7C7BFA5B");
        }

        [Test]
        public void PPCDis_mcrfs()
        {
            AssertCode("mcrfs\t07,07", "FFFEF080");
        }

        [Test]
        public void PPCDis_msgclr()
        {
            AssertCode("msgclr\tr13", "7C6F69DC");
        }

        [Test]
        public void PPCDis_msgclrp()
        {
            AssertCode("msgclrp\tr27", "7D17D95C");
        }

        [Test]
        public void PPCDis_msgsnd()
        {
            AssertCode("msgsnd\tr20", "7CEEA19D");
        }

        [Test]
        public void PPCDis_mtlr()
        {
            var instr = DisassembleWord(0x7C0803A6);
            Assert.AreEqual("mtlr\tr0", instr.ToString());
        }

        [Test]
        public void PPCDis_mtvsrd()
        {
            AssertCode("mtvsrd\tv24,r9", "7D890166");
        }


        [Test]
        public void PPCDis_mtvsrdd()
        {
            AssertCode("mtvsrdd\tv31,r28,r2", "7FFC1366");
        }

        [Test]
        public void PPCDis_mtvsrwa()
        {
            AssertCode("mtvsrwa\tv47,r3", "7DE3E9A7");
        }

        [Test]
        public void PPCDis_mulhd()
        {
            AssertCode("mulhd.\tr5,r30,r25", "7CBEC893");
        }

        [Test]
        public void PPCDis_mulhdu()
        {
            AssertCode("mulhdu.\tr21,r11,r14", "7EAB7413");
        }

        [Test]
        public void PPCDis_mulli()
        {
            var instr = DisassembleWord(0x1F1F0003);
            Assert.AreEqual("mulli\tr24,r31,+0003", instr.ToString());
        }

        [Test]
        public void PPCDis_nop()
        {
            AssertCode("nop", "60000000");
        }

        [Test]
        public void PPCDis_stb()
        {
            var instr = DisassembleWord(0x9A3FFE00);
            Assert.AreEqual("stb\tr17,-512(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stbcix()
        {
            AssertCode("stbcix\tr20,r21,r28", "7E95E7AA");
        }

        [Test]
        public void PPCDis_stbu()
        {
            var instr = DisassembleWord(0x9C01FFEE);
            Assert.AreEqual("stbu\tr0,-18(r1)", instr.ToString());
        }

        [Test]
        public void PPCDis_stbux()
        {
            var instr = DisassembleWord(0x7CAA01EE);
            Assert.AreEqual("stbux\tr5,r10,r0", instr.ToString());
        }

        [Test]
        public void PPCDis_stdat()
        {
            AssertCode("stdat\tr11,r3,and", "7D631DCC");
        }

        [Test]
        public void PPCDis_stdbrx()
        {
            AssertCode("stdbrx\tr3,r1,r24", "7C61C528");
        }

        [Test]
        public void PPCDis_stdcix()
        {
            AssertCode("stdcix\tr13,r3,r26", "7DA3D7EB");
        }

        [Test]
        public void PPCDis_stdux()
        {
            AssertCode("stdux\tr10,r1,r9", "7D41496A");
        }

        [Test]
        public void PPCDis_sthbrx()
        {
            AssertCode("sthbrx\tr10,r11,r3", "7D6A1F2C");
        }

        [Test]
        public void PPCDis_stop()
        {
            AssertCode("stop", "4F8E1AE5");
        }

        [Test]
        public void PPCDis_stqcx()
        {
            AssertCode("stqcx.\tr6,r0,r22", "7CC0B16D");
        }

        [Test]
        public void PPCDis_stxsiwx()
        {
            AssertCode("stxsiwx\tv7,r5,r18", "7CE59118");
        }

        [Test]
        public void PPCDis_stswx()
        {
            AssertCode("stswx\tr5,r0,r3", "7CA01D2A");
        }

        [Test]
        public void PPCDis_stw()
        {
            var instr = DisassembleWord(0x903FFFF8);
            Assert.AreEqual("stw\tr1,-8(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stwcix()
        {
            AssertCode("stwcix\tr11,r18,r5", "7D722F2A");
        }

        [Test]
        public void PPCDis_stwu()
        {
            var instr = DisassembleWord(0x943F0005);
            Assert.AreEqual("stwu\tr1,5(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stxsdx()
        {
            AssertCode("stxsdx\tv1,r24,r16", "7C388598");
        }

        [Test]
        public void PPCDis_stxv()
        {
            AssertCode("stxv\tf7,-2060(r22)", "F4F6F7F5");
        }

        [Test]
        public void PPCDis_stxvd2x()
        {
            AssertCode("stxvd2x\tv31,r20,r9", "7FF44F98");
        }

        [Test]
        public void PPCDis_xori()
        {
            var instr = DisassembleWord(0b011010_00001_00011_0101010101010101);
            Assert.AreEqual("xori\tr3,r1,5555", instr.ToString());
        }

        [Test]
        public void PPCDis_xoris()
        {
            var instr = DisassembleWord(0b011011_00001_00011_0101010101010101);
            Assert.AreEqual("xoris\tr3,r1,5555", instr.ToString());
        }

        [Test]
        public void PPCDis_xor_()
        {
            var instr = DisassembleWord(0b011111_00010_00001_00011_0100111100_1);
            Assert.AreEqual("xor.\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void PPCDis_lhz()
        {
            var instr = DisassembleWord(0b101000_00010_00001_1111111111111000);
            Assert.AreEqual("lhz\tr2,-8(r1)", instr.ToString());
        }

        [Test]
        public void PPCDis_subfic()
        {
            var instr = DisassembleWord(0b001000_00010_00001_1111111111111000);
            Assert.AreEqual("subfic\tr2,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_subfme()
        {
            AssertCode("subfme.\tr3,r25", "7C79E9D1");
        }

        [Test]
        public void PPCDis_trechkpt()
        {
            AssertCode("trechkpt.", "7EFEE7DC");
        }

        [Test]
        public void PPCDis_tsr()
        {
            AssertCode("tsr.\t00", "7D0BADDC");
        }

        [Test]
        public void PPCDis_twi()
        {
            var instr = DisassembleWord(0b000011_00010_00001_1111111111111000);
            Assert.AreEqual("twi\t02,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_addic_()
        {
            var instr = DisassembleWord(0b001101_00010_00001_1111111111111000);
            Assert.AreEqual("addic.\tr2,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_sc()
        {
            var instr = DisassembleWord(0b010001_00010_00000_0000000000000010);
            Assert.AreEqual("sc", instr.ToString());
        }

        [Test]
        public void PPCDis_setb()
        {
            AssertCode("setb\tr11,cr7", "7D7F8100");
        }

        [Test]
        public void PPCDis_crnor()
        {
            var instr = DisassembleWord(0b010011_00001_00010_00011_00001000010);
            Assert.AreEqual("crnor\t01,02,03", instr.ToString());
        }

        [Test]
        public void PPCDis_cror()
        {
            var instr = DisassembleWord(0b010011_00001_00010_00011_01110000010);
            Assert.AreEqual("cror\t01,02,03", instr.ToString());
        }

        [Test]
        public void PPCDis_or()
        {
            PowerPcInstruction instr = DisassembleX(31, 2, 1, 3, 444, 0);
            Assert.AreEqual("or\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void PPCDis_or_()
        {
            PowerPcInstruction instr = DisassembleX(31, 2, 1, 3, 444, 1);
            Assert.AreEqual("or.\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void PPCDis_ori()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 0x60, 0x1F, 0x44, 0x44 });
            Assert.AreEqual(Mnemonic.ori, instr.Mnemonic);
            Assert.AreEqual(3, instr.Operands.Length);
            Assert.AreEqual("ori\tr31,r0,4444", instr.ToString());
        }

        [Test]
        public void PPCDis_oris()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 0x64, 0x1F, 0x44, 0x44 });
            Assert.AreEqual("oris\tr31,r0,4444", instr.ToString());
        }

        [Test]
        public void PPCDis_popcntw()
        {
            AssertCode("popcntw\tr13,r23", "7EEDFAF5");
        }

        [Test]
        public void PPCDis_prtyd()
        {
            AssertCode("prtyd\tr24,r18", "7E58A974");
        }

        [Test]
        public void PPCDis_rfebb()
        {
            AssertCode("rfebb\t00", "4C969124");
        }

        [Test]
        public void PPCDis_rfi()
        {
            var instr = DisassembleWord(0b010011_00000_00000_00000_0_0000100100);
            Assert.AreEqual("rfid", instr.ToString());
        }

        [Test]
        public void PPCDis_andis_()
        {
            var instr = DisassembleWord(0b011101_00001_00011_1111110001100100);
            Assert.AreEqual("andis.\tr3,r1,FC64", instr.ToString());
        }

        [Test]
        public void PPCDis_cmp()
        {
            var instr = DisassembleWord(0b011111_01100_00001_00010_0000000000_0);
            Assert.AreEqual("cmp\tcr3,r1,r2", instr.ToString());
        }

        [Test]
        public void PPCDis_tabort()
        {
            AssertCode("tabort.\tr29", "7D1D671C");
        }

        [Test]
        public void PPCDis_tabortdc()
        {
            AssertCode("tabortdc.\t16,r29,r4", "7EDD265C");
        }

        [Test]
        public void PPCDis_tbegin()
        {
            AssertCode("tbegin.\t00", "7D8B751D");
        }

        [Test]
        public void PPCDis_tw()
        {
            var instr = DisassembleWord(0b011111_01100_00001_00010_0000000100_0);
            Assert.AreEqual("tw\t0C,r1,r2", instr.ToString());
        }

        [Test]
        public void PPCDis_lmw()
        {
            var instr = DisassembleWord(0b101110_00001_00010_111111111110100_0);
            Assert.AreEqual("lmw\tr1,-24(r2)", instr.ToString());
        }

        [Test]
        public void PPCDis_stmw()
        {
            var instr = DisassembleWord(0b101111_00001_00010_111111111110100_0);
            Assert.AreEqual("stmw\tr1,-24(r2)", instr.ToString());
        }

        [Test]
        public void PPCDis_lfs()
        {
            var instr = DisassembleWord(0b110000_00001_00010_111111111110100_0);
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
            RunTest("fdivs.\tf1,f2,f3", 0b111011_00001_00010_00011_00000_10010_1);
            RunTest("fsubs.\tf1,f2,f3", 0b111011_00001_00010_00011_00000_10100_1);
            RunTest("fadds.\tf1,f2,f3", 0b111011_00001_00010_00011_00000_10101_1);
            RunTest("fsqrts.\tf1,f3", 0b111011_00001_00010_00011_00000_10110_1);
            RunTest("fres.\tf1,f3", 0b111011_00001_00010_00011_00000_11000_1);
            RunTest("fmuls.\tf1,f2,f4", 0b111011_00001_00010_00000_00100_11001_1);
            RunTest("fmsubs.\tf1,f2,f4,f3", 0b111011_00001_00010_00011_00100_11100_1);
            RunTest("fmadds.\tf1,f2,f4,f3", 0b111011_00001_00010_00011_00100_11101_1);
            RunTest("fnmsubs.\tf1,f2,f3,f4", 0b111011_00001_00010_00011_00100_11110_1);
            RunTest("fnmadds.\tf1,f2,f3,f4", 0b111011_00001_00010_00011_00100_11111_1);
        }

        [Test]
        public void PPCDis_mfbhrbe()
        {
            AssertCode("mfbhrbe\tr11,4C", "7D62625C");
        }

        [Test]
        public void PPCDis_mflr()
        {
            AssertCode("mflr\tr0", "7C0802A6");
        }

        [Test]
        public void PPCDis_mfvsrld()
        {
            AssertCode("mfvsrld\tr30,v25", "7F3E1266");
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
        public void PPCDis_stxsd()
        {
            AssertCode("stxsd\tf6,4268(r20)", "F4D410AE");
        }

        [Test]
        public void PPCDis_stxsihx()
        {
            AssertCode("stxsihx\tv44,r12,r25", "7D8CCF5B");
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
        public void PPCDis_mtctr()
        {
            AssertCode(0x7d0903a6, "mtctr\tr8");
        }

        [Test]
        public void PPCDis_mtocrf()
        {
            AssertCode("mtocrf\t07,r7", "7CF1F921");
        }

        [Test]
        public void PPCDis_mtvsrws()
        {
            AssertCode("mtvsrws\tv43,r5", "7D652327");
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
        public void PPCDis_slbie()
        {
            AssertCode("slbie\tr1", "7EAD0B65");
        }

        [Test]
        public void PPCDis_slbieg()
        {
            AssertCode("slbieg\tr27,r15", "7F757BA4");
        }

        [Test]
        public void PPCDis_slbmfee()
        {
            AssertCode("slbmfee\tr3,r26", "7C6BD726");
        }

        [Test]
        public void PPCDis_slbmte()
        {
            AssertCode("slbmte\tr3,r16", "7C718324");
        }

        [Test]
        public void PPCDis_slbsync()
        {
            AssertCode("slbsync", "7F3882A5");
        }

        [Test]
        public void PPCDis_slw()
        {
            AssertCode("slw\tr0,r10,r0", "7d400030");
        }

        [Test]
        public void PPCDis_fcfidu()
        {
            AssertCode("fcfidu.\tf26,f19", "FF549F9D");
        }

        [Test]
        public void PPCDis_fcfidus()
        {
            AssertCode("fcfidus\tf30,f12", "EFC0679C");
        }

        [Test]
        public void PPCDis_fcmpu()
        {
            AssertCode(0xff810000, "fcmpu\tcr7,f1,f0");
        }

        [Test]
        public void PPCDis_fctiduz()
        {
            AssertCode("fctiduz\tf0,f31", "FC00FF5E");
        }

        [Test]
        public void PPCDis_fctiwz()
        {
            AssertCode(0xfc00081e, "fctiwz\tf0,f1");
        }

        [Test]
        public void PPCDis_fmr()
        {
            AssertCode(0xFFE00890, "fmr\tf31,f1");
        }

        [Test]
        public void PPCDis_fmul()
        {
            AssertCode(0xfc010032, "fmul\tf0,f1,f0");
        }

        [Test]
        public void PPCDis_frim()
        {
            AssertCode("frim.\tf18,f30", "FE50F3D1");
        }

        [Test]
        public void PPCDis_frsqrtes()
        {
            AssertCode("frsqrtes.\tf15,f8", "EDE24135");
        }

        [Test]
        public void PPCDis_mtcrf()
        {
            AssertCode(0x7d808120, "mtcrf\t08,r12");
        }


        [Test]
        public void PPCDis_bcctrl()
        {
            AssertCode(0x4e800421, "bcctrl\t14,00");
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
            AssertCode(0x7c8318f8, "nor\tr3,r4,r3");
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
            AssertCode(0x1020634a, "vcfsx\tv1,v12,00");
            AssertCode(0x118c0404, "vand\tv12,v12,v0");
            AssertCode(0x116c5080, "vadduwm\tv11,v12,v10");
            AssertCode(0x110c5404, "vand\tv8,v12,v10");
            AssertCode(0x1021ac44, "vandc\tv1,v1,v21");
            AssertCode(0x11083086, "vcmpequw\tv8,v8,v6");
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
        public void PPCDis_modsd()
        {
            AssertCode("modsd\tr12,r12,r17", "7D8C8E13");
        }

        [Test]
        public void PPCDis_moduw()
        {
            AssertCode("moduw\tr25,r4,r31", "7F24FA16");
        }

        [Test]
        public void PPCDis_More64()
        {
            AssertCode(0xfd600018, "frsp\tf11,f0");
            AssertCode(0xec1f07ba, "fmadds\tf0,f31,f30,f0");
            AssertCode(0xec216824, "fdivs\tf1,f1,f13");
            AssertCode(0x7c4048ce, "lvx\tv2,r0,r9");
            AssertCode(0x4d9e0020, "beqlr\tcr7");
            AssertCode(0x5c00c03e, "rlwnm\tr0,r0,r24,00,1F");
            AssertCode(0x4c9d0020, "blelr\tcr7");
            AssertCode(0x7c00222c, "dcbt\tr0,r4,00");
            AssertCode(0x7c0004ac, "sync");
            AssertCode(0x7c00f078, "andc\tr0,r0,r30");
            AssertCode(0x7c005836, "sld\tr0,r0,r11");
            AssertCode(0x7c0a31d2, "mulld\tr0,r10,r6");
            AssertCode(0x7c07492a, "stdx\tr0,r7,r9");
        }

        [Test]
        public void PPCDis_sradi()
        {
            AssertCode(0x7c0bfe76, "sradi\tr11,r0,+0000003F");
        }

        [Test]
        public void PPCDis_lswx()
        {
            AssertCode("lswx\tr5,r0,r4", "7CA0242A");
        }

        [Test]
        public void PPCDis_lvebx()
        {
            AssertCode("lvewx\tv24,r15,r0", "7F0F000F");
        }

        [Test]
        public void PPCDis_lvlx()
        {
            AssertCode(0x7c6b040e, "lvlx\tv3,r11,r0");
        }

        [Test]
        public void PPCDis_lvsr()
        {
            AssertCode("lvsr\tv0,r0,r10", "7C00504C");
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
            //AssertCode(0x4280fef8, "bc+    20,lt,0xffffffffffffff24<64>	 ");
            AssertCode(0x4300fef8, "bdnz\t$000FFEF8");
            AssertCode(0x4e800420, "bcctr\t14,00");
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
        public void PPCDis_stfiwx()
        {
            AssertCode(0x7c004fae, "stfiwx\tf0,r0,r9");
        }


        [Test]
        public void PPCDis_stvx()
        {
            AssertCode(0x7c2019ce, "stvx\tv1,r0,r3");
        }

        [Test]
        public void PPCDis_stvxl()
        {
            AssertCode("stvxl\tr28,r5,r11", "7F855BCE");
        }

        [Test]
        public void PPCDis_stxvb16x()
        {
            AssertCode("stxvb16x\tv23,r23,r14", "7EF777D8");
        }

        [Test]
        public void PPCDis_stxvh8x()
        {
            AssertCode("stxvh8x\tv31,r30,r9", "7FFE4F58");
        }

        [Test]
        public void PPCDis_stxvw4x()
        {
            AssertCode("stxvw4x\tv7,r23,r5", "7CF72F18");
        }

        [Test]
        public void PPCDis_cntlzd()
        {
            AssertCode(0x7d600074, "cntlzd\tr0,r11");
        }

        [Test]
        public void PPCDis_vctsxs()
        {
            AssertCode(0x118063ca, "vctsxs\tv12,v12,00");
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
        public void PPCDis_mtfsfi()
        {
            AssertCode("mtfsfi\t01,01,00", "FE42110C");
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
            AssertCode(0x7C7A03A6, "mtspr\tsrr0,r3");
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
            AssertCode(0xE0030000, "lq\tr0,0(r3)");
            AssertCode(0xF0090000, "xsaddsp\tv0,v18,v0");
            AssertCode(0x7D0B506E, "lwzux\tr8,r11,r10");

            AssertCode(0x7c001fac, "icbi\tr0,r3");
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

            AssertCode(0x7D6B5238, "eqv\tr11,r11,r10");
            AssertCode(0x7C8B22AA, "lwax\tr4,r11,r4");
            AssertCode(0x7D6B5392, "divdu\tr11,r11,r10");
            AssertCode(0x7D2943D2, "divd\tr9,r9,r8");
            AssertCode(0x7C0A5C6E, "lfsux\tf0,r10,r11");
            AssertCode(0x7DAA44EE, "lfdux\tf13,r10,r8");
            AssertCode(0x7D2B5E34, "srad\tr11,r9,r11");
            AssertCode(0x7C23F7EC, "dcbz\tr3,r30");

            AssertCode(0x7c0019ec, "dcbtst\tr0,r3");


            // The following instructions were found in an
            // XBox 360 binary, but no PowerPC documentation
            // seems to exist for them.
            /*
            AssertCode(0x12a0f9c7, ".long 0x12a0f9c7<32>");
            AssertCode(0x10030001, ".long 0x10030001<32>");
            AssertCode(0x10011003, ".long 0x10011003<32>");
            AssertCode(0x111110b0, ".long 0x111110b0<32>");
            AssertCode(0x100050c3, ".long 0x100050c3<32>");
            AssertCode(0x100130cb, ".long 0x100130cb<32>");
            AssertCode(0x13fff935, ".long 0x13fff935<32>");
            AssertCode(0x136a2987, ".long 0x136a2987<32>");
            AssertCode(0x13D29A35, ".long 0x13d29a35<32>");
            AssertCode(0x13e95187, ".long 0x13e95187<32>");
            AssertCode(0x100059c3, ".long 0x100059c3<32>");
            AssertCode(0x100b61cb, ".long 0x100b61cb<32>");
            AssertCode(0x13d29a35, ".long 0x13d29a35<32>");
            AssertCode(0x4d48c976, ".long 0x4d48c976<32>");
            AssertCode(0x4f8e1ae5, ".long 0x4f8e1ae5<32>");
            AssertCode(0x4c4d4e4f, ".long 0x4c4d4e4f<32>");
            AssertCode(0x7c53b17e, ".long 0x7c53b17e<32>");
            AssertCode(0x7dc2dec0, ".long 0x7dc2dec0<32>");
            AssertCode(0x7f7f7f7f, ".long 0x7f7f7f7f<32>");
            AssertCode(0x7f7f7f7f, ".long 0x7f7f7f7f<32>");
            AssertCode(0x7fefffff, ".long 0x7fefffff<32>");



            AssertCode(0x102038C3, ".long 0x102038c3<32>");
            AssertCode(0x102020CB, ".long 0x102020cb<32>");
            AssertCode(0x13CA1987, ".long 0x13ca1987<32>");
            AssertCode(0x100059C3, ".long 0x100059c3<32>");
            AssertCode(0x13E051C7, ".long 0x13e051c7<32>");
            AssertCode(0x116021C3, ".long 0x116021c3<32>");
            AssertCode(0x126B61CB, ".long 0x126b61cb<32>");


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
            AssertCode(0xf7Ad3927, "stxsd\tf29,14628(r13)"); // stfdp\tf28,14628(r13) odd floating point register
            AssertCode(0xf78d392E, "stxsd\tf28,14636(r13)"); // "stfdp\tf28,14628(r13) odd offset
            AssertCode(0xf78d3928, "stfdp\tf28,14632(r13)");
        }

        [Test]
        public void PPCDis_mfspr()
        {
            AssertCode(0x7FF94AA6, "mfspr\t00000139,r31");
        }

        [Test]
        public void PPCDis_lhzux()
        {
            AssertCode(0x7D69026E, "lhzux\tr11,r9,r0");
        }

        [Test]
        public void PPCDis_subfco()
        {
            AssertCode(0x7C0A5C11, "subfco.\tr0,r10,r11");
        }

        [Test]
        public void PPCDis_tlbie()
        {
            AssertCode(0x7C004A64, "tlbie\tr9");
        }

        [Test]
        public void PPCDis_vmaxfp()
        {
            AssertCode(0x1102640A, "vmaxfp\tv8,v2,v12");
        }

        [Test]
        public void PPCDis_bcdutrunc()
        {
            AssertCode(0x11417F41, "bcdutrunc.\tv10,v1,v15");
        }

        [Test]
        public void PPCDis_vcmpgtsh()
        {
            AssertCode(0x11417F46, "vcmpgtsh.\tv10,v1,v15");
        }

        [Test]
        public void PPCDis_vslv()
        {
            AssertCode(0x11417F44, "vslv\tv10,v1,v15");
        }

        [Test]
        public void PPCDis_vmsumubm()
        {
            AssertCode(0x11337F64, "vmsumubm\tv9,v19,v15,v29");
        }

        [Test]
        public void PPCDis_vsrv()
        {
            AssertCode(0x11337F04, "vsrv\tv9,v19,v15");
        }

        [Test]
        public void PPCDis_maddhd()
        {
            AssertCode(0x11337EB0, "maddhd\tr9,r19,r15,r26");
        }

        [Test]
        public void PPCDis_vmhaddshs()
        {
            AssertCode(0x11337FA0, "vmhaddshs\tv9,v19,v15,v30");
        }

        [Test]
        public void PPCDis_vmsumshm()
        {
            AssertCode(0x11338028, "vmsumshm\tv9,v19,v16,v0");
        }

        [Test]
        public void PPCDis_vmladduhm()
        {
            AssertCode(0x11337FA2, "vmladduhm\tv9,v19,v15,v30");
        }

        [Test]
        public void PPCDis_vsldoi()
        {
            AssertCode(0x1133812C, "vsldoi\tv9,v19,v16,04");
        }

        [Test]
        public void PPCDis_vaddubs()
        {
            AssertCode(0x11338200, "vaddubs\tv9,v19,v16");
        }

        [Test]
        public void PPCDis_vadduws()
        {
            AssertCode(0x11338280, "vadduws\tv9,v19,v16");
        }

        [Test]
        public void PPCDis_vaddeuqm()
        {
            AssertCode(0x11338D7C, "vaddeuqm\tv9,v19,v17,v21");
        }

        [Test]
        public void PPCDis_vmrgew()
        {
            AssertCode(0x11337F8C, "vmrgew\tv9,v19,v15");
        }

        [Test]
        public void PPCDis_vgbbd()
        {
            AssertCode(0x11338D0C, "vgbbd\tv9,v17");
        }

        [Test]
        public void PPCDis_vsubuwm()
        {
            AssertCode(0x11338C80, "vsubuwm\tv9,v19,v17");
        }

        [Test]
        public void PPCDis_vpmsumh()
        {
            AssertCode(0x11338C48, "vpmsumh\tv9,v19,v17");
        }

        [Test]
        public void PPCDis_vspltisw()
        {
            AssertCode(0x11338B8C, "vspltisw\tv9,-0000000D");
        }

        [Test]
        public void PPCDis_vspltish()
        {
            AssertCode(0x11338B4C, "vspltish\tv9,-0000000D");
        }

        [Test]
        public void PPCDis_vspltisb()
        {
            AssertCode(0x11338B0C, "vspltisb\tv9,-0000000D");
        }

        [Test]
        public void PPCDis_vspltw()
        {
            AssertCode(0x11338A8C, "vspltw\tv9,v17,03");
        }

        [Test]
        public void PPCDis_vnmsubfp()
        {
            AssertCode(0x102bf06f, "vnmsubfp\tv1,v11,v1,v30");
        }

        [Test]
        public void PPCDis_vperm()
        {
            AssertCode(0x114948ab, "vperm\tv10,v9,v9,v2");
        }

        [Test]
        public void PPCDis_xscmpexpqp()
        {
            AssertCode("xscmpexpqp\t03,v8,v31", "FDC8F949");
        }

        [Test]
        public void PPCDis_xscmpoqp()
        {
            AssertCode("xscmpoqp\t07,v31,v0", "FFFF0109");
        }

        [Test]
        public void PPCDis_xscpsgnqp()
        {
            AssertCode("xscpsgnqp\tv30,v8,v25", "FFC8C8C8");
        }

        [Test]
        public void PPCDis_xscvdpsxws()
        {
            AssertCode("xscvdpsxws\tv32,v1", "F0000961");
        }

        [Test]
        public void PPCDis_xscvuxddp()
        {
            AssertCode("xscvuxddp\tv63,v63", "F3E0FDA3");
        }

        [Test]
        public void PPCDis_xsdivdp()
        {
            AssertCode("xsdivdp\tv2,v2,v63", "F021F9C2");
        }

        [Test]
        public void PPCDis_xsdivqp()
        {
            AssertCode("xsdivqp\tv5,v19,v21", "FCB3AC48");
        }

        [Test]
        public void PPCDis_xsiexpdp()
        {
            AssertCode("xsiexpdp\tv60,r16,r28", "F390E72F");
        }

        [Test]
        public void PPCDis_xsmaddmdp()
        {
            AssertCode("xsmaddmdp\tv58,v50,v28", "F3B97148");
        }

        [Test]
        public void PPCDis_xsmaddqpo()
        {
            AssertCode("xsmaddqpo\tv13,v15,v15", "FDAF7B09");
        }

        [Test]
        public void PPCDis_xsmaxcdp()
        {
            AssertCode("xsmaxcdp\tv14,v48,v62", "F0F8FC00");
        }

        [Test]
        public void PPCDis_xsmsubqp()
        {
            AssertCode("xsmsubqp\tv17,v7,v13", "FE276B48");
        }

        [Test]
        public void PPCDis_xsnegdp()
        {
            AssertCode("xsnegdp\tv7,v38", "F0ED35E6");
        }

        [Test]
        public void PPCDis_xsnmsubmdp()
        {
            AssertCode("xsnmsubmdp\tv14,v52,v62", "F0FAFDC8");
        }

        [Test]
        public void PPCDis_xsrdpip()
        {
            AssertCode("xsrdpip\tv36,v40", "F25DA1A4");
        }

        [Test]
        public void PPCDis_xsredp()
        {
            AssertCode("xsredp\tv32,v13", "F0146969");
        }

        [Test]
        public void PPCDis_xsrqpix()
        {
            AssertCode("xsrqpix\t01,v31,v0,00", "FFE1000A");
        }

        [Test]
        public void PPCDis_xsrqpxp()
        {
            AssertCode("xsrqpxp\t01,v2,v29,00", "FC47E84A");
        }

        [Test]
        public void PPCDis_xssubdp()
        {
            AssertCode("xssubdp\tv6,v31,v26", "F06F6944");
        }

        [Test]
        public void PPCDis_xstdivdp()
        {
            AssertCode("xstdivdp\t07,v30,v37", "F3FE29EA");
        }

        [Test]
        public void PPCDis_xstsqrtdp()
        {
            AssertCode("xstsqrtdp\t02,v53", "F17AA9AB");
        }

        [Test]
        public void PPCDis_xststdcqp()
        {
            AssertCode("xststdcqp\t07,v62,7F", "FFFFFD88");
        }

        [Test]
        public void PPCDis_xvadddp()
        {
            AssertCode("xvadddp\tv32,v2,v2", "F2010B00");
        }

        [Test]
        public void PPCDis_xvcvdpsxws()
        {
            AssertCode("xvcvdpsxws\tv16,v24", "F208C360");
        }

        [Test]
        public void PPCDis_xvdivdp()
        {
            AssertCode("xvdivdp\tv1,v25,v43", "F00CABC7");
        }

        [Test]
        public void PPCDis_xvsubdp()
        {
            AssertCode("xvsubdp\tv0,v31,v36", "F00F9344");
        }

        [Test]
        public void PPCDis_xvsubsp()
        {
            AssertCode("xvsubsp\tv28,v40,v57", "F1D4E242");
        }

        [Test]
        public void PPCDis_xxlorc()
        {
            AssertCode("xxlorc\tv1,v1,v27", "F0006D57");
        }

        [Test]
        public void PPCDis_xxlxor()
        {
            AssertCode("xxlxor\tv58,v58,v58", "F3BDECD0");
        }

        [Test]
        public void PPCDis_xxpermdi()
        {
            AssertCode("xxpermdi\tv24,v1,v1,02", "F1800256");
        }

        [Test]
        public void PPCDis_xxsel()
        {
            AssertCode("xxsel\tv31,v36,v61,v6", "F1F2F0F3");
        }

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues

        // Reko: a decoder for the instruction 28BC407D at address 00000000000053C0 has not been implemented. (ldbrx)
        // Reko: a decoder for the instruction 28DC207D at address 00000000000053C4 has not been implemented. (ldbrx)
        // Reko: a decoder for the instruction F84B487D at address 00000000000053D0 has not been implemented. (cmpb)
        // Reko: a decoder for the instruction F8D3437D at address 00000000000053D4 has not been implemented. (cmpb)
        // Reko: a decoder for the instruction 6601097C at address 0000000000006914 has not been implemented. (mtvsrd)

        // Reko: a decoder for the instruction 500000F0 at address 0000000000006920 has not been implemented. (Ext3C 010)

        // Reko: a decoder for the instruction 576D00F0 at address 0000000000008314 has not been implemented. (Ext3C 010)

        // Reko: a decoder for the instruction 6A49417D at address 000000000001241C has not been implemented. (stdux)

        // Reko: a decoder for the instruction 570D00F0 at address 00000000000146F4 has not been implemented. (Ext3C 010)
 
        // Reko: a decoder for the instruction 500200F0 at address 00000000000170C8 has not been implemented. (Ext3C 010)

        // Reko: a decoder for the instruction D0544AF1 at address 0000000000018878 has not been implemented. (Ext3C 010)

        // Reko: a decoder for the instruction 6601037C at address 0000000000018B1C has not been implemented. (mtvsrd)

        // Reko: a decoder for the instruction 9C0700EC at address 0000000000018B24 has not been implemented. (fcfidus[.])

        // Reko: a decoder for the instruction 5E0700FC at address 0000000000018B40 has not been implemented. (fctiduz[.])
  
        // Reko: a decoder for the instruction 994CE07F at address 0000000000019048 has not been implemented. (lxsdx)

        // Reko: a decoder for the instruction 994CC07F at address 0000000000019050 has not been implemented. (lxsdx)

        // Reko: a decoder for the instruction A2F520F0 at address 00000000000190F0 has not been implemented. (xscvuxddp)
        
        // Reko: a decoder for the instruction A3FDE0F3 at address 00000000000190F4 has not been implemented. (xscvuxddp)
    
        // Reko: a decoder for the instruction C2F921F0 at address 0000000000019114 has not been implemented. (xsdivdp)
 
        // Reko: a decoder for the instruction 9C07E0EF at address 0000000000019F20 has not been implemented. (fcfidus[.])

        // Reko: a decoder for the instruction 9C07C0EF at address 0000000000019F28 has not been implemented. (fcfidus[.])

        // Reko: a decoder for the instruction 5EFF00FC at address 000000000001A070 has not been implemented. (fctiduz[.])
  
        // Reko: a decoder for the instruction 6601897D at address 000000000001A238 has not been implemented. (mtvsrd)
        // Reko: a decoder for the instruction 9C67C0EF at address 000000000001A248 has not been implemented. (fcfidus[.])

        // Reko: a decoder for the instruction D0ECBDF3 at address 000000000001A750 has not been implemented. (Ext3C 010)
   
        // Reko: a decoder for the instruction 281C407D at address 000000000001ECC8 has not been implemented. (ldbrx)

        // Reko: a decoder for the instruction 2824007D at address 000000000001ECCC has not been implemented. (ldbrx)
   
        // Reko: a decoder for the instruction F843497D at address 000000000001ECDC has not been implemented. (cmpb)
        // Reko: a decoder for the instruction F81B437D at address 000000000001ECE0 has not been implemented. (cmpb)
   
        // Reko: a decoder for the instruction 560280F1 at address 000000000002201C has not been implemented. (Ext3C 010)

        // Reko: a decoder for the instruction D0648CF1 at address 0000000000026850 has not been implemented. (Ext3C 010)
 
        // Reko: a decoder for the instruction D06CADF1 at address 0000000000026854 has not been implemented. (Ext3C 010)

        // Reko: a decoder for the instruction 616100F0 at address 00000000000268CC has not been implemented. (xscvdpsxws)
 
        // Reko: a decoder for the instruction D05C6BF1 at address 0000000000026904 has not been implemented. (Ext3C 010)
    
        // Reko: a decoder for the instruction 610900F0 at address 00000000000269B0 has not been implemented. (xscvdpsxws)

        // Reko: a decoder for the instruction D01442F0 at address 0000000000026AD8 has not been implemented. (Ext3C 010)



    }
}
