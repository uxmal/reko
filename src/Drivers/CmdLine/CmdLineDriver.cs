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
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Libraries.Microchip;
using Reko.Loading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;


namespace Reko.CmdLine
{
    public class CmdLineDriver
    {
        private IServiceProvider services;
        private ILoader ldr;
        private IDecompiler decompiler;
        private IConfigurationService config;
        private IDiagnosticsService diagnosticSvc;
        private CmdLineListener listener;
        private Timer timer;
        private PICCrownking picDB;

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
            var ldr = new Loader(services);
            var decompiler = new DecompilerDriver(ldr, services);
            var driver = new CmdLineDriver(services, ldr, decompiler, listener);
            driver.Execute(args);
        }

        public CmdLineDriver(
            IServiceProvider services,
            ILoader ldr,
            IDecompiler decompiler,
            CmdLineListener listener)
        {
            this.services = services;
            this.ldr = ldr;
            this.decompiler = decompiler;
            this.listener = listener;
            this.config = services.RequireService<IConfigurationService>();
            this.diagnosticSvc = services.RequireService<IDiagnosticsService>();
        }

        public void Execute(string[] args)
        {
            var pArgs = ProcessArguments(Console.Out, args);
            if (pArgs == null)
                return;

            if (pArgs.TryGetValue("--default-to", out object defaultTo))
            {
                ldr.DefaultToFormat = (string)defaultTo;
            }

            StartTimer(pArgs);

            if (OverridesRequested(pArgs))
            {
                DecompileRawImage(pArgs);
            }
            else if (pArgs.ContainsKey("filename"))
            {
                Decompile(pArgs);
            }
            else
            {
                Usage(Console.Out);
            }
        }

        private void StartTimer(Dictionary<string, object> pArgs)
        {
            if (pArgs.TryGetValue("time-limit", out object oLimit))
            {
                int msecLimit = 1000 * (int)oLimit;
                this.timer = new Timer(TimeLimitExpired, null, msecLimit, Timeout.Infinite);
            }
        }

        private void TimeLimitExpired(object state)
        {
            this.listener.CancelDecompilation("User-specified time limit has expired.");
        }

