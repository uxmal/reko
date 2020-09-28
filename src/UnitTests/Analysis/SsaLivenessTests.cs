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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Analysis;
using Reko.UnitTests.Mocks;
using Reko.UnitTests.TestCode;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class SsaLivenessTests : AnalysisTestBase
	{
		private Procedure proc;
		private SsaState ssa;
		private SsaLivenessAnalysis sla;
		private SsaLivenessAnalysis2 sla2;

		[Test]
		public void SltLiveLoop()
		{
			Build(new LiveLoopMock().Procedure, new FakeArchitecture());

			using (FileUnitTester fut = new FileUnitTester("Analysis/SltLiveLoop.txt"))
			{
				sla.Write(proc, fut.TextWriter);
				fut.TextWriter.WriteLine("=======================");
                fut.AssertFilesEqual();
			}
            SsaIdentifier i = ssa.Identifiers.Where(s => s.Identifier.Name == "i").Single();
            SsaIdentifier i_1 = ssa.Identifiers.Where(s => s.Identifier.Name == "i_1").Single();
            SsaIdentifier i_3 = ssa.Identifiers.Where(s => s.Identifier.Name == "i_3").Single();
			Assert.IsFalse(sla.IsLiveOut(i.Identifier, i_1.DefStatement));
            var block1 = proc.ControlGraph.Blocks.Where(b => b.Name =="loop").Single();
			Assert.AreEqual("branch Mem0[i_3:byte] != 0 loop", block1.Statements[2].Instruction.ToString());
			Assert.IsTrue(sla.IsLiveOut(i_1.Identifier, block1.Statements[2]), "i_1 should be live at the end of block 1");
			Assert.IsTrue(sla.IsLiveOut(i_3.Identifier, block1.Statements[2]),"i_3 should be live at the end of block 1");
			Assert.AreEqual("i_1 = PHI((i, LiveLoopMock_entry), (i_3, loop))", block1.Statements[0].Instruction.ToString());
			Assert.IsFalse(sla.IsLiveOut(i_3.Identifier, block1.Statements[0]), "i_3 is dead after the phi function");
		}

		[Test]
		public void SltSimple()
		{
			Build(new SimpleMock().Procedure, new FakeArchitecture());

			using (FileUnitTester fut = new FileUnitTester("Analysis/SltSimple.txt"))
			{
				ssa.Write(fut.TextWriter);
				sla.Write(proc, fut.TextWriter);
				fut.TextWriter.WriteLine();
				fut.AssertFilesEqual();
			}

			Block block = proc.EntryBlock.Succ[0];
			Assert.AreEqual("Mem4[0x10000000:word32] = a + b", block.Statements[0].Instruction.ToString());
			Assert.AreEqual("Mem5[0x10000004:word32] = a", block.Statements[1].Instruction.ToString());

			SsaIdentifier a = ssa.Identifiers.Where(s=>s.Identifier.Name=="a").Single();
            SsaIdentifier b = ssa.Identifiers.Where(s => s.Identifier.Name == "b").Single();
            SsaIdentifier c_3 = ssa.Identifiers.Where(s => s.Identifier.Name == "c_3").Single();
			Assert.IsFalse(sla.IsLiveOut(a.Identifier, block.Statements[1]), "a should be dead after its last use");
			Assert.IsTrue(sla.IsLiveOut(a.Identifier, block.Statements[0]), "a should be live after the first use");
			Assert.IsFalse(sla.IsDefinedAtStatement(c_3, block.Statements[0]));
			Assert.IsFalse(sla.IsDefinedAtStatement(b, block.Statements[0]));
		}

		[Test]
		public void SltWhileGoto()
		{
			Program program = RewriteFile("Fragments/while_goto.asm");
			Build(program.Procedures.Values[0], program.Architecture);

			using (FileUnitTester fut = new FileUnitTester("Analysis/SltWhileGoto.txt"))
			{
				ssa.Write(fut.TextWriter);
				sla.Write(proc, fut.TextWriter);
				fut.TextWriter.WriteLine();
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void SltManyIncrements()
		{
			Build(new ManyIncrements().Procedure, new FakeArchitecture());
			using (FileUnitTester fut = new FileUnitTester("Analysis/SltManyIncrements.txt"))
			{
				ssa.Write(fut.TextWriter);
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine("- Interference graph -------------------");
				sla2.InterferenceGraph.Write(fut.TextWriter);
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
            var sst = new SsaTransform(
                program,
                proc,
                new HashSet<Procedure>(),
                dynamicLinker,
                new ProgramDataFlow());
            sst.Transform();
			ssa = sst.SsaState;
			ConditionCodeEliminator cce = new ConditionCodeEliminator(ssa, platform);
			cce.Transform();
            var segmentMap = new SegmentMap(Address.Ptr32(0x00123400));
			ValuePropagator vp = new ValuePropagator(segmentMap, ssa, program.CallGraph, dynamicLinker, new FakeDecompilerEventListener());
			vp.Transform();
			DeadCode.Eliminate(ssa);
			Coalescer coa = new Coalescer(ssa);
			coa.Transform();
			DeadCode.Eliminate(ssa);

			sla = new SsaLivenessAnalysis(ssa);
			sla2 = new SsaLivenessAnalysis2(ssa);
			sla2.Analyze();
		}

		public class SimpleMock : ProcedureBuilder
		{
			protected override void BuildBody()
			{
				Identifier a  = Local32("a");
				Identifier b  = Local32("b");
				Identifier c  = Local32("c");

				Assign(c, IAdd(a, b));
				MStore(Word32(0x10000000), c);
				MStore(Word32(0x10000004), a);
			}
		}
	}
}
