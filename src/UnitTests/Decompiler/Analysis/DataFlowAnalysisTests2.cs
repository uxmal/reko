#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Hll.C;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Decompiler.Analysis
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class DataFlowAnalysisTests2
    {
        private ProgramBuilder pb;
        private Mock<IDynamicLinker> dynamicLinker;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.dynamicLinker = new Mock<IDynamicLinker>();
            this.sc = new ServiceContainer();
            var eventListener = new FakeDecompilerEventListener();
            sc.AddService<IEventListener>(eventListener);
            sc.AddService<IDecompilerEventListener>(eventListener);
            sc.AddService<IPluginLoaderService>(new PluginLoaderService());
        }

        private void AssertProgram(string sExp, Program program)
        {
            var sw = new StringWriter();
            var sep = "";
            foreach (var proc in program.Procedures.Values)
            {
                sw.Write(sep);
                proc.Write(false, sw);
                sep = "===" + Environment.NewLine;
            }
            var sActual = sw.ToString();
            if (sExp != sActual) 
                Debug.WriteLine(sActual);
            Assert.AreEqual(sExp, sw.ToString());
        }

        private void AssertProgramFlow(string sExp, Program program, ProgramDataFlow flow)
        {
            var sw = new StringWriter();
            var sep = "";
            foreach (var proc in program.Procedures.Values)
            {
                sw.Write(sep);
                var pflow = flow.ProcedureFlows[proc];
                sw.WriteLine("// Trashed   {0}", string.Join(", ", pflow.Trashed.OrderBy(s => s.ToString())));
                sw.WriteLine("// Preserved {0}", string.Join(", ", pflow.Preserved.OrderBy(s => s.ToString())));
                sw.WriteLine("// Used      {0}", string.Join(", ", pflow.BitsUsed.Select(s => string.Format("({0}:{1})", s.Key, s.Value)).OrderBy(s => s)));
                proc.Write(false, sw);
                sep = "===" + Environment.NewLine;
            }
            var sActual = sw.ToString();
            if (sExp != sActual)
                Debug.WriteLine(sActual);
            Assert.AreEqual(sExp, sw.ToString());
        }

        private void RunTest(Program program, TextWriter writer)
        {
            var dfa = new DataFlowAnalysis(program, dynamicLinker.Object, sc);
            dfa.AnalyzeProgram();
            foreach (var proc in program.Procedures.Values)
            {
                proc.Write(false, writer);
            }
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Dfa2_Simple()
        {
            var pb = new ProgramBuilder(new FakeArchitecture(new ServiceContainer()));
            pb.Add("test", m =>
                {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                    m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                    m.Assign(r1, m.Mem32(m.Word32(0x010000)));
                    m.Assign(r2, m.Mem32(m.Word32(0x010004)));
                    m.MStore(m.Word32(0x010008), m.IAdd(r1, r2));
                    m.Return();
                });

            var dfa = new DataFlowAnalysis(pb.BuildProgram(), dynamicLinker.Object, sc);
            dfa.AnalyzeProgram();
            var sExp = @"// test
// Return size: 0
void test()
test_entry:
	// succ:  l1
l1:
	Mem6[0x10008<32>:word32] = Mem0[0x10000<32>:word32] + Mem0[0x10004<32>:word32]
	return
	// succ:  test_exit
test_exit:
";
            AssertProgram(sExp, pb.Program);
        }

        [Test]
        public void Dfa2_StackArgs()
        {
            var pb = new ProgramBuilder(new FakeArchitecture(new ServiceContainer()));
            pb.Add("test", m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r1, m.Mem32(m.IAdd(sp, 4)));
                m.Assign(r2, m.Mem32(m.IAdd(sp, 8)));
                m.Assign(r1, m.IAdd(r1, r2));
                m.MStore(m.Ptr32(0x010008), r1);
                m.Return();
            });
            var dfa = new DataFlowAnalysis(pb.BuildProgram(), dynamicLinker.Object, sc);
            dfa.AnalyzeProgram();
            var sExp = @"// test
// Return size: 0
void test(word32 dwArg04, word32 dwArg08)
test_entry:
	// succ:  l1
l1:
	Mem7[0x00010008<p32>:word32] = dwArg04 + dwArg08
	return
	// succ:  test_exit
