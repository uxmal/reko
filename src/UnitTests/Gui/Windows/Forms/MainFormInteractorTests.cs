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
using Reko.Core.Configuration;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Gui.Windows;
using Reko.Gui.Windows.Forms;
using Reko.Loading;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Text;
using Reko.UnitTests.Core.Serialization;

namespace Reko.UnitTests.Gui.Windows.Forms
{
	[TestFixture]
    [Category(Categories.UserInterface)]
	public class MainFormInteractorTests
	{
        private MockRepository mr;
        private MockFactory mockFactory;
        private IMainForm form;
		private TestMainFormInteractor interactor;
        private Program program;
        private MemoryStream xmlStm;
        private IArchiveBrowserService archSvc;
        private IDialogFactory dlgFactory;
        private IServiceFactory svcFactory;
        private ServiceContainer services;
        private ILowLevelViewService memSvc;
        private IDisassemblyViewService disasmSvc;
        private IDiagnosticsService diagnosticSvc;
        private IDecompilerShellUiService uiSvc;
        private IConfigurationService configSvc;
        private ITypeLibraryLoaderService typeLibSvc;
        private IProjectBrowserService brSvc;
        private IFileSystemService fsSvc;
        private ILoader loader;
        private IUiPreferencesService uiPrefs;
        private ITabControlHostService tcHostSvc;
        private IDecompilerService dcSvc;
        private ISearchResultService srSvc;
        private IDecompiler decompiler;
        private IResourceEditorService resEditSvc;
        private ICallGraphViewService cgvSvc;
        private IViewImportsService vimpSvc;
        private ISymbolLoadingService symLdrSvc;

        [SetUp]
		public void Setup()
		{
            mr = new MockRepository();
            mockFactory = new MockFactory(mr);
            services = new ServiceContainer();
            configSvc = mr.Stub<IConfigurationService>();
            services.AddService<IConfigurationService>(configSvc);
		}

