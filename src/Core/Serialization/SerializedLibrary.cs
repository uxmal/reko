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
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    [XmlRoot(ElementName = "library", Namespace = SerializedLibrary.Namespace_v1)]
    public class SerializedLibrary
    {
        public const string Namespace_v5 = "http://schemata.jklnet.org/Reko/v5";
        public const string Namespace_v4 = "http://schemata.jklnet.org/Reko/v4";
        public const string Namespace_v3 = "http://schemata.jklnet.org/Reko/v3";
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
        public List<ProcedureBase_v1> Procedures;

        [XmlElement("global", typeof(GlobalVariable_v1))]
        public List<GlobalVariable_v1> Globals;

        private static XmlSerializer serializer;

        public SerializedLibrary()
        {
            this.Procedures = new List<ProcedureBase_v1>();
            this.Globals = new List<GlobalVariable_v1>();
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
                serializer = CreateSerializer(typeof(SerializedLibrary), Namespace_v1);
            }
            return serializer;
        }

        public static XmlSerializer CreateSerializer(Type rootType, string @namespace)
        {
            var attrOverrides = SerializedType.GetAttributeOverrides(TypesToDecorate, @namespace);
            return new XmlSerializer(rootType, attrOverrides);
        }

        public static XmlSerializer CreateSerializer_v5(Type rootType)
        {
            return CreateSerializer(rootType, Namespace_v5);
        }

        public static XmlSerializer CreateSerializer_v4(Type rootType)
        {
            return CreateSerializer(rootType, Namespace_v4);
        }

        public static XmlSerializer CreateSerializer_v3(Type rootType)
        {
            return CreateSerializer(rootType, Namespace_v3);
        }

        public static XmlSerializer CreateSerializer_v2(Type rootType)
        {
            return CreateSerializer(rootType, Namespace_v2);
        }

        public static XmlSerializer CreateSerializer_v1(Type rootType)
        {
            return CreateSerializer(rootType, Namespace_v1);
        }

        private static Type[] TypesToDecorate = new Type[] 
        {
            typeof(PrimitiveType_v1),
            typeof(PointerType_v1),
            typeof(ArrayType_v1),
            typeof(CodeType_v1),
            typeof(SerializedEnumType),
            typeof(StructType_v1),
            typeof(StructField_v1),
            typeof(UnionType_v1),
            typeof(UnionAlternative_v1),
            typeof(StringType_v2),
            typeof(SerializedSignature),
            typeof(SerializedTypedef),
            typeof(SerializedLibrary),
            typeof(MemoryMap_v1),
            typeof(Argument_v1),
            typeof(GlobalVariable_v1),
            typeof(GlobalDataItem_v2),
        };
    }

    /// <summary>
    /// Defines a global variable in a library.
    /// </summary>
    public class GlobalVariable_v1
    {
        public const int NoOrdinal = -1;

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("ordinal")]
        [DefaultValue(NoOrdinal)]
        public int Ordinal;
    
        public SerializedType Type;
    }
}
