#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.IO;

namespace Decompiler.Core.Types
{
	/// <summary>
	/// Represents a memory structure type, consisting of several Fields.
	/// </summary>
	public class StructureType : DataType
	{
		public StructureType() : this(null, 0)
		{
		}

        public StructureType(int size) : this(null, size)
        {
        }

		public StructureType(string name, int size) : base(name)
		{
			this.Fields = new StructureFieldCollection();
			this.Size = size; 
		}

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitStructure(this);
        }

		public override DataType Clone()
		{
			var s = new StructureType(Name, Size);
			s.IsSegment = IsSegment;
			foreach (StructureField f in Fields)
			{
				s.Fields.Add(f.Clone());
			}
			return s;
		}

		public StructureFieldCollection Fields { get; private set; }
		public override bool IsComplex  { get { return true; } }

		/// <summary>
		/// If true, the structure is an Intel-style segment. In particular, segments must never be converted to 
		/// primitive types.
		/// </summary>
		public bool IsSegment { get ; set; }
        public bool IsEmpty { get { return Size == 0 && Fields.Count == 0; } }
		public override int Size { get; set; }

        public DataType Simplify()
        {
            if (Fields.Count == 1 && !IsSegment)
            {
                StructureField f = Fields[0];
                if (f.Offset == 0 && (Size == 0 || Size == f.DataType.Size))
                {
                    return f.DataType;
                }
            }
            return this;
        }
	}
}
