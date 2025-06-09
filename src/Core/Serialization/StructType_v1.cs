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

using Reko.Core.Types;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Serialization format for <see cref="StructureType"/>.
    /// </summary>
	public class StructType_v1 : SerializedTaggedType
	{
        /// <summary>
        /// Specified size of the structure in storage units.
        /// </summary>
		[XmlAttribute("size")]
        [DefaultValue(0)]
		public int ByteSize;

        /// <summary>
        /// Disallow simplification of this structure type.
        /// </summary>
        [XmlAttribute("force")]
        [DefaultValue(false)]
        public bool ForceStructure;

        /// <summary>
        /// Construct an uninitialized instance of <see cref="StructType_v1"/>.
        /// </summary>
        public StructType_v1()
		{
		}

        /// <summary>
        /// Collections of fields.
        /// </summary>
		[XmlElement("field", typeof (StructField_v1))]
		public StructField_v1[]? Fields;

        /// <inheritdoc/>
        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitStructure(this);
        }

        /// <inheritdoc/>
		public override string ToString()
		{
			var sb = new StringBuilder("struct(");
            if (!string.IsNullOrEmpty(Name))
                sb.AppendFormat("{0}, ", Name);
            if (ByteSize > 0)
                sb.AppendFormat("{0}, ", ByteSize);
            if (Fields is not null)
            {
                foreach (StructField_v1 f in Fields)
                {
                    sb.AppendFormat("({0}, {1}, {2})", f.Offset, f.Name ?? "?", f.Type);
                }
            }
            sb.Append(")");
			return sb.ToString();
		}
	}

    /// <summary>
    /// Serialization format for a structure field.
    /// </summary>
	public class StructField_v1
	{
        /// <summary>
        /// Field offset.
        /// </summary>
		[XmlAttribute("offset")]
		public int Offset;

        /// <summary>
        /// Field name.
        /// </summary>
		[XmlAttribute("name")]
		public string? Name;

        /// <summary>
        /// Field type.
        /// </summary>
		[XmlElement("prim", typeof (PrimitiveType_v1))]
		[XmlElement("ptr", typeof (PointerType_v1))]
		public SerializedType? Type;

        /// <summary>
        /// Construct an uninitialized instance of <see cref="StructField_v1"/>.
        /// </summary>
		public StructField_v1()
		{
		}

        /// <summary>
        /// Construct an instance of <see cref="StructField_v1"/>.
        /// </summary>
        /// <param name="offset">Field offset.</param>
        /// <param name="name">Field name.</param>
        /// <param name="type">Field data type.</param>
		public StructField_v1(int offset, string name, SerializedType type)
		{
			this.Offset = offset;
			this.Name = name;
			this.Type = type;
		}
	}
}
