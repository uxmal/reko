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

using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LiveOutUse = Reko.Analysis.DataFlow.LiveOutUse;
using LiveOutFlagsUse = Reko.Analysis.DataFlow.LiveOutFlagsUse;

namespace Reko.Analysis
{
    /// <summary>
    /// This interprocedural program analysis determines, for each procedure,
    /// which out parameters are not actually used, and removes them both from
    /// the procedure itself and also from all of its callers.
    /// </summary>
    public class UnusedOutValuesRemover
    {
        public static readonly TraceSwitch trace = new (nameof(UnusedOutValuesRemover) , "Trace removal of unused out values")
        {
            Level = TraceLevel.Warning,
        };

        private readonly IEnumerable<SsaState> ssaStates;
        private readonly WorkList<SsaState> wl;
        private readonly Program program;
        private readonly Dictionary<Procedure, SsaState> procToSsa;
        private readonly ProgramDataFlow dataFlow;
        private readonly IDynamicLinker dynamicLinker;
        private readonly IServiceProvider services;
        private readonly IEventListener eventListener;

        public UnusedOutValuesRemover(
            Program program,
            IEnumerable<SsaState> ssaStates,
            ProgramDataFlow dataFlow,
            IDynamicLinker dynamicLinker,
            IServiceProvider services)
        {
            this.dataFlow = dataFlow;
            this.program = program;
            this.ssaStates = ssaStates;
            this.dynamicLinker = dynamicLinker;
            this.services = services;
            this.eventListener = services.RequireService<IEventListener>();
            this.wl = new WorkList<SsaState>();
            this.procToSsa = ssaStates
                .ToDictionary(s => s.Procedure);
        }

        public void Transform()
        {
            CollectLiveOutStorages();
            DumpLiveOut();
            bool change;
            do
            {
                if (eventListener.IsCanceled())
                    return;
                change = false;
                this.wl.AddRange(ssaStates);
                while (wl.TryGetWorkItem(out SsaState? ssa))
                {
                    if (this.eventListener.IsCanceled())
                        return;
                    var context = new AnalysisContext(program, ssa.Procedure, dynamicLinker, services, eventListener);
                    var vp = new ValuePropagator(context);
                    vp.Transform(ssa);
                    change |= RemoveUnusedDefinedValues(ssa, wl);
                    //DataFlowAnalysis.DumpWatchedProcedure("After RemoveUnusedDefinedValues", ssa.Procedure);
                    change |= RemoveLiveInStorages(ssa.Procedure, dataFlow[ssa.Procedure], wl);
                    //DataFlowAnalysis.DumpWatchedProcedure("After RemoveLiveInStorages", ssa.Procedure);
                }
            } while (change);
            foreach (var proc in procToSsa.Keys)
            {
                var liveOut = CollectLiveOutStorages(proc);
                var flow = this.dataFlow[proc];
                flow.BitsLiveOut = SummarizeStorageBitranges(flow.BitsLiveOut.Concat(liveOut));
                flow.LiveOutFlags = SummarizeFlagGroups(liveOut);
            }
        }

        /// <summary>
        /// Remove any storages in the ProcedureFlow <paramref name="flow"/> associated
        /// with the procedure <paramref name="proc"/> if they are dead.
        /// </summary>
        private bool RemoveLiveInStorages(Procedure proc, ProcedureFlow flow, WorkList<SsaState> wl)
        {
            var defs = proc.EntryBlock.Statements
                .Select(s => s.Instruction as DefInstruction)
                .Where(s => s is not null)
                .Select(s => s!.Identifier.Storage)
                .ToHashSet();
            var deadStgs = flow.BitsUsed.Keys.Except(defs).ToHashSet();
            bool changed = false;
            foreach (var d in deadStgs)
            {
                flow.BitsUsed.Remove(d);
                changed = true;
            }
            if (changed)
            {
                foreach (Statement stm in program.CallGraph.FindCallerStatements(proc))
                {
                    if (stm.Instruction is not CallInstruction ci)
                        continue;
                    var ssaCaller = this.procToSsa[stm.Block.Procedure];
                    if (RemoveDeadCallUses(ssaCaller, stm, ci, deadStgs))
                    {
                        wl.Add(ssaCaller);
                    }
                }
            }
            return changed;
        }

        /// <summary>
        /// For each procedure/SSA state, compute the variables that are live-out
        /// (and which bit ranges are live-out) and store them in the procedure flow.
        /// </summary>
        private void CollectLiveOutStorages()
        {
            var wl = WorkList.Create(ssaStates);
            while (wl.TryGetWorkItem(out SsaState? ssa))
            {
                var liveOut = CollectLiveOutStorages(ssa.Procedure);
                var flow = dataFlow.ProcedureFlows[ssa.Procedure];
                var changed = MergeLiveOut(flow, liveOut);
                if (changed)
                {
                    wl.AddRange(program.CallGraph.Callees(ssa.Procedure).Select(p => procToSsa[p]));
                }
            }
        }

