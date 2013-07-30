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

using Decompiler.Arch.X86;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
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
        private RewriterHost host;
        private X86State state;
        private Address baseAddr32 = new Address(0x10000000);

        public X86RewriterTests()
        {
            arch = new IntelArchitecture(ProcessorMode.Real);
            arch32 = new IntelArchitecture(ProcessorMode.Protected32);
        }

        [SetUp]
        public void Setup()
        {
            state = (X86State) arch.CreateProcessorState();
        }

        private IntelAssembler Create16bitAssembler()
        {
            var asm = new IntelAssembler(arch, new Address(0xC00, 0x000), new List<EntryPoint>());
            host = new RewriterHost(asm.ImportThunks);
            return asm;
        }

        private IntelAssembler Create32bitAssembler()
        {
            var asm = new IntelAssembler(arch32, baseAddr32, new List<EntryPoint>());
            host = new RewriterHost(asm.ImportThunks);
            return asm;
        }

        private void AssertCode(string expected, IEnumerator<RtlInstructionCluster> e)
        {
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(expected, e.Current.ToString());
        }

        private void AssertCode(IEnumerator<RtlInstructionCluster> e, params string[] expected)
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

        private Identifier Reg(IntelRegister r)
        {
            return new Identifier(r.Name, 0, r.DataType, r);
        }

        private MemoryOperand Mem16(RegisterOperand reg, int offset)
        {
            return new MemoryOperand(PrimitiveType.Word16, reg.Register, Constant.Create(reg.Register.DataType, offset));
        }

        private ImmediateOperand Imm16(ushort u) { return new ImmediateOperand(Constant.Word16(u)); }

        private PrimitiveType Word16 { get { return PrimitiveType.Word16; } }

        private IntelInstruction Instr(Opcode op, PrimitiveType dSize, PrimitiveType aSize, params MachineOperand[] ops)
        {
            return new IntelInstruction(op, dSize, aSize, ops);
        }

        private class RewriterHost : IRewriterHost
        {
            private Dictionary<string, PseudoProcedure> ppp;
            private Dictionary<uint, PseudoProcedure> importThunks;

            public RewriterHost(Dictionary<uint, PseudoProcedure> importThunks)
            {
                this.importThunks = importThunks;
                this.ppp = new Dictionary<string, PseudoProcedure>();
            }

            public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
            {
                PseudoProcedure p;
                if (ppp.TryGetValue(name, out p))
                    return p;
                p = new PseudoProcedure(name, returnType, arity);
                ppp.Add(name, p);
                return p;
            }

            public ProcedureSignature GetCallSignatureAtAddress(Address addrCallInstruction)
            {
                throw new NotImplementedException();
            }

            public PseudoProcedure GetImportThunkAtAddress(uint addrThunk)
            {
                PseudoProcedure p;
                if (importThunks.TryGetValue(addrThunk, out p))
                    return p;
                else
                    return null;
            }
        }

        [Test]
        public void MovAxBx()
        {
            var m = Create16bitAssembler();
            m.Mov(m.ax, m.bx);
            var rw = CreateRewriter(m);
            var e = rw.GetEnumerator();
            AssertCode(e,
                "0|0C00:0000(2): 1 instructions",
                "1|ax = bx");
        }

        private X86Rewriter CreateRewriter(IntelAssembler m)
        {
            return new X86Rewriter(arch, host, state, m.GetImage().Image.CreateReader(0), new Frame(arch.WordWidth));
        }

        private X86Rewriter CreateRewriter32(IntelAssembler m)
        {
            return new X86Rewriter(arch32, host, state, m.GetImage().Image.CreateReader(0), new Frame(arch32.WordWidth));
        }

        private X86Rewriter CreateRewriter32(byte [] bytes)
        {
            var image = new LoadedImage(baseAddr32, bytes);
            return new X86Rewriter(arch32, host, state, new LeImageReader(image, 0), new Frame(arch32.WordWidth));
        }

        [Test]
        public void MovStackArgument()
        {
            var m = Create16bitAssembler();
            m.Mov(m.ax, m.MemW(Registers.bp, -8));
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode(e,
                "0|0C00:0000(3): 1 instructions",
                "1|ax = Mem0[ss:bp - 0x0008:word16]");
        }

        [Test]
        public void AddToReg()
        {
            var m = Create16bitAssembler();
            m.Add(m.ax, m.MemW(Registers.si, 4));
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode(e,
                "0|0C00:0000(3): 2 instructions",
                "1|ax = ax + Mem0[ds:si + 0x0004:word16]",
                "2|SCZO = cond(ax)");
        }

        [Test]
        public void AddToMem()
        {
            var m = Create16bitAssembler();
            m.Add(m.WordPtr(0x1000), 3);
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode(e,
                "0|0C00:0000(5): 3 instructions",
                "1|v3 = Mem0[ds:0x1000:word16] + 0x0003",
                "2|Mem0[ds:0x1000:word16] = v3",
                "3|SCZO = cond(v3)");
        }

        [Test]
        public void Sub()
        {
            var m = Create16bitAssembler();
            m.Sub(m.ecx, 0x12345);
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode(e,
                "0|0C00:0000(7): 2 instructions",
                "1|ecx = ecx - 0x00012345",
                "2|SCZO = cond(ecx)");
        }

        [Test]
        public void Or()
        {
            var m = Create16bitAssembler();
            m.Or(m.ax, m.dx);
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode(e,
                "0|0C00:0000(2): 3 instructions",
                "1|ax = ax | dx",
                "2|SZO = cond(ax)",
                "3|C = false");
        }

        [Test]
        public void And()
        {
            var m = Create16bitAssembler();
            m.And(m.si, m.Imm(0x32));
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode(e,
                "0|0C00:0000(3): 3 instructions",
                "1|si = si & 0x0032",
                "2|SZO = cond(si)",
                "3|C = false");
        }



        [Test]
        public void Xor()
        {
            var m = Create16bitAssembler();
            m.Xor(m.eax, m.eax);
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode(e,
                "0|0C00:0000(3): 3 instructions",
                "1|eax = eax ^ eax",
                "2|SZO = cond(eax)",
                "3|C = false");
        }

        [Test]
        public void Test()
        {
            var m = Create16bitAssembler();
            m.Test(m.edi, m.Imm(0xFFFFFFFFu));
            var e = CreateRewriter(m).GetEnumerator();
            AssertCode(e,
                "0|0C00:0000(7): 2 instructions",
                "1|SZO = cond(edi & 0xFFFFFFFF)",
                "2|C = false");
        }

        private IEnumerator<RtlInstructionCluster> Run16bitTest(Action<IntelAssembler> fn)
        {
            var m = Create16bitAssembler();
            fn(m);
            return CreateRewriter(m).GetEnumerator();
        }

        private IEnumerator<RtlInstructionCluster> Run32bitTest(Action<IntelAssembler> fn)
        {
            var m = Create32bitAssembler();
            fn(m);
            return CreateRewriter32(m).GetEnumerator();
        }

        private IEnumerator<RtlInstructionCluster> Run32bitTest(params byte [] bytes)
        {
            return CreateRewriter32(bytes).GetEnumerator();
        }

        [Test]
        public void Cmp()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Cmp(m.ebx, 3);
            });
            AssertCode(e,
                "0|0C00:0000(4): 1 instructions",
                "1|SCZO = cond(ebx - 0x00000003)");
        }

        [Test]
        public void PushPop()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Push(m.eax);
                m.Pop(m.ebx);
            });
            AssertCode(e,
                "0|0C00:0000(2): 2 instructions",
                "1|sp = sp - 0x0004",
                "2|Mem0[ss:sp:word32] = eax",
                "3|0C00:0002(2): 2 instructions",
                "4|ebx = Mem0[ss:sp:word32]",
                "5|sp = sp + 0x0004");
        }

        [Test]
        public void Jmp()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("lupe");
                m.Jmp("lupe");
            });
            AssertCode(e,
                "0|0C00:0000(3): 1 instructions",
                "1|goto 0C00:0000");
        }

        [Test]
        public void JmpIndirect()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Jmp(m.WordPtr(m.bx, 0x10));
            });
            AssertCode(e,
                "0|0C00:0000(3): 1 instructions",
                "1|goto Mem0[ds:bx + 0x0010:word16]");
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
            AssertCode(e,
                "0|0C00:0000(2): 1 instructions",
                "1|if (Test(NE,Z)) branch 0C00:0000",
                "2|0C00:0002(2): 3 instructions",
                "3|ax = ax ^ ax");
        }

        [Test]
        public void Call16bit()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("self");
                m.Call("self");
            });
            AssertCode(e,
                "0|0C00:0000(3): 1 instructions",
                "1|call 0C00:0000 (2)");
        }

        [Test]
        public void Call32Bit()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Label("self");
                m.Call("self");
            });
            AssertCode(e,
                "0|10000000(5): 1 instructions",
                "1|call 10000000 (4)");
        }

        [Test]
        public void Bswap()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Bswap(m.ebx);
            });
            AssertCode(e,
                "0|10000000(2): 1 instructions",
                "1|ebx = __bswap(ebx)");
        }

        [Test]
        public void IntInstruction()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Mov(m.ax, 0x4C00);
                m.Int(0x21);
            });
            AssertCode(e,
                "0|0C00:0000(3): 1 instructions",
                "1|ax = 0x4C00",
                "2|0C00:0003(2): 1 instructions",
                "3|__syscall(0x21)");
            var s = (RtlSideEffect) e.Current.Instructions[0];
            var app = (Application) s.Expression;
            var pc = (ProcedureConstant) app.Procedure;
            var ppp = (PseudoProcedure) pc.Procedure;
            Assert.AreEqual("__syscall", ppp.Name);
        }

        [Test]
        public void InInstruction()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.In(m.al, m.dx);
            });
            AssertCode(e,
                "0|0C00:0000(1): 1 instructions",
                "1|al = __inb(dx)");
        }

        [Test]
        public void RetInstruction()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Ret();
            });
            AssertCode(e,
                "0|0C00:0000(1): 1 instructions",
                "1|return (2,0)");
        }

        [Test]
        public void RealModeReboot()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.JmpF(new Address(0xF000, 0xFFF0));
            });
            AssertCode(e,
                "0|0C00:0000(5): 1 instructions",
                "1|__bios_reboot()");
        }

        [Test]
        public void RetNInstruction()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Ret(8);
            });
            AssertCode(e,
                "0|0C00:0000(3): 1 instructions",
                "1|return (2,8)");
        }

        [Test]
        public void Loop()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("lupe");
                m.Loop("lupe");
            });
            AssertCode(e,
                "0|0C00:0000(2): 2 instructions",
                "1|cx = cx - 0x0001",
                "2|if (cx != 0x0000) branch 0C00:0000");
        }

        [Test]
        public void Loope()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("lupe");
                m.Loope("lupe");
                m.Mov(m.bx, m.ax);
            });
            AssertCode(e,
                "0|0C00:0000(2): 2 instructions",
                "1|cx = cx - 0x0001",
                "2|if (Test(EQ,Z) && cx != 0x0000) branch 0C00:0000",
                "3|0C00:0002(2): 1 instructions",
                "4|bx = ax");
        }

        [Test]
        public void X86RW_Adc()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Adc(m.WordPtr(0x100), m.ax);
            });
            AssertCode(e,
                "0|0C00:0000(4): 3 instructions",
                "1|v5 = Mem0[ds:0x0100:word16] + ax + C",
                "2|Mem0[ds:0x0100:word16] = v5",
                "3|SCZO = cond(v5)");
        }

        [Test]
        public void Lea()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Lea(m.bx, m.MemW(Registers.bx, 4));
            });
            AssertCode(e,
                "0|0C00:0000(3): 1 instructions",
                "1|bx = bx + 0x0004");
        }

        [Test]
        public void Enter()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Enter(16, 0);
            });
            AssertCode(e,
                "0|0C00:0000(4): 4 instructions",
                "1|sp = sp - 0x0002",
                "2|Mem0[ss:sp:word16] = bp",
                "3|bp = sp",
                "4|sp = sp - 0x0010");
        }

        [Test]
        public void Neg()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Neg(m.ecx);
            });
            AssertCode(e,
                "0|0C00:0000(3): 3 instructions",
                "1|ecx = -ecx",
                "2|SCZO = cond(ecx)",
                "3|C = ecx == 0x00000000");
        }

        [Test]
        public void Not()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Not(m.bx);
            });
            AssertCode(e,
                "0|0C00:0000(2): 1 instructions",
                "1|bx = ~bx");
        }

        [Test]
        public void Out()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Out(m.dx, m.al);
            });
            AssertCode(e,
                "0|0C00:0000(1): 1 instructions",
                "1|__outb(dx, al)");
        }

        [Test]
        public void Jcxz()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("lupe");
                m.Jcxz("lupe");
            });
            AssertCode(e,
                "0|0C00:0000(2): 1 instructions",
                "1|if (cx == 0x0000) branch 0C00:0000");
        }

        [Test]
        public void RepLodsw()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Rep();
                m.Lodsw();
                m.Xor(m.ax, m.ax);
            });
            AssertCode(e,
                "0|0C00:0000(2): 5 instructions",
                "1|if (cx == 0x0000) branch 0C00:0002",
                "2|ax = Mem0[ds:si:word16]",
                "3|si = si + 0x0002",
                "4|cx = cx - 0x0001",
                "5|goto 0C00:0000",
                "6|0C00:0002(2): 3 instructions",
                "7|ax = ax ^ ax",
                "8|SZO = cond(ax)",
                "9|C = false");
        }

        [Test]
        public void Shld()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Shld(m.edx, m.eax, m.cl);
            });
            AssertCode(e,
                "0|0C00:0000(4): 1 instructions",
                "1|edx = __shld(edx, eax, cl)");
        }

        [Test]
        public void Shrd()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Shrd(m.eax, m.edx, 4);
            });
            AssertCode(e,
                "0|0C00:0000(5): 1 instructions",
                "1|eax = __shrd(eax, edx, 0x04)");
        }

        [Test]
        public void Fild()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Fild(m.MemDw(Registers.ebx, 4));
            });
            AssertCode(e,
                "0|10000000(3): 1 instructions",
                "1|rLoc1 = (real64) Mem0[ebx + 0x00000004:int32]");
        }

        [Test]
        public void Fstp()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Fstp(m.MemDw(Registers.ebx, 4));
            });
            AssertCode(e,
                "0|10000000(3): 1 instructions",
                "1|Mem0[ebx + 0x00000004:real32] = rArg0");

        }
        [Test]
        public void RepScasb()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Rep();
                m.Scasb();
                m.Ret();
            });
            AssertCode(e,
                "0|0C00:0000(2): 5 instructions",
                "1|if (cx == 0x0000) branch 0C00:0002",
                "2|SCZO = cond(al - Mem0[es:di:byte])",
                "3|di = di + 0x0001",
                "4|cx = cx - 0x0001",
                "5|if (Test(NE,Z)) branch 0C00:0000",
                "6|0C00:0002(1): 1 instructions",
                "7|return (2,0)");
        }

        [Test]
        public void RewriteLesBxStack()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Les(m.bx, m.MemW(Registers.bp, 6));
            });
            var ass = (RtlAssignment) SingleInstruction(e);
            Assert.AreEqual("es_bx = Mem0[ss:bp + 0x0006:segptr32]", ass.ToString());
            Assert.AreSame(PrimitiveType.SegPtr32, ass.Src.DataType);
        }

        private RtlInstruction SingleInstruction(IEnumerator<RtlInstructionCluster> e)
        {
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(1, e.Current.Instructions.Count);
            var instr = e.Current.Instructions[0];
            return instr;
        }

        [Test]
        public void RewriteBswap()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Bswap(m.ebx);
            });
            Assert.AreEqual("ebx = __bswap(ebx)", SingleInstruction(e).ToString());
        }

        [Test]
        public void RewriteFiadd()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Fiadd(m.WordPtr(m.bx, 0));
            });
            Assert.AreEqual("rArg0 = rArg0 + (real64) Mem0[ds:bx + 0x0000:word16]", SingleInstruction(e).ToString());
        }

        /// <summary>
        /// Captures the side effect of setting CF = 0
        /// </summary>
        [Test]
        public void RewriteAnd()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.And(m.ax, m.Const(8));
            });
            AssertCode(e,
                "0|0C00:0000(3): 3 instructions",
                "1|ax = ax & 0x0008",
                "2|SZO = cond(ax)",
                "3|C = false");
        }

        [Test(Description = "Captures the side effect of setting CF = 0")]
        public void RewriteTest()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Test(m.ax, m.Const(8));
            });
            AssertCode(e,
                "0|0C00:0000(4): 2 instructions",
                "1|SZO = cond(ax & 0x0008)",
                "2|C = false");
        }

        [Test]
        public void RewriteImul()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Imul(m.cx);
            });
            AssertCode(e,
                "0|0C00:0000(2): 2 instructions",
                "1|dx_ax = cx *s ax",
                "2|SCZO = cond(dx_ax)");
        }

        [Test]
        public void RewriteMul()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Mul(m.cx);
            });
            AssertCode(e,
                "0|0C00:0000(2): 2 instructions",
                "1|dx_ax = cx *u ax",
                "2|SCZO = cond(dx_ax)");
        }

        [Test]
        public void RewriteFmul()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Fmul(m.St(1));
            });
            AssertCode(e,
                "0|0C00:0000(2): 1 instructions",
                "1|rArg0 = rArg0 *s rArg1");
        }

        [Test]
        public void RewriteDivWithRemainder()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Div(m.cx);
            });
            AssertCode(e,
                    "0|0C00:0000(2): 3 instructions",
                    "1|dx = dx_ax % cx",
                    "2|ax = dx_ax /u cx",
                    "3|SCZO = cond(ax)");
        }

        [Test]
        public void RewriteIdivWithRemainder()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Idiv(m.cx);
            });
            AssertCode(e,
                    "0|0C00:0000(2): 3 instructions",
                    "1|dx = dx_ax % cx",
                    "2|ax = dx_ax / cx",
                    "3|SCZO = cond(ax)");
        }

        [Test]
        public void RewriteBsr()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Bsr(m.ecx, m.eax);
            });
            AssertCode(e,
                "0|10000000(3): 2 instructions",
                "1|Z = eax == 0x00000000",
                "2|ecx = __bsr(eax)");
        }

        [Test]
        public void RewriteIndirectCalls()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Call(m.bx);
                m.Call(m.WordPtr(m.bx, 4));
                m.Call(m.DwordPtr(m.bx, 8));
            });
            AssertCode(e,
                "0|0C00:0000(2): 1 instructions",
                "1|call SEQ(cs, bx) (2)",
                "2|0C00:0002(3): 1 instructions",
                "3|call SEQ(cs, Mem0[ds:bx + 0x0004:word16]) (2)",
                "4|0C00:0005(3): 1 instructions",
                "5|call Mem0[ds:bx + 0x0008:ptr32] (4)");
        }


        [Test]
        [Ignore("Look at the other jump statement tests.")]
        public void RewriteJp()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                //new IntelInstruction(Opcode.jpe, PrimitiveType.Word16, PrimitiveType.Word16, new ImmediateOperand(new Constant(PrimitiveType.Word32, 0x100))),
                //new IntelInstruction(Opcode.jpo, PrimitiveType.Word16, PrimitiveType.Word16, new ImmediateOperand(new Constant(PrimitiveType.Word32, 0x102)))
            });
            //Assert.AreEqual("branch Test(PE,P) l0C00_0100", rw.Block.Statements[0].Instruction.ToString());
            //Assert.AreEqual("branch Test(PO,P) l0C00_0102", rw.Block.Statements[1].Instruction.ToString());
        }

        [Test]
        public void FstswSahf()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Fstsw(m.ax);
                m.Sahf();
            });
            AssertCode(e,
                "0|0C00:0000(2): 1 instructions",
                "1|ax = (word16) FPUF << 8",
                "2|0C00:0002(1): 1 instructions",
                "3|SCZO = ah");
        }

        [Test]
        [Ignore("The X86rewriter needs to keep a history of past instructions, or do this fstsw elimination at a higher level (ugh).")]
        public void FstswTestAhEq()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("foo");
                m.Fcompp();
                m.Fstsw(m.ax);
                m.Test(m.ah, m.Const(0x44));
                m.Jpe("foo");
            });
            AssertCode(e,
                "0|0C00:0000(2): 1 instructions",
                "1|FPUF = cond(rArg0 - rArg1)",
                "2|0C00:0002(2): 1 instructions",
                "3|ax = (word16) FPUF << 8",
                "4|0C00:0004(3): 2 instructions",
                "5|SZO = cond(ah & 0x44)",
                "6|C = false",
                "7|0C00:0007(2): 1 instructions",
                "8|if (Test(NE,FPUF)) branch 0C00:0000");
        }

        [Test]
        public void Sar()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Sar(m.ax, 1);
                m.Sar(m.bx, m.cl);
                m.Sar(m.dx, 4);
            });
            AssertCode(e,
                "0|0C00:0000(2): 2 instructions",
                "1|ax = ax >> 0x0001",
                "2|SCZO = cond(ax)",
                "3|0C00:0002(2): 2 instructions",
                "4|bx = bx >> cl",
                "5|SCZO = cond(bx)",
                "6|0C00:0004(3): 2 instructions",
                "7|dx = dx >> 0x0004",
                "8|SCZO = cond(dx)");
        }

        [Test]
        public void Xlat16()
        {
            var e = Run16bitTest(delegate(IntelAssembler m)
            {
                m.Xlat();
            });
            AssertCode(e,
                "0|0C00:0000(1): 1 instructions",
                "1|al = Mem0[ds:bx + (uint16) al:byte]");
        }

        [Test]
        public void Xlat32()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Xlat();
            });
            AssertCode(e,
                "0|10000000(1): 1 instructions",
                "1|al = Mem0[ebx + (uint32) al:byte]");
        }

        [Test]
        public void Aaa()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Aaa();
            });
            AssertCode(e,
                "0|10000000(1): 1 instructions",
                "1|C = __aaa(al, ah, &al, &ah)");
        }

        [Test]
        public void Aam()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Aam();
            });
            AssertCode(e,
                "0|10000000(2): 1 instructions",
                "1|ax = __aam(al)");
        }

        [Test]
        public void Cmpsb()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Cmpsb();
            });
            AssertCode(e,
                "0|10000000(1): 3 instructions",
                "1|SCZO = cond(Mem0[esi:byte] - Mem0[edi:byte])",
                "2|esi = esi + 0x00000001",
                "3|edi = edi + 0x00000001");
        }

        [Test]
        public void Hlt()
        {
            var e = Run32bitTest(delegate(IntelAssembler m)
            {
                m.Hlt();
            });
            Assert.IsTrue(e.MoveNext());
            var rtls = e.Current;
            Assert.AreEqual(1, rtls.Instructions.Count);
            var rtl = (RtlSideEffect) rtls.Instructions[0];
            var fnExp = ((Application) rtl.Expression).Procedure;
            var ppp = ((ProcedureConstant) fnExp).Procedure;
            Assert.IsTrue(ppp.Characteristics.Terminates);
        }

        [Test]
        public void Pushf_Popf()
        {
            var e = Run16bitTest((m) =>
            {
                m.Pushf();
                m.Popf();
            });
            AssertCode(e,
                "0|0C00:0000(1): 2 instructions",
                "1|sp = sp - 0x0002",
                "2|Mem0[ss:sp:word16] = SCZDOP",
                "3|0C00:0001(1): 2 instructions",
                "4|SCZDOP = Mem0[ss:sp:word16]",
                "5|sp = sp + 0x0002");
        }

        [Test]
        public void X86RW_Std_Lodsw()
        {
            var e = Run32bitTest(m =>
            {
                m.Std();
                m.Lodsw();
                m.Cld();
                m.Lodsw();
            });
            AssertCode(e,
                "0|10000000(1): 1 instructions",
                "1|D = true",
                "2|10000001(2): 2 instructions",
                "3|ax = Mem0[esi:word16]",
                "4|esi = esi - 0x00000002",
                "5|10000003(1): 1 instructions",
                "6|D = false",
                "7|10000004(2): 2 instructions",
                "8|ax = Mem0[esi:word16]",
                "9|esi = esi + 0x00000002");
        }

        [Test]
        public void X86RW_les_bx()
        {
            var e = Run16bitTest(m =>
            {
                m.Les(m.bx, m.DwordPtr(m.bx, 0));
            });
            AssertCode(e,
                "0|0C00:0000(3): 1 instructions",
                "1|es_bx = Mem0[ds:bx + 0x0000:segptr32]");
        }

        [Test]
        public void X86RW_cmovz()
        {
            var e = Run32bitTest(0x0F, 0x44, 0xC8);
           AssertCode(e,
                "0|10000000(3): 1 instructions",
                "1|if (Test(EQ,Z)) ecx = eax");
        }

        [Test]
        public void X86RW_cmp_Ev_Ib()
        {
            var e = Run32bitTest(0x83, 0x3F, 0xFF);
            AssertCode(e,
                "0|10000000(3): 1 instructions",
                "1|SCZO = cond(Mem0[edi:word32] - 0xFFFFFFFF)");
         }
    }
}
