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
using NUnit.Framework;
using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Environments.AmigaOS;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.AmigaOS
{
    [TestFixture]
    public class AmigaOSPlatformTests
    {
        private M68kArchitecture arch;
        private Mock<IFileSystemService> fsSvc;
        private Mock<ITypeLibraryLoaderService> tllSvc;
        private ServiceContainer services;
        private RtlEmitter m;
        private AmigaOSPlatform platform;
        private List<RtlInstruction> rtls;
        private IStorageBinder binder;

        [SetUp]
        public void Setup()
        {
            this.fsSvc = new Mock<IFileSystemService>();
            this.tllSvc = new Mock<ITypeLibraryLoaderService>();
            this.services = new ServiceContainer();
            var cfgSvc = new Mock<IConfigurationService>();
            var env = new Mock<PlatformDefinition>();
            this.arch = new M68kArchitecture("m68k");
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            cfgSvc.Setup(c => c.GetEnvironment("amigaOS")).Returns(env.Object);
            env.Setup(e => e.TypeLibraries).Returns(new List<TypeLibraryDefinition>());
            env.Setup(e => e.CharacteristicsLibraries).Returns(new List<TypeLibraryDefinition>());
            env.Setup(e => e.Options).Returns(new Dictionary<string, object>
            {
                { "versionDependentLibraries", new Dictionary<string,object>
                    {
                        { "33", new List<object> { "exec_v33", "dos_v33" } },
                        { "34", new List<object> { "exec_v34", "dos_v34" } },
                    }
                }
            });
            this.services.AddService(typeof(IConfigurationService), cfgSvc.Object);
            this.services.AddService(typeof(IFileSystemService), fsSvc.Object);
            this.services.AddService(typeof(ITypeLibraryLoaderService), tllSvc.Object);
            this.binder = new Frame(arch.FramePointerType);
        }

        [Test]
        public void AOS_LookupA6Call()
        {
            Given_Func("#1 512 0x200 Allocate( heap/a1, size/d0)\n");

            When_Create_Platform();
            m.Call(m.IAdd(binder.EnsureRegister(Registers.a6), -512), 4);
            var state = arch.CreateProcessorState();
            var svc = platform.FindService(rtls.Last(), state);

            Assert.AreEqual("Allocate", svc.Name);
            Assert.AreEqual(2, svc.Signature.Parameters.Length);
            Assert.AreEqual("a1", svc.Signature.Parameters[0].Storage.ToString());
            Assert.AreEqual("d0", svc.Signature.Parameters[1].Storage.ToString());
        }

        [Test]
        public void AOS_LibrarySelection()
        {
            When_Create_Platform();
            var libs_v0 = platform.GetLibrarySetForKickstartVersion(0);
            var libs_v33 = platform.GetLibrarySetForKickstartVersion(33);
            var libs_v34 = platform.GetLibrarySetForKickstartVersion(34);
            var libs_v999 = platform.GetLibrarySetForKickstartVersion(999);

            Assert.AreEqual(0, libs_v0.Count);
            Assert.IsTrue(libs_v33.Contains("exec_v33"));
            Assert.IsTrue(libs_v34.Contains("exec_v34"));
            Assert.IsTrue(libs_v999.Contains("exec_v34")); //TODO: should select version from highest available kickstart version
        }

        private void Given_Func(string fileContents)
        {
            var stm = new MemoryStream(Encoding.ASCII.GetBytes(fileContents));
            fsSvc.Setup(f => f.CreateFileStream("exec.funcs", FileMode.Open, FileAccess.Read))
                .Returns(stm);
            tllSvc.Setup(t => t.InstalledFileLocation(It.IsAny<string>())).Returns("exec.funcs");
        }

        private void When_Create_Platform()
        {
            platform = new AmigaOSPlatform(services, arch);
        }
    }
}
