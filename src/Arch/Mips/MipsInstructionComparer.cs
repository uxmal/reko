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

using Reko.Arch.Mips.Machine;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.Mips
{
    public class MipsInstructionComparer : InstructionComparer
    {
        private const int MultiRegCode = 0x11;
        public MipsInstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
        {
            switch (op1) {
            case RegisterStorage rA:
                if (NormalizeRegisters)
                    return true;
                var rB = (RegisterStorage)op2;
                return rA == rB;
            case Constant iA:
                if (NormalizeConstants)
                    return true;
                var iB = (Constant)op2;
                return CompareConstants(iA, iB);
            case Address aA:
                if (NormalizeConstants)
                    return true;
                var aB = (Address)op2;
                return aA.ToLinear() == aB.ToLinear();
            case MemoryOperand mA:
                var mB = (MemoryOperand)op2;
                if (!NormalizeRegisters && mA.Base != mB.Base)
                    return false;
                if (mA.Offset is not null)
                {
                    if (mB.Offset is null)
                        return false;   
                    if (!NormalizeConstants && mA.Offset != mB.Offset)
                        return false;
                }
                else
                {
                    if (mA.Index is null || mA.Index != mB.Index)
                        return false;
                }
                return true;
            case MultiRegisterOperand mreg:
                return CompareMultiRegisterOperands(mreg, (MultiRegisterOperand) op2);
            }
            throw new NotImplementedException(op1.GetType().Name);
        }

        private bool CompareMultiRegisterOperands(MultiRegisterOperand mreg1, MultiRegisterOperand mreg2)
        {
            return mreg1.Bitmask == mreg2.Bitmask;
        }

        public override int GetOperandHash(MachineOperand op)
        {
            switch (op)
            {
            case RegisterStorage r:
                if (NormalizeRegisters)
                    return 0;
                else
                    return GetRegisterHash(r);
            case Constant i:
                if (NormalizeConstants)
                    return 0;
                else
                    return GetConstantHash(i);
            case Address a:
                if (NormalizeConstants)
                    return 0;
                else
                    return a.GetHashCode();
            case MemoryOperand m:
                int h = 0;
                if (!NormalizeRegisters)
                    h = GetRegisterHash(m.Base);
                if (!NormalizeConstants && m.Offset is not null)
                    h ^= m.Offset.GetHashCode();
                if (m.Index is not null)
                    h ^= GetRegisterHash(m.Index);
                return h;
            case MultiRegisterOperand mreg:
                return MultiRegCode * 17 ^ (int)mreg.Bitmask;
            }
            throw new NotImplementedException(op.GetType().Name);
        }
    }
}