#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Arch.Xtensa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;

namespace Reko.UnitTests.Arch.Xtensa
{
    [TestFixture]
    public class XtensaDisassemblerTests : DisassemblerTestBase<XtensaInstruction>
    {
        private XtensaArchitecture arch;

        public XtensaDisassemblerTests()
        { 
            this.arch = new XtensaArchitecture();
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new LeImageWriter(bytes);
        }

        private void AssertCode(string sExp, uint uInstr)
        {
            var i = DisassembleWord(uInstr);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void Xtdasm_l32r()
        {
            AssertCode("l32r\ta7,000FFFFC", 0xFFFF71);
            AssertCode("l32r\ta2,000FFFF8", 0xFFFE21);
        }

        [Test]
        public void Xtdasm_ret()
        {
            AssertCode("ret\t", 0x000080);
        }

        [Test]
        public void Xtdasm_ill()
        {
            AssertCode("ill\t", 0x000000);
        }

        [Test]
        public void Xtdasm_wsr()
        {
            AssertCode("wsr\ta2,VECBASE", 0x13E720);
        }

        [Test]
        public void Xtdasm_or()
        {
            AssertCode("or\ta1,a1,a1", 0x201110);
        }

        [Test]
        public void Xtdasm_call0()
        {
            AssertCode("call0\t00100B24", 0x00B205);
        }
    }
}
