/* 
 * Copyright (C) 1999-2008 John Källén.
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

using System;
using System.IO;
using System.Collections;

namespace Decompiler.Core.Types
{
	/// <summary>
	/// Represents a union of non-compatible types such as int and reals, or pointers and ints, or differently sized integers.
	/// </summary>
	/// <remarks>
	/// Union alternatives are required to be inserted in-order according to the total ordering introduced by DataTypeComparer.
	/// </remarks>
	public class UnionType : DataType
	{
		public DataType PreferredType;
		private UnionAlternativeCollection alts = new UnionAlternativeCollection();

		public UnionType() : base(null)
		{
			this.PreferredType = null;
		}

		public UnionType(string name, DataType preferredType) : base(name)
		{
			this.Name = name;
			this.PreferredType = preferredType; 
		}

		public UnionType(string name, DataType preferredType, ICollection alternatives) : base(name)
		{
			this.Name = name; this.PreferredType = preferredType; 
			foreach (DataType dt in alternatives)
			{
				Alternatives.Add(new UnionAlternative(dt));
			}
		}

		public override DataType Accept(DataTypeTransformer t)
		{
			return t.TransformUnionType(this);
		}


		public override void Accept(IDataTypeVisitor v)
		{
			v.VisitUnion(this);
		}


		public UnionAlternativeCollection Alternatives
		{
			get { return alts; }
		}

		public override DataType Clone()
		{
			DataType pre = PreferredType != null ? PreferredType.Clone() : null;
			UnionType u = new UnionType(Name, pre);
			foreach (UnionAlternative a in this.Alternatives)
			{
				UnionAlternative aClone = new UnionAlternative(a.DataType.Clone());
				u.Alternatives.Add(aClone);
			}
			return u;
		}

		public UnionAlternative FindAlternative(DataType dtOrig)
		{
			foreach (UnionAlternative alt in Alternatives)
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

		public override int Size
		{
			get
			{
				int size = 0;
				foreach (UnionAlternative alt in Alternatives)
				{
					size = Math.Max(size, alt.DataType.Size);
				}
				return size;
			}
			set { ThrowBadSize(); }
		}

		public DataType Simplify()
		{
			if (Alternatives.Count == 1)
			{
				return Alternatives[0].DataType;
			}
			else
				return this;
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("(union");
			if (name != null)
			{
				writer.Write(" \"{0}\"", name);
			}
			int i = 0;
			foreach (UnionAlternative alt in Alternatives)
			{
				writer.Write(" (");
				alt.DataType.Write(writer);
				writer.Write(" {0})", alt.MakeName(i));
				++i;
			}
			writer.Write(")");
		}
	}

	public class UnionAlternative
	{
		public DataType DataType;
		public string Name;

		public UnionAlternative(DataType t)
		{
			this.DataType = t;
		}

		public UnionAlternative(string name, DataType dt)
		{
			DataType = dt;
			Name = name;
		}

		public string MakeName(int i)
		{
			if (Name == null)
				return string.Format("u{0}", i);
			else
				return Name;
		}
	}

	public class UnionAlternativeCollection : IDictionary
	{
		private SortedList innerList;

		public UnionAlternativeCollection()
		{
			innerList = new SortedList(new DataTypeComparer());
		}

		public UnionAlternative this[int i]
		{
			get { return (UnionAlternative) innerList.GetByIndex(i); }
		}

		object IDictionary.this[object dt]
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}
	
		/// <summary>
		/// Adds an alternative to the collection.
		/// </summary>
		/// <param name="a"></param>
		public void Add(UnionAlternative a)
		{
			innerList[a.DataType] = a;
		}

		public void Add(DataType dt)
		{
			Add(new UnionAlternative(dt));
		}

		public void CopyTo(Array a, int i)
		{
			innerList.Values.CopyTo(a, i);
		}

		public int Count
		{
			get { return innerList.Count; }
		}

		public bool IsSynchronized
		{
			get { return innerList.IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return innerList.SyncRoot; }
		}

		void IDictionary.Add(object dt, object ua)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			innerList.Clear();
		}

		public bool Contains(object o)
		{
			return innerList.Contains(o);
		}

		public IEnumerator GetEnumerator()
		{
			return innerList.Values.GetEnumerator();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			throw new NotSupportedException();
		}

		public bool IsFixedSize
		{
			get { return innerList.IsFixedSize; }
		}

		public bool IsReadOnly
		{
			get { return innerList.IsReadOnly; }
		}

		public ICollection Keys
		{
			get { return innerList.Keys; }
		}

		void IDictionary.Remove(object dt)
		{
			throw new NotSupportedException();
		}

		public ICollection Values
		{
			get { return innerList.Values; }
		}
	}
}
