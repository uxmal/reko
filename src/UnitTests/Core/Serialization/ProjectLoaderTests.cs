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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Reko.Core.Configuration;
using Reko.Core.CLanguage;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class ProjectLoaderTests
    {
        private MockRepository mr;
        private MockFactory mockFactory;
        private ServiceContainer sc;
        private IConfigurationService cfgSvc;
        private IPlatform platform;
        private IProcessorArchitecture arch;
        private Dictionary<string, object> loadedOptions;
        private ITypeLibraryLoaderService tlSvc;
        private OperatingEnvironment oe;
        private DecompilerEventListener listener;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.mockFactory = new MockFactory(mr);
            this.sc = new ServiceContainer();
            this.cfgSvc = mr.Stub<IConfigurationService>();
            this.listener = mr.Stub<DecompilerEventListener>();
            this.sc.AddService<IConfigurationService>(cfgSvc);
        }

        private void Given_TestArch()
        {
            this.arch = mr.Stub<IProcessorArchitecture>();
            this.cfgSvc.Stub(c => c.GetArchitecture("testArch")).Return(arch);
        }

        private void Given_TestOS()
        {
            this.oe = mr.Stub<OperatingEnvironment>();
            this.platform = mr.Stub<IPlatform>();
            this.cfgSvc.Stub(c => c.GetEnvironment("testOS")).Return(oe);
            oe.Stub(e => e.Load(sc, null)).IgnoreArguments().Return(platform);
            this.platform.Stub(p => p.CreateMetadata()).Return(new TypeLibrary());
        }

        private void Given_Platform(IPlatform platform)
        {
            this.oe = mr.Stub<OperatingEnvironment>();
            this.platform = platform;
            this.cfgSvc.Stub(c => c.GetEnvironment("testOS")).Return(oe);
            oe.Stub(e => e.Load(sc, null)).IgnoreArguments().Return(platform);
        }

        private void Given_TypeLibraryLoaderService()
        {
            this.tlSvc = mr.Stub<ITypeLibraryLoaderService>();
            sc.AddService<ITypeLibraryLoaderService>(this.tlSvc);
        }

        [Test(Description = "If the project file just has a single metadata file, we don't know what the platform is; so ask the user.")]
        public void Prld_LoadMetadata_NoPlatform_ShouldQuery()
        {
            var ldr = mr.Stub<ILoader>();
            var oracle = mr.StrictMock<IOracleService>();
            var platform = mockFactory.CreatePlatform();
            var typeLib = new TypeLibrary();
            ldr.Stub(l => l.LoadMetadata(Arg<string>.Is.NotNull, Arg<IPlatform>.Is.Equal(platform), Arg<TypeLibrary>.Is.NotNull)).Return(typeLib);
            oracle.Expect(o => o.QueryPlatform(Arg<string>.Is.NotNull)).Return(platform);
            sc.AddService<IOracleService>(oracle);
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr, listener);
            prld.LoadProject(
                "project.dcproj",
                new Project_v2
                {
                    Inputs = {
                        new MetadataFile_v2 {
                            Filename = "foo",
                        }
                    }
                });
            mr.VerifyAll();
        }

          private void Given_Binary(ILoader ldr, IPlatform platform)
        {
            ldr.Stub(l => l.LoadImageBytes(
                Arg<string>.Is.Anything,
                Arg<int>.Is.Anything)).Return(new byte[100]);
            ldr.Stub(l => l.LoadExecutable(
                Arg<string>.Is.Anything,
                Arg<byte[]>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<Address>.Is.Anything)).Return(new Program { Platform = platform, Architecture = arch });
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

            public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
            {
                throw new NotImplementedException();
            }

            public override HashSet<RegisterStorage> CreateTrashedRegisters()
            {
                throw new NotImplementedException();
            }

            public override CallingConvention GetCallingConvention(string ccName)
            {
                throw new NotImplementedException();
            }

            public override SystemService FindService(int vector, ProcessorState state)
            {
                throw new NotImplementedException();
            }

            public override int GetByteSizeFromCBasicType(CBasicType cb)
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
    <user>
      <platform>
        <item key=""Name"">Bob</item>
        <item key=""Name2"">Sue</item>
      </platform>
    </user>
  </input>
</project>";
            var ldr = mr.Stub<ILoader>();
            Given_TestArch();
            Given_TestOS();
            Given_Binary(ldr, platform);
            Expect_LoadOptions();
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr, listener);
            prld.LoadProject("/foo/bar", new MemoryStream(Encoding.UTF8.GetBytes(sExp)));

            Assert.AreEqual(2, loadedOptions.Count);
            Assert.AreEqual("Bob", loadedOptions["Name"]);
            Assert.AreEqual("Sue", loadedOptions["Name2"]);
        }

        private void Expect_LoadOptions()
        {
            platform.Stub(p => p.LoadUserOptions(null))
                .IgnoreArguments()
                .Do(new Action<Dictionary<string, object>>(options => { this.loadedOptions = options; }));
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
            var ldr = mr.Stub<ILoader>();
            Given_TestArch();
            Given_TestOS();
            Given_Binary(ldr, platform);
            Expect_LoadOptions();
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr, listener);
            prld.LoadProject("/ff/b/foo.proj", new MemoryStream(Encoding.UTF8.GetBytes(sExp)));

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
            var ldr = mr.Stub<ILoader>();
            Given_TestArch();
            Given_TestOS();
            Given_Binary(ldr, platform);
            Expect_LoadOptions();

            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr, listener);
            prld.LoadProject("c:\\foo\\bar.proj", new MemoryStream(Encoding.UTF8.GetBytes(sproject)));

            var list = (IDictionary)loadedOptions["Names"];
            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void Prld_MakePathsAbsolute()
        {
            var sProject = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                Inputs =
                {
                    new DecompilerInput_v4
                    {
                        Filename = "foo.exe",
                    }
                }
            };
            Given_TestArch();
            Given_TestOS();
            var ldr = mr.Stub<ILoader>();
            ldr.Stub(l => l.LoadExecutable(null, null, null, null)).IgnoreArguments().Return(new Program());
            ldr.Stub(l => l.LoadImageBytes(null, 0)).IgnoreArguments().Return(new byte[1000]);
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr, listener);
            var project = prld.LoadProject(OsPath.Absolute("users", "bob", "projects", "foo.project"), sProject);
            Assert.AreEqual(OsPath.Absolute("users", "bob", "projects", "foo.exe"), project.Programs[0].Filename);
        }

        [Test]
        public void Prld_LoadUserDefinedMetadata()
        {
            var sProject = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                Inputs =
                {
                    new DecompilerInput_v4
                    {
                        Filename = "foo.exe",
                        User = new UserData_v4
                        {
                            LoadAddress = "00123400"
                        }
                    },
                    new MetadataFile_v3 {
                        Filename = "meta1.xml",
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
            arch.Stub(a => a.TryParseAddress(
                Arg<string>.Is.Equal("00123400"),
                out Arg<Address>.Out(Address.Ptr32(0x00123400)).Dummy))
                .Return(true);
            mockFactory.CreateLoadMetadataStub(
                OsPath.Absolute("meta1.xml"),
                this.platform,
                new TypeLibrary(
                    types1, new Dictionary<string, FunctionType>(), new Dictionary<string, DataType>()
                )
            );
            mockFactory.CreateLoadMetadataStub(
                OsPath.Absolute("meta2.xml"),
                this.platform,
                new TypeLibrary(
                    types2, new Dictionary<string, FunctionType>(), new Dictionary<string, DataType>()
                )
            );
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr, listener);
            var project = prld.LoadProject(OsPath.Absolute("foo.project"), sProject);
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
        public void Prld_v2()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns=""http://schemata.jklnet.org/Decompiler/v2"">
  <input>
  </input>
</project>";
            var ldr = mr.Stub<ILoader>();
            var platform = new TestPlatform(sc);
            Given_Binary(ldr, platform);
            Given_TypeLibraryLoaderService();
            cfgSvc.Stub(c => c.GetEnvironment("testOS")).Return(new OperatingEnvironmentElement
            {

            });
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr, listener);
            var project = prld.LoadProject("/foo/bar", new MemoryStream(Encoding.UTF8.GetBytes(sExp)));

            Assert.AreEqual(1, project.Programs.Count);
        }

        [Test(Description = "Failure to load v3 project file")]
        public void Prld_issue_299()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://schemata.jklnet.org/Reko/v3"">
  <input>
    <filename>switch.dll</filename>
    <disassembly>switch.asm</disassembly>
    <intermediate-code>switch.dis</intermediate-code>
    <output>switch.c</output>
    <types-file>switch.h</types-file>
    <global-vars>switch.globals.c</global-vars>
    <user>
      <procedure name=""get"">
        <address>10071000</address>
        <CSignature>char * get(unsigned int n)</CSignature>
      </procedure>
      <heuristic name=""shingle"" />
    </user>
  </input>
