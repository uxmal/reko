#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class MicroMipsDisassemblerTests : DisassemblerTestBase<MipsInstruction>
    {
        private MipsProcessorArchitecture arch;

        [SetUp]
        public void Setup()
        {
            this.arch = new MipsBe32Architecture("mips-be-16");
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        protected override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new MicroMipsDisassembler(this.arch, rdr);
        }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        private void AssertCode(string expectedAsm, string hexInstr)
        {
            var instr = DisassembleHexBytes(hexInstr);
            Assert.AreEqual(expectedAsm, instr.ToString());
        }

        [Test]
        public void uMips_lbu16()
        {
            AssertCode("lbu16\tr4,000A(r3)", "0A3A");
        }
    }
}
