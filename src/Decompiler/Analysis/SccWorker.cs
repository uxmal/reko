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
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Worker class that performs data flow analysis on a strongly
    /// connected component (SCC) of the call graph.
    /// </summary>
    /// <remarks>
    /// This worker class will run in its own concurrent Task. Therefore
    /// code needs to make sure no shared state is mutated. Prefer using
    /// immutable or read-only data whenever possible.
    /// </remarks>
    public class SccWorker
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(SccWorker), "");

        private readonly DataFlowAnalysis dfa;
        private readonly Procedure[] procs;
        private readonly IDynamicLinker dynamicLinker;
        private readonly IServiceProvider services;
        private readonly IReadOnlyProgram program;
        private readonly ProgramDataFlow flow;  //$MUTABLE
        private readonly IReadOnlySet<Procedure> sccProcs;
        private readonly IDecompilerEventListener eventListener;

        public SccWorker(
            DataFlowAnalysis dfa,
            Procedure[] sccProcs, 
            IDynamicLinker dynamicLinker,
            IServiceProvider services)
        {
            this.dfa = dfa;
            this.procs = sccProcs;
            this.dynamicLinker = dynamicLinker;
            this.services = services;
            this.program = dfa.Program;
            this.flow = dfa.ProgramDataFlow;
            this.sccProcs = sccProcs.ToHashSet();
            this.eventListener = services.RequireService<IDecompilerEventListener>();
        }

        /// <summary>
        /// This callback is called from the SccFinder, which passes it a list
        /// of Procedures that form a SCC.
        /// </summary>
        public SsaTransform[] Transform()
        {
            if (eventListener.IsCanceled())
                return Array.Empty<SsaTransform>();
            Debug.Print("== SCC: {0} ===", string.Join(",", sccProcs));
            flow.CreateFlowsFor(sccProcs);

            // Convert all procedures in the SCC to SSA form and perform
            // value propagation.
            dfa.DumpWatchedProcedure("worker", "Before SSA", procs);
            var ssts = procs.Select(ConvertToSsa).ToArray();
            dfa.DumpWatchedProcedure("esv", "After extra stack vars", ssts);
            if (eventListener.IsCanceled()) return ssts;

            // At this point, the computation of ProcedureFlow is possible.
            var trf = new TrashedRegisterFinder(program, flow, ssts, this.eventListener);
            trf.Compute();
            if (eventListener.IsCanceled()) return ssts;

            // New stack based variables may be available now.
            foreach (var sst in ssts)
            {
                if (eventListener.IsCanceled())
                    return ssts;
                var vp = new ValuePropagator(program, sst.SsaState, this.dynamicLinker, services);
                vp.Transform();
                sst.RenameFrameAccesses = true;
                sst.Transform();
                dfa.DumpWatchedProcedure("esv2", "After extra stack vars 2", sst.SsaState.Procedure);
            }

            foreach (var sst in ssts)
            {
                if (eventListener.IsCanceled())
                    return ssts;
                var ssa = sst.SsaState;
                RemoveImplicitRegistersFromHellNodes(ssa);
                var sac = new SegmentedAccessClassifier(ssa);
                sac.Classify();
                var prj = new ProjectionPropagator(ssa, sac);
                prj.Transform();
                dfa.DumpWatchedProcedure("prpr", "After projection propagation", ssa.Procedure);

                // Stores of sliced long variables can be fused.
                var stfu = new StoreFuser(ssa, eventListener);
                stfu.Transform();
                dfa.DumpWatchedProcedure("stfu", "After store fusion", ssa.Procedure);
            }

            var uid = new UsedRegisterFinder(program, flow, procs, this.eventListener);
            foreach (var sst in ssts)
            {
                if (eventListener.IsCanceled())
                    return ssts;
                var ssa = sst.SsaState;
                RemovePreservedUseInstructions(ssa);
                DeadCode.Eliminate(ssa);
                uid.ComputeLiveIn(ssa, true);
                var procFlow = flow[ssa.Procedure];
                RemoveDeadArgumentsFromCalls(ssa.Procedure, procFlow, ssts);
                dfa.DumpWatchedProcedure("dcar", "After dead call argument removal", ssa.Procedure);
            }
            eventListener.Progress.Advance(ssts.Length);
            return ssts;
        }

        /// <summary>
        /// Converts all registers and stack accesses to SSA variables.
        /// </summary>
        /// <param name="proc"></param>
        /// <returns>The <sse cref="SsaTransform" /> for the procedure.</returns>
        public SsaTransform ConvertToSsa(Procedure proc)
        {
            if (!program.NeedsSsaTransform)
            {
                // Some formats, like LLVM, already have phi functions.
                var sst = new SsaTransform(program, proc, sccProcs!, this.dynamicLinker, this.flow);
                return sst;
            }

            var analysisFactory = proc.Architecture.CreateExtension<IAnalysisFactory>();
            var context = new AnalysisContext(program, sccProcs, dynamicLinker, services, eventListener);

            try
            {
                // Transform the procedure to SSA state. When encountering 'call'
                // instructions, they can be to functions already visited. If so,
                // they have a "ProcedureFlow" associated with them. If they have
                // not been visited, or are computed destinations  (e.g. vtables)
                // they will have no "ProcedureFlow" associated with them yet, in
                // which case the the SSA treats the call as a "hell node".
                var sst = new SsaTransform(program, proc, sccProcs!, this.dynamicLinker, this.flow);
                var ssa = sst.Transform();
                dfa.DumpWatchedProcedure("ssa", "After SSA", ssa);

                ssa = dfa.RunAnalyses(analysisFactory, context, AnalysisStage.AfterRegisterSsa, ssa);

                // Merge unaligned memory accesses.
                var fuser = new UnalignedMemoryAccessFuser(ssa);
                fuser.Transform();

                // Fuse additions and subtractions that are linked by the carry flag.
                var larw = new LongAddRewriter(ssa, eventListener);
                larw.Transform();
                dfa.DumpWatchedProcedure("larw", "After long add rewriter", ssa);

                // After value propagation expressions like (x86) 
                // mem[esp_42+4] will have been converted to mem[fp - 30]. 
                // We also hope that procedure constants
                // kept in registers are propagated to the corresponding call
                // sites.
                var vp = new ValuePropagator(program, ssa, this.dynamicLinker, services);
                vp.Transform();
                dfa.DumpWatchedProcedure("vp", "After first VP", ssa);

                // Value propagation may uncover more opportunities.
                larw = new LongAddRewriter(ssa, eventListener);
                larw.Transform();
                dfa.DumpWatchedProcedure("larw2", "After second long add rewriter", ssa);

                // Eliminate condition codes by discovering uses of ccodes
                // and replacing them with higher-level constructs.
                var cce = new ConditionCodeEliminator(program, ssa, eventListener);
                cce.Transform();
                vp.Transform();
                dfa.DumpWatchedProcedure("cce", "After CCE", ssa);

                var efif = new EscapedFrameIntervalsFinder(
                    program, flow, ssa, eventListener);
                var escapedFrameIntervals = efif.Find();
                if (escapedFrameIntervals.Count > 0)
                {
                    var csvt = new ComplexStackVariableTransformer(
                        ssa, escapedFrameIntervals, eventListener);
                    csvt.Transform();
                }

                // Now compute SSA for the stack-based variables as well. That is:
                // mem[fp - 30] becomes wLoc30, while 
                // mem[fp + 30] becomes wArg30.
                // This allows us to compute the dataflow of this procedure.
                sst.RenameFrameAccesses = true;
                sst.Transform();
                dfa.DumpWatchedProcedure("ssaframe", "After SSA frame accesses", ssa);

                var icrw = new IndirectCallRewriter(program, ssa, eventListener);
                while (!eventListener.IsCanceled() && icrw.Rewrite())
                {
                    vp.Transform();
                    sst.RenameFrameAccesses = true;
                    sst.Transform();
                }

                var fpuGuesser = new FpuStackReturnGuesser(ssa, eventListener);
                fpuGuesser.Transform();
                dfa.DumpWatchedProcedure("fpug", "After FPU stack guesser", ssa);

                // By placing use statements in the exit block, we will collect
                // reaching definitions in the use statements.
                sst.AddUsesToExitBlock();
                sst.RemoveDeadSsaIdentifiers();

                // Backpropagate stack pointer from procedure return.
                var spBackpropagator = new StackPointerBackpropagator(ssa, eventListener);
                spBackpropagator.BackpropagateStackPointer();
                dfa.DumpWatchedProcedure("spbp", "After SP BP", ssa);

                // Propagate newly created stack-relative identifiers and transform
                // any new frame-relative memory accesses to identifiers.
                vp.Transform();
                sst.Transform();
                dfa.DumpWatchedProcedure("vp2", "After VP2", ssa);

                // Guess arguments to functions whose signatures we don't know.
                var argGuesser = new ArgumentGuesser(program.Platform, ssa, eventListener);
                argGuesser.Transform();
                dfa.DumpWatchedProcedure("argg", "After argument guessing", ssa);

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

        private static bool IsStackStorageOfPreservedRegister(
            SsaState ssa,
            IReadOnlySet<RegisterStorage> trashedRegisters,
            IReadOnlySet<RegisterStorage> preservedRegisters,
            CallBinding use)
        {
            if (use.Storage is not StackStorage)
                return false;
            if (use.Expression is not Identifier id)
                return false;
            if (!(ssa.Identifiers[id].IsOriginal))
                return false;
            return IsPreservedRegister(trashedRegisters, preservedRegisters, id.Storage);
        }

        /// <summary>
        /// Returns true if the register <paramref name="stg"/> is not
        /// one of the supplied <paramref name="trashedRegisters"/>.
        /// The idea is to eliminate registers that are not supposed
        /// to be modified by the ABI of the processor or platform.
        /// </summary>
        /// <param name="trashedRegisters"></param>
        /// <param name="preservedRegisters"></param>
        /// <param name="stg"></param>
        /// <returns></returns>
        private static bool IsPreservedRegister(
            IReadOnlySet<RegisterStorage> trashedRegisters,
            IReadOnlySet<RegisterStorage> preservedRegisters,
            Storage stg)
        {
            if (stg is not RegisterStorage reg)
                return false;
            if (preservedRegisters.Any(r => r.Covers(reg)))
                return true;
            if (trashedRegisters.Count == 0)
                return false;
            return !trashedRegisters.Where(r => r.OverlapsWith(stg)).Any();
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
                if (pc.Procedure is not ExternalProcedure)
                    return;
            }
            var trashedRegisters = program.Platform.TrashedRegisters;
            var preservedRegisters = program.Platform.PreservedRegisters;
            var platform = program.Platform;
            foreach (var use in ci.Uses.ToList())
            {
                if (IsPreservedRegister(trashedRegisters, preservedRegisters, use.Storage) ||
                    IsStackStorageOfPreservedRegister(
                        ssa,
                        trashedRegisters,
                        preservedRegisters,
                        use) ||
                    (use.Storage is RegisterStorage reg &&
                     platform.IsImplicitArgumentRegister(reg)))
                {
                    ci.Uses.Remove(use);
                    ssa.RemoveUses(stm, use.Expression);
                }
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
            foreach (Statement stm in program.CallGraph.FindCallerStatements(proc))
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
        /// Remove any Use instruction that uses identifiers that are marked as
        /// preserved.
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
    }
}
