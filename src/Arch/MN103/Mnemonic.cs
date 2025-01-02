#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Arch.MN103
{
    public enum Mnemonic
    {
        Invalid,
        nyi,
        clr,
        mov,
        movbu,
        movhu,
        extbu,
        extb,
        exthu,
        exth,
        add,
        inc,
        inc4,
        asl2,
        cmp,
        blt,
        bgt,
        bge,
        ble,

        bcs,
        bhi,
        bcc,
        bls,

        beq,
        bne,
        bra,
        nop,
        jmp,
        call,
        movm,
        llt,
        lgt,
        lge,
        lle,
        lcs,
        lhi,
        lcc,
        lls,
        leq,
        lne,
        lra,
        setlb,
        retf,
        ret,
        bset,
        bclr,
        rets,
        rti,
        trap,
        calls,
        sub,
        addc,
        subc,
        and,
        or,
        xor,
        not,
        divu,
        div,
        mulu,
        mul,
        rol,
        ror,
        asr,
        lsr,
        asl,
        ext,
        bvc,
        bvs,
        bnc,
        bns,
        btst,
    }
}
