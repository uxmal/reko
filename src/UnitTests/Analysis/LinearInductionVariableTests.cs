/* 
 * Copyright (C) 1999-2009 John Källén.
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
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class LinearInductionVariableTests : AnalysisTestBase
	{
		private Procedure proc;
		private SsaIdentifierCollection ssaIds;
		private DominatorGraph doms;

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
			Block b1 = new Block(null, "b1");
			b1.RpoNumber = 1;
			Block b2 = new Block(null, "b2");
			b2.RpoNumber = 2;

			Identifier a1 = new Identifier("a1", 0, PrimitiveType.Word32, null);
			Identifier a2 = new Identifier("a2", 1, PrimitiveType.Word32, null);
			Identifier a3 = new Identifier("a3", 2, PrimitiveType.Word32, null);
			PhiFunction phi = new PhiFunction(a1.DataType, new Expression [] { a1, a3 });

			Statement stm_a1 = new Statement(new Assignment(a1, Constant.Word32(0)), null);
			Statement stm_a2 = new Statement(new PhiAssignment(a2, new PhiFunction(a1.DataType, new Expression[] { a1, a3 } )), null);
			Statement stm_ex = new Statement(new Branch(new BinaryExpression(Operator.ne, PrimitiveType.Bool, a2, Constant.Word32(10))), null);
			Statement stm_a3 = new Statement(new Assignment(a3, new BinaryExpression(Operator.add, a3.DataType, a2, Constant.Word32(4))), null);
			b1.Statements.Add(stm_a1);
			b2.Statements.Add(stm_a2);
			b2.Statements.Add(stm_a3);

			SsaIdentifier sid_a1 = new SsaIdentifier(a1, a1, stm_a1, ((Assignment)stm_a1.Instruction).Src, false);
            SsaIdentifier sid_a2 = new SsaIdentifier(a2, a2, stm_a2, ((PhiAssignment) stm_a2.Instruction).Src, false);
            SsaIdentifier sid_a3 = new SsaIdentifier(a3, a3, stm_a3, ((Assignment) stm_a3.Instruction).Src, false);
			sid_a1.Uses.Add(stm_a2);
			ssaIds = new SsaIdentifierCollection();
			ssaIds.Add(sid_a1);
			ssaIds.Add(sid_a2);
			ssaIds.Add(sid_a3);

            List<SsaIdentifier> list = new List<SsaIdentifier>();
			list.Add(ssaIds[0]);
			list.Add(ssaIds[1]);
			list.Add(ssaIds[2]);
			return list;
		}

		[Test]
		public void FindPhi()
		{
			List<SsaIdentifier> a = BuildScc();
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(null, null, null);
			PhiFunction p = liv.FindPhiFunction(a);
			Assert.IsNotNull(p, "Didn't find phi function!");
		}

		[Test]
		public void FindLinearIncrement()
		{
			List<SsaIdentifier> a = BuildScc();
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(null, null, null);
			Constant c = liv.FindLinearIncrement(a);
			Assert.AreEqual(4, c.ToInt32());
		}

		[Test]
		public void FindInitialValue()
		{
			List<SsaIdentifier> a = BuildScc();
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(null, ssaIds, null);
			PhiFunction phi = liv.FindPhiFunction(a);
			Constant c = liv.FindInitialValue(phi);
            Assert.AreEqual(0, c.ToInt32());
		}

		[Test]
		public void FindFinalValue()
		{
			Prepare(new ByteArrayLoopMock().Procedure);
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(proc, ssaIds, null);
			List<SsaIdentifier> a = new List<SsaIdentifier>();
			a.Add(ssaIds[5]);
			a.Add(ssaIds[8]);
			Constant c = liv.FindFinalValue(a);
			Assert.AreEqual(10, c.ToInt32());
		}

		[Test]
		public void WhileLtIncMock()
		{
			RunTest(new WhileLtIncMock().Procedure, "Analysis/LivWhileLtInc.txt");
		}

		[Test]
		public void WhileGeDecMock()
		{
			RunTest(new WhileGtDecMock().Procedure, "Analysis/LivWhileGtDec.txt");
		}

        [Test]
        public void ArrayLoopMock()
        {
            RunTest(new ArrayLoopMock().Procedure, "Analysis/LivArrayLoopMock.txt");
        }

		public void Create1()
		{
			Prepare(new WhileLtIncMock().Procedure);
			DominatorGraph doms = new DominatorGraph(proc);
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(proc, ssaIds, doms);
			Assert.IsNull(liv.PhiIdentifier);
			Assert.IsNull(liv.PhiStatement);

			liv.PhiStatement = ssaIds[5].DefStatement;
			liv.PhiIdentifier = (Identifier) ((PhiAssignment) liv.PhiStatement.Instruction).Dst;
			liv.TestStatement = proc.RpoBlocks[2].Statements[1];
			liv.DeltaStatement = ssaIds[8].DefStatement;
			liv.InitialValue = Constant.Word32(0);
			liv.DeltaValue = Constant.Word32(1);
			liv.TestValue = Constant.Word32(10);
			LinearInductionVariable iv = liv.CreateInductionVariable();
			Assert.AreEqual("X", iv.ToString());
		}

		[Test]
		public void CreateNo()
		{
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(null, null, null);
			Assert.IsNull(liv.CreateInductionVariable());
		}

		[Test]
		public void CreateBareMinimum()
		{
			ssaIds = new SsaIdentifierCollection();
			Identifier id0 = new Identifier("foo", 1, PrimitiveType.Word32, new TemporaryStorage());
			Identifier id1 = new Identifier("bar", 2, PrimitiveType.Word32, new TemporaryStorage());
			ssaIds.Add(new SsaIdentifier(id0, id0, null, null, false));
			ssaIds.Add(new SsaIdentifier(id1, id1, null, null, false));
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(null, ssaIds, null);
			liv.PhiStatement = new Statement(null, null);
			liv.PhiIdentifier = new Identifier("i_3", 1, PrimitiveType.Word32, null);
			liv.DeltaValue = Constant.Word32(1);
			LinearInductionVariable iv = liv.CreateInductionVariable();
			Assert.AreEqual("(? 0x00000001 ?)", iv.ToString());
		}

		[Test]
		public void CreateIncInitialValue()
		{
			ssaIds = new SsaIdentifierCollection();
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(null, ssaIds, null);
			liv.InitialValue = Constant.Word32(0);
			liv.PhiStatement = new Statement(null, null);
			liv.PhiIdentifier = new Identifier("foo_0", 0, PrimitiveType.Word32, null);
            ssaIds.Add(new SsaIdentifier(liv.PhiIdentifier, liv.PhiIdentifier, liv.PhiStatement, null, false));
			liv.DeltaValue = Constant.Word32(1);
			liv.DeltaStatement = new Statement(new Assignment(new Identifier("foo_1", 1, PrimitiveType.Word32, null), 
				new BinaryExpression(Operator.add, PrimitiveType.Word32, liv.PhiIdentifier, liv.DeltaValue)), null);
			ssaIds[liv.PhiIdentifier].Uses.Add(liv.DeltaStatement);

			LinearInductionVariable iv = liv.CreateInductionVariable();
			Assert.AreEqual("(0x00000001 0x00000001 ?)", iv.ToString());
		}

		[Test]
		public void CreateNoincInitialValue()
		{
			ProcedureMock m = new ProcedureMock();
			ssaIds = new SsaIdentifierCollection();
			SsaId(new Identifier("id0", 0, PrimitiveType.Word32, new TemporaryStorage()), null, null, false);
			SsaId(new Identifier("id1", 1, PrimitiveType.Word32, new TemporaryStorage()), null, null, false);
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(null, ssaIds, null);

			liv.InitialValue = Constant.Word32(0);
			Identifier id2 = m.Local32("id_2");
			SsaId(id2, new Statement(null, null), null, false);
			Assert.AreEqual(3, ssaIds.Count);

			Identifier id3 = m.Local32("id_3");
			Assert.AreEqual(3, id3.Number);
			Identifier id4 = m.Local32("id_4");
			liv.PhiStatement = m.Phi(id3, id2, id4);
			liv.PhiIdentifier = id3;
			SsaId(id3, liv.PhiStatement, ((PhiAssignment)liv.PhiStatement.Instruction).Src, false);
			Assert.AreEqual(4, ssaIds.Count);

			Statement use = new Statement(null, null);
			ssaIds[id3].Uses.Add(use);

			liv.DeltaValue = m.Int32(1);
			liv.DeltaStatement = m.Add(id4, id3, liv.DeltaValue);
			ssaIds[id3].Uses.Add(liv.DeltaStatement);

			LinearInductionVariable iv = liv.CreateInductionVariable();
			Assert.AreEqual("(0x00000000 0x00000001 ?)", iv.ToString());

		}

        [Test]
        public void PreTestedUge()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier i = m.Local32("i");
            m.Label("test");
            m.BranchIf(m.Uge(i, 10), "done");
            m.Store(m.Word32(0x4204), i);
            m.Assign(i, m.Add(i, 1));
            m.Jump("test");
            m.Label("done");
            m.Store(m.Word32(0x4200), i);
            m.Return();
            m.Procedure.Dump(true, false);
            m.Procedure.RenumberBlocks();
            Prepare(m.Procedure);
            LinearInductionVariableFinder liv = new LinearInductionVariableFinder(m.Procedure, ssaIds, doms);
            liv.Find();
            Assert.AreEqual("@@@", liv.InductionVariables[0].ToString());
        }

		[Test]
		public void CreateDecTest()
		{
			ProcedureMock m = new ProcedureMock();
			ssaIds = new SsaIdentifierCollection();
			Identifier id0 = new Identifier("id0", 0, PrimitiveType.Word32, new TemporaryStorage());
			Identifier id1 = new Identifier("id1", 1, PrimitiveType.Word32, new TemporaryStorage());
			ssaIds.Add(new SsaIdentifier(id0, id0, null, null, false));
			ssaIds.Add(new SsaIdentifier(id1, id1, null, null, false));
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(null, ssaIds, null);

			liv.InitialValue = Constant.Word32(10);		// id_2 = 10;
			Identifier id2 = m.Local32("id_2");
			SsaId(id2, new Statement(null, null), null, false);

			m.Label("loop");
			Identifier id3 = m.Local32("id_3");		// do {
			Identifier id4 = m.Local32("id_4");		//   id_3 = phi(id_2, id_4);
			liv.PhiStatement = m.Phi(id3, id2, id4);
			liv.PhiIdentifier = id3;
			SsaId(id3, liv.PhiStatement, ((PhiAssignment) liv.PhiStatement.Instruction).Src, false);

			liv.DeltaValue = m.Int32(-1);				//  id_4 = id_3 - 1;
			liv.DeltaStatement = m.Sub(id4, id3, m.Int32(1));
            SsaId(id4, liv.DeltaStatement, ((Assignment) liv.DeltaStatement.Instruction).Src, false);
			ssaIds[id3].Uses.Add(liv.DeltaStatement);

			liv.TestStatement = m.BranchIf(m.Ge(id4, 0), "loop");
			liv.TestOperator = Operator.ge;				//  if (id_4 >= 0)
			liv.TestValue = Constant.Word32(0);
			ssaIds[id4].Uses.Add(liv.DeltaStatement);

			LinearInductionVariable iv = liv.CreateInductionVariable();
			Assert.AreEqual("(0x00000009 0xFFFFFFFF 0xFFFFFFFE)", iv.ToString());
		}

		[Test]
		public void Commensurate()
		{
			LinearInductionVariable liv1 = new LinearInductionVariable(null, Constant.Word32(1), null, false);
            LinearInductionVariable liv2 = new LinearInductionVariable(null, Constant.Word32(2), null, false);
			LinearInductionVariable liv =
				LinearInductionVariable.Merge(liv1, liv2);
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
			this.proc = proc;
			doms = new DominatorGraph(proc);
			SsaTransform sst = new SsaTransform(proc, doms, false);
			SsaState ssa = sst.SsaState;
			ssaIds = ssa.Identifiers;
	

			ConditionCodeEliminator cce = new ConditionCodeEliminator(ssaIds, new ArchitectureMock());
			cce.Transform();
			

			DeadCode.Eliminate(proc, ssa);

			ValuePropagator vp = new ValuePropagator(ssa.Identifiers, proc);
			vp.Transform();

			DeadCode.Eliminate(proc, ssa);
		}

		private void RunTest(Procedure proc, string outputFile)
		{
			Prepare(proc);
			LinearInductionVariableFinder liv = new LinearInductionVariableFinder(proc, ssaIds, doms);
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
			ssaIds.Add(new SsaIdentifier(id, id, stm, expr, isSideEffect));
		}
	}
}
