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
using Reko.Arch.Cray;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Cray.Ymp
{
    [TestFixture]
    public class YmpRewriterTests : RewriterTestBase
    {
        private IProcessorArchitecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new CrayYmpArchitecture(CreateServiceContainer(), "ymp", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;


        protected void Given_OctalString(string octalBytes)
        {
            var words = YmpDisassemblerTests.ToUInt16s(YmpDisassemblerTests.OctalStringToBytes(octalBytes))
                .ToArray();
            Given_MemoryArea(new Word16MemoryArea(LoadAddress, words));
        }

        [Test]
        public void YmpRw_clz()
        {
            Given_OctalString("027200");
            AssertCode(     // A2 ZS0
                "0|L--|00100000(1): 1 instructions",
                "1|L--|A2 = __clz(S0)");
        }

        [Test]
        public void YmpRw_S_and()
        {
            Given_OctalString("044123");
            AssertCode(
                "0|L--|00100000(1): 1 instructions",
                "1|L--|S1 = S2 & S3");
        }

        [Test]
        public void YmpRw_mov_Ai_Sj()
        {
            Given_OctalString("023710");  // A7 S1
            AssertCode(
                "0|L--|00100000(1): 1 instructions",
                "1|L--|A7 = S1");
        }

        [Test]
        public void YmpRw_mov_Si_Vj_Ak()
        {
            Given_OctalString("076123");  // S1 V2,A3
            AssertCode(
                "0|L--|00100000(1): 1 instructions",
                "1|L--|S1 = V2[A3]");
        }

        [Test]
        public void YmpRw_fmul_Sj_Sk()
        {
            Given_OctalString("064123");  // S1\tS2*FS3
            AssertCode(
                "0|L--|00100000(1): 1 instructions",
                "1|L--|S1 = S2 * S3");
        }

        [Test]
        public void YmpRw_iadd()
        {
            Given_OctalString("030252");
            AssertCode(     // A2 A5+A2
                "0|L--|00100000(1): 1 instructions",
                "1|L--|A2 = A5 + A2");
        }

        [Test]
        public void YmpRw_isub()
        {
            Given_OctalString("061373");
            AssertCode(     // S3 S7-S3
                "0|L--|00100000(1): 1 instructions",
                "1|L--|S3 = S7 - S3");
        }

        [Test]
        public void YmpRw_j_Bjk()
        {
            Given_OctalString("005077");  // J B63
            AssertCode(
                "0|T--|00100000(1): 1 instructions",
                "1|T--|goto B63");
        }

        [Test]
        public void YmpRw_jaz()
        {
            Given_OctalString("010000 133445");
            AssertCode(     // jaz 000000000056
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (A0 == 0<32>) branch 0000B725");
        }

        [Test]
        public void YmpRw_jsn()
        {
            Given_OctalString("015000000115");
            AssertCode(     // jsn 000000000115
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (S0 != 0<64>) branch 0000004D");
        }

        [Test]
        public void YmpRw_lmask()
        {
            Given_OctalString("042355");
            AssertCode(     // S3 <23
                "0|L--|00100000(1): 1 instructions",
                "1|L--|S3 = 0x7FFFF<64>");
        }

        [Test]
        public void YmpRw_lmask_allones()
        {
            Given_OctalString("042300");
            AssertCode(     // S3 <100
                "0|L--|00100000(1): 1 instructions",
                "1|L--|S3 = 0xFFFFFFFFFFFFFFFF<64>");
        }

        [Test]
        public void YmpRw_load()
        {
            Given_OctalString("120000056561000000");
            AssertCode(     // S0 056561,
                "0|L--|00100000(3): 1 instructions",
                "1|L--|S0 = Mem0[0x5D71<32>:word64]");
        }

        [Test]
        public void YmpRw_lsl()
        {
            Given_OctalString("054640");
            AssertCode(     // _lsl S6,S6,000040
                "0|L--|00100000(1): 1 instructions",
                "1|L--|S6 = S6 << 0x20<8>");
        }

        [Test]
        public void YmpRw_lsr()
        {
            Given_OctalString("055240");
            AssertCode(     // S2 S2>000040
                "0|L--|00100000(1): 1 instructions",
                "1|L--|S2 = S2 >>u 0x20<8>");
        }

        [Test]
        public void YmpRw_movz()
        {
            Given_OctalString("071306");
            AssertCode(     // S3 A6
                "0|L--|00100000(1): 1 instructions",
                "1|L--|S3 = CONVERT(A6, word32, word64)");
        }

        [Test]
        public void YmpRw_or()
        {
            Given_OctalString("051003");
            AssertCode(     // S0 S0!S3
                "0|L--|00100000(1): 1 instructions",
                "1|L--|S0 = S0 | S3");
        }

        [Test]
        public void YmpRw_r()
        {
            Given_OctalString("007001002514");
            AssertCode(     // r 000000202514
                "0|T--|00100000(2): 1 instructions",
                "1|T--|call 0001054C (0)");
        }

        [Test]
        public void YmpRw_store()
        {
            Given_OctalString("131700000000000000");
            AssertCode(     // 000000,A1 S7
                "0|L--|00100000(3): 1 instructions",
                "1|L--|Mem0[A1 + 0<32>:word64] = S7");
        }
    }
}
