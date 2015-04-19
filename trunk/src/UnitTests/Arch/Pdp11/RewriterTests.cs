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

using Decompiler.Arch.Pdp11;
using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Pdp11
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
                "0|0200(2): 6 instructions",
                "1|L--|v3 = Mem0[r1:word16]",
                "2|L--|r1 = r1 + 0x0002",
                "3|L--|r0 = r0 ^ v3",
                "4|L--|NZ = cond(r0)",
                "5|L--|C = false",
                "6|L--|V = false");
        }
    }
}
