#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Represents a location  in an image whose name or size is 
    /// known. ImageSymbols are used to "seed" the Scanner
    /// phase of the decompiler.
    /// </summary>
    public class ImageSymbol
    {
        public ImageSymbol(Address address)
        {
            this.Address = address;
        }

        public Address Address { get; private set; }

        /// <summary>
        /// The name of the symbol if known, null or blank if unknown.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The size of the object referred to by the symbol, or 0 if 
        /// unknown.
        /// </summary>
        public uint Size { get; set; }

        public SymbolTypeq Type { get; set; }
    }

    public enum SymbolTypeq
    {
        Unknown,        // Unknown type
        Code,           // executable code
        Data,           // non-executable data.
    }
}
