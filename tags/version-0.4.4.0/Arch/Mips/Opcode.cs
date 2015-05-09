#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

namespace Decompiler.Arch.Mips
{
    public enum Opcode
    {
        None,

        add,
        addi,
        addiu,
        addu,
        and,
        andi,
        beq,
        beql,
        bgez,
        bgezal,
        bgezall,
        bgezl,
        bgtz,
        bgtzl,
        blez,
        blezl,
        bltz,
        bltzal,
        bltzall,
        bltzl,
        bne,
        bnel,
        @break,
        dadd,
        daddi,
        daddiu,
        daddu,
        ddiv,
        ddivu,
        div,
        divu,
        dmult,
        dmultu,
        dsll,
        dsll32,
        dsllv,
        dsra,
        dsra32,
        dsrav,
        dsrl,
        dsrl32,
        dsrlv,
        dsub,
        dsubu,
        j,
        jal,
        jalr,
        jr,
        lb,
        lbu,
        ld,
        ldl,
        ldr,
        lh,
        lhu,
        ll,
        lld,
        lui,
        lw,
        lwl,
        lwr,
        lwu,
        mfhi,
        mflo,
        mthi,
        mtlo,
        movn,
        movz,
        mult,
        multu,
        nor,
        or,
        ori,
        pref,
        sb,
        sc,
        scd,
        sd,
        sdl,
        sdr,
        sh,
        slti,
        sltiu,
        sw,
        swl,
        swr,
        swu,
        xor,
        xori,
    }
}
