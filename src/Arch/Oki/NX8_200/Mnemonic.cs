#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Arch.Oki.NX8_200;

public enum Mnemonic
{
    Invalid,
    nyi,
    brk,
    rb,
    clr,
    mov,
    mb,
    jeq,
    jgt,
    jlt,
    jne,
    jle,
    jge,
    extnd,
    inc,
    incb,
    j,
    jrnz,
    l,
    rc,
    rti,
    sj,
    cal,
    cmp,
    cmpb,
    daa,
    das,
    sc,
    rorb,
    ror,
    rolb,
    rol,
    swapb,
    swap,
    sub,
    subb,
    pushs,
    pops,
    movb,
    andb,
    and,
    or,
    orb,
    adcb,
    adc,
    add,
    addb,
    clrb,
    cmpc,
    decb,
    dec,
    divb,
    div,
    lb,
    lc,
    lcb,
    mbr,
    mulb,
    mul,
    nop,
    rbr,
    rt,
    sb,
    sbcb,
    sbc,
    sbr,
    scal,
    sll,
    sllb,
    sra,
    srab,
    srl,
    srlb,
    stb,
    st,
    tbr,
    vcal,
    xchg,
    xchgb,
    xnbl,
    xor,
    xorb,
}
