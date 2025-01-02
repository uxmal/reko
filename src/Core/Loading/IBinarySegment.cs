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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Loading;

/// <summary>
/// Represents a segment of the executable file.
/// </summary>
/// <remarks>
/// Some file formats distinguish between segments and sections
/// (e.g. ELF and MachO) while others conflate the two
/// concepts (e.g. PE). 
/// </remarks>
public interface IBinarySegment
{
    /// <summary>
    /// File format specific segment type.
    /// </summary>
    uint Type { get; }

    /// <summary>
    /// The file offset from which the contents of the segment are loaded.
    /// </summary>
    ulong FileOffset { get; }

    /// <summary>
    /// Virtual address at which this segment expects to be loaded.
    /// </summary>
    Address VirtualAddress { get; }

    /// <summary>
    ///  Physical address at which this segment expects to be loaded.
    /// </summary>
    Address PhysicalAddress { get; }

    /// <summary>
    /// The size of the segment in the file.
    /// </summary>
    ulong FileSize { get; }

    /// <summary>
    /// The size occupied by the segment in memory.
    /// </summary>
    ulong MemorySize { get; }

    /// <summary>
    /// File format specific flags.
    /// </summary>
    ulong Flags { get; }

    /// <summary>
    /// Alignment requirement for this segment.
    /// </summary>
    ulong Alignment { get; }

    /// <summary>
    /// Access mode for this segment.
    /// </summary>
    AccessMode AccessMode { get; }

    /// <summary>
    /// Sections contained by this segment.
    /// </summary>
    IReadOnlyList<IBinarySection> Sections { get; }
}

