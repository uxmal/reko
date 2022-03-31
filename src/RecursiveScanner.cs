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
        private ConcurrentDictionary<Address, Worker> workers;
        private ConcurrentDictionary<Address, Worker> suspendedWorkers;

        public RecursiveScanner(Reko.Core.Program program)
        {
            this.program = program;
            this.cfg = new Cfg();
            this.wl = new WorkList<Worker>();
            this.host = new RewriterHost(program);
            this.blockStarts = new ConcurrentDictionary<Address, Address>();
            this.blockEnds = new ConcurrentDictionary<Address, Address>();
            this.workers = new();
            this.suspendedWorkers = new();
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

        public Block RegisterBlock(IProcessorArchitecture arch, Address addrBlock, List<(Address, Instruction)> instrs)
        {
            var id = program.NamingPolicy.BlockName(addrBlock);
            var block = new Block(arch, id, addrBlock, instrs);
            var success = cfg.Blocks.TryAdd(addrBlock, block);
            Debug.Assert(success);
            return block;
        }

        public bool TryRegisterBlockEnd(Address addrBlockStart, Address addrBlockLast)
        {
            return this.blockEnds.TryAdd(addrBlockLast, addrBlockStart);
        }

        public bool TrySuspendWorker(ProcedureWorker worker)
        {
            if (workers.TryRemove(worker.ProcedureAddress, out var w))
            {
                Debug.Assert(worker == w);
                suspendedWorkers.TryAdd(worker.ProcedureAddress, worker);
            }
            return true;
        }

        public void ResumeWorker(ProcedureWorker worker, Address addrCaller, Address addrFallthrough)
        {
            suspendedWorkers.TryRemove(worker.ProcedureAddress, out var w);
            Debug.Assert(worker == w);
            workers.TryAdd(worker.ProcedureAddress, worker);
            wl.Add(worker);
        }

        public void Splitblock(object block, Address address)
        {
            throw new NotImplementedException();
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
}
