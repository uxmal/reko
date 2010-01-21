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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Output;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class CallRewriterTests : AnalysisTestBase
	{
		private DataFlowAnalysis dfa;

		[Test]
		public void CrwAsciiHex()
		{
			RunTest("Fragments/ascii_hex.asm", "Analysis/CrwAsciiHex.txt");
		}

		[Test]
		public void CrwNoCalls()
		{
			RunTest("Fragments/diamond.asm", "Analysis/CrwNoCalls.txt");
		}

		[Test]
		public void CrwEvenOdd()
		{
			RunTest("Fragments/multiple/even_odd.asm", "Analysis/CrwEvenOdd.txt");
		}

		[Test]
		public void CrwFactorial()
		{
			RunTest("Fragments/factorial.asm", "Analysis/CrwFactorial.txt");
		}

		[Test]
		public void CrwFactorialReg()
		{
			RunTest("Fragments/factorial_reg.asm", "Analysis/CrwFactorialReg.txt");
		}

		[Test]
		public void CrwLeakyLiveness()
		{
			RunTest("Fragments/multiple/leaky_liveness.asm", "Analysis/CrwLeakyLiveness.txt");
		}


		[Test]
		public void CrwManyStackArgs()
		{
			RunTest("Fragments/multiple/many_stack_args.asm", "Analysis/CrwManyStackArgs.txt");
		}

		[Test]
		public void CrwStackVariables()
		{
			RunTest("Fragments/stackvars.asm", "Analysis/CrwStackVariables.txt");
		}

		[Test]
		[Ignore("Won't pass until ProcedureSignatures for call tables and call pointers are implemented")]
		public void CrwCallTables()
		{
			RunTest("Fragments/multiple/calltables.asm", "Analysis/CrwCallTables.txt");
		}

		[Test]
		public void CrwFpuArgs()
		{
			RunTest("Fragments/multiple/fpuargs.asm", "Analysis/CrwFpuArgs.txt");
		}

		[Test]
		public void CrwFpuOps()
		{
			RunTest("Fragments/fpuops.asm", "Analysis/CrwFpuOps.txt");
		}

		[Test]
		public void CrwIpLiveness()
		{
			RunTest("Fragments/multiple/ipliveness.asm", "Analysis/CrwIpLiveness.txt");
		}

		[Test]
		public void CrwVoidFunctions()
		{
			RunTest("Fragments/multiple/voidfunctions.asm", "Analysis/CrwVoidFunctions.txt");
		}

		[Test]
		public void CrwMutual()
		{
			RunTest("Fragments/multiple/mutual.asm", "Analysis/CrwMutual.txt");
		}

		[Test]
		public void CrwMemPreserve()
		{
			RunTest("Fragments/multiple/mempreserve.asm", "Analysis/CrwMemPreserve.xml", "Analysis/CrwMemPreserve.txt");
		}

		[Test]
		public void CrwSliceReturn()
		{
			RunTest("Fragments/multiple/slicereturn.asm", "Analysis/CrwSliceReturn.txt");
		}

		[Test]
		public void CrwProcIsolation()
		{
			RunTest("Fragments/multiple/procisolation.asm", "Analysis/CrwProcIsolation.txt");
		}

		[Test]
		public void CrwFibonacci()
		{
			RunTest32("Fragments/multiple/fibonacci.asm", "Analysis/CrwFibonacci.txt");
		}


		protected override void RunTest(Program prog, FileUnitTester fut)
		{
			dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in prog.Procedures.Values)
			{
				ProcedureFlow flow = dfa.ProgramDataFlow[proc];
				proc.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.ArgumentKind, new Formatter(fut.TextWriter));
				fut.TextWriter.WriteLine();
				flow.Emit(prog.Architecture, fut.TextWriter);
				proc.Write(true, fut.TextWriter);
				fut.TextWriter.Flush();
			}
		}
	}
}
