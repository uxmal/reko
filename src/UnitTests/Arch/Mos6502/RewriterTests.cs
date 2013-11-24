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
    public class RewriterTests
    {
        private IEnumerator<RtlInstructionCluster> eCluster;

        private void BuildTest(params byte[] bytes)
        {
            var arch = new Mos6502ProcessorArchitecture();
            var image = new LoadedImage(new Address(0x200), bytes);
            var rdr = new LeImageReader(image, 0);
            var dasm = new Disassembler(rdr);
            var rewriter = new Rewriter(arch, image.CreateReader(0), new Mos6502ProcessorState(arch), new Frame(arch.FramePointerType));
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
        public void Rw6502_tax()
        {
            BuildTest(0xAA);
            AssertCode("0|00000200(1): 2 instructions",
                "1|x = a",
                "2|NZ = cond(x)");
        }

        [Test]
        public void Rw6502_sbc()
        {
            BuildTest(0xF1, 0xE0);
            AssertCode("0|00000200(2): 2 instructions",
                "1|a = a - Mem0[Mem0[0x00E0:ptr16] + (uint16) y:byte] - !C",
                "2|NVZC = cond(a)");
        }
    }
}