#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Arch.M6800;
using Reko.Arch.M6800.M6812;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.M6800
{
    [TestFixture]
    public class M6812DisassemblerTests : DisassemblerTestBase<M6812Instruction>
    {
        private M6812Architecture arch;
        private Address addrLoad;
        private M6812Instruction instr;

        public M6812DisassemblerTests()
        {
            this.arch = new M6812Architecture("m6812");
            this.addrLoad = Address.Ptr16(0);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void Given_Code(string hexBytes)
        {
            this.instr = DisassembleHexBytes(hexBytes);
        }

        private void Expect_Instruction(string sExp)
        {
            Assert.AreEqual(sExp, this.instr.ToString());
        }

        [Test]
        public void M6812Dis_pshd()
        {
            Given_Code("3B");
            Expect_Instruction("pshd");
        }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
