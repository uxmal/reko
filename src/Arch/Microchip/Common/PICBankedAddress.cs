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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;

namespace Reko.Arch.Microchip.Common
{
    /// <summary>
    /// A PIC banked data memory address.
    /// </summary>
    public abstract class PICBankedAddress : Address
    {

        #region Locals

        public readonly PICRegisterStorage BankSelect;
        public readonly Constant BankOffset;
        protected readonly ushort banknum;
        protected readonly ushort bankoffset;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bnum">The bank number.</param>
        /// <param name="boff">The offset in the bank.</param>
        public PICBankedAddress(byte bnum, byte boff) : base(PrimitiveType.Ptr16)
        {
            banknum = bnum;
            bankoffset = boff;
        }

        #endregion

        #region Overridden 'Address' methods

        public override bool IsNull => false;

        public override ulong Offset
            => bankoffset;

        public override ushort? Selector
            => banknum;

        public override string GenerateName(string prefix, string suffix)
        {
            return $"{prefix}bank{banknum}_{bankoffset:X2}{suffix}";
        }

        public override Constant ToConstant()
            => Constant.UInt32(ToUInt32());

        public override ushort ToUInt16()
            => (ushort)(banknum * BankSize + bankoffset);

        public override uint ToUInt32()
            => (uint)(banknum * BankSize + bankoffset);

        public override ulong ToLinear()
            => (ulong)(banknum * BankSize + bankoffset);

        #endregion

        #region Abstract methods

        public abstract ushort BankSize { get; }

        #endregion

    }

}
