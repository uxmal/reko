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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.MN103
{
    public enum Mnemonic
    {
        Invalid,
        nyi,
        clr,
        mov,
        movbu,
        movhu,
        extbu,
        extb,
        exthu,
        exth,
        add,
        inc,
        inc4,
        asl2,
        cmp,
        blt,
        bgt,
        bge,
        ble,

        bcs,
        bhi,
        bcc,
        bls,

        beq,
        bne,
        bra,
        nop,
        jmp,
        call,
        movm,
        llt,
        lgt,
        lge,
        lle,
        lcs,
        lhi,
        lcc,
        lls,
        leq,
        lne,
        lra,
        setlb,
        retf,
        ret,
        bset,
        bclr,
        rets,
        rti,
        trap,
        calls,
        sub,
        addc,
        subc,
        and,
        or,
        xor,
        not,
        divu,
        div,
        mulu,
        mul,
        rol,
        ror,
        asr,
        lsr,
        asl,
        ext,
        bvc,
        bvs,
        bnc,
        bns,
        btst,
        udf00,
        udf01,
        udf02,
        udf03,
        udf04,
        udf05,
        udf06,
        udf07,
        udf08,
        udf09,
        udf10,
        udf11,
        udf12,
        udf13,
        udf14,
        udf15,
        udf20,
        udf21,
        udf22,
        udf23,
        udf24,
        udf25,
        udf26,
        udf27,
        udf28,
        udf29,
        udf30,
        udf31,
        udf32,
        udf33,
        udf34,
        udf35,

        udfu00,
        udfu01,
        udfu02,
        udfu03,
        udfu04,
        udfu05,
        udfu06,
        udfu07,
        udfu08,
        udfu09,
        udfu10,
        udfu11,
        udfu12,
        udfu13,
        udfu14,
        udfu15,
        udfu20,
        udfu21,
        udfu22,
        udfu23,
        udfu24,
        udfu25,
        udfu26,
        udfu27,
        udfu28,
        udfu29,
        udfu30,
        udfu31,
        udfu32,
        udfu33,
        udfu34,
        udfu35,
        syscall,
    }
}
