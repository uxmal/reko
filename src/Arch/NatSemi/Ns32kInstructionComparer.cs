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

namespace Reko.Arch.NatSemi;

public class Ns32kInstructionComparer : InstructionComparer
{
    private const int MemoryCode = 0x10;
    private const int RegisterSetCode = 0x10;

    public Ns32kInstructionComparer(Normalize norm) : base(norm)
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
            LiteralOperand l1 => CompareLiterals(l1, (LiteralOperand) op2),
            RegisterSetOperand rs1 => CompareRegisterSets(rs1, (RegisterSetOperand) op2),
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
            LiteralOperand l => GetLiteralHash(l),
            RegisterSetOperand rs => GetRegisterSetHash(rs),
            _ => throw new NotImplementedException($"{op.GetType().Name} not implemented.")
        };
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
    {
        if (!CompareOperands(m1.Base, m2.Base))
            return false;
        if (!CompareOperands(m1.Displacement, m2.Displacement))
            return false;
        return m1.Scale == m2.Scale;
    }

    private int GetMemoryOperandHash(MemoryOperand m)
    {
        int h = MemoryCode;
        h ^= 17 * (m.Base is not null
            ? GetOperandHash(m.Base)
            : 0);
        h ^= 23 * (m.Displacement is not null
            ? GetOperandHash(m.Displacement)
            : 0);
        h ^= 31 * m.Scale;
        return h;
    }

    private bool CompareRegisterSets(RegisterSetOperand rs1, RegisterSetOperand op2)
    {
        if (NormalizeRegisters)
            return true;
        return rs1.RegisterSet == op2.RegisterSet;
    }

    private int GetRegisterSetHash(RegisterSetOperand rs)
    {
        int h = RegisterSetCode;
        if (!NormalizeRegisters)
        {
            h = h * 17 ^ rs.RegisterSet;
        }
        return h;
    }

}
