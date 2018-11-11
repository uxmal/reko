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

        public MachineOperand[] Operands { get; internal set; }

        public override InstructionClass InstructionClass { get;  }

        public override bool IsValid { get; }

        public override int OpcodeAsInteger => (int) Opcode;

        public override MachineOperand GetOperand(int i)
        {
            if (0 <= i && i < Operands.Length)
                return Operands[i];
            return null;
        }
    }
}
