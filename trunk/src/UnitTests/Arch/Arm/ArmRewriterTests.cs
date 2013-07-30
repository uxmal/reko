#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Arch.Arm;
using Decompiler.Core;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Arm
{
    [TestFixture]   
    public class ArmRewriterTests : ArmTestBase
    {
        private IEnumerator<RtlInstructionCluster> eCluster;

        protected override IProcessorArchitecture CreateArchitecture()
        {
            return new ArmProcessorArchitecture();
        }

        protected void RewriteBits(string bitPattern)
        {
            var arch = new ArmProcessorArchitecture();
            var image = new LoadedImage(new Address(0x00100000), new byte[4]);
            LeImageWriter w = new LeImageWriter(image.Bytes);
            uint instr = ParseBitPattern(bitPattern);
            w.WriteLeUInt32(0, instr);
            var dasm = new ArmDisassembler2(new ArmProcessorArchitecture(), image.CreateReader(0));
            var rewriter = new ArmRewriter(arch, image.CreateReader(0), new ArmProcessorState(arch), new Frame(PrimitiveType.Word32));
            eCluster = rewriter.GetEnumerator();
        }

        private void AssertCode(params string[] expected)
        {
            int i = 0;
            var e = eCluster;
            while (i < expected.Length && e.MoveNext())
            {
                Assert.AreEqual(expected[i], string.Format("{0}|{1}", i, e.Current));
                ++i;
                var ee = e.Current.Instructions.GetEnumerator();
                while (i < expected.Length && ee.MoveNext())
                {
                    Assert.AreEqual(expected[i], string.Format("{0}|{1}", i, ee.Current));
                    ++i;
                }
            }
            Assert.AreEqual(expected.Length, i, "Expected " + expected.Length + " instructions.");
            Assert.IsFalse(e.MoveNext(), "More instructions were emitted than were expected.");
        }

        [Test]
        public void mov_r1_r2()
        {
            RewriteBits("1110 00 0 1101 0 0000 0001 00000000 0010");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|r1 = r2");
        }

        [Test]
        public void add_r1_r2_r3()
        {
            RewriteBits("1110 00 0 0100 0 0010 0001 00000000 0011");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|r1 = r2 + r3");
        }

        [Test]
        public void adds_r1_r2_r3()
        {
            RewriteBits("1110 00 0 0100 1 0010 0001 00000000 0011");
            AssertCode(
                "0|00100000(4): 2 instructions",
                "1|r1 = r2 + r3",
                "2|SZCO = cond(r1)");
        }

        [Test]
        public void subgt_r1_r2_imm4()
        {
            RewriteBits("1100 00 1 0010 0 0010 0001 0000 00000100");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|if (Test(GT,SCZO)) r1 = r2 - 0x00000004");
        }

        [Test]
        public void orr_r3_r4_r5_lsl_5()
        {
            RewriteBits("1110 00 0 1100 0 1100 0001 00100 000 0100");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|r1 = r12 | r4 << 0x04");
        }

        [Test]
        public void brgt()
        {
            RewriteBits("1100 1010 000000000000000000000000");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|if (Test(GT,SCZO)) branch 00100008");
        }

    }
}
