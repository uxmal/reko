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

        public const uint MAXPROGBYTADDR = 0x1FFFFFU;

        private Constant Value;

        public static readonly PICProgAddress Invalid = new PICProgAddress(Constant.Invalid);


        public PICProgAddress(uint addr) : base(PrimitiveType.Ptr32)
        {
            Value = Constant.Create(DataType, addr & MAXPROGBYTADDR);
        }

        public PICProgAddress(Constant addr) : base(addr.DataType)
        {
            Value = addr;
        }


        #region Properties

        public override bool IsNull => false;
        public override ulong Offset => Value.ToUInt32(); 
        public override ushort? Selector => null;

        #endregion

        #region Methods

        public override Address Add(long offset)
            => new PICProgAddress((uint)(Value.ToUInt32() + offset));

        public override Address Align(int alignment)
            => new PICProgAddress((uint)(alignment * ((Value.ToUInt32() + alignment - 1) / alignment)));

        public override Expression CloneExpression()
            => new PICProgAddress(Value);

        public override string GenerateName(string prefix, string suffix)
            => $"{prefix}{Value:X6}{suffix}";

        public override Address NewOffset(ulong offset)
            => new PICProgAddress((uint)offset);

        public override Constant ToConstant()
            => Value;

        public override ushort ToUInt16()
            => throw new InvalidOperationException("Returning UInt16 would lose precision.");

        public override uint ToUInt32()
            => Value.ToUInt32();

        public override ulong ToLinear()
            => Value.ToUInt32();

        public override string ToString()
            => $"{Value:X6}";

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

        public const uint MAXDATABYTADDR = 0x3FFF;

        private Constant Value;

        public static readonly PICDataAddress Invalid = new PICDataAddress(Constant.Invalid);

        public PICDataAddress(uint addr) : base(PrimitiveType.Ptr16)
        {
            var uValue = (ushort)(addr & MAXDATABYTADDR);
            Value = Constant.Create(DataType, uValue);
        }

        public PICDataAddress(Constant addr) : base(addr.DataType)
        {
            Value = addr;
        }

        #region Properties


        public override bool IsNull => false;
        public override ulong Offset => ToLinear(); 
        public override ushort? Selector => null;

        public bool IsValid => Value.IsValid;

        #endregion

        #region Methods

        public override Address Add(long offset)
            => new PICDataAddress((uint)(Value.ToUInt16() + offset));

        public override Address Align(int alignment)
            => new PICDataAddress((uint)(alignment * ((Value.ToUInt16() + alignment - 1) / alignment)));

        public override Expression CloneExpression()
            => new PICDataAddress(Value);

        public override string GenerateName(string prefix, string suffix)
            => $"{prefix}{Value:X4}{suffix}";

        public override Address NewOffset(ulong offset)
            => new PICDataAddress((uint)offset);

        public override Constant ToConstant()
            => Value;

        public override ushort ToUInt16()
            => Value.ToUInt16();

        public override uint ToUInt32()
            => Value.ToUInt32();

        public override ulong ToLinear()
            => Value.ToUInt32();

        public override string ToString()
            => $"0x{Value.ToUInt16():X4}";

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

        public PICBankedAddress() : base(PrimitiveType.Ptr16)
        {
        }

        /// <summary>
        /// Gets the data memory bank selector value.
        /// </summary>
        public byte BankSelect { get; protected set; }

        /// <summary>
        /// Gets the offset in the data memory bank..
        /// </summary>
        public ushort BankOffset { get; protected set; }


        // Overridden 'Address' properties/methods

        public override bool IsNull => false;

        public override ulong Offset => BankOffset;

        public override ushort? Selector => BankSelect;

        public override bool IsZero => false;

        public override Constant ToConstant() => throw new NotImplementedException();
        public override ushort ToUInt16() => throw new NotImplementedException();
        public override uint ToUInt32() => throw new NotImplementedException();
        public override ulong ToLinear() => throw new NotImplementedException();
        public override Address Add(long offset) => throw new NotImplementedException();
        public override Address Align(int alignment) => throw new NotImplementedException();
        public override string GenerateName(string prefix, string suffix) => throw new NotImplementedException();
        public override Expression CloneExpression() => throw new NotImplementedException();
        public override Address NewOffset(ulong offset) => throw new NotImplementedException();
        public override Expression Invert() => base.Invert();

    }

}
