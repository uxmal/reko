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

using Reko.Core.Types;
using System;

namespace Reko.Core.Machine;

/// <summary>
/// Represents a bit operand number in a machine instruction.
/// </summary>
public class BitOperand : AbstractMachineOperand
{
    /// <summary>
    /// Constructs a bit operand.
    /// </summary>
    /// <param name="op">Operand of which a bit is to be extracted.</param>
    /// <param name="bitPos">Bit position.</param>
    public BitOperand(MachineOperand op, int bitPos) : base(PrimitiveType.Bool)
    {
        this.Operand = op;
        this.BitPosition = bitPos;
    }

    /// <summary>
    /// The operand from which the bit is extracted.
    /// </summary>
    public MachineOperand Operand { get; }

    /// <summary>
    /// The bit position of the operand.
    /// </summary>
    public int BitPosition { get; }

    /// <summary>
    /// Not implemented.
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="options"></param>
    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        throw new NotImplementedException();
    }
}
