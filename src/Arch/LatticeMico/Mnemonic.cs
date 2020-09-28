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

namespace Reko.Arch.LatticeMico
{
    public enum Mnemonic
    {
        Invalid,

        add,
        addi,
        and,
        andhi,
        andi,
        b,
        be,
        bg,
        bge,
        bgeu,
        bgu,
        bi,
        bne,
        call,
        calli,
        cmpe,
        cmpei,
        cmpg,
        cmpge,
        cmpgei,
        cmpgeu,
        cmpgeui,
        cmpgi,
        cmpgu,
        cmpgui,
        cmpne,
        cmpnei,
        div,
        divu,
        lb,
        lbu,
        lh,
        lhu,
        lw,
        mod,
        modu,
        mul,
        muli,
        nor,
        nori,
        or,
        orhi,
        ori,
        raise,
        rcsr,
        reserved,
        sb,
        sextb,
        sexth,
        sh,
        sl,
        sli,
        sr,
        sri,
        sru,
        srui,
        sub,
        sw,
        wcsr,
        xnor,
        xnori,
        xor,
        xori,
    }
}