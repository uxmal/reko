#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Arch.Mos6502;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Mos6502
{
    [TestFixture]
    class RewriterTests : RewriterTestBase
    {
        private IEnumerator<RtlInstructionCluster> eCluster;
        private Mos6502ProcessorArchitecture arch = new Mos6502ProcessorArchitecture();
        private LoadedImage image;
        private Address addrBase = new Address(0x0200);

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame)
        {
            return new Rewriter(arch, image.CreateReader(0), new Mos6502ProcessorState(arch), new Frame(arch.FramePointerType));
        }

        public override Address LoadAddress
        {
            get { return addrBase; }
        }

        private void BuildTest(params byte[] bytes)
        {
            image = new LoadedImage(new Address(0x200), bytes);
        }

        [Test]
        public void Rw6502_tax()
        {
            BuildTest(0xAA);
            AssertCode(
                "0|00000200(1): 2 instructions",
                "1|L--|x = a",
                "2|L--|NZ = cond(x)");
        }

        [Test]
        public void Rw6502_sbc()
        {
            BuildTest(0xF1, 0xE0);
            AssertCode(
                "0|00000200(2): 2 instructions",
                "1|L--|a = a - Mem0[Mem0[0x00E0:ptr16] + (uint16) y:byte] - !C",
                "2|L--|NVZC = cond(a)");
        }
    }
}