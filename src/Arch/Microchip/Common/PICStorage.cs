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

using Microchip.Crownking;
using Reko.Core;
using Reko.Core.Types;
using System;

namespace Reko.Arch.Microchip.Common
{
    public static class PICRegisterEx
    {

        /// <summary>
        /// Converts bit size to most appropriate Reko primitive type.
        /// </summary>
        /// <param name="bitSize">The bit size.</param>
        /// <returns>
        /// A PrimitiveType value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when bit size is outside the required range.</exception>
        internal static PrimitiveType Size2PrimitiveType(uint bitSize)
        {
            if (bitSize == 0)
                throw new ArgumentOutOfRangeException(nameof(bitSize));
            if (bitSize == 1)
                return PrimitiveType.Bool;
            if (bitSize <= 8)
                return PrimitiveType.Byte;
            if (bitSize <= 16)
                return PrimitiveType.Word16;
            if (bitSize <= 32)
                return PrimitiveType.Word32;
            if (bitSize <= 64)
                return PrimitiveType.Word64;
            if (bitSize <= 128)
                return PrimitiveType.Word128;
            if (bitSize <= 256)
                return PrimitiveType.Word256;
            throw new ArgumentOutOfRangeException(nameof(bitSize));
        }

    }

    /// <summary>
    /// Defines a PIC register (with/without named bit fields).
    /// </summary>
    public class PICRegisterStorage : FlagRegister
    {

        #region Properties

        /// <summary>
        /// Gets the SFR register definition as defined by Microchip (Crownking DB).
        /// </summary>
        /// <value>
        /// The register definition.
        /// </value>
        public SFRDef SFRDef { get; }

        /// <summary>
        /// Gets the data memory address of this register (or null if Non-Memory-Mapped).
        /// </summary>
        /// <value>
        /// The address or null.
        /// </value>
        public Address Address { get; }

        /// <summary>
        /// Gets a value indicating whether this register is Non-Memory-Mapped.
        /// </summary>
        /// <value>
        /// True if this register is Non-Memory-Mapped, false if not.
        /// </value>
        public bool IsNMMR => SFRDef?.IsNMMR ?? true;

        /// <summary>
        /// Gets this register access bits descriptor.
        /// </summary>
        /// <value>
        /// The access descriptor as a string (see <seealso cref="Microchip.Crownking.PIC"/>.
        /// </value>
        public string Access => SFRDef?.Access;

        /// <summary>
        /// Gets the actual bit width of this register.
        /// </summary>
        /// <value>
        /// The width in number of bits.
        /// </value>
        public uint BitWidth => SFRDef?.NzWidth ?? 0;

        public new static PICRegisterStorage None =
            new PICRegisterStorage("None", -1, PrimitiveType.Ptr16);

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sfr">The SFR definition.</param>
        /// <param name="number">The Reko index number of this register.</param>
        /// <param name="isNMMR">(Optional)
        ///                      True if this register is Non-Memory-Mapped, false if not.</param>
        public PICRegisterStorage(SFRDef sfr, int number)
            : base(sfr.Name, number, PICRegisterEx.Size2PrimitiveType(sfr.NzWidth))
        {
            SFRDef = sfr;
            Address = (IsNMMR ? null : Address.Ptr16((ushort)sfr.Addr));
        }

        public PICRegisterStorage(string name, int number, PrimitiveType dt)
            : base(name, number, dt)
        {
            SFRDef = null;
            Address = null;
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            var that = obj as PICRegisterStorage;
            if (that == null)
                return false;
            return Address == that.Address &&
                BitAddress == that.BitAddress &&
                BitSize == that.BitSize;
        }

        public override int GetHashCode()
            => Address?.GetHashCode() ?? 0 * 23 ^ base.GetHashCode();

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitRegisterStorage(this);
        }

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitRegisterStorage(this, context);
        }

        #endregion

    }

    /// <summary>
    /// Defines a PIC register's named bit field storage.
    /// </summary>
    public class PICBitFieldStorage : FlagGroupStorage
    {

        #region Properties

        /// <summary>
        /// Gets the bit field definition as defined by Microchip (Crownking DB).
        /// </summary>
        public SFRFieldDef SFRField { get; }

        /// <summary>
        /// Gets the bit field position in the register (LSb number).
        /// </summary>
        /// <value>
        /// The least significant bit number as a byte.
        /// </value>
        public byte BitPos { get; }

        /// <summary>
        /// Gets the bit width of the field.
        /// </summary>
        /// <value>
        /// The width as number of bits.
        /// </value>
        public uint BitWidth => SFRField.NzWidth;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reg">The PIC register containing the bit field.</param>
        /// <param name="sfrdef">The bit field definition per PIC XML definition.</param>
        /// <param name="bitPos">The least significant bit number as a byte.</param>
        /// <param name="uMask">The bit field mask.</param>
        public PICBitFieldStorage(PICRegisterStorage reg, SFRFieldDef sfrdef, byte bitPos, uint uMask)
            : base(reg, (uMask << bitPos), sfrdef.Name, PICRegisterEx.Size2PrimitiveType(sfrdef.NzWidth))
        {
            SFRField = sfrdef;
            BitPos = bitPos;
        }

        #endregion

    }

}
