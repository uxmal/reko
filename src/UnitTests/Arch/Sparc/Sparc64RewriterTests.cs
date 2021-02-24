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
using Reko.Arch.Sparc;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Sparc
{
    public class Sparc64RewriterTests : RewriterTestBase
    {
        private readonly SparcArchitecture64 arch;
        private readonly Address addrLoad;

        public Sparc64RewriterTests()
        {
            this.arch = new SparcArchitecture64(CreateServiceContainer(), "sparc64", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr64(0x10_0000_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var rdr = mem.CreateBeReader(0);
            return arch.CreateRewriter(rdr, arch.CreateProcessorState(), binder, host);
        }


        [Test]
        public void Sparc64Rw_brz()
        {
            Given_HexString("02C24008");
            AssertCode(     // brz	%o1,0000000000010850
                "0|TD-|0000001000000000(4): 1 instructions",
                "1|TD-|if (o1 == 0<64>) branch 0000001000010020");
        }

        [Test]
        public void Sparc64Rw_fcvtsd()
        {
            Given_HexString("93A09937 "); // 0CFFA93B4F089C2CF32A5B47");
            AssertCode(
                "0|L--|0000001000000000(4): 1 instructions",
                "1|L--|d20 = CONVERT(f23, real32, real64)");
        }

        [Test]
        public void Sparc64Rw_fitoq()
        {
            Given_HexString("89BC0CCC");
            AssertCode(
                "0|L--|0000001000000000(4): 1 instructions",
                "1|L--|q4 = CONVERT(f12, int32, real128)");
        }

        [Test]
        public void Sparc64Rw_ldx()
        {
            Given_HexString("D25BA8AF");
            AssertCode(     // ldx	[%sp+2223],%o1
                "0|L--|0000001000000000(4): 1 instructions",
                "1|L--|o1 = Mem0[sp + 2223<i32>:word64]");
        }

        [Test]
        public void Sparc64Rw_sllx()
        {
            Given_HexString("BB2F7020");
            AssertCode(     // sllx	%i5,FFFFFFE0,%i5
                "0|L--|0000001000000000(4): 1 instructions",
                "1|L--|i5 = i5 << 0x20<64>");
        }

        [Test]
        public void Sparc64Rw_srax()
        {
            Given_HexString("B93F3003");
            AssertCode(     // srax	%i4,00000003,%i4
                "0|L--|0000001000000000(4): 1 instructions",
                "1|L--|i4 = i4 >> 3<64>");
        }

        [Test]
        public void Sparc64Rw_stx()
        {
            Given_HexString("F277A887");
            AssertCode(     // stx	%i1,[%i6+2183]
                "0|L--|0000001000000000(4): 1 instructions",
                "1|L--|Mem0[i6 + 2183<i32>:word64] = i1");
        }

        [Test]
        public void Sparc64Rw_stw()
        {
            Given_HexString("C227A87F");
            AssertCode(     // stw	%g1,[%i6+2175]
                "0|L--|0000001000000000(4): 1 instructions",
                "1|L--|Mem0[i6 + 2175<i32>:word32] = SLICE(g1, word32, 0)");
        }

        [Test]
        public void Sparc64Rw_mulx()
        {
            Given_HexString("82488001");
            AssertCode(     // mulx	%g2,%g1,%g1
                "0|L--|0000001000000000(4): 1 instructions",
                "1|L--|g1 = g2 * g1");
        }
    }
}
