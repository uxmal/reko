using Reko;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Services;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace Reko.ScannerV2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LoadProgram(args[0]);
        }

        private static void LoadProgram(string filename)
        {
            var sc = new ServiceContainer();
            sc.AddService<IPluginLoaderService>(new PluginLoaderService());
            var fsSvc = new FileSystemServiceImpl();
            sc.AddService<IFileSystemService>(fsSvc);
            var cfgSvc = RekoConfigurationService.Load(sc, "reko/reko.config");
            sc.AddService<IConfigurationService>(cfgSvc);
            var listener = new NullDecompilerEventListener();
            sc.AddService<DecompilerEventListener>(listener);
            sc.AddService<ITypeLibraryLoaderService>(new TypeLibraryLoaderServiceImpl(sc));
            sc.AddService<IDecompiledFileService>(new DecompiledFileService(sc, fsSvc, listener));
            var tgSvc = new TestGenerationService(sc);
            sc.AddService<ITestGenerationService>(tgSvc);
            var ldr = new Reko.Loading.Loader(sc);
            Project project;
            switch (ldr.Load(ImageLocation.FromUri(filename)))
            {
            case Reko.Core.Program image:
                project = Project.FromSingleProgram(image);
                break;
            case Project proj:
                project = proj;
                break;
            default: throw new NotSupportedException();
            }
            var program = project.Programs[0];
            tgSvc.OutputDirectory = program.DisassemblyDirectory;
            Console.WriteLine("= Loaded file ======");
            Console.WriteLine("{0} entry points", program.EntryPoints.Count);
            Console.WriteLine("{0} symbols", program.ImageSymbols.Count);

            var scanner = new RecursiveScanner(program, listener);
            Console.WriteLine("= Recursive scan ======");
            var (cfg, recTime) = Time(() => scanner.ScanProgram());
            Console.WriteLine("Found {0} procs", cfg.Procedures.Count);
            Console.WriteLine("      {0} basic blocks", cfg.Blocks.Count);
            Console.WriteLine("      in {0} msec", (int)recTime.TotalMilliseconds);

            var shScanner = new ShingleScanner(program, cfg, listener);
            Console.WriteLine("= Shingle scan ======");
            var (cfg2, shTime) = Time(() => shScanner.ScanProgram());
            Console.WriteLine("Found {0} procs", cfg2.Procedures.Count);
            Console.WriteLine("    {0} basic blocks", cfg2.Blocks.Count);
            Console.WriteLine("    in {0} msec", (int)shTime.TotalMilliseconds);
            cfg = null;

            Console.WriteLine("= Predecessor edges ======");
            var (cfg3, predTime) = Time(shScanner.RegisterPredecessors);
            Console.WriteLine("Graph has {0} predecessors", cfg3.Predecessors.Count);
            Console.WriteLine("    {0} successors", cfg2.Successors .Count);
            Console.WriteLine("    in {0} msec", (int)predTime.TotalMilliseconds);
            cfg2 = null;

            var procDetector = new ProcedureDetector(program, cfg3, listener);
            Console.WriteLine("= Procedure detector ======");
            var (procs, pdTime) = Time(() => procDetector.DetectProcedures());
            Console.WriteLine("      Found a total of {0} procs", procs.Count);
            Console.WriteLine("      in {0} msec", (int)pdTime.TotalMilliseconds);

            //var dec = new Reko.Decompiler(project, sc);
            //var stopw = new Stopwatch();
            //stopw.Start();
            //dec.ScanPrograms();
            //stopw.Stop();
            //Console.WriteLine("Scanned {0} in {1} msec", filename, stopw.ElapsedMilliseconds);
        }

        private static (T, TimeSpan) Time<T>(Func<T> fn)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = fn();
            stopwatch.Stop();
            return (result, stopwatch.Elapsed);
        }
    }
}
