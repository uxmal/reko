#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Analysis;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.IO;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
	public class GrfDefinitionFinderTests : AnalysisTestBase
	{
        protected override void RunTest(Program program, TextWriter writer)
        {
            var dynamicLinker = new Mock<IDynamicLinker>();
            var flow = new ProgramDataFlow(program);
            var sst = new SsaTransform(
                program, 
                program.Procedures.Values[0],
                new HashSet<Procedure>(),
                dynamicLinker.Object,
                flow);
            var ssa = sst.Transform();
            var grfd = new GrfDefinitionFinder(ssa.Identifiers);
            foreach (SsaIdentifier sid in ssa.Identifiers)
            {
                var id = sid.OriginalIdentifier;
                if (id is null || !(id.Storage is FlagGroupStorage) || sid.Uses.Count == 0)
                    continue;
                writer.Write("{0}: ", sid.DefStatement.Instruction);
                grfd.FindDefiningExpression(sid);
                string fmt = grfd.IsNegated ? "!{0};" : "{0}";
                writer.WriteLine(fmt, grfd.DefiningExpression);
            }
        }

		[Test]
        [Category(Categories.IntegrationTests)]
        public void GrfdAdcMock()
		{
			RunFileTest(new AdcMock(), "Analysis/GrfdAdcMock.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void GrfdAddSubCarries()
		{
			RunFileTest_x86_real("Fragments/addsubcarries.asm", "Analysis/GrfdAddSubCarries.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void GrfdCmpMock()
		{
			RunFileTest(new CmpMock(), "Analysis/GrfdCmpMock.txt");
		}
	}
}
