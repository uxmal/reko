using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
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

        public RecursiveScanner(Reko.Core.Program program)
        {
            this.program = program;
			this.cfg = new Cfg();
			this.wl = new WorkList<Worker>();
            this.host = new RewriterHost(program);
        }

		public void ScanProgram()
		{
			var seeds = CollectSeeds();
			EnqueueWorkers(seeds.Select(MakeSeedWorker));
			ProcessWorkers();
			var gaps = FindGaps(cfg);
			var shingle = new ShingleScanner(program);
			var newCfg = shingle.Scan(gaps);
			if (newCfg.Procedures.Count > 0)
            {
				EnqueueWorkers(newCfg.Procedures.Select(MakeProcWorker));
            }
		}



        internal bool TryRegisterBlockStart(Address addr, Address address)
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<RtlInstructionCluster> MakeTrace(Address addr, ProcessorState state, IStorageBinder binder)
        {
            var arch = state.Architecture;
            var rdr = program.CreateImageReader(arch, addr);
            var rw = arch.CreateRewriter(rdr, state, binder, host);
            throw new NotImplementedException();
        }

        private object FindGaps(Cfg cfg)
        {
            throw new NotImplementedException();
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
			var proc = new Proc
			{
				Address = seed.Key,
				Provenance = ProvenanceType.ImageEntrypoint
			};
			if (cfg.Procedures.TryAdd(proc.Address, proc))
			{
				return new ProcedureWorker(this, proc, seed.Value.ProcessorState);
			}
			else
			{
				return NullWorker.Instance;
			}
		}

        private Worker MakeProcWorker(KeyValuePair<Address, Proc> de)
        {
            var proc = new Proc
            {
                Address = de.Key,
                Provenance = ProvenanceType.Scanning
            };
            if (cfg.Procedures.TryAdd(proc.Address, proc))
            {
                return new ProcedureWorker(this, proc, null);
            }
            else
            {
                return NullWorker.Instance;
            }
        }

        internal Block RegisterBlock(Address address, int size)
        {
            throw new NotImplementedException();
        }

        internal void ResumeWorkers(Address address)
        {
            throw new NotImplementedException();
        }

        internal void Splitblock(object block, Address address)
        {
            throw new NotImplementedException();
        }

        internal bool TryRegisterBlockEnd(object address1, Address address2)
        {
            throw new NotImplementedException();
        }

        private void ProcessWorkers()
        {
			while (wl.TryGetWorkItem(out var worker))
				worker.Run();
		}

        private class RewriterHost : IRewriterHost
        {
            private Program program;

            public RewriterHost(Program program)
            {
                this.program = program;
            }


            public void Error(Address address, string format, params object[] args)
            {
                throw new NotImplementedException();
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

            public void Warn(Address address, string format, params object[] args)
            {
                throw new NotImplementedException();
            }
        }
    }
}
