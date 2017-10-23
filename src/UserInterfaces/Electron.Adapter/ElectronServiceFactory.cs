using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Gui.Windows.Forms;
using Reko.Loading;

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

        public IDiagnosticsService CreateDiagnosticsService(object list)
        {
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

        public InitialPageInteractor CreateInitialPageInteractor()
        {
            throw new NotImplementedException();
        }

        public ILoadedPageInteractor CreateLoadedPageInteractor()
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

        public IProjectBrowserService CreateProjectBrowserService(ITreeView treeView)
        {
            throw new NotImplementedException();
        }

        public IResourceEditorService CreateResourceEditorService()
        {
            throw new NotImplementedException();
        }

        public ISearchResultService CreateSearchResultService(ListView listView)
        {
            return new ElectronSearchResultService(searchResultChannel);
        }

        public IDecompilerShellUiService CreateShellUiService(IMainForm form, DecompilerMenus dm)
        {
            throw new NotImplementedException();
        }

        public ITabControlHostService CreateTabControlHost(TabControl tabControl)
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
    }
}
