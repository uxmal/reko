#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Fragments;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Reko.Core.Analysis;

namespace Reko.UnitTests.Decompiler.Typing
{
    [TestFixture]
    public class DataTypeBuilderTests : TypingTestBase
    {
        private TypeFactory factory;
        private TypeStore store;
        private ExpressionNormalizer aen;
        private EquivalenceClassBuilder eqb;
        private DataTypeBuilder dtb;
        private FakeArchitecture arch;
        private Program program;
        private FakeDecompilerEventListener listener;

        [SetUp]
        public void SetUp()
        {
            store = new TypeStore();
            factory = new TypeFactory();
            listener = new FakeDecompilerEventListener();
            aen = new ExpressionNormalizer(PrimitiveType.Ptr32);
            eqb = new EquivalenceClassBuilder(factory, store, listener);
            arch = new FakeArchitecture(new ServiceContainer());
            program = new Program();
            program.Architecture = arch;
            program.Platform = new DefaultPlatform(null, arch);
            dtb = new DataTypeBuilder(factory, store, program.Platform);
        }

        protected override void RunTest(Program program, string outputFile)
        {
            aen.Transform(program);
            eqb.Build(program);
            TypeCollector trco = new TypeCollector(factory, store, program, listener);
            trco.CollectTypes();
            dtb.BuildEquivalenceClassDataTypes();
            Verify(program, outputFile);
        }

        private TypeVariable TypeVar(Expression e) => store.GetTypeVariable(e);

        private void Verify(string outputFile)
        {
            Verify(null, outputFile);
        }

        private void Verify(Program program, string outputFile)
        {
            store.CopyClassDataTypesToTypeVariables();
            using (FileUnitTester fut = new FileUnitTester(outputFile))
            {
                if (program is not null)
                {
                    foreach (Procedure proc in program.Procedures.Values)
                    {
                        proc.Write(false, fut.TextWriter);
                        fut.TextWriter.WriteLine();
                    }
                }
                store.Write(false, fut.TextWriter);
                fut.AssertFilesEqual();
            }
        }

