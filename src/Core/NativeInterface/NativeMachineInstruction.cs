using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
    public class NativeMachineInstruction : MachineInstruction
    {
        public override int OpcodeAsInteger
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override MachineOperand GetOperand(int i)
        {
            // GetOperand is only ever used by X86 code and that may be going away soon.
            throw new NotSupportedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            base.Render(writer, options);
        }
    }
}
