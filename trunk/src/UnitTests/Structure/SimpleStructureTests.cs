/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Output;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	public class SimpleStructureTests : StructureTestBase
	{
		[Test]
		public void StrIf()
		{
			RunTest("Fragments/if.asm", "Structure/StrIf.txt");
		}

		[Test]
		public void StrFactorialReg()
		{
			RunTest("Fragments/factorial_reg.asm", "Structure/StrFactorialReg.txt");
		}

		[Test]
		public void StrFactorial()
		{
			RunTest("Fragments/factorial.asm", "Structure/StrFactorial.txt");
		}

		[Test]
		public void StrWhileBreak()
		{
			RunTest("Fragments/while_break.asm", "Structure/StrWhileBreak.txt");
		}

		[Test]
		public void StrWhileRepeat()
		{
			RunTest("Fragments/while_repeat.asm", "Structure/StrWhileRepeat.txt");
		}

		[Test]
		public void StrForkInLoop()
		{
			RunTest("Fragments/forkedloop.asm", "Structure/StrForkInLoop.txt");
		}

		[Test]
		public void StrNestedIf()
		{
			RunTest("Fragments/nested_ifs.asm", "Structure/StrNestedIf.txt");
		}

		[Test]
		public void StrNestedLoops()
		{
			RunTest("Fragments/matrix_addition.asm", "Structure/StrNestedLoop.txt");
		}

		[Test]
		public void StrReg00006()
		{
			RunTest("Fragments/regressions/r00006.asm", "Structure/StrReg00006.txt", new Address(0x100048B0));
		}


		[Test]
		[Ignore("Not quite ready yet")]
		public void StrNonreducible()
		{
			RunTest("Fragments/nonreducible.asm", "Structure/StrNonreducible.txt");
		}

		[Test]
		public void StrWhileGoto()
		{
			RunTest("Fragments/while_goto.asm", "Structure/StrWhileGoto.txt");
		}

		[Test]
		public void StrIfElseIf()
		{
			RunTest(new MockIfElseIf(), "Structure/StrIfElseIf.txt");
		}

		[Test]
		public void StrReg00011()
		{
			RunTest(new Reg00011Mock(), "Structure/StrReg00011.txt");
		}

		private void RunTest(string sourceFilename, string outFilename)
		{
			RunTest(sourceFilename, outFilename, new Address(0xC00, 0));
		}
		
		private void RunTest(ProcedureMock mock, string outFilename)
		{
			Procedure proc = mock.Procedure;
			using (FileUnitTester fut = new FileUnitTester(outFilename))
			{
				ControlFlowGraphCleaner cfgc = new ControlFlowGraphCleaner(proc);
				cfgc.Transform();
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();

				StructureAnalysis sa = new StructureAnalysis(proc);
				sa.FindStructures();
				CodeFormatter fmt = new CodeFormatter(fut.TextWriter);
				fmt.Write(proc);
				fut.TextWriter.WriteLine("===========================");

				fut.AssertFilesEqual();
			}
		}

		private void RunTest(string sourceFilename, string outFilename, Address addrBase)
		{
			using (FileUnitTester fut = new FileUnitTester(outFilename))
			{
				RewriteProgram(sourceFilename, addrBase);
				foreach (Procedure proc in prog.Procedures.Values)
				{
					ControlFlowGraphCleaner cfgc = new ControlFlowGraphCleaner(prog.Procedures[0]);
					cfgc.Transform();
					proc.Write(false, fut.TextWriter);
					fut.TextWriter.WriteLine();

					StructureAnalysis sa = new StructureAnalysis(proc);
					sa.FindStructures();
					CodeFormatter fmt = new CodeFormatter(fut.TextWriter);
					fmt.Write(proc);
					fut.TextWriter.WriteLine("===========================");
				}
				fut.AssertFilesEqual();
			}
		}
	}
}
