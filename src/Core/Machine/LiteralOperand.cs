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

using Reko.Core.Types;

namespace Reko.Core.Machine;

/// <summary>
/// Represents a literal string-valued operand.
/// </summary>
public class LiteralOperand : AbstractMachineOperand
{
    private readonly string literal;

    /// <summary>
    /// Constructs a literal operand.
    /// </summary>
    /// <param name="value">Literal operand.</param>
    public LiteralOperand(string value) : base(PrimitiveType.Byte)
    {
        this.literal = value;
    }

    /// <summary>
    /// Returns the string value of the literal operand.
    /// </summary>
    /// <param name="renderer">Output sink.</param>
    /// <param name="options">Options controlling the output</param>
    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        renderer.WriteString(literal);
    }
}
