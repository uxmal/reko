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
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;

namespace Reko.Arch.Zilog.Z80
{
    public class Z80InstructionComparer : InstructionComparer
    {
        private const int MemCode = 0x10;

        public Z80InstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool DoCompareOperands(MachineOperand opA, MachineOperand opB)
        {
            return opA switch
            {
                RegisterStorage regOpA => CompareRegisters(regOpA, (RegisterStorage) opB),
                Constant immOpA => CompareConstants(immOpA, (Constant) opB),
                Address addrOpA => CompareAddresses(addrOpA, (Address) opB),
                ConditionOperand<CondCode> condOpA =>
                    condOpA.Condition == ((ConditionOperand<CondCode>) opB).Condition,
                MemoryOperand memOpA => CompareMemoryOperand(memOpA, (MemoryOperand) opB),
                _ => throw new NotImplementedException(opA.GetType().Name)
            };
        }

        private bool CompareMemoryOperand(MemoryOperand memOpA, MemoryOperand memOpB)
        {
            if (!CompareRegisters(memOpA.Base, memOpB.Base))
                return false;
            if (!CompareConstants(memOpA.Offset, memOpB.Offset))
                return false;
                    return true;
            }

        public override int GetOperandHash(MachineOperand op)
        {
            return op switch
            {
                RegisterStorage regOp => GetRegisterHash(regOp),
                Constant immOp => GetConstantHash(immOp),
                Address addrOp => GetAddressHash(addrOp),
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
}