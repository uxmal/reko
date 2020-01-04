#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
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
            this.arch = new Reko.Arch.Cray.CrayYmpArchitecture("ymp");
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var state = new CrayProcessorState(arch);
            return arch.CreateRewriter(new BeImageReader(mem, 0), state, binder, host);
        }

        protected void Given_OctalString(string octalBytes)
        {
            var bytes = YmpDisassemblerTests.OctalStringToBytes(octalBytes);
            Given_MemoryArea(new MemoryArea(LoadAddress, bytes));
        }

        [Test]
        public void YmpRw_S_and()
        {
            Given_OctalString("043123");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|S1 = S2 & S3");
        }

        [Test]
        public void YmpRw_mov_Ai_Sj()
        {
            Given_OctalString("023710");  // A7 S1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|A7 = S1");
        }

        [Test]
        public void YmpRw_mov_Si_Vj_Ak()
        {
            Given_OctalString("076123");  // S1 V2,A3
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|S1 = V2[A3]");
        }

        [Test]
        public void YmpRw_fmul_Sj_Sk()
        {
            Given_OctalString("064123");  // S1\tS2*FS3
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|S1 = S2 * S3");
        }

        [Test]
        public void YmpRw_j_Bjk()
        {
            Given_OctalString("005077");  // J B63
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto B63");
        }
    }
}
