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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Rhino.Mocks;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class ProjectSerializerTests
    {
        private MockRepository mr;
        private ServiceContainer sc;
        private ILoader loader;
        private IProcessorArchitecture arch;
        private IConfigurationService cfgSvc;
        private IPlatform platform;
        private DecompilerEventListener listener;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.sc = new ServiceContainer();
            loader = mr.Stub<ILoader>();
            arch = mr.StrictMock<IProcessorArchitecture>();
            this.listener = mr.Stub<DecompilerEventListener>();
            Address dummy;
            arch.Stub(a => a.TryParseAddress(null, out dummy)).IgnoreArguments().WhenCalled(m =>
            {
                Address addr;
                var sAddr = (string)m.Arguments[0];
                var iColon = sAddr.IndexOf(':');
                if (iColon > 0)
                {
                    addr = Address.SegPtr(
                        Convert.ToUInt16(sAddr.Remove(iColon)),
                        Convert.ToUInt16(sAddr.Substring(iColon+1)));
                    m.ReturnValue = true;
                }
                else
                {
                    m.ReturnValue = Address32.TryParse32((string)m.Arguments[0], out addr);
                }
                m.Arguments[1] = addr;
            }).Return(false);
            this.cfgSvc = mr.Stub<IConfigurationService>();
            this.sc.AddService<IConfigurationService>(cfgSvc);
        }

        private void Given_Architecture()
        {
            this.arch.Stub(a => a.Name).Return("testArch");
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
            this.cfgSvc.Stub(c => c.GetEnvironment("testOS")).Return(oe);
            oe.Stub(e => e.Load(sc, arch)).Return(platform);
        }

        private void Given_Platform_Address(string sAddr, uint uAddr)
        {
            this.platform.Stub(p => p.TryParseAddress(
                Arg<string>.Is.Equal(sAddr),
                out Arg<Address>.Out(Address.Ptr32(uAddr)).Dummy)).Return(true);
        }

        [Test]
        public void Ps_Load_v4()
        {
            var bytes = new byte[100];
            loader.Stub(l => l.LoadImageBytes(null, 0)).IgnoreArguments().Return(bytes);
            loader.Stub(l => l.LoadExecutable(null, null, null, null)).IgnoreArguments().Return(
                new Program { Architecture = arch });
            Given_Architecture();
            Given_TestOS_Platform();
            Given_Platform_Address("113800", 0x113800);
            Given_Platform_Address("114000", 0x114000);
            Given_Platform_Address("115000", 0x115000);
            Given_Platform_Address("115012", 0x115012);
            Given_Platform_Address("11504F", 0x11504F);
            arch.Stub(a => a.GetRegister("r1")).Return(new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));
            mr.ReplayAll();

            var sp = new Project_v4
            {
                ArchitectureName = "testArch",
                PlatformName = "testOS",
                Inputs = {
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
            var ps = new ProjectLoader(sc, loader, listener);
            var p = ps.LoadProject("c:\\tmp\\fproj.proj", sp);
            Assert.AreEqual(1, p.Programs.Count);
            var inputFile = p.Programs[0]; 
            Assert.AreEqual(1, inputFile.User.Procedures.Count);
            Assert.AreEqual("Fn", inputFile.User.Procedures.First().Value.Name);

            Assert.AreEqual(1, inputFile.User.JumpTables.Count);
            var jumpTable = inputFile.User.JumpTables[Address.Ptr32(0x114000)];
            Assert.AreEqual(Address.Ptr32(0x00115000), jumpTable.Addresses[0]);
            Assert.AreEqual(Address.Ptr32(0x00115012), jumpTable.Addresses[1]);
            Assert.AreEqual(Address.Ptr32(0x0011504F), jumpTable.Addresses[2]);

            Assert.AreEqual(1, inputFile.User.IndirectJumps.Count);
            var indJump = inputFile.User.IndirectJumps[Address.Ptr32(0x00113800)];
            Assert.AreSame(jumpTable, indJump.Table);

        }

        [Test]
        public void Save_v4()
        {
            var sp = new Project_v4
            {
                Inputs = {
                    new DecompilerInput_v4 {
                        Filename ="foo.exe",
                        User = new UserData_v4 {
                            Heuristics = {
                                new Heuristic_v3 { Name = "shingle" }
                            }
                        }
                    },
                    new AssemblerFile_v3 { Filename="foo.asm", Assembler="x86-att" }
                }
            };
            var xw = new FilteringXmlWriter(new StringWriter());
            new ProjectSaver(sc).Save(sp, xw);
        }
    }
}
