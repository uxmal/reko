#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.MN103;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.UnitTests.Arch.MN103
{
    [TestFixture]
    public class MN103RewriterTests : RewriterTestBase
    {
        private readonly MN103Architecture arch;
        private readonly Address addr;

        public MN103RewriterTests()
        {
            this.arch = new MN103Architecture(CreateServiceContainer(), "mn103", new(), new(), new());
            this.addr = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;


        [Test]
        public void Mn103Rw_add_imm()
        {
            Given_HexString("23A1");
            AssertCode(     // add	A1,a3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|a3 = a3 + 0xFFFFFFA1<32>",
                "2|L--|VCNZ = cond(a3)");
        }

        [Test]
        public void Mn103Rw_add_reg_reg()
        {
            Given_HexString("EE");
            AssertCode(     // add	d3,d2
                "0|L--|00100000(1): 2 instructions",
                "1|L--|d2 = d2 + d3",
                "2|L--|VCNZ = cond(d2)");
        }

        [Test]
        public void Mn103Rw_addc()
        {
            Given_HexString("F144");
            AssertCode(     // addc	d1,d0
                "0|L--|00100000(2): 2 instructions",
                "1|L--|d0 = d0 + d1 + C",
                "2|L--|VCNZ = cond(d0)");
        }


        [Test]
        public void Mn103Rw_and()
        {
            Given_HexString("F20B");
            AssertCode(     // and	d2,d3
                "0|L--|00100000(2): 3 instructions",
                "1|L--|d3 = d3 & d2",
                "2|L--|NZ = cond(d3)",
                "3|L--|VC = 0<16>");
        }

        [Test]
        public void Mn103Rw_asl2()
        {
            Given_HexString("57");
            AssertCode(     // asl2	d3
                "0|L--|00100000(1): 1 instructions",
                "1|L--|d3 = d3 << 2<8>");
        }

        [Test]
        public void Mn103Rw_asr()
        {
            Given_HexString("F2BE");
            AssertCode(     // asr	d3,d2
                "0|L--|00100000(2): 2 instructions",
                "1|L--|d2 = d2 >> d3",
                "2|L--|CNZ = cond(d2)");
        }

        [Test]
        public void Mn103Rw_bcc()
        {
            Given_HexString("C6D2");
            AssertCode(     // bcc	000FFFFA
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 000FFFD2");
        }

        [Test]
        public void Mn103Rw_blt()
        {
            Given_HexString("C01C");
            AssertCode(     // blt	00101B3A
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(LT,VN)) branch 0010001C");
        }

        [Test]
        public void Mn103Rw_bra()
        {
            Given_HexString("CA6B");
            AssertCode(     // bra	001002F9
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto 0010006B");
        }

        [Test]
        public void Mn103Rw_bclr()
        {
            Given_HexString("F09C");
            AssertCode(     // bclr	d3,(a0)
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v5 = Mem0[a0:byte]",
                "2|L--|Z = cond(CONVERT(v5, byte, word32) & d3)",
                "3|L--|VCN = 0<16>",
                "4|L--|Mem0[a0:byte] = ~v5");
        }

        [Test]
        public void Mn103Rw_bset()
        {
            Given_HexString("F08C");
            AssertCode(     // bset	d3,(a0)
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v5 = Mem0[a0:byte]",
                "2|L--|Z = cond(CONVERT(v5, byte, word32) & d3)",
                "3|L--|VCN = 0<16>",
                "4|L--|Mem0[a0:byte] = v5 | d3");
        }

        [Test]
        public void Mn103Rw_btst()
        {
            Given_HexString("F8EECA");
            AssertCode(     // btst	-36,d2
                "0|L--|00100000(3): 2 instructions",
                "1|L--|NZ = d2 & 0xFFFFFFCA<32>",
                "2|L--|VC = 0<16>");
        }

        [Test]
        public void Mn103Rw_call()
        {
            Given_HexString("CD1A03E014");
            AssertCode(     // call f,[d2,d3,a2],+14
                "0|T--|00100000(5): 9 instructions",
                "1|L--|v4 = sp",
                "2|L--|v4 = v4 - 4<i32>",
                "3|L--|Mem0[v4:word32] = d2",
                "4|L--|v4 = v4 - 4<i32>",
                "5|L--|Mem0[v4:word32] = d3",
                "6|L--|v4 = v4 - 4<i32>",
                "7|L--|Mem0[v4:word32] = a2",
                "8|L--|sp = sp - 0x14<32>",
                "9|T--|call 0010031A (0)");
        }

        [Test]
        public void Mn103Rw_calls()
        {
            Given_HexString("F0F2");
            AssertCode(     // calls	(a2)
                "0|L--|00100000(2): 2 instructions",
                "1|L--|mdr = 00100002",
                "2|T--|call Mem0[a2:word32] (4)");
        }

        [Test]
        public void Mn103Rw_clr_d0()
        {
            Given_HexString("00");
            AssertCode(     // clr	d0
                "0|L--|00100000(1): 3 instructions",
                "1|L--|d0 = 0<32>",
                "2|L--|VCN = 0<16>",
                "3|L--|Z = 1<16>");
        }

        [Test]
        public void Mn103Rw_cmp_imm()
        {
            Given_HexString("BA55");
            AssertCode(     // cmp	55,a2
                "0|L--|00100000(2): 1 instructions",
                "1|L--|VCNZ = cond(a2 - 0x55<32>)");
        }

        [Test]
        public void Mn103Rw_cmp_reg()
        {
            Given_HexString("B8");
            AssertCode(     // cmp	a2,a0
                "0|L--|00100000(1): 1 instructions",
                "1|L--|VCNZ = cond(a0 - a2)");
        }

        [Test]
        public void Mn103Rw_divu()
        {
            Given_HexString("F274");
            AssertCode(     // divu	d1,d0
                "0|L--|00100000(2): 3 instructions",
                "1|L--|d0 = mdr_d0 /u d1",
                "2|L--|d1 = mdr_d0 %u d1",
                "3|L--|VCNZ = cond(d0)");
        }


        [Test]
        public void Mn103Rw_extb()
        {
            Given_HexString("13");
            AssertCode(     // extb	d3
                "0|L--|00100000(1): 2 instructions",
                "1|L--|v4 = SLICE(d3, byte, 0)",
                "2|L--|d3 = CONVERT(v4, byte, int32)");
        }

        [Test]
        public void Mn103Rw_extbu()
        {
            Given_HexString("14");
            AssertCode(     // extbu	d0
                "0|L--|00100000(1): 2 instructions",
                "1|L--|v4 = SLICE(d0, byte, 0)",
                "2|L--|d0 = CONVERT(v4, byte, uint32)");
        }

        [Test]
        public void Mn103Rw_exth()
        {
            Given_HexString("18");
            AssertCode(     // exth	d0
                "0|L--|00100000(1): 2 instructions",
                "1|L--|v4 = SLICE(d0, word16, 0)",
                "2|L--|d0 = CONVERT(v4, word16, int32)");
        }

        [Test]
        public void Mn103Rw_exthu()
        {
            Given_HexString("1E");
            AssertCode(     // exthu	d2
                "0|L--|00100000(1): 2 instructions",
                "1|L--|v4 = SLICE(d2, word16, 0)",
                "2|L--|d2 = CONVERT(v4, word16, uint32)");
        }

        [Test]
        public void Mn103Rw_inc()
        {
            Given_HexString("48");
            AssertCode(     // inc	d2
                "0|L--|00100000(1): 2 instructions",
                "1|L--|d2 = d2 + 1<32>",
                "2|L--|VCNZ = cond(d2)");
        }

        [Test]
        public void Mn103Rw_inc4()
        {
            Given_HexString("53");
            AssertCode(     // inc4	a3
                "0|L--|00100000(1): 1 instructions",
                "1|L--|a3 = a3 + 4<32>");
        }

        [Test]
        public void Mn103Rw_lsr()
        {
            Given_HexString("F2A2");
            AssertCode(     // lsr	d0,d2
                "0|L--|00100000(2): 2 instructions",
                "1|L--|d2 = d2 >>u d0",
                "2|L--|CNZ = cond(d2)");
        }

        [Test]
        public void Mn103Rw_mov_reg_abs()
        {
            Given_HexString("0DCF8A");
            AssertCode(     // mov	d3,00008ACF
                "0|L--|00100000(3): 1 instructions",
                "1|L--|Mem0[0x00008ACF<p32>:word32] = d3");
        }

        [Test]
        public void Mn103Rw_mov_reg_reg()
        {
            Given_HexString("83");
            AssertCode(     // mov	d0,d3
                "0|L--|00100000(1): 1 instructions",
                "1|L--|d3 = d0");
        }

        [Test]
        public void Mn103Rw_mov_mem_reg()
        {
            Given_HexString("5DF7");
            AssertCode(     // mov	(F7,sp),a1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|a1 = Mem0[sp + 247<i32>:word32]");
        }

        [Test]
        public void Mn103Rw_movbu_mem_reg()
        {
            Given_HexString("3775BF");
            AssertCode(     // movbu	(0000BF75),d3
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v3 = Mem0[0x0000BF75<p32>:byte]",
                "2|L--|d3 = CONVERT(v3, byte, uint32)");
        }

        [Test]
        public void Mn103Rw_movhu_reg_abs16()
        {
            Given_HexString("037C3F");
            AssertCode(     // movhu	d0,00003F7C
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v4 = SLICE(d0, word16, 0)",
                "2|L--|Mem0[0x00003F7C<p32>:word16] = v4");
        }

        [Test]
        [Ignore("Need more info on how to properly do this")]
        public void Mn103Rw_movm_to_regs()
        {
            Given_HexString("CEEE");
            AssertCode(     // movm	(sp),[d2,d3,a2,d0,d1,a0,a1,mdr,lir,lar]
                "0|L--|00100000(2): 20 instructions",
                "1|L--|sp = sp - 4<i32>",
                "2|L--|Mem0[sp:word32] = d2",
                "3|L--|sp = sp - 4<i32>",
                "4|L--|Mem0[sp:word32] = d3",
                "5|L--|sp = sp - 4<i32>",
                "6|L--|Mem0[sp:word32] = a2",
                "7|L--|sp = sp - 4<i32>",
                "8|L--|Mem0[sp:word32] = d0",
                "9|L--|sp = sp - 4<i32>",
                "10|L--|Mem0[sp:word32] = d1",
                "11|L--|sp = sp - 4<i32>",
                "12|L--|Mem0[sp:word32] = a0",
                "13|L--|sp = sp - 4<i32>",
                "14|L--|Mem0[sp:word32] = a1",
                "15|L--|sp = sp - 4<i32>",
                "16|L--|Mem0[sp:word32] = mdr",
                "17|L--|sp = sp - 4<i32>",
                "18|L--|Mem0[sp:word32] = CONVERT(lir, word16, uint32)",
                "19|L--|sp = sp - 4<i32>",
                "20|L--|Mem0[sp:word32] = CONVERT(lar, word16, uint32)"

);
        }

        [Test]
        public void Mn103Rw_mul()
        {
            Given_HexString("F24B");
            AssertCode(     // mul	d2,d3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|mdr_d3 = d3 *64 d2",
                "2|L--|VCNZ = mdr_d3");
        }

        [Test]
        public void Mn103Rw_mulu()
        {
            Given_HexString("F25E");
            AssertCode(     // mulu	d3,d2
                "0|L--|00100000(2): 2 instructions",
                "1|L--|mdr_d2 = d2 *64 d3",
                "2|L--|VCNZ = mdr_d2");
        }

        [Test]
        public void Mn103Rw_not()
        {
            Given_HexString("F232");
            AssertCode(     // not	d2
                "0|L--|00100000(2): 3 instructions",
                "1|L--|d2 = ~d2",
                "2|L--|NZ = cond(d2)",
                "3|L--|VC = 0<16>");
        }

        [Test]
        public void Mn103Rw_or()
        {
            Given_HexString("F21E");
            AssertCode(     // or	d3,d2
                "0|L--|00100000(2): 3 instructions",
                "1|L--|d2 = d2 | d3",
                "2|L--|NZ = cond(d2)",
                "3|L--|VC = 0<16>");
        }

        [Test]
        public void Mn103Rw_ret()
        {
            Given_HexString("DFC00C");
            AssertCode(     //ret [d2,d3],+0C
                "0|R--|00100000(3): 6 instructions",
                "1|L--|sp = sp + 0xC<32>",
                "2|L--|d3 = Mem0[sp:word32]",
                "3|L--|sp = sp + 4<i32>",
                "4|L--|d2 = Mem0[sp:word32]",
                "5|L--|sp = sp + 4<i32>",
                "6|R--|return (4,12)");
        }

        [Test]
        public void Mn103Rw_retf()
        {
            Given_HexString("DED708");
            AssertCode(     // retf	[d2,d3,a3]
                "0|R--|00100000(3): 9 instructions",
                "1|L--|v3 = mdr",
                "2|L--|sp = sp + 8<32>",
                "3|L--|a3 = Mem0[sp:word32]",
                "4|L--|sp = sp + 4<i32>",
                "5|L--|d3 = Mem0[sp:word32]",
                "6|L--|sp = sp + 4<i32>",
                "7|L--|d2 = Mem0[sp:word32]",
                "8|L--|sp = sp + 4<i32>",
                "9|T--|goto v3");
        }

        [Test]
        public void Mn103Rw_rol()
        {
            Given_HexString("F282");
            AssertCode(     // rol	d2
                "0|L--|00100000(2): 3 instructions",
                "1|L--|d2 = __rcl<word32,byte>(d2, 1<8>, C)",
                "2|L--|CNZ = cond(d2)",
                "3|L--|V = 0<16>");
        }

        [Test]
        public void Mn103Rw_rti()
        {
            Given_HexString("F0FD");
            AssertCode(     // rti
                "0|R--|00100000(2): 2 instructions",
                "1|L--|__return_from_interrupt()",
                "2|R--|return (0,0)");
        }


        [Test]
        public void Mn103Rw_setlb()
        {
            Given_HexString("DB");
            AssertCode(     // setlb
                "0|L--|00100000(1): 2 instructions",
                "1|L--|lir = Mem0[0x00100001<p32>:word32]",
                "2|L--|lar = 00100001");
        }

        [Test]
        public void Mn103Rw_sub()
        {
            Given_HexString("F13D");
            AssertCode(     // sub	a3,a1
                "0|L--|00100000(2): 2 instructions",
                "1|L--|a1 = a1 - a3",
                "2|L--|VCNZ = cond(a1)");
        }

        [Test]
        public void Mn103Rw_subc()
        {
            Given_HexString("F18D");
            AssertCode(     // subc	d3,d1
                "0|L--|00100000(2): 2 instructions",
                "1|L--|d1 = d1 - d3 - C",
                "2|L--|VCNZ = cond(d1)");
        }

        [Test]
        public void Mn103Rw_trap()
        {
            Given_HexString("F0FE");
            AssertCode(     // trap
                "0|T--|00100000(2): 1 instructions",
                "1|L--|__syscall()");
        }

        [Test]
        public void Mn103Rw_xor()
        {
            Given_HexString("FAE867C1");
            AssertCode(     // xor	-3E99,a0
                "0|L--|00100000(4): 3 instructions",
                "1|L--|a0 = a0 ^ 0xFFFFC167<32>",
                "2|L--|NZ = a0",
                "3|L--|VC = 0<16>");
        }


        [Test]
        public void Mn103Rw_asl()
        {
            Given_HexString("F291");
            AssertCode(     // asl	d0,d1
                "0|L--|00100000(2): 2 instructions",
                "1|L--|d1 = d1 << d0",
                "2|L--|CNZ = cond(d1)");
        }

        [Test]
        public void Mn103Rw_bns()
        {
            Given_HexString("F8EB47");
            AssertCode(     // bns	00100D0C
                "0|L--|00100000(3): 1 instructions",
                "1|T--|if (Test(LT,N)) branch 00100047");
        }


        [Test]
        public void Mn103Rw_ext()
        {
            Given_HexString("F2D2");
            AssertCode(     // ext	d2
                "0|L--|00100000(2): 1 instructions",
                "1|L--|mdr_d2 = CONVERT(d2, word32, int64)");
        }

        [Test]
        public void Mn103Rw_rets()
        {
            Given_HexString("F0FC");
            AssertCode(     // rets
                "0|R--|00100000(2): 1 instructions",
                "1|R--|return (0,0)");
        }
    }
}
