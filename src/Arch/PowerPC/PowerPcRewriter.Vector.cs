using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public partial class PowerPcRewriter
    {
        public void RewriteVspltw()
        {
            var opD = RewriteOperand(instr.op1);
            var opS = RewriteOperand(instr.op2);
            var opI = RewriteOperand(instr.op3);

            emitter.Assign(opD, PseudoProc("__vspltw", opD.DataType, opS, opI));
        }
    }
}
