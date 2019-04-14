#region License
/* Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Loading;
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
        void ScanPrograms();
        void IdentifyLibraryCode();
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
	/// <remarks>
	/// This class is named this way as the previous name 'Decompiler' causes C# to get confused
	/// between the namespace and the class name.
	/// </remarks>
	public class DecompilerDriver : IDecompiler
	{
		private DecompilerHost host;
		private readonly ILoader loader;
		private IScanner scanner;
        private DecompilerEventListener eventListener;
        private IServiceProvider services;

        public DecompilerDriver(ILoader ldr, IServiceProvider services)
        {
            this.loader = ldr ?? throw new ArgumentNullException("ldr");
            if (services == null)
                throw new ArgumentNullException("services");
            this.host = services.RequireService<DecompilerHost>();
            this.services = services;
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
                ApplyByteSignitures();
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
                eventListener.ShowStatus("Performing interprocedural analysis.");
                var ir = new ImportResolver(project, program, eventListener);
                var dfa = new DataFlowAnalysis(program, ir, eventListener);
                if (program.NeedsSsaTransform)
                {
                    dfa.UntangleProcedures();
                }
                dfa.BuildExpressionTrees();
                host.WriteIntermediateCode(program, writer => { EmitProgram(program, dfa, writer); });
            }
            eventListener.ShowStatus("Interprocedural analysis complete.");
        }


        public virtual void ApplyByteSignitures()
        {
            foreach (Program program in Project.Programs)
            {
                ApplyByteSigniture(program);
            }
        }

        public virtual void ApplyByteSigniture(Program program)
        {
            try
            {
                var eventListener = services.RequireService<DecompilerEventListener>();
                eventListener.ShowStatus("Performing signature analysis.");

                // Get the architeture for the program so we know which directory of sig files to load
                string name = program.Architecture.Name;
                var fsSvc = services.RequireService<IFileSystemService>();
                string path = fsSvc.GetCurrentDirectory();
                string[] fileEntries = Directory.GetFiles(path + "\\Sigs\\" + name, "*.sig");
                if (fileEntries.Length == 0)
                {
                    return;
                }

                // Create instance of sig flirt engine
                ByteSignatureMatcher signitureEngine = new ByteSignatureMatcher(services);

                foreach (string fileName in fileEntries)
                {
                    signitureEngine.LoadByteSignatures(fileName);
                }

                // We need to look at the instructions to work out were and transfer address exists in the code
                // the reason for doing this is that the values in the static library will have been updated 
                // as a result of the linker on the application build process.
                // By finding them we will know which bytes we can ignore as part of the matching process.

                // Get the list of instructions in the program
                Dictionary<ImageMapBlock, MachineInstruction[]> instructions;

                instructions = new Dictionary<ImageMapBlock, MachineInstruction[]>();
                foreach (var bi in program.ImageMap.Items.Values.OfType<ImageMapBlock>().ToList())
                {
                    var instrs = new List<MachineInstruction>();
                    var addrStart = bi.Address;
                    var addrEnd = bi.Address + bi.Size;
                    var dasm = program.CreateDisassembler(program.Architecture, addrStart).GetEnumerator();
                    while (dasm.MoveNext() && dasm.Current.Address < addrEnd)
                    {
                        instrs.Add(dasm.Current);
                    }
                    instructions.Add(bi, instrs.ToArray());
                }

                // Loop through each prodecure we have to go through a number of steps to get the dat we need for the signiture match
                foreach (Procedure proc in program.Procedures.Values)
                {
                    try
                    {
                        {
                            Address funcStartAddress = proc.ControlGraph.Blocks[2].Address;
                            int lastInstLength = 0;
                            ImageSegment seg;
                            ImageMapItem item;
                            
                            if (proc.ControlGraph.Blocks.Count < 3)
                            {
                                break;
                            }

                            Block endBlock = proc.ControlGraph.Blocks[2];
                            // Loop through the blocks in the procedure and find the call and transfer instuctions and record the address for each
                            for (int blockIndex = 2; blockIndex < proc.ControlGraph.Blocks.Count; blockIndex++)
                            {
                                Block block = proc.ControlGraph.Blocks[blockIndex];
                                Address funcBlockAddress = block.Address;

                                // Find the last address in the blocks, do not assume the last block contains the last address
                                if (block.Address >= endBlock.Address)
                                {
                                    endBlock = block;
  
                                    if (funcBlockAddress != null)
                                    {
                                        program.SegmentMap.TryFindSegment(funcBlockAddress, out seg);
                                        program.ImageMap.TryFindItem(funcBlockAddress, out item);

                                        var itemBlock = item as ImageMapBlock;

                                        MachineInstruction[] instrs = instructions[itemBlock];

                                        // Step through and check each instruction
                                        lastInstLength = instrs[instrs.Length - 1].Length;
                                    }
                                }
                            }

                            // We now need to get the bytes of the method,
                            var linStart = funcStartAddress.ToLinear();

                            // Take the address of the last statement and then add the length of the statement, which will give us the last byte of the method.
                            ulong linEnd = endBlock.Statements[endBlock.Statements.Count - 1].LinearAddress + (ulong) lastInstLength;
                            ulong procLength = (linEnd - linStart);

                            byte[] abCode = new byte[procLength];
                            int index = 0;
                            bool gettingDataError = false;
                            var rdr = program.CreateImageReader(program.Architecture, funcStartAddress);
                            while (rdr.Address.ToLinear() < linEnd)
                            {
                                try
                                {
                                    if (rdr.IsValid)
                                    {
                                        abCode[index] = rdr.ReadByte();
                                        ++index;
                                    }
                                    else
                                    {
                                        gettingDataError = true;
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    eventListener.Error(new NullCodeLocation(""), ex, "An error occurred while performing getting procudred data in signature analysis.");
                                }
                            }

                            // We name have the length of an address, list of locations of call and transfer addresses, and the procedure byte stream.
                            // Can now call the signitrue matcher, if it returns a string, we can rename the prodecure.
                            if (gettingDataError == false)
                            {
                                string sigMethodName = signitureEngine.FindMatchingSignitureStart(eventListener.CreateAddressNavigator(program, proc.EntryAddress), abCode, (int)procLength);
                                if (sigMethodName.Length > 0)
                                {
                                    proc.ChangeName(sigMethodName, ProvenanceType.ByteSignature);
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        eventListener.Error(new NullCodeLocation(""), ex, "An error occurred while performing procedure signature analysis.");
                    }
                }
            }
            catch(Exception ex)
            {
                eventListener.Error(new NullCodeLocation(""), ex, "An error occurred while performing signature analysis.");
            }
            eventListener.ShowStatus("Signature analysis complete.");
        }

        public void DumpAssembler(Program program, Formatter wr)
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

        private void EmitProgram(Program program, DataFlowAnalysis dfa, TextWriter output)
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

                        BlockFlow bf = dfa.ProgramDataFlow[block];
                        if (bf != null)
                        {
                            bf.Emit(program.Architecture, output);
                            output.WriteLine();
                        }
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
                this.Project = CreateDefaultProject(fileName, program);
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
            Project = CreateDefaultProject(fileName, program);
            eventListener.ShowStatus("Assembled program.");
        }

        /// <summary>
        /// Loads a program into memory using the additional information in 
        /// <paramref name="raw"/>. Use this to open files with insufficient or
        /// no metadata.
        /// </summary>
        /// <param name="fileName">Name of the file to be loaded.</param>
        /// <param name="raw">Extra metadata supllied by the user.</param>
        public Program LoadRawImage(string fileName, LoadDetails raw)
        {
            eventListener.ShowStatus("Loading raw bytes.");
            byte[] image = loader.LoadImageBytes(fileName, 0);
            var program = loader.LoadRawImage(fileName, image, null, raw);
            Project = CreateDefaultProject(fileName, program);
            eventListener.ShowStatus("Raw bytes loaded.");
            return program;
        }

        protected Project CreateDefaultProject(string fileNameWithPath, Program program)
        {
            program.Filename = fileNameWithPath;
            program.EnsureFilenames(fileNameWithPath);
            program.User.ExtractResources = true;
            var project = new Project
            {
                Programs = { program },
            };
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
                TypeAnalyzer analyzer = new TypeAnalyzer(eventListener);
                try
                {
                    try
                    {
                        analyzer.RewriteProgram(program);
                    }
                    catch (Exception ex)
                    {
                        eventListener.Error(new NullCodeLocation(""), ex, "Error when reconstructing types.");
                    }
                } 
                finally
                {
                    host.WriteTypes(program, analyzer.WriteTypes);
                }
            }
        }

        public void WriteDecompiledProcedures(Program program, TextWriter w)
        {
            WriteHeaderComment(Path.GetFileName(program.OutputFilename), program, w);
            w.WriteLine("#include \"{0}\"", Path.GetFileName(program.TypesFilename));
            w.WriteLine();
            var fmt = new AbsynCodeFormatter(new TextFormatter(w));
            foreach (var de in program.Procedures)
            {
                w.WriteLine("// {0}: {1}", de.Key, de.Value);
                var proc = de.Value;
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

        public void WriteGlobals(Program program, TextWriter w)
        {
            WriteHeaderComment(Path.GetFileName(program.OutputFilename), program, w);
            w.WriteLine("#include \"{0}\"", Path.GetFileName(program.TypesFilename));
            w.WriteLine();
            var gdw = new GlobalDataWriter(program, services);
            gdw.WriteGlobals(new TextFormatter(w));
            w.WriteLine();
        }
    
        public void WriteDecompiledTypes(Program program, TextWriter w)
        {
            WriteHeaderComment(Path.GetFileName(program.TypesFilename), program, w);
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
                host.WriteDisassembly(program, w => DumpAssembler(program, w));
                host.WriteIntermediateCode(program, w => EmitProgram(program, null, w));
            }
        }

        public void IdentifyLibraryCode()
        {
            if (Project.Programs.Count == 0)
                throw new InvalidOperationException("Programs must be loaded first.");

            foreach (Program program in Project.Programs)
            {
                IdentifyLibraryCode(program);
            }
        }

        private void IdentifyLibraryCode(Program program)
        {
            try
            {
                eventListener.ShowStatus("Scanning code for libraries and applying signitures.");
                ApplyByteSigniture(program);
                eventListener.ShowStatus("Finished Scanning code for libraries and applying signitures.");
            }
            catch (Exception ex)
            {
                eventListener.Error(new NullCodeLocation(""), ex, "Error when identifying and renaming methods from signature files.");
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
                new ImportResolver(project, program, eventListener),
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
                host.WriteTypes(program, w => WriteDecompiledTypes(program, w));
                host.WriteDecompiledCode(program, w => WriteDecompiledProcedures(program, w));
                host.WriteGlobals(program, w => WriteGlobals(program, w));
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
