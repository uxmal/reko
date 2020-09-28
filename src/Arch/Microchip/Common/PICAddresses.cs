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

using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Expressions;
using System;

namespace Reko.Arch.MicrochipPIC.Common
{

    /// <summary>
    /// A PIC 21-bit program address (word-aligned).
    /// </summary>
    public class PICProgAddress : Address
    {

        public const uint MAXPROGBYTADDR = 0x3FFFFFu;
        public readonly Constant Value;
        public static readonly PICProgAddress Invalid = new PICProgAddress(Constant.Invalid);

        public PICProgAddress(uint addr) : base(PrimitiveType.Ptr32)
        {
            Value = Constant.Create(DataType, addr & MAXPROGBYTADDR);
        }

        public PICProgAddress(Constant addr) : base(addr.DataType)
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
            => throw new InvalidOperationException("Returning UInt16 would lose precision.");

        public override uint ToUInt32()
            => Value.ToUInt32();

        public override ulong ToLinear()
            => Value.ToUInt32();

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
            => $"{ToLinear():X6}";

    }

    /// <summary>
    /// A PIC 12/14-bit data address.
    /// </summary>
    public class PICDataAddress : Address
    {

        public const uint MAXDATABYTADDR = 0x3FFFu;
        public readonly Constant Value;
        public static readonly PICDataAddress Invalid = new PICDataAddress(Constant.Invalid);


        public PICDataAddress(uint addr) : base(PrimitiveType.Ptr16)
        {
            Value = Constant.Create(DataType, addr & MAXDATABYTADDR);
        }

        public PICDataAddress(Constant addr) : base(addr.DataType)
        {
            Value = addr;
        }

        public bool IsValid => Value.IsValid;
        public override bool IsNull => false;
        public override ulong Offset => Value.ToUInt16();
        public override ushort? Selector => null;

        public override Constant ToConstant()
            => Value;

        public override ushort ToUInt16()
            => Value.ToUInt16();

        public override uint ToUInt32()
            => Value.ToUInt16();

        public override ulong ToLinear()
            => Value.ToUInt16();

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
    public abstract class PICBankedAddress
    {

        public PICBankedAddress(Constant bankSel, Constant bankOff, bool access = false)
        {
            BankSelect = bankSel;
            BankOffset = bankOff;
            IsAccessRAMAddr = access;
        }

        /// <summary>
        /// Gets the data memory bank selector value.
        /// </summary>
        public Constant BankSelect { get; }

        /// <summary>
        /// Gets the data memory bank selector value.
        /// </summary>
        public Constant BankOffset { get; }

        /// <summary>
        /// Gets a value indicating whether this address is an Access RAM address.
        /// </summary>
        public bool IsAccessRAMAddr { get; }

        public abstract int BankWidth { get; }

        public PICDataAddress ToDataAddress(IMemoryRegion region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));
            var baseaddr = region.PhysicalByteAddrRange.Begin.ToUInt32() & ~((1 << BankWidth) - 1);
            return new PICDataAddress((uint)(baseaddr + BankOffset.ToUInt32()));
        }

    }

    /// <summary>
    /// The PIC16 Banked Data memory address.
    /// </summary>
    public class PIC16BankedAddress : PICBankedAddress
    {

        public const int DataBankWidth = 7;

        public PIC16BankedAddress(Constant bankSelect, Constant bankOffset)
            : base(bankSelect, bankOffset, false)
        {
        }

        public override int BankWidth => DataBankWidth;

    }

    /// <summary>
    /// The PIC18 Banked Data memory address with Access RAM.
    /// </summary>
    public class PIC18BankedAddress : PICBankedAddress
    {

        public const int DataBankWidth = 8;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bankSelect">The bank select.</param>
        /// <param name="bankOffset">The bank offset.</param>
        /// <param name="access">(Optional) True if address is to Access RAM.</param>
        public PIC18BankedAddress(Constant bankSelect, Constant bankOffset, bool access = false)
            : base(bankSelect, bankOffset, access)
        {
        }

        public override int BankWidth => DataBankWidth;



    }

}
