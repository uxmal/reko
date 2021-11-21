#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Arch.X86
{
    [TestFixture]
    partial class X86RewriterTests : Arch.RewriterTestBase
    {
        private readonly IntelArchitecture arch16;
        private readonly IntelArchitecture arch32;
        private readonly IntelArchitecture arch64;
        private readonly Address baseAddr16;
        private readonly Address baseAddr32;
        private readonly Address baseAddr64;
        private IntelArchitecture arch;
        private Address baseAddr;
        private ServiceContainer sc;
        private RewriterHost host;

        public X86RewriterTests()
        {
            var sc = CreateServiceContainer();
            arch16 = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            arch32 = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            arch64 = new X86ArchitectureFlat64(sc, "x86-protected-64", new Dictionary<string, object>());
            baseAddr16 = Address.SegPtr(0x0C00, 0x0000);
            baseAddr32 = Address.Ptr32(0x10000000);
            baseAddr64 = Address.Ptr64(0x140000000ul);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => baseAddr;

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
            Given_MemoryArea(new ByteMemoryArea(baseAddr16, bytes));
            host = new RewriterHost(null);
        }

        private void Run32bitTest(params byte[] bytes)
        {
            arch = arch32;
            Given_MemoryArea(new ByteMemoryArea(baseAddr32, bytes));
            host = new RewriterHost(null);
        }

        private void Run32bitTest(string hexBytes)
        {
            arch = arch32;
            Given_MemoryArea(new ByteMemoryArea(baseAddr32, BytePattern.FromHexBytes(hexBytes)));
            host = new RewriterHost(null);
        }

        private void Run64bitTest(params byte[] bytes)
        {
            arch = arch64;
            Given_MemoryArea(new ByteMemoryArea(baseAddr64, bytes));
            host = new RewriterHost(null);
        }

        private void Run64bitTest(string hexBytes)
        {
            arch = arch64;
            Given_MemoryArea(new ByteMemoryArea(baseAddr64, BytePattern.FromHexBytes(hexBytes))); 
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

        [Test]
        public void X86rw_MovStackArgument()
        {
            Run16bitTest(m =>
            {
                m.Mov(m.ax, m.MemW(Registers.bp, -8));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|ax = Mem0[ss:bp - 8<16>:word16]");
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
                "1|L--|ax = ax + Mem0[ds:si + 4<16>:word16]",
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
                "1|L--|v3 = Mem0[ds:0x1000<16>:word16] + 3<16>",
                "2|L--|Mem0[ds:0x1000<16>:word16] = v3",
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
                "1|L--|ecx = ecx - 0x12345<32>",
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
                "1|L--|si = si & 0x32<16>",
                "2|L--|SZO = cond(si)",
                "3|L--|C = false");
        }

        [Test]
        public void X86Rw_andnpd()
        {
            Run64bitTest("660F55D9");
            AssertCode(     // andnpd	xmm3,xmm1
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm3 = __andnpd(xmm3, xmm1)");
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
                "1|L--|SZO = cond(edi & 0xFFFFFFFF<32>)",
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
                "1|L--|SCZO = cond(ebx - 3<32>)");
        }

        [Test]
        public void X86Rw_cmpss()
        {
            Run64bitTest("F30FC2C805");
            AssertCode(     // cmpss	xmm1,xmm0,5h
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm1 = SLICE(xmm1, real32, 0) >= SLICE(xmm0, real32, 0) ? 0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128> : 0<128>");
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
                "1|L--|sp = sp - 4<i16>",
                "2|L--|Mem0[ss:sp:word32] = eax",
                "3|L--|0C00:0002(2): 2 instructions",
                "4|L--|ebx = Mem0[ss:sp:word32]",
                "5|L--|sp = sp + 4<i16>");
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
                "1|T--|goto Mem0[ds:bx + 0x10<16>:word16]");
        }

        [Test]
        public void X86Rw_JmpFarIndirect()
        {
            Run16bitTest(0xFF, 0x6F, 0x34);
            AssertCode(
                "0|T--|0C00:0000(3): 1 instructions",
                "1|T--|goto Mem0[ds:bx + 0x34<16>:segptr32]");
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
                "1|L--|ax = 0x4C00<16>",
                "2|T--|0C00:0003(2): 1 instructions",
                "3|L--|__syscall(0x21<8>)");
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
                "0|R--|0C00:0000(1): 1 instructions",
                "1|R--|return (2,0)");
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
                "0|R--|0C00:0000(3): 1 instructions",
                "1|R--|return (2,8)");
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
                "1|L--|cx = cx - 1<16>",
                "2|T--|if (cx != 0<16>) branch 0C00:0000");
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
                "1|L--|cx = cx - 1<16>",
                "2|T--|if (Test(EQ,Z) && cx != 0<16>) branch 0C00:0000",
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
                "1|L--|v5 = Mem0[ds:0x100<16>:word16] + ax + C",
                "2|L--|Mem0[ds:0x100<16>:word16] = v5",
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
                "1|L--|bx = bx + 4<16>");
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
                "1|L--|sp = sp - 2<i16>",
                "2|L--|Mem0[ss:sp:word16] = bp",
                "3|L--|bp = sp",
                "4|L--|sp = sp - 16<i16>");
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
                "3|L--|C = ecx == 0<32>");
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
                "1|T--|if (cx == 0<16>) branch 0C00:0000");
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
                "1|T--|if (cx == 0<16>) branch 0C00:0002",
                "2|L--|ax = Mem0[ds:si:word16]",
                "3|L--|si = si + 2<i16>",
                "4|L--|cx = cx - 1<16>",
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
        public void X86Rw_shlx()
        {
            Run64bitTest("C44289F7C6");
            AssertCode(     // shlx	r8,r14,r14
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|r8 = r14 << r14");
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
                "1|L--|eax = __shrd(eax, edx, 4<8>)");
        }

        [Test]
        public void X86Rw_shrx()
        {
            Run64bitTest("C4C23BF7ED");
            AssertCode(     // shrx	ebp,r13d,r8d
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|ebp = r13d >>u r8d",
                "2|L--|rbp = CONVERT(ebp, word32, uint64)");
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
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[ebx + 4<32>:int32], int32, real64)");
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
                "1|L--|Mem0[ebx + 4<32>:real32] = CONVERT(ST[Top:real64], real64, real32)",
                "2|L--|Top = Top + 1<i8>");
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
                "1|T--|if (cx == 0<16>) branch 0C00:0002",
                "2|L--|SCZO = cond(al - Mem0[es:di:byte])",
                "3|L--|di = di + 1<i16>",
                "4|L--|cx = cx - 1<16>",
                "5|T--|if (Test(NE,Z)) branch 0C00:0000",
                "6|R--|0C00:0002(1): 1 instructions",
                "7|R--|return (2,0)");
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
                "1|L--|es_bx = Mem0[ss:bp + 6<16>:segptr32]");
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
                "1|L--|ST[Top:real64] = ST[Top:real64] + CONVERT(Mem0[ds:bx + 0<16>:int16], int16, real64)");
        }

        /// <summary>
        /// Captures the side effect of setting CF = 0
        /// </summary>
        [Test]
        public void X86rw_and()
        {
            Run16bitTest(m =>
            {
                m.And(m.ax, m.Const(8));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 3 instructions",
                "1|L--|ax = ax & 8<16>",
                "2|L--|SZO = cond(ax)",
                "3|L--|C = false");
        }

        [Test]
        public void X86rw_andn()
        {
            Run64bitTest("C4E278F2CB"); //andn\tecx,eax,ebx
            AssertCode(
                "0|L--|0000000140000000(5): 4 instructions",
                "1|L--|ecx = ebx & ~eax",
                "2|L--|rcx = CONVERT(ecx, word32, uint64)",
                "3|L--|SZO = cond(ecx)",
                "4|L--|C = false");
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
                "1|L--|SZO = cond(ax & 8<16>)",
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
                "1|L--|dx_ax = cx *s32 ax",
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
                "1|L--|dx_ax = cx *u32 ax",
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
                "1|L--|ST[Top:real64] = ST[Top:real64] * ST[Top + 1<i8>:real64]");
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
                "2|L--|dx = CONVERT(v5 % cx, word32, uint16)",
                "3|L--|ax = CONVERT(v5 /u cx, word16, uint16)",
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
                    "2|L--|dx = CONVERT(v5 % cx, word32, int16)",
                    "3|L--|ax = CONVERT(v5 /16 cx, word16, int16)",
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
                "1|L--|Z = eax == 0<32>",
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
                "1|T--|call SEQ(0xC00<16>, bx) (2)",
                "2|T--|0C00:0002(3): 1 instructions",
                "3|T--|call SEQ(0xC00<16>, Mem0[ds:bx + 4<16>:word16]) (2)",
                "4|T--|0C00:0005(3): 1 instructions",
                "5|T--|call Mem0[ds:bx + 8<16>:segptr32] (4)");
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
        public void X86rw_Lahf()
        {
            Run16bitTest(m =>
            {
                m.Lahf();
            });
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|ah = SCZOP");
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
                "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 1<i8>:real64])",
                "2|L--|Top = Top + 2<i8>",
                "3|T--|0C00:0002(7): 2 instructions",
                "4|L--|SCZO = FPUF",
                "5|T--|if (Test(NE,FPUF)) branch 0C00:0000");
        }

        [Test]
        public void X86Rw_fstsw_InterleavedInstructions()
        {
            Run32bitTest(
                "D9 44 24 14" +         // fld dword ptr[esp + 14h]
                "D8 D9" +               // fcomp st(0),st(1)
                "DF E0" +               // fstsw ax
                "DD D8" +               // fstp st(0)
                "F6 C4 05" +            // test ah,5h
                "0F 8B 2E FF FF FF");   // jpo 41E652h
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[esp + 0x14<32>:real32], real32, real64)",
                "3|L--|10000004(2): 2 instructions",
                "4|L--|FPUF = cond(ST[Top:real64] - ST[Top + 1<i8>:real64])",
                "5|L--|Top = Top + 1<i8>",
                "6|T--|10000006(13): 4 instructions",
                "7|L--|ST[Top:real64] = ST[Top:real64]",
                "8|L--|Top = Top + 1<i8>",
                "9|L--|SCZO = FPUF",
                "10|T--|if (Test(LT,FPUF)) branch 0FFFFF41");
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
                "2|L--|eax = Mem0[esp + 4<32>:word32]",
                "3|T--|if (Test(LE,FPUF)) branch 10000000");
        }

        [Test]
        public void X86rw_sar()
        {
            Run16bitTest(m =>
            {
                m.Sar(m.ax, 1);
                m.Sar(m.bx, m.cl);
                m.Sar(m.dx, 4);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 2 instructions",
                "1|L--|ax = ax >> 1<16>",
                "2|L--|SCZO = cond(ax)",
                "3|L--|0C00:0002(2): 2 instructions",
                "4|L--|bx = bx >> cl",
                "5|L--|SCZO = cond(bx)",
                "6|L--|0C00:0004(3): 2 instructions",
                "7|L--|dx = dx >> 4<16>",
                "8|L--|SCZO = cond(dx)");
        }

        [Test]
        public void X86Rw_sarx()
        {
            Run64bitTest("C462D2F7C1");
            AssertCode(     // sarx	r8,rcx,rbp
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|r8 = rcx >> rbp");
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
                "1|L--|al = Mem0[ds:bx + CONVERT(al, uint8, uint16):byte]");
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
                "1|L--|al = Mem0[ebx + CONVERT(al, uint8, uint32):byte]");
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
                "2|L--|esi = esi + 1<i32>",
                "3|L--|edi = edi + 1<i32>");
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
        public void X86Rw_popcnt()
        {
            Run64bitTest("F3480FB8C7");
            AssertCode(     // popcnt	rax,rdi
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|rax = __popcnt(rdi)",
                "2|L--|Z = rdi == 0<64>");
        }

        [Test]
        public void X86Rw_popcnt_mem()
        {
            Run64bitTest("F3480FB800");
            AssertCode(     // popcnt	rax,rdi
                "0|L--|0000000140000000(5): 3 instructions",
                "1|L--|v3 = Mem0[rax:word64]",
                "2|L--|rax = __popcnt(v3)",
                "3|L--|Z = v3 == 0<64>");
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
                "1|L--|sp = sp - 2<i16>",
                "2|L--|Mem0[ss:sp:word16] = SCZDOP",
                "3|L--|0C00:0001(1): 2 instructions",
                "4|L--|SCZDOP = Mem0[ss:sp:word16]",
                "5|L--|sp = sp + 2<i16>");
        }

        [Test]
        public void X86rw_std_Lodsw()
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
                "4|L--|esi = esi - 2<i32>",
                "5|L--|10000003(1): 1 instructions",
                "6|L--|D = false",
                "7|L--|10000004(2): 2 instructions",
                "8|L--|ax = Mem0[esi:word16]",
                "9|L--|esi = esi + 2<i32>");
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
                "1|L--|es_bx = Mem0[ds:bx + 0<16>:segptr32]");
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
                "1|L--|SCZO = cond(Mem0[edi:word32] - 0xFFFFFFFF<32>)");
        }

        [Test]
        public void X86rw_rol_Eb()
        {
            Run32bitTest(0xC0, 0xC0, 0xC0);
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|v2 = (al & 1<8> << 8<8> - 0xC0<8>) != 0<8>", 
                "2|L--|al = __rol(al, 0xC0<8>)",
                "3|L--|C = v2");
        }

        [Test]
        public void X86Rw_rorx()
        {
            Run64bitTest("C443FBF0D602");
            AssertCode(     // rorx	r10,r14,2h
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|r10 = __ror(r14, 2<8>)");
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
                "1|L--|mm1 = 0<64>");
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
        public void X86rw_xabort()
        {
            Run64bitTest("44C6F842");
            AssertCode(     // xabort
                "0|H--|0000000140000000(4): 1 instructions",
                "1|H--|__xabort(0x42<8>)");
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
              "1|L--|eax = CONVERT(SLICE(xmm3, real64, 0), real64, int32)");
        }


        [Test]
        public void X86rw_fucompp()
        {
            Run32bitTest(0xDA, 0xE9);
            AssertCode(
              "0|L--|10000000(2): 2 instructions",
              "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 1<i8>:real64])",
              "2|L--|Top = Top + 2<i8>");
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
                "1|L--|cl = CONVERT(Test(OV,O), bool, int8)");
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
                "1|L--|cl = CONVERT(Test(ULT,C), bool, int8)");
        }

        [Test]
        public void X86rw_movd_xmm()
        {
            Run32bitTest(0x66, 0x0f, 0x6e, 0xc0);
            AssertCode(  // movd
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = CONVERT(eax, word32, word128)");
        }

        [Test]
        public void X86rw_more_xmm()
        {
            Run32bitTest(0x66, 0x0f, 0x7e, 0x01);
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = CONVERT(xmm0, word128, word32)",
                "2|L--|Mem0[ecx:word32] = v4");
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
                "1|L--|xmm0 = __pshufd(xmm0, xmm0, 0<8>)");
        }

        [Test]
        public void X86rw_64_movsxd()
        {
            Run64bitTest(0x48, 0x63, 0x48, 0x3c); // "movsxd\trcx,dword ptr [rax+3C]", 
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|rcx = CONVERT(Mem0[rax + 0x3C<64>:word32], word32, int64)");
        }

        [Test]
        public void X86rw_64_movsxd_dword()
        {
            Run64bitTest(0x63, 0x48, 0x3c); // "movsxd\tecx,dword ptr [rax+3C]", 
            AssertCode(
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|ecx = Mem0[rax + 0x3C<64>:word32]");
        }

        [Test]
        public void X86rw_64_rip_relative()
        {
            Run64bitTest(0x49, 0x8b, 0x05, 0x00, 0x00, 0x10, 0x00); // "mov\trax,qword ptr [rip+10000000]",
            AssertCode(
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|rax = Mem0[0x0000000140100007<p64>:word64]");
        }


        [Test]
        public void X86rw_64_sub_immediate_dword()
        {
            Run64bitTest(0x48, 0x81, 0xEC, 0x08, 0x05, 0x00, 0x00); // "sub\trsp,+00000508", 
            AssertCode(
                 "0|L--|0000000140000000(7): 2 instructions",
                 "1|L--|rsp = rsp - 0x508<64>",
                 "2|L--|SCZO = cond(rsp)");
        }

        [Test]
        public void X86rw_64_repne()
        {
            Run64bitTest(0xF3, 0x48, 0xA5);   // "rep\tmovsd"
            AssertCode(
                 "0|L--|0000000140000000(3): 7 instructions",
                 "1|T--|if (rcx == 0<64>) branch 0000000140000003",
                 "2|L--|v3 = Mem0[rsi:word64]",
                 "3|L--|Mem0[rdi:word64] = v3",
                 "4|L--|rsi = rsi + 8<i64>", 
                 "5|L--|rdi = rdi + 8<i64>",
                 "6|L--|rcx = rcx - 1<64>",
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
                "0|T--|0C00:0000(4): 1 instructions",
                "1|T--|call 0C00:3246 (4)");
        }

        [Test]
        public void X86rw_fstp_real32()
        {
            Run32bitTest(0xd9, 0x1c, 0x24);
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[esp:real32] = CONVERT(ST[Top:real64], real64, real32)",
                "2|L--|Top = Top + 1<i8>");
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
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[ds:si + 0x40<16>:real32], real32, real64)");
        }

        [Test]
        public void X86rw_movaps()
        {
            Run64bitTest(0x0F, 0x28, 0x05, 0x40, 0x12, 0x00, 0x00); //  movaps xmm0,[rip+00001240]
            AssertCode(
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|xmm0 = Mem0[0x0000000140001247<p64>:word128]");
        }

        [Test]
        public void X86rw_idiv()
        {
            Run32bitTest(0xF7, 0x7C, 0x24, 0x04);       // idiv [esp+04]
            AssertCode(
                  "0|L--|10000000(4): 4 instructions",
                  "1|L--|v5 = edx_eax",
                  "2|L--|edx = CONVERT(v5 % Mem0[esp + 4<32>:word32], word64, int32)",
                  "3|L--|eax = CONVERT(v5 /32 Mem0[esp + 4<32>:word32], word32, int32)",
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
                "1|L--|xmm6[2<i32>] = xmm3[0<i32>]",
                "2|L--|xmm6[3<i32>] = xmm3[1<i32>]");
        }

        [Test]
        public void X86rw_fyl2xp1()
        {
            Run16bitTest(0xD9, 0xF9);
            AssertCode(
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ST[Top + 1<i8>:real64] = ST[Top + 1<i8>:real64] * lg2(ST[Top:real64] + 1.0)",
                "2|L--|FPUF = cond(ST[Top + 1<i8>:real64])");
        }

        [Test]
        public void X86rw_fucomi()
        {
            Run32bitTest(0xDB, 0xEB);  // fucomi\tst(0),st(3)
            AssertCode(
               "0|L--|10000000(2): 3 instructions",
               "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 3<i8>:real64])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_fucomip()
        {
            Run32bitTest(0xDF, 0xE9);   // fucomip\tst(0),st(1)
            AssertCode(
               "0|L--|10000000(2): 4 instructions",
               "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 1<i8>:real64])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_movups()
        {
            Run64bitTest(0x0F, 0x10, 0x45, 0xE0);   // movups xmm0,XMMWORD PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = Mem0[rbp - 0x20<64>:word128]");

            Run64bitTest(0x66, 0x0F, 0x10, 0x45, 0xE0);   // movupd xmm0,XMMWORD PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = Mem0[rbp - 0x20<64>:word128]");

            Run64bitTest(0x0F, 0x11, 0x44, 0x24, 0x20); // movups\t[rsp+20],xmm0, 
            AssertCode(
                  "0|L--|0000000140000000(5): 1 instructions",
                  "1|L--|Mem0[rsp + 0x20<64>:word128] = xmm0");
        }

        [Test]
        public void X86rw_movss()
        {
            Run64bitTest(0xF3, 0x0F, 0x10, 0x45, 0xE0);   // movss xmm0,dword PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = SEQ(0<96>, Mem0[rbp - 0x20<64>:real32])");
            Run64bitTest(0xF3, 0x0F, 0x11, 0x45, 0xE0);   // movss dword PTR[rbp - 0x20], xmm0,
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|Mem0[rbp - 0x20<64>:real32] = SLICE(xmm0, real32, 0)");
            Run64bitTest(0xF3, 0x0F, 0x10, 0xC3);         // movss xmm0, xmm3,
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), SLICE(xmm3, real32, 0))");
        }

        [Test(Description = "Regression reported by @mewmew")]
        public void X86rw_regression1()
        {
            Run32bitTest(0xDB, 0x7C, 0x47, 0x83);       // fstp [esi-0x7D + eax*2]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|Mem0[edi - 0x7D<32> + eax * 2<32>:real80] = CONVERT(ST[Top:real64], real64, real80)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fdivr()
        {
            Run32bitTest(0xDC, 0x3D, 0x78, 0x56, 0x34, 0x12); // fdivr [12345678]
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|ST[Top:real64] = Mem0[0x12345678<p32>:real64] / ST[Top:real64]");
        }

        [Test]
        public void X86rw_movsd()
        {
            Run64bitTest(0xF2, 0x0F, 0x10, 0x45, 0xE0);   // movsd xmm0,dword PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = SEQ(0<64>, Mem0[rbp - 0x20<64>:real64])");
            Run64bitTest(0xF2, 0x0F, 0x11, 0x45, 0xE0);   // movsd dword PTR[rbp - 0x20], xmm0,
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|Mem0[rbp - 0x20<64>:real64] = SLICE(xmm0, real64, 0)");
            Run64bitTest(0xF2, 0x0F, 0x10, 0xC3);   // movsd xmm0, xmm3,
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = SEQ(SLICE(xmm0, word64, 64), SLICE(xmm3, real64, 0))");
        }

        [Test(Description = "Intel and AMD state that if you set the low 32-bits of a register in 64-bit mode, they are zero extended.")]
        public void X86rw_64bit_clearHighBits()
        {
            Run64bitTest(0x33, 0xC0);
            AssertCode(
               "0|L--|0000000140000000(2): 4 instructions",
               "1|L--|eax = eax ^ eax",
               "2|L--|rax = CONVERT(eax, word32, uint64)",
               "3|L--|SZO = cond(eax)",
               "4|L--|C = false");
        }

        [Test]
        public void X86Rw_tzcnt()
        {
            Run64bitTest("F30FBCCA");
            AssertCode(     // tzcnt	ecx,edx
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ecx = __tzcnt(edx)");
        }

        [Test]
        public void X86rw_ucomiss()
        {
            Run64bitTest(0x0F, 0x2E, 0x05, 0x2D, 0xB1, 0x00, 0x00);
            AssertCode( // ucomiss\txmm0,dword ptr [rip+0000B12D]
               "0|L--|0000000140000000(7): 3 instructions",
               "1|L--|CZP = cond(SLICE(xmm0, real32, 0) - Mem0[0x000000014000B134<p64>:real32])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_ucomisd()
        {
            Run64bitTest(0x66, 0x0F, 0x2E, 0x05, 0x2D, 0xB1, 0x00, 0x00);
            AssertCode( // ucomisd\txmm0,qword ptr [rip+0000B12D]
               "0|L--|0000000140000000(8): 3 instructions",
               "1|L--|CZP = cond(SLICE(xmm0, real64, 0) - Mem0[0x000000014000B135<p64>:real64])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_addss()
        {
            Run64bitTest(0xF3, 0x0F, 0x58, 0x0D, 0xFB, 0xB0, 0x00, 0x00);
            AssertCode( //addss\txmm1,dword ptr [rip+0000B0FB]
               "0|L--|0000000140000000(8): 3 instructions",
               "1|L--|v3 = SLICE(xmm1, real32, 0) + Mem0[0x000000014000B103<p64>:real32]",
               "2|L--|v4 = SLICE(xmm1, word96, 32)",
               "3|L--|xmm1 = SEQ(v4, v3)");
        }

        [Test]
        public void X86rw_subss()
        {
            Run64bitTest(0xF3, 0x0F, 0x5C, 0xCD);
            AssertCode(     // subss\txmm1,dword ptr [rip+0000B0FB]
               "0|L--|0000000140000000(4): 3 instructions",
               "1|L--|v3 = SLICE(xmm1, real32, 0) - SLICE(xmm5, real32, 0)",
               "2|L--|v5 = SLICE(xmm1, word96, 32)",
               "3|L--|xmm1 = SEQ(v5, v3)");
        }

        [Test]
        public void X86rw_vsubss()
        {
            Run64bitTest("C5BE5CCD");
            AssertCode(     // vsubss xmm1,xmm8,xmm5
               "0|L--|0000000140000000(4): 2 instructions",
               "1|L--|v3 = SLICE(xmm8, real32, 0) - SLICE(xmm5, real32, 0)",
               "2|L--|xmm1 = SEQ(0<96>, v3)");
        }

        [Test]
        public void X86rw_cvtsi2ss()
        {
            Run64bitTest(0xF3, 0x48, 0x0F, 0x2A, 0xC0);
            AssertCode(     // "cvtsi2ss\txmm0,rax", 
               "0|L--|0000000140000000(5): 2 instructions",
               "1|L--|v4 = CONVERT(rax, int64, real32)",
               "2|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), v4)");
        }

        [Test]
        public void X86rw_cvtpi2ps()
        {
            Run64bitTest("49 0F 2A C5");
            AssertCode( // cvtpi2ps xmm0, mm5
               "0|L--|0000000140000000(4): 3 instructions",
               "1|L--|v3 = CONVERT(SLICE(mm5, int32, 0), int32, real32)",
               "2|L--|v4 = CONVERT(SLICE(mm5, int32, 32), int32, real32)",
               "3|L--|xmm0 = SEQ(v4, v3)");
        }

        [Test]
        public void X86rw_addps()
        {
            Run64bitTest("0F 58 0D FB B0 00 00");
            AssertCode( // addps xmm1,[0000000000415EF4]
                "0|L--|0000000140000000(7): 3 instructions",
                "1|L--|v3 = xmm1",
                "2|L--|v4 = Mem0[0x000000014000B102<p64>:word128]",
                "3|L--|xmm1 = __addps(v3, v4)");
        }

        [Test]
        public void X86Rw_divpd()
        {
            Run64bitTest("660F5EF1");
            AssertCode(     // divpd	xmm6,xmm1
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v4 = xmm6",
                "2|L--|v5 = xmm1",
                "3|L--|xmm6 = __divpd(v4, v5)");
        }

        [Test]
        public void X86rw_divsd()
        {
            Run64bitTest("F2 0F 5E C1");
            AssertCode( // divsd xmm0,xmm1
               "0|L--|0000000140000000(4): 3 instructions",
               "1|L--|v3 = SLICE(xmm0, real64, 0) / SLICE(xmm1, real64, 0)",
               "2|L--|v5 = SLICE(xmm0, word64, 64)",
               "3|L--|xmm0 = SEQ(v5, v3)");
        }

        [Test]
        public void X86rw_mulsd()
        {
            Run64bitTest("F2 0F 59 05 92 AD 00 00 ");
            AssertCode(     // mulsd xmm0,qword ptr[rip + ad92]
               "0|L--|0000000140000000(8): 3 instructions",
               "1|L--|v3 = SLICE(xmm0, real64, 0) * Mem0[0x000000014000AD9A<p64>:real64]",
               "2|L--|v4 = SLICE(xmm0, word64, 64)",
               "3|L--|xmm0 = SEQ(v4, v3)");
        }

        [Test]
        public void X86rw_subps()
        {
            Run64bitTest("0F 5C 05 61 AA 00 00");
            AssertCode( // subps xmm0,[0000000000415F0C]
               "0|L--|0000000140000000(7): 3 instructions",
               "1|L--|v3 = xmm0",
               "2|L--|v4 = Mem0[0x000000014000AA68<p64>:word128]",
               "3|L--|xmm0 = __subps(v3, v4)");
        }

        [Test]
        public void X86rw_cvttss2si()
        {
            Run64bitTest("F3 4C 0F 2C F8");
            AssertCode(     // cvttss2si r15d, xmm0
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|r15d = CONVERT(SLICE(xmm0, real32, 0), real32, int32)");
        }

        [Test]
        public void X86rw_mulps()
        {
            Run64bitTest("0F 59 4A 08");
            AssertCode(     // mulps xmm1,[rdx+08]
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v4 = xmm1",
                "2|L--|v5 = Mem0[rdx + 8<64>:word128]",
                "3|L--|xmm1 = __mulps(v4, v5)");
        }

        [Test]
        public void X86rw_mulss()
        {
            Run64bitTest("F3 0F 59 D8");
            AssertCode(     // mulss xmm3, xmm0
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v3 = SLICE(xmm3, real32, 0) * SLICE(xmm0, real32, 0)",
                "2|L--|v5 = SLICE(xmm3, word96, 32)",
                "3|L--|xmm3 = SEQ(v5, v3)");
        }

        [Test]
        public void X86rw_vmulss()
        {
            Run64bitTest("C5 BE 59 D8");
            AssertCode(     // mulss xmm3, xmm8, xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = SLICE(xmm8, real32, 0) * SLICE(xmm0, real32, 0)",
                "2|L--|xmm3 = SEQ(0<96>, v3)");
        }

        [Test]
        public void X86Rw_mulx()
        {
            Run64bitTest("C442FBF6E2");
            AssertCode(     // mulx	r12,rax,rdx,r10
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|r12_rax = rdx *u128 r10");
        }

        [Test]
        public void X86rw_divss()
        {
            Run64bitTest("F3 0F 5E C1");
            AssertCode(     // divss xmm0, xmm1
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v3 = SLICE(xmm0, real32, 0) / SLICE(xmm1, real32, 0)",
                "2|L--|v5 = SLICE(xmm0, word96, 32)",
                "3|L--|xmm0 = SEQ(v5, v3)");
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
                "2|L--|Top = Top - 1<i8>",
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
              "1|L--|ST[Top:real64] = ST[Top:real64] * CONVERT(Mem0[0x0000000140009F8F<p64>:real32], real32, real64)");
        }

        [Test]
        public void X86Rw_fneni()
        {
            Given_HexString("DBE0");
            AssertCode(     // fneni
                "0|L--|10000000(2): 1 instructions",
                "1|L--|nop");
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
        public void X86rw_x64_push_immediate_32bit()
        {
            Run64bitTest(0x6A, 0xC2);
            AssertCode(     // "push 0xC2", 
                "0|L--|0000000140000000(2): 2 instructions",
                "1|L--|rsp = rsp - 4<i64>",
                "2|L--|Mem0[rsp:word32] = 0xFFFFFFC2<32>");
        }

        [Test]
        public void X86rw_x64_push_immediate_64bit()
        {
            Run64bitTest(0x48, 0x6A, 0xC2);
            AssertCode(     // "push 0xC2", 
                "0|L--|0000000140000000(3): 2 instructions",
                "1|L--|rsp = rsp - 8<i64>",
                "2|L--|Mem0[rsp:word64] = 0xFFFFFFFFFFFFFFC2<64>");
        }

        [Test]
        public void X86rw_x64_push_register()
        {
            Run64bitTest(0x53);
            AssertCode(     // "push rbx", 
                "0|L--|0000000140000000(1): 2 instructions",
                "1|L--|rsp = rsp - 8<i64>",
                "2|L--|Mem0[rsp:word64] = rbx");
        }

        [Test]
        public void X86rw_x64_push_memoryload()
        {
            Run64bitTest(0xFF, 0x75, 0xE0);
            AssertCode(     // "push rbx", 
                "0|L--|0000000140000000(3): 3 instructions",
                "1|L--|v4 = Mem0[rbp - 0x20<64>:word64]",
                "2|L--|rsp = rsp - 8<i64>",
                "3|L--|Mem0[rsp:word64] = v4");
        }

        [Test]
        public void X86rw_push_segreg()
        {
            Run32bitTest(0x06);
            AssertCode(     // "push es", 
                "0|L--|10000000(1): 2 instructions",
                "1|L--|esp = esp - 2<i32>",
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
                "1|L--|xmm0 = SEQ(0<96>, Mem0[0x0000000140000359<p64>:real32])");
        }

        [Test]
        public void X86rw_vmovss_store()
        {
            Run64bitTest(0xC5, 0xFA, 0x11, 0x85, 0x2C, 0xFF, 0xFF, 0xFF); // vmovss dword ptr [rbp-0xd4], xmm0
            AssertCode(
                "0|L--|0000000140000000(8): 1 instructions",
                "1|L--|Mem0[rbp - 0xD4<64>:real32] = SLICE(xmm0, real32, 0)");
        }

        [Test]
        public void X86rw_vcvtsi2ss()
        {
            Run64bitTest(0xC4, 0xE1, 0xFA, 0x2A, 0xC0);     // vcvtsi2ss\txmm0,xmm0,rax
            AssertCode(
             "0|L--|0000000140000000(5): 2 instructions",
             "1|L--|v4 = CONVERT(rax, int64, real32)",
             "2|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), v4)");
        }

        [Test]
        public void X86rw_vcvtsi2sd()
        {
            Run64bitTest(0xC4, 0xE1, 0xFB, 0x2A, 0xC2); // vcvtsi2sd\txmm0,xmm0,rdx
            AssertCode(
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = CONVERT(rdx, int64, real64)",
                "2|L--|xmm0 = SEQ(SLICE(xmm0, word64, 64), v4)");
        }

        [Test]
        public void X86rw_vmovsd()
        {
            Run64bitTest(0xC5, 0xFB, 0x11, 0x01); // vmovsd double ptr[rcx], xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|Mem0[rcx:real64] = SLICE(xmm0, real64, 0)");
        }


        [Test]
        public void X86Rw_vminpd()
        {
            Run64bitTest("C401E95DFB");
            AssertCode(     // vminpd	xmm15,xmm2,xmm11
                "0|L--|0000000140000000(5): 3 instructions",
                "1|L--|v5 = xmm2",
                "2|L--|v6 = xmm11",
                "3|L--|xmm15 = __minpd(v5, v6)");
        }

        [Test]
        public void X86Rw_vpbroadcastb()
        {
            Run64bitTest("C4E27D78C0");
            AssertCode(     // vpbroadcastb	ymm0,al
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|ymm0 = __pbroadcastb(al)");
        }

        [Test]
        public void X86Rw_vpcmpeqb()
        {
            Run64bitTest("C5ED74D8");
            AssertCode(     // vpcmpeqb	ymm3,ymm0
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v4 = ymm3",
                "2|L--|v5 = ymm0",
                "3|L--|ymm3 = __pcmpeqb(v4, v5)");
        }

        [Test]
        public void X86Rw_vpmaddubsw()
        {
            Run64bitTest("660F3804D6");
            AssertCode(     // vpmaddubsw	xmm2,xmm6
                "0|L--|0000000140000000(5): 3 instructions",
                "1|L--|v4 = xmm2",
                "2|L--|v5 = xmm6",
                "3|L--|xmm2 = __pmaddubsw(v4, v5)");
        }

        [Test]
        public void X86Rw_vpmovmskb()
        {
            Run64bitTest("C5FDD7CA");
            AssertCode(     // vpmovmskb	ecx,ymm2
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = __pmovmskb(ymm2)",
                "2|L--|ecx = SEQ(SLICE(ecx, word24, 8), v4)");
        }

        [Test]
        public void X86Rw_vpsubsw()
        {
            Run64bitTest("C501E92F");
            AssertCode(     // vpsubsw	xmm13,xmm15,[rdi]
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v5 = xmm15",
                "2|L--|v6 = Mem0[rdi:word128]",
                "3|L--|xmm13 = __psubsw(v5, v6)");
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
                "1|L--|v3 = SLICE(xmm0, real64, 0) + SLICE(xmm0, real64, 0)",
                "2|L--|xmm0 = SEQ(0<64>, v3)");
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
                "1|L--|xmm0 = __andnps(xmm0, Mem0[edx + 0x42<32>:word128])");
    }

        [Test]
        public void X86rw_andps()
        {
            Run32bitTest(0x0F, 0x54, 0x42, 0x42);    // andps\txmm0,[edx+42]
            AssertCode(
               "0|L--|10000000(4): 3 instructions",
               "1|L--|v4 = xmm0",
               "2|L--|v5 = Mem0[edx + 0x42<32>:word128]",
               "3|L--|xmm0 = __andps(v4, v5)");
        }

        [Test]
        public void X86Rw_bextr()
        {
            Run64bitTest("C4C228F7F9");
            AssertCode(     // bextr	edi,r9d,r10d
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|edi = __bextr(r9d, r10d)",
                "2|L--|Z = edi == 0<32>");
        }

        [Test]
        public void X86Rw_blsi()
        {
            Run64bitTest("C4C298F3DB");
            AssertCode(     // blsi	r12,r11
                "0|L--|0000000140000000(5): 4 instructions",
                "1|L--|r12 = __blsi(r11)",
                "2|L--|Z = r12 == 0<64>",
                "3|L--|S = r12 <= 0<64>",
                "4|L--|C = r11 == 0<64>");
        }

        [Test]
        public void X86Rw_blsmsk()
        {
            Run64bitTest("C4C290F3D2");
            AssertCode(     // blsmsk	r13,r10
                "0|L--|0000000140000000(5): 4 instructions",
                "1|L--|r13 = __blsmsk(r10)",
                "2|L--|Z = r13 == 0<64>",
                "3|L--|S = r13 <= 0<64>",
                "4|L--|C = r10 == 0<64>");
        }

        [Test]
        public void X86Rw_blsr()
        {
            Run64bitTest("C4E2A8F3CA");
            AssertCode(     // blsr r10,rdx
                "0|L--|0000000140000000(5): 4 instructions",
                "1|L--|r10 = __blsr(rdx)",
                "2|L--|Z = r10 == 0<64>",
                "3|L--|S = r10 <= 0<64>",
                "4|L--|C = rdx == 0<64>");
        }


        [Test]
        public void X86rw_bsf()
        {
            Run32bitTest(0x0F, 0xBC, 0x42, 0x42);    // bsf\teax,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|Z = Mem0[edx + 0x42<32>:word32] == 0<32>",
                "2|L--|eax = __bsf(Mem0[edx + 0x42<32>:word32])");
        }

        [Test]
        public void X86rw_btc()
        {
            Run32bitTest(0x0F, 0xBB, 0x42, 0x42);    // btc\teax,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|C = __btc(eax, Mem0[edx + 0x42<32>:word32], out eax)");
        }

        [Test]
        public void X86rw_btr()
        {
            Run32bitTest(0x0F, 0xBA, 0xF3, 0x00);
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|C = __btr(ebx, 0<8>, out ebx)");
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
        public void X86Rw_bzhi()
        {
            Run64bitTest("C46290F5EB");
            AssertCode(     // bzhi	r13,rbx,r13
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|r13 = __bzhi(rbx, r13)");
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
                "2|L--|v5 = Mem0[edx + 0x42<32>:word128]",
                "3|L--|xmm0 = __cmpps(v4, v5, 8<8>)");
        }

        [Test]
        public void X86rw_comiss()
        {
            Run32bitTest(0x0F, 0x2F, 0x42, 0x42);    // comiss\txmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|CZP = cond(SLICE(xmm0, real32, 0) - Mem0[edx + 0x42<32>:real32])",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86rw_comiss_reg()
        {
            Run32bitTest(0x0F, 0x2F, 0xCF);
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|CZP = cond(SLICE(xmm1, real32, 0) - SLICE(xmm7, real32, 0))",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86rw_cvtdq2ps()
        {
            Run32bitTest(0x0F, 0x5B, 0x42, 0x42);    // cvtdq2ps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v3 = Mem0[edx + 0x42<32>:word128]",
                "2|L--|xmm0 = __cvtdq2ps(v3)");
        }

        [Test]
        public void X86rw_cvtps2pd()
        {
            Run32bitTest(0x0F, 0x5A, 0x42, 0x42);    // cvtps2pd\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v3 = Mem0[edx + 0x42<32>:word128]",
                "2|L--|xmm0 = __cvtps2pd(v3)");
        }

        [Test]
        public void X86rw_cvtps2pi()
        {
            Run32bitTest(0x0F, 0x2D, 0x42, 0x42);    // cvtps2pi\tmm0,xmmword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v3 = Mem0[edx + 0x42<32>:word128]",
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
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovbe()
        {
            Run32bitTest(0xDA, 0xD1);    // fcmovbe\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(GT,CZ)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmove()
        {
            Run32bitTest(0xDA, 0xC9);    // fcmove\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovnbe()
        {
            Run32bitTest(0xDB, 0xD1);    // fcmovnbe\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(LE,CZ)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovne()
        {
            Run32bitTest(0xDB, 0xC9);    // fcmovne\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovnu()
        {
            Run32bitTest(0xDB, 0xD9);    // fcmovnu\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(IS_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovu()
        {
            Run32bitTest(0xDA, 0xD9);    // fcmovu\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NOT_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcomip()
        {
            Run32bitTest(0xDF, 0xF2);    // fcomip\tst(0),st(2)
            AssertCode(
                "0|L--|10000000(2): 4 instructions",
                "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 2<i8>:real64])",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86rw_ffree()
        {
            Run32bitTest(0xDD, 0xC2);    // ffree\tst(2)
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__ffree(ST[Top + 2<i8>:real64])");
        }

        [Test]
        public void X86rw_fild_i16()
        {
            Run32bitTest(0xDF, 0x40, 0x42);    // fild\tword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[eax + 0x42<32>:int16], int16, real64)");
        }

        [Test]
        public void X86rw_fisttp()
        {
            Run32bitTest(0xDB, 0x08);    // fisttp\tdword ptr [eax]
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|Mem0[eax:int32] = CONVERT(trunc(ST[Top:real64]), real64, int32)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fisttp_int16()
        {
            Run32bitTest(0xDF, 0x48, 0x42);    // fisttp\tword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[eax + 0x42<32>:int16] = CONVERT(trunc(ST[Top:real64]), real64, int16)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fisttp_int64()
        {
            Run32bitTest(0xDD, 0x48, 0x42);    // fisttp\tqword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[eax + 0x42<32>:int64] = CONVERT(trunc(ST[Top:real64]), real64, int64)");
        }

        [Test]
        public void X86rw_fld_real80()
        {
            Run32bitTest(0xDB, 0x28);    // fld\ttword ptr [eax]
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[eax:real80], real80, real64)");
        }

        [Test]
        public void X86rw_fucom()
        {
            Run32bitTest(0xDD, 0xE5);    // fucom\tst(5),st(0)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|FPUF = cond(ST[Top + 5<i8>:real64] - ST[Top:real64])");
        }

        [Test]
        public void X86rw_fcomp()
        {
            Run32bitTest("D8 D9");               // fcomp st(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 1<i8>:real64])");
        }

        [Test]
        public void X86rw_fucomp()
        {
            Run32bitTest(0xDD, 0xEA);    // fucomp\tst(2)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 2<i8>:real64])");
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
                "1|L--|eax = __lar(Mem0[edx + 0x42<32>:word16])",
                "2|L--|Z = true");
        }

        [Test]
        public void X86rw_lsl()
        {
            Run32bitTest(0x0F, 0x03, 0x42, 0x42);    // lsl\teax,word ptr [edx+42]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|eax = __lsl(Mem0[edx + 0x42<32>:word16])");
        }

        [Test]
        public void X86rw_lss_64bit()
        {
            Run64bitTest("480FB206");
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ss_rax = Mem0[rsi + 0<64>:segptr80]");
        }

        [Test]
        public void X86Rw_maskmovdqu()
        {
            Run32bitTest("660FF7E2");
            AssertCode(     // maskmovdqu	xmm4,xmm2
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm4 = __maskmovdqu(xmm4, xmm2)");
        }

        [Test]
        public void X86rw_maskmovq()
        {
            Run32bitTest(0x0F, 0xF7, 0x42, 0x42);    // maskmovq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __maskmovq(mm0, Mem0[edx + 0x42<32>:word64])");
        }

        [Test]
        public void X86rw_minps()
        {
            Run32bitTest(0x0F, 0x5D, 0x42, 0x42);    // minps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word128]",
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
                "0|R--|10000000(2): 2 instructions",
                "1|L--|__sysexit()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void X86rw_sysret()
        {
            Run64bitTest(0x0F, 0x07);    // sysret
            AssertCode(
                "0|R--|0000000140000000(2): 2 instructions",
                "1|L--|__sysret()",
                "2|R--|return (0,0)");
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
                "2|L--|v5 = Mem0[edx + 0x42<32>:word128]",
                "3|L--|xmm0 = __unpcklps(v4, v5)");
            Run32bitTest(0x66, 0x0F, 0x14, 0x42, 0x42);    // unpcklpd\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word128]",
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
                "1|L--|__prefetchw(Mem0[edx + 0x42<32>:word32])");
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
                "2|L--|Mem0[edx + 0x42<32>:word64] = __movhps(v4)");
            Run32bitTest(0x66, 0x0F, 0x17, 0x42, 0x42);    // movhpd\tqword ptr [edx+42],xmm0
            AssertCode(
                "0|L--|10000000(5): 2 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|Mem0[edx + 0x42<32>:word64] = __movhpd(v4)");
        }

        [Test]
        public void X86rw_movlps()
        {
            Run32bitTest(0x0F, 0x12, 0x42, 0x42);    // movlps\txmm0,qword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 0x42<32>:word64]",
                "2|L--|xmm0 = __movlps(v4)");
        }

        [Test]
        public void X86rw_movmskps()
        {
            Run32bitTest(0x0F, 0x50, 0x42);    // movmskps\teax,xmm2
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v4 = __movmskps(xmm2)",
                "2|L--|eax = SEQ(SLICE(eax, word24, 8), v4)");
        }

        [Test]
        public void X86rw_movnti()
        {
            //$TODO: should use intrisic here.
            Run32bitTest(0x0F, 0xC3, 0x42, 0x42);    // movnti\t[edx+42],eax
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 0x42<32>:word32] = eax");
        }

        [Test]
        public void X86rw_movntps()
        {
            Run32bitTest(0x0F, 0x2B, 0x42, 0x42);    // movntps\t[edx+42],xmm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 0x42<32>:word128] = xmm0");
        }


        [Test]
        public void X86rw_movntq()
        {
            Run32bitTest(0x0F, 0xE7, 0x42, 0x42);    // movntq\t[edx+42],mm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 0x42<32>:word64] = mm0");
        }


        [Test]
        public void X86rw_orps()
        {
            Run32bitTest(0x0F, 0x56, 0x42, 0x42);    // orps\txmm0,[edx+42]
            AssertCode(
               "0|L--|10000000(4): 3 instructions",
               "1|L--|v4 = xmm0",
               "2|L--|v5 = Mem0[edx + 0x42<32>:word128]",
               "3|L--|xmm0 = __orps(v4, v5)");
        }

        [Test]
        public void X86rw_packssdw()
        {
            Run32bitTest(0x0F, 0x6B, 0x42, 0x42);    // packssdw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word32]",
                "3|L--|mm0 = __packssdw(v4, v5)");
        }

        [Test]
        public void X86rw_packuswb()
        {
            Run32bitTest(0x0F, 0x67, 0x42, 0x42);    // packuswb\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word32]",
                "3|L--|mm0 = __packuswb(v4, v5)");
        }

        [Test]
        public void X86rw_paddb()
        {
            Run32bitTest(0x0F, 0xFC, 0x42, 0x42);    // paddb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __paddb(v4, v5)");
        }

        [Test]
        public void X86rw_paddd()
        {
            Run32bitTest(0x0F, 0xFE, 0x42, 0x42);    // paddd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __paddd(v4, v5)");
        }

        [Test]
        public void X86rw_paddsw()
        {
            Run32bitTest(0x0F, 0xED, 0x42, 0x42);    // paddsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __paddsw(v4, v5)");
        }

        [Test]
        public void X86rw_paddusb()
        {
            Run32bitTest(0x0F, 0xDC, 0x42, 0x42);    // paddusb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __paddusb(v4, v5)");
        }

        [Test]
        public void X86rw_paddw()
        {
            Run32bitTest(0x0F, 0xFD, 0x42, 0x42);    // paddw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
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
        public void X86Rw_pext()
        {
            Run64bitTest("C4C282F5D6");
            AssertCode(     // pext	rdx,r15,r14
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|rdx = __pext(r15, r14)");
        }

        [Test]
        public void X86Rw_pdep()
        {
            Run64bitTest("C4E2E3F5F2");
            AssertCode(     // pdep	rsi,rbx,rdx
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|rsi = __pdep(rbx, rdx)");
        }

        [Test]
        public void X86rw_pextrw()
        {
            Run32bitTest(0x0F, 0xC5, 0x42, 0x42);    // pextrw\teax,mm2,42
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|eax = __pextrw(eax, mm2, 0x42<8>)");
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
        public void X86Rw_pinsrd()
        {
            Run32bitTest("660F3A2244240802");
            AssertCode(     // pinsrd	xmm0,dword ptr [esp+8h],2h
                "0|L--|10000000(8): 1 instructions",
                "1|L--|xmm0 = __pinsrd(xmm0, Mem0[esp + 8<32>:word32], 2<8>)");
        }

        [Test]
        public void X86rw_pxor()
        {
            Run32bitTest(0x0F, 0xEF, 0x42, 0x42);    // pxor\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pxor(mm0, Mem0[edx + 0x42<32>:word64])");
        }

        [Test]
        public void X86rw_rcpps()
        {
            Run32bitTest(0x0F, 0x53, 0x42, 0x42);    // rcpps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 0x42<32>:word128]",
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
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_eax = __rdtsc()");
        }

        [Test]
        public void X86rw_rsqrtps()
        {
            Run32bitTest(0x0F, 0x52, 0x42, 0x42);    // rsqrtps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 0x42<32>:word128]",
                "2|L--|xmm0 = __rsqrtps(v4)");
        }

        [Test]
        public void X86rw_sqrtps()
        {
            Run32bitTest(0x0F, 0x51, 0x42, 0x42);    // sqrtps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 0x42<32>:word128]",
                "2|L--|xmm0 = __sqrtps(v4)");
        }

        [Test]
        public void X86rw_pcmpgtb()
        {
            Run32bitTest(0x0F, 0x64, 0x42, 0x42);    // pcmpgtb\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:(arr byte 8)]",
                "3|L--|mm0 = __pcmpgtb(v4, v5)");
        }


        [Test]
        public void X86rw_pcmpgtw()
        {
            Run32bitTest(0x0F, 0x65, 0x42, 0x42);    // pcmpgtw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:(arr word16 4)]",
                "3|L--|mm0 = __pcmpgtw(v4, v5)");
        }

        [Test]
        public void X86rw_pcmpgtd()
        {
            Run32bitTest(0x0F, 0x66, 0x42, 0x42);    // pcmpgtd\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:(arr word32 2)]",
                "3|L--|mm0 = __pcmpgtd(v4, v5)");
        }

        [Test]
        public void X86rw_punpckhbw()
        {
            Run32bitTest(0x0F, 0x68, 0x42, 0x42);    // punpckhbw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhbw(mm0, Mem0[edx + 0x42<32>:word32])");
        }

        [Test]
        public void X86rw_punpckhwd()
        {
            Run32bitTest(0x0F, 0x69, 0x42, 0x42);    // punpckhwd\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhwd(mm0, Mem0[edx + 0x42<32>:word32])");
        }

        [Test]
        public void X86rw_punpckhdq()
        {
            Run32bitTest(0x0F, 0x6A, 0x42, 0x42);    // punpckhdq\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhdq(mm0, Mem0[edx + 0x42<32>:word32])");
        }

        [Test]
        public void X86rw_punpckldq()
        {
            Run32bitTest(0x0F, 0x62, 0x42, 0x42);    // punpckldq\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckldq(mm0, Mem0[edx + 0x42<32>:word32])");
        }

        [Test]
        public void X86Rw_punpcklqdq()
        {
            Run64bitTest("660F6CC0");
            AssertCode(     // punpcklqdq	xmm0,xmm0
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklqdq(xmm0, xmm0)");
        }

        [Test]
        public void X86rw_pcmpeqd()
        {
            Run32bitTest(0x0F, 0x76, 0x42, 0x42);    // pcmpeqd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:(arr word32 2)]",
                "3|L--|mm0 = __pcmpeqd(v4, v5)");
        }

        [Test]
        public void X86rw_pcmpeqw()
        {
            Run32bitTest(0x0F, 0x75, 0x42, 0x42);    // pcmpeqw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:(arr word16 4)]",
                "3|L--|mm0 = __pcmpeqw(v4, v5)");
        }

        [Test]
        public void X86rw_vmread()
        {
            Run32bitTest(0x0F, 0x78, 0x42, 0x42);    // vmread\t[edx+42],eax
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 0x42<32>:word32] = __vmread(eax)");
        }


        [Test]
        public void X86rw_vmwrite()
        {
            Run32bitTest(0x0F, 0x79, 0x42, 0x42);    // vmwrite\teax,[edx+42]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|__vmwrite(eax, Mem0[edx + 0x42<32>:word32])");
        }


        [Test]
        public void X86rw_vshufps()
        {
            Run32bitTest(0x0F, 0xC6, 0x42, 0x42, 0x7);    // vshufps\txmm0,[edx+42],07
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word128]",
                "3|L--|xmm0 = __shufps(v4, v5, 7<8>)");
        }

        [Test]
        public void X86rw_pminub()
        {
            Run32bitTest(0x0F, 0xDA, 0x42, 0x42);    // pminub\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
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
                "2|L--|eax = SEQ(SLICE(eax, word24, 8), v4)");
        }

        [Test]
        public void X86rw_psrad()
        {
            Run32bitTest(0x0F, 0xE2, 0x42, 0x42);    // psrad\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = mm0",
                "2|L--|mm0 = __psrad(v4, Mem0[edx + 0x42<32>:word64])");
        }

        [Test]
        public void X86Rw_psrldq()
        {
            Run64bitTest("660F73DA08");
            AssertCode(     // psrldq	xmm2,8h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v3 = SLICE(xmm2, word128, 0)",
                "2|L--|xmm2 = v3");
        }

        [Test]
        public void X86rw_psrlq()
        {
            Run32bitTest(0x0F, 0xD3, 0x42, 0x42);    // psrlq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __psrlq(v4, v5)");
        }

        [Test]
        public void X86rw_psubusb()
        {
            Run32bitTest(0x0F, 0xD8, 0x42, 0x42);    // psubusb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __psubusb(v4, v5)");
        }

        [Test]
        public void X86rw_pmaxub()
        {
            Run32bitTest(0x0F, 0xDE, 0x42, 0x42);    // pmaxub\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __pmaxub(v4, v5)");
        }

        [Test]
        public void X86rw_pavgb()
        {
            Run32bitTest(0x0F, 0xE0, 0x42, 0x42);    // pavgb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 0x42<32>:(arr byte 8)]",
                "2|L--|mm0 = __pavgb(v4)");
        }

        [Test]
        public void X86rw_psraw()
        {
            Run32bitTest(0x0F, 0xE1, 0x42, 0x42);    // psraw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = mm0",
                "2|L--|mm0 = __psraw(v4, Mem0[edx + 0x42<32>:word64])");
        }

        [Test]
        public void X86rw_pmulhuw()
        {
            Run32bitTest(0x0F, 0xE4, 0x42, 0x42);    // pmulhuw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __pmulhuw(v4, v5)");
        }

        [Test]
        public void X86rw_pmulhw()
        {
            Run32bitTest(0x0F, 0xE5, 0x42, 0x42);    // pmulhw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __pmulhw(v4, v5)");
        }

        [Test]
        public void X86rw_psubb()
        {
            Run32bitTest(0x0F, 0xF8, 0x42, 0x42);    // psubb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __psubb(v4, v5)");
        }

        [Test]
        public void X86rw_psubd()
        {
            Run32bitTest(0x0F, 0xFA, 0x42, 0x42);    // psubd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __psubd(v4, v5)");
        }

        [Test]
        public void X86rw_psubq()
        {
            Run32bitTest(0x0F, 0xFB, 0x42, 0x42);    // psubq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __psubq(v4, v5)");
        }

        [Test]
        public void X86rw_psubsw()
        {
            Run32bitTest(0x0F, 0xE9, 0x42, 0x42);    // psubsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __psubsw(v4, v5)");
        }

        [Test]
        public void X86rw_psubw()
        {
            Run32bitTest(0x0F, 0xF9, 0x42, 0x42);    // psubw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __psubw(v4, v5)");
        }

        [Test]
        public void X86rw_psubsb()
        {
            Run32bitTest(0x0F, 0xE8, 0x42, 0x42);    // psubsb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __psubsb(v4, v5)");
        }

        [Test]
        public void X86rw_pmaxsw()
        {
            Run32bitTest(0x0F, 0xEE, 0x42, 0x42);    // pmaxsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __pmaxsw(v4, v5)");
        }

        [Test]
        public void X86rw_pminsw()
        {
            Run32bitTest(0x0F, 0xEA, 0x42, 0x42);    // pminsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __pminsw(v4, v5)");
        }

        [Test]
        public void X86rw_por()
        {
            Run32bitTest(0x0F, 0xEB, 0x42, 0x42);    // por\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = mm0 | Mem0[edx + 0x42<32>:word64]");
        }

        [Test]
        public void X86Rw_vpor()
        {
            Run64bitTest(0xC5, 0x01, 0xEB, 0x8B, 0xE8, 0x09, 0xE8, 0x00);
            AssertCode(     // vpor	xmm9,xmm15,[rbx+0E809E8h]
                "0|L--|0000000140000000(8): 1 instructions",
                "1|L--|xmm9 = xmm15 | Mem0[rbx + 0xE809E8<64>:word128]");
        }

        [Test]
        public void X86rw_pslld()
        {
            Run32bitTest(0x0F, 0xF2, 0x42, 0x42);    // pslld\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = mm0",
                "2|L--|mm0 = __pslld(v4, Mem0[edx + 0x42<32>:word64])");
        }

        [Test]
        public void X86rw_psllq()
        {
            Run32bitTest(0x0F, 0xF3, 0x42, 0x42);    // psllq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = mm0",
                "2|L--|mm0 = __psllq(v4, Mem0[edx + 0x42<32>:word64])");
        }

        [Test]
        public void X86rw_psllw()
        {
            Run32bitTest(0x0F, 0xF1, 0x42, 0x42);    // psllw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = mm0",
                "2|L--|mm0 = __psllw(v4, Mem0[edx + 0x42<32>:word64])");
        }

        [Test]
        public void X86rw_pmaddwd()
        {
            Run32bitTest(0x0F, 0xF5, 0x42, 0x42);    // pmaddwd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __pmaddwd(v4, v5)");
        }

        [Test]
        public void X86rw_pmuludq()
        {
            Run32bitTest(0x0F, 0xF4, 0x42, 0x42);    // pmuludq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
                "3|L--|mm0 = __pmuludq(v4, v5)");
        }

        [Test]
        public void X86rw_psadbw()
        {
            Run32bitTest(0x0F, 0xF6, 0x42, 0x42);    // psadbw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = mm0",
                "2|L--|v5 = Mem0[edx + 0x42<32>:word64]",
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
                "1|L--|xmm0 = 0<128>");
        }

        [Test]
        public void X86Rw_stmxcsr()
        {
            Run32bitTest(0x0F, 0xAE, 0x5D, 0xF0);	// stmxcsr	dword ptr [ebp-10]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[ebp - 0x10<32>:word32] = mxcsr");
        }

        [Test]
        public void X86Rw_fcmovu()
        {
            Run32bitTest(0xDA, 0xDD);	// fcmovu	st(0),st(5)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NOT_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 5<i8>:real64]");
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
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
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
                "2|L--|v5 = Mem0[ebp + 0xC<32>:word64]",
                "3|L--|mm0 = __psubusw(v4, v5)");
        }

        [Test]
        public void X86Rw_pshufw()
        {
            Run32bitTest(0x0F, 0x70, 0x02, 0x00);	// pshufw	mm0,[edx],00
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pshufw(mm0, Mem0[edx:word64], 0<8>)");
        }

        [Test]
        public void X86Rw_fxsave()
        {
            Run32bitTest("0FAE05");
            AssertCode(     // fxsave
                "0|L--|10000000(3): 1 instructions",
                "1|L--|__fxsave()");
        }

        [Test]
        public void X86Rw_fxtract()
        {
            Run32bitTest(0xD9, 0xF4);	// fxtract
            AssertCode(
                "0|L--|10000000(2): 4 instructions",
                "1|L--|v3 = ST[Top:real64]",
                "2|L--|Top = Top - 1<i8>",
                "3|L--|ST[Top + 1<i8>:real64] = __exponent(v3)",
                "4|L--|ST[Top:real64] = __significand(v3)");
        }

        [Test]
        public void X86Rw_fprem()
        {
            Run32bitTest(0xD9, 0xF8);	// fprem
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|ST[Top:real64] = __fprem_x87(ST[Top:real64], ST[Top + 1<i8>:real64])",
                "2|L--|C2 = __fprem_incomplete(ST[Top:real64])");
        }

        [Test]
        public void X86Rw_fprem1()
        {
            Run32bitTest(0xD9, 0xF5);	// fprem1	st(5),st(0)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|ST[Top:real64] = ST[Top:real64] % ST[Top + 1<i8>:real64]",
                "2|L--|C2 = __fprem_incomplete(ST[Top:real64])");
        }

        [Test]
        public void X86Rw_andpd()
        {
            Run32bitTest(0x66, 0x0F, 0x54, 0x05, 0x50, 0x59, 0x57, 0x00);	// andpd	xmm0,[00575950]
            AssertCode(
                "0|L--|10000000(8): 3 instructions",
                "1|L--|v3 = xmm0",
                "2|L--|v4 = Mem0[0x00575950<p32>:word128]",
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
        public void X86Rw_vpsrlq()
        {
            Run32bitTest(0x66, 0x0F, 0xD3, 0xCA);	// vpsrlq	xmm1,xmm2
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = xmm1",
                "2|L--|v5 = xmm2",
                "3|L--|xmm1 = __psrlq(v4, v5)");
        }

        [Test]
        public void X86Rw_pshufb()
        {
            Run32bitTest("660F38000CDD20220F08");
            AssertCode(     // pshufb	xmm1,[80F2220h+ebx*8]
                "0|L--|10000000(10): 3 instructions",
                "1|L--|v4 = xmm1",
                "2|L--|v5 = xmm1",
                "3|L--|xmm1 = __pshufb(v4, v5, Mem0[0x80F2220<32> + ebx * 8<32>:word128])");
        }

        [Test]
        public void X86Rw_pshuflw()
        {
            Run32bitTest("F20F70C0E0");
            AssertCode(     // pshuflw	xmm0,xmm0,0E0h
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v3 = xmm0",
                "2|L--|v4 = xmm0",
                "3|L--|xmm0 = __pshuflw(v3, v4, 0xE0<8>)");
        }

        [Test]
        public void X86Rw_vpsllq()
        {
            Run32bitTest(0x66, 0x0F, 0xF3, 0xCA);	// psllq	xmm1,xmm2
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = xmm1",
                "2|L--|xmm1 = __psllq(v4, xmm2)");
        }

        [Test]
        public void X86Rw_orpd()
        {
            Run32bitTest(0x66, 0x0F, 0x56, 0x1D, 0xA0, 0x59, 0x57, 0x00);	// orpd	xmm3,[005759A0]
            AssertCode(
                "0|L--|10000000(8): 3 instructions",
                "1|L--|v3 = xmm3",
                "2|L--|v4 = Mem0[0x005759A0<p32>:word128]",
                "3|L--|xmm3 = __orpd(v3, v4)");
        }

        [Test]
        public void X86Rw_movlpd()
        {
            Run32bitTest(0x66, 0x0F, 0x12, 0x44, 0x24, 0x04);	// movlpd	xmm0,qword ptr [esp+04]
            AssertCode(
                "0|L--|10000000(6): 2 instructions",
                "1|L--|v4 = Mem0[esp + 4<32>:word64]",
                "2|L--|xmm0 = __movlpd(v4)");
        }

        [Test]
        public void X86Rw_vextrw()
        {
            Run32bitTest(0x66, 0x0F, 0xC5, 0xC0, 0x03);	// vextrw	eax,xmm0,03
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|eax = __pextrw(eax, xmm0, 3<8>)");
        }

        [Test]
        public void X86Rw_cvtsd2si()
        {
            Run32bitTest(0xF2, 0x0F, 0x2D, 0xD1);	// cvtsd2si	edx,xmm1
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|edx = CONVERT(SLICE(xmm1, real64, 0), real64, int32)");
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
                "1|L--|mxcsr = Mem0[ebp + 8<32>:word32]");
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
                "1|L--|v3 = CONVERT(Mem0[0x00000001403247BE<p64>:real32], real32, real64)",
                "2|L--|xmm1 = SEQ(SLICE(xmm1, word64, 64), v3)");
        }


        [Test]
        public void X86Rw_cvtsd2ss()
        {
            Run64bitTest(0xF2, 0x48, 0x0F, 0x5A, 0xC0);	// cvtsd2ss	xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v3 = CONVERT(SLICE(xmm0, real64, 0), real64, real32)",
                "2|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), v3)");
        }

        [Test]
        public void X86Rw_cvtss2si()
        {
            Run64bitTest(0xF3, 0x48, 0x0F, 0x2D, 0x50, 0x10);	// cvtss2si	rdx,dword ptr [rax+10]
            AssertCode(
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|rdx = CONVERT(Mem0[rax + 0x10<64>:real32], real32, int64)");
        }

        [Test]
        public void X86Rw_sqrtsd()
        {
            Run64bitTest(0xF2, 0x0F, 0x51, 0xC0);	// sqrtsd	xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = __sqrt(xmm0)",
                "2|L--|xmm0 = SEQ(SLICE(xmm0, word64, 64), v3)");
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
                "2|L--|v5 = Mem0[rdx + 0x42<64>:word128]",
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
                "1|L--|Mem0[edx + 0x50486<32>:word48] = __sidt()");
        }

        [Test]
        public void X86Rw_lldt()
        {
            Run32bitTest(0x0F, 0x00, 0x55, 0x8D);	// lldt	word ptr [ebp-73]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|__lldt(Mem0[ebp - 0x73<32>:word48])");
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
                "1|L--|v4 = Mem0[ebp - 0x18<32>:word128]",
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

        [Test]
         public void X86Rw_cmpxchg8b()
        {
            Run32bitTest(0x0F, 0xC7, 0x0F);	// cmpxchg8b	qword ptr [edi]
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|Z = __cmpxchg8b(edx_eax, Mem0[edi:word64], ecx_ebx, out edx_eax)");
        }

        [Test]
        public void X86Rw_unpckhps()
        {
            Run32bitTest(0x0F, 0x15, 0x10);	// unpckhps	xmm2,[eax]
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|v4 = xmm2",
                "2|L--|v5 = Mem0[eax:word128]",
                "3|L--|xmm2 = __unpckhps128(v4, v5)");
        }

        [Test]
        public void X86Rw_ltr()
        {
            Run32bitTest(0x0F, 0x00, 0x98, 0x3F, 0x05, 0x10, 0x19);	// ltr	word ptr [eax+1910053F]
            AssertCode(
                "0|S--|10000000(7): 1 instructions",
                "1|L--|__load_task_register(Mem0[eax + 0x1910053F<32>:word16])");
        }

        [Test]
        public void X86Rw_ffreep()
        {
            Run32bitTest(0xDF, 0xC7);	// ffreep	st(7)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|__ffree(ST[Top + 7<i8>:real64])",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86Rw_verr()
        {
            Run32bitTest(0x0F, 0x00, 0xA5, 0x64, 0x0F, 0x00, 0xA5);	// verr	word ptr [ebp+A5000F64]
            AssertCode(
                "0|L--|10000000(7): 1 instructions",
                "1|L--|Z = __verify_readable(Mem0[ebp + 0xA5000F64<32>:word16])");
        }

        [Test]
        public void X86Rw_verw()
        {
            Run32bitTest(0x0F, 0x00, 0xEB);	// verw	bx
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|Z = __verify_writeable(bx)");
        }

        [Test]
        public void X86Rw_str()
        {
            Run32bitTest(0x0F, 0x00, 0x8B, 0xF3, 0x8B, 0x4D, 0xFC);	// str	word ptr [ebx+FC4D8BF3]
            AssertCode(
                "0|S--|10000000(7): 1 instructions",
                "1|L--|Mem0[ebx + 0xFC4D8BF3<32>:word16] = __store_task_register()");
        }

        [Test]
        public void X86Rw_jmpe()
        {
            Run32bitTest(0x0F, 0xB8);	// jmpe
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__jmpe()");
        }

        [Test]
        public void X86Rw_femms()
        {
            Run32bitTest(0x0F, 0x0E);	// femms
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__femms()");
        }

        [Test]
        public void X86Rw_invlpg()
        {
            Run32bitTest(0x0F, 0x01, 0x38);	// invlpg	byte ptr [eax]
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|__invlpg(Mem0[eax:byte])");
        }

        [Test]
        public void X86Rw_rsm()
        {
            Run32bitTest(0x0F, 0xAA);	// rsm
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void X86Rw_movdqa()
        {
            Run64bitTest(0x66, 0x0F, 0x6F, 0x4D, 0xC0);
            AssertCode(
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm1 = Mem0[rbp - 0x40<64>:word128]");
        }

        [Test]
        public void X86Rw_vmovdqa()
        {
            Run64bitTest(0xC5, 0xF9, 0x6F, 0x4D, 0xC0);
            AssertCode(
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm1 = Mem0[rbp - 0x40<64>:word128]");
        }

        [Test]
        public void X86Rw_vdivss()
        {
            Run64bitTest("C5 FB 5E 45 E8");
            AssertCode(
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v3 = SLICE(xmm0, real64, 0) / Mem0[rbp - 0x18<64>:real64]",
                "2|L--|xmm0 = SEQ(0<64>, v3)");
        }

        [Test]
        public void X86Rw_push_64()
        {
            Run64bitTest("66 51");
            AssertCode(
                "0|L--|0000000140000000(2): 2 instructions",
                "1|L--|rsp = rsp - 2<i64>",
                "2|L--|Mem0[rsp:word16] = cx");
        }

        /*
        R:push   0x8f865955<32>                        68 55 59 86 8F
        O:push   0xffffffff8f865955<64>                68 55 59 86 8F
        */

        /*
         * R:imul   eax,esi,0xea                      6B C6 EA
        O:imul   eax,esi,0xffffffea<32>                6B C6 EA
        */

        [Test]
        public void X86Rw_add_64_rex()
        {
            Run64bitTest("48 05 44 EB 24 C4"); // add rax,0xffffffffc424eb44<64>
            AssertCode(
                "0|L--|0000000140000000(6): 2 instructions",
                "1|L--|rax = rax + 0xFFFFFFFFC424EB44<64>",
                "2|L--|SCZO = cond(rax)");
        }

        [Test]
        public void X86Rw_and_64_rex()
        {
            Run64bitTest("48 25 44 EB 24 C4"); // and rax,0xffffffffc424eb44<64>
            //$REVIEW: and's should be unsigned masks no?
            AssertCode(
                "0|L--|0000000140000000(6): 3 instructions",
                "1|L--|rax = rax & 0xFFFFFFFFC424EB44<64>",
                "2|L--|SZO = cond(rax)",
                "3|L--|C = false");
        }

        [Test]
        public void X86Rw_cwd()
        {
            Run16bitTest(0x99); // cwd
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|dx_ax = CONVERT(ax, int16, int32)");
        }

        [Test]
        public void X86Rw_cdq()
        {
            Run32bitTest(0x99); // cdq
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|edx_eax = CONVERT(eax, int32, int64)");
        }


        [Test]
        public void X86Rw_cqo()
        {
            Run64bitTest(0x48, 0x99); // cqo
            AssertCode(
                "0|L--|0000000140000000(2): 1 instructions",
                "1|L--|rdx_rax = CONVERT(rax, int64, int128)");
        }


        [Test]
        public void X86Rw_cbw()
        {
            Run16bitTest(0x98); // cbw
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|ax = CONVERT(al, int8, int16)");
        }

        [Test]
        public void X86Rw_cwde()
        {
            Run32bitTest(0x98); // cwde
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|eax = CONVERT(ax, int16, int32)");
        }

        [Test]
        public void X86Rw_cdqe()
        {
            Run64bitTest(0x48, 0x98); // cdqe
            AssertCode(
                "0|L--|0000000140000000(2): 1 instructions",
                "1|L--|rax = CONVERT(eax, int32, int64)");
        }

        [Test]
        public void X86Rw_icebp()
        {
            Run32bitTest("F1");
            AssertCode(
                "0|---|10000000(1): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86Rw_adc_imm8()
        {
            Run32bitTest("83 56 FE FF");
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v4 = Mem0[esi - 2<32>:word32] + 0xFFFFFFFF<32> + C",
                "2|L--|Mem0[esi - 2<32>:word32] = v4",
                "3|L--|SCZO = cond(v4)");
        }

        [Test]
        public void X86Rw_fstsw_and_cmp_jz__eq()
        {
            Run32bitTest(
                "DF E0" +       // fstsw	ax
                "80 E4 45" +    // and	ah,45
                "80 FC 40" +    // cmp	ah,40
                "74 02");       // jz	$+4
            AssertCode(
                "0|T--|10000000(10): 1 instructions",
                "1|T--|if (Test(EQ,FPUF)) branch 1000000C");
        }

        [Test]
        public void X86Rw_fstsw_and_xor_40_jz__ne()
        {
            Run32bitTest(
                "DF E0" +       // fstsw	ax
                "80 E4 45" +    // and	ah,45
                "80 F4 40" +    // xor ah, 40
                "74 02");       // jz	$+4
            AssertCode(
                "0|T--|10000000(10): 1 instructions",
                "1|T--|if (Test(NE,FPUF)) branch 1000000C");
        }

        [Test]
        public void X86Rw_fstsw_test_45_jz__gt()
        {
            Run32bitTest(
                "DF E0" +       // fstsw	ax
                "F6 C4 45" +    // test	ah,45
                "74 02");       // jz	0804849B
            AssertCode(
                "0|T--|10000000(7): 2 instructions",
                "1|L--|SCZO = FPUF",
                "2|T--|if (Test(GT,FPUF)) branch 10000009");
        }

        [Test]
        public void X86Rw_fstsw_ah_44_pe()
        {
            Run32bitTest(
                "D8 5C 24 24" + // fcomp dword ptr [esp+24h]
                "DF E0" +       // fstsw ax
                "89 54 24 2C" + // mov [esp+2Ch],edx
                "F6 C4 44" +    // test ah,44h
                "7A 26");       // jpe 0D2316h
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|FPUF = cond(ST[Top:real64] - Mem0[esp + 0x24<32>:real32])",
                "2|L--|Top = Top + 1<i8>",
                "3|T--|10000004(11): 3 instructions",
                "4|L--|Mem0[esp + 0x2C<32>:word32] = edx",
                "5|L--|SCZO = FPUF",
                "6|T--|if (Test(NE,FPUF)) branch 10000035");
        }

        [Test]
        public void X86Rw_fstsw_test_05_jz__ge()
        {
            Run32bitTest(
                "DF E0" +       // fstsw	ax
                "F6 C4 05" +    // test	ah,05 -- ge
                "74 02");       // jz	080484BE
            AssertCode(
                "0|T--|10000000(7): 2 instructions",
                "1|L--|SCZO = FPUF",
                "2|T--|if (Test(GE,FPUF)) branch 10000009");
        }

        [Test]
        public void X86rw_short_push()
        {
            Run32bitTest(
                "66 68 34 12"   // push word 1234h
                );
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|esp = esp - 2<i32>",
                "2|L--|Mem0[esp:word16] = 0x1234<16>");
        }

        [Test]
        public void X86Rw_fld_st0()
        {
            // This is a 'duplicate top of stack' instruction.
            Run32bitTest("D9 C0"); // fld st(0)");
            AssertCode(
              "0|L--|10000000(2): 3 instructions",
              "1|L--|v3 = ST[Top:real64]",
              "2|L--|Top = Top - 1<i8>",
              "3|L--|ST[Top:real64] = v3");
        }

        [Test]
        public void X86Rw_fsubr()
        {
            Run32bitTest("D8 AD 78 FF FF FF"); // fsubr dword ptr [ebp-00000088]
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|ST[Top:real64] = CONVERT(Mem0[ebp - 0x88<32>:real32], real32, real64) - ST[Top:real64]");
        }

        [Test]
        public void X86Rw_lea_short_dst()
        {
            Run64bitTest("8D 4C 09 01"); // lea ecx,[rcx+rcx+01]
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|ecx = SLICE(rcx + 1<64> + rcx, word32, 0)",
                "2|L--|rcx = CONVERT(ecx, word32, uint64)");
        }

        [Test]
        public void X86Rw_lea_16bit_dst()
        {
            Run64bitTest("668D03"); // lea\tax,[rbx]
            AssertCode(
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|ax = SLICE(rbx, word16, 0)");
        }

        [Test]
        public void X86Rw_lea_16bit_dst_32bit_src()
        {
            Run64bitTest("67668D03"); // lea\tax,[ebx]
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ax = SLICE(ebx, word16, 0)");
        }

        [Test]
        public void X86Rw_lea_32bit_dst_64bit_src()
        {
            Run64bitTest("8D03"); // lea\teax,[rbx]
            AssertCode(
                "0|L--|0000000140000000(2): 2 instructions",
                "1|L--|eax = SLICE(rbx, word32, 0)",
                "2|L--|rax = CONVERT(eax, word32, uint64)");
        }

        [Test]
        public void X86Rw_lea_32bit_dst_32bit_src()
        {
            Run64bitTest("678D03"); // lea\teax,[ebx]
            AssertCode(
                "0|L--|0000000140000000(3): 2 instructions",
                "1|L--|eax = ebx",
                "2|L--|rax = CONVERT(eax, word32, uint64)");
        }

        [Test]
        public void X86Rw_lea_64bit_dst_64bit_src()
        {
            Run64bitTest("488D03"); // lea\teax,[rbx]
            AssertCode(
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|rax = rbx");
        }

        [Test]
        public void X86Rw_lea_64bit_dst_32bit_src()
        {
            Run64bitTest("67488D03"); // lea\trax,[ebx]
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|rax = CONVERT(ebx, word32, word64)");
        }

        [Test]
        public void X86Rw_movdqu()
        {
            Run64bitTest("F30F7F01");
            AssertCode(     // movdqu	[rcx],xmm0
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|Mem0[rcx:word128] = xmm0");
        }

        [Test]
        public void X86Rw_cvtdq2pd()
        {
            Run64bitTest("F30FE6C9");
            AssertCode(     // cvtdq2pd	xmm1,xmm1
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = xmm1",
                "2|L--|xmm1 = __cvtdq2pd(v3)");
        }

        [Test]
        public void X86Rw_aesenc()
        {
            Run64bitTest("660F38DCC0");
            AssertCode(     // vaesenc	xmm0,xmm0
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm0 = __aesenc(xmm0, xmm0)");
        }

        [Test]
        public void X86Rw_lzcnt()
        {
            Run64bitTest("F30FBDCE");
            AssertCode(     // lzcnt	ecx,esi
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ecx = __lzcnt(esi)");
        }

        [Test]
        public void X86Rw_maxsd_legacy()
        {
            Run64bitTest("F20F5FD0");
            AssertCode(     // maxsd	xmm2,xmm0
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm2 = SEQ(SLICE(xmm2, word64, 64), max(SLICE(xmm2, real64, 0), SLICE(xmm0, real64, 0)))");
        }

        [Test]
        public void X86Rw_cvtpd2ps()
        {
            Run64bitTest("660F5AC0");
            AssertCode(     // cvtpd2ps	xmm0,xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = xmm0",
                "2|L--|xmm0 = __cvtpd2ps(v3)");
        }

        [Test]
        public void X86Rw_cvtpd2dq()
        {
            Run64bitTest("F20FE6E7");
            AssertCode(     // cvtpd2dq	xmm4,xmm7
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = xmm7",
                "2|L--|xmm4 = __cvtpd2dq(v3)");
        }

        [Test]
        public void X86Rw_fxrstor()
        {
            Run32bitTest("0FAE0C");
            AssertCode(     // fxrstor
                "0|L--|10000000(3): 1 instructions",
                "1|L--|__fxrstor()");
        }

        [Test]
        public void X86Rw_adcx()
        {
            Run32bitTest("660F38F6C3");
            AssertCode(     // adcx eax,ebx
                "0|L--|10000000(5): 2 instructions",
                "1|L--|eax = eax + ebx + C",
                "2|L--|C = cond(eax)");
        }

        [Test]
        public void X86Rw_adox()
        {
            Run32bitTest("F30F38F6C4");
            AssertCode(     // adox eax,esp
                "0|L--|10000000(5): 2 instructions",
                "1|L--|eax = eax + esp + O",
                "2|L--|O = cond(eax)");
        }

        [Test]
        public void X86Rw_mov_imm_64()
        {
            Run64bitTest("48B88A7A6A5A4A3A2A1A");
            AssertCode( // mov rax,1A2A3A4A5A6A7A8Ah
                "0|L--|0000000140000000(10): 1 instructions",
                "1|L--|rax = 0x1A2A3A4A5A6A7A8A<64>");
        }

        [Test]
        public void X86Rw_cmpsd()
        {
            Run64bitTest("F20FC2E806");
            AssertCode(     // cmpsd	xmm5,xmm0,6h
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm5 = SLICE(xmm5, real64, 0) > SLICE(xmm0, real64, 0) ? 0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128> : 0<128>");
        }

        [Test]
        public void X86Rw_cmpsd_x()
        {
            Run64bitTest("F20FC244241800");
            AssertCode(     // cmpsd	xmm0,[esp+18],00
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|xmm0 = SLICE(xmm0, real64, 0) == SLICE(Mem0[rsp + 0x18<64>:word128], real64, 0) ? 0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128> : 0<128>");
        }

        [Test]
        public void X86Rw_cvttpd2dq()
        {
            Run64bitTest("66450FE6C9");
            AssertCode(     // cvttpd2dq	xmm9,xmm9
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v3 = xmm9",
                "2|L--|xmm9 = __cvttpd2dq(v3)");
        }

        [Test]
        public void X86Rw_cvttps2dq()
        {
            Run64bitTest("F30F5BC0");
            AssertCode(     // cvttps2dq	xmm0,xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = xmm0",
                "2|L--|xmm0 = __cvttps2dq(v3)");
        }

        [Test]
        public void X86Rw_vpsrad()
        {
            Run64bitTest("660F72E203");
            AssertCode(     // vpsrad	xmm2,3h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v3 = xmm2",
                "2|L--|xmm2 = __psrad(v3, 3<8>)");
        }

        [Test]
        public void X86Rw_vpsraw()
        {
            Run64bitTest("660F71E008");
            AssertCode(     // vpsraw	xmm0,8h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v3 = xmm0",
                "2|L--|xmm0 = __psraw(v3, 8<8>)");
        }

        [Test]
        public void X86Rw_vpsrlw()
        {
            Run64bitTest("660F71D008");
            AssertCode(     // vpsrlw	xmm0,8h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v3 = xmm0",
                "2|L--|xmm0 = __psrlw(v3, 8<8>)");
        }

        [Test]
        public void X86Rw_sqrtss()
        {
            Run64bitTest("F30F51C8");
            AssertCode(     // sqrtss	xmm1,xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = __fsqrt(xmm0)",
                "2|L--|xmm1 = SEQ(SLICE(xmm1, word96, 32), v4)");
        }

        [Test]
        public void X86Rw_vpsllw()
        {
            Run64bitTest("660F71F508");
            AssertCode(     // vpsllw	xmm5,8h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v3 = xmm5",
                "2|L--|xmm5 = __psllw(v3, 8<8>)");
        }

        [Test]
        public void X86Rw_sqrtpd()
        {
            Run64bitTest("660F51E4");
            AssertCode(     // sqrtpd	xmm4,xmm4
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = xmm4",
                "2|L--|xmm4 = __sqrtpd(v3)");
        }

        [Test]
        public void X86Rw_cvtps2dq()
        {
            Run64bitTest("660F5BC0");
            AssertCode(     // cvtps2dq	xmm0,xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v3 = xmm0",
                "2|L--|xmm0 = __cvtps2dq(v3)");
        }

        [Test]
        public void X86Rw_roundss()
        {
            Run64bitTest("66 0F 3A 0A C3 03");
            AssertCode(     // roundss xmm0,xmm3,3h
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), truncf(SLICE(xmm3, real32, 0)))");
        }

        [Test]
        public void X86Rw_packsswb()
        {
            Run64bitTest("660F63C1");
            AssertCode(     // packsswb	xmm0,xmm1
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = xmm1",
                "3|L--|xmm0 = __packsswb(v4, v5)");
        }

        [Test]
        public void X86Rw_xsaveopt()
        {
            Run32bitTest("0FAE74D8BE");
            AssertCode(     // xsaveopt dword ptr [eax+ebx*8-42h]
                "0|L--|10000000(5): 1 instructions",
                "1|L--|__xsaveopt(&Mem0[eax - 0x42<32> + ebx * 8<32>:word32])");
        }

        [Test]
        public void X86rw_lea_ptr64_into_32_bitreg()
        {
            Run64bitTest("8D 4C 09 01");    // lea ecx,[rcx + rcx + 1]
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|ecx = SLICE(rcx + 1<64> + rcx, word32, 0)");
        }

        [Test]
        public void X86Rw_fdiv_mem()
        {
            Run32bitTest("D8 B6 3C 01 00 00");  // fdiv dword ptr [esi+0000013C]
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|ST[Top:real64] = ST[Top:real64] / CONVERT(Mem0[esi + 0x13C<32>:real32], real32, real64)");
        }


        [Test]
        public void X86Rw_vzeroupper()
        {
            Run64bitTest("C5F877");
            AssertCode(     // vzeroupper
                "0|L--|0000000140000000(3): 16 instructions",
                "1|L--|ymm0 = CONVERT(xmm0, word128, word256)",
                "2|L--|ymm1 = CONVERT(xmm1, word128, word256)",
                "3|L--|ymm2 = CONVERT(xmm2, word128, word256)",
                "4|L--|ymm3 = CONVERT(xmm3, word128, word256)",
                "5|L--|ymm4 = CONVERT(xmm4, word128, word256)",
                "6|L--|ymm5 = CONVERT(xmm5, word128, word256)",
                "7|L--|ymm6 = CONVERT(xmm6, word128, word256)",
                "8|L--|ymm7 = CONVERT(xmm7, word128, word256)",
                "9|L--|ymm8 = CONVERT(xmm8, word128, word256)",
                "10|L--|ymm9 = CONVERT(xmm9, word128, word256)",
                "11|L--|ymm10 = CONVERT(xmm10, word128, word256)",
                "12|L--|ymm11 = CONVERT(xmm11, word128, word256)",
                "13|L--|ymm12 = CONVERT(xmm12, word128, word256)",
                "14|L--|ymm13 = CONVERT(xmm13, word128, word256)",
                "15|L--|ymm14 = CONVERT(xmm14, word128, word256)",
                "16|L--|ymm15 = CONVERT(xmm15, word128, word256)");
        }

        /*
        [Test]
        public void X86Rw_fstsw_and_cmp_jz()
        {
            Run32bitTest(
                "DF E0" +       // fstsw	ax
                "80 E4 45" +    // and	ah,45
                "80 FC 40" +    // cmp	ah,40
                "74 02");       // jz	$+4
            AssertCode("@@@");
        }

        [Test]
        public void X86Rw_fstsw_and_cmp_jz()
        {
            Run32bitTest(
                "DF E0" +       // fstsw	ax
                "80 E4 45" +    // and	ah,45
                "80 FC 40" +    // cmp	ah,40
                "74 02");       // jz	$+4
            AssertCode("@@@");
        }
        [Test]
        public void X86Rw_fstsw_and_cmp_jz()
        {
            Run32bitTest(
                "DF E0" +       // fstsw	ax
                "80 E4 45" +    // and	ah,45
                "80 FC 40" +    // cmp	ah,40
                "74 02");       // jz	$+4
            AssertCode("@@@");
        }
        [Test]
        public void X86Rw_fstsw_and_cmp_jz()
        {
            Run32bitTest(
                "DF E0" +       // fstsw	ax
                "80 E4 45" +    // and	ah,45
                "80 FC 40" +    // cmp	ah,40
                "74 02");       // jz	$+4
            AssertCode("@@@");
        }
        */
    }
}