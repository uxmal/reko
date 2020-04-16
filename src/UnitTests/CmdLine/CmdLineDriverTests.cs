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
using Reko.CmdLine;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Drivers.CmdLine
{
    [TestFixture]
    public class CmdLineDriverTests
    {
        private ServiceContainer sc;
        private Mock<ILoader> ldr;
        private Mock<IDecompiler> decompiler;
        private Mock<IConfigurationService> configSvc;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            this.ldr = new Mock<ILoader>();
            this.decompiler = new Mock<IDecompiler>();
            this.configSvc = new Mock<IConfigurationService>();
            sc.AddService<IDiagnosticsService>(new FakeDiagnosticsService());
            sc.AddService<IConfigurationService>(configSvc.Object);
        }

        [Test(Description = "PR #406 describes this as causing a crash.")]
        public void CmdLine_EntryPoint()
        {
            var arch = new Mock<IProcessorArchitecture>();
            var map = new SegmentMap(Address.Ptr32(0x010000));
            var state = new FakeProcessorState(arch.Object);
            configSvc.Setup(s => s.GetArchitecture("mmix")).Returns(arch.Object);

            Address addr = Address.Ptr32(0x010000);
            arch.Setup(a => a.TryParseAddress(
                "010000",
                out addr))
                .Returns(true);
            addr = Address.Ptr32(0x010100);
            arch.Setup(a => a.TryParseAddress(
                "010100",
                out addr))
                .Returns(true);
            arch.Setup(a => a.CreateProcessorState()).Returns(state);
            decompiler.Setup(d => d.LoadRawImage(
                It.IsAny<string>(),
                It.IsAny<LoadDetails>()))
                .Returns(new Program());

            var cmdline = new CmdLineDriver(sc, ldr.Object, decompiler.Object, null);
            cmdline.Execute(new string[]
            {
                "--arch",  "mmix",
                "--base",  "010000",
                "--entry", "010100",
                "foo.exe"
            });
        }

        // Verify that architecture options are being passed correctly.
        [Test]
        public void CmdLine_ArchOptions()
        {
            var arch = new Mock<IProcessorArchitecture>();
            configSvc.Setup(c => c.GetArchitecture("test")).Returns(arch.Object);
            arch.Setup(a => a.LoadUserOptions(
                It.Is<Dictionary<string, object>>(d =>
                    (string) d["option1"] == "value1" &&
                    (string) d["option2"] == "value2"))).Verifiable();
            var addr = Address.Ptr32(0x0000);
            arch.Setup(a => a.TryParseAddress(
                "0000",
                out addr))
                .Returns(true);

            var cmdline = new CmdLineDriver(sc, ldr.Object, decompiler.Object, null);
            cmdline.Execute(new[]
            {
                "--arch", "test",
                "--arch-option", "option1=value1",
                "--arch-option", "option2=value2",
                "--base", "0000",
                "foo.bin"
            });
            arch.VerifyAll();
        }
    }
}
