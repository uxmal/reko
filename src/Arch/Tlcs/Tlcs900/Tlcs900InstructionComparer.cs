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

namespace Reko.Arch.Tlcs.Tlcs900
{
    public class Tlcs900InstructionComparer : InstructionComparer
    {
        private Normalize norm;

        public Tlcs900InstructionComparer(Normalize norm) : base(norm)
        {
            this.norm = norm;
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (Tlcs900Instruction)x;
            var b = (Tlcs900Instruction)y;
            return CompareOp(a.Operands[0], b.Operands[0]) &&
                   CompareOp(a.Operands[1], b.Operands[1]);
        }

        private bool CompareOp(MachineOperand opA, MachineOperand opB)
        {
            if (opA is null && opB is null)
                return true;
            if (opA is null || opB is null)
                return false;
            if (opA.GetType() != opB.GetType())
                return false;
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
                return CompareValues(immOpA, immOpB);
            case Address addrOpA:
                if (NormalizeConstants)
                    return true;
                var addrOpB = (Address)opB;
                return addrOpA.ToLinear() == addrOpB.ToLinear();
            case ConditionOperand<CondCode> condOpA:
                return condOpA.Condition == ((ConditionOperand<CondCode>)opB).Condition;
            case MemoryOperand memOpA:
                var memOpB = (MemoryOperand) opB;
                if (NormalizeRegisters && !CompareRegisters(memOpA.Base, memOpB.Base))
                    return false;
                if (NormalizeConstants && !CompareValues(memOpA.Offset, memOpB.Offset))
                    return false;
                return true;
            }
            throw new NotImplementedException();
        }

        public override int GetOperandsHash(MachineInstruction instr)
        {
            var tinstr = (Tlcs900Instruction)instr;
            return
                HashOp(tinstr.Operands[0]) ^
                HashOp(tinstr.Operands[1]) * 17;
        }

        private int HashOp(MachineOperand op)
        {
            if (op is null)
                return 0;
            int h = op.GetType().GetHashCode();
            if (op is RegisterStorage regOp)
            {
                if (NormalizeRegisters)
                    return h;
                else
                    return h * 29 ^ regOp.GetHashCode();
            }
            if (op is Constant immOp)
            {
                if (NormalizeConstants)
                    return h;
                else
                    return h * 13 ^ GetConstantHash(immOp);
            }
            if (op is Address addrOp)
            {
                if (NormalizeConstants)
                    return h;
                else
                    return h * 29 ^ addrOp.GetHashCode();
            }
            if (op is ConditionOperand<CondCode> condOp)
            {
                return h * 19 ^ condOp.Condition.GetHashCode();
            }
            if (op is MemoryOperand memOp)
            {
                if (!NormalizeRegisters && memOp.Base is not null)
                    h = h * 23 ^ memOp.Base.GetHashCode();
                if (!NormalizeConstants && memOp.Offset is not null)
                    h = h * 17 ^ GetConstantHash(memOp.Offset);
                return h;
            }
            throw new NotImplementedException(string.Format("{0} ({1})", op, op.GetType().Name));
        }
    }
}