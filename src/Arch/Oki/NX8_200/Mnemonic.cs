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

namespace Reko.Arch.Oki.NX8_200;

public enum Mnemonic
{
    Invalid,

    adc,
    adcb,
    add,
    addb,
    and,
    andb,
    brk,
    cal,
    clr,
    clrb,
    cmp,
    cmpb,
    cmpc,
    cmpcb,
    daa,
    das,
    dec,
    decb,
    div,
    divb,
    extnd,
    inc,
    incb,
    j,
    jbr,
    jbs,
    jeq,
    jge,
    jgt,
    jle,
    jlt,
    jne,
    jrnz,
    l,
    lb,
    lc,
    lcb,
    mb,
    mbr,
    mov,
    movb,
    mul,
    mulb,
    nop,
    nyi,
    or,
    orb,
    pops,
    pushs,
    rb,
    rbr,
    rc,
    rol,
    rolb,
    ror,
    rorb,
    rt,
    rti,
    sb,
    sbc,
    sbcb,
    sbr,
    sc,
    scal,
    sj,
    sll,
    sllb,
    sra,
    srab,
    srl,
    srlb,
    st,
    stb,
    sub,
    subb,
    swap,
    swapb,
    tbr,
    vcal,
    xchg,
    xchgb,
    xnbl,
    xor,
    xorb,
}
