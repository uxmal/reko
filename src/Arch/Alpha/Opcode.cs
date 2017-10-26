#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

namespace Reko.Arch.Alpha
{
    public enum Opcode
    {
        invalid,

        addl,
        addl_v,
        addq,
        addq_v,
        and,
        lda,
        ldah,
        ldbu,
        ldq_u,
        ldwu,
        stb,
        stq,
        stq_u,
        stw,
        subl,
        bic,
        bis,
        cmovlbc,
        cmovlbs,
        cmovlt,
        cmovge,
        cmovgt,
        cmovne,
        cmpbge,
        cmpeq,
        cmple,
        cmplt,
        cmpule,
        cmpult,

        ornot,
        implver,
        s4addl,
        s4addq,
        s4subl,
        s4subq,
        s8addl,
        s8addq,
        s8subl,
        s8subq,
        subl_v,
        subq,
        subq_v,
        xor,
        ldl,
        ldq,
        stl,
        ldl_l,
        stl_c,
        stq_c,
        ldq_l,
        bgt,
        bge,
        bne,
        blbs,
        ble,
        blt,
        beq,
        blbc,
        fbgt,
        fbge,
        fbne,
        bsr,
        fble,
        fblt,
        fbeq,
        br,
    }
}