#region License
/* 
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

namespace Reko.Core.Types
{
	/// <summary>
	/// Represents a union of non-compatible types such as int and reals, or
    /// pointers and ints, or differently sized integers.
	/// </summary>
	/// <remarks>
	/// Union alternatives are required to be inserted in-order according to
    /// the total ordering introduced by DataTypeComparer.
	/// </remarks>
	public class UnionType : DataType
	{
        /// <summary>
        /// Constructs a union type with a specific name and an optional preferred
        /// alternative type.
        /// </summary>
        /// <param name="name">Optional name.</param>
        /// <param name="preferredType">Optional preferred type.</param>
        public UnionType(string? name, DataType? preferredType) : this(name, preferredType, false)
        {
        }

        /// <summary>
        /// Constructs a union type with a specific name and an optional preferred
        /// alternative type.
        /// </summary>
        /// <param name="name">Optional name.</param>
        /// <param name="preferredType">Optional preferred type.</param>
        /// <param name="alternatives">Collection of alternatives.</param>
		public UnionType(string? name, DataType? preferredType, ICollection<DataType> alternatives) : this(name, preferredType, false)
        {
            foreach (DataType dt in alternatives)
            {
                AddAlternative(dt);
            }
        }

        /// <summary>
        /// Constructs a union type with a specific name and an optional preferred
        /// alternative type.
        /// </summary>
        /// <param name="name">Optional name.</param>
        /// <param name="preferredType">Optional preferred type.</param>
        /// <param name="alternatives">Collection of alternatives.</param>
        public UnionType(string? name, DataType? preferredType, params DataType [] alternatives) : this(name, preferredType, false, alternatives)
        {
        }

        /// <summary>
        /// Constructs a union type with a specific name and an optional preferred
        /// alternative type.
        /// </summary>
        /// <param name="name">Optional name.</param>
        /// <param name="preferredType">Optional preferred type.</param>
        /// <param name="userDefined">True if the union is user-defined.</param>
        /// <param name="alternatives">Collection of alternatives.</param>
        public UnionType(string? name, DataType? preferredType, bool userDefined, params DataType[] alternatives) 
            : base(Domain.Union, name)
        {
            this.Alternatives = [];
            this.PreferredType = preferredType;
            this.UserDefined = userDefined;
            foreach (DataType dt in alternatives)
            {
                AddAlternative(dt);
            }
        }

        /// <summary>
        /// The alternatives of the union.
        /// </summary>
        public UnionAlternativeCollection Alternatives { get; }
        
        /// <summary>
        /// If not null, the preferred alternative.
        /// </summary>
        public DataType? PreferredType { get; set; }

        /// <summary>
        /// True if this union is "user-defined" and shouldn't be mutated
        /// by type inference.
        /// </summary>
        public bool UserDefined { get; private set; }

        /// <inheritdoc/>
        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitUnion(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitUnion(this);
        }

        /// <summary>
        /// Adds an alternative to this union.
        /// </summary>
        /// <param name="dt">Data type of the laternative.
        /// </param>
        /// <returns>An instance of <see cref="UnionAlternative"/>.
        /// </returns>
        public UnionAlternative AddAlternative(DataType dt)
        {
            var alt = new UnionAlternative(dt, Alternatives.Count);
            Alternatives.Add(alt);
            return alt;
        }

        /// <inheritdoc/>
        public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
		{
			var pre = PreferredType?.Clone(clonedTypes);
			var u = new UnionType(Name, pre);
            u.Qualifier = Qualifier;
            u.UserDefined = UserDefined;
            foreach (var a in this.Alternatives.Values)
			{
                u.Alternatives.Add(a.Clone());
			}
			return u;
		}

        /// <summary>
        /// Finds the alternative matching <paramref name="dtOrig"/>.
        /// </summary>
		public UnionAlternative? FindAlternative(DataType dtOrig)
		{
			foreach (UnionAlternative alt in Alternatives.Values)
			{
				if (Object.Equals(alt.DataType, dtOrig))
					return alt;
			}
			return null;
		}

        /// <inheritdoc/>
        public override bool IsComplex => true;

        private static int nestoMatic;


        /// <inheritdoc/>
		public override int BitSize
		{
			get
			{
				int size = 0;
                ++nestoMatic;
                if (nestoMatic > 100)
                    return 32;
				foreach (UnionAlternative alt in Alternatives.Values)
				{
					size = Math.Max(size, alt.DataType.BitSize);
				}
                --nestoMatic;
				return size;
			}
		}

        /// <inheritdoc/>
        public override int Size
        {
            get { return (BitSize + (BitsPerByte - 1)) / BitsPerByte; }
            set { ThrowBadSize(); }
        }

        /// <summary>
        /// Simplifies the union; if there is only one alternative, return
        /// that single alternative.
        /// </summary>
        public DataType Simplify()
		{
			if (Alternatives.Count == 1)
			{
				return Alternatives.Values[0].DataType;
			}
			else
				return this;
		}
	}

    /// <summary>
    /// Represents one of the alternative of a <see cref="UnionType"/>.
    /// </summary>
	public class UnionAlternative : Field
	{
        /// <summary>
        /// Constructs a union alternative with no specific name.
        /// </summary>
        /// <param name="dt">Data type of the alternative.</param>
        /// <param name="index">Index within the union.</param>
        public UnionAlternative(DataType dt, int index) : base(dt)
		{
			this.DataType = dt;
            this.Index = index;
		}

        /// <summary>
        /// Constructs a union alternative with a name.
        /// </summary>
        /// <param name="name">Optional name of the alternative.</param>
        /// <param name="dt">Data type of the alternative.</param>
        /// <param name="index">Index within the union.
        /// </param>
		public UnionAlternative(string? name, DataType dt, int index) : base(dt)
		{
			DataType = dt;
			this.name = name;
            Index = index;
        }

        /// <inheritdoc/>
        public override string Name { get { if (name is null) return GenerateDefaultName(); return name; } set { name = value; } }

        /// <summary>
        /// Index of the union alternative within the union.
        /// </summary>
        public int Index { get; }

        private string? name;

        private string GenerateDefaultName()
        {
            return string.Format("u{0}", Index);
        }

        /// <summary>
        /// Clones this alternative.
        /// </summary>
        /// <returns>Creates a new union alternative.</returns>
        public UnionAlternative Clone()
        {
            return new UnionAlternative(name, DataType, Index);
        }
    }

    /// <summary>
    /// Represents a collection of union alternatives.
    /// </summary>
	public class UnionAlternativeCollection : SortedList<DataType,UnionAlternative>
	{
        /// <summary>
        /// Constructs an empty collection of union alternatives.
        /// </summary>
        public UnionAlternativeCollection()
            : base(DataTypeComparer.Instance)
        {
        }

        /// <summary>
        /// Adds a new alternative to the union.
        /// </summary>
        /// <param name="a">Alternative to add.</param>
		public void Add(UnionAlternative a)
		{
            base[a.DataType] = a;
		}

        /// <summary>
        /// Adds a new alternative to the union.
        /// </summary>
        /// <param name="dt">Datatype to add.</param>
		public void Add(DataType dt)
		{
			Add(new UnionAlternative(dt, Count));
		}

        /// <summary>
        /// Adds a range of alternatives to the union.
        /// </summary>
        /// <param name="alternatives">Alternatives to add.</param>
        public void AddRange(IEnumerable<UnionAlternative> alternatives)
        {
            foreach (var alt in alternatives)
            {
                Add(alt);
            }
        }
    }
}
