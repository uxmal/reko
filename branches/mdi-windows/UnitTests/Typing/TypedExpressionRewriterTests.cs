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
        private ExpressionNormalizer aen;
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
            prog.Architecture = new ArchitectureMock();
            SetupPreStages(prog.Architecture);
			Identifier id = new Identifier("v0", 0, PrimitiveType.Word32, null);
			Expression cmp = MemLoad(id, 4, PrimitiveType.Word32);

            cmp.Accept(aen);
			cmp.Accept(eqb);
			coll = new TraitCollector(factory, store, dtb, prog);
			cmp.Accept(coll);
			dtb.BuildEquivalenceClassDataTypes();

			tvr.ReplaceTypeVariables();
			trans.Transform();
			ctn.RenameAllTypes(store);

			ter = new TypedExpressionRewriter(store, prog.Globals);
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
            prog.Architecture = new ArchitectureMock();
            SetupPreStages(prog.Architecture);
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
			TypedExpressionRewriter ter = new TypedExpressionRewriter(store, prog.Globals);
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

		[Test]
		public void TerSegMemPtr2()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new SegmentedMemoryPointerMock2());
			RunTest(mock.BuildProgram(), "Typing/TerSegMemPtr2.txt");
		}
		
		[Test]
		public void TerSegMem3()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new SegMem3Mock());
			RunTest(mock.BuildProgram(), "Typing/TerSegMem3.txt");
		}

		[Test]
        [Ignore("Need a constant pointer analysis phase")]
		public void TerArrayConstantPointers()
		{
			ProgramMock pp = new ProgramMock();

			ProcedureMock m = new ProcedureMock("Fn");
			Identifier a = m.Local32("a");
			Identifier i = m.Local32("i");
			m.Assign(a, 0x00123456);		// array pointer
			m.Store(m.Add(a, m.Mul(i, 8)), m.Int32(42));
			pp.Add(m);

			RunTest(pp.BuildProgram(), "Typing/TerArrayConstantPointers.txt");
		}

        [Test]
        public void TerReg00008()
        {
            RunTest("fragments/regressions/r00008.asm", "Typing/TerReg00008.txt");
        }

        [Test]
        public void TerReg00011()
        {
            RunTest("fragments/regressions/r00011.asm", "Typing/TerReg00011.txt");
        }

        [Test]
        public void TerReg00012()
        {
            RunTest("fragments/regressions/r00012.asm", "Typing/TerReg00012.txt");
        }

        [Test]
        public void TerAddNonConstantToPointer()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier i = m.Local16("i");
            Identifier p = m.Local16("p");

            m.Store(p, m.Word16(4));
            m.Store(m.Add(p, 4), m.Word16(4));
            m.Assign(p, m.Add(p, i));
            ProgramMock prog = new ProgramMock();
            prog.Add(m);

            RunTest(prog.BuildProgram(), "Typing/TerAddNonConstantToPointer.txt");
        }

        [Test]
        public void TerSignedCompare()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier p = m.Local32("p");
            Identifier ds = m.Local16("ds");
            ds.DataType = PrimitiveType.SegmentSelector;
            Identifier ds2 = m.Local16("ds2");
            ds2.DataType = PrimitiveType.SegmentSelector;
            m.Assign(ds2, ds);
            m.Store(
                m.SegMem(PrimitiveType.Bool, ds, m.Word16(0x5400)),
                m.Lt(m.SegMemW(ds, m.Word16(0x5404)), m.Word16(20)));
            m.Store(m.SegMemW(ds2, m.Word16(0x5404)), m.Word16(0));

            ProgramMock prog = new ProgramMock();
            prog.Add(m);
            RunTest(prog.BuildProgram(), "Typing/TerSignedCompare.txt");
        }

        [Test]
        public void TerDereferenceSignedCompare()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier p = m.Local32("p");
            Identifier ds = m.Local16("ds");
            ds.DataType = PrimitiveType.SegmentSelector;
            Identifier ds2 = m.Local16("ds2");
            ds2.DataType = PrimitiveType.SegmentSelector;
            m.Assign(ds2, ds);
            m.Store(
                m.SegMem(PrimitiveType.Bool, ds, m.Word16(0x5400)),
                m.Lt(
                    m.SegMemW(ds, m.Add(m.SegMemW(ds, m.Word16(0x5404)), 4)),
                    m.Word16(20)));
            m.Store(m.SegMemW(ds2, m.Add(m.SegMemW(ds2, m.Word16(0x5404)), 4)), m.Word16(0));

            ProgramMock prog = new ProgramMock();
            prog.Add(m);
            RunTest(prog.BuildProgram(), "Typing/TerDereferenceSignedCompare.txt");
        }

        [Test]
        public void TerFlatDereferenceSignedCompare()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier ds = m.Local32("ds");
            Identifier ds2 = m.Local32("ds2");
            m.Assign(ds2, ds);
            m.Store(
                m.Add(ds, m.Word32(0x5400)),
                m.Lt(
                    m.LoadW(m.Add(m.LoadDw(m.Add(ds, m.Word32(0x5404))), 4)),
                    m.Word16(20)));
            m.Store(m.Add(m.LoadDw(m.Add(ds2, m.Word32(0x5404))), 4), m.Word16(0));

            ProgramMock prog = new ProgramMock();
            prog.Add(m);
            RunTest(prog.BuildProgram(), "Typing/TerFlatDereferenceSignedCompare.txt");
        }

        [Test]
        public void TerComparison()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier p = m.Local32("p");
            Expression fetch = m.Load(new Pointer(new StructureType("foo", 8), 4), m.Add(p, 4));
            m.Assign(m.LocalBool("f"), m.Lt(fetch, m.Word32(3)));

            ProgramMock prog = new ProgramMock();
            prog.Add(m);
            RunTest(prog.BuildProgram(), "Typing/TerComparison.txt");
        }

        [Test]
        public void TerUnionConstants()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier bx = m.Local16("bx");
            m.Assign(bx, m.Shr(bx, 2));     // makes bx unsigned uint16
            m.Assign(m.LocalBool("f"), m.Lt(bx, 4));    // makes bx also signed; assembler bug, but forces a union.
            m.Assign(bx, m.Word16(4));          // what type should 4 have?

            ProgramMock prog = new ProgramMock();
            prog.Add(m);
            RunTest(prog.BuildProgram(), "Typing/TerUnionConstants.txt");
        }

        [Test]
        public void TerOffsetInArrayLoop()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier ds = m.Local16("ds");
            Identifier cx = m.Local16("cx");
            Identifier di = m.Local16("di");
            m.Assign(di, 0);
            m.Label("lupe");
            m.SegStoreW(ds, m.Add(di, 0x5388), m.Word16(0));
            m.Assign(di, m.Add(di, 2));
            m.Assign(cx, m.Sub(cx, 1));
            m.BranchIf(m.Ne(cx, 0), "lupe");
            m.Return();

            ProgramMock pm = new ProgramMock();
            pm.Add(m);
            RunTest(pm, "Typing/TerOffsetInArrayLoop.txt");
        }

        [Test]
        public void TerSegmentedLoadLoad()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier ds = m.Local(PrimitiveType.SegmentSelector, "ds");
            Identifier bx = m.Local(PrimitiveType.Word16, "bx");
            m.SegStoreW(ds, m.Word16(0x300), m.SegMemW(ds, m.SegMemW(ds, bx)));

            ProgramMock pm = new ProgramMock();
            pm.Add(m);
            RunTest(pm, "Typing/TerSegmentedLoadLoad.txt");
        }

        //////////////////////////////////////////////////////////////////////////


		protected override void RunTest(Program prog, string outputFile)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
                SetupPreStages(prog.Architecture);
                aen.Transform(prog);
				eqb.Build(prog);
				coll = new TraitCollector(factory, store, dtb, prog);
				coll.CollectProgramTraits(prog);
				dtb.BuildEquivalenceClassDataTypes();
				cpf.FollowConstantPointers(prog);
				tvr.ReplaceTypeVariables();
				trans.Transform();
				ctn.RenameAllTypes(store);

				ter = new TypedExpressionRewriter(store, prog.Globals);
				try
				{
					ter.RewriteProgram(prog);
				}
				catch (Exception ex)
				{
					fut.TextWriter.WriteLine("** Exception **");
					fut.TextWriter.WriteLine(ex);
				}
				finally
				{
					DumpProgAndStore(prog, fut);
				}
			}
		}

		private void DumpProgAndStore(Program prog, FileUnitTester fut)
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();
			}

			store.Write(fut.TextWriter);
			fut.AssertFilesEqual();
		}


        public void SetupPreStages(IProcessorArchitecture arch)
        {
			factory = new TypeFactory();
			store = new TypeStore();
            aen = new ExpressionNormalizer(arch.PointerType);
			eqb = new EquivalenceClassBuilder(factory, store);
            dtb = new DataTypeBuilder(factory, store, arch);
			cpf = new DerivedPointerAnalysis(factory, store, dtb, arch);
			tvr = new TypeVariableReplacer(store);
			trans = new TypeTransformer(factory, store, null);
			ctn = new ComplexTypeNamer();
		}

        private void SetType(Expression e, DataType t)
        {
            e.DataType = t;
        }
	}

	public class SegmentedMemoryPointerMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier cs = Local16("cs");
			cs.DataType = PrimitiveType.SegmentSelector;
			Identifier ax = Local16("ax");
			Identifier si = Local16("si");
			Identifier si2 = Local16("si2");
			Assign(si, Int16(0x0001));
			Assign(ax, SegMemW(cs, si));
			Assign(si2, Int16(0x0005));
			Assign(ax, SegMemW(cs, si2));
			Store(SegMemW(cs, Int16(0x1234)), ax);
			Store(SegMemW(cs, Add(si, 2)), ax);
		}
	}

	public class SegmentedMemoryPointerMock2 : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier ds = Local16("ds");
			ds.DataType = PrimitiveType.SegmentSelector;
			Identifier ax = Local16("ax");
			Identifier bx = Local16("bx");
			Assign(ax, SegMemW(ds, bx));
			Assign(ax, SegMemW(ds, Add(bx, 4)));
		}
	}

	
	public class SegMem3Mock : ProcedureMock
	{
		private Constant Seg(int seg)
		{
			return new Constant(PrimitiveType.SegmentSelector, seg);
		}

		protected override void BuildBody()
		{
			Identifier ds = base.Local(PrimitiveType.SegmentSelector, "ds");
			Identifier ax = Local16("ax");
			Identifier ds2 = base.Local(PrimitiveType.SegmentSelector, "ds2");
			
			base.Store(SegMemW(Seg(0x1796), Int16(0x0001)), Seg(0x0800));
			Store(SegMemW(Seg(0x800), Int16(0x5422)), ds);
			Store(SegMemW(Seg(0x800), Int16(0x0066)), SegMemW(Seg(0x0800), Int16(0x5420)));
		}
	}
}
 