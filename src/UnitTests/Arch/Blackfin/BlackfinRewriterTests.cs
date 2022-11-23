#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Arch.Blackfin;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Blackfin
{
    [TestFixture]
    public class BlackfinRewriterTests : RewriterTestBase
    {
        private BlackfinArchitecture arch;

        [SetUp]
        public void Setup()
        {
            this.arch = new BlackfinArchitecture(CreateServiceContainer(), "blackfin", new Dictionary<string, object>());
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr32(0x00100000);

        [Test]
        public void BlackfinRw_add()
        {
            Given_HexString("C26F");
            AssertCode(     // P2 += FFFFFFF8;
                "0|L--|00100000(2): 2 instructions",
                "1|L--|P2 = P2 + 0xFFFFFFF8<32>",
                "2|L--|NZVC = cond(P2)");
        }

        [Test]
        public void BlackfinRw_add3()
        {
            Given_HexString("8E5B");
            AssertCode(     // SP = SP + P1;
                "0|L--|00100000(2): 2 instructions",
                "1|L--|SP = SP + P1");
        }

        [Test]
        public void BlackfinRw_and3()
        {
            Given_HexString("0854");
            AssertCode(     // R0 = R0 & R1;
                "0|L--|00100000(2): 4 instructions",
                "1|L--|R0 = R0 & R1",
                "2|L--|NZ = cond(R0)",
                "3|L--|V = false",
                "4|L--|AC0 = false");
        }

        [Test]
        public void BlackfinRw_asr()
        {
            Given_HexString("124D");
            AssertCode(     // R2 >>>= 02;
                "0|L--|00100000(2): 2 instructions",
                "1|L--|R2 = R2 >> 2<8>",
                "2|L--|NZV = cond(R2)");
        }

        [Test]
        public void BlackfinRw_asr3()
        {
            Given_HexString("82C68905");
            AssertCode(     // R2 = R1 >>> 0F;
                "0|L--|00100000(4): 2 instructions",
                "1|L--|R2 = R1 >> 0xF<8>",
                "2|L--|NZV = cond(R2)");
        }

        [Test]
        public void BlackfinRw_bitclr()
        {
            Given_HexString("394C");
            AssertCode(     // BITCLR(R1,07);
                "0|L--|00100000(2): 5 instructions",
                "1|L--|R1 = __clear_bit<word32,byte>(R1, 7<8>)",
                "2|L--|AN = cond(R1)",
                "3|L--|AZ = false",
                "4|L--|V = false",
                "5|L--|AC0 = false");
        }

        [Test]
        public void BlackfinRw_bitset()
        {
            Given_HexString("F84A");
            AssertCode(     // BITSET(R0,1F);
                "0|L--|00100000(2): 5 instructions",
                "1|L--|R0 = __set_bit<word32,byte>(R0, 0x1F<8>)",
                "2|L--|AN = cond(R0)",
                "3|L--|AZ = false",
                "4|L--|V = false",
                "5|L--|AC0 = false");
        }

        [Test]
        public void BlackfinRw_CALL()
        {
            Given_HexString("FFE34FFF");
            AssertCode(     // CALL 07F67224;
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 000FFE9E (0)");
        }

        [Test]
        public void BlackfinRw_CSYNC()
        {
            Given_HexString("2300");
            AssertCode(     // CSYNC;
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__core_synchronize()");
        }


        [Test]
        public void BlackfinRw_JUMP()
        {
            Given_HexString("5200");
            AssertCode(     // JUMP [P2];
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto Mem0[P2:word32]");
        }

        [Test]
        public void BlackfinRw_ld_pair()
        {
            Given_HexString("1805");
            AssertCode( // (FP:P0) = [SP++];
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v3 = SP",
                "2|L--|SP = SP + 4<i32>");
        }

        [Test]
        public void BlackfinRw_ldb_postdec()
        {
            Given_HexString("AD98");
            AssertCode(
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v3 = P5",
                "2|L--|P5 = P5 - 2<i32>",
                "3|L--|v4 = SLICE(Mem0[v3:word16], byte, 0)",
                "4|L--|R5 = CONVERT(v4, byte, word32)");
        }

        [Test]
        public void BlackfinRw_ldsw()
        {
            Given_HexString("0AA9");
            AssertCode( // R2 = W[P1 + 0x0008] (X);
                "0|L--|00100000(2): 1 instructions",
                "1|L--|R2 = CONVERT(Mem0[P1 + 8<i32>:word16], int16, int32)");
        }

        [Test]
        public void BlackfinRw_mov()
        {
            Given_HexString("0EE1EC0F");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|SP.L = 0xFEC<16>");
        }

        [Test]
        public void BlackfinRw_mov_x()
        {
            Given_HexString("20E1E803");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|R0 = CONVERT(0x3E8<16>, int16, int32)");
        }

        [Test]
        public void BlackfinRw_store_postinc()
        {
            Given_HexString("059A");
            AssertCode( // B[P0++] = R5;
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v4 = P0",
                "2|L--|P0 = P0 + 1<i32>",
                "3|L--|Mem0[v4:byte] = R5");
        }

        [Test]
        public void BlackfinRw_JUMP_L()
        {
            Given_HexString("FFE2D05C");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 000EB9A0");
        }



        [Test]
        public void BlackfinRw_CLI()
        {
            Given_HexString("3000");
            AssertCode(
                "0|S--|00100000(2): 1 instructions",
                "1|L--|__cli()");
        }

        [Test]
        public void BlackfinRw_EXCPT()
        {
            Given_HexString("A100");
            AssertCode(     // EXCPT 01;
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__force_exception(1<8>)");
        }

        [Test]
        public void BlackfinRw_JUMP_S()
        {
            Given_HexString("FF2F");
            AssertCode(     // JUMP.S FFA0000A;
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto 000FFFFE");
        }

        [Test]
        public void BlackfinRw_LINK()
        {
            Given_HexString("00E80200");
            AssertCode(     // LINK +00000000;
                "0|L--|00100000(4): 3 instructions",
                "1|L--|SP = SP - 8<i32>",
                "2|L--|Mem0[SP:word32] = FP",
                "3|L--|SP = SP - 32<i32>");
        }

        [Test]
        public void BlackfinRw_lsl()
        {
            Given_HexString("8840");
            AssertCode(     // R0 <<= R1;
                "0|L--|00100000(2): 2 instructions",
                "1|L--|R0 = R0 << R1",
                "2|L--|NZV = cond(R0)");
        }

        [Test]
        public void BlackfinRw_lsl3()
        {
            Given_HexString("82C6C084");
            AssertCode(     // R2 = R0 << F8;
                "0|L--|00100000(4): 2 instructions",
                "1|L--|R2 = R0 << 0xF8<8>",
                "2|L--|NZV = cond(R2)");
        }

        [Test]
        public void BlackfinRw_lsr()
        {
            Given_HexString("204E");
            AssertCode(     // R0 >>= 04;
                "0|L--|00100000(2): 2 instructions",
                "1|L--|R0 = R0 >>u 4<8>",
                "2|L--|NZV = cond(R0)");
        }

        [Test]
        public void BlackfinRw_lsr3()
        {
            Given_HexString("82C6F885");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|R2 = R0 >>u 1<8>",
                "2|L--|NZV = cond(R2)");
        }

        [Test]
        public void BlackfinRw_mov_cc_lt()
        {
            Given_HexString("810C");
            AssertCode(     // CC = R1 < 00000001;
                "0|L--|00100000(2): 2 instructions",
                "1|L--|CC = R1 < 1<32>",
                "2|L--|NZVC = R1 < 1<32>");
        }

        [Test]
        public void BlackfinRw_mov_cc_ule()
        {
            Given_HexString("010A");
            AssertCode(     // CC = R0 <= R1;
                "0|L--|00100000(2): 2 instructions",
                "1|L--|CC = R0 <=u R1",
                "2|L--|NZVC = R0 <=u R1");
        }

        [Test]
        public void BlackfinRw_mov_zb()
        {
            Given_HexString("4043");
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v3 = SLICE(R0, byte, 0)",
                "2|L--|R0 = CONVERT(v3, byte, word32)");
        }

        [Test]
        public void BlackfinRw_mov_xb()
        {
            Given_HexString("0143");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|R1 = CONVERT(SLICE(R0, int8, 0), int8, int32)");
        }

        [Test]
        [Ignore("Need an actual binary")]
        public void BlackfinRw_mov_cc_eq()
        {
            Given_HexString("010C");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("Need an actual binary")]
        public void BlackfinRw_mov_cc_n_bittest()
        {
            Given_HexString("3948");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("Need an actual binary")]
        public void BlackfinRw_mov_cc_le()
        {
            Given_HexString("020D");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void BlackfinRw_mov_z()
        {
            Given_HexString("1095");
            AssertCode(     // R0 = W[P2] (Z);
                "0|L--|00100000(2): 1 instructions",
                "1|L--|R0 = CONVERT(Mem0[P2:word16], word16, word32)");
        }
        [Test]
        public void BlackfinRw_mov_zl()
        {
            Given_HexString("C342");
            AssertCode(     // R3 = R0.L (Z);
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v3 = SLICE(R0, word16, 0)",
                "2|L--|R3 = CONVERT(v3, word16, word32)");
        }

        [Test]
        public void BlackfinRw_mul()
        {
            Given_HexString("CA40");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|R2 = R2 * R1");
        }

        [Test]
        public void BlackfinRw_neg()
        {
            Given_HexString("8243");
            AssertCode(     // R2 = -R0;
                "0|L--|00100000(2): 3 instructions",
                "1|L--|R2 = -R0",
                "2|L--|NZV = cond(R2)",
                "3|L--|AC0 = R2 == 0<32>");
        }

        [Test]
        public void BlackfinRw_NOP()
        {
            Given_HexString("0000");
            AssertCode(     // NOP;
                "0|L--|00100000(2): 1 instructions",
                "1|L--|nop");
        }


        [Test]
        public void BlackfinRw_RTN()
        {
            Given_HexString("1300");
            AssertCode(
                "0|R--|00100000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void BlackfinRw_RTS()
        {
            Given_HexString("1000");
            AssertCode(
                "0|R--|00100000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void BlackfinRw_SSYNC()
        {
            Given_HexString("2400");
            AssertCode(     // SSYNC;
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__system_synchronize()");
        }

        [Test]
        public void BlackfinRw_sub3()
        {
            Given_HexString("4152");
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|R1 = R1 - R0",
                "2|L--|NZVC = cond(R1)");
        }

        [Test]
        public void BlackfinRw_UNLINK()
        {
            Given_HexString("01E80000");
            AssertCode(     // UNLINK;
                "0|L--|00100000(4): 3 instructions",
                "1|L--|SP = FP",
                "2|L--|FP = Mem0[SP:word32]",
                "3|L--|SP = SP + 8<i32>");
        }

        [Test]
        public void BlackfinRw_xor3()
        {
            Given_HexString("C858");
            AssertCode(
                "0|L--|00100000(2): 4 instructions",
                "1|L--|R3 = R0 ^ R1",
                "2|L--|NZ = cond(R3)",
                "3|L--|V = false",
                "4|L--|AC0 = false");
        }
    }
}