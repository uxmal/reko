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
using System;

namespace Reko.Arch.Padauk;

internal class PadaukInstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;
    private const int PortCode = 0x11;

    public PadaukInstructionComparer(Normalize norm) : base(norm)
    {
    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        return op1 switch
        {
            RegisterStorage reg1 => CompareRegisters(reg1, (RegisterStorage) op2),
            Constant c1 => CompareConstants(c1, (Constant) op2),
            Address a1 => CompareAddresses(a1, (Address) op2),
            MemoryOperand m1 => CompareMemoryOperands(m1, (MemoryOperand) op2),
            PortOperand p1 => ComparePorts(p1, (PortOperand) op2),
            _ => throw new NotImplementedException(op1.GetType().Name)
        };
    }

    private bool ComparePorts(PortOperand p1, PortOperand p2)
    {
        return
            p1.Port == p2.Port &&
            p1.Bit == p2.Bit;
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
    {
        if (!CompareConstants(m1.Offset, m2.Offset))
            return false;
        if (m1.Bit != m2.Bit)
            return false;
        return m1.Indirect == m2.Indirect;
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            MemoryOperand m => GetMemoryHash(m),
            PortOperand port => GetPortHash(port),
            _ => throw new NotImplementedException(op.GetType().Name)
        };
    }

    private int GetPortHash(PortOperand port)
    {
        int hash = PortCode;
        hash = hash * 17 ^ (int)port.Port;
        hash = hash * 17 ^ (port.Bit ?? -1) + 1;
        return hash;
    }

    private int GetMemoryHash(MemoryOperand m)
    {
        int hash = MemCode;
        hash = hash * 17 ^ GetConstantHash(m.Offset);
        hash = hash * 17 ^ (m.Bit ?? -1) + 1;
        hash = hash * 31 ^ (m.Indirect ? 1 : 0);
        return hash;
    }
}