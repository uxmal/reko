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

using Reko.Arch.Mos6502;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Mos6502
{
    [TestFixture]
    class RewriterTests : RewriterTestBase
    {
        private Mos6502ProcessorArchitecture arch = new Mos6502ProcessorArchitecture("mos6502");
        private MemoryArea image;
        private Address addrBase = Address.Ptr16(0x0200);

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            var state = new Mos6502ProcessorState(arch, new SegmentMap(addrBase));
            return new Rewriter(arch, image.CreateLeReader(0), state, new Frame(arch.FramePointerType), host);
        }

        public override Address LoadAddress
        {
            get { return addrBase; }
        }

        private void BuildTest(params byte[] bytes)
        {
            image = new MemoryArea(addrBase, bytes);
        }

        [Test]
        public void Rw6502_tax()
        {
            BuildTest(0xAA);
            AssertCode(
                "0|L--|0200(1): 2 instructions",
                "1|L--|x = a",
                "2|L--|NZ = cond(x)");
        }

        [Test]
        public void Rw6502_sbc()
        {
            BuildTest(0xF1, 0xE0);
            AssertCode(
                "0|L--|0200(2): 2 instructions",
                "1|L--|a = a - Mem0[Mem0[0x00E0:ptr16] + (uint16) y:byte] - !C",
                "2|L--|NVZC = cond(a)");
        }

        [Test]
        public void Rw6502_dec_A()
        {
            BuildTest(0xCE, 0x34, 0x12);
            AssertCode(
                "0|L--|0200(3): 3 instructions",
                "1|L--|v2 = Mem0[0x1234:byte] - 0x01",
                "2|L--|Mem0[0x1234:byte] = v2",
                "3|L--|NZ = cond(v2)");
        }

        [Test]
        public void Rw6502_rts()
        {
            BuildTest(0x60);
            AssertCode(
                "0|T--|0200(1): 1 instructions",
                "1|T--|return (2,0)");
        }

        [Test]
        public void Rw6502_pha()
        {
            BuildTest(0x48);
            AssertCode(
                "0|L--|0200(1): 2 instructions",
                "1|L--|s = s - 0x01",
                "2|L--|Mem0[s:byte] = a");
        }

        [Test]
        public void Rw6502_pla()
        {
            BuildTest(0x68);
            AssertCode(
                "0|L--|0200(1): 3 instructions",
                "1|L--|a = Mem0[s:byte]",
                "2|L--|s = s + 0x01",
                "3|L--|NZ = cond(a)");
        }
        [Test]
        public void Rw6502_asl_zx()
        {
            BuildTest(0x16, 0x64);
            AssertCode(
                "0|L--|0200(2): 3 instructions",
                "1|L--|v3 = Mem0[0x0064 + x:byte] << 0x01",
                "2|L--|Mem0[0x0064 + x:byte] = v3",
                "3|L--|NCZ = cond(v3)");
        }

        [Test]
        public void Rw6502_beq()
        {
            BuildTest(0xF0, 0x64);
            AssertCode(
                "0|T--|0200(2): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 0266");
        }

        [Test]
        public void Rw6502_cmp_iX()
        {
            BuildTest(0xC1, 0x38);
            AssertCode(
                "0|L--|0200(2): 1 instructions",
                "1|L--|NCZ = cond(a - Mem0[Mem0[0x0038 + (uint16) x:ptr16]:byte])");
        }

        [Test]
        public void Rw6502_ldy_x()
        {
            BuildTest(0xBC, 0x34, 0x12);
            AssertCode(
                "0|L--|0200(3): 2 instructions",
                "1|L--|y = Mem0[0x1234 + x:byte]",
                "2|L--|NZ = cond(y)");
        }

        [Test]
        public void Rw6502_asl()
        {
            BuildTest(0x0A);
            AssertCode(
                "0|L--|0200(1): 3 instructions",
                "1|L--|v3 = a << 0x01",
                "2|L--|a = v3",
                "3|L--|NCZ = cond(v3)");
        }

        [Test]
        public void Rw6502_jsr()
        {
            BuildTest(0x20, 0x13, 0xEA);
            AssertCode(
                "0|T--|0200(3): 1 instructions",
                "1|T--|call 0xEA13 (2)");
        }

        [Test]
        public void Rw6502_sta()
        {
            BuildTest(0x85, 0xD0);
            AssertCode(
                "0|L--|0200(2): 1 instructions",
                "1|L--|Mem0[0x00D0:byte] = a");
        }
    }
}