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
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// This interprocedural program analysis determines, for each procedure,
    /// which out parameters are not actually used, and removes them both from
    /// the procedure itself and also from all of its callers.
    /// </summary>
    public class UnusedOutValuesRemover
    {
        public static TraceSwitch trace = new TraceSwitch(typeof(UnusedOutValuesRemover).Name, "Trace removal of unused out values");

        private List<SsaTransform> ssts;
        private WorkList<SsaState> wl;
        private Program program;
        private Dictionary<Procedure, SsaState> procToSsa;
        private ProgramDataFlow dataFlow;
        private DecompilerEventListener eventListener;

        public UnusedOutValuesRemover(
            Program program,
            List<SsaTransform> ssts,
            ProgramDataFlow dataFlow,
            DecompilerEventListener eventListener)
        { 
            this.dataFlow = dataFlow;
            this.program = program;
            this.ssts = ssts;
            this.eventListener = eventListener;
            this.procToSsa = ssts
                .Select(t => t.SsaState)
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
                this.wl = new WorkList<SsaState>(ssts.Select(t => t.SsaState));
                while (wl.GetWorkItem(out SsaState ssa))
                {
                    if (this.eventListener.IsCanceled())
                        return;
                    var vp = new ValuePropagator(program.SegmentMap, ssa, eventListener);
                    vp.Transform();
                    change |= RemoveUnusedDefinedValues(ssa, wl);
                }
            } while (change);
            foreach (var proc in procToSsa.Keys)
            {
                var liveOut = CollectLiveOutStorages(proc);
                var flow = this.dataFlow[proc];
                flow.LiveOut = SummarizeStorageBitranges(flow.LiveOut.Concat(liveOut));
                flow.grfLiveOut = SummarizeFlagGroups(liveOut);
            }
        }

        private void CollectLiveOutStorages()
        {
            var wl = new WorkList<SsaState>(ssts.Select(s => s.SsaState));
            while (wl.GetWorkItem(out SsaState ssa))
            {
                var liveOut = CollectLiveOutStorages(ssa.Procedure);
                var changed = MergeLiveOut(dataFlow.ProcedureFlows[ssa.Procedure], liveOut);
                if (changed)
                {
                    wl.AddRange(program.CallGraph.Callees(ssa.Procedure).Select(p => procToSsa[p]));
                }
            }
        }

        [Conditional("DEBUG")]
        public void DumpLiveOut()
        {
            foreach (var flow in this.dataFlow.ProcedureFlows.OrderBy(de => de.Key.Name))
            {
                Debug.Print("== {0} ========", flow.Key.Name);
                var sw = new StringWriter();
                DataFlow.EmitRegisterValues("liveOut: ", flow.Value.LiveOut, sw);
                Debug.Print(sw.ToString());
            }
        }

        private Dictionary<Storage, BitRange> SummarizeStorageBitranges(
            IEnumerable<KeyValuePair<Storage, BitRange>> items)
        {
            var registerDomains = new Dictionary<StorageDomain, KeyValuePair<Storage, BitRange>>();
            var fpuStorages = new Dictionary<Storage, BitRange>();
            foreach (var item in items)
            {
                if (item.Key is RegisterStorage reg)
                {
                    if (!registerDomains.TryGetValue(reg.Domain, out var widestRange))
                    {
                        widestRange = new KeyValuePair<Storage, BitRange>(reg, item.Value);
                        registerDomains.Add(reg.Domain, widestRange);
                    }
                    else
                    {
                        int min = Math.Min(item.Value.Lsb, widestRange.Value.Lsb);
                        int max = Math.Min(item.Value.Msb, widestRange.Value.Msb);
                        if (item.Key.Covers(widestRange.Key))
                        {
                            widestRange = new KeyValuePair<Storage, BitRange>(reg, item.Value);
                            registerDomains[reg.Domain] = widestRange;
                        }
                    }
                }
                else if (item.Key is FpuStackStorage fpu)
                {
                    fpuStorages[fpu] = new BitRange(0, (int)fpu.BitSize);
                }
            }
            return registerDomains.Select(r => r.Value).Concat(fpuStorages)
                .ToDictionary(k => k.Key, v => v.Value);
        }

        private Dictionary<RegisterStorage, uint> SummarizeFlagGroups(Dictionary<Storage, BitRange> liveOut)
        {
            var results = new Dictionary<RegisterStorage, uint>();
            foreach (var de in liveOut)
            {
                if (de.Key is FlagGroupStorage grf && grf.FlagGroupBits != 0)
                {
                    results[grf.FlagRegister] = results.Get(grf.FlagRegister) | grf.FlagGroupBits;
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
            DebugEx.PrintIf(trace.TraceVerbose, "UVR: {0}", ssa.Procedure.Name);
            var liveOutStorages = this.dataFlow[ssa.Procedure].LiveOut;
            var deadStms = new HashSet<Statement>();
            var deadStgs = new HashSet<Storage>();
            FindDeadStatementsInExitBlock(ssa, liveOutStorages, deadStms, deadStgs);

            // Remove 'use' statements that are known to be dead from the exit block.
            foreach (var stm in deadStms)
            {
                DebugEx.PrintIf(trace.TraceVerbose, "UVR: {0}, deleting {1}", ssa.Procedure.Name, stm.Instruction);
                ssa.DeleteStatement(stm);
                change = true;
            }

            // If any instructions were removed, update the callers. 
            if (!ssa.Procedure.Signature.ParametersValid && deadStms.Count > 0)
            {
                DeadCode.Eliminate(ssa);
                foreach (Statement stm in program.CallGraph.CallerStatements(ssa.Procedure))
                {
                    var ci = stm.Instruction as CallInstruction;
                    if (ci == null)
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

        private static void FindDeadStatementsInExitBlock(
            SsaState ssa,
            Dictionary<Storage, BitRange> liveOutStorages,
            HashSet<Statement> deadStms, 
            HashSet<Storage> deadStgs)
        {
            foreach (var stm in ssa.Procedure.ExitBlock.Statements)
            {
                var use = stm.Instruction as UseInstruction;
                if (use == null)
                    continue;
                var ids = ExpressionIdentifierUseFinder.Find(ssa.Identifiers, use.Expression);
                foreach (var id in ids)
                {
                    var stg = id.Storage;
                    if (!liveOutStorages.ContainsKey(stg))
                    {
                        deadStgs.Add(stg);
                        deadStms.Add(stm);
                    }
                }
            }
        }

        /// <summary>
        /// Collects the storages that are live-out by looking at all known
        /// call sites and forming the union of all live storages at the call
        /// site.
        /// </summary>
        /// <param name="procCallee">Procedure that was called</param>
        /// <returns></returns>
        private Dictionary<Storage, BitRange> CollectLiveOutStorages(Procedure procCallee)
        {
            DebugEx.PrintIf(trace.TraceVerbose, "== Collecting live out storages of {0}", procCallee.Name);
            var liveOutStorages = new Dictionary<Storage, BitRange>();

            var sig = procCallee.Signature;
            if (sig.ParametersValid)
            {
                // Already have a signature, use that to define the 
                // set of live storages.

                if (!sig.HasVoidReturn)
                {
                    liveOutStorages.Add(
                        sig.ReturnValue.Storage,
                        new BitRange(0, sig.ReturnValue.DataType.BitSize));
                }
            }
            else
            {
                var urf = new UsedRegisterFinder(program.Architecture, dataFlow, eventListener);
                foreach (Statement stm in program.CallGraph.CallerStatements(procCallee))
                {
                    var ci = stm.Instruction as CallInstruction;
                    if (ci == null)
                        continue;
                    DebugEx.PrintIf(trace.TraceVerbose, "  {0}", ci);
                    var ssaCaller = this.procToSsa[stm.Block.Procedure];
                    foreach (var def in ci.Definitions)
                    {
                        var id = def.Expression as Identifier;
                        if (id == null)
                            continue;
                        var sid = ssaCaller.Identifiers[id];
                        if (sid.Uses.Count > 0)
                        {
                            var br = urf.Classify(ssaCaller, sid, true);
                            DebugEx.PrintIf(trace.TraceVerbose, "  {0}: {1}", sid.Identifier.Name, br);
                            if (liveOutStorages.TryGetValue(def.Storage, out BitRange brOld))
                            {
                                br = br | brOld;
                                liveOutStorages[def.Storage] = br;
                            }
                            else if (!br.IsEmpty)
                            {
                                liveOutStorages[def.Storage] = br;
                            }
                        }
                    }
                }
            }
            return liveOutStorages;
        }

        private bool MergeLiveOut(ProcedureFlow flow, Dictionary<Storage, BitRange> newLiveOut)
        {
            bool change = false;
            foreach (var de in newLiveOut)
            {
                if (flow.LiveOut.TryGetValue(de.Key, out BitRange oldRange))
                {
                    var range = oldRange | de.Value;
                    if (range != oldRange)
                    {
                        flow.LiveOut[de.Key] = range;
                        change = true;
                    }
                }
                else
                {
                    var range = de.Value;
                    flow.LiveOut[de.Key] = range;
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
        private bool RemoveDeadCallDefinitions(SsaState ssa, CallInstruction ci, HashSet<Storage> deadStgs)
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
                        sidDef.DefStatement = null;
                        sidDef.DefExpression = null;
                        return true;
                    }
                }
                return false;
            });
            return cRemoved > 0;
        }
    }
}