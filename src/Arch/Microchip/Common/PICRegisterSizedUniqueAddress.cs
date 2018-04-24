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

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Microchip.Common
{
    [DebuggerDisplay("RegSizedAddr={_debugDisplay()}")]
    public class PICRegisterSizedUniqueAddress : PICRegisterUniqueAddress,
        IComparable<PICRegisterSizedUniqueAddress>, IComparer<PICRegisterSizedUniqueAddress>,
        IEquatable<PICRegisterSizedUniqueAddress>, IEqualityComparer<PICRegisterSizedUniqueAddress>,
        IComparable
    {

        /// <summary>
        /// The bit width of the register.
        /// </summary>
        public readonly int BitWidth;

        public PICRegisterSizedUniqueAddress(PICAddress addr, int bitWidth = 0) : base(addr)
        {
            BitWidth = bitWidth;
        }

        public PICRegisterSizedUniqueAddress(ushort addr, int bitWidth = 0) : this(PICDataAddress.Ptr(addr), bitWidth)
        {
        }

        public PICRegisterSizedUniqueAddress(string nmmrID, int bitWidth = 0) : base(nmmrID)
        {
            BitWidth = bitWidth;
        }

        public static bool Normalize { get; set; } = false;

        public int CompareTo(PICRegisterSizedUniqueAddress other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            var res = base.CompareTo(other);
            if (res == 0)
            {
                if (Normalize || (BitWidth <= 0) || (other.BitWidth <= 0))
                    return 0;
                return BitWidth.CompareTo(other.BitWidth);
            }
            return res;
        }

        public override int CompareTo(object obj) => CompareTo(obj as PICRegisterSizedUniqueAddress);

        public int Compare(PICRegisterSizedUniqueAddress x, PICRegisterSizedUniqueAddress y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (x is null)
                return -1;
            return x.CompareTo(y);
        }

        public bool Equals(PICRegisterSizedUniqueAddress other) => CompareTo(other) == 0;

        public bool Equals(PICRegisterSizedUniqueAddress x, PICRegisterSizedUniqueAddress y) => Compare(x, y) == 0;

        public override bool Equals(object obj) => CompareTo(obj as PICRegisterSizedUniqueAddress) == 0;

        public int GetHashCode(PICRegisterSizedUniqueAddress obj) => obj?.GetHashCode() ?? 0;

        public override int GetHashCode() => BitWidth.GetHashCode() ^ base.GetHashCode();

        private string _debugDisplay() => (Addr is null ? $"NMMRID({NMMRID}[b{BitWidth}])" : $"{Addr}[b{BitWidth}]");

    }

}
