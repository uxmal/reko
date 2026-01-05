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
using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
	/// <summary>
	/// Serialized representation of serialized primitive types.
	/// </summary>
	public class PrimitiveType_v1 : SerializedType
	{
        /// <summary>
        /// Type domain of the primitive type.
        /// </summary>
		[XmlAttribute("domain")]
		public Domain Domain;

        /// <summary>
        /// Size in bytes of the data type.
        /// </summary>
		[XmlAttribute("size")]
		public int ByteSize;

        /// <summary>
        /// Default constructor.
        /// </summary>
		public PrimitiveType_v1()
		{
		}

        /// <summary>
        /// Creates a boolean type.
        /// </summary>
        /// <param name="byteSize">Size of the type.</param>
        public static SerializedType Bool(int byteSize = 1)
        {
            return new PrimitiveType_v1 { Domain = Domain.Boolean, ByteSize = byteSize };
        }

        /// <summary>
        /// Creates an eight-bit character type.
        /// </summary>
        public static SerializedType Char8()
        {
            return new PrimitiveType_v1 { Domain = Domain.Character, ByteSize = 1 };
        }

        /// <summary>
        /// Creates a 16-bit character type.
        /// </summary>
        public static SerializedType WChar16()
        {
            return new PrimitiveType_v1 { Domain = Domain.Character, ByteSize = 2 };
        }

        /// <summary>
        /// Creates an 8-bit signed integer.
        /// </summary>
        public static SerializedType SChar8()
        {
            return new PrimitiveType_v1 { Domain = Domain.Character | Domain.SignedInt, ByteSize = 1 };
        }

        /// <summary>
        /// Creates an 8-bit unsigned integer.
        /// </summary>
        public static SerializedType UChar8()
        {
            return new PrimitiveType_v1 { Domain = Domain.Character|Domain.UnsignedInt, ByteSize = 1 };
        }

        /// <summary>
        /// Creates an 16-bit signed integer.
        /// </summary>
        public static SerializedType Int16()
        {
            return new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 2 };
        }

        /// <summary>
        /// Creates an 16-bit unsigned integer.
        /// </summary>
        public static SerializedType UInt16()
        {
            return new PrimitiveType_v1 { Domain = Domain.UnsignedInt, ByteSize = 2 };
        }

        /// <summary>
        /// Creates an 32-bit signed integer.
        /// </summary>
        public static SerializedType Int32()
        {
            return new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 4 };
        }

        /// <summary>
        /// Creates an 32-bit unsigned integer.
        /// </summary>
        public static SerializedType UInt32()
        {
            return new PrimitiveType_v1 { Domain = Domain.UnsignedInt, ByteSize = 4 };
        }

        /// <summary>
        /// Creates an 64-bit signed integer.
        /// </summary>
        public static SerializedType Int64()
        {
            return new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 8 };
        }

        /// <summary>
        /// Creates an 64-bit unsigned integer.
        /// </summary>
        public static SerializedType UInt64()
        {
            return new PrimitiveType_v1 { Domain = Domain.UnsignedInt, ByteSize = 8 };
        }

        /// <summary>
        /// Creates a 32-bit floating point number.
        /// </summary>
        public static SerializedType Real32()
        {
            return new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 4 };
        }

        /// <summary>
        /// Creates a 64-bit floating point number.
        /// </summary>
        public static SerializedType Real64()
        {
            return new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 8 };
        }

        /// <summary>
        /// Creates a 80-bit floating point number.
        /// </summary>
        public static SerializedType Real80()
        {
            return new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 10 };
        }

        /// <summary>
        /// Creates a 32-bit pointer.
        /// </summary>
        public static SerializedType Ptr32()
        {
            return new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = 4 };
        }

        /// <summary>
        /// Constructs a primitive type with the specified domain and size.
        /// </summary>
        /// <param name="domain">Type domain.</param>
        /// <param name="byteSize">Byte size.</param>
        public PrimitiveType_v1(Domain domain, int byteSize)
		{
			this.Domain = domain;	
			this.ByteSize = byteSize;
		}

        /// <inheritdoc/>
        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitPrimitive(this);
        }

        /// <inheritdoc/>
		public override string ToString()
		{
            var sb = new StringBuilder();
			sb.AppendFormat("prim({0},{1}", Domain, ByteSize);
            base.WriteQualifier(Qualifier, sb);
            sb.Append(")");
            return sb.ToString();
		}
    }
}
