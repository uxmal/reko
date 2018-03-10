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

using Reko.Libraries.Microchip;
using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic ;
using System.Linq;

namespace Reko.Arch.Microchip.Common
{
    public static class PICRegisterEx
    {

        /// <summary>
        /// Converts bit size to most appropriate Reko primitive word type.
        /// </summary>
        /// <param name="bitSize">The bit size.</param>
        /// <returns>
        /// A PrimitiveType value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when bit size is outside the required range.</exception>
        internal static PrimitiveType Size2PrimitiveType(this uint bitSize)
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
    public class PICRegisterStorage : RegisterStorage
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PICRegisterStorage() : base("None", -1, 0, PrimitiveType.Byte)
        {
            Traits = new PICRegisterTraits();
            SubRegs = null;
        }

        /// <summary>
        /// Constructor of a named PIC register.
        /// </summary>
        /// <param name="sfr">The SFR definition.</param>
        /// <param name="number">The Reko index number of this register.</param>
        public PICRegisterStorage(SFRDef sfr, int number)
            : base(sfr.CName, number, 0, sfr.NzWidth.Size2PrimitiveType())
        {
            Traits = new PICRegisterTraits(sfr);
            SubRegs = null;
        }

        public PICRegisterStorage(JoinedSFRDef jsfr, ICollection<PICRegisterStorage> subregs)
            : base(jsfr.CName, subregs.First().Number, 0, jsfr.NzWidth.Size2PrimitiveType())
        {
            Traits = new PICRegisterTraits(jsfr, subregs);
            SubRegs = subregs;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the traits of the PIC register.
        /// </summary>
        public PICRegisterTraits Traits { get; }

        /// <summary>
        /// Gets the data memory address of this register (or null if Non-Memory-Mapped).
        /// </summary>
        public PICDataAddress Address => Traits.Address;

        /// <summary>
        /// Gets the Non-Memory-Mapped ID or null if register is memory-mapped.
        /// </summary>
        public string NMMRID => Traits.NMMRID;

        /// <summary>
        /// Gets a value indicating whether this register is Memory-Mapped (true) or Non-Memory-Mapped (false)..
        /// </summary>
        public bool IsMemoryMapped => Traits.IsMemoryMapped; 

        /// <summary>
        /// Gets this register access bits descriptor.
        /// </summary>
        /// <value>
        /// The access descriptor as a string (see <seealso cref="Microchip.Crownking.PIC"/>.
        /// </value>
        public string Access => Traits.Access;

        /// <summary>
        /// Gets the state of each register's bit after a Master-Clear Reset.
        /// </summary>
        public string MCLR => Traits.MCLR;

        /// <summary>
        /// Gets the state of each register's bit after a Power-On Reset.
        /// </summary>
        public string POR => Traits.POR;

        /// <summary>
        /// Gets the implementation bit-mask.
        /// </summary>
        public ulong Impl => Traits.Impl;

        /// <summary>
        /// Gets a value indicating whether this PIC register is volatile.
        /// </summary>
        public bool IsVolatile => Traits.IsVolatile;

        /// <summary>
        /// Gets a value indicating whether this PIC register is indirect.
        /// </summary>
        public bool IsIndirect => Traits.IsIndirect;

        /// <summary>
        /// Gets the actual bit width of this PIC register.
        /// </summary>
        /// <value>
        /// The width in number of bits.
        /// </value>
        public int BitWidth => Traits.BitWidth;

        /// <summary>
        /// Gets the sub-registers of this PIC register, if any.
        /// </summary>
        /// <value>
        /// The sub-registers enumeration.
        /// </value>
        public IEnumerable<PICRegisterStorage> SubRegs { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC register has sub-registers.
        /// </summary>
        public bool HasSubRegs => ((SubRegs?.Count() ?? 0) > 0);

        /// <summary>
        /// The "None" PIC register.
        /// </summary>
        public new static PICRegisterStorage None = new PICRegisterStorage();

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj is PICRegisterStorage that)
            {
                return Address == that.Address &&
                    BitAddress == that.BitAddress &&
                    BitSize == that.BitSize;
            }
            return false;
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
    /// Defines the storage of a PIC register's named bit field.
    /// </summary>
    public class PICBitFieldStorage : FlagGroupStorage
    {

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reg">The PIC register containing the bit field.</param>
        /// <param name="sfrdef">The bit field definition per PIC XML definition.</param>
        /// <param name="bitPos">The least significant bit number as a byte.</param>
        /// <param name="uMask">The bit field mask.</param>
        public PICBitFieldStorage(PICRegisterStorage reg, SFRFieldDef sfrdef, byte bitPos, uint uMask)
            : base(reg, (uMask << bitPos), sfrdef.CName, sfrdef.NzWidth.Size2PrimitiveType())
        {
            SFRField = sfrdef;
            BitPos = bitPos;
        }

        #endregion

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

    }

}
