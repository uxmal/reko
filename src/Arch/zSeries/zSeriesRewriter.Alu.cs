using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.zSeries
{
    public partial class zSeriesRewriter
    {
        private void RewriteLarl()
        {
            var src = Addr(instr.Ops[1]);
            var dst = Reg(instr.Ops[0]);
            m.Assign(dst, src);
        }

        private void RewriteStmg()
        {
            var rStart = ((RegisterOperand)instr.Ops[0]).Register;
            var rEnd = ((RegisterOperand)instr.Ops[1]).Register;
            var ea = EffectiveAddress(instr.Ops[2]);
            var tmp = binder.CreateTemporary(ea.DataType);
            m.Assign(tmp, ea);
            int i = rStart.Number;
            for (; ; )
            {
                var r = binder.EnsureRegister(Registers.GpRegisters[i]);
                m.Assign(m.Mem(r.DataType, tmp), r);
                if (i == rEnd.Number)
                    break;
                m.Assign(tmp, m.IAdd(tmp, Constant.Int(r.DataType, r.DataType.Size)));
                i = (i + 1) % 16;
            }
        }
    }
}
