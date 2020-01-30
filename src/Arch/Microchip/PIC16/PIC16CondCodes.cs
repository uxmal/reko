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

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    /// <summary>
    /// Class defining PIC16 instructions' usage and alteration of ALU condition flags.
    /// </summary>
    public static class PIC16CC
    {
        private static Dictionary<Mnemonic, FlagM> defCC = new Dictionary<Mnemonic, FlagM>()
        {
            { Mnemonic.ANDLW,  FlagM.Z },
            { Mnemonic.ANDWF,  FlagM.Z },
            { Mnemonic.CLRF,   FlagM.Z },
            { Mnemonic.CLRW,   FlagM.Z },
            { Mnemonic.COMF,   FlagM.Z },
            { Mnemonic.DECF,   FlagM.Z },
            { Mnemonic.INCF,   FlagM.Z },
            { Mnemonic.IORLW,  FlagM.Z },
            { Mnemonic.IORWF,  FlagM.Z },
            { Mnemonic.MOVF,   FlagM.Z },
            { Mnemonic.MOVIW,  FlagM.Z },
            { Mnemonic.XORLW,  FlagM.Z },
            { Mnemonic.XORWF,  FlagM.Z },
            { Mnemonic.RLF,    FlagM.C },
            { Mnemonic.RRF,    FlagM.C },
            { Mnemonic.ASRF,   FlagM.C | FlagM.Z },
            { Mnemonic.LSLF,   FlagM.C | FlagM.Z },
            { Mnemonic.LSRF,   FlagM.C | FlagM.Z },
            { Mnemonic.ADDLW,  FlagM.C | FlagM.DC | FlagM.Z },
            { Mnemonic.ADDWF,  FlagM.C | FlagM.DC | FlagM.Z },
            { Mnemonic.ADDWFC, FlagM.C | FlagM.DC | FlagM.Z },
            { Mnemonic.SUBLW,  FlagM.C | FlagM.DC | FlagM.Z },
            { Mnemonic.SUBWF,  FlagM.C | FlagM.DC | FlagM.Z },
            { Mnemonic.SUBWFB, FlagM.C | FlagM.DC | FlagM.Z },
        };

        private static Dictionary<Mnemonic, FlagM> useCC = new Dictionary<Mnemonic, FlagM>()
        {
            { Mnemonic.ADDWFC, FlagM.C },
            { Mnemonic.RLF,    FlagM.C },
            { Mnemonic.RRF,    FlagM.C },
            { Mnemonic.SUBWFB, FlagM.C },
        };

        /// <summary>
        /// Gets the condition code flags altered by the given instruction.
        /// </summary>
        /// <param name="mnemonic">The mnemonic for the instruction.</param>
        public static FlagM Defined(Mnemonic mnemonic)
        {
            if (defCC.TryGetValue(mnemonic, out FlagM flg))
                return flg;
            return 0;
        }

        /// <summary>
        /// Gets the condition code flags used by the given instruction.
        /// </summary>
        /// <param name="mnemonic">The mnemonic for the instruction.</param>
        public static FlagM Used(Mnemonic mnemonic)
        {
            if (useCC.TryGetValue(mnemonic, out FlagM flg))
                return flg;
            return 0;
        }
    }

}
