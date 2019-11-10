using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Arc
{
    public enum Mnemonic
    {
        Invalid,
        nop,
        unimp_s,
        push_s,
        st,
        mov,
        add_s,
        mov_s,
        st_s,
        stw_s,
        bl,
        ld,
        ldb,
        ldw,
        stw,
        stb,
        pop_s,
        j_s,
        sub_s,
        ldw_s,
        bic,
    }
}
