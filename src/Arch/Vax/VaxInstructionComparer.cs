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

namespace Reko.Arch.Vax;

internal class VaxInstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;
    private const int IndexCode = 0x20;

    public VaxInstructionComparer(Normalize norm) : base(norm)
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
            IndexOperand ix1 => CompareIndexOperands(ix1, (IndexOperand) op2),
            _ => throw new NotImplementedException(op1.GetType().Name)
        };
    }

    private bool CompareIndexOperands(IndexOperand ix1, IndexOperand ix2)
    { 
        if (!CompareOperands(ix1.Base, ix2.Base))
            return false;
        return CompareRegisters(ix1.Index, ix2.Index);
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
    {
        if (m1.Deferred != m2.Deferred ||
            m1.AutoIncrement != m2.AutoIncrement ||
            m1.AutoDecrement != m2.AutoDecrement)
            return false;

        if (!CompareConstants(m1.Offset, m2.Offset))
            return false;
        return CompareOperands(m1.Base, m2.Base);
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            MemoryOperand m => GetMemoryHash(m),
            IndexOperand ix => GetIndexHash(ix),
            _ => throw new NotImplementedException(op.GetType().Name)
        };
    }

    private int GetIndexHash(IndexOperand ix)
    {
        int hash = IndexCode;
        hash = hash * 17 ^ GetOperandHash(ix.Base);
        hash = hash * 17 ^ GetRegisterHash(ix.Index);
        return hash;
    }

    private int GetMemoryHash(MemoryOperand m)
    {
        int hash = MemCode;
        hash = hash * 17 ^ (m.Base is null ? 0 : GetOperandHash(m.Base));
        hash = hash * 7 ^ GetConstantHash(m.Offset);
        hash = hash ^ (m.Deferred ? 4 : 0);
        hash = hash ^ (m.AutoIncrement ? 2 : 0);
        hash = hash ^ (m.AutoDecrement ? 1 : 0);
        return hash;
    }
}