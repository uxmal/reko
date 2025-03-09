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
/// Represents an address relocation.
/// </summary>
public interface IRelocation
{
    /// <summary>
    /// Offset at which the relocation is to take place.
    /// </summary>
    /// <remarks>
    /// In relocatable files, this offset may be relative
    /// to a section start. Otherwise, the offset is
    /// relative to the base of the executable.
    /// </remarks>
    ulong Offset { get; }

    /// <summary>
    /// Optional added to apply when relocating.
    /// </summary>
    long? Addend { get; }

    /// <summary>
    /// Optional symbol whose value may or may not
    /// be used when computing the relocation.
    /// </summary>
    IBinarySymbol? Symbol { get; }

    /// <summary>
    /// File format-specific information about this
    /// relocation.
    /// </summary>
    ulong Info { get; }
}

