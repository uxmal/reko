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

namespace Reko.Arch.Rl78
{
    public enum Mnemonic
    {
        invalid,

        add,
        addc,
        addw,
        and,
        and1,
        bc,
        bf,
        bh,
        bnc,
        bnh,
        bnz,
        br,
        brk,
        bt,
        btclr,
        bz,
        call,
        callt,
        clr1,
        clrb,
        clrw,
        cmp,
        cmp0,
        cmps,
        cmpw,
        dec,
        decw,
        halt,
        inc,
        incw,
        mov,
        mov1,
        movs,
        movw,
        mulu,
        nop,
        not1,
        oneb,
        onew,
        or,
        or1,
        pop,
        push,
        ret,
        retb,
        reti,
        rol,
        rolc,
        rolwc,
        ror,
        rorc,
        sar,
        sarw,
        sel,
        set1,
        shl,
        shlw,
        shr,
        shrw,
        skc,
        skh,
        sknc,
        sknh,
        sknz,
        skz,
        stop,
        sub,
        subc,
        subw,
        xch,
        xchw,
        xor,
        xor1,
    }
}
