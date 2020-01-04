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

using System;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Reko.Libraries.Microchip
{
    /// <summary>
    /// The abstract class <see cref="MemoryAddrRange"/> represents a PIC memory address range [begin, end) (either in data, program or absolute space).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class MemoryAddrRange : IPICMemoryAddrRange,
        IEquatable<MemoryAddrRange>, IEquatable<IPICMemoryAddrRange>, IEqualityComparer<MemoryAddrRange>,
        IComparable<MemoryAddrRange>, IComparable<IPICMemoryAddrRange>, IComparer<MemoryAddrRange>
    {

        public MemoryAddrRange() { }

        /// <summary> Gets the memory domain. </summary>
        [XmlIgnore]
        public abstract PICMemoryDomain MemoryDomain { get; }

        /// <summary> Gets the memory sub domain. </summary>
        [XmlIgnore]
        public abstract PICMemorySubDomain MemorySubDomain { get; }

        /// <summary> Gets the begin address of the memory range. </summary>
        [XmlIgnore]
        public uint BeginAddr { get; private set; }

        /// <summary> Gets the end address of the memory range. </summary>
        [XmlIgnore]
        public uint EndAddr { get; private set; }

        [XmlAttribute(AttributeName = "beginaddr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _beginaddrformatted { get => $"0x{BeginAddr:X}"; set => BeginAddr = value.ToUInt32Ex(); }

        [XmlAttribute(AttributeName = "endaddr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _endaddrformatted { get => $"0x{EndAddr:X}"; set => EndAddr = value.ToUInt32Ex(); }


        #region Implementation of the equality/comparison interfaces

        public bool Equals(IPICMemoryAddrRange other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (MemoryDomain != other.MemoryDomain)
                return false;
            if (BeginAddr != other.BeginAddr)
                return false;
            return (EndAddr == other.EndAddr);
        }

        public bool Equals(MemoryAddrRange other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (MemoryDomain != other.MemoryDomain)
                return false;
            if (BeginAddr != other.BeginAddr)
                return false;
            return (EndAddr == other.EndAddr);
        }

        public override bool Equals(object obj) => Equals(obj as IPICMemoryAddrRange);

        public override int GetHashCode() => (BeginAddr.GetHashCode() + 17 * EndAddr.GetHashCode()) ^ MemoryDomain.GetHashCode();

        public static bool operator ==(MemoryAddrRange reg1, MemoryAddrRange reg2) => _Compare(reg1, reg2) == 0;

        public static bool operator !=(MemoryAddrRange reg1, MemoryAddrRange reg2) => _Compare(reg1, reg2) != 0;

        public int Compare(MemoryAddrRange x, MemoryAddrRange y)
            => _Compare(x, y);

        private static int _Compare(MemoryAddrRange x, IPICMemoryAddrRange y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if ((object)x == null)
                return -1;
            return x.CompareTo(y);
        }

        public bool Equals(MemoryAddrRange x, MemoryAddrRange y)
        {
            if (ReferenceEquals(x, y))
                return true;
            return x?.Equals(y) ?? false;
        }

        public int GetHashCode(MemoryAddrRange obj) => obj?.GetHashCode() ?? 0;

        public int CompareTo(IPICMemoryAddrRange other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            if (MemoryDomain != other.MemoryDomain)
                return MemoryDomain.CompareTo(other.MemoryDomain);
            if (BeginAddr == other.BeginAddr)
                return EndAddr.CompareTo(other.EndAddr);
            return BeginAddr.CompareTo(other.BeginAddr);
        }

        public int CompareTo(MemoryAddrRange other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            if (MemoryDomain != other.MemoryDomain)
                return MemoryDomain.CompareTo(other.MemoryDomain);
            if (BeginAddr == other.BeginAddr)
                return EndAddr.CompareTo(other.EndAddr);
            return BeginAddr.CompareTo(other.BeginAddr);
        }

        #endregion

    }

}
