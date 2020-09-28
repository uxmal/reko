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

namespace Reko.Arch.Tlcs.Tlcs900
{
    public enum Tlcs900Mnemonic
    {
        invalid,
        adc,
        add,
        and,
        bit,
        bs1b,
        bs1f,
        call,
        calr,
        ccf,
        chg,
        cp,
        cpl,
        daa,
        dec,
        decf,
        di,
        div,
        divs,
        djnz,
        ei,
        ex,
        exts,
        extz,
        halt,
        inc,
        incf,
        jp,
        jr,
        ld,
        lda,
        ldar,
        ldc,
        ldcf,
        ldd,
        lddr,
        ldf,
        ldi,
        ldir,
        ldirw,
        link,
        mdec1,
        mdec2,
        mul,
        mula,
        muls,
        neg,
        nop,
        or,
        paa,
        pop,
        push,
        rcf,
        res,
        ret,
        retd,
        reti,
        rl,
        rlc,
        rld,
        rr,
        rrc,
        rrd,
        sbc,
        scc,
        scf,
        set,
        sla,
        sll,
        sra,
        srl,
        sub,
        swi,
        tset,
        unlk,
        xor,
        zcf,
    }
}
