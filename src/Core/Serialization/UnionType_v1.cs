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
    /// Serialization of <see cref="UnionType"/>.
    /// </summary>
    public class UnionType_v1 : SerializedTaggedType
    {
        /// <summary>
        /// Size of the union in bytes, if known.
        /// </summary>
        [XmlAttribute("size")]
        [DefaultValue(0)]
        public int ByteSize;

        /// <summary>
        /// Constructs an empty union type.
        /// </summary>
        public UnionType_v1()
        {
        }

        /// <summary>
        /// Union alternatives.
        /// </summary>
        [XmlElement("alt", typeof(UnionAlternative_v1))]
        public UnionAlternative_v1[]? Alternatives;

        /// <inheritdoc/>
        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitUnion(this);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("union({0}", ByteSize);
            if (Alternatives is not null)
            {
                foreach (UnionAlternative_v1 alt in Alternatives)
                {
                    sb.AppendFormat(", ({0}, {1})", alt.Name is not null ? alt.Name : "?", alt.Type);
                }
                sb.Append(")");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Serialized representation of a union alternative.
    /// </summary>
    public class UnionAlternative_v1
    {
        /// <summary>
        /// Name of the union alternative.
        /// </summary>
        [XmlAttribute("name")]
        public string? Name;

        /// <summary>
        /// Type of the union alternative.
        /// </summary>
        public SerializedType? Type;

        /// <summary>
        /// Constructs an empty instance of <see cref="UnionAlternative_v1"/>.
        /// </summary>
        public UnionAlternative_v1()
        {
        }

        /// <summary>
        /// Constructs an instance of <see cref="UnionAlternative_v1"/> with the given name and type.
        /// </summary>
        /// <param name="name">Name of the alternative.</param>
        /// <param name="type">Type of the alternative.</param>
        public UnionAlternative_v1(string name, SerializedType type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
