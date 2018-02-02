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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Loading;
using Reko.UnitTests.Mocks;
using Reko.Gui.Windows;
using Reko.Gui.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class ScannedPageInteractorTests
    {
        private IMainForm form;
        private Program program;
        private ScannedPageInteractor interactor;
        private IDecompilerService decSvc;
        private ServiceContainer sc;
        private MockRepository mr;
        private ImageSegment mapSegment1;
        private ImageSegment mapSegment2;
        private IDecompilerShellUiService uiSvc;
        private ILowLevelViewService memSvc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();

            form = new MainForm();

            var platform = mr.Stub<IPlatform>();
            program = new Program();
            program.Architecture = new X86ArchitectureReal();
            program.Platform = platform;
            var mem = new MemoryArea(Address.SegPtr(0xC00, 0), new byte[10000]);
            program.SegmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment("0C00", mem, AccessMode.ReadWriteExecute));

            program.SegmentMap.AddSegment(Address.SegPtr(0x0C10, 0), "0C10", AccessMode.ReadWrite, 0);
            program.SegmentMap.AddSegment(Address.SegPtr(0x0C20, 0), "0C20", AccessMode.ReadWrite, 0);
            mapSegment1 = program.SegmentMap.Segments.Values[0];
            mapSegment2 = program.SegmentMap.Segments.Values[1];

            decSvc = new DecompilerService();

            sc.AddService<IDecompilerService>(decSvc);
            sc.AddService<IWorkerDialogService>(new FakeWorkerDialogService());
            sc.AddService<DecompilerEventListener>(new FakeDecompilerEventListener());
            sc.AddService<IStatusBarService>(new FakeStatusBarService());
            sc.AddService<DecompilerHost>(new FakeDecompilerHost());
            uiSvc = AddService<IDecompilerShellUiService>();
            memSvc = AddService<ILowLevelViewService>();

            ILoader ldr = mr.StrictMock<ILoader>();
            ldr.Stub(l => l.LoadImageBytes("test.exe", 0)).Return(new byte[400]);
            ldr.Stub(l => l.LoadExecutable(
                Arg<string>.Is.NotNull,
                Arg<byte[]>.Is.NotNull,
                Arg<string>.Is.Null,
                Arg<Address>.Is.Null)).Return(program);
            ldr.Replay();
            decSvc.Decompiler = new DecompilerDriver(ldr, sc);
            decSvc.Decompiler.Load("test.exe");

            interactor = new ScannedPageInteractor(sc);
        }

        [TearDown]
        public void Teardown()
        {
            form.Dispose();
        }

        private T AddService<T>() where T : class
        {
            var oldSvc = sc.GetService(typeof(T));
            if (oldSvc != null)
                sc.RemoveService(typeof(T));
            var svc = mr.DynamicMock<T>();
            sc.AddService(typeof(T), svc);
            return svc;
        }

        [Test]
        public void LpiPopulateBrowserWithScannedProcedures()
        {
            var brSvc = AddService<IProjectBrowserService>();
            brSvc.Expect(b => b.Reload());
            mr.ReplayAll();

            // Instead write expectations for the two added items.

            AddProcedure(Address.SegPtr(0xC20, 0x0000), "Test1");
            AddProcedure(Address.SegPtr(0xC20, 0x0002), "Test2");
            interactor.EnterPage();
        }

        private void AddProcedure(Address addr, string procName)
        {
            program.Procedures.Add(addr,
                new Procedure(procName, program.Architecture.CreateFrame()));
        }

        [Test]
        public void Lpi_SetBrowserCaptionWhenEnteringPage()
        {
            AddService<IDecompilerShellUiService>();
            AddService<ILowLevelViewService>();
            AddService<IDisassemblyViewService>();
            AddService<IProjectBrowserService>();
            mr.ReplayAll();

            interactor.EnterPage();

            mr.VerifyAll();
        }

        [Test]
        public void Lpi_CallScanProgramWhenenteringPage()
        {
            var decSvc = AddService<IDecompilerService>();
            var decompiler = mr.Stub<IDecompiler>();
            var program = new Program();
            var mem = new MemoryArea(Address.Ptr32(0x3000), new byte[10]);
            program.SegmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment(".text", mem, AccessMode.ReadWriteExecute));
            var project = new Project { Programs = { program } };
            decompiler.Stub(x => x.Project).Return(project);
            decSvc.Stub(x => x.Decompiler).Return(decompiler);
            AddService<IDecompilerShellUiService>();
            AddService<ILowLevelViewService>();
            AddService<IDisassemblyViewService>();
            AddService<IProjectBrowserService>();
            mr.ReplayAll();

            Assert.IsNotNull(sc);
            interactor.EnterPage();

            mr.VerifyAll();
        }

        private MenuStatus QueryStatus(int cmdId)
        {
            CommandStatus status = new CommandStatus();
            interactor.QueryStatus(new CommandID(CmdSets.GuidReko, cmdId), status, null);
            return status.Status;
        }
    }
}
