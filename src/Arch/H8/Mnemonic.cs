#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

namespace Reko.Arch.H8
{
    public enum Mnemonic
    {
        Invalid,

        addx,
        bra,
        brn,
        bhi,
        bls,
        bcc,
        bcs,
        bne,
        beq,
        bvc,
        bvs,
        bpl,
        bmi,
        bge,
        blt,
        bgt,
        ble,
        mov,
        jsr,
        rts,
        xor,
        nop,
        sub,
        add,
        cmp,
        adds,
        not,
        jmp,
        subs,
        btst,
        or,
        and,
        extu,
        exts,
        subx,
        mulxu,
        bset,
        bnot,
        bclr,
        dec,
        ldc,
        shal,
        shll,
        bst,
        bist,
        bld,
        bild,
        shlr,
        shar,
        rotxl,
        rotl,
        rotxr,
        rotr,
        stc,
        xorc,
        andc,
        orc,
        inc,
        neg,
        rte,
        bor,
        bior,
        bxor,
        bixor,
        band,
        biand,
    }
}