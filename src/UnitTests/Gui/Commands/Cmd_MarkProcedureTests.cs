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
using Reko.Core;
using Reko.Gui;
using Reko.Gui.Commands;
using Reko.UnitTests.Mocks;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Commands
{
    [TestFixture]
    public class Cmd_MarkProcedureTests
    {
        private ServiceContainer sc;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            var mem = new MemoryArea(Address.SegPtr(0x0C00, 0), new byte[100]);
            this.program = new Program
            {
                SegmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment("0C00", mem, AccessMode.ReadWriteExecute)),
                Architecture = new FakeArchitecture(),
            };
            program.BuildImageMap();
        }

        [Test]
        public void Cmasp_Do()
        {
            var addr = Address.SegPtr(0x0C20, 0);
            var proc = new Procedure(new FakeArchitecture(), "foo", addr, null);
            var progaddr = new ProgramAddress(program, addr);
            var dc = new Mock<IDecompiler>();
            var dcSvc = new Mock<IDecompilerService>();
            var brSvc = new Mock<IProjectBrowserService>();
            var procSvc = new Mock<IProcedureListService>();
            var uiSvc = new Mock<IDecompilerShellUiService>();
            dcSvc.Setup(d => d.Decompiler).Returns(dc.Object);
            sc.AddService<IDecompilerService>(dcSvc.Object);
            sc.AddService<IProjectBrowserService>(brSvc.Object);
            sc.AddService<IProcedureListService>(procSvc.Object);
            sc.AddService<IDecompilerShellUiService>(uiSvc.Object);
            dc.Setup(d => d. ScanProcedure(
                It.IsAny<ProgramAddress>(),
                It.IsAny<IProcessorArchitecture>())
            ).Returns(proc).Verifiable();
            brSvc.Setup(b => b.Reload()).Verifiable();
            brSvc.Setup(b => b.CurrentProgram).Returns(program);

            var locations = new[] { new ProgramAddress(program, addr) };
            var cmd = new Cmd_MarkProcedures(sc, locations);
            cmd.DoIt();

            dc.VerifyAll();
            brSvc.Verify();
            Assert.AreEqual(1, program.User.Procedures.Count);
            var uproc = program.User.Procedures.Values[0];
            Assert.AreEqual("0C20:0000", uproc.Address);
        }
    }
}
