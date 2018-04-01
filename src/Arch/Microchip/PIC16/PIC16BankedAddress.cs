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

using Reko.Arch.Microchip.Common;
using Reko.Core;
using Reko.Core.Expressions;

namespace Reko.Arch.Microchip.PIC16
{
    /// <summary>
    /// The PIC16 Banked Data memory address.
    /// </summary>
    public class PIC16BankedAddress : PICBankedAddress
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bankSelect">The bank select.</param>
        /// <param name="bankOffset">The bank offset.</param>
        public PIC16BankedAddress(byte bankSelect, ushort bankOffset)
        {
            BankSelect = bankSelect;
            BankOffset = (ushort)(bankOffset & 0x7F);
        }


        // Overridden 'Address' methods

        /// <summary>
        /// Generates a symbolic name with given prefix and suffix.
        /// </summary>
        /// <param name="prefix">The prefix to use.</param>
        /// <param name="suffix">The suffix to use.</param>
        /// <returns>
        /// The name as a string.
        /// </returns>
        public override string GenerateName(string prefix, string suffix)
            => $"{prefix}Bank{BankSelect:X2}_{BankOffset:X2}{suffix}";

        public override Constant ToConstant()
            => Constant.UInt32(ToUInt32());

        public override ushort ToUInt16()
            => (ushort)((BankSelect << 7) + BankOffset);

        public override uint ToUInt32()
            => (uint)((BankSelect << 7) + BankOffset);

        public override ulong ToLinear()
            => (ulong)((BankSelect << 7) + BankOffset);

        public override Address NewOffset(ulong offset)
            => new PIC16BankedAddress(BankSelect, (ushort)offset);

        public override Address Add(long offset)
            => new PIC16BankedAddress(BankSelect, (ushort)((BankOffset + offset) & 0x7F));

        public override Expression CloneExpression()
            => new PIC16BankedAddress(BankSelect, BankOffset);

        public override Address Align(int alignment)
            => new PIC16BankedAddress(BankSelect, ((ushort)(alignment * ((BankOffset + alignment - 1) / alignment))));

    }

}