        [Conditional("DEBUG")]
        public void DumpLiveOut()
        {
            if (!trace.TraceVerbose)
                return;
            foreach (var flow in this.dataFlow.ProcedureFlows.OrderBy(de => de.Key.Name))
            {
                Debug.Print("UVR: == {0} ========", flow.Key.Name);
                var sw = new StringWriter();
                DataFlow.EmitRegisterValues("liveOut: ", flow.Value.BitsLiveOut, sw);
                Debug.Print(sw.ToString());
            }
        }

        private static Dictionary<Storage, LiveOutUse> SummarizeStorageBitranges(
            IEnumerable<KeyValuePair<Storage, LiveOutUse>> items)
        {
            var registerDomains = new Dictionary<StorageDomain, KeyValuePair<Storage, LiveOutUse>>();
            var fpuStorages = new Dictionary<Storage, LiveOutUse>();
            var seqStorages = new Dictionary<Storage, LiveOutUse>();
            foreach (var item in items)
            {
                switch (item.Key)
                {
                case RegisterStorage reg:
                    if (!registerDomains.TryGetValue(reg.Domain, out var widestRange))
                    {
                        widestRange = new (reg, item.Value);
                        registerDomains.Add(reg.Domain, widestRange);
                    }
                    else
                    {
                        int min = Math.Min(item.Value.Range.Lsb, widestRange.Value.Range.Lsb);
                        int max = Math.Min(item.Value.Range.Msb, widestRange.Value.Range.Msb);
                        if (item.Key.Covers(widestRange.Key))
                        {
                            widestRange = new (reg, item.Value);
                            registerDomains[reg.Domain] = widestRange;
                        }
                    }
                    break;
                case FpuStackStorage fpu:
                    fpuStorages[fpu] = new(new BitRange(0, (int)fpu.BitSize), item.Value.Procedure);
                    break;
                case SequenceStorage seq:
                    seqStorages[seq] = item.Value;
                    break;
                }
            }
            return registerDomains
                .Select(r => r.Value)
                .Concat(fpuStorages)
                .Concat(seqStorages)
                .ToDictionary(k => k.Key, v => v.Value);
        }

        private static Dictionary<RegisterStorage, LiveOutFlagsUse> SummarizeFlagGroups(
            Dictionary<Storage, LiveOutUse> liveOut)
        {
            var results = new Dictionary<RegisterStorage, LiveOutFlagsUse>();
            foreach (var de in liveOut)
            {
                if (de.Key is FlagGroupStorage grf && grf.FlagGroupBits != 0)
                {
                    var lof = results.Get(grf.FlagRegister);
                    var flagBits = lof.Flags | grf.FlagGroupBits;
                    var proc = lof.Procedure ?? de.Value.Procedure;
                    results[grf.FlagRegister] = new(flagBits, proc);
                }
            }
            return results;
        }

        /// <summary>
        /// Remove any UseInstructions in the exit block of the procedure that 
        /// can be proved to be dead out.
        /// </summary>
        /// <param name="ssa">SSA of the procedure whose exit block is to be examined.</param>
        /// <param name="wl">Worklist of SSA states.</param>
        /// <returns>True if any change was made to SSA.</returns>
        public bool RemoveUnusedDefinedValues(SsaState ssa, WorkList<SsaState> wl)
        {
            bool change = false;
            trace.Verbose("UVR: {0}", ssa.Procedure.Name);
            var (deadStms, deadStgs) = FindDeadStatementsInExitBlock(ssa, this.dataFlow[ssa.Procedure].BitsLiveOut);

            // Remove 'use' statements that are known to be dead from the exit block.
            foreach (var stm in deadStms)
            {
                trace.Verbose("UVR: {0}, deleting {1}", ssa.Procedure.Name, stm.Instruction);
                ssa.DeleteStatement(stm);
                change = true;
            }

            // If any instructions were removed, update the callers. 
            if (!ssa.Procedure.Signature.ParametersValid && deadStms.Count > 0)
            {
                DeadCode.Eliminate(ssa);
                foreach (Statement stm in program.CallGraph.FindCallerStatements(ssa.Procedure))
                {
                    if (stm.Instruction is not CallInstruction ci)
                        continue;
                    var ssaCaller = this.procToSsa[stm.Block.Procedure];
                    if (RemoveDeadCallDefinitions(ssaCaller, ci, deadStgs))
                    {
                        wl.Add(ssaCaller);
                    }
                }
            }
            return change;
        }

        private static (HashSet<Statement> deadStms, HashSet<Storage> deadStgs) FindDeadStatementsInExitBlock(
            SsaState ssa,
            Dictionary<Storage, LiveOutUse> liveOutStorages)
        {
            var deadStms = new HashSet<Statement>();
            var deadStgs = new HashSet<Storage>();

            foreach (var stm in ssa.Procedure.ExitBlock.Statements)
            {
                if (stm.Instruction is not UseInstruction use)
                    continue;
                var ids = ExpressionIdentifierUseFinder.Find(use.Expression);
                foreach (var id in ids)
                {
                    var stg = id.Storage;
                    if (!liveOutStorages.Keys.Any(stgLiveOut => stgLiveOut.OverlapsWith(stg)))
                    {
                        deadStgs.Add(stg);
                        deadStms.Add(stm);
                    }
                }
            }
            return (deadStms, deadStgs);
        }

