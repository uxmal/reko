#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Types;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Expressions;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class TypeLibraryDeserializerTests
    {
        private MockRepository mr;
        private IProcessorArchitecture arch;
        private IPlatform platform;
        private ProcedureSerializer procSer;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
        }

        private void Given_ArchitectureStub()
        {
            arch = mr.DynamicMock<IProcessorArchitecture>();
            arch.Stub(a => a.PointerType).Return(PrimitiveType.Pointer32);
            arch.Stub(a => a.WordWidth).Return(PrimitiveType.Pointer32);
            platform = mr.DynamicMock<IPlatform>();
            platform.Stub(p => p.PointerType).Return(PrimitiveType.Pointer32);
            platform.Stub(p => p.Architecture).Return(arch);
            platform.Replay();
            this.procSer = new ProcedureSerializer(platform, null, "");
        }

        private void Given_Arch_PointerDataType(PrimitiveType dt)
        {
            arch.Stub(a => a.PointerType).Return(dt);
        }

        [Test]
        public void Tlldr_Empty()
        {
            Given_ArchitectureStub();
            mr.ReplayAll();

            var tlLdr = new TypeLibraryDeserializer(platform, true, new TypeLibrary());
            TypeLibrary lib = tlLdr.Load(new SerializedLibrary());
            Assert.AreEqual(0, lib.Types.Count);
            Assert.AreEqual(0, lib.Signatures.Count);
            Assert.AreEqual(1, lib.Modules.Count, "The blank module is there");
        }

        [Test]
        public void Tlldr_typedef_int()
        {
            Given_ArchitectureStub();
            mr.ReplayAll();

            var tlLdr = new TypeLibraryDeserializer(platform, true, new TypeLibrary());
            var slib = new SerializedLibrary
            {
                Types = new SerializedType[]
                {
                    new SerializedTypedef { Name="int", DataType=new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize=4 }}
                }
            };
            var lib = tlLdr.Load(slib);

            Assert.AreSame(PrimitiveType.Int32, lib.LookupType("int"));
        }

        [Test]
        public void Tlldr_typedef_ptr_int()
        {
            Given_ArchitectureStub();
            Given_Arch_PointerDataType(PrimitiveType.Pointer32);
            mr.ReplayAll();

            var tlLdr = new TypeLibraryDeserializer(platform, true, new TypeLibrary());
            var slib = new SerializedLibrary
            {
                Types = new SerializedType[]
                {
                    new SerializedTypedef {
                        Name="pint",
                        DataType= new PointerType_v1
                        {
                            DataType = new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize=4 }
                        }
                    }
                }
            };
            var lib = tlLdr.Load(slib);

            Assert.AreEqual("(ptr int32)", lib.LookupType("pint").ToString());
        }

        [Test]
        public void Tlldr_void_fn()
        {
            Given_ArchitectureStub();
            Given_ProcedureSignature(new FunctionType());
            mr.ReplayAll();

            var tlLdr = new TypeLibraryDeserializer(platform, true, new TypeLibrary());
            var slib = new SerializedLibrary
            {
                Procedures = {
                    new Procedure_v1 {
                        Name="foo",
                        Signature = new SerializedSignature
                        {
                            Convention="__cdecl",
                            ReturnValue = new Argument_v1 {
                                Type = new VoidType_v1()
                            },
                        }
                    }
                }
            };
            var lib = tlLdr.Load(slib);

            mr.VerifyAll();
            Assert.AreEqual(
                "void foo()",
                lib.Lookup("foo").ToString("foo"));
        }

        [Test]
        public void Tlldr_BothOrdinalAndName()
        {
            Given_ArchitectureStub();
            Given_ProcedureSignature(new FunctionType());
            mr.ReplayAll();

            var tlLDr = new TypeLibraryDeserializer(platform, true, new TypeLibrary());
            var slib = new SerializedLibrary
            {
                Procedures = {
                    new SerializedService {
                        Name="foo",
                        Ordinal=2,
                        Signature = new SerializedSignature {
                            ReturnValue = new Argument_v1 {
                                Type = new VoidType_v1()
                            }
                        }
                    }
                }
            };
            var lib = tlLDr.Load(slib);

            mr.VerifyAll();
            Assert.AreEqual(1, lib.Modules[""].ServicesByVector.Count);
            Assert.IsNotNull(lib.Modules[""].ServicesByVector[2]);
        }

        [Test(Description = "Resolve a typedef declaration of a structure")]
        public void Tlldr_typedef_struct()
        {
            Given_ArchitectureStub();
            mr.ReplayAll();

            var typelib = new TypeLibrary();
            var tlldr = new TypeLibraryDeserializer(platform, true, typelib);
            new StructType_v1
            {
                Name = "localeinfo_struct",
                Fields = new StructField_v1[]
                {
                    new StructField_v1 {
                        Name = "foo",
                        Offset = 0,
                        Type = new PrimitiveType_v1 { Domain = Domain.Integer, ByteSize=4 }
                    }
                }
            }.Accept(tlldr);
            new SerializedTypedef
            {
                Name = "_locale_tstruct",
                DataType = new StructType_v1
                {
                    Name = "localeinfo_struct",
                }
            }.Accept(tlldr);

            var str = (StructureType)typelib.Types["_locale_tstruct"];
            Assert.AreEqual("(struct \"localeinfo_struct\" (0 ui32 foo))", str.ToString());
        }

        [Test(Description = "Resolve a forward declaration of a structure")]
        public void Tlldr_typedef_forwarded_struct()
        {
            Given_ArchitectureStub();
            mr.ReplayAll();

            var typelib = new TypeLibrary();
            var tlldr = new TypeLibraryDeserializer(platform, true, typelib);
            new SerializedTypedef
            {
                Name = "_locale_tstruct",
                DataType = new StructType_v1
                {
                    Name = "localeinfo_struct",
                }
            }.Accept(tlldr);
            new StructType_v1
            {
                Name = "localeinfo_struct",
                Fields = new StructField_v1[]
                {
                    new StructField_v1 {
                        Name = "foo",
                        Offset =0,
                        Type = new PrimitiveType_v1 { Domain = Domain.Integer, ByteSize=4 }
                    }
                }
            }.Accept(tlldr);

            var str = (StructureType)typelib.Types["_locale_tstruct"];
            Assert.AreEqual("(struct \"localeinfo_struct\" (0 ui32 foo))", str.ToString());
        }

        [Test(Description = "Resolve a forward declaration of a union")]
        public void Tlldr_typedef_forwarded_union()
        {
            Given_ArchitectureStub();
            mr.ReplayAll();

            var typelib = new TypeLibrary();
            var tlldr = new TypeLibraryDeserializer(platform, true, typelib);
            new SerializedTypedef
            {
                Name = "variant_t",
                DataType = new UnionType_v1
                {
                    Name = "variant_union",
                }
            }.Accept(tlldr);
            new UnionType_v1
            {
                Name = "variant_union",
                Alternatives = new[]
                {
                    new SerializedUnionAlternative {
                        Name = "foo",
                        Type = new PrimitiveType_v1 { Domain = Domain.Integer, ByteSize=4 },
                    },
                    new SerializedUnionAlternative {
                        Name = "bar",
                        Type = new PrimitiveType_v1 { Domain = Domain.Real, ByteSize=4 }
                    }
                }
            }.Accept(tlldr);

            var str = (UnionType)typelib.Types["variant_t"];
            Assert.AreEqual("(union \"variant_union\" (ui32 foo) (real32 bar))", str.ToString());
        }

        [Test(Description = "Load a serialized signature")]
        public void Tlldr_signature()
        {
            Given_ArchitectureStub();
            arch.Stub(a => a.GetRegister("r3")).Return(new RegisterStorage("r3", 3, 0, PrimitiveType.Word32));
            var r3 = new RegisterStorage("r3", 3, 0, PrimitiveType.Word32);
            Given_ProcedureSignature(new FunctionType(
                new Identifier("", PrimitiveType.Int32, r3),
                new[] {
                    new Identifier("", PrimitiveType.Real32, r3)}
                ));
            mr.ReplayAll();

            var typelib = new TypeLibrary();
            var tlldr = new TypeLibraryDeserializer(platform, true, typelib);
            var fn = tlldr.VisitSignature(new SerializedSignature
            {
                Arguments = new[]
                {
                    new Argument_v1 {
                        Name = "reg1",
                        Type = new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 4 },
                        Kind = new Register_v1 { Name = "r3" }
                    }
                },
                ReturnValue = new Argument_v1
                {
                    Type = new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 4 },
                    Kind = new Register_v1 { Name = "r3" }
                }
            });
            Assert.AreEqual("(fn int32 (real32))", fn.ToString());
            mr.VerifyAll();
        }

        private void Given_ProcedureSignature(FunctionType sig)
        {
            //procSer.Stub(p => p.Deserialize(null, null)).IgnoreArguments().Return(sig);
        }

        [Test(Description = "Verifies that globals can be specified by ordinal")]
        public void Tlldr_LoadGlobalByOrdinal()
        {
            var typelib = new TypeLibrary();
            platform = mr.Stub<IPlatform>();
            platform.Stub(p => p.DefaultCallingConvention).Return("__cdecl");
            platform.Replay();
            var tlldr = new TypeLibraryDeserializer(platform, true, typelib);
            tlldr.Load(new SerializedLibrary
            {
                ModuleName = "stdlib",
                Globals = new List<GlobalVariable_v1>
                 {
                     new GlobalVariable_v1
                     {
                         Name = "errno",
                         Ordinal = 42,
                         Type = PrimitiveType_v1.Int32(),
                     }
                 }
            });
            var stdlib = typelib.Modules["stdlib"];
            var globalByName = stdlib.GlobalsByName["errno"];
            var globalByOrdinal = stdlib.GlobalsByOrdinal[42];
            Assert.AreSame(globalByName, globalByOrdinal);
        }
    }
}
