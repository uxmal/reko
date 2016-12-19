#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Arch.Tlcs;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Tlcs
{
    [TestFixture]
    public class Tlcs900RewriterTests : RewriterTestBase
    {
        private Tlcs900Architecture arch = new Tlcs900Architecture();
        private Address baseAddr = Address.Ptr32(0x0010000);
        private Tlcs900ProcessorState state;
        private MemoryArea image;

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            return new Tlcs900Rewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host);
        }

        public override Address LoadAddress
        {
            get { return baseAddr; }
        }

        protected override MemoryArea RewriteCode(string hexBytes)
        {
            var bytes = OperatingEnvironmentElement.LoadHexBytes(hexBytes)
                .ToArray();
            this.image = new MemoryArea(LoadAddress, bytes);
            return image;
        }


        [SetUp]
        public void Setup()
        {
            state = (Tlcs900ProcessorState)arch.CreateProcessorState();
        }

        [Test]
        public void Tlcs900_rw_ld()
        {
            RewriteCode("9F1621");
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|v3 = Mem0[xsp + 22:word16]",
                "2|L--|bc = v3");
        }

        [Test]
        public void Tlcs900_rw_add()
        {
            RewriteCode("E9C8FFFFFFFF");
            AssertCode(
                "0|L--|00010000(6): 3 instructions",
                "1|L--|xbc = xbc + 0xFFFFFFFF",
                "2|L--|N = false",
                "3|L--|SZHVC = cond(xbc)");
        }
    }
}
