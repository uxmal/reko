#region License
/* Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Assemblers;
using Reko.Core.Configuration;
using Reko.Core.Lib;
using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Scanning;
using Reko.Structure;
using Reko.Typing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Reko
{
    public interface IDecompiler
    {
        Project Project { get; }

        bool Load(string fileName, string loader=null);
        Program LoadRawImage(string file, LoadDetails raw);
        Program LoadRawImage(byte[] bytes, LoadDetails raw);
        void ScanPrograms();
        ProcedureBase ScanProcedure(ProgramAddress paddr, IProcessorArchitecture arch);
        void AnalyzeDataFlow();
        void ReconstructTypes();
        void StructureProgram();
        void WriteDecompilerProducts();
        void ExtractResources();

        void Assemble(string file, Assembler asm);
    }

	/// <summary>
	/// The main driver class for decompilation of binaries. 
	/// </summary>
	public class Decompiler : IDecompiler
	{
		private IDecompiledFileService host;
		private readonly ILoader loader;
		private IScanner scanner;
        private DecompilerEventListener eventListener;
        private IServiceProvider services;

        public Decompiler(ILoader ldr, IServiceProvider services)
        {
            this.loader = ldr ?? throw new ArgumentNullException("ldr");
            this.services = services ?? throw new ArgumentNullException("services");
            this.host = services.RequireService<IDecompiledFileService>();
            this.eventListener = services.RequireService<DecompilerEventListener>();
        }

        public Project Project { get { return project; } set { project = value; ProjectChanged.Fire(this); } }
        public event EventHandler ProjectChanged;
        private Project project;

        /// <summary>
        /// Main entry point of the decompiler. Loads, decompiles, and outputs the results.
        /// </summary>
        public void Decompile(string filename, string loader = null)
        {
            try
            {
                Load(filename, loader);
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
                    new NullCodeLocation(filename),
                    ex,
                    "An internal error occurred while decompiling.");
            }
            finally
            {
                eventListener.ShowStatus("Decompilation finished.");
            }
        }

        ///<summary>
        /// Determines the signature of the procedures,
		/// the locations and types of all the values in the program.
		///</summary>
        public virtual void AnalyzeDataFlow()
        {
            var eventListener = services.RequireService<DecompilerEventListener>();
            foreach (var program in project.Programs)
            {
                if (eventListener.IsCanceled())
                    return;
                var ir = new DynamicLinker(project, program, eventListener);
                var dfa = new DataFlowAnalysis(program, ir, eventListener);
                if (program.NeedsSsaTransform)
                {
                    eventListener.ShowStatus("Performing interprocedural analysis.");
                    dfa.UntangleProcedures();
                }
                eventListener.ShowStatus("Building complex expressions.");
                dfa.BuildExpressionTrees();
                host.WriteIntermediateCode(program, (name, writer) => { EmitProgram(program, dfa, name, writer); });
            }
            eventListener.ShowStatus("Interprocedural analysis complete.");
        }

        public void DumpAssembler(Program program, string filename, Formatter wr)
        {
            if (wr == null || program.Architecture == null)
                return;
            Dumper dump = new Dumper(program)
            {
                ShowAddresses = program.User.ShowAddressesInDisassembly,
                ShowCodeBytes = program.User.ShowBytesInDisassembly
            };
            dump.Dump(wr);
        }

        private void EmitProgram(Program program, DataFlowAnalysis dfa, string filename, TextWriter output)
        {
            if (output == null)
                return;
            foreach (Procedure proc in program.Procedures.Values)
            {
                if (program.NeedsSsaTransform && dfa != null)
                {
                    ProcedureFlow flow = dfa.ProgramDataFlow[proc];
                    TextFormatter f = new TextFormatter(output);
                    if (flow.Signature != null)
                        flow.Signature.Emit(proc.Name, FunctionType.EmitFlags.LowLevelInfo, f);
                    else
                        proc.Signature.Emit(proc.Name, FunctionType.EmitFlags.LowLevelInfo, f);
                    output.WriteLine();
                    flow.Emit(program.Architecture, output);
                    foreach (Block block in new DfsIterator<Block>(proc.ControlGraph).PostOrder().Reverse())
                    {
                        if (block == null)
                            continue;
                        block.Write(output);
                    }
                }
                else
                {
                    proc.Write(false, output);
                }
                output.WriteLine();
                output.WriteLine();
            }
            output.Flush();
        }

        /// <summary>
        /// Loads (or assembles) the decompiler project. If a binary file is
        /// specified instead, we create a simple project for the file.
        /// </summary>
        /// <param name="fileName">The filename to load.</param>
        /// <param name="loaderName">Optional .NET class name of a custom
        /// image loader</param>
        /// <returns>True if the file could be loaded.</returns>
        public bool Load(string fileName, string loaderName = null)
        {
            eventListener.ShowStatus("Loading source program.");
            byte[] image = loader.LoadImageBytes(fileName, 0);
            var projectLoader = new ProjectLoader(this.services, loader, eventListener);
            projectLoader.ProgramLoaded += (s, e) => { RunScriptOnProgramImage(e.Program, e.Program.User.OnLoadedScript); };
            this.Project = projectLoader.LoadProject(fileName, image);
            if (Project == null)
            {
                var program = loader.LoadExecutable(fileName, image, loaderName, null);
                if (program == null)
                    return false;
                this.Project = AddProgramToProject(fileName, program);
                this.Project.LoadedMetadata = program.Platform.CreateMetadata();
                program.EnvironmentMetadata = this.Project.LoadedMetadata;
            }
            BuildImageMaps();
            eventListener.ShowStatus("Source program loaded.");
            return true;
        }

        /// <summary>
        /// Build image maps for each program in preparation of the scanning
        /// phase.
        /// </summary>
        private void BuildImageMaps()
        {
            foreach (var program in this.Project.Programs)
            {
                program.BuildImageMap();
            }
        }

        public void RunScriptOnProgramImage(Program program, Script_v2 script)
        {
            if (script == null || !script.Enabled)
                return;
            IScriptInterpreter interpreter;
            try
            {
                //$TODO: should be in the config file, yeah.
                var type = Type.GetType("Reko.ImageLoaders.OdbgScript.OllyLang,Reko.ImageLoaders.OdbgScript");
                interpreter = (IScriptInterpreter)Activator.CreateInstance(type, services);
            }
            catch (Exception ex)
            {
                eventListener.Error(new NullCodeLocation(""), ex, "Unable to load OllyLang script interpreter.");
                return;
            }

            try
            {
                interpreter.LoadFromString(script.Script, null);
                interpreter.Run();
            }
            catch (Exception ex)
            {
                eventListener.Error(new NullCodeLocation(""), ex, "An error occurred while running the script.");
            }
        }

        public void Assemble(string fileName, Assembler asm)
        {
            eventListener.ShowStatus("Assembling program.");
            var program = loader.AssembleExecutable(fileName, asm, null);
            Project = AddProgramToProject(fileName, program);
            eventListener.ShowStatus("Assembled program.");
        }

        /// <summary>
        /// Loads a program from a file into memory using the additional information in 
        /// <paramref name="raw"/>. Use this to open files with insufficient or
        /// no metadata.
        /// </summary>
        /// <param name="fileName">Name of the file to be loaded.</param>
        /// <param name="raw">Extra metadata supllied by the user.</param>
        public Program LoadRawImage(string fileName, LoadDetails raw)
        {
            eventListener.ShowStatus("Loading raw bytes.");
            raw.ArchitectureOptions = raw.ArchitectureOptions ?? new Dictionary<string, object>();
            byte[] image = loader.LoadImageBytes(fileName, 0);
            var program = loader.LoadRawImage(fileName, image, null, raw);
            Project = AddProgramToProject(fileName, program);
            eventListener.ShowStatus("Raw bytes loaded.");
            return program;
        }

        /// <summary>
        /// Loads a program into memory using the additional information in 
        /// <paramref name="raw"/>. Use this to decompile raw blobs of data.
        /// </summary>
        /// <param name="fileName">Name of the file to be loaded.</param>
        /// <param name="raw">Extra metadata supllied by the user.</param>
        public Program LoadRawImage(byte[] image, LoadDetails raw)
        {
            eventListener.ShowStatus("Loading raw bytes.");
            raw.ArchitectureOptions = raw.ArchitectureOptions ?? new Dictionary<string, object>();
            var program = loader.LoadRawImage("image", image, null, raw);
            Project = AddProgramToProject("image", program);
            eventListener.ShowStatus("Raw bytes loaded.");
            return program;
        }

        protected Project AddProgramToProject(string fileNameWithPath, Program program)
        {
            if (this.project == null)
            {
                this.project = new Project();
            }
            program.Filename = fileNameWithPath;
            program.EnsureDirectoryNames(fileNameWithPath);
            program.User.ExtractResources = true;

            project.Programs.Add(program);
            return project;
        }

        public void ExtractResources()
        {
            foreach (var program in project.Programs)
            {
                if (program.User.ExtractResources)
                {
                    ExtractResources(program);
                }
            }
        }

        public void ExtractResources(Program program)
        {
            var prg = program.Resources;
            if (prg == null)
                return;
            var fsSvc = services.RequireService<IFileSystemService>();
            var resourceDir = program.ResourcesDirectory;
            if (prg.Name == "PE resources")
            {
                try
                {
                    fsSvc.CreateDirectory(resourceDir);
                }
                catch (Exception ex)
                {
                    var diagSvc = services.RequireService<IDiagnosticsService>();
                    diagSvc.Error(ex, $"Unable to create directory '{0}'.");
                    return;
                }
                foreach (ProgramResourceGroup pr in prg.Resources)
                {
                    switch (pr.Name)
                    {
                    case "CURSOR":
                        {
                            if (!WriteResourceFile(fsSvc, resourceDir, "Cursor", ".cur", pr))
                                return;
                        }
                        break;
                    case "BITMAP":
                        {
                            if (!WriteResourceFile(fsSvc, resourceDir, "Bitmap", ".bmp", pr))
                                return;
                        }
                        break;
                    case "ICON":
                        {
                            if (!WriteResourceFile(fsSvc, resourceDir, "Icon", ".ico", pr))
                                return;
                        }
                        break;
                    case "FONT":
                        {
                            if (!WriteResourceFile(fsSvc, resourceDir, "Font", ".bin", pr))
                                return;
                        }
                        break;
                    case "NEWBITMAP":
                        {
                            if (!WriteResourceFile(fsSvc, resourceDir, "NewBitmap", ".bmp", pr))
                                return;
                        }
                        break;

                    default:
                        break;
                    }
                }
            }
        }

        private bool WriteResourceFile(IFileSystemService fsSvc, string outputDir, string ResourceType, string ext, ProgramResourceGroup pr)
        {
            var dirPath = Path.Combine(outputDir, ResourceType);
            try
            {
                fsSvc.CreateDirectory(dirPath);
            }
            catch (Exception ex)
            {
                var diagSvc = services.RequireService<IDiagnosticsService>();
                diagSvc.Error(ex, $"Unable to create directory '{dirPath}'.");
                return false;
            }
            string path = "";
            try
            {
                foreach (ProgramResourceGroup pr1 in pr.Resources)
                {
                    foreach (ProgramResourceInstance pr2 in pr1.Resources)
                    {
                        path = Path.Combine(dirPath, pr1.Name + ext);
                        fsSvc.WriteAllBytes(path, pr2.Bytes);
                    }
                }
            }
            catch (Exception ex)
            {
                var diagSvc = services.RequireService<IDiagnosticsService>();
                diagSvc.Error(ex, $"Unable to write file '{path}'");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Extracts type information from the typeless rewritten programs.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="ivs"></param>
        public void ReconstructTypes()
        {
            foreach (var program in Project.Programs.Where(p => p.NeedsTypeReconstruction))
            {
                var analyzer = new TypeAnalyzer(eventListener);
                try
                {
                    analyzer.RewriteProgram(program);
                }
                catch (Exception ex)
                {
                    eventListener.Error(new NullCodeLocation(""), ex, "Error when reconstructing types.");
                } 
                finally
                {
                    host.WriteTypes(program, analyzer.WriteTypes);
                }
            }
        }

        public void WriteDecompiledProcedures(Program program, string filename, IEnumerable<Procedure> procs, TextWriter w)
        {
            var headerfile = Path.ChangeExtension(filename, ".h");
            WriteHeaderComment(filename, program, w);
            w.WriteLine("#include \"{0}\"", headerfile);
            w.WriteLine();
            var fmt = new AbsynCodeFormatter(new TextFormatter(w));
            foreach (var proc in procs)
            {
                w.WriteLine("// {0}: {1}", proc.EntryAddress, proc);
                try
                {
                    fmt.Write(proc);
                    w.WriteLine();
                }
                catch (Exception ex)
                {
                    w.WriteLine();
                    w.WriteLine("// Exception {0} when writing procedure.", ex.Message);
                }
            }
        }

        public void WriteGlobals(Program program, string filename, TextWriter w)
        {
            var headerfile = Path.ChangeExtension(Path.GetFileName(program.Filename), ".h");
            WriteHeaderComment(filename, program, w);
            w.WriteLine("#include \"{0}\"", headerfile);
            w.WriteLine();
            var gdw = new GlobalDataWriter(program, services);
            gdw.WriteGlobals(new TextFormatter(w));
            w.WriteLine();
        }
    
        public void WriteDecompiledTypes(Program program, string headerFilename, TextWriter w)
        {
            WriteHeaderComment(headerFilename, program, w);
            w.WriteLine("/*"); program.TypeStore.Write(w); w.WriteLine("*/");
            var tf = new TextFormatter(w)
            {
                Indentation = 0,
            };
            var fmt = new TypeFormatter(tf);
            foreach (EquivalenceClass eq in program.TypeStore.UsedEquivalenceClasses)
            {
                if (eq.DataType != null)
                {
                    tf.WriteKeyword("typedef");     //$REVIEW: C/C++-specific
                    tf.Write(" ");
                    fmt.Write(eq.DataType, eq.Name);
                    w.WriteLine(";");
                    w.WriteLine();
                }
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
            if (scanner == null)        //$TODO: it's unfortunate that we depend on the scanner of the Decompiler class.
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
			if (Project.Programs.Count == 0)
				throw new InvalidOperationException("Programs must be loaded first.");

            foreach (Program program in Project.Programs)
            {
                ScanProgram(program);
            }
		}

        private void ScanProgram(Program program)
        {
            if (!program.NeedsScanning)
                return;
            try
            {
                eventListener.ShowStatus("Rewriting reachable machine code.");
                scanner = CreateScanner(program);
                scanner.ScanImage();
                eventListener.ShowStatus("Finished rewriting reachable machine code.");
            }
            finally
            {
                eventListener.ShowStatus("Writing .asm and .dis files.");
                host.WriteDisassembly(program, (n, w) => DumpAssembler(program, n, w));
                host.WriteIntermediateCode(program, (n, w) => EmitProgram(program, null, n, w));
            }
        }

        public IDictionary<Address, FunctionType> LoadCallSignatures(
            Program program, 
            ICollection<SerializedCall_v1> userCalls)
        {
            return
                userCalls
                .Where(sc => sc != null && sc.Signature != null)
                .Select(sc =>
                {
                //$BUG: need access to platform.Metadata.
                    var sser = program.CreateProcedureSerializer();
                    if (program.Architecture.TryParseAddress(sc.InstructionAddress, out var addr))
                    {
                        return new KeyValuePair<Address, FunctionType>(
                            addr,
                            sser.Deserialize(sc.Signature, program.Architecture.CreateFrame()));
                    }
                    else
                        return new KeyValuePair<Address, FunctionType>(null, null);
                })
                .ToDictionary(item => item.Key, item => item.Value);
        }

        private IScanner CreateScanner(Program program)
        {
            return new Scanner(
                program,
                new DynamicLinker(project, program, eventListener),
                services);
        }

        /// <summary>
        /// Extracts structured program constructs out of snarled goto nests, if possible.
        /// Since procedures are now independent of each other, this analysis
        /// is done one procedure at a time.
        /// </summary>
        public void StructureProgram()
		{
            foreach (var program in project.Programs)
            {
                int i = 0;
                foreach (Procedure proc in program.Procedures.Values)
                {
                    if (eventListener.IsCanceled())
                        return;
                    try
                    {
                        eventListener.ShowProgress("Rewriting procedures to high-level language.", i, program.Procedures.Values.Count);
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
                WriteDecompilerProducts();
            }
			eventListener.ShowStatus("Rewriting complete.");
		}

		public void WriteDecompilerProducts()
		{
            foreach (var program in Project.Programs)
            {
                host.WriteTypes(program, (n, w) => WriteDecompiledTypes(program, n, w));
                host.WriteDecompiledCode(program, (n, p, w) => WriteDecompiledProcedures(program, n, p, w));
                host.WriteGlobals(program, (n, w) => WriteGlobals(program, n, w));
            }
		}

		public void WriteHeaderComment(string filename, Program program, TextWriter w)
		{
			w.WriteLine("// {0}", filename);
			w.WriteLine("// Generated by decompiling {0}", Path.GetFileName(program.Filename));
			w.WriteLine("// using Reko decompiler version {0}.", AssemblyMetadata.AssemblyFileVersion);
			w.WriteLine();
		}
	}
}
