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
        public void NanoMipsDis_addiu32()
        {
            AssertCode("addiu\tr2,r0,000007FF", "004007FF");
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
        public void NanoMipsDis_lw16()
        {
            AssertCode("lw\tr19,0008(r19)", "15B2");
        }

        [Test]
        public void NanoMipsDis_lw_sp()
        {
            AssertCode("lw\tr17,001C(sp)", "34C7");
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
        public void NanoMipsDis_xor16()
        {
            AssertCode("xor\tr5,r5,r16", "5284");
        }

        [Test]
        public void NanoMipsDis_xori()
        {
            AssertCode("xori\tr0,r8,00000142", "80081142");
        }


    }
}
