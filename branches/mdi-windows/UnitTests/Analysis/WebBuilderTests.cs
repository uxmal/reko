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
using Decompiler.Core.Code;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class WebBuilderTests : AnalysisTestBase
	{
		[Test]
		public void WebAddSubCarries()
		{
			RunTest("Fragments/addsubcarries.asm", "Analysis/WebAddSubCarries.txt");
		}

		[Test]
		public void WebNestedRepeats()
		{
			RunTest("fragments/nested_repeats.asm", "Analysis/WebNestedRepeats.txt");
		}

		[Test]
		public void WebWhileLoop()
		{
			RunTest("fragments/while_loop.asm", "Analysis/WebWhileLoop.txt");
		}

		[Test]
		public void WebGlobalHandle()
		{
			RunTest32("Fragments/import32/globalhandle.asm", "Analysis/WebGlobalHandle.txt");
		}

		[Test]
		public void WebSuccessiveDecs()
		{
			RunTest("Fragments/multiple/successivedecs.asm", "Analysis/WebSuccessiveDecs.txt");
		}

		private void Build(Program prog)
		{
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in prog.Procedures.Values)
			{
				Aliases alias = new Aliases(proc, prog.Architecture);
				alias.Transform();
				DominatorGraph gr = new DominatorGraph(proc);
				SsaTransform sst = new SsaTransform(proc, gr, false);
				SsaState ssa = sst.SsaState;

				ConditionCodeEliminator cce = new ConditionCodeEliminator(ssa.Identifiers, prog.Architecture);
				cce.Transform();

				DeadCode.Eliminate(proc, ssa);

				ValuePropagator vp = new ValuePropagator(ssa.Identifiers, proc);
				vp.Transform();

				DeadCode.Eliminate(proc, ssa);

				Coalescer coa = new Coalescer(proc, ssa);
				coa.Transform();

				DeadCode.Eliminate(proc, ssa);

				LiveCopyInserter lci = new LiveCopyInserter(proc, ssa.Identifiers);
				lci.Transform();

				WebBuilder web = new WebBuilder(proc, ssa.Identifiers, new Dictionary<Identifier,LinearInductionVariable>());
				web.Transform();

				ssa.ConvertBack(false);
			}

		}

		protected override void RunTest(Program prog, FileUnitTester fut)
		{
			Build(prog);
			foreach (Procedure proc in prog.Procedures.Values)
			{
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();
			}
		}	
	}
}
