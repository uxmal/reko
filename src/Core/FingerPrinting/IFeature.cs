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

namespace Reko.Core.FingerPrinting;

/// <summary>
/// Represents a fingerprint feature: a recognizable property of a procedure
/// or data item.
/// </summary>
public interface IFeature
{
    /// <summary>
    /// The type of this fingerprint feature.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// The value of this fingerprint feature.
    /// </summary>
    object Value { get; }

    /// <summary>
    /// Returns true if this feature matches the other feature.
    /// </summary>
    /// <param name="other">Other feature to match.</param>
    bool IsMatch(IFeature other);
}
