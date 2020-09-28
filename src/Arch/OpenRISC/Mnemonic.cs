#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

namespace Reko.Arch.OpenRISC
{
    public enum Mnemonic
    {
        Invalid,
        l_add,
        l_addc,
        l_addi,
        l_addic,
        l_adrp,
        l_and,
        l_andi,
        l_bf,
        l_bnf,
        l_cmov,
        l_div,
        l_divu,
        l_ff1,
        l_fl1,
        l_j,
        l_jal,
        l_jalr,
        l_jr,
        l_lbs,
        l_lbz,
        l_ld,
        l_lf,
        l_lhs,
        l_lhz,
        l_lwa,
        l_lws,
        l_lwz,
        l_maci,
        l_macrc,
        l_mfspr,
        l_movhi,
        l_mtspr,
        l_mul,
        l_muld,
        l_muldu,
        l_muli,
        l_mulu,
        l_nop,
        l_or,
        l_ori,
        l_rfe,
        l_ror,
        l_rori,
        l_sb,
        l_sd,
        l_sfeq,
        l_sfeqi,
        l_sfges,
        l_sfgesi,
        l_sfgeu,
        l_sfgeui,
        l_sfgts,
        l_sfgtsi,
        l_sfgtu,
        l_sfgtui,
        l_sfles,
        l_sflesi,
        l_sfleu,
        l_sfleui,
        l_sflts,
        l_sfltsi,
        l_sfltu,
        l_sfltui,
        l_sfne,
        l_sfnei,
        l_sh,
        l_sll,
        l_slli,
        l_sra,
        l_srai,
        l_srl,
        l_srli,
        l_sub,
        l_sw,
        l_sys,
        l_trap,
        l_xor,
        l_xori,
        l_msync,
        l_psync,
        l_csync,
    }
}