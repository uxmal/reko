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
/// Represents a fingerprint feature that is the hash of the machine
/// instructions of a procedure.
/// </summary>
public class MachineInstructionHashFeature : IFeature
{
    private readonly int hash;

    /// <summary>
    /// Constructs an instance of <see cref="MachineInstructionHashFeature"/>.
    /// </summary>
    /// <param name="hash">The machine instruction hash.</param>
    public MachineInstructionHashFeature(int hash)
    {
        this.hash = hash;
    }

    /// <inheritdoc/>
    public string Type => FeatureNames.MachineInstructionHash;

    /// <inheritdoc/>
    public object Value => hash;

    /// <inheritdoc/>
    public bool IsMatch(IFeature other)
    {
        if (other is not MachineInstructionHashFeature that)
            return false;
        return this.hash == that.hash;
    }
}
