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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Refers to another type by name only. Requires an external symbol table to
    /// resolve the type.
    /// </summary>
    public class TypeReference_v1 : SerializedType
    {
        /// <summary>
        /// Name of the type.
        /// </summary>
        [XmlText]
        public string? TypeName;

        /// <summary>
        /// Scope or namespace of the time.
        /// </summary>
        [XmlElement]
        public string[]? Scope;
        
        /// <summary>
        /// Generic type arguments if any.
        /// </summary>
        [XmlElement("tyArg")]
        public SerializedType[]? TypeArguments;

        /// <summary>
        /// Creates an empty instance of <see cref="TypeReference_v1"/>.
        /// </summary>
        public TypeReference_v1()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="TypeReference_v1"/> in 
        /// the global namespace.
        /// </summary>
        public TypeReference_v1(string typeName)
        {
            this.TypeName = typeName;
        }

        /// <summary>
        /// Creates an instance of <see cref="TypeReference_v1"/>.
        /// </summary>
        /// <param name="scope">Scope or namespace.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="typeArgs">Generic type arguments.</param>
        public TypeReference_v1(string[] scope, string typeName, SerializedType[] typeArgs)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="TypeReference_v1"/>.
        /// </summary>
        /// <param name="scope">Scope or namespace.</param>
        /// <param name="typeName">Name of the type.</param>
        public TypeReference_v1(string [] scope, string typeName)
        {
            this.TypeName = typeName;
        }

        /// <inheritdoc/>
        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitTypeReference(this);
        }

        /// <summary>
        /// Returns a string representation of the type reference.
        /// </summary>
        public override string ToString()
        {
            return TypeName ?? "";
        }
    }
}
