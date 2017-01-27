#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

namespace Reko.Arch.M68k
{
    internal class M68kInstructionComparer : InstructionComparer
    {
        public M68kInstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (M68kInstruction)x;
            var b = (M68kInstruction)y;
            return
                Compare(a.op1, a.op1) &&
                Compare(a.op2, a.op2) &&
                Compare(a.op3, a.op3);
        }

        private bool Compare(MachineOperand opA, MachineOperand opB)
        {
            if (opA == null && opB == null)
                return true;
            if (opA == null || opB == null)
                return false;

            if (opA.GetType() != opB.GetType())
                return false;
            var regA = opA as RegisterOperand;
            if (regA != null)
            {
                var regB = opB as RegisterOperand;
                return NormalizeRegisters || regA.Register == regB.Register;
            }
            var immA = opA as ImmediateOperand;
            if (immA != null)
            {
                var immB = opB as ImmediateOperand;
                return CompareValues(immA.Value, immB.Value);
            }
            throw new NotImplementedException();
        }

        public override int GetOperandsHash(MachineInstruction instr)
        {
            var i = (M68kInstruction)instr;
            return
                OperandHash(i.op1) * 23 ^
                OperandHash(i.op2) * 29 ^
                OperandHash(i.op3) * 9;
        }

        private int OperandHash(MachineOperand op)
        {
            var rop = op as RegisterOperand;
            if (rop != null)
            {
                if (NormalizeRegisters)
                    return 0;
                else
                    return rop.Register.GetHashCode();
            }
            var immop = op as ImmediateOperand;
            if (immop != null)
            {
                if (NormalizeConstants)
                    return 0;
                else
                    return immop.Value.GetHashCode();
            }
            throw new NotImplementedException();

        }
    }
}