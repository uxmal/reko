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
        private RiscVArchitecture arch = new RiscVArchitecture("riscV");
        private Address baseAddr = Address.Ptr64(0x0010000);
        private MemoryArea image;

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            var segMap = new SegmentMap(baseAddr, new ImageSegment("code", image, AccessMode.ReadExecute));
            var state = (RiscVState)arch.CreateProcessorState();
            return new RiscVRewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host);
        }

        public override Address LoadAddress
        {
            get { return baseAddr; }
        }

        protected override MemoryArea RewriteCode(uint[] words)
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

            this.image = new MemoryArea(LoadAddress, bytes);
            var dasm = new RiscVDisassembler(arch, image.CreateLeReader(LoadAddress));
            return image;
        }


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RiscV_rw_auipc()
        {
            Rewrite(0xFFFFF517u); // auipc\tgp,0x000FFFFD
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = 000000000000F000");
        }

        [Test]
        public void RiscV_rw_lb()
        {
            Rewrite(0x87010183u);
            AssertCode(
               "0|L--|0000000000010000(4): 1 instructions",
               "1|L--|gp = (word64) Mem0[sp + -1936:int8]");
        }

        [Test]
        public void RiscV_rw_jal_zero()
        {
            Rewrite(0x9F4FF06Fu);
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto 000000000000F1F4");
        }

        [Test]
        public void RiscV_rw_jal_not_zero()
        {
            Rewrite(0x9F4FF0EFu);
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call 000000000000F1F4 (0)");
        }

        [Test]
        public void RiscV_rw_jalr_zero()
        {
            Rewrite(0x00078067); // jalr zero, a5, 0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto a5");
        }

        [Test]
        public void RiscV_rw_jalr_zero_ra()
        {
            Rewrite(0x00008067); // jalr zero,ra,0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void RiscV_rw_jalr_ra()
        {
            Rewrite(0x003780E7);    // jalr ra,a5,0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call a5 + 3 (0)");
        }

        [Test]
        public void RiscV_rw_sd()
        {
            Rewrite(0x19513423u);    // sd\ts5,sp,392
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[sp + 392:word64] = s5");
        }

        [Test]
        public void RiscV_rw_lui()
        {
            Rewrite(0x000114B7u);   // lui s1,0x00000011
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|s1 = 0x0000000000011000");
        }

        [Test]
        public void RiscV_rw_lh()
        {
            Rewrite(0x03131083u);   // lh
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|ra = (word64) Mem0[t1 + 49:int16]");
        }

        [Test]
        public void RiscV_rw_fmadd()
        {
            Rewrite(0x8293FD43);
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|fs10 = ft7 * fs1 + fa6");
        }

        [Test]
        public void RiscV_rw_jal()
        {
            Rewrite(0x9F4FF06F);    // jal\tzero,00000000000FF1F4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_addiw()
        {
            Rewrite(0x0087879Bu);    // addiw\ta5,a5,+00000008
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = (int64) ((word32) a5 + 8)");
        }

        [Test]
        public void RiscV_rw_x1()
        {
            Rewrite(0x12E50463u);    // beq\ta0,a4,0000000000100128
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_x4()
        {
            // Rewrite(0x02079793u);    // @@@
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_jalr()
        {
            Rewrite(0x00078067u);    // jalr\tzero,a5,+00000000
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_or()
        {
            Rewrite(0x01846433u);    // or\ts0,s0,s8
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_aa()
        {
            Rewrite(0x00E787B3u);    // add\ta5,a5,a4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_and()
        {
            Rewrite(0x00F477B3u);    // and\ta5,s0,a5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = s0 & a5");
        }

        [Test]
        public void RiscV_rw_subw()
        {
            Rewrite(0x40F686BBu);    // subw\ta3,a3,a5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a3 = 4 = (int64) (int32) (a5 >>u 0x000@@@");
        }

        [Test]
        public void RiscV_rw_srliw()
        {
            Rewrite(0x0017D71Bu);    // srliw\ta4,a5,00000001
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_lbu()
        {
            Rewrite(0x00094703u);    // lbu\ta4,s2,+00000000
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_beq()
        {
            Rewrite(0x00F58063u);    // beq\ta1,a5,0000000000010000
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|if (a1 == a5) branch 0000000000010000");
        }

        [Test]
        public void RiscV_rw_flw()
        {
            Rewrite(0x03492707u);    // flw\tfa4,s2,+00000034
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_fmv_s_x()
        {
            Rewrite(0xF00007D3u);    // fmv.s.x\tfa5,zero
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_fmv_d_x()
        {
            Rewrite(0xE2070753u);    // fmv.d.x\tfa4,a4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_lwu()
        {
            Rewrite(0x00446703u);    // lwu\ta4,s0,+00000004
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_fcvt_d_s()
        {
            Rewrite(0x42070753u);    // fcvt.d.s\tfa4,fa4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_feq_s()
        {
            // 1010000 011110111001001111 10100 11
            Rewrite(0xA0F727D3u);    // feq.s\ta5,fa4,fa5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        public void RiscV_rw_addiw_negative()
        {
            Rewrite(0x0000347D);    // c.addiw\ts0,FFFFFFFFFFFFFFFF
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_sw()
        {
            Rewrite(0xC29C);    // c.sw\ta3,0(a5)
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_sdsp()
        {
            Rewrite(0xE4CE);    // c.sdsp\ts3,00000048
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_beqz()
        {
            Rewrite(0x0000C121);    // c.beqz\ta0,0000000000100040
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_lui()
        {
            Rewrite(0x00006585);    // c.lui\ta1,00001000
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_negative_3()
        {
            Rewrite(0x34F5);    // c.addiw\ts1,FFFFFFFFFFFFFFFD
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_ld()
        {
            Rewrite(0x00006568);    // c.ld\ta0,200(a0)
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_bnez()
        {
            Rewrite(0x0000EF09);    // c.bnez\ta4,000000000010001A
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_remuw()
        {
            Rewrite(0x02C8783B);    // remuw\ta6,a6,a2
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_li()
        {
            Rewrite(0x00004521);    // c.li\ta0,00000008
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_swsp()
        {
            Rewrite(0xC22A);    // c.swsp\ta0,00000080
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_li_minus3()
        {
            Rewrite(0x00005775);    // c.li\ta4,FFFFFFFFFFFFFFFD
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_lwsp()
        {
            Rewrite(0x00004512);    // c.lwsp\ttp,00000044
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_mv()
        {
            Rewrite(0x844E);    // c.mv\ts0,s3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|s0 = s3");
        }

        [Test]
        public void RiscV_rw_c_lw()
        {
            Rewrite(0x000043F4);    // c.lw\ta5,68(a3)
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_divw()
        {
            Rewrite(0x02B4443B);    // divw\ts0,s0,a1
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_addi16sp()
        {
            Rewrite(0x6169);    // c.addi16sp\t000000D0
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_beqz_backward()
        {
            Rewrite(0xD399);    // c.beqz\ta5,00000000000FFF06
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_addiw_sign_extend()
        {
            Rewrite(0x00002301);    // c.addiw\tt1,00000000
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_li()
        {
            Rewrite(0x00004385);    // c.li\tt2,00000001
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_beqz_0000C3F1()
        {
            Rewrite(0x0000C3F1);    // c.beqz\ta5,00000000001000C4
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_bnez_backward()
        {
            Rewrite(0xFB05);    // c.bnez\ta4,00000000000FFF30
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_addiw()
        {
            Rewrite(0x00002405);    // c.addiw\ts0,00000001
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        // Reko: a decoder for RiscV instruction 62696C2F at address 00100000 has not been implemented. (amo)
        [Test]
        [Ignore("ASCII code decoded as text")]
        public void RiscV_rw_62696C2F()
        {
            Rewrite(0x62696C2F);    // @@@
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        // Reko: a decoder for RiscV instruction 2D646C2F at address 00100000 has not been implemented. (amo)
        [Test]
        [Ignore("ASCII code decoded as text")]

        public void RiscV_rw_2D646C2F()
        {
            Rewrite(0x2D646C2F);    // @@@
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        // Reko: a decoder for RiscV instruction 36766373 at address 00100000 has not been implemented. (system)
        [Test]
        [Ignore("ASCII code decoded as text")]
        public void RiscV_rw_36766373()
        {
            Rewrite(0x36766373);    // @@@
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_fldsp()
        {
            Rewrite(0x00003436);    // c.fldsp\tfa3,00000228
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        // Reko: a decoder for RiscV instruction 312E6F73 at address 00100000 has not been implemented. (system)
        [Test]
        [Ignore("ASCII code decoded as text")]
        public void RiscV_rw_312E6F73()
        {
            Rewrite(0x312E6F73);    // @@@
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_invalid()
        {
            Rewrite(0x00000000);    // invalid
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|invalid");
        }

        [Test]
        public void RiscV_rw_jr_ra()
        {
            Rewrite(0x00008082);    // c.jr\tra
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_or()
        {
            Rewrite(0x8E55);    // c.or\ta2,a3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a2 = a2 | a3");
        }

        [Test]
        public void RiscV_rw_c_and()
        {
            Rewrite(0x8FF5);    // c.and\ta5,a3
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = a5 & a3");
        }

        [Test]
        public void RiscV_rw_c_j()
        {
            Rewrite(0x0000B7D5);    // c.j\t00000000001003FC
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|goto 00000000001003FC");
        }

        [Test]
        public void RiscV_rw_c_sub()
        {
            Rewrite(0x8D89);    // c.sub\ta1,a0
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_j_backward()
        {
            Rewrite(0x0000BF1D);    // c.j\t00000000000FFF9E
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_addi4spn()
        {
            Rewrite(0x0000101C);    // c.addi4spn\ta5,00000020
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_jr()
        {
            Rewrite(0x00008782);    // c.jr\ta5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_subw()
        {
            Rewrite(0x00009D1D);    // c.subw\ta0,a5
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_addi()
        {
            Rewrite(0x00000785);    // c.addi\ta5,00000001
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a5 = a5 + 1");
        }

        [Test]
        public void RiscV_rw_c_addw()
        {
            Rewrite(0x00009FB5);    // c.addw\ta5,a3
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a5 = (int64) (int32) a5 + a3");
        }

        [Test]
        public void RiscV_rw_c_srli()
        {
            Rewrite(0x000083A9);    // c.srli\ta5,0000000A
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_srai()
        {
            Rewrite(0x0000977D);    // c.srai\ta4,0000003F
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_andi()
        {
            Rewrite(0x00008A61);    // c.andi\ta2,00000018
            AssertCode(
                "0|L--|0000000000010000(2): 1 instructions",
                "1|L--|a2 = a2 & 0x0000000018");
        }

        [Test]
        public void RiscV_rw_c_ldsp()
        {
            Rewrite(0x00006BA2);    // c.ldsp\ts0,000001D0
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_slli()
        {
            Rewrite(0x0000040E);    // c.slli\ts0,03
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void RiscV_rw_c_fld()
        {
            Rewrite(0x00002E64);    // c.fld\tfa2,216(s1)
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|@@@");
        }
    }
}