test_exit:
";
            AssertProgram(sExp, pb.Program);
        }

        private ProcedureBase GivenFunction(string name, RegisterStorage ret, params object [] args)
        {
            var frame = pb.Program.Architecture.CreateFrame();
            Identifier idRet = null;
            if (ret is not null)
            {
                idRet = frame.EnsureRegister(ret);
            }
            List<Identifier> parameters = new List<Identifier>();
            foreach (int offset in args)
            {
                parameters.Add(frame.EnsureStackArgument(offset, pb.Program.Architecture.WordWidth));
            }
            var proc = new ExternalProcedure(name, FunctionType.Create(
                idRet, 
                parameters.ToArray()));
            return proc;
        }

        [Test]
        public void Dfa2_CallProc()
        {
            pb = new ProgramBuilder();
            pb.Add("test", m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);

                var fooProc = GivenFunction("foo", m.Architecture.GetRegister("r1"), 4, 8);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, m.Word32(2));
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, m.Word32(1));
                m.Call(fooProc, 4);
                m.Assign(sp, m.IAdd(sp, 8));
                m.Return();
            });

            var dfa = new DataFlowAnalysis(pb.BuildProgram(), null, sc);
            dfa.AnalyzeProgram();
            var sExp = @"// test
// Return size: 0
void test()
test_entry:
	// succ:  l1
l1:
	foo(1<32>, 2<32>)
	return
	// succ:  test_exit
test_exit:
";
            AssertProgram(sExp, pb.Program);
        }

        [Test]
        [Category(Categories.FailedTests)]
        public void Dfa2_UserDefinedStackArgs()
        {
            var arch = new X86ArchitectureFlat32(new ServiceContainer(), "x86-protected-32",  new Dictionary<string, object>());
            var pb = new ProgramBuilder(arch);
            var test = pb.Add(
                new UserProcedure(pb.NextAddress(), "test")
                {
                    CSignature = "void test(int a, int b)"
                },
                m => {
                    var r1 = m.Reg32("r1", 1);
                    var r2 = m.Reg32("r2", 2);
                    var fp = m.Frame.FramePointer;
                    m.Assign(r1, m.Mem32(m.IAdd(fp, 4)));
                    m.Assign(r2, m.Mem32(m.IAdd(fp, 8)));
                    m.Assign(r1, m.IAdd(r1, r2));
                    m.MStore(m.Ptr32(0x010008), r1);
                    m.Return();
                });
            var program = pb.BuildProgram();
            var platform = new Mock<IPlatform>();
            platform.Setup(p => p.Architecture).Returns(arch);
            platform.Setup(p => p.IsImplicitArgumentRegister(It.IsAny<RegisterStorage>()))
                .Returns(false);
            platform.Setup(p => p.DefaultCallingConvention).Returns("__cdecl");
            platform.Setup(p => p.GetCallingConvention(null))
                .Returns(new X86CallingConvention(4, 4, true, false));
            platform.Setup(p => p.GetBitSizeFromCBasicType(CBasicType.Int)).Returns(32);
            platform.Setup(p => p.PointerType).Returns(PrimitiveType.Ptr32);
            platform.Setup(p => p.CreateCParser(It.IsAny<TextReader>(), It.IsAny<ParserState>()))
                .Returns(new Func<TextReader, ParserState, CParser>((r, s) =>
                {
                    var lex = new CLexer(r, CLexer.MsvcKeywords);
                    return new CParser(s ?? new ParserState(), lex);
                }));
            platform.Setup(p => p.PreservedRegisters).Returns(new HashSet<RegisterStorage>());

            var dynamicLinker = new Mock<IDynamicLinker>().Object;
            program.Platform = platform.Object;
            var usb = new UserSignatureBuilder(program);
            usb.BuildSignatures(new FakeDecompilerEventListener());
            var dfa = new DataFlowAnalysis(program, dynamicLinker, sc);
            dfa.AnalyzeProgram();
            var sExp = @"// test
// Return size: 4
void test(int32 a, int32 b)
test_entry:
	// succ:  l1
l1:
	Mem6[0x00010008<p32>:word32] = a + b
	return
	// succ:  test_exit
test_exit:
";
            AssertProgram(sExp, pb.Program);
        }

        [Test]
        public void Dfa2_Untangle_CallChain()
        {
            pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var r1 = m.Register("r1");
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(r1, m.Mem32(m.Ptr32(0x00123400)));
                m.Call("level1", 0);
                m.MStore(m.Ptr32(0x00123400), r1);
                m.Return();
            });
            pb.Add("level1", m =>
            {
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Call("level2", 0);
                m.Return();
            });
            pb.Add("level2", m =>
            {
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(r2, -1);
                m.Assign(r1, m.IAdd(r1, 1));
                m.Return();
            });
            var program = pb.BuildProgram();

            var dfa = new DataFlowAnalysis(program, dynamicLinker.Object, sc);
            dfa.AnalyzeProgram();

            var sExp =
            #region Expected
