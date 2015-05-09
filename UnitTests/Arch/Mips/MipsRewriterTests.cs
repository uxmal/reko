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

using Decompiler.Arch.Mips;
using Decompiler.Core;
using Decompiler.Core.Rtl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Mips
{
    [TestFixture]
    public class MipsRewriterTests : RewriterTestBase
    {
        static MipsProcessorArchitecture arch = new MipsProcessorArchitecture();
        private MipsDisassembler dasm;

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        private void RunTest(params string[] bitStrings)
        {
            var bytes = bitStrings.Select(bits => base.ParseBitPattern(bits))
                .SelectMany(u => new byte[] { (byte) (u >> 24), (byte) (u >> 16), (byte) (u >> 8), (byte) u })
                .ToArray();
            dasm = new MipsDisassembler(arch, new BeImageReader(new LoadedImage(Address.Ptr32(0x00100000), bytes), 0));
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            return new MipsRewriter(arch, dasm, frame);
        }

        [Test]
        public void MipsRw_lh()
        {
            RunTest("100001 01001 00011 1111111111001000");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|r3 = (word32) Mem0[r9 - 0x00000038:int16]");
        }

        [Test]
        public void MipsRw_lhu()
        {
            RunTest("100101 01011 01101 1111111111111000");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|r13 = (word32) Mem0[r11 - 0x00000008:word16]");
        }

        [Test]
        public void MipsRw_lui()
        {
            RunTest("001111 00000 00011 1111111111001000");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|r3 = 0xFFC80000");
        }

        [Test]
        public void MipsRw_ori_r0()
        {
            RunTest("001101 00000 00101 1111100000100111");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|r5 = 0x0000F827");
        }

        [Test]
        public void MipsRw_addi_r0()
        {
            RunTest("001000 00000 00010 1111111111111000");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|r2 = -8");
        }

        [Test]
        public void MipsRw_add()
        {
            RunTest("000000 00001 00010 00011 00000 100000");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|r3 = r1 + r2");
        }

        [Test]
        public void MipsRw_andi_0()
        {
            RunTest("001100 00000 00101 0000000000000000");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|r5 = 0x00000000");
        }

        [Test]
        public void MipsRw_bgtz()
        {
            RunTest("000111 00011 00000 1111111111111110");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|TD-|if (r3 > 0x00000000) branch 000FFFFC");
        }

        [Test]
        public void MipsRw_j()
        {
            RunTest("000010 11111111111111111111111111");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|TD-|goto 0FFFFFFC");
        }
    }
}
