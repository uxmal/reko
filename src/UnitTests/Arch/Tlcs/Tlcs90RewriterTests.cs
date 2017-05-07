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
using Reko.Arch.Tlcs.Tlcs90;
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
    public class Tlcs90RewriterTests : RewriterTestBase
    {
        private Tlcs90Architecture arch = new Tlcs90Architecture();
        private Address baseAddr = Address.Ptr16(0x0100);
        private Tlcs90State state;
        private MemoryArea image;

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            return new Tlcs90Rewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host);
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
            state = (Tlcs90State)arch.CreateProcessorState();
        }

        [Test]
        public void Tlcs90_rw_jp()
        {
            RewriteCode("1A0001");	// jp	0100
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|goto 0100");
        }

        [Test]
        public void Tlcs90_rw_ld()
        {
            RewriteCode("EB002026");	// ld	(2000),a
            AssertCode(
                "0|L--|0100(4): 2 instructions",
                "1|L--|v3 = a",
                "2|L--|Mem0[0x2000:byte] = v3");
        }

        [Test]
        public void Tlcs90_rw_pop()
        {
            RewriteCode("58");	// pop	bc
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|bc = Mem0[sp:word16]",
                "2|L--|sp = sp + 0x0002");
        }

        [Test]
        public void Tlcs90_rw_ret()
        {
            RewriteCode("1E");	// ret
            AssertCode(
                "0|T--|0100(1): 1 instructions",
                "1|T--|return (2,0)");
        }

        [Test]
        public void Tlcs90_rw_push()
        {
            RewriteCode("50");	// push	bc
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|sp = sp - 0x0002",
                "2|L--|Mem0[sp:word16] = bc");
        }

        [Test]
        public void Tlcs90_rw_ld_iy_nn()
        {
            RewriteCode("E300404D");    // ld\tiy,(4000)
            AssertCode(
                "0|L--|0100(4): 2 instructions",
                "1|L--|v2 = Mem0[0x4000:word16]",
                "2|L--|iy = v2");
        }
    }
}
