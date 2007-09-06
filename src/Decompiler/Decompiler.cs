/* Copyright (C) 1999-2007 John Källén.
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
using Decompiler.Scanning;
using Decompiler.Loading;
using Decompiler.Analysis;
using Decompiler.Structure;
using Decompiler.Typing;
using System;
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
		private Program program;
		private DecompilerProject project;
		private Loader loader;
		private Scanner scanner;
		private RewriterHost rewriterHost;
		private InductionVariableCollection ivs;

		public DecompilerDriver(DecompilerProject project)
		{
			this.project = project;
			this.program = new Program();
		}

		public DecompilerDriver(string binaryFilename)
		{
			this.project = new DecompilerProject();
			this.project.Input.Filename = binaryFilename;
			this.program = new Program();
		}

		public void AnalyzeDataFlow(DecompilerHost host)
		{
			// Data flow analysis: determines the signature of the procedures,
			// the locations and types of all the values in the program.

			DataFlowAnalysis dfa = new DataFlowAnalysis(program, host);
			RegisterLiveness rl = dfa.UntangleProcedures();
			host.InterproceduralAnalysisComplete();

			dfa.BuildExpressionTrees(rl);
			EmitProgram(dfa, host.IntermediateCodeWriter);
			
			ivs = dfa.InductionVariables;
			host.ProceduresTransformed();
		}



		public void Decompile(DecompilerHost host)
		{
			try 
			{
				LoadProgram(host);
				ScanProgram(host);
				RewriteMachineCode(host);
				AnalyzeDataFlow(host);
				ReconstructTypes(host);
				StructureProgram(host);
				WriteDecompiledProcedures(host);
			} 
			catch (Exception e)
			{
				host.WriteDiagnostic(Diagnostic.FatalError, "{0}", e.Message + Environment.NewLine + e.StackTrace);
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
				foreach (Procedure proc in program.Procedures.Values)
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
						flow.Emit(program.Architecture, output);
						foreach (Block block in proc.RpoBlocks)
						{
							if (block == null)
								continue;

							block.Write(output); output.Flush();
							BlockFlow bf = dfa.ProgramDataFlow[block];
							if (bf != null) bf.Emit(program.Architecture, output);
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
		public void LoadProgram(DecompilerHost host)
		{
			loader = new Loader(program);
			switch (project.Input.FileFormat)
			{
			case InputFormat.Assembler:
				loader.Assemble(project.Input.Filename, new IntelArchitecture(ProcessorMode.Real), project.Input.BaseAddress);
				break;
			case InputFormat.AssemblerFragment:
				loader.AssembleFragment(project.Input.Filename, new IntelArchitecture(ProcessorMode.Real), project.Input.BaseAddress);
				break;
			case InputFormat.Binary:
			case InputFormat.COM:
				if (project.Input.BaseAddress == null)
					throw new ArgumentException("Base address must be specified when input format is Binary or COM");
				loader.LoadBinary(project.Input.Filename, project.Input.BaseAddress);
				break;
			default:
				loader.LoadExecutable(project.Input.Filename, project.Input.BaseAddress);
				break;
			}
			host.ProgramLoaded();
		}

		public Program Program
		{
			get { return program; }
		}


		/// <summary>
		/// Extracts type information from the typeless rewritten program.
		/// </summary>
		/// <param name="host"></param>
		/// <param name="ivs"></param>
		public void ReconstructTypes(DecompilerHost host)
		{
			if (project.Output.TypeInference)
			{
				TypeAnalyzer analyzer = new TypeAnalyzer(program, ivs, host);
				analyzer.RewriteProgram();
				analyzer.WriteTypes(host.TypesWriter);
			}
			else
			{
				MemReplacer mem = new MemReplacer(program);
				mem.RewriteProgram();
			}
		}

		/// <summary>
		/// Converts the machine-specific machine code to intermediate format.
		/// </summary>
		/// <param name="prog">the program to rewrite</param>
		/// <param name="cfg">configuration information</param>
		public void RewriteMachineCode(DecompilerHost host)
		{
			rewriterHost = new RewriterHost(program, host, scanner.ImageMap, scanner.SystemCalls, scanner.VectorUses);
			rewriterHost.LoadCallSignatures(this.project.UserCalls);
			rewriterHost.RewriteProgram();

			EmitProgram(null, host.IntermediateCodeWriter);
			host.MachineCodeRewritten();
		}

		public void WriteDecompiledProcedures(DecompilerHost host)
		{
			CodeFormatter fmt = new CodeFormatter(host.DecompiledCodeWriter);
			foreach (Procedure proc in program.Procedures.Values)
			{
				fmt.Write(proc);
				host.DecompiledCodeWriter.WriteLine();
			}
		}

		/// <summary>
		/// Generates the control flow graph and finds executable code.
		/// </summary>
		/// <param name="prog">the program whose flow graph we seek</param>
		/// <param name="cfg">configuration information</param>
		public void ScanProgram(DecompilerHost host)
		{
			if (loader == null)
				throw new InvalidOperationException("Program must be loaded before it can be scanned.");
			scanner = new Scanner(program, null);
			try
			{
				scanner.Parse(loader.EntryPoints, project.UserProcedures);
				// Dump all procedures in DFS order.
				host.ProgramScanned();
			}
			finally
			{
				loader = null;
				if (host.DisassemblyWriter != null)
				{
					program.DumpAssembler(scanner.ImageMap, host.DisassemblyWriter);
					host.DisassemblyWriter.Flush();
				}
			}
		}

		public void StructureProgram(DecompilerHost host)
		{
			if (project.Output.ControlStructure)
			{
				// At this point, the functions are unsnarled from each other, and 
				// can be analyzed separately.

				// Extracts structured program constructs out of snarled goto nests, if possible.
				// Since procedures are now independent of each other, this analysis
				// is done one procedure at a time.

				foreach (Procedure proc in program.DfsProcedures)
				{
					StructureAnalysis sa = new StructureAnalysis(proc);
					sa.FindStructures();
				}
			}
			host.CodeStructuringComplete();
		}

		// class Timer /////////////////////////////////////////////////////

		public class Timer
		{
			private DateTime time;

			public Timer()
			{
				Start();
			}
			
			public TimeSpan Elapsed() 
			{
				return DateTime.Now - time; 
			}
			
			public void Start()
			{
				time = DateTime.Now;
			}
		}
	}
}
