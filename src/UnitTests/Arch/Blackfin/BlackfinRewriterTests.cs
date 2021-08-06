#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void BlackfinRw_mov()
        {
            Given_HexString("0EE1EC0F");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|SP.L = 0xFEC<16>");
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
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v3 = P5",
                "2|L--|P5 = P5 - 2<i32>",
                "3|L--|R5 = CONVERT(SLICE(Mem0[v3:word16], byte, 0), byte, word32)");
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
        public void BlackfinRw_RTN()
        {
            Given_HexString("1300");
            AssertCode(
                "0|R--|00100000(2): 1 instructions",
                "1|R--|return (0,0)");
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
        public void BlackfinRw_xor3()
        {
            Given_HexString("C858");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|R3 = R0 ^ R1");
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
        public void BlackfinRw_RTS()
        {
            Given_HexString("1000");
            AssertCode(
                "0|R--|00100000(2): 1 instructions",
                "1|R--|return (0,0)");
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
        public void BlackfinRw_mov_zb()
        {
            Given_HexString("4043");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|R0 = CONVERT(SLICE(R0, byte, 0), byte, word32)");
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
        public void BlackfinRw_sub3()
        {
            Given_HexString("4152");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|R1 = R1 - R0");
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
        public void BlackfinRw_CLI()
        {
            Given_HexString("3000");
            AssertCode(
                "0|S--|00100000(2): 1 instructions",
                "1|L--|__cli()");
        }

        [Test]
        [Ignore("Decoder NYI")]
        public void BlackfinRw_lsr3()
        {
            Given_HexString("82C6F885");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }
    }
}