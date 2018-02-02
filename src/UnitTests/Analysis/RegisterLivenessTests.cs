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

using Reko.Arch.X86;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.UnitTests.Analysis
{
	/// <summary>
	/// Used to test register liveness across the whole program.
	/// </summary>
	[TestFixture]
    [Ignore("Register liveness is now handled by the new SsaTransform class")]
	public class RegisterLivenessTests : AnalysisTestBase
	{
		protected override void RunTest(Program program, TextWriter writer)
		{
			var eventListener = new FakeDecompilerEventListener();
			var dfa = new DataFlowAnalysis(program, null, eventListener);
			var trf = new TrashedRegisterFinder(program, program.Procedures.Values, dfa.ProgramDataFlow, eventListener);
			trf.Compute();
			trf.RewriteBasicBlocks();
			var rl = RegisterLiveness.Compute(program, dfa.ProgramDataFlow, eventListener);
			DumpProcedureFlows(program, dfa, rl, writer);
		}

		[Test]
		public void RlDataConstraint()
		{
			RunFileTest_x86_real("Fragments/data_constraint.asm", "Analysis/RlDataConstraint.txt");
		}

		[Test]
		public void RlOneProcedure()
		{
			RunFileTest_x86_real("Fragments/one_procedure.asm", "Analysis/RlOneProcedure.txt");
		}

		/// <summary>
		/// Test that self-recursive functions are handled correctly.
		/// </summary>
		[Test]
        [Category(Categories.UnitTests)]
        public void RlFactorialReg()
		{
			RunFileTest_x86_real("Fragments/factorial_reg.asm", "Analysis/RlFactorialReg.txt");
		}

		[Test]
        [Category(Categories.UnitTests)]
        public void RlFactorial()
		{
			RunFileTest_x86_real("Fragments/factorial.asm", "Analysis/RlFactorial.txt");
		}

		[Test]
		public void RlCalleeSave()
		{
			RunFileTest_x86_real("Fragments/callee_save.asm", "Analysis/RlCalleeSave.txt");
		}

		[Test]
		public void RlDeepNest()
		{
			RunFileTest_x86_real("Fragments/deep_nest.asm", "Analysis/RlDeepNest.txt");
		}

		[Test]
		public void RlSequence()
		{
			RunFileTest_x86_real("Fragments/sequence_calls_reg.asm", "Analysis/RlSequence.txt");
		}

		[Test]
		public void RlMiniFloats()
		{
			RunFileTest_x86_real("Fragments/mini_msfloats_regs.asm", "Analysis/RlMiniFloats.txt");
		}

		[Test]
		[Ignore("Won't pass until ProcedureSignatures for call tables and call pointers are implemented")]
		public void RlCallTables()
		{
			RunFileTest_x86_real("Fragments/multiple/calltables.asm", "Analysis/RlCallTables.txt");
		}

		[Test]
		public void RlConditionals()
		{
			RunFileTest_x86_real("Fragments/multiple/conditionals.asm", "Analysis/RlConditionals.txt");
		}

		[Test]
		public void RlFpuOps()
		{
			RunFileTest_x86_real("Fragments/fpuops.asm", "Analysis/RlFpuOps.txt");
		}

		[Test]
		public void RlIpLiveness()
		{
			RunFileTest_x86_real("Fragments/multiple/ipliveness.asm", "Analysis/RlIpLiveness.txt");
		}

		[Test]
		public void RlMutual()
		{
			RunFileTest_x86_real("Fragments/multiple/mutual.asm", "Analysis/RlMutual.txt");
		}

		[Test]
		public void RlStackVariables()
		{
			RunFileTest_x86_real("Fragments/stackvars.asm", "Analysis/RlStackVariables.txt");
		}

		[Test]
		public void RlProcIsolation()
		{
			RunFileTest_x86_real("Fragments/multiple/procisolation.asm", "Analysis/RlProcIsolation.txt");
		}

		[Test]
		public void RlLeakyLiveness()
		{
			RunFileTest_x86_real("Fragments/multiple/leaky_liveness.asm", "Analysis/RlLeakyLiveness.txt");
		}

		[Test]
		public void RlStringInstructions()
		{
			RunFileTest_x86_real("Fragments/stringinstr.asm", "Analysis/RlStringInstructions.txt");
		}

		[Test]
        [Ignore("scanning-development")]
        public void RlTermination()
		{
			RunFileTest_x86_real("Fragments/multiple/termination.asm", "Fragments/multiple/termination.xml", "Analysis/RlTermination.txt");
		}

		[Test]
		public void RlLivenessAfterCall()
		{
			RunFileTest_x86_real("Fragments/multiple/livenessaftercall.asm", "Analysis/RlLivenessAfterCall.txt");
		}

        [Test]
        public void RlPushedRegisters()
        {
            RunFileTest_x86_real("Fragments/multiple/pushed_registers.asm", "Analysis/RlPushedRegisters.txt");
        }

        [Test]
        public void RlReg00005()
        {
            RunFileTest_x86_real("Fragments/regressions/r00005.asm", "Analysis/RlReg00005.txt");
        }

        [Test]
        public void RlReg00007()
        {
            RunFileTest_x86_real("Fragments/regressions/r00007.asm", "Analysis/RlReg00007.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void RlReg00010()
        {
            RunFileTest_x86_real("Fragments/regressions/r00010.asm", "Analysis/RlReg00010.txt");
        }

        [Test]
        [Ignore("scanning-development")]
        [Category(Categories.UnitTests)]
        public void RlReg00015()
        {
            RunFileTest_x86_real("Fragments/regressions/r00015.asm", "Analysis/RlReg00015.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void RlPushPop()
        {
            RunFileTest_x86_real("Fragments/pushpop.asm", "Analysis/RlPushPop.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void RlChainTest()
        {
            RunFileTest_x86_real("Fragments/multiple/chaincalls.asm", "Analysis/RlChainTest.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void RlSliceReturn()
        {
            RunFileTest_x86_real("Fragments/multiple/slicereturn.asm", "Analysis/RlSliceReturn.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void RlRecurseWithPushes()
        {
            RunFileTest_x86_real("Fragments/multiple/recurse_with_pushes.asm", "Analysis/RlRecurseWithPushes.txt");
        }
    }
}
