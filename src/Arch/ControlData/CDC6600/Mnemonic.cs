#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Arch.ControlData.CDC6600;

public enum Mnemonic
{
    Invalid,

    DF,
    EQ,
    GE,
    ID,
    IR,
    JP,
    LT,
    NE,
    NG,
    NO,
    NZ,
    OR,
    PL,
    PS,
    REC,
    RJ,
    WEC,
    ZR,
    and,
    andn,
    asr,
    asrn,
    dadd,
    dmul,
    dsub,
    fadd,
    fadd_round,
    fdiv,
    fdiv_round,
    fmul,
    fmul_round,
    fsub,
    fsub_round,
    iadd,
    isub,
    lsl,
    lsln,
    mask,
    mov,
    neg,
    normalize,
    or,
    orn,
    pack,
    popcnt,
    round_normalize,
    sadd,
    ssub,
    unpack,
    xor,
    xorn,
}
