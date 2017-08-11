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
using Reko.CmdLine;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
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
        private MockRepository mr;
        private ServiceContainer sc;
        private ILoader ldr;
        private IDecompiler decompiler;
        private IConfigurationService configSvc;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.sc = new ServiceContainer();
            this.ldr = mr.Stub<ILoader>();
            this.decompiler = mr.Stub<IDecompiler>();
            this.configSvc = mr.Stub<IConfigurationService>();
            sc.AddService<IDiagnosticsService>(new FakeDiagnosticsService());
            sc.AddService<IConfigurationService>(configSvc);
        }

        [Test(Description = "PR #406 describes this as causing a crash.")]
        public void CmdLine_EntryPoint()
        {
            var arch = mr.Stub<IProcessorArchitecture>();
            var state = mr.Stub<ProcessorState>();
            configSvc.Stub(s => s.GetArchitecture("mmix")).Return(arch);
            arch.Stub(a => a.TryParseAddress(
                Arg<string>.Is.Equal("010000"),
                out Arg<Address>.Out(Address.Ptr32(0x010000)).Dummy))
                .Return(true);
            arch.Stub(a => a.TryParseAddress(
                Arg<string>.Is.Equal("010100"),
                out Arg<Address>.Out(Address.Ptr32(0x010100)).Dummy))
                .Return(true); mr.ReplayAll();
            arch.Stub(a => a.CreateProcessorState()).Return(state);
            decompiler.Stub(d => d.LoadRawImage(null, null))
                .IgnoreArguments()
                .Return(new Program());
            mr.ReplayAll();

            var cmdline = new CmdLineDriver(sc, ldr, decompiler, null);
            cmdline.Execute(new string[]
            {
                "--arch",  "mmix",
                "--base",  "010000",
                "--entry", "010100",
                "foo.exe"
            });

            mr.VerifyAll();
        }
    }
}
