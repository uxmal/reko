#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Arch.Arm.AArch64
{
    public enum Opcode
    {
        Invalid,

        add,
        adds,
        adr,
        adrp,
        and,
        ands,
        asr,
        b,
        bfm,
        bic,
        bics,
        bl,
        blr,
        br,
        cbnz,
        cbz,
        ccmn,
        ccmp,
        csel,
        csinc,
        csinv,
        csneg,
        drps,
        eon,
        eor,
        eret,
        ldp,
        ldr,
        ldrb,
        ldrsb,
        ldrsw,
        ldurb,
        lsl,
        lsr,
        mov,
        movk,
        movn,
        movz,
        nop,
        orn,
        orr,
        prfm,
        ret,
        ror,
        sbfm,
        sev,
        sevl,
        stp,
        str,
        strb,
        sub,
        subs,
        sxtb,
        sxth,
        sxtw,
        sxtx,
        tbnz,
        tbz,
        ubfm,
        uxtb,
        uxth,
        uxtw,
        uxtx,
        wfe,
        wfi,
        yield,
        sdiv,
        strh,
        ldrh,
        ldrsh,
        sturb,
        ldursb,
        stur,
        ldur,
        sturh,
        ldurh,
        ldursh,
        ldursw,
        madd,
        msub,
        smaddl,
        smsubl,
        smulh,
        umaddl,
        umsubl,
        umulh,
        mul,
        mneg,
        cmp,
    }
}
