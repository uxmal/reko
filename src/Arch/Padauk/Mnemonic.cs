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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Padauk
{
    public enum Mnemonic
    {
        Invalid,

        add,
        addc,
        and,
        call,
        ceqsn,
        clear,
        cneqsn,
        comp,
        dec,
        disgint,
        dzsn,
        engint,
        @goto,
        idxm,
        inc,
        izsn,
        ldspth,
        ldsptl,
        ldt16,
        ldtabh,
        ldtabl,
        mov,
        mul,
        nadd,
        neg,
        nop,
        not,
        or,
        pcadd,
        popaf,
        pushaf,
        reset,
        ret,
        reti,
        set0,
        set1,
        sl,
        slc,
        sr,
        src,
        stopexe,
        stopsys,
        stt16,
        sub,
        subc,
        swap,
        swapc,
        t0sn,
        t1sn,
        trap,
        wdreset,
        xch,
        xor,
    }
}
