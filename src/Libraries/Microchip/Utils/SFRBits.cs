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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Libraries.Microchip
{
    /// <summary>
    /// SFR bits access modes.
    /// </summary>
    public enum SFRBitAccess : byte
    {
        /// <summary>Bit is read-write ('n') </summary>
        RW = 0,
        /// <summary>Bit is read-write persistent ('N') (no longer used?) </summary>
        RW_Persistant = 1,
        /// <summary>Bit is read-only ('r') </summary>
        ROnly = 2,
        /// <summary>Bit is write-only ('w') </summary>
        WOnly = 3,
        /// <summary>Bit is settable only ('s') </summary>
        Set = 4,
        /// <summary>Bit is clear-able only ('c') </summary>
        Clr = 5,
        /// <summary>Bit is always 0 ('0') </summary>
        Zero = 6,
        /// <summary>Bit is always 1 ('1') </summary>
        One = 7,
        /// <summary>Bit is unimplemented ('-') </summary>
        UnImpl = 8,
        /// <summary>Bit is undetermined ('x') </summary>
        UnDef = 9,
        /// <summary>Bit access mode is unknown ('?') </summary>
        Unknown = 10
    }

    /// <summary>
    /// Values that represent SFR bit state at resets (Master Clear, Power-On, ...).
    /// </summary>
    public enum SFRBitReset
    {
        /// <summary>Bit is reset to 0 ('0'). </summary>
        Zero = 0,
        /// <summary>Bit is reset to 1 ('1'). </summary>
        One = 1,
        /// <summary>Bit is unchanged ('u'). </summary>
        Unchanged = 2,
        /// <summary>Bit depends on condition ('q'). </summary>
        Cond = 3,
        /// <summary>Bit is unknown ('x'). </summary>
        Unknown = 4,
        /// <summary>Bit is not implemented ('-'). </summary>
        UnImpl = 5
    }

}
