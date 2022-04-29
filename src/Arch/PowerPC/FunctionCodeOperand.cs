using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PowerPC
{
    public class FunctionCodeOperand : AbstractMachineOperand
    {
        public FunctionCodeOperand(FunctionCode code)
            : base(PrimitiveType.Byte)
        {
            this.FunctionCode = code;
        }

        public FunctionCode FunctionCode { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString(FunctionCode.ToString());
        }
    }

    public enum FunctionCode
    {
        Invalid = -1,
        add,
        xor,
        or,
        and,

        umax,
        smax,
        umin,
        smin,

        swap,
        casne,
        twin,
        incb,
        ince,
        decb,
    }
}
