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
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class NanoMipsDisassemblerTests : DisassemblerTestBase<MipsInstruction>
    {
        private MipsProcessorArchitecture arch;
        private MipsInstruction instr;

        [SetUp]
        public void Setup()
        {
            this.arch = new MipsBe32Architecture("mips-be-micro");
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        protected override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new NanoMipsDisassembler(this.arch, rdr);
        }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        private void AssertCode(string expectedAsm, string hexInstr)
        {
            this.instr = DisassembleHexBytes(hexInstr);
            Assert.AreEqual(expectedAsm, instr.ToString());
        }

        private void Given_Mips64Architecture()
        {
            this.arch = new MipsBe64Architecture("mips-be-micro");
        }

        [Test]
        public void NanoMipsDis_Generate()
        {
            var ab = new byte[1000];
            var rnd = new Random(0x4711);
            rnd.NextBytes(ab);
            var mem = new MemoryArea(Address.Ptr32(0x00100000), ab);
            var rdr = new BeImageReader(mem, 0);
            var dasm = new NanoMipsDisassembler(arch, rdr);
            foreach (var instr in dasm)
            {
            }
        }

        [Test]
        public void NanoMipsDis_addiu_32()
        {
            AssertCode("addiu\tr2,r0,000007FF", "004007FF");
        }

        [Test]
        public void NanoMipsDis_addiu_gp_w()
        {
            AssertCode("addiu\tr20,r28,00102100", "42902100");
        }

        [Test]
        public void NanoMipsDis_addiu_neg()
        {
            AssertCode("addiu\tr7,r11,FFFFFFFF", "80EB8001");
        }

        [Test]
        public void NanoMipsDis_addiu_rs5()
        {
            AssertCode("addiu\tr10,r10,00000001", "9149");
            AssertCode("addiu\tr10,r10,00000007", "914F");
            AssertCode("addiu\tr10,r10,FFFFFFFF", "915F");
            AssertCode("addiu\tr10,r10,FFFFFFF8", "9158");
        }

        [Test]
        public void NanoMipsDis_addu16()
        {
            AssertCode("addu\tr20,r20,r11", "3E83");
        }

        [Test]
        public void NanoMipsDis_addu32()
        {
            AssertCode("addu\tr6,r12,r6", "20CC3150");
        }

        [Test]
        public void NanoMipsDis_andi_32()
        {
            AssertCode("andi\tr7,r7,000007FF", "80E727FF");
        }

        [Test]
        public void NanoMipsDis_balc16()
        {
            AssertCode("balc\t00100088", "3886");
        }

        [Test]
        public void NanoMipsDis_bbeqzc()
        {
            AssertCode("bbeqzc\tr4,00000017,00100072", "C884B86E");
        }

        [Test]
        public void NanoMipsDis_bbnezc()
        {
            AssertCode("bbnezc\tr9,00000013,00100028", "C9349824");
        }

        [Test]
        public void NanoMipsDis_bc16()
        {
            AssertCode("bc\t00100002", "1800");
            AssertCode("bc\t00100004", "1802");
            AssertCode("bc\t00100400", "1BFE");
            AssertCode("bc\t00100000", "1BFF");
            AssertCode("bc\t000FFC02", "1801");
        }

        [Test]
        public void NanoMipsDis_bc_32()
        {
            AssertCode("bc\t00100002", "29FFFFFF");
            AssertCode("bc\t000FFA56", "29FFFA53");
        }

        [Test]
        public void NanoMipsDis_beqc_32()
        {
            AssertCode("beqc\tr10,r2,000FFD7E", "884A3D7B");
        }

        [Test]
        public void NanoMipsDis_beqic()
        {
            AssertCode("beqic\tr6,00000020,0010000C", "C8C10008");
        }

        [Test]
        public void NanoMipsDis_beqzc_16()
        {
            AssertCode("beqzc\tr7,0010001C", "9B9A");
        }

        [Test]
        public void NanoMipsDis_bgec()
        {
            AssertCode("bgec\tr0,r11,0010014E", "8960814A");
        }

        [Test]
        public void NanoMipsDis_bgeic()
        {
            AssertCode("bgeic\tr6,00000020,0010003E", "C8C9003A");
        }

        [Test]
        public void NanoMipsDis_bltc()
        {
            AssertCode("bltc\tr10,r13,00100070", "A9AA806C");
        }

        [Test]
        public void NanoMipsDis_bnec()
        {
            AssertCode("bnec\tr5,r3,001002C6", "A86502C2");
            AssertCode("bnec\tr5,r3,00100002", "A8653FFF");
        }

        [Test]
        public void NanoMipsDis_bnezc_16()
        {
            AssertCode("bnezc\tr4,0010000A", "BA08");
        }

        [Test]
        public void NanoMipsDis_clz()
        {
            AssertCode("clz\tr7,r4", "20E45B3F");
        }

        [Test]
        public void NanoMipsDis_ext()
        {
            AssertCode("ext\tr10,r5,00000000,00000014", "8145F4C0");
        }


        [Test]
        public void NanoMipsDis_ins()
        {
            AssertCode("ins\tr6,r0,00000007,00000001", "80C0E5D7");
        }

        [Test]
        public void NanoMipsDis_jrc()
        {
            AssertCode("jrc\tra", "DBE0");
        }

        [Test]
        public void NanoMipsDis_li_16()
        {
            AssertCode("li\tr7,00000020", "D3A0");
        }

        [Test]
        public void NanoMipsDis_li_48()
        {
            AssertCode("li\tr7,000FFFFF", "60E0FFFF000F");
        }

        [Test]
        public void NanoMipsDis_lui()
        {
            AssertCode("lui\tr6,00000080", "E0C80000");
        }

        [Test]
        public void NanoMipsDis_lw_16()
        {
            AssertCode("lw\tr19,0008(r19)", "15B2");
        }

        [Test]
        public void NanoMipsDis_lw_sp()
        {
            AssertCode("lw\tr17,001C(sp)", "34C7");
        }

        [Test]
        public void NanoMipsDis_lw_gp_w()
        {
            AssertCode("lw\tr28,102108(r28)", "4390210A");
        }

        [Test]
        public void NanoMipsDis_lwxs()
        {
            AssertCode("lwxs\tr18,r19(r4)", "5235");
        }

        [Test]
        public void NanoMipsDis_movep()
        {
            AssertCode("movep\tr4,r5,r9,r8", "BC01");
        }

        [Test]
        public void NanoMipsDis_movep_rev()
        {
            AssertCode("movep\tr8,r9,r6,r7", "FC28");
        }

        [Test]
        public void NanoMipsDis_movz()
        {
            AssertCode("movz\tr4,r0,r6", "20C02210");
        }

        [Test]
        public void NanoMipsDis_mul4x4()
        {
            AssertCode("mul\tr8,r8,r16", "3C18");
        }

        [Test]
        public void NanoMipsDis_nor()
        {
            AssertCode("nor\tr11,r0,r11", "21605AD0");
        }

        [Test]
        public void NanoMipsDis_or32()
        {
            AssertCode("or\tr9,r9,r7", "20E94A90");
        }


        [Test]
        public void NanoMipsDis_restore16()
        {
            AssertCode("restore_jrc\t00000060,ra,00000006", "1F66");
        }

        [Test]
        public void NanoMipsDis_save16()
        {
            AssertCode("save\t000000F0,r30,0000000A", "1CFA");
        }

        [Test]
        public void NanoMipsDis_sigrie()
        {
            AssertCode("sigrie\t00000000", "00000000");
            Assert.AreEqual(InstrClass.Padding | InstrClass.Zero | InstrClass.Terminates, instr.InstructionClass);
        }

        [Test]
        public void NanoMipsDis_sll16()
        {
            AssertCode("sll\tr17,r19,00000008", "30B0");
        }

        [Test]
        public void NanoMipsDis_sll32()
        {
            AssertCode("sll\tr7,r9,00000003", "80E9C003");
        }

        [Test]
        public void NanoMipsDis_sllv()
        {
            AssertCode("sllv\tr4,r4,r6", "20C42010");
        }

        [Test]
        public void NanoMipsDis_slti()
        {
            AssertCode("slti\tr6,r6,00000020", "80C64020");
        }

        [Test]
        public void NanoMipsDis_sltu()
        {
            AssertCode("sltu\tr10,r12,r4", "208C5390");
        }

        [Test]
        public void NanoMipsDis_srl16()
        {
            AssertCode("srl\tr5,r17,00000003", "329B");
        }

        [Test]
        public void NanoMipsDis_srl32()
        {
            AssertCode("srl\tr3,r7,0000001F", "8067C05F");
        }

        [Test]
        public void NanoMipsDis_srlv()
        {
            AssertCode("srlv\tr7,r4,r7", "20E43850");
        }

        [Test]
        public void NanoMipsDis_subu_16()
        {
            AssertCode("subu\tr7,r7,r6", "B37F");
        }

        [Test]
        public void NanoMipsDis_subu_32()
        {
            AssertCode("subu\tr7,r0,r6", "20C039D0");
        }

        [Test]
        public void NanoMipsDis_sw_4x4()
        {
            AssertCode("sw\tr6,0000(r8)", "F4C0");
        }

        [Test]
        public void NanoMipsDis_sw_gp_w()
        {
            AssertCode("sw\tr0,101BB4(r28)", "40101BB7");
        }

        [Test]
        public void NanoMipsDis_xor16()
        {
            AssertCode("xor\tr5,r5,r16", "5284");
        }

        [Test]
        public void NanoMipsDis_xori()
        {
            AssertCode("xori\tr0,r8,00000142", "80081142");
        }

#if BORED
        // Reko: a decoder for nanoMips instruction A1E5D0DC at address 0010000E has not been implemented. (invalid(cp1))
        [Test]
        public void NanoMipsDis_A1E5D0DC()
        {
            AssertCode("@@@", "A1E5D0DC");
        }

        // Reko: a decoder for nanoMips instruction 45EAD0A6 at address 00100014 has not been implemented. (p.gp.bh)
        [Test]
        public void NanoMipsDis_45EAD0A6()
        {
            AssertCode("@@@", "45EAD0A6");
        }

        // Reko: a decoder for nanoMips instruction 00009688 at address 00100018 has not been implemented. (sw[16])
        [Test]
        public void NanoMipsDis_00009688()
        {
            AssertCode("@@@", "00009688");
        }

        // Reko: a decoder for nanoMips instruction 0000D9FB at address 00100026 has not been implemented. (P16.BR1)
        [Test]
        public void NanoMipsDis_0000D9FB()
        {
            AssertCode("@@@", "0000D9FB");
        }

        // Reko: a decoder for nanoMips instruction 86300045 at address 0010002C has not been implemented. (p.ls.u12)
        [Test]
        public void NanoMipsDis_86300045()
        {
            AssertCode("@@@", "86300045");
        }

        // Reko: a decoder for nanoMips instruction 000091B7 at address 00100030 has not been implemented. (ADDIU[R2])
        [Test]
        public void NanoMipsDis_000091B7()
        {
            AssertCode("@@@", "000091B7");
        }

        // Reko: a decoder for nanoMips instruction 0A0F30A3 at address 00100032 has not been implemented. (move.balc)
        [Test]
        public void NanoMipsDis_0A0F30A3()
        {
            AssertCode("@@@", "0A0F30A3");
        }

        // Reko: a decoder for nanoMips instruction 097B47FD at address 00100038 has not been implemented. (move.balc)
        [Test]
        public void NanoMipsDis_097B47FD()
        {
            AssertCode("@@@", "097B47FD");
        }

        // Reko: a decoder for nanoMips instruction 896170A8 at address 00100040 has not been implemented. (P.BR3A)
        [Test]
        public void NanoMipsDis_896170A8()
        {
            AssertCode("@@@", "896170A8");
        }

        // Reko: a decoder for nanoMips instruction 87795088 at address 00100050 has not been implemented. (p.ls.u12)
        [Test]
        public void NanoMipsDis_87795088()
        {
            AssertCode("@@@", "87795088");
        }

        // Reko: a decoder for nanoMips instruction 47104F25 at address 00100060 has not been implemented. (p.gp.bh)
        [Test]
        public void NanoMipsDis_47104F25()
        {
            AssertCode("@@@", "47104F25");
        }

        // Reko: a decoder for nanoMips instruction 00005CE0 at address 00100070 has not been implemented. (p16.lb)
        [Test]
        public void NanoMipsDis_00005CE0()
        {
            AssertCode("@@@", "00005CE0");
        }

        // Reko: a decoder for nanoMips instruction 00007589 at address 00100072 has not been implemented. (lw[4x4])
        [Test]
        public void NanoMipsDis_00007589()
        {
            AssertCode("@@@", "00007589");
        }

        // Reko: a decoder for nanoMips instruction 00009325 at address 0010007E has not been implemented. (ADDIU[R2])
        [Test]
        public void NanoMipsDis_00009325()
        {
            AssertCode("@@@", "00009325");
        }

        // Reko: a decoder for nanoMips instruction A7F9A72F at address 00100084 has not been implemented. (p.ls.s9)
        [Test]
        public void NanoMipsDis_A7F9A72F()
        {
            AssertCode("@@@", "A7F9A72F");
        }

        // Reko: a decoder for nanoMips instruction A78DFBDC at address 001000A2 has not been implemented. (p.ls.s9)
        [Test]
        public void NanoMipsDis_A78DFBDC()
        {
            AssertCode("@@@", "A78DFBDC");
        }

        // Reko: a decoder for nanoMips instruction A26394ED at address 001000AA has not been implemented. (invalid(cp1))
        [Test]
        public void NanoMipsDis_A26394ED()
        {
            AssertCode("@@@", "A26394ED");
        }

        // Reko: a decoder for nanoMips instruction 0000F2FC at address 001000D0 has not been implemented. (andi[16])
        [Test]
        public void NanoMipsDis_0000F2FC()
        {
            AssertCode("@@@", "0000F2FC");
        }

        // Reko: a decoder for nanoMips instruction 858D5581 at address 001000DE has not been implemented. (p.ls.u12)
        [Test]
        public void NanoMipsDis_858D5581()
        {
            AssertCode("@@@", "858D5581");
        }

        // Reko: a decoder for nanoMips instruction 8B486407 at address 001000EE has not been implemented. (P.BR3A)
        [Test]
        public void NanoMipsDis_8B486407()
        {
            AssertCode("@@@", "8B486407");
        }

        // Reko: a decoder for nanoMips instruction 0A4ACFF2 at address 001000F8 has not been implemented. (move.balc)
        [Test]
        public void NanoMipsDis_0A4ACFF2()
        {
            AssertCode("@@@", "0A4ACFF2");
        }

        // Reko: a decoder for nanoMips instruction 0000D886 at address 001000FC has not been implemented. (P16.BR1)
        [Test]
        public void NanoMipsDis_0000D886()
        {
            AssertCode("@@@", "0000D886");
        }

        // Reko: a decoder for nanoMips instruction 602B1D92 at address 00100106 has not been implemented. (LWPC[48])
        [Test]
        public void NanoMipsDis_602B1D92()
        {
            AssertCode("@@@", "602B1D92");
        }

        // Reko: a decoder for nanoMips instruction 0000D70E at address 0010010A has not been implemented. (sw[gp16])
        [Test]
        public void NanoMipsDis_0000D70E()
        {
            AssertCode("@@@", "0000D70E");
        }

        // Reko: a decoder for nanoMips instruction 05C5AF45 at address 00100120 has not been implemented. (addiupc[32])
        [Test]
        public void NanoMipsDis_05C5AF45()
        {
            AssertCode("@@@", "05C5AF45");
        }

        // Reko: a decoder for nanoMips instruction A7D8C2FE at address 00100128 has not been implemented. (p.ls.s9)
        [Test]
        public void NanoMipsDis_A7D8C2FE()
        {
            AssertCode("@@@", "A7D8C2FE");
        }

        // Reko: a decoder for nanoMips instruction 00007341 at address 0010012E has not been implemented. (p16.a1)
        [Test]
        public void NanoMipsDis_00007341()
        {
            AssertCode("@@@", "00007341");
        }

        // Reko: a decoder for nanoMips instruction 00007443 at address 00100130 has not been implemented. (lw[4x4])
        [Test]
        public void NanoMipsDis_00007443()
        {
            AssertCode("@@@", "00007443");
        }

        // Reko: a decoder for nanoMips instruction 09772075 at address 0010013A has not been implemented. (move.balc)
        [Test]
        public void NanoMipsDis_09772075()
        {
            AssertCode("@@@", "09772075");
        }

        // Reko: a decoder for nanoMips instruction 000076B3 at address 00100152 has not been implemented. (lw[4x4])
        [Test]
        public void NanoMipsDis_000076B3()
        {
            AssertCode("@@@", "000076B3");
        }

        // Reko: a decoder for nanoMips instruction 00007731 at address 0010015E has not been implemented. (lw[4x4])
        [Test]
        public void NanoMipsDis_00007731()
        {
            AssertCode("@@@", "00007731");
        }

        // Reko: a decoder for nanoMips instruction A4911CE3 at address 00100160 has not been implemented. (p.ls.s9)
        [Test]
        public void NanoMipsDis_A4911CE3()
        {
            AssertCode("@@@", "A4911CE3");
        }

        // Reko: a decoder for nanoMips instruction 88A965BD at address 00100168 has not been implemented. (P.BR3A)
        [Test]
        public void NanoMipsDis_88A965BD()
        {
            AssertCode("@@@", "88A965BD");
        }

        // Reko: a decoder for nanoMips instruction 084CD35A at address 0010016C has not been implemented. (move.balc)
        [Test]
        public void NanoMipsDis_084CD35A()
        {
            AssertCode("@@@", "084CD35A");
        }

        // Reko: a decoder for nanoMips instruction 000072D3 at address 00100176 has not been implemented. (p16.a1)
        [Test]
        public void NanoMipsDis_000072D3()
        {
            AssertCode("@@@", "000072D3");
        }

        // Reko: a decoder for nanoMips instruction A2924FB8 at address 0010017C has not been implemented. (invalid(cp1))
        [Test]
        public void NanoMipsDis_A2924FB8()
        {
            AssertCode("@@@", "A2924FB8");
        }

        // Reko: a decoder for nanoMips instruction A15DF251 at address 00100180 has not been implemented. (invalid(cp1))
        [Test]
        public void NanoMipsDis_A15DF251()
        {
            AssertCode("@@@", "A15DF251");
        }

        // Reko: a decoder for nanoMips instruction 4B1149E0 at address 001001B4 has not been implemented. (p.j)
        [Test]
        public void NanoMipsDis_4B1149E0()
        {
            AssertCode("@@@", "4B1149E0");
        }

        // Reko: a decoder for nanoMips instruction A12D737B at address 001001E4 has not been implemented. (invalid(cp1))
        [Test]
        public void NanoMipsDis_A12D737B()
        {
            AssertCode("@@@", "A12D737B");
        }

        // Reko: a decoder for nanoMips instruction 0000B653 at address 001001E8 has not been implemented. (sw[sp])
        [Test]
        public void NanoMipsDis_0000B653()
        {
            AssertCode("@@@", "0000B653");
        }

        // Reko: a decoder for nanoMips instruction 0000761D at address 001001F2 has not been implemented. (lw[4x4])
        [Test]
        public void NanoMipsDis_0000761D()
        {
            AssertCode("@@@", "0000761D");
        }

        // Reko: a decoder for nanoMips instruction 0B452C20 at address 001001F4 has not been implemented. (move.balc)
        [Test]
        public void NanoMipsDis_0B452C20()
        {
            AssertCode("@@@", "0B452C20");
        }

        // Reko: a decoder for nanoMips instruction 00005ED8 at address 00100208 has not been implemented. (p16.lb)
        [Test]
        public void NanoMipsDis_00005ED8()
        {
            AssertCode("@@@", "00005ED8");
        }

        // Reko: a decoder for nanoMips instruction 0000D9BA at address 0010020A has not been implemented. (P16.BR1)
        [Test]
        public void NanoMipsDis_0000D9BA()
        {
            AssertCode("@@@", "0000D9BA");
        }

        // Reko: a decoder for nanoMips instruction A19E0DF9 at address 00100216 has not been implemented. (invalid(cp1))
        [Test]
        public void NanoMipsDis_A19E0DF9()
        {
            AssertCode("@@@", "A19E0DF9");
        }

        // Reko: a decoder for nanoMips instruction 000095FF at address 0010021C has not been implemented. (sw[16])
        [Test]
        public void NanoMipsDis_000095FF()
        {
            AssertCode("@@@", "000095FF");
        }

        // Reko: a decoder for nanoMips instruction C05DC481 at address 00100222 has not been implemented. (invalid(mips64))
        [Test]
        public void NanoMipsDis_C05DC481()
        {
            AssertCode("@@@", "C05DC481");
        }

        // Reko: a decoder for nanoMips instruction 207BCB4F at address 00100234 has not been implemented. (LSA)
        [Test]
        public void NanoMipsDis_207BCB4F()
        {
            AssertCode("@@@", "207BCB4F");
        }

        // Reko: a decoder for nanoMips instruction 00005C5F at address 0010023E has not been implemented. (p16.lb)
        [Test]
        public void NanoMipsDis_00005C5F()
        {
            AssertCode("@@@", "00005C5F");
        }

        // Reko: a decoder for nanoMips instruction 00005D43 at address 00100248 has not been implemented. (p16.lb)
        [Test]
        public void NanoMipsDis_00005D43()
        {
            AssertCode("@@@", "00005D43");
        }

        // Reko: a decoder for nanoMips instruction C0D2D697 at address 00100282 has not been implemented. (invalid(mips64))
        [Test]
        public void NanoMipsDis_C0D2D697()
        {
            AssertCode("@@@", "C0D2D697");
        }

        // Reko: a decoder for nanoMips instruction 00009310 at address 00100292 has not been implemented. (ADDIU[R2])
        [Test]
        public void NanoMipsDis_00009310()
        {
            AssertCode("@@@", "00009310");
        }

        // Reko: a decoder for nanoMips instruction 00007644 at address 0010029E has not been implemented. (lw[4x4])
        [Test]
        public void NanoMipsDis_00007644()
        {
            AssertCode("@@@", "00007644");
        }

        // Reko: a decoder for nanoMips instruction 00007FA2 at address 001002AC has not been implemented. (p16.lh)
        [Test]
        public void NanoMipsDis_00007FA2()
        {
            AssertCode("@@@", "00007FA2");
        }

        // Reko: a decoder for nanoMips instruction A0BBE873 at address 001002BA has not been implemented. (invalid(cp1))
        [Test]
        public void NanoMipsDis_A0BBE873()
        {
            AssertCode("@@@", "A0BBE873");
        }

        // Reko: a decoder for nanoMips instruction 0000D442 at address 001002BE has not been implemented. (sw[gp16])
        [Test]
        public void NanoMipsDis_0000D442()
        {
            AssertCode("@@@", "0000D442");
        }

        // Reko: a decoder for nanoMips instruction A7CC8EE0 at address 001002C0 has not been implemented. (p.ls.s9)
        [Test]
        public void NanoMipsDis_A7CC8EE0()
        {
            AssertCode("@@@", "A7CC8EE0");
        }

        // Reko: a decoder for nanoMips instruction 44C5EC08 at address 001002C4 has not been implemented. (p.gp.bh)
        [Test]
        public void NanoMipsDis_44C5EC08()
        {
            AssertCode("@@@", "44C5EC08");
        }

        // Reko: a decoder for nanoMips instruction 0000F1CE at address 001002D0 has not been implemented. (andi[16])
        [Test]
        public void NanoMipsDis_0000F1CE()
        {
            AssertCode("@@@", "0000F1CE");
        }

        // Reko: a decoder for nanoMips instruction 00005E61 at address 001002D6 has not been implemented. (p16.lb)
        [Test]
        public void NanoMipsDis_00005E61()
        {
            AssertCode("@@@", "00005E61");
        }

        // Reko: a decoder for nanoMips instruction 00007F0F at address 001002DE has not been implemented. (p16.lh)
        [Test]
        public void NanoMipsDis_00007F0F()
        {
            AssertCode("@@@", "00007F0F");
        }

        // Reko: a decoder for nanoMips instruction 0AB31349 at address 001002E2 has not been implemented. (move.balc)
        [Test]
        public void NanoMipsDis_0AB31349()
        {
            AssertCode("@@@", "0AB31349");
        }

        // Reko: a decoder for nanoMips instruction 000096F1 at address 001002FC has not been implemented. (sw[16])
        [Test]
        public void NanoMipsDis_000096F1()
        {
            AssertCode("@@@", "000096F1");
        }

        // Reko: a decoder for nanoMips instruction 00005D8C at address 001002FE has not been implemented. (p16.lb)
        [Test]
        public void NanoMipsDis_00005D8C()
        {
            AssertCode("@@@", "00005D8C");
        }

        // Reko: a decoder for nanoMips instruction 60A37E55 at address 00100316 has not been implemented. (ADDIUPC[48])
        [Test]
        public void NanoMipsDis_60A37E55()
        {
            AssertCode("@@@", "60A37E55");
        }

        // Reko: a decoder for nanoMips instruction 45E6FAC4 at address 0010031A has not been implemented. (p.gp.bh)
        [Test]
        public void NanoMipsDis_45E6FAC4()
        {
            AssertCode("@@@", "45E6FAC4");
        }

        // Reko: a decoder for nanoMips instruction C0890D8D at address 00100320 has not been implemented. (invalid(mips64))
        [Test]
        public void NanoMipsDis_C0890D8D()
        {
            AssertCode("@@@", "C0890D8D");
        }

        // Reko: a decoder for nanoMips instruction 0000D9C8 at address 0010032E has not been implemented. (P16.BR1)
        [Test]
        public void NanoMipsDis_0000D9C8()
        {
            AssertCode("@@@", "0000D9C8");
        }

        // Reko: a decoder for nanoMips instruction 0000D81F at address 00100330 has not been implemented. (P16.BR1)
        [Test]
        public void NanoMipsDis_0000D81F()
        {
            AssertCode("@@@", "0000D81F");
        }

        // Reko: a decoder for nanoMips instruction 06C47647 at address 00100334 has not been implemented. (addiupc[32])
        [Test]
        public void NanoMipsDis_06C47647()
        {
            AssertCode("@@@", "06C47647");
        }

        // Reko: a decoder for nanoMips instruction 00007651 at address 00100342 has not been implemented. (lw[4x4])
        [Test]
        public void NanoMipsDis_00007651()
        {
            AssertCode("@@@", "00007651");
        }

        // Reko: a decoder for nanoMips instruction 000097C7 at address 00100344 has not been implemented. (sw[16])
        [Test]
        public void NanoMipsDis_000097C7()
        {
            AssertCode("@@@", "000097C7");
        }

        // Reko: a decoder for nanoMips instruction 00007C8C at address 0010034C has not been implemented. (p16.lh)
        [Test]
        public void NanoMipsDis_00007C8C()
        {
            AssertCode("@@@", "00007C8C");
        }

        // Reko: a decoder for nanoMips instruction 000097FB at address 00100356 has not been implemented. (sw[16])
        [Test]
        public void NanoMipsDis_000097FB()
        {
            AssertCode("@@@", "000097FB");
        }

        // Reko: a decoder for nanoMips instruction 8B01757B at address 0010035E has not been implemented. (P.BR3A)
        [Test]
        public void NanoMipsDis_8B01757B()
        {
            AssertCode("@@@", "8B01757B");
        }

        // Reko: a decoder for nanoMips instruction C082CB92 at address 0010036C has not been implemented. (invalid(mips64))
        [Test]
        public void NanoMipsDis_C082CB92()
        {
            AssertCode("@@@", "C082CB92");
        }

        // Reko: a decoder for nanoMips instruction 85D25953 at address 00100374 has not been implemented. (p.ls.u12)
        [Test]
        public void NanoMipsDis_85D25953()
        {
            AssertCode("@@@", "85D25953");
        }

        // Reko: a decoder for nanoMips instruction 00007D00 at address 00100382 has not been implemented. (p16.lh)
        [Test]
        public void NanoMipsDis_00007D00()
        {
            AssertCode("@@@", "00007D00");
        }

        // Reko: a decoder for nanoMips instruction 87F34BFD at address 00100386 has not been implemented. (p.ls.u12)
        [Test]
        public void NanoMipsDis_87F34BFD()
        {
            AssertCode("@@@", "87F34BFD");
        }

        // Reko: a decoder for nanoMips instruction 495FB9DF at address 0010039C has not been implemented. (p.j)
        [Test]
        public void NanoMipsDis_495FB9DF()
        {
            AssertCode("@@@", "495FB9DF");
        }

        // Reko: a decoder for nanoMips instruction 05CB553E at address 001003A4 has not been implemented. (addiupc[32])
        [Test]
        public void NanoMipsDis_05CB553E()
        {
            AssertCode("@@@", "05CB553E");
        }

        // Reko: a decoder for nanoMips instruction 0000D70D at address 001003A8 has not been implemented. (sw[gp16])
        [Test]
        public void NanoMipsDis_0000D70D()
        {
            AssertCode("@@@", "0000D70D");
        }

        // Reko: a decoder for nanoMips instruction 0000D87B at address 001003B2 has not been implemented. (P16.BR1)
        [Test]
        public void NanoMipsDis_0000D87B()
        {
            AssertCode("@@@", "0000D87B");
        }

        // Reko: a decoder for nanoMips instruction 86D9DE40 at address 001003B8 has not been implemented. (p.ls.u12)
        [Test]
        public void NanoMipsDis_86D9DE40()
        {
            AssertCode("@@@", "86D9DE40");
        }

        // Reko: a decoder for nanoMips instruction 4415CB1C at address 001003C0 has not been implemented. (p.gp.bh)
        [Test]
        public void NanoMipsDis_4415CB1C()
        {
            AssertCode("@@@", "4415CB1C");
        }

        // Reko: a decoder for nanoMips instruction C17D8B2E at address 001003C4 has not been implemented. (invalid(mips64))
        [Test]
        public void NanoMipsDis_C17D8B2E()
        {
            AssertCode("@@@", "C17D8B2E");
        }

        // Reko: a decoder for nanoMips instruction 8729425C at address 001003C8 has not been implemented. (p.ls.u12)
        [Test]
        public void NanoMipsDis_8729425C()
        {
            AssertCode("@@@", "8729425C");
        }

        // Reko: a decoder for nanoMips instruction 000091F3 at address 001003CC has not been implemented. (ADDIU[R2])
        [Test]
        public void NanoMipsDis_000091F3()
        {
            AssertCode("@@@", "000091F3");
        }

        // Reko: a decoder for nanoMips instruction 859C873E at address 001003D2 has not been implemented. (p.ls.u12)
        [Test]
        public void NanoMipsDis_859C873E()
        {
            AssertCode("@@@", "859C873E");
        }

        // Reko: a decoder for nanoMips instruction 636B43C8 at address 001003E0 has not been implemented. (LWPC[48])
        [Test]
        public void NanoMipsDis_636B43C8()
        {
            AssertCode("@@@", "636B43C8");
        }

        // Reko: a decoder for nanoMips instruction A39D06C1 at address 001003E4 has not been implemented. (invalid(cp1))
        [Test]
        public void NanoMipsDis_A39D06C1()
        {
            AssertCode("@@@", "A39D06C1");
        }
#endif

    }
}
