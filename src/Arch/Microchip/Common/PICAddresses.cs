#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2025 John Källén.
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
    public class PICProgAddress // : Address
    {

        public const uint MAXPROGBYTADDR = 0x3FFFFFu;
        public readonly Constant Value;
        public static readonly PICProgAddress Invalid = new PICProgAddress(InvalidConstant.Create(PrimitiveType.Ptr32));
        public static readonly PrimitiveType PointerType = PrimitiveType.Create(Domain.Pointer, 21);

        public PICProgAddress(uint addr)
        {
            DataType = PrimitiveType.Ptr32;
            Value = Constant.Create(DataType, addr & MAXPROGBYTADDR);
        }

        public PICProgAddress(Constant addr)
        {
            DataType = addr.DataType;
            Value = addr;
        }

        public DataType DataType { get; }
        public bool IsValid => Value.IsValid;
        public bool IsNull => false;
        public ulong Offset => Value.ToUInt32();
        public ushort? Selector => null;

        public Constant ToConstant()
            => Value;

        public ushort ToUInt16()
            => throw new InvalidOperationException("Returning UInt16 would lose precision.");

        public uint ToUInt32()
            => Value.ToUInt32();

        public ulong ToLinear()
            => Value.ToUInt32();

        public PICProgAddress Add(long offset)
            => new PICProgAddress((uint)(Value.ToUInt32() + offset));

        public PICProgAddress Align(int alignment)
            => new PICProgAddress((uint)(alignment * ((Value.ToUInt32() + alignment - 1) / alignment)));

        public PICProgAddress CloneExpression()
            => new PICProgAddress(Value);

        public string GenerateName(string prefix, string suffix)
            => $"{prefix}{Value.ToUInt32():X6}{suffix}";

        public  PICProgAddress NewOffset(ulong offset)
            => new PICProgAddress((uint)offset);

        /// <summary>
        /// Create a <see cref="PICProgAddress"/> instance with specified byte address.
        /// </summary>
        /// <param name="addr">The program byte address as an integer.</param>
        /// <returns>
        /// The <see cref="PICProgAddress"/>.
        /// </returns>
        public static Address Ptr(uint addr)
            => Address.Create(PointerType, addr);

        /// <summary>
        /// Create a <see cref="PICProgAddress"/> instance with specified address.
        /// </summary>
        /// <param name="aaddr">The address.</param>
        /// <returns>
        /// The <see cref="PICProgAddress"/>.
        /// </returns>
        public static PICProgAddress Ptr(Address aaddr)
            => new PICProgAddress(aaddr.ToUInt32());

        protected string ConvertToString()
            => $"{ToLinear():X6}";

        public Address ToAddress()
        {
            return Address.Create(this.DataType, this.ToUInt32());
        }
    }

    /// <summary>
    /// A PIC 12/14-bit data address.
    /// </summary>
    public class PICDataAddress : IComparable<PICDataAddress>
    {

        public const uint MAXDATABYTADDR = 0x3FFFu;
        public readonly Constant Value;
        public static readonly PICDataAddress Invalid = new PICDataAddress(InvalidConstant.Create(PrimitiveType.Ptr16));

        public PICDataAddress(uint addr)
        {
            DataType = PrimitiveType.Ptr16;
            Value = Constant.Create(DataType, addr & MAXDATABYTADDR);
        }

        public PICDataAddress(Constant addr)
        {
            DataType = addr.DataType;
            Value = addr;
        }

        public DataType DataType { get; }

        public bool IsValid => Value.IsValid;
        public bool IsNull => false;
        public ulong Offset => Value.ToUInt16();
        public ushort? Selector => null;

        public Address ToAddress()
            => Address.Create(this.DataType, ToUInt16());
        public Constant ToConstant()
            => Value;

        public ushort ToUInt16()
            => Value.ToUInt16();

        public uint ToUInt32()
            => Value.ToUInt16();

        public ulong ToLinear()
            => Value.ToUInt16();

        public PICDataAddress Add(long offset)
            => new PICDataAddress((uint)(Value.ToUInt16() + offset));

        public PICDataAddress Align(int alignment)
            => new PICDataAddress((uint)(alignment * ((Value.ToUInt16() + alignment - 1) / alignment)));

        //public override Expression CloneExpression()
        //    => new PICDataAddress(Value);

        public int CompareTo(PICDataAddress? that)
        {
            if (that is null)
                return 1;
            return this.Offset.CompareTo(that.Offset);
        }

        public string GenerateName(string prefix, string suffix)
            => $"{prefix}{Value.ToUInt16():X4}{suffix}";

        public PICDataAddress NewOffset(ulong offset)
            => new PICDataAddress((uint)offset);

        public static PICDataAddress Ptr(uint addr)
            => new PICDataAddress(addr);

        public static PICDataAddress Ptr(Address aaddr)
            => new PICDataAddress(aaddr.ToUInt32());

        protected string ConvertToString()
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
            if (region is null)
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
