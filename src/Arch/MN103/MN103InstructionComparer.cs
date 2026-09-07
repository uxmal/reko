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

namespace Reko.Arch.MN103;

public class MN103InstructionComparer : InstructionComparer
{
    public MN103InstructionComparer(Normalize norm) : base(norm)
    {
    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        switch (op1)
        {
        case RegisterStorage reg1:
            return base.CompareRegisters(reg1, (RegisterStorage) op2);
        case Constant imm1:
            return base.CompareConstants(imm1, (Constant) op2);
        case Address addr1:
            return base.CompareAddresses(addr1, (Address) op2);
        case MemoryOperand mem1:
            var mem2 = (MemoryOperand) op2;
            if (!CompareRegisters(mem1.Base, mem2.Base))
                return false;
            if (mem1.Displacement != mem2.Displacement)
                return false;
            return CompareRegisters(mem1.Index, mem2.Index);
        }
        throw new System.NotImplementedException();
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => base.GetRegisterHash(reg),
            Constant imm => base.GetConstantHash(imm),
            Address addr => base.GetAddressHash(addr),
            MemoryOperand mem => HashCode.Combine(
                mem.Displacement,
                base.GetRegisterHash(mem.Base),
                base.GetRegisterHash(mem.Index)),
            _ => throw new System.NotImplementedException()
        };
    }
}