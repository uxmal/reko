using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Reko.Arch.Microchip.PIC18
{
    public class PIC18Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private PIC18Architecture arch;
        private IStorageBinder binder;
        private IRewriterHost host;
        private IEnumerator<PIC18Instruction> dasm;
        private ProcessorState state;
        private PIC18Instruction instr;
        private RtlClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private List<RtlInstructionCluster> clusters;
        private RtlEmitter m;

        public PIC18Rewriter(PIC18Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.dasm = new PIC18Disassembler(arch, rdr).GetEnumerator();
            this.state = state;
            this.binder = binder;
            this.host = host;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}
