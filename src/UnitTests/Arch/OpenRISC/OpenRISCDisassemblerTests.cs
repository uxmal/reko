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
using Reko.Arch.OpenRISC;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.OpenRISC
{
    [TestFixture]
    public class OpenRISCDisassemblerTests : DisassemblerTestBase<OpenRISCInstruction>
    {
        private OpenRISCArchitecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new OpenRISCArchitecture("openRisc");
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void OpenRiscDis_l_add()
        {
            AssertCode("l.add\tr5,r5,r20", "E0A5A400");
        }

        [Test]
        public void OpenRiscDis_l_j()
        {
            AssertCode("l.j\t000FFFFC", "03FFFFFF");
        }

        [Test]
        public void OpenRiscDis_l_addic()
        {
            AssertCode("l.addic\tr5,r5,-00000003", "A0A5FFFD");
        }

        [Test]
        public void OpenRiscDis_jal()
        {
            AssertCode("l.jal\t0010A914", "04002A45");
        }

        [Test]
        public void OpenRiscDis_jal_backwards()
        {
            AssertCode("l.jal\t000FF674", "07FFFD9D");
        }

        [Test]
        public void OpenRiscDis_bf()
        {
            AssertCode("l.bf\t000FFFF4", "13FFFFFD");
        }


        [Test]
        public void OpenRiscDis_movhi()
        {
            AssertCode("l.movhi\tr3,000001F0", "186001F0");
        }

        [Test]
        public void OpenRiscDis_addi()
        {
            AssertCode("l.addi\tr1,r1,+00000010", "9C210010");
        }

        [Test]
        public void OpenRiscDis_jr()
        {
            AssertCode("l.jr\tr9", "44004800");
        }

        [Test]
        public void OpenRiscDis_nop()
        {
            AssertCode("l.nop", "15000000");
        }

        [Test]
        public void OpenRiscDis_slli()
        {
            AssertCode("l.slli\tr7,r2,00000006", "B8E20006");
        }

        [Test]
        public void OpenRiscDis_lwz()
        {
            AssertCode("l.lwz\tr2,-16(r1)", "8441FFF0");
        }

        [Test]
        public void OpenRiscDis_lwz_neg()
        {
            AssertCode("l.lwz\tr9,-4(r1)", "8521FFFC");
        }

        [Test]
        public void OpenRiscDis_ori()
        {
            AssertCode("l.ori\tr3,r16,00003004", "A8703004");
        }

        [Test]
        public void OpenRiscDis_srli()
        {
            AssertCode("l.srli\tr26,r11,0000001F", "BB4B005F");
        }

        [Test]
        public void OpenRiscDis_sw()
        {
            AssertCode("l.sw\t4(r1),r11", "D4015804");
        }

        [Test]
        public void OpenRiscDis_andi()
        {
            AssertCode("l.andi\tr11,r11,000000FF", "A56B00FF");
        }

        [Test]
        public void OpenRiscDis_sfnei()
        {
            AssertCode("l.sfnei\tr3,00000003", "BC230003");
        }

        [Test]
        public void OpenRiscDis_sfne()
        {
            AssertCode("l.sfne\tr11,r4", "E42B2000");
        }

        [Test]
        public void OpenRiscDis_l_jal()
        {
            AssertCode("l.jal\t00105CFC", "0400173F");
        }

        [Test]
        public void OpenRiscDis_jalr()
        {
            AssertCode("l.jalr\tr11", "48005800");
        }

        [Test]
        public void OpenRiscDis_lbz()
        {
            AssertCode("l.lbz\tr3,4(r1)", "8C610004");
        }

        [Test]
        public void OpenRiscDis_lbs()
        {
            AssertCode("l.lbs\tr4,0(r3)", "90830000");
        }

        [Test]
        public void OpenRiscDis_lhz()
        {
            AssertCode("l.lhz\tr4,8(r4)", "94840008");
        }

        [Test]
        public void OpenRiscDis_xori()
        {
            AssertCode("l.xori\tr11,r11,0000FFFF", "AD6BFFFF");
        }

        [Test]
        public void OpenRiscDis_rfe()
        {
            AssertCode("l.rfe", "25782C20");
        }

        [Test]
        public void OpenRiscDis_lwa()
        {
            AssertCode("l.lwa\tr19,11296(r7)", "6E672C20");
        }

        [Test]
        public void OpenRiscDis_sb()
        {
            AssertCode("l.sb\t17(r1),r2", "D8011011");
        }

        [Test]
        public void OpenRiscDis_adrp()
        {
            AssertCode("l.adrp\tr8,00104000", "09000002");
        }

        [Test]
        public void OpenRiscDis_mfspr()
        {
            AssertCode("l.mfspr\tr4,r0,00000020", "B4800020");
        }

        [Test]
        public void OpenRiscDis_mtspr()
        {
            AssertCode("l.mtspr\tr0,r5,SR", "C0002811");
        }

        [Test]
        public void OpenRiscDis_mtspr_grp()
        {
            AssertCode("l.mtspr\tr0,r4,00002002", "C0802002");
        }

        [Test]
        public void OpenRiscDis_mul()
        {
            AssertCode("l.mul\tr14,r11,r4", "E1CB2306");
        }

        [Test]
        public void OpenRiscDis_csync()
        {
            AssertCode("l.csync", "23000000");
        }

        [Test]
        public void OpenRiscDis_psync()
        {
            AssertCode("l.psync", "22800000");
        }
    }
}
