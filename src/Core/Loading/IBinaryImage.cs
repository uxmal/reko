#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
    IBinaryHeader Header { get; }
    EndianServices Endianness { get; }

    IReadOnlyList<IBinarySection> Sections {get;}
    IReadOnlyList<IBinarySegment> Segments { get; }
    IBinaryDebugInfo? DebugInfo { get; }

    IReadOnlyList<ImageSymbol> Symbols { get; }
    IReadOnlyList<ImageSymbol> DynamicSymbols { get; }

    IReadOnlyList<IRelocation> Relocations { get; }
    IReadOnlyList<IRelocation> DynamicRelocations { get; }

    IBinaryFormatter CreateFormatter(string? outputFormat = null);

    Program Load();
}
