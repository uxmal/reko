#region License
/* Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Lib;
using Decompiler.Core.Output;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using Decompiler.Scanning;
using Decompiler.Loading;
using Decompiler.Analysis;
using Decompiler.Structure;
using Decompiler.Typing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Decompiler
{
    public interface IDecompiler
    {
        Program Program { get; }
        Project Project { get; }

        void LoadProgram(string fileName);
        void LoadRawImage(string fileName, IProcessorArchitecture arch, Platform platform, Address addrBase);
        void ScanProgram();
        ProcedureBase ScanProcedure(Address procAddress);
        DataFlowAnalysis AnalyzeDataFlow();
        void ReconstructTypes();
        void StructureProgram();
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
		private LoaderBase loader;
		private IScanner scanner;
        private DecompilerEventListener eventListener;
        private IServiceProvider services;

        public DecompilerDriver(LoaderBase ldr, DecompilerHost host, IServiceProvider services)
        {
            this.loader = ldr;
            this.host = host;
            this.services = services;
            this.eventListener = (DecompilerEventListener)services.GetService(typeof(DecompilerEventListener));
        }

        public Program Program { get { return prog; } set { prog = value; ProgramChanged.Fire(this); } }
        public event EventHandler ProgramChanged;
        private Program prog;

        public Project Project { get { return project; } set { project = value; ProjectChanged.Fire(this); } }
        public event EventHandler ProjectChanged;
        private Project project;

        /// <summary>
        /// Main entry point of the decompiler. Loads, decompiles, and outputs the results.
        /// </summary>
        public void Decompile(string filename)
        {
            try
            {
                LoadProgram(filename);
                ScanProgram();
                AnalyzeDataFlow();
                ReconstructTypes();
                StructureProgram();
                WriteDecompilerProducts();
            }
            catch (Exception ex)
            {
                eventListener.AddDiagnostic(
                    new NullCodeLocation(filename),
                    new ErrorDiagnostic(string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace)));
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
        public virtual DataFlowAnalysis AnalyzeDataFlow()
        {
            eventListener.ShowStatus("Performing interprocedural analysis.");
            DataFlowAnalysis dfa = new DataFlowAnalysis(Program, eventListener);
            dfa.UntangleProcedures();

            dfa.BuildExpressionTrees();
            host.WriteIntermediateCode(writer => { EmitProgram(dfa, writer); });
            eventListener.ShowStatus("Analysis complete.");
            return dfa;
        }

        private void EmitProgram(DataFlowAnalysis dfa, TextWriter output)
        {
            if (output == null)
                return;
            foreach (Procedure proc in Program.Procedures.Values)
            {
                if (dfa != null)
                {
                    ProcedureFlow flow = dfa.ProgramDataFlow[proc];
                    Formatter f = new Formatter(output);
                    if (flow.Signature != null)
                        flow.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.LowLevelInfo, f);
                    else if (proc.Signature != null)
                        proc.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.LowLevelInfo, f);
                    else
                        output.Write("Warning: no signature found for {0}", proc.Name);
                    output.WriteLine();
                    flow.Emit(Program.Architecture, output);
                    foreach (Block block in new DfsIterator<Block>(proc.ControlGraph).PostOrder().Reverse())
                    {
                        if (block == null)
                            continue;
                        block.Write(output); output.Flush();
                        BlockFlow bf = dfa.ProgramDataFlow[block];
                        if (bf != null)
                        {
                            bf.Emit(Program.Architecture, output);
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
		/// Loads (or assembles) the program in memory, performing relocations as necessary.
		/// </summary>
		/// <param name="program"></param>
		/// <param name="cfg"></param>
        public void LoadProgram(string fileName)
        {
            eventListener.ShowStatus("Loading source program.");
            byte[] image = loader.LoadImageBytes(fileName, 0);
            Project = DeserializeProject(image);
            if (Project != null)
            {
                Program = loader.Load(
                    loader.LoadImageBytes(Project.InputFiles[0].Filename, 0),
                    Project.InputFiles[0].BaseAddress);
            }
            else
            {
                Program = loader.Load(image, null);
                Project = CreateDefaultProject(fileName, Program);
            }
            eventListener.ShowStatus("Source program loaded.");
        }

        /// <summary>
        /// Loads a program into memory, but performs no relocations.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arch"></param>
        /// <param name="platform"></param>
        public void LoadRawImage(string fileName, IProcessorArchitecture arch, Platform platform, Address addrBase)
        {
            eventListener.ShowStatus("Loading raw bytes.");
            byte[] image = loader.LoadImageBytes(fileName, 0);
            var loadedImage = new LoadedImage(addrBase, image);
            Program = new Program
            {
                Image = loadedImage,
                ImageMap = new ImageMap(loadedImage),
                Architecture = arch,
                Platform = platform
            };
            Project = CreateDefaultProject(fileName, Program);
            eventListener.ShowStatus("Raw bytes loaded.");
        }

        private Project DeserializeProject(byte[] image)
        {
            if (!IsXmlFile(image))
                return null;
            try
            {
                Stream stm = new MemoryStream(image);
                return new ProjectSerializer().LoadProject(stm);
            }
            catch (XmlException)
            {
                return null;
            }
        }

        /// <summary>
        /// Peeks at the beginning of the image to determine if it's an XML file.
        /// </summary>
        /// <remarks>
        /// We do not attempt to handle UTF-8 encoded Unicode BOM characters.
        /// </remarks>
        /// <param name="image"></param>
        /// <returns></returns>
        private static bool IsXmlFile(byte[] image)
        {
            if (LoadedImage.CompareArrays(image, 0, new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C }, 5)) // <?xml
                return true;
            return false;
        }

        protected Project CreateDefaultProject(string filename, Program prog)
        {
            var inputFile = new InputFile {
                Filename = filename,
                BaseAddress = prog.Image.BaseAddress,
            };
            inputFile.SetDefaultFileNames(filename);
            var project = new Project
            {
                InputFiles = { inputFile },
            };
            return project;
        }

		/// <summary>
		/// Extracts type information from the typeless rewritten program.
		/// </summary>
		/// <param name="host"></param>
		/// <param name="ivs"></param>
        public void ReconstructTypes()
        {
            TypeAnalyzer analyzer = new TypeAnalyzer(Program, eventListener);
            analyzer.RewriteProgram();
            host.WriteTypes(analyzer.WriteTypes);
        }

        public void WriteDecompiledProcedures(TextWriter w)
        {
            WriteHeaderComment(Path.GetFileName(Project.InputFiles[0].OutputFilename), w);
            w.WriteLine("#include \"{0}\"", Path.GetFileName(Project.InputFiles[0].TypesFilename));
            w.WriteLine();
            var fmt = new CodeFormatter(new Formatter(w));
            foreach (Procedure proc in Program.Procedures.Values)
            {
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

        public void WriteDecompiledTypes(TextWriter w)
        {
            WriteHeaderComment(Path.GetFileName(Project.InputFiles[0].TypesFilename), w);
            w.WriteLine("/*"); Program.TypeStore.Write(w); w.WriteLine("*/");
            TypeFormatter fmt = new TypeFormatter(new Formatter(w), false);
            foreach (EquivalenceClass eq in Program.TypeStore.UsedEquivalenceClasses)
            {
                if (eq.DataType != null)
                {
                    w.Write("typedef ");
                    fmt.Write(eq.DataType, eq.Name);
                    w.WriteLine(";");
                    w.WriteLine();
                }
            }
        }

        /// <summary>
        /// Starts a scan at address <paramref name="addr"/> on the user's request.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns>a ProcedureBase, because the target procedure may have been a thunk or 
        /// an linked procedure the user has decreed not decompileable.</returns>
		public ProcedureBase ScanProcedure(Address addr)
		{
			if (scanner == null)        //$TODO: it's unfortunate that we depend on the scanner of the Decompiler class.
				scanner = CreateScanner(Program, eventListener);
			return scanner.ScanProcedure(addr, null, Program.Architecture.CreateProcessorState());
		}

		/// <summary>
		/// Generates the control flow graph and finds executable code.
		/// </summary>
		/// <param name="prog">the program whose flow graph we seek</param>
		/// <param name="cfg">configuration information</param>
		public void ScanProgram()
		{
			if (loader == null)
				throw new InvalidOperationException("Program must be loaded first.");

			try
			{
                eventListener.ShowStatus("Rewriting reachable machine code.");
                scanner = CreateScanner(Program, eventListener);
				foreach (EntryPoint ep in loader.EntryPoints)
				{
					scanner.EnqueueEntryPoint(ep);
				}
				foreach (SerializedProcedure sp in Project.InputFiles[0].UserProcedures.Values)
				{
					scanner.EnqueueUserProcedure(sp);
				}
				scanner.ScanImage();
                eventListener.ShowStatus("Finished rewriting reachable machine code.");
			}
			finally
			{
				loader = null;
				host.WriteDisassembly(Program.DumpAssembler);
                host.WriteIntermediateCode((w) => { EmitProgram(null, w); });
			}
		}

        public IDictionary<Address, ProcedureSignature> LoadCallSignatures(ICollection<SerializedCall_v1> serializedCalls)
        {
            return
                (from sc in serializedCalls
                 where sc != null && sc.Signature != null
                 let sser = new ProcedureSerializer(Program.Architecture, "stdapi")
                 select new KeyValuePair<Address, ProcedureSignature>(
                     Address.Parse(sc.InstructionAddress, 16),
                     sser.Deserialize(sc.Signature, Program.Architecture.CreateFrame())
                 )).ToDictionary(item => item.Key, item => item.Value);
        }

        private IScanner CreateScanner(Program prog, DecompilerEventListener eventListener)
        {
            return new Scanner(
                prog, 
                LoadCallSignatures(Project.InputFiles[0].UserCalls.Values),
                eventListener);
        }

        /// <summary>
        /// Extracts structured program constructs out of snarled goto nests, if possible.
        /// Since procedures are now independent of each other, this analysis
        /// is done one procedure at a time.
        /// </summary>
        public void StructureProgram()
		{
            int i = 0;
			foreach (Procedure proc in Program.Procedures.Values)
			{
				try
				{
					eventListener.ShowProgress("Rewriting procedures to high-level language.", i, Program.Procedures.Values.Count);
					++i;

					StructureAnalysis sa = new StructureAnalysis(proc);
					sa.Structure();
				}
                catch (Exception e)
                {
                    eventListener.AddDiagnostic(
                        eventListener.CreateProcedureNavigator(proc),
                        new ErrorDiagnostic("An error occurred while rewriting procedure to high-level language.", e));
                }
			}
            WriteDecompilerProducts();
			eventListener.ShowStatus("Rewriting complete.");
		}

		private void WriteDecompilerProducts()
		{
			host.WriteTypes(WriteDecompiledTypes);
            host.WriteDecompiledCode(WriteDecompiledProcedures);
		}

		public void WriteHeaderComment(string filename, TextWriter w)
		{
			w.WriteLine("// {0}", filename);
			w.WriteLine("// Generated on {0} by decompiling {1}", DateTime.Now, Project.InputFiles[0].Filename);
			w.WriteLine("// using Decompiler version {0}.", AssemblyMetadata.AssemblyFileVersion);
			w.WriteLine();
		}
	}
}
