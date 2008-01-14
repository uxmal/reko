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
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class DataTypeBuilderTests : TypingTestBase
	{
		private TypeFactory factory;
		private TypeStore store;
		private ArrayExpressionNormalizer aen;
		private EquivalenceClassBuilder eqb;
		private DataTypeBuilder dtb;

		[Test]
		public void DtbArrayAccess()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new ArrayAccess());
			RunTest(mock, "Typing/DtbArrayAccess.txt");
		}

		private class ArrayAccess : ProcedureMock
		{
			protected override void BuildBody()
			{
				Identifier i = Local32("i");
				Identifier r = Local32("r");
				Identifier r2 = Local16("r2");
				Store(Add(Add(r, 20), Muls(i, 10)), 0);
				Return(Load(PrimitiveType.Word16, 
					Add(Add(r, 16), Muls(i, 10))));
			}
		}

		[Test]
		public void DtbArrayLoopMock()
		{
			ProgramMock mock = new Mocks.ProgramMock();
			mock.Add(new ArrayLoopMock());
			RunTest(mock, "Typing/DtbArrayLoopMock.txt");
		}

		[Test]
		public void DtbGlobalVariables()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new GlobalVariablesMock());
			RunTest(mock, "Typing/DtbGlobalVariables.txt");
		}

		[Test]
		public void DtbEqClass()
		{
			Identifier id1 = new Identifier("foo", 0, PrimitiveType.Word32, null);
			Identifier id2 = new Identifier("bar", 1, PrimitiveType.Real32, null);
			id1.Accept(eqb);
			id2.Accept(eqb);
			store.MergeClasses(id1.TypeVariable, id2.TypeVariable);

			dtb.DataTypeTrait(id1.TypeVariable, id1.DataType);
			dtb.DataTypeTrait(id2.TypeVariable, id2.DataType);
			dtb.BuildEquivalenceClassDataTypes();

			ICollection used = store.UsedEquivalenceClasses;
			Assert.AreEqual(1, used.Count);
			Verify("Typing/DtbEqClass.txt");
		}

		[Test]
		public void DtbEqClassType()
		{
			Identifier id1 = new Identifier("foo", 0, PrimitiveType.Word32, null);
			Identifier id2 = new Identifier("bar", 1, PrimitiveType.Real32, null);
			id1.Accept(eqb);
			id2.Accept(eqb);
			store.MergeClasses(id1.TypeVariable, id2.TypeVariable);
			
			dtb.DataTypeTrait(id1.TypeVariable, id1.DataType);
			dtb.DataTypeTrait(id2.TypeVariable, id2.DataType);
			dtb.BuildEquivalenceClassDataTypes();
			
			EquivalenceClass e = id1.TypeVariable.Class;
			PrimitiveType p = (PrimitiveType) e.DataType;
			Assert.AreEqual(PrimitiveType.Real32, p);

			Verify("Typing/DtbEqClassType.txt");
		}

		[Test]
		public void DtbFramePointer()
		{
			ProgramMock mock = new Mocks.ProgramMock();
			mock.Add(new FramePointerMock(factory));
			RunTest(mock, "Typing/DtbFramePointer.txt");
		}

		[Test]
		public void DtbFnPointerMock()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new FnPointerMock());
			RunTest(mock, "Typing/DtbFnPointerMock.txt");
		}

		[Test]
		public void DtbMems()
		{
			Identifier foo = new Identifier("foo", 0, PrimitiveType.Word32, null);
			Identifier bar = new Identifier("bar", 1, PrimitiveType.Word16, null);
			Identifier baz = new Identifier("baz", 2, PrimitiveType.Word32, null);
			Identifier fred = new Identifier("fred", 3, PrimitiveType.Word32, null);
			Assignment ass1 = new Assignment(bar, MemLoad(foo, 4, PrimitiveType.Word16));
			Assignment ass2 = new Assignment(baz, MemLoad(foo, 6, PrimitiveType.Word32));
			Assignment ass3 = new Assignment(fred, MemLoad(baz, 0, PrimitiveType.Word32));
			ass1.Accept(eqb);
			ass2.Accept(eqb);
			ass3.Accept(eqb);
			TraitCollector trco = new TraitCollector(factory, store, dtb, new Identifier("globals", 0, PrimitiveType.Pointer, null), new InductionVariableCollection());
			trco.Procedure = new Procedure("foo", new Frame(PrimitiveType.Word32));
			trco.VisitAssignment(ass1);
			trco.VisitAssignment(ass2);
			trco.VisitAssignment(ass3);
			dtb.BuildEquivalenceClassDataTypes();

			Verify("Typing/DtbMems.txt");
		}


		[Test]
		public void DtbRepeatedLoads()
		{
			Identifier pfoo = new Identifier("pfoo", 0, PrimitiveType.Word32, null);
			Identifier x = new Identifier("x", 1, PrimitiveType.Word32, null);
			Assignment ass1 = new Assignment(x, MemLoad(pfoo, 4, PrimitiveType.Word32));
			Assignment ass2 = new Assignment(x, MemLoad(pfoo, 4, PrimitiveType.Word32));
			ass1.Accept(eqb);
			ass2.Accept(eqb);
			TraitCollector trco = new TraitCollector(factory, store, dtb, new Identifier("globals", 0, PrimitiveType.Pointer, null), ivs);
			trco.Procedure = new Procedure("foo", null);
			trco.VisitAssignment(ass1);
			trco.VisitAssignment(ass2);
			dtb.BuildEquivalenceClassDataTypes();

			Verify("Typing/DtbRepeatedLoads.txt");


		}

		[Test]
		public void DtbSameMemFetch()
		{
			Identifier foo = new Identifier("foo", 0, PrimitiveType.Word32, null);
			Identifier bar = new Identifier("bar", 1, PrimitiveType.Word16, null);
			Identifier baz = new Identifier("baz", 1, PrimitiveType.Word16, null);
			Assignment ass1 = new Assignment(bar, MemLoad(foo, 4, PrimitiveType.Word16));
			Assignment ass2 = new Assignment(baz, MemLoad(foo, 4, PrimitiveType.Word16));
			ass1.Accept(eqb);
			ass2.Accept(eqb);
			TraitCollector trco = new TraitCollector(factory, store, dtb, new Identifier("globals", 0, PrimitiveType.Pointer, null), ivs);
			trco.Procedure = new Procedure("foo", null);
			trco.VisitAssignment(ass1);
			trco.VisitAssignment(ass2);
			dtb.BuildEquivalenceClassDataTypes();
			
			Verify("Typing/DtbSameMemFetch.txt");
		}

		/// <summary>
		/// Should result in an array of bytes and an array of words.
		/// </summary>
		[Test]
		public void DtbInductionVariables()
		{
			Identifier globals = new Identifier("globals", 0, PrimitiveType.Pointer, null);
			TraitCollector trco = new TraitCollector(factory, store, dtb, globals, ivs);

			Identifier i = new Identifier("i", 0, PrimitiveType.Word32, null);
			MemoryAccess load = new MemoryAccess(MemoryIdentifier.GlobalMemory, i, PrimitiveType.Int32);
			Identifier i2 = new Identifier("i2", 1, PrimitiveType.Word32, null);
			MemoryAccess ld2 = new MemoryAccess(MemoryIdentifier.GlobalMemory, i2, PrimitiveType.Int32);
			Procedure proc = new Procedure("foo", null);
			trco.Procedure = proc;

			LinearInductionVariable iv = new LinearInductionVariable(
				Constant.Word32(0), 
				Constant.Word32(1),
				Constant.Word32(10));
			LinearInductionVariable iv2 = new LinearInductionVariable(
				Constant.Word32(0x0010000),
				Constant.Word32(4),
				Constant.Word32(0x0010040));

			ivs.Add(proc, i, iv);
			ivs.Add(proc, i2, iv2);

			globals.Accept(eqb);
			load.Accept(eqb);
			ld2.Accept(eqb);
			trco.Procedure = proc;
			globals.Accept(trco);
			load.Accept(trco);
			ld2.Accept(trco);
			dtb.BuildEquivalenceClassDataTypes();

			Verify("Typing/DtbInductionVariables.txt");
		}

		[Test]
		public void DtbBuildEqClassDataTypes()
		{
			TypeVariable tv1 = store.EnsureTypeVariable(factory, null);
			tv1.OriginalDataType = PrimitiveType.Word32;
			TypeVariable tv2 = store.EnsureTypeVariable(factory, null);
			tv2.OriginalDataType = PrimitiveType.Real32;
			store.MergeClasses(tv1, tv2);

			dtb.BuildEquivalenceClassDataTypes();
			Assert.AreEqual(PrimitiveType.Real32, tv1.Class.DataType);
		}

		[Test]
		public void DtbTypeVariable()
		{
			Expression e1 = Constant.Word32(42);
			e1.Accept(eqb);
			dtb.DataTypeTrait(e1.TypeVariable, e1.DataType);
			dtb.BuildEquivalenceClassDataTypes();
			Verify("Typing/DtbTypeVariable.txt");
		}

		[Test]
		public void DtbGlobalArray()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier globals = m.Local32("globals");
			Identifier i = m.Local32("i");
			Expression ea = m.Add(globals, m.Add(m.Shl(i, 2), 0x3000));
			Expression e = m.Load(PrimitiveType.Int32, ea);
			TraitCollector trco = new TraitCollector(factory, store, dtb, globals, null);
			e = e.Accept(aen);
			e.Accept(eqb);
			e.Accept(trco);
			Verify("Typing/DtbGlobalArray.txt");
		}

		[Test]
		public void DtbUnion()
		{
			Identifier id1 = new Identifier("foo", 0, PrimitiveType.Int32, null);		// note signed: can't be unified with real
			Identifier id2 = new Identifier("bar", 1, PrimitiveType.Real32, null);
			id1.Accept(eqb);
			id2.Accept(eqb);
			store.MergeClasses(id1.TypeVariable, id2.TypeVariable);
			
			dtb.DataTypeTrait(id1.TypeVariable, id1.DataType);
			dtb.DataTypeTrait(id2.TypeVariable, id2.DataType);
			dtb.BuildEquivalenceClassDataTypes();
			
			UnionType u = (UnionType) id1.TypeVariable.Class.DataType;
			Assert.AreEqual(2, u.Alternatives.Count);
		}

		[Test]
		public void DtbReals()
		{
			RewriteFile("Fragments/fpuops.asm");
            RunTest(this.prog, "Typing/DtbReals.txt");
		}

		[Test]
		public void DtbSegmentedAccess()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier globals = m.Local32("globals");

			Identifier ds = m.Local16("ds");
			Identifier bx = m.Local16("bx");
			Expression e = m.SegMem(bx.DataType, ds, m.Add(bx, 4));
			TraitCollector trco = new TraitCollector(factory, store, dtb, globals, null);
			e = e.Accept(aen);
			e.Accept(eqb);
			e.Accept(trco);
			dtb.BuildEquivalenceClassDataTypes();
			Verify("Typing/DtbSegmentedAccess.txt");
		}

		[Test]
		public void DtbSegmentedDirectAddress()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier globals = m.Local32("globals");
			store.EnsureTypeVariable(factory, globals);

			Identifier ds = m.Local16("ds");
			Expression e = m.SegMem(PrimitiveType.Byte, ds, m.Int16(0x0200));
			
			TraitCollector coll = new TraitCollector(factory, store, dtb, globals, ivs);
			coll.Procedure = new Procedure("foo", new Frame(PrimitiveType.Word32));
			e = e.Accept(aen);
			e.Accept(eqb);
			e.Accept(coll);
			dtb.BuildEquivalenceClassDataTypes();
			Verify("Typing/DtbSegmentedDirectAddress.txt");
		}

		[Test]
		public void DtbSegmentedDoubleReference()
		{
			ProgramMock m = new ProgramMock();
			m.Add(new SegmentedDoubleReferenceMock());
			RunTest(m, "Typing/DtbSegmentedDoubleReference.txt");
		}

		[Test]
		public void DtbReg00008()
		{
			this.RewriteFile("fragments/regressions/r00008.asm");
			RunTest(prog, "Typing/DtbReg00008.txt");
		}

		[Test]
		public void DtbTreeFind()
		{
			ProgramMock m = new ProgramMock();
			m.Add(new TreeFindMock());
			RunTest(m, "Typing/DtbTreeFind.txt");
		}

		[Test]
		public void DtbSegmentedMemoryPointer()
		{
			ProgramMock m = new ProgramMock();
			m.Add(new SegmentedMemoryPointerMock());
			RunTest(m.BuildProgram(), "Typing/DtbSegmentedMemoryPointer.txt");
		}

		[SetUp]
		public void SetUp()
		{
			store = new TypeStore();
			factory = new TypeFactory();
			ivs = new InductionVariableCollection();
			aen = new ArrayExpressionNormalizer();
			eqb = new EquivalenceClassBuilder(factory, store);
			dtb = new DataTypeBuilder(factory, store);
		}

		private void RunTest(ProgramMock mock, string outputFile)
		{
			prog = mock.BuildProgram();
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerHost());
			dfa.BuildExpressionTrees(null);
			ivs = dfa.InductionVariables;
			RunTest(prog, outputFile);
		}

		private void RunTest(Program prog, string outputFile)
		{
			aen.Transform(prog);
			eqb.Build(prog);
			TraitCollector trco = new TraitCollector(factory, store, dtb, prog.Globals, this.ivs);
			trco.CollectProgramTraits(prog);
			dtb.BuildEquivalenceClassDataTypes();
			Verify(outputFile);
		}

		private void Verify(string outputFile)
		{
			store.CopyClassDataTypesToTypeVariables();
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				store.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}
