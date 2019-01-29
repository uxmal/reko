#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using Reko.Core.Rtl;
using Reko.Core.Serialization;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class WebBuilderTests : AnalysisTestBase
	{
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
            Given_FakeWin32Platform();
            this.platformMock.Setup(p => p.ResolveImportByName(It.IsAny<string>(), It.IsAny<string>())).Returns((Expression) null);
            this.platformMock.Setup(p => p.DataTypeFromImportName(It.IsAny<string>())).Returns((Tuple<string, SerializedType, SerializedType>) null);
            this.platformMock.Setup(p => p.ResolveIndirectCall(It.IsAny<RtlCall>())).Returns((Address) null);

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

				var vp = new ValuePropagator(program.SegmentMap, ssa, null, eventListener);
				vp.Transform();

				DeadCode.Eliminate(proc, ssa);

				Coalescer coa = new Coalescer(proc, ssa);
				coa.Transform();

				DeadCode.Eliminate(proc, ssa);

				LiveCopyInserter lci = new LiveCopyInserter(proc, ssa.Identifiers);
				lci.Transform();

				WebBuilder web = new WebBuilder(program, proc, ssa.Identifiers, new Dictionary<Identifier,LinearInductionVariable>(), eventListener);
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
