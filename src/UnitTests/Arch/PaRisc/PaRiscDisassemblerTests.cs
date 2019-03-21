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
using Reko.Arch.PaRisc;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.PaRisc
{
    [TestFixture]
    public class PaRiscDisassemblerTests : DisassemblerTestBase<PaRiscInstruction>
    {
        private readonly PaRiscArchitecture arch;

        public PaRiscDisassemblerTests()
        {
            this.arch = new PaRiscArchitecture("paRisc");
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr32(0x00100000);


        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new BeImageWriter(bytes);
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void PaRiscDis_add()
        {
            AssertCode("add,*nuv\tr1,r7,r4", "08E18624");
        }


        [Test]
        public void PaRiscDis_break()
        {
            AssertCode("break\t00,0000", "00000000");
        }

        [Test]
        public void PaRiscDis_bl()
        {
            AssertCode("b,l\t001001F4,r2", "E800A3D8");
        }

        [Test]
        public void PaRiscDis_nop()
        {
            AssertCode("or\tr0,r0,r0", "08000240");
        }

        [Test]
        public void PaRiscDis_ldw()
        {
            AssertCode("ldw\t-24(r30),r2", "4BC23FD1");
        }

        [Test]
        public void PaRiscDis_bv_n()
        {
            AssertCode("bv,n\tr0(r2)", "E840D002");
        }

        [Test]
        public void PaRiscDis_ldx_short()
        {
            AssertCode("ldw\t4(sr0,r22),r19", "0EC41093");
        }

        [Test]
        public void PaRiscDis_ldsid()
        {
            AssertCode("ldsid\tr22,r1", "02C010A1");
        }

        [Test]
        public void PaRiscDis_mtsp()
        {
            AssertCode("mtsp\tr1,sr0", "00011820");
        }

        [Test]
        public void PaRiscDis_be()
        {
            AssertCode("be\t0(sr0,r22)", "E2C00000");
        }

        [Test]
        public void PaRiscDis_stw()
        {
            AssertCode("stw\tr2,-24(r30)", "6BC23FD1");
        }

        [Test]
        public void PaRiscDis_ldo()
        {
            AssertCode("ldo\t64(r30),r30", "37DE0080");
        }

        [Test]
        public void PaRiscDis_ldil()
        {
            AssertCode("ldil\t00012000,r31", "23E12000");
        }

        [Test]
        public void PaRiscDis_ble()
        {
            AssertCode("be,l\t7648(sr0,r31)", "E7E02EF0");
        }

        [Test]
        public void PaRiscDis_ldo_copy()
        {
            AssertCode("ldo\t0(r31),r2", "37E20000");
        }

        [Test]
        public void PaRiscDis_cmpb_ult()
        {
            AssertCode("cmpb,<<\tr7,r30,0010177C", "83C78EEC");
        }

        [Test]
        public void PaRiscDis_addib_64()
        {
            AssertCode("addib,*>=\t-10,r30,000FF7F0", "AFC1CFD5");
        }

        [Test]
        public void PaRiscDis_stb()
        {
            AssertCode("stb\tr17,2837(r11)", "61716B15");
        }

        [Test]
        public void PaRiscDis_sth()
        {
            AssertCode("sth\tr14,1024(r11)", "656e6400");
        }

        [Test]
        public void PaRiscDis_sth_neg()
        {
            AssertCode("sth\tr13,-264e(r11)", "656d6d65");
        }


        [Test]
        public void PaRiscDis_depwi()
        {
            AssertCode("depwi\t00,1F,00000003,r30", "d7c01c1d");
        }

        [Test]
        public void PaRiscDis_depwi_z()
        {
            AssertCode("depwi,z\t00,1F,00000003,r30", "d7c0181d");
        }

        [Test]
        public void PaRiscDis_fstw()
        {
            AssertCode("fstw\tfr0,-4(r27)", "27791200");
        }

        [Test]
        public void PaRiscDis_fldw()
        {
            AssertCode("fldw\t-4(r27),fr0", "27791000");
        }

        [Test]
        public void PaRiscDis_addi()
        {
            AssertCode("addi,tr\t+00000061,r0,r0", "b40010c2");
        }

        [Test]
        public void PaRiscDis_cmpb_ugt_n()
        {
            AssertCode("cmpb,>>,n\tr23,r30,0010003C", "8bd7a06a");
        }

        [Test]
        public void PaRiscDis_cmpb_ult_n()
        {
            AssertCode("cmpb,<<,n\tr23,r24,00100038", "83178062");
        }

        [Test]
        public void PaRiscDis_extrw_u()
        {
            AssertCode("extrw,u\tr5,0F,06,r6", "d0a619fa");
        }

        [Test]
        public void PaRiscDis_extrw_u_2()
        {
            AssertCode("extrw,u\tr31,1D,1E,r31", "D3FF1BA2");
        }


        [Test]
        public void PaRiscDis_addil()
        {
            AssertCode("addil\tL%55595000,r27,r1", "2B6AAAAA");
        }

        [Test]
        public void PaRiscDis_stw_ma()
        {
            AssertCode("stw,ma\tr3,128(r30)", "6fc30100");
        }

        [Test]
        public void PaRiscDis_stw_ma_negative_offset()
        {
            AssertCode("stw,ma\tr3,128(r30)", "6fc35555");
        }
        [Test]
        public void PaRiscDis_ldb()
        {
            AssertCode("ldb\t0(sr0,r31),r24", "0fe01018");
        }

        [Test]
        public void PaRiscDis_stb_disp()
        {
            AssertCode("stb\tr0,9(r25)", "0f201212");
        }

        [Test]
        public void PaRiscDis_shladd()
        {
            AssertCode("shladd\tr3,02,r31,r4", "0BE30A84");
        }

        [Test]
        public void PaRiscDis_ldw_regression()
        {
            AssertCode("ldw\t0(sr0,r4),r22", "0C801096");
        }

        [Test]
        public void PaRiscDis_ldw_mb()
        {
            AssertCode("ldw,mb\t-64(r30),r3", "4FC33F81");
        }

        [Test]
        public void PaRiscDis_27101005()
        {
            AssertCode("fldw\t8(r24),fr5", "27101005");
        }

        [Test]
        public void PaRiscDis_27101045()
        {
            AssertCode("fldw\t8(r24),fr5", "27101005");
        }

        [Test]
        public void PaRiscDis_fldw_R()
        {
            AssertCode("fldw\t-16(r24),fr7R", "27011047");
        }

        [Test]
        public void PaRiscDis_fldw_24000000()
        {
            AssertCode("fldw\tr0(r0),fr0", "24000000");
        }

        [Test]
        public void PaRiscDis_fldw_24504c54()
        {
            AssertCode("fldw\tr16(sr1,rp),fr20R", "24505054");
        }

        [Test]
        public void PaRiscDis_fmpy_dbl()
        {
            AssertCode("fmpy,dbl\tfr10,fr4,fr24", "31444e18");
        }

        [Test]
        public void PaRiscDis_fmpy_dbl_2()
        {
            AssertCode("fmpy_dbl\tfr10,fr4,fr24", "31444418");
        }

        [Test]
        public void PaRiscDis_fstw_R()
        {
            AssertCode("fstw\tfr26R,-12(r24)", "2709125a");
        }

        [Test]
        public void PaRiscDis_iitlbp()
        {
            AssertCode("iitlbp\tr18,(sr1,r8)", "05121200");
        }

        [Test]
        public void PaRiscDis_spop0_0_4_10000004()
        {
            AssertCode("spop0_0_4\t", "10000004");
        }

        [Test]
        public void PaRiscDis_ldb_long()
        {
            AssertCode("ldb\t2048(r0),r0", "40001000");
        }

        [Test]
        public void PaRiscDis_cmpb_n()
        {
            AssertCode("cmpb,n\tr0,r0,000FE00C", "8000000b");
        }

        [Test]
        public void PaRiscDis_copr_0_32d317_n_332d3037()
        {
            AssertCode("copr_0_32d317_n\t", "332d2d37");
        }

        [Test]
        public void PaRiscDis_ldb_42000000()
        {
            AssertCode("ldb\t0(r16),r0", "42000000");
        }

        [Test]
        public void PaRiscDis_copr_0_1312f7_n_31312e37()
        {
            AssertCode("copr_0_1312f7_n\t", "31313137");
        }

        [Test]
        public void PaRiscDis_copr_0_100000_31000000()
        {
            AssertCode("copr_0_100000\t", "31000000");
        }

        [Test]
        public void PaRiscDis_copr_0_232300_n_32323020()
        {
            AssertCode("copr_0_232300_n\t", "32323220");
        }

        [Test]
        public void PaRiscDis_stw_mb()
        {
            AssertCode("stw_mb\tr20,-4d8(r3)", "6c747451");
        }

        [Test]
        public void PaRiscDis_sth_64000000()
        {
            AssertCode("sth\tr0,0(r0)", "64000000");
        }

        [Test]
        public void PaRiscDis_cstw_5_24434f44()
        {
            AssertCode("cstw_5\tr4,r3(sr1,rp)", "24434344");
        }

        [Test]
        public void PaRiscDis_fmpyadd_dbl_1b8177db()
        {
            AssertCode("fmpyadd_dbl\tfr28,fpe2,fr27,fr31,fr14", "1b8181db");
        }


        [Test]
        public void PaRiscDis_add_c()
        {
            AssertCode("add,c\tr7,r16,r5", "0a070705");
        }

        [Test]
        public void PaRiscDis_fmpysub_sgl_99c4bb7d()
        {
            AssertCode("fmpysub_sgl\tfr30,fr20,fr29,fr29,fr23", "99c4c47d");
        }

        [Test]
        public void PaRiscDis_cldw_5_bc_24544558()
        {
            AssertCode("cldw_5_bc\tr20(sr1,rp),r24", "24545458");
        }

        [Test]
        public void PaRiscDis_cldd_7_2f1681d4()
        {
            AssertCode("cldd_7\tr22(sr2,r24),r20", "2f1616d4");
        }

        [Test]
        public void PaRiscDis_cldd_5_m_2f188176()
        {
            AssertCode("cldd_5_m\tr24(sr2,r24),r22", "2f181876");
        }

        [Test]
        public void PaRiscDis_uaddcm__ndc_086b99b6()
        {
            AssertCode("uaddcm__ndc\tr11,r3,r22", "086b6bb6");
        }

        [Test]
        public void PaRiscDis_add_c_ge()
        {
            AssertCode("add,c,>=\tr23,r18,r8", "0a575708");
        }

        [Test]
        public void PaRiscDis_cstw_3_o_bc()
        {
            AssertCode("cstw_3_o_bc\tr6,0(r24)", "271515e6");
        }

        [Test]
        public void PaRiscDis_movb()
        {
            AssertCode("movb\tr16,r18,00100848", "ca505080");
        }

        [Test]
        public void PaRiscDis_movib()
        {
            AssertCode("movib\t8,r7,0xffffe374", "ccf0f0d9");
        }


        [Test]
        public void PaRiscDis_fmpysub_sgl()
        {
            AssertCode("fmpysub_sgl\tfr26,fr21,fr31,fr21,fr24", "9b55556f");
        }

        [Test]
        public void PaRiscDis_fmpysub_dbl()
        {
            AssertCode("fmpysub_dbl\tfr26,fr21,fr7,fr22,fr8", "9b555587");
        }

        [Test]
        public void PaRiscDis_shrpd__ne_d0a9a573()
        {
            AssertCode("shrpd__<>\tr9,r5,52,r19", "d0a9a973");
        }

        [Test]
        public void PaRiscDis_ldcw_s_0d77a9d4()
        {
            AssertCode("ldcw_s\tr23(sr2,r11),r20", "0d7777d4");
        }

        [Test]
        public void PaRiscDis_addb_nuv_n()
        {
            AssertCode("addb_nuv_n\tr24,r11,0xffffe0f0", "a17878d3");
        }

        [Test]
        public void PaRiscDis_addib_eq()
        {
            AssertCode("addib,=\t-6,r24,0xfffffb78", "a71515e5");
        }

        [Test]
        public void PaRiscDis_spop0_0_c040()
        {
            AssertCode("spop0_0_c0400\t", "13010100");
        }

        [Test]
        public void PaRiscDis_ldd_m()
        {
            AssertCode("#\td00ffff", "0d0000ff");
        }

        [Test]
        public void PaRiscDis_fldd_s()
        {
            AssertCode("fldd_s\tr19(sr1,r19),fr0", "2e737300");
        }

        [Test]
        public void PaRiscDis_ldb_neg_disp()
        {
            AssertCode("ldb\t-34de(r26),r15", "434f4f45");
        }

        [Test]
        public void PaRiscDis_add_l()
        {
            AssertCode("add,l,<>\tr26,r1,r29", "083A3A1D");
        }

        [Test]
        public void PaRiscDis_subi_tsv()
        {
            AssertCode("subi,tsv,<>\t+00000280,r25,r29", "973d3d00");
        }

        [Test]
        public void PaRiscDis_ds()
        {
            AssertCode("ds\tr0,ret1,r0", "0ba0a040");
        }

        [Test]
        public void PaRiscDis_cmpiclr__le__()
        {
            AssertCode("cmpiclr__le_\t0,ret1,r0", "93a0a000");
        }

        [Test]
        public void PaRiscDis_bb_uge_n()
        {
            AssertCode("bb_uge_n\tr22,1e,0x10", "c7d6d612");
        }

        [Test]
        public void PaRiscDis_addil_neg()
        {
            AssertCode("addil\tL%-00080800,r27,r1", "2b7f7fff");
        }


        [Test]
        public void PaRiscDis_fcnvxf_sgl_dbl()
        {
            AssertCode("fcnvxf_sgl_dbl\tfr9R,fr10", "3920208a");
        }

        [Test]
        public void PaRiscDis_fcnvfxt_dbl_sgl()
        {
            AssertCode("fcnvfxt_dbl_sgl\tfr24,fr7", "33010107");
        }

        [Test]
        public void PaRiscDis_fcnvfxt()
        {
            AssertCode("fcnvfxt,dbl,sgl fr24, fr7", "33018a07");
        }

        [Test]
        public void PaRiscDis_xmpyu()
        {
            AssertCode("xmpyu\tfr9,fr5,fr26", "3925251a");
        }

        [Test]
        public void PaRiscDis_subi()
        {
            AssertCode("subi,>>\t+00000280,r29,r21", "97b5b500");
        }

        [Test]
        public void PaRiscDis_and_od()
        {
            AssertCode("and,od\tr2,r23,r18", "0AE2E212");
        }

        [Test]
        public void PaRiscDis_movb_ne_n()
        {
            AssertCode("movb,<>,n\tr0,r5,00100014", "C8A0A01A");
        }

        [Test]
        public void PaRiscDis_cmpb_gt()
        {
            AssertCode("cmpb,>\tr0,r3,00100020", "88606030");
        }

        [Test]
        public void PaRiscDis_subi_od()
        {
            AssertCode("subi,od\t+00000200,r31,r4", "97e4e400");
        }

        [Test]
        public void PaRiscDis_cmpb_ev()
        {
            AssertCode("cmpb,ev\tr3,r31,001001C4", "8be3e378");
        }

        [Test]
        public void PaRiscDis_mtctl()
        {
            AssertCode("mtctl\tr26,tr3", "037a7a40");
        }

        [Test]
        public void PaRiscDis_mfctl()
        {
            AssertCode("mfctl\ttr3,ret0", "036060bc");
        }

        [Test]
        public void PaRiscDis_b_l()
        {
            AssertCode("b,l\t0010005C,r0", "e80000a8");
        }

        [Test]
        public void PaRiscDis_sub_b()
        {
            AssertCode("sub,b,nsv\tr21,r22,r1", "0ad5d501");
        }

        [Test]
        public void PaRiscDis_cmpib_uge()
        {
            AssertCode("cmpib,>>=\tFFFFFFFF,r28,000FFFA0", "8F9F9F35");
        }

        [Test]
        public void PaRiscDis_stw_sp()
        {
            AssertCode("stw\tr2,-24(r30)", "6bc23fd1");
        }
    }
}
