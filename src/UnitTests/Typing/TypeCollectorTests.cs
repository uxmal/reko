#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Fragments;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace Reko.UnitTests.Typing
{
    [TestFixture]
    public class TypeCollectorTests : TypingTestBase
    {
        private bool buildEquivalenceClasses;
        private FakeDecompilerEventListener eventListener;

        [SetUp]
        public void Setup()
        {
            this.buildEquivalenceClasses = false;
            this.eventListener = new FakeDecompilerEventListener();
        }

        private static Program Given_FlatProgram()
        {
            var arch = new FakeArchitecture(new ServiceContainer());
            var platform = new FakePlatform(arch.Services, arch);
            var mem = new ByteMemoryArea(Address.Ptr32(0x00123300), new byte[0x1000]);
            var segment = new ImageSegment(".data", mem, AccessMode.ReadWrite);
            var segments = new SegmentMap(segment.Address, segment);
            var program = new Program(segments, arch, platform);
            return program;
        }


        protected override void RunTest(Program program, string outputFile)
        {
            FileUnitTester fut = null;
            try
            {
                fut = new FileUnitTester(outputFile);
                var factory = program.TypeFactory;
                var store = program.TypeStore;
                var aen = new ExpressionNormalizer(program.Platform.PointerType);
                var eqb = new EquivalenceClassBuilder(factory, store, eventListener);
                var tyco = new TypeCollector(factory, store, program, eventListener);

                aen.Transform(program);
                eqb.Build(program);
                tyco.CollectTypes();
                if (buildEquivalenceClasses)
                {
                    store.BuildEquivalenceClassDataTypes(factory);
                    new TypeVariableReplacer(store).ReplaceTypeVariables();
                }

            }
            catch (Exception ex)
            {
                fut.TextWriter.WriteLine(ex.Message);
                fut.TextWriter.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                DumpProgAndStore(program, fut.TextWriter);
                fut.AssertFilesEqual();
                fut.Dispose();
            }
        }

        protected override void RunTest(Action<ProcedureBuilder> doBuild, string outputFile)
        {
            var pb = new ProgramBuilder();
            pb.Add("proc1", doBuild);
            var program = pb.BuildProgram();
            RunTest(program, outputFile);
        }

        protected void RunStringTest(Program program, string expectedOutput)
        {
            var sw = new StringWriter();

            var factory = program.TypeFactory;
            var store = program.TypeStore;
            var aen = new ExpressionNormalizer(program.Platform.PointerType);
            var eqb = new EquivalenceClassBuilder(factory, store, eventListener);
            var tyco = new TypeCollector(factory, store, program, eventListener);

            aen.Transform(program);
            eqb.Build(program);
            tyco.CollectTypes();

            aen.Transform(program);
            eqb.Build(program);
            var coll = new TypeCollector(program.TypeFactory, program.TypeStore, program, eventListener);
            coll.CollectTypes();
            program.TypeStore.Dump();

            program.TypeStore.BuildEquivalenceClassDataTypes(program.TypeFactory);
            DumpProgAndStore(program, sw);
            Assert.AreEqual(expectedOutput, sw.ToString());
        }

        private TypeCollector Given_TypeCollector(Program program)
        {
            var tyco = new TypeCollector(
                program.TypeFactory,
                program.TypeStore,
                program,
                eventListener);
            return tyco;
        }

        private void DumpProgAndStore(Program program, TextWriter writer)
        {
            foreach (Procedure proc in program.Procedures.Values)
            {
                proc.Write(false, writer);
                writer.WriteLine();
            }
            program.TypeStore.Write(false, writer);
        }

        [Test]
        public void TycoMemStore()
        {
            RunTest(Fragments.MemStore, "Typing/TycoMemStore.txt");
        }

        [Test]
        public void TycoIndexedDisplacement()
        {
            RunTest(m =>
            {
                var ds = m.Temp(PrimitiveType.SegmentSelector, "ds");
                var bx = m.Temp(PrimitiveType.Word16, "bx");
                var si = m.Temp(PrimitiveType.Int16, "si");
                m.Assign(bx, m.SegMem16(ds, m.Word16(0xC00)));
                m.SStore(ds, m.IAdd(
                                m.IAdd(bx, 10),
                                si),
                           m.Byte(0xF8));
            }, "Typing/TycoIndexedDisplacement.txt");
        }

        [Test]
        public void TycoNestedStructsPtr()
        {
            RunTest(m =>
            {
                var eax = m.Reg32("eax", 0);
                var ecx = m.Reg32("ecx", 1);
                var strInner = new StructureType("strInner", 8, true)
                {
                    Fields = {
                        { 0, PrimitiveType.Real32, "innerAttr00" },
                        { 4, PrimitiveType.Int32, "innerAttr04" },
                    }
                };
                var str = new StructureType("str", 8, true)
                {
                    Fields = {
                        { 0, new Pointer(strInner, 32), "strAttr00" },
                        { 4, PrimitiveType.Int32, "strAttr04" },
                    }
                };
                var v = m.Frame.EnsureStackArgument(4, new Pointer(str, 32));
                m.Declare(eax, m.Mem(PrimitiveType.Word32, v));
                m.Declare(ecx, m.Mem(PrimitiveType.Word32, eax));
            }, "Typing/TycoNestedStructsPtr.txt");
        }

        [Test]
        public void TycoAddressOf()
        {
            RunTest(m =>
            {
                var foo = Identifier.Global("foo", new UnknownType());
                var r1 = m.Reg32("r1", 1);
                m.Assign(r1, m.AddrOf(PrimitiveType.Ptr32, foo));
                m.MStore(r1, m.Word16(0x1234));
                m.MStore(m.IAdd(r1, 4), m.Byte(0x0A));
                m.Return();
            }, "Typing/TycoAddressOf.txt");
        }

        [Test]
        public void TycoTypedAddressOf()
        {
            RunTest(m =>
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
            }, "Typing/TycoTypedAddressOf.txt");
        }

        [Test]
        public void TycoArrayConstantPointers()
        {
            ProgramBuilder pp = new ProgramBuilder();
            pp.Add("Fn", m =>
            {
                Identifier a = m.Local32("a");
                Identifier i = m.Local32("i");
                m.Assign(a, 0x00123456);		// array pointer
                m.MStore(m.IAdd(a, m.IMul(i, 8)), m.Word32(42));
            });
            RunTest(pp.BuildProgram(), "Typing/TycoArrayConstantPointers.txt");
        }

        [Test]
        public void TycoFramePointer()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new FramePointerFragment(mock.Program.TypeFactory));
            RunTest(mock, "Typing/TycoFramePointer.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TycoReg00014()
        {
            RunTest32("Fragments/regressions/r00014.asm", "Typing/TycoReg00014.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TycoReg00300()
        {
            buildEquivalenceClasses = true;
            RunTest(m =>
            {
                m.MStore(m.Word32(0x123400), m.IAdd(m.Mem32(m.Word32(0x123400)), 1));
                m.MStore(m.Word32(0x123400), m.IAdd(m.Mem32(m.Word32(0x123400)), 1));
            }, "Typing/TycoReg00300.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TycoCallFunctionWithArraySize()
        {
            var m = new ProcedureBuilder();
            var sig = FunctionType.Func(
                new Identifier("", new Pointer(VoidType.Instance, 32), null),
                m.Frame.EnsureStackArgument(0, PrimitiveType.Word32));
            var ex = new ExternalProcedure("malloc", sig, new ProcedureCharacteristics
            {
                Allocator = true,
                ArraySize = new ArraySizeCharacteristic
                {
                    Argument = "r",
                    Factors = new ArraySizeFactor[]
                    {
                        new ArraySizeFactor { Constant = "1" }
                    }
                }
            });

            RunTest(n =>
            {
                Identifier eax = m.Local32("eax");
                var call = n.Assign(eax, n.Fn(new ProcedureConstant(PrimitiveType.Word32, ex), n.Word32(3)));
            }, "Typing/TycoCallFunctionWithArraySize.txt");
        }

        [Test(Description = "According to C/C++ rules, adding signed + unsigned yields an unsigned value.")]
        public void TycoSignedUnsignedAdd()
        {
            ProgramBuilder pp = new ProgramBuilder();
            pp.Add("Fn", m =>
            {
                Identifier a = m.Local32("a");
                Identifier b = m.Local32("b");
                Identifier c = m.Local32("c");
                a.DataType = PrimitiveType.Int32;
                b.DataType = PrimitiveType.UInt32;
                m.Assign(c, m.IAdd(a, b));
            });
            RunTest(pp.BuildProgram(), "Typing/TycoSignedUnsignedAdd.txt");
        }

        [Test(Description = "According to C/C++ rules, adding signed + unsigned yields an unsigned value.")]
        public void TycoStructMembers()
        {
            ProgramBuilder pp = new ProgramBuilder();
            pp.Add("Fn", m =>
            {
                Identifier ptr = m.Local32("ptr");
                Identifier b16 = m.Local16("b16");
                Identifier c16 = m.Local16("c16");
                m.MStore(m.IAdd(ptr, 200), m.Word32(0x1234));
                m.MStore(m.IAdd(ptr, 12), m.Word32(0x5678));
                m.Assign(b16, m.Or(m.Mem16(m.IAdd(ptr, 14)), m.Word16(0x00FF)));
            });
            RunTest(pp.BuildProgram(), "Typing/TycoStructMembers.txt");
        }


        [Test]
        public void TycoUserData()
        {
            var addrUserData = Address.Ptr32(0x00001400);
            var program = new ProgramBuilder().BuildProgram();
            program.User = new UserData
            {
                Globals =
                {
                    {
                        addrUserData, new UserGlobal(addrUserData, "xAcceleration", PrimitiveType_v1.Real64())
                    }
                }
            };
            new EquivalenceClassBuilder(program.TypeFactory, program.TypeStore, eventListener).Build(program);
            var tyco = Given_TypeCollector(program);
            tyco.CollectGlobalType();
            tyco.CollectUserGlobalVariableTypes();

            Then_GlobalFieldsAre(program, "1400: xAcceleration: real64");
        }

        [Test]
        public void TycoUserSegmentedData()
        {
            var addrUserData = Address.SegPtr(0xC30, 0x0042);
            var addrSeg = Address.SegPtr(0xC30, 0);
            var seg = new ImageSegment("seg0C30", addrSeg, new ByteMemoryArea(addrSeg, new byte[0x100]), AccessMode.ReadWriteExecute);
            seg.Identifier = new Identifier("seg0C30", PrimitiveType.SegmentSelector, MemoryStorage.Instance);
            var program = new ProgramBuilder().BuildProgram();
            program.SegmentMap.AddSegment(seg);
            program.User = new UserData
            {
                Globals =
                {
                    {
                        addrUserData, new UserGlobal(addrUserData, "myGlobal", PrimitiveType_v1.Real32())
                    }
                }
            };
            var eqb = new EquivalenceClassBuilder(program.TypeFactory, program.TypeStore, eventListener);
            eqb.EnsureSegmentTypeVariables(program.SegmentMap.Segments.Values);
            var tyco = Given_TypeCollector(program);

            tyco.CollectUserGlobalVariableTypes();

            Assert.AreEqual("42: myGlobal: real32", program.TypeStore.SegmentTypes[seg].Fields.First().ToString());
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TycoFactorial()
        {
            RunTest16("Fragments/factorial.asm", "Typing/TycoFactorial.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TycoFactorialReg()
        {
            RunTest16("Fragments/factorial_reg.asm", "Typing/TycoFactorialReg.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TycoReg00011()
        {
            RunTest16("Fragments/regressions/r00011.asm", "Typing/TycoReg00011.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TycoReg00012()
        {
            RunTest16("Fragments/regressions/r00012.asm", "Typing/TycoReg00012.txt");
        }

        [Test]
        public void TycoImageSymbol()
        {
            var program = Given_FlatProgram();
            var sym = ImageSymbol.DataObject(program.Architecture, Address.Ptr32(0x00123400), "a_data", PrimitiveType.Word32);
            program.ImageSymbols.Add(sym.Address, sym);
            new EquivalenceClassBuilder(program.TypeFactory, program.TypeStore, eventListener).Build(program);
            var tyco = new TypeCollector(program.TypeFactory, program.TypeStore, program, new FakeDecompilerEventListener());
            tyco.CollectGlobalType();
            tyco.CollectImageSymbols();

            Then_GlobalFieldsAre(program, "123400: a_data: T_2");
        }

        private void Then_GlobalFieldsAre(Program program, params string [] sExpected)
        {
            var fields = ((StructureType) ((Pointer) program.Globals.TypeVariable.OriginalDataType).Pointee).Fields.ToArray();
            var c = Math.Min(fields.Length, sExpected.Length);
            for (int i = 0; i < fields.Length; ++i)
            {
                Assert.AreEqual(sExpected[i], fields[i].ToString(), $"Field {i} mismatch");
            }
            Assert.AreEqual(fields.Length, sExpected.Length, "Field count mismatch");
        }
    }
}
