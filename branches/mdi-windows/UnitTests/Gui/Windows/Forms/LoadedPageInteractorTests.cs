/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
	[TestFixture]
	public class LoadedPageInteractorTests
	{
		private IMainForm form;
        private Program prog;
        private LoadedPageInteractor interactor;
        private DecompilerService decSvc;
        private FakeComponentSite site;

		[SetUp]
		public void Setup()
		{
            form = new MainForm();
            interactor = new LoadedPageInteractor();

            site = new FakeComponentSite(interactor);

            ProgramImageBrowserService svc = new ProgramImageBrowserService(form.BrowserList);
            site.AddService(typeof(IProgramImageBrowserService), svc);

            decSvc = new DecompilerService();
            site.AddService(typeof(IDecompilerService), decSvc);

            site.AddService(typeof(IWorkerDialogService), new FakeWorkerDialogService());
            site.AddService(typeof(DecompilerEventListener), new FakeDecompilerEventListener());
            site.AddService(typeof(IStatusBarService), new FakeStatusBarService());

            prog = new Program();
            prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
            prog.Image = new ProgramImage(new Address(0xC00, 0), new byte[10000]);
            prog.Image.Map.AddSegment(new Address(0x0C10, 0), "0C10", AccessMode.ReadWrite);
            prog.Image.Map.AddSegment(new Address(0x0C20, 0), "0C20", AccessMode.ReadWrite);

            TestLoader ldr = new TestLoader(new DecompilerProject(), prog);
            decSvc.Decompiler = new DecompilerDriver(ldr, new FakeDecompilerHost(), site);
            decSvc.Decompiler.LoadProgram("test.exe");

		}

		[TearDown]
		public void Teardown()
		{
			form.Dispose();
		}

		[Test]
		public void Populate()
		{
            var memSvc = AddService<IMemoryViewService>();
            var disSvc = AddService<IDisassemblyViewService>();
            var uiSvc = AddService<IDecompilerShellUiService>();
            interactor.Site = site;

            interactor.EnterPage();
			ListView lv = form.BrowserList;
			Assert.AreEqual(3, lv.Items.Count, "There should be three segments in the image.");
		}

        private T AddService<T>() where T : class
        {
            var svc = MockRepository.GenerateMock<T>();
            site.AddService(typeof(T), svc);
            return svc;
        }

		[Test]
		public void PopulateBrowserWithScannedProcedures()
		{
            var memSvc = AddService<IMemoryViewService>();
            var disSvc = AddService<IDisassemblyViewService>();
            var uiSvc = AddService<IDecompilerShellUiService>();
            interactor.Site = site;

            AddProcedure(new Address(0xC20, 0x0000), "Test1");
            AddProcedure(new Address(0xC20, 0x0002), "Test2");
            interactor.EnterPage();
            Assert.AreEqual(3, form.BrowserList.Items.Count);
			Assert.AreEqual("0C20", form.BrowserList.Items[2].Text);
		}

        private void AddProcedure(Address addr, string procName)
        {
            Program prog = decSvc.Decompiler.Program;
            decSvc.Decompiler.Program.Procedures.Add(addr,
                new Procedure(procName, prog.Architecture.CreateFrame()));
        }

		[Test]
		public void MarkingProceduresShouldAddToUserProceduresList()
		{
            var memSvc = AddService<IMemoryViewService>();
            var uiSvc = AddService<IDecompilerShellUiService>();
            var disSvc = AddService<IDisassemblyViewService>();
            interactor.Site = site;
            Assert.AreEqual(0, decSvc.Decompiler.Project.UserProcedures.Count);
            var addr = new Address(0x0C20, 0);
            memSvc.ShowMemoryAtAddress(addr);
            memSvc.Expect(s => s.GetSelectedAddressRange()).Return(new AddressRange(addr, addr));
            memSvc.Expect(s => s.InvalidateWindow()).IgnoreArguments();
			interactor.MarkAndScanProcedure();

            memSvc.VerifyAllExpectations();
            Assert.AreEqual(1, decSvc.Decompiler.Project.UserProcedures.Count);
			SerializedProcedure uproc = (SerializedProcedure) decSvc.Decompiler.Project.UserProcedures[0];
			Assert.AreEqual("0C20:0000", uproc.Address);
		}

		[Test]
		public void QueryStatus()
		{
			Assert.AreEqual(MenuStatus.Enabled|MenuStatus.Visible, QueryStatus(CmdIds.ViewFindFragments));
		}

        [Test]
        public void ShowEditFindDialogButDontRunIt()
        {
            var memSvc = AddService<IMemoryViewService>();
            var disSvc = AddService<IDisassemblyViewService>();
            var uiSvc = AddService<IDecompilerShellUiService>();
            interactor.Site = site;

            uiSvc.Expect(s => s.ShowModalDialog(
                Arg<FindDialog>.Is.TypeOf))
                .Return(DialogResult.Cancel);
            interactor.Execute(ref CmdSets.GuidDecompiler, CmdIds.EditFind);
            uiSvc.VerifyAllExpectations();
        }

		private MenuStatus QueryStatus(int cmdId)
		{
			CommandStatus status = new CommandStatus();
			interactor.QueryStatus(ref CmdSets.GuidDecompiler, cmdId, status, null);
			return status.Status;
		}


        private class TestLoader : LoaderBase
        {
            private DecompilerProject project;
            private Program prog;

            public TestLoader(DecompilerProject project, Program prog)
            {
                this.project = project;
                this.prog = prog;
            }

            public override Program Load(byte[] imageFile, Address userSpecifiedAddress)
            {
                return prog;
            }

            public override byte[] LoadImageBytes(string fileName, int offset)
            {
                return new byte[400];
            }
        }
	}
}
