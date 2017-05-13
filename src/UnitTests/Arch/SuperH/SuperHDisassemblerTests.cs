#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Arch.SuperH;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Tlcs
{
    [TestFixture]
    public class SuperHDisassemblerTests : DisassemblerTestBase<SuperHInstruction>
    {
        private SuperHArchitecture arch;

        public SuperHDisassemblerTests()
        {
            this.arch = new SuperHArchitecture();
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress { get { return Address.Ptr32(0x00010000); } }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new LeImageWriter(bytes);
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void SHDis_add_imm_rn()
        {
            AssertCode("add\t#FF,r3", "73FF");
        }

        [Test]
        public void SHDis_add_rm_rn()
        {
            AssertCode("add\tr4,r2", "324C");
        }

        [Test]
        public void SHDis_addc_rm_rn()
        {
            AssertCode("addc\tr4,r2", "324E");
        }

        [Test]
        public void SHDis_addv_rm_rn()
        {
            AssertCode("addv\tr4,r2", "324F");
        }

        [Test]
        public void SHDis_and_rm_rn()
        {
            AssertCode("and\tr4,r3", "2349");
        }

        [Test]
        public void SHDis_and_imm_r0()
        {
            AssertCode("and\t#F0,r0", "C9F0");
        }

        [Test]
        public void SHDis_and_b_imm_r0()
        {
            AssertCode("and.b\t#F0,@(r0,gbr)", "CDF0");
        }

        [Test]
        public void SHDis_bf()
        {
            AssertCode("bf\t0000FFE4", "8BF0");
        }

        [Test]
        public void SHDis_bf_s()
        {
            AssertCode("bf/s\t0000FFE4", "8FF0");
        }

        [Test]
        public void SHDis_bra()
        {
            AssertCode("bra\t0000FFE4", "AFF0");
        }

        [Test]
        public void SHDis_braf_reg()
        {
            AssertCode("braf\tr1", "0123");
        }

        [Test]
        public void SHDis_brk()
        {
            AssertCode("brk", "003B");
        }

        [Test]
        public void SHDis_bsr()
        {
            AssertCode("bsr\t0000FFE4", "BFF0");
        }
    }
}

