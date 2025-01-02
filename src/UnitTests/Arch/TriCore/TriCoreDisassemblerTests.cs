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

using NUnit.Framework;
using Reko.Arch.Infineon;
using Reko.Core;

namespace Reko.UnitTests.Arch.TriCore
{
    [TestFixture]
    public class TriCoreDisassemblerTests : DisassemblerTestBase<TriCoreInstruction>
    {
        private readonly TriCoreArchitecture arch;
        private readonly Address addr;

        public TriCoreDisassemblerTests()
        {
            this.arch = new TriCoreArchitecture(CreateServiceContainer(), "tricore", new());
            this.addr = Address.Ptr32(0x10_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            if (sExpected != instr.ToString()) // && instr.Mnemonic == Mnemonic.Nyi)
                Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void TriCoreDis_add_d15_SRC()
        {
            AssertCode("add\td15,#0x1", "C21F");
        }

        [Test]
        public void TriCoreDis_add_RR()
        {
            AssertCode("add\td2,d4,d1", "0B140020");
        }

        [Test]
        public void TriCoreDis_add_SRC()
        {
            AssertCode("add\td4,d15,#0x1", "9214");
        }

        [Test]
        public void TriCoreDis_add_SRR()
        {
            AssertCode("add\td15,d0", "420F");
        }

        [Test]
        public void TriCoreDis_add_SRR_d15()
        {
            AssertCode("add\td3,d15,d10", "12A3");
        }

        [Test]
        public void TriCoreDis_add_a_SRC()
        {
            AssertCode("add.a\ta2,#0x1", "B012");
        }

        [Test]
        public void TriCoreDis_add_a_SRR()
        {
            AssertCode("add.a\ta3,a4", "3043");
        }

        [Test]
        public void TriCoreDis_addi()
        {
            AssertCode("addi\td0,d0,#0x348", "1B803400");
        }

        [Test]
        public void TriCoreDis_addih()
        {
            AssertCode("addih\td8,d8,#0x1", "9B180080");
        }

        [Test]
        public void TriCoreDis_addih_a()
        {
            AssertCode("addih.a\ta2,a2,#0x1", "11120020");
        }

        [Test]
        public void TriCoreDis_adds_SRR()
        {
            AssertCode("adds\td1,d0", "2201");
        }

        [Test]
        public void TriCoreDis_addsc_a_RR()
        {
            AssertCode("addsc.a\ta3,a4,d8,#0x1", "01480136");
        }

        [Test]
        public void TriCoreDis_addsc_a_SRRS()
        {
            AssertCode("addsc.a\ta8,a0,d15,#0x0", "1008");
        }

        [Test]
        public void TriCoreDis_and_SC()
        {
            AssertCode("and\td15,#0x3", "1603");
        }

        [Test]
        public void TriCoreDis_and_SRR()
        {
            AssertCode("and\td0,d2", "2620");
        }

        [Test]
        public void TriCoreDis_and_and_t()
        {
            AssertCode("and.and.t\td0,d1,#0xF,d0,#0x1", "47018F00");
        }

        [Test]
        public void TriCoreDis_andn_imm()
        {
            AssertCode("andn\td0,d0,#0x80", "8F00C801");
        }

        [Test]
        public void TriCoreDis_and_ne_RC()
        {
            AssertCode("and.ne\td15,d2,#0xF", "8BF220F4");
        }

        [Test]
        public void TriCoreDis_cadd_SRC()
        {
            AssertCode("cadd\td2,d15,#0x1", "8A12");
        }

        [Test]
        public void TriCoreDis_call()
        {
            AssertCode("call\t00101E84", "6D00420F");
        }

        [Test]
        public void TriCoreDis_call_SB()
        {
            AssertCode("call\t00100080", "5C40");
        }

        [Test]
        public void TriCoreDis_calla()
        {
            AssertCode("calla\t00FF9000", "EDFF0090");
        }

        [Test]
        public void TriCoreDis_calli()
        {
            AssertCode("calli\ta2", "2D020000");
        }

        [Test]
        public void TriCoreDis_cmov_SRR()
        {
            AssertCode("cmov\td1,d15,d2", "2A21");
        }

        [Test]
        public void TriCoreDis_cmovn()
        {
            AssertCode("cmovn\td15,d15,d15", "6AFF");
        }

        [Test]
        public void TriCoreDis_div_RR()
        {
            AssertCode("div\te2,d4,d2", "4B240122");
        }

        [Test]
        public void TriCoreDis_div_u_RR()
        {
            AssertCode("div.u\te0,d15,d0", "4B0F1102");
        }

        [Test]
        public void TriCoreDis_eq_SRR()
        {
            AssertCode("eq\td15,d2,d15", "3AF280F2");
        }

        [Test]
        public void TriCoreDis_extr()
        {
            AssertCode("extr\td3,d3,#0x2,#0x1", "37036131");
        }

        [Test]
        public void TriCoreDis_fcall()
        {
            //$REVIEW
            AssertCode("fcall\tFFF9FBDC", "61F4EEFD");
        }

        [Test]
        public void TriCoreDis_insert_RCPW()
        {
            AssertCode("insert\td2,d2,#0xF,#0x1D,#0x1", "B7F2812E");
        }

        [Test]
        public void TriCoreDis_insert_RCRR()
        {
            AssertCode("insert\td0,d0,#0x0,d0", "97005D00");
        }

        [Test]
        public void TriCoreDis_isync()
        {
            AssertCode("isync", "0D00C004");
        }

        [Test]
        public void TriCoreDis_j()
        {
            AssertCode("j\t000FFFFE", "1DFF FFFF");
        }

        [Test]
        public void TriCoreDis_j_SB()
        {
            AssertCode("j\t000FFFFE", "3CFF");
        }

        [Test]
        public void TriCoreDis_jeq_a()
        {
            AssertCode("jeq.a\ta14,a15,0010001A", "7DFE0D00");
        }

        [Test]
        public void TriCoreDis_jgtz()
        {
            AssertCode("jgtz\td0,00100002", "4E01");
        }

        [Test]
        public void TriCoreDis_jl()
        {
            AssertCode("jl\t001000D0", "5D006800");
        }

        [Test]
        public void TriCoreDis_jne_SBR()
        {
            AssertCode("jne\td15,d3,0010001E", "FE3F");
        }

        [Test]
        public void TriCoreDis_jnz_SB()
        {
            AssertCode("jnz\td15,000FFFFE", "EEFF");
        }

        [Test]
        public void TriCoreDis_jnz_SBR()
        {
            AssertCode("jnz\td2,0010001E", "F62F");
        }

        [Test]
        public void TriCoreDis_jnz_a_SBR()
        {
            AssertCode("jnz.a\ta15,00100016", "7CFB");
        }

        [Test]
        public void TriCoreDis_jnz_t_SBRN()
        {
            AssertCode("jnz.t\td15,#0x0,0010000C", "AE06");
        }

        [Test]
        public void TriCoreDis_jz_SB()
        {
            AssertCode("jz\td15,000FFFFE", "6EFF");
        }

        [Test]
        public void TriCoreDis_jz_SBR()
        {
            AssertCode("jz\td0,0010001E", "760F");
        }

        [Test]
        public void TriCoreDis_jz_a()
        {
            AssertCode("jz.a\ta15,00100006", "BCF3");
        }

        [Test]
        public void TriCoreDis_jz_t()
        {
            AssertCode("jz.t\td1,#0x0,00100008", "6F010400");
        }

        [Test]
        public void TriCoreDis_jz_t_SBRN()
        {
            AssertCode("jz.t\td15,#0x4,0010000A", "2E45");
        }

        [Test]
        public void TriCoreDis_ld_a()
        {
            AssertCode("ld.a\ta10,[0003FFFF]", "85FA FFFB");
        }

        [Test]
        public void TriCoreDis_ld_a_postinc()
        {
            AssertCode("ld.a\ta15,[a13+]", "C4DF");
        }

        [Test]
        public void TriCoreDis_ji()
        {
            AssertCode("ji\ta15", "DC0F");
        }

        [Test]
        public void TriCoreDis_ld_a_SLR()
        {
            AssertCode("ld.a\ta14,[a3]", "D43E6DFF");
        }

        [Test]
        public void TriCoreDis_ld_a_SLRO()
        {
            AssertCode("ld.a\ta1,[a15]", "C801");
        }

        [Test]
        public void TriCoreDis_ld_b_BOL()
        {
            AssertCode("ld.b\td15,[a15]-1083", "79FF85FF");
        }

        [Test]
        public void TriCoreDis_ld_bu()
        {
            AssertCode("ld.bu\td0,[a0+]", "0400");
        }

        [Test]
        public void TriCoreDis_ld_bu_BOL()
        {
            AssertCode("ld.bu\td3,[a4]16", "39431000");
        }

        [Test]
        public void TriCoreDis_ld_bu_SLRO()
        {
            AssertCode("ld.bu\td0,[a15]", "0800BBD0");
        }

        [Test]
        public void TriCoreDis_ld_bu_SLR()
        {
            AssertCode("ld.bu\td15,[a0]", "140F");
        }

        [Test]
        public void TriCoreDis_ld_bu_SRO()
        {
            AssertCode("ld.bu\td15,[a2]", "0C20");
        }

        [Test]
        public void TriCoreDis_ld_d_postinc()
        {
            AssertCode("ld.d\te14,[a15+]8", "09FE4801");
        }

        [Test]
        public void TriCoreDis_ld_h_short()
        {
            AssertCode("ld.h\td15,[a8]30", "8C8F");
        }

        [Test]
        public void TriCoreDis_ld_h_BOL()
        {
            AssertCode("ld.h\td12,[a15]10080", "C9FC60D2");
        }

        [Test]
        public void TriCoreDis_ldh_SLR_post()
        {
            AssertCode("ld.h\td14,[a15+]", "84FE");
        }

        [Test]
        public void TriCoreDis_ld_hu_BOL()
        {
            AssertCode("ld.hu\td15,[a4]20", "B94F1400");
        }

        [Test]
        public void TriCoreDis_ld_w()
        {
            AssertCode("ld.w\td1,[0003C5F0]", "85F17070");
        }

        [Test]
        public void TriCoreDis_ld_w_a15()
        {
            AssertCode("ld.w\td2,[a15]28", "4872");
        }

        [Test]
        public void TriCoreDis_ld_w_BOL()
        {
            AssertCode("ld.w\td3,[a4]16384", "19430004");
        }

        [Test]
        public void TriCoreDis_ld_w_postinc()
        {
            AssertCode("ld.w\td2,[a14+]", "44E27422");
        }

        [Test]
        public void TriCoreDis_ld_w_preinc()
        {
            AssertCode("ld.w\td4,[+a0]-340", "09042CA5");
        }

        [Test]
        public void TriCoreDis_ld_w_SC()
        {
            AssertCode("ld.w\td15,[a10]28", "5807");
        }

        [Test]
        public void TriCoreDis_ld_w_SLR()
        {
            AssertCode("ld.w\td0,[a4]", "5440");
        }

        [Test]
        public void TriCoreDis_ld_w_SRO()
        {
            AssertCode("ld.w\td15,[a15]", "4CF0");
        }

        [Test]
        public void TriCoreDis_ldmst()
        {
            AssertCode("ldmst\t[0001DBCF],e14", "E57E8FF1");
        }

        [Test]
        public void TriCoreDis_lea_bol()
        {
            AssertCode("lea\ta10,[a10]", "D9AA0000");
            AssertCode("lea\ta10,[a2]-1408", "D92A80AF");
        }

        [Test]
        public void TriCoreDis_lha()
        {
            AssertCode("lha\ta15,[0003FBC5]", "C5FF85FF");
        }

        [Test]
        public void TriCoreDis_loop_SBR()
        {
            AssertCode("loop\td5,000FFFFE", "FC5F");
        }

        [Test]
        public void TriCoreDis_madd_u_RCR()
        {
            AssertCode("madd.u\td4,d2,d5,#0x2", "13254042");
        }

        [Test]
        public void TriCoreDis_madds_u()
        {
            AssertCode("madds.u\td1,d2,d14,#0x2F", "13FE8212");
        }

        [Test]
        public void TriCoreDis_mfcr()
        {
            AssertCode("mfcr\td0,PSW", "4D40E00F");
        }

        [Test]
        public void TriCoreDis_mov_Edd()
        {
            AssertCode("mov\te8,d5,d6", "0B651088");
        }

        [Test]
        public void TriCoreDis_mov_E_RLC()
        {
            AssertCode("mov\te4,#0x4C7", "FB7F4C40");
        }

        [Test]
        public void TriCoreDis_mov_RLC()
        {
            AssertCode("mov\td4,#0xFF", "3BF00F40");
        }

        [Test]
        public void TriCoreDis_mov_SRR()
        {
            AssertCode("mov\td4,d2", "0224");
        }

        [Test]
        public void TriCoreDis_mov_a_SRC()
        {
            AssertCode("mov.a\ta2,#0x0", "A002");
        }

        [Test]
        public void TriCoreDis_mov_a_SRR()
        {
            AssertCode("mov.a\ta14,d14", "60EE");
        }

        [Test]
        public void TriCoreDis_mov_aa_RR()
        {
            AssertCode("mov.aa\ta5,a0", "01000C50");
        }

        [Test]
        public void TriCoreDis_mov_aa_SRR()
        {
            AssertCode("mov.aa\ta4,a10", "40A46D00");
        }

        [Test]
        public void TriCoreDis_mov_SRC()
        {
            AssertCode("mov\td4,#0x0", "8204");
        }

        [Test]
        public void TriCoreDis_mov_d_SRR()
        {
            AssertCode("mov.d\td1,a2", "8021");
        }

        [Test]
        public void TriCoreDis_mov_u_RLC()
        {
            AssertCode("mov.u\td1,#0x900D", "BBD00019");
        }

        [Test]
        public void TriCoreDis_movh()
        {
            AssertCode("movh\td0,#0xD000", "7B00000D");
        }

        [Test]
        public void TriCoreDis_movh_a()
        {
            AssertCode("movh.a\ta10,#0xB001", "911000AB");
        }

        [Test]
        public void TriCoreDis_mtcr()
        {
            AssertCode("mtcr\tISP,d0", "CD80E20F");
        }

        [Test]
        public void TriCoreDis_mul_SRR()
        {
            AssertCode("mul\td15,d0", "E20F");
        }

        [Test]
        public void TriCoreDis_ne_a()
        {
            AssertCode("ne.a\td4,a0,a0", "01001444");
        }

        [Test]
        public void TriCoreDis_nop_short()
        {
            AssertCode("nop", "0000");
        }

        [Test]
        public void TriCoreDis_or_imm()
        {
            AssertCode("or\td0,d0,#0x7F", "8FF04701");
        }

        [Test]
        public void TriCoreDis_or_SC()
        {
            AssertCode("or\td15,#0xF0", "96F0");
        }

        [Test]
        public void TriCoreDis_or_SRR()
        {
            AssertCode("or\td2,d3", "A632");
        }

        [Test]
        public void TriCoreDis_rslcx()
        {
            AssertCode("rslcx", "0D8060D2");
        }

        [Test]
        public void TriCoreDis_sel_RCR()
        {
            AssertCode("sel\td7,d2,d7,#0x2", "AB278072");
        }

        [Test]
        public void TriCoreDis_xor_RC()
        {
            AssertCode("xor\td3,d3,#0x2", "8F238031");
        }

        [Test]
        public void TriCoreDis_sh_SRC()
        {
            AssertCode("sh\td3,#0xFFFFFFF8", "0683");
        }

        [Test]
        public void TriCoreDis_sh_and_t()
        {
            AssertCode("sh.and.t\td13,d0,#0xF,d0,#0x7", "27008FD3");
        }

        [Test]
        public void TriCoreDis_st_a_ABS()
        {
            AssertCode("st.a\t[0003C5F0],a0", "A5F07070");
        }

        [Test]
        public void TriCoreDis_st_a_BOL()
        {
            AssertCode("st.a\t[a10]12,d6", "B5A60C00");
        }

        [Test]
        public void TriCoreDis_st_a_SSR()
        {
            AssertCode("st.a\t[a5],a4", "F454");
        }

        [Test]
        public void TriCoreDis_st_a_SSRO()
        {
            AssertCode("st.a\t[a15]60,a3", "E8F39140");
        }

        [Test]
        public void TriCoreDis_st_b_BOL()
        {
            AssertCode("st.b\t[a2]16,d15", "E92F1000");
        }

        [Test]
        public void TriCoreDis_st_b_postinc()
        {
            AssertCode("st.b\t[a2+],d8", "2428FC5E");
        }

        [Test]
        public void TriCoreDis_st_b_BO_postinc()
        {
            AssertCode("st.b\t[a14+]8,d14", "89EE4801");
        }

        [Test]
        public void TriCoreDis_st_b_short()
        {
            AssertCode("st.b\t[a2],d3", "3423");
        }

        [Test]
        public void TriCoreDis_st_b_SRO()
        {
            AssertCode("st.b\t[a10]2,d15", "2CA2");
        }

        [Test]
        public void TriCoreDis_st_b_stk()
        {
            AssertCode("st.b\t[a15]2,d0", "2820");
        }

        [Test]
        public void TriCoreDis_st_h_postinc()
        {
            AssertCode("st.h\t[a14+],d14", "A4EEDF01");
        }

        [Test]
        public void TriCoreDis_st_h_SRO()
        {
            AssertCode("st.h\t[a15]28,d15", "ACFE");
        }

        [Test]
        public void TriCoreDis_st_h_SSR()
        {
            AssertCode("st.h\t[a4],d2", "B442");
        }

        [Test]
        public void TriCoreDis_st_w_short()
        {
            AssertCode("st.w\t[a15]60,d3", "68F3");
        }

        [Test]
        public void TriCoreDis_st_w_sp()
        {
            AssertCode("st.w\t[a10]24,d15", "7806");
        }

        [Test]
        public void TriCoreDis_st_w_postinc()
        {
            AssertCode("st.w\t[a0+],d14", "640E");
        }

        [Test]
        public void TriCoreDis_st_w_BOL()
        {
            AssertCode("st.w\t[a10]4,d4", "59A40400");
        }

        [Test]
        public void TriCoreDis_st_w_SRO()
        {
            AssertCode("st.w\t[a4]4,d15", "6C41");
        }

        [Test]
        public void TriCoreDis_st_w_SSR()
        {
            AssertCode("st.w\t[a10],d4", "74A4");
        }

        [Test]
        public void TriCoreDis_stucx()
        {
            AssertCode("stucx\t[00000082]", "15090224");
        }

        [Test]
        public void TriCoreDis_sub_SRR()
        {
            AssertCode("sub\td8,d15", "A2F8");
        }

        [Test]
        public void TriCoreDis_sub_SRR_d15()
        {
            AssertCode("sub\td15,d4,d5", "5A54");
        }

        [Test]
        public void TriCoreDis_sub_a_SC()
        {
            AssertCode("sub.a\ta8,#0x8", "2008");
        }

        [Test]
        public void TriCoreDis_swap_w()
        {
            AssertCode("swap.w\t[a0+]-453,d0", "49003B80");
        }

        [Test]
        public void TriCoreDis_xor_SRR()
        {
            AssertCode("xor\td15,d4", "C64F");
        }
    }
}
