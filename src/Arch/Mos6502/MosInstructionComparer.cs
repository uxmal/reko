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

namespace Reko.Arch.Mos6502;

internal class MosInstructionComparer : InstructionComparer
{
    public MosInstructionComparer(Normalize norm) : base(norm)
    {
    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        return op1 switch
        {
            RegisterStorage reg1 => CompareRegisters(reg1, (RegisterStorage) op2),
            Constant c1 => CompareConstants(c1, (Constant) op2),
            Address a1 => CompareAddresses(a1, (Address) op2),
            Operand op => CompareMosOperands(op, (Operand) op2),
            _ => throw new NotImplementedException(op1.GetType().Name)
        };
    }

    private bool CompareMosOperands(Operand op1, Operand op2)
    {
        if (op1.Mode != op2.Mode)
            return false;
        if (!CompareRegisters(op1.Register, op2.Register))
            return false;
        return CompareConstants(op1.Offset, op2.Offset);
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            Operand mop => GetMosOperandHash(mop),
            _ => throw new NotImplementedException(op.GetType().Name)
        };
    }

    private int GetMosOperandHash(Operand mop)
    {
        int hash = (int) mop.Mode;
        hash = hash * 17 ^ GetRegisterHash(mop.Register);
        hash = hash * 17 ^ GetConstantHash(mop.Offset);
        return hash;
    }
}