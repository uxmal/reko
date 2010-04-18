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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using Decompiler.Analysis;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class ConditionCodeEliminatorTests : AnalysisTestBase
	{
		private SsaIdentifierCollection ssaIds; 

		[SetUp]
		public void Setup()
		{
			ssaIds = new SsaIdentifierCollection();
		}

		[Test]
		public void CceAsciiHex()
		{
			RunTest("Fragments/ascii_hex.asm", "Analysis/CceAsciiHex.txt");
		}


		[Test]
		public void CceAddSubCarries()
		{
			RunTest("Fragments/addsubcarries.asm", "Analysis/CceAddSubCarries.txt");
		}

		[Test]
		public void CceAdcMock()
		{
			RunTest(new AdcMock(), "Analysis/CceAdcMock.txt");
		}

		[Test]
		public void CceCmpMock()
		{
			RunTest(new CmpMock(), "Analysis/CceCmpMock.txt");
		}

		[Test]
		public void CceFrame32()
		{
			RunTest32("fragments/multiple/frame32.asm", "Analysis/CceFrame32.txt");
		}

		[Test]
		public void CceWhileLoop()
		{
			RunTest("fragments/while_loop.asm", "Analysis/CceWhileLoop.txt");
		}

		[Test]
		public void CceReg00005()
		{
			RunTest("Fragments/regressions/r00005.asm", "Analysis/CceReg00005.txt");
		}

		[Test]
		public void CceReg00007()
		{
			RunTest("Fragments/regressions/r00007.asm", "Analysis/CceReg00007.txt");
		}

        [Test]
        public void CceFstswTestAx()
        {
            Program prog = RewriteCodeFragment(@"
                fcomp   dword ptr [bx]
                fstsw  ax
                test    ah,0x41
                jz      done
                xor     ecx,ecx
 done:
                ret
");
            SaveRunOutput(prog, "Analysis/CceFstswTestAx.txt");
        }

        [Test]
        public void CceFstswTextAxWithConstantBl()
        {
            Program prog = RewriteCodeFragment(@"
                mov     bx,1
                fcomp   dword ptr[si]
                fstsw   ax
                test    bl,ah
                jz      done
                mov     byte ptr [0x0300],1
done:
                ret
");
            SaveRunOutput(prog, "Analysis/CceFstswTextAxWithConstantBl.txt");
        }

		[Test]
		public void CceEqId()
		{
			Identifier r = Reg32("r");
			Identifier z = Reg32("z");
			Identifier y = Reg32("y");

            ProcedureMock m = new ProcedureMock();
            Statement stmZ = m.Assign(z, new ConditionOf(r));
			ssaIds[z].DefStatement = stmZ;
            Statement stmY = m.Assign(y, z);
			ssaIds[y].DefStatement = stmY;
			ssaIds[z].Uses.Add(stmY);
			Statement stmBr = m.BranchIf(new TestCondition(ConditionCode.EQ, y), "foo");
			ssaIds[y].Uses.Add(stmBr);

			ConditionCodeEliminator cce = new ConditionCodeEliminator(ssaIds, new ArchitectureMock());
			Instruction instr = stmBr.Instruction.Accept(cce);
			Assert.AreEqual("branch r == 0x00000000 foo", instr.ToString());
		}

		[Test]
		public void CceSetnz()
		{
			Identifier r = Reg32("r");
			Identifier Z = FlagGroup("Z");
			Identifier f = Reg32("f");

			Statement stmZ = new Statement(new Assignment(Z, new ConditionOf(new BinaryExpression(Operator.Sub, PrimitiveType.Word32, r, Constant.Word32(0)))), null);
			ssaIds[Z].DefStatement = stmZ;
			Statement stmF = new Statement(new Assignment(f, new TestCondition(ConditionCode.NE, Z)), null);
			ssaIds[f].DefStatement = stmF;
			ssaIds[Z].Uses.Add(stmF);

			ConditionCodeEliminator cce = new ConditionCodeEliminator(ssaIds, new ArchitectureMock());
			cce.Transform();
			Assert.AreEqual("f = r != 0x00000000", stmF.Instruction.ToString());
		}

		[Test]
		public void SignedIntComparisonFromConditionCode()
		{
			ConditionCodeEliminator cce = new ConditionCodeEliminator(null, new ArchitectureMock());
			BinaryExpression bin = new BinaryExpression(Operator.Sub, PrimitiveType.Word16, new Identifier("a", 0, PrimitiveType.Word16, null), new Identifier("b", 1, PrimitiveType.Word16, null));
			BinaryExpression b = (BinaryExpression) cce.ComparisonFromConditionCode(ConditionCode.LT, bin, false);
			Assert.AreEqual("a < b", b.ToString());
			Assert.AreEqual("LtOperator", b.op.GetType().Name);
		}

		[Test]
		public void RealComparisonFromConditionCode()
		{
			ConditionCodeEliminator cce = new ConditionCodeEliminator(null, new ArchitectureMock());
			BinaryExpression bin = new BinaryExpression(Operator.Sub, PrimitiveType.Real64, new Identifier("a", 0, PrimitiveType.Real64, null), new Identifier("b", 1, PrimitiveType.Real64, null));
			BinaryExpression b = (BinaryExpression) cce.ComparisonFromConditionCode(ConditionCode.LT, bin, false);
			Assert.AreEqual("a < b", b.ToString());
			Assert.AreEqual("RltOperator", b.op.GetType().Name);
		}

		private Identifier Reg32(string name)
		{
			MachineRegister mr = new MachineRegister(name, ssaIds.Count, PrimitiveType.Word32);
			Identifier id = new Identifier(name, ssaIds.Count, PrimitiveType.Word32, new RegisterStorage(mr));
			return ssaIds.Add(id, null, null, false).Identifier;
		}

		private Identifier FlagGroup(string name)
		{
			Identifier id = new Identifier(name, ssaIds.Count, PrimitiveType.Word32, new FlagGroupStorage(1U, "C"));
			return ssaIds.Add(id, null, null, false).Identifier;
		}

		protected override void RunTest(Program prog, FileUnitTester fut)
		{
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in prog.Procedures.Values)
			{
				Aliases alias = new Aliases(proc, prog.Architecture, dfa.ProgramDataFlow);
				alias.Transform();
				SsaTransform sst = new SsaTransform(proc, proc.CreateBlockDominatorGraph(), true);
				SsaState ssa = sst.SsaState;

                ValuePropagator vp = new ValuePropagator(ssa.Identifiers, proc);
                vp.Transform();

				ConditionCodeEliminator cce = new ConditionCodeEliminator(ssa.Identifiers, prog.Architecture);
				cce.Transform();
				DeadCode.Eliminate(proc, ssa);

				ssa.Write(fut.TextWriter);
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();
			}
		}
	}
}
