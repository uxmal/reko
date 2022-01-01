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

#pragma warning disable IDE1006

using NUnit.Framework;
using Reko.Arch.zSeries;
using Reko.Core;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.zSeries
{
    [TestFixture]
    public class zSeriesDisassemblerTests : DisassemblerTestBase<zSeriesInstruction>
    {
        private zSeriesArchitecture arch;

        public zSeriesDisassemblerTests()
        {
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        [SetUp]
        public void Setup()
        {
            this.arch = new zSeriesArchitecture(CreateServiceContainer(), "zSeries", new Dictionary<string, object>());
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get; }

        public void AssertCode(string sExp, string machineCode)
        {
            var instr = DisassembleHexBytes(machineCode);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void zSerDasm_ae()
        {
            AssertCode("ae\tf11,-272(r12,r13)", "7ABCDEF0");
        }

        [Test]
        public void zSerDasm_agfi()
        {
            AssertCode("agfi\tr3,00015180", "C23800015180");
        }

        [Test]
        public void zSerDasm_aghi()
        {
            AssertCode("aghi\tr15,-000000B0", "A7FB FF50");
        }

        [Test]
        public void zSerDasm_aghik()
        {
            AssertCode("aghik\tr0,+0001,r1", "EC01000100D9");
        }

        [Test]
        public void zSerDasm_agr()
        {
            AssertCode("agr\tr3,r1", "b9080031");
        }

        [Test]
        public void zSerDasm_agrk()
        {
            AssertCode("agrk\tr1,r2,r1", "B9E81012");
        }

        [Test]
        public void zSerDasm_ahi()
        {
            AssertCode("ahi\tr1,-00000002", "a71afffe");
        }

        [Test]
        public void zSerDasm_ahik()
        {
            AssertCode("ahik\tr0,-0001,r1", "EC01FFFF00D8");
        }

        [Test]
        public void zSerDasm_al()
        {
            AssertCode("al\tr9,578(r11,r4)", "5E9B4242");
        }

        [Test]
        public void zSerDasm_alfi()
        {
            AssertCode("alfi\tr1,7FFFFFFF", "C21B7FFFFFFF");
        }

        [Test]
        public void zSerDasm_algfi()
        {
            AssertCode("algfi\tr1,00015180", "C21A00015180");
        }

        [Test]
        public void zSerDasm_algrk()
        {
            AssertCode("algrk\tr10,r9,r1", "B9EA10A9");
        }

        [Test]
        public void zSerDasm_ar()
        {
            AssertCode("ar\tr1,r2", "1A12");
        }

        [Test]
        public void zSerDasm_ark()
        {
            AssertCode("ark\tr5,r1,r3", "B9F83051");
        }

        [Test]
        public void zSerDasm_au()
        {
            AssertCode("au\tf2,292(r1)", "7E210124");
        }

        [Test]
        public void zSerDasm_aw()
        {
            AssertCode("aw\tf9,578(r2,r4)", "6E924242");
        }

        [Test]
        public void zSerDasm_awr()
        {
            AssertCode("awr\tf3,f1", "2E31");
        }

        [Test]
        public void zSerDasm_bal()
        {
            AssertCode("bal\tr1,578(r1,r4)", "45114242");
        }

        [Test]
        public void zSerDasm_bl()
        {
            AssertCode("bl\t(r14,r15)", "474EF000");
        }

        [Test]
        public void zSerDasm_balr()
        {
            AssertCode("balr\tr14,r11", "05EB");
        }

        [Test]
        public void zSerDasm_basr()
        {
            AssertCode("basr\tr14,r1", "0de1");
        }

        [Test]
        public void zSerDasm_bctr()
        {
            AssertCode("bctr\tr2,r10", "062A");
        }

        [Test]
        public void zSerDasm_ber()
        {
            AssertCode("ber\tr14", "078e");
        }

        [Test]
        public void zSerDasm_bler()
        {
            AssertCode("bler\tr14", "07ce");
        }

        [Test]
        public void zSerDasm_bpp()
        {
            AssertCode("bpp\t04,00108484,270914(r4)", "C74B42424242");
        }

        [Test]
        public void zSerDasm_bprp()
        {
            AssertCode("bprp\t0F,+00000712,+00345678", "C5F712345678");
        }

        [Test]
        public void zSerDasm_br()
        {
            AssertCode("br\tr1", "07f1");
        }

        [Test]
        public void zSerDasm_brxh()
        {
            AssertCode("brxh\tr7,r2,00102020", "84721010");
        }

        [Test]
        public void zSerDasm_brxhg()
        {
            AssertCode("brxhg\tr6,r0,FFDC0088", "EC60FFE60044");
        }

        [Test]
        public void zSerDasm_brxle()
        {
            AssertCode("brxle\tr1,r2,00102020", "85121010");
        }

        [Test]
        public void zSerDasm_brxlg()
        {
            AssertCode("brxlg\tr11,r4,FFEA008A", "ECB4FFED0045");
        }

        [Test]
        public void zSerDasm_c()
        {
            AssertCode("c\tr1,-225(r11)", "5910BF1F");
        }


        [Test]
        public void zSerDasm_cdb()
        {
            AssertCode("cdb\tf2,(r13)", "ED20D0000019");
        }

        [Test]
        public void zSerDasm_ce()
        {
            AssertCode("ce\tf4,578(r12,r4)", "794C4242");
        }

        [Test]
        public void zSerDasm_cfi()
        {
            AssertCode("cfi\tr4,6474E551", "C24D6474E551");
        }

        [Test]
        public void zSerDasm_cgfi()
        {
            AssertCode("cgfi\tr1,7FFFFFFD", "C21C7FFFFFFD");
        }

        [Test]
        public void zSerDasm_cgij()
        {
            AssertCode("cgij\tr3,-01,08,00100014", "EC38000AFF7C");
        }

        [Test]
        public void zSerDasm_cgr()
        {
            AssertCode("cgr\tr1,r2", "B920 0012");
        }

        [Test]
        public void zSerDasm_cgrj()
        {
            AssertCode("cgrj\tr2,r1,08,00100014", "EC21000A8064");
        }

        [Test]
        public void zSerDasm_ch()
        {
            AssertCode("ch\tr14,564(r2,r1)", "49E21234");
        }

        [Test]
        public void zSerDasm_chi()
        {
            AssertCode("chi\tr1,+00000001", "a71e0001");
        }

        [Test]
        public void zSerDasm_cij()
        {
            AssertCode("cij\tr2,+00,0C,00100016", "EC2C000B007E");
        }

        [Test]
        public void zSerDasm_cl()
        {
            AssertCode("cl\tr0,4(r1)", "55001004");
        }

        [Test]
        public void zSerDasm_clc()
        {
            AssertCode("clc\t(8,r13),(r1)", "d507d0001000");
        }

        [Test]
        public void zSerDasm_clcle()
        {
            AssertCode("clcle\tr10,r15,578(r4)", "A9AF4242");
        }

        [Test]
        public void zSerDasm_clfgfi()
        {
            AssertCode("clgfi\tr2,00000004", "C22E00000004");
        }

        [Test]
        public void zSerDasm_clfi()
        {
            AssertCode("clfi\tr2,FFFFFFFF", "C22F FFFF FFFF");
        }

        [Test]
        public void zSerDasm_clg()
        {
            AssertCode("clg\tr1,(r5)", "e31050000021");
        }

        [Test]
        public void zSerDasm_clgfi()
        {
            AssertCode("clgfi\tr4,00000002", "C24E00000002");
        }

        [Test]
        public void zSerDasm_cliy()
        {
            AssertCode("cliy\t-8(r4),00", "EB004FF8FF55");
        }

        [Test]
        public void zSerDasm_clgij()
        {
            AssertCode("clgij\tr0,+02,02,00100022", "EC020011027D");
        }

        [Test]
        public void zSerDasm_cli()
        {
            AssertCode("cli\t(r11),00", "9500b000");
        }

        [Test]
        public void zSerDasm_clgr()
        {
            AssertCode("clgr\tr1,r11", "B921 001B");
        }

        [Test]
        public void zSerDasm_clgrj()
        {
            AssertCode("clgrj\tr0,r1,0C,0010003A", "EC01001DC065");
        }

        [Test]
        public void zSerDasm_clm()
        {
            AssertCode("clm\tr9,16(r1),01", "BD911010");
        }

        [Test]
        public void zSerDasm_clmh()
        {
            AssertCode("clmh\tr11,0A,-326880(r14)", "EBBAE320B020");
        }

        [Test]
        public void zSerDasm_cp()
        {
            AssertCode("cp\t291(6,r0),1383(9,r4)", "F95801234567");
        }

        [Test]
        public void zSerDasm_csg()
        {
            AssertCode("csg\tr1,r3,(r2)", "EB1320000030");
        }

        [Test]
        public void zSerDasm_cvb()
        {
            AssertCode("cvb\tr11,564(r7,r1)", "4FB71234");
        }

        [Test]
        public void zSerDasm_d()
        {
            AssertCode("d\tr7,564(r14,r1)", "5D7E1234");
        }

        [Test]
        public void zSerDasm_dd()
        {
            AssertCode("dd\tf11,-1912(r10,r8)", "6DBA8888");
        }

        [Test]
        public void zSerDasm_ed()
        {
            AssertCode("ed\t291(48,r0),1383(r4)", "DE2F01234567");
        }

        [Test]
        public void zSerDasm_edmk()
        {
            AssertCode("edmk\t1638(87,r6),1638(r6)", "DF5666666666");
        }

        [Test]
        public void zSerDasm_ex()
        {
            AssertCode("ex\tr12,1072(r4)", "44C04430");
        }

        [Test]
        public void zSerDasm_la()
        {
            AssertCode("la\tr4,8(r15)", "4140F008");
        }

        [Test]
        public void zSerDasm_la_abs()
        {
            AssertCode("la\tr7,00000008", "4170 0008");
        }

        [Test]
        public void zSerDasm_lae()
        {
            AssertCode("lae\tr0,564(r1,r1)", "51011234");
        }

        [Test]
        public void zSerDasm_lam()
        {
            AssertCode("lam\tr1,r15,00000124", "9A1F0124");
        }

        [Test]
        public void zSerDasm_lan()
        {
            AssertCode("lan\tr1,r1,(r2)", "EB11200000F4");
        }

        [Test]
        public void zSerDasm_larl()
        {
            AssertCode("larl\tr6,00100262", "C060 0000 0131");
        }

        [Test]
        public void zSerDasm_lay()
        {
            AssertCode("lay\tr15,-160(r15)", "E3F0 FF60 FF71");
        }

        [Test]
        public void zSerDasm_lcer()
        {
            AssertCode("lcer\tf7,f15", "337F");
        }

        [Test]
        public void zSerDasm_lcr()
        {
            AssertCode("lcr\tr4,r7", "1347");
        }

        [Test]
        public void zSerDasm_le()
        {
            AssertCode("le\tf0,16(r1)", "78001010");
        }

        [Test]
        public void zSerDasm_lg()
        {
            AssertCode("lg\tr3,(r15)", "E330 F000 0004");
        }

        [Test]
        public void zSerDasm_lgdr()
        {
            AssertCode("lgdr\tr15,r0", "B3CD 00F0");
        }

        [Test]
        public void zSerDasm_lgfr()
        {
            AssertCode("lgfr\tr1,r1", "b9140011");
        }

        [Test]
        public void zSerDasm_lghi()
        {
            AssertCode("lghi\tr0,-00000010", "A709 FFF0");
        }

        [Test]
        public void zSerDasm_ldgr()
        {
            AssertCode("ldgr\tr2,r11", "B3C1 002B");
        }

        [Test]
        public void zSerDasm_lmg()
        {
            AssertCode("lmg\tr6,r15,208(r15)", "eb6ff0d00004");
        }

        [Test]
        public void zSerDasm_locg()
        {
            //$TODO: locgh
            AssertCode("locg\tr1,02,(r3)", "EB12300000E2");
        }

        [Test]
        public void zSerDasm_lpr()
        {
            AssertCode("lpr\tr0,r0", "1000");
        }

        [Test]
        public void zSerDasm_lpsw()
        {
            AssertCode("lpsw\t16(r1)", "82ED1010");
        }

        [Test]
        public void zSerDasm_lr()
        {
            AssertCode("lr\tr10,r1", "18a1");
        }

        [Test]
        public void zSerDasm_lxdb()
        {
            AssertCode("lxdb\tf0,240(r15)", "ED00F0F00005");
        }

        [Test]
        public void zSerDasm_brctg()
        {
            AssertCode("brctg\tr11,000FFFC8", "A7B7FFE4");
        }

        [Test]
        public void zSerDasm_j()
        {
            AssertCode("j\t0010003C", "a7f4001e");
        }

        [Test]
        public void zSerDasm_j_negative_offset()
        {
            AssertCode("j\t000FFFFC", "a7f4fffe");
        }

        [Test]
        public void zSerDasm_l()
        {
            AssertCode("l\tr1,164(r11)", "5810b0a4");
        }

        [Test]
        public void zSerDasm_laa()
        {
            AssertCode("laa\tr1,r1,16(r4)", "EB11401000F8");
        }

        [Test]
        public void zSerDasm_ld()
        {
            AssertCode("ld\tf7,-18(r14,r7)", "687E7FEE");
        }

        [Test]
        public void zSerDasm_ldy()
        {
            AssertCode("ldy\tr8,14112(r15)", "ED80F7200365");
        }

        [Test]
        public void zSerDasm_lgr()
        {
            AssertCode("lgr\tr1,r2", "b9040012");
        }

        [Test]
        public void zSerDasm_lgrl()
        {
            AssertCode("lgrl\tr2,0010001C", "C428 0000000E");
        }

        [Test]
        public void zSerDasm_loc()
        {
            AssertCode("loc\tr10,08,160(r15)", "EBA8F0A000F2");
        }

        [Test]
        public void zSerDasm_lra()
        {
            AssertCode("lra\tr6,564(r1)", "B1601234");
        }

        [Test]
        public void zSerDasm_ltg()
        {
            AssertCode("ltg\tr1,(r1)", "E310 1000 0002");
        }

        [Test]
        public void zSerDasm_ltgr()
        {
            AssertCode("ltgr\tr1,r1", "b9020011");
        }

        [Test]
        public void zSerDasm_mc()
        {
            AssertCode("mc\t578(r4),FF", "AFFF4242");
        }

        [Test]
        public void zSerDasm_mc_1()
        {
            AssertCode("mc\t00000001,FF", "AFFF0001");
        }

        [Test]
        public void zSerDasm_mdb()
        {
            AssertCode("mdb\tf0,8(r13)", "ED00D008001C");
        }

        [Test]
        public void zSerDasm_mdr()
        {
            AssertCode("mdr\tf8,f1", "2C81");
        }

        [Test]
        public void zSerDasm_meeb()
        {
            AssertCode("meeb\tf0,96(r13)", "ED00D0600017");
        }

        [Test]
        public void zSerDasm_mp()
        {
            AssertCode("mp\t564(6,r1),1656(8,r5)", "FC5712345678");
        }

        [Test]
        public void zSerDasm_ms()
        {
            AssertCode("ms\tf7,00000124", "71700124");
        }

        [Test]
        public void zSerDasm_mseb()
        {
            AssertCode("mseb\tf13,164(r15),f15", "EDF0F0A4D00F");
        }

        [Test]
        public void zSerDasm_msfi()
        {
            AssertCode("msfi\tr4,B6DB6DB7", "C241B6DB6DB7");
        }

        [Test]
        public void zSerDasm_mvck()
        {
            AssertCode("mvck\t578(r6,r4),578(r4),r2", "D96242424242");
        }

        [Test]
        public void zSerDasm_mvcle()
        {
            AssertCode("mvcle\tr4,r10,-1216(r1)", "A84A1B40");
        }

        [Test]
        public void zSerDasm_mvcos()
        {
            AssertCode("mvcos\t2036(r10),-312(r15),r6", "C860A7F4FEC8");
        }

        [Test]
        public void zSerDasm_mvcs()
        {
            AssertCode("mvcs\t-292(r11,r15),-1384(r11),r2", "DBB2FEDCBA98");
        }

        [Test]
        public void zSerDasm_mvhi()
        {
            AssertCode("mvhi\t(r1),0000", "E54C 1000 0000");
        }

        [Test]
        public void zSerDasm_mvi()
        {
            AssertCode("mvi\t(r11),01", "9201b000");
        }

        [Test]
        public void zSerDasm_mvo()
        {
            AssertCode("mvo\t578(4,r4),578(1,r4)", "F13042424242");
        }

        [Test]
        public void zSerDasm_mxd()
        {
            AssertCode("mxd\tf1,-292(r7,r15)", "6717FEDC");
        }

        [Test]
        public void zSerDasm_nr()
        {
            AssertCode("nr\tr1,r2", "1412");
        }

        [Test]
        public void zSerDasm_nrk()
        {
            AssertCode("nrk\tr1,r5,r1", "B9F41015");
        }

        [Test]
        public void zSerDasm_ngr_rr()
        {
            AssertCode("ngr\tr15,r0", "B980 00F0");
        }

        [Test]
        public void zSerDasm_ngrk()
        {
            AssertCode("ngrk\tr0,r3,r1", "B9E41003");
        }

        [Test]
        public void zSerDasm_nopr()
        {
            AssertCode("nopr\tr7", "0707");
        }

        [Test]
        public void zSerDasm_ogrk()
        {
            AssertCode("ogrk\tr1,r2,r3", "B9E63012");
        }

        [Test]
        public void zSerDasm_oi()
        {
            AssertCode("oi\t000000FF,04", "960400FF");
        }

        [Test]
        public void zSerDasm_or()
        {
            AssertCode("or\tr4,r2", "1642");
        }

        [Test]
        public void zSerDasm_pfd()
        {
            AssertCode("pfd\t02,1024(r4)", "E32044000036");
        }


        [Test]
        public void zSerDasm_risbg()
        {
            AssertCode("risbg\tr0,r0,1C,BB,04", "EC001CBB0455");
        }

        [Test]
        public void zSerDasm_rosbg()
        {
            AssertCode("rosbg\tr1,r2,20,2A,00", "EC12202A0056");
        }

        [Test]
        public void zSerDasm_stg()
        {
            AssertCode("stg\tr3,160(r11)", "e330b0a00024");
        }

        [Test]
        public void zSerDasm_stnsm()
        {
            AssertCode("stnsm\t578(r4),D7", "ACD74242");
        }

        [Test]
        public void zSerDasm_stosm()
        {
            AssertCode("stosm\t578(r4),F5", "ADF54242");
        }

        [Test]
        public void zSerDasm_srag()
        {
            AssertCode("srag\tr3,r3,00000001", "eb330001000a");
        }

        [Test]
        public void zSerDasm_srak()
        {
            AssertCode("srak\tr1,r2,00000008", "EB12000800DC");
        }

        [Test]
        public void zSerDasm_srk()
        {
            AssertCode("srk\tr2,r4,r1", "B9F91024");
        }

        [Test]
        public void zSerDasm_srlg()
        {
            AssertCode("srlg\tr1,r3,0000003F", "eb13003f000c");
        }

        [Test]
        public void zSerDasm_srlk()
        {
            AssertCode("srlk\tr2,r1,0000001F", "EB21001F00DE");
        }

        [Test]
        public void zSerDasm_swr()
        {
            AssertCode("swr\tf6,f12", "2F6C");
        }

        [Test]
        public void zSerDasm_bassm()
        {
            AssertCode("bassm\tr13,r4", "0CD4");
        }

        [Test]
        public void zSerDasm_her()
        {
            AssertCode("her\tf2,f14", "342E");
        }

        [Test]
        public void zSerDasm_icm()
        {
            AssertCode("icm\tr1,578(r4),0F", "BF1F4242");
        }

        [Test]
        public void zSerDasm_iilh()
        {
            AssertCode("iilh\tr1,+00004242", "A5124242");
        }

        [Test]
        public void zSerDasm_jh()
        {
            AssertCode("jh\t0010000C", "a7240006");
        }

        [Test]
        public void zSerDasm_jne()
        {
            AssertCode("jne\t00100010", "a7740008");
        }

        [Test]
        public void zSerDasm_je()
        {
            AssertCode("je\t00100016", "a784000b");
        }


        [Test]
        public void zSerDasm_jg()
        {
            AssertCode("jg\t000FFF3A", "c0f4ffffff9d");
        }

        [Test]
        public void zSerDasm_ltr()
        {
            AssertCode("ltr\tr11,r11", "12BB");
        }

        [Test]
        public void zSerDasm_llill()
        {
            AssertCode("llill\tr1,+00004242", "A51F4242");
        }

        [Test]
        public void zSerDasm_clr()
        {
            AssertCode("clr\tr1,r2", "1512");
        }

        [Test]
        public void zSerDasm_clrj()
        {
            AssertCode("clrj\tr1,r6,04,0010114C", "EC1608A64077");
        }

        [Test]
        public void zSerDasm_cr()
        {
            AssertCode("cr\tr2,r10", "192A");
        }

        [Test]
        public void zSerDasm_n()
        {
            AssertCode("n\tr1,578(r4)", "54104242");
        }

        [Test]
        public void zSerDasm_lh()
        {
            AssertCode("lh\tr1,578(r2,r4)", "48124242");
        }

        [Test]
        public void zSerDasm_lper()
        {
            AssertCode("lper\tf0,f0", "3000");
        }

        [Test]
        public void zSerDasm_stc()
        {
            AssertCode("stc\tr12,578(r4)", "42C04242");
        }

        [Test]
        public void zSerDasm_sra()
        {
            AssertCode("sra\tr4,1602(r1)", "8A401642");
        }


        [Test]
        public void zSerDasm_pka()
        {
            AssertCode("pka\t578(r4),578(112,r4)", "E96F42424242");
        }
        [Test]
        public void zSerDasm_pku()
        {
            AssertCode("pku\t00000040,64(34,r0)", "E12100400040");
        }

        [Test]
        public void zSerDasm_sth()
        {
            AssertCode("sth\tr0,26(r1)", "4000101A");
        }

        [Test]
        public void zSerDasm_lpdr()
        {
            AssertCode("lpdr\tf0,f0", "2000");
        }

        [Test]
        public void zSerDasm_std()
        {
            AssertCode("std\tf4,34(r8,r1)", "60481022");
        }

        [Test]
        public void zSerDasm_srl()
        {
            AssertCode("srl\tr2,00000122", "88200122");
        }

        [Test]
        public void zSerDasm_s()
        {
            AssertCode("s\tr1,168(r15)", "5B10F0A8");
        }

        [Test]
        public void zSerDasm_srp()
        {
            AssertCode("srp\t564(11,r1),1656(r5),08", "F0A812345678");
        }

        [Test]
        public void zSerDasm_sr()
        {
            AssertCode("sr\tr4,r7", "1B47");
        }

        [Test]
        public void zSerDasm_sgr()
        {
            AssertCode("sgr\tr1,r2", "b9090012");
        }

        [Test]
        public void zSerDasm_sgrk()
        {
            AssertCode("sgrk\tr5,r1,r0", "B9E90051");
        }

        [Test]
        public void zSerDasm_sigp()
        {
            AssertCode("sigp\tr14,578(r4)", "AEEC 4242");
        }

        [Test]
        public void zSerDasm_ste()
        {
            AssertCode("ste\tf0,578(r1,r4)", "70014242");
        }

        [Test]
        public void zSerDasm_sl()
        {
            AssertCode("sl\tr4,578(r1,r4)", "5F414242");
        }

        [Test]
        public void zSerDasm_slfi()
        {
            AssertCode("slfi\tr2,0000D800", "C2250000D800");
        }

        [Test]
        public void zSerDasm_slgfi()
        {
            AssertCode("slgfi\tr3,00015180", "C23400015180");
        }

        [Test]
        public void zSerDasm_slgrk()
        {
            AssertCode("slgrk\tr1,r2,r1", "B9EB1012");
        }

        [Test]
        public void zSerDasm_sll()
        {
            AssertCode("sll\tr4,578(r4)", "89404242");
        }

        [Test]
        public void zSerDasm_sllg()
        {
            AssertCode("sllg\tr1,r1,00000003", "EB110003000D");
        }

        [Test]
        public void zSerDasm_sllk()
        {
            AssertCode("sllk\tr6,r2,00000001", "EB62000100DF");
        }

        [Test]
        public void zSerDasm_sp()
        {
            AssertCode("sp\t-292(5,r15),-1384(5,r11)", "FB44FEDCBA98");
        }

        [Test]
        public void zSerDasm_ssm()
        {
            AssertCode("ssm\t1552(r1)", "80001610");
        }

        [Test]
        public void zSerDasm_st()
        {
            AssertCode("st\tr1,164(r11)", "5010b0a4");
        }

        [Test]
        public void zSerDasm_stmg()
        {
            AssertCode("stmg\tr14,r15,160(r15)", "EBEF F0A0 0024");
        }

        [Test]
        public void zSerDasm_stmh()
        {
            AssertCode("stmh\tr14,r5,2308(r11)", "EBE5B9040026");
        }

        [Test]
        public void zSerDasm_stm()
        {
            AssertCode("stm\tr0,r0,1092(r4)", "90004444");
        }

        [Test]
        public void zSerDasm_stgrl()
        {
            AssertCode("stgrl\tr1,0010000E", "C41B 00000007");
        }

        [Test]
        public void zSerDasm_sur()
        {
            AssertCode("sur\tf1,f7", "3F17");
        }

        [Test]
        public void zSerDasm_svc()
        {
            AssertCode("svc\t00", "0A00");
        }

        [Test]
        public void zSerDasm_sw()
        {
            AssertCode("sw\tf13,578(r5,r4)", "6FD54242");
        }

        [Test]
        public void zSerDasm_tcxb()
        {
            AssertCode("tcxb\tf8,00000555", "ED8005550012");
        }

        [Test]
        public void zSerDasm_tmy()
        {
            AssertCode("tmy\t-29(r12),01", "EB01CFE3FF51");
        }

        [Test]
        public void zSeriesDasm_tp()
        {
            AssertCode("tp\t-1267(6,r13)", "EB5F DB0D 08C0");
        }

        [Test]
        public void zSerDasm_tr()
        {
            AssertCode("tr\t578(240,r4),578(r4)", "DCEF42424242");
        }

        [Test]
        public void zSerDasm_tragc()
        {
            AssertCode("tracg\tr9,r1,3096(r14)", "EB91EC18000F");
        }

        [Test]
        public void zSerDasm_unpk()
        {
            AssertCode("unpk\t16(2,r1),52(1,r3)", "F31010103034");
        }

        [Test]
        public void zSerDasm_unpka()
        {
            AssertCode("unpka\t16(3,r0),00000010", "EA0200100010");
        }

        [Test]
        public void zSerDasm_vclgd()
        {
            AssertCode("vclgd\tv16,v8", "E7085820A2C0");
        }

        [Test]
        public void zSeriesDis_verim()
        {
            AssertCode("verimg\tv13,v1,v11,B8", "E7D1B7B83472");
        }

        [Test]
        public void zSeriesDis_vfs()
        {
            AssertCode("vfsdb\tv8,v21,v2", "E785 21E9 36E2");
        }

        [Test]
        public void zSerDasm_vl()
        {
            AssertCode("vl\tv8,88(r15)", "E780F0580806");
        }

        [Test]
        public void zSeriesDis_vgbm()
        {
            AssertCode("vgbm\tv5,FD8B", "E755 FD8B F844");
        }

        [Test]
        public void zSeriesDis_vftci()
        {
            AssertCode("vftci\tv23,v28,0FDA", "E77CFDA0DE4A");
        }

        [Test]
        public void zSeriesDasm_vlgv()
        {
            AssertCode("vlgv\tr0,v0,432(r12)", "E700C1B0B921E3095E5268B455A66A29");
        }

        [Test]
        public void zSerDasm_vlm()
        {
            AssertCode("vlm\tv24,v31,160(r15)", "E78FF0A00C36");
        }

        [Test]
        public void zSeriesDis_vmahb()
        {
            AssertCode("vmahb\tv20,v9,v12,v16", "E749C60B02AB");
        }

        [Test]
        public void zSeriesDis_vmxlg()
        {
            AssertCode("vmxlg\tv15,v0,v14", "E7F0E01737FD");
        }

        [Test]
        public void zSeriesDis_vpks()
        {
            AssertCode("vpksf\tv8,v0,v1", "E78015E92997");
        }

        [Test]
        public void zSeriesDasm_vrep()
        {
            AssertCode("vrep\tv31,v10,B654", "E7FAB654564D");
        }

        [Test]
        public void zSeriesDis_vscef()
        {
            AssertCode("vscef\tv18,v31,1034(r8)", "E72F840A2B1B");
        }

        [Test]
        public void zSerDasm_vst()
        {
            AssertCode("vst\tv8,88(r12)", "E780C058080E");
        }

        [Test]
        public void zSerDasm_vstm()
        {
            AssertCode("vstm\tv24,v31,160(r15)", "E78FF0A00C3E");
        }

        [Test]
        public void zSerDasm_x()
        {
            AssertCode("x\tr1,801(r4)", "57104321");
        }

        [Test]
        public void zSerDasm_xc()
        {
            AssertCode("xc\t(8,r15),(r15)", "D707 F000 F000");
        }

        [Test]
        public void zSerDasm_xgrk()
        {
            AssertCode("xgrk\tr1,r7,r1", "B9E71017");
        }

        [Test]
        public void zSerDasm_zap()
        {
            AssertCode("zap\t578(16,r4),578(7,r4)", "F8F642424242");
        }
    }
}
