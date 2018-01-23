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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using Rhino.Mocks;
using System.Collections.Generic;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class SsaTests : AnalysisTestBase
	{
		private SsaState ssa;

		[Test]
		public void SsaSimple()
		{
			RunFileTest("Fragments/ssasimple.asm", "Analysis/SsaSimple.txt");
		}

		[Test]
		public void SsaConverge()
		{
			RunFileTest("Fragments/3converge.asm", "Analysis/SsaConverge.txt");
		}

		[Test]
		public void SsaMemoryTest()
		{
			RunFileTest("Fragments/memory_simple.asm", "Analysis/SsaMemoryTest.txt");			
		}

		[Test]
		public void SsaReg00004()
		{
			RunFileTest32("Fragments/regressions/r00004.asm", "Analysis/SsaReg00004.txt");
		}

		[Test]
		public void SsaReg00005()
		{
			RunFileTest("Fragments/regressions/r00005.asm", "Analysis/SsaReg00005.txt");
		}

		[Test]
		public void SsaAddSubCarries()
		{
			RunFileTest("Fragments/addsubcarries.asm", "Analysis/SsaAddSubCarries.txt");			
		}

		[Test]
		public void SsaSwitch()
		{
			RunFileTest("Fragments/switch.asm", "Analysis/SsaSwitch.txt");
		}
		
		[Test]
		public void SsaFactorial()
		{
			RunFileTest("Fragments/factorial.asm", "Analysis/SsaFactorial.txt");
		}

		[Test]
		public void SsaFactorialReg()
		{
			RunFileTest("Fragments/factorial_reg.asm", "Analysis/SsaFactorialReg.txt");
		}

		[Test]
		public void SsaForkedLoop()
		{
			RunFileTest("Fragments/forkedloop.asm", "Analysis/SsaForkedLoop.txt");
		}

		[Test]
		public void SsaNestedRepeats()
		{
			RunFileTest("Fragments/nested_repeats.asm", "Analysis/SsaNestedRepeats.txt");
		}

        [Test]
        public void SsaOutParamters()
        {
            ProcedureBuilder m = new ProcedureBuilder("foo");
            Identifier r4 = m.Register(4);
            m.Store(m.Word32(0x400), m.Fn("foo", m.Out(PrimitiveType.Pointer32, r4)));
            m.Return();

            RunFileTest(m, "Analysis/SsaOutParameters.txt");
        }

        [Test]
        public void SsaPushAndPop()
        {
            // Mirrors the pattern of stack accesses used by x86 compilers.
            var m = new ProcedureBuilder("SsaPushAndPop");
            var esp = EnsureRegister32(m, "esp");
            var ebp = EnsureRegister32(m, "ebp");
            var eax = EnsureRegister32(m, "eax");
            m.Assign(esp, m.ISub(esp, 4));
            m.Store(esp, ebp);
            m.Assign(ebp, esp);
            m.Assign(eax, m.Mem32(m.IAdd(ebp, 8)));  // dwArg04
            m.Assign(ebp, m.Mem32(esp));
            m.Assign(esp, m.IAdd(esp,4));
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

        [Test]
        public void SsaCallIndirect()
        {
            var m = new ProcedureBuilder("SsaCallIndirect");
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);
            m.Assign(r1, m.Mem32(r2));
            m.Call(r1, 4);
            m.Return();

            RunUnitTest(m, "Analysis/SsaCallIndirect.txt");
        }

        private void RunUnitTest(ProcedureBuilder m, string outfile)
        {
            var proc = m.Procedure;
            var sst = new SsaTransform(
                new ProgramDataFlow(),
                proc,
                null,
                proc.CreateBlockDominatorGraph(),
                new HashSet<RegisterStorage>());
            ssa = sst.SsaState;
            using (var fut = new FileUnitTester(outfile))
            {
                ssa.Write(fut.TextWriter);
                proc.Write(false, fut.TextWriter);
                fut.AssertFilesEqual();
                ssa.CheckUses(s => Assert.Fail(s));
            }
        }

        private Identifier EnsureRegister16(ProcedureBuilder m, string name)
        {
            return m.Frame.EnsureRegister(new RegisterStorage(name, m.Frame.Identifiers.Count, 0, PrimitiveType.Word16));
        }

        private Identifier EnsureRegister32(ProcedureBuilder m, string name)
        {
            return m.Frame.EnsureRegister(new RegisterStorage(name, m.Frame.Identifiers.Count, 0, PrimitiveType.Word32));
        }

		protected override void RunTest(Program prog, TextWriter writer)
		{
            var flow = new ProgramDataFlow(prog);
            var eventListener = new FakeDecompilerEventListener();
            var importResolver = MockRepository.GenerateStub<IImportResolver>();
            importResolver.Replay();
            var trf = new TrashedRegisterFinder(prog, prog.Procedures.Values, flow, eventListener);
            trf.Compute();
            trf.RewriteBasicBlocks();
            Dump(prog.CallGraph);
            RegisterLiveness.Compute(prog, flow, eventListener);
            GlobalCallRewriter.Rewrite(prog, flow, eventListener);

			foreach (Procedure proc in prog.Procedures.Values)
			{
				Aliases alias = new Aliases(proc, prog.Architecture);
				alias.Transform();
				var gr = proc.CreateBlockDominatorGraph();
				SsaTransform sst = new SsaTransform(
                    flow,
                    proc,
                    importResolver,
                    gr,
                    new HashSet<RegisterStorage>());
				ssa = sst.SsaState;
				ssa.Write(writer);
				proc.Write(false, true, writer);
				writer.WriteLine();
                ssa.CheckUses(s => Assert.Fail(s));
			}
		}

        private void Dump(CallGraph cg)
        {
            var sw = new StringWriter();
            cg.Write(sw);
            Debug.Print("{0}", sw.ToString());
        }

        [Test]
        public void SsaSwitchWithSharedBranches()
        {
            var m = new ProcedureBuilder("SsaSwitchWithSharedBranches");

            var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);
            var foo = new ExternalProcedure("foo", new FunctionType(
                new Identifier("", VoidType.Instance, null),
                new Identifier("arg1", PrimitiveType.Int32, new StackArgumentStorage(4, PrimitiveType.Int32))));
            m.Assign(sp, m.Frame.FramePointer);
            m.Assign(r1, m.Mem32(m.IAdd(sp, 4)));
            m.BranchIf(m.Ugt(r1, m.Word32(0x5)), "m4_default");
            m.Label("m1");
            m.Switch(r1,
                "m2", "m2", "m3", "m3", "m2", "m3");
            m.Label("m2");
            m.Assign(sp, m.ISub(sp, 4));
            m.Store(sp, m.Word32(0x42));
            m.Call(foo, 4);
            m.Assign(sp, m.IAdd(sp, 4));
            // fall through
            m.Label("m3");
            m.Assign(sp, m.ISub(sp, 4));
            m.Store(sp, m.Word32(42));
            m.Call(foo, 4);
            m.Assign(sp, m.IAdd(sp, 4));
            // fall through
            m.Label("m4_default");
            m.Assign(sp, m.ISub(sp, 4));
            m.Store(sp, m.Word32(0));
            m.Call(foo, 4);
            m.Assign(sp, m.IAdd(sp, 4));

            m.Return();

            RunUnitTest(m, "Analysis/SsaSwitchWithSharedBranches.txt");
        }
    }
}
