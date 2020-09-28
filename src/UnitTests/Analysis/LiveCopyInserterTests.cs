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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.UnitTests.Mocks;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class LiveCopyInserterTests : AnalysisTestBase
	{
		private Procedure proc;
        private SsaState ssa;
		private SsaIdentifierCollection ssaIds;

		[Test]
		public void LciFindCopyslots()
		{
			Build(new LiveLoopMock().Procedure, new FakeArchitecture());
			var lci = new LiveCopyInserter(ssa);
			Assert.AreEqual(2, lci.IndexOfInsertedCopy(proc.ControlGraph.Blocks[0]));
            Assert.AreEqual(0, lci.IndexOfInsertedCopy(proc.ControlGraph.Blocks[2].Succ[0]));
            Assert.AreEqual(0, lci.IndexOfInsertedCopy(proc.ControlGraph.Blocks[2].Succ[0].Succ[0]));
		}

		[Test]
		public void LciLiveAtLoop()
		{
			Build(new LiveLoopMock().Procedure, new FakeArchitecture());
			var lci = new LiveCopyInserter(ssa);

            ssa.Dump(true);
            var i = ssaIds.Where(s => s.Identifier.Name == "i").Single().Identifier;
			var i_3 = ssaIds.Where(s => s.Identifier.Name == "i_3").Single().Identifier;
            var loopHdr = proc.ControlGraph.Blocks[2];
			Assert.IsFalse(lci.IsLiveAtCopyPoint(i, loopHdr));
            Assert.IsTrue(lci.IsLiveAtCopyPoint(i_3, loopHdr), "i_3 should be live");
		}

		[Test]
		public void LciLiveAtCopy()
		{
			Build(new LiveCopyMock().Procedure, new FakeArchitecture());
			var lci = new LiveCopyInserter(ssa);

			var reg   = ssaIds.Where(s => s.Identifier.Name == "reg").Single();
			var reg_3 = ssaIds.Where(s => s.Identifier.Name == "reg_3").Single();
            var reg_4 = ssaIds.Where(s => s.Identifier.Name == "reg_4").Single();

            ssa.Dump(true);
			Assert.AreEqual("reg_4 = PHI((reg, l1), (reg_3, l2))", reg_4.DefStatement.Instruction.ToString());
			Assert.IsTrue(lci.IsLiveOut(reg.Identifier, reg_4.DefStatement));
		}

		[Test]
		public void LciInsertAssignmentCopy()
		{
			Build(new LiveCopyMock().Procedure, new FakeArchitecture());
			var lci = new LiveCopyInserter(ssa);

			int i = lci.IndexOfInsertedCopy(proc.ControlGraph.Blocks[2]);
			Assert.AreEqual(i, 0);
            var idNew = lci.InsertAssignmentNewId(ssaIds.Where(s => s.Identifier.Name == "reg").Single().Identifier, proc.ControlGraph.Blocks[2], i);
            Assert.AreEqual("reg_6 = reg", proc.ControlGraph.Blocks[2].Statements[0].Instruction.ToString());
            Assert.AreSame(proc.ControlGraph.Blocks[2].Statements[0], ssaIds[idNew].DefStatement);
		}

		[Test]
		public void LciInsertAssignmentLiveLoop()
		{
			Build(new LiveLoopMock().Procedure, new FakeArchitecture());
			var lci = new LiveCopyInserter(ssa);

            var i_3 = ssaIds.Where(s => s.Identifier.Name == "i_3").Single();
			var idNew = lci.InsertAssignmentNewId(i_3.Identifier, proc.ControlGraph.Blocks[2], 2);
			Assert.AreEqual("i_7 = i_3", proc.ControlGraph.Blocks[2].Statements[2].Instruction.ToString());
			Assert.AreSame(proc.ControlGraph.Blocks[2].Statements[2], ssaIds[idNew].DefStatement);
		}

		[Test]
		public void LciRenameDominatedIdentifiers()
		{
			Build(new LiveLoopMock().Procedure, new FakeArchitecture());
			var lci = new LiveCopyInserter(ssa);

            var i_3 = ssaIds.Where(s => s.Identifier.Name == "i_3").Single();
            var idNew = lci.InsertAssignmentNewId(i_3.Identifier, proc.ControlGraph.Blocks[2], 2);
			lci.RenameDominatedIdentifiers(i_3, ssaIds[idNew]);
            Assert.AreEqual("return i_1", proc.ControlGraph.Blocks[2].ElseBlock.Statements[0].Instruction.ToString());
		}

		[Test]
		public void LciLiveLoop()
		{
			Build(new LiveLoopMock().Procedure, new FakeArchitecture());
			LiveCopyInserter lci = new LiveCopyInserter(ssa);
			lci.Transform();
			using (FileUnitTester fut = new FileUnitTester("Analysis/LciLiveLoop.txt"))
			{
				proc.Write(false, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void LciLiveCopy()
		{
			Build(new LiveCopyMock().Procedure, new FakeArchitecture());
			LiveCopyInserter lci = new LiveCopyInserter(ssa);
			lci.Transform();
			using (FileUnitTester fut = new FileUnitTester("Analysis/LciLiveCopy.txt"))
			{
				proc.Write(false, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void LciNestedRepeats()
		{
			RunTest("Fragments/nested_repeats.asm", "Analysis/LciNestedRepeats.txt");
		}

		[Test]
		public void LciWhileGoto()
		{
			RunTest("Fragments/while_goto.asm", "Analysis/LciWhileGoto.txt");
		}

		[Test]
		public void LciWhileLoop()
		{
			RunTest("Fragments/while_loop.asm", "Analysis/LciWhileLoop.txt");
		}

		protected void RunTest(string sourceFile, string outputFile)
		{
			Program program = RewriteFile(sourceFile);
			Build(program.Procedures.Values[0], program.Architecture);
			LiveCopyInserter lci = new LiveCopyInserter(ssa);
			lci.Transform();
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				proc.Write(false, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		private void Build(Procedure proc, IProcessorArchitecture arch)
		{
            var platform = new DefaultPlatform(null, arch);
            var program = new Program()
            {
                Architecture = arch,
                Platform = platform,
            };
            this.proc = proc;
            var dynamicLinker = new Mock<IDynamicLinker>().Object;
			var gr = proc.CreateBlockDominatorGraph();
            SsaTransform sst = new SsaTransform(
                program,
                proc,
                new HashSet<Procedure>(),
                null,
                new ProgramDataFlow());
            sst.Transform();
            this.ssa = sst.SsaState;
			this.ssaIds = ssa.Identifiers;

			ConditionCodeEliminator cce = new ConditionCodeEliminator(ssa, platform);
			cce.Transform();
			DeadCode.Eliminate(ssa);

            var segmentMap = new SegmentMap(Address.Ptr32(0x00400000));
			ValuePropagator vp = new ValuePropagator(
                segmentMap, 
                ssa,
                program.CallGraph, 
                null, 
                new FakeDecompilerEventListener());
			vp.Transform();

			Coalescer coa = new Coalescer(ssa);
			coa.Transform();

			DeadCode.Eliminate(ssa);
		}
	}
}
