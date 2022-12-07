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

        beqi__,
        ble__i__,
        l_blti__,
        bne__,
        bnei__,
        bt_trap,
        entri__,
        l_add__,
        l_addi,
        l_addi__,
        l_and,
        l_andi,
        l_andi__,
        l_bf,
        l_bnf__,
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
        l_nop,
        l_or__,
        l_ori,
        l_sb__,
        l_sfeqi,
        l_sfgeu,
        l_sfgtui,
        l_sfne,
        l_sh__,
        l_sll__,
        l_slli__,
        l_srai__,
        l_srl__,
        l_srli__,
        l_sw,
        l_sw__,
        l_syncwritebuffer,
        l_xor__,
        mov__,
    }
}