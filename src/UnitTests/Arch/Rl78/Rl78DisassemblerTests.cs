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
using Reko.Arch.Rl78;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Rl78
{
    [TestFixture]
    public class Rl78DisassemblerTests : DisassemblerTestBase<Rl78Instruction>
    {
        private Rl78Architecture arch;
        private Address addr;

        public Rl78DisassemblerTests()
        {
            this.arch = new Rl78Architecture("rl78");
            this.addr = Address.Ptr32(0x1000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void Rl78Dis_nop()
        {
            AssertCode("nop", "00");
        }

        [Test]
        public void Rl78Dis_mov_x_imm()
        {
            AssertCode("mov\tx,0x42", "50 42");
        }

        [Test]
        public void Rl78Dis_cmp_a_hl()
        {
            AssertCode("cmp\ta,[hl]", "4D");
        }

        [Test]
        public void Rl78Dis_sub_a_es_addr16()
        {
            AssertCode("sub\ta,es:[1234h]", "11 2F 34 12");
        }

        [Test]
        public void Rl78Dis_add_a_hl_off()
        {
            AssertCode("add\ta,[hl+80h]", "0E 80");
        }

        [Test]
        public void Rl78Dis_movw_addr_c_ax()
        {
            AssertCode("movw\t[1234h+c],ax", "68 34 12");
        }

        [Test]
        public void Rl78Dis_mov_psw_imm()
        {
            AssertCode("mov\t[0FFFFAh],0x42", "CE FA 42");
        }

        [Test]
        public void Rl78Dis_shr_a_6()
        {
            AssertCode("shr\ta,0x06", "31 6A");
        }

        [Test]
        public void Rl78Dis_set1_hl()
        {
            AssertCode("set1\t[hl].7", "71 F2");
        }

    }
}
