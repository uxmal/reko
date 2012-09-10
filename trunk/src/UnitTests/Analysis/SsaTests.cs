#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Lib;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class SsaTests : AnalysisTestBase
	{
		private SsaState ssa;

		[Test]
		public void SsaSimple()
		{
			RunTest("Fragments/ssasimple.asm", "Analysis/SsaSimple.txt");
		}

		[Test]
		public void SsaConverge()
		{
			RunTest("Fragments/3converge.asm", "Analysis/SsaConverge.txt");
		}

		[Test]
		public void SsaMemoryTest()
		{
			RunTest("Fragments/memory_simple.asm", "Analysis/SsaMemoryTest.txt");			
		}

		[Test]
		public void SsaReg00004()
		{
			RunTest32("Fragments/regressions/r00004.asm", "Analysis/SsaReg00004.txt");
		}

		[Test]
		public void SsaReg00005()
		{
			RunTest("Fragments/regressions/r00005.asm", "Analysis/SsaReg00005.txt");
		}

		[Test]
		public void SsaAddSubCarries()
		{
			RunTest("Fragments/addsubcarries.asm", "Analysis/SsaAddSubCarries.txt");			
		}

		[Test]
		public void SsaSwitch()
		{
			RunTest("Fragments/Switch.asm", "Analysis/SsaSwitch.txt");
		}
		
		[Test]
		public void SsaFactorial()
		{
			RunTest("Fragments/factorial.asm", "Analysis/SsaFactorial.txt");
		}

		[Test]
		public void SsaFactorialReg()
		{
			RunTest("Fragments/factorial_reg.asm", "Analysis/SsaFactorialReg.txt");
		}

		[Test]
		public void SsaForkedLoop()
		{
			RunTest("Fragments/forkedloop.asm", "Analysis/SsaForkedLoop.txt");
		}

		[Test]
		public void SsaNestedRepeats()
		{
			RunTest("Fragments/nested_repeats.asm", "Analysis/SsaNestedRepeats.txt");
		}

        [Test]
        public void SsaMockTest()
        {
            var m = new ProcedureBuilder("SsaMock");
            Identifier r0 = m.Register(0);
            Identifier r1 = m.Register(1);

            m.Assign(r0, m.Int32(0));

            m.Label("top");
            m.Compare("Z", r0, m.Int32(2));
            m.BranchCc(ConditionCode.NE, "skip");
            m.Assign(r0, m.Int32(0));

            m.Label("skip");
            m.Compare("Z", r1, m.Int32(3));
            m.BranchCc(ConditionCode.NE, "top");
            m.Return();
            RunTest(m, "Analysis/SsaMockTest.txt");
        }

        [Test]
        public void SsaOutParamters()
        {
            ProcedureBuilder m = new ProcedureBuilder("foo");
            Identifier r4 = m.Register(4);
            m.Store(m.Int32(0x400), m.Fn("foo", m.AddrOf(r4)));
            m.Return();

            RunTest(m, "Analysis/SsaOutParameters.txt");
        }

        [Test]
        public void SsaPushAndPop()
        {
            // Mirrors the pattern of stack accesses used by x86 compilers.
            var m = new ProcedureBuilder("SsaPushAndPop");
            var esp = EnsureRegister32(m, "esp");
            var ebp = EnsureRegister32(m, "ebp");
            var eax = EnsureRegister32(m, "eax");
            m.Assign(esp, m.Sub(esp, 4));
            m.Store(esp, ebp);
            m.Assign(ebp, esp);
            m.Assign(eax, m.LoadDw(m.Add(ebp, 8)));  // dwArg04
            m.Assign(ebp, m.LoadDw(esp));
            m.Assign(esp, m.Add(esp,4));
            m.Return();

            RunUnitTest(m, "Analysis/SsaPushAndPop.txt");
        }

        [Test]
        public void SsaStackReference_Load()
        {
            var m = new ProcedureBuilder("SsaStackReference");
            var wRef = m.Frame.EnsureStackArgument(4, PrimitiveType.Word16);
            var ax = EnsureRegister16(m, "ax");
            m.Assign(ax, wRef);
            m.Return();

            RunUnitTest(m, "Analysis/SsaStackReference.txt");
        }

        private void RunUnitTest(ProcedureBuilder m, string outfile)
        {
            var proc = m.Procedure;
            var sst = new SsaTransform(proc, proc.CreateBlockDominatorGraph());
            ssa = sst.SsaState;
            using (var fut = new FileUnitTester(outfile))
            {
                ssa.Write(fut.TextWriter);
                proc.Write(false, fut.TextWriter);
                fut.AssertFilesEqual();
            }
        }

        private Identifier EnsureRegister16(ProcedureBuilder m, string name)
        {
            return m.Frame.EnsureRegister(new RegisterStorage(name, m.Frame.Identifiers.Count, PrimitiveType.Word16));
        }

        private Identifier EnsureRegister32(ProcedureBuilder m, string name)
        {
            return m.Frame.EnsureRegister(new RegisterStorage(name, m.Frame.Identifiers.Count, PrimitiveType.Word32));
        }

		protected override void RunTest(Program prog, FileUnitTester fut)
		{
            var flow = new ProgramDataFlow(prog);
            var eventListener = new FakeDecompilerEventListener();
            var trf = new TrashedRegisterFinder(prog, prog.Procedures.Values, flow, eventListener);
            trf.Compute();
            trf.RewriteBasicBlocks();
            var rl = RegisterLiveness.Compute(prog, flow, eventListener);
            GlobalCallRewriter.Rewrite(prog, flow);

			foreach (Procedure proc in prog.Procedures.Values)
			{
				Aliases alias = new Aliases(proc, prog.Architecture);
				alias.Transform();
				var gr = proc.CreateBlockDominatorGraph();
				SsaTransform sst = new SsaTransform(proc, gr);
				ssa = sst.SsaState;
				ssa.Write(fut.TextWriter);
				proc.Write(false, true, fut.TextWriter);
				fut.TextWriter.WriteLine();
			}
		}
	}
}
