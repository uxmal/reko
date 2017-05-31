#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

namespace Reko.Core.Expressions
{
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

        IS_NAN, // comparison yielded an floating point NaN
    }
}
