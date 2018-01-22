#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Types;

namespace Reko.Arch.Microchip.PIC18
{
    /// <summary>
    /// The PIC18 Banked Data memory address.
    /// </summary>
    public class PIC18BankedAddress : Address
    {

        #region Locals

        /// <summary>
        /// The memory bank select index; null if Access RAM.
        /// </summary>
        public readonly byte? BankSelect;

        /// <summary>
        /// The memory bank offset.
        /// </summary>
        public readonly ushort BankOffset;

        public readonly bool IsAccessRAM;

        #endregion

        #region Constructors

        public PIC18BankedAddress()
            : base(PrimitiveType.Ptr16)
        {
            BankSelect = null;
            BankOffset = 0;
            IsAccessRAM = true;
        }

        public PIC18BankedAddress(ushort bankOffset)
            : base(PrimitiveType.Ptr16)
        {
            BankSelect = null;
            BankOffset = bankOffset;
            IsAccessRAM = true;
        }

        public PIC18BankedAddress(byte? bankSelect, ushort bankOffset, bool access = false)
            : base(PrimitiveType.Ptr16)
        {
            IsAccessRAM = access;
            BankSelect = (access ? bankSelect : null);
            BankOffset = bankOffset;
        }

        #endregion

        /// <summary>
        /// Gets the data memory bank size for PIC18 family.
        /// </summary>
        /// <value>
        /// The size in bytes of the memory bank.
        /// </value>
        public ushort BankSize => 256;

        #region Overridden 'Address' methods

        public override bool IsNull => false;

        public override ulong Offset => BankOffset;

        public override ushort? Selector => BankSelect;

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

        #endregion

        #region Methods

        public override Address NewOffset(ulong offset)
        {
            if (BankSelect.HasValue)
                return new PIC18BankedAddress(BankSelect.Value, (ushort)offset, IsAccessRAM);
            return new PIC18BankedAddress(BankOffset);
        }

        public override Address Add(long offset)
        {
            ushort newOff = (ushort)(BankOffset + offset);
            return new PIC18BankedAddress(BankSelect, newOff, IsAccessRAM);
        }

        public override Expression CloneExpression()
            => new PIC18BankedAddress(BankSelect, BankOffset, IsAccessRAM);

        public override Address Align(int alignment)
            => new PIC18BankedAddress(BankSelect, ((byte)(alignment * ((BankOffset + alignment - 1) / alignment))), IsAccessRAM);

        #endregion

    }

}
