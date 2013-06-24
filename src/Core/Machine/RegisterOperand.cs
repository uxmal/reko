using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Machine
{
    /// <summary>
    /// Represents a register operand of a <code>MachineInstruction</code>. Most
    /// modern architectures support this.
    /// </summary>
    public class RegisterOperand : MachineOperand
    {
        private RegisterStorage reg;

        public RegisterOperand(RegisterStorage reg) :
            base(reg.DataType)
        {
            this.reg = reg;
        }

        public RegisterStorage Register
        {
            get { return reg; }
        }

        public override string ToString()
        {
            return reg.ToString();
        }
    }
}
