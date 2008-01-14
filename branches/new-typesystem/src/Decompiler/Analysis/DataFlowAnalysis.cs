/* 
 * Copyright (C) 1999-2008 John Källén.
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
using Decompiler.Core.Code;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Analysis
{
	/// <summary>
	/// We are keenly interested in discovering the register linkage 
	/// between procedures, i.e. what registers are used by a called 
	/// procedure, and what modified registers are used by a calling 
	/// procedure. Once these registers have been discovered, we can
	/// separate the procedures from each other and proceed with the
	/// decompilation.
	/// </summary>
	public class DataFlowAnalysis
	{
		private Program prog;
		private DecompilerHost host;
		private ProgramDataFlow flow;
		private InductionVariableCollection ivs;
	
		public DataFlowAnalysis(Program prog, DecompilerHost host)
		{
			this.prog = prog;
			this.host = host;
			this.ivs = new InductionVariableCollection();
			this.flow = new ProgramDataFlow(prog);
		}

		public void AnalyzeProgram()
		{
			RegisterLiveness rl = UntangleProcedures();
			BuildExpressionTrees(rl);
		}

		public void BuildExpressionTrees(RegisterLiveness rl)
		{
			int total = prog.Procedures.Count;
			foreach (Procedure proc in prog.Procedures.Values)
			{
				Aliases alias = new Aliases(proc, prog.Architecture, flow);
				alias.Transform();
				DominatorGraph doms = new DominatorGraph(proc);
				SsaTransform sst = new SsaTransform(proc, doms, true);
				SsaState ssa = sst.SsaState;

				ConditionCodeEliminator cce = new ConditionCodeEliminator(ssa.Identifiers);
				cce.Transform();

				DeadCode.Eliminate(proc, ssa);

				ValuePropagator vp = new ValuePropagator(ssa.Identifiers, proc);
				vp.Transform();
				DeadCode.Eliminate(proc, ssa);

				// Build expressions. A definition with a single value can be subsumed
				// into the using expression. Definitions with multiple uses and variables 
				// joined by PHI functions become webs.

				Coalescer coa = new Coalescer(proc, ssa);
				coa.Transform();
				DeadCode.Eliminate(proc, ssa);

				LinearInductionVariableFinder liv = new LinearInductionVariableFinder(proc, ssa.Identifiers, doms);
				liv.Find();

				OutParameterTransformer opt = new OutParameterTransformer(proc, ssa.Identifiers);
				opt.Transform();
				DeadCode.Eliminate(proc, ssa);

				WebBuilder web = new WebBuilder(proc, ssa.Identifiers, this.ivs);
				web.Transform();
				ssa.ConvertBack(false);
			} 
		}

		public void DumpProgram(RegisterLiveness rl)
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
				StringWriter output = new StringWriter();
				ProcedureFlow pf= this.flow[proc];
				if (pf.Signature != null)
					pf.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, output);
				else if (proc.Signature != null)
					proc.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, output);
				else
					output.Write("Warning: no signature found for {0}", proc.Name);
				output.WriteLine();
				pf.Emit(prog.Architecture, output);

				output.WriteLine("// {0}", proc.Name);
				proc.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, output);
				output.WriteLine();
				foreach (Block block in proc.RpoBlocks)
				{
					if (block != null)
					{
						BlockFlow bf = this.flow[block];
						bf.Emit(prog.Architecture, output);
						output.WriteLine();
						block.Write(output);
					}
				}
				Debug.WriteLine(output.ToString());
			}
		}

		public InductionVariableCollection InductionVariables
		{
			get { return ivs; }
		}

		public ProgramDataFlow ProgramDataFlow
		{
			get { return flow; }
		}

		/// <summary>
		/// Inserts "fake" use statements
		/// in the return block for each defined variable, in order to compute reaching definitions.
		/// Only variables that can escape are marked as live -- ie. only machine registers.
		/// </summary>
		/// <param name="proc"></param>
		public static void InsertExitBlockStatements(Procedure proc)
		{
			Block blReturn = proc.ExitBlock;
			foreach (Identifier a in proc.Frame.Identifiers)
			{
				if (a.Storage is RegisterStorage || a.Storage is FlagGroupStorage)
				{
					blReturn.Statements.Add(new UseInstruction(a));
				}
			}
		}

		/// <summary>
		/// Finds all interprocedural register dependencies (in- and out-parameters) and
		/// abstracts them away by rewriting as calls.
		/// </summary>
		public RegisterLiveness UntangleProcedures()
		{
			host.WriteDiagnostic(Diagnostic.Info, "Finding trashed registers");
			TrashedRegisterFinder trf = new TrashedRegisterFinder(prog, flow);
			trf.DecompilerHost = host;
			trf.Compute();
			host.WriteDiagnostic(Diagnostic.Info, "Computing register liveness");
			RegisterLiveness rl = RegisterLiveness.Compute(prog, flow);
			host.WriteDiagnostic(Diagnostic.Info, "Rewriting calls");
			GlobalCallRewriter.Rewrite(prog, flow);
			return rl;
		}
	}
}
