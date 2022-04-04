using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public class RecursiveScanner
    {
        private Reko.Core.Program program;
        private Cfg cfg;
        private readonly WorkList<ProcedureWorker> wl;
        private readonly IRewriterHost host;
        private readonly ConcurrentDictionary<Address, Address> blockStarts;
        private readonly ConcurrentDictionary<Address, Address> blockEnds;
        private readonly ConcurrentDictionary<Address, ProcedureWorker> activeWorkers;
        private readonly ConcurrentDictionary<Address, ProcedureWorker> suspendedWorkers; 
        private readonly ConcurrentDictionary<Address, ReturnStatus> procReturnStatus;
        private readonly DecompilerEventListener listener;

        public RecursiveScanner(Reko.Core.Program program, DecompilerEventListener listener)
        {
            this.program = program;
            this.cfg = new Cfg();
            this.wl = new WorkList<ProcedureWorker>();
            this.host = new RewriterHost(program);
            this.blockStarts = new ConcurrentDictionary<Address, Address>();
            this.blockEnds = new ConcurrentDictionary<Address, Address>();
            this.activeWorkers = new();
            this.suspendedWorkers = new();
            this.procReturnStatus = new();
            this.listener = listener;
        }

        public ExpressionSimplifier CreateEvaluator(ProcessorState state)
        {
            return new ExpressionSimplifier(
                program.SegmentMap,
                state,
                listener);
        }

        public Cfg ScanProgram()
        {
            var seeds = CollectSeeds();
            EnqueueWorkers(seeds.Select(MakeSeedWorker));
            ProcessWorkers();
            RegisterPredecessors();
            //var gaps = FindGaps(cfg);
            //var shingle = new ShingleScanner(program);
            //var newCfg = shingle.Scan(gaps);
            //if (newCfg.Procedures.Count > 0)
            //{
            //    EnqueueWorkers(newCfg.Procedures.Select(MakeProcWorker));
            //}
            return cfg;
        }

        public IEnumerable<RtlInstructionCluster> MakeTrace(Address addr, ProcessorState state, IStorageBinder binder)
        {
            var arch = state.Architecture;
            var rdr = program.CreateImageReader(arch, addr);
            var rw = arch.CreateRewriter(rdr, state, binder, host);
            return rw;
        }

        private object? FindGaps(Cfg cfg)
        {
            return null;
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
            return result;
        }

        /// <summary>
        /// Returns true if the given address is inside an executable
        /// <see cref="ImageSegment"/>.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns>Whether or not the address is executable.</returns>
        public bool IsExecutableAddress(Address addr) =>
            program.SegmentMap.IsExecutableAddress(addr);

        private ProcedureWorker? MakeSeedWorker(KeyValuePair<Address, ImageSymbol> seed)
        {
            var name = program.NamingPolicy.ProcedureName(seed.Key);
            var proc = new Proc(seed.Key, ProvenanceType.ImageEntrypoint, seed.Value.Architecture, name);
            if (cfg.Procedures.TryAdd(proc.Address, proc))
            {
                var state = seed.Value.ProcessorState ?? seed.Value.Architecture.CreateProcessorState();
                return new ProcedureWorker(this, proc, state);
            }
            else
            {
                return null;
            }
        }

        private ProcedureWorker? MakeProcWorker(KeyValuePair<Address, Proc> de)
        {
            var name = program.NamingPolicy.ProcedureName(de.Key);
            var proc = new Proc(de.Key, ProvenanceType.Scanning, de.Value.Architecture, name);
            if (cfg.Procedures.TryAdd(proc.Address, proc))
            {
                return new ProcedureWorker(this, proc, proc.Architecture.CreateProcessorState());
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

        public bool TryRegisterBlockStart(Address addrBlock, Address addrProc)
        {
            return this.blockStarts.TryAdd(addrBlock, addrProc);
        }

        public Block RegisterBlock(
            IProcessorArchitecture arch,
            Address addrBlock,
            long length,
            Address addrFallthrough,
            List<RtlInstructionCluster> instrs)
        {
            var id = program.NamingPolicy.BlockName(addrBlock);
            var block = new Block(arch, addrBlock, id, (int)length, addrFallthrough, instrs);
            var success = cfg.Blocks.TryAdd(addrBlock, block);
            Debug.Assert(success);
            return block;
        }

        public bool TryRegisterBlockEnd(Address addrBlockStart, Address addrBlockLast)
        {
            return this.blockEnds.TryAdd(addrBlockLast, addrBlockStart);
        }

        public bool TryStartProcedureWorker(
            Address addrProc,
            ProcessorState state,
            [MaybeNullWhen(false)] out ProcedureWorker worker)
        {
            Proc? proc;
            while (!cfg.Procedures.TryGetValue(addrProc, out proc))
            {
                var name = program.NamingPolicy.ProcedureName(addrProc);
                proc = new Proc(addrProc, ProvenanceType.Scanning, state.Architecture, name);
                if (cfg.Procedures.TryAdd(proc.Address, proc))
                    break;
            }
            if (this.suspendedWorkers.TryGetValue(addrProc, out worker))
            {
                // It's already running, but suspended.
                return true;
            }
            while (!this.activeWorkers.TryGetValue(addrProc, out worker))
            {
                worker = new ProcedureWorker(this, proc, state);
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

        public void RegisterEdge(Edge edge)
        {
            if (!cfg.Successors.TryGetValue(edge.From, out var edges))
            {
                edges = new List<Edge>();
                cfg.Successors.TryAdd(edge.From, edges);
            }
            edges.Add(edge);
        }

        private void RegisterPredecessors()
        {
            foreach (var (from, succs) in cfg.Successors)
            {
                foreach (var edge in succs)
                {
                    if (!cfg.Predecessors.TryGetValue(edge.To, out var edges))
                    {
                        edges = new List<Edge>();
                        cfg.Predecessors.TryAdd(edge.To, edges);
                    }
                    edges.Add(edge);
                }
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

        public void Splitblock(Block block, Address addrEnd)
        {
            var wl = new Queue<(Block block, Address addrEnd)>();
            wl.Enqueue((block, addrEnd));
            while (wl.TryDequeue(out var item))
            {
                Block blockB;
                (blockB, addrEnd) = item;
                // Invariant: we arrive here if blockB ends at the same
                // address as some other block A. Get the address of that
                // block.

                var addrA = this.blockEnds[addrEnd];
                var addrB = block.Address;

                // If both addresses are the same, we have a self-loop. No need for splitting.
                if (addrA == addrB)
                    continue;

                var blockA = cfg.Blocks[addrA];

                // Find the indices where both blocks have the same instructions: "shared tails"
                // Because both blocks end at addrEnd, we're guaranteed at least one shared instruction.
                var (a, b) = FindSharedInstructions(blockA, blockB);

                if (a > 0 && b > 0)
                {
                    // The shared instructions S do not fully take up either of the
                    // two blocks. One of the blocks starts in the middle of an
                    // instruction in the other block:
                    //
                    //  addrA     S
                    //  +-+--+--+-+-----+
                    //      +--+--+-----+
                    //      addrB
                    //
                    // We want to create a new block S with the shared instructions:
                    //  addrA       S
                    //  +-+--+--+-+ +-----+
                    //      +--+--+
                    var addrS = blockA.Instructions[a].Address;
                    if (TryRegisterBlockStart(addrS, blockA.Address))
                    {
                        // S didn't exist already.
                        var sSize = blockA.Length - (addrS - blockA.Address);
                        var instrs = blockA.Instructions.Skip(a).ToList();
                        var blockS = RegisterBlock(
                            blockA.Architecture,
                            addrS,
                            sSize,
                            blockA.FallThrough,
                            instrs);
                        StealEdges(blockA, blockS);
                    }
                    // Trim off the last instructions of A and B, then
                    // replace their original values
                    //$REVIEW: the below code is not thread safe.
                    RegisterEdge(new Edge(addrA, addrS, EdgeType.Jump));
                    RegisterEdge(new Edge(addrB, addrS, EdgeType.Jump));
                    var newA = Chop(blockA, 0, a, addrS - addrA);
                    var newB = Chop(blockB, 0, b, addrS - addrB);
                    cfg.Blocks.TryUpdate(addrA, newA, blockA);
                    cfg.Blocks.TryUpdate(addrB, newB, blockB);
                    blockEnds.TryUpdate(newA.Address, addrS, addrEnd);
                    blockEnds.TryUpdate(newB.Address, addrS, addrEnd);
                }
                else if (a == 0)
                {
                    // Block B falls through into block A
                    // 
                    //       addrA
                    //       +-----------+
                    //  +----------------+
                    //  addrB
                    //
                    // We split block B so that it falls through to block A
                    var newB = Chop(blockB, 0, b, addrA - addrB);
                    cfg.Blocks.TryUpdate(addrB, newB, blockB);
                    RegisterEdge(new Edge(blockB.Address, addrA, EdgeType.Jump));
                    var newBEnd = newB.Instructions[^1].Address;
                    if (!TryRegisterBlockEnd(newB.Address, newBEnd))
                    {
                        // There is already a block ending at newBEnd.
                        wl.Enqueue((newB, newBEnd));
                    }
                }
                else
                {
                    Debug.Assert(b == 0);
                    // Block A falls through into B
                    //
                    // addrA
                    // +----------------+
                    //       +----------+
                    //       addrB
                    // We split block A so that it falls through to B. We make
                    // B be the new block end, and move the out edges of block
                    // A to block B.
                    var newA = Chop(blockA, 0, a, addrB - addrA);
                    cfg.Blocks.TryUpdate(addrA, newA, blockA);
                    //$TODO: check for race conditions.
                    if (!blockEnds.TryUpdate(addrEnd, addrB, addrA))
                    {
                        throw new Exception("Who stole it?");
                    }
                    StealEdges(blockA, blockB);
                    RegisterEdge(new Edge(newA.Address, blockB.Address, EdgeType.Jump));
                    var newAEnd = newA.Instructions[^1].Address;
                    if (!TryRegisterBlockEnd(newA.Address, newAEnd))
                    {
                        // There is already a block ending at newAEnd
                        wl.Enqueue((newA, newAEnd));
                    }
                }
            }
        }

        /// <summary>
        /// Starting at the end of two blocks, walk towards their respective beginnings
        /// while the addresses match. 
        /// </summary>
        /// <param name="blockA">First block</param>
        /// <param name="blockB">Second block</param>
        /// <returns>A pair of indices into the respective instruction lists where the 
        /// instruction addresses start matching.
        /// </returns>
        private (int, int) FindSharedInstructions(Block blockA, Block blockB)
        {
            int a = blockA.Instructions.Count - 1;
            int b = blockB.Instructions.Count - 1;
            for (; a >= 0 && b >= 0; --a, --b)
            {
                if (blockA.Instructions[a].Address != blockB.Instructions[b].Address)
                    break;
            }
            return (a + 1, b + 1);
        }

        /// <summary>
        /// Creates a new block from an existing block, using the instruction range
        /// [iStart, iEnd).
        /// </summary>
        /// <param name="block">The block to chop.</param>
        /// <param name="iStart">Index of first instruction in the new block.</param>
        /// <param name="iEnd">Index of the first instruction to not include.</param>
        /// <param name="blockSize">The size in address units of the resulting block.</param>
        /// <returns>A new, terminated but unregistered block. The caller is responsible for 
        /// registering it.
        /// </returns>
        private Block Chop(Block block, int iStart, int iEnd, long blockSize)
        {
            var instrs = new List<RtlInstructionCluster>(iEnd - iStart);
            for (int i = iStart; i < iEnd; ++i)
            {
                instrs.Add(block.Instructions[i]);
            }
            var addr = instrs[0].Address;
            var instrLast = instrs[^1];
            var id = program.NamingPolicy.BlockName(addr);
            var addrFallthrough = addr + blockSize;
            return new Block(block.Architecture, addr, id, (int)blockSize, addrFallthrough, instrs);
        }

        private void StealEdges(Block from, Block to)
        {
            if (cfg.Successors.TryGetValue(from.Address, out var edges))
            {
                cfg.Successors.TryRemove(from.Address, out _);
                var newEges = edges.Select(e => new Edge(to.Address, e.To, e.Type)).ToList();
                cfg.Successors.TryAdd(to.Address, newEges);
            }
        }

        private class RewriterHost : IRewriterHost
        {
            private Reko.Core.Program program;

            public RewriterHost(Reko.Core.Program program)
            {
                this.program = program;
            }

            public IProcessorArchitecture GetArchitecture(string archMoniker)
            {
                throw new NotImplementedException();
            }

            public Expression GetImport(Address addrThunk, Address addrInstr)
            {
                throw new NotImplementedException();
            }

            public ExternalProcedure GetImportedProcedure(IProcessorArchitecture arch, Address addrThunk, Address addrInstr)
            {
                throw new NotImplementedException();
            }

            public ExternalProcedure GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
            {
                throw new NotImplementedException();
            }

            public IntrinsicProcedure EnsureIntrinsic(string name, bool hasSideEffect, DataType returnType, int arity)
            {
                var args = Enumerable.Range(0, arity).Select(i => Constant.Create(program.Architecture.WordWidth, 0)).ToArray();
                var intrinsic = program.EnsureIntrinsicProcedure(name, hasSideEffect, returnType, args);
                return intrinsic;
            }

            public Expression CallIntrinsic(string name, bool hasSideEffect, FunctionType fnType, params Expression[] args)
            {
                var intrinsic = program.EnsureIntrinsicProcedure(name, hasSideEffect, fnType);
                return new Application(
                    new ProcedureConstant(program.Architecture.PointerType, intrinsic),
                    fnType.ReturnValue.DataType,
                    args);
            }

            public Expression Intrinsic(string name, bool hasSideEffect, DataType returnType, params Expression[] args)
            {
                var intrinsic = program.EnsureIntrinsicProcedure(name, hasSideEffect, returnType, args);
                return new Application(
                    new ProcedureConstant(program.Architecture.PointerType, intrinsic),
                    returnType,
                    args);
            }


            public Expression Intrinsic(string name, bool hasSideEffect, ProcedureCharacteristics c, DataType returnType, params Expression[] args)
            {
                var intrinsic = program.EnsureIntrinsicProcedure(name, hasSideEffect, returnType, args);
                intrinsic.Characteristics = c;
                return new Application(
                    new ProcedureConstant(program.Architecture.PointerType, intrinsic),
                    returnType,
                    args);
            }

            public bool TryRead(IProcessorArchitecture arch, Address addr, PrimitiveType dt, out Constant value)
            {
                throw new NotImplementedException();
            }

            public void Error(Address address, string format, params object[] args)
            {
                var msg = string.Format(format, args);
                Debug.WriteLine($"E: {address}: {msg}");
            }

            public void Warn(Address address, string format, params object[] args)
            {
                throw new NotImplementedException();
            }
        }
    }

    public enum ReturnStatus
    {
        Unknown,
        Returns,
        Diverges,
    }
}
