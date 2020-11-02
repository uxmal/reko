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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.RiscV
{
    [TestFixture]
    public class RiscVDisassemblerTests : DisassemblerTestBase<RiscVInstruction>
    {
        public RiscVDisassemblerTests()
        {
            this.Architecture = new RiscVArchitecture(new ServiceContainer(), "riscV");
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        private void AssertCode(string sExp, uint uInstr)
        {
            var i = DisassembleWord(uInstr);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void AssertBitString(string sExp, string bits)
        {
            var i = DisassembleBits(bits);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void RiscV_dasm_lui()
        {
            AssertBitString("lui\tt6,00012345", "00010010001101000101 11111 01101 11");
        }

        [Test]
        public void RiscV_dasm_sh()
        {
            AssertBitString("sh\ts5,sp,+00000182", "0001100 10101 00010 001 00010 01000 11");
        }

        [Test]
        public void RiscV_dasm_lb()
        {
            AssertBitString("lb\tgp,sp,-00000790", "100001110000 00010 000 00011 00000 11");
        }

        [Test]
        public void RiscV_dasm_addi()
        {
            AssertBitString("addi\tsp,sp,-000001C0", "1110010000000001000000010 00100 11");
        }

        [Test]
        public void RiscV_dasm_auipc()
        {
            AssertBitString("auipc\tgp,000FFFFD", "11111111111111111 101 00011 00101 11");
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
        public void RiscV_dasm_c_sd()
        {
            AssertCode("c.sd\ts0,56(s0)", "00FC");
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
        public void RiscV_dasm_slli()
        {
            AssertCode("slli\ta2,s2,00000020", 0x02091613);
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

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues

        // Reko: a decoder for the instruction 73B00230 at address 23000002 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73B00230()
        {
            AssertBitString("@@@", "73B00230");
        }
        // Reko: a decoder for the instruction 912C at address 23082634 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_912C()
        {
            AssertCode("@@@", "912C");
        }
        // Reko: a decoder for the instruction AF028507 at address 2308294A has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AF028507()
        {
            AssertCode("@@@", "AF028507");
        }
        // Reko: a decoder for the instruction F7031447 at address 2300673C has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7031447()
        {
            AssertCode("@@@", "F7031447");
        }
        // Reko: a decoder for the instruction FB0061B7 at address 23004C4A has not been implemented. (custom-3)
        [Test]
        public void RiscV_dasm_FB0061B7()
        {
            AssertCode("@@@", "FB0061B7");
        }
        // Reko: a decoder for the instruction 73292000 at address 230A1B8C has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73292000()
        {
            AssertCode("@@@", "73292000");
        }
        // Reko: a decoder for the instruction F3272000 at address 230A2FB4 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3272000()
        {
            AssertCode("@@@", "F3272000");
        }

        // Reko: a decoder for the instruction 8922 at address 23029150 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_8922()
        {
            AssertCode("@@@", "8922");
        }
        // Reko: a decoder for the instruction 73700430 at address 23063658 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73700430()
        {
            AssertCode("@@@", "73700430");
        }
        // Reko: a decoder for the instruction 73600430 at address 230623FC has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73600430()
        {
            AssertCode("@@@", "73600430");
        }
        // Reko: a decoder for the instruction DB008326 at address 23082E6C has not been implemented. (custom-2)
        [Test]
        public void RiscV_dasm_DB008326()
        {
            AssertCode("@@@", "DB008326");
        }
        // Reko: a decoder for the instruction F3220030 at address 23064700 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3220030()
        {
            AssertCode("@@@", "F3220030");
        }
        // Reko: a decoder for the instruction 73005010 at address 23000BC4 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73005010()
        {
            AssertCode("@@@", "73005010");
        }
        // Reko: a decoder for the instruction 1FE726C6 at address 23061F94 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_1FE726C6()
        {
            AssertCode("@@@", "1FE726C6");
        }
        // Reko: a decoder for the instruction DF92DDBF at address 2304E896 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_DF92DDBF()
        {
            AssertCode("@@@", "DF92DDBF");
        }
        // Reko: a decoder for the instruction 1FA8631B at address 2304F4CC has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_1FA8631B()
        {
            AssertCode("@@@", "1FA8631B");
        }
        // Reko: a decoder for the instruction 7FA61397 at address 2306159E has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_7FA61397()
        {
            AssertCode("@@@", "7FA61397");
        }
        // Reko: a decoder for the instruction 5FC0EDB7 at address 2304B4EA has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_5FC0EDB7()
        {
            AssertCode("@@@", "5FC0EDB7");
        }
        // Reko: a decoder for the instruction 5F8A3765 at address 2304A904 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_5F8A3765()
        {
            AssertCode("@@@", "5F8A3765");
        }
        // Reko: a decoder for the instruction 70FC at address 2304A70C has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_70FC()
        {
            AssertCode("@@@", "70FC");
        }
        // Reko: a decoder for the instruction E924 at address 23055320 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_E924()
        {
            AssertCode("@@@", "E924");
        }
        // Reko: a decoder for the instruction 0FC0E35A at address 2304F270 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_0FC0E35A()
        {
            AssertCode("@@@", "0FC0E35A");
        }
        // Reko: a decoder for the instruction 0523 at address 23057C20 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0523()
        {
            AssertCode("@@@", "0523");
        }
        // Reko: a decoder for the instruction 2D24 at address 23049A58 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_2D24()
        {
            AssertCode("@@@", "2D24");
        }
        // Reko: a decoder for the instruction E522 at address 23049A9A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_E522()
        {
            AssertCode("@@@", "E522");
        }
        // Reko: a decoder for the instruction 3922 at address 23049B50 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3922()
        {
            AssertCode("@@@", "3922");
        }
        // Reko: a decoder for the instruction 6528 at address 23049BA6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_6528()
        {
            AssertCode("@@@", "6528");
        }
        // Reko: a decoder for the instruction FD28 at address 23049B60 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_FD28()
        {
            AssertCode("@@@", "FD28");
        }
        // Reko: a decoder for the instruction 9924 at address 23049A3C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_9924()
        {
            AssertCode("@@@", "9924");
        }
        // Reko: a decoder for the instruction 3528 at address 23049C46 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3528()
        {
            AssertCode("@@@", "3528");
        }
        // Reko: a decoder for the instruction F70F63F4 at address 23052BD8 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F70F63F4()
        {
            AssertCode("@@@", "F70F63F4");
        }
        // Reko: a decoder for the instruction D529 at address 230553C4 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_D529()
        {
            AssertCode("@@@", "D529");
        }
        // Reko: a decoder for the instruction 292C at address 230553D0 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_292C()
        {
            AssertCode("@@@", "292C");
        }
        // Reko: a decoder for the instruction 9FB3D247 at address 2304C9A0 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_9FB3D247()
        {
            AssertCode("@@@", "9FB3D247");
        }
        // Reko: a decoder for the instruction 0129 at address 23059156 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0129()
        {
            AssertCode("@@@", "0129");
        }
        // Reko: a decoder for the instruction 1129 at address 2305917A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1129()
        {
            AssertCode("@@@", "1129");
        }
        // Reko: a decoder for the instruction B52D at address 23058F12 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_B52D()
        {
            AssertCode("@@@", "B52D");
        }
        // Reko: a decoder for the instruction 3929 at address 23058F62 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3929()
        {
            AssertCode("@@@", "3929");
        }
        // Reko: a decoder for the instruction 0FAE41FD at address 23055302 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_0FAE41FD()
        {
            AssertCode("@@@", "0FAE41FD");
        }
        // Reko: a decoder for the instruction B126 at address 23059034 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_B126()
        {
            AssertCode("@@@", "B126");
        }
        // Reko: a decoder for the instruction 6D21 at address 2305910C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_6D21()
        {
            AssertCode("@@@", "6D21");
        }
        // Reko: a decoder for the instruction FF013346 at address 23079F28 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_FF013346()
        {
            AssertCode("@@@", "FF013346");
        }
        // Reko: a decoder for the instruction 4D20 at address 2307B008 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4D20()
        {
            AssertCode("@@@", "4D20");
        }
        // Reko: a decoder for the instruction E92C at address 230590A6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_E92C()
        {
            AssertCode("@@@", "E92C");
        }
        // Reko: a decoder for the instruction 0B23D98F at address 23059FFE has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B23D98F()
        {
            AssertCode("@@@", "0B23D98F");
        }
        // Reko: a decoder for the instruction 0B23CE85 at address 2305A0F4 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B23CE85()
        {
            AssertCode("@@@", "0B23CE85");
        }
        // Reko: a decoder for the instruction 0B238509 at address 2305AA7A has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B238509()
        {
            AssertCode("@@@", "0B238509");
        }
        // Reko: a decoder for the instruction 0B231306 at address 2305A95C has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B231306()
        {
            AssertCode("@@@", "0B231306");
        }
        // Reko: a decoder for the instruction 0487 at address 2305A51E has not been implemented. (reserved)
        [Test]
        public void RiscV_dasm_0487()
        {
            AssertCode("@@@", "0487");
        }
        // Reko: a decoder for the instruction DFE23247 at address 2305A444 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_DFE23247()
        {
            AssertCode("@@@", "DFE23247");
        }
        // Reko: a decoder for the instruction 0B239385 at address 2305A76E has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B239385()
        {
            AssertCode("@@@", "0B239385");
        }
        // Reko: a decoder for the instruction E92D at address 2306B538 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_E92D()
        {
            AssertCode("@@@", "E92D");
        }
        // Reko: a decoder for the instruction 5FDA2104 at address 2306F9FE has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_5FDA2104()
        {
            AssertCode("@@@", "5FDA2104");
        }
        // Reko: a decoder for the instruction 3F93FD57 at address 230706E8 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3F93FD57()
        {
            AssertCode("@@@", "3F93FD57");
        }
        // Reko: a decoder for the instruction 8B925286 at address 230708BE has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_8B925286()
        {
            AssertCode("@@@", "8B925286");
        }
        // Reko: a decoder for the instruction 312E at address 2306F01E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_312E()
        {
            AssertCode("@@@", "312E");
        }
        // Reko: a decoder for the instruction BFF037F5 at address 2306CB36 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFF037F5()
        {
            AssertCode("@@@", "BFF037F5");
        }
        // Reko: a decoder for the instruction D702DDBF at address 23070E80 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D702DDBF()
        {
            AssertCode("@@@", "D702DDBF");
        }
        // Reko: a decoder for the instruction 7FA32247 at address 230741EC has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_7FA32247()
        {
            AssertCode("@@@", "7FA32247");
        }
        // Reko: a decoder for the instruction 3FC483A7 at address 23071FD2 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3FC483A7()
        {
            AssertCode("@@@", "3FC483A7");
        }
        // Reko: a decoder for the instruction BFCF8357 at address 23073334 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFCF8357()
        {
            AssertCode("@@@", "BFCF8357");
        }
        // Reko: a decoder for the instruction 0FA8F9B5 at address 23077364 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_0FA8F9B5()
        {
            AssertCode("@@@", "0FA8F9B5");
        }
        // Reko: a decoder for the instruction 1929 at address 2306EF24 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1929()
        {
            AssertCode("@@@", "1929");
        }
        // Reko: a decoder for the instruction 252D at address 2306ED02 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_252D()
        {
            AssertCode("@@@", "252D");
        }
        // Reko: a decoder for the instruction 7FE711BF at address 23070330 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_7FE711BF()
        {
            AssertCode("@@@", "7FE711BF");
        }
        // Reko: a decoder for the instruction CD29 at address 2306B720 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_CD29()
        {
            AssertCode("@@@", "CD29");
        }
        // Reko: a decoder for the instruction FFAE1DE9 at address 2306BB7A has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_FFAE1DE9()
        {
            AssertCode("@@@", "FFAE1DE9");
        }
        // Reko: a decoder for the instruction 5FDFFD57 at address 23075846 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_5FDFFD57()
        {
            AssertCode("@@@", "5FDFFD57");
        }
        // Reko: a decoder for the instruction 0B2337FC at address 2305B084 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B2337FC()
        {
            AssertCode("@@@", "0B2337FC");
        }
        // Reko: a decoder for the instruction BFBE48D0 at address 2305BFAE has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFBE48D0()
        {
            AssertCode("@@@", "BFBE48D0");
        }
        // Reko: a decoder for the instruction F3242000 at address 230A04D0 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3242000()
        {
            AssertCode("@@@", "F3242000");
        }
        // Reko: a decoder for the instruction F3292000 at address 230A2308 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3292000()
        {
            AssertCode("@@@", "F3292000");
        }
        // Reko: a decoder for the instruction 8FF6AA85 at address 2305CFC6 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_8FF6AA85()
        {
            AssertCode("@@@", "8FF6AA85");
        }
        // Reko: a decoder for the instruction 6520 at address 2305D1FE has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_6520()
        {
            AssertCode("@@@", "6520");
        }
        // Reko: a decoder for the instruction 0933 at address 2305CF70 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0933()
        {
            AssertCode("@@@", "0933");
        }
        // Reko: a decoder for the instruction 9FE42A84 at address 2305C92A has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_9FE42A84()
        {
            AssertCode("@@@", "9FE42A84");
        }
        // Reko: a decoder for the instruction 053B at address 2305CF42 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_053B()
        {
            AssertCode("@@@", "053B");
        }
        // Reko: a decoder for the instruction 8539 at address 2305D002 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_8539()
        {
            AssertCode("@@@", "8539");
        }
        // Reko: a decoder for the instruction F52C at address 2307ADE2 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_F52C()
        {
            AssertCode("@@@", "F52C");
        }
        // Reko: a decoder for the instruction 2124 at address 2307AF38 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_2124()
        {
            AssertCode("@@@", "2124");
        }
        // Reko: a decoder for the instruction 6D24 at address 2307AE96 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_6D24()
        {
            AssertCode("@@@", "6D24");
        }
        // Reko: a decoder for the instruction 853A at address 23084C0C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_853A()
        {
            AssertCode("@@@", "853A");
        }
        // Reko: a decoder for the instruction 0F9B1304 at address 230851DA has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_0F9B1304()
        {
            AssertCode("@@@", "0F9B1304");
        }
        // Reko: a decoder for the instruction 73D02334 at address 23085534 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73D02334()
        {
            AssertCode("@@@", "73D02334");
        }
        // Reko: a decoder for the instruction 73900630 at address 23085572 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73900630()
        {
            AssertCode("@@@", "73900630");
        }
        // Reko: a decoder for the instruction 1D23 at address 23085ECC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1D23()
        {
            AssertCode("@@@", "1D23");
        }
        // Reko: a decoder for the instruction 3D24 at address 23086198 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3D24()
        {
            AssertCode("@@@", "3D24");
        }
        // Reko: a decoder for the instruction C121 at address 23085F32 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C121()
        {
            AssertCode("@@@", "C121");
        }
        // Reko: a decoder for the instruction 3D2C at address 23086176 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3D2C()
        {
            AssertCode("@@@", "3D2C");
        }
        // Reko: a decoder for the instruction 732F3034 at address 2308538C has not been implemented. (system)
        [Test]
        public void RiscV_dasm_732F3034()
        {
            AssertCode("@@@", "732F3034");
        }
        // Reko: a decoder for the instruction F7008D8F at address 2305F596 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7008D8F()
        {
            AssertCode("@@@", "F7008D8F");
        }
        // Reko: a decoder for the instruction 7F859374 at address 2305F93E has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_7F859374()
        {
            AssertCode("@@@", "7F859374");
        }
        // Reko: a decoder for the instruction F928 at address 2308382C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_F928()
        {
            AssertCode("@@@", "F928");
        }
        // Reko: a decoder for the instruction 0FBDDDBF at address 230665D6 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_0FBDDDBF()
        {
            AssertCode("@@@", "0FBDDDBF");
        }
        // Reko: a decoder for the instruction 113D at address 2306663A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_113D()
        {
            AssertCode("@@@", "113D");
        }
        // Reko: a decoder for the instruction EB7963F1 at address 23066E26 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_EB7963F1()
        {
            AssertCode("@@@", "EB7963F1");
        }
        // Reko: a decoder for the instruction 3FB229B7 at address 23066E84 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3FB229B7()
        {
            AssertCode("@@@", "3FB229B7");
        }
        // Reko: a decoder for the instruction 9F882AD2 at address 23068140 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_9F882AD2()
        {
            AssertCode("@@@", "9F882AD2");
        }
        // Reko: a decoder for the instruction 8083 at address 23083D38 has not been implemented. (reserved)
        [Test]
        public void RiscV_dasm_8083()
        {
            AssertCode("@@@", "8083");
        }
        // Reko: a decoder for the instruction 0B233EC6 at address 230687BC has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B233EC6()
        {
            AssertCode("@@@", "0B233EC6");
        }
        // Reko: a decoder for the instruction D7FE8547 at address 23068F8C has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D7FE8547()
        {
            AssertCode("@@@", "D7FE8547");
        }
        // Reko: a decoder for the instruction C93D at address 23068FC0 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C93D()
        {
            AssertCode("@@@", "C93D");
        }
        // Reko: a decoder for the instruction 092C at address 23069A52 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_092C()
        {
            AssertCode("@@@", "092C");
        }
        // Reko: a decoder for the instruction 812A at address 23068F62 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_812A()
        {
            AssertCode("@@@", "812A");
        }
        // Reko: a decoder for the instruction F7441307 at address 230689C4 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7441307()
        {
            AssertCode("@@@", "F7441307");
        }
        // Reko: a decoder for the instruction 7F830509 at address 2305E4FA has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_7F830509()
        {
            AssertCode("@@@", "7F830509");
        }
        // Reko: a decoder for the instruction 7537 at address 2305D866 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_7537()
        {
            AssertCode("@@@", "7537");
        }
        // Reko: a decoder for the instruction E128 at address 2305C0E0 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_E128()
        {
            AssertCode("@@@", "E128");
        }
        // Reko: a decoder for the instruction 0BE003C6 at address 2305A6BE has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0BE003C6()
        {
            AssertCode("@@@", "0BE003C6");
        }
        // Reko: a decoder for the instruction 0B231307 at address 2305A462 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B231307()
        {
            AssertCode("@@@", "0B231307");
        }
        // Reko: a decoder for the instruction 0B239386 at address 2305A1CC has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B239386()
        {
            AssertCode("@@@", "0B239386");
        }
        // Reko: a decoder for the instruction 0B23B240 at address 23055B16 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B23B240()
        {
            AssertCode("@@@", "0B23B240");
        }
        // Reko: a decoder for the instruction FFB383C7 at address 230507A2 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_FFB383C7()
        {
            AssertCode("@@@", "FFB383C7");
        }
        // Reko: a decoder for the instruction 0290 at address 23035472 has not been implemented. (c.ebreak)
        [Test]
        public void RiscV_dasm_0290()
        {
            AssertCode("@@@", "0290");
        }
        // Reko: a decoder for the instruction 73280030 at address 2302BA96 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73280030()
        {
            AssertCode("@@@", "73280030");
        }
        // Reko: a decoder for the instruction 2FDFB240 at address 2302C2EE has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_2FDFB240()
        {
            AssertCode("@@@", "2FDFB240");
        }
        // Reko: a decoder for the instruction 73270030 at address 2302B850 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73270030()
        {
            AssertCode("@@@", "73270030");
        }
        // Reko: a decoder for the instruction F3290030 at address 23029A22 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3290030()
        {
            AssertCode("@@@", "F3290030");
        }
        // Reko: a decoder for the instruction 73900930 at address 23029AB4 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73900930()
        {
            AssertCode("@@@", "73900930");
        }
        // Reko: a decoder for the instruction E12D at address 23029C60 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_E12D()
        {
            AssertCode("@@@", "E12D");
        }
        // Reko: a decoder for the instruction F7048357 at address 23029FD0 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7048357()
        {
            AssertCode("@@@", "F7048357");
        }
        // Reko: a decoder for the instruction F32A0030 at address 2302BB7C has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F32A0030()
        {
            AssertCode("@@@", "F32A0030");
        }
        // Reko: a decoder for the instruction F3240030 at address 23035160 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3240030()
        {
            AssertCode("@@@", "F3240030");
        }
        // Reko: a decoder for the instruction 73240030 at address 230381F8 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73240030()
        {
            AssertCode("@@@", "73240030");
        }
        // Reko: a decoder for the instruction BFE4B705 at address 2300BB5C has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFE4B705()
        {
            AssertCode("@@@", "BFE4B705");
        }
        // Reko: a decoder for the instruction 5FEDAA89 at address 23091FAA has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_5FEDAA89()
        {
            AssertCode("@@@", "5FEDAA89");
        }
        // Reko: a decoder for the instruction D7FA1844 at address 230922B6 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D7FA1844()
        {
            AssertCode("@@@", "D7FA1844");
        }
        // Reko: a decoder for the instruction 2F8C2A84 at address 23092DBC has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_2F8C2A84()
        {
            AssertCode("@@@", "2F8C2A84");
        }
        // Reko: a decoder for the instruction 0923 at address 23093A3C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0923()
        {
            AssertCode("@@@", "0923");
        }
        // Reko: a decoder for the instruction 9F892A84 at address 23093C0E has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_9F892A84()
        {
            AssertCode("@@@", "9F892A84");
        }
        // Reko: a decoder for the instruction F4FC at address 230937FE has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_F4FC()
        {
            AssertCode("@@@", "F4FC");
        }
        // Reko: a decoder for the instruction EB0081EA at address 2309627A has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_EB0081EA()
        {
            AssertCode("@@@", "EB0081EA");
        }
        // Reko: a decoder for the instruction FF94E342 at address 23092BDA has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_FF94E342()
        {
            AssertCode("@@@", "FF94E342");
        }
        // Reko: a decoder for the instruction 4925 at address 2308B19E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4925()
        {
            AssertCode("@@@", "4925");
        }
        // Reko: a decoder for the instruction D701E20A at address 230971B6 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D701E20A()
        {
            AssertCode("@@@", "D701E20A");
        }
        // Reko: a decoder for the instruction 3521 at address 23096C16 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3521()
        {
            AssertCode("@@@", "3521");
        }
        // Reko: a decoder for the instruction 20FD at address 23096990 has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_20FD()
        {
            AssertCode("@@@", "20FD");
        }
        // Reko: a decoder for the instruction FFEFE34B at address 2308F4DE has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_FFEFE34B()
        {
            AssertCode("@@@", "FFEFE34B");
        }
        // Reko: a decoder for the instruction 5F90E346 at address 23092CB0 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_5F90E346()
        {
            AssertCode("@@@", "5F90E346");
        }
        // Reko: a decoder for the instruction AFAF2A84 at address 23094BF2 has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFAF2A84()
        {
            AssertCode("@@@", "AFAF2A84");
        }
        // Reko: a decoder for the instruction D70205EB at address 2309233A has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D70205EB()
        {
            AssertCode("@@@", "D70205EB");
        }
        // Reko: a decoder for the instruction 7FC62A8C at address 23095446 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_7FC62A8C()
        {
            AssertCode("@@@", "7FC62A8C");
        }
        // Reko: a decoder for the instruction BF802A84 at address 23095016 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BF802A84()
        {
            AssertCode("@@@", "BF802A84");
        }
        // Reko: a decoder for the instruction 1FC8DDBF at address 23095164 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_1FC8DDBF()
        {
            AssertCode("@@@", "1FC8DDBF");
        }
        // Reko: a decoder for the instruction BFBB85BF at address 2309613A has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFBB85BF()
        {
            AssertCode("@@@", "BFBB85BF");
        }
        // Reko: a decoder for the instruction E121 at address 23088218 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_E121()
        {
            AssertCode("@@@", "E121");
        }
        // Reko: a decoder for the instruction F4FE at address 23087F06 has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_F4FE()
        {
            AssertCode("@@@", "F4FE");
        }
        // Reko: a decoder for the instruction FFAE2A84 at address 230952B0 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_FFAE2A84()
        {
            AssertCode("@@@", "FFAE2A84");
        }
        // Reko: a decoder for the instruction 9FD011E5 at address 23095B18 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_9FD011E5()
        {
            AssertCode("@@@", "9FD011E5");
        }
        // Reko: a decoder for the instruction AFF16310 at address 2308830A has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFF16310()
        {
            AssertCode("@@@", "AFF16310");
        }
        // Reko: a decoder for the instruction 448B at address 2309743E has not been implemented. (reserved)
        [Test]
        public void RiscV_dasm_448B()
        {
            AssertCode("@@@", "448B");
        }
        // Reko: a decoder for the instruction 1D3D at address 2308F9AA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1D3D()
        {
            AssertCode("@@@", "1D3D");
        }
        // Reko: a decoder for the instruction 5FC47975 at address 23090002 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_5FC47975()
        {
            AssertCode("@@@", "5FC47975");
        }
        // Reko: a decoder for the instruction 4D2D at address 2308FDF2 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4D2D()
        {
            AssertCode("@@@", "4D2D");
        }
        // Reko: a decoder for the instruction 792A at address 23090306 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_792A()
        {
            AssertCode("@@@", "792A");
        }
        // Reko: a decoder for the instruction F70FE315 at address 2309053A has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F70FE315()
        {
            AssertCode("@@@", "F70FE315");
        }
        // Reko: a decoder for the instruction 1FE96DD1 at address 23096AB0 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_1FE96DD1()
        {
            AssertCode("@@@", "1FE96DD1");
        }
        // Reko: a decoder for the instruction F70F2380 at address 2308B4E4 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F70F2380()
        {
            AssertCode("@@@", "F70F2380");
        }
        // Reko: a decoder for the instruction 1F01B3CF at address 23097450 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_1F01B3CF()
        {
            AssertCode("@@@", "1F01B3CF");
        }
        // Reko: a decoder for the instruction 1FB28387 at address 230939BA has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_1FB28387()
        {
            AssertCode("@@@", "1FB28387");
        }
        // Reko: a decoder for the instruction FB18EFD0 at address 2308DA72 has not been implemented. (custom-3)
        [Test]
        public void RiscV_dasm_FB18EFD0()
        {
            AssertCode("@@@", "FB18EFD0");
        }
        // Reko: a decoder for the instruction 8FA66F30 at address 230968DE has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_8FA66F30()
        {
            AssertCode("@@@", "8FA66F30");
        }
        // Reko: a decoder for the instruction F7021440 at address 2308E9DE has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7021440()
        {
            AssertCode("@@@", "F7021440");
        }
        // Reko: a decoder for the instruction D492 at address 2308CA54 has not been implemented. (reserved)
        [Test]
        public void RiscV_dasm_D492()
        {
            AssertCode("@@@", "D492");
        }
        // Reko: a decoder for the instruction BFC02A84 at address 2308D832 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFC02A84()
        {
            AssertCode("@@@", "BFC02A84");
        }
        // Reko: a decoder for the instruction 0530 at address 4200CD4A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0530()
        {
            AssertCode("@@@", "0530");
        }
        // Reko: a decoder for the instruction B530 at address 4200CCEA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_B530()
        {
            AssertCode("@@@", "B530");
        }
        // Reko: a decoder for the instruction D12A at address 4200CB6C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_D12A()
        {
            AssertCode("@@@", "D12A");
        }
        // Reko: a decoder for the instruction D524 at address 4200CA00 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_D524()
        {
            AssertCode("@@@", "D524");
        }
        // Reko: a decoder for the instruction F3282000 at address 230A32BC has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3282000()
        {
            AssertCode("@@@", "F3282000");
        }
        // Reko: a decoder for the instruction 8FED2A84 at address 2309BC3A has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_8FED2A84()
        {
            AssertCode("@@@", "8FED2A84");
        }
        // Reko: a decoder for the instruction 3FA3FD57 at address 2309C3C4 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3FA3FD57()
        {
            AssertCode("@@@", "3FA3FD57");
        }
        // Reko: a decoder for the instruction 0B231305 at address 23099776 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B231305()
        {
            AssertCode("@@@", "0B231305");
        }
        // Reko: a decoder for the instruction 7F8AF808 at address 2309CAAC has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_7F8AF808()
        {
            AssertCode("@@@", "7F8AF808");
        }
        // Reko: a decoder for the instruction AFDB03A7 at address 2309A2F8 has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFDB03A7()
        {
            AssertCode("@@@", "AFDB03A7");
        }
        // Reko: a decoder for the instruction AFB537A7 at address 2309A262 has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFB537A7()
        {
            AssertCode("@@@", "AFB537A7");
        }
        // Reko: a decoder for the instruction AB0037A7 at address 2309A41E has not been implemented. (custom-1)
        [Test]
        public void RiscV_dasm_AB0037A7()
        {
            AssertCode("@@@", "AB0037A7");
        }
        // Reko: a decoder for the instruction E139 at address 23091490 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_E139()
        {
            AssertCode("@@@", "E139");
        }
        // Reko: a decoder for the instruction AFF32A84 at address 2308EBB4 has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFF32A84()
        {
            AssertCode("@@@", "AFF32A84");
        }
        // Reko: a decoder for the instruction 9F968328 at address 2308B0F2 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_9F968328()
        {
            AssertCode("@@@", "9F968328");
        }
        // Reko: a decoder for the instruction 5522 at address 2308850E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_5522()
        {
            AssertCode("@@@", "5522");
        }
        // Reko: a decoder for the instruction 9123 at address 23087490 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_9123()
        {
            AssertCode("@@@", "9123");
        }
        // Reko: a decoder for the instruction 4528 at address 2308743C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4528()
        {
            AssertCode("@@@", "4528");
        }
        // Reko: a decoder for the instruction 5123 at address 23087444 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_5123()
        {
            AssertCode("@@@", "5123");
        }
        // Reko: a decoder for the instruction 7FD68A85 at address 230877A0 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_7FD68A85()
        {
            AssertCode("@@@", "7FD68A85");
        }
        // Reko: a decoder for the instruction D7003317 at address 23071934 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D7003317()
        {
            AssertCode("@@@", "D7003317");
        }
        // Reko: a decoder for the instruction 9FF60945 at address 230714AC has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_9FF60945()
        {
            AssertCode("@@@", "9FF60945");
        }
        // Reko: a decoder for the instruction 5701DC47 at address 2307105C has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_5701DC47()
        {
            AssertCode("@@@", "5701DC47");
        }
        // Reko: a decoder for the instruction 192E at address 23086626 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_192E()
        {
            AssertCode("@@@", "192E");
        }
        // Reko: a decoder for the instruction 7F996D57 at address 2306C4C6 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_7F996D57()
        {
            AssertCode("@@@", "7F996D57");
        }
        // Reko: a decoder for the instruction F3A60530 at address 23085498 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3A60530()
        {
            AssertCode("@@@", "F3A60530");
        }
        // Reko: a decoder for the instruction 3FD1B726 at address 23067CB6 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3FD1B726()
        {
            AssertCode("@@@", "3FD1B726");
        }
        // Reko: a decoder for the instruction BFC18657 at address 23067DAE has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFC18657()
        {
            AssertCode("@@@", "BFC18657");
        }
        // Reko: a decoder for the instruction F7009305 at address 23035638 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7009305()
        {
            AssertCode("@@@", "F7009305");
        }
        // Reko: a decoder for the instruction D70A9305 at address 23034DD4 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D70A9305()
        {
            AssertCode("@@@", "D70A9305");
        }
        // Reko: a decoder for the instruction 052A at address 23084ACA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_052A()
        {
            AssertCode("@@@", "052A");
        }
        // Reko: a decoder for the instruction 73002030 at address 230841C4 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73002030()
        {
            AssertCode("@@@", "73002030");
        }
        // Reko: a decoder for the instruction 3928 at address 2308240A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3928()
        {
            AssertCode("@@@", "3928");
        }
        // Reko: a decoder for the instruction 57FF3387 at address 2308218E has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_57FF3387()
        {
            AssertCode("@@@", "57FF3387");
        }
        // Reko: a decoder for the instruction D70063F9 at address 23081998 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D70063F9()
        {
            AssertCode("@@@", "D70063F9");
        }
        // Reko: a decoder for the instruction E0FE at address 23081D52 has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_E0FE()
        {
            AssertCode("@@@", "E0FE");
        }
        // Reko: a decoder for the instruction 8B419C18 at address 23081B92 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_8B419C18()
        {
            AssertCode("@@@", "8B419C18");
        }
        // Reko: a decoder for the instruction D70F0547 at address 23081184 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D70F0547()
        {
            AssertCode("@@@", "D70F0547");
        }
        // Reko: a decoder for the instruction AD29 at address 2307ACC6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_AD29()
        {
            AssertCode("@@@", "AD29");
        }
        // Reko: a decoder for the instruction 5926 at address 2307AD24 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_5926()
        {
            AssertCode("@@@", "5926");
        }
        // Reko: a decoder for the instruction 4126 at address 2307AD5E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4126()
        {
            AssertCode("@@@", "4126");
        }
        // Reko: a decoder for the instruction 0490 at address 230809D8 has not been implemented. (reserved)
        [Test]
        public void RiscV_dasm_0490()
        {
            AssertCode("@@@", "0490");
        }
        // Reko: a decoder for the instruction D70C63CD at address 23081254 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D70C63CD()
        {
            AssertCode("@@@", "D70C63CD");
        }
        // Reko: a decoder for the instruction AFC78144 at address 23080306 has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFC78144()
        {
            AssertCode("@@@", "AFC78144");
        }
        // Reko: a decoder for the instruction 0FFB9147 at address 2307FFE0 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_0FFB9147()
        {
            AssertCode("@@@", "0FFB9147");
        }
        // Reko: a decoder for the instruction 592B at address 230603C6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_592B()
        {
            AssertCode("@@@", "592B");
        }
        // Reko: a decoder for the instruction AFCD3705 at address 230607B2 has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFCD3705()
        {
            AssertCode("@@@", "AFCD3705");
        }
        // Reko: a decoder for the instruction B924 at address 23060798 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_B924()
        {
            AssertCode("@@@", "B924");
        }
        // Reko: a decoder for the instruction 792B at address 23060448 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_792B()
        {
            AssertCode("@@@", "792B");
        }
        // Reko: a decoder for the instruction 092A at address 23060858 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_092A()
        {
            AssertCode("@@@", "092A");
        }
        // Reko: a decoder for the instruction C52E at address 2307EA5C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C52E()
        {
            AssertCode("@@@", "C52E");
        }
        // Reko: a decoder for the instruction D926 at address 2307EA86 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_D926()
        {
            AssertCode("@@@", "D926");
        }
        // Reko: a decoder for the instruction 052D at address 2307E9B4 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_052D()
        {
            AssertCode("@@@", "052D");
        }
        // Reko: a decoder for the instruction B12B at address 2307E9E8 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_B12B()
        {
            AssertCode("@@@", "B12B");
        }
        // Reko: a decoder for the instruction 892B at address 2307E564 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_892B()
        {
            AssertCode("@@@", "892B");
        }
        // Reko: a decoder for the instruction ED2E at address 2307E6BC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_ED2E()
        {
            AssertCode("@@@", "ED2E");
        }
        // Reko: a decoder for the instruction C529 at address 230604F6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C529()
        {
            AssertCode("@@@", "C529");
        }
        // Reko: a decoder for the instruction 852E at address 230605EC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_852E()
        {
            AssertCode("@@@", "852E");
        }
        // Reko: a decoder for the instruction AFBA1305 at address 2307E3D6 has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFBA1305()
        {
            AssertCode("@@@", "AFBA1305");
        }
        // Reko: a decoder for the instruction 3131 at address 2305D066 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3131()
        {
            AssertCode("@@@", "3131");
        }
        // Reko: a decoder for the instruction 4D2C at address 230606CA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4D2C()
        {
            AssertCode("@@@", "4D2C");
        }
        // Reko: a decoder for the instruction B52C at address 23060700 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_B52C()
        {
            AssertCode("@@@", "B52C");
        }
        // Reko: a decoder for the instruction A12C at address 23060704 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_A12C()
        {
            AssertCode("@@@", "A12C");
        }
        // Reko: a decoder for the instruction 2D26 at address 2307ADB4 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_2D26()
        {
            AssertCode("@@@", "2D26");
        }
        // Reko: a decoder for the instruction 1125 at address 2307B7DC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1125()
        {
            AssertCode("@@@", "1125");
        }
        // Reko: a decoder for the instruction 252E at address 2307B90E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_252E()
        {
            AssertCode("@@@", "252E");
        }
        // Reko: a decoder for the instruction AFEBA245 at address 2306319A has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFEBA245()
        {
            AssertCode("@@@", "AFEBA245");
        }
        // Reko: a decoder for the instruction 9126 at address 2307BB32 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_9126()
        {
            AssertCode("@@@", "9126");
        }
        // Reko: a decoder for the instruction 6D29 at address 2307BA46 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_6D29()
        {
            AssertCode("@@@", "6D29");
        }
        // Reko: a decoder for the instruction 9523 at address 2307B99C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_9523()
        {
            AssertCode("@@@", "9523");
        }
        // Reko: a decoder for the instruction 652B at address 2307B828 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_652B()
        {
            AssertCode("@@@", "652B");
        }
        // Reko: a decoder for the instruction F92D at address 2307B762 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_F92D()
        {
            AssertCode("@@@", "F92D");
        }
        // Reko: a decoder for the instruction 652D at address 23064F02 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_652D()
        {
            AssertCode("@@@", "652D");
        }
        // Reko: a decoder for the instruction 94FC at address 23061CF6 has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_94FC()
        {
            AssertCode("@@@", "94FC");
        }
        // Reko: a decoder for the instruction 3FDDC5BF at address 23066BD4 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3FDDC5BF()
        {
            AssertCode("@@@", "3FDDC5BF");
        }
        // Reko: a decoder for the instruction 5FF1B755 at address 23064EB8 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_5FF1B755()
        {
            AssertCode("@@@", "5FF1B755");
        }
        // Reko: a decoder for the instruction 9522 at address 2307B0B4 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_9522()
        {
            AssertCode("@@@", "9522");
        }
        // Reko: a decoder for the instruction AD2A at address 2307B092 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_AD2A()
        {
            AssertCode("@@@", "AD2A");
        }
        // Reko: a decoder for the instruction 6121 at address 2307AC7C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_6121()
        {
            AssertCode("@@@", "6121");
        }
        // Reko: a decoder for the instruction 5D2A at address 2307AEAE has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_5D2A()
        {
            AssertCode("@@@", "5D2A");
        }
        // Reko: a decoder for the instruction 3D2A at address 2307A9A6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3D2A()
        {
            AssertCode("@@@", "3D2A");
        }
        // Reko: a decoder for the instruction 8F00B3C3 at address 23079530 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_8F00B3C3()
        {
            AssertCode("@@@", "8F00B3C3");
        }
        // Reko: a decoder for the instruction BF007A93 at address 23079490 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BF007A93()
        {
            AssertCode("@@@", "BF007A93");
        }
        // Reko: a decoder for the instruction C126 at address 23078E9A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C126()
        {
            AssertCode("@@@", "C126");
        }
        // Reko: a decoder for the instruction 1121 at address 23078E56 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1121()
        {
            AssertCode("@@@", "1121");
        }
        // Reko: a decoder for the instruction 5121 at address 23078DCC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_5121()
        {
            AssertCode("@@@", "5121");
        }
        // Reko: a decoder for the instruction FF8611E9 at address 2306B442 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_FF8611E9()
        {
            AssertCode("@@@", "FF8611E9");
        }
        // Reko: a decoder for the instruction 952A at address 2306B908 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_952A()
        {
            AssertCode("@@@", "952A");
        }
        // Reko: a decoder for the instruction F128 at address 2306B6CA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_F128()
        {
            AssertCode("@@@", "F128");
        }
        // Reko: a decoder for the instruction 3FB66376 at address 230770C0 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3FB66376()
        {
            AssertCode("@@@", "3FB66376");
        }
        // Reko: a decoder for the instruction AFBE2685 at address 23076F2E has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFBE2685()
        {
            AssertCode("@@@", "AFBE2685");
        }
        // Reko: a decoder for the instruction AD3F at address 2307646E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_AD3F()
        {
            AssertCode("@@@", "AD3F");
        }
        // Reko: a decoder for the instruction F7008347 at address 230752F4 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7008347()
        {
            AssertCode("@@@", "F7008347");
        }
        // Reko: a decoder for the instruction BFBC2285 at address 2306DE30 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFBC2285()
        {
            AssertCode("@@@", "BFBC2285");
        }
        // Reko: a decoder for the instruction 3FB92A86 at address 2306D090 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3FB92A86()
        {
            AssertCode("@@@", "3FB92A86");
        }
        // Reko: a decoder for the instruction 1FBB7400 at address 2306CCC8 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_1FBB7400()
        {
            AssertCode("@@@", "1FBB7400");
        }
        // Reko: a decoder for the instruction F371EFF0 at address 2306DB4C has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F371EFF0()
        {
            AssertCode("@@@", "F371EFF0");
        }
        // Reko: a decoder for the instruction D7FA9147 at address 2306D46E has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D7FA9147()
        {
            AssertCode("@@@", "D7FA9147");
        }
        // Reko: a decoder for the instruction F7004183 at address 2306C5DA has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7004183()
        {
            AssertCode("@@@", "F7004183");
        }
        // Reko: a decoder for the instruction F70079BF at address 2306AE3C has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F70079BF()
        {
            AssertCode("@@@", "F70079BF");
        }
        // Reko: a decoder for the instruction F30B2A84 at address 23065D5C has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F30B2A84()
        {
            AssertCode("@@@", "F30B2A84");
        }
        // Reko: a decoder for the instruction 5D20 at address 23065530 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_5D20()
        {
            AssertCode("@@@", "5D20");
        }
        // Reko: a decoder for the instruction 752E at address 23065340 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_752E()
        {
            AssertCode("@@@", "752E");
        }
        // Reko: a decoder for the instruction 6123 at address 23065174 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_6123()
        {
            AssertCode("@@@", "6123");
        }
        // Reko: a decoder for the instruction 73905730 at address 23065130 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73905730()
        {
            AssertCode("@@@", "73905730");
        }
        // Reko: a decoder for the instruction F3271030 at address 23064F74 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3271030()
        {
            AssertCode("@@@", "F3271030");
        }
        // Reko: a decoder for the instruction F3275030 at address 23064256 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3275030()
        {
            AssertCode("@@@", "F3275030");
        }
        // Reko: a decoder for the instruction 73905230 at address 2306460A has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73905230()
        {
            AssertCode("@@@", "73905230");
        }
        // Reko: a decoder for the instruction 73901234 at address 23064488 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73901234()
        {
            AssertCode("@@@", "73901234");
        }
        // Reko: a decoder for the instruction 73900230 at address 230644D8 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73900230()
        {
            AssertCode("@@@", "73900230");
        }
        // Reko: a decoder for the instruction 73903200 at address 230644CE has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73903200()
        {
            AssertCode("@@@", "73903200");
        }
        // Reko: a decoder for the instruction 4123 at address 23060466 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4123()
        {
            AssertCode("@@@", "4123");
        }
        // Reko: a decoder for the instruction E921 at address 23060492 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_E921()
        {
            AssertCode("@@@", "E921");
        }
        // Reko: a decoder for the instruction 752A at address 230607A0 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_752A()
        {
            AssertCode("@@@", "752A");
        }
        // Reko: a decoder for the instruction E520 at address 23060894 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_E520()
        {
            AssertCode("@@@", "E520");
        }
        // Reko: a decoder for the instruction AD28 at address 23060902 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_AD28()
        {
            AssertCode("@@@", "AD28");
        }
        // Reko: a decoder for the instruction C92A at address 23060814 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C92A()
        {
            AssertCode("@@@", "C92A");
        }
        // Reko: a decoder for the instruction A12A at address 2305D14E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_A12A()
        {
            AssertCode("@@@", "A12A");
        }
        // Reko: a decoder for the instruction 9128 at address 2305C2F6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_9128()
        {
            AssertCode("@@@", "9128");
        }
        // Reko: a decoder for the instruction 1122 at address 2305C1CA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1122()
        {
            AssertCode("@@@", "1122");
        }
        // Reko: a decoder for the instruction 8F8F3246 at address 2304DC18 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_8F8F3246()
        {
            AssertCode("@@@", "8F8F3246");
        }
        // Reko: a decoder for the instruction 2527 at address 2305B938 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_2527()
        {
            AssertCode("@@@", "2527");
        }
        // Reko: a decoder for the instruction 4923 at address 23058ED6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4923()
        {
            AssertCode("@@@", "4923");
        }
        // Reko: a decoder for the instruction 1FEB3305 at address 23058FEC has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_1FEB3305()
        {
            AssertCode("@@@", "1FEB3305");
        }
        // Reko: a decoder for the instruction 1480 at address 2305871A has not been implemented. (reserved)
        [Test]
        public void RiscV_dasm_1480()
        {
            AssertCode("@@@", "1480");
        }
        // Reko: a decoder for the instruction 9FC02ACA at address 2305876E has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_9FC02ACA()
        {
            AssertCode("@@@", "9FC02ACA");
        }
        // Reko: a decoder for the instruction 0FF12308 at address 23058308 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_0FF12308()
        {
            AssertCode("@@@", "0FF12308");
        }
        // Reko: a decoder for the instruction 9FF82285 at address 23050FF8 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_9FF82285()
        {
            AssertCode("@@@", "9FF82285");
        }
        // Reko: a decoder for the instruction 3FC4D285 at address 230516B0 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3FC4D285()
        {
            AssertCode("@@@", "3FC4D285");
        }
        // Reko: a decoder for the instruction 9FBEE5FC at address 23057F98 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_9FBEE5FC()
        {
            AssertCode("@@@", "9FBEE5FC");
        }
        // Reko: a decoder for the instruction DFF08346 at address 2304F5CC has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_DFF08346()
        {
            AssertCode("@@@", "DFF08346");
        }
        // Reko: a decoder for the instruction 0F009387 at address 230566D4 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_0F009387()
        {
            AssertCode("@@@", "0F009387");
        }
        // Reko: a decoder for the instruction DFC70145 at address 23053530 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_DFC70145()
        {
            AssertCode("@@@", "DFC70145");
        }
        // Reko: a decoder for the instruction 0FA811C5 at address 23055038 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_0FA811C5()
        {
            AssertCode("@@@", "0FA811C5");
        }
        // Reko: a decoder for the instruction 2D21 at address 23055482 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_2D21()
        {
            AssertCode("@@@", "2D21");
        }
        // Reko: a decoder for the instruction 4534 at address 230524EA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4534()
        {
            AssertCode("@@@", "4534");
        }
        // Reko: a decoder for the instruction 3FCE71BF at address 23055114 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3FCE71BF()
        {
            AssertCode("@@@", "3FCE71BF");
        }
        // Reko: a decoder for the instruction 0FC62244 at address 23054996 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_0FC62244()
        {
            AssertCode("@@@", "0FC62244");
        }
        // Reko: a decoder for the instruction 0B23B285 at address 23053FA4 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B23B285()
        {
            AssertCode("@@@", "0B23B285");
        }
        // Reko: a decoder for the instruction 7F858145 at address 2305261C has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_7F858145()
        {
            AssertCode("@@@", "7F858145");
        }
        // Reko: a decoder for the instruction BFA003D9 at address 2304A7B8 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFA003D9()
        {
            AssertCode("@@@", "BFA003D9");
        }
        // Reko: a decoder for the instruction DF8C03C7 at address 2304FAE4 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_DF8C03C7()
        {
            AssertCode("@@@", "DF8C03C7");
        }
        // Reko: a decoder for the instruction 448A at address 2304C106 has not been implemented. (reserved)
        [Test]
        public void RiscV_dasm_448A()
        {
            AssertCode("@@@", "448A");
        }
        // Reko: a decoder for the instruction F7000327 at address 2304A488 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7000327()
        {
            AssertCode("@@@", "F7000327");
        }
        // Reko: a decoder for the instruction 8926 at address 230489E8 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_8926()
        {
            AssertCode("@@@", "8926");
        }
        // Reko: a decoder for the instruction 2FCB2286 at address 23042B8C has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_2FCB2286()
        {
            AssertCode("@@@", "2FCB2286");
        }
        // Reko: a decoder for the instruction 5FDE1DA8 at address 230451BE has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_5FDE1DA8()
        {
            AssertCode("@@@", "5FDE1DA8");
        }
        // Reko: a decoder for the instruction F70403D7 at address 2303D1DE has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F70403D7()
        {
            AssertCode("@@@", "F70403D7");
        }
        // Reko: a decoder for the instruction FF890145 at address 23045490 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_FF890145()
        {
            AssertCode("@@@", "FF890145");
        }
        // Reko: a decoder for the instruction 5FE96DF1 at address 2303C5E6 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_5FE96DF1()
        {
            AssertCode("@@@", "5FE96DF1");
        }
        // Reko: a decoder for the instruction 8B419847 at address 2303C542 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_8B419847()
        {
            AssertCode("@@@", "8B419847");
        }
        // Reko: a decoder for the instruction C531 at address 2304636E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C531()
        {
            AssertCode("@@@", "C531");
        }
        // Reko: a decoder for the instruction 04FE at address 23046B7E has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_04FE()
        {
            AssertCode("@@@", "04FE");
        }
        // Reko: a decoder for the instruction 0B23DA85 at address 23046F26 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B23DA85()
        {
            AssertCode("@@@", "0B23DA85");
        }
        // Reko: a decoder for the instruction F4FF at address 23046198 has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_F4FF()
        {
            AssertCode("@@@", "F4FF");
        }
        // Reko: a decoder for the instruction F7018347 at address 23046532 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7018347()
        {
            AssertCode("@@@", "F7018347");
        }
        // Reko: a decoder for the instruction FB023314 at address 23046190 has not been implemented. (custom-3)
        [Test]
        public void RiscV_dasm_FB023314()
        {
            AssertCode("@@@", "FB023314");
        }
        // Reko: a decoder for the instruction F7023775 at address 2303BCDA has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7023775()
        {
            AssertCode("@@@", "F7023775");
        }
        // Reko: a decoder for the instruction C92B at address 2304889A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C92B()
        {
            AssertCode("@@@", "C92B");
        }
        // Reko: a decoder for the instruction ED21 at address 2303CC8E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_ED21()
        {
            AssertCode("@@@", "ED21");
        }
        // Reko: a decoder for the instruction BFFB2955 at address 230447CC has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFFB2955()
        {
            AssertCode("@@@", "BFFB2955");
        }
        // Reko: a decoder for the instruction 713F at address 230452D4 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_713F()
        {
            AssertCode("@@@", "713F");
        }
        // Reko: a decoder for the instruction F7F837E4 at address 2303F482 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7F837E4()
        {
            AssertCode("@@@", "F7F837E4");
        }
        // Reko: a decoder for the instruction 77FFB386 at address 23040886 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_77FFB386()
        {
            AssertCode("@@@", "77FFB386");
        }
        // Reko: a decoder for the instruction 30FB at address 2304308A has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_30FB()
        {
            AssertCode("@@@", "30FB");
        }
        // Reko: a decoder for the instruction F703F517 at address 23045146 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F703F517()
        {
            AssertCode("@@@", "F703F517");
        }
        // Reko: a decoder for the instruction F700EFF0 at address 23044C84 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F700EFF0()
        {
            AssertCode("@@@", "F700EFF0");
        }
        // Reko: a decoder for the instruction 1FF98347 at address 23044B0E has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_1FF98347()
        {
            AssertCode("@@@", "1FF98347");
        }
        // Reko: a decoder for the instruction F7028347 at address 230445F6 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7028347()
        {
            AssertCode("@@@", "F7028347");
        }
        // Reko: a decoder for the instruction AFE7B1BF at address 2303F13A has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFE7B1BF()
        {
            AssertCode("@@@", "AFE7B1BF");
        }
        // Reko: a decoder for the instruction FFBFAA84 at address 2303ECA8 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_FFBFAA84()
        {
            AssertCode("@@@", "FFBFAA84");
        }
        // Reko: a decoder for the instruction ED2D at address 2303BFA6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_ED2D()
        {
            AssertCode("@@@", "ED2D");
        }
        // Reko: a decoder for the instruction 732A0030 at address 2302E0C2 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_732A0030()
        {
            AssertCode("@@@", "732A0030");
        }
        // Reko: a decoder for the instruction 9529 at address 2302B5E6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_9529()
        {
            AssertCode("@@@", "9529");
        }
        // Reko: a decoder for the instruction 3524 at address 2302B60C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3524()
        {
            AssertCode("@@@", "3524");
        }
        // Reko: a decoder for the instruction 04FD at address 2302FDF4 has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_04FD()
        {
            AssertCode("@@@", "04FD");
        }
        // Reko: a decoder for the instruction 73260030 at address 23030120 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73260030()
        {
            AssertCode("@@@", "73260030");
        }
        // Reko: a decoder for the instruction AFDF01CD at address 23033F96 has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFDF01CD()
        {
            AssertCode("@@@", "AFDF01CD");
        }
        // Reko: a decoder for the instruction F3770430 at address 23029C7C has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3770430()
        {
            AssertCode("@@@", "F3770430");
        }
        // Reko: a decoder for the instruction D702E34A at address 23029C34 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D702E34A()
        {
            AssertCode("@@@", "D702E34A");
        }
        // Reko: a decoder for the instruction 0BFA8507 at address 23029C4E has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0BFA8507()
        {
            AssertCode("@@@", "0BFA8507");
        }
        // Reko: a decoder for the instruction 73100B30 at address 2302DEDC has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73100B30()
        {
            AssertCode("@@@", "73100B30");
        }
        // Reko: a decoder for the instruction BD23 at address 230324E6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_BD23()
        {
            AssertCode("@@@", "BD23");
        }
        // Reko: a decoder for the instruction 73290030 at address 2303366A has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73290030()
        {
            AssertCode("@@@", "73290030");
        }
        // Reko: a decoder for the instruction 73900430 at address 230396D6 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73900430()
        {
            AssertCode("@@@", "73900430");
        }
        // Reko: a decoder for the instruction F3260030 at address 23038D64 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3260030()
        {
            AssertCode("@@@", "F3260030");
        }
        // Reko: a decoder for the instruction 2D2E at address 23038524 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_2D2E()
        {
            AssertCode("@@@", "2D2E");
        }
        // Reko: a decoder for the instruction DFCFF240 at address 23038080 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_DFCFF240()
        {
            AssertCode("@@@", "DFCFF240");
        }
        // Reko: a decoder for the instruction A124 at address 23037F76 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_A124()
        {
            AssertCode("@@@", "A124");
        }
        // Reko: a decoder for the instruction 5526 at address 23037E1A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_5526()
        {
            AssertCode("@@@", "5526");
        }
        // Reko: a decoder for the instruction F70F230D at address 23037DD6 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F70F230D()
        {
            AssertCode("@@@", "F70F230D");
        }
        // Reko: a decoder for the instruction C92D at address 23037B9E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C92D()
        {
            AssertCode("@@@", "C92D");
        }
        // Reko: a decoder for the instruction F70F6360 at address 23038026 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F70F6360()
        {
            AssertCode("@@@", "F70F6360");
        }
        // Reko: a decoder for the instruction 732B0030 at address 2303740A has not been implemented. (system)
        [Test]
        public void RiscV_dasm_732B0030()
        {
            AssertCode("@@@", "732B0030");
        }
        // Reko: a decoder for the instruction 7F8F9147 at address 23036A7E has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_7F8F9147()
        {
            AssertCode("@@@", "7F8F9147");
        }
        // Reko: a decoder for the instruction F7F8A685 at address 23035C66 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7F8A685()
        {
            AssertCode("@@@", "F7F8A685");
        }
        // Reko: a decoder for the instruction 192D at address 23032E8E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_192D()
        {
            AssertCode("@@@", "192D");
        }
        // Reko: a decoder for the instruction 9D28 at address 23032EFE has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_9D28()
        {
            AssertCode("@@@", "9D28");
        }
        // Reko: a decoder for the instruction 112C at address 23032F08 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_112C()
        {
            AssertCode("@@@", "112C");
        }
        // Reko: a decoder for the instruction D702E297 at address 23030A12 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D702E297()
        {
            AssertCode("@@@", "D702E297");
        }
        // Reko: a decoder for the instruction AFFF2A84 at address 23030ACE has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFFF2A84()
        {
            AssertCode("@@@", "AFFF2A84");
        }
        // Reko: a decoder for the instruction CC89 at address 2303084A has not been implemented. (reserved)
        [Test]
        public void RiscV_dasm_CC89()
        {
            AssertCode("@@@", "CC89");
        }
        // Reko: a decoder for the instruction F3270030 at address 2302FF6C has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3270030()
        {
            AssertCode("@@@", "F3270030");
        }
        // Reko: a decoder for the instruction 093D at address 2302FBB8 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_093D()
        {
            AssertCode("@@@", "093D");
        }
        // Reko: a decoder for the instruction DFF315C1 at address 2302BFBC has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_DFF315C1()
        {
            AssertCode("@@@", "DFF315C1");
        }
        // Reko: a decoder for the instruction E52B at address 23029D72 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_E52B()
        {
            AssertCode("@@@", "E52B");
        }
        // Reko: a decoder for the instruction 6523 at address 23029DC2 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_6523()
        {
            AssertCode("@@@", "6523");
        }
        // Reko: a decoder for the instruction 6933 at address 230014AA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_6933()
        {
            AssertCode("@@@", "6933");
        }
        // Reko: a decoder for the instruction 3D29 at address 23023E0E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3D29()
        {
            AssertCode("@@@", "3D29");
        }
        // Reko: a decoder for the instruction F3471C40 at address 23028D28 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3471C40()
        {
            AssertCode("@@@", "F3471C40");
        }
        // Reko: a decoder for the instruction 2121 at address 23027744 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_2121()
        {
            AssertCode("@@@", "2121");
        }
        // Reko: a decoder for the instruction F7EF2319 at address 23012E5E has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7EF2319()
        {
            AssertCode("@@@", "F7EF2319");
        }
        // Reko: a decoder for the instruction 093F at address 2300E55A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_093F()
        {
            AssertCode("@@@", "093F");
        }
        // Reko: a decoder for the instruction D7120D46 at address 2300F240 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D7120D46()
        {
            AssertCode("@@@", "D7120D46");
        }
        // Reko: a decoder for the instruction F70F0A07 at address 23020C86 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F70F0A07()
        {
            AssertCode("@@@", "F70F0A07");
        }
        // Reko: a decoder for the instruction F7001843 at address 2302156E has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7001843()
        {
            AssertCode("@@@", "F7001843");
        }
        // Reko: a decoder for the instruction BFF42146 at address 2301F866 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFF42146()
        {
            AssertCode("@@@", "BFF42146");
        }
        // Reko: a decoder for the instruction 3D23 at address 23018E3C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3D23()
        {
            AssertCode("@@@", "3D23");
        }
        // Reko: a decoder for the instruction 8BFBFD57 at address 23014116 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_8BFBFD57()
        {
            AssertCode("@@@", "8BFBFD57");
        }
        // Reko: a decoder for the instruction C8E7 at address 23002778 has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_C8E7()
        {
            AssertCode("@@@", "C8E7");
        }
        // Reko: a decoder for the instruction D70083A7 at address 2301056E has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D70083A7()
        {
            AssertCode("@@@", "D70083A7");
        }
        // Reko: a decoder for the instruction D523 at address 23018D86 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_D523()
        {
            AssertCode("@@@", "D523");
        }
        // Reko: a decoder for the instruction D7028946 at address 2300CD14 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D7028946()
        {
            AssertCode("@@@", "D7028946");
        }
        // Reko: a decoder for the instruction F700EFE0 at address 2300D254 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F700EFE0()
        {
            AssertCode("@@@", "F700EFE0");
        }
        // Reko: a decoder for the instruction AFC39307 at address 2300D26C has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AFC39307()
        {
            AssertCode("@@@", "AFC39307");
        }
        // Reko: a decoder for the instruction F700B697 at address 2300CEDC has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F700B697()
        {
            AssertCode("@@@", "F700B697");
        }
        // Reko: a decoder for the instruction 8129 at address 23018B44 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_8129()
        {
            AssertCode("@@@", "8129");
        }
        // Reko: a decoder for the instruction 2923 at address 23018A8A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_2923()
        {
            AssertCode("@@@", "2923");
        }
        // Reko: a decoder for the instruction 0D25 at address 23002590 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0D25()
        {
            AssertCode("@@@", "0D25");
        }
        // Reko: a decoder for the instruction 4526 at address 23017C0C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4526()
        {
            AssertCode("@@@", "4526");
        }
        // Reko: a decoder for the instruction 8123 at address 23017526 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_8123()
        {
            AssertCode("@@@", "8123");
        }
        // Reko: a decoder for the instruction 6526 at address 23017B3E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_6526()
        {
            AssertCode("@@@", "6526");
        }
        // Reko: a decoder for the instruction 892D at address 230173FE has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_892D()
        {
            AssertCode("@@@", "892D");
        }
        // Reko: a decoder for the instruction 4525 at address 23015C7A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4525()
        {
            AssertCode("@@@", "4525");
        }
        // Reko: a decoder for the instruction F93E at address 230110EA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_F93E()
        {
            AssertCode("@@@", "F93E");
        }
        // Reko: a decoder for the instruction 2F9B8327 at address 230105C4 has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_2F9B8327()
        {
            AssertCode("@@@", "2F9B8327");
        }
        // Reko: a decoder for the instruction C520 at address 2300A47C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C520()
        {
            AssertCode("@@@", "C520");
        }
        // Reko: a decoder for the instruction F7020347 at address 2301016E has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7020347()
        {
            AssertCode("@@@", "F7020347");
        }
        // Reko: a decoder for the instruction AF8A9307 at address 2300FDF4 has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_AF8A9307()
        {
            AssertCode("@@@", "AF8A9307");
        }
        // Reko: a decoder for the instruction 1F9A1335 at address 2302525C has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_1F9A1335()
        {
            AssertCode("@@@", "1F9A1335");
        }
        // Reko: a decoder for the instruction 8523 at address 2302217E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_8523()
        {
            AssertCode("@@@", "8523");
        }
        // Reko: a decoder for the instruction 092D at address 23018D58 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_092D()
        {
            AssertCode("@@@", "092D");
        }
        // Reko: a decoder for the instruction C52A at address 230179C4 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C52A()
        {
            AssertCode("@@@", "C52A");
        }
        // Reko: a decoder for the instruction D0FD at address 2301C81E has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_D0FD()
        {
            AssertCode("@@@", "D0FD");
        }
        // Reko: a decoder for the instruction 8F810346 at address 2301B98C has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_8F810346()
        {
            AssertCode("@@@", "8F810346");
        }
        // Reko: a decoder for the instruction 9D3B at address 2300219C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_9D3B()
        {
            AssertCode("@@@", "9D3B");
        }
        // Reko: a decoder for the instruction 1525 at address 23017A28 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1525()
        {
            AssertCode("@@@", "1525");
        }
        // Reko: a decoder for the instruction 7D3F at address 23003128 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_7D3F()
        {
            AssertCode("@@@", "7D3F");
        }
        // Reko: a decoder for the instruction 7925 at address 23018CDC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_7925()
        {
            AssertCode("@@@", "7925");
        }
        // Reko: a decoder for the instruction 8526 at address 23017EBE has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_8526()
        {
            AssertCode("@@@", "8526");
        }
        // Reko: a decoder for the instruction 3D20 at address 230147E8 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3D20()
        {
            AssertCode("@@@", "3D20");
        }
        // Reko: a decoder for the instruction 2FF79305 at address 23013004 has not been implemented. (amo)
        [Test]
        public void RiscV_dasm_2FF79305()
        {
            AssertCode("@@@", "2FF79305");
        }
        // Reko: a decoder for the instruction 4D23 at address 2300C1DC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4D23()
        {
            AssertCode("@@@", "4D23");
        }
        // Reko: a decoder for the instruction 570184C3 at address 2300A9E6 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_570184C3()
        {
            AssertCode("@@@", "570184C3");
        }
        // Reko: a decoder for the instruction 2D22 at address 2300A788 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_2D22()
        {
            AssertCode("@@@", "2D22");
        }
        // Reko: a decoder for the instruction C12D at address 2300A1E2 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C12D()
        {
            AssertCode("@@@", "C12D");
        }
        // Reko: a decoder for the instruction F7003706 at address 23007E38 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7003706()
        {
            AssertCode("@@@", "F7003706");
        }
        // Reko: a decoder for the instruction F7FD2326 at address 230079DA has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7FD2326()
        {
            AssertCode("@@@", "F7FD2326");
        }
        // Reko: a decoder for the instruction 4498 at address 23007E80 has not been implemented. (reserved)
        [Test]
        public void RiscV_dasm_4498()
        {
            AssertCode("@@@", "4498");
        }
        // Reko: a decoder for the instruction FFC20D45 at address 23006424 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_FFC20D45()
        {
            AssertCode("@@@", "FFC20D45");
        }
        // Reko: a decoder for the instruction F937 at address 23002DA2 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_F937()
        {
            AssertCode("@@@", "F937");
        }
        // Reko: a decoder for the instruction 993F at address 23001FBC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_993F()
        {
            AssertCode("@@@", "993F");
        }
        // Reko: a decoder for the instruction 3522 at address 2300110C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3522()
        {
            AssertCode("@@@", "3522");
        }
        // Reko: a decoder for the instruction 9D37 at address 23000A80 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_9D37()
        {
            AssertCode("@@@", "9D37");
        }
        // Reko: a decoder for the instruction 1D3F at address 23000AB0 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1D3F()
        {
            AssertCode("@@@", "1D3F");
        }
        // Reko: a decoder for the instruction 5D26 at address 23000B50 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_5D26()
        {
            AssertCode("@@@", "5D26");
        }
        // Reko: a decoder for the instruction F539 at address 23000776 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_F539()
        {
            AssertCode("@@@", "F539");
        }
        // Reko: a decoder for the instruction 3D33 at address 23000744 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3D33()
        {
            AssertCode("@@@", "3D33");
        }
        // Reko: a decoder for the instruction F7001145 at address 230007F4 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7001145()
        {
            AssertCode("@@@", "F7001145");
        }
        // Reko: a decoder for the instruction 73A00230 at address 230000D4 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73A00230()
        {
            AssertCode("@@@", "73A00230");
        }
        // Reko: a decoder for the instruction 73230030 at address 230000D8 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73230030()
        {
            AssertCode("@@@", "73230030");
        }
        // Reko: a decoder for the instruction 73103000 at address 230000E4 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73103000()
        {
            AssertCode("@@@", "73103000");
        }
        // Reko: a decoder for the instruction 7D2E at address 23000B5E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_7D2E()
        {
            AssertCode("@@@", "7D2E");
        }
        // Reko: a decoder for the instruction 0D2A at address 2300110E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0D2A()
        {
            AssertCode("@@@", "0D2A");
        }
        // Reko: a decoder for the instruction D128 at address 23001406 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_D128()
        {
            AssertCode("@@@", "D128");
        }
        // Reko: a decoder for the instruction 253D at address 23001476 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_253D()
        {
            AssertCode("@@@", "253D");
        }
        // Reko: a decoder for the instruction 1537 at address 2300224C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1537()
        {
            AssertCode("@@@", "1537");
        }
        // Reko: a decoder for the instruction 013D at address 23002360 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_013D()
        {
            AssertCode("@@@", "013D");
        }
        // Reko: a decoder for the instruction 8D25 at address 23002514 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_8D25()
        {
            AssertCode("@@@", "8D25");
        }
        // Reko: a decoder for the instruction A52A at address 23002516 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_A52A()
        {
            AssertCode("@@@", "A52A");
        }
        // Reko: a decoder for the instruction 1D25 at address 23002596 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1D25()
        {
            AssertCode("@@@", "1D25");
        }
        // Reko: a decoder for the instruction B122 at address 23002936 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_B122()
        {
            AssertCode("@@@", "B122");
        }
        // Reko: a decoder for the instruction 3D22 at address 23002954 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3D22()
        {
            AssertCode("@@@", "3D22");
        }
        // Reko: a decoder for the instruction 1522 at address 2300295E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1522()
        {
            AssertCode("@@@", "1522");
        }
        // Reko: a decoder for the instruction 4D3F at address 23003134 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4D3F()
        {
            AssertCode("@@@", "4D3F");
        }
        // Reko: a decoder for the instruction F708B7F7 at address 230079DE has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F708B7F7()
        {
            AssertCode("@@@", "F708B7F7");
        }
        // Reko: a decoder for the instruction 9F891305 at address 2300D258 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_9F891305()
        {
            AssertCode("@@@", "9F891305");
        }
        // Reko: a decoder for the instruction 0521 at address 23017B8C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0521()
        {
            AssertCode("@@@", "0521");
        }
        // Reko: a decoder for the instruction 912E at address 23017B92 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_912E()
        {
            AssertCode("@@@", "912E");
        }
        // Reko: a decoder for the instruction 7923 at address 23017BB0 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_7923()
        {
            AssertCode("@@@", "7923");
        }
        // Reko: a decoder for the instruction D12C at address 23017C12 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_D12C()
        {
            AssertCode("@@@", "D12C");
        }
        // Reko: a decoder for the instruction 312B at address 23017C22 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_312B()
        {
            AssertCode("@@@", "312B");
        }
        // Reko: a decoder for the instruction 952D at address 23018CE2 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_952D()
        {
            AssertCode("@@@", "952D");
        }
        // Reko: a decoder for the instruction A12B at address 23018D0A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_A12B()
        {
            AssertCode("@@@", "A12B");
        }
        // Reko: a decoder for the instruction 7521 at address 23018E5A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_7521()
        {
            AssertCode("@@@", "7521");
        }
        // Reko: a decoder for the instruction 0D20 at address 230221FC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0D20()
        {
            AssertCode("@@@", "0D20");
        }
        // Reko: a decoder for the instruction 1123 at address 2302220E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1123()
        {
            AssertCode("@@@", "1123");
        }
        // Reko: a decoder for the instruction C923 at address 23022214 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C923()
        {
            AssertCode("@@@", "C923");
        }
        // Reko: a decoder for the instruction 73100430 at address 23029286 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73100430()
        {
            AssertCode("@@@", "73100430");
        }
        // Reko: a decoder for the instruction F7E683A7 at address 23029C38 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7E683A7()
        {
            AssertCode("@@@", "F7E683A7");
        }
        // Reko: a decoder for the instruction 73100930 at address 23029C84 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73100930()
        {
            AssertCode("@@@", "73100930");
        }
        // Reko: a decoder for the instruction 5D2D at address 23029CB4 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_5D2D()
        {
            AssertCode("@@@", "5D2D");
        }
        // Reko: a decoder for the instruction F925 at address 23029CD0 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_F925()
        {
            AssertCode("@@@", "F925");
        }
        // Reko: a decoder for the instruction 5923 at address 23029D78 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_5923()
        {
            AssertCode("@@@", "5923");
        }
        // Reko: a decoder for the instruction A4FF at address 23029FD4 has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_A4FF()
        {
            AssertCode("@@@", "A4FF");
        }
        // Reko: a decoder for the instruction 73100730 at address 2302A238 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73100730()
        {
            AssertCode("@@@", "73100730");
        }
        // Reko: a decoder for the instruction F3750430 at address 2302BA1A has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3750430()
        {
            AssertCode("@@@", "F3750430");
        }
        // Reko: a decoder for the instruction 73100630 at address 2302BA4A has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73100630()
        {
            AssertCode("@@@", "73100630");
        }
        // Reko: a decoder for the instruction 73100830 at address 2302BB36 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73100830()
        {
            AssertCode("@@@", "73100830");
        }
        // Reko: a decoder for the instruction 73900A30 at address 2302BBDA has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73900A30()
        {
            AssertCode("@@@", "73900A30");
        }
        // Reko: a decoder for the instruction 73100A30 at address 2302E0F0 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73100A30()
        {
            AssertCode("@@@", "73100A30");
        }
        // Reko: a decoder for the instruction 73770430 at address 2302E238 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73770430()
        {
            AssertCode("@@@", "73770430");
        }
        // Reko: a decoder for the instruction 73900730 at address 2302E23C has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73900730()
        {
            AssertCode("@@@", "73900730");
        }
        // Reko: a decoder for the instruction F32D0030 at address 23030024 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F32D0030()
        {
            AssertCode("@@@", "F32D0030");
        }
        // Reko: a decoder for the instruction 73900D30 at address 23030048 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73900D30()
        {
            AssertCode("@@@", "73900D30");
        }
        // Reko: a decoder for the instruction F32B0030 at address 230300E0 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F32B0030()
        {
            AssertCode("@@@", "F32B0030");
        }
        // Reko: a decoder for the instruction 73900B30 at address 23030102 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73900B30()
        {
            AssertCode("@@@", "73900B30");
        }
        // Reko: a decoder for the instruction F7023767 at address 2303802A has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F7023767()
        {
            AssertCode("@@@", "F7023767");
        }
        // Reko: a decoder for the instruction 0B238A07 at address 2303802E has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B238A07()
        {
            AssertCode("@@@", "0B238A07");
        }
        // Reko: a decoder for the instruction 4539 at address 2303BD1A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4539()
        {
            AssertCode("@@@", "4539");
        }
        // Reko: a decoder for the instruction 9127 at address 2303BFBC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_9127()
        {
            AssertCode("@@@", "9127");
        }
        // Reko: a decoder for the instruction D7409008 at address 2304088A has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D7409008()
        {
            AssertCode("@@@", "D7409008");
        }
        // Reko: a decoder for the instruction 3FBE9307 at address 23044C88 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3FBE9307()
        {
            AssertCode("@@@", "3FBE9307");
        }
        // Reko: a decoder for the instruction F70037A5 at address 23045618 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F70037A5()
        {
            AssertCode("@@@", "F70037A5");
        }
        // Reko: a decoder for the instruction 0524 at address 23049A62 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0524()
        {
            AssertCode("@@@", "0524");
        }
        // Reko: a decoder for the instruction D92A at address 23049AAC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_D92A()
        {
            AssertCode("@@@", "D92A");
        }
        // Reko: a decoder for the instruction 24FC at address 23051D00 has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_24FC()
        {
            AssertCode("@@@", "24FC");
        }
        // Reko: a decoder for the instruction D7000145 at address 23052BDC has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D7000145()
        {
            AssertCode("@@@", "D7000145");
        }
        // Reko: a decoder for the instruction 8527 at address 23055E3A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_8527()
        {
            AssertCode("@@@", "8527");
        }
        // Reko: a decoder for the instruction F723AA85 at address 230566D8 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_F723AA85()
        {
            AssertCode("@@@", "F723AA85");
        }
        // Reko: a decoder for the instruction 1FE48547 at address 230568D8 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_1FE48547()
        {
            AssertCode("@@@", "1FE48547");
        }
        // Reko: a decoder for the instruction 2D23 at address 2305903C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_2D23()
        {
            AssertCode("@@@", "2D23");
        }
        // Reko: a decoder for the instruction 5134 at address 2305A6C6 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_5134()
        {
            AssertCode("@@@", "5134");
        }
        // Reko: a decoder for the instruction 452A at address 2305AF8E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_452A()
        {
            AssertCode("@@@", "452A");
        }
        // Reko: a decoder for the instruction 0B238347 at address 2305B088 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B238347()
        {
            AssertCode("@@@", "0B238347");
        }
        // Reko: a decoder for the instruction BFEB1C40 at address 2305DE76 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_BFEB1C40()
        {
            AssertCode("@@@", "BFEB1C40");
        }
        // Reko: a decoder for the instruction 3523 at address 230603EC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3523()
        {
            AssertCode("@@@", "3523");
        }
        // Reko: a decoder for the instruction B52B at address 230603EE has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_B52B()
        {
            AssertCode("@@@", "B52B");
        }
        // Reko: a decoder for the instruction 1926 at address 23060664 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1926()
        {
            AssertCode("@@@", "1926");
        }
        // Reko: a decoder for the instruction 152C at address 23060736 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_152C()
        {
            AssertCode("@@@", "152C");
        }
        // Reko: a decoder for the instruction FD24 at address 230607BC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_FD24()
        {
            AssertCode("@@@", "FD24");
        }
        // Reko: a decoder for the instruction F12C at address 230607CE has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_F12C()
        {
            AssertCode("@@@", "F12C");
        }
        // Reko: a decoder for the instruction 692A at address 230607D0 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_692A()
        {
            AssertCode("@@@", "692A");
        }
        // Reko: a decoder for the instruction 3F95EFE0 at address 23064044 has not been implemented. (64-bit instruction)
        [Test]
        public void RiscV_dasm_3F95EFE0()
        {
            AssertCode("@@@", "3F95EFE0");
        }
        // Reko: a decoder for the instruction FF959C40 at address 23064048 has not been implemented. (>= 80-bit instruction)
        [Test]
        public void RiscV_dasm_FF959C40()
        {
            AssertCode("@@@", "FF959C40");
        }
        // Reko: a decoder for the instruction 73A04730 at address 23064272 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73A04730()
        {
            AssertCode("@@@", "73A04730");
        }
        // Reko: a decoder for the instruction F3223000 at address 23064386 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3223000()
        {
            AssertCode("@@@", "F3223000");
        }
        // Reko: a decoder for the instruction 73252034 at address 230643AA has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73252034()
        {
            AssertCode("@@@", "73252034");
        }
        // Reko: a decoder for the instruction F3251034 at address 230643BA has not been implemented. (system)
        [Test]
        public void RiscV_dasm_F3251034()
        {
            AssertCode("@@@", "F3251034");
        }
        // Reko: a decoder for the instruction 8D20 at address 23065546 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_8D20()
        {
            AssertCode("@@@", "8D20");
        }
        // Reko: a decoder for the instruction 0D2C at address 23069A62 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0D2C()
        {
            AssertCode("@@@", "0D2C");
        }
        // Reko: a decoder for the instruction 8F9EAA84 at address 2306DB50 has not been implemented. (misc-mem)
        [Test]
        public void RiscV_dasm_8F9EAA84()
        {
            AssertCode("@@@", "8F9EAA84");
        }
        // Reko: a decoder for the instruction 0B002319 at address 2306E1F8 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B002319()
        {
            AssertCode("@@@", "0B002319");
        }
        // Reko: a decoder for the instruction 0B006381 at address 2306E1FC has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B006381()
        {
            AssertCode("@@@", "0B006381");
        }
        // Reko: a decoder for the instruction 0B020567 at address 2306E200 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B020567()
        {
            AssertCode("@@@", "0B020567");
        }
        // Reko: a decoder for the instruction D7001377 at address 23071938 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D7001377()
        {
            AssertCode("@@@", "D7001377");
        }
        // Reko: a decoder for the instruction DFE52205 at address 23071EF2 has not been implemented. (48-bit instruction)
        [Test]
        public void RiscV_dasm_DFE52205()
        {
            AssertCode("@@@", "DFE52205");
        }
        // Reko: a decoder for the instruction D7000347 at address 23076A88 has not been implemented. (Reserved)
        [Test]
        public void RiscV_dasm_D7000347()
        {
            AssertCode("@@@", "D7000347");
        }
        // Reko: a decoder for the instruction 40FC at address 23077988 has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_40FC()
        {
            AssertCode("@@@", "40FC");
        }
        // Reko: a decoder for the instruction FD21 at address 23078DD8 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_FD21()
        {
            AssertCode("@@@", "FD21");
        }
        // Reko: a decoder for the instruction F926 at address 23078DDA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_F926()
        {
            AssertCode("@@@", "F926");
        }
        // Reko: a decoder for the instruction 292A at address 2307A9CA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_292A()
        {
            AssertCode("@@@", "292A");
        }
        // Reko: a decoder for the instruction 0526 at address 2307AD8A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0526()
        {
            AssertCode("@@@", "0526");
        }
        // Reko: a decoder for the instruction 4D2A at address 2307AEF8 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_4D2A()
        {
            AssertCode("@@@", "4D2A");
        }
        // Reko: a decoder for the instruction 1922 at address 2307AF5E has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_1922()
        {
            AssertCode("@@@", "1922");
        }
        // Reko: a decoder for the instruction F125 at address 2307B78C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_F125()
        {
            AssertCode("@@@", "F125");
        }
        // Reko: a decoder for the instruction 552B at address 2307B7EC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_552B()
        {
            AssertCode("@@@", "552B");
        }
        // Reko: a decoder for the instruction 0123 at address 2307B83C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0123()
        {
            AssertCode("@@@", "0123");
        }
        // Reko: a decoder for the instruction 352B at address 2307B864 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_352B()
        {
            AssertCode("@@@", "352B");
        }
        // Reko: a decoder for the instruction 3D26 at address 2307B918 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3D26()
        {
            AssertCode("@@@", "3D26");
        }
        // Reko: a decoder for the instruction 0D2E at address 2307BB6A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0D2E()
        {
            AssertCode("@@@", "0D2E");
        }
        // Reko: a decoder for the instruction 692C at address 2307BBF8 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_692C()
        {
            AssertCode("@@@", "692C");
        }
        // Reko: a decoder for the instruction 0B009397 at address 23082E70 has not been implemented. (custom-0)
        [Test]
        public void RiscV_dasm_0B009397()
        {
            AssertCode("@@@", "0B009397");
        }
        // Reko: a decoder for the instruction 73D02234 at address 230853D6 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73D02234()
        {
            AssertCode("@@@", "73D02234");
        }
        // Reko: a decoder for the instruction 3924 at address 230861BE has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_3924()
        {
            AssertCode("@@@", "3924");
        }
        // Reko: a decoder for the instruction A0F8 at address 23092BDE has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_A0F8()
        {
            AssertCode("@@@", "A0F8");
        }
        // Reko: a decoder for the instruction A129 at address 23096C1A has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_A129()
        {
            AssertCode("@@@", "A129");
        }
        // Reko: a decoder for the instruction 0121 at address 23096C4C has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0121()
        {
            AssertCode("@@@", "0121");
        }
        // Reko: a decoder for the instruction D126 at address 23096CAE has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_D126()
        {
            AssertCode("@@@", "D126");
        }
        // Reko: a decoder for the instruction B92E at address 23096CEE has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_B92E()
        {
            AssertCode("@@@", "B92E");
        }
        // Reko: a decoder for the instruction 73272000 at address 230A31C4 has not been implemented. (system)
        [Test]
        public void RiscV_dasm_73272000()
        {
            AssertCode("@@@", "73272000");
        }
        // Reko: a decoder for the instruction 6926 at address 4200CA38 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_6926()
        {
            AssertCode("@@@", "6926");
        }
        // Reko: a decoder for the instruction 0D24 at address 4200CBA0 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_0D24()
        {
            AssertCode("@@@", "0D24");
        }
        // Reko: a decoder for the instruction 48E0 at address 4200CC0E has not been implemented. (fsw)
        [Test]
        public void RiscV_dasm_48E0()
        {
            AssertCode("@@@", "48E0");
        }
        // Reko: a decoder for the instruction D530 at address 4200CC72 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_D530()
        {
            AssertCode("@@@", "D530");
        }
        // Reko: a decoder for the instruction DD38 at address 4200CC74 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_DD38()
        {
            AssertCode("@@@", "DD38");
        }
        // Reko: a decoder for the instruction 7D30 at address 4200CCA8 has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_7D30()
        {
            AssertCode("@@@", "7D30");
        }
        // Reko: a decoder for the instruction C130 at address 4200CCAA has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_C130()
        {
            AssertCode("@@@", "C130");
        }
        // Reko: a decoder for the instruction BD38 at address 4200CCEC has not been implemented. (c.jal)
        [Test]
        public void RiscV_dasm_BD38()
        {
            AssertCode("@@@", "BD38");
        }

    }
}
