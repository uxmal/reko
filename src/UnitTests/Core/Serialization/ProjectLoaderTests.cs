#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Scripts;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class ProjectLoaderTests
    {
        private CommonMockFactory mockFactory;
        private ServiceContainer sc;
        private Mock<IConfigurationService> cfgSvc;
        private Mock<IPlatform> platform;
        private Mock<IProcessorArchitecture> arch;
        private Dictionary<string, object> loadedOptions;
        private Mock<ITypeLibraryLoaderService> tlSvc;
        private Mock<PlatformDefinition> oe;
        private Mock<DecompilerEventListener> listener;

        [SetUp]
        public void Setup()
        {
            this.mockFactory = new CommonMockFactory();
            this.sc = new ServiceContainer();
            this.cfgSvc = new Mock<IConfigurationService>();
            this.listener = new Mock<DecompilerEventListener>();
            this.sc.AddService(cfgSvc.Object);
        }

        private string AbsolutePathEndingWith(params string [] dirs)
        {
            var relativePath = Path.Combine(dirs);
            if (Path.DirectorySeparatorChar == '/')
            {
                // Un*x
                return $"/{relativePath.Replace('\\', '/')}";
            }
            else
            {
                // Windows
                return $@"c:\{relativePath}";
            }
        }

        private static bool IsPathEndingWith(string path, string sExpected)
        {
            path = path.Replace('\\', '/');
            return path.EndsWith(sExpected);
        }

        private void Given_TestArch()
        {
            this.arch = new Mock<IProcessorArchitecture>();
            this.arch.Setup(a => a.Name).Returns("testArch");
            this.cfgSvc.Setup(c => c.GetArchitecture("testArch")).Returns(arch.Object);
        }

        private void Given_TestOS()
        {
            this.oe = new Mock<PlatformDefinition>();
            this.platform = new Mock<IPlatform>();
            this.platform.Setup(a => a.Name).Returns("testOS");
            this.cfgSvc.Setup(c => c.GetEnvironment("testOS")).Returns(oe.Object);
            oe.Setup(e => e.Load(sc, It.IsAny<IProcessorArchitecture>())).Returns(platform.Object);
            this.platform.Setup(p => p.CreateMetadata()).Returns(new TypeLibrary());
        }

        //private void Given_Platform(IPlatform platform)
        //{
        //    this.oe = new Mock<OperatingEnvironment>();
        //    this.platform = platform;
        //    this.cfgSvc.Setup(c => c.GetEnvironment("testOS")).Returns(oe);
        //    oe.Setup(e => e.Load(sc, null)).IgnoreArguments().Returns(platform);
        //}

        private void Given_TypeLibraryLoaderService()
        {
            this.tlSvc = new Mock<ITypeLibraryLoaderService>();
            sc.AddService<ITypeLibraryLoaderService>(this.tlSvc.Object);
        }

        private void Given_Binary(Mock<ILoader> ldr, IPlatform platform)
        {
            ldr.Setup(l => l.LoadFileBytes(
                It.IsAny<string>())).Returns(new byte[100]);
            ldr.Setup(l => l.Load(
                It.IsAny<ImageLocation>(),
                null,
                null)).Returns(new Func<ImageLocation, string, Address, ILoadedImage>(
                    (i, l, a) => new Program { 
                        Architecture = arch.Object,
                        Platform = platform,
                    }));
        }

        private void Given_Script(Mock<ILoader> ldr, string fileName)
        {
            var scriptUri = ImageLocation.FromUri(fileName);
            var scriptFile = new Mock<ScriptFile>(
                null, scriptUri, new byte[100]);
            ldr.Setup(l => l.LoadScript(scriptUri))
                .Returns(scriptFile.Object);
        }

        private void Expect_Arch_ParseAddress(string sExp, Address result)
        {
            arch.Setup(a => a.TryParseAddress(
                sExp,
                out result))
                .Returns(true);
        }

        public class TestPlatform : Platform
        {
            public Dictionary<string, object> Test_Options;
            public TestPlatform(IServiceProvider services) : base(services, null, "testOS") { }

            public override string DefaultCallingConvention
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override HashSet<RegisterStorage> CreateTrashedRegisters()
            {
                throw new NotImplementedException();
            }

            public override CallingConvention GetCallingConvention(string ccName)
            {
                throw new NotImplementedException();
            }

            public override SystemService FindService(int vector, ProcessorState state, SegmentMap segmentMap)
            {
                throw new NotImplementedException();
            }

            public override int GetBitSizeFromCBasicType(CBasicType cb)
            {
                throw new NotImplementedException();
            }

            public override void LoadUserOptions(Dictionary<string, object> options) { Test_Options = options; }

            public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void Prld_PlatformOptions_Scalar()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v4"">
  <arch>testArch</arch>
  <platform>testOS</platform>
  <input>
    <filename>/foo/foo</filename>
    <user>
      <platform>
        <item key=""Name"">Bob</item>
        <item key=""Name2"">Sue</item>
      </platform>
    </user>
  </input>
</project>";
            var ldr = new Mock<ILoader>();
            Given_TestArch();
            Given_TestOS();
            Given_Binary(ldr, platform.Object);
            Expect_LoadOptions();

            var prld = new ProjectLoader(sc, ldr.Object, ImageLocation.FromUri("file:/foo/bar"), listener.Object);
            prld.LoadProject(new MemoryStream(Encoding.UTF8.GetBytes(sExp)));

            Assert.AreEqual(2, loadedOptions.Count);
            Assert.AreEqual("Bob", loadedOptions["Name"]);
            Assert.AreEqual("Sue", loadedOptions["Name2"]);
        }

        private void Expect_LoadOptions()
        {
            platform.Setup(p => p.LoadUserOptions(It.IsAny<Dictionary<string,object>>()))
                .Callback((Dictionary<string,object> options) => {
                    this.loadedOptions = options;
                });
        }

        [Test]
        public void Prld_PlatformOptions_List()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v4"">
  <arch>testArch</arch>
  <platform>testOS</platform>
  <input>
    <filename>/ff/b/foo.exe</filename>,
    <user>
      <platform>
        <list key=""Names"">
          <item>Adam</item>
          <item>Bob</item>
          <item>Cecil</item>
        </list>
        <item key=""Name2"">Sue</item>
      </platform>
    </user>
  </input>
</project>";
            var ldr = new Mock<ILoader>();
            Given_TestArch();
            Given_TestOS();
            Given_Binary(ldr, platform.Object);
            Expect_LoadOptions();

            var prld = new ProjectLoader(sc, ldr.Object, ImageLocation.FromUri("/ff/b/foo.proj"), listener.Object);
            prld.LoadProject(new MemoryStream(Encoding.UTF8.GetBytes(sExp)));

            var list = (IList)loadedOptions["Names"];
            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void Prld_PlatformOptions_Dictionary()
        {
            var sproject =
    @"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v4"">
  <arch>testArch</arch>
  <platform>testOS</platform>
  <input>
    <filename>c:\foo\foo.exe</filename>
    <user>
      <platform>
        <dict key=""Names"">
          <item key=""Adam"">30</item>
          <item key=""Bob"">10</item>
          <item key=""Cecil"">120</item>
        </dict>
        <item key=""Name2"">Sue</item>
      </platform>
    </user>
  </input>
</project>";
            var ldr = new Mock<ILoader>();
            Given_TestArch();
            Given_TestOS();
            Given_Binary(ldr, platform.Object);
            Expect_LoadOptions();

            var prld = new ProjectLoader(sc, ldr.Object, ImageLocation.FromUri("c:\\foo\\bar.proj"), listener.Object);
            prld.LoadProject(new MemoryStream(Encoding.UTF8.GetBytes(sproject)));

            var list = (IDictionary)loadedOptions["Names"];
            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void Prld_MakePathsAbsolute_From_Filename()
        {
            var sProject = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                InputFiles =
                {
                    new DecompilerInput_v4
                    {
                        Filename = "foo.exe",
                    },
                }
            };
            Given_TestArch();
            Given_TestOS();
            var ldr = new Mock<ILoader>();
            ldr.Setup(l => l.Load(
                It.IsAny<ImageLocation>(),
                It.IsAny<string>(),
                It.IsAny<Address>())).Returns(new Program());
            ldr.Setup(l => l.LoadFileBytes(
                It.IsAny<string>())).Returns(new byte[1000]);

            var location = ImageLocation.FromUri(OsPath.Absolute("users", "bob", "projects", "foo.project"));
            var prld = new ProjectLoader(sc, ldr.Object, location, listener.Object);
            var project = prld.LoadProject(sProject);

            var programUri = project.Programs[0].Location;
            Assert.AreEqual(OsPath.Absolute("users","bob","projects","foo.exe"), programUri.FilesystemPath);
        }

        [Test]
        public void Prld_MakePathsAbsolute_From_Uri()
        {
            var sProject = new Project_v5
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                InputFiles =
                {
                    new DecompilerInput_v5
                    {
                        Location = "file:foo.exe",
                    },
                }
            };
            Given_TestArch();
            Given_TestOS();
            var ldr = new Mock<ILoader>();
            ldr.Setup(l => l.Load(
                It.IsAny<ImageLocation>(),
                It.IsAny<string>(),
                It.IsAny<Address>())).Returns(new Program());
            ldr.Setup(l => l.LoadFileBytes(
                It.IsAny<string>())).Returns(new byte[1000]);

            var projectPath = OsPath.Absolute("users", "bob", "projects", "foo.project");
            var location = ImageLocation.FromUri(projectPath);
            var prld = new ProjectLoader(sc, ldr.Object, location, listener.Object);
            var project = prld.LoadProject(sProject);
            Assert.AreEqual(OsPath.Absolute("users", "bob", "projects", "foo.exe"), project.Programs[0].Location.FilesystemPath);
        }


        [Test]
        public void Prld_LoadUserDefinedMetadata()
        {
            var sProject = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                InputFiles =
                {
                    new DecompilerInput_v4
                    {
                        Filename = "foo.exe",
                        User = new UserData_v4
                        {
                            LoadAddress = "00123400"
                        }
                    },
                },
                MetadataFiles = {
                    new MetadataFile_v3 {
                        Location = "file:meta1.xml",
                    },
                    new MetadataFile_v3 {
                        Filename = "meta2.xml",
                    },
                }
            };

            var types1 = new Dictionary<string, DataType>()
            {
                {"USRTYPE1", PrimitiveType.Word16}
            };
            var types2 = new Dictionary<string, DataType>()
            {
                {"USRTYPE2", PrimitiveType.Word32}
            };

            var ldr = mockFactory.CreateLoader();
            Given_TestArch();
            Given_TestOS();
            var addr = Address.Ptr32(0x00123400);
            arch.Setup(a => a.TryParseAddress(
                "00123400",
                out addr))
                .Returns(true);
            mockFactory.CreateLoadMetadataStub(
                ImageLocation.FromUri(OsPath.Absolute("meta1.xml")),
                this.platform.Object,
                new TypeLibrary(
                    false,
                    types1,
                    new Dictionary<string, FunctionType>(),
                    new Dictionary<string, ProcedureCharacteristics>(),
                    new Dictionary<string, DataType>()
                )
            );
            mockFactory.CreateLoadMetadataStub(
                ImageLocation.FromUri(OsPath.Absolute("meta2.xml")),
                this.platform.Object,
                new TypeLibrary(
                    false,
                    types2,
                    new Dictionary<string, FunctionType>(),
                    new Dictionary<string, ProcedureCharacteristics>(),
                    new Dictionary<string, DataType>()
                )
            );

            var projectPath = OsPath.Absolute("foo.project");
            var location = ImageLocation.FromUri(projectPath);
            var prld = new ProjectLoader(sc, ldr, location, listener.Object);
            var project = prld.LoadProject(sProject);
            Assert.AreEqual(2, project.Programs[0].EnvironmentMetadata.Types.Count);
            Assert.AreEqual(
                "word16",
                project.Programs[0].EnvironmentMetadata.Types["USRTYPE1"].ToString()
            );
            Assert.AreEqual(
                "word32",
                project.Programs[0].EnvironmentMetadata.Types["USRTYPE2"].ToString()
            );
            Assert.AreEqual(
                Address.Ptr32(0x00123400),
                project.Programs[0].User.LoadAddress);
        }

        [Test]
        public void Prld_LoadScripts_OldFormat()
        {
            var xml =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v5"">
  <arch>testArch</arch>
  <platform>testOS</platform>
  <script>
    <filename>fake.script</filename>
  </script>
</project>";
            var ldr = new Mock<ILoader>();
            Given_TestArch();
            Given_TestOS();
            Given_Script(ldr, AbsolutePathEndingWith("dir", "fake.script"));

            var location = ImageLocation.FromUri(AbsolutePathEndingWith("dir", "script.proj"));
            var prld = new ProjectLoader(sc, ldr.Object, location, listener.Object);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var project = prld.LoadProject(stream);

            Assert.AreEqual(
                ImageLocation.FromUri(AbsolutePathEndingWith("dir", "fake.script")),
                project.ScriptFiles.Single().Location);
        }

        [Test]
        public void Prld_LoadScripts()
        {
            var xml =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v5"">
  <arch>testArch</arch>
  <platform>testOS</platform>
  <script>
    <location>file:fake.script</location>
  </script>
</project>";
            var ldr = new Mock<ILoader>();
            Given_TestArch();
            Given_TestOS();
            Given_Script(ldr, AbsolutePathEndingWith("dir", "fake.script"));

            var location = ImageLocation.FromUri(OsPath.Absolute("dir", "script.proj"));
            var prld = new ProjectLoader(sc, ldr.Object, location, listener.Object);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var project = prld.LoadProject(stream);

            Assert.AreEqual(
                OsPath.Absolute("dir","fake.script"),
                project.ScriptFiles.Single().Location.FilesystemPath);
        }

        [Test]
        public void Prld_LoadGlobalUserData()
        {
            Given_TestArch();
            Given_TestOS();
            Expect_Arch_ParseAddress("10000010", Address.Ptr32(0x10000010));

            var sproject = new Project_v4
            {
                ArchitectureName = arch.Object.Name,
                PlatformName = platform.Object.Name,
                InputFiles =
                {
                    new DecompilerInput_v4
                    {
                        Filename = "c:\\tmp\\foo\\foo.exe",
                        User = new UserData_v4
                        {
                            GlobalData =
                            {
                                new GlobalDataItem_v2
                                {
                                    Address = "10000010",
                                    Name = "testVar",
                                    DataType = new ArrayType_v1
                                    {
                                        ElementType = new TypeReference_v1
                                        {
                                             TypeName = "Blob"
                                        },
                                        Length = 10
                                    }
                                }
                            }
                        }
                    },
                }
            };
            var ldr = mockFactory.CreateLoader();
            var location = ImageLocation.FromUri(@"c:\foo\global_user.proj");
            var prld = new ProjectLoader(sc, ldr, location, listener.Object);
            
            var project = prld.LoadProject(sproject);

            Assert.AreEqual(1, project.Programs.Count);
            Assert.AreEqual(1, project.Programs[0].User.Globals.Count);
            var globalVariable = project.Programs[0].User.Globals.Values[0];
            Assert.AreEqual("10000010", globalVariable.Address.ToString());
            Assert.AreEqual("testVar", globalVariable.Name);
            Assert.AreEqual("arr(Blob,10)", globalVariable.DataType.ToString());
        }

        [Test]
        public void Prld_LoadAnnotation()
        {
            var sproject = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                InputFiles =
                {
                    new DecompilerInput_v4
                    {
                        Filename = "c:\\tmp\\foo\\foo.exe",
                        User = new UserData_v4
                        {
                            Annotations =
                            {
                                new Annotation_v3
                                {
                                    Address = "commentAddress",
                                    Text = "User comment",
                                }
                            }
                        }
                    },
                }
            };
            var ldr = mockFactory.CreateLoader();
            Given_TestArch();
            Given_TestOS();
            var addr = Address.Ptr32(0x0000CADD);
            arch.Setup(a => a.TryParseAddress(
                "commentAddress",
                out addr))
                .Returns(true);

            var location = ImageLocation.FromUri(@"c:\foo\annot.proj");
            var prld = new ProjectLoader(sc, ldr, location, listener.Object);
            var project = prld.LoadProject(
                sproject);

            Assert.AreEqual(1, project.Programs.Count);
            Assert.AreEqual(1, project.Programs[0].User.Annotations.Count());
            var annotation = project.Programs[0].User.Annotations.First();
            Assert.AreEqual("0000CADD", annotation.Address.ToString());
            Assert.AreEqual("User comment", annotation.Text);
        }

        [Test(Description = "Issue #9: if user proc has no Decompile set but no signature provided")]
        public void Prld_Partial_UserProc()
        {
            var sProject = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                InputFiles =
                {
                    new DecompilerInput_v4
                    {
                        Filename = "c:\\tmp\\foo\\foo.exe",
                        User = new UserData_v4
                        {
                            Procedures =
                            {
                                new Procedure_v1
                                {
                                    Address = "00123400",
                                    Decompile = false,
                                }
                            }
                        }
                    }
                }
            };

            var ldr = mockFactory.CreateLoader();
            Given_TestArch();
            Given_TestOS();
            var addrNav = new Mock<ICodeLocation>();
            listener.Setup(l => l.CreateAddressNavigator(It.IsAny<Program>(), It.IsAny<Address>()))
                .Returns(addrNav.Object);
            listener.Setup(l => l.Warn(
                It.IsAny<ICodeLocation>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()))
                .Verifiable();

            var location = ImageLocation.FromUri("foo.dcproject");
            var prld = new ProjectLoader(sc, ldr, location, listener.Object);
            prld.LoadProject(sProject);

            listener.Verify();
        }

        public void Prld_EnableExtractResources()
        {
            var sProject = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                InputFiles =
                {
                    new DecompilerInput_v4
                    {
                        User = new UserData_v4
                        {
                            ExtractResources = true
                        }
                    }
                }
            };

            var ldr = mockFactory.CreateLoader();
            Given_TestArch();
            Given_TestOS();
            var location = ImageLocation.FromUri("foo.dcproject");
            var prld = new ProjectLoader(sc, ldr, location, listener.Object);
            var project = prld.LoadProject(sProject);

            Assert.IsTrue(project.Programs[0].User.ExtractResources);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Prld_UserSegments()
        {
            var sProject = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                InputFiles =
                {
                    new DecompilerInput_v4
                    {
                        Filename = "foo.exe",
                        User = new UserData_v4
                        {
                            Segments =
                            {
                                new Segment_v4
                                {
                                     Name="text", Address="00123400", Length="C00", Architecture="testArch", Access="r-x",
                                },
                                new Segment_v4
                                {
                                     Name="data", Address="00200000", Offset="C00", Length="1200", Access="rw" 
                                },
                            }
                        }
                    }
                }
            };

            var ldr = mockFactory.CreateLoader();
            Given_TestArch();
            Given_TestOS();
            Address addr = Address.Ptr32(0x00123400);
            platform.Setup(p => p.TryParseAddress("00123400", out addr)).Returns(true);
            addr = Address.Ptr32(0x00200000);
            platform.Setup(p => p.TryParseAddress("00200000", out addr)).Returns(true);

            var location = ImageLocation.FromUri("foo.dcproject");
            var prld = new ProjectLoader(sc, ldr, location, listener.Object);
            var prj = prld.LoadProject(sProject);

            var u = prj.Programs[0].User;
            Assert.AreEqual(2, u.Segments.Count);

            Assert.AreEqual("text", u.Segments[0].Name);
            Assert.AreEqual("00123400", u.Segments[0].Address.ToString());
            Assert.AreEqual(0xC00, u.Segments[0].Length);
            Assert.AreEqual(0, u.Segments[0].Offset);
            Assert.AreEqual(AccessMode.ReadExecute, u.Segments[0].AccessMode);

            Assert.AreEqual("data", u.Segments[1].Name);
            Assert.AreEqual("00200000", u.Segments[1].Address.ToString());
            Assert.AreEqual(0x1200, u.Segments[1].Length);
            Assert.AreEqual(0x0C00, u.Segments[1].Offset);
            Assert.AreEqual(AccessMode.ReadWrite, u.Segments[1].AccessMode);
        }
        
        [Test]
        [Category(Categories.UnitTests)]
        public void Prld_Procedure_Placement()
        {
            var sProject = new Project_v5
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                InputFiles =
                {
                    new DecompilerInput_v5
                    {
                        Filename = Path.Combine("foo", "bar.exe"),
                        User = new UserData_v4
                        {
                            Procedures =
                            {
                                new Procedure_v1
                                {
                                    Address = "00123400",
                                    OutputFile = Path.Combine("src","bar.c")
                                }
                            }
                        }
                    },
                }
            };
            var ldr = mockFactory.CreateLoader();
            Given_TestArch();
            Given_TestOS();
            Address addr = Address.Ptr32(0x00123400);
            platform.Setup(p => p.TryParseAddress("00123400", out addr)).Returns(true);

            var location = ImageLocation.FromUri("/foo/foo.dcproject");
            var prld = new ProjectLoader(sc, ldr, location, listener.Object);
            var prj = prld.LoadProject(sProject);

            var u = prj.Programs[0].User;
            var placement = u.ProcedureSourceFiles[addr];
            Assert.IsTrue(IsPathEndingWith(placement, "foo/src/bar.c"));
        }
    }
}
