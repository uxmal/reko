#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
 .
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
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// This class implements a recursive traversal strategy to discover code
    /// in a binary. The algorithm creates a <see cref="ProcedureWorker"/>
    /// for each known "seed". Seeds are obtained from the binary itself (such
    /// as entry points and exported functions) and from the user (such as
    /// user-specified functions). 
    /// </summary>
    public class RecursiveScanner : AbstractScanner
    {
        private readonly ConcurrentQueue<ProcedureWorker> wl;
        private readonly ConcurrentDictionary<Address, ProcedureWorker> activeWorkers;
        private readonly ConcurrentDictionary<Address, ProcedureWorker> suspendedWorkers;

        /// <summary>
        /// Constructs a new <see cref="RecursiveScanner"/> instance.
        /// </summary>
        /// <param name="program">Program being analyzed.</param>
        /// <param name="provenance"><see cref="ProvenanceType">Provenance</see> of
        /// the procedures discovered by this scanner.</param>
        /// <param name="dynamicLinker"><see cref="IDynamicLinker"/> instance used
        /// to resolve calls to imported procedures.</param>
        /// <param name="listener"><see cref="IEventListener"/></param>
        /// <param name="services"></param>
        public RecursiveScanner(
            Program program,
            ProvenanceType provenance,
            IDynamicLinker dynamicLinker,
            IEventListener listener,
            IServiceProvider services)
            : this(program, new ScanResultsV2(), provenance, dynamicLinker, listener, services)
        { }

        /// <summary>
        /// Constructs a new <see cref="RecursiveScanner"/> instance.
        /// </summary>
        /// <param name="program">Program being analyzed.</param>
        /// <param name="sr">Collected scanning results.</param>
        /// <param name="provenance"><see cref="ProvenanceType">Provenance</see> of
        /// the procedures discovered by this scanner.</param>
        /// <param name="dynamicLinker"><see cref="IDynamicLinker"/> instance used
        /// to resolve calls to imported procedures.</param>
        /// <param name="listener"><see cref="IEventListener"/></param>
        /// <param name="services"></param>
        public RecursiveScanner(
            Program program, 
            ScanResultsV2 sr,
            ProvenanceType provenance,
            IDynamicLinker dynamicLinker,
            IEventListener listener,
            IServiceProvider services)
            : base(program, sr, provenance, dynamicLinker, listener, services)
        {
            this.wl = new ConcurrentQueue<ProcedureWorker>();
            this.activeWorkers = new();
            this.suspendedWorkers = new();
        }

        /// <inheritdoc/>
        public override bool AllowSpeculativeTransfers => true;

        /// <inheritdoc/>
        public ScanResultsV2 ScanProgram()
        {
            var seeds = CollectSeeds();
            ProcessWorkers(seeds.Select(MakeSeedWorker));
            return sr;
        }

        /// <summary>
        /// Starts scanning a procedure at the given address.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> used to
        /// lift the procedure's machine code to RTL instructions.</param>
        /// <param name="addr">Address at which the procedure starts.</param>
        /// <param name="procedureName">Optional name for the procedure.</param>
        /// <param name="state"><see cref="ProcessorState"/> to use while processing
        /// the procedure.</param>
        /// <returns>An modified <see cref="ScanResultsV2"/> instance.
        /// </returns>
        public ScanResultsV2 ScanProcedure(IProcessorArchitecture arch, Address addr, string? procedureName, ProcessorState state)
        {
            var symbol = ImageSymbol.Create(SymbolType.Code, arch, addr, procedureName);
            symbol.ProcessorState = state;
            //$TODO: scannerv2 pass in provenance in parameters of interface method ScanProcedure.
            //$TODO: add existing Procedures
            var worker = MakeSeedWorker(new(addr, (symbol, ProvenanceType.Image)));
            ProcessWorkers(new[] { worker });
            return sr;
        }

        /// <summary>
        /// Collects the starting points, or seeds, of the recursive traversal
        /// algorithm. User-provided information always takes precedence before
        /// symbols obtained from the image.
        /// </summary>
        /// <returns>A dictionary of seeds, indexed by address.</returns>
        private Dictionary<Address, (ImageSymbol, ProvenanceType)> CollectSeeds()
        {
            var result = new Dictionary<Address, (ImageSymbol, ProvenanceType)>();
            foreach (var (addr, up) in program.User.Procedures)
            {
                //$TODO: scannerV2 how to respect no-decompile flag.
                result.Add(addr, (ImageSymbol.Procedure(
                    program.Architecture,
                    addr,
                    up.Name,
                    null,
                    up.Signature),
                    ProvenanceType.UserInput));
            }
            foreach (var sym in program.EntryPoints)
            {
                result.TryAdd(sym.Key, (sym.Value, ProvenanceType.ImageEntrypoint));
            }
            foreach (var sym in program.ImageSymbols)
            {
                if ((sym.Value.Type == SymbolType.Code ||
                    sym.Value.Type == SymbolType.Procedure)
                    && IsExecutableAddress(sym.Value.Address))
                {
                    result.TryAdd(sym.Key, (sym.Value, ProvenanceType.Image));
                }
            }
            return result;
        }

        private ProcedureWorker? MakeSeedWorker(KeyValuePair<Address, (ImageSymbol, ProvenanceType)> seed)
        {
            var (sym, provenance) = seed.Value;
            var name = sym.Name ?? program.NamingPolicy.ProcedureName(seed.Key);
            var proc = new RtlProcedure(sym.Architecture, seed.Key, name, provenance, new HashSet<RtlBlock>());
            if (sr.Procedures.TryAdd(proc.Address, proc))
            {
                var state = sym.ProcessorState ?? sym.Architecture.CreateProcessorState();
                return new ProcedureWorker(this, proc, state, rejectMask, listener);
            }
            else
            {
                // This procedure appears to already have been scanned.
                return null;
            }
        }

        private ProcedureWorker? MakeHeuristicWorker(IProcessorArchitecture arch, Address address)
        {
            var name = program.NamingPolicy.ProcedureName(address);
            var proc = new RtlProcedure(arch, address, name, ProvenanceType.Heuristic, new HashSet<RtlBlock>());
            if (sr.Procedures.TryAdd(address, proc))
            {
                var state = arch.CreateProcessorState();
                return new ProcedureWorker(this, proc, state, rejectMask, listener);
            }
            else
            {
                // A procedure appears to already exist at the given address.
                return null;
            }
        }

        private void ProcessWorkers(IEnumerable<ProcedureWorker?> seeds)
        {
            foreach (var worker in seeds)
            {
                if (worker is null ||
                    !this.activeWorkers.TryAdd(worker.Procedure.Address, worker))
                    continue;

                this.wl.Enqueue(worker);
                while (wl.TryDequeue(out var w))
                {
                    w.Run();
                }
            }
        }

        /// <summary>
        /// Attempts to start a <see cref="ProcedureWorker"/> for a 
        /// procedure at the given address. If a worker is already
        /// working on the procedure, it is returned in the output parameter.
        /// </summary>
        /// <param name="addrProc">Address of the new procedure.</param>
        /// <param name="state">ProcessorState to use with the new procedure.</param>
        /// <param name="worker">Resulting <see cref="ProcedureWorker"/> if 
        /// that procedure isn't finished processing.</param>
        /// <returns>True if a new or existing <see cref="ProcedureWorker"/> is
        /// returned to caller; false otherwise.
        /// </returns>
        public bool TryStartProcedureWorker(
            Address addrProc,
            ProcessorState state,
            [MaybeNullWhen(false)] out ProcedureWorker worker)
        {
            RtlProcedure? proc;
            while (!sr.Procedures.TryGetValue(addrProc, out proc))
            {
                var name = program.NamingPolicy.ProcedureName(addrProc);
                proc = new RtlProcedure(state.Architecture, addrProc, name, ProvenanceType.Scanning, new HashSet<RtlBlock>());
                if (sr.Procedures.TryAdd(proc.Address, proc))
                    break;
            }
            if (this.suspendedWorkers.TryGetValue(addrProc, out worker))
            {
                // It's already running, but suspended.
                return true;
            }
            while (!this.activeWorkers.TryGetValue(addrProc, out worker))
            {
                worker = new ProcedureWorker(this, proc, state, rejectMask, listener);
                if (activeWorkers.TryAdd(addrProc, worker))
                {
                    wl.Enqueue(worker);
                    return true;
                }
            }
            return true;
        }

        /// <summary>
        /// Attempt to suspend a worker.
        /// </summary>
        /// <param name="worker"></param>
        /// <returns></returns>
        public bool TrySuspendWorker(ProcedureWorker worker)
        {
            trace.Verbose("    {0}: Suspending...", worker.Procedure.Address);

            if (activeWorkers.TryRemove(worker.Procedure.Address, out var w))
            {
                trace.Verbose("    {0}: removed from active workers", worker.Procedure.Address);
                Debug.Assert(worker == w);
            }
            if (!suspendedWorkers.TryAdd(worker.Procedure.Address, worker))
            {
                //$Already suspended.
                trace.Verbose("    {0}: already on suspended workers", worker.Procedure.Address);
            }
            return true;
        }

        /// <summary>
        /// Given the address of a called procedure, returns its 
        /// <see cref="ReturnStatus"/>.
        /// </summary>
        /// <param name="addrProc">The address of the procedure.</param>
        /// <returns>The current <see cref="ReturnStatus"/> for the procedure.
        /// </returns>
        /// <remarks>This is shared mutable state, take appropriate measures
        /// when getting this value.
        /// </remarks>
        public ReturnStatus GetProcedureReturnStatus(Address addrProc)
        {
            return this.sr.ProcReturnStatus.TryGetValue(addrProc, out var status)
                ? status
                : ReturnStatus.Unknown;
        }

        /// <summary>
        /// Sets the return status of a procedure. If the return status is not
        /// known yet, the provided value is accepted. If the return status 
        /// already has a value, change it only if the old status was not 
        /// "Returns".
        /// </summary>
        /// <param name="address">The address of a procedure.</param>
        /// <param name="returns">The new return status. </param>
        public void SetProcedureReturnStatus(Address address, ReturnStatus returns)
        {
            if (this.sr.ProcReturnStatus.TryAdd(address, returns))
            {
                return;
            }
            if (this.sr.ProcReturnStatus.TryGetValue(address, out var oldRet))
            {
                if (oldRet == ReturnStatus.Returns)
                    return;
                this.sr.ProcReturnStatus[address] = returns;
            }
        }

        /// <summary>
        /// Called to resume a <see cref="ProcedureWorker"/> that was previously
        /// suspended waiting for a callee procedure to return.
        /// </summary>
        /// <param name="worker">Caller worker to resume.</param>
        /// <param name="addrCaller">Address of the call instruction.</param>
        /// <param name="addrFallthrough">Address of the instruction after
        /// the call instruction.</param>
        /// <param name="state">ProcessorState to use when resuming execution.
        /// </param>
        public void ResumeWorker(
            ProcedureWorker worker,
            Address addrCaller,
            Address addrFallthrough,
            ProcessorState state)
        {
            worker.UnsuspendCall(addrCaller);
            worker.AddCallFallthroughJob(addrCaller, addrFallthrough, state);
            if (!suspendedWorkers.TryRemove(worker.Procedure.Address, out var w))
            {
                // Tried to resume not suspended worker
                if (!activeWorkers.TryGetValue(worker.Procedure.Address, out w))
                    throw new Exception("Expected active worker!");
                return;
            }
            Debug.Assert(worker == w);
            //$TODO: move the line below to the top of the method to avoid a gap
            // when the worker is in limbo? 
            activeWorkers.TryAdd(worker.Procedure.Address, worker);
            wl.Enqueue(worker);
        }

        /// <summary>
        /// Called when a <see cref="ProcedureWorker"/> has completed processing
        /// a procedure.
        /// </summary>
        /// <param name="worker">Worker that completed.</param>
        /// <param name="waitingCalls">Number of remaining waiting calls.
        /// </param>
        public void OnWorkerCompleted(ProcedureWorker worker, int waitingCalls)
        {
            var removed = activeWorkers.TryRemove(worker.Procedure.Address, out _);
            Debug.Assert(removed, "Procedure worker should have been on the active list.");

            // If there are no more waiting calls, the worker has run
            // out of things to do. It can be discarded.
            if (waitingCalls == 0)
            {
                trace.Verbose("Procedure worker {0} is retired.", worker.Procedure.Address);
                return;
            }

            // If there are waiting calls, we must suspend this worker.
            var suspended = TrySuspendWorker(worker);
        }
    }

    /// <summary>
    /// This class represents a <see cref="ProcedureWorker"/> that is 
    /// waiting until another ProcedureWorker discovers whether a callee
    /// procedure returns or not.
    /// </summary>
    /// <param name="Worker"></param>
    /// <param name="CallAddress"></param>
    /// <param name="FallthroughAddress"></param>
    /// <param name="State"></param>
    public record WaitingCaller(
        ProcedureWorker Worker,
        Address CallAddress,
        Address FallthroughAddress,
        ProcessorState State);

    /// <summary>
    /// Classiation of the return status of a procedure.
    /// </summary>
    public enum ReturnStatus
    {
        /// <summary>
        /// Used when it is not known whether the procedure returns.
        /// </summary>
        Unknown,

        /// <summary>
        /// Used when at least one return instruction in the procedure
        /// was reached.
        /// </summary>
        Returns,

        /// <summary>
        /// Used when it was impossible to prove that the procedure returns.
        /// </summary>
        Diverges,
    }
}
