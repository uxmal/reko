#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
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

using System.Collections.Generic;

namespace Reko.Libraries.Microchip
{
    /// <summary>
    /// Various extensions methods to manipulate PIC definitions.
    /// </summary>
    public static partial class PICCrownkingEx
    {

        private static readonly Dictionary<string, SFRBitAccess> _xlat2Access = new Dictionary<string, SFRBitAccess>()
        {
            { "x", SFRBitAccess.RW },               // read-write
            { "n", SFRBitAccess.RW },               // read-write
            { "X", SFRBitAccess.RW_Persistant },    // read-write persistent
            { "r", SFRBitAccess.ROnly },            // read-only
            { "w", SFRBitAccess.WOnly },            // write-only
            { "0", SFRBitAccess.Zero },             // 0-value
            { "1", SFRBitAccess.One },              // 1-value
            { "c", SFRBitAccess.Clr },              // clear-able only
            { "s", SFRBitAccess.Set },              // settable only
            { "-", SFRBitAccess.UnImpl },           // not implemented
        };

        /// <summary>
        /// Translates the first char of a SFR access mode string to a value from the <see cref="SFRBitAccess"/> enumeration.
        /// </summary>
        /// <param name="sAccess">The access mode string. Only first char is used.</param>
        /// <returns>
        /// A value from <see cref="SFRBitAccess"/> enumeration.
        /// </returns>
        public static SFRBitAccess SFRBitAccessMode(this string sAccess)
        {
            if (sAccess.Length >= 1)
            {
                if (_xlat2Access.TryGetValue(sAccess.Substring(1, 1), out SFRBitAccess bmode))
                    return bmode;
            }
            return SFRBitAccess.Unknown;
        }

        private static readonly Dictionary<string, SFRBitReset> _xlat2BitReset = new Dictionary<string, SFRBitReset>()
        {
            { "0", SFRBitReset.Zero },          // reset to 0.
            { "1", SFRBitReset.One },           // reset to 1.
            { "u", SFRBitReset.Unchanged },     // unchanged by reset.
            { "x", SFRBitReset.Unknown },       // unknown after reset.
            { "q", SFRBitReset.Cond },          // depends on condition.
            { "-", SFRBitReset.UnImpl },        // no implemented.
        };

        /// <summary>
        /// Translates the SFR bit reset mode string (MCLR, POR) to a value from the <see cref="SFRBitReset"/> enumeration.
        /// </summary>
        /// <param name="sReset">The reset mode string.</param>
        /// <returns>
        /// A value from <see cref="SFRBitReset"/> enumeration.
        /// </returns>
        public static SFRBitReset SFRBitResetMode(this string sReset)
        {
            if (sReset.Length >= 1)
            {
                if (_xlat2BitReset.TryGetValue(sReset.Substring(1, 1), out SFRBitReset bmode))
                    return bmode;
            }
            return SFRBitReset.Unknown;
        }

        /// <summary>
        /// A PICCrownking extension method that gets a PIC descriptor.
        /// </summary>
        /// <param name="db">The PIC database to retrieve definition from.</param>
        /// <param name="sPICName">Name of the PIC.</param>
        /// <returns>
        /// The PIC descriptor or null.
        /// </returns>
        public static PIC GetPIC(this PICCrownking db, string sPICName)
            => db.GetPICAsXML(sPICName)?.ToObject<PIC>();

        /// <summary>
        /// A PICCrownking extension method that gets a PIC descriptor.
        /// </summary>
        /// <param name="db">The PIC database to retrieve definition from.</param>
        /// <param name="iProcID">Identifier for the processor.</param>
        /// <returns>
        /// The PIC descriptor or null.
        /// </returns>
        public static PIC GetPIC(this PICCrownking db, int iProcID)
            => db.GetPICAsXML(iProcID)?.ToObject<PIC>();

    }

}
