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
using Reko.Arch.CompactRisc;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.CompactRisc
{
    [TestFixture]
    public class Cr16DisassemblerTests : DisassemblerTestBase<Cr16Instruction>
    {
        private readonly Cr16Architecture arch;
        private readonly Address addrLoad;

        public Cr16DisassemblerTests()
        {
            this.arch = new Cr16Architecture(CreateServiceContainer(), "cr16c", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr16(0x8000);
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addrLoad;

        // [Test]
        public void Cr16Dasm_Gen()
        {
            var mem = new ByteMemoryArea(Address.Ptr16(0x8000), new byte[1024]);
            var rnd = new Random(0x4711);
            rnd.NextBytes(mem.Bytes);
            var rdr = mem.CreateBeReader(0);
            var dasm = arch.CreateDisassembler(rdr);
            dasm.Take(100).ToArray();
        }

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void Cr16Dasm_movb_imm()
        {
            AssertCode("movb\t$1,r2", "1258");
        }

        [Test]
        public void Cr16Dasm_movb_reg()
        {
            AssertCode("movb\tr1,r2", "1259");
        }

        [Test]
        public void Cr16Dasm_movd_imm20()
        {
            AssertCode("movd\t$12345,r2", "2105 4523");
        }
    }
}
