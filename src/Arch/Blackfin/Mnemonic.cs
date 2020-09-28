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

namespace Reko.Arch.Blackfin
{
    public enum Mnemonic
    {
        invalid,
        JUMP,
        JUMP_S,
        mov,
        mov_x,
        mov_xb,
        mov_zb,
        mov_xl,
        mov_zl,
        NOP,
        RTS,
        RTI,
        RTX,
        RTN,
        RTE,
        IDLE,
        CSYNC,
        SSYNC,
        EMUEXCEPT,
        ABORT,
        CLI,
        STI,
        CALL,
        add,
        asr,
        mul,
        mov_z,
        if_cc_jump,
        if_cc_jump_bp,
        if_cc_mov,
        mov_cc_eq,
        mov_cc_le,
        mov_cc_lt,
        mov_cc_ule,
        mov_cc_ult,
        if_ncc_jump,
        if_ncc_jump_bp,
        if_ncc_mov,
        JUMP_L,
        add3,
        sub3,
        and3,
        or3,
        xor3,
        shift1add,
        shift2add,
        iflush,
        prefetch,
        flush,
        flushinv,
        mov_cc,
        neg,
        not,
        mov_cc_n_bittest,
        mov_cc_bittest,
        bitset,
        bittgl,
        bitclr,
        lsr,
        lsl,
        lsr3,
        RAISE,
        EXCPT,
        sub,
        LINK,
        UNLINK,
        MNOP,
        asr3,
        lsl3,
        neg_cc,
        mov_r_cc,
        add_sh1,
        add_sh2,
        DIVS,
        DIVQ,
    }
}