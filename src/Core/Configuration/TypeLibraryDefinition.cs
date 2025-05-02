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
using System.Linq;

namespace Reko.Core.Configuration
{
    /// <summary>
    /// Type library definition.
    /// </summary>
    public class TypeLibraryDefinition
    {
        /// <summary>
        /// Unique identifier for referring to the type library programmatically.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Optinal comma-separated list of CPU architectures for which this type library is applicable.
        /// </summary>
        public string? Architecture { get; set; }

        /// <summary>
        /// Optional name of the module that contains the objects described by the type library.
        /// </summary>
        public string? Module { get; set; }

        /// <summary>
        /// CLR type name of the loader that can interpret the contents of the serialized type library.
        /// </summary>
        public string? Loader { get; set; }

        /// <summary>
        /// Checks if a given architecture matches any architecture of this type library.
        /// </summary>
        /// <param name="archName">Identifier of the architecture to check.</param>
        /// <returns>True if the architecture is a match; otherwise false.</returns>
        public bool MatchArchitecture(string archName)
        {
            if (Architecture is null)
                return true;
            var archs = Architecture.Split(',');
            return archs.Contains(archName);
        }
    }
}
