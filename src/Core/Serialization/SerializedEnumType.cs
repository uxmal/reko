#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Serialization format of an enumerated type.
    /// </summary>
    public class SerializedEnumType : SerializedTaggedType
    {
        /// <summary>
        /// Size in storage units of the enum type values.
        /// </summary>
        [XmlAttribute("size")]
        [DefaultValue(0)]
        public int Size;

        /// <summary>
        /// Type domain for the values -- typically <see cref="Domain.SignedInt"/> or
        /// <see cref="Domain.UnsignedInt"/>.
        /// </summary>
        [XmlAttribute("Domain")]
        [DefaultValue(Domain.None)]
        public Domain Domain;

        /// <summary>
        /// The collection of values of this enumerated type.
        /// </summary>
        [XmlElement("member")]
        public SerializedEnumValue[]? Values;

        /// <summary>
        /// Creates an uninitialized serialized enum type.
        /// </summary>
        public SerializedEnumType()
        {
        }

        /// <summary>
        /// Constructs a serialized enum type.
        /// </summary>
        /// <param name="size">The size of the enum type values.</param>
        /// <param name="domain">Type domain for the values -- typically <see cref="Domain.SignedInt"/>
        /// or <see cref="Domain.UnsignedInt"/>.
        /// </param>
        /// <param name="name">Name of the enumerated type.</param>
        public SerializedEnumType(int size, Types.Domain domain, string name)
        {
            this.Size = size;
            this.Domain = domain;
            this.Name = name;
        }

        /// <inheritdoc/>
        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitEnum(this);
        }
    }

    /// <summary>
    /// Serialized value of an enumerated type.
    /// </summary>
    public class SerializedEnumValue
    {
        /// <summary>
        /// Name of the value.
        /// </summary>
        [XmlAttribute("name")]
        public string? Name;

        /// <summary>
        /// The value of the item.
        /// </summary>
        [XmlAttribute("value")]
        public int Value;
    }
}
