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
using Reko.Arch.Pdp;
using Reko.Arch.Pdp.Memory;
using Reko.Arch.Pdp.Pdp7;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Pdp.Pdp7
{
    [TestFixture]
    public class Pdp7DisassemblerTests : DisassemblerTestBase<Pdp7Instruction>
    {
        public Pdp7DisassemblerTests()
        {
            var options = new Dictionary<string, object>();
            Architecture = new Pdp7Architecture(CreateServiceContainer(), "pdp7", options);
            LoadAddress = new Address18(0x001000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        private void AssertCode(string sExp, string octalWord)
        {
            uint word = Pdp7Architecture.OctalStringToWord(octalWord);
            var mem = new Word18MemoryArea(LoadAddress, new uint[] { word });
            var i = Disassemble(mem);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void Pdp7Dis_lac()
        {
            AssertCode("lac\t1234", "201234");
        }
    }
}
