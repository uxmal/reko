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
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
	/// <summary>
	/// Serialization format for procedure arguments.
	/// </summary>
	public class Argument_v1
	{
        public Argument_v1() { }

        public Argument_v1(string name, SerializedType type, SerializedKind kind, bool outParameter)
        {
            Name = name;
            Type = type;
            Kind = kind;
            OutParameter = outParameter;
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        public SerializedType Type; // Reference to a type

        [XmlElement("reg", typeof(Register_v1))]
        [XmlElement("stack", typeof(StackVariable_v1))]
        [XmlElement("fpustack", typeof(FpuStackVariable_v1))]
        [XmlElement("seq", typeof(SerializedSequence))]
        [XmlElement("flag", typeof(FlagGroup_v1))]
        [ReadOnly(true)]
        public SerializedKind Kind { get; set; }

        [XmlAttribute("out")]
        [DefaultValue(false)]
        public bool OutParameter { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder("arg(");
            if (!string.IsNullOrEmpty(Name))
                sb.AppendFormat("{0},", Name);
            sb.Append(Type);
            sb.Append(")");
            return sb.ToString();
        }
	}
}
