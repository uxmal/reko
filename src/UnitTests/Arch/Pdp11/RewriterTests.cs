#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
        private LoadedImage image;
        private Address addrBase = Address.Ptr16(0x0200);

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            var dasm = new Pdp11Disassembler(arch.CreateImageReader(image, 0), arch);
            return new Pdp11Rewriter(arch, dasm, frame);
        }

        public override Address LoadAddress
        {
            get { return addrBase; }
        }

        private void BuildTest(params ushort[] words)
        {
            var bytes = words
                .SelectMany(
                    w => new byte[] { (byte) w, (byte) (w >> 8) })
                .ToArray();
            image = new LoadedImage(LoadAddress, bytes);
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
    }
}
