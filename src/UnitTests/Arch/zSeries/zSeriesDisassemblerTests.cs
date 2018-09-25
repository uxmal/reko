#region License
/*
 * Copyright (C) 1999-2018 John Källén.
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

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            throw new NotImplementedException();
        }

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
            AssertCode("swr\tr6,r12", "2F6C");
        }

        [Test]
        public void zSerDasm_00000CD4()
        {
            AssertCode("bassm\tr13,r4", "0CD4");
        }
    }
}
