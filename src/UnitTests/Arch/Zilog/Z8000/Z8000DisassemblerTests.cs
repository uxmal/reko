#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.Zilog;
using Reko.Arch.Zilog.Z8000;
using Reko.Core;
using Reko.Core.Lib;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Zilog.Z8000
{
    [TestFixture]
    public class Z8000DisassemblerTests : DisassemblerTestBase<Z8000Instruction>
    {
        private readonly Z8000Architecture arch;

        public Z8000DisassemblerTests()
        {
            arch = new Z8000Architecture(CreateServiceContainer(), "z8000", new(), new(), new());
            LoadAddress = Address.Ptr16(0x100);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress { get; }

        private void AssertCode(string sExpected, string hexString)
        {
            var instr = DisassembleHexBytes(hexString);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void Z8000Dis_adc()
        {
            AssertCode("adc\tr1,r2", "B521");
        }

        [Test]
        public void Z8000Dis_adcb()
        {
            AssertCode("adcb\trh1,rh2", "B421");
        }

        [Test]
        public void Z8000Dis_add_r()
        {
            AssertCode("add\tr1,r2", "8121");
        }

        [Test]
        public void Z8000Dis_addb_r()
        {
            AssertCode("addb\trh1,rh2", "8021");
        }

        [Test]
        public void Z8000Dis_addl_r()
        {
            AssertCode("addl\trr0,rr2", "9621");
        }

        [Test]
        public void Z8000Dis_add_im()
        {
            AssertCode("add\tr4,#%1234", "0104 1234");
        }

        [Test]
        public void Z8000Dis_addb_im()
        {
            AssertCode("addb\trh4,#%34", "0004 1234");
        }

        [Test]
        public void Z8000Dis_addl_im()
        {
            AssertCode("addl\trr4,#%12345678", "1604 1234 5678");
        }
    }
}
