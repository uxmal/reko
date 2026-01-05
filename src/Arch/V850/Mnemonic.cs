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

namespace Reko.Arch.V850
{
    public enum Mnemonic
    {
        invalid,

        add,
        addi,
        and,
        andi,
        bge,
        bgt,
        bh,
        bl,
        ble,
        blt,
        bn,
        bnh,
        bnl,
        bnv,
        bnz,
        bp,
        br,
        bsa,
        bv,
        bz,
        clr1,
        cmp,
        dispose,
        divh,
        fetrap,
        jarl,
        jmp,
        jr,
        ld_b,
        ld_bu,
        ld_h,
        ld_hu,
        ld_w,
        mov,
        movea,
        movhi,
        mulh,
        mulhi,
        nop,
        not,
        not1,
        or,
        ori,
        rie,
        sar,
        satadd,
        satsub,
        satsubi,
        satsubr,
        set1,
        shl,
        shr,
        sld_b,
        sld_bu,
        sld_h,
        sld_hu,
        sld_w,
        sst_b,
        sst_h,
        sst_w,
        st_b,
        st_h,
        st_w,
        sub,
        subr,
        @switch,
        sxb,
        sxh,
        synce,
        syncm,
        syncp,
        tst,
        tst1,
        xor,
        xori,
        zxb,
        zxh,
    }
}