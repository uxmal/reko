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
using Reko.Arch.M6800.M6809;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.M6800
{
    [TestFixture]
    public class M6809DisassemblerTests : DisassemblerTestBase<M6809Instruction>
    {
        private Reko.Arch.M6800.M6809Architecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new Reko.Arch.M6800.M6809Architecture("m6809");
            this.addr = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string expectedAsm, string hexInstr)
        {
            var instr = DisassembleHexBytes(hexInstr);
            Assert.AreEqual(expectedAsm, instr.ToString());
        }

        [Test]
        public void M6809Dis_neg()
        {
            AssertCode("neg\t>$42", "0042");
        }

        [Test]
        public void M6809Dis_com_const_offset_5()
        {
            AssertCode("com\t-$10,s", "6370");
        }

        [Test]
        public void M6809Dis_lsr_const_offset_0()
        {
            AssertCode("lsr\t,x", "6484");
        }

        [Test]
        public void M6809Dis_ror_const_offset_8()
        {
            AssertCode("ror\t-$80,y", "66A880");
        }

        [Test]
        public void M6809Dis_asr_const_offset_16()
        {
            AssertCode("asr\t$D000,u", "67C9D000");
        }

        [Test]
        public void M6809Dis_lsl_accumulator_offset_a()
        {
            AssertCode("lsl\ta,s", "68E6");
        }

        [Test]
        public void M6809Dis_mul()
        {
            AssertCode("mul", "3D");
        }

        [Test]
        public void M6809Dis_rol_accumulator_offset_b()
        {
            AssertCode("rol\tb,x", "6985");
        }

        [Test]
        public void M6809Dis_dec_accumulator_offset_d()
        {
            AssertCode("dec\td,x", "6A8B");
        }

        [Test]
        public void M6809Dis_inc_postInc_1()
        {
            AssertCode("inc\t,y+", "6CA0");
        }

        [Test]
        public void M6809Dis_inc_pcrel_8()
        {
            AssertCode("inc\t-$80,pcr", "6CCC80");
        }

        [Test]
        public void M6809Dis_inc_pcrel_16()
        {
            AssertCode("inc\t-$8000,pcr", "6CED8000");
        }

        [Test]
        public void M6809Dis_tst_pcrel_16()
        {
            AssertCode("tst\t[-$10,s]", "6DF8F0");
        }

        [Test]
        public void M6809Dis_jmp_extind_16()
        {
            AssertCode("jmp\t[$8080]", "6E9F8080");
        }

        [Test]
        public void M6809Dis_clr_ext_16()
        {
            AssertCode("clr\t$DCBA", "7FDCBA");
        }

        [Test]
        public void M6809Dis_suba_imm()
        {
            AssertCode("suba\t#$8A", "808A");
        }

        [Test]
        public void M6809Dis_lbra_imm()
        {
            AssertCode("lbra\t0100", "16FFFD");
        }

        [Test]
        public void M6809Dis_exg()
        {
            AssertCode("exg\tx,u", "1E13");
        }
    }
}
