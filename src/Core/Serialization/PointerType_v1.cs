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

using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Serialized representation of serialized pointer types.
    /// </summary>
	public class PointerType_v1 : SerializedType
	{
        /// <summary>
        /// Data type of the pointer's pointee.
        /// </summary>
		public SerializedType? DataType;

        /// <summary>
        /// The size of the pointer, in storage units.
        /// </summary>
        [XmlAttribute("size")]
        [DefaultValue(0)]
        public int PointerSize;

        /// <summary>
        /// Constructs an empty instance of <see cref="PointerType_v1"/>.
        /// </summary>
		public PointerType_v1()
		{
		}

        /// <summary>
        /// Constructs an instance of <see cref="PointerType_v1"/>.
        /// </summary>
        /// <param name="pointee">Data type of the pointer's pointee.</param>
        /// <param name="pointerSize">Size of the pointer, in storage units.</param>
		public PointerType_v1(SerializedType pointee, int pointerSize)
		{
			DataType = pointee;
            PointerSize = pointerSize;
		}

        /// <summary>
        /// Creates a serialized pointer type.
        /// </summary>
        /// <param name="dt">Pointee type.</param>
        /// <param name="n">Pointer size in storage units.
        /// </param>
        /// <returns></returns>
        public static SerializedType Create(SerializedType dt, int n)
        {
            return new PointerType_v1(dt, n);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitPointer(this);
        }

        /// <inheritdoc/>
		public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("ptr({0}", DataType);
            WriteQualifier(Qualifier, sb);
            sb.Append(")");
            return sb.ToString();
        }
    }
}
