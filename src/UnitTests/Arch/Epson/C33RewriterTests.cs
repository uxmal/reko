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
using Reko.Arch.Epson;
using Reko.Core;
using Reko.Core.Memory;
using System;

namespace Reko.UnitTests.Arch.Epson;

public class C33RewriterTests : RewriterTestBase
{
    private readonly C33Architecture arch;
    private readonly Address addr;

    public C33RewriterTests()
    {
        this.arch = new C33Architecture(CreateServiceContainer(), "c33", []);
        this.addr = Address.Ptr32(0x0010_0000);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;

    [Test]
    public void C33Rw_Gen()
    {
        var rnd = new Random(0x42211);
        var bytes = new byte[1600000];
        rnd.NextBytes(bytes);
        var mem = new ByteMemoryArea(addr, bytes);
        var rdr = mem.CreateLeReader(0);
        var rw = arch.CreateRewriter(
            rdr,
            arch.CreateProcessorState(),
            new StorageBinder(),
            CreateRewriterHost());
        foreach (var cluster in rw)
        {
            rw.ToString();
        }
    }

    [Test]
    public void C33Rw_add_imm()
    {
        Given_HexString("2963");
        AssertCode(     // add	%r9,0x32
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r9 = r9 + 0x32<32>",
            "2|L--|CNVZ = cond(r9)");
    }

    [Test]
    public void C33Rw_add_reg()
    {
        Given_HexString("E922");
        AssertCode(     // add	%r9,%r14
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r9 = r9 + r14",
            "2|L--|CNVZ = cond(r9)");
    }

    [Test]
    public void C33Rw_add_reg_ext()
    {
        Given_HexString("01C0 E922");
        AssertCode(     // add	%r9,%r14,0x1
            "0|L--|00100000(4): 2 instructions",
            "1|L--|r9 = r14 + 1<32>",
            "2|L--|CNVZ = cond(r9)");
    }

    [Test]
    public void C33Rw_add_sp()
    {
        Given_HexString("AAA2");
        AssertCode(     // add	%sp,0x2AA
            "0|L--|00100000(2): 1 instructions",
            "1|L--|sp = sp + 0xAA8<32>");
    }

    [Test]
    public void C33Rw_and_reg()
    {
        Given_HexString("5932");
        AssertCode(     // and	%r9,%r5
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r9 = r9 & r5",
            "2|L--|NZ = cond(r9)");
    }

    [Test]
    public void C33Rw_and_reg_ext()
    {
        Given_HexString("00C4 5932");
        AssertCode(     // and	%r9,%r5
            "0|L--|00100000(4): 2 instructions",
            "1|L--|r9 = r5 & 0x400<32>",
            "2|L--|NZ = cond(r9)");
    }

    [Test]
    public void C33Rw_brk()
    {
        Given_HexString("0004");
        AssertCode(     // brk
            "0|H--|00100000(2): 1 instructions",
            "1|L--|__brk()");
    }

    [Test]
    public void C33Rw_call()
    {
        Given_HexString("9E1C");
        AssertCode(     // call	00100CFA
            "0|T--|00100000(2): 1 instructions",
            "1|T--|call 000FFF3C (4)");
    }

    [Test]
    public void C33Rw_call_d()
    {
        Given_HexString("881D");
        AssertCode(     // call.d	0010011E
            "0|TD-|00100000(2): 1 instructions",
            "1|TD-|call 000FFF10 (4)");
    }

    [Test]
    public void C33Rw_cmp_imm()
    {
        Given_HexString("AE6A");
        AssertCode(     // cmp	%r14,0x2A
            "0|L--|00100000(2): 1 instructions",
            "1|L--|CNVZ = r14 - 0xFFFFFFEA<32>");
    }

    [Test]
    public void C33Rw_cmp_reg()
    {
        Given_HexString("0C2A");
        AssertCode(     // cmp	%r12,%r0
            "0|L--|00100000(2): 1 instructions",
            "1|L--|CNVZ = r12 - r0");
    }

    [Test]
    public void C33Rw_halt()
    {
        Given_HexString("8000");
        AssertCode(     // halt
            "0|H--|00100000(2): 1 instructions",
            "1|L--|__halt()");
    }

    [Test]
    public void C33Rw_int()
    {
        Given_HexString("8004");
        AssertCode(     // int	0x0
            "0|T--|00100000(2): 1 instructions",
            "1|L--|__syscall<word32>(0<32>)");
    }

    [Test]
    public void C33Rw_jp()
    {
        Given_HexString("871E");
        AssertCode(     // jp
            "0|T--|00100000(2): 1 instructions",
            "1|T--|goto 000FFF0E");
    }

    [Test]
    public void C33Rw_jp_d()
    {
        Given_HexString("4B1F");
        AssertCode(     // jp.d
            "0|TD-|00100000(2): 1 instructions",
            "1|TD-|goto 00100096");
    }

    [Test]
    public void C33Rw_jpr()
    {
        Given_HexString("CD02");
        AssertCode(     // jpr	%r13
            "0|T--|00100000(2): 1 instructions",
            "1|T--|goto 0x00100000<p32> + r13");
    }

    [Test]
    public void C33Rw_jrne()
    {
        Given_HexString("961A");
        AssertCode(     // jrne	001000FE
            "0|T--|00100000(2): 1 instructions",
            "1|T--|if (Test(NE,Z)) branch 000FFF2C");
    }

    [Test]
    public void C33Rw_jruge_d()
    {
        Given_HexString("6013");
        AssertCode(     // jruge.d	00100964
            "0|TD-|00100000(2): 1 instructions",
            "1|TD-|if (Test(UGE,C)) branch 001000C0");
    }

    [Test]
    public void C33Rw_jrugt_d()
    {
        Given_HexString("0211");
        AssertCode(     // jrugt.d	001000BA
            "0|TD-|00100000(2): 1 instructions",
            "1|TD-|if (Test(UGT,CZ)) branch 00100004");
    }

    [Test]
    public void C33Rw_ld_b()
    {
        Given_HexString("E9C4B841");
        AssertCode(     // ld.b	%r8,[%sp+0x13A5B]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r8 = Mem0[sp + 0x13A5B<u32>:int8]");
    }

    [Test]
    public void C33Rw_ld_cf()
    {
        Given_HexString("D001");
        AssertCode(     // ld.cf	%psr
            "0|L--|00100000(2): 1 instructions",
            "1|L--|psr = __load_coprocessor_flags()");
    }

    [Test]
    public void C33Rw_ld_h()
    {
        Given_HexString("FFD18F4B");
        AssertCode(     // ld.h	%r15,[%sp+0x47FF8]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = Mem0[sp + 0x47FF8<u32>:int16]");
    }

    [Test]
    public void C33Rw_ld_ub_sp()
    {
        Given_HexString("EE46");
        AssertCode(     // ld.ub	%r14,[%sp+0x2E]
            "0|L--|00100000(2): 1 instructions",
            "1|L--|r14 = Mem0[sp + 0x2E<u32>:byte]");
    }

    [Test]
    public void C33Rw_ld_ub_post()
    {
        Given_HexString("B025");
        AssertCode(     // ld.ub	%r0,[%r11]+
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r0 = Mem0[r11:byte]",
            "2|L--|r11 = r11 + 1<i32>");
    }

    [Test]
    public void C33Rw_ld_uh_sp()
    {
        Given_HexString("A54D");
        AssertCode(     // ld.uh	%r5,[%sp+0x34]
            "0|L--|00100000(2): 1 instructions",
            "1|L--|r5 = Mem0[sp + 0x34<u32>:word16]");
    }

    [Test]
    public void C33Rw_ld_uh_post()
    {
        Given_HexString("AF2D");
        AssertCode(     // ld.uh	%r15,[%r10]+
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r15 = Mem0[r10:word16]",
            "2|L--|r10 = r10 + 2<i32>");
    }

    [Test]
    public void C33Rw_ld_w_move_imm()
    {
        Given_HexString("766F");
        AssertCode(     // ld.w	%r6,0x37
            "0|L--|00100000(2): 1 instructions",
            "1|L--|r6 = 0xFFFFFFF7<32>");
    }

    [Test]
    public void C33Rw_ld_w_move_reg()
    {
        Given_HexString("292E");
        AssertCode(     // ld.w	%r9,%r2
            "0|L--|00100000(2): 1 instructions",
            "1|L--|r9 = r2");
    }

    [Test]
    public void C33Rw_ld_w_post()
    {
        Given_HexString("643D");
        AssertCode(     // ld.w	[%r6]+,%r4
            "0|L--|00100000(2): 2 instructions",
            "1|L--|Mem0[r6:word32] = r4",
            "2|L--|r6 = r6 + 4<i32>");
    }

    [Test]
    public void C33Rw_not_imm()
    {
        Given_HexString("F47D");
        AssertCode(     // not	%r4,0x1F
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r4 = ~0x1F<32>",
            "2|L--|NZ = cond(r4)");
    }

    [Test]
    public void C33Rw_not_reg()
    {
        Given_HexString("ED3E");
        AssertCode(     // not	%r13,%r14
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r13 = ~r14",
            "2|L--|NZ = cond(r13)");
    }

    [Test]
    public void C33Rw_or_imm()
    {
        Given_HexString("9575");
        AssertCode(     // or	%r5,0x19
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r5 = r5 | 0x19<32>",
            "2|L--|NZ = cond(r5)");
        }

    [Test]
    public void C33Rw_or_reg()
    {
        Given_HexString("2736");
        AssertCode(     // or	%r7,%r2
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r7 = r7 | r2",
            "2|L--|NZ = cond(r7)");
    }

    [Test]
    public void C33Rw_pop()
    {
        Given_HexString("5000");
        AssertCode(     // pop	%r0
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r0 = Mem0[sp:word32]",
            "2|L--|sp = sp + 4<i32>");
    }

    [Test]
    public void C33Rw_popn()
    {
        Given_HexString("4202");
        AssertCode(     // popn	%r2
            "0|L--|00100000(2): 4 instructions",
            "1|L--|r0 = Mem0[sp:word32]",
            "2|L--|r1 = Mem0[sp + 4<i32>:word32]",
            "3|L--|r2 = Mem0[sp + 8<i32>:word32]",
            "4|L--|sp = sp + 12<i32>");
    }

    [Test]
    public void C33Rw_pops()
    {
        Given_HexString("D300");
        AssertCode(     // pops	%ttbr
            "0|L--|00100000(2): 2 instructions",
            "1|L--|ahr_alr = Mem0[sp:word64]",
            "2|L--|sp = sp + 8<i32>");
    }

    [Test]
    public void C33Rw_push()
    {
        Given_HexString("1200");
        AssertCode(     // push	%r2
            "0|L--|00100000(2): 2 instructions",
            "1|L--|sp = sp - 4<i32>",
            "2|L--|Mem0[sp:word32] = r2");
    }

    [Test]
    public void C33Rw_pushn()
    {
        Given_HexString("0302");
        AssertCode(     // pushn	%r3
            "0|L--|00100000(2): 5 instructions",
            "1|L--|Mem0[sp - 4<i32>:word32] = r3",
            "2|L--|Mem0[sp - 8<i32>:word32] = r2",
            "3|L--|Mem0[sp - 12<i32>:word32] = r1",
            "4|L--|Mem0[sp - 16<i32>:word32] = r0",
            "5|L--|sp = sp - 16<i32>");
    }

    [Test]
    public void C33Rw_pushs()
    {
        Given_HexString("9300");
        AssertCode(     // pushs	%idir
            "0|L--|00100000(2): 2 instructions",
            "1|L--|sp = sp - 8<i32>",
            "2|L--|Mem0[sp:word64] = ahr_alr");
    }

    [Test]
    public void C33Rw_ret_d()
    {
        Given_HexString("4507");
        AssertCode(     // ret.d
            "0|RD-|00100000(2): 1 instructions",
            "1|RD-|return (4,0)");
    }

    [Test]
    public void C33Rw_retd()
    {
        Given_HexString("4004");
        AssertCode(     // retd
            "0|R--|00100000(2): 2 instructions",
            "1|L--|__return_from_debug_exception()",
            "2|R--|return (4,0)");
    }

    [Test]
    public void C33Rw_reti()
    {
        Given_HexString("C004");
        AssertCode(     // reti
            "0|R--|00100000(2): 2 instructions",
            "1|L--|__return_from_interrupt()",
            "2|R--|return (4,0)");
    }

    [Test]
    public void C33Rw_rl_reg()
    {
        Given_HexString("4ABD");
        AssertCode(     // rl	%r10,%r4
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r10 = __rol<word32,word32>(r10, r4)",
            "2|L--|NZ = cond(r10)");
    }

    [Test]
    public void C33Rw_rr_reg()
    {
        Given_HexString("75B9");
        AssertCode(     // rr	%r5,%r7
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r5 = __ror<word32,word32>(r5, r7)",
            "2|L--|NZ = cond(r5)");
    }

    [Test]
    public void C33Rw_sla_imm()
    {
        Given_HexString("A72F");
        AssertCode(     // sla	%r7,0xA
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r7 = r7 << 0xA<32>",
            "2|L--|NZ = cond(r7)");
    }

    [Test]
    public void C33Rw_sll()
    {
        Given_HexString("7BAD");
        AssertCode(     // sll	%r11,%r7
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r11 = r11 << r7",
            "2|L--|NZ = cond(r11)");
    }

    [Test]
    public void C33Rw_slp()
    {
        Given_HexString("4000");
        AssertCode(     // slp
            "0|L--|00100000(2): 1 instructions",
            "1|L--|__sleep()");
    }

    [Test]
    public void C33Rw_srl()
    {
        Given_HexString("B123");
        AssertCode(     // srl	%r1,0xB
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r1 = r1 >>u 0xB<32>",
            "2|L--|NZ = cond(r1)");
    }

    [Test]
    public void C33Rw_sub_reg()
    {
        Given_HexString("7C26");
        AssertCode(     // sub	%r12,%r7
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r12 = r12 - r7",
            "2|L--|CNVZ = cond(r12)");
    }

    [Test]
    public void C33Rw_sub_sp()
    {
        Given_HexString("6FA7");
        AssertCode(     // sub	%sp,0x36F
            "0|L--|00100000(2): 1 instructions",
            "1|L--|sp = sp - 0xDBC<32>");
    }

    [Test]
    public void C33Rw_swap()
    {
        Given_HexString("6BB2");
        AssertCode(     // swap	%r11,%r6
            "0|L--|00100000(2): 1 instructions",
            "1|L--|r11 = __swap(r6)");
    }

    [Test]
    public void C33Rw_swaph()
    {
        Given_HexString("A7BA");
        AssertCode(     // swaph	%r7,%r10
            "0|L--|00100000(2): 1 instructions",
            "1|L--|r7 = __swaph(r10)");
    }

    [Test]
    public void C33Rw_xor()
    {
        Given_HexString("387A");
        AssertCode(     // xor	%r8,0xFFFFFFE3
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r8 = r8 ^ 0xFFFFFFE3<32>",
            "2|L--|NZ = cond(r8)");
    }

    [Test]
    public void C33Rw_xor_reg()
    {
        Given_HexString("9F3A");
        AssertCode(     // xor	%r15,%r9
            "0|L--|00100000(2): 2 instructions",
            "1|L--|r15 = r15 ^ r9",
            "2|L--|NZ = cond(r15)");
    }
}
