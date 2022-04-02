using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public class BlockWorker
    {
        private ProcedureWorker pw;
        public Address Address;
        public IEnumerator<RtlInstructionCluster> Trace;
        public ProcessorState State;

        public BlockWorker(
            ProcedureWorker pw,
        Address Address,
        IEnumerator<RtlInstructionCluster> Trace,
        ProcessorState State)
        {
            this.pw = pw;
            this.Address = Address;
            this.Trace = Trace;
            this.State = State;
        }
    }
}
