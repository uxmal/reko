#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Analysis;
using System.IO;
using System.Diagnostics;
using Rhino.Mocks;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Core.Expressions;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class UnusedOutValuesRemoverTests
    {
        private MockRepository mr;
        private IImportResolver import;
        private DecompilerEventListener eventListener;

        [SetUp]
        public void Setup()
        {
            UnusedOutValuesRemover.trace.Level = TraceLevel.Verbose;

            this.mr = new MockRepository();
            this.import = mr.Stub<IImportResolver>();
            this.eventListener = mr.Stub<DecompilerEventListener>();
        }

        private void RunTest(string sExp, Program program)
        {
            mr.ReplayAll();

            var dfa = new DataFlowAnalysis(program, import, eventListener);
            var ssts = dfa.RewriteProceduresToSsa();

            var uvr = new UnusedOutValuesRemover(program, ssts, dfa.ProgramDataFlow, eventListener);
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
                Debug.Print(sActual);
                Assert.AreEqual(sExp, sActual);
            }
            mr.VerifyAll();
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
                m.Store(m.Word16(0x123408), r1);
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
                m.Store(m.Word32(0x123420), r1);
            });
            pb.Add("foo", m =>
            {
                var r1 = m.Frame.EnsureRegister(_r1);
                var r2 = m.Frame.EnsureRegister(_r2);
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(r1, m.Mem32(m.Word32(0x123400)));
                m.Store(m.Word32(0x123408), r1);
                m.Assign(r2, m.Mem32(m.Word32(0x123410)));
                m.Store(m.Word32(0x123418), r2);
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
                m.Store(m.Word32(0x123420), r1);
            });
            pb.Add("foo", m =>
            {
                var r1 = m.Frame.EnsureRegister(_r1);
                var r2 = m.Frame.EnsureRegister(_r2);
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(r1, m.Mem32(m.Word32(0x123400)));
                m.Store(m.Word32(0x123408), r1);
                m.Assign(r2, m.Mem32(m.Word32(0x123410)));
                m.Store(m.Word32(0x123418), r2);
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
                m.Store(m.Word32(0x00123400), m.Cast(PrimitiveType.Byte, r1)); // forces r1 to be liveout on level1
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
    }
}
