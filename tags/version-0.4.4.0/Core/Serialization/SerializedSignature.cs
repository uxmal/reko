#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core.Lib;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
	/// <summary>
	/// Serialized representation of a procedure signature.
	/// </summary>
	public class SerializedSignature : SerializedType
	{
        public SerializedSignature()
        {
        }

		[XmlElement("return")]
		public Argument_v1 ReturnValue;

		[XmlAttribute("convention")]
		public string Convention;		

		[XmlAttribute("stackDelta")]
		[DefaultValue(0)]
		public int StackDelta;

		[XmlElement("arg")]
		public Argument_v1 [] Arguments;

        [XmlAttribute("fpuStackDelta")]
        [DefaultValue(0)]
        public int FpuStackDelta;

        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitSignature(this);
        }

        public override Types.DataType BuildDataType(Types.TypeFactory factory)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("fn(");
            if (!string.IsNullOrEmpty(Convention))
                sb.AppendFormat("{0},", Convention);
            if (ReturnValue != null)
            {
                sb.Append(ReturnValue.ToString());
            }
            else 
                sb.Append("void");
            sb.Append(",(");
            if (Arguments != null)
            {
                foreach (var arg in Arguments)
                {
                    sb.Append(arg.ToString());
                }
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
