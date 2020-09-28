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
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UnitTests.Gui.Forms;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class MainFormInteractorTests
    {
        private CommonMockFactory mockFactory;
        private Mock<IMainForm> form;
        private MainFormInteractor interactor;
        private MemoryStream xmlStm;
        private ServiceContainer services;
        private Mock<IArchiveBrowserService> archSvc;
        private Mock<IDialogFactory> dlgFactory;
        private Mock<IServiceFactory> svcFactory;
        private Mock<ILowLevelViewService> memSvc;
        private Mock<IDisassemblyViewService> disasmSvc;
        private Mock<IDiagnosticsService> diagnosticSvc;
        private Mock<IDecompilerShellUiService> uiSvc;
        private Mock<IConfigurationService> configSvc;
        private Mock<ITypeLibraryLoaderService> typeLibSvc;
        private Mock<IProjectBrowserService> brSvc;
        private Mock<IProcedureListService> procSvc;
        private Mock<IFileSystemService> fsSvc;
        private Mock<ILoader> loader;
        private Mock<IUiPreferencesService> uiPrefs;
        private Mock<ITabControlHostService> tcHostSvc;
        private Mock<IDecompilerService> dcSvc;
        private Mock<ISearchResultService> srSvc;
        private Mock<IDecompiler> decompiler;
        private Mock<IResourceEditorService> resEditSvc;
        private Mock<ICallGraphViewService> cgvSvc;
        private Mock<IStatusBarService> sbSvc;
        private Mock<IViewImportsService> vimpSvc;
        private Mock<ICodeViewerService> cvSvc;
        private Mock<ImageSegmentService> imgSegSvc;
        private Mock<ISymbolLoadingService> symLoadSvc;
        private Mock<ISelectionService> selSvc;
        private Mock<ICallHierarchyService> callHierSvc;
        private Mock<IDecompiledFileService> dcFileSvc;

        [SetUp]
        public void Setup()
        {
            mockFactory = new CommonMockFactory();
            services = new ServiceContainer();
            configSvc = new Mock<IConfigurationService>();
            uiSvc = new Mock<IDecompilerShellUiService>();
            services.AddService(uiSvc.Object);
        }

        [Test]
        public void Mfi_OpenBinary_SwitchToInitialPhase()
        {
            Given_Loader();
            Given_MainFormInteractor();
            diagnosticSvc.Setup(d => d.ClearDiagnostics());
            brSvc.Setup(d => d.Clear());
            Given_DecompilerInstance();
            Given_LoadPreferences();
            Given_XmlWriter();
            Given_SavePrompt(true);
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Returns("foo.exe");
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", null)).Returns((string)null);
            uiSvc.Setup(u => u.ShowSaveFileDialog("foo.dcproject")).Returns("foo.dcproject");
            sbSvc.Setup(s => s.SetText("")).Verifiable();

            When_CreateMainFormInteractor();
            interactor.OpenBinary("floxie.exe");

            sbSvc.Verify();
            Assert.AreSame(interactor.CurrentPhase, interactor.InitialPageInteractor);
            Assert.IsTrue(((FakeInitialPageInteractor)interactor.InitialPageInteractor).OpenBinaryCalled);
        }

        [Test]
        public void Mfi_OpenBinary_ClearDiagnostics()
        {
            Given_Loader();
            Given_MainFormInteractor();
            diagnosticSvc.Setup(d => d.Error(
                It.IsNotNull<ICodeLocation>(),
                It.IsNotNull<string>()));
            diagnosticSvc.Setup(d => d.ClearDiagnostics()).Verifiable();
            brSvc.Setup(b => b.Clear());
            Expect_UiPreferences_Loaded();
            Expect_MainForm_SizeSet();
            Given_DecompilerInstance();
            Given_XmlWriter();
            Given_SavePrompt(true);
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Returns("foo.exe");
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", It.IsNotNull<string>())).Returns((string)null);
            uiSvc.Setup(u => u.ShowSaveFileDialog("foo.dcproject")).Returns("foo.dcproject");

            When_CreateMainFormInteractor();
            diagnosticSvc.Object.Error(new NullCodeLocation(""), "test");
            interactor.OpenBinary(null);

            uiSvc.Verify();
            diagnosticSvc.Verify();
        }

        [Test]
        public void Mfi_OpenBinary_CloseAllWindows()
        {
            var docWindows = new List<IWindowFrame>();
            Given_Loader();
            Given_MainFormInteractor();
            Given_DecompilerInstance();
            Expect_UiPreferences_Loaded();
            Expect_MainForm_SizeSet();
            Given_XmlWriter();
            Given_SavePrompt(true);
            diagnosticSvc.Setup(d => d.ClearDiagnostics());
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Returns("foo.exe");
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", null)).Returns((string)null);
            uiSvc.Setup(u => u.ShowSaveFileDialog("foo.dcproject")).Returns("foo.dcproject");

            When_CreateMainFormInteractor();
            interactor.OpenBinary("");

            uiSvc.Verify();
            form.Verify();
        }

        private void Expect_MainForm_SizeSet()
        {
            form.SetupSet(f => f.Size = new Size(1000, 700)).Verifiable();
            form.SetupSet(f => f.WindowState = FormWindowState.Normal).Verifiable();
        }

        private void Expect_UiPreferences_Loaded()
        {
            uiPrefs.Setup(u => u.Load());
            uiPrefs.Setup(u => u.WindowState).Returns(FormWindowState.Normal);
            uiPrefs.Setup(u => u.WindowSize).Returns(new Size(1000, 700));
        }

        [Test]
        public void Mfi_NextPhase_AdvanceToNextInteractor()
        {
            Given_Loader();
            Given_MainFormInteractor();
            diagnosticSvc.Setup(d => d.ClearDiagnostics()).Verifiable();
            brSvc.Setup(b => b.Clear());
            Given_LoadPreferences();
            Given_DecompilerInstance();
            Given_XmlWriter();
            Given_SavePrompt(true);
            //dcSvc.Setup(d => d.Decompiler = null);
            uiSvc.Setup(u => u.DocumentWindows).Returns(new List<IWindowFrame>());
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Returns("foo.exe");
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", null)).Returns((string)null);
            uiSvc.Setup(u => u.ShowSaveFileDialog("foo.dcproject")).Returns("foo.dcproject");

            When_CreateMainFormInteractor();
            interactor.OpenBinary(null);
            Assert.AreSame(interactor.InitialPageInteractor, interactor.CurrentPhase);
            interactor.NextPhase();
            Assert.AreSame(interactor.ScannedPageInteractor, interactor.CurrentPhase);

            form.Verify();
            diagnosticSvc.Verify();
        }

        [Test]
        public void Mfi_FinishDecompilation()
        {
            Given_MainFormInteractor();
            Given_LoadPreferences();
            Given_DecompilerInstance();
            Given_XmlWriter();
            Given_SavePrompt(false);
            //dcSvc.Setup(d => d.Decompiler = null).IgnoreArguments();
            decompiler.Setup(d => d.AnalyzeDataFlow());
            decompiler.Setup(d => d.ReconstructTypes());
            decompiler.Setup(d => d.StructureProgram());
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Returns("foo.exe");
            brSvc.Setup(b => b.Reload())
                .Verifiable();

            When_CreateMainFormInteractor();
            interactor.OpenBinary("foo.exe");

            Assert.AreSame(interactor.InitialPageInteractor, interactor.CurrentPhase);
            interactor.FinishDecompilation();
            Assert.AreSame(interactor.FinalPageInteractor, interactor.CurrentPhase);
            brSvc.Verify();
        }

        private void Given_Loader()
        {
            var bytes = new byte[1000];
            var mem = new MemoryArea(Address.SegPtr(0x0C00, 0x0000), bytes);
            loader = new Mock<ILoader>();
            loader.Setup(l => l.LoadImageBytes(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(bytes);
            loader.Setup(l => l.LoadExecutable(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<Address>()))
                .Returns(new Program
                {
                    SegmentMap = new SegmentMap(
                        mem.BaseAddress,
                        new ImageSegment("0C00", mem, AccessMode.ReadWriteExecute)),
                    Platform = mockFactory.CreateMockPlatform().Object
                });
        }

        private void Given_XmlWriter()
        {
            this.xmlStm = new MemoryStream();
            var xw = new XmlnsHidingWriter(xmlStm, new UTF8Encoding(false))
            {
                Formatting = Formatting.Indented,
            };
            fsSvc.Setup(f => f.CreateXmlWriter(It.IsAny<string>())).Returns(xw);
        }

        [Test]
        public void Mfi_Save()
        {
            Given_Loader();
            Given_MainFormInteractor();
            Given_LoadPreferences();
            Given_DecompilerInstance();
            Given_XmlWriter();
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Returns("foo.exe");
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", null)).Returns((string)null);
            uiSvc.Setup(u => u.ShowSaveFileDialog("foo.dcproject")).Returns("foo.dcproject").Verifiable();

            When_CreateMainFormInteractor();
            Assert.IsNotNull(loader);
            dcSvc.Object.Decompiler.Load("foo.exe");
            var p = new Reko.Core.Serialization.Procedure_v1
            {
                Address = "12345",
                Name = "MyProc",
            };
            var program = dcSvc.Object.Decompiler.Project.Programs[0];
            program.User.Procedures.Add(Address.Ptr32(0x12345), p);
            program.User.Heuristics.Add("shingle");
            program.User.ExtractResources = true;

            interactor.Save();
            string s =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v5"">
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
            uiSvc.Verify();
        }

        [Test]
        public void Mfi_SaveShouldShowDialog()
        {
            Given_MainFormInteractor();
            Given_Loader();
            Given_DecompilerInstance();
            Given_XmlWriter();
            Given_LoadPreferences();
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Returns("foo.exe");
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", null)).Returns((string)null);
            uiSvc.Setup(u => u.ShowSaveFileDialog("foo.dcproject"))
                .Returns("foo.dcproject")
                .Verifiable();

            When_CreateMainFormInteractor();
            Assert.IsNull(interactor.ProjectFileName);

            Assert.IsTrue(string.IsNullOrEmpty(interactor.ProjectFileName), "project filename should be clear");
            interactor.Save();

            uiSvc.Verify();
        }

        [Test]
        public void Mfi_IsNextPhaseEnabled()
        {
            Given_MainFormInteractor();
            Given_UiSvc_IgnoresCommands();
            dcSvc.Setup(d => d.ProjectName).Returns("foo.exe");

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
            Assert.AreEqual(MenuStatus.Visible | MenuStatus.Enabled, status.Status);
        }

#if NO_STATUS_STRIP_YET
        // During the reorganization of the GUI front ends,
        // we have lost access to the status strip. Recall that it is now
        // to be accessses via a statusbarservice which isn't yet 
        // implemented.
        [Test]
        public void Mfi_StatusBarServiceSetText() //$REVIEW: this was removed in the merge, why?
        {
            Given_MainFormInteractor();
            dcSvc.Setup(d => d.ProjectName).Returns("foo.exe");
            mr.ReplayAll();

            When_CreateMainFormInteractor();
            var sbSvc = interactor.Services.RequireService<IStatusBarService>();
            sbSvc.SetText("Hello!");
            Assert.AreEqual("Hello!", form.StatusStrip.Items[0].Text);
        }
#endif

        [Test]
        public void Mfi_FindAllProcedures_NoLoadedProgram_QueryStatusDisabled()
        {
            Given_Loader();
            Given_MainFormInteractor();
            Given_UiSvc_ReturnsFalseOnQueryStatus();
            Given_NoDecompilerInstance();
            dcSvc.Setup(d => d.ProjectName).Returns("foo.exe");

            When_CreateMainFormInteractor();
            CommandStatus status;
            status = QueryStatus(CmdIds.ViewFindAllProcedures);
            Assert.AreEqual(MenuStatus.Visible, status.Status);
        }

        private void Given_UiSvc_ReturnsFalseOnQueryStatus()
        {
            var cmdset = CmdSets.GuidReko;
            uiSvc.Setup(u => u.QueryStatus(
                It.IsAny<CommandID>(),
                It.IsAny<CommandStatus>(),
                It.IsAny<CommandText>()))
                .Returns(false);
        }

        [Test]
        public void MainForm_FindAllProcedures_LoadedProgram_QueryStatusEnabled()
        {
            Given_Loader();
            Given_MainFormInteractor();
            Given_UiSvc_ReturnsFalseOnQueryStatus();
            Given_DecompilerInstance();

            When_MainFormInteractorWithLoader();
            var status = QueryStatus(CmdIds.ViewFindAllProcedures);
            Assert.AreEqual(MenuStatus.Visible | MenuStatus.Enabled, status.Status);
        }

        [Test]
        public void Mfi_ExecuteFindProcedures()
        {
            Given_Loader();
            Given_MainFormInteractor();
            Expect_RestoreWindowSize();
            var srSvc = new Mock<ISearchResultService>();
            srSvc.Setup(s => s.ShowSearchResults(
                It.IsAny<ISearchResult>()))
                .Verifiable();
            Given_DecompilerInstance();

            When_MainFormInteractorWithLoader();
            dcSvc.Object.Decompiler.Load("foo.exe");
            interactor.FindProcedures(srSvc.Object);

            srSvc.Verify();
            form.Verify();
        }

        private void Given_DecompilerInstance()
        {
            this.decompiler = new Mock<IDecompiler>();
            // Having a compiler presupposes having a project.
            var platform = mockFactory.CreateMockPlatform().Object;
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

            dcSvc.Setup(d => d.Decompiler).Returns(decompiler.Object);
            dcSvc.Setup(d => d.ProjectName).Returns("foo.exe");
            decompiler.Setup(d => d.Project).Returns(project);
            decompiler.Setup(d => d.Load(It.IsNotNull<string>(), null)).Returns(false);
        }

        private void Given_NoDecompilerInstance()
        {
            dcSvc.Setup(d => d.Decompiler).Returns((IDecompiler)null);
        }

        private void ReplaceService<T>(T svcInstance)
        {
            var sc = interactor.Services.RequireService<IServiceContainer>();
            sc.RemoveService(typeof(T));
            sc.AddService(typeof(T), svcInstance);
        }

        private void Given_UiSvc_IgnoresCommands()
        {
            uiSvc.Setup(u => u.QueryStatus(
                It.IsNotNull<CommandID>(),
                It.IsNotNull<CommandStatus>(),
                It.IsAny<CommandText>())).Returns(false);
            uiSvc.Setup(u => u.Execute(
                It.IsNotNull<CommandID>())).Returns(false);
        }

        [Test]
        public void MainForm_CloseAllWindows()
        {
            Given_Loader();
            Given_MainFormInteractor();
            var docWindows = new List<IWindowFrame>();
            uiSvc.Setup(u => u.DocumentWindows).Returns(docWindows);
            dcSvc.Setup(d => d.ProjectName).Returns("foo.exe");
            //form.Expect(f => f.CloseAllDocumentWindows());
            Given_LoadPreferences();
            Given_CommandNotHandledBySubwindow();

            When_CreateMainFormInteractor();
            var mdi = new TestForm();
            //form.DocumentWindows.Add(mdi);
            //Assert.AreEqual(1, form.DocumentWindows.Count);
            interactor.Execute(new CommandID(CmdSets.GuidReko, CmdIds.WindowsCloseAll));

            uiSvc.Verify();
        }

        private void Given_LoadPreferences()
        {
            uiPrefs.Setup(u => u.Load());
            uiPrefs.Setup(u => u.WindowSize).Returns(new Size());
            uiPrefs.Setup(u => u.WindowState).Returns(FormWindowState.Normal);
            //form.Setup(f => f.WindowState = FormWindowState.Normal);
        }

        private void Given_SavePrompt(bool answer)
        {
            uiSvc.Setup(u => u.Prompt(It.IsNotNull<string>())).Returns(answer);
        }

        private void Given_CommandNotHandledBySubwindow()
        {
            uiSvc.Setup(u => u.Execute(It.IsAny<CommandID>()))
                .Returns(false);
        }

        [Test]
        public void Mfi_AddMetadata()
        {
            Given_Loader();
            Given_MainFormInteractor();
            Given_DecompilerInstance();
            Expect_RestoreWindowSize();

            loader.Setup(d => d.LoadMetadata(
                "foo.def",
                It.IsNotNull<IPlatform>(),
                It.IsNotNull<TypeLibrary>()))
                    .Returns(new TypeLibrary())
                    .Verifiable();
            services.AddService<IDecompilerService>(dcSvc.Object);
            uiSvc.Setup(u => u.ShowOpenFileDialog(null))
                .Returns("foo.def")
                .Verifiable();
            Given_CommandNotHandledBySubwindow();
            this.tcHostSvc.Setup(t => t.Execute(It.IsAny<CommandID>())).Returns(false);

            When_MainFormInteractorWithLoader();
            interactor.Execute(new CommandID(CmdSets.GuidReko, CmdIds.FileAddMetadata));

            loader.Verify();
            uiSvc.Verify();
            form.Verify();
        }

        [Test]
        public void Mfi_CloseProject()
        {
            Given_Loader();
            Given_MainFormInteractor();
            Given_LoadPreferences();
            Given_CommandNotHandledBySubwindow();
            uiSvc.Setup(u => u.DocumentWindows).Returns(new List<IWindowFrame>());
            //form.Expect(f => f.CloseAllDocumentWindows());
            diagnosticSvc.Setup(d => d.ClearDiagnostics()).Verifiable();
            Given_DecompilerInstance();
            Given_SavePrompt(true);
            Given_XmlWriter();
            //dcSvc.Expect(d => d.Decompiler = Arg<IDecompiler>.Is.Anything);
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Returns("foo.exe");
            fsSvc.Setup(f => f.MakeRelativePath(It.IsAny<string>(), null)).Returns((string)null);
            uiSvc.Setup(u => u.ShowSaveFileDialog("foo.dcproject")).Returns("foo.dcproject");

            When_CreateMainFormInteractor();
            Assert.IsNotNull(dcSvc.Object.Decompiler.Project);
            var executed = interactor.Execute(new CommandID(CmdSets.GuidReko, CmdIds.FileCloseProject));

            Assert.IsTrue(executed);
            diagnosticSvc.Verify();
        }

        private class TestForm : System.Windows.Forms.Form, IWindowFrame
        {
            public IWindowPane Pane { get; private set; }
            public string Title { get { return Text; } set { Text = value; } }
        }

        private void Given_MainFormInteractor()
        {
            svcFactory = new Mock<IServiceFactory>();
            archSvc = new Mock<IArchiveBrowserService>();
            dlgFactory = new Mock<IDialogFactory>();
            memSvc = new Mock<ILowLevelViewService>();
            disasmSvc = new Mock<IDisassemblyViewService>();
            typeLibSvc = new Mock<ITypeLibraryLoaderService>();
            brSvc = new Mock<IProjectBrowserService>();
            procSvc = new Mock<IProcedureListService>();
            uiPrefs = new Mock<IUiPreferencesService>();
            fsSvc = new Mock<IFileSystemService>();
            tcHostSvc = new Mock<ITabControlHostService>();
            dcSvc = new Mock<IDecompilerService>();
            srSvc = new Mock<ISearchResultService>();
            srSvc.As<IWindowPane>();
            diagnosticSvc = new Mock<IDiagnosticsService>();
            diagnosticSvc.As<IWindowPane>();
            resEditSvc = new Mock<IResourceEditorService>();
            cgvSvc = new Mock<ICallGraphViewService>();
            loader = new Mock<ILoader>();
            sbSvc = new Mock<IStatusBarService>();
            vimpSvc = new Mock<IViewImportsService>();
            cvSvc = new Mock<ICodeViewerService>();
            imgSegSvc = new Mock<ImageSegmentService>();
            symLoadSvc = new Mock<ISymbolLoadingService>();
            selSvc = new Mock<ISelectionService>();
            callHierSvc = new Mock<ICallHierarchyService>();
            dcFileSvc = new Mock<IDecompiledFileService>();

            svcFactory.Setup(s => s.CreateArchiveBrowserService()).Returns(archSvc.Object);
            svcFactory.Setup(s => s.CreateCodeViewerService()).Returns(cvSvc.Object);
            svcFactory.Setup(s => s.CreateDecompilerConfiguration()).Returns(new FakeDecompilerConfiguration());
            svcFactory.Setup(s => s.CreateDiagnosticsService()).Returns(diagnosticSvc.Object);
            svcFactory.Setup(s => s.CreateDecompilerService()).Returns(dcSvc.Object);
            svcFactory.Setup(s => s.CreateDisassemblyViewService()).Returns(disasmSvc.Object);
            svcFactory.Setup(s => s.CreateMemoryViewService()).Returns(memSvc.Object);
            svcFactory.Setup(s => s.CreateDecompilerEventListener()).Returns(new FakeDecompilerEventListener());
            svcFactory.Setup(s => s.CreateImageSegmentService()).Returns(imgSegSvc.Object);
            svcFactory.Setup(s => s.CreateInitialPageInteractor()).Returns(new FakeInitialPageInteractor());
            svcFactory.Setup(s => s.CreateScannedPageInteractor()).Returns(new FakeScannedPageInteractor());
            svcFactory.Setup(s => s.CreateTypeLibraryLoaderService()).Returns(typeLibSvc.Object);
            svcFactory.Setup(s => s.CreateProjectBrowserService()).Returns(brSvc.Object);
            svcFactory.Setup(s => s.CreateProcedureListService()).Returns(procSvc.Object);
            svcFactory.Setup(s => s.CreateUiPreferencesService()).Returns(uiPrefs.Object);
            svcFactory.Setup(s => s.CreateFileSystemService()).Returns(fsSvc.Object);
            svcFactory.Setup(s => s.CreateStatusBarService()).Returns(sbSvc.Object);
            svcFactory.Setup(s => s.CreateTabControlHost()).Returns(tcHostSvc.Object);
            svcFactory.Setup(s => s.CreateLoader()).Returns(loader.Object);
            svcFactory.Setup(s => s.CreateSearchResultService()).Returns(srSvc.Object);
            svcFactory.Setup(s => s.CreateResourceEditorService()).Returns(resEditSvc.Object);
            svcFactory.Setup(s => s.CreateCallGraphViewService()).Returns(cgvSvc.Object);
            svcFactory.Setup(s => s.CreateViewImportService()).Returns(vimpSvc.Object);
            svcFactory.Setup(s => s.CreateSymbolLoadingService()).Returns(symLoadSvc.Object);
            svcFactory.Setup(s => s.CreateSelectionService()).Returns(selSvc.Object);
            svcFactory.Setup(s => s.CreateCallHierarchyService()).Returns(callHierSvc.Object);
            svcFactory.Setup(s => s.CreateDecompiledFileService()).Returns(dcFileSvc.Object);
            services.AddService<IDialogFactory>(dlgFactory.Object);
            services.AddService<IServiceFactory>(svcFactory.Object);
            services.AddService<IFileSystemService>(fsSvc.Object);
            brSvc.Setup(b => b.Clear());

            form = new Mock<IMainForm>();
            form.Setup(f => f.Dispose());
            form.Setup(f => f.UpdateToolbarState());
            tcHostSvc.Setup(t => t.QueryStatus(
                It.IsAny<CommandID>(),
                It.IsAny<CommandStatus>(),
                It.IsAny<CommandText>())).Returns(false);
            tcHostSvc.Setup(t => t.Execute(It.IsAny<CommandID>())).Returns(false);
            fsSvc.Setup(f => f.CreateStreamWriter(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Encoding>()))
                .Returns((string f, bool a, Encoding e) => new StringWriter());
            uiSvc.Setup(u => u.DocumentWindows).Returns(new List<IWindowFrame>());
            brSvc.Setup(u => u.ContainsFocus).Returns(false);
            tcHostSvc.Setup(u => u.ContainsFocus).Returns(false);

            // We currently don't care about testing the appearance of the Main window text.
            // Should this be required, you will need to remove the line below and add an 
            // appropriate stub/expectation in all the tests below. Good luck with that.
            //form.Setup(f => f.TitleText = "").IgnoreArguments();
        }

        private void Expect_RestoreWindowSize()
        {
            var size = new Size(800, 600);
            uiPrefs.Setup(u => u.Load());
            uiPrefs.Setup(u => u.WindowSize).Returns(size);
            uiPrefs.Setup(u => u.WindowState).Returns(FormWindowState.Normal);
            form.SetupSet(f => f.Size = size);
            form.SetupSet(f => f.WindowState = FormWindowState.Normal);
        }

        private void When_CreateMainFormInteractor()
        {
            Assert.IsNotNull(dlgFactory, "Make sure you have called SetupMainFormInteractor to set up all mocks and stubs");
            interactor = new MainFormInteractor(services);
            interactor.Attach(form.Object);
        }

        private void When_MainFormInteractorWithLoader()
        {
            Assert.IsNotNull(dlgFactory, "Make sure you have called SetupMainFormInteractor to set up all mocks and stubs");
            var services = new ServiceContainer();
            services.AddService<IDialogFactory>(dlgFactory.Object);
            services.AddService<IServiceFactory>(svcFactory.Object);
            services.AddService<IDecompilerShellUiService>(uiSvc.Object);
            services.AddService<IFileSystemService>(fsSvc.Object);
            interactor = new MainFormInteractor(services);
            interactor.Attach(form.Object);
        }

        private CommandStatus QueryStatus(int cmdId)
        {
            CommandStatus status = new CommandStatus();
            if (interactor.QueryStatus(new CommandID(CmdSets.GuidReko, cmdId), status, null))
                return status;
            else
                return null;
        }

        [Test]
        public void Mfi_FinishDecompilation_UpdateProcedureList()
        {
            Given_MainFormInteractor();
            Given_LoadPreferences();
            Given_DecompilerInstance();
            Given_XmlWriter();
            Given_SavePrompt(false);
            decompiler.Setup(d => d.AnalyzeDataFlow());
            decompiler.Setup(d => d.ReconstructTypes());
            decompiler.Setup(d => d.StructureProgram());
            fsSvc.Setup(f => f.MakeRelativePath("foo.dcproject", "foo.exe")).Returns("foo.exe");
            brSvc.Setup(b => b.Reload())
                .Verifiable();
            procSvc.Setup(p => p.Load(
                It.IsNotNull<Project>()))
                .Verifiable();

            When_CreateMainFormInteractor();
            interactor.OpenBinary("foo.exe");

            Assert.AreSame(interactor.InitialPageInteractor, interactor.CurrentPhase);
            interactor.FinishDecompilation();
            procSvc.Verify();
        }

    }
}
