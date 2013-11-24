#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Core
{
    [TestFixture]
    public class TypeLibraryLoaderTests
    {
        private MockRepository repository;
        private IProcessorArchitecture arch;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            repository.VerifyAll();
        }

        private void Given_ArchitectureStub()
        {
            arch = repository.DynamicMock<IProcessorArchitecture>();
        }

        private void Given_Arch_PointerDataType(PrimitiveType dt)
        {
            arch.Stub(a => a.PointerType).Return(dt);
        }


        [Test]
        public void Tlldr_Empty()
        {
            Given_ArchitectureStub();
            repository.ReplayAll();

            var tlLdr = new TypeLibraryLoader(arch);
            TypeLibrary lib = tlLdr.Load(new SerializedLibrary());
        }

        [Test]
        public void Tlldr_typedef_int()
        {
            Given_ArchitectureStub();
            repository.ReplayAll();

            var tlLdr = new TypeLibraryLoader(arch);
            var slib = new SerializedLibrary
            {
                Types = new SerializedType[]
                {
                    new SerializedTypedef { Name="int", DataType=new SerializedPrimitiveType { Domain = Domain.SignedInt, ByteSize=4 }}
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
            repository.ReplayAll();

            var tlLdr = new TypeLibraryLoader(arch);
            var slib = new SerializedLibrary
            {
                Types = new SerializedType[]
                {
                    new SerializedTypedef { 
                        Name="pint", 
                        DataType= new SerializedPointerType
                        {
                            DataType = new SerializedPrimitiveType { Domain = Domain.SignedInt, ByteSize=4 } 
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
            repository.ReplayAll();

            var tlLdr = new TypeLibraryLoader(arch);
            var slib = new SerializedLibrary
            {
                Procedures = {
                    new SerializedProcedure { 
                        Name="foo",
                        Signature = new SerializedSignature
                        {
                            Convention="__cdecl",
                            ReturnValue = new SerializedArgument {
                                Type = new SerializedVoidType()
                            },
                        }
                    }
                }
            };
            var lib = tlLdr.Load(slib);

            repository.VerifyAll();
            Assert.AreEqual(
                "void foo()",
                lib.Lookup("foo").ToString("foo"));
        }

        [Test]
        public void Tlldr_fn_struct_param()
        {
            Given_ArchitectureStub();
            repository.ReplayAll();

            var tlLdr = new TypeLibraryLoader(arch);
            var slib = new SerializedLibrary
            {
                Types = new SerializedType[]
                {
                    new SerializedStructType {
                        Name = "tagFoo",
                        Fields = new SerializedStructField [] 
                        {
                            new SerializedStructField { Name="Bob", Offset=3, Type=new SerializedPrimitiveType { Domain=Domain.SignedInt, ByteSize=4 } }
                        }
                    },
                },
                Procedures = {
                    new SerializedProcedure { 
                        Name="foo",
                        Signature = new SerializedSignature
                        {
                            Convention="__cdecl",
                            ReturnValue = new SerializedArgument {
                                Type = new SerializedVoidType {}
                            },
                            Arguments = new SerializedArgument[] {
                                new SerializedArgument {
                                    Name = "bar",
                                    Type = new SerializedStructType { Name="tagFoo" },
                                    Kind = new SerializedStackVariable { ByteSize= 4 }
                                }
                            }
                        }
                    }
                }
            };
            var lib = tlLdr.Load(slib);

            repository.VerifyAll();
            Assert.AreEqual(
                "void foo(Stack (struct \"tagFoo\") bar)",
                lib.Lookup("foo").ToString("foo"));
        }
    }
}
