#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;

namespace Reko.Arch.Mips
{
    public class MipsInstructionComparer : InstructionComparer
    {
        public MipsInstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (MipsInstruction)x;
            var b = (MipsInstruction)y;
            return Compare(a.Operands[0], b.Operands[0]) &&
                   Compare(a.Operands[1], b.Operands[1]) &&
                   Compare(a.Operands[2], b.Operands[2]) &&
                   Compare(a.Operands[3], b.Operands[3]);
        }

        private bool Compare(MachineOperand a, MachineOperand b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            if (a.GetType() != b.GetType())
                return false;
            switch (a) {
            case RegisterStorage rA:
                if (NormalizeRegisters)
                    return true;
                var rB = (RegisterStorage)b;
                return rA == rB;
            case Constant iA:
                if (NormalizeConstants)
                    return true;
                var iB = (Constant)b;
                return CompareValues(iA, iB);
            case Address aA:
                if (NormalizeConstants)
                    return true;
                var aB = (Address)b;
                return aA.ToLinear() == aB.ToLinear();
            case MemoryOperand mA:
                var mB = (MemoryOperand)b;
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
            }
            throw new NotImplementedException();
        }

        public override int GetOperandsHash(MachineInstruction oinstr)
        {
            int h = 0;
            var instr = (MipsInstruction)oinstr;
            h = h*23 ^ GetHashCode(instr.Operands[0]);
            h = h*23 ^ GetHashCode(instr.Operands[1]);
            h = h*23 ^ GetHashCode(instr.Operands[2]);
            h = h*23 ^ GetHashCode(instr.Operands[3]);
            return h;
        }

        private int GetHashCode(MachineOperand op)
        {
            if (op is null)
                return 0;
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
            }
            return 42;
        }
    }
}