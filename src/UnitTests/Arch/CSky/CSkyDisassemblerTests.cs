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
using Reko.Arch.CSky;
using Reko.Core;

namespace Reko.UnitTests.Arch.CSky
{
    public class CSkyDisassemblerTests : DisassemblerTestBase<CSkyInstruction>
    {
        private readonly CSkyArchitecture arch;
        private readonly Address addr;

        public CSkyDisassemblerTests()
        {
            this.arch = new CSkyArchitecture(CreateServiceContainer(), "csky", new());
            this.addr = Address.Ptr32(0x10_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExpected, string sHexBytes)
        {
            var instr = base.DisassembleHexBytes(sHexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void CSkyDis_abs()
        {
            AssertCode("abs\tr30,r31", "1FC4 1E02");
        }

        [Test]
        public void CSkyDis_addc_16()
        {
            AssertCode("addc\tr7,r8", "E161");
        }

        [Test]
        public void CSkyDis_addc_32()
        {
            AssertCode("addc\tr31,r30,r24", "1EC7 5F00");
        }

        [Test]
        public void CSkyDis_addi_16()
        {
            AssertCode("addi\tr0,0x3", "0220");
        }

        [Test]
        public void CSkyDis_addi_16_b()
        {
            AssertCode("addi\tr0,r6,0x2", "065E");
        }

        [Test]
        public void CSkyDis_addi_r28()
        {
            AssertCode("addi\tr30,r28,0x40000", "DFCF FFFF");
        }

        [Test]
        public void CSkyDis_addi_sp()
        {
            AssertCode("addi\tr6,r14,0x3FC", "FF1E");
        }

        [Test]
        public void CSkyDis_addi_sp_sp()
        {
            AssertCode("addi\tr14,r14,0x1FC", "1F17");
        }

        [Test]
        public void CSkyDis_addu_16()
        {
            AssertCode("addu\tr15,r5", "D463");
        }

        [Test]
        public void CSkyDis_addu_3_16()
        {
            AssertCode("addu\tr5,r4,r6", "B85C");
        }

        [Test]
        public void CSkyDis_addu_32()
        {
            AssertCode("addu\tr21,r20,r24", "14C7 3500");
        }

        [Test]
        public void CSkyDis_and_16()
        {
            AssertCode("and\tr9,r8", "606A");
        }

        [Test]
        public void CSkyDis_and_32()
        {
            AssertCode("and\tr17,r20,r24", "14C7 3120");
        }

        [Test]
        public void CSkyDis_andi()
        {
            AssertCode("andi\tr25,r17,0xFFF", "31E7 FF2F");
        }

        [Test]
        public void CSkyDis_andn_16()
        {
            AssertCode("andn\tr9,r11", "6D6A");
        }

        [Test]
        public void CSkyDis_andn_32()
        {
            AssertCode("andn\tr17,r20,r24", "14C7 5120");
        }

        [Test]
        public void CSkyDis_andni()
        {
            AssertCode("andni\tr17,r19,0xFFF", "33E6 FF3F");
        }

        [Test]
        public void CSkyDis_asr_16()
        {
            AssertCode("asr\tr9,r1", "4672");
        }

        [Test]
        public void CSkyDis_asr_32()
        {
            AssertCode("asr\tr19,r1,r17", "21C6 9340");
        }

        [Test]
        public void CSkyDis_asrc()
        {
            AssertCode("asrc\tr4,r17,0x12", "31C6 844C");
        }

        [Test]
        public void CSkyDis_asri_16()
        {
            AssertCode("asri\tr3,r4,0x1F", "7F54");
        }

        [Test]
        public void CSkyDis_asri_32()
        {
            AssertCode("asri\tr17,r1,0x1B", "61C7 9148");
        }

        [Test]
        public void CSkyDis_bclri_16()
        {
            AssertCode("bclri\tr7,0x1F", "9F3F");
        }

        [Test]
        public void CSkyDis_bclri_32()
        {
            AssertCode("bseti\tr18,r8,0x12", "48C7 5228");
        }

        [Test]
        public void CSkyDis_bez()
        {
            AssertCode("bez\tr17,000FFFFE", "11E9FFFF");
        }

        [Test]
        public void CSkyDis_bf_16()
        {
            AssertCode("bf\t000FFFFE", "FF0F");
        }

        [Test]
        public void CSkyDis_bf_32()
        {
            AssertCode("bf\t000FFFFE", "40E8FFFF");
        }

        [Test]
        public void CSkyDis_bgenr()
        {
            AssertCode("bgenr\tr28,r17", "11C4 5C50");
        }

        [Test]
        public void CSkyDis_bhsz()
        {
            AssertCode("bhsz\tr19,000F0000", "B3E9 0080");
        }

        [Test]
        public void CSkyDis_bkpt()
        {
            AssertCode("bkpt", "0000");
        }

        [Test]
        public void CSkyDis_bmaski()
        {
            AssertCode("bmaski\tr17,0x12", "20C6 3150");
        }

        [Test]
        public void CSkyDis_bmclr()
        {
            AssertCode("bmclr", "00C0 2014");
        }

        [Test]
        public void CSkyDis_bmset()
        {
            AssertCode("bmset", "00C0 2010");
        }

        [Test]
        public void CSkyDis_bpop_h()
        {
            AssertCode("bpop.h\tr5", "B414");
        }

        [Test]
        public void CSkyDis_bpop_w()
        {
            AssertCode("bpop.w\tr5", "B614");
        }

        [Test]
        public void CSkyDis_bpush_h()
        {
            AssertCode("bpush.h\tr5", "F414");
        }

        [Test]
        public void CSkyDis_bpush_w()
        {
            AssertCode("bpush.w\tr5", "F614");
        }

        [Test]
        public void CSkyDis_br_16()
        {
            AssertCode("br\t000FFC00", "0006");
        }

        [Test]
        public void CSkyDis_br_32()
        {
            AssertCode("br\t000F0000", "00E8 0080");
        }

        [Test]
        public void CSkyDis_brev()
        {
            AssertCode("brev\tr19,r17", "11C4 1362");
        }

        [Test]
        public void CSkyDis_bseti_16()
        {
            AssertCode("bseti\tr7,0x1F", "BF3F");
        }

        [Test]
        public void CSkyDis_bseti_32()
        {
            AssertCode("bseti\tr17,r17,0x11", "31C6 5128");
        }

        [Test]
        public void CSkyDis_bsr()
        {
            AssertCode("bsr\tFC100000", "00E2 0000");
        }

        [Test]
        public void CSkyDis_bt_16()
        {
            AssertCode("bt\t000FFC00", "000A");
        }

        [Test]
        public void CSkyDis_bt_32()
        {
            AssertCode("bt\t000F0000", "60E8 0080");
        }

        [Test]
        public void CSkyDis_btsti_16()
        {
            AssertCode("btsti\tr7,0x15", "D53F");
        }

        [Test]
        public void CSkyDis_btsti_32()
        {
            AssertCode("btsti\tr0,r17,0x0", "31C6 8028");
        }

        [Test]
        public void CSkyDis_clrf()
        {
            AssertCode("clrf\tr17", "20C6 202C");
        }

        [Test]
        public void CSkyDis_clrt()
        {
            AssertCode("clrt\tr17", "20C6 402C");
        }  

        [Test]
        public void CSkyDis_cmphs()
        {
            AssertCode("cmphs\tr8,r0", "2064");
        }

        [Test]
        public void CSkyDis_cmphsi_16()
        {
            AssertCode("cmphsi\tr5,0x20", "1F3D");
        }

        [Test]
        public void CSkyDis_cmphsi_32()
        {
            AssertCode("cmphsi\tr17,0x10000", "11EB FFFF");
        }

        [Test]
        public void CSkyDis_cmplt()
        {
            AssertCode("cmplt\tr8,r0", "2164");
        }

        [Test]
        public void CSkyDis_decf()
        {
            AssertCode("decf\tr17,r19,0x15", "33C6 950C");
        }

        [Test]
        public void CSkyDis_decgt()
        {
            AssertCode("decgt\tr23,r19,0x4", "93C4 3710");
        }

        [Test]
        public void CSkyDis_declt()
        {
            AssertCode("declt\tr23,r19,0x4", "93C4 5710");
        }

        [Test]
        public void CSkyDis_decne()
        {
            AssertCode("decne\tr23,r19,0x4", "93C4 9710");
        }

        [Test]
        public void CSkyDis_dect()
        {
            AssertCode("dect\tr17,r19,0x15", "33C6 150D");
        }

        [Test]
        public void CSkyDis_divs()
        {
            AssertCode("divs\tr23,r17,r25", "31C7 5780");
        }

        [Test]
        public void CSkyDis_divu()
        {
            AssertCode("divu\tr23,r17,r25", "31C7 3780");
        }

        [Test]
        public void CSkyDis_doze()
        {
            AssertCode("doze", "00C0 2050");
        }

        [Test]
        public void CSkyDis_ff0()
        {
            AssertCode("ff0\tr21,r19", "13C4 357C");
        }

        [Test]
        public void CSkyDis_ff1()
        {
            AssertCode("ff1\tr21,r19", "13C4 557C");
        }

        [Test]
        public void CSkyDis_grs()
        {
            AssertCode("grs\tr17,000C0000", "2ECE 0000");
        }

        [Test]
        public void CSkyDis_idly()
        {
            AssertCode("idly\t0x19", "20C3 201C");
        }

        [Test]
        public void CSkyDis_incf()
        {
            AssertCode("incf\tr17,r22,0x17", "36C6 370C");
        }

        [Test]
        public void CSkyDis_inct()
        {
            AssertCode("inct\tr17,r22,0x17", "36C6 570C");
        }

        [Test]
        public void CSkyDis_ins()
        {
            AssertCode("ins\tr17,r23,0x5,0x1", "37C6 815C");
        }

        [Test]
        public void CSkyDis_ipop()
        {
            AssertCode("ipop", "6314");
        }

        [Test]
        public void CSkyDis_ipush()
        {
            AssertCode("ipush", "6214");
        }

        [Test]
        public void CSkyDis_ixd()
        {
            AssertCode("ixd\tr17,r22,r23", "F6C6 9108");
        }

        [Test]
        public void CSkyDis_ixh()
        {
            AssertCode("ixh\tr17,r22,r23", "F6C6 3108");
        }

        [Test]
        public void CSkyDis_ixw()
        {
            AssertCode("ixw\tr17,r22,r23", "F6C6 5108");
        }

        [Test]
        public void CSkyDis_jmp_16()
        {
            AssertCode("jmp\tr9", "2478");
        }

        [Test]
        public void CSkyDis_jmp_32()
        {
            AssertCode("jmp\tr17", "D1E8 0000");
        }

        [Test]
        public void CSkyDis_jmpi()
        {
            AssertCode("jmpi\t00100100", "C0EA 8000");
        }

        [Test]
        public void CSkyDis_jmpix_16()
        {
            AssertCode("jmpix\tr4,0x18", "E13C");
        }

        [Test]
        public void CSkyDis_jmpix_32()
        {
            AssertCode("jmpix\tr17,0x28", "F1E90300");
        }

        [Test]
        public void CSkyDis_jsr_16()
        {
            AssertCode("jsr\tr12", "F17B");
        }

        [Test]
        public void CSkyDis_jsr_32()
        {
            AssertCode("jsr\tr1", "E1E8 0000");
        }

        [Test]
        public void CSkyDis_jsri()
        {
            AssertCode("jsri\t00100100", "E0EA 8000");
        }

        [Test]
        public void CSkyDis_ld_b_16()
        {
            AssertCode("ld.b\tr5,(r4,16)", "B084");
        }

        [Test]
        public void CSkyDis_ld_b_32()
        {
            AssertCode("ld.b\tr17,(r17,2048)", "31DA 0008");
        }

        [Test]
        public void CSkyDis_ld_bs()
        {
            AssertCode("ld.bs\tr17,(r17,2048)", "31DA 0048");
        }

        [Test]
        public void CSkyDis_ld_d()
        {
            AssertCode("ld.d\tr17,(r17,16384)", "31DA 0038");
        }

        [Test]
        public void CSkyDis_ld_h_16()
        {
            AssertCode("ld.h\tr5,(r4,32)", "B08C");
        }

        [Test]
        public void CSkyDis_ld_h_32()
        {
            AssertCode("ld.h\tr17,(r17,4096)", "31DA 0018");
        }

        [Test]
        public void CSkyDis_ld_hs_32()
        {
            AssertCode("ld.hs\tr17,(r17,4096)", "31DA 0058");
        }

        [Test]
        public void CSkyDis_ld_w_16a()
        {
            AssertCode("ld.w\tr5,(r4,64)", "B094");
        }

        [Test]
        public void CSkyDis_ld_w_16b()
        {
            AssertCode("ld.w\tr5,(r14,64)", "B09C");
        }

        [Test]
        public void CSkyDis_ld_w_32()
        {
            AssertCode("ld.w\tr17,(r17,8192)", "31DA 0028");
        }

        [Test]
        public void CSkyDis_ldex_w()
        {
            AssertCode("ldex.w\tr17,(r17,8192)", "31DA 0078");
        }

        [Test]
        [Ignore("Needs registerrange")]
        public void CSkyDis_ldm()
        {
            AssertCode("ld.hs", "37D2 351C");
        }

        [Test]
        public void CSkyDis_ldr_b_a()
        {
            AssertCode("ldr.b\tr17,(r19,r23)", "F3D2 3100");
        }

        [Test]
        public void CSkyDis_ldr_b_b()
        {
            AssertCode("ldr.b\tr17,(r19,r23<<1)", "F3D2 5100");
        }

        [Test]
        public void CSkyDis_ldr_b_c()
        {
            AssertCode("ldr.b\tr17,(r19,r23<<2)", "F3D2 9100");
        }

        [Test]
        public void CSkyDis_ldr_b_d()
        {
            AssertCode("ldr.b\tr17,(r19,r23<<3)", "F3D2 1101");
        }

        [Test]
        public void CSkyDis_ldr_bs()
        {
            AssertCode("ldr.bs\tr17,(r19,r23<<3)", "F3D2 1111");
        }

        [Test]
        public void CSkyDis_ldr_h()
        {
            AssertCode("ldr.h\tr17,(r19,r23<<3)", "F3D2 1105");
        }

        [Test]
        public void CSkyDis_ldr_hs()
        {
            AssertCode("ldr.hs\tr17,(r19,r23<<3)", "F3D2 1115");
        }

        [Test]
        public void CSkyDis_ldr_w()
        {
            AssertCode("ldr.w\tr17,(r19,r23<<3)", "F3D2 1109");
        }

        [Test]
        public void CSkyDis_lrs_b()
        {
            AssertCode("lrs.b\tr17,r28,000E0000", "22CE 0000");
        }

        [Test]
        public void CSkyDis_lrs_h()
        {
            AssertCode("lrs.h\tr17,r28,000C0000", "26CE 0000");
        }

        [Test]
        public void CSkyDis_lrs_w()
        {
            AssertCode("lrs.w\tr17,r28,00080000", "2ACE 0000");
        }

        [Test]
        public void CSkyDis_lrw_16()
        {
            AssertCode("lrw\tr3,[0010014C]", "7302");
        }

        [Test]
        public void CSkyDis_lrw_32()
        {
            AssertCode("lrw\tr1,[0010034C]", "2C11");
        }

        [Test]
        public void CSkyDis_lrw_pcrel()
        {
            AssertCode("lrw\tr8,[00100300]", "88EAC000");
        }

        [Test]
        public void CSkyDis_lsl_16()
        {
            AssertCode("lsl\tr11,r4", "D072");
        }

        [Test]
        public void CSkyDis_lsl_32()
        {
            AssertCode("lsl\tr23,r19,r17", "33C6 3740");
        }

        [Test]
        public void CSkyDis_lslc()
        {
            AssertCode("lslc\tr23,r19,0x12", "33C6 374C");
        }

        [Test]
        public void CSkyDis_lsli_16()
        {
            AssertCode("lsli\tr7,r6,0x1F", "FF46");
        }

        [Test]
        public void CSkyDis_lsli_32()
        {
            AssertCode("lsli\tr23,r19,0x11", "33C6 3748");
            AssertCode("lsli\tr7,r9,0x1F", " E9C7 2748");
//2000259C C7E9 4827 lsli r7, r9,0x20
//200025A0 C428 4846 lsri r6, r8,0x2

        }

        [Test]
        public void CSkyDis_lsr_16()
        {
            AssertCode("lsr\tr11,r4", "D172");
        }

        [Test]
        public void CSkyDis_lsr_32()
        {
            AssertCode("lsr\tr23,r19,r17", "33C6 5740");
        }

        [Test]
        public void CSkyDis_lsrc_32()
        {
            AssertCode("lsrc\tr23,r19,0x12", "33C6 574C");
        }

        [Test]
        public void CSkyDis_lsri_16()
        {
            AssertCode("lsri\tr7,r6,0x1F", "FF4E");
        }

        [Test]
        public void CSkyDis_lsri_32()
        {
            AssertCode("lsri\tr23,r19,0x11", "33C6 5748");
        }

        [Test]
        public void CSkyDis_mfcr()
        {
            AssertCode("mfcr\tr23,cr831", "3FC3 3760");
        }

        [Test]
        public void CSkyDis_mfhi()
        {
            AssertCode("mfhi\tr23", "00C4 379C");
        }

        [Test]
        public void CSkyDis_mfhis()
        {
            AssertCode("mfhis\tr23", "00C4 3798");
        }

        [Test]
        public void CSkyDis_mflo()
        {
            AssertCode("mflo\tr23", "00C4 979C");
        }

        [Test]
        public void CSkyDis_mflos()
        {
            AssertCode("mflos\tr23", "00C4 9798");
        }

        [Test]
        public void CSkyDis_mov_16()
        {
            AssertCode("mov\tr14,r7", "9F6F");
        }

        [Test]
        public void CSkyDis_mov_32()
        {
            AssertCode("mov\tr31,r19", "13C4 3F48");
        }

        /*
        [Test]
        public void CSkyDis_movf()
        {
            AssertCode("mov\tr14,r7", "3FC6 200C");
        }
        */

        [Test]
        public void CSkyDis_movi_16()
        {
            AssertCode("movi\tr7,0xFF", "FF37");
            AssertCode("movi\tr3,0x1", "0133");
            AssertCode("movi\tr0,0x38", "3830");
        }

        [Test]
        public void CSkyDis_movi_32()
        {
            AssertCode("movi\tr23,0xFFFF", "17EA FFFF");
        }

        [Test]
        public void CSkyDis_movih()
        {
            AssertCode("movih\tr23,0xFFFF", "37EA FFFF");
        }

        //[Test]
        //public void CSkyDis_movt()
        //{
        //    AssertCode("mov\tr14,r7", "3FC6 400C");
        //}

        [Test]
        public void CSkyDis_mtcr()
        {
            AssertCode("mtcr\tcr567,r31", "37C2 3F64");
        }

        [Test]
        public void CSkyDis_mthi()
        {
            AssertCode("mthi\tr0", "33C6 409C");
        }

        [Test]
        public void CSkyDis_mtlo()
        {
            AssertCode("mtlo\tr0", "33C6 009D");
        }

        [Test]
        public void CSkyDis_muls()
        {
            AssertCode("muls\tr20,r17", "34C6 208C");
        }

        [Test]
        public void CSkyDis_mulsa()
        {
            AssertCode("mulsa\tr20,r17", "34C6 408C");
        }

        [Test]
        public void CSkyDis_mulsh_16()
        {
            AssertCode("mulsh\tr12,r3", "0D7F");
        }

        [Test]
        public void CSkyDis_mulsh_32()
        {
            AssertCode("mulsh\tr31,r20,r17", "34C6 3F90");
        }

        [Test]
        public void CSkyDis_mulsha()
        {
            AssertCode("mulsha\tr20,r17", "34C6 4090");
        }

        [Test]
        public void CSkyDis_mulshs()
        {
            AssertCode("mulshs\tr20,r17", "34C6 8090");
        }

        [Test]
        public void CSkyDis_mulss()
        {
            AssertCode("mulss\tr20,r17", "34C6 808C");
        }

        [Test]
        public void CSkyDis_mulsw()
        {
            AssertCode("mulw\tr20,r17", "34C6 5794");
        }

        [Test]
        public void CSkyDis_mulswa()
        {
            AssertCode("mulswa\tr20,r17", "34C6 8094");
        }

        [Test]
        public void CSkyDis_mulsws()
        {
            AssertCode("mulsws\tr20,r17", "34C6 0095");
        }

        [Test]
        public void CSkyDis_mult_16()
        {
            AssertCode("mult\tr12,r2", "087F");
        }

        [Test]
        public void CSkyDis_mult_32()
        {
            AssertCode("mult\tr23,r20,r17", "34C6 3784");
        }

        [Test]
        public void CSkyDis_mulu()
        {
            AssertCode("mulu\tr20,r17", "34C6 2088");
        }

        [Test]
        public void CSkyDis_mulua()
        {
            AssertCode("mulua\tr20,r17", "34C6 4088");
        }

        [Test]
        public void CSkyDis_mulus()
        {
            AssertCode("mulus\tr20,r17", "34C6 8088");
        }

        [Test]
        public void CSkyDis_mvc()
        {
            AssertCode("mvc\tr21", "00C4 1505");
        }

        [Test]
        public void CSkyDis_mvcv_16()
        {
            AssertCode("mvcv\tr12", "0367");
        }

        [Test]
        public void CSkyDis_mvcv_32()
        {
            AssertCode("mvcv\tr21", "00C4 1506");
        }

        [Test]
        public void CSkyDis_mvtc()
        {
            AssertCode("mvtc\tr0", "00C4 009A");
        }

        [Test]
        public void CSkyDis_nie()
        {
            AssertCode("nie", "6014");
        }

        [Test]
        public void CSkyDis_nir()
        {
            AssertCode("nir", "6114");
        }

        [Test]
        public void CSkyDis_nor_16()
        {
            AssertCode("nor\tr0,r1", "066C");
        }

        [Test]
        public void CSkyDis_nor_32()
        {
            AssertCode("nor\tr23,r20,r17", "34C6 9724");
        }
        /*
        [Test]
        public void CSkyDis_not_16()
        {
            AssertCode("nor\tr14,r7", "0667");
        }

        [Test]
        public void CSkyDis_not_32()
        {
            AssertCode("nor\tr14,r7", "34C6 9724");
        }
        */

        [Test]
        public void CSkyDis_or_16()
        {
            AssertCode("or\tr8,r9", "246E");
        }

        [Test]
        public void CSkyDis_or_32()
        {
            AssertCode("or\tr23,r20,r17", "34C6 3724");
        }

        [Test]
        public void CSkyDis_ori()
        {
            AssertCode("ori\tr17,r20,0x8000", "34EE 0080");
        }

        [Test]
        public void CSkyDis_pldr()
        {
            AssertCode("pldr\t(r19,8196)", "13D8 0168");
        }

        [Test]
        public void CSkyDis_pldw()
        {
            AssertCode("pldrw\t(r19,8196)", "13DC 0168");
        }

        [Test]
        public void CSkyDis_pop_16()
        {
            AssertCode("pop\tr4-r6,r15", "9314");
        }

        [Test]
        public void CSkyDis_push_16()
        {
            AssertCode("push\tr4-r6,r15", "D314");
        }

        [Test]
        public void CSkyDis_revb_16()
        {
            AssertCode("revb\tr12,r1", "067B");
        }

        [Test]
        public void CSkyDis_revb_32()
        {
            AssertCode("revb\tr20,r17", "11C4 9460");
        }

        [Test]
        public void CSkyDis_revh_16()
        {
            AssertCode("revh\tr12,r1", "077B");
        }

        [Test]
        public void CSkyDis_revh_32()
        {
            AssertCode("revh\tr20,r17", "11C4 1461");
        }

        [Test]
        public void CSkyDis_rfi()
        {
            AssertCode("rfi", "00C0 2044");
        }

        [Test]
        public void CSkyDis_rotl_16()
        {
            AssertCode("rotl\tr8,r9", "2772");
        }

        [Test]
        public void CSkyDis_rotl_32()
        {
            AssertCode("rotl\tr23,r19,r17", "33C6 1741");
        }

        [Test]
        public void CSkyDis_rotli()
        {
            AssertCode("rotli\tr23,r19,0x11", "33C6 1749");
        }

        [Test]
        public void CSkyDis_rte()
        {
            AssertCode("rte", "00C0 2040");
        }

        [Test]
        public void CSkyDis_sce()
        {
            AssertCode("sce\t0x5", "A0C0 2018");
        }

        [Test]
        public void CSkyDis_se()
        {
            AssertCode("se", "00C0 2058");
        }

        [Test]
        public void CSkyDis_sext()
        {
            AssertCode("sext\tr1,r19,0x1,0x1F", "33C4 F25B");
        }

        [Test]
        public void CSkyDis_sextb()
        {
            AssertCode("sextb\tr12,r1", "0677");
        }

        [Test]
        public void CSkyDis_sexth()
        {
            AssertCode("sexth\tr12,r1", "0777");
        }

        [Test]
        public void CSkyDis_srs_b()
        {
            AssertCode("srs.b\tr24,r28,000E0001", "12CF 0100");
        }

        [Test]
        public void CSkyDis_srs_h()
        {
            AssertCode("srs.h\tr24,r28,000C0002", "16CF 0100");
        }

        [Test]
        public void CSkyDis_srs_w()
        {
            AssertCode("srs.w\tr24,r28,00080004", "1ACF 0100");
        }

        [Test]
        public void CSkyDis_srte()
        {
            AssertCode("srte", "00C0 207C");
        }

        [Test]
        public void CSkyDis_st_b_16()
        {
            AssertCode("st.b\tr2,(r6,31)", "5FA6");
        }

        [Test]
        public void CSkyDis_st_b_32()
        {
            AssertCode("st.b\tr17,(r19,2049)", "33DE 0108");
        }

        [Test]
        public void CSkyDis_st_d_32()
        {
            AssertCode("st.d\tr17,(r19,16392)", "33DE 0138");
        }

        [Test]
        public void CSkyDis_st_h_16()
        {
            AssertCode("st.h\tr2,(r4,31)", "5FAC");
        }

        [Test]
        public void CSkyDis_st_h_32()
        {
            AssertCode("st.h\tr17,(r19,4098)", "33DE 0118");
        }

        [Test]
        public void CSkyDis_st_w_16()
        {
            AssertCode("st.w\tr2,(r6,124)", "5FB6");
        }

        [Test]
        public void CSkyDis_st_w_sp()
        {
            AssertCode("st.w\tr2,(r14,380)", "5FBE");
        }

        [Test]
        public void CSkyDis_st_w_32()
        {
            AssertCode("st.w\tr17,(r19,8196)", "33DE 0128");
        }

        [Test]
        public void CSkyDis_stex_w()
        {
            AssertCode("stex.w\tr17,(r19,8196)", "33DE 0178");
        }

        [Test]
        public void CSkyDis_stop()
        {
            AssertCode("stop", "00C0 2048");
        }

        [Test]
        public void CSkyDis_str_b_a()
        {
            AssertCode("str.b\tr21,(r19,r17)", "33D6 3500");
        }

        [Test]
        public void CSkyDis_str_b_b()
        {
            AssertCode("str.b\tr21,(r19,r17<<1)", "33D6 5500");
        }

        [Test]
        public void CSkyDis_str_b_c()
        {
            AssertCode("str.b\tr21,(r19,r17<<2)", "33D6 9500");
        }

        [Test]
        public void CSkyDis_str_b_d()
        {
            AssertCode("str.b\tr21,(r19,r17<<3)", "33D6 1501");
        }

        [Test]
        public void CSkyDis_str_h()
        {
            AssertCode("str.h\tr21,(r19,r17)", "33D6 3504");
        }

        [Test]
        public void CSkyDis_str_w()
        {
            AssertCode("str.w\tr21,(r19,r17)", "33D6 3508");
        }

        [Test]
        public void CSkyDis_strap()
        {
            AssertCode("strap\tpsr,r0", "00C0 2078");
        }

        [Test]
        public void CSkyDis_subc_16()
        {
            AssertCode("subc\tr8,r9", "2762");
        }

        [Test]
        public void CSkyDis_subc_32()
        {
            AssertCode("subc\tr23,r19,r17", "33C6 1701");
        }

        [Test]
        public void CSkyDis_subi_16()
        {
            AssertCode("subi\tr0,0x1", "002A");
            AssertCode("subi\tr0,0x2", "012A");
            AssertCode("subi\tr7,0x100", "FF2A");
            AssertCode("subi\tr0,0x2", "012E");
        }

        [Test]
        public void CSkyDis_subi_16_a()
        {
            AssertCode("subi\tr3,r2,0x1", "635A");
            AssertCode("subi\tr3,r2,0x2", "675A");
            AssertCode("subi\tr3,r2,0x8", "7F5A");
        }

        [Test]
        public void CSkyDis_subi_32()
        {
            AssertCode("subi\tr21,r21,0x800", "B5E60018");
        }

        [Test]
        public void CSkyDis_subi_sp()
        {
            AssertCode("subi\tr14,r14,0x0", "2014");
            AssertCode("subi\tr14,r14,0x4", "2114");
            AssertCode("subi\tr14,r14,0x84", "2115");
            AssertCode("subi\tr14,r14,0x1FC", "3F17");
        }



        [Test]
        public void CSkyDis_subu_16_a()
        {
            AssertCode("subu\tr0,r1", "0660");
        }

        [Test]
        public void CSkyDis_subu_16_b()
        {
            AssertCode("subu\tr1,r3,r4", "315B");
        }

        [Test]
        public void CSkyDis_subu_32()
        {
            AssertCode("subu\tr23,r19,r17", "33C6 9700");
        }

        [Test]
        public void CSkyDis_sync()
        {
            AssertCode("sync\t0x1", "20C0 2004");
        }

        [Test]
        public void CSkyDis_trap()
        {
            AssertCode("trap\t0x1", "00C0 2024");
            AssertCode("trap\t0x3", "00C0 202C");
        }

        [Test]
        public void CSkyDis_tst_16()
        {
            AssertCode("tst\tr12,r0", "026B");
        }

        [Test]
        public void CSkyDis_tst_32()
        {
            AssertCode("tst\tr19,r17", "33C6 8020");
        }

        [Test]
        public void CSkyDis_tstnbz_16()
        {
            AssertCode("tstnbz\tr1", "0768");
        }

        [Test]
        public void CSkyDis_tstnbz_32()
        {
            AssertCode("tstnbz\tr19", "13C4 0021");
        }

        [Test]
        public void CSkyDis_vmulsh()
        {
            AssertCode("vmulsh\tr19,r17", "33C6 20B0");
        }

        [Test]
        public void CSkyDis_vmulsha()
        {
            AssertCode("vmulsha\tr19,r17", "33C6 40B0");
        }

        [Test]
        public void CSkyDis_vmulshs()
        {
            AssertCode("vmulshs\tr19,r17", "33C6 80B0");
        }

        [Test]
        public void CSkyDis_vmulsw()
        {
            AssertCode("vmulsw\tr19,r17", "33C6 20B4");
        }

        [Test]
        public void CSkyDis_vmulswa()
        {
            AssertCode("vmulswa\tr19,r17", "33C6 40B4");
        }

        [Test]
        public void CSkyDis_vmulsws()
        {
            AssertCode("vmulsws\tr19,r17", "33C6 80B4");
        }

        [Test]
        public void CSkyDis_wait()
        {
            AssertCode("wait", "00C0 204C");
        }

        [Test]
        public void CSkyDis_we()
        {
            AssertCode("we", "00C0 2054");
        }

        [Test]
        public void CSkyDis_xor_16()
        {
            AssertCode("xor\tr0,r1", "056C");
        }

        [Test]
        public void CSkyDis_xor_32()
        {
            AssertCode("xor\tr23,r19,r17", "33C6 5724");
        }

        [Test]
        public void CSkyDis_xori()
        {
            AssertCode("xori\tr17,r19,0x801", "33E6 0148");
        }

        [Test]
        public void CSkyDis_xsr()
        {
            AssertCode("xsr\tr3,r19,0x12", "33C6 034D");
        }

        [Test]
        public void CSkyDis_xtb0()
        {
            AssertCode("xtb0\tr21,r19", "13C4 3570");
        }

        [Test]
        public void CSkyDis_xtb1()
        {
            AssertCode("xtb1\tr21,r19", "13C4 5570");
        }

        [Test]
        public void CSkyDis_xtb2()
        {
            AssertCode("xtb2\tr21,r19", "13C4 9570");
        }

        [Test]
        public void CSkyDis_xtb3()
        {
            AssertCode("xtb3\tr21,r19", "13C4 1571");
        }

        [Test]
        public void CSkyDis_zext()
        {
            AssertCode("zext\tr16,r19,0x10,0x1C", "13C6 9557");
        }

        [Test]
        public void CSkyDis_zextb()
        {
            AssertCode("zextb\tr12,r4", "1077");
        }

        [Test]
        public void CSkyDis_zexth()
        {
            AssertCode("zexth\tr12,r4", "1177");
        }
    }
}
