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
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Core.Machine;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class ValuePropagationTests : AnalysisTestBase
	{
		SsaIdentifierCollection ssaIds;

		[SetUp]
		public void Setup()
		{
			ssaIds = new SsaIdentifierCollection();
		}

		[Test]
		public void VpAddSubCarries()
		{
			RunTest("Fragments/addsubcarries.asm", "Analysis/VpAddSubCarries.txt");
		}

		[Test]
		public void VpChainTest()
		{
			RunTest("Fragments/multiple/chaincalls.asm", "Analysis/VpChainTest.txt");
		}


		[Test]
		public void VpConstPropagation()
		{
			RunTest("Fragments/constpropagation.asm", "Analysis/VpConstPropagation.txt");
		}


		[Test]
		public void VpGlobalHandle()
		{
			RunTest32("Fragments/import32/globalhandle.asm", "Analysis/VpGlobalHandle.txt");
		}

		[Test]
		public void VpNegsNots()
		{
			RunTest("Fragments/negsnots.asm", "Analysis/VpNegsNots.txt");
		}

		[Test]
		public void VpNestedRepeats()
		{
			RunTest("Fragments/nested_repeats.asm", "Analysis/VpNestedRepeats.txt");
		}

		[Test]
		public void VpStringInstructions()
		{
			RunTest("Fragments/stringinstr.asm", "Analysis/VpStringInstructions.txt");
		}

		[Test]
		public void VpSuccessiveDecs()
		{
			RunTest("Fragments/multiple/successivedecs.asm", "Analysis/VpSuccessiveDecs.txt");
		}

		[Test]
		public void VpWhileGoto()
		{
			RunTest("Fragments/while_goto.asm", "Analysis/VpWhileGoto.txt");
		}

		[Test]
		public void VpDbp()
		{
			Procedure proc = new DpbMock().Procedure;
			DominatorGraph gr = new DominatorGraph(proc);
			SsaTransform sst = new SsaTransform(proc, gr, false);
			SsaState ssa = sst.SsaState;
			ValuePropagator vp = new ValuePropagator(ssa.Identifiers, proc);
			vp.Transform();
			DeadCode.Eliminate(proc, ssa);

			using (FileUnitTester fut = new FileUnitTester("Analysis/VpDbp.txt"))
			{
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void VpEquality()
		{
			Identifier foo = Reg32("foo");

			ValuePropagator vp = new ValuePropagator(ssaIds, null);
			BinaryExpression expr = 
				new BinaryExpression(Operator.eq, PrimitiveType.Bool, 
				new BinaryExpression(Operator.sub, PrimitiveType.Word32, foo,
				Constant.Word32(1)),
				Constant.Word32(0));
			Assert.AreEqual("foo - 0x00000001 == 0x00000000", expr.ToString());

			Expression simpler = vp.TransformBinaryExpression(expr);
			Assert.AreEqual("foo == 0x00000001", simpler.ToString());
		}

		[Test]
		public void VpAddZero()
		{
			Identifier r = Reg32("r");
			Identifier s = Reg32("s");

			Statement stm = new Statement(
				new Assignment(s, 
				new BinaryExpression(Operator.sub, PrimitiveType.Word32, new MemoryAccess(MemoryIdentifier.GlobalMemory, r, PrimitiveType.Word32), Constant.Word32(0))), null);

			ValuePropagator vp = new ValuePropagator(ssaIds, null);
			Instruction instr = stm.Instruction.Accept(vp);
			Assert.AreEqual("s = Mem0[r:word32]", instr.ToString());
		}

		[Test]
		public void VpEquality2()
		{
			// Makes sure that 
			// y = x - 2
			// if (y == 0) ...
			// doesn't get munged into
			// y = x - 2
			// if (x == 2)

			Identifier x = Reg32("x");
			Identifier y = Reg32("y");
            ProcedureMock m = new ProcedureMock();
            Statement stmX = m.Assign(x, m.LoadDw(Constant.Word32(0x1000300)));
            Statement stmY = m.Assign(y, m.Sub(x, 2));
			Statement stm = m.BranchIf(m.Eq(y, 0), "test");
			ssaIds[x].DefStatement = stmX;
			ssaIds[y].DefStatement = stmY;
			Assert.AreEqual("x = Mem0[0x01000300:word32]", stmX.Instruction.ToString());
			Assert.AreEqual("y = x - 0x00000002", stmY.Instruction.ToString());
			Assert.AreEqual("branch y == 0x00000000 test", stm.Instruction.ToString());

			ValuePropagator vp = new ValuePropagator(ssaIds, null);
			Instruction instr = stm.Instruction.Accept(vp);
			Assert.AreEqual("branch y == 0x00000000 test", instr.ToString());
		}

		[Test]
		public void VpCopyPropagate()
		{
			Identifier x = Reg32("x");
			Identifier y = Reg32("y");
			Identifier z = Reg32("z");
			Identifier w = Reg32("w");
			Statement stmX = new Statement(new Assignment(x, new MemoryAccess(MemoryIdentifier.GlobalMemory, Constant.Word32(0x10004000), PrimitiveType.Word32)), null);
			Statement stmY = new Statement(new Assignment(y, x), null);
			Statement stmZ = new Statement(new Assignment(z, new BinaryExpression(Operator.add, PrimitiveType.Word32, y, Constant.Word32(2))), null);
			Statement stmW = new Statement(new Assignment(w, y), null);
			ssaIds[x].DefStatement = stmX;
			ssaIds[y].DefStatement = stmY;
			ssaIds[z].DefStatement = stmZ;
			ssaIds[w].DefStatement = stmW;
			ssaIds[x].Uses.Add(stmY);
			ssaIds[y].Uses.Add(stmZ);
			ssaIds[y].Uses.Add(stmW);
			Assert.AreEqual("x = Mem0[0x10004000:word32]", stmX.Instruction.ToString());
			Assert.AreEqual("y = x", stmY.Instruction.ToString());
			Assert.AreEqual("z = y + 0x00000002", stmZ.Instruction.ToString());
			Assert.AreEqual("w = y", stmW.Instruction.ToString());

			ValuePropagator vp = new ValuePropagator(ssaIds, null);
			vp.Transform(stmX);
			vp.Transform(stmY);
			vp.Transform(stmZ);
			vp.Transform(stmW);

			Assert.AreEqual("x = Mem0[0x10004000:word32]", stmX.Instruction.ToString());
			Assert.AreEqual("y = x", stmY.Instruction.ToString());
			Assert.AreEqual("z = x + 0x00000002", stmZ.Instruction.ToString());
			Assert.AreEqual("w = x", stmW.Instruction.ToString());
			Assert.AreEqual(3, ssaIds[x].Uses.Count);
			Assert.AreEqual(0, ssaIds[y].Uses.Count);
		}

		[Test]
		public void VpSliceConstant()
		{
			ValuePropagator vp = new ValuePropagator(ssaIds, null);
			Expression c = vp.TransformSlice(new Slice(PrimitiveType.Byte, Constant.Word32(0x10FF), 0));
			Assert.AreEqual("0xFF", c.ToString());
		}

		[Test]
		public void VpNegSub()
		{
			Identifier x = Reg32("x");
			Identifier y = Reg32("y");
			ValuePropagator vp = new ValuePropagator(ssaIds, null);
			Expression e = vp.TransformUnaryExpression(
				new UnaryExpression(Operator.neg, PrimitiveType.Word32, new BinaryExpression(
				Operator.sub, PrimitiveType.Word32, x, y)));
			Assert.AreEqual("y - x", e.ToString());
		}

		/// <summary>
		/// (<< (+ (* id c1) id) c2))
		/// </summary>
		[Test] 
		public void VpMulAddShift()
		{
			Identifier id = Reg32("id");
			Identifier x =  Reg32("x");
			ValuePropagator vp = new ValuePropagator(ssaIds, null);
			PrimitiveType t = PrimitiveType.Int32;
			BinaryExpression b = new BinaryExpression(Operator.shl, t, 
				new BinaryExpression(Operator.add, t, 
					new BinaryExpression(Operator.muls, t, id, new Constant(t, 4)),
					id),
				new Constant(t, 2));
			Expression e = vp.TransformBinaryExpression(b);
			Assert.AreEqual("id *s 20", e.ToString());

		}

		[Test]
		public void VpShiftShift()
		{
			Identifier id = Reg32("id");
			ProcedureMock m = new ProcedureMock();
			Expression e = m.Shl(m.Shl(id, 1), 4);
			ValuePropagator vp = new ValuePropagator(ssaIds, null);
			e = e.Accept(vp);
			Assert.AreEqual("id << 0x00000005", e.ToString());
		}

		[Test]
		public void VpShiftSum()
		{
			Constant c = Constant.Word32(1);
			ProcedureMock m = new ProcedureMock();
			Expression e = m.Shl(1, m.Sub(new Constant(PrimitiveType.Byte, 32), 1));
			ValuePropagator vp = new ValuePropagator(ssaIds, null);
			e = e.Accept(vp);
			Assert.AreEqual("0x80000000", e.ToString());
		}

		[Test]
		public void VpSequenceOfConstants()
		{
			Constant pre = new Constant(PrimitiveType.Word16, 0x0001);
			Constant fix = new Constant(PrimitiveType.Word16, 0x0002);
			Expression e = new MkSequence(PrimitiveType.Word32, pre, fix);
			ValuePropagator vp = new ValuePropagator(ssaIds, null);
			e = e.Accept(vp);
			Assert.AreEqual("0x00010002", e.ToString());
		}

        [Test]
        public void SliceShift()
        {
            Constant eight = new Constant(PrimitiveType.Word16, 8);
            Constant ate = new Constant(PrimitiveType.Word32, 8);
            Identifier C = Reg8("C");
            Identifier ax = Reg16("ax");
            Expression e = new Slice(PrimitiveType.Byte, new BinaryExpression(Operator.shl, PrimitiveType.Word16, C, eight), 8);
            ValuePropagator vp = new ValuePropagator(ssaIds, null);
            e = e.Accept(vp);
            Assert.AreEqual("C", e.ToString());
        }
		private Identifier Reg32(string name)
		{
			MachineRegister mr = new MachineRegister(name, ssaIds.Count, PrimitiveType.Word32);
			Identifier id = new Identifier(mr.Name, ssaIds.Count, mr.DataType, new RegisterStorage(mr));
			SsaIdentifier sid = new SsaIdentifier(id, id, null, null, false);
			ssaIds.Add(sid);
			return sid.Identifier;
		}

        private Identifier Reg16(string name)
        {
            MachineRegister mr = new MachineRegister(name, ssaIds.Count, PrimitiveType.Word16);
            Identifier id = new Identifier(mr.Name, ssaIds.Count, mr.DataType, new RegisterStorage(mr));
            SsaIdentifier sid = new SsaIdentifier(id, id, null, null, false);
            ssaIds.Add(sid);
            return sid.Identifier;
        }


        private Identifier Reg8(string name)
        {
            MachineRegister mr = new MachineRegister(name, ssaIds.Count, PrimitiveType.Byte);
            Identifier id = new Identifier(mr.Name, ssaIds.Count, mr.DataType, new RegisterStorage(mr));
            SsaIdentifier sid = new SsaIdentifier(id, id, null, null, false);
            ssaIds.Add(sid);
            return sid.Identifier;
        }

        protected override void RunTest(Program prog, FileUnitTester fut)
		{
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in prog.Procedures.Values)
			{
				fut.TextWriter.WriteLine("= {0} ========================", proc.Name);
				DominatorGraph gr = new DominatorGraph(proc);
				Aliases alias = new Aliases(proc, prog.Architecture);
				alias.Transform();
				SsaTransform sst = new SsaTransform(proc, gr, true);
				SsaState ssa = sst.SsaState;

				ssa.Write(fut.TextWriter);
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();

				ValuePropagator vp = new ValuePropagator(ssa.Identifiers, proc);
				vp.Transform();
				DeadCode.Eliminate(proc, ssa);

				ssa.Write(fut.TextWriter);
				proc.Write(false, fut.TextWriter);
			}
		}

		private class DpbMock : ProcedureMock
		{
			protected override void BuildBody()
			{
				Identifier dl = LocalByte("dl");
				Identifier dx = Local16("dx");
				Identifier edx = Local32("edx");


				Assign(edx, Int32(0x0AAA00AA));
				Assign(edx, Dpb(edx, Int8(0x55), 8, 8));
				Store(Int32(0x1000000), edx);


				Assign(edx, Int32(0));
                Assign(edx, Dpb(edx, dl, 0, 8));
				Return(edx);
			}
		}
	}
}
