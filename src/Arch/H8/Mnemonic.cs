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

namespace Reko.Arch.H8
{
    public enum Mnemonic
    {
        Invalid,
        Nyi,

        add,
        adds,
        addx,
        and,
        andc,
        band,
        bcc,
        bclr,
        bcs,
        beq,
        bge,
        bgt,
        bhi,
        biand,
        bild,
        bior,
        bist,
        bixor,
        bld,
        ble,
        bls,
        blt,
        bmi,
        bne,
        bnot,
        bor,
        bpl,
        bra,
        brn,
        bset,
        bsr,
        bst,
        btst,
        bvc,
        bvs,
        bxor,
        clrmac,
        cmp,
        daa,
        das,
        dec,
        divxs,
        divxu,
        eepmov,
        exts,
        extu,
        inc,
        jmp,
        jsr,
        ldc,
        ldm,
        ldmac,
        mac,
        mov,
        movfpe,
        movtpe,
        mulxs,
        mulxu,
        neg,
        nop,
        not,
        or,
        orc,
        rotl,
        rotr,
        rotxl,
        rotxr,
        rte,
        rts,
        shal,
        shar,
        shll,
        shlr,
        sleep,
        stc,
        stm,
        stmac,
        sub,
        subs,
        subx,
        tas,
        trapa,
        xor,
        xorc,
   }
}