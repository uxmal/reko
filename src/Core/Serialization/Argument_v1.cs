#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
        /// <summary>
        /// Constructs an empty instance of the <see cref="Argument_v1"/> class.
        /// </summary>
        public Argument_v1() { }

        /// <summary>
        /// Constructs an initialized instance of the <see cref="Argument_v1"/> class.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="type">The serialized data type of the argument.</param>
        /// <param name="kind">The storage type of the argument.</param>
        /// <param name="outParameter">True if this argument is an out parameter.</param>
        public Argument_v1(string name, SerializedType type, SerializedStorage kind, bool outParameter)
        {
            Name = name;
            Type = type;
            Kind = kind;
            OutParameter = outParameter;
        }

        /// <summary>
        /// The name of this parameter.
        /// </summary>
        [XmlAttribute("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The serialized data type of this parameter.
        /// </summary>
        public SerializedType? Type; // Reference to a type

        /// <summary>
        /// Storage type of this argument.
        /// </summary>
        [XmlElement("reg", typeof(Register_v1))]
        [XmlElement("stack", typeof(StackVariable_v1))]
        [XmlElement("fpustack", typeof(FpuStackVariable_v1))]
        [XmlElement("seq", typeof(SerializedSequence))]
        [XmlElement("flag", typeof(FlagGroup_v1))]
        [ReadOnly(true)]
        public SerializedStorage? Kind { get; set; }

        /// <summary>
        /// Whether this is an out parameter or not.
        /// </summary>
        [XmlAttribute("out")]
        [DefaultValue(false)]
        public bool OutParameter { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder("arg(");
            if (!string.IsNullOrEmpty(Name))
                sb.AppendFormat("{0},", Name);
            sb.Append(Type);
            sb.Append(')');
            return sb.ToString();
        }
	}
}
