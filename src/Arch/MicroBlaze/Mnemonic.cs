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


namespace Reko.Arch.MicroBlaze
{
    public enum Mnemonic
    {
        Invalid,
        xor,
        addk,
        rsubk,
        cmp,
        cmpu,
        addik,
        mul,
        mulh,
        mulhsu,
        mulhu,
        sra,
        src,
        srl,
        sext16,
        br,
        brd,
        brld,
        brad,
        bra,
        brald,
        brk,
        ori,
        andi,
        xori,
        andni,
        bri,
        brai,
        brki,
        brid,
        brlid,
        braid,
        bralid,
        beqi,
        bnei,
        blti,
        blei,
        bgti,
        bgei,
        beqid,
        bneid,
        bltid,
        bleid,
        bgtid,
        bgeid,
        lw,
        lbu,
        lhu,
        sb,
        sh,
        sw,
        lwi,
        lbui,
        lhui,
        sbi,
        shi,
        swi,
        sext8,
        imm,
        rtsd,
        rtid,
        rtbd,
        rted,
        or,
        and,
        andn,
        add,
        rsub,
        addc,
        rsubc,
        addi,
        rsubi,
        addic,
        rsubic,
        rsubik,
        addikc,
        rsubikc,
        nop,
    }
}