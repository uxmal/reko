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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    public enum Opcode
    {
        illegal,

        add,
        add_d,
        addi,
        addiu,
        addu,
        and,
        andi,
        bc1f,
        bc1t,
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
        c_le_d,
        cfc1,
        ctc1,
        cvt_w_d,
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
        mfc0,
        mfc1,
        mfhi,
        mflo,
        mtc0,
        mtc1,
        mthi,
        mtlo,
        movn,
        movz,
        mult,
        multu,
        nop,
        nor,
        or,
        ori,
        pref,
        rdhwr,
        sb,
        sc,
        scd,
        sd,
        sdl,
        sdr,
        sh,
        sll,
        sllv,
        slt,
        slti,
        sltiu,
        sltu,
        sra,
        srav,
        srl,
        srlv,
        sub,
        subu,
        sw,
        swc1,
        swl,
        swr,
        sync,
        syscall,
        teq,
        tlt,
        tltu,
        tge,
        tgeu,
        tne,
        xor,
        xori,
    }
}
