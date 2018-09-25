#region License
/*
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Arch.zSeries;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.zSeries
{
    [TestFixture]
    public class zSeriesRewriterTests : RewriterTestBase
    {
        private MemoryArea image;

        public zSeriesRewriterTests()
        {
            this.Architecture = new zSeriesArchitecture("zSeries");
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        protected override MemoryArea RewriteCode(string hexBytes)
        {
            return image;
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            return Architecture.CreateRewriter(
                new BeImageReader(image, image.BaseAddress),
                Architecture.CreateProcessorState(),
                binder,
                host);
        }

        private void Given_MachineCode(string hex)
        {
            var bytes = OperatingEnvironmentElement.LoadHexBytes(hex)
                .ToArray();
            this.image = new MemoryArea(LoadAddress, bytes);
        }

        [Test]
        public void zSeriesRw_stmg()
        {
            Given_MachineCode("EB68F0300024");
            AssertCode(     // stmg	r6,r8,48(r15)
                "0|L--|00100000(6): 6 instructions",
                "1|L--|v3 = r15 + 48",
                "2|L--|Mem0[v3:word64] = r6",
                "3|L--|v3 = v3 + 8",
                "4|L--|Mem0[v3:word64] = r7",
                "5|L--|v3 = v3 + 8",
                "6|L--|Mem0[v3:word64] = r8");
        }

        [Test]
        public void zSeriesRw_larl()
        {
            Given_MachineCode("C05000000140");
            AssertCode(     // larl	r5,00100280
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r5 = 00100280");
        }

        [Test]
        public void zSeriesRw_br()
        {
            Given_MachineCode("07FE");
            AssertCode(     // br	r14
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto r14");
        }
    }
}
