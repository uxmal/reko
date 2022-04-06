using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public class ChunkWorker
    {
        private ShingleScanner scanner;
        private IStorageBinder binder;
        private MemoryArea mem;
        private int[] blockNos; // 0 = not owned by anyone.

        public ChunkWorker(
            ShingleScanner scanner,
            IProcessorArchitecture arch,
            MemoryArea mem,
            Address addrStart,
            int chunkUnits)
        {
            this.scanner = scanner;
            this.Architecture = arch;
            this.mem = mem;
            this.Address = addrStart;
            this.Length = chunkUnits;
            this.binder = new StorageBinder();
            this.blockNos = new int[chunkUnits];
        }

        public Address Address { get; }

        public IProcessorArchitecture Architecture { get; }

        public int Length { get; }


        public void Run()
        {
            var stepsize = Architecture.InstructionBitSize / Architecture.MemoryGranularity;
            int i = 0;
            //for (int i = 0; i < chunkUnits; i += stepsize)
            {
                int iMark = i;
                var addrBlock = this.Address + iMark;
                var state = Architecture.CreateProcessorState();
                var trace = scanner.MakeTrace(addrBlock, state, binder).GetEnumerator();
                var instrs = new List<RtlInstructionCluster>();
                while (trace.MoveNext())
                {
                    var cluster = trace.Current; 
                    foreach (var rtl in cluster.Instructions)
                    {
                        switch (rtl)
                        {
                        case RtlAssignment:
                            instrs.Add(cluster);
                            continue;
                        case RtlReturn:
                            instrs.Add(cluster);
                            break;
                        }
                        var addrFallthrough = cluster.Address + cluster.Length;
                        var block = scanner.RegisterBlock(
                            Architecture,
                            addrBlock,
                            (cluster.Address - addrBlock) + cluster.Length,
                            addrFallthrough,
                            instrs);
                        break;
                    }
                }
            }
        }
    }
}
