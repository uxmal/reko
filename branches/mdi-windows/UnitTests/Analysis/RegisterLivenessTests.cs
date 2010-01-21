/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Analysis
{
	/// <summary>
	/// Used to test register liveness across the whole program.
	/// </summary>
	[TestFixture]
	public class RegisterLivenessTests : AnalysisTestBase
	{
		[Test]
		public void RlDataConstraint()
		{
			RunTest("Fragments/data_constraint.asm", "Analysis/RlDataConstraint.txt");
		}

		[Test]
		public void RlOneProcedure()
		{
			RunTest("Fragments/one_procedure.asm", "Analysis/RlOneProcedure.txt");
		}

		/// <summary>
		/// Test that self-recursive functions are handled correctly.
		/// </summary>
		[Test]
		public void RlFactorialReg()
		{
			RunTest("Fragments/factorial_reg.asm", "Analysis/RlFactorialReg.txt");
		}

		[Test]
		public void RlFactorial()
		{
			RunTest("Fragments/factorial.asm", "Analysis/RlFactorial.txt");
		}

		[Test]
		public void RlCalleeSave()
		{
			RunTest("Fragments/callee_save.asm", "Analysis/RlCalleeSave.txt");
		}

		[Test]
		public void RlDeepNest()
		{
			RunTest("Fragments/deep_nest.asm", "Analysis/RlDeepNest.txt");
		}

		[Test]
		public void RlSequence()
		{
			RunTest("Fragments/sequence_calls_reg.asm", "Analysis/RlSequence.txt");
		}

		[Test]
		public void RlMiniFloats()
		{
			RunTest("Fragments/mini_msfloats_regs.asm", "Analysis/RlMiniFloats.txt");
		}

		[Test]
		[Ignore("Won't pass until ProcedureSignatures for call tables and call pointers are implemented")]
		public void RlCallTables()
		{
			RunTest("Fragments/multiple/calltables.asm", "Analysis/RlCallTables.txt");
		}

		[Test]
		public void RlConditionals()
		{
			RunTest("Fragments/multiple/conditionals.asm", "Analysis/RlConditionals.txt");
		}

		[Test]
		public void RlFpuOps()
		{
			RunTest("Fragments/fpuops.asm", "Analysis/RlFpuOps.txt");
		}

		[Test]
		public void RlIpLiveness()
		{
			RunTest("Fragments/multiple/ipliveness.asm", "Analysis/RlIpLiveness.txt");
		}

		[Test]
		public void RlMutual()
		{
			RunTest("Fragments/multiple/mutual.asm", "Analysis/RlMutual.txt");
		}

		[Test]
		public void RlStackVariables()
		{
			RunTest("Fragments/stackvars.asm", "Analysis/RlStackVariables.txt");
		}

		[Test]
		public void RlProcIsolation()
		{
			RunTest("Fragments/multiple/procisolation.asm", "Analysis/RlProcIsolation.txt");
		}

		[Test]
		public void RlLeakyLiveness()
		{
			RunTest("Fragments/multiple/leaky_liveness.asm", "Analysis/RlLeakyLiveness.txt");
		}

		[Test]
		public void RlStringInstructions()
		{
			RunTest("Fragments/stringinstr.asm", "Analysis/RlStringInstructions.txt");
		}

		[Test]
		public void RlTermination()
		{
			RunTest("Fragments/multiple/termination.asm", "Fragments/multiple/termination.xml", "Analysis/RlTermination.txt");
		}

		[Test]
		public void RlLivenessAfterCall()
		{
			RunTest("Fragments/multiple/livenessaftercall.asm", "Analysis/RlLivenessAfterCall.txt");
		}

		[Test]
		public void RlReg00010()
		{
			RunTest("Fragments/regressions/r00010.asm", "Analysis/RlReg00010.txt");
		}
        
        protected override void RunTest(Program prog, FileUnitTester fut)
		{
            FakeDecompilerEventListener eventListener = new FakeDecompilerEventListener();
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, eventListener);
			TrashedRegisterFinder trf = new TrashedRegisterFinder(prog, dfa.ProgramDataFlow, eventListener);
			trf.Compute();
			RegisterLiveness rl = RegisterLiveness.Compute(prog, dfa.ProgramDataFlow, eventListener);
			DumpProcedureFlows(prog, dfa, rl, fut.TextWriter);
		}
	}
}
