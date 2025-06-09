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
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class ProjectSerializerTests
    {
        private ServiceContainer sc;
        private Mock<ILoader> loader;
        private Mock<IProcessorArchitecture> arch;
        private Mock<IConfigurationService> cfgSvc;
        private Mock<IPlatform> platform;
        private Mock<IEventListener> listener;


        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            loader = new Mock<ILoader>();
            arch = new Mock<IProcessorArchitecture>();
            this.listener = new Mock<IEventListener>();
            Address dummy;
            arch.Setup(a => a.Name).Returns("FakeArch");
            arch.Setup(a => a.TryParseAddress(It.IsAny<string>(), out dummy))
                .Returns(new StringToAddress((string sAddr, out Address addr) => {
                    var iColon = sAddr.IndexOf(':');
                    if (iColon > 0)
                    {
                        addr = Address.SegPtr(
                            Convert.ToUInt16(sAddr.Remove(iColon)),
                            Convert.ToUInt16(sAddr.Substring(iColon + 1)));
                        return true;
                    }
                    else
                    {
                        return Address.TryParse32(sAddr, out addr);
                    }
                }));
            this.cfgSvc = new Mock<IConfigurationService>();
            this.sc.AddService<IConfigurationService>(cfgSvc.Object);
        }

        private void Given_Architecture()
        {
            this.arch.Setup(a => a.Name).Returns("testArch");
            this.cfgSvc.Setup(c => c.GetArchitecture("testArch")).Returns(arch.Object);
        }

        private void Given_TestOS_Platform()
        {

            Debug.Assert(arch is not null, "Must call Given_Architecture first.");
            // A very simple dumb platform with no intelligent behaviour.
            this.platform = new Mock<IPlatform>();
            var oe = new Mock<PlatformDefinition>();
            this.platform.Setup(p => p.Name).Returns("testOS");
            this.platform.Setup(p => p.SaveUserOptions()).Returns((Dictionary<string,object>)null);
            this.platform.Setup(p => p.Architecture).Returns(arch.Object);
            this.platform.Setup(p => p.CreateMetadata()).Returns(new TypeLibrary());
            this.cfgSvc.Setup(c => c.GetEnvironment("testOS")).Returns(oe.Object);
            oe.Setup(e => e.Load(sc, arch.Object)).Returns(platform.Object);
        }

        private void Given_Platform_Address(string sAddr, uint uAddr)
        {
            var addr = Address.Ptr32(uAddr);
            this.platform.Setup(p => p.TryParseAddress(
                sAddr,
                out addr)).Returns(true);
        }

        [Test]
        public void Ps_Load_v4()
        {
            var bytes = new byte[100];
            loader.Setup(l => l.LoadFileBytes(
                It.IsAny<string>())).
                Returns(bytes);
            loader.Setup(l => l.Load(
                It.IsAny<ImageLocation>(),
                null,
                null)).Returns(new Func<ImageLocation, string, Address, ILoadedImage>(
                    (i, l, a) => new Program { Architecture = arch.Object }));
            Given_Architecture();
            Given_TestOS_Platform();
            Given_Platform_Address("113800", 0x113800);
            Given_Platform_Address("114000", 0x114000);
            Given_Platform_Address("115000", 0x115000);
            Given_Platform_Address("115012", 0x115012);
            Given_Platform_Address("11504F", 0x11504F);
            arch.Setup(a => a.GetRegister("r1")).Returns(RegisterStorage.Reg32("r1", 1));

            var sp = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                InputFiles = {
                    new DecompilerInput_v4
                    {
                        Filename = "f.exe",
                        User = new UserData_v4
                        {
                            Procedures =
                            {
                                new Procedure_v1 {
                                    Name = "Fn",
                                    Decompile = true,
                                    Characteristics = new ProcedureCharacteristics
                                    {
                                        Terminates = true,
                                    },
                                    Address = "113300",
                                    Signature = new SerializedSignature {
                                        ReturnValue = new Argument_v1 {
                                            Type = new PrimitiveType_v1(Domain.SignedInt, 4),
                                        },
                                        Arguments = new Argument_v1[] {
                                            new Argument_v1
                                            {
                                                Name = "a",
                                                Kind = new StackVariable_v1(),
                                                Type = new PrimitiveType_v1(Domain.Character, 2)
                                            },
                                            new Argument_v1
                                            {
                                                Name = "b",
                                                Kind = new StackVariable_v1(),
                                                Type = new PointerType_v1 { DataType = new PrimitiveType_v1(Domain.Character, 2) }
                                            }
                                        }
                                    }
                                }
                            },
                            IndirectJumps =
                            {
                                new IndirectJump_v4
                                {
                                    InstructionAddress = "113800",
                                    IndexRegister = "r1",
                                    TableAddress = "114000",
                                }
                            },
                            JumpTables =
                            {
                                new JumpTable_v4
                                {
                                    TableAddress = "114000",
                                    Destinations = new []
                                    {
                                        "115000",
                                        "115012",
                                        "11504F",
                                    }
                                }
                            }
                        }
                    }
                }
            };
            var location = ImageLocation.FromUri("c:\\tmp\\fproj.proj");
            var ps = new ProjectLoader(sc, loader.Object, location, listener.Object);
            var p = ps.LoadProject(sp);

            Assert.AreEqual(1, p.Programs.Count);
            var inputFile0 = p.Programs[0]; 
            Assert.AreEqual(1, inputFile0.User.Procedures.Count);
            Assert.AreEqual("Fn", inputFile0.User.Procedures.First().Value.Name);

            Assert.AreEqual(1, inputFile0.User.JumpTables.Count);
            var jumpTable = inputFile0.User.JumpTables[Address.Ptr32(0x114000)];
            Assert.AreEqual(Address.Ptr32(0x00115000), jumpTable.Addresses[0]);
            Assert.AreEqual(Address.Ptr32(0x00115012), jumpTable.Addresses[1]);
            Assert.AreEqual(Address.Ptr32(0x0011504F), jumpTable.Addresses[2]);

            Assert.AreEqual(1, inputFile0.User.IndirectJumps.Count);
            var indJump = inputFile0.User.IndirectJumps[Address.Ptr32(0x00113800)];
            Assert.AreSame(jumpTable, indJump.Table);
        }

        [Test]
        public void Ps_Load_v5()
        {
            var bytes = new byte[100];
            loader.Setup(l => l.LoadFileBytes(
                It.IsAny<string>())).
                Returns(bytes);
            loader.Setup(l => l.Load(
                It.IsAny<ImageLocation>(),
                null,
                null)).Returns(new Func<ImageLocation, string, Address, ILoadedImage>(
                    (i, l, a) => new Program { Architecture = arch.Object }));
            Given_Architecture();
            Given_TestOS_Platform();
            Given_Platform_Address("113800", 0x113800);
            Given_Platform_Address("114000", 0x114000);
            Given_Platform_Address("115000", 0x115000);
            Given_Platform_Address("115012", 0x115012);
            Given_Platform_Address("11504F", 0x11504F);
            arch.Setup(a => a.GetRegister("r1")).Returns(RegisterStorage.Reg32("r1", 1));

            var sp = new Project_v5
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                InputFiles = {
                    new DecompilerInput_v5
                    {
                        Filename = "f.exe",
                        User = new UserData_v4
                        {
                            Procedures =
                            {
                                new Procedure_v1 {
                                    Name = "Fn",
                                    Decompile = true,
                                    Characteristics = new ProcedureCharacteristics
                                    {
                                        Terminates = true,
                                    },
                                    Address = "113300",
                                    Signature = new SerializedSignature {
                                        ReturnValue = new Argument_v1 {
                                            Type = new PrimitiveType_v1(Domain.SignedInt, 4),
                                        },
                                        Arguments = new Argument_v1[] {
                                            new Argument_v1
                                            {
                                                Name = "a",
                                                Kind = new StackVariable_v1(),
                                                Type = new PrimitiveType_v1(Domain.Character, 2)
                                            },
                                            new Argument_v1
                                            {
                                                Name = "b",
                                                Kind = new StackVariable_v1(),
                                                Type = new PointerType_v1 { DataType = new PrimitiveType_v1(Domain.Character, 2) }
                                            }
                                        }
                                    }
                                }
                            },
                            IndirectJumps =
                            {
                                new IndirectJump_v4
                                {
                                    InstructionAddress = "113800",
                                    IndexRegister = "r1",
                                    TableAddress = "114000",
                                }
                            },
                            JumpTables =
                            {
                                new JumpTable_v4
                                {
                                    TableAddress = "114000",
                                    Destinations = new []
                                    {
                                        "115000",
                                        "115012",
                                        "11504F",
                                    }
                                }
                            },
                            OutputFilePolicy = Program.SegmentFilePolicy,
                            BlockLabels =
                            {
                                new BlockLabel_v1
                                {
                                    Location = "115252",
                                    Name = "errorExit",
                                }
                            },
                            AggressiveBranchRemoval = true,
                            Heuristics =
                            {
                                new Heuristic_v3 { Name = "shingle" }
                            },
                            RenderInstructionsCanonically = true,
                        },
                    },
                    new DecompilerInput_v5
                    {
                        Location = OsPath.Relative("foo", "i am positive+.exe")
                    }
                }
            };
            var location = ImageLocation.FromUri(OsPath.Absolute("tmp", "fproj.proj"));
            var ps = new ProjectLoader(sc, loader.Object, location, listener.Object);
            var p = ps.LoadProject(sp);

            Assert.AreEqual(2, p.Programs.Count);
            var inputFile0 = p.Programs[0];
            var inputFile1 = p.Programs[1];
            Assert.AreEqual(1, inputFile0.User.Procedures.Count);
            Assert.AreEqual("Fn", inputFile0.User.Procedures.First().Value.Name);

            Assert.AreEqual(1, inputFile0.User.JumpTables.Count);
            var jumpTable = inputFile0.User.JumpTables[Address.Ptr32(0x114000)];
            Assert.AreEqual(Address.Ptr32(0x00115000), jumpTable.Addresses[0]);
            Assert.AreEqual(Address.Ptr32(0x00115012), jumpTable.Addresses[1]);
            Assert.AreEqual(Address.Ptr32(0x0011504F), jumpTable.Addresses[2]);

            Assert.AreEqual(1, inputFile0.User.IndirectJumps.Count);
            var indJump = inputFile0.User.IndirectJumps[Address.Ptr32(0x00113800)];
            Assert.AreSame(jumpTable, indJump.Table);

            Assert.AreEqual(1, inputFile0.User.BlockLabels.Count);
            var blockLabel = inputFile0.User.BlockLabels["115252"];
            Assert.AreEqual("errorExit", blockLabel);
            Assert.AreEqual(Program.SegmentFilePolicy, inputFile0.User.OutputFilePolicy);

            Assert.IsTrue(inputFile0.User.Heuristics.Contains("aggressive-branch-removal"));
            Assert.IsTrue(inputFile0.User.Heuristics.Contains("shingle"));
            Assert.IsTrue(inputFile0.User.RenderInstructionsCanonically);

            Assert.AreEqual(OsPath.Absolute("tmp","foo","i am positive+.exe"), inputFile1.Location.FilesystemPath);
        }

        [Test]
        public void Save_v5()
        {
            var sp = new Project_v5
            {
                InputFiles = {
                    new DecompilerInput_v5 {
                        Filename ="foo.exe",
                        User = new UserData_v4 {
                            Heuristics = {
                                new Heuristic_v3 { Name = "shingle" }
                            },
                        }
                    },
                },
                AssemblerFiles = {
                    new AssemblerFile_v3 { Filename="foo.asm", Assembler="x86-att" }
                }
            };
            var xw = new FilteringXmlWriter(new StringWriter());
            new ProjectSaver(sc).Save(sp, xw);
        }
    }
}
