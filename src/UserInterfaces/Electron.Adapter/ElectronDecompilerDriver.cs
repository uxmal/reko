using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Lib;
using Reko.Core.Serialization.Json;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Loading;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.Gui.Electron.Adapter
{
    public class ElectronDecompilerDriver
    {
        private Project project;

	    private string secret = null;

	    private string appConfigPath;
	    private string fileNamePath;
	    private ElectronDiagnosticsService diagService;

	    private ServiceContainer services;

		public async Task<object> Hello2(dynamic foo) {
		    MessageBox.Show("Reko2");
		    secret = "foo";
		    return "Hello World2";
		}

	    public async Task<object> Hello3(dynamic foo) {
		    MessageBox.Show("Reko3");
		    return secret;
	    }

		public async Task<object> Hello(object foo)
        {
            MessageBox.Show("Hello, i'm Reko");
	        return new {
		        Reko2 = (Func<object, Task<object>>)Hello2,
		        Reko3 = (Func<object, Task<object>>)Hello3
	        };
        }

	    public async Task<object> CreateReko(dynamic input) {
		    appConfigPath = input.appConfig;
		    fileNamePath = input.fileName;

		    this.diagService = new ElectronDiagnosticsService((Func<object, Task<object>>)input.notify);

			var config = RekoConfigurationService.Load(this.appConfigPath);
		    var listener = new ElectronEventListener(this.diagService);

		    this.services = new ServiceContainer();
			services.AddService<DecompilerEventListener>(listener);
		    services.AddService<IConfigurationService>(config);
		    services.AddService<ITypeLibraryLoaderService>(new TypeLibraryLoaderServiceImpl(services));
		    services.AddService<IDiagnosticsService>(this.diagService);
		    services.AddService<IFileSystemService>(new FileSystemServiceImpl());
		    services.AddService<DecompilerHost>(new ElectronDecompilerHost());


			return new {
				Decompile = (Func<object, Task<object>>)Decompile,
				RenderProcedure = (Func<object, Task<object>>)RenderProcedure,
                RenderProjectJson = (Func<object, Task<object>>)RenderProjectJson,
				ShowSearchResult = (Func<object, Task<object>>)ShowSearchResult
			};
	    }

		/// <summary>
		/// Decompiles the current project.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<object> Decompile(dynamic input)
        {
            var ldr = new Loader(services);
            var decompiler = new DecompilerDriver(ldr, services);
            decompiler.Decompile(this.fileNamePath);
            //$REVIEW: Ew. using a global variable to keep this alive unti the next call.
            // How do we reason about instances?
            project = decompiler.Project;
            return await Task.FromResult(project.Programs[0].Name);
        }

	    public async Task<object> ShowSearchResult(dynamic input) {
		    string command = (string) input.command;
		    var searchResult = new {
			    header = new {
				    cssclass = "srHeader",
				    columns = new[] {
					    new {
						    title = "Address",
						    cssclass = "addrColumn"
					    },
					    new {
						    title = "Bytes",
						    cssclass = "bytecolumn"
					    },
					    new {
						    title = "Text",
						    cssclass = "textColumn"
					    }
				    },
			    },
			    data = new[] {
				    new object[] {"00123400", "61 62 63 64", "abcd[wers..."},
				    new object[] {"00123508", "31 32 33 34", "12345678...."},
			    }
		    };
		    return searchResult;
	    }

		/// <summary>
		/// Renders a procedure as HTML
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
        public async Task<object> RenderProcedure(dynamic input)
        {
            string sProgramProcedure = (string)input;
            var a = sProgramProcedure.Split(':');
            var sProgram = a[0];
            var sAddr = a[1];

			// It should be impossible for the statement below to fail
			// because it should be using the name of the program that 
			// was passed to it when reko first returned the tree.
			var program = (from p in project.Programs
                           where p.Name == sProgram
                           select p).Single();
			Address addr;
			if (!program.Architecture.TryParseAddress(sAddr, out addr))
			{
				throw new ArgumentException(string.Format("Invalid address '{0}' supplied.", sAddr));
			}
			Procedure proc;
			if (!program.Procedures.TryGetValue(addr, out proc)) 
			{
				throw new ArgumentException(string.Format("No known procedure at address {0}.", addr));
			}
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

        /// <summary>
        /// Renders the Reko project into a JSON string.
        /// </summary>
        public async Task<object> RenderProjectJson(dynamic input)
        {
            var jsp = new JsonProjectSerializer();
            var sw = new StringWriter();
            jsp.Serialize(this.project, sw);
            return await Task.FromResult(sw.ToString());
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
