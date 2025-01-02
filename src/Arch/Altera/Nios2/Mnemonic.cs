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

namespace Reko.Arch.Altera.Nios2
{
    public enum Mnemonic
    {
        Invalid,

        add,
        addi,
        and,
        andhi,
        andi,
        beq,
        bge,
        bgeu,
        blt,
        bltu,
        bne,
        br,
        @break,
        bret,
        call,
        callr,
        cmpeq,
        cmpeqi,
        cmpge,
        cmpgei,
        cmpgeu,
        cmpgeui,
        cmplt,
        cmplti,
        cmpltu,
        cmpltui,
        cmpne,
        cmpnei,
        custom,
        div,
        divu,
        eret,
        flushd,
        flushda,
        flushi,
        flushp,
        initd,
        initda,
        initi,
        jmp,
        jmpi,
        ldb,
        ldbio,
        ldbu,
        ldbuio,
        ldh,
        ldhio,
        ldhu,
        ldhuio,
        ldw,
        ldwio,
        mul,
        muli,
        mulxss,
        mulxsu,
        mulxuu,
        nextpc,
        nor,
        or,
        orhi,
        ori,
        rdctl,
        rdprs,
        ret,
        rol,
        roli,
        ror,
        sll,
        slli,
        sra,
        srai,
        srl,
        srli,
        stb,
        stbio,
        sth,
        sthio,
        stw,
        stwio,
        sub,
        sync,
        trap,
        wrctl,
        wrprs,
        xor,
        xorhi,
        xori,
    }
}