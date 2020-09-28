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
using Reko.Arch.MicroBlaze;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.MicroBlaze
{
    [TestFixture]
    public class MicroBlazeRewriterTests : RewriterTestBase
    {
        private MicroBlazeArchitecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new MicroBlazeArchitecture("microBlaze");
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var state = new MicroBlazeState(arch);
            return arch.CreateRewriter(new BeImageReader(mem, 0), state, binder, host);
        }

        [Test]
        public void MicroBlazeRw_add_r0_r0()
        {
            Given_HexString("00600000"); // add\tr3,r0,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = 0x00000000");
        }

        [Test]
        public void MicroBlazeRw_add_r0()
        {
            Given_HexString("03850000"); // add\tr28,r5,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = r5");
        }

        [Test]
        public void MicroBlazeRw_addc()
        {
            Given_HexString("08631800"); // addc\tr3,r3,r3
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r3 + r3 + C",
                "2|L--|C = cond(r3)");
        }

        [Test]
        public void MicroBlazeRw_addik()
        {
            Given_HexString("3021001C"); // addik\tr1,r1,0000001C
            AssertCode(
                "0|L--|00100000(4): 1 instructions");
        }

        [Test]
        public void MicroBlazeRw_addk()
        {
            Given_HexString("10632800"); // addk\tr3,r3,r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 + r5");
        }

        [Test]
        public void MicroBlazeRw_addk_r0_r0()
        {
            Given_HexString("10600000"); // addk\tr3,r0,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = 0x00000000");
        }

        [Test]
        public void MicroBlazeRw_and()
        {
            Given_HexString("84671800"); // and\tr3,r7,r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r7 & r3");
        }

        [Test]
        public void MicroBlazeRw_andi()
        {
            Given_HexString("A4A400FF"); // andi\tr5,r4,000000FF
            AssertCode(
                "0|L--|00100000(4): 1 instructions");
        }

        [Test]
        public void MicroBlazeRw_beqi()
        {
            Given_HexString("BC13FEB4"); // beqi\tr19,000FFEB4
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(EQ,r19)) branch 000FFEB4");
        }

        [Test]
        public void MicroBlazeRw_beqid()
        {
            Given_HexString("BE03FFD8"); // beqid\tr3,000FFFD8
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(EQ,r3)) branch 000FFFD8");
        }

        [Test]
        public void MicroBlazeRw_bgei()
        {
            Given_HexString("BCA40094"); // bgei\tr4,00100094
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GE,r4)) branch 00100094");
        }

        [Test]
        public void MicroBlazeRw_bgeid()
        {
            Given_HexString("BEB2FFE0"); // bgeid\tr18,000FFFE0
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(GE,r18)) branch 000FFFE0");
        }

        [Test]
        public void MicroBlazeRw_bgtid()
        {
            Given_HexString("BE83FFC8"); // bgtid\tr3,000FFFC8
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(GT,r3)) branch 000FFFC8");
        }

        [Test]
        public void MicroBlazeRw_blei()
        {
            Given_HexString("BC7AFF64"); // blei\tr26,000FFF64
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(LE,r26)) branch 000FFF64");
        }

        [Test]
        public void MicroBlazeRw_blti()
        {
            Given_HexString("BC52FFD4"); // blti\tr18,000FFFD4
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(LT,r18)) branch 000FFFD4");
        }

        [Test]
        public void MicroBlazeRw_bltid()
        {
            Given_HexString("BE52FFD8"); // bltid\tr18,000FFFD8
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(LT,r18)) branch 000FFFD8");
        }

        [Test]
        public void MicroBlazeRw_bnei()
        {
            Given_HexString("BC23FF8C"); // bnei\tr3,000FFF8C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,r3)) branch 000FFF8C");
        }

        [Test]
        public void MicroBlazeRw_bneid()
        {
            Given_HexString("BE36FFC4"); // bneid\tr22,000FFFC4
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(NE,r22)) branch 000FFFC4");
        }

        [Test]
        public void MicroBlazeRw_bra()
        {
            Given_HexString("98081800"); // bra\tr3
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto r3");
        }

        [Test]
        public void MicroBlazeRw_brad()
        {
            Given_HexString("98181800");   // brad	r3
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|goto r3");
        }


        [Test]
        public void MicroBlazeRw_brald()
        {
            Given_HexString("99FC1800");   // brald	r15,r3
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|L--|r15 = 00100000",
                "2|TD-|call 0x00100000 + r3 (0)");
        }

        [Test]
        public void MicroBlazeRw_bri()
        {
            Given_HexString("B800FE40"); // bri\t000FFE40
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 000FFE40");
        }

        [Test]
        public void MicroBlazeRw_brid()
        {
            Given_HexString("B810FFF0"); // brid\t000FFFF0
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|goto 000FFFF0");
        }

        [Test]
        public void MicroBlazeRw_brlid()
        {
            Given_HexString("B9F468D0"); // brlid\tr15,001068D0
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|L--|r15 = 00100000",
                "2|TD-|call 001068D0 (0)");
        }

        [Test]
        public void MicroBlazeRw_cmp()
        {
            Given_HexString("1643B001"); // cmp\tr18,r3,r22
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = cond(r22 - r3)");
        }

        [Test]
        public void MicroBlazeRw_cmpu()
        {
            Given_HexString("1644B803"); // cmpu\tr18,r4,r23
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = cond(r23 -u r4)");
        }

        [Test]
        public void MicroBlazeRw_lbu()
        {
            Given_HexString("C283B800");   // lbu	r20,r3,r23
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r3 + r23:byte]",
                "2|L--|r20 = (word32) v5");
        }

        [Test]
        public void MicroBlazeRw_imm_lbui()
        {
            Given_HexString("B0002000E060D644"); // imm 2000; lbui\tr3,r0,FFFFD644
            AssertCode(
                "0|L--|00100000(8): 2 instructions",
                "1|L--|v3 = Mem0[0x1FFFD644:byte]",
                "2|L--|r3 = (word32) v3");
        }

        [Test]
        public void MicroBlazeRw_lbui()
        {
            Given_HexString("E060D644"); // lbui\tr3,r0,FFFFD644
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = Mem0[0xFFFFD644:byte]",
                "2|L--|r3 = (word32) v3");
        }

        [Test]
        public void MicroBlazeRw_lhu()
        {
            Given_HexString("C4C69800");   // lhu	r6,r6,r19
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r6 + r19:word16]",
                "2|L--|r6 = (word32) v4");
        }

        [Test]
        public void MicroBlazeRw_lhui()
        {
            Given_HexString("E4C40000");   // lhui	r6,r4,00000000
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r4:word16]",
                "2|L--|r6 = (word32) v4");
        }

        [Test]
        public void MicroBlazeRw_lw()
        {
            Given_HexString("C884A800"); // lw\tr4,r4,r21
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = Mem0[r4 + r21:word32]");
        }

        [Test]
        public void MicroBlazeRw_lwi()
        {
            Given_HexString("EAA10028"); // lwi\tr21,r1,00000028
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = Mem0[r1 + 40:word32]");
        }

        [Test]
        public void MicroBlazeRw_mul()
        {
            Given_HexString("40641800"); // mul\tr3,r4,r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r4 * r3");
        }

        [Test]
        public void MicroBlazeRw_neg()
        {
            Given_HexString("24A50000"); // rsubi\tr5,r5,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = -r5");
        }

        [Test]
        public void MicroBlazeRw_or()
        {
            Given_HexString("80641800"); // or\tr3,r4,r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r4 | r3");
        }

        [Test]
        public void MicroBlazeRw_ori()
        {
            Given_HexString("A2D60020");   // ori	r22,r22,00000020
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r22 = r22 | 0x00000020");
        }

        [Test]
        public void MicroBlazeRw_rsub()
        {
            Given_HexString("06453000"); // rsub\tr18,r5,r6
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r18 = r6 - r5",
                "2|L--|C = cond(r18)");
        }

        [Test]
        public void MicroBlazeRw_rsubk()
        {
            Given_HexString("16A3A800"); // rsubk\tr21,r3,r21
            AssertCode(
                "0|L--|00100000(4): 1 instructions");
        }

        [Test]
        public void MicroBlazeRw_rtsd_8()
        {
            Given_HexString("B60F0008"); // rtsd\tr15,00000008
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|return (0,0)");
        }

        [Test]
        public void MicroBlazeRw_sb()
        {
            Given_HexString("D2C41800");   // sb	r22,r4,r3
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r22, byte, 0)",
                "2|L--|Mem0[r4 + r3:byte] = v5");
        }

        [Test]
        public void MicroBlazeRw_sbi()
        {
            Given_HexString("F060D644");   // sbi	r3,r0,FFFFD644
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = SLICE(r3, byte, 0)",
                "2|L--|Mem0[0xFFFFD644:byte] = v3");
        }

        [Test]
        public void MicroBlazeRw_sext8()
        {
            Given_HexString("90630060"); // sext8\tr3,r3
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = SLICE(r3, int8, 0)",
                "2|L--|r3 = (int32) v2");
        }

        [Test]
        public void MicroBlazeRw_sh()
        {
            Given_HexString("D743A000");   // sh	r26,r3,r20
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r26, word16, 0)",
                "2|L--|Mem0[r3 + r20:word16] = v5");
        }

        [Test]
        public void MicroBlazeRw_shi()
        {
            Given_HexString("F5040000");   // shi	r8,r4,00000000
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r8, word16, 0)",
                "2|L--|Mem0[r4:word16] = v4");
        }

        [Test]
        public void MicroBlazeRw_sra()
        {
            Given_HexString("92640001"); // sra\tr19,r4
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r19 = r4 >> 1",
                "2|L--|C = cond(r19)");
        }

        [Test]
        public void MicroBlazeRw_src()
        {
            Given_HexString("90F90021");   // src	r7,r25
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r7 = __rcr(r25, 1, C)",
                "2|L--|C = cond(r7)");
        }

        [Test]
        public void MicroBlazeRw_srl()
        {
            Given_HexString("92A40041"); // srl\tr21,r4
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r21 = r4 >>u 1",
                "2|L--|C = cond(r21)");
        }

        [Test]
        public void MicroBlazeRw_swi()
        {
            Given_HexString("F8650008"); // swi\tr3,r5,00000008
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r5 + 8:word32] = r3");
        }

        [Test]
        public void MicroBlazeRw_xor()
        {
            Given_HexString("88844000"); // xor\tr4,r4,r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r4 ^ r8");
        }

        [Test]
        public void MicroBlazeRw_xori()
        {
            Given_HexString("AAA3FFFF"); // xori\tr21,r3,FFFFFFFF
            AssertCode(
                "0|L--|00100000(4): 1 instructions");
        }
    }
}
