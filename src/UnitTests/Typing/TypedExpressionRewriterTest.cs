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
	public class TypedExpressionRewriterTests : TypingTestBase
	{
		private TypeFactory factory;
		private TypeStore store;
		private TypedExpressionRewriter ter;
		private EquivalenceClassBuilder eqb;
		private TraitCollector coll;
		private DataTypeBuilder dtb;
		private DerivedPointerAnalysis cpf;
		private TypeVariableReplacer tvr;
		private TypeTransformer trans;
		private ComplexTypeNamer ctn;

		[Test]
		public void TerComplex()
		{
			Program prog = new Program();
			Identifier id = new Identifier("v0", 0, PrimitiveType.Word32, null);
			Expression cmp = MemLoad(id, 4, PrimitiveType.Word32);

			cmp.Accept(eqb);
			coll = new TraitCollector(factory, store, dtb, prog.Globals, ivs);
			cmp.Accept(coll);
			dtb.BuildEquivalenceClassDataTypes();

			tvr.ReplaceTypeVariables();
			trans.Transform();
			ctn.RenameAllTypes(store);

			ter = new TypedExpressionRewriter(store, prog);
			cmp = cmp.Accept(ter);
			Assert.AreEqual("v0->dw0004", cmp.ToString());
		}

		[Test] public void TerPtrPtrInt()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new PtrPtrIntMock());
			RunTest(mock.BuildProgram(), "Typing/TerPtrPtrInt.txt");
		}

		[Test]
		public void TerUnionIntReal()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new UnionIntRealMock());
			RunTest(mock.BuildProgram(), "Typing/TerUnionIntReal.txt");
		}

		[Test]
		public void TerConstantUnion()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new ConstantUnionMock());
			RunTest(mock.BuildProgram(), "Typing/TerConstantUnion.txt");
		}

		[Test]
		public void TerConstants()
		{
			Program prog = new Program();
			Constant r = new Constant(3.0F);
			Constant i = new Constant(PrimitiveType.Int32, 1);
			Identifier x = new Identifier("x", 0, PrimitiveType.Word32, null);
			Assignment ass = new Assignment(x, r);
			TypeVariable tvR = r.TypeVariable = factory.CreateTypeVariable();
			TypeVariable tvI = i.TypeVariable = factory.CreateTypeVariable();
			TypeVariable tvX = x.TypeVariable = factory.CreateTypeVariable();
			store.TypeVariables.AddRange(new TypeVariable[] { tvR, tvI, tvX });
			UnionType u = factory.CreateUnionType(null, null, new DataType[] { r.DataType, i.DataType });
			tvR.OriginalDataType = r.DataType;
			tvI.OriginalDataType = i.DataType;
			tvX.OriginalDataType = x.DataType;
			tvR.DataType = u;
			tvI.DataType = u;
			tvX.DataType = u;
			ctn.RenameAllTypes(store);
			TypedExpressionRewriter ter = new TypedExpressionRewriter(store, prog);
			Instruction instr = ter.TransformAssignment(ass);
			Assert.AreEqual("x.u1 = 3F;", instr.ToString());
		}

		[Test]
		public void TerVector()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new VectorMock());
			RunTest(mock.BuildProgram(), "Typing/TerVector.txt");
		}

		[Test]
		public void TerGlobalVariables()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new GlobalVariablesMock());
			RunTest(mock.BuildProgram(), "Typing/TerGlobalVariables.txt");
		}

		[Test]
		public void TerSegmentedMemoryPointer()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new SegmentedMemoryPointerMock());
			RunTest(mock.BuildProgram(), "Typing/TerSegmentedMemoryPointer.txt");
		}


		private void RunTest(Program prog, string outputFile)
		{
			eqb.Build(prog);
			coll = new TraitCollector(factory, store, dtb, prog.Globals, ivs);
			coll.CollectProgramTraits(prog);
			dtb.BuildEquivalenceClassDataTypes();
			cpf.FollowConstantPointers(prog);
			tvr.ReplaceTypeVariables();
			trans.Transform();
			ctn.RenameAllTypes(store);
			ter = new TypedExpressionRewriter(store, prog);
			ter.RewriteProgram();
			using (FileUnitTester fut = new FileUnitTester(outputFile))
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

		[SetUp]
		public void Setup()
		{
			factory = new TypeFactory();
			store = new TypeStore();
			eqb = new EquivalenceClassBuilder(factory, store);
			dtb = new DataTypeBuilder(factory, store);
			cpf = new DerivedPointerAnalysis(factory, store, dtb);
			tvr = new TypeVariableReplacer(store);
			trans = new TypeTransformer(factory, store, null);
			ctn = new ComplexTypeNamer();
		}
	}

	public class SegmentedMemoryPointerMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier cs = Local16("cs");
			cs.DataType = PrimitiveType.Segment;
			Identifier ax = Local16("ax");
			Identifier si = Local16("si");
			Identifier si2 = Local16("si2");
			Assign(si, Int16(0x0001));
			Assign(ax, SegMemW(cs, si));
			Assign(si2, Int16(0x0005));
			Assign(ax, SegMemW(cs, si2));
		}
	}

}
 