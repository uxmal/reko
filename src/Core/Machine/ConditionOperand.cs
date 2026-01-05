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
/// Utility class for creating condition code operands.
/// </summary>
public static class ConditionOperand
{
    /// <summary>
    /// Constructs a condition code operand.
    /// </summary>
    /// <typeparam name="TCondCode">Architecture-specific condition code type.</typeparam>
    /// <param name="code">Condition code to wrap.</param>
    /// <returns>A <see cref="ConditionOperand{TCondCode}"/> instance.</returns>
    public static ConditionOperand<TCondCode> Create<TCondCode>(TCondCode code)
        where TCondCode : System.Enum
    {
        return new ConditionOperand<TCondCode>(code);
    }
}

/// <summary>
/// Represents a condition code operand in a machine instruction.
/// </summary>
/// <typeparam name="TCondCode">Type of condition code.</typeparam>
public class ConditionOperand<TCondCode> : AbstractMachineOperand
    where TCondCode : System.Enum
{
    /// <summary>
    /// Constructs a condition code operand.
    /// </summary>
    /// <param name="code">Condition code.</param>
    public ConditionOperand(TCondCode code)
        : base(PrimitiveType.Byte)
    {
        this.Condition = code;
    }

    /// <summary>
    /// The condition code of this operand.
    /// </summary>
    public TCondCode Condition { get; }

    /// <inheritdoc/>
    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        renderer.WriteString(Condition.ToString());
    }
}
