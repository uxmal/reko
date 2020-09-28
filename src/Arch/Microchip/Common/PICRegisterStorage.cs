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

using Reko.Core;
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.MicrochipPIC.Common
{

    /// <summary>
    /// Defines a PIC register (with/without named bit-fields or attached-registers).
    /// </summary>
    public class PICRegisterStorage : RegisterStorage,
        IComparable<PICRegisterStorage>, IComparer<PICRegisterStorage>,
        IEquatable<PICRegisterStorage>, IEqualityComparer<PICRegisterStorage>,
        IComparable
    {

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PICRegisterStorage() : base("None", -1, 0, PrimitiveType.Byte)
        {
            Traits = new PICRegisterTraits();
            AttachedRegs = null;
            BitFields = null;
        }

        /// <summary>
        /// Constructor of a named PIC register.
        /// </summary>
        /// <param name="regName">Name of the PIC register.</param>
        /// <param name="regNumber">The Reko index number of this register.</param>
        /// <param name="bitAddress">The lowest bit address of this register.</param>
        /// <param name="dt">The register word type.</param>
        /// <param name="traits">The traits of the PIC register.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="traits"/> is null.</exception>
        public PICRegisterStorage(string regName, int regNumber, uint bitAddress, PrimitiveType dt, PICRegisterTraits traits)
            : base(regName, regNumber, bitAddress, dt)
        {
            Traits = traits ?? throw new ArgumentNullException(nameof(traits));
            AttachedRegs = new List<PICRegisterStorage>();
            BitFields = new SortedList<PICRegisterBitFieldSortKey, PICRegisterBitFieldStorage>();
        }

        /// <summary>
        /// Constructor of a named PIC register.
        /// </summary>
        /// <param name="sfr">The SFR definition.</param>
        /// <param name="number">The Reko index number of this register.</param>
        public PICRegisterStorage(ISFRRegister sfr, int number)
            : this(sfr.Name, number, 0, PrimitiveType.CreateWord(sfr.BitWidth), new PICRegisterTraits(sfr))
        {
        }

        /// <summary>
        /// Constructor of a named joined PIC register.
        /// </summary>
        /// <param name="jsfr">The joined SFR definition.</param>
        /// <param name="number">The Reko index number of this register.</param>
        /// <param name="subregs">The sub-registers of the joint.</param>
        public PICRegisterStorage(IJoinedRegister jsfr, int number, IList<PICRegisterStorage> subregs)
            : base(jsfr.Name, number, 0, PrimitiveType.CreateWord(jsfr.BitWidth))
        {
            Traits = new PICRegisterTraits(jsfr, subregs);
            AttachedRegs = subregs.ToList();
            AttachedRegs.ForEach(r => r.ParentRegister = this);
            ParentRegister = null;
            BitFields = new SortedList<PICRegisterBitFieldSortKey, PICRegisterBitFieldStorage>();
        }

        /// <summary>
        /// Gets the optional parent PIC register.
        /// </summary>
        public PICRegisterStorage ParentRegister { get; internal set; }

        /// <summary>
        /// Gets the traits of the PIC register as provided by Microchip.
        /// </summary>
        public PICRegisterTraits Traits { get; }

        /// <summary>
        /// Gets the attached registers of this PIC register, if any.
        /// </summary>
        public List<PICRegisterStorage> AttachedRegs { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC register has attached registers.
        /// </summary>
        public bool HasAttachedRegs => ((AttachedRegs?.Count() ?? 0) > 0);

        /// <summary>
        /// Gets the bit-fields composing this register. Sorted by increasing width then bit position.
        /// </summary>
        public SortedList<PICRegisterBitFieldSortKey, PICRegisterBitFieldStorage> BitFields { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC register is composed of bit-fields.
        /// </summary>
        public bool HasBitFields => ((BitFields?.Count() ??0) > 0);

        /// <summary>
        /// The "None" PIC register.
        /// </summary>
        public new static PICRegisterStorage None = new PICRegisterStorage();

        public override T Accept<T>(StorageVisitor<T> visitor) => visitor.VisitRegisterStorage(this);

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context) => visitor.VisitRegisterStorage(this, context);

        public int CompareTo(PICRegisterStorage other)
        {
            if (other is null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            return Traits.CompareTo(other.Traits);
        }

        public int CompareTo(object obj) => CompareTo(obj as PICRegisterStorage);

        public int Compare(PICRegisterStorage x, PICRegisterStorage y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (x is null)
                return -1;
            return x.CompareTo(y);
        }

        public override bool Equals(object obj) => CompareTo(obj as PICRegisterStorage) == 0;

        public bool Equals(PICRegisterStorage other) => CompareTo(other) == 0;

        public bool Equals(PICRegisterStorage x, PICRegisterStorage y) => Compare(x, y) == 0;

        public int GetHashCode(PICRegisterStorage obj) => obj?.GetHashCode() ?? 0;

        public override int GetHashCode() => (Traits.GetHashCode() * 759523) ^ base.GetHashCode();

    }

}
