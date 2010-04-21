/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
	public class SerializedStructType : SerializedType
	{
		private List<SerializedStructField> fields;

		[XmlElement("name")]
		public string Name;

		[XmlElement("size")]
		public int ByteSize;

		public SerializedStructType()
		{
            fields = new List<SerializedStructField>();
		}

		[XmlElement("field", typeof (SerializedStructField))]
		public List<SerializedStructField> Fields
		{
			get { return fields; }
		}

		public override Decompiler.Core.Types.DataType BuildDataType(Decompiler.Core.Types.TypeFactory factory)
		{
			StructureType str = factory.CreateStructureType(null, 0);
			foreach (SerializedStructField f in fields)
			{
				str.Fields.Add(new StructureField(f.Offset, f.Type.BuildDataType(factory), f.Name));
			}
			return str;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("struct({0}", ByteSize);
			foreach (SerializedStructField f in fields)
			{
				sb.AppendFormat(", ({0}, {1}, {2})", f.Offset, f.Name != null?f.Name: "?", f.Type);
			}
			sb.Append(")");
			return sb.ToString();
		}

	}

	public class SerializedStructField
	{
		[XmlAttribute("offset")]
		public int Offset;

		[XmlAttribute("name")]
		public string Name;

		[XmlElement("prim", typeof (SerializedPrimitiveType))]
		[XmlElement("ptr", typeof (SerializedPointerType))]
		public SerializedType Type;

		public SerializedStructField()
		{
		}

		public SerializedStructField(int offset, string name, SerializedType type)
		{
			this.Offset = offset;
			this.Name = name;
			this.Type = type;
		}
	}
}
