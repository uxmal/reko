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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.Arch.Sparc;

/// <summary>
/// Used to represent sliced immediate values in 
/// sethi instructions.
/// </summary>
public class SliceOperand : AbstractMachineOperand
{
    public SliceOperand(
        SliceType slice,
        Constant value,
        MachineOperand inferredValue)
        : base(value.DataType)
    {
        this.Slice = slice;
        this.Value = value;
        this.InferredValue = inferredValue;
    }

    public SliceType Slice { get; }
    public Constant Value { get; }
    public MachineOperand InferredValue { get; }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        renderer.WriteString(Slice.Format());
        renderer.WriteChar('(');
        InferredValue.Render(renderer, options);
        renderer.WriteChar(')');
    }
}

public enum SliceType
{
    None,

    Hi,
    Lo,
}

public static class SliceTypeExtensions
{
    private static readonly Dictionary<SliceType, string> strFormats = new()
    {
        { SliceType.Hi, "%hi" },
        { SliceType.Lo, "%lo" },
    };

    public static string Format(this SliceType type)
    {
        return strFormats[type];
    }
}
