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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    public class EnumType_v1 : SerializedTaggedType
    {
        [XmlAttribute("size")]
        [DefaultValue(0)]
        public int Size;

        [XmlAttribute("Domain")]
        [DefaultValue(Domain.None)]
        public Domain Domain;

        [XmlElement("member")]
        public SerializedEnumValue[]  Values;

        public EnumType_v1()
        {
        }

        public EnumType_v1(int size, Types.Domain domain, string p)
        {
            this.Size = size;
            this.Domain = domain;
            this.Name = p;
        }

        public override DataType BuildDataType(TypeFactory factory)
        {
            return factory.CreateEnum(Size, Domain, Name, Values);
        }

        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitEnum(this);
        }
    }

    public class SerializedEnumValue
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("value")]
        public int Value;
    }
}
