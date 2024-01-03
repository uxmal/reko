#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.Sparc;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Sparc
{
    [TestFixture]
    public class Sparc64DisassemblerTests : DisassemblerTestBase<SparcInstruction>
    {
        private readonly SparcArchitecture64 arch;
        private readonly Address addrLoad;

        public Sparc64DisassemblerTests()
        {
            this.arch = new SparcArchitecture64(CreateServiceContainer(), "sparc64", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr64(0x10_0000_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void Sparc64Dasm_be_pn()
        {
            AssertCode("be,pn\t0000001000000020", "02600008");
        }

        [Test]
        public void Sparc64Dasm_ldx()
        {
            AssertCode("ldx\t[%l7+%g1],%g1", "C25DC001");
        }

        [Test]
        public void Sparc64Dasm_fblg_a()
        {
            AssertCode("fblg,a,pn\t0000000FFFF00000", "25640000");
        }

        [Test]
        public void Sparc64Dasm_fbu()
        {
            AssertCode("fbu,a,pt\t0000000FFFF1A588", "2F6C6962");
        }

        [Test]
        public void Spar64Dasm_movcs()
        {
            AssertCode("movcs\t%xcc,-00000001,%g1", "836577FF");
        }

        [Test]
        public void Sparc64Dasm_return()
        {
            AssertCode("return\t%i7+00000008", "81CFE008");
        }

        [Test]
        public void Sparc64Dasm_stx()
        {
            AssertCode("stx\t%i1,[%i6+2176]", "F277A880");
        }

    }
}
