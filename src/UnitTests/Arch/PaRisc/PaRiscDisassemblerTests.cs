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
        private PaRiscArchitecture arch;

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr32(0x00100000);

        private void AssertCode(string sExp, string hexBytes)
        {
            this.arch = new PaRiscArchitecture("paRisc");
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void AssertCode64(string sExp, string hexBytes)
        {
            this.arch = new PaRiscArchitecture("paRisc");
            arch.Options["WordSize"] = "64";
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
        public void PaRiscDis_ldw_long_offset()
        {
            AssertCode("ldw\t-24(r30),r2", "4BC23FD1");
        }

        [Test]
        public void PaRiscDis_ldb()
        {
            AssertCode("ldb\t1(r6),r17", "0CC23C11");
        }

        [Test]
        public void PaRiscDis_ldh()
        {
            AssertCode("ldh\t1(r6),r17", "0CC23C51");
        }


        [Test]
        public void PaRiscDis_ldw()
        {
            AssertCode("ldw\t1(r6),r17", "0CC23C91");
        }


        [Test]
        public void PaRiscDis_ldd()
        {
            AssertCode("ldd\t1(r6),r17", "0CC23CD1");
        }


        [Test]
        public void PaRiscDis_ldda()
        {
            AssertCode("ldda\t1(r6),r17", "0CC23D11");
        }


        [Test]
        public void PaRiscDis_ldcd()
        {
            AssertCode("ldcd\t1(r6),r17", "0CC23D51");
        }

        [Test]
        public void PaRiscDis_ldwa()
        {
            AssertCode("ldwa\t1(r6),r17", "0CC23D91");
        }

        [Test]
        public void PaRiscDis_ldcw()
        {
            AssertCode("ldcw\t1(r6),r17", "0CC23DD1");
        }

        [Test]
        public void PaRiscDis_stb()
        {
            AssertCode("stb\tr2,-8(r6)", "0CC23E11");
        }

        [Test]
        public void PaRiscDis_stb_long()
        {
            AssertCode("stb\tr17,-2678(r11)", "61716B15");
        }

        [Test]
        public void PaRiscDis_sth()
        {
            AssertCode("sth\tr2,-8(r6)", "0CC23E51");
        }

        [Test]
        public void PaRiscDis_sth_long()
        {
            AssertCode("sth\tr14,4608(r11)", "656e6400");
        }

        [Test]
        public void PaRiscDis_stw()
        {
            AssertCode("stw\tr2,-8(r6)", "0CC23E91");
        }

        [Test]
        public void PaRiscDis_stw_long()
        {
            AssertCode("stw\tr2,-24(r30)", "6BC23FD1");
        }

        [Test]
        public void PaRiscDis_std()
        {
            AssertCode("std\tr2,-8(r6)", "0CC23ED1");
        }

        [Test]
        public void PaRiscDis_stby_e()
        {
            AssertCode("stby,e\tr2,-8(r6)", "0CC23F11");
        }

        [Test]
        public void PaRiscDis_stdby_e()
        {
            AssertCode("stdby,e\tr2,-8(r6)", "0CC23F51");
        }

        [Test]
        public void PaRiscDis_stwa()
        {
            AssertCode("stwa\tr2,-8(r6)", "0CC23F91");
        }

        [Test]
        public void PaRiscDis_stda()
        {
            AssertCode("stda\tr2,-8(r6)", "0CC23FD1");
        }

        [Test]
        public void PaRiscDis_bv_n()
        {
            AssertCode("bv,n\tr0(r2)", "E840D002");
        }

        [Test]
        public void PaRiscDis_ldx_short()
        {
            AssertCode("ldw\t2(r22),r19", "0EC41093");
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
            AssertCode("be\t0(r22)", "E2C00000");
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
            AssertCode("be,l\t7648(r31)", "E7E02EF0");
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
            AssertCode64("addib,*>=\t-00000010,r30,000FF7F0", "AFC1CFD5");
        }

        [Test]
        public void PaRiscDis_sth_2000()
        {
            AssertCode64("sth\tr14,8192(r11)", "656E4000");
        }

        [Test]
        public void PaRiscDis_sth_4000()
        {
            AssertCode("sth\tr14,0(r11)", "656E8000");
        }

        [Test]
        public void PaRiscDis_sth_8000()
        {
            AssertCode("sth\tr14,-8192(r11)", "656E8001");
        }

        [Test]
        public void PaRiscDis_sth_8000_64()
        {
            AssertCode64("sth\tr14,-24576(r11)", "656E8001");
        }

        [Test]
        public void PaRiscRw_sth_64()
        {
            AssertCode64("sth\tr14,12800(r11)", "656e6400");
        }

        [Test]
        public void PaRiscDis_sth_neg()
        {
            AssertCode("sth\tr13,-2382(r11)", "656d6d65");
        }

        [Test]
        public void PaRiscDis_depwi()
        {
            AssertCode("depwi\t+00000003,1F,00000003,r30", "d7c61c1d");
        }

        [Test]
        public void PaRiscDis_depwi_z()
        {
            AssertCode("depwi,z\t+00000003,1F,00000003,r30", "d7c6181d");
        }

        [Test]
        public void PaRiscDis_fstw()
        {
            AssertCode("fstw\tfr0L,-4(r27)", "27791200");
        }

        [Test]
        public void PaRiscDis_fldw_L()
        {
            AssertCode("fldw\t-16(r24),fr7L", "27011007");
        }

        [Test]
        public void PaRiscDis_fldw_R()
        {
            AssertCode("fldw\t-16(r24),fr7R", "27011047");
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
        public void PaRiscDis_stw_mb_negative_offset()
        {
            AssertCode("stw,mb\tr3,-5464(r30)", "6fc35555");
        }

        [Test]
        public void PaRiscDis_stb_disp()
        {
            AssertCode("stb\tr0,9(r25)", "0F201212");
        }

        [Test]
        public void PaRiscDis_shladd()
        {
            AssertCode("shladd\tr3,02,r31,r4", "0BE30A84");
        }

        [Test]
        public void PaRiscDis_ldw_regression()
        {
            AssertCode("ldw\t0(r4),r22", "0C801096");
        }

        [Test]
        public void PaRiscDis_ldw_mb()
        {
            AssertCode("ldw,mb\t-64(r30),r3", "4FC33F81");
        }

        [Test]
        public void PaRiscDis_fldw_fr5L()
        {
            AssertCode("fldw\t8(r24),fr5L", "27101005");
        }

        [Test]
        public void PaRiscDis_fldw_fr5R()
        {
            AssertCode("fldw\t8(r24),fr5R", "27101045");
        }

        [Test]
        public void PaRiscDis_fldw_24000000()
        {
            AssertCode("fldw\tr0(r0),fr0L", "24000000");
        }

        [Test]
        public void PaRiscDis_fldw_24504c54()
        {
            AssertCode("fldw\t8(sr1,r2),fr20R", "24505054");
        }

        [Test]
        public void PaRiscDis_fmpy_dbl_0E()
        {
            AssertCode("fmpy,dbl\tfr10L,fr4L,fr24L", "39444618");
        }

        [Test]
        public void PaRiscDis_fmpy_dbl_0C()
        {
            AssertCode("fmpy,dbl\tfr10,fr4,fr24", "31444e18");
        }


        [Test]
        public void PaRiscDis_fstw_R()
        {
            AssertCode("fstw\tfr26R,-12(r24)", "2709125a");
        }

        [Test]
        public void PaRiscDis_pdtlb()
        {
            AssertCode("pdtlb\tr18(r8)", "05121200");
        }

        [Test]
        public void PaRiscDis_pdtlb_l()
        {
            AssertCode("pdtlb,l\tr18(r8)", "05121600");
        }

        [Test]
        public void PaRiscDis_spop0()
        {
            AssertCode("spop0\t00000007,0000001F", "100001DF");
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
        public void PaRiscDis_ldb_42000000()
        {
            AssertCode("ldb\t0(r16),r0", "42000000");
        }

        [Test]
        [Ignore("")]
        public void PaRiscDis_copr_0_32d317_n_332d3037()
        {
            AssertCode("copr_0_32d317_n\t", "332d2d37");
        }

        [Test]
        [Ignore("")]
        public void PaRiscDis_copr_0_1312f7_n_31312e37()
        {
            AssertCode("copr_0_1312f7_n\t", "31313137");
        }

        [Test]
        [Ignore("")]
        public void PaRiscDis_copr_0_100000_31000000()
        {
            AssertCode("copr_0_100000\t", "31000000");
        }

        [Test]
        [Ignore("")]
        public void PaRiscDis_copr_0_232300_n_32323020()
        {
            AssertCode("copr_0_232300_n\t", "32323220");
        }

        [Test]
        public void PaRiscDis_stw_mb()
        {
            AssertCode("stw,mb\tr20,-1496(r3)", "6c747451");
        }

        [Test]
        public void PaRiscDis_sth_64000000()
        {
            AssertCode("sth\tr0,0(r0)", "64000000");
        }



        [Test]
        public void PaRiscDis_fmpyadd_dbl_1b8177db()
        {
            AssertCode("fmpyadd,dbl\tfr28,fr1,fr27,fr7,fr16", "1b8181db");
        }

        [Test]
        public void PaRiscDis_add_c()
        {
            AssertCode("add,c\tr7,r16,r5", "0a070705");
        }

        [Test]
        public void PaRiscDis_fmpysub_sgl_99c4bb7d()
        {
            AssertCode("fmpysub,sgl\tfr30L,fr20L,fr29L,fr17L,fr24L", "99c4c47d");
        }

        [Test]
        public void PaRiscDis_fldd_sl()
        {
            AssertCode("fldd,sl\t12(r24),fr6", "2f181806");
        }

        [Test]
        public void PaRiscDis_fldw_sl()
        {
            AssertCode("fldw,sl\t10(sr1,r2),fr24R", "24545858");
        }

        [Test]
        public void PaRiscDis_cstw()
        {
            AssertCode("cstw,5\tr4,r3(sr1,r2)", "24434344");
        }

        [Test]
        public void PaRiscDis_cstd()
        {
            AssertCode("cstd,3,bc\tr20,11(r24)", "2f1616d4");
        }

        [Test]
        public void PaRiscDis_add_c_ge()
        {
            AssertCode("add,c,>=\tr23,r18,r8", "0a575708");
        }

        [Test]
        public void PaRiscDis_movb()
        {
            AssertCode("movb,<\tr16,r18,00100848", "ca505080");
        }

        [Test]
        public void PaRiscDis_movib()
        {
            AssertCode("movib,ev\t+00000008,r7,000FE874", "ccf0f0d9");
        }

        [Test]
        public void PaRiscDis_fmpysub_sgl()
        {
            AssertCode("fmpysub,sgl\tfr26L,fr21L,fr31L,fr21L,fr26L", "9b55556f");
        }

        [Test]
        public void PaRiscDis_fmpysub_dbl()
        {
            AssertCode("fmpysub,dbl\tfr26,fr21,fr7,fr22,fr10", "9b555587");
        }

        [Test]
        public void PaRiscDis_shrpw()
        {
            AssertCode("shrpw,*<>\tr9,r5,r19", "d0a9a973");
        }

        [Test]
        public void PaRiscDis_addb_le_n()
        {
            AssertCode("addb,<=,n\tr24,r11,000FEC70", "a17878d3");
        }

        [Test]
        public void PaRiscDis_addib()
        {
            AssertCode("addib\t-00000006,r24,000FFAF8", "a71515e5");
        }

        [Test]
        public void PaRiscDis_spop0_0_c040()
        {
            AssertCode("spop0\t00000004,00000000", "13010100");
        }

        [Test]
        public void PaRiscDis_ldd_m()
        {
            AssertCode("ldd,m\tr0(r8),r31", "0d0000ff");
        }

        [Test]
        public void PaRiscDis_fldd_s()
        {
            AssertCode("fldd,s\tr3(r19),fr0", "2E632000");
        }

        [Test]
        public void PaRiscDis_ldb_neg_disp()
        {
            AssertCode("ldb\t-6238(r26),r15", "434f4f45");
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
            AssertCode("ds\tr1,r2,r3", "08410443");
        }

        [Test]
        public void PaRiscDis_cmpiclr()
        {
            AssertCode("cmpiclr,<<=\t+00000000,r29,r0", "93a0a000");
        }

        [Test]
        public void PaRiscDis_bb_uge_n()
        {
            AssertCode("bb,>=,n\tr22,0000001E,00100B10", "c7d6d612");
        }

        [Test]
        public void PaRiscDis_addil_neg()
        {
            AssertCode("addil\tL%-00080800,r27,r1", "2b7f7fff");
        }

        [Test]
        public void PaRiscDis_fcnv_sgl_dbl_0E()
        {
            AssertCode("fcnv,sgl,dbl\tfr9L,fr10R", "392022ca");
        }

        [Test]
        public void PaRiscDis_fcnv_w_dbl_0E()
        {
            AssertCode("fcnv,w,dbl\tfr9L,fr10R", "3920a28a");
        }

        [Test]
        public void PaRiscDis_fcnv_sgl_dw_0E()
        {
            AssertCode("fcnv,sgl,dw\tfr9L,fr10R", "3921224a");
        }

        [Test]
        public void PaRiscDis_fcnvfxt_sgl_dw_0E()
        {
            AssertCode("fcnv,t,sgl,dw\tfr9L,fr10R", "3921a20a");
        }

        [Test]
        public void PaRiscDis_fcnvfxt()
        {
            AssertCode("fcnvfxt,dbl,w\tfr24,fr7", "33018a07");
        }

        [Test]
        public void PaRiscDis_fcnvff_sgl_sgl()
        {
            AssertCode("fcnvff,sgl,sgl\tfr8,fr0", "31000200");
        }

        [Test]
        public void PaRiscDis_fcnvff_dbl_sgl()
        {
            AssertCode("fcnvff,dbl,sgl\tfr8,fr0", "31000a00");
        }

        [Test]
        public void PaRiscDis_fcnvff_quad_sgl()
        {
            AssertCode("fcnvff,quad,sgl\tfr8,fr0", "31001a00");
        }

        [Test]
        public void PaRiscDis_fcnvff_sgl_dbl()
        {
            AssertCode("fcnvff,sgl,dbl\tfr8,fr0", "31002200");
        }

        [Test]
        public void PaRiscDis_fcnvff_dbl_dbl()
        {
            AssertCode("fcnvff,dbl,dbl\tfr8,fr0", "31002a00");
        }

        [Test]
        public void PaRiscDis_fcnvff_quad_dbl()
        {
            AssertCode("fcnvff,quad,dbl\tfr8,fr0", "31003a00");
        }

        [Test]
        public void PaRiscDis_fcnvxf_w_sgl()
        {
            AssertCode("fcnvxf,w,sgl\tfr8,fr0", "31008200");
        }

        [Test]
        public void PaRiscDis_fcnvfx_sgl_w()
        {
            AssertCode("fcnvfx,sgl,w\tfr8,fr0", "31010200");
        }

        [Test]
        public void PaRiscDis_fcnvfxt_sgl_w()
        {
            AssertCode("fcnvfxt,sgl,w\tfr8,fr0", "31018200");
        }

        [Test]
        public void PaRiscDis_fcnv_uw_sgl()
        {
            AssertCode("fcnv,uw,sgl\tfr8,fr0", "31028200");
        }

        [Test]
        public void PaRiscDis_fcnv_sgl_uw()
        {
            AssertCode("fcnv,sgl,uw\tfr8,fr0", "31030200");
        }

        [Test]
        public void PaRiscDis_fcnv_t_sgl_uw()
        {
            AssertCode("fcnv,t,sgl,uw\tfr8,fr0", "31038200");
        }


        [Test]
        public void PaRiscDis_xmpyu()
        {
            AssertCode("xmpyu\tfr18L,fr10L,fr26", "3925271A");
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
        public void PaRiscDis_cldw_7_ma()
        {
            AssertCode("cldw,7,ma\t-6(r24),r6", "271515e6");
        }

        [Test]
        public void PaRiscDis_cmpb_ev()
        {
            AssertCode("cmpb,ev\tr3,r31,001001C4", "8be3e378");
        }

        [Test]
        public void PaRiscDis_mtctl()
        {
            AssertCode("mtctl\tr26,tr3", "037A1840");
        }


        [Test]
        public void PaRiscDis_mfctl()
        {
            AssertCode("mfctl\trctr,r0", "000008A0");
            AssertCode("mfctl\tcr1,r0", "002008A0");
            AssertCode("mfctl\tcr2,r0", "004008A0");
            AssertCode("mfctl\tcr3,r0", "006008A0");
            AssertCode("mfctl\tcr4,r0", "008008A0");
            AssertCode("mfctl\tcr5,r0", "00A008A0");
            AssertCode("mfctl\tcr6,r0", "00C008A0");
            AssertCode("mfctl\tcr7,r0", "00E008A0");
            AssertCode("mfctl\tpidr1,r0", "010008A0");
            AssertCode("mfctl\tpidr2,r0", "012008A0");
            AssertCode("mfctl\tccr,r0", "014008A0");
            AssertCode("mfctl\tsar,r0", "016008A0");
            AssertCode("mfctl\tpidr3,r0", "018008A0");
            AssertCode("mfctl\tpidr4,r0", "01A008A0");
            AssertCode("mfctl\tiva,r0", "01C008A0");
            AssertCode("mfctl\teiem,r0", "01E008A0");
            AssertCode("mfctl\titmr,r0", "020008A0");
            AssertCode("mfctl\tpcsq,r0", "022008A0");
            AssertCode("mfctl\tpcoq,r0", "024008A0");
            AssertCode("mfctl\tiir,r0", "026008A0");
            AssertCode("mfctl\tisr,r0", "028008A0");
            AssertCode("mfctl\tior,r0", "02A008A0");
            AssertCode("mfctl\tipsw,r0", "02C008A0");
            AssertCode("mfctl\teirr,r0", "02E008A0");
            AssertCode("mfctl\ttr0,r0", "030008A0");
            AssertCode("mfctl\ttr1,r0", "032008A0");
            AssertCode("mfctl\ttr2,r0", "034008A0");
            AssertCode("mfctl\ttr3,r0", "036008A0");
            AssertCode("mfctl\ttr4,r0", "038008A0");
            AssertCode("mfctl\ttr5,r0", "03A008A0");
            AssertCode("mfctl\ttr6,r0", "03C008A0");
            AssertCode("mfctl\ttr7,r0", "03E008A0");
            AssertCode("mfctl,w\tsar,r0", "016048A0");
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

        [Test]
        public void PaRiscDis_andcm()
        {
            AssertCode("andcm\tr18,r4,r3", "08920003");
        }

        [Test]
        public void PaRiscDis_hsub_us()
        {
            AssertCode("hsub,us\tr18,r4,r3", "08920103");
        }

        [Test]
        public void PaRiscDis_hub_ss()
        {
            AssertCode("hsub,ss\tr18,r4,r3", "08920143");
        }

        [Test]
        public void PaRiscDis_hsub()
        {
            AssertCode("hsub\tr18,r4,r3", "089201c3");
        }

        [Test]
        public void PaRiscDis_and()
        {
            AssertCode("and\tr18,r4,r3", "08920203");
        }

        [Test]
        public void PaRiscDis_or()
        {
            AssertCode("or\tr18,r4,r3", "08920243");
        }

        [Test]
        public void PaRiscDis_xor()
        {
            AssertCode("xor\tr18,r4,r3", "08920283");
        }

        [Test]
        public void PaRiscDis_havg()
        {
            AssertCode("havg\tr18,r4,r3", "089202c3");
        }

        [Test]
        public void PaRiscDis_hadd_us()
        {
            AssertCode("hadd,us\tr18,r4,r3", "08920303");
        }

        [Test]
        public void PaRiscDis_hadd_ss()
        {
            AssertCode("hadd,ss\tr18,r4,r3", "08920343");
        }

        [Test]
        public void PaRiscDis_uxor()
        {
            AssertCode("uxor\tr18,r4,r3", "08920383");
        }

        [Test]
        public void PaRiscDis_hadd_r1()
        {
            AssertCode("hadd\tr18,r4,r3", "089203c3");
        }

        [Test]
        public void PaRiscDis_sub()
        {
            AssertCode("sub\tr18,r4,r3", "08920403");
        }

        [Test]
        public void PaRiscDis_sub64()
        {
            AssertCode64("sub,*<>\tr18,r4,r3", "08923423");
        }

        [Test]
        public void PaRiscDis_sub_tc()
        {
            AssertCode("sub,tc\tr18,r4,r3", "089204c3");
        }

        [Test]
        public void PaRiscDis_hshradd_1()
        {
            AssertCode("hshradd\tr18,+00000001,r4,r3", "08920543");
        }

        [Test]
        public void PaRiscDis_hshradd_2()
        {
            AssertCode("hshradd\tr18,+00000002,r4,r3", "08920583");
        }

        [Test]
        public void PaRiscDis_hshradd_3()
        {
            AssertCode("hshradd\tr18,+00000003,r4,r3", "089205c3");
        }

        [Test]
        public void PaRiscDis_add_r0()
        {
            AssertCode("add\tr18,r4,r3", "08920603");
        }

        [Test]
        public void PaRiscDis_shladd_1()
        {
            AssertCode("shladd\tr18,01,r4,r3", "08920643");
        }

        [Test]
        public void PaRiscDis_shladd_2()
        {
            AssertCode("shladd\tr18,02,r4,r3", "08920683");
        }

        [Test]
        public void PaRiscDis_shladd_3()
        {
            AssertCode("shladd\tr18,03,r4,r3", "089206c3");
        }

        [Test]
        public void PaRiscDis_hshladd_1()
        {
            AssertCode("hshladd\tr18,+00000001,r4,r3", "08920743");
        }

        [Test]
        public void PaRiscDis_hshladd_2()
        {
            AssertCode("hshladd\tr18,+00000002,r4,r3", "08920783");
        }

        [Test]
        public void PaRiscDis_addi_tc()
        {
            AssertCode("addi,tc,=\t+00000000,r25,r0", "B3202000");
        }
        
        [Test]
        public void PaRiscDis_depw_z()
        {
            AssertCode("depw,z\tr2,1A,0000001B,r19", "D66208A5");
        }

        [Test]
        public void PaRiscDis_fcpy_0C()
        {
            AssertCode("fcpy,dbl\tfr6L,fr14L", "3872502E");
        }

        [Test]
        public void PaRiscDis_shrpd()
        {
            AssertCode("shrpd,*<>\tr9,r5,00000034,r19", "D0A9A573");
        }

        [Test]
        public void PaRiscDis_addi_tsv()
        {
            AssertCode("addi,tsv,<=\t-0000018C,r31,r25", "B7F96CE9");
        }

        [Test]
        public void PaRiscDis_fsub_0C()
        {
            AssertCode("fsub,dbl\tfr9,fr22,fr17", "31362F31");
        }

        [Test]
        public void PaRiscDis_fid()
        {
            AssertCode("fid", "30000000");
        }

        [Test]
        public void PaRiscDis_fcpy_0E()
        {
            AssertCode("fcpy,dbl\tfr29R,fr20R", "3BC541D4");
        }

        [Test]
        public void PaRiscDis_diag()
        {
            AssertCode("diag\t+00008000", "14008000");
        }

        [Test]
        public void PaRiscDis_rfi()
        {
            AssertCode("rfi", "00002C00");
        }

        [Test]
        public void PaRiscDis_rfi_r()
        {
            AssertCode("rfi,r", "00002CA0");
        }

        [Test]
        public void PaRiscDis_mtsm()
        {
            AssertCode("mtsm\tr0", "00003860");
        }
    }
}
