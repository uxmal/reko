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
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class WebBuilderTests : AnalysisTestBase
	{
        private MockRepository mr;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

		[Test]
		public void WebNestedRepeats()
		{
			RunFileTest("Fragments/nested_repeats.asm", "Analysis/WebNestedRepeats.txt");
		}

		[Test]
		public void WebWhileLoop()
		{
			RunFileTest("Fragments/while_loop.asm", "Analysis/WebWhileLoop.txt");
		}

		[Test]
		public void WebGlobalHandle()
        {
            Given_FakeWin32Platform(mr);
            this.platform.Stub(p => p.ResolveImportByName(null, null)).IgnoreArguments().Return(null);
            this.platform.Stub(p => p.DataTypeFromImportName(null)).IgnoreArguments().Return(null);
            this.platform.Stub(p => p.ResolveIndirectCall(null)).IgnoreArguments().Return(null);
            mr.ReplayAll();

			RunFileTest32("Fragments/import32/GlobalHandle.asm", "Analysis/WebGlobalHandle.txt");
		}

		[Test]
		public void WebSuccessiveDecs()
		{
			RunFileTest("Fragments/multiple/successivedecs.asm", "Analysis/WebSuccessiveDecs.txt");
		}

		private void Build(Program program)
		{
            var eventListener = new FakeDecompilerEventListener();
            DataFlowAnalysis dfa = new DataFlowAnalysis(program, null, eventListener);
			dfa.UntangleProcedures();
			foreach (Procedure proc in program.Procedures.Values)
			{
                Aliases alias = new Aliases(proc);
				alias.Transform();
				var gr = proc.CreateBlockDominatorGraph();
				SsaTransform sst = new SsaTransform(dfa.ProgramDataFlow, proc, null, gr, new HashSet<RegisterStorage>());
				SsaState ssa = sst.SsaState;

				ConditionCodeEliminator cce = new ConditionCodeEliminator(ssa, program.Platform);
				cce.Transform();

				DeadCode.Eliminate(proc, ssa);

				var vp = new ValuePropagator(program.SegmentMap, ssa, eventListener);
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

		protected override void RunTest(Program program, TextWriter writer)
		{
			Build(program);
			foreach (Procedure proc in program.Procedures.Values)
			{
				proc.Write(false, writer);
				writer.WriteLine();
			}
		}	
	}
}
