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
using Reko.Core.Configuration;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.zSeries
{
    [TestFixture]
    public class zSeriesRewriterTests : RewriterTestBase
    {
        public zSeriesRewriterTests()
        {
            this.Architecture = new zSeriesArchitecture("zSeries");
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            return Architecture.CreateRewriter(
                new BeImageReader(mem, mem.BaseAddress),
                Architecture.CreateProcessorState(),
                binder,
                host);
        }

        [Test]
        public void zSeriesRw_stmg()
        {
            Given_HexString("EB68F0300024");
            AssertCode(     // stmg	r6,r8,48(r15)
                "0|L--|00100000(6): 6 instructions",
                "1|L--|v3 = r15 + 48",
                "2|L--|Mem0[v3:word64] = r6",
                "3|L--|v3 = v3 + 8",
                "4|L--|Mem0[v3:word64] = r7",
                "5|L--|v3 = v3 + 8",
                "6|L--|Mem0[v3:word64] = r8");
        }

        [Test]
        public void zSeriesRw_larl()
        {
            Given_HexString("C05000000140");
            AssertCode(     // larl	r5,00100280
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r5 = DPB(r5, 0x00100280, 0)");
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
        public void zSeriesRw_la()
        {
            Given_HexString("4140F008");
            AssertCode(     // la	r4,8(r15)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r15 + 8");
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
        public void zSeriesRw_aghi()
        {
            Given_HexString("A7FBFF58");
            AssertCode(     // aghi	r15,-000000A8
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r15 = r15 - 168",
                "2|L--|CC = cond(r15)");
        }

        [Test]
        public void zSeriesRw_sgr()
        {
            Given_HexString("B9090031");
            AssertCode(     // sgr	r3,r1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 - r1");
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
        public void zSeriesRw_stg()
        {
            Given_HexString("E310F0000024");
            AssertCode(     // stg	r1,(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|Mem0[r15:word64] = r1");
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
        public void zSeriesRw_st()
        {
            Given_HexString("5010B0A4");
            AssertCode(     // st	r1,164(r11)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + 164:word32] = (word32) r1");
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
        public void zSeriesRw_clc()
        {
            Given_HexString("D507D0002000");
            AssertCode(     // clc	(8,r13),(r2)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = cond(Mem0[r13:byte] - Mem0[r2:byte])");
        }

        [Test]
        public void zSeriesRw_cli()
        {
            Given_HexString("9500B000");
            AssertCode(     // cli	(r11),00
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(Mem0[r11:byte] - 0x00)");
        }

        [Test]
        public void zSeriesRw_srag()
        {
            Given_HexString("EB330003000A");
            AssertCode(     // srag	r3,r3,00000003
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r3 = r3 >> 3");
        }

        [Test]
        public void zSeriesRw_lghi()
        {
            Given_HexString("A709FFF0");
            AssertCode(     // lghi	r0,-00000010
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = 0xFFFFFFFFFFFFFFF0");
        }

        [Test]
        public void zSeriesRw_ltgr()
        {
            Given_HexString("B9020011");
            AssertCode(     // ltgr	r1,r1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1",
                "2|L--|CC = cond(r1)");
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
                "1|L--|CC = cond(r1 - 0x0000000000000001)");
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
        public void zSeriesRw_je()
        {
            Given_HexString("A7840012");
            AssertCode(     // je	00000876
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(EQ,CC)) branch 00100024");
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
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r1 = r3 >>u 63");
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
                "1|L--|v3 = r15 + 208",
                "2|L--|r6 = Mem0[v3:word64]",
                "3|L--|v3 = v3 + 8",
                "4|L--|r7 = Mem0[v3:word64]",
                "5|L--|v3 = v3 + 8",
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
                "1|L--|v3 = (word32) r1",
                "2|L--|r1 = (int64) v3");
        }

        [Test]
        public void zSeriesRw_mvi()
        {
            Given_HexString("9201B000");
            AssertCode(     // mvi	(r11),01
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11:byte] = 0x01");
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
        public void zSeriesRw_xc()
        {
            Given_HexString("D707F000F000");
            AssertCode(     // xc	(8,r15),(r15)
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v3 = 0x00",
                "2|L--|Mem0[r15:byte] = 0x00",
                "3|L--|CC = cond(v3)");
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
        public void zSeriesRw_j()
        {
            Given_HexString("A7F4001E");
            AssertCode(     // j	000007CA
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 0010003C");
        }

        [Test]
        public void zSeriesRw_ahi()
        {
            Given_HexString("A71AFFFF");
            AssertCode(     // ahi	r1,-00000001
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = (word32) r1 - 1",
                "2|L--|v4 = SLICE(r1, word32, 32)",
                "3|L--|r1 = SEQ(v4, v3)",
                "4|L--|CC = cond(v3)");
        }

        [Test]
        public void zSeriesRw_brctg()
        {
            Given_HexString("A7B7FFF4");
            AssertCode(     // brctg	r11,0000085A
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r11 = r11 - 1",
                "2|T--|if (r11 != 0x0000000000000000) branch 000FFFE8");
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
                "1|L--|v4 = (word32) r1 + (word32) r2",
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
        public void zSeriesRw_nc()
        {
            Given_HexString("D407C038D000");
            AssertCode(     // nc	56(8,r12),(r13)
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v3 = Mem0[r12 + 56:byte] & Mem0[r13:byte]",
                "2|L--|Mem0[r12 + 56:byte] = v3",
                "3|L--|CC = cond(v3)");
        }

        [Test]
        public void zSeriesRw_lhi()
        {
            Given_HexString("A728FFF0");
            AssertCode(     // lhi	r2,+00000010
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = 0xFFFFFFF0");
        }


        [Test]
        public void zSeriesRw_lgf()
        {
            Given_HexString("E314F15C0014");
            AssertCode(     // lgf	r1,348(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r1 = (int64) Mem0[r15 + r4 + 348:int32]");
        }

        [Test]
        public void zSeriesRw_mvz()
        {
            Given_HexString("D207F0B0F160");
            AssertCode(     // mvz	176(8,r15),352(r15)
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v3 = __move_zones(Mem0[r15 + 176:byte], Mem0[r15 + 352:byte])",
                "2|L--|Mem0[r15 + 176:byte] = v3");
        }
    }
}
