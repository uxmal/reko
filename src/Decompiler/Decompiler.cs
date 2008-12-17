/* Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Arch.Intel;
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
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Decompiler
{
	/// <summary>
	/// The main driver class for decompilation of binaries. 
	/// </summary>
	/// <remarks>
	/// This class is named this way as the previous name 'Decompiler' caused C# 1.1 to get confused
	/// between the namespace and the class name.
	/// </remarks>
	public class DecompilerDriver
	{
		private Program prog;
		private DecompilerProject project;
		private DecompilerHost host;
		private LoaderBase loader;
		private Scanner scanner;
		private RewriterHost rewriterHost;

        public DecompilerDriver(LoaderBase ldr, Program prog, DecompilerHost host)
        {
            this.loader = ldr;
            this.prog = prog;
            this.host = host;
        }

		///<summary>
        /// Determines the signature of the procedures,
		/// the locations and types of all the values in the program.
		///</summary>
		public virtual DataFlowAnalysis AnalyzeDataFlow()
		{
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, host);
			RegisterLiveness rl = dfa.UntangleProcedures();
			host.InterproceduralAnalysisComplete();

			dfa.BuildExpressionTrees(rl);
			using (TextWriter textWriter = host.CreateIntermediateCodeWriter())
			{
				if (textWriter != null)
					EmitProgram(dfa, textWriter);
			}
			host.ProceduresTransformed();
            return dfa;
		}

        /// <summary>
        /// Main entry point of the decompiler. Loads, decompiles, and outputs the results.
        /// </summary>
		public void Decompile()
		{
			try 
			{
				LoadProgram();
				ScanProgram();
				RewriteMachineCode();
				AnalyzeDataFlow();
				ReconstructTypes();
				StructureProgram();
				WriteDecompilerProducts();
			} 
			catch (Exception e)
			{
				host.WriteDiagnostic(Diagnostic.FatalError, null, "{0}", e.Message + Environment.NewLine + e.StackTrace);
			}				
			finally
			{
				host.DecompilationFinished();
			}
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
						if (flow.Signature != null)
							flow.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, output);
						else if (proc.Signature != null)
							proc.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, output);
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
		public void LoadProgram()
		{
            project = loader.Load(null);
			host.ProgramLoaded();
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
			if (project.Output.TypeInference)
			{
				TypeAnalyzer analyzer = new TypeAnalyzer(prog, host);
				analyzer.RewriteProgram();
				using (TextWriter w = host.CreateTypesWriter(project.Output.TypesFilename))
				{
					analyzer.WriteTypes(w);
				}
			}
			else
			{
				MemReplacer mem = new MemReplacer(prog);
				mem.RewriteProgram();
			}
		}

		/// <summary>
		/// Converts the machine-specific machine code to intermediate format.
		/// </summary>
		/// <param name="prog">the program to rewrite</param>
		/// <param name="cfg">configuration information</param>
		public virtual void RewriteMachineCode()
		{
			rewriterHost = new RewriterHost(prog, host, scanner.SystemCalls, scanner.VectorUses);
			rewriterHost.LoadCallSignatures(this.project.UserCalls);
			rewriterHost.RewriteProgram();

			using (TextWriter w = host.CreateDisassemblyWriter())
			{
				EmitProgram(null, w);
				host.MachineCodeRewritten();
			}
		}

		public void WriteDecompiledProcedures(DecompilerHost host)
		{
			using (TextWriter w = host.CreateDecompiledCodeWriter(project.Output.OutputFilename))
			{
				WriteHeaderComment(Path.GetFileName(project.Output.OutputFilename), w);
				w.WriteLine("#include \"{0}\"", Path.GetFileName(project.Output.TypesFilename));
				w.WriteLine();
				CodeFormatter fmt = new CodeFormatter(w);
				foreach (Procedure proc in prog.Procedures.Values)
				{
					fmt.Write(proc);
					w.WriteLine();
				}
			}
		}

		public void WriteDecompiledTypes(DecompilerHost host)
		{
			using (TextWriter w = host.CreateTypesWriter(project.Output.TypesFilename))
			{
				WriteHeaderComment(Path.GetFileName(project.Output.TypesFilename), w);
				w.WriteLine("/*");prog.TypeStore.Write(w); w.WriteLine("*/");
				TypeFormatter fmt = new TypeFormatter(w, false);
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
		}


        /// <summary>
        /// Starts a scan at address <paramref name="addr"/> on the user's request.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
		public Procedure ScanProcedure(Address addr)
		{
			if (scanner == null)        //$TODO: it's unfortunate that we depend on the scanner of the Decompiler class.
				scanner = new Scanner(prog, host);
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
				scanner = new Scanner(prog, host);
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
				host.ProgramScanned();
			}
			finally
			{
				loader = null;
				using (TextWriter w = host.CreateDisassemblyWriter())
				{
					prog.DumpAssembler(w);
				}
			}
		}

		public void StructureProgram()
		{
			// At this point, the functions are unsnarled from each other, and 
			// can be analyzed separately.

			// Extracts structured program constructs out of snarled goto nests, if possible.
			// Since procedures are now independent of each other, this analysis
			// is done one procedure at a time.

			foreach (Procedure proc in prog.Procedures.Values)
			{
				StructureAnalysis sa = new StructureAnalysis(proc);
				sa.FindStructures();
			}
			host.CodeStructuringComplete();
		}


		public void WriteDecompilerProducts()
		{
			WriteDecompiledTypes(host);
			WriteDecompiledProcedures(host);
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
