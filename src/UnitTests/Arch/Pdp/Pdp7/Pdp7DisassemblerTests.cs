#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Arch.Pdp;
using Reko.Arch.Pdp.Memory;
using Reko.Arch.Pdp.Pdp7;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Pdp.Pdp7
{
    [TestFixture]
    public class Pdp7DisassemblerTests : DisassemblerTestBase<Pdp7Instruction>
    {
        public Pdp7DisassemblerTests()
        {
            var options = new Dictionary<string, object>();
            Architecture = new Pdp7Architecture(CreateServiceContainer(), "pdp7", options);
            LoadAddress = Pdp10Architecture.Ptr18(0x001000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        private void AssertCode(string sExp, string octalWord)
        {
            uint word = Pdp7Architecture.OctalStringToWord(octalWord);
            var mem = new Word18MemoryArea(LoadAddress, new uint[] { word });
            var i = Disassemble(mem);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void Pdp7Dis_add()
        {
            AssertCode("add\t1234", "301234");
            AssertCode("add\ti 5234", "325234");
        }

        [Test]
        public void Pdp7Dis_and()
        {
            AssertCode("and\t1234", "501234");
            AssertCode("and\ti 5234", "525234");
        }

        [Test]
        public void Pdp7Dis_cal()
        {
            AssertCode("cal", "000000");
        }

        [Test]
        public void Pdp7Dis_cla()
        {
            AssertCode("cla", "750000");
        }

        [Test]
        public void Pdp7Dis_cll()
        {
            AssertCode("cll", "744000");
        }

        [Test]
        public void Pdp7Dis_cma()
        {
            AssertCode("cma", "740001");
        }

        [Test]
        public void Pdp7Dis_cml()
        {
            AssertCode("cml", "740002");
        }

        [Test]
        public void Pdp7Dis_dac()
        {
            AssertCode("dac\t1234", "041234");
            AssertCode("dac\ti 5234", "065234");
        }

        [Test]
        public void Pdp7Dis_dzm()
        {
            AssertCode("dzm\t1234", "141234");
            AssertCode("dzm\ti 5234", "165234");
        }

        [Test]
        public void Pdp7Dis_jmp()
        {
            AssertCode("jmp\t5234", "605234");
        }

        [Test]
        public void Pdp7Dis_jms()
        {
            AssertCode("jms\t5234", "105234");
        }

        [Test]
        public void Pdp7Dis_hlt()
        {
            AssertCode("hlt", "740040");
        }

        [Test]
        public void Pdp7Dis_isz()
        {
            AssertCode("isz\t5234", "445234");
        }

        [Test]
        public void Pdp7Dis_lac()
        {
            AssertCode("lac\t1234", "201234");
        }

        [Test]
        public void Pdp7Dis_oas()
        {
            AssertCode("oas", "740004");
        }

        [Test]
        public void Pdp7Dis_opr()
        {
            AssertCode("opr", "740000");
        }

        [Test]
        public void Pdp7Dis_ral()
        {
            AssertCode("ral", "740010");
        }

        [Test]
        public void Pdp7Dis_rar()
        {
            AssertCode("rar", "740020");
        }

        [Test]
        public void Pdp7Dis_rtl()
        {
            AssertCode("rtl", "742010");
        }

        [Test]
        public void Pdp7Dis_rtr()
        {
            AssertCode("rtr", "742020");
        }

        [Test]
        public void Pdp7Dis_sad()
        {
            AssertCode("sad\ti 5234", "565234");
        }

        [Test]
        public void Pdp7Dis_sma()
        {
            AssertCode("sma", "740100");
        }

        [Test]
        public void Pdp7Dis_sna()
        {
            AssertCode("sna", "741200");
        }

        [Test]
        public void Pdp7Dis_snl()
        {
            AssertCode("snl", "740400");
        }

        [Test]
        public void Pdp7Dis_spa()
        {
            AssertCode("spa", "741100");
        }

        [Test]
        public void Pdp7Dis_sza()
        {
            AssertCode("sza", "740200");
        }

        [Test]
        public void Pdp7Dis_szl()
        {
            AssertCode("szl", "741400");
        }

        [Test]
        public void Pdp7Dis_tad()
        {
            AssertCode("tad\ti 5234", "365234");
        }

        [Test]
        public void Pdp7Dis_xct()
        {
            AssertCode("xct\ti 5234", "425234");
        }

        [Test]
        public void Pdp7Dis_xor()
        {
            AssertCode("xor\ti 5234", "265234");
        }
    }
}
