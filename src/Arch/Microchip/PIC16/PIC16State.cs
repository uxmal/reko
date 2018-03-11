using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Microchip.PIC16
{
    public class PIC16State: ProcessorState
    {
        private PIC16Architecture pIC16Architecture;

        public PIC16State(PIC16Architecture pIC16Architecture) => this.pIC16Architecture = pIC16Architecture;

        public override IProcessorArchitecture Architecture => throw new NotImplementedException();

        public override ProcessorState Clone() => throw new NotImplementedException();
        public override Constant GetRegister(RegisterStorage r) => throw new NotImplementedException();
        public override void SetRegister(RegisterStorage r, Constant v) => throw new NotImplementedException();
        public override void SetInstructionPointer(Address addr) => throw new NotImplementedException();
        public override void OnProcedureEntered() => throw new NotImplementedException();
        public override void OnProcedureLeft(FunctionType procedureSignature) => throw new NotImplementedException();
        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize) => throw new NotImplementedException();
        public override void OnAfterCall(FunctionType sigCallee) => throw new NotImplementedException();
    }
}
