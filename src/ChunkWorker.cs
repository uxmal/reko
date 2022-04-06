using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    /// <summary>
    /// Performs scan of a chunk of memory that wasn't reached
    /// by the <see cref="RecursiveScanner"/>, finding blocks
    /// by doing linear scan.
    /// </summary>
    public class ChunkWorker : AbstractProcedureWorker
    {
        private ShingleScanner shScanner;
        private MemoryArea mem;
        private int[] blockNos; // 0 = not owned by anyone.

        public ChunkWorker(
            ShingleScanner scanner,
            IProcessorArchitecture arch,
            MemoryArea mem,
            Address addrStart,
            int chunkUnits)
            : base(scanner)
        {
            this.shScanner = scanner;
            this.Architecture = arch;
            this.mem = mem;
            this.Address = addrStart;
            this.Length = chunkUnits;
            this.blockNos = new int[chunkUnits];
        }

        public Address Address { get; }

        public IProcessorArchitecture Architecture { get; }

        public int Length { get; }

        public void Run()
        {
            var stepsize = Architecture.InstructionBitSize / Architecture.MemoryGranularity;
            int i = 0;
            var job = AddJob(this.Address, Architecture.CreateProcessorState());
            //for (int i = 0; i < chunkUnits; i += stepsize)
            {
                int iMark = i;
                var addrBlock = this.Address + iMark;
                var (block, state) = job.ParseBlock();
                if (block.IsValid)
                {
                    HandleBlockEnd(block, job.Trace, state);
                }
                else
                {
                    HandleBadBlock(addrBlock);
                }
            }
        }

        public override AbstractBlockWorker AddJob(Address addr, IEnumerator<RtlInstructionCluster> trace, ProcessorState state)
        {
            return new ShingleWorker(shScanner, this, addr, trace, state);
        }

        protected override void ProcessCall(Block block, Edge edge, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        protected override void ProcessReturn()
        {
        }
    }
}
