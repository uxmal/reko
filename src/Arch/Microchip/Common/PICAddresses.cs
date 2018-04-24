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

    public class PICAddress : Address
    {
        public readonly Constant Value;
        public static readonly PICAddress Invalid = new PICAddress(Constant.Invalid);

        public PICAddress(uint addr, PrimitiveType dt) : base(dt)
        {
            Value = Constant.Create(DataType, addr);
        }

        public PICAddress(Constant addr) : base(addr.DataType)
        {
            Value = addr;
        }

        public bool IsValid => Value.IsValid;

        public override bool IsNull => false;
        public override ulong Offset => Value.ToUInt32();
        public override ushort? Selector => null;

        public override Constant ToConstant()
            => Value;

        public override ushort ToUInt16()
            => Value.ToUInt16();

        public override uint ToUInt32()
            => Value.ToUInt32();

        public override ulong ToLinear()
            => Value.ToUInt32();

        public override Address Add(long offset) => throw new NotImplementedException();
        public override Address Align(int alignment) => throw new NotImplementedException();
        public override Expression CloneExpression() => throw new NotImplementedException();
        public override string GenerateName(string prefix, string suffix) => throw new NotImplementedException();
        public override Address NewOffset(ulong offset) => throw new NotImplementedException();


    }

    /// <summary>
    /// A PIC 21-bit program address.
    /// </summary>
    public class PICProgAddress : PICAddress
    {

        public const uint MAXPROGBYTADDR = 0x1FFFFFU;

        public PICProgAddress(uint addr) : base(addr & MAXPROGBYTADDR, PrimitiveType.Ptr32)
        {
        }

        public PICProgAddress(Constant addr) : base(addr)
        {
        }


        public override Address Add(long offset)
            => new PICProgAddress((uint)(Value.ToUInt32() + offset));

        public override Address Align(int alignment)
            => new PICProgAddress((uint)(alignment * ((Value.ToUInt32() + alignment - 1) / alignment)));

        public override Expression CloneExpression()
            => new PICProgAddress(Value);

        public override string GenerateName(string prefix, string suffix)
            => $"{prefix}{Value.ToUInt32():X6}{suffix}";

        public override Address NewOffset(ulong offset)
            => new PICProgAddress((uint)offset);

        public override ushort ToUInt16()
            => throw new InvalidOperationException("Returning UInt16 would lose precision.");

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

        public override string ToString()
            => $"{ToLinear():X8}";

    }

    /// <summary>
    /// A PIC 12/14-bit data address.
    /// </summary>
    public class PICDataAddress : PICAddress
    {

        public const uint MAXDATABYTADDR = 0x3FFF;

        public PICDataAddress(uint addr) : base(addr & MAXDATABYTADDR, PrimitiveType.Ptr16)
        {
        }

        public PICDataAddress(Constant addr) : base(addr)
        {
        }


        public override Address Add(long offset)
            => new PICDataAddress((uint)(Value.ToUInt16() + offset));

        public override Address Align(int alignment)
            => new PICDataAddress((uint)(alignment * ((Value.ToUInt16() + alignment - 1) / alignment)));

        public override Expression CloneExpression()
            => new PICDataAddress(Value);

        public override string GenerateName(string prefix, string suffix)
            => $"{prefix}{Value.ToUInt16():X4}{suffix}";

        public override Address NewOffset(ulong offset)
            => new PICDataAddress((uint)offset);

        public static PICDataAddress Ptr(uint addr)
            => new PICDataAddress(addr);

        public static PICDataAddress Ptr(Address aaddr)
            => new PICDataAddress(aaddr.ToUInt32());

        public override string ToString()
            => $"{ToLinear():X4}";


    }

    /// <summary>
    /// A PIC banked data memory address. Bank size depends on PIC family. Class must be inherited.
    /// </summary>
    public abstract class PICBankedAddress : PICAddress
    {

        public PICBankedAddress(byte bankSel, ushort bankOff, bool access = false)
            : base(bankOff, PrimitiveType.Ptr16)
        {
            BankSelect = bankSel;
            IsAccessRAM = access;
        }

        /// <summary>
        /// Gets the data memory bank selector value.
        /// </summary>
        public byte BankSelect { get; }

        /// <summary>
        /// Gets a value indicating whether this address is an Access RAM address.
        /// </summary>
        public bool IsAccessRAM { get; }

        public abstract int BankWidth { get; }

        // Overridden 'Address' properties/methods

        public override ushort? Selector => BankSelect;

        public override bool IsZero => false;

        public override Constant ToConstant()
            => Constant.UInt32(ToUInt32());

        public override ushort ToUInt16()
            => (ushort)(IsAccessRAM ? (ushort)Offset : (BankSelect << BankWidth) + (int)Offset);

        public override uint ToUInt32()
            => (uint)(IsAccessRAM ? (uint)Offset : (uint)((BankSelect << BankWidth) + (int)Offset));

        public override ulong ToLinear()
            => (IsAccessRAM ? Offset : (ulong)((BankSelect << BankWidth) + (int)Offset));

        /// <summary>
        /// Generates a symbolic name with given prefix and suffix.
        /// </summary>
        /// <param name="prefix">The prefix to use.</param>
        /// <param name="suffix">The suffix to use.</param>
        /// <returns>
        /// The symbolic name as a string.
        /// </returns>
        public override string GenerateName(string prefix, string suffix)
            => (IsAccessRAM ?
                    $"{prefix}AccessRAM{Offset:X2}{suffix}" : 
                    $"{prefix}Bank{BankSelect:X2}_{Offset:X2}{suffix}");

        /// <summary>
        /// Applies logical (not-bitwise) negation to the expression.
        /// </summary>
        public override Expression Invert()
            => base.Invert();

        public override Expression CloneExpression() => throw new NotImplementedException();
        public override Address NewOffset(ulong offset) => throw new NotImplementedException();
        public override Address Add(long offset) => throw new NotImplementedException();
        public override Address Align(int alignment) => throw new NotImplementedException();

    }

    /// <summary>
    /// The PIC16 Banked Data memory address.
    /// </summary>
    public class PIC16BankedAddress : PICBankedAddress
    {

        public PIC16BankedAddress(byte bankSelect, ushort bankOffset)
            : base(bankSelect, bankOffset)
        {
        }

        public override int BankWidth => 7;

        // Overridden 'Address' methods

        public override Address NewOffset(ulong offset)
            => new PIC16BankedAddress(BankSelect, (ushort)offset);

        public override Address Add(long offset)
            => new PIC16BankedAddress(BankSelect, (ushort)((long)Offset + offset));

        public override Expression CloneExpression()
            => new PIC16BankedAddress(BankSelect, (ushort)Offset);

        public override Address Align(int alignment)
            => new PIC16BankedAddress(BankSelect, ((ushort)(alignment * (((int)Offset + alignment - 1) / alignment))));

    }

    /// <summary>
    /// The PIC18 Banked Data memory address with Access RAM.
    /// </summary>
    public class PIC18BankedAddress : PICBankedAddress
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bankSelect">The bank select.</param>
        /// <param name="bankOffset">The bank offset.</param>
        /// <param name="access">(Optional) True if address is to Access RAM.</param>
        public PIC18BankedAddress(byte bankSelect, ushort bankOffset, bool access = false)
            : base(bankSelect, bankOffset, access)
        {
        }

        /// <summary>
        /// Constructor for an Access RAM address.
        /// </summary>
        /// <param name="bankOffset">The bank offset in the Access RAM bank.</param>
        public PIC18BankedAddress(ushort bankOffset)
            : this(0, bankOffset, true)
        {
        }

        public override int BankWidth => 8;

        // Overridden 'Address' methods

        public override Address NewOffset(ulong offset)
            => new PIC18BankedAddress(BankSelect, (ushort)offset, IsAccessRAM);

        public override Address Add(long offset)
            => new PIC18BankedAddress(BankSelect, (ushort)((long)Offset + offset), IsAccessRAM);

        public override Expression CloneExpression()
            => new PIC18BankedAddress(BankSelect, (ushort)Offset, IsAccessRAM);

        public override Address Align(int alignment)
            => new PIC18BankedAddress(BankSelect, ((ushort)(alignment * (((int)Offset + alignment - 1) / alignment))), IsAccessRAM);

    }

}
