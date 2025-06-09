#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Forms
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class ScannedPageInteractorTests
    {
        private Mock<IMainForm> form;
        private Program program;
        private ScannedPageInteractor interactor;
        private IDecompilerService decSvc;
        private ServiceContainer sc;
        private ImageSegment mapSegment1;
        private ImageSegment mapSegment2;
        private Mock<IDecompilerShellUiService> uiSvc;
        private Mock<ILowLevelViewService> memSvc;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();

            form = new Mock<IMainForm>();

            var arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            var platform = new Mock<IPlatform>();
            platform.Setup(p => p.CreateMetadata()).Returns(new TypeLibrary());
            platform.Setup(p => p.Architecture).Returns(arch);
            program = new Program();
            program.Architecture = arch;
            program.Platform = platform.Object;
            var mem = new ByteMemoryArea(Address.SegPtr(0xC00, 0), new byte[10000]);
            program.SegmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment("0C00", mem, AccessMode.ReadWriteExecute));
            program.SegmentMap.AddOverlappingSegment("0C10", mem, Address.SegPtr(0x0C10, 0), AccessMode.ReadWrite);
            program.SegmentMap.AddOverlappingSegment("0C20", mem, Address.SegPtr(0x0C20, 0), AccessMode.ReadWrite);
            mapSegment1 = program.SegmentMap.Segments.Values[0];
            mapSegment2 = program.SegmentMap.Segments.Values[1];

            var project = new Project();
            project.AddProgram(ImageLocation.FromUri("/home/bob/test.exe"), program);

            decSvc = new DecompilerService();

            sc.AddService<IDecompilerService>(decSvc);
            sc.AddService<IWorkerDialogService>(new FakeWorkerDialogService());
            var eventListener = new FakeDecompilerEventListener();
            sc.AddService<IDecompilerEventListener>(eventListener);
            sc.AddService<IStatusBarService>(new FakeStatusBarService());
            sc.AddService<IDecompiledFileService>(new FakeDecompiledFileService());
            uiSvc = AddService<IDecompilerShellUiService>();
            memSvc = AddService<ILowLevelViewService>();

            decSvc.Decompiler = new Reko.Decompiler(project, sc);

            interactor = new ScannedPageInteractor(sc);
        }

        [TearDown]
        public void Teardown()
        {
            form.Object.Dispose();
        }

        private Mock<T> AddService<T>() where T : class
        {
            var oldSvc = sc.GetService(typeof(T));
            if (oldSvc is not null)
                sc.RemoveService(typeof(T));
            var svc = new Mock<T>();
            sc.AddService(typeof(T), svc.Object);
            return svc;
        }

        [Test]
        public void Lpi_PopulateBrowserWithScannedProcedures()
        {
            var brSvc = AddService<IProjectBrowserService>();
            AddService<IProcedureListService>();
            brSvc.Setup(b => b.Reload()).Verifiable();

            // Instead write expectations for the two added items.

            AddProcedure(Address.SegPtr(0xC20, 0x0000), "Test1");
            AddProcedure(Address.SegPtr(0xC20, 0x0002), "Test2");
            interactor.EnterPage();

            brSvc.VerifyAll();
        }

        private void AddProcedure(Address addr, string procName)
        {
            program.Procedures.Add(
                addr,
                new Procedure(program.Architecture, procName, addr, program.Architecture.CreateFrame()));
        }

        [Test]
        public void Lpi_SetBrowserCaptionWhenEnteringPage()
        {
            AddService<IDecompilerShellUiService>();
            AddService<ILowLevelViewService>();
            AddService<IDisassemblyViewService>();
            AddService<IProjectBrowserService>();
            AddService<IProcedureListService>();
            interactor.EnterPage();
        }

        [Test]
        public void Lpi_CallScanProgramWhenenteringPage()
        {
            var decSvc = AddService<IDecompilerService>();
            var decompiler = new Mock<IDecompiler>();
            var program = new Program();
            var mem = new ByteMemoryArea(Address.Ptr32(0x3000), new byte[10]);
            program.SegmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment(".text", mem, AccessMode.ReadWriteExecute));
            var project = new Project { Programs = { program } };
            decompiler.Setup(x => x.Project).Returns(project);
            decSvc.Setup(x => x.Decompiler).Returns(decompiler.Object);
            AddService<IDecompilerShellUiService>();
            AddService<ILowLevelViewService>();
            AddService<IDisassemblyViewService>();
            AddService<IProjectBrowserService>();
            AddService<IProcedureListService>();

            Assert.IsNotNull(sc);
            interactor.EnterPage();
        }

        private MenuStatus QueryStatus(int cmdId)
        {
            CommandStatus status = new CommandStatus();
            interactor.QueryStatus(new CommandID(CmdSets.GuidReko, cmdId), status, null);
            return status.Status;
        }
    }
}
