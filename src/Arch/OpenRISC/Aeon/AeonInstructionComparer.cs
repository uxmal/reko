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

namespace Reko.Arch.OpenRISC.Aeon;

internal class AeonInstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;

    public AeonInstructionComparer(Normalize norm)
        : base(norm)
    {
    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        if (op1.GetType() != op2.GetType())
            return false;
        switch (op1)
        {
        case RegisterStorage ro1:
            return CompareRegisters(ro1, op2 as RegisterStorage);
        case Constant co1:
            return CompareConstants(co1, op2 as Constant);
        case Address addr1:
            return CompareAddresses(addr1, (Address)op2);
        case MemoryOperand m1:
            return CompareMemoryOperands(m1, (MemoryOperand) op2);
        }
        throw new NotImplementedException(op1.GetType().Name);
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand op2)
    {
        return
            CompareRegisters(m1.Base, op2.Base) &&
            CompareConstants(m1.Offset, op2.Offset);
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            MemoryOperand m => GetMemoryHash(m),
            _ => throw new NotImplementedException(op.GetType().Name),
        };
    }

    private int GetMemoryHash(MemoryOperand m)
    {
        int hash = MemCode;
        hash = hash * 17 ^ GetRegisterHash(m.Base);
        hash = hash * 17 ^ GetConstantHash(m.Offset);
        return hash;
    }
}