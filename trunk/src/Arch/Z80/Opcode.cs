using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Z80
{
    public enum Opcode
    {
        illegal = 0,

        // i8080 opcodes
        aci,
        adi,
        dad,
        dcr,
        inr,
        lda,
        ldax,
        lhld,
        lxi,
        mov,
        mvi,
        shld,
        sphl,
        sta,
        stax,
        sbb,
        sbi,
        sui,

        // Z80 opcodes
        dec,
        inc,
        ld,
        sbc,

        // Shared opcodes
        adc,
        add,
        di,
        ei,
        hlt,
        nop,
        sub,
    }
}
