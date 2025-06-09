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

namespace Reko.Arch.RiscV
{
    public class RiscVInstructionComparer : InstructionComparer
    {
        public RiscVInstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (RiscVInstruction)x;
            var b = (RiscVInstruction)y;
            return
                CompareOperands(a.Operands[0], b.Operands[0]) &&
                CompareOperands(a.Operands[1], b.Operands[1]) &&
                CompareOperands(a.Operands[2], b.Operands[2]);
        }

        private bool CompareOperands(MachineOperand opA, MachineOperand opB)
        {
            if (opA is null && opB is null)
                return true;
            if (opA is null || opB is null)
                return false;
            if (opA.GetType() != opB.GetType())
                return false;

            switch (opA)
            {
            case RegisterStorage ropA:
                var ropB = (RegisterStorage) opB;
                return NormalizeRegisters || ropA == ropB;
            case Constant immA:
                var immB = (Constant) opB;
                return NormalizeConstants || base.CompareValues(immA, immB);
            case Address addrA:
                var addrB = (Address) opB;
                return NormalizeConstants || addrA == addrB;
            default:
                throw new NotImplementedException();
            }
        }

        public override int GetOperandsHash(MachineInstruction instr)
        {
            var i = (RiscVInstruction)instr;
            int hash =
                GetOperandHash(i.Operands[0]) * 23 ^
                GetOperandHash(i.Operands[1]) * 19 ^
                GetOperandHash(i.Operands[2]);
            return hash;
        }

        private int GetOperandHash(MachineOperand op)
        {
            return op switch
            {
                RegisterStorage rop =>
                    (NormalizeRegisters)
                        ? 0
                        : rop.Number.GetHashCode(),

                Constant immop =>
                    (NormalizeConstants)
                        ? 0
                        : base.GetConstantHash(immop),

                Address aop =>
                    (NormalizeConstants)
                        ? 0
                        : aop.GetHashCode(),
                null => 0,
                _ =>
                throw new NotImplementedException(
                    string.Format("RiscV operand {0} ({1}) not implemented.", op, op.GetType().Name))
            };
        }
    }
}