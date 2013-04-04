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
        ana,
        cma,
        cmc,
        cmp,
        dad,
        dcr,
        dcx,
        inr,
        inx,
        jc,
        jm,
        jmp,
        jnc,
        jnz,
        jpe,
        jpo,
        jz,
        lda,
        ldax,
        lhld,
        lxi,
        mov,
        mvi,
        ora,
        pchl,
        shld,
        sphl,
        sta,
        stax,
        sbb,
        sbi,
        stc,
        sui,
        xra,

        // Z80 opcodes
        and,
        ccf,
        cp,
        cpl,
        dec,
        inc,
        ld,
        or,
        sbc,
        scf,
        xor,

        // Shared opcodes
        adc,
        add,
        daa,
        di,
        ei,
        hlt,
        jp,
        nop,
        sub,
    }
}
