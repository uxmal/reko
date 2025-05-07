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

namespace Reko.Arch.Motorola.M88k;

public class M88kInstructionComparer : InstructionComparer
{
    private const int Memory = 0x10;
    private const int Condition = 0x11;

    public M88kInstructionComparer(Normalize norm) : base(norm)
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
            ConditionOperand<CCode> cc1 => CompareConditions(cc1, (ConditionOperand<CCode>) op2),
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
            ConditionOperand<CCode> cc => GetConditionHash(cc),
            _ => throw new NotImplementedException($"{op.GetType().Name} not implemented.")
        };
    }



    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
    {
        if (!CompareRegisters(m1.Base, m2.Base))
            return false;
        if (m1.Offset != m2.Offset && !NormalizeConstants)
            return false;
        if (!CompareRegisters(m1.Index, m2.Index))
            return false;
        return m1.Scale == m2.Scale;
    }

    private int GetMemoryOperandHash(MemoryOperand m)
    {
        int h = Memory;
        h = h * 31 ^ GetRegisterHash(m.Base);
        h = h * 7 ^ (NormalizeConstants ? 0 : m.Offset);
        h = h * 17 ^ GetRegisterHash(m.Index);
        h = h * 23 ^ m.Scale;
        return h;
    }

    private bool CompareConditions(ConditionOperand<CCode> cc1, ConditionOperand<CCode> cc2)
    {
        return cc1.Condition == cc2.Condition;
    }


    private int GetConditionHash(ConditionOperand<CCode> cc)
    {
        int h = Condition;
        return h ^ ((int) cc.Condition * 17);
    }
}
