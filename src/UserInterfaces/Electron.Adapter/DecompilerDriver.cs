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
        public void Hello(object foo)
        {
            MessageBox.Show("Hello, i'm Reko");
        }

        public void Decompile(string filename)
        {
            var services = new ServiceContainer();
            var listener = new NullDecompilerEventListener();       //$TODO: spew to JAvascript 
            var config = RekoConfigurationService.Load(@"C:\dev\uxmal\reko\master\src\UserInterfaces\Electron\generated\assemblies\Reko.config");
            var diagnosticSvc = new ElectronDiagnosticsService(Console.Out);
            services.AddService<DecompilerEventListener>(listener);
            services.AddService<IConfigurationService>(config);
            services.AddService<ITypeLibraryLoaderService>(new TypeLibraryLoaderServiceImpl(services));
            services.AddService<IDiagnosticsService>(diagnosticSvc);
            services.AddService<IFileSystemService>(new FileSystemServiceImpl());
            services.AddService<DecompilerHost>(new ElectronDecompilerHost());
            var ldr = new Loader(services);
            var decompiler = new DecompilerDriver(ldr, services);
            decompiler.Decompile(filename);
        }

        public static void Main(string[] args)
        {
            var x = new ElectronDecompilerDriver();
            x.Decompile(@"C:\dev\uxmal\reko\zoo\users\smxsmx\abheram\Aberaham.exe");
        }
    }
}
