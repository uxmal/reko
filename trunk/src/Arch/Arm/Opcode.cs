#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

namespace Decompiler.Arch.Arm
{
    public enum Opcode : short
    {
        illegal,

        adc,
        add,
        adr,
        adds,
        add16,
        add8,
        addsubx,
        subaddx,
        and,

        b,
        beq,
        bic,
        bkpt,
        bl,
        blx,
        bx,
        bxj,
        clz,
        cmn,
        cmp,
        cpy,
        cps,
        eor,

        ldc,
        ldm,
        ldf,
        ldr,
        ldrb,
        ldrbt,
        ldrd,
        ldrex,
        ldrh,
        ldrsb,
        ldrsh,
        ldrt,
        lfm,

        mla,
        mov,
        mvn,
        mul,
        orr,
        pkhbt,
        pkhtb,
        rev,
        rev16,
        revsh,
        rfe,
        rsb,
        rsc,
        sbc,
        sel,
        setendbe,
        setendle,
        sfm,
        smlad,
        smlald,
        smlalxy,
        smlawy,
        smlaxy,
        smlsd,
        smlsld,
        smmla,
        smmls,
        smmul,
        smuad,
        smull,
        smulwy,
        smulxy,
        smusd,
        smlal,

        stf,
        stm,
        str,
        strb,
        strbt,
        strd,
        strex,
        strh,
        strt,
        sub,
        sub16,
        sub8,
        swi,
        swp,
        swpb,
        teq,
        tst,
        umaal,
        umull,
        umlal,
        ands,
        srs,
        eors,
        subs,
        rsbs,
        adcs,
        sbcs,
        rscs,
        tsts,
        flt,
        fix,
        wfs,
        rfs,
        wfc,
        rfc,
        cmf,
        cnf,
        cmfe,
        cnfe,
        mcr,
        mrc,
        adf,
        muf,
        cdp,
        dvf,
        rdf,
        pow,
        rpw,
        rsf,
        suf,
        fdv,
        frd,
        rmf,
        fml,
        pol,
        mvf,
        log,
        abs,
        rnd,
        sqt,
        lgn,
        exp,
        sin,
        cos,
        mnf,
        tan,
        asn,
        acs,
        atn,
        nrm,
        urd,
        mrs,
        msr,
        stc,
        lsl,
        lsr,
        asr,
        ror,
        rrx,



    }
}
