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

namespace Reko.Arch.SuperH
{
    public enum Mnemonic
    {
        invalid,
        add,
        addc,
        addv,
        and,
        and_b,
        bf,
        bf_s,
        bra,
        braf,
        brk,
        bsr,
        bsrf,
        bt,
        bt_s,
        clrmac,
        clrs,
        clrt,
        cmp_eq,
        cmp_ge,
        cmp_gt,
        cmp_hi,
        cmp_hs,
        cmp_pl,
        cmp_pz,
        cmp_str,
        div0s,
        div0u,
        div1,
        dmuls_l,
        dmulu_l,
        dt,
        exts_b,
        exts_w,
        extu_b,
        extu_w,
        fabs,
        fadd,
        fcmp_eq,
        fcmp_gt,
        fcnvds,
        fcnvsd,
        fdiv,
        fipr,
        flds,
        fldi0,
        fldi1,
        fmac,
        jmp,
        jsr,
        ldc,
        lds,
        lds_l,
        ldtlb,
        mac_l,
        mov,
        mov_b,
        mov_w,
        mov_l,
        mova,
        movt,
        mul_l,
        muls_w,
        mulu_w,
        neg,
        negc,
        nop,
        not,
        ocbi,
        or,
        rotcl,
        rotcr,
        rotl,
        rotr,
        rts,
        sett,
        shad,
        shar,
        shld,
        shll,
        shll2,
        shll8,
        shll16,
        shlr,
        shlr2,
        shlr8,
        shlr16,
        stc,
        stc_l,
        sts,
        sts_l,
        sub,
        subc,
        subv,
        swap_w,
        tst,
        xor,
        xtrct,
        pref,
        ocbp,
        rte,
        sleep,
        movco_l,
        movca_l,
        movmu_l,
        shal,
        ldc_l,
        tas_b,
        mulr,
        divs,
        mac_w,
        fsub,
        fmul,
        fmov,
        fmov_s,
        fmov_d,
        @float,
        fsca,
        ftrv,
        frchg,
        fschg,
        ftrc,
        fneg,
        fsqrt,
    }
}
