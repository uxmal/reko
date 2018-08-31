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


        [Test]
        public void M6812Dis_aba()
        {
            Given_Code("1806");
            Expect_Instruction("aba");
        }


        [Test]
        public void M6812Dis_adca_immediate()
        {
            Given_Code("8942");
            Expect_Instruction("adca\t#$42");
        }

        [Test]
        public void M6812Dis_adca_direct()
        {
            Given_Code("8942");
            Expect_Instruction("adca\t#$42");
        }

        [Test]
        public void M6812Dis_addd_imm()
        {
            Given_Code("C31234");
            Expect_Instruction("addd\t#$1234");
        }

        [Test]
        public void M6812Dis_bcc()
        {
            Given_Code("24FE");
            Expect_Instruction("bcc\t0000");
        }

        [Test]
        public void M6812Dis_lbcc()
        {
            Given_Code("1824FFFC");
            Expect_Instruction("lbcc\t0000");
        }
    }
}
