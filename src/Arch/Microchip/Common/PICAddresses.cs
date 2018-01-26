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

namespace Reko.Arch.Microchip.Common
{

    /// <summary>
    /// A PIC 21-bit program address.
    /// </summary>
    public class PICProgAddress : Address32
    {
        public const uint MAXPROGBYTADDR = 0x1FFFFFU;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="addr">The program byte address.</param>
        public PICProgAddress(uint addr) : base(addr & MAXPROGBYTADDR) { }

        public override bool IsNull => false;

        public override string ToString() => $"0x{ToUInt32():X6}";

        /// <summary>
        /// Create a <see cref="PICProgAddress"/> instance with specified byte address.
        /// </summary>
        /// <param name="addr">The program byte address as an integer.</param>
        /// <returns>
        /// The <see cref="PICProgAddress"/>.
        /// </returns>
        public static PICProgAddress Ptr(uint addr)            => new PICProgAddress(addr);

        /// <summary>
        /// Create a <see cref="PICProgAddress"/> instance with specified address.
        /// </summary>
        /// <param name="aaddr">The address.</param>
        /// <returns>
        /// The <see cref="PICProgAddress"/>.
        /// </returns>
        public static PICProgAddress Ptr(Address aaddr)            => new PICProgAddress(aaddr.ToUInt32());
    }

    /// <summary>
    /// A PIC 12/14-bit data address.
    /// </summary>
    public class PICDataAddress : Address32
    {
        public const uint MAXDATABYTADDR = 0x3FFFFU;

        public PICDataAddress(uint addr) : base(addr & MAXDATABYTADDR) { }

        public override bool IsNull => false;

        public override string ToString() => $"{ToUInt32():X4}";

        public static PICDataAddress Ptr(uint addr) => new PICDataAddress(addr);

        public static PICDataAddress Ptr(Address aaddr) => new PICDataAddress(aaddr.ToUInt32());

    }

    /// <summary>
    /// A PIC 8-bit data bank address.
    /// </summary>
    public class PICBankAddress : Address32
    {
        public PICBankAddress(byte addr) : base(addr) { }

        public override bool IsNull => false;

        public override string ToString()
        {
            return string.Format("0x{0:X2}", ToUInt32());
        }

        public static PICBankAddress Ptr(byte addr)
            => new PICBankAddress(addr);

    }

}
