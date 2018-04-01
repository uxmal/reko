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

namespace Reko.Arch.Microchip.PIC18
{

    /// <summary>
    /// The PIC18 Banked Data memory address with Access RAM.
    /// </summary>
    public class PIC18BankedAddress : PICBankedAddress
    {

        /// <summary>
        /// True if address is pointing to Access RAM, false if not.
        /// </summary>
        public readonly bool IsAccessRAM;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bankSelect">The bank select.</param>
        /// <param name="bankOffset">The bank offset.</param>
        /// <param name="access">(Optional) True if address is to access RAM.</param>
        public PIC18BankedAddress(byte bankSelect, ushort bankOffset, bool access = false)
        {
            IsAccessRAM = access;
            BankSelect = bankSelect;
            BankOffset = (ushort)(bankOffset & 0xFF);
        }

        /// <summary>
        /// Constructor for an Access RAM address.
        /// </summary>
        /// <param name="bankOffset">The bank offset in the Access RAM bank.</param>
        public PIC18BankedAddress(ushort bankOffset)
        {
            IsAccessRAM = true;
            BankSelect = 0;
            BankOffset = (ushort)(bankOffset & 0xFF);
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
        {
            if (IsAccessRAM)
                return $"{prefix}AccessRAM{BankOffset:X2}{suffix}";
            return $"{prefix}Bank{BankSelect:X2}_{BankOffset:X2}{suffix}";
        }

        public override Constant ToConstant()
            => Constant.UInt32(ToUInt32());

        public override ushort ToUInt16()
            => (IsAccessRAM ? BankOffset : (ushort)((BankSelect << 8) + BankOffset));

        public override uint ToUInt32()
            => (IsAccessRAM ? BankOffset : (uint)((BankSelect << 8) + BankOffset));

        public override ulong ToLinear()
            => (IsAccessRAM ? BankOffset : (ulong)((BankSelect << 8) + BankOffset));

        public override Address NewOffset(ulong offset)
            => new PIC18BankedAddress(BankSelect, (ushort)offset, IsAccessRAM);

        public override Address Add(long offset)
            => new PIC18BankedAddress(BankSelect, (ushort)((BankOffset + offset) & 0xFF), IsAccessRAM);

        public override Expression CloneExpression()
            => new PIC18BankedAddress(BankSelect, BankOffset, IsAccessRAM);

        public override Address Align(int alignment)
            => new PIC18BankedAddress(BankSelect, ((ushort)(alignment * ((BankOffset + alignment - 1) / alignment))), IsAccessRAM);

    }

}
