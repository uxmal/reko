using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tlcs
{
    public class Tlcs90Instruction : MachineInstruction
    {
        public override InstructionClass InstructionClass 
        {
            get { return InstructionClass.Linear; }
        }

        public override bool IsValid
        {
            get { }
            {
                throw new NotImplementedException();
            }
        }

        public override int OpcodeAsInteger
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }
    }
}
