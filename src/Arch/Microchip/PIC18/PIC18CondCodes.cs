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

namespace Reko.Arch.MicrochipPIC.PIC18
{
    using Common;

    /// <summary>
    /// Class defining PIC18 instructions' usage and alteration of ALU condition flags.
    /// </summary>
    public static class PIC18CC
    {
        private static Dictionary<Opcode, FlagM> defCC = new Dictionary<Opcode, FlagM>()
        {
            { Opcode.DAW,    FlagM.C },
            { Opcode.CLRF,   FlagM.Z },
            { Opcode.ANDLW,  FlagM.Z | FlagM.N },
            { Opcode.ANDWF,  FlagM.Z | FlagM.N },
            { Opcode.COMF,   FlagM.Z | FlagM.N },
            { Opcode.IORLW,  FlagM.Z | FlagM.N },
            { Opcode.IORWF,  FlagM.Z | FlagM.N },
            { Opcode.MOVF,   FlagM.Z | FlagM.N },
            { Opcode.RLNCF,  FlagM.Z | FlagM.N },
            { Opcode.RRNCF,  FlagM.Z | FlagM.N },
            { Opcode.XORLW,  FlagM.Z | FlagM.N },
            { Opcode.XORWF,  FlagM.Z | FlagM.N },
            { Opcode.RLCF,   FlagM.C | FlagM.Z | FlagM.N },
            { Opcode.RRCF,   FlagM.C | FlagM.Z | FlagM.N },
            { Opcode.ADDLW,  FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Opcode.ADDWF,  FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Opcode.ADDWFC, FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Opcode.DECF,   FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Opcode.INCF,   FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Opcode.NEGF,   FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Opcode.RESET,  FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Opcode.SUBFWB, FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Opcode.SUBLW,  FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Opcode.SUBWF,  FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
            { Opcode.SUBWFB, FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
        };

        private static Dictionary<Opcode, FlagM> useCC = new Dictionary<Opcode, FlagM>()
        {
                { Opcode.ADDWFC, FlagM.C },
                { Opcode.BC,     FlagM.C },
                { Opcode.BNC,    FlagM.C },
                { Opcode.RLCF,   FlagM.C },
                { Opcode.RRCF,   FlagM.C },
                { Opcode.SUBFWB, FlagM.C },
                { Opcode.SUBWFB, FlagM.C },
                { Opcode.DAW,    FlagM.C | FlagM.DC },
                { Opcode.BN,     FlagM.N },
                { Opcode.BNN,    FlagM.N },
                { Opcode.BNZ,    FlagM.Z },
                { Opcode.BZ,     FlagM.Z },
                { Opcode.BNOV,   FlagM.OV },
                { Opcode.BOV,    FlagM.OV },
                { Opcode.RETURN, FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
                { Opcode.RETFIE, FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N },
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
