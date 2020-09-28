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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class LinearInductionVariableTests : AnalysisTestBase
	{
		private SsaState ssa;
		private BlockDominatorGraph doms;
        private BlockDominatorGraph dom;

		/// <summary>
		/// Builds a strongly connected component corresponding to:
		/// a1 = 0
		/// a2 = phi(a1, a3)
		/// while (a2 != 10)
		/// {
		///    a3 = a2 + 4
		/// }
		/// </summary>
		private List<SsaIdentifier> BuildScc()
		{
            var m = new ProcedureBuilder("test");
			Identifier a = new Identifier("a", PrimitiveType.Word32, new RegisterStorage("a", 1, 0, PrimitiveType.Word32));
            m.Label("b1");
            m.Assign(a, Constant.Word32(0));
            m.Label("b2");
            m.Assign(a, m.IAdd(a, 4));
            m.BranchIf(m.Ne(a, 10), "b2");
            m.Label("b3");
            m.Return();
            this.dom = m.Procedure.CreateBlockDominatorGraph();
            var program = new Program()
            {
                Architecture = m.Architecture,
            };
            var sst = new SsaTransform(
                program,
                m.Procedure,
                new HashSet<Procedure>(),
                null,
                new ProgramDataFlow());
            sst.Transform();
            /*
            
            proc = new Procedure("test", new Frame(PrimitiveType.Word32));
			Block b1 = proc.AddBlock("b1");
			Block b2 = proc.AddBlock("b2");

			Identifier a2 = new Identifier("a2", PrimitiveType.Word32, null);
			Identifier a3 = new Identifier("a3", PrimitiveType.Word32, null);
			PhiFunction phi = new PhiFunction(a1.DataType, new Expression [] { a1, a3 });

			Statement stm_a1 = new Statement(0, new Assignment(a1, Constant.Word32(0)), null);
			Statement stm_a2 = new Statement(0, new PhiAssignment(a2, new PhiFunction(a1.DataType,  a1, a3 )), null);
			Statement stm_ex = new Statement(0, new Branch(new BinaryExpression(Operator.Ne, PrimitiveType.Bool, a2, Constant.Word32(10)), b2), null);
			Statement stm_a3 = new Statement(0, new Assignment(a3, new BinaryExpression(Operator.IAdd, a3.DataType, a2, Constant.Word32(4))), null);
			b1.Statements.Add(stm_a1);

			b2.Statements.Add(stm_a2);
			b2.Statements.Add(stm_a3);

			SsaIdentifier sid_a1 = new SsaIdentifier(a1, a1, stm_a1, ((Assignment)stm_a1.Instruction).Src, false);
            SsaIdentifier sid_a2 = new SsaIdentifier(a2, a2, stm_a2, ((PhiAssignment) stm_a2.Instruction).Src, false);
            SsaIdentifier sid_a3 = new SsaIdentifier(a3, a3, stm_a3, ((Assignment) stm_a3.Instruction).Src, false);
			sid_a1.Uses.Add(stm_a2);
			ssaIds = new SsaIdentifierCollection();
			ssaIds.Add(a1, sid_a1);
			ssaIds.Add(a2, sid_a2);
			ssaIds.Add(a3, sid_a3);
            */
            ssa = sst.SsaState;

            List<SsaIdentifier> list = new List<SsaIdentifier> {
                ssa.Identifiers.Where(i => i.Identifier.Name == "a_1").Single(),
                ssa.Identifiers.Where(i => i.Identifier.Name == "a_2").Single(),
                ssa.Identifiers.Where(i => i.Identifier.Name == "a_3").Single(),
            };
			return list;
		}

		[Test]
		public void Liv_FindPhi()
		{
			List<SsaIdentifier> a = BuildScc();
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(null, dom);
			PhiFunction p = liv.FindPhiFunction(a);
			Assert.IsNotNull(p, "Didn't find phi function!");
		}

		[Test]
		public void Liv_FindLinearIncrement()
		{
            List<SsaIdentifier> a = BuildScc();
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(null, dom);
			Constant c = liv.FindLinearIncrement(a);
			Assert.AreEqual(4, c.ToInt32());
            Assert.AreEqual("a_3 = a_2 + 0x00000004", liv.Context.DeltaStatement.ToString());
		}

		[Test]
		public void Liv_FindInitialValue()
		{
			List<SsaIdentifier> a = BuildScc();
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(ssa, dom);
			PhiFunction phi = liv.FindPhiFunction(a);
			Constant c = liv.FindInitialValue(phi);
            Assert.AreEqual(0, c.ToInt32());
            Assert.AreEqual("a_1 = 0x00000000", liv.Context.InitialStatement.ToString());
		}

		[Test]
		public void Liv_FindFinalValue()
		{
			Prepare(new ByteArrayLoopMock().Procedure);
			var liv = new LinearInductionVariableFinder(ssa, null);
			var a = new List<SsaIdentifier>();
			a.Add(ssa.Identifiers.Where(s => s.Identifier.Name == "i_2").Single());
			a.Add(ssa.Identifiers.Where(s => s.Identifier.Name == "i_5").Single());
			Constant c = liv.FindFinalValue(a);
			Assert.AreEqual(10, c.ToInt32());
            Assert.AreEqual("branch i_2 < 0x0000000A body", liv.Context.TestStatement.ToString());
		}

		[Test]
		public void Liv_WhileLtIncMock()
		{
			RunTest(new WhileLtIncMock().Procedure, "Analysis/LivWhileLtInc.txt");
		}

		[Test]
		public void Liv_WhileGeDecMock()
		{
			RunTest(new WhileGtDecMock().Procedure, "Analysis/LivWhileGtDec.txt");
		}

        [Test]
        public void Liv_ArrayLoopMock()
        {
            RunTest(new ArrayLoopMock().Procedure, "Analysis/LivArrayLoopMock.txt");
        }

		public void Create1()
		{
			Prepare(new WhileLtIncMock().Procedure);
			var doms = ssa.Procedure.CreateBlockDominatorGraph();
			var liv = new LinearInductionVariableFinder(ssa, doms);
			Assert.IsNull(liv.Context.PhiIdentifier);
			Assert.IsNull(liv.Context.PhiStatement);
            Assert.Fail(); /*
			liv.Context.PhiStatement = ssaIds[5].DefStatement;
			liv.Context.PhiIdentifier = (Identifier) ((PhiAssignment) liv.Context.PhiStatement.Instruction).Dst;
			liv.Context.TestStatement = proc.ControlGraph.Blocks[2].Statements[1];
			liv.Context.DeltaStatement = ssaIds[8].DefStatement;
			liv.Context.InitialValue = Constant.Word32(0);
			liv.Context.DeltaValue = Constant.Word32(1);
			liv.Context.TestValue = Constant.Word32(10);
			LinearInductionVariable iv = liv.CreateInductionVariable();
			Assert.AreEqual("X", iv.ToString()); */
		}

		[Test]
		public void Liv_CreateNo()
		{
			var liv = new LinearInductionVariableFinder(null, null);
			Assert.IsNull(liv.CreateInductionVariable());
		}

		[Test]
		public void Liv_CreateBareMinimum()
		{
            var pb = new ProcedureBuilder();
            ssa = new SsaState(pb.Procedure);
            Identifier id0 = new Identifier("foo", PrimitiveType.Word32, new TemporaryStorage("foo", 1, PrimitiveType.Word32));
            Identifier id1 = new Identifier("bar", PrimitiveType.Word32, new TemporaryStorage("bar", 1, PrimitiveType.Word32));
            Identifier phi = new Identifier("i_3", PrimitiveType.Word32, null);
			ssa.Identifiers.Add(id0, new SsaIdentifier(id0, id0, null, null, false));
			ssa.Identifiers.Add(id1, new SsaIdentifier(id1, id1, null, null, false));
            ssa.Identifiers.Add(phi, new SsaIdentifier(phi, phi, null, null, false));
			var liv = new LinearInductionVariableFinder(ssa, null);
			liv.Context.PhiStatement = new Statement(0, null, null);
            liv.Context.PhiIdentifier = phi;
			liv.Context.DeltaValue = Constant.Word32(1);
			LinearInductionVariable iv = liv.CreateInductionVariable();
			Assert.AreEqual("(? 0x00000001 ?)", iv.ToString());
		}

		[Test]
		public void Liv_CreateIncInitialValue()
		{
            var pb = new ProcedureBuilder();
            ssa = new SsaState(pb.Procedure);
            LinearInductionVariableFinder liv = new LinearInductionVariableFinder(ssa, null);
			liv.Context.InitialValue = Constant.Word32(0);
			liv.Context.PhiStatement = new Statement(0, null, null);
			liv.Context.PhiIdentifier = new Identifier("foo_0", PrimitiveType.Word32, null);
            ssa.Identifiers.Add(liv.Context.PhiIdentifier, new SsaIdentifier(liv.Context.PhiIdentifier, liv.Context.PhiIdentifier, liv.Context.PhiStatement, null, false));
			liv.Context.DeltaValue = Constant.Word32(1);
			liv.Context.DeltaStatement = new Statement(0, new Assignment(new Identifier("foo_1", PrimitiveType.Word32, null), 
				new BinaryExpression(Operator.IAdd, PrimitiveType.Word32, liv.Context.PhiIdentifier, liv.Context.DeltaValue)), null);
			ssa.Identifiers[liv.Context.PhiIdentifier].Uses.Add(liv.Context.DeltaStatement);

			LinearInductionVariable iv = liv.CreateInductionVariable();
			Assert.AreEqual("(0x00000001 0x00000001 ?)", iv.ToString());
		}

		[Test]
		public void CreateNoincInitialValue()
		{
			ProcedureBuilder m = new ProcedureBuilder();
            ssa = new SsaState(m.Procedure);
			SsaId(new Identifier("id0", PrimitiveType.Word32, new TemporaryStorage("id0", 0, PrimitiveType.Word32)), null, null, false);
			SsaId(new Identifier("id1", PrimitiveType.Word32, new TemporaryStorage("id1", 1, PrimitiveType.Word32)), null, null, false);
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(ssa, null);

			liv.Context.InitialValue = Constant.Word32(0);
			Identifier id2 = m.Local32("id_2");
			SsaId(id2, new Statement(0, null, null), null, false);
			Assert.AreEqual(3, ssa.Identifiers.Count);

			Identifier id3 = m.Local32("id_3");
			Identifier id4 = m.Local32("id_4");
			liv.Context.PhiStatement = m.Phi(id3, (id2, "block2"), (id4, "block4"));
			liv.Context.PhiIdentifier = id3;
			SsaId(id3, liv.Context.PhiStatement, ((PhiAssignment)liv.Context.PhiStatement.Instruction).Src, false);
			Assert.AreEqual(4, ssa.Identifiers.Count);

			Statement use = new Statement(0, null, null);
			ssa.Identifiers[id3].Uses.Add(use);

			liv.Context.DeltaValue = m.Word32(1);
            m.Assign(id4, m.IAdd(id3, liv.Context.DeltaValue));
            liv.Context.DeltaStatement = m.Block.Statements.Last;
			ssa.Identifiers[id3].Uses.Add(liv.Context.DeltaStatement);

			LinearInductionVariable iv = liv.CreateInductionVariable();
			Assert.AreEqual("(0x00000000 0x00000001 ?)", iv.ToString());

		}

        [Test]
        public void Liv_PreTestedUge()
        {
            Prepare(m =>
            {
                Identifier i = m.Local32("i");
                m.Label("test");
                m.BranchIf(m.Uge(i, 10), "done");
                m.MStore(m.Word32(0x4204), i);
                m.Assign(i, m.IAdd(i, 1));
                m.Goto("test");
                m.Label("done");
                m.MStore(m.Word32(0x4200), i);
                m.Return();
            });
            var liv = new LinearInductionVariableFinder(ssa, doms);
            liv.Find();
            Assert.AreEqual("(? 0x00000001 0x0000000A)", liv.InductionVariables[0].ToString());
        }

		[Test]
		public void Liv_CreateDecTest()
		{
            Prepare(m =>
            {
                Identifier id = m.Local32("id");
                m.Assign(id, Constant.Word32(10));
                m.Label("loop");
                m.Assign(id, m.ISub(id, 1));
                m.BranchIf(m.Ge(id, 0), "loop");
                m.MStore(m.Word32(0x4232), id);
                m.Return(id);
            });
            var liv = new LinearInductionVariableFinder(ssa, doms);
            liv.Find();
			var iv = liv.InductionVariables[0];
			Assert.AreEqual("(0x00000009 -1 0xFFFFFFFF signed)", iv.ToString());
		}

		[Test]
		public void Liv_Commensurate()
		{
			var liv1 = new LinearInductionVariable(null, Constant.Word32(1), null, false);
            var liv2 = new LinearInductionVariable(null, Constant.Word32(2), null, false);
			var liv = LinearInductionVariable.Merge(liv1, liv2);
			Assert.IsNotNull(liv);
			Assert.AreEqual(1, liv.Delta.ToInt32());
		}

		[Test]
		public void Commensurate2()
		{
            LinearInductionVariable liv1 = new LinearInductionVariable(null, Constant.Word32(2), null, false);
            LinearInductionVariable liv2 = new LinearInductionVariable(null, Constant.Word32(8), null, false);
			LinearInductionVariable liv =
				LinearInductionVariable.Merge(liv1, liv2);
			Assert.IsNotNull(liv);
			Assert.AreEqual(2, liv.Delta.ToInt32());
		}

		[Test]
		public void InCommensurate()
		{
			LinearInductionVariable liv1 = new LinearInductionVariable(null, Constant.Word32(3), null, false);
			LinearInductionVariable liv2 = new LinearInductionVariable(null, Constant.Word32(8), null, false);
			LinearInductionVariable liv =
				LinearInductionVariable.Merge(liv1, liv2);
			Assert.IsNull(liv);
		}

		private void Prepare(Procedure proc)
		{
            var listener = new FakeDecompilerEventListener();
            var dynamicLinker = new Mock<IDynamicLinker>().Object;
            doms = proc.CreateBlockDominatorGraph();
            SsaTransform sst = new SsaTransform(
                new Program(),
                proc,
                new HashSet<Procedure>(),
                dynamicLinker,
                new ProgramDataFlow());
            sst.Transform();
			this.ssa = sst.SsaState;

            var arch = new FakeArchitecture();
            var cce = new ConditionCodeEliminator(ssa, new DefaultPlatform(null, arch));
			cce.Transform();

			DeadCode.Eliminate(ssa);

            var segmentMap = new SegmentMap(Address.Ptr32(0x00123400));
			var vp = new ValuePropagator(segmentMap, ssa, new CallGraph(), dynamicLinker, listener);
			vp.Transform();

			DeadCode.Eliminate(ssa);
		}

        private void Prepare(Action<ProcedureBuilder> m)
        {
            ProcedureBuilder mock = new ProcedureBuilder();
            m(mock);
            Prepare(mock.Procedure);
        }

		private void RunTest(Procedure proc, string outputFile)
		{
			Prepare(proc);
			var liv = new LinearInductionVariableFinder(ssa, doms);
			liv.Find();
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();
				foreach (LinearInductionVariable iv in liv.InductionVariables)
				{
					fut.TextWriter.WriteLine(iv);
				}
				fut.AssertFilesEqual();
			}
		}

		private void SsaId(Identifier id, Statement stm, Expression expr, bool isSideEffect)
		{
			ssa.Identifiers.Add(id, new SsaIdentifier(id, id, stm, expr, isSideEffect));
		}
	}
}
