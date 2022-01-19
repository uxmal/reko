#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Hll.C;
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
    // https://patterns.eecs.berkeley.edu/?page_id=609
    //    Examples of Task graphs.
    public class DataFlowAnalysis
	{
		private readonly Program program;
        private readonly IServiceProvider services;
		private readonly DecompilerEventListener eventListener;
        private readonly IDynamicLinker dynamicLinker;
        private readonly Dictionary<string, int> phaseNumbering;
        private readonly ProgramDataFlow flow;
        private List<SsaTransform>? ssts;
        private HashSet<Procedure>? sccProcs;

        public DataFlowAnalysis(
            Program program,
            IDynamicLinker dynamicLinker,
            IServiceProvider services)
		{
			this.program = program;
            this.dynamicLinker = dynamicLinker;
            this.services = services;
            this.eventListener = services.RequireService<DecompilerEventListener>();
			this.flow = new ProgramDataFlow(program);
            this.phaseNumbering = new Dictionary<string, int>();
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
            ClearTestFiles();
            UntangleProcedures();
            if (eventListener.IsCanceled()) return;
            BuildExpressionTrees();
        }

        /// <summary>
        /// Summarizes the net effect each procedure has on registers,
        /// then removes trashed registers that aren't live-out.
        /// </summary>
        public List<SsaTransform> UntangleProcedures()
        {
            eventListener.ShowProgress("Rewriting procedures.", 0, program.Procedures.Count);

            var ssts = new List<SsaTransform>();
            IntraBlockDeadRegisters.Apply(program, eventListener);
            if (eventListener.IsCanceled())
                return ssts;

            AdjacentBranchCollector.Transform(program, eventListener);
            if (eventListener.IsCanceled())
                return ssts;

            ssts = RewriteProceduresToSsa();
            if (eventListener.IsCanceled())
                return ssts;

            // Recreate user-defined signatures. It should prevent type
            // inference between user-defined parameters and other expressions
            var usb = new UserSignatureBuilder(program);
            usb.BuildSignatures(eventListener);
            if (eventListener.IsCanceled())
                return ssts;

            // Discover ssaId's that are live out at each call site.
            // Delete all others.
            var uvr = new UnusedOutValuesRemover(
                program,
                ssts.Select(sst => sst.SsaState),
                this.flow,
                dynamicLinker,
                eventListener);
            uvr.Transform();
            if (eventListener.IsCanceled())
                return ssts;

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
            sscf.FindAll();
            return ssts;
        }

        /// <summary>
        /// This callback is called from the SccFinder, which passes it a list
        /// of Procedures that form a SCC.
        /// </summary>
        /// <param name="procs"></param>
        private void UntangleProcedureScc(IList<Procedure> procs)
        {
            if (eventListener.IsCanceled())
                return;
            Debug.Print("== SCC: {0} ===", string.Join(",",procs));
            this.sccProcs = procs.ToHashSet();
            flow.CreateFlowsFor(procs);

            // Convert all procedures in the SCC to SSA form and perform
            // value propagation.
            DumpWatchedProcedure("scc", "Before SSA", procs);
            var ssts = procs.Select(ConvertToSsa).ToArray();
            this.ssts!.AddRange(ssts);
            DumpWatchedProcedure("esv", "After extra stack vars", ssts);
            if (eventListener.IsCanceled()) return;

            // At this point, the computation of ProcedureFlow is possible.
            var trf = new TrashedRegisterFinder(program, flow, ssts, this.eventListener);
            trf.Compute();
            if (eventListener.IsCanceled()) return;

            // New stack based variables may be available now.
            foreach (var sst in ssts)
            {
                if (eventListener.IsCanceled())
                    return;
                var vp = new ValuePropagator(program.SegmentMap, sst.SsaState, program.CallGraph, dynamicLinker, this.eventListener);
                vp.Transform();
                sst.RenameFrameAccesses = true;
                sst.Transform();
                DumpWatchedProcedure("esv2", "After extra stack vars 2", sst.SsaState.Procedure);
            }

            foreach (var sst in ssts)
            {
                if (eventListener.IsCanceled())
                    return;
                var ssa = sst.SsaState;
                RemoveImplicitRegistersFromHellNodes(ssa);
                var sac = new SegmentedAccessClassifier(ssa);
                sac.Classify();
                var prj = new ProjectionPropagator(ssa, sac);
                prj.Transform();
                DumpWatchedProcedure("prpr", "After projection propagation", ssa.Procedure);
                //var stfu = new StoreFuser(ssa);
                //DumpWatchedProcedure("stfu", "After store fusion", ssa.Procedure);
            }

            var uid = new UsedRegisterFinder(flow, procs, this.eventListener);
            foreach (var sst in ssts)
            {
                if (eventListener.IsCanceled())
                    return;
                var ssa = sst.SsaState;
                RemovePreservedUseInstructions(ssa);
                DeadCode.Eliminate(ssa);
                uid.ComputeLiveIn(ssa, true);
                var procFlow = flow[ssa.Procedure];
                RemoveDeadArgumentsFromCalls(ssa.Procedure, procFlow, ssts);
                DumpWatchedProcedure("dcar", "After dead call argument removal", ssa.Procedure);
            }
            eventListener.Advance(procs.Count);
        }

        private void RemoveImplicitRegistersFromHellNodes(SsaState ssa)
        {
            foreach (var stm in ssa.Procedure.Statements)
            {
                RemoveImplicitRegistersFromHellNode(ssa, stm);
            }
        }

        private void RemoveImplicitRegistersFromHellNode(
            SsaState ssa,
            Statement stm)
        {
            if (stm.Instruction is not CallInstruction ci)
                return;
            if (ci.Callee is ProcedureConstant pc)
            {
                if (!(pc.Procedure is ExternalProcedure))
                    return;
            }
            var trashedRegisters = program.Platform.CreateTrashedRegisters();
            var platform = program.Platform;
            foreach (var use in ci.Uses.ToList())
            {
                if (IsPreservedRegister(trashedRegisters, use.Storage) ||
                    IsStackStorageOfPreservedRegister(
                        ssa,
                        trashedRegisters,
                        use) ||
                    (use.Storage is RegisterStorage reg &&
                     platform.IsImplicitArgumentRegister(reg)))
                {
                    ci.Uses.Remove(use);
                    ssa.RemoveUses(stm, use.Expression);
                }
            }
        }

        private bool IsStackStorageOfPreservedRegister(
            SsaState ssa,
            HashSet<RegisterStorage> trashedRegisters,
            CallBinding use)
        {
            if (use.Storage is not StackStorage)
                return false;
            if (use.Expression is not Identifier id)
                return false;
            if (!(ssa.Identifiers[id].IsOriginal))
                return false;
            if (!IsPreservedRegister(trashedRegisters, id.Storage))
                return false;
            return true;
        }

        private bool IsPreservedRegister(
            HashSet<RegisterStorage> trashedRegisters,
            Storage stg)
        {
            if (!(stg is RegisterStorage))
                return false;
            if (trashedRegisters.Count == 0)
                return false;
            return !trashedRegisters.Where(r => r.OverlapsWith(stg)).Any();
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
            ProcedureFlow flow,
            IEnumerable<SsaTransform> ssts)
        {
            var mpProcSsa = ssts.ToDictionary(d => d.SsaState.Procedure, d => d.SsaState);
            foreach (Statement stm in program.CallGraph.CallerStatements(proc))
            {
                if (!mpProcSsa.TryGetValue(stm.Block.Procedure, out var ssa))
                    continue;

                // We have a call statement that calls `proc`. Make sure 
                // that only arguments present in the procedure flow are present.
                if (stm.Instruction is not CallInstruction call)
                    continue;
                var filteredUses = ProcedureFlow.IntersectCallBindingsWithUses(call.Uses, flow.BitsUsed)
                    .ToArray();
                ssa.RemoveUses(stm);
                call.Uses.Clear();
                call.Uses.UnionWith(filteredUses);
                ssa.AddUses(stm);
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
            eventListener.ShowProgress("Building expressions.", 0, program.Procedures.Count);
            foreach (var sst in this.ssts!)
            {
                var ssa = sst.SsaState;
                try
                {
                    if (program.User.AggressiveBranchRemoval)
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

                    var vp = new ValuePropagator(program.SegmentMap, ssa, program.CallGraph, dynamicLinker,  eventListener);
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
                    var web = new WebBuilder(program, ssa, program.InductionVariables, eventListener);
                    web.Transform();
                    ssa.ConvertBack(false);

                    DumpWatchedProcedure("dfa", "After data flow analysis", ssa.Procedure);
                }
                catch (Exception ex)
                {
                    eventListener.Error(
                        eventListener.CreateProcedureNavigator(program, ssa.Procedure),
                        ex,
                        "An internal error occurred while building the expressions of {0}",
                        ssa.Procedure.Name);
                }
                eventListener.Advance(1);
            }
        }

        /// <summary>
        /// Converts all registers and stack accesses to SSA variables.
        /// </summary>
        /// <param name="proc"></param>
        /// <returns>The SsaTransform for the procedure.</returns>
        public SsaTransform ConvertToSsa(Procedure proc)
        {
            if (!program.NeedsSsaTransform)
            {
                // Some formats, like LLVM, already have phi functions.
                var sst = new SsaTransform(program, proc, sccProcs!, dynamicLinker, this.ProgramDataFlow);
                return sst;
            }

            try
            {
                // Transform the procedure to SSA state. When encountering 'call'
                // instructions, they can be to functions already visited. If so,
                // they have a "ProcedureFlow" associated with them. If they have
                // not been visited, or are computed destinations  (e.g. vtables)
                // they will have no "ProcedureFlow" associated with them yet, in
                // which case the the SSA treats the call as a "hell node".
                var sst = new SsaTransform(program, proc, sccProcs!, dynamicLinker, this.ProgramDataFlow);
                var ssa = sst.Transform();
                DumpWatchedProcedure("ssa", "After SSA", ssa);
                // Merge unaligned memory accesses.
                var fuser = new UnalignedMemoryAccessFuser(ssa);
                fuser.Transform();

                // Fuse additions and subtractions that are linked by the carry flag.
                var larw = new LongAddRewriter(ssa, eventListener);
                larw.Transform();
                DumpWatchedProcedure("larw", "After long add rewriter", ssa);

                // After value propagation expressions like (x86) 
                // mem[esp_42+4] will have been converted to mem[fp - 30]. 
                // We also hope that procedure constants
                // kept in registers are propagated to the corresponding call
                // sites.
                var vp = new ValuePropagator(program.SegmentMap, ssa, program.CallGraph, dynamicLinker, eventListener);
                vp.Transform();
                DumpWatchedProcedure("vp", "After first VP", ssa);

                // Value propagation may uncover more opportunities.
                larw = new LongAddRewriter(ssa, eventListener);
                larw.Transform();
                DumpWatchedProcedure("larw2", "After second long add rewriter", ssa);

                // Eliminate condition codes by discovering uses of ccodes
                // and replacing them with higher-level constructs.
                var cce = new ConditionCodeEliminator(program, ssa, eventListener);
                cce.Transform();
                vp.Transform();
                DumpWatchedProcedure("cce", "After CCE", ssa);

                // Now compute SSA for the stack-based variables as well. That is:
                // mem[fp - 30] becomes wLoc30, while 
                // mem[fp + 30] becomes wArg30.
                // This allows us to compute the dataflow of this procedure.
                sst.RenameFrameAccesses = true;
                sst.Transform();
                DumpWatchedProcedure("ssaframe", "After SSA frame accesses", ssa);

                var icrw = new IndirectCallRewriter(program, ssa, eventListener);
                while (!eventListener.IsCanceled() && icrw.Rewrite())
                {
                    vp.Transform();
                    sst.RenameFrameAccesses = true;
                    sst.Transform();
                }

                var fpuGuesser = new FpuStackReturnGuesser(ssa, eventListener);
                fpuGuesser.Transform();
                DumpWatchedProcedure("fpug", "After FPU stack guesser", ssa);

                // By placing use statements in the exit block, we will collect
                // reaching definitions in the use statements.
                sst.AddUsesToExitBlock();
                sst.RemoveDeadSsaIdentifiers();

                // Backpropagate stack pointer from procedure return.
                var spBackpropagator = new StackPointerBackpropagator(ssa, eventListener);
                spBackpropagator.BackpropagateStackPointer();
                DumpWatchedProcedure("spbp", "After SP BP", ssa);

                // Propagate those newly created stack-based identifiers.
                vp.Transform();
                DumpWatchedProcedure("vp2", "After VP2", ssa);

                return sst;
            }
            catch (Exception ex)
            {
                var nl = Environment.NewLine;
                var banner = $"// {proc.Name} ==========={nl}{ex.Message}{nl}{ex.StackTrace}{nl}{nl}";
                services.GetService<ITestGenerationService>()?
                    .ReportProcedure($"analysis_{99:00}_crash.txt", banner, proc);
                throw;
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
            if (program.User.DebugTraceProcedures.Contains(proc.Name) ||
                proc.Name == "usb_device_info")
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
