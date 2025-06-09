#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Concurrent;
using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Graphs;
using Reko.Core.Hll.C;
using Reko.Core.Output;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Services;
using System;
using System.Collections.Concurrent;
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
        private readonly IDecompilerEventListener eventListener;
        private readonly Dictionary<string, int> phaseNumbering;

        public DataFlowAnalysis(
            Program program,
            IDynamicLinker dynamicLinker,
            IServiceProvider services)
		{
			this.Program = program;
            this.dynamicLinker = dynamicLinker;
            this.services = services;
            this.eventListener = services.RequireService<IDecompilerEventListener>();
			this.ProgramDataFlow = new ProgramDataFlow(program);
            this.phaseNumbering = [];
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
				if (pf.Signature is not null)
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
					block?.Write(output);
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
        public IReadOnlyCollection<SsaTransform> UntangleProcedures()
        {
            eventListener.Progress.ShowProgress("Rewriting procedures.", 0, Program.Procedures.Count);

            IReadOnlyCollection<SsaTransform> ssts = [];
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
                procflow.LiveInDataTypes.Clear();
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
            foreach (var worker in sccWorkers)
            {
                if (eventListener.IsCanceled())
                    break;
                var sccSsts = worker.Transform();
                ssts.AddRange(sccSsts);
            }
            return ssts;
        }

        public IReadOnlyCollection<SsaTransform> RewriteProceduresToSsa_Concurrent()
        {
            var ssts = new ConcurrentQueue<SsaTransform>();
            var sccs = SccFinder.Condense(new ProcedureGraph(Program));
            var coordinator = new SccWorkerCoordinator<Procedure>(sccs, eventListener, procs =>
            {
                Debug.Print("== Working on {0}", string.Join(",", procs.Select(p => p.Name)));
                if (eventListener.IsCanceled())
                    return;
                var worker = CreateSccWorker(procs);
                var sccSsts = worker.Transform();
                foreach (var sccSst in sccSsts)
                {
                    ssts.Enqueue(sccSst);
                }
                Debug.Print("== Done with {0}", string.Join(",", procs.Select(p => p.Name)));

            });
            coordinator.RunAsync().Wait();
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
        public void BuildExpressionTrees(IReadOnlyCollection<SsaTransform> ssts)
        {
            eventListener.Progress.ShowProgress("Building expressions.", 0, Program.Procedures.Count);
            foreach (var sst in ssts)
            {
                var ssa = sst.SsaState;
                var analysisFactory = ssa.Procedure.Architecture.CreateExtension<IAnalysisFactory>();
                var context = new AnalysisContext(
                    Program, new HashSet<Procedure>() { ssa.Procedure },
                    dynamicLinker, services, eventListener);
                try
                {
                    var vp = new ValuePropagator(context);
                    vp.Transform(ssa);

                    if (Program.User.Heuristics.Contains(AnalysisHeuristics.AggressiveBranchRemoval))
                    {
                        // This ends up being very aggressive and doesn't replicate the original
                        // binary code. See discussion on https://github.com/uxmal/reko/issues/932
                        DumpWatchedProcedure("urb", "Before unreachable block removal", ssa);
                        var urb = new UnreachableBlockRemover(context);
                        urb.Transform(ssa);
                    }

                    DumpWatchedProcedure("precoa", "Before expression coalescing", ssa);
                    
                    // Procedures should be untangled from each other. Now process
                    // each one separately.
                    DeadCode.Eliminate(ssa);

                    // Build expressions. A definition with a single use can be subsumed
                    // into the using expression. 
                    var coa = new Coalescer(context);
                    coa.Transform(ssa);
                    DeadCode.Eliminate(ssa);

                    vp.Transform(ssa);

                    DumpWatchedProcedure("postcoa", "After expression coalescing", ssa);

                    ssa = RunAnalyses(analysisFactory, context, AnalysisStage.AfterExpressionCoalescing, ssa);

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
                    DumpWatchedProcedure("sr", "After strength reduction", ssa);

                    // Definitions with multiple uses and variables joined by PHI functions become webs.
                    var web = new WebBuilder(context, Program.InductionVariables);
                    web.Transform(ssa);

                    var unssa = new UnSsaTransform(false);
                    unssa.Transform(ssa);

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

        public SsaState RunAnalyses(IAnalysisFactory? analysisFactory, AnalysisContext context, AnalysisStage stage, SsaState ssa)
        {
            if (analysisFactory is null)
                return ssa;
            var procSpecificAnalyses = analysisFactory.CreateSsaAnalyses(stage, ssa, context);
            if (procSpecificAnalyses is null)
                return ssa;
            foreach (var analysis in procSpecificAnalyses)
            {
                var (ssaNew, _) = analysis.Transform(ssa);
                DumpWatchedProcedure(analysis.Id, analysis.Description, ssa);
                ssa = ssaNew;
            }
            return ssa;
        }

        [Conditional("DEBUG")]
        public void ClearTestFiles()
        {
            var testSvc = this.services.GetService<ITestGenerationService>();
            testSvc?.RemoveFiles("analysis_");
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
            if (Program.User.DebugTraceProcedures.Contains(ssa.Procedure.Name))
            {
                Debug.Print("// {0}: {1}: SSA graph ==================", ssa.Procedure.Name, caption);
                var sw = new StringWriter();
                ssa.Write(sw);
                Debug.WriteLine(sw);
            }
            DumpWatchedProcedure(phase, caption, ssa.Procedure);
            if (Program.User.DebugTraceProcedures.Contains(ssa.Procedure.Name))
                ssa.Validate(s =>
            {
                Console.WriteLine("== SSA validation failure; {0} {1}", caption, ssa.Procedure.Name);
                Console.WriteLine("    {0}", s);
                ssa.Write(Console.Out);
                ssa.Procedure.Write(false, Console.Out);

                Debug.Print("== SSA validation failure; {0} {1}", caption, ssa.Procedure.Name);
                Debug.Print("    {0}", s);
                ssa.Dump(true);
            });
        }

        [Conditional("DEBUG")]
        public void DumpWatchedProcedure(string phase, string caption, Procedure proc)
        {
            if (Program.User.DebugTraceProcedures.Contains(proc.Name))
            {
                Debug.Print("// {0}: {1} ==================", proc.Name, caption);
                //MockGenerator.DumpMethod(proc);
                proc.Dump(true);
                var testSvc = this.services.GetService<ITestGenerationService>();
                if (testSvc is not null)
                {
                    if (!this.phaseNumbering.TryGetValue(phase, out int n))
                    {
                        n = phaseNumbering.Count + 1;
                        phaseNumbering.Add(phase, n);
                    }
                    var banner = $"// {proc.Name} ===========";
                    testSvc.ReportProcedure($"analysis_{n:00}_{phase}.txt", banner, proc);
                    testSvc.GenerateUnitTestFromProcedure($"analysis_{n:00}_{phase}.rekoir", banner, proc);
                }
            }
        }
    }
}
