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

        bt_trap,
        bn_add,
        bt_add__,
        bt_addi__,
        bn_addi,
        bg_addi,
        bn_and,
        bn_andi,
        bg_andi,

        // These have not been reversed yet.
        bg_b000__,
        bg_b111__,

        bg_b011i__,
        bg_b100i__,
        bg_b111i__,

        bg_beq__,
        bg_beqi__,
        bn_beqi__,
        bn_bf,
        bg_bf,
        bg_bges__,
        bg_bgeu__,
        bn_bgt__i__,
        bg_bgts__,
        bg_bgtui__,
        bn_ble__i__,
        bg_blesi__,
        bg_bltsi__,
        bg_bltui__,
        bg_bne__,
        bn_bnei__,
        bn_bnf__,
        bn_cmov____,
        bn_cmovi____,
        bn_divs__,
        bn_divu,
        bn_entri__,
        bn_extbz__,
        bn_exthz__,
        bn_ff1__,
        bg_flush_line,
        bg_invalidate_line,
        bt_j,
        bn_j____,
        bg_j,
        bg_jal,
        bt_jalr__,
        bt_jr,
        bg_lbs__,
        bg_lbz__,
        bn_lbz__,
        bn_lhz,
        bg_lhz__,
        bn_lwz,
        bg_lwz,
        bg_mfspr,
        bt_mov__,
        bt_movhi__,
        bn_movhi__,
        bg_movhi,
        bt_movi__,
        bg_mtspr,
        bn_mul,
        bn_nand__,
        bt_nop,
        bn_nop,
        bn_or,
        bn_ori,
        bg_ori,
        bt_rfe,
        bn_rtnei__,
        bn_sb__,
        bg_sb__,
        bn_sfeq__,
        bn_sfeqi,
        bn_sfgeu,
        bg_sfgeui__,
        bn_sfgtu,
        bn_sfgtui,
        bn_sflesi__,
        bn_sfleui__,
        bg_sfleui__,
        bn_sfne,
        bg_sfnei__,
        bn_sfnei__,
        bn_sh__,
        bg_sh__,
        bn_sll__,
        bn_slli__,
        bn_srai__,
        bn_srl__,
        bn_srli__,
        bn_sub,
        bt_sw__,
        bn_sw,
        bg_sw,
        bg_sw__,
        bg_syncwritebuffer,
        bn_xor__,
    }
}