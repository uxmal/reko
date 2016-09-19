#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Loading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Reko.CmdLine
{
    public class CmdLineDriver
    {
        private IServiceProvider services;
        private RekoConfigurationService config;

        public static void Main(string[] args)
        {
            var services = new ServiceContainer();
            var listener = new CmdLineListener();
            var config = RekoConfigurationService.Load();
            var diagnosticSvc = new CmdLineDiagnosticsService(Console.Out);
            services.AddService<DecompilerEventListener>(listener);
            services.AddService<IConfigurationService>(config);
            services.AddService<ITypeLibraryLoaderService>(new TypeLibraryLoaderServiceImpl(services));
            services.AddService<IDiagnosticsService>(diagnosticSvc);
            services.AddService<IFileSystemService>(new FileSystemServiceImpl());
            services.AddService<DecompilerHost>(new CmdLineHost());
            var driver = new CmdLineDriver(services, config);
            driver.Execute(args);
        }

        public CmdLineDriver(IServiceProvider services, RekoConfigurationService config)
        {
            this.services = services;
            this.config = config;
        }

        public void Execute(string[] args)
        {
            var pArgs = ProcessArguments(Console.Out, args);
            if (pArgs == null)
                return;

            var ldr = new Loader(services);
            object defaultTo;
            if (pArgs.TryGetValue("--default-to", out defaultTo))
            {
                ldr.DefaultToFormat = (string)defaultTo;
            }
            var dec = new DecompilerDriver(ldr, services);

            if (OverridesRequested(pArgs))
            {
                DecompileRawImage(dec, pArgs);
            }
            else if (pArgs.ContainsKey("filename"))
            {
                dec.Decompile((string)pArgs["filename"]);
            }
            else
            {
                Usage(Console.Out);
            }
        }

        public bool OverridesRequested(Dictionary<string,object> pArgs)
        {
            if (pArgs.ContainsKey("--arch") ||
                pArgs.ContainsKey("--env") ||
                pArgs.ContainsKey("--base") ||
                pArgs.ContainsKey("--entry"))
            {
                if (pArgs.ContainsKey("--arch") &&
                    pArgs.ContainsKey("--base") &&
                    pArgs.ContainsKey("filename"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void DecompileRawImage(DecompilerDriver dec, Dictionary<string, object> pArgs)
        {
            var arch = config.GetArchitecture((string)pArgs["--arch"]);
            if (arch == null)
                throw new ApplicationException(string.Format("Unknown architecture {0}", pArgs["--arch"]));

            object sEnv;
            pArgs.TryGetValue("--env", out sEnv);

            Address addrBase;
            Address addrEntry;
            if (!arch.TryParseAddress((string)pArgs["--base"], out addrBase))
                throw new ApplicationException(string.Format("'{0}' doesn't appear to be a valid address.", pArgs["--base"]));
            if (pArgs.ContainsKey("--entry"))
            {
                if (!arch.TryParseAddress((string)pArgs["--base"], out addrEntry))
                    throw new ApplicationException(string.Format("'{0}' doesn't appear to be a valid address.", pArgs["--base"]));
            }
            else
                addrEntry = addrBase;


            var state = CreateInitialState(arch, pArgs);
            dec.LoadRawImage((string)pArgs["filename"], (string)pArgs["--arch"], (string) sEnv, addrBase);
            dec.Project.Programs[0].EntryPoints.Add(
                addrEntry,
                new ImageSymbol(addrEntry)
                {
                    ProcessorState = state
                });
            object oHeur;
            if (pArgs.TryGetValue("heuristics", out oHeur))
            {
                dec.Project.Programs[0].User.Heuristics = ((string[])oHeur).ToSortedSet();
            }
            dec.ScanPrograms();
            dec.AnalyzeDataFlow();
            dec.ReconstructTypes();
            dec.StructureProgram();
            dec.WriteDecompilerProducts();
        }

        private ProcessorState CreateInitialState(IProcessorArchitecture arch, Dictionary<string, object> args)
        {
            var state = arch.CreateProcessorState();
            if (!args.ContainsKey("--reg"))
                return state;
            var regs = (List<string>)args["--reg"];
            foreach (var regValue in regs.Where(r => !string.IsNullOrEmpty(r)))
            {
                var rr = regValue.Split(':');
                if (rr == null || rr.Length != 2)
                    continue;
                var reg = arch.GetRegister(rr[0]);
                state.SetRegister(reg, Constant.Create(reg.DataType, Convert.ToInt64(rr[1], 16)));
            }
            return state;
        }

        private Dictionary<string,object> ProcessArguments(TextWriter w, string[] args)
        {
            if (args.Length == 0)
            {
                Usage(w);
                return null;
            }
            var parsedArgs = new Dictionary<string, object>();
            for (int i = 0; i < args.Length; ++i)
            {
                if (string.IsNullOrEmpty(args[i]))
                    continue;
                var arg = args[i];
                if (arg.StartsWith("-h") || arg.StartsWith("--help"))
                {
                    Usage(w);
                    return null;
                }
                else if (arg.StartsWith("--version"))
                {
                    ShowVersion(w);
                    return null;
                }
                else if (args[i] == "--arch")
                {
                    if (i < args.Length - 1)
                        parsedArgs["--arch"] = args[++i];
                }
                else if (args[i] == "--env")
                {
                    if (i < args.Length - 1)
                        parsedArgs["--env"] = args[++i];
                }
                else if (args[i] == "--base")
                {
                    if (i < args.Length - 1)
                        parsedArgs["--base"] = args[++i];
                }
                else if (args[i] == "--entry")
                {
                    if (i < args.Length - 1)
                        parsedArgs["--entry"] = args[++i];
                }
                else if (args[i] == "--default-to")
                {
                    if (i < args.Length - 1)
                        parsedArgs["--default-to"] = args[++i];
                }
                else if (args[i] == "--reg" || args[i] == "-r")
                {
                    if (i < args.Length - 1)
                    {
                        object oRegs;
                        List<string> regs;
                        if (!parsedArgs.TryGetValue("--reg", out oRegs))
                        {
                            regs = new List<string>();
                            parsedArgs["--reg"] = regs;
                        }
                        else
                        {
                            regs = (List<string>)oRegs;
                        }
                        regs.Add(args[++i]);
                    }
                }
                else if (args[i] == "--heuristic")
                {
                    if (!string.IsNullOrEmpty(args[i]))
                    {
                        parsedArgs["heuristics"] = args[i].Split(',');
                    }
                }
                else if (arg.StartsWith("-"))
                {
                    w.WriteLine("error: uncrecognized option {0}", arg);
                    return null;
                }
                else
                {
                    parsedArgs["filename"] = args[i];
                }
            }
            return parsedArgs;
        }

        private static void ShowVersion(TextWriter w)
        {
            var attrs = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
            if (attrs.Length < 1)
                return;
            var attr = (AssemblyFileVersionAttribute)attrs[0];
            w.WriteLine("Decompile.exe version {0}", attr.Version);
        }

        private void Usage(TextWriter w)
        {
            w.WriteLine("usage: decompile [options] <filename>");
            w.WriteLine("    <filename> can be either an executable file or a project file.");
            w.WriteLine();
            w.WriteLine("Options:");
            w.WriteLine(" --version                Show version number and exit");
            w.WriteLine(" -h, --help               Show this message and exit");
            w.WriteLine(" --arch <architecture>    Use an architecture from the following:");
            DumpArchitectures(config, w, "    {0,-25} {1}");
            w.WriteLine(" --env <environment>      Use an operating environment from the following:");
            DumpEnvironments(config, w, "    {0,-25} {1}");
            w.WriteLine(" --base <address>         Use <address> as the base address of the program");
            w.WriteLine(" --default-to <format>    If no executable format can be recognized, default");
            w.WriteLine("                          to one of the following formats:");
            DumpRawFiles(config, w, "    {0,-25} {1}");
            w.WriteLine(" --entry <address>        Use <address> as an entry point to the program");
            w.WriteLine(" --reg <regInit>          Set register to value, where regInit is formatted as");
            w.WriteLine("                          reg_name:value, e.g. sp:FF00");
            w.WriteLine(" --heuristic <h1>[,<h2>...] Use one of the following heuristics to examine binary:");
            w.WriteLine("    shingle               Use shingle assembler to discard data ");
        }

        private static void DumpArchitectures(RekoConfigurationService config, TextWriter w, string fmtString)
        {
            foreach (var arch in config.GetArchitectures()
                .OfType<ArchitectureElement>()
                .OrderBy(a => a.Name))
            {
                w.WriteLine(fmtString, arch.Name, arch.Description);
            }
        }

        private static void DumpEnvironments(RekoConfigurationService config, TextWriter w, string fmtString)
        {
            foreach (var arch in config.GetEnvironments()
                .OfType<OperatingEnvironmentElement>()
                .OrderBy(a => a.Name))
            {
                w.WriteLine(fmtString, arch.Name, arch.Description);
            }
        }

        private static void DumpRawFiles(RekoConfigurationService config, TextWriter w, string fmtString)
        {
            foreach (var raw in config.GetRawFiles()
                .OfType<RawFileElement>()
                .OrderBy(a => a.Name))
            {
                w.WriteLine(fmtString, raw.Name, raw.Description);
            }
        }
    }
}
