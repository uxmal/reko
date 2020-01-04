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

using NUnit.Framework;
using Reko.Arch.MicroBlaze;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.MicroBlaze
{
    [TestFixture]
    public class MicroBlazeDisassemblerTests : DisassemblerTestBase<MicroBlazeInstruction>
    {
        private MicroBlazeArchitecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new MicroBlazeArchitecture("microBlaze");
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        private void AssertCode(string expectedAsm, string hexInstr)
        {
            var instr = DisassembleHexBytes(hexInstr);
            Assert.AreEqual(expectedAsm, instr.ToString());
        }

        [Test]
        public void MicroBlazeDis_add_r0_r0()
        {
            AssertCode("add\tr3,r0,r0", "00600000");
        }

        [Test]
        public void MicroBlazeDis_add_r0()
        {
            AssertCode("add\tr28,r5,r0", "03850000");
        }

        [Test]
        public void MicroBlazeDis_addc()
        {
            AssertCode("addc\tr3,r3,r3", "08631800");
        }

        [Test]
        public void MicroBlazeDis_addik()
        {
            AssertCode("addik\tr1,r1,0000001C", "3021001C");
        }

        [Test]
        public void MicroBlazeDis_addk()
        {
            AssertCode("addk\tr3,r3,r5", "10632800");
        }

        [Test]
        public void MicroBlazeDis_addk_r0_r0()
        {
            AssertCode("addk\tr3,r0,r0", "10600000");
        }

        [Test]
        public void MicroBlazeDis_and()
        {
            AssertCode("and\tr3,r7,r3", "84671800");
        }

        [Test]
        public void MicroBlazeDis_andi()
        {
            AssertCode("andi\tr5,r4,000000FF", "A4A400FF");
        }

        [Test]
        public void MicroBlazeDis_beqi()
        {
            AssertCode("beqi\tr19,000FFEB4", "BC13FEB4");
        }

        [Test]
        public void MicroBlazeDis_beqid()
        {
            AssertCode("beqid\tr3,000FFFD8", "BE03FFD8");
        }

        [Test]
        public void MicroBlazeDis_bgei()
        {
            AssertCode("bgei\tr4,00100094", "BCA40094");
        }

        [Test]
        public void MicroBlazeDis_bgeid()
        {
            AssertCode("bgeid\tr18,000FFFE0", "BEB2FFE0");
        }

        [Test]
        public void MicroBlazeDis_bgtid()
        {
            AssertCode("bgtid\tr3,000FFFC8", "BE83FFC8");
        }

        [Test]
        public void MicroBlazeDis_blei()
        {
            AssertCode("blei\tr26,000FFF64", "BC7AFF64");
        }

        [Test]
        public void MicroBlazeDis_blti()
        {
            AssertCode("blti\tr18,000FFFD4", "BC52FFD4");
        }

        [Test]
        public void MicroBlazeDis_bltid()
        {
            AssertCode("bltid\tr18,000FFFD8", "BE52FFD8");
        }

        [Test]
        public void MicroBlazeDis_bnei()
        {
            AssertCode("bnei\tr3,000FFF8C", "BC23FF8C");
        }

        [Test]
        public void MicroBlazeDis_bneid()
        {
            AssertCode("bneid\tr22,000FFFC4", "BE36FFC4");
        }

        [Test]
        public void MicroBlazeDis_bra()
        {
            AssertCode("bra\tr3", "98081800");
        }

        [Test]
        public void MicroBlazeDis_brai()
        {
            AssertCode("brai\t00002E40", "B8082E40");
        }

        [Test]
        public void MicroBlazeDis_bri()
        {
            AssertCode("bri\t000FFE40", "B800FE40");
        }

        [Test]
        public void MicroBlazeDis_brid()
        {
            AssertCode("brid\t000FFFF0", "B810FFF0");
        }

        [Test]
        public void MicroBlazeDis_brlid()
        {
            AssertCode("brlid\tr15,001068D0", "B9F468D0");
        }

        [Test]
        public void MicroBlazeDis_cmp()
        {
            AssertCode("cmp\tr18,r3,r22", "1643B001");
        }

        [Test]
        public void MicroBlazeDis_cmpu()
        {
            AssertCode("cmpu\tr18,r4,r23", "1644B803");
        }

        [Test]
        public void MicroBlazeDis_lbui()
        {
            AssertCode("lbui\tr3,r0,FFFFD644", "E060D644");
        }

        [Test]
        public void MicroBlazeDis_lw()
        {
            AssertCode("lw\tr4,r4,r21", "C884A800");
        }

        [Test]
        public void MicroBlazeDis_lwi()
        {
            AssertCode("lwi\tr21,r1,00000028", "EAA10028");
        }

        [Test]
        public void MicroBlazeDis_mul()
        {
            AssertCode("mul\tr3,r4,r3", "40641800");
        }

        [Test]
        public void MicroBlazeDis_neg()
        {
            AssertCode("rsubi\tr5,r5,00000000", "24A50000");
        }

        [Test]
        public void MicroBlazeDis_or()
        {
            AssertCode("or\tr3,r4,r3", "80641800");
        }

        [Test]
        public void MicroBlazeDis_rsub()
        {
            AssertCode("rsub\tr18,r5,r6", "06453000");
        }

        [Test]
        public void MicroBlazeDis_rsubk()
        {
            AssertCode("rsubk\tr21,r3,r21", "16A3A800");
        }

        [Test]
        public void MicroBlazeDis_rtsd()
        {
            AssertCode("rtsd\tr15,00000008", "B60F0008");
        }

        [Test]
        public void MicroBlazeDis_sext8()
        {
            AssertCode("sext8\tr3,r3", "90630060");
        }

        [Test]
        public void MicroBlazeDis_sra()
        {
            AssertCode("sra\tr19,r4", "92640001");
        }

        [Test]
        public void MicroBlazeDis_srl()
        {
            AssertCode("srl\tr21,r4", "92A40041");
        }

        [Test]
        public void MicroBlazeDis_swi()
        {
            AssertCode("swi\tr3,r5,00000008", "F8650008");
        }

        [Test]
        public void MicroBlazeDis_xor()
        {
            AssertCode("xor\tr4,r4,r8", "88844000");
        }

        [Test]
        public void MicroBlazeDis_xori()
        {
            AssertCode("xori\tr21,r3,FFFFFFFF", "AAA3FFFF");
        }
    }
}
