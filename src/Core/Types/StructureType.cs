#region License
/* 
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
using System.IO;
using System.Linq;

namespace Reko.Core.Types
{
	/// <summary>
	/// Represents a memory structure type, consisting of several Fields.
	/// </summary>
	public class StructureType : CompositeType
	{
        public StructureType() : this(null, 0, false)
		{
		}

        public StructureType(int size) : this(null, size, false)
        {
        }

		public StructureType(string name, int size) : this(name, size, false)
		{
		}

        public StructureType(string name, int size, bool userDefined) : base(name)
        {
            this.UserDefined = userDefined;
            this.Fields = new StructureFieldCollection();
            this.Size = size;
        }

        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitStructure(this);
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitStructure(this);
        }

        public override DataType Clone(IDictionary<DataType, DataType> clonedTypes)
		{
            if (clonedTypes == null)
                clonedTypes = new Dictionary<DataType, DataType>();
            if (clonedTypes.ContainsKey(this))
                return clonedTypes[this];
			var s = new StructureType(Name, Size);
            clonedTypes[this] = s;
            s.Qualifier = Qualifier;
            s.UserDefined = UserDefined;
			s.IsSegment = IsSegment;
            s.ForceStructure = ForceStructure;
			foreach (var f in Fields)
			{
				s.Fields.Add(f.Clone(clonedTypes));
			}
			return s;
		}

        public bool UserDefined { get; private set; }
        public StructureFieldCollection Fields { get; private set; }
		public override bool IsComplex  { get { return true; } }

        /// <summary>
        /// If true, then this structure can never be simplfied by Simplify().
        /// </summary>
        public bool ForceStructure { get; set; }
        
        /// <summary>
        /// If true, the structure is an Intel-style segment. In particular, segments must never be converted to 
        /// primitive types.
        /// </summary>
        public bool IsSegment { get ; set; }
        public bool IsEmpty { get { return Size == 0 && Fields.Count == 0; } }

        /// <summary>
        /// Specific size. This is set if the actual size of a structure is known
        /// (typically because it is an element of an array) or a user has specifically
        /// set the size to a value. Use GetInferredSize() to get the size based on
        /// what fields are present.
        /// </summary>
		public override int Size { get; set; }

        /// <summary>
        /// If the exact size is not known, compute the deferred size by finding 
        /// the field with the highest offset.
        /// </summary>
        /// <returns></returns>
        public int GetInferredSize()
        {
            if (Size > 0)
                return Size;
            if (Fields.Count == 0)
                return 0;
            var maxField = Fields.Last();
            return maxField.Offset + maxField.DataType.Size;    //$BUG: nested structs?
        }

        public DataType Simplify()
        {
            if (Fields.Count == 1 && !IsSegment && !ForceStructure)
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
