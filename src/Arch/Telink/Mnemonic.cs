using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Telink
{
    public enum Mnemonic
    {
        Invalid,
        Nyi,
        tpush,
        tloadr,
        tloadrb,
        tcmp,

        tjeq,
        tjne,
        tjcs,
        tjcc,
        tjmi,
        tjpl,
        tjvs,
        tjvc,
        tjhi,
        tjls,
        tjge,
        tjlt,
        tjgt,
        tjle,
        tjal,
    }
}
