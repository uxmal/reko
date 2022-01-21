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
using Reko.Arch.Pdp10;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Pdp10
{
    [TestFixture]
    public class Pdp10DisassemblerTests : DisassemblerTestBase<Pdp10Instruction>
    {
        public Pdp10DisassemblerTests()
        {
            var options = new Dictionary<string, object>();
            Architecture = new Pdp10Architecture(CreateServiceContainer(), "pdp10", options);
            LoadAddress = new Address18(0x001000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        private void AssertCode(string sExp, string octalWord)
        {
            ulong word = Pdp10Architecture.OctalStringToWord(octalWord);
            var mem = new Word36MemoryArea(LoadAddress, new ulong[] { word });
            var i = Disassemble(mem);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void Pdp10Dis_addi()
        {
            AssertCode("addi\t12,60", "271500000060");
        }

        [Test]
        public void Pdp10Dis_jfcl()
        {
            AssertCode("jfcl", "255000000000");
        }

        [Test]
        public void Pdp10Dis_movei()
        {
            AssertCode("movei\t7,1", "201340000001");
        }

        [Test]
        public void Pdp10Dis_sojge()
        {
            AssertCode("sojge\t4,000147", "365200000147");
        }

        
    }
}
