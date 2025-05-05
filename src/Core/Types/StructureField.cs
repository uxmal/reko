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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;

namespace Reko.Core.Types
{
    /// <summary>
    /// Describes a field of a structure.
    /// //$REVIEW: investigate the similarities with ImageMapItem.
    /// </summary>
	public class StructureField : Field
	{
        /// <summary>
        /// Constructs a new <see cref="StructureField"/> instance.
        /// </summary>
        /// <param name="offset">Offset of the field.</param>
        /// <param name="type">Data type of the field.</param>
        /// <param name="name">Optional field name.</param>
		public StructureField(int offset, DataType type, string? name = null) : base(type)
		{
            this.Offset = offset;
            this.name = name;
		}

        /// <summary>
        /// The name of this field.
        /// </summary>
        public override string Name
        {
            get { return name ?? DefaultName(); }
            set { name = value; }
        }

        /// <summary>
        /// True if the name is user-defined.
        /// </summary>
        public bool IsNameSet => name is not null;

        private string? name;

        //$TODO: make offsets long, to handle the situation where the Program
        // globals struct is maintaining 64-bit addresses.
        /// <summary>
        /// Signed field offset (in storage units) from the start of the structure.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Clones this structure field.
        /// </summary>
        /// <param name="clonedTypes">Dictionary to use when cloning types.</param>
        /// <returns></returns>
        public StructureField Clone(IDictionary<DataType, DataType>? clonedTypes = null)
		{
			return new StructureField(Offset, DataType.Clone(clonedTypes), name);
		}

        private string DefaultName()
        {
            return NamingPolicy.Instance.Types.StructureFieldName(this, this.name);
        }

        /// <summary>
        /// Given a constant, returns the offset as an integer.
        /// </summary>
        /// <param name="offset">Offset as a <see cref="Constant"/>.</param>
        /// <returns>An integer offset, or null if the offset wasn't an integer.
        /// </returns>
        public static int? ToOffset(Constant? offset)
        {
            if (offset is null)
                return 0;
            PrimitiveType? pt = offset.DataType.ResolveAs<PrimitiveType>();
            if (pt is null)
                return null;
            if (pt.Domain == Domain.SignedInt)
                return offset.ToInt32();
            else if (pt.Domain == Domain.Real)
                return null;
            else
                return (int) offset.ToUInt32();
        }

