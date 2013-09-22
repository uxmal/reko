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

using Decompiler.Arch.M68k;
using Decompiler.Core;
using Decompiler.Core.Rtl;
using Decompiler.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.M68k
{
    [TestFixture]
    public class RewriterTests
    {
        private IEnumerable<RtlInstructionCluster> rw;

        private void Rewrite(params ushort[] opcodes)
        {
            byte[] bytes = new byte[opcodes.Length * 2];
            var writer = new BeImageWriter(bytes);
            foreach (ushort opcode in opcodes)
            {
                writer.WriteBeUint16(opcode);
            }
            var image = new LoadedImage(new Address(0x00010000), bytes);

            var arch = new M68kArchitecture();
            rw = arch.CreateRewriter(image.CreateReader(0), arch.CreateProcessorState(), arch.CreateFrame(), new RewriterHost());
        }

        private void AssertCode(params string[] expected)
        {
            var e = rw.GetEnumerator();
            int i = 0;
            while (i < expected.Length && e.MoveNext())
            {
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

        private class RewriterHost : IRewriterHost
        {
            public PseudoProcedure EnsurePseudoProcedure(string name, Decompiler.Core.Types.DataType returnType, int arity)
            {
                throw new NotImplementedException();
            }

            public PseudoProcedure GetImportThunkAtAddress(uint addrThunk)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void M68krw_Movea_l()
        {
            Rewrite(0x2261);        // movea.l   (a1)-,a1
            AssertCode(
                "0|a1 = a1 - 0x00000004",
                "1|a1 = Mem0[a1:word32]");
        }

        [Test]
        public void M68krw_Eor_b()
        {
            Rewrite(0xB103);        // eorb %d0,%d3
            AssertCode(
                "0|v4 = (byte) d3 ^ (byte) d0",
                "1|d3 = DPB(d3, v4, 0, 8)",
                "2|ZN = cond(v4)",
                "3|C = false",
                "4|V = false");
        }        

        [Test]
        public void M68krw_Eor_l()
        {
            Rewrite(0xB183);        // eorl %d0,%d3
            AssertCode(
                "0|d3 = d3 ^ d0",
                "1|ZN = cond(d3)",
                "2|C = false",
                "3|V = false");
        }

        [Test]
        public void M68krw_adda_postinc() // addal (a4)+,%a5
        {
            Rewrite(0xDBDC);
            AssertCode(
                "0|v3 = Mem0[a4:word32]",
                "1|a4 = a4 + 0x00000004",
                "2|a5 = a5 + v3");
        }

        [Test]
        public void M68krw_or_imm()
        {
            Rewrite(0x867c, 0x1123);    // or.w #$1123,d3
            AssertCode(
                "0|v3 = (word16) d3 | 0x1123",
                "1|d3 = DPB(d3, v3, 0, 16)",
                "2|ZN = cond(v3)",
                "3|C = false",
                "4|V = false");
        }

        [Test]
        public void M68krw_movew_indirect()
        {
            Rewrite(0x3410);    // move.w (A0),D2
            AssertCode(
                "0|v4 = Mem0[a0:word16]",
                "1|d2 = DPB(d2, v4, 0, 16)",
                "2|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_move_pre_and_postdec()
        {
            Rewrite(0x36E3);    // move.w -(a3),(a3)+
            AssertCode(
                "0|a3 = a3 - 0x00000002",
                "1|v3 = Mem0[a3:word16]",
                "2|Mem0[a3:word16] = v3",
                "3|a3 = a3 + 0x00000002",
                "4|CVZN = cond(v3)");
        }

        [Test]
        public void M68krw_add_pre()
        {
            Rewrite(0xadd); // add -(r3),r3
        }
    }
}
