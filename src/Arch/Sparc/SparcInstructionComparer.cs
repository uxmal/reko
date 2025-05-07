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

namespace Reko.Arch.Sparc;

public class SparcInstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;
    private const int CondCode = 0x11;
        public SparcInstructionComparer(Normalize norm) : base(norm)
        {
        }

    public override bool DoCompareOperands(MachineOperand a, MachineOperand b)
        {
            switch (a)
            {
            case RegisterStorage rA:
                return CompareRegisters(rA, (RegisterStorage) b);
            case Constant immA:
            return CompareConstants(immA, (Constant) b);
            case Address addrA:
            return CompareAddresses(addrA, (Address) b);
            case MemoryOperand mA:
                var mB = (MemoryOperand) b;
                if (!CompareRegisters(mA.Base, mB.Base))
                    return false;
                if (mA.Offset is not null)
                {
                    return mA.IntOffset() == mB.IntOffset();
                }
                else
                {
                    return CompareRegisters(mA.Index, mB.Index);
                }
        case ConditionOperand<ConditionField> cA:
            var cB = (ConditionOperand<ConditionField>) b;
            return cA.Condition == cB.Condition;
            }
        throw new NotImplementedException(a.GetType().Name);
        }

    public override int GetOperandHash(MachineOperand op)
        {
            switch (op)
            {
            case RegisterStorage r:
                return GetRegisterHash(r);
            case Constant i:
                return GetConstantHash(i);
            case Address a:
                return a.GetHashCode();
            case MemoryOperand m:
            var h = MemCode ^ GetRegisterHash(m.Base);
                h = h ^ 29 * m.IntOffset();
                h = h ^ 59 * GetRegisterHash(m.Index);
                return h;
        case ConditionOperand<ConditionField> c:
            return CondCode * 17 ^ (int) c.Condition;
            }
        throw new NotImplementedException(op.GetType().Name);
        }
}