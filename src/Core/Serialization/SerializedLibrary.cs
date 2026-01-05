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
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Serialization format for a Reko type library
    /// </summary>
    [XmlRoot(ElementName = "library", Namespace = SerializedLibrary.Namespace_v1)]
    public class SerializedLibrary
    {
        /// <summary>
        /// Namespace to use for Reko file format 5.
        /// </summary>
        public const string Namespace_v5 = "http://schemata.jklnet.org/Reko/v5";

        /// <summary>
        /// Namespace to use for Reko file format 4.
        /// </summary>
        public const string Namespace_v4 = "http://schemata.jklnet.org/Reko/v4";

        /// <summary>
        /// Namespace to use for Reko file format 1.
        /// </summary>
        public const string Namespace_v1 = "http://schemata.jklnet.org/Decompiler";

        /// <summary>
        /// Module name of the library. This is the name of the module this library
        /// corresponds to.
        /// </summary>
        [XmlAttribute("module")]
        public string? ModuleName;

        /// <summary>
        /// Appears unused.
        /// </summary>
        [XmlAttribute("case")]
        public string? Case;

        /// <summary>
        /// Library defaults to use.
        /// </summary>
        [XmlElement("default")]
        public SerializedLibraryDefaults? Defaults;

        /// <summary>
        /// The data types of this library.
        /// </summary>
        [XmlArray("types")]
        public SerializedType[]? Types;

        /// <summary>
        /// The procedures and system services in this library.
        /// </summary>
        [XmlElement("procedure", typeof(Procedure_v1))]
        [XmlElement("service", typeof(SerializedService))]
        public List<ProcedureBase_v1> Procedures;

        /// <summary>
        /// Global variables of this library.
        /// </summary>
        [XmlElement("global", typeof(GlobalVariable_v1))]
        public List<GlobalVariable_v1> Globals;

        /// <summary>
        /// Annotations of this library.
        /// </summary>
        [XmlElement("annotation", typeof(Annotation_v3))]
        public List<Annotation_v3> Annotations;

        /// <summary>
        /// Image segments of this library.
        /// </summary>
        [XmlElement("segment", typeof(MemorySegment_v1))]
        public List<MemorySegment_v1> Segments;

        //$REVIEW: smell of singleton.
        private static XmlSerializer? serializer;

        /// <summary>
        /// Creates an empty instance of the <see cref="SerializedLibrary"/> class.
        /// </summary>
        public SerializedLibrary()
        {
            this.Procedures = new List<ProcedureBase_v1>();
            this.Globals = new List<GlobalVariable_v1>();
            this.Annotations = new List<Annotation_v3>();
            this.Segments = new List<MemorySegment_v1>();
        }

        /// <summary>
        /// Loads a <see cref="SerializedLibrary"/> instance from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stm"><see cref="Stream"/> to load the library from.</param>
        /// <returns>The serialized library.</returns>
        public static SerializedLibrary LoadFromStream(Stream stm)
        {
            var ser = CreateSerializer();
            var rdr = new XmlTextReader(stm);
            return (SerializedLibrary) ser.Deserialize(rdr)!;
        }


        /// <summary>
        /// Creates an XML serializer for the <see cref="SerializedLibrary"/> class,
        /// or reuses the previously created one.
        /// </summary>
        /// <returns>An XML serializer customized to handle Reko types.</returns>
        public static XmlSerializer CreateSerializer()
        {
            if (serializer is null)
            {
                serializer = CreateSerializer(typeof(SerializedLibrary), Namespace_v1);
            }
            return serializer;
        }

        /// <summary>
        /// Creates an XML serializer for the given root type and XML namespace.
        /// </summary>
        /// <param name="rootType">Root type of the library.</param>
        /// <param name="namespace">XML namespace to use.</param>
        /// <returns>An XML serializer.</returns>
        public static XmlSerializer CreateSerializer(Type rootType, string @namespace)
        {
            var attrOverrides = SerializedType.GetAttributeOverrides(TypesToDecorate, @namespace);
            return new XmlSerializer(rootType, attrOverrides);
        }

        /// <summary>
        /// Creates an XML serializer for the <see cref="SerializedLibrary"/> class using
        /// the v5 XML namespace.
        /// </summary>
        /// <param name="rootType">Root type.</param>
        /// <returns>An XML serializer.</returns>
        public static XmlSerializer CreateSerializer_v5(Type rootType)
        {
            return CreateSerializer(rootType, Namespace_v5);
        }

        /// <summary>
        /// Creates an XML serializer for the <see cref="SerializedLibrary"/> class using
        /// the v4 XML namespace.
        /// </summary>
        /// <param name="rootType">Root type.</param>
        /// <returns>An XML serializer.</returns>
        public static XmlSerializer CreateSerializer_v4(Type rootType)
        {
            return CreateSerializer(rootType, Namespace_v4);
        }

        /// <summary>
        /// Creates an XML serializer for the <see cref="SerializedLibrary"/> class using
        /// the v1 XML namespace.
        /// </summary>
        /// <param name="rootType">Root type.</param>
        /// <returns>An XML serializer.</returns>
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
        /// <summary>
        /// Special value for <see cref="Ordinal"/> that indicates that there is no ordinal.
        /// </summary>
        public const int NoOrdinal = -1;

        /// <summary>
        /// Name of the global variable.
        /// </summary>
        [XmlAttribute("name")]
        public string? Name;

        /// <summary>
        /// Optional ordinal of the global variable.
        /// </summary>
        [XmlAttribute("ordinal")]
        [DefaultValue(NoOrdinal)]
        public int Ordinal;

        /// <summary>
        /// Address of the global variable.
        /// </summary>
        [XmlAttribute("addr")]
        public string? Address;

        /// <summary>
        /// Data type of the global variable.
        /// </summary>

        public SerializedType? DataType;
    }
}
