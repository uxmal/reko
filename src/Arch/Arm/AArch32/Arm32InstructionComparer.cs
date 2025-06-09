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

namespace Reko.Arch.Arm.AArch32
{
    public class Arm32InstructionComparer : InstructionComparer
    {
        private Normalize norm;

        public Arm32InstructionComparer(Normalize norm) : base(norm)
        {
            this.norm = norm;
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            return false;
            /*
            var b = (Arm32InstructionOld)y;
            var aInvalid = a.instruction is null;
            var bInvalid = b.instruction is null;

            // Follow the example of NaN.
            if (aInvalid != bInvalid)
                return false;
            if (aInvalid && bInvalid)
                return true;

            var aa = a.instruction.ArchitectureDetail;
            var bb = b.instruction.ArchitectureDetail;
            if (aa.Operands.Length != bb.Operands.Length)
                return false;
            for (int i = 0; i < aa.Operands.Length; ++i)
            {
                var aop = aa.Operands[i];
                var bop = bb.Operands[i];
                if (aop.Type != bop.Type)
                    return false;
                switch (aop.Type)
                {
                case ArmInstructionOperandType.Register:
                    if (!base.NormalizeRegisters && aop.RegisterValue.Value != bop.RegisterValue.Value)
                        return false;
                    break;
                case ArmInstructionOperandType.Immediate:
                    if (!base.NormalizeConstants && aop.ImmediateValue.Value != bop.ImmediateValue.Value)
                        return false;
                    break;
                case ArmInstructionOperandType.Memory:
                    if (!base.NormalizeRegisters)
                    {
                        if (aop.MemoryValue.BaseRegister != bop.MemoryValue.BaseRegister)
                            return false;
                        if (aop.MemoryValue.IndexRegister != bop.MemoryValue.IndexRegister)
                            return false;
                    }
                    if (!base.NormalizeConstants)
                    {
                        if (aop.MemoryValue.Displacement != bop.MemoryValue.Displacement)
                            return false;
                        if (aop.MemoryValue.IndexRegisterScale != bop.MemoryValue.IndexRegisterScale)
                            return false;
                    }
                    break;
                case ArmInstructionOperandType.CImmediate:
                case ArmInstructionOperandType.PImmediate:
                    if (!NormalizeConstants)
                    {
                        if (aop.ImmediateValue.Value != bop.ImmediateValue.Value)
                            return false;
                    }
                    break;
                default: throw new AddressCorrelatedException(
                    x.Address,
                    "ARM32 instruction comparer doesn't handle {0} yet.",
                    aop.Type);
                }
            }

            return true;
        */
        }

        public override int GetOperandsHash(MachineInstruction instr)
        {
            return 1;
            /*
            var arm = ((Arm32InstructionOld)instr).instruction;
            if (arm is null)
                return 0;
            var ops = arm.ArchitectureDetail.Operands;
            int hash = 0;
            for (int i = 0; i < ops.Length; ++i)
            {
                var op = ops[i];
                hash = hash * 23;
                switch (op.Type)
                {
                case ArmInstructionOperandType.Register:
                    if (!NormalizeRegisters)
                    {
                        hash ^= op.RegisterValue.Value.GetHashCode();
                    }
                    break;
                case ArmInstructionOperandType.Immediate:
                    if (!NormalizeConstants)
                    {
                        hash ^= op.ImmediateValue.Value.GetHashCode();
                    }
                    break;
                case ArmInstructionOperandType.Memory:
                    if (!NormalizeRegisters)
                    {
                        hash ^= op.MemoryValue.BaseRegister.GetHashCode();
                    }
                    hash *= 29;
                    if (!NormalizeConstants)
                    {
                        hash ^= op.MemoryValue.Displacement.GetHashCode();
                    }
                    break;
                case ArmInstructionOperandType.CImmediate:
                case ArmInstructionOperandType.PImmediate:
                    if (!NormalizeConstants)
                    {
                        hash ^= op.ImmediateValue.Value;
                    }
                    break;
                default:
                    throw new AddressCorrelatedException(
                          instr.Address,
                          "ARM32 instruction comparer doesn't handle {0} yet.",
                          op.Type);
                }
            }
            return hash;
            */
        }
    }
}