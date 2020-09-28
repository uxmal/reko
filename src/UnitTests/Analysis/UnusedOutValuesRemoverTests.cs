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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class UnusedOutValuesRemoverTests
    {
        private Mock<IDynamicLinker> import;
        private Mock<DecompilerEventListener> eventListener;
        private ProgramBuilder pb;
        private List<SsaState> ssaStates;
        private RegisterStorage regA;
        private RegisterStorage regB;
        private RegisterStorage lowA;
        private RegisterStorage psw;
        private FlagGroupStorage C;

        [SetUp]
        public void Setup()
        {
            UnusedOutValuesRemover.trace.Level = TraceLevel.Verbose;

            this.import = new Mock<IDynamicLinker>();
            this.eventListener = new Mock<DecompilerEventListener>();
            this.pb = new ProgramBuilder();
            this.ssaStates = new List<SsaState>();
            this.regA = new RegisterStorage("regA", 0x1234, 0, PrimitiveType.Word32);
            this.regB = new RegisterStorage("regB", 0x5678, 0, PrimitiveType.Word32);
            this.lowA = new RegisterStorage("lowA", 0x1234, 0, PrimitiveType.Byte);
            this.psw = new RegisterStorage("psw", 0x4242, 0, PrimitiveType.Word32);
            this.C = new FlagGroupStorage(psw, 0b01, "C", PrimitiveType.Bool);
        }

        private void Given_Procedure(
            string name,
            Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder(name);
            builder(m);
            pb.Add(m);
            ssaStates.Add(m.Ssa);
        }

        private void When_RunUnusedOutValuesRemover()
        {
            var program = pb.BuildProgram();
            var dataFlow = new ProgramDataFlow(program);
            var uvr = new UnusedOutValuesRemover(
                program,
                ssaStates,
                dataFlow,
                import.Object,
                eventListener.Object);
            uvr.Transform();
        }

        private void AssertProceduresCode(string expected)
        {
            var writer = new StringWriter();
            foreach (var ssa in ssaStates)
            {
                writer.WriteLine("========================");
                ssa.Procedure.WriteBody(false, writer);
            }
            var actual = writer.ToString();
            if (actual != expected)
            {
                Console.WriteLine(actual);
            }
            Assert.AreEqual(expected, actual);
        }

        private void RunTest(string sExp, Program program)
        {
            var dfa = new DataFlowAnalysis(
                program, 
                import.Object, 
                eventListener.Object);
            var ssts = dfa.RewriteProceduresToSsa();

            var uvr = new UnusedOutValuesRemover(
                program,
                ssts.Select(sst => sst.SsaState),
                dfa.ProgramDataFlow,
                import.Object,
                eventListener.Object);
            uvr.Transform();

            var sb = new StringWriter();
            foreach (var proc in program.Procedures.Values)
            {
                proc.Write(false, sb);
                sb.WriteLine("===");
            }
            var sActual = sb.ToString();
            if (sExp != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        [Test]
        public void Uvr_Simple()
        {
            var pb = new ProgramBuilder();
            var _r1 = new RegisterStorage("r1", 1, 0, PrimitiveType.Word32);

            pb.Add("main", m =>
            {
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Call("foo", 0);
            });
            pb.Add("foo", m =>
            {
                var r1 = m.Frame.EnsureRegister(_r1);
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(r1, m.Mem32(m.Word32(0x123400)));
                m.MStore(m.Word16(0x123408), r1);
                m.Return();
            });

            pb.BuildProgram();

            var sExp =
            #region Expected
@"// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	call foo (retsize: 0;)
main_exit:
===
// foo
// Return size: 0
define foo
foo_entry:
	def Mem0
	// succ:  l1
l1:
	r1_4 = Mem0[0x00123400:word32]
	Mem5[0x3408:word32] = r1_4
	return
	// succ:  foo_exit
foo_exit:
===
";
            #endregion

            RunTest(sExp, pb.Program);
        }

        [Test(Description = "")]
        public void Uvr_Forks()
        {
            var arch = new FakeArchitecture();
            var pb = new ProgramBuilder(arch);
            var _r1 = arch.GetRegister("r1");
            var _r2 = arch.GetRegister("r2");

            pb.Add("main", m =>
            {
                var r1 = m.Frame.EnsureRegister(_r1);
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Call("foo", 0);
                m.MStore(m.Word32(0x123420), r1);
            });
            pb.Add("foo", m =>
            {
                var r1 = m.Frame.EnsureRegister(_r1);
                var r2 = m.Frame.EnsureRegister(_r2);
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(r1, m.Mem32(m.Word32(0x123400)));
                m.MStore(m.Word32(0x123408), r1);
                m.Assign(r2, m.Mem32(m.Word32(0x123410)));
                m.MStore(m.Word32(0x123418), r2);
                m.Return();
            });

            pb.BuildProgram();

            var sExp =
            #region Expected
@"// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	call foo (retsize: 0;)
		defs: r1:r1_3
	Mem5[0x00123420:word32] = r1_3
main_exit:
===
// foo
// Return size: 0
define foo
foo_entry:
	def Mem0
	// succ:  l1
l1:
	r1_4 = Mem0[0x00123400:word32]
	Mem5[0x00123408:word32] = r1_4
	r2_6 = Mem5[0x00123410:word32]
	Mem7[0x00123418:word32] = r2_6
	return
	// succ:  foo_exit
foo_exit:
	use r1_4
===
";
            #endregion

            RunTest(sExp, pb.Program);
        }

        [Test(Description = "Respect any provided procedure signature")]
        public void Uvr_Signature()
        {
            var arch = new FakeArchitecture();
            var pb = new ProgramBuilder(arch);
            var _r1 = arch.GetRegister("r1");
            var _r2 = arch.GetRegister("r2");

            pb.Add("main", m =>
            {
                var r1 = m.Frame.EnsureRegister(_r1);
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Call("foo", 0);
                m.MStore(m.Word32(0x123420), r1);
            });
            pb.Add("foo", m =>
            {
                var r1 = m.Frame.EnsureRegister(_r1);
                var r2 = m.Frame.EnsureRegister(_r2);
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(r1, m.Mem32(m.Word32(0x123400)));
                m.MStore(m.Word32(0x123408), r1);
                m.Assign(r2, m.Mem32(m.Word32(0x123410)));
                m.MStore(m.Word32(0x123418), r2);
                m.Return();

                m.Procedure.Signature = FunctionType.Func(
                    new Identifier(null, PrimitiveType.Word32, _r1),
                    new Identifier("arg1", PrimitiveType.Word32, _r2));
            });

            pb.BuildProgram();

            var sExp =
            #region Expected
@"// main
// Return size: 0
define main
main_entry:
	def r2
	// succ:  l1
l1:
	r1_4 = foo(r2)
	Mem5[0x00123420:word32] = r1_4
main_exit:
===
// foo
// Return size: 0
word32 foo(word32 arg1)
foo_entry:
	def fp
	def Mem0
	// succ:  l1
l1:
	r63_2 = fp
	r1_4 = Mem0[0x00123400:word32]
	Mem5[0x00123408:word32] = r1_4
	r2_6 = Mem5[0x00123410:word32]
	Mem7[0x00123418:word32] = r2_6
	return
	// succ:  foo_exit
foo_exit:
	use r1_4
===
";
            #endregion

            RunTest(sExp, pb.Program);
        }

        [Test(Description = "Tests a long chain of calls.")]
        [Category(Categories.UnitTests)]
        public void Uvr_Chain()
        {
            var arch = new FakeArchitecture();
            var _r1 = arch.GetRegister("r1");
            var _r2 = arch.GetRegister("r2");
            var _r3 = arch.GetRegister("r3");
            var pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var r1 = m.Frame.EnsureRegister(_r1);
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Call("level1", 0);
                m.MStore(m.Word32(0x00123400), m.Cast(PrimitiveType.Byte, r1)); // forces r1 to be liveout on level1
                m.Return();
            });
            pb.Add("level1", m =>
            {
                // We expect r2 to be live-in, as level2 uses it, and r1 to be leve
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Call("level2", 0);
                m.Return();
            });
            pb.Add("level2", m =>
            {
                var r1 = m.Frame.EnsureRegister(_r1);
                var r2 = m.Frame.EnsureRegister(_r2);
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(r1, m.Mem32(r2));
                m.Return();
            });
            pb.BuildProgram();
            var sExp =
            #region
@"// main
// Return size: 0
define main
main_entry:
	def r2
	// succ:  l1
l1:
	call level1 (retsize: 0;)
		uses: r2:r2
		defs: r1:r1_4
	Mem5[0x00123400:byte] = (byte) r1_4
	return
	// succ:  main_exit
main_exit:
===
// level1
// Return size: 0
define level1
level1_entry:
	def r2
	// succ:  l1
l1:
	call level2 (retsize: 0;)
		uses: r2:r2
		defs: r1:r1_4
	return
	// succ:  level1_exit
level1_exit:
	use r1_4
===
// level2
// Return size: 0
define level2
level2_entry:
	def r2
	def Mem0
	// succ:  l1
l1:
	r1_5 = Mem0[r2:word32]
	return
	// succ:  level2_exit
level2_exit:
	use r1_5
===
";
            #endregion
            RunTest(sExp, pb.Program);
        }

        [Test]
        public void Uvr_Call_DefStgDiffersFromIdentifierStg()
        {
            Given_Procedure("fn", m =>
            {
                var a = m.Reg("a", regA);
                var b = m.Reg("b", regB);
                m.Label("body");
                m.Assign(a, 0x12);
                m.Assign(b, 0x34);
                m.AddUseToExitBlock(a);
                m.AddUseToExitBlock(b);
            });
            Given_Procedure("main", m =>
            {
                var a = m.Local32("a", -4);
                var b = m.Local32("b", -8);
                m.Label("body");
                var uses = new (Storage, Expression)[]
                {
                };
                var defines = new (Storage, Identifier)[]
                {
                    (regA, a), (regB, b)
                };
                m.Call("fn", 4, uses, defines);
                m.MStore(m.Word32(0x5678), b);
                m.AddUseToExitBlock(a);
                m.AddUseToExitBlock(b);
            });

            When_RunUnusedOutValuesRemover();

            var expected =
            #region
@"========================
fn_entry:
body:
	b = 0x00000034
fn_exit:
	use b
========================
main_entry:
body:
	call fn (retsize: 4;)
		defs: regB:b
	Mem2[0x00005678:word32] = b
main_exit:
";
            #endregion
            AssertProceduresCode(expected);
        }

        [Test]
        [Category(Categories.FailedTests)]
        public void Uvr_Call_UseRegIsWiderThanSignatureReturn()
        {
            Given_Procedure("fn", m =>
            {
                m.Ssa.Procedure.Signature = FunctionType.Func(
                    new Identifier("", lowA.DataType, lowA));
                var a = m.Reg("a", regA);
                m.Label("body");
                m.Assign(a, 0x12);
                m.AddUseToExitBlock(a);
            });

            When_RunUnusedOutValuesRemover();

            var expected =
            #region
@"========================
fn_entry:
body:
	a = 0x00000012
fn_exit:
	use a
";
            #endregion
            AssertProceduresCode(expected);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Uvr_Call_UseRegIsWiderThanSignatureReturn_TwoProcedures()
        {
            Given_Procedure("fn", m =>
            {
                var a = m.Reg("a", regA);
                m.Label("body");
                m.Assign(a, 0x12);
                m.AddUseToExitBlock(a);
            });
            Given_Procedure("main", m =>
            {
                m.Ssa.Procedure.Signature = FunctionType.Func(
                    new Identifier("", lowA.DataType, lowA));
                var a = m.Reg("a", regA);
                m.Label("body");
                var uses = new (Storage, Expression)[]
                {
                };
                var defines = new (Storage, Identifier)[]
                {
                    (regA, a)
                };
                m.Call("fn", 4, uses, defines);
                m.AddUseToExitBlock(a);
            });

            When_RunUnusedOutValuesRemover();

            var expected =
            #region
@"========================
fn_entry:
body:
	a = 0x00000012
fn_exit:
	use a
========================
main_entry:
body:
	call fn (retsize: 4;)
		defs: regA:a
main_exit:
	use a
";
            #endregion
            AssertProceduresCode(expected);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Uvr_FlagGroup_SingleBit()
        {
            Given_Procedure("fn", m =>
            {
                var a = m.Reg("a", regA);
                var C_1 = m.Flags("C_1", this.C);
                m.Label("body");
                m.Assign(C_1, m.Cond(m.ISub(a, 0x12)));
                m.Return();
                m.AddUseToExitBlock(a);
                m.AddUseToExitBlock(C_1);
            });
            Given_Procedure("main", m =>
            {
                m.Ssa.Procedure.Signature = FunctionType.Func(
                    new Identifier("", lowA.DataType, lowA));
                var a = m.Reg("a", regA);
                var C_2 = m.Flags("C_2", this.C);
                m.Label("body");
                var uses = new (Storage, Expression)[]
                {
                };
                var defines = new (Storage, Identifier)[]
                {
                    (C, C_2)
                };
                m.Call("fn", 4, uses, defines);
                m.BranchIf(C_2, "m3");
                m.Label("m2");
                m.MStore(m.Word32(0x00123400), m.Byte(0));
                m.Label("m3");
                m.Return();
                m.AddUseToExitBlock(C_2);
            });

            When_RunUnusedOutValuesRemover();

            var expected =
            #region 
@"========================
fn_entry:
body:
	C_1 = cond(a - 0x00000012)
	return
fn_exit:
	use C_1
========================
main_entry:
body:
	call fn (retsize: 4;)
		defs: C:C_2
	branch C_2 m3
m2:
	Mem2[0x00123400:byte] = 0x00
m3:
	return
main_exit:
";
            #endregion

            AssertProceduresCode(expected);
        }
    }
}
