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

using Reko.Core.Serialization;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Represents a named data type.
    /// </summary>
    public class NamedDataType
    {
        /// <summary>
        /// The name of the data type.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The size of the data type in storage units.
        /// </summary>
        public int Size { get; set; } // in bytes.

        /// <summary>
        /// Its corresponding Reko data type.
        /// </summary>
        public SerializedType? DataType { get; set; }
    }
}
