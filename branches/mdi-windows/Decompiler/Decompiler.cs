/* Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Output;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.Scanning;
using Decompiler.Loading;
using Decompiler.Analysis;
using Decompiler.Structure;
using Decompiler.Typing;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler
{
    public interface IDecompiler
    {
        Program Program { get; }
        DecompilerProject Project { get; }

        void LoadProgram(string fileName);
        void ScanProgram();
        Procedure ScanProcedure(Address procAddress);
        void RewriteMachineCode();
        DataFlowAnalysis AnalyzeDataFlow();
        void ReconstructTypes();
        void StructureProgram();
    }

	/// <summary>
	/// The main driver class for decompilation of binaries. 
	/// </summary>
	/// <remarks>
	/// This class is named this way as the previous name 'Decompiler' caused C# 1.1 to get confused
	/// between the namespace and the class name.
	/// </remarks>
	public class DecompilerDriver : IDecompiler
	{
		private Program prog;
		private DecompilerProject project;
		private DecompilerHost host;
		private LoaderBase loader;
		private Scanner scanner;
		private RewriterHost rewriterHost;
        private DecompilerEventListener eventListener;
        private IServiceProvider services;

        public DecompilerDriver(LoaderBase ldr, DecompilerHost host, IServiceProvider services)
        {
            this.loader = ldr;
            this.host = host;
            this.services = services;
            this.eventListener = (DecompilerEventListener)services.GetService(typeof(DecompilerEventListener));
        }

        /// <summary>
        /// Main entry point of the decompiler. Loads, decompiles, and outputs the results.
        /// </summary>
        public void Decompile(string filename)
        {
            try
            {
                LoadProgram(filename);
                ScanProgram();
                RewriteMachineCode();
                AnalyzeDataFlow();
                ReconstructTypes();
                StructureProgram();
                WriteDecompilerProducts();
            }
            catch (Exception e)
            {
                eventListener.AddDiagnostic(new ErrorDiagnostic(null, "{0}", e.Message + Environment.NewLine + e.StackTrace));
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
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, eventListener);
			dfa.UntangleProcedures();

			dfa.BuildExpressionTrees();
            host.WriteIntermediateCode(delegate(TextWriter writer)
            {
                EmitProgram(dfa, writer);
            });
			eventListener.ShowStatus("Analysis complete.");
            return dfa;
		}

		private void EmitProgram(DataFlowAnalysis dfa, TextWriter output)
		{	
			if (output != null)
			{
				foreach (Procedure proc in prog.Procedures.Values)
				{
					if (dfa != null)
					{
						ProcedureFlow flow = dfa.ProgramDataFlow[proc];
                        Formatter f = new Formatter(output);
						if (flow.Signature != null)
							flow.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, f);
						else if (proc.Signature != null)
							proc.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, f);
						else
							output.Write("Warning: no signature found for {0}", proc.Name);
						output.WriteLine();
						flow.Emit(prog.Architecture, output);
						foreach (Block block in proc.RpoBlocks)
						{
							if (block == null)
								continue;

							block.Write(output); output.Flush();
							BlockFlow bf = dfa.ProgramDataFlow[block];
							if (bf != null) bf.Emit(prog.Architecture, output);
						}
					}
					else
					{
						proc.Write(false, output);
					}
					output.WriteLine();
				}
				output.Flush();
			}
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
            project = DeserializeProject(image);
            if (project != null)
            {
                prog = loader.Load(
                    loader.LoadImageBytes(project.Input.Filename, 0),
                    project.Input.BaseAddress);
            }
            else
            {
                prog = loader.Load(image, null);
                project = CreateDefaultProject(fileName, prog);
            }
            eventListener.ShowStatus("Source program loaded.");
        }
        

        private DecompilerProject DeserializeProject(byte[] image)
        {
            if (IsXmlFile(image))
            {
                try
                {
                    XmlSerializer ser = new XmlSerializer(typeof(DecompilerProject));
                    return (DecompilerProject)ser.Deserialize(new MemoryStream(image));
                }
                catch (XmlException)
                {
                    return null;
                }
            }
            else
                return null;
        }

        private static bool IsXmlFile(byte[] image)
        {
            bool isXmlFile = ProgramImage.CompareArrays(image, 0, new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C }, 5);	// <?xml
            return isXmlFile;
        }


        protected DecompilerProject CreateDefaultProject(string filename, Program prog)
        {
            DecompilerProject project = new DecompilerProject();
            project.SetDefaultFileNames(filename);
            project.Input.BaseAddress = prog.Image.BaseAddress;
            return project;
        }

		public Program Program
		{
			get { return prog; }
		}

		public DecompilerProject Project
		{
			get { return project; }
		}

		/// <summary>
		/// Extracts type information from the typeless rewritten program.
		/// </summary>
		/// <param name="host"></param>
		/// <param name="ivs"></param>
        public void ReconstructTypes()
        {
            TypeAnalyzer analyzer = new TypeAnalyzer(prog, eventListener);
            analyzer.RewriteProgram();
            host.WriteTypes(analyzer.WriteTypes);
        }

		/// <summary>
		/// Converts the machine-specific machine code to intermediate format.
		/// </summary>
		/// <param name="prog">the program to rewrite</param>
		/// <param name="cfg">configuration information</param>
		public virtual void RewriteMachineCode()
		{
            eventListener.ShowStatus("Rewriting machine code to intermediate code.");
            if (scanner == null)
                throw new InvalidOperationException("Program must be scanned before it can be rewritten.");
			rewriterHost = new RewriterHost(prog, eventListener, scanner.SystemCalls, scanner.VectorUses);
			rewriterHost.LoadCallSignatures(this.project.UserCalls);
			rewriterHost.RewriteProgram();

            host.WriteIntermediateCode(delegate(TextWriter writer)
            {
                EmitProgram(null, writer);
            });
			eventListener.ShowStatus("Machine code rewritten.");
		}

        public void WriteDecompiledProcedures(TextWriter w)
        {
            WriteHeaderComment(Path.GetFileName(project.Output.OutputFilename), w);
            w.WriteLine("#include \"{0}\"", Path.GetFileName(project.Output.TypesFilename));
            w.WriteLine();
            CodeFormatter fmt = new CodeFormatter(new Formatter(w));
            foreach (Procedure proc in prog.Procedures.Values)
            {
                fmt.Write(proc);
                w.WriteLine();
            }
        }

        public void WriteDecompiledTypes(TextWriter w)
        {
            WriteHeaderComment(Path.GetFileName(project.Output.TypesFilename), w);
            w.WriteLine("/*"); prog.TypeStore.Write(w); w.WriteLine("*/");
            TypeFormatter fmt = new TypeFormatter(new Formatter(w), false);
            foreach (EquivalenceClass eq in prog.TypeStore.UsedEquivalenceClasses)
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
        /// <returns></returns>
		public Procedure ScanProcedure(Address addr)
		{
			if (scanner == null)        //$TODO: it's unfortunate that we depend on the scanner of the Decompiler class.
				scanner = new Scanner(prog, eventListener);
			Procedure proc = scanner.EnqueueProcedure(null, addr, null);
			scanner.ProcessQueues();
            return proc;
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
                eventListener.ShowStatus("Tracing reachable machine code.");
				scanner = new Scanner(prog, eventListener);
				foreach (EntryPoint ep in loader.EntryPoints)
				{
					prog.AddEntryPoint(ep);
					scanner.EnqueueEntryPoint(ep);
				}
				foreach (SerializedProcedure sp in project.UserProcedures)
				{
					scanner.EnqueueUserProcedure(sp);
				}
				scanner.ProcessQueues();
                eventListener.ShowStatus("Finished tracing reachable machine code.");

			}
			finally
			{
				loader = null;
				host.WriteDisassembly(prog.DumpAssembler);
			}
		}
        /// <summary>
        /// Extracts structured program constructs out of snarled goto nests, if possible.
        /// Since procedures are now independent of each other, this analysis
        /// is done one procedure at a time.
        /// </summary>
        public void StructureProgram()
		{
            int i = 0;
			foreach (Procedure proc in prog.Procedures.Values)
			{
                try
                {
                    eventListener.ShowProgress("Rewriting procedures to high-level language.", i, prog.Procedures.Values.Count);
                    ++i;

                    StructureAnalysis sa = new StructureAnalysis(proc);
                    sa.Structure();
                }
                catch (Exception e)
                {
                    eventListener.AddDiagnostic(new ErrorDiagnostic(null, e, "An error occurred while rewriting procedure {0} to high-level language.", proc.Name));
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
			w.WriteLine("// Generated on {0} by decompiling {1}", DateTime.Now, project.Input.Filename);
			w.WriteLine("// using Decompiler.");
			w.WriteLine();
		}
	}
}
