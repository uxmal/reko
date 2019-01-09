#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Arch.RiscV;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.RiscV
{
    [TestFixture]
    public class RiscVDisassemblerTests : DisassemblerTestBase<RiscVInstruction>
    {
        private RiscVArchitecture arch;

        public RiscVDisassemblerTests()
        {
            this.arch = new RiscVArchitecture("riscV");
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new LeImageWriter(bytes);
        }

        private void AssertCode(string sExp, uint uInstr)
        {
            var i = DisassembleWord(uInstr);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void DumpWord(uint uInstr)
        {
            var sb = new StringBuilder();
            for (uint m = 0x80000000; m != 0; m >>= 1)
            {
                sb.Append((uInstr & m) != 0 ? '1' : '0');
            }
            Debug.Print("AssertCode(\"@@@\", \"{0}\");", sb);
        }

        private void AssertCode(string sExp, string bits)
        {
            var i = DisassembleBits(bits);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void RiscV_dasm_lui()
        {
            AssertCode("lui\tt6,0x00012345", "00010010001101000101 11111 01101 11");
        }

        [Test]
        public void RiscV_dasm_sh()
        {
            AssertCode("sh\ts5,sp,386", "0001100 10101 00010 001 00010 01000 11");
        }

        [Test]
        public void RiscV_dasm_lb()
        {
            AssertCode("lb\tgp,sp,-1936", "100001110000 00010 000 00011 00000 11");
        }

        [Test]
        public void RiscV_dasm_addi()
        {
            AssertCode("addi\tsp,sp,-448", "1110010000000001000000010 00100 11");
        }

        [Test]
        public void RiscV_dasm_auipc()
        {
            AssertCode("auipc\tgp,0x000FFFFD", "11111111111111111 101 00011 00101 11");
        }

        [Test]
        public void RiscV_dasm_jal()
        {
            AssertCode("jal\tzero,00000000000FF1F4", 0x9F4FF06F);
        }

        [Test]
        public void RiscV_dasm_sd()
        {
            AssertCode("sd\ts5,sp,392", 0x19513423u);
        }

        [Test]
        public void RiscV_dasm_addiw()
        {
            AssertCode("addiw\ta5,a5,8", 0x0087879Bu);
        }

        [Test]
        public void RiscV_dasm_x1()
        {
            AssertCode("beq\ta0,a4,0000000000100128", 0x12E50463u);
        }

        [Test]
        public void RiscV_dasm_x4()
        {
            // AssertCode("@@@", 0x02079793u);
        }

        [Test]
        public void RiscV_dasm_jalr()
        {
            AssertCode("jalr\tzero,a5,0", 0x00078067u);
        }

        [Test]
        public void RiscV_dasm_or()
        {
            AssertCode("or\ts0,s0,s8", 0x01846433u);
        }

        [Test]
        public void RiscV_dasm_aa()
        {
            AssertCode("add\ta5,a5,a4", 0x00E787B3u);
        }

        [Test]
        public void RiscV_dasm_add()
        {
            AssertCode("and\ta5,s0,a5", 0x00F477B3u);
        }

        [Test]
        public void RiscV_dasm_subw()
        {
            AssertCode("subw\ta3,a3,a5", 0x40F686BBu);
        }

        [Test]
        public void RiscV_dasm_srliw()
        {
            AssertCode("srliw\ta4,a5,0x00000001", 0x0017D71Bu);
        }

        [Test]
        public void RiscV_dasm_lbu()
        {
            AssertCode("lbu\ta4,s2,0", 0x00094703u);
        }

        [Test]
        public void RiscV_dasm_beq()
        {
            AssertCode("beq\ta1,a5,0000000000100000", 0x00F58063u);
        }

        [Test]
        public void RiscV_dasm_flw()
        {
            AssertCode("flw\tfa4,s2,52", 0x03492707u);
        }

        [Test]
        public void RiscV_dasm_fmv_s_x()
        {
            AssertCode("fmv.s.x\tfa5,zero", 0xF00007D3u);
        }

        [Test]
        public void RiscV_dasm_fmv_d_x()
        {
            AssertCode("fmv.d.x\tfa4,a4", 0xE2070753u);
        }

        [Test]
        public void RiscV_dasm_lwu()
        {
            AssertCode("lwu\ta4,s0,4", 0x00446703u);
        }

        [Test]
        public void RiscV_dasm_fcvt_d_s()
        {
            AssertCode("fcvt.d.s\tfa4,fa4", 0x42070753u);
        }

        [Test]
        public void RiscV_dasm_feq_s()
        {
            // 1010000 011110111001001111 10100 11
            AssertCode("feq.s\ta5,fa4,fa5", 0xA0F727D3u);
        }

        [Test]
        public void RiscV_dasm_fmadd()
        {
            AssertCode("fmadd.s\tfs10,ft7,fs1,fa6", 0x8293FD43);
        }
    }
}
