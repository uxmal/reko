#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Arch.Avr;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Avr
{
    [TestFixture]
    public class Avr8DisassemblerTests : DisassemblerTestBase<AvrInstruction>
    {
        private Avr8Architecture arch;

        public Avr8DisassemblerTests()
        {
            this.arch = new Avr8Architecture();
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress { get { return Address.Ptr16(0x0000); } }

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
        public void Avr8_dis_rjmp()
        {
            AssertCode("rjmp\t001A", 0xC00C);
        }

        [Test]
        public void Avr8_dis_eor()
        {
            AssertCode("eor\tr1,r1", 0x2411);
        }

        [Test]
        public void Avr8_dis_out()
        {
            AssertCode("out\t3F,r1", 0xBE1F);
        }

        [Test]
        public void Avr8_dis_in()
        {
            AssertCode("in\tr1,3F", 0xB61F);
        }

        [Test]
        public void Avr8_dis_ldi()
        {
            AssertCode("ldi\tr28,5F", 0xE5CF);
        }

        [Test]
        public void Avr8_dis_rcall()
        {
            AssertCode("rcall\t0006", 0xD002);
        }

        [Test]
        public void Avr8_dis_push()
        {
            AssertCode("push\tr29", 0x93DF);
        }

        [Test]
        public void Avr8_dis_pop()
        {
            AssertCode("pop\tr28", 0x91CF);
        }

        [Test]
        public void Avr8_dis_ret()
        {
            AssertCode("ret", 0x9508);
        }

        [Test]
        public void Avr8_dis_cli()
        {
            AssertCode("cli", 0x94F8);
        }
    }
}
