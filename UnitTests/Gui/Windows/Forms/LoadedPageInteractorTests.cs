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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using Decompiler.Gui;
using Decompiler.Gui.Forms;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    public class LoadedPageInteractorTests
    {
        private IMainForm form;
        private Program prog;
        private LoadedPageInteractor interactor;
        private IDecompilerService decSvc;
        private ServiceContainer sc;
        private MockRepository mr;
        private ImageMapSegment mapSegment1;
        private ImageMapSegment mapSegment2;
        private IDecompilerShellUiService uiSvc;
        private ILowLevelViewService memSvc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();

            form = new MainForm();

            prog = new Program();
            prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
            prog.Image = new LoadedImage(Address.SegPtr(0xC00, 0), new byte[10000]);
            prog.ImageMap = prog.Image.CreateImageMap();

            prog.ImageMap.AddSegment(Address.SegPtr(0x0C10, 0), "0C10", AccessMode.ReadWrite);
            prog.ImageMap.AddSegment(Address.SegPtr(0x0C20, 0), "0C20", AccessMode.ReadWrite);
            mapSegment1 = prog.ImageMap.Segments.Values[0];
            mapSegment2 = prog.ImageMap.Segments.Values[1];

            sc = new ServiceContainer();
            decSvc = new DecompilerService();

            sc.AddService(typeof(IDecompilerService), decSvc);
            sc.AddService(typeof(IWorkerDialogService), new FakeWorkerDialogService());
            sc.AddService(typeof(DecompilerEventListener), new FakeDecompilerEventListener());
            sc.AddService(typeof(IStatusBarService), new FakeStatusBarService());
            uiSvc = AddService<IDecompilerShellUiService>();
            memSvc = AddService<ILowLevelViewService>();

            ILoader ldr = mr.StrictMock<ILoader>();
            ldr.Stub(l => l.LoadImageBytes("test.exe", 0)).Return(new byte[400]);
            ldr.Stub(l => l.LoadExecutable(
                Arg<string>.Is.NotNull,
                Arg<byte[]>.Is.NotNull,
                Arg<Address>.Is.Null)).Return(prog);
            ldr.Replay();
            decSvc.Decompiler = new DecompilerDriver(ldr, new FakeDecompilerHost(), sc);
            decSvc.Decompiler.Load("test.exe");

            interactor = new LoadedPageInteractor(sc);
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
        [Ignore]
        public void LpiPopulateBrowserWithScannedProcedures()
        {
            // Instead write expectations for the two added items.

            AddProcedure(Address.SegPtr(0xC20, 0x0000), "Test1");
            AddProcedure(Address.SegPtr(0xC20, 0x0002), "Test2");
            interactor.EnterPage();
            //Assert.AreEqual(3, form.BrowserList.Items.Count);
            //Assert.AreEqual("0C20", form.BrowserList.Items[2].Text);
        }

        private void AddProcedure(Address addr, string procName)
        {
            prog.Procedures.Add(addr,
                new Procedure(procName, prog.Architecture.CreateFrame()));
        }

        [Test]
        [Ignore("Move this to low-level? Or Decompiler?")]
        public void LpiMarkingProceduresShouldAddToUserProceduresList()
        {
            var disSvc = AddService<IDisassemblyViewService>();
            Assert.AreEqual(0, decSvc.Decompiler.Project.Programs[0].UserProcedures.Count);
            var addr = Address.SegPtr(0x0C20, 0);
            memSvc.Expect(s => s.GetSelectedAddressRange()).Return(new AddressRange(addr, addr));
            memSvc.Expect(s => s.InvalidateWindow()).IgnoreArguments();
            mr.ReplayAll();

            //interactor.MarkAndScanProcedure(prog);

            mr.VerifyAll();
            //$REVIEW: Need to pass InputFile into the SelectedProcedureEntry piece.
            var program = decSvc.Decompiler.Project.Programs[0];
            Assert.AreEqual(1, program.UserProcedures.Count);
            Procedure_v1 uproc = (Procedure_v1)program.UserProcedures.Values[0];
            Assert.AreEqual("0C20:0000", uproc.Address);
        }

        [Test]
        public void Lpi_QueryStatus()
        {
            Assert.AreEqual(MenuStatus.Enabled | MenuStatus.Visible, QueryStatus(CmdIds.ViewFindFragments));
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
            var prog = new Program();
            prog.Image = new LoadedImage(Address.Ptr32(0x3000), new byte[10]);
            prog.ImageMap = prog.Image.CreateImageMap();
            var project = new Project { Programs = { prog } };
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
            interactor.QueryStatus(new CommandID(CmdSets.GuidDecompiler, cmdId), status, null);
            return status.Status;
        }
    }
}
