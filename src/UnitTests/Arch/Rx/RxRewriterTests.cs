#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Arch.Renesas;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Rx
{
    [TestFixture]
    public class RxRewriterTests : RewriterTestBase
    {
        public RxRewriterTests()
        {
            var options = new Dictionary<string, object>()
            {
                { ProcessorOption.Endianness, "big" }
            };
            this.Architecture = new RxArchitecture(CreateServiceContainer(), "rxv2", options);
            this.LoadAddress = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture { get; }
        public override Address LoadAddress { get; }


        [Test]
        public void RxRw_abs_dest()
        {
            Given_HexString("7E24");
            AssertCode(        // abs\tr4
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r4 = abs<word32>(r4)",
                "2|L--|OSZ = cond(r4)");
        }

        [Test]
        public void RxRw_abs_src_dest()
        {
            Given_HexString("FC0F34");
            AssertCode(        // abs\tr3,r4
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r4 = abs<word32>(r3)",
                "2|L--|OSZ = cond(r4)");
        }

        [Test]
        public void RxRw_adc_simm8_dest()
        {
            Given_HexString("FD7424 FF");
            AssertCode(        // adc\t#0FFFFFFFFh,r4
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r4 = r4 + 0xFFFFFFFF<32> + C",
                "2|L--|COSZ = cond(r4)");
        }

        [Test]
        public void RxRw_adc_simm24_dest()
        {
            Given_HexString("FD7C24 A6789A");
            AssertCode(        // adc\t#0FFA6789Ah,r4
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r4 = r4 + 0xFFA6789A<32> + C",
                "2|L--|COSZ = cond(r4)");
        }

        [Test]
        public void RxRw_adc_src_dest()
        {
            Given_HexString("FC0B30");
            AssertCode(        // adc\tr3,sp
                "0|L--|00100000(3): 2 instructions",
                "1|L--|sp = sp + r3 + C",
                "2|L--|COSZ = cond(sp)");
        }

        [Test]
        public void RxRw_adc_src_dest_mem()
        {
            Given_HexString("06A10230 7F");
            AssertCode(        // adc.l\t508[r3],sp
                "0|L--|00100000(5): 2 instructions",
                "1|L--|sp = sp + Mem0[r3 + 508<i32>:word32] + C",
                "2|L--|COSZ = cond(sp)");
            Given_HexString("06A20230 8764");
            AssertCode(        // adc.l\t-30876[r3],sp
                "0|L--|00100000(6): 2 instructions",
                "1|L--|sp = sp + Mem0[r3 + -30876<i32>:word32] + C",
                "2|L--|COSZ = cond(sp)");
        }

        [Test]
        public void RxRw_add_imm()
        {
            Given_HexString("62F3");
            AssertCode(        // add\t#0Fh,r3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r3 = r3 + 0xF<32>",
                "2|L--|COSZ = cond(r3)");
        }

        [Test]
        public void RxRw_add_ub_src()
        {
            Given_HexString("4923FF");
            AssertCode(        // add.ub\t-1[r2],r3
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r3 = r3 + Mem0[r2 + -1<i32>:byte]",
                "2|L--|COSZ = cond(r3)");
        }

        [Test]
        public void RxRw_add_notub_src()
        {
            Given_HexString("064923FF");
            AssertCode(        // add.w\t-2[r2],r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r3 + Mem0[r2 + -2<i32>:int16]",
                "2|L--|COSZ = cond(r3)");
        }

        [Test]
        public void RxRw_add_simm()
        {
            Given_HexString("72238000");
            AssertCode(        // add\t#0FFFF8000h,r2,r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r2 + 0xFFFF8000<32>",
                "2|L--|COSZ = cond(r3)");
        }

        [Test]
        public void RxRw_add3()
        {
            Given_HexString("FF2234");
            AssertCode(        // add\tr2,r3,r4
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r4 = r3 + r2",
                "2|L--|COSZ = cond(r4)");
        }

        [Test]
        public void RxRw_and_imm3_reg()
        {
            Given_HexString("6434");
            AssertCode(        // and\t#3h,r4
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r4 = r4 & 3<32>",
                "2|L--|SZ = cond(r4)");
        }

        [Test]
        public void RxRw_and_3_reg()
        {
            Given_HexString("FF4231");
            AssertCode(        // and\tr2,r3,r1
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r1 = r3 & r2",
                "2|L--|SZ = cond(r1)");
        }

        [Test]
        public void RxRw_bclr_imm3()
        {
            Given_HexString("F03F");
            AssertCode(        // bclr.ub\t#7h,[r3]
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v4 = __clear_bit<byte,byte>(Mem0[r3:byte], 7<8>)",
                "2|L--|Mem0[r3:byte] = v4");
        }

        [Test]
        public void RxRw_beq_disp16()
        {
            Given_HexString("3AFFFF");
            AssertCode(        // beq.w\t000FFFFF
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 000FFFFF");
        }

        [Test]
        public void RxRw_bmcnd_3()
        {
            Given_HexString("FCFD31 FF");
            AssertCode(        // bmne.b\t#7h,-1[r3]
                 "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = __set_bit(Test(NE,Z), 7<8>)",
                "2|L--|Mem0[r3 + -1<i32>:int8] = v4");
            Given_HexString("FCFE31 8000");
            AssertCode(        // bmne.b\t#7h,-32768[r3]
                "0|L--|00100000(5): 2 instructions",
                "1|L--|v4 = __set_bit(Test(NE,Z), 7<8>)",
                "2|L--|Mem0[r3 + -32768<i32>:int8] = v4");
        }

        [Test]
        public void RxRw_bnot_imm3()
        {
            Given_HexString("FCF04F");
            AssertCode(        // bnot.b\t#4h,[r4]
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v4 = __invert_bit<int8,byte>(Mem0[r4:int8], 4<8>)",
                "2|L--|Mem0[r4:int8] = v4");
        }

        [Test]
        public void RxRw_bnot_imm5_reg()
        {
            Given_HexString("FDFFF3");
            AssertCode(        // bnot\t#1Fh,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = __invert_bit<word32,word32>(r3, 0x1F<32>)");
        }

        [Test]
        public void RxRw_bra_s()
        {
            Given_HexString("08");
            AssertCode(        // bra.s\t00100008
                "0|T--|00100000(1): 1 instructions",
                "1|T--|goto 00100008");
        }

        [Test]
        public void RxRw_bra_l()
        {
            Given_HexString("7F44");
            AssertCode(        // bra.l\tr4
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto r4");
        }

        [Test]
        public void RxRw_bsr_w()
        {
            Given_HexString("398000");
            AssertCode(        // bsr.w\t000F8000
                "0|T--|00100000(3): 1 instructions",
                "1|T--|call 000F8000 (4)");
        }

        [Test]
        public void RxRw_btst_imm3()
        {
            Given_HexString("F487");
            AssertCode(        // btst\t#7h,r7
                "0|L--|00100000(2): 3 instructions",
                "1|L--|r7 = __bit<word32,byte>(r7, 7<8>)",
                "2|L--|C = r7 ? 1<32> : 0<32>",
                "3|L--|Z = r7 ? 0<32> : 2<32>");
        }

        [Test]
        public void RxRw_clrpsw()
        {
            Given_HexString("7FB9");
            AssertCode(        // clrpsw\tu
                "0|L--|00100000(2): 1 instructions",
                "1|L--|U = 0x20000<32>");
        }

        [Test]
        public void RxRw_cmp_imm8_reg()
        {
            Given_HexString("7553 FF");
            AssertCode(        // cmp\t#0FFh,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|COSZ = cond(r3 - 0xFF<8>)");
        }

        [Test]
        public void RxRw_div_simm()
        {
            Given_HexString("FD7C84 123456");
            AssertCode(        // div\t#123456h,r4
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r4 = r4 / 0x123456<32>",
                "2|L--|O = cond(r4)");
        }

        [Test]
        public void RxRw_divu_uimm()
        {
            Given_HexString("FD7C94 123456");
            AssertCode(        // divu\t#123456h,r4
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r4 = r4 /u 0x123456<32>",
                "2|L--|O = cond(r4)");
        }

        [Test]
        public void RxRw_emaca()
        {
            Given_HexString("FD07C2");
            AssertCode(        // emaca\tr12,r2,a0
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v5 = CONVERT(r12 *s64 r2, int64, int72)",
                "2|L--|a0 = a0 + v5");
            Given_HexString("FD0FC2");
            AssertCode(        // emaca\tr12,r2,a1
                 "0|L--|00100000(3): 2 instructions",
                "1|L--|v5 = CONVERT(r12 *s64 r2, int64, int72)",
                "2|L--|a1 = a1 + v5");
        }

        [Test]
        public void RxRw_emsba()
        {
            Given_HexString("FD47C23");
            AssertCode(        // emsba\tr12,r2,a0
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v5 = CONVERT(r12 *s64 r2, int64, int72)",
                "2|L--|a0 = a0 - v5");
            Given_HexString("FD4FC23");
            AssertCode(        // emsba\tr12,r2,a1
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v5 = CONVERT(r12 *s64 r2, int64, int72)",
                "2|L--|a1 = a1 - v5");
        }

        [Test]
        public void RxRw_emul()
        {
            Given_HexString("FD7463 42");
            AssertCode(        // emul\t#42h,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4_r3 = r3 *s64 0x42<32>");
        }

        [Test]
        public void RxRw_emula()
        {
            Given_HexString("FD0363");
            AssertCode(        // emula\tr6,r3,a0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|a0 = CONVERT(r3 *s64 r6, int64, int72)");
        }

        [Test]
        public void RxRw_emulu()
        {
            Given_HexString("FD7C73 123456");
            AssertCode(        // emulu\t#123456h,r3
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r4_r3 = r3 *u64 0x123456<32>");
            Given_HexString("FC1E34 1234");
            AssertCode(        // emulu.l\t4660[r3],r4
                "0|L--|00100000(5): 1 instructions",
                "1|L--|r5_r4 = r4 *u64 Mem0[r3 + 4660<i32>:word32]");
            Given_HexString("06630723");
            AssertCode(        // emulu\tr2,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4_r3 = r3 *u64 r2");
        }

        [Test]
        public void RxRw_fadd_imm()
        {
            Given_HexString("FD7224 3FC00000");
            AssertCode(        // fadd\t#1.5,r4
                "0|L--|00100000(7): 2 instructions",
                "1|L--|r4 = r4 + 1.5F",
                "2|L--|SZ = cond(r4)");
        }

        [Test]
        public void RxRw_fadd_mem()
        {
            Given_HexString("FC8989 12");
            AssertCode(        // fadd.l\t18[r8],r9
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r9 = r9 + Mem0[r8 + 18<i32>:word32]",
                "2|L--|SZ = cond(r9)");
        }

        [Test]
        public void RxRw_fadd_reg()
        {
            Given_HexString("FFA231");
            AssertCode(        // fadd\tr2,r3,r1
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r1 = r3 + r2",
                "2|L--|SZ = cond(r1)");
        }

        [Test]
        public void RxRw_fcmp_imm()
        {
            Given_HexString("FD721F BF800000");
            AssertCode(        // fcmp\t#-1,r15
                "0|L--|00100000(7): 1 instructions",
                "1|L--|OSZ = cond(r15 - -1.0F)");
        }

        [Test]
        public void RxRw_fcmp_regs()
        {
            Given_HexString("FC8723");
            AssertCode(        // fcmp\tr2,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|OSZ = cond(r3 - r2)");
        }

        [Test]
        public void RxRw_fdiv_imm()
        {
            Given_HexString("FD7243 40800000");
            AssertCode(        // fdiv\t#4,r3
                "0|L--|00100000(7): 2 instructions",
                "1|L--|r3 = r3 / 4.0F",
                "2|L--|SZ = cond(r3)");
        }

        [Test]
        public void RxRw_fmul()
        {
            Given_HexString("FD7233 41800000");
            AssertCode(        // fmul\t#16,r3
                "0|L--|00100000(7): 2 instructions",
                "1|L--|r3 = r3 * 16.0F",
                "2|L--|SZ = cond(r3)");
            Given_HexString("FC8EAB 1234");
            AssertCode(        // fmul.l\t4660[r10],r11
                "0|L--|00100000(5): 2 instructions",
                "1|L--|r11 = r11 * Mem0[r10 + 4660<i32>:word32]",
                "2|L--|SZ = cond(r11)");
            Given_HexString("FFB231");
            AssertCode(        // fmul\tr2,r3,r1
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r1 = r3 * r2",
                "2|L--|SZ = cond(r1)");
        }

        [Test]
        public void RxRw_fsqrt()
        {
            Given_HexString("FCA223 4242");
            AssertCode(        // fsqrt.l\t16962[r2],r3
                "0|L--|00100000(5): 2 instructions",
                "1|L--|r3 = sqrtf(Mem0[r2 + 16962<i32>:word32])",
                "2|L--|SZ = cond(r3)");
        }

        [Test]
        public void RxRw_fsub()
        {
            Given_HexString("FD7203 BE800000");
            AssertCode(        // fsub\t#-0.25,r3
                "0|L--|00100000(7): 2 instructions",
                "1|L--|r3 = r3 - -0.25F",
                "2|L--|SZ = cond(r3)");
            Given_HexString("FC8243 BE80");
            AssertCode(        // fsub.l\t-16768[r4],r3
                "0|L--|00100000(5): 2 instructions",
                "1|L--|r3 = r3 - Mem0[r4 + -16768<i32>:word32]",
                "2|L--|SZ = cond(r3)");
            Given_HexString("FF8231");
            AssertCode(        // fsub\tr2,r3,r1
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r1 = r3 - r2",
                "2|L--|SZ = cond(r1)");
        }

        [Test]
        public void RxRw_ftoi()
        {
            Given_HexString("FC9634 0123");
            AssertCode(        // ftoi.l\t291[r3],r4
                "0|L--|00100000(5): 3 instructions",
                "1|L--|v3 = truncf(Mem0[r3 + 291<i32>:word32])",
                "2|L--|r4 = CONVERT(v3, real32, int32)",
                "3|L--|SZ = cond(r4)");
        }

        [Test]
        public void RxRw_ftou()
        {
            Given_HexString("FCA634 0123");
            AssertCode(        // ftou.l\t291[r3],r4
                "0|L--|00100000(5): 3 instructions",
                "1|L--|v3 = truncf(Mem0[r3 + 291<i32>:word32])",
                "2|L--|r4 = CONVERT(v3, real32, uint32)",
                "3|L--|SZ = cond(r4)");
        }

        [Test]
        public void RxRw_int()
        {
            Given_HexString("7560 42");
            AssertCode(        // int\t#42h
                "0|L--|00100000(3): 1 instructions",
                "1|L--|__syscall<byte>(0x42<8>)");
        }

        [Test]
        public void RxRw_itof()
        {
            Given_HexString("FC4523 42");
            AssertCode(        // itof.l\t66[r2],r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = CONVERT(Mem0[r2 + 66<i32>:word32], int32, real32)",
                "2|L--|SZ = cond(r3)");
            Given_HexString("06621123 8000");
            AssertCode(        // itof.w\t-128[r2],r3
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r3 = CONVERT(Mem0[r2 + -32768<i32>:int16], int16, real32)",
                "2|L--|SZ = cond(r3)");
        }

        [Test]
        public void RxRw_jmp()
        {
            Given_HexString("7F03");
            AssertCode(        // jmp\tr3
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto r3");
        }

        [Test]
        public void RxRw_jsr()
        {
            Given_HexString("7F13");
            AssertCode(        // jsr\tr3
                "0|T--|00100000(2): 1 instructions",
                "1|T--|call r3 (4)");
        }

        [Test]
        public void RxRw_machi()
        {
            Given_HexString("FD0C23");
            AssertCode(        // machi\tr2,r3,a1
                "0|L--|00100000(3): 4 instructions",
                "1|L--|v5 = SLICE(r3, word16, 16)",
                "2|L--|v6 = SLICE(r2, word16, 16)",
                "3|L--|v7 = v5 *s32 v6",
                "4|L--|a1 = a1 + (v7 << 0x10<8>)");
        }

        [Test]
        public void RxRw_maclh()
        {
            Given_HexString("FD0E23");
            AssertCode(        // maclh\tr2,r3,a1
                "0|L--|00100000(3): 4 instructions",
                "1|L--|v5 = SLICE(r3, word16, 16)",
                "2|L--|v6 = SLICE(r2, word16, 0)",
                "3|L--|v7 = v5 *s32 v6",
                "4|L--|a1 = a1 + (v7 << 0x10<8>)");
        }

        [Test]
        public void RxRw_maclo()
        {
            Given_HexString("FD0D23");
            AssertCode(        // maclo\tr2,r3,a1
                "0|L--|00100000(3): 4 instructions",
                "1|L--|v5 = SLICE(r3, word16, 16)",
                "2|L--|v6 = SLICE(r2, word16, 0)",
                "3|L--|v7 = v5 *s32 v6",
                "4|L--|a1 = a1 + (v7 << 0x10<8>)");
        }

        [Test]
        public void RxRw_max()
        {
            Given_HexString("FD7844 1234");
            AssertCode(        // max\t#1234h,r4
                "0|L--|00100000(5): 1 instructions",
                "1|L--|r4 = max<int32>(r4, 0x1234<32>)");
            Given_HexString("FC1223 1234");
            AssertCode(        // max.uw\t4660[r2],r3
                "0|L--|00100000(5): 1 instructions",
                "1|L--|r3 = max<int32>(r3, Mem0[r2 + 4660<i32>:word16])");
        }

        [Test]
        public void RxRw_min()
        {
            Given_HexString("FD7854 1234");
            AssertCode(        // min\t#1234h,r4
                "0|L--|00100000(5): 1 instructions",
                "1|L--|r4 = min<int32>(r4, 0x1234<32>)");
            Given_HexString("FC1623 1234");
            AssertCode(        // min.uw\t4660[r2],r3
                "0|L--|00100000(5): 1 instructions",
                "1|L--|r3 = min<int32>(r3, Mem0[r2 + 4660<i32>:word16])");
            Given_HexString("06620523 1234");
            AssertCode(        // min.w\t18[r2],r3
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r3 = min<int32>(r3, Mem0[r2 + 4660<i32>:int16])");
        }

        [Test]
        public void RxRw_mov()
        {
            Given_HexString("87AB");
            AssertCode(        // mov.b\tr3,31[r2]
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r2 + 31<i32>:int8] = r3");
            Given_HexString("9FAB");
            AssertCode(        // mov.w\t31[r2],r3
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = Mem0[r2 + 31<i32>:int16]");
            Given_HexString("66F3");
            AssertCode(        // mov\t#0Fh,r3
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = 0xF<32>");
            Given_HexString("3DAF 42");
            AssertCode(        // mov.w\t#42h,31[r2]
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v3 = 0x42<8>",
                "2|L--|Mem0[r2 + 31<i32>:int16] = v3");
            Given_HexString("7543 42");
            AssertCode(        // mov\t#42h,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = 0x42<8>");
            Given_HexString("FB3A 4242");
            AssertCode(        // mov\t#4242h,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = 0x4242<32>");
            Given_HexString("EF23");
            AssertCode(        // mov.l\tr2,r3
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = r2");
            Given_HexString("FA3A 1234 5678");
            AssertCode(        // mov.l\t#5678h,4660[r3]
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v3 = 0x5678<32>",
                "2|L--|Mem0[r3 + 4660<i32>:word32] = v3");
            Given_HexString("EE23 1234");
            AssertCode(        // mov.l\t4660[r2],r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = Mem0[r2 + 4660<i32>:word32]");
            Given_HexString("FE5523");
            AssertCode(        // mov.w\t[r5,r2],r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = Mem0[r2 + r5 * 2<32>:int16]");
            Given_HexString("EB23 1234");
            AssertCode(        // mov.l\tr3,4660[r2]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r2 + 4660<i32>:word32] = r3");
            Given_HexString("FE1523");
            AssertCode(        // mov.w\tr3,[r5,r2]
                "0|L--|00100000(3): 1 instructions",
                "1|L--|Mem0[r2 + r5 * 2<32>:int16] = r3");
            Given_HexString("EA23 1234 5678");
            AssertCode(        // mov.l\t4660[r2],22136[r3]
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v4 = Mem0[r2 + 4660<i32>:word32]",
                "2|L--|Mem0[r3 + 22136<i32>:word32] = v4");
        }

        [Test]
        public void RxRw_mov_pre_pos_inc()
        {
            Given_HexString("FD2123");
            AssertCode(        // mov.w\t[r2+],r3
                "0|L--|00100000(3): 3 instructions",
                "1|L--|v4 = r2",
                "2|L--|r2 = r2 + 2<32>",
                "3|L--|r3 = Mem0[v4:int16]");
            Given_HexString("FD2523");
            AssertCode(        // mov.w\t[-r2],r3
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r2 = r2 - 2<32>",
                "2|L--|r3 = Mem0[r2:int16]");
            Given_HexString("FD2923");
            AssertCode(        // mov.w\tr3,[r2+]
                "0|L--|00100000(3): 3 instructions",
                "1|L--|v5 = r2",
                "2|L--|r2 = r2 + 2<32>",
                "3|L--|Mem0[v5:int16] = r3");
            Given_HexString("FD2D23");
            AssertCode(        // mov.w\tr3,[-r2]
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r2 = r2 - 2<32>",
                "2|L--|Mem0[r2:int16] = r3");
        }

        [Test]
        [Ignore("LI flag poorly documented")]
        public void RxRw_movco()
        {
            Given_HexString("FD2723");
            AssertCode(        // movco\tr3,r2
                "@@@");
        }

        [Test]
        [Ignore("LI flag poorly documented")]
        public void RxRw_movli()
        {
            Given_HexString("FD2F23");
            AssertCode(        // movli\tr3,r2
                "@@@");
        }

        [Test]
        public void RxRw_movu()
        {
            Given_HexString("B7AB");
            AssertCode(        // movu.b\t31[r2],r7
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = CONVERT(Mem0[r2 + 31<i32>:int8], int8, word32)");
            Given_HexString("5E34 1234");
            AssertCode(        // movu.w\t4660[r3],r4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = CONVERT(Mem0[r3 + 4660<i32>:int16], int16, word32)");
            Given_HexString("FED342");
            AssertCode(        // movu.w\t[r3,r4],r2
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r2 = CONVERT(Mem0[r4 + r3 * 2<32>:int16], int16, word32)");
            Given_HexString("FD3D34");
            AssertCode(        // movu.w\t[-r3],r4
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r3 = r3 - 2<32>",
                "2|L--|r4 = CONVERT(Mem0[r3:int16], int16, word32)");
        }

        [Test]
        public void RxRw_msbhi()
        {
            Given_HexString("FD4C23");
            AssertCode(        // msbhi\tr2,r3,a1
                "0|L--|00100000(3): 4 instructions",
                "1|L--|v5 = SLICE(r3, word16, 16)",
                "2|L--|v6 = SLICE(r2, word16, 16)",
                "3|L--|v7 = v5 *s32 v6",
                "4|L--|a1 = a1 - (v7 << 0x10<8>)");
        }

        [Test]
        public void RxRw_msblh()
        {
            Given_HexString("FD4E23");
            AssertCode(        // msblh\tr2,r3,a1
                "0|L--|00100000(3): 4 instructions",
                "1|L--|v5 = SLICE(r3, word16, 16)",
                "2|L--|v6 = SLICE(r2, word16, 0)",
                "3|L--|v7 = v5 *s32 v6",
                "4|L--|a1 = a1 - (v7 << 0x10<8>)");
        }

        [Test]
        public void RxRw_msblo()
        {
            Given_HexString("FD4D23");
            AssertCode(        // msblo\tr2,r3,a1
                "0|L--|00100000(3): 4 instructions",
                "1|L--|v5 = SLICE(r3, word16, 16)",
                "2|L--|v6 = SLICE(r2, word16, 0)",
                "3|L--|v7 = v5 *s32 v6",
                "4|L--|a1 = a1 - (v7 << 0x10<8>)");
        }

        [Test]
        public void RxRw_mul()
        {
            Given_HexString("63F3");
            AssertCode(        // mul\t#0Fh,r3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r3 = r3 * 0xF<32>",
                "2|L--|SZ = cond(r3)");
            Given_HexString("7713 123456");
            AssertCode(        // mul\t#123456h,r3
                "0|L--|00100000(5): 2 instructions",
                "1|L--|r3 = r3 * 0x123456<32>",
                "2|L--|SZ = cond(r3)");
            Given_HexString("4E23 1234");
            AssertCode(        // mul.ub\t4660[r2],r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r3 *32 Mem0[r2 + 4660<i32>:byte]",
                "2|L--|SZ = cond(r3)");
            Given_HexString("06CE23 1234");
            AssertCode(        // mul.b\t4660[r2],r3
                "0|L--|00100000(5): 2 instructions",
                "1|L--|r3 = r3 *32 Mem0[r2 + 4660<i32>:int8]",
                "2|L--|SZ = cond(r3)");
            Given_HexString("FF3231");
            AssertCode(        // mul\tr2,r3,r1
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r1 = r3 * r2",
                "2|L--|SZ = cond(r1)");
        }

        [Test]
        public void RxRw_mulhi()
        {
            Given_HexString("FD0823");
            AssertCode(        // mulhi\tr2,r3,a1
                "0|L--|00100000(3): 4 instructions",
                "1|L--|v5 = SLICE(r3, word16, 16)",
                "2|L--|v6 = SLICE(r2, word16, 16)",
                "3|L--|v7 = CONVERT(v5 *s32 v6, int32, int72)",
                "4|L--|a1 = v7 << 0x10<8>");
        }

        [Test]
        public void RxRw_mullh()
        {
            Given_HexString("FD0A23");
            AssertCode(        // mullh\tr2,r3,a1
                "0|L--|00100000(3): 4 instructions",
                "1|L--|v5 = SLICE(r3, word16, 16)",
                "2|L--|v6 = SLICE(r2, word16, 0)",
                "3|L--|v7 = CONVERT(v5 *s32 v6, int32, int72)",
                "4|L--|a1 = v7 << 0x10<8>");
        }

        [Test]
        public void RxRw_mullo()
        {
            Given_HexString("FD0923");
            AssertCode(        // mullo\tr2,r3,a1
                "0|L--|00100000(3): 4 instructions",
                "1|L--|v5 = SLICE(r3, word16, 16)",
                "2|L--|v6 = SLICE(r2, word16, 0)",
                "3|L--|v7 = CONVERT(v5 *s32 v6, int32, int72)",
                "4|L--|a1 = v7 << 0x10<8>");
        }

        [Test]
        public void RxRw_mvfacgu()
        {
            Given_HexString("FD1FF3");
            AssertCode(        // mvfacgu\t#1h,a1,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = __move_from_accumulator_guard(1<32>, a1)");
        }

        [Test]
        public void RxRw_mvfachi()
        {
            Given_HexString("FD1FC3");
            AssertCode(        // mvfachi\t#1h,a1,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = __move_from_accumulator_high(1<32>, a1)");
        }

        [Test]
        public void RxRw_mvfaclo()
        {
            Given_HexString("FD1FD3");
            AssertCode(        // mvfaclo\t#1h,a1,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = __move_from_accumulator_low(1<32>, a1)");
        }

        [Test]
        public void RxRw_mvfacmi()
        {
            Given_HexString("FD1FE3");
            AssertCode(        // mvfacmi\t#1h,a1,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = __move_from_accumulator_middle(1<32>, a1)");
        }

        [Test]
        public void RxRw_mvfc()
        {
            Given_HexString("FD6AD3");
            AssertCode(        // mvfc\tEXTB,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = __read_control_register(EXTB)");
        }

        [Test]
        public void RxRw_mvtacgu()
        {
            Given_HexString("FD17B3");
            AssertCode(        // mvtacgu\tr3,a1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|a1 = __move_to_accumulator_guard(r3, a1)");
        }

        [Test]
        public void RxRw_mvtachi()
        {
            Given_HexString("FD1783");
            AssertCode(        // mvtachi\tr3,a1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|a1 = __move_to_accumulator_high(r3, a1)");
        }

        [Test]
        public void RxRw_mvtaclo()
        {
            Given_HexString("FD1793");
            AssertCode(        // mvtaclo\tr3,a1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|a1 = __move_to_accumulator_low(r3, a1)");
        }

        [Test]
        public void RxRw_mvtc()
        {
            Given_HexString("FD7303 12345678");
            AssertCode(        // mvtc\t#12345678h,FPSW
                "0|L--|00100000(7): 1 instructions",
                "1|L--|__write_control_register(FPSW, 0x12345678<32>)");
            Given_HexString("FD68A3");
            AssertCode(        // mvtc\tr10,FPSW
                "0|L--|00100000(3): 1 instructions",
                "1|L--|__write_control_register(FPSW, r10)");
        }

        [Test]
        public void RxRw_mvtipl()
        {
            Given_HexString("75700F");
            AssertCode(        // mvtipl\t#0h
                "0|L--|00100000(3): 1 instructions",
                "1|L--|PSW = __move_to_interrupt_priority_level(PSW, 0xF<8>)");
        }

        [Test]
        public void RxRw_neg()
        {
            Given_HexString("7E13");
            AssertCode(        // neg\tr3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r3 = -r3",
                "2|L--|COSZ = cond(r3)");
            Given_HexString("FC0734");
            AssertCode(        // neg\tr3,r4
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r4 = -r3",
                "2|L--|COSZ = cond(r4)");
        }

        [Test]
        public void RxRw_not()
        {
            Given_HexString("7E03");
            AssertCode(        // not\tr3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r3 = ~r3",
                "2|L--|SZ = cond(r3)");
            Given_HexString("FC3B34");
            AssertCode(        // not\tr3,r4
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r4 = ~r3",
                "2|L--|SZ = cond(r4)");
        }

        [Test]
        public void RxRw_or()
        {
            Given_HexString("65AA");
            AssertCode(        // or\t#0Ah,r10
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r10 = r10 | 0xA<32>",
                "2|L--|SZ = cond(r10)");
            Given_HexString("7634 1234");
            AssertCode(        // or\t#1234h,r4
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r4 = r4 | 0x1234<32>",
                "2|L--|SZ = cond(r4)");
            Given_HexString("5523 34");
            AssertCode(        // or.ub\t52[r2],r3
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r3 = r3 | Mem0[r2 + 52<i32>:byte]",
                "2|L--|SZ = cond(r3)");
            Given_HexString("06D522 34");
            AssertCode(        // or.b\t52[r2],r2
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2 = r2 | Mem0[r2 + 52<i32>:int8]",
                "2|L--|SZ = cond(r2)");
            Given_HexString("FF5231");
            AssertCode(        // or\tr2,r3,r1
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r1 = r3 | r2",
                "2|L--|SZ = cond(r1)");
        }

        [Test]
        public void RxRw_pop()
        {
            Given_HexString("7EBA");
            AssertCode(        // pop\tr10
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r10 = Mem0[sp:word32]",
                "2|L--|sp = sp + 4<i32>");
        }

        [Test]
        public void RxRw_popc()
        {
            Given_HexString("7EEA");
            AssertCode(        // popc\tISP
                "0|L--|00100000(2): 2 instructions",
                "1|L--|__write_control_register(ISP, Mem0[sp:word32])",
                "2|L--|sp = sp + 4<i32>");
        }

        [Test]
        public void RxRw_popm()
        {
            Given_HexString("6F24");
            AssertCode(        // popm\tr2-r4
                "0|L--|00100000(2): 6 instructions",
                "1|L--|r2 = Mem0[sp:word32]",
                "2|L--|sp = sp + 4<i32>",
                "3|L--|r3 = Mem0[sp:word32]",
                "4|L--|sp = sp + 4<i32>",
                "5|L--|r4 = Mem0[sp:word32]",
                "6|L--|sp = sp + 4<i32>");
        }

        [Test]
        public void RxRw_push()
        {
            Given_HexString("7E93");
            AssertCode(        // push\tr3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|sp = sp - 4<i32>",
                "2|L--|Mem0[sp:word32] = r3");
            Given_HexString("F639 1234");
            AssertCode(        // push.w\t4660[r3]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|sp = sp - 2<i32>",
                "2|L--|Mem0[sp:int16] = Mem0[r3 + 4660<i32>:int16]");
        }

        [Test]
        public void RxRw_pushc()
        {
            Given_HexString("7EC9");
            AssertCode(        // pushc\tBPC
                "0|L--|00100000(2): 2 instructions",
                "1|L--|sp = sp - 4<i32>",
                "2|L--|Mem0[sp:word32] = __read_control_register(BPC)");
        }

        [Test]
        public void RxRw_pushm()
        {
            Given_HexString("6E24");
            AssertCode(        // pushm\tr2-r4
                "0|L--|00100000(2): 6 instructions",
                "1|L--|sp = sp - 4<i32>",
                "2|L--|Mem0[sp:word32] = r4",
                "3|L--|sp = sp - 4<i32>",
                "4|L--|Mem0[sp:word32] = r3",
                "5|L--|sp = sp - 4<i32>",
                "6|L--|Mem0[sp:word32] = r2");
        }

        [Test]
        public void RxRw_racl()
        {
            Given_HexString("FD1990");
            AssertCode(        // racl\t#2h,a1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|a1 = __round_accumulator_long(2<32>)");
        }

        [Test]
        public void RxRw_racw()
        {
            Given_HexString("FD1810");
            AssertCode(        // racw\t#2h,a0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|a0 = __round_accumulator_word(2<32>)");
        }

        [Test]
        public void RxRw_rdacl()
        {
            Given_HexString("FD19D0");
            AssertCode(        // rdacl\t#2h,a1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|a1 = __round_down_accumulator_long(2<32>)");
        }

        [Test]
        public void RxRw_rdacw()
        {
            Given_HexString("FD18D0");
            AssertCode(        // rdacw\t#2h,a0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|a0 = __round_down_accumulator_word(2<32>)");
        }

        [Test]
        public void RxRw_revl()
        {
            Given_HexString("FD6723");
            AssertCode(        // revl\tr2,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = __reverse_long(r2)");
        }

        [Test]
        public void RxRw_revw()
        {
            Given_HexString("FD6523");
            AssertCode(        // revw\tr2,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = __reverse_words(r2)");
        }

        [Test]
        public void RxRw_rmpa()
        {
            Given_HexString("7F8E");
            AssertCode(        // rmpa.l
                 "0|L--|00100000(2): 6 instructions",
                "1|T--|if (r3 == 0<32>) branch 00100002",
                "2|L--|v7 = CONVERT(Mem0[r1:word32] *s64 Mem0[r2:word32], int64, int96)",
                "3|L--|r1 = r1 + 4<32>",
                "4|L--|r2 = r2 + 4<32>",
                "5|L--|r3 = r3 - 1<32>",
                "6|T--|goto 00100000");
        }

        [Test]
        public void RxRw_rolc()
        {
            Given_HexString("7E52");
            AssertCode(        // rolc\tr2
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r2 = __rcl<word32,byte>(r2, 1<8>, C)",
                "2|L--|CZ = cond(r2)");
        }

        [Test]
        public void RxRw_rorc()
        {
            Given_HexString("7E42");
            AssertCode(        // rorc\tr2
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r2 = __rcr<word32,byte>(r2, 1<8>, C)",
                "2|L--|CZ = cond(r2)");
        }

        [Test]
        public void RxRw_rotl()
        {
            Given_HexString("FD6FF3");
            AssertCode(        // rotl\t#1Fh,r3
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r3 = __rol<word32,word32>(r3, 0x1F<32>)",
                "2|L--|CZ = cond(r3)");
            Given_HexString("FD6634");
            AssertCode(        // rotl\tr3,r4
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r4 = __rol<word32,word32>(r4, r3)",
                "2|L--|CZ = cond(r4)");
        }

        [Test]
        public void RxRw_rotr()
        {
            Given_HexString("FD6DF3");
            AssertCode(        // rotr\t#1Fh,r3
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r3 = __ror<word32,word32>(r3, 0x1F<32>)",
                "2|L--|CZ = cond(r3)");
            Given_HexString("FD6434");
            AssertCode(        // rotr\tr3,r4
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r4 = __ror<word32,word32>(r4, r3)",
                "2|L--|CZ = cond(r4)");
        }

        [Test]
        public void RxRw_round()
        {
            Given_HexString("FC9923 FE");
            AssertCode(        // round.l\t-2[r2],r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = roundf(Mem0[r2 + -2<i32>:word32])",
                "2|L--|SZ = cond(r3)");
        }

        [Test]
        public void RxRw_rte()
        {
            Given_HexString("7F95");
            AssertCode(        // rte
                "0|R--|00100000(2): 2 instructions",
                "1|L--|__return_from_exception()",
                "2|R--|return (4,0)");
        }

        [Test]
        public void RxRw_rtfi()
        {
            Given_HexString("7F94");
            AssertCode(        // rtfi
                "0|R--|00100000(2): 2 instructions",
                "1|L--|__return_from_fast_interrupt()",
                "2|R--|return (4,0)");
        }

        [Test]
        public void RxRw_rts()
        {
            Given_HexString("02");
            AssertCode(        // rts
                "0|R--|00100000(1): 1 instructions",
                "1|R--|return (4,0)");
        }

        [Test]
        public void RxRw_rtsd()
        {
            Given_HexString("6702");
            AssertCode(        // rtsd\t#2h
                 "0|L--|00100000(2): 1 instructions",
                 "1|R--|return (4,2)");
            Given_HexString("3F13 02");
            AssertCode(        // rtsd\t#2h,r1-r3
                "0|L--|00100000(3): 7 instructions",
                "1|L--|r1 = Mem0[sp:word32]",
                "2|L--|sp = sp + 4<i32>",
                "3|L--|r2 = Mem0[sp:word32]",
                "4|L--|sp = sp + 4<i32>",
                "5|L--|r3 = Mem0[sp:word32]",
                "6|L--|sp = sp + 4<i32>",
                "7|R--|return (4,2)");
        }

        [Test]
        public void RxRw_sat()
        {
            Given_HexString("7E32");
            AssertCode(        // sat\tr2
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r2 = __saturate<word32>(OS, r2)");
        }

        [Test]
        public void RxRw_satr()
        {
            Given_HexString("7F93");
            AssertCode(        // satr
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r6_r5_r4 = __saturate<word96>(OS, r6_r5_r4)");
        }

        [Test]
        public void RxRw_sbb()
        {
            Given_HexString("FC0323");
            AssertCode(        // sbb\tr2,r3
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r3 = r3 - r2 - C",
                "2|L--|COSZ = cond(r3)");
            Given_HexString("06A20023 1234");
            AssertCode(        // sbb.l\t4660[r2],r3
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r3 = r3 - Mem0[r2 + 4660<i32>:word32] - C",
                "2|L--|COSZ = cond(r3)");
        }

        [Test]
        public void RxRw_sceq()
        {
            Given_HexString("FCD130 02");
            AssertCode(        // sceq.b\t2[r3]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Test(EQ,Z) ? 1<i8> : 0<i8>",
                "2|L--|Mem0[r3 + 2<i32>:int8] = v4");
        }

        [Test]
        public void RxRw_scmpu()
        {
            Given_HexString("7F83");
            AssertCode(        // scmpu
                "0|L--|00100000(2): 7 instructions",
                "1|T--|if (r3 == 0<32>) branch 00100002",
                "2|L--|v6 = Mem0[r1:byte]",
                "3|L--|r1 = r1 + 1<32>",
                "4|L--|v7 = Mem0[r2:byte]",
                "5|L--|r2 = r2 + 1<32>",
                "6|L--|r3 = r3 - 1<32>",
                "7|T--|if (v6 == v7 && v6 != 0<8>) branch 00100000");
        }

        [Test]
        public void RxRw_setpsw()
        {
            Given_HexString("7FA0");
            AssertCode(        // setpsw\tc
                "0|L--|00100000(2): 1 instructions",
                "1|L--|C = 1<32>");
        }

        [Test]
        public void RxRw_shar()
        {
            Given_HexString("6BF3");
            AssertCode(        // shar\t#1Fh,r3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r3 = r3 >> 0x1F<32>",
                "2|L--|CZ = cond(r3)");
            Given_HexString("FD6132");
            AssertCode(        // shar\tr3,r2
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r2 = r2 >> r3",
                "2|L--|CZ = cond(r2)");
            Given_HexString("FDB132");
            AssertCode(        // shar\t#11h,r3,r2
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r2 = r3 >> 0x11<32>",
                "2|L--|CZ = cond(r2)");
        }

        [Test]
        public void RxRw_shll()
        {
            Given_HexString("6DF3");
            AssertCode(        // shll\t#1Fh,r3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r3 = r3 << 0x1F<32>",
                "2|L--|CZ = cond(r3)");
            Given_HexString("FD6232");
            AssertCode(        // shll\tr3,r2
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r2 = r2 << r3",
                "2|L--|CZ = cond(r2)");
            Given_HexString("FDD132");
            AssertCode(        // shll\t#11h,r3,r2
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r2 = r3 << 0x11<32>",
                "2|L--|CZ = cond(r2)");
        }

        [Test]
        public void RxRw_shlr()
        {
            Given_HexString("69F3");
            AssertCode(        // shlr\t#1Fh,r3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r3 = r3 >>u 0x1F<32>",
                "2|L--|CZ = cond(r3)");
            Given_HexString("FD6032");
            AssertCode(        // shlr\tr3,r2
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r2 = r2 >>u r3",
                "2|L--|CZ = cond(r2)");
            Given_HexString("FD9132");
            AssertCode(        // shlr\t#11h,r3,r2
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r2 = r3 >>u 0x11<32>",
                "2|L--|CZ = cond(r2)");
        }

        [Test]
        public void RxRw_smovb()
        {
            Given_HexString("7F8B");
            AssertCode(        // smovb
                "0|L--|00100000(2): 7 instructions",
                "1|T--|if (r3 == 0<32>) branch 00100002",
                "2|L--|v6 = Mem0[r2:byte]",
                "3|L--|r2 = r2 - 1<32>",
                "4|L--|Mem0[r1:byte] = v6",
                "5|L--|r1 = r1 - 1<32>",
                "6|L--|r3 = r3 - 1<32>",
                "7|T--|goto 00100000");
        }

        [Test]
        public void RxRw_smovf()
        {
            Given_HexString("7F8F");
            AssertCode(        // smovf
                "0|L--|00100000(2): 7 instructions",
                "1|T--|if (r3 == 0<32>) branch 00100002",
                "2|L--|v6 = Mem0[r2:byte]",
                "3|L--|r2 = r2 + 1<32>",
                "4|L--|Mem0[r1:byte] = v6",
                "5|L--|r1 = r1 + 1<32>",
                "6|L--|r3 = r3 - 1<32>",
                "7|T--|goto 00100000");
        }

        [Test]
        public void RxRw_smovu()
        {
            Given_HexString("7F87");
            AssertCode(        // smovu
                "0|L--|00100000(2): 7 instructions",
                "1|T--|if (r3 == 0<32>) branch 00100002",
                "2|L--|v6 = Mem0[r2:byte]",
                "3|L--|r2 = r2 + 1<32>",
                "4|L--|Mem0[r1:byte] = v6",
                "5|L--|r1 = r1 + 1<32>",
                "6|L--|r3 = r3 - 1<32>",
                "7|T--|if (v6 != 0<8>) branch 00100000");
        }

        [Test]
        public void RxRw_sstr()
        {
            Given_HexString("7F89");
            AssertCode(        // sstr.w
                "0|L--|00100000(2): 6 instructions",
                "1|L--|v7 = SLICE(r2, int16, 0)",
                "2|T--|if (r3 == 0<32>) branch 00100002",
                "3|L--|Mem0[r1:int16] = v7",
                "4|L--|r1 = r1 + 1<32>",
                "5|L--|r3 = r3 - 1<32>",
                "6|T--|goto 00100000");
        }

        [Test]
        public void RxRw_stnz()
        {
            Given_HexString("FD78F3 1234");
            AssertCode(        // stnz\t#1234h,r3
                "0|L--|00100000(5): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100005",
                "2|L--|r3 = 0x1234<32>");
            Given_HexString("FC4B23");
            AssertCode(        // stnz\tr2,r3
                "0|L--|00100000(3): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100003",
                "2|L--|r3 = r2");
        }

        [Test]
        public void RxRw_stz()
        {
            Given_HexString("FD78E3 1234");
            AssertCode(        // stz\t#1234h,r3
                "0|L--|00100000(5): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100005",
                "2|L--|r3 = 0x1234<32>");
            Given_HexString("FC4B23");
            AssertCode(        // stnz\tr2,r3
                "0|L--|00100000(3): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100003",
                "2|L--|r3 = r2");
        }

        [Test]
        public void RxRw_sub()
        {
            Given_HexString("6023");
            AssertCode(        // sub\t#2h,r3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r3 = r3 - 2<32>",
                "2|L--|COSZ = cond(r3)");
            Given_HexString("4123 80");
            AssertCode(        // sub.ub\t-128[r2],r3
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r3 = r3 - Mem0[r2 + -128<i32>:byte]",
                "2|L--|COSZ = cond(r3)");
            Given_HexString("06C123 80");
            AssertCode(        // sub.b\t-128[r2],r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r3 - Mem0[r2 + -128<i32>:int8]",
                "2|L--|COSZ = cond(r3)");
            Given_HexString("FF0231");
            AssertCode(        // sub\tr2,r3,r1
                "0|L--|00100000(3): 2 instructions",
                "1|L--|r1 = r3 - r2",
                "2|L--|COSZ = cond(r1)");
        }

        [Test]
        public void RxRw_suntil()
        {
            Given_HexString("7F82");
            AssertCode(        // suntil.l
                "0|L--|00100000(2): 6 instructions",
                "1|T--|if (r3 == 0<32>) branch 00100002",
                "2|L--|v6 = CONVERT(Mem0[r1:byte], byte, word32)",
                "3|L--|r1 = r1 + 1<32>",
                "4|L--|r3 = r3 - 1<32>",
                "5|L--|CZ = cond(v6 - r2)",
                "6|T--|if (Test(NE,CZ)) branch 00100000");
        }

        [Test]
        public void RxRw_swhile()
        {
            Given_HexString("7F85");
            AssertCode(        // swhile.w
                "0|L--|00100000(2): 6 instructions",
                "1|T--|if (r3 == 0<32>) branch 00100002",
                "2|L--|v6 = CONVERT(Mem0[r1:byte], byte, word32)",
                "3|L--|r1 = r1 + 1<32>",
                "4|L--|r3 = r3 - 1<32>",
                "5|L--|CZ = cond(v6 - r2)",
                "6|T--|if (Test(EQ,CZ)) branch 00100000");
        }

        [Test]
        public void RxRw_tst()
        {
            Given_HexString("FD78C3 1234");
            AssertCode(        // tst\t#1234h,r3
                "0|L--|00100000(5): 1 instructions",
                "1|L--|SZ = cond(r3 & 0x1234<32>)");
            Given_HexString("FC3123 12");
            AssertCode(        // tst.ub\t18[r2],r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|SZ = cond(r3 & Mem0[r2 + 18<i32>:byte])");
            AssertCode(        // tst.w\t18[r2],r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|SZ = cond(r3 & Mem0[r2 + 18<i32>:byte])");
        }

        [Test]
        public void RxRw_utof()
        {
            Given_HexString("FC5523 12");
            AssertCode(        // utof.ub\t18[r2],r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = CONVERT(Mem0[r2 + 18<i32>:byte], uint8, real32)",
                "2|L--|SZ = cond(r3)");
            Given_HexString("06611523 12");
            AssertCode(        // utof.w\t18[r2],r3
                "0|L--|00100000(5): 2 instructions",
                "1|L--|r3 = CONVERT(Mem0[r2 + 18<i32>:int16], uint16, real32)",
                "2|L--|SZ = cond(r3)");
        }

        [Test]
        public void RxRw_wait()
        {
            Given_HexString("7F96");
            AssertCode(        // wait
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__wait()");
        }

        [Test]
        public void RxRw_xchg()
        {
            Given_HexString("FC4123 12");
            AssertCode(        // xchg.ub\t18[r2],r3
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = Mem0[r2 + 18<i32>:byte]",
                "2|L--|Mem0[r2 + 18<i32>:byte] = r3",
                "3|L--|r3 = v5");
            Given_HexString("06611023 32");
            AssertCode(        // xchg.w\t50[r2],r3
                "0|L--|00100000(5): 3 instructions",
                "1|L--|v5 = Mem0[r2 + 50<i32>:int16]",
                "2|L--|Mem0[r2 + 50<i32>:int16] = r3",
                "3|L--|r3 = v5");
        }

        [Test]
        public void RxRw_xor()
        {
            Given_HexString("FD74D3 12");
            AssertCode(        // xor\t#12h,r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r3 ^ 0x12<32>",
                "2|L--|SZ = cond(r3)");
            Given_HexString("FC3523 12");
            AssertCode(        // xor.ub\t18[r2],r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r3 ^ Mem0[r2 + 18<i32>:byte]",
                "2|L--|SZ = cond(r3)");
            Given_HexString("06610D23 12");
            AssertCode(        // xor.w\t18[r2],r3
                "0|L--|00100000(5): 2 instructions",
                "1|L--|r3 = r3 ^ Mem0[r2 + 18<i32>:int16]",
                "2|L--|SZ = cond(r3)");
        }

    }
}
