#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Graphs;
using Reko.Core.Hll.C;
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
    // https://patterns.eecs.berkeley.edu/?page_id=609
    //    Examples of Task graphs.
    public class DataFlowAnalysis
	{
        private readonly IDynamicLinker dynamicLinker;
        private readonly IServiceProvider services;
        private readonly DecompilerEventListener eventListener;
        private readonly Dictionary<string, int> phaseNumbering;

        public DataFlowAnalysis(
            Program program,
            IDynamicLinker dynamicLinker,
            IServiceProvider services)
		{
			this.Program = program;
            this.dynamicLinker = dynamicLinker;
            this.services = services;
            this.eventListener = services.RequireService<DecompilerEventListener>();
			this.ProgramDataFlow = new ProgramDataFlow(program);
            this.phaseNumbering = new Dictionary<string, int>();
		}

        public Program Program { get; }
        public ProgramDataFlow ProgramDataFlow { get; }

        public void DumpProgram()
		{
			foreach (Procedure proc in Program.Procedures.Values)
			{
				var output = new StringWriter();
                ProcedureFlow pf = this.ProgramDataFlow[proc];
                var f = new TextFormatter(output);
				if (pf.Signature != null)
					pf.Signature.Emit(proc.Name, FunctionType.EmitFlags.None, f);
				else
					proc.Signature.Emit(proc.Name, FunctionType.EmitFlags.None, f);
				output.WriteLine();
				pf.Emit(proc.Architecture, output);

				output.WriteLine("// {0}", proc.Name);
				proc.Signature.Emit(proc.Name, FunctionType.EmitFlags.None, f);
				output.WriteLine();
				foreach (Block block in proc.ControlGraph.Blocks)
				{
					if (block != null)
					{
						block.Write(output);
					}
				}
				Debug.WriteLine(output.ToString());
			}
		}

        /// <summary>
        /// Analyzes the procedures of a program by finding all strongly 
        /// connected components (SCCs) and processing the SCCs one by one.
        /// </summary>
        public void AnalyzeProgram()
        {
            ClearTestFiles();
            var ssts = UntangleProcedures();
            if (eventListener.IsCanceled()) return;
            BuildExpressionTrees(ssts);
        }

        /// <summary>
        /// Summarizes the net effect each procedure has on registers,
        /// then removes trashed registers that aren't live-out.
        /// </summary>
        public List<SsaTransform> UntangleProcedures()
        {
            eventListener.Progress.ShowProgress("Rewriting procedures.", 0, Program.Procedures.Count);

            var ssts = new List<SsaTransform>();
            IntraBlockDeadRegisters.Apply(Program, eventListener);
            if (eventListener.IsCanceled())
                return ssts;

            AdjacentBranchCollector.Transform(Program, eventListener);
            if (eventListener.IsCanceled())
                return ssts;

            ssts = RewriteProceduresToSsa();
            if (eventListener.IsCanceled())
                return ssts;

            // Recreate user-defined signatures. It should prevent type
            // inference between user-defined parameters and other expressions
            var usb = new UserSignatureBuilder(Program);
            usb.BuildSignatures(eventListener);
            if (eventListener.IsCanceled())
                return ssts;

            // Discover ssaId's that are live out at each call site.
            // Delete all others.
            var uvr = new UnusedOutValuesRemover(
                Program,
                ssts.Select(sst => sst.SsaState),
                this.ProgramDataFlow,
                dynamicLinker,
                services);
            uvr.Transform();
            if (eventListener.IsCanceled())
                return ssts;

            // At this point, the exit blocks contain only live out registers.
            // We can create signatures from that.
            CallRewriter.Rewrite(Program.Platform, ssts, this.ProgramDataFlow, eventListener);

            //$TODO: at this point, the type information in ProcedureFlow is no longer needed,
            // so we clear it to save space. It's possible that in the future, we do all the 
            // type analysis as part of the Analysis stage.
            foreach (var procflow in ProgramDataFlow.ProcedureFlows.Values)
                procflow.LiveInDataTypes = new Dictionary<Storage, DataType>(0);
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
            var ssts = new List<SsaTransform>();
            var sccs = SccFinder.Condense(new ProcedureGraph(Program));
            var sccWorkers = sccs.Members.Values.Select(CreateSccWorker);
            //$TODO: make a TaskTree out of this.
            foreach (var worker in sccWorkers)
            {
                if (eventListener.IsCanceled())
                    break;
                var sccSsts = worker.Transform();
                ssts.AddRange(sccSsts);
            }
            return ssts;
        }

        private SccWorker CreateSccWorker(Procedure[] scc)
        {
            return new SccWorker(this, scc, this.dynamicLinker, this.services);
        }


        /// <summary>
        /// Processes procedures individually, building complex expression
        /// trees out of the simple, close-to-the-machine code generated by
        /// the disassembly.
        /// </summary>
        public void BuildExpressionTrees(List<SsaTransform> ssts)
        {
            eventListener.Progress.ShowProgress("Building expressions.", 0, Program.Procedures.Count);
            foreach (var sst in ssts)
            {
                var ssa = sst.SsaState;
                try
                {
                    if (Program.User.Heuristics.Contains(AnalysisHeuristics.AggressiveBranchRemoval))
                    {
                        // This ends up being very aggressive and doesn't replicate the original
                        // binary code. See discussion on https://github.com/uxmal/reko/issues/932
                        DumpWatchedProcedure("urb", "Before unreachable block removal", ssa.Procedure);
                        var urb = new UnreachableBlockRemover(ssa, eventListener);
                        urb.Transform();
                    }

                    DumpWatchedProcedure("precoa", "Before expression coalescing", ssa.Procedure);
                    
                    // Procedures should be untangled from each other. Now process
                    // each one separately.
                    DeadCode.Eliminate(ssa);

                    // Build expressions. A definition with a single use can be subsumed
                    // into the using expression. 
                    var coa = new Coalescer(ssa);
                    coa.Transform();
                    DeadCode.Eliminate(ssa);

                    var vp = new ValuePropagator(Program, ssa, dynamicLinker, services);
                    vp.Transform();

                    DumpWatchedProcedure("postcoa", "After expression coalescing", ssa.Procedure);

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
                    DeadCode.Eliminate(ssa);
                    DumpWatchedProcedure("sr", "After strength reduction", ssa.Procedure);

                    // Definitions with multiple uses and variables joined by PHI functions become webs.
                    var web = new WebBuilder(Program, ssa, Program.InductionVariables, eventListener);
                    web.Transform();
                    ssa.ConvertBack(false);

                    DumpWatchedProcedure("dfa", "After data ProgramFlow analysis", ssa.Procedure);
                }
                catch (Exception ex)
                {
                    eventListener.Error(
                        eventListener.CreateProcedureNavigator(Program, ssa.Procedure),
                        ex,
                        "An internal error occurred while building the expressions of {0}",
                        ssa.Procedure.Name);
                }
                eventListener.Progress.Advance(1);
            }
        }

        [Conditional("DEBUG")]
        public void ClearTestFiles()
        {
            var testSvc = this.services.GetService<ITestGenerationService>();
            if (testSvc != null)
            {
                testSvc.RemoveFiles("analysis_");
            }
        }

        [Conditional("DEBUG")]
        public void DumpWatchedProcedure(string phase, string caption, IEnumerable<Procedure> procs)
        {
            foreach(var proc in procs)
            {
                DumpWatchedProcedure(phase, caption, proc);
            }
        }

        [Conditional("DEBUG")]
        public void DumpWatchedProcedure(string phase, string caption, IEnumerable<SsaTransform> ssts)
        {
            foreach (var sst in ssts)
            {
                DumpWatchedProcedure(phase, caption, sst.SsaState);
            }
        }

        [Conditional("DEBUG")]
        public void DumpWatchedProcedure(string phase, string caption, SsaState ssa)
        {
            
            DumpWatchedProcedure(phase, caption, ssa.Procedure);
#if FIND_BUGS
// This is currently disabled because of hard-to-fix problems with the UnalignedMemoryAccessFuser
            ssa.Validate(s =>
            {
                Console.WriteLine("== SSA validation failure; {0} {1}", caption, ssa.Procedure.Name,  s);
                Console.WriteLine("    {0}", s);
                ssa.Write(Console.Out);
                ssa.Procedure.Write(false, Console.Out);
            });
#endif
        }

        [Conditional("DEBUG")]
        public void DumpWatchedProcedure(string phase, string caption, Procedure proc)
        {
            if (Program.User.DebugTraceProcedures.Contains(proc.Name) ||
                proc.Name == "usb_device_info"||
                proc.Name == "fn0002466C" ||
                proc.Name == "PM_CUSOR_DRAW_CreateSurfaceAndImgDecoding" ||
                false)
            {
                Debug.Print("// {0}: {1} ==================", proc.Name, caption);
                //MockGenerator.DumpMethod(proc);
                proc.Dump(true);
                var testSvc = this.services.GetService<ITestGenerationService>();
                if (testSvc != null)
                {
                    if (!this.phaseNumbering.TryGetValue(phase, out int n))
                    {
                        n = phaseNumbering.Count + 1;
                        phaseNumbering.Add(phase, n);
                    }
                    testSvc.ReportProcedure($"analysis_{n:00}_{phase}.txt", $"// {proc.Name} ===========", proc);
                }
            }
        }
    }
}
