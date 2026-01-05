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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;

namespace Reko.Arch.Motorola.M88k;

public class MemoryOperand : AbstractMachineOperand
{
    private MemoryOperand(PrimitiveType dt, RegisterStorage @base, RegisterStorage? index) :
        base(dt)
    {
        this.Base = @base;
        this.Index = index;
    }

    public RegisterStorage Base { get; }
    public int Offset { get; private set; }
    public RegisterStorage? Index { get; }
    public int Scale { get; private set; }

    public static MemoryOperand Indirect(
        PrimitiveType dt,
        RegisterStorage reg,
        uint offset)
    {
        return new MemoryOperand(dt, reg, null)
        {
            Offset = (int)offset,
        };
    }

    public static MemoryOperand Indexed(
        PrimitiveType dt,
        RegisterStorage reg,
        RegisterStorage index,
        int scale)
    {
        return new MemoryOperand(dt, reg, index)
        {
            Scale = scale
        };
    }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        renderer.WriteString(this.Base.Name);
        if (Index is null)
        {
            renderer.WriteString($",0x{Offset:X}");
        }
        else if (Scale == 0)
        {
            renderer.WriteString($",{Index.Name}");
        }
        else
        {
            renderer.WriteString($"[{Index.Name}]");
        }
    }
}