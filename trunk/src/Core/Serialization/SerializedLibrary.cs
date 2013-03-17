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

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
    [XmlRoot(ElementName = "library", Namespace = SerializedLibrary.Namespace)]
    public class SerializedLibrary
    {
        public const string Namespace = "http://schemata.jklnet.org/Decompiler";

        [XmlAttribute("case")]
        public string Case;

        [XmlElement("default")]
        public SerializedLibraryDefaults Defaults;

        [XmlElement("types")]
        public SerializedType[] Types;

        [XmlElement("procedure", typeof(SerializedProcedure))]
        [XmlElement("service", typeof(SerializedService))]
        public List<SerializedProcedureBase> Procedures;

        private static XmlSerializer serializer;

        public SerializedLibrary()
        {
            this.Procedures = new List<SerializedProcedureBase>();
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
                var attrOverrides = SerializedType.GetAttributeOverrides(TypesToDecorate);
                serializer = new XmlSerializer(typeof(SerializedLibrary), attrOverrides);
            }
            return serializer;
        }

        private static Type[] TypesToDecorate = new Type[] 
        {
            typeof(SerializedPrimitiveType),
            typeof(SerializedPointerType),
            //typeof(SerializedArray),
            typeof(SerializedStructType),
            //typeof(serializedUnionType),
            typeof(SerializedSignature),
            typeof(SerializedTypedef),
            typeof(SerializedLibrary),
        };
    }
}
