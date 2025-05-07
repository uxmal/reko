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

namespace Reko.Arch.RiscV;

public class RiscVInstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;

        public RiscVInstructionComparer(Normalize norm) : base(norm)
        {
        }

    public override bool DoCompareOperands(MachineOperand opA, MachineOperand opB)
        {
            switch (opA)
            {
            case RegisterStorage ropA:
            return CompareRegisters(ropA, (RegisterStorage) opB);
            case Constant immA:
            return CompareConstants(immA, (Constant) opB);
            case Address addrA:
            return CompareAddresses(addrA, (Address)opB);
        case MemoryOperand memA:
            return CompareMemoryOperands(memA, (MemoryOperand) opB);
            default:
            throw new NotImplementedException(opA.GetType().Name);
            }
        }

    private bool CompareMemoryOperands(MemoryOperand memA, MemoryOperand memB)
        {
        if (!CompareRegisters(memA.Base, memB.Base))
            return false;
        return CompareOperands(memA.Offset, memB.Offset);
        }

    public override int GetOperandHash(MachineOperand op)
        {
            return op switch
            {
            RegisterStorage rop => GetRegisterHash(rop),
            Constant immop => GetConstantHash(immop),
            Address aop => GetAddressHash(aop),
            MemoryOperand mop => GetMemoryHash(mop),
            _ => throw new NotImplementedException(
                    string.Format("RiscV operand {0} ({1}) not implemented.", op, op.GetType().Name))
            };
        }

    private int GetMemoryHash(MemoryOperand mop)
    {
        int hash = MemCode;
        hash = hash * 17 ^ GetRegisterHash(mop.Base);
        hash = hash * 7 ^ GetOperandHash(mop.Offset);
        return hash;
    }
}