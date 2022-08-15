#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.Loading;
using Reko.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents;
using System;
using System.ComponentModel.Design;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaServiceFactory : IServiceFactory
    {
        private IServiceProvider services;
        private MainViewModel mainViewModel;

        public AvaloniaServiceFactory(IServiceProvider services, MainViewModel mainViewModel)
        {
            this.services = services;
            this.mainViewModel = mainViewModel;
        }

        public IAnalyzedPageInteractor CreateAnalyzedPageInteractor()
        {
            return new AnalyzedPageInteractorImpl(services);
        }

        public IArchiveBrowserService CreateArchiveBrowserService()
        {
            return new AvaloniaArchiveBrowserService(this.services);
        }

        public ICallGraphViewService CreateCallGraphViewService()
        {
            return new AvaloniaCallGraphViewService(this.services);
        }

        public ICallHierarchyService CreateCallHierarchyService()
        {
            return new AvaloniaCallHierarchyService(services);
        }

        public ICodeViewerService CreateCodeViewerService()
        {
            return new AvaloniaCodeViewerService();
        }

        public IDecompiledFileService CreateDecompiledFileService()
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var listener = services.RequireService<DecompilerEventListener>();
            var svc = new DecompiledFileService(services, fsSvc, listener);
            return svc;
        }

        public IConfigurationService CreateDecompilerConfiguration()
        {
            return RekoConfigurationService.Load(services);
        }

        public DecompilerEventListener CreateDecompilerEventListener()
        {
            return new AvaloniaEventListener(services);
        }

        public IDecompilerService CreateDecompilerService()
        {
            return new DecompilerService();
        }

        public IDiagnosticsService CreateDiagnosticsService()
        {
            return new AvaloniaDiagnosticsService(mainViewModel);
        }

        public IDisassemblyViewService CreateDisassemblyViewService()
        {
            return new AvaloniaDisassemblyViewService(services);
        }

        public IFileSystemService CreateFileSystemService()
        {
            return new FileSystemServiceImpl();
        }

        public IFinalPageInteractor CreateFinalPageInteractor()
        {
            return new FinalPageInteractor(services);
        }

        public IHexDisassemblerService CreateHexDisassemblerService()
        {
            return new HexDisassemblerService(this.services);
        }

        public ImageSegmentService CreateImageSegmentService()
        {
            return new ImageSegmentServiceImpl(
                services,
                (s, p) => new ImageSegmentViewModel(s, p));
        }

        public InitialPageInteractor CreateInitialPageInteractor()
        {
            return new InitialPageInteractorImpl(services);
        }

        public ILoader CreateLoader()
        {
            return new Loader(services);
        }

        public ILowLevelViewService CreateMemoryViewService()
        {
            return new AvaloniaMemoryViewService(services);
        }

        public IOutputService CreateOutputService()
        {
            return new AvaloniaOutputService(services);
        }

        public IProcedureListService CreateProcedureListService()
        {
            return new AvaloniaProcedureListService(services);
        }

        public IProjectBrowserService CreateProjectBrowserService()
        {
            return new ProjectBrowserService(
                services,
                mainViewModel.ProjectBrowser,
                mainViewModel.ProjectBrowser?.TreeView);
        }

        public IResourceEditorService CreateResourceEditorService()
        {
            return new AvaloniaResourceEditorService(services);
        }

        public IScannedPageInteractor CreateScannedPageInteractor()
        {
            return new ScannedPageInteractor(services);
        }

        public ISearchResultService CreateSearchResultService()
        {
            return new AvaloniaSearchResultService(services);
        }

        public ISelectedAddressService CreateSelectedAddressService()
        {
            return new SelectedAddressService();
        }

        public ISelectionService CreateSelectionService()
        {
            return new SelectionService();
        }

        public IStackTraceService CreateStackTraceService()
        {
            return new AvaloniaStackTraceService(services);
        }

        public IStatusBarService CreateStatusBarService()
        {
            if (this.mainViewModel.Status is null)
            {
                mainViewModel.Status = new AvaloniaStatusBarService();
            }
            return this.mainViewModel.Status;
        }

        public ISymbolLoadingService CreateSymbolLoadingService()
        {
            return new SymbolLoadingService(services);
        }

        public ITabControlHostService CreateTabControlHost()
        {
            return new AvaloniaTabControlHostService(services);
        }

        public ITestGenerationService CreateTestGenerationService()
        {
            return new TestGenerationService(services);
        }

        public ITextFileEditor CreateTextFileEditor()
        {
            return new AvaloniaTextFileEditor();
        }

        public ITextFileEditorService CreateTextFileEditorService()
        {
            return new AvaloniaTextFileViewerService(services);
        }

        public ITypeLibraryLoaderService CreateTypeLibraryLoaderService()
        {
            return new TypeLibraryLoaderServiceImpl(services);
        }

        public IUiPreferencesService CreateUiPreferencesService()
        {
            return new AvaloniaUiPreferencesService(services);
        }

        public IUserEventService CreateUserEventService()
        {
            return new AvaloniaUserEventService();
        }

        public IViewImportsService CreateViewImportService()
        {
            return new AvaloniaViewImportsService(this.services);
        }

        public ISegmentListService CreateSegmentListService()
        {
            return new SegmentListService(this.services);
        }
    }
}
