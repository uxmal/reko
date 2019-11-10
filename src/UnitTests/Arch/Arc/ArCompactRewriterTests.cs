#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Reko.Arch.Arc;
using Reko.Core;
using Reko.Core.Rtl;

namespace Reko.UnitTests.Arch.Arc
{
    [TestFixture]
    public class ARCompactRewriterTests : RewriterTestBase
    {
        private ARCompactArchitecture arch;
        private Address addr;
        private MemoryArea image;

        [SetUp]
        public void Setup()
        {
            this.arch = new ARCompactArchitecture("arCompact");
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override MemoryArea RewriteCode(string hexBytes)
        {
            var bytes = HexStringToBytes(hexBytes);
            this.image = new MemoryArea(LoadAddress, bytes);
            return image;
        }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(IStorageBinder binder, IRewriterHost host)
        {
            var state = new ARCompactState(arch);
            var rdr = arch.CreateImageReader(image, 0);
            return new ARCompactRewriter(arch, rdr, state, binder, host);
        }

        [Test]
        public void ARCompactRw_push_s()
        {
            RewriteCode("C5E1"); // push_s	r13
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }
    }
}
