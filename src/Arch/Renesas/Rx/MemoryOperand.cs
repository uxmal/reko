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
using System.Diagnostics;

namespace Reko.Arch.Renesas.Rx;

public class MemoryOperand : AbstractMachineOperand
{
    public MemoryOperand(PrimitiveType dt, RegisterStorage reg, int offset) : base(dt)
    {
        this.Base = reg;
        this.Offset = offset;
    }

    public MemoryOperand(PrimitiveType dt, RegisterStorage @base, RegisterStorage index) : base(dt)
    {
        this.Base = @base;
        this.Index = index;
        this.AutoIncrement = AutoIncrement.None;
    }

    private MemoryOperand(PrimitiveType dt, RegisterStorage reg, AutoIncrement inc) : base(dt)
    {
        this.Base = reg;
        this.AutoIncrement = inc;
    }

    public RegisterStorage? Base { get; set; }
    public int Offset { get; set; }
    public RegisterStorage? Index { get; set; }
    public AutoIncrement AutoIncrement { get; set; }

    public static MachineOperand PostIncrement(PrimitiveType dataWidth, RegisterStorage reg)
    {
        return new MemoryOperand(dataWidth, reg, AutoIncrement.PostIncrement);
    }

    public static MachineOperand PreDecrement(PrimitiveType dataWidth, RegisterStorage reg)
    {
        return new MemoryOperand(dataWidth, reg, AutoIncrement.PreDecrement);
    }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        Debug.Assert(Base is not null);
        switch (this.AutoIncrement)
        {
        case AutoIncrement.None:
            if (Index is not null)
            {
                renderer.WriteString($"[{Index.Name},{Base.Name}]");
            }
            else if (Offset != 0)
            {
                renderer.WriteString($"{Offset}[{Base.Name}]");
            }
            else
            {
                renderer.WriteString($"[{Base.Name}]");
            }
            break;
        case AutoIncrement.PreDecrement:
            renderer.WriteString($"[-{Base.Name}]");
            break;
        case AutoIncrement.PostIncrement:
            renderer.WriteString($"[{Base.Name}+]");
            break;
        default:
            throw new InvalidOperationException("Impossible auto increment mode");
        }
    }
}
