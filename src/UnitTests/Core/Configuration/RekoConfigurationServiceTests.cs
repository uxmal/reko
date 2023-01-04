#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Services;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Configuration
{
    [TestFixture]
    public class RekoConfigurationServiceTests
    {
        private ServiceContainer sc;
        private FakeDecompilerEventListener listener;
        private Mock<IPluginLoaderService> pluginSvc;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            this.listener = new FakeDecompilerEventListener();
            this.pluginSvc = new Mock<IPluginLoaderService>();
            sc.AddService<DecompilerEventListener>(listener);
            sc.AddService<IPluginLoaderService>(pluginSvc.Object);
        }

        [Test]
        public void Rcfg_LoadOperatingEnvironment()
        {
            var sc = new ServiceContainer();
            var cfgSvc = new RekoConfigurationService(sc, "reko.config", new RekoConfiguration_v1
            {
                Environments = new []
                {
                    new Environment_v1
                    {
                        Name = "testOS",
                        Description = "Test OS",
                        Heuristics = new PlatformHeuristics_v1
                        {
                            ProcedurePrologs = new []
                            {
                                new BytePattern_v1
                                {
                                    Bytes = "55 8B EC",
                                    Mask = "FF FF FF",
                                },
                                new BytePattern_v1
                                {
                                    Bytes= "55 ?? 30"
                                }
                            }
                        },
                        Architectures = new[]
                        {
                            new PlatformArchitecture_v1
                            {
                                Name="testCPU",
                                TypeLibraries = new[]
                                {
                                    new TypeLibraryReference_v1
                                    {
                                        Name = "lp32.xml",
                                    }
                                },
                                ProcedurePrologs = new[]
                                {
                                    new BytePattern_v1
                                    {
                                        Bytes = "55 8B EC"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            var env = cfgSvc.GetEnvironment("testOS");
            Assert.AreEqual(2, env.Heuristics.ProcedurePrologs.Length);
            var pattern0 = env.Heuristics.ProcedurePrologs[0];
            Assert.AreEqual("55 8B EC", pattern0.Bytes);
            Assert.AreEqual("FF FF FF", pattern0.Mask);

            var pattern1 = env.Heuristics.ProcedurePrologs[1];
            Assert.AreEqual("55 ?? 30", pattern1.Bytes);
            Assert.IsNull(pattern1.Mask);

            var archs = env.Architectures;
            Assert.AreEqual(1, archs.Count);
            Assert.AreEqual("lp32.xml", archs[0].TypeLibraries[0].Name);
            Assert.AreEqual(0xEC, archs[0].ProcedurePrologs[0].Bytes[2]);
        }

        [Test]
        public void Rcfg_LoadLoader()
        {
            var cfgSvc = new RekoConfigurationService(sc, "reko.config", new RekoConfiguration_v1
            {
                Loaders = new[]
                {
                    new RekoLoader
                    {
                        MagicNumber = "425325",
                        Offset = "0x4242",
                        Extension = ".foo",
                        Argument = "fnord",
                        Label = "lay-bul",
                        Type= "foo.Loader,foo",
                    },
                    new RekoLoader
                    {
                        MagicNumber = "444444",
                        Offset = "123",
                        Extension = ".bar",
                        Argument = "bok",
                        Label = "bullet",
                        Type= "bar.Loader,bar",
                    }
                }
            });

            var ldrs = cfgSvc.GetImageLoaders().ToArray();

            Assert.AreEqual("425325", ldrs[0].MagicNumber);
            Assert.AreEqual(0x4242, ldrs[0].Offset);
            Assert.AreEqual(".foo", ldrs[0].Extension);
            Assert.AreEqual("fnord", ldrs[0].Argument);
            Assert.AreEqual("lay-bul", ldrs[0].Label);
            Assert.AreEqual("foo.Loader,foo", ldrs[0].TypeName);

            Assert.AreEqual("444444", ldrs[1].MagicNumber);
            Assert.AreEqual(123, ldrs[1].Offset);
            Assert.AreEqual(".bar", ldrs[1].Extension);
            Assert.AreEqual("bok", ldrs[1].Argument);
            Assert.AreEqual("bullet", ldrs[1].Label);
            Assert.AreEqual("bar.Loader,bar", ldrs[1].TypeName);
        }

        [Test]
        public void Rcfg_LoadArchitectureModel()
        {
            var cfgSvc = new RekoConfigurationService(sc, "reko.config", new RekoConfiguration_v1
            {
                Architectures = new[]{
                    new Architecture_v1
                    {
                        Name = "fake",
                        Type = typeof(FakeArchitecture).AssemblyQualifiedName,
                        Models = new[]
                        {
                            new ModelDefinition_v1
                            {
                                Name = "Fake-2000",
                                Options = new[]
                                {
                                    new ListOption_v1 { Text=ProcessorOption.WordSize, Value="32" }
                                }
                            }
                        }
                    }
                }
            });
            pluginSvc.Setup(p => p.GetType(It.IsAny<string>()))
                .Returns(typeof(FakeArchitecture));

            var arch = cfgSvc.GetArchitecture("fake", "Fake-2000");
            var options = arch.SaveUserOptions();
            Assert.AreEqual("Fake-2000", options[ProcessorOption.Model]);
            Assert.AreEqual("32", options[ProcessorOption.WordSize]);
        }

        [Test]
        public void Rcfg_LoadArchitecture_UnknownModel()
        {
            var cfgSvc = new RekoConfigurationService(sc, "reko.config", new RekoConfiguration_v1
            {
                Architectures = new[]{
                    new Architecture_v1
                    {
                        Name = "fake",
                        Type = typeof(FakeArchitecture).AssemblyQualifiedName,
                        Models = new[]
                        {
                            new ModelDefinition_v1
                            {
                                Name = "Fake-2000",
                                Options = new[]
                                {
                                    new ListOption_v1 { Text=ProcessorOption.WordSize, Value="32" }
                                }
                            }
                        }
                    }
                }
            });
            pluginSvc.Setup(p => p.GetType(It.IsAny<string>()))
                .Returns(typeof(FakeArchitecture));

            var arch = cfgSvc.GetArchitecture("fake", "Unknown Model");
            Assert.AreEqual("WarningDiagnostic -  - Model 'Unknown Model' is not defined for architecture 'fake'.", listener.LastDiagnostic);
        }
    }
}
