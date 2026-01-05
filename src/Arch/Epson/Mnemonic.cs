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

namespace Reko.Arch.Epson;

public enum Mnemonic
{
    Invalid,
    Nyi,
    ext,
    nop,
    add,
    sub,
    cmp,
    ld_w,
    and,
    or,
    xor,
    not,
    push,
    slp,
    halt,
    ld_cf,
    pop,
    pushs,
    pops,
    brk,
    pushn,
    popn,
    jpr,
    jpr_d,
    retd,
    @int,
    reti,
    call,
    ret,
    jp,
    call_d,
    ret_d,
    jp_d,
    jrgt,
    jrge,
    jrlt,
    jrle,
    jrugt,
    jruge,
    jrult,
    jrule,
    jreq,
    jrne,
    jrgt_d,
    jrge_d,
    jrlt_d,
    jrle_d,
    jrugt_d,
    jruge_d,
    jrult_d,
    jrule_d,
    jreq_d,
    jrne_d,
    ld_b,
    srl,
    ld_ub,
    sll,
    ld_h,
    sra,
    ld_uh,
    sla,
    rr,
    rl,
    swap,
    swaph,
    mlt_h,
    mltu_h,
    btst,
    mltu_w,
    mlt_w,
    bclr,
    bset,
    ld_c,
    sbc,
    adc,
    bnot,
    do_c,
    psrset,
    psrclr,
}