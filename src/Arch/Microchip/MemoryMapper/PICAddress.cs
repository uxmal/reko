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

namespace Microchip.MemoryMapper
{

    /// <summary>
    /// The <see cref="ProgAddress"/> class defines the PIC Program Memory address. Common to PIC16 and PIC18.
    /// </summary>
    public class ProgAddress : Address
    {
        private const uint cMaxAddress = 0x1FFFFF;

        private uint uAddr;

        public static readonly Address NULL = new ProgAddress(0);
        public static readonly ProgAddress MINADDRESS = new ProgAddress(0);
        public static readonly ProgAddress MAXADDRESS = new ProgAddress(cMaxAddress);

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="addr">The program memory address as a 32-bit unsigned integer.</param>
        public ProgAddress(uint addr) : base(PrimitiveType.Pointer32)
        {
            uAddr = addr & cMaxAddress;
        }

        public override bool IsNull { get { return uAddr == 0; } }
        public override ulong Offset { get { return uAddr; } }
        public override ushort? Selector { get { return null; } }

        public override Address Add(long offset)
        {
            var uNew = uAddr + offset;
            return new ProgAddress((uint)uNew);
        }

        public override Address Align(int alignment)
            => new ProgAddress((uint)(alignment * ((uAddr + alignment - 1) / alignment)));

        public override Expression CloneExpression() => new ProgAddress(uAddr);

        public override string GenerateName(string prefix, string suffix) => $"{prefix}{uAddr:X6}{suffix}";

        public override Address NewOffset(ulong offset) => new ProgAddress((uint)offset);

        public override Constant ToConstant() => Constant.UInt32(uAddr);

        public override ushort ToUInt16()
        {
            throw new InvalidOperationException("Returning UInt16 would lose precision.");
        }

        public override uint ToUInt32() => uAddr;

        public override ulong ToLinear() => uAddr;

        public static Address operator +(ProgAddress a, ulong off)
        {
            return a.Add((long)off);
        }

        public static Address operator +(ProgAddress a, long off)
        {
            return a.Add(off);
        }

        public static Address operator -(ProgAddress a, int delta)
        {
            return a.Add(-delta);
        }

        public static long operator -(ProgAddress a, Address b)
        {
            return (long)a.ToLinear() - (long)b.ToLinear();
        }

        public override string ToString() => $"{uAddr:X6}";

    }

    public class DataAddress : Address
    {
        private const ushort cMaxAddress = 0x3FFF;

        private ushort uAddr;

        public static readonly Address NULL = new DataAddress(0);
        public static readonly DataAddress MINADDRESS = new DataAddress(0);
        public static readonly DataAddress MAXADDRESS = new DataAddress(cMaxAddress);

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="addr">The program memory address as a 16-bit unsigned integer.</param>
        public DataAddress(ushort addr) : base(PrimitiveType.Ptr16)
        {
            uAddr = (ushort)(addr & cMaxAddress);
        }

        public override bool IsNull { get { return uAddr == 0; } }
        public override ulong Offset { get { return uAddr; } }
        public override ushort? Selector { get { return null; } }

        public override Address Add(long offset)
        {
            var uNew = uAddr + offset;
            return new DataAddress((ushort)uNew);
        }

        public override Address Align(int alignment)
        {
            return new DataAddress((ushort)(alignment * ((uAddr + alignment - 1) / alignment)));
        }

        public override Expression CloneExpression() => new DataAddress(uAddr);

        public override string GenerateName(string prefix, string suffix) => $"{prefix}{uAddr:X4}{suffix}";

        public override Address NewOffset(ulong offset) => new DataAddress((ushort)offset);

        public override Constant ToConstant() => Constant.UInt16(uAddr);

        public override ushort ToUInt16() => uAddr;

        public override uint ToUInt32() => uAddr;

        public override ulong ToLinear() => uAddr;

        public static Address operator +(DataAddress a, ulong off)
        {
            return a.Add((long)off);
        }

        public static Address operator +(DataAddress a, long off)
        {
            return a.Add(off);
        }

        public static Address operator -(DataAddress a, int delta)
        {
            return a.Add(-delta);
        }

        public static long operator -(DataAddress a, Address b)
        {
            return (long)a.ToLinear() - (long)b.ToLinear();
        }

        public override string ToString() => $"{uAddr:X4}";

    }

}
