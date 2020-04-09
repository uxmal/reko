#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Msdos;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;

namespace Reko.UnitTests.Arch.X86
{
    [TestFixture]
    partial class X86RewriterTests : Arch.RewriterTestBase
    {
        private IntelArchitecture arch;
        private IntelArchitecture arch16;
        private IntelArchitecture arch32;
        private IntelArchitecture arch64;
        private RewriterHost host;
        private X86State state;
        private Address baseAddr;
        private Address baseAddr16;
        private Address baseAddr32;
        private Address baseAddr64;
        private ServiceContainer sc;

        public X86RewriterTests()
        {
            arch16 = new X86ArchitectureReal("x86-real-16");
            arch32 = new X86ArchitectureFlat32("x86-protected-32");
            arch64 = new X86ArchitectureFlat64("x86-protected-64");
            baseAddr16 = Address.SegPtr(0x0C00, 0x0000);
            baseAddr32 = Address.Ptr32(0x10000000);
            baseAddr64 = Address.Ptr64(0x140000000ul);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => baseAddr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            return arch.CreateRewriter(
                new LeImageReader(mem, 0),
                arch.CreateProcessorState(),
                binder,
                this.host);
        }

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
        }

        private X86Assembler Create16bitAssembler()
        {
            arch = arch16;
            baseAddr = baseAddr16;
            var asm = new X86Assembler(arch, baseAddr16, new List<ImageSymbol>());
            host = new RewriterHost(arch, asm.ImportReferences);
            return asm;
        }

        private X86Assembler Create32bitAssembler()
        {
            arch = arch32;
            baseAddr = baseAddr32;
            var asm = new X86Assembler(arch, baseAddr32, new List<ImageSymbol>());
            host = new RewriterHost(arch, asm.ImportReferences);
            return asm;
        }

        private Identifier Reg(RegisterStorage r)
        {
            return new Identifier(r.Name, r.DataType, r);
        }

        private MemoryOperand Mem16(RegisterOperand reg, int offset)
        {
            return new MemoryOperand(PrimitiveType.Word16, reg.Register, Constant.Create(reg.Register.DataType, offset));
        }

        private ImmediateOperand Imm16(ushort u) { return new ImmediateOperand(Constant.Word16(u)); }

        private PrimitiveType Word16 { get { return PrimitiveType.Word16; } }

        private void Run16bitTest(Action<X86Assembler> fn)
        {
            var m = Create16bitAssembler();
            fn(m);
            Given_MemoryArea(m.GetImage().SegmentMap.Segments.Values.First().MemoryArea);
        }

        private void Run32bitTest(Action<X86Assembler> fn)
        {
            var m = Create32bitAssembler();
            fn(m);
            Given_MemoryArea(m.GetImage().SegmentMap.Segments.Values.First().MemoryArea);
        }

        private void Run16bitTest(params byte[] bytes)
        {
            arch = arch16;
            Given_MemoryArea(new MemoryArea(baseAddr16, bytes));
            host = new RewriterHost(null);
        }

        private void Run32bitTest(params byte[] bytes)
        {
            arch = arch32;
            Given_MemoryArea(new MemoryArea(baseAddr32, bytes));
            host = new RewriterHost(null);
        }

        private void Run64bitTest(params byte[] bytes)
        {
            arch = arch64;
            Given_MemoryArea(new MemoryArea(baseAddr64, bytes));
            host = new RewriterHost(null);
        }

        private void Run64bitTest(string hexBytes)
        {
            arch = arch64;
            Given_MemoryArea(new MemoryArea(baseAddr64, BytePattern.FromHexBytes(hexBytes).ToArray()));
            host = new RewriterHost(null);
        }

