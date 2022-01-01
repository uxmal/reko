#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Arch.Tms320C28;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Arch.Tms320C28
{
    [TestFixture]
    public class Tms320C28DisassemblerTests : DisassemblerTestBase<Tms320C28Instruction>
    {
        private readonly Tms320C28Architecture arch;
        private readonly Address addr;

        public Tms320C28DisassemblerTests()
        {
            this.arch = new Tms320C28Architecture(CreateServiceContainer(), "tms320c28", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void Tms320C28Dasm_aborti()
        {
            AssertCode("aborti", "0001");
        }

        [Test]
        public void Tms320C28Dasm_subb_acc_imm()
        {
            AssertCode("subb\tacc,#3", "1903");
        }
    }
}
