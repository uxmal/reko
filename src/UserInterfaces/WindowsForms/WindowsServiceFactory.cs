#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Loading;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Factory class for all services used by the Windows Forms version
    /// of the Reko UI.
    /// </summary>
    public class WindowsServiceFactory : IServiceFactory
    {
        private IServiceProvider services;

        public WindowsServiceFactory(IServiceProvider services)
        {
            this.services = services;
        }

        public IArchiveBrowserService CreateArchiveBrowserService()
        {
            return new ArchiveBrowserService(services);
        }

        public ICodeViewerService CreateCodeViewerService()
        {
            return new CodeViewerServiceImpl(services);
        }

        public IConfigurationService CreateDecompilerConfiguration()
        {
            return RekoConfigurationService.Load();
        }

        public IDiagnosticsService CreateDiagnosticsService(object listView)
        {
            var d = new DiagnosticsInteractor();
            d.Attach((ListView)listView);
            return d;
        }

        public ImageSegmentService CreateImageSegmentService()
        {
            return new ImageSegmentServiceImpl(services);
        }

        public ILowLevelViewService CreateMemoryViewService()
        {
            return new LowLevelViewServiceImpl(services);
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

        public ILoadedPageInteractor CreateLoadedPageInteractor()
        {
            return new LoadedPageInteractor(services);
        }

        public ITypeLibraryLoaderService CreateTypeLibraryLoaderService()
        {
            return new TypeLibraryLoaderServiceImpl(services);
        }

        public IProjectBrowserService CreateProjectBrowserService(ITreeView treeView)
        {
            return new ProjectBrowserService(services, treeView);
        }

        public ISearchResultService CreateSearchResultService(object listView)
        {
            return new SearchResultServiceImpl(services, (ListView)listView);
        }

        public IResourceEditorService CreateResourceEditorService()
        {
            return new ResourceEditorService(services);
        }

        public ITabControlHostService CreateTabControlHost(object tabControl)
        {
            return new TabControlHost(services, (TabControl)tabControl);
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
    }
}
