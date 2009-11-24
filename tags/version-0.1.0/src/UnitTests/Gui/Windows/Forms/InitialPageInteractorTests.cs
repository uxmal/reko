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
using System;
using System.ComponentModel.Design;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
	[TestFixture]
	public class InitialPageInteractorTests
	{
		private IMainForm form;
		private IStartPage page;
		private TestInitialPageInteractor i;
        private FakeUiService uiSvc;

		[SetUp]
		public void Setup()
		{
			form = new MainForm();
            page = form.InitialPage;
            i = new TestInitialPageInteractor(page);
            FakeComponentSite site = new FakeComponentSite(i);
            uiSvc = new FakeUiService();
            site.AddService(typeof(IDecompilerUIService), uiSvc);
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
		public void ClickOnBrowseBinaryFile()
		{
			page.InputFile.Text = "foo.bar";
			uiSvc.OpenFileResult = "baz\\foo.bar";
			i.BrowseInputFile_Click(null, EventArgs.Empty);

			Assert.AreEqual("baz\\foo.bar", page.InputFile.Text);
		}

		[Test]
		public void CancelBrowseBinaryFile()
		{
			page.InputFile.Text = "foo.bar";
			uiSvc.OpenFileResult = "NIX";
			uiSvc.SimulateUserCancel = true;
			i.BrowseInputFile_Click(null, EventArgs.Empty);

			Assert.AreEqual("foo.bar", page.InputFile.Text);
		}

        [Test]
        public void CanAdvance()
        {
            Assert.IsFalse(i.CanAdvance);
            i.OpenBinary("floxe.exe", new FakeDecompilerHost());
            Assert.IsTrue(i.CanAdvance, "Page should be ready to advance");
        }

        [Test]
        public void OpenBinary_ShouldPopulateFields()
        {
            Assert.IsFalse(i.CanAdvance, "Page should not be ready to advance");
            i.OpenBinary("floxe.exe", new FakeDecompilerHost());
            Assert.AreEqual("floxe.exe", page.InputFile.Text);
            Assert.AreEqual("floxe.asm", page.AssemblerFile.Text);
            Assert.AreEqual("floxe.dis", page.IntermediateFile.Text);
            Assert.AreEqual("floxe.c", page.SourceFile.Text);
            Assert.AreEqual("floxe.h", page.HeaderFile.Text);
            Assert.IsTrue(i.CanAdvance, "Page should be ready to advance");
        }

        [Test]
        public void LeavePage()
        {
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

        private class TestInitialPageInteractor : InitialPageInteractorImpl
        {
            public FakeDecompiler FakeDecompiler;

            public TestInitialPageInteractor(IStartPage page)
                : base(page)
            {
            }

            protected override LoaderBase CreateLoader(string filename, IServiceContainer sc)
            {
                return new FakeLoader(filename);
            }

            protected override IDecompiler CreateDecompiler(LoaderBase ldr, DecompilerHost host, IServiceProvider sp)
            {
                FakeDecompiler = new FakeDecompiler(ldr);
                return FakeDecompiler;
            }
        }
	}
}
