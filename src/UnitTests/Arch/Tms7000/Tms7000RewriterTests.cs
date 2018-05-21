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
using Reko.Arch.Tms7000;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Tms7000
{
    [TestFixture]
    public class Tms7000RewriterTests : RewriterTestBase
    {
        private Tms7000Architecture arch;
        private MemoryArea image;
        private Tms7000Disassembler dasm;

        public Tms7000RewriterTests()
        {
            this.arch = new Tms7000Architecture("tms7000");
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr16(0x0100);

        private MemoryArea RewriteBytes(params byte[] bytes)
        {
            this.image = new MemoryArea(LoadAddress, bytes);
            this.dasm = new Tms7000Disassembler(arch, image.CreateBeReader(LoadAddress));
            return image;
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            return new Tms7000Rewriter(arch, new BeImageReader(this.image, 0), new Tms7000State(arch), binder, host);
        }

        [Test]
        public void Tms7000Rw_nop()
        {
            RewriteBytes(0x00);
            AssertCode(
                "0|L--|0100(0): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Tms7000rw_and()
        {
            RewriteBytes(0x63);
            AssertCode(
                "0|L--|0100(0): 3 instructions",
                "1|L--|a = a & b",
                "2|L--|NZ = cond(a)",
                "3|L--|C = false");
        }
    }
}
