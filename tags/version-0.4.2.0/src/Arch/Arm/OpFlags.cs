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
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Arm
{
    [Flags]
    public enum OpFlags : short
    {
        None,
        S = 1,
        D = 2,
        E = 3,
        P = 4,

        // Rounding modes
        Pl = 5,
        Mi = 6,
        Zr = 7,

        L = 0x10,
     
        // Ldm/stm
        ED = 0x20,
        EA = 0x30,
        FD = 0x40,
        FA = 0x50,

        DA = 0x60,
        DB = 0x70,
        IA = 0x80,
        IB = 0x80,

        // 
        T = 0x40,
    }
}