        public bool OverridesRequested(Dictionary<string,object> pArgs)
        {
            if (pArgs.ContainsKey("--arch") ||
                pArgs.ContainsKey("--env") ||
                pArgs.ContainsKey("--base") ||
                pArgs.ContainsKey("--entry"))
            {
                // User must supply these arguments for a meaningful
                // decompilation.
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

        private void Decompile(Dictionary<string, object> pArgs)
        {

            pArgs.TryGetValue("--loader", out object loader);
            try
            {
                decompiler.Load((string)pArgs["filename"], (string)loader);
                if (pArgs.TryGetValue("heuristics", out object oHeur))
                {
                    decompiler.Project.Programs[0].User.Heuristics = ((string[])oHeur).ToSortedSet();
                }
                decompiler.ScanPrograms();
                decompiler.AnalyzeDataFlow();
                decompiler.ReconstructTypes();
                decompiler.StructureProgram();
                decompiler.WriteDecompilerProducts();
            }
            catch (Exception ex)
            {
                diagnosticSvc.Error(ex, "An error occurred during decompilation.");
            }
        }

        private void DecompileRawImage(Dictionary<string, object> pArgs)
        {
            try
            {
                var arch = config.GetArchitecture((string)pArgs["--arch"]);
                if (arch == null)
                    throw new ApplicationException(string.Format("Unknown architecture {0}", pArgs["--arch"]));
                string cpuModel = string.Empty;
                if (pArgs.ContainsKey("--cpumodel")) cpuModel = pArgs["--cpumodel"] as string;

                pArgs.TryGetValue("--env", out object sEnv);

                if (!arch.TryParseAddress((string)pArgs["--base"], out Address addrBase))
                    throw new ApplicationException(string.Format("'{0}' doesn't appear to be a valid address.", pArgs["--base"]));
                pArgs.TryGetValue("--entry", out object oAddrEntry);

                pArgs.TryGetValue("--loader", out object sLoader);
                var state = CreateInitialState(arch, pArgs);
                var program = decompiler.LoadRawImage((string)pArgs["filename"], new LoadDetails
                {
                    LoaderName = (string)sLoader,
                    ArchitectureName = (string)pArgs["--arch"],
                    CPUModelName = cpuModel,
                    PlatformName = (string)sEnv,
                    LoadAddress = (string)pArgs["--base"],
                    EntryPoint = new EntryPointElement { Address = (string)oAddrEntry }
                });
                if (pArgs.TryGetValue("heuristics", out object oHeur))
                {
                    decompiler.Project.Programs[0].User.Heuristics = ((string[])oHeur).ToSortedSet();
                }
                decompiler.ScanPrograms();
                decompiler.AnalyzeDataFlow();
                decompiler.ReconstructTypes();
                decompiler.StructureProgram();
                decompiler.WriteDecompilerProducts();
            }
            catch (Exception ex)
            {
                diagnosticSvc.Error(ex, "An error occurred during decompilation.");
            }
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
                else if (arg.StartsWith("--pics"))
                {
                    ShowPICs(w);
                    return null;
                }
                else if (args[i] == "--arch")
                {
                    if (i < args.Length - 1)
                        parsedArgs["--arch"] = args[++i];
                }
                else if (args[i] == "--cpumodel")
                {
                    if (i < args.Length - 1)
                        parsedArgs["--cpumodel"] = args[++i];
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
                else if (args[i] == "-l" || args[i] == "--loader")
                {
                    if (i < args.Length - 1)
                        parsedArgs["--loader"] = args[++i];
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
                    if (i < args.Length-1 && !string.IsNullOrEmpty(args[i+1]))
                    {
                        parsedArgs["heuristics"] = args[i+1].Split(',');
                        ++i;
                    }
                }
                else if (args[i] == "--time-limit")
                {
                    int timeLimit;
                    if (i >= args.Length - 1 || !int.TryParse(args[i+1], out timeLimit))
                    {
                        w.WriteLine("error: time-limit option expects a numerical argument.");
                        return null;
                    }
                    parsedArgs["time-limit"] = timeLimit;
                    ++i;
                }
                else if (arg.StartsWith("-"))
                {
                    w.WriteLine("error: unrecognized option {0}", arg);
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
            w.WriteLine(" --version                Show version number and exit.");
            w.WriteLine(" -h, --help               Show this message and exit.");
            w.WriteLine(" -l, --loader <ldr>       Use a custom loader where <ldr> is either the file");
            w.WriteLine("                          name containing a loader script or the CLR type name");
            w.WriteLine("                          of the loader.");
            w.WriteLine(" --arch <architecture>    Use an architecture from the following:");
            DumpArchitectures(config, w, "    {0,-25} {1}");
            w.WriteLine(" --cpumodel <cpuname>     Use a specific CPU model.");
            w.WriteLine("                          Applicable to 'pic16' and 'pic18' architectures only.");
            w.WriteLine("                          See option '--pics' for a list of PIC models.");
            w.WriteLine(" --pics                   Show a list of supported PIC models and exit.");
            w.WriteLine(" --env <environment>      Use an operating environment from the following:");
            DumpEnvironments(config, w, "    {0,-25} {1}");
            w.WriteLine(" --base <address>         Use <address> as the base address of the program.");
            w.WriteLine(" --default-to <format>    If no executable format can be recognized, default");
            w.WriteLine("                          to one of the following formats:");
            DumpRawFiles(config, w, "    {0,-25} {1}");
            w.WriteLine(" --entry <address>        Use <address> as an entry point to the program.");
            w.WriteLine(" --reg <regInit>          Set register to value, where regInit is formatted as");
            w.WriteLine("                          reg_name:value, e.g. sp:FF00");
            w.WriteLine(" --heuristic <h1>[,<h2>...] Use one of the following heuristics to examine");
            w.WriteLine("                          the binary:");
            w.WriteLine("    shingle               Use shingle assembler to discard data ");
            w.WriteLine(" --time-limit <s>         Limit execution time to s seconds.");
            //           01234567890123456789012345678901234567890123456789012345678901234567890123456789
        }

        private void ShowPICs(TextWriter w)
        {
            picDB = PICCrownking.GetDB();
            if (picDB == null || PICCrownking.LastError != DBErrorCode.NoError)
            {
                Console.WriteLine($"No or wrong PIC database (Error={PICCrownking.LastError}).");
                return;
            }

            int i = 0;
            foreach (var pic in picDB.EnumPICList(p => p.StartsWith("PIC16")))
            {
                if ((i % 5) == 0)
                {
                    i = 0;
                    w.WriteLine();
                }
                w.Write($"{pic,-20}");
                i++;
            }
            w.WriteLine();
            i = 0;
            foreach (var pic in picDB.EnumPICList(p => p.StartsWith("PIC18")))
            {
                if ((i % 5) == 0)
                {
                    i = 0;
                    w.WriteLine();
                }
                w.Write($"{pic,-20}");
                i++;
            }
            w.WriteLine();
        }

        private static void DumpArchitectures(IConfigurationService config, TextWriter w, string fmtString)
        {
            foreach (var arch in config.GetArchitectures()
                .OfType<ArchitectureElement>()
                .OrderBy(a => a.Name))
            {
                w.WriteLine(fmtString, arch.Name, arch.Description);
            }
        }

        private static void DumpEnvironments(IConfigurationService config, TextWriter w, string fmtString)
        {
            foreach (var arch in config.GetEnvironments()
                .OfType<OperatingEnvironmentElement>()
                .OrderBy(a => a.Name))
            {
                w.WriteLine(fmtString, arch.Name, arch.Description);
            }
        }

        private static void DumpRawFiles(IConfigurationService config, TextWriter w, string fmtString)
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
