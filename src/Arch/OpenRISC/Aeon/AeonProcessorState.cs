using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Code;

namespace Reko.Arch.OpenRISC.Aeon
{
    public class AeonProcessorState : ProcessorState
    {
        private AeonArchitecture arch;
        private Constant[] iregs;       // integer register values.
        private bool[] valid;       // whether the regs are valid or not.

        public AeonProcessorState(AeonArchitecture arch)
        {
            this.arch = arch;
            this.iregs = new Constant[32];
            this.valid = new bool[32];
        }

        public AeonProcessorState(AeonProcessorState that) : base(that)
        {
            this.arch = that.arch;
            this.iregs = (Constant[]) that.iregs.Clone();
            this.valid = (bool[]) that.valid.Clone();
        }

        public override IProcessorArchitecture Architecture => arch;

        public override ProcessorState Clone()
        {
            return new AeonProcessorState(this);
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            int rn = r.Number;
            if (0 <= rn && rn < 32 && valid[rn])
            {
                return iregs[rn];
            }
            else
            {
                return InvalidConstant.Create(r.DataType);
            }
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            int rn = r.Number;
            if (0 <= rn && rn < 32)
            {
                // Integer register.
                if (v.IsValid)
                {
                    iregs[rn] = v;
                    valid[rn] = true;
                    return;
                }
                else
                {
                    valid[rn] = false;
                }
            }
        }

        public override void OnProcedureEntered(Address addr)
        {
        }

        public override void OnProcedureLeft(FunctionType procedureSignature)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            return new CallSite(0, 0);
        }

        public override void OnAfterCall(FunctionType? sigCallee)
        {
        }
    }
}