		[Test]
		public void Mfi_OpenBinary_SwitchToInitialPhase()
		{
            Given_Loader();
            Given_MainFormInteractor();
            diagnosticSvc.Stub(d => d.ClearDiagnostics());
            brSvc.Stub(d => d.Clear());
            Given_DecompilerInstance();
            dcSvc.Stub(d => d.Decompiler = null);
            Given_XmlWriter();
            Given_SavePrompt(true);
            fsSvc.Stub(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Return("foo.exe");
            fsSvc.Stub(f => f.MakeRelativePath(Arg<string>.Is.Equal("foo.dcproject"), Arg<string>.Is.Null)).Return(null);
            mr.ReplayAll();

            When_CreateMainFormInteractor();
			interactor.OpenBinary("floxie.exe");
            Assert.AreSame(interactor.CurrentPhase, interactor.InitialPageInteractor);
            Assert.IsTrue(((FakeInitialPageInteractor)interactor.InitialPageInteractor).OpenBinaryCalled);
		}

        [Test]
        public void Mfi_OpenBinary_ClearDiagnostics()
        {
            Given_Loader();
            Given_MainFormInteractor();
            diagnosticSvc.Stub(d => d.Error(
                Arg<ICodeLocation>.Is.NotNull, 
                Arg<string>.Is.NotNull)).IgnoreArguments();
            diagnosticSvc.Expect(d => d.ClearDiagnostics());
            brSvc.Stub(b => b.Clear());
            Expect_UiPreferences_Loaded();
            Expect_MainForm_SizeSet();
            Given_DecompilerInstance();
            Given_XmlWriter();
            Given_SavePrompt(true);
            dcSvc.Expect(d => d.Decompiler = null);
            fsSvc.Stub(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Return("foo.exe");
            fsSvc.Stub(f => f.MakeRelativePath(Arg<string>.Is.Equal("foo.dcproject"), Arg<string>.Is.Null)).Return(null);
            mr.ReplayAll();

            When_CreateMainFormInteractor();
            diagnosticSvc.Error(new NullCodeLocation(""), "test");
            interactor.OpenBinary(null);

            mr.VerifyAll();
        }

        [Test]
        public void Mfi_OpenBinary_CloseAllWindows()
        {
            var docWindows = new List<IWindowFrame>();
            Given_Loader();
            Given_MainFormInteractor();
            Given_DecompilerInstance();
            dcSvc.Expect(d => d.Decompiler = null);
            Expect_UiPreferences_Loaded();
            Expect_MainForm_SizeSet();
            Given_XmlWriter();
            Given_SavePrompt(true);
            diagnosticSvc.Stub(d => d.ClearDiagnostics());
            fsSvc.Stub(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Return("foo.exe");
            fsSvc.Stub(f => f.MakeRelativePath(Arg<string>.Is.Equal("foo.dcproject"), Arg<string>.Is.Null)).Return(null);
            mr.ReplayAll();

            When_CreateMainFormInteractor();
            interactor.OpenBinary("");

            mr.VerifyAll();
        }

        private void Expect_MainForm_SizeSet()
        {
            form.Expect(f => f.Size = new Size(1000, 700));
            form.Expect(f => f.WindowState = FormWindowState.Normal);
        }

        private void Expect_UiPreferences_Loaded()
        {
            uiPrefs.Expect(u => u.Load());
            uiPrefs.Stub(u => u.WindowState).Return(FormWindowState.Normal);
            uiPrefs.Stub(u => u.WindowSize).Return(new Size(1000, 700));
        }

		[Test]
		public void Mfi_NextPhase_AdvanceToNextInteractor()
		{
            Given_Loader();
            Given_MainFormInteractor();
            diagnosticSvc.Expect(d => d.ClearDiagnostics());
            brSvc.Stub(b => b.Clear());
            Given_LoadPreferences();
            Given_DecompilerInstance();
            Given_XmlWriter();
            Given_SavePrompt(true);
            dcSvc.Stub(d => d.Decompiler = null);
            uiSvc.Stub(u => u.DocumentWindows).Return(new List<IWindowFrame>());
            fsSvc.Stub(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Return("foo.exe");
            fsSvc.Stub(f => f.MakeRelativePath(Arg<string>.Is.Equal("foo.dcproject"), Arg<string>.Is.Null)).Return(null);
            mr.ReplayAll();

            When_CreateMainFormInteractor();
			interactor.OpenBinary(null);
			Assert.AreSame(interactor.InitialPageInteractor, interactor.CurrentPhase);
			interactor.NextPhase();
			Assert.AreSame(interactor.ScannedPageInteractor, interactor.CurrentPhase);

            mr.VerifyAll();
		}

        [Test]
        public void Mfi_FinishDecompilation()
        {
            Given_MainFormInteractor();
            Given_LoadPreferences();
            Given_DecompilerInstance();
            Given_XmlWriter();
            Given_SavePrompt(true);
            dcSvc.Stub(d => d.Decompiler = null);
            fsSvc.Stub(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Return("foo.exe");
            brSvc.Expect(b => b.Reload());
            mr.ReplayAll();

            When_CreateMainFormInteractor();
            interactor.OpenBinary(null);
            Assert.AreSame(interactor.InitialPageInteractor, interactor.CurrentPhase);
            interactor.FinishDecompilation();
            Assert.AreSame(interactor.FinalPageInteractor, interactor.CurrentPhase);

            mr.VerifyAll();
        }

        private void Given_Loader()
        {
            var bytes = new byte[1000];
            var mem = new MemoryArea(Address.SegPtr(0x0C00, 0x0000), bytes);
            loader = mr.StrictMock<ILoader>();
            loader.Stub(l => l.LoadImageBytes(null, 0)).IgnoreArguments()
                .Return(bytes);
            loader.Stub(l => l.LoadExecutable(null, null, null, null)).IgnoreArguments()
                .Return(new Program
                {
                    SegmentMap = new SegmentMap(
                        mem.BaseAddress,
                        new ImageSegment("0C00", mem, AccessMode.ReadWriteExecute)),
                    Platform = mockFactory.CreatePlatform()
                });
        }

        private void Given_XmlWriter()
        {
            this.xmlStm = new MemoryStream();
            var xw = new XmlnsHidingWriter(xmlStm, new UTF8Encoding(false))
            {
                Formatting = Formatting.Indented,
            };
            fsSvc.Stub(f => f.CreateXmlWriter(null)).IgnoreArguments().Return(xw);
        }

        [Test]
        public void Mfi_Save()
        {
            Given_Loader();
            Given_MainFormInteractor();
            Given_LoadPreferences();
            Given_DecompilerInstance();
            Given_XmlWriter();
            fsSvc.Stub(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Return("foo.exe");
            fsSvc.Stub(f => f.MakeRelativePath(Arg<string>.Is.Equal("foo.dcproject"), Arg<string>.Is.Null)).Return(null);
            mr.ReplayAll();

            When_CreateMainFormInteractor();
            Assert.IsNotNull(loader);
            dcSvc.Decompiler.Load("foo.exe");
            var p = new Reko.Core.Serialization.Procedure_v1
            {
                Address = "12345",
                Name = "MyProc",
            };
            var program = dcSvc.Decompiler.Project.Programs[0];
            program.User.Procedures.Add(Address.Ptr32(0x12345), p);
            program.User.Heuristics.Add("shingle");

            interactor.Save();
            string s =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v4"">
  <arch>x86-protected-32</arch>
  <platform>TestPlatform</platform>
  <input>
    <filename>foo.exe</filename>
    <user>
      <processor />
      <procedure name=""MyProc"">
        <address>00012345</address>
      </procedure>
      <heuristic name=""shingle"" />
      <registerValues />
    </user>
  </input>
</project>";
            var res = Encoding.UTF8.GetString(xmlStm.ToArray());
            Assert.AreEqual(s, res);
            mr.VerifyAll();
        }

        [Test]
        public void Mfi_SaveShouldShowDialog()
        {
            Given_MainFormInteractor();
            Given_Loader();
            Given_DecompilerInstance();
            Given_XmlWriter();
            fsSvc.Stub(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Return("foo.exe");
            fsSvc.Stub(f => f.MakeRelativePath(Arg<string>.Is.Equal("foo.dcproject"), Arg<string>.Is.Null)).Return(null);
            mr.ReplayAll();

            When_CreateMainFormInteractor();
            Assert.IsNull(interactor.ProjectFileName);

            Assert.IsTrue(string.IsNullOrEmpty(interactor.ProjectFileName), "project filename should be clear");
            interactor.Save();
            Assert.IsTrue(interactor.Test_PromptedForSaving, "Should have prompted for saving as no file name was supplied.");
            Assert.AreEqual("foo.dcproject", interactor.Test_Filename);
        }

        [Test] 
        public void Mfi_IsNextPhaseEnabled()
        {
            Given_MainFormInteractor();
            Given_UiSvc_IgnoresCommands();
            dcSvc.Stub(d => d.ProjectName).Return("foo.exe");
            mr.ReplayAll();

            When_MainFormInteractorWithLoader();
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
        public void Mfi_StatusBarServiceSetText()
        {
            Given_MainFormInteractor();
            dcSvc.Stub(d => d.ProjectName).Return("foo.exe");
            mr.ReplayAll();

            When_CreateMainFormInteractor();
            var sbSvc = interactor.Services.RequireService<IStatusBarService>();
            sbSvc.SetText("Hello!");
            Assert.AreEqual("Hello!", form.StatusStrip.Items[0].Text);
        }

        [Test]
        public void Mfi_FindAllProcedures_NoLoadedProgram_QueryStatusDisabled()
        {
            Given_Loader();
            Given_MainFormInteractor();
            Given_UiSvc_ReturnsFalseOnQueryStatus();
            Given_NoDecompilerInstance();
            dcSvc.Stub(d => d.ProjectName).Return("foo.exe");
            mr.ReplayAll();

            When_CreateMainFormInteractor();
            CommandStatus status;
            status = QueryStatus(CmdIds.ViewFindAllProcedures);
            Assert.AreEqual(MenuStatus.Visible,  status.Status);
        }

        private void Given_UiSvc_ReturnsFalseOnQueryStatus()
        {
            var cmdset = CmdSets.GuidReko;
            uiSvc.Stub(u => u.QueryStatus(null, null, null)).IgnoreArguments().Return(false);
        }

        [Test]
        public void MainForm_FindAllProcedures_LoadedProgram_QueryStatusEnabled()
        {
            Given_Loader();
            Given_MainFormInteractor();
            Given_UiSvc_ReturnsFalseOnQueryStatus();
            Given_DecompilerInstance();
            mr.ReplayAll();

            When_MainFormInteractorWithLoader();
            var status = QueryStatus(CmdIds.ViewFindAllProcedures);
            Assert.AreEqual(MenuStatus.Visible|MenuStatus.Enabled, status.Status);
        }

        [Test]
        public void Mfi_ExecuteFindProcedures()
        {
            Given_Loader();
            Given_MainFormInteractor();
            var srSvc = mr.StrictMock<ISearchResultService>();
            srSvc.Expect(s => s.ShowSearchResults(
                Arg<ISearchResult>.Is.Anything));
            Given_DecompilerInstance();
            mr.ReplayAll();

            When_MainFormInteractorWithLoader();
            dcSvc.Decompiler.Load("foo.exe");
            interactor.FindProcedures(srSvc);

            mr.VerifyAll();
        }

        private void Given_DecompilerInstance()
        {
            this.decompiler = mr.StrictMock<IDecompiler>();
            // Having a compiler presupposes having a project.
            var platform = mockFactory.CreatePlatform();
            var mem = new MemoryArea(Address.Ptr32(0x00010000), new byte[100]);
            var project = new Project
            {
                Programs = { new Program
                {
                    Filename="foo.exe" ,
                    SegmentMap = new SegmentMap(
                        mem.BaseAddress,
                        new ImageSegment(".text", mem, AccessMode.ReadExecute)),
                    Platform = platform,
                    Architecture = platform.Architecture,
                }
                }
            };
            
            dcSvc.Stub(d => d.Decompiler).Return(decompiler);
            dcSvc.Stub(d => d.ProjectName).Return("foo.exe");
            decompiler.Stub(d => d.Project).Return(project);
            decompiler.Stub(d => d.Load(Arg<string>.Is.NotNull, Arg<string>.Is.Null)).Return(false);
        }

        private void Given_NoDecompilerInstance()
        {
            dcSvc.Stub(d => d.Decompiler).Return(null);
        }

        private void ReplaceService<T>(T svcInstance)
        {
            var sc = interactor.Services.RequireService<IServiceContainer>();
            sc.RemoveService(typeof(T));
            sc.AddService(typeof(T), svcInstance);
        }

        private void Given_UiSvc_IgnoresCommands()
        {
            uiSvc.Stub(u => u.QueryStatus(
                Arg<CommandID>.Is.NotNull,
                Arg<CommandStatus>.Is.NotNull,
                Arg<CommandText>.Is.Anything)).Return(false);
            uiSvc.Stub(u => u.Execute(
                Arg<CommandID>.Is.NotNull)).Return(false);
        }

        [Test]
        public void MainForm_CloseAllWindows()
        {
            Given_Loader();
            Given_MainFormInteractor();
            var docWindows = new List<IWindowFrame>();
            uiSvc.Stub(u => u.DocumentWindows).Return(docWindows);
            dcSvc.Stub(d => d.ProjectName).Return("foo.exe");
            //form.Expect(f => f.CloseAllDocumentWindows());
            Given_LoadPreferences();
            Given_CommandNotHandledBySubwindow();
            mr.ReplayAll();

            When_CreateMainFormInteractor();
            var mdi = new TestForm();
            //form.DocumentWindows.Add(mdi);
            //Assert.AreEqual(1, form.DocumentWindows.Count);
            interactor.Execute(new CommandID(CmdSets.GuidReko, CmdIds.WindowsCloseAll));

            mr.VerifyAll();
        }

        private void Given_LoadPreferences()
        {
            uiPrefs.Stub(u => u.Load());
            uiPrefs.Stub(u => u.WindowSize).Return(new Size());
            uiPrefs.Stub(u => u.WindowState).Return(FormWindowState.Normal);
            form.Stub(f => f.WindowState = FormWindowState.Normal);
        }

        private void Given_SavePrompt(bool answer)
        {
            uiSvc.Stub(u => u.Prompt(Arg<string>.Is.NotNull)).Return(answer);
        }

        private void Given_CommandNotHandledBySubwindow()
        {
            uiSvc.Stub(u => u.Execute(null))
                .IgnoreArguments()
                .Return(false);
        }

        [Test]
        public void Mfi_AddMetadata()
        {
            Given_Loader();
            Given_MainFormInteractor();
            Given_DecompilerInstance();
            loader.Expect(d => d.LoadMetadata(
                Arg<string>.Is.Equal("foo.def"),
                Arg<IPlatform>.Is.NotNull,
                Arg<TypeLibrary>.Is.NotNull))
                    .Return(new TypeLibrary());
            services.AddService(typeof(IDecompilerService), dcSvc);
            uiSvc.Expect(u => u.ShowOpenFileDialog(null)).IgnoreArguments().Return("foo.def");
            Given_CommandNotHandledBySubwindow();
            this.tcHostSvc.Stub(t => t.Execute(null)).IgnoreArguments().Return(false);
            mr.ReplayAll();

            When_MainFormInteractorWithLoader();
            interactor.Execute(new CommandID(CmdSets.GuidReko, CmdIds.FileAddMetadata));
            
            mr.VerifyAll();
        }

        [Test]
        public void Mfi_CloseProject()
        {
            Given_Loader();
            Given_MainFormInteractor();
            Given_LoadPreferences();
            Given_CommandNotHandledBySubwindow();
            uiSvc.Stub(u => u.DocumentWindows).Return(new List<IWindowFrame>());
            //form.Expect(f => f.CloseAllDocumentWindows());
            diagnosticSvc.Expect(d => d.ClearDiagnostics());
            Given_DecompilerInstance();
            Given_SavePrompt(true);
            Given_XmlWriter();
            dcSvc.Expect(d => d.Decompiler = Arg<IDecompiler>.Is.Anything);
            fsSvc.Stub(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Return("foo.exe");
            fsSvc.Stub(f => f.MakeRelativePath(Arg<string>.Is.Anything, Arg<string>.Is.Null)).Return(null);

            mr.ReplayAll();

            When_CreateMainFormInteractor();
            Assert.IsNotNull(dcSvc.Decompiler.Project);
            var executed = interactor.Execute(new CommandID(CmdSets.GuidReko, CmdIds.FileCloseProject));

            Assert.IsTrue(executed);
            mr.VerifyAll();
        }

        private class TestForm : Form, IWindowFrame
        {
            public IWindowPane Pane { get; private set; }
            public string Title { get { return Text; } set { Text = value; } }
        }

        private Program CreateFakeProgram()
        {
            Program prog = new Program();
            prog.Architecture = new X86ArchitectureReal("x86-real-16");
            var mem = new MemoryArea(Address.SegPtr(0xC00, 0), new byte[300]);
            prog.SegmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment("0C00", mem, AccessMode.ReadWriteExecute));
            return prog; 
        }

        private void Given_MainFormInteractor()
        {
            program = CreateFakeProgram();
            svcFactory = mr.StrictMock<IServiceFactory>();
            archSvc = mr.StrictMock<IArchiveBrowserService>();
            dlgFactory = mr.StrictMock<IDialogFactory>();
            uiSvc = mr.StrictMock<IDecompilerShellUiService>();
            memSvc = mr.StrictMock<ILowLevelViewService>();
            disasmSvc = mr.StrictMock<IDisassemblyViewService>();
            typeLibSvc = mr.StrictMock<ITypeLibraryLoaderService>();
            brSvc = mr.Stub<IProjectBrowserService>();
            uiPrefs = mr.StrictMock<IUiPreferencesService>();
            fsSvc = mr.StrictMock<IFileSystemService>();
            tcHostSvc = mr.StrictMock<ITabControlHostService>();
            dcSvc = mr.StrictMock<IDecompilerService>();
            srSvc = MockRepository.GenerateMock<ISearchResultService, IWindowPane>();
            diagnosticSvc = MockRepository.GenerateMock<IDiagnosticsService, IWindowPane>();
            resEditSvc = mr.StrictMock<IResourceEditorService>();
            cgvSvc = mr.StrictMock<ICallGraphViewService>();
            loader = mr.StrictMock<ILoader>();
            vimpSvc = mr.StrictMock<IViewImportsService>();
            symLdrSvc = mr.StrictMock<ISymbolLoadingService>();

            svcFactory.Stub(s => s.CreateArchiveBrowserService()).Return(archSvc);
            svcFactory.Stub(s => s.CreateDecompilerConfiguration()).Return(new FakeDecompilerConfiguration());
            svcFactory.Stub(s => s.CreateDiagnosticsService(Arg<ListView>.Is.Anything)).Return(diagnosticSvc);
            svcFactory.Stub(s => s.CreateDecompilerService()).Return(dcSvc); 
            svcFactory.Stub(s => s.CreateDisassemblyViewService()).Return(disasmSvc);
            svcFactory.Stub(s => s.CreateMemoryViewService()).Return(memSvc);
            svcFactory.Stub(s => s.CreateDecompilerEventListener()).Return(new FakeDecompilerEventListener());
            svcFactory.Stub(s => s.CreateInitialPageInteractor()).Return(new FakeInitialPageInteractor());
            svcFactory.Stub(s => s.CreateScannedPageInteractor()).Return(new FakeScannedPageInteractor());
            svcFactory.Stub(s => s.CreateAnalyzedPageInteractor()).Return(new FakeAnalyzedPageInteractor());
            svcFactory.Stub(s => s.CreateFinalPageInteractor()).Return(new FakeFinalPageInteractor());
            svcFactory.Stub(s => s.CreateTypeLibraryLoaderService()).Return(typeLibSvc);
            svcFactory.Stub(s => s.CreateProjectBrowserService(Arg<ITreeView>.Is.NotNull)).Return(brSvc);
            svcFactory.Stub(s => s.CreateUiPreferencesService()).Return(uiPrefs);
            svcFactory.Stub(s => s.CreateFileSystemService()).Return(fsSvc);
            svcFactory.Stub(s => s.CreateShellUiService(Arg<IMainForm>.Is.NotNull,Arg<DecompilerMenus>.Is.NotNull)).Return(uiSvc);
            svcFactory.Stub(s => s.CreateTabControlHost(Arg<TabControl>.Is.NotNull)).Return(tcHostSvc);
            svcFactory.Stub(s => s.CreateLoader()).Return(loader);
            svcFactory.Stub(s => s.CreateSearchResultService(Arg<ListView>.Is.NotNull)).Return(srSvc);
            svcFactory.Stub(s => s.CreateResourceEditorService()).Return(resEditSvc);
            svcFactory.Stub(s => s.CreateCallGraphViewService()).Return(cgvSvc);
            svcFactory.Stub(s => s.CreateViewImportService()).Return(vimpSvc);
            svcFactory.Stub(s => s.CreateSymbolLoadingService()).Return(symLdrSvc);
            services.AddService(typeof(IDialogFactory), dlgFactory);
            services.AddService(typeof(IServiceFactory), svcFactory);
            brSvc.Stub(b => b.Clear());

            form = mr.StrictMock<IMainForm>();
            var listView = new ListView();
            var imagelist = new ImageList();
            var tabResults = new TabPage();
            var tabDiagnostics = new TabPage();
            var tabControl = new TabControl { TabPages = { tabResults, tabDiagnostics } };
            var toolStrip = new ToolStrip { };
            var statusStrip = new StatusStrip { Items = { new ToolStripLabel() } };
            var brToolbar = new ToolStrip();
            var projectBrowser = mr.Stub<ITreeView>();
            form.Stub(f => f.DiagnosticsList).Return(listView);
            form.Stub(f => f.ImageList).Return(imagelist);
            form.Stub(f => f.Menu).SetPropertyAndIgnoreArgument();
            form.Stub(f => f.AddToolbar(null)).IgnoreArguments();
            form.Stub(f => f.AddProjectBrowserToolbar(null)).IgnoreArguments();
            form.Stub(f => f.Dispose());
            form.Stub(f => f.TabControl).Return(tabControl);
            form.Stub(f => f.FindResultsPage).Return(tabResults);
            form.Stub(f => f.DiagnosticsPage).Return(tabDiagnostics);
            form.Stub(f => f.FindResultsList).Return(listView);
            form.Stub(f => f.ToolBar).Return(toolStrip);
            form.Stub(f => f.ProjectBrowserToolbar).Return(toolStrip);
            form.Stub(f => f.ProjectBrowser).Return(projectBrowser);
            form.Stub(f => f.StatusStrip).Return(statusStrip);
            form.Stub(f => f.AddProjectBrowserToolbar(null)).IgnoreArguments();
            form.Stub(f => f.ProjectBrowserToolbar).Return(brToolbar);
            form.Stub(f => f.TitleText = "main.exe ").IgnoreArguments();
            form.Load += null;
            LastCall.IgnoreArguments();
            form.Closed += null;
            LastCall.IgnoreArguments();
            form.ProcessCommandKey += null;
            LastCall.IgnoreArguments();
            dlgFactory.Stub(d => d.CreateMainForm()).Return(form);
            tcHostSvc.Stub(t => t.Attach(Arg<IWindowPane>.Is.NotNull, Arg<TabPage>.Is.NotNull));
            tcHostSvc.Stub(t => t.QueryStatus(
                Arg<CommandID>.Is.Anything,
                Arg<CommandStatus>.Is.Anything,
                Arg<CommandText>.Is.Anything)).Return(false);
            tcHostSvc.Stub(t => t.Execute(Arg<CommandID>.Is.Anything)).Return(false);

            uiSvc.Stub(u => u.DocumentWindows).Return(new List<IWindowFrame>());
        }

        private void When_CreateMainFormInteractor()
        {
            Assert.IsNotNull(dlgFactory, "Make sure you have called SetupMainFormInteractor to set up all mocks and stubs");
            var services = new ServiceContainer();
            services.AddService(typeof(IDialogFactory), dlgFactory);
            services.AddService(typeof(IServiceFactory), svcFactory);
            interactor = new TestMainFormInteractor(program, services);
            interactor.LoadForm();
            form.Raise(f => f.Load += null, form, EventArgs.Empty);
        }

        private void When_MainFormInteractorWithLoader()
        {
            Assert.IsNotNull(dlgFactory, "Make sure you have called SetupMainFormInteractor to set up all mocks and stubs");
            var services = new ServiceContainer();
            services.AddService(typeof(IDialogFactory), dlgFactory);
            services.AddService(typeof(IServiceFactory), svcFactory);
            Program prog = new Program();
            interactor = new TestMainFormInteractor(prog, loader, services);
            form = interactor.LoadForm();
        }

        private CommandStatus QueryStatus(int cmdId)
        {
            CommandStatus status = new CommandStatus();
            if (interactor.QueryStatus(new CommandID(CmdSets.GuidReko, cmdId), status, null))
                return status;
            else
                return null;
        }
	}

	public class TestMainFormInteractor : MainFormInteractor
	{
		private DecompilerDriver decompiler;
        private ILoader ldr;
		private Program program;
        private StringWriter sw;
        private XmlTextWriter xw;
        private MemoryStream xmlStm;
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

        public TestMainFormInteractor(Program prog, ILoader ldr, IServiceProvider sp)
            : base(sp)
        {
            this.program = prog;
            this.ldr = ldr;
        }

        // Overrides of creation methods.

        public override IDecompiler CreateDecompiler(ILoader ldr)
		{
            if (decompiler != null)
                return decompiler;
            return base.CreateDecompiler(ldr);
		}

        public override TextWriter CreateTextWriter(string filename)
        {
            testFilename = filename;
            sw = new StringWriter();
            return sw;
        }

        public override XmlWriter CreateXmlWriter(string filename)
        {
            testFilename = filename;
            xmlStm = new MemoryStream();
            xw = new XmlnsHidingWriter(xmlStm, new UTF8Encoding(false));
            xw.Formatting = Formatting.Indented;
            return xw;
        }

        protected override string PromptForFilename(string suggestedName)
        {
            promptedForSaving = true;
            testFilename = suggestedName;
            return suggestedName;
        }

        public string Test_SavedProjectXml
        {
            get { return Encoding.UTF8.GetString(xmlStm.ToArray()); }
        }

        public string Test_Filename
        {
            get { return testFilename; }
        }

        public bool Test_PromptedForSaving
        {
            get { return promptedForSaving; }
        }
    }
}
