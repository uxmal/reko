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

namespace Reko.Arch.H8;

internal class H8InstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;
    private const int RegListCode = 0x11;

    public H8InstructionComparer(Normalize norm) : base(norm)
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
            RegisterListOperand rl1 => CompareRegisterLists(rl1, (RegisterListOperand) op2),
            _ => throw new NotImplementedException(op1.GetType().Name)
        };
    }

    private bool CompareRegisterLists(RegisterListOperand rl1, RegisterListOperand rl2)
    {
        return
            rl1.RegisterNumber == rl2.RegisterNumber &&
            rl1.Count == rl2.Count;
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand op2)
    {
        if (!CompareRegisters(m1.Base, op2.Base))
            return false;
        if (!CompareConstants(m1.Offset, op2.Offset))
            return false;
        return m1.PreDecrement == op2.PreDecrement &&
               m1.PostIncrement == op2.PostIncrement &&
               m1.AddressWidth == op2.AddressWidth &&
               m1.Deferred == op2.Deferred;
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            MemoryOperand m => GetMemoryHash(m),
            RegisterListOperand rl => GetRegisterListHash(rl),
            _ => throw new NotImplementedException(op.GetType().Name)
        };
    }

    private int GetRegisterListHash(RegisterListOperand rl)
    {
        int hash = RegListCode;
        hash = hash * 17 ^ rl.RegisterNumber;
        hash = hash * 17 ^ rl.Count;
        return hash;
    }

    private int GetMemoryHash(MemoryOperand m)
    {
        int hash = MemCode;
        hash = hash * 17 ^ GetRegisterHash(m.Base);
        hash = hash * 31 ^ GetConstantHash(m.Offset);
        hash = hash * 5;
        hash = hash ^ (m.PreDecrement ? 4 : 0);
        hash = hash ^ (m.PostIncrement ? 2 : 0);
        hash = hash ^ (m.Deferred ? 1 : 0);
        return hash;
    }
}