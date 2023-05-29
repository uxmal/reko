#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

        private void Run16bitTest(string hexBytes)
        {
            arch = arch16;
            Given_MemoryArea(new ByteMemoryArea(baseAddr16, BytePattern.FromHexBytes(hexBytes)));
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
        public void X86rw_mov_stackArgument()
        {
            Run16bitTest(m =>
            {
                m.Mov(m.ax, m.MemW(Registers.bp, -8));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|ax = Mem0[ss:bp - 8<i16>:word16]");
        }

        [Test]
        public void X86rw_add_reg_mem()
        {
            Run16bitTest(m =>
            {
                m.Add(m.ax, m.MemW(Registers.si, 4));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 2 instructions",
                "1|L--|ax = ax + Mem0[ds:si + 4<i16>:word16]",
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
                "1|L--|v4 = Mem0[ds:0x1000<16>:word16] + 3<16>",
                "2|L--|Mem0[ds:0x1000<16>:word16] = v4",
                "3|L--|SCZO = cond(v4)");
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
                "1|L--|xmm3 = __andnpd<word128>(xmm3, xmm1)");
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
            AssertCode(     // cmpnltss	xmm1,xmm0,5h
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
                "1|T--|goto Mem0[ds:bx + 16<i16>:word16]");
        }

        [Test]
        public void X86Rw_jmpf_indirect()
        {
            Run16bitTest("FF6F34");
            AssertCode(
                "0|T--|0C00:0000(3): 1 instructions",
                "1|T--|goto Mem0[ds:bx + 52<i16>:segptr32]");
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
        public void X86rw_bswap()
        {
            Run32bitTest(m =>
            {
                m.Bswap(m.ebx);
            });
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|ebx = __bswap<word32>(ebx)");
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
                "3|L--|__syscall<byte>(0x21<8>)");
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
                "1|L--|al = __in<byte>(dx)");
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
        public void X86rw_adc()
        {
            Run16bitTest(m =>
            {
                m.Adc(m.WordPtr(0x100), m.ax);
            });
            AssertCode(
                "0|L--|0C00:0000(4): 3 instructions",
                "1|L--|v6 = Mem0[ds:0x100<16>:word16] + ax + C",
                "2|L--|Mem0[ds:0x100<16>:word16] = v6",
                "3|L--|SCZO = cond(v6)");
        }

        [Test]
        public void X86rw_lea()
        {
            Run16bitTest(m =>
            {
                m.Lea(m.bx, m.MemW(Registers.bx, 4));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|bx = bx + 4<i16>");
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
            // Intel manual states that the C flag is set 
            // to 0 if the _source_ operand is 0.
            Run16bitTest(m =>
            {
                m.Neg(m.ecx);
            });
            AssertCode(
                "0|L--|0C00:0000(3): 3 instructions",
                "1|L--|C = ecx != 0<32>",
                "2|L--|ecx = -ecx",
                "3|L--|SZO = cond(ecx)");
        }

        [Test] 
        public void X86rw_Neg_mem()
        {
            Run32bitTest("F719");   // neg dword ptr [ecx]
            AssertCode(
                "0|L--|10000000(2): 5 instructions",
                "1|L--|v4 = Mem0[ecx:word32]",
                "2|L--|C = v4 != 0<32>",
                "3|L--|v6 = -v4",
                "4|L--|Mem0[ecx:word32] = v6",
                "5|L--|SZO = cond(v6)");
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
        public void X86rw_out()
        {
            Run16bitTest(m =>
            {
                m.Out(m.dx, m.al);
            });
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|__out<byte>(dx, al)");
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
        public void X86rw_shld()
        {
            Run16bitTest(m =>
            {
                m.Shld(m.edx, m.eax, m.cl);
            });
            AssertCode(
                "0|L--|0C00:0000(4): 1 instructions",
                "1|L--|edx = __shld<word32>(edx, eax, cl)");
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
        public void X86rw_shrd()
        {
            Run16bitTest(m =>
            {
                m.Shrd(m.eax, m.edx, 4);
            });
            AssertCode(
                "0|L--|0C00:0000(5): 1 instructions",
                "1|L--|eax = __shrd<word32>(eax, edx, 4<8>)");
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
                "2|L--|ST[Top:real64] = CONVERT(Mem0[ebx + 4<i32>:int32], int32, real64)");
        }

        [Test]
        public void X86Rw_frstpm()
        {
            Run16bitTest("DBE5");
            AssertCode(     // frstpm
                "0|L--|0C00:0000(2): 1 instructions",
                "1|L--|__frstpm()");
        }

        [Test]
        public void X86rw_fstp()
        {
            Run32bitTest(m =>
            {
                m.Fstp(m.MemDw(Registers.ebx, 4));
            });
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[ebx + 4<i32>:real32] = CONVERT(ST[Top:real64], real64, real32)",
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
        public void X86rw_les_bx_stack()
        {
            Run16bitTest(m =>
            {
                m.Les(m.bx, m.MemW(Registers.bp, 6));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|es_bx = Mem0[ss:bp + 6<i16>:segptr32]");
        }

        [Test]
        public void X86rw_fiadd()
        {
            Run16bitTest(m =>
            {
                m.Fiadd(m.WordPtr(m.bx, 0));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|ST[Top:real64] = ST[Top:real64] + CONVERT(Mem0[ds:bx:int16], int16, real64)");
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
                "1|L--|v6 = dx_ax",
                "2|L--|dx = CONVERT(v6 %u cx, word32, uint16)",
                "3|L--|ax = CONVERT(v6 /u cx, word16, uint16)",
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
                    "1|L--|v6 = dx_ax",
                    "2|L--|dx = CONVERT(v6 %s cx, word32, int16)",
                    "3|L--|ax = CONVERT(v6 /16 cx, word16, int16)",
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
                "2|L--|ecx = __bsr<word32>(eax)");
        }

        [Test]
        public void X86rw_IndirectCalls()
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
                "3|T--|call SEQ(0xC00<16>, Mem0[ds:bx + 4<i16>:word16]) (2)",
                "4|T--|0C00:0005(3): 1 instructions",
                "5|T--|call Mem0[ds:bx + 8<i16>:segptr32] (4)");
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
                "2|L--|ST[Top:real64] = CONVERT(Mem0[esp + 20<i32>:real32], real32, real64)",
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
                "2|L--|eax = Mem0[esp + 4<i32>:word32]",
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
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ax = ax >> 1<16>",
                "2|L--|SCZ = cond(ax)",
                "3|L--|O = false",
                "4|L--|0C00:0002(2): 2 instructions",
                "5|L--|bx = bx >> cl",
                "6|L--|SCZ = cond(bx)",
                "7|L--|0C00:0004(3): 2 instructions",
                "8|L--|dx = dx >> 4<16>",
                "9|L--|SCZ = cond(dx)");
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
                "1|H--|__halt()");
        }

        [Test]
        public void X86Rw_popcnt()
        {
            Run64bitTest("F3480FB8C7");
            AssertCode(     // popcnt	rax,rdi
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|rax = __popcnt<word64,int64>(rdi)",
                "2|L--|Z = rdi == 0<64>");
        }

        [Test]
        public void X86Rw_popcnt_mem()
        {
            Run64bitTest("F3480FB800");
            AssertCode(     // popcnt	rax,rdi
                "0|L--|0000000140000000(5): 3 instructions",
                "1|L--|v4 = Mem0[rax:word64]",
                "2|L--|rax = __popcnt<word64,int64>(v4)",
                "3|L--|Z = v4 == 0<64>");
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
                "1|L--|es_bx = Mem0[ds:bx:segptr32]");
        }

        [Test]
        public void X86rw_cmovz()
        {
            Run32bitTest("0F44C8");
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 10000003",
                "2|L--|ecx = eax");
        }

        [Test]
        public void X86rw_cmp_Ev_Ib()
        {
            Run32bitTest("833FFF");
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|SCZO = cond(Mem0[edi:word32] - 0xFFFFFFFF<32>)");
        }

        [Test]
        public void X86rw_rol_Eb()
        {
            Run32bitTest("C0C0C0");
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|v3 = (al & 1<8> << 8<8> - 0xC0<8>) != 0<8>", 
                "2|L--|al = __rol<byte,byte>(al, 0xC0<8>)",
                "3|L--|C = v3");
        }

        [Test]
        public void X86Rw_rorx()
        {
            Run64bitTest("C443FBF0D602");
            AssertCode(     // rorx	r10,r14,2h
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|r10 = __ror<word64,byte>(r14, 2<8>)");
        }

        [Test]
        public void X86rw_pause()
        {
            Run32bitTest("F390");
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__pause()");
        }

        [Test]
        public void X86rw_pxor_self()
        {
            Run32bitTest("0FEFC9");
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm1 = 0<64>");
        }

        [Test]
        public void X86rw_lock()
        {
            Run32bitTest("F0");
            AssertCode(
                  "0|L--|10000000(1): 1 instructions",
                  "1|L--|__lock()");
        }

        [Test]
        public void X86rw_cmpxchg()
        {
            Run32bitTest("0FB10A");
            AssertCode(
              "0|L--|10000000(3): 1 instructions",
              "1|L--|Z = __cmpxchg<word32>(Mem0[edx:word32], ecx, eax, out eax)");
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
            Run32bitTest("0fC1C2");
            AssertCode(
               "0|L--|10000000(3): 2 instructions",
               "1|L--|edx = __xadd<word32>(edx, eax)",
               "2|L--|SCZO = cond(edx)");
        }

        [Test]
        public void X86rw_cvttsd2si()
        {
            Run32bitTest("F20F2CC3");
            AssertCode(
              "0|L--|10000000(4): 1 instructions",
              "1|L--|eax = CONVERT(SLICE(xmm3, real64, 0), real64, int32)");
        }


        [Test]
        public void X86rw_fucompp()
        {
            Run32bitTest("DAE9");
            AssertCode(
              "0|L--|10000000(2): 2 instructions",
              "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 1<i8>:real64])",
              "2|L--|Top = Top + 2<i8>");
        }

        [Test]
        public void X86rw_fs_prefix()
        {
            Run32bitTest("648B0A");
            AssertCode(
           "0|L--|10000000(3): 1 instructions",
           "1|L--|ecx = Mem0[fs:edx:word32]");
        }

        [Test]
        public void X86rw_seto()
        {
            Run32bitTest("0f90c1");
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|cl = CONVERT(Test(OV,O), bool, int8)");
        }

        [Test]
        public void X86rw_cpuid()
        {
            Run32bitTest("0FA2");
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__cpuid(eax, ecx, &eax, &ebx, &ecx, &edx)");
        }

        [Test]
        public void X86rw_xgetbv()
        {
            Run32bitTest("0F01D0");
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|edx_eax = __xgetbv(ecx)");
        }

        [Test]
        public void X86rw_setc()
        {
            Run32bitTest("0F92C1");
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|cl = CONVERT(Test(ULT,C), bool, int8)");
        }

        [Test]
        public void X86rw_movd_xmm()
        {
            Run32bitTest("660f6ec0");
            AssertCode(  // movd
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = CONVERT(eax, word32, word128)");
        }

        [Test]
        public void X86rw_more_xmm()
        {
            Run32bitTest("660F7E01");
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = SLICE(xmm0, word32, 0)",
                "2|L--|Mem0[ecx:word32] = v5");
            Run32bitTest("660f60c0");
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklbw<word128>(xmm0, xmm0)");
            Run32bitTest("660f61c0");
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklwd<word128>(xmm0, xmm0)");
            Run32bitTest("660f70c000");
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|xmm0 = __pshuf<word32[4]>(xmm0, xmm0, 0<8>)");
        }

        [Test]
        public void X86rw_64_movsxd()
        {
            Run64bitTest("4863483c"); // "movsxd\trcx,dword ptr [rax+3C]", 
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|rcx = CONVERT(Mem0[rax + 60<i64>:word32], word32, int64)");
        }

        [Test]
        public void X86rw_64_movsxd_dword()
        {
            Run64bitTest("63483c"); // "movsxd\tecx,dword ptr [rax+3C]", 
            AssertCode(
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|ecx = Mem0[rax + 60<i64>:word32]");
        }

        [Test]
        public void X86rw_64_rip_relative()
        {
            Run64bitTest("498b0500001000"); // "mov\trax,qword ptr [rip+10000000]",
            AssertCode(
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|rax = Mem0[0x0000000140100007<p64>:word64]");
        }


        [Test]
        public void X86rw_64_sub_immediate_dword()
        {
            Run64bitTest("4881EC08050000"); // "sub\trsp,+00000508", 
            AssertCode(
                 "0|L--|0000000140000000(7): 2 instructions",
                 "1|L--|rsp = rsp - 0x508<64>",
                 "2|L--|SCZO = cond(rsp)");
        }

        [Test]
        public void X86rw_64_repne()
        {
            Run64bitTest("F348A5");   // "rep\tmovsd"
            AssertCode(
                 "0|L--|0000000140000000(3): 7 instructions",
                 "1|T--|if (rcx == 0<64>) branch 0000000140000003",
                 "2|L--|v4 = Mem0[rsi:word64]",
                 "3|L--|Mem0[rdi:word64] = v4",
                 "4|L--|rsi = rsi + 8<i64>", 
                 "5|L--|rdi = rdi + 8<i64>",
                 "6|L--|rcx = rcx - 1<64>",
                 "7|T--|goto 0000000140000000");
        }

        [Test]
        public void X86rw_PIC_idiom()
        {
            Run32bitTest("E80000000059");        // call $+5, pop ecx
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|ecx = 10000005");
        }

        [Test]
        public void X86rw_invalid_les()
        {
            Run32bitTest("C4C0");
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_push_cs_call_near()
        {
            Run16bitTest("0EE84232");
            AssertCode(
                "0|T--|0C00:0000(4): 1 instructions",
                "1|T--|call 0C00:3246 (4)");
        }

        [Test]
        public void X86rw_fstp_real32()
        {
            Run32bitTest("d91c24");
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[esp:real32] = CONVERT(ST[Top:real64], real64, real32)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_cmpxchg_byte()
        {
            Run32bitTest("F00FB023"); // lock cmpxchg[ebx], ah
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|__lock()",
                "2|L--|10000001(3): 1 instructions",
                "3|L--|Z = __cmpxchg<byte>(Mem0[ebx:byte], ah, al, out al)");
        }

        [Test]
        public void X86rw_fld_real32()
        {
            Run16bitTest("D94440"); // fld word ptr [foo]
            AssertCode(
                "0|L--|0C00:0000(3): 2 instructions",
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[ds:si + 64<i16>:real32], real32, real64)");
        }

        [Test]
        public void X86rw_movaps()
        {
            Run64bitTest("0F280540120000"); //  movaps xmm0,[rip+00001240]
            AssertCode(
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|xmm0 = Mem0[0x0000000140001247<p64>:word128]");
        }

        [Test]
        public void X86rw_idiv()
        {
            Run32bitTest("F77C2404");       // idiv [esp+04]
            AssertCode(
                  "0|L--|10000000(4): 4 instructions",
                  "1|L--|v6 = edx_eax",
                  "2|L--|edx = CONVERT(v6 %s Mem0[esp + 4<i32>:word32], word64, int32)",
                  "3|L--|eax = CONVERT(v6 /32 Mem0[esp + 4<i32>:word32], word32, int32)",
                  "4|L--|SCZO = cond(eax)");
        }

        [Test]
        public void X86rw_long_nop()
        {
            Run32bitTest("660f1f440000"); // nop WORD PTR[eax + eax*1 + 0x0]
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void X86rw_movlhps()
        {
            Run32bitTest("0F16F3");
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|xmm6[2<i32>] = xmm3[0<i32>]",
                "2|L--|xmm6[3<i32>] = xmm3[1<i32>]");
        }

        [Test]
        public void X86rw_fyl2x()
        {
            Run16bitTest("d9f1");
            AssertCode(
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ST[Top + 1<i8>:real64] = ST[Top + 1<i8>:real64] * lg2(ST[Top:real64])",
                "2|L--|FPUF = cond(ST[Top + 1<i8>:real64])",
                "3|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fyl2xp1()
        {
            Run16bitTest("D9F9");
            AssertCode(
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ST[Top + 1<i8>:real64] = ST[Top + 1<i8>:real64] * lg2(ST[Top:real64] + 1.0)",
                "2|L--|FPUF = cond(ST[Top + 1<i8>:real64])",
                "3|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fucomi()
        {
            Run32bitTest("DBEB");  // fucomi\tst(0),st(3)
            AssertCode(
               "0|L--|10000000(2): 3 instructions",
               "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 3<i8>:real64])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_fucomip()
        {
            Run32bitTest("DFE9");   // fucomip\tst(0),st(1)
            AssertCode(
               "0|L--|10000000(2): 4 instructions",
               "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 1<i8>:real64])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_movups()
        {
            Run64bitTest("0F1045E0");   // movups xmm0,XMMWORD PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = Mem0[rbp - 32<i64>:word128]");

            Run64bitTest("660F1045E0");   // movupd xmm0,XMMWORD PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = Mem0[rbp - 32<i64>:word128]");

            Run64bitTest("0F11442420"); // movups\t[rsp+20],xmm0, 
            AssertCode(
                  "0|L--|0000000140000000(5): 1 instructions",
                  "1|L--|Mem0[rsp + 32<i64>:word128] = xmm0");
        }

        [Test]
        public void X86rw_movss()
        {
            Run64bitTest("F30F1045E0");   // movss xmm0,dword PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = SEQ(0<96>, Mem0[rbp - 32<i64>:real32])");
            Run64bitTest("F30F1145E0");   // movss dword PTR[rbp - 0x20], xmm0,
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|Mem0[rbp - 32<i64>:real32] = SLICE(xmm0, real32, 0)");
            Run64bitTest("F30F10C3");         // movss xmm0, xmm3,
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), SLICE(xmm3, real32, 0))");
        }

        [Test]
        public void X86Rw_movzx()
        {
            Run32bitTest("0F B7 C0");   // movzx eax, ax
            AssertCode(
               "0|L--|10000000(3): 1 instructions",
               "1|L--|eax = CONVERT(ax, word16, word32)");
        }

        [Test(Description = "Regression reported by @mewmew")]
        public void X86rw_regression1()
        {
            Run32bitTest("DB7C4783");       // fstp [esi-0x7D + eax*2]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|Mem0[edi - 125<i32> + eax * 2<32>:real80] = CONVERT(ST[Top:real64], real64, real80)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fdivr()
        {
            Run32bitTest("DC3D78563412"); // fdivr [12345678]
            AssertCode(
                "0|L--|10000000(6): 1 instructions",
                "1|L--|ST[Top:real64] = Mem0[0x12345678<p32>:real64] / ST[Top:real64]");
        }

        [Test]
        public void X86rw_movsd()
        {
            Run64bitTest("F20F1045E0");   // movsd xmm0,dword PTR[rbp - 0x20]
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|xmm0 = SEQ(0<64>, Mem0[rbp - 32<i64>:real64])");
            Run64bitTest("F20F1145E0");   // movsd dword PTR[rbp - 0x20], xmm0,
            AssertCode(
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|Mem0[rbp - 32<i64>:real64] = SLICE(xmm0, real64, 0)");
            Run64bitTest("F20F10C3");   // movsd xmm0, xmm3,
            AssertCode(
               "0|L--|0000000140000000(4): 1 instructions",
               "1|L--|xmm0 = SEQ(SLICE(xmm0, word64, 64), SLICE(xmm3, real64, 0))");
        }

        [Test(Description = "Intel and AMD state that if you set the low 32-bits of a register in 64-bit mode, they are zero extended.")]
        public void X86rw_64bit_clearHighBits()
        {
            Run64bitTest("33C0");
            AssertCode(
               "0|L--|0000000140000000(2): 4 instructions",
               "1|L--|eax = eax ^ eax",
               "2|L--|rax = CONVERT(eax, word32, uint64)",
               "3|L--|SZO = cond(eax)",
               "4|L--|C = false");
        }

        [Test]
        public void X86Rw_cldemote()
        {
            Run64bitTest("0F1C78CB");
            AssertCode(     // cldemote	byte ptr [rax-35h]
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|__cache_line_demote(&Mem0[rax - 53<i64>:byte])");
        }

        [Test]
        public void X86Rw_tzcnt()
        {
            Run64bitTest("F30FBCCA");
            AssertCode(     // tzcnt	ecx,edx
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|ecx = __tzcnt<word32>(edx)",
                "2|L--|Z = ecx == 0<32>");
        }

        [Test]
        public void X86rw_ucomiss()
        {
            Run64bitTest("0F2E052DB10000");
            AssertCode( // ucomiss\txmm0,dword ptr [rip+0000B12D]
               "0|L--|0000000140000000(7): 3 instructions",
               "1|L--|CZP = cond(SLICE(xmm0, real32, 0) - Mem0[0x000000014000B134<p64>:real32])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_ucomisd()
        {
            Run64bitTest("660F2E052DB10000");
            AssertCode( // ucomisd\txmm0,qword ptr [rip+0000B12D]
               "0|L--|0000000140000000(8): 3 instructions",
               "1|L--|CZP = cond(SLICE(xmm0, real64, 0) - Mem0[0x000000014000B135<p64>:real64])",
               "2|L--|O = false",
               "3|L--|S = false");
        }

        [Test]
        public void X86rw_addss()
        {
            Run64bitTest("F30F580DFBB00000");
            AssertCode( //addss\txmm1,dword ptr [rip+0000B0FB]
               "0|L--|0000000140000000(8): 3 instructions",
               "1|L--|v4 = SLICE(xmm1, real32, 0) + Mem0[0x000000014000B103<p64>:real32]",
               "2|L--|v5 = SLICE(xmm1, word96, 32)",
               "3|L--|xmm1 = SEQ(v5, v4)");
        }

        [Test]
        public void X86rw_subss()
        {
            Run64bitTest("F30F5CCD");
            AssertCode(     // subss\txmm1,dword ptr [rip+0000B0FB]
               "0|L--|0000000140000000(4): 3 instructions",
               "1|L--|v4 = SLICE(xmm1, real32, 0) - SLICE(xmm5, real32, 0)",
               "2|L--|v6 = SLICE(xmm1, word96, 32)",
               "3|L--|xmm1 = SEQ(v6, v4)");
        }

        [Test]
        public void X86rw_vsubss()
        {
            Run64bitTest("C5BE5CCD");
            AssertCode(     // vsubss xmm1,xmm8,xmm5
               "0|L--|0000000140000000(4): 2 instructions",
               "1|L--|v4 = SLICE(xmm8, real32, 0) - SLICE(xmm5, real32, 0)",
               "2|L--|xmm1 = SEQ(0<96>, v4)");
        }

        [Test]
        public void X86rw_cvtsi2ss()
        {
            Run64bitTest("F3480F2AC0");
            AssertCode(     // "cvtsi2ss\txmm0,rax", 
               "0|L--|0000000140000000(5): 2 instructions",
               "1|L--|v5 = CONVERT(rax, int64, real32)",
               "2|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), v5)");
        }

        [Test]
        public void X86rw_cvtpi2ps()
        {
            Run64bitTest("49 0F 2A C5");
            AssertCode( // cvtpi2ps xmm0, mm5
               "0|L--|0000000140000000(4): 3 instructions",
               "1|L--|v4 = CONVERT(SLICE(mm5, int32, 0), int32, real32)",
               "2|L--|v5 = CONVERT(SLICE(mm5, int32, 32), int32, real32)",
               "3|L--|xmm0 = SEQ(v5, v4)");
        }

        [Test]
        public void X86rw_addps()
        {
            Run64bitTest("0F 58 0D FB B0 00 00");
            AssertCode( // addps xmm1,[0000000000415EF4]
                "0|L--|0000000140000000(7): 3 instructions",
                "1|L--|v4 = xmm1",
                "2|L--|v5 = Mem0[0x000000014000B102<p64>:word128]",
                "3|L--|xmm1 = __simd_fadd<real32[4]>(v4, v5)");
        }

        [Test]
        public void X86Rw_divpd()
        {
            Run64bitTest("660F5EF1");
            AssertCode(     // divpd	xmm6,xmm1
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v5 = xmm6",
                "2|L--|v6 = xmm1",
                "3|L--|xmm6 = __divp<real64[2]>(v5, v6)");
        }

        [Test]
        public void X86rw_divsd()
        {
            Run64bitTest("F2 0F 5E C1");
            AssertCode( // divsd xmm0,xmm1
               "0|L--|0000000140000000(4): 3 instructions",
               "1|L--|v4 = SLICE(xmm0, real64, 0) / SLICE(xmm1, real64, 0)",
               "2|L--|v6 = SLICE(xmm0, word64, 64)",
               "3|L--|xmm0 = SEQ(v6, v4)");
        }

        [Test]
        public void X86rw_mulsd()
        {
            Run64bitTest("F2 0F 59 05 92 AD 00 00 ");
            AssertCode(     // mulsd xmm0,qword ptr[rip + ad92]
               "0|L--|0000000140000000(8): 3 instructions",
               "1|L--|v4 = SLICE(xmm0, real64, 0) * Mem0[0x000000014000AD9A<p64>:real64]",
               "2|L--|v5 = SLICE(xmm0, word64, 64)",
               "3|L--|xmm0 = SEQ(v5, v4)");
        }

        [Test]
        public void X86rw_subps()
        {
            Run64bitTest("0F 5C 05 61 AA 00 00");
            AssertCode( // subps xmm0,[0000000000415F0C]
               "0|L--|0000000140000000(7): 3 instructions",
               "1|L--|v4 = xmm0",
               "2|L--|v5 = Mem0[0x000000014000AA68<p64>:word128]",
               "3|L--|xmm0 = __simd_fsub<real32[4]>(v4, v5)");
        }

        [Test]
        public void X86rw_cvttss2si()
        {
            Run64bitTest("F3 4C 0F 2C F8");
            AssertCode(     // cvttss2si r15, xmm0
               "0|L--|0000000140000000(5): 1 instructions",
               "1|L--|r15 = CONVERT(SLICE(xmm0, real32, 0), real32, int64)");
        }

        [Test]
        public void X86rw_mulps()
        {
            Run64bitTest("0F 59 4A 08");
            AssertCode(     // mulps xmm1,[rdx+08]
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v5 = xmm1",
                "2|L--|v6 = Mem0[rdx + 8<i64>:word128]",
                "3|L--|xmm1 = __mulp<real32[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_mulss()
        {
            Run64bitTest("F3 0F 59 D8");
            AssertCode(     // mulss xmm3, xmm0
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v4 = SLICE(xmm3, real32, 0) * SLICE(xmm0, real32, 0)",
                "2|L--|v6 = SLICE(xmm3, word96, 32)",
                "3|L--|xmm3 = SEQ(v6, v4)");
        }

        [Test]
        public void X86rw_vmulss()
        {
            Run64bitTest("C5 BE 59 D8");
            AssertCode(     // mulss xmm3, xmm8, xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = SLICE(xmm8, real32, 0) * SLICE(xmm0, real32, 0)",
                "2|L--|xmm3 = SEQ(0<96>, v4)");
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
                "1|L--|v4 = SLICE(xmm0, real32, 0) / SLICE(xmm1, real32, 0)",
                "2|L--|v6 = SLICE(xmm0, word96, 32)",
                "3|L--|xmm0 = SEQ(v6, v4)");
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
            Run16bitTest("D9F2");
            AssertCode(     // fptan
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ST[Top:real64] = tan(ST[Top:real64])",
                "2|L--|Top = Top - 1<i8>",
                "3|L--|ST[Top:real64] = 1.0");
        }

        [Test]
        public void X86rw_f2xm1()
        {
            Run16bitTest("D9F0");
            AssertCode(     // f2xm1
                "0|L--|0C00:0000(2): 1 instructions",
                "1|L--|ST[Top:real64] = pow(2.0, ST[Top:real64]) - 1.0");
        }

        [Test]
        public void X86rw_fpu_load()
        {
            Run64bitTest("D80D899F0000");
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
            Run32bitTest("DBE3");
            AssertCode(     // fninit
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__fninit()");
        }

        /// <summary>
        /// This appears to be an obsolete 286 whose net effect is negligible.
        /// </summary>
        [Test]
        public void X86Rw_fnsetpm()
        {
            Run32bitTest("DBE4");	// fnsetpm
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void X86rw_x64_push_immediate_32bit()
        {
            Run64bitTest("6AC2");
            AssertCode(     // "push 0xC2", 
                "0|L--|0000000140000000(2): 2 instructions",
                "1|L--|rsp = rsp - 4<i64>",
                "2|L--|Mem0[rsp:word32] = 0xFFFFFFC2<32>");
        }

        [Test]
        public void X86rw_x64_push_immediate_64bit()
        {
            Run64bitTest("486AC2");
            AssertCode(     // "push 0xC2", 
                "0|L--|0000000140000000(3): 2 instructions",
                "1|L--|rsp = rsp - 8<i64>",
                "2|L--|Mem0[rsp:word64] = 0xFFFFFFFFFFFFFFC2<64>");
        }

        [Test]
        public void X86rw_x64_push_register()
        {
            Run64bitTest("53");
            AssertCode(     // "push rbx", 
                "0|L--|0000000140000000(1): 2 instructions",
                "1|L--|rsp = rsp - 8<i64>",
                "2|L--|Mem0[rsp:word64] = rbx");
        }

        [Test]
        public void X86rw_x64_push_memoryload()
        {
            Run64bitTest("FF75E0");
            AssertCode(     // "push rbx", 
                "0|L--|0000000140000000(3): 3 instructions",
                "1|L--|v5 = Mem0[rbp - 32<i64>:word64]",
                "2|L--|rsp = rsp - 8<i64>",
                "3|L--|Mem0[rsp:word64] = v5");
        }

        [Test]
        public void X86rw_push_segreg()
        {
            Run32bitTest("06");
            AssertCode(     // "push es", 
                "0|L--|10000000(1): 2 instructions",
                "1|L--|esp = esp - 2<i32>",
                "2|L--|Mem0[esp:word16] = es");
        }

        [Test]
        public void X86rw_mfence()
        {
            Run32bitTest("0FAEF0");   // mfence
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|__mfence()");
        }

        [Test]
        public void X86rw_lfence()
        {
            Run32bitTest("0FAEE8"); // lfence
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|__lfence()");
        }

        [Test]
        public void X86rw_prefetch()
        {
            Run64bitTest("410F1808"); // prefetch
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|__prefetcht0<ptr64>(Mem0[r8:byte])");
        }

        [Test]
        public void X86rw_xorps()
        {
            Run64bitTest("0F57C3"); // xorps\txmm0,xmm3
            AssertCode(
                "0|L--|0000000140000000(3): 3 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|v6 = xmm3",
                "3|L--|xmm0 = __xorp<word32[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_xorps_same_register()
        {
            Run64bitTest("0F57C0"); // xorps\txmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|xmm0 = 0<128>");
        }

        [Test]
        public void X86rw_aesimc()
        {
            Run32bitTest("660F38DBC0"); // aesimc\txmm0,xmm0
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|xmm0 = __aesimc(xmm0)");
        }

        [Test]
        public void X86Rw_vmovd()
        {
            Run64bitTest("C4C1F96ECE");
            AssertCode(     // vmovd	xmm1,r14
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm1 = CONVERT(r14, word64, word128)");
        }

        [Test]
        public void X86rw_vmovd_slicing()
        {
            Run64bitTest("C4C1F97EC0");
            AssertCode(     // vmovd	r8,xmm0
                  "0|L--|0000000140000000(5): 1 instructions",
                  "1|L--|r8 = SLICE(xmm0, word64, 0)");
        }

        [Test]
        public void X86rw_vmovss_load()
        {
            Run64bitTest("C5FA100551030000"); // vmovss txmm0,dword ptr [rip+00000351]
            AssertCode(
                "0|L--|0000000140000000(8): 1 instructions",
                "1|L--|xmm0 = SEQ(0<96>, Mem0[0x0000000140000359<p64>:real32])");
        }

        [Test]
        public void X86rw_vmovss_store()
        {
            Run64bitTest("C5FA11852CFFFFFF"); // vmovss dword ptr [rbp-0xd4], xmm0
            AssertCode(
                "0|L--|0000000140000000(8): 1 instructions",
                "1|L--|Mem0[rbp - 212<i64>:real32] = SLICE(xmm0, real32, 0)");
        }

        [Test]
        public void X86Rw_vmovups()
        {
            Run64bitTest("C4C17C10049B");
            AssertCode(     // vmovups	ymm0,[r11+rbx*4]
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|ymm0 = Mem0[r11 + rbx * 4<64>:word256]");
        }

        [Test]
        public void X86rw_vcvtsi2ss()
        {
            Run64bitTest("C4E1FA2AC0");     // vcvtsi2ss\txmm0,xmm0,rax
            AssertCode(
             "0|L--|0000000140000000(5): 2 instructions",
             "1|L--|v5 = CONVERT(rax, int64, real32)",
             "2|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), v5)");
        }

        [Test]
        public void X86rw_vcvtsi2sd()
        {
            Run64bitTest("C4E1FB2AC2"); // vcvtsi2sd\txmm0,xmm0,rdx
            AssertCode(
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v5 = CONVERT(rdx, int64, real64)",
                "2|L--|xmm0 = SEQ(SLICE(xmm0, word64, 64), v5)");
        }

        [Test]
        public void X86rw_vmovsd()
        {
            Run64bitTest("C5FB1101"); // vmovsd double ptr[rcx], xmm0
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
                "1|L--|v6 = xmm2",
                "2|L--|v7 = xmm11",
                "3|L--|xmm15 = __minp<real64[2]>(v6, v7)");
        }

        [Test]
        public void X86Rw_vpbroadcastb()
        {
            Run64bitTest("C4E27D78C0");
            AssertCode(     // vpbroadcastb	ymm0,al
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|ymm0 = __pbroadcast<byte,byte[32]>(SLICE(ymm0, byte, 0))");
        }

        [Test]
        public void X86Rw_vpcmpeqb()
        {
            Run64bitTest("C5ED74D8");
            AssertCode(     // vpcmpeqb	ymm3,ymm2,ymm0
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v6 = ymm2",
                "2|L--|v7 = ymm0",
                "3|L--|ymm3 = __pcmpeq<byte[32]>(v6, v7)");
        }

        [Test]
        public void X86Rw_pmaddubsw()
        {
            Run64bitTest("660F3804D6");
            AssertCode(     // pmaddubsw	xmm2,xmm6
                "0|L--|0000000140000000(5): 3 instructions",
                "1|L--|v5 = xmm2",
                "2|L--|v6 = xmm6",
                "3|L--|xmm2 = __pmaddubsw<uint8[16],int8[16],int16[8]>(v5, v6)");
        }

        [Test]
        public void X86Rw_vpmaxub()
        {
            Run64bitTest("C5C9DEC9");
            AssertCode(     // vpmaxub	xmm1,xmm6,xmm1
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v5 = xmm6",
                "2|L--|v6 = xmm1",
                "3|L--|xmm1 = __pmaxu<uint8[16]>(v5, v6)");
        }

        [Test]
        public void X86Rw_vpmovmskb()
        {
            Run64bitTest("C5FDD7CA");
            AssertCode(     // vpmovmskb	ecx,ymm2
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v5 = __pmovmskb<word256>(ymm2)",
                "2|L--|ecx = SEQ(SLICE(ecx, word24, 8), v5)");
        }

        [Test]
        public void X86Rw_vpsubsw()
        {
            Run64bitTest("C501E92F");
            AssertCode(     // vpsubsw	xmm13,xmm15,[rdi]
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v6 = xmm15",
                "2|L--|v7 = Mem0[rdi:word128]",
                "3|L--|xmm13 = __psubs<int16[8]>(v6, v7)");
        }

        [Test]
        public void X86rw_vmovaps_xmm()
        {
            Run64bitTest("C5F92800"); // vmovaps xmm0,[rax]
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = Mem0[rax:word128]");
        }

        [Test]
        public void X86rw_vmovaps_ymm()
        {
            Run64bitTest("C5FD2800"); // vmovaps ymm0,[rax]
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|ymm0 = Mem0[rax:word256]");
        }

        [Test]
        public void X86Rw_vaddpd()
        {
            Run64bitTest("C5FD588570FFFFFF");
            AssertCode(     // vaddpd   ymm0,ymm0,[rbp-90h]
                "0|L--|0000000140000000(8): 3 instructions",
                "1|L--|v5 = ymm0",
                "2|L--|v6 = Mem0[rbp - 144<i64>:word256]",
                "3|L--|ymm0 = __simd_fadd<real64[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_vaddsd()
        {
            Run64bitTest("C5FB58C0");   // vaddsd xmm0,xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = SLICE(xmm0, real64, 0) + SLICE(xmm0, real64, 0)",
                "2|L--|xmm0 = SEQ(0<64>, v4)");
        }

        [Test]
        public void X86rw_vxorpd()
        {
            Run64bitTest("C5F957C0");   // vxorpd xmm0,xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = 0<128>");
        }

        [Test]
        public void X86rw_vxorpd_mem_256()
        {
            Run64bitTest("C5FD5709");   // vxorpd\tymm1,ymm0,[rcx]
            AssertCode(
             "0|L--|0000000140000000(4): 3 instructions",
             "1|L--|v6 = ymm0",
             "2|L--|v7 = Mem0[rcx:word256]",
             "3|L--|ymm1 = __xorp<word64[4]>(v6, v7)");
        }

        // new instructions

        [Test]
        public void X86rw_andnps()
        {
            Run32bitTest("0F554242");    // andnps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __andnps<word128>(xmm0, Mem0[edx + 66<i32>:word128])");
    }

        [Test]
        public void X86rw_andps()
        {
            Run32bitTest("0F544242");    // andps\txmm0,[edx+42]
            AssertCode(
               "0|L--|10000000(4): 3 instructions",
               "1|L--|v5 = xmm0",
               "2|L--|v6 = Mem0[edx + 66<i32>:word128]",
               "3|L--|xmm0 = __andp<word32[4]>(v5, v6)");
        }

        [Test]
        public void X86Rw_bextr()
        {
            Run64bitTest("C4C228F7F9");
            AssertCode(     // bextr	edi,r9d,r10d
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|edi = __bextr<word32>(r9d, r10d)",
                "2|L--|Z = edi == 0<32>");
        }

        [Test]
        public void X86Rw_blsi()
        {
            Run64bitTest("C4C298F3DB");
            AssertCode(     // blsi	r12,r11
                "0|L--|0000000140000000(5): 4 instructions",
                "1|L--|r12 = __blsi<word64>(r11)",
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
                "1|L--|r13 = __blsmsk<word64>(r10)",
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
                "1|L--|r10 = __blsr<word64>(rdx)",
                "2|L--|Z = r10 == 0<64>",
                "3|L--|S = r10 <= 0<64>",
                "4|L--|C = rdx == 0<64>");
        }

        [Test]
        public void X86rw_bsf()
        {
            Run32bitTest("0FBC4242");    // bsf\teax,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|Z = Mem0[edx + 66<i32>:word32] == 0<32>",
                "2|L--|eax = __bsf<word32>(Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_btc()
        {
            Run32bitTest("0FBB4242");    // btc\teax,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|C = __btc<word32>(Mem0[edx + 66<i32>:word32], eax, out Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_btr()
        {
            Run32bitTest("0FBAF300");
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|C = __btr<word32>(ebx, 0<8>, out ebx)");
        }


        [Test]
        public void X86rw_bts()
        {
            Run32bitTest("0FAB0424");
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|C = __bts<word32>(Mem0[esp:word32], eax, out Mem0[esp:word32])");
        }

        [Test]
        public void X86Rw_bzhi()
        {
            Run64bitTest("C46290F5EB");
            AssertCode(     // bzhi	r13,rbx,r13
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|r13 = __bzhi<word64>(rbx, r13)");
        }

        [Test]
        public void X86rw_clts()
        {
            Run32bitTest("0F06");    // clts
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|cr0 = __clts<word32>(cr0)");
        }

        [Test]
        public void X86rw_cmpltps()
        {
            Run32bitTest("0FC2424201");    // cmpltps\txmm0,[edx+42],01
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word128]",
                "3|L--|xmm0 = __cmpltp<real32[4],word32[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_comiss()
        {
            Run32bitTest("0F2F4242");    // comiss\txmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|CZP = cond(SLICE(xmm0, real32, 0) - Mem0[edx + 66<i32>:real32])",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86rw_comiss_reg()
        {
            Run32bitTest("0F2FCF");
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|CZP = cond(SLICE(xmm1, real32, 0) - SLICE(xmm7, real32, 0))",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86rw_cvtdq2ps()
        {
            Run32bitTest("0F5B4242");    // cvtdq2ps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 66<i32>:word128]",
                "2|L--|xmm0 = __cvtdq2ps<int64[2],real32[2]>(v4)");
        }

        [Test]
        public void X86rw_cvtps2pd()
        {
            Run32bitTest("0F5A4242");    // cvtps2pd\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 66<i32>:word128]",
                "2|L--|xmm0 = __cvtps2pd<real32[4],real64[4]>(v4)");
        }

        [Test]
        public void X86rw_cvtps2pi()
        {
            Run32bitTest("0F2D4242");    // cvtps2pi\tmm0,xmmword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v4 = Mem0[edx + 66<i32>:word128]",
                "2|L--|mm0 = __cvtps2pi<real32[4],int32[4]>(v4)");
        }

        [Test]
        public void X86rw_emms()
        {
            Run32bitTest("0F77");    // emms
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__emms()");
        }

        [Test]
        public void X86rw_fclex()
        {
            Run32bitTest("DBE2");    // fclex
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__fclex()");
        }

        [Test]
        public void X86rw_fcmovb()
        {
            Run32bitTest("DAC1");    // fcmovb\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(GE,C)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovbe()
        {
            Run32bitTest("DAD1");    // fcmovbe\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(GT,CZ)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmove()
        {
            Run32bitTest("DAC9");    // fcmove\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovnbe()
        {
            Run32bitTest("DBD1");    // fcmovnbe\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(LE,CZ)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovne()
        {
            Run32bitTest("DBC9");    // fcmovne\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovnu()
        {
            Run32bitTest("DBD9");    // fcmovnu\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(IS_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcmovu()
        {
            Run32bitTest("DAD9");    // fcmovu\tst(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NOT_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fcomip()
        {
            Run32bitTest("DFF2");    // fcomip\tst(0),st(2)
            AssertCode(
                "0|L--|10000000(2): 4 instructions",
                "1|L--|CZP = cond(ST[Top:real64] - ST[Top + 2<i8>:real64])",
                "2|L--|O = false",
                "3|L--|S = false");
        }

        [Test]
        public void X86rw_ffree()
        {
            Run32bitTest("DDC2");    // ffree\tst(2)
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__ffree(ST[Top + 2<i8>:real64])");
        }

        [Test]
        public void X86rw_fild_i16()
        {
            Run32bitTest("DF4042");    // fild\tword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[eax + 66<i32>:int16], int16, real64)");
        }

        [Test]
        public void X86rw_fisttp()
        {
            Run32bitTest("DB08");    // fisttp\tdword ptr [eax]
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|Mem0[eax:int32] = CONVERT(trunc(ST[Top:real64]), real64, int32)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fisttp_int16()
        {
            Run32bitTest("DF4842");    // fisttp\tword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[eax + 66<i32>:int16] = CONVERT(trunc(ST[Top:real64]), real64, int16)",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fisttp_int64()
        {
            Run32bitTest("DD4842");    // fisttp\tqword ptr [eax+42]
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|Mem0[eax + 66<i32>:int64] = CONVERT(trunc(ST[Top:real64]), real64, int64)");
        }

        [Test]
        public void X86rw_fld_real80()
        {
            Run32bitTest("DB28");    // fld\ttword ptr [eax]
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[eax:real80], real80, real64)");
        }

        [Test]
        public void X86rw_fucom()
        {
            Run32bitTest("DDE5");    // fucom\tst(5),st(0)
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
            Run32bitTest("DDEA");    // fucomp\tst(2)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|FPUF = cond(ST[Top:real64] - ST[Top + 2<i8>:real64])");
        }

        [Test]
        public void X86rw_invd()
        {
            Run32bitTest("0F08");    // invd
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__invd()");
        }

        [Test]
        public void X86rw_lar()
        {
            Run32bitTest("0F024242");    // lar\teax,word ptr [edx+42]
            AssertCode(
                "0|S--|10000000(4): 2 instructions",
                "1|L--|eax = __lar<word32>(&Mem0[edx + 66<i32>:word16])",
                "2|L--|Z = true");
        }

        [Test]
        public void X86rw_lsl()
        {
            Run32bitTest("0F034242");    // lsl\teax,word ptr [edx+42]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|eax = __load_segment_limit<word32>(Mem0[edx + 66<i32>:word16])");
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
                "1|L--|xmm4 = __maskmovdqu<word128>(xmm4, xmm2)");
        }

        [Test]
        public void X86rw_maskmovq()
        {
            Run32bitTest("0FF74242");    // maskmovq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __maskmovq<word64>(mm0, Mem0[edx + 66<i32>:word64])");
        }

        [Test]
        public void X86rw_minps()
        {
            Run32bitTest("0F5D4242");    // minps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word128]",
                "3|L--|xmm0 = __minp<real32[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_syscall()
        {
            Run64bitTest("0F05");    // syscall
            AssertCode(
                "0|T--|0000000140000000(2): 1 instructions",
                "1|L--|__syscall()");
            Run32bitTest("0F05");    // illegal
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_sysenter()
        {
            Run32bitTest("0F34");    // sysenter
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__sysenter()");
        }

        [Test]
        public void X86rw_sysexit()
        {
            Run32bitTest("0F35");    // sysexit
            AssertCode(
                "0|R--|10000000(2): 2 instructions",
                "1|L--|__sysexit()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void X86rw_sysret()
        {
            Run64bitTest("0F07");    // sysret
            AssertCode(
                "0|R--|0000000140000000(2): 2 instructions",
                "1|L--|__sysret()",
                "2|R--|return (0,0)");
            Run32bitTest("0F07");    // illegal
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_ud2()
        {
            Run32bitTest("0F0B");    // ud2
            AssertCode(
                "0|---|10000000(2): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86rw_unpcklps()
        {
            Run32bitTest("0F144242");    // unpcklps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word128]",
                "3|L--|xmm0 = __unpcklp<real32[4]>(v5, v6)");
            Run32bitTest("660F144242");    // unpcklpd\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word128]",
                "3|L--|xmm0 = __unpcklp<real64[2]>(v5, v6)");
        }


        [Test]
        public void X86rw_wbinvd()
        {
            Run32bitTest("0F09");    // wbinvd
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__wbinvd()");
        }

        [Test]
        public void X86rw_prefetchw()
        {
            Run32bitTest("0F0D4242");    // prefetchw\tdword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|__prefetchw<ptr32>(Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_mov_from_control_Reg()
        {
            Run32bitTest("0F2042");    // mov\tedx,cr0
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|edx = cr0");
        }

        [Test]
        public void X86rw_mov_debug_reg()
        {
            Run32bitTest("0F2142");    // mov\tedx,dr0
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|edx = dr0");
        }

        [Test]
        public void X86rw_mov_sib_eiz()
        {
            Run32bitTest("8B4C61F8");   // mov\tecx,[ecx+eiz*2-8h]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|ecx = Mem0[ecx - 8<i32>:word32]");
        }

        [Test]
        public void X86rw_mov_control_reg()
        {
            Run32bitTest("0F2242");    // mov\tcr0,edx
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|cr0 = edx");
        }

        [Test]
        public void X86rw_mov_to_debug_reg()
        {
            Run32bitTest("0F2342");    // mov\tdr0,edx
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|dr0 = edx");
        }

        [Test]
        public void X86rw_movhpd()
        {
            Run32bitTest("0F174242");    // movhps\tqword ptr [edx+42],xmm0
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|Mem0[edx + 66<i32>:word64] = __movhp<real32[4],real64[1]>(v5)");
            Run32bitTest("660F174242");    // movhpd\tqword ptr [edx+42],xmm0
            AssertCode(
                "0|L--|10000000(5): 2 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|Mem0[edx + 66<i32>:word64] = __movhp<real64[2],real64[1]>(v5)");
        }

        [Test]
        public void X86rw_movlps()
        {
            Run32bitTest("0F124242");    // movlps\txmm0,qword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = Mem0[edx + 66<i32>:word64]",
                "2|L--|xmm0 = __movlp<real32[2],real64[2]>(v5)");
        }

        [Test]
        public void X86rw_movmskps()
        {
            Run32bitTest("0F5042");    // movmskps\teax,xmm2
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v5 = __movmskps<word128>(xmm2)",
                "2|L--|eax = SEQ(SLICE(eax, word24, 8), v5)");
        }

        [Test]
        public void X86rw_movnti()
        {
            //$TODO: should use intrisic here.
            Run32bitTest("0FC34242");    // movnti\t[edx+42],eax
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 66<i32>:word32] = eax");
        }

        [Test]
        public void X86rw_movntps()
        {
            Run32bitTest("0F2B4242");    // movntps\t[edx+42],xmm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 66<i32>:word128] = xmm0");
        }


        [Test]
        public void X86rw_movntq()
        {
            Run32bitTest("0FE74242");    // movntq\t[edx+42],mm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 66<i32>:word64] = mm0");
        }


        [Test]
        public void X86rw_orps()
        {
            Run32bitTest("0F564242");    // orps\txmm0,[edx+42]
            AssertCode(
               "0|L--|10000000(4): 3 instructions",
               "1|L--|v5 = xmm0",
               "2|L--|v6 = Mem0[edx + 66<i32>:word128]",
               "3|L--|xmm0 = __orp<real32[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_packssdw()
        {
            Run32bitTest("0F6B4242");    // packssdw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word32]",
                "3|L--|mm0 = __packss<int32[2],int16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_packuswb()
        {
            Run32bitTest("0F674242");    // packuswb\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word32]",
                "3|L--|mm0 = __packus<uint16[4],uint8[8]>(v5, v6)");
        }

        [Test]
        public void X86rw_paddb()
        {
            Run32bitTest("0FFC4242");    // paddb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __simd_add<byte[8]>(v5, v6)");
        }

        [Test]
        public void X86rw_paddd()
        {
            Run32bitTest("0FFE4242");    // paddd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __simd_add<word32[2]>(v5, v6)");
        }

        [Test]
        public void X86rw_paddsw()
        {
            Run32bitTest("0FED4242");    // paddsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __padds<int16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_paddusb()
        {
            Run32bitTest("0FDC4242");    // paddusb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __paddus<byte[8]>(v5, v6)");
        }

        [Test]
        public void X86rw_paddw()
        {
            Run32bitTest("0FFD4242");    // paddw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __simd_add<word16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_getsec()
        {
            Run32bitTest("0F37");    // getsec
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
                "1|L--|rdx = __pext<word64>(r15, r14)");
        }

        [Test]
        public void X86Rw_pdep()
        {
            Run64bitTest("C4E2E3F5F2");
            AssertCode(     // pdep	rsi,rbx,rdx
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|rsi = __pdep<word64>(rbx, rdx)");
        }

        [Test]
        public void X86rw_pextrw()
        {
            Run32bitTest("0FC54242");    // pextrw\teax,mm2,42
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|eax = __pextrw<word64>(mm2, 0x42<8>)");
        }

        [Test]
        public void X86rw_pinsrw()
        {
            //$TODO check encoding; look in the Intel spec.
            Run32bitTest("0FC442");    // pinsrw\tmm0,edx
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm0 = __pinsr<word64,word16>(mm0, mm0, edx)");
        }

        [Test]
        public void X86Rw_pinsrd()
        {
            Run32bitTest("660F3A2244240802");
            AssertCode(     // pinsrd	xmm0,dword ptr [esp+8h],2h
                "0|L--|10000000(8): 1 instructions",
                "1|L--|xmm0 = __pinsr<word128,word32>(xmm0, Mem0[esp + 8<i32>:word32], 2<8>)");
        }

        [Test]
        public void X86rw_pxor()
        {
            Run32bitTest("0FEF4242");    // pxor\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pxor<word64>(mm0, Mem0[edx + 66<i32>:word64])");
        }

        [Test]
        public void X86rw_rcpps()
        {
            Run32bitTest("0F534242");    // rcpps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = Mem0[edx + 66<i32>:word128]",
                "2|L--|xmm0 = __rcpp<real32[4]>(v5)");
        }

        [Test]
        public void X86rw_rdmsr()
        {
            Run32bitTest("0F32");    // rdmsr
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_eax = __rdmsr(ecx)");
        }

        [Test]
        public void X86rw_rdpmc()
        {
            Run32bitTest("0F33");    // rdpmc
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_eax = __rdpmc(ecx)");
        }

        [Test]
        public void X86Rw_rdrand()
        {
            Run32bitTest("0FC7F3");
            AssertCode(     // rdrand	ebx
                "0|L--|10000000(3): 5 instructions",
                "1|L--|C = __rdrand(out ebx)",
                "2|L--|S = false",
                "3|L--|Z = false",
                "4|L--|O = false",
                "5|L--|P = false");
        }

        [Test]
        public void X86rw_rdtsc()
        {
            Run32bitTest("0F31");
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_eax = __rdtsc()");
        }

        [Test]
        public void X86Rw_rdtscp()
        {
            Run32bitTest("0F01F9");
            AssertCode(     // rdtscp
                "0|L--|10000000(3): 1 instructions",
                "1|L--|edx_eax = __rdtscp(out ecx)");
        }

        [Test]
        public void X86rw_rsqrtps()
        {
            Run32bitTest("0F524242");    // rsqrtps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = Mem0[edx + 66<i32>:word128]",
                "2|L--|xmm0 = __rsqrtp<real32[4]>(v5)");
        }

        [Test]
        public void X86rw_sqrtps()
        {
            Run32bitTest("0F514242");    // sqrtps\txmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = Mem0[edx + 66<i32>:word128]",
                "2|L--|xmm0 = __sqrtp<real32[4]>(v5)");
        }

        [Test]
        public void X86rw_pcmpgtb()
        {
            Run32bitTest("0F644242");    // pcmpgtb\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:(arr byte 8)]",
                "3|L--|mm0 = __pcmpgt<byte[8]>(v5, v6)");
        }


        [Test]
        public void X86rw_pcmpgtw()
        {
            Run32bitTest("0F654242");    // pcmpgtw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:(arr word16 4)]",
                "3|L--|mm0 = __pcmpgt<word16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_pcmpgtd()
        {
            Run32bitTest("0F664242");    // pcmpgtd\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:(arr word32 2)]",
                "3|L--|mm0 = __pcmpgt<word32[2]>(v5, v6)");
        }

        [Test]
        public void X86rw_punpckhbw()
        {
            Run32bitTest("0F684242");    // punpckhbw\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhbw<word64>(mm0, Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_punpckhwd()
        {
            Run32bitTest("0F694242");    // punpckhwd\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhwd<word64>(mm0, Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_punpckhdq()
        {
            Run32bitTest("0F6A4242");    // punpckhdq\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckhdq<word64>(mm0, Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_punpckldq()
        {
            Run32bitTest("0F624242");    // punpckldq\tmm0,dword ptr [edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __punpckldq<word64>(mm0, Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86Rw_punpcklqdq()
        {
            Run64bitTest("660F6CC0");
            AssertCode(     // punpcklqdq	xmm0,xmm0
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm0 = __punpcklqdq<word128>(xmm0, xmm0)");
        }

        [Test]
        public void X86rw_pcmpeqd()
        {
            Run32bitTest("0F764242");    // pcmpeqd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:(arr word32 2)]",
                "3|L--|mm0 = __pcmpeq<word32[2]>(v5, v6)");
        }

        [Test]
        public void X86rw_pcmpeqw()
        {
            Run32bitTest("0F754242");    // pcmpeqw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:(arr word16 4)]",
                "3|L--|mm0 = __pcmpeq<word16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_vmread()
        {
            Run32bitTest("0F784242");    // vmread\t[edx+42],eax
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|Mem0[edx + 66<i32>:word32] = __vmread<word32,word32>(eax)");
        }

        [Test]
        public void X86rw_vmwrite()
        {
            Run32bitTest("0F794242");    // vmwrite\teax,[edx+42]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|__vmwrite<word32,word32>(eax, Mem0[edx + 66<i32>:word32])");
        }

        [Test]
        public void X86rw_vshufps()
        {
            Run32bitTest("0FC6424207");    // vshufps\txmm0,[edx+42],07
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word128]",
                "3|L--|xmm0 = __shufp<real32[4]>(v5, v6, 7<8>)");
        }

        [Test]
        public void X86rw_pminub()
        {
            Run32bitTest("0FDA4242");    // pminub\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __pminu<uint8[8]>(v5, v6)");
        }

        [Test]
        public void X86rw_pmulhrsw()
        {
            Run32bitTest("0F 38 0B 0B");    // pmulhrsw mm1,[ebx]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm1",
                "2|L--|v6 = Mem0[ebx:word64]",
                "3|L--|mm1 = __pmulhrs<int16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_pmullw()
        {
            Run32bitTest("0FD54242");    // pmullw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __pmull<int16[4],int16[4]>(v5, v6)");
        }
        [Test]
        public void X86rw_pmovmskb()
        {
            Run32bitTest("0FD742");    // pmovmskb\teax,mm2
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v5 = __pmovmskb<word64>(mm2)",
                "2|L--|eax = SEQ(SLICE(eax, word24, 8), v5)");
        }

        [Test]
        public void X86rw_psrad()
        {
            Run32bitTest("0FE24242");    // psrad\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psra<int32[2]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86Rw_psrldq()
        {
            Run64bitTest("660F73DA08");
            AssertCode(     // psrldq	xmm2,8h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = SLICE(xmm2, word128, 0)",
                "2|L--|xmm2 = v4");
        }

        [Test]
        public void X86rw_psrlq()
        {
            Run32bitTest("0FD34242");    // psrlq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psrl<word64[1]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86rw_psubusb()
        {
            Run32bitTest("0FD84242");    // psubusb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __psubus<uint8[8]>(v5, v6)");
        }

        [Test]
        public void X86rw_pmaxub()
        {
            Run32bitTest("0FDE4242");    // pmaxub\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __pmaxu<uint8[8]>(v5, v6)");
        }

        [Test]
        public void X86rw_pavgb()
        {
            Run32bitTest("0FE04242");    // pavgb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pavg<byte[8]>(mm0, Mem0[edx + 66<i32>:(arr byte 8)])");
        }

        [Test]
        public void X86rw_psraw()
        {
            Run32bitTest("0FE14242");    // psraw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psra<int16[4]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86rw_pmulhuw()
        {
            Run32bitTest("0FE44242");    // pmulhuw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __pmulhu<uint16[4],uint16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_pmulhw()
        {
            Run32bitTest("0FE54242");    // pmulhw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __pmulh<int16[4],int16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_psubb()
        {
            Run32bitTest("0FF84242");    // psubb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __psub<byte[8]>(v5, v6)");
        }

        [Test]
        public void X86rw_psubd()
        {
            Run32bitTest("0FFA4242");    // psubd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __psub<word32[2]>(v5, v6)");
        }

        [Test]
        public void X86rw_psubq()
        {
            Run32bitTest("0FFB4242");    // psubq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __psub<word64[1]>(v5, v6)");
        }

        [Test]
        public void X86rw_psubsw()
        {
            Run32bitTest("0FE94242");    // psubsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __psubs<int16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_psubw()
        {
            Run32bitTest("0FF94242");    // psubw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __psub<word16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_psubsb()
        {
            Run32bitTest("0FE84242");    // psubsb\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __psubs<int8[8]>(v5, v6)");
        }

        [Test]
        public void X86rw_pmaxsw()
        {
            Run32bitTest("0FEE4242");    // pmaxsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __pmaxs<int16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_pminsw()
        {
            Run32bitTest("0FEA4242");    // pminsw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __pmins<int16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_por()
        {
            Run32bitTest("0FEB4242");    // por\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = mm0 | Mem0[edx + 66<i32>:word64]");
        }

        [Test]
        public void X86Rw_vpor()
        {
            Run64bitTest("C501EB8BE809E800");
            AssertCode(     // vpor	xmm9,xmm15,[rbx+0E809E8h]
                "0|L--|0000000140000000(8): 1 instructions",
                "1|L--|xmm9 = xmm15 | Mem0[rbx + 15206888<i64>:word128]");
        }

        [Test]
        public void X86rw_pslld()
        {
            Run32bitTest("0FF24242");    // pslld\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psll<word32[2]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86rw_psllq()
        {
            Run32bitTest("0FF34242");    // psllq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psll<word64[1]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86rw_psllw()
        {
            Run32bitTest("0FF14242");    // psllw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = mm0",
                "2|L--|mm0 = __psll<word16[4]>(v5, Mem0[edx + 66<i32>:byte])");
        }

        [Test]
        public void X86rw_pmaddwd()
        {
            Run32bitTest("0FF54242");    // pmaddwd\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __pmaddwd<word16[4],word32[2]>(v5, v6)");
        }

        [Test]
        public void X86rw_pmuludq()
        {
            Run32bitTest("0FF44242");    // pmuludq\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __pmulu<uint32[2],uint64[1]>(v5, v6)");
        }

        [Test]
        public void X86rw_psadbw()
        {
            Run32bitTest("0FF64242");    // psadbw\tmm0,[edx+42]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[edx + 66<i32>:word64]",
                "3|L--|mm0 = __psadbw<byte[8],word16[4]>(v5, v6)");
        }

        [Test]
        public void X86rw_wrmsr()
        {
            Run32bitTest("0F30");    // wrmsr
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|__wrmsr(ecx, edx_eax)");
        }

        [Test]
        public void X86Rw_pushw()
        {
            Run64bitTest("66683412");
            AssertCode(     // pushw	1234h
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|rsp = rsp - 2<i64>",
                "2|L--|Mem0[rsp:word16] = 0x1234<16>");
        }

        [Test]
        public void X86Rw_pxor()
        {
            Run32bitTest("660FEFC0");	// pxor	xmm0,xmm0
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = 0<128>");
        }

        [Test]
        public void X86Rw_stmxcsr()
        {
            Run32bitTest("0FAE5DF0");	// stmxcsr	dword ptr [ebp-10]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|Mem0[ebp - 16<i32>:word32] = mxcsr");
        }

        [Test]
        public void X86Rw_fcmovu()
        {
            Run32bitTest("DADD");	// fcmovu	st(0),st(5)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(NOT_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 5<i8>:real64]");
        }

        [Test]
        public void X86Rw_psrlw()
        {
            Run32bitTest("0FD1E8");	// psrlw	mm5,mm0
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v5 = mm5",
                "2|L--|mm5 = __psrl<word16[4]>(v5, SLICE(mm0, byte, 0))");
        }

        [Test]
        public void X86Rw_psrld()
        {
            Run32bitTest("0FD2F9");	// psrld	mm7,mm1
            AssertCode(
                "0|L--|10000000(3): 2 instructions",
                "1|L--|v5 = mm7",
                "2|L--|mm7 = __psrl<word32[2]>(v5, SLICE(mm1, byte, 0))");
        }

        [Test]
        public void X86Rw_fcmovnu()
        {
            Run32bitTest("DBD9");	// fcmovnu	st(0),st(1)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|T--|if (Test(IS_NAN,P)) branch 10000002",
                "2|L--|ST[Top:real64] = ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86Rw_paddq()
        {
            Run32bitTest("0FD408");	// paddq	mm1,[eax]
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|v5 = mm1",
                "2|L--|v6 = Mem0[eax:word64]",
                "3|L--|mm1 = __simd_add<word64[1]>(v5, v6)");
        }

        [Test]
        public void X86Rw_psubusw()
        {
            Run32bitTest("0FD9450C");	// psubusw	mm0,[ebp+0C]
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = mm0",
                "2|L--|v6 = Mem0[ebp + 12<i32>:word64]",
                "3|L--|mm0 = __psubus<uint16[4]>(v5, v6)");
        }

        [Test]
        public void X86Rw_pshufw()
        {
            Run32bitTest("0F700200");	// pshufw	mm0,[edx],00
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mm0 = __pshuf<word16[4]>(mm0, Mem0[edx:word64], 0<8>)");
        }

        [Test]
        public void X86Rw_fxrstor()
        {
            Run64bitTest("0FAE0B");
            AssertCode(     // fxrstor
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|__fxrstor()");
        }

        [Test]
        public void X86Rw_fxsave()
        {
            Run64bitTest("0FAE00");
            AssertCode(     // fxsave
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|__fxsave()");
        }



        [Test]
        public void X86Rw_fxtract()
        {
            Run32bitTest("D9F4");	// fxtract
            AssertCode(
                "0|L--|10000000(2): 4 instructions",
                "1|L--|v4 = ST[Top:real64]",
                "2|L--|Top = Top - 1<i8>",
                "3|L--|ST[Top + 1<i8>:real64] = __exponent(v4)",
                "4|L--|ST[Top:real64] = __significand(v4)");
        }

        [Test]
        public void X86Rw_fprem()
        {
            Run32bitTest("D9F8");	// fprem
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|ST[Top:real64] = __fprem_x87(ST[Top:real64], ST[Top + 1<i8>:real64])",
                "2|L--|C2 = __fprem_incomplete(ST[Top:real64])");
        }

        [Test]
        public void X86Rw_fprem1()
        {
            Run32bitTest("D9F5");	// fprem1	st(5),st(0)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|ST[Top:real64] = ST[Top:real64] % ST[Top + 1<i8>:real64]",
                "2|L--|C2 = __fprem_incomplete(ST[Top:real64])");
        }

        [Test]
        public void X86Rw_andpd()
        {
            Run32bitTest("660F540550595700");	// andpd	xmm0,[00575950]
            AssertCode(
                "0|L--|10000000(8): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = Mem0[0x00575950<p32>:word128]",
                "3|L--|xmm0 = __andp<word64[2]>(v4, v5)");
        }

        [Test]
        public void X86Rw_vpsubd()
        {
            Run32bitTest("660FFAD0");	// vpsubd	xmm2,xmm0
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = xmm2",
                "2|L--|v6 = xmm0",
                "3|L--|xmm2 = __psub<word32[4]>(v5, v6)");
        }

        [Test]
        public void X86Rw_vpsrlq()
        {
            Run32bitTest("660FD3CA");	// vpsrlq	xmm1,xmm2
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = xmm1",
                "2|L--|xmm1 = __psrl<word64[2]>(v5, SLICE(xmm2, byte, 0))");
        }

        [Test]
        public void X86Rw_pshufb()
        {
            Run32bitTest("660F38000CDD20220F08");
            AssertCode(     // pshufb	xmm1,[80F2220h+ebx*8]
                "0|L--|10000000(10): 3 instructions",
                "1|L--|v5 = xmm1",
                "2|L--|v6 = xmm1",
                "3|L--|xmm1 = __pshufb<byte[16]>(v5, v6, Mem0[0x80F2220<32> + ebx * 8<32>:word128])");
        }

        [Test]
        public void X86Rw_pshuflw()
        {
            Run32bitTest("F20F70C0E0");
            AssertCode(     // pshuflw	xmm0,xmm0,0E0h
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|v5 = xmm0",
                "3|L--|xmm0 = __pshuflw<byte[16]>(v4, v5, 0xE0<8>)");
        }

        [Test]
        public void X86Rw_vpsllq()
        {
            Run32bitTest("660FF3CA");	// psllq	xmm1,xmm2
            AssertCode(
                "0|L--|10000000(4): 2 instructions",
                "1|L--|v5 = xmm1",
                "2|L--|xmm1 = __psll<word64[2]>(v5, SLICE(xmm2, byte, 0))");
        }

        [Test]
        public void X86Rw_orpd()
        {
            Run32bitTest("660F561DA0595700");	// orpd	xmm3,[005759A0]
            AssertCode(
                "0|L--|10000000(8): 3 instructions",
                "1|L--|v4 = xmm3",
                "2|L--|v5 = Mem0[0x005759A0<p32>:word128]",
                "3|L--|xmm3 = __orp<real64[2]>(v4, v5)");
        }

        [Test]
        public void X86Rw_movlpd()
        {
            Run32bitTest("660F12442404");	// movlpd	xmm0,qword ptr [esp+04]
            AssertCode(
                "0|L--|10000000(6): 2 instructions",
                "1|L--|v5 = Mem0[esp + 4<i32>:word64]",
                "2|L--|xmm0 = __movlp<real64[1],real64[2]>(v5)");
        }

        [Test]
        public void X86Rw_vextrw()
        {
            Run32bitTest("660FC5C003");	// vextrw	eax,xmm0,03
            AssertCode(
                "0|L--|10000000(5): 1 instructions",
                "1|L--|eax = __pextrw<word128>(xmm0, 3<8>)");
        }

        [Test]
        public void X86Rw_cvtsd2si()
        {
            Run32bitTest("F20F2DD1");	// cvtsd2si	edx,xmm1
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|edx = CONVERT(SLICE(xmm1, real64, 0), real64, int32)");
        }

        [Test]
        public void X86Rw_vpand()
        {
            Run32bitTest("660FDBFE");	// vpand	xmm7,xmm6
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm7 = __pand<word128>(xmm7, xmm6)");
        }

        [Test]
        public void X86Rw_pand()
        {
            Run32bitTest("0FDBFE");	// pand	mm7,mm6
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm7 = __pand<word64>(mm7, mm6)");
        }

        [Test]
        public void X86Rw_vpaddq()
        {
            Run32bitTest("660FD4FE");	// vpaddq	xmm7,xmm6
            AssertCode(
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = xmm7",
                "2|L--|v6 = xmm6",
                "3|L--|xmm7 = __simd_add<word64[2]>(v5, v6)");
        }

        [Test]
        public void X86Rw_vpandn()
        {
            Run32bitTest("660FDFF2");	// vpandn	xmm6,xmm2
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm6 = __pandn<word128>(xmm6, xmm2)");
        }

        [Test]
        public void X86Rw_pandn()
        {
            Run32bitTest("0FDFF2");	// pandn	mm6,mm2
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|mm6 = __pandn<word64>(mm6, mm2)");
        }

        [Test]
        public void X86Rw_vpinsrw()
        {
            Run32bitTest("660FC4C0");	// vpinsrw	xmm0,eax
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|xmm0 = __pinsr<word128,word16>(xmm0, xmm0, eax)");
        }

        [Test]
        public void X86Rw_ldmxcsr()
        {
            Run32bitTest("0FAE5508");	// ldmxcsr	dword ptr [ebp+08]
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|mxcsr = Mem0[ebp + 8<i32>:word32]");
        }

        [Test]
        public void X86Rw_paddsb()
        {
            Run32bitTest("0FECFF");	// paddsb	mm7,mm7
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|v4 = mm7",
                "2|L--|v5 = mm7",
                "3|L--|mm7 = __padds<int8[8]>(v4, v5)");
        }

        [Test]
        public void X86Rw_getsec()
        {
            Run32bitTest("0F37");	// getsec
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|L--|edx_ebx = __getsec(eax)");
        }

        [Test(Description = "We cannot make 16-bit calls in 32- or 64-bit mode")]
        public void X86Rw_invalid_call()
        {
            Run32bitTest("66FF51CC"); // call word ptr[ecx - 34]
            AssertCode(
                "0|---|10000000(4): 1 instructions",
                "1|---|<invalid>");
        }


        [Test]
        public void X86Rw_cvtss2sd()
        {
            Run64bitTest("F3480F5A0DB5473200");	// cvtss2sd	xmm1,dword ptr [rip+003247B5]
            AssertCode(
                "0|L--|0000000140000000(9): 2 instructions",
                "1|L--|v4 = CONVERT(Mem0[0x00000001403247BE<p64>:real32], real32, real64)",
                "2|L--|xmm1 = SEQ(SLICE(xmm1, word64, 64), v4)");
        }


        [Test]
        public void X86Rw_cvtsd2ss()
        {
            Run64bitTest("F2480F5AC0");	// cvtsd2ss	xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = CONVERT(SLICE(xmm0, real64, 0), real64, real32)",
                "2|L--|xmm0 = SEQ(SLICE(xmm0, word96, 32), v4)");
        }

        [Test]
        public void X86Rw_cvtss2si()
        {
            Run64bitTest("F3480F2D5010");	// cvtss2si	rdx,dword ptr [rax+10]
            AssertCode(
                "0|L--|0000000140000000(6): 1 instructions",
                "1|L--|rdx = CONVERT(Mem0[rax + 16<i64>:real32], real32, int64)");
        }

        [Test]
        public void X86Rw_sqrtsd()
        {
            Run64bitTest("F20F51C0");	// sqrtsd	xmm0,xmm0
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = sqrt(SLICE(xmm0, real64, 0))",
                "2|L--|xmm0 = SEQ(SLICE(xmm0, word64, 64), v4)");
        }

        [Test]
        public void X86Rw_sldt()
        {
            Run64bitTest("0F0001");  // sldt	word ptr [ecx]
            AssertCode(
                 "0|S--|0000000140000000(3): 1 instructions",
                 "1|L--|Mem0[rcx:word16] = __sldt<word16>()");
        }

        [Test]
        public void X86Rw_minpd()
        {
            Run64bitTest("660F5D4242"); // minpd\txmm0,[rdx+42]
            AssertCode(
                "0|L--|0000000140000000(5): 3 instructions",
                "1|L--|v5 = xmm0",
                "2|L--|v6 = Mem0[rdx + 66<i64>:word128]",
                "3|L--|xmm0 = __minp<real64[2]>(v5, v6)");
        }

        [Test]
        public void X86Rw_sgdt()
        {
            Run32bitTest("0F0100");	// sgdt	[eax]
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|Mem0[eax:word48] = __sgdt<word48>()");
        }

        [Test]
        public void X86Rw_sidt()
        {
            Run32bitTest("0F018A86040500");	// sidt	[edx+00050486]
            AssertCode(
                "0|S--|10000000(7): 1 instructions",
                "1|L--|Mem0[edx + 0x50486<32>:word48] = __sidt<word48>()");
        }

        [Test]
        public void X86Rw_lldt()
        {
            Run32bitTest("0F00558D");	// lldt	word ptr [ebp-73]
            AssertCode(
                "0|S--|10000000(4): 1 instructions",
                "1|L--|__lldt<word16>(Mem0[ebp - 115<i32>:word16])");
        }

        [Test]
        public void X86Rw_ud0()
        {
            Run32bitTest("0FFFFF");	// ud0	edi,edi
            AssertCode(
                "0|---|10000000(3): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86Rw_ud1()
        {
            Run32bitTest("0FB900");	// ud1	eax,[eax]
            AssertCode(
                "0|---|10000000(3): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void X86Rw_lidt()
        {
            Run32bitTest("0F011B");	// lidt	[ebx]
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|__lidt<word48>(Mem0[ebx:word48])");
        }

        [Test]
        public void X86Rw_movbe()
        {
            Run32bitTest("0F38F1C3");
            AssertCode(
                "0|L--|10000000(4): 1 instructions",
                "1|L--|ebx = __movbe<word32>(eax)");
        }



        [Test]
        public void X86Rw_sha1msg2()
        {
            Run32bitTest("0F38CA75E8");	// sha1msg2	xmm6,[ebp-18]
            AssertCode(
                "0|L--|10000000(5): 3 instructions",
                "1|L--|v5 = Mem0[ebp - 24<i32>:word128]",
                "2|L--|v6 = xmm6",
                "3|L--|xmm6 = __sha1msg2(v6, v5)");
        }

        [Test]
        public void X86Rw_sha256mds2()
        {
            Run64bitTest("0F38CB00");
            AssertCode(     // sha256mds2	xmm0,[rax]
                "0|L--|0000000140000000(4): 3 instructions",
                "1|L--|v5 = Mem0[rax:word128]",
                "2|L--|v6 = xmm0",
                "3|L--|xmm0 = __sha256mds2(v6, v5)");
        }

        [Test]
        public void X86Rw_smsw()
        {
            Run32bitTest("0F01E0");	// smsw	ax
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|eax = __smsw<word32>()");
        }

        [Test]
        public void X86Rw_lmsw()
        {
            Run32bitTest("0F01F0");	// lmsw	ax
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|__load_machine_status_word(ax)");
        }

        [Test]
         public void X86Rw_cmpxchg8b()
        {
            Run32bitTest("0FC70F");	// cmpxchg8b	qword ptr [edi]
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|Z = __cmpxchg<word64>(edx_eax, Mem0[edi:word64], ecx_ebx, out edx_eax)");
        }

        [Test]
        public void X86Rw_unpckhps()
        {
            Run32bitTest("0F1510");	// unpckhps	xmm2,[eax]
            AssertCode(
                "0|L--|10000000(3): 3 instructions",
                "1|L--|v5 = xmm2",
                "2|L--|v6 = Mem0[eax:word128]",
                "3|L--|xmm2 = __unpckhp<real32[4]>(v5, v6)");
        }

        [Test]
        public void X86Rw_ltr()
        {
            Run32bitTest("0F00983F051019");	// ltr	word ptr [eax+1910053F]
            AssertCode(
                "0|S--|10000000(7): 1 instructions",
                "1|L--|__load_task_register(Mem0[eax + 0x1910053F<32>:word16])");
        }

        [Test]
        public void X86Rw_ffreep()
        {
            Run32bitTest("DFC7");	// ffreep	st(7)
            AssertCode(
                "0|L--|10000000(2): 2 instructions",
                "1|L--|__ffree(ST[Top + 7<i8>:real64])",
                "2|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86Rw_verr()
        {
            Run32bitTest("0F00A5640F00A5");	// verr	word ptr [ebp+A5000F64]
            AssertCode(
                "0|L--|10000000(7): 1 instructions",
                "1|L--|Z = __verify_readable(Mem0[ebp + 0xA5000F64<32>:word16])");
        }

        [Test]
        public void X86Rw_verw()
        {
            Run32bitTest("0F00EB");	// verw	bx
            AssertCode(
                "0|L--|10000000(3): 1 instructions",
                "1|L--|Z = __verify_writeable(bx)");
        }

        [Test]
        public void X86Rw_str()
        {
            Run32bitTest("0F008BF38B4DFC");	// str	word ptr [ebx+FC4D8BF3]
            AssertCode(
                "0|S--|10000000(7): 1 instructions",
                "1|L--|Mem0[ebx + 0xFC4D8BF3<32>:word16] = __store_task_register()");
        }

        [Test]
        public void X86Rw_jmpe()
        {
            Run32bitTest("0FB8");	// jmpe
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__jmpe()");
        }

        [Test]
        public void X86Rw_femms()
        {
            Run32bitTest("0F0E");	// femms
            AssertCode(
                "0|L--|10000000(2): 1 instructions",
                "1|L--|__femms()");
        }

        [Test]
        public void X86Rw_invlpg()
        {
            Run32bitTest("0F0138");	// invlpg	byte ptr [eax]
            AssertCode(
                "0|S--|10000000(3): 1 instructions",
                "1|L--|__invldpg<ptr32>(Mem0[eax:byte])");
        }

        [Test]
        public void X86Rw_rsm()
        {
            Run32bitTest("0FAA");	// rsm
            AssertCode(
                "0|S--|10000000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void X86Rw_movdqa()
        {
            Run64bitTest("660F6F4DC0");
            AssertCode(
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm1 = Mem0[rbp - 64<i64>:word128]");
        }

        [Test]
        public void X86Rw_vmovdqa()
        {
            Run64bitTest("C5F96F4DC0");
            AssertCode(
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm1 = Mem0[rbp - 64<i64>:word128]");
        }

        [Test]
        public void X86Rw_vdivss()
        {
            Run64bitTest("C5 FB 5E 45 E8");
            AssertCode(
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = SLICE(xmm0, real64, 0) / Mem0[rbp - 24<i64>:real64]",
                "2|L--|xmm0 = SEQ(0<64>, v4)");
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
            Run16bitTest("99"); // cwd
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|dx_ax = CONVERT(ax, int16, int32)");
        }

        [Test]
        public void X86Rw_cdq()
        {
            Run32bitTest("99"); // cdq
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|edx_eax = CONVERT(eax, int32, int64)");
        }


        [Test]
        public void X86Rw_cqo()
        {
            Run64bitTest("4899"); // cqo
            AssertCode(
                "0|L--|0000000140000000(2): 1 instructions",
                "1|L--|rdx_rax = CONVERT(rax, int64, int128)");
        }


        [Test]
        public void X86Rw_cbw()
        {
            Run16bitTest("98"); // cbw
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|ax = CONVERT(al, int8, int16)");
        }

        [Test]
        public void X86Rw_cwde()
        {
            Run32bitTest("98"); // cwde
            AssertCode(
                "0|L--|10000000(1): 1 instructions",
                "1|L--|eax = CONVERT(ax, int16, int32)");
        }

        [Test]
        public void X86Rw_cdqe()
        {
            Run64bitTest("4898"); // cdqe
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
                "1|L--|v5 = Mem0[esi - 2<i32>:word32] + 0xFFFFFFFF<32> + C",
                "2|L--|Mem0[esi - 2<i32>:word32] = v5",
                "3|L--|SCZO = cond(v5)");
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

        // If you XOR with 0x40 and you get a 0 result, AH
        // must have been 0x40 before the XOR and after the
        // AND, which according to the Intel manuals means
        // only the C3 bit is set, i.e. "equals".
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
                "1|T--|if (Test(EQ,FPUF)) branch 1000000C");
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
                "1|L--|FPUF = cond(ST[Top:real64] - Mem0[esp + 36<i32>:real32])",
                "2|L--|Top = Top + 1<i8>",
                "3|T--|10000004(11): 3 instructions",
                "4|L--|Mem0[esp + 44<i32>:word32] = edx",
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
              "1|L--|v4 = ST[Top:real64]",
              "2|L--|Top = Top - 1<i8>",
              "3|L--|ST[Top:real64] = v4");
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
                "1|L--|ecx = SLICE(rcx + 1<i64> + rcx, word32, 0)",
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
                "1|L--|v4 = xmm1",
                "2|L--|xmm1 = __cvtdq2pd<int32[4],real64[4]>(v4)");
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
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|ecx = __lzcnt<word32>(esi)",
                "2|L--|Z = ecx == 0<32>");
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
                "1|L--|v4 = xmm0",
                "2|L--|xmm0 = __cvtpd2ps<real64[2],real32[2]>(v4)");
        }

        [Test]
        public void X86Rw_cvtpd2dq()
        {
            Run64bitTest("F20FE6E7");
            AssertCode(     // cvtpd2dq	xmm4,xmm7
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = xmm7",
                "2|L--|xmm4 = __cvtpd2dq<real64[2],int32[2]>(v4)");
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
        [Ignore("This isn't expressing the idea of 'eq'")]
        public void X86Rw_cmpeqps()
        {
            Run32bitTest("0FC20800");
            AssertCode(     // cmpeqps	xmm1,[eax]
                "0|L--|10000000(4): 3 instructions",
                "1|L--|v5 = xmm1",
                "2|L--|v6 = xmm1",
                "3|L--|xmm1 = __cmpp<real32[4],word32[4]>(v5, v6, Mem0[eax:word128])");
        }

        [Test]
        public void X86Rw_cmplesd()
        {
            Run64bitTest("F20FC2E806");
            AssertCode(     // cmplesd	xmm5,6h
                "0|L--|0000000140000000(5): 1 instructions",
                "1|L--|xmm5 = SLICE(xmm5, real64, 0) > SLICE(xmm0, real64, 0) ? 0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128> : 0<128>");
        }

        [Test]
        public void X86Rw_cmpeqsd()
        {
            Run64bitTest("F20FC244241800");
            AssertCode(     // cmpeqsd	xmm0,[esp+18]
                "0|L--|0000000140000000(7): 1 instructions",
                "1|L--|xmm0 = SLICE(xmm0, real64, 0) == Mem0[rsp + 24<i64>:real64] ? 0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128> : 0<128>");
        }

        [Test]
        public void X86Rw_cvttpd2dq()
        {
            Run64bitTest("66450FE6C9");
            AssertCode(     // cvttpd2dq	xmm9,xmm9
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = xmm9",
                "2|L--|xmm9 = __cvttpd2dq<real64[2],int32[2]>(v4)");
        }

        [Test]
        public void X86Rw_cvttps2dq()
        {
            Run64bitTest("F30F5BC0");
            AssertCode(     // cvttps2dq	xmm0,xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|xmm0 = __cvttps2dq<real32[4],int32[4]>(v4)");
        }

        [Test]
        public void X86Rw_vpsrad()
        {
            Run64bitTest("660F72E203");
            AssertCode(     // vpsrad	xmm2,3h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = xmm2",
                "2|L--|xmm2 = __psra<int32[4]>(v4, 3<8>)");
        }

        [Test]
        public void X86Rw_vpsraw()
        {
            Run64bitTest("660F71E008");
            AssertCode(     // vpsraw	xmm0,8h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|xmm0 = __psra<int16[8]>(v4, 8<8>)");
        }

        [Test]
        public void X86Rw_vpsrlw()
        {
            Run64bitTest("660F71D008");
            AssertCode(     // vpsrlw	xmm0,8h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|xmm0 = __psrl<word16[8]>(v4, 8<8>)");
        }

        [Test]
        public void X86Rw_segment_override_string_instr()
        {
            Run16bitTest("26 A4");
            AssertCode(     // movsb\tbyte ptr es:[di],byte ptr es:[si]
                "0|L--|0C00:0000(2): 4 instructions",
                "1|L--|v3 = Mem0[es:si:byte]",
                "2|L--|Mem0[es:di:byte] = v3",
                "3|L--|si = si + 1<i16>",
                "4|L--|di = di + 1<i16>");
        }

        [Test]
        public void X86Rw_sqrtss()
        {
            Run64bitTest("F30F51C8");
            AssertCode(     // sqrtss	xmm1,xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v5 = fsqrt(SLICE(xmm0, real32, 0))",
                "2|L--|xmm1 = SEQ(SLICE(xmm1, word96, 32), v5)");
        }

        [Test]
        public void X86Rw_vpsllw()
        {
            Run64bitTest("660F71F508");
            AssertCode(     // vpsllw	xmm5,8h
                "0|L--|0000000140000000(5): 2 instructions",
                "1|L--|v4 = xmm5",
                "2|L--|xmm5 = __psll<word16[8]>(v4, 8<8>)");
        }

        [Test]
        public void X86Rw_vpxor()
        {
            Run64bitTest("C501EFC0");
            AssertCode(     // vpxor xmm8,xmm15,xmm0
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm8 = __pxor<word128>(xmm15, xmm0)");
        }

        [Test]
        public void X86Rw_vpxor_same_register()
        {
            Run64bitTest("C579EFC0");
            AssertCode(     // vpxor xmm8,xmm0,xmm0
                "0|L--|0000000140000000(4): 1 instructions",
                "1|L--|xmm8 = 0<128>");
        }

        [Test]
        public void X86Rw_sqrtpd()
        {
            Run64bitTest("660F51E4");
            AssertCode(     // sqrtpd	xmm4,xmm4
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = xmm4",
                "2|L--|xmm4 = __sqrtp<real64[2]>(v4)");
        }

        [Test]
        public void X86Rw_cvtps2dq()
        {
            Run64bitTest("660F5BC0");
            AssertCode(     // cvtps2dq	xmm0,xmm0
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|v4 = xmm0",
                "2|L--|xmm0 = __cvtps2dq<real32[4],int32[4]>(v4)");
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
                "1|L--|v5 = xmm0",
                "2|L--|v6 = xmm1",
                "3|L--|xmm0 = __packss<int16[8],int8[16]>(v5, v6)");
        }

        [Test]
        public void X86Rw_xsaveopt()
        {
            Run32bitTest("0FAE74D8BE");
            AssertCode(     // xsaveopt dword ptr [eax+ebx*8-42h]
                "0|L--|10000000(5): 1 instructions",
                "1|L--|__xsaveopt(edx_eax, &Mem0[eax - 66<i32> + ebx * 8<32>:word32])");
        }

        [Test]
        public void X86rw_lea_ptr64_into_32_bitreg()
        {
            Run64bitTest("8D 4C 09 01");    // lea ecx,[rcx + rcx + 1]
            AssertCode(
                "0|L--|0000000140000000(4): 2 instructions",
                "1|L--|ecx = SLICE(rcx + 1<i64> + rcx, word32, 0)",
                "2|L--|rcx = CONVERT(ecx, word32, uint64)");
        }

        [Test]
        public void X86rw_lea_ptr64_into_32_bitreg_2()
        {
            Run64bitTest("8D 14 BD 00 00 00 00");
            AssertCode(
                "0|L--|0000000140000000(7): 2 instructions",
                "1|L--|edx = SLICE(0<i64> + rdi * 4<64>, word32, 0)",
                "2|L--|rdx = CONVERT(edx, word32, uint64)");
        }

        [Test]
        public void X86rw_lea_ptr64_into_32_bitreg_negative_offset()
        {
            Run64bitTest("8D 14 7D F8 FF FF FF ");
            AssertCode(
                "0|L--|0000000140000000(7): 2 instructions",
                "1|L--|edx = SLICE(-8<i64> + rdi * 2<64>, word32, 0)",
                "2|L--|rdx = CONVERT(edx, word32, uint64)");
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
        public void X86Rw_vmptrld()
        {
            Run64bitTest("0FC732");
            AssertCode(     // vmptrld	qword ptr [edx]
                "0|S--|0000000140000000(3): 1 instructions",
                "1|L--|__vmptrld(Mem0[rdx:word64])");
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

        [Test]
        public void X86Rw_wrpkru()
        {
            Run64bitTest("0F01EF");
            AssertCode(     // wrpkru
                "0|L--|0000000140000000(3): 1 instructions",
                "1|L--|__wrpkru(eax)");
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