        /// <summary>
        /// Collects the storages that are live-out by looking at all known
        /// call sites and forming the union of all live storages at the call
        /// site.
        /// </summary>
        /// <param name="procCallee">Procedure that was called</param>
        /// <returns></returns>
        private Dictionary<Storage, LiveOutUse> CollectLiveOutStorages(
            Procedure procCallee)
        {
            trace.Verbose("== Collecting live out storages of {0}", procCallee.Name);
            var liveOutStorages = new Dictionary<Storage, LiveOutUse>();

            var sig = procCallee.Signature;
            if (sig.ParametersValid)
            {
                // Already have a signature, use that to define the 
                // set of live storages.

                if (!sig.HasVoidReturn)
                {
                    liveOutStorages.Add(
                        sig.Outputs[0].Storage,
                        new(new BitRange(0, sig.Outputs[0].DataType.BitSize),
                         procCallee));
                }
            }
            else
            {
                var urf = new UsedRegisterFinder(
                    program,
                    dataFlow,
                    Array.Empty<Procedure>(),
                    eventListener);
                foreach (Statement stm in program.CallGraph.FindCallerStatements(procCallee))
                {
                    if (stm.Instruction is not CallInstruction ci)
                        continue;
                    trace.Verbose("  {0}", ci);
                    var ssaCaller = this.procToSsa[stm.Block.Procedure];
                    foreach (var def in ci.Definitions)
                    {
                        if (def.Expression is not Identifier id)
                            continue;
                        var sid = ssaCaller.Identifiers[id];
                        if (sid.Uses.Count > 0)
                        {
                            var br = urf.Classify(ssaCaller, sid, def.Storage, true);
                            trace.Verbose("  {0}: {1}", sid.Identifier.Name, br);
                            if (liveOutStorages.TryGetValue(def.Storage, out var brOld))
                            {
                                br |= brOld.Range;
                                liveOutStorages[def.Storage] = new(br, brOld.Procedure);
                            }
                            else if (!br.IsEmpty)
                            {
                                liveOutStorages[def.Storage] = new(br, stm.Block.Procedure);
                            }
                        }
                    }
                }
            }
            return liveOutStorages;
        }

        private static bool MergeLiveOut(
            ProcedureFlow flow,
            Dictionary<Storage, LiveOutUse> newLiveOut)
        {
            bool change = false;
            foreach (var de in newLiveOut)
            {
                if (flow.BitsLiveOut.TryGetValue(de.Key, out var oldRange))
                {
                    var range = oldRange.Range | de.Value.Range;
                    if (range != oldRange.Range)
                    {
                        flow.BitsLiveOut[de.Key] = new(range, oldRange.Procedure);
                        change = true;
                    }
                }
                else
                {
                    var range = de.Value.Range;
                    flow.BitsLiveOut[de.Key] = new(range, de.Value.Procedure);
                    change = true;
                }
            }
            return change;
        }

        /// <summary>
        /// From the call instruction <paramref name="ci"/>, removes those 'def's 
        /// that have been marked as dead in <paramref name="deadStgs"/>.
        /// </summary>
        /// <param name="ssa">The SSA state of the procedure containing <paramref name="ci"/>.</param>
        /// <param name="ci">The call instruction to mutate.</param>
        /// <param name="deadStgs">Set of dead storages to remove.</param>
        /// <returns>True if any 'def's were removed.</returns>
        private static bool RemoveDeadCallDefinitions(
            SsaState ssa,
            CallInstruction ci,
            HashSet<Storage> deadStgs)
        {
            //$REVIEW: this code is similar to DeadCode.AdjustCallWithDeadDefinitions
            // Move it to SsaState? Somewhere else?
            int cRemoved = ci.Definitions.RemoveWhere(def =>
            {
                if (def.Expression is Identifier id)
                {
                    if (deadStgs.Contains(id.Storage))
                    {
                        var sidDef = ssa.Identifiers[id];
                        sidDef.DefStatement = null!;
                        return true;
                    }
                }
                return false;
            });
            return cRemoved > 0;
        }

        /// <summary>
        /// From the call instruction <paramref name="ci"/>, in statement <paramref name="stmCall"/>,
        /// removes those 'use's that have been marked as dead in <paramref name="deadStgs"/>.
        /// </summary>
        private static bool RemoveDeadCallUses(
            SsaState ssa,
            Statement stmCall,
            CallInstruction ci,
            HashSet<Storage> deadStgs)
        {
            //$REVIEW: this code is similar to DeadCode.AdjustCallWithDeadDefinitions
            // Move it to SsaState? Somewhere else?
            int cRemoved = ci.Uses.RemoveWhere(use =>
            {
                if (use.Expression is Identifier id)
                {
                    if (deadStgs.Contains(id.Storage))
                    {
                        var sidUse = ssa.Identifiers[id];
                        sidUse.Uses.Remove(stmCall);
                        return true;
                    }
                }
                return false;
            });
            return cRemoved > 0;
        }
    }
}