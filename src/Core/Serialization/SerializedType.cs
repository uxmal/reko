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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Abstract base class for serialized types.
    /// </summary>
    public abstract class SerializedType
    {
        /// <summary>
        /// The qualifier of the type. This is used to indicate whether the type
        /// is <c>const</c>, <c>volatile</c>, or <c>restricted</c>.
        /// </summary>
        [DefaultValue(Qualifier.None)]
        public Qualifier Qualifier;

        /// <summary>
        /// Accepts a visitor implementing the <see cref="ISerializedTypeVisitor{T}"/>
        /// interface. The visitor is expected to return a value of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor">Accepted visitor.</param>
        /// <returns>Value returned by visitor.</returns>
        public abstract T Accept<T>(ISerializedTypeVisitor<T> visitor);

        /// <summary>
        /// Generates attribute overrides for each field that holds a serialized
        /// DataType.
        /// </summary>
        /// <param name="typesToDecorate"></param>
        /// <param name="xmlNamespace"></param>
        /// <returns></returns>
        public static XmlAttributeOverrides GetAttributeOverrides(IEnumerable<Type> typesToDecorate, string xmlNamespace)
        {
            var overrides = typesToDecorate
                .SelectMany(x => x.GetFields())
                .Select(f => new
                {
                    Field = f,
                    AttrCreator = GetAttributeCreator(f)
                })
                .Where(f => f.AttrCreator is not null)
                .Aggregate(
                    new XmlAttributeOverrides(),
                    (ov, field) =>
                    { 
                        ov.Add(field.Field.DeclaringType!, field.Field.Name, field.AttrCreator!(xmlNamespace)); 
                        return ov;
                    });
            return overrides;
        }

        private static Func<string, XmlAttributes>? GetAttributeCreator(FieldInfo f)
        {
            if (f.FieldType == typeof(SerializedType))
                return CreateElementAttributes;
            if (!f.FieldType.IsArray)
                return null;
            if (f.FieldType.GetElementType() == typeof(SerializedType))
                return CreateArrayElementAttributes;
            else
                return null;
        }

        private static XmlAttributes CreateElementAttributes(string @namespace)
        {
            var sertypeAttributes = new XmlAttributes
            {
                XmlElements = 
                {
                    new XmlElementAttribute("prim", typeof(PrimitiveType_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("code", typeof(CodeType_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("ptr", typeof(PointerType_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("arr", typeof(ArrayType_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("enum", typeof(SerializedEnumType)) { Namespace = @namespace},
                    new XmlElementAttribute("str", typeof(StringType_v2)) { Namespace = @namespace},
                    new XmlElementAttribute("struct", typeof(StructType_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("union", typeof(UnionType_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("fn", typeof(SerializedSignature)) { Namespace = @namespace},
                    new XmlElementAttribute("typedef", typeof(SerializedTypedef)) { Namespace = @namespace},
                    new XmlElementAttribute("type", typeof(TypeReference_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("void", typeof(VoidType_v1)) { Namespace = @namespace},
                }
            };
            return sertypeAttributes;
        }

        private static XmlAttributes CreateArrayElementAttributes(string @namespace)
        {
            var sertypeAttributes = new XmlAttributes
            {
                XmlArrayItems = 
                {
                    new XmlArrayItemAttribute("prim", typeof(PrimitiveType_v1)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("ptr", typeof(PointerType_v1)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("arr", typeof(ArrayType_v1)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("enum", typeof(SerializedEnumType)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("str", typeof(StringType_v2)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("struct", typeof(StructType_v1)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("union", typeof(UnionType_v1)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("fn", typeof(SerializedSignature)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("typedef", typeof(SerializedTypedef)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("type", typeof(TypeReference_v1)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("void", typeof(VoidType_v1)) { Namespace = @namespace},
                }
            };
            return sertypeAttributes;
        }

        /// <summary>
        /// Writes the qualifiers of the type to the given <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="q">Qualifier to render.</param>
        /// <param name="sb">Output sink.</param>
        protected void WriteQualifier(Qualifier q, StringBuilder sb)
        {
            if ((q & Qualifier.Const) != 0)
            {
                sb.Append(",const");
            }
            if ((q & Qualifier.Volatile) != 0)
            {
                sb.Append(",volatile");
            }
            if ((q & Qualifier.Restricted) != 0)
            {
                sb.Append(",restricted");
            }
        }

    }

    /// <summary>
    /// Base class for serialized types that have a name, like <c>struct</c>
    /// or <c>union</c>.
    /// </summary>
    public abstract class SerializedTaggedType : SerializedType
    {
        /// <summary>
        /// Name of the tagged type.
        /// </summary>
        [XmlAttribute("name")]
        public string? Name;
    }
}