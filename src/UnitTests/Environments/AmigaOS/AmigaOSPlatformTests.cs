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

using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Environments.AmigaOS;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Reko.Core.Configuration;

namespace Reko.UnitTests.Environments.AmigaOS
{
    [TestFixture]
    public class AmigaOSPlatformTests
    {
        private M68kArchitecture arch;
        private MockRepository mr;
        private IFileSystemService fsSvc;
        private ITypeLibraryLoaderService tllSvc;
        private IServiceProvider services;
        private RtlEmitter m;
        private AmigaOSPlatform platform;
        private List<RtlInstruction> rtls;
        private IStorageBinder frame;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.fsSvc = mr.StrictMock<IFileSystemService>();
            this.tllSvc = mr.Stub<ITypeLibraryLoaderService>();
            this.services = mr.StrictMock<IServiceProvider>();
            var cfgSvc = mr.Stub<IConfigurationService>();
            var env = mr.Stub<OperatingEnvironment>();
            this.arch = new M68kArchitecture();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            cfgSvc.Stub(c => c.GetEnvironment("amigaOS")).Return(env);
            env.Stub(e => e.TypeLibraries).Return(new List<ITypeLibraryElement>());
            env.Stub(e => e.CharacteristicsLibraries).Return(new List<ITypeLibraryElement>());
            env.Stub(e => e.Options).Return(new Dictionary<string, object>
            {
                { "versionDependentLibraries", new Dictionary<string,object>
                    {
                        { "33", new List<object> { "exec_v33", "dos_v33" } },
                        { "34", new List<object> { "exec_v34", "dos_v34" } },
                    }
                }
            });
            this.services.Stub(s => s.GetService(typeof(IConfigurationService))).Return(cfgSvc);
            this.services.Stub(s => s.GetService(typeof(IFileSystemService))).Return(fsSvc);
            this.services.Stub(s => s.GetService(typeof(ITypeLibraryLoaderService))).Return(tllSvc);
            this.frame = new Frame(arch.FramePointerType);
        }

        [Test]
        public void AOS_LookupA6Call()
        {
            Given_Func("#1 512 0x200 Allocate( heap/a1, size/d0)\n");
            mr.ReplayAll();

            When_Create_Platform();
            m.Call(m.IAdd(frame.EnsureRegister(Registers.a6), -512), 4);
            var state = arch.CreateProcessorState();
            var svc = platform.FindService(rtls.Last(), state);

            Assert.AreEqual("Allocate", svc.Name);
            Assert.AreEqual(2, svc.Signature.Parameters.Length);
            Assert.AreEqual("a1", svc.Signature.Parameters[0].Storage.ToString());
            Assert.AreEqual("d0", svc.Signature.Parameters[1].Storage.ToString());
            mr.VerifyAll();
        }

        [Test]
        public void AOS_LibrarySelection()
        {
            mr.ReplayAll();
            When_Create_Platform();
            var libs_v0 = platform.GetLibrarySetForKickstartVersion(0);
            var libs_v33 = platform.GetLibrarySetForKickstartVersion(33);
            var libs_v34 = platform.GetLibrarySetForKickstartVersion(34);
            var libs_v999 = platform.GetLibrarySetForKickstartVersion(999);

            Assert.AreEqual(0, libs_v0.Count);
            Assert.IsTrue(libs_v33.Contains("exec_v33"));
            Assert.IsTrue(libs_v34.Contains("exec_v34"));
            Assert.IsTrue(libs_v999.Contains("exec_v34")); //TODO: should select version from highest available kickstart version
            mr.VerifyAll();
        }

        private void Given_Func(string fileContents)
        {
            var stm = new MemoryStream(Encoding.ASCII.GetBytes(fileContents));
            fsSvc.Expect(f => f.CreateFileStream("exec.funcs", FileMode.Open, FileAccess.Read))
                .Return(stm);
            tllSvc.Stub(t => t.InstalledFileLocation(null)).IgnoreArguments().Return("exec.funcs");
        }

        private void When_Create_Platform()
        {
            platform = new AmigaOSPlatform(services, arch);
        }
    }
}
