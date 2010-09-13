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
        private IntelArchitecture arch32;
        private IntelEmitter emitter;
        private RewriterHost host;

        public X86RewriterTests()
        {
            arch = new IntelArchitecture(ProcessorMode.Real);
            arch32 = new IntelArchitecture(ProcessorMode.ProtectedFlat);
        }

        [SetUp]
        public void Setup()
        {
            host = new RewriterHost();
        }

        private IntelAssembler Create16bitAssembler()
        {
            emitter = new IntelEmitter();
            return new IntelAssembler(arch, PrimitiveType.Word16, new Address(0xC00, 0x000), emitter, new List<EntryPoint>());
        }

        private IntelAssembler Create32bitAssembler()
        {
            emitter = new IntelEmitter();
            return new IntelAssembler(arch32, PrimitiveType.Word32, new Address(0x10000000), emitter, new List<EntryPoint>());
        }

        private void AssertCode(string expected, IEnumerator<RewrittenInstruction> e)
        {
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(expected, e.Current.Instruction.ToString());
        }

        private void AssertCode(uint expectedAddr, string expected, IEnumerator<RewrittenInstruction> e)
        {
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(expectedAddr, e.Current.Address.Linear, "Linear address was not as expected.");
            Assert.AreEqual(expected, e.Current.Instruction.ToString(), "Instruction was not rewritten as expected");
        }

        private class RewriterHost : IRewriterHost2
        {
            private Dictionary<string, PseudoProcedure> ppp = new Dictionary<string,PseudoProcedure>();

            public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
            {
                PseudoProcedure p;
                if (ppp.TryGetValue(name, out p))
                    return p;
                p = new PseudoProcedure(name, returnType, arity);
                ppp.Add(name, p);
                return p;
            }
        }

        [Test]
        public void MovAxBx()
        {
            var m = Create16bitAssembler();
            m.Mov(m.ax, m.bx);
            var rw = CreateRewriter(m);
            var e = rw.GetEnumerator();
            AssertCode(0xC000, "ax = bx", e);
        }

        private X86Rewriter CreateRewriter(IntelAssembler m)
        {
            return new X86Rewriter(host, arch, m.GetImage().CreateReader(0), new Frame(arch.WordWidth));
        }

        private X86Rewriter CreateRewriter32(IntelAssembler m)
        {
            return new X86Rewriter(host, arch32, m.GetImage().CreateReader(0), new Frame(arch32.WordWidth));
        }

        [Test]
        public void MovStackArgument()
        {
            var m = Create16bitAssembler();
            m.Mov(m.ax, m.Mem(Registers.bp, -8));
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode("ax = Mem0[ss:bp - 0x0008:word16]", e);
        }

        [Test]
        public void AddToReg()
        {
            var m = Create16bitAssembler();
            m.Add(m.ax, m.Mem(Registers.si, 4));
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode(0x0C000, "ax = ax + Mem0[ds:si + 0x0004:word16]", e);
            AssertCode(0x0C000, "SCZO = cond(ax)", e);
        }

        [Test]
        public void AddToMem()
        {
            var m = Create16bitAssembler();
            m.Add(m.WordPtr(0x1000), 3);
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode("v3 = Mem0[ds:0x1000:word16] + 0x0003", e);
            AssertCode("store(Mem0[ds:0x1000:word16]) = v3", e);
            AssertCode("SCZO = cond(v3)", e);
        }

        [Test]
        public void Sub()
        {
            var m = Create16bitAssembler();
            m.Sub(m.ecx, 0x12345);
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode("ecx = ecx - 0x00012345", e);
            AssertCode("SCZO = cond(ecx)", e);
        }

        [Test]
        public void Or()
        {
            var m = Create16bitAssembler();
            m.Or(m.ax, m.dx);
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode("ax = ax | dx", e);
            AssertCode("SZO = cond(ax)", e);
            AssertCode("C = false", e);
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
            AssertCode("eax = eax ^ eax", e);
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

        private IEnumerator<RewrittenInstruction> Run32bitTest(Action<IntelAssembler> fn)
        {
            var m = Create32bitAssembler();
            fn(m);
            return CreateRewriter32(m).GetEnumerator();
        }

        [Test]
        public void Cmp()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
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
            AssertCode(0xC000, "sp = sp - 0x0004", e);
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

        [Test]
        public void JmpIndirect()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Jmp(m.WordPtr(m.bx, 0x10));
            });
            AssertCode(0xC000, "goto Mem0[ds:bx + 0x0010:word16]", e);
        }

        [Test]
        public void Jne()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("lupe");
                m.Jnz("lupe");
                m.Xor(m.ax, m.ax);
            });
            AssertCode(0xC000, "if (Test(NE,Z)) goto 0C00:0000", e);
            AssertCode(0xC002, "ax = ax ^ ax", e);
        }

        [Test]
        public void Call16bit()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("self");
                m.Call("self");
            });
            AssertCode(0x0C000, "sp = sp - 0x0002",e);
            AssertCode(0x0C000, "icall 0C00:0000", e);
        }

        [Test]
        public void Call32Bit()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Label("self");
                m.Call("self");
            });
            AssertCode(0x10000000, "esp = esp - 0x00000004", e);
            AssertCode(0x10000000, "icall 0x10000000", e);
        }

        [Test]
        public void Bswap()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Bswap(m.ebx);
            });
            AssertCode("ebx = __bswap(ebx)",e);
        }

        [Test]
        public void IntInstruction()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Mov(m.ax, 0x4C00);
                m.Int(0x21);
            });
            AssertCode("ax = 0x4C00", e);
            AssertCode("__syscall(0x21)", e);
        }
    }
}
