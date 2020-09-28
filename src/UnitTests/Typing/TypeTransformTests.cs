#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Fragments;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Typing
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

        protected override void RunTest(Program program, string outputFileName)
        {
            var listener = new FakeDecompilerEventListener();
            ExpressionNormalizer aen = new ExpressionNormalizer(program.Architecture.PointerType);
            aen.Transform(program);
            EquivalenceClassBuilder eq = new EquivalenceClassBuilder(factory, store, listener);
            eq.Build(program);
#if OLD
			DataTypeBuilder dtb = new DataTypeBuilder(factory, store, program.Architecture);
			TraitCollector coll = new TraitCollector(factory, store, dtb, program);
			coll.CollectProgramTraits(program);
			sktore.BuildEquivalenceClassDataTypes(factory);
#else
            TypeCollector coll = new TypeCollector(factory, store, program, listener);
            coll.CollectTypes();

            store.BuildEquivalenceClassDataTypes(factory);
#endif

            TypeVariableReplacer tvr = new TypeVariableReplacer(store);
            tvr.ReplaceTypeVariables();

            Exception theEx = null;
            try
            {
                TypeTransformer trans = new TypeTransformer(factory, store, program);
                trans.Transform();
            } catch (Exception ex)
            {
                theEx = ex;
            }
            using (FileUnitTester fut = new FileUnitTester(outputFileName))
            {
                if (theEx != null)
                {
                    fut.TextWriter.WriteLine(theEx.Message);
                    fut.TextWriter.WriteLine(theEx.StackTrace);
                }
                foreach (Procedure proc in program.Procedures.Values)
                {
                    proc.Write(false, fut.TextWriter);
                    fut.TextWriter.WriteLine();
                }
                store.Write(fut.TextWriter);
                fut.AssertFilesEqual();
            }
        }

        [Test]
        public void TtranUnknown()
        {
            RunTest16("Fragments/type/unknown.asm", "Typing/TtranUnknown.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TtranFactorial()
        {
            RunTest16("Fragments/factorial.asm", "Typing/TtranFactorial.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TtranFactorialReg()
        {
            RunTest16("Fragments/factorial_reg.asm", "Typing/TtranFactorialReg.txt");
        }

        [Test]
        public void TtranLength()
        {
            RunTest16("Fragments/type/listlength.asm", "Typing/TtranLength.txt");
        }

        [Test]
        public void TtranIntegers()
        {
            RunTest16("Fragments/type/integraltypes.asm", "Typing/TtranIntegers.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TtranReals()
        {
            RunTest16("Fragments/fpuops.asm", "Typing/TtranReals.txt");
        }

        [Test]
        public void TtranReg00008()
        {
            RunTest16("Fragments/regressions/r00008.asm", "Typing/TtranReg00008.txt");
        }

        [Test]
        public void TtranMemAccesses()
        {
            RunTest16("Fragments/multiple/memaccesses.asm", "Typing/TtranMemAccesses.txt");
        }

        [Test]
        public void TtranPtrPtrInt()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new PtrPtrIntMock());
            RunTest(mock.BuildProgram(), "Typing/TtranPtrPtrInt.txt");
        }

        [Test]
        public void TtranSegMem3()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new SegMem3Mock());
            RunTest(mock.BuildProgram(), "Typing/TtranSegMem3.txt");
        }

        [Test]
        public void TtranGlobalVariables()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new GlobalVariablesMock());
            RunTest(mock.BuildProgram(), "Typing/TtranGlobalVariables.txt");
        }

        [Test]
        [Ignore("Frames require escape and aliasing analysis.")]
        public void TtranFramePointer()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add(new FramePointerFragment(factory));
            RunTest(pb.BuildProgram(), "Typing/TtranFramePointer.txt");
        }

        [Test]
        public void TtranRepeatedLoads()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add(new RepeatedLoadsFragment());
            RunTest(pb.BuildProgram(), "Typing/TtranRepeatedLoads.txt");
        }

        [Test]
        public void TtranStaggeredArrays()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add(new StaggeredArraysFragment());
            RunTest(pb.BuildProgram(), "Typing/TtranStaggeredArrays.txt");
        }

        [Test]
        public void TtranFnPointerMock()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add(new FnPointerFragment());
            RunTest(pb.BuildProgram(), "Typing/TtranFnPointerMock.txt");
        }

        [Test]
        public void TtranSimplify()
        {
            UnionType u = new UnionType(null, null);
            u.Alternatives.Add(new Pointer(PrimitiveType.Real32, 32));
            u.Alternatives.Add(new Pointer(PrimitiveType.Real32, 32));
            TypeTransformer trans = new TypeTransformer(factory, store, null);
            DataType dt = u.Accept(trans);
            Assert.AreEqual("(ptr32 real32)", dt.ToString());
        }

        [Test]
        public void TtranSimplify2()
        {
            UnionType u = new UnionType(null, null);
            u.Alternatives.Add(PrimitiveType.Word32);
            u.Alternatives.Add(new Pointer(PrimitiveType.Real32, 32));
            TypeTransformer trans = new TypeTransformer(factory, store, null);
            DataType dt = u.Accept(trans);
            Assert.AreEqual("(ptr32 real32)", dt.ToString());
        }

        [Test]
        public void TtranUnion()
        {
            UnionType ut = factory.CreateUnionType("foo", null);
            ut.AddAlternative(PrimitiveType.Word32);
            ut.AddAlternative(PrimitiveType.Word32);
            ut.AddAlternative(PrimitiveType.Word32);
            ut.AddAlternative(PrimitiveType.Word32);
            TypeTransformer trans = new TypeTransformer(factory, null, null);
            PrimitiveType dt = (PrimitiveType) ut.Accept(trans);
            Assert.AreEqual("word32", dt.ToString());

            ut.AddAlternative(PrimitiveType.Real32);
            ut.AddAlternative(PrimitiveType.Int32);
            DataType d = ut.Accept(trans);
            Assert.AreEqual("(union \"foo\" (int32 u0) (real32 u1))", d.ToString());
        }

        [Test]
        public void TtranUnionPointersStructures()
        {
            UnionType ut = factory.CreateUnionType("foo", null);
            var str1 = new StructureType()
            {
                Fields =
                {
                    {0, PrimitiveType.Word32},
                    {4, PrimitiveType.Int32},
                },
            };
            var str2 = new StructureType()
            {
                Fields =
                {
                    {0, PrimitiveType.Real32},
                    {4, PrimitiveType.Word32},
                },
            };
            var eq1 = new EquivalenceClass(factory.CreateTypeVariable(), str1);
            var eq2 = new EquivalenceClass(factory.CreateTypeVariable(), str2);
            ut.AddAlternative(new Pointer(eq1, 32));
            ut.AddAlternative(new Pointer(eq2, 32));
            var trans = new TypeTransformer(factory, null, null);
            var ptr = (Pointer)ut.Accept(trans);
            var eq = (EquivalenceClass)ptr.Pointee;
            Assert.AreEqual(
                "(struct (0 real32 r0000) (4 int32 dw0004))",
                eq.DataType.ToString());
        }

        [Test]
        public void TtranUnionPointersStructuresWithDifferentSizes()
        {
            UnionType ut = factory.CreateUnionType("foo", null);
            var str1 = new StructureType(12)
            {
                Fields =
                {
                    {0, PrimitiveType.Word32},
                    {4, PrimitiveType.Int32},
                },
            };
            var str2 = new StructureType(16)
            {
                Fields =
                {
                    {0, PrimitiveType.Real32},
                    {4, PrimitiveType.Word32},
                },
            };
            var eq1 = new EquivalenceClass(factory.CreateTypeVariable(), str1);
            var eq2 = new EquivalenceClass(factory.CreateTypeVariable(), str2);
            ut.AddAlternative(new Pointer(eq1, 32));
            ut.AddAlternative(new Pointer(eq2, 32));
            var trans = new TypeTransformer(factory, null, null);
            var dt = ut.Accept(trans);
            Assert.AreEqual(
                "(union \"foo\" ((ptr32 Eq_1) u0) ((ptr32 Eq_2) u1))",
                dt.ToString());
        }

        [Test]
        public void TtranMergeIdenticalStructureFields()
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
        public void TtranHasCoincidentFields()
        {
            StructureType s = new StructureType(null, 0);
            s.Fields.Add(4, new TypeVariable(1));
            s.Fields.Add(4, PrimitiveType.Word16);
            Assert.AreEqual(2, s.Fields.Count);
            TypeTransformer trans = new TypeTransformer(factory, null, null);
            Assert.IsTrue(trans.HasCoincidentFields(s));
        }

        [Test]
        public void TtranHasNoCoincidentFields()
        {
            StructureType s = new StructureType(null, 0);
            s.Fields.Add(4, new TypeVariable(1));
            s.Fields.Add(5, PrimitiveType.Word16);
            Assert.AreEqual(2, s.Fields.Count);
            TypeTransformer trans = new TypeTransformer(factory, null, null);
            Assert.IsFalse(trans.HasCoincidentFields(s));
        }

        [Test]
        public void TtranHasCoincidentUnion()
        {
            var eq = new EquivalenceClass(
                new TypeVariable(42),
                new UnionType(null, null,
                    PrimitiveType.SegPtr32, PrimitiveType.Word16));
            var s = new StructureType(null, 0)
            {
                Fields =
                { 
                    { 0, eq },
                    { 0, PrimitiveType.SegmentSelector }
                }
            };
            TypeTransformer trans = new TypeTransformer(factory, null, null);
            Assert.IsTrue(trans.HasCoincidentFields(s));
        }

        [Test]
        public void TtranIntelIndexedAddressingMode()
        {
            ProgramBuilder m = new ProgramBuilder();
            m.Add(new IntelIndexedAddressingMode());
            RunTest(m.BuildProgram(), "Typing/TtranIntelIndexedAddressingMode.txt");
        }

        [Test]
        public void TtranTreeFind()
        {
            ProgramBuilder m = new ProgramBuilder();
            m.Add(new TreeFindMock());
            RunTest(m.BuildProgram(), "Typing/TtranTreeFind.txt");
        }

        [Test]
        public void TtranSegmentedPointer()
        {
            var m = new ProgramBuilder();
            m.Add(new SegmentedPointerProc());
            RunTest(m.BuildProgram(), "Typing/TtranSegmentedPointer.txt");
        }

        [Test]
        public void TtranArrayConstantPointers()
        {
            ProgramBuilder pp = new ProgramBuilder();
            pp.Add("Fn", m =>
            {
                Identifier a = m.Local32("a");
                Identifier i = m.Local32("i");
                m.Assign(a, 0x00123456);		// array pointer
                m.MStore(m.IAdd(a, m.IMul(i, 8)), m.Int32(42));
            });
            RunTest(pp.BuildProgram(), "Typing/TtranArrayConstantPointers.txt");
        }

        [Test]
        public void TtranSegmentedCall()
        {
            var m = new ProgramBuilder();
            m.Add(new SegmentedCallFragment());
            RunTest(m.BuildProgram(), "Typing/TtranSegmentedCall.txt");
        }

        [Test]
        public void TtranArrayExpression()
        {
            var m = new ProgramBuilder();
            m.Add(new ArrayExpressionFragment());
            RunTest(m.BuildProgram(), "Typing/TtranArrayExpression.txt");
        }

        [Test]
        public void TtranCallTable()
        {
            var pb = new ProgramBuilder();
            pb.Add(new IndirectCallFragment());
            RunTest(pb.BuildProgram(), "Typing/TtranCallTable.txt");
        }

        [Test]
        public void TtranSegmentedArray()
        {
            var pb = new ProgramBuilder();
            pb.Add("SegmentedArray", m =>
            {
                var ds = m.Temp(PrimitiveType.SegmentSelector, "ds");
                var bx = m.Temp(PrimitiveType.Word16, "bx");
                m.SStore(
                    ds, m.Word16( 0x1234),
                    m.SegMem16(
                        ds,
                        m.IAdd(m.IMul(bx, 2), m.Word16(0x5388))));
            });
            RunTest(pb.BuildProgram(), "Typing/TtranSegmentedArray.txt");
        }

        [Test]
        public void TtranMemStore()
        {
            RunTest(Fragments.MemStore, "Typing/TtranMemStore.txt");
        }

        [Test]
        public void TtranUserStruct()
        {
            var t1 = new StructureType("T1", 4, true);
            var t2 = new StructureType("T2", 0, true)
            {
                Fields = { { 0, t1 } }
            };
            var ttran = new TypeTransformer(factory, store, null);
            var dt = t2.Accept(ttran);
            Assert.AreSame(t2, dt, "Should not affect user-defined types");
        }

        [Test]
        public void TtranNonUserStruct()
        {
            var t1 = new StructureType("T1", 4, true);
            var t2 = new StructureType("T2", 0, false)
            {
                Fields = { { 0, t1 } }
            };
            var ttran = new TypeTransformer(factory, store, null);
            var dt = t2.Accept(ttran);
            Assert.AreSame(t1, dt, "Should reduce fields at offset 0 ");
        }

        [Test]
        public void TtranAddressOf()
        {
            var pb = new ProgramBuilder();
            pb.Add("AddressOf", m =>
            {
                var foo = Identifier.Global("foo", new UnknownType());
                var r1 = m.Reg32("r1", 1);
                m.Declare(r1, m.AddrOf(PrimitiveType.Ptr32, foo));
                m.MStore(r1, m.Word16(0x1234));
                m.MStore(m.IAdd(r1, 4), m.Byte(0x0A));
                m.Return();
            });
            RunTest(pb.BuildProgram(), "Typing/TtranAddressOf.txt");
        }

        [Test]
        public void TtranTypedAddressOf()
        {
            var pb = new ProgramBuilder();
            pb.Add("TypedAddressOf", m =>
            {
                var str = new TypeReference("foo", new StructureType("foo", 0)
                {
                    Fields = {
                        { 0, PrimitiveType.Int16, "word00" },
                        { 4, PrimitiveType.Byte, "byte004"}
                    }
                });
                var foo = Identifier.Global("foo", str);
                var r1 = m.Reg32("r1", 1);
                m.Declare(r1, m.AddrOf(PrimitiveType.Ptr32, foo));
                m.MStore(r1, m.Word16(0x1234));
                m.MStore(m.IAdd(r1, 4), m.Byte(0x0A));
                m.Return();
            });
            RunTest(pb.BuildProgram(), "Typing/TtranTypedAddressOf.txt");
        }

        [Test]
        public void TtranSelfArray()
        {
            var pb = new ProgramBuilder();
            pb.Add("SelfArray", m =>
            {
                var d0 = m.Reg32("d0", 0);
                var a4 = m.Reg32("a4", 12);

                m.MStore(m.IAdd(a4, m.Shl(d0, 2)), a4);
                m.Return();
            });
            RunTest(pb.BuildProgram(), "Typing/TtranSelfArray.txt");
        }

        [Test]
        public void TtranSelfRef()
        {
            var pb = new ProgramBuilder();
            pb.Add("SelfRef", m =>
            {
                var a4 = m.Reg32("a4", 12);

                m.MStore(a4, a4);
                m.Return();
            });
            RunTest(pb.BuildProgram(), "Typing/TtranSelfRef.txt");
        }
    }
}
