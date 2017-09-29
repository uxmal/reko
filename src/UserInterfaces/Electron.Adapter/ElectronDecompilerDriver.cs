using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Lib;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Loading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.Gui.Electron.Adapter
{
    public class ElectronDecompilerDriver
    {
        //$REVIEW: Ew. Global variable. Ew. Ew.
        private static Project project;

        public async Task<object> Hello(object foo)
        {
            MessageBox.Show("Hello, i'm Reko");
            return null;
        }

        public async Task<object> Decompile(dynamic input)
        {
            var services = new ServiceContainer();
            var config = RekoConfigurationService.Load(input.appConfig as string);
            var diagnosticSvc = new ElectronDiagnosticsService(Console.Out, (Func<object, Task<object>>)input.notify);
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
            //$REVIEW: Ew. using a global variable to keep this alive unti the next call.
            // How do we reason about instances?
            project = decompiler.Project;
            return await Task.FromResult(project.Programs[0].Name);
        }

        public async Task<object> RenderProcedure(dynamic input)
        {
            string sProgramProcedure = (string)input;
            var a = sProgramProcedure.Split(':');
            var sProgram = a[0];
            var sProcedure = a[1];
            var program = (from p in project.Programs
                           where p.Name == sProgram
                           select p).Single();
            var proc = (from p in program.Procedures.Values
                        where p.Name == sProcedure
                        select p).Single();
            var html = RenderProcedureToHtml(program, proc);
            return await Task.FromResult(html);
        }

        private string RenderProcedureToHtml(Program program, Procedure proc)
        {
            var output = new StringWriter();
            var f = new HtmlCodeFormatter(output, program.Procedures);
            f.Write(proc);
            output.WriteLine();
            return output.ToString();
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
