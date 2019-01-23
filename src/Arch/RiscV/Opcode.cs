#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

namespace Reko.Arch.RiscV
{
    public enum Opcode
    {
        invalid, 

        and,
        addi,
        addiw,
        andi,
        auipc,

        beq,
        bge,
        bgeu,
        blt,
        bltu,
        bne,

        c_add,
        c_addi,
        c_addi16sp,
        c_addi4spn,
        c_addiw,
        c_addw,
        c_and,
        c_andi,
        c_beqz,
        c_bnez,
        c_fld,
        c_fldsp,
        c_flw,
        c_flwsp,
        c_j,
        c_jalr,
        c_jr,
        c_ld,
        c_ldsp,
        c_li,
        c_lui,
        c_lw,
        c_lwsp,
        c_mv,
        c_or,
        c_sd,
        c_sdsp,
        c_slli,
        c_srai,
        c_srli,
        c_sub,
        c_subw,
        c_sw,
        c_swsp,
        c_xor,

        flw,

        jal,
        jalr,

        lui,

        lb,
        ld,
        lh,

        or,
        ori,
        lw,
        sb,
        sd,
        sh,
        sw,

        slli,
        sra,
        srai,
        srl,
        srli,

        slti,
        sltiu,
        xori,
        add,
        sub,
        sll,
        slt,
        sltu,
        xor,
        addw,
        subw,
        sllw,
        srlw,
        sraw,
        slliw,
        srliw,
        sraiw,
        lbu,
        lhu,

        fadd_d,
        fadd_s,
        fcvt_d_s,
        fcvt_s_d,
        feq_s,
        fmv_d_x,
        fmv_s_x,
        fmadd_s,
        fmsub_s,
        fnmsub_s,
        fnmadd_s,
        lwu,
        fsw,

        mulw,
        divw,
        remuw,
        fld,
        fsd,
        c_fsdsp,
        c_fsd,
        remw,
        divuw,
    }
}