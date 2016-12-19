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
using Reko.Arch.Tlcs;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Tlcs
{
    [TestFixture]
    public class Tlcs900DisassemblerTests : DisassemblerTestBase<Tlcs900Instruction>
    {
        private Tlcs900Architecture arch;

        public Tlcs900DisassemblerTests()
        {
            this.arch = new Tlcs900Architecture();
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress { get { return Address.Ptr16(0x0000); } }

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
        public void Tlcs900_dis_nop()
        {
            AssertCode("nop", "00");
        }

        [Test]
        public void Tlcs900_dis_push_RR()
        {
            AssertCode("push\twa", "28");
        }

        [Test]
        public void Tlcs900_dis_ld_reg_byte()
        {
            AssertCode("ld\ta,b", "CA89");
        }

        [Test]
        public void Tlcs900_dis_add_reg_indirect_word()
        {
            AssertCode("add\tbc,(xhl)", "9381");
        }

        [Test]
        public void Tlcs900_dis_sub_reg_indexed_8()
        {
            AssertCode("sub\tde,(xsp+-4)", "9FFCA2");
        }

        [Test]
        public void Tlcs900_dis_xor_reg_indexed_16()
        {
            AssertCode("xor\tde,(xsp+-4)", "D31DFCFFD2");
        }

        [Test]
        public void Tlcs900_dis_inc_reg()
        {
            AssertCode("inc\t00000004,xbc", "E964");
        }

        [Test]
        public void Tlcs900_dis_inc_predec()
        {
            AssertCode("inc\t00000001,(-xde)", "E40961");
        }
    }
}
