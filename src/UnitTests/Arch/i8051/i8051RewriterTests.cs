#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.i8051;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.i8051
{
    [TestFixture]
    public class i8051RewriterTests : RewriterTestBase
    {
        private readonly i8051Architecture arch;

        public i8051RewriterTests()
        {
            this.arch = new i8051Architecture(CreateServiceContainer(), "8051", new Dictionary<string, object>());
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr16(0);


        [Test]
        public void I8051_rw_nop()
        {
            Given_Bytes(0); // nop
            AssertCode(
                "0|L--|0000(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void I8051_rw_ajmp()
        {
            Given_Bytes(0xE1, 0xFF); // ajmp\t07FF
            AssertCode(
                "0|T--|0000(2): 1 instructions",
                "1|T--|goto 07FF");
        }

        [Test]
        public void I8051_rw_cpl_C() {
            Given_Bytes(0xB3);  //  cpl C
            AssertCode(
                "0|L--|0000(1): 1 instructions",
                "1|L--|C = ~C");
        }

        [Test]
        public void I8051_rw_ljmp()
        {
            Given_Bytes(0x02, 0x12, 0x34); // ljmp\t1234
            AssertCode(
                "0|T--|0000(3): 1 instructions",
                "1|T--|goto 1234");
        }

        [Test]
        public void I8051_rw_jbc()
        {
            Given_Bytes(0x10, 0x82, 0x0D); // jbc\tP0.2,0010
            AssertCode(
                "0|T--|0000(3): 3 instructions",
                "1|T--|if ((P0 >>u 2<8> & 1<8>) == 0<8>) branch 0003",
                "2|L--|P0 = P0 & 0xFB<8> | 0<8> << 2<8>",
                "3|T--|goto 0010");
        }

        [Test]
        public void I8051_rw_inc()
        {
            Given_Bytes(0x08); // inc\tR0
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|R0 = R0 + 1<8>",
                "2|L--|P = cond(R0)");

            Given_Bytes(0x04); // inc\tA
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|A = A + 1<8>",
                "2|L--|P = cond(A)");

            Given_Bytes(0x05, 0x88); // inc\t[0088]
            AssertCode(
                "0|L--|0000(2): 1 instructions",
                "1|L--|Mem0[__data:0x0088<p16>:byte] = Mem0[__data:0x0088<p16>:byte] + 1<8>");
        }

        [Test]
        public void I8051_rw_add()
        {
            Given_Bytes(0x24, 02); // add\tA,02
            AssertCode(
                "0|L--|0000(2): 2 instructions",
                "1|L--|A = A + 2<8>",
                "2|L--|CAOP = cond(A)");

            Given_Bytes(0x25, 0x25); // add\tA,[0025]
            AssertCode(
                "0|L--|0000(2): 2 instructions",
                "1|L--|A = A + Mem0[__data:0x0025<p16>:byte]",
                "2|L--|CAOP = cond(A)");

            Given_Bytes(0x27); // add\tA,@R1
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|A = A + Mem0[__data:CONVERT(R1, byte, word16):byte]",
                "2|L--|CAOP = cond(A)");

            Given_Bytes(0x2B); // add\tA,R3
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|A = A + R3",
                "2|L--|CAOP = cond(A)");
        }

        [Test]
        public void I8051_rw_anl()
        {
            Given_Bytes(0xB0, 0x93); // anl\tC,/P1.3
            AssertCode(
                "0|L--|0000(2): 1 instructions",
                "1|L--|C = C & !(P1 >>u 3<8> & 1<8>)");

            Given_Bytes(0x56); // anl\tA,@R0
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|A = A & Mem0[__data:CONVERT(R0, byte, word16):byte]",
                "2|L--|P = cond(A)");
        }

        [Test]
        public void I8051_rw_mov()
        {
            Given_Bytes(0x75, 0x42, 0x1); // mov\t[0042],01
            AssertCode(
                "0|L--|0000(3): 1 instructions",
                "1|L--|Mem0[__data:0x0042<p16>:byte] = 1<8>");

            Given_Bytes(0x79, 0x42); // mov\tR1,42
            AssertCode(
                "0|L--|0000(2): 1 instructions",
                "1|L--|R1 = 0x42<8>");

            Given_Bytes(0xA2, 0x84); // mov\tC,P0.4
            AssertCode(
                "0|L--|0000(2): 1 instructions",
                "1|L--|C = P0 >>u 4<8> & 1<8>");

            Given_Bytes(0x85, 0x90, 0x80); // mov\t[0090],[0080]
            AssertCode(
                "0|L--|0000(3): 1 instructions",
                "1|L--|Mem0[__data:0x0090<p16>:byte] = Mem0[__data:0x0080<p16>:byte]");

            Given_Bytes(0x8A, 0x42); // mov\t[0042],R2
            AssertCode(
                "0|L--|0000(2): 1 instructions",
                "1|L--|Mem0[__data:0x0042<p16>:byte] = R2");

            Given_Bytes(0xAA, 0x42); // mov\tR2,[0042]
            AssertCode(
                "0|L--|0000(2): 1 instructions",
                "1|L--|R2 = Mem0[__data:0x0042<p16>:byte]");

            Given_Bytes(0xE5, 0x42); // mov\tA,[0042]
            AssertCode(
                "0|L--|0000(2): 1 instructions",
                "1|L--|A = Mem0[__data:0x0042<p16>:byte]");

            Given_Bytes(0xEC); // mov\tA,R4
            AssertCode(
                "0|L--|0000(1): 1 instructions",
                "1|L--|A = R4");

            Given_Bytes(0xF5, 0x42); // mov\t[0042],A
            AssertCode(
                "0|L--|0000(2): 1 instructions",
                "1|L--|Mem0[__data:0x0042<p16>:byte] = A");

            Given_Bytes(0xFF); // mov\tR7,A
            AssertCode(
                "0|L--|0000(1): 1 instructions",
                "1|L--|R7 = A");
        }

        [Test]
        public void I8051_rw_sjmp()
        {
            Given_Bytes(0x80, 0xFE); // sjmp\t0000
            AssertCode(
                "0|T--|0000(2): 1 instructions",
                "1|T--|goto 0000");
        }

        [Test]
        public void I8051_rw_clr()
        {
            Given_Bytes(0xC2, 0x87); // clr\tP0.7
            AssertCode(
                "0|L--|0000(2): 1 instructions",
                "1|L--|P0 = P0 & 0x7F<8> | 0<8> << 7<8>");

            Given_Bytes(0xC3); // clr\tC
            AssertCode(
                "0|L--|0000(1): 1 instructions",
                "1|L--|C = false");
        }

        [Test]
        public void i8051_rw_cpl_bit()
        {
            Given_HexString("B232");
            AssertCode(     // cpl	/SFR30.2
                "0|L--|0000(2): 1 instructions",
                "1|L--|SFR30 = SFR30 ^ 4<8>");
        }

        [Test]
        public void I8051_rw_movx()
        {
            Given_Bytes(0xE0); // movx\tA,@DPTR
            AssertCode(
                "0|L--|0000(1): 1 instructions",
                "1|L--|A = Mem0[__data:DPTR:byte]");

            Given_Bytes(0xF2); // movx\t@R0,A
            AssertCode(
                "0|L--|0000(1): 1 instructions",
                "1|L--|Mem0[__data:CONVERT(R0, byte, word16):byte] = A");
        }

        [Test]
        public void I8051_rw_orl()
        {
            Given_Bytes(0x44, 0x42); // orl\tA,42
            AssertCode(
                "0|L--|0000(2): 2 instructions",
                "1|L--|A = A | 0x42<8>",
                "2|L--|P = cond(A)");
        }

        [Test]
        public void I8051_rw_pop()
        {
            Given_Bytes(0xD0, 0x82); // pop\t[0082]
            AssertCode(
                "0|L--|0000(2): 2 instructions",
                "1|L--|DPL = Mem0[__data:SP:byte]",
                "2|L--|SP = SP - 1<8>");
        }

        [Test]
        public void I8051_rw_reti()
        {
            Given_Bytes(0x32); // reti
            AssertCode(
                "0|R--|0000(1): 1 instructions",
                "1|R--|return (2,0)");
        }

        [Test]
        public void i8051_rw_rlc()
        {
            Given_HexString("33");
            AssertCode(     // rlc	A
                "0|L--|0000(1): 3 instructions",
                "1|L--|v5 = (A & 0x80<8>) != 0<8>",
                "2|L--|A = __rcl<byte,byte>(A, 1<8>, C)",
                "3|L--|C = v5");
        }

        [Test]
        public void i8051_rw_rrc()
        {
            Given_HexString("13");
            AssertCode(     // rrc	A
                "0|L--|0000(1): 3 instructions",
                "1|L--|v5 = (A & 1<8>) != 0<8>",
                "2|L--|A = __rcr<byte,byte>(A, 1<8>, C)",
                "3|L--|C = v5");
        }

        [Test]
        public void I8051_rw_xrl()
        {
            Given_Bytes(0x67); // xrl\tA,@R1
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|A = A ^ Mem0[__data:CONVERT(R1, byte, word16):byte]",
                "2|L--|P = cond(A)");
        }

        [Test]
        public void I8051_rw_addc()
        {
            Given_Bytes(0x34, 0x00); // addc\tA,00
            AssertCode(
                "0|L--|0000(2): 2 instructions",
                "1|L--|A = A + 0<8> + C",
                "2|L--|CAOP = cond(A)");
        }

        [Test]
        public void I8051_rw_setb()
        {
            Given_Bytes(0xD2, 0x96); // setb\tP1.6
            AssertCode(
                "0|L--|0000(2): 1 instructions",
                "1|L--|P1 = P1 & 0xBF<8> | true << 6<8>");
        }

        [Test]
        public void I8051_rw_cjne()
        {
            Given_Bytes(0xBC, 0x09, 0x03); // cjne\tR4,09,0006
            AssertCode(
                "0|T--|0000(3): 2 instructions",
                "1|L--|C = cond(R4 - 9<8>)",
                "2|T--|if (R4 != 9<8>) branch 0006");
        }

        [Test]
        public void I8051_rw_subb()
        {
            Given_Bytes(0x95, 0x42); // subb\tA,[0042]
            AssertCode(
                "0|L--|0000(2): 2 instructions",
                "1|L--|A = A - Mem0[__data:0x0042<p16>:byte] - C",
                "2|L--|CAOP = cond(A)");
        }

        [Test]
        public void I8051_rw_jc()
        {
            Given_Bytes(0x40, 0x0E); // jc\t0010
            AssertCode(
                "0|T--|0000(2): 1 instructions",
                "1|T--|if (Test(ULT,C)) branch 0010");
        }

        [Test]
        public void I8051_rw_xch()
        {
            Given_Bytes(0xCE); // xch\tA,R6
            AssertCode(
                "0|L--|0000(1): 3 instructions",
                "1|L--|v3 = A",
                "2|L--|A = R6",
                "3|L--|R6 = v3");
        }

        [Test]
        public void I8051_rw_jnz()
        {
            Given_Bytes(0x70, 0x2E); // jnz\t0030
            AssertCode(
                "0|T--|0000(2): 1 instructions",
                "1|T--|if (A != 0<8>) branch 0030");
        }

        [Test]
        public void I8051_rw_jnb()
        {
            Given_Bytes(0x30, 0x90, 0x3D); // jnb\tP1.0,0040
            AssertCode(
                "0|T--|0000(3): 1 instructions",
                "1|T--|if ((P1 & 1<8>) == 0<8>) branch 0040");
        }

        [Test]
        public void i8051_rw_da()
        {
            Given_HexString("D4");
            AssertCode(     // da	A
                "0|L--|0000(1): 1 instructions",
                "1|L--|C = __decimal_adjust_addition(A, out A)");
        }

        [Test]
        public void I8051_rw_dec()
        {
            Given_Bytes(0x18); // dec\tR0
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|R0 = R0 - 1<8>",
                "2|L--|P = cond(R0)");
        }

        [Test]
        public void I8051_rw_div()
        {
            Given_HexString("84");
            AssertCode(     // div	AB
                "0|L--|0000(1): 6 instructions",
                "1|L--|v5 = A /u B",
                "2|L--|v6 = A %u B",
                "3|L--|A = v5",
                "4|L--|B = v6",
                "5|L--|C = false",
                "6|L--|O = false");
        }

        [Test]
        public void I8051_rw_djnz()
        {
            Given_Bytes(0xD9, 0x08); // djnz\tR1,000A
            AssertCode(
                "0|T--|0000(2): 2 instructions",
                "1|L--|R1 = R1 - 1<8>",
                "2|T--|if (R1 != 0<8>) branch 000A");
        }

        [Test]
        public void I8051_rw_ret()
        {
            Given_Bytes(0x22); // ret
            AssertCode(
                "0|R--|0000(1): 1 instructions",
                "1|R--|return (2,0)");
        }

        [Test]
        public void I8051_rw_rl()
        {
            Given_Bytes(0x23); // rl\tA
            AssertCode(
                "0|L--|0000(1): 1 instructions",
                "1|L--|A = __rol<byte,byte>(A, 1<8>)");
        }

        [Test]
        public void I8051_rw_jnc()
        {
            Given_Bytes(0x50, 0x1E); // jnc\t0020
            AssertCode(
                "0|T--|0000(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 0020");
        }

        [Test]
        public void I8051_rw_jb()
        {
            Given_Bytes(0x20, 0x85, 0x6D); // jb\tP0.5,0070
            AssertCode(
                "0|T--|0000(3): 1 instructions",
                "1|T--|if ((P0 >>u 5<8> & 1<8>) != 0<8>) branch 0070");
        }

        [Test]
        public void I8051_rw_jz()
        {
            Given_Bytes(0x60, 0x6E); // jz\t0070
            AssertCode(
                "0|T--|0000(2): 1 instructions",
                "1|T--|if (A == 0<8>) branch 0070");
        }

        [Test]
        public void I8051_rw_push()
        {
            Given_Bytes(0xC0, 0x90); // push\t[0090]
            AssertCode(
                "0|L--|0000(2): 2 instructions",
                "1|L--|SP = SP + 1<8>",
                "2|L--|Mem0[__data:SP:byte] = Mem0[__data:0x0090<p16>:byte]");
        }

        [Test]
        public void I8051_rw_push_acc()
        {
            Given_Bytes(0xC0, 0xE0); // push\t[00E0] = alias for ACC.
            AssertCode(
                "0|L--|0000(2): 2 instructions",
                "1|L--|SP = SP + 1<8>",
                "2|L--|Mem0[__data:SP:byte] = A");
        }

        [Test]
        public void I8051_rw_mov_dptr()
        {
            Given_Bytes(0x90, 0x12, 0x34); // mov\tDPTR,1234
            AssertCode(
                "0|L--|0000(3): 1 instructions",
                "1|L--|DPTR = 0x1234<16>");
        }

        [Test]
        public void I8051_rw_movc()
        {
            Given_Bytes(0x93); // movc\t@DPTR+A
            AssertCode(
                "0|L--|0000(1): 1 instructions",
                "1|L--|A = Mem0[DPTR + A:byte]");
        }

        [Test]
        public void I8051_rw_movc_pcrel()
        {
            Given_Bytes(0x83); // movc\t@PC+A
            AssertCode(
                "0|L--|0000(1): 1 instructions",
                "1|L--|A = Mem0[0x0001<p16> + A:byte]");
        }

        [Test]
        public void I8051_rw_mul()
        {
            Given_Bytes(0xA4); // mul\tAB
            AssertCode(
                "0|L--|0000(1): 4 instructions",
                "1|L--|B_A = A *u16 B",
                "2|L--|P = cond(B_A)",
                "3|L--|O = B_A >u 0xFF<16>",
                "4|L--|C = false");
        }

        [Test]
        public void I8051_rw_jmp()
        {
            Given_Bytes(0x73); // jmp\t@DPTR+A
            AssertCode(
                "0|T--|0000(1): 1 instructions",
                "1|T--|goto DPTR + A");
        }

        [Test]
        public void I8051_rw_swap()
        {
            Given_Bytes(0xC4); // swap\tA
            AssertCode(
                "0|L--|0000(1): 3 instructions",
                "1|L--|v3 = A << 4<8>",
                "2|L--|A = A >>u 4<8>",
                "3|L--|A = A | v3");
        }

        [Test]
        public void i8051_rw_xchd()
        {
            Given_HexString("D6");
            AssertCode(     // xchd	A,@R0
                "0|L--|0000(1): 5 instructions",
                "1|L--|v3 = SLICE(A, word4, 0)",
                "2|L--|v4 = Mem0[__data:CONVERT(R0, byte, word16):byte]",
                "3|L--|A = SEQ(SLICE(A, word4, 4), SLICE(v4, word4, 0))",
                "4|L--|v4 = SEQ(SLICE(v4, word4, 4), v3)",
                "5|L--|Mem0[__data:CONVERT(R0, byte, word16):byte] = v4");
        }
    }
}
