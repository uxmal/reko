#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

namespace Reko.Arch.PaRisc
{
    public enum Opcode
    {
        invalid,

        diag,
        fmpyadd,
        ldil,
        addil,
        copr,
        ldo,
        ldb,
        ldh,
        ldw,
        ldwm,
        stb,
        sth,
        stw,
        stwm,
        combt,
        comibt,
        combf,
        comibf,
        comiclr,
        fmpysub,
        addbt,
        addibt,
        addbf,
        addibf,
        bvb,
        bb,
        movb,
        movib,
        be,
        be_l,
        add,
        @break,
        addo,
        addc,
        addco,
        shladd,
        shladdo,
        sub,
        subo,
        subt,
        subto,
        subb,
        subbo,
        ds,
        andcm,
        and,
        or,
        xor,
        uxor,
        comclr,
        uaddcm,
        uaddcmt,
        addl,
        sh1addl,
        sh2addl,
        sh3addl,
        dcor,
        idcor,
        b_l,
        gate,
        blr,
        bv,

        sync,
        syncdma,
        rfi,
        rfir,
        ssm,
        rsm,
        mtsm,
        ldsid,
        mtsp,
        mfsp,
        mtctl,
        mfctl,
        mfctl_w,
        bve,
        blrpush,

        ldd,
        ldda,
        ldcd,
        ldwa,
        ldcw,
        std,
        stby,
        stdby,
        stwa,
        stda,
        cmpb,
        cstd,
        cmpib,
        depwi,
        fldw,
        fstw,
        addi,
        extrw,
        addib,
        addb,
        subi,
        subi_tsv,
        add_l,
        sub_b,
        idtlbt,
        pdtlb,
        pdtlbe,
        fdc,
        fdce,
        pdc,
        fic,
        probe,
        probei,
        lpa,
        lci,
        add_c,
        fsub,
        fadd,
        fmpy,
        fdiv,
        fcmp,
        spop0,
        spop1,
        spop2,
        spop3,
    }
}