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

using Decompiler;
using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

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
			RunTest(new SsaMock(), "Analysis/SsaMockTest.txt");
		}

        [Test]
        public void SsaOutParamters()
        {
            ProcedureMock m = new ProcedureMock("foo");
            Identifier r4 = m.Register(4);
            m.Store(m.Int32(0x400), m.Fn("foo", m.AddrOf(r4)));
            m.Return();

            RunTest(m, "Analysis/SsaOutParameters.txt");
        }

		protected override void RunTest(Program prog, FileUnitTester fut)
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
				Aliases alias = new Aliases(proc, prog.Architecture);
				alias.Transform();
				var gr = proc.CreateBlockDominatorGraph();
				SsaTransform sst = new SsaTransform(proc, gr, false);
				ssa = sst.SsaState;
				ssa.Write(fut.TextWriter);
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();
			}
		}

		private class SsaMock : ProcedureMock
		{
			protected override void BuildBody()
			{
				Identifier r0 = Register(0);
				Identifier r1 = Register(1);

				Assign(r0, Int32(0));
			
				Label("top");
				Compare("Z", r0, Int32(2));
				Branch(ConditionCode.NE, "skip");
				Assign(r0, Int32(0));
				
				Label("skip");
				Compare("Z", r1, Int32(3));
				Branch(ConditionCode.NE, "top");
				Return();
			}
		}
	}
}
