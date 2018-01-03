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

using Reko.Core;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.Microchip.PIC18
{
    /// <summary>
    /// Compares pairs of PIC18 instructions -- for equality only.
    /// </summary>
    /// <remarks>
    /// Used by the InstructionTrie class.
    /// </remarks>
    /// 
    public class PIC18InstructionComparer : InstructionComparer
    {
        public PIC18InstructionComparer(Normalize norm) 
            : base(norm)
        {
        }

        public bool CompareOperands(MachineOperand opA, MachineOperand opB)
        {
            if (opA.GetType() != opB.GetType())
                return false;

            RegisterOperand regOpA = opA as RegisterOperand;
            if (regOpA != null)
            {
                RegisterOperand regOpB = (RegisterOperand)opB;
                return NormalizeRegisters || regOpA.Register == regOpB.Register;
            }
            ImmediateOperand immOpA = opA as ImmediateOperand;
            if (immOpA != null)
            {
                var immOpB = (ImmediateOperand)opB;
                return NormalizeConstants || immOpA.Value.Equals(immOpB.Value);         // disregard immediate values.
            }
            var addrOpA = opA as AddressOperand;
            if (addrOpA != null)
            {
                var addrOpB = (AddressOperand)opB;
                return NormalizeConstants || addrOpA.Address == addrOpB.Address;
            }
            var memOpA = opA as MemoryOperand;
            if (memOpA != null)
            {
                var memOpB = (MemoryOperand)opB;
                if (!base.CompareRegisters(memOpA.Base, memOpB.Base))
                    return false;
                if (memOpA.Width != memOpB.Width)
                    return false;
                return true;
            }
            throw new NotImplementedException(string.Format("NYI: {0}", opA.GetType()));
        }

        /// <summary>
        /// Implementation of IComparer.Compare. In reality, 
        /// </summary>
        /// <param name="oInstrA"></param>
        /// <param name="oInstrB"></param>
        /// <returns></returns>
        public override bool CompareOperands(MachineInstruction a, MachineInstruction b)
        {
            var instrA = (PIC18Instruction)a;
            var instrB = (PIC18Instruction)b;

            if (instrA.Opcode != instrB.Opcode)
                return false;
            if (instrA.NumberOfOperands != instrB.NumberOfOperands)
                return false;

            bool retval = true;
            if (instrA.NumberOfOperands > 0)
            {
                retval = CompareOperands(instrA.op1, instrB.op1);
                if (retval && instrA.NumberOfOperands > 1)
                {
                    retval = CompareOperands(instrA.op2, instrB.op2);
                }
            }
            return retval;
        }

        public override int GetOperandsHash(MachineInstruction inst)
        {
            var instr = (PIC18Instruction)inst;
            int hash = instr.NumberOfOperands.GetHashCode();
            if (instr.NumberOfOperands > 0)
            {
                hash = hash * 23 + GetHashCode(instr.op1);
                if (instr.NumberOfOperands > 1)
                {
                    hash = hash * 17 + GetHashCode(instr.op2);
                }
            }
            return hash;
        }

        private int GetHashCode(MachineOperand op)
        {
            int h;
            RegisterOperand regOp = op as RegisterOperand;
            if (regOp != null)
            {
                return base.GetRegisterHash(regOp.Register);
            }
            ImmediateOperand immOp = op as ImmediateOperand;
            if (immOp != null)
            {
                return base.GetConstantHash(immOp.Value);
            }
            var addrOp = op as AddressOperand;
            if (addrOp != null)
            {
                return base.NormalizeConstants
                    ? 1
                    : addrOp.Address.GetHashCode();
            }
            var memOp = op as MemoryOperand;
            if (memOp != null)
            {
                h = 0;
                if (memOp.Base != null)
                {
                    h = base.GetRegisterHash(memOp.Base);
                }
                return h;
            }
            throw new NotImplementedException("Unhandled operand type: " + op.GetType().FullName);
        }

    }

}
