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
using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;
using Reko.Core.Types;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class CallRewriterTests : AnalysisTestBase
	{
		private DataFlowAnalysis dfa;

		[Test]
		public void CrwAsciiHex()
		{
			RunFileTest("Fragments/ascii_hex.asm", "Analysis/CrwAsciiHex.txt");
		}

		[Test]
		public void CrwNoCalls()
		{
			RunFileTest("Fragments/diamond.asm", "Analysis/CrwNoCalls.txt");
		}

		[Test]
		public void CrwEvenOdd()
		{
			RunFileTest("Fragments/multiple/even_odd.asm", "Analysis/CrwEvenOdd.txt");
		}

		[Test]
		public void CrwFactorial()
		{
			RunFileTest("Fragments/factorial.asm", "Analysis/CrwFactorial.txt");
		}

		[Test]
		public void CrwFactorialReg()
		{
			RunFileTest("Fragments/factorial_reg.asm", "Analysis/CrwFactorialReg.txt");
		}

		[Test]
		public void CrwLeakyLiveness()
		{
			RunFileTest("Fragments/multiple/leaky_liveness.asm", "Analysis/CrwLeakyLiveness.txt");
		}

		[Test]
		public void CrwManyStackArgs()
		{
			RunFileTest("Fragments/multiple/many_stack_args.asm", "Analysis/CrwManyStackArgs.txt");
		}

		[Test]
		public void CrwStackVariables()
		{
			RunFileTest("Fragments/stackvars.asm", "Analysis/CrwStackVariables.txt");
		}

		[Test]
		[Ignore("Won't pass until ProcedureSignatures for call tables and call pointers are implemented")]
		public void CrwCallTables()
		{
			RunFileTest("Fragments/multiple/calltables.asm", "Analysis/CrwCallTables.txt");
		}

		[Test]
		public void CrwFpuArgs()
		{
			RunFileTest("Fragments/multiple/fpuArgs.asm", "Analysis/CrwFpuArgs.txt");
		}

		[Test]
		public void CrwFpuOps()
		{
			RunFileTest("Fragments/fpuops.asm", "Analysis/CrwFpuOps.txt");
		}

		[Test]
		public void CrwIpLiveness()
		{
			RunFileTest("Fragments/multiple/ipliveness.asm", "Analysis/CrwIpLiveness.txt");
		}

		[Test]
		public void CrwVoidFunctions()
		{
			RunFileTest("Fragments/multiple/voidfunctions.asm", "Analysis/CrwVoidFunctions.txt");
		}

		[Test]
		public void CrwMutual()
		{
			RunFileTest("Fragments/multiple/mutual.asm", "Analysis/CrwMutual.txt");
		}

		[Test]
        [Ignore("scanning-development")]
        public void CrwMemPreserve()
		{
			RunFileTest("Fragments/multiple/mempreserve.asm", "Analysis/CrwMemPreserve.xml", "Analysis/CrwMemPreserve.txt");
		}

		[Test]
		public void CrwSliceReturn()
		{
			RunFileTest("Fragments/multiple/slicereturn.asm", "Analysis/CrwSliceReturn.txt");
		}

		[Test]
		public void CrwProcIsolation()
		{
			RunFileTest("Fragments/multiple/procisolation.asm", "Analysis/CrwProcIsolation.txt");
		}

		[Test]
		public void CrwFibonacci()
		{
			RunFileTest32("Fragments/multiple/fibonacci.asm", "Analysis/CrwFibonacci.txt");
		}

        protected override void RunTest(Program prog, TextWriter writer)
		{
			dfa = new DataFlowAnalysis(prog, null, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in prog.Procedures.Values)
			{
				ProcedureFlow flow = dfa.ProgramDataFlow[proc];
				proc.Signature.Emit(proc.Name, FunctionType.EmitFlags.ArgumentKind, new TextFormatter(writer));
				writer.WriteLine();
				flow.Emit(prog.Architecture, writer);
				proc.Write(true, writer);
				writer.Flush();
			}
		}
	}
}