        [Test]
        public void X86rw_MovAxBx()
        {
            Run16bitTest(m =>
            {
                m.Mov(m.ax, m.bx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 1 instructions",
                "1|L--|ax = bx");
        }

        private X86Rewriter CreateRewriter32(X86Assembler m)
        {
            var program = m.GetImage();
            state = new X86State(arch32);
            return new X86Rewriter(
                arch32,
                host,
                state,
                program.SegmentMap.Segments.Values.First().MemoryArea.CreateLeReader(0),
                new Frame(arch32.WordWidth));
        }

        [Test]
        public void X86rw_MovStackArgument()
        {
            Run16bitTest(m =>
            {
                m.Mov(m.ax, m.MemW(Registers.bp, -8));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|ax = Mem0[ss:bp - 0x0008:word16]");
        }

        [Test]
        public void X86rw_AddToReg()
        {
            Run16bitTest(m =>
            {
                m.Add(m.ax, m.MemW(Registers.si, 4));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 2 instructions",
                "1|L--|ax = ax + Mem0[ds:si + 0x0004:word16]",
                "2|L--|SCZO = cond(ax)");
        }

        [Test]
        public void X86rw_AddToMem()
        {
            Run16bitTest(m =>
            {
                m.Add(m.WordPtr(0x1000), 3);
            });
            AssertCode(
                "0|L--|0C00:0000(5): 3 instructions",
                "1|L--|v3 = Mem0[ds:0x1000:word16] + 0x0003",
                "2|L--|Mem0[ds:0x1000:word16] = v3",
                "3|L--|SCZO = cond(v3)");
        }

        [Test]
        public void X86rw_Sub()
        {
            Run16bitTest(m =>
            {
                m.Sub(m.ecx, 0x12345);
            });
            AssertCode(
                "0|L--|0C00:0000(7): 2 instructions",
                "1|L--|ecx = ecx - 0x00012345",
                "2|L--|SCZO = cond(ecx)");
        }

        [Test]
        public void X86rw_Or()
        {
            Run16bitTest(m =>
            {
                m.Or(m.ax, m.dx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ax = ax | dx",
                "2|L--|SZO = cond(ax)",
                "3|L--|C = false");
        }

        [Test]
        public void X86rw_And()
        {
            Run16bitTest(m =>
            {
                m.And(m.si, m.Imm(0x32));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 3 instructions",
                "1|L--|si = si & 0x0032",
                "2|L--|SZO = cond(si)",
                "3|L--|C = false");
        }

        [Test]
        public void X86rw_Xor()
        {
            Run16bitTest(m =>
            {
                m.Xor(m.eax, m.eax);
            });
            AssertCode(
                "0|L--|0C00:0000(3): 3 instructions",
                "1|L--|eax = eax ^ eax",
                "2|L--|SZO = cond(eax)",
                "3|L--|C = false");
        }

        [Test]
        public void X86rw_Test()
        {
            Run16bitTest(m =>
            {
                m.Test(m.edi, m.Imm(0xFFFFFFFFu));
            });
            AssertCode(
                "0|L--|0C00:0000(7): 2 instructions",
                "1|L--|SZO = cond(edi & 0xFFFFFFFF)",
                "2|L--|C = false");
        }

        [Test]
        public void X86rw_Cmp()
        {
            Run16bitTest(m =>
            {
                m.Cmp(m.ebx, 3);
            });
            AssertCode(
                "0|L--|0C00:0000(4): 1 instructions",
                "1|L--|SCZO = cond(ebx - 0x00000003)");
        }

        [Test]
        public void X86rw_PushPop()
        {
            Run16bitTest(m =>
            {
                m.Push(m.eax);
                m.Pop(m.ebx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 2 instructions",
                "1|L--|sp = sp - 4",
                "2|L--|Mem0[ss:sp:word32] = eax",
                "3|L--|0C00:0002(2): 2 instructions",
                "4|L--|ebx = Mem0[ss:sp:word32]",
                "5|L--|sp = sp + 4");
        }

        [Test]
        public void X86rw_Jmp()
        {
            Run16bitTest(m =>
            {
                m.Label("lupe");
                m.Jmp("lupe");
            });
            AssertCode(
                "0|T--|0C00:0000(3): 1 instructions",
                "1|T--|goto 0C00:0000");
        }

        [Test]
        public void X86rw_JmpIndirect()
        {
            Run16bitTest(m =>
            {
                m.Jmp(m.WordPtr(m.bx, 0x10));
            });
            AssertCode(
                "0|T--|0C00:0000(3): 1 instructions",
                "1|T--|goto Mem0[ds:bx + 0x0010:word16]");
        }

        [Test]
        public void X86Rw_JmpFarIndirect()
        {
            Run16bitTest(0xFF, 0x6F, 0x34);
            AssertCode(
                "0|T--|0C00:0000(3): 1 instructions",
                "1|T--|goto Mem0[ds:bx + 0x0034:ptr32]");
        }

        [Test]
        public void X86rw_Jne()
        {
            Run16bitTest(m =>
            {
                m.Label("lupe");
                m.Jnz("lupe");
                m.Xor(m.ax, m.ax);
            });
            AssertCode(
                "0|T--|0C00:0000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 0C00:0000",
                "2|L--|0C00:0002(2): 3 instructions",
                "3|L--|ax = ax ^ ax");
        }

        [Test]
        public void X86rw_Call16bit()
        {
            Run16bitTest(m =>
            {
                m.Label("self");
                m.Call("self");
            });
            AssertCode(
                "0|T--|0C00:0000(3): 1 instructions",
                "1|T--|call 0C00:0000 (2)");
        }

        [Test]
        public void X86rw_Call32Bit()
        {
            Run32bitTest(m =>
            {
                m.Label("self");
                m.Call("self");
            });
            AssertCode(
                "0|T--|10000000(5): 1 instructions",
                "1|T--|call 10000000 (4)");
        }

        [Test]
        public void X86rw_Bswap()
        {
            Run32bitTest(m =>
            {
                m.Bswap(m.ebx);
            });
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|ebx = __bswap(ebx)");
        }

        [Test]
        public void X86rw_IntInstruction()
        {
            Run16bitTest(m =>
            {
                m.Mov(m.ax, 0x4C00);
                m.Int(0x21);
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|ax = 0x4C00",
                "2|T--|0C00:0003(2): 1 instructions",
                "3|L--|__syscall(0x21)");
        }

        [Test]
        public void X86rw_InInstruction()
        {
            Run16bitTest(m =>
            {
                m.In(m.al, m.dx);
            });
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|al = __inb(dx)");
        }

        [Test]
        public void X86rw_RetInstruction()
        {
            Run16bitTest(m =>
            {
                m.Ret();
            });
            AssertCode(
                "0|T--|0C00:0000(1): 1 instructions",
                "1|T--|return (2,0)");
        }

        [Test]
        public void X86rw_RealModeReboot()
        {
            Run16bitTest(m =>
            {
                m.JmpF(Address.SegPtr(0xF000, 0xFFF0));
            });
            AssertCode(
                "0|T--|0C00:0000(5): 1 instructions",
                "1|L--|__bios_reboot()");
        }

        [Test]
        public void X86rw_RetNInstruction()
        {
            Run16bitTest(m =>
            {
                m.Ret(8);
            });
            AssertCode(
                "0|T--|0C00:0000(3): 1 instructions",
                "1|T--|return (2,8)");
        }

        [Test]
        public void X86rw_Loop()
        {
            Run16bitTest(m =>
            {
                m.Label("lupe");
                m.Loop("lupe");
            });
            AssertCode(
                "0|T--|0C00:0000(2): 2 instructions",
                "1|L--|cx = cx - 0x0001",
                "2|T--|if (cx != 0x0000) branch 0C00:0000");
        }

        [Test]
        public void X86rw_Loope()
        {
            Run16bitTest(m =>
            {
                m.Label("lupe");
                m.Loope("lupe");
                m.Mov(m.bx, m.ax);
            });
            AssertCode(
                "0|T--|0C00:0000(2): 2 instructions",
                "1|L--|cx = cx - 0x0001",
                "2|T--|if (Test(EQ,Z) && cx != 0x0000) branch 0C00:0000",
                "3|L--|0C00:0002(2): 1 instructions",
                "4|L--|bx = ax");
        }

        [Test]
        public void X86rw_Adc()
        {
            Run16bitTest(m =>
            {
                m.Adc(m.WordPtr(0x100), m.ax);
            });
            AssertCode(
                "0|L--|0C00:0000(4): 3 instructions",
                "1|L--|v5 = Mem0[ds:0x0100:word16] + ax + C",
                "2|L--|Mem0[ds:0x0100:word16] = v5",
                "3|L--|SCZO = cond(v5)");
        }

        [Test]
        public void X86rw_Lea()
        {
            Run16bitTest(m =>
            {
                m.Lea(m.bx, m.MemW(Registers.bx, 4));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|bx = bx + 0x0004");
        }

        [Test]
        public void X86rw_Enter()
        {
            Run16bitTest(m =>
            {
                m.Enter(16, 0);
            });
            AssertCode(
                "0|L--|0C00:0000(4): 4 instructions",
                "1|L--|sp = sp - 2",
                "2|L--|Mem0[ss:sp:word16] = bp",
                "3|L--|bp = sp",
                "4|L--|sp = sp - 16");
        }

        [Test]
        public void X86rw_Neg()
        {
            Run16bitTest(m =>
            {
                m.Neg(m.ecx);
            });
            AssertCode(
                "0|L--|0C00:0000(3): 3 instructions",
                "1|L--|ecx = -ecx",
                "2|L--|SCZO = cond(ecx)",
                "3|L--|C = ecx == 0x00000000");
        }

        [Test]
        public void X86rw_Not()
        {
            Run16bitTest(m =>
            {
                m.Not(m.bx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 1 instructions",
                "1|L--|bx = ~bx");
        }

        [Test]
        public void X86rw_Out()
        {
            Run16bitTest(m =>
            {
                m.Out(m.dx, m.al);
            });
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|__outb(dx, al)");
        }

        [Test]
        public void X86rw_Jcxz()
        {
            Run16bitTest(m =>
            {
                m.Label("lupe");
                m.Jcxz("lupe");
            });
            AssertCode(
                "0|T--|0C00:0000(2): 1 instructions",
                "1|T--|if (cx == 0x0000) branch 0C00:0000");
        }

        [Test]
        public void X86rw_RepLodsw()
        {
            Run16bitTest(m =>
            {
                m.Rep();
                m.Lodsw();
                m.Xor(m.ax, m.ax);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 5 instructions",
                "1|T--|if (cx == 0x0000) branch 0C00:0002",
                "2|L--|ax = Mem0[ds:si:word16]",
                "3|L--|si = si + 0x0002",
                "4|L--|cx = cx - 0x0001",
                "5|T--|goto 0C00:0000",
                "6|L--|0C00:0002(2): 3 instructions",
                "7|L--|ax = ax ^ ax",
                "8|L--|SZO = cond(ax)",
                "9|L--|C = false");
        }

        [Test]
        public void X86rw_Shld()
        {
            Run16bitTest(m =>
            {
                m.Shld(m.edx, m.eax, m.cl);
            });
            AssertCode(
                "0|L--|0C00:0000(4): 1 instructions",
                "1|L--|edx = __shld(edx, eax, cl)");
        }

        [Test]
        public void X86rw_Shrd()
        {
            Run16bitTest(m =>
            {
                m.Shrd(m.eax, m.edx, 4);
            });
            AssertCode(
                "0|L--|0C00:0000(5): 1 instructions",
                "1|L--|eax = __shrd(eax, edx, 0x04)");
        }

        [Test]
        public void X86rw_Fild()
        {
            Run32bitTest(m =>
            {
                m.Fild(m.MemDw(Registers.ebx, 4));
            });
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Top = Top - 1",
                "2|L--|ST[Top:real64] = (real64) Mem0[ebx + 0x00000004:int32]");
        }

        [Test]
        public void X86rw_Fstp()
        {
            Run32bitTest(m =>
            {
                m.Fstp(m.MemDw(Registers.ebx, 4));
            });
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[ebx + 0x00000004:real32] = (real32) ST[Top:real64]",
                "2|L--|Top = Top + 1");
        }
        [Test]
        public void X86rw_RepScasb()
        {
            Run16bitTest(m =>
            {
                m.Rep();
                m.Scasb();
                m.Ret();
            });
            AssertCode(
                "0|L--|0C00:0000(2): 5 instructions",
                "1|T--|if (cx == 0x0000) branch 0C00:0002",
                "2|L--|SCZO = cond(al - Mem0[es:di:byte])",
                "3|L--|di = di + 0x0001",
                "4|L--|cx = cx - 0x0001",
                "5|T--|if (Test(NE,Z)) branch 0C00:0000",
                "6|T--|0C00:0002(1): 1 instructions",
                "7|T--|return (2,0)");
        }

        [Test]
        public void X86rw_RewriteLesBxStack()
        {
            Run16bitTest(m =>
            {
                m.Les(m.bx, m.MemW(Registers.bp, 6));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|es_bx = Mem0[ss:bp + 0x0006:segptr32]");
        }

        private RtlInstruction SingleInstruction(IEnumerator<RtlInstructionCluster> e)
        {
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(1, e.Current.Instructions.Length);
            var instr = e.Current.Instructions[0];
            return instr;
        }

        [Test]
        public void X86rw_RewriteBswap()
        {
            Run32bitTest(m =>
            {
                m.Bswap(m.ebx);
            });
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|ebx = __bswap(ebx)");
        }

        [Test]
        public void X86rw_RewriteFiadd()
        {
            Run16bitTest(m =>
            {
                m.Fiadd(m.WordPtr(m.bx, 0));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|ST[Top:real64] = ST[Top:real64] + (real64) Mem0[ds:bx + 0x0000:word16]");
        }

        /// <summary>
        /// Captures the side effect of setting CF = 0
        /// </summary>
        [Test]
        public void X86rw_RewriteAnd()
        {
            Run16bitTest(m =>
            {
                m.And(m.ax, m.Const(8));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 3 instructions",
                "1|L--|ax = ax & 0x0008",
                "2|L--|SZO = cond(ax)",
                "3|L--|C = false");
        }

        [Test(Description = "Captures the side effect of setting CF = 0")]
        public void X86rw_RewriteTest()
        {
            Run16bitTest(m =>
            {
                m.Test(m.ax, m.Const(8));
            });
            AssertCode(
                "0|L--|0C00:0000(4): 2 instructions",
                "1|L--|SZO = cond(ax & 0x0008)",
                "2|L--|C = false");
        }

        [Test]
        public void X86rw_RewriteImul()
        {
            Run16bitTest(m =>
            {
                m.Imul(m.cx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 2 instructions",
                "1|L--|dx_ax = cx *s ax",
                "2|L--|SCZO = cond(dx_ax)");
        }

        [Test]
        public void X86rw_RewriteMul()
        {
            Run16bitTest(m =>
            {
                m.Mul(m.cx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 2 instructions",
                "1|L--|dx_ax = cx *u ax",
                "2|L--|SCZO = cond(dx_ax)");
        }

        [Test]
        public void X86rw_RewriteFmul()
        {
            Run16bitTest(m =>
            {
                m.Fmul(m.St(1));
            });
            AssertCode(
                "0|L--|0C00:0000(2): 1 instructions",
                "1|L--|ST[Top:real64] = ST[Top:real64] * ST[Top + 1:real64]");
        }

        [Test]
        public void X86rw_RewriteDivWithRemainder()
        {
            Run16bitTest(m =>
            {
                m.Div(m.cx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 4 instructions",
                "1|L--|v5 = dx_ax",
                "2|L--|dx = (uint16) (v5 % cx)",
                "3|L--|ax = (uint16) (v5 /u cx)",
                "4|L--|SCZO = cond(ax)");
        }

        [Test]
        public void X86rw_RewriteIdivWithRemainder()
        {
            Run16bitTest(m =>
            {
                m.Idiv(m.cx);
            });
            AssertCode(
                    "0|L--|0C00:0000(2): 4 instructions",
                    "1|L--|v5 = dx_ax",
                    "2|L--|dx = (int16) (v5 % cx)",
                    "3|L--|ax = (int16) (v5 / cx)",
                    "4|L--|SCZO = cond(ax)");
        }

        [Test]
        public void X86rw_bsr()
        {
            Run32bitTest(m =>
            {
                m.Bsr(m.ecx, m.eax);
            });
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Z = eax == 0x00000000",
                "2|L--|ecx = __bsr(eax)");
        }




        [Test]
        public void X86rw_RewriteIndirectCalls()
        {
            Run16bitTest(m =>
            {
                m.Call(m.bx);
                m.Call(m.WordPtr(m.bx, 4));
                m.Call(m.DwordPtr(m.bx, 8));
            });
            AssertCode(
                "0|T--|0C00:0000(2): 1 instructions",
                "1|T--|call SEQ(0x0C00, bx) (2)",
                "2|T--|0C00:0002(3): 1 instructions",
                "3|T--|call SEQ(0x0C00, Mem0[ds:bx + 0x0004:word16]) (2)",
                "4|T--|0C00:0005(3): 1 instructions",
                "5|T--|call Mem0[ds:bx + 0x0008:ptr32] (4)");
        }

        [Test]
        public void X86rw_RewriteJp()
        {
            Run16bitTest(m =>
            {
                m.Label("foo");
                m.Jpe("foo");
                m.Jpo("foo");
            });
            AssertCode(
                "0|T--|0C00:0000(2): 1 instructions",
                "1|T--|if (Test(PE,P)) branch 0C00:0000",
                "2|T--|0C00:0002(2): 1 instructions",
                "3|T--|if (Test(PO,P)) branch 0C00:0000");
        }

        [Test]
        public void X86rw_FstswSahf()
        {
            Run16bitTest(m =>
            {
                m.Fstsw(m.ax);
                m.Sahf();
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|SCZO = FPUF");
        }

        [Test]
        public void X86rw_FstswTestAhEq()
        {
            Run16bitTest(m =>
            {
                m.Label("foo");
                m.Fcompp();
                m.Fstsw(m.ax);
                m.Test(m.ah, m.Const(0x44));
                m.Jpe("foo");
            });
            AssertCode(
                "0|L--|0C00:0000(2): 2 instructions",
                "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 1:real64])",
                "2|L--|Top = Top + 2",
                "3|T--|0C00:0002(7): 2 instructions",
                "4|L--|SCZO = FPUF",
                "5|T--|if (Test(NE,FPUF)) branch 0C00:0000");
        }

        [Test]
        public void X86rw_FstswTestMov()
        {
            Run32bitTest(m =>
            {
                m.Label("foo");
                m.Fstsw(m.ax);
                m.Test(m.ah, 0x41);
                m.Mov(m.eax, m.DwordPtr(m.esp, 4));
                m.Jnz("foo");
            });
            AssertCode(
                "0|T--|10000000(12): 3 instructions",
                "1|L--|SCZO = FPUF",
                "2|L--|eax = Mem0[esp + 0x00000004:word32]",
                "3|T--|if (Test(LE,FPUF)) branch 10000000");
        }

        [Test]
        public void X86rw_Sar()
        {
            Run16bitTest(m =>
            {
                m.Sar(m.ax, 1);
                m.Sar(m.bx, m.cl);
                m.Sar(m.dx, 4);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 2 instructions",
                "1|L--|ax = ax >> 0x0001",
                "2|L--|SCZO = cond(ax)",
                "3|L--|0C00:0002(2): 2 instructions",
                "4|L--|bx = bx >> cl",
                "5|L--|SCZO = cond(bx)",
                "6|L--|0C00:0004(3): 2 instructions",
                "7|L--|dx = dx >> 0x0004",
                "8|L--|SCZO = cond(dx)");
        }

        [Test]
        public void X86rw_Xlat16()
        {
            Run16bitTest(m =>
            {
                m.Xlat();
            });
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|al = Mem0[ds:bx + (uint16) al:byte]");
        }

        [Test]
        public void X86rw_Xlat32()
        {
            Run32bitTest(m =>
            {
                m.Xlat();
            });
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|al = Mem0[ebx + (uint32) al:byte]");
        }

        [Test]
        public void X86rw_Aaa()
        {
            Run32bitTest(m =>
            {
                m.Aaa();
            });
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|C = __aaa(al, ah, &al, &ah)");
        }

        [Test]
        public void X86rw_Aam()
        {
            Run32bitTest(m =>
            {
                m.Aam();
            });
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|ax = __aam(al)");
        }

        [Test]
        public void X86rw_Cmpsb()
        {
            Run32bitTest(m =>
            {
                m.Cmpsb();
            });
            AssertCode(
                "0|L--|10000000(1): 3 instructions",
                "1|L--|SCZO = cond(Mem0[esi:byte] - Mem0[edi:byte])",
                "2|L--|esi = esi + 0x00000001",
                "3|L--|edi = edi + 0x00000001");
        }

        [Test]
        public void X86rw_Hlt()
        {
            Run32bitTest(m =>
            {
                m.Hlt();
            });
            AssertCode(
                "0|H--|10000000(1): 1 instructions",
                "1|H--|__hlt()");
        }

        [Test]
        public void X86rw_Pushf_Popf()
        {
            Run16bitTest((m) =>
            {
                m.Pushf();
                m.Popf();
            });
            AssertCode(
                "0|L--|0C00:0000(1): 2 instructions",
                "1|L--|sp = sp - 2",
                "2|L--|Mem0[ss:sp:word16] = SCZDOP",
                "3|L--|0C00:0001(1): 2 instructions",
                "4|L--|SCZDOP = Mem0[ss:sp:word16]",
                "5|L--|sp = sp + 2");
        }

        [Test]
        public void X86rw_Std_Lodsw()
        {
            Run32bitTest(m =>
            {
                m.Std();
                m.Lodsw();
                m.Cld();
                m.Lodsw();
            });
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|D = true",
                "2|L--|10000001(2): 2 instructions",
                "3|L--|ax = Mem0[esi:word16]",
                "4|L--|esi = esi - 0x00000002",
                "5|L--|10000003(1): 1 instructions",
                "6|L--|D = false",
                "7|L--|10000004(2): 2 instructions",
                "8|L--|ax = Mem0[esi:word16]",
                "9|L--|esi = esi + 0x00000002");
        }

        [Test]
        public void X86rw_les_bx()
        {
            Run16bitTest(m =>
            {
                m.Les(m.bx, m.DwordPtr(m.bx, 0));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|es_bx = Mem0[ds:bx + 0x0000:segptr32]");
        }

        [Test]
        public void X86rw_cmovz()
        {
            Run32bitTest(0x0F, 0x44, 0xC8);
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 10000003",
                "2|L--|ecx = eax");
        }

        [Test]
        public void X86rw_cmp_Ev_Ib()
        {
            Run32bitTest(0x83, 0x3F, 0xFF);
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|SCZO = cond(Mem0[edi:word32] - 0xFFFFFFFF)");
        }

        [Test]
        public void X86rw_rol_Eb()
        {
            Run32bitTest(0xC0, 0xC0, 0xC0);
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|v2 = (al & 0x01 << 0x08 - 0xC0) != 0x00",
                "2|L--|al = __rol(al, 0xC0)",
                "3|L--|C = v2");
        }

        [Test]
        public void X86rw_pause()
        {
            Run32bitTest(0xF3, 0x90);
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__pause()");
        }

        [Test]
        public void X86rw_pxor_self()
        {
            Run32bitTest(0x0F, 0xEF, 0xC9);
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm1 = (word64) 0");
        }

        [Test]
        public void X86rw_lock()
        {
            Run32bitTest(0xF0);
            AssertCode(
                  "0|L--|10000000(1): 1 instructions",
                  "1|L--|__lock()");
        }

        [Test]
        public void X86rw_Cmpxchg()
        {
            Run32bitTest(0x0F, 0xB1, 0x0A);
            AssertCode(
              "0|L--|10000000(3): 1 instructions",
              "1|L--|Z = __cmpxchg(Mem0[edx:word32], ecx, eax, out eax)");
        }

        [Test]
        public void X86rw_Xadd()
        {
            Run32bitTest(0x0f, 0xC1, 0xC2);
            AssertCode(
               "0|L--|10000000(3): 2 instructions",
               "1|L--|edx = __xadd(edx, eax)",
               "2|L--|SCZO = cond(edx)");
        }

        [Test]
        public void X86rw_cvttsd2si()
        {
            Run32bitTest(0xF2, 0x0F, 0x2C, 0xC3);
            AssertCode(
              "0|L--|10000000(4): 1 instructions",
              "1|L--|eax = (int32) xmm3");
        }


        [Test]
        public void X86rw_fucompp()
        {
            Run32bitTest(0xDA, 0xE9);
            AssertCode(
              "0|L--|10000000(2): 2 instructions",
              "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 1:real64])",
              "2|L--|Top = Top + 2");
        }

        [Test]
        public void X86rw_fs_prefix()
        {
            Run32bitTest(0x64, 0x8B, 0x0A);
            AssertCode(
           "0|L--|10000000(3): 1 instructions",
           "1|L--|ecx = Mem0[fs:edx:word32]");
        }

        [Test]
        public void X86rw_seto()
        {
            Run32bitTest(0x0f, 0x90, 0xc1);
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|cl = Test(OV,O)");
        }

        [Test]
        public void X86rw_bts()
        {
            Run32bitTest(0x0F, 0xAB, 0x04, 0x24);
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|C = __bts(Mem0[esp:word32], eax, out Mem0[esp:word32])");
        }

        [Test]
        public void X86rw_cpuid()
        {
            Run32bitTest(0x0F, 0xA2);
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__cpuid(eax, ecx, &eax, &ebx, &ecx, &edx)");
        }

        [Test]
        public void X86rw_xgetbv()
        {
            Run32bitTest(0x0F, 0x01, 0xD0);
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|edx_eax = __xgetbv(ecx)");
        }



        [Test]
        public void X86rw_setc()
        {
            Run32bitTest(0x0F, 0x92, 0xC1);
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|cl = Test(ULT,C)");
        }

        [Test]
        public void X86rw_btr()
        {
            Run32bitTest(0x0F, 0xBA, 0xF3, 0x00);
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|C = __btr(ebx, 0x00, out ebx)");
        }

        [Test]
        public void X86rw_more_xmm()
        {
            Run32bitTest(0x66, 0x0f, 0x6e, 0xc0);
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = (word128) eax");
            Run32bitTest(0x66, 0x0f, 0x7e, 0x01);
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[ecx:word32] = (word32) xmm0");
            Run32bitTest(0x66, 0x0f, 0x60, 0xc0);
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklbw(xmm0, xmm0)");
            Run32bitTest(0x66, 0x0f, 0x61, 0xc0);
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklwd(xmm0, xmm0)");
            Run32bitTest(0x66, 0x0f, 0x70, 0xc0, 0x00);
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|xmm0 = __pshufd(xmm0, xmm0, 0x00)");
        }

        [Test]
        public void X86rw_64_movsxd()
        {
            Run64bitTest(0x48, 0x63, 0x48, 0x3c); // "movsx\trcx,dword ptr [rax+3C]", 
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|rcx = (int64) Mem0[rax + 0x000000000000003C:word32]");
        }

        [Test]
        public void X86rw_64_rip_relative()
        {
            Run64bitTest(0x49, 0x8b, 0x05, 0x00, 0x00, 0x10, 0x00); // "mov\trax,qword ptr [rip+10000000]",
            AssertCode(
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|rax = Mem0[0x0000000140100007:word64]");
        }


        [Test]
        public void X86rw_64_sub_immediate_dword()
        {
            Run64bitTest(0x48, 0x81, 0xEC, 0x08, 0x05, 0x00, 0x00); // "sub\trsp,+00000508", 
            AssertCode(
                 "0|L--|0000000140000000(7): 2 instructions",
                 "1|L--|rsp = rsp - 1288",
                 "2|L--|SCZO = cond(rsp)");
        }

        [Test]
        public void X86rw_64_repne()
        {
            Run64bitTest(0xF3, 0x48, 0xA5);   // "rep\tmovsd"
            AssertCode(
                 "0|L--|0000000140000000(3): 7 instructions",
                 "1|T--|if (rcx == 0x0000000000000000) branch 0000000140000003",
                 "2|L--|v3 = Mem0[rsi:word64]",
                 "3|L--|Mem0[rdi:word64] = v3",
                 "4|L--|rsi = rsi + 0x0000000000000008",
                 "5|L--|rdi = rdi + 0x0000000000000008",
                 "6|L--|rcx = rcx - 0x0000000000000001",
                 "7|T--|goto 0000000140000000");
        }

        [Test]
        public void X86rw_PIC_idiom()
        {
            Run32bitTest(0xE8, 0, 0, 0, 0, 0x59);        // call $+5, pop ecx
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|ecx = 10000005");
        }

        [Test]
        public void X86rw_invalid_les()
        {
            Run32bitTest(0xC4, 0xC0);
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_push_cs_call_near()
        {
            Run16bitTest(0x0E, 0xE8, 0x42, 0x32);
            AssertCode(
                "0|T--|0C00:0000(4): 2 instructions",
                "1|L--|sp = sp - 2",
                "2|T--|call 0C00:3246 (2)");
        }

        [Test]
        public void X86rw_fstp_real32()
        {
            Run32bitTest(0xd9, 0x1c, 0x24);
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[esp:real32] = (real32) ST[Top:real64]",
                "2|L--|Top = Top + 1");
        }

        [Test]
        public void X86rw_cmpxchg_byte()
        {
            Run32bitTest(0xF0, 0x0F, 0xB0, 0x23); // lock cmpxchg[ebx], ah
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|__lock()",
                "2|L--|10000001(3): 1 instructions",
                "3|L--|Z = __cmpxchg(Mem0[ebx:byte], ah, al, out al)");
        }

        [Test]
        public void X86rw_fld_real32()
        {
            Run16bitTest(0xD9, 0x44, 0x40); // fld word ptr [foo]
            AssertCode(
                "0|L--|0C00:0000(3): 2 instructions",
                "1|L--|Top = Top - 1",
                "2|L--|ST[Top:real64] = (real64) Mem0[ds:si + 0x0040:real32]");
        }

        [Test]
        public void X86rw_movaps()
        {
            Run64bitTest(0x0F, 0x28, 0x05, 0x40, 0x12, 0x00, 0x00); //  movaps xmm0,[rip+00001240]
            AssertCode(
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|xmm0 = Mem0[0x0000000140001247:word128]");
        }

        [Test]
        public void X86rw_idiv()
        {
            Run32bitTest(0xF7, 0x7C, 0x24, 0x04);       // idiv [esp+04]
            AssertCode(
                  "0|L--|10000000(4): 4 instructions",
                  "1|L--|v5 = edx_eax",
                  "2|L--|edx = (int32) (v5 % Mem0[esp + 0x00000004:word32])",
                  "3|L--|eax = (int32) (v5 / Mem0[esp + 0x00000004:word32])",
                  "4|L--|SCZO = cond(eax)");
        }

        [Test]
        public void X86rw_long_nop()
        {
            Run32bitTest(0x66, 0x0f, 0x1f, 0x44, 0x00, 0x00); // nop WORD PTR[eax + eax*1 + 0x0]
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void X86rw_movlhps()
        {
            Run32bitTest(0x0F, 0x16, 0xF3);
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|xmm6[2] = xmm3[0]",
                "2|L--|xmm6[3] = xmm3[1]");
        }

        [Test]
        public void X86rw_fyl2xp1()
        {
            Run16bitTest(0xD9, 0xF9);
            AssertCode(
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ST[Top + 1:real64] = ST[Top + 1:real64] * lg2(ST[Top:real64] + 1.0)",
                "2|L--|FPUF = cond(ST[Top + 1:real64])");
        }

        [Test]
        public void X86rw_fucomi()
        {
            Run32bitTest(0xDB, 0xEB);  // fucomi\tst(0),st(3)
            AssertCode(
               "0|L--|10000000(2): 3 instructions",
               "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 3:real64])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_fucomip()
        {
            Run32bitTest(0xDF, 0xE9);   // fucomip\tst(0),st(1)
            AssertCode(
               "0|L--|10000000(2): 4 instructions",
               "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 1:real64])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_movups()
        {
            Run64bitTest(0x0F, 0x10, 0x45, 0xE0);   // movups xmm0,XMMWORD PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = Mem0[rbp - 0x0000000000000020:word128]");

            Run64bitTest(0x66, 0x0F, 0x10, 0x45, 0xE0);   // movupd xmm0,XMMWORD PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = Mem0[rbp - 0x0000000000000020:word128]");

            Run64bitTest(0x0F, 0x11, 0x44, 0x24, 0x20); // movups\t[rsp+20],xmm0, 
            AssertCode(
                  "0|L--|0000000140000000(5): 1 instructions",
                  "1|L--|Mem0[rsp + 0x0000000000000020:word128] = xmm0");
        }

        [Test]
        public void X86rw_movss()
        {
            Run64bitTest(0xF3, 0x0F, 0x10, 0x45, 0xE0);   // movss xmm0,dword PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = DPB(xmm0, Mem0[rbp - 0x0000000000000020:real32], 0)");
            Run64bitTest(0xF3, 0x0F, 0x11, 0x45, 0xE0);   // movss dword PTR[rbp - 0x20], xmm0,
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|Mem0[rbp - 0x0000000000000020:real32] = (real32) xmm0");
            Run64bitTest(0xF3, 0x0F, 0x10, 0xC3);         // movss xmm0, xmm3,
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = DPB(xmm0, (real32) xmm3, 0)");
        }

        [Test(Description = "Regression reported by @mewmew")]
        public void X86rw_regression1()
        {
            Run32bitTest(0xDB, 0x7C, 0x47, 0x83);       // fstp [esi-0x7D + eax*2]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|Mem0[edi - 0x0000007D + eax * 0x00000002:real80] = (real80) ST[Top:real64]",
                "2|L--|Top = Top + 1");
        }

        [Test]
        public void X86rw_fdivr()
        {
            Run32bitTest(0xDC, 0x3D, 0x78, 0x56, 0x34, 0x12); // fdivr [12345678]
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|ST[Top:real64] = Mem0[0x12345678:real64] / ST[Top:real64]");
        }

        [Test]
        public void X86rw_movsd()
        {
            Run64bitTest(0xF2, 0x0F, 0x10, 0x45, 0xE0);   // movsd xmm0,dword PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = DPB(xmm0, Mem0[rbp - 0x0000000000000020:real64], 0)");
            Run64bitTest(0xF2, 0x0F, 0x11, 0x45, 0xE0);   // movsd dword PTR[rbp - 0x20], xmm0,
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|Mem0[rbp - 0x0000000000000020:real64] = (real64) xmm0");
            Run64bitTest(0xF2, 0x0F, 0x10, 0xC3);   // movsd xmm0, xmm3,
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = DPB(xmm0, (real64) xmm3, 0)");
        }

        [Test(Description = "Intel and AMD state that if you set the low 32-bits of a register in 64-bit mode, they are zero extended.")]
        public void X86rw_64bit_clearHighBits()
        {
            Run64bitTest(0x33, 0xC0);
            AssertCode(
               "0|L--|0000000140000000(2): 3 instructions",
               "1|L--|rax = (uint64) (eax ^ eax)",
               "2|L--|SZO = cond(eax)",
               "3|L--|C = false");
        }

        [Test]
        public void X86rw_ucomiss()
        {
            Run64bitTest(0x0F, 0x2E, 0x05, 0x2D, 0xB1, 0x00, 0x00);
            AssertCode( // ucomiss\txmm0,dword ptr [rip+0000B12D]
               "0|L--|0000000140000000(7): 3 instructions",
               "1|L--|CZP = cond((real32) xmm0 - Mem0[0x000000014000B134:real32])",
               "2|L--|O = false",
               "3|L--|S = false");

        }

        [Test]
        public void X86rw_ucomisd()
        {
            Run64bitTest(0x66, 0x0F, 0x2E, 0x05, 0x2D, 0xB1, 0x00, 0x00);
            AssertCode( // ucomisd\txmm0,qword ptr [rip+0000B12D]
               "0|L--|0000000140000000(8): 3 instructions",
               "1|L--|CZP = cond((real64) xmm0 - Mem0[0x000000014000B135:real64])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_addss()
        {
            Run64bitTest(0xF3, 0x0F, 0x58, 0x0D, 0xFB, 0xB0, 0x00, 0x00);
            AssertCode( //addss\txmm1,dword ptr [rip+0000B0FB]
               "0|L--|0000000140000000(8): 2 instructions",
               "1|L--|v3 = (real32) xmm1 + Mem0[0x000000014000B103:real32]",
               "2|L--|xmm1 = DPB(xmm1, v3, 0)");
        }

        [Test]
        public void X86rw_subss()
        {
            Run64bitTest(0xF3, 0x0F, 0x5C, 0xCD);
            AssertCode(     // subss\txmm1,dword ptr [rip+0000B0FB]
               "0|L--|0000000140000000(4): 2 instructions",
               "1|L--|v3 = (real32) xmm1 - xmm5",
               "2|L--|xmm1 = DPB(xmm1, v3, 0)");
    }

        [Test]
        public void X86rw_cvtsi2ss()
        {
            Run64bitTest(0xF3, 0x48, 0x0F, 0x2A, 0xC0);
            AssertCode(     // "cvtsi2ss\txmm0,rax", 
               "0|L--|0000000140000000(5): 2 instructions",
               "1|L--|v4 = (real32) rax",
               "2|L--|xmm0 = DPB(xmm0, v4, 0)");
        }

        [Test]
        public void X86rw_cvtpi2ps()
        {
            Run64bitTest("49 0F 2A C5");
            AssertCode( // cvtpi2ps xmm0, mm5
               "0|L--|0000000140000000(4): 3 instructions",
               "1|L--|v3 = (real32) SLICE(mm5, int32, 0)",
               "2|L--|v4 = (real32) SLICE(mm5, int32, 32)",
               "3|L--|xmm0 = SEQ(v4, v3)");
        }

        [Test]
        public void X86rw_addps()
        {
            Run64bitTest("0F 58 0D FB B0 00 00");
            AssertCode( // addps xmm1,[0000000000415EF4]
                "0|L--|0000000140000000(7): 3 instructions",
                "1|L--|v3 = xmm1",
                "2|L--|v4 = Mem0[0x000000014000B102:word128]",
                "3|L--|xmm1 = __addps(v3, v4)");
        }

        [Test]
        public void X86rw_divsd()
        {
            Run64bitTest("F2 0F 5E C1");
            AssertCode( // divsd xmm0,xmm1
               "0|L--|0000000140000000(4): 2 instructions",
               "1|L--|v3 = (real64) xmm0 / xmm1",
               "2|L--|xmm0 = DPB(xmm0, v3, 0)");
        }

        [Test]
        public void X86rw_mulsd()
        {
            Run64bitTest("F2 0F 59 05 92 AD 00 00 ");
            AssertCode(     // mulsd xmm0,qword ptr[rip + ad92]
               "0|L--|0000000140000000(8): 2 instructions",
               "1|L--|v3 = (real64) xmm0 * Mem0[0x000000014000AD9A:real64]",
               "2|L--|xmm0 = DPB(xmm0, v3, 0)");
        }

        [Test]
        public void X86rw_subps()
        {
            Run64bitTest("0F 5C 05 61 AA 00 00");
            AssertCode( // subps xmm0,[0000000000415F0C]
               "0|L--|0000000140000000(7): 3 instructions",
               "1|L--|v3 = xmm0",
               "2|L--|v4 = Mem0[0x000000014000AA68:word128]",
               "3|L--|xmm0 = __subps(v3, v4)");
        }

        [Test]
        public void X86rw_cvttss2si()
        {
            Run64bitTest("F3 4C 0F 2C F8");
            AssertCode(     // cvttss2si r15d, xmm0
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|r15d = (int32) xmm0");
        }

        [Test]
        public void X86rw_mulps()
        {
            Run64bitTest("0F 59 4A 08");
            AssertCode(     // mulps xmm1,[rdx+08]
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v4 = xmm1",
                "2|L--|v5 = Mem0[rdx + 0x0000000000000008:word128]",
                "3|L--|xmm1 = __mulps(v4, v5)");
        }

        [Test]
        public void X86rw_mulss()
        {
            Run64bitTest("F3 0F 59 D8");
            AssertCode(     // mulss xmm3, xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = (real32) xmm3 * xmm0",
                "2|L--|xmm3 = DPB(xmm3, v3, 0)");
        }

        [Test]
        public void X86rw_divss()
        {
            Run64bitTest("F3 0F 5E C1");
            AssertCode(     // divss xmm0, xmm1
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = (real32) xmm0 / xmm1",
                "2|L--|xmm0 = DPB(xmm0, v3, 0)");
        }

        [Test(Description = "RET n instructions with an odd n are unlikely to be valid.")]
        public void X86rw_invalid_ret_n()
        {
            Run64bitTest("C2 01 00");
            AssertCode(     // ret 0001
                "0|---|0000000140000000(3): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_fptan()
        {
            Run16bitTest(0xD9, 0xF2);
            AssertCode(     // fptan
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ST[Top:real64] = tan(ST[Top:real64])",
                "2|L--|Top = Top - 1",
                "3|L--|ST[Top:real64] = 1.0");
        }

        [Test]
        public void X86rw_f2xm1()
        {
            Run16bitTest(0xD9, 0xF0);
            AssertCode(     // f2xm1
                "0|L--|0C00:0000(2): 1 instructions",
                "1|L--|ST[Top:real64] = pow(2.0, ST[Top:real64]) - 1.0");
        }

        [Test]
        public void X86rw_fpu_load()
        {
            Run64bitTest(0xD8, 0x0D, 0x89, 0x9F, 0x00, 0x00);
            AssertCode(     // fmul
              "0|L--|0000000140000000(6): 1 instructions",
              "1|L--|ST[Top:real64] = ST[Top:real64] * Mem0[0x0000000140009F8F:real32]");
        }

        [Test]
        public void X86rw_fninit()
        {
            Run32bitTest(0xDB, 0xE3);
            AssertCode(     // fninit
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__fninit()");
        }

        [Test]
        public void X86rw_x64_push_immediate()
        {
            Run64bitTest(0x6A, 0xC2);
            AssertCode(     // "push 0xC2", 
                "0|L--|0000000140000000(2): 2 instructions",
                "1|L--|rsp = rsp - 8",
                "2|L--|Mem0[rsp:word64] = 0xFFFFFFFFFFFFFFC2");
        }

        [Test]
        public void X86rw_x64_push_register()
        {
            Run64bitTest(0x53);
            AssertCode(     // "push rbx", 
                "0|L--|0000000140000000(1): 2 instructions",
                "1|L--|rsp = rsp - 8",
                "2|L--|Mem0[rsp:word64] = rbx");
        }

        [Test]
        public void X86rw_x64_push_memoryload()
        {
            Run64bitTest(0xFF, 0x75, 0xE0);
            AssertCode(     // "push rbx", 
                "0|L--|0000000140000000(3): 3 instructions",
                "1|L--|v4 = Mem0[rbp - 0x0000000000000020:word64]",
                "2|L--|rsp = rsp - 8",
                "3|L--|Mem0[rsp:word64] = v4");
        }

        [Test]
        public void X86rw_push_segreg()
        {
            Run32bitTest(0x06);
            AssertCode(     // "push es", 
                "0|L--|10000000(1): 2 instructions",
                "1|L--|esp = esp - 2",
                "2|L--|Mem0[esp:word16] = es");
        }

        [Test]
        public void X86rw_mfence()
        {
            Run32bitTest(0x0F, 0xAE, 0xF0);   // mfence
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|__mfence()");
        }

        [Test]
        public void X86rw_lfence()
        {
            Run32bitTest(0x0F, 0xAE, 0xE8); // lfence
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|__lfence()");
        }

        [Test]
        public void X86rw_prefetch()
        {
            Run64bitTest(0x41, 0x0F, 0x18, 0x08); // prefetch
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|__prefetcht0(Mem0[r8:byte])");
        }

        [Test]
        public void X86rw_xorps()
        {
            Run64bitTest(0x0F, 0x57, 0xC0); // xorps\txmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(3): 3 instructions",
                "1|L--|v3 = xmm0",
                "2|L--|v4 = xmm0",
                "3|L--|xmm0 = __xorps(v3, v4)");
        }

        [Test]
        public void X86rw_aesimc()
        {
            Run32bitTest(0x66, 0x0F, 0x38, 0xDB, 0xC0); // aesimc\txmm0,xmm0
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|xmm0 = __aesimc(xmm0)");
        }

        [Test]
        public void X86rw_vmovss_load()
        {
            Run64bitTest(0xC5, 0xFA, 0x10, 0x05, 0x51, 0x03, 0x00, 0x00); // vmovss txmm0,dword ptr [rip+00000351]
            AssertCode(
                "0|L--|0000000140000000(8): 1 instructions",
                "1|L--|xmm0 = DPB(xmm0, Mem0[0x0000000140000359:real32], 0)");
        }

        [Test]
        public void X86rw_vmovss_store()
        {
            Run64bitTest(0xC5, 0xFA, 0x11, 0x85, 0x2C, 0xFF, 0xFF, 0xFF); // vmovss dword ptr [rbp-0xd4], xmm0
            AssertCode(
                "0|L--|0000000140000000(8): 1 instructions",
                "1|L--|Mem0[rbp - 0x00000000000000D4:real32] = (real32) xmm0");
        }

        [Test]
        public void X86rw_vcvtsi2ss()
        {
            Run64bitTest(0xC4, 0xE1, 0xFA, 0x2A, 0xC0);     // vcvtsi2ss\txmm0,xmm0,rax
            AssertCode(
             "0|L--|0000000140000000(5): 2 instructions",
             "1|L--|v4 = (real32) rax",
             "2|L--|xmm0 = DPB(xmm0, v4, 0)");
        }

        [Test]
        public void X86rw_vcvtsi2sd()
        {
            Run64bitTest(0xC4, 0xE1, 0xFB, 0x2A, 0xC2); // vcvtsi2sd\txmm0,xmm0,rdx
            AssertCode(
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = (real64) rdx",
                "2|L--|xmm0 = DPB(xmm0, v4, 0)");
        }

        [Test]
        public void X86rw_vmovsd()
        {
            Run64bitTest(0xC5, 0xFB, 0x11, 0x01); // vmovsd double ptr[rcx], xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|Mem0[rcx:real64] = (real64) xmm0");
        }

        [Test]
        public void X86rw_vmovaps_xmm()
        {
            Run64bitTest(0xC5, 0xF9, 0x28, 0x00); // vmovaps xmm0,[rax]
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = Mem0[rax:word128]");
        }

        [Test]
        public void X86rw_vmovaps_ymm()
        {
            Run64bitTest(0xC5, 0xFD, 0x28, 0x00); // vmovaps ymm0,[rax]
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ymm0 = Mem0[rax:word256]");
        }

        [Test]
        public void X86rw_vaddsd()
        {
            Run64bitTest(0xC5, 0xFB, 0x58, 0xC0);   // vaddsd xmm0,xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = (real64) xmm0 + (real64) xmm0",
                "2|L--|xmm0 = DPB(xmm0, v3, 0)");
        }

        [Test]
        public void X86rw_vxorpd()
        {
            Run64bitTest(0xC5, 0xF9, 0x57, 0xC0);   // vxorpd xmm0,xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v3 = xmm0",
                "2|L--|v4 = xmm0",
                "3|L--|xmm0 = __xorpd(v3, v4)");
        }

        [Test]
        public void X86rw_vxorpd_mem_256()
        {
            Run64bitTest(0xC5, 0xFD, 0x57, 0x09);   // vxorpd\tymm1,ymm0,[rcx]
            AssertCode(
             "0|L--|0000000140000000(4): 3 instructions",
             "1|L--|v5 = ymm0",
             "2|L--|v6 = Mem0[rcx:word256]",
             "3|L--|ymm1 = __xorpd(v5, v6)");
        }

        // new instructions

        [Test]
        public void X86rw_andnps()
        {
            Run32bitTest(0x0F, 0x55, 0x42, 0x42);    // andnps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __andnps(xmm0, Mem0[edx + 0x00000042:word128])");
    }

        [Test]
        public void X86rw_andps()
        {
            Run32bitTest(0x0F, 0x54, 0x42, 0x42);    // andps\txmm0,[edx+42]
            AssertCode(
               "0|L--|10000000(4): 3 instructions",
               "1|L--|v4 = xmm0",
               "2|L--|v5 = Mem0[edx + 0x00000042:word128]",
               "3|L--|xmm0 = __andps(v4, v5)");
        }

        [Test]
        public void X86rw_bsf()
        {
            Run32bitTest(0x0F, 0xBC, 0x42, 0x42);    // bsf\teax,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|Z = Mem0[edx + 0x00000042:word32] == 0x00000000",
                "2|L--|eax = __bsf(Mem0[edx + 0x00000042:word32])");
        }

        [Test]
        public void X86rw_btc()
        {
            Run32bitTest(0x0F, 0xBB, 0x42, 0x42);    // btc\teax,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|C = __btc(eax, Mem0[edx + 0x00000042:word32], out eax)");
        }

        [Test]
        public void X86rw_clts()
        {
            Run32bitTest(0x0F, 0x06);    // clts
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|cr0 = __clts(cr0)");
        }

        [Test]
        public void X86rw_cmpps()
        {
            Run32bitTest(0x0F, 0xC2, 0x42, 0x42, 0x08);    // cmpps\txmm0,[edx+42],08
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word128]",
                "3|L--|xmm0 = __cmpps(v4, v5, 0x08)");
        }

        [Test]
        public void X86rw_comiss()
        {
            Run32bitTest(0x0F, 0x2F, 0x42, 0x42);    // comiss\txmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|CZP = cond((real32) xmm0 - Mem0[edx + 0x00000042:real32])",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86rw_cvtdq2ps()
        {
            Run32bitTest(0x0F, 0x5B, 0x42, 0x42);    // cvtdq2ps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v3 = Mem0[edx + 0x00000042:word128]",
                "2|L--|xmm0 = __cvtdq2ps(v3)");
        }

        [Test]
        public void X86rw_cvtps2pd()
        {
            Run32bitTest(0x0F, 0x5A, 0x42, 0x42);    // cvtps2pd\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v3 = Mem0[edx + 0x00000042:word128]",
                "2|L--|xmm0 = __cvtps2pd(v3)");
        }

        [Test]
        public void X86rw_cvtps2pi()
        {
            Run32bitTest(0x0F, 0x2D, 0x42, 0x42);    // cvtps2pi\tmm0,xmmword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v3 = Mem0[edx + 0x00000042:word128]",
                "2|L--|mm0 = __cvtps2pi(v3)");
        }

        [Test]
        public void X86rw_emms()
        {
            Run32bitTest(0x0F, 0x77);    // emms
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__emms()");
        }

        [Test]
        public void X86rw_fclex()
        {
            Run32bitTest(0xDB, 0xE2);    // fclex
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__fclex()");
        }

        [Test]
        public void X86rw_fcmovb()
        {
            Run32bitTest(0xDA, 0xC1);    // fcmovb\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(GE,C)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1:real64]");
        }

        [Test]
        public void X86rw_fcmovbe()
        {
            Run32bitTest(0xDA, 0xD1);    // fcmovbe\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(GT,CZ)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1:real64]");
        }

        [Test]
        public void X86rw_fcmove()
        {
            Run32bitTest(0xDA, 0xC9);    // fcmove\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1:real64]");
        }

        [Test]
        public void X86rw_fcmovnbe()
        {
            Run32bitTest(0xDB, 0xD1);    // fcmovnbe\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(LE,CZ)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1:real64]");
        }

        [Test]
        public void X86rw_fcmovne()
        {
            Run32bitTest(0xDB, 0xC9);    // fcmovne\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1:real64]");
        }

        [Test]
        public void X86rw_fcmovnu()
        {
            Run32bitTest(0xDB, 0xD9);    // fcmovnu\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(IS_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1:real64]");
        }

        [Test]
        public void X86rw_fcmovu()
        {
            Run32bitTest(0xDA, 0xD9);    // fcmovu\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NOT_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1:real64]");
        }

        [Test]
        public void X86rw_fcomip()
        {
            Run32bitTest(0xDF, 0xF2);    // fcomip\tst(0),st(2)
            AssertCode(
                "0|L--|10000000(2): 4 instructions",
                "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 2:real64])",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86rw_ffree()
        {
            Run32bitTest(0xDD, 0xC2);    // ffree\tst(2)
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__ffree(ST[Top + 2:real64])");
        }

        [Test]
        public void X86rw_fild_i16()
        {
            Run32bitTest(0xDF, 0x40, 0x42);    // fild\tword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Top = Top - 1",
                "2|L--|ST[Top:real64] = (real64) Mem0[eax + 0x00000042:int16]");
        }

        [Test]
        public void X86rw_fisttp()
        {
            Run32bitTest(0xDB, 0x08);    // fisttp\tdword ptr [eax]
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|Mem0[eax:int32] = (int32) trunc(ST[Top:real64])",
                "2|L--|Top = Top + 1");
        }

        [Test]
        public void X86rw_fisttp_int16()
        {
            Run32bitTest(0xDF, 0x48, 0x42);    // fisttp\tword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[eax + 0x00000042:int16] = (int16) trunc(ST[Top:real64])",
                "2|L--|Top = Top + 1");
        }

        [Test]
        public void X86rw_fisttp_int64()
        {
            Run32bitTest(0xDD, 0x48, 0x42);    // fisttp\tqword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[eax + 0x00000042:int64] = (int64) trunc(ST[Top:real64])");
        }

        [Test]
        public void X86rw_fld_real80()
        {
            Run32bitTest(0xDB, 0x28);    // fld\ttword ptr [eax]
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|Top = Top - 1",
                "2|L--|ST[Top:real64] = (real64) Mem0[eax:real80]");
        }

        [Test]
        public void X86rw_fucom()
        {
            Run32bitTest(0xDD, 0xE5);    // fucom\tst(5),st(0)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 5:real64])");
        }

        [Test]
        public void X86rw_fucomp()
        {
            Run32bitTest(0xDD, 0xEA);    // fucomp\tst(2)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 2:real64])");
        }

        [Test]
        public void X86rw_invd()
        {
            Run32bitTest(0x0F, 0x08);    // invd
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__invd()");
        }

        [Test]
        public void X86rw_lar()
        {
            Run32bitTest(0x0F, 0x02, 0x42, 0x42);    // lar\teax,word ptr [edx+42]
            AssertCode(
                "0|S--|10000000(4): 2 instructions",
                "1|L--|eax = __lar(Mem0[edx + 0x00000042:word16])",
                "2|L--|Z = true");
        }

        [Test]
        public void X86rw_lsl()
        {
            Run32bitTest(0x0F, 0x03, 0x42, 0x42);    // lsl\teax,word ptr [edx+42]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|eax = __lsl(Mem0[edx + 0x00000042:word16])");
        }

        [Test]
        public void X86rw_maskmovq()
        {
            Run32bitTest(0x0F, 0xF7, 0x42, 0x42);    // maskmovq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __maskmovq(mm0, Mem0[edx + 0x00000042:word64])");
        }

        [Test]
        public void X86rw_minps()
        {
            Run32bitTest(0x0F, 0x5D, 0x42, 0x42);    // minps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word128]",
                "3|L--|xmm0 = __minps(v4, v5)");
        }

        [Test]
        public void X86rw_syscall()
        {
            Run64bitTest(0x0F, 0x05);    // syscall
            AssertCode(
                "0|T--|0000000140000000(2): 1 instructions",
                "1|L--|__syscall()");
            Run32bitTest(0x0F, 0x05);    // illegal
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_sysenter()
        {
            Run32bitTest(0x0F, 0x34);    // sysenter
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__sysenter()");
        }

        [Test]
        public void X86rw_sysexit()
        {
            Run32bitTest(0x0F, 0x35);    // sysexit
            AssertCode(
                "0|T--|10000000(2): 2 instructions",
                "1|L--|__sysexit()",
                "2|T--|return (0,0)");
        }

        [Test]
        public void X86rw_sysret()
        {
            Run64bitTest(0x0F, 0x07);    // sysret
            AssertCode(
                "0|T--|0000000140000000(2): 2 instructions",
                "1|L--|__sysret()",
                "2|T--|return (0,0)");
            Run32bitTest(0x0F, 0x07);    // illegal
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_ud2()
        {
            Run32bitTest(0x0F, 0x0B);    // ud2
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_unpcklps()
        {
            Run32bitTest(0x0F, 0x14, 0x42, 0x42);    // unpcklps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word128]",
                "3|L--|xmm0 = __unpcklps(v4, v5)");
            Run32bitTest(0x66, 0x0F, 0x14, 0x42, 0x42);    // unpcklpd\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word128]",
                "3|L--|xmm0 = __unpcklpd(v4, v5)");
        }


        [Test]
        public void X86rw_wbinvd()
        {
            Run32bitTest(0x0F, 0x09);    // wbinvd
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__wbinvd()");
        }

        [Test]
        public void X86rw_prefetchw()
        {
            Run32bitTest(0x0F, 0x0D, 0x42, 0x42);    // prefetchw\tdword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|__prefetchw(Mem0[edx + 0x00000042:word32])");
        }

        [Test]
        public void X86rw_mov_from_control_Reg()
        {
            Run32bitTest(0x0F, 0x20, 0x42);    // mov\tedx,cr0
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|edx = cr0");
        }

        [Test]
        public void X86rw_mov_debug_reg()
        {
            Run32bitTest(0x0F, 0x21, 0x42);    // mov\tedx,dr0
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|edx = dr0");
        }

        [Test]
        public void X86rw_mov_control_reg()
        {
            Run32bitTest(0x0F, 0x22, 0x42);    // mov\tcr0,edx
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|cr0 = edx");
        }

        [Test]
        public void X86rw_mov_to_debug_reg()
        {
            Run32bitTest(0x0F, 0x23, 0x42);    // mov\tdr0,edx
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|dr0 = edx");
        }

        [Test]
        public void X86rw_movhpd()
        {
            Run32bitTest(0x0F, 0x17, 0x42, 0x42);    // movhps\tqword ptr [edx+42],xmm0
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|Mem0[edx + 0x00000042:word64] = __movhps(v4)");
            Run32bitTest(0x66, 0x0F, 0x17, 0x42, 0x42);    // movhpd\tqword ptr [edx+42],xmm0
            AssertCode(
                "0|L--|10000000(5): 2 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|Mem0[edx + 0x00000042:word64] = __movhpd(v4)");
        }

        [Test]
        public void X86rw_movlps()
        {
            Run32bitTest(0x0F, 0x12, 0x42, 0x42);    // movlps\txmm0,qword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 0x00000042:word64]",
                "2|L--|xmm0 = __movlps(v4)");
        }

        [Test]
        public void X86rw_movmskps()
        {
            Run32bitTest(0x0F, 0x50, 0x42);    // movmskps\teax,xmm2
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v4 = __movmskps(xmm2)",
                "2|L--|eax = DPB(eax, v4, 0)");
        }

        [Test]
        public void X86rw_movnti()
        {
            //$TODO: should use intrisic here.
            Run32bitTest(0x0F, 0xC3, 0x42, 0x42);    // movnti\t[edx+42],eax
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 0x00000042:word32] = eax");
        }

        [Test]
        public void X86rw_movntps()
        {
            Run32bitTest(0x0F, 0x2B, 0x42, 0x42);    // movntps\t[edx+42],xmm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 0x00000042:word128] = xmm0");
        }


        [Test]
        public void X86rw_movntq()
        {
            Run32bitTest(0x0F, 0xE7, 0x42, 0x42);    // movntq\t[edx+42],mm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 0x00000042:word64] = mm0");
        }


        [Test]
        public void X86rw_orps()
        {
            Run32bitTest(0x0F, 0x56, 0x42, 0x42);    // orps\txmm0,[edx+42]
            AssertCode(
               "0|L--|10000000(4): 3 instructions",
               "1|L--|v4 = xmm0",
               "2|L--|v5 = Mem0[edx + 0x00000042:word128]",
               "3|L--|xmm0 = __orps(v4, v5)");
        }

        [Test]
        public void X86rw_packssdw()
        {
            Run32bitTest(0x0F, 0x6B, 0x42, 0x42);    // packssdw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word32]",
                "3|L--|mm0 = __packssdw(v4, v5)");
        }

        [Test]
        public void X86rw_packuswb()
        {
            Run32bitTest(0x0F, 0x67, 0x42, 0x42);    // packuswb\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word32]",
                "3|L--|mm0 = __packuswb(v4, v5)");
        }

        [Test]
        public void X86rw_paddb()
        {
            Run32bitTest(0x0F, 0xFC, 0x42, 0x42);    // paddb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __paddb(v4, v5)");
        }

        [Test]
        public void X86rw_paddd()
        {
            Run32bitTest(0x0F, 0xFE, 0x42, 0x42);    // paddd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __paddd(v4, v5)");
        }

        [Test]
        public void X86rw_paddsw()
        {
            Run32bitTest(0x0F, 0xED, 0x42, 0x42);    // paddsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __paddsw(v4, v5)");
        }

        [Test]
        public void X86rw_paddusb()
        {
            Run32bitTest(0x0F, 0xDC, 0x42, 0x42);    // paddusb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __paddusb(v4, v5)");
        }

        [Test]
        public void X86rw_paddw()
        {
            Run32bitTest(0x0F, 0xFD, 0x42, 0x42);    // paddw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __paddw(v4, v5)");
        }

        [Test]
        public void X86rw_getsec()
        {
            Run32bitTest(0x0F, 0x37);    // getsec
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_ebx = __getsec(eax)");
        }

        [Test]
        public void X86rw_pextrw()
        {
            Run32bitTest(0x0F, 0xC5, 0x42, 0x42);    // pextrw\teax,mm2,42
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|eax = __pextrw(eax, mm2, 0x42)");
        }

        [Test]
        public void X86rw_pinsrw()
        {
            //$TODO check encoding; look in the Intel spec.
            Run32bitTest(0x0F, 0xC4, 0x42);    // pinsrw\tmm0,edx
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm0 = __pinsrw(mm0, mm0, edx)");
        }

        [Test]
        public void X86rw_pxor()
        {
            Run32bitTest(0x0F, 0xEF, 0x42, 0x42);    // pxor\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pxor(mm0, Mem0[edx + 0x00000042:word64])");
        }

        [Test]
        public void X86rw_rcpps()
        {
            Run32bitTest(0x0F, 0x53, 0x42, 0x42);    // rcpps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 0x00000042:word128]",
                "2|L--|xmm0 = __rcpps(v4)");
        }

        [Test]
        public void X86rw_rdmsr()
        {
            Run32bitTest(0x0F, 0x32);    // rdmsr
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_eax = __rdmsr(ecx)");
        }

        [Test]
        public void X86rw_rdpmc()
        {
            Run32bitTest(0x0F, 0x33);    // rdpmc
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_eax = __rdpmc(ecx)");
        }


        [Test]
        public void X86rw_rdtsc()
        {
            Run32bitTest(0x0F, 0x31);
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|edx_eax = __rdtsc()");
        }

        [Test]
        public void X86rw_rsqrtps()
        {
            Run32bitTest(0x0F, 0x52, 0x42, 0x42);    // rsqrtps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 0x00000042:word128]",
                "2|L--|xmm0 = __rsqrtps(v4)");
        }

        [Test]
        public void X86rw_sqrtps()
        {
            Run32bitTest(0x0F, 0x51, 0x42, 0x42);    // sqrtps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 0x00000042:word128]",
                "2|L--|xmm0 = __sqrtps(v4)");
        }

        [Test]
        public void X86rw_pcmpgtb()
        {
            Run32bitTest(0x0F, 0x64, 0x42, 0x42);    // pcmpgtb\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:(arr byte 8)]",
                "3|L--|mm0 = __pcmpgtb(v4, v5)");
        }


        [Test]
        public void X86rw_pcmpgtw()
        {
            Run32bitTest(0x0F, 0x65, 0x42, 0x42);    // pcmpgtw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:(arr word16 4)]",
                "3|L--|mm0 = __pcmpgtw(v4, v5)");
        }

        [Test]
        public void X86rw_pcmpgtd()
        {
            Run32bitTest(0x0F, 0x66, 0x42, 0x42);    // pcmpgtd\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:(arr word32 2)]",
                "3|L--|mm0 = __pcmpgtd(v4, v5)");
        }

        [Test]
        public void X86rw_punpckhbw()
        {
            Run32bitTest(0x0F, 0x68, 0x42, 0x42);    // punpckhbw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhbw(mm0, Mem0[edx + 0x00000042:word32])");
        }

        [Test]
        public void X86rw_punpckhwd()
        {
            Run32bitTest(0x0F, 0x69, 0x42, 0x42);    // punpckhwd\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhwd(mm0, Mem0[edx + 0x00000042:word32])");
        }

        [Test]
        public void X86rw_punpckhdq()
        {
            Run32bitTest(0x0F, 0x6A, 0x42, 0x42);    // punpckhdq\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhdq(mm0, Mem0[edx + 0x00000042:word32])");
        }

        [Test]
        public void X86rw_punpckldq()
        {
            Run32bitTest(0x0F, 0x62, 0x42, 0x42);    // punpckldq\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckldq(mm0, Mem0[edx + 0x00000042:word32])");
        }

        [Test]
        public void X86rw_pcmpeqd()
        {
            Run32bitTest(0x0F, 0x76, 0x42, 0x42);    // pcmpeqd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:(arr word32 2)]",
                "3|L--|mm0 = __pcmpeqd(v4, v5)");
        }

        [Test]
        public void X86rw_pcmpeqw()
        {
            Run32bitTest(0x0F, 0x75, 0x42, 0x42);    // pcmpeqw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:(arr word16 4)]",
                "3|L--|mm0 = __pcmpeqw(v4, v5)");
        }

        [Test]
        public void X86rw_vmread()
        {
            Run32bitTest(0x0F, 0x78, 0x42, 0x42);    // vmread\t[edx+42],eax
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 0x00000042:word32] = __vmread(eax)");
        }


        [Test]
        public void X86rw_vmwrite()
        {
            Run32bitTest(0x0F, 0x79, 0x42, 0x42);    // vmwrite\teax,[edx+42]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|__vmwrite(eax, Mem0[edx + 0x00000042:word32])");
        }


        [Test]
        public void X86rw_vshufps()
        {
            Run32bitTest(0x0F, 0xC6, 0x42, 0x42, 0x7);    // vshufps\txmm0,[edx+42],07
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word128]",
                "3|L--|xmm0 = __vshufps(v4, v5, 0x07)");
        }

        [Test]
        public void X86rw_pminub()
        {
            Run32bitTest(0x0F, 0xDA, 0x42, 0x42);    // pminub\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __pminub(v4, v5)");
        }

        [Test]
        public void X86rw_pmullw()
        {
            Run32bitTest(0x0F, 0xD5, 0x42, 0x42);    // pmullw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0");
        }

        [Test]
        public void X86rw_pmovmskb()
        {
            Run32bitTest(0x0F, 0xD7, 0x42);    // pmovmskb\teax,mm2
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v4 = __pmovmskb(mm2)",
                "2|L--|eax = DPB(eax, v4, 0)");
        }

        [Test]
        public void X86rw_psrad()
        {
            Run32bitTest(0x0F, 0xE2, 0x42, 0x42);    // psrad\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psrad(v4, v5)");
        }

        [Test]
        public void X86rw_psrlq()
        {
            Run32bitTest(0x0F, 0xD3, 0x42, 0x42);    // psrlq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psrlq(v4, v5)");
        }

        [Test]
        public void X86rw_psubusb()
        {
            Run32bitTest(0x0F, 0xD8, 0x42, 0x42);    // psubusb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psubusb(v4, v5)");
        }

        [Test]
        public void X86rw_pmaxub()
        {
            Run32bitTest(0x0F, 0xDE, 0x42, 0x42);    // pmaxub\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __pmaxub(v4, v5)");
        }

        [Test]
        public void X86rw_pavgb()
        {
            Run32bitTest(0x0F, 0xE0, 0x42, 0x42);    // pavgb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 0x00000042:(arr byte 8)]",
                "2|L--|mm0 = __pavgb(v4)");
        }

        [Test]
        public void X86rw_psraw()
        {
            Run32bitTest(0x0F, 0xE1, 0x42, 0x42);    // psraw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psraw(v4, v5)");
        }



        [Test]
        public void X86rw_pmulhuw()
        {
            Run32bitTest(0x0F, 0xE4, 0x42, 0x42);    // pmulhuw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __pmulhuw(v4, v5)");
        }

        [Test]
        public void X86rw_pmulhw()
        {
            Run32bitTest(0x0F, 0xE5, 0x42, 0x42);    // pmulhw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __pmulhw(v4, v5)");
        }

        [Test]
        public void X86rw_psubb()
        {
            Run32bitTest(0x0F, 0xF8, 0x42, 0x42);    // psubb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psubb(v4, v5)");
        }

        [Test]
        public void X86rw_psubd()
        {
            Run32bitTest(0x0F, 0xFA, 0x42, 0x42);    // psubd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psubd(v4, v5)");
        }

        [Test]
        public void X86rw_psubq()
        {
            Run32bitTest(0x0F, 0xFB, 0x42, 0x42);    // psubq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psubq(v4, v5)");
        }

        [Test]
        public void X86rw_psubsw()
        {
            Run32bitTest(0x0F, 0xE9, 0x42, 0x42);    // psubsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psubsw(v4, v5)");
        }

        [Test]
        public void X86rw_psubw()
        {
            Run32bitTest(0x0F, 0xF9, 0x42, 0x42);    // psubw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psubw(v4, v5)");
        }

        [Test]
        public void X86rw_psubsb()
        {
            Run32bitTest(0x0F, 0xE8, 0x42, 0x42);    // psubsb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psubsb(v4, v5)");
        }

        [Test]
        public void X86rw_pmaxsw()
        {
            Run32bitTest(0x0F, 0xEE, 0x42, 0x42);    // pmaxsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __pmaxsw(v4, v5)");
        }

        [Test]
        public void X86rw_pminsw()
        {
            Run32bitTest(0x0F, 0xEA, 0x42, 0x42);    // pminsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __pminsw(v4, v5)");
        }

        [Test]
        public void X86rw_por()
        {
            Run32bitTest(0x0F, 0xEB, 0x42, 0x42);    // por\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __por(mm0, Mem0[edx + 0x00000042:word64])");
        }

        [Test]
        public void X86rw_pslld()
        {
            Run32bitTest(0x0F, 0xF2, 0x42, 0x42);    // pslld\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __pslld(v4, v5)");
        }

        [Test]
        public void X86rw_psllq()
        {
            Run32bitTest(0x0F, 0xF3, 0x42, 0x42);    // psllq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psllq(v4, v5)");
        }

        [Test]
        public void X86rw_psllw()
        {
            Run32bitTest(0x0F, 0xF1, 0x42, 0x42);    // psllw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psllw(v4, v5)");
        }

        [Test]
        public void X86rw_pmaddwd()
        {
            Run32bitTest(0x0F, 0xF5, 0x42, 0x42);    // pmaddwd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __pmaddwd(v4, v5)");
        }

        [Test]
        public void X86rw_pmuludq()
        {
            Run32bitTest(0x0F, 0xF4, 0x42, 0x42);    // pmuludq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __pmuludq(v4, v5)");
        }

        [Test]
        public void X86rw_psadbw()
        {
            Run32bitTest(0x0F, 0xF6, 0x42, 0x42);    // psadbw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x00000042:word64]",
                "3|L--|mm0 = __psadbw(v4, v5)");
        }

        [Test]
        public void X86rw_wrmsr()
        {
            Run32bitTest(0x0F, 0x30);    // wrmsr
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__wrmsr(ecx, edx_eax)");
        }


        [Test]
        public void X86Rw_vpxor()
        {
            Run32bitTest(0x66, 0x0F, 0xEF, 0xC0);	// vpxor	xmm0,xmm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = (word128) 0");
        }

        [Test]
        public void X86Rw_stmxcsr()
        {
            Run32bitTest(0x0F, 0xAE, 0x5D, 0xF0);	// stmxcsr	dword ptr [ebp-10]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[ebp - 0x00000010:word32] = mxcsr");
        }

        [Test]
        public void X86Rw_fcmovu()
        {
            Run32bitTest(0xDA, 0xDD);	// fcmovu	st(0),st(5)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NOT_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 5:real64]");
        }

        [Test]
        public void X86Rw_psrlw()
        {
            Run32bitTest(0x0F, 0xD1, 0xE8);	// psrlw	mm5,mm0
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v4 = mm5",
                "2|L--|mm5 = __psrlw(v4, mm0)");
        }

        [Test]
        public void X86Rw_psrld()
        {
            Run32bitTest(0x0F, 0xD2, 0xF9);	// psrld	mm7,mm1
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v4 = mm7",
                "2|L--|mm7 = __psrld(v4, mm1)");
        }

        [Test]
        public void X86Rw_fcmovnu()
        {
            Run32bitTest(0xDB, 0xD9);	// fcmovnu	st(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(IS_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1:real64]");
        }

        [Test]
        public void X86Rw_paddq()
        {
            Run32bitTest(0x0F, 0xD4, 0x08);	// paddq	mm1,[eax]
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|v4 = mm1",
                "2|L--|v5 = Mem0[eax:word64]",
                "3|L--|mm1 = __paddq(v4, v5)");
        }

        [Test]
        public void X86Rw_psubusw()
        {
            Run32bitTest(0x0F, 0xD9, 0x45, 0x0C);	// psubusw	mm0,[ebp+0C]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[ebp + 0x0000000C:word64]",
                "3|L--|mm0 = __psubusw(v4, v5)");
        }

        [Test]
        public void X86Rw_pshufw()
        {
            Run32bitTest(0x0F, 0x70, 0x02, 0x00);	// pshufw	mm0,[edx],00
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pshufw(mm0, Mem0[edx:word64], 0x00)");
        }

        [Test]
        public void X86Rw_fxtract()
        {
            Run32bitTest(0xD9, 0xF4);	// fxtract
            AssertCode(
                "0|L--|10000000(2): 4 instructions",
                "1|L--|v3 = ST[Top:real64]",
                "2|L--|Top = Top - 1",
                "3|L--|ST[Top + 1:real64] = __exponent(v3)",
                "4|L--|ST[Top:real64] = __significand(v3)");
        }

        [Test]
        public void X86Rw_fprem1()
        {
            Run32bitTest(0xD9, 0xF5);	// fprem1	st(5),st(0)
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|ST[Top + 5:real64] = __fprem1(ST[Top + 5:real64], ST[Top:real64])");
        }

        [Test]
        public void X86Rw_andpd()
        {
            Run32bitTest(0x66, 0x0F, 0x54, 0x05, 0x50, 0x59, 0x57, 0x00);	// andpd	xmm0,[00575950]
            AssertCode(
                "0|L--|10000000(8): 3 instructions",
                "1|L--|v3 = xmm0",
                "2|L--|v4 = Mem0[0x00575950:word128]",
                "3|L--|xmm0 = __andpd(v3, v4)");
        }

        [Test]
        public void X86Rw_vpsubd()
        {
            Run32bitTest(0x66, 0x0F, 0xFA, 0xD0);	// vpsubd	xmm2,xmm0
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = xmm2",
                "2|L--|v5 = xmm0",
                "3|L--|xmm2 = __psubd(v4, v5)");
        }

        [Test]
        public void X86Rw_vpmullw()
        {
            Run32bitTest(0x66, 0x0F, 0xD3, 0xCA);	// vpmullw	xmm1,xmm2
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = xmm1",
                "2|L--|v5 = xmm2",
                "3|L--|xmm1 = __pmullw(v4, v5)");
        }

        [Test]
        public void X86Rw_vpsllq()
        {
            Run32bitTest(0x66, 0x0F, 0xF3, 0xCA);	// vpsllq	xmm1,xmm2
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = xmm1",
                "2|L--|v5 = xmm2",
                "3|L--|xmm1 = __psllq(v4, v5)");
        }

