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

namespace Reko.Arch.Blackfin;

internal class BlackfinInstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;
    private const int RegRangeCode = 0x11;

    public BlackfinInstructionComparer(Normalize norm) : base(norm)
    {
    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        return op1 switch
        {
            RegisterStorage reg1 => CompareRegisters(reg1, (RegisterStorage) op2),
            Constant c1 => CompareConstants(c1, (Constant) op2),
            Address a1 => CompareAddresses(a1, (Address) op2),
            MemoryOperand m1 => CompareMemoryOperand(m1, (MemoryOperand)op2),
            RegisterRange rr => CompareRegisterRange(rr, (RegisterRange) op2),
            _ => throw new NotImplementedException(op1.GetType().Name)
        };
    }

    private bool CompareMemoryOperand(MemoryOperand m1, MemoryOperand m2)
    {
        if (!CompareRegisters(m1.Base, m2.Base))
            return false;
        if (!NormalizeConstants && m1.Offset != m2.Offset)
            return false;
        if (!CompareRegisters(m1.Index, m2.Index))
            return false;
        return m1.PreDecrement == m2.PreDecrement &&
               m1.PostDecrement == m2.PostDecrement &&
               m1.PostIncrement == m2.PostIncrement;
    }

    private bool CompareRegisterRange(RegisterRange rr1, RegisterRange rr2)
    {
        if (rr1.Registers.Length != rr2.Registers.Length)
            return false;
        for (int i = 0; i < rr1.Registers.Length; ++i)
        {
            if (rr1.Registers[i] != rr2.Registers[i])
                return false;
        }
        return true;
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            MemoryOperand m => GetMemoryOperandHash(m),
            RegisterRange rr => GetRegisterRangeHash(rr),
            _ => throw new NotImplementedException(op.GetType().Name)
        };
    }

    private int GetRegisterRangeHash(RegisterRange rr)
    {
        int hash = RegRangeCode;
        foreach (var reg in rr.Registers)
        {
            hash = hash * 17 ^ reg.Number;
        }
        return hash;
    }

    private int GetMemoryOperandHash(MemoryOperand m)
    {
        int hash = MemCode;
        hash = hash * 17 ^ GetRegisterHash(m.Base);
        hash = hash * 23 ^ GetRegisterHash(m.Index);
        hash = hash * 19 ^ m.Offset;
        hash = hash * 7;
        hash ^= m.PreDecrement ? 4 : 0;
        hash ^= m.PostDecrement ? 2 : 0;
        hash ^= m.PostIncrement ? 1 : 0;
        return hash;
    }
}