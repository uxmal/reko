#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reko.UnitTests.Gui.Forms
{
    [TestFixture]
    public class MainFormInteractorTests
    {
        private ServiceContainer sc;
        private Mock<IFileSystemService> fsSvc;
        private Mock<IServiceFactory> svcFactory;
        private Mock<InitialPageInteractor> initialPageInteractor;
        private Mock<IDecompilerService> decompilerSvc;

        [SetUp]
        public void Setup()
        {
            this.sc = null;
            this.fsSvc = null;
            this.svcFactory = null;
            this.initialPageInteractor = null;
            this.decompilerSvc = null;
        }

        private void Given_ServiceContainer()
        {
            this.sc = new ServiceContainer();
            var df = new Mock<IDialogFactory>();
            var uiSvc = new Mock<IDecompilerShellUiService>();
            fsSvc = new Mock<IFileSystemService>();
            svcFactory = new Mock<IServiceFactory>();
            var settingsSvc = new Mock<ISettingsService>();
            var selAddrSvc = new Mock<ISelectedAddressService>();

            sc.AddService<IDialogFactory>(df.Object);
            sc.AddService<IFileSystemService>(fsSvc.Object);
            sc.AddService<IDecompilerShellUiService>(uiSvc.Object);
            sc.AddService<IServiceFactory>(svcFactory.Object);
            sc.AddService<ISettingsService>(settingsSvc.Object);
            sc.AddService<ISelectedAddressService>(selAddrSvc.Object);
    
            var config = new Mock<IConfigurationService>();
            var eventBus = new Mock<IEventBus>();
            var cmdFactory = new Mock<ICommandFactory>();
            var commandRouter = new Mock<ICommandRouterService>();

            var diagnosticsSvc = new Mock<IDiagnosticsService>();
            decompilerSvc = new Mock<IDecompilerService>();
            var selSvc = new Mock<ISelectionService>();
            var codeViewSvc = new Mock<ICodeViewerService>();
            var textEditorSvc = new Mock<ITextFileEditorService>();
            var segmentViewSvc = new Mock<ImageSegmentService>();
            initialPageInteractor = new Mock<InitialPageInteractor>();
            var sbSvc = new Mock<IStatusBarService>();
            var del = new FakeDecompilerEventListener();
            var dfSvc = new Mock<IDecompiledFileService>();

            var loader = new Mock<ILoader>();
            var abSvc = new Mock<IArchiveBrowserService>();
            var llvSvc = new Mock<ILowLevelViewService>();
            var dasmviewSvc = new Mock<IDisassemblyViewService>();
            var tlSvc = new Mock<ITypeLibraryLoaderService>();
            var projectBrowserSvc = new Mock<IProjectBrowserService>();
            var procedureListSvc = new Mock<IProcedureListService>();
            var upSvc = new Mock<IUiPreferencesService>();
            var srSvc = new Mock<ISearchResultService>();
            var searchResultsTabControl = new Mock<ITabControlHostService>();
            var resEditService = new Mock<IResourceEditorService>();
            var cgvSvc = new Mock<ICallGraphViewService>();
            var viewImpSvc = new Mock<IViewImportsService>();
            var symLdrSvc = new Mock<ISymbolLoadingService>();
            var testGenSvc = new Mock<ITestGenerationService>();
            var userEventSvc = new Mock<IUserEventService>();
            var outputSvc = new Mock<IOutputService>();
            var stackTraceSvc = new Mock<IStackTraceService>();
            var hexDasmSvc = new Mock<IHexDisassemblerService>();
            var segListSvc = new Mock<ISegmentListService>();
            var bafSvc = new Mock<IBaseAddressFinderService>();
            var seSvc = new Mock<IStructureEditorService>();
            var cgnSvc = new Mock<ICallGraphNavigatorService>();

            /////
            svcFactory.Setup(s => s.CreateDecompilerConfiguration()).Returns(config.Object);
            svcFactory.Setup(s => s.CreateEventBus()).Returns(eventBus.Object);
            svcFactory.Setup(s => s.CreateCommandRouterService()).Returns(commandRouter.Object);
            svcFactory.Setup(s => s.CreateDiagnosticsService()).Returns(diagnosticsSvc.Object);
            svcFactory.Setup(s => s.CreateDecompilerService()).Returns(decompilerSvc.Object);
            svcFactory.Setup(s => s.CreateSelectionService()).Returns(selSvc.Object);
            svcFactory.Setup(s => s.CreateSelectedAddressService()).Returns(selAddrSvc.Object);
            svcFactory.Setup(s => s.CreateCodeViewerService()).Returns(codeViewSvc.Object);
            svcFactory.Setup(s => s.CreateTextFileEditorService()).Returns(textEditorSvc.Object);
            svcFactory.Setup(s => s.CreateImageSegmentService()).Returns(segmentViewSvc.Object);
            svcFactory.Setup(s => s.CreateInitialPageInteractor()).Returns(initialPageInteractor.Object);
            svcFactory.Setup(s => s.CreateStatusBarService(
                It.IsAny<ISelectedAddressService>())).Returns(sbSvc.Object);
            //sc.AddService(typeof(IWorkerDialogService), workerDlgSvc.Object);
            svcFactory.Setup(s => s.CreateDecompilerEventListener()).Returns(del);
            svcFactory.Setup(s => s.CreateDecompiledFileService()).Returns(dfSvc.Object);
            svcFactory.Setup(s => s.CreateLoader()).Returns(loader.Object);
            svcFactory.Setup(s => s.CreateArchiveBrowserService()).Returns(abSvc.Object);
            svcFactory.Setup(s => s.CreateMemoryViewService()).Returns(llvSvc.Object);
            svcFactory.Setup(s => s.CreateDisassemblyViewService()).Returns(dasmviewSvc.Object);
            svcFactory.Setup(s => s.CreateTypeLibraryLoaderService()).Returns(tlSvc.Object);
            svcFactory.Setup(s => s.CreateProjectBrowserService()).Returns(projectBrowserSvc.Object);
            svcFactory.Setup(s => s.CreateProcedureListService()).Returns(procedureListSvc.Object);
            svcFactory.Setup(s => s.CreateUiPreferencesService()).Returns(upSvc.Object);
            svcFactory.Setup(s => s.CreateSearchResultService()).Returns(srSvc.Object);
            svcFactory.Setup(s => s.CreateTabControlHost()).Returns(searchResultsTabControl.Object);
            svcFactory.Setup(s => s.CreateResourceEditorService()).Returns(resEditService.Object);
            svcFactory.Setup(s => s.CreateCallGraphViewService()).Returns(cgvSvc.Object);
            svcFactory.Setup(s => s.CreateViewImportService()).Returns(viewImpSvc.Object);
            svcFactory.Setup(s => s.CreateSymbolLoadingService()).Returns(symLdrSvc.Object);
            svcFactory.Setup(s => s.CreateTestGenerationService()).Returns(testGenSvc.Object);
            svcFactory.Setup(s => s.CreateUserEventService()).Returns(userEventSvc.Object);
            svcFactory.Setup(s => s.CreateOutputService()).Returns(outputSvc.Object);
            svcFactory.Setup(s => s.CreateStackTraceService()).Returns(stackTraceSvc.Object);
            svcFactory.Setup(s => s.CreateHexDisassemblerService()).Returns(hexDasmSvc.Object);
            svcFactory.Setup(s => s.CreateSegmentListService()).Returns(segListSvc.Object);
            svcFactory.Setup(s => s.CreateBaseAddressFinderService()).Returns(bafSvc.Object);
            svcFactory.Setup(s => s.CreateStructureEditorService()).Returns(seSvc.Object);
            svcFactory.Setup(s => s.CreateCallGraphNavigatorService()).Returns(cgnSvc.Object);

        }

        [Test]
        public async Task MFI_Title()
        {
            Given_ServiceContainer();
            fsSvc.Setup(f => f.CreateStreamWriter(It.IsNotNull<string>(), false, It.IsNotNull<Encoding>()))
                .Returns(new StringWriter());
            initialPageInteractor.Setup(i => i.OpenBinary(It.IsNotNull<string>()))
                .Returns(ValueTask.FromResult(true));

            var mfi = new MainFormInteractor(sc);
            var mainForm = new Mock<IMainForm>();
            mfi.Attach(mainForm.Object);
            var defaultText = mfi.TitleText;

            Assert.AreEqual("Reko Decompiler (??-bit)", Regex.Replace(mfi.TitleText, "32|64", "??"));

            decompilerSvc.Setup(d => d.ProjectName).Returns("foo.exe");
            await mfi.OpenBinary("/home/bob/foo.exe");

            Assert.AreEqual("foo.exe - Reko Decompiler (??-bit)", Regex.Replace(mfi.TitleText, "32|64", "??"));

            decompilerSvc.Setup(d => d.ProjectName).Returns((string)null);
            await mfi.CloseProject();
            Assert.AreEqual("Reko Decompiler (??-bit)", Regex.Replace(mfi.TitleText, "32|64", "??"));
        }
    }
}
