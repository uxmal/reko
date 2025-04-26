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
using System.IO;
using System.Linq;

namespace Reko.Core.Types
{
	/// <summary>
	/// Represents a memory structure type, consisting of several Fields.
	/// </summary>
	public class StructureType : CompositeType
	{
        /// <summary>
        /// Constructs a structure instance.
        /// </summary>
        public StructureType() : this(null, 0, false)
		{
		}

        /// <summary>
        /// Constructs a structure with a known size.
        /// </summary>
        /// <param name="size">The size of the structure in
        /// storage units, or 0 if the size is unknown.
        /// </param>
        public StructureType(int size) : this(null, size, false)
        {
        }

        /// <summary>
        /// Constructs a structure with a known size and name.
        /// </summary>
        /// <param name="name">Optional name.</param>
        /// <param name="size">The size of the structure in storage units,
        /// or 0 if the size is unknown.</param>
		public StructureType(string? name, int size) : this(name, size, false)
		{
		}

        /// <summary>
        /// Constructs a structure with a known size and name.
        /// </summary>
        /// <param name="name">Optional name.</param>
        /// <param name="size">The size of the structure in storage units,
        /// or 0 if the size is unknown.</param>
        /// <param name="userDefined">True if the structure is user-defined; false otherwise.
        /// </param>
        public StructureType(string? name, int size, bool userDefined) 
            : base(Domain.Structure, name)
        {
            this.UserDefined = userDefined;
            this.Fields = [];
            this.Size = size;
        }

        /// <inheritdoc />
        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitStructure(this);
        }

        /// <inheritdoc />
        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitStructure(this);
        }

        /// <summary>
        /// Clones the structure type. The clone is a deep copy of the structure type.
        /// </summary>
        /// <param name="clonedTypes"></param>
        /// <returns>A cloned copy of the structure.</returns>
        public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
		{
            clonedTypes ??= new Dictionary<DataType, DataType>();
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

        /// <summary>
        /// The fields of the structure, ordered by their offset.
        /// </summary>
        public StructureFieldCollection Fields { get; private set; }

        /// <inheritdoc />
        public override bool IsComplex => true;

        /// <summary>
        /// If true, then this structure can never be simplfied by Simplify().
        /// </summary>
        public bool ForceStructure { get; set; }
        
        /// <summary>
        /// If true, the structure is an Intel-style segment. In particular,
        /// segments must never be converted to primitive types.
        /// </summary>
        public bool IsSegment { get ; set; }

        /// <summary>
        /// Returns true if the structur has no known size and zero fields.
        /// </summary>
        public bool IsEmpty => Size == 0 && Fields.Count == 0;

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
        /// <returns>The exact size if known, otherwise the inferred size
        /// based on data type analysis.
        /// </returns>
        public int GetInferredSize()
        {
            if (Size > 0)
                return Size;
            if (Fields.Count == 0)
                return 0;
            var maxField = Fields.Last();
            return maxField.Offset + maxField.DataType.Size;    //$BUG: nested structs?
        }

        /// <summary>
        /// Simplifies the structure type. If the structure type has only one field, and
        /// it is at offset 0, return that field's type.
        /// </summary>
        /// <returns>If the structure can be simplified, returns the type of the
        /// field; otherwise return the whole structure.
        /// </returns>
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
