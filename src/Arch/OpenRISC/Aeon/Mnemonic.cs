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
        l_movhi,
        l_nop,
        bt_trap,
        l_jr,
        mov__,
        l_add__,
        l_j,
        l_andi__,
        l_addi__,

        l_movhi__,
        l_lhz,
        l_sw,
        l_lwz__,
        l_sw__,
        l_addi,
        l_bf,

        beqi__,
        ble__i__,
        l_mul,
        l_and,
        l_or__,
        l_slli__,
        l_srli__,
        l_ori,
        l_andi,
        l_sfgtui,
        entri__,
        l_sfeqi,
        l_sfne,
        l_sfgeu,

        l_mtspr,
        l_mfspr,
        l_jal__,

        l_invalidate_line,
        l_syncwritebuffer
    }
}