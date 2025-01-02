#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Arch.Vax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using NUnit.Framework;
using Reko.Core.Types;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.Vax
{
    [TestFixture]
    public class VaxDisassemblerTests : DisassemblerTestBase<VaxInstruction>
    {
        private readonly VaxArchitecture arch;
        private readonly Address addr;

        public VaxDisassemblerTests()
        {
            this.arch = new VaxArchitecture(CreateServiceContainer(), "vax", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExp, params byte[] bytes)
        {
            var i = DisassembleBytes(bytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void AssertCode(string sExp, string bytes)
        {
            var i = DisassembleHexBytes(bytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void VaxDis_movab_pcrelative()
        {
            AssertCode("movab\t000FFFFE,r2", 0x9E, 0xAF, 0xFB, 0x52);
        }

        [Test]
        public void VaxDis_movl_reg_reg()
        {
            AssertCode("movl\tr0,r1", 0xD0, 0x50, 0x51);
        }

        [Test]
        public void VaxDis_movl_reg_longwordDisplacementDeferred()
        {
            AssertCode("movl\tr9,001037E5", "D0 59 EF DE 37 00 00");
        }

        [Test]
        public void VaxDis_movw_IndexedDisplacementDeferred()
        {
            AssertCode("movw\t00120454[r0],r0", "b0 40 ef 4d 04 02 00 50");
        }

        [Test]
        public void VaxDis_pushl_imm()
        {
            AssertCode("pushl\t#00000000", 0xDD, 0x00);
        }

        [Test]
        public void VaxRw_pushr()
        {
            AssertCode("pushr\t#0003", "BB03");
        }

        [Test]
        public void VaxDis_brb()
        {
            AssertCode("brb\t00100005", 0x11, 0x03);
            AssertCode("brw\t00100005", 0x31, 0x02, 0x00);
        }

        [Test]
        public void VaxDis_beq()
        {
            AssertCode("beql\t0010001A", 0x13, 0x18);
        }

        [Test]
        public void VaxDis_jsb()
        {
            AssertCode("jsb\t00100000", 0x16, 0xCF, 0xFC, 0xFF);
        }

        [Test]
        public void VaxDis_cvtlf()
        {
            AssertCode("cvtlf\t+6F(r11),ap", 0x4E, 0xAB, 0x6F, 0x5C);
        }

        [Test]
        public void VaxDis_cmpf()
        {
            AssertCode("cmpf\tap,#5.0", 0x51, 0x5C, 0x1A);
        }

        [Test]
        public void VaxDis_literalOperand_f32()
        {
            Assert.AreEqual("5.0", VaxDisassembler.LiteralOperand(PrimitiveType.Real32, 0x1A).ToString());
        }

        [Test]
        public void VaxDis_index()
        {
            AssertCode("index\t" +
                "+6F(r11),"+
                "#00000000," +
                "#00000005," +
                "#00000001," + 
                "#00000000," +
                "ap",
                0x0A, 0xAB, 0x6F, 0x00, 0x05, 0x01, 0x00, 0x5C);
        }

        [Test]
        public void VaxDis_InvalidWriteToConstant()
        {
            AssertCode("Invalid", 0xD0, 0x50, 0x03);
        }

        [Test]
        public void VaxDis_calls_DisplacementDeferred()
        {
            AssertCode("calls\t#03,@00106F82", 0xFB, 0x03, 0xFF, 0x7B, 0x6F, 0x00, 0x00);
        }
    }
}
