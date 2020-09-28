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
using Reko.Arch.RiscV;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.RiscV
{
    [TestFixture]
    public class RiscVRewriterTests : RewriterTestBase
    {
        private readonly RiscVArchitecture arch = new RiscVArchitecture("riscV");
        private readonly Address baseAddr = Address.Ptr64(0x0010000);

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => baseAddr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var segMap = new SegmentMap(baseAddr, new ImageSegment("code", mem, AccessMode.ReadExecute));
            var state = (RiscVState) arch.CreateProcessorState();
            return new RiscVRewriter(arch, new LeImageReader(mem, 0), state, new Frame(arch.WordWidth), host);
        }

        private void Given_RiscVInstructions(params uint[] words)
        {
            byte[] bytes;
            if ((words[0] & 0b11) != 0b11)
            {
                bytes = new byte[] {
                    (byte) words[0],
                    (byte) (words[0] >> 8),
                };
            }
            else
            {
                bytes = words.SelectMany(w => new byte[]
                {
                    (byte) w,
                    (byte) (w >> 8),
                    (byte) (w >> 16),
                    (byte) (w >> 24)
                }).ToArray();
            }
            Given_MemoryArea(new MemoryArea(LoadAddress, bytes));
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RiscV_rw_auipc()
        {
            Given_RiscVInstructions(0xFFFFF517u); // auipc\tgp,0x000FFFFD
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = 000000000000F000");
        }

        [Test]
        public void RiscV_rw_lb()
        {
            Given_RiscVInstructions(0x87010183u);
            AssertCode(
               "0|L--|0000000000010000(4): 1 instructions",
               "1|L--|gp = (int64) Mem0[sp + -1936:int8]");
        }

        [Test]
        public void RiscV_rw_jal_zero()
        {
            Given_RiscVInstructions(0x9F4FF06Fu);
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto 000000000000F1F4");
        }

        [Test]
        public void RiscV_rw_jal_not_zero()
        {
            Given_RiscVInstructions(0x9F4FF0EFu);
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call 000000000000F1F4 (0)");
        }

        [Test]
        public void RiscV_rw_jalr_zero()
        {
            Given_RiscVInstructions(0x00078067); // jalr zero, a5, 0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto a5");
        }

        [Test]
        public void RiscV_rw_jalr_zero_ra()
        {
            Given_RiscVInstructions(0x00008067); // jalr zero,ra,0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void RiscV_rw_jalr_ra()
        {
            Given_RiscVInstructions(0x003780E7);    // jalr ra,a5,0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call a5 + 3 (0)");
        }

        [Test]
        public void RiscV_rw_sd()
        {
            Given_RiscVInstructions(0x19513423u);    // sd\ts5,sp,392
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[sp + 392:word64] = s5");
        }

        [Test]
        public void RiscV_rw_lui()
        {
            Given_RiscVInstructions(0x000114B7u);   // lui s1,0x00000011
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|s1 = 0x0000000000011000");
        }

        [Test]
        public void RiscV_rw_lh()
        {
            Given_RiscVInstructions(0x03131083u);   // lh
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|ra = (int64) Mem0[t1 + 49:int16]");
        }

        [Test]
        public void RiscV_rw_fmadd()
        {
            Given_RiscVInstructions(0x8293FD43);
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|fs10 = ft7 * fs1 + fa6");
        }

        [Test]
        public void RiscV_rw_jal()
        {
            Given_RiscVInstructions(0x9F4FF06F);    // jal\tzero,00000000000FF1F4
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto 000000000000F1F4");
        }

        [Test]
        public void RiscV_rw_addiw()
        {
            Given_RiscVInstructions(0x0087879Bu);    // addiw\ta5,a5,+00000008
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = (int64) ((word32) a5 + 8)");
        }

        [Test]
        public void RiscV_rw_x1()
        {
            Given_RiscVInstructions(0x12E50463u);    // beq\ta0,a4,0000000000100128
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|if (a0 == a4) branch 0000000000010128");
        }

        [Test]
        public void RiscV_rw_jalr()
        {
            Given_RiscVInstructions(0x00078067u);    // jalr\tzero,a5,+00000000
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto a5");
        }

        [Test]
        public void RiscV_rw_or()
        {
            Given_RiscVInstructions(0x01846433u);    // or\ts0,s0,s8
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s0 = s0 | s8");
        }

        [Test]
        public void RiscV_rw_add()
        {
            Given_RiscVInstructions(0x00E787B3u);    // add\ta5,a5,a4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = a5 + a4");
        }

        [Test]
        public void RiscV_rw_and()
        {
            Given_RiscVInstructions(0x00F477B3u);    // and\ta5,s0,a5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = s0 & a5");
        }

        [Test]
        public void RiscV_rw_subw()
        {
            Given_RiscVInstructions(0x40F686BBu);    // subw\ta3,a3,a5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = (int64) (word32) (a3 - a5)");
        }

        [Test]
        public void RiscV_rw_srliw()
        {
            Given_RiscVInstructions(0x0017D71Bu);    // srliw\ta4,a5,00000001
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = (int64) (word32) (a5 >>u 1)");
        }

        [Test]
        public void RiscV_rw_lbu()
        {
            Given_RiscVInstructions(0x00094703u);    // lbu\ta4,s2,+00000000
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = (int64) Mem0[s2:byte]");
        }

        [Test]
        public void RiscV_rw_beq()
        {
            Given_RiscVInstructions(0x00F58063u);    // beq\ta1,a5,0000000000010000
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|if (a1 == a5) branch 0000000000010000");
        }

        [Test]
        public void RiscV_rw_flw()
        {
            Given_RiscVInstructions(0x03492707u);    // flw\tfa4,s2,+00000034
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = Mem0[s2 + 52:real32]");
        }

        [Test]
        public void RiscV_rw_fmv_w_x()
        {
            Given_RiscVInstructions(0xF00007D3u);    // fmv.w.x\tfa5,zero
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa5 = (real32) 0x0000000000000000");
        }

        [Test]
        public void RiscV_rw_fmv_d_x()
        {
            Given_RiscVInstructions(0xE2070753u);    // fmv.d.x\tfa4,a4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = (real64) a4");
        }

        [Test]
        public void RiscV_rw_lwu()
        {
            Given_RiscVInstructions(0x00446703u);    // lwu\ta4,s0,+00000004
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a4 = (int64) Mem0[s0 + 4:uint32]");
        }

        [Test]
        public void RiscV_rw_fcvt_d_s()
        {
            Given_RiscVInstructions(0x42070753u);    // fcvt.d.s\tfa4,fa4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|fa4 = (real64) fa4");
        }

        [Test]
        public void RiscV_rw_feq_s()
        {
            // 1010000 011110111001001111 10100 11
            Given_RiscVInstructions(0xA0F727D3u);    // feq.s\ta5,fa4,fa5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = (word64) ((real32) fa4 == (real32) fa5)");
        }


        [Test]
        public void RiscV_rw_addiw_negative()
        {
            Given_RiscVInstructions(0x0000347D);    // c.addiw\ts0,FFFFFFFFFFFFFFFF
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s0 = (int64) (word32) (s0 + 0xFFFFFFFFFFFFFFFF)");
        }

        [Test]
        public void RiscV_rw_c_sw()
        {
            Given_RiscVInstructions(0xC29C);    // c.sw\ta3,0(a5)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[a5:word32] = (word32) a3");
        }

        [Test]
        public void RiscV_rw_c_sdsp()
        {
            Given_RiscVInstructions(0xE4CE);    // c.sdsp\ts3,00000048
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[sp + 72:word64] = s3");
        }

        [Test]
        public void RiscV_rw_c_beqz()
        {
            Given_RiscVInstructions(0x0000C121);    // c.beqz\ta0,0000000000100040
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a0 == 0x0000000000000000) branch 0000000000010040");
        }

        [Test]
        public void RiscV_rw_c_lui()
        {
            Given_RiscVInstructions(0x00006585);    // c.lui\ta1,00001000
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a1 = 0x0000000001000000");
        }

        [Test]
        public void RiscV_rw_c_ld()
        {
            Given_RiscVInstructions(0x00006568);    // c.ld\ta0,200(a0)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a0 = Mem0[a0 + 200:word64]");
        }

        [Test]
        public void RiscV_rw_c_bnez()
        {
            Given_RiscVInstructions(0x0000EF09);    // c.bnez\ta4,000000000010001A
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a4 != 0x0000000000000000) branch 000000000001001A");
        }

        [Test]
        public void RiscV_rw_remuw()
        {
            Given_RiscVInstructions(0x02C8783B);    // remuw\ta6,a6,a2
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a6 = (int64) (word32) (a6 % a2)");
        }

        [Test]
        public void RiscV_rw_c_li()
        {
            Given_RiscVInstructions(0x00004521);    // c.li\ta0,00000008
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a0 = 0x0000000000000008");
        }


        [Test]
        public void RiscV_rw_c_li_minus3()
        {
            Given_RiscVInstructions(0x00005775);    // c.li\ta4,FFFFFFFFFFFFFFFD
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a4 = 0xFFFFFFFFFFFFFFFD");
        }

        [Test]
        public void RiscV_rw_c_swsp()
        {
            Given_RiscVInstructions(0xC22A);    // c.swsp\ta0,00000080
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[sp + 128:word32] = (word32) a0");
        }

        [Test]
        public void RiscV_rw_c_lwsp()
        {
            Given_RiscVInstructions(0x00004512);    // c.lwsp\ttp,00000044
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|tp = (int64) Mem0[sp + 68:word32]");
        }

        [Test]
        public void RiscV_rw_c_mv()
        {
            Given_RiscVInstructions(0x844E);    // c.mv\ts0,s3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s0 = s3");
        }

        [Test]
        public void RiscV_rw_c_lw()
        {
            Given_RiscVInstructions(0x000043F4);    // c.lw\ta5,68(a3)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = (int64) Mem0[a3 + 68:word32]");
        }

        [Test]
        public void RiscV_rw_divw()
        {
            Given_RiscVInstructions(0x02B4443B);    // divw\ts0,s0,a1
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s0 = (int64) (word32) (s0 / a1)");
        }

        [Test]
        public void RiscV_rw_c_addi16sp()
        {
            Given_RiscVInstructions(0x6169);    // c.addi16sp\t000000D0
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|sp = sp + 208");
        }



        [Test]
        public void RiscV_rw_addiw_sign_extend()
        {
            Given_RiscVInstructions(0x00002301);    // c.addiw\tt1,00000000
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|t1 = (int64) (word32) t1");
        }

        [Test]
        public void RiscV_rw_li()
        {
            Given_RiscVInstructions(0x00004385);    // c.li\tt2,00000001
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|t2 = 0x0000000000000001");
        }

        [Test]
        public void RiscV_rw_beqz()
        {
            Given_RiscVInstructions(0xC3F1);    // c.beqz\ta5,00000000001000C4
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a5 == 0x0000000000000000) branch 00000000000100C4");
        }

        [Test]
        public void RiscV_rw_beqz_backward()
        {
            Given_RiscVInstructions(0xD399);    // c.beqz\ta5,00000000000FFF06
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a5 == 0x0000000000000000) branch 000000000000FF06");
        }

        [Test]
        public void RiscV_rw_c_bnez_backward()
        {
            Given_RiscVInstructions(0xFB05);    // c.bnez\ta4,00000000000FFF30
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|if (a4 != 0x0000000000000000) branch 000000000000FF30");
        }

        [Test]
        public void RiscV_rw_c_addiw()
        {
            Given_RiscVInstructions(0x00002405);    // c.addiw\ts0,00000001
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s0 = (int64) (word32) (s0 + 0x0000000000000001)");
        }

        // Reko: a decoder for RiscV instruction 62696C2F at address 00100000 has not been implemented. (amo)
        [Test]
        [Ignore("ASCII code decoded as text")]
        public void RiscV_rw_62696C2F()
        {
            Given_RiscVInstructions(0x62696C2F);    // @@@
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        // Reko: a decoder for RiscV instruction 2D646C2F at address 00100000 has not been implemented. (amo)
        [Test]
        [Ignore("ASCII code decoded as text")]

        public void RiscV_rw_2D646C2F()
        {
            Given_RiscVInstructions(0x2D646C2F);    // @@@
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        // Reko: a decoder for RiscV instruction 36766373 at address 00100000 has not been implemented. (system)
        [Test]
        [Ignore("ASCII code decoded as text")]
        public void RiscV_rw_36766373()
        {
            Given_RiscVInstructions(0x36766373);    // @@@
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_fldsp()
        {
            Given_RiscVInstructions(0x00003436);    // c.fldsp\tfa3,00000228
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|fa3 = Mem0[sp + 552:real64]");
        }

        // Reko: a decoder for RiscV instruction 312E6F73 at address 00100000 has not been implemented. (system)
        [Test]
        [Ignore("ASCII code decoded as text")]
        public void RiscV_rw_312E6F73()
        {
            Given_RiscVInstructions(0x312E6F73);    // @@@
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_invalid()
        {
            Given_RiscVInstructions(0x00000000);    // invalid
            AssertCode(
                "0|---|0000000000010000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void RiscV_rw_jr_ra()
        {
            Given_RiscVInstructions(0x8082);    // c.jr\tra
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void RiscV_rw_c_or()
        {
            Given_RiscVInstructions(0x8E55);    // c.or\ta2,a3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a2 = a2 | a3");
        }

        [Test]
        public void RiscV_rw_c_and()
        {
            Given_RiscVInstructions(0x8FF5);    // c.and\ta5,a3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = a5 & a3");
        }

        [Test]
        public void RiscV_rw_c_j()
        {
            Given_RiscVInstructions(0x0000B7D5);    // c.j\t00000000001003FC
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|goto 00000000000103FC");
        }

        [Test]
        public void RiscV_rw_c_sub()
        {
            Given_RiscVInstructions(0x8D89);    // c.sub\ta1,a0
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a1 = a1 - a0");
        }

        [Test]
        public void RiscV_rw_c_j_backward()
        {
            Given_RiscVInstructions(0x0000BF1D);    // c.j\t00000000000FFF9E
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|goto 000000000000FF9E");
        }

        [Test]
        public void RiscV_rw_c_addi4spn()
        {
            Given_RiscVInstructions(0x0000101C);    // c.addi4spn\ta5,00000020
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = sp + 32");
        }

        [Test]
        public void RiscV_rw_c_jr()
        {
            Given_RiscVInstructions(0x00008782);    // c.jr\ta5
            AssertCode(
                "0|T--|0000000000010000(2): 1 instructions",
                "1|T--|goto a5");
        }

        [Test]
        public void RiscV_rw_c_subw()
        {
            Given_RiscVInstructions(0x00009D1D);    // c.subw\ta0,a5
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a0 = (int64) (word32) (a0 - a5)");
        }

        [Test]
        public void RiscV_rw_c_addi()
        {
            Given_RiscVInstructions(0x00000785);    // c.addi\ta5,00000001
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = a5 + 0x0000000000000001");
        }

        [Test]
        public void RiscV_rw_c_addw()
        {
            Given_RiscVInstructions(0x00009FB5);    // c.addw\ta5,a3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = (int64) (word32) (a5 + a3)");
        }

        [Test]
        public void RiscV_rw_c_slli()
        {
            Given_RiscVInstructions(0x0000040E);    // c.slli\ts0,03
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s0 = s0 << 3");
        }

        [Test]
        public void RiscV_rw_c_srli()
        {
            Given_RiscVInstructions(0x000083A9);    // c.srli\ta5,0000000A
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = a5 >>u 10");
        }

        [Test]
        public void RiscV_rw_c_srai()
        {
            Given_RiscVInstructions(0x0000977D);    // c.srai\ta4,0000003F
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a4 = a4 >> 63");
        }

        [Test]
        public void RiscV_rw_c_andi()
        {
            Given_RiscVInstructions(0x00008A61);    // c.andi\ta2,00000018
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a2 = a2 & 0x0000000000000018");
        }

        [Test]
        public void RiscV_rw_c_ldsp()
        {
            Given_RiscVInstructions(0x00006BA2);    // c.ldsp\ts0,000001D0
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s0 = Mem0[sp + 464:word64]");
        }

        [Test]
        public void RiscV_rw_c_fld()
        {
            Given_RiscVInstructions(0x00002E64);    // c.fld\tfa2,216(s1)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|fa2 = Mem0[s1 + 216:real64]");
        }

        [Test]
        public void RiscV_jal_ra()
        {
            Given_RiscVInstructions(0x02C000EF);    // jal ra,0000B6A4
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call 000000000001002C (0)");
        }

        [Test]
        public void RiscV_rw_sltiu()
        {
            Given_RiscVInstructions(0x0014B493);	// sltiu	s1,s1,+00000001
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|s1 = (word64) (s1 <u 1)");
        }

        [Test]
        public void RiscV_rw_fsw()
        {
            Given_RiscVInstructions(0x8963A3A7);	// fsw	fs6,8732(a5)
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[a5 + 8732:real32] = (real32) fs6");
        }

        [Test]
        public void RiscV_rw_srl()
        {
            Given_RiscVInstructions(0x02B6D6B3);	// srl	a3,a3,a1
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = a3 >>u a1");
        }

        [Test]
        public void RiscV_rw_fsd()
        {
            Given_RiscVInstructions(0x639435A7);    // fsd	fs9,12632(s0)
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[s0 + 12632:real64] = fs9");
        }

        [Test]
        public void RiscV_rw_sltu()
        {
            Given_RiscVInstructions(0x00A03533);    // sltu\ta0,zero,a0
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = (word64) (a0 != 0x0000000000000000)");
        }

        [Test]
        public void RiscV_rw_slt()
        {
            Given_RiscVInstructions(0x00A7A533);    // slt\ta0,a5,a0
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = (word64) (a5 < a0)");
        }

        [Test]
        public void RiscV_rw_remw()
        {
            Given_RiscVInstructions(0x02D7E6BB);    // remw\ta3,a5,a3
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = (int64) (word32) (a5 % a3)");
        }

        [Test]
        [Ignore("Not ready for fma stuff")]
        public void RiscV_rw_fmsub_s()
        {
            Given_RiscVInstructions(0x6318B5C7);    // fmsub.s\tfa1,fa7,fa7,fa2
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("Not ready for fma stuff")]
        public void RiscV_rw_fnmsub_s()
        {
            Given_RiscVInstructions(0x4789004B);    // fnmsub.s\tft0,fs2,fs8,fs0
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("Not ready for fma stuff")]
        public void RiscV_rw_fnmadd_s()
        {
            Given_RiscVInstructions(0x04B3FDCF);    // fnmadd.s\tfs11,ft7,fa1,ft0
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_divuw()
        {
            Given_RiscVInstructions(0x02C857BB);    // divuw\ta5,a6,a2
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = (int64) (word32) (a6 /u a2)");
        }

        [Test]
        public void RiscV_rw_c_fsd()
        {
            Given_RiscVInstructions(0x0000A604);    // c.fsd\tfa2,8(s1)
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[s1 + 8:real64] = fa2");
        }

        [Test]
        public void RiscV_rw_c_fsdsp()
        {
            Given_RiscVInstructions(0xA7E6);        // c.fsdsp\tfs9,000001C8
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|Mem0[sp + 456:real64] = fs9");
        }
    }
}
