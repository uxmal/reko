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

namespace Reko.Arch.C166;

internal class C166InstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;
    private const int CondCode = 0x11;

    public C166InstructionComparer(Normalize norm) : base(norm)
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
            BitOperand b1 => CompareBitOperands(b1, (BitOperand) op2),
            ConditionOperand<CondCode> cc1 => CompareConditionOperands(cc1, (ConditionOperand<CondCode>) op2),
            _ => throw new NotImplementedException(op1.GetType().Name)
        };
    }

    private bool CompareConditionOperands(ConditionOperand<CondCode> cc1, ConditionOperand<CondCode> cc2)
    {
        return cc1.Condition == cc2.Condition;
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
    {
        if (!CompareRegisters(m1.Base, m2.Base))
            return false;
        if (!NormalizeConstants && m1.Offset != m2.Offset)
            return false;
        return m1.Predecrement == m2.Predecrement &&
               m1.Postincrement == m2.Postincrement;
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            MemoryOperand m => GetMemoryOperandHash(m),
            BitOperand b => GetBitOperandHash(b),
            ConditionOperand<CondCode> cc => GetConditionHash(cc),
            _ => throw new NotImplementedException(op.GetType().Name)
        };
    }

    private int GetConditionHash(ConditionOperand<CondCode> cc)
    {
        int hash = CondCode;
        hash = hash * 17 ^ (int) cc.Condition;
        return hash;
    }

    private int GetMemoryOperandHash(MemoryOperand m)
    {
        int hash = MemCode;
        hash = hash * 17 ^ GetRegisterHash(m.Base);
        hash = hash * 23 ^ (NormalizeConstants ? 0 : m.Offset);
        hash = hash * 7;
        hash ^= m.Predecrement ? 2 : 0;
        hash ^= m.Postincrement? 1 : 0;
        return hash;
    }
}