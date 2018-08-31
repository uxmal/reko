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
using Reko.Arch.M6800;
using Reko.Arch.M6800.M6812;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.M6800
{
    [TestFixture]
    public class M6812DisassemblerTests : DisassemblerTestBase<M6812Instruction>
    {
        private M6812Architecture arch;
        private Address addrLoad;
        private M6812Instruction instr;

        public M6812DisassemblerTests()
        {
            this.arch = new M6812Architecture("m6812");
            this.addrLoad = Address.Ptr16(0);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            throw new NotImplementedException();
        }
        private void Given_Code(string hexBytes)
        {
            this.instr = DisassembleHexBytes(hexBytes);
        }

        private void Expect_Instruction(string sExp)
        {
            Assert.AreEqual(sExp, this.instr.ToString());
        }

        [Test]
        public void M6812Dis_pshd()
        {
            Given_Code("3B");
            Expect_Instruction("pshd");
        }

        [Test]
        public void M6812Dis_clr()
        {
            Given_Code("6980");
            Expect_Instruction("clr\t$0000,sp");
        }

        [Test]
        public void M6812Dis_suba_imm()
        {
            Given_Code("8042");
            Expect_Instruction("suba\t#$42");
        }

        [Test]
        public void M6812Dis_cmpa_imm()
        {
            Given_Code("8142");
            Expect_Instruction("cmpa\t#$42");
        }

        [Test]
        public void M6812Dis_ldab_direct()
        {
            Given_Code("F64242");
            Expect_Instruction("ldab\t$4242");
        }

        // An M6812 decoder for instruction 37 has not been implemented.
        [Test]
        public void M6812Dis_37()
        {
            Given_Code("37");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction CD has not been implemented.
        [Test]
        public void M6812Dis_CD()
        {
            Given_Code("CD");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction C1 has not been implemented.
        [Test]
        public void M6812Dis_C1()
        {
            Given_Code("C1");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction 11 has not been implemented.
        [Test]
        public void M6812Dis_11()
        {
            Given_Code("11");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction 26 has not been implemented.
        [Test]
        public void M6812Dis_26()
        {
            Given_Code("26");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction 15 has not been implemented.
        [Test]
        public void M6812Dis_15()
        {
            Given_Code("15");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction 1F has not been implemented.
        [Test]
        public void M6812Dis_1F()
        {
            Given_Code("1F");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction 39 has not been implemented.
        [Test]
        public void M6812Dis_39()
        {
            Given_Code("39");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction 28 has not been implemented.
        [Test]
        public void M6812Dis_28()
        {
            Given_Code("28");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction 20 has not been implemented.
        [Test]
        public void M6812Dis_20()
        {
            Given_Code("20");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction 10 has not been implemented.
        [Test]
        public void M6812Dis_10()
        {
            Given_Code("10");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction C6 has not been implemented.
        [Test]
        public void M6812Dis_C6()
        {
            Given_Code("C6");
            Expect_Instruction("@@@");
        }

        // An M6812 decoder for instruction A7 has not been implemented.
        [Test]
        public void M6812Dis_A7()
        {
            Given_Code("A7");
            Expect_Instruction("@@@");
        }
    }
}
