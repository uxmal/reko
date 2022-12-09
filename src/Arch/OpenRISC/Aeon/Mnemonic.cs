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
        bn_ble__i__,
        bg_bltsi__,
        bne__,
        bn_bnei__,
        bt_trap,
        bn_entri__,
        bn_add,
        bt_add__,
        l_add____,
        bt_addi__,
        bn_addi,
        bg_addi,
        bn_and,
        bn_andi,
        bg_andi,
        bn_bf,
        bg_bf,
        bn_bnf__,
        bn_cmov____,
        bn_cmovi____,
        bn_divu,
        bg_flush_line,
        bg_invalidate_line,
        bt_j,
        bg_j,
        bg_jal,
        bt_jr,
        bg_lbz__,
        bn_lhz,
        bg_lhz__,
        bn_lwz__,
        bg_lwz__,
        bg_mfspr,
        bg_movhi,
        bn_movhi__,
        bt_movi__,
        bg_mtspr,
        bn_mul,
        bn_nand__,
        bt_nop,
        bn_nop,
        bn_or,
        bn_ori,
        bg_ori,
        bn_sb__,
        bg_sb__,
        bn_sfeqi,
        bn_sfgeu,
        bn_sfgtui,
        bn_sfleui__,
        bn_sfltu,
        bn_sfne,
        bg_sfnei__,
        bn_sh__,
        bg_sh__,
        bn_sll__,
        bn_slli__,
        bn_srai__,
        bn_srl__,
        bn_srli__,
        bn_sub,
        bn_sw,
        bg_sw,
        bg_sw__,
        bg_syncwritebuffer,
        bn_xor__,
        bt_mov__,
    }
}