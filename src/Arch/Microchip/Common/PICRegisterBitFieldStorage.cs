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
using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.MicrochipPIC.Common
{

    /// <summary>
    /// A key based on the bit-field's width and position.
    /// The narrowest fields are considered smaller than the larger fields, and then the bit positions discriminate against any equality.
    /// </summary>
    public class PICRegisterBitFieldSortKey : IComparable<PICRegisterBitFieldSortKey>, IEquatable<PICRegisterBitFieldSortKey>, IComparable
    {
        private const int FNV_offset = unchecked((int)2166136261);
        private const int FNV_prime = 16777619;

        public readonly byte BitWidth;
        public readonly byte BitPos;

        public PICRegisterBitFieldSortKey(byte bitPos, byte bitWidth)
        {
            BitPos = bitPos;
            BitWidth = bitWidth;
        }

        public override int GetHashCode()
        {
            // FNV-like hash
            unchecked
            {
                int hashCode = FNV_offset;
                hashCode = (hashCode * FNV_prime) ^ BitPos.GetHashCode();
                hashCode = (hashCode * FNV_prime) ^ BitWidth.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(PICRegisterBitFieldSortKey? other) => CompareTo(other) == 0;

        public override bool Equals(object? obj) => Equals(obj as PICRegisterBitFieldSortKey);

        public int CompareTo(PICRegisterBitFieldSortKey? other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(other, this))
                return 0;
            if (BitWidth == 0 || other.BitWidth == 0 || BitWidth == other.BitWidth) // zero-width is considered a wildchar (normalize).
                return BitPos.CompareTo(other.BitPos);
            return BitWidth.CompareTo(other.BitWidth);
        }

        public int CompareTo(object? obj) => CompareTo(obj as PICRegisterBitFieldSortKey);

    }


    /// <summary>
    /// Defines the storage of a PIC register's named bit field.
    /// </summary>
    public class PICRegisterBitFieldStorage : FlagGroupStorage
    {

        public readonly PICRegisterBitFieldSortKey BitFieldSortKey;

        /// <summary>
        /// Instantiates a new PIC register's bit-field.
        /// </summary>
        /// <param name="reg">The PIC register containing the bit field.</param>
        /// <param name="sfrfielddef">The bit field definition per PIC XML definition.</param>
        public PICRegisterBitFieldStorage(PICRegisterStorage reg, ISFRBitField sfrfielddef)
            : base(reg, (uint)(sfrfielddef.BitMask << sfrfielddef.BitPos), sfrfielddef.Name)
        {
            SFRField = sfrfielddef;
            BitFieldSortKey = new PICRegisterBitFieldSortKey(sfrfielddef.BitPos, (byte)sfrfielddef.BitWidth);
        }

        /// <summary>
        /// Gets the bit field definition as defined by Microchip (Crownking DB).
        /// </summary>
        public ISFRBitField SFRField { get; }

        /// <summary>
        /// Gets the bit field position in the register (LSb number).
        /// </summary>
        /// <value>
        /// The least significant bit number as a byte.
        /// </value>
        public byte BitPos => BitFieldSortKey.BitPos;

        /// <summary>
        /// Gets the bit width of the field.
        /// </summary>
        /// <value>
        /// The width as number of bits.
        /// </value>
        public uint BitWidth => BitFieldSortKey.BitWidth;

        /// <summary>
        /// Gets the parent register.
        /// </summary>
        public PICRegisterStorage? ParentReg => FlagRegister as PICRegisterStorage;
    }

}
