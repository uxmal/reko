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

using Reko.Arch.Arm;
using Reko.Core;
using Reko.Core.Rtl;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public class ThumbRewriterTests : RewriterTestBase
    {
        private ThumbProcessorArchitecture arch = new ThumbProcessorArchitecture();
        private LoadedImage image;
        private Address baseAddress = Address.Ptr32(0x00100000);

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return baseAddress; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            return new ThumbRewriter(arch, new LeImageReader(image, 0), new ThumbProcessorState(arch), frame, host);
        }

        private void BuildTest(params ushort[] words)
        {
            var bytes = words
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), })
                .ToArray();
            image = new LoadedImage(Address.Ptr32(0x00100000), bytes);
        }

        [Test]
        public void ThumbRw_push()
        {
            BuildTest(0xE92D, 0x4800); // "push.w\t{fp,lr}"
            AssertCode(
                "0|00100000(4): 3 instructions",
                "1|L--|sp = sp - 8",
                "2|L--|Mem0[sp + 0:word32] = lr",
                "3|L--|Mem0[sp + 4:word32] = fp");
        }

        [Test]
        public void ThumbRw_pop()
        {
            BuildTest(0xE8BD, 0x8800); // pop.w\t{fp,pc}
            AssertCode(
                "0|00100000(4): 3 instructions",
                "1|L--|fp = Mem0[sp + 4:word32]",
                "2|L--|pc = Mem0[sp + 0:word32]",
                "3|L--|sp = sp + 8");
        }

        [Test]
        public void ThumbRw_mov()
        {
            BuildTest(0x46EB); // mov\tfp,sp
            AssertCode(
                "0|00100000(2): 1 instructions",
                "1|L--|fp = sp");
        }

        [Test]
        public void ThumbRw_sub()
        {
            BuildTest(0xB082); // sub\tsp,#8
            AssertCode(
                "0|00100000(2): 1 instructions",
                "1|L--|sp = sp - 8");
        }

        [Test]
        public void ThumbRw_bl()
        {
            BuildTest(0xF000, 0xFA06); // bl\t$00100410
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|T--|call 00100410 (0)");
        }

        [Test]
        public void ThumbRw_str()
        {
            BuildTest(0x9000); // str\tr0,[sp]
            AssertCode(
                "0|00100000(2): 1 instructions",
                "1|L--|Mem0[sp:word32] = r0");
        }

        [Test]
        public void ThumbRw_ldr()
        {
            BuildTest(0x9B00); // ldr\tr3,[sp]
            AssertCode(
                "0|00100000(2): 1 instructions",
                "1|L--|r3 = Mem0[sp:word32]");
        }

        [Test]
        public void ThumbRw_ldr_displacement()
        {
            BuildTest(0x9801); // ldr\tr0,[sp,#4]
            AssertCode(
                "0|00100000(2): 1 instructions",
                "1|L--|r0 = Mem0[sp + 4:word32]");
        }

        [Test]
        public void ThumbRw_add()
        {
            BuildTest(0xB002); // add\tsp,#8
            AssertCode(
                "0|00100000(2): 1 instructions",
                "1|L--|sp = sp + 8");
        }

    
    }
}
