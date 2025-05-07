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

namespace Reko.Arch.M16C;

internal class M16CInstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;
    private const int MultiRegCode = 0x11;

    public M16CInstructionComparer(Normalize norm) : base(norm)
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
            FlagGroupStorage grf1 => CompareFlagGroups(grf1, (FlagGroupStorage) op2),
            SequenceStorage s1 => CompareSequences(s1, (SequenceStorage) op2),
            MultiRegisterOperand mr1 => CompareMultiRegisters(mr1, (MultiRegisterOperand) op2),
            _ => throw new NotImplementedException(op1.GetType().Name)
        };
    }

    private bool CompareMultiRegisters(MultiRegisterOperand mr1, MultiRegisterOperand mr2)
    {
        return mr1.BitPattern == mr2.BitPattern;
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
    {
        if (!CompareStorages(m1.Base, m2.Base))
            return false;
        return CompareConstants(m1.Offset, m2.Offset);
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            MemoryOperand m => GetMemoryHash(m),
            FlagGroupStorage grf => GetFlagGroupHash(grf),
            SequenceStorage s => GetSequenceHash(s),
            MultiRegisterOperand mr => GetMultiRegisterHash(mr),
            _ => throw new NotImplementedException(op.GetType().Name)
        };
    }

    private int GetMultiRegisterHash(MultiRegisterOperand mr)
    {
        int hash = MultiRegCode;
        hash = hash * 17 ^ (int) mr.BitPattern;
        return hash;
    }

    private int GetMemoryHash(MemoryOperand m)
    {
        int hash = MemCode;
        hash = hash * 17 ^ GetStorageHash(m.Base);
        hash = hash * 17 ^ GetConstantHash(m.Offset);
        return hash;
    }
}