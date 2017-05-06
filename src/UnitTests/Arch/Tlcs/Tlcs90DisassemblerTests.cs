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

using Reko.Arch.Tlcs.Tlcs90;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Arch.Tlcs;
using NUnit.Framework;

namespace Reko.UnitTests.Arch.Tlcs
{
    public class Tlcs90DisassemblerTests : DisassemblerTestBase<Tlcs90Instruction>
    {
        private Tlcs90Architecture arch = new Tlcs90Architecture();

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return Address.Ptr16(0x0000); }
        }

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
        public void Tlcs90_dis_nop()
        {
            AssertCode("nop", "00");
        }

        [Test]
        public void Tlcs90_dst()
        {
            AssertCode("ld\t(0000),a", "eb000026");
        }

        [Test]
        public void Tlcs90_dis_jp()
        {
            AssertCode("jp\t0100", "1a0001");
        }

        [Test]
        public void Tlcs90_dis_pop_bc()
        {
            AssertCode("pop\tbc", "58");
        }

        [Test]
        public void Tlcs90_dis_ld_n()
        {
            AssertCode("ld\t(FF42),a", "2F42");
        }

        [Test]
        public void Tlcs90_dis_ret()
        {
            AssertCode("ret", "1E");
        }
    }
}
