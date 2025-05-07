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

namespace Reko.Arch.Maxim;

internal class MaxqInstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;
    private const int ModRegCode = 0x11;
    private const int CondCode = 0x12;

    public MaxqInstructionComparer(Normalize norm) : base(norm)
    {
    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        return op1 switch
        {
            RegisterStorage reg1 => CompareRegisters(reg1, (RegisterStorage) op2),
            Constant c1 => CompareConstants(c1, (Constant) op2),
            Address a1 => CompareAddresses(a1, (Address) op2),
            FlagGroupStorage grf1 => CompareFlagGroups(grf1, (FlagGroupStorage) op2),
            MemoryOperand m1 => CompareMemoryOperands(m1, (MemoryOperand) op2),
            ModuleRegister mod1 => CompareModuleRegisters(mod1, (ModuleRegister) op2),
            ConditionOperand<CCode> cc1 => CompareConditionOperands(cc1, (ConditionOperand<CCode>) op2),
            BitOperand b1 => CompareBitOperands(b1, (BitOperand) op2),
            _ => throw new NotImplementedException(op1.GetType().Name)
        };
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
    {
        if (!CompareRegisters(m1.Base, m2.Base))
            return false;
        if (!CompareOperands(m1.Offset, m2.Offset))
            return false;
        return m1.Increment == m2.Increment;
    }

    private bool CompareConditionOperands(ConditionOperand<CCode> cc1, ConditionOperand<CCode> op2)
    {
        return cc1.Condition == op2.Condition;
    }

    private bool CompareModuleRegisters(ModuleRegister mod1, ModuleRegister op2)
    {
        return mod1.Module == op2.Module &&
               mod1.Index == op2.Index;
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            FlagGroupStorage grf => GetFlagGroupHash(grf),
            MemoryOperand m => GetMemoryHash(m),
            ModuleRegister mod => GetModuleRegisterHash(mod),
            ConditionOperand<CCode> cc => GetConditionHash(cc),
            BitOperand b => GetBitOperandHash(b),
            _ => throw new NotImplementedException(op.GetType().Name)
        };
    }

    private int GetMemoryHash(MemoryOperand m)
    {
        int hash = MemCode;
        hash = hash * 17 ^ GetRegisterHash(m.Base);
        hash = hash * 17 ^ (m.Offset is null ? 0 : GetOperandHash(m.Offset));
        hash = hash * 17 ^ m.Increment.GetHashCode();
        return hash;
    }

    private int GetConditionHash(ConditionOperand<CCode> cc)
    {
        int hash = CondCode * 17 ^ (int) cc.Condition;
        return hash;
    }

    private int GetModuleRegisterHash(ModuleRegister mod)
    {
        int hash = ModRegCode;
        hash = hash * 17 ^ GetConstantHash(mod.Module);
        hash = hash * 17 ^ GetConstantHash(mod.Index);
        return hash;
    }
}