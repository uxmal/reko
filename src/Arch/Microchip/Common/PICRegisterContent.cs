#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
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

namespace Reko.Arch.MicrochipPIC.Common
{

    //
    //    PIC registers individual bit's access mode are coded as-follows:
    //
    //        "x" -> read-write
    //        "n" -> read-write
    //        "X" -> read-write persistent
    //        "r" -> read-only
    //        "w" -> write-only. Read as 0.
    //        "0" -> 0-value
    //        "1" -> 1-value
    //        "c" -> clear-able only
    //        "s" -> settable only
    //        "-" -> not implemented. Read as 0.
    //
    //    PIC registers Power-On/Master-Reset bits' state are coded as-follows:
    //
    //        "0" -> reset to 0.
    //        "1" -> reset to 1.
    //        "u" -> unchanged by reset.
    //        "x" -> unknown after reset.
    //        "q" -> depends on condition.
    //        "-" -> no implemented. Read as 0.
    //


    /// <summary>
    /// This class provides PIC register content taking into account the PIC register's traits when resetting, reading or writing values.
    /// </summary>
    public class PICRegisterContent
    {
        private readonly PICRegisterTraits traits;

        public PICRegisterContent(PICRegisterTraits traits)
        {
            this.traits = traits ?? throw new ArgumentNullException(nameof(traits));
        }

        private PICRegisterAccessMasks Bits
        {
            get
            {
                if (bits is null)
                {
                    bits = PICRegisterAccessMasks.Create(traits);
                    actualValue = bits.ResetValue;
                }
                return bits;
            }
        }
        private PICRegisterAccessMasks? bits;

        public uint ActualValue
        {
            get => ((actualValue | Bits.ReadOrMask) & Bits.ReadAndMask) & Bits.ImplementedMask;
            set => actualValue = ((value & Bits.WriteAndMask) | Bits.WriteOrMask) & Bits.ImplementedMask;
        }
        private uint actualValue;

        public uint ResetValue
            => Bits.ResetValue;

    }

}
