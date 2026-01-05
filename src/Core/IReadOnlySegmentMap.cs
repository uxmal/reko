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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Reko.Core.Loading;

namespace Reko.Core
{
    /// <summary>
    /// A read-only view of a <see cref="SegmentMap"/>. Only
    /// actions with no side effects are allowed on this interface.
    /// </summary>
    public interface IReadOnlySegmentMap
    {
        /// <summary>
        /// Maps the selector values to their corresponding segments.
        /// </summary>
        IReadOnlyDictionary<ushort, ImageSegment> Selectors { get; }

        /// <summary>
        /// Returns true if <paramref name="addr"/> is a valid address in the
        /// segment map.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        bool IsValidAddress(Address addr);

        /// <summary>
        /// Given an address, locates the segment the address is in.
        /// </summary>
        /// <param name="addr">Address to use.</param>
        /// <param name="seg">The image segment containing that address, if any.
        /// </param>
        /// <returns>True if a segment was found; otherwise false.
        /// </returns>
        bool TryFindSegment(Address addr, [MaybeNullWhen(false)] out ImageSegment seg);
    }
}
