#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Arch.MCore
{
    public enum Mnemonic
    {
        invalid,
        ld_h,
        st_b,
        bkpt,
        sync,
        rte,
        rfi,
        stop,
        wait,
        doze,


        trap,

        mvc,
        mvcv,
        ldq,
        stq,
        ldm,
        stm,
        dect,
        decf,
        inct,
        incf,
        jmp,
        jsr,
        ff1,
        brev,
        xtrb3,
        xtrb2,
        xtrb1,
        xtrb0,
        zextb,
        sextb,
        zexth,
        sexth,
        declt,
        tstnbz,
        decgt,
        decne,
        clrt,
        clrf,
        abs,
        not,
        movt,
        mult,
        loopt,
        subu,
        addc,
        subc,
        movf,
        lsr,
        cmphs,
        cmplt,
        tst,
        cmpne,
        mfcr,
        mov,
        bgenr,
        rsub,
        ixw,
        and,
        xor,
        mtcr,
        asr,
        lsl,
        addu,
        ixh,
        or,
        andn,

        addi,
        cmplti,
        subi,
        rsubi,
        cmpnei,
        bmaski,
        divu,
        andi,
        bclri,
        divs,
        bgeni,
        bseti,
        btsti,
        xsr,
        rotli,
        asrc,
        asri,
        lslc,
        lsli,
        lsrc,
        lsri,

        h__exec,
        h__ret,
        h__call,
        h__ld,
        h__st,
        h__ld_h,
        h__st_h,
        movi,
        lrw,
        jmpi,
        jsri,
        bt,
        bf,
        br,
        bsr,
        st_h,
        ld_b,
        ld,
        st,
    }
}
