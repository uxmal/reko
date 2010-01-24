/* 
 * Copyright (C) 1999-2010 John Källén.
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
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
	[TestFixture]
	public class MainFormInteractorTests
	{
		private IMainForm form;
		private TestMainFormInteractor interactor;
        private MockRepository repository;

		[SetUp]
		public void Setup()
		{
            repository = new MockRepository();
		}

		[TearDown]
		public void TearDown()
		{
			form.Dispose();
		}

		[Test]
		public void CreateForm()
		{
            CreateMainFormInteractor();
			Assert.AreSame(interactor.InitialPageInteractor, interactor.CurrentPage);
		}

		[Test]
		public void OpenBinary()
		{
            CreateMainFormInteractor();
			interactor.OpenBinary("floxie.exe");
            Assert.AreSame(interactor.CurrentPage, interactor.InitialPageInteractor);
            Assert.IsTrue(((FakeInitialPageInteractor)interactor.InitialPageInteractor).OpenBinaryCalled);
		}

        [Test]
        public void OpenBinaryShouldClearDiagnostics()
        {
            CreateMainFormInteractor();
            IDiagnosticsService svc = (IDiagnosticsService) interactor.ProbeGetService(typeof(IDiagnosticsService));
            svc.AddDiagnostic(new ErrorDiagnostic(null, "test"));
            interactor.OpenBinary(null);
            Assert.AreEqual(0, form.DiagnosticsList.Items.Count);
        }

		[Test]
		public void NextPhase()
		{
            CreateMainFormInteractor();
			interactor.OpenBinary(null);
			Assert.AreSame(interactor.InitialPageInteractor, interactor.CurrentPage);
			interactor.NextPhase();
			Assert.AreSame(interactor.LoadedPageInteractor, interactor.CurrentPage);
		}


        [Test]
        public void Save()
        {
            CreateMainFormInteractor();

            IDecompilerService svc = (IDecompilerService)interactor.ProbeGetService(typeof(IDecompilerService));
            svc.Decompiler = interactor.CreateDecompiler(new FakeLoader());
            svc.Decompiler.LoadProgram("foo.exe");
            Decompiler.Core.Serialization.SerializedProcedure p = new Decompiler.Core.Serialization.SerializedProcedure();
            p.Address = "12345";
            p.Name = "MyProc";
            svc.Decompiler.Project.UserProcedures.Add(p);

            interactor.Save();
            string s =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<project xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://schemata.jklnet.org/Decompiler"">
  <input>
    <filename>foo.exe</filename>
    <address>0C00:0000</address>
  </input>
  <output>
    <disassembly>foo.asm</disassembly>
    <intermediate-code>foo.dis</intermediate-code>
    <output>foo.c</output>
    <types-file>foo.h</types-file>
  </output>
  <procedure name=""MyProc"">
    <address>12345</address>
  </procedure>
</project>";
            Console.WriteLine(interactor.ProbeSavedProjectXml);

            Assert.AreEqual(s, interactor.ProbeSavedProjectXml);
        }

        [Test]
        public void SaveShouldShowDialog()
        {
            CreateMainFormInteractor();
            Assert.IsNull(interactor.ProjectFileName);

            IDecompilerService svc = (IDecompilerService)interactor.ProbeGetService(typeof(IDecompilerService));
            svc.Decompiler = interactor.CreateDecompiler(new FakeLoader());
            svc.Decompiler.LoadProgram("foo.exe");

            Assert.IsTrue(string.IsNullOrEmpty(interactor.ProjectFileName), "project filename should be clear");
            interactor.Save();
            Assert.IsTrue(interactor.ProbePromptedForSaving, "Should have prompted for saving as no file name was supplied.");
            Assert.AreEqual("foo.dcproject", interactor.ProbeFilename);
        }

        [Test]
        public void GetDiagnostics()
        {
            CreateMainFormInteractorWithLoader();
            form.Show();
            object oSvc = interactor.ProbeGetService(typeof(IDiagnosticsService));
            Assert.IsNotNull(oSvc, "IDiagnosticsService should be available!");
            IDiagnosticsService svc = (IDiagnosticsService) oSvc;
            svc.AddDiagnostic(new WarningDiagnostic(new Address(0x30000), "Whoa"));
            Assert.AreEqual(1, form.DiagnosticsList.Items.Count, "Should have added an item to diagnostics list.");
        }

        [Test]
        public void DecompilerServiceInstalled()
        {
            CreateMainFormInteractor();
            Assert.IsNotNull(interactor.ProbeGetService(typeof (IDecompilerService)), "Should have IDecompilerService available.");
        }

        [Test] 
        public void IsNextPhaseEnabled()
        {
            CreateMainFormInteractorWithLoader();
            var page = new FakePhasePageInteractor();
            interactor.SwitchInteractor(page);
            CommandStatus status;
            page.CanAdvance = false;
            status = QueryStatus(CmdIds.ActionNextPhase);
            Assert.IsNotNull(status, "MainFormInteractor should know this command.");
            Assert.AreEqual(MenuStatus.Visible, status.Status);
            page.CanAdvance = true;
            status = QueryStatus(CmdIds.ActionNextPhase);
            Assert.IsNotNull(status, "MainFormInteractor should know this command.");
            Assert.AreEqual(MenuStatus.Visible|MenuStatus.Enabled, status.Status);
        }

        [Test]
        public void GetWorkerService()
        {
            CreateMainFormInteractor();
            Assert.IsNotNull(interactor.ProbeGetService(typeof(IWorkerDialogService)));

        }

        [Test]
        public void StatusBarServiceSetText()
        {
            CreateMainFormInteractor();
            var sbSvc = (IStatusBarService)interactor.ProbeGetService(typeof(IStatusBarService));
            sbSvc.SetText("Hello!");
            Assert.AreEqual("Hello!", form.StatusStrip.Items[0].Text);
        }

        [Test]
        public void QueryFindProceduresNoProgramLoaded()
        {
            CreateMainFormInteractor();
            CommandStatus status;
            status = QueryStatus(CmdIds.ViewFindAllProcedures);
            Assert.AreEqual(MenuStatus.Visible,  status.Status);
        }


        [Test]
        public void QueryFindProceduresLoaded()
        {
            CreateMainFormInteractorWithLoader();
            IDecompilerService svc = (IDecompilerService)interactor.ProbeGetService(typeof(IDecompilerService));
            svc.Decompiler = interactor.CreateDecompiler(new FakeLoader());
            svc.Decompiler.LoadProgram("foo.exe");
            var status = QueryStatus(CmdIds.ViewFindAllProcedures);
            Assert.AreEqual(MenuStatus.Visible|MenuStatus.Enabled, status.Status);
        }

        [Test]
        public void ExecuteFindProcedures()
        {
            CreateMainFormInteractorWithLoader();
            IDecompilerService svc = (IDecompilerService)interactor.ProbeGetService(typeof(IDecompilerService));
            svc.Decompiler = interactor.CreateDecompiler(new FakeLoader());
            svc.Decompiler.LoadProgram("foo.exe");

            var srSvc = repository.StrictMock<ISearchResultService>();
            srSvc.Expect(s => s.ShowSearchResults(
                Arg<ISearchResult>.Is.Anything));
            repository.ReplayAll();

            interactor.FindProcedures(srSvc);

            repository.VerifyAll();
        }

        private Program CreateFakeProgram()
        {
            Program prog = new Program();
            prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
            prog.Image = new ProgramImage(new Address(0xC00, 0), new byte[300]);
            return prog; 
        }

        private void CreateMainFormInteractorWithLoader()
        {
            Program prog = new Program();
            interactor = new TestMainFormInteractor(prog, new FakeLoader());
            form = interactor.LoadForm();
        }

        private void CreateMainFormInteractor()
        {
            Program prog = CreateFakeProgram();
            interactor = new TestMainFormInteractor(prog);
            form = interactor.LoadForm();
        }

        //$REFACTOR: copied from LoadedPageInteractor, should
        // push to base class or utility class.
        private CommandStatus QueryStatus(int cmdId)
        {
            CommandStatus status = new CommandStatus();
            if (interactor.QueryStatus(ref CmdSets.GuidDecompiler, cmdId, status, null))
                return status;
            else
                return null;
        }

	}

	public class TestMainFormInteractor : MainFormInteractor
	{
		private DecompilerDriver decompiler;
        private LoaderBase ldr;
		private Program program;
        private StringWriter sw;
        private string testFilename;
        private bool promptedForSaving;

		public TestMainFormInteractor(Program prog)
		{
            this.program = prog;
		}

		public TestMainFormInteractor(DecompilerDriver decompiler)
		{
            this.decompiler = decompiler;
		}

        public TestMainFormInteractor(Program prog, LoaderBase ldr)
        {
            this.program = prog;
            this.ldr = ldr;
        }

        // Overrides of creation methods.

        public override IDecompiler CreateDecompiler(LoaderBase ldr)
		{
            if (decompiler != null)
                return decompiler;
            return base.CreateDecompiler(ldr);
		}

        protected override DecompilerEventListener CreateDecompilerListener()
        {
            return new FakeDecompilerEventListener();
        }

        protected override InitialPageInteractor CreateInitialPageInteractor()
        {
            return new FakeInitialPageInteractor();
        }

        protected override ILoadedPageInteractor CreateLoadedPageInteractor()
        {
            return new FakeLoadedPageInteractor();
        }

        protected override IDecompilerShellUiService CreateShellUiService(DecompilerMenus dm)
        {
            return new FakeShellUiService();
        }

        public override TextWriter CreateTextWriter(string filename)
        {
            testFilename = filename;
            sw = new StringWriter();
            return sw;
        }

        protected override string PromptForFilename(string suggestedName)
        {
            promptedForSaving = true;
            testFilename = suggestedName;
            return suggestedName;
        }

        public object ProbeGetService(Type service)
        {
            return base.GetService(service);
        }

        public string ProbeSavedProjectXml
        {
            get { return sw.ToString(); }
        }

        public string ProbeFilename
        {
            get { return testFilename; }
        }


        public bool ProbePromptedForSaving
        {
            get { return promptedForSaving; }
        }


    }
}
