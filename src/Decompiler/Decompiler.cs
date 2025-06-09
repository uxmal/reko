#region License
/* Copyright (C) 1999-2025 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Graphs;
using Reko.Core.Loading;
using Reko.Core.Output;
using Reko.Core.Scripts;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Loading;
using Reko.Scanning;
using Reko.Services;
using Reko.Structure;
using Reko.Typing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Reko
{
    /// <summary>
    /// The main driver class for decompilation of binaries. 
    /// </summary>
    public class Decompiler : IDecompiler
	{
		private readonly IDecompiledFileService host;
		private IScanner? scanner;
        private readonly IDecompilerEventListener eventListener;
        private readonly IServiceProvider services;

        /// <summary>
        /// Creates an instance of the Reko decompiler.
        /// </summary>
        /// <param name="project">Project to be analyzed.</param>
        /// <param name="services"><see cref="IServiceProvider"/> instance
        /// providing services. </param>
        public Decompiler(Project project, IServiceProvider services)
        {
            this.project = project;
            this.services = services ?? throw new ArgumentNullException(nameof(services));
            this.host = services.RequireService<IDecompiledFileService>();
            this.eventListener = services.RequireService<IDecompilerEventListener>();
            BuildImageMaps();
        }

        /// <summary>
        /// Convenience method that creates a new instance of the decompiler, loading a binary
        /// file into it. 
        /// </summary>
        /// <remarks>
        /// This method is a convenient way to create a new instance of the decompiler,
        /// at the cost of flexibility. To attain more flexibility, e.g. the capability
        /// to override default load address or loading raw binary files that lack
        /// any structure, you must load the binary file separately using the one of the methods on the <see cref="Loader"/> class,
        /// and call the constructor of this class with the resulting <see cref="Core.Project"/> instance.
        /// </remarks>
        /// <param name="filePath">File system path to the binary file or Reko project file to load.</param>
        /// <param name="rekoConfigFilePath">File system path to the reko.config file which contains all
        /// necessary Reko configuration info.
        /// </param>
        public static Decompiler Create(string filePath, string rekoConfigFilePath)
        {
            var services = new ServiceContainer();
            var pluginSvc = new PluginLoaderService();
            services.AddService<IPluginLoaderService>(pluginSvc);
            var fsSvc = new FileSystemService();
            services.AddService<IFileSystemService>(fsSvc);
            var eventListener = new NullDecompilerEventListener();
            services.AddService<IDecompilerEventListener>(eventListener);
            services.AddService<IEventListener>(eventListener);
            var configSvc = RekoConfigurationService.Load(services, rekoConfigFilePath);
            services.AddService<IConfigurationService>(configSvc);
            var typelibSvc = new TypeLibraryLoaderServiceImpl(services);
            services.AddService<ITypeLibraryLoaderService>(typelibSvc);
            var decFileSvc = new DecompiledFileService(services, fsSvc, eventListener);
            services.AddService<IDecompiledFileService>(decFileSvc);

            var loader = new Loader(services);
            var input = loader.Load(ImageLocation.FromUri(filePath));
            if (input is not Project project)
            {
                if (input is not Program program)
                    throw new InvalidOperationException(
                        "The input file is not a Reko project or a recognized binary file. " +
                        "Consider loading the file separately using one of the methods of " +
                        "Reko.Loading.Loader.");
                project = Project.FromSingleProgram(program);
            }
            return new Decompiler(project, services);
        }

        /// <summary>
        /// The Reko project being analyzed.
        /// </summary>
        public Project Project { get { return project; } set { project = value; ProjectChanged?.Invoke(this, EventArgs.Empty); } }
        public event EventHandler? ProjectChanged;
        private Project project;

        /// <summary>
        /// Main entry point of the decompiler. Decompiles, and outputs the results.
        /// </summary>
        public void Decompile()
        {
            try
            {
                ExtractResources();
                ScanPrograms();
                AnalyzeDataFlow();
                ReconstructTypes();
                StructureProgram();
                WriteDecompilerProducts();
            }
            catch (Exception ex)
            {
                eventListener.Error(
                    new NullCodeLocation(""),
                    ex,
                    "An internal error occurred while decompiling.");
            }
            finally
            {
                eventListener.Progress.ShowStatus("Decompilation finished.");
            }
        }

        ///<summary>
        /// Determines the signature of the procedures,
		/// the locations and types of all the values in the program.
		///</summary>
        public virtual void AnalyzeDataFlow()
        {
            if (project is null)
                return;
            var eventListener = services.RequireService<IDecompilerEventListener>();
            foreach (var program in project.Programs)
            {
                try
                {
                    if (eventListener.IsCanceled())
                        return;
                    var ir = new DynamicLinker(project, program, eventListener);
                    var dfa = new DataFlowAnalysis(program, ir, services);
                    dfa.ClearTestFiles();
                    IReadOnlyCollection<SsaTransform> ssas = new List<SsaTransform>();
                    if (program.NeedsSsaTransform)
                    {
                        eventListener.Progress.ShowStatus("Performing interprocedural analysis.");
                        ssas = dfa.UntangleProcedures();
                    }
                    eventListener.Progress.ShowStatus("Building complex expressions.");
                    dfa.BuildExpressionTrees(ssas);
                    host.WriteIntermediateCode(program, (name, procs, writer) => { EmitProgram(program, procs, dfa, writer); });
                }
                catch (Exception ex)
                {
                    eventListener.Error(
                        new NullCodeLocation(program.Location.FilesystemPath),
                        ex,
                        "An internal error occurred while analyzing {0}.",
                        program.Location);
                }
            }
            eventListener.Progress.ShowStatus("Interprocedural analysis complete.");
        }

        /// <summary>
        /// Writes the disassembly of the program to one or more files.
        /// Each segment of the binary program generates one more output files.
        /// </summary>
        /// <param name="program">Program to disassemble.</param>
        /// <param name="segmentItems">A dictionary mapping each <see cref="ImageSegment"/>
        /// to a list of <see cref="ImageMapItem"/>s discovered in that segment.
        /// </param>
        /// <param name="wr">Object responsible for formatting the output files.
        /// </param>
        public void DumpAssembler(
            Program program, 
            Dictionary<ImageSegment, List<ImageMapItem>> segmentItems,
            Formatter wr)
        {
            if (wr is null || program.Architecture is null)
                return;
            try
            {
                Dumper dump = new Dumper(program)
                {
                    ShowAddresses = program.User.ShowAddressesInDisassembly,
                    ShowCodeBytes = program.User.ShowBytesInDisassembly,
                    RenderInstructionsCanonically = program.User.RenderInstructionsCanonically,
                };
                dump.Dump(segmentItems, wr);
            } 
            catch (Exception ex)
            {
                eventListener.Error(ex, "An error occurred while writing assembly language output.");
            }
        }

        private void EmitProgram(
            Program program, 
            IEnumerable<object> objects,
            DataFlowAnalysis? dfa,
            TextWriter output)
        {
            if (output is null)
                return;
            foreach (object o in objects)
            {
                if (o is Procedure proc)
                {
                    if (program.NeedsSsaTransform && dfa is not null)
                    {
                        ProcedureFlow flow = dfa.ProgramDataFlow[proc];
                        TextFormatter f = new TextFormatter(output);
                        CodeFormatter cf = new CodeFormatter(f);
                        var signature = flow.Signature ?? proc.Signature;
                        signature.Emit(proc.Name, FunctionType.EmitFlags.LowLevelInfo, f);
                        WriteProcedureCallers(program, proc, output);
                        flow.Emit(proc.Architecture, output);
                        new ProcedureFormatter(proc, new BlockDecorator(), cf).WriteProcedureBlocks();

                        /// For debugging
                        //output.WriteLine();
                        //output.WriteLine("/*");
                        //var mockWriter = new MockGenerator(output, "m.");
                        //mockWriter.WriteMethod(proc);
                        //output.WriteLine("*/");
                        //output.WriteLine();
                    }
                    else
                    {
                        proc.Write(false, output);
                    }
                }
                output.WriteLine();
                output.WriteLine();
            }
            output.Flush();
        }

        /// <summary>
        /// Build image maps for each program in preparation of the scanning
        /// phase.
        /// </summary>
        private void BuildImageMaps()
        {
            if (this.Project is null)
                return;
            foreach (var program in this.Project.Programs)
            {
                program.BuildImageMap();
            }
        }

        /// <summary>
        /// Extracts embedded resources from all programs in the project.
        /// </summary>
        public void ExtractResources()
        {
            if (project is null)
                return;
            foreach (var program in project.Programs)
            {
                if (program.User.ExtractResources)
                {
                    ExtractResources(program);
                }
            }
        }

        /// <summary>
        /// Extracts embedded resources from the given program.
        /// </summary>
        /// <param name="program">Program file from which to extract resources.
        /// </param>
        public void ExtractResources(Program program)
        { 
            var prg = program.Resources;
            if (prg.Count == 0)
                return;
            var fsSvc = services.RequireService<IFileSystemService>();
            var resourceDir = program.ResourcesDirectory;
            try
            {
                fsSvc.CreateDirectory(resourceDir);
            }
            catch (Exception ex)
            {
                eventListener.Error(ex, $"Unable to create directory '{resourceDir}'.");
                return;
            }
            foreach (var resource in program.Resources)
            {
                WriteResource(resource, fsSvc, resourceDir, "", null!);
            }
        }

        private bool WriteResource(
            ProgramResource? resource,
            IFileSystemService fsSvc,
            string outputDir,
            string resourceType,
            ProgramResourceGroup pr)
        {
            if (resource is ProgramResourceGroup grp)
            {
                outputDir = Path.Combine(outputDir, grp.Name!);
                try
                {
                    fsSvc.CreateDirectory(outputDir);
                }
                catch (Exception ex)
                {
                    eventListener.Error(ex, $"Unable to create directory '{outputDir}'.");
                    return false;
                }
                foreach (var res in grp.Resources)
                {
                    if (!WriteResource(res, fsSvc, outputDir, "", null!))
                        return false;
                }
                return true;
            }
            else if (resource is ProgramResourceInstance pr2)
            {
                string path = "";
                try
                {
                    if (pr2.Bytes is not null)
                    {
                        var filename = $"{pr2.Type}_{pr2.Name}{pr2.FileExtension}";
                        path = Path.Combine(outputDir, filename);
                        fsSvc.WriteAllBytes(path, pr2.Bytes);
                    }
                }
                catch (Exception ex)
                {
                    eventListener.Error(ex, $"Unable to write file '{path}'.");
                    return false;
                }
                return true;
            }
            return true;
        }

        /// <summary>
        /// Extracts type information from the typeless rewritten programs.
        /// </summary>
        public void ReconstructTypes()
        {
            if (Project is null)
                return;
            foreach (var program in Project.Programs.Where(p => p.NeedsTypeReconstruction))
            {
                var analyzer = new TypeAnalyzer(eventListener);
                try
                {
                    analyzer.RewriteProgram(program);
                }
                catch (Exception ex)
                {
                    eventListener.Error(
                        new NullCodeLocation(program.Location.FilesystemPath),
                        ex,
                        "An internal error occurred while reconstructing data types in {0}.",
                        program.Location);
                }
                finally
                {
                    host.WriteDeclarations(program, analyzer.WriteTypes);
                }
            }
        }

        /// <summary>
        /// Writes the high-level language decompiled objects to a file.
        /// </summary>
        /// <param name="program">Program that was decompiled.</param>
        /// <param name="filename">The filename of the written file.</param>
        /// <param name="objects">Objects that are to be written.</param>
        /// <param name="w">Formatter object that converts the object to text.
        /// </param>
        public void WriteDecompiledObjects(
            Program program,
            string filename, 
            IEnumerable<IAddressable> objects,
            TextWriter w)
        {
            WriteHeaderComment(filename, program, w);
            //$REFACTOR: common code -- hardwired ".h"
            var headerfile = DecompiledFileService.GenerateDerivedFilename(program, ".h");
            w.WriteLine("#include \"{0}\"", headerfile);
            w.WriteLine();
            var fmt = new AbsynCodeFormatter(new TextFormatter(w));
            var gdw = new GlobalDataWriter(program, fmt.InnerFormatter, false, true, this.services);
            IAddressable? prev = null;
            foreach (var o in objects)
            {
                if (o is Procedure proc)
                {
                    try
                    {
                        if (prev is GlobalVariable)
                            w.WriteLine();
                        WriteProcedureHeader(program, proc, w);
                        fmt.Write(proc);
                        w.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        w.WriteLine();
                        w.WriteLine("// Exception {0} when writing procedure.", ex.Message);
                    }
                }
                else if (o is GlobalVariable global)
                {
                    gdw.WriteGlobalVariable(global.Address, global.DataType, global.Name);
                }
            }
        }

        /// <summary>
        /// Writes the high-level language procedure header for procedure <paramref name="proc"/>.
        /// </summary>
        /// <remarks>
        /// //$REFACTOR: this is using C++-style comments. This should be in a HLL-specific class.
        /// </remarks>
        /// <param name="program">Program in which <paramref name="proc"/> is located.</param>
        /// <param name="proc">Procedure whose heder is to rendered.</param>
        /// <param name="w"><see cref="TextWriter"/> into which the output is written.
        /// </param>
        private void WriteProcedureHeader(Program program, Procedure proc, TextWriter w)
        {
            w.WriteLine("// {0}: {1}", proc.EntryAddress, proc);
            WriteProcedureCallers(program, proc, w);
        }

        /// <summary>
        /// Writes the names of the procedures calling the procedure <paramref name="proc" />.
        /// </summary>
        private static void WriteProcedureCallers(Program program, Procedure proc, TextWriter w)
        {
            var callers = program.CallGraph.CallerProcedures(proc)
                .Distinct()
                .OrderBy(p => p.EntryAddress)
                .ToArray();
            if (callers.Length > 0)
            {
                w.WriteLine("// Called from:");
                foreach (var caller in callers)
                {
                    w.WriteLine("//      {0}", caller.Name);
                }
            }
        }

        /// <summary>
        /// Writes all detected global variables to a file.
        /// </summary>
        /// <param name="program">Program whose globals are to be written.</param>
        /// <param name="filename">The name of the source file receiving the written globals.</param>
        /// <param name="w">Formatting object that converts data globals to text.</param>
        public void WriteGlobals(Program program, string filename, TextWriter w)
        {
            var headerfile = DecompiledFileService.GenerateDerivedFilename(program, ".h");
            WriteHeaderComment(filename, program, w);
            w.WriteLine("#include \"{0}\"", headerfile);
            w.WriteLine();
            var gdw = new GlobalDataWriter(program, new TextFormatter(w), true, true, services);
            gdw.Write();
            w.WriteLine();
        }
    
        public void WriteDecompiledDeclarations(Program program, string headerFilename, TextWriter w)
        {
            WriteHeaderComment(headerFilename, program, w);
            w.WriteLine("/*"); program.TypeStore.Write(true, w); w.WriteLine("*/");
            var tf = new TextFormatter(w)
            {
                Indentation = 0,
            };
            var fmt = new TypeFormatter(tf);
            foreach (EquivalenceClass eq in program.TypeStore.UsedEquivalenceClasses)
            {
                if (eq.DataType is not null)
                {
                    tf.WriteKeyword("typedef");     //$REVIEW: C/C++-specific
                    tf.Write(" ");
                    fmt.Write(eq.DataType, eq.Name);
                    w.WriteLine(";");
                    w.WriteLine();
                }
            }
            WriteExternalProcedures(program, w);
        }

        public static void WriteExternalProcedures(
            Program program, TextWriter w)
        {
            var externalProcedures = program.ExternalProcedures.OrderBy(
                p => (
                    p.Value.Item1,
                    p.Value.Item3.EnclosingType?.ToString(),
                    p.Value.Item3.Name)
            ).ToList();
            if (externalProcedures.Count == 0)
                return;
            w.WriteLine("// Declarations for external procedures");
            w.WriteLine();
            foreach (var (importName, (dllName, convention, ep)) in externalProcedures)
            {
                var qualifiedName = ep.QualifiedName();
                if (!string.IsNullOrEmpty(convention))
                {
                    qualifiedName = $"{convention} {qualifiedName}";
                }
                ep.Signature.Emit(
                    qualifiedName,
                    FunctionType.EmitFlags.None,
                    w);
                string comment;
                if (!string.IsNullOrEmpty(dllName))
                {
                    comment = $"{dllName}!{importName}";
                }
                else
                {
                    comment = importName;
                }
                w.WriteLine($"; // {comment}");
            }
        }

        /// <summary>
        /// Starts a scan at address <paramref name="paddr"/> on the user's request.
        /// </summary>
        /// <param name="paddr"></param>
        /// <returns>a ProcedureBase, because the target procedure may have been a thunk or 
        /// an linked procedure the user has decreed not decompileable.</returns>
        public ProcedureBase ScanProcedure(ProgramAddress paddr, IProcessorArchitecture arch)
        {
            var program = paddr.Program;
                //$TODO: it's unfortunate that we depend on the scanner of the Decompiler class.
            if (scanner is null)       
                scanner = CreateScanner(program);
            var procName = program.User.Procedures.TryGetValue(
                paddr.Address, out var sProc) ? sProc.Name : null;
            return scanner.ScanProcedure(
                arch,
                paddr.Address,
                procName, 
                program.Architecture.CreateProcessorState());
        }

		/// <summary>
		/// Generates the control flow graph and finds executable code in each program.
		/// </summary>
		/// <param name="program">the program whose flow graph we seek</param>
		/// <param name="cfg">configuration information</param>
		public void ScanPrograms()
		{
			if (Project is null || Project.Programs.Count == 0)
				throw new InvalidOperationException("Programs must be loaded first.");
            Project.FireScriptEvent(ScriptEvent.OnProgramDecompiling);
            foreach (Program program in Project.Programs)
            {
                try
                {
                    ScanProgram(program);
                } 
                catch (Exception ex)
                {
                    eventListener.Error(
                        new NullCodeLocation(program.Location.FilesystemPath),
                        ex,
                        "An internal error occurred while scanning {0}.",
                        program.Location);
                }
            }
            Project.FireScriptEvent(ScriptEvent.OnProgramScanned);
        }

        private void ScanProgram(Program program)
        {
            try
            {
                if (program.NeedsScanning)
                {
                    eventListener.Progress.ShowStatus("Rewriting reachable machine code.");
                    scanner = CreateScanner(program);
                    scanner.ScanImage();
                    eventListener.Progress.ShowStatus("Finished rewriting reachable machine code.");
                }
            }
            finally
            {
                eventListener.Progress.ShowStatus("Writing .asm and .dis files.");
                host.WriteDisassembly(program, (n, items, w) => DumpAssembler(program, items, w));
                host.WriteIntermediateCode(program, (n, procs, w) => EmitProgram(program, procs, null, w));
                // Uncomment the following for debugging.
                // WriteSccs(program);
            }
        }

        //$REVIEW: doesn't seem to be used anywhere?
        public IDictionary<Address, FunctionType> LoadCallSignatures(
            Program program,
            ICollection<SerializedCall_v1> userCalls)
        {
            return
                userCalls
                .Where(sc => sc is not null && sc.Signature is not null)
                .Select(sc =>
                {
                //$BUG: need access to platform.Metadata.
                    var sser = program.CreateProcedureSerializer();
                    if (program.Architecture.TryParseAddress(sc.InstructionAddress, out var addr))
                    {
                        return new KeyValuePair<Address, FunctionType?>(
                            addr,
                            sser.Deserialize(sc.Signature, program.Architecture.CreateFrame()));
                    }
                    else
                        return new KeyValuePair<Address, FunctionType?>(default, null);
                })
                .Where(item => item.Value is not null)
                .ToDictionary(item => item.Key!, item => item.Value!);
        }

        private IScanner CreateScanner(Program program)
        {
#if NEW_SCANNER
            return new Scanner2(
                program,
                project!.LoadedMetadata,
                new DynamicLinker(project!, program, eventListener),
                services);
#else
            return new Scanner(
                program,
                project!.LoadedMetadata,
                new DynamicLinker(project!, program, eventListener),
                services);
#endif
        }

        /// <summary>
        /// Extracts structured program constructs out of snarled goto nests, if possible.
        /// Since procedures are now independent of each other, this analysis
        /// is done one procedure at a time.
        /// </summary>
        public void StructureProgram()
		{
            if (project is null)
                return;
            foreach (var program in project.Programs)
            {
                int i = 0;
                foreach (Procedure proc in program.Procedures.Values)
                {
                    if (eventListener.IsCanceled())
                        return;
                    try
                    {
                        eventListener.Progress.ShowProgress("Rewriting procedures to high-level language.", i, program.Procedures.Values.Count);
                        ++i;
                        IStructureAnalysis sa = new StructureAnalysis(eventListener, program, proc);
                        sa.Structure();
                    }
                    catch (Exception e)
                    {
                        eventListener.Error(
                            eventListener.CreateProcedureNavigator(program, proc),
                            e,
                            "An error occurred while rewriting procedure to high-level language.");
                    }
                }
            }
            project.FireScriptEvent(ScriptEvent.OnProgramDecompiled);
            WriteDecompilerProducts();
            eventListener.Progress.ShowStatus("Rewriting complete.");
		}

		public void WriteDecompilerProducts()
		{
            if (Project is null)
                return;
            foreach (var program in Project.Programs)
            {
                host.WriteDeclarations(program, (n, w) => WriteDecompiledDeclarations(program, n, w));
                host.WriteDecompiledCode(program, (n, p, w) => WriteDecompiledObjects(program, n, p, w));
            }
		}

        private void WriteEntryPoints()
        {
            if (Project is null)
                return;
            foreach (var program in Project.Programs)
            {
                WriteEntryPoints(program);
            }
        }

        private void WriteEntryPoints(Program program)
        {
            if (program.Platform is null)
                return;
            var irPath = Path.Combine(program.SourceDirectory, Path.GetFileNameWithoutExtension(program.Location.FilesystemPath));
            program.Platform.WriteMetadata(program, irPath);
        }

		public void WriteHeaderComment(string filename, Program program, TextWriter w)
		{
			w.WriteLine("// {0}", filename);
			w.WriteLine("// Generated by decompiling {0}", program.Location.GetFilename());
			w.WriteLine("// using Reko decompiler version {0}.", AssemblyMetadata.AssemblyFileVersion);
			w.WriteLine();
		}

        /// <summary>
        /// Writes the strongly-connected components (SCCs) of the given <see cref="Program"/>'s 
        /// call graph.
        /// </summary>
        /// <remarks>
        /// The entries are reverse depth first ordered, which means leaf procedures are written
        /// before their callers. The intent is to assist in debugging large program with deep
        /// call hierarchies.
        /// </remarks>
        public void WriteSccs(Program program)
        {
            var filename = Path.ChangeExtension(program.Location.GetFilename(), "sccs");
            var globalsPath = Path.Combine(program.SourceDirectory, filename);
            using (TextWriter output = host.CreateTextWriter(globalsPath))
            {
                var sccs = SccFinder.FindAll(new ProcedureGraph(program));
                foreach (var scc in sccs)
                {
                    output.WriteLine("== {0} procedures ===", scc.Length);
                    var procs = scc.OrderBy(p => p.EntryAddress).ToList();
                    foreach (var proc in procs)
                    {
                        output.Write(proc.Name);
                        if (program.EntryPoints.ContainsKey(proc.EntryAddress))
                            output.Write(" (Entry point)");
                        output.WriteLine();
                    }
                }
            }
        }

        public void ReplaceProgram(Program oldProgram, Program newProgram)
        {
            var programs = Project.Programs;
            bool change = false;
            for (int i = 0; i < programs.Count; ++i)
            {
                if (programs[i] == oldProgram)
                {
                    programs[i] = newProgram;
                    change = true;
                }
            }
            if (change) ProjectChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
