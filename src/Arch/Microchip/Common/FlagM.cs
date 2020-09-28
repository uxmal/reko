#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
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

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// Values that represent PIC condition code flags in STATUS register.
    /// </summary>
    [Flags]
    public enum FlagM
    {
        /// <summary>Carry/Borrow flag.</summary>
        C = 1,
        /// <summary>Digit Carry/Borrow flag.</summary>
        DC = 2,
        /// <summary>Zero flag.</summary>
        Z = 4,
        /// <summary>Overflow flag. PIC18 only.</summary>
        OV = 8,
        /// <summary>Negative flag. PIC18 only.</summary>
        N = 16,
    }

}
