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

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// This class permits to define a unique register's address whether it is an actual data memory address or a non-memory-mapped ID (NMMRID).
    /// </summary>
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public class PICRegisterUniqueAddress :
        IComparable<PICRegisterUniqueAddress>, IComparer<PICRegisterUniqueAddress>,
        IEquatable<PICRegisterUniqueAddress>, IEqualityComparer<PICRegisterUniqueAddress>,
        IComparable
    {
        /// <summary>
        /// The register address if memory-mapped.
        /// </summary>
        public readonly PICDataAddress? Addr;

        /// <summary>
        /// The register ID is non-memory-mapped.
        /// </summary>
        public readonly string NMMRID;

        public PICRegisterUniqueAddress(PICDataAddress addr)
        {
            Addr = addr ?? throw new ArgumentNullException(nameof(addr));
            NMMRID = string.Empty;
        }

        public PICRegisterUniqueAddress(ushort addr) : this(PICDataAddress.Ptr(addr))
        {
        }

        public PICRegisterUniqueAddress(string nmmrID)
        {
            if (string.IsNullOrEmpty(nmmrID))
                throw new ArgumentNullException(nameof(nmmrID));
            Addr = null;
            NMMRID = nmmrID;
        }

        public int CompareTo(PICRegisterUniqueAddress? other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            if (Addr is null)
            {
                if (other.Addr is null)
                    return NMMRID.CompareTo(other.NMMRID);
                return -1;
            }
            if (other.Addr is null)
                return 1;
            return Addr.CompareTo(other.Addr);
        }

        public virtual int CompareTo(object? obj) => CompareTo(obj as PICRegisterUniqueAddress);

        public int Compare(PICRegisterUniqueAddress? x, PICRegisterUniqueAddress? y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (x is null)
                return -1;
            return x.CompareTo(y);
        }

        public bool Equals(PICRegisterUniqueAddress? other) => CompareTo(other) == 0;

        public bool Equals(PICRegisterUniqueAddress? x, PICRegisterUniqueAddress? y) => Compare(x, y) == 0;

        public override bool Equals(object? obj) => CompareTo(obj as PICRegisterUniqueAddress) == 0;

        public int GetHashCode(PICRegisterUniqueAddress? obj) => obj?.GetHashCode() ?? 0;

        public override int GetHashCode() => ((Addr?.GetHashCode() ?? NMMRID.GetHashCode()) * 512137) ^ base.GetHashCode();

        private string _debugDisplay
            => "RegAddr=" + (Addr is null ? $"NMMRID({NMMRID})" : $"{Addr}");

    }

}
