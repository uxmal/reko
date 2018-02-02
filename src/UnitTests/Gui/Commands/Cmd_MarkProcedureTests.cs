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
using Reko.Core.Serialization;
using Reko.Gui;
using Reko.Gui.Commands;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Gui.Commands
{
    [TestFixture]
    public class Cmd_MarkProcedureTests
    {
        private ServiceContainer sc;
        private MockRepository mr;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            this.mr = new MockRepository();
            var mem = new MemoryArea(Address.SegPtr(0x0C00, 0), new byte[100]);
            this.program = new Program
            {
                SegmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment("0C00", mem, AccessMode.ReadWriteExecute))
            };
        }

        [Test]
        public void Cmasp_Do()
        {
            var addr = Address.SegPtr(0x0C20, 0);
            var proc = new Procedure("foo", null);
            var progaddr = new ProgramAddress(program, addr);
            var dc = mr.Stub<IDecompiler>();
            var dcSvc = mr.Stub<IDecompilerService>();
            var brSvc = mr.Stub<IProjectBrowserService>();
            dcSvc.Decompiler = dc;
            sc.AddService<IDecompilerService>(dcSvc);
            sc.AddService<IProjectBrowserService>(brSvc);
            sc.AddService<IDecompilerShellUiService>(new FakeShellUiService());
            dc.Expect(d => d.ScanProcedure(progaddr)).IgnoreArguments().Return(proc);
            brSvc.Expect(b => b.Reload());
            mr.ReplayAll();

            var locations = new[] { new ProgramAddress(program, addr) };
            var cmd = new Cmd_MarkProcedures(sc, locations);
            cmd.DoIt();

            mr.VerifyAll();
            Assert.AreEqual(1, program.User.Procedures.Count);
            var uproc = program.User.Procedures.Values[0];
            Assert.AreEqual("0C20:0000", uproc.Address);
        }
    }
}
