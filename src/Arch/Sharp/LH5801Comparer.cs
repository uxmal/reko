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

namespace Reko.Arch.Sharp;

public class LH5801Comparer : InstructionComparer
{
    public LH5801Comparer(Normalize norm) : base(norm)
    {
    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        switch (op1)
        {
        case RegisterStorage reg1:
            return base.CompareRegisters(reg1, (RegisterStorage) op2);
        case Constant c1:
            return CompareConstants(c1, (Constant) op2);
        case Address addr1:
            return CompareAddresses(addr1, (Address) op2);
        case LiteralOperand lit1:
            return CompareLiterals(lit1, (LiteralOperand) op2);
        case MemoryOperand mem1:
            var mem2 = (MemoryOperand) op2;
            return CompareOperands(mem1.EffectiveAddress, mem2.EffectiveAddress);
        }
        throw new System.NotImplementedException(op1.GetType().Name);
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => base.GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            LiteralOperand l => GetLiteralHash(l),
            MemoryOperand m => 47 ^ GetOperandHash(m.EffectiveAddress),
            _ => throw new System.NotImplementedException()
        };
    }
}
