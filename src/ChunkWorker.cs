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
        private IProcessorArchitecture arch;
        private IStorageBinder binder;
        private MemoryArea mem;
        private Address addrStart;
        private int chunkUnits;
        private int[] blockNos; // 0 = not owned by anyone.

        public ChunkWorker(
            ShingleScanner scanner,
            IProcessorArchitecture arch,
            MemoryArea mem,
            Address addrStart,
            int chunkUnits)
        {
            this.scanner = scanner;
            this.arch = arch;
            this.mem = mem;
            this.addrStart = addrStart;
            this.chunkUnits = chunkUnits;
            this.binder = new StorageBinder();
            this.blockNos = new int[chunkUnits];
        }

        public void Run()
        {
            var stepsize = arch.InstructionBitSize / arch.MemoryGranularity;
            int i = 0;
            //for (int i = 0; i < chunkUnits; i += stepsize)
            {
                int iMark = i;
                var addrBlock = this.addrStart + iMark;
                var state = arch.CreateProcessorState();
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
                            arch,
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
