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
/// Represents the size of a <see cref="Procedure"/> measured in the number of 
/// machine instructions.
/// </summary>
public class ProcedureSizeFeature : IFeature
{
    private int size;

    /// <summary>
    /// Constructs an instance of <see cref="ProcedureSizeFeature"/>.
    /// </summary>
    /// <param name="size">Number of machine instructions.</param>
    public ProcedureSizeFeature(int size)
    {
        this.size = size;
    }

    /// <inheritdoc/>
    public string Type => "procsize";

    /// <summary>
    /// Number of machine instructions in the procedure.
    /// </summary>
    public object Value => this.size;

    /// <inheritdoc/>
    public bool IsMatch(IFeature other)
    {
        if (other is not ProcedureSizeFeature that)
            return false;
        return this.size == that.size;
    }
}