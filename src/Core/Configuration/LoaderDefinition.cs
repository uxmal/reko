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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Reko.Core.Configuration
{
    public class LoaderDefinition
    {
        /// <summary>
        /// The first few bytes of an image file expressed as a hexadecimal string. The presence of such a
        /// sequence of bytes selects this loader
        /// </summary>
        /// <remarks>
        /// For instance, the 'MZ' signature of MS-DOS executables is expressed as the hexadecimal string 4D5A.
        /// </remarks>
        public string MagicNumber { get; set; } 

        /// <summary>
        /// The offset at which to look for the magic number. By default, a missing value means
        /// offset 0.
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// The assembly-qualified name for the .NET type that is responsible for handling this
        /// format.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// If the file being opened has this file extension, this loader will be used.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// A string label used to refer to specific loaders.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// A format string that can be used to pass parameters to a loader implemented as an executable.
        /// </summary>
        public string Argument { get; set; }

        /// <summary>
        /// Text to display in user interfaces when browsing loaders.
        /// </summary>
        public string Description { get; set; }
    }
}
