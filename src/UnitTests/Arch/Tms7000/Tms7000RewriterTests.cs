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
                "0|L--|0100(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Tms7000rw_and()
        {
            RewriteBytes(0x63);
            AssertCode(
                "0|L--|0100(1): 3 instructions",
                "1|L--|a = a & b",
                "2|L--|NZ = cond(a)",
                "3|L--|C = false");
        }

        [Test]
        public void Tms7000rw_add()
        {
            RewriteBytes(0x58, 0x32);
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|b = b + 0x32",
                "2|L--|CNZ = cond(b)");
        }

        [Test]
        public void Tms7000rw_andp()
        {
            RewriteBytes(0xA3, 0x03, 0x44);
            AssertCode(
                "0|L--|0100(3): 3 instructions",
                "1|L--|p68 = p68 & 0x03",
                "2|L--|NZ = cond(p68)",
                "3|L--|C = false");
        }

        [Test]
        public void Tms7000rw_btjz()
        {
            RewriteBytes(0x27, 0x0F, 0x44);
            AssertCode(
                "0|T--|0100(3): 3 instructions",
                "1|L--|NZ = cond(a & ~0x0F)",
                "2|L--|C = false",
                "3|T--|if (Test(NE,Z)) branch 0147");
        }

        [Test]
        public void Tms7000rw_br_direct()
        {
            RewriteBytes(0x8C, 0x12, 0x34);
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|goto 1234");
        }

        [Test]
        public void Tms7000rw_br_indexed()
        {
            RewriteBytes(0xAC, 0x12, 0x34);
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|goto 0x1234 + (uint16) b");
        }

        [Test]
        public void Tms7000rw_br_indirect()
        {
            RewriteBytes(0x9C, 0x12);
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto r18_r17");
        }
    }
}
