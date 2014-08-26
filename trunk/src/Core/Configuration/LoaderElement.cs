#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
        string TypeName { get; }
        string Offset { get; }
        string Extension { get; }
    }

    public class LoaderElementImpl : ConfigurationElement, LoaderElement
    {
        /// <summary>
        /// The first few bytes of an image file expressed as a hexadecimal string. The presence of such a
        /// sequence of bytes selects this loader
        /// </summary>
        /// <remarks>
        /// For instance, the 'MZ' signature of MS-DOS executables is expressed as the hexadecimal string 4D5A.</remarks>
        [ConfigurationProperty("MagicNumber", IsRequired = true)]
        public string MagicNumber
        {
            get { return (string)this["MagicNumber"]; }
            set { this["MagicNumber"] = value; }
        }

        [ConfigurationProperty("Offset", IsRequired = false)]
        public string Offset
        {
            get { return (string) this["Offset"]; }
            set { this["Offset"] = value; }
        }

        [ConfigurationProperty("Type", IsRequired = true)]
        public string TypeName
        {
            get { return (string)this["Type"]; }
            set { this["Type"] = value; }
        }

        [ConfigurationProperty("Extension", IsRequired = false)]
        public string Extension
        {
            get { return (string) this["Extension"]; }
            set { this["Extension"] = value; }
        }
    }
}
