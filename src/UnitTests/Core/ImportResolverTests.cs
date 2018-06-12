#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class ImportResolverTests
    {
        private MockRepository mr;
        private MockFactory mockFactory;
        private IPlatform platform;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.mockFactory = new MockFactory(mr);
            this.platform = mockFactory.CreatePlatform();
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

            var impres = new ImportResolver(proj, program, new FakeDecompilerEventListener());
            var ep = impres.ResolveProcedure("foo", "bar@4", platform);
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

            var impres = new ImportResolver(proj, program, new FakeDecompilerEventListener());
            var ep = impres.ResolveProcedure("foo", 9, platform);
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

            var impres = new ImportResolver(proj, program, new FakeDecompilerEventListener());
            var ep = impres.ResolveProcedure("foo", "bar", platform);
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
                        new ImageSymbol(program.Architecture)
                        {
                            Name = "bar",
                            Type = SymbolType.Data,
                            DataType = new StructureType
                            {
                                Fields =
                                {
                                    { 0, new Pointer(PrimitiveType.Char, 32), "name" },
                                    { 4, PrimitiveType.Int32, "age" }
                                }
                            }
                        }
                    }
                }
            };
            program.EnvironmentMetadata.Modules.Add(module.ModuleName, module);

            var impres = new ImportResolver(proj, program, new FakeDecompilerEventListener());
            var dt = impres.ResolveImport("foo", "bar", platform);
            Assert.AreEqual("&bar", dt.ToString());
        }

        [Test]
        public void Impres_VtblFromMsMangledName()
        {
            var proj = new Project();
            var impres = new ImportResolver(proj, program, new FakeDecompilerEventListener());
            platform.Stub(p => p.ResolveImportByName(null, null)).
                IgnoreArguments().Return(null);
            SerializedType nullType = null;
            platform.Stub(p => p.DataTypeFromImportName("??_7Scope@@6B@")).
                Return(Tuple.Create("`vftable'", nullType, nullType));

            var id = impres.ResolveImport("foo", "??_7Scope@@6B@", platform);

            Assert.AreEqual("`vftable'", id.ToString());
            Assert.IsInstanceOf<UnknownType>(id.DataType);
        }
    }
}
