using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public class ShingleWorker : AbstractBlockWorker
    {
        public ShingleWorker(
            ShingleScanner scanner,
            ChunkWorker worker,
            Address address, 
            IEnumerator<RtlInstructionCluster> trace,
            ProcessorState state)
            : base(scanner, worker, address, trace, state)
        {
        }


        protected override void EmulateState(RtlAssignment ass)
        {
            // We don't emulate state in the shingle worker.
        }
    }
}