</project>
";
            var ldr = mr.Stub<ILoader>();
            var platform = new TestPlatform(sc);
            Given_TestArch();
            Given_TestOS();
            Given_Binary(ldr, platform);
            Given_TypeLibraryLoaderService();
            oe.Stub(o => o.TypeLibraries).Return(new List<ITypeLibraryElement>());
            oe.Stub(o => o.CharacteristicsLibraries).Return(new List<ITypeLibraryElement>());
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr, listener);
            var project = prld.LoadProject("/foo/bar", new MemoryStream(Encoding.UTF8.GetBytes(sExp)));

            Assert.AreEqual(1, project.Programs.Count);
        }
       
        [Test]
        public void Prld_LoadGlobalUserData()
        {
            var sproject = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                Inputs =
                {
                    new DecompilerInput_v4
                    {
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
            Given_TestArch();
            Given_TestOS();
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr, listener);
            var project = prld.LoadProject(
                @"c:\foo\global_user.proj",
                sproject);

            Assert.AreEqual(1, project.Programs.Count);
            Assert.AreEqual(1, project.Programs[0].User.Globals.Count);
            var globalVariable = project.Programs[0].User.Globals.Values[0];
            Assert.AreEqual("10000010", globalVariable.Address);
            Assert.AreEqual("testVar", globalVariable.Name);
            Assert.AreEqual("arr(Blob,10)", globalVariable.DataType.ToString());
        }

        [Test(Description = "Issue #9: if user proc has no Decompile set but no signature provided")]
        public void Prld_Partial_UserProc()
        {
            var sProject = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                Inputs =
                {
                    new DecompilerInput_v4
                    {
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
            var addrNav = mr.Stub<ICodeLocation>();
            listener.Stub(l => l.CreateAddressNavigator(null, null))
                .IgnoreArguments()
                .Return(addrNav);
            listener.Expect(l => l.Warn(null, null, null))
                .IgnoreArguments();
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr, listener);
            prld.LoadProject("foo.dcproject", sProject);

            mr.VerifyAll();
        }
    }
}
