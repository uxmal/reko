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

using Decompiler.Arch.Z80;
using Decompiler.Core;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Z80
{
    [TestFixture]
    public class RewriterTests
    {
        private Z80ProcessorArchitecture arch = new Z80ProcessorArchitecture();
        private Address baseAddr = new Address(0x0100);
        private Z80ProcessorState state;
        private IRewriterHost host;
        private IEnumerator<RtlInstructionCluster> e;
        private MockRepository repository;

        [SetUp]
        public void Setup()
        {
            state = (Z80ProcessorState) arch.CreateProcessorState();
            repository = new MockRepository();
            host = repository.StrictMock<IRewriterHost>();
        }

        private void AssertCode(params string[] expected)
        {
            int i = 0;
            while (i < expected.Length && e.MoveNext())
            {
                Assert.AreEqual(expected[i], string.Format("{0}|{1}", i, e.Current));
                ++i;
                var ee = e.Current.Instructions.GetEnumerator();
                while (i < expected.Length && ee.MoveNext())
                {
                    Assert.AreEqual(expected[i], string.Format("{0}|{1}|{2}", i, RtlInstruction.FormatClass(ee.Current.Class), ee.Current));
                    ++i;
                }
            }
            Assert.AreEqual(expected.Length, i, "Expected " + expected.Length + " instructions.");
            Assert.IsFalse(e.MoveNext(), "More instructions were emitted than were expected.");
        }

        
        private void BuildTest(params byte[] bytes)
        {
            repository.ReplayAll();
            var image = new LoadedImage(baseAddr, bytes);
            e = new Z80Rewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host).GetEnumerator();
        }

        [Test]
        public void Z80rw_lxi()
        {
            BuildTest(0x21, 0x34, 0x12);
            AssertCode("0|00000100(3): 1 instructions",
                "1|L--|hl = 0x1234");
        }

        [Test]
        public void Z80rw_mov_a_hl()
        {
            BuildTest(0x7E);
            AssertCode("0|00000100(1): 1 instructions",
                "1|L--|a = Mem0[hl:byte]");
        }

        [Test]
        public void Z80rw_jp()
        {
            BuildTest(0xC3, 0xAA, 0xBB);
            AssertCode("0|00000100(3): 1 instructions",
                "1|T--|goto 0xBBAA");
        }

        [Test]
        public void Z80rw_jp_nz()
        {
            BuildTest(0xC2, 0xAA, 0xBB);
            AssertCode("0|00000100(3): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch BBAA");
        }

        [Test]
        public void Z80rw_stx_b_d()
        {
            BuildTest(0xDD, 0x71, 0x80);
            AssertCode("0|00000100(3): 1 instructions",
                "1|L--|Mem0[ix - 0x0080:byte] = c");
        }

        [Test]
        public void Z80rw_push_hl()
        {
            BuildTest(0xE5);
            AssertCode("0|00000100(1): 2 instructions",
                "1|L--|sp = sp - 0x0002",
                "2|L--|Mem0[sp:word16] = hl");
        }
    }
}
