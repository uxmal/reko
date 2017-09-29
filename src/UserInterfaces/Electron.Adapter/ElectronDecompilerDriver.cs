using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Loading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.Gui.Electron.Adapter
{
    public class ElectronDecompilerDriver 
    {
        public async Task<object> Hello(object foo)
        {
            MessageBox.Show("Hello, i'm Reko");
            return null;
        }

        public async Task<object> Decompile(dynamic input)
        {
            var services = new ServiceContainer();
            var config = RekoConfigurationService.Load(input.appConfig as string);
            var diagnosticSvc = new ElectronDiagnosticsService(Console.Out);
            var listener = new ElectronEventListener(diagnosticSvc); 
            services.AddService<DecompilerEventListener>(listener);
            services.AddService<IConfigurationService>(config);
            services.AddService<ITypeLibraryLoaderService>(new TypeLibraryLoaderServiceImpl(services));
            services.AddService<IDiagnosticsService>(diagnosticSvc);
            services.AddService<IFileSystemService>(new FileSystemServiceImpl());
            services.AddService<DecompilerHost>(new ElectronDecompilerHost());
            var ldr = new Loader(services);
            var decompiler = new DecompilerDriver(ldr, services);
            decompiler.Decompile(input.fileName);

            return null;
        }

#if false
		public static void Main(string[] args)
        {
            var x = new ElectronDecompilerDriver();
            x.Decompile(@"C:\dev\uxmal\reko\zoo\users\smxsmx\abheram\Aberaham.exe");
        }
#endif
    }
}
