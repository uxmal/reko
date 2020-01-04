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

using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.PIC18
{
    using Common;

    /// <summary>
    /// Class defining PIC18 instructions' usage and alteration of ALU condition flags.
    /// </summary>
    public static class PIC18CC
    {
        private static Dictionary<Mnemonic, FlagM> defCC = new Dictionary<Mnemonic, FlagM>()
        {
            { Mnemonic.DAW,    FlagM.C },
            { Mnemonic.CLRF,   FlagM.Z },
            { Mnemonic.ANDLW,  FlagM.Z | FlagM.N },
            { Mnemonic.ANDWF,  FlagM.Z | FlagM.N },
            { Mnemonic.COMF,   FlagM.Z | FlagM.N },
            { Mnemonic.IORLW,  FlagM.Z | FlagM.N },
            { Mnemonic.IORWF,  FlagM.Z | FlagM.N },
            { Mnemonic.MOVF,   FlagM.Z | FlagM.N },
            { Mnemonic.RLNCF,  FlagM.Z | FlagM.N },
            { Mnemonic.RRNCF,  FlagM.Z | FlagM.N },
            { Mnemonic.XORLW,  FlagM.Z | FlagM.N },
            { Mnemonic.XORWF,  FlagM.Z | FlagM.N },
            { Mnemonic.RLCF,   FlagM.C | FlagM.Z | FlagM.N },
            { Mnemonic.RRCF,   FlagM.C | FlagM.Z | FlagM.N },
            { Mnemonic.ADDLW,  FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Mnemonic.ADDWF,  FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Mnemonic.ADDWFC, FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Mnemonic.DECF,   FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Mnemonic.INCF,   FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Mnemonic.NEGF,   FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Mnemonic.RESET,  FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Mnemonic.SUBFWB, FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Mnemonic.SUBLW,  FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Mnemonic.SUBWF,  FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Mnemonic.SUBWFB, FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
        };

        private static Dictionary<Mnemonic, FlagM> useCC = new Dictionary<Mnemonic, FlagM>()
        {
                { Mnemonic.ADDWFC, FlagM.C },
                { Mnemonic.BC,     FlagM.C },
                { Mnemonic.BNC,    FlagM.C },
                { Mnemonic.RLCF,   FlagM.C },
                { Mnemonic.RRCF,   FlagM.C },
                { Mnemonic.SUBFWB, FlagM.C },
                { Mnemonic.SUBWFB, FlagM.C },
                { Mnemonic.DAW,    FlagM.C | FlagM.DC },
                { Mnemonic.BN,     FlagM.N },
                { Mnemonic.BNN,    FlagM.N },
                { Mnemonic.BNZ,    FlagM.Z },
                { Mnemonic.BZ,     FlagM.Z },
                { Mnemonic.BNOV,   FlagM.OV },
                { Mnemonic.BOV,    FlagM.OV },
                { Mnemonic.RETURN, FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
                { Mnemonic.RETFIE, FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
        };

        /// <summary>
        /// Gets the condition code flags altered by the given opcode.
        /// </summary>
        /// <param name="opc">The opcode.</param>
        public static FlagM Defined(Mnemonic opc)
        {
            if (defCC.TryGetValue(opc, out FlagM flg))
                return flg;
            return 0;
        }

        /// <summary>
        /// Gets the condition code flags used by the given opcode.
        /// </summary>
        /// <param name="opc">The opcode.</param>
        public static FlagM Used(Mnemonic opc)
        {
            if (useCC.TryGetValue(opc, out FlagM flg))
                return flg;
            return 0;
        }
    }

}
