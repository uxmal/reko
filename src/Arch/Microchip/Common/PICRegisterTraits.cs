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

using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Reko.Arch.MicrochipPIC.Common
{

    [DebuggerDisplay("RegTrait={_debugDisplay()}")]
    /// <summary>
    /// PIC register traits.
    /// </summary>
    public class PICRegisterTraits :
        IComparable<PICRegisterTraits>, IComparer<PICRegisterTraits>,
        IEquatable<PICRegisterTraits>, IEqualityComparer<PICRegisterTraits>,
        IComparable
    {

        public readonly PICRegisterSizedUniqueAddress RegAddress;

        public PICRegisterTraits()
        {
            RegAddress = new PICRegisterSizedUniqueAddress(PICAddress.Invalid, 8);
            Name = "None";
            Desc = "";
            Impl = 0xFF;
            Access = new string('n', 8);
            POR = MCLR = new string('u', 8);
            IsVolatile = false;
            IsIndirect = false;

        }

        /// <summary>
        /// Construct the PIC register's traits based on the given <see cref="SFRDef"/> descriptor.
        /// </summary>
        /// <param name="sfr">The PIC register descriptor.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sfr"/> is null.</exception>
        public PICRegisterTraits(SFRDef sfr)
        {
            if (sfr is null)
                throw new ArgumentNullException(nameof(sfr));
            if (string.IsNullOrEmpty(sfr.NMMRID))
                RegAddress = new PICRegisterSizedUniqueAddress(PICDataAddress.Ptr(sfr.Addr), (int)sfr.NzWidth);
            else
                RegAddress = new PICRegisterSizedUniqueAddress(sfr.NMMRID, (int)sfr.NzWidth);
            Name = sfr.CName;
            Desc = sfr.Desc;
            Impl = sfr.Impl;
            Access = AdjustString(sfr.Access, '-');
            MCLR = AdjustString(sfr.MCLR, 'u');
            POR = AdjustString(sfr.POR, '0');
            IsVolatile = sfr.IsVolatile;
            IsIndirect = sfr.IsIndirect;
        }

        /// <summary>
        /// Construct the PIC register's traits based on the given <see cref="JoinedSFRDef"/> descriptor.
        /// </summary>
        /// <param name="joinedSFR">The joined PIC register descriptor.</param>
        /// <param name="joinedRegs">The attached registers.</param>
        /// <exception cref="ArgumentNullException">Thrown if either of the arguments are null.</exception>
        public PICRegisterTraits(JoinedSFRDef joinedSFR, ICollection<PICRegisterStorage> joinedRegs)
        {
            if (joinedSFR is null)
                throw new ArgumentNullException(nameof(joinedSFR));
            if (joinedRegs is null)
                throw new ArgumentNullException(nameof(joinedRegs));
            Name = joinedSFR.CName;
            Desc = joinedSFR.Desc;
            RegAddress = new PICRegisterSizedUniqueAddress(PICDataAddress.Ptr(joinedSFR.Addr), (int)joinedSFR.NzWidth);
            Access = AdjustString(String.Join("", joinedRegs.Reverse().Select(e => e.Traits.Access)), '-');
            MCLR = AdjustString(String.Join("", joinedRegs.Reverse().Select(e => e.Traits.MCLR)), 'u');
            POR = AdjustString(String.Join("", joinedRegs.Reverse().Select(e => e.Traits.POR)), '0');
            Impl = joinedRegs.Reverse().Aggregate(0UL, (total, reg) => total = (total << 8) + reg.Traits.Impl);
            IsVolatile = joinedRegs.Any(e => e.Traits.IsVolatile == true);
            IsIndirect = joinedRegs.Any(e => e.Traits.IsIndirect == true);
        }


        /// <summary>
        /// Gets the PIC register name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the PIC register description.
        /// </summary>
        public string Desc { get; }

        /// <summary>
        /// Gets the register memory address or null if not memory-mapped.
        /// </summary>
        public PICAddress Address => RegAddress.Addr;

        /// <summary>
        /// Gets the Non-Memory-Mapped ID of the register or null if memory mapped.
        /// </summary>
        public string NMMRID => RegAddress.NMMRID;

        /// <summary>
        /// Gets the PIC register bit width.
        /// </summary>
        public int BitWidth => RegAddress.BitWidth;

        /// <summary>
        /// Gets the individual bits access modes.
        /// </summary>
        public string Access { get; private set; }

        /// <summary>
        /// Gets the individual bits state after a Master Clear.
        /// </summary>
        public string MCLR { get; private set; }

        /// <summary>
        /// Gets the individual bits state after a Power-On reset.
        /// </summary>
        public string POR { get; private set; }

        /// <summary>
        /// Gets the PIC register implementation mask.
        /// </summary>
        public ulong Impl { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC register is volatile.
        /// </summary>
        public bool IsVolatile { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC register is indirect.
        /// </summary>
        public bool IsIndirect { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC register is memory mapped.
        /// </summary>
        public bool IsMemoryMapped => !(Address is null);

        public int CompareTo(PICRegisterTraits other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            return RegAddress.CompareTo(other.RegAddress);
        }

        public int CompareTo(object obj) => CompareTo(obj as PICRegisterTraits);

        public int Compare(PICRegisterTraits x, PICRegisterTraits y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (x is null)
                return -1;
            return x.CompareTo(y);
        }

        public bool Equals(PICRegisterTraits obj) => CompareTo(obj) == 0;

        public bool Equals(PICRegisterTraits x, PICRegisterTraits y) => Compare(x, y) == 0;

        public override bool Equals(object obj) => CompareTo(obj as PICRegisterTraits) == 0;

        public int GetHashCode(PICRegisterTraits obj) => obj?.GetHashCode() ?? 0;

        public override int GetHashCode() => (RegAddress.GetHashCode() * 1234417) ^ base.GetHashCode();

        private string AdjustString(string s, char pad)
        {
            var len = BitWidth;
            if (s.Length < len)
            {
                s = new string(pad, len - s.Length) + s;
            }
            else if (s.Length > len)
            {
                s = s.Substring(s.Length - len);
            }
            return s;
        }

        private string _debugDisplay() => (RegAddress.Addr is null ? $"'{RegAddress.NMMRID}'" : $"{RegAddress.Addr}") + $"[b{BitWidth}]";

    }

}
