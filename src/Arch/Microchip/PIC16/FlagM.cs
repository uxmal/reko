#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    /// <summary>
    /// Values that represent PIC16 condition code flags in STATUS register.
    /// </summary>
    public enum FlagM
    {
        /// <summary>Carry/Borrow flag.</summary>
        C = 1,
        /// <summary>Digit Carry/Borrow flag.</summary>
        DC = 2,
        /// <summary>Zero flag.</summary>
        Z = 4,
    }

    /// <summary>
    /// Class defining PIC16 instructions' usage and alteration of ALU condition flags.
    /// </summary>
    public static class PIC16CC
    {
        private static Dictionary<Opcode, FlagM> defCC = new Dictionary<Opcode, FlagM>()
        {
            { Opcode.CLRF,   FlagM.Z },
            { Opcode.CLRW,   FlagM.Z },
            { Opcode.ANDLW,  FlagM.Z },
            { Opcode.ANDWF,  FlagM.Z },
            { Opcode.COMF,   FlagM.Z },
            { Opcode.IORLW,  FlagM.Z },
            { Opcode.IORWF,  FlagM.Z },
            { Opcode.MOVF,   FlagM.Z },
            { Opcode.MOVIW,  FlagM.Z },
            { Opcode.DECF,   FlagM.Z },
            { Opcode.INCF,   FlagM.Z },
            { Opcode.XORLW,  FlagM.Z },
            { Opcode.XORWF,  FlagM.Z },
            { Opcode.RLF,    FlagM.C },
            { Opcode.RRF,    FlagM.C },
            { Opcode.ASRF,   FlagM.C | FlagM.Z },
            { Opcode.LSLF,   FlagM.C | FlagM.Z },
            { Opcode.LSRF,   FlagM.C | FlagM.Z },
            { Opcode.ADDLW,  FlagM.C | FlagM.DC | FlagM.Z },
            { Opcode.ADDWF,  FlagM.C | FlagM.DC | FlagM.Z },
            { Opcode.ADDWFC, FlagM.C | FlagM.DC | FlagM.Z },
            { Opcode.RESET,  FlagM.C | FlagM.DC | FlagM.Z },
            { Opcode.SUBLW,  FlagM.C | FlagM.DC | FlagM.Z },
            { Opcode.SUBWF,  FlagM.C | FlagM.DC | FlagM.Z },
            { Opcode.SUBWFB, FlagM.C | FlagM.DC | FlagM.Z },
        };

        private static Dictionary<Opcode, FlagM> useCC = new Dictionary<Opcode, FlagM>()
        {
            { Opcode.ADDWFC, FlagM.C },
            { Opcode.RLF,    FlagM.C },
            { Opcode.RRF,    FlagM.C },
            { Opcode.SUBWFB, FlagM.C },
        };

        /// <summary>
        /// Gets the condition code flags altered by the given opcode.
        /// </summary>
        /// <param name="opc">The opcode.</param>
        public static FlagM Defined(Opcode opc)
        {
            if (defCC.TryGetValue(opc, out FlagM flg))
                return flg;
            return 0;
        }

        /// <summary>
        /// Gets the condition code flags used by the given opcode.
        /// </summary>
        /// <param name="opc">The opcode.</param>
        public static FlagM Used(Opcode opc)
        {
            if (useCC.TryGetValue(opc, out FlagM flg))
                return flg;
            return 0;
        }
    }

}
