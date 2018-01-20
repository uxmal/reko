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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Gui.Windows;
using Reko.Gui.Windows.Forms;
using Reko.Loading;
using System;
using System.Windows.Forms;

namespace Reko.Gui
{
    /// <summary>
    /// Decouples the creation of services, so that proper unit testing can be done.
    /// </summary>
    public interface IServiceFactory
    {
        DecompilerEventListener CreateDecompilerEventListener();
        IArchiveBrowserService CreateArchiveBrowserService();
        IConfigurationService CreateDecompilerConfiguration();
        IDecompilerShellUiService CreateShellUiService(IMainForm form, DecompilerMenus dm);
        IDecompilerService CreateDecompilerService();
        IDiagnosticsService CreateDiagnosticsService(ListView list);
        IDisassemblyViewService CreateDisassemblyViewService();
        IFileSystemService CreateFileSystemService();
        InitialPageInteractor CreateInitialPageInteractor();
        IScannedPageInteractor CreateScannedPageInteractor();
        ILowLevelViewService CreateMemoryViewService();
        IProjectBrowserService CreateProjectBrowserService(ITreeView treeView);
        ISearchResultService CreateSearchResultService(ListView listView);
        IResourceEditorService CreateResourceEditorService();
        ITabControlHostService CreateTabControlHost(TabControl tabControl);
        ITypeLibraryLoaderService CreateTypeLibraryLoaderService();
        IUiPreferencesService CreateUiPreferencesService();
        ILoader CreateLoader();
        ICallGraphViewService CreateCallGraphViewService();
        IViewImportsService CreateViewImportService();
    }

    public class ServiceFactory : IServiceFactory
    {
        private IServiceProvider services;

        public ServiceFactory(IServiceProvider services)
        {
            this.services = services;
        }

        public IArchiveBrowserService CreateArchiveBrowserService()
        {
            return new ArchiveBrowserService(services);
        }

        public IConfigurationService CreateDecompilerConfiguration()
        {
            return RekoConfigurationService.Load();
        }

        public IDiagnosticsService CreateDiagnosticsService(ListView list)
        {
            var d = new DiagnosticsInteractor();
            d.Attach(list);
            return d;
        }

        public IDecompilerShellUiService CreateShellUiService(IMainForm form, DecompilerMenus dm)
        {
            return new DecompilerShellUiService(form, dm, form.OpenFileDialog, form.SaveFileDialog, services);
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

        public IScannedPageInteractor CreateScannedPageInteractor()
        {
            return new ScannedPageInteractor(services);
        }

        public ITypeLibraryLoaderService CreateTypeLibraryLoaderService()
        {
            return new TypeLibraryLoaderServiceImpl(services);
        }

        public IProjectBrowserService CreateProjectBrowserService(ITreeView treeView)
        {
            return new ProjectBrowserService(services, treeView);
        }

        public ISearchResultService CreateSearchResultService(ListView listView)
        {
            return new SearchResultServiceImpl(services, listView);
        }

        public IResourceEditorService CreateResourceEditorService()
        {
            return new ResourceEditorService(services);
        }

        public ITabControlHostService CreateTabControlHost(TabControl tabControl)
        {
            return new TabControlHost(services, tabControl);
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
