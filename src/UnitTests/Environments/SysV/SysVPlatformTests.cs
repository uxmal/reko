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

using Moq;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Environments.SysV;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Reko.Core.Types;

namespace Reko.UnitTests.Environments.SysV
{
    [TestFixture]
    public class SysVPlatformTests
    {
        private ServiceContainer sc;
        private Mock<IProcessorArchitecture> arch;
        private Mock<IConfigurationService> cfgSvc;
        private Mock<ITypeLibraryLoaderService> tlSvc;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            this.arch = new Mock<IProcessorArchitecture>();
            this.tlSvc = new Mock<ITypeLibraryLoaderService>();
            this.cfgSvc = new Mock<IConfigurationService>();
            sc.AddService<IConfigurationService>(cfgSvc.Object);
            sc.AddService<ITypeLibraryLoaderService>(tlSvc.Object);

        }

        [Test]
        public void SysV_TerminatingFunction()
        {
            cfgSvc.Setup(c => c.GetInstallationRelativePath("libc.xml"))
                .Returns("libc.xml");

            var env = new PlatformDefinition
            {
                TypeLibraries =
                     {
                         new TypeLibraryDefinition
                         {
                              Name="libc.xml"
                         }
                     },
                CharacteristicsLibraries =
                     {
                         new TypeLibraryDefinition
                         {
                             Name="libcharacteristics.xml",
                         }
                     }
            };

            Given_EnvironmentConfiguration(env);
            tlSvc.Setup(t => t.LoadCharacteristics(It.IsAny<string>()))
                .Returns(new CharacteristicsLibrary
                {
                    Entries =
                    {
                        {
                            "exit",
                            new ProcedureCharacteristics {
                                Terminates = true
                            }
                        }
                    }
                });
            tlSvc.Setup(t => t.LoadMetadataIntoLibrary(
                It.IsAny<IPlatform>(),
                It.IsAny<TypeLibraryDefinition>(),
                It.IsAny<TypeLibrary>()))
                .Returns(new TypeLibrary
                {
                    Signatures =
                    {
                         {
                            "exit",
                            new FunctionType()
                         }
                     }
                });

            var sysv = new SysVPlatform(sc, arch.Object);
            var proc = sysv.LookupProcedureByName(null, "exit");
            Assert.IsTrue(proc.Characteristics.Terminates, "exit should have been marked as terminating.");
        }

        private void Given_EnvironmentConfiguration(PlatformDefinition env)
        {
            cfgSvc.Setup(c => c.GetEnvironment(It.IsAny<string>()))
                .Returns(env);
        }

        [Test]
        public void SysV_LoadTrashedRegisters()
        {
            arch.Setup(a => a.Name).Returns("mmix");
            arch.Setup(a => a.GetRegister(It.IsAny<string>()))
                .Returns((string r) => new RegisterStorage(r, (int)r[1], 0, PrimitiveType.Word32));
            var env = new PlatformDefinition
            {
                Architectures =
                {
                    new PlatformArchitectureDefinition()
                    {
                         Name = "mmix",
                         TrashedRegisters =
                         {
                             "r2", "r3"
                         }
                    }
                }
            };

            Given_EnvironmentConfiguration(env);

            var sysv = new SysVPlatform(sc, arch.Object);
            var regs = sysv.CreateTrashedRegisters();
            Assert.AreEqual(2, regs.Count);
            Assert.AreEqual("r2,r3", string.Join(",", regs.Select(r => r.Name)));
        }
    }
}
