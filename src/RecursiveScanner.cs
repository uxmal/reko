using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
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
        private Program program;
        private Cfg cfg;
        private WorkList<Worker> wl;
        private IRewriterHost host;
        private ConcurrentDictionary<Address, Address> blockStarts;
        private ConcurrentDictionary<Address, Address> blockEnds;
        private ConcurrentDictionary<Address, Worker> activeWorkers;
        private ConcurrentDictionary<Address, Worker> suspendedWorkers; 
        private ConcurrentDictionary<Address, ReturnStatus> procReturnStatus;

        public RecursiveScanner(Reko.Core.Program program)
        {
            this.program = program;
            this.cfg = new Cfg();
            this.wl = new WorkList<Worker>();
            this.host = new RewriterHost(program);
            this.blockStarts = new ConcurrentDictionary<Address, Address>();
            this.blockEnds = new ConcurrentDictionary<Address, Address>();
            this.activeWorkers = new();
            this.suspendedWorkers = new();
            this.procReturnStatus = new();
        }

        public Cfg ScanProgram()
        {
            var seeds = CollectSeeds();
            EnqueueWorkers(seeds.Select(MakeSeedWorker));
            ProcessWorkers();
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

        private void EnqueueWorkers(IEnumerable<Worker> workers)
        {
            wl.AddRange(workers);
        }

        private Dictionary<Address, ImageSymbol> CollectSeeds()
        {
            var result = new Dictionary<Address, ImageSymbol>();
            foreach (var sym in program.EntryPoints)
            {
                result.Add(sym.Key, sym.Value);
            }
            return result;
        }

        private Worker MakeSeedWorker(KeyValuePair<Address, ImageSymbol> seed)
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
                return NullWorker.Instance;
            }
        }

        private Worker MakeProcWorker(KeyValuePair<Address, Proc> de)
        {
            var name = program.NamingPolicy.ProcedureName(de.Key);
            var proc = new Proc(de.Key, ProvenanceType.Scanning, de.Value.Architecture, name);
            if (cfg.Procedures.TryAdd(proc.Address, proc))
            {
                return new ProcedureWorker(this, proc, proc.Architecture.CreateProcessorState());
            }
            else
            {
                return NullWorker.Instance;
            }
        }

        private void ProcessWorkers()
        {
            while (wl.TryGetWorkItem(out var worker))
                worker.Run();
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
            List<(Address, RtlInstruction)> instrs)
        {
            var id = program.NamingPolicy.BlockName(addrBlock);
            var block = new Block(arch, id, addrBlock, (int)length, addrFallthrough, instrs);
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
            Worker? w;
            while (!this.activeWorkers.TryGetValue(addrProc, out w))
            {
                worker = new ProcedureWorker(this, proc, state);
                if (activeWorkers.TryAdd(addrProc, worker))
                {
                    wl.Add(worker);
                    return true;
                }
            }
            worker = w as ProcedureWorker;
            return worker is not null;
        }

        public bool TrySuspendWorker(ProcedureWorker worker)
        {
            if (activeWorkers.TryRemove(worker.ProcedureAddress, out var w))
            {
                Debug.Assert(worker == w);
            }
            suspendedWorkers.TryAdd(worker.ProcedureAddress, worker);
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

        public void ResumeWorker(
            ProcedureWorker worker,
            Address addrCaller,
            Address addrFallthrough, 
            ProcessorState state)
        {
            RegisterEdge(new Edge(addrCaller, addrFallthrough, EdgeType.Fallthrough));
            worker.AddJob(addrFallthrough, state);
            suspendedWorkers.TryRemove(worker.ProcedureAddress, out var w);
            Debug.Assert(worker == w);
            activeWorkers.TryAdd(worker.ProcedureAddress, worker);
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
                    throw new NotImplementedException();
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
                    var newBEnd = newB.Instructions[^1].Item1;
                    if (!TryRegisterBlockEnd(newB.Address, newBEnd))
                    {
                        // There is already a block ending at newBEnd.
                        wl.Enqueue((newB, newBEnd));
                    }
                }
                else if (b == 0)
                {
                    // Block A falls through into B
                    //
                    // addrA
                    // +----------------+
                    //       +----------+
                    //       addrB
                    // We split block A so that it falls through to B. We also
                    // move the out edges of block A to block B.
                    var newA = Chop(blockA, 0, a, addrB - addrA);
                    cfg.Blocks.TryUpdate(addrA, newA, blockA);
                    StealEdges(blockA, blockB);
                    RegisterEdge(new Edge(newA.Address, blockB.Address, EdgeType.Jump));
                    var newAEnd = newA.Instructions[^1].Item1;
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
                if (blockA.Instructions[a].Item1 != blockB.Instructions[b].Item1)
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
            var instrs = new List<(Address, RtlInstruction)>(iEnd - iStart);
            for (int i = iStart; i < iEnd; ++i)
            {
                instrs.Add(block.Instructions[i]);
            }
            var addr = instrs[0].Item1;
            var instrLast = instrs[^1];
            var id = program.NamingPolicy.BlockName(addr);
            var addrFallthrough = addr + blockSize;
            return new Block(block.Architecture, id, addr, (int)blockSize, addrFallthrough, instrs);
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
            private Program program;

            public RewriterHost(Program program)
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
                throw new NotImplementedException();
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
