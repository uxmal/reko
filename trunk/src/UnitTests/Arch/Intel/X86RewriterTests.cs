#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Arch.Intel
{

    [TestFixture]
    public partial class X86RewriterTests
    {
        private IntelArchitecture arch;
        private IntelEmitter emitter;

        public X86RewriterTests()
        {
            arch = new IntelArchitecture(ProcessorMode.Real);
            emitter = new IntelEmitter();
        }

        private IntelAssembler Create16bitAssembler()
        {
            return new IntelAssembler(arch, PrimitiveType.Word16, new Address(0xC00, 0x000), emitter, new List<EntryPoint>());
        }

        [Test]
        public void MovAxBx()
        {
            var m = Create16bitAssembler();
            m.Mov(m.ax, m.bx);
            var rw = CreateRewriter(m);
            var e = rw.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(0xC000, e.Current.LinearAddress);
            Assert.AreEqual("ax = bx", e.Current.Instruction.ToString());
        }

        private X86Rewriter CreateRewriter(IntelAssembler m)
        {
            return new X86Rewriter(arch, m.GetImage().CreateReader(0), new Frame(arch.WordWidth));
        }

        [Test]
        public void MovStackArgument()
        {
            var m = Create16bitAssembler();
            m.Mov(m.ax, m.Mem(Registers.bp, -8));
            var e = CreateRewriter(m).GetEnumerator();
            e.MoveNext();
            Assert.AreEqual("ax = Mem0[ss:bp - 0x0008:word16]", e.Current.Instruction.ToString());
        }
    }
}
