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
/// Computes a fingerprint feature for a procedure.
/// </summary>
public interface IFeatureComputer
{
    /// <summary>
    /// The type of feature that this class computes.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Computes a fingerprint feature for the given procedure.
    /// </summary>
    /// <param name="proc">Procedure whose feature is computed.</param>
    /// <param name="program">Program context.</param>
    /// <returns>An <see cref="IFeature"/> implementation.</returns>
    IFeature ComputeFeature(Procedure proc, Program program);
}
