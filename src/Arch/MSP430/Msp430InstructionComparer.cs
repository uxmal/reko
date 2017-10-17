using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.Msp430
{
    internal class Msp430InstructionComparer : InstructionComparer 
    {
        public Msp430InstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (Msp430Instruction)x;
            var b = (Msp430Instruction)y;
            return CompareOperands(a.op1, b.op1) && CompareOperands(a.op2, b.op2);
        }

        private bool CompareOperands(MachineOperand op1, MachineOperand op2)
        {
            if (op1 == null && op2 == null)
                return true;
            if (op1 == null || op2 == null)
                return false;
            if (op1.GetType() != op2.GetType())
                return false;
            var r1 = op1 as RegisterOperand;
            if (r1 != null)
            {
                var r2 = (RegisterOperand)op2;
                return CompareRegisters(r1.Register, r2.Register);
            }
            throw new NotImplementedException();
        }

        public override int GetOperandsHash(MachineInstruction instr)
        {
            var a = (Msp430Instruction)instr;
            var h = GetOperandHash(a.op1) ^ GetOperandHash(a.op2) * 37;
            return h;
        }

        private int GetOperandHash(MachineOperand op)
        {
            var h = op.GetType().GetHashCode();
            var r = op as RegisterOperand;
            if (r != null)
            {
                return GetRegisterHash(r.Register);
            }
            throw new NotImplementedException();
        }
    }
}