using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Arm
{
    public enum Condition : byte
    {
        eq,
        ne,
        cs,
        cc,
        mi,
        pl,
        vs,
        vc,
        hi,
        ls,
        ge,
        lt,
        gt,
        le,
        al,
        nv,
    }
}
