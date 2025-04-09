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

namespace Reko.Arch.Oki.NX8_500;

public enum Mnemonic
{
    Invalid,
    nyi,

    adc,
    adcb,
    brk,
    cal,
    clr,
    cmp,
    cmpb,
    dec,
    decb,
    di,
    div,
    l,
    lb,
    mov,
    movb,
    mul,
    nop,
    swap,
    srlb,
    srl,
    stb,
    st,
    addb,
    add,
    sdd,
    sj,
    sll,
    sllb,
    subb,
    sub,
}
