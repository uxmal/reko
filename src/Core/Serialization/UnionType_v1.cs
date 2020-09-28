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
    public class UnionType_v1 : SerializedTaggedType
    {
        [XmlAttribute("size")]
        [DefaultValue(0)]
        public int ByteSize;

        public UnionType_v1()
        {
        }

        [XmlElement("alt", typeof(UnionAlternative_v1))]
        public UnionAlternative_v1[] Alternatives;

        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitUnion(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("union({0}", ByteSize);
            if (Alternatives != null)
            {
                foreach (UnionAlternative_v1 alt in Alternatives)
                {
                    sb.AppendFormat(", ({0}, {1})", alt.Name != null ? alt.Name : "?", alt.Type);
                }
                sb.Append(")");
            }
            return sb.ToString();
        }
    }

    public class UnionAlternative_v1
    {
        [XmlAttribute("name")]
        public string Name;

        public SerializedType Type;

        public UnionAlternative_v1()
        {
        }

        public UnionAlternative_v1(string name, SerializedType type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
