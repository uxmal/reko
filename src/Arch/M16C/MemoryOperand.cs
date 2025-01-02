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

namespace Reko.Arch.M16C;

public class MemoryOperand : AbstractMachineOperand
{
    private MemoryOperand(DataType width, Storage? baseReg, int offset) 
        : base(width)
    {
        this.Base = baseReg;
        this.Offset = offset;
    }

    public static MachineOperand Direct(PrimitiveType dt, int uAddr)
    {
        return new MemoryOperand(dt, null, uAddr);
    }

    public static MemoryOperand Indirect(PrimitiveType dt, Storage baseReg)
    {
        return new MemoryOperand(dt, baseReg, 0);
    }

    public static MemoryOperand Indirect(PrimitiveType dt, RegisterStorage baseReg, int displacement)
    {
        return new MemoryOperand(dt, baseReg, displacement);
    }

    public Storage? Base { get; }

    public int Offset { get; }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        if (Base is not null)
        {
            if (Offset != 0)
            {
                renderer.WriteString(M16CInstruction.FormatSignedValue(Offset, null));
            }
            renderer.WriteChar('[');
            renderer.WriteString(Base.Name);
            renderer.WriteChar(']');
        }
        else
        {
            renderer.WriteChar('[');
            renderer.WriteString(M16CInstruction.FormatUnsignedValue((uint)Offset, null));
            renderer.WriteChar(']');
        }
    }
}
