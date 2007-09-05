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

		public event ProgramLoadedEventHandler Loaded;
		public event EventHandler Scanned;
		public event EventHandler Rewritten;
		public event EventHandler ProceduresUntangled;
		public event EventHandler DataAnalyzed;
		public event EventHandler ProgramStructured;
		public event EventHandler Finished;

		public DecompilerDriver()
		{
		}

		private DataFlowAnalysis AnalyzeData(DecompilerHost host)
		{
			// Data flow analysis: determines the signature of the procedures,
			// the locations and types of all the values in the program.

			DataFlowAnalysis dfa = new DataFlowAnalysis(program, host);
			RegisterLiveness rl = dfa.UntangleProcedures();
			OnProceduresUntangled();

			dfa.BuildExpressionTrees(rl);
			EmitProgram(dfa, host.IntermediateCodeWriter);
			
			OnDataAnalyzed();
			return dfa;
		}


		public void DecompileBinary(string binaryFilename, DecompilerHost host)
		{
			DecompilerProject p = new DecompilerProject();
			p.Input.Filename = binaryFilename;
			Decompile(p, host);
		}

		public void Decompile(DecompilerProject proj, DecompilerHost host)
		{
			this.project = proj;
			program = new Program();
			try 
			{
				Loader ldr = Load(program);
				Scanner scanner = Scan(ldr, program, host);
				Rewrite(program, scanner, host);
				DataFlowAnalysis dfa =  AnalyzeData(host);
				ReconstructTypes(host, dfa.InductionVariables);
				StructureProgram();
				WriteDecompiledProcedures(host);
			} 
			catch (Exception e)
			{
				host.WriteDiagnostic(Diagnostic.FatalError, "{0}", e.Message + Environment.NewLine + e.StackTrace);
			}				
			finally
			{
				host.Finished();
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
						ProcedureFlow flow = dfa.FlowOf(proc);
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
							BlockFlow bf = dfa.FlowOf(block);
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
			OnFinished();
		}

		/// <summary>
		/// Loads (or assembles) the program in memory, performing relocations as necessary.
		/// </summary>
		/// <param name="program"></param>
		/// <param name="cfg"></param>
		public Loader Load(Program program)
		{
			Loader ld = new Loader(program);
			switch (project.Input.FileFormat)
			{
			case InputFormat.Assembler:
				ld.Assemble(project.Input.Filename, new IntelArchitecture(ProcessorMode.Real), project.Input.BaseAddress);
				break;
			case InputFormat.AssemblerFragment:
				ld.AssembleFragment(project.Input.Filename, new IntelArchitecture(ProcessorMode.Real), project.Input.BaseAddress);
				break;
			case InputFormat.Binary:
			case InputFormat.COM:
				if (project.Input.BaseAddress == null)
					throw new ArgumentException("Base address must be specified when input format is Binary or COM");
				ld.LoadBinary(project.Input.Filename, project.Input.BaseAddress);
				break;
			default:
				ld.LoadExecutable(project.Input.Filename, project.Input.BaseAddress);
				break;
			}
			OnLoaded(ld);
			return ld;
		}

		protected void OnProgramStructured()
		{
			if (ProgramStructured != null)
				ProgramStructured(this, EventArgs.Empty);
		}

		protected void OnDataAnalyzed()
		{
			if (DataAnalyzed != null)
				DataAnalyzed(this, EventArgs.Empty);
		}

		protected void OnFinished()
		{
			if (Finished != null)
				Finished(this, EventArgs.Empty);
		}

		protected void OnLoaded(Loader ldr)
		{
			if (Loaded != null)
				Loaded(this, new ProgramLoadedEventArgs(ldr));
		}

		protected void OnProceduresUntangled()
		{
			if (ProceduresUntangled != null)
				ProceduresUntangled(this, EventArgs.Empty);
		}

		protected void OnRewritten()
		{
			if (Rewritten != null)
				Rewritten(this, EventArgs.Empty);
		}

		protected void OnScanned()
		{
			if (Scanned != null)
				Scanned(this, EventArgs.Empty);
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
		public void ReconstructTypes(DecompilerHost host, InductionVariableCollection ivs)
		{
			if (this.project.Output.TypeInference)
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
		public void Rewrite(Program prog, Scanner scanner, DecompilerHost host)
		{
			RewriterHost rwHost = new RewriterHost(prog, host, scanner.ImageMap, scanner.SystemCalls, scanner.VectorUses);
			rwHost.LoadCallSignatures(this.project.UserCalls);
			rwHost.RewriteProgram();

			EmitProgram(null, host.IntermediateCodeWriter);
			OnRewritten();
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
		/// Generates the control flow graph and finds procedures.
		/// </summary>
		/// <param name="prog">the program whose flow graph we seek</param>
		/// <param name="cfg">configuration information</param>
		public Scanner Scan(Loader ld, Program prog, DecompilerHost host)
		{
			Scanner scanner = new Scanner(program, ld.ImageMap, null);
			try
			{
				scanner.Parse(ld.EntryPoints, project.UserProcedures);
				// Dump all procedures in DFS order.
				OnScanned();
				return scanner;
			}
			finally
			{
				prog.DumpAssembler(scanner.ImageMap, host.DisassemblyWriter);
				host.DisassemblyWriter.Flush();
			}
		}

		public void StructureProgram()
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
			OnProgramStructured();
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

	// Delegates for the decompiler driver 
	//$REVIEW: consider making these members of the DecompilerHost instead.
	public delegate void ProgramLoadedEventHandler(object sender, ProgramLoadedEventArgs l);

	public class ProgramLoadedEventArgs
	{
		public Loader Loader;

		public ProgramLoadedEventArgs(Loader ldr)
		{
			this.Loader = ldr;
		}
	}


}
