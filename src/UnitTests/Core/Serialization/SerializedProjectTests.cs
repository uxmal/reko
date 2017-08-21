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
using Reko.Core.Configuration;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System;
using Reko.Core.Expressions;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
	public class SerializedProjectTests
	{
        private MockRepository mr;
        private MockFactory mockFactory;
        private ServiceContainer sc;
        private ILoader loader;
        private IProcessorArchitecture arch;
        private IPlatform platform;
        private IConfigurationService cfgSvc;
        private DecompilerEventListener listener;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.mockFactory = new MockFactory(mr);
            this.cfgSvc = mr.Stub<IConfigurationService>();
            this.listener = mr.Stub<DecompilerEventListener>();
            this.sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl('/'));
            sc.AddService<IConfigurationService>(cfgSvc);
        }

        private void Given_Architecture()
        {
            this.arch = mr.StrictMock<IProcessorArchitecture>();
            this.arch.Stub(a => a.Name).Return("testArch");
            this.arch.Stub(a => a.SaveUserOptions()).Return(null);
            this.cfgSvc.Stub(c => c.GetArchitecture("testArch")).Return(arch);
        }

        private void Given_TestOS_Platform()
        {
            Debug.Assert(arch != null, "Must call Given_Architecture first.");
            // A very simple dumb platform with no intelligent behaviour.
            this.platform = mr.Stub<IPlatform>();
            var oe = mr.Stub<OperatingEnvironment>();
            this.platform.Stub(p => p.Name).Return("testOS");
            this.platform.Stub(p => p.SaveUserOptions()).Return(null);
            this.platform.Stub(p => p.Architecture).Return(arch);
            this.platform.Stub(p => p.CreateMetadata()).Return(new TypeLibrary());
            this.platform.Stub(p => p.DefaultCallingConvention).Return("");
            this.cfgSvc.Stub(c => c.GetEnvironment("testOS")).Return(oe);
            oe.Stub(e => e.Load(sc, arch)).Return(platform);
        }

        private void Expect_TryParseAddress(string sAddr, Address addr)
        {
            platform.Stub(p => p.TryParseAddress(
                Arg<string>.Is.Equal(sAddr),
                out Arg<Address>.Out(addr).Dummy)).Return(true);
        }

        [Test]
        public void SudWrite()
        {
            Project_v4 ud = new Project_v4
            {
                Inputs =
                {
                    new DecompilerInput_v4
                    {
                        DisassemblyFilename = "foo.asm",
                        IntermediateFilename = "foo.cod",
                        User = new UserData_v4 {
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
                XmlSerializer ser = SerializedLibrary.CreateSerializer_v4(typeof(Project_v4));
				ser.Serialize(writer, ud);
				fut.AssertFilesEqual();
			}
		}

        [Test]
        public void SudSaveProject()
        {
            Given_Architecture();
            Given_TestOS_Platform();
            var eax = new RegisterStorage("eax", 0, 0, PrimitiveType.Word32);
            var ecx = new RegisterStorage("ecx", 1, 0, PrimitiveType.Word32);
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
                        Architecture = arch,
                        Platform = platform,
                        SegmentMap = new SegmentMap(Address.SegPtr(0x1000, 0)), //, new byte[100]),
                        DisassemblyFilename = "foo.asm",
                        IntermediateFilename = "foo.cod",
                        User = new UserData
                        {
                            Loader = "CustomLoader",
                            Procedures =
                            {
                                {
                                    Address.SegPtr(0x1000, 0x10),
                                    new Procedure_v1
                                    {
                                        Name = "foo",
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
                                  new GlobalDataItem_v2 {
                                       Address = Address.SegPtr(0x2000, 0).ToString(),
                                       DataType = new StringType_v2 {
                                           Termination=StringType_v2.ZeroTermination,
                                           CharType = new PrimitiveType_v1 { Domain = Domain.Character, ByteSize = 1 }
                                       }
                                  }
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
                                        new UserRegisterValue { Register = eax, Value=Constant.Word32(0x01231) },
                                        new UserRegisterValue { Register = ecx, Value=Constant.Word32(0x42424711) },
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
                                        IndexRegister = new RegisterStorage("R1", 1, 0, PrimitiveType.Word32)
                                   }
                                }
                            },
                            JumpTables =
                            {
                                { jumpTable.Address, jumpTable }
                            }
                        }
                    }
                }
            };
            mr.ReplayAll();

            using (FileUnitTester fut = new FileUnitTester("Core/SudSaveProject.txt"))
            {
                FilteringXmlWriter writer = new FilteringXmlWriter(fut.TextWriter);
                writer.Formatting = System.Xml.Formatting.Indented;
                XmlSerializer ser = SerializedLibrary.CreateSerializer_v4(typeof(Project_v4));
                Project_v4 ud = new ProjectSaver(sc).Serialize("/var/foo/foo.proj", project);
                ser.Serialize(writer, ud);
                fut.AssertFilesEqual();
            }
        }

        [Test]
        public void Sud_LoadProject_Inputs_v2()
        {
            Given_Loader();
            Given_Architecture();
            Expect_Arch_ParseAddress("1000:0400", Address.SegPtr(0x1000, 0x0400));
            Given_ExecutableProgram("foo.exe", Address.SegPtr(0x1000, 0x0000));
            Given_BinaryFile("foo.bin", Address.SegPtr(0x1000, 0x0000));


            var sProject = new Project_v2
            {
                Inputs = {
                    new DecompilerInput_v2 {
                        Filename = "foo.exe",
                        Address = "1000:0000",
                        Comment = "main file",
                        UserGlobalData = new List<GlobalDataItem_v2>
                        {
                            new GlobalDataItem_v2 { Address = "1000:0400", DataType = new StringType_v2 { 
                                Termination=StringType_v2.ZeroTermination,
                                CharType= new PrimitiveType_v1 { ByteSize = 1, Domain=Domain.Character} } 
                            }
                        }
                    },
                    new DecompilerInput_v2 {
                        Filename = "foo.bin",
                        Address = "1000:D000",
                        Comment = "overlay",
                        Processor = "x86-real-16",
                    }
                }
            };
            mr.ReplayAll();

            var ps = new ProjectLoader(sc, loader, listener);
            var project = ps.LoadProject("project.dcproj", sProject);
            Assert.AreEqual(2, project.Programs.Count);
            var input0 = project.Programs[0];
            Assert.AreEqual(1, input0.User.Globals.Count);
            Assert.AreEqual("1000:0400", input0.User.Globals.Values[0].Address);
            var str_t = (StringType_v2)input0.User.Globals.Values[0].DataType;
            Assert.AreEqual("prim(Character,1)", str_t.CharType.ToString());
            mr.VerifyAll();
        }

        private void Given_BinaryFile(string exeName, Address address)
        {
            var bytes = new byte[0x10];
            var program = new Program
            {
                Architecture = arch,
                SegmentMap = new SegmentMap(
                    address,
                    new ImageSegment(
                        ".text", 
                        new MemoryArea(address, bytes),
                        AccessMode.ReadWriteExecute))
            };
            loader.Stub(l => l.LoadImageBytes(
                Arg<string>.Matches(s => s.EndsWith(exeName)),
                Arg<int>.Is.Anything)).Return(bytes);
            loader.Stub(l => l.LoadExecutable(
                Arg<string>.Matches(s => s.EndsWith(exeName)),
                Arg<byte[]>.Is.NotNull,
                Arg<string>.Is.Null,
                Arg<Address>.Is.Null)).Return(program);
        }

        private void Given_ExecutableProgram(string exeName, Address address)
        {
            var bytes = new byte[0x1000];
            var mem = new MemoryArea(address, bytes);
            var segmentMap = new SegmentMap(address,
                    new ImageSegment(".text", mem, AccessMode.ReadWriteExecute));
            var program = new Program
            {
                Architecture = arch,
                Platform = mockFactory.CreatePlatform(),
                SegmentMap = segmentMap,
                ImageMap = segmentMap.CreateImageMap()
            };
            loader.Stub(l => l.LoadImageBytes(
                Arg<string>.Matches(s => s.EndsWith(exeName)),
                Arg<int>.Is.Anything)).Return(bytes);
            loader.Stub(l => l.LoadExecutable(
                Arg<string>.Matches(s => s.EndsWith(exeName)),
                Arg<byte[]>.Is.NotNull,
                Arg<string>.Is.Null,
                Arg<Address>.Is.Null)).Return(program);
        }

        private void Expect_Arch_ParseAddress(string sExp, Address result)
        {
            arch.Stub(a => a.TryParseAddress(
                Arg<string>.Is.Equal("1000:0400"),
                out Arg<Address>.Out(Address.SegPtr(0x1000, 0x0400)).Dummy)).Return(true);
        }

        private void Given_Loader()
        {
            this.loader = mr.Stub<ILoader>();
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
                        Architecture = arch,
                        Platform = platform,
                        Filename = "c:\\test\\foo.exe",
                    }
                },
                MetadataFiles =
                {
                    new MetadataFile
                    {
                        Filename = "c:\\test\\foo.def",
                        ModuleName = "foo.def",
                    }
                }
            };
            mr.ReplayAll();

            var ps = new ProjectSaver(sc);
            ps.Serialize("c:\\test\\foo.project", project);
            Assert.AreEqual(1, project.MetadataFiles.Count);
            Assert.AreEqual("c:\\test\\foo.def", project.MetadataFiles[0].Filename);
            Assert.AreEqual("foo.def", project.MetadataFiles[0].ModuleName);
        }

        [Test]
        public void SudLoadMetadata()
        {
            var sProject = new Project_v2 {
                Inputs =
                {
                    new MetadataFile_v2
                    {
                        Filename = "c:\\tmp\\foo.def"
                    }
                }
            };
            var loader = mr.Stub<ILoader>();
            var typelib = new TypeLibrary();
            loader.Stub(l => l.LoadMetadata("", null, null)).IgnoreArguments().Return(typelib);
            var oracle = mr.Stub<IOracleService>();
            oracle.Stub(o => o.QueryPlatform(Arg<string>.Is.NotNull)).Return(mockFactory.CreatePlatform());
            sc.AddService<IOracleService>(oracle);
            mr.ReplayAll();

            var ploader = new ProjectLoader(sc, loader, listener);
            var project = ploader.LoadProject("c:\\bar\\bar.dcproj", sProject);
            Assert.AreEqual(1, project.MetadataFiles.Count);
            Assert.AreEqual("c:\\tmp\\foo.def", project.MetadataFiles[0].Filename);
        }

        [Test]
        public void SudLoadProgramOptions()
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
                            Heuristics = { new Heuristic_v3 { Name="HeuristicScanning" } },
                            TextEncoding = "windows-1251",
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
            arch.Stub(a => a.GetRegister("eax")).Return(new RegisterStorage("eax", 0, 0, PrimitiveType.Word32));
            arch.Stub(a => a.GetRegister("ecx")).Return(new RegisterStorage("ecx", 1, 0, PrimitiveType.Word32));
            var loader = mr.Stub<ILoader>();
            loader.Stub(l => l.LoadImageBytes(null, 0))
                .IgnoreArguments()
                .Return(new byte[10]);
            loader.Stub(l => l.LoadExecutable(null, null, null, null))
                .IgnoreArguments()
                .Return(new Program
                {
                    Platform = this.platform,
                });
            mr.ReplayAll();

            var ploader = new ProjectLoader(sc, loader, listener);
            var project = ploader.LoadProject("c:\\tmp\\foo\\bar.proj", sProject);
            Assert.IsTrue(project.Programs[0].User.Heuristics.Contains("HeuristicScanning"));
            Assert.AreEqual("windows-1251", project.Programs[0].User.TextEncoding.WebName);
            Assert.AreEqual(1, project.Programs[0].User.RegisterValues.Count);
            Assert.AreEqual(2, project.Programs[0].User.RegisterValues[Address.Ptr32(0x00443210)].Count);
        }

        [Test]
        public void SudSaveProgramOptions()
        {
            var program = new Program();
            program.User.Heuristics.Add("shingle");
            program.User.TextEncoding = Encoding.GetEncoding("windows-1251");
        
            var pSaver = new ProjectSaver(sc);
            var file = pSaver.VisitProgram("foo.proj", program);
            var ip = (DecompilerInput_v4)file;
            Assert.IsTrue(ip.User.Heuristics.Any(h => h.Name == "shingle"));
            Assert.AreEqual("windows-1251", ip.User.TextEncoding);
        }

        private void When_SaveToTextWriter(Program program, TextWriter sw)
        {
            var saver = new ProjectSaver(sc);
            var sProj = new Project_v4
            {
                Inputs = { saver.VisitProgram("foo.exe", program) }
            };
            var writer = new FilteringXmlWriter(sw);
            writer.Formatting = System.Xml.Formatting.Indented;
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v4(typeof(Project_v4));
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
<project xmlns=""http://schemata.jklnet.org/Reko/v4"">
  <input>
    <user>
      <platform>
        <item key=""Name"">Bob</item>
        <item key=""Name2"">Sue</item>
      </platform>
      <registerValues />
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
<project xmlns=""http://schemata.jklnet.org/Reko/v4"">
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
<project xmlns=""http://schemata.jklnet.org/Reko/v4"">
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
    </user>
  </input>
</project>";
            if (sw.ToString() != sExp)
                Debug.Print("{0}", sw.ToString());
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
                    Globals =
                    {
                        {
                            Address.Ptr32(0x01234),
                            new GlobalDataItem_v2
                            {
                                 DataType = PrimitiveType_v1.Real32(),
                                 Name = "pi"
                            }
                        }
                    }
                }
            };
            var sw = new StringWriter();
            When_SaveToTextWriter(program, sw);
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v4"">
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
