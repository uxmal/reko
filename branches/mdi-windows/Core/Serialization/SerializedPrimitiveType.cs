/* 
 * Copyright (C) 1999-2009 John Källén.
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
using System.ComponentModel;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
	/// <summary>
	/// Serialized representation of serialized primitive types.
	/// </summary>
	public class SerializedPrimitiveType : SerializedType
	{
		[XmlAttribute("domain")]
		public Domain Domain;

		[XmlAttribute("size")]
		public int ByteSize;

		public SerializedPrimitiveType()
		{
		}

		public SerializedPrimitiveType(Domain domain, int byteSize)
		{
			this.Domain = domain;	
			this.ByteSize = byteSize;
		}

		public override DataType BuildDataType(TypeFactory factory)
		{
			return factory.CreatePrimitiveType(Domain, ByteSize);
		}

		public override string ToString()
		{
			return string.Format("prim({0},{1})", Domain, ByteSize);
		}
	}
}
