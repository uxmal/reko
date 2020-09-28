#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.PowerPC
{
    public class PowerPcInstructionComparer : InstructionComparer
    {
        private Normalize norm;

        public PowerPcInstructionComparer(Normalize norm) : base(norm)
        {
            this.norm = norm;
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (PowerPcInstruction)x;
            var b = (PowerPcInstruction)y;
            return
                Compare(a.Operands[0], b.Operands[0]) &&
                Compare(a.Operands[1], b.Operands[1]) &&
                Compare(a.Operands[2], b.Operands[2]) &&
                Compare(a.Operands[3], b.Operands[3]) &&
                Compare(a.Operands[4], b.Operands[4]);
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
                var regB = (RegisterOperand) opB;
                return CompareRegisters(regA.Register, regB.Register);
            }
            var immA = opA as ImmediateOperand;
            if (immA != null)
            {
                var immB = (ImmediateOperand)opB;
                return CompareValues(immA.Value, immB.Value);
            }
            var addrA = opA as AddressOperand;
            if (addrA != null)
            {
                var addrB = (AddressOperand)opB;
                return NormalizeConstants || addrA.Address == addrB.Address;
            }
            var memA = opA as MemoryOperand;
            if (memA != null)
            {
                var memB = (MemoryOperand)opB;
                return CompareRegisters(memA.BaseRegister, memB.BaseRegister) &&
                    CompareValues(memB.Offset, memB.Offset);
            }
            var cA = opA as ConditionOperand;
            if (cA != null)
            {
                var cB = (ConditionOperand)opB;
                return cA.condition == cB.condition;
            }
            throw new NotImplementedException(string.Format("PowerPC operand type {0} not implemented.", opA.GetType().Name));
        }

        public override int GetOperandsHash(MachineInstruction instr)
        {
            var i = (PowerPcInstruction)instr;
            return
                OperandHash(i.Operands[0]) * 23 ^
                OperandHash(i.Operands[1]) * 29 ^
                OperandHash(i.Operands[2]) * 9 ^
                OperandHash(i.Operands[3]) * 7 ^
                OperandHash(i.Operands[4]) * 3;
        }

        private int OperandHash(MachineOperand op)
        {
            if (op == null)
                return 0;
            int h = op.GetType().GetHashCode();
            var reg = op as RegisterOperand;
            if (reg != null)
            {
                if (!NormalizeRegisters)
                    h ^= GetRegisterHash(reg.Register);
                return h;
            }
            var imm = op as ImmediateOperand;
            if (imm != null)
            {
                if (!NormalizeConstants)
                    h ^= base.GetConstantHash(imm.Value);
                return h;
            }
            var addr = op as AddressOperand;
            if (addr != null)
            {
                if (!NormalizeConstants)
                    h ^= addr.Address.GetHashCode();
                return h;
            }
            var mem = op as MemoryOperand;
            if (mem != null)
            {
                if (!NormalizeRegisters)
                    h ^= GetRegisterHash(mem.BaseRegister);
                if (!NormalizeConstants)
                    h ^= GetConstantHash(mem.Offset);
                return h;
            }
            var c = op as ConditionOperand;
            if (c != null)
            {
                h ^= c.condition.GetHashCode();
                return h;
            }
            throw new NotImplementedException(string.Format("PowerPC operand type {0} not implemented.", op.GetType().Name));
        }
    }
}
