#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
	public class WebBuilderTests : AnalysisTestBase
	{
		[Test]
		public void WebNestedRepeats()
		{
			RunFileTest_x86_real("Fragments/nested_repeats.asm", "Analysis/WebNestedRepeats.txt");
		}

		[Test]
		public void WebWhileLoop()
		{
			RunFileTest_x86_real("Fragments/while_loop.asm", "Analysis/WebWhileLoop.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void WebGlobalHandle()
        {
            Given_FakeWin32Platform();
            this.platformMock.Setup(p => p.ResolveImportByName(It.IsAny<string>(), It.IsAny<string>())).Returns((Expression) null);
            this.platformMock.Setup(p => p.DataTypeFromImportName(It.IsAny<string>()))
                .Returns((Tuple<string, SerializedType, SerializedType>) null);
            this.platformMock.Setup(p => p.ResolveIndirectCall(It.IsAny<RtlCall>())).Returns((Address) null);

			RunFileTest_x86_32("Fragments/import32/GlobalHandle.asm", "Analysis/WebGlobalHandle.txt");
		}

		[Test]
        public void WebSuccessiveDecs()
		{
			RunFileTest_x86_real("Fragments/multiple/successivedecs.asm", "Analysis/WebSuccessiveDecs.txt");
		}

		private void Build(Program program)
		{
            var eventListener = new FakeDecompilerEventListener();
            var dfa = new DataFlowAnalysis(program, null, eventListener);
			var ssts = dfa.UntangleProcedures();
			foreach (Procedure proc in program.Procedures.Values)
			{
                var sst = ssts.Single(s => s.SsaState.Procedure == proc);
				var ssa = sst.SsaState;

				Coalescer coa = new Coalescer(ssa);
				coa.Transform();

				DeadCode.Eliminate(ssa);

				LiveCopyInserter lci = new LiveCopyInserter(ssa);
				lci.Transform();

				WebBuilder web = new WebBuilder(program, ssa, new Dictionary<Identifier,LinearInductionVariable>(), eventListener);
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
