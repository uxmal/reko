/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using System;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
	/// <summary>
	/// In-memory description of XML format for serializing Variables.
	/// </summary>
	public class SerializedArgument
	{
		[XmlAttribute("name")]
		public string Name;

		[XmlElement("type")]
		public string Type;				//$REVIEW: this needs to be SerializedType.

		[XmlElement("reg", typeof (SerializedRegister))]
		[XmlElement("stack", typeof (SerializedStackVariable))]
		[XmlElement("fpustack", typeof (SerializedFpuStackVariable))]
		[XmlElement("seq", typeof (SerializedSequence))]
		[XmlElement("flag", typeof (SerializedFlag))]
		public SerializedKind  Kind;

		[XmlAttribute("out")]
		[System.ComponentModel.DefaultValue(false)]
		public bool OutParameter;

		public SerializedArgument()
		{
		}

		public SerializedArgument(Identifier arg)
		{
			Name = arg.Name;
			Kind = arg.Storage.Serialize();
		}
	}
}
