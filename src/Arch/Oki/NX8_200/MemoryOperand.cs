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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Oki.NX8_200;

public class MemoryOperand : AbstractMachineOperand
{
    public RegisterStorage? Segment { get; }
    public RegisterStorage? Base { get; }

    public RegisterStorage? Index { get; private set; }
    public int Offset { get; }
    public bool PostDecrement { get; private set; }
    public bool PostIncrement { get; private set; }

    private MemoryOperand(RegisterStorage? segment, RegisterStorage? @base, int offset)
        : base(PrimitiveType.Byte)
    {
        Segment = segment;
        Base = @base;
        Offset = offset;
    }

    public static MachineOperand Direct(RegisterStorage? segment, int offset)
    {
        return new MemoryOperand(segment, null, offset);
    }

    public static MachineOperand Indirect(RegisterStorage? segment, RegisterStorage @base, int offset)
    {
        return new MemoryOperand(segment, @base, offset);
    }


    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        var b = Base;
        if (b is not null)
        {
            if (b == Registers.Lrb)
            {
                renderer.WriteString("off ");
                renderer.WriteString($"0{Offset:X}h");
                return;
            }
            if (Offset != 0)
            {
                renderer.WriteFormat("{0}", Offset);
            }
            renderer.WriteChar('[');
            renderer.WriteString(Segment?.Name ?? "");
            renderer.WriteChar(':');
            renderer.WriteString(b.Name);
            if (PostIncrement)
                renderer.WriteChar('+');
            if (PostDecrement)
                renderer.WriteChar('-');
            renderer.WriteChar(']');
        }
        else
        {
            if (Segment is null)
            {
                renderer.WriteString($"0{Offset:X}h");
            }
            else
            {
                renderer.WriteString($"[{Segment}:{Offset:X4}]");
            }
        }
    }

    public static MemoryOperand PostIncremented(RegisterStorage seg, RegisterStorage @base)
    {
        var mem = new MemoryOperand(seg, @base, 0);
        mem.PostIncrement = true;
        return mem;
    }

    public static MemoryOperand PostDecremented(RegisterStorage seg, RegisterStorage @base)
    {
        var mem = new MemoryOperand(seg, @base, 0);
        mem.PostDecrement = true;
        return mem;
    }

    public static MemoryOperand Indexed(RegisterStorage dsr, RegisterStorage @base, RegisterStorage index)
    {
        var mem = new MemoryOperand(dsr, @base, 0);
        mem.Index = index;
        return mem;
    }
}