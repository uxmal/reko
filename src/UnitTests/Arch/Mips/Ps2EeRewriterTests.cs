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
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Mips;

using static MipsGenerator;

[TestFixture]
public class Ps2EeRewriterTests : RewriterTestBase
{
    private readonly MipsArchitecture arch;
    private readonly Address addrBase;

    public Ps2EeRewriterTests()
    {
        var options = new Dictionary<string, object>
        {
           { ProcessorOption.InstructionSet, "ps2ee"  }
        };
        this.arch = new MipsLe32Architecture(CreateServiceContainer(), "mips-32-le", options);
        this.addrBase = Address.Ptr32(0x0010_0000);
    }

    public override IProcessorArchitecture Architecture => this.arch;
    public override Address LoadAddress => addrBase;

    private void AssertCode(uint instr, params string[] expected)
    {
        Given_UInt32s(instr);
        base.AssertCode(expected);
    }

    [Test]
    public void Ps2EeRw_add_s()
    {
        AssertCode((uint) (0x46000000 | (3 << 16) | (2 << 11) | (1 << 6)),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|f1 = f2 + f3");
    }

    [Test]
    public void Ps2EeRw_addiu()
    {
        AssertCode(W(9, rs: 4, rt: 2, rest: 0xFFF8 & 0xFFFF),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r4 + -8<i32>");
    }

    [Test]
    public void Ps2EeRw_addu_zero_optimization()
    {
        // addu r2,r0,r4 == move
        AssertCode(W(0, rs: 0, rt: 4, rd: 2, funct: 0x21),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r4");
    }

    [Test]
    public void Ps2EeRw_and_or_xor()
    {
        AssertCode(W(0, rs: 4, rt: 5, rd: 2, funct: 0x24),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r4 & r5");
        AssertCode(W(0, rs: 4, rt: 5, rd: 2, funct: 0x25),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r4 | r5");
        AssertCode(W(0x0E, rs: 4, rt: 2, rest: 0xFF),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r4 ^ 0xFF<32>");
    }

    [Test]
    public void Ps2EeRw_bc1t()
    {
        AssertCode((uint) (0x45010000 | 1),
            "0|TD-|00100000(4): 1 instructions",
            "1|TD-|if (cc1) branch 00100008");
    }


    [Test]
    public void Ps2EeRw_beq()
    {
        AssertCode(W(4, rs: 4, rt: 5, rest: 1),
            "0|TD-|00100000(4): 1 instructions",
            "1|TD-|if (r4 == r5) branch 00100008");
    }

    [Test]
    public void Ps2EeRw_beq_self_is_goto()
    {
        AssertCode(W(4, rs: 4, rt: 4, rest: 1),
            "0|TD-|00100000(4): 1 instructions",
            "1|TD-|goto 00100008");
    }

    [Test]
    public void Ps2EeRw_bltz_never_taken_for_r0()
    {
        AssertCode(W(1, rs: 0, rt: 0, rest: 1),
            "0|L--|00100000(4): 0 instructions");
    }

    [Test]
    public void Ps2EeRw_c_lt_s_sets_flag()
    {
        AssertCode(W(0b010001, rs:0b10000, rd:2, rt:3, funct: 0x34),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|cc1 = f2 < f3");
    }

    [Test]
    public void Ps2EeRw_cfc2_ctc2()
    {
        AssertCode(Cop2W(2, rt: 2, rd: 5),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = vi5");
        AssertCode(Cop2W(6, rt: 2, rd: 8),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|vi8 = r2");
    }


    [Test]
    public void Ps2EeRw_cvt_s_w()
    {
        Given_HexString("20008046");
        AssertCode(     // cvt.s.w	f0,f0
            "0|L--|00100000(4): 1 instructions",
            "1|L--|f0 = CONVERT(f0, int32, real32)");
    }

    [Test]
    public void Ps2EeRw_cvt_w_s()
    {
        Given_HexString("A4000046");
        AssertCode(     // cvt.w.s	f2,f0
            "0|L--|00100000(4): 1 instructions",
            "1|L--|f2 = CONVERT(roundf(f0), real32, int32)");
    }


    [Test]
    public void Ps2EeRw_div()
    {
        AssertCode(W(0, rs: 4, rt: 5, funct: 0x1A),
            "0|L--|00100000(4): 4 instructions",
            "1|L--|v7 = SLICE(r4, word32, 0) / SLICE(r5, word32, 0)",
            "2|L--|lo = CONVERT(v7, word32, int64)",
            "3|L--|v8 = SLICE(r4, word32, 0) %s SLICE(r5, word32, 0)",
            "4|L--|hi = CONVERT(v8, word32, int64)");
    }

    [Test]
    public void Ps2EeRw_div1()
    {
        Given_HexString("1A00E370");
        AssertCode(     // div1	r0,r7,r3
            "0|L--|00100000(4): 4 instructions",
            "1|L--|v7 = SLICE(r7, word32, 0) / SLICE(r3, word32, 0)",
            "2|L--|lo1 = CONVERT(v7, word32, int64)",
            "3|L--|v8 = SLICE(r7, word32, 0) %s SLICE(r3, word32, 0)",
            "4|L--|hi1 = CONVERT(v8, word32, int64)");
    }

    [Test]
    public void Ps2EeRw_div1_uses_second_accumulator()
    {
        AssertCode(MmiW(4, 5, 0, 0, 0x1A),
            "0|L--|00100000(4): 4 instructions",
            "1|L--|v7 = SLICE(r4, word32, 0) / SLICE(r5, word32, 0)",
            "2|L--|lo1 = CONVERT(v7, word32, int64)",
            "3|L--|v8 = SLICE(r4, word32, 0) %s SLICE(r5, word32, 0)",
            "4|L--|hi1 = CONVERT(v8, word32, int64)");
        }


    [Test]
    public void Ps2EeRw_dsll32()
    {
        AssertCode(W(0, rt: 3, rd: 2, sa: 2, funct: 0x3C),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r3 << 0x22<8>");
    }

    [Test]
    public void Ps2EeRw_eret()
    {
        AssertCode(W(0x10, rs: 0x10, funct: 0x18),
            "0|R--|00100000(4): 1 instructions",
            "1|R--|return (0,0)");
    }

    [Test]
    public void Ps2EeRw_ei()
    {
        Given_HexString("38000042");
        AssertCode(     // ei
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__enable_interrupts()");
    }

    [Test]
    public void Ps2EeRw_jr_ra_returns()
    {
        AssertCode(W(0, rs: 31, funct: 0x08),
            "0|RD-|00100000(4): 1 instructions",
            "1|RD-|return (0,0)");
    }

    [Test]
    public void Ps2EeRw_jalr()
    {
        AssertCode(W(0, rs: 25, rd: 31, funct: 0x09),
            "0|TD-|00100000(4): 1 instructions",
            "1|TD-|call SLICE(r25, ptr32, 0) (0)");
    }

    [Test]
    public void Ps2EeRw_lb_sign_extend()
    {
        AssertCode(W(0x20, rs: 29, rt: 4, rest: 0),
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v4 = SLICE(sp, ptr32, 0)",
            "2|L--|r4 = CONVERT(Mem0[v4:byte], byte, int128)");
    }

    [Test]
    public void Ps2EeRw_ldl_ldr()
    {
        AssertCode(W(0x1A, rs: 5, rt: 4, rest: 7),
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = __ldl(r5, 7<i32>)",
            "2|L--|r4 = CONVERT(v5, word64, int128)");
    }

    [Test]
    public void Ps2EeRw_lq_sq()
    {
        AssertCode(W(0x1E, rs: 29, rt: 2, rest: 0x10),
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v4 = SLICE(sp, ptr32, 0)",
            "2|L--|r2 = Mem0[v4 + 16<i32>:word128]");
        AssertCode(W(0x1F, rs: 28, rt: 3, rest: 0xFFF0),
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = SLICE(r28, ptr32, 0)",
            "2|L--|Mem0[v5 - 16<i32>:word128] = r3");
    }

    [Test]
    public void Ps2EeRw_lqc2_sqc2()
    {
        AssertCode(W(0x36, rs: 4, rt: 16, rest: 0x20),
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v4 = SLICE(r4, ptr32, 0)",
            "2|L--|vf16 = Mem0[v4 + 32<i32>:word128]");
        AssertCode(W(0x3E, rs: 5, rt: 17, rest: 0x30),
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = SLICE(r5, ptr32, 0)",
            "2|L--|Mem0[v5 + 48<i32>:word128] = vf17");
    }

    [Test]
    public void Ps2EeRw_lui()
    {
        AssertCode(W(0x0F, rt: 2, rest: 0x1234),
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v4 = 0x12340000<32>",
            "2|L--|r2 = CONVERT(v4, word32, int128)");
    }

    [Test]
    public void Ps2EeRw_lw_sw()
    {
        AssertCode(W(0x23, rs: 29, rt: 4, rest: 8),
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v4 = SLICE(sp, ptr32, 0)",
            "2|L--|r4 = CONVERT(Mem0[v4 + 8<i32>:word32], word32, int128)");
        AssertCode(W(0x2B, rs: 29, rt: 4, rest: 12),
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = SLICE(sp, ptr32, 0)",
            "2|L--|Mem0[v5 + 12<i32>:word32] = SLICE(r4, word32, 0)");
    }

    [Test]
    public void Ps2EeRw_madda_s_accumulates()
    {
        AssertCode(W(0b010001, rs:0b10000, rd:2, rt:3, funct: 0x1E),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|acc = acc + f2 * f3");
    }

    [Test]
    public void Ps2EeRw_mfhi_mflo()
    {
        AssertCode(W(0, rd: 2, funct: 0x10),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = SEQ(SLICE(r2, word64, 64), hi)");
    }


    [Test]
    public void Ps2EeRw_mfhi1()
    {
        Given_HexString("10300070");
        AssertCode(     // mfhi1	r6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r6 = SEQ(SLICE(r6, word64, 64), hi1)");
    }

    [Test]
    public void Ps2EeRw_mflo1()
    {
        Given_HexString("12380070");
        AssertCode(     // mflo1	r7
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r7 = SEQ(SLICE(r7, word64, 64), lo1)");
    }

    [Test]
    public void Ps2EeRw_mfsa_mtsa()
    {
        AssertCode(W(0, rd: 2, funct: 0x28),
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = sa",
            "2|L--|r2 = CONVERT(v5, word64, int128)");
        AssertCode(W(0, rs: 4, funct: 0x29),
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v3 = SLICE(r4, word64, 0)",
            "2|L--|sa = v3");
    }

    [Test]
    public void Ps2EeRw_movz()
    {
        AssertCode(W(0, rs: 4, rt: 5, rd: 2, funct: 0x0A),
            "0|L--|00100000(4): 2 instructions",
            "1|T--|if (SLICE(r5, word32, 0) != 0<32>) branch 00100004",
            "2|L--|r2 = r4");
    }

    [Test]
    public void Ps2EeRw_mtlo1()
    {
        Given_HexString("13004070");
        AssertCode(     // mtlo1	r0
            "0|L--|00100000(4): 1 instructions",
            "1|L--|lo1 = SLICE(0<128>, word64, 0)");
    }

    [Test]
    public void Ps2EeRw_mult()
    {
        AssertCode(W(0, rs: 4, rt: 5, funct: 0x18),
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = r4 *s64 r5",
                "2|L--|lo = CONVERT(SLICE(v5, ui32, 0), ui32, int64)",
                "3|L--|hi = CONVERT(SLICE(v5, ui32, 32), ui32, int64)");
    }

    [Test]
    public void Ps2EeRw_multu1()
    {
        Given_HexString("19006570");
        AssertCode(     // multu1	r0,r3,r5
            "0|L--|00100000(4): 3 instructions",
            "1|L--|v5 = r0 *u64 r3",
            "2|L--|lo1 = CONVERT(SLICE(v5, ui32, 0), ui32, int64)",
            "3|L--|hi1 = CONVERT(SLICE(v5, ui32, 32), ui32, int64)");
    }

    [Test]
    public void Ps2EeRw_mult1_second_accumulator()
    {
        AssertCode(MmiW(4, 5, 6, 0, 0x18),
            "0|L--|00100000(4): 5 instructions",
            "1|L--|v6 = r4 *s64 r5",
            "2|L--|r6 = CONVERT(v6, int64, int128)",
            "3|L--|v7 = r6 *s64 r4",
            "4|L--|lo1 = CONVERT(SLICE(v7, ui32, 0), ui32, int64)",
            "5|L--|hi1 = CONVERT(SLICE(v7, ui32, 32), ui32, int64)");
    }

    [Test]
    public void Ps2EeRw_pand_pxor_are_bitwise()
    {
        // pand q2,q4,q5: opcode 0x1C funct 0x09 sa 0x12.
        AssertCode(MmiW(4, 5, 2, 0x12, 0x09),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = __pand(r4, r5)");
        AssertCode(MmiW(4, 5, 2, 0x13, 0x09),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = __pxor(r4, r5)");
    }

    [Test]
    public void Ps2EeRw_paddw_intrinsic()
    {
        AssertCode(MmiW(4, 5, 2, 0x00, 0x08),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = __simd_add<word32[4]>(r4, r5)");
    }

    [Test]
    public void Ps2EeRw_psrlh_shift()
    {
        AssertCode(MmiW(0, 3, 2, 4, 0x36),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = __simd_shr<word16[8],byte>(r3, 4<8>)");
    }

    [Test]
    public void Ps2EeRw_pcpyh()
    {
        Given_HexString("E91E0870");
        AssertCode(     // pcpyh	r3,r8
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r3 = __p_copy_halfword(r8)");
    }

    [Test]
    public void Ps2EeRw_pcpyld()
    {
        Given_HexString("89434270");
        AssertCode(     // pcpyld	r8,r2,r2
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r8 = __p_copy_lower_dword(r2, r2)");
    }

    [Test]
    public void Ps2EeRw_pcpyud()
    {
        Given_HexString("A9530771");
        AssertCode(     // pcpyud	r10,r8,r7
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r10 = __p_copy_upper_dword(r8, r7)");
    }

    [Test]
    public void Ps2EeRw_pnor()
    {
        Given_HexString("E91C0270");
        AssertCode(     // pnor	r3,r0,r2
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r3 = __pnor(0<128>, r2)");
    }

    [Test]
    public void Ps2EeRw_psubb()
    {
        Given_HexString("48126870");
        AssertCode(     // psubb	r2,r3,r8
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = __simd_sub<byte[16]>(r3, r8)");
    }

    [Test]
    public void Ps2EeRw_psubw()
    {
        Given_HexString("48384370");
        AssertCode(     // psubw	r7,r2,r3
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r7 = __simd_sub<word32[4]>(r2, r3)");
    }

    [Test]
    public void Ps2EeRw_pxor()
    {
        Given_HexString("C9444370");
        AssertCode(     // pxor	r8,r2,r3
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r8 = __pxor(r2, r3)");
    }

    [Test]
    public void Ps2EeRw_qfsrv_uses_sa()
    {
        AssertCode(MmiW(4, 5, 2, 0x1B, 0x28),
            "0|L--|00100000(4): 3 instructions",
            "1|L--|v4 = SEQ(r4, r5)",
            "2|L--|v4 = v4 >>u sa",
            "3|L--|r2 = SLICE(v4, word128, 0)");
    }

    [Test]
    public void Ps2EeRw_qmfc2_qmtc2()
    {
        AssertCode(Cop2W(1, rt: 2, rd: 1),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = vf1");
        AssertCode(Cop2W(5, rt: 2, rd: 3),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|vf3 = r2");
    }


    [Test]
    public void Ps2EeRw_slt()
    {
        AssertCode(W(0, rs: 4, rt: 5, rd: 2, funct: 0x2A),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = CONVERT(r4 < r5, bool, word128)");
    }

    [Test]
    public void Ps2EeRw_sll()
    {
        AssertCode(W(0, rt: 3, rd: 2, sa: 4),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r3 << 4<8>");
    }


    [Test]
    public void Ps2EeRw_sq()
    {
        Given_HexString("0000407C");
        AssertCode(     // sq	r0,0000(r2)
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v4 = SLICE(r2, ptr32, 0)",
            "2|L--|Mem0[v4:word128] = 0<128>");
    }

    [Test]
    public void Ps2EeRw_vadd_vector()
    {
        // VADD.xyzw vf1,vf2,vf3
        AssertCode(Cop2W(0x1F, rt: 3, rd: 2, sa: 1, funct: 0x28),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|vf1 = __vadd(vf2, vf3)");
    }

    [Test]
    public void Ps2EeRw_vmulax_accumulates_into_ACC()
    {
        // VMULAx.xyzw ACC,vf1,vf2 (the classic dot-product idiom)
        AssertCode(Cop2W(0x1F, rt: 2, rd: 1, sa: 6, funct: 0x3C),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|acc = __vmulax(vf1, vf2)");
    }

    [Test]
    public void Ps2EeRw_vmove_unary()
    {
        AssertCode(Cop2W(0x1F, rt: 1, rd: 2, sa: 12, funct: 0x3C),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|vf1 = vf2");
    }

    [Test]
    public void Ps2EeRw_viadd_integer_pipe()
    {
        AssertCode(Cop2W(0x10, rt: 5, rd: 4, sa: 3, funct: 0x30),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|vi3 = vi4 + vi5");
    }

    [Test]
    public void Ps2EeRw_vdiv_writes_Q()
    {
        AssertCode(Cop2W(0x10, rt: 2, rd: 1, sa: 14, funct: 0x3C),
            "0|L--|00100000(4): 1 instructions",
            "1|L--|q = __vdiv(vf1, vf2)");
    }

    [Test]
    public void Ps2EeRw_real_world_dot_product()
    {
        Given_UInt32s(0xD8590030u, 0x4BF681BCu, 0x4BF688BDu, 0x4BF790BEu);
        base.AssertCode(
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v4 = SLICE(r2, ptr32, 0)",
            "2|L--|vf25 = Mem0[v4 + 48<i32>:word128]",
            "3|L--|00100004(4): 1 instructions",
            "4|L--|acc = __vmulax(vf16, vf22)",
            "5|L--|00100008(4): 1 instructions",
            "6|L--|acc = __vmadday(acc, vf17, vf22)",
            "7|L--|0010000C(4): 1 instructions",
            "8|L--|acc = __vmaddaz(acc, vf18, vf23)");
    }

}
