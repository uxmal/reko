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
            AssertCode("add\tr1,r7,r4,tr", "08E18624");
        }


        // memMgmt
        [Test]
        [Ignore("Format is complex; try simpler ones first")]
        public void PaRiscDis_058C7910()
        {
            AssertCode("@@@", "058C7910");
        }


        [Test]
        public void PaRiscDis_break()
        {
            AssertCode("break\t00,0000", "00000000");
        }


        [Test]
        public void PaRiscDis_bl()
        {
            AssertCode("bl\t00101EC8", "E800A3D8");
        }

        [Test]
        public void PaRiscDis_nop()
        {
            AssertCode("or\tr0,r0,r0", "08000240");
        }

        [Test]
        public void PaRiscDis_ldw()
        {
            AssertCode("ldw\t-47(sr0,r30),r2", "4BC23FD1");
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
            AssertCode("stw\tr2,-47(sr0,r30)", "6BC23FD1");
        }

        [Test]
        public void PaRiscDis_ldo()
        {
            AssertCode("ldo\t128(r30),r30", "37DE0080");
        }

        [Test]
        public void PaRiscDis_ldil()
        {
            AssertCode("ldil\t00012000,r31", "23E12000");
        }

        [Test]
        public void PaRiscDis_ble()
        {
            AssertCode("ble\t7648(sr0,r31)", "E7E02EF0");
        }

        [Test]
        public void PaRiscDis_ldo_copy()
        {
            AssertCode("ldo\t0(r31),r2", "37E20000");
        }

        [Test]
        public void PaRiscDis_83C78EEC()
        {
            AssertCode("@@@", "83C78EEC");
        }

        [Test]
        public void PaRiscDis_addibf()
        {
            AssertCode("addibf\t+00000001,r30,00101FB4", "AFC1CFD5");
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
        public void PaRiscDis_iitlbp_05124000()
        {
            AssertCode("iitlbp\tr18,(sr1,r8)", "05121200");
        }

        [Test]
        public void PaRiscDis_ldb_40001e80()
        {
            AssertCode("ldb\tf40(r0),r0", "40000080");
        }

        [Test]
        public void PaRiscDis_spop0_0_4_10000004()
        {
            AssertCode("spop0_0_4\t", "10000004");
        }

        [Test]
        public void PaRiscDis_ldb_40001000()
        {
            AssertCode("ldb\t800(r0),r0", "40000000");
        }

        [Test]
        public void PaRiscDis_cmpb_n_8000000b()
        {
            AssertCode("cmpb_n\tr0,r0,0xffffe00c", "8000000b");
        }

        [Test]
        public void PaRiscDis_39323435_39323435()
        {
            AssertCode("#39323435\t", "39323235");
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
        public void PaRiscDis_copr_0_1312f6_n_31312e36()
        {
            AssertCode("copr_0_1312f6_n\t", "31313136");
        }

        [Test]
        public void PaRiscDis_ldw_mb_4d0eff52()
        {
            AssertCode("ldw_mb\t7fa9(r8),r14", "4d0e0e52");
        }

        [Test]
        public void PaRiscDis_ldh_446c6444()
        {
            AssertCode("ldh\t3222(r3),r12", "446c6c44");
        }

        [Test]
        public void PaRiscDis_stw_ma_6c384e67()
        {
            AssertCode("stw_ma\tr24,-38cd(r1)", "6c383867");
        }

        [Test]
        public void PaRiscDis_ldw_ma_4c6b5447()
        {
            AssertCode("ldw_ma\t-35dd(r3),r11", "4c6b6b47");
        }

        [Test]
        public void PaRiscDis_sth_64333166()
        {
            AssertCode("sth\tr19,18b3(r1)", "64333366");
        }

        [Test]
        public void PaRiscDis_copr_4_238612_n_32386132()
        {
            AssertCode("copr_4_238612_n\t", "32383832");
        }

        [Test]
        public void PaRiscDis_stw_ma_6e6e5872()
        {
            AssertCode("stw_ma\tr14,2c39(r19)", "6e6e6e72");
        }

        [Test]
        public void PaRiscDis_addil_2b000000()
        {
            AssertCode("addil\tL%0,r24,r1", "2b000000");
        }

        [Test]
        public void PaRiscDis_ldb_40282329()
        {
            AssertCode("ldb\t-e6c(r1),r8", "40282829");
        }

        [Test]
        public void PaRiscDis_cldw_5_sm_bc_24526576()
        {
            AssertCode("cldw_5_sm_bc\tr18(sr1,rp),r22", "24525276");
        }

        [Test]
        public void PaRiscDis_stw_mb_6e3a2039()
        {
            AssertCode("stw_mb\tr26,-fe4(r17)", "6e3a3a39");
        }

        [Test]
        public void PaRiscDis_copr_4_234353_n_32343533()
        {
            AssertCode("copr_4_234353_n\t", "32343433");
        }

        [Test]
        public void PaRiscDis_cstd_4_o_bc_2d303720()
        {
            AssertCode("cstd_4_o_bc\tr0,0(r9)", "2d303020");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c696e6b()
        {
            AssertCode("stw_mb\tr9,-28cb(r3)", "6c69696b");
        }

        [Test]
        public void PaRiscDis_sth_6572206c()
        {
            AssertCode("sth\tr18,1036(r11)", "6572726c");
        }

        [Test]
        public void PaRiscDis_copr_4_12e371_n_312e3731()
        {
            AssertCode("copr_4_12e371_n\t", "312e2e31");
        }

        [Test]
        public void PaRiscDis_copr_0_232300_n_32323020()
        {
            AssertCode("copr_0_232300_n\t", "32323220");
        }

        [Test]
        public void PaRiscDis_fldw_24000000()
        {
            AssertCode("fldw\tr0(r0),fr0", "24000000");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c743651()
        {
            AssertCode("stw_mb\tr20,-4d8(r3)", "6c747451");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c483161()
        {
            AssertCode("stw_mb\tr8,-750(rp)", "6c484861");
        }

        [Test]
        public void PaRiscDis_stw_mb_6f646f63()
        {
            AssertCode("stw_mb\tr4,-284f(dp)", "6f646463");
        }

        [Test]
        public void PaRiscDis_sth_6747464c()
        {
            AssertCode("sth\tr7,2326(r26)", "6747474c");
        }

        [Test]
        public void PaRiscDis_384c7539_384c7539()
        {
            AssertCode("#384c7539\t", "384c4c39");
        }

        [Test]
        public void PaRiscDis_copr_0_241338_n_32413238()
        {
            AssertCode("copr_0_241338_n\t", "32414138");
        }

        [Test]
        public void PaRiscDis_ldw_ma_4d4d5a36()
        {
            AssertCode("ldw_ma\t2d1b(r10),r13", "4d4d4d36");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c336e79()
        {
            AssertCode("stw_mb\tr19,-28c4(r1)", "6c333379");
        }

        [Test]
        public void PaRiscDis_sth_64000000()
        {
            AssertCode("sth\tr0,0(r0)", "64000000");
        }

        [Test]
        public void PaRiscDis_3f200000_3f200000()
        {
            AssertCode("#3f200000\t", "3f202000");
        }

        [Test]
        public void PaRiscDis_c0000800_c0000800()
        {
            AssertCode("#c0000800\t", "c0000000");
        }

        [Test]
        public void PaRiscDis__000068b8()
        {
            AssertCode("#\t68b8", "000000b8");
        }

        [Test]
        public void PaRiscDis_3e280000_3e280000()
        {
            AssertCode("#3e280000\t", "3e282800");
        }

        [Test]
        public void PaRiscDis_3e280800_3e280800()
        {
            AssertCode("#3e280800\t", "3e282800");
        }

        [Test]
        public void PaRiscDis_3e280a00_3e280a00()
        {
            AssertCode("#3e280a00\t", "3e282800");
        }

        [Test]
        public void PaRiscDis_ldb_40001008()
        {
            AssertCode("ldb\t804(r0),r0", "40000008");
        }

        [Test]
        public void PaRiscDis_3e281000_3e281000()
        {
            AssertCode("#3e281000\t", "3e282800");
        }

        [Test]
        public void PaRiscDis_3e281800_3e281800()
        {
            AssertCode("#3e281800\t", "3e282800");
        }

        [Test]
        public void PaRiscDis_ldb_40001c38()
        {
            AssertCode("ldb\te1c(r0),r0", "40000038");
        }

        [Test]
        public void PaRiscDis_3e282600_3e282600()
        {
            AssertCode("#3e282600\t", "3e282800");
        }

        [Test]
        public void PaRiscDis_ldb_40001c60()
        {
            AssertCode("ldb\te30(r0),r0", "40000060");
        }

        [Test]
        public void PaRiscDis_3e282700_3e282700()
        {
            AssertCode("#3e282700\t", "3e282800");
        }

        [Test]
        public void PaRiscDis_ldb_40001e50()
        {
            AssertCode("ldb\tf28(r0),r0", "40000050");
        }

        [Test]
        public void PaRiscDis_3e282800_3e282800()
        {
            AssertCode("#3e282800\t", "3e282800");
        }

        [Test]
        public void PaRiscDis_ldb_40001e70()
        {
            AssertCode("ldb\tf38(r0),r0", "40000070");
        }

        [Test]
        public void PaRiscDis_3e285000_3e285000()
        {
            AssertCode("#3e285000\t", "3e282800");
        }

        [Test]
        public void PaRiscDis_fstqs_3e285200()
        {
            AssertCode("fstqs\tfr0,4(sr1,r17)", "3e282800");
        }

        [Test]
        public void PaRiscDis_ldb_40001e88()
        {
            AssertCode("ldb\tf44(r0),r0", "40000088");
        }

        [Test]
        public void PaRiscDis_fstw_24505249()
        {
            AssertCode("fstw\tfr9R,8(sr1,rp)", "24505049");
        }

        [Test]
        public void PaRiscDis_fldw_24444c54()
        {
            AssertCode("fldw\tr4(sr1,rp),fr20R", "24444454");
        }

        [Test]
        public void PaRiscDis_fldw_24504c54()
        {
            AssertCode("fldw\tr16(sr1,rp),fr20R", "24505054");
        }

        [Test]
        public void PaRiscDis_fldw_sl_2453484c()
        {
            AssertCode("fldw_sl\tr19(sr1,rp),fr12R", "2453534c");
        }

        [Test]
        public void PaRiscDis_ldb_41544124()
        {
            AssertCode("ldb\t2092(r10),r20", "41545424");
        }

        [Test]
        public void PaRiscDis_cldw_5_bc_24544558()
        {
            AssertCode("cldw_5_bc\tr20(sr1,rp),r24", "24545458");
        }

        [Test]
        public void PaRiscDis_ldw_ma_4e464f24()
        {
            AssertCode("ldw_ma\t2792(r18),r6", "4e464624");
        }

        [Test]
        public void PaRiscDis_cldw_5_24444154()
        {
            AssertCode("cldw_5\tr4(sr1,rp),r20", "24444454");
        }

        [Test]
        public void PaRiscDis_ldb_415f5354()
        {
            AssertCode("ldb\t29aa(r10),r31", "415f5f54");
        }

        [Test]
        public void PaRiscDis_ldb_41525424()
        {
            AssertCode("ldb\t2a12(r10),r18", "41525224");
        }

        [Test]
        public void PaRiscDis_cldw_5_sl_244d494c()
        {
            AssertCode("cldw_5_sl\tr13(sr1,rp),r12", "244d4d4c");
        }

        [Test]
        public void PaRiscDis_ldw_ma_4c49434f()
        {
            AssertCode("ldw_ma\t-3e59(rp),r9", "4c49494f");
        }

        [Test]
        public void PaRiscDis_ldh_44452400()
        {
            AssertCode("ldh\t1200(rp),r5", "44454500");
        }

        [Test]
        public void PaRiscDis_cstw_5_24434f44()
        {
            AssertCode("cstw_5\tr4,r3(sr1,rp)", "24434344");
        }

        [Test]
        public void PaRiscDis_ldh_45240000()
        {
            AssertCode("ldh\t0(r9),r4", "45242400");
        }

        [Test]
        public void PaRiscDis_cldw_5_sl_244c4954()
        {
            AssertCode("cldw_5_sl\tr12(sr1,rp),r20", "244c4c54");
        }

        [Test]
        public void PaRiscDis_fstw_24554e57()
        {
            AssertCode("fstw\tfr23R,r21(sr1,rp)", "24555557");
        }

        [Test]
        public void PaRiscDis_ldw_ma_4d494c4c()
        {
            AssertCode("ldw_ma\t2626(r10),r9", "4d49494c");
        }

        [Test]
        public void PaRiscDis_ldh_454e4424()
        {
            AssertCode("ldh\t2212(r10),r14", "454e4e24");
        }

        [Test]
        public void PaRiscDis_cldw_5_bc_24524543()
        {
            AssertCode("cldw_5_bc\tr18(sr1,rp),r3", "24525243");
        }

        [Test]
        public void PaRiscDis_ldw_ma_4f564552()
        {
            AssertCode("ldw_ma\t22a9(r26),r22", "4f565652");
        }

        [Test]
        public void PaRiscDis_fstw_bc_24504641()
        {
            AssertCode("fstw_bc\tfpe3,r16(sr1,rp)", "24505041");
        }

        [Test]
        public void PaRiscDis_ldw_ma_4e544552()
        {
            AssertCode("ldw_ma\t22a9(r18),r20", "4e545452");
        }

        [Test]
        public void PaRiscDis_ldb_41240000()
        {
            AssertCode("ldb\t0(r9),r4", "41242400");
        }

        [Test]
        public void PaRiscDis_fldw_sl_2453484f()
        {
            AssertCode("fldw_sl\tr19(sr1,rp),fr15R", "2453534f");
        }

        [Test]
        public void PaRiscDis_fldw_24474c4f()
        {
            AssertCode("fldw\tr7(sr1,rp),fr15R", "2447474f");
        }

        [Test]
        public void PaRiscDis_ldb_42414c24()
        {
            AssertCode("ldb\t2612(r18),r1", "42414124");
        }

        [Test]
        public void PaRiscDis_cstw_5_o_24425353()
        {
            AssertCode("cstw_5_o\tr19,0(sr1,rp)", "24424253");
        }

        [Test]
        public void PaRiscDis__058c7910()
        {
            AssertCode("#\t58c7910", "058c8c10");
        }

        [Test]
        public void PaRiscDis_3bc541d4_3bc541d4()
        {
            AssertCode("#3bc541d4\t", "3bc5c5d4");
        }

        [Test]
        public void PaRiscDis_388b9a76_388b9a76()
        {
            AssertCode("#388b9a76\t", "388b8b76");
        }

        [Test]
        public void PaRiscDis_cmpb_lsh_83c78eec()
        {
            AssertCode("cmpb_<<\tr7,sp,0x177c", "83c7c7ec");
        }

        [Test]
        public void PaRiscDis__0d77a9a4()
        {
            AssertCode("#\td77a9a4", "0d7777a4");
        }

        [Test]
        public void PaRiscDis_cmpb_le_n_83c67266()
        {
            AssertCode("cmpb__le__n\tr6,sp,0x1938", "83c6c666");
        }

        [Test]
        public void PaRiscDis_cmpib_gt_8d1978d0()
        {
            AssertCode("cmpib_>\t-4,r8,0xc70", "8d1919d0");
        }

        [Test]
        public void PaRiscDis_cmpb_le_n_83c36bc7()
        {
            AssertCode("cmpb__le__n\tr3,sp,0xfffff5e8", "83c3c3c7");
        }

        [Test]
        public void PaRiscDis_cmpb_le_n_83c66836()
        {
            AssertCode("cmpb__le__n\tr6,sp,0x1420", "83c6c636");
        }

        [Test]
        public void PaRiscDis_cmpb_ule_831bb280()
        {
            AssertCode("cmpb_<_le_\tdp,r24,0x948", "831b1b80");
        }

        [Test]
        public void PaRiscDis_fmpyadd_dbl_1b8177db()
        {
            AssertCode("fmpyadd_dbl\tfr28,fpe2,fr27,fr31,fr14", "1b8181db");
        }

        [Test]
        public void PaRiscDis_cldd_5_m_2f188176()
        {
            AssertCode("cldd_5_m\tr24(sr2,r24),r22", "2f181876");
        }

        [Test]
        public void PaRiscDis_and_NULL__0a07b205()
        {
            AssertCode("and_NULL_\tr7,r16,r5", "0a070705");
        }

        [Test]
        public void PaRiscDis_sth_671536e7()
        {
            AssertCode("sth\tr21,-48d(r24)", "671515e7");
        }

        [Test]
        public void PaRiscDis_fmpysub_sgl_99c4bb7d()
        {
            AssertCode("fmpysub_sgl\tfr30,fr20,fr29,fr29,fr23", "99c4c47d");
        }

        [Test]
        public void PaRiscDis_cldd_7_2f1681d4()
        {
            AssertCode("cldd_7\tr22(sr2,r24),r20", "2f1616d4");
        }

        [Test]
        public void PaRiscDis_cmpb__le__80047bc4()
        {
            AssertCode("cmpb__le_\tr4,r0,0x1de8", "800404c4");
        }

        [Test]
        public void PaRiscDis_uaddcm__ndc_086b99b6()
        {
            AssertCode("uaddcm__ndc\tr11,r3,r22", "086b6bb6");
        }

        [Test]
        public void PaRiscDis_add_l_nsv_0a57da08()
        {
            AssertCode("add_l_nsv\tr23,r18,r8", "0a575708");
        }

        [Test]
        public void PaRiscDis_add_dc___0af80729()
        {
            AssertCode("add_dc__\tr24,r23,r9", "0af8f829");
        }

        [Test]
        public void PaRiscDis_cstw_3_o_bc_271536e6()
        {
            AssertCode("cstw_3_o_bc\tr6,0(r24)", "271515e6");
        }

        [Test]
        public void PaRiscDis_movb_ca500580()
        {
            AssertCode("movb\tr16,r18,0x2c8", "ca505080");
        }

        [Test]
        public void PaRiscDis_movib_ccf006d9()
        {
            AssertCode("movib\t8,r7,0xffffe374", "ccf0f0d9");
        }

        [Test]
        public void PaRiscDis_ldd_0e6b1ccb()
        {
            AssertCode("ldd\t-b(r19),r11", "0e6b6bcb");
        }

        [Test]
        public void PaRiscDis_stw_ma_6c7444b8()
        {
            AssertCode("stw_ma\tr20,225c(r3)", "6c7474b8");
        }

        [Test]
        public void PaRiscDis_3872502e_3872502e()
        {
            AssertCode("#3872502e\t", "3872722e");
        }

        [Test]
        public void PaRiscDis_fmpysub_sgl_9b55456f()
        {
            AssertCode("fmpysub_sgl\tfr26,fr21,fr31,fr21,fr24", "9b55556f");
        }

        [Test]
        public void PaRiscDis_fmpysub_dbl_9b554587()
        {
            AssertCode("fmpysub_dbl\tfr26,fr21,fr7,fr22,fr8", "9b555587");
        }

        [Test]
        public void PaRiscDis_3c4e7ce9_3c4e7ce9()
        {
            AssertCode("#3c4e7ce9\t", "3c4e4ee9");
        }

        [Test]
        public void PaRiscDis_389ebae5_389ebae5()
        {
            AssertCode("#389ebae5\t", "389e9ee5");
        }

        [Test]
        public void PaRiscDis_shrpd__ne_d0a9a573()
        {
            AssertCode("shrpd__<>\tr9,r5,52,r19", "d0a9a973");
        }

        [Test]
        public void PaRiscDis__0661c0b6()
        {
            AssertCode("#\t661c0b6", "066161b6");
        }

        [Test]
        public void PaRiscDis_addi_tsv_le_b7f96ce9()
        {
            AssertCode("addi_tsv__le_\t-18c,r31,r25", "b7f9f9e9");
        }

        [Test]
        public void PaRiscDis__0dcbc5bf()
        {
            AssertCode("#\tdcbc5bf", "0dcbcbbf");
        }

        [Test]
        public void PaRiscDis_cmpb_le_83ff8eec()
        {
            AssertCode("cmpb_<<\tr31,r31,0x177c", "83ffffec");
        }

        [Test]
        public void PaRiscDis_ldcw_s_0d77a9d4()
        {
            AssertCode("ldcw_s\tr23(sr2,r11),r20", "0d7777d4");
        }

        [Test]
        public void PaRiscDis_cmpb_le_n_83fe7266()
        {
            AssertCode("cmpb__le__n\tsp,r31,0x1938", "83fefe66");
        }

        [Test]
        public void PaRiscDis_cmpib_gt_8d197b50()
        {
            AssertCode("cmpib_>\t-4,r8,0xdb0", "8d191950");
        }

        [Test]
        public void PaRiscDis_cmpb_le_n_83fb6bc7()
        {
            AssertCode("cmpb__le__n\tdp,r31,0xfffff5e8", "83fbfbc7");
        }

        [Test]
        public void PaRiscDis_cmpb_le_n_83fe6836()
        {
            AssertCode("cmpb__le__n\tsp,r31,0x1420", "83fefe36");
        }

        [Test]
        public void PaRiscDis_ead766c3_ead766c3()
        {
            AssertCode("#ead766c3\t", "ead7d7c3");
        }

        [Test]
        public void PaRiscDis_cmpb_ule_831bb2f0()
        {
            AssertCode("cmpb_<_le_\tdp,r24,0x980", "831b1bf0");
        }

        [Test]
        public void PaRiscDis_addb__uge_a8c1cfd5()
        {
            AssertCode("addb__>=\tr1,r6,0xfffff7f0", "a8c1c1d5");
        }

        [Test]
        public void PaRiscDis_addb__uge_a8c1cf75()
        {
            AssertCode("addb__>=\tr1,r6,0xfffff7c0", "a8c1c175");
        }

        [Test]
        public void PaRiscDis_cldd_4_2f188106()
        {
            AssertCode("cldd_4\tr24(sr2,r24),r6", "2f181806");
        }

        [Test]
        public void PaRiscDis_addb_nuv_n_a17881d3()
        {
            AssertCode("addb_nuv_n\tr24,r11,0xffffe0f0", "a17878d3");
        }

        [Test]
        public void PaRiscDis_addb_nuv_a1788171()
        {
            AssertCode("addb_nuv\tr24,r11,0xffffe0c0", "a1787871");
        }

        [Test]
        public void PaRiscDis_cmpb_ugt_n_8a07b206()
        {
            AssertCode("cmpb_>>_n\tr7,r16,0x1908", "8a070706");
        }

        [Test]
        public void PaRiscDis_sth_67153697()
        {
            AssertCode("sth\tr21,-4b5(r24)", "67151597");
        }

        [Test]
        public void PaRiscDis_cldd_6_m_2f1681a4()
        {
            AssertCode("cldd_6_m\tr22(sr2,r24),r4", "2f1616a4");
        }

        [Test]
        public void PaRiscDis_addb_eq_n_a26a29a7()
        {
            AssertCode("addb_=_n\tr10,r19,0xfffff4d8", "a26a6aa7");
        }

        [Test]
        public void PaRiscDis_cmpb_uge_886b99b5()
        {
            AssertCode("cmpb_>>=\tr11,r3,0xfffffce0", "886b6bb5");
        }

        [Test]
        public void PaRiscDis_cmpb_nsv_n_8a57da0b()
        {
            AssertCode("cmpb_nsv_n\tr23,r18,0xffffed0c", "8a57570b");
        }

        [Test]
        public void PaRiscDis_addb__eq_a178bccd()
        {
            AssertCode("addb__=\tr24,r11,0xfffffe6c", "a17878cd");
        }

        [Test]
        public void PaRiscDis_addib_eq_a71536e5()
        {
            AssertCode("addib_=\t-6,r24,0xfffffb78", "a71515e5");
        }

        [Test]
        public void PaRiscDis_movib_cd500580()
        {
            AssertCode("movib\t8,r10,0x2c8", "cd505080");
        }

        [Test]
        public void PaRiscDis_addb__lt_a178c2f1()
        {
            AssertCode("addb__<\tr24,r11,0xffffe180", "a17878f1");
        }

        [Test]
        public void PaRiscDis_movib_ccc806d9()
        {
            AssertCode("movib\t4,r6,0xffffe374", "ccc8c8d9");
        }

        [Test]
        public void PaRiscDis_spop0_0_c0400_13010000()
        {
            AssertCode("spop0_0_c0400\t", "13010100");
        }

        [Test]
        public void PaRiscDis_ldb_40001eb0()
        {
            AssertCode("ldb\tf58(r0),r0", "400000b0");
        }

        [Test]
        public void PaRiscDis__0700ffff()
        {
            AssertCode("#\t700ffff", "070000ff");
        }

        [Test]
        public void PaRiscDis_ldb_40001eac()
        {
            AssertCode("ldb\tf56(r0),r0", "400000ac");
        }

        [Test]
        public void PaRiscDis_ldb_40001ea8()
        {
            AssertCode("ldb\tf54(r0),r0", "400000a8");
        }

        [Test]
        public void PaRiscDis_ldb_40001ea4()
        {
            AssertCode("ldb\tf52(r0),r0", "400000a4");
        }

        [Test]
        public void PaRiscDis_ldb_40001ea0()
        {
            AssertCode("ldb\tf50(r0),r0", "400000a0");
        }

        [Test]
        public void PaRiscDis_ldb_40001010()
        {
            AssertCode("ldb\t808(r0),r0", "40000010");
        }

        [Test]
        public void PaRiscDis_ldb_4000100c()
        {
            AssertCode("ldb\t806(r0),r0", "4000000c");
        }

        [Test]
        public void PaRiscDis_ldb_40001ee0()
        {
            AssertCode("ldb\tf70(r0),r0", "400000e0");
        }

        [Test]
        public void PaRiscDis_ldb_40001018()
        {
            AssertCode("ldb\t80c(r0),r0", "40000018");
        }

        [Test]
        public void PaRiscDis_ldb_40001e40()
        {
            AssertCode("ldb\tf20(r0),r0", "40000040");
        }

        [Test]
        public void PaRiscDis__0d00ffff()
        {
            AssertCode("#\td00ffff", "0d0000ff");
        }

        [Test]
        public void PaRiscDis_cldd_5_sm_2f746d70()
        {
            AssertCode("cldd_5_sm\tr20(sr1,dp),r16", "2f747470");
        }

        [Test]
        public void PaRiscDis_cldd_5_sm_sl_2f6c6962()
        {
            AssertCode("cldd_5_sm_sl\tr12(sr1,dp),rp", "2f6c6c62");
        }

        [Test]
        public void PaRiscDis_cstd_4_o_bc_2d363733()
        {
            AssertCode("cstd_4_o_bc\tr19,0(r9)", "2d363633");
        }

        [Test]
        public void PaRiscDis_copr_4_133392_n_31333932()
        {
            AssertCode("copr_4_133392_n\t", "31333332");
        }

        [Test]
        public void PaRiscDis_stw_ma_6c65002f()
        {
            AssertCode("stw_ma\tr5,-1fe9(r3)", "6c65652f");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c6f6361()
        {
            AssertCode("stw_mb\tr15,-2e50(r3)", "6c6f6f61");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c2f6c69()
        {
            AssertCode("stw_mb\tr15,-29cc(r1)", "6c2f2f69");
        }

        [Test]
        public void PaRiscDis_stw_mb_6f63616c()
        {
            AssertCode("stw_mb\tr3,30b6(dp)", "6f63636c");
        }

        [Test]
        public void PaRiscDis_stw_mb_6d616769()
        {
            AssertCode("stw_mb\tr1,-2c4c(r11)", "6d616169");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c69622f()
        {
            AssertCode("stw_mb\tr9,-2ee9(r3)", "6c69692f");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c696272()
        {
            AssertCode("stw_mb\tr9,3139(r3)", "6c696972");
        }

        [Test]
        public void PaRiscDis_sth_65676578()
        {
            AssertCode("sth\tr7,32bc(r11)", "65676778");
        }

        [Test]
        public void PaRiscDis_fldd_s_2e736c00()
        {
            AssertCode("fldd_s\tr19(sr1,r19),fr0", "2e737300");
        }

        [Test]
        public void PaRiscDis_cstd_5_o_2f757372()
        {
            AssertCode("cstd_5_o\tr18,0(sr1,dp)", "2f757572");
        }

        [Test]
        public void PaRiscDis_cstd_5_sm_2f6c6f63()
        {
            AssertCode("cstd_5_sm\tr3,r12(sr1,dp)", "2f6c6c63");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c696263()
        {
            AssertCode("stw_mb\tr9,-2ecf(r3)", "6c696963");
        }

        [Test]
        public void PaRiscDis_cldd_1_2e32005f()
        {
            AssertCode("cldd_1\tr18(r17),r31", "2e32325f");
        }

        [Test]
        public void PaRiscDis_sth_65707600()
        {
            AssertCode("sth\tr16,3b00(r11)", "65707000");
        }

        [Test]
        public void PaRiscDis_sth_6572726e()
        {
            AssertCode("sth\tr18,3937(r11)", "6572726e");
        }

        [Test]
        public void PaRiscDis_stw_ma_6f005f5f()
        {
            AssertCode("stw_ma\tr0,-3051(r24)", "6f00005f");
        }

        [Test]
        public void PaRiscDis_stw_ma_6c645f6c()
        {
            AssertCode("stw_ma\tr4,2fb6(r3)", "6c64646c");
        }

        [Test]
        public void PaRiscDis_stw_ma_6f63005f()
        {
            AssertCode("stw_ma\tr3,-1fd1(dp)", "6f63635f");
        }

        [Test]
        public void PaRiscDis_ldh_454d5f49()
        {
            AssertCode("ldh\t-305c(r10),r13", "454d4d49");
        }

        [Test]
        public void PaRiscDis_ldh_44005f43()
        {
            AssertCode("ldh\t-305f(r0),r0", "44000043");
        }

        [Test]
        public void PaRiscDis_ldh_45564953()
        {
            AssertCode("ldh\t-3b57(r10),r22", "45565653");
        }

        [Test]
        public void PaRiscDis_ldh_454c005f()
        {
            AssertCode("ldh\t-1fd1(r10),r12", "454c4c5f");
        }

        [Test]
        public void PaRiscDis_sth_656e6400()
        {
            AssertCode("sth\tr14,3200(r11)", "656e6e00");
        }

        [Test]
        public void PaRiscDis_ldb_434f5645()
        {
            AssertCode("ldb\t-34de(r26),r15", "434f4f45");
        }

        [Test]
        public void PaRiscDis_ldh_44002452()
        {
            AssertCode("ldh\t1229(r0),r0", "44000052");
        }

        [Test]
        public void PaRiscDis_ldh_45434f56()
        {
            AssertCode("ldh\t27ab(r10),r3", "45434356");
        }

        [Test]
        public void PaRiscDis_ldh_45525f53()
        {
            AssertCode("ldh\t-3057(r10),r18", "45525253");
        }

        [Test]
        public void PaRiscDis_ldb_41524756()
        {
            AssertCode("ldb\t23ab(r10),r18", "41525256");
        }

        [Test]
        public void PaRiscDis_sth_6578745f()
        {
            AssertCode("sth\tr24,-25d1(r11)", "6578785f");
        }

        [Test]
        public void PaRiscDis_sth_6769635f()
        {
            AssertCode("sth\tr9,-2e51(dp)", "6769695f");
        }

        [Test]
        public void PaRiscDis_stw_mb_6f70656e()
        {
            AssertCode("stw_mb\tr16,32b7(dp)", "6f70706e");
        }

        [Test]
        public void PaRiscDis_stw_mb_6f6d7069()
        {
            AssertCode("stw_mb\tr13,-27cc(dp)", "6f6d6d69");
        }

        [Test]
        public void PaRiscDis_stw_ma_6c650067()
        {
            AssertCode("stw_ma\tr5,-1fcd(r3)", "6c656567");
        }

        [Test]
        public void PaRiscDis_sth_65746c69()
        {
            AssertCode("sth\tr20,-29cc(r11)", "65747469");
        }

        [Test]
        public void PaRiscDis_stw_ma_6e65006d()
        {
            AssertCode("stw_ma\tr5,-1fca(r19)", "6e65656d");
        }

        [Test]
        public void PaRiscDis_sth_64006d61()
        {
            AssertCode("sth\tr0,-2950(r0)", "64000061");
        }

        [Test]
        public void PaRiscDis_sth_65727369()
        {
            AssertCode("sth\tr18,-264c(r11)", "65727269");
        }

        [Test]
        public void PaRiscDis_stw_ma_6f6e006d()
        {
            AssertCode("stw_ma\tr14,-1fca(dp)", "6f6e6e6d");
        }

        [Test]
        public void PaRiscDis_sth_65006d61()
        {
            AssertCode("sth\tr0,-2950(r8)", "65000061");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c697374()
        {
            AssertCode("stw_mb\tr9,39ba(r3)", "6c696974");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c6f7365()
        {
            AssertCode("stw_mb\tr15,-264e(r3)", "6c6f6f65");
        }

        [Test]
        public void PaRiscDis_sth_65747061()
        {
            AssertCode("sth\tr20,-27d0(r11)", "65747461");
        }

        [Test]
        public void PaRiscDis_sth_65746f70()
        {
            AssertCode("sth\tr20,37b8(r11)", "65747470");
        }

        [Test]
        public void PaRiscDis_stw_ma_6e67006d()
        {
            AssertCode("stw_ma\tr7,-1fca(r19)", "6e67676d");
        }

        [Test]
        public void PaRiscDis_sth_66676574()
        {
            AssertCode("sth\tr7,32ba(r19)", "66676774");
        }

        [Test]
        public void PaRiscDis_sth_66737461()
        {
            AssertCode("sth\tr19,-25d0(r19)", "66737361");
        }

        [Test]
        public void PaRiscDis_sth_6574706f()
        {
            AssertCode("sth\tr20,-27c9(r11)", "6574746f");
        }

        [Test]
        public void PaRiscDis_stw_ma_6d700073()
        {
            AssertCode("stw_ma\tr16,-1fc7(r11)", "6d707073");
        }

        [Test]
        public void PaRiscDis_stw_mb_6f736500()
        {
            AssertCode("stw_mb\tr19,3280(dp)", "6f737300");
        }

        [Test]
        public void PaRiscDis_sth_66707269()
        {
            AssertCode("sth\tr16,-26cc(r19)", "66707069");
        }

        [Test]
        public void PaRiscDis_stw_mb_6e746600()
        {
            AssertCode("stw_mb\tr20,3300(r19)", "6e747400");
        }

        [Test]
        public void PaRiscDis_sth_65617436()
        {
            AssertCode("sth\tr1,3a1b(r11)", "65616136");
        }

        [Test]
        public void PaRiscDis_stw_mb_6f363400()
        {
            AssertCode("stw_mb\tr22,1a00(r25)", "6f363600");
        }

        [Test]
        public void PaRiscDis_sth_6672656f()
        {
            AssertCode("sth\tr18,-2d49(r19)", "6672726f");
        }

        [Test]
        public void PaRiscDis_stw_mb_6e636d70()
        {
            AssertCode("stw_mb\tr3,36b8(r19)", "6e636370");
        }

        [Test]
        public void PaRiscDis_sth_65616c6c()
        {
            AssertCode("sth\tr1,3636(r11)", "6561616c");
        }

        [Test]
        public void PaRiscDis_stw_mb_6f633634()
        {
            AssertCode("stw_mb\tr3,1b1a(dp)", "6f636334");
        }

        [Test]
        public void PaRiscDis_sth_656e7600()
        {
            AssertCode("sth\tr14,3b00(r11)", "656e6e00");
        }

        [Test]
        public void PaRiscDis_sth_65005f5f()
        {
            AssertCode("sth\tr0,-3051(r8)", "6500005f");
        }

        [Test]
        public void PaRiscDis_stw_mb_6f006666()
        {
            AssertCode("stw_mb\tr0,3333(r24)", "6f000066");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c757368()
        {
            AssertCode("stw_mb\tr21,39b4(r3)", "6c757568");
        }

        [Test]
        public void PaRiscDis_stw_mb_6c656e00()
        {
            AssertCode("stw_mb\tr5,3700(r3)", "6c656500");
        }

        [Test]
        public void PaRiscDis_sth_65616436()
        {
            AssertCode("sth\tr1,321b(r11)", "65616136");
        }

        [Test]
        public void PaRiscDis_sth_666f7065()
        {
            AssertCode("sth\tr15,-27ce(r19)", "666f6f65");
        }

        [Test]
        public void PaRiscDis_stw_mb_6e363400()
        {
            AssertCode("stw_mb\tr22,1a00(r17)", "6e363600");
        }

        [Test]
        public void PaRiscDis_sth_65656b36()
        {
            AssertCode("sth\tr5,359b(r11)", "65656536");
        }

        [Test]
        public void PaRiscDis_sth_66707574()
        {
            AssertCode("sth\tr16,3aba(r19)", "66707074");
        }

        [Test]
        public void PaRiscDis_stw_ma_6f69005f()
        {
            AssertCode("stw_ma\tr9,-1fd1(dp)", "6f69695f");
        }

        [Test]
        public void PaRiscDis_sth_66696c65()
        {
            AssertCode("sth\tr9,-29ce(r19)", "66696965");
        }

        [Test]
        public void PaRiscDis_sth_65007374()
        {
            AssertCode("sth\tr0,39ba(r8)", "65000074");
        }

        [Test]
        public void PaRiscDis_sth_656c6c6f()
        {
            AssertCode("sth\tr12,-29c9(r11)", "656c6c6f");
        }

        [Test]
        public void PaRiscDis_sth_656d7365()
        {
            AssertCode("sth\tr13,-264e(r11)", "656d6d65");
        }

        [Test]
        public void PaRiscDis_stw_ma_6d74005f()
        {
            AssertCode("stw_ma\tr20,-1fd1(r11)", "6d74745f");
        }

        [Test]
        public void PaRiscDis_stw_mb_6f707461()
        {
            AssertCode("stw_mb\tr16,-25d0(dp)", "6f707061");
        }

        [Test]
        public void PaRiscDis_stw_ma_6e6f006f()
        {
            AssertCode("stw_ma\tr15,-1fc9(r19)", "6e6f6f6f");
        }

        [Test]
        public void PaRiscDis_and_eq_08392200()
        {
            AssertCode("and_=\tr25,r1,r0", "08393900");
        }

        [Test]
        public void PaRiscDis_addi_tc_eq_b3202000()
        {
            AssertCode("addi_tc_=\t0,r25,r0", "b3202000");
        }

        [Test]
        public void PaRiscDis_and_083a021d()
        {
            AssertCode("and\tr26,r1,ret1", "083a3a1d");
        }

        [Test]
        public void PaRiscDis_cmpib_uge_n_8f20422a()
        {
            AssertCode("cmpib_uge_n\t0,r25,0x11c", "8f20202a");
        }

        [Test]
        public void PaRiscDis_subi_973d0000()
        {
            AssertCode("subi\t0,r25,ret1", "973d3d00");
        }

        [Test]
        public void PaRiscDis_ds_0ba00440()
        {
            AssertCode("ds\tr0,ret1,r0", "0ba0a040");
        }

        [Test]
        public void PaRiscDis_ds_0b20045d()
        {
            AssertCode("ds\tr0,r25,ret1", "0b20205d");
        }

        [Test]
        public void PaRiscDis_add_c_08210701()
        {
            AssertCode("add_c\tr1,r1,r1", "08212101");
        }

        [Test]
        public void PaRiscDis_ds_0b3d045d()
        {
            AssertCode("ds\tret1,r25,ret1", "0b3d3d5d");
        }

        [Test]
        public void PaRiscDis_cmpiclr__le__93a06000()
        {
            AssertCode("cmpiclr__le_\t0,ret1,r0", "93a0a000");
        }

        [Test]
        public void PaRiscDis_sub_uge_0b3a941d()
        {
            AssertCode("sub_>>=\tr26,r25,ret1", "0b3a3a1d");
        }

        [Test]
        public void PaRiscDis_bb_uge_n_c7d6c012()
        {
            AssertCode("bb_uge_n\tr22,1e,0x10", "c7d6d612");
        }

        [Test]
        public void PaRiscDis_depwi_d6c01c1e()
        {
            AssertCode("depwi\t0,31,2,r22", "d6c0c01e");
        }

        [Test]
        public void PaRiscDis_depwi_d7c01c1d()
        {
            AssertCode("depwi\t0,31,3,sp", "d7c0c01d");
        }

        [Test]
        public void PaRiscDis_addil_2b7fefff()
        {
            AssertCode("addil\tL%-1000,dp,r1", "2b7f7fff");
        }

        [Test]
        public void PaRiscDis_fstw_27791200()
        {
            AssertCode("fstw\tfr0,-4(dp)", "27797900");
        }

        [Test]
        public void PaRiscDis_fldw_27791000()
        {
            AssertCode("fldw\t-4(dp),fr0", "27797900");
        }

        [Test]
        public void PaRiscDis_addi_tr_b40010c2()
        {
            AssertCode("addi_tr\t61,r0,r0", "b40000c2");
        }

        [Test]
        public void PaRiscDis_b_l_n_e80001aa()
        {
            AssertCode("b_l_n\t0xdc,r0", "e80000aa");
        }

        [Test]
        public void PaRiscDis_cmpb_ugt_n_8bd7a06a()
        {
            AssertCode("cmpb,>>,n\tr23,sp,0x3c", "8bd7d76a");
        }

        [Test]
        public void PaRiscDis_cmpb_ult_n_83178062()
        {
            AssertCode("cmpb_<<_n\tr23,r24,0x38", "83171762");
        }

        [Test]
        public void PaRiscDis_extrw_u_d0a619fa()
        {
            AssertCode("extrw_u\tr5,15,6,r6", "d0a6a6fa");
        }

        [Test]
        public void PaRiscDis_sth_0c861240()
        {
            AssertCode("sth\tr6,0(r4)", "0c868640");
        }

        [Test]
        public void PaRiscDis_extrw_u_d0a61a9b()
        {
            AssertCode("extrw_u\tr5,20,5,r6", "d0a6a69b");
        }

        [Test]
        public void PaRiscDis_sth_0c861244()
        {
            AssertCode("sth\tr6,2(r4)", "0c868644");
        }

        [Test]
        public void PaRiscDis_addil_2803f0b0()
        {
            AssertCode("addil\tL%58c7800,r0,r1", "280303b0");
        }

        [Test]
        public void PaRiscDis_addil_2817e0a8()
        {
            AssertCode("addil\tL%54ef000,r0,r1", "281717a8");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_n_83f3200a()
        {
            AssertCode("cmpb_=_n\tr19,r31,0xc", "83f3f30a");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_n_8bf3208a()
        {
            AssertCode("cmpb_ne_n\tr19,r31,0x4c", "8bf3f38a");
        }

        [Test]
        public void PaRiscDis_addil_2bc10000()
        {
            AssertCode("addil\tL%2000,sp,r1", "2bc1c100");
        }

        [Test]
        public void PaRiscDis_b_l_e8400068()
        {
            AssertCode("b_l\t0x3c,rp", "e8404068");
        }

        [Test]
        public void PaRiscDis_addil_2b600000()
        {
            AssertCode("addil\tL%0,dp,r1", "2b606000");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_n_86c02032()
        {
            AssertCode("cmpib_=_n\t0,r22,0x20", "86c0c032");
        }

        [Test]
        public void PaRiscDis_b_l_ebff1cf5()
        {
            AssertCode("b_l\t0xfffffe80,r31", "ebfffff5");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_82c02010()
        {
            AssertCode("cmpb_=\tr0,r22,0x10", "82c0c010");
        }

        [Test]
        public void PaRiscDis_b_l_ebff1c5d()
        {
            AssertCode("b_l\t0xfffffe34,r31", "ebffff5d");
        }

        [Test]
        public void PaRiscDis_stw_ma_6fc30100()
        {
            AssertCode("stw_ma\tr3,80(sp)", "6fc3c300");
        }

        [Test]
        public void PaRiscDis_addil_28022000()
        {
            AssertCode("addil\tL%5000,r0,r1", "28020200");
        }

        [Test]
        public void PaRiscDis_b_l_e84018d0()
        {
            AssertCode("b_l\t0xc70,rp", "e84040d0");
        }

        [Test]
        public void PaRiscDis_b_l_e8400148()
        {
            AssertCode("b_l\t0xac,rp", "e8404048");
        }

        [Test]
        public void PaRiscDis_b_l_e84018a0()
        {
            AssertCode("b_l\t0xc58,rp", "e84040a0");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_80a02030()
        {
            AssertCode("cmpb_=\tr0,r5,0x20", "80a0a030");
        }

        [Test]
        public void PaRiscDis_b_l_e8400108()
        {
            AssertCode("b_l\t0x8c,rp", "e8404008");
        }

        [Test]
        public void PaRiscDis_b_l_e8401860()
        {
            AssertCode("b_l\t0xc38,rp", "e8404060");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_n_80802072()
        {
            AssertCode("cmpb_=_n\tr0,r4,0x40", "80808072");
        }

        [Test]
        public void PaRiscDis_addil_2b7fffff()
        {
            AssertCode("addil\tL%-800,dp,r1", "2b7f7fff");
        }

        [Test]
        public void PaRiscDis_b_l_e8400118()
        {
            AssertCode("b_l\t0x94,rp", "e8404018");
        }

        [Test]
        public void PaRiscDis_b_l_e8401810()
        {
            AssertCode("b_l\t0xc10,rp", "e8404010");
        }

        [Test]
        public void PaRiscDis_b_l_e8400088()
        {
            AssertCode("b_l\t0x4c,rp", "e8404088");
        }

        [Test]
        public void PaRiscDis_b_l_e84017e0()
        {
            AssertCode("b_l\t0xbf8,rp", "e84040e0");
        }

        [Test]
        public void PaRiscDis_b_l_e84017c0()
        {
            AssertCode("b_l\t0xbe8,rp", "e84040c0");
        }

        [Test]
        public void PaRiscDis_b_l_e84016a0()
        {
            AssertCode("b_l\t0xb58,rp", "e84040a0");
        }

        [Test]
        public void PaRiscDis_ldw_mb_4fc33f01()
        {
            AssertCode("ldw_mb\t-80(sp),r3", "4fc3c301");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_881a2010()
        {
            AssertCode("cmpb_<>\tr26,r0,0x10", "881a1a10");
        }

        [Test]
        public void PaRiscDis_ldb_43570000()
        {
            AssertCode("ldb\t0(r26),r23", "43575700");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_82e03fe5()
        {
            AssertCode("cmpb_=\tr0,r23,0xfffffff8", "82e0e0e5");
        }

        [Test]
        public void PaRiscDis_ldb_0fe01018()
        {
            AssertCode("ldb\t0(r31),r24", "0fe0e018");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_8b003fe5()
        {
            AssertCode("cmpb_<>\tr0,r24,0xfffffff8", "8b0000e5");
        }

        [Test]
        public void PaRiscDis_stb_0f201212()
        {
            AssertCode("stb\tr0,9(r25)", "0f202012");
        }

        [Test]
        public void PaRiscDis_fldd_2f001004()
        {
            AssertCode("fldd\t0(r24),fr4", "2f000004");
        }

        [Test]
        public void PaRiscDis_fldw_27101005()
        {
            AssertCode("fldw\t8(r24),fr5", "27101005");
        }

        [Test]
        public void PaRiscDis_fldw_27011049()
        {
            AssertCode("fldw\t-10(r24),fr9R", "27010149");
        }

        [Test]
        public void PaRiscDis_fldw_27011047()
        {
            AssertCode("fldw\t-10(r24),fr7R", "27010147");
        }

        [Test]
        public void PaRiscDis_fcnvxf_sgl_dbl_3920a28a()
        {
            AssertCode("fcnvxf_sgl_dbl\tfr9R,fr10", "3920208a");
        }

        [Test]
        public void PaRiscDis_fcnvxf_sgl_dbl_38e0a28b()
        {
            AssertCode("fcnvxf_sgl_dbl\tfr7R,fr11", "38e0e08b");
        }

        [Test]
        public void PaRiscDis_fmpy_dbl_31444e18()
        {
            AssertCode("fmpy_dbl\tfr10,fr4,fr24", "31444418");
        }

        [Test]
        public void PaRiscDis_fmpy_dbl_31644e16()
        {
            AssertCode("fmpy_dbl\tfr11,fr4,fr22", "31646416");
        }

        [Test]
        public void PaRiscDis_fcnvfxt_dbl_sgl_33018a07()
        {
            AssertCode("fcnvfxt_dbl_sgl\tfr24,fr7", "33010107");
        }

        [Test]
        public void PaRiscDis_fstw_27111207()
        {
            AssertCode("fstw\tfr7,-8(r24)", "27111107");
        }

        [Test]
        public void PaRiscDis_fcnvfxt_dbl_sgl_32c18a09()
        {
            AssertCode("fcnvfxt_dbl_sgl\tfr22,fr9", "32c1c109");
        }

        [Test]
        public void PaRiscDis_fcnvxf_sgl_dbl_30e0a206()
        {
            AssertCode("fcnvxf_sgl_dbl\tfr7,fr6", "30e0e006");
        }

        [Test]
        public void PaRiscDis_fcnvxf_sgl_dbl_30e0a208()
        {
            AssertCode("fcnvxf_sgl_dbl\tfr7,fr8", "30e0e008");
        }

        [Test]
        public void PaRiscDis_xmpyu_3925471a()
        {
            AssertCode("xmpyu\tfr9,fr5,fr26", "3925251a");
        }

        [Test]
        public void PaRiscDis_fstw_2709125a()
        {
            AssertCode("fstw\tfr26R,-c(r24)", "2709095a");
        }

        [Test]
        public void PaRiscDis_fmpy_dbl_30c44e19()
        {
            AssertCode("fmpy_dbl\tfr6,fr4,fr25", "30c4c419");
        }

        [Test]
        public void PaRiscDis_fmpy_dbl_31044e17()
        {
            AssertCode("fmpy_dbl\tfr8,fr4,fr23", "31040417");
        }

        [Test]
        public void PaRiscDis_fcnvfxt_dbl_sgl_33218a0a()
        {
            AssertCode("fcnvfxt_dbl_sgl\tfr25,fr10", "3321210a");
        }

        [Test]
        public void PaRiscDis_fstw_2700120a()
        {
            AssertCode("fstw\tfr10,0(r24)", "2700000a");
        }

        [Test]
        public void PaRiscDis_fcnvfxt_dbl_sgl_32e18a0b()
        {
            AssertCode("fcnvfxt_dbl_sgl\tfr23,fr11", "32e1e10b");
        }

        [Test]
        public void PaRiscDis_sub_083a041c()
        {
            AssertCode("sub\tr26,r1,ret0", "083a3a1c");
        }

        [Test]
        public void PaRiscDis_stb_0efc1208()
        {
            AssertCode("stb\tret0,4(r23)", "0efcfc08");
        }

        [Test]
        public void PaRiscDis_fcnvxf_sgl_dbl_3140a207()
        {
            AssertCode("fcnvxf_sgl_dbl\tfr10,fr7", "31404007");
        }

        [Test]
        public void PaRiscDis_fcnvxf_sgl_dbl_3140a209()
        {
            AssertCode("fcnvxf_sgl_dbl\tfr10,fr9", "31404009");
        }

        [Test]
        public void PaRiscDis_xmpyu_3965471a()
        {
            AssertCode("xmpyu\tfr11,fr5,fr26", "3965651a");
        }

        [Test]
        public void PaRiscDis_fstw_2719125a()
        {
            AssertCode("fstw\tfr26R,-4(r24)", "2719195a");
        }

        [Test]
        public void PaRiscDis_fmpy_dbl_30e44e18()
        {
            AssertCode("fmpy_dbl\tfr7,fr4,fr24", "30e4e418");
        }

        [Test]
        public void PaRiscDis_fmpy_dbl_31244e16()
        {
            AssertCode("fmpy_dbl\tfr9,fr4,fr22", "31242416");
        }

        [Test]
        public void PaRiscDis_fcnvfxt_dbl_sgl_3b018a4a()
        {
            AssertCode("fcnvfxt_dbl_sgl\tfr24,fr10R", "3b01014a");
        }

        [Test]
        public void PaRiscDis_fstw_2710124a()
        {
            AssertCode("fstw\tfr10R,8(r24)", "2710104a");
        }

        [Test]
        public void PaRiscDis_fcnvfxt_dbl_sgl_3ac18a4b()
        {
            AssertCode("fcnvfxt_dbl_sgl\tfr22,fr11R", "3ac1c14b");
        }

        [Test]
        public void PaRiscDis_sub_0b360416()
        {
            AssertCode("sub\tr22,r25,r22", "0b363616");
        }

        [Test]
        public void PaRiscDis_stb_0ef61206()
        {
            AssertCode("stb\tr22,3(r23)", "0ef6f606");
        }

        [Test]
        public void PaRiscDis_fcnvxf_sgl_dbl_3940a286()
        {
            AssertCode("fcnvxf_sgl_dbl\tfr10R,fr6", "39404086");
        }

        [Test]
        public void PaRiscDis_fcnvxf_sgl_dbl_3940a288()
        {
            AssertCode("fcnvxf_sgl_dbl\tfr10R,fr8", "39404088");
        }

        [Test]
        public void PaRiscDis_xmpyu_3965479a()
        {
            AssertCode("xmpyu\tfr11R,fr5,fr26", "3965659a");
        }

        [Test]
        public void PaRiscDis_fstw_2708125a()
        {
            AssertCode("fstw\tfr26R,4(r24)", "2708085a");
        }

        [Test]
        public void PaRiscDis_fstw_2701120a()
        {
            AssertCode("fstw\tfr10,-10(r24)", "2701010a");
        }

        [Test]
        public void PaRiscDis_sub_0b550415()
        {
            AssertCode("sub\tr21,r26,r21", "0b555515");
        }

        [Test]
        public void PaRiscDis_stb_0ef51204()
        {
            AssertCode("stb\tr21,2(r23)", "0ef5f504");
        }

        [Test]
        public void PaRiscDis_fstw_2718125a()
        {
            AssertCode("fstw\tfr26R,c(r24)", "2718185a");
        }

        [Test]
        public void PaRiscDis_fstw_2711124a()
        {
            AssertCode("fstw\tfr10R,-8(r24)", "2711114a");
        }

        [Test]
        public void PaRiscDis_sub_0bb40414()
        {
            AssertCode("sub\tr20,ret1,r20", "0bb4b414");
        }

        [Test]
        public void PaRiscDis_stb_0ef41202()
        {
            AssertCode("stb\tr20,1(r23)", "0ef4f402");
        }

        [Test]
        public void PaRiscDis_sub_0a930413()
        {
            AssertCode("sub\tr19,r20,r19", "0a939313");
        }

        [Test]
        public void PaRiscDis_stb_0ef31200()
        {
            AssertCode("stb\tr19,0(r23)", "0ef3f300");
        }

        [Test]
        public void PaRiscDis_sub_0abd041d()
        {
            AssertCode("sub\tret1,r21,ret1", "0abdbd1d");
        }

        [Test]
        public void PaRiscDis_stb_0efd121f()
        {
            AssertCode("stb\tret1,-1(r23)", "0efdfd1f");
        }

        [Test]
        public void PaRiscDis_sub_0ada041a()
        {
            AssertCode("sub\tr26,r22,r26", "0adada1a");
        }

        [Test]
        public void PaRiscDis_stb_0efa121d()
        {
            AssertCode("stb\tr26,-2(r23)", "0efafa1d");
        }

        [Test]
        public void PaRiscDis_xmpyu_39654718()
        {
            AssertCode("xmpyu\tfr11,fr5,fr24", "39656518");
        }

        [Test]
        public void PaRiscDis_fstw_27181258()
        {
            AssertCode("fstw\tfr24R,c(r24)", "27181858");
        }

        [Test]
        public void PaRiscDis_fmpy_dbl_30e44e08()
        {
            AssertCode("fmpy_dbl\tfr7,fr4,fr8", "30e4e408");
        }

        [Test]
        public void PaRiscDis_fcnvfxt_dbl_sgl_31018a06()
        {
            AssertCode("fcnvfxt_dbl_sgl\tfr8,fr6", "31010106");
        }

        [Test]
        public void PaRiscDis_fcnvfxt_dbl_sgl_3ac18a4a()
        {
            AssertCode("fcnvfxt_dbl_sgl\tfr22,fr10R", "3ac1c14a");
        }

        [Test]
        public void PaRiscDis_sub_08390419()
        {
            AssertCode("sub\tr25,r1,r25", "08393919");
        }

        [Test]
        public void PaRiscDis_stb_0ee1121b()
        {
            AssertCode("stb\tr1,-3(r23)", "0ee1e11b");
        }

        [Test]
        public void PaRiscDis_xmpyu_38c5470a()
        {
            AssertCode("xmpyu\tfr6,fr5,fr10", "38c5c50a");
        }

        [Test]
        public void PaRiscDis_fstw_2709124a()
        {
            AssertCode("fstw\tfr10R,-c(r24)", "2709094a");
        }

        [Test]
        public void PaRiscDis_sub_083c0419()
        {
            AssertCode("sub\tret0,r1,r25", "083c3c19");
        }

        [Test]
        public void PaRiscDis_stb_0efd1219()
        {
            AssertCode("stb\tret1,-4(r23)", "0efdfd19");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_830020f0()
        {
            AssertCode("cmpb_=\tr0,r24,0x80", "830000f0");
        }

        [Test]
        public void PaRiscDis_subi_975d07ff()
        {
            AssertCode("subi\t-1,r26,ret1", "975d5dff");
        }

        [Test]
        public void PaRiscDis_subi_97b50000()
        {
            AssertCode("subi\t0,ret1,r21", "97b5b500");
        }

        [Test]
        public void PaRiscDis_cmpiclr_uge_92bf5000()
        {
            AssertCode("cmpiclr_>=\t0,r21,r31", "92bfbf00");
        }

        [Test]
        public void PaRiscDis_extrw_u_d3f81bfe()
        {
            AssertCode("extrw_u\tr31,31,2,r24", "d3f8f8fe");
        }

        [Test]
        public void PaRiscDis_ldb_0fa01014()
        {
            AssertCode("ldb\t0(ret1),r20", "0fa0a014");
        }

        [Test]
        public void PaRiscDis_stb_0e741200()
        {
            AssertCode("stb\tr20,0(r19)", "0e747400");
        }

        [Test]
        public void PaRiscDis_cmpib_gt_n_8fe8606a()
        {
            AssertCode("cmpib_>_n\t4,r31,0x3c", "8fe8e86a");
        }

        [Test]
        public void PaRiscDis_depw_d7e00c1e()
        {
            AssertCode("depw\tr0,31,2,r31", "d7e0e01e");
        }

        [Test]
        public void PaRiscDis_ldb_0f1f101a()
        {
            AssertCode("ldb\t-1(r24),r26", "0f1f1f1a");
        }

        [Test]
        public void PaRiscDis_stb_0efa121f()
        {
            AssertCode("stb\tr26,-1(r23)", "0efafa1f");
        }

        [Test]
        public void PaRiscDis_ldb_0f001019()
        {
            AssertCode("ldb\t0(r24),r25", "0f000019");
        }

        [Test]
        public void PaRiscDis_stb_0ef91200()
        {
            AssertCode("stb\tr25,0(r23)", "0ef9f900");
        }

        [Test]
        public void PaRiscDis_ldb_0f021001()
        {
            AssertCode("ldb\t1(r24),r1", "0f020201");
        }

        [Test]
        public void PaRiscDis_stb_0ee11202()
        {
            AssertCode("stb\tr1,1(r23)", "0ee1e102");
        }

        [Test]
        public void PaRiscDis_ldb_0f041016()
        {
            AssertCode("ldb\t2(r24),r22", "0f040416");
        }

        [Test]
        public void PaRiscDis_stb_0ef61204()
        {
            AssertCode("stb\tr22,2(r23)", "0ef6f604");
        }

        [Test]
        public void PaRiscDis_stw_ma_6fc30480()
        {
            AssertCode("stw_ma\tr3,240(sp)", "6fc3c380");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_80052010()
        {
            AssertCode("cmpb_=\tr5,r0,0x10", "80050510");
        }

        [Test]
        public void PaRiscDis_b_l_e8400e00()
        {
            AssertCode("b_l\t0x708,rp", "e8404000");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_879f24f0()
        {
            AssertCode("cmpib_=\t-1,ret0,0x280", "879f9ff0");
        }

        [Test]
        public void PaRiscDis_b_l_e8400e10()
        {
            AssertCode("b_l\t0x710,rp", "e8404010");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_8b982500()
        {
            AssertCode("cmpb_<>\tr24,ret0,0x288", "8b989800");
        }

        [Test]
        public void PaRiscDis_ldh_47dc3ccd()
        {
            AssertCode("ldh\t-19a(sp),ret0", "47dcdccd");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_n_8b3c203a()
        {
            AssertCode("cmpb_ne_n\tret0,r25,0x24", "8b3c3c3a");
        }

        [Test]
        public void PaRiscDis_ldh_47d73cc9()
        {
            AssertCode("ldh\t-19c(sp),r23", "47d7d7c9");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_82b72050()
        {
            AssertCode("cmpb_=\tr23,r21,0x30", "82b7b750");
        }

        [Test]
        public void PaRiscDis_extrw_s_d2ff1ff0()
        {
            AssertCode("extrw_s\tr23,31,16,r31", "d2fffff0");
        }

        [Test]
        public void PaRiscDis_cmpb_lt_n_835f400a()
        {
            AssertCode("cmpb_lt_n\tr31,r26,0xc", "835f5f0a");
        }

        [Test]
        public void PaRiscDis_cmpb__le__83bf6030()
        {
            AssertCode("cmpb__le_\tr31,ret1,0x20", "83bfbf30");
        }

        [Test]
        public void PaRiscDis_b_l_e85f15fd()
        {
            AssertCode("b_l\t0xfffffb04,rp", "e85f5ffd");
        }

        [Test]
        public void PaRiscDis_b_l_e8400f50()
        {
            AssertCode("b_l\t0x7b0,rp", "e8404050");
        }

        [Test]
        public void PaRiscDis_b_l_e8400d30()
        {
            AssertCode("b_l\t0x6a0,rp", "e8404030");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_n_8a7c200a()
        {
            AssertCode("cmpb_ne_n\tret0,r19,0xc", "8a7c7c0a");
        }

        [Test]
        public void PaRiscDis_ldh_47d43dcd()
        {
            AssertCode("ldh\t-11a(sp),r20", "47d4d4cd");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_n_8688202a()
        {
            AssertCode("cmpib_=_n\t4,r20,0x1c", "8688882a");
        }

        [Test]
        public void PaRiscDis_b_l_e85f156d()
        {
            AssertCode("b_l\t0xfffffabc,rp", "e85f5f6d");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_8b8023f8()
        {
            AssertCode("cmpb_<>\tr0,ret0,0x204", "8b8080f8");
        }

        [Test]
        public void PaRiscDis_b_l_e8400ca8()
        {
            AssertCode("b_l\t0x65c,rp", "e84040a8");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_879f23e0()
        {
            AssertCode("cmpib_=\t-1,ret0,0x1f8", "879f9fe0");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_n_88a0241a()
        {
            AssertCode("cmpb_ne_n\tr0,r5,0x214", "88a0a01a");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_n_89c02882()
        {
            AssertCode("cmpb_ne_n\tr0,r14,0x448", "89c0c082");
        }

        [Test]
        public void PaRiscDis_b_l_e8400cb0()
        {
            AssertCode("b_l\t0x660,rp", "e84040b0");
        }

        [Test]
        public void PaRiscDis_cmpb_lt_n_83ef4962()
        {
            AssertCode("cmpb_lt_n\tr15,r31,0x4b8", "83efef62");
        }

        [Test]
        public void PaRiscDis_ldb_43ce3ea1()
        {
            AssertCode("ldb\t-b0(sp),r14", "43cecea1");
        }

        [Test]
        public void PaRiscDis_cmpiclr_eq_93ad2000()
        {
            AssertCode("cmpiclr_=\t0,ret1,r13", "93adad00");
        }

        [Test]
        public void PaRiscDis_depw_d5cd0cff()
        {
            AssertCode("depw\tr13,24,1,r14", "d5cdcdff");
        }

        [Test]
        public void PaRiscDis_add_l_086c0a0e()
        {
            AssertCode("add_l\tr12,r3,r14", "086c6c0e");
        }

        [Test]
        public void PaRiscDis_depw_d6600c01()
        {
            AssertCode("depw\tr0,31,31,r19", "d6606001");
        }

        [Test]
        public void PaRiscDis_addil_2b7fc7ff()
        {
            AssertCode("addil\tL%-40002000,dp,r1", "2b7f7fff");
        }

        [Test]
        public void PaRiscDis_cmpiclr_eq_92e02000()
        {
            AssertCode("cmpiclr_=\t0,r23,r0", "92e0e000");
        }

        [Test]
        public void PaRiscDis_cmpiclr_ne_92e03000()
        {
            AssertCode("cmpiclr_<>\t0,r23,r0", "92e0e000");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_n_82f92752()
        {
            AssertCode("cmpb_=_n\tr25,r23,0x3b0", "82f9f952");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_n_82fa2722()
        {
            AssertCode("cmpb_=_n\tr26,r23,0x398", "82fafa22");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_n_8aff28f2()
        {
            AssertCode("cmpb_ne_n\tr31,r23,0x480", "8afffff2");
        }

        [Test]
        public void PaRiscDis_depwi_d55f1c3f()
        {
            AssertCode("depwi\t-1,30,1,r10", "d55f5f3f");
        }

        [Test]
        public void PaRiscDis_b_l_e8000738()
        {
            AssertCode("b_l\t0x3a4,r0", "e8000038");
        }

        [Test]
        public void PaRiscDis_b_l_e84009a0()
        {
            AssertCode("b_l\t0x4d8,rp", "e84040a0");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_879f2660()
        {
            AssertCode("cmpib_=\t-1,ret0,0x338", "879f9f60");
        }

        [Test]
        public void PaRiscDis_b_l_e81f1cfd()
        {
            AssertCode("b_l\t0xfffffe84,r0", "e81f1ffd");
        }

        [Test]
        public void PaRiscDis_add_l_08ee0a0b()
        {
            AssertCode("add_l\tr14,r7,r11", "08eeee0b");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1195()
        {
            AssertCode("b_l\t0xfffff8d0,rp", "e85f5f95");
        }

        [Test]
        public void PaRiscDis_b_l_e81f1ad5()
        {
            AssertCode("b_l\t0xfffffd70,r0", "e81f1fd5");
        }

        [Test]
        public void PaRiscDis_b_l_e85f114d()
        {
            AssertCode("b_l\t0xfffff8ac,rp", "e85f5f4d");
        }

        [Test]
        public void PaRiscDis_b_l_e81f1ac5()
        {
            AssertCode("b_l\t0xfffffd68,r0", "e81f1fc5");
        }

        [Test]
        public void PaRiscDis_depwi_d47f1cdf()
        {
            AssertCode("depwi\t-1,25,1,r3", "d47f7fdf");
        }

        [Test]
        public void PaRiscDis_depwi_d4ff1cdf()
        {
            AssertCode("depwi\t-1,25,1,r7", "d4ffffdf");
        }

        [Test]
        public void PaRiscDis_extrw_s_uge_d2c0df9f()
        {
            AssertCode("extrw_s_>=\tr22,28,1,r0", "d2c0c09f");
        }

        [Test]
        public void PaRiscDis_depwi_d47f1cff()
        {
            AssertCode("depwi\t-1,24,1,r3", "d47f7fff");
        }

        [Test]
        public void PaRiscDis_b_l_e81f1bd5()
        {
            AssertCode("b_l\t0xfffffdf0,r0", "e81f1fd5");
        }

        [Test]
        public void PaRiscDis_depwi_d57f1cdf()
        {
            AssertCode("depwi\t-1,25,1,r11", "d57f7fdf");
        }

        [Test]
        public void PaRiscDis_b_l_e85f10cd()
        {
            AssertCode("b_l\t0xfffff86c,rp", "e85f5fcd");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_80a03be5()
        {
            AssertCode("cmpb_=\tr0,r5,0xfffffdf8", "80a0a0e5");
        }

        [Test]
        public void PaRiscDis_b_l_e8400a78()
        {
            AssertCode("b_l\t0x544,rp", "e8404078");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_n_879f22d2()
        {
            AssertCode("cmpib_=_n\t-1,ret0,0x170", "879f9fd2");
        }

        [Test]
        public void PaRiscDis_ldb_42010428()
        {
            AssertCode("ldb\t214(r16),r1", "42010128");
        }

        [Test]
        public void PaRiscDis_extrw_s_d0331ff8()
        {
            AssertCode("extrw_s\tr1,31,8,r19", "d03333f8");
        }

        [Test]
        public void PaRiscDis_ldb_4211042a()
        {
            AssertCode("ldb\t215(r16),r17", "4211112a");
        }

        [Test]
        public void PaRiscDis_shladd_l_0ab50a5f()
        {
            AssertCode("shladd_l\tr21,1,r21,r31", "0ab5b55f");
        }

        [Test]
        public void PaRiscDis_shladd_l_0abf0ad3()
        {
            AssertCode("shladd_l\tr31,3,r21,r19", "0abfbfd3");
        }

        [Test]
        public void PaRiscDis_ldb_4215042e()
        {
            AssertCode("ldb\t217(r16),r21", "4215152e");
        }

        [Test]
        public void PaRiscDis_extrw_s_d2221ff8()
        {
            AssertCode("extrw_s\tr17,31,8,rp", "d22222f8");
        }

        [Test]
        public void PaRiscDis_ldb_42180448()
        {
            AssertCode("ldb\t224(r16),r24", "42181848");
        }

        [Test]
        public void PaRiscDis_shladd_l_0b5a0a41()
        {
            AssertCode("shladd_l\tr26,1,r26,r1", "0b5a5a41");
        }

        [Test]
        public void PaRiscDis_ldb_421f0430()
        {
            AssertCode("ldb\t218(r16),r31", "421f1f30");
        }

        [Test]
        public void PaRiscDis_extrw_s_d2af1ff8()
        {
            AssertCode("extrw_s\tr21,31,8,r15", "d2afaff8");
        }

        [Test]
        public void PaRiscDis_depw_z_d72108a5()
        {
            AssertCode("depw_z\tr1,26,27,r25", "d72121a5");
        }

        [Test]
        public void PaRiscDis_shladd_l_0b3a0a9d()
        {
            AssertCode("shladd_l\tr26,2,r25,ret1", "0b3a3a9d");
        }

        [Test]
        public void PaRiscDis_shladd_l_0a730a97()
        {
            AssertCode("shladd_l\tr19,2,r19,r23", "0a737397");
        }

        [Test]
        public void PaRiscDis_shladd_l_08140a54()
        {
            AssertCode("shladd_l\tr20,1,r0,r20", "08141454");
        }

        [Test]
        public void PaRiscDis_shladd_l_0bb70ac2()
        {
            AssertCode("shladd_l\tr23,3,ret1,rp", "0bb7b7c2");
        }

        [Test]
        public void PaRiscDis_cmpiclr_ne_931130d2()
        {
            AssertCode("cmpiclr_<>\t69,r24,r17", "931111d2");
        }

        [Test]
        public void PaRiscDis_shladd_l_0a940a97()
        {
            AssertCode("shladd_l\tr20,2,r20,r23", "0a949497");
        }

        [Test]
        public void PaRiscDis_extrw_s_d3f91ff8()
        {
            AssertCode("extrw_s\tr31,31,8,r25", "d3f9f9f8");
        }

        [Test]
        public void PaRiscDis_add_l_0ae20a12()
        {
            AssertCode("add_l\trp,r23,r18", "0ae2e212");
        }

        [Test]
        public void PaRiscDis_cmpb_ev_8811e018()
        {
            AssertCode("cmpb_ev\tr17,r0,0x14", "88111118");
        }

        [Test]
        public void PaRiscDis_add_l_09f20a0f()
        {
            AssertCode("add_l\tr18,r15,r15", "09f2f20f");
        }

        [Test]
        public void PaRiscDis_ldb_4210044a()
        {
            AssertCode("ldb\t225(r16),r16", "4210104a");
        }

        [Test]
        public void PaRiscDis_cmpiclr_ne_921130c2()
        {
            AssertCode("cmpiclr_<>\t61,r16,r17", "921111c2");
        }

        [Test]
        public void PaRiscDis_extrw_s_d1f81ff0()
        {
            AssertCode("extrw_s\tr15,31,16,r24", "d1f8f8f0");
        }

        [Test]
        public void PaRiscDis_cmpclr_lt_0a984880()
        {
            AssertCode("cmpclr_<\tr24,r20,r0", "0a989880");
        }

        [Test]
        public void PaRiscDis_cmpiclr_ne_92203000()
        {
            AssertCode("cmpiclr_<>\t0,r17,r0", "92202000");
        }

        [Test]
        public void PaRiscDis_b_l_n_e80000e2()
        {
            AssertCode("b_l_n\t0x78,r0", "e80000e2");
        }

        [Test]
        public void PaRiscDis_b_l_e84008f0()
        {
            AssertCode("b_l\t0x480,rp", "e84040f0");
        }

        [Test]
        public void PaRiscDis_cmpib_uge_8f9f40c0()
        {
            AssertCode("cmpib_>=\t-1,ret0,0x68", "8f9f9fc0");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_n_89572022()
        {
            AssertCode("cmpb_ne_n\tr23,r10,0x18", "89575722");
        }

        [Test]
        public void PaRiscDis_andcm_0bb80001()
        {
            AssertCode("andcm\tr24,ret1,r1", "0bb8b801");
        }

        [Test]
        public void PaRiscDis_depwi_d6e01c14()
        {
            AssertCode("depwi\t0,31,12,r23", "d6e0e014");
        }

        [Test]
        public void PaRiscDis_depwi_d6ff1d9f()
        {
            AssertCode("depwi\t-1,19,1,r23", "d6ffff9f");
        }

        [Test]
        public void PaRiscDis_andcm_0bee001c()
        {
            AssertCode("andcm\tr14,r31,ret0", "0beeee1c");
        }

        [Test]
        public void PaRiscDis_sub_0ad70402()
        {
            AssertCode("sub\tr23,r22,rp", "0ad7d702");
        }

        [Test]
        public void PaRiscDis_add_l_08410a17()
        {
            AssertCode("add_l\tr1,rp,r23", "08414117");
        }

        [Test]
        public void PaRiscDis_sub_0ace0418()
        {
            AssertCode("sub\tr14,r22,r24", "0acece18");
        }

        [Test]
        public void PaRiscDis_cmpclr_ule_0af8a880()
        {
            AssertCode("cmpclr_<_le_\tr24,r23,r0", "0af8f880");
        }

        [Test]
        public void PaRiscDis_movb_tr_n_c8a0801a()
        {
            AssertCode("movb_tr_n\tr0,r5,0x14", "c8a0a01a");
        }

        [Test]
        public void PaRiscDis_sub_0b170413()
        {
            AssertCode("sub\tr23,r24,r19", "0b171713");
        }

        [Test]
        public void PaRiscDis_andcm_0bf30002()
        {
            AssertCode("andcm\tr19,r31,rp", "0bf3f302");
        }

        [Test]
        public void PaRiscDis_extrw_s_d1f91ff0()
        {
            AssertCode("extrw_s\tr15,31,16,r25", "d1f9f9f0");
        }

        [Test]
        public void PaRiscDis_cmpclr_lt_0ad94880()
        {
            AssertCode("cmpclr_<\tr25,r22,r0", "0ad9d980");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_8005395d()
        {
            AssertCode("cmpb_=\tr5,r0,0xfffffcb4", "8005055d");
        }

        [Test]
        public void PaRiscDis_b_l_e84005a8()
        {
            AssertCode("b_l\t0x2dc,rp", "e84040a8");
        }

        [Test]
        public void PaRiscDis_cmpib_ne_8f9f390d()
        {
            AssertCode("cmpib_<>\t-1,ret0,0xfffffc8c", "8f9f9f0d");
        }

        [Test]
        public void PaRiscDis_b_l_e85f0dad()
        {
            AssertCode("b_l\t0xfffff6dc,rp", "e85f5fad");
        }

        [Test]
        public void PaRiscDis_b_l_n_e81f18c7()
        {
            AssertCode("b_l_n\t0xfffffc68,r0", "e81f1fc7");
        }

        [Test]
        public void PaRiscDis_b_l_e8400778()
        {
            AssertCode("b_l\t0x3c4,rp", "e8404078");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_n_879f3f1f()
        {
            AssertCode("cmpib_=_n\t-1,ret0,0xffffff94", "879f9f1f");
        }

        [Test]
        public void PaRiscDis_ldb_42020808()
        {
            AssertCode("ldb\t404(r16),rp", "42020208");
        }

        [Test]
        public void PaRiscDis_extrw_s_d0541ff8()
        {
            AssertCode("extrw_s\trp,31,8,r20", "d05454f8");
        }

        [Test]
        public void PaRiscDis_ldb_4212080a()
        {
            AssertCode("ldb\t405(r16),r18", "4212120a");
        }

        [Test]
        public void PaRiscDis_shladd_l_0ad60a57()
        {
            AssertCode("shladd_l\tr22,1,r22,r23", "0ad6d657");
        }

        [Test]
        public void PaRiscDis_shladd_l_0ad70ad4()
        {
            AssertCode("shladd_l\tr23,3,r22,r20", "0ad7d7d4");
        }

        [Test]
        public void PaRiscDis_ldb_4216080e()
        {
            AssertCode("ldb\t407(r16),r22", "4216160e");
        }

        [Test]
        public void PaRiscDis_extrw_s_d24f1ff8()
        {
            AssertCode("extrw_s\tr18,31,8,r15", "d24f4ff8");
        }

        [Test]
        public void PaRiscDis_ldb_42190c08()
        {
            AssertCode("ldb\t604(r16),r25", "42191908");
        }

        [Test]
        public void PaRiscDis_shladd_l_0bbd0a42()
        {
            AssertCode("shladd_l\tret1,1,ret1,rp", "0bbdbd42");
        }

        [Test]
        public void PaRiscDis_ldb_42170810()
        {
            AssertCode("ldb\t408(r16),r23", "42171710");
        }

        [Test]
        public void PaRiscDis_extrw_s_d2d11ff8()
        {
            AssertCode("extrw_s\tr22,31,8,r17", "d2d1d1f8");
        }

        [Test]
        public void PaRiscDis_depw_z_d66208a5()
        {
            AssertCode("depw_z\trp,26,27,r19", "d66262a5");
        }

        [Test]
        public void PaRiscDis_shladd_l_0a940a9a()
        {
            AssertCode("shladd_l\tr20,2,r20,r26", "0a94949a");
        }

        [Test]
        public void PaRiscDis_shladd_l_0a7d0a93()
        {
            AssertCode("shladd_l\tret1,2,r19,r19", "0a7d7d93");
        }

        [Test]
        public void PaRiscDis_shladd_l_0a7a0acf()
        {
            AssertCode("shladd_l\tr26,3,r19,r15", "0a7a7acf");
        }

        [Test]
        public void PaRiscDis_shladd_l_08150a55()
        {
            AssertCode("shladd_l\tr21,1,r0,r21", "08151555");
        }

        [Test]
        public void PaRiscDis_cmpiclr_ne_933130d2()
        {
            AssertCode("cmpiclr_<>\t69,r25,r17", "933131d2");
        }

        [Test]
        public void PaRiscDis_shladd_l_0ab50a98()
        {
            AssertCode("shladd_l\tr21,2,r21,r24", "0ab5b598");
        }

        [Test]
        public void PaRiscDis_extrw_s_d2fa1ff8()
        {
            AssertCode("extrw_s\tr23,31,8,r26", "d2fafaf8");
        }

        [Test]
        public void PaRiscDis_add_l_0b0f0a1f()
        {
            AssertCode("add_l\tr15,r24,r31", "0b0f0f1f");
        }

        [Test]
        public void PaRiscDis_add_l_0a5f0a0f()
        {
            AssertCode("add_l\tr31,r18,r15", "0a5f5f0f");
        }

        [Test]
        public void PaRiscDis_ldb_42110c0a()
        {
            AssertCode("ldb\t605(r16),r17", "4211110a");
        }

        [Test]
        public void PaRiscDis_cmpiclr_ne_923130c2()
        {
            AssertCode("cmpiclr_<>\t61,r17,r17", "923131c2");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_n_879f3e07()
        {
            AssertCode("cmpib_=_n\t-1,ret0,0xffffff08", "879f9f07");
        }

        [Test]
        public void PaRiscDis_b_l_e81f1ce5()
        {
            AssertCode("b_l\t0xfffffe78,r0", "e81f1fe5");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_80a03a1d()
        {
            AssertCode("cmpb_=\tr0,r5,0xfffffd14", "80a0a01d");
        }

        [Test]
        public void PaRiscDis_sub_09cb040f()
        {
            AssertCode("sub\tr11,r14,r15", "09cbcb0f");
        }

        [Test]
        public void PaRiscDis_andcm_0bef0001()
        {
            AssertCode("andcm\tr15,r31,r1", "0befef01");
        }

        [Test]
        public void PaRiscDis_cmpclr_eq_0a8a2880()
        {
            AssertCode("cmpclr_=\tr10,r20,r0", "0a8a8a80");
        }

        [Test]
        public void PaRiscDis_b_l_e8400388()
        {
            AssertCode("b_l\t0x1cc,rp", "e8404088");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_838f36f5()
        {
            AssertCode("cmpb_=\tr15,ret0,0xfffffb80", "838f8ff5");
        }

        [Test]
        public void PaRiscDis_b_l_e85f0b8d()
        {
            AssertCode("b_l\t0xfffff5cc,rp", "e85f5f8d");
        }

        [Test]
        public void PaRiscDis_b_l_e81f16b5()
        {
            AssertCode("b_l\t0xfffffb60,r0", "e81f1fb5");
        }

        [Test]
        public void PaRiscDis_b_l_e85f0b45()
        {
            AssertCode("b_l\t0xfffff5a8,rp", "e85f5f45");
        }

        [Test]
        public void PaRiscDis_b_l_n_e81f195f()
        {
            AssertCode("b_l_n\t0xfffffcb4,r0", "e81f1f5f");
        }

        [Test]
        public void PaRiscDis_b_l_e81f1685()
        {
            AssertCode("b_l\t0xfffffb48,r0", "e81f1f85");
        }

        [Test]
        public void PaRiscDis_depwi_d59f1c5f()
        {
            AssertCode("depwi\t-1,29,1,r12", "d59f9f5f");
        }

        [Test]
        public void PaRiscDis_depwi_d5bf1c7f()
        {
            AssertCode("depwi\t-1,28,1,r13", "d5bfbf7f");
        }

        [Test]
        public void PaRiscDis_addil_28002000()
        {
            AssertCode("addil\tL%1000,r0,r1", "28000000");
        }

        [Test]
        public void PaRiscDis_addil_28031000()
        {
            AssertCode("addil\tL%6800,r0,r1", "28030300");
        }

        [Test]
        public void PaRiscDis_add_l_09c30a02()
        {
            AssertCode("add_l\tr3,r14,rp", "09c3c302");
        }

        [Test]
        public void PaRiscDis_add_l_09650a03()
        {
            AssertCode("add_l\tr5,r11,r3", "09656503");
        }

        [Test]
        public void PaRiscDis_sub_0a820411()
        {
            AssertCode("sub\trp,r20,r17", "0a828211");
        }

        [Test]
        public void PaRiscDis_b_l_ebff05c5()
        {
            AssertCode("b_l\t0xfffff2e8,r31", "ebffffc5");
        }

        [Test]
        public void PaRiscDis_ldw_mb_4fc33b81()
        {
            AssertCode("ldw_mb\t-240(sp),r3", "4fc3c381");
        }

        [Test]
        public void PaRiscDis_depwi_d53f1c3f()
        {
            AssertCode("depwi\t-1,30,1,r9", "d53f3f3f");
        }

        [Test]
        public void PaRiscDis_b_l_e81f1e45()
        {
            AssertCode("b_l\t0xffffff28,r0", "e81f1f45");
        }

        [Test]
        public void PaRiscDis_b_l_n_e8000412()
        {
            AssertCode("b_l_n\t0x210,r0", "e8000012");
        }

        [Test]
        public void PaRiscDis_b_l_n_e80003d2()
        {
            AssertCode("b_l_n\t0x1f0,r0", "e80000d2");
        }

        [Test]
        public void PaRiscDis_b_l_n_e8000392()
        {
            AssertCode("b_l_n\t0x1d0,r0", "e8000092");
        }

        [Test]
        public void PaRiscDis_b_l_n_e8000352()
        {
            AssertCode("b_l_n\t0x1b0,r0", "e8000052");
        }

        [Test]
        public void PaRiscDis_b_l_n_e80002d2()
        {
            AssertCode("b_l_n\t0x170,r0", "e80000d2");
        }

        [Test]
        public void PaRiscDis_b_l_n_e8000292()
        {
            AssertCode("b_l_n\t0x150,r0", "e8000092");
        }

        [Test]
        public void PaRiscDis_b_l_n_e8000252()
        {
            AssertCode("b_l_n\t0x130,r0", "e8000052");
        }


        [Test]
        public void PaRiscDis_b_l_n_e80001d2()
        {
            AssertCode("b_l_n\t0xf0,r0", "e80000d2");
        }

        [Test]
        public void PaRiscDis_b_l_n_e8000192()
        {
            AssertCode("b_l_n\t0xd0,r0", "e8000092");
        }

        [Test]
        public void PaRiscDis_b_l_n_e8000152()
        {
            AssertCode("b_l_n\t0xb0,r0", "e8000052");
        }

        [Test]
        public void PaRiscDis_b_l_n_e80000d2()
        {
            AssertCode("b_l_n\t0x70,r0", "e80000d2");
        }

        [Test]
        public void PaRiscDis_b_l_n_e8000092()
        {
            AssertCode("b_l_n\t0x50,r0", "e8000092");
        }

        [Test]
        public void PaRiscDis_b_l_n_e8000052()
        {
            AssertCode("b_l_n\t0x30,r0", "e8000052");
        }

        [Test]
        public void PaRiscDis_b_l_n_e8000012()
        {
            AssertCode("b_l_n\t0x10,r0", "e8000012");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1ba5()
        {
            AssertCode("b_l\t0xfffffdd8,rp", "e85f5fa5");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1d25()
        {
            AssertCode("b_l\t0xfffffe98,rp", "e85f5f25");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1cf5()
        {
            AssertCode("b_l\t0xfffffe80,rp", "e85f5ff5");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1cb5()
        {
            AssertCode("b_l\t0xfffffe60,rp", "e85f5fb5");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1c65()
        {
            AssertCode("b_l\t0xfffffe38,rp", "e85f5f65");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1c35()
        {
            AssertCode("b_l\t0xfffffe20,rp", "e85f5f35");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1c15()
        {
            AssertCode("b_l\t0xfffffe10,rp", "e85f5f15");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1af5()
        {
            AssertCode("b_l\t0xfffffd80,rp", "e85f5ff5");
        }

        [Test]
        public void PaRiscDis_stw_ma_6fc30080()
        {
            AssertCode("stw_ma\tr3,40(sp)", "6fc3c380");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_n_801a20c2()
        {
            AssertCode("cmpb_=_n\tr26,r0,0x68", "801a1ac2");
        }

        [Test]
        public void PaRiscDis_cmpb_ule_n_83f4a06a()
        {
            AssertCode("cmpb_ule_n\tr20,r31,0x3c", "83f4f46a");
        }

        [Test]
        public void PaRiscDis_extrw_u_d3ff1ba2()
        {
            AssertCode("extrw_u\tr31,29,30,r31", "d3ffffa2");
        }

        [Test]
        public void PaRiscDis_cmpb_uge_8be04058()
        {
            AssertCode("cmpb_>=\tr0,r31,0x34", "8be0e058");
        }

        [Test]
        public void PaRiscDis_cmpb_gt_88606030()
        {
            AssertCode("cmpb_>\tr0,r3,0x20", "88606030");
        }

        [Test]
        public void PaRiscDis_shladd_l_0be30a84()
        {
            AssertCode("shladd_l\tr3,2,r31,r4", "0be3e384");
        }

        [Test]
        public void PaRiscDis_b_l_ebff17d9()
        {
            AssertCode("b_l\t0xffffebf4,r31", "ebffffd9");
        }

        [Test]
        public void PaRiscDis_ldw_mb_4fc33f81()
        {
            AssertCode("ldw_mb\t-40(sp),r3", "4fc3c381");
        }

        [Test]
        public void PaRiscDis_cmpb_uge_n_8a7f9faf()
        {
            AssertCode("cmpb,>>=,n\tr31,r19,0xffffffdc", "8a7f7faf");
        }

        [Test]
        public void PaRiscDis_cmpb__le__n_801f7f97()
        {
            AssertCode("cmpb__le__n\tr31,r0,0xffffffd0", "801f1f97");
        }

        [Test]
        public void PaRiscDis_subi_97e40000()
        {
            AssertCode("subi\t0,r31,r4", "97e4e400");
        }

        [Test]
        public void PaRiscDis_b_l_ebff1719()
        {
            AssertCode("b_l\t0xffffeb94,r31", "ebffff19");
        }

        [Test]
        public void PaRiscDis_addib_lt_a4825fdd()
        {
            AssertCode("addib_<\t1,r4,0xfffffff4", "a48282dd");
        }


        [Test]
        public void PaRiscDis_b_l_e85f1e4d()
        {
            AssertCode("b_l\t0xffffff2c,rp", "e85f5f4d");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1e15()
        {
            AssertCode("b_l\t0xffffff10,rp", "e85f5f15");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_83e02028()
        {
            AssertCode("cmpb_=\tr0,r31,0x1c", "83e0e028");
        }

        [Test]
        public void PaRiscDis_cmpb_ugt_8be3a178()
        {
            AssertCode("cmpb_>>\tr3,r31,0xc4", "8be3e378");
        }

        [Test]
        public void PaRiscDis_cmpb_ev_8817e140()
        {
            AssertCode("cmpb_ev\tr23,r0,0xa8", "88171740");
        }

        [Test]
        public void PaRiscDis_depwi_d6e01c1f()
        {
            AssertCode("depwi\t0,31,1,r23", "d6e0e01f");
        }

        [Test]
        public void PaRiscDis_b_l_e8000120()
        {
            AssertCode("b_l\t0x98,r0", "e8000020");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_n_83e0219a()
        {
            AssertCode("cmpb_=_n\tr0,r31,0xd4", "83e0e09a");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1135()
        {
            AssertCode("b_l\t0xfffff8a0,rp", "e85f5f35");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_879f2148()
        {
            AssertCode("cmpib_=\t-1,ret0,0xac", "879f9f48");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1375()
        {
            AssertCode("b_l\t0xfffff9c0,rp", "e85f5f75");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_83002108()
        {
            AssertCode("cmpb,=\tr0,r24,0x8c", "83000008");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1b45()
        {
            AssertCode("b_l\t0xfffffda8,rp", "e85f5f45");
        }

        public void PaRiscDis_b_l_e85f0fe5()
        {
            AssertCode("b_l\t0xfffff7f8,rp", "e85f5fe5");
        }

        [Test]
        public void PaRiscDis_b_l_e81f1e5d()
        {
            AssertCode("b_l\t0xffffff34,r0", "e81f1f5d");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_82c03e8d()
        {
            AssertCode("cmpb_=\tr0,r22,0xffffff4c", "82c0c08d");
        }

        [Test]
        public void PaRiscDis_b_l_ebff1401()
        {
            AssertCode("b_l\t0xffffea08,r31", "ebffff01");
        }

        [Test]
        public void PaRiscDis_b_l_e85f137d()
        {
            AssertCode("b_l\t0xfffff9c4,rp", "e85f5f7d");
        }


        [Test]
        public void PaRiscDis_mtctl_037a1840()
        {
            AssertCode("mtctl\tr26,tr3", "037a7a40");
        }

        [Test]
        public void PaRiscDis_mfctl_036008bc()
        {
            AssertCode("mfctl\ttr3,ret0", "036060bc");
        }

        [Test]
        public void PaRiscDis_sth_67d93f35()
        {
            AssertCode("sth\tr25,-66(sp)", "67d9d935");
        }

        [Test]
        public void PaRiscDis_ldh_47d93f35()
        {
            AssertCode("ldh\t-66(sp),r25", "47d9d935");
        }

        [Test]
        public void PaRiscDis_b_l_e85f1fbd()
        {
            AssertCode("b_l\t0xffffffe4,rp", "e85f5fbd");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_801c2030()
        {
            AssertCode("cmpb_=\tret0,r0,0x20", "801c1c30");
        }

        [Test]
        public void PaRiscDis_cmpclr_eq_081c2896()
        {
            AssertCode("cmpclr_=\tret0,r0,r22", "081c1c96");
        }

        [Test]
        public void PaRiscDis_depw_z_d7f6081f()
        {
            AssertCode("depw_z\tr22,31,1,r31", "d7f6f61f");
        }

        [Test]
        public void PaRiscDis_cmpiclr_eq_92742000()
        {
            AssertCode("cmpiclr_=\t0,r19,r20", "92747400");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_879f27d0()
        {
            AssertCode("cmpib_=\t-1,ret0,0x3f0", "879f9fd0");
        }

        [Test]
        public void PaRiscDis_addi_nuv_b680870d()
        {
            AssertCode("addi_nuv\t-7a,r20,r0", "b680800d");
        }

        [Test]
        public void PaRiscDis_ldw_s_0ed42081()
        {
            AssertCode("ldw_s\tr20(r22),r1", "0ed4d481");
        }

        [Test]
        public void PaRiscDis_b_l_n_e80006fa()
        {
            AssertCode("b_l_n\t0x384,r0", "e80000fa");
        }

        [Test]
        public void PaRiscDis_b_l_e80006d8()
        {
            AssertCode("b_l\t0x374,r0", "e80000d8");
        }

        [Test]
        public void PaRiscDis_depwi_z_d6c21b1f()
        {
            AssertCode("depwi_z\t1,7,1,r22", "d6c2c21f");
        }

        [Test]
        public void PaRiscDis_b_l_e80006b0()
        {
            AssertCode("b_l\t0x360,r0", "e80000b0");
        }

        [Test]
        public void PaRiscDis_b_l_e8000688()
        {
            AssertCode("b_l\t0x34c,r0", "e8000088");
        }

        [Test]
        public void PaRiscDis_b_l_e8000660()
        {
            AssertCode("b_l\t0x338,r0", "e8000060");
        }

        [Test]
        public void PaRiscDis_b_l_e8000630()
        {
            AssertCode("b_l\t0x320,r0", "e8000030");
        }

        [Test]
        public void PaRiscDis_b_l_e8000600()
        {
            AssertCode("b_l\t0x308,r0", "e8000000");
        }

        [Test]
        public void PaRiscDis_b_l_e80005e8()
        {
            AssertCode("b_l\t0x2fc,r0", "e80000e8");
        }

        [Test]
        public void PaRiscDis_b_l_e80005d0()
        {
            AssertCode("b_l\t0x2f0,r0", "e80000d0");
        }

        [Test]
        public void PaRiscDis_b_l_e80005a8()
        {
            AssertCode("b_l\t0x2dc,r0", "e80000a8");
        }

        [Test]
        public void PaRiscDis_b_l_e8000580()
        {
            AssertCode("b_l\t0x2c8,r0", "e8000080");
        }

        [Test]
        public void PaRiscDis_cmpib_ule_n_87f4a082()
        {
            AssertCode("cmpib_ule_n\ta,r31,0x48", "87f4f482");
        }

        [Test]
        public void PaRiscDis_shladd_081406d5()
        {
            AssertCode("shladd\tr20,3,r0,r21", "081414d5");
        }

        [Test]
        public void PaRiscDis_ldw_0e75009a()
        {
            AssertCode("ldw\tr21(r19),r26", "0e75759a");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_n_8780202a()
        {
            AssertCode("cmpib_=_n\t0,ret0,0x1c", "8780802a");
        }

        [Test]
        public void PaRiscDis_cmpib_ugt_8ff4bf7d()
        {
            AssertCode("cmpib_>>\ta,r31,0xffffffc4", "8ff4f47d");
        }

        [Test]
        public void PaRiscDis_cmpib_ne_n_8e74201a()
        {
            AssertCode("cmpib_ne_n\ta,r19,0x14", "8e74741a");
        }

        [Test]
        public void PaRiscDis_shladd_080106df()
        {
            AssertCode("shladd\tr1,3,r0,r31", "080101df");
        }

        [Test]
        public void PaRiscDis_ldw_0edf0093()
        {
            AssertCode("ldw\tr31(r22),r19", "0edfdf93");
        }

        [Test]
        public void PaRiscDis_b_l_n_e8000462()
        {
            AssertCode("b_l_n\t0x238,r0", "e8000062");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_n_86c02012()
        {
            AssertCode("cmpib_=_n\t0,r22,0x10", "86c0c012");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_n_88012042()
        {
            AssertCode("cmpb_ne_n\tr1,r0,0x28", "88010142");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_881c2010()
        {
            AssertCode("cmpb_<>\tret0,r0,0x10", "881c1c10");
        }

        [Test]
        public void PaRiscDis_b_l_e8000ab0()
        {
            AssertCode("b_l\t0x560,r0", "e80000b0");
        }

        [Test]
        public void PaRiscDis_b_l_e8000360()
        {
            AssertCode("b_l\t0x1b8,r0", "e8000060");
        }

        [Test]
        public void PaRiscDis_b_l_e8000338()
        {
            AssertCode("b_l\t0x1a4,r0", "e8000038");
        }

        [Test]
        public void PaRiscDis_b_l_e8000310()
        {
            AssertCode("b_l\t0x190,r0", "e8000010");
        }

        [Test]
        public void PaRiscDis_b_l_e80002e8()
        {
            AssertCode("b_l\t0x17c,r0", "e80000e8");
        }

        [Test]
        public void PaRiscDis_b_l_e80002d0()
        {
            AssertCode("b_l\t0x170,r0", "e80000d0");
        }

        [Test]
        public void PaRiscDis_b_l_e80002b0()
        {
            AssertCode("b_l\t0x160,r0", "e80000b0");
        }

        [Test]
        public void PaRiscDis_b_l_e8000280()
        {
            AssertCode("b_l\t0x148,r0", "e8000080");
        }

        [Test]
        public void PaRiscDis_b_l_e8000250()
        {
            AssertCode("b_l\t0x130,r0", "e8000050");
        }
        [Test]
        public void PaRiscDis_b_l_n_e80001f2()
        {
            AssertCode("b_l_n\t0x100,r0", "e80000f2");
        }

        [Test]
        public void PaRiscDis_b_l_e80001d0()
        {
            AssertCode("b_l\t0xf0,r0", "e80000d0");
        }

        [Test]
        public void PaRiscDis_b_l_e80001a8()
        {
            AssertCode("b_l\t0xdc,r0", "e80000a8");
        }

        [Test]
        public void PaRiscDis_cmpb_ne_n_8814202a()
        {
            AssertCode("cmpb_ne_n\tr20,r0,0x1c", "8814142a");
        }

        [Test]
        public void PaRiscDis_b_l_e8000788()
        {
            AssertCode("b_l\t0x3cc,r0", "e8000088");
        }

        [Test]
        public void PaRiscDis_b_l_e8000098()
        {
            AssertCode("b_l\t0x54,r0", "e8000098");
        }

        [Test]
        public void PaRiscDis_and_0ad50201()
        {
            AssertCode("and\tr21,r22,r1", "0ad5d501");
        }

        [Test]
        public void PaRiscDis_cmpib_ne_8f9f3835()
        {
            AssertCode("cmpib_<>\t-1,ret0,0xfffffc20", "8f9f9f35");
        }

        [Test]
        public void PaRiscDis_cmpib_eq_n_8420200a()
        {
            AssertCode("cmpib_=_n\t0,r1,0xc", "8420200a");
        }

        [Test]
        public void PaRiscDis_b_l_e8000618()
        {
            AssertCode("b_l\t0x314,r0", "e8000018");
        }

        [Test]
        public void PaRiscDis_cmpb_eq_n_83fc206a()
        {
            AssertCode("cmpb_=_n\tret0,r31,0x3c", "83fcfc6a");
        }

        [Test]
        public void PaRiscDis_cmpib_ult_n_8686826a()
        {
            AssertCode("cmpib_<<_n\t3,r20,0x13c", "8686866a");
        }

        [Test]
        public void PaRiscDis_cmpib_ne_n_8e802002()
        {
            AssertCode("cmpib_ne_n\t0,r20,0x8", "8e808002");
        }

        [Test]
        public void PaRiscDis_stw_sp()
        {
            AssertCode("stw rp,-18(sp)", "6bc23fd1");
        }
    }
}
