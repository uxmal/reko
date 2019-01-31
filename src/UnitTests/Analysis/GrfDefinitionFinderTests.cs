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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.IO;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
	public class GrfDefinitionFinderTests : AnalysisTestBase
	{
		[Test]
		public void GrfdAdcMock()
		{
			RunFileTest(new AdcMock(), "Analysis/GrfdAdcMock.txt");
		}

		[Test]
		public void GrfdAddSubCarries()
		{
			RunFileTest("Fragments/addsubcarries.asm", "Analysis/GrfdAddSubCarries.txt");
		}

		[Test]
		public void GrfdCmpMock()
		{
			RunFileTest(new CmpMock(), "Analysis/GrfdCmpMock.txt");
		}

		protected override void RunTest(Program program, TextWriter writer)
		{
            var importResolver = new Mock<IImportResolver>().Object;
            var dfa = new DataFlowAnalysis(program, importResolver, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in program.Procedures.Values)
			{
				Aliases alias = new Aliases(proc);
				alias.Transform();
				SsaTransform sst = new SsaTransform(
                    dfa.ProgramDataFlow,
                    proc,
                    importResolver,
                    proc.CreateBlockDominatorGraph(),
                    new HashSet<RegisterStorage>());
				SsaState ssa = sst.SsaState;
				GrfDefinitionFinder grfd = new GrfDefinitionFinder(ssa.Identifiers);
				foreach (SsaIdentifier sid in ssa.Identifiers)
				{
                    var id = sid.OriginalIdentifier as Identifier;
					if (id == null || !(id.Storage is FlagGroupStorage) || sid.Uses.Count == 0)
						continue;
					writer.Write("{0}: ", sid.DefStatement.Instruction);
					grfd.FindDefiningExpression(sid);
					string fmt = grfd.IsNegated ? "!{0};" : "{0}";
					writer.WriteLine(fmt, grfd.DefiningExpression);
				}
			}
		}
	}
}
