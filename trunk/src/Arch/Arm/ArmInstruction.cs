using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Arm
{
    public class ArmInstruction
    {
        public Condition Cond;
        public Opcode Opcode;
        public MachineOperand Dst;
        public MachineOperand Src1;
        public MachineOperand Src2;
    }
}
