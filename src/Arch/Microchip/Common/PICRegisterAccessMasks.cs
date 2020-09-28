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
    /// This class provides read/write/reset masks values for accessing a PIC register.
    /// </summary>
    public class PICRegisterAccessMasks
    {
        public readonly uint ImplementedMask;
        public readonly uint ResetValue;
        public readonly uint ReadOrMask;
        public readonly uint ReadAndMask;
        public readonly uint WriteOrMask;
        public readonly uint WriteAndMask;

        private PICRegisterAccessMasks(PICRegisterTraits traits)
        {
            string sPOR = traits?.POR ?? new string('0', traits.BitWidth);
            string sAccess = traits?.Access ?? new string('n', traits.BitWidth);

            ResetValue = 0;
            foreach (var c in sPOR)
            {
                ResetValue <<= 1;
                ResetValue |= (c == '1' ? 1U : 0U);
            }

            ulong fullimpl = ((1UL << traits.BitWidth) - 1UL) & traits.Impl;
            ReadOrMask = 0;
            ReadAndMask = (uint)fullimpl;
            WriteOrMask = 0;
            WriteAndMask = (uint)fullimpl;

            int bitno = traits.BitWidth-1;
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
                    case 'c': // Bit is clearable-only
                    case 's': // Bit is setable-only
                        break;

                }
                bitno--;
            }

            ImplementedMask = (uint)fullimpl;
            ResetValue &= ImplementedMask;
        }

        public static PICRegisterAccessMasks Create(PICRegisterTraits traits)
        {
            return new PICRegisterAccessMasks(traits ?? throw new ArgumentNullException(nameof(traits)));
        }
    }

}
