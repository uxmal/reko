#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Z80
{
    public enum Mnemonic
    {
        illegal = 0,

        // i8080 mnemonics
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

        // Z80 mnemonics
        and,
        bit,
        call,
        ccf,
        cp,
        cpd,
        cpdr,
        cpi,
        cpir,
        cpl,
        dec,
        djnz,
        ex,
        ex_af,
        exx,
        jr,
        inc,
        ld,
        ldd,
        lddr,
        ldi,
        ldir,
        im,
        @in,
        ind,
        indr,
        ini,
        inir,
        neg,
        or,
        otdr,
        otir,
        @out,
        outd,
        outi,
        outr,
        pop,
        push,
        res,
        ret,
        reti,
        retn,
        rla,
        rl,
        rlc,
        rlca,
        rld,
        rra,
        rr,
        rrc,
        rrd,
        rrca,
        rst,
        sbc,
        scf,
        sla,
        srl,
        sra,
        set,
        swap,
        xor,

        // Shared mnemonics
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
