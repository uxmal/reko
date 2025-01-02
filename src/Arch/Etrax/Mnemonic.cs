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
using System.Text;

namespace Reko.Arch.Etrax
{
    public enum Mnemonic
    {
        Invalid,
        nyi,

        abs,
        add,
        addi,
        addq,
        adds,
        addu,
        and,
        andq,
        asr,
        asrq,
        ba,
        bcc,
        bcs,
        beq,
        bge,
        bgt,
        bhi,
        ble,
        bls,
        blt,
        bmi,
        bne,
        bound,
        bpl,
        btst,
        btstq,
        bvc,
        bvs,
        bwf,
        clearf,
        cmp,
        cmpq,
        cmps,
        cmpu,
        dstep,
        jbrc,
        jir,
        jirc,
        jmpu,
        jsr,
        jsrc,
        jump,
        lsl,
        lslq,
        lsr,
        lsrq,
        lz,
        move,
        movem,
        moveq,
        movs,
        movu,
        mstep,
        muls,
        mulu,
        neg,
        nop,
        or,
        orq,
        rbf,
        sa,
        sbfs,
        scc,
        scs,
        seq,
        setf,
        sge,
        sgt,
        shi,
        sle,
        sls,
        slt,
        smi,
        sne,
        spl,
        sub,
        subq,
        subs,
        subu,
        svc,
        svs,
        swap,
        swf,
        test,
        xor,
        ret,
    }
}