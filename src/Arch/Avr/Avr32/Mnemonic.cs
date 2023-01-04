#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System.Text;

namespace Reko.Arch.Avr.Avr32
{
    public enum Mnemonic
    {
        invalid,

        mov,
        eor,
        ld_w,
        st_w,
        lddpc,
        sub,
        rsub,
        mcall,
        pushm,
        ld_ub,
        br,
        movh,
        cp_b,
        lddsp,
        stdsp,
        cp_w,
        icall,
        popm,
        ld_sh,
        ld_uh,
        st_b,
        rcall,
        lsr,
        rjmp,
        lsl,
        or,
        stm,
        abs,
        st_h,
        st_d,
        ret,
        sr,
        add,
        and,
        andnot,
        tst,
        casts_h,
        casts_b,
        castu_b,
        castu_h,
        com,
        cpc,
        musfr,
        mustr,
        neg,
        rol,
        ror,
        scr,
        swap_b,
        swap_bh,
        swap_h,
        tbnz,
        mul,
        sbr,
        ld_sb,
        asr,
        acr,
        ldm,
        cbr,
        ld_d,
        adc,
        acall,
        andl,
        andh,
        satsub_w,
        stcond,
        sbc,
        cp_h,
        bfextu,
        bfexts,
        divs,
        divu,
        mulu_d,
        muls_d,
        macu_d,
        macs_d,
        max,
        min,
        orl,
        orh,
        clz,
        bld,
        subf,
        bst,
        eorl,
        eorh,
        nop,
        xchg,
        sats,
        satu,
    }
}
