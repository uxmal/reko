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

using Decompiler.Arch.M68k;
using Decompiler.Core;
using Decompiler.Core.Rtl;
using Decompiler.Core.Services;
using Decompiler.Environments.AmigaOS;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Environments.AmigaOS
{
    [TestFixture]
    public class AmigaOSPlatformTests
    {
        private M68kArchitecture arch;
        private MockRepository mr;
        private IFileSystemService fsSvc;
        private IServiceProvider services;
        private RtlEmitter m;
        private AmigaOSPlatform platform;
        private List<RtlInstruction> rtls;
        private Frame frame;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.fsSvc = mr.StrictMock<IFileSystemService>();
            this.services = mr.StrictMock<IServiceProvider>();
            this.arch = new M68kArchitecture();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            this.services.Stub(s => s.GetService(typeof(IFileSystemService))).Return(fsSvc);
            this.frame = new Frame(arch.FramePointerType);
        }

        [Test]
        public void AOS_LookupA6Call()
        {
            Given_Func("#1 512 0x200 Allocate( heap/a1, size/d0)\n");
            mr.ReplayAll();

            When_Create_Platform();
            m.Call(m.LoadDw(m.IAdd(frame.EnsureRegister(Registers.a6), -512)), 4);
            var state = arch.CreateProcessorState();
            var svc = platform.FindService(rtls.Last(), state);

            Assert.AreEqual("Allocate", svc.Name);
            Assert.AreEqual(2, svc.Signature.Parameters.Length);
            Assert.AreEqual("a1", svc.Signature.Parameters[0].Storage.ToString());
            Assert.AreEqual("d0", svc.Signature.Parameters[1].Storage.ToString());
            mr.VerifyAll();
        }

        private void Given_Func(string fileContents)
        {
            var stm = new MemoryStream(Encoding.ASCII.GetBytes(fileContents));
            fsSvc.Expect(f => f.CreateFileStream("exec.funcs", FileMode.Open, FileAccess.Read))
                .Return(stm);
        }

        private void When_Create_Platform()
        {
            platform = new AmigaOSPlatform(services, arch);
        }
    }
}
