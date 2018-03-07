#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
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

            switch (opA)
            {
                case RegisterOperand regOpA:
                    var regOpB = (RegisterOperand)opB;
                    return NormalizeRegisters || regOpA.Register == regOpB.Register;

                case ImmediateOperand immOpA:
                    var immOpB = (ImmediateOperand)opB;
                    return NormalizeConstants || immOpA.Value.Equals(immOpB.Value);         // disregard immediate values.

                case AddressOperand addrOpA:
                    var addrOpB = (AddressOperand)opB;
                    return NormalizeConstants || addrOpA.Address == addrOpB.Address;

                case MemoryOperand memOpA:
                    var memOpB = (MemoryOperand)opB;
                    if (!base.CompareRegisters(memOpA.Base, memOpB.Base))
                        return false;
                    if (memOpA.Width != memOpB.Width)
                        return false;
                    if (memOpA.IsAccess != memOpB.IsAccess)
                        return false;
                    return true;

                case PseudoDataOperand pseudoA:
                    var pseudoB = (PseudoDataOperand)opB;
                    if (pseudoA.Width != pseudoB.Width)
                        return false;
                    if (pseudoA.Values.Length != pseudoB.Values.Length)
                        return false;
                    return NormalizeConstants || (pseudoA.Values == pseudoB.Values);
                    
                default:
                    throw new NotImplementedException($"NYI: {opA.GetType()}");
            }
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
            switch (op)
            {
                case RegisterOperand regOp:
                    return base.GetRegisterHash(regOp.Register);

                case ImmediateOperand immOp:
                    return base.GetConstantHash(immOp.Value);

                case AddressOperand addrOp:
                    return base.NormalizeConstants
                        ? 1
                        : addrOp.Address.GetHashCode();

                case MemoryOperand memOp:
                    return (memOp.Base != null
                        ? base.GetRegisterHash(memOp.Base)
                        : 0);

                case PseudoDataOperand pseudoOp:
                    return base.NormalizeConstants
                        ? 1
                        : pseudoOp.Values.GetHashCode();

                default:
                    throw new NotImplementedException("Unhandled operand type: " + op.GetType().FullName);
            }

        }

    }

}
