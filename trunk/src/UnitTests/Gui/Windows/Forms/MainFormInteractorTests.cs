#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Forms;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        private Program prog;
        private IDialogFactory dlgFactory;
        private IServiceFactory svcFactory;
        private ServiceContainer services;
        private IMemoryViewService memSvc;
        private IDisassemblyViewService disasmSvc;
        private IDiagnosticsService diagnosticSvc;
        private IDecompilerShellUiService uiSvc;
        private ITypeLibraryLoaderService typeLibSvc;

		[SetUp]
		public void Setup()
		{
            repository = new MockRepository();
            services = new ServiceContainer();
		}

		[Test]
		public void OpenBinary_SwitchToInitialPhase()
		{
            Given_MainFormInteractor();
            diagnosticSvc.Stub(d => d.ClearDiagnostics());
            form.Stub(f => f.CloseAllDocumentWindows());
            repository.ReplayAll();

            When_CreateMainFormInteractor();
			interactor.OpenBinary("floxie.exe");
            Assert.AreSame(interactor.CurrentPhase, interactor.InitialPageInteractor);
            Assert.IsTrue(((FakeInitialPageInteractor)interactor.InitialPageInteractor).OpenBinaryCalled);
		}

        [Test]
        public void OpenBinary_ClearDiagnostics()
        {
            Given_MainFormInteractor();
            form.Stub(f => f.CloseAllDocumentWindows());
            diagnosticSvc.Stub(d => d.AddDiagnostic(null, null)).IgnoreArguments();
            diagnosticSvc.Expect(d => d.ClearDiagnostics());
            repository.ReplayAll();

            When_CreateMainFormInteractor();
            var svc = interactor.ProbeGetService<IDiagnosticsService>();
            svc.AddDiagnostic(new NullCodeLocation(""), new ErrorDiagnostic("test"));
            interactor.OpenBinary(null);
            repository.VerifyAll();
        }

        [Test]
        public void OpenBinary_CloseAllWindows()
        {
            var docWindows = new List<IWindowFrame>();
            Given_MainFormInteractor();
            form.Stub(f => f.DocumentWindows).Return(docWindows);
            form.Expect(f => f.CloseAllDocumentWindows());
            diagnosticSvc.Stub(d => d.ClearDiagnostics());
            repository.ReplayAll();

            When_CreateMainFormInteractor();
            form.DocumentWindows.Add(new TestForm());
            interactor.OpenBinary("");

            repository.VerifyAll();
        }

		[Test]
		public void MainForm_NextPhase_AdvanceToNextInteractor()
		{
            Given_MainFormInteractor();
            diagnosticSvc.Expect(d => d.ClearDiagnostics());
            form.Expect(f => f.CloseAllDocumentWindows());
            repository.ReplayAll();

            When_CreateMainFormInteractor();
			interactor.OpenBinary(null);
			Assert.AreSame(interactor.InitialPageInteractor, interactor.CurrentPhase);
			interactor.NextPhase();
			Assert.AreSame(interactor.LoadedPageInteractor, interactor.CurrentPhase);
            repository.VerifyAll();
		}


        [Test]
        public void MainForm_Save()
        {
            Given_MainFormInteractor();
            repository.ReplayAll();

            When_CreateMainFormInteractor();
            repository.ReplayAll();

            IDecompilerService svc = (IDecompilerService)interactor.ProbeGetService<IDecompilerService>();
            svc.Decompiler = interactor.CreateDecompiler(new FakeLoader());
            svc.Decompiler.LoadProgram("foo.exe");
            Decompiler.Core.Serialization.SerializedProcedure p = new Decompiler.Core.Serialization.SerializedProcedure();
            p.Address = "12345";
            p.Name = "MyProc";
            svc.Decompiler.Project.UserProcedures.Add(new Address(0x12345), p);

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
    <address>00012345</address>
  </procedure>
</project>";
            Assert.AreEqual(s, interactor.ProbeSavedProjectXml);
            repository.VerifyAll();
        }

        [Test]
        public void SaveShouldShowDialog()
        {
            Given_MainFormInteractor();
            repository.ReplayAll();

            When_CreateMainFormInteractor();
            Assert.IsNull(interactor.ProjectFileName);

            IDecompilerService svc = (IDecompilerService)interactor.ProbeGetService<IDecompilerService>();
            svc.Decompiler = interactor.CreateDecompiler(new FakeLoader());
            svc.Decompiler.LoadProgram("foo.exe");

            Assert.IsTrue(string.IsNullOrEmpty(interactor.ProjectFileName), "project filename should be clear");
            interactor.Save();
            Assert.IsTrue(interactor.ProbePromptedForSaving, "Should have prompted for saving as no file name was supplied.");
            Assert.AreEqual("foo.dcproject", interactor.ProbeFilename);
        }

        [Test]
        public void DecompilerServiceInstalled()
        {
            Given_MainFormInteractor();
            repository.ReplayAll();

            When_CreateMainFormInteractor();
            Assert.IsNotNull(interactor.ProbeGetService<IDecompilerService>(), "Should have IDecompilerService available.");
        }

        [Test] 
        public void IsNextPhaseEnabled()
        {
            Given_MainFormInteractor();
            repository.ReplayAll();

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
            Given_MainFormInteractor();
            repository.ReplayAll();
            When_CreateMainFormInteractor();
            Assert.IsNotNull(interactor.ProbeGetService<IWorkerDialogService>());

        }

        [Test]
        public void StatusBarServiceSetText()
        {
            Given_MainFormInteractor();
            repository.ReplayAll();

            When_CreateMainFormInteractor();
            var sbSvc = (IStatusBarService)interactor.ProbeGetService<IStatusBarService>();
            sbSvc.SetText("Hello!");
            Assert.AreEqual("Hello!", form.StatusStrip.Items[0].Text);
        }

        [Test]
        public void MainForm_FindAllProcedures_NoLoadedProgram_QueryStatusDisabled()
        {
            Given_MainFormInteractor();
            Given_UiSvc_ReturnsFalseOnQueryStatus();
            repository.ReplayAll();

            When_CreateMainFormInteractor();
            CommandStatus status;
            status = QueryStatus(CmdIds.ViewFindAllProcedures);
            Assert.AreEqual(MenuStatus.Visible,  status.Status);
        }

        private void Given_UiSvc_ReturnsFalseOnQueryStatus()
        {
            var cmdset = CmdSets.GuidDecompiler;
            uiSvc.Stub(u => u.QueryStatus(ref cmdset, 0, null, null)).IgnoreArguments().Return(false);
        }

        [Test]
        public void MainForm_FindAllProcedures_LoadedProgram_QueryStatusEnabled()
        {
            Given_MainFormInteractor();
            Given_UiSvc_ReturnsFalseOnQueryStatus();
            repository.ReplayAll();

            CreateMainFormInteractorWithLoader();
            IDecompilerService svc = (IDecompilerService)interactor.ProbeGetService<IDecompilerService>();
            svc.Decompiler = interactor.CreateDecompiler(new FakeLoader());
            svc.Decompiler.LoadProgram("foo.exe");
            var status = QueryStatus(CmdIds.ViewFindAllProcedures);
            Assert.AreEqual(MenuStatus.Visible|MenuStatus.Enabled, status.Status);
        }

        [Test]
        public void MainForm_ExecuteFindProcedures()
        {
            Given_MainFormInteractor();

            var srSvc = repository.StrictMock<ISearchResultService>();
            srSvc.Expect(s => s.ShowSearchResults(
                Arg<ISearchResult>.Is.Anything));
            repository.ReplayAll();

            CreateMainFormInteractorWithLoader();
            IDecompilerService svc = (IDecompilerService)interactor.ProbeGetService<IDecompilerService>();
            svc.Decompiler = interactor.CreateDecompiler(new FakeLoader());
            svc.Decompiler.LoadProgram("foo.exe");
            interactor.FindProcedures(srSvc);

            repository.VerifyAll();
        }

        [Test]
        public void MainForm_ViewMemoryWindow()
        {
            Given_MainFormInteractor();
            var disasmSvc = repository.StrictMock<IDisassemblyViewService>();
            svcFactory.Stub(s => s.CreateDisassemblyViewService()).Return(disasmSvc);       //$REVIEW: this shouldn't be necessary -- only if user explicitly asks for it.
            memSvc.Expect(x => x.ShowWindow());
            memSvc.Stub(m => m.SelectionChanged += null).IgnoreArguments();
            memSvc.Expect(m => m.ViewImage(Arg<ProgramImage>.Is.NotNull));
            repository.ReplayAll();

            CreateMainFormInteractorWithLoader();
            interactor.ProbeGetService<IDecompilerService>().Decompiler = new DecompilerDriver(null, null, services)
            {
                Program = new Program
                {
                    Image = new ProgramImage(new Address(0x0004), new byte[0x100])
                }
            };
            interactor.Execute(ref CmdSets.GuidDecompiler, CmdIds.ViewMemory);

            repository.VerifyAll();
        }

        private void ReplaceService<T>(T svcInstance)
        {
            var sc = interactor.ProbeGetService<IServiceContainer>();
            sc.RemoveService(typeof(T));
            sc.AddService(typeof(T), svcInstance);
        }

        [Test]
        public void MainForm_ViewDisassemblyWindow()
        {
            Given_MainFormInteractor();
            disasmSvc.Expect(x => x.ShowWindow());
            repository.ReplayAll();

            CreateMainFormInteractorWithLoader();
            interactor.Execute(ref CmdSets.GuidDecompiler, CmdIds.ViewDisassembly);

            repository.VerifyAll();
        }


        [Test]
        public void MainForm_CloseAllWindows()
        {
            Given_MainFormInteractor();
            var docWindows = new List<IWindowFrame>();
            form.Stub(f => f.DocumentWindows).Return(docWindows);
            form.Expect(f => f.CloseAllDocumentWindows());
            Expect_CommandNotHandledBySubwindow();
            repository.ReplayAll();

            When_CreateMainFormInteractor();
            var mdi = new TestForm();
            form.DocumentWindows.Add(mdi);
            Assert.AreEqual(1, form.DocumentWindows.Count);
            interactor.Execute(ref CmdSets.GuidDecompiler, CmdIds.WindowsCloseAll);

            repository.VerifyAll();
        }

        private void Expect_CommandNotHandledBySubwindow()
        {
            Guid guid = new Guid();
            uiSvc.Stub(u => u.Execute(ref guid, 0))
                .IgnoreArguments()
                .Return(false);
        }

        private class TestForm : Form, IWindowFrame
        {

        }

        private Program CreateFakeProgram()
        {
            Program prog = new Program();
            prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
            prog.Image = new ProgramImage(new Address(0xC00, 0), new byte[300]);
            return prog; 
        }

        private void Given_MainFormInteractor()
        {
            prog = CreateFakeProgram();
            svcFactory = repository.StrictMock<IServiceFactory>();
            dlgFactory = repository.StrictMock<IDialogFactory>();
            uiSvc = repository.StrictMock<IDecompilerShellUiService>();
            memSvc = repository.StrictMock<IMemoryViewService>();
            disasmSvc = repository.StrictMock<IDisassemblyViewService>();
            diagnosticSvc = repository.StrictMock<IDiagnosticsService>();
            typeLibSvc = repository.StrictMock<ITypeLibraryLoaderService>();

            memSvc.Stub(m => m.SelectionChanged += null).IgnoreArguments();

            svcFactory.Stub(s => s.CreateDecompilerConfiguration()).Return(new FakeDecompilerConfiguration());
            svcFactory.Stub(s => s.CreateDiagnosticsService(Arg<ListView>.Is.Anything)).Return(diagnosticSvc);
            svcFactory.Stub(s => s.CreateDecompilerService()).Return(new DecompilerService { }); 
            svcFactory.Stub(s => s.CreateDisassemblyViewService()).Return(disasmSvc);
            svcFactory.Stub(s => s.CreateMemoryViewService()).Return(memSvc);
            svcFactory.Stub(s => s.CreateDecompilerEventListener()).Return(new FakeDecompilerEventListener());
            svcFactory.Stub(s => s.CreateInitialPageInteractor()).Return(new FakeInitialPageInteractor());
            svcFactory.Stub(s => s.CreateLoadedPageInteractor()).Return(new FakeLoadedPageInteractor());
            svcFactory.Stub(s => s.CreateTypeLibraryLoaderService()).Return(typeLibSvc);
            services.AddService(typeof(IDialogFactory), dlgFactory);
            services.AddService(typeof(IServiceFactory), svcFactory);

            form = repository.StrictMock<IMainForm>();
            var listView = new ListView();
            var imagelist = new ImageList();
            var tabPage = new TabPage();
            var tabControl = new TabControl { TabPages = { tabPage } };
            var toolStrip = new ToolStrip { };
            var statusStrip = new StatusStrip { Items = { new ToolStripLabel() } };
            form.Stub(f => f.DiagnosticsList).Return(listView);
            form.Stub(f => f.ImageList).Return(imagelist);
            form.Stub(f => f.Menu).SetPropertyAndIgnoreArgument();
            form.Stub(f => f.AddToolbar(null)).IgnoreArguments();
            form.Stub(f => f.BrowserList).Return(listView);
            form.Stub(f => f.Dispose());
            form.Stub(f => f.TabControl).Return(tabControl);
            form.Stub(f => f.FindResultsPage).Return(tabPage);
            form.Stub(f => f.FindResultsList).Return(listView);
            form.Stub(f => f.ToolBar).Return(toolStrip);
            form.Stub(f => f.StatusStrip).Return(statusStrip);
            form.Load += null;
            LastCall.IgnoreArguments();
            form.Closed += null;
            LastCall.IgnoreArguments();
            dlgFactory.Stub(d => d.CreateMainForm()).Return(form);
        }

        private void When_CreateMainFormInteractor()
        {
            Assert.IsNotNull(dlgFactory, "Make sure you have called SetupMainFormInteractor to set up all mocks and stubs");
            var services = new ServiceContainer();
            services.AddService(typeof(IDialogFactory), dlgFactory);
            services.AddService(typeof(IServiceFactory), svcFactory);
            interactor = new TestMainFormInteractor(prog, services);
            interactor.Test_UiSvc = uiSvc;
            interactor.LoadForm();
            form.Raise(f => f.Load += null, form, EventArgs.Empty);
        }

        private void CreateMainFormInteractorWithLoader()
        {
            Assert.IsNotNull(dlgFactory, "Make sure you have called SetupMainFormInteractor to set up all mocks and stubs");
            var services = new ServiceContainer();
            services.AddService(typeof(IDialogFactory), dlgFactory);
            services.AddService(typeof(IServiceFactory), svcFactory);
            Program prog = new Program();
            interactor = new TestMainFormInteractor(prog, new FakeLoader(), services);
            form = interactor.LoadForm();
        }


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

		public TestMainFormInteractor(Program prog, IServiceProvider sp) : base(sp)
		{
            this.program = prog;
		}

        public TestMainFormInteractor(DecompilerDriver decompiler, IServiceProvider sp)
            : base(sp)
		{
            this.decompiler = decompiler;
		}

        public TestMainFormInteractor(Program prog, LoaderBase ldr, IServiceProvider sp)
            : base(sp)
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

        public IDecompilerShellUiService Test_UiSvc { get; set; }
        protected override IDecompilerShellUiService CreateShellUiService(DecompilerMenus dm)
        {
            if (Test_UiSvc != null)
                return Test_UiSvc;
            else 
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

        public T ProbeGetService<T>()
        {
            return (T)base.GetService(typeof(T));
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
