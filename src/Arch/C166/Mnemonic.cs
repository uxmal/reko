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

namespace Reko.Arch.C166
{
    public enum Mnemonic
    {
        Invalid,

        add,
        add1,
        addb,
        addb1,
        addc,
        addc1,
        addcb,
        addcb1,
        and,
        and1,
        andb,
        andb1,
        ashr,
        atomic2,
        band,
        bclr,
        bcmp,
        bfldh,
        bfldl,
        bmov,
        bmovn,
        bor,
        bset,
        bxor,
        calla,
        calli,
        callr,
        calls,
        cmp,
        cmp1,
        cmpb,
        cmpb1,
        cmpd1,
        cmpd2,
        cmpi1,
        cmpi2,
        cpl,
        cplb,
        diswdt,
        div,
        divl,
        divlu,
        divu,
        einit,
        extp,
        extr,
        idle,
        jb,
        jbc,
        jmpa,
        jmpi,
        jmpr,
        jmps,
        jnb,
        jnbs,
        mov,
        movb,
        movbs,
        movbz,
        mul,
        mulu,
        neg,
        negb,
        nop,
        or,
        or1,
        orb,
        orb1,
        pcall,
        pop,
        prior,
        push,
        pwrdn,
        ret,
        reti,
        retp,
        rets,
        rol,
        ror,
        scxt,
        shl,
        shr,
        srst,
        srvwdt,
        sub,
        sub1,
        subb,
        subb1,
        subc,
        subc1,
        subcb,
        subcb1,
        trap,
        xor,
        xor1,
        xorb,
        xorb1,
    }
}
