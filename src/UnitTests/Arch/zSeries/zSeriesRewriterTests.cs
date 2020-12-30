#region License
/*
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Configuration;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.zSeries
{
#pragma warning disable IDE1006

    [TestFixture]
    public class zSeriesRewriterTests : RewriterTestBase
    {
        public zSeriesRewriterTests()
        {
            this.Architecture = new zSeriesArchitecture(CreateServiceContainer(), "zSeries", new Dictionary<string, object>());
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            return Architecture.CreateRewriter(
                mem.CreateBeReader(mem.BaseAddress),
                Architecture.CreateProcessorState(),
                binder,
                host);
        }

        [Test]
        public void zSeriesRw_aghi()
        {
            Given_HexString("A7FBFF58");
            AssertCode(     // aghi	r15,-000000A8
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r15 = r15 - 168<i64>",
                "2|L--|CC = cond(r15)");
        }

        [Test]
        public void zSeriesRw_aghik()
        {
            Given_HexString("EC84FFFF00D9");
            AssertCode(     // aghik	r8,FFFF,r4
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r8 = r4 - 1<i64>",
                "2|L--|CC = cond(r8)");
        }

        [Test]
        public void zSeriesRw_agr()
        {
            Given_HexString("B9080031");
            AssertCode(     // agr	r3,r1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r3 + r1",
                "2|L--|CC = cond(r3)");
        }

        [Test]
        public void zSeriesRw_ahi()
        {
            Given_HexString("A71AFFFF");
            AssertCode(     // ahi	r1,-00000001
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = SLICE(r1, word32, 0)",
                "2|L--|v4 = v3 - 1<i32>",
                "3|L--|r1 = SEQ(SLICE(r1, word32, 32), v4)",
                "4|L--|CC = cond(v4)");
        }

        [Test]
        public void zSeriesRw_ahik()
        {
            Given_HexString("EC31076C00D8");
            AssertCode(     // ahik	r3,076C,r1
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v3 = SLICE(r1, word32, 0)",
                "2|L--|v5 = v3 + 1900<i32>",
                "3|L--|r3 = SEQ(SLICE(r3, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_al()
        {
            Given_HexString("5E9BB9E9");
            AssertCode(     // al	r9,-1559(r11,r11)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = SLICE(r9, word32, 0)",
                "2|L--|v5 = v3 + Mem0[(r11 + r11) + -1559<i64>:word32]",
                "3|L--|r9 = SEQ(SLICE(r9, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_ber()
        {
            Given_HexString("078E");
            AssertCode(     // ber	r14
                "0|T--|00100000(2): 2 instructions",
                "1|T--|if (Test(NE,CC)) branch 00100002",
                "2|T--|goto r14");
        }

        [Test]
        public void zSeriesRw_bner()
        {
            Given_HexString("077E");
            AssertCode(     // bner	r14
                "0|T--|00100000(2): 2 instructions",
                "1|T--|if (Test(EQ,CC)) branch 00100002",
                "2|T--|goto r14");
        }

        [Test]
        public void zSeriesRw_br()
        {
            Given_HexString("07FE");
            AssertCode(     // br	r14
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto r14");
        }

        [Test]
        public void zSeriesRw_brasl()
        {
            Given_HexString("C0E5FFFFFE95");
            AssertCode(     // brasl	r14,00000560
                "0|T--|00100000(6): 2 instructions",
                "1|L--|r14 = 00100006",
                "2|T--|call 000FFD2A (0)");
        }

        [Test]
        public void zSeriesRw_brctg()
        {
            Given_HexString("A7B7FFF4");
            AssertCode(     // brctg	r11,0000085A
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r11 = r11 - 1<i64>",
                "2|T--|if (r11 != 0<64>) branch 000FFFE8");
        }

        [Test]
        public void zSeriesRw_cghi()
        {
            Given_HexString("A71FFFFF");
            AssertCode(     // cghi	r1,-00000001
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(r1 - 0xFFFFFFFFFFFFFFFF<64>)");
        }

        [Test]
        public void zSeriesRw_cgij()
        {
            Given_HexString("EC18008B007C");
            AssertCode(     // cgij	r1,+00,01,0000000080000952
                "0|T--|00100000(6): 1 instructions",
                "1|T--|if (r1 == 0<i64>) branch 00100116");
        }

        [Test]
        public void zSeriesRw_cgr()
        {
            Given_HexString("B9200012");
            AssertCode(     // cgr	r1,r2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(r1 - r2)");
        }

        [Test]
        public void zSeriesRw_cgrj()
        {
            Given_HexString("EC4300348064");
            AssertCode(     // cgrj	r4,r3,08,000000008006CCAE
                "0|T--|00100000(6): 1 instructions",
                "1|T--|if (r4 == r3) branch 00100068");
        }

        [Test]
        public void zSeriesRw_cij()
        {
            Given_HexString("ECC8FFFC077E");
            AssertCode(     // cij	r12,+07,08,000000008006D0A2
                "0|T--|00100000(6): 2 instructions",
                "1|L--|v3 = SLICE(r12, int32, 0)",
                "2|T--|if (v3 == 7<i32>) branch 000FFFF8");
        }

        [Test]
        public void zSeriesRw_clc()
        {
            Given_HexString("D507D0002000");
            AssertCode(     // clc	(8,r13),(r2)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = cond(Mem0[r13:byte] - Mem0[r2:byte])");
        }

        [Test]
        public void zSeriesRw_clfi()
        {
            Given_HexString("C24F00000018");
            AssertCode(     // clfi	r4,00000018
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v3 = SLICE(r4, uint32, 0)",
                "2|L--|CC = cond(v3 - 0x18<u32>)");
        }

        [Test]
        public void zSeriesRw_clg()
        {
            Given_HexString("E31050000021");
            AssertCode(     // clg	r1,(r5)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = cond(r1 - Mem0[r5:byte])");
        }

        [Test]
        public void zSeriesRw_clgij()
        {
            Given_HexString("EC4C0039037D");
            AssertCode(     // clgij	r4,+03,04,000000008003CA5A
                "0|T--|00100000(6): 1 instructions",
                "1|T--|if (r4 <=u 3<64>) branch 00100072");
        }

        [Test]
        public void zSeriesRw_clgr()
        {
            Given_HexString("B921001B");
            AssertCode(     // clgr	r1,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(r1 - r11)");
        }

        [Test]
        public void zSeriesRw_clgrj()
        {
            Given_HexString("EC12000D2065");
            AssertCode(     // clgrj	r1,r2,02,0000000080069FD6
                "0|T--|00100000(6): 1 instructions",
                "1|T--|if (r1 >u r2) branch 0010001A");
        }

        [Test]
        public void zSeriesRw_clgrl()
        {
            Given_HexString("C65A00029D13");
            AssertCode(     // clgrl	r5,000000008009A4B8
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = cond(r5 - Mem0[0x00153A26<p32>:word64])");
        }

        [Test]
        public void zSeriesRw_cli()
        {
            Given_HexString("9500B000");
            AssertCode(     // cli	(r11),00
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(Mem0[r11:byte] - 0<8>)");
        }

        [Test]
        public void zSeriesRw_clij()
        {
            Given_HexString("EC420058027F");
            AssertCode(     // clij	r4,+02,04,000000008005C5E8
                "0|T--|00100000(6): 2 instructions",
                "1|L--|v3 = SLICE(r4, word32, 0)",
                "2|T--|if (v3 >u 2<32>) branch 001000B0");
        }

        [Test]
        public void zSeriesRw_cs()
        {
            Given_HexString("BA121010");
            AssertCode(     // cs	r1,r2,16(r1)
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = SLICE(r1, word32, 0)",
                "2|L--|v5 = SLICE(r2, word32, 0)",
                "3|L--|CC = __compare_and_swap(v3, v5, &Mem0[r1 + 16<i64>:word32], out v3)");
        }

        [Test]
        public void zSeriesRw_csg()
        {
            Given_HexString("EB1240000030");
            AssertCode(     // csg	r1,r2,(r4)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = __compare_and_swap(r1, r2, &Mem0[r4:word64], out r1)");
        }

        [Test]
        public void zSeriesRw_dp()
        {
            Given_HexString("FD8AB90400B2");
            AssertCode(     // dp	-1788(9,r11),178(11,r0)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = __packed_divide(r11, -1788<i32>, r0, 178<i32>, r11)");
        }

        [Test]
        public void zSeriesRw_exrl()
        {
            Given_HexString("C64000000028");
            AssertCode(     // exrl	r4,0000000080018122
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = __execute(r4, 0x00100050<p32>)");
        }

        [Test]
        public void zSeriesRw_la()
        {
            Given_HexString("4140F008");
            AssertCode(     // la	r4,8(r15)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r15 + 8<i64>");
        }

        [Test]
        public void zSeriesRw_larl()
        {
            Given_HexString("C05000000140");
            AssertCode(     // larl	r5,00100280
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r5 = SEQ(SLICE(r5, word32, 32), 0x00100280<p32>)");
        }

        [Test]
        public void zSeriesRw_lay()
        {
            Given_HexString("E3F0FF60FF71");
            AssertCode(     // lay	r15,-160(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r15 = r15 + -160<i64>");
        }

        [Test]
        public void zSeriesRw_lcer()
        {
            Given_HexString("337F");
            AssertCode(     // lcer	f7,f15
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v3 = SLICE(f15, real32, 0)",
                "2|L--|v5 = -v3",
                "3|L--|f7 = SEQ(SLICE(f7, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_ld()
        {
            Given_HexString("6800F0A0");
            AssertCode(     // ld	f0,160(r15)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = Mem0[r15 + 160<i64>:word64]");
        }

        [Test]
        public void zSeriesRw_ldgr()
        {
            Given_HexString("B3C1002B");
            AssertCode(     // ldgr	r2,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f2 = r11");
        }

        [Test]
        public void zSeriesRw_lgfrl()
        {
            Given_HexString("C42C0001ECC8");
            AssertCode(     // lgfrl	r2,000000008009A9A8
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r2 = CONVERT(Mem0[0x0013D990<p32>:int32], int32, int64)");
        }

        [Test]
        public void zSeriesRw_lgf()
        {
            Given_HexString("E314F15C0014");
            AssertCode(     // lgf	r1,348(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r1 = CONVERT(Mem0[r15 + r4 + 348<i64>:int32], int32, int64)");
        }


        [Test]
        public void zSeriesRw_lgr()
        {
            Given_HexString("B904001F");
            AssertCode(     // lgr	r1,r15
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r15");
        }

        [Test]
        public void zSeriesRw_lgrl()
        {
            Given_HexString("C41800000D32");
            AssertCode(     // lgrl	r1,0000000000001FF8
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r1 = Mem0[0x00101A64<p32>:word64]");
        }

        [Test]
        public void zSeriesRw_srag()
        {
            Given_HexString("EB330003000A");
            AssertCode(     // srag	r3,r3,00000003
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r3 = r3 >> 3<i32>",
                "2|L--|CC = cond(r3)");
        }

        [Test]
        public void zSeriesRw_lg()
        {
            Given_HexString("E330F0000004");
            AssertCode(     // lg	r3,(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r3 = Mem0[r15:word64]");
        }

        [Test]
        public void zSeriesRw_lgdr()
        {
            Given_HexString("B3CD00F0");
            AssertCode(     // lgdr	r15,r0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r15 = f0");
        }

        [Test]
        public void zSeriesRw_lghi()
        {
            Given_HexString("A709FFF0");
            AssertCode(     // lghi	r0,-00000010
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = 0xFFFFFFFFFFFFFFF0<64>");
        }

        [Test]
        public void zSeriesRw_lh()
        {
            Given_HexString("4810400C");
            AssertCode(     // lh	r1,12(r4)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r4 + 12<i64>:word16]",
                "2|L--|r1 = SEQ(SLICE(r1, word48, 16), v4)");
        }

        [Test]
        public void zSeriesRw_lhrl()
        {
            Given_HexString("C4250003F737");
            AssertCode(     // lhrl	r2,000000008009A9CE
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v3 = CONVERT(Mem0[0x0017EE6E<p32>:int16], int16, int32)",
                "2|L--|r2 = SEQ(SLICE(r2, word32, 32), v3)");
        }

        [Test]
        public void zSeriesRw_lnr()
        {
            Given_HexString("1122");
            AssertCode(     // lnr	r2,r2
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v3 = SLICE(r2, int32, 0)",
                "2|L--|v4 = -v3",
                "3|L--|r2 = SEQ(SLICE(r2, word32, 32), v4)",
                "4|L--|CC = cond(v4)");
        }

        [Test]
        public void zSeriesRw_locg()
        {
            Given_HexString("EB1C201000E2");
            AssertCode(     // locg	r1,0C,16(r2)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r1 = Mem0[r2 + 16<i64>:word64]");
        }

        [Test]
        public void zSeriesRw_locgre()
        {
            Given_HexString("B9E280D1");
            AssertCode(     // locgre	r13,r1
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,CC)) branch 00100004",
                "2|L--|r13 = r1");
        }

        [Test]
        public void zSeriesRw_locgrne()
        {
            Given_HexString("B9E270C1");
            AssertCode(     // locgrne	r12,r1
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,CC)) branch 00100004",
                "2|L--|r12 = r1");
        }

        [Test]
        public void zSeriesRw_lrl()
        {
            Given_HexString("C41D00028A15");
            AssertCode(     // lrl	r1,000000008009A948
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v3 = Mem0[0x0015142A<p32>:word32]",
                "2|L--|r1 = SEQ(SLICE(r1, word32, 32), v3)");
        }

        [Test]
        public void zSeriesRw_lt()
        {
            Given_HexString("E31010000012");
            AssertCode(     // lt	r1,(r1)
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v3 = Mem0[r1:word32]",
                "2|L--|r1 = SEQ(SLICE(r1, word32, 32), v3)",
                "3|L--|CC = cond(v3 - 0<32>)");
        }

        [Test]
        public void zSeriesRw_ltr()
        {
            Given_HexString("1244");
            AssertCode(     // ltr	r4,r4
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v3 = SLICE(r4, word32, 0)",
                "2|L--|r4 = SEQ(SLICE(r4, word32, 32), v3)",
                "3|L--|CC = cond(v3 - 0<32>)");
        }

        [Test]
        public void zSeriesRw_ltgr()
        {
            Given_HexString("B9020011");
            AssertCode(     // ltgr	r1,r1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1",
                "2|L--|CC = cond(r1 - 0<64>)");
        }

        [Test]
        public void zSeriesRw_bler()
        {
            Given_HexString("07CE");
            AssertCode(     // bler	r14
                "0|T--|00100000(2): 2 instructions",
                "1|T--|if (Test(GT,CC)) branch 00100002",
                "2|T--|goto r14");
        }

        [Test]
        public void zSeriesRw_chi()
        {
            Given_HexString("A71E0001");
            AssertCode(     // chi	r1,+00000001
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(r1 - 1<64>)");
        }


        [Test]
        public void zSeriesRw_je()
        {
            Given_HexString("A7840012");
            AssertCode(     // je	00000876
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(EQ,CC)) branch 00100024");
        }

        [Test]
        public void zSeriesRw_jhe()
        {
            Given_HexString("A7A40015");
            AssertCode(     // jhe	0000000000000630
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(UGE,CC)) branch 0010002A");
        }

        [Test]
        public void zSeriesRw_jne()
        {
            Given_HexString("A7740008");
            AssertCode(     // jne	0000074C
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,CC)) branch 00100010");
        }


        [Test]
        public void zSeriesRw_srlg()
        {
            Given_HexString("EB13003F000C");
            AssertCode(     // srlg	r1,r3,0000003F
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r1 = r3 >>u 63<i32>",
                "2|L--|CC = cond(r1)");
        }

        [Test]
        public void zSeriesRw_nopr()
        {
            Given_HexString("0707");
            AssertCode(     // nopr	r7
                "0|L--|00100000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void zSeriesRw_ngr()
        {
            Given_HexString("B98000F0");
            AssertCode(     // ngr	r15,r0
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r15 = r15 & r0",
                "2|L--|CC = cond(r15)");
        }

        [Test]
        public void zSeriesRw_jh()
        {
            Given_HexString("A7240006");
            AssertCode(     // jh	00000792
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(UGT,CC)) branch 0010000C");
        }

        [Test]
        public void zSeriesRw_lmg()
        {
            Given_HexString("EB68F0D00004");
            AssertCode(     // lmg	r6,r8,208(r15)
                "0|L--|00100000(6): 6 instructions",
                "1|L--|v3 = r15 + 208<i64>",
                "2|L--|r6 = Mem0[v3:word64]",
                "3|L--|v3 = v3 + 8<i64>",
                "4|L--|r7 = Mem0[v3:word64]",
                "5|L--|v3 = v3 + 8<i64>",
                "6|L--|r8 = Mem0[v3:word64]");
        }

        [Test]
        public void zSeriesRw_basr()
        {
            Given_HexString("0DE1");
            AssertCode(     // basr	r14,r1
                "0|T--|00100000(2): 2 instructions",
                "1|L--|r14 = 00100002",
                "2|T--|call r1 (0)");
        }

        [Test]
        public void zSeriesRw_lgfr()
        {
            Given_HexString("B9140011");
            AssertCode(     // lgfr	r1,r1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = CONVERT(r1, word64, word32)",
                "2|L--|r1 = CONVERT(v3, word32, int64)");
        }

        [Test]
        public void zSeriesRw_lpdr()
        {
            Given_HexString("2001");
            AssertCode(     // lpdr	f0,f1
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f0 = fabs(f1)",
                "2|L--|CC = cond(f0)");
        }

        [Test]
        public void zSeriesRw_lper()
        {
            Given_HexString("3001");
            AssertCode(     // lper	f0,f1
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v3 = SLICE(f1, real32, 0)",
                "2|L--|v5 = fabsf(v3)",
                "3|L--|f0 = SEQ(SLICE(f0, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_lpr()
        {
            Given_HexString("1008");
            AssertCode(     // lpr	r0,r8
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v3 = SLICE(r8, int32, 0)",
                "2|L--|v5 = abs(v3)",
                "3|L--|r0 = SEQ(SLICE(r0, word32, 32), v5)");
        }

        [Test]
        public void zSeriesRw_ltg()
        {
            Given_HexString("E31010000002");
            AssertCode(     // ltg	r1,(r1)
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r1 = Mem0[r1:word64]",
                "2|L--|CC = cond(r1 - 0<64>)");
        }

        [Test]
        public void zSeriesRw_mvhi()
        {
            Given_HexString("E54CF0A00004");
            AssertCode(     // mvhi	(r15),0004
                "0|L--|00100000(6): 1 instructions",
                "1|L--|Mem0[r15:word16] = 4<16>");
        }

        [Test]
        public void zSeriesRw_mvi()
        {
            Given_HexString("9201B000");
            AssertCode(     // mvi	(r11),01
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11:byte] = 1<8>");
        }


        [Test]
        public void zSeriesRw_j()
        {
            Given_HexString("A7F4001E");
            AssertCode(     // j	000007CA
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 0010003C");
        }

        [Test]
        public void zSeriesRw_jg()
        {
            Given_HexString("C0F4FFFFFF9D");
            AssertCode(     // jg	00000680
                "0|T--|00100000(6): 1 instructions",
                "1|T--|if (Test(GT,CC)) branch 000FFF3A");
        }

        [Test]
        public void zSeriesRw_ar()
        {
            Given_HexString("1A12");
            AssertCode( // ar\tr1,r2
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v4 = CONVERT(r1, word64, word32) + CONVERT(r2, word64, word32)",
                "2|L--|v5 = SLICE(r1, word32, 32)",
                "3|L--|r1 = SEQ(v5, v4)",
                "4|L--|CC = cond(v4)");
        }

        [Test]
        public void zSeriesRw_lr()
        {
            Given_HexString("18A1");
            AssertCode(     // lr	r10,r1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r10 = r1");
        }

        [Test]
        public void zSeriesRw_lhi()
        {
            Given_HexString("A728FFF0");
            AssertCode(     // lhi	r2,+00000010
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = 0xFFFFFFFFFFFFFFF0<64>");
        }

        [Test]
        public void zSeriesRw_mvz()
        {
            Given_HexString("D207F0B0F160");
            AssertCode(     // mvz	176(8,r15),352(r15)
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v3 = __move_zones(Mem0[r15 + 176<i64>:byte], Mem0[r15 + 352<i64>:byte])",
                "2|L--|Mem0[r15 + 176<i64>:byte] = v3");
        }

        [Test]
        public void zSeriesRw_nc()
        {
            Given_HexString("D407C038D000");
            AssertCode(     // nc	56(8,r12),(r13)
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v3 = Mem0[r12 + 56<i64>:byte] & Mem0[r13:byte]",
                "2|L--|Mem0[r12 + 56<i64>:byte] = v3",
                "3|L--|CC = cond(v3)");
        }

        [Test]
        public void zSeriesRw_sgr()
        {
            Given_HexString("B9090031");
            AssertCode(     // sgr	r3,r1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r3 - r1",
                "2|L--|CC = cond(r3)");
        }

        [Test]
        public void zSeriesRw_stmg()
        {
            Given_HexString("EB68F0300024");
            AssertCode(     // stmg	r6,r8,48(r15)
                "0|L--|00100000(6): 6 instructions",
                "1|L--|v3 = r15 + 48<i64>",
                "2|L--|Mem0[v3:word64] = r6",
                "3|L--|v3 = v3 + 8<i64>",
                "4|L--|Mem0[v3:word64] = r7",
                "5|L--|v3 = v3 + 8<i64>",
                "6|L--|Mem0[v3:word64] = r8");
        }

        [Test]
        public void zSeriesRw_xc()
        {
            Given_HexString("D707F000F000");
            AssertCode(     // xc	(8,r15),(r15)
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v3 = 0<8>",
                "2|L--|Mem0[r15:byte] = 0<8>",
                "3|L--|CC = cond(v3)");
        }

        [Test]
        public void zSeriesRw_std()
        {
            Given_HexString("6080E000");
            AssertCode(     // std	f8,(r14)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r14:word64] = f8");
        }

        [Test]
        public void zSeriesRw_mvcle()
        {
            Given_HexString("A8245000");
            AssertCode(     // mvcle	r2,r4,00000000
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2_r4 = __mvcle(r5)",
                "2|L--|CC = cond(r2_r4)");
        }

        [Test]
        public void zSeriesRw_jle()
        {
            Given_HexString("A7C40009");
            AssertCode(     // jle	000000008006BE94
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(LE,CC)) branch 00100012");
        }

        [Test]
        public void zSeriesRw_risbg()
        {
            Given_HexString("EC223EBF0055");
            AssertCode(     // risbg	r2,r2,3E,BF,00
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r2 = __risbg(r2, 0x3E<8>, 0xBF<8>, 0<8>)",
                "2|L--|CC = cond(r2)");
        }

        [Test]
        public void zSeriesRw_risbgn()
        {
            Given_HexString("ECBB38BF4B59");
            AssertCode(     // risbgn	r11,r11,38,BF,4B
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r11 = __risbgn(r11, 0x38<8>, 0xBF<8>, 0x4B<8>)",
                "2|L--|CC = cond(r11)");
        }

        [Test]
        public void zSeriesRw_ex()
        {
            Given_HexString("44405000");
            AssertCode(     // ex	r4,(r5)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = __execute(r4, Mem0[r5:word32])");
        }

        [Test]
        public void zSeriesRw_ic()
        {
            Given_HexString("43003000");
            AssertCode(     // ic	r0,(r3)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = Mem0[r3:byte]",
                "2|L--|r0 = SEQ(SLICE(r0, word56, 8), v2)");
        }

        [Test]
        public void zSeriesRw_icm()
        {
            Given_HexString("BF0F1000");
            AssertCode(     // icm	r0,(r1),0F
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __insert_char_mask(r0, Mem0[r1:word32], 0xF<8>)",
                "2|L--|v3 = Mem0[r1 + 1:byte]",
                "3|L--|v4 = Mem0[r1 + 2:byte]",
                "4|L--|v5 = Mem0[r1 + 3:byte]");
        }


        [Test]
        public void zSeriesRw_mvo()
        {
            Given_HexString("F1100004EBBF");
            AssertCode(     // mvo	4(2,r0),-1089(1,r14)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_ni()
        {
            Given_HexString("94FE2002");
            AssertCode(     // ni	2(r2),FE
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = Mem0[r2 + 2<i64>:byte] & 0xFE<8>",
                "2|L--|Mem0[r2 + 2<i64>:byte] = v3",
                "3|L--|CC = cond(v3)");
        }

        [Test]
        public void zSeriesRw_oi()
        {
            Given_HexString("96012002");
            AssertCode(     // oi	2(r2),01
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = Mem0[r2 + 2<i64>:byte] | 1<8>",
                "2|L--|Mem0[r2 + 2<i64>:byte] = v3",
                "3|L--|CC = cond(v3)");
        }

        [Test]
        public void zSeriesRw_or()
        {
            Given_HexString("164B");
            AssertCode(     // or	r4,r11
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v3 = SLICE(r4, word32, 0)",
                "2|L--|v5 = SLICE(r11, word32, 0)",
                "3|L--|v6 = v3 | v5",
                "4|L--|r4 = SEQ(SLICE(r4, word32, 32), v6)",
                "5|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_s()
        {
            Given_HexString("5B203010");
            AssertCode(     // s	r2,16(r3)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = SLICE(r2, int32, 0)",
                "2|L--|v5 = v3 - Mem0[r3 + 16<i64>:word32]",
                "3|L--|r2 = SEQ(SLICE(r2, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_sdr()
        {
            Given_HexString("2B7E");
            AssertCode(     // sdr	f7,f14
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f7 = f7 - f14",
                "2|L--|CC = cond(f7)");
        }

        [Test]
        public void zSeriesRw_sl()
        {
            Given_HexString("5F4F4112");
            AssertCode(     // sl	r4,274(r15,r4)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = SLICE(r4, word32, 0)",
                "2|L--|v5 = v3 - Mem0[(r4 + r15) + 274<i64>:word32]",
                "3|L--|r4 = SEQ(SLICE(r4, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_sla()
        {
            Given_HexString("8B9DB904");
            AssertCode(     // sla	r9,-1788(r11)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = SLICE(r9, int32, 0)",
                "2|L--|v4 = v3 << 4<i32>",
                "3|L--|r9 = SEQ(SLICE(r9, word32, 32), v4)",
                "4|L--|CC = cond(v4)");
        }

        [Test]
        public void zSeriesRw_sll()
        {
            Given_HexString("89100001");
            AssertCode(     // sll	r1,00000001
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = SLICE(r1, word32, 0)",
                "2|L--|v4 = v3 << 1<i32>",
                "3|L--|r1 = SEQ(SLICE(r1, word32, 32), v4)",
                "4|L--|CC = cond(v4)");
        }

        [Test]
        public void zSeriesRw_sllg()
        {
            Given_HexString("EB110003000D");
            AssertCode(     // sllg	r1,r1,00000003
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r1 = r1 << 3<i32>",
                "2|L--|CC = cond(r1)");
        }

        [Test]
        public void zSeriesRw_sllk()
        {
            Given_HexString("EB15000100DF");
            AssertCode(     // sllk	r1,r5,00000001
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v3 = SLICE(r5, word32, 0)",
                "2|L--|v5 = v3 << 1<i32>",
                "3|L--|r1 = SEQ(SLICE(r1, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_slr()
        {
            Given_HexString("1F00");
            AssertCode(     // slr	r0,r0
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v3 = SLICE(r0, word32, 0)",
                "2|L--|v4 = SLICE(r0, word32, 0)",
                "3|L--|v5 = v3 - v4", 
                "4|L--|r0 = SEQ(SLICE(r0, word32, 32), v5)",
                "5|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_srl()
        {
            Given_HexString("8820001F");
            AssertCode(     // srl	r2,0000001F
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = SLICE(r2, word32, 0)",
                "2|L--|v4 = v3 >>u 31<i32>",
                "3|L--|r2 = SEQ(SLICE(r2, word32, 32), v4)",
                "4|L--|CC = cond(v4)");
        }

        [Test]
        public void zSeriesRw_srp()
        {
            Given_HexString("F0F8000407F4");
            AssertCode(     // srp	4(16,r0),000007F4,08
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v2 = __shift_and_round_decimal(Mem0[r0 + 16:@@@], Mem0[000007F4:word32], 8<32>",
                "2|L--|Mem0[r0 + 16:@@@] = v2",
                "3|L--|CC = cond(v2)");
        }

        [Test]
        public void zSeriesRw_ssm()
        {
            Given_HexString("80009200");
            AssertCode(     // ssm	512(r9)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_st()
        {
            Given_HexString("5010B0A4");
            AssertCode(     // st	r1,164(r11)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + 164<i64>:word32] = SLICE(r1, word32, 0)");
        }

        [Test]
        public void zSeriesRw_stctl()
        {
            Given_HexString("B6EEE310");
            AssertCode(     // stctl	r14,784(r14)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_stg()
        {
            Given_HexString("E310F0000024");
            AssertCode(     // stg	r1,(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|Mem0[r15:word64] = r1");
        }

        [Test]
        public void zSeriesRw_stgrl()
        {
            Given_HexString("C41B00000D0D");
            AssertCode(     // stgrl	r1,0000000000002028
                "0|L--|00100000(6): 1 instructions",
                "1|L--|Mem0[0x00101A1A<p32>:word64] = r1");
        }

        [Test]
        public void zSeriesRw_sth()
        {
            Given_HexString("4065B904");
            AssertCode(     // sth	r6,-1788(r5,r11)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + r5 + -1788<i64>:word16] = SLICE(r6, word16, 0)");
        }

        [Test]
        public void zSeriesRw_stm()
        {
            Given_HexString("9082EC28");
            AssertCode(     // stm	r8,r2,-984(r14)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_sur()
        {
            Given_HexString("3F17");
            AssertCode(     // sur	f1,f7
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v3 = SLICE(f1, real32, 0)",
                "2|L--|v5 = SLICE(f7, real32, 0)",
                "3|L--|v6 = v3 - v5",
                "4|L--|f1 = SEQ(SLICE(f1, word32, 32), v6)",
                "5|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_svc()
        {
            Given_HexString("0A68");
            AssertCode(     // svc	68
                "0|T--|00100000(2): 1 instructions",
                "1|L--|__syscall(0x68<8>)");
        }

        [Test]
        public void zSeriesRw_tm()
        {
            Given_HexString("91403148");
            AssertCode(     // tm	328(r3),40
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_tmy()
        {
            Given_HexString("EB01CFE3FF51");
            AssertCode(     // tmy	-29(r12),01
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_trtr()
        {
            Given_HexString("D003A7F4FF76");
            AssertCode(     // trtr	2036(4,r10),-138(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_x()
        {
            Given_HexString("5725B904");
            AssertCode(     // x	r2,-1788(r5,r11)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = SLICE(r2, word32, 0)",
                "2|L--|v6 = v3 ^ Mem0[(r11 + r5) + -1788<i64>:word32]",
                "3|L--|r2 = SEQ(SLICE(r2, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

    }
}
