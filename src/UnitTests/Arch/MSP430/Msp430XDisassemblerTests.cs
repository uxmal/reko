#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Machine;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.Tlcs
{
    [TestFixture]
    public class MSP430XDisassemblerTests : DisassemblerTestBase<Msp430Instruction>
    {
        public MSP430XDisassemblerTests()
        {
            this.Architecture = new Msp430Architecture(new ServiceContainer(), "msp430", new()
            {
                { ProcessorOption.InstructionSet, "MSP430X" }
            });
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
        public void MSP430Dis_cmpa()
        {
            AssertCode("cmpa.a\tr9,r4", "D409");
        }

        [Test]
        public void MSP430Dis_mova_idx()
        {
            AssertCode("mova.a\t-0040(r12),sr", "320CC0FF");
        }

        [Test]
        public void MSP430Dis_mova_indir()
        {
            AssertCode("mova.a\t@sp,pc", "0001");
        }

        [Test]
        public void MSP430Dis_mova_postinc()
        {
            AssertCode("mova.a\t@r4+,pc", "1004");
        }

        [Test]
        public void MSP430Dis_popm()
        {
            AssertCode("popm.w\t#01,r10", "0A17");
        }

        [Test]
        public void MSP430Dis_repeat_const()
        {
            AssertCode("rpt #14 rrax.w\tr12", "4d180c11");
            AssertCode("rpt r13 rrax.w\tr12", "CD180c11");
        }

        [Test]
        public void MSP430Dis_rrum()
        {
            AssertCode("rrum.w\t#01,r12", "5C03");
        }

        [Test]
        public void MSP430XDis_mov_to_pc()
        {
            AssertCode("br\tr6", "0046");
        }
    }
}