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

namespace Reko.Arch.Infineon.M8C;

public class M8CInstructionComparer : InstructionComparer
{
    public M8CInstructionComparer(Normalize norm) : base(norm)
    {
    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        return op1 switch
        {
            RegisterStorage r1 => CompareRegisters(r1, (RegisterStorage) op2),
            Constant c1 => CompareConstants(c1, (Constant) op2),
            Address a1 => CompareAddresses(a1, (Address) op2),
            MemoryOperand m1 => CompareMemoryOperands(m1, (MemoryOperand) op2),
            _ => throw new NotImplementedException($"{op1.GetType().Name} not implemented.")
        };
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage r => GetRegisterHash(r),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            MemoryOperand m => GetMemoryOperandHash(m),
            _ => throw new NotImplementedException($"{op.GetType().Name} not implemented.")
        };
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
    {
        if (m1.AddressingMode != m2.AddressingMode)
            return false;
        if (m1.Offset != m2.Offset && !base.NormalizeConstants)
            return false;
        return CompareRegisters(m1.Index, m2.Index);
    }

    private int GetMemoryOperandHash(MemoryOperand m)
    {
        int h = (int) m.AddressingMode;
        h = h * 31 ^ (NormalizeConstants ? 0 : m.Offset);
        h = h * 17 ^ GetRegisterHash(m.Index);
        return h;
    }
}
