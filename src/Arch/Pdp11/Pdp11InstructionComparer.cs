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

namespace Reko.Arch.Pdp11
{
    public class Pdp11InstructionComparer : InstructionComparer
    {
        public Pdp11InstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (Pdp11Instruction)x;
            var b = (Pdp11Instruction)y;
            return
                Compare(a.op1, b.op1) &&
                Compare(a.op1, b.op2);
        }

        private bool Compare(MachineOperand opA, MachineOperand opB)
        {
            if (opA == null && opB == null)
                return true;
            if (opA == null || opB == null)
                return false;
            if (opA.GetType() != opB.GetType())
                return false;
            var ropA = opA as RegisterOperand;
            if (ropA != null)
            {
                var ropB = (RegisterOperand)opB;
                return ropA.Register == ropB.Register; 
            }
            var immA = opA as ImmediateOperand;
            if (immA != null)
            {
                var immB = (ImmediateOperand)opB;
                return CompareValues(immA.Value, immB.Value);
            }
            throw new NotImplementedException();
        }

        public override int GetOperandsHash(MachineInstruction i)
        {
            var instr = (Pdp11Instruction)i;
            return
                Hash(instr.op1) * 23 ^
                Hash(instr.op2);
        }

        private int Hash(MachineOperand op)
        {
            if (op == null)
                return 0;
            int hash = op.GetType().GetHashCode();
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
                if (NormalizeRegisters)
                    return 0;
                else 
                    return immop.Value.GetHashCode();
            }
            var mem = op as MemoryOperand;
            if (mem != null)
            {
                var r = NormalizeRegisters || mem.Register == null
                    ? 0
                    : mem.Register.GetHashCode();
                var o = NormalizeConstants
                    ? 0
                    : mem.EffectiveAddress.GetHashCode();
                return mem.Mode.GetHashCode() ^
                    r * 17 ^
                    o * 5;
            }
            throw new NotImplementedException();
        }
    }
}