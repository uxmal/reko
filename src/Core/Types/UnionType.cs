#region License
/* 
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
using System.IO;

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
		private UnionAlternativeCollection alts = new UnionAlternativeCollection();

        public UnionType() : this(null, null, false)
        {
        }

        public UnionType(string name, DataType preferredType) : this(name, preferredType, false)
        {
        }

		public UnionType(string name, DataType preferredType, ICollection<DataType> alternatives) : this(name, preferredType, false)
        {
            foreach (DataType dt in alternatives)
            {
                AddAlternative(dt);
            }
        }

        public UnionType(string name, DataType preferredType, params DataType [] alternatives) : this(name, preferredType, false, alternatives)
        {
        }

        public UnionType(string name, DataType preferredType, bool userDefined, params DataType[] alternatives) : base(name)
        {
            this.PreferredType = preferredType;
            this.UserDefined = userDefined;
            foreach (DataType dt in alternatives)
            {
                AddAlternative(dt);
            }
        }

        public DataType PreferredType { get; set; }
        public bool UserDefined { get; private set; }

        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitUnion(this);
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitUnion(this);
        }

		public UnionAlternativeCollection Alternatives
		{
			get { return alts; }
		}

        public UnionAlternative AddAlternative(DataType dt)
        {
            var alt = new UnionAlternative(dt, Alternatives.Count);
            Alternatives.Add(alt);
            return alt;
        }

        public override DataType Clone(IDictionary<DataType, DataType> clonedTypes)
		{
			var pre = PreferredType != null ? PreferredType.Clone(clonedTypes) : null;
			var u = new UnionType(Name, pre);
            u.Qualifier = Qualifier;
            u.UserDefined = UserDefined;
            foreach (var a in this.Alternatives.Values)
			{
                u.Alternatives.Add(a.Clone());
			}
			return u;
		}

		public UnionAlternative FindAlternative(DataType dtOrig)
		{
			foreach (UnionAlternative alt in Alternatives.Values)
			{
				if (Object.Equals(alt.DataType, dtOrig))
					return alt;
			}
			return null;
		}

		public override bool IsComplex
		{
			get { return true; }
		}

		public override string Prefix
		{
			get { return "u"; }
		}

        private static int nestoMatic;

		public override int Size
		{
			get
			{
				int size = 0;
                ++nestoMatic;
                if (nestoMatic > 100)
                    return 4; ;
				foreach (UnionAlternative alt in Alternatives.Values)
				{
					size = Math.Max(size, alt.DataType.Size);
				}
                --nestoMatic;
				return size;
			}
			set { ThrowBadSize(); }
		}

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

	public class UnionAlternative : Field
	{
        private int index;

        public UnionAlternative(DataType t, int index)
		{
			this.DataType = t;
            this.index = index;
		}

		public UnionAlternative(string name, DataType dt, int index)
		{
			DataType = dt;
			Name = name;
            this.index = index;
        }

        public override string Name { get { if (name == null) return GenerateDefaultName(); return name; } set { name = value; } }
        private string name;

        private string GenerateDefaultName()
        {
            return string.Format("u{0}", index);
        }

        public UnionAlternative Clone()
        {
            return new UnionAlternative(name, DataType, index);
        }
    }

	public class UnionAlternativeCollection : SortedList<DataType,UnionAlternative>
	{
        public UnionAlternativeCollection()
            : base(new DataTypeComparer())
        {
        }
		public void Add(UnionAlternative a)
		{
            base[a.DataType] = a;
		}

		public void Add(DataType dt)
		{
			Add(new UnionAlternative(dt, Count));
		}

        public void AddRange(IEnumerable<UnionAlternative> alternatives)
        {
            foreach (var alt in alternatives)
            {
                Add(alt);
            }
        }
    }
}
