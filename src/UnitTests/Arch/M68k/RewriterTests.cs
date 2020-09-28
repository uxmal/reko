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

using Reko.Arch.M68k;
using Reko.Assemblers.M68k;
using Reko.Core;
using Reko.Core.Rtl;
using Reko.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.M68k
{
    [TestFixture]
    public class RewriterTests : RewriterTestBase
    {
        private readonly M68kArchitecture arch = new M68kArchitecture("m68k");
        private readonly Address addrBase = Address.Ptr32(0x00010000);

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrBase;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var state = arch.CreateProcessorState();
            return arch.CreateRewriter(mem.CreateBeReader(0), state, arch.CreateFrame(), host);
        }

        public void Given_Assembler(Action<M68kAssembler> build)
        {
            var asm = new M68kAssembler(arch, addrBase, new List<ImageSymbol>());
            build(asm);
            var mem = asm.GetImage().SegmentMap.Segments.Values.First().MemoryArea;
            base.Given_MemoryArea(mem);
        }

        [Test]
        public void M68krw_movea_l()
        {
            Given_UInt16s(0x2261);        // movea.l   (a1)-,a1
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|L--|a1 = a1 - 4",
                "2|L--|a1 = Mem0[a1:word32]");
        }

        [Test]
        public void M68krw_Eor_b()
        {
            Given_UInt16s(0xB103);        // eorb %d0,%d3
            AssertCode(
                "0|L--|00010000(2): 6 instructions",
                "1|L--|v4 = SLICE(d3, byte, 0) ^ SLICE(d0, byte, 0)",
                "2|L--|v5 = SLICE(d3, word24, 8)",
                "3|L--|d3 = SEQ(v5, v4)",
                "4|L--|ZN = cond(v4)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_Eor_l()
        {
            Given_UInt16s(0xB183);        // eorl %d0,%d3
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|d3 = d3 ^ d0",
                "2|L--|ZN = cond(d3)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void M68krw_Ext()
        {
            Given_UInt16s(0x4884, 0x48C4, 0x49C4);
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|L--|d4 = (int16) (int8) d4",
                "2|L--|ZN = cond(d4)",
                "3|L--|00010002(2): 2 instructions",
                "4|L--|d4 = (int32) (int16) d4",
                "5|L--|ZN = cond(d4)",
                "6|L--|00010004(2): 2 instructions",
                "7|L--|d4 = (int32) (int8) d4",
                "8|L--|ZN = cond(d4)");
        }

        [Test]
        public void M68krw_subq_areg()
        {
            Given_UInt16s(0x594F);    // subq.w #$4,a7
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|a7 = a7 - 4");
        }

        [Test]
        public void M68krw_adda_postinc() // addal (a4)+,%a5
        {
            Given_UInt16s(0xDBDC);
            AssertCode(
                "0|L--|00010000(2): 3 instructions",
                "1|L--|v3 = Mem0[a4:word32]",
                "2|L--|a4 = a4 + 4",
                "3|L--|a5 = a5 + v3");
        }

        [Test]
        public void M68krw_or_imm()
        {
            Given_UInt16s(0x867c, 0x1123);    // or.w #$1123,d3
            AssertCode(
                "0|L--|00010000(4): 6 instructions",
                "1|L--|v3 = SLICE(d3, word16, 0) | 0x1123",
                "2|L--|v4 = SLICE(d3, word16, 16)",
                "3|L--|d3 = SEQ(v4, v3)",
                "4|L--|ZN = cond(v3)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_movew_indirect()
        {
            Given_UInt16s(0x3410);    // move.w (A0),D2
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v4 = Mem0[a0:word16]",
                "2|L--|v5 = SLICE(d2, word16, 16)",
                "3|L--|d2 = SEQ(v5, v4)",
                "4|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_move_pre_and_postdec()
        {
            Given_UInt16s(0x36E3);    // move.w -(a3),(a3)+
            AssertCode(
                "0|L--|00010000(2): 5 instructions",
                "1|L--|a3 = a3 - 2",
                "2|L--|v3 = Mem0[a3:word16]",
                "3|L--|Mem0[a3:word16] = v3",
                "4|L--|a3 = a3 + 2",
                "5|L--|CVZN = cond(v3)");
        }

        [Test]
        public void M68krw_muls_w()
        {
            Given_UInt16s(0xC1E3); // muls.w -(a3),r3
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|a3 = a3 - 2",
                "2|L--|d0 = d0 *s Mem0[a3:word16]",
                "3|L--|VZN = cond(d0)",
                "4|L--|C = false");
        }

        [Test]
        public void M68krw_mulu_l()
        {
            Given_UInt16s(0x4c00, 0x7406); // mulu.l d0,d6,d7
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|d6_d7 = d7 *u d0",
                "2|L--|VZN = cond(d6_d7)",
                "3|L--|C = false");
        }

        [Test]
        public void M68krw_not_w()
        {
            Given_UInt16s(0x4643); // not.w d3
            AssertCode(
                "0|L--|00010000(2): 6 instructions",
                "1|L--|v3 = ~(word16) d3",
                "2|L--|v4 = SLICE(d3, word16, 16)",
                "3|L--|d3 = SEQ(v4, v3)",
                "4|L--|ZN = cond(v3)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        public void M68krw_not_l_reg()
        {
            Given_UInt16s(0x4684);    // not.l d4
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|d4 = ~d4",
                "2|L--|ZN = cond(d4)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void M68krw_not_l_pre()
        {
            Given_UInt16s(0x46A4);    // not.l -(a4)
            AssertCode(
                "0|L--|00010000(2): 6 instructions",
                "1|L--|a4 = a4 - 4",
                "2|L--|v3 = ~Mem0[a4:word32]",
                "3|L--|Mem0[a4:word32] = v3",
                "4|L--|ZN = cond(v3)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_and_re()
        {
            Given_UInt16s(0xC363);    // and.w d1,-(a3)
            AssertCode(
                "0|L--|00010000(2): 6 instructions",
                "1|L--|a3 = a3 - 2",
                "2|L--|v4 = Mem0[a3:word16] & SLICE(d1, word16, 0)",
                "3|L--|Mem0[a3:word16] = v4",
                "4|L--|ZN = cond(v4)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_andi_32()
        {
            Given_UInt16s(0x029C, 0x0001, 0x0000);    // and.l #00010000,(a4)+
            AssertCode(
                "0|L--|00010000(6): 6 instructions",
                "1|L--|v3 = Mem0[a4:word32] & 0x00010000",
                "2|L--|Mem0[a4:word32] = v3",
                "3|L--|a4 = a4 + 4",
                "4|L--|ZN = cond(v3)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_andi_8()
        {
            Given_UInt16s(0x0202, 0x00F0);     // andi.b #F0,d2"
            AssertCode(
                "0|L--|00010000(4): 6 instructions",
                "1|L--|v3 = SLICE(d2, byte, 0) & 0xF0",
                "2|L--|v4 = SLICE(d2, word24, 8)",
                "3|L--|d2 = SEQ(v4, v3)",
                "4|L--|ZN = cond(v3)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_asrb_qb()
        {
            Given_UInt16s(0xEE00);        // asr.b\t#7,d0
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v3 = SLICE(d0, byte, 0) >> 0x07",
                "2|L--|v4 = SLICE(d0, word24, 8)",
                "3|L--|d0 = SEQ(v4, v3)",
                "4|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_neg_w_post()
        {
            Given_UInt16s(0x445B);
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v3 = -Mem0[a3:word16]",
                "2|L--|Mem0[a3:word16] = v3",
                "3|L--|a3 = a3 + 2",
                "4|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_neg_w_mem()
        {
            Given_UInt16s(0x4453);
            AssertCode(
                "0|L--|00010000(2): 3 instructions",
                "1|L--|v3 = -Mem0[a3:word16]",
                "2|L--|Mem0[a3:word16] = v3",
                "3|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_negx_8()
        {
            Given_UInt16s(0x4021);        // negx.b -(a1)

            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|a1 = a1 - 1",
                "2|L--|v3 = -Mem0[a1:byte] - X",
                "3|L--|Mem0[a1:byte] = v3",
                "4|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_sub_er_16()
        {
            Given_UInt16s(0x9064);        // sub.w -(a4),d0
            AssertCode(
                "0|L--|00010000(2): 5 instructions",
                "1|L--|a4 = a4 - 2",
                "2|L--|v4 = SLICE(d0, word16, 0) - Mem0[a4:word16]",
                "3|L--|v5 = SLICE(d0, word16, 16)",
                "4|L--|d0 = SEQ(v5, v4)",
                "5|L--|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_suba_16()
        {
            Given_UInt16s(0x90DC);      // suba.w (a4)+,a0
            AssertCode(
                "0|L--|00010000(2): 5 instructions",
                "1|L--|v3 = Mem0[a4:word16]",
                "2|L--|a4 = a4 + 2",
                "3|L--|v5 = SLICE(a0, word16, 0) - v3",
                "4|L--|v6 = SLICE(a0, word16, 16)",
                "5|L--|a0 = SEQ(v6, v5)");
        }

        [Test]
        public void M68krw_clrw_ea_off()
        {
            Given_UInt16s(0x4268, 0xFFF8);    // clr.w\t$0008(a0)
            AssertCode(
                "0|L--|00010000(4): 5 instructions",
                "1|L--|Mem0[a0 + -8:word16] = 0x0000",
                "2|L--|Z = true",
                "3|L--|C = false",
                "4|L--|N = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_clrw_reg()
        {
            Given_UInt16s(0x4240);        // clr.w\td0
            AssertCode(
                "0|L--|00010000(2): 6 instructions",
                "1|L--|v4 = SLICE(d0, word16, 16)",
                "2|L--|d0 = SEQ(v4, 0x0000)",
                "3|L--|Z = true",
                "4|L--|C = false",
                "5|L--|N = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_clrb_idx()
        {
            Given_UInt16s(0x4230, 0x0800);
            AssertCode(
                "0|L--|00010000(4): 5 instructions",
                "1|L--|Mem0[a0 + d0:byte] = 0x00",
                "2|L--|Z = true",
                "3|L--|C = false",
                "4|L--|N = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_clrl_postInc()
        {
            Given_UInt16s(0x4298);
            AssertCode(
                "0|L--|00010000(2): 6 instructions",
                "1|L--|Mem0[a0:word32] = 0x00000000",
                "2|L--|a0 = a0 + 4",
                "3|L--|Z = true",
                "4|L--|C = false",
                "5|L--|N = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_cmpib_d()
        {
            Given_UInt16s(0x0C18, 0x0042);    // cmpi.b #$42,(a0)+
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|v3 = Mem0[a0:byte]",
                "2|L--|a0 = a0 + 1",
                "3|L--|v4 = v3 - 0x42",
                "4|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpw_d()
        {
            Given_UInt16s(0xB041);        // cmp.w d1,d0
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|L--|v4 = SLICE(d0, word16, 0) - SLICE(d1, word16, 0)",
                "2|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpw_pre_pre()
        {
            Given_UInt16s(0xB066);        // cmp.w -(a6),d0
            AssertCode(
                "0|L--|00010000(2): 3 instructions",
                "1|L--|a6 = a6 - 2",
                "2|L--|v4 = SLICE(d0, word16, 0) - Mem0[a6:word16]",
                "3|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpaw()
        {
            Given_UInt16s(0xB0EC, 0x0022);    // cmpa.w $22(a4),a0
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|v4 = SLICE(a0, word16, 0) - Mem0[a4 + 34:word16]",
                "2|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpal()
        {
            Given_UInt16s(0xB1EC, 0x0010);    // cmpa.l $10(a4),a0
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|v4 = a0 - Mem0[a4 + 16:word32]",
                "2|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_jsr_mem()
        {
            Given_UInt16s(0x4E90);    // jsr (a0)
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|call a0 (4)");
        }

        [Test]
        public void M68krw_jsr()
        {
            Given_UInt16s(
                0x4EB9, 0x0018, 0x5050, // jsr $00185050
                0x4EB8, 0xFFFA);        // jsr $FFFFFFFA
            AssertCode(
                "0|T--|00010000(6): 1 instructions",
                "1|T--|call 00185050 (4)",
                "2|T--|00010006(4): 1 instructions",
                "3|T--|call 0000FFFA (4)");
        }

        [Test]
        public void M68krw_or_rev()
        {
            Given_UInt16s(0x81A8, 0xFFF8);
            AssertCode(
                "0|L--|00010000(4): 5 instructions",
                "1|L--|v4 = Mem0[a0 + -8:word32] | d0",
                "2|L--|Mem0[a0 + -8:word32] = v4",
                "3|L--|ZN = cond(v4)");
        }

        [Test]
        public void M68krw_lsl_w()
        {
            Given_UInt16s(0xE148);    // lsl.w #$08,d0"
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v3 = SLICE(d0, word16, 0) << 0x0008",
                "2|L--|v4 = SLICE(d0, word16, 16)",
                "3|L--|d0 = SEQ(v4, v3)",
                "4|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_subiw()
        {
            Given_UInt16s(0x0440, 0x0140);    // subiw #320,%d0
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|v3 = SLICE(d0, word16, 0) - 0x0140",
                "2|L--|v4 = SLICE(d0, word16, 16)",
                "3|L--|d0 = SEQ(v4, v3)",
                "4|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_sub_re()
        {
            Given_UInt16s(0x919F);    // sub.l\td0,(a7)+
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v4 = Mem0[a7:word32] - d0",
                "2|L--|Mem0[a7:word32] = v4",
                "3|L--|a7 = a7 + 4",
                "4|L--|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_subq_w()
        {
            Given_UInt16s(0x5F66);    // subq.w\t#$07,-(a6)
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|a6 = a6 - 2",
                "2|L--|v3 = Mem0[a6:word16] - 0x0007",
                "3|L--|Mem0[a6:word16] = v3",
                "4|L--|CVZNX = cond(v3)");
            Given_UInt16s(0x5370, 0x1034);    // subq.w\t#$01,(34,a0,d1)
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|v4 = Mem0[a0 + 52 + d1:word16] - 0x0001",
                "2|L--|Mem0[a0 + 52 + d1:word16] = v4",
                "3|L--|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_rts()
        {
            Given_UInt16s(0x4E75);    // rts
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|return (4,0)");
        }

        [Test]
        public void M68krw_asr_ea()
        {
            Given_UInt16s(0xE0E5);    // asr.w\t-(a5)
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|a5 = a5 - 2",
                "2|L--|v3 = Mem0[a5:word16] >> 1",
                "3|L--|Mem0[a5:word16] = v3",
                "4|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_subx_mm()
        {
            Given_UInt16s(0x9149);   // subx.w\t-(a1),-(a0)
            AssertCode(
                "0|L--|00010000(2): 6 instructions",
                "1|L--|a1 = a1 - 2",
                "2|L--|v4 = Mem0[a1:word16]",
                "3|L--|a0 = a0 - 2",
                "4|L--|v5 = Mem0[a0:word16] - v4 - X",
                "5|L--|Mem0[a0:word16] = v5",
                "6|L--|CVZNX = cond(v5)");
        }

        [Test]
        public void M68krw_lsl_ea()
        {
            Given_UInt16s(0xE3D1);    // lsl.w\t(a1)
            AssertCode(
                "0|L--|00010000(2): 3 instructions",
                "1|L--|v3 = Mem0[a1:word16] << 1",
                "2|L--|Mem0[a1:word16] = v3",
                "3|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_lsl_r()
        {
            Given_UInt16s(0xE36C);    // lsl.w\td1,d4
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v4 = SLICE(d4, word16, 0) << SLICE(d1, word16, 0)",
                "2|L--|v5 = SLICE(d4, word16, 16)",
                "3|L--|d4 = SEQ(v5, v4)",
                "4|L--|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_asl_w()
        {
            Given_Assembler(m => { m.Asl_l(3, m.d1); });   // asl.l #$03,d0"
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|L--|d1 = d1 << 0x00000003",
                "2|L--|CVZNX = cond(d1)");
        }

        [Test]
        [Ignore("Hard to fit into the existing structure.")]
        public void M68krw_bchg_s()
        {
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|v2 = 0x00000001 << 0x00000003",
                "2|L--|v4 = Mem0[a0:word32]",
                "3|L--|Mem0[a0] = v4 ^ v5",
                "4|L--|Z = cond(v4 & v5)");
        }

        [Test]
        public void M68krw_dbra()
        {
            Given_UInt16s(0x51CD, 0xFFFA);        // dbra -$6
            AssertCode(
                "0|T--|00010000(4): 5 instructions",
                "1|L--|v3 = SLICE(d5, word16, 0)",
                "2|L--|v3 = v3 - 1",
                "3|L--|v4 = SLICE(d5, word16, 16)",
                "4|L--|d5 = SEQ(v4, v3)",
                "5|T--|if (v3 != 0xFFFF) branch 0000FFFC");
        }

        [Test]
        public void M68krw_dble()
        {
            Given_UInt16s(0x5FCF, 0xFFFA);
            AssertCode(
                "0|T--|00010000(4): 6 instructions",
                "1|T--|if (Test(GT,VZN)) branch 00010004",
                "2|L--|v4 = SLICE(d7, word16, 0)",
                "3|L--|v4 = v4 - 1",
                "4|L--|v5 = SLICE(d7, word16, 16)",
                "5|L--|d7 = SEQ(v5, v4)",
                "6|T--|if (v4 != 0xFFFF) branch 0000FFFC");
        }

        [Test]
        public void M68krw_unlk()
        {
            Given_UInt16s(0x4E5D);
            AssertCode(
                "0|L--|00010000(2): 3 instructions",
                "1|L--|a7 = a5",
                "2|L--|a5 = Mem0[a7:word32]",
                "3|L--|a7 = a7 + 0x00000004");
        }

        [Test]
        public void M68krw_link()
        {
            Given_UInt16s(0x4E52, 0xFFF8);
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|a7 = a7 - 0x00000004",
                "2|L--|Mem0[a7:word32] = a2",
                "3|L--|a2 = a7",
                "4|L--|a7 = a7 - 0x00000008");
        }

        [Test]
        public void M68krw_link_32()
        {
            Given_UInt16s(0x480B, 0xFFFE, 0x0104);
            AssertCode(
                "0|L--|00010000(6): 4 instructions",
                "1|L--|a7 = a7 - 0x00000004",
                "2|L--|Mem0[a7:word32] = a3",
                "3|L--|a3 = a7",
                "4|L--|a7 = a7 - 0x0001FEFC");
        }

        [Test]
        public void M68krw_movem_pop()
        {
            Given_UInt16s(0x4CDF, 0x4C04);
            AssertCode(
                "0|L--|00010000(4): 8 instructions",
                "1|L--|d2 = Mem0[a7:word32]",
                "2|L--|a7 = a7 + 4",
                "3|L--|a2 = Mem0[a7:word32]",
                "4|L--|a7 = a7 + 4",
                "5|L--|a3 = Mem0[a7:word32]",
                "6|L--|a7 = a7 + 4",
                "7|L--|a6 = Mem0[a7:word32]",
                "8|L--|a7 = a7 + 4");
        }

        [Test]
        public void M68krw_bra()
        {
            Given_UInt16s(0x6008);
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|goto 0001000A");
        }

        [Test]
        public void M68krw_lea_direct()
        {
            Given_UInt16s(0x49f9, 0x0000, 0x7ffe);
            AssertCode(
                "0|L--|00010000(6): 1 instructions",
                "1|L--|a4 = 00007FFE");
        }

        [Test]
        public void M68krw_lea_mem()
        {
            Given_UInt16s(0x43EB, 0xFFFE);
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|a1 = a3 + -2");
        }

        [Test]
        public void M68krw_bcc()
        {
            Given_UInt16s(0x6438, 0x6636);
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 0001003A",
                "2|T--|00010002(2): 1 instructions",
                "3|T--|if (Test(NE,Z)) branch 0001003A");
        }

        [Test]
        public void M68krw_bcc_invalid_address()
        {
            Given_UInt16s(0x6439);
            AssertCode(
                "0|---|00010000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void M68krw_addq_d()
        {
            Given_UInt16s(0x5401);
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v3 = SLICE(d1, byte, 0) + 0x02",
                "2|L--|v4 = SLICE(d1, word24, 8)",
                "3|L--|d1 = SEQ(v4, v3)",
                "4|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_subq_a()
        {
            Given_UInt16s(0x5549);
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|a1 = a1 - 2");
        }

        [Test]
        public void M68krw_moveq()
        {
            Given_UInt16s(0x72FF);
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|L--|d1 = -1",
                "2|L--|CVZN = cond(d1)");
        }

        [Test]
        public void M68krw_lea_pc()
        {
            Given_UInt16s(0x45FA, 0x0012);
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|a2 = 00010014");
        }

        [Test]
        public void M68krw_tst()
        {
            Given_UInt16s(0x4ABA, 0x0124);
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|ZN = cond(Mem0[0x00010126:word32] - 0x00000000)",
                "2|L--|C = false",
                "3|L--|V = false");
        }

        [Test]
        public void M68krw_pea()
        {
            Given_UInt16s(0x486A, 0x0004);
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|a7 = a7 - 0x00000004",
                "2|L--|Mem0[a7:word32] = a2 + 4");
        }

        [Test]
        public void M68krw_IndirectIndexed()
        {
            Given_UInt16s(0x4AB3, 0x0000);
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|ZN = cond(Mem0[a3 + (int32) ((int16) d0):word32] - 0x00000000)",
                "2|L--|C = false",
                "3|L--|V = false");
        }

        [Test]
        public void M68krw_Swap()
        {
            Given_UInt16s(0x4847);
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|d7 = __swap(d7)",
                "2|L--|ZN = cond(d7)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void M68krw_clr_d1()
        {
            Given_UInt16s(0x4241);
            AssertCode(
                "0|L--|00010000(2): 6 instructions",
                "1|L--|v4 = SLICE(d1, word16, 16)",
                "2|L--|d1 = SEQ(v4, 0x0000)",
                "3|L--|Z = true",
                "4|L--|C = false",
                "5|L--|N = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_ori()
        {
            Given_UInt16s(0x0038, 0x584F, 0x4000);
            AssertCode(
                "0|L--|00010000(6): 5 instructions",
                "1|L--|v2 = Mem0[0x00004000:byte] | 0x4F",
                "2|L--|Mem0[0x00004000:byte] = v2",
                "3|L--|ZN = cond(v2)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_addx()
        {
            Given_UInt16s(0xD38D);
            AssertCode(
                "0|L--|00010000(2): 6 instructions",
                "1|L--|a5 = a5 - 4",
                "2|L--|v5 = Mem0[a5:word32]",
                "3|L--|a1 = a1 - 4",
                "4|L--|v6 = v5 + Mem0[a1:word32] + X",
                "5|L--|Mem0[a1:word32] = v6",
                "6|L--|CVZNX = cond(v6)");
        }

        [Test]
        public void M68krw_movem_to_reg()
        {
            Given_UInt16s(0x4cef, 0x0003, 0x0030);
            AssertCode(
                "0|L--|00010000(6): 5 instructions",
                "1|L--|v3 = a7 + 48",
                "2|L--|d0 = Mem0[v3:word32]",
                "3|L--|v3 = v3 + 4",
                "4|L--|d1 = Mem0[v3:word32]");
        }

        [Test]
        public void M68krw_divu_w()
        {
            Given_UInt16s(0x80C1);
            AssertCode(
                "0|L--|00010000(2): 5 instructions",
                "1|L--|v3 = (uint16) (d0 % SLICE(d1, uint16, 0))",
                "2|L--|v4 = (uint16) (d0 /u SLICE(d1, uint16, 0))",
                "3|L--|d0 = SEQ(v3, v4)",
                "4|L--|VZN = cond(v4)",
                "5|L--|C = false");
        }

        [Test]
        public void M68krw_rol()
        {
            Given_UInt16s(0xE199);
            AssertCode(
               "0|L--|00010000(2): 3 instructions",
               "1|L--|d1 = __rol(d1, 0x00000008)",
               "2|L--|CZN = cond(d1)",
               "3|L--|V = false");
        }

        [Test]
        public void M68krw_roxl()
        {
            Given_UInt16s(0xE391);
            AssertCode(
               "0|L--|00010000(2): 3 instructions",
               "1|L--|d1 = __rcl(d1, 0x00000001, X)",
               "2|L--|CZNX = cond(d1)",
               "3|L--|V = false");
        }

        [Test]
        public void M68krw_st()
        {
            Given_UInt16s(0x50EF, 0x0002);
            AssertCode(
               "0|L--|00010000(4): 1 instructions",
               "1|L--|Mem0[a7 + 2:bool] = true");
        }

        [Test]
        public void M68krw_tst_mem()
        {
            Given_UInt16s(0x4AB9, 0x0000, 0x13F8);
            AssertCode(
                "0|L--|00010000(6): 3 instructions",
                "1|L--|ZN = cond(Mem0[0x000013F8:word32] - 0x00000000)",
                "2|L--|C = false",
                "3|L--|V = false");
        }

        [Test]
        public void M68krw_rorx()
        {
            Given_UInt16s(0xE014);
            AssertCode(
                "0|L--|00010000(2): 5 instructions",
                "1|L--|v4 = __rcr(SLICE(d4, byte, 0), 0x08, X)",
                "2|L--|v5 = SLICE(d4, word24, 8)",
                "3|L--|d4 = SEQ(v5, v4)",
                "4|L--|CZNX = cond(v4)",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_ror_ea()
        {
            Given_UInt16s(0xE6D4);
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v3 = __ror(Mem0[a4:word32], 0x01)",
                "2|L--|Mem0[a4:word32] = v3",
                "3|L--|CZN = cond(v3)");
        }

        [Test]
        public void M68krw_jsr_pc()
        {
            Given_UInt16s(0x4EBA, 0x0030);
            AssertCode(
                "0|T--|00010000(4): 1 instructions",
                "1|T--|call 00010032 (4)");
        }

        [Test]
        public void M68krw_clr_addr()
        {
            Given_UInt16s(0x42B9, 0x0000, 0x15E8);
            AssertCode(
                "0|L--|00010000(6): 5 instructions",
                "1|L--|Mem0[0x000015E8:word32] = 0x00000000",
                "2|L--|Z = true",
                "3|L--|C = false",
                "4|L--|N = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_bset_addr()
        {
            Given_UInt16s(0x08e8, 0x0001, 0x0010); //                 bset #1,%a0@(1)
            AssertCode(
                "0|L--|00010000(6): 1 instructions",
                "1|L--|Z = __bset(Mem0[a0 + 16:byte], 0x0001, out Mem0[a0 + 16:byte])");
            //.data:00000006 08 a8 00 00 00 10                bclr #0,%a0@(16)
        }

        [Test]
        public void M68krw_bset_effectivezero()
        {
            Given_UInt16s(0x01F0, 0x01C0);
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|Z = __bset(Mem0[null:byte], d0, out Mem0[null:byte])");
        }

        [Test]
        public void M68krw_bset_effective()
        {
            Given_UInt16s(0x08F1, 0x9708, 0xF1CC);
            AssertCode(
                "0|L--|00010000(6): 1 instructions",
                "1|L--|Z = __bset(Mem0[null:byte], 0x9708, out Mem0[null:byte])");
        }

        [Test]
        public void M68krw_bclr_addr()
        {
            Given_UInt16s(0x08A8, 0x0001, 0x0010); //                 bclr #1,%a0@(1)
            AssertCode(
                "0|L--|00010000(6): 1 instructions",
                "1|L--|Z = __bclr(Mem0[a0 + 16:byte], 0x01, out Mem0[a0 + 16:byte])");
            //.data:00000006 08 a8 00 00 00 10                bclr #0,%a0@(16)
        }

        [Test]
        public void M68krw_addi()
        {
            Given_UInt16s(0x0646, 0x000F);            // addiw #15,%d6
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|v3 = SLICE(d6, word16, 0) + 0x000F",
                "2|L--|v4 = SLICE(d6, word16, 16)",
                "3|L--|d6 = SEQ(v4, v3)",
                "4|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_eori()
        {
            Given_UInt16s(0x0A40, 0x000F);     //                    eoriw #15,%d0    
            AssertCode(
                "0|L--|00010000(4): 6 instructions",
                "1|L--|v3 = SLICE(d0, word16, 0) ^ 0x000F",
                "2|L--|v4 = SLICE(d0, word16, 16)",
                "3|L--|d0 = SEQ(v4, v3)",
                "4|L--|ZN = cond(v3)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_address_mode()
        {
            Given_UInt16s(0x2432, 0x04fc);    // move.l\t(-04,a2,d0*2),d2",
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|d2 = Mem0[a2 + -4 + (int32) ((int16) d0) * 4:word32]"
                );
        }

        [Test]
        public void M68krw_movem()
        {
            Given_UInt16s(0x4CB9, 0x0003, 0x0004, 0x000A); // , "movem.w\t$0004000A,d0-d1");
            AssertCode(
               "0|L--|00010000(8): 5 instructions",
               "1|L--|v2 = 0004000A",
               "2|L--|d0 = Mem0[v2:word16]",
               "3|L--|v2 = v2 + 2",
               "4|L--|d1 = Mem0[v2:word16]",
               "5|L--|v2 = v2 + 2");
        }

        [Test]
        public void M68krw_indexedOperand()
        {
            Given_UInt16s(0x2C70, 0xE9B5, 0x0001, 0x7FEC);
            AssertCode(
               "0|L--|00010000(8): 1 instructions",
               "1|L--|a6 = Mem0[Mem0[0x00017FEC:word32] + a6:word32]");
        }

        [Test]
        public void M68krw_PcRelative()
        {
            Given_UInt16s(0x2A7B, 0x0804);
            AssertCode(
               "0|L--|00010000(4): 1 instructions",
               "1|L--|a5 = Mem0[0x00010006 + d0:word32]");
        }

        [Test]
        public void M68krw_JmpIndirect()
        {
            Given_UInt16s(0x4ED5);
            AssertCode(
                 "0|T--|00010000(2): 1 instructions",
                 "1|T--|goto a5");
        }


        [Test]
        public void M68krw_JmpLong()
        {
            Given_UInt16s(0x4EF9, 0x0001, 0xE5B2);
            AssertCode(
                 "0|T--|00010000(6): 1 instructions",
                 "1|T--|goto 0001E5B2");
        }

        [Test]
        public void M68krw_dbne()
        {
            Given_UInt16s(0x56C8, 0xFFFA);
            AssertCode(
                "0|T--|00010000(4): 6 instructions",
                "1|T--|if (Test(NE,Z)) branch 00010004",
                "2|L--|v4 = SLICE(d0, word16, 0)",
                "3|L--|v4 = v4 - 1",
                "4|L--|v5 = SLICE(d0, word16, 16)",
                "5|L--|d0 = SEQ(v5, v4)",
                "6|T--|if (v4 != 0xFFFF) branch 0000FFFC");
        }

        [Test]
        public void M68krw_cmpm()
        {
            Given_UInt16s(0xB308);
            AssertCode(
                "0|L--|00010000(2): 6 instructions",
                "1|L--|v3 = Mem0[a0:byte]",
                "2|L--|a0 = a0 + 1",
                "3|L--|v5 = Mem0[a1:byte]",
                "4|L--|a1 = a1 + 1",
                "5|L--|v6 = v5 - v3",
                "6|L--|CVZN = cond(v6)");
        }

        [Test]
        public void M68krw_move_pc_indexed()
        {
            Given_UInt16s(0x303B, 0x0006);    // move.w (06, pc, d0), d0
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|v3 = Mem0[0x00010008 + (int32) ((int16) d0):word16]",
                "2|L--|v4 = SLICE(d0, word16, 16)",
                "3|L--|d0 = SEQ(v4, v3)",
                "4|L--|CVZN = cond(v3)");
        }

        [Test]
        public void M68krw_move_sr()
        {
            Given_UInt16s(0x40E7);        // move sr,-(a7)
            AssertCode(
                "0|S--|00010000(2): 3 instructions",
                "1|L--|a7 = a7 - 2",
                "2|L--|v4 = sr",
                "3|L--|Mem0[a7:word16] = v4");
        }

        [Test]
        public void M68krw_move_sr_2()
        {
            Given_UInt16s(0x46FC, 0x2700);        // move #$2700,sr
            AssertCode(
                "0|S--|00010000(4): 1 instructions",
                "1|L--|sr = 0x2700");
        }

        [Test]
        public void M68krw_divs()
        {
            Given_UInt16s(0x81C1);                // divs
            AssertCode(
                "0|L--|00010000(2): 5 instructions",
                "1|L--|v3 = (int16) (d0 % SLICE(d1, word16, 0))",
                "2|L--|v4 = (int16) (d0 / SLICE(d1, word16, 0))",
                "3|L--|d0 = SEQ(v3, v4)",
                "4|L--|VZN = cond(v4)",
                "5|L--|C = false");
        }

        [Test]
        public void M68krw_fmove_d_to_register()
        {
            Given_UInt16s(0xF22E, 0x5400, 0xFFF8); // fmove.d $-0008(a6),fp0
            AssertCode(
                "0|L--|00010000(6): 2 instructions",
                "1|L--|fp0 = (real96) Mem0[a6 + -8:real64]",
                "2|L--|FPUFLAGS = cond(fp0)");
        }

        [Test]
        public void M68krw_fmove_d_to_memory()
        {
            Given_UInt16s(0xF22E, 0x7400, 0xFFF8); // fmove.d\tfp0,$-0008(a6)
            AssertCode(
                "0|L--|00010000(6): 3 instructions",
                "1|L--|v4 = (real64) fp0",
                "2|L--|Mem0[a6 + -8:real64] = v4",
                "3|L--|FPUFLAGS = cond(v4)");
        }

        [Test]
        public void M68krw_fmul_d()
        {
            Given_UInt16s(0xF22E, 0x5423, 0x0008); // fmul.d $0008(a6),fp0
            AssertCode(
               "0|L--|00010000(6): 2 instructions",
               "1|L--|fp0 = fp0 * Mem0[a6 + 8:real64]",
               "2|L--|FPUFLAGS = cond(fp0)");
        }


        [Test]
        public void M68krw_fdivd()
        {
            Given_UInt16s(0xF23C, 0x5420, 0x4018, 0x0000, 0x0000, 0x0000); // fdiv.d\t#6.0,fp0
            AssertCode(
               "0|L--|00010000(12): 2 instructions",
               "1|L--|fp0 = fp0 / 6.0",
               "2|L--|FPUFLAGS = cond(fp0)");
        }

        [Test]
        public void M68krw_fmovecr()
        {
            Given_UInt16s(0xF200, 0x5CB2);    // fmove cr#$32,fp1
            AssertCode(
               "0|L--|00010000(4): 2 instructions",
               "1|L--|fp1 = 100.0",
               "2|L--|fpsr = cond(fp1)");
        }

        [Test]
        public void M68krw_fcmp()
        {
            Given_UInt16s(0xF22E, 0x5438, 0x0010);  // fcmpd a6(16),fp0 
            AssertCode(
               "0|L--|00010000(6): 1 instructions",
               "1|L--|FPUFLAGS = cond((real64) fp0 - Mem0[a6 + 16:real64])");
        }

        [Test]
        public void M68krw_fbnge()
        {
            Given_UInt16s(0xF29C, 0x00E0);  // fbnge 0x000000e8
            AssertCode(
               "0|T--|00010000(4): 1 instructions",
               "1|T--|if (Test(LT,FPUFLAGS)) branch 000100E2");
        }

        [Test]
        public void M68krw_fmovem()
        {
            Given_UInt16s(0xF227, 0xE004);  // fmovem.x fp2,-(a7)
            AssertCode(
               "0|L--|00010000(4): 2 instructions",
               "1|L--|a7 = a7 - 12",
               "2|L--|Mem0[a7:real96] = fp2");
        }

        [Test]
        public void M68krw_fmovem_to_reg()
        {
            Given_UInt16s(0xF22E, 0xD020, 0xFFE8); //  fmovemx %fp@(-24),%fp2
            AssertCode(
                "0|L--|00010000(6): 3 instructions",
                "1|L--|v3 = a6 + -24",
                "2|L--|fp2 = Mem0[v3:real96]",
                "3|L--|v3 = v3 + 12");
        }

        [Test]
        public void M68krw_chk16_dreg()
        {
            Given_UInt16s(0x4D82);         // chk
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|T--|if (SLICE(d2, word16, 0) >= 0x0000 && SLICE(d2, word16, 0) <= SLICE(d6, word16, 0)) branch 00010002",
                "2|L--|__syscall(0x06)");
        }

        [Test]
        public void M68krw_chk16_indirect()
        {
            Given_UInt16s(0x4D92);         // chk
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|T--|if (Mem0[a2:word16] >= 0x0000 && Mem0[a2:word16] <= SLICE(d6, word16, 0)) branch 00010002",
                "2|L--|__syscall(0x06)");
        }

        [Test]
        public void M68krw_chk16_postinc()
        {
            Given_UInt16s(0x4D9A);         // chk
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v3 = Mem0[a2:word16]",
                "2|L--|a2 = a2 + 2",
                "3|T--|if (v3 >= 0x0000 && v3 <= SLICE(d6, word16, 0)) branch 00010002",
                "4|L--|__syscall(0x06)");
        }

        [Test]
        public void M68krw_chk32_dreg()
        {
            Given_UInt16s(0x4D02);         // chk
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|T--|if (d2 >= 0x00000000 && d2 <= d6) branch 00010002",
                "2|L--|__syscall(0x06)");
        }

        [Test]
        public void M68krw_movep()
        {
            Given_UInt16s(0x0949, 0x0010);        // movep.w $10(a1), d4
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|__movep_l(Mem0[a1 + 16:word32], d4)");
        }

        [Test]
        public void M68krw_cmp2()
        {
            Given_UInt16s(0x04D1, 0xA000);        // cmp2 (a1),a2
            AssertCode(
              "0|L--|00010000(4): 2 instructions",
              "1|L--|C = a2 < Mem0[a1:word32] || a2 > Mem0[a1 + 0x00000004:word32]",
              "2|L--|Z = a2 == Mem0[a1:word32] || a2 == Mem0[a1 + 0x00000004:word32]");
        }

        [Test]
        public void M68krw_cas()
        {
            //$TODO: add "stdatomic.h"  to output file.
            Given_UInt16s(0x0ED3, 0x0102);        // cas.w d2,d1,(a3)
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|CVZN = atomic_compare_exchange_weak(&Mem0[a3:word16], SLICE(d1, word16, 0), SLICE(d2, word16, 0))");
        }

        [Test]
        public void M68krw_trap()
        {
            Given_UInt16s(0x4E4E);
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|L--|__syscall(0x0E)");
        }

        [Test]
        public void M68krw_move_to_ccr()
        {
            Given_UInt16s(0x44c3);
            AssertCode(     // move\td3,ccr
                "0|L--|00010000(2): 1 instructions",
                "1|L--|ccr = SLICE(d3, word16, 0)");
        }

        [Test]
        public void M68krw_move_fr_ccr()
        {
            Given_UInt16s(0x42d3);
            AssertCode( // move\tccr,(a3)",
                "0|L--|00010000(2): 2 instructions",
                "1|L--|v4 = (uint16) ccr",
                "2|L--|Mem0[a3:uint16] = v4");
        }

        [Test]
        public void M68krw_move_pc_index()
        {
            Given_UInt16s(0x4BFB, 0x0170, 0x0000, 0x3D60);    //  lea (00003D60, pc),a5
            AssertCode(
                "0|L--|00010000(8): 1 instructions",
                "1|L--|a5 = 00013D60");
        }

        [Test]
        public void M68krw_divsl()
        {
            Given_UInt16s(0x4C40, 0x3801);        // divsl d0,d1,d3
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|d1 = d3 % d0",
                "2|L--|d3 = d3 / d0",
                "3|L--|VZN = cond(d3)",
                "4|L--|C = false");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void M68krw_clrw_regression()
        {
            Given_UInt16s(0x4270, 0xA9A0, 0x0C97);    //  clr.w (+0C97, a2)
            AssertCode(
                "0|L--|00010000(6): 5 instructions",
                "1|L--|Mem0[a2 + 3223:word16] = 0x0000",
                "2|L--|Z = true",
                "3|L--|C = false",
                "4|L--|N = false",
                "5|L--|V = false");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void M68krw_moves()
        {
            Given_UInt16s(0x0EA0, 0x0048);    // moves.w -(a0),d0
            AssertCode(
                "0|S--|00010000(4): 4 instructions",
                "1|L--|a0 = a0 - 2",
                "2|L--|v4 = __moves(Mem0[a0:word16])",
                "3|L--|v5 = SLICE(d0, word16, 16)",
                "4|L--|d0 = SEQ(v5, v4)");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void M68krw_negx()
        {
            Given_UInt16s(0x4036, 0x600A); // negx.b (0A, a6, d6)
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|v4 = -Mem0[a6 + 10 + d6:byte] - X",
                "2|L--|Mem0[a6 + 10 + d6:byte] = v4",
                "3|L--|CVZNX = cond(v4)");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void M68krw_rte()
        {
            Given_UInt16s(0x4E73);    // rte
            AssertCode(
                "0|S--|00010000(2): 3 instructions",
                "1|L--|sr = Mem0[a7:word16]",
                "2|L--|a7 = a7 + 2",
                "3|T--|return (4,0)");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void M68krw_bkpt()
        {
            Given_UInt16s(0x484B);    // bkpt#$03
            AssertCode(
               "0|---|00010000(2): 1 instructions",
               "1|L--|__bkpt(0x03)");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void M68krw_move16()
        {
            Given_UInt16s(0xF605, 0x6600, 0xFFF4);    // move16 (a5)+,#$6600FFF4
            AssertCode(
               "0|L--|00010000(6): 2 instructions",
               "1|L--|v3 = Mem0[a5:word128]",
               "2|L--|a5 = a5 + 16");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void M68krw_sbcd()
        {
            Given_UInt16s(0x8F02);    // sbcd d2,d7
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v5 = SLICE(d2, byte, 0) - SLICE(d7, byte, 0) - X",
                "2|L--|v6 = SLICE(d7, word24, 8)",
                "3|L--|d7 = SEQ(v6, v5)",
                "4|L--|CVZNX = cond(v5)");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void M68krw_fbolt()
        {
            Given_UInt16s(0xF684, 0x0678);    // fbolt$00001CCE
            AssertCode(
                "0|T--|00010000(4): 1 instructions",
                "1|T--|if (Test(LT,FPUFLAGS)) branch 0001067A");
        }

        [Test]
        public void M68krw_dbcs()
        {
            Given_UInt16s(0x55CF, 0xFFF2);        // dbcs d7,$000F21B2
            AssertCode(
                "0|T--|00010000(4): 6 instructions",
                "1|T--|if (Test(ULT,C)) branch 00010004",
                "2|L--|v4 = SLICE(d7, word16, 0)",
                "3|L--|v4 = v4 - 1",
                "4|L--|v5 = SLICE(d7, word16, 16)",
                "5|L--|d7 = SEQ(v5, v4)",
                "6|T--|if (v4 != 0xFFFF) branch 0000FFF4");
        }

        [Test]
        public void M68krw_movem_w()
        {
            Given_UInt16s(0x48A7, 0xC000);            // movem.w d0-d1,-(sp)
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|a7 = a7 - 2",
                "2|L--|Mem0[a7:word16] = d1",
                "3|L--|a7 = a7 - 2",
                "4|L--|Mem0[a7:word16] = d0");
        }

        [Test]
        public void M68krw_nbcd()
        {
            Given_UInt16s(0x4822);    // nbcd -(a2)
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|a2 = a2 - 1",
                "2|L--|v4 = 0x00 - Mem0[a2:byte] - X",
                "3|L--|Mem0[a2:byte] = v4",
                "4|L--|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_pack()
        {
            Given_UInt16s(0x8F47, 0x0002);   // pack d7, d7, 2
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|v3 = __pack(SLICE(d7, uint16, 0), 0x0002)",
                "2|L--|v4 = SLICE(d7, word24, 8)",
                "3|L--|d7 = SEQ(v4, v3)");
        }

        [Test]
        public void M68krw_unpk()
        {
            Given_UInt16s(0x8784, 0x0784);    // unpk d4, d3
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|v4 = __unpk(SLICE(d4, byte, 0), 0x0784)",
                "2|L--|v5 = SLICE(d3, word16, 16)",
                "3|L--|d3 = SEQ(v5, v4)");
        }

        [Test]
        public void M68krw_svc()
        {
            Given_UInt16s(0x58EE, 0x26FC);    // svc $26FC(a6)
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|Mem0[a6 + 9980:byte] = V");
        }

        [Test]
        public void M68krw_abcd()
        {
            Given_UInt16s(0xC700);        // abcd d0, d3
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v5 = SLICE(d3, byte, 0) + SLICE(d0, byte, 0) + X",
                "2|L--|v6 = SLICE(d3, word24, 8)",
                "3|L--|d3 = SEQ(v6, v5)",
                "4|L--|CVZNX = cond(v5)");
        }

        [Test]
        public void M68krw_pc_relative_indexing()
        {
            Given_UInt16s(0x0C3B, 0x0004, 0x0028);    // cmpi.b\t#$04,($2C,pc,d0.w)
            AssertCode(
                "0|L--|00010000(6): 2 instructions",
                "1|L--|v3 = Mem0[0x0001002C + (int32) ((int16) d0):byte] - 4",
                "2|L--|CVZN = cond(v3)");
        }

        [Test]
        [Ignore("Need an OperandRewriter mode where the [a6] is returned to the caller.")]
        public void M68krw_tas()
        {
            Given_UInt16s(0x4AE6);    // tas -(a6)
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|a6 = a6 - 1",
                "2|L--|ZN = __tas(Mem0[a6:byte])");
        }

        [Test]
        public void M68krw_divul()
        {
            Given_UInt16s(0x4C44, 0x00A0); // divul.l d4, d0
            AssertCode(
                "0|L--|00010000(4): 5 instructions",
                "1|L--|v4 = d4",
                "2|L--|d4 = d0 % v4",
                "3|L--|d0 = d0 /u v4",
                "4|L--|VZN = cond(d0)",
                "5|L--|C = false");
        }

        [Test]
        public void M68krw_trapeq()
        {
            Given_UInt16s(0x57FA, 0x0029); // trapeq #$0029
            AssertCode(
                "0|T--|00010000(4): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 00010004",
                "2|L--|__syscall(0x0007, 0x0029)");
        }

        [Test]
        public void M68krw_indexedindirect()
        {
            Given_UInt16s(0x4033, 0xB316, 0x008B); // negx.b([a3], a3.w * 2, +008B)
            AssertCode(
                "0|L--|00010000(6): 3 instructions",
                "1|L--|v3 = -Mem0[Mem0[a3:word32] + (word32) ((int16) a3) * 2 + 139:byte] - X",
                "2|L--|Mem0[Mem0[a3:word32] + (word32) ((int16) a3) * 2 + 139:byte] = v3",
                "3|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_lea_indexedindirect()
        {
            Given_UInt16s(0x41F5, 0xB316, 0x0080);    // lea([a5],a3.w * 2,+0080),a0
            AssertCode(
                "0|L--|00010000(6): 1 instructions",
                "1|L--|a0 = Mem0[a5:word32] + (word32) SLICE(a3, int16, 0) * 2 + 128");
        }

        [Test]
        public void M68krw_chk_zeroextension()
        {
            Given_UInt16s(0x4736, 0x05C0);    // chk
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|T--|if (null >= 0x00000000 && null <= d3) branch 00010004",
                "2|L--|__syscall(0x06)");
        }

        [Test]
        public void M68krw_ptest()
        {
            Given_UInt16s(0xF000, 0x8000);    // ptest
            AssertCode(
                "0|S--|00010000(4): 1 instructions",
                "1|L--|__ptest(d0, 0x00)");
        }

        [Test]
        public void M68krw_trapf()
        {
            Given_UInt16s(0x51FC);    // trapf
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void M68krw_cmpi_b()
        {
            Given_UInt16s(0x0C03, 0x0016);    // cmpi.b
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|v3 = SLICE(d3, byte, 0) - 0x16",
                "2|L--|CVZN = cond(v3)");
        }

        [Test]
        public void M68krw_pc_relative()
        {
            Given_UInt16s(0x207B, 0x0170, 0x0000, 0x025C);
            AssertCode(
                "0|L--|00010000(8): 1 instructions",
                "1|L--|a0 = Mem0[0x0001025E:word32]");
        }

        [Test]
        public void M68krw_dblt()
        {
            Given_UInt16s(0x5DCA, 0x4EF9);    // dblt d2,$0016B6AB
            AssertCode(
                "0|T--|00010000(4): 6 instructions",
                "1|T--|if (Test(LT,VN)) branch 00010004",
                "2|L--|v4 = SLICE(d2, word16, 0)",
                "3|L--|v4 = v4 - 1",
                "4|L--|v5 = SLICE(d2, word16, 16)",
                "5|L--|d2 = SEQ(v5, v4)",
                "6|T--|if (v4 != 0xFFFF) branch 00014EFB");
        }

        [Test]
        public void M68krw_rtd()
        {
            Given_UInt16s(0x4E74, 0x0006);    // rtd #$0006
            AssertCode(
                "0|T--|00010000(4): 1 instructions",
                "1|T--|return (4,6)");
        }

        [Test]
        public void M68krw_trapmi()
        {
            Given_UInt16s(0x5BFC);    // trapmi
            AssertCode(
                "0|T--|00010000(2): 2 instructions",
                "1|T--|if (Test(GE,N)) branch 00010002",
                "2|L--|__syscall(0x0007)");
        }

        [Test]
        public void M68krw_dbeq()
        {
            Given_UInt16s(0x57C9, 0xFFFC);    //  dbeq d1,$001062C4
            AssertCode(
                "0|T--|00010000(4): 6 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00010004",
                "2|L--|v4 = SLICE(d1, word16, 0)",
                "3|L--|v4 = v4 - 1",
                "4|L--|v5 = SLICE(d1, word16, 16)",
                "5|L--|d1 = SEQ(v5, v4)",
                "6|T--|if (v4 != 0xFFFF) branch 0000FFFE");
        }

        [Test]
        public void M68krw_AddressRegisterIndirect()
        {
            Given_UInt16s(0xD831, 0x6000); // add.b\t(a1,d6.w)",
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|v5 = SLICE(d4, byte, 0) + Mem0[a1 + (int32) ((int16) d6):byte]",
                "2|L--|v6 = SLICE(d4, word24, 8)",
                "3|L--|d4 = SEQ(v6, v5)",
                "4|L--|CVZNX = cond(v5)");
        }
    }
}
