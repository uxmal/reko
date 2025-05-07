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

using System;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;

namespace Reko.Arch.Tlcs.Tlcs90;

public class Tlcs90InstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;

        public Tlcs90InstructionComparer(Normalize norm) : base(norm)
        {
        }

    public override bool DoCompareOperands(MachineOperand opA, MachineOperand opB)
        {
            switch (opA)
            {
            case RegisterStorage regOpA:
                if (NormalizeRegisters)
                    return true;
                var regOpB = (RegisterStorage)opB;
                return regOpA == regOpB;
            case Constant immOpA:
                if (NormalizeConstants)
                    return true;
                var immOpB = (Constant)opB;
            return CompareConstants(immOpA, immOpB);
            case Address addrOpA:
                if (NormalizeConstants)
                    return true;
                var addrOpB = (Address) opB;
                return addrOpA.ToLinear() == addrOpB.ToLinear();
            case ConditionOperand<CondCode> condOpA:
                return condOpA.Condition == ((ConditionOperand<CondCode>)opB).Condition;
            case MemoryOperand memOpA:
                var memOpB = (MemoryOperand) opB;
                if (NormalizeRegisters && !CompareRegisters(memOpA.Base, memOpB.Base))
                    return false;
            if (NormalizeConstants && !CompareConstants(memOpA.Offset, memOpB.Offset))
                    return false;
                return true;
            }
            throw new NotImplementedException();
        }

    public override int GetOperandHash(MachineOperand op)
        {
        return op switch
        {
            RegisterStorage regOp => GetRegisterHash(regOp),
            Constant immOp => GetConstantHash(immOp),
            Address addr => GetAddressHash(addr),
            ConditionOperand<CondCode> condOp => (int) condOp.Condition,
            MemoryOperand memOp => GetMemoryOperandHash(memOp),
            _ => throw new NotImplementedException(string.Format("{0} ({1})", op, op.GetType().Name))
        };
        }

    private int GetMemoryOperandHash(MemoryOperand memOp)
        {
        int h = MemCode;
        h = h * 23 ^ GetRegisterHash(memOp.Base);
                    h = h * 17 ^ GetConstantHash(memOp.Offset);
                return h;
            }
}