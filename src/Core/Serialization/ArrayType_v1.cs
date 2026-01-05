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

using System.ComponentModel;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Serialized representation of an array type.
    /// </summary>
    public class ArrayType_v1 : SerializedType
    {
        /// <summary>
        /// The type of the elements in the array.
        /// </summary>
        public SerializedType? ElementType;

        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        [XmlAttribute("length")]
        [DefaultValue(0)]
        public int Length;

        /// <inheritdoc/>
        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitArray(this);
        }

        /// <summary>
        /// Returns a string representation of the array type.
        /// </summary>
        public override string ToString()
        {
            return string.Format("arr({0},{1})", ElementType, Length);
        }
    }
}
