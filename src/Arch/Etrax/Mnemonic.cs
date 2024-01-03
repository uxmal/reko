#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

namespace Reko.Arch.Etrax
{
    public enum Mnemonic
    {
        Invalid,

        addq,
        moveq,
        subq,
        cmpq,

        andq,
        orq,
        btstq,
        asrq,
        lslq,
        lsrq,

        addu,
        adds,
        movu,
        movs,
        subu,
        subs,
        lsl,
        addi,
        muls,
        mulu,
        neg,
        bound,
        add,
        move,
        sub,
        cmp,
        and,
        or,
        asr,
        lsr,
        nop,
        abs,
        dstep,
        mstep,
        xor,
        test,
        cmpu,
        cmps,

        bcc,
        bcs,
        bne,
        beq,
        bvc,
        bvs,
        bpl,
        bmi,
        bls,
        bhi,
        bge,
        blt,
        bgt,
        ble,
        ba,
        bwf,

        scc,
        scs,
        sne,
        seq,
        svc,
        svs,
        spl,
        smi,
        sls,
        shi,
        sge,
        slt,
        sgt,
        sle,
        sa,
        swf,
        lz,
        clearf,
        jump,
        jsr,
        swap,
        movem,
    }
}
