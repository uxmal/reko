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

using Reko.Core;
using Reko.Core.Lib;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
	/// <summary>
	/// Serialized representation of a procedure signature.
	/// </summary>
	public class SerializedSignature : SerializedType
	{
        public SerializedSignature()
        {
        }

        [XmlElement("type")]
        public SerializedType EnclosingType;

		[XmlElement("return")]
		public Argument_v1 ReturnValue;

		[XmlAttribute("convention")]
		public string Convention;

        [XmlAttribute("isInstance")]
        [DefaultValue(false)]
        public bool IsInstanceMethod;

        /// <summary>
        /// Number of bytes occupied by the return address
        /// on the stack. If 0, use the default value
        /// provided by the Architecture.
        /// </summary>
        [XmlAttribute("retOnStack")]
		[DefaultValue(0)]
        public int ReturnAddressOnStack;

        [XmlAttribute("stackDelta")]
		[DefaultValue(0)]
		public int StackDelta;

		[XmlElement("arg")]
		public Argument_v1 [] Arguments;

        [XmlAttribute("fpuStackDelta")]
        [DefaultValue(0)]
        public int FpuStackDelta;

        [XmlAttribute("parametersValid")]
        [DefaultValue(true)]
        public bool ParametersValid = true;

        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitSignature(this);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("fn(");
            if (this.EnclosingType != null)
                sb.AppendFormat("{0},", EnclosingType);
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
                sb.Append(string.Join(",", Arguments.Select(a => a.ToString())));
            }
            sb.Append(")");
            sb.Append(")");
            return sb.ToString();
        }
    }
}
