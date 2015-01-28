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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;

namespace Decompiler.Core.Configuration
{
    public interface LoaderElement
    {
        string MagicNumber { get; }
        string TypeName { get; set; }
        string Offset { get; }
        string Extension { get; }

        string Label { get; set; }
        string Argument { get; set; }
    }

    public class LoaderElementImpl : ConfigurationElement, LoaderElement
    {
        /// <summary>
        /// The first few bytes of an image file expressed as a hexadecimal string. The presence of such a
        /// sequence of bytes selects this loader
        /// </summary>
        /// <remarks>
        /// For instance, the 'MZ' signature of MS-DOS executables is expressed as the hexadecimal string 4D5A.
        /// </remarks>
        [ConfigurationProperty("MagicNumber", IsRequired = false)]
        public string MagicNumber
        {
            get { return (string)this["MagicNumber"]; }
            set { this["MagicNumber"] = value; }
        }

        /// <summary>
        /// The offset at which to look for the magic number. By default, a missing value means
        /// offset 0.
        /// </summary>
        [ConfigurationProperty("Offset", IsRequired = false)]
        public string Offset
        {
            get { return (string) this["Offset"]; }
            set { this["Offset"] = value; }
        }

        /// <summary>
        /// The assembly-qualified name for the .NET type that is responsible for handling this
        /// format.
        /// </summary>
        [ConfigurationProperty("Type", IsRequired = false)]
        public string TypeName
        {
            get { return (string)this["Type"]; }
            set { this["Type"] = value; }
        }

        /// <summary>
        /// If the file being opened has this file extension, this loader will be used.
        /// </summary>
        [ConfigurationProperty("Extension", IsRequired = false)]
        public string Extension
        {
            get { return (string) this["Extension"]; }
            set { this["Extension"] = value; }
        }

        /// <summary>
        /// A string label used to refer to specific loaders.
        /// </summary>
        [ConfigurationProperty("Label", IsRequired = false)]
        public string Label
        {
            get { return (string) this["Label"]; }
            set { this["Label"] = value; }
        }

        /// <summary>
        /// A format string that can be used to pass parameters to a loader implemented as an executable.
        /// </summary>
        [ConfigurationProperty("Argument", IsRequired = false)]
        public string Argument
        {
            get { return (string) this["Argument"]; }
            set { this["Argument"] = value; }
        }
    }
}
