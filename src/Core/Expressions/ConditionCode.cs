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

using Reko.Core.NativeInterface;
using System;

namespace Reko.Core.Expressions
{
    [NativeInterop]
    public enum ConditionCode
    {
        None,
        UGT,	// Unsigned >
        ULE,	// Unsigned <=
        ULT,	// Unsigned <
        GT,		// >
        GE,		// >=
        LT,		// <
        LE,		// <=
        UGE,	// Unsigned >=
        NO,		// No overflow
        NS,		// >= 0
        NE,		// != 
        OV,		// Overflow
        SG,		// < 0
        EQ,		// ==	
        PE,     // Parity even
        PO,     // parity odd

        ALWAYS, // Some architectures have this.
        NEVER,

        IS_NAN,     // comparison discovered an floating point NaN
        NOT_NAN,    // comparison didn't discover a NaN
    }

    public static class ConditionCodeEx
    {
        private static ConditionCode[] invertMap = new[]
        {
            ConditionCode.None,
            ConditionCode.ULE,
            ConditionCode.UGT,
            ConditionCode.UGE,
            ConditionCode.LE,
            ConditionCode.LT,
            ConditionCode.GE,
            ConditionCode.GT,
            ConditionCode.ULT,
            ConditionCode.OV,
            ConditionCode.SG,
            ConditionCode.EQ,
            ConditionCode.NO,
            ConditionCode.NS,
            ConditionCode.NE,
            ConditionCode.PO,
            ConditionCode.PE,
            ConditionCode.NEVER,
            ConditionCode.ALWAYS,
            ConditionCode.NOT_NAN,
            ConditionCode.IS_NAN
        };

        public static ConditionCode Invert(this ConditionCode cc)
        {
            return invertMap[(int)cc];
        }
    }
}
