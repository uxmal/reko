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
        [XmlText]
        public string TypeName;

        [XmlElement]
        public string[] Scope;
        
        [XmlElement("tyArg")]
        public SerializedType[] TypeArguments;

        public TypeReference_v1()
        {
        }

        public TypeReference_v1(string typeName)
        {
            this.TypeName = typeName;
        }

        public TypeReference_v1(string[] scope, string typeName, SerializedType[] typeArgs)
        {
        }

        public TypeReference_v1(string [] scope, string typeName)
        {
            this.TypeName = typeName;
        }

        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitTypeReference(this);
        }

        public override string ToString()
        {
            return TypeName;
        }
    }
}
