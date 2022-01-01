#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Arch.Avr;
using Reko.Arch.Avr.Avr32;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Avr
{
    [TestFixture]
    public class Avr32DisassemblerTests : DisassemblerTestBase<Avr32Instruction>
    {
        private readonly Avr32Architecture arch;

        public Avr32DisassemblerTests()
        {
            this.arch = new Avr32Architecture(CreateServiceContainer(), "avr32", new Dictionary<string, object>());
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress { get; }

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void Avr32Dis_asr2()
        {
            AssertCode("asr\tr8,+00000001", "A158");
        }

        [Test]
        public void Avr32Dis_asr3()
        {
            AssertCode("asr\tr8,r8,r9", "F0090848");
        }

        [Test]
        public void Avr32Dis_cbr()
        {
            AssertCode("cbr\tr8,+00000001", "A1D8");
        }

        [Test]
        public void Avr32Dis_mov_r_imm()
        {
            AssertCode("mov\tr7,00000000", "3007");
        }

        [Test]
        public void Avr32Dis_ld_w_post()
        {
            AssertCode("ld.w\tr11,sp++", "1B0B");
        }

        [Test]
        public void Avr32Dis_mov_tworegs()
        {
            AssertCode("mov\tr10,sp", "1A9A");
        }

        [Test]
        public void Avr32Dis_movne()
        {
            AssertCode("movne\tr12,r8", "F00C1710");
        }

        [Test]
        public void Avr32Dis_mulu_d()
        {
            AssertCode("mulu.d\tr4,r11,r8", "F6080644");
        }

        [Test]
        public void Avr32Dis_st_w_pre()
        {
            AssertCode("st.w\t--sp,r10", "1ADA");
        }

        [Test]
        public void Avr32Dis_st_w_idx()
        {
            AssertCode("st.w\tr11[r10<<2],r8", "F60A0928");
        }

        [Test]
        public void Avr32Dis_lddpc()
        {
            AssertCode("lddpc\tr6,pc[20]", "4856");
        }

        [Test]
        public void Avr32Dis_lddpc_to_pc()
        {
            var instr = DisassembleHexBytes("485F");
            Assert.AreEqual(InstrClass.Transfer, instr.InstructionClass);
            Assert.AreEqual("lddpc\tpc,pc[20]", instr.ToString());
        }

        [Test]
        public void Avr32Dis_macu_d()
        {
            AssertCode("macu.d\tr4,r10,r9", "F4090744");
        }

        [Test]
        public void Avr32Dis_rsub()
        {
            AssertCode("rsub\tr6,pc", "1E26");
        }

        [Test]
        public void Avr32Dis_sub3_imm()
        {
            AssertCode("sub\tr9,pc,0000002C", "FEC9002C");
        }

        [Test]
        public void Avr32Dis_subcs_imm()
        {
            AssertCode("subcs\tr12,FFFFFFEA", "F7BC03EA");
        }

        [Test]
        public void Avr32Dis_mcall()
        {
            AssertCode("mcall\tr6[480]", "F0160078");
        }

        [Test]
        public void Avr32Dis_pushm()
        {
            AssertCode("pushm\tr4-r7,lr", "D421");
        }

        [Test]
        public void Avr32Dis_ld_ub()
        {
            AssertCode("ld.ub\tr9,r8[0]", "1189");
        }

        [Test]
        public void Avr32Dis_cp_b()
        {
            AssertCode("cp.b\tr9,r8", "F0091800");
        }

        [Test]
        public void Avr32Dis_breq()
        {
            AssertCode("breq\t00100008", "C040");
        }

        [Test]
        public void Avr32Dis_brmi_long()
        {
            AssertCode("brmi\t000FFE5C", "FE96FF2E");
        }

        [Test]
        public void Avr32Dis_popm_pc()
        {
            var instr = DisassembleHexBytes("D822");
            Assert.AreEqual("popm\tr4-r7,pc", instr.ToString());
            Assert.AreEqual(InstrClass.Transfer, instr.InstructionClass);
        }

        [Test]
        public void Avr32Dis_st_w_disp4()
        {
            AssertCode("st.w\tr10[0],r9", "9509");
        }

        [Test]
        public void Avr32Dis_icall()
        {
            AssertCode("icall\tr8", "5D18");
        }

        [Test]
        public void Avr32Dis_ld_w_format3()
        {
            AssertCode("ld.w\tr8,r8[0]", "7008");
        }

        [Test]
        public void Avr32Dis_ld_w_idx()
        {
            AssertCode("ld.w\tr3,r10[r5<<3]", "F4050333");
        }

        [Test]
        public void Avr32Dis_cp_w()
        {
            AssertCode("cp.w\tr8,00000000", "5808");
        }

        [Test]
        public void Avr32Dis_st_b()
        {
            AssertCode("st.b\tr8[0],r9", "B089");
        }

        [Test]
        public void Avr32Dis_cp_w_reg()
        {
            AssertCode("cp.w\tlr,r9", "123E");
        }

        [Test]
        public void Avr32Dis_cpc()
        {
            AssertCode("cpc\tr11,r9", "F20B1300");
        }

        [Test]
        public void Avr32Dis_st_w_postinc()
        {
            AssertCode("st.w\tr12++,sp", "18AD");
        }

        [Test]
        public void Avr32Dis_andh_COH()
        {
            AssertCode("andh\tr12,8000,COH", "E61C8000");
        }

        [Test]
        public void Avr32Dis_sub_sp_8imm()
        {
            AssertCode("sub\tsp,00000024", "209D");
        }

        [Test]
        public void Avr32Dis_sub2_imm()
        {
            AssertCode("sub\tr12,00000069", "269C");
        }

        [Test]
        public void Avr32Dis_eor3_lsl()
        {
            AssertCode("eor\tr12,r11,r9", "F7E9200C");
        }

        [Test]
        public void Avr32Dis_eorl()
        {
            AssertCode("eorl\tr8,0002", "EC180002");
        }

        [Test]
        public void Avr32Dis_rcall()
        {
            AssertCode("rcall\t000FFD12", "C89E");
        }

        [Test]
        public void Avr32Dis_rjmp()
        {
            AssertCode("rjmp\t00100006", "C038");
        }

        [Test]
        public void Avr32Dis_or3_lsl()
        {
            AssertCode("or\tr12,r10,r11<<1", "F5EB101C");
        }




        [Test]
        public void Avr32Dis_stm()
        {
            AssertCode("stm\t--sp,r0-r7,lr", "EBCD40FF");
        }

        [Test]
        public void Avr32Dis_st_d_disp()
        {
            AssertCode("st.d\tr7[-36],r9:r8", "EEE9FFDC");
        }

        [Test]
        public void Avr32Dis_lsl2()
        {
            AssertCode("lsl\tr11,+00000001", "A17B");
        }

        [Test]
        public void Avr32Dis_abs()
        {
            AssertCode("abs\tr11", "5C4B");
        }

        [Test]
        public void Avr32Dis_lsl_imm()
        {
            AssertCode("lsl\tr12,r11,00000001", "F60C1501");
        }



        [Test]
        public void Avr32Dis_reteq()
        {
            AssertCode("reteq\tr11", "5E0B");
        }

        [Test]
        public void Avr32Dis_st_w_disp()
        {
            AssertCode("st.w\tr7[-8],r8", "EF48FFF8");
        }

        // Reko: a decoder for AVR32 instruction E6080C02 at address 0000CEFE has not been implemented.
        [Test]
        public void Avr32Dis_divs()
        {
            AssertCode("divs\tr2,r3,r8", "E6080C02");
        }

        [Test]
        public void Avr32Dis_ld_d()
        {
            AssertCode("ld.d\tr11:r10,r12[0]", "F8EA0000");
        }

        // Reko: a decoder for AVR32 instruction E06807D9 at address 00006BAE has not been implemented. (0b00110)
        [Test]
        public void Avr32Dis_mov_long()
        {
            AssertCode("mov\tr8,000007D9", "E06807D9");
        }

        [Test]
        public void Avr32Dis_bfextu()
        {
            AssertCode("bfextu\tr2,r7,+00000001,+00000001", "E5D7C021");
        }

        [Test]
        public void Avr32Dis_sub3()
        {
            AssertCode("sub\tr9,r7,r4", "EE040109 ");
        }

        [Test]
        public void Avr32Dis_add3()
        {
            AssertCode("add\tr9,r8,r9", "F0090009");
        }

        [Test]
        public void Avr32Dis_lsr_long()
        {
            AssertCode("lsr\tr7,r11,00000014", "F6071614");
        }

        [Test]
        public void Avr32Dis_retmi()
        {
            AssertCode("retmi\tsp", "5E6D");
        }

        [Test]
        public void Avr32Dis_lsr2()
        {
            AssertCode("lsr\tr10,+00000008", "A98A");
        }

        [Test]
        public void Avr32Dis_casts_h()
        {
            AssertCode("casts.h\tr4", "5C84");
        }

        [Test]
        public void Avr32Dis_castu_b()
        {
            AssertCode("castu.b\tr12", "5C5C");
        }


        [Test]
        public void Avr32Dis_st_b_post()
        {
            AssertCode("st.b\tr7++,r9", "0EC9");
        }

        [Test]
        public void Avr32Dis_sub2()
        {
            AssertCode("sub\tr7,r12", "1817");
        }


        [Test]
        public void Avr32Dis_add2()
        {
            AssertCode("add\tr8,r10", "1408");
        }

        [Test]
        public void Avr32Dis_breq_long()
        {
            AssertCode("breq\t001001BA", "E08000DD");
        }

        [Test]
        public void Avr32Dis_acr()
        {
            AssertCode("acr\tr5", "5C05");
        }

        [Test]
        public void Avr32Dis_stm_pre()
        {
            AssertCode("stm\t--sp,r6,lr", "EBCD4040");
        }

        [Test]
        public void Avr32Dis_ldm_post()
        {
            var instr = DisassembleHexBytes("E3CD8040");
            Assert.AreEqual(InstrClass.Transfer, instr.InstructionClass);
            Assert.AreEqual("ldm\tsp++,r6,pc", instr.ToString());
        }

        [Test]
        public void Avr32Dis_movh()
        {
            AssertCode("movh\tlr,1892", "FC1E1892");
        }

        [Test]
        public void Avr32Dis_cp_w_imm()
        {
            AssertCode("cp.w\tr9,000000FF", "E04900FF");
        }

        [Test]
        public void Avr32Dis_bld()
        {
            AssertCode("bld\tr11,+00000014", "EDBB0014");
        }

        [Test]
        public void Avr32Dis_rsub3()
        {
            AssertCode("rsub\tr9,r9,00000001", "F2091101");
        }
    }
}