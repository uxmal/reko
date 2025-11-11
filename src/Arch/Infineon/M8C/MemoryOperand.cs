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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Infineon.M8C;

public class MemoryOperand : AbstractMachineOperand
{
    private Type addressingMode;

    private enum Type
    {
        Direct,
        DirectIndexed,
        RegDirect,
        RegDirectIndexed,
        IndirectPostinc,
    }

    private MemoryOperand(Type type,RegisterStorage? index, int uAddr) : base(PrimitiveType.Byte)
    {
        Offset = uAddr;
        Index = index;
        addressingMode = type;
    }

    public int Offset { get; }
    public RegisterStorage? Index { get; }

    public static MemoryOperand Direct(uint uAddr)
    {
        return new MemoryOperand(Type.Direct, null, (int)uAddr);
    }

    public static MachineOperand DirectIndexed(RegisterStorage x, byte uAddr)
    {
        return new MemoryOperand(Type.DirectIndexed, x, uAddr);
    }

    internal static MachineOperand IndirectPostinc(byte uAddr)
    {
        return new MemoryOperand(Type.IndirectPostinc, null, uAddr);
    }

    internal static MachineOperand RegDirect(byte uAddr)
    {
        return new MemoryOperand(Type.RegDirect, null, uAddr);
    }

    internal static MachineOperand RegDirectIndexed(RegisterStorage x, byte uAddr)
    {
        return new MemoryOperand(Type.RegDirectIndexed, x, uAddr);
    }


    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        renderer.WriteChar('[');
        if (this.addressingMode == Type.IndirectPostinc)
        {
            renderer.WriteChar('[');
        }
        if (Index is null)
        {
            renderer.WriteString($"0x{this.Offset:X}");
        }
        else
        {
            renderer.WriteString(Index.Name);
            if (Offset != 0)
            {
                renderer.WriteString($"+0x{this.Offset:X}");
            }
        }
        if (this.addressingMode == Type.IndirectPostinc)
        {
            renderer.WriteString("]++");
        }
        renderer.WriteChar(']');
    }
}
