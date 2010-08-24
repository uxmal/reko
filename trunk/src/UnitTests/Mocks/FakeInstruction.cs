using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class FakeInstruction : MachineInstruction
    {
        private Operation operation;
        private MachineOperand[] ops;

        public FakeInstruction(Operation operation, params MachineOperand[] ops)
        {
            this.operation = operation;
            this.ops = ops;
        }

        public Operation Operation { get { return operation; } }
        public MachineOperand[] Operands { get { return ops; } }

        public override uint DefCc()
        {
            throw new NotImplementedException();
        }

        public override uint UseCc()
        {
            throw new NotImplementedException();
        }
    }
    public enum Operation
    {
        Add,
        Mul,
        Jump,
        Branch,
    }
}
