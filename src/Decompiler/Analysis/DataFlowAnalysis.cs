#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Output;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.Analysis
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
		private Program program;
		private DecompilerEventListener eventListener;
        private IImportResolver importResolver;
		private ProgramDataFlow flow;
        private List<SsaTransform> ssts;
        private HashSet<Procedure> sccProcs;

        public DataFlowAnalysis(
            Program program,
            IImportResolver importResolver,
            DecompilerEventListener eventListener)
		{
			this.program = program;
            this.importResolver = importResolver;
            this.eventListener = eventListener;
			this.flow = new ProgramDataFlow();
		}

		public void DumpProgram()
		{
			foreach (Procedure proc in program.Procedures.Values)
			{
				StringWriter output = new StringWriter();
                ProcedureFlow pf = this.flow[proc];
                TextFormatter f = new TextFormatter(output);
				if (pf.Signature != null)
					pf.Signature.Emit(proc.Name, FunctionType.EmitFlags.None, f);
				else
					proc.Signature.Emit(proc.Name, FunctionType.EmitFlags.None, f);
				output.WriteLine();
				pf.Emit(program.Architecture, output);

				output.WriteLine("// {0}", proc.Name);
				proc.Signature.Emit(proc.Name, FunctionType.EmitFlags.None, f);
				output.WriteLine();
				foreach (Block block in proc.ControlGraph.Blocks)
				{
					if (block != null)
					{
						BlockFlow bf = this.flow[block];
						bf.Emit(program.Architecture, output);
						output.WriteLine();
						block.Write(output);
					}
				}
				Debug.WriteLine(output.ToString());
			}
		}

		public ProgramDataFlow ProgramDataFlow
		{
			get { return flow; }
		}

        /// <summary>
        /// Analyzes the procedures of a program by finding all strongly 
        /// connected components (SCCs) and processing the SCCs one by one.
        /// </summary>
        public void AnalyzeProgram()
        {
            UntangleProcedures();
            BuildExpressionTrees();
        }

        /// <summary>
        /// Summarizes the net effect each procedure has on registers,
        /// then removes trashed registers that aren't live-out.
        /// </summary>
        public List<SsaTransform> UntangleProcedures()
        {
            var usb = new UserSignatureBuilder(program);
            usb.BuildSignatures(eventListener);

            IntraBlockDeadRegisters.Apply(program, eventListener);

            var ssts = RewriteProceduresToSsa();

            // Discover ssaId's that are live out at each call site.
            // Delete all others.
            var uvr = new UnusedOutValuesRemover(program, ssts, this.flow, eventListener);
            uvr.Transform();

            // At this point, the exit blocks contain only live out registers.
            // We can create signatures from that.
            CallRewriter.Rewrite(program.Platform, ssts, this.flow, eventListener);
            return ssts;
        }

        /// <summary>
        /// Traverses the call graph, and for each strongly connected
        /// component (SCC) performs SSA transformation and detection of 
        /// trashed and preserved registers.
        /// </summary>
        /// <returns></returns>
        public List<SsaTransform> RewriteProceduresToSsa()
        {
            this.ssts = new List<SsaTransform>();
            var sscf = new SccFinder<Procedure>(new ProcedureGraph(program), UntangleProcedureScc);
            foreach (var procedure in program.Procedures.Values)
            {
                sscf.Find(procedure);
            }
            return ssts;
        }

        /// <summary>
        /// This callback is called from the SccFinder, which passes it a list
        /// of Procedures that form a SCC.
        /// </summary>
        /// <param name="procs"></param>
        private void UntangleProcedureScc(IList<Procedure> procs)
        {
            this.sccProcs = procs.ToHashSet();
            flow.CreateFlowsFor(program.Architecture, procs);

            // Convert all procedures in the SCC to SSA form and perform
            // value propagation.
            var ssts = procs.Select(ConvertToSsa).ToArray();
            this.ssts.AddRange(ssts);

            // At this point, the computation of ProcedureFlow is possible.
            var trf = new TrashedRegisterFinder3(program, flow, ssts, this.eventListener);
            trf.Compute();
            var uid = new UsedRegisterFinder(program.Architecture, flow, this.eventListener);
            foreach (var sst in ssts)
            {
                var ssa = sst.SsaState;
                RemovePreservedUseInstructions(ssa);
                DeadCode.Eliminate(ssa);
                uid.ComputeLiveIn(ssa, false);
                RemoveDeadArgumentsFromCalls(ssa.Procedure, ssts);
            }
        }

        /// <summary>
        /// Remove any Use instruction that uses identifiers
        /// that are marked as preserved.
        /// </summary>
        /// <param name="ssa"></param>
        private void RemovePreservedUseInstructions(SsaState ssa)
        {
            var flow = this.flow[ssa.Procedure];
            var deadStms = new List<Statement>();
            foreach (var stm in ssa.Procedure.ExitBlock.Statements)
            {
                if (stm.Instruction is UseInstruction u &&
                    u.Expression is Identifier id &&
                    flow.Preserved.Contains(id.Storage))
                {
                    deadStms.Add(stm);
                }
            }
            foreach (var stm in deadStms)
            {
                ssa.DeleteStatement(stm);
            }
        }

        /// <summary>
        /// After running the Used register analysis, the ProcedureFlow of 
        /// the procedure <paramref name="proc"/> may have been modified to
        /// exclude some parameters. Functions in the current SCC need to be
        /// adjusted to no longer refer to those parameters.
        /// </summary>
        /// <param name="proc"></param>
        private void RemoveDeadArgumentsFromCalls(
            Procedure proc, 
            IEnumerable<SsaTransform> ssts)
        {
            var mpProcSsa = ssts.ToDictionary(d => d.SsaState.Procedure, d => d.SsaState);
            var flow = this.flow[proc];
            foreach (Statement stm in program.CallGraph.CallerStatements(proc))
            {
                if (!mpProcSsa.TryGetValue(stm.Block.Procedure, out var ssa))
                    continue;

                // We have a call statement that calls `proc`. Make sure 
                // that only arguments present in the procedure flow are present.
                var call = (CallInstruction)stm.Instruction;
                var deadUses =
                    from u in call.Uses
                    where !flow.BitsUsed.ContainsKey(u.Storage)
                    select u;
                foreach (var du in deadUses.ToList())
                {
                    call.Uses.Remove(du);
                }
            }
        }

        /// <summary>
        /// Processes procedures individually, building complex expression
        /// trees out of the simple, close-to-the-machine code generated by
        /// the disassembly.
        /// </summary>
        /// <param name="rl"></param>
        public void BuildExpressionTrees()
        {
            foreach (var sst in this.ssts)
            {
                var ssa = sst.SsaState;

                // Procedures should be untangled from each other. Now process
                // each one separately.
                DeadCode.Eliminate(ssa);

                // Build expressions. A definition with a single use can be subsumed
                // into the using expression. 

                var coa = new Coalescer(ssa);
                coa.Transform();
                DeadCode.Eliminate(ssa);

                var vp = new ValuePropagator(program.Architecture, ssa, eventListener);
                vp.Transform();

                var liv = new LinearInductionVariableFinder(
                    ssa,
                    new BlockDominatorGraph(
                        ssa.Procedure.ControlGraph, 
                        ssa.Procedure.EntryBlock));
                liv.Find();

                foreach (var de in liv.Contexts)
                {
                    var str = new StrengthReduction(ssa, de.Key, de.Value);
                    str.ClassifyUses();
                    str.ModifyUses();
                }

                //var opt = new OutParameterTransformer(proc, ssa.Identifiers);
                //opt.Transform();
                DeadCode.Eliminate(ssa);

                // Definitions with multiple uses and variables joined by PHI functions become webs.
                var web = new WebBuilder(ssa, program.InductionVariables);
                web.Transform();
                ssa.ConvertBack(false);
            }
        }
        /// <summary>
        /// Converts all registers and stack accesses to SSA variables.
        /// </summary>
        /// <param name="proc"></param>
        /// <returns>The SsaTransform for the procedure.</returns>
        public SsaTransform ConvertToSsa(Procedure proc)
        {
            if (program.NeedsSsaTransform)
            {
                // Transform the procedure to SSA state. When encountering 'call'
                // instructions, they can be to functions already visited. If so,
                // they have a "ProcedureFlow" associated with them. If they have
                // not been visited, or are computed destinations  (e.g. vtables)
                // they will have no "ProcedureFlow" associated with them yet, in
                // which case the the SSA treats the call as a "hell node".
                var sst = new SsaTransform(program, proc, sccProcs, importResolver, this.ProgramDataFlow);
                var ssa = sst.Transform();

                // Merge unaligned memory accesses.
                var fuser = new UnalignedMemoryAccessFuser(ssa);
                fuser.Transform();

                // After value propagation expressions like (x86) 
                // mem[esp_42+4] will have been converted to mem[fp - 30]. 
                // We also hope that procedure constants
                // kept in registers are propagated to the corresponding call
                // sites.
                var vp = new ValuePropagator(program.Architecture, ssa, eventListener);
                vp.Transform();

                // Fuse additions and subtractions that are linked by the carry flag.
                var larw = new LongAddRewriter(program.Architecture, ssa);
                larw.Transform();

                // Propagate condition codes and registers. 
                var cce = new ConditionCodeEliminator(ssa, program.Platform);
                cce.Transform();

                vp.Transform();

                // Now compute SSA for the stack-based variables as well. That is:
                // mem[fp - 30] becomes wLoc30, while 
                // mem[fp + 30] becomes wArg30.
                // This allows us to compute the dataflow of this procedure.
                sst.RenameFrameAccesses = true;
                sst.Transform();

                var icrw = new IndirectCallRewriter(program, ssa, eventListener);
                while (!eventListener.IsCanceled() && icrw.Rewrite())
                {
                    vp.Transform();
                    sst.RenameFrameAccesses = true;
                    sst.Transform();
                }

                // By placing use statements in the exit block, we will collect
                // reaching definitions in the use statements.
                sst.AddUsesToExitBlock();
                sst.RemoveDeadSsaIdentifiers();

                // Propagate those newly created stack-based identifiers.
                vp.Transform();

                return sst;
            }
            else
            {
                // We are assuming phi functions are already generated.
                var sst = new SsaTransform(program, proc, sccProcs, importResolver, this.ProgramDataFlow);
                return sst;
            }
        }
	}
}
