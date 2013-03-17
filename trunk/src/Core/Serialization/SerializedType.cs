#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Types;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Reflection;

namespace Decompiler.Core.Serialization
{
    /// <summary>
    /// Abstract base class for serialized types.
    /// </summary>
    public abstract class SerializedType
    {
        public SerializedType()
        {
        }

        public abstract DataType BuildDataType(TypeFactory factory);

        public static XmlAttributeOverrides GetAttributeOverrides(IEnumerable<Type> typesToDecorate)
        {
            var overrides = typesToDecorate
                .SelectMany(x => x.GetFields())
                .Select(f => new
                {
                    Field = f,
                    AttrCreator = GetAttributeCreator(f)
                })
                .Where(f => f.AttrCreator != null)
                .Aggregate(
                    new XmlAttributeOverrides(),
                    (ov, field) =>
                    { 
                        ov.Add(field.Field.DeclaringType, field.Field.Name, field.AttrCreator()); 
                        return ov;
                    });
            return overrides;
        }

        private static Func<XmlAttributes> GetAttributeCreator(FieldInfo f)
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

        private static XmlAttributes CreateElementAttributes()
        {
            var sertypeAttributes = new XmlAttributes
            {
                XmlElements = 
                {
                    new XmlElementAttribute("prim", typeof(SerializedPrimitiveType)),
                    new XmlElementAttribute("ptr", typeof(SerializedPointerType)),
                    //new XmlElementAttribute("arr", typeof(SerializedArrayType)),
                    new XmlElementAttribute("struct", typeof(SerializedStructType)),
                    //new XmlElementAttribute("union", typeof(SerializedUnionType)),
                    new XmlElementAttribute("fn", typeof(SerializedSignature)),
                    new XmlElementAttribute("typedef", typeof(SerializedTypedef))
                }
            };
            return sertypeAttributes;
        }

        private static XmlAttributes CreateArrayElementAttributes()
        {
            var sertypeAttributes = new XmlAttributes
            {
                XmlArrayItems = 
                {
                    new XmlArrayItemAttribute("prim", typeof(SerializedPrimitiveType)),
                    new XmlArrayItemAttribute("ptr", typeof(SerializedPointerType)),
                    //new XmlArrayItemAttribute("arr", typeof(SerializedArrayType)),
                    new XmlArrayItemAttribute("struct", typeof(SerializedStructType)),
                    //new XmlArrayItemAttribute("union", typeof(SerializedUnionType)),
                    new XmlArrayItemAttribute("fn", typeof(SerializedSignature)),
                    new XmlArrayItemAttribute("typedef", typeof(SerializedTypedef))
                }
            };
            return sertypeAttributes;
        }
    }

    public static class CollectionExtension
    {
        public static void AddRange(this IList list, IEnumerable items)
        {
            foreach (var item in items)
                list.Add(item);
        }
    }
}