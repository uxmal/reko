#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Services;
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
    /// in a binary. The algorithm creates a <see cref="ProcedureWorkder"/>
    /// for each known "seed". Seeds are obtained from the binary itself (such
    /// as entry points and exported functions) and from the user (such as
    /// user-specified functions). 
    /// </summary>
    public class RecursiveScanner : AbstractScanner
    {
        private readonly WorkList<ProcedureWorker> wl;
        private readonly ConcurrentDictionary<Address, ProcedureWorker> activeWorkers;
        private readonly ConcurrentDictionary<Address, ProcedureWorker> suspendedWorkers; 
        private readonly ConcurrentDictionary<Address, ReturnStatus> procReturnStatus;

        public RecursiveScanner(
            Program program,
            IDynamicLinker dynamicLinker,
            DecompilerEventListener listener,
            IServiceProvider services)
            : this(program, new ScanResultsV2(), dynamicLinker, listener, services)
        { }

        public RecursiveScanner(
            Program program, 
            ScanResultsV2 sr, 
            IDynamicLinker dynamicLinker,
            DecompilerEventListener listener,
            IServiceProvider services)
            : base(program, sr, dynamicLinker, listener, services)
        {
            this.wl = new WorkList<ProcedureWorker>();
            this.activeWorkers = new();
            this.suspendedWorkers = new();
            this.procReturnStatus = new();
        }

        public ScanResultsV2 ScanProgram()
        {
            var seeds = CollectSeeds();
            EnqueueWorkers(seeds.Select(MakeSeedWorker));
            ProcessWorkers();
            return sr;
        }

        public ScanResultsV2 ScanProcedure(IProcessorArchitecture arch, Address addr, string? procedureName, ProcessorState state)
        {
            var symbol = ImageSymbol.Create(SymbolType.Code, arch, addr, procedureName);
            symbol.ProcessorState = state;
            var worker = MakeSeedWorker(new(addr, symbol));
            EnqueueWorkers(new[] { worker });
            ProcessWorkers();
            return sr;
        }

        private void EnqueueWorkers(IEnumerable<ProcedureWorker?> workers)
        {
            foreach (var worker in workers)
            {
                if (worker is null)
                    continue;
                if (!this.activeWorkers.TryAdd(worker.Procedure.Address, worker))
                {
                    Debug.Print("RecScan: worker for {0} already enqueued.", worker.Procedure.Address);
                }
                else
                {
                    wl.Add(worker);
                }
            }
        }

        private Dictionary<Address, ImageSymbol> CollectSeeds()
        {
            var result = new Dictionary<Address, ImageSymbol>();
            foreach (var sym in program.EntryPoints)
            {
                result.Add(sym.Key, sym.Value);
            }
            foreach (var sym in program.ImageSymbols)
            {
                if ((sym.Value.Type == SymbolType.Code ||
                    sym.Value.Type == SymbolType.Procedure)
                    && IsExecutableAddress(sym.Value.Address))
                {
                    result.TryAdd(sym.Key, sym.Value);
                }
            }
            //$TODO: user-provided entry points.
            return result;
        }

        private ProcedureWorker? MakeSeedWorker(KeyValuePair<Address, ImageSymbol> seed)
        {
            var name = seed.Value.Name ?? program.NamingPolicy.ProcedureName(seed.Key);
            var proc = new Proc(seed.Key, ProvenanceType.Image, seed.Value.Architecture, name);
            if (sr.Procedures.TryAdd(proc.Address, proc))
            {
                var state = seed.Value.ProcessorState ?? seed.Value.Architecture.CreateProcessorState();
                return new ProcedureWorker(this, proc, state, listener);
            }
            else
            {
                // This procedure appears to already have been scanned.
                return null;
            }
        }

        private ProcedureWorker? MakeProcedureWorker(KeyValuePair<Address, Proc> de)
        {
            var name = program.NamingPolicy.ProcedureName(de.Key);
            var proc = new Proc(de.Key, ProvenanceType.Scanning, de.Value.Architecture, name);
            if (sr.Procedures.TryAdd(proc.Address, proc))
            {
                var state = proc.Architecture.CreateProcessorState();
                return new ProcedureWorker(this, proc, state, listener);
            }
            else
            {
                return null;
            }
        }

        private void ProcessWorkers()
        {
            while (wl.TryGetWorkItem(out var worker))
            {
                worker.Run();
            }
        }

        public bool TryStartProcedureWorker(
            Address addrProc,
            ProcessorState state,
            [MaybeNullWhen(false)] out ProcedureWorker worker)
        {
            Proc? proc;
            while (!sr.Procedures.TryGetValue(addrProc, out proc))
            {
                var name = program.NamingPolicy.ProcedureName(addrProc);
                proc = new Proc(addrProc, ProvenanceType.Scanning, state.Architecture, name);
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
                worker = new ProcedureWorker(this, proc, state, listener);
                if (activeWorkers.TryAdd(addrProc, worker))
                {
                    wl.Add(worker);
                    return true;
                }
            }
            return true;
        }

        public bool TrySuspendWorker(ProcedureWorker worker)
        {
            if (activeWorkers.TryRemove(worker.Procedure.Address, out var w))
            {
                Debug.Assert(worker == w);
            }
            if (!suspendedWorkers.TryAdd(worker.Procedure.Address, worker))
            {
                //$Already suspended.
            }
            return true;
        }

        public ReturnStatus GetProcedureReturnStatus(Address addrProc)
        {
            return this.procReturnStatus.TryGetValue(addrProc, out var status)
                ? status
                : ReturnStatus.Unknown;
        }

        public void SetProcedureReturnStatus(Address address, ReturnStatus returns)
        {
            if (this.procReturnStatus.TryAdd(address, returns))
            {
                return;
            }
            if (this.procReturnStatus.TryGetValue(address, out var oldRet))
            {
                if (oldRet == ReturnStatus.Returns)
                    return;
                this.procReturnStatus[address] = returns;
            }
        }

        public void ResumeWorker(
            ProcedureWorker worker,
            Address addrCaller,
            Address addrFallthrough, 
            ProcessorState state)
        {
            RegisterEdge(new Edge(addrCaller, addrFallthrough, EdgeType.Fallthrough));
            worker.AddJob(addrFallthrough, state);
            if (!suspendedWorkers.TryRemove(worker.Procedure.Address, out var w))
            {
                // Tried to resume not suspended worker
                if (!activeWorkers.TryGetValue(worker.Procedure.Address, out w))
                    throw new Exception("Expected active worker!");
                return;
            }
            else
            {
                Debug.Assert(worker == w);
            }
            activeWorkers.TryAdd(worker.Procedure.Address, worker);
            wl.Add(worker);
        }
    }

    /// <summary>
    /// This class represents a <see cref="ProcedureWorker"/> that is 
    /// waiting until another ProcedureWorker discoveres whether a callee
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

    public enum ReturnStatus
    {
        Unknown,
        Returns,
        Diverges,
    }
}
