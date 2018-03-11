using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Rtl;

namespace Reko.Arch.Microchip.PIC16
{
    public class PIC16Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private PIC16Architecture pIC16Architecture;
        private EndianImageReader rdr;
        private PIC16State state;
        private IStorageBinder frame;
        private IRewriterHost host;

        public PIC16Rewriter(PIC16Architecture pIC16Architecture, EndianImageReader rdr, PIC16State state, IStorageBinder frame, IRewriterHost host)
        {
            this.pIC16Architecture = pIC16Architecture;
            this.rdr = rdr;
            this.state = state;
            this.frame = frame;
            this.host = host;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }

}
