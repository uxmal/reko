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
using Reko.Arch.WE32100;
using Reko.Core;
using System;
using System.Collections.Generic;
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
            this.arch = new WE32100Architecture("we32100");
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            throw new NotImplementedException();
        }

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
    }
}
