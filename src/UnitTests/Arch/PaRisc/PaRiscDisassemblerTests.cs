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
using Reko.Arch.PaRisc;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.PaRisc
{
    [TestFixture]
    public class PaRiscDisassemblerTests : DisassemblerTestBase<PaRiscInstruction>
    {
        private readonly PaRiscArchitecture arch;

        public PaRiscDisassemblerTests()
        {
            this.arch = new PaRiscArchitecture("paRisc");
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr32(0x00100000);


        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new BeImageWriter(bytes);
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void PaRiscDis_add()
        {
            AssertCode("add\tgr1,gr7,gr4,tr", "08E18624");
        }


    // memMgmt
    [Test]
    [Ignore("Format is complex; try simpler ones first")]
    public void PaRiscDis_058C7910()
        {
            AssertCode("@@@", "058C7910");
        }


        [Test]
        public void PaRiscDis_break()
        {
            AssertCode("break\t00,0000", "00000000");
        }


        // branch
        [Test]
        public void PaRiscDis_bl()
        {
            AssertCode("bl\t00101EC8", "E800A3D8");
        }

        [Test]
        public void PaRiscDis_nop()
        {
            AssertCode("or\tgr0,gr0,gr0", "08000240");
        }

        [Test]
        public void PaRiscDis_ldw()
        {
            AssertCode("ldw\t-47(gr30,sr0),gr2", "4BC23FD1");
        }

        [Test]
        public void PaRiscDis_bv_n()
        {
            AssertCode("bv,n\tgr0(gr2)", "E840D002");
        }

        /*
        // ldw
        [Test]
        public void PaRiscDis_4B753D21()
        {
            AssertCode("@@@", "4B753D21");
        }
        
        // indexMem
        [Test]
        public void PaRiscDis_0EC41093()
        {
            AssertCode("@@@", "0EC41093");
        }

        // indexMem
        [Test]
        public void PaRiscDis_0EDD1096()
        {
            AssertCode("@@@", "0EDD1096");
        }

        // systemOp
        [Test]
        public void PaRiscDis_02C010A1()
        {
            AssertCode("@@@", "02C010A1");
        }

        // systemOp
        [Test]
        public void PaRiscDis_00011820()
        {
            AssertCode("@@@", "00011820");
        }

        // be
        [Test]
        public void PaRiscDis_E2C00000()
        {
            AssertCode("@@@", "E2C00000");
        }

        // stw
        [Test]
        public void PaRiscDis_6BC23FD1()
        {
            AssertCode("@@@", "6BC23FD1");
        }

        // branch
        [Test]
        public void PaRiscDis_EAC0D000()
        {
            AssertCode("@@@", "EAC0D000");
        }

        // ldw
        [Test]
        public void PaRiscDis_4B733D29()
        {
            AssertCode("@@@", "4B733D29");
        }

        // branch
        [Test]
        public void PaRiscDis_EAA0D000()
        {
            AssertCode("@@@", "EAA0D000");
        }

        // ldil
        [Test]
        public void PaRiscDis_23603800()
        {
            AssertCode("@@@", "23603800");
        }

        // ldo
        [Test]
        public void PaRiscDis_377B0D00()
        {
            AssertCode("@@@", "377B0D00");
        }

        // ldo
        [Test]
        public void PaRiscDis_37DE0100()
        {
            AssertCode("@@@", "37DE0100");
        }

        // deposit
        [Test]
        public void PaRiscDis_D7C01C1D()
        {
            AssertCode("@@@", "D7C01C1D");
        }

        // indexMem
        [Test]
        public void PaRiscDis_0F201083()
        {
            AssertCode("@@@", "0F201083");
        }

        // addil
        [Test]
        public void PaRiscDis_2B7FEFFF()
        {
            AssertCode("@@@", "2B7FEFFF");
        }

        // stw
        [Test]
        public void PaRiscDis_68230300()
        {
            AssertCode("@@@", "68230300");
        }

        // stw
        [Test]
        public void PaRiscDis_68380330()
        {
            AssertCode("@@@", "68380330");
        }

        // coprW
        [Test]
        public void PaRiscDis_27791200()
        {
            AssertCode("@@@", "27791200");
        }

        // ldil
        [Test]
        public void PaRiscDis_20A00000()
        {
            AssertCode("@@@", "20A00000");
        }

        // ldo
        [Test]
        public void PaRiscDis_34A50000()
        {
            AssertCode("@@@", "34A50000");
        }

        // indexMem
        [Test]
        public void PaRiscDis_0F791084()
        {
            AssertCode("@@@", "0F791084");
        }

        // or
        [Test]
        public void PaRiscDis_08A40245()
        {
            AssertCode("@@@", "08A40245");
        }

        // indexMem
        [Test]
        public void PaRiscDis_0F651299()
        {
            AssertCode("@@@", "0F651299");
        }

        // coprW
        [Test]
        public void PaRiscDis_27791000()
        {
            AssertCode("@@@", "27791000");
        }

        // addi
        [Test]
        public void PaRiscDis_B40010C2()
        {
            AssertCode("@@@", "B40010C2");
        }

        // branch
        [Test]
        public void PaRiscDis_E80001AA()
        {
            AssertCode("@@@", "E80001AA");
        }

        // combf
        [Test]
        public void PaRiscDis_8BD7A06A()
        {
            AssertCode("@@@", "8BD7A06A");
        }

        // combt
        [Test]
        public void PaRiscDis_83178062()
        {
            AssertCode("@@@", "83178062");
        }

        // indexMem
        [Test]
        public void PaRiscDis_0EE01085()
        {
            AssertCode("@@@", "0EE01085");
        }

        // stw
        [Test]
        public void PaRiscDis_68250320()
        {
            AssertCode("@@@", "68250320");
        }

        // indexMem
        [Test]
        public void PaRiscDis_0EF81085()
        {
            AssertCode("@@@", "0EF81085");
        }

        // stw
        [Test]
        public void PaRiscDis_68250328()
        {
            AssertCode("@@@", "68250328");
        }

        // ldil
        [Test]
        public void PaRiscDis_20802800()
        {
            AssertCode("@@@", "20802800");
        }

        // ldo
        [Test]
        public void PaRiscDis_34840018()
        {
            AssertCode("@@@", "34840018");
        }

        // indexMem
        [Test]
        public void PaRiscDis_0EE81085()
        {
            AssertCode("@@@", "0EE81085");
        }

        // extract
        [Test]
        public void PaRiscDis_D0A619FA()
        {
            AssertCode("@@@", "D0A619FA");
        }

        // indexMem
        [Test]
        public void PaRiscDis_0C861240()
        {
            AssertCode("@@@", "0C861240");
        }

        // extract
        [Test]
        public void PaRiscDis_D0A61A9B()
        {
            AssertCode("@@@", "D0A61A9B");
        }

        // indexMem
        [Test]
        public void PaRiscDis_0C861244()
        {
            AssertCode("@@@", "0C861244");
        }

        // addil
        [Test]
        public void PaRiscDis_2803F0B0()
        {
            AssertCode("@@@", "2803F0B0");
        }

        // ldo
        [Test]
        public void PaRiscDis_34330220()
        {
            AssertCode("@@@", "34330220");
        }

        // ldil
        [Test]
        public void PaRiscDis_20202000()
        {
            AssertCode("@@@", "20202000");
        }

        // ldw
        [Test]
        public void PaRiscDis_483F0000()
        {
            AssertCode("@@@", "483F0000");
        }

        // addil
        [Test]
        public void PaRiscDis_2817E0A8()
        {
            AssertCode("@@@", "2817E0A8");
        }

        // combt
        [Test]
        public void PaRiscDis_83F3200A()
        {
            AssertCode("@@@", "83F3200A");
        }

        // ldo
        [Test]
        public void PaRiscDis_34330C60()
        {
            AssertCode("@@@", "34330C60");
        }

        // combf
        [Test]
        public void PaRiscDis_8BF3208A()
        {
            AssertCode("@@@", "8BF3208A");
        }

        // ldo
        [Test]
        public void PaRiscDis_37C70000()
        {
            AssertCode("@@@", "37C70000");
        }

        // addil
        [Test]
        public void PaRiscDis_2BC10000()
        {
            AssertCode("@@@", "2BC10000");
        }

        // ldo
        [Test]
        public void PaRiscDis_343E0000()
        {
            AssertCode("@@@", "343E0000");
        }

        // ldo
        [Test]
        public void PaRiscDis_37440000()
        {
            AssertCode("@@@", "37440000");
        }

        // ldo
        [Test]
        public void PaRiscDis_37250000()
        {
            AssertCode("@@@", "37250000");
        }

        // ldo
        [Test]
        public void PaRiscDis_37060000()
        {
            AssertCode("@@@", "37060000");
        }

        // ldo
        [Test]
        public void PaRiscDis_37D90000()
        {
            AssertCode("@@@", "37D90000");
        }

        // ldil
        [Test]
        public void PaRiscDis_23E10000()
        {
            AssertCode("@@@", "23E10000");
        }

        // ble
        [Test]
        public void PaRiscDis_E7E02630()
        {
            AssertCode("@@@", "E7E02630");
        }

        // ldo
        [Test]
        public void PaRiscDis_37E20000()
        {
            AssertCode("@@@", "37E20000");
        }

        // ldo
        [Test]
        public void PaRiscDis_349A0000()
        {
            AssertCode("@@@", "349A0000");
        }

        // ldo
        [Test]
        public void PaRiscDis_34B90000()
        {
            AssertCode("@@@", "34B90000");
        }

        // ldo
        [Test]
        public void PaRiscDis_34D80000()
        {
            AssertCode("@@@", "34D80000");
        }

        // ldo
        [Test]
        public void PaRiscDis_34FE0000()
        {
            AssertCode("@@@", "34FE0000");
        }

        // branch
        [Test]
        public void PaRiscDis_E8400068()
        {
            AssertCode("@@@", "E8400068");
        }

        // ldil
        [Test]
        public void PaRiscDis_23E12000()
        {
            AssertCode("@@@", "23E12000");
        }

        // ble
        [Test]
        public void PaRiscDis_E7E023B0()
        {
            AssertCode("@@@", "E7E023B0");
        }

        // indexMem
        [Test]
        public void PaRiscDis_0FC01299()
        {
            AssertCode("@@@", "0FC01299");
        }

        [Test]
        public void PaRiscDis_addil_neg()
        {
            AssertCode("@@@", "2B7FEFFF");
        }

        // ldw
        [Test]
        public void PaRiscDis_48380330()
        {
            AssertCode("@@@", "48380330");
        }

        // addil
        [Test]
        public void PaRiscDis_addil_0()
        {
            AssertCode("@@@", "2B600000");
        }

        // ldo
        [Test]
        public void PaRiscDis_34210000()
        {
            AssertCode("@@@", "34210000");
        }

        // ldw
        [Test]
        public void PaRiscDis_483600B8()
        {
            AssertCode("@@@", "483600B8");
        }

        // comibt
        [Test]
        public void PaRiscDis_86C02032()
        {
            AssertCode("@@@", "86C02032");
        }

        // ldo
        [Test]
        public void PaRiscDis_341A0002()
        {
            AssertCode("@@@", "341A0002");
        }

        // branch
        [Test]
        public void PaRiscDis_EBFF1CF5()
        {
            AssertCode("@@@", "EBFF1CF5");
        }

        // ldil
        [Test]
        public void PaRiscDis_23E03000()
        {
            AssertCode("@@@", "23E03000");
        }

        // ble
        [Test]
        public void PaRiscDis_E7E02AB0()
        {
            AssertCode("@@@", "E7E02AB0");
        }

        // systemOp
        [Test]
        public void PaRiscDis_0001A004()
        {
            AssertCode("@@@", "0001A004");
        }

        // stw
        [Test]
        public void PaRiscDis_6BC23FD9()
        {
            AssertCode("@@@", "6BC23FD9");
        }

        // ldo
        [Test]
        public void PaRiscDis_37DE0080()
        {
            AssertCode("@@@", "37DE0080");
        }

        // ldw
        [Test]
        public void PaRiscDis_4BF600B0()
        {
            AssertCode("@@@", "4BF600B0");
        }

        // combt
        [Test]
        public void PaRiscDis_82C02010()
        {
            AssertCode("@@@", "82C02010");
        }

        // ble
        [Test]
        public void PaRiscDis_E6C02000()
        {
            AssertCode("@@@", "E6C02000");
        }

        // systemOp
        [Test]
        public void PaRiscDis_004010A1()
        {
            AssertCode("@@@", "004010A1");
        }

        // be
        [Test]
        public void PaRiscDis_E0400002()
        {
            AssertCode("@@@", "E0400002");
        }

        // stwm
        [Test]
        public void PaRiscDis_6FC30100()
        {
            AssertCode("@@@", "6FC30100");
        }

        // stw
        [Test]
        public void PaRiscDis_6BC43F09()
        {
            AssertCode("@@@", "6BC43F09");
        }

        // addil
        [Test]
        public void PaRiscDis_28022000()
        {
            AssertCode("@@@", "28022000");
        }

        [Test]
        public void PaRiscDis_6BDA3F21()
        {
            AssertCode("@@@", "6BDA3F21");
        }
*/
    }
}
