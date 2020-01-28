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
using Reko.Arch.zSeries;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.arch = new zSeriesArchitecture("zSeries");
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get; }

        public void AssertCode(string sExp, string machineCode)
        {
            var instr = DisassembleHexBytes(machineCode);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void zSerDasm_ar()
        {
            AssertCode("ar\tr1,r2", "1A12");
        }

        [Test]
        public void zSerDasm_nr()
        {
            AssertCode("nr\tr1,r2", "1412");
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
        public void zSerDasm_lg()
        {
            AssertCode("lg\tr3,(r15)", "E330 F000 0004");
        }

        [Test]
        public void zSerDasm_lghi()
        {
            AssertCode("lghi\tr0,-00000010", "A709 FFF0");
        }

        [Test]
        public void zSerDasm_ngr_rr()
        {
            AssertCode("ngr\tr15,r0", "B980 00F0");
        }

        [Test]
        public void zSerDasm_xc()
        {
            AssertCode("xc\t(8,r15),(r15)", "D707 F000 F000");
        }

        [Test]
        public void zSerDasm_stmg()
        {
            AssertCode("stmg\tr14,r15,160(r15)", "EBEF F0A0 0024");
        }

        [Test]
        public void zSerDasm_larl()
        {
            AssertCode("larl\tr6,00100262", "C060 0000 0131");
        }

        [Test]
        public void zSerDasm_aghi()
        {
            AssertCode("aghi\tr15,-000000B0", "A7FB FF50");
        }

        [Test]
        public void zSerDasm_nopr()
        {
            AssertCode("nopr\tr7", "0707");
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
        public void zSerDasm_br()
        {
            AssertCode("br\tr1", "07f1");
        }

        [Test]
        public void zSerDasm_basr()
        {
            AssertCode("basr\tr14,r1", "0de1");
        }

        [Test]
        public void zSerDasm_lr()
        {
            AssertCode("lr\tr10,r1", "18a1");
        }

        [Test]
        public void zSerDasm_st()
        {
            AssertCode("st\tr1,164(r11)", "5010b0a4");
        }

        [Test]
        public void zSerDasm_l()
        {
            AssertCode("l\tr1,164(r11)", "5810b0a4");
        }

        [Test]
        public void zSerDasm_mvi()
        {
            AssertCode("mvi\t(r11),01", "9201b000");
        }

        [Test]
        public void zSerDasm_cli()
        {
            AssertCode("cli\t(r11),00", "9500b000");
        }

        [Test]
        public void zSerDasm_ahi()
        {
            AssertCode("ahi\tr1,-00000002", "a71afffe");
        }

        [Test]
        public void zSerDasm_chi()
        {
            AssertCode("chi\tr1,+00000001", "a71e0001");
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
        public void zSerDasm_ltgr()
        {
            AssertCode("ltgr\tr1,r1", "b9020011");
        }

        [Test]
        public void zSerDasm_lgr()
        {
            AssertCode("lgr\tr1,r2", "b9040012");
        }

        [Test]
        public void zSerDasm_agr()
        {
            AssertCode("agr\tr3,r1", "b9080031");
        }

        [Test]
        public void zSerDasm_sgr()
        {
            AssertCode("sgr\tr1,r2", "b9090012");
        }

        [Test]
        public void zSerDasm_lgfr()
        {
            AssertCode("lgfr\tr1,r1", "b9140011");
        }

        [Test]
        public void zSerDasm_jg()
        {
            AssertCode("jg\t000FFF3A", "c0f4ffffff9d");
        }

        [Test]
        public void zSerDasm_clc()
        {
            AssertCode("clc\t(8,r13),(r1)", "d507d0001000");
        }

        [Test]
        public void zSerDasm_clg()
        {
            AssertCode("clg\tr1,(r5)", "e31050000021");
        }

        [Test]
        public void zSerDasm_stg()
        {
            AssertCode("stg\tr3,160(r11)", "e330b0a00024");
        }

        [Test]
        public void zSerDasm_srlg()
        {
            AssertCode("srlg\tr1,r3,0000003F", "eb13003f000c");
        }

        [Test]
        public void zSerDasm_srag()
        {
            AssertCode("srag\tr3,r3,00000001", "eb330001000a");
        }

        [Test]
        public void zSerDasm_lmg()
        {
            AssertCode("lmg\tr6,r15,208(r15)", "eb6ff0d00004");
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
            AssertCode("her\tr2,r14", "342E");
        }

        [Test]
        public void zSerDasm_awr()
        {
            AssertCode("awr\tf3,f1", "2E31");
        }

        [Test]
        public void zSerDasm_bl()
        {
            AssertCode("bl\t(r14,r15)", "474EF000");
        }

        [Test]
        public void zSerDasm_cl()
        {
            AssertCode("cl\tr0,4(r1)", "55001004");
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
            AssertCode("lper\tr0,r0", "3000");
        }

        [Test]
        public void zSerDasm_iilh()
        {
            AssertCode("iilh\tr1,+00004242", "A5124242");
        }

        [Test]
        public void zSerDasm_stc()
        {
            AssertCode("stc\tr12,578(r4)", "42C04242");
        }

        [Test]
        public void zSerDasm_c()
        {
            AssertCode("c\tr1,-225(r11)", "5910BF1F");
        }

        [Test]
        public void zSerDasm_lpr()
        {
            AssertCode("lpr\tr0,r0", "1000");
        }

        [Test]
        public void zSerDasm_lcr()
        {
            AssertCode("lcr\tr4,r7", "1347");
        }

        [Test]
        public void zSerDasm_sra()
        {
            AssertCode("sra\tr4,1602(r1)", "8A401642");
        }

        [Test]
        public void zSerDasm_or()
        {
            AssertCode("or\tr4,r2", "1642");
        }

        [Test]
        public void zSerDasm_sth()
        {
            AssertCode("sth\tr0,26(r1)", "4000101A");
        }

        [Test]
        public void zSerDasm_lpdr()
        {
            AssertCode("lpdr\tr0,r0", "2000");
        }

        [Test]
        public void zSerDasm_ex()
        {
            AssertCode("ex\tr12,1072(r4)", "44C04430");
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
        public void zSerDasm_mvcle()
        {
            AssertCode("mvcle\tr4,r10,-1216(r1)", "A84A1B40");
        }

        [Test]
        public void zSerDasm_sr()
        {
            AssertCode("sr\tr4,r7", "1B47");
        }

        [Test]
        public void zSerDasm_oi()
        {
            AssertCode("oi\t000000FF,04", "960400FF");
        }

        [Test]
        public void zSerDasm_ste()
        {
            AssertCode("ste\tf0,578(r1,r4)", "70014242");
        }

        [Test]
        public void zSerDasm_sll()
        {
            AssertCode("sll\tr4,578(r4)", "89404242");
        }

        [Test]
        public void zSerDasm_icm()
        {
            AssertCode("icm\tr1,578(r4),0F", "BF1F4242");
        }

        [Test]
        public void zSerDasm_mvo()
        {
            AssertCode("mvo\t578(4,r4),578(1,r4)", "F13042424242");
        }

        [Test]
        public void zSerDasm_ssm()
        {
            AssertCode("ssm\t1552(r1)", "80001610");
        }

        [Test]
        public void zSerDasm_lra()
        {
            AssertCode("lra\tr6,564(r1)", "B1601234");
        }

        [Test]
        public void zSerDasm_x()
        {
            AssertCode("x\tr1,801(r4)", "57104321");
        }

        [Test]
        public void zSerDasm_ld()
        {
            AssertCode("ld\tf7,-18(r14,r7)", "687E7FEE");
        }

        [Test]
        public void zSerDasm_au()
        {
            AssertCode("au\tf2,292(r1)", "7E210124");
        }

        [Test]
        public void zSerDasm_stm()
        {
            AssertCode("stm\tr0,r0,1092(r4)", "90004444");
        }

        [Test]
        public void zSerDasm_mxd()
        {
            AssertCode("mxd\tf1,-292(r7,r15)", "6717FEDC");
        }

        [Test]
        public void zSerDasm_mp()
        {
            AssertCode("mp\t564(6,r1),1656(8,r5)", "FC5712345678");
        }

        [Test]
        public void zSerDasm_sp()
        {
            AssertCode("sp\t-292(5,r15),-1384(5,r11)", "FB44FEDCBA98");
        }

        [Test]
        public void zSerDasm_ae()
        {
            AssertCode("ae\tf11,-272(r12,r13)", "7ABCDEF0");
        }

        [Test]
        public void zSerDasm_bctr()
        {
            AssertCode("bctr\tr2,r10", "062A");
        }

        [Test]
        public void zSerDasm_ms()
        {
            AssertCode("ms\tr7,00000124", "71700124");
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
        public void zSerDasm_balr()
        {
            AssertCode("balr\tr14,r11", "05EB");
        }

        [Test]
        public void zSerDasm_edmk()
        {
            AssertCode("edmk\t1638(87,r6),1638(r6)", "DF5666666666");
        }

        [Test]
        public void zSerDasm_lae()
        {
            AssertCode("lae\tr0,564(r1,r1)", "51011234");
        }

        [Test]
        public void zSerDasm_cvb()
        {
            AssertCode("cvb\tr11,564(r7,r1)", "4FB71234");
        }

        [Test]
        public void zSerDasm_mvcs()
        {
            AssertCode("mvcs\t-292(r11,r15),-1384(r11),r2", "DBB2FEDCBA98");
        }

        [Test]
        public void zSerDasm_ch()
        {
            AssertCode("ch\tr14,564(r2,r1)", "49E21234");
        }

        [Test]
        public void zSerDasm_bprp()
        {
            AssertCode("bprp\t0F,+00000712,+00345678", "C5F712345678");
        }

        [Test]
        public void zSerDasm_pku()
        {
            AssertCode("pku\t00000040,64(34,r0)", "E12100400040");
        }

        [Test]
        public void zSerDasm_mdr()
        {
            AssertCode("mdr\tf8,f1", "2C81");
        }

        [Test]
        public void zSerDasm_sllg()
        {
            AssertCode("sllg\tr1,r1,00000003", "EB110003000D");
        }

        [Test]
        public void zSerDasm_clmh()
        {
            AssertCode("clmh\tr11,0A,-326880(r14)", "EBBAE320B020");
        }

        [Test]
        public void zSerDasm_unpka()
        {
            AssertCode("unpka\t16(3,r0),00000010", "EA0200100010");
        }

        [Test]
        public void zSerDasm_lam()
        {
            AssertCode("lam\tr1,r15,00000124", "9A1F0124");
        }
    }
}
