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
    public class BankedAddress : PICBankedAddress
    {

        #region Locals

        /// <summary>
        /// True if address is pointing to Access RAM, false if not.
        /// </summary>
        public readonly bool IsAccessRAM;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bankSelect">The bank select.</param>
        /// <param name="bankOffset">The bank offset.</param>
        /// <param name="access">(Optional) True if address is to access RAM.</param>
        public BankedAddress(byte? bankSelect, ushort bankOffset, bool access = false)
        {
            IsAccessRAM = access;
            BankSelect = (access ? bankSelect : null);
            BankOffset = bankOffset;
        }

        /// <summary>
        /// Default constructor. Instantiates an access RAM slot 0 address.
        /// </summary>
        public BankedAddress()
            : this(null, 0, true)
        {
        }

        /// <summary>
        /// Default constructor. Instantiates an access RAM address.
        /// </summary>
        /// <param name="bankOffset">The offset in the access RAM.</param>
        public BankedAddress(ushort bankOffset)
            : this(null, bankOffset, true)
        {
        }

        #endregion

        /// <summary>
        /// Gets the data memory bank size (in number of bytes) for PIC18 family.
        /// </summary>
        public override ushort BankSize => 256;

        #region Overridden 'Address' methods

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
            if (BankSelect.HasValue)
                return $"{prefix}Bank{BankSelect.Value:X2}_{BankOffset:X2}{suffix}";
            return $"{prefix}RAM_{BankOffset:X2}{suffix}";
        }

        public override Constant ToConstant()
            => Constant.UInt32(ToUInt32());

        public override ushort ToUInt16()
        {
            if (BankSelect.HasValue && !IsAccessRAM)
                return (ushort)(BankSelect.Value * BankSize + BankOffset);
            return BankOffset;
        }

        public override uint ToUInt32()
        {
            if (BankSelect.HasValue && !IsAccessRAM)
                return (uint)(BankSelect.Value * BankSize + BankOffset);
            return BankOffset;
        }

        public override ulong ToLinear()
        {
            if (BankSelect.HasValue && !IsAccessRAM)
                return (ulong)(BankSelect.Value * BankSize + BankOffset);
            return BankOffset;
        }

        public override Address NewOffset(ulong offset)
        {
            if (BankSelect.HasValue)
                return new BankedAddress(BankSelect.Value, (ushort)offset, IsAccessRAM);
            return new BankedAddress(BankOffset);
        }

        public override Address Add(long offset)
        {
            // Simply rollover offset in same memory bank.
            ushort newOff = (ushort)((BankOffset + offset) & (BankSize - 1));
            return new BankedAddress(BankSelect, newOff, IsAccessRAM);
        }

        public override Expression CloneExpression()
            => new BankedAddress(BankSelect, BankOffset, IsAccessRAM);

        public override Address Align(int alignment)
            => new BankedAddress(BankSelect, ((ushort)(alignment * ((BankOffset + alignment - 1) / alignment))), IsAccessRAM);

        #endregion

    }

}
