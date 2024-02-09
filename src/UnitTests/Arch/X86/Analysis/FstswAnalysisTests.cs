#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Arch.X86.Analysis;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Scanning;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Arch.X86.Analysis
{
    [TestFixture]
    public class FstswAnalysisTests
    {
        private void RunTest(string sExpected, Action<X86Assembler> test)
        {
            var sc = new ServiceContainer();
            var arch = new Reko.Arch.X86.X86ArchitectureFlat32(
                sc,
                "x86-protected-32",
                new());
            var symbols = new List<ImageSymbol>();
            var asm = new X86Assembler(
                arch,
                Address.Ptr32(0x10_0000),
                symbols);
            asm.Proc(nameof(FstswAnalysisTests));
            test(asm);
            var program = asm.GetImage();
            foreach (var sym in symbols)
            {
                program.ImageSymbols.Add(sym.Address, sym);
            }
            program.Platform = new DefaultPlatform(sc, arch);

            DoRunTest(sExpected, sc, program);
        }

        private void RunTest(string sExpected, string hexbytes)
        {
            var sc = new ServiceContainer();
            var arch = new Reko.Arch.X86.X86ArchitectureFlat32(
                sc,
                "x86-protected-32",
                new());
            var symbols = new List<ImageSymbol>();
            var mem = new ByteMemoryArea(Address.Ptr32(0x10_0000), BytePattern.FromHexBytes(hexbytes));
            var segmentMap = new SegmentMap(new ImageSegment("code", mem, AccessMode.ReadExecute));
            var program = new Program(
                new ProgramMemory(segmentMap),
                arch,
                new DefaultPlatform(sc, arch));
            program.EntryPoints.Add(mem.BaseAddress, ImageSymbol.Procedure(arch, mem.BaseAddress));

            DoRunTest(sExpected, sc, program);
        }

        private static void DoRunTest(string sExpected, ServiceContainer sc, Program program)
        {
            var listener = new FakeDecompilerEventListener();
            sc.AddService<IEventListener>(listener);
            sc.AddService<IDecompilerEventListener>(listener);
            var dynlinker = new Mock<IDynamicLinker>().Object;
            var scanner = new Scanner(program, new TypeLibrary(), dynlinker, sc);
            scanner.ScanImage();
            var proc = program.Procedures.Values.First();
            var sst = new SsaTransform(program, proc, new HashSet<Procedure> { proc }, dynlinker, new());
            var ssa = sst.Transform();

            var fststw = new FstswAnalysis(program, listener);
            (ssa, _) = fststw.Transform(ssa);

            var sw = new StringWriter();
            sw.WriteLine();
            ssa.Procedure.EntryBlock.Succ[0].Write(sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
            ssa.Validate(s => Assert.Fail(s));
        }

        [Test]
        public void Fstsw_fcmp()
        {
            var sExpected =
            #region Expected
                @"
l00100000:
	FPUF_5 = cond(ST[Top:real64] - ST[Top + 1<i8>:real64])
	Top_6 = Top + 2<i8>
	ax_7 = __fstsw(FPUF_5)
	ah_8 = SLICE(ax_7, byte, 8) (alias)
	SZP_9 = cond(ah_8 & 0x40<8>)
	Z_12 = SLICE(SZP_9, bool, 2) (alias)
	O_10 = false
	C_11 = false
	branch Test(EQ,FPUF_5) l0010000D
";
            #endregion
            RunTest(sExpected, m =>
            {
                m.Fcompp();
                m.Fstsw(m.ax);
                m.Test(m.ah, m.Const(0x40));
                m.Jnz("foo");
                m.Xor(m.eax, m.eax);
                m.Ret();
                m.Label("foo");
                m.Mov(m.eax, 1);
                m.Ret();
            });
        }

        [Test]
        public void FstswSahf()
        {
            var sExpected =
            #region Expected
                @"
l00100000:
	ax_4 = __fstsw(FPUF)
	ah_5 = SLICE(ax_4, byte, 8) (alias)
	SCZO_6 = FPUF
	Z_7 = SLICE(SCZO_6, bool, 2) (alias)
	branch Test(NE,Z_7) l00100006
";
            #endregion
            RunTest(sExpected, m =>
            {
                m.Fstsw(m.ax);
                m.Sahf();
                m.Jnz("foo");
                m.Label("foo");
                m.Ret();
            });
        }

        [Test]
        public void FstswTestAhEq()
        {
            RunTest(@"
l00100000:
	Top_3 = PHI((Top, fn00100000_entry), (Top_6, l00100000))
	FPUF_5 = cond(ST[Top_3:real64] - ST[Top_3 + 1<i8>:real64])
	Top_6 = Top_3 + 2<i8>
	ax_7 = __fstsw(FPUF_5)
	ah_8 = SLICE(ax_7, byte, 8) (alias)
	SZP_9 = cond(ah_8 & 0x44<8>)
	P_12 = SLICE(SZP_9, bool, 5) (alias)
	O_10 = false
	C_11 = false
	branch Test(NE,FPUF_5) l00100000
",
            m =>
            {
                m.Label("foo");
                m.Fcompp();
                m.Fstsw(m.ax);
                m.Test(m.ah, m.Const(0x44));
                m.Jpe("foo");
                m.Ret();
            });
        }

        [Test]
        public void Fstsw_InterleavedInstructions()
        {
            RunTest(
                @"
l00100000:
	Top_4 = Top - 1<i8>
	ST6[Top_4:real64] = CONVERT(Mem0[esp_2 + 20<i32>:real32], real32, real64)
	FPUF_7 = cond(ST6[Top_4:real64] - ST6[Top_4 + 1<i8>:real64])
	Top_8 = Top_4 + 1<i8>
	ax_9 = __fstsw(FPUF_7)
	ah_12 = SLICE(ax_9, byte, 8) (alias)
	ST10[Top_8:real64] = ST6[Top_8:real64]
	Top_11 = Top_8 + 1<i8>
	SZP_13 = cond(ah_12 & 5<8>)
	P_16 = SLICE(SZP_13, bool, 5) (alias)
	O_14 = false
	C_15 = false
	branch Test(LT,FPUF_7) l00100014
",

                "D9 44 24 14" +         // fld dword ptr[esp + 14h]
                "D8 D9" +               // fcomp st(0),st(1)
                "DF E0" +               // fstsw ax
                "DD D8" +               // fstp st(0)
                "F6 C4 05" +            // test ah,5h
                "0F 8B 01 00 00 00" +   // jpo 41E652h
                "C3" +                  // ret
                "C3");                  // ret
        }

        [Test]
        public void FstswTestMov()
        {
            RunTest(
                 @"
l00100000:
	ax_4 = __fstsw(FPUF)
	ah_5 = SLICE(ax_4, byte, 8) (alias)
	SZP_6 = cond(ah_5 & 0x41<8>)
	Z_12 = SLICE(SZP_6, bool, 2) (alias)
	O_7 = false
	C_8 = false
	eax_11 = Mem0[esp_2 + 4<i32>:word32]
	branch Test(LE,FPUF) l00100000
",
                 m =>
            {
                m.Label("foo");
                m.Fstsw(m.ax);
                m.Test(m.ah, 0x41);
                m.Mov(m.eax, m.DwordPtr(m.esp, 4));
                m.Jnz("foo");
                m.Ret();
            });
        }

        [Test]
        public void Fstsw_and_cmp_jz__eq()
        {
            RunTest(
               @"
l00100000:
	ax_4 = __fstsw(FPUF)
	ah_5 = SLICE(ax_4, byte, 8) (alias)
	ah_6 = ah_5 & 0x45<8>
	SZ_7 = cond(ah_6)
	O_8 = false
	C_9 = false
	SCZO_10 = cond(ah_6 - 0x40<8>)
	Z_11 = SLICE(SCZO_10, bool, 2) (alias)
	branch Test(EQ,FPUF) l00100000
",
               m =>
               {
                   m.Label("foo");
                   m.Fstsw(m.ax);
                   m.And(m.ah, 0x45);
                   m.Cmp(m.ah, 0x40);
                   m.Jz("foo");
                   m.Ret();
               });
                //"DF E0" +       // fstsw	ax
                //"80 E4 45" +    // and	ah,45
                //"80 FC 40" +    // cmp	ah,40
                //"74 02");       // jz	$+4
        }

        // If you XOR with 0x40 and you get a 0 result, AH
        // must have been 0x40 before the XOR and after the
        // AND, which according to the Intel manuals means
        // only the C3 bit is set, i.e. "equals".
        [Test]
        public void Fstsw_and_xor_40_jz__ne()
        {
            RunTest(
@"
l00100000:
	ax_4 = __fstsw(FPUF)
	ah_5 = SLICE(ax_4, byte, 8) (alias)
	ah_6 = ah_5 & 0x45<8>
	SZ_7 = cond(ah_6)
	O_8 = false
	C_9 = false
	ah_10 = ah_6 ^ 0x40<8>
	SZ_11 = cond(ah_10)
	Z_14 = SLICE(SZ_11, bool, 2) (alias)
	O_12 = false
	C_13 = false
	branch Test(EQ,FPUF) l00100000
",
            m =>
            {
                m.Label("foo");
                m.Fstsw(m.ax);
                m.And(m.ah, 0x45);
                m.Xor(m.ah, 0x40);
                m.Jz("foo");
                m.Ret();
            });
                //"DF E0" +       // fstsw	ax
                //"80 E4 45" +    // and	ah,45
                //"80 F4 40" +    // xor ah, 40
                //"74 02");       // jz	$+4
        }

        [Test]
        public void Fstsw_test_45_jz__gt()
        {
            RunTest(
                 @"
l00100000:
	ax_4 = __fstsw(FPUF)
	ah_5 = SLICE(ax_4, byte, 8) (alias)
	SZP_6 = cond(ah_5 & 0x45<8>)
	Z_9 = SLICE(SZP_6, bool, 2) (alias)
	O_7 = false
	C_8 = false
	branch Test(GT,FPUF) l00100000
",
                 m =>
                 {
                     m.Label("foo");
                     m.Fstsw(m.ax);
                     m.Test(m.ah, 0x45);
                     m.Jz("foo");
                     m.Ret();
                 });
                //"DF E0" +       // fstsw	ax
                //"F6 C4 45" +    // test	ah,45
                //"74 02");       // jz	0804849B
        }

        [Test]
        public void Fstsw_ah_44_pe()
        {
            RunTest(
                 @"
l00100000:
	Mem6 = PHI((Mem0, fn00100000_entry), (Mem11, l00100000))
	Top_3 = PHI((Top, fn00100000_entry), (Top_8, l00100000))
	FPUF_7 = cond(ST[Top_3:real64] - Mem6[esp_2 + 36<i32>:real32])
	Top_8 = Top_3 + 1<i8>
	ax_9 = __fstsw(FPUF_7)
	ah_12 = SLICE(ax_9, byte, 8) (alias)
	Mem11[esp_2 + 44<i32>:word32] = edx
	SZP_13 = cond(ah_12 & 0x44<8>)
	P_16 = SLICE(SZP_13, bool, 5) (alias)
	O_14 = false
	C_15 = false
	branch Test(NE,FPUF_7) l00100000
",
                 m =>
                 {
                     m.Fcomp(m.DwordPtr(m.esp, 0x24));
                     m.Fstsw(m.ax);
                     m.Mov(m.DwordPtr(m.esp, 0x2C), m.edx);
                     m.Test(m.ah, 0x44);
                     m.Jpe(nameof(Fstsw_ah_44_pe));
                     m.Ret();
                 });
                //"D8 5C 24 24" + // fcomp dword ptr [esp+24h]
                //"DF E0" +       // fstsw ax
                //"89 54 24 2C" + // mov [esp+2Ch],edx
                //"F6 C4 44" +    // test ah,44h
                //"7A 26");       // jpe 0D2316h
        }

        [Test]
        public void Fstsw_test_05_jz__ge()
        {
            RunTest(
                 @"
l00100000:
	ax_4 = __fstsw(FPUF)
	ah_5 = SLICE(ax_4, byte, 8) (alias)
	SZP_6 = cond(ah_5 & 5<8>)
	Z_9 = SLICE(SZP_6, bool, 2) (alias)
	O_7 = false
	C_8 = false
	branch Test(GE,FPUF) l00100000
",
                 m =>
                 {
                     m.Label("foo");
                     m.Fstsw(m.ax);
                     m.Test(m.ah, 5);
                     m.Jz("foo");
                     m.Ret();
                 });
                //"DF E0" +       // fstsw	ax
                //"F6 C4 05" +    // test	ah,05 -- ge
                //"74 02");       // jz	080484BE
        }

        [Test]
        public void Fstsw_sahf_jp()
        {
            RunTest(@"
l00100000:
	ax_4 = __fstsw(FPUF)
	ah_5 = SLICE(ax_4, byte, 8) (alias)
	SCZO_6 = FPUF
	C_7 = SLICE(SCZO_6, bool, 1) (alias)
	branch Test(ULT,C_7) l00100000
",
            m =>
            {
                m.Label("foo");
                m.Fstsw(m.ax);
                m.Sahf();
                m.Jc("foo");
                m.Ret();
            });
        }

        [Test]
        public void Fstsw_ax_40_jne()
        {
            RunTest(
                 @"
l00100000:
	ax_4 = __fstsw(FPUF)
	ah_5 = SLICE(ax_4, byte, 8) (alias)
	SZP_6 = cond(ah_5 & 0x40<8>)
	Z_9 = SLICE(SZP_6, bool, 2) (alias)
	O_7 = false
	C_8 = false
	branch Test(EQ,FPUF) l00100000
",
                m => {
                    m.Label("foo");
                    m.Fstsw(m.ax);
                    m.Test(m.ah, 0x40);
                    m.Jnz("foo");
                    m.Ret();
                });
        }

        [Test]
        public void Fstsw_ax_01_je()
        {
            RunTest(
                 @"
l00100000:
	ax_4 = __fstsw(FPUF)
	ah_5 = SLICE(ax_4, byte, 8) (alias)
	SZP_6 = cond(ah_5 & 1<8>)
	Z_9 = SLICE(SZP_6, bool, 2) (alias)
	O_7 = false
	C_8 = false
	branch Test(GE,FPUF) l00100000
",
            m =>
            {
                m.Label("foo");
                m.Fstsw(m.ax);
                m.Test(m.ah, 0x01);
                m.Jz("foo");
                m.Ret();
            });
        }

        // If you XOR with 0x40 and you get a non-0 result, AH
        // cannot have been 0x40 before the XOR and after the
        // AND, which means it could not have been "equals".
        [Test]
        public void Fstsw_ax_xor_40()
        {
            RunTest(@"
l00100000:
	ax_4 = __fstsw(FPUF)
	ah_5 = SLICE(ax_4, byte, 8) (alias)
	ah_6 = ah_5 & 0x45<8>
	SZ_7 = cond(ah_6)
	O_8 = false
	C_9 = false
	ah_10 = ah_6 ^ 0x40<8>
	SZ_11 = cond(ah_10)
	Z_14 = SLICE(SZ_11, bool, 2) (alias)
	O_12 = false
	C_13 = false
	branch Test(NE,FPUF) l00100000
",
                m =>
            {
                m.Label("foo");
                m.Fstsw(m.ax);
                m.And(m.ah, 0x45);
                m.Xor(m.ah, 0x40);
                m.Jnz("foo");
                m.Ret();
            });
        }


        [Test]
        public void Fstsw_through_alias()
        {
            RunTest(@"
l00100000:
	ax_4 = __fstsw(FPUF)
	ah_5 = SLICE(ax_4, byte, 8) (alias)
	SZP_6 = cond(ah_5 & 0x41<8>)
	Z_9 = SLICE(SZP_6, bool, 2) (alias)
	O_7 = false
	C_8 = false
	eax_12 = SEQ(eax_16_16, ax_4) (alias)
	branch Test(GT,FPUF) l00100000
",
                m =>
                {
                    m.Label("foo");
                    m.Fstsw(m.ax);
                    m.Test(m.ah, 0x41);
                    m.Jz("foo");

                    m.Label("m1");
                    m.Or(m.eax, m.Imm(0xFFFFFFFF));
                    m.Ret();
                });
        }
    }
}
