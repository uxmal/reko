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
    //        "w" -> write-only
    //        "0" -> 0-value
    //        "1" -> 1-value
    //        "c" -> clear-able only
    //        "s" -> settable only
    //        "-" -> not implemented
    //
    //    PIC registers Power-On/Master-Reset bits' state are coded as-follows:
    //
    //        "0" -> reset to 0.
    //        "1" -> reset to 1.
    //        "u" -> unchanged by reset.
    //        "x" -> unknown after reset.
    //        "q" -> depends on condition.
    //        "-" -> no implemented.
    //



    /// <summary>
    /// This class provides read and write methods to take into account the PIC register's traits.
    /// </summary>
    public class PICRegisterBinaryTraits
    {
        private class RegisterAccessBits
        {
            public readonly uint ImplementedMask;
            public readonly uint ResetValue;
            public readonly uint ReadOrMask;
            public readonly uint ReadAndMask;
            public readonly uint WriteOrMask;
            public readonly uint WriteAndMask;

            public RegisterAccessBits(PICRegisterTraits traits)
            {
                string sPOR = traits?.POR ?? throw new ArgumentNullException(nameof(traits.POR));
                string sAccess = traits?.Access ?? throw new ArgumentNullException(nameof(traits.Access));

                ResetValue = 0;
                foreach (var c in sPOR)
                {
                    ResetValue <<= 1;
                    ResetValue = (c == '1' ? 1U : 0U);
                }

                int bitno = 0;
                uint fullimpl = (1U << traits.BitWidth) - 1U;
                ReadOrMask = 0;
                ReadAndMask = ~ReadOrMask;
                WriteOrMask = 0;
                WriteAndMask = ~WriteOrMask;
                foreach (char a in sAccess)
                {
                    uint orMask = 1U << bitno;
                    uint andMask = ~orMask;
                    switch (a)
                    {
                        case '0': // Bit is always 0.
                            ReadAndMask &= andMask;
                            WriteAndMask &= andMask;
                            break;

                        case '1': // Bit is always 1.
                            ReadOrMask |= orMask;
                            WriteOrMask |= orMask;
                            break;

                        case '-': // Bit is not implemented. Read as 0.
                            ReadAndMask &= andMask;
                            fullimpl &= andMask;
                            break;

                        case 'r': // Bit is readonly. Write as 0.
                            WriteAndMask &= andMask;
                            break;

                        case 'w': // Bit is write-only. Read as 0.
                            ReadAndMask &= andMask;
                            break;

                        case 'x': // Bit is read-write
                        case 'X':
                        case 'n':
                            break;

                        case 'c': // Bit is clearable-only
                            WriteAndMask &= andMask;
                            break;

                        case 's': // Bit is setable-only
                            WriteOrMask |= orMask;
                            break;
                    }
                    bitno++;
                }

                ImplementedMask = fullimpl;
                ResetValue &= ImplementedMask;
            }
        }

        private readonly PICRegisterTraits traits;

        public PICRegisterBinaryTraits(PICRegisterTraits traits)
        {
            this.traits = traits ?? throw new ArgumentNullException(nameof(traits));
        }

        private RegisterAccessBits Bits
        {
            get
            {
                if (bits == null)
                {
                    bits = new RegisterAccessBits(traits);
                }
                return bits;
            }
        }
        private RegisterAccessBits bits;

        public uint ReadRegister(uint rawvalue)
        {
            return ((rawvalue | bits.ReadOrMask) & bits.ReadAndMask) & bits.ImplementedMask;
        }

        public uint WriteRegister(uint rawvalue)
        {
            return ((rawvalue & bits.WriteAndMask) | bits.WriteOrMask) & bits.ImplementedMask;
        }

    }

}
