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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Scripts;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
	public class SerializedProjectTests
	{
        private CommonMockFactory mockFactory;
        private ServiceContainer sc;
        private Mock<ILoader> loader;
        private Mock<IProcessorArchitecture> arch;
        private Mock<IPlatform> platform;
        private Mock<IConfigurationService> cfgSvc;
        private Mock<IEventListener> listener;

        [SetUp]
        public void Setup()
        {
            this.mockFactory = new CommonMockFactory();
            this.cfgSvc = new Mock<IConfigurationService>();
            this.listener = new Mock<IEventListener>();
            this.sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemService('/'));
            sc.AddService<IConfigurationService>(cfgSvc.Object);
        }

        private void Given_Architecture()
        {
            this.arch = new Mock<IProcessorArchitecture>();
            this.arch.Setup(a => a.Name).Returns("testArch");
            this.arch.Setup(a => a.SaveUserOptions()).Returns((Dictionary<string,object>)null);
            this.cfgSvc.Setup(c => c.GetArchitecture("testArch")).Returns(arch.Object);
        }

        private void Given_TestOS_Platform()
        {
            Debug.Assert(arch.Object is not null, "Must call Given_Architecture first.");
            // A very simple dumb platform with no intelligent behaviour.
            this.platform = new Mock<IPlatform>();
            var oe = new Mock<PlatformDefinition>();
            this.platform.Setup(p => p.Name).Returns("testOS");
            this.platform.Setup(p => p.SaveUserOptions()).Returns((Dictionary<string,object>) null);
            this.platform.Setup(p => p.Architecture).Returns(arch.Object);
            this.platform.Setup(p => p.CreateMetadata()).Returns(new TypeLibrary());
            this.platform.Setup(p => p.DefaultCallingConvention).Returns("");
            this.cfgSvc.Setup(c => c.GetEnvironment("testOS")).Returns(oe.Object);
            oe.Setup(e => e.Load(sc, arch.Object)).Returns(platform.Object);
        }

        private void Expect_TryParseAddress(string sAddr, Address addr)
        {
            platform.Setup(p => p.TryParseAddress(
                sAddr, out addr))
                .Returns(true);
        }

        private void Expect_TryGetRegister(Mock<IProcessorArchitecture> arch, string regName, RegisterStorage reg)
        {
            arch.Setup(a => a.TryGetRegister(
                regName, out reg))
                .Returns(true);
        }

        [Test]
        public void SudWrite()
        {
            Project_v5 ud = new Project_v5
            {
                InputFiles =
                {
                    new DecompilerInput_v5
                    {
                        DisassemblyDirectory = "",
                        User = new UserData_v4 {
                            ExtractResources = true,
                            Procedures =
                            {
                                new Procedure_v1
                                {
                                    Name = "foo",
                                    Signature = new SerializedSignature
                                    {
                                        ReturnValue = new Argument_v1
                                        {
                                            Kind = new Register_v1("eax")
                                        },
                                        Arguments = new Argument_v1[]
                                        {
                                            new Argument_v1
                                            {
                                                Kind = new StackVariable_v1(),
                                                Type = new PrimitiveType_v1(Domain.SignedInt, 4)
                                            },
                                            new Argument_v1
                                            {
                                                Kind = new StackVariable_v1(),
                                                Type = new PrimitiveType_v1(Domain.SignedInt, 4)
                                            }
                                        }
                                    }
                                }
                            },
                            LoadAddress = "1000:0000",
                            IndirectJumps =
                            {
                                new IndirectJump_v4 {
                                    InstructionAddress ="1000:0220",
                                    TableAddress ="1000:0228"
                                },
                            },
                            JumpTables = {
                                new JumpTable_v4
                                {
                                    TableAddress = "1000:0228",
                                    Destinations = new string[]
                                    {
                                        "1000:0230",
                                        "1000:0244",
                                        "1000:033F",
                                    }
                                }
                            }
                        }
                    }
                }
            };

			using (FileUnitTester fut = new FileUnitTester("Core/SudWrite.txt"))
			{
			    var writer = new FilteringXmlWriter(fut.TextWriter);
				writer.Formatting = System.Xml.Formatting.Indented;
                XmlSerializer ser = SerializedLibrary.CreateSerializer_v5(typeof(Project_v5));
				ser.Serialize(writer, ud);
				fut.AssertFilesEqual();
			}
		}

        [Test]
        public void SudSaveProject()
        {
            Given_Architecture();
            Given_TestOS_Platform();
            var eax = RegisterStorage.Reg32("eax", 0);
            var ecx = RegisterStorage.Reg32("ecx", 1);
            var jumpTable = new ImageMapVectorTable(
                Address.SegPtr(0x1000, 0x400),
                new Address[] {
                    Address.SegPtr(0x1000, 0x500),
                    Address.SegPtr(0x1000, 0x513),
                    Address.SegPtr(0x1000, 0x5BA),
                }, 0);

            Project project = new Project
            {
                Programs =
                {
                    new Program
                    {
                        Architecture = arch.Object,
                        Platform = platform.Object,
                        SegmentMap = new SegmentMap(Address.SegPtr(0x1000, 0)), //, new byte[100]),
                        DisassemblyDirectory = "",
                        User = new UserData
                        {
                            Loader = "CustomLoader",
                            ExtractResources = true,
                            Procedures =
                            {
                                {
                                    Address.SegPtr(0x1000, 0x10),
                                    new UserProcedure(Address.SegPtr(0x1000, 0x10), "foo")
                                    {
                                        Signature = new SerializedSignature
                                        {
                                            ReturnValue = new Argument_v1 { Kind = new Register_v1("eax") },
                                            Arguments = new Argument_v1[]
                                            {
                                                new Argument_v1
                                                {
                                                    Kind = new StackVariable_v1(),
                                                    Type = new PrimitiveType_v1(Domain.SignedInt, 4)
                                                },
                                                new Argument_v1
                                                {
                                                    Kind = new StackVariable_v1(),
                                                    Type = new PrimitiveType_v1(Domain.SignedInt, 4)
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Globals =
                            {
                                {
                                  Address.SegPtr(0x2000, 0),
                                  new UserGlobal(Address.SegPtr(0x2000, 0), "g_20000", new StringType_v2 {
                                           Termination=StringType_v2.ZeroTermination,
                                           CharType = new PrimitiveType_v1 { Domain = Domain.Character, ByteSize = 1 }
                                       })
                                }
                            },
                            Calls =
                            {
                                {
                                    Address.SegPtr(0x1000, 0x0320),
                                    new UserCallData
                                    {
                                        Address = Address.SegPtr(0x1000, 0x0320),
                                        NoReturn = true,
                                    }
                                }
                            },
                            RegisterValues =
                            {
                                {
                                    Address.Ptr32(0x012310),
                                    new List<UserRegisterValue>
                                    {
                                        new UserRegisterValue(eax, Constant.Word32(0x01231)),
                                        new UserRegisterValue(ecx, Constant.Word32(0x42424711)),
                                    }
                                }
                            },
                            IndirectJumps =
                            {
                                {
                                    Address.SegPtr(0x1000, 0x380),
                                    new UserIndirectJump {
                                        Address = jumpTable.Address,
                                        Table = jumpTable,
                                        IndexRegister = RegisterStorage.Reg32("R1", 1)
                                   }
                                }
                            },
                            JumpTables =
                            {
                                { jumpTable.Address, jumpTable }
                            },
                            OutputFilePolicy = Program.SegmentFilePolicy,
                            RenderInstructionsCanonically = true,
                        }
                    }
                }
            };

            using (FileUnitTester fut = new FileUnitTester("Core/SudSaveProject.txt"))
            {
                FilteringXmlWriter writer = new FilteringXmlWriter(fut.TextWriter);
                writer.Formatting = System.Xml.Formatting.Indented;
                XmlSerializer ser = SerializedLibrary.CreateSerializer_v5(typeof(Project_v5));
                Project_v5 ud = new ProjectSaver(sc).Serialize(ImageLocation.FromUri("/var/foo/foo.proj"), project);
                ser.Serialize(writer, ud);
                fut.AssertFilesEqual();
            }
        }

        [Test]
        public void SudSaveScripts()
        {
            var project = new Project
            {
                ScriptFiles =
                {
                    new Mock<ScriptFile>(
                        null, ImageLocation.FromUri("/var/foo/script.fake"), new byte[100]).Object
                },
            };
            var sw = new StringWriter();
            var writer = new FilteringXmlWriter(sw)
            {
                Formatting = System.Xml.Formatting.Indented
            };
            var ser = SerializedLibrary.CreateSerializer_v5(
                typeof(Project_v5));

            var saver = new ProjectSaver(sc);
            var sProject = saver.Serialize(ImageLocation.FromUri("/var/foo/foo.proj"), project);
            ser.Serialize(writer, sProject);

            var expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v5"">
  <script>
    <location>script.fake</location>
  </script>
</project>";
            if (sw.ToString() != expected)
                Console.WriteLine("{0}", sw.ToString());
            Assert.AreEqual(expected, sw.ToString());
        }

        [Test]
        public void Sud_LoadProject_Inputs_v5()
        {
            Given_Loader();
            Given_Architecture();
            Given_TestOS_Platform();
            Expect_Arch_ParseAddress("1000:0400", Address.SegPtr(0x1000, 0x0400));
            Given_ExecutableProgram("foo.exe", Address.SegPtr(0x1000, 0x0000));
            Given_BinaryFile("foo.bin", Address.SegPtr(0x1000, 0x0000));

            var sProject = new Project_v5
            {
                ArchitectureName = arch.Object.Name,
                PlatformName = platform.Object.Name,
                InputFiles = {
                    new DecompilerInput_v5 {
                        Filename = "foo.exe",
                        User = {
                            LoadAddress = "1000:0000",
                            GlobalData = {
                                new GlobalDataItem_v2 { Address = "1000:0400", DataType = new StringType_v2 {
                                    Termination=StringType_v2.ZeroTermination,
                                    CharType= new PrimitiveType_v1 { ByteSize = 1, Domain=Domain.Character} }
                                }
                            }
                        },
                        Comment = "main file",
                    },
                    new DecompilerInput_v5 {
                        Filename = "foo.bin",
                        Comment = "overlay",
                        User = { 
                            LoadAddress = "1000:D000",
                            Processor = new ProcessorOptions_v4 {
                                Name = "x86-real-16",
                            }
                        }
                    }
                }
            };

            var location = ImageLocation.FromUri("/var/project.dcproj");
            var ps = new ProjectLoader(sc, loader.Object, location, listener.Object);
            var project = ps.LoadProject(sProject);

            Assert.AreEqual(2, project.Programs.Count);
            var input0 = project.Programs[0];
            Assert.AreEqual(1, input0.User.Globals.Count);
            Assert.AreEqual("1000:0400", input0.User.Globals.Values[0].Address.ToString());
            var str_t = (StringType_v2)input0.User.Globals.Values[0].DataType;
            Assert.AreEqual("prim(Character,1)", str_t.CharType.ToString());
        }

        private void Given_BinaryFile(string exeName, Address address)
        {
            var bytes = new byte[0x10];
            var program = new Program
            {
                Architecture = arch.Object,
                SegmentMap = new SegmentMap(
                    address,
                    new ImageSegment(
                        ".text", 
                        new ByteMemoryArea(address, bytes),
                        AccessMode.ReadWriteExecute))
            };
            loader.Setup(l => l.LoadFileBytes(
                It.Is<string>(s => s.EndsWith(exeName)))).Returns(bytes);
            loader.Setup(l => l.Load(
                It.Is<ImageLocation>(s => s.EndsWith(exeName)),
                null,
                null)).Returns(program);
        }

        private void Given_ExecutableProgram(string exeName, Address address)
        {
            var bytes = new byte[0x1000];
            var mem = new ByteMemoryArea(address, bytes);
            var segmentMap = new SegmentMap(address,
                    new ImageSegment(".text", mem, AccessMode.ReadWriteExecute));
            var program = new Program
            {
                Architecture = arch.Object,
                Platform = mockFactory.CreateMockPlatform().Object,
                SegmentMap = segmentMap,
                ImageMap = segmentMap.CreateImageMap()
            };
            loader.Setup(l => l.LoadFileBytes(
                It.Is<string>(s => s.EndsWith(exeName)))).Returns(bytes);
            loader.Setup(l => l.Load(
                It.Is<ImageLocation>(s => s.EndsWith(exeName)),
                null,
                null)).Returns(program);
        }

        private void Expect_Arch_ParseAddress(string sExp, Address result)
        {
            arch.Setup(a => a.TryParseAddress(
                sExp,
                out result))
                .Returns(true);
        }

        private void Given_Loader()
        {
            this.loader = new Mock<ILoader>();
        }

        [Test]
        public void Sud_SaveMetadataReference()
        {
            Given_Architecture();
            Given_TestOS_Platform();
            var project = new Project
            {
                Programs =
                {
                    new Program
                    {
                        Architecture = arch.Object,
                        Platform = platform.Object,
                        Location = ImageLocation.FromUri(OsPath.Absolute("test","foo.exe")),
                    }
                },
                MetadataFiles =
                {
                    new MetadataFile
                    {
                        Location = ImageLocation.FromUri(OsPath.Absolute("test","foo.def")),
                        ModuleName = "foo.def",
                    }
                }
            };

            var ps = new ProjectSaver(sc);
            var sProject = ps.Serialize(ImageLocation.FromUri(OsPath.Absolute("test","foo.project")), project);
            Assert.AreEqual(1, sProject.MetadataFiles.Count);
            Assert.AreEqual("foo.def", sProject.MetadataFiles[0].Location);
            Assert.AreEqual("foo.def", sProject.MetadataFiles[0].ModuleName);
        }

        [Test]
        public void SudLoadMetadata()
        {
            Given_Architecture();
            Given_TestOS_Platform();

            var sProject = new Project_v5 {
                ArchitectureName = arch.Object.Name,
                PlatformName = platform.Object.Name,
                MetadataFiles =
                {
                    new MetadataFile_v3
                    {
                        Filename = "c:\\tmp\\foo.def"
                    }
                }
            };
            var loader = new Mock<ILoader>();
            var typelib = new TypeLibrary();
            loader.Setup(l => l.LoadMetadata(
                It.IsAny<ImageLocation>(),
                It.IsAny<IPlatform>(),
                It.IsAny<TypeLibrary>()))
                .Returns(typelib);

            var location = ImageLocation.FromUri("c:\\bar\\bar.dcproj");
            var ploader = new ProjectLoader(sc, loader.Object, location, listener.Object);
            var project = ploader.LoadProject(sProject);

            Assert.AreEqual(1, project.MetadataFiles.Count);
            Assert.IsTrue(project.MetadataFiles[0].Location.EndsWith("foo.def"));
        }

        [Test]
        public void SudLoadProgramOptions()
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
                            Heuristics = { new Heuristic_v3 { Name="HeuristicScanning" } },
                            TextEncoding = "utf-16",
                            Calls =
                            {
                                new SerializedCall_v1
                                {
                                    InstructionAddress = "0041230",
                                    NoReturn = true, 
                                }
                            },
                            RegisterValues = new RegisterValue_v2[]
                            {
                                new RegisterValue_v2 { Address="00443210", Register="eax", Value="42" },
                                new RegisterValue_v2 { Address="00443210", Register="ecx", Value="10" },
                            }
                        }
                    }
                }
            };
            Given_Architecture();
            Given_TestOS_Platform();
            Expect_TryParseAddress("0041230", Address.Ptr32(0x0041230));
            Expect_TryParseAddress("00443210", Address.Ptr32(0x00443210));
            Expect_TryGetRegister(arch, "eax", RegisterStorage.Reg32("eax", 0));
            Expect_TryGetRegister(arch, "ecx", RegisterStorage.Reg32("ecx", 1));
            var loader = new Mock<ILoader>();
            loader.Setup(l => l.LoadFileBytes(It.IsAny<string>()))
                .Returns(new byte[10]);
            loader.Setup(l => l.Load(
                It.IsAny<ImageLocation>(),
                It.IsAny<string>(),
                It.IsAny<Address?>()))
                .Returns(new Program
                {
                    Platform = this.platform.Object
                });

            var location = ImageLocation.FromUri("c:\\tmp\\foo\\bar.proj");
            var ploader = new ProjectLoader(sc, loader.Object, location, new FakeDecompilerEventListener());
            var project = ploader.LoadProject(sProject);
            Assert.IsTrue(project.Programs[0].User.Heuristics.Contains("HeuristicScanning"));
            Assert.AreEqual("utf-16", project.Programs[0].User.TextEncoding.WebName);
            Assert.AreEqual(1, project.Programs[0].User.RegisterValues.Count);
            Assert.AreEqual(2, project.Programs[0].User.RegisterValues[Address.Ptr32(0x00443210)].Count);
        }

        [Test]
        public void SudSaveProgramOptions()
        {
            var program = new Program();
            program.User.Heuristics.Add("shingle");
            program.User.TextEncoding = Encoding.GetEncoding("utf-8");
        
            var pSaver = new ProjectSaver(sc);
            var file = pSaver.VisitProgram(ImageLocation.FromUri("file:foo.proj"), program);
            var ip = (DecompilerInput_v5)file;
            Assert.IsTrue(ip.User.Heuristics.Any(h => h.Name == "shingle"));
            Assert.AreEqual("utf-8", ip.User.TextEncoding);
        }

        private void When_SaveToTextWriter(Program program, TextWriter sw)
        {
            var saver = new ProjectSaver(sc);
            var sProj = new Project_v5
            {
                InputFiles = { saver.VisitProgram(ImageLocation.FromUri("file:foo.exe"), program) }
            };
            var writer = new FilteringXmlWriter(sw);
            writer.Formatting = System.Xml.Formatting.Indented;
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v5(typeof(Project_v5));
            ser.Serialize(writer, sProj);
        }

        private class TestPlatform : DefaultPlatform
        {
            public Dictionary<string, object> Test_Options;
            public TestPlatform() : base(null, null) { }
            public override Dictionary<string, object> SaveUserOptions() { return Test_Options; }
            public override void LoadUserOptions(Dictionary<string,object> options) { Test_Options = options; }
        }

        [Test]
        public void SudSavePlatformOptions_Scalar()
        {
            var platform = new TestPlatform
            {
                Test_Options = new Dictionary<string, object>
                {
                    { "Name", "Bob" },
                    { "Name2", "Sue" },
                }
            };
            var program = new Program
            {
                Platform = platform
            };
            var sw = new StringWriter();
            When_SaveToTextWriter(program, sw);
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v5"">
  <input>
    <user>
      <platform>
        <item key=""Name"">Bob</item>
        <item key=""Name2"">Sue</item>
      </platform>
      <registerValues />
      <extractResources>false</extractResources>
    </user>
  </input>
</project>";
            if (sw.ToString() != sExp)
                Debug.Print("{0}", sw.ToString());
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void SudSavePlatformOptions_List()
        {
            var platform = new TestPlatform
            {
                Test_Options = new Dictionary<string, object>
                {
                    { "Names", new List<string> { "Adam", "Bob", "Cecil" } },
                    { "Name2", "Sue" },
                }
            };
            var program = new Program
            {
                Platform = platform
            };
            var sw = new StringWriter();
            When_SaveToTextWriter(program, sw);
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v5"">
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
      <registerValues />
      <extractResources>false</extractResources>
    </user>
  </input>
</project>";
            if (sw.ToString() != sExp)
                Debug.Print("{0}", sw.ToString());
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void SudSavePlatformOptions_Dictionary()
        {
            var platform = new TestPlatform
            {
                Test_Options = new Dictionary<string, object>
                {
                    {
                        "Names", new Dictionary<string, object> {
                            {  "Adam", "30" },
                            {  "Bob", "10" },
                            {  "Cecil", "120" }
                        }
                    },
                    { "Name2", "Sue" },
                }
            };
            var program = new Program
            {
                Platform = platform
            };
            var sw = new StringWriter();
            When_SaveToTextWriter(program, sw);
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v5"">
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
      <registerValues />
      <extractResources>false</extractResources>
    </user>
  </input>
</project>";
            if (sw.ToString() != sExp)
                Console.WriteLine("{0}", sw.ToString());
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void SudSave_UserGlobal_issue_201()
        {
            var platform = new TestPlatform
            {
            };
            var program = new Program
            {
                Platform = platform,
                User = new UserData
                {
                    ExtractResources = true,
                    Globals =
                    {
                        {
                            Address.Ptr32(0x01234),
                            new UserGlobal(Address.Ptr32(0x01234), "pi", PrimitiveType_v1.Real32())
                        }
                    }
                }
            };
            var sw = new StringWriter();
            When_SaveToTextWriter(program, sw);
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v5"">
  <input>
    <user>
      <global>
        <Address>00001234</Address>
        <prim domain=""Real"" size=""4"" />
        <Name>pi</Name>
      </global>
      <registerValues />
    </user>
  </input>
</project>";
            if (sw.ToString() != sExp)
                Debug.Print("{0}", sw.ToString());
            Assert.AreEqual(sExp, sw.ToString());
        }
    }
}
