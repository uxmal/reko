#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.zSeries
{
    public enum Opcode
    {
        invalid,

        aghi,
        ar,
        bor,
        bhr,
        bnler,
        blr,
        bnher,
        blhr,
        bner,
        ber,
        bnlhr,
        bher,
        bnlr,
        bler,
        bnhr,
        bnor,
        br,
        basr,
        cgf,
        cli,
        l,
        la,
        lg,
        lghi,
        larl,
        lr,
        mvi,
        ngr,
        nopr,
        nr,
        st,
        stmg,
        xc,
        ahi,
        lhi,
        cghi,
        chi,
        brc,
        jo,
        jh,
        jnle,
        jl,
        jnhe,
        jlh,
        jne,
        je,
        jnlh,
        jhe,
        jnl,
        jle,
        jnh,
        jno,
        j,
        jgo,
        jgh,
        jgnle,
        jgl,
        jgnhe,
        jglh,
        jgne,
        jge,
        jgnlh,
        jghe,
        jgnl,
        jgle,
        jgnh,
        jgno,
        jg,
        brctg,
        ltgfr,
        lgr,
        ltgr,
        agr,
        sgr,
        lgfr,
        brasl,
        brcl,
        nc,
        clc,
        oc,
        trtr,
        mvc,
        mvz,
        clg,
        stg,
        lmg,
        srag,
        slag,
        srlg,
        swr,
        cd,
        lgf,
        bassm,
    }
}
