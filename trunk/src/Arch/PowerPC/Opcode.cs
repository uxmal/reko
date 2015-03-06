#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public enum Opcode : ushort
    {
        illegal,

        add,
        addc,
        addco,
        addi,
        addic,
        addis,
        addze,
        andi,
        andis,
        b,
        bc,
        bcl,
        bcctr,
        bctrl,
        beq,
        beql,
        bge,
        bgel,
        bgt,
        bgtl,
        bl,
        ble,
        blel,
        blr,
        blrl,
        blt,
        bltl,
        bne,
        bnel,
        bns,
        bnsl,
        bso,
        bsol,
        cmp,
        cmpi,
        cmpl,
        cmpli,
        cmplw,
        cmpwi,
        cntlzw,
        crnor,
        cror,
        crxor,
        divwu,
        fadd,
        fadds,
        fcmpu,
        fctiwz,
        fdiv,
        fdivs,
        fsub,
        fsubs,
        fmul,
        fmuls,
        fres,
        fmadds,
        fmr,
        fmsubs,
        fnmadds,
        fnmsubs,
        fsqrts,
        lbz,
        lbzu,
        lbzux,
        lbzx,
        lfd,
        lfdu,
        lfs,
        lfsu,
        lha,
        lhau,
        lhz,
        lhzu,
        lhzx,
        lmw,
        lwarx,
        lwzu,
        lwz,
        lwzx,
        mfcr,
        mfctr,
        mflr,
        mtcrf,
        mtctr,
        mtlr,
        mulli,
        mullw,
        neg,
        nor,
        or,
        ori,
        oris,
        rfi,
        rlwimi,
        rlwinm,
        sc,
        slw,
        srawi,
        stb,
        stbu,
        stbux,
        stbx,
        stfd,
        stfdu,
        stfs,
        stfsu,
        sth,
        sthu,
        stmw,
        stw,
        stwu,
        stwux,
        stwx,
        subf,
        subfic,
        tw,
        twi,
        xor,
        xori,
        xoris,
    }
}
