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
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
	public class PointerType_v1 : SerializedType
	{
		public SerializedType DataType;
        [XmlAttribute("size")]
        [DefaultValue(0)]
        public int PointerSize;

		public PointerType_v1()
		{
		}

		public PointerType_v1(SerializedType pointee)
		{
			DataType = pointee;
		}

        public static SerializedType Create(SerializedType dt, int n)
        {
            return new PointerType_v1 { DataType = dt, PointerSize = n };
        }

        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitPointer(this);
        }

		public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("ptr({0}", DataType);
            WriteQualifier(Qualifier, sb);
            sb.Append(")");
            return sb.ToString();
        }
    }
}
