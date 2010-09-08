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
        }

        private IntelAssembler Create16bitAssembler()
        {
            emitter = new IntelEmitter();
            return new IntelAssembler(arch, PrimitiveType.Word16, new Address(0xC00, 0x000), emitter, new List<EntryPoint>());
        }

        private void AssertCode(string expected, IEnumerator<RewrittenInstruction> e)
        {
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(expected, e.Current.Instruction.ToString());
        }

        private void AssertCode(uint expectedAddr, string expected, IEnumerator<RewrittenInstruction> e)
        {
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(expectedAddr, e.Current.LinearAddress, "Linear address was not as expected.");
            Assert.AreEqual(expected, e.Current.Instruction.ToString(), "Instruction was not rewritten as expected");
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

        [Test]
        public void AddToReg()
        {
            var m = Create16bitAssembler();
            m.Add(m.ax, m.Mem(Registers.si, 4));
            var e = CreateRewriter(m).GetEnumerator();
            e.MoveNext();
            Assert.AreEqual(0x0C000, e.Current.LinearAddress);
            Assert.AreEqual("ax = ax + Mem0[ds:si + 0x0004:word16]", e.Current.Instruction.ToString());
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(0xC000, e.Current.LinearAddress);
            Assert.AreEqual("SCZO = cond(ax)", e.Current.Instruction.ToString());
        }

        [Test]
        public void AddToMem()
        {
            var m = Create16bitAssembler();
            m.Add(m.WordPtr(0x1000), 3);
            var e = CreateRewriter(m).GetEnumerator();
            e.MoveNext();
            Assert.AreEqual("v3 = Mem0[ds:0x1000:word16] + 0x0003", e.Current.Instruction.ToString());
            e.MoveNext();
            Assert.AreEqual("store(Mem0[ds:0x1000:word16]) = v3", e.Current.Instruction.ToString());
            e.MoveNext();
            Assert.AreEqual("SCZO = cond(v3)", e.Current.Instruction.ToString());
        }

        [Test]
        public void Sub()
        {
            var m = Create16bitAssembler();
            m.Sub(m.ecx, 0x12345);
            var e = CreateRewriter(m).GetEnumerator();
            e.MoveNext();
            Assert.AreEqual("ecx = ecx - 0x00012345", e.Current.Instruction.ToString());
            e.MoveNext();
            Assert.AreEqual("SCZO = cond(ecx)", e.Current.Instruction.ToString());
        }

        [Test]
        public void Or()
        {
            var m = Create16bitAssembler();
            m.Or(m.ax, m.dx);
            var e = CreateRewriter(m).GetEnumerator();
            e.MoveNext();
            Assert.AreEqual("ax = ax | dx", e.Current.Instruction.ToString());
            e.MoveNext();
            Assert.AreEqual("SZO = cond(ax)", e.Current.Instruction.ToString());
            e.MoveNext();
            Assert.AreEqual("C = false", e.Current.Instruction.ToString());
        }

        [Test]
        public void And()
        {
            var m = Create16bitAssembler();
            m.And(m.si, m.Imm(0x32));
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode("si = si & 0x0032", e);
            AssertCode("SZO = cond(si)", e);
            AssertCode("C = false", e);
        }



        [Test]
        public void Xor()
        {
            var m = Create16bitAssembler();
            m.Xor(m.eax, m.eax);
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode("eax = eax ^ eax",e);
            AssertCode("SZO = cond(eax)", e);
            AssertCode("C = false", e);
        }

        [Test]
        public void Test()
        {
            var m = Create16bitAssembler();
            m.Test(m.edi, m.Imm(0xFFFFFFFFu));
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode("SZO = cond(edi & 0xFFFFFFFF)", e);
            AssertCode("C = false", e);
        }

        private IEnumerator<RewrittenInstruction> Run16bitTest(Action<IntelAssembler> fn)
        {
            var m = Create16bitAssembler();
            fn(m);
            return CreateRewriter(m).GetEnumerator();
        }

        [Test]
        public void Cmp()
        {
            var e = Run16bitTest(delegate (IntelAssembler m) 
            {
                m.Cmp(m.ebx, 3);
            });
            AssertCode("SCZO = cond(ebx - 0x00000003)", e);
        }

        [Test]
        public void PushPop()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Push(m.eax);
                m.Pop(m.ebx);
            });
            AssertCode(0xC000, "sp = sp - 0x0004",e );
            AssertCode(0xC000, "store(Mem0[ss:sp:word32]) = eax", e);
            AssertCode(0xC002, "ebx = Mem0[ss:sp:word32]", e);
            AssertCode(0xC002, "sp = sp + 0x0004", e);
        }

        [Test]
        public void Jmp()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("lupe");
                m.Jmp("lupe");
            });
            AssertCode("goto 0C00:0000", e);
        }
    }
}
