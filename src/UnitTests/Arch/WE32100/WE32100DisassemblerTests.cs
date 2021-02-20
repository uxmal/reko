#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Arch.WE32100;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.WE32100
{
    [TestFixture]
    public class WE32100DisassemblerTests : DisassemblerTestBase<WE32100Instruction>
    {
        private readonly WE32100Architecture arch;
        private readonly Address addr;

        public WE32100DisassemblerTests()
        {
            this.arch = new WE32100Architecture(new ServiceContainer(), "we32100", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExpected, string hexInstr)
        {
            var instr = DisassembleHexBytes(hexInstr);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void WE32100Dis_movb()
        {
            AssertCode("movb\t$0x100,%r0", "877F0001000040");
        }

        [Test]
        public void WE32100Dis_dech()
        {
            AssertCode("dech\t*$0x100", "96EF00010000");
        }

        [Test]
        public void WE32100Dis_xorw2()
        {
            AssertCode("xorw2\t-4(%r1),%r4", "B4C1FC44");
        }

        [Test]
        public void WE32100Dis_add3b()
        {
            AssertCode("addb3\t*-4(%r5),%r4,%r3", "DFD5FC4443");
        }

        [Test]
        public void WE32100Dis_movb_deferred_short()
        {
            AssertCode("movb\t06,*31688(%sp)", "8706BCC87B");
        }

        [Test]
        public void WE32100Dis_addb3()
        {
            AssertCode("addb3\tFA,*-45(%r7),2(%ap)", "DFFAD7D372"); // 46DEF06F57F3DC06FB274C");
        }

        [Test]
        public void WE32100Dis_movb_deferred()
        {
            AssertCode("movb\t0C,*969639990(%pc)", "870C9F3688CB39"); // E8110FF6AB36EDCADF");
        }

        [Test]
        public void WE32100Dis_movb_register_deferred()
        {
            AssertCode("movb\t22,0(%r1)", "872251"); // E05FEE0AC1CC7F38E3B5F38E92");
        }

        [Test]
        public void WE32100Dis_xorh2_fpoffset()
        {
            AssertCode("xorh2\t003E,13(%fp)", "B63E6D");// AD3B86740534DE74DF38FC18C6");
        }
    }
}
