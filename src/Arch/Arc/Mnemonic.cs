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
        cmp_s,
        bne_s,
        beq_s,
        b_s,
        bgt_s,
        bge_s,
        blt_s,
        ble_s,
        bhi_s,
        bhs_s,
        blo_s,
        bls_s,
        asl_s,
        lsr_s,
        asr_s,
        bset_s,
        bclr_s,
        bmsk_s,
        btst_s,
        lr,
        b,
    }
}
