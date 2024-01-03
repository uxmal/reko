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
using Reko.Arch.Fujitsu;
using Reko.Arch.Fujitsu.F2MC16FX;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Fujitsu.F2MC16FX
{
    [TestFixture]
    public class DisassemblerTests : DisassemblerTestBase<Instruction>
    {
        private readonly F2MC16FXArchitecture arch;

        public DisassemblerTests()
        {
            this.arch = new F2MC16FXArchitecture(CreateServiceContainer(), "f2mc16fx", new());
            this.LoadAddress = Address.Ptr32(0x0010_0000);
        }
        public override IProcessorArchitecture Architecture => arch;


        public override Address LoadAddress { get; }

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void F2MC16FXDis_jmp_indirect_post()
        {
            AssertCode("jmp\t@rw3+", "730F");
        }

        [Test]
        public void F2MC16FXDis_nop()
        {
            AssertCode("nop", "00");
        }
    }
}
