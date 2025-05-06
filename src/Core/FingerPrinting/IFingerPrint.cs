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

namespace Reko.Core.FingerPrinting;

/// <summary>
/// A finger print is a collection of features that can be used to
/// identify a procedure.
/// </summary>
public interface IFingerPrint
{
    /// <summary>
    /// Returns true if this fingerprint matches the other fingerprint.
    /// </summary>
    /// <param name="other">Other fingerprint.</param>
    /// <returns></returns>
    bool IsMatch(IFingerPrint other);

    /// <summary>
    /// A collection of <see cref="IFeature"/>s that make up this 
    /// fingerprint.
    /// </summary>
    IEnumerable<IFeature> Features { get; }

    /// <summary>
    /// Facts discovered about this fingerprint.
    /// </summary>
    IDictionary<string, object> Facts { get; }
}
