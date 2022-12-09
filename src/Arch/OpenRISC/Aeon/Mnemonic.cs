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

namespace Reko.Arch.OpenRISC.Aeon
{
    public enum Mnemonic
    {
        Invalid,
        Nyi,

        bn_beqi__,
        bg_beqi__,
        bgtu__,
        bgtui__,
        ble__i__,
        bg_blti__,
        bne__,
        bn_bnei__,
        bt_trap,
        bn_entri__,
        l_add,
        l_add__,
        l_add____,
        bt_addi__,
        bn_addi,
        bg_addi,
        l_and,
        l_andi,
        l_andi__,
        bn_bf,
        bg_bf,
        bn_bnf__,
        l_cmov____,
        l_cmovi____,
        bn_divu,
        l_flush_line,
        l_invalidate_line,
        l_j,
        l_jal,
        l_jr,
        l_lbz__,
        l_lhz,
        l_lhz__,
        l_lwz__,
        l_mfspr,
        l_movhi,
        l_movhi__,
        l_movi__,
        l_mtspr,
        l_mul,
        bn_nand__,
        bt_nop,
        bn_nop,
        l_or,
        l_ori,
        l_sb__,
        l_sfeqi,
        l_sfgeu,
        l_sfgtui,
        l_sfleui__,
        l_sfltu,
        l_sfne,
        l_sfnei__,
        l_sh__,
        bn_sll__,
        bn_slli__,
        bn_srai__,
        bn_srl__,
        bn_srli__,
        bn_sub,
        l_sw,
        l_sw__,
        bg_syncwritebuffer,
        bn_xor__,
        mov__,
    }
}