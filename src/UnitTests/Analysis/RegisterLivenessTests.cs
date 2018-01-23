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
	public class RegisterLivenessTests : AnalysisTestBase
	{
		protected override void RunTest(Program prog, TextWriter writer)
		{
			var eventListener = new FakeDecompilerEventListener();
			var dfa = new DataFlowAnalysis(prog, null, eventListener);
			var trf = new TrashedRegisterFinder(prog, prog.Procedures.Values, dfa.ProgramDataFlow, eventListener);
			trf.Compute();
			trf.RewriteBasicBlocks();
			var rl = RegisterLiveness.Compute(prog, dfa.ProgramDataFlow, eventListener);
			DumpProcedureFlows(prog, dfa, rl, writer);
		}

		[Test]
		public void RlDataConstraint()
		{
			RunFileTest("Fragments/data_constraint.asm", "Analysis/RlDataConstraint.txt");
		}

		[Test]
		public void RlOneProcedure()
		{
			RunFileTest("Fragments/one_procedure.asm", "Analysis/RlOneProcedure.txt");
		}

		/// <summary>
		/// Test that self-recursive functions are handled correctly.
		/// </summary>
		[Test]
        [Category(Categories.UnitTests)]
        public void RlFactorialReg()
		{
			RunFileTest("Fragments/factorial_reg.asm", "Analysis/RlFactorialReg.txt");
		}

		[Test]
        [Category(Categories.UnitTests)]
        public void RlFactorial()
		{
			RunFileTest("Fragments/factorial.asm", "Analysis/RlFactorial.txt");
		}

		[Test]
		public void RlCalleeSave()
		{
			RunFileTest("Fragments/callee_save.asm", "Analysis/RlCalleeSave.txt");
		}

		[Test]
		public void RlDeepNest()
		{
			RunFileTest("Fragments/deep_nest.asm", "Analysis/RlDeepNest.txt");
		}

		[Test]
		public void RlSequence()
		{
			RunFileTest("Fragments/sequence_calls_reg.asm", "Analysis/RlSequence.txt");
		}

		[Test]
		public void RlMiniFloats()
		{
			RunFileTest("Fragments/mini_msfloats_regs.asm", "Analysis/RlMiniFloats.txt");
		}

		[Test]
		[Ignore("Won't pass until ProcedureSignatures for call tables and call pointers are implemented")]
		public void RlCallTables()
		{
			RunFileTest("Fragments/multiple/calltables.asm", "Analysis/RlCallTables.txt");
		}

		[Test]
		public void RlConditionals()
		{
			RunFileTest("Fragments/multiple/conditionals.asm", "Analysis/RlConditionals.txt");
		}

		[Test]
		public void RlFpuOps()
		{
			RunFileTest("Fragments/fpuops.asm", "Analysis/RlFpuOps.txt");
		}

		[Test]
		public void RlIpLiveness()
		{
			RunFileTest("Fragments/multiple/ipliveness.asm", "Analysis/RlIpLiveness.txt");
		}

		[Test]
		public void RlMutual()
		{
			RunFileTest("Fragments/multiple/mutual.asm", "Analysis/RlMutual.txt");
		}

		[Test]
		public void RlStackVariables()
		{
			RunFileTest("Fragments/stackvars.asm", "Analysis/RlStackVariables.txt");
		}

		[Test]
		public void RlProcIsolation()
		{
			RunFileTest("Fragments/multiple/procisolation.asm", "Analysis/RlProcIsolation.txt");
		}

		[Test]
		public void RlLeakyLiveness()
		{
			RunFileTest("Fragments/multiple/leaky_liveness.asm", "Analysis/RlLeakyLiveness.txt");
		}

		[Test]
		public void RlStringInstructions()
		{
			RunFileTest("Fragments/stringinstr.asm", "Analysis/RlStringInstructions.txt");
		}

		[Test]
        [Ignore("scanning-development")]
        public void RlTermination()
		{
			RunFileTest("Fragments/multiple/termination.asm", "Fragments/multiple/termination.xml", "Analysis/RlTermination.txt");
		}

		[Test]
		public void RlLivenessAfterCall()
		{
			RunFileTest("Fragments/multiple/livenessaftercall.asm", "Analysis/RlLivenessAfterCall.txt");
		}

        [Test]
        public void RlPushedRegisters()
        {
            RunFileTest("Fragments/multiple/pushed_registers.asm", "Analysis/RlPushedRegisters.txt");
        }

        [Test]
        public void RlReg00005()
        {
            RunFileTest("Fragments/regressions/r00005.asm", "Analysis/RlReg00005.txt");
        }

        [Test]
        public void RlReg00007()
        {
            RunFileTest("Fragments/regressions/r00007.asm", "Analysis/RlReg00007.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void RlReg00010()
        {
            RunFileTest("Fragments/regressions/r00010.asm", "Analysis/RlReg00010.txt");
        }

        [Test]
        [Ignore("scanning-development")]
        [Category(Categories.UnitTests)]
        public void RlReg00015()
        {
            RunFileTest("Fragments/regressions/r00015.asm", "Analysis/RlReg00015.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void RlPushPop()
        {
            RunFileTest("Fragments/pushpop.asm", "Analysis/RlPushPop.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void RlChainTest()
        {
            RunFileTest("Fragments/multiple/chaincalls.asm", "Analysis/RlChainTest.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void RlSliceReturn()
        {
            RunFileTest("Fragments/multiple/slicereturn.asm", "Analysis/RlSliceReturn.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void RlRecurseWithPushes()
        {
            RunFileTest("Fragments/multiple/recurse_with_pushes.asm", "Analysis/RlRecurseWithPushes.txt");
        }
    }
}
