#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
    [XmlRoot(ElementName = "library", Namespace = SerializedLibrary.Namespace_v1)]
    public class SerializedLibrary
    {
        public const string Namespace_v2 = "http://schemata.jklnet.org/Decompiler/v2";
        public const string Namespace_v1 = "http://schemata.jklnet.org/Decompiler";

        [XmlAttribute("module")]
        public string ModuleName;

        [XmlAttribute("case")]
        public string Case;

        [XmlElement("default")]
        public SerializedLibraryDefaults Defaults;

        [XmlArray("types")]
        public SerializedType[] Types;

        [XmlElement("procedure", typeof(Procedure_v1))]
        [XmlElement("service", typeof(SerializedService))]
        public List<SerializedProcedureBase_v1> Procedures;

        private static XmlSerializer serializer;

        public SerializedLibrary()
        {
            this.Procedures = new List<SerializedProcedureBase_v1>();
        }

        public static SerializedLibrary LoadFromStream(Stream stm)
        {
            var ser = CreateSerializer();
            var rdr = new XmlTextReader(stm);
            return (SerializedLibrary) ser.Deserialize(rdr);
        }

        public static XmlSerializer CreateSerializer()
        {
            if (serializer == null)
            {
                serializer = CreateSerializer_v1(typeof(SerializedLibrary));
            }
            return serializer;
        }

        public static XmlSerializer CreateSerializer_v2(Type rootType)
        {
            var attrOverrides = SerializedType.GetAttributeOverrides(TypesToDecorate, Namespace_v2);
            return new XmlSerializer(rootType, attrOverrides);
        }

        public static XmlSerializer CreateSerializer_v1(Type rootType)
        {
            var attrOverrides = SerializedType.GetAttributeOverrides(TypesToDecorate, Namespace_v1);
            return new XmlSerializer(rootType, attrOverrides);
        }

        private static Type[] TypesToDecorate = new Type[] 
        {
            typeof(PrimitiveType_v1),
            typeof(PointerType_v1),
            typeof(ArrayType_v1),
            typeof(CodeType_v1),
            typeof(SerializedEnumType),
            typeof(SerializedStructType),
            typeof(StructField_v1),
            typeof(UnionType_v1),
            typeof(SerializedUnionAlternative),
            typeof(StringType_v2),
            typeof(SerializedSignature),
            typeof(SerializedTypedef),
            typeof(SerializedLibrary),
            typeof(Argument_v1),
            typeof(GlobalDataItem_v2)
        };
    }
}
