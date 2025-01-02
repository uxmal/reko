#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Diagnostics;
using System.Threading;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider services;
        private readonly MainViewModel mainViewModel;

        public AvaloniaServiceFactory(IServiceProvider services, MainViewModel mainViewModel)
        {
            this.services = services;
            this.mainViewModel = mainViewModel;
        }

        public ISettingsService CreateSettingsService()
        {
            return new FileSystemSettingsService(services.RequireService<IFileSystemService>());
        }

        public IAnalyzedPageInteractor CreateAnalyzedPageInteractor()
        {
            return new AnalyzedPageInteractorImpl(services);
        }

        public IArchiveBrowserService CreateArchiveBrowserService()
        {
            return new ArchiveBrowserService(this.services);
        }

        public ICallGraphViewService CreateCallGraphViewService()
        {
            return new AvaloniaCallGraphViewService(this.services);
        }

        public ICodeViewerService CreateCodeViewerService()
        {
            return new AvaloniaCodeViewerService();
        }

        public IDecompiledFileService CreateDecompiledFileService()
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var listener = services.RequireService<IDecompilerEventListener>();
            var svc = new DecompiledFileService(services, fsSvc, listener);
            return svc;
        }

        public IConfigurationService CreateDecompilerConfiguration()
        {
            return RekoConfigurationService.Load(services);
        }

        public IDecompilerEventListener CreateDecompilerEventListener()
        {
            return new AvaloniaEventListener(services);
        }

        public IDecompilerService CreateDecompilerService()
        {
            return new DecompilerService();
        }

        public IDiagnosticsService CreateDiagnosticsService()
        {
            return new AvaloniaDiagnosticsService(
                services,
                mainViewModel.DiagnosticsList,
                SynchronizationContext.Current!);
        }

        public IDisassemblyViewService CreateDisassemblyViewService()
        {
            return new AvaloniaDisassemblyViewService(services);
        }

        public IFileSystemService CreateFileSystemService()
        {
            return new FileSystemService();
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
            return new LowLevelViewService(services);
        }

        public IOutputService CreateOutputService()
        {
            return new AvaloniaOutputService(services);
        }

        public IProcedureListService CreateProcedureListService()
        {
            return new AvaloniaProcedureListService(services, mainViewModel.ProcedureList);
        }

        public IProjectBrowserService CreateProjectBrowserService()
        {
            var pbSvc = new ProjectBrowserService(
                services,
                mainViewModel.ProjectBrowser,
                mainViewModel.ProjectBrowser?.TreeView);
            return pbSvc;
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
            return new AvaloniaSearchResultService(mainViewModel.SearchResults, services);

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

        public IStatusBarService CreateStatusBarService(ISelectedAddressService selAddrSvc)
        {
            if (this.mainViewModel.Status is null)
            {
                mainViewModel.Status = new AvaloniaStatusBarService(selAddrSvc);
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
            var configSvc = services.RequireService<IConfigurationService>();
            var settingsSvc = services.RequireService<ISettingsService>();
            return new AvaloniaUiPreferencesService(configSvc, settingsSvc);
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

        public IBaseAddressFinderService CreateBaseAddressFinderService()
        {
            return new BaseAddressFinderService(this.services);
        }

        public IStructureEditorService CreateStructureEditorService()
        {
            return new StructureEditorService(this.services);
        }

        public ICallGraphNavigatorService CreateCallGraphNavigatorService()
        {
            return new AvaloniaCallGraphNavigatorService(this.services, mainViewModel);
        }

        public IEventBus CreateEventBus()
        {
            Debug.Assert(SynchronizationContext.Current is not null);
            return new EventBus(SynchronizationContext.Current);
        }

        public ICommandRouterService CreateCommandRouterService()
        {
            return new CommandRouterService();
        }
    }
}
