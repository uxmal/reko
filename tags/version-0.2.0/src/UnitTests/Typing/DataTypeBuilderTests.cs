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
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class DataTypeBuilderTests : TypingTestBase
	{
		private TypeFactory factory;
		private TypeStore store;
		private ExpressionNormalizer aen;
		private EquivalenceClassBuilder eqb;
		private DataTypeBuilder dtb;
        private ArchitectureMock arch;
        private Program prog;

        [SetUp]
        public void SetUp()
        {
            store = new TypeStore();
            factory = new TypeFactory();
            aen = new ExpressionNormalizer(PrimitiveType.Pointer32);
            eqb = new EquivalenceClassBuilder(factory, store);
            arch = new ArchitectureMock();
            prog = new Program();
            prog.Architecture = arch;
            dtb = new DataTypeBuilder(factory, store, arch);
        }

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
        [Ignore("Infrastructure needs to be built to handle negative induction variables correctly.")]
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

			IList<EquivalenceClass> used = store.UsedEquivalenceClasses;
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
        public void DtbArrayAccess2()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier ds = m.Local(PrimitiveType.SegmentSelector, "ds");
            Identifier bx = m.Local16("bx");
            Expression e = m.Array(PrimitiveType.Word32, m.Seq(ds, m.Word16(0x300)), m.Mul(bx, 8));
            e.Accept(eqb);

            TraitCollector coll = new TraitCollector(factory, store, dtb, prog);
            e.Accept(coll);
            Verify("Typing/DtbArrayAccess2.txt");
        }


		[Test]
        [Ignore("Frame pointers require escape and alias analysis.")]
		public void DtbFramePointer()
		{
			ProgramMock mock = new Mocks.ProgramMock();
			mock.Add(new FramePointerMock(factory));
			RunTest(mock, "Typing/DtbFramePointer.txt");
            throw new NotImplementedException();
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
            TraitCollector trco = new TraitCollector(factory, store, dtb, prog);
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
            TraitCollector trco = new TraitCollector(factory, store, dtb, prog);
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
            TraitCollector trco = new TraitCollector(factory, store, dtb, prog);
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
			Identifier i = new Identifier("i", 0, PrimitiveType.Word32, null);
			MemoryAccess load = new MemoryAccess(MemoryIdentifier.GlobalMemory, i, PrimitiveType.Int32);
			Identifier i2 = new Identifier("i2", 1, PrimitiveType.Word32, null);
			MemoryAccess ld2 = new MemoryAccess(MemoryIdentifier.GlobalMemory, i2, PrimitiveType.Int32);

			LinearInductionVariable iv = new LinearInductionVariable(
				Constant.Word32(0), 
				Constant.Word32(1),
				Constant.Word32(10),
                false);
			LinearInductionVariable iv2 = new LinearInductionVariable(
				Constant.Word32(0x0010000),
				Constant.Word32(4),
				Constant.Word32(0x0010040),
                false);

			prog.InductionVariables.Add(i, iv);
            prog.InductionVariables.Add(i2, iv2);
            TraitCollector trco = new TraitCollector(factory, store, dtb, prog);

			prog.Globals.Accept(eqb);
			load.Accept(eqb);
			ld2.Accept(eqb);
			prog.Globals.Accept(trco);
			load.Accept(trco);
			ld2.Accept(trco);
			dtb.BuildEquivalenceClassDataTypes();

			Verify("Typing/DtbInductionVariables.txt");
		}

		[Test]
		public void DtbBuildEqClassDataTypes()
		{
			TypeVariable tv1 = store.EnsureExpressionTypeVariable(factory, null);
			tv1.OriginalDataType = PrimitiveType.Word32;
			TypeVariable tv2 = store.EnsureExpressionTypeVariable(factory, null);
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
			Identifier i = m.Local32("i");
			Expression ea = m.Add(prog.Globals, m.Add(m.Shl(i, 2), 0x3000));
			Expression e = m.Load(PrimitiveType.Int32, ea);
			TraitCollector trco = new TraitCollector(factory, store, dtb, prog);
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
            RunTest(RewriteFile("Fragments/fpuops.asm"), "Typing/DtbReals.txt");
		}

		[Test]
		public void DtbSegmentedAccess()
		{
			ProcedureMock m = new ProcedureMock();

			Identifier ds = m.Local16("ds");
			Identifier bx = m.Local16("bx");
			Expression e = m.SegMem(bx.DataType, ds, m.Add(bx, 4));
            Program prog = new Program();
            prog.Architecture = new Decompiler.Arch.Intel.IntelArchitecture(Decompiler.Arch.Intel.ProcessorMode.Real);
			TraitCollector trco = new TraitCollector(factory, store, dtb, prog);
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
            Program prog = new Program();
            prog.Architecture = new Decompiler.Arch.Intel.IntelArchitecture(Decompiler.Arch.Intel.ProcessorMode.Real);
            store.EnsureExpressionTypeVariable(factory, prog.Globals);

			Identifier ds = m.Local16("ds");
			Expression e = m.SegMem(PrimitiveType.Byte, ds, m.Int16(0x0200));
			
			TraitCollector coll = new TraitCollector(factory, store, dtb, prog);
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
			RunTest("fragments/regressions/r00008.asm", "Typing/DtbReg00008.txt");
		}

        [Test]
        public void DtbReg00011()
        {
            RunTest("fragments/regressions/r00011.asm", "Typing/DtbReg00011.txt");
        }

        [Test]
        public void DtbReg00012()
        {
            RunTest("fragments/regressions/r00012.asm", "Typing/DtbReg00012.txt");
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

		[Test]
		public void DtbFn1CallFn2()
		{
			ProgramMock pp = new ProgramMock();
			ProcedureMock m = new ProcedureMock("Fn1");
			Identifier loc1 = m.Local32("loc1");
			Identifier loc2 = m.Local32("loc2");
			m.Assign(loc2, m.Fn("Fn2", loc1));
			m.Return();
			m.Procedure.RenumberBlocks();
			pp.Add(m);

			m = new ProcedureMock("Fn2");
			Identifier arg1 = m.Local32("arg1");
			Identifier ret = m.Register(1);
			m.Procedure.Signature = new ProcedureSignature(ret, new Identifier[] { arg1 });
			m.Procedure.Signature.FormalArguments[0] = arg1;
			m.Assign(ret, m.Add(arg1, 1));
			m.Return(ret);
			m.Procedure.RenumberBlocks();
			pp.Add(m);

			RunTest(pp.BuildProgram(), "Typing/DtbFn1CallFn2.txt");
		}

		[Test]
		public void DtbStructurePointerPassedToFunction()
		{
			ProgramMock pp = new ProgramMock();
			
			ProcedureMock m = new ProcedureMock("Fn1");
			Identifier p = m.Local32("p");
			m.Store(m.Add(p, 4), m.Word32(0x42));
			m.SideEffect(m.Fn("Fn2", p));
			m.Return();
			m.Procedure.RenumberBlocks();
			pp.Add(m);

			m = new ProcedureMock("Fn2");
			Identifier arg1 = m.Local32("arg1");
			m.Procedure.Signature = new ProcedureSignature(null, new Identifier[] { arg1 });
			m.Store(m.Add(arg1, 8), m.Int32(0x23));
			m.Return();
			m.Procedure.RenumberBlocks();
			pp.Add(m);

			RunTest(pp.BuildProgram(), "Typing/DtbStructurePointerPassedToFunction.txt");
		}


        [Test]
        public void DtbSignedCompare()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier p = m.Local32("p");
            Identifier ds = m.Local16("ds");
            ds.DataType = PrimitiveType.SegmentSelector;
            Identifier ds2 = m.Local16("ds2");
            ds.DataType = PrimitiveType.SegmentSelector;
            m.Assign(ds2, ds);
            m.Store(
                m.SegMem(PrimitiveType.Bool, ds, m.Word16(0x5400)),
                m.Lt(m.SegMemW(ds, m.Word16(0x5404)), m.Word16(20)));
            m.Store(m.SegMemW(ds2, m.Word16(0x5404)), m.Word16(0));

            ProgramMock prog = new ProgramMock();
            prog.Add(m);
            RunTest(prog.BuildProgram(), "Typing/DtbSignedCompare.txt");

        }

        [Test]
        public void DtbSequenceWithSegment()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier ds = m.Local16("ds");
            ds.DataType = PrimitiveType.SegmentSelector;
            m.Store(m.Word16(0x0100), m.Seq(ds, m.Word16(0x1234)));

            ProgramMock prog = new ProgramMock();
            prog.Add(m);
            RunTest(prog.BuildProgram(), "Typing/DtbSequenceWithSegment.txt");
        }

		protected override void RunTest(Program prog, string outputFile)
		{
			aen.Transform(prog);
			eqb.Build(prog);
			TraitCollector trco = new TraitCollector(factory, store, dtb, prog);
			trco.CollectProgramTraits(prog);
			dtb.BuildEquivalenceClassDataTypes();
			Verify(prog, outputFile);
		}

		private void Verify(string outputFile)
		{
			Verify(null, outputFile);
		}

		private void Verify(Program prog, string outputFile)
		{
			store.CopyClassDataTypesToTypeVariables();
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				if (prog != null)
				{
					foreach (Procedure proc in prog.Procedures.Values)
					{
						proc.Write(false, fut.TextWriter);
						fut.TextWriter.WriteLine();
					}
				}
				store.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}
