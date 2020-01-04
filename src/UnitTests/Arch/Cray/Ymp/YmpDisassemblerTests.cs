#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Arch.Cray;
using Reko.Arch.Cray.Cray1;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Cray.Ymp
{
    [TestFixture]
    public class YmpDisassemblerTests : DisassemblerTestBase<CrayInstruction>
    {
        private CrayYmpArchitecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new CrayYmpArchitecture("crayYmp");
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExp, string octBytes)
        {
            var instr = DisassembleOctBytes(octBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new BeImageWriter(bytes);
        }

        protected CrayInstruction DisassembleOctBytes(string octalBytes)
        {
            var img = new MemoryArea(LoadAddress, new byte[256]);
            byte[] bytes = OctalStringToBytes(octalBytes);
            return DisassembleBytes(bytes);
        }

        [Test]
        public void YmpDis_S_and()
        {
            AssertCode("S1\tS2&S3", "043123");
        }

        [Test]
        public void YmpDis_mov_Ai_Sj()
        {
            AssertCode("A7\tS1", "023710");
        }

        [Test]
        public void YmpDis_mov_Si_Vj_Ak()
        {
            AssertCode("S1\tV2,A3", "076123");
        }

        [Test]
        public void YmpDis_fmul_Sj_Sk()
        {
            AssertCode("S1\tS2*FS3", "064123");
        }

        [Test]
        public void YmpDis_j_Bjk()
        {
            AssertCode("j\tB63", "005077");
        }
    }
}
