#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Array size specifying an array size.
    /// </summary>
    public class ArraySizeCharacteristic
    {
        /// <summary>
        /// Name of the argument.
        /// </summary>
        [XmlAttribute("argument")]
        public string? Argument { get; set; }

        /// <summary>
        /// Sizes of the array definitions.
        /// </summary>
        [XmlElement("factor")]
        public ArraySizeFactor[]? Factors { get; set; }
    }

    /// <summary>
    /// Array dimension size.
    /// </summary>
    public class ArraySizeFactor
    {
        /// <summary>
        /// Argumment name.
        /// </summary>
        [XmlAttribute("argument")]
        public string? Argument { get; set; }

        /// <summary>
        /// Constant value.
        /// </summary>
        [XmlAttribute("constant")]
        public string? Constant { get; set; }
    }
}
