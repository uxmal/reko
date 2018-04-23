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
using Reko.Gui.Forms;
using Reko.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Reko.Gui.Electron.Adapter
{
    /// <summary>
    /// Factory class for all services used by the Electron version
    /// of the Reko UI.
    /// </summary>
    public class ElectronServiceFactory : IServiceFactory
    {
        private IServiceProvider services;
        private object searchResultChannel;

        public ElectronServiceFactory(
            IServiceProvider services, 
            object searchResultsChannel)
        {
            this.services = services;
            this.searchResultChannel = searchResultsChannel;
        }

        public IArchiveBrowserService CreateArchiveBrowserService()
        {
            throw new NotImplementedException();
        }

        public ICallGraphViewService CreateCallGraphViewService()
        {
            throw new NotImplementedException();
        }

        public ICodeViewerService CreateCodeViewerService()
        {
            throw new NotImplementedException();
        }

        public IConfigurationService CreateDecompilerConfiguration()
        {
            return RekoConfigurationService.Load();
        }

        public DecompilerEventListener CreateDecompilerEventListener()
        {
            return new ElectronEventListener(services.RequireService<IDiagnosticsService>());
        }

        public IDecompilerService CreateDecompilerService()
        {
            return new DecompilerService();
        }

        public IDiagnosticsService CreateDiagnosticsService()
        {
            //$TODO: get some "handle" to the widget that shows diagnostics.
            object list = null;
            return new ElectronDiagnosticsService((Func<object, Task<object>>)list);
        }

        public IDisassemblyViewService CreateDisassemblyViewService()
        {
            throw new NotImplementedException();
        }

        public IFileSystemService CreateFileSystemService()
        {
            return new FileSystemServiceImpl();
        }

        public ImageSegmentService CreateImageSegmentService()
        {
            throw new NotImplementedException();
        }

        public InitialPageInteractor CreateInitialPageInteractor()
        {
            throw new NotImplementedException();
        }

        public IScannedPageInteractor CreateScannedPageInteractor()
        {
            throw new NotImplementedException();
        }

        public ILoader CreateLoader()
        {
            return new Loader(services);
        }

        public ILowLevelViewService CreateMemoryViewService()
        {
            throw new NotImplementedException();
        }

        public IProjectBrowserService CreateProjectBrowserService()
        {
            throw new NotImplementedException();
        }

        public IResourceEditorService CreateResourceEditorService()
        {
            throw new NotImplementedException();
        }

        public ISearchResultService CreateSearchResultService()
        {
            return new ElectronSearchResultService(searchResultChannel);
        }

        public IStatusBarService CreateStatusBarService()
        {
            throw new NotImplementedException();
        }

        public ITabControlHostService CreateTabControlHost()
        {
            throw new NotImplementedException();
        }

        public ITypeLibraryLoaderService CreateTypeLibraryLoaderService()
        {
            throw new NotImplementedException();
        }

        public IUiPreferencesService CreateUiPreferencesService()
        {
            throw new NotImplementedException();
        }

        public IViewImportsService CreateViewImportService()
        {
            throw new NotImplementedException();
        }

        public IAnalyzedPageInteractor CreateAnalyzedPageInteractor()
        {
            throw new NotImplementedException();
        }

        public IFinalPageInteractor CreateFinalPageInteractor()
        {
            throw new NotImplementedException();
        }

        public ISymbolLoadingService CreateSymbolLoadingService()
        {
            throw new NotImplementedException();
        }
    }
}
