#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.Loading;
using Reko.Services;
using Reko.UserInterfaces.WindowsForms.Controls;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.ComponentModel.Design;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Factory class for all services used by the Windows Forms version
    /// of the Reko UI.
    /// </summary>
    public class WindowsServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider services;
        private readonly MainForm mainForm;

        public WindowsServiceFactory(IServiceProvider services, MainForm mainForm)
        {
            this.services = services;
            this.mainForm = mainForm;
        }

        public IArchiveBrowserService CreateArchiveBrowserService()
        {
            return new ArchiveBrowserService(services);
        }

        public ICodeViewerService CreateCodeViewerService()
        {
            return new CodeViewerServiceImpl(services);
        }

        public ITextFileEditorService CreateTextFileEditorService()
        {
            return new TextFileViewerServiceImpl(services);
        }

        public ITextFileEditor CreateTextFileEditor()
        {
            return new TextFileEditor();
        }

        public IConfigurationService CreateDecompilerConfiguration()
        {
            return RekoConfigurationService.Load(services);
        }

        public IDiagnosticsService CreateDiagnosticsService()
        {
            var d = new DiagnosticsInteractor();
            d.Attach(mainForm.DiagnosticsList, mainForm.DiagnosticsFilter);
            return d;
        }

        public ImageSegmentService CreateImageSegmentService()
        {
            return new ImageSegmentServiceImpl(services, (s, p) => new ImageSegmentPane(s, p));
        }

        public ILowLevelViewService CreateMemoryViewService()
        {
            return new WindowsLowLevelViewService(services);
        }

        public IDisassemblyViewService CreateDisassemblyViewService()
        {
            return new DisassemblyViewServiceImpl(services);
        }

        public IDecompilerService CreateDecompilerService()
        {
            return new DecompilerService();
        }

        public ILoader CreateLoader()
        {
            return new Loader(services);
        }

        public DecompilerEventListener CreateDecompilerEventListener()
        {
            return new WindowsDecompilerEventListener(services);
        }

        public InitialPageInteractor CreateInitialPageInteractor()
        {
            return new InitialPageInteractorImpl(this.services);
        }

        public IScannedPageInteractor CreateScannedPageInteractor()
        {
            return new ScannedPageInteractor(services);
        }

        public ITypeLibraryLoaderService CreateTypeLibraryLoaderService()
        {
            return new TypeLibraryLoaderServiceImpl(services);
        }

        public IProjectBrowserService CreateProjectBrowserService()
        {
            return new ProjectBrowserService(services, mainForm.ProjectBrowserTab, mainForm.ProjectBrowser);
        }

        public ISearchResultService CreateSearchResultService()
        {
            return new SearchResultServiceImpl(services, mainForm.FindResultsList);
        }

        public IResourceEditorService CreateResourceEditorService()
        {
            return new ResourceEditorService(services);
        }

        public IStatusBarService CreateStatusBarService(ISelectedAddressService selAddrSvc)
        {
            return new WindowsStatusBarService(mainForm.StatusStrip, mainForm.SelectedAddressLabel, selAddrSvc);
        }

        public ITabControlHostService CreateTabControlHost()
        {
            var srSvc = services.RequireService<ISearchResultService>();
            var diagnosticsSvc = services.RequireService<IDiagnosticsService>();
            var callHierSvc = services.RequireService<ICallHierarchyService>();
            var tchSvc = new TabControlHost(services, mainForm.TabControl);
            tchSvc.Attach((IWindowPane)srSvc, mainForm.FindResultsPage);
            tchSvc.Attach((IWindowPane)diagnosticsSvc, mainForm.DiagnosticsPage);
            tchSvc.Attach((IWindowPane) callHierSvc, mainForm.CallHierarchyPage);

            return tchSvc;
        }

        public IUiPreferencesService CreateUiPreferencesService()
        {
            var configSvc = services.RequireService<IConfigurationService>();
            var settingsSvc = services.RequireService<ISettingsService>();
            return new UiPreferencesService(configSvc, settingsSvc);
        }

        public IFileSystemService CreateFileSystemService()
        {
            return new FileSystemServiceImpl();
        }

        public ICallGraphViewService CreateCallGraphViewService()
        {
            return new CallGraphViewService(services);
        }

        public IViewImportsService CreateViewImportService()
        {
            return new ViewImportsService(services);
        }

        public IAnalyzedPageInteractor CreateAnalyzedPageInteractor()
        {
            return new AnalyzedPageInteractorImpl(services);
        }

        public IFinalPageInteractor CreateFinalPageInteractor()
        {
            return new FinalPageInteractor(services);
        }

        public ISymbolLoadingService CreateSymbolLoadingService()
        {
            return new SymbolLoadingService(services);
        }

        public ISelectionService CreateSelectionService()
        {
            return new SelectionService();
        }

        public ISelectedAddressService CreateSelectedAddressService()
        {
            return new SelectedAddressService();
        }

        public IProcedureListService CreateProcedureListService()
        {
            return new ProcedureListService(services, mainForm.ProcedureListTab, mainForm.ProcedureFilter, mainForm.ProcedureList);
        }

        public ICallHierarchyService CreateCallHierarchyService()
        {
            var svc = new CallHierarchyInteractor(this.services, mainForm.CallHierarchy);
            return svc;
        }

        public IDecompiledFileService CreateDecompiledFileService()
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var listener = services.RequireService<DecompilerEventListener>();
            var svc = new DecompiledFileService(services, fsSvc, listener);
            return svc;
        }

        public ITestGenerationService CreateTestGenerationService()
        {
            return new TestGenerationService(services);
        }

        public IUserEventService CreateUserEventService()
        {
            return new UserEventService();
        }

        public IOutputService CreateOutputService()
        {
            var outputSvc = new OutputWindowInteractor();
            outputSvc.Attach(mainForm.OutputWindowSources, mainForm.OutputWindowPanel);
            return outputSvc;
        }

        public IStackTraceService CreateStackTraceService()
        {
            return new StackTraceService(services, mainForm);
        }

        public IHexDisassemblerService CreateHexDisassemblerService()
        {
            return new HexDisassemblerService(services);
        }

        public ISegmentListService CreateSegmentListService()
        {
            return new SegmentListService(services);
        }

        public IBaseAddressFinderService CreateBaseAddressFinderService()
        {
            return new BaseAddressFinderService(this.services);
        }
    }
}
