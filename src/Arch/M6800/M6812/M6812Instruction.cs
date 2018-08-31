using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.M6800.M6812
{
    public class M6812Instruction : MachineInstruction
    {
        internal MachineOperand[] Operands;

        public override InstructionClass InstructionClass => GetInstructionClass();

        public override bool IsValid => Opcode != Opcode.invalid;

        public override int OpcodeAsInteger => (int)Opcode;

        public Opcode Opcode { get; set; }

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
        }

        private InstructionClass GetInstructionClass()
        {
            return InstructionClass.Linear;
        }
    }
}
