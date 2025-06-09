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
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Fragments;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using Reko.Core.Serialization;
using System.IO;

namespace Reko.UnitTests.Decompiler.Typing
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

        private void RunTestCore(Program program)
        {
            var listener = new FakeDecompilerEventListener();
            var aen = new ExpressionNormalizer(program.Architecture.PointerType);
            aen.Transform(program);
            var eq = new EquivalenceClassBuilder(factory, store, listener);
            eq.Build(program);
            var coll = new TypeCollector(factory, store, program, listener);
            coll.CollectTypes();
            store.BuildEquivalenceClassDataTypes(factory);

            var tvr = new TypeVariableReplacer(store);
            tvr.ReplaceTypeVariables();

            var trans = new TypeTransformer(factory, store, program);
            trans.Transform();
        }

        protected void RunStringTest(string sExpected, Program program)
        {
            RunTestCore(program);
            var sw = new StringWriter();
            WriteTestResults(program, sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        protected override void RunTest(Program program, string outputFileName)
        {
            Exception theEx = null;
            try
            {
                RunTestCore(program);
            }
            catch (Exception ex)
            {
                theEx = ex;
            }
            try
            {
                TypeTransformer trans = new TypeTransformer(factory, store, program);
                trans.Transform();
            }
            catch (Exception ex)
            {
                theEx = ex;
            }
            using (FileUnitTester fut = new FileUnitTester(outputFileName))
            {
                if (theEx is not null)
                {
                    fut.TextWriter.WriteLine(theEx.Message);
                    fut.TextWriter.WriteLine(theEx.StackTrace);
                }
                WriteTestResults(program, fut.TextWriter);
                fut.AssertFilesEqual();
            }
        }

        private void WriteTestResults(Program program, TextWriter writer)
        {
            foreach (Procedure proc in program.Procedures.Values)
            {
                proc.Write(false, writer);
                writer.WriteLine();
            }
            store.Write(false, writer);
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
            // TypeTransformer.Transform() clear some state (TypeTransformer.visitedTypes) between iterations.
            trans = new TypeTransformer(factory, null, null);
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
                m.Assign(a, 0x00123456);    // array pointer
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
                    ds, m.Word16(0x1234),
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
            Assert.AreSame(t1, dt, "Should reduce fields at Offset 0 ");
        }

        [Test]
        public void TtranAddressOf()
        {
            var pb = new ProgramBuilder();
            pb.Add("AddressOf", m =>
            {
                var foo = Identifier.Global("foo", new UnknownType());
                var r1 = m.Reg32("r1", 1);
                m.Assign(r1, m.AddrOf(PrimitiveType.Ptr32, foo));
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
                m.Assign(r1, m.AddrOf(PrimitiveType.Ptr32, foo));
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

        [Test]
        public void TtranMalloc()
        {
            var r1 = RegisterStorage.Reg32("r1", 1);
            var malloc = new ExternalProcedure("malloc", FunctionType.Create(
                new Identifier("", new Pointer(new UnknownType(), 32), r1),
                new Identifier("size", PrimitiveType.UInt32, RegisterStorage.Reg32("r2", 2))),
                new ProcedureCharacteristics
                {
                    Allocator = true,
                });

            var pb = new ProgramBuilder();
            pb.Add(nameof(TtranMalloc), m =>
            {
                var r1_1 = new Identifier("r1_1", PrimitiveType.Word32, r1);
                var r1_2 = new Identifier("r1_2", PrimitiveType.Word32, r1);
                m.Assign(r1_1, m.Fn(malloc, m.Word32(40)));
                m.MStore(m.IAddS(r1_1, 4), Constant.Real32(0.5F));
                m.Assign(r1_2, m.Fn(malloc, m.Word32(20)));
                m.MStore(m.IAddS(r1_2, 4), Constant.Int32(-2));
                m.Return();
            });
            RunTest(pb.BuildProgram(), $"Typing/{nameof(TtranMalloc)}.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TtranArrayAssignment()
        {
            var pp = new ProgramBuilder();
            pp.Add("Fn", m =>
            {
                Identifier rbx_18 = m.Local(PrimitiveType.Create(Domain.Integer | Domain.Real | Domain.Pointer, 64), "rbx_18");
                Identifier rdx = m.Local(PrimitiveType.Create(Domain.Integer | Domain.Real | Domain.Pointer, 64), "rdx");
                Identifier rax_22 = m.Local(PrimitiveType.Create(Domain.Integer | Domain.Real | Domain.Pointer, 64), "rax_22");
                Identifier rsi = m.Local(PrimitiveType.Create(Domain.Integer | Domain.Real | Domain.Pointer, 64), "rsi");
                Identifier rdi = m.Local(PrimitiveType.Create(Domain.Integer | Domain.Real | Domain.Pointer, 64), "rdi");

                m.Label("l000000000040EC30");
                m.Assign(rbx_18, m.ISub(rdx, Constant.Create(PrimitiveType.Create(Domain.Integer | Domain.Real | Domain.Pointer, 64), 0x1)));
                m.BranchIf(m.Eq(rdx, Constant.Create(PrimitiveType.Create(Domain.Integer | Domain.Real | Domain.Pointer, 64), 0x0)), "l000000000040EC69");

                m.Label("l000000000040EC40");
                m.Assign(rax_22, m.Word64(0x10000040));

                m.Label("l000000000040EC50");
                m.MStore(m.IAdd(rdi, rbx_18), m.Convert(m.Mem32(
                    m.IAdd(m.Mem64(rax_22), m.IMul(
                        m.Convert(
                            m.Convert(
                                m.Mem8(m.IAdd(rsi, rbx_18)),
                                PrimitiveType.Byte,
                                PrimitiveType.Word32),
                            PrimitiveType.Word32,
                            PrimitiveType.Word64),
                        Constant.Create(PrimitiveType.Word64, 0x4)))),
                        PrimitiveType.Word32,
                        PrimitiveType.Byte));
                m.Assign(rbx_18, m.ISub(rbx_18, Constant.Create(PrimitiveType.Word64, 0x1)));
                m.BranchIf(m.Ne(rbx_18, Constant.Create(PrimitiveType.Word64, 0xFFFFFFFFFFFFFFFF)), "l000000000040EC50");

                m.Label("l000000000040EC69");
                m.Return();
            });
            RunTest(pp.BuildProgram(), $"Typing/{nameof(TtranArrayAssignment)}.txt");
        }

        [Test]
        public void TtranPassARefToFunction()
        {
            var sExp =
@"// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	r1 = Mem0[r0 + 0<32>:word32]
	r1 = r0[r1 * 4<32>]
main_exit:

// Equivalence classes ////////////
Eq_1: (struct ""Globals"")
	globals_t (in globals : (ptr32 (struct ""Globals"")))
// Type Variables ////////////
globals_t: (in globals : (ptr32 (struct ""Globals"")))
  Class: Eq_1
  DataType: (ptr32 Eq_1)
  OrigDataType: (ptr32 (struct ""Globals""))
T_2: (in r0 : word32)
  Class: Eq_2
  DataType: (ptr32 (arr ui32))
  OrigDataType: (ptr32 (struct (0 (arr T_5) a0000)))
T_3: (in 0<32> : word32)
  Class: Eq_3
  DataType: word32
  OrigDataType: word32
T_4: (in r0 + 0<32> : word32)
  Class: Eq_4
  DataType: word32
  OrigDataType: word32
T_5: (in Mem0[r0 + 0<32>:word32] : word32)
  Class: Eq_5
  DataType: ui32
  OrigDataType: word32
T_6: (in r1 : word32)
  Class: Eq_5
  DataType: ui32
  OrigDataType: ui32
T_7: (in 4<32> : word32)
  Class: Eq_7
  DataType: ui32
  OrigDataType: ui32
T_8: (in r1 * 4<32> : word32)
  Class: Eq_8
  DataType: ui32
  OrigDataType: ui32
T_9: (in r0[r1 * 4<32>] : word32)
  Class: Eq_5
  DataType: ui32
  OrigDataType: word32
T_10:
  Class: Eq_5
  DataType: ui32
  OrigDataType: (struct 0004 (0 ui32 dw0000))
T_11:
  Class: Eq_11
  DataType: (arr ui32)
  OrigDataType: (arr T_10)
";
            var pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var r0 = m.Register("r0");
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var r3 = m.Register("r3");
                m.Assign(r1, m.Mem32(m.IAdd(r0, 0)));
                m.Assign(r1, m.Mem32(m.IAdd(r0, m.IMul(r1, 4))));
            });
            RunStringTest(sExp, pb.BuildProgram());
        }
    }
}
