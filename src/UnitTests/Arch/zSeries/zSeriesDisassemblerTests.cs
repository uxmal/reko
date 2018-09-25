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
using Reko.Arch.zSeries;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.zSeries
{
    [TestFixture]
    public class zSeriesDisassemblerTests : DisassemblerTestBase<zSeriesInstruction>
    {
        private zSeriesArchitecture arch;

        public zSeriesDisassemblerTests()
        {
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        [SetUp]
        public void Setup()
        {
            this.arch = new zSeriesArchitecture("zSeries");
        }

        public override IProcessorArchitecture Architecture {  get { return arch; } }

        public override Address LoadAddress { get; }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public void AssertCode(string sExp, string machineCode)
        {
            var instr = DisassembleHexBytes(machineCode);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void zSysDasm_ar()
        {
            AssertCode("ar\tr1,r2", "1A12");
        }

        [Test]
        public void zSysDasm_nr()
        {
            AssertCode("nr\tr1,r2", "1412");
        }
    }
}
