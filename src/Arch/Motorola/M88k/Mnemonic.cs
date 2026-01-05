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

namespace Reko.Arch.Motorola.M88k;

public enum Mnemonic
{
    Invalid,
    Nyi,

    add,
    add_ci,
    add_cio,
    add_co,
    addu,
    addu_ci,
    addu_cio,
    addu_co,
    and,
    and_c,
    and_u,
    bb0,
    bb0_n,
    bb1,
    bb1_n,
    bcnd,
    bcnd_n,
    br,
    br_n,
    bsr,
    bsr_n,
    clr,
    cmp,
    div,
    divu,
    ext,
    extu,
    fadd,
    fcmp,
    fdiv,
    ff0,
    ff1,
    fldcr,
    flt,
    fmul,
    fstcr,
    fsub,
    fxcr,
    @int,
    jmp,
    jmp_n,
    jsr,
    jsr_n,
    ld,
    ld_b,
    ld_bu,
    ld_d,
    ld_h,
    ld_hu,
    lda,
    lda_b,
    lda_d,
    lda_h,
    ldcr,
    mak,
    mask,
    mask_u,
    mul,
    nint,
    or,
    or_c,
    or_u,
    rot,
    rte,
    set,
    st,
    st_b,
    st_d,
    st_h,
    stcr,
    sub,
    sub_ci,
    sub_cio,
    sub_co,
    subu,
    subu_ci,
    subu_cio,
    subu_co,
    tb0,
    tb1,
    tbnd,
    tcnd,
    trnc,
    xcr,
    xmem,
    xmem_bu,
    xor,
    xor_c,
    xor_u,
}