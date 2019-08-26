using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.WindowsItp.Decoders
{
    public class TestInstruction : MachineInstruction
    {
        public TestInstruction()
        { }

        public Opcode Opcode { get; set; }

#if !ARRAY_OPERANDS
        public MachineOperand[] Operands;
#else
        public MachineOperand Op1;
        public MachineOperand Op2;
        public MachineOperand Op3;
#endif
        public override int OpcodeAsInteger => (int) Opcode;

        public override MachineOperand GetOperand(int i)
        {
            return null;
        }
    }
}