        [Test]
        public void DtbArrayAccess()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new ArrayAccess());
            RunTest(mock, "Typing/DtbArrayAccess.txt");
        }

        private class ArrayAccess : ProcedureBuilder
        {
            protected override void BuildBody()
            {
                Identifier i = Local32("i");
                Identifier r = Local32("r");
                Assign(Frame.EnsureRegister(Architecture.StackRegister), Frame.FramePointer);
                MStore(IAdd(IAdd(r, 20), SMul(i, 10)), Int32(0));
                Return(Mem(PrimitiveType.Word16,
                    IAdd(IAdd(r, 16), SMul(i, 10))));
            }
        }

        [Test]
        public void DtbArrayLoopMock()
        {
            var pb = new Mocks.ProgramBuilder();
            pb.Add(new ArrayLoopMock());
            RunTest(pb, "Typing/DtbArrayLoopMock.txt");
        }

        [Test]
        public void DtbGlobalVariables()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new GlobalVariablesMock());
            RunTest(mock, "Typing/DtbGlobalVariables.txt");
        }

        [Test]
        public void DtbEqClass()
        {
            Identifier id1 = new Identifier("foo", PrimitiveType.Word32, null);
            Identifier id2 = new Identifier("bar", PrimitiveType.Real32, null);
            id1.Accept(eqb);
            id2.Accept(eqb);
            store.MergeClasses(TypeVar(id1), TypeVar(id2));

            dtb.DataTypeTrait(TypeVar(id1), id1.DataType);
            dtb.DataTypeTrait(TypeVar(id2), id2.DataType);
            dtb.BuildEquivalenceClassDataTypes();

            IList<EquivalenceClass> used = store.UsedEquivalenceClasses;
            Assert.AreEqual(1, used.Count);
            Verify("Typing/DtbEqClass.txt");
        }

        [Test]
        public void DtbEqClassType()
        {
            Identifier id1 = new Identifier("foo", PrimitiveType.Word32, null);
            Identifier id2 = new Identifier("bar", PrimitiveType.Real32, null);
            id1.Accept(eqb);
            id2.Accept(eqb);
            store.MergeClasses(TypeVar(id1), TypeVar(id2));

            dtb.DataTypeTrait(TypeVar(id1), id1.DataType);
            dtb.DataTypeTrait(TypeVar(id2), id2.DataType);
            dtb.BuildEquivalenceClassDataTypes();

            EquivalenceClass e = TypeVar(id1).Class;
            PrimitiveType p = (PrimitiveType) e.DataType;
            Assert.AreEqual(PrimitiveType.Real32, p);

            Verify("Typing/DtbEqClassType.txt");
        }

        [Test]
        public void DtbArrayAccess2()
        {
            ProcedureBuilder m = new ProcedureBuilder();
            Identifier ds = m.Local(PrimitiveType.SegmentSelector, "ds");
            Identifier bx = m.Local16("bx");
            Expression e = m.Array(PrimitiveType.Word32, m.Seq(ds, m.Word16(0x300)), m.IMul(bx, 8));
            e.Accept(eqb);

            TraitCollector coll = new TraitCollector(factory, store, dtb, program);
            e.Accept(coll);
            Verify("Typing/DtbArrayAccess2.txt");
        }

        [Test]
        public void DtbFnPointerMock()
        {
            //$TODO: find out where pfn_0 is comingfrom.
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new FnPointerFragment());
            RunTest(mock, "Typing/DtbFnPointerMock.txt");
        }

        [Test]
        public void DtbSegMem3()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new SegMem3Mock());
            RunTest(mock.BuildProgram(), "Typing/DtbSegMem3.txt");
        }

        [Test]
        public void DtbMems()
        {
            Identifier foo = new Identifier("foo", PrimitiveType.Word32, null);
            Identifier bar = new Identifier("bar", PrimitiveType.Word16, null);
            Identifier baz = new Identifier("baz", PrimitiveType.Word32, null);
            Identifier fred = new Identifier("fred", PrimitiveType.Word32, null);
            Assignment ass1 = new Assignment(bar, MemLoad(foo, 4, PrimitiveType.Word16));
            Assignment ass2 = new Assignment(baz, MemLoad(foo, 6, PrimitiveType.Word32));
            Assignment ass3 = new Assignment(fred, MemLoad(baz, 0, PrimitiveType.Word32));
            ass1.Accept(eqb);
            ass2.Accept(eqb);
            ass3.Accept(eqb);
            TraitCollector trco = new TraitCollector(factory, store, dtb, program);
            trco.VisitAssignment(ass1);
            trco.VisitAssignment(ass2);
            trco.VisitAssignment(ass3);
            dtb.BuildEquivalenceClassDataTypes();

            Verify("Typing/DtbMems.txt");
        }

        [Test]
        public void DtbRepeatedLoads()
        {
            Identifier pfoo = new Identifier("pfoo", PrimitiveType.Word32, null);
            Identifier x = new Identifier("x", PrimitiveType.Word32, null);
            Assignment ass1 = new Assignment(x, MemLoad(pfoo, 4, PrimitiveType.Word32));
            Assignment ass2 = new Assignment(x, MemLoad(pfoo, 4, PrimitiveType.Word32));
            ass1.Accept(eqb);
            ass2.Accept(eqb);
            TraitCollector trco = new TraitCollector(factory, store, dtb, program);
            trco.VisitAssignment(ass1);
            trco.VisitAssignment(ass2);
            dtb.BuildEquivalenceClassDataTypes();

            Verify("Typing/DtbRepeatedLoads.txt");
        }

        [Test]
        public void DtbSameMemFetch()
        {
            Identifier foo = new Identifier("foo", PrimitiveType.Word32, null);
            Identifier bar = new Identifier("bar", PrimitiveType.Word16, null);
            Identifier baz = new Identifier("baz", PrimitiveType.Word16, null);
            Assignment ass1 = new Assignment(bar, MemLoad(foo, 4, PrimitiveType.Word16));
            Assignment ass2 = new Assignment(baz, MemLoad(foo, 4, PrimitiveType.Word16));
            ass1.Accept(eqb);
            ass2.Accept(eqb);
            TraitCollector trco = new TraitCollector(factory, store, dtb, program);
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
            Identifier i = new Identifier("i", PrimitiveType.Word32, null);
            MemoryAccess load = new MemoryAccess(MemoryStorage.GlobalMemory, i, PrimitiveType.Int32);
            Identifier i2 = new Identifier("i2", PrimitiveType.Word32, null);
            MemoryAccess ld2 = new MemoryAccess(MemoryStorage.GlobalMemory, i2, PrimitiveType.Int32);

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

            program.InductionVariables.Add(i, iv);
            program.InductionVariables.Add(i2, iv2);
            program.Platform = new DefaultPlatform(null, arch);
            TraitCollector trco = new TraitCollector(factory, store, dtb, program);

            program.Globals.Accept(eqb);
            load.Accept(eqb);
            ld2.Accept(eqb);
            program.Globals.Accept(trco);
            load.Accept(trco);
            ld2.Accept(trco);
            dtb.BuildEquivalenceClassDataTypes();

            Verify("Typing/DtbInductionVariables.txt");
        }

        [Test]
        public void DtbBuildEqClassDataTypes()
        {
            TypeVariable tv1 = store.CreateTypeVariable(factory);
            tv1.OriginalDataType = PrimitiveType.Word32;
            TypeVariable tv2 = store.CreateTypeVariable(factory);
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
            dtb.DataTypeTrait(TypeVar(e1), e1.DataType);
            dtb.BuildEquivalenceClassDataTypes();
            Verify("Typing/DtbTypeVariable.txt");
        }

        [Test]
        public void DtbUnion()
        {
            Identifier id1 = new Identifier("foo", PrimitiveType.Int32, null);		// note signed: can't be unified with real
            Identifier id2 = new Identifier("bar", PrimitiveType.Real32, null);
            id1.Accept(eqb);
            id2.Accept(eqb);
            store.MergeClasses(TypeVar(id1), TypeVar(id2));

            dtb.DataTypeTrait(TypeVar(id1), id1.DataType);
            dtb.DataTypeTrait(TypeVar(id2), id2.DataType);
            dtb.BuildEquivalenceClassDataTypes();

            UnionType u = (UnionType) TypeVar(id1).Class.DataType;
            Assert.AreEqual(2, u.Alternatives.Count);
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void DtbReals()
        {
            RunTest(RewriteFile16("Fragments/fpuops.asm"), "Typing/DtbReals.txt");
        }

        [Test]
        public void DtbSegmentedAccess()
        {
            ProcedureBuilder m = new ProcedureBuilder();

            Identifier ds = m.Local16("ds");
            Identifier bx = m.Local16("bx");
            Expression e = m.SegMem(bx.DataType, ds, m.IAdd(bx, 4));
            var sc = new ServiceContainer();
            var arch = new Reko.Arch.X86.X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            Program program = new Program
            {
                Architecture = arch,
                Platform = new DefaultPlatform(null, arch),
            };
            TraitCollector trco = new TraitCollector(factory, store, dtb, program);
            e = e.Accept(aen);
            e.Accept(eqb);
            e.Accept(trco);
            dtb.BuildEquivalenceClassDataTypes();
            Verify("Typing/DtbSegmentedAccess.txt");
        }

        [Test]
        public void DtbSegmentedPointer()
        {
            var m = new ProgramBuilder();
            m.Add(new SegmentedPointerProc());
            RunTest(m.BuildProgram(), "Typing/DtbSegmentedPointer.txt");
        }

        [Test]
        public void DtbSegmentedDirectAddress()
        {
            ProcedureBuilder m = new ProcedureBuilder();
            var sc = new ServiceContainer();
            var arch = new Reko.Arch.X86.X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            var program = new Program
            {
                Architecture = arch,
                Platform = new DefaultPlatform(sc, arch)
            };
            store.EnsureExpressionTypeVariable(factory, null, program.Globals);

            Identifier ds = m.Local16("ds");
            Expression e = m.SegMem(PrimitiveType.Byte, ds, m.Word16(0x0200));

            var coll = new TraitCollector(factory, store, dtb, program);
            e = e.Accept(aen);
            e.Accept(eqb);
            e.Accept(coll);
            dtb.BuildEquivalenceClassDataTypes();
            Verify("Typing/DtbSegmentedDirectAddress.txt");
        }

        [Test]
        public void DtbSegmentedDoubleReference()
        {
            ProgramBuilder m = new ProgramBuilder();
            m.Add(new SegmentedDoubleReferenceMock());
            RunTest(m, "Typing/DtbSegmentedDoubleReference.txt");
        }

        [Test]
        public void DtbReg00008()
        {
            RunTest16("Fragments/regressions/r00008.asm", "Typing/DtbReg00008.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void DtbReg00011()
        {
            RunTest16("Fragments/regressions/r00011.asm", "Typing/DtbReg00011.txt");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void DtbReg00012()
        {
            RunTest16("Fragments/regressions/r00012.asm", "Typing/DtbReg00012.txt");
        }

        [Test]
        public void DtbTreeFind()
        {
            ProgramBuilder m = new ProgramBuilder();
            m.Add(new TreeFindMock());
            RunTest(m, "Typing/DtbTreeFind.txt");
        }

        [Test]
        public void DtbSegmentedMemoryPointer()
        {
            ProgramBuilder m = new ProgramBuilder();
            m.Add(new SegmentedMemoryPointerMock());
            RunTest(m.BuildProgram(), "Typing/DtbSegmentedMemoryPointer.txt");
        }

        [Test]
        public void DtbFn1CallFn2()
        {
            ProgramBuilder pp = new ProgramBuilder();
            pp.Add("Fn1", m =>
            {
                Identifier loc1 = m.Local32("loc1");
                Identifier loc2 = m.Local32("loc2");
                m.Assign(loc2, m.Fn("Fn2", loc1));
                m.Return();
            });
            pp.Add("Fn2", m =>
            {
                Identifier arg1 = m.Local32("arg1");
                Identifier ret = m.Register("r1");
                m.Procedure.Signature = FunctionType.Create(ret, arg1);
                m.Procedure.Signature.Parameters[0] = arg1;
                m.Assign(ret, m.IAdd(arg1, 1));
                m.Return(ret);
            });
            RunTest(pp.BuildProgram(), "Typing/DtbFn1CallFn2.txt");
        }

        [Test]
        public void DtbStructurePointerPassedToFunction()
        {
            ProgramBuilder pp = new ProgramBuilder();
            pp.Add("Fn1", m =>
            {
                Identifier p = m.Local32("p");
                m.MStore(m.IAdd(p, 4), m.Word32(0x42));
                m.SideEffect(m.Fn("Fn2", p));
                m.Return();
            });

            pp.Add("Fn2", m =>
            {
                Identifier arg1 = m.Local32("arg1");
                m.Procedure.Signature = FunctionType.Action(arg1);
                m.MStore(m.IAdd(arg1, 8), m.Word32(0x23));
                m.Return();
            });
            RunTest(pp.BuildProgram(), "Typing/DtbStructurePointerPassedToFunction.txt");
        }


        [Test]
        public void DtbSignedCompare()
        {
            ProcedureBuilder m = new ProcedureBuilder();
            Identifier ds = m.Local16("ds");
            ds.DataType = PrimitiveType.SegmentSelector;
            Identifier ds2 = m.Local16("ds2");
            ds.DataType = PrimitiveType.SegmentSelector;
            m.Assign(ds2, ds);
            m.Store(
                m.SegMem(PrimitiveType.Bool, ds, m.Word16(0x5400)),
                m.Lt(m.SegMem16(ds, m.Word16(0x5404)), m.Word16(20)));
            m.Store(m.SegMem16(ds2, m.Word16(0x5404)), m.Word16(0));

            ProgramBuilder program = new ProgramBuilder();
            program.Add(m);
            RunTest(program.BuildProgram(), "Typing/DtbSignedCompare.txt");
        }

        [Test]
        public void DtbSequenceWithSegment()
        {
            ProcedureBuilder m = new ProcedureBuilder();
            Identifier ds = m.Local16("ds");
            ds.DataType = PrimitiveType.SegmentSelector;
            m.SStore(ds, m.Word16(0x0100), m.Seq(ds, m.Word16(0x1234)));

            ProgramBuilder pb = new ProgramBuilder();
            pb.Add(m);
            RunTest(pb.BuildProgram(), "Typing/DtbSequenceWithSegment.txt");
        }


        [Test]
        public void DtbCallTable()
        {
            var pb = new ProgramBuilder();
            pb.Add(new IndirectCallFragment());
            RunTest(pb.BuildProgram(), "Typing/DtbCallTable.txt");
        }
    }
}
