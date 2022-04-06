using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public abstract class AbstractScanner
    {
        protected readonly Core.Program program;
        protected readonly Cfg cfg;
        protected readonly DecompilerEventListener listener;
        private readonly IRewriterHost host;

        protected AbstractScanner(Core.Program program, Cfg cfg, DecompilerEventListener listener)
        {
            this.program = program;
            this.cfg = cfg;
            this.listener = listener;
            this.host = new RewriterHost(program);
        }

        /// <summary>
        /// Returns true if the given address is inside an executable
        /// <see cref="ImageSegment"/>.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns>Whether or not the address is executable.</returns>
        public bool IsExecutableAddress(Address addr) =>
            program.SegmentMap.IsExecutableAddress(addr);

        public IEnumerable<RtlInstructionCluster> MakeTrace(Address addr, ProcessorState state, IStorageBinder binder)
        {
            var arch = state.Architecture;
            var rdr = program.CreateImageReader(arch, addr);
            var rw = arch.CreateRewriter(rdr, state, binder, host);
            return rw;
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

        public void RegisterEdge(Edge edge)
        {
            if (!cfg.Successors.TryGetValue(edge.From, out var edges))
            {
                edges = new List<Edge>();
                cfg.Successors.TryAdd(edge.From, edges);
            }
            edges.Add(edge);
        }

        public abstract void Splitblock(Block block, Address lastAddr);

        public abstract bool TryRegisterBlockEnd(Address addrStart, Address addrLast);



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
}