        [Test]
        public void X86Rw_orpd()
        {
            Run32bitTest(0x66, 0x0F, 0x56, 0x1D, 0xA0, 0x59, 0x57, 0x00);	// orpd	xmm3,[005759A0]
            AssertCode(
                "0|L--|10000000(8): 3 instructions",
                "1|L--|v3 = xmm3",
                "2|L--|v4 = Mem0[0x005759A0:word128]",
                "3|L--|xmm3 = __orpd(v3, v4)");
        }

        [Test]
        public void X86Rw_movlpd()
        {
            Run32bitTest(0x66, 0x0F, 0x12, 0x44, 0x24, 0x04);	// movlpd	xmm0,qword ptr [esp+04]
            AssertCode(
                "0|L--|10000000(6): 2 instructions",
                "1|L--|v4 = Mem0[esp + 0x00000004:word64]",
                "2|L--|xmm0 = __movlpd(v4)");
        }

        [Test]
        public void X86Rw_vextrw()
        {
            Run32bitTest(0x66, 0x0F, 0xC5, 0xC0, 0x03);	// vextrw	eax,xmm0,03
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|eax = __pextrw(eax, xmm0, 0x03)");
        }

        [Test]
        public void X86Rw_cvtsd2si()
        {
            Run32bitTest(0xF2, 0x0F, 0x2D, 0xD1);	// cvtsd2si	edx,xmm1
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|edx = (int32) xmm1");
        }

