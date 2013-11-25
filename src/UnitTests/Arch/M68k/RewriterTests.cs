#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Arch.M68k;
using Decompiler.Core;
using Decompiler.Core.Rtl;
using Decompiler.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.M68k
{
    [TestFixture]
    public class RewriterTests
    {
        private IEnumerable<RtlInstructionCluster> rw;

        private void Rewrite(params ushort[] opcodes)
        {
            byte[] bytes = new byte[opcodes.Length * 2];
            var writer = new BeImageWriter(bytes);
            foreach (ushort opcode in opcodes)
            {
                writer.WriteBeUint16(opcode);
            }
            var image = new LoadedImage(new Address(0x00010000), bytes);

            var arch = new M68kArchitecture();
            rw = arch.CreateRewriter(image.CreateReader(0), arch.CreateProcessorState(), arch.CreateFrame(), new RewriterHost());
        }

        private void AssertCode(params string[] expected)
        {
            var e = rw.GetEnumerator();
            int i = 0;
            while (i < expected.Length && e.MoveNext())
            {
                var ee = e.Current.Instructions.GetEnumerator();
                while (i < expected.Length && ee.MoveNext())
                {
                    Assert.AreEqual(expected[i], string.Format("{0}|{1}", i, ee.Current));
                    ++i;
                }
            }
            Assert.AreEqual(expected.Length, i, "Expected " + expected.Length + " instructions.");
            Assert.IsFalse(e.MoveNext(), "More instructions were emitted than were expected.");
        }

        private class RewriterHost : IRewriterHost
        {
            public PseudoProcedure EnsurePseudoProcedure(string name, Decompiler.Core.Types.DataType returnType, int arity)
            {
                throw new NotImplementedException();
            }

            public PseudoProcedure GetImportThunkAtAddress(uint addrThunk)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void M68krw_Movea_l()
        {
            Rewrite(0x2261);        // movea.l   (a1)-,a1
            AssertCode(
                "0|a1 = a1 - 0x00000004",
                "1|a1 = Mem0[a1:word32]");
        }

        [Test]
        public void M68krw_Eor_b()
        {
            Rewrite(0xB103);        // eorb %d0,%d3
            AssertCode(
                "0|v4 = (byte) d3 ^ (byte) d0",
                "1|d3 = DPB(d3, v4, 0, 8)",
                "2|ZN = cond(v4)",
                "3|C = false",
                "4|V = false");
        }        

        [Test]
        public void M68krw_Eor_l()
        {
            Rewrite(0xB183);        // eorl %d0,%d3
            AssertCode(
                "0|d3 = d3 ^ d0",
                "1|ZN = cond(d3)",
                "2|C = false",
                "3|V = false");
        }

        [Test]
        public void M68krw_adda_postinc() // addal (a4)+,%a5
        {
            Rewrite(0xDBDC);
            AssertCode(
                "0|v3 = Mem0[a4:word32]",
                "1|a4 = a4 + 0x00000004",
                "2|a5 = a5 + v3");
        }

        [Test]
        public void M68krw_or_imm()
        {
            Rewrite(0x867c, 0x1123);    // or.w #$1123,d3
            AssertCode(
                "0|v3 = (word16) d3 | 0x1123",
                "1|d3 = DPB(d3, v3, 0, 16)",
                "2|ZN = cond(v3)",
                "3|C = false",
                "4|V = false");
        }

        [Test]
        public void M68krw_movew_indirect()
        {
            Rewrite(0x3410);    // move.w (A0),D2
            AssertCode(
                "0|v4 = Mem0[a0:word16]",
                "1|d2 = DPB(d2, v4, 0, 16)",
                "2|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_move_pre_and_postdec()
        {
            Rewrite(0x36E3);    // move.w -(a3),(a3)+
            AssertCode(
                "0|a3 = a3 - 0x00000002",
                "1|v3 = Mem0[a3:word16]",
                "2|Mem0[a3:word16] = v3",
                "3|a3 = a3 + 0x00000002",
                "4|CVZN = cond(v3)");
        }

        [Test]
        public void M68krw_muls_w()
        {
            Rewrite(0xC1E3); // muls.w -(a3),r3
            AssertCode(
                "0|a3 = a3 - 0x00000002",
                "1|d0 = d0 *s Mem0[a3:word16]",
                "2|VZN = cond(d0)",
                "3|C = false");
        }

        [Test]
        public void M68krw_mulu_l()
        {
            Rewrite(0x4c00, 0x7406); // mulu.l d0,d6,d7
            AssertCode(
                "0|d6_d7 = d7 *u d0",
                "1|VZN = cond(d6_d7)",
                "2|C = false");
        }

        [Test]
        public void M68krw_not_w()
        {
            Rewrite(0x4643); // not.w d3
            AssertCode(
                "0|v3 = ~(word16) d3",
                "1|d3 = DPB(d3, v3, 0, 16)",
                "2|ZN = cond(v3)",
                "3|C = false",
                "4|V = false");
        }

        public void M68krw_not_l_reg()
        {
            Rewrite(0x4684);    // not.l d4
            AssertCode(
                "0|d4 = ~d4",
                "2|ZN = cond(d4)",
                "3|C = false",
                "4|V = false");
        }

        [Test]
        public void M68krw_not_l_pre()
        {
            Rewrite(0x46A4);    // not.l -(a4)
            AssertCode(
                "0|a4 = a4 - 0x00000004",
                "1|v3 = ~Mem0[a4:word32]",
                "2|Mem0[a4:word32] = v3",
                "3|ZN = cond(v3)",
                "4|C = false",
                "5|V = false");
        }

        [Test]
        public void M68krw_and_re()
        {
            Rewrite(0xC363);    // and.w d1,-(a3)
            AssertCode(
                "0|a3 = a3 - 0x00000002",
                "1|v4 = Mem0[a3:word16] & (word16) d1",
                "2|Mem0[a3:word16] = v4",
                "3|ZN = cond(v4)",
                "4|C = false",
                "5|V = false");
        }

        [Test]
        public void M68krw_andi_32()
        {
            Rewrite(0x029C, 0x0001, 0x0000);    // and.l #00010000,(a4)+
            AssertCode(
                "0|v3 = Mem0[a4:word32] & 0x00010000",
                "1|Mem0[a4:word32] = v3",
                "2|a4 = a4 + 0x00000004",
                "3|ZN = cond(v3)",
                "4|C = false",
                "5|V = false");
        }

        [Test]
        public void M68krw_andi_8()
        {
            Rewrite(0x0202, 0x00F0);     // and.l #F0,d2"
            AssertCode(
                "0|v3 = (byte) d2 & 0xF0",
                "1|d2 = DPB(d2, v3, 0, 8)",
                "2|ZN = cond(v3)",
                "3|C = false",
                "4|V = false");
        }

        [Test]
        public void M68krw_asrb_qb()
        {
            Rewrite(0xEE00);        // asr.b\t#7,d0
            AssertCode(
                "0|v3 = (byte) d0 >> 0x07",
                "1|d0 = DPB(d0, v3, 0, 8)",
                "2|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_neg_w_post()
        {
            Rewrite( 0x445B);
            AssertCode(
                "0|v3 = -Mem0[a3:word16]",
                "1|Mem0[a3:word16] = v3",
                "2|a3 = a3 + 0x00000002",
                "3|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_neg_w_mem()
        {
            Rewrite(0x4453);
            AssertCode(
                "0|v3 = -Mem0[a3:word16]",
                "1|Mem0[a3:word16] = v3",
                "2|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_negx_8()
        {
            Rewrite(0x4021);        // negx.b -(a1)
 
            AssertCode(
                "0|a1 = a1 - 0x00000001",
                "1|v3 = -Mem0[a1:byte] - X",
                "2|Mem0[a1:byte] = v3",
                "3|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_sub_er_16()
        {
            Rewrite(0x9064);        // sub.w -(a4),d0
            AssertCode(
                "0|a4 = a4 - 0x00000002",
                "1|v4 = (word16) d0 - Mem0[a4:word16]",
                "2|d0 = DPB(d0, v4, 0, 16)",
                "3|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_suba_16()
        {
            Rewrite(0x90DC);      // suba.w (a4)+,a0
            AssertCode(
                "0|v3 = Mem0[a4:word16]",
                "1|a4 = a4 + 0x00000002",
                "2|v5 = (word16) a0 - v3",
                "3|a0 = DPB(a0, v5, 0, 16)");
        }

        [Test]
        public void M68krw_clrw_ea_off()
        {
            Rewrite(0x4268, 0xFFF8);    // clr.w\t$0008(a0)
            AssertCode(
                "0|Mem0[a0 + -8:word16] = 0x0000",
                "1|Z = true",
                "2|C = false",
                "3|N = false",
                "4|V = false");
        }

        [Test]
        public void M68krw_cmpib_d()
        {
            Rewrite(0x0C18, 0x0042);    // cmpi.b #$42,(a0)+
            AssertCode(
                "0|v3 = Mem0[a0:byte]",
                "1|a0 = a0 + 0x00000001",
                "2|v4 = v3 - 0x42",
                "3|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpw_d()
        {
            Rewrite(0xB041);        // cmp.w d1,d0
            AssertCode(
                "0|v4 = (word16) d0 - (word16) d1",
                "1|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpw_pre_pre()
        {
            Rewrite(0xB066);        // cmp.w -(a6),d0
            AssertCode(
                "0|a6 = a6 - 0x00000002",
                "1|v4 = (word16) d0 - Mem0[a6:word16]",
                "2|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpaw()
        {
            Rewrite(0xB0EC, 0x0022);    // cmpa.w $22(a4),a0
            AssertCode(
                "0|v4 = (word16) a0 - Mem0[a4 + 34:word16]",
                "1|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpal()
        {
            Rewrite(0xB1EC, 0x0010);    // cmpa.l $10(a4),a0
            AssertCode(
                "0|v4 = a0 - Mem0[a4 + 16:word32]",
                "1|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_jsr_mem()
        {
            Rewrite(0x4E90);    // jsr (a0)
            AssertCode(
                "0|call Mem0[a0:word32] (4)");
        }

        [Test]
        public void M68krw_jsr()
        {
            Rewrite(
                0x4EB9, 0x0018, 0x5050, // jsr $00185050
                0x4EB8, 0xFFFA);        // jsr $FFFFFFFA
            AssertCode(
                "0|call 00185050 (4)",
                "1|call 0000FFFA (4)");
        }

        [Test]
        public void M68krw_or_rev()
        {
            Rewrite(0x81A8, 0xFFF8);
            AssertCode(
                "0|v4 = Mem0[a0 + -8:word32] | d0",
                "1|Mem0[a0 + -8:word32] = v4",
                "2|ZN = cond(v4)");
        }

        [Test]
        public void M68krw_lsl_w()
        {
            Rewrite(0xE148);    // lsl.w #$01,d0"
            AssertCode(
                "0|v3 = (word16) d0 << 0x0008",
                "1|d0 = DPB(d0, v3, 0, 16)",
                "2|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_subiw()
        {
            Rewrite(0x0440, 0x0140);    // subiw #320,%d0
            AssertCode(
                "0|v3 = (word16) d0 - 0x0140",
                "1|d0 = DPB(d0, v3, 0, 16)",
                "2|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_sub_re()
        {
            Rewrite(0x919F);    // sub.l\td0,(a7)+
            AssertCode(
                "0|v4 = Mem0[a7:word32] - d0",
                "1|Mem0[a7:word32] = v4",
                "2|a7 = a7 + 0x00000004",
                "3|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_subq_w()
        {
            Rewrite(0x5F66);    // subq.w\t#$07,-(a6)
            AssertCode(
                "0|a6 = a6 - 0x00000002",
                "1|v3 = Mem0[a6:word16] - 0x0007",
                "2|Mem0[a6:word16] = v3",
                "3|CVZNX = cond(v3)");
            Rewrite(0x5370, 0x1034);    // subq.w\t#$01,(34,a0,d1)
            AssertCode(
                "0|v4 = Mem0[a0 + 52 + d1:word16] - 0x0001",
                "1|Mem0[a0 + 52 + d1:word16] = v4",
                "2|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_rts()
        {
            Rewrite(0x4E75);    // rts
            AssertCode(
                "0|return (4,0)");
        }

        [Test]
        public void M68krw_asr_ea()
        {
            Rewrite(0xE0E5);    // asr.w\t-(a5)
            AssertCode(
                "0|a5 = a5 - 0x00000002",
                "1|v3 = Mem0[a5:word16] >> 1",
                "2|Mem0[a5:word16] = v3",
                "3|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_subx_mm()
        {
            Rewrite(0x9149);   // subx.w\t-(a1),-(a0)
            AssertCode(
                "0|a1 = a1 - 0x00000002",
                "1|v4 = Mem0[a1:word16]",
                "2|a0 = a0 - 0x00000002",
                "3|v5 = Mem0[a0:word16] - v4 - X",
                "4|Mem0[a0:word16] = v5",
                "5|CVZNX = cond(v5)");
        }

        [Test]
        public void M68krw_lsl_ea()
        {
            Rewrite(0xE3D1);    // lsl.w\t(a1)
            AssertCode(
                "0|v3 = Mem0[a1:word16] << 1",
                "1|Mem0[a1:word16] = v3",
                "2|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_lsl_r()
        {
            Rewrite(0xE36C);    // lsl.w\td1,d4
            AssertCode(
                "0|v4 = (word16) d4 << (word16) d1",
                "1|d4 = DPB(d4, v4, 0, 16)",
                "2|CVZNX = cond(v4)");
        }

    }
}
