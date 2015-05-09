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
    partial class X86RewriterTests : Arch.RewriterTestBase
    {
        private LoadedImage image;
        private IntelArchitecture arch;
        private IntelArchitecture arch16;
        private IntelArchitecture arch32;
        private RewriterHost host;
        private X86State state;
        private Address baseAddr;
        private Address baseAddr16;
        private Address baseAddr32;

        public X86RewriterTests()
        {
            arch16 = new IntelArchitecture(ProcessorMode.Real);
            arch32 = new IntelArchitecture(ProcessorMode.Protected32);
            baseAddr16 = Address.SegPtr(0x0C00, 0x0000);
            baseAddr32 = Address.Ptr32(0x10000000);
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return baseAddr; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            return arch.CreateRewriter(
                new LeImageReader(image, 0),
                arch.CreateProcessorState(),
                frame,
                this.host);
        }

        [SetUp]
        public void Setup()
        {
        }

        private IntelAssembler Create16bitAssembler()
        {
            arch = arch16;
            baseAddr = baseAddr16;
            var asm = new IntelAssembler(arch, baseAddr16, new List<EntryPoint>());
            host = new RewriterHost(asm.ImportReferences);
            return asm;
        }

        private IntelAssembler Create32bitAssembler()
        {
            arch = arch32;
            baseAddr = baseAddr32;
            var asm = new IntelAssembler(arch, baseAddr32, new List<EntryPoint>());
            host = new RewriterHost(asm.ImportReferences);
            return asm;
        }

        private Identifier Reg(IntelRegister r)
        {
            return new Identifier(r.Name, r.DataType, r);
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
            private Dictionary<Address, ImportReference> importThunks;

            public RewriterHost(Dictionary<Address, ImportReference> importThunks)
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

            public ExternalProcedure GetImportedProcedure(Address addrThunk, Address addrInstruction)
            {
                ImportReference p;
                if (importThunks.TryGetValue(addrThunk, out p))
                    throw new NotImplementedException();
                else
                    return null;
            }


            public ExternalProcedure GetInterceptedCall(Address addrImportThunk)
            {
                throw new NotImplementedException();
            }
        }

        private void Run16bitTest(Action<IntelAssembler> fn)
        {
            var m = Create16bitAssembler();
            fn(m);
            image = m.GetImage().Image;
        }

        private void Run32bitTest(Action<IntelAssembler> fn)
        {
            var m = Create32bitAssembler();
            fn(m);
            image = m.GetImage().Image;
        }

        private void Run32bitTest(params byte[] bytes)
        {
            arch = arch32;
            image = new LoadedImage(baseAddr32, bytes);
            host = new RewriterHost(null);
        }

        [Test]
        public void X86Rw_MovAxBx()
        {
            Run16bitTest(m =>
            {
                m.Mov(m.ax, m.bx);
            });
            AssertCode(
                "0|0C00:0000(2): 1 instructions",
                "1|L--|ax = bx");
        }

        private X86Rewriter CreateRewriter32(IntelAssembler m)
        {
            state = new X86State(arch32);
            return new X86Rewriter(arch32, host, state, m.GetImage().Image.CreateLeReader(0), new Frame(arch32.WordWidth));
        }

        private X86Rewriter CreateRewriter32(byte [] bytes)
        {
            state = new X86State(arch32);
            return new X86Rewriter(arch32, host, state, new LeImageReader(image, 0), new Frame(arch32.WordWidth));
        }

        [Test]
        public void X86Rw_MovStackArgument()
        {
            Run16bitTest(m =>
            {
                m.Mov(m.ax, m.MemW(Registers.bp, -8));
            });
            AssertCode(
                "0|0C00:0000(3): 1 instructions",
                "1|L--|ax = Mem0[ss:bp - 0x0008:word16]");
        }

        [Test]
        public void X86Rw_AddToReg()
        {
            Run16bitTest(m =>
            {
                m.Add(m.ax, m.MemW(Registers.si, 4));
            });
            AssertCode(
                "0|0C00:0000(3): 2 instructions",
                "1|L--|ax = ax + Mem0[ds:si + 0x0004:word16]",
                "2|L--|SCZO = cond(ax)");
        }

        [Test]
        public void X86Rw_AddToMem()
        {
            Run16bitTest(m =>
            {
                m.Add(m.WordPtr(0x1000), 3);
            });
            AssertCode(
                "0|0C00:0000(5): 3 instructions",
                "1|L--|v3 = Mem0[ds:0x1000:word16] + 0x0003",
                "2|L--|Mem0[ds:0x1000:word16] = v3",
                "3|L--|SCZO = cond(v3)");
        }

        [Test]
        public void X86Rw_Sub()
        {
            Run16bitTest(m =>
            {
                m.Sub(m.ecx, 0x12345);
            });
            AssertCode(
                "0|0C00:0000(7): 2 instructions",
                "1|L--|ecx = ecx - 0x00012345",
                "2|L--|SCZO = cond(ecx)");
        }

        [Test]
        public void X86Rw_Or()
        {
            Run16bitTest(m =>
            {
                m.Or(m.ax, m.dx);
            });
            AssertCode(
                "0|0C00:0000(2): 3 instructions",
                "1|L--|ax = ax | dx",
                "2|L--|SZO = cond(ax)",
                "3|L--|C = false");
        }

        [Test]
        public void X86Rw_And()
        {
            Run16bitTest(m =>
            {
                m.And(m.si, m.Imm(0x32));
            });
            AssertCode(
                "0|0C00:0000(3): 3 instructions",
                "1|L--|si = si & 0x0032",
                "2|L--|SZO = cond(si)",
                "3|L--|C = false");
        }

        [Test]
        public void X86Rw_Xor()
        {
            Run16bitTest(m =>
            {
                m.Xor(m.eax, m.eax);
            });
            AssertCode(
                "0|0C00:0000(3): 3 instructions",
                "1|L--|eax = eax ^ eax",
                "2|L--|SZO = cond(eax)",
                "3|L--|C = false");
        }

        [Test]
        public void X86Rw_Test()
        {
            Run16bitTest(m =>
            {
                m.Test(m.edi, m.Imm(0xFFFFFFFFu));
            });
            AssertCode(
                "0|0C00:0000(7): 2 instructions",
                "1|L--|SZO = cond(edi & 0xFFFFFFFF)",
                "2|L--|C = false");
        }

        [Test]
        public void X86Rw_Cmp()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Cmp(m.ebx, 3);
            });
            AssertCode(
                "0|0C00:0000(4): 1 instructions",
                "1|L--|SCZO = cond(ebx - 0x00000003)");
        }

        [Test]
        public void X86Rw_PushPop()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Push(m.eax);
                m.Pop(m.ebx);
            });
            AssertCode(
                "0|0C00:0000(2): 2 instructions",
                "1|L--|sp = sp - 0x0004",
                "2|L--|Mem0[ss:sp:word32] = eax",
                "3|0C00:0002(2): 2 instructions",
                "4|L--|ebx = Mem0[ss:sp:word32]",
                "5|L--|sp = sp + 0x0004");
        }

        [Test]
        public void X86Rw_Jmp()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("lupe");
                m.Jmp("lupe");
            });
            AssertCode(
                "0|0C00:0000(3): 1 instructions",
                "1|T--|goto 0C00:0000");
        }

        [Test]
        public void X86Rw_JmpIndirect()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Jmp(m.WordPtr(m.bx, 0x10));
            });
            AssertCode(
                "0|0C00:0000(3): 1 instructions",
                "1|T--|goto Mem0[ds:bx + 0x0010:word16]");
        }

        [Test]
        public void X86Rw_Jne()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("lupe");
                m.Jnz("lupe");
                m.Xor(m.ax, m.ax);
            });
            AssertCode(
                "0|0C00:0000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 0C00:0000",
                "2|0C00:0002(2): 3 instructions",
                "3|L--|ax = ax ^ ax");
        }

        [Test]
        public void X86Rw_Call16bit()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("self");
                m.Call("self");
            });
            AssertCode(
                "0|0C00:0000(3): 1 instructions",
                "1|T--|call 0C00:0000 (2)");
        }

        [Test]
        public void X86Rw_Call32Bit()
        {
            Run32bitTest(delegate(IntelAssembler m)
            {
                m.Label("self");
                m.Call("self");
            });
            AssertCode(
                "0|10000000(5): 1 instructions",
                "1|T--|call 10000000 (4)");
        }

        [Test]
        public void X86Rw_Bswap()
        {
            Run32bitTest(delegate(IntelAssembler m)
            {
                m.Bswap(m.ebx);
            });
            AssertCode(
                "0|10000000(2): 1 instructions",
                "1|L--|ebx = __bswap(ebx)");
        }

        [Test]
        public void X86Rw_IntInstruction()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Mov(m.ax, 0x4C00);
                m.Int(0x21);
            });
            AssertCode(
                "0|0C00:0000(3): 1 instructions",
                "1|L--|ax = 0x4C00",
                "2|0C00:0003(2): 1 instructions",
                "3|L--|__syscall(0x21)");
        }

        [Test]
        public void X86Rw_InInstruction()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.In(m.al, m.dx);
            });
            AssertCode(
                "0|0C00:0000(1): 1 instructions",
                "1|L--|al = __inb(dx)");
        }

        [Test]
        public void X86Rw_RetInstruction()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Ret();
            });
            AssertCode(
                "0|0C00:0000(1): 1 instructions",
                "1|T--|return (2,0)");
        }

        [Test]
        public void X86Rw_RealModeReboot()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.JmpF(Address.SegPtr(0xF000, 0xFFF0));
            });
            AssertCode(
                "0|0C00:0000(5): 1 instructions",
                "1|L--|__bios_reboot()");
        }

        [Test]
        public void X86Rw_RetNInstruction()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Ret(8);
            });
            AssertCode(
                "0|0C00:0000(3): 1 instructions",
                "1|T--|return (2,8)");
        }

        [Test]
        public void X86Rw_Loop()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("lupe");
                m.Loop("lupe");
            });
            AssertCode(
                "0|0C00:0000(2): 2 instructions",
                "1|L--|cx = cx - 0x0001",
                "2|T--|if (cx != 0x0000) branch 0C00:0000");
        }

        [Test]
        public void X86Rw_Loope()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("lupe");
                m.Loope("lupe");
                m.Mov(m.bx, m.ax);
            });
            AssertCode(
                "0|0C00:0000(2): 2 instructions",
                "1|L--|cx = cx - 0x0001",
                "2|T--|if (Test(EQ,Z) && cx != 0x0000) branch 0C00:0000",
                "3|0C00:0002(2): 1 instructions",
                "4|L--|bx = ax");
        }

        [Test]
        public void X86Rw_Adc()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Adc(m.WordPtr(0x100), m.ax);
            });
            AssertCode(
                "0|0C00:0000(4): 3 instructions",
                "1|L--|v5 = Mem0[ds:0x0100:word16] + ax + C",
                "2|L--|Mem0[ds:0x0100:word16] = v5",
                "3|L--|SCZO = cond(v5)");
        }

        [Test]
        public void X86Rw_Lea()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Lea(m.bx, m.MemW(Registers.bx, 4));
            });
            AssertCode(
                "0|0C00:0000(3): 1 instructions",
                "1|L--|bx = bx + 0x0004");
        }

        [Test]
        public void X86Rw_Enter()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Enter(16, 0);
            });
            AssertCode(
                "0|0C00:0000(4): 4 instructions",
                "1|L--|sp = sp - 0x0002",
                "2|L--|Mem0[ss:sp:word16] = bp",
                "3|L--|bp = sp",
                "4|L--|sp = sp - 0x0010");
        }

        [Test]
        public void X86Rw_Neg()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Neg(m.ecx);
            });
            AssertCode(
                "0|0C00:0000(3): 3 instructions",
                "1|L--|ecx = -ecx",
                "2|L--|SCZO = cond(ecx)",
                "3|L--|C = ecx == 0x00000000");
        }

        [Test]
        public void X86Rw_Not()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Not(m.bx);
            });
            AssertCode(
                "0|0C00:0000(2): 1 instructions",
                "1|L--|bx = ~bx");
        }

        [Test]
        public void X86Rw_Out()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Out(m.dx, m.al);
            });
            AssertCode(
                "0|0C00:0000(1): 1 instructions",
                "1|L--|__outb(dx, al)");
        }

        [Test]
        public void X86Rw_Jcxz()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("lupe");
                m.Jcxz("lupe");
            });
            AssertCode(
                "0|0C00:0000(2): 1 instructions",
                "1|T--|if (cx == 0x0000) branch 0C00:0000");
        }

        [Test]
        public void X86Rw_RepLodsw()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Rep();
                m.Lodsw();
                m.Xor(m.ax, m.ax);
            });
            AssertCode(
                "0|0C00:0000(2): 5 instructions",
                "1|T--|if (cx == 0x0000) branch 0C00:0002",
                "2|L--|ax = Mem0[ds:si:word16]",
                "3|L--|si = si + 0x0002",
                "4|L--|cx = cx - 0x0001",
                "5|T--|goto 0C00:0000",
                "6|0C00:0002(2): 3 instructions",
                "7|L--|ax = ax ^ ax",
                "8|L--|SZO = cond(ax)",
                "9|L--|C = false");
        }

        [Test]
        public void X86Rw_Shld()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Shld(m.edx, m.eax, m.cl);
            });
            AssertCode(
                "0|0C00:0000(4): 1 instructions",
                "1|L--|edx = __shld(edx, eax, cl)");
        }

        [Test]
        public void X86Rw_Shrd()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Shrd(m.eax, m.edx, 4);
            });
            AssertCode(
                "0|0C00:0000(5): 1 instructions",
                "1|L--|eax = __shrd(eax, edx, 0x04)");
        }

        [Test]
        public void X86Rw_Fild()
        {
            Run32bitTest(delegate(IntelAssembler m)
            {
                m.Fild(m.MemDw(Registers.ebx, 4));
            });
            AssertCode(
                "0|10000000(3): 1 instructions",
                "1|L--|rLoc1 = (real64) Mem0[ebx + 0x00000004:int32]");
        }

        [Test]
        public void X86Rw_Fstp()
        {
            Run32bitTest(delegate(IntelAssembler m)
            {
                m.Fstp(m.MemDw(Registers.ebx, 4));
            });
            AssertCode(
                "0|10000000(3): 1 instructions",
                "1|L--|Mem0[ebx + 0x00000004:real32] = rArg0");

        }
        [Test]
        public void X86Rw_RepScasb()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Rep();
                m.Scasb();
                m.Ret();
            });
            AssertCode(
                "0|0C00:0000(2): 5 instructions",
                "1|T--|if (cx == 0x0000) branch 0C00:0002",
                "2|L--|SCZO = cond(al - Mem0[es:di:byte])",
                "3|L--|di = di + 0x0001",
                "4|L--|cx = cx - 0x0001",
                "5|T--|if (Test(NE,Z)) branch 0C00:0000",
                "6|0C00:0002(1): 1 instructions",
                "7|T--|return (2,0)");
        }

        [Test]
        public void X86Rw_RewriteLesBxStack()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Les(m.bx, m.MemW(Registers.bp, 6));
            });
            AssertCode(
                "0|0C00:0000(3): 1 instructions",
                "1|L--|es_bx = Mem0[ss:bp + 0x0006:segptr32]");
        }

        private RtlInstruction SingleInstruction(IEnumerator<RtlInstructionCluster> e)
        {
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(1, e.Current.Instructions.Count);
            var instr = e.Current.Instructions[0];
            return instr;
        }

        [Test]
        public void X86Rw_RewriteBswap()
        {
            Run32bitTest(delegate(IntelAssembler m)
            {
                m.Bswap(m.ebx);
            });
            AssertCode(
                "0|10000000(2): 1 instructions",
                "1|L--|ebx = __bswap(ebx)");
        }

        [Test]
        public void X86Rw_RewriteFiadd()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Fiadd(m.WordPtr(m.bx, 0));
            });
            AssertCode(
                "0|0C00:0000(3): 1 instructions",
                "1|L--|rArg0 = rArg0 + (real64) Mem0[ds:bx + 0x0000:word16]");
        }

        /// <summary>
        /// Captures the side effect of setting CF = 0
        /// </summary>
        [Test]
        public void X86Rw_RewriteAnd()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.And(m.ax, m.Const(8));
            });
            AssertCode(
                "0|0C00:0000(3): 3 instructions",
                "1|L--|ax = ax & 0x0008",
                "2|L--|SZO = cond(ax)",
                "3|L--|C = false");
        }

        [Test(Description = "Captures the side effect of setting CF = 0")]
        public void X86Rw_RewriteTest()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Test(m.ax, m.Const(8));
            });
            AssertCode(
                "0|0C00:0000(4): 2 instructions",
                "1|L--|SZO = cond(ax & 0x0008)",
                "2|L--|C = false");
        }

        [Test]
        public void X86Rw_RewriteImul()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Imul(m.cx);
            });
            AssertCode(
                "0|0C00:0000(2): 2 instructions",
                "1|L--|dx_ax = cx *s ax",
                "2|L--|SCZO = cond(dx_ax)");
        }

        [Test]
        public void X86Rw_RewriteMul()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Mul(m.cx);
            });
            AssertCode(
                "0|0C00:0000(2): 2 instructions",
                "1|L--|dx_ax = cx *u ax",
                "2|L--|SCZO = cond(dx_ax)");
        }

        [Test]
        public void X86Rw_RewriteFmul()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Fmul(m.St(1));
            });
            AssertCode(
                "0|0C00:0000(2): 1 instructions",
                "1|L--|rArg0 = rArg0 * rArg1");
        }

        [Test]
        public void X86Rw_RewriteDivWithRemainder()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Div(m.cx);
            });
            AssertCode(
                    "0|0C00:0000(2): 3 instructions",
                    "1|L--|dx = dx_ax % cx",
                    "2|L--|ax = dx_ax /u cx",
                    "3|L--|SCZO = cond(ax)");
        }

        [Test]
        public void X86Rw_RewriteIdivWithRemainder()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Idiv(m.cx);
            });
            AssertCode(
                    "0|0C00:0000(2): 3 instructions",
                    "1|L--|dx = dx_ax % cx",
                    "2|L--|ax = dx_ax / cx",
                    "3|L--|SCZO = cond(ax)");
        }

        [Test]
        public void X86Rw_RewriteBsr()
        {
            Run32bitTest(delegate(IntelAssembler m)
            {
                m.Bsr(m.ecx, m.eax);
            });
            AssertCode(
                "0|10000000(3): 2 instructions",
                "1|L--|Z = eax == 0x00000000",
                "2|L--|ecx = __bsr(eax)");
        }

        [Test]
        public void X86Rw_RewriteIndirectCalls()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Call(m.bx);
                m.Call(m.WordPtr(m.bx, 4));
                m.Call(m.DwordPtr(m.bx, 8));
            });
            AssertCode(
                "0|0C00:0000(2): 1 instructions",
                "1|T--|call SEQ(cs, bx) (2)",
                "2|0C00:0002(3): 1 instructions",
                "3|T--|call SEQ(cs, Mem0[ds:bx + 0x0004:word16]) (2)",
                "4|0C00:0005(3): 1 instructions",
                "5|T--|call Mem0[ds:bx + 0x0008:ptr32] (4)");
        }

        [Test]
        public void X86Rw_RewriteJp()
        {
            Run16bitTest(m =>
            {
                m.Label("foo");
                m.Jpe("foo");
                m.Jpo("foo");
            });
            AssertCode(
                "0|0C00:0000(2): 1 instructions",
                "1|T--|if (Test(PE,P)) branch 0C00:0000",
                "2|0C00:0002(2): 1 instructions",
                "3|T--|if (Test(PO,P)) branch 0C00:0000");
        }

        [Test]
        public void X86Rw_FstswSahf()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Fstsw(m.ax);
                m.Sahf();
            });
            AssertCode(
                "0|0C00:0000(3): 1 instructions",
                "1|L--|SCZO = FPUF");
        }

        [Test]
        public void X86Rw_FstswTestAhEq()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Label("foo");
                m.Fcompp();
                m.Fstsw(m.ax);
                m.Test(m.ah, m.Const(0x44));
                m.Jpe("foo");
            });
            AssertCode(
                "0|0C00:0000(2): 1 instructions",
                "1|L--|FPUF = cond(rArg0 - rArg1)",
                "2|0C00:0002(7): 2 instructions",
                "3|L--|SCZO = FPUF",
                "4|T--|if (Test(NE,FPUF)) branch 0C00:0000");
        }

        [Test]
        public void X86Rw_Sar()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Sar(m.ax, 1);
                m.Sar(m.bx, m.cl);
                m.Sar(m.dx, 4);
            });
            AssertCode(
                "0|0C00:0000(2): 2 instructions",
                "1|L--|ax = ax >> 0x0001",
                "2|L--|SCZO = cond(ax)",
                "3|0C00:0002(2): 2 instructions",
                "4|L--|bx = bx >> cl",
                "5|L--|SCZO = cond(bx)",
                "6|0C00:0004(3): 2 instructions",
                "7|L--|dx = dx >> 0x0004",
                "8|L--|SCZO = cond(dx)");
        }

        [Test]
        public void X86Rw_Xlat16()
        {
            Run16bitTest(delegate(IntelAssembler m)
            {
                m.Xlat();
            });
            AssertCode(
                "0|0C00:0000(1): 1 instructions",
                "1|L--|al = Mem0[ds:bx + (uint16) al:byte]");
        }

        [Test]
        public void X86Rw_Xlat32()
        {
            Run32bitTest(delegate(IntelAssembler m)
            {
                m.Xlat();
            });
            AssertCode(
                "0|10000000(1): 1 instructions",
                "1|L--|al = Mem0[ebx + (uint32) al:byte]");
        }

        [Test]
        public void X86Rw_Aaa()
        {
            Run32bitTest(delegate(IntelAssembler m)
            {
                m.Aaa();
            });
            AssertCode(
                "0|10000000(1): 1 instructions",
                "1|L--|C = __aaa(al, ah, &al, &ah)");
        }

        [Test]
        public void X86Rw_Aam()
        {
            Run32bitTest(delegate(IntelAssembler m)
            {
                m.Aam();
            });
            AssertCode(
                "0|10000000(2): 1 instructions",
                "1|L--|ax = __aam(al)");
        }

        [Test]
        public void X86Rw_Cmpsb()
        {
            Run32bitTest(delegate(IntelAssembler m)
            {
                m.Cmpsb();
            });
            AssertCode(
                "0|10000000(1): 3 instructions",
                "1|L--|SCZO = cond(Mem0[esi:byte] - Mem0[edi:byte])",
                "2|L--|esi = esi + 0x00000001",
                "3|L--|edi = edi + 0x00000001");
        }

        [Test]
        public void X86Rw_Hlt()
        {
            Run32bitTest(delegate(IntelAssembler m)
            {
                m.Hlt();
            });
            AssertCode(
                "0|10000000(1): 1 instructions",
                "1|L--|__hlt()");
        }

        [Test]
        public void X86Rw_Pushf_Popf()
        {
            Run16bitTest((m) =>
            {
                m.Pushf();
                m.Popf();
            });
            AssertCode(
                "0|0C00:0000(1): 2 instructions",
                "1|L--|sp = sp - 0x0002",
                "2|L--|Mem0[ss:sp:word16] = SCZDOP",
                "3|0C00:0001(1): 2 instructions",
                "4|L--|SCZDOP = Mem0[ss:sp:word16]",
                "5|L--|sp = sp + 0x0002");
        }

        [Test]
        public void X86Rw_Std_Lodsw()
        {
            Run32bitTest(m =>
            {
                m.Std();
                m.Lodsw();
                m.Cld();
                m.Lodsw();
            });
            AssertCode(
                "0|10000000(1): 1 instructions",
                "1|L--|D = true",
                "2|10000001(2): 2 instructions",
                "3|L--|ax = Mem0[esi:word16]",
                "4|L--|esi = esi - 0x00000002",
                "5|10000003(1): 1 instructions",
                "6|L--|D = false",
                "7|10000004(2): 2 instructions",
                "8|L--|ax = Mem0[esi:word16]",
                "9|L--|esi = esi + 0x00000002");
        }

        [Test]
        public void X86Rw_les_bx()
        {
            Run16bitTest(m =>
            {
                m.Les(m.bx, m.DwordPtr(m.bx, 0));
            });
            AssertCode(
                "0|0C00:0000(3): 1 instructions",
                "1|L--|es_bx = Mem0[ds:bx + 0x0000:segptr32]");
        }

        [Test]
        public void X86Rw_cmovz()
        {
            Run32bitTest(0x0F, 0x44, 0xC8);
            AssertCode(
                "0|10000000(3): 1 instructions",
                "1|L--|if (Test(EQ,Z)) ecx = eax");
        }

        [Test]
        public void X86Rw_cmp_Ev_Ib()
        {
            Run32bitTest(0x83, 0x3F, 0xFF);
            AssertCode(
                "0|10000000(3): 1 instructions",
                "1|L--|SCZO = cond(Mem0[edi:word32] - 0xFFFFFFFF)");
         }

        [Test]
        public void X86Rw_rol_Eb()
        {
            Run32bitTest(0xC0, 0xC0, 0xC0);
            AssertCode(
                "0|10000000(3): 3 instructions",
                "1|L--|v2 = (al & 0x01 << 0x08 - 0xC0) != 0x00",
                "2|L--|al = __rol(al, 0xC0)",
                "3|L--|C = v2");
         }

        [Test]
        public void X86Rw_pxor_self()
        {
            Run32bitTest(0x0F, 0xEF, 0xC9);
            AssertCode(
                "0|10000000(3): 1 instructions",
                "1|L--|mm1 = (word64) 0");
        }

        [Test]
        public void X86Rw_lock()
        {
            Run32bitTest(0xF0);
            AssertCode(
                  "0|10000000(1): 1 instructions",
                  "1|L--|__lock()");
        }

        [Test]
        public void X86Rw_Cmpxchg()
        {
            Run32bitTest(0x0F, 0xB1, 0x0A); 
            AssertCode(
              "0|10000000(3): 1 instructions",
              "1|L--|Z = __cmpxchg(Mem0[edx:word32], ecx, eax, out eax)");
        }

        [Test]
        public void X86Rw_Xadd()
        {
            Run32bitTest(0x0f, 0xC1, 0xC2);
            AssertCode(
               "0|10000000(3): 2 instructions",
               "1|L--|edx = __xadd(edx, eax)",
               "2|L--|SCZO = cond(edx)");
        }

        [Test]
        public void X86Rw_cvttsd2si()
        {
            Run32bitTest(0xF2, 0x0F, 0x2C, 0xC3);
            AssertCode(
              "0|10000000(4): 1 instructions",
              "1|L--|eax = (int32) xmm3");
        }


        [Test]
        public void X86Rw_fucompp()
        {
            Run32bitTest(0xDA, 0xE9);
            AssertCode(
              "0|10000000(2): 1 instructions",
              "1|L--|FPUF = cond(rArg0 - rArg1)");
        }

        [Test]
        public void X86rw_fs_prefix()
        {
            Run32bitTest(0x64, 0x8B, 0x0A);
               AssertCode(
              "0|10000000(3): 1 instructions",
              "1|L--|ecx = Mem0[fs:edx:word32]");
        }

        [Test]
        public void X86rw_seto()
        {
            Run32bitTest(0x0f, 0x90, 0xc1);
            AssertCode(
                "0|10000000(3): 1 instructions",
                "1|L--|cl = Test(OV,O)");
        }

        [Test]
        public void X86rw_bts()
        {
            Run32bitTest(0x0F, 0xAB, 0x04, 0x24);
            AssertCode(
                "0|10000000(4): 1 instructions",
                "1|L--|C = __bts(Mem0[esp:word32], eax, out Mem0[esp:word32])");
        }

        [Test]
        public void X86rw_cpuid()
        {
            Run32bitTest(0x0F, 0xA2);
            AssertCode(
                "0|10000000(2): 1 instructions",
                "1|L--|__cpuid(eax, ecx, &eax, &ebx, &ecx, &edx)");
        }

        [Test]
        public void X86rw_xgetbv()
        {
            Run32bitTest(0x0F, 0x01, 0xD0);
            AssertCode(
                "0|10000000(3): 1 instructions",
                "1|L--|edx_eax = __xgetbv(ecx)");
        }

        [Test]
        public void X86rw_rdtcs()
        {
            Run32bitTest(0x0F, 0x31);
            AssertCode(
                "0|10000000(2): 1 instructions",
                "1|L--|edx_eax = __rdtsc()");
        }

        [Test]
        public void X86rw_setc()
        {
            Run32bitTest(0x0F, 0x92, 0xC1);
            AssertCode(
                "0|10000000(3): 1 instructions",
                "1|L--|cl = Test(ULT,C)");
        }

        [Test]
        public void X86rw_btr()
        {
            Run32bitTest(0x0F, 0xBA, 0xF3, 0x00);
            AssertCode(
                "0|10000000(4): 1 instructions",
                "1|L--|C = __btr(ebx, 0x00, out ebx)");
        }

        [Test]
        public void X86rw_more_xmm()
        {
            Run32bitTest(0x66, 0x0f, 0x6e, 0xc0);
            AssertCode(
                "0|10000000(4): 1 instructions",
                "1|L--|xmm0 = (word128) eax");
            Run32bitTest(0x66, 0x0f, 0x7e, 0x01);
            AssertCode(
                "0|10000000(4): 1 instructions",
                "1|L--|Mem0[ecx:word32] = (word32) xmm0");
            Run32bitTest(0x66, 0x0f, 0x60, 0xc0);
            AssertCode(
                "0|10000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklbw(xmm0, xmm0)");
            Run32bitTest(0x66, 0x0f, 0x61, 0xc0);
            AssertCode(
                "0|10000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklwd(xmm0, xmm0)");
            Run32bitTest(0x66, 0x0f, 0x70, 0xc0, 0x00);
            AssertCode(
                "0|10000000(5): 1 instructions",
                "1|L--|xmm0 = __pshufd(xmm0, xmm0, 0x00)");
        }
    }
}
