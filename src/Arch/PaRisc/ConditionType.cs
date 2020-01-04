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

namespace Reko.Arch.PaRisc
{
    public enum ConditionType
    {
        Never,
        Eq,
        Lt,
        Le,
        Nuv,
        Znv,
        Sv,
        Odd,
        Tr,
        Ne,
        Ge,
        Gt,
        Uv,
        Vnz,
        Nsv,
        Even,
        Ult,
        Ule,
        Uge,
        Ugt,

        Eq64,
        Lt64,
        Le64,
        Nuv64,
        Znv64,
        Sv64,
        Odd64,
        Ne64,
        Ge64,
        Gt64,
        Uv64,
        Vnz64,
        Nsv64,
        Even64,
        Ult64,
        Ule64,
        Uge64,
        Ugt64,

        FpFalseQ,
        FpFalse,
        Fp2,
        Fp3,

        Fp4,
        Fp5,
        Fp6,
        Fp7,

        Fp8,
        Fp9,
        Fp10,
        Fp11,

        Fp12,
        Fp13,
        Fp14,
        Fp15,

        Fp16,
        Fp17,
        Fp18,
        Fp19,

        Fp20,
        Fp21,
        Fp22,
        Fp23,

        Fp24,
        Fp25,
        Fp26,
        Fp27,

        Fp28,
        Fp29,
        Fp30,
        Fp31,
    }
}