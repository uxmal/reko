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

        [Test]
        public void RiscV_dasm_c_addi4spn()
        {
            AssertCode("c.addi4spn\ta1,0x000000000000009C", 0x696C);
        }

        [Test]
        public void RiscV_dasm_c_slli()
        {
            AssertCode("c.slli\ta0,0x3B", 0x756E);
        }

        [Test]
        public void RiscV_dasm_c_addi()
        {
            AssertCode("c.addi\ta0,00000002", 0x0000E509);
        }

        // Reko: a decoder for RiscV instruction 00008082 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008082()
        {
            AssertCode("@@@", 0x00008082);
        }

        // Reko: a decoder for RiscV instruction 000087AA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087AA()
        {
            AssertCode("@@@", 0x000087AA);
        }

        // Reko: a decoder for RiscV instruction 00007149 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00007149()
        {
            AssertCode("@@@", 0x00007149);
        }

        // Reko: a decoder for RiscV instruction 00006388 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006388()
        {
            AssertCode("@@@", 0x00006388);
        }

        // Reko: a decoder for RiscV instruction 0000E2D2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E2D2()
        {
            AssertCode("@@@", 0x0000E2D2);
        }

        // Reko: a decoder for RiscV instruction 0000F262 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F262()
        {
            AssertCode("@@@", 0x0000F262);
        }

        // Reko: a decoder for RiscV instruction 00008C2A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008C2A()
        {
            AssertCode("@@@", 0x00008C2A);
        }

        // Reko: a decoder for RiscV instruction 0000FDBE at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FDBE()
        {
            AssertCode("@@@", 0x0000FDBE);
        }

        // Reko: a decoder for RiscV instruction 0000F686 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F686()
        {
            AssertCode("@@@", 0x0000F686);
        }

        // Reko: a decoder for RiscV instruction 0000F65E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F65E()
        {
            AssertCode("@@@", 0x0000F65E);
        }

        // Reko: a decoder for RiscV instruction 0000F2A2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F2A2()
        {
            AssertCode("@@@", 0x0000F2A2);
        }

        // Reko: a decoder for RiscV instruction 00008BAE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008BAE()
        {
            AssertCode("@@@", 0x00008BAE);
        }

        // Reko: a decoder for RiscV instruction 0000EEA6 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EEA6()
        {
            AssertCode("@@@", 0x0000EEA6);
        }

        // Reko: a decoder for RiscV instruction 0000EACA at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EACA()
        {
            AssertCode("@@@", 0x0000EACA);
        }

        // Reko: a decoder for RiscV instruction 0000E6CE at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E6CE()
        {
            AssertCode("@@@", 0x0000E6CE);
        }

        // Reko: a decoder for RiscV instruction 0000FE56 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FE56()
        {
            AssertCode("@@@", 0x0000FE56);
        }

        // Reko: a decoder for RiscV instruction 0000FA5A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FA5A()
        {
            AssertCode("@@@", 0x0000FA5A);
        }

        // Reko: a decoder for RiscV instruction 0000EE66 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EE66()
        {
            AssertCode("@@@", 0x0000EE66);
        }

        // Reko: a decoder for RiscV instruction 0000EA6A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EA6A()
        {
            AssertCode("@@@", 0x0000EA6A);
        }

        // Reko: a decoder for RiscV instruction 0000E66E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E66E()
        {
            AssertCode("@@@", 0x0000E66E);
        }

        // Reko: a decoder for RiscV instruction 0000E388 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E388()
        {
            AssertCode("@@@", 0x0000E388);
        }

        // Reko: a decoder for RiscV instruction 00004519 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004519()
        {
            AssertCode("@@@", 0x00004519);
        }

        // Reko: a decoder for RiscV instruction 0000853E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000853E()
        {
            AssertCode("@@@", 0x0000853E);
        }

        // Reko: a decoder for RiscV instruction 0000F0BE at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F0BE()
        {
            AssertCode("@@@", 0x0000F0BE);
        }

        // Reko: a decoder for RiscV instruction 00004971 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004971()
        {
            AssertCode("@@@", 0x00004971);
        }

        // Reko: a decoder for RiscV instruction 0000E791 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E791()
        {
            AssertCode("@@@", 0x0000E791);
        }

        // Reko: a decoder for RiscV instruction 000085CE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085CE()
        {
            AssertCode("@@@", 0x000085CE);
        }

        // Reko: a decoder for RiscV instruction 00008522 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008522()
        {
            AssertCode("@@@", 0x00008522);
        }

        [Test]
        public void RiscV_dasm_addiw_negative()
        {
            AssertCode("c.addiw\ts0,FFFFFFFFFFFFFFFF", 0x0000347D);
        }

        // Reko: a decoder for RiscV instruction 000085DE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085DE()
        {
            AssertCode("@@@", 0x000085DE);
        }

        // Reko: a decoder for RiscV instruction 00008562 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008562()
        {
            AssertCode("@@@", 0x00008562);
        }

        // Reko: a decoder for RiscV instruction 00004701 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004701()
        {
            AssertCode("@@@", 0x00004701);
        }

        // Reko: a decoder for RiscV instruction 0000463D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000463D()
        {
            AssertCode("@@@", 0x0000463D);
        }

        // Reko: a decoder for RiscV instruction 00004BD4 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004BD4()
        {
            AssertCode("@@@", 0x00004BD4);
        }

        // Reko: a decoder for RiscV instruction 0000639C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000639C()
        {
            AssertCode("@@@", 0x0000639C);
        }

        // Reko: a decoder for RiscV instruction 0000FBF5 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FBF5()
        {
            AssertCode("@@@", 0x0000FBF5);
        }

        // Reko: a decoder for RiscV instruction 00004785 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004785()
        {
            AssertCode("@@@", 0x00004785);
        }

        // Reko: a decoder for RiscV instruction 0000439C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000439C()
        {
            AssertCode("@@@", 0x0000439C);
        }

        // Reko: a decoder for RiscV instruction 00004689 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004689()
        {
            AssertCode("@@@", 0x00004689);
        }

        // Reko: a decoder for RiscV instruction 00004685 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004685()
        {
            AssertCode("@@@", 0x00004685);
        }

        // Reko: a decoder for RiscV instruction 000087EE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087EE()
        {
            AssertCode("@@@", 0x000087EE);
        }

        // Reko: a decoder for RiscV instruction 00006790 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006790()
        {
            AssertCode("@@@", 0x00006790);
        }

        // Reko: a decoder for RiscV instruction 00006394 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006394()
        {
            AssertCode("@@@", 0x00006394);
        }

        // Reko: a decoder for RiscV instruction 0000C601 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C601()
        {
            AssertCode("@@@", 0x0000C601);
        }

        // Reko: a decoder for RiscV instruction 00006B9C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006B9C()
        {
            AssertCode("@@@", 0x00006B9C);
        }

        // Reko: a decoder for RiscV instruction 0000C391 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C391()
        {
            AssertCode("@@@", 0x0000C391);
        }

        // Reko: a decoder for RiscV instruction 000087B6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087B6()
        {
            AssertCode("@@@", 0x000087B6);
        }

        // Reko: a decoder for RiscV instruction 0000F6FD at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F6FD()
        {
            AssertCode("@@@", 0x0000F6FD);
        }

        // Reko: a decoder for RiscV instruction 0000401C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000401C()
        {
            AssertCode("@@@", 0x0000401C);
        }

        // Reko: a decoder for RiscV instruction 0000649C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000649C()
        {
            AssertCode("@@@", 0x0000649C);
        }

        // Reko: a decoder for RiscV instruction 0000C789 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C789()
        {
            AssertCode("@@@", 0x0000C789);
        }

        // Reko: a decoder for RiscV instruction 00004FD4 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004FD4()
        {
            AssertCode("@@@", 0x00004FD4);
        }

        // Reko: a decoder for RiscV instruction 00008F55 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008F55()
        {
            AssertCode("@@@", 0x00008F55);
        }

        // Reko: a decoder for RiscV instruction 0000FFED at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FFED()
        {
            AssertCode("@@@", 0x0000FFED);
        }

        // Reko: a decoder for RiscV instruction 0000689C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000689C()
        {
            AssertCode("@@@", 0x0000689C);
        }

        // Reko: a decoder for RiscV instruction 00008E55 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008E55()
        {
            AssertCode("@@@", 0x00008E55);
        }

        // Reko: a decoder for RiscV instruction 00004681 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004681()
        {
            AssertCode("@@@", 0x00004681);
        }

        // Reko: a decoder for RiscV instruction 00004F8C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004F8C()
        {
            AssertCode("@@@", 0x00004F8C);
        }

        // Reko: a decoder for RiscV instruction 00008ECD at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008ECD()
        {
            AssertCode("@@@", 0x00008ECD);
        }

        // Reko: a decoder for RiscV instruction 00008F51 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008F51()
        {
            AssertCode("@@@", 0x00008F51);
        }

        // Reko: a decoder for RiscV instruction 0000CF89 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CF89()
        {
            AssertCode("@@@", 0x0000CF89);
        }

        // Reko: a decoder for RiscV instruction 0000EF89 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EF89()
        {
            AssertCode("@@@", 0x0000EF89);
        }

        // Reko: a decoder for RiscV instruction 00004CD8 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004CD8()
        {
            AssertCode("@@@", 0x00004CD8);
        }

        // Reko: a decoder for RiscV instruction 00009BF5 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009BF5()
        {
            AssertCode("@@@", 0x00009BF5);
        }

        // Reko: a decoder for RiscV instruction 00009B75 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009B75()
        {
            AssertCode("@@@", 0x00009B75);
        }

        // Reko: a decoder for RiscV instruction 00004CDC at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004CDC()
        {
            AssertCode("@@@", 0x00004CDC);
        }

        // Reko: a decoder for RiscV instruction 0000CB19 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CB19()
        {
            AssertCode("@@@", 0x0000CB19);
        }

        // Reko: a decoder for RiscV instruction 00008FD9 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008FD9()
        {
            AssertCode("@@@", 0x00008FD9);
        }

        // Reko: a decoder for RiscV instruction 0000CB99 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CB99()
        {
            AssertCode("@@@", 0x0000CB99);
        }

        // Reko: a decoder for RiscV instruction 0000C719 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C719()
        {
            AssertCode("@@@", 0x0000C719);
        }

        // Reko: a decoder for RiscV instruction 000050DC at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_000050DC()
        {
            AssertCode("@@@", 0x000050DC);
        }

        // Reko: a decoder for RiscV instruction 00006709 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006709()
        {
            AssertCode("@@@", 0x00006709);
        }

        // Reko: a decoder for RiscV instruction 00004C98 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004C98()
        {
            AssertCode("@@@", 0x00004C98);
        }

        // Reko: a decoder for RiscV instruction 0000854E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000854E()
        {
            AssertCode("@@@", 0x0000854E);
        }

        // Reko: a decoder for RiscV instruction 000089AA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000089AA()
        {
            AssertCode("@@@", 0x000089AA);
        }

        // Reko: a decoder for RiscV instruction 000085AA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085AA()
        {
            AssertCode("@@@", 0x000085AA);
        }

        // Reko: a decoder for RiscV instruction 0000611C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000611C()
        {
            AssertCode("@@@", 0x0000611C);
        }

        // Reko: a decoder for RiscV instruction 00006088 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006088()
        {
            AssertCode("@@@", 0x00006088);
        }

        // Reko: a decoder for RiscV instruction 00004621 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004621()
        {
            AssertCode("@@@", 0x00004621);
        }

        // Reko: a decoder for RiscV instruction 00008B62 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008B62()
        {
            AssertCode("@@@", 0x00008B62);
        }

        // Reko: a decoder for RiscV instruction 00005CFD at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00005CFD()
        {
            AssertCode("@@@", 0x00005CFD);
        }

        // Reko: a decoder for RiscV instruction 0000609C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000609C()
        {
            AssertCode("@@@", 0x0000609C);
        }

        // Reko: a decoder for RiscV instruction 00006318 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006318()
        {
            AssertCode("@@@", 0x00006318);
        }

        // Reko: a decoder for RiscV instruction 000096E2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000096E2()
        {
            AssertCode("@@@", 0x000096E2);
        }

        // Reko: a decoder for RiscV instruction 00004350 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004350()
        {
            AssertCode("@@@", 0x00004350);
        }

        // Reko: a decoder for RiscV instruction 000097E2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000097E2()
        {
            AssertCode("@@@", 0x000097E2);
        }

        // Reko: a decoder for RiscV instruction 00008E81 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008E81()
        {
            AssertCode("@@@", 0x00008E81);
        }

        // Reko: a decoder for RiscV instruction 00006398 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006398()
        {
            AssertCode("@@@", 0x00006398);
        }

        // Reko: a decoder for RiscV instruction 00004318 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004318()
        {
            AssertCode("@@@", 0x00004318);
        }

        // Reko: a decoder for RiscV instruction 0000854A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000854A()
        {
            AssertCode("@@@", 0x0000854A);
        }

        // Reko: a decoder for RiscV instruction 0000776E at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_0000776E()
        {
            AssertCode("@@@", 0x0000776E);
        }

        // Reko: a decoder for RiscV instruction 000070B6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000070B6()
        {
            AssertCode("@@@", 0x000070B6);
        }

        // Reko: a decoder for RiscV instruction 00007416 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007416()
        {
            AssertCode("@@@", 0x00007416);
        }

        // Reko: a decoder for RiscV instruction 000064F6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000064F6()
        {
            AssertCode("@@@", 0x000064F6);
        }

        // Reko: a decoder for RiscV instruction 00006956 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006956()
        {
            AssertCode("@@@", 0x00006956);
        }

        // Reko: a decoder for RiscV instruction 000069B6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000069B6()
        {
            AssertCode("@@@", 0x000069B6);
        }

        // Reko: a decoder for RiscV instruction 00006A16 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006A16()
        {
            AssertCode("@@@", 0x00006A16);
        }

        // Reko: a decoder for RiscV instruction 00007AF2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007AF2()
        {
            AssertCode("@@@", 0x00007AF2);
        }

        // Reko: a decoder for RiscV instruction 00007B52 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007B52()
        {
            AssertCode("@@@", 0x00007B52);
        }

        // Reko: a decoder for RiscV instruction 00007BB2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007BB2()
        {
            AssertCode("@@@", 0x00007BB2);
        }

        // Reko: a decoder for RiscV instruction 00007C12 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007C12()
        {
            AssertCode("@@@", 0x00007C12);
        }

        // Reko: a decoder for RiscV instruction 00006CF2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006CF2()
        {
            AssertCode("@@@", 0x00006CF2);
        }

        // Reko: a decoder for RiscV instruction 00006D52 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006D52()
        {
            AssertCode("@@@", 0x00006D52);
        }

        // Reko: a decoder for RiscV instruction 00006DB2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006DB2()
        {
            AssertCode("@@@", 0x00006DB2);
        }

        // Reko: a decoder for RiscV instruction 00006175 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006175()
        {
            AssertCode("@@@", 0x00006175);
        }

        // Reko: a decoder for RiscV instruction 000056FD at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000056FD()
        {
            AssertCode("@@@", 0x000056FD);
        }

        // Reko: a decoder for RiscV instruction 0000C394 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C394()
        {
            AssertCode("@@@", 0x0000C394);
        }

        // Reko: a decoder for RiscV instruction 00008D0A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008D0A()
        {
            AssertCode("@@@", 0x00008D0A);
        }

        // Reko: a decoder for RiscV instruction 0000A01D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A01D()
        {
            AssertCode("@@@", 0x0000A01D);
        }

        // Reko: a decoder for RiscV instruction 00008D6E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008D6E()
        {
            AssertCode("@@@", 0x00008D6E);
        }

        // Reko: a decoder for RiscV instruction 00008C66 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008C66()
        {
            AssertCode("@@@", 0x00008C66);
        }

        // Reko: a decoder for RiscV instruction 0000C7BD at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C7BD()
        {
            AssertCode("@@@", 0x0000C7BD);
        }

        // Reko: a decoder for RiscV instruction 00008DBE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008DBE()
        {
            AssertCode("@@@", 0x00008DBE);
        }

        // Reko: a decoder for RiscV instruction 000085EE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085EE()
        {
            AssertCode("@@@", 0x000085EE);
        }

        // Reko: a decoder for RiscV instruction 00008CAA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008CAA()
        {
            AssertCode("@@@", 0x00008CAA);
        }

        // Reko: a decoder for RiscV instruction 00004615 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004615()
        {
            AssertCode("@@@", 0x00004615);
        }

        // Reko: a decoder for RiscV instruction 0000862A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000862A()
        {
            AssertCode("@@@", 0x0000862A);
        }

        // Reko: a decoder for RiscV instruction 0000FBD9 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FBD9()
        {
            AssertCode("@@@", 0x0000FBD9);
        }

        // Reko: a decoder for RiscV instruction 00006782 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006782()
        {
            AssertCode("@@@", 0x00006782);
        }

        // Reko: a decoder for RiscV instruction 000077C2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000077C2()
        {
            AssertCode("@@@", 0x000077C2);
        }

        // Reko: a decoder for RiscV instruction 0000B335 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B335()
        {
            AssertCode("@@@", 0x0000B335);
        }

        // Reko: a decoder for RiscV instruction 0000BFB1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFB1()
        {
            AssertCode("@@@", 0x0000BFB1);
        }

        // Reko: a decoder for RiscV instruction 0000B145 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B145()
        {
            AssertCode("@@@", 0x0000B145);
        }

        // Reko: a decoder for RiscV instruction 0000B535 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B535()
        {
            AssertCode("@@@", 0x0000B535);
        }

        // Reko: a decoder for RiscV instruction 0000C29C at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C29C()
        {
            AssertCode("@@@", 0x0000C29C);
        }

        // Reko: a decoder for RiscV instruction 0000BDE9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BDE9()
        {
            AssertCode("@@@", 0x0000BDE9);
        }

        // Reko: a decoder for RiscV instruction 0000E398 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E398()
        {
            AssertCode("@@@", 0x0000E398);
        }

        // Reko: a decoder for RiscV instruction 0000BEBD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BEBD()
        {
            AssertCode("@@@", 0x0000BEBD);
        }

        // Reko: a decoder for RiscV instruction 00009B4A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009B4A()
        {
            AssertCode("@@@", 0x00009B4A);
        }

        // Reko: a decoder for RiscV instruction 0000A021 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A021()
        {
            AssertCode("@@@", 0x0000A021);
        }

        // Reko: a decoder for RiscV instruction 0000E30D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E30D()
        {
            AssertCode("@@@", 0x0000E30D);
        }

        // Reko: a decoder for RiscV instruction 0000D7FD at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D7FD()
        {
            AssertCode("@@@", 0x0000D7FD);
        }

        // Reko: a decoder for RiscV instruction 0000A029 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A029()
        {
            AssertCode("@@@", 0x0000A029);
        }

        // Reko: a decoder for RiscV instruction 0000688C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000688C()
        {
            AssertCode("@@@", 0x0000688C);
        }

        // Reko: a decoder for RiscV instruction 0000855E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000855E()
        {
            AssertCode("@@@", 0x0000855E);
        }

        // Reko: a decoder for RiscV instruction 0000865E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000865E()
        {
            AssertCode("@@@", 0x0000865E);
        }

        // Reko: a decoder for RiscV instruction 0000F575 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F575()
        {
            AssertCode("@@@", 0x0000F575);
        }

        // Reko: a decoder for RiscV instruction 0000BFD1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFD1()
        {
            AssertCode("@@@", 0x0000BFD1);
        }

        // Reko: a decoder for RiscV instruction 0000648C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000648C()
        {
            AssertCode("@@@", 0x0000648C);
        }

        // Reko: a decoder for RiscV instruction 0000E91C at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E91C()
        {
            AssertCode("@@@", 0x0000E91C);
        }

        // Reko: a decoder for RiscV instruction 0000E514 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E514()
        {
            AssertCode("@@@", 0x0000E514);
        }

        // Reko: a decoder for RiscV instruction 0000CD18 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000CD18()
        {
            AssertCode("@@@", 0x0000CD18);
        }

        // Reko: a decoder for RiscV instruction 0000E11C at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E11C()
        {
            AssertCode("@@@", 0x0000E11C);
        }

        // Reko: a decoder for RiscV instruction 0000B371 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B371()
        {
            AssertCode("@@@", 0x0000B371);
        }

        // Reko: a decoder for RiscV instruction 0000B1A5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B1A5()
        {
            AssertCode("@@@", 0x0000B1A5);
        }

        // Reko: a decoder for RiscV instruction 000047BD at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000047BD()
        {
            AssertCode("@@@", 0x000047BD);
        }

        // Reko: a decoder for RiscV instruction 00004B01 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004B01()
        {
            AssertCode("@@@", 0x00004B01);
        }

        // Reko: a decoder for RiscV instruction 000085DA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085DA()
        {
            AssertCode("@@@", 0x000085DA);
        }

        // Reko: a decoder for RiscV instruction 0000892A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000892A()
        {
            AssertCode("@@@", 0x0000892A);
        }

        // Reko: a decoder for RiscV instruction 00004014 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004014()
        {
            AssertCode("@@@", 0x00004014);
        }

        // Reko: a decoder for RiscV instruction 00006705 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006705()
        {
            AssertCode("@@@", 0x00006705);
        }

        // Reko: a decoder for RiscV instruction 00008FF5 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008FF5()
        {
            AssertCode("@@@", 0x00008FF5);
        }

        // Reko: a decoder for RiscV instruction 000085A2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085A2()
        {
            AssertCode("@@@", 0x000085A2);
        }

        // Reko: a decoder for RiscV instruction 0000C531 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C531()
        {
            AssertCode("@@@", 0x0000C531);
        }

        // Reko: a decoder for RiscV instruction 0000864E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000864E()
        {
            AssertCode("@@@", 0x0000864E);
        }

        // Reko: a decoder for RiscV instruction 0000D575 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D575()
        {
            AssertCode("@@@", 0x0000D575);
        }

        // Reko: a decoder for RiscV instruction 0000D57D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D57D()
        {
            AssertCode("@@@", 0x0000D57D);
        }

        // Reko: a decoder for RiscV instruction 0000B7D5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7D5()
        {
            AssertCode("@@@", 0x0000B7D5);
        }

        // Reko: a decoder for RiscV instruction 00008D8A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008D8A()
        {
            AssertCode("@@@", 0x00008D8A);
        }

        // Reko: a decoder for RiscV instruction 0000B585 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B585()
        {
            AssertCode("@@@", 0x0000B585);
        }

        // Reko: a decoder for RiscV instruction 0000E171 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E171()
        {
            AssertCode("@@@", 0x0000E171);
        }

        // Reko: a decoder for RiscV instruction 0000F965 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F965()
        {
            AssertCode("@@@", 0x0000F965);
        }

        // Reko: a decoder for RiscV instruction 0000855A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000855A()
        {
            AssertCode("@@@", 0x0000855A);
        }

        // Reko: a decoder for RiscV instruction 0000BB1D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB1D()
        {
            AssertCode("@@@", 0x0000BB1D);
        }

        // Reko: a decoder for RiscV instruction 0000B5F1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B5F1()
        {
            AssertCode("@@@", 0x0000B5F1);
        }

        // Reko: a decoder for RiscV instruction 0000B755 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B755()
        {
            AssertCode("@@@", 0x0000B755);
        }

        // Reko: a decoder for RiscV instruction 0000E529 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E529()
        {
            AssertCode("@@@", 0x0000E529);
        }

        // Reko: a decoder for RiscV instruction 0000BF41 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF41()
        {
            AssertCode("@@@", 0x0000BF41);
        }

        // Reko: a decoder for RiscV instruction 0000E919 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E919()
        {
            AssertCode("@@@", 0x0000E919);
        }

        // Reko: a decoder for RiscV instruction 0000B7AD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7AD()
        {
            AssertCode("@@@", 0x0000B7AD);
        }

        // Reko: a decoder for RiscV instruction 0000F57D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F57D()
        {
            AssertCode("@@@", 0x0000F57D);
        }

        // Reko: a decoder for RiscV instruction 0000B7C5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7C5()
        {
            AssertCode("@@@", 0x0000B7C5);
        }

        // Reko: a decoder for RiscV instruction 0000BF45 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF45()
        {
            AssertCode("@@@", 0x0000BF45);
        }

        // Reko: a decoder for RiscV instruction 0000B705 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B705()
        {
            AssertCode("@@@", 0x0000B705);
        }

        // Reko: a decoder for RiscV instruction 00006785 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006785()
        {
            AssertCode("@@@", 0x00006785);
        }

        // Reko: a decoder for RiscV instruction 00008B2A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008B2A()
        {
            AssertCode("@@@", 0x00008B2A);
        }

        // Reko: a decoder for RiscV instruction 000086AA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000086AA()
        {
            AssertCode("@@@", 0x000086AA);
        }

        // Reko: a decoder for RiscV instruction 0000A829 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A829()
        {
            AssertCode("@@@", 0x0000A829);
        }

        // Reko: a decoder for RiscV instruction 000097BA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000097BA()
        {
            AssertCode("@@@", 0x000097BA);
        }

        // Reko: a decoder for RiscV instruction 0000BD99 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD99()
        {
            AssertCode("@@@", 0x0000BD99);
        }

        // Reko: a decoder for RiscV instruction 0000BD89 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD89()
        {
            AssertCode("@@@", 0x0000BD89);
        }

        // Reko: a decoder for RiscV instruction 00006380 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006380()
        {
            AssertCode("@@@", 0x00006380);
        }

        // Reko: a decoder for RiscV instruction 0000BFE1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFE1()
        {
            AssertCode("@@@", 0x0000BFE1);
        }

        // Reko: a decoder for RiscV instruction 0000B7C1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7C1()
        {
            AssertCode("@@@", 0x0000B7C1);
        }

        // Reko: a decoder for RiscV instruction 0000B775 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B775()
        {
            AssertCode("@@@", 0x0000B775);
        }

        // Reko: a decoder for RiscV instruction 00006582 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006582()
        {
            AssertCode("@@@", 0x00006582);
        }

        // Reko: a decoder for RiscV instruction 0000880A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000880A()
        {
            AssertCode("@@@", 0x0000880A);
        }

        // Reko: a decoder for RiscV instruction 00008302 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008302()
        {
            AssertCode("@@@", 0x00008302);
        }

        // Reko: a decoder for RiscV instruction 00008D89 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008D89()
        {
            AssertCode("@@@", 0x00008D89);
        }

        // Reko: a decoder for RiscV instruction 0000858D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_0000858D()
        {
            AssertCode("@@@", 0x0000858D);
        }

        // Reko: a decoder for RiscV instruction 000095BE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000095BE()
        {
            AssertCode("@@@", 0x000095BE);
        }

        // Reko: a decoder for RiscV instruction 00008585 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008585()
        {
            AssertCode("@@@", 0x00008585);
        }

        // Reko: a decoder for RiscV instruction 0000C981 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C981()
        {
            AssertCode("@@@", 0x0000C981);
        }

        // Reko: a decoder for RiscV instruction 0000EB85 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EB85()
        {
            AssertCode("@@@", 0x0000EB85);
        }

        // Reko: a decoder for RiscV instruction 0000E406 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E406()
        {
            AssertCode("@@@", 0x0000E406);
        }

        // Reko: a decoder for RiscV instruction 0000C799 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C799()
        {
            AssertCode("@@@", 0x0000C799);
        }

        // Reko: a decoder for RiscV instruction 00006308 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006308()
        {
            AssertCode("@@@", 0x00006308);
        }

        // Reko: a decoder for RiscV instruction 00009782 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009782()
        {
            AssertCode("@@@", 0x00009782);
        }

        // Reko: a decoder for RiscV instruction 000060A2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000060A2()
        {
            AssertCode("@@@", 0x000060A2);
        }

        // Reko: a decoder for RiscV instruction 0000E822 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E822()
        {
            AssertCode("@@@", 0x0000E822);
        }

        // Reko: a decoder for RiscV instruction 0000EC06 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC06()
        {
            AssertCode("@@@", 0x0000EC06);
        }

        // Reko: a decoder for RiscV instruction 0000E426 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E426()
        {
            AssertCode("@@@", 0x0000E426);
        }

        // Reko: a decoder for RiscV instruction 0000E04A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E04A()
        {
            AssertCode("@@@", 0x0000E04A);
        }

        // Reko: a decoder for RiscV instruction 0000C415 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C415()
        {
            AssertCode("@@@", 0x0000C415);
        }

        // Reko: a decoder for RiscV instruction 000084AE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000084AE()
        {
            AssertCode("@@@", 0x000084AE);
        }

        // Reko: a decoder for RiscV instruction 0000A019 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A019()
        {
            AssertCode("@@@", 0x0000A019);
        }

        // Reko: a decoder for RiscV instruction 00006000 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006000()
        {
            AssertCode("@@@", 0x00006000);
        }

        // Reko: a decoder for RiscV instruction 0000C00D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C00D()
        {
            AssertCode("@@@", 0x0000C00D);
        }

        // Reko: a decoder for RiscV instruction 0000641C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000641C()
        {
            AssertCode("@@@", 0x0000641C);
        }

        // Reko: a decoder for RiscV instruction 0000608C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000608C()
        {
            AssertCode("@@@", 0x0000608C);
        }

        // Reko: a decoder for RiscV instruction 0000D96D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D96D()
        {
            AssertCode("@@@", 0x0000D96D);
        }

        // Reko: a decoder for RiscV instruction 0000481C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000481C()
        {
            AssertCode("@@@", 0x0000481C);
        }

        // Reko: a decoder for RiscV instruction 0000CB91 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CB91()
        {
            AssertCode("@@@", 0x0000CB91);
        }

        // Reko: a decoder for RiscV instruction 000060E2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000060E2()
        {
            AssertCode("@@@", 0x000060E2);
        }

        // Reko: a decoder for RiscV instruction 00006442 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006442()
        {
            AssertCode("@@@", 0x00006442);
        }

        // Reko: a decoder for RiscV instruction 000064A2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000064A2()
        {
            AssertCode("@@@", 0x000064A2);
        }

        // Reko: a decoder for RiscV instruction 00006902 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006902()
        {
            AssertCode("@@@", 0x00006902);
        }

        // Reko: a decoder for RiscV instruction 00006105 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006105()
        {
            AssertCode("@@@", 0x00006105);
        }

        // Reko: a decoder for RiscV instruction 00007159 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00007159()
        {
            AssertCode("@@@", 0x00007159);
        }

        // Reko: a decoder for RiscV instruction 0000F0A2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F0A2()
        {
            AssertCode("@@@", 0x0000F0A2);
        }

        // Reko: a decoder for RiscV instruction 0000ECA6 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000ECA6()
        {
            AssertCode("@@@", 0x0000ECA6);
        }

        // Reko: a decoder for RiscV instruction 0000E46E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E46E()
        {
            AssertCode("@@@", 0x0000E46E);
        }

        // Reko: a decoder for RiscV instruction 0000F486 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F486()
        {
            AssertCode("@@@", 0x0000F486);
        }

        // Reko: a decoder for RiscV instruction 0000E8CA at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E8CA()
        {
            AssertCode("@@@", 0x0000E8CA);
        }

        [Test]
        public void RiscV_dasm_c_sdsp()
        {
            AssertCode("c.sdsp\ts3,00000048", 0xE4CE);
        }

        // Reko: a decoder for RiscV instruction 0000E0D2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E0D2()
        {
            AssertCode("@@@", 0x0000E0D2);
        }

        // Reko: a decoder for RiscV instruction 0000FC56 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FC56()
        {
            AssertCode("@@@", 0x0000FC56);
        }

        // Reko: a decoder for RiscV instruction 0000F85A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F85A()
        {
            AssertCode("@@@", 0x0000F85A);
        }

        // Reko: a decoder for RiscV instruction 0000F45E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F45E()
        {
            AssertCode("@@@", 0x0000F45E);
        }

        // Reko: a decoder for RiscV instruction 0000F062 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F062()
        {
            AssertCode("@@@", 0x0000F062);
        }

        // Reko: a decoder for RiscV instruction 0000EC66 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC66()
        {
            AssertCode("@@@", 0x0000EC66);
        }

        // Reko: a decoder for RiscV instruction 0000E86A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E86A()
        {
            AssertCode("@@@", 0x0000E86A);
        }

        // Reko: a decoder for RiscV instruction 00008DB2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008DB2()
        {
            AssertCode("@@@", 0x00008DB2);
        }

        // Reko: a decoder for RiscV instruction 0000842E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000842E()
        {
            AssertCode("@@@", 0x0000842E);
        }

        // Reko: a decoder for RiscV instruction 000084B6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000084B6()
        {
            AssertCode("@@@", 0x000084B6);
        }

        // Reko: a decoder for RiscV instruction 0000C205 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C205()
        {
            AssertCode("@@@", 0x0000C205);
        }

        // Reko: a decoder for RiscV instruction 000097CE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000097CE()
        {
            AssertCode("@@@", 0x000097CE);
        }

        // Reko: a decoder for RiscV instruction 0000E2C9 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E2C9()
        {
            AssertCode("@@@", 0x0000E2C9);
        }

        // Reko: a decoder for RiscV instruction 000099EE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000099EE()
        {
            AssertCode("@@@", 0x000099EE);
        }

        // Reko: a decoder for RiscV instruction 000097CA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000097CA()
        {
            AssertCode("@@@", 0x000097CA);
        }

        // Reko: a decoder for RiscV instruction 0000630C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000630C()
        {
            AssertCode("@@@", 0x0000630C);
        }

        // Reko: a decoder for RiscV instruction 00006298 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006298()
        {
            AssertCode("@@@", 0x00006298);
        }

        // Reko: a decoder for RiscV instruction 0000619C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000619C()
        {
            AssertCode("@@@", 0x0000619C);
        }

        // Reko: a decoder for RiscV instruction 000043DC at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_000043DC()
        {
            AssertCode("@@@", 0x000043DC);
        }

        // Reko: a decoder for RiscV instruction 00004781 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004781()
        {
            AssertCode("@@@", 0x00004781);
        }

        // Reko: a decoder for RiscV instruction 0000A031 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A031()
        {
            AssertCode("@@@", 0x0000A031);
        }

        // Reko: a decoder for RiscV instruction 00006310 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006310()
        {
            AssertCode("@@@", 0x00006310);
        }

        // Reko: a decoder for RiscV instruction 00004250 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004250()
        {
            AssertCode("@@@", 0x00004250);
        }

        // Reko: a decoder for RiscV instruction 000070A6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000070A6()
        {
            AssertCode("@@@", 0x000070A6);
        }

        // Reko: a decoder for RiscV instruction 00007406 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007406()
        {
            AssertCode("@@@", 0x00007406);
        }

        // Reko: a decoder for RiscV instruction 000064E6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000064E6()
        {
            AssertCode("@@@", 0x000064E6);
        }

        // Reko: a decoder for RiscV instruction 00006946 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006946()
        {
            AssertCode("@@@", 0x00006946);
        }

        // Reko: a decoder for RiscV instruction 000069A6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000069A6()
        {
            AssertCode("@@@", 0x000069A6);
        }

        // Reko: a decoder for RiscV instruction 00006A06 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006A06()
        {
            AssertCode("@@@", 0x00006A06);
        }

        // Reko: a decoder for RiscV instruction 00007AE2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007AE2()
        {
            AssertCode("@@@", 0x00007AE2);
        }

        // Reko: a decoder for RiscV instruction 00007B42 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007B42()
        {
            AssertCode("@@@", 0x00007B42);
        }

        // Reko: a decoder for RiscV instruction 00007BA2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007BA2()
        {
            AssertCode("@@@", 0x00007BA2);
        }

        // Reko: a decoder for RiscV instruction 00007C02 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007C02()
        {
            AssertCode("@@@", 0x00007C02);
        }

        // Reko: a decoder for RiscV instruction 00006CE2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006CE2()
        {
            AssertCode("@@@", 0x00006CE2);
        }

        // Reko: a decoder for RiscV instruction 00006D42 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006D42()
        {
            AssertCode("@@@", 0x00006D42);
        }

        // Reko: a decoder for RiscV instruction 00006DA2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006DA2()
        {
            AssertCode("@@@", 0x00006DA2);
        }

        // Reko: a decoder for RiscV instruction 00006165 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006165()
        {
            AssertCode("@@@", 0x00006165);
        }

        // Reko: a decoder for RiscV instruction 0000B741 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B741()
        {
            AssertCode("@@@", 0x0000B741);
        }

        // Reko: a decoder for RiscV instruction 0000974E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000974E()
        {
            AssertCode("@@@", 0x0000974E);
        }

        // Reko: a decoder for RiscV instruction 0000E8C1 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E8C1()
        {
            AssertCode("@@@", 0x0000E8C1);
        }

        // Reko: a decoder for RiscV instruction 00008656 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008656()
        {
            AssertCode("@@@", 0x00008656);
        }

        // Reko: a decoder for RiscV instruction 00004344 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004344()
        {
            AssertCode("@@@", 0x00004344);
        }

        // Reko: a decoder for RiscV instruction 000086A6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000086A6()
        {
            AssertCode("@@@", 0x000086A6);
        }

        // Reko: a decoder for RiscV instruction 0000866E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000866E()
        {
            AssertCode("@@@", 0x0000866E);
        }

        // Reko: a decoder for RiscV instruction 0000C085 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C085()
        {
            AssertCode("@@@", 0x0000C085);
        }

        // Reko: a decoder for RiscV instruction 000087EA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087EA()
        {
            AssertCode("@@@", 0x000087EA);
        }

        // Reko: a decoder for RiscV instruction 0000F0F5 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F0F5()
        {
            AssertCode("@@@", 0x0000F0F5);
        }

        // Reko: a decoder for RiscV instruction 0000BF1D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF1D()
        {
            AssertCode("@@@", 0x0000BF1D);
        }

        // Reko: a decoder for RiscV instruction 0000BF8D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF8D()
        {
            AssertCode("@@@", 0x0000BF8D);
        }

        // Reko: a decoder for RiscV instruction 000047B5 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000047B5()
        {
            AssertCode("@@@", 0x000047B5);
        }

        // Reko: a decoder for RiscV instruction 0000842A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000842A()
        {
            AssertCode("@@@", 0x0000842A);
        }

        // Reko: a decoder for RiscV instruction 00006384 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006384()
        {
            AssertCode("@@@", 0x00006384);
        }

        // Reko: a decoder for RiscV instruction 0000872A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000872A()
        {
            AssertCode("@@@", 0x0000872A);
        }

        // Reko: a decoder for RiscV instruction 000086A2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000086A2()
        {
            AssertCode("@@@", 0x000086A2);
        }

        // Reko: a decoder for RiscV instruction 0000864A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000864A()
        {
            AssertCode("@@@", 0x0000864A);
        }

        // Reko: a decoder for RiscV instruction 00008526 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008526()
        {
            AssertCode("@@@", 0x00008526);
        }

        // Reko: a decoder for RiscV instruction 000047B1 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000047B1()
        {
            AssertCode("@@@", 0x000047B1);
        }

        // Reko: a decoder for RiscV instruction 00008FF9 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008FF9()
        {
            AssertCode("@@@", 0x00008FF9);
        }

        // Reko: a decoder for RiscV instruction 0000EBA9 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EBA9()
        {
            AssertCode("@@@", 0x0000EBA9);
        }

        // Reko: a decoder for RiscV instruction 0000E022 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E022()
        {
            AssertCode("@@@", 0x0000E022);
        }

        [Test]
        public void RiscV_dasm_c_beqz()
        {
            AssertCode("c.beqz\t000@@@", 0x0000C121);
        }

        // Reko: a decoder for RiscV instruction 0000701C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000701C()
        {
            AssertCode("@@@", 0x0000701C);
        }

        // Reko: a decoder for RiscV instruction 00007414 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007414()
        {
            AssertCode("@@@", 0x00007414);
        }

        // Reko: a decoder for RiscV instruction 000097B6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000097B6()
        {
            AssertCode("@@@", 0x000097B6);
        }

        // Reko: a decoder for RiscV instruction 0000C709 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C709()
        {
            AssertCode("@@@", 0x0000C709);
        }

        // Reko: a decoder for RiscV instruction 00007818 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007818()
        {
            AssertCode("@@@", 0x00007818);
        }

        // Reko: a decoder for RiscV instruction 00007C14 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007C14()
        {
            AssertCode("@@@", 0x00007C14);
        }

        // Reko: a decoder for RiscV instruction 00009736 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009736()
        {
            AssertCode("@@@", 0x00009736);
        }

        // Reko: a decoder for RiscV instruction 00006034 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006034()
        {
            AssertCode("@@@", 0x00006034);
        }

        // Reko: a decoder for RiscV instruction 00008F15 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008F15()
        {
            AssertCode("@@@", 0x00008F15);
        }

        // Reko: a decoder for RiscV instruction 0000E719 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E719()
        {
            AssertCode("@@@", 0x0000E719);
        }

        // Reko: a decoder for RiscV instruction 0000CC10 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000CC10()
        {
            AssertCode("@@@", 0x0000CC10);
        }

        // Reko: a decoder for RiscV instruction 00006402 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006402()
        {
            AssertCode("@@@", 0x00006402);
        }

        // Reko: a decoder for RiscV instruction 0000711D at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_0000711D()
        {
            AssertCode("@@@", 0x0000711D);
        }

        // Reko: a decoder for RiscV instruction 0000F05A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F05A()
        {
            AssertCode("@@@", 0x0000F05A);
        }

        // Reko: a decoder for RiscV instruction 0000EC86 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC86()
        {
            AssertCode("@@@", 0x0000EC86);
        }

        // Reko: a decoder for RiscV instruction 0000EC3E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC3E()
        {
            AssertCode("@@@", 0x0000EC3E);
        }

        // Reko: a decoder for RiscV instruction 0000E8A2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E8A2()
        {
            AssertCode("@@@", 0x0000E8A2);
        }

        // Reko: a decoder for RiscV instruction 0000E4A6 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E4A6()
        {
            AssertCode("@@@", 0x0000E4A6);
        }

        // Reko: a decoder for RiscV instruction 0000E0CA at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E0CA()
        {
            AssertCode("@@@", 0x0000E0CA);
        }

        // Reko: a decoder for RiscV instruction 0000FC4E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FC4E()
        {
            AssertCode("@@@", 0x0000FC4E);
        }

        // Reko: a decoder for RiscV instruction 0000F852 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F852()
        {
            AssertCode("@@@", 0x0000F852);
        }

        // Reko: a decoder for RiscV instruction 0000F456 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F456()
        {
            AssertCode("@@@", 0x0000F456);
        }

        // Reko: a decoder for RiscV instruction 0000C929 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C929()
        {
            AssertCode("@@@", 0x0000C929);
        }

        // Reko: a decoder for RiscV instruction 0000C7A9 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C7A9()
        {
            AssertCode("@@@", 0x0000C7A9);
        }

        // Reko: a decoder for RiscV instruction 0000ED31 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000ED31()
        {
            AssertCode("@@@", 0x0000ED31);
        }

        // Reko: a decoder for RiscV instruction 000084AA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000084AA()
        {
            AssertCode("@@@", 0x000084AA);
        }

        // Reko: a decoder for RiscV instruction 00006762 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006762()
        {
            AssertCode("@@@", 0x00006762);
        }

        // Reko: a decoder for RiscV instruction 00006446 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006446()
        {
            AssertCode("@@@", 0x00006446);
        }

        // Reko: a decoder for RiscV instruction 000060E6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000060E6()
        {
            AssertCode("@@@", 0x000060E6);
        }

        // Reko: a decoder for RiscV instruction 000064A6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000064A6()
        {
            AssertCode("@@@", 0x000064A6);
        }

        // Reko: a decoder for RiscV instruction 00006906 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006906()
        {
            AssertCode("@@@", 0x00006906);
        }

        // Reko: a decoder for RiscV instruction 000079E2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000079E2()
        {
            AssertCode("@@@", 0x000079E2);
        }

        // Reko: a decoder for RiscV instruction 00007A42 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007A42()
        {
            AssertCode("@@@", 0x00007A42);
        }

        // Reko: a decoder for RiscV instruction 00007AA2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007AA2()
        {
            AssertCode("@@@", 0x00007AA2);
        }

        // Reko: a decoder for RiscV instruction 00007B02 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007B02()
        {
            AssertCode("@@@", 0x00007B02);
        }

        // Reko: a decoder for RiscV instruction 00006125 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006125()
        {
            AssertCode("@@@", 0x00006125);
        }

        // Reko: a decoder for RiscV instruction 0000ED3D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000ED3D()
        {
            AssertCode("@@@", 0x0000ED3D);
        }

        // Reko: a decoder for RiscV instruction 00008626 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008626()
        {
            AssertCode("@@@", 0x00008626);
        }

        // Reko: a decoder for RiscV instruction 000046C1 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000046C1()
        {
            AssertCode("@@@", 0x000046C1);
        }

        // Reko: a decoder for RiscV instruction 00008552 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008552()
        {
            AssertCode("@@@", 0x00008552);
        }

        // Reko: a decoder for RiscV instruction 000094BE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000094BE()
        {
            AssertCode("@@@", 0x000094BE);
        }

        // Reko: a decoder for RiscV instruction 000049ED at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000049ED()
        {
            AssertCode("@@@", 0x000049ED);
        }

        // Reko: a decoder for RiscV instruction 00004901 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004901()
        {
            AssertCode("@@@", 0x00004901);
        }

        // Reko: a decoder for RiscV instruction 00008005 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008005()
        {
            AssertCode("@@@", 0x00008005);
        }

        // Reko: a decoder for RiscV instruction 000094D6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000094D6()
        {
            AssertCode("@@@", 0x000094D6);
        }

        // Reko: a decoder for RiscV instruction 0000CD11 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CD11()
        {
            AssertCode("@@@", 0x0000CD11);
        }

        // Reko: a decoder for RiscV instruction 0000BF89 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF89()
        {
            AssertCode("@@@", 0x0000BF89);
        }

        // Reko: a decoder for RiscV instruction 000089A2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000089A2()
        {
            AssertCode("@@@", 0x000089A2);
        }

        // Reko: a decoder for RiscV instruction 0000BFD9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFD9()
        {
            AssertCode("@@@", 0x0000BFD9);
        }

        // Reko: a decoder for RiscV instruction 0000B71D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B71D()
        {
            AssertCode("@@@", 0x0000B71D);
        }

        // Reko: a decoder for RiscV instruction 00008782 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008782()
        {
            AssertCode("@@@", 0x00008782);
        }

        // Reko: a decoder for RiscV instruction 0000BFC1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFC1()
        {
            AssertCode("@@@", 0x0000BFC1);
        }

        // Reko: a decoder for RiscV instruction 00004705 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004705()
        {
            AssertCode("@@@", 0x00004705);
        }

        // Reko: a decoder for RiscV instruction 0000BF05 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF05()
        {
            AssertCode("@@@", 0x0000BF05);
        }

        // Reko: a decoder for RiscV instruction 0000B70D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B70D()
        {
            AssertCode("@@@", 0x0000B70D);
        }

        // Reko: a decoder for RiscV instruction 0000C911 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C911()
        {
            AssertCode("@@@", 0x0000C911);
        }

        // Reko: a decoder for RiscV instruction 00004721 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004721()
        {
            AssertCode("@@@", 0x00004721);
        }

        // Reko: a decoder for RiscV instruction 000047A1 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000047A1()
        {
            AssertCode("@@@", 0x000047A1);
        }

        // Reko: a decoder for RiscV instruction 0000B57D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B57D()
        {
            AssertCode("@@@", 0x0000B57D);
        }

        // Reko: a decoder for RiscV instruction 0000B575 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B575()
        {
            AssertCode("@@@", 0x0000B575);
        }

        // Reko: a decoder for RiscV instruction 0000B56D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B56D()
        {
            AssertCode("@@@", 0x0000B56D);
        }

        // Reko: a decoder for RiscV instruction 0000B565 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B565()
        {
            AssertCode("@@@", 0x0000B565);
        }

        // Reko: a decoder for RiscV instruction 000047D9 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000047D9()
        {
            AssertCode("@@@", 0x000047D9);
        }

        // Reko: a decoder for RiscV instruction 0000BD2D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD2D()
        {
            AssertCode("@@@", 0x0000BD2D);
        }

        // Reko: a decoder for RiscV instruction 0000B52D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B52D()
        {
            AssertCode("@@@", 0x0000B52D);
        }

        // Reko: a decoder for RiscV instruction 0000BF59 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF59()
        {
            AssertCode("@@@", 0x0000BF59);
        }

        // Reko: a decoder for RiscV instruction 0000B51D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B51D()
        {
            AssertCode("@@@", 0x0000B51D);
        }

        // Reko: a decoder for RiscV instruction 0000BD19 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD19()
        {
            AssertCode("@@@", 0x0000BD19);
        }

        // Reko: a decoder for RiscV instruction 0000BFAD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFAD()
        {
            AssertCode("@@@", 0x0000BFAD);
        }

        // Reko: a decoder for RiscV instruction 0000B519 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B519()
        {
            AssertCode("@@@", 0x0000B519);
        }

        // Reko: a decoder for RiscV instruction 0000B511 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B511()
        {
            AssertCode("@@@", 0x0000B511);
        }

        // Reko: a decoder for RiscV instruction 0000BB81 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB81()
        {
            AssertCode("@@@", 0x0000BB81);
        }

        // Reko: a decoder for RiscV instruction 0000715D at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_0000715D()
        {
            AssertCode("@@@", 0x0000715D);
        }

        // Reko: a decoder for RiscV instruction 0000F44E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F44E()
        {
            AssertCode("@@@", 0x0000F44E);
        }

        // Reko: a decoder for RiscV instruction 0000F84A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F84A()
        {
            AssertCode("@@@", 0x0000F84A);
        }

        // Reko: a decoder for RiscV instruction 0000E0A2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E0A2()
        {
            AssertCode("@@@", 0x0000E0A2);
        }

        // Reko: a decoder for RiscV instruction 0000E486 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E486()
        {
            AssertCode("@@@", 0x0000E486);
        }

        // Reko: a decoder for RiscV instruction 0000FC26 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FC26()
        {
            AssertCode("@@@", 0x0000FC26);
        }

        // Reko: a decoder for RiscV instruction 0000F052 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F052()
        {
            AssertCode("@@@", 0x0000F052);
        }

        // Reko: a decoder for RiscV instruction 0000EC3A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC3A()
        {
            AssertCode("@@@", 0x0000EC3A);
        }

        // Reko: a decoder for RiscV instruction 0000C811 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C811()
        {
            AssertCode("@@@", 0x0000C811);
        }

        // Reko: a decoder for RiscV instruction 00006408 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006408()
        {
            AssertCode("@@@", 0x00006408);
        }

        // Reko: a decoder for RiscV instruction 00006004 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006004()
        {
            AssertCode("@@@", 0x00006004);
        }

        // Reko: a decoder for RiscV instruction 00008426 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008426()
        {
            AssertCode("@@@", 0x00008426);
        }

        // Reko: a decoder for RiscV instruction 0000F865 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F865()
        {
            AssertCode("@@@", 0x0000F865);
        }

        // Reko: a decoder for RiscV instruction 00006595 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006595()
        {
            AssertCode("@@@", 0x00006595);
        }

        // Reko: a decoder for RiscV instruction 00008622 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008622()
        {
            AssertCode("@@@", 0x00008622);
        }

        // Reko: a decoder for RiscV instruction 000057FD at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000057FD()
        {
            AssertCode("@@@", 0x000057FD);
        }

        // Reko: a decoder for RiscV instruction 0000E519 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E519()
        {
            AssertCode("@@@", 0x0000E519);
        }

        // Reko: a decoder for RiscV instruction 0000C509 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C509()
        {
            AssertCode("@@@", 0x0000C509);
        }

        // Reko: a decoder for RiscV instruction 0000E288 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E288()
        {
            AssertCode("@@@", 0x0000E288);
        }

        // Reko: a decoder for RiscV instruction 000060A6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000060A6()
        {
            AssertCode("@@@", 0x000060A6);
        }

        // Reko: a decoder for RiscV instruction 00006406 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006406()
        {
            AssertCode("@@@", 0x00006406);
        }

        // Reko: a decoder for RiscV instruction 000074E2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000074E2()
        {
            AssertCode("@@@", 0x000074E2);
        }

        // Reko: a decoder for RiscV instruction 00007942 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007942()
        {
            AssertCode("@@@", 0x00007942);
        }

        // Reko: a decoder for RiscV instruction 000079A2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000079A2()
        {
            AssertCode("@@@", 0x000079A2);
        }

        // Reko: a decoder for RiscV instruction 00007A02 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007A02()
        {
            AssertCode("@@@", 0x00007A02);
        }

        // Reko: a decoder for RiscV instruction 00006161 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006161()
        {
            AssertCode("@@@", 0x00006161);
        }

        // Reko: a decoder for RiscV instruction 0000E7B9 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E7B9()
        {
            AssertCode("@@@", 0x0000E7B9);
        }

        // Reko: a decoder for RiscV instruction 00006585 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006585()
        {
            AssertCode("@@@", 0x00006585);
        }

        // Reko: a decoder for RiscV instruction 00008A2A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008A2A()
        {
            AssertCode("@@@", 0x00008A2A);
        }

        // Reko: a decoder for RiscV instruction 0000C43E at address 00100000 has not been implemented. (swsp)
        [Test]
        public void RiscV_dasm_0000C43E()
        {
            AssertCode("@@@", 0x0000C43E);
        }

        // Reko: a decoder for RiscV instruction 00004761 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004761()
        {
            AssertCode("@@@", 0x00004761);
        }

        // Reko: a decoder for RiscV instruction 0000B505 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B505()
        {
            AssertCode("@@@", 0x0000B505);
        }

        // Reko: a decoder for RiscV instruction 0000DB45 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000DB45()
        {
            AssertCode("@@@", 0x0000DB45);
        }

        // Reko: a decoder for RiscV instruction 0000BD11 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD11()
        {
            AssertCode("@@@", 0x0000BD11);
        }

        // Reko: a decoder for RiscV instruction 0000D7F1 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D7F1()
        {
            AssertCode("@@@", 0x0000D7F1);
        }

        // Reko: a decoder for RiscV instruction 0000D379 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D379()
        {
            AssertCode("@@@", 0x0000D379);
        }

        // Reko: a decoder for RiscV instruction 0000B3FD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B3FD()
        {
            AssertCode("@@@", 0x0000B3FD);
        }

        // Reko: a decoder for RiscV instruction 0000B525 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B525()
        {
            AssertCode("@@@", 0x0000B525);
        }

        // Reko: a decoder for RiscV instruction 000067C2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000067C2()
        {
            AssertCode("@@@", 0x000067C2);
        }

        // Reko: a decoder for RiscV instruction 0000BBD1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BBD1()
        {
            AssertCode("@@@", 0x0000BBD1);
        }

        // Reko: a decoder for RiscV instruction 0000B3C1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B3C1()
        {
            AssertCode("@@@", 0x0000B3C1);
        }

        // Reko: a decoder for RiscV instruction 00007139 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00007139()
        {
            AssertCode("@@@", 0x00007139);
        }

        // Reko: a decoder for RiscV instruction 0000F822 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F822()
        {
            AssertCode("@@@", 0x0000F822);
        }

        // Reko: a decoder for RiscV instruction 0000F426 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F426()
        {
            AssertCode("@@@", 0x0000F426);
        }

        // Reko: a decoder for RiscV instruction 0000F04A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F04A()
        {
            AssertCode("@@@", 0x0000F04A);
        }

        // Reko: a decoder for RiscV instruction 0000FC06 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FC06()
        {
            AssertCode("@@@", 0x0000FC06);
        }

        // Reko: a decoder for RiscV instruction 00006414 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006414()
        {
            AssertCode("@@@", 0x00006414);
        }

        // Reko: a decoder for RiscV instruction 00006818 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006818()
        {
            AssertCode("@@@", 0x00006818);
        }

        // Reko: a decoder for RiscV instruction 00006C1C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006C1C()
        {
            AssertCode("@@@", 0x00006C1C);
        }

        // Reko: a decoder for RiscV instruction 0000EC7A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC7A()
        {
            AssertCode("@@@", 0x0000EC7A);
        }

        // Reko: a decoder for RiscV instruction 0000E876 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E876()
        {
            AssertCode("@@@", 0x0000E876);
        }

        // Reko: a decoder for RiscV instruction 0000E472 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E472()
        {
            AssertCode("@@@", 0x0000E472);
        }

        // Reko: a decoder for RiscV instruction 0000E01A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E01A()
        {
            AssertCode("@@@", 0x0000E01A);
        }

        // Reko: a decoder for RiscV instruction 0000480D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000480D()
        {
            AssertCode("@@@", 0x0000480D);
        }

        // Reko: a decoder for RiscV instruction 0000479D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000479D()
        {
            AssertCode("@@@", 0x0000479D);
        }

        // Reko: a decoder for RiscV instruction 0000476D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000476D()
        {
            AssertCode("@@@", 0x0000476D);
        }

        // Reko: a decoder for RiscV instruction 00004C3C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004C3C()
        {
            AssertCode("@@@", 0x00004C3C);
        }

        // Reko: a decoder for RiscV instruction 00006108 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006108()
        {
            AssertCode("@@@", 0x00006108);
        }

        // Reko: a decoder for RiscV instruction 00007E7D at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00007E7D()
        {
            AssertCode("@@@", 0x00007E7D);
        }

        // Reko: a decoder for RiscV instruction 0000547C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000547C()
        {
            AssertCode("@@@", 0x0000547C);
        }

        // Reko: a decoder for RiscV instruction 0000E02A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E02A()
        {
            AssertCode("@@@", 0x0000E02A);
        }

        // Reko: a decoder for RiscV instruction 000091D1 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_000091D1()
        {
            AssertCode("@@@", 0x000091D1);
        }

        // Reko: a decoder for RiscV instruction 0000E41A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E41A()
        {
            AssertCode("@@@", 0x0000E41A);
        }

        // Reko: a decoder for RiscV instruction 00007442 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007442()
        {
            AssertCode("@@@", 0x00007442);
        }

        // Reko: a decoder for RiscV instruction 000070E2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000070E2()
        {
            AssertCode("@@@", 0x000070E2);
        }

        // Reko: a decoder for RiscV instruction 000074A2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000074A2()
        {
            AssertCode("@@@", 0x000074A2);
        }

        // Reko: a decoder for RiscV instruction 00007902 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007902()
        {
            AssertCode("@@@", 0x00007902);
        }

        // Reko: a decoder for RiscV instruction 00006121 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006121()
        {
            AssertCode("@@@", 0x00006121);
        }

        // Reko: a decoder for RiscV instruction 000087B2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087B2()
        {
            AssertCode("@@@", 0x000087B2);
        }

        // Reko: a decoder for RiscV instruction 000086AE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000086AE()
        {
            AssertCode("@@@", 0x000086AE);
        }

        // Reko: a decoder for RiscV instruction 00007135 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00007135()
        {
            AssertCode("@@@", 0x00007135);
        }

        // Reko: a decoder for RiscV instruction 0000F8D2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F8D2()
        {
            AssertCode("@@@", 0x0000F8D2);
        }

        // Reko: a decoder for RiscV instruction 0000F0DA at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F0DA()
        {
            AssertCode("@@@", 0x0000F0DA);
        }

        // Reko: a decoder for RiscV instruction 0000ED06 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000ED06()
        {
            AssertCode("@@@", 0x0000ED06);
        }

        // Reko: a decoder for RiscV instruction 0000E922 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E922()
        {
            AssertCode("@@@", 0x0000E922);
        }

        // Reko: a decoder for RiscV instruction 0000E526 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E526()
        {
            AssertCode("@@@", 0x0000E526);
        }

        // Reko: a decoder for RiscV instruction 0000E14A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E14A()
        {
            AssertCode("@@@", 0x0000E14A);
        }

        // Reko: a decoder for RiscV instruction 0000FCCE at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FCCE()
        {
            AssertCode("@@@", 0x0000FCCE);
        }

        // Reko: a decoder for RiscV instruction 0000F4D6 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F4D6()
        {
            AssertCode("@@@", 0x0000F4D6);
        }

        // Reko: a decoder for RiscV instruction 00008B2E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008B2E()
        {
            AssertCode("@@@", 0x00008B2E);
        }

        // Reko: a decoder for RiscV instruction 000084CA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000084CA()
        {
            AssertCode("@@@", 0x000084CA);
        }

        // Reko: a decoder for RiscV instruction 00004981 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004981()
        {
            AssertCode("@@@", 0x00004981);
        }

        // Reko: a decoder for RiscV instruction 00004A99 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004A99()
        {
            AssertCode("@@@", 0x00004A99);
        }

        // Reko: a decoder for RiscV instruction 0000C961 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C961()
        {
            AssertCode("@@@", 0x0000C961);
        }

        // Reko: a decoder for RiscV instruction 0000C179 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C179()
        {
            AssertCode("@@@", 0x0000C179);
        }

        // Reko: a decoder for RiscV instruction 00006094 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006094()
        {
            AssertCode("@@@", 0x00006094);
        }

        // Reko: a decoder for RiscV instruction 0000E02E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E02E()
        {
            AssertCode("@@@", 0x0000E02E);
        }

        // Reko: a decoder for RiscV instruction 0000F826 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F826()
        {
            AssertCode("@@@", 0x0000F826);
        }

        // Reko: a decoder for RiscV instruction 0000F41E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F41E()
        {
            AssertCode("@@@", 0x0000F41E);
        }

        // Reko: a decoder for RiscV instruction 0000F03A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F03A()
        {
            AssertCode("@@@", 0x0000F03A);
        }

        // Reko: a decoder for RiscV instruction 0000EC36 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC36()
        {
            AssertCode("@@@", 0x0000EC36);
        }

        // Reko: a decoder for RiscV instruction 0000E816 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E816()
        {
            AssertCode("@@@", 0x0000E816);
        }

        // Reko: a decoder for RiscV instruction 0000E47E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E47E()
        {
            AssertCode("@@@", 0x0000E47E);
        }

        // Reko: a decoder for RiscV instruction 0000E8FA at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E8FA()
        {
            AssertCode("@@@", 0x0000E8FA);
        }

        // Reko: a decoder for RiscV instruction 0000E4F6 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E4F6()
        {
            AssertCode("@@@", 0x0000E4F6);
        }

        // Reko: a decoder for RiscV instruction 0000E0F2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E0F2()
        {
            AssertCode("@@@", 0x0000E0F2);
        }

        // Reko: a decoder for RiscV instruction 0000FC1A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FC1A()
        {
            AssertCode("@@@", 0x0000FC1A);
        }

        // Reko: a decoder for RiscV instruction 0000BBE5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BBE5()
        {
            AssertCode("@@@", 0x0000BBE5);
        }

        // Reko: a decoder for RiscV instruction 00004789 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004789()
        {
            AssertCode("@@@", 0x00004789);
        }

        // Reko: a decoder for RiscV instruction 00004489 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004489()
        {
            AssertCode("@@@", 0x00004489);
        }

        [Test]
        public void RiscV_dasm_negative_3()
        {
            AssertCode("c.addiw\ts1,FFFFFFFFFFFFFFFD", 0x34F5);
        }

        // Reko: a decoder for RiscV instruction 000098F5 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_000098F5()
        {
            AssertCode("@@@", 0x000098F5);
        }

        // Reko: a decoder for RiscV instruction 00006294 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006294()
        {
            AssertCode("@@@", 0x00006294);
        }

        // Reko: a decoder for RiscV instruction 0000BE41 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BE41()
        {
            AssertCode("@@@", 0x0000BE41);
        }

        // Reko: a decoder for RiscV instruction 0000BE4D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BE4D()
        {
            AssertCode("@@@", 0x0000BE4D);
        }

        // Reko: a decoder for RiscV instruction 00004795 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004795()
        {
            AssertCode("@@@", 0x00004795);
        }

        // Reko: a decoder for RiscV instruction 0000478D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000478D()
        {
            AssertCode("@@@", 0x0000478D);
        }

        // Reko: a decoder for RiscV instruction 0000B389 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B389()
        {
            AssertCode("@@@", 0x0000B389);
        }

        // Reko: a decoder for RiscV instruction 0000BC99 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BC99()
        {
            AssertCode("@@@", 0x0000BC99);
        }

        // Reko: a decoder for RiscV instruction 00009D1D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009D1D()
        {
            AssertCode("@@@", 0x00009D1D);
        }

        // Reko: a decoder for RiscV instruction 00007908 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007908()
        {
            AssertCode("@@@", 0x00007908);
        }

        // Reko: a decoder for RiscV instruction 0000799C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000799C()
        {
            AssertCode("@@@", 0x0000799C);
        }

        // Reko: a decoder for RiscV instruction 00006D68 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006D68()
        {
            AssertCode("@@@", 0x00006D68);
        }

        // Reko: a decoder for RiscV instruction 00006DFC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006DFC()
        {
            AssertCode("@@@", 0x00006DFC);
        }

        // Reko: a decoder for RiscV instruction 00007168 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007168()
        {
            AssertCode("@@@", 0x00007168);
        }

        // Reko: a decoder for RiscV instruction 000071FC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000071FC()
        {
            AssertCode("@@@", 0x000071FC);
        }

        // Reko: a decoder for RiscV instruction 00007568 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007568()
        {
            AssertCode("@@@", 0x00007568);
        }

        // Reko: a decoder for RiscV instruction 000075FC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000075FC()
        {
            AssertCode("@@@", 0x000075FC);
        }

        // Reko: a decoder for RiscV instruction 00007968 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007968()
        {
            AssertCode("@@@", 0x00007968);
        }

        // Reko: a decoder for RiscV instruction 000079FC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000079FC()
        {
            AssertCode("@@@", 0x000079FC);
        }

        // Reko: a decoder for RiscV instruction 00007D68 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007D68()
        {
            AssertCode("@@@", 0x00007D68);
        }

        // Reko: a decoder for RiscV instruction 00007DFC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007DFC()
        {
            AssertCode("@@@", 0x00007DFC);
        }

        // Reko: a decoder for RiscV instruction 00007108 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007108()
        {
            AssertCode("@@@", 0x00007108);
        }

        // Reko: a decoder for RiscV instruction 0000719C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000719C()
        {
            AssertCode("@@@", 0x0000719C);
        }

        // Reko: a decoder for RiscV instruction 00007508 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007508()
        {
            AssertCode("@@@", 0x00007508);
        }

        // Reko: a decoder for RiscV instruction 0000759C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000759C()
        {
            AssertCode("@@@", 0x0000759C);
        }

        // Reko: a decoder for RiscV instruction 00007548 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007548()
        {
            AssertCode("@@@", 0x00007548);
        }

        // Reko: a decoder for RiscV instruction 000075DC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000075DC()
        {
            AssertCode("@@@", 0x000075DC);
        }

        // Reko: a decoder for RiscV instruction 00007948 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007948()
        {
            AssertCode("@@@", 0x00007948);
        }

        // Reko: a decoder for RiscV instruction 000079DC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000079DC()
        {
            AssertCode("@@@", 0x000079DC);
        }

        // Reko: a decoder for RiscV instruction 00007D48 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007D48()
        {
            AssertCode("@@@", 0x00007D48);
        }

        // Reko: a decoder for RiscV instruction 00007DDC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007DDC()
        {
            AssertCode("@@@", 0x00007DDC);
        }

        // Reko: a decoder for RiscV instruction 00006168 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006168()
        {
            AssertCode("@@@", 0x00006168);
        }

        // Reko: a decoder for RiscV instruction 000061FC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000061FC()
        {
            AssertCode("@@@", 0x000061FC);
        }

        // Reko: a decoder for RiscV instruction 00006568 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006568()
        {
            AssertCode("@@@", 0x00006568);
        }

        // Reko: a decoder for RiscV instruction 000065FC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000065FC()
        {
            AssertCode("@@@", 0x000065FC);
        }

        // Reko: a decoder for RiscV instruction 00006128 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006128()
        {
            AssertCode("@@@", 0x00006128);
        }

        // Reko: a decoder for RiscV instruction 000061BC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000061BC()
        {
            AssertCode("@@@", 0x000061BC);
        }

        // Reko: a decoder for RiscV instruction 00006968 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006968()
        {
            AssertCode("@@@", 0x00006968);
        }

        // Reko: a decoder for RiscV instruction 000069FC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000069FC()
        {
            AssertCode("@@@", 0x000069FC);
        }

        // Reko: a decoder for RiscV instruction 00004108 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004108()
        {
            AssertCode("@@@", 0x00004108);
        }

        // Reko: a decoder for RiscV instruction 0000419C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000419C()
        {
            AssertCode("@@@", 0x0000419C);
        }

        // Reko: a decoder for RiscV instruction 00004148 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004148()
        {
            AssertCode("@@@", 0x00004148);
        }

        // Reko: a decoder for RiscV instruction 000041DC at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_000041DC()
        {
            AssertCode("@@@", 0x000041DC);
        }

        // Reko: a decoder for RiscV instruction 00004D08 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004D08()
        {
            AssertCode("@@@", 0x00004D08);
        }

        // Reko: a decoder for RiscV instruction 00004D9C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004D9C()
        {
            AssertCode("@@@", 0x00004D9C);
        }

        // Reko: a decoder for RiscV instruction 0000711C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000711C()
        {
            AssertCode("@@@", 0x0000711C);
        }

        // Reko: a decoder for RiscV instruction 00007198 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007198()
        {
            AssertCode("@@@", 0x00007198);
        }

        // Reko: a decoder for RiscV instruction 00007590 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007590()
        {
            AssertCode("@@@", 0x00007590);
        }

        // Reko: a decoder for RiscV instruction 0000953E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000953E()
        {
            AssertCode("@@@", 0x0000953E);
        }

        // Reko: a decoder for RiscV instruction 00006390 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006390()
        {
            AssertCode("@@@", 0x00006390);
        }

        // Reko: a decoder for RiscV instruction 00006134 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006134()
        {
            AssertCode("@@@", 0x00006134);
        }

        // Reko: a decoder for RiscV instruction 000061B8 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000061B8()
        {
            AssertCode("@@@", 0x000061B8);
        }

        // Reko: a decoder for RiscV instruction 00008F99 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008F99()
        {
            AssertCode("@@@", 0x00008F99);
        }

        // Reko: a decoder for RiscV instruction 0000C3D9 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C3D9()
        {
            AssertCode("@@@", 0x0000C3D9);
        }

        // Reko: a decoder for RiscV instruction 00004709 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004709()
        {
            AssertCode("@@@", 0x00004709);
        }

        // Reko: a decoder for RiscV instruction 000097AA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000097AA()
        {
            AssertCode("@@@", 0x000097AA);
        }

        // Reko: a decoder for RiscV instruction 0000CA81 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CA81()
        {
            AssertCode("@@@", 0x0000CA81);
        }

        // Reko: a decoder for RiscV instruction 0000853A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000853A()
        {
            AssertCode("@@@", 0x0000853A);
        }

        // Reko: a decoder for RiscV instruction 0000B769 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B769()
        {
            AssertCode("@@@", 0x0000B769);
        }

        // Reko: a decoder for RiscV instruction 0000B751 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B751()
        {
            AssertCode("@@@", 0x0000B751);
        }

        // Reko: a decoder for RiscV instruction 00004605 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004605()
        {
            AssertCode("@@@", 0x00004605);
        }

        // Reko: a decoder for RiscV instruction 00007179 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00007179()
        {
            AssertCode("@@@", 0x00007179);
        }

        // Reko: a decoder for RiscV instruction 0000E84A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E84A()
        {
            AssertCode("@@@", 0x0000E84A);
        }

        // Reko: a decoder for RiscV instruction 0000F406 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F406()
        {
            AssertCode("@@@", 0x0000F406);
        }

        // Reko: a decoder for RiscV instruction 0000F022 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F022()
        {
            AssertCode("@@@", 0x0000F022);
        }

        // Reko: a decoder for RiscV instruction 0000EC26 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC26()
        {
            AssertCode("@@@", 0x0000EC26);
        }

        // Reko: a decoder for RiscV instruction 0000E44E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E44E()
        {
            AssertCode("@@@", 0x0000E44E);
        }

        // Reko: a decoder for RiscV instruction 0000E052 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E052()
        {
            AssertCode("@@@", 0x0000E052);
        }

        // Reko: a decoder for RiscV instruction 000067D5 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_000067D5()
        {
            AssertCode("@@@", 0x000067D5);
        }

        // Reko: a decoder for RiscV instruction 00004661 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004661()
        {
            AssertCode("@@@", 0x00004661);
        }

        // Reko: a decoder for RiscV instruction 000087D2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087D2()
        {
            AssertCode("@@@", 0x000087D2);
        }

        // Reko: a decoder for RiscV instruction 00004401 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004401()
        {
            AssertCode("@@@", 0x00004401);
        }

        // Reko: a decoder for RiscV instruction 00008822 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008822()
        {
            AssertCode("@@@", 0x00008822);
        }

        // Reko: a decoder for RiscV instruction 000087CE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087CE()
        {
            AssertCode("@@@", 0x000087CE);
        }

        // Reko: a decoder for RiscV instruction 000070A2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000070A2()
        {
            AssertCode("@@@", 0x000070A2);
        }

        // Reko: a decoder for RiscV instruction 00007402 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007402()
        {
            AssertCode("@@@", 0x00007402);
        }

        // Reko: a decoder for RiscV instruction 000064E2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000064E2()
        {
            AssertCode("@@@", 0x000064E2);
        }

        // Reko: a decoder for RiscV instruction 00006942 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006942()
        {
            AssertCode("@@@", 0x00006942);
        }

        // Reko: a decoder for RiscV instruction 000069A2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000069A2()
        {
            AssertCode("@@@", 0x000069A2);
        }

        // Reko: a decoder for RiscV instruction 00006A02 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006A02()
        {
            AssertCode("@@@", 0x00006A02);
        }

        // Reko: a decoder for RiscV instruction 00006145 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006145()
        {
            AssertCode("@@@", 0x00006145);
        }

        // Reko: a decoder for RiscV instruction 0000D771 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D771()
        {
            AssertCode("@@@", 0x0000D771);
        }

        // Reko: a decoder for RiscV instruction 0000B765 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B765()
        {
            AssertCode("@@@", 0x0000B765);
        }

        // Reko: a decoder for RiscV instruction 00009F99 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009F99()
        {
            AssertCode("@@@", 0x00009F99);
        }

        // Reko: a decoder for RiscV instruction 00007594 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007594()
        {
            AssertCode("@@@", 0x00007594);
        }

        // Reko: a decoder for RiscV instruction 00007D94 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007D94()
        {
            AssertCode("@@@", 0x00007D94);
        }

        // Reko: a decoder for RiscV instruction 0000973E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000973E()
        {
            AssertCode("@@@", 0x0000973E);
        }

        // Reko: a decoder for RiscV instruction 000061B4 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000061B4()
        {
            AssertCode("@@@", 0x000061B4);
        }

        // Reko: a decoder for RiscV instruction 0000C28D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C28D()
        {
            AssertCode("@@@", 0x0000C28D);
        }

        // Reko: a decoder for RiscV instruction 0000BFF1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFF1()
        {
            AssertCode("@@@", 0x0000BFF1);
        }

        // Reko: a decoder for RiscV instruction 00007998 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007998()
        {
            AssertCode("@@@", 0x00007998);
        }

        // Reko: a decoder for RiscV instruction 00006314 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006314()
        {
            AssertCode("@@@", 0x00006314);
        }

        // Reko: a decoder for RiscV instruction 0000EF09 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EF09()
        {
            AssertCode("@@@", 0x0000EF09);
        }

        // Reko: a decoder for RiscV instruction 00004801 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004801()
        {
            AssertCode("@@@", 0x00004801);
        }

        // Reko: a decoder for RiscV instruction 00004629 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004629()
        {
            AssertCode("@@@", 0x00004629);
        }

        // Reko: a decoder for RiscV instruction 02C8783B at address 00100000 has not been implemented.
        [Test]
        public void RiscV_dasm_02C8783B()
        {
            AssertCode("@@@", 0x02C8783B);
        }

        // Reko: a decoder for RiscV instruction 0000BF55 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF55()
        {
            AssertCode("@@@", 0x0000BF55);
        }

        // Reko: a decoder for RiscV instruction 00007598 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007598()
        {
            AssertCode("@@@", 0x00007598);
        }

        // Reko: a decoder for RiscV instruction 000098BA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000098BA()
        {
            AssertCode("@@@", 0x000098BA);
        }

        // Reko: a decoder for RiscV instruction 00006405 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006405()
        {
            AssertCode("@@@", 0x00006405);
        }

        // Reko: a decoder for RiscV instruction 0000C38D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C38D()
        {
            AssertCode("@@@", 0x0000C38D);
        }

        // Reko: a decoder for RiscV instruction 0000E442 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E442()
        {
            AssertCode("@@@", 0x0000E442);
        }

        // Reko: a decoder for RiscV instruction 0000E046 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E046()
        {
            AssertCode("@@@", 0x0000E046);
        }

        // Reko: a decoder for RiscV instruction 00006882 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006882()
        {
            AssertCode("@@@", 0x00006882);
        }

        // Reko: a decoder for RiscV instruction 00006822 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006822()
        {
            AssertCode("@@@", 0x00006822);
        }

        // Reko: a decoder for RiscV instruction 000094AA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000094AA()
        {
            AssertCode("@@@", 0x000094AA);
        }

        // Reko: a decoder for RiscV instruction 000087A2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087A2()
        {
            AssertCode("@@@", 0x000087A2);
        }

        // Reko: a decoder for RiscV instruction 00004715 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004715()
        {
            AssertCode("@@@", 0x00004715);
        }

        // Reko: a decoder for RiscV instruction 0000470D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000470D()
        {
            AssertCode("@@@", 0x0000470D);
        }

        // Reko: a decoder for RiscV instruction 00004711 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004711()
        {
            AssertCode("@@@", 0x00004711);
        }

        // Reko: a decoder for RiscV instruction 00004725 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004725()
        {
            AssertCode("@@@", 0x00004725);
        }

        // Reko: a decoder for RiscV instruction 0000CFB5 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CFB5()
        {
            AssertCode("@@@", 0x0000CFB5);
        }

        // Reko: a decoder for RiscV instruction 00004719 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004719()
        {
            AssertCode("@@@", 0x00004719);
        }

        // Reko: a decoder for RiscV instruction 0000471D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000471D()
        {
            AssertCode("@@@", 0x0000471D);
        }

        // Reko: a decoder for RiscV instruction 00008399 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008399()
        {
            AssertCode("@@@", 0x00008399);
        }

        // Reko: a decoder for RiscV instruction 00008B9D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008B9D()
        {
            AssertCode("@@@", 0x00008B9D);
        }

        // Reko: a decoder for RiscV instruction 000047C1 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000047C1()
        {
            AssertCode("@@@", 0x000047C1);
        }

        // Reko: a decoder for RiscV instruction 00004541 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004541()
        {
            AssertCode("@@@", 0x00004541);
        }

        // Reko: a decoder for RiscV instruction 00007194 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007194()
        {
            AssertCode("@@@", 0x00007194);
        }

        // Reko: a decoder for RiscV instruction 000096BE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000096BE()
        {
            AssertCode("@@@", 0x000096BE);
        }

        // Reko: a decoder for RiscV instruction 00007D98 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007D98()
        {
            AssertCode("@@@", 0x00007D98);
        }

        // Reko: a decoder for RiscV instruction 0317F83B at address 00100000 has not been implemented.
        [Test]
        public void RiscV_dasm_0317F83B()
        {
            AssertCode("@@@", 0x0317F83B);
        }

        // Reko: a decoder for RiscV instruction 00004394 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004394()
        {
            AssertCode("@@@", 0x00004394);
        }

        // Reko: a decoder for RiscV instruction 00009FB5 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009FB5()
        {
            AssertCode("@@@", 0x00009FB5);
        }

        // Reko: a decoder for RiscV instruction 0000C385 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C385()
        {
            AssertCode("@@@", 0x0000C385);
        }

        // Reko: a decoder for RiscV instruction 000075D4 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000075D4()
        {
            AssertCode("@@@", 0x000075D4);
        }

        // Reko: a decoder for RiscV instruction 000079D8 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000079D8()
        {
            AssertCode("@@@", 0x000079D8);
        }

        // Reko: a decoder for RiscV instruction 000083A9 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_000083A9()
        {
            AssertCode("@@@", 0x000083A9);
        }

        // Reko: a decoder for RiscV instruction 0000CF99 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CF99()
        {
            AssertCode("@@@", 0x0000CF99);
        }

        // Reko: a decoder for RiscV instruction 000075D8 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000075D8()
        {
            AssertCode("@@@", 0x000075D8);
        }

        // Reko: a decoder for RiscV instruction 000097B2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000097B2()
        {
            AssertCode("@@@", 0x000097B2);
        }

        // Reko: a decoder for RiscV instruction 00004398 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004398()
        {
            AssertCode("@@@", 0x00004398);
        }

        // Reko: a decoder for RiscV instruction 0000C701 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C701()
        {
            AssertCode("@@@", 0x0000C701);
        }

        // Reko: a decoder for RiscV instruction 000048A9 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000048A9()
        {
            AssertCode("@@@", 0x000048A9);
        }

        // Reko: a decoder for RiscV instruction 00008B8D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008B8D()
        {
            AssertCode("@@@", 0x00008B8D);
        }

        // Reko: a decoder for RiscV instruction 02E7F7BB at address 00100000 has not been implemented.
        [Test]
        public void RiscV_dasm_02E7F7BB()
        {
            AssertCode("@@@", 0x02E7F7BB);
        }

        // Reko: a decoder for RiscV instruction 00007394 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007394()
        {
            AssertCode("@@@", 0x00007394);
        }

        // Reko: a decoder for RiscV instruction 00007F94 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00007F94()
        {
            AssertCode("@@@", 0x00007F94);
        }

        // Reko: a decoder for RiscV instruction 000073B4 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000073B4()
        {
            AssertCode("@@@", 0x000073B4);
        }

        // Reko: a decoder for RiscV instruction 0000430C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000430C()
        {
            AssertCode("@@@", 0x0000430C);
        }

        // Reko: a decoder for RiscV instruction 000063D4 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000063D4()
        {
            AssertCode("@@@", 0x000063D4);
        }

        // Reko: a decoder for RiscV instruction 00009181 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009181()
        {
            AssertCode("@@@", 0x00009181);
        }

        // Reko: a decoder for RiscV instruction 000073D4 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000073D4()
        {
            AssertCode("@@@", 0x000073D4);
        }

        // Reko: a decoder for RiscV instruction 00004194 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004194()
        {
            AssertCode("@@@", 0x00004194);
        }

        // Reko: a decoder for RiscV instruction 0000852E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000852E()
        {
            AssertCode("@@@", 0x0000852E);
        }

        // Reko: a decoder for RiscV instruction 0000C795 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C795()
        {
            AssertCode("@@@", 0x0000C795);
        }

        // Reko: a decoder for RiscV instruction 000096A2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000096A2()
        {
            AssertCode("@@@", 0x000096A2);
        }

        // Reko: a decoder for RiscV instruction 0000C519 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C519()
        {
            AssertCode("@@@", 0x0000C519);
        }

        // Reko: a decoder for RiscV instruction 0000638C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000638C()
        {
            AssertCode("@@@", 0x0000638C);
        }

        // Reko: a decoder for RiscV instruction 0000C199 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C199()
        {
            AssertCode("@@@", 0x0000C199);
        }

        // Reko: a decoder for RiscV instruction 00006018 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006018()
        {
            AssertCode("@@@", 0x00006018);
        }

        // Reko: a decoder for RiscV instruction 0000409C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000409C()
        {
            AssertCode("@@@", 0x0000409C);
        }

        // Reko: a decoder for RiscV instruction 0000E43A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E43A()
        {
            AssertCode("@@@", 0x0000E43A);
        }

        // Reko: a decoder for RiscV instruction 0000C23E at address 00100000 has not been implemented. (swsp)
        [Test]
        public void RiscV_dasm_0000C23E()
        {
            AssertCode("@@@", 0x0000C23E);
        }

        // Reko: a decoder for RiscV instruction 00004088 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004088()
        {
            AssertCode("@@@", 0x00004088);
        }

        // Reko: a decoder for RiscV instruction 00006722 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006722()
        {
            AssertCode("@@@", 0x00006722);
        }

        // Reko: a decoder for RiscV instruction 00004692 at address 00100000 has not been implemented. (lwsp)
        [Test]
        public void RiscV_dasm_00004692()
        {
            AssertCode("@@@", 0x00004692);
        }

        // Reko: a decoder for RiscV instruction 0000601C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000601C()
        {
            AssertCode("@@@", 0x0000601C);
        }

        // Reko: a decoder for RiscV instruction 00009D15 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009D15()
        {
            AssertCode("@@@", 0x00009D15);
        }

        // Reko: a decoder for RiscV instruction 0000628C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000628C()
        {
            AssertCode("@@@", 0x0000628C);
        }

        // Reko: a decoder for RiscV instruction 0000E43E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E43E()
        {
            AssertCode("@@@", 0x0000E43E);
        }

        // Reko: a decoder for RiscV instruction 0000EC4E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC4E()
        {
            AssertCode("@@@", 0x0000EC4E);
        }

        // Reko: a decoder for RiscV instruction 0000E852 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E852()
        {
            AssertCode("@@@", 0x0000E852);
        }

        // Reko: a decoder for RiscV instruction 00008A2E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008A2E()
        {
            AssertCode("@@@", 0x00008A2E);
        }

        // Reko: a decoder for RiscV instruction 0000850A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000850A()
        {
            AssertCode("@@@", 0x0000850A);
        }

        // Reko: a decoder for RiscV instruction 0000E03E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E03E()
        {
            AssertCode("@@@", 0x0000E03E);
        }

        // Reko: a decoder for RiscV instruction 00004D5C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004D5C()
        {
            AssertCode("@@@", 0x00004D5C);
        }

        // Reko: a decoder for RiscV instruction 00004ADC at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004ADC()
        {
            AssertCode("@@@", 0x00004ADC);
        }

        // Reko: a decoder for RiscV instruction 000069E2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000069E2()
        {
            AssertCode("@@@", 0x000069E2);
        }

        // Reko: a decoder for RiscV instruction 00006A42 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006A42()
        {
            AssertCode("@@@", 0x00006A42);
        }

        // Reko: a decoder for RiscV instruction 0000892E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000892E()
        {
            AssertCode("@@@", 0x0000892E);
        }

        // Reko: a decoder for RiscV instruction 00006794 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006794()
        {
            AssertCode("@@@", 0x00006794);
        }

        // Reko: a decoder for RiscV instruction 00006755 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006755()
        {
            AssertCode("@@@", 0x00006755);
        }

        // Reko: a decoder for RiscV instruction 00004521 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004521()
        {
            AssertCode("@@@", 0x00004521);
        }

        // Reko: a decoder for RiscV instruction 00004561 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004561()
        {
            AssertCode("@@@", 0x00004561);
        }

        // Reko: a decoder for RiscV instruction 0000CB89 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CB89()
        {
            AssertCode("@@@", 0x0000CB89);
        }

        // Reko: a decoder for RiscV instruction 0000E42E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E42E()
        {
            AssertCode("@@@", 0x0000E42E);
        }

        // Reko: a decoder for RiscV instruction 000065A2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000065A2()
        {
            AssertCode("@@@", 0x000065A2);
        }

        // Reko: a decoder for RiscV instruction 0000FC42 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FC42()
        {
            AssertCode("@@@", 0x0000FC42);
        }

        // Reko: a decoder for RiscV instruction 00005A7D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00005A7D()
        {
            AssertCode("@@@", 0x00005A7D);
        }

        // Reko: a decoder for RiscV instruction 000085A6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085A6()
        {
            AssertCode("@@@", 0x000085A6);
        }

        // Reko: a decoder for RiscV instruction 00007762 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007762()
        {
            AssertCode("@@@", 0x00007762);
        }

        // Reko: a decoder for RiscV instruction 0000B7E9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7E9()
        {
            AssertCode("@@@", 0x0000B7E9);
        }

        // Reko: a decoder for RiscV instruction 0000832A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000832A()
        {
            AssertCode("@@@", 0x0000832A);
        }

        // Reko: a decoder for RiscV instruction 0000CFAD at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CFAD()
        {
            AssertCode("@@@", 0x0000CFAD);
        }

        // Reko: a decoder for RiscV instruction 00004290 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004290()
        {
            AssertCode("@@@", 0x00004290);
        }

        // Reko: a decoder for RiscV instruction 00004188 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004188()
        {
            AssertCode("@@@", 0x00004188);
        }

        // Reko: a decoder for RiscV instruction 0000869A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000869A()
        {
            AssertCode("@@@", 0x0000869A);
        }

        // Reko: a decoder for RiscV instruction 0000F7F9 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F7F9()
        {
            AssertCode("@@@", 0x0000F7F9);
        }

        // Reko: a decoder for RiscV instruction 0000BFC9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFC9()
        {
            AssertCode("@@@", 0x0000BFC9);
        }

        // Reko: a decoder for RiscV instruction 0000BF65 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF65()
        {
            AssertCode("@@@", 0x0000BF65);
        }

        // Reko: a decoder for RiscV instruction 00004885 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004885()
        {
            AssertCode("@@@", 0x00004885);
        }

        // Reko: a decoder for RiscV instruction 0000FBF9 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FBF9()
        {
            AssertCode("@@@", 0x0000FBF9);
        }

        // Reko: a decoder for RiscV instruction 0000B7ED at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7ED()
        {
            AssertCode("@@@", 0x0000B7ED);
        }

        // Reko: a decoder for RiscV instruction 000096B2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000096B2()
        {
            AssertCode("@@@", 0x000096B2);
        }

        // Reko: a decoder for RiscV instruction 00006098 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006098()
        {
            AssertCode("@@@", 0x00006098);
        }

        // Reko: a decoder for RiscV instruction 000089AE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000089AE()
        {
            AssertCode("@@@", 0x000089AE);
        }

        // Reko: a decoder for RiscV instruction 00004792 at address 00100000 has not been implemented. (lwsp)
        [Test]
        public void RiscV_dasm_00004792()
        {
            AssertCode("@@@", 0x00004792);
        }

        // Reko: a decoder for RiscV instruction 000046A1 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000046A1()
        {
            AssertCode("@@@", 0x000046A1);
        }

        // Reko: a decoder for RiscV instruction 00009522 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009522()
        {
            AssertCode("@@@", 0x00009522);
        }

        // Reko: a decoder for RiscV instruction 0000C236 at address 00100000 has not been implemented. (swsp)
        [Test]
        public void RiscV_dasm_0000C236()
        {
            AssertCode("@@@", 0x0000C236);
        }

        // Reko: a decoder for RiscV instruction 0000C23A at address 00100000 has not been implemented. (swsp)
        [Test]
        public void RiscV_dasm_0000C23A()
        {
            AssertCode("@@@", 0x0000C23A);
        }

        // Reko: a decoder for RiscV instruction 00006694 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006694()
        {
            AssertCode("@@@", 0x00006694);
        }

        // Reko: a decoder for RiscV instruction 00007119 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00007119()
        {
            AssertCode("@@@", 0x00007119);
        }

        // Reko: a decoder for RiscV instruction 0000F0CA at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F0CA()
        {
            AssertCode("@@@", 0x0000F0CA);
        }

        // Reko: a decoder for RiscV instruction 0000ECCE at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000ECCE()
        {
            AssertCode("@@@", 0x0000ECCE);
        }

        // Reko: a decoder for RiscV instruction 0000F8A2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F8A2()
        {
            AssertCode("@@@", 0x0000F8A2);
        }

        // Reko: a decoder for RiscV instruction 0000F4A6 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F4A6()
        {
            AssertCode("@@@", 0x0000F4A6);
        }

        // Reko: a decoder for RiscV instruction 0000FC86 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FC86()
        {
            AssertCode("@@@", 0x0000FC86);
        }

        // Reko: a decoder for RiscV instruction 0000E8D2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E8D2()
        {
            AssertCode("@@@", 0x0000E8D2);
        }

        // Reko: a decoder for RiscV instruction 0000E4D6 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E4D6()
        {
            AssertCode("@@@", 0x0000E4D6);
        }

        // Reko: a decoder for RiscV instruction 0000FC3E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FC3E()
        {
            AssertCode("@@@", 0x0000FC3E);
        }

        // Reko: a decoder for RiscV instruction 0000C791 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C791()
        {
            AssertCode("@@@", 0x0000C791);
        }

        // Reko: a decoder for RiscV instruction 0000858A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000858A()
        {
            AssertCode("@@@", 0x0000858A);
        }

        // Reko: a decoder for RiscV instruction 00009702 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009702()
        {
            AssertCode("@@@", 0x00009702);
        }

        // Reko: a decoder for RiscV instruction 0000C545 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C545()
        {
            AssertCode("@@@", 0x0000C545);
        }

        // Reko: a decoder for RiscV instruction 000099A6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000099A6()
        {
            AssertCode("@@@", 0x000099A6);
        }

        // Reko: a decoder for RiscV instruction 0000610C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000610C()
        {
            AssertCode("@@@", 0x0000610C);
        }

        // Reko: a decoder for RiscV instruction 00008726 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008726()
        {
            AssertCode("@@@", 0x00008726);
        }

        // Reko: a decoder for RiscV instruction 00006611 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006611()
        {
            AssertCode("@@@", 0x00006611);
        }

        // Reko: a decoder for RiscV instruction 0000A011 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A011()
        {
            AssertCode("@@@", 0x0000A011);
        }

        // Reko: a decoder for RiscV instruction 000086BA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000086BA()
        {
            AssertCode("@@@", 0x000086BA);
        }

        // Reko: a decoder for RiscV instruction 000097AE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000097AE()
        {
            AssertCode("@@@", 0x000097AE);
        }

        // Reko: a decoder for RiscV instruction 00008FF1 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008FF1()
        {
            AssertCode("@@@", 0x00008FF1);
        }

        // Reko: a decoder for RiscV instruction 0000F7F5 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F7F5()
        {
            AssertCode("@@@", 0x0000F7F5);
        }

        // Reko: a decoder for RiscV instruction 000070E6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000070E6()
        {
            AssertCode("@@@", 0x000070E6);
        }

        // Reko: a decoder for RiscV instruction 00007446 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007446()
        {
            AssertCode("@@@", 0x00007446);
        }

        // Reko: a decoder for RiscV instruction 000074A6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000074A6()
        {
            AssertCode("@@@", 0x000074A6);
        }

        // Reko: a decoder for RiscV instruction 00007906 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007906()
        {
            AssertCode("@@@", 0x00007906);
        }

        // Reko: a decoder for RiscV instruction 000069E6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000069E6()
        {
            AssertCode("@@@", 0x000069E6);
        }

        // Reko: a decoder for RiscV instruction 00006A46 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006A46()
        {
            AssertCode("@@@", 0x00006A46);
        }

        // Reko: a decoder for RiscV instruction 00006AA6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006AA6()
        {
            AssertCode("@@@", 0x00006AA6);
        }

        // Reko: a decoder for RiscV instruction 00006109 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006109()
        {
            AssertCode("@@@", 0x00006109);
        }

        // Reko: a decoder for RiscV instruction 00006A82 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006A82()
        {
            AssertCode("@@@", 0x00006A82);
        }

        // Reko: a decoder for RiscV instruction 00008556 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008556()
        {
            AssertCode("@@@", 0x00008556);
        }

        // Reko: a decoder for RiscV instruction 00008652 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008652()
        {
            AssertCode("@@@", 0x00008652);
        }

        // Reko: a decoder for RiscV instruction 000085D6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085D6()
        {
            AssertCode("@@@", 0x000085D6);
        }

        // Reko: a decoder for RiscV instruction 00004729 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004729()
        {
            AssertCode("@@@", 0x00004729);
        }

        // Reko: a decoder for RiscV instruction 00008452 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008452()
        {
            AssertCode("@@@", 0x00008452);
        }

        // Reko: a decoder for RiscV instruction 0000B745 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B745()
        {
            AssertCode("@@@", 0x0000B745);
        }

        // Reko: a decoder for RiscV instruction 0000E90D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E90D()
        {
            AssertCode("@@@", 0x0000E90D);
        }

        // Reko: a decoder for RiscV instruction 00009A02 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009A02()
        {
            AssertCode("@@@", 0x00009A02);
        }

        // Reko: a decoder for RiscV instruction 0000BD8D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD8D()
        {
            AssertCode("@@@", 0x0000BD8D);
        }

        // Reko: a decoder for RiscV instruction 0000B5FD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B5FD()
        {
            AssertCode("@@@", 0x0000B5FD);
        }

        // Reko: a decoder for RiscV instruction 0000942A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000942A()
        {
            AssertCode("@@@", 0x0000942A);
        }

        // Reko: a decoder for RiscV instruction 0000C22A at address 00100000 has not been implemented. (swsp)
        [Test]
        public void RiscV_dasm_0000C22A()
        {
            AssertCode("@@@", 0x0000C22A);
        }

        // Reko: a decoder for RiscV instruction 0000EBA5 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EBA5()
        {
            AssertCode("@@@", 0x0000EBA5);
        }

        // Reko: a decoder for RiscV instruction 000086D2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000086D2()
        {
            AssertCode("@@@", 0x000086D2);
        }

        // Reko: a decoder for RiscV instruction 0000C715 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C715()
        {
            AssertCode("@@@", 0x0000C715);
        }

        // Reko: a decoder for RiscV instruction 0000C185 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C185()
        {
            AssertCode("@@@", 0x0000C185);
        }

        // Reko: a decoder for RiscV instruction 00006198 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006198()
        {
            AssertCode("@@@", 0x00006198);
        }

        // Reko: a decoder for RiscV instruction 0000CF11 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CF11()
        {
            AssertCode("@@@", 0x0000CF11);
        }

        // Reko: a decoder for RiscV instruction 000066A2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000066A2()
        {
            AssertCode("@@@", 0x000066A2);
        }

        // Reko: a decoder for RiscV instruction 0000BF51 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF51()
        {
            AssertCode("@@@", 0x0000BF51);
        }

        // Reko: a decoder for RiscV instruction 0000CBA5 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CBA5()
        {
            AssertCode("@@@", 0x0000CBA5);
        }

        // Reko: a decoder for RiscV instruction 0000882A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000882A()
        {
            AssertCode("@@@", 0x0000882A);
        }

        // Reko: a decoder for RiscV instruction 00008542 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008542()
        {
            AssertCode("@@@", 0x00008542);
        }

        [Test]
        public void RiscV_dasm_c_li_minus3()
        {
            AssertCode("c.li\ta4,FFFFFFFFFFFFFFFD", 0x00005775);
        }

        // Reko: a decoder for RiscV instruction 0000C7A5 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C7A5()
        {
            AssertCode("@@@", 0x0000C7A5);
        }

        // Reko: a decoder for RiscV instruction 00004294 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004294()
        {
            AssertCode("@@@", 0x00004294);
        }

        // Reko: a decoder for RiscV instruction 0000EE95 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EE95()
        {
            AssertCode("@@@", 0x0000EE95);
        }

        // Reko: a decoder for RiscV instruction 000085CA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085CA()
        {
            AssertCode("@@@", 0x000085CA);
        }

        // Reko: a decoder for RiscV instruction 0000B74D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B74D()
        {
            AssertCode("@@@", 0x0000B74D);
        }

        // Reko: a decoder for RiscV instruction 0000E399 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E399()
        {
            AssertCode("@@@", 0x0000E399);
        }

        // Reko: a decoder for RiscV instruction 000089B2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000089B2()
        {
            AssertCode("@@@", 0x000089B2);
        }

        // Reko: a decoder for RiscV instruction 0000E33D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E33D()
        {
            AssertCode("@@@", 0x0000E33D);
        }

        // Reko: a decoder for RiscV instruction 0000C222 at address 00100000 has not been implemented. (swsp)
        [Test]
        public void RiscV_dasm_0000C222()
        {
            AssertCode("@@@", 0x0000C222);
        }

        // Reko: a decoder for RiscV instruction 00004512 at address 00100000 has not been implemented. (lwsp)
        [Test]
        public void RiscV_dasm_00004512()
        {
            AssertCode("@@@", 0x00004512);
        }

        // Reko: a decoder for RiscV instruction 00009281 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009281()
        {
            AssertCode("@@@", 0x00009281);
        }

        // Reko: a decoder for RiscV instruction 000096CA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000096CA()
        {
            AssertCode("@@@", 0x000096CA);
        }

        // Reko: a decoder for RiscV instruction 0000974A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000974A()
        {
            AssertCode("@@@", 0x0000974A);
        }

        // Reko: a decoder for RiscV instruction 0000992A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000992A()
        {
            AssertCode("@@@", 0x0000992A);
        }

        // Reko: a decoder for RiscV instruction 00006789 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006789()
        {
            AssertCode("@@@", 0x00006789);
        }

        // Reko: a decoder for RiscV instruction 02D8783B at address 00100000 has not been implemented.
        [Test]
        public void RiscV_dasm_02D8783B()
        {
            AssertCode("@@@", 0x02D8783B);
        }

        // Reko: a decoder for RiscV instruction 00004515 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004515()
        {
            AssertCode("@@@", 0x00004515);
        }

        // Reko: a decoder for RiscV instruction 0000BF5D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF5D()
        {
            AssertCode("@@@", 0x0000BF5D);
        }

        // Reko: a decoder for RiscV instruction 0000BF6D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF6D()
        {
            AssertCode("@@@", 0x0000BF6D);
        }

        // Reko: a decoder for RiscV instruction 0000CF81 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CF81()
        {
            AssertCode("@@@", 0x0000CF81);
        }

        // Reko: a decoder for RiscV instruction 0000C731 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C731()
        {
            AssertCode("@@@", 0x0000C731);
        }

        // Reko: a decoder for RiscV instruction 0000681C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000681C()
        {
            AssertCode("@@@", 0x0000681C);
        }

        // Reko: a decoder for RiscV instruction 00006010 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006010()
        {
            AssertCode("@@@", 0x00006010);
        }

        // Reko: a decoder for RiscV instruction 00004B98 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004B98()
        {
            AssertCode("@@@", 0x00004B98);
        }

        // Reko: a decoder for RiscV instruction 0000BFD5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFD5()
        {
            AssertCode("@@@", 0x0000BFD5);
        }

        // Reko: a decoder for RiscV instruction 00004F98 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004F98()
        {
            AssertCode("@@@", 0x00004F98);
        }

        // Reko: a decoder for RiscV instruction 0000E456 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E456()
        {
            AssertCode("@@@", 0x0000E456);
        }

        // Reko: a decoder for RiscV instruction 00008AAA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008AAA()
        {
            AssertCode("@@@", 0x00008AAA);
        }

        // Reko: a decoder for RiscV instruction 00008085 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008085()
        {
            AssertCode("@@@", 0x00008085);
        }

        // Reko: a decoder for RiscV instruction 00009426 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009426()
        {
            AssertCode("@@@", 0x00009426);
        }

        // Reko: a decoder for RiscV instruction 00009452 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009452()
        {
            AssertCode("@@@", 0x00009452);
        }

        // Reko: a decoder for RiscV instruction 0000600C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000600C()
        {
            AssertCode("@@@", 0x0000600C);
        }

        // Reko: a decoder for RiscV instruction 000089A6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000089A6()
        {
            AssertCode("@@@", 0x000089A6);
        }

        // Reko: a decoder for RiscV instruction 0000BFF9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFF9()
        {
            AssertCode("@@@", 0x0000BFF9);
        }

        // Reko: a decoder for RiscV instruction 00006AA2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006AA2()
        {
            AssertCode("@@@", 0x00006AA2);
        }

        // Reko: a decoder for RiscV instruction 0000D565 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D565()
        {
            AssertCode("@@@", 0x0000D565);
        }

        // Reko: a decoder for RiscV instruction 000084CE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000084CE()
        {
            AssertCode("@@@", 0x000084CE);
        }

        // Reko: a decoder for RiscV instruction 000094D2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000094D2()
        {
            AssertCode("@@@", 0x000094D2);
        }

        // Reko: a decoder for RiscV instruction 0000B7CD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7CD()
        {
            AssertCode("@@@", 0x0000B7CD);
        }

        // Reko: a decoder for RiscV instruction 0000844E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000844E()
        {
            AssertCode("@@@", 0x0000844E);
        }

        // Reko: a decoder for RiscV instruction 0000BF75 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF75()
        {
            AssertCode("@@@", 0x0000BF75);
        }

        // Reko: a decoder for RiscV instruction 00008BAA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008BAA()
        {
            AssertCode("@@@", 0x00008BAA);
        }

        // Reko: a decoder for RiscV instruction 00008DAE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008DAE()
        {
            AssertCode("@@@", 0x00008DAE);
        }

        // Reko: a decoder for RiscV instruction 0000C919 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C919()
        {
            AssertCode("@@@", 0x0000C919);
        }

        // Reko: a decoder for RiscV instruction 00004298 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004298()
        {
            AssertCode("@@@", 0x00004298);
        }

        // Reko: a decoder for RiscV instruction 0000C290 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C290()
        {
            AssertCode("@@@", 0x0000C290);
        }

        // Reko: a decoder for RiscV instruction 000043F4 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_000043F4()
        {
            AssertCode("@@@", 0x000043F4);
        }

        // Reko: a decoder for RiscV instruction 00004A81 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004A81()
        {
            AssertCode("@@@", 0x00004A81);
        }

        // Reko: a decoder for RiscV instruction 00004A01 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004A01()
        {
            AssertCode("@@@", 0x00004A01);
        }

        // Reko: a decoder for RiscV instruction 00008CEA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008CEA()
        {
            AssertCode("@@@", 0x00008CEA);
        }

        // Reko: a decoder for RiscV instruction 0000CB61 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CB61()
        {
            AssertCode("@@@", 0x0000CB61);
        }

        // Reko: a decoder for RiscV instruction 00004799 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004799()
        {
            AssertCode("@@@", 0x00004799);
        }

        // Reko: a decoder for RiscV instruction 00008B3D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008B3D()
        {
            AssertCode("@@@", 0x00008B3D);
        }

        // Reko: a decoder for RiscV instruction 00009762 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009762()
        {
            AssertCode("@@@", 0x00009762);
        }

        // Reko: a decoder for RiscV instruction 00008702 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008702()
        {
            AssertCode("@@@", 0x00008702);
        }

        // Reko: a decoder for RiscV instruction 000086DA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000086DA()
        {
            AssertCode("@@@", 0x000086DA);
        }

        // Reko: a decoder for RiscV instruction 0000D6A9 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D6A9()
        {
            AssertCode("@@@", 0x0000D6A9);
        }

        // Reko: a decoder for RiscV instruction 0000D331 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D331()
        {
            AssertCode("@@@", 0x0000D331);
        }

        // Reko: a decoder for RiscV instruction 00004405 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004405()
        {
            AssertCode("@@@", 0x00004405);
        }

        // Reko: a decoder for RiscV instruction 0000F321 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F321()
        {
            AssertCode("@@@", 0x0000F321);
        }

        // Reko: a decoder for RiscV instruction 000043F8 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_000043F8()
        {
            AssertCode("@@@", 0x000043F8);
        }

        // Reko: a decoder for RiscV instruction 00009F11 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009F11()
        {
            AssertCode("@@@", 0x00009F11);
        }

        // Reko: a decoder for RiscV instruction 000084EA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000084EA()
        {
            AssertCode("@@@", 0x000084EA);
        }

        // Reko: a decoder for RiscV instruction 0000BF29 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF29()
        {
            AssertCode("@@@", 0x0000BF29);
        }

        // Reko: a decoder for RiscV instruction 000043F0 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_000043F0()
        {
            AssertCode("@@@", 0x000043F0);
        }

        // Reko: a decoder for RiscV instruction 0000BDED at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BDED()
        {
            AssertCode("@@@", 0x0000BDED);
        }

        // Reko: a decoder for RiscV instruction 00004384 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004384()
        {
            AssertCode("@@@", 0x00004384);
        }

        // Reko: a decoder for RiscV instruction 0000C8CD at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C8CD()
        {
            AssertCode("@@@", 0x0000C8CD);
        }

        // Reko: a decoder for RiscV instruction 0000977D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_0000977D()
        {
            AssertCode("@@@", 0x0000977D);
        }

        // Reko: a decoder for RiscV instruction 00008EF9 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008EF9()
        {
            AssertCode("@@@", 0x00008EF9);
        }

        // Reko: a decoder for RiscV instruction 0000BDD1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BDD1()
        {
            AssertCode("@@@", 0x0000BDD1);
        }

        // Reko: a decoder for RiscV instruction 0000B555 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B555()
        {
            AssertCode("@@@", 0x0000B555);
        }

        // Reko: a decoder for RiscV instruction 000084DA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000084DA()
        {
            AssertCode("@@@", 0x000084DA);
        }

        // Reko: a decoder for RiscV instruction 0000BD95 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD95()
        {
            AssertCode("@@@", 0x0000BD95);
        }

        // Reko: a decoder for RiscV instruction 000099BA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000099BA()
        {
            AssertCode("@@@", 0x000099BA);
        }

        // Reko: a decoder for RiscV instruction 000047A9 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000047A9()
        {
            AssertCode("@@@", 0x000047A9);
        }

        // Reko: a decoder for RiscV instruction 000046A5 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000046A5()
        {
            AssertCode("@@@", 0x000046A5);
        }

        // Reko: a decoder for RiscV instruction 0000BD6D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD6D()
        {
            AssertCode("@@@", 0x0000BD6D);
        }

        // Reko: a decoder for RiscV instruction 00009F01 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009F01()
        {
            AssertCode("@@@", 0x00009F01);
        }

        // Reko: a decoder for RiscV instruction 000084A2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000084A2()
        {
            AssertCode("@@@", 0x000084A2);
        }

        // Reko: a decoder for RiscV instruction 000043BC at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_000043BC()
        {
            AssertCode("@@@", 0x000043BC);
        }

        // Reko: a decoder for RiscV instruction 0000CFA9 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CFA9()
        {
            AssertCode("@@@", 0x0000CFA9);
        }

        // Reko: a decoder for RiscV instruction 00006300 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006300()
        {
            AssertCode("@@@", 0x00006300);
        }

        // Reko: a decoder for RiscV instruction 0000B38D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B38D()
        {
            AssertCode("@@@", 0x0000B38D);
        }

        // Reko: a decoder for RiscV instruction 0000C298 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C298()
        {
            AssertCode("@@@", 0x0000C298);
        }

        // Reko: a decoder for RiscV instruction 0000BB0D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB0D()
        {
            AssertCode("@@@", 0x0000BB0D);
        }

        // Reko: a decoder for RiscV instruction 0000431C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000431C()
        {
            AssertCode("@@@", 0x0000431C);
        }

        // Reko: a decoder for RiscV instruction 0000C314 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C314()
        {
            AssertCode("@@@", 0x0000C314);
        }

        // Reko: a decoder for RiscV instruction 0000EA91 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EA91()
        {
            AssertCode("@@@", 0x0000EA91);
        }

        // Reko: a decoder for RiscV instruction 0000C31C at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C31C()
        {
            AssertCode("@@@", 0x0000C31C);
        }

        // Reko: a decoder for RiscV instruction 000067A1 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_000067A1()
        {
            AssertCode("@@@", 0x000067A1);
        }

        // Reko: a decoder for RiscV instruction 000067C1 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_000067C1()
        {
            AssertCode("@@@", 0x000067C1);
        }

        // Reko: a decoder for RiscV instruction 00006791 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006791()
        {
            AssertCode("@@@", 0x00006791);
        }

        // Reko: a decoder for RiscV instruction 00009C2D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009C2D()
        {
            AssertCode("@@@", 0x00009C2D);
        }

        // Reko: a decoder for RiscV instruction 02B4443B at address 00100000 has not been implemented.
        [Test]
        public void RiscV_dasm_02B4443B()
        {
            AssertCode("@@@", 0x02B4443B);
        }

        // Reko: a decoder for RiscV instruction 0000460D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000460D()
        {
            AssertCode("@@@", 0x0000460D);
        }

        // Reko: a decoder for RiscV instruction 00009526 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009526()
        {
            AssertCode("@@@", 0x00009526);
        }

        // Reko: a decoder for RiscV instruction 0000C7E1 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C7E1()
        {
            AssertCode("@@@", 0x0000C7E1);
        }

        // Reko: a decoder for RiscV instruction 00004899 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004899()
        {
            AssertCode("@@@", 0x00004899);
        }

        // Reko: a decoder for RiscV instruction 0000A811 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A811()
        {
            AssertCode("@@@", 0x0000A811);
        }

        // Reko: a decoder for RiscV instruction 00004F9C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004F9C()
        {
            AssertCode("@@@", 0x00004F9C);
        }

        // Reko: a decoder for RiscV instruction 0000CABD at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CABD()
        {
            AssertCode("@@@", 0x0000CABD);
        }

        // Reko: a decoder for RiscV instruction 00009F2D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009F2D()
        {
            AssertCode("@@@", 0x00009F2D);
        }

        // Reko: a decoder for RiscV instruction 00009F3D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009F3D()
        {
            AssertCode("@@@", 0x00009F3D);
        }

        // Reko: a decoder for RiscV instruction 0000CE09 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CE09()
        {
            AssertCode("@@@", 0x0000CE09);
        }

        // Reko: a decoder for RiscV instruction 000053D4 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_000053D4()
        {
            AssertCode("@@@", 0x000053D4);
        }

        // Reko: a decoder for RiscV instruction 00008ABD at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008ABD()
        {
            AssertCode("@@@", 0x00008ABD);
        }

        // Reko: a decoder for RiscV instruction 0000C641 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C641()
        {
            AssertCode("@@@", 0x0000C641);
        }

        // Reko: a decoder for RiscV instruction 0000F66D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F66D()
        {
            AssertCode("@@@", 0x0000F66D);
        }

        // Reko: a decoder for RiscV instruction 00004390 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004390()
        {
            AssertCode("@@@", 0x00004390);
        }

        // Reko: a decoder for RiscV instruction 000065C1 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_000065C1()
        {
            AssertCode("@@@", 0x000065C1);
        }

        // Reko: a decoder for RiscV instruction 00009F35 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009F35()
        {
            AssertCode("@@@", 0x00009F35);
        }

        // Reko: a decoder for RiscV instruction 0000B779 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B779()
        {
            AssertCode("@@@", 0x0000B779);
        }

        // Reko: a decoder for RiscV instruction 0000BDF1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BDF1()
        {
            AssertCode("@@@", 0x0000BDF1);
        }

        // Reko: a decoder for RiscV instruction 0000BFB5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFB5()
        {
            AssertCode("@@@", 0x0000BFB5);
        }

        // Reko: a decoder for RiscV instruction 0000B719 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B719()
        {
            AssertCode("@@@", 0x0000B719);
        }

        // Reko: a decoder for RiscV instruction 0000C7CD at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C7CD()
        {
            AssertCode("@@@", 0x0000C7CD);
        }

        // Reko: a decoder for RiscV instruction 0000EAB1 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EAB1()
        {
            AssertCode("@@@", 0x0000EAB1);
        }

        // Reko: a decoder for RiscV instruction 0000863E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000863E()
        {
            AssertCode("@@@", 0x0000863E);
        }

        // Reko: a decoder for RiscV instruction 0000CA85 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CA85()
        {
            AssertCode("@@@", 0x0000CA85);
        }

        // Reko: a decoder for RiscV instruction 0000E1C1 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E1C1()
        {
            AssertCode("@@@", 0x0000E1C1);
        }

        // Reko: a decoder for RiscV instruction 000046E1 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000046E1()
        {
            AssertCode("@@@", 0x000046E1);
        }

        // Reko: a decoder for RiscV instruction 00008FD1 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008FD1()
        {
            AssertCode("@@@", 0x00008FD1);
        }

        // Reko: a decoder for RiscV instruction 0000C691 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C691()
        {
            AssertCode("@@@", 0x0000C691);
        }

        // Reko: a decoder for RiscV instruction 0000CB95 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CB95()
        {
            AssertCode("@@@", 0x0000CB95);
        }

        // Reko: a decoder for RiscV instruction 00008E5D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008E5D()
        {
            AssertCode("@@@", 0x00008E5D);
        }

        // Reko: a decoder for RiscV instruction 0000C310 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C310()
        {
            AssertCode("@@@", 0x0000C310);
        }

        // Reko: a decoder for RiscV instruction 00004699 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004699()
        {
            AssertCode("@@@", 0x00004699);
        }

        // Reko: a decoder for RiscV instruction 00008A61 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008A61()
        {
            AssertCode("@@@", 0x00008A61);
        }

        // Reko: a decoder for RiscV instruction 0000DDF1 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000DDF1()
        {
            AssertCode("@@@", 0x0000DDF1);
        }

        // Reko: a decoder for RiscV instruction 00007155 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00007155()
        {
            AssertCode("@@@", 0x00007155);
        }

        // Reko: a decoder for RiscV instruction 0000F54E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F54E()
        {
            AssertCode("@@@", 0x0000F54E);
        }

        // Reko: a decoder for RiscV instruction 0000F94A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F94A()
        {
            AssertCode("@@@", 0x0000F94A);
        }

        // Reko: a decoder for RiscV instruction 0000F152 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F152()
        {
            AssertCode("@@@", 0x0000F152);
        }

        // Reko: a decoder for RiscV instruction 0000E586 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E586()
        {
            AssertCode("@@@", 0x0000E586);
        }

        // Reko: a decoder for RiscV instruction 0000E1A2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E1A2()
        {
            AssertCode("@@@", 0x0000E1A2);
        }

        // Reko: a decoder for RiscV instruction 0000FD26 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FD26()
        {
            AssertCode("@@@", 0x0000FD26);
        }

        // Reko: a decoder for RiscV instruction 0000ED56 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000ED56()
        {
            AssertCode("@@@", 0x0000ED56);
        }

        // Reko: a decoder for RiscV instruction 0000737D at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_0000737D()
        {
            AssertCode("@@@", 0x0000737D);
        }

        // Reko: a decoder for RiscV instruction 00006605 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006605()
        {
            AssertCode("@@@", 0x00006605);
        }

        // Reko: a decoder for RiscV instruction 0000911A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000911A()
        {
            AssertCode("@@@", 0x0000911A);
        }

        // Reko: a decoder for RiscV instruction 0000960A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000960A()
        {
            AssertCode("@@@", 0x0000960A);
        }

        // Reko: a decoder for RiscV instruction 0000E214 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E214()
        {
            AssertCode("@@@", 0x0000E214);
        }

        // Reko: a decoder for RiscV instruction 00006A85 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006A85()
        {
            AssertCode("@@@", 0x00006A85);
        }

        // Reko: a decoder for RiscV instruction 0000747D at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_0000747D()
        {
            AssertCode("@@@", 0x0000747D);
        }

        // Reko: a decoder for RiscV instruction 0000978A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000978A()
        {
            AssertCode("@@@", 0x0000978A);
        }

        // Reko: a decoder for RiscV instruction 00006685 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006685()
        {
            AssertCode("@@@", 0x00006685);
        }

        // Reko: a decoder for RiscV instruction 0000943E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000943E()
        {
            AssertCode("@@@", 0x0000943E);
        }

        // Reko: a decoder for RiscV instruction 0000970A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000970A()
        {
            AssertCode("@@@", 0x0000970A);
        }

        // Reko: a decoder for RiscV instruction 000077FD at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_000077FD()
        {
            AssertCode("@@@", 0x000077FD);
        }

        // Reko: a decoder for RiscV instruction 0000663D at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_0000663D()
        {
            AssertCode("@@@", 0x0000663D);
        }

        // Reko: a decoder for RiscV instruction 00006689 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006689()
        {
            AssertCode("@@@", 0x00006689);
        }

        // Reko: a decoder for RiscV instruction 00008F71 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008F71()
        {
            AssertCode("@@@", 0x00008F71);
        }

        // Reko: a decoder for RiscV instruction 00006305 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006305()
        {
            AssertCode("@@@", 0x00006305);
        }

        // Reko: a decoder for RiscV instruction 000060AE at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000060AE()
        {
            AssertCode("@@@", 0x000060AE);
        }

        // Reko: a decoder for RiscV instruction 0000640E at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_0000640E()
        {
            AssertCode("@@@", 0x0000640E);
        }

        // Reko: a decoder for RiscV instruction 000074EA at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000074EA()
        {
            AssertCode("@@@", 0x000074EA);
        }

        // Reko: a decoder for RiscV instruction 0000794A at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_0000794A()
        {
            AssertCode("@@@", 0x0000794A);
        }

        // Reko: a decoder for RiscV instruction 000079AA at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000079AA()
        {
            AssertCode("@@@", 0x000079AA);
        }

        // Reko: a decoder for RiscV instruction 00007A0A at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007A0A()
        {
            AssertCode("@@@", 0x00007A0A);
        }

        // Reko: a decoder for RiscV instruction 00006AEA at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006AEA()
        {
            AssertCode("@@@", 0x00006AEA);
        }

        [Test]
        public void RiscV_dasm_c_addi16sp()
        {
            AssertCode("c.addi16sp\t000000D0", 0x6169);
        }

        // Reko: a decoder for RiscV instruction 0000767D at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_0000767D()
        {
            AssertCode("@@@", 0x0000767D);
        }

        // Reko: a decoder for RiscV instruction 0000963E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000963E()
        {
            AssertCode("@@@", 0x0000963E);
        }

        // Reko: a decoder for RiscV instruction 000087CA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087CA()
        {
            AssertCode("@@@", 0x000087CA);
        }

        // Reko: a decoder for RiscV instruction 0000C11D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C11D()
        {
            AssertCode("@@@", 0x0000C11D);
        }

        // Reko: a decoder for RiscV instruction 0000FF8D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FF8D()
        {
            AssertCode("@@@", 0x0000FF8D);
        }

        // Reko: a decoder for RiscV instruction 0000B5C5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B5C5()
        {
            AssertCode("@@@", 0x0000B5C5);
        }

        // Reko: a decoder for RiscV instruction 000087AE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087AE()
        {
            AssertCode("@@@", 0x000087AE);
        }

        // Reko: a decoder for RiscV instruction 00004641 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004641()
        {
            AssertCode("@@@", 0x00004641);
        }

        // Reko: a decoder for RiscV instruction 0000EC56 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC56()
        {
            AssertCode("@@@", 0x0000EC56);
        }

        // Reko: a decoder for RiscV instruction 0000E85A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E85A()
        {
            AssertCode("@@@", 0x0000E85A);
        }

        // Reko: a decoder for RiscV instruction 0000E45E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E45E()
        {
            AssertCode("@@@", 0x0000E45E);
        }

        // Reko: a decoder for RiscV instruction 0000E408 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E408()
        {
            AssertCode("@@@", 0x0000E408);
        }

        // Reko: a decoder for RiscV instruction 00004881 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004881()
        {
            AssertCode("@@@", 0x00004881);
        }

        // Reko: a decoder for RiscV instruction 0000A801 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A801()
        {
            AssertCode("@@@", 0x0000A801);
        }

        // Reko: a decoder for RiscV instruction 0000CF01 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CF01()
        {
            AssertCode("@@@", 0x0000CF01);
        }

        // Reko: a decoder for RiscV instruction 0000EE2D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EE2D()
        {
            AssertCode("@@@", 0x0000EE2D);
        }

        // Reko: a decoder for RiscV instruction 00008F6D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008F6D()
        {
            AssertCode("@@@", 0x00008F6D);
        }

        // Reko: a decoder for RiscV instruction 0000D67D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D67D()
        {
            AssertCode("@@@", 0x0000D67D);
        }

        // Reko: a decoder for RiscV instruction 0000F7ED at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F7ED()
        {
            AssertCode("@@@", 0x0000F7ED);
        }

        // Reko: a decoder for RiscV instruction 0000EA39 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EA39()
        {
            AssertCode("@@@", 0x0000EA39);
        }

        // Reko: a decoder for RiscV instruction 00008A26 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008A26()
        {
            AssertCode("@@@", 0x00008A26);
        }

        // Reko: a decoder for RiscV instruction 0000640C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000640C()
        {
            AssertCode("@@@", 0x0000640C);
        }

        // Reko: a decoder for RiscV instruction 000095CE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000095CE()
        {
            AssertCode("@@@", 0x000095CE);
        }

        // Reko: a decoder for RiscV instruction 00009902 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009902()
        {
            AssertCode("@@@", 0x00009902);
        }

        // Reko: a decoder for RiscV instruction 0000DD71 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000DD71()
        {
            AssertCode("@@@", 0x0000DD71);
        }

        // Reko: a decoder for RiscV instruction 00006AE2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006AE2()
        {
            AssertCode("@@@", 0x00006AE2);
        }

        // Reko: a decoder for RiscV instruction 00006B42 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006B42()
        {
            AssertCode("@@@", 0x00006B42);
        }

        // Reko: a decoder for RiscV instruction 00006BA2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006BA2()
        {
            AssertCode("@@@", 0x00006BA2);
        }

        // Reko: a decoder for RiscV instruction 0000E380 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E380()
        {
            AssertCode("@@@", 0x0000E380);
        }

        // Reko: a decoder for RiscV instruction 0000E018 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E018()
        {
            AssertCode("@@@", 0x0000E018);
        }

        // Reko: a decoder for RiscV instruction 0000434C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000434C()
        {
            AssertCode("@@@", 0x0000434C);
        }

        // Reko: a decoder for RiscV instruction 00006718 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006718()
        {
            AssertCode("@@@", 0x00006718);
        }

        // Reko: a decoder for RiscV instruction 00006B08 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006B08()
        {
            AssertCode("@@@", 0x00006B08);
        }

        // Reko: a decoder for RiscV instruction 0000D3ED at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D3ED()
        {
            AssertCode("@@@", 0x0000D3ED);
        }

        // Reko: a decoder for RiscV instruction 0000D7E1 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D7E1()
        {
            AssertCode("@@@", 0x0000D7E1);
        }

        // Reko: a decoder for RiscV instruction 0000FC5E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FC5E()
        {
            AssertCode("@@@", 0x0000FC5E);
        }

        // Reko: a decoder for RiscV instruction 0000F466 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F466()
        {
            AssertCode("@@@", 0x0000F466);
        }

        // Reko: a decoder for RiscV instruction 0000E0DA at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E0DA()
        {
            AssertCode("@@@", 0x0000E0DA);
        }

        // Reko: a decoder for RiscV instruction 000097E6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000097E6()
        {
            AssertCode("@@@", 0x000097E6);
        }

        // Reko: a decoder for RiscV instruction 0000F862 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F862()
        {
            AssertCode("@@@", 0x0000F862);
        }

        // Reko: a decoder for RiscV instruction 00009B22 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009B22()
        {
            AssertCode("@@@", 0x00009B22);
        }

        // Reko: a decoder for RiscV instruction 000066E2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000066E2()
        {
            AssertCode("@@@", 0x000066E2);
        }

        // Reko: a decoder for RiscV instruction 00006B06 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006B06()
        {
            AssertCode("@@@", 0x00006B06);
        }

        // Reko: a decoder for RiscV instruction 00007BE2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007BE2()
        {
            AssertCode("@@@", 0x00007BE2);
        }

        // Reko: a decoder for RiscV instruction 00007C42 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007C42()
        {
            AssertCode("@@@", 0x00007C42);
        }

        // Reko: a decoder for RiscV instruction 00007CA2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007CA2()
        {
            AssertCode("@@@", 0x00007CA2);
        }

        // Reko: a decoder for RiscV instruction 0000C901 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C901()
        {
            AssertCode("@@@", 0x0000C901);
        }

        // Reko: a decoder for RiscV instruction 0000B759 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B759()
        {
            AssertCode("@@@", 0x0000B759);
        }

        // Reko: a decoder for RiscV instruction 0000BF79 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF79()
        {
            AssertCode("@@@", 0x0000BF79);
        }

        // Reko: a decoder for RiscV instruction 0000FF69 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FF69()
        {
            AssertCode("@@@", 0x0000FF69);
        }

        // Reko: a decoder for RiscV instruction 0000C398 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C398()
        {
            AssertCode("@@@", 0x0000C398);
        }

        // Reko: a decoder for RiscV instruction 0000B749 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B749()
        {
            AssertCode("@@@", 0x0000B749);
        }

        // Reko: a decoder for RiscV instruction 0000B785 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B785()
        {
            AssertCode("@@@", 0x0000B785);
        }

        // Reko: a decoder for RiscV instruction 0000BF3D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF3D()
        {
            AssertCode("@@@", 0x0000BF3D);
        }

        // Reko: a decoder for RiscV instruction 0000F11D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F11D()
        {
            AssertCode("@@@", 0x0000F11D);
        }

        // Reko: a decoder for RiscV instruction 000046AD at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000046AD()
        {
            AssertCode("@@@", 0x000046AD);
        }

        // Reko: a decoder for RiscV instruction 0000CB54 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000CB54()
        {
            AssertCode("@@@", 0x0000CB54);
        }

        // Reko: a decoder for RiscV instruction 0000BF19 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF19()
        {
            AssertCode("@@@", 0x0000BF19);
        }

        // Reko: a decoder for RiscV instruction 000046B1 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000046B1()
        {
            AssertCode("@@@", 0x000046B1);
        }

        // Reko: a decoder for RiscV instruction 0000B5F5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B5F5()
        {
            AssertCode("@@@", 0x0000B5F5);
        }

        // Reko: a decoder for RiscV instruction 0000B5C9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B5C9()
        {
            AssertCode("@@@", 0x0000B5C9);
        }

        // Reko: a decoder for RiscV instruction 0000B55D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B55D()
        {
            AssertCode("@@@", 0x0000B55D);
        }

        // Reko: a decoder for RiscV instruction 00004695 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004695()
        {
            AssertCode("@@@", 0x00004695);
        }

        // Reko: a decoder for RiscV instruction 0000BDB5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BDB5()
        {
            AssertCode("@@@", 0x0000BDB5);
        }

        // Reko: a decoder for RiscV instruction 000046BD at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000046BD()
        {
            AssertCode("@@@", 0x000046BD);
        }

        // Reko: a decoder for RiscV instruction 0000B5A9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B5A9()
        {
            AssertCode("@@@", 0x0000B5A9);
        }

        // Reko: a decoder for RiscV instruction 0000BD21 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD21()
        {
            AssertCode("@@@", 0x0000BD21);
        }

        // Reko: a decoder for RiscV instruction 0000BD35 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD35()
        {
            AssertCode("@@@", 0x0000BD35);
        }

        // Reko: a decoder for RiscV instruction 0000B3DD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B3DD()
        {
            AssertCode("@@@", 0x0000B3DD);
        }

        // Reko: a decoder for RiscV instruction 0000B3E9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B3E9()
        {
            AssertCode("@@@", 0x0000B3E9);
        }

        // Reko: a decoder for RiscV instruction 0000B599 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B599()
        {
            AssertCode("@@@", 0x0000B599);
        }

        // Reko: a decoder for RiscV instruction 0000BBFD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BBFD()
        {
            AssertCode("@@@", 0x0000BBFD);
        }

        // Reko: a decoder for RiscV instruction 0000BB69 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB69()
        {
            AssertCode("@@@", 0x0000BB69);
        }

        // Reko: a decoder for RiscV instruction 0000BBC9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BBC9()
        {
            AssertCode("@@@", 0x0000BBC9);
        }

        // Reko: a decoder for RiscV instruction 0000BBBD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BBBD()
        {
            AssertCode("@@@", 0x0000BBBD);
        }

        // Reko: a decoder for RiscV instruction 000046B9 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000046B9()
        {
            AssertCode("@@@", 0x000046B9);
        }

        // Reko: a decoder for RiscV instruction 0000BB91 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB91()
        {
            AssertCode("@@@", 0x0000BB91);
        }

        // Reko: a decoder for RiscV instruction 0000B32D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B32D()
        {
            AssertCode("@@@", 0x0000B32D);
        }

        // Reko: a decoder for RiscV instruction 0000BB01 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB01()
        {
            AssertCode("@@@", 0x0000BB01);
        }

        // Reko: a decoder for RiscV instruction 0000B9F9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B9F9()
        {
            AssertCode("@@@", 0x0000B9F9);
        }

        // Reko: a decoder for RiscV instruction 000046A9 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000046A9()
        {
            AssertCode("@@@", 0x000046A9);
        }

        // Reko: a decoder for RiscV instruction 0000B955 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B955()
        {
            AssertCode("@@@", 0x0000B955);
        }

        // Reko: a decoder for RiscV instruction 0000B1F5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B1F5()
        {
            AssertCode("@@@", 0x0000B1F5);
        }

        // Reko: a decoder for RiscV instruction 0000BB11 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB11()
        {
            AssertCode("@@@", 0x0000BB11);
        }

        // Reko: a decoder for RiscV instruction 0000B1E5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B1E5()
        {
            AssertCode("@@@", 0x0000B1E5);
        }

        // Reko: a decoder for RiscV instruction 0000B1DD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B1DD()
        {
            AssertCode("@@@", 0x0000B1DD);
        }

        // Reko: a decoder for RiscV instruction 0000B961 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B961()
        {
            AssertCode("@@@", 0x0000B961);
        }

        // Reko: a decoder for RiscV instruction 0000B151 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B151()
        {
            AssertCode("@@@", 0x0000B151);
        }

        // Reko: a decoder for RiscV instruction 0000B9B5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B9B5()
        {
            AssertCode("@@@", 0x0000B9B5);
        }

        // Reko: a decoder for RiscV instruction 0000C388 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C388()
        {
            AssertCode("@@@", 0x0000C388);
        }

        // Reko: a decoder for RiscV instruction 0000B1B1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B1B1()
        {
            AssertCode("@@@", 0x0000B1B1);
        }

        // Reko: a decoder for RiscV instruction 0000B931 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B931()
        {
            AssertCode("@@@", 0x0000B931);
        }

        // Reko: a decoder for RiscV instruction 0000B129 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B129()
        {
            AssertCode("@@@", 0x0000B129);
        }

        // Reko: a decoder for RiscV instruction 0000BEE5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BEE5()
        {
            AssertCode("@@@", 0x0000BEE5);
        }

        // Reko: a decoder for RiscV instruction 0000B6DD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B6DD()
        {
            AssertCode("@@@", 0x0000B6DD);
        }

        // Reko: a decoder for RiscV instruction 0000BED1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BED1()
        {
            AssertCode("@@@", 0x0000BED1);
        }

        // Reko: a decoder for RiscV instruction 0000B6C9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B6C9()
        {
            AssertCode("@@@", 0x0000B6C9);
        }

        // Reko: a decoder for RiscV instruction 0000BE45 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BE45()
        {
            AssertCode("@@@", 0x0000BE45);
        }

        // Reko: a decoder for RiscV instruction 0000BE79 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BE79()
        {
            AssertCode("@@@", 0x0000BE79);
        }

        // Reko: a decoder for RiscV instruction 0000B671 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B671()
        {
            AssertCode("@@@", 0x0000B671);
        }

        // Reko: a decoder for RiscV instruction 0000BE8D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BE8D()
        {
            AssertCode("@@@", 0x0000BE8D);
        }

        // Reko: a decoder for RiscV instruction 0000B6A5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B6A5()
        {
            AssertCode("@@@", 0x0000B6A5);
        }

        // Reko: a decoder for RiscV instruction 0000BEA1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BEA1()
        {
            AssertCode("@@@", 0x0000BEA1);
        }

        // Reko: a decoder for RiscV instruction 0000B6B9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B6B9()
        {
            AssertCode("@@@", 0x0000B6B9);
        }

        // Reko: a decoder for RiscV instruction 0000BE0D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BE0D()
        {
            AssertCode("@@@", 0x0000BE0D);
        }

        // Reko: a decoder for RiscV instruction 0000EF8D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EF8D()
        {
            AssertCode("@@@", 0x0000EF8D);
        }

        // Reko: a decoder for RiscV instruction 0000C088 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C088()
        {
            AssertCode("@@@", 0x0000C088);
        }

        // Reko: a decoder for RiscV instruction 0000A809 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A809()
        {
            AssertCode("@@@", 0x0000A809);
        }

        // Reko: a decoder for RiscV instruction 0000492D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000492D()
        {
            AssertCode("@@@", 0x0000492D);
        }

        // Reko: a decoder for RiscV instruction 0000EC5E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC5E()
        {
            AssertCode("@@@", 0x0000EC5E);
        }

        // Reko: a decoder for RiscV instruction 0000E862 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E862()
        {
            AssertCode("@@@", 0x0000E862);
        }

        // Reko: a decoder for RiscV instruction 0000E466 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E466()
        {
            AssertCode("@@@", 0x0000E466);
        }

        // Reko: a decoder for RiscV instruction 0000E06A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E06A()
        {
            AssertCode("@@@", 0x0000E06A);
        }

        // Reko: a decoder for RiscV instruction 00009442 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009442()
        {
            AssertCode("@@@", 0x00009442);
        }

        // Reko: a decoder for RiscV instruction 00006014 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006014()
        {
            AssertCode("@@@", 0x00006014);
        }

        // Reko: a decoder for RiscV instruction 000048A5 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000048A5()
        {
            AssertCode("@@@", 0x000048A5);
        }

        // Reko: a decoder for RiscV instruction 00006BE2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006BE2()
        {
            AssertCode("@@@", 0x00006BE2);
        }

        // Reko: a decoder for RiscV instruction 00006C42 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006C42()
        {
            AssertCode("@@@", 0x00006C42);
        }

        // Reko: a decoder for RiscV instruction 00006CA2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006CA2()
        {
            AssertCode("@@@", 0x00006CA2);
        }

        // Reko: a decoder for RiscV instruction 00006D02 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006D02()
        {
            AssertCode("@@@", 0x00006D02);
        }

        // Reko: a decoder for RiscV instruction 00008566 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008566()
        {
            AssertCode("@@@", 0x00008566);
        }

        // Reko: a decoder for RiscV instruction 000095BA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000095BA()
        {
            AssertCode("@@@", 0x000095BA);
        }

        // Reko: a decoder for RiscV instruction 0000D969 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D969()
        {
            AssertCode("@@@", 0x0000D969);
        }

        // Reko: a decoder for RiscV instruction 0000DD49 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000DD49()
        {
            AssertCode("@@@", 0x0000DD49);
        }

        // Reko: a decoder for RiscV instruction 0000B7E1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7E1()
        {
            AssertCode("@@@", 0x0000B7E1);
        }

        // Reko: a decoder for RiscV instruction 0000DD35 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000DD35()
        {
            AssertCode("@@@", 0x0000DD35);
        }

        // Reko: a decoder for RiscV instruction 0000B76D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B76D()
        {
            AssertCode("@@@", 0x0000B76D);
        }

        // Reko: a decoder for RiscV instruction 00008BC1 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008BC1()
        {
            AssertCode("@@@", 0x00008BC1);
        }

        // Reko: a decoder for RiscV instruction 0000F3BD at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F3BD()
        {
            AssertCode("@@@", 0x0000F3BD);
        }

        // Reko: a decoder for RiscV instruction 000047A5 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_000047A5()
        {
            AssertCode("@@@", 0x000047A5);
        }

        // Reko: a decoder for RiscV instruction 0000B3A1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B3A1()
        {
            AssertCode("@@@", 0x0000B3A1);
        }

        [Test]
        public void RiscV_dasm_beqz_backward()
        {
            AssertCode("c.beqz\ta5,00000000000FFF06", 0xD399);
        }

        // Reko: a decoder for RiscV instruction 0000B5B9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B5B9()
        {
            AssertCode("@@@", 0x0000B5B9);
        }

        // Reko: a decoder for RiscV instruction 0000BD15 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD15()
        {
            AssertCode("@@@", 0x0000BD15);
        }

        // Reko: a decoder for RiscV instruction 0000BD39 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD39()
        {
            AssertCode("@@@", 0x0000BD39);
        }

        // Reko: a decoder for RiscV instruction 00004388 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004388()
        {
            AssertCode("@@@", 0x00004388);
        }

        // Reko: a decoder for RiscV instruction 00008D7D at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008D7D()
        {
            AssertCode("@@@", 0x00008D7D);
        }

        [Test]
        public void RiscV_dasm_addiw_sign_extend()
        {
            AssertCode("c.addiw\tt1,00000000", 0x00002301);
        }

        [Test]
        public void RiscV_dasm_li()
        {
            AssertCode("c.li\tt2,00000001", 0x00004385);
        }

        // Reko: a decoder for RiscV instruction 00009732 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009732()
        {
            AssertCode("@@@", 0x00009732);
        }

        // Reko: a decoder for RiscV instruction 0000BB9D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB9D()
        {
            AssertCode("@@@", 0x0000BB9D);
        }

        // Reko: a decoder for RiscV instruction 0000BD31 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD31()
        {
            AssertCode("@@@", 0x0000BD31);
        }

        // Reko: a decoder for RiscV instruction 0000B531 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B531()
        {
            AssertCode("@@@", 0x0000B531);
        }

        // Reko: a decoder for RiscV instruction 0000BBF5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BBF5()
        {
            AssertCode("@@@", 0x0000BBF5);
        }

        // Reko: a decoder for RiscV instruction 0000B3F5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B3F5()
        {
            AssertCode("@@@", 0x0000B3F5);
        }

        // Reko: a decoder for RiscV instruction 0000BBE9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BBE9()
        {
            AssertCode("@@@", 0x0000BBE9);
        }

        // Reko: a decoder for RiscV instruction 00008B89 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008B89()
        {
            AssertCode("@@@", 0x00008B89);
        }

        // Reko: a decoder for RiscV instruction 0000B355 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B355()
        {
            AssertCode("@@@", 0x0000B355);
        }

        // Reko: a decoder for RiscV instruction 0000C09C at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C09C()
        {
            AssertCode("@@@", 0x0000C09C);
        }

        // Reko: a decoder for RiscV instruction 0000BB61 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB61()
        {
            AssertCode("@@@", 0x0000BB61);
        }

        // Reko: a decoder for RiscV instruction 00008536 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008536()
        {
            AssertCode("@@@", 0x00008536);
        }

        // Reko: a decoder for RiscV instruction 0000BE9D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BE9D()
        {
            AssertCode("@@@", 0x0000BE9D);
        }

        // Reko: a decoder for RiscV instruction 0000BBA5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BBA5()
        {
            AssertCode("@@@", 0x0000BBA5);
        }

        // Reko: a decoder for RiscV instruction 0000BBA9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BBA9()
        {
            AssertCode("@@@", 0x0000BBA9);
        }

        // Reko: a decoder for RiscV instruction 0000B399 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B399()
        {
            AssertCode("@@@", 0x0000B399);
        }

        // Reko: a decoder for RiscV instruction 0000B615 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B615()
        {
            AssertCode("@@@", 0x0000B615);
        }

        // Reko: a decoder for RiscV instruction 0000A0E9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A0E9()
        {
            AssertCode("@@@", 0x0000A0E9);
        }

        // Reko: a decoder for RiscV instruction 0000473D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000473D()
        {
            AssertCode("@@@", 0x0000473D);
        }

        // Reko: a decoder for RiscV instruction 0000CBD8 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000CBD8()
        {
            AssertCode("@@@", 0x0000CBD8);
        }

        // Reko: a decoder for RiscV instruction 0000BCE9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BCE9()
        {
            AssertCode("@@@", 0x0000BCE9);
        }

        // Reko: a decoder for RiscV instruction 0000B9C9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B9C9()
        {
            AssertCode("@@@", 0x0000B9C9);
        }

        // Reko: a decoder for RiscV instruction 0000458D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000458D()
        {
            AssertCode("@@@", 0x0000458D);
        }

        // Reko: a decoder for RiscV instruction 0000BC45 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BC45()
        {
            AssertCode("@@@", 0x0000BC45);
        }

        // Reko: a decoder for RiscV instruction 0000B17D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B17D()
        {
            AssertCode("@@@", 0x0000B17D);
        }

        // Reko: a decoder for RiscV instruction 0000B14D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B14D()
        {
            AssertCode("@@@", 0x0000B14D);
        }

        // Reko: a decoder for RiscV instruction 0000B959 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B959()
        {
            AssertCode("@@@", 0x0000B959);
        }

        // Reko: a decoder for RiscV instruction 0000B995 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B995()
        {
            AssertCode("@@@", 0x0000B995);
        }

        // Reko: a decoder for RiscV instruction 0000B655 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B655()
        {
            AssertCode("@@@", 0x0000B655);
        }

        // Reko: a decoder for RiscV instruction 0000B43D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B43D()
        {
            AssertCode("@@@", 0x0000B43D);
        }

        // Reko: a decoder for RiscV instruction 0000B125 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B125()
        {
            AssertCode("@@@", 0x0000B125);
        }

        // Reko: a decoder for RiscV instruction 00006788 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006788()
        {
            AssertCode("@@@", 0x00006788);
        }

        // Reko: a decoder for RiscV instruction 0000BE25 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BE25()
        {
            AssertCode("@@@", 0x0000BE25);
        }

        // Reko: a decoder for RiscV instruction 0000B60D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B60D()
        {
            AssertCode("@@@", 0x0000B60D);
        }

        // Reko: a decoder for RiscV instruction 0000BEC1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BEC1()
        {
            AssertCode("@@@", 0x0000BEC1);
        }

        // Reko: a decoder for RiscV instruction 0000472D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000472D()
        {
            AssertCode("@@@", 0x0000472D);
        }

        // Reko: a decoder for RiscV instruction 0000B249 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B249()
        {
            AssertCode("@@@", 0x0000B249);
        }

        [Test]
        public void RiscV_dasm_beqz_0000C3F1()
        {
            AssertCode("c.beqz\ta5,0000000001000C4", 0x0000C3F1);
        }

        // Reko: a decoder for RiscV instruction 0000BAA1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BAA1()
        {
            AssertCode("@@@", 0x0000BAA1);
        }

        // Reko: a decoder for RiscV instruction 0000E114 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E114()
        {
            AssertCode("@@@", 0x0000E114);
        }

        // Reko: a decoder for RiscV instruction 0000B63D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B63D()
        {
            AssertCode("@@@", 0x0000B63D);
        }

        // Reko: a decoder for RiscV instruction 0000BC3D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BC3D()
        {
            AssertCode("@@@", 0x0000BC3D);
        }

        // Reko: a decoder for RiscV instruction 000086BE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000086BE()
        {
            AssertCode("@@@", 0x000086BE);
        }

        // Reko: a decoder for RiscV instruction 0000B971 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B971()
        {
            AssertCode("@@@", 0x0000B971);
        }

        // Reko: a decoder for RiscV instruction 0000B7F5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7F5()
        {
            AssertCode("@@@", 0x0000B7F5);
        }

        // Reko: a decoder for RiscV instruction 0000B439 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B439()
        {
            AssertCode("@@@", 0x0000B439);
        }

        // Reko: a decoder for RiscV instruction 0000FD0D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FD0D()
        {
            AssertCode("@@@", 0x0000FD0D);
        }

        // Reko: a decoder for RiscV instruction 0000BACD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BACD()
        {
            AssertCode("@@@", 0x0000BACD);
        }

        // Reko: a decoder for RiscV instruction 0000B2C5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B2C5()
        {
            AssertCode("@@@", 0x0000B2C5);
        }

        // Reko: a decoder for RiscV instruction 0000B2C9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B2C9()
        {
            AssertCode("@@@", 0x0000B2C9);
        }

        // Reko: a decoder for RiscV instruction 0000B7BD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7BD()
        {
            AssertCode("@@@", 0x0000B7BD);
        }

        // Reko: a decoder for RiscV instruction 0000F969 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F969()
        {
            AssertCode("@@@", 0x0000F969);
        }

        // Reko: a decoder for RiscV instruction 0000B7B1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7B1()
        {
            AssertCode("@@@", 0x0000B7B1);
        }

        // Reko: a decoder for RiscV instruction 0000B28D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B28D()
        {
            AssertCode("@@@", 0x0000B28D);
        }

        // Reko: a decoder for RiscV instruction 0000BA05 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BA05()
        {
            AssertCode("@@@", 0x0000BA05);
        }

        // Reko: a decoder for RiscV instruction 0000B5ED at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B5ED()
        {
            AssertCode("@@@", 0x0000B5ED);
        }

        // Reko: a decoder for RiscV instruction 0000B87D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B87D()
        {
            AssertCode("@@@", 0x0000B87D);
        }

        // Reko: a decoder for RiscV instruction 0000BD3D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD3D()
        {
            AssertCode("@@@", 0x0000BD3D);
        }

        // Reko: a decoder for RiscV instruction 0000BD05 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD05()
        {
            AssertCode("@@@", 0x0000BD05);
        }

        // Reko: a decoder for RiscV instruction 00004731 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004731()
        {
            AssertCode("@@@", 0x00004731);
        }

        // Reko: a decoder for RiscV instruction 0000B9E5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B9E5()
        {
            AssertCode("@@@", 0x0000B9E5);
        }

        // Reko: a decoder for RiscV instruction 0000BB79 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB79()
        {
            AssertCode("@@@", 0x0000BB79);
        }

        // Reko: a decoder for RiscV instruction 0000B321 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B321()
        {
            AssertCode("@@@", 0x0000B321);
        }

        // Reko: a decoder for RiscV instruction 0000B9ED at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B9ED()
        {
            AssertCode("@@@", 0x0000B9ED);
        }

        // Reko: a decoder for RiscV instruction 0000B99D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B99D()
        {
            AssertCode("@@@", 0x0000B99D);
        }

        // Reko: a decoder for RiscV instruction 0000B9A9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B9A9()
        {
            AssertCode("@@@", 0x0000B9A9);
        }

        // Reko: a decoder for RiscV instruction 00004735 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004735()
        {
            AssertCode("@@@", 0x00004735);
        }

        // Reko: a decoder for RiscV instruction 00008B91 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008B91()
        {
            AssertCode("@@@", 0x00008B91);
        }

        // Reko: a decoder for RiscV instruction 0000BAF5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BAF5()
        {
            AssertCode("@@@", 0x0000BAF5);
        }

        // Reko: a decoder for RiscV instruction 0000BB75 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB75()
        {
            AssertCode("@@@", 0x0000BB75);
        }

        // Reko: a decoder for RiscV instruction 0000BC69 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BC69()
        {
            AssertCode("@@@", 0x0000BC69);
        }

        // Reko: a decoder for RiscV instruction 0000B3BD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B3BD()
        {
            AssertCode("@@@", 0x0000B3BD);
        }

        // Reko: a decoder for RiscV instruction 0000BD59 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD59()
        {
            AssertCode("@@@", 0x0000BD59);
        }

        // Reko: a decoder for RiscV instruction 0000B97D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B97D()
        {
            AssertCode("@@@", 0x0000B97D);
        }

        // Reko: a decoder for RiscV instruction 0000BC4D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BC4D()
        {
            AssertCode("@@@", 0x0000BC4D);
        }

        // Reko: a decoder for RiscV instruction 0000B499 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B499()
        {
            AssertCode("@@@", 0x0000B499);
        }

        // Reko: a decoder for RiscV instruction 0000BE49 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BE49()
        {
            AssertCode("@@@", 0x0000BE49);
        }

        // Reko: a decoder for RiscV instruction 0000B65D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B65D()
        {
            AssertCode("@@@", 0x0000B65D);
        }

        // Reko: a decoder for RiscV instruction 0000B6F1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B6F1()
        {
            AssertCode("@@@", 0x0000B6F1);
        }

        // Reko: a decoder for RiscV instruction 0000BEF5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BEF5()
        {
            AssertCode("@@@", 0x0000BEF5);
        }

        // Reko: a decoder for RiscV instruction 0000B11D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B11D()
        {
            AssertCode("@@@", 0x0000B11D);
        }

        // Reko: a decoder for RiscV instruction 0000B4D9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B4D9()
        {
            AssertCode("@@@", 0x0000B4D9);
        }

        // Reko: a decoder for RiscV instruction 0000B441 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B441()
        {
            AssertCode("@@@", 0x0000B441);
        }

        // Reko: a decoder for RiscV instruction 0000EF95 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EF95()
        {
            AssertCode("@@@", 0x0000EF95);
        }

        // Reko: a decoder for RiscV instruction 00009381 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00009381()
        {
            AssertCode("@@@", 0x00009381);
        }

        // Reko: a decoder for RiscV instruction 0000F775 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F775()
        {
            AssertCode("@@@", 0x0000F775);
        }

        // Reko: a decoder for RiscV instruction 0000EBDD at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EBDD()
        {
            AssertCode("@@@", 0x0000EBDD);
        }

        // Reko: a decoder for RiscV instruction 0000CD4D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CD4D()
        {
            AssertCode("@@@", 0x0000CD4D);
        }

        // Reko: a decoder for RiscV instruction 0000C32D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C32D()
        {
            AssertCode("@@@", 0x0000C32D);
        }

        // Reko: a decoder for RiscV instruction 0000ED35 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000ED35()
        {
            AssertCode("@@@", 0x0000ED35);
        }

        // Reko: a decoder for RiscV instruction 0000E935 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E935()
        {
            AssertCode("@@@", 0x0000E935);
        }

        // Reko: a decoder for RiscV instruction 0000E535 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E535()
        {
            AssertCode("@@@", 0x0000E535);
        }

        // Reko: a decoder for RiscV instruction 0000E135 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E135()
        {
            AssertCode("@@@", 0x0000E135);
        }

        // Reko: a decoder for RiscV instruction 0000C39D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C39D()
        {
            AssertCode("@@@", 0x0000C39D);
        }

        // Reko: a decoder for RiscV instruction 00004310 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004310()
        {
            AssertCode("@@@", 0x00004310);
        }

        // Reko: a decoder for RiscV instruction 0000C01C at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C01C()
        {
            AssertCode("@@@", 0x0000C01C);
        }

        // Reko: a decoder for RiscV instruction 0000BF69 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF69()
        {
            AssertCode("@@@", 0x0000BF69);
        }

        // Reko: a decoder for RiscV instruction 0000F129 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F129()
        {
            AssertCode("@@@", 0x0000F129);
        }

        // Reko: a decoder for RiscV instruction 0000F90D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F90D()
        {
            AssertCode("@@@", 0x0000F90D);
        }

        // Reko: a decoder for RiscV instruction 0000B7E5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7E5()
        {
            AssertCode("@@@", 0x0000B7E5);
        }

        // Reko: a decoder for RiscV instruction 0000429C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000429C()
        {
            AssertCode("@@@", 0x0000429C);
        }

        // Reko: a decoder for RiscV instruction 0000C77D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C77D()
        {
            AssertCode("@@@", 0x0000C77D);
        }

        // Reko: a decoder for RiscV instruction 0000CB1D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CB1D()
        {
            AssertCode("@@@", 0x0000CB1D);
        }

        // Reko: a decoder for RiscV instruction 0000474D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000474D()
        {
            AssertCode("@@@", 0x0000474D);
        }

        // Reko: a decoder for RiscV instruction 0000FB71 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FB71()
        {
            AssertCode("@@@", 0x0000FB71);
        }

        // Reko: a decoder for RiscV instruction 0000F779 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F779()
        {
            AssertCode("@@@", 0x0000F779);
        }

        // Reko: a decoder for RiscV instruction 0000678D at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_0000678D()
        {
            AssertCode("@@@", 0x0000678D);
        }

        // Reko: a decoder for RiscV instruction 000067AD at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_000067AD()
        {
            AssertCode("@@@", 0x000067AD);
        }

        // Reko: a decoder for RiscV instruction 0000B7F1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7F1()
        {
            AssertCode("@@@", 0x0000B7F1);
        }

        // Reko: a decoder for RiscV instruction 0000BF7D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF7D()
        {
            AssertCode("@@@", 0x0000BF7D);
        }

        // Reko: a decoder for RiscV instruction 0000B77D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B77D()
        {
            AssertCode("@@@", 0x0000B77D);
        }

        // Reko: a decoder for RiscV instruction 0000BF71 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF71()
        {
            AssertCode("@@@", 0x0000BF71);
        }

        // Reko: a decoder for RiscV instruction 0000B771 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B771()
        {
            AssertCode("@@@", 0x0000B771);
        }

        // Reko: a decoder for RiscV instruction 0000C3C5 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C3C5()
        {
            AssertCode("@@@", 0x0000C3C5);
        }

        // Reko: a decoder for RiscV instruction 0000CBC1 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CBC1()
        {
            AssertCode("@@@", 0x0000CBC1);
        }

        // Reko: a decoder for RiscV instruction 00004905 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004905()
        {
            AssertCode("@@@", 0x00004905);
        }

        // Reko: a decoder for RiscV instruction 0000DC21 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000DC21()
        {
            AssertCode("@@@", 0x0000DC21);
        }

        // Reko: a decoder for RiscV instruction 0000485C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000485C()
        {
            AssertCode("@@@", 0x0000485C);
        }

        // Reko: a decoder for RiscV instruction 0000D42D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D42D()
        {
            AssertCode("@@@", 0x0000D42D);
        }

        // Reko: a decoder for RiscV instruction 0000B7C9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7C9()
        {
            AssertCode("@@@", 0x0000B7C9);
        }

        // Reko: a decoder for RiscV instruction 0000BF25 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF25()
        {
            AssertCode("@@@", 0x0000BF25);
        }

        // Reko: a decoder for RiscV instruction 0000FB05 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FB05()
        {
            AssertCode("@@@", 0x0000FB05);
        }

        // Reko: a decoder for RiscV instruction 0000BFCD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFCD()
        {
            AssertCode("@@@", 0x0000BFCD);
        }

        // Reko: a decoder for RiscV instruction 00006418 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006418()
        {
            AssertCode("@@@", 0x00006418);
        }

        // Reko: a decoder for RiscV instruction 0000B735 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B735()
        {
            AssertCode("@@@", 0x0000B735);
        }

        // Reko: a decoder for RiscV instruction 0000B709 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B709()
        {
            AssertCode("@@@", 0x0000B709);
        }

        // Reko: a decoder for RiscV instruction 0000BDE1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BDE1()
        {
            AssertCode("@@@", 0x0000BDE1);
        }

        // Reko: a decoder for RiscV instruction 0000B551 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B551()
        {
            AssertCode("@@@", 0x0000B551);
        }

        // Reko: a decoder for RiscV instruction 0000BDA9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BDA9()
        {
            AssertCode("@@@", 0x0000BDA9);
        }

        // Reko: a decoder for RiscV instruction 000095DE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000095DE()
        {
            AssertCode("@@@", 0x000095DE);
        }

        // Reko: a decoder for RiscV instruction 0000BBE1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BBE1()
        {
            AssertCode("@@@", 0x0000BBE1);
        }

        // Reko: a decoder for RiscV instruction 0000B37D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B37D()
        {
            AssertCode("@@@", 0x0000B37D);
        }

        // Reko: a decoder for RiscV instruction 0000B351 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B351()
        {
            AssertCode("@@@", 0x0000B351);
        }

        // Reko: a decoder for RiscV instruction 0000BB05 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB05()
        {
            AssertCode("@@@", 0x0000BB05);
        }

        // Reko: a decoder for RiscV instruction 0000B319 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B319()
        {
            AssertCode("@@@", 0x0000B319);
        }

        // Reko: a decoder for RiscV instruction 000040D0 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_000040D0()
        {
            AssertCode("@@@", 0x000040D0);
        }

        // Reko: a decoder for RiscV instruction 0000B1C9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B1C9()
        {
            AssertCode("@@@", 0x0000B1C9);
        }

        // Reko: a decoder for RiscV instruction 0000C505 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C505()
        {
            AssertCode("@@@", 0x0000C505);
        }

        // Reko: a decoder for RiscV instruction 00006C18 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006C18()
        {
            AssertCode("@@@", 0x00006C18);
        }

        // Reko: a decoder for RiscV instruction 0000505C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000505C()
        {
            AssertCode("@@@", 0x0000505C);
        }

        // Reko: a decoder for RiscV instruction 0000C904 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000C904()
        {
            AssertCode("@@@", 0x0000C904);
        }

        // Reko: a decoder for RiscV instruction 0000E518 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E518()
        {
            AssertCode("@@@", 0x0000E518);
        }

        // Reko: a decoder for RiscV instruction 0000CD1C at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000CD1C()
        {
            AssertCode("@@@", 0x0000CD1C);
        }

        // Reko: a decoder for RiscV instruction 0000B7FD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7FD()
        {
            AssertCode("@@@", 0x0000B7FD);
        }

        // Reko: a decoder for RiscV instruction 0000714D at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_0000714D()
        {
            AssertCode("@@@", 0x0000714D);
        }

        // Reko: a decoder for RiscV instruction 0000FE26 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FE26()
        {
            AssertCode("@@@", 0x0000FE26);
        }

        // Reko: a decoder for RiscV instruction 00006B94 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006B94()
        {
            AssertCode("@@@", 0x00006B94);
        }

        // Reko: a decoder for RiscV instruction 0000D03A at address 00100000 has not been implemented. (swsp)
        [Test]
        public void RiscV_dasm_0000D03A()
        {
            AssertCode("@@@", 0x0000D03A);
        }

        // Reko: a decoder for RiscV instruction 0000E686 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E686()
        {
            AssertCode("@@@", 0x0000E686);
        }

        // Reko: a decoder for RiscV instruction 0000E2A2 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E2A2()
        {
            AssertCode("@@@", 0x0000E2A2);
        }

        // Reko: a decoder for RiscV instruction 0000FA4A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000FA4A()
        {
            AssertCode("@@@", 0x0000FA4A);
        }

        // Reko: a decoder for RiscV instruction 0000F646 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F646()
        {
            AssertCode("@@@", 0x0000F646);
        }

        // Reko: a decoder for RiscV instruction 0000E832 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E832()
        {
            AssertCode("@@@", 0x0000E832);
        }

        // Reko: a decoder for RiscV instruction 0000E385 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E385()
        {
            AssertCode("@@@", 0x0000E385);
        }

        // Reko: a decoder for RiscV instruction 0000F6F5 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F6F5()
        {
            AssertCode("@@@", 0x0000F6F5);
        }

        // Reko: a decoder for RiscV instruction 00007732 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007732()
        {
            AssertCode("@@@", 0x00007732);
        }

        // Reko: a decoder for RiscV instruction 00006416 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006416()
        {
            AssertCode("@@@", 0x00006416);
        }

        // Reko: a decoder for RiscV instruction 000060B6 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000060B6()
        {
            AssertCode("@@@", 0x000060B6);
        }

        // Reko: a decoder for RiscV instruction 000074F2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000074F2()
        {
            AssertCode("@@@", 0x000074F2);
        }

        // Reko: a decoder for RiscV instruction 00007952 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007952()
        {
            AssertCode("@@@", 0x00007952);
        }

        // Reko: a decoder for RiscV instruction 00006171 at address 00100000 has not been implemented. (lui)
        [Test]
        public void RiscV_dasm_00006171()
        {
            AssertCode("@@@", 0x00006171);
        }

        // Reko: a decoder for RiscV instruction 0000FBDD at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FBDD()
        {
            AssertCode("@@@", 0x0000FBDD);
        }

        // Reko: a decoder for RiscV instruction 0000CD51 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CD51()
        {
            AssertCode("@@@", 0x0000CD51);
        }

        // Reko: a decoder for RiscV instruction 00006488 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006488()
        {
            AssertCode("@@@", 0x00006488);
        }

        // Reko: a decoder for RiscV instruction 0000CC08 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000CC08()
        {
            AssertCode("@@@", 0x0000CC08);
        }

        // Reko: a decoder for RiscV instruction 00006894 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006894()
        {
            AssertCode("@@@", 0x00006894);
        }

        // Reko: a decoder for RiscV instruction 000050D8 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_000050D8()
        {
            AssertCode("@@@", 0x000050D8);
        }

        // Reko: a decoder for RiscV instruction 0000549C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000549C()
        {
            AssertCode("@@@", 0x0000549C);
        }

        // Reko: a decoder for RiscV instruction 0000E814 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E814()
        {
            AssertCode("@@@", 0x0000E814);
        }

        // Reko: a decoder for RiscV instruction 0000CC58 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000CC58()
        {
            AssertCode("@@@", 0x0000CC58);
        }

        // Reko: a decoder for RiscV instruction 0000D01C at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000D01C()
        {
            AssertCode("@@@", 0x0000D01C);
        }

        // Reko: a decoder for RiscV instruction 0000C12D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C12D()
        {
            AssertCode("@@@", 0x0000C12D);
        }

        // Reko: a decoder for RiscV instruction 00006504 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006504()
        {
            AssertCode("@@@", 0x00006504);
        }

        // Reko: a decoder for RiscV instruction 0000C7A1 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C7A1()
        {
            AssertCode("@@@", 0x0000C7A1);
        }

        // Reko: a decoder for RiscV instruction 000097D2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000097D2()
        {
            AssertCode("@@@", 0x000097D2);
        }

        // Reko: a decoder for RiscV instruction 0000DBBD at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000DBBD()
        {
            AssertCode("@@@", 0x0000DBBD);
        }

        // Reko: a decoder for RiscV instruction 0000BF15 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF15()
        {
            AssertCode("@@@", 0x0000BF15);
        }

        // Reko: a decoder for RiscV instruction 0000845A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000845A()
        {
            AssertCode("@@@", 0x0000845A);
        }

        // Reko: a decoder for RiscV instruction 0000B78D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B78D()
        {
            AssertCode("@@@", 0x0000B78D);
        }

        // Reko: a decoder for RiscV instruction 0000BFB9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFB9()
        {
            AssertCode("@@@", 0x0000BFB9);
        }

        // Reko: a decoder for RiscV instruction 00006080 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006080()
        {
            AssertCode("@@@", 0x00006080);
        }

        // Reko: a decoder for RiscV instruction 0000E909 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E909()
        {
            AssertCode("@@@", 0x0000E909);
        }

        // Reko: a decoder for RiscV instruction 0000A015 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A015()
        {
            AssertCode("@@@", 0x0000A015);
        }

        // Reko: a decoder for RiscV instruction 0000C90D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C90D()
        {
            AssertCode("@@@", 0x0000C90D);
        }

        // Reko: a decoder for RiscV instruction 0000843E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000843E()
        {
            AssertCode("@@@", 0x0000843E);
        }

        // Reko: a decoder for RiscV instruction 0000A835 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A835()
        {
            AssertCode("@@@", 0x0000A835);
        }

        // Reko: a decoder for RiscV instruction 0000CD09 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CD09()
        {
            AssertCode("@@@", 0x0000CD09);
        }

        // Reko: a decoder for RiscV instruction 0000E09C at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E09C()
        {
            AssertCode("@@@", 0x0000E09C);
        }

        // Reko: a decoder for RiscV instruction 0000CD21 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CD21()
        {
            AssertCode("@@@", 0x0000CD21);
        }

        // Reko: a decoder for RiscV instruction 0000873E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000873E()
        {
            AssertCode("@@@", 0x0000873E);
        }

        // Reko: a decoder for RiscV instruction 0000631C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000631C()
        {
            AssertCode("@@@", 0x0000631C);
        }

        // Reko: a decoder for RiscV instruction 0000FFF5 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FFF5()
        {
            AssertCode("@@@", 0x0000FFF5);
        }

        // Reko: a decoder for RiscV instruction 0000E304 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E304()
        {
            AssertCode("@@@", 0x0000E304);
        }

        // Reko: a decoder for RiscV instruction 0000CD19 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CD19()
        {
            AssertCode("@@@", 0x0000CD19);
        }

        // Reko: a decoder for RiscV instruction 0000E31C at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E31C()
        {
            AssertCode("@@@", 0x0000E31C);
        }

        // Reko: a decoder for RiscV instruction 0000E004 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E004()
        {
            AssertCode("@@@", 0x0000E004);
        }

        // Reko: a decoder for RiscV instruction 0000B7D1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B7D1()
        {
            AssertCode("@@@", 0x0000B7D1);
        }

        // Reko: a decoder for RiscV instruction 00006D08 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006D08()
        {
            AssertCode("@@@", 0x00006D08);
        }

        // Reko: a decoder for RiscV instruction 0000A819 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A819()
        {
            AssertCode("@@@", 0x0000A819);
        }

        // Reko: a decoder for RiscV instruction 0000CF19 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CF19()
        {
            AssertCode("@@@", 0x0000CF19);
        }

        // Reko: a decoder for RiscV instruction 0000D66D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D66D()
        {
            AssertCode("@@@", 0x0000D66D);
        }

        [Test]
        public void RiscV_dasm_c_addiw()
        {
            AssertCode("c.addiw\ts0,00000001", 0x00002405);
        }

        // Reko: a decoder for RiscV instruction 0000F3FD at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F3FD()
        {
            AssertCode("@@@", 0x0000F3FD);
        }

        // Reko: a decoder for RiscV instruction 000089D6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000089D6()
        {
            AssertCode("@@@", 0x000089D6);
        }

        // Reko: a decoder for RiscV instruction 00005BFD at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00005BFD()
        {
            AssertCode("@@@", 0x00005BFD);
        }

        // Reko: a decoder for RiscV instruction 0000C119 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C119()
        {
            AssertCode("@@@", 0x0000C119);
        }

        // Reko: a decoder for RiscV instruction 00008DAA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008DAA()
        {
            AssertCode("@@@", 0x00008DAA);
        }

        // Reko: a decoder for RiscV instruction 000085E2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085E2()
        {
            AssertCode("@@@", 0x000085E2);
        }

        // Reko: a decoder for RiscV instruction 0000856A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000856A()
        {
            AssertCode("@@@", 0x0000856A);
        }

        // Reko: a decoder for RiscV instruction 0000C925 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C925()
        {
            AssertCode("@@@", 0x0000C925);
        }

        // Reko: a decoder for RiscV instruction 0000CC88 at address 00100000 has not been implemented. (sw)
        [Test]
        public void RiscV_dasm_0000CC88()
        {
            AssertCode("@@@", 0x0000CC88);
        }

        // Reko: a decoder for RiscV instruction 0000F15D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F15D()
        {
            AssertCode("@@@", 0x0000F15D);
        }

        // Reko: a decoder for RiscV instruction 0000A005 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A005()
        {
            AssertCode("@@@", 0x0000A005);
        }

        // Reko: a decoder for RiscV instruction 0000D541 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000D541()
        {
            AssertCode("@@@", 0x0000D541);
        }

        // Reko: a decoder for RiscV instruction 000087A6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087A6()
        {
            AssertCode("@@@", 0x000087A6);
        }

        // Reko: a decoder for RiscV instruction 0000FB79 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FB79()
        {
            AssertCode("@@@", 0x0000FB79);
        }

        // Reko: a decoder for RiscV instruction 00006508 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006508()
        {
            AssertCode("@@@", 0x00006508);
        }

        // Reko: a decoder for RiscV instruction 0000E01C at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E01C()
        {
            AssertCode("@@@", 0x0000E01C);
        }

        // Reko: a decoder for RiscV instruction 000085EA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085EA()
        {
            AssertCode("@@@", 0x000085EA);
        }

        // Reko: a decoder for RiscV instruction 00008DD2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008DD2()
        {
            AssertCode("@@@", 0x00008DD2);
        }

        // Reko: a decoder for RiscV instruction 000094EE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000094EE()
        {
            AssertCode("@@@", 0x000094EE);
        }

        // Reko: a decoder for RiscV instruction 0000BD91 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD91()
        {
            AssertCode("@@@", 0x0000BD91);
        }

        // Reko: a decoder for RiscV instruction 00004A05 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004A05()
        {
            AssertCode("@@@", 0x00004A05);
        }

        // Reko: a decoder for RiscV instruction 0000BD41 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD41()
        {
            AssertCode("@@@", 0x0000BD41);
        }

        // Reko: a decoder for RiscV instruction 0000B595 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B595()
        {
            AssertCode("@@@", 0x0000B595);
        }

        // Reko: a decoder for RiscV instruction 0000B545 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B545()
        {
            AssertCode("@@@", 0x0000B545);
        }

        // Reko: a decoder for RiscV instruction 000089BE at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000089BE()
        {
            AssertCode("@@@", 0x000089BE);
        }

        // Reko: a decoder for RiscV instruction 0000E05A at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E05A()
        {
            AssertCode("@@@", 0x0000E05A);
        }

        // Reko: a decoder for RiscV instruction 0000E105 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E105()
        {
            AssertCode("@@@", 0x0000E105);
        }

        // Reko: a decoder for RiscV instruction 00005018 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00005018()
        {
            AssertCode("@@@", 0x00005018);
        }

        // Reko: a decoder for RiscV instruction 00006B02 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006B02()
        {
            AssertCode("@@@", 0x00006B02);
        }

        // Reko: a decoder for RiscV instruction 00004FBC at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004FBC()
        {
            AssertCode("@@@", 0x00004FBC);
        }

        // Reko: a decoder for RiscV instruction 00004BBC at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004BBC()
        {
            AssertCode("@@@", 0x00004BBC);
        }

        // Reko: a decoder for RiscV instruction 0000F545 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F545()
        {
            AssertCode("@@@", 0x0000F545);
        }

        // Reko: a decoder for RiscV instruction 0000F159 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F159()
        {
            AssertCode("@@@", 0x0000F159);
        }

        // Reko: a decoder for RiscV instruction 0000E789 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E789()
        {
            AssertCode("@@@", 0x0000E789);
        }

        // Reko: a decoder for RiscV instruction 0000EFC5 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EFC5()
        {
            AssertCode("@@@", 0x0000EFC5);
        }

        // Reko: a decoder for RiscV instruction 0000F121 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F121()
        {
            AssertCode("@@@", 0x0000F121);
        }

        // Reko: a decoder for RiscV instruction 0000B701 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B701()
        {
            AssertCode("@@@", 0x0000B701);
        }

        // Reko: a decoder for RiscV instruction 00006C08 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006C08()
        {
            AssertCode("@@@", 0x00006C08);
        }

        // Reko: a decoder for RiscV instruction 0000874A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_0000874A()
        {
            AssertCode("@@@", 0x0000874A);
        }

        // Reko: a decoder for RiscV instruction 00004805 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004805()
        {
            AssertCode("@@@", 0x00004805);
        }

        // Reko: a decoder for RiscV instruction 00008FE9 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008FE9()
        {
            AssertCode("@@@", 0x00008FE9);
        }

        // Reko: a decoder for RiscV instruction 0000CF9D at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CF9D()
        {
            AssertCode("@@@", 0x0000CF9D);
        }

        // Reko: a decoder for RiscV instruction 00008736 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008736()
        {
            AssertCode("@@@", 0x00008736);
        }

        // Reko: a decoder for RiscV instruction 0000DA65 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000DA65()
        {
            AssertCode("@@@", 0x0000DA65);
        }

        // Reko: a decoder for RiscV instruction 0000C8A9 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C8A9()
        {
            AssertCode("@@@", 0x0000C8A9);
        }

        // Reko: a decoder for RiscV instruction 0000C219 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C219()
        {
            AssertCode("@@@", 0x0000C219);
        }

        // Reko: a decoder for RiscV instruction 00008A4A at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008A4A()
        {
            AssertCode("@@@", 0x00008A4A);
        }

        // Reko: a decoder for RiscV instruction 00005B7D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00005B7D()
        {
            AssertCode("@@@", 0x00005B7D);
        }

        // Reko: a decoder for RiscV instruction 0000C529 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C529()
        {
            AssertCode("@@@", 0x0000C529);
        }

        // Reko: a decoder for RiscV instruction 0000E808 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E808()
        {
            AssertCode("@@@", 0x0000E808);
        }

        // Reko: a decoder for RiscV instruction 0000B5AD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B5AD()
        {
            AssertCode("@@@", 0x0000B5AD);
        }

        // Reko: a decoder for RiscV instruction 0000BD81 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD81()
        {
            AssertCode("@@@", 0x0000BD81);
        }

        // Reko: a decoder for RiscV instruction 0000B5C1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B5C1()
        {
            AssertCode("@@@", 0x0000B5C1);
        }

        // Reko: a decoder for RiscV instruction 0000BD09 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD09()
        {
            AssertCode("@@@", 0x0000BD09);
        }

        // Reko: a decoder for RiscV instruction 0000C905 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C905()
        {
            AssertCode("@@@", 0x0000C905);
        }

        // Reko: a decoder for RiscV instruction 00004A85 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004A85()
        {
            AssertCode("@@@", 0x00004A85);
        }

        // Reko: a decoder for RiscV instruction 000073BC at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_000073BC()
        {
            AssertCode("@@@", 0x000073BC);
        }

        // Reko: a decoder for RiscV instruction 0000EC08 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000EC08()
        {
            AssertCode("@@@", 0x0000EC08);
        }

        // Reko: a decoder for RiscV instruction 0000CBB1 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CBB1()
        {
            AssertCode("@@@", 0x0000CBB1);
        }

        // Reko: a decoder for RiscV instruction 0000C131 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C131()
        {
            AssertCode("@@@", 0x0000C131);
        }

        // Reko: a decoder for RiscV instruction 00009A3E at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00009A3E()
        {
            AssertCode("@@@", 0x00009A3E);
        }

        // Reko: a decoder for RiscV instruction 0000551C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000551C()
        {
            AssertCode("@@@", 0x0000551C);
        }

        // Reko: a decoder for RiscV instruction 0000F3C1 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F3C1()
        {
            AssertCode("@@@", 0x0000F3C1);
        }

        // Reko: a decoder for RiscV instruction 0000F165 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F165()
        {
            AssertCode("@@@", 0x0000F165);
        }

        // Reko: a decoder for RiscV instruction 0000BFC5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BFC5()
        {
            AssertCode("@@@", 0x0000BFC5);
        }

        // Reko: a decoder for RiscV instruction 0000F43E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F43E()
        {
            AssertCode("@@@", 0x0000F43E);
        }

        // Reko: a decoder for RiscV instruction 00007722 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00007722()
        {
            AssertCode("@@@", 0x00007722);
        }

        // Reko: a decoder for RiscV instruction 0000C621 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C621()
        {
            AssertCode("@@@", 0x0000C621);
        }

        // Reko: a decoder for RiscV instruction 000085B2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085B2()
        {
            AssertCode("@@@", 0x000085B2);
        }

        // Reko: a decoder for RiscV instruction 0000659C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000659C()
        {
            AssertCode("@@@", 0x0000659C);
        }

        // Reko: a decoder for RiscV instruction 0000A039 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A039()
        {
            AssertCode("@@@", 0x0000A039);
        }

        // Reko: a decoder for RiscV instruction 000087BA at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000087BA()
        {
            AssertCode("@@@", 0x000087BA);
        }

        // Reko: a decoder for RiscV instruction 0000E394 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E394()
        {
            AssertCode("@@@", 0x0000E394);
        }

        // Reko: a decoder for RiscV instruction 0000FF65 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FF65()
        {
            AssertCode("@@@", 0x0000FF65);
        }

        // Reko: a decoder for RiscV instruction 0000618C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_0000618C()
        {
            AssertCode("@@@", 0x0000618C);
        }

        // Reko: a decoder for RiscV instruction 0000F5E5 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F5E5()
        {
            AssertCode("@@@", 0x0000F5E5);
        }

        // Reko: a decoder for RiscV instruction 00006A1C at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006A1C()
        {
            AssertCode("@@@", 0x00006A1C);
        }

        // Reko: a decoder for RiscV instruction 00006210 at address 00100000 has not been implemented. (flw / ld)
        [Test]
        public void RiscV_dasm_00006210()
        {
            AssertCode("@@@", 0x00006210);
        }

        // Reko: a decoder for RiscV instruction 0000F665 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000F665()
        {
            AssertCode("@@@", 0x0000F665);
        }

        // Reko: a decoder for RiscV instruction 0000E3A1 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E3A1()
        {
            AssertCode("@@@", 0x0000E3A1);
        }

        // Reko: a decoder for RiscV instruction 0000E7C9 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E7C9()
        {
            AssertCode("@@@", 0x0000E7C9);
        }

        // Reko: a decoder for RiscV instruction 0000E3C9 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E3C9()
        {
            AssertCode("@@@", 0x0000E3C9);
        }

        // Reko: a decoder for RiscV instruction 0000BDF5 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BDF5()
        {
            AssertCode("@@@", 0x0000BDF5);
        }

        // Reko: a decoder for RiscV instruction 0000E359 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E359()
        {
            AssertCode("@@@", 0x0000E359);
        }

        // Reko: a decoder for RiscV instruction 0000BD55 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BD55()
        {
            AssertCode("@@@", 0x0000BD55);
        }

        // Reko: a decoder for RiscV instruction 0000B5F9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B5F9()
        {
            AssertCode("@@@", 0x0000B5F9);
        }

        // Reko: a decoder for RiscV instruction 0000DB61 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000DB61()
        {
            AssertCode("@@@", 0x0000DB61);
        }

        // Reko: a decoder for RiscV instruction 0000B579 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B579()
        {
            AssertCode("@@@", 0x0000B579);
        }

        // Reko: a decoder for RiscV instruction 0000B58D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B58D()
        {
            AssertCode("@@@", 0x0000B58D);
        }

        // Reko: a decoder for RiscV instruction 0000C909 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C909()
        {
            AssertCode("@@@", 0x0000C909);
        }

        // Reko: a decoder for RiscV instruction 0000E118 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E118()
        {
            AssertCode("@@@", 0x0000E118);
        }

        // Reko: a decoder for RiscV instruction 0000FFE5 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FFE5()
        {
            AssertCode("@@@", 0x0000FFE5);
        }

        // Reko: a decoder for RiscV instruction 0000E008 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E008()
        {
            AssertCode("@@@", 0x0000E008);
        }

        // Reko: a decoder for RiscV instruction 00008B85 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008B85()
        {
            AssertCode("@@@", 0x00008B85);
        }

        // Reko: a decoder for RiscV instruction 0000C711 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C711()
        {
            AssertCode("@@@", 0x0000C711);
        }

        // Reko: a decoder for RiscV instruction 00008B11 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008B11()
        {
            AssertCode("@@@", 0x00008B11);
        }

        // Reko: a decoder for RiscV instruction 0000BBCD at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BBCD()
        {
            AssertCode("@@@", 0x0000BBCD);
        }

        // Reko: a decoder for RiscV instruction 00004741 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004741()
        {
            AssertCode("@@@", 0x00004741);
        }

        // Reko: a decoder for RiscV instruction 0000EB99 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EB99()
        {
            AssertCode("@@@", 0x0000EB99);
        }

        // Reko: a decoder for RiscV instruction 0000EB89 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000EB89()
        {
            AssertCode("@@@", 0x0000EB89);
        }

        // Reko: a decoder for RiscV instruction 00008BA1 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008BA1()
        {
            AssertCode("@@@", 0x00008BA1);
        }

        // Reko: a decoder for RiscV instruction 0000E395 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E395()
        {
            AssertCode("@@@", 0x0000E395);
        }

        // Reko: a decoder for RiscV instruction 00008B41 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008B41()
        {
            AssertCode("@@@", 0x00008B41);
        }

        // Reko: a decoder for RiscV instruction 0000CF09 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CF09()
        {
            AssertCode("@@@", 0x0000CF09);
        }

        // Reko: a decoder for RiscV instruction 00008B95 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008B95()
        {
            AssertCode("@@@", 0x00008B95);
        }

        // Reko: a decoder for RiscV instruction 0000CFC1 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000CFC1()
        {
            AssertCode("@@@", 0x0000CFC1);
        }

        // Reko: a decoder for RiscV instruction 0000BAD1 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BAD1()
        {
            AssertCode("@@@", 0x0000BAD1);
        }

        // Reko: a decoder for RiscV instruction 0000B155 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B155()
        {
            AssertCode("@@@", 0x0000B155);
        }

        // Reko: a decoder for RiscV instruction 0000BF11 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF11()
        {
            AssertCode("@@@", 0x0000BF11);
        }

        // Reko: a decoder for RiscV instruction 0000B621 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B621()
        {
            AssertCode("@@@", 0x0000B621);
        }

        // Reko: a decoder for RiscV instruction 0000E10D at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E10D()
        {
            AssertCode("@@@", 0x0000E10D);
        }

        // Reko: a decoder for RiscV instruction 0000E501 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E501()
        {
            AssertCode("@@@", 0x0000E501);
        }

        // Reko: a decoder for RiscV instruction 0000B41D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B41D()
        {
            AssertCode("@@@", 0x0000B41D);
        }

        // Reko: a decoder for RiscV instruction 0000BC11 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BC11()
        {
            AssertCode("@@@", 0x0000BC11);
        }

        // Reko: a decoder for RiscV instruction 00008B05 at address 00100000 has not been implemented. (misc-alu)
        [Test]
        public void RiscV_dasm_00008B05()
        {
            AssertCode("@@@", 0x00008B05);
        }

        // Reko: a decoder for RiscV instruction 0000E911 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E911()
        {
            AssertCode("@@@", 0x0000E911);
        }

        // Reko: a decoder for RiscV instruction 0000BA9D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BA9D()
        {
            AssertCode("@@@", 0x0000BA9D);
        }

        // Reko: a decoder for RiscV instruction 0000E402 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E402()
        {
            AssertCode("@@@", 0x0000E402);
        }

        // Reko: a decoder for RiscV instruction 000086D6 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000086D6()
        {
            AssertCode("@@@", 0x000086D6);
        }

        // Reko: a decoder for RiscV instruction 0000BA6D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BA6D()
        {
            AssertCode("@@@", 0x0000BA6D);
        }

        // Reko: a decoder for RiscV instruction 0000BB39 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BB39()
        {
            AssertCode("@@@", 0x0000BB39);
        }

        // Reko: a decoder for RiscV instruction 000085D2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_000085D2()
        {
            AssertCode("@@@", 0x000085D2);
        }

        // Reko: a decoder for RiscV instruction 0000B1A9 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000B1A9()
        {
            AssertCode("@@@", 0x0000B1A9);
        }

        // Reko: a decoder for RiscV instruction 000067A2 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_000067A2()
        {
            AssertCode("@@@", 0x000067A2);
        }

        // Reko: a decoder for RiscV instruction 0000C611 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C611()
        {
            AssertCode("@@@", 0x0000C611);
        }

        // Reko: a decoder for RiscV instruction 0000FE65 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000FE65()
        {
            AssertCode("@@@", 0x0000FE65);
        }

        // Reko: a decoder for RiscV instruction 0000E014 at address 00100000 has not been implemented. (fsw / sd)
        [Test]
        public void RiscV_dasm_0000E014()
        {
            AssertCode("@@@", 0x0000E014);
        }

        // Reko: a decoder for RiscV instruction 0000E802 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E802()
        {
            AssertCode("@@@", 0x0000E802);
        }

        // Reko: a decoder for RiscV instruction 0000EC02 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC02()
        {
            AssertCode("@@@", 0x0000EC02);
        }

        // Reko: a decoder for RiscV instruction 0000F002 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000F002()
        {
            AssertCode("@@@", 0x0000F002);
        }

        // Reko: a decoder for RiscV instruction 0000E84E at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000E84E()
        {
            AssertCode("@@@", 0x0000E84E);
        }

        // Reko: a decoder for RiscV instruction 0000EC22 at address 00100000 has not been implemented. (sdsp)
        [Test]
        public void RiscV_dasm_0000EC22()
        {
            AssertCode("@@@", 0x0000EC22);
        }

        // Reko: a decoder for RiscV instruction 0000E901 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E901()
        {
            AssertCode("@@@", 0x0000E901);
        }

        // Reko: a decoder for RiscV instruction 0000A001 at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000A001()
        {
            AssertCode("@@@", 0x0000A001);
        }

        // Reko: a decoder for RiscV instruction 0000462D at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_0000462D()
        {
            AssertCode("@@@", 0x0000462D);
        }

        // Reko: a decoder for RiscV instruction 00006522 at address 00100000 has not been implemented. (ldsp)
        [Test]
        public void RiscV_dasm_00006522()
        {
            AssertCode("@@@", 0x00006522);
        }

        // Reko: a decoder for RiscV instruction 00004631 at address 00100000 has not been implemented. (li)
        [Test]
        public void RiscV_dasm_00004631()
        {
            AssertCode("@@@", 0x00004631);
        }

        // Reko: a decoder for RiscV instruction 0000E899 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E899()
        {
            AssertCode("@@@", 0x0000E899);
        }

        // Reko: a decoder for RiscV instruction 0000C501 at address 00100000 has not been implemented. (beqz)
        [Test]
        public void RiscV_dasm_0000C501()
        {
            AssertCode("@@@", 0x0000C501);
        }

        // Reko: a decoder for RiscV instruction 0000E121 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E121()
        {
            AssertCode("@@@", 0x0000E121);
        }

        // Reko: a decoder for RiscV instruction 00004114 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004114()
        {
            AssertCode("@@@", 0x00004114);
        }

        // Reko: a decoder for RiscV instruction 0000BF4D at address 00100000 has not been implemented. (j)
        [Test]
        public void RiscV_dasm_0000BF4D()
        {
            AssertCode("@@@", 0x0000BF4D);
        }

        // Reko: a decoder for RiscV instruction 00004118 at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_00004118()
        {
            AssertCode("@@@", 0x00004118);
        }

        // Reko: a decoder for RiscV instruction 0000E509 at address 00100000 has not been implemented. (bnez)
        [Test]
        public void RiscV_dasm_0000E509()
        {
            AssertCode("@@@", 0x0000E509);
        }

        // Reko: a decoder for RiscV instruction 0000400C at address 00100000 has not been implemented. (lw)
        [Test]
        public void RiscV_dasm_0000400C()
        {
            AssertCode("@@@", 0x0000400C);
        }

        // Reko: a decoder for RiscV instruction 00008AB2 at address 00100000 has not been implemented. (jalr)
        [Test]
        public void RiscV_dasm_00008AB2()
        {
            AssertCode("@@@", 0x00008AB2);
        }
    }
}