        [Test]
        public void X86Rw_vpand()
        {
            Run32bitTest(0x66, 0x0F, 0xDB, 0xFE);	// vpand	xmm7,xmm6
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm7 = __pand(xmm7, xmm6)");
        }

        [Test]
        public void X86Rw_pand()
        {
            Run32bitTest(0x0F, 0xDB, 0xFE);	// pand	mm7,mm6
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm7 = __pand(mm7, mm6)");
        }

        [Test]
        public void X86Rw_vpaddq()
        {
            Run32bitTest(0x66, 0x0F, 0xD4, 0xFE);	// vpaddq	xmm7,xmm6
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = xmm7",
                "2|L--|v5 = xmm6",
                "3|L--|xmm7 = __paddq(v4, v5)");
        }

        [Test]
        public void X86Rw_vpandn()
        {
            Run32bitTest(0x66, 0x0F, 0xDF, 0xF2);	// vpandn	xmm6,xmm2
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm6 = __pandn(xmm6, xmm2)");
        }

        [Test]
        public void X86Rw_pandn()
        {
            Run32bitTest(0x0F, 0xDF, 0xF2);	// pandn	mm6,mm2
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm6 = __pandn(mm6, mm2)");
        }

        [Test]
        public void X86Rw_vpinsrw()
        {
            Run32bitTest(0x66, 0x0F, 0xC4, 0xC0);	// vpinsrw	xmm0,eax
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __pinsrw(xmm0, xmm0, eax)");
        }

        [Test]
        public void X86Rw_ldmxcsr()
        {
            Run32bitTest(0x0F, 0xAE, 0x55, 0x08);	// ldmxcsr	dword ptr [ebp+08]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mxcsr = Mem0[ebp + 0x00000008:word32]");
        }

        [Test]
        public void X86Rw_paddsb()
        {
            Run32bitTest(0x0F, 0xEC, 0xFF);	// paddsb	mm7,mm7
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|v3 = mm7",
                "2|L--|v4 = mm7",
                "3|L--|mm7 = __paddsb(v3, v4)");
        }

        [Test]
        public void X86Rw_getsec()
        {
            Run32bitTest(0x0F, 0x37);	// getsec
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_ebx = __getsec(eax)");
        }

        [Test(Description = "We cannot make 16-bit calls in 32- or 64-bit mode")]
        public void X86Rw_invalid_call()
        {
            Run32bitTest(0x66, 0xFF, 0x51, 0xCC); // call word ptr[ecx - 34]
            AssertCode(
                "0|---|10000000(4): 1 instructions",
                "1|---|<invalid>");
        }


        [Test]
        public void X86Rw_cvtss2sd()
        {
            Run64bitTest(0xF3, 0x48, 0x0F, 0x5A, 0x0D, 0xB5, 0x47, 0x32, 0x00);	// cvtss2sd	xmm1,dword ptr [rip+003247B5]
            AssertCode(
                "0|L--|0000000140000000(9): 2 instructions",
                "1|L--|v3 = (real64) Mem0[0x00000001403247BE:real32]",
                "2|L--|xmm1 = DPB(xmm1, v3, 0)");
        }


        [Test]
        public void X86Rw_cvtsd2ss()
        {
            Run64bitTest(0xF2, 0x48, 0x0F, 0x5A, 0xC0);	// cvtsd2ss	xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v3 = (real32) xmm0",
                "2|L--|xmm0 = DPB(xmm0, v3, 0)");
        }


        [Test]
        public void X86Rw_cvtss2si()
        {
            Run64bitTest(0xF3, 0x48, 0x0F, 0x2D, 0x50, 0x10);	// cvtss2si	rdx,dword ptr [rax+10]
            AssertCode(
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|rdx = (int64) Mem0[rax + 0x0000000000000010:real32]");
        }

        [Test]
        public void X86Rw_sqrtsd()
        {
            Run64bitTest(0xF2, 0x0F, 0x51, 0xC0);	// sqrtsd	xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = __sqrt(xmm0)",
                "2|L--|xmm0 = DPB(xmm0, v3, 0)");
        }

        [Test]
        public void X86Rw_sldt()
        {
            Run64bitTest(0x0F, 0x00, 0x01);  // sldt	word ptr [ecx]
            AssertCode(
                 "0|S--|0000000140000000(3): 1 instructions",
                 "1|L--|Mem0[rcx:word16] = __sldt()");
        }


        [Test]
        public void X86Rw_minpd()
        {
            Run64bitTest(0x66, 0x0F, 0x5D, 0x42, 0x42); // minpd\txmm0,[rdx+42]
            AssertCode(
                "0|L--|0000000140000000(5): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = Mem0[rdx + 0x0000000000000042:word128]",
                "3|L--|xmm0 = __minpd(v4, v5)");
        }

        [Test]
        public void X86Rw_sgdt()
        {
            Run32bitTest(0x0F, 0x01, 0x00);	// sgdt	[eax]
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|Mem0[eax:word48] = __sgdt()");
        }

        [Test]
        public void X86Rw_sidt()
        {
            Run32bitTest(0x0F, 0x01, 0x8A, 0x86, 0x04, 0x05, 0x00);	// sidt	[edx+00050486]
            AssertCode(
                "0|S--|10000000(7): 1 instructions",
                "1|L--|Mem0[edx + 0x00050486:word48] = __sidt()");
        }

        [Test]
        public void X86Rw_lldt()
        {
            Run32bitTest(0x0F, 0x00, 0x55, 0x8D);	// lldt	word ptr [ebp-73]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|__lldt(Mem0[ebp - 0x00000073:word48])");
        }

        [Test]
        public void X86Rw_ud0()
        {
            Run32bitTest(0x0F, 0xFF, 0xFF);	// ud0	edi,edi
            AssertCode(
                "0|---|10000000(3): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86Rw_ud1()
        {
            Run32bitTest(0x0F, 0xB9, 0x00);	// ud1	eax,[eax]
            AssertCode(
                "0|---|10000000(3): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86Rw_lidt()
        {
            Run32bitTest(0x0F, 0x01, 0x1B);	// lidt	[ebx]
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|__lidt(Mem0[ebx:word48])");
        }

        [Test]
        public void X86Rw_sha1msg2()
        {
            Run32bitTest(0x0F, 0x38, 0xCA, 0x75, 0xE8);	// sha1msg2	xmm6,[ebp-18]
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v4 = Mem0[ebp - 0x00000018:word128]",
                "2|L--|v5 = xmm6",
                "3|L--|xmm6 = __sha1msg2(v5, v4)");
        }

        [Test]
        public void X86Rw_movbe()
        {
            Run32bitTest(0x0F, 0x38, 0xF1, 0xC3);
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|ebx = __movbe_32(eax)");
        }

        /// <summary>
        /// This appears to be an obsolete 286 whose net effect is negligible.
        /// </summary>
        [Test]
        public void X86Rw_fnsetpm()
        {
            Run32bitTest(0xDB, 0xE4);	// fnsetpm
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void X86Rw_smsw()
        {
            Run32bitTest(0x0F, 0x01, 0xE0);	// smsw	ax
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|eax = __smsw()");
        }

        [Test]
        public void X86Rw_lmsw()
        {
            Run32bitTest(0x0F, 0x01, 0xF0);	// lmsw	ax
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|__lmsw(ax)");
        }
    }
}