@"// main
// Return size: 0
void main()
main_entry:
	// succ:  l1
l1:
	Mem7[0x00123400<p32>:word32] = level1(Mem0[0x00123400<p32>:word32])
	return
	// succ:  main_exit
main_exit:
===
// level1
// Return size: 0
word32 level1(word32 r1)
level1_entry:
	// succ:  l1
l1:
	return level2(r1)
	// succ:  level1_exit
level1_exit:
===
// level2
// Return size: 0
word32 level2(word32 r1)
level2_entry:
	// succ:  l1
l1:
	return r1 + 1<32>
	// succ:  level2_exit
level2_exit:
";
            #endregion
            AssertProgram(sExp, pb.Program);
        }

        [Test(Description = "One level calls the next")]
        public void Dfa2_StackArgs_Nested()
        {
            pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var r1 = m.Register("r1");
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, m.Mem32(m.Ptr32(0x123400)));
                m.Call("level1", 4);
                m.Assign(sp, m.IAdd(sp, 4));
                m.MStore(m.Ptr32(0x123404), r1);
                m.Return();
            });
            pb.Add("level1", m =>
            {
                var r1 = m.Register("r1");
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r1, m.Mem32(m.IAdd(sp, 4)));
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, r1);
                m.Call("level2", 4);
                m.Assign(sp, m.IAdd(sp, 4));
                m.Return();
            });
            pb.Add("level2", m =>
            {
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r1, m.Mem32(m.IAdd(sp, 4)));
                m.Assign(r1, m.IAdd(r1, 1));
                m.Return();
            });
            var program = pb.BuildProgram();

            var dfa = new DataFlowAnalysis(program, dynamicLinker.Object, sc);
            dfa.AnalyzeProgram();

            var sExp =
            #region Expected
@"// main
// Return size: 0
void main()
main_entry:
	// succ:  l1
l1:
	Mem8[0x00123404<p32>:word32] = level1(Mem0[0x00123400<p32>:word32])
	return
	// succ:  main_exit
main_exit:
===
// level1
// Return size: 0
word32 level1(word32 dwArg04)
level1_entry:
	// succ:  l1
l1:
	return level2(dwArg04)
	// succ:  level1_exit
level1_exit:
===
// level2
// Return size: 0
word32 level2(word32 dwArg04)
level2_entry:
	// succ:  l1
l1:
	return dwArg04 + 1<32>
	// succ:  level2_exit
level2_exit:
";
            #endregion
            AssertProgram(sExp, pb.Program);
        }

        [Test]
        public void Dfa2_Byte_Arg()
        {
            pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var r1 = m.Register("r1");
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r1, m.Mem32(m.IAdd(sp, 4)));
                m.Call("level1", 4);
                m.Return();
            });
            pb.Add("level1", m =>
            {
                var r1 = m.Register("r1");
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(sp, m.Frame.FramePointer);
                m.MStore(m.Ptr32(0x1234), m.Slice(r1, PrimitiveType.Byte));
                m.Return();
            });
            var program = pb.BuildProgram();

            var dfa = new DataFlowAnalysis(program, dynamicLinker.Object, sc);
            dfa.AnalyzeProgram();

            var sExp =
            #region Expected
@"// Trashed   r1
// Preserved r63
// Used      (Stack +0004:[0..31])
// main
// Return size: 0
void main(word32 dwArg04)
main_entry:
	// succ:  l1
l1:
	level1(dwArg04)
	return
	// succ:  main_exit
main_exit:
===
// Trashed   
// Preserved r63
// Used      (r1:[0..7])
// level1
// Return size: 0
void level1(word32 r1)
level1_entry:
	// succ:  l1
l1:
	Mem4[0x00001234<p32>:byte] = SLICE(r1, byte, 0)
	return
	// succ:  level1_exit
level1_exit:
";
            #endregion
            AssertProgramFlow(sExp, pb.Program, dfa.ProgramDataFlow);
        }
    }
}
