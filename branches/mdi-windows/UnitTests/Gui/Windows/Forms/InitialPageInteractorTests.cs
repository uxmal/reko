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

using Decompiler.Core.Services;
using Decompiler.Gui;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.ComponentModel.Design;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
	[TestFixture]
	public class InitialPageInteractorTests
	{
		private IMainForm form;
		private TestInitialPageInteractor i;
        private FakeUiService uiSvc;
        private FakeComponentSite site;

		[SetUp]
		public void Setup()
		{
			form = new MainForm();
            i = new TestInitialPageInteractor();
            site = new FakeComponentSite(i);
            uiSvc = new FakeShellUiService();
            site.AddService(typeof(IDecompilerUIService), uiSvc);
            site.AddService(typeof(IDecompilerShellUiService), uiSvc);
            site.AddService(typeof(IDecompilerService), new DecompilerService());
            site.AddService(typeof(IWorkerDialogService), new FakeWorkerDialogService());
            site.AddService(typeof(DecompilerEventListener), new FakeDecompilerEventListener());
            site.AddService(typeof(IProgramImageBrowserService), new ProgramImageBrowserService(form.BrowserList));
            i.Site = site;
		}

		[TearDown]
		public void Teardown()
		{
			form.Dispose();
		}

        [Test]
        public void InitialControls()
        {
            i.EnterPage();
            // When opening the application for the very first time, we should be on the initial page, and 
            // most controls on the mainform should be disabled.

            Assert.IsFalse(form.BrowserList.Enabled, "Browser list should be disabled");
        }


        [Test]
        public void OpenBinary_CanAdvance()
        {
            AddFakeMemoryViewService();
            Assert.IsFalse(i.CanAdvance);
            i.OpenBinary("floxe.exe", new FakeDecompilerHost());
            Assert.IsTrue(i.CanAdvance, "Page should be ready to advance");
        }

        [Test]
        public void OpenBinary_ShouldPopulateFields()
        {
            AddFakeMemoryViewService();
            Assert.IsFalse(i.CanAdvance, "Page should not be ready to advance");
            i.OpenBinary("floxe.exe", new FakeDecompilerHost());
            Assert.IsTrue(i.CanAdvance, "Page should be ready to advance");
        }

        [Test]
        public void OpenBinary_ShouldShowMemoryWindow()
        {
            var memSvc = AddFakeMemoryViewService();
            memSvc.Expect(s => s.ViewImage(null)).IgnoreArguments();

            i.OpenBinary("floxe.exe", new FakeDecompilerHost());

            memSvc.VerifyAllExpectations();

        }

        [Test]
        public void LeavePage()
        {
            AddFakeMemoryViewService();
            i.OpenBinary("foo.exe", new FakeDecompilerHost());
            Assert.IsTrue(i.FakeDecompiler.LoadProgram_Called, "LoadProgram should have been called");
            Assert.IsTrue(i.LeavePage());
            Assert.IsTrue(i.FakeDecompiler.ScanProgram_Called, "ScanProgram should have been called");
        }

        //$REFACTOR: copied from LoadedPageInteractor, should
        // push to base class or utility class.
        private MenuStatus QueryStatus(int cmdId)
        {
            CommandStatus status = new CommandStatus();
            i.QueryStatus(ref CmdSets.GuidDecompiler, cmdId, status, null);
            return status.Status;
        }

        private IMemoryViewService AddFakeMemoryViewService()
        {
            var memSvc = MockRepository.GenerateMock<IMemoryViewService>();
            site.AddService(typeof(IMemoryViewService), memSvc);
            return memSvc;
        }



        private class TestInitialPageInteractor : InitialPageInteractorImpl
        {
            public FakeDecompiler FakeDecompiler;

            public TestInitialPageInteractor()
            {
            }

            protected override LoaderBase CreateLoader(IServiceContainer sc)
            {
                return new FakeLoader();
            }

            protected override IDecompiler CreateDecompiler(LoaderBase ldr, DecompilerHost host, IServiceProvider sp)
            {
                FakeDecompiler = new FakeDecompiler(ldr);
                return FakeDecompiler;
            }
        }
	}
}
