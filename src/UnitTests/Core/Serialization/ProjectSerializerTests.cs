#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Loading;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class ProjectSerializerTests
    {
        private MockRepository mr;
        private ServiceContainer sc;
        private ILoader loader;
        private IProcessorArchitecture arch;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.sc = new ServiceContainer();
            loader = mr.Stub<ILoader>();
            arch = mr.StrictMock<IProcessorArchitecture>();
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
        }

        [Test]
        public void Ps_Load_v3()
        {
            var bytes = new byte[100];
            loader.Stub(l => l.LoadImageBytes(null, 0)).IgnoreArguments().Return(bytes);
            loader.Stub(l => l.LoadExecutable(null, null, null)).IgnoreArguments().Return(
                new Program { Architecture = arch });

            mr.ReplayAll();

            var sp = new Project_v3
            {
                Inputs = {
                    new DecompilerInput_v3
                    {
                        Filename = "f.exe",
                        User = new UserData_v3
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
                            }
                        }
                    }
                }
            };
            var ps = new ProjectLoader(sc, loader);
            var p = ps.LoadProject("c:\\tmp\\fproj.proj", sp);
            Assert.AreEqual(1, p.Programs.Count);
            var inputFile = p.Programs[0]; 
            Assert.AreEqual(1, inputFile.User.Procedures.Count);
            Assert.AreEqual("Fn", inputFile.User.Procedures.First().Value.Name);
        }

        [Test]
        public void Save_v3()
        {
            var sp = new Project_v3
            {
                Inputs = {
                    new DecompilerInput_v3 {
                        Filename ="foo.exe",
                        User = new UserData_v3 {
                            Heuristics = {
                                new Heuristic_v3 { Name = "shingle" }
                            }
                        }
                    },
                    new AssemblerFile_v3 { Filename="foo.asm", Assembler="x86-att" }
                }
            };
            var sw = new StringWriter();
            new ProjectSaver(sc).Save(sp, sw);
            Console.WriteLine(sw);
        }
    }
}
