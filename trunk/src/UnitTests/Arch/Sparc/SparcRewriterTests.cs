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

using Decompiler.Arch.Sparc;
using Decompiler.Core;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Sparc
{
    [TestFixture]
    public class SparcRewriterTests
    {
        private SparcArchitecture arch = new SparcArchitecture(PrimitiveType.Word32);
        private Address baseAddr = new Address(0x00100000);
        private SparcProcessorState state;
        private IRewriterHost host;
        private IEnumerator<RtlInstructionCluster> e;

        [SetUp]
        public void Setup()
        {
            state = (SparcProcessorState) arch.CreateProcessorState();
        }

        private void AssertCode( params string[] expected)
        {
            int i = 0;
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

        private void BuildTest(params uint[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) (w >> 24),
                (byte) (w >> 16),
                (byte) (w >> 8),
                (byte) w
            }).ToArray();
            var image = new ProgramImage(baseAddr, bytes);
            e = new SparcRewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host).GetEnumerator();
        }

        [Test]
        public void SparcRw_call()
        {
            BuildTest(0x7FFFFFFF);  // "call\t000FFFFC"
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|call 000FFFFC (0)");
        }

        [Test]
        public void SparcRw_addcc()
        {
            BuildTest(0x8A804004); // "addcc\t%g0,%g4,%g5"
            AssertCode(
                "0|00100000(4): 2 instructions",
                "1|g5 = g1 + g4",
                "2|NZVC = cond(g5)");
        }

        [Test]
        public void SparcRw_or_imm()
        {
            BuildTest(0xBE10E004);//"or\t%g3,0x00000004,%i7");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|i7 = g3 | 0x00000004");
        }

        [Test]
        public void SparcRw_and_neg()
        {
            BuildTest(0x86087FFE); // "and\t%g1,0xFFFFFFFE,%g3")
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|g3 = g1 & 0xFFFFFFFE");
        }

        [Test]
        public void SparcRw_sll_imm()
        {
            BuildTest(0xAB2EA01F);
            AssertCode( "sll\t%i2,0x0000001F,%l5");
        }

        [Test]
        public void SparcRw_sethi()
        {
            BuildTest(0x0B00AAAA);
            AssertCode( "sethi\t0x0000AAAA,%g5");
        }

        [Test]
        public void SparcRw_taddcc()
        {
            BuildTest(0x8B006001);
            AssertCode("taddcc\t%g1,0x00000001,%g5");
        }

        [Test]
        public void SparcRw_mulscc()
        {
            BuildTest(0x8B204009);
            AssertCode("mulscc\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcRw_umul()
        {
            BuildTest(0x8A504009);
            AssertCode("umul\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcRw_smulcc()
        {
            BuildTest(0x8A584009);
            AssertCode("smul\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcRw_udivcc()
        {
            BuildTest(0x8AF04009);
            AssertCode("udivcc\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcRw_sdiv()
        {
            BuildTest(0x8A784009);
            AssertCode("sdiv\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcRw_save()
        {
            BuildTest(0x8BE04009);
            AssertCode("save\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcRw_be()
        {
            BuildTest(0x02800001);
            AssertCode("be\t00100004");
        }

        [Test]
        public void SparcRw_fbne()
        {
            BuildTest(0x03800001);
            AssertCode("fbne\t00100004");
        }

        [Test]
        public void SparcRw_jmpl()
        {
            BuildTest(0x8FC07FFF);
            AssertCode("jmpl\t%g1,-1,%g7");
        }

        [Test]
        public void SparcRw_rett()
        {
            BuildTest(0x81C86009);
            AssertCode("rett\t%g1,9");
        }

        [Test]
        public void SparcRw_ta()
        {
            BuildTest(0x91D02999);
            AssertCode("ta\t%g1,0x00000019");
        }

        [Test]
        public void SparcRw_fitos()
        {
            BuildTest(0x8BA0188A);
            AssertCode("fitos\t%f10,%f5");
        }

        [Test]
        public void SparcRw_ldsb()
        {
            BuildTest(0xC248A044);
            AssertCode("ldsb\t[%g2+68],%g1"); 
        }

        [Test]
        public void SparcRw_sth()
        {
            BuildTest(0xC2307FF0);
            AssertCode("sth\t[%g2+68],%g1"); 
        }
    }
}
