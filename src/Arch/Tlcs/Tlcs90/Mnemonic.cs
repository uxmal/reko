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
using System.Threading.Tasks;

namespace Reko.Arch.Tlcs.Tlcs90
{
    public enum Mnemonic
    {
        invalid,
        adc,
        add,
        and,
        bit,
        call,
        callr,
        ccf,
        cp,
        cpl,
        daa,
        dec,
        decx,
        djnz,
        di,
        div,
        ei,
        ex,
        exx,
        halt,
        inc,
        incx,
        jp,
        jr,
        ld,
        ldar,
        ldw,
        mul,
        neg,
        nop,
        or,
        push,
        pop,
        rcf,
        res,
        ret,
        reti,
        rl,
        rr,
        rrc,
        sbc,
        scf,
        set,
        sra,
        sla,
        sll,
        srl,
        sub,
        xor,
        tset,
        ldi,
        ldir,
        ldd,
        lddr,
        cpi,
        cpir,
        cpdr,
        cpd,
        rlc,
        rld,
        rrd,
        incw,
        decw,
        swi,
    }
}
