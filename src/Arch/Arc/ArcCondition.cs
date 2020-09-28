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

using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Arc
{
    public enum ArcCondition
    {
        AL = 0x00, //  RA Always 1
        EQ = 0x01, //  Z Zero Z
        NE = 0x02, //  NZ Non-Zero /Z
        PL = 0x03, //  P Positive /N
        MI = 0x04, //  N Negative N
        CS = 0x05, //  C, LO Carry set, lower than (unsigned) C
        CC = 0x06, //  NC, HS Carry clear, higher or same (unsigned) /C
        VS = 0x07, //  V Over-flow set V
        VC = 0x08, //  NV Over-flow clear /V
        GT = 0x09, //  Greater than (signed) (N and V and /Z) or (/N and /V and /Z)
        GE = 0x0A, // Greater than or equal to (signed) (N and V) or (/N and /V)
        LT = 0x0B, // Less than (signed) (N and /V) or (/N and V)
        LE = 0x0C, // Less than or equal to (signed) Z or (N and /V) or (/N and V)
        HI = 0x0D, // Higher than (unsigned) /C and /Z
        LS = 0x0E, // Lower than or same (unsigned) C or Z
        PNZ = 0x0F, //  Positive non-zero /N and /Z

        SC = 0x10,  // Saturation flags clear
        SS = 0x11,  // Saturation flags set

        Max = 0x12
    }
}
