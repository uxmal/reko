/* 
 * Copyright (C) 1999-2008 John Källén.
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
using Decompiler.Core.Types;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class TypeTransformTests : TypingTestBase
	{
		private TypeFactory factory;
		private TypeStore store;


		[SetUp]
		public void SetUp()
		{
			factory = new TypeFactory();
			store = new TypeStore();
		}

		[Test]
		public void TtranUnknown()
		{
			RunTest("Fragments/Type/unknown.asm", "Typing/TtranUnknown.txt");
		}

		[Test]
		public void TtranFactorial()
		{
			RunTest("Fragments/factorial.asm", "Typing/TtranFactorial.txt");
		}

		[Test]
		public void TtranFactorialReg()
		{
			RunTest("Fragments/factorial_reg.asm", "Typing/TtranFactorialReg.txt");
		}


		[Test]
		public void TtranLength()
		{
			RunTest("Fragments/Type/listlength.asm", "Typing/TtranLength.txt");
		}

		[Test]
		public void TtranIntegers()
		{
			RunTest("Fragments/Type/integraltypes.asm", "Typing/TtranIntegers.txt");
		}

		[Test]
		public void TtranReals()
		{
			RunTest("Fragments/fpuops.asm", "Typing/TtranReals.txt");
		}

		[Test]
		public void TtranReg00008()
		{
			RunTest("fragments/regressions/r00008.asm", "Typing/TtranReg00008.txt");
		}

		[Test]
		public void TtranMemAccesses()
		{
			RunTest("fragments/multiple/memaccesses.asm", "Typing/TtranMemAccesses.txt");
		}

		[Test]
		public void TtranPtrPtrInt()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new PtrPtrIntMock());
			RunTest(mock.BuildProgram(), "Typing/TtranPtrPtrInt.txt");
		}

		[Test]
		public void TtranGlobalVariables()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new GlobalVariablesMock());
			RunTest(mock.BuildProgram(), "Typing/TtranGlobalVariables.txt");
		}

	
		[Test]
		public void TtranFramePointer()
		{
			ProgramMock prog = new ProgramMock();
			prog.Add(new FramePointerMock(factory));
			RunTest(prog.BuildProgram(), "Typing/TtranFramePointer.txt");
		}


		[Test]
		public void TtranRepeatedLoads()
		{
			ProgramMock prog = new ProgramMock();
			prog.Add(new RepeatedLoadsMock());
			RunTest(prog.BuildProgram(), "Typing/TtranRepeatedLoads.txt");
		}
		
		[Test]
		public void TtranStaggeredArrays()
		{
			ProgramMock prog = new ProgramMock();
			prog.Add(new StaggeredArraysMock());
			RunTest(prog.BuildProgram(), "Typing/TtranStaggeredArrays.txt");
		}

		[Test]
		public void TtranFnPointerMock()
		{
			ProgramMock prog = new ProgramMock();
			prog.Add(new FnPointerMock());
			RunTest(prog.BuildProgram(), "Typing/TtranFnPointerMock.txt");
		}

		[Test]
		public void TtranSimplify()
		{
			UnionType u = new UnionType(null, null);
			u.Alternatives.Add(new Pointer(PrimitiveType.Real32, 0));
			u.Alternatives.Add(new Pointer(PrimitiveType.Real32, 0));
			TypeTransformer trans = new TypeTransformer(factory, store, null);
			DataType dt = u.Accept(trans);
			Assert.AreEqual("(ptr real32)", dt.ToString());
		}

		[Test]
		public void TtranSimplify2()
		{
			UnionType u = new UnionType(null, null);
			u.Alternatives.Add(PrimitiveType.Word32);
			u.Alternatives.Add(new Pointer(PrimitiveType.Real32, 0));
			TypeTransformer trans = new TypeTransformer(factory, store, null);
			DataType dt = u.Accept(trans);
			Assert.AreEqual("(ptr real32)", dt.ToString());
		}

		[Test]
		public void TtranUnion()
		{
			UnionType t = factory.CreateUnionType("foo", null);
			t.Alternatives.Add(new UnionAlternative(PrimitiveType.Word32));
			t.Alternatives.Add(new UnionAlternative(PrimitiveType.Word32));
			t.Alternatives.Add(new UnionAlternative(PrimitiveType.Word32));
			t.Alternatives.Add(new UnionAlternative(PrimitiveType.Word32));
			TypeTransformer trans = new TypeTransformer(factory, null, null);
			PrimitiveType dt = (PrimitiveType) t.Accept(trans);
			Assert.AreEqual("word32", dt.ToString());

			t.Alternatives.Add(PrimitiveType.Real32);
			t.Alternatives.Add(PrimitiveType.Int32);
			DataType d = t.Accept(trans);
			Assert.AreEqual("(union \"foo\" (int32 u0) (real32 u1))", d.ToString());
		}

		[Test]
		public void MergeIdenticalStructureFields()
		{
			StructureType s = factory.CreateStructureType(null, 0);
			s.Fields.Add(4, new TypeVariable(1));
			s.Fields.Add(4, new TypeVariable(1));
			s.Fields.Add(4, new TypeVariable(1));
			s.Fields.Add(4, new TypeVariable(1));
			s.Fields[0].DataType = PrimitiveType.Int16;
			s.Fields[1].DataType = PrimitiveType.Int16;
			s.Fields[2].DataType = PrimitiveType.Int16;
			s.Fields[3].DataType = PrimitiveType.Int16;
			Assert.AreEqual(4, s.Fields.Count);
			TypeTransformer trans = new TypeTransformer(factory, null, null);
			StructureType sNew = trans.MergeStructureFields(s);
			Assert.AreEqual(1, sNew.Fields.Count);
			Assert.AreEqual("int16", sNew.Fields[0].DataType.ToString());
		}

		[Test]
		public void HasCoincidentFields()
		{
			StructureType s = new StructureType(null, 0);
			s.Fields.Add(4, new TypeVariable(1));
			s.Fields.Add(4, PrimitiveType.Word16);
			Assert.AreEqual(2, s.Fields.Count);
			TypeTransformer trans = new TypeTransformer(factory, null, null);
			Assert.IsTrue(trans.HasCoincidentFields(s));
		}

		[Test]
		public void HasNoCoincidentFields()
		{
			StructureType s = new StructureType(null, 0);
			s.Fields.Add(4, new TypeVariable(1));
			s.Fields.Add(5, PrimitiveType.Word16);
			Assert.AreEqual(2, s.Fields.Count);
			TypeTransformer trans = new TypeTransformer(factory, null, null);
			Assert.IsFalse(trans.HasCoincidentFields(s));
		}

		[Test]
		public void TtranIntelIndexedAddressingMode()
		{
			ProgramMock m = new ProgramMock();
			m.Add(new IntelIndexedAddressingMode());
			RunTest(m.BuildProgram(), "Typing/TtranIntelIndexedAddressingMode.txt");
		}

		[Test]
		public void TtranTreeFind()
		{
			ProgramMock m = new ProgramMock();
			m.Add(new TreeFindMock());
			RunTest(m.BuildProgram(), "Typing/TtranTreeFind.txt");
		}

		private void RunTest(Program prog, string outputFileName)
		{
			ArrayExpressionNormalizer aen = new ArrayExpressionNormalizer();
			aen.Transform(prog);
			EquivalenceClassBuilder eq = new EquivalenceClassBuilder(factory, store);
			eq.Build(prog);
			DataTypeBuilder dtb = new DataTypeBuilder(factory, store);
			TraitCollector coll = new TraitCollector(factory, store, dtb, prog.Globals, ivs);
			coll.CollectProgramTraits(prog);
			dtb.BuildEquivalenceClassDataTypes();

			DerivedPointerAnalysis cpf = new DerivedPointerAnalysis(factory, store, dtb);
			cpf.FollowConstantPointers(prog);

			TypeVariableReplacer tvr = new TypeVariableReplacer(store);
			tvr.ReplaceTypeVariables();

			TypeTransformer trans = new TypeTransformer(factory, store, null);
			trans.Transform();
			using (FileUnitTester fut = new FileUnitTester(outputFileName))
			{
				foreach (Procedure proc in prog.Procedures.Values)
				{
					proc.Write(false, fut.TextWriter);
					fut.TextWriter.WriteLine();
				}
				store.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		private void RunTest(string asmSourceFileName, string testFileName)
		{
			RewriteFile(asmSourceFileName);
			RunTest(prog, testFileName);
		}
	}

	public class FramePointerMock : ProcedureMock
	{
		private TypeFactory factory;

		public FramePointerMock(TypeFactory factory)
		{
			this.factory = factory;
		}

		protected override void BuildBody()
		{
			Identifier frame = Declare(new StructureType("frame_t", 0), "frame");
			Identifier fp = Local32("fp");
			Assign(fp, AddrOf(frame));
			Store(Add(fp, 4), Load(PrimitiveType.Word32, Add(fp, 8)));
		}
	}

	public class RepeatedLoadsMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier pfoo = Local32("pfoo");
			Identifier x = Local32("x");
			Load(x, Add(pfoo, 4));
			Load(x, Add(pfoo, 4));
		}
	}

	public class StaggeredArraysMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier p = Local32("p");
			Identifier x = Local32("x");
			Identifier i = Local32("i");
			Load(x, Add(p, Muls(i, 8)));
			Load(x, Add(p, Add(Muls(i, 8), 4)));
		}
	}

	public class FnPointerMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier pfn = Local32("pfn");
			Assign(pfn, Int32(0x1213130));
			Store(Int32(0x10000000), pfn);
			this.Emit(new IndirectCall(LoadDw(Int32(0x10000000)), 0, 0));
		}
	}

}
