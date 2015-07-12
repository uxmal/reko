#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.Typing;
using Decompiler.UnitTests.Fragments;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
    [TestFixture]
    public class TypedExpressionRewriterTests : TypingTestBase
    {
        private TypedExpressionRewriter ter;
        private ExpressionNormalizer aen;
        private EquivalenceClassBuilder eqb;
        private TraitCollector coll;
        private DataTypeBuilder dtb;
        private TypeVariableReplacer tvr;
        private TypeTransformer trans;
        private ComplexTypeNamer ctn;

        protected override void RunTest(Program program, string outputFile)
        {
            using (FileUnitTester fut = new FileUnitTester(outputFile))
            {
                fut.TextWriter.WriteLine("// Before ///////");
                DumpProgram(program, fut);

                SetupPreStages(program);
                aen.Transform(program);
                eqb.Build(program);
#if OLD
                coll = new TraitCollector(factory, store, dtb, program);
                coll.CollectProgramTraits(program);
#else
                var coll = new TypeCollector(program.TypeFactory, program.TypeStore, program);
                coll.CollectTypes();
#endif
                program.TypeStore.BuildEquivalenceClassDataTypes(program.TypeFactory);
                program.TypeStore.Dump();
                tvr.ReplaceTypeVariables();
                trans.Transform();
                ctn.RenameAllTypes(program.TypeStore);

                ter = new TypedExpressionRewriter(program);
                try
                {
                    ter.RewriteProgram(program);
                }
                catch (Exception ex)
                {
                    fut.TextWriter.WriteLine("** Exception **");
                    fut.TextWriter.WriteLine(ex);
                }
                finally
                {
                    fut.TextWriter.WriteLine("// After ///////");
                    DumpProgAndStore(program, fut);
                }
            }
        }

        private ProgramBuilder CreateProgramBuilder(uint linearAddress, int size)
        {
            return new ProgramBuilder(
                new LoadedImage(Address.Ptr32(linearAddress), new byte[size]));
        }

        private void DumpProgram(Program program, FileUnitTester fut)
        {
            foreach (Procedure proc in program.Procedures.Values)
            {
                proc.Write(false, fut.TextWriter);
                fut.TextWriter.WriteLine();
            }
        }

        private void DumpProgAndStore(Program prog, FileUnitTester fut)
        {
            foreach (Procedure proc in prog.Procedures.Values)
            {
                proc.Write(false, fut.TextWriter);
                fut.TextWriter.WriteLine();
            }

            prog.TypeStore.Write(fut.TextWriter);
            fut.AssertFilesEqual();
        }

        public void SetupPreStages(Program prog)
        {
            aen = new ExpressionNormalizer(prog.Platform.PointerType);
            eqb = new EquivalenceClassBuilder(prog.TypeFactory, prog.TypeStore);
            dtb = new DataTypeBuilder(prog.TypeFactory, prog.TypeStore, prog.Platform);
            tvr = new TypeVariableReplacer(prog.TypeStore);
            trans = new TypeTransformer(prog.TypeFactory, prog.TypeStore, prog);
            ctn = new ComplexTypeNamer();
        }

        private void SetType(Expression e, DataType t)
        {
            e.DataType = t;
        }

        [Test]
        public void TerComplex()
        {
            Program program = new Program();
            program.Architecture = new FakeArchitecture();
            program.Platform = new DefaultPlatform(null, program.Architecture);
            SetupPreStages(program);
            Identifier id = new Identifier("v0", PrimitiveType.Word32, null);
            Expression cmp = MemLoad(id, 4, PrimitiveType.Word32);

            program.Globals.Accept(eqb);
            cmp.Accept(aen);
            cmp.Accept(eqb);
            coll = new TraitCollector(program.TypeFactory, program.TypeStore, dtb, program);
            cmp.Accept(coll);
            dtb.BuildEquivalenceClassDataTypes();

            tvr.ReplaceTypeVariables();
            trans.Transform();
            ctn.RenameAllTypes(program.TypeStore);

            ter = new TypedExpressionRewriter(program);
            cmp = cmp.Accept(ter);
            Assert.AreEqual("v0->dw0004", cmp.ToString());
        }

        [Test]
        public void TerPtrPtrInt()
        {
            ProgramBuilder mock = CreateProgramBuilder(0x0010000, 0x1000);
            mock.Add(new PtrPtrIntMock());
            RunTest(mock.BuildProgram(), "Typing/TerPtrPtrInt.txt");
        }

        [Test]
        public void TerUnionIntReal()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new UnionIntRealMock());
            RunTest(mock.BuildProgram(), "Typing/TerUnionIntReal.txt");
        }

        [Test]
        public void TerConstantUnion()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new ConstantUnionMock());
            RunTest(mock.BuildProgram(), "Typing/TerConstantUnion.txt");
        }

        [Test]
        public void TerConstants()
        {
            Program prog = new Program();
            prog.Architecture = new FakeArchitecture();
            prog.Platform = new DefaultPlatform(null, prog.Architecture);
            SetupPreStages(prog);
            Constant r = Constant.Real32(3.0F);
            Constant i = Constant.Int32(1);
            Identifier x = new Identifier("x", PrimitiveType.Word32, null);
            Assignment ass = new Assignment(x, r);
            TypeVariable tvR = r.TypeVariable = prog.TypeFactory.CreateTypeVariable();
            TypeVariable tvI = i.TypeVariable = prog.TypeFactory.CreateTypeVariable();
            TypeVariable tvX = x.TypeVariable = prog.TypeFactory.CreateTypeVariable();
            prog.TypeStore.TypeVariables.AddRange(new TypeVariable[] { tvR, tvI, tvX });
            UnionType u = prog.TypeFactory.CreateUnionType(null, null, new DataType[] { r.DataType, i.DataType });
            tvR.OriginalDataType = r.DataType;
            tvI.OriginalDataType = i.DataType;
            tvX.OriginalDataType = x.DataType;
            tvR.DataType = u;
            tvI.DataType = u;
            tvX.DataType = u;
            ctn.RenameAllTypes(prog.TypeStore);
            TypedExpressionRewriter ter = new TypedExpressionRewriter(prog);
            Instruction instr = ter.TransformAssignment(ass);
            Assert.AreEqual("x.u1 = 3F;", instr.ToString());
        }

        [Test]
        public void TerVector()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new VectorFragment());
            RunTest(mock.BuildProgram(), "Typing/TerVector.txt");
        }

        [Test]
        public void TerGlobalVariables()
        {
            ProgramBuilder mock = CreateProgramBuilder(0x10000000, 0x1000);
            mock.Add(new GlobalVariablesMock());
            RunTest(mock.BuildProgram(), "Typing/TerGlobalVariables.txt");
        }

        [Test]
        public void TerSegmentedMemoryPointer()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new SegmentedMemoryPointerMock());
            RunTest(mock.BuildProgram(), "Typing/TerSegmentedMemoryPointer.txt");
        }

        [Test]
        public void TerSegMemPtr2()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new SegmentedMemoryPointerMock2());
            RunTest(mock.BuildProgram(), "Typing/TerSegMemPtr2.txt");
        }

        [Test]
        public void TerSegMem3()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new SegMem3Mock());
            RunTest(mock.BuildProgram(), "Typing/TerSegMem3.txt");
        }

        [Test]
        public void TerArrayConstantPointers()
        {
            ProgramBuilder pp = new ProgramBuilder(new LoadedImage(Address.Ptr32(0x00123000), new byte[4000]));
            pp.Add("Fn", m =>
            {
                Identifier a = m.Local32("a");
                Identifier i = m.Local32("i");
                m.Assign(a, 0x00123456);		// array pointer
                m.Store(m.IAdd(a, m.IMul(i, 8)), m.Int32(42));
            });
            RunTest(pp.BuildProgram(), "Typing/TerArrayConstantPointers.txt");
        }

        [Test]
        public void TerReg00008()
        {
            RunTest16("Fragments/regressions/r00008.asm", "Typing/TerReg00008.txt");
        }

        [Test]
        public void TerReg00011()
        {
            RunTest16("Fragments/regressions/r00011.asm", "Typing/TerReg00011.txt");
        }

        [Test]
        public void TerReg00012()
        {
            RunTest16("Fragments/regressions/r00012.asm", "Typing/TerReg00012.txt");
        }

        [Test]
        public void TerAddNonConstantToPointer()
        {
            ProgramBuilder prog = new ProgramBuilder();
            prog.Add("proc1", m =>
            {
                Identifier i = m.Local16("i");
                Identifier p = m.Local16("p");

                m.Store(p, m.Word16(4));
                m.Store(m.IAdd(p, 4), m.Word16(4));
                m.Assign(p, m.IAdd(p, i));

            });
            RunTest(prog.BuildProgram(), "Typing/TerAddNonConstantToPointer.txt");
        }

        [Test]
        public void TerSignedCompare()
        {
            ProgramBuilder prog = new ProgramBuilder();
            prog.Add("proc1", m =>
            {
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
            });
            RunTest(prog.BuildProgram(), "Typing/TerSignedCompare.txt");
        }

        [Test]
        public void TerDereferenceSignedCompare()
        {
            ProgramBuilder prog = CreateProgramBuilder(0x5000, 0x1000);
            prog.Add("proc1", m =>
            {
                Identifier p = m.Local32("p");
                Identifier ds = m.Local16("ds");
                ds.DataType = PrimitiveType.SegmentSelector;
                Identifier ds2 = m.Local16("ds2");
                ds2.DataType = PrimitiveType.SegmentSelector;
                m.Assign(ds2, ds);
                m.Store(
                    m.SegMem(PrimitiveType.Bool, ds, m.Word16(0x5400)),
                    m.Lt(
                        m.SegMemW(ds, m.IAdd(m.SegMemW(ds, m.Word16(0x5404)), 4)),
                        m.Word16(20)));
                m.Store(m.SegMemW(ds2, m.IAdd(m.SegMemW(ds2, m.Word16(0x5404)), 4)), m.Word16(0));
                m.Return();
            });
            RunTest(prog.BuildProgram(), "Typing/TerDereferenceSignedCompare.txt");
        }

        [Test]
        public void TerFlatDereferenceSignedCompare()
        {
            ProgramBuilder prog = CreateProgramBuilder(0x5400, 0x1000);
            prog.Add("proc1", m =>
            {
                Identifier ds = m.Local32("ds");
                Identifier ds2 = m.Local32("ds2");
                m.Assign(ds2, ds);
                m.Store(
                    m.IAdd(ds, m.Word32(0x5400)),
                    m.Lt(
                        m.LoadW(m.IAdd(m.LoadDw(m.IAdd(ds, m.Word32(0x5404))), 4)),
                        m.Word16(20)));
                m.Store(m.IAdd(m.LoadDw(m.IAdd(ds2, m.Word32(0x5404))), 4), m.Word16(0));
            });
            RunTest(prog.BuildProgram(), "Typing/TerFlatDereferenceSignedCompare.txt");
        }

        [Test]
        public void TerComparison()
        {
            ProgramBuilder prog = new ProgramBuilder(new LoadedImage(Address.Ptr32(0x00100000), new byte[0x4000]));
            prog.Add("proc1", m =>
            {
                Identifier p = m.Local32("p");
                Expression fetch = m.Load(new Pointer(new StructureType("foo", 8), 4), m.IAdd(p, 4));
                m.Assign(m.LocalBool("f"), m.Lt(fetch, m.Word32(0x00100028)));
            });
            RunTest(prog.BuildProgram(), "Typing/TerComparison.txt");
        }

        [Test]
        public void TerUnionConstants()
        {
            ProgramBuilder prog = new ProgramBuilder();
            prog.Add("proc1", m =>
            {
                Identifier bx = m.Local16("bx");
                m.Assign(bx, m.Shr(bx, 2));     // makes bx unsigned uint16
                m.Assign(m.LocalBool("f"), m.Lt(bx, 4));    // makes bx also signed; assembler bug, but forces a union.
                m.Assign(bx, m.Word16(4));          // what type should 4 have?
            });
            RunTest(prog.BuildProgram(), "Typing/TerUnionConstants.txt");
        }

        [Test]
        public void TerOffsetInArrayLoop()
        {
            ProgramBuilder pm = new ProgramBuilder();
            pm.Add("proc1", m =>
            {
                var ds = m.Local16("ds");
                var cx = m.Local16("cx");
                var di = m.Local16("di");
                m.Assign(di, 0);
                m.Label("lupe");
                m.SegStore(ds, m.IAdd(di, 0x5388), m.Word16(0));
                m.Assign(di, m.IAdd(di, 2));
                m.Assign(cx, m.ISub(cx, 1));
                m.BranchIf(m.Ne(cx, 0), "lupe");
                m.Return();
            });
            RunTest(pm, "Typing/TerOffsetInArrayLoop.txt");
        }

        [Test]
        public void TerSegmentedLoadLoad()
        {
            ProgramBuilder pm = new ProgramBuilder();
            pm.Add("proc1", m =>
            {
                var ds = m.Local(PrimitiveType.SegmentSelector, "ds");
                var bx = m.Local(PrimitiveType.Word16, "bx");
                m.SegStore(ds, m.Word16(0x300), m.SegMemW(ds, m.SegMemW(ds, bx)));
                m.Return();
            });
            RunTest(pm, "Typing/TerSegmentedLoadLoad.txt");
        }

        [Test]
        public void TerIntelIndexedAddressingMode()
        {
            ProgramBuilder m = new ProgramBuilder();
            m.Add(new IntelIndexedAddressingMode());
            RunTest(m.BuildProgram(), "Typing/TerIntelIndexedAddressingMode.txt");
        }

        [Test]
        public void TerReg00016()
        {
            RunHexTest("fragments/regressions/r00016.dchex", "Typing/TerReg00016.txt");
        }

        [Test]
        public void TerReg00017()
        {
            RunTest32("Fragments/regressions/r00017.asm", "Typing/TerReg00017.txt");
        }

        [Test]
        public void TerCallTable()
        {
            var pb = new ProgramBuilder();
            pb.Add(new IndirectCallFragment());
            RunTest(pb.BuildProgram(), "Typing/TerCallTable.txt");
        }

        [Test]
        public void TerSegmentedCall()
        {
            var pb = new ProgramBuilder();
            pb.Add(new SegmentedCallFragment());
            RunTest(pb.BuildProgram(), "Typing/TerSegmentedCall.txt");
        }

        [Test]
        public void TerPointerChain()
        {
            var pb = new ProgramBuilder();
            pb.Add(new PointerChainFragment());
            RunTest(pb.BuildProgram(), "Typing/TerPointerChain.txt");
        }

        [Test]
        public void TerStaggeredArrays()
        {
            ProgramBuilder prog = new ProgramBuilder();
            prog.Add(new StaggeredArraysFragment());
            RunTest(prog.BuildProgram(), "Typing/TerStaggeredArrays.txt");
        }

        [Test]
        public void TerDeclaration()
        {
            ProgramBuilder pm = new ProgramBuilder();
            pm.Add("proc1", m =>
            {
                var ax = m.Reg16("ax");
                var rand = new ExternalProcedure(
                    "rand",
                    new ProcedureSignature(
                        new Identifier("ax", PrimitiveType.Int16, ax.Storage),
                        new Identifier[0]));
                m.Declare(ax, m.Fn(rand));
                m.Store( m.Word16(0x300), ax);
                m.Return();
            });
            RunTest(pm, "Typing/TerDeclaration.txt");
        }
    }

	public class SegmentedMemoryPointerMock : ProcedureBuilder
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
			Store(SegMemW(cs, IAdd(si, 2)), ax);
		}
	}

	public class SegmentedMemoryPointerMock2 : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			Identifier ds = Local16("ds");
			ds.DataType = PrimitiveType.SegmentSelector;
			Identifier ax = Local16("ax");
			Identifier bx = Local16("bx");
			Assign(ax, SegMemW(ds, bx));
			Assign(ax, SegMemW(ds, IAdd(bx, 4)));
		}
	}
	
	public class SegMem3Mock : ProcedureBuilder
	{
		private Constant Seg(int seg)
		{
			return Constant.Create(PrimitiveType.SegmentSelector, seg);
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
 