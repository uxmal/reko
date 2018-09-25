using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.zSeries
{
    public partial class zSeriesRewriter
    {
        private void RewriteBr()
        {
            this.rtlc = RtlClass.Transfer;
            var dst = Reg(instr.Ops[0]);
            m.Goto(dst);
        }
    }
}
