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
using Reko.Arch.MSP430;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Tlcs
{
    [TestFixture]
    public class MSP430DisassemblerTests : DisassemblerTestBase<Msp430Instruction>
    {
        private Msp430Architecture arch;

        public MSP430DisassemblerTests()
        {
            this.arch = new Msp430Architecture();
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress { get { return Address.Ptr16(0x0100); } }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new LeImageWriter(bytes);
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void MSP430Dis_xor()
        {
            AssertCode("xor.w\t@r4,r5", "25E4");
        }

        [Test]
        public void MSP430Dis_and_b()
        {
            AssertCode("and.b\t@r4,r6", "66F4");
        }

        [Test]
        public void MSP430Dis_bis_b()
        {
            AssertCode("bis.w\t@r4,1234(r6)", "A6D43412");
        }

        [Test]
        public void MSP430Dis_jmp()
        {
            AssertCode("jmp\t0100", "FF3F");
        }

        [Test]
        public void MSP430Dis_push_b()
        {
            AssertCode("push.b\tr5", "4512");
        }
    }
}