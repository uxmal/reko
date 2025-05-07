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

namespace Reko.Arch.Oki.NX8_500;

internal class NX8_500InstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;

    public NX8_500InstructionComparer(Normalize norm) : base(norm)
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
            _ => throw new NotImplementedException(op1.GetType().Name)
        };
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
    {
        if (!CompareRegisters(m1.Segment, m2.Segment))
            return false;
        if (!CompareRegisters(m1.Base, m2.Base))
            return false;
        if (!CompareRegisters(m1.Index, m2.Index))
            return false;
        if (!CompareConstants(m1.Offset, m2.Offset))
            return false;
        return m1.PostDecrement == m2.PostDecrement &&
               m1.PostIncrement == m2.PostIncrement;
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            MemoryOperand m => GetMemoryHash(m),
            _ => throw new NotImplementedException(op.GetType().Name)
        };
    }

    private int GetMemoryHash(MemoryOperand m)
    {
        int hash = MemCode;
        hash = (hash * 31) ^ GetRegisterHash(m.Segment);
        hash = (hash * 17) ^ GetRegisterHash(m.Base);
        hash = (hash * 23) ^ GetRegisterHash(m.Index);
        hash = (hash * 17) ^ GetConstantHash(m.Offset);
        hash = (hash * 7) ^ (m.PostDecrement ? 1 : 0);
        hash = hash ^ (m.PostDecrement ? 1 : 0);
        return hash;
    }
}
