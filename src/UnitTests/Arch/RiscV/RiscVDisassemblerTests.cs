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
            //DumpWord(uInstr);
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
            AssertCode("lui\tt6,00012345", "00010010001101000101 11111 01101 11");
        }

        [Test]
        public void RiscV_dasm_sh()
        {
            AssertCode("sh\ts5,sp,+00000182", "0001100 10101 00010 001 00010 01000 11");
        }

        [Test]
        public void RiscV_dasm_lb()
        {
            AssertCode("lb\tgp,sp,-00000790", "100001110000 00010 000 00011 00000 11");
        }

        [Test]
        public void RiscV_dasm_addi()
        {
            AssertCode("addi\tsp,sp,-000001C0", "1110010000000001000000010 00100 11");
        }

        [Test]
        public void RiscV_dasm_auipc()
        {
            AssertCode("auipc\tgp,000FFFFD", "11111111111111111 101 00011 00101 11");
        }

        [Test]
        public void RiscV_dasm_jal()
        {
            AssertCode("jal\tzero,00000000000FF1F4", 0x9F4FF06F);
        }

        [Test]
        public void RiscV_dasm_sd()
        {
            AssertCode("sd\ts5,sp,+00000188", 0x19513423u);
        }

        [Test]
        public void RiscV_dasm_addiw()
        {
            AssertCode("addiw\ta5,a5,00000008", 0x0087879Bu);
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
            AssertCode("jalr\tzero,a5,+00000000", 0x00078067u);
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
        public void RiscV_dasm_and()
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
            AssertCode("srliw\ta4,a5,00000001", 0x0017D71Bu);
        }

        [Test]
        public void RiscV_dasm_lbu()
        {
            AssertCode("lbu\ta4,s2,+00000000", 0x00094703u);
        }

        [Test]
        public void RiscV_dasm_beq()
        {
            AssertCode("beq\ta1,a5,0000000000100000", 0x00F58063u);
        }

        [Test]
        public void RiscV_dasm_flw()
        {
            AssertCode("flw\tfa4,52(s2)", 0x03492707u);
        }

        [Test]
        public void RiscV_dasm_fmv_w_x()
        {
            AssertCode("fmv.w.x\tfa5,zero", 0xF00007D3u);
        }

        [Test]
        public void RiscV_dasm_fmv_d_x()
        {
            AssertCode("fmv.d.x\tfa4,a4", 0xE2070753u);
        }

        [Test]
        public void RiscV_dasm_lwu()
        {
            AssertCode("lwu\ta4,s0,+00000004", 0x00446703u);
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
        public void RiscV_dasm_addiw_negative()
        {
            AssertCode("c.addiw\ts0,FFFFFFFFFFFFFFFF", 0x0000347D);
        }

        [Test]
        public void RiscV_dasm_c_sw()
        {
            AssertCode("c.sw\ta3,0(a5)", 0xC29C);
        }

        [Test]
        public void RiscV_dasm_c_sdsp()
        {
            AssertCode("c.sdsp\ts3,00000048", 0xE4CE);
        }

        [Test]
        public void RiscV_dasm_c_beqz()
        {
            AssertCode("c.beqz\ta0,0000000000100040", 0x0000C121);
        }

        [Test]
        public void RiscV_dasm_c_lui()
        {
            AssertCode("c.lui\ta1,00001000", 0x00006585);
        }

        [Test]
        public void RiscV_dasm_negative_3()
        {
            AssertCode("c.addiw\ts1,FFFFFFFFFFFFFFFD", 0x34F5);
        }

        [Test]
        public void RiscV_dasm_c_ld()
        {
            AssertCode("c.ld\ta0,200(a0)", 0x00006568);
        }

        [Test]
        public void RiscV_dasm_c_bnez()
        {
            AssertCode("c.bnez\ta4,000000000010001A", 0x0000EF09);
        }

        [Test]
        public void RiscV_dasm_remuw()
        {
            AssertCode("remuw\ta6,a6,a2", 0x02C8783B);
        }

        [Test]
        public void RiscV_dasm_c_li()
        {
            AssertCode("c.li\ta0,00000008", 0x00004521);
        }

        [Test]
        public void RiscV_dasm_c_swsp()
        {
            AssertCode("c.swsp\ta0,00000080", 0xC22A);
        }

        [Test]
        public void RiscV_dasm_c_li_minus3()
        {
            AssertCode("c.li\ta4,FFFFFFFFFFFFFFFD", 0x00005775);
        }

        [Test]
        public void RiscV_dasm_c_lwsp()
        {
            AssertCode("c.lwsp\ttp,00000044", 0x00004512);
        }

        [Test]
        public void RiscV_dasm_c_mv()
        {
            AssertCode("c.mv\ts0,s3", 0x844E);
        }

        [Test]
        public void RiscV_dasm_c_lw()
        {
            AssertCode("c.lw\ta5,68(a3)", 0x000043F4);
        }

        [Test]
        public void RiscV_dasm_divw()
        {
            AssertCode("divw\ts0,s0,a1", 0x02B4443B);
        }

        [Test]
        public void RiscV_dasm_c_addi16sp()
        {
            AssertCode("c.addi16sp\t000000D0", 0x6169);
        }

        [Test]
        public void RiscV_dasm_beqz_backward()
        {
            AssertCode("c.beqz\ta5,00000000000FFF06", 0xD399);
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

        [Test]
        public void RiscV_dasm_beqz_0000C3F1()
        {
            AssertCode("c.beqz\ta5,00000000001000C4", 0x0000C3F1);
        }

        [Test]
        public void RiscV_dasm_c_bnez_backward()
        {
            AssertCode("c.bnez\ta4,00000000000FFF30", 0xFB05);
        }

        [Test]
        public void RiscV_dasm_c_addiw()
        {
            AssertCode("c.addiw\ts0,00000001", 0x00002405);
        }

        // Reko: a decoder for RiscV instruction 62696C2F at address 00100000 has not been implemented. (amo)
        [Test]
        [Ignore("ASCII code decoded as text")]
        public void RiscV_dasm_62696C2F()
        {
            AssertCode("@@@", 0x62696C2F);
        }


        // Reko: a decoder for RiscV instruction 36766373 at address 00100000 has not been implemented. (system)
        [Test]
        [Ignore("ASCII code decoded as text")]
        public void RiscV_dasm_36766373()
        {
            AssertCode("@@@", 0x36766373);
        }

        [Test]
        public void RiscV_dasm_c_fldsp()
        {
            AssertCode("c.fldsp\tfa3,00000228", 0x00003436);
        }

        [Test]
        public void RiscV_dasm_invalid()
        {
            AssertCode("invalid", 0x00000000);
        }

        [Test]
        public void RiscV_dasm_jr_ra()
        {
            AssertCode("c.jr\tra", 0x00008082);
        }

        [Test]
        public void RiscV_dasm_c_or()
        {
            AssertCode("c.or\ta2,a3", 0x8E55);
        }

        [Test]
        public void RiscV_dasm_c_and()
        {
            AssertCode("c.and\ta5,a3", 0x8FF5);
        }

        [Test]
        public void RiscV_dasm_c_j()
        {
            AssertCode("c.j\t00000000001003FC", 0x0000B7D5);
        }

        [Test]
        public void RiscV_dasm_c_sub()
        {
            AssertCode("c.sub\ta1,a0", 0x8D89);
        }

        [Test]
        public void RiscV_dasm_c_j_backward()
        {
            AssertCode("c.j\t00000000000FFF9E", 0x0000BF1D);
        }

        [Test]
        public void RiscV_dasm_c_addi4spn()
        {
            AssertCode("c.addi4spn\ta5,00000020", 0x0000101C);
        }

        [Test]
        public void RiscV_dasm_c_jr()
        {
            AssertCode("c.jr\ta5", 0x00008782);
        }

        [Test]
        public void RiscV_dasm_c_subw()
        {
            AssertCode("c.subw\ta0,a5", 0x00009D1D);
        }

        [Test]
        public void RiscV_dasm_c_addi()
        {
            AssertCode("c.addi\ta5,00000001", 0x00000785);
        }

        [Test]
        public void RiscV_dasm_c_addw()
        {
            AssertCode("c.addw\ta5,a3", 0x00009FB5);
        }

        [Test]
        public void RiscV_dasm_c_srli()
        {
            AssertCode("c.srli\ta5,0000000A", 0x000083A9);
        }

        [Test]
        public void RiscV_dasm_c_srai()
        {
            AssertCode("c.srai\ta4,0000003F", 0x0000977D);
        }

        [Test]
        public void RiscV_dasm_c_andi()
        {
            AssertCode("c.andi\ta2,00000018", 0x00008A61);
        }

        [Test]
        public void RiscV_dasm_c_ldsp()
        {
            AssertCode("c.ldsp\ts0,000001D0", 0x00006BA2);
        }

        [Test]
        public void RiscV_dasm_c_slli()
        {
            AssertCode("c.slli\ts0,03", 0x0000040E);
        }

        [Test]
        public void RiscV_dasm_c_fld()
        {
            AssertCode("c.fld\tfa2,216(s1)", 0x00002E64);
        }

        [Test]
        public void RiscV_dasm_sll()
        {
            AssertCode("sll\ta5,s6,s0", 0x008B17B3);
        }

        [Test]
        public void RiscV_dasm_sltu()
        {
            AssertCode("sltu\ta0,zero,a0", 0x00A03533);
        }

        [Test]
        public void RiscV_dasm_slt()
        {
            AssertCode("slt\ta0,a5,a0", 0x00A7A533);
        }

        [Test]
        public void RiscV_dasm_remw()
        {
            AssertCode("remw\ta3,a5,a3", 0x02D7E6BB);
        }

        [Test]
        public void RiscV_dasm_fmsub_s()
        {
            AssertCode("fmsub.s\tfa1,fa7,fa7,fa2", 0x6318B5C7);
        }

        [Test]
        public void RiscV_dasm_fnmsub_s()
        {
            AssertCode("fnmsub.s\tft0,fs2,fs8,fs0", 0x4789004B);
        }


        [Test]
        public void RiscV_dasm_fnmadd_s()
        {
            AssertCode("fnmadd.s\tfs11,ft7,fa1,ft0", 0x04B3FDCF);
        }

        [Test]
        public void RiscV_dasm_divuw()
        {
            AssertCode("divuw\ta5,a6,a2", 0x02C857BB);
        }


        [Test]
        public void RiscV_dasm_c_fsd()
        {
            AssertCode("c.fsd\tfa2,8(s1)", 0x0000A604);
        }

        [Test]
        public void RiscV_dasm_c_fsdsp()
        {
            AssertCode("c.fsdsp\tfs9,000001C8", 0xA7E6);
        }
    }
}
