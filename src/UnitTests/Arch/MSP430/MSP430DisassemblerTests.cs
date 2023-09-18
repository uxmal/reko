#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.Msp430;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Tlcs
{
    [TestFixture]
    public class MSP430DisassemblerTests : DisassemblerTestBase<Msp430Instruction>
    {
        public MSP430DisassemblerTests()
        {
            this.Architecture = new Msp430Architecture(new ServiceContainer(), "msp430", new Dictionary<string, object>());
            this.LoadAddress = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void MSP430Dis_xor()
        {
            AssertCode("xor.w\t@r4,r5", "25E4");
        }

        [Test]
        public void MSP430Dis_and_b()
        {
            AssertCode("and.b\t@r4,r6", "66F4");
        }

        [Test]
        public void MSP430Dis_bis_b()
        {
            AssertCode("bis.w\t@r4,1234(r6)", "A6D43412");
        }

        [Test]
        public void MSP430Dis_jmp()
        {
            AssertCode("jmp\t0100", "FF3F");
        }

        [Test]
        public void MSP430Dis_push_b()
        {
            AssertCode("push.b\tr5", "4512");
        }

        [Test]
        public void MSP430Dis_push_w()
        {
            AssertCode("push.w\tr5", "0512");
        }

        [Test]
        public void MSP430Dis_call()
        {
            AssertCode("call\tr8", "8812");
        }

        [Test]
        public void MSP430Dis_symbolic()
        {
            AssertCode("rrc.w\t1236(pc)", "1010 3412");
        }

        [Test]
        public void MSP430Dis_call_immediate()
        {
            AssertCode("call\t1234", "B012 3412");
        }

        [Test]
        public void MSP430Dis_dint()
        {
            AssertCode("dint", "32C2");
        }

        [Test]
        public void MSP430Dis_eint()
        {
            AssertCode("eint", "32D2");
        }

        [Test]
        public void MSP430Dis_add_two_abs()
        {
            AssertCode("add.w\t&579C,&7778", "9252 9C57 7877");
        }

        [Test]
        public void MSP430Dis_quick_immediates()
        {
            AssertCode("mov.w\t#0004,r8", "28 42");
            AssertCode("mov.w\t#0008,r8", "38 42");
            AssertCode("mov.w\t#0000,r8", "08 43");
            AssertCode("mov.w\t#0001,r8", "18 43");
            AssertCode("mov.w\t#0002,r8", "28 43");
            AssertCode("mov.w\t#FFFF,r8", "38 43");
            AssertCode("mov.b\t#02,r8", "68 43");
        }

        [Test]
        public void MSP430Dis_ret()
        {
            AssertCode("ret.w", "3041");
        }

        [Test]
        public void MSP430Dis_rrc()
        {
            AssertCode("rrc.w\tpc", "0010");
        }

        [Test]
        public void MSP430Dis_sub_w()
        {
            AssertCode("sub.w\t-0040(r5),r4", "1485C0FF");
        }

        [Test]
        public void MSP430Dis_bic()
        {
            AssertCode("bic.w\t#0004,sr", "22C2");
        }

        [Test]
        public void MSP430Dis_bic_gie()
        {
            AssertCode("dint", "32C2");
        }

        [Test]
        public void MSP430Dis_goto()
        {
            AssertCode("br.w\t414C", "30404C41");
        }

        [Test]
        public void MSP430Dis_add_b_pcrel_pcrel()
        {
            AssertCode("add.b\t4768(pc),-08BC(pc)", "D050 6647 40F7");
        }

        [Test]
        public void MSP430Dis_mov_imm()
        {
            AssertCode("mov.w\t#EEA0,r12", "3C40 A0EE");
        }
    }
}
