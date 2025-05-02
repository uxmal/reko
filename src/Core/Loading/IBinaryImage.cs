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

using System.Collections.Generic;

namespace Reko.Core.Loading;

/// <summary>
/// Represents the content of a binary object file.
/// </summary>
public interface IBinaryImage
{
    /// <summary>
    /// Represents the header of a binary object file.
    /// </summary>
    IBinaryHeader Header { get; }

    /// <summary>
    /// The endianness of the binary object.
    /// </summary>
    EndianServices Endianness { get; }

    /// <summary>
    /// The <see cref="IBinarySection"/>s contained in the binary object.
    /// </summary>
    /// <remarks>
    /// Not all binary files will contain sections.
    /// </remarks>
    IReadOnlyList<IBinarySection> Sections { get; }

    /// <summary>
    /// The <see cref="IBinarySegment"/>s contained in the binary object.
    /// </summary>
    IReadOnlyList<IBinarySegment> Segments { get; }

    /// <summary>
    /// Optional debugging information contained in the binary object.
    /// </summary>
    IBinaryDebugInfo? DebugInfo { get; }

    /// <summary>
    /// Symbols contained in the binary object.
    /// </summary>
    IReadOnlyList<IBinarySymbol> Symbols { get; }

    /// <summary>
    /// Special symbols that are used to resolve dynamic references.
    /// </summary>
    IReadOnlyDictionary<int, IBinarySymbol> DynamicSymbols { get; }

    /// <summary>
    /// The static relocations in this binary image.
    /// </summary>
    IReadOnlyDictionary<int, IReadOnlyList<IRelocation>> Relocations { get; }

    /// <summary>
    /// Relocations specific to dynamic linking.
    /// </summary>
    IReadOnlyList<IRelocation> DynamicRelocations { get; }

    /// <summary>
    /// Loads the binary image from the specified file.
    /// </summary>
    /// <param name="addrLoad">An optional base address. If none is provided,
    /// the loader will loader the image at its preferred address.</param>
    /// <returns>A <see cref="Program"/> instance.
    /// </returns>
    Program Load(Address? addrLoad = null);
}
