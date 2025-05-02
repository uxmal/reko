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

namespace Reko.Core.Loading;

/// <summary>
/// Represents a segment of the executable file.
/// </summary>
/// <remarks>
/// Some file formats distinguish between segments and sections
/// (e.g. ELF and MachO) while others conflate the two
/// concepts (e.g. PE). In general, sections are smaller, or
/// contained within segments.
/// </remarks>
public interface IBinarySection
{
    /// <summary>
    /// The index of this section.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// The name of this section.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The size (in memory) of this item.
    /// </summary>
    ulong Size { get; }

    /// <summary>
    /// The address at which the section is to be loaded.
    /// </summary>
    Address VirtualAddress { get; }

    /// <summary>
    /// Offset in the file from which the section is loaded.
    /// </summary>
    ulong FileOffset { get; }

    /// <summary>
    /// Size in the file from which the section is loaded.
    /// </summary>
    ulong FileSize { get; }

    /// <summary>
    /// The alignment requirement for this section.
    /// </summary>
    ulong Alignment { get; }

    /// <summary>
    /// Image format specific flags.
    /// </summary>
    ulong Flags { get; }

    /// <summary>
    /// Access mode for this section.
    /// </summary>
    AccessMode AccessMode { get; }
}