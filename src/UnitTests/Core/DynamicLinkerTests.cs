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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonMockFactory = Reko.UnitTests.Mocks.CommonMockFactory;
using Reko.Core.Code;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class DynamicLinkerTests
    {
        private CommonMockFactory mockFactory;
        private Mock<IPlatform> platform;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.mockFactory = new CommonMockFactory();
            this.platform = mockFactory.CreateMockPlatform();
            this.program = mockFactory.CreateProgram();
        }

        [Test]
        public void Impres_ProcedureByName()
        {
            var proj = new Project
            {
                MetadataFiles = 
                {
                    new MetadataFile
                    {
                         ModuleName = "foo"
                    }
                },
                Programs =
                {
                    program
                }
            };

            var module = new ModuleDescriptor("foo")
            {
                ServicesByName =
                {
                    {
                        "bar@4",
                         new SystemService
                         {
                            Name = "bar",
                            Signature = new FunctionType()
                         }
                    }
                }
            };
            program.EnvironmentMetadata.Modules.Add("foo", module);

            var impres = new DynamicLinker(proj, program, new FakeDecompilerEventListener());
            var ep = impres.ResolveProcedure("foo", "bar@4", platform.Object);
            Assert.AreEqual("bar", ep.Name);
        }

        [Test]
        public void Impres_ProcedureByOrdinal()
        {
            var proj = new Project
            {
                MetadataFiles =
                {
                    new MetadataFile
                    {
                         ModuleName = "foo"
                    }
                },
                Programs =
                {
                    program
                }
            };

            var module = new ModuleDescriptor("foo")
            {
                ServicesByOrdinal =
                {
                    {
                         9,
                         new SystemService
                         {
                            Name = "bar",
                            Signature = new FunctionType()
                         }
                    }
                }
            };
            program.EnvironmentMetadata.Modules.Add(module.ModuleName, module);

            var impres = new DynamicLinker(proj, program, new FakeDecompilerEventListener());
            var ep = impres.ResolveProcedure("foo", 9, platform.Object);
            Assert.AreEqual("bar", ep.Name);
        }

        [Test]
        public void Impres_ProcedureByName_NoModule()
        {
            var proj = new Project
            {
                MetadataFiles =
                {
                    new MetadataFile
                    {
                         ModuleName = "foo"
                    }
                },
                Programs =
                {
                    program
                }
            };

            var barSig = new FunctionType(
                    new Identifier(
                        "res",
                        PrimitiveType.Word16,
                        new RegisterStorage("ax", 0, 0, PrimitiveType.Word16)
                    ),
                    new[] {
                        new Identifier(
                            "a",
                            PrimitiveType.Word16,
                            new RegisterStorage("cx", 0, 0, PrimitiveType.Word16)
                        ),
                        new Identifier(
                            "b",
                            PrimitiveType.Word16,
                            new RegisterStorage("dx", 0, 0, PrimitiveType.Word16)
                        )
                    }
                );

            program.EnvironmentMetadata.Modules.Add("foo", new ModuleDescriptor("foo")
            {
                ServicesByName =
                {
                    {  "bar", new SystemService {
                         Name = "bar",
                         Signature = barSig }
                    }
                }
            });

            var impres = new DynamicLinker(proj, program, new FakeDecompilerEventListener());
            var ep = impres.ResolveProcedure("foo", "bar", platform.Object);
            Assert.AreEqual("bar", ep.Name);

            var sigExp =
@"Register word16 bar(Register word16 a, Register word16 b)
// stackDelta: 0; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sigExp, ep.Signature.ToString("bar", FunctionType.EmitFlags.AllDetails));
        }

        [Test]
        public void Impres_GlobalByName()
        {
            var proj = new Project
            {
                MetadataFiles =
                {
                    new MetadataFile
                    {
                         ModuleName = "foo"
                    }
                },
                Programs =
                {
                    program
                }
            };

            var module = new ModuleDescriptor("foo")
            {
                GlobalsByName =
                {
                    {
                        "bar",
                        ImageSymbol.DataObject(
                            program.Architecture,
                            null,
                            "bar",
                            new StructureType
                            {
                                Fields =
                                {
                                    { 0, new Pointer(PrimitiveType.Char, 32), "name" },
                                    { 4, PrimitiveType.Int32, "age" }
                                }
                            })
                    }
                }
            };
            program.EnvironmentMetadata.Modules.Add(module.ModuleName, module);

            var impres = new DynamicLinker(proj, program, new FakeDecompilerEventListener());
            var dt = impres.ResolveImport("foo", "bar", platform.Object);
            Assert.AreEqual("&bar", dt.ToString());
        }

        [Test]
        public void Impres_VtblFromMsMangledName()
        {
            var proj = new Project();
            var impres = new DynamicLinker(proj, program, new FakeDecompilerEventListener());
            platform.Setup(p => p.ResolveImportByName(
                It.IsAny<string>(),
                It.IsAny<string>()))
              .Returns((Expression)null);
            SerializedType nullType = null;
            platform.Setup(p => p.DataTypeFromImportName("??_7Scope@@6B@")).
                Returns(Tuple.Create("`vftable'", nullType, nullType));

            var id = impres.ResolveImport("foo", "??_7Scope@@6B@", platform.Object);

            Assert.AreEqual("`vftable'", id.ToString());
            Assert.IsInstanceOf<UnknownType>(id.DataType);
        }

        /// <summary>
        /// In certain binaries, like ELF, the binary format can be 32-bit while the ABI
        /// is 64-bit.
        /// </summary>
        [Test]
        public void Impres_LP32_weirdness()
        {
            var memText = new MemoryArea(Address.Ptr64(0x00123400), new byte[100]);
            var memGot = new MemoryArea(Address.Ptr64(0x00200000), new byte[100]);
            var wr = new LeImageWriter(memGot.Bytes);
            wr.WriteLeUInt32(0x00300000);
            wr.WriteLeUInt32(0x00300004);
            var arch = new FakeArchitecture64();
            //var arch = new Mock<IProcessorArchitecture>();
            //arch.Setup(a => a.Endianness).Returns(EndianServices.Little);
            //arch.Setup(a => a.Name).Returns("fakeArch");
            //arch.Setup(a => a.PointerType).Returns(PrimitiveType.Ptr64);
            //arch.Setup(a => a.MakeAddressFromConstant(
            //    It.IsAny<Constant>(),
            //    It.IsAny<bool>())).Returns((Constant c, bool b) =>
            //        Address.Ptr64(c.ToUInt32()));
            var project = new Project();
            var sc = new ServiceContainer();
            var program = new Program
            {
                Architecture = arch,
                Platform = new DefaultPlatform(sc, arch),
                SegmentMap = new SegmentMap(memGot.BaseAddress, 
                    new ImageSegment(".text", memText, AccessMode.ReadExecute),
                    new ImageSegment(".got", memGot, AccessMode.Read)),
            };
            program.ImportReferences.Add(
                Address.Ptr32(0x00200000),
                new NamedImportReference(
                    Address.Ptr32(0x00200000), null, "my_global_var", SymbolType.Data));

            var impres = new DynamicLinker(project, program, new FakeDecompilerEventListener());

            var m = new ExpressionEmitter();
            var proc = program.EnsureProcedure(program.Architecture, Address.Ptr64(0x00123000), "foo_proc");
            var block = new Block(proc, "foo");
            var stm = new Statement(0x00123400, new Store(m.Word64(0x00123400), Constant.Real32(1.0F)), block);

            var result = impres.ResolveToImportedValue(stm, Constant.Word32(0x00200000));

            Assert.AreEqual("0x0000000000300000", result.ToString());
        }
    }
}
