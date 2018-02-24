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
using Reko.Core.Types;
using Reko.Core.Expressions;
using System;

namespace Reko.Arch.Microchip.Common
{

    /// <summary>
    /// A PIC 21-bit program address.
    /// </summary>
    public class PICProgAddress : Address
    {

        #region Static and Constant fields

        public const uint MAXPROGBYTADDR = 0x1FFFFFU;

        #endregion

        #region Members fields

        private uint uValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="addr">The program byte address.</param>
        public PICProgAddress(uint addr) : base(PrimitiveType.Ptr32)
        {
            uValue = addr & MAXPROGBYTADDR;
        }

        #endregion

        #region Properties

        public override bool IsNull => false;
        public override ulong Offset => uValue; 
        public override ushort? Selector => null;

        #endregion

        #region Methods

        public override Address Add(long offset)
            => new PICProgAddress((uint)(uValue + offset));

        public override Address Align(int alignment)
            => new PICProgAddress((uint)(alignment * ((uValue + alignment - 1) / alignment)));

        public override Expression CloneExpression()
            => new PICProgAddress(uValue);

        public override string GenerateName(string prefix, string suffix)
            => $"{prefix}{uValue:X6}{suffix}";

        public override Address NewOffset(ulong offset)
            => new PICProgAddress((uint)offset);

        public override Constant ToConstant()
            => Constant.UInt32(uValue);

        public override ushort ToUInt16()
            => throw new InvalidOperationException("Returning UInt16 would lose precision.");

        public override uint ToUInt32()
            => uValue;

        public override ulong ToLinear()
            => uValue;

        public override string ToString()
            => $"{uValue:X6}";

        /// <summary>
        /// Create a <see cref="PICProgAddress"/> instance with specified byte address.
        /// </summary>
        /// <param name="addr">The program byte address as an integer.</param>
        /// <returns>
        /// The <see cref="PICProgAddress"/>.
        /// </returns>
        public static PICProgAddress Ptr(uint addr)
            => new PICProgAddress(addr);

        /// <summary>
        /// Create a <see cref="PICProgAddress"/> instance with specified address.
        /// </summary>
        /// <param name="aaddr">The address.</param>
        /// <returns>
        /// The <see cref="PICProgAddress"/>.
        /// </returns>
        public static PICProgAddress Ptr(Address aaddr)
            => new PICProgAddress(aaddr.ToUInt32());

        #endregion
    }

    /// <summary>
    /// A PIC 12/14-bit data address.
    /// </summary>
    public class PICDataAddress : Address
    {

        #region Static and Constant fields

        public const uint MAXDATABYTADDR = 0x3FFF;

        #endregion

        #region Members fields

        private ushort uValue;

        #endregion

        #region Constructors

        public PICDataAddress(uint addr) : base(PrimitiveType.Ptr16)
        {
            uValue = (ushort)(addr & MAXDATABYTADDR);
        }

        #endregion

        #region Properties

        public override bool IsNull => false;
        public override ulong Offset => uValue; 
        public override ushort? Selector => null;

        #endregion

        #region Methods

        public override Address Add(long offset)
            => new PICProgAddress((uint)(uValue + offset));

        public override Address Align(int alignment)
            => new PICProgAddress((uint)(alignment * ((uValue + alignment - 1) / alignment)));

        public override Expression CloneExpression()
            => new PICProgAddress(uValue);

        public override string GenerateName(string prefix, string suffix)
            => $"{prefix}{uValue:X4}{suffix}";

        public override Address NewOffset(ulong offset)
            => new PICProgAddress((uint)offset);

        public override Constant ToConstant()
            => Constant.UInt16(uValue);

        public override ushort ToUInt16()
            => uValue;

        public override uint ToUInt32()
            => uValue;

        public override ulong ToLinear()
            => uValue;

        public override string ToString()
            => $"{uValue:X4}";

        public static PICDataAddress Ptr(uint addr)
            => new PICDataAddress(addr);

        public static PICDataAddress Ptr(Address aaddr)
            => new PICDataAddress(aaddr.ToUInt32());

        #endregion

    }

    /// <summary>
    /// A PIC banked data memory address. Bank size depends on PIC family. Class must be inherited.
    /// </summary>
    public abstract class PICBankedAddress : Address
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PICBankedAddress() : base(PrimitiveType.Ptr16)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data memory bank selector value or null..
        /// </summary>
        public byte? BankSelect { get; protected set; }

        /// <summary>
        /// Gets the offset in the data memory bank..
        /// </summary>
        public ushort BankOffset
        {
            get => bankOff; 
            protected set => bankOff = (ushort)(value & (BankSize - 1)); 
        }
        private ushort bankOff;

        #endregion

        #region Overridden 'Address' properties/methods

        #region Properties

        public override bool IsNull => false;

        public override ulong Offset => BankOffset;

        public override ushort? Selector => BankSelect;

        public override bool IsZero => false;

        #endregion

        #region Methods

        public override Constant ToConstant()
            => throw new NotImplementedException();

        public override ushort ToUInt16()
            => throw new NotImplementedException();

        public override uint ToUInt32()
            => throw new NotImplementedException();

        public override ulong ToLinear()
            => throw new NotImplementedException();

        public override Address Add(long offset)
            => throw new NotImplementedException();

        public override Address Align(int alignment)
            => throw new NotImplementedException();

        public override string GenerateName(string prefix, string suffix)
            => throw new NotImplementedException();

        public override Expression CloneExpression()
            => throw new NotImplementedException();

        public override Address NewOffset(ulong offset)
            => throw new NotImplementedException();

        public override Expression Invert()
            => base.Invert();

        #endregion

        #endregion

        #region Abstract methods/properties

        /// <summary>
        /// Gets the size of the data memory bank. Must be a power of 2.
        /// </summary>
        public abstract ushort BankSize { get; }

        #endregion

    }

}
