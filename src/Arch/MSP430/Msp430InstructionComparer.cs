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
using Reko.Core.Machine;

namespace Reko.Arch.Msp430
{
    internal class Msp430InstructionComparer : InstructionComparer 
    {
        public Msp430InstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (Msp430Instruction)x;
            var b = (Msp430Instruction)y;
            return CompareOperands(a.Operands[0], b.Operands[0]) && CompareOperands(a.Operands[1], b.Operands[1]);
        }

        private bool CompareOperands(MachineOperand op1, MachineOperand op2)
        {
            if (op1 is null && op2 is null)
                return true;
            if (op1 is null || op2 is null)
                return false;
            if (op1.GetType() != op2.GetType())
                return false;
            if (op1 is RegisterStorage r1)
            {
                var r2 = (RegisterStorage) op2;
                return CompareRegisters(r1, r2);
            }
            throw new NotImplementedException();
        }

        public override int GetOperandsHash(MachineInstruction instr)
        {
            var a = (Msp430Instruction)instr;
            var h = GetOperandHash(a.Operands[0]) ^ GetOperandHash(a.Operands[1]) * 37;
            return h;
        }

        private int GetOperandHash(MachineOperand op)
        {
            var h = op.GetType().GetHashCode();
            if (op is RegisterStorage r)
            {
                return GetRegisterHash(r);
            }
            throw new NotImplementedException();
        }
    }
}