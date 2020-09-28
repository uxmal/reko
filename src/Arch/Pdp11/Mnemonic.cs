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

namespace Reko.Arch.Pdp11
{
    public enum Mnemonic
    {
        illegal = -1,

        adc,
        adcb,
        add,
        addb,
        asl,
        aslb,
        asr,
        asrb,
        bcc,
        bcs,
        beq,
        bge,
        bgt,
        bhi,
        bic,
        bicb,
        bis,
        bisb,
        bit,
        bitb,
        ble,
        blos,
        blt,
        bmi,
        bne,
        bpl,
        bpt,
        br,
        bvc,
        bvs,
        clr,
        clrflags,
        cmp,
        cmpb,
        com,
        comb,
        dec,
        decb,
        div,

        emt,
        halt,
        inc,
        incb,
        iot,
        ash,
        clrb,
        ashc,
        jmp,
        jsr,
        mark,
        mov,
        movb,
        mul,
        mfpd,
        mfpi,
        mfps,
        mtps,
        mtpi,
        mtpd,
        neg,
        negb,
        nop,

        reset,
        rol,
        rolb,
        ror,
        rorb,

        rti,
        rtt,
        rts,
        sbc,
        sbcb,
        setflags,
        sob,
        spl,
        sub,
        swab,
        sxt,
        trap,
        tst,
        tstb,
        wait,
        xor,
        mulf,
        modf,
        addf,
        subf,
        cmpf,
        divf,
        stexp,
        stcdi,
        stcfd,
        ldexp,
        ldcid,
        ldcfd,
    }
}
