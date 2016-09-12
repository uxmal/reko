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

using Reko.Arch.Pdp11;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Pdp11
{
    [TestFixture]
    class RewriterTests : RewriterTestBase
    {
        private Pdp11Architecture arch = new Pdp11Architecture();
        private MemoryArea image;
        private Address addrBase = Address.Ptr16(0x0200);

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            var dasm = new Pdp11Disassembler(arch.CreateImageReader(image, 0), arch);
            return new Pdp11Rewriter(arch, dasm, frame, base.CreateHost());
        }

        public override Address LoadAddress
        {
            get { return addrBase; }
        }

        private void BuildTest(params ushort[] words)
        {
            var bytes = words
                .SelectMany(
                    w => new byte[] { (byte)w, (byte)(w >> 8) })
                .ToArray();
            image = new MemoryArea(LoadAddress, bytes);
        }

        [Test]
        public void Pdp11Rw_xor()
        {
            BuildTest(0x7811);
            AssertCode(
                "0|L--|0200(2): 6 instructions",
                "1|L--|v3 = Mem0[r1:word16]",
                "2|L--|r1 = r1 + 0x0002",
                "3|L--|r0 = r0 ^ v3",
                "4|L--|NZ = cond(r0)",
                "5|L--|V = false",
                "6|L--|C = false");
        }

        [Test]
        public void Pdp11Rw_mov()
        {
            BuildTest(0x12C2);
            AssertCode(
                "0|L--|0200(2): 3 instructions",
                "1|L--|r2 = Mem0[r3:word16]",
                "2|L--|NZ = cond(r2)",
                "3|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_movb()
        {
            BuildTest(0x92C2);
            AssertCode(
                "0|L--|0200(2): 3 instructions",
                "1|L--|r2 = (int16) Mem0[r3:byte]",
                "2|L--|NZ = cond(r2)",
                "3|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_clrb()
        {
            BuildTest(0x8A10);
            AssertCode(
                "0|L--|0200(2): 7 instructions",
                "1|L--|v3 = 0x00",
                "2|L--|Mem0[r0:byte] = v3",
                "3|L--|r0 = r0 + 0x0001",
                "4|L--|N = false",
                "5|L--|V = false",
                "6|L--|C = false",
                "7|L--|Z = true");
        }

        [Test]
        public void Pdp11Rw_jsr_relative()
        {
            BuildTest(0x09F7, 0x0582);  // jsr\tpc,0582(pc)
            AssertCode(
                "0|T--|0200(4): 1 instructions",
                "1|T--|call 0784 (2)");
        }

        [Test]
        public void Pdp11Rw_br()
        {
            BuildTest(0x01FF);  // br\t0200
            AssertCode(
                "0|T--|0200(2): 1 instructions",
                "1|T--|goto 0200");
        }

        [Test]
        public void Pdp11Rw_bne()
        {
            BuildTest(0x02FE);  // bne\t01FE
            AssertCode(
                "0|T--|0200(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 01FE");
        }

        [Test]
        public void Pdp11Rw_clr()
        {
            BuildTest(0x0A22);  // clr -(r2)
            AssertCode(
                "0|L--|0200(2): 6 instructions",
                "1|L--|r2 = r2 - 0x0002",
                "2|L--|Mem0[r2:word16] = 0x0000",
                "3|L--|N = false",
                "4|L--|V = false",
                "5|L--|C = false",
                "6|L--|Z = true");
        }

        [Test]
        public void Pdp11Rw_bisb()
        {
            BuildTest(0xD5DF, 0x2000, 0x0024);  // "bisb\t#0024,@#2000",
            AssertCode(
                "0|L--|0200(6): 4 instructions",
                "1|L--|v2 = Mem0[0x0024:word16] | 0x2000",
                "2|L--|Mem0[0x0024:word16] = v2",
                "3|L--|NZ = cond(v2)",
                "4|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_tst()
        {
            BuildTest(0x8BF3, 0x0075); // tst 0075(r3)
            AssertCode(
                "0|L--|0200(4): 5 instructions",
                "1|L--|v4 = Mem0[r3 + 0x0075:byte]",
                "2|L--|v4 = v4 & v4",
                "3|L--|NZ = cond(v4)",
                "4|L--|V = false",
                "5|L--|C = false");
        }

        [Test]
        public void Pdp11Rw_bic()
        {
            BuildTest(0x45C4, 0x0001); // bic r4,#0001
            AssertCode(
                "0|L--|0200(4): 3 instructions",
                "1|L--|r4 = r4 & ~0x0001",
                "2|L--|NZ = cond(r4)",
                "3|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_add()
        {
            BuildTest(0x65C2, 0x00B2); // add #00B2,r2
            AssertCode(
                "0|L--|0200(4): 2 instructions",
                "1|L--|r2 = r2 + 0x00B2",
                "2|L--|NZVC = cond(r2)");
        }
    }
}
