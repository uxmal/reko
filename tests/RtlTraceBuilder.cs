using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.ScannerV2.UnitTests
{
    public class RtlTraceBuilder : IEnumerable<RtlTrace>
    {
        public RtlTraceBuilder()
        {
            this.Traces = new SortedList<Address, RtlTrace>();
        }

        public SortedList<Address, RtlTrace> Traces { get; private set; }

        public void Add(RtlTrace trace)
        {
            Traces.Add(trace.StartAddress, trace);
        }

        public IEnumerator<RtlTrace> GetEnumerator()
        {
            return Traces.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class RtlTrace : IEnumerable<RtlInstructionCluster>
    {
        private Address addr;
        private List<RtlInstructionCluster> clusters;

        public RtlTrace(uint addr)
        {
            this.addr = Address.Ptr32(addr);
            this.StartAddress = this.addr;
            this.clusters = new List<RtlInstructionCluster>();
        }

        public Address StartAddress { get; private set; }

        public void Add(Action<RtlEmitter> generator)
        {
            Add(4, generator);
        }

        public void Add(int length, Action<RtlEmitter> generator)
        {
            var rtls = new List<RtlInstruction>();
            generator(new RtlEmitter(rtls));
            var artls = rtls.ToArray();
            clusters.Add(new RtlInstructionCluster(addr, 4, artls)
            {
                Class = artls[^1].Class,
            });
            addr += length;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            return clusters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
