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

using Decompiler.Core;
using Decompiler.Core.Code;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
	/// <summary>
	/// In-memory description of XML format for serializing Variables.
	/// </summary>
	public class SerializedArgument
	{
        public SerializedArgument() { }

        public SerializedArgument(string name, string type, SerializedKind kind, bool outParameter)
        {
            Name = name;
            Type = type;
            Kind = kind;
            OutParameter = outParameter;
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("type")]
        public string Type { get;set; }		//$REVIEW: this needs to be SerializedType.
        [XmlElement("taip")]
        public SerializedType Taip { get; set; }

        [XmlElement("reg", typeof(SerializedRegister))]
        [XmlElement("stack", typeof(SerializedStackVariable))]
        [XmlElement("fpustack", typeof(SerializedFpuStackVariable))]
        [XmlElement("seq", typeof(SerializedSequence))]
        [XmlElement("flag", typeof(SerializedFlag))]
        [ReadOnly(true)]
        public SerializedKind Kind { get; set; }

        [XmlAttribute("out")]
        [DefaultValue(false)]
        public bool OutParameter { get; set; }

	}
}