        /// <summary>
        /// Returns a string representation of this field.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0:X}: {1}: {2}", Offset, Name, DataType);
        }
	}

    /// <summary>
    /// Collection of <see cref="StructureField"/> instances.
    /// </summary>
	public class StructureFieldCollection : ICollection<StructureField>
	{
        private const int BinarySearchLimit = 0;

        private readonly List<StructureField> innerList = new List<StructureField>();

        /// <summary>
        /// Gets the field at position <paramref name="i"/> in the collection, <em>not</em>
        /// the offset.
        /// </summary>
        /// <param name="i">Index into the collection.</param>
        /// <returns>The field of that index.</returns>
        public StructureField this[int i]
        {
            get { return innerList[i]; }
        }

        /// <summary>
        /// Adds a field to the collection.
        /// </summary>
        /// <param name="offset">Offset for the field.</param>
        /// <param name="dt">Data type of the field.</param>
        /// <returns>The new structure field.</returns>
		public StructureField Add(int offset, DataType dt)
		{
			return Add(new StructureField(offset, dt));
		}

        /// <summary>
        /// Adds an (optinally named) field to the collection.
        /// </summary>
        /// <param name="offset">Offset for the field.</param>
        /// <param name="dt">Data type of the field.</param>
        /// <param name="name">Optional name for the field.</param>
        /// <returns>The new structure field.</returns>
		public StructureField Add(int offset, DataType dt, string? name)
		{
			return Add(new StructureField(offset, dt, name));
		}

        //$PERF: slow, should use binary search.
        /// <summary>
        /// Adds a field to the collection.
        /// </summary>
        /// <param name="f">Field to add.</param>
        /// <returns>The added field.</returns>
		public StructureField Add(StructureField f)
		{
			int i;
            int c = innerList.Count;
            if (c >= BinarySearchLimit)
            {
                for (i = 0; i < innerList.Count; ++i)
                {
                    var ff = innerList[i];
                    if (f.Offset == ff.Offset)
                    {
                        if (f.DataType == ff.DataType)
                            return ff;
                    }
                    if (f.Offset <= ff.Offset)
                        break;
                }
                innerList.Insert(i, f);
                return f;
            }
            else
            {
                i = 0;
                {
                    int iMax = c;
                    int iMin = 0;
                    while (iMin < iMax)
                    {
                        i = iMin + (iMax - iMin) / 2;
                        var ff = innerList[i];
                        if (ff.Offset > f.Offset)
                        {
                            iMax = i;
                        }
                        else
                        {
                            iMin = i + 1;
                        }
                    }
                    if (i < c)
                    {
                        var fff = innerList[i];
                        if (f.Offset == fff.Offset)
                        {
                            if (f.DataType == fff.DataType)
                                return fff;
                            i = i + 1;
                        }
                        else
                        {
                            i = iMax;
                        }
                    }
                }
                innerList.Insert(i, f); //$PERF: slow...
                return f;
            }
		}

        private void Dump(int iFail)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("== Order violated ==");
            for (int i = 0; i < innerList.Count; ++i)
            {
                var f = innerList[i];
                sb.AppendFormat("{0} {1:X8} {2}", i == iFail ? "*" : " ", f.Offset, f.Name);
                sb.AppendLine();
            }
            throw new Exception(sb.ToString());
        }

        void ICollection<StructureField>.Add(StructureField f)
        {
            Add(f);
        }

        /// <summary>
        /// Adds a range of fields to the collection.
        /// </summary>
        /// <param name="fields">Fields to add.</param>
        public void AddRange(IEnumerable<StructureField> fields)
        {
            foreach (var field in fields)
            {
                Add(field);
            }
        }

        /// <summary>
        /// Returns the structure field exactly located at the specified offset.
        /// </summary>
        /// <param name="offset">Offset (in bytes) of the field to retrieve.</param>
        /// <returns>The requested StructureField if it exists at <paramref>offset</paramref>, otherwise null.</returns>
        public StructureField? AtOffset(int offset)
        {
            if (innerList.Count >= BinarySearchLimit)
            {
                foreach (StructureField f in innerList)
                {
                    if (f.Offset == offset)
                        return f;
                }
                return null;
            }
            else
            {
                int iMin = 0;
                int iMax = innerList.Count - 1;
                while (iMin <= iMax)
                {
                    int iMid = iMin + (iMax - iMin) / 2;
                    var f = innerList[iMid];
                    int cmp = f.Offset - offset;
                    if (f.Offset == offset)
                        return f;
                    else if (f.Offset < offset)
                    {
                        iMin = iMid + 1;
                    }
                    else
                    {
                        iMax = iMid - 1;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the field with the highest offset that is less than or equal to the
        /// specified <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
		public StructureField? LowerBound(int offset)
		{
			StructureField? fPrev = null;
			foreach (StructureField f in innerList)
			{
				if (f.Offset <= offset)
					fPrev = f;
			}
			return fPrev;
		}

        /// <summary>
        /// Removes the field at the specified index.
        /// </summary>
        /// <param name="i">Index of field to remove.</param>
        public void RemoveAt(int i)
        {
            innerList.RemoveAt(i);
        }

        #region ICollection<StructureField> Members


        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            innerList.Clear();
        }

        /// <summary>
        /// Returns true if the collection contains the specified field.
        /// </summary>
        /// <param name="item">Field to look for.</param>
        /// <returns>True if the field is contained in the collection; otherwise false.</returns>
        public bool Contains(StructureField item)
        {
            return innerList.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(StructureField[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the number of fields in the collection.
        /// </summary>
        public int Count
        {
            get { return innerList.Count; }
        }

        /// <summary>
        /// Returns true if the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the specified field from the collection.
        /// </summary>
        /// <param name="item">Field to remove.</param>
        /// <returns>True if the field was found and removed; otherwise false.
        /// </returns>
        public bool Remove(StructureField item)
        {
            return innerList.Remove(item);
        }

        #endregion

        #region IEnumerable<StructureField> Members

        /// <summary>
        /// Returns an enumerator that iterates through the  fields in the collection.
        /// </summary>
        public IEnumerator<StructureField> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion
    }
}
