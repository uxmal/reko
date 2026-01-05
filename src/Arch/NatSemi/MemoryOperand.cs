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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.NatSemi;

public class MemoryOperand : AbstractMachineOperand
{
    public MachineOperand? Base { get; private set; }
    public MachineOperand? Displacement { get; private set; }
    public int Scale { get; private set; }

    public static MemoryOperand RegisterRelative(PrimitiveType dt, RegisterStorage reg, int displacement)
    {
        return new MemoryOperand(dt)
        {
            Base = reg,
            Displacement = Constant.Int32(displacement)
        };
    }

    public static MemoryOperand MemoryRelative(PrimitiveType dataSize, RegisterStorage reg, int value1, int value2)
    {
        var mem = new MemoryOperand(PrimitiveType.Ptr32)
        {
            Base = reg,
            Displacement = Constant.Int32(value1)
        };
        mem = new MemoryOperand(dataSize)
        {
            Base = mem,
            Displacement = Constant.Int32(value2)
        };
        return mem;
    }

    public static MachineOperand? Indexed(
        PrimitiveType dataSize,
        MachineOperand baseOperand,
        RegisterStorage indexReg,
        int scale)
    {
        var mem = new MemoryOperand(dataSize)
        {
            Base = baseOperand,
            Displacement = indexReg,
            Scale = scale,
        };
        return mem;
    }


    internal static MachineOperand? Absolute(PrimitiveType dataSize, Address address)
    {
        var mem = new MemoryOperand(dataSize)
        {
            Base = address,
            Displacement = null,
            Scale = 0
        };
        return mem;
    }

    private MemoryOperand(PrimitiveType dt)
        : base(dt)
    {
    }


    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        if (Base is null)
        {
            throw new NotImplementedException();
        }
        if (Scale == 0)
        {
            if (Base is Address addr)
            {
                renderer.WriteAddress($"@{addr}", addr);
                return;
            }
            if (Displacement is not null)
            {
                if (Displacement is Constant c)
                {
                    int i = c.ToInt32();
                    renderer.WriteFormat("{0}", i);
                }
                else
                {
                    Displacement.Render(renderer, options);
                }
            }
            renderer.WriteChar('(');
            Base.Render(renderer, options);
            renderer.WriteChar(')');
        }
        else 
        {
            Base.Render(renderer, options);
            renderer.WriteChar('[');
            Displacement!.Render(renderer, options);
            renderer.WriteString(Scale switch
            {
                1 => ":b",
                2 => ":w",
                4 => ":d",
                8 => ":q",
                _ => throw new NotSupportedException($"Scale {Scale} is not supported.")
            });
            renderer.WriteChar(']');
        }
    }